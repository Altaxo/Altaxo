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
  /// Implements chat against Ollama's native chat API and invokes Mcp tools from Altaxo.
  /// </summary>
  public partial record OllamaChatService : IChatService, Main.IImmutable
  {
    /// <summary>
    /// Gets the base Uri for the endpoint API.
    /// </summary>
    public Uri BaseUri { get; init; } = new Uri("http://localhost:11434");

    /// <summary>
    /// Gets the model identifier.
    /// </summary>
    public string Model { get; init; } = string.Empty;

    /// <summary>
    /// Gets an optional bearer token.
    /// </summary>
    public string ApiKey { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the context length for the chat session. A value of null means no limit.
    /// </summary>
    public int? ContextLength { get; init; }

    /// <summary>
    /// Gets or sets the temperature used when generating responses.
    /// </summary>
    public double? Temperature { get; init; }

    /// <summary>
    /// Gets or sets the maximum number of tokens to generate. A value of null uses Ollama's default.
    /// </summary>
    public int? MaximumNumberOfTokensToGenerate { get; init; }

    /// <summary>
    /// Gets or sets the maximum number of tool-call rounds allowed in a single chat session.
    /// </summary>
    public int MaximumNumberOfToolCallRounds { get; init; } = 1000;

    /// <summary>
    /// Controls a model's thinking output. True requests thinking, false requests no thinking output, and null uses the model default.
    /// </summary>
    public bool? Think { get; init; } = null;

    /// <summary>
    /// The keep alive time of the model. After not receiving a message for the designated time, the model will be unloaded.
    /// </summary>
    public DimensionfulQuantity KeepAliveTime { get; init; } = new DimensionfulQuantity(30, Units.Time.Minute.Instance);

    /// <summary>
    /// Time out time for receiving a response from the model.
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
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(OllamaChatService), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public virtual void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (OllamaChatService)o;
        info.AddValue("Endpoint", s.BaseUri.OriginalString);
        info.AddValue("Model", s.Model);
        info.AddValue("ApiKey", s.ApiKey);
        info.AddValue("ContextLength", s.ContextLength);
        info.AddValue("Temperature", s.Temperature);
        info.AddValue("MaximumNumberOfTokensToGenerate", s.MaximumNumberOfTokensToGenerate);
        info.AddValue("MaximumNumberOfToolCallRounds", s.MaximumNumberOfToolCallRounds);
        info.AddValue("Think", s.Think);
        info.AddValue("KeepAliveTime", s.KeepAliveTime);
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
        var contextLength = info.GetNullableInt32("ContextLength");
        var temperature = info.GetNullableDouble("Temperature");
        var maximumNumberOfTokensToGenerate = info.GetNullableInt32("MaximumNumberOfTokensToGenerate");
        var maximumNumberOfToolCallRounds = info.GetInt32("MaximumNumberOfToolCallRounds");
        var think = info.GetNullableBoolean("Think");
        var keepAliveTime = info.GetValue<DimensionfulQuantity>("KeepAliveTime", null);
        var messageTimeout = info.GetValue<DimensionfulQuantity>("MessageTimeout", null);
        var useStreaming = info.GetBoolean("UseStreaming");
        var logToConsole = info.GetBoolean("LogToConsole");

        return o is OllamaChatService ocs ?
            ocs with
            {
              BaseUri = endpoint,
              Model = model,
              ApiKey = apiKey,
              ContextLength = contextLength,
              Temperature = temperature,
              MaximumNumberOfTokensToGenerate = maximumNumberOfTokensToGenerate,
              MaximumNumberOfToolCallRounds = maximumNumberOfToolCallRounds,
              Think = think,
              KeepAliveTime = keepAliveTime,
              MessageTimeout = messageTimeout,
              UseStreaming = useStreaming,
              LogToConsole = logToConsole
            } :
            new OllamaChatService
            {
              BaseUri = endpoint,
              Model = model,
              ApiKey = apiKey,
              ContextLength = contextLength,
              Temperature = temperature,
              MaximumNumberOfTokensToGenerate = maximumNumberOfTokensToGenerate,
              MaximumNumberOfToolCallRounds = maximumNumberOfToolCallRounds,
              Think = think,
              KeepAliveTime = keepAliveTime,
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
      if (ContextLength < 0)
        throw new ArgumentOutOfRangeException(nameof(ContextLength), "The context length cannot be negative.");
      if (Temperature is { } t && (!double.IsFinite(t) || Temperature < 0))
        throw new ArgumentOutOfRangeException(nameof(Temperature), "Temperature must be a finite, non-negative value.");
      if (MaximumNumberOfTokensToGenerate < 0)
        throw new ArgumentOutOfRangeException(nameof(MaximumNumberOfTokensToGenerate), "The maximum number of tokens cannot be negative.");
      if (MaximumNumberOfToolCallRounds < 0)
        throw new ArgumentOutOfRangeException(nameof(MaximumNumberOfToolCallRounds), "The maximum number of tool-call rounds cannot be negative.");
      var messageTimeoutSeconds = MessageTimeout.AsValueIn(Units.Time.Second.Instance);
      if (!double.IsFinite(messageTimeoutSeconds) || messageTimeoutSeconds <= 0 || messageTimeoutSeconds > TimeSpan.MaxValue.TotalSeconds)
        throw new ArgumentOutOfRangeException(nameof(MessageTimeout), "The message timeout must be finite and greater than zero.");
      var keepAliveSeconds = KeepAliveTime.AsValueIn(Units.Time.Second.Instance);
      if (!double.IsFinite(keepAliveSeconds))
        throw new ArgumentOutOfRangeException(nameof(KeepAliveTime), "The keep-alive time must be finite.");

      if (UseStreaming)
        return new ChatSessionStreaming(new HttpClient { Timeout = TimeSpan.FromSeconds(messageTimeoutSeconds) }, this, McpServerHost.ConfiguredPipeName);
      else
        return new ChatSessionNonStreaming(new HttpClient { Timeout = TimeSpan.FromSeconds(messageTimeoutSeconds) }, this, McpServerHost.ConfiguredPipeName);
    }
  }
}
