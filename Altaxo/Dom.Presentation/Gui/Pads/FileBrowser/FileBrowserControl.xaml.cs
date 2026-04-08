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

#nullable disable warnings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Altaxo.Gui.Pads.FileBrowser
{
  /// <summary>
  /// Interaction logic for FileScoutControl.xaml
  /// </summary>
  public partial class FileBrowserControl : UserControl, IFileBrowserView
  {
    private FileBrowserController _controller;

    /// <summary>
    /// Occurs when the user activates the selected items (either by double-clicking on it, or by pressing Enter).
    /// </summary>
    public event Action SelectedItemsActivated;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileBrowserControl"/> class.
    /// </summary>
    public FileBrowserControl()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Gets or sets the controller.
    /// </summary>
    public FileBrowserController Controller
    {
      get
      {
        return _controller;
      }
      set
      {
        if (!object.ReferenceEquals(_controller, value))
        {
          _controller = value;
        }
      }
    }

    #region Focus

    /// <summary>
    /// Gets the control that should initially receive the focus.
    /// </summary>
    public object InitiallyFocusedControl
    {
      get { return _treeView; }
    }

    #endregion Focus

    #region IFileTreeView

    /// <summary>
    /// Initializes the folder tree.
    /// </summary>
    /// <param name="nodes">The tree nodes.</param>
    public void Initialize_FolderTree(Collections.NGTreeNodeCollection nodes)
    {
      _treeView.ItemsSource = nodes;
    }

    /// <summary>
    /// Occurs when a folder tree node is selected.
    /// </summary>
    public event Action<Collections.NGTreeNode> FolderTreeNodeSelected;

    #endregion IFileTreeView

    #region IFileListView

    /// <summary>
    /// Initializes the file list column names.
    /// </summary>
    /// <param name="names">The column names.</param>
    public void Initialize_FileListColumnNames(ICollection<string> names)
    {
      int i = 0;
      foreach (var name in names)
      {
        /*
				var gvc = new GridViewColumn() { Header = name };
				gvc.DisplayMemberBinding = new Binding(i == 0 ? "Text " : "Text" + i.ToString());

				grid.Columns.Add(gvc);
				*/
        ++i;
      }
    }

    /// <summary>
    /// Initializes the file list.
    /// </summary>
    /// <param name="files">The files to display.</param>
    public void Initialize_FileList(Collections.SelectableListNodeList files)
    {
      _listView.ItemsSource = files;
    }

    #endregion IFileListView

    private void EhTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
      FolderTreeNodeSelected?.Invoke((Collections.NGTreeNode)_treeView.SelectedItem);
    }

    private void EhListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      SelectedItemsActivated?.Invoke();
      e.Handled = true;
    }

    private void EhListViewItem_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter)
      {
        e.Handled = true;
        SelectedItemsActivated?.Invoke();
      }
    }
  }
}
