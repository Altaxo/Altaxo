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
using System.Net.Http;
using Altaxo.Units;

namespace Altaxo.Chat
{
  /// <summary>
  /// Implements chat completions against an OpenAI-compatible endpoint and invokes Mcp tools from Altaxo.
  /// </summary>
  public partial record OpenAiCompatibleChatService : IChatService, Main.IImmutable
  {
    /// <summary>
    /// Gets the base Uri for the endpoint API.
    /// </summary>
    public Uri BaseUri { get; init; } = new Uri("http://localhost:1234/v1");

    /// <summary>
    /// Gets the model identifier.
    /// </summary>
    public string Model { get; init; } = string.Empty;

    /// <summary>
    /// Gets an optional bearer token.
    /// </summary>
    public string ApiKey { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the temperature used when generating responses.
    /// </summary>
    public double? Temperature { get; init; }

    /// <summary>
    /// Gets or sets the maximum number of tokens to generate. A value of 0 uses Ollama's default.
    /// </summary>
    public int? MaximumNumberOfTokensToGenerate { get; init; }

    /// <summary>
    /// Gets or sets the maximum number of tool-call rounds allowed in a single chat session.
    /// </summary>
    public int MaximumNumberOfToolCallRounds { get; init; } = 1000;

    /// <summary>
    /// Gets or sets the maximum time to wait for a response from the endpoint.
    /// </summary>
    public DimensionfulQuantity MessageTimeout { get; init; } = new DimensionfulQuantity(60, Units.Time.Minute.Instance);

    /// <summary>
    /// Gets or sets whether to use streaming for receiving responses. If false, the entire response is received at once.
    /// </summary>
    public bool UseStreaming { get; init; }

    /// <summary>
    /// Gets or sets whether to log messages sent to and received from the LLM to Altaxo's console.
    /// </summary>
    public bool LogToConsole { get; init; }

    #region Serialization

    /// <summary>
    /// V0: 2026-10-06 initial version.
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(OpenAiCompatibleChatService), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public virtual void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (OpenAiCompatibleChatService)o;
        info.AddValue("Endpoint", s.BaseUri.OriginalString);
        info.AddValue("Model", s.Model);
        info.AddValue("ApiKey", s.ApiKey);
        info.AddValue("Temperature", s.Temperature);
        info.AddValue("MaximumNumberOfTokensToGenerate", s.MaximumNumberOfTokensToGenerate);
        info.AddValue("MaximumNumberOfToolCallRounds", s.MaximumNumberOfToolCallRounds);
        info.AddValue("MessageTimeout", s.MessageTimeout);
        info.AddValue("UseStreaming", s.UseStreaming);
        info.AddValue("LogToConsole", s.LogToConsole);
      }

      /// <inheritdoc/>
      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var endpoint = new Uri(info.GetString("Endpoint"));
        var model = info.GetString("Model");
        var apiKey = info.GetString("ApiKey");
        var temperature = info.GetNullableDouble("Temperature");
        var maximumNumberOfTokensToGenerate = info.GetNullableInt32("MaximumNumberOfTokensToGenerate");
        var maximumNumberOfToolCallRounds = info.GetInt32("MaximumNumberOfToolCallRounds");
        var messageTimeout = info.GetValue<DimensionfulQuantity>("MessageTimeout", null);
        var useStreaming = info.GetBoolean("UseStreaming");
        var logToConsole = info.GetBoolean("LogToConsole");

        return o is OpenAiCompatibleChatService ocs ?
            ocs with
            {
              BaseUri = endpoint,
              Model = model,
              ApiKey = apiKey,
              Temperature = temperature,
              MaximumNumberOfTokensToGenerate = maximumNumberOfTokensToGenerate,
              MaximumNumberOfToolCallRounds = maximumNumberOfToolCallRounds,
              MessageTimeout = messageTimeout,
              UseStreaming = useStreaming,
              LogToConsole = logToConsole
            } :
            new OpenAiCompatibleChatService
            {
              BaseUri = endpoint,
              Model = model,
              ApiKey = apiKey,
              Temperature = temperature,
              MaximumNumberOfTokensToGenerate = maximumNumberOfTokensToGenerate,
              MaximumNumberOfToolCallRounds = maximumNumberOfToolCallRounds,
              MessageTimeout = messageTimeout,
              UseStreaming = useStreaming,
              LogToConsole = logToConsole
            };
      }
    }

    #endregion Serialization


    /// <summary>
    /// Creates an independent chat session using the configured endpoint.
    /// </summary>
    /// <returns>A new chat session.</returns>
    /// <exception cref="ObjectDisposedException">The service has been disposed.</exception>
    public IChatSession CreateSession()
    {
      var timeoutInSeconds = MessageTimeout.AsValueIn(Units.Time.Second.Instance);
      if (!double.IsFinite(timeoutInSeconds) || timeoutInSeconds <= 0 || timeoutInSeconds > TimeSpan.MaxValue.TotalSeconds)
        throw new ArgumentOutOfRangeException(nameof(MessageTimeout), "The message timeout must be finite and greater than zero.");

      if (UseStreaming)
        return new ChatSessionStreaming(new HttpClient { Timeout = TimeSpan.FromSeconds(timeoutInSeconds) }, this, McpServerHost.ConfiguredPipeName);
      else
        return new ChatSessionNonStreaming(new HttpClient { Timeout = TimeSpan.FromSeconds(timeoutInSeconds) }, this, McpServerHost.ConfiguredPipeName);
    }
  }
}
