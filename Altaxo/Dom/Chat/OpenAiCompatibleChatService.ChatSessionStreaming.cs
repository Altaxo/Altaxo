#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

namespace Altaxo.Chat
{
  public partial record OpenAiCompatibleChatService
  {
    private sealed class ChatSessionStreaming : IChatSession, IAsyncDisposable
    {
      private readonly HttpClient _httpClient;
      private readonly OpenAiCompatibleChatService _options;
      private readonly string _pipeName;
      private readonly List<JsonObject> _conversation = new();
      private NamedPipeClientStream? _pipe;
      private McpClient? _mcpClient;
      private IReadOnlyDictionary<string, McpClientTool>? _tools;
      private bool _disposed;

      public ChatSessionStreaming(HttpClient httpClient, OpenAiCompatibleChatService options, string pipeName)
      {
        _httpClient = httpClient;
        _options = options;
        _pipeName = pipeName;
      }

      private Uri GetChatCompletionsEndpoint()
      {
        var endpointBuilder = new UriBuilder(_options.BaseUri);
        const string chatCompletionsPath = "/chat/completions";
        if (!endpointBuilder.Path.EndsWith(chatCompletionsPath, StringComparison.OrdinalIgnoreCase))
          endpointBuilder.Path = $"{endpointBuilder.Path.TrimEnd('/')}{chatCompletionsPath}";

        return endpointBuilder.Uri;
      }

      public async IAsyncEnumerable<string> StreamAsync(string prompt, [EnumeratorCancellation] CancellationToken cancellationToken, byte[]? imageData = null, string? imageMediaType = null)
      {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (string.IsNullOrWhiteSpace(prompt) && imageData is null)
          throw new ArgumentException("A prompt or image attachment is required.", nameof(prompt));
        if (imageData is { Length: 0 })
          throw new ArgumentException("The image attachment cannot be empty.", nameof(imageData));
        if (imageData is null && imageMediaType is not null)
          throw new ArgumentException("An image media type cannot be specified without image data.", nameof(imageMediaType));
        if (imageData is not null && imageMediaType is not ("image/png" or "image/jpeg" or "image/gif" or "image/webp"))
          throw new ArgumentException("The image attachment must be PNG, JPEG, GIF, or WebP.", nameof(imageMediaType));

        await EnsureMcpClientAsync(cancellationToken).ConfigureAwait(false);
        var userMessage = new JsonObject { ["role"] = "user" };
        if (imageData is null)
        {
          userMessage["content"] = prompt;
        }
        else
        {
          var content = new JsonArray();
          if (!string.IsNullOrWhiteSpace(prompt))
            content.Add(new JsonObject { ["type"] = "text", ["text"] = prompt });
          content.Add(new JsonObject
          {
            ["type"] = "image_url",
            ["image_url"] = new JsonObject
            {
              ["url"] = $"data:{imageMediaType};base64,{Convert.ToBase64String(imageData)}"
            }
          });
          userMessage["content"] = content;
        }
        _conversation.Add(userMessage);

        var toolCallRounds = 0;
        while (true)
        {
          var assistantTurn = new AssistantTurn();
          await foreach (var chunk in GetAssistantTextChunksAsync(assistantTurn, cancellationToken).ConfigureAwait(false))
            yield return chunk;
          _conversation.Add(assistantTurn.Message);

          if (assistantTurn.ToolCalls.Count == 0)
            yield break;

          if (++toolCallRounds > _options.MaximumNumberOfToolCallRounds)
            throw new InvalidOperationException($"The assistant exceeded the limit of {_options.MaximumNumberOfToolCallRounds} tool-call rounds.");

          foreach (var toolCall in assistantTurn.ToolCalls)
          {
            cancellationToken.ThrowIfCancellationRequested();
            var result = await InvokeToolAsync(toolCall, cancellationToken).ConfigureAwait(false);
            _conversation.Add(result);
          }
        }
      }

      private async Task EnsureMcpClientAsync(CancellationToken cancellationToken)
      {
        if (_mcpClient is not null)
          return;

        var pipe = new NamedPipeClientStream(".", _pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
        try
        {
          await pipe.ConnectAsync(5000, cancellationToken).ConfigureAwait(false);
          var transport = new StreamClientTransport(pipe, pipe, NullLoggerFactory.Instance);
          _mcpClient = await McpClient.CreateAsync(transport, null, NullLoggerFactory.Instance, cancellationToken).ConfigureAwait(false);
          var tools = await _mcpClient.ListToolsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
          _tools = tools.ToDictionary(tool => tool.Name, StringComparer.Ordinal);
          _pipe = pipe;
        }
        catch
        {
          pipe.Dispose();
          throw;
        }
      }

      private async IAsyncEnumerable<string> GetAssistantTextChunksAsync(AssistantTurn assistantTurn, [EnumeratorCancellation] CancellationToken cancellationToken)
      {
        var requestBody = new JsonObject { ["model"] = _options.Model };
        var messages = new JsonArray();
        foreach (var item in _conversation)
          messages.Add(item.DeepClone());
        requestBody["messages"] = messages;
        requestBody["stream"] = true;
        if (_options.Temperature.HasValue && _options.Temperature.Value > 0)
          requestBody["temperature"] = _options.Temperature.Value;
        if (_options.MaximumNumberOfTokensToGenerate.HasValue && _options.MaximumNumberOfTokensToGenerate.Value > 0)
          requestBody["max_tokens"] = _options.MaximumNumberOfTokensToGenerate.Value;

        if (_tools is { Count: > 0 })
        {
          var tools = new JsonArray();
          foreach (var tool in _tools.Values)
          {
            var function = new JsonObject
            {
              ["name"] = tool.Name,
              ["description"] = tool.Description,
              ["parameters"] = JsonSerializer.SerializeToNode(tool.JsonSchema)
            };
            tools.Add(new JsonObject { ["type"] = "function", ["function"] = function });
          }
          requestBody["tools"] = tools;
          requestBody["tool_choice"] = "auto";
        }
        if (_options.LogToConsole)
        {
          Current.Console.WriteLine("OpenAI-compatible chat request messages:\r\n{0}", requestBody.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, GetChatCompletionsEndpoint())
        {
          Content = JsonContent.Create(requestBody)
        };
        if (!string.IsNullOrWhiteSpace(_options.ApiKey))
          request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);

        using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
          var responseContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
          var detail = responseContent.Length <= 1000 ? responseContent : responseContent.Substring(0, 1000);
          throw new HttpRequestException($"The LLM endpoint returned HTTP {(int)response.StatusCode} ({response.ReasonPhrase}). {detail}", null, response.StatusCode);
        }

        await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        using var reader = new StreamReader(responseStream);
        var text = new System.Text.StringBuilder();
        var toolCalls = new SortedDictionary<int, ToolCallBuilder>();
        var done = false;
        while (!done)
        {
          var line = await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false);
          if (line is null)
            break;
          if (!line.StartsWith("data:", StringComparison.Ordinal))
            continue;

          var data = line.AsSpan(5).Trim().ToString();
          if (data == "[DONE]")
            break;
          if (data.Length == 0)
            continue;

          using var document = JsonDocument.Parse(data);
          if (!document.RootElement.TryGetProperty("choices", out var choices) || choices.ValueKind != JsonValueKind.Array || choices.GetArrayLength() == 0)
            continue;
          var choice = choices[0];
          if (choice.TryGetProperty("delta", out var delta))
          {
            if (delta.TryGetProperty("content", out var content) && content.ValueKind == JsonValueKind.String)
            {
              var chunk = content.GetString() ?? string.Empty;
              if (chunk.Length > 0)
              {
                text.Append(chunk);
                yield return chunk;
              }
            }
            if (delta.TryGetProperty("tool_calls", out var calls) && calls.ValueKind == JsonValueKind.Array)
            {
              foreach (var call in calls.EnumerateArray())
              {
                var index = call.GetProperty("index").GetInt32();
                if (!toolCalls.TryGetValue(index, out var builder))
                  toolCalls.Add(index, builder = new ToolCallBuilder());
                builder.Append(call);
              }
            }
          }
          if (choice.TryGetProperty("finish_reason", out var finishReason) && finishReason.ValueKind == JsonValueKind.String && finishReason.GetString() is not null)
            done = true;
        }

        assistantTurn.ToolCalls.AddRange(toolCalls.Values.Select(builder => builder.Build()));
        assistantTurn.Message = new JsonObject
        {
          ["role"] = "assistant",
          ["content"] = text.ToString()
        };
        if (assistantTurn.ToolCalls.Count > 0)
        {
          var calls = new JsonArray();
          foreach (var call in assistantTurn.ToolCalls)
            calls.Add(JsonNode.Parse(call.GetRawText()));
          assistantTurn.Message["tool_calls"] = calls;
        }
      }

      private sealed class AssistantTurn
      {
        public JsonObject Message { get; set; } = new();
        public List<JsonElement> ToolCalls { get; } = new();
      }

      private sealed class ToolCallBuilder
      {
        private string? _id;
        private string? _type;
        private string? _name;
        private readonly System.Text.StringBuilder _arguments = new();

        public void Append(JsonElement delta)
        {
          if (delta.TryGetProperty("id", out var id))
            _id = id.GetString();
          if (delta.TryGetProperty("type", out var type))
            _type = type.GetString();
          if (delta.TryGetProperty("function", out var function))
          {
            if (function.TryGetProperty("name", out var name))
              _name = (_name ?? string.Empty) + (name.GetString() ?? string.Empty);
            if (function.TryGetProperty("arguments", out var arguments) && arguments.ValueKind == JsonValueKind.String)
              _arguments.Append(arguments.GetString());
          }
        }

        public JsonElement Build()
        {
          using var document = JsonDocument.Parse(JsonSerializer.Serialize(new
          {
            id = _id,
            type = _type ?? "function",
            function = new { name = _name, arguments = _arguments.ToString() }
          }));
          return document.RootElement.Clone();
        }
      }

      private async Task<JsonObject> InvokeToolAsync(JsonElement toolCall, CancellationToken cancellationToken)
      {
        var id = toolCall.TryGetProperty("id", out var idElement) ? idElement.GetString() : null;
        if (string.IsNullOrWhiteSpace(id))
          throw new InvalidOperationException("The LLM endpoint returned a tool call without an identifier.");

        var function = toolCall.GetProperty("function");
        var name = function.GetProperty("name").GetString();
        var argumentsElement = function.GetProperty("arguments");
        var argumentsText = argumentsElement.ValueKind == JsonValueKind.String ? argumentsElement.GetString()! : argumentsElement.GetRawText();
        string content;

        if (string.IsNullOrWhiteSpace(name) || _tools is null || !_tools.ContainsKey(name))
        {
          content = JsonSerializer.Serialize(new { error = "The requested tool is not available." });
        }
        else
        {
          try
          {
            var arguments = JsonSerializer.Deserialize<Dictionary<string, object?>>(argumentsText) ?? new Dictionary<string, object?>();
            var result = await _mcpClient!.CallToolAsync(name, arguments, cancellationToken: cancellationToken).ConfigureAwait(false);
            content = JsonSerializer.Serialize(result);
          }
          catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
          {
            throw;
          }
          catch (Exception ex)
          {
            content = JsonSerializer.Serialize(new { error = ex.Message });
          }
        }

        return new JsonObject
        {
          ["role"] = "tool",
          ["tool_call_id"] = id,
          ["name"] = name,
          ["content"] = content
        };
      }

      private static string GetAssistantText(JsonElement message)
      {
        if (!message.TryGetProperty("content", out var content) || content.ValueKind == JsonValueKind.Null)
          return string.Empty;
        return content.ValueKind == JsonValueKind.String ? content.GetString() ?? string.Empty : content.GetRawText();
      }

      public async ValueTask DisposeAsync()
      {
        if (_disposed)
          return;

        _httpClient?.Dispose();

        _disposed = true;
        if (_mcpClient is not null)
          await _mcpClient.DisposeAsync().ConfigureAwait(false);
        _pipe?.Dispose();
      }
    }
  }
}
