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
    private sealed class ChatSessionNonStreaming : IChatSession, IAsyncDisposable
    {
      private readonly HttpClient _httpClient;
      private readonly OpenAiCompatibleChatService _options;
      private readonly string _pipeName;
      private readonly List<JsonObject> _conversation = new();
      private NamedPipeClientStream? _pipe;
      private McpClient? _mcpClient;
      private IReadOnlyDictionary<string, McpClientTool>? _tools;
      private bool _disposed;

      public ChatSessionNonStreaming(HttpClient httpClient, OpenAiCompatibleChatService options, string pipeName)
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

      public async Task<string> SendAsync(string prompt, CancellationToken cancellationToken, byte[]? imageData = null, string? imageMediaType = null)
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
          var responseMessage = await GetAssistantMessageAsync(cancellationToken).ConfigureAwait(false);
          _conversation.Add(JsonNode.Parse(responseMessage.GetRawText())!.AsObject());

          if (!responseMessage.TryGetProperty("tool_calls", out var toolCalls) || toolCalls.ValueKind != JsonValueKind.Array || toolCalls.GetArrayLength() == 0)
            return GetAssistantText(responseMessage);

          if (++toolCallRounds > _options.MaximumNumberOfToolCallRounds)
            throw new InvalidOperationException($"The assistant exceeded the limit of {_options.MaximumNumberOfToolCallRounds} tool-call rounds.");

          foreach (var toolCall in toolCalls.EnumerateArray())
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

      private async Task<JsonElement> GetAssistantMessageAsync(CancellationToken cancellationToken)
      {
        var requestBody = new JsonObject { ["model"] = _options.Model };
        var messages = new JsonArray();
        foreach (var item in _conversation)
          messages.Add(item.DeepClone());
        requestBody["messages"] = messages;
        requestBody["temperature"] = _options.Temperature;
        if (_options.MaximumNumberOfTokensToGenerate > 0)
          requestBody["max_tokens"] = _options.MaximumNumberOfTokensToGenerate;

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

        using var request = new HttpRequestMessage(HttpMethod.Post, GetChatCompletionsEndpoint())
        {
          Content = JsonContent.Create(requestBody)
        };
        if (!string.IsNullOrWhiteSpace(_options.ApiKey))
          request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);

        using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
          var detail = responseContent.Length <= 1000 ? responseContent : responseContent.Substring(0, 1000);
          throw new HttpRequestException($"The LLM endpoint returned HTTP {(int)response.StatusCode} ({response.ReasonPhrase}). {detail}", null, response.StatusCode);
        }

        using var document = JsonDocument.Parse(responseContent);
        if (!document.RootElement.TryGetProperty("choices", out var choices) || choices.GetArrayLength() == 0 || !choices[0].TryGetProperty("message", out var message))
          throw new InvalidOperationException("The LLM endpoint response did not contain a chat-completions message.");

        return message.Clone();
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

      public async IAsyncEnumerable<string> StreamAsync(string prompt, [EnumeratorCancellation] CancellationToken cancellationToken, byte[]? imageData = null, string? imageMediaType = null)
      {
        var response = await SendAsync(prompt, cancellationToken, imageData, imageMediaType).ConfigureAwait(false);
        if (response.Length > 0)
          yield return response;
      }
    }
  }
}
