#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Altaxo.Gui.Settings
{
  /// <summary>
  /// Interaction logic for SettingsView.xaml
  /// </summary>
  public partial class SettingsControl : UserControl, ISettingsView
  {
    /// <summary>Occurs when the current topic view was entered.</summary>
    public event Action? CurrentTopicViewMadeDirty;

    public SettingsControl()
    {
      InitializeComponent();
      _guiControlHost.PreviewGotKeyboardFocus += new KeyboardFocusChangedEventHandler(EhHostControlKeyboardFocused);
    }

    private void EhHostControlKeyboardFocused(object sender, KeyboardFocusChangedEventArgs e)
    {
      if (CurrentTopicViewMadeDirty is not null)
        CurrentTopicViewMadeDirty();
    }

    private void EhTopicChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
      if (TopicSelectionChanged is not null)
        TopicSelectionChanged((Collections.NGTreeNode)e.NewValue);
    }

    public event Action<Collections.NGTreeNode>? TopicSelectionChanged;

    public void InitializeTopics(Collections.NGTreeNodeCollection topics)
    {
      _guiTopics.ItemsSource = topics;
    }

    public void InitializeTopicView(string title, object guiTopicObject)
    {
      _guiTopicLabel.Content = title;
      _guiControlHost.Content = guiTopicObject as UIElement;
    }

    public void InitializeTopicViewDirtyIndicator(int dirtyIndicator)
    {
      switch (dirtyIndicator)
      {
        case 0:
          _guiDirtyIndicator.Fill = Brushes.Transparent;
          break;

        case 1:
          _guiDirtyIndicator.Fill = Brushes.Black;
          break;

        case 2:
          _guiDirtyIndicator.Fill = Brushes.Red;
          break;
      }
    }

    public void SetSelectedNode(Collections.NGTreeNode node)
    {
      var item = _guiTopics.ItemContainerGenerator.ContainerFromItem(node) as TreeViewItem;

      if (item is not null)
      {
        item.Focus();
        item.IsSelected = true;
      }
    }
  }
}
