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
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Altaxo.Chat;
using Altaxo.Units;

namespace Altaxo.Gui.Chat
{
  /// <summary>
  /// Defines the view used to edit a chat service instance.
  /// </summary>
  public interface IOpenAiCompatibleChatServiceView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Edits a chat service instance.
  /// </summary>
  [ExpectedTypeOfView(typeof(IOpenAiCompatibleChatServiceView))]
  [UserControllerForObject(typeof(OpenAiCompatibleChatService))]
  public class OpenAiCompatibleChatServiceController : MVCANControllerEditImmutableDocBase<OpenAiCompatibleChatService, IOpenAiCompatibleChatServiceView>
  {
    private CancellationTokenSource? _modelLoadingCancellation;

    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    /// <summary>
    /// Gets or sets the endpoint URI text.
    /// </summary>
    public string BaseUri
    {
      get => field;
      set
      {
        if (field != value)
        {
          field = value;
          OnPropertyChanged(nameof(BaseUri));
          _ = LoadModelsAsync(value);
        }
      }
    }

    /// <summary>
    /// Gets the unit environment for time quantities.
    /// </summary>
    public QuantityWithUnitGuiEnvironment EnvironmentForTime => TimeEnvironment.Instance;


    /// <summary>
    /// Gets or sets the maximum time to wait for a response.
    /// </summary>
    public DimensionfulQuantity MessageTimeout
    {
      get => field;
      set
      {
        if (field != value)
        {
          field = value;
          OnPropertyChanged(nameof(MessageTimeout));
        }
      }
    }

    /// <summary>
    /// Gets model identifiers discovered at the configured endpoint.
    /// </summary>
    public ObservableCollection<string> Models { get; } = new();

    /// <inheritdoc/>
    public override void Dispose(bool isDisposing)
    {
      if (isDisposing)
      {
        _modelLoadingCancellation?.Cancel();
        _modelLoadingCancellation?.Dispose();
        _modelLoadingCancellation = null;
      }
      base.Dispose(isDisposing);
    }

    /// <summary>
    /// Gets or sets the model identifier.
    /// </summary>
    public string Model
    {
      get => field;
      set
      {
        if (field != value)
        {
          field = value;
          OnPropertyChanged(nameof(Model));
        }
      }
    }


    /// <summary>
    /// Gets or sets the API key.
    /// </summary>
    public string ApiKey
    {
      get => field;
      set
      {
        if (field != value)
        {
          field = value;
          OnPropertyChanged(nameof(ApiKey));
          _ = LoadModelsAsync(BaseUri);
        }
      }
    }


    /// <summary>
    /// Gets or sets the generation temperature.
    /// </summary>
    public double Temperature
    {
      get => field;
      set
      {
        if (field != value)
        {
          field = value;
          OnPropertyChanged(nameof(Temperature));
        }
      }
    }

    /// <summary>
    /// Gets or sets whether the temperature is enabled. If false, the temperature is ignored and the default value is used.
    /// </summary>
    public bool IsTemperatureEnabled
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(IsTemperatureEnabled));
        }
      }
    }


    /// <summary>
    /// Gets or sets the maximum number of tokens to generate.
    /// </summary>
    public int MaximumNumberOfTokensToGenerate
    {
      get => field;
      set
      {
        if (field != value)
        {
          field = value;
          OnPropertyChanged(nameof(MaximumNumberOfTokensToGenerate));
        }
      }
    }

    /// <summary>
    /// Gets or sets whether the maximum number of tokens to generate is enabled. If false, the maximum number of tokens to generate is ignored and the default value is used.
    /// </summary>
    public bool IsMaximumNumberOfTokensToGenerateEnabled
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(IsMaximumNumberOfTokensToGenerateEnabled));
        }
      }
    }


    /// <summary>
    /// Gets or sets the maximum number of tool call rounds.
    /// </summary>
    public int MaximumNumberOfToolCallRounds
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(MaximumNumberOfToolCallRounds));
        }
      }
    }

    /// <summary>
    /// Gets or sets whether to use streaming for receiving responses. If false, the entire response is received at once.
    /// </summary>
    public bool UseStreaming
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(UseStreaming));
        }
      }
    }

    /// <summary>
    /// Gets or sets whether to log messages to the console. This is useful for debugging and development purposes.
    /// </summary>
    public bool LogToConsole
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(LogToConsole));
        }
      }
    }

    #endregion

    private async Task LoadModelsAsync(string baseUriText)
    {
      _modelLoadingCancellation?.Cancel();
      _modelLoadingCancellation?.Dispose();
      var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(5));
      _modelLoadingCancellation = cancellation;

      if (!Uri.TryCreate(baseUriText, UriKind.Absolute, out var baseUri) || (baseUri.Scheme != Uri.UriSchemeHttp && baseUri.Scheme != Uri.UriSchemeHttps))
      {
        Models.Clear();
        return;
      }

      try
      {
        var endpointBuilder = new UriBuilder(baseUri);
        endpointBuilder.Path = endpointBuilder.Path.TrimEnd('/');
        if (!endpointBuilder.Path.EndsWith("/models", StringComparison.OrdinalIgnoreCase))
          endpointBuilder.Path += "/models";

        using var httpClient = new HttpClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, endpointBuilder.Uri);
        if (!string.IsNullOrWhiteSpace(ApiKey))
          request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);
        using var response = await httpClient.SendAsync(request, cancellation.Token);
        response.EnsureSuccessStatusCode();
        await using var responseStream = await response.Content.ReadAsStreamAsync(cancellation.Token);
        using var document = await JsonDocument.ParseAsync(responseStream, cancellationToken: cancellation.Token);

        if (cancellation.IsCancellationRequested || !ReferenceEquals(_modelLoadingCancellation, cancellation))
          return;

        var modelNames = new List<string>();
        if (document.RootElement.TryGetProperty("data", out var models) && models.ValueKind == JsonValueKind.Array)
        {
          foreach (var model in models.EnumerateArray())
          {
            if (model.TryGetProperty("id", out var id) && id.ValueKind == JsonValueKind.String && !string.IsNullOrWhiteSpace(id.GetString()))
              modelNames.Add(id.GetString()!);
          }
        }

        Models.Clear();
        foreach (var modelName in modelNames)
          Models.Add(modelName);
      }
      catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
      {
      }
      catch (HttpRequestException)
      {
      }
      catch (JsonException)
      {
      }
    }


    /// <inheritdoc/>
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        BaseUri = _doc.BaseUri.OriginalString;
        Model = _doc.Model;
        ApiKey = _doc.ApiKey;
        IsTemperatureEnabled = _doc.Temperature.HasValue;
        Temperature = _doc.Temperature ?? 0;
        IsMaximumNumberOfTokensToGenerateEnabled = _doc.MaximumNumberOfTokensToGenerate.HasValue;
        MaximumNumberOfTokensToGenerate = _doc.MaximumNumberOfTokensToGenerate ?? 0;
        MaximumNumberOfToolCallRounds = _doc.MaximumNumberOfToolCallRounds;
        MessageTimeout = _doc.MessageTimeout;
        UseStreaming = _doc.UseStreaming;
        LogToConsole = _doc.LogToConsole;
      }

    }
    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      if (!Uri.TryCreate(BaseUri, UriKind.Absolute, out var baseUri))
      {
        Current.Gui.ErrorMessageBox("The endpoint must be an absolute HTTP or HTTPS URI.");
        return ApplyEnd(false, disposeController);
      }

      var timeoutInSeconds = MessageTimeout.AsValueIn(Altaxo.Units.Time.Second.Instance);
      if (!double.IsFinite(timeoutInSeconds) || timeoutInSeconds <= 0 || timeoutInSeconds > TimeSpan.MaxValue.TotalSeconds)
      {
        Current.Gui.ErrorMessageBox("The message timeout must be finite and greater than zero.");
        return ApplyEnd(false, disposeController);
      }

      _doc = _doc with
      {
        BaseUri = baseUri,
        Model = Model,
        ApiKey = ApiKey,
        Temperature = Temperature,
        MaximumNumberOfTokensToGenerate = MaximumNumberOfTokensToGenerate,
        MaximumNumberOfToolCallRounds = MaximumNumberOfToolCallRounds,
        MessageTimeout = MessageTimeout,
        UseStreaming = UseStreaming,
        LogToConsole = LogToConsole
      };


      return ApplyEnd(true, disposeController);
    }
  }
}
