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
  public partial record OllamaChatService
  {
    private sealed class ChatSessionStreaming : IChatSession, IAsyncDisposable
    {
      private readonly OllamaChatService _options;

      private readonly HttpClient _httpClient;
      private readonly string _pipeName;
      private readonly List<JsonObject> _conversation = new();
      private NamedPipeClientStream? _pipe;
      private McpClient? _mcpClient;
      private IReadOnlyDictionary<string, McpClientTool>? _tools;
      private bool _disposed;

      public ChatSessionStreaming(HttpClient httpClient, OllamaChatService options, string pipeName)
      {
        _httpClient = httpClient;
        _options = options;
        _pipeName = pipeName;
      }

      private Uri GetChatEndpoint()
      {
        var endpointBuilder = new UriBuilder(_options.BaseUri);
        if (endpointBuilder.Path.EndsWith("/v1", StringComparison.OrdinalIgnoreCase))
          endpointBuilder.Path = endpointBuilder.Path.Substring(0, endpointBuilder.Path.Length - 3);
        const string chatPath = "/api/chat";
        if (!endpointBuilder.Path.EndsWith(chatPath, StringComparison.OrdinalIgnoreCase))
          endpointBuilder.Path = $"{endpointBuilder.Path.TrimEnd('/')}{chatPath}";
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
        var userMessage = new JsonObject { ["role"] = "user", ["content"] = prompt };
        if (imageData is not null)
          userMessage["images"] = new JsonArray(Convert.ToBase64String(imageData));
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
        var requestBody = new JsonObject
        {
          ["model"] = _options.Model,
          ["messages"] = new JsonArray(_conversation.Select(item => item.DeepClone()).ToArray()),
          ["stream"] = true,
          ["keep_alive"] = FormatOllamaDuration(_options.KeepAliveTime.AsValueIn(Units.Time.Second.Instance))
        };

        if (_options.Think.HasValue)
        {
          requestBody["think"] = _options.Think.Value;
        }

        if (_options.Temperature.HasValue || _options.ContextLength.HasValue || _options.MaximumNumberOfTokensToGenerate.HasValue)
        {
          var options = new JsonObject();
          if (_options.Temperature.HasValue && _options.Temperature.Value > 0)
            options["temperature"] = _options.Temperature.Value;
          if (_options.ContextLength.HasValue)
            options["num_ctx"] = _options.ContextLength.Value;
          if (_options.MaximumNumberOfTokensToGenerate.HasValue)
            options["num_predict"] = _options.MaximumNumberOfTokensToGenerate.Value;
          requestBody["options"] = options;
        }

        if (_tools is { Count: > 0 })
        {
          var tools = new JsonArray();
          foreach (var tool in _tools.Values)
          {
            tools.Add(new JsonObject
            {
              ["type"] = "function",
              ["function"] = new JsonObject
              {
                ["name"] = tool.Name,
                ["description"] = tool.Description,
                ["parameters"] = JsonSerializer.SerializeToNode(tool.JsonSchema)
              }
            });
          }
          requestBody["tools"] = tools;
        }

        if (_options.LogToConsole)
        {
          Current.Console.WriteLine("Ollama chat request messages:\r\n{0}", requestBody.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, GetChatEndpoint()) { Content = JsonContent.Create(requestBody) };
        if (!string.IsNullOrWhiteSpace(_options.ApiKey))
          request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);

        using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
          var responseContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
          var detail = responseContent.Length <= 1000 ? responseContent : responseContent.Substring(0, 1000);
          throw new HttpRequestException($"The Ollama endpoint returned HTTP {(int)response.StatusCode} ({response.ReasonPhrase}). {detail}", null, response.StatusCode);
        }

        await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        using var reader = new StreamReader(responseStream);
        var text = new System.Text.StringBuilder();
        while (await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false) is { } line)
        {
          if (line.Length == 0)
            continue;
          using var document = JsonDocument.Parse(line);
          if (!document.RootElement.TryGetProperty("message", out var message))
            continue;

          if (message.TryGetProperty("content", out var content) && content.ValueKind == JsonValueKind.String)
          {
            var chunk = content.GetString() ?? string.Empty;
            if (chunk.Length > 0)
            {
              text.Append(chunk);
              yield return chunk;
            }
          }

          if (message.TryGetProperty("tool_calls", out var calls) && calls.ValueKind == JsonValueKind.Array)
          {
            foreach (var call in calls.EnumerateArray())
              assistantTurn.ToolCalls.Add(call.Clone());
          }

          assistantTurn.Message = JsonNode.Parse(message.GetRawText())!.AsObject();
          if (document.RootElement.TryGetProperty("done", out var done) && done.ValueKind == JsonValueKind.True)
            break;
        }
      }

      private sealed class AssistantTurn
      {
        public JsonObject Message { get; set; } = new() { ["role"] = "assistant", ["content"] = string.Empty };
        public List<JsonElement> ToolCalls { get; } = new();
      }

      private static string FormatOllamaDuration(double seconds)
      {
        if (seconds < 0)
          return "-1";
        return $"{seconds.ToString("G", System.Globalization.CultureInfo.InvariantCulture)}s";
      }

      private async Task<JsonObject> InvokeToolAsync(JsonElement toolCall, CancellationToken cancellationToken)
      {
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

        return new JsonObject { ["role"] = "tool", ["name"] = name, ["content"] = content };
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
        _disposed = true;

        _httpClient?.Dispose();

        if (_mcpClient is not null)
          await _mcpClient.DisposeAsync().ConfigureAwait(false);
        _pipe?.Dispose();
      }
    }
  }
}
