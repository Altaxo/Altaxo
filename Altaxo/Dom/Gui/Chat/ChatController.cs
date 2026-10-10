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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Altaxo.Chat;
using Altaxo.Gui.Workbench;
using Altaxo.Main.Services;

namespace Altaxo.Gui.Chat
{
  /// <summary>
  /// Defines the view used by the Mcp chat pad.
  /// </summary>
  public interface IChatView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Displays one message in the Mcp chat conversation.
  /// </summary>
  public sealed class McpChatMessageViewModel : INotifyPropertyChanged
  {
    private readonly StringBuilder _text = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="McpChatMessageViewModel"/> class.
    /// </summary>
    /// <param name="role">The message role.</param>
    /// <param name="text">The message text.</param>
    /// <param name="attachmentName">The optional name of an image attached to the message.</param>
    public McpChatMessageViewModel(string role, string text, string? attachmentName = null)
    {
      Role = role;
      _text.Append(text);
      AttachmentName = attachmentName;
    }

    /// <summary>
    /// Gets the message role.
    /// </summary>
    public string Role { get; }

    /// <summary>
    /// Gets the message text.
    /// </summary>
    public string Text => _text.ToString();

    /// <summary>
    /// Occurs when a message property changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Appends a streamed text chunk.
    /// </summary>
    /// <param name="text">The text chunk.</param>
    internal void AppendText(string text)
    {
      _text.Append(text);
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Text)));
    }

    /// <summary>
    /// Gets the name of the image attached to the message, if any.
    /// </summary>
    public string? AttachmentName { get; }
  }

  /// <summary>
  /// Provides the view model and conversation behavior for the Mcp chat pad.
  /// </summary>
  [ExpectedTypeOfView(typeof(IChatView))]
  public sealed class ChatController : AbstractPadContent
  {
    private IChatView? _view;
    private IChatSession? _session;
    private IChatService? _chatServiceConfiguration;
    private CancellationTokenSource? _activeRequest;
    private string _inputText = string.Empty;
    private string _statusText = "Ready";
    private bool _isBusy;
    private bool _disposed;
    private byte[]? _pendingImageData;
    private string? _pendingImageMediaType;
    private string? _pendingImageName;
    private const int MaximumImageSizeInBytes = 20 * 1024 * 1024;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatController"/> class.
    /// </summary>
    public ChatController()
    {
    }

    /// <summary>
    /// Gets the messages displayed in the conversation.
    /// </summary>
    public ObservableCollection<McpChatMessageViewModel> Messages { get; } = new ObservableCollection<McpChatMessageViewModel>();

    /// <summary>
    /// Gets or sets the text currently entered by the user.
    /// </summary>
    public string InputText
    {
      get => _inputText;
      set
      {
        if (_inputText != value)
        {
          _inputText = value;
          OnPropertyChanged(nameof(InputText));
          CmdSend.OnCanExecuteChanged();
        }
      }
    }

    /// <summary>
    /// Gets a value indicating whether a request is in progress.
    /// </summary>
    public bool IsBusy
    {
      get => _isBusy;
      private set
      {
        if (_isBusy != value)
        {
          _isBusy = value;
          OnPropertyChanged(nameof(IsBusy));
          CmdSend.OnCanExecuteChanged();
          CmdCancel.OnCanExecuteChanged();
          CmdAttachImage.OnCanExecuteChanged();
          CmdRemoveImage.OnCanExecuteChanged();
        }
      }
    }

    /// <summary>
    /// Gets the current activity or error message.
    /// </summary>
    public string StatusText
    {
      get => _statusText;
      private set
      {
        if (_statusText != value)
        {
          _statusText = value;
          OnPropertyChanged(nameof(StatusText));
        }
      }
    }

    /// <summary>
    /// Gets the command used to submit a prompt.
    /// </summary>
    public RelayCommand CmdSend => field ??= new RelayCommand(() => _ = SendAsync(), () => !IsBusy && (!string.IsNullOrWhiteSpace(InputText) || _pendingImageData is not null));

    /// <summary>
    /// Gets the command used to cancel an active request.
    /// </summary>
    public RelayCommand CmdCancel => field ??= new RelayCommand(() => _activeRequest?.Cancel(), () => IsBusy);

    /// <summary>
    /// Gets the command used to show the chat service settings dialog.
    /// </summary>
    public RelayCommand CmdShowSettings => field ??= new RelayCommand(ShowChatServiceSettingsDialog, () => !IsBusy);



    /// <summary>
    /// Gets the command used to attach an image from a file.
    /// </summary>
    public RelayCommand CmdAttachImage => field ??= new RelayCommand(AttachImageFromFile, () => !IsBusy);

    /// <summary>
    /// Gets the command used to remove the pending image.
    /// </summary>
    public RelayCommand CmdRemoveImage => field ??= new RelayCommand(ClearPendingImage, () => !IsBusy && _pendingImageData is not null);


    /// <summary>
    /// Gets the name of the image currently attached to the unsent message, if any.
    /// </summary>
    public string? PendingImageName => _pendingImageName;

    /// <summary>
    /// Attaches an image copied to the clipboard. Clipboard images are encoded as PNG.
    /// </summary>
    /// <param name="imageData">The encoded PNG image data.</param>
    /// <exception cref="ArgumentNullException"><paramref name="imageData"/> is <see langword="null"/>.</exception>
    public void AttachClipboardImage(byte[] imageData)
    {
      ArgumentNullException.ThrowIfNull(imageData);
      SetPendingImage(imageData, "image/png", "Clipboard image.png");
    }

    /// <inheritdoc/>
    public override object? ViewObject
    {
      get => _view;
      set
      {
        if (!ReferenceEquals(_view, value))
        {
          if (_view is not null)
            _view.DataContext = null;

          _view = value as IChatView;
          if (_view is not null)
            _view.DataContext = this;
        }
      }
    }

    /// <inheritdoc/>
    public override object ModelObject => null!;

    /// <inheritdoc/>
    public override void Dispose()
    {
      _disposed = true;
      _activeRequest?.Cancel();
      base.Dispose();

      if (_session is not null)
      {
        var disposeTask = _session.DisposeAsync().AsTask();
        Altaxo.Current.GetService<IShutdownService>()?.AddBackgroundTask(disposeTask);
        _session = null;
      }
    }

    private async Task SendAsync()
    {
      var prompt = InputText.Trim();
      if ((prompt.Length == 0 && _pendingImageData is null) || IsBusy || _disposed)
        return;

      var imageData = _pendingImageData;
      var imageMediaType = _pendingImageMediaType;
      var imageName = _pendingImageName;
      InputText = string.Empty;
      ClearPendingImage();
      Messages.Add(new McpChatMessageViewModel("You", prompt, imageName));
      IsBusy = true;
      StatusText = "Waiting for the assistant...";

      var cancellationTokenSource = new CancellationTokenSource();
      _activeRequest = cancellationTokenSource;
      try
      {
        var chatServiceConfiguration = ChatServiceRouter.GetConfiguredChatService();
        if (chatServiceConfiguration is null)
        {
          ShowChatServiceSettingsDialog();
          chatServiceConfiguration = ChatServiceRouter.GetConfiguredChatService();
          if (chatServiceConfiguration is null)
          {

            StatusText = "Chat service is not configured";
            return;
          }
        }

        if (_session is null || !_chatServiceConfiguration!.Equals(chatServiceConfiguration))
        {
          if (_session is not null)
            await _session.DisposeAsync();
          _session = null;
          _chatServiceConfiguration = chatServiceConfiguration;
          _session = ChatServiceRouter.CreateSession(chatServiceConfiguration);
        }

        if (_session is null)
          return;

        var assistantMessage = new McpChatMessageViewModel(_chatServiceConfiguration.Model, string.Empty);
        if (!_disposed)
          Messages.Add(assistantMessage);
        await foreach (var text in _session.StreamAsync(prompt, cancellationTokenSource.Token, imageData, imageMediaType))
        {
          if (!_disposed)
          {
            assistantMessage.AppendText(text);
          }
        }
        StatusText = "Ready";
      }
      catch (OperationCanceledException) when (cancellationTokenSource.IsCancellationRequested)
      {
        if (!_disposed)
          Messages.Add(new McpChatMessageViewModel("System", "Request cancelled."));
        StatusText = "Cancelled";
      }
      catch (Exception ex)
      {
        if (!_disposed)
          Messages.Add(new McpChatMessageViewModel("Error", ex.Message));
        StatusText = "Request failed";
      }
      finally
      {
        _activeRequest = null;
        cancellationTokenSource.Dispose();
        IsBusy = false;
      }
    }

    private void ShowChatServiceSettingsDialog()
    {
      var chatService = ChatServiceRouter.GetConfiguredChatService();
      var controller = (IMVCANController)Current.Gui.GetController(new object[] { chatService }, typeof(IChatService), typeof(IMVCANController), UseDocument.Directly);

      if (Current.Gui.ShowDialog(controller, "Chat service settings", showApplyButton: false))
      {
        chatService = (IChatService)controller.ModelObject;
        Current.PropertyService.SetValue(ChatServiceRouter.PropertyKeyChatService, chatService);
      }
    }

    private void AttachImageFromFile()
    {
      var options = new OpenFileOptions { Title = "Attach image to chat" };
      options.AddFilter("*.png;*.jpg;*.jpeg;*.gif;*.webp", "Image files (*.png;*.jpg;*.jpeg;*.gif;*.webp)");
      if (!Current.Gui.ShowOpenFileDialog(options))
        return;

      var mediaType = GetImageMediaType(options.FileName);
      if (mediaType is null)
      {
        Current.Gui.ErrorMessageBox("Select a PNG, JPEG, GIF, or WebP image.");
        return;
      }

      try
      {
        var fileInfo = new FileInfo(options.FileName);
        if (fileInfo.Length > MaximumImageSizeInBytes)
        {
          Current.Gui.ErrorMessageBox("The image must not exceed 20 MB.");
          return;
        }

        SetPendingImage(File.ReadAllBytes(options.FileName), mediaType, Path.GetFileName(options.FileName));
      }
      catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException)
      {
        Current.Gui.ErrorMessageBox(ex.Message);
      }
    }

    private void SetPendingImage(byte[] imageData, string mediaType, string imageName)
    {
      if (imageData.Length == 0)
      {
        Current.Gui.ErrorMessageBox("The selected image is empty.");
        return;
      }
      if (imageData.Length > MaximumImageSizeInBytes)
      {
        Current.Gui.ErrorMessageBox("The image must not exceed 20 MB.");
        return;
      }

      _pendingImageData = imageData;
      _pendingImageMediaType = mediaType;
      _pendingImageName = imageName;
      OnPropertyChanged(nameof(PendingImageName));
      CmdSend.OnCanExecuteChanged();
      CmdRemoveImage.OnCanExecuteChanged();
    }

    private void ClearPendingImage()
    {
      _pendingImageData = null;
      _pendingImageMediaType = null;
      _pendingImageName = null;
      OnPropertyChanged(nameof(PendingImageName));
      CmdSend.OnCanExecuteChanged();
      CmdRemoveImage.OnCanExecuteChanged();
    }

    private static string? GetImageMediaType(string fileName)
    {
      return Path.GetExtension(fileName).ToLowerInvariant() switch
      {
        ".png" => "image/png",
        ".jpg" or ".jpeg" => "image/jpeg",
        ".gif" => "image/gif",
        ".webp" => "image/webp",
        _ => null
      };
    }

  }
}
