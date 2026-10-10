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
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Altaxo.Gui.Chat
{
  /// <summary>
  /// Displays the Mcp chat pad.
  /// </summary>
  public partial class ChatControl : UserControl, IChatView
  {
    private ChatController? _chatController;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatControl"/> class.
    /// </summary>
    public ChatControl()
    {
      InitializeComponent();
      Loaded += EhLoaded;
      Unloaded += EhUnloaded;
      DataContextChanged += EhDataContextChanged;
    }

    private void EhLoaded(object sender, RoutedEventArgs e)
    {
      AttachChatController(DataContext as ChatController);
      ScrollToLatestMessage();
    }

    private void EhUnloaded(object sender, RoutedEventArgs e)
    {
      AttachChatController(null);
    }

    private void EhDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (IsLoaded)
        AttachChatController(e.NewValue as ChatController);
    }

    private void AttachChatController(ChatController? chatController)
    {
      if (ReferenceEquals(_chatController, chatController))
        return;

      if (_chatController is not null)
      {
        _chatController.Messages.CollectionChanged -= EhMessagesCollectionChanged;
        foreach (var message in _chatController.Messages)
          message.PropertyChanged -= EhMessagePropertyChanged;
      }

      _chatController = chatController;
      if (_chatController is not null)
      {
        _chatController.Messages.CollectionChanged += EhMessagesCollectionChanged;
        foreach (var message in _chatController.Messages)
          message.PropertyChanged += EhMessagePropertyChanged;
      }
    }

    private void EhMessagesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.OldItems is not null)
      {
        foreach (McpChatMessageViewModel message in e.OldItems)
          message.PropertyChanged -= EhMessagePropertyChanged;
      }
      if (e.NewItems is not null)
      {
        foreach (McpChatMessageViewModel message in e.NewItems)
          message.PropertyChanged += EhMessagePropertyChanged;
      }
      ScrollToLatestMessage();
    }

    private void EhMessagePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(McpChatMessageViewModel.Text))
        ScrollToLatestMessage();
    }

    private void ScrollToLatestMessage()
    {
      Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() => ChatScrollViewer.ScrollToEnd()));
    }

    private void EhInputPreviewKeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) != 0 && DataContext is ChatController chatController && Clipboard.ContainsImage())
      {
        if (Clipboard.GetImage() is BitmapSource bitmap)
        {
          var encoder = new PngBitmapEncoder();
          encoder.Frames.Add(BitmapFrame.Create(bitmap));
          using var stream = new MemoryStream();
          encoder.Save(stream);
          chatController.AttachClipboardImage(stream.ToArray());
          e.Handled = true;
          return;
        }
      }

      if (e.Key == Key.Enter && (Keyboard.Modifiers & ModifierKeys.Shift) == 0)
      {
        e.Handled = true;
        if (DataContext is ChatController controller && controller.CmdSend.CanExecute(null))
          controller.CmdSend.Execute(null);
      }
    }
  }
}
