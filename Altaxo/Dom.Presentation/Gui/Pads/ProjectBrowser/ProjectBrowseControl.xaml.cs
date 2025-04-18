﻿#region Copyright

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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Altaxo.Gui.AddInItems;
using GongSolutions.Wpf.DragDrop;

namespace Altaxo.Gui.Pads.ProjectBrowser
{
  /// <summary>
  /// Interaction logic for UserControl1.xaml
  /// </summary>
  public partial class ProjectBrowseControl : UserControl, IProjectBrowseView
  {
    private ProjectBrowseController _controller;

    private ContextMenu _treeNodeContextMenu;

    public ProjectBrowseControl()
    {
      InitializeComponent();
    }

    public ProjectBrowseController Controller
    {
      get { return _controller; }
      set
      {
        if (!object.ReferenceEquals(_controller, value))
        {
          _controller = value;

          if (_controller is not null)
          {
            ContextMenu mnu1 = MenuService.CreateContextMenu(_controller, "/Altaxo/Pads/ProjectBrowser/ItemList/ContextMenu");
            _listView.ContextMenu = mnu1;

            ContextMenu mnu2 = MenuService.CreateContextMenu(_controller, "/Altaxo/Pads/ProjectBrowser/TreeView/ContextMenu");
            _treeView.ContextMenu = mnu2;

            _treeNodeContextMenu = MenuService.CreateContextMenu(_controller, "/Altaxo/Pads/ProjectBrowser/TreeNode/ContextMenu");
          }
        }
      }
    }

    private void EhTreeNodeAfterSelect(object sender, RoutedEventArgs e)
    {
      if (_controller is not null)
      {
        _controller.EhTreeNodeAfterSelect((NGBrowserTreeNode)_treeView.SelectedValue);
      }
    }

    public object InitiallyFocusedControl
    {
      get { return null; }
    }

    public void Dispose()
    {
    }

    public void InitializeTree(Collections.NGTreeNode root)
    {
      root.Nodes[0].IsExpanded = true;
      root.Nodes[0].IsSelected = true;

      _treeView.ItemsSource = root.Nodes;
    }

    public void SilentSelectTreeNode(Collections.NGTreeNode node)
    {
      // Trick to silently select the node: disable the controller temporarily
      var helper = _controller;
      _controller = null;
      node.IsSelected = true;
      _controller = helper;
    }

    public object TreeNodeContextMenu
    {
      get { return _treeNodeContextMenu; }
    }


    public void InitializeList(Collections.SelectableListNodeList list)
    {
      _listView.ItemsSource = null;
      _listView.Items.Clear();
      _listView.ItemsSource = list;


    }

    bool _isFullNameFolder = true;
    double _columnNameRev_Width = 100;

    public void InitializeCurrentFolder(string currentFolder, bool isFullNameFolder)
    {
      _guiCurrentFolderName.Text = currentFolder;

      if (_isFullNameFolder != isFullNameFolder)
      {
        _isFullNameFolder = isFullNameFolder;

        if (_isFullNameFolder)
        {
          _listViewCol3.Width = _columnNameRev_Width;
        }
        else
        {
          _columnNameRev_Width = _listViewCol3.ActualWidth;
          _listViewCol3.Width = 0;
        }

      }
    }

    public void SynchronizeListSelection()
    {
      foreach (Collections.SelectableListNode node in _listView.Items)
        node.IsSelected = false;
      foreach (Collections.SelectableListNode node in _listView.SelectedItems)
        node.IsSelected = true;
    }

    private void EhListViewItemDoubleClick(object sender, MouseButtonEventArgs e)
    {
      SynchronizeListSelection();
      if (_controller is not null)
        _controller.EhListViewDoubleClick();
    }

    private void EhListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      SynchronizeListSelection();
      if (_controller is not null)
        _controller.EhListViewAfterSelect();
    }

    private void EhDeleteCommandExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      e.Handled = true;
    }

    private void EhDeleteCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.Handled = true;
    }

    private void EhListViewDeleteCommandExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      _controller?.DeleteSelectedListItems();
      e.Handled = true;
    }

    private void EhListViewDeleteCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = _listView.SelectedItems.Count > 0;
      e.Handled = true;
    }

    private void EhListViewSelectAllCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
      e.Handled = true;
    }

    private void EhListViewCopyCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      if (_controller is not null)
      {
        e.CanExecute = _listView.SelectedItems.Count > 0;
        e.Handled = true;
      }
    }

    private void EhListViewCopyCommandExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      if (_controller is not null)
      {
        _controller.CopySelectedListItemsToClipboard();
        e.Handled = true;
      }
    }

    private void EhListViewPasteCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      if (_controller is not null)
      {
        e.CanExecute = _controller.CanPasteItemsFromClipboard();
        e.Handled = true;
      }
    }

    private void EhListViewPasteCommandExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      if (_controller is not null)
      {
        _controller.PasteItemsFromClipboard();
        e.Handled = true;
      }
    }

    private void EhListViewSelectAllCommandExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      _listView.SelectAll();
    }

    private void EhListView_OneFolderUp(object sender, RoutedEventArgs e)
    {
      if (_controller is not null)
        _controller.EhListView_OneFolderUp();
    }

    private void EhNavigateBackward(object sender, RoutedEventArgs e)
    {
      if (_controller is not null)
        _controller.EhNavigateBackward();
    }

    private void EhNavigateForward(object sender, RoutedEventArgs e)
    {
      if (_controller is not null)
        _controller.EhNavigateForward();
    }

    private GridViewColumnHeader _currentPrimarySortedColumnHeader = null;
    private Altaxo.Gui.Common.SortAdorner _currentPrimarySortAdorner = null;

    private GridViewColumnHeader _currentSecondarySortedColumnHeader = null;
    private Altaxo.Gui.Common.SortAdorner _currentSecondarySortAdorner = null;

    private void SetSortAdorner(Visual visual, ref Adorner adorner, bool isSorted, bool isDescendingSort, bool isSecondaryAdorner)
    {
      if (adorner is not null)
      {
        AdornerLayer.GetAdornerLayer(visual)?.Remove(adorner);
        adorner = null;
      }

      if (isSorted)
      {
        adorner = new Common.SortAdorner((UIElement)visual, isDescendingSort ? ListSortDirection.Descending : ListSortDirection.Ascending, isSecondaryAdorner);
        var adornerLayer = AdornerLayer.GetAdornerLayer(visual); // adornerLayer is valid only after the visual was visible for the first time
        if (adornerLayer is not null)
          adornerLayer.Add(adorner);
      }
    }

    private Adorner _nameColumnSortAdorner;

    public void SetSortIndicator_NameColumn(bool isSorted, bool isDescendingSort, bool isSecondaryAdorner)
    {
      SetSortAdorner(_listViewColHeader_Name, ref _nameColumnSortAdorner, isSorted, isDescendingSort, isSecondaryAdorner);
    }

    private Adorner _creationDateColumnSortAdorner;

    public void SetSortIndicator_CreationDateColumn(bool isSorted, bool isDescendingSort, bool isSecondaryAdorner)
    {
      SetSortAdorner(_listViewColHeader_CreationDate, ref _creationDateColumnSortAdorner, isSorted, isDescendingSort, isSecondaryAdorner);
    }

    private Adorner _changeDateColumnSortAdorner;

    public void SetSortIndicator_ChangeDateColumn(bool isSorted, bool isDescendingSort, bool isSecondaryAdorner)
    {
      SetSortAdorner(_listViewColHeader_ChangeDate, ref _changeDateColumnSortAdorner, isSorted, isDescendingSort, isSecondaryAdorner);
    }

    private Adorner _nameRevColumnSortAdorner;

    public void SetSortIndicator_NameRevColumn(bool isSorted, bool isDescendingSort, bool isSecondaryAdorner)
    {
      SetSortAdorner(_listViewColHeader_NameRev, ref _nameRevColumnSortAdorner, isSorted, isDescendingSort, isSecondaryAdorner);
    }


    private void EhListView_ColumnHeaderClicked_Name(object sender, RoutedEventArgs e)
    {
      if (_controller is not null)
        _controller.EhToggleListSort_Name();
    }

    private void EhListView_ColumnHeaderClicked_CreationDate(object sender, RoutedEventArgs e)
    {
      if (_controller is not null)
        _controller.EhToggleListSort_CreationDate();
    }

    private void EhListView_ColumnHeaderClicked_ChangeDate(object sender, RoutedEventArgs e)
    {
      if (_controller is not null)
        _controller.EhToggleListSort_ChangeDate();
    }

    private void EhListView_ColumnHeaderClicked_NameRev(object sender, RoutedEventArgs e)
    {
      if (_controller is not null)
        _controller.EhToggleListSort_NameRev();
    }


    /// <summary>Ehes the list view sort.</summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
    /// <remarks>
    /// <code>
    /// Behaviour when user clicked on:
    /// - primary sort column: toggle direction of primary sort column, leave secondary as is
    /// - secondary sort column: make old primary sort column now secondary sort column, clicked column now the primary sort column with same direction as formerly secondary sort column
    /// - other column: make old primary sort column now secondary sort column, clicked column now the primary sort column ascending
    /// </code>
    /// </remarks>
    private void EhListViewSort(object sender, RoutedEventArgs e)
    {
      var columnHeaderClicked = sender as GridViewColumnHeader;
      String sortPropertyName = columnHeaderClicked.Tag as string;

      bool clickedOnPrimarySortColumn = columnHeaderClicked.Equals(_currentPrimarySortedColumnHeader);
      bool clickedOnSecondarySortColumn = columnHeaderClicked.Equals(_currentSecondarySortedColumnHeader);

      _listView.Items.SortDescriptions.Clear();
      if (_currentSecondarySortedColumnHeader is not null)
        AdornerLayer.GetAdornerLayer(_currentSecondarySortedColumnHeader).Remove(_currentSecondarySortAdorner);
      if (_currentPrimarySortedColumnHeader is not null)
        AdornerLayer.GetAdornerLayer(_currentPrimarySortedColumnHeader).Remove(_currentPrimarySortAdorner);

      ListSortDirection newDir = ListSortDirection.Ascending;

      if (clickedOnPrimarySortColumn)
      {
        if (_currentPrimarySortAdorner.Direction == ListSortDirection.Ascending)
          newDir = ListSortDirection.Descending;

        _currentPrimarySortAdorner = new Common.SortAdorner(_currentPrimarySortedColumnHeader, newDir, false);
      }
      else if (clickedOnSecondarySortColumn)
      {
        newDir = _currentSecondarySortAdorner.Direction;
        _currentSecondarySortAdorner = new Common.SortAdorner(_currentPrimarySortedColumnHeader, _currentPrimarySortAdorner.Direction, true);
        _currentSecondarySortedColumnHeader = _currentPrimarySortedColumnHeader;

        _currentPrimarySortAdorner = new Common.SortAdorner(columnHeaderClicked, newDir, false);
        _currentPrimarySortedColumnHeader = columnHeaderClicked;
      }
      else // clicked in any other column
      {
        if (_currentPrimarySortedColumnHeader is not null)
        {
          _currentSecondarySortAdorner = new Common.SortAdorner(_currentPrimarySortedColumnHeader, _currentPrimarySortAdorner.Direction, true);
          _currentSecondarySortedColumnHeader = _currentPrimarySortedColumnHeader;
        }

        _currentPrimarySortAdorner = new Common.SortAdorner(columnHeaderClicked, newDir, false);
        _currentPrimarySortedColumnHeader = columnHeaderClicked;
      }

      AdornerLayer.GetAdornerLayer(_currentPrimarySortedColumnHeader).Add(_currentPrimarySortAdorner);
      _listView.Items.SortDescriptions.Add(new SortDescription(_currentPrimarySortedColumnHeader.Tag as string, _currentPrimarySortAdorner.Direction));

      if (_currentSecondarySortedColumnHeader is not null)
      {
        AdornerLayer.GetAdornerLayer(_currentSecondarySortedColumnHeader).Add(_currentSecondarySortAdorner);
        _listView.Items.SortDescriptions.Add(new SortDescription(_currentSecondarySortAdorner.Tag as string, _currentSecondarySortAdorner.Direction));
      }
    }

    private void EhListViewPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      // ensures that the list view get focused even if the user only clicks with the right mouse button
      // so it is ensured that the paste command will be available, even if the list view contains no items and the user only opens the context menu with the right mouse button click
      if (!_listView.IsKeyboardFocusWithin)
        _listView.Focus();
    }

    #region ListView Drag drop support

    private IDragSource _listViewDragHandler;

    public IDragSource ListViewDragHandler
    {
      get
      {
        if (_listViewDragHandler is null)
          _listViewDragHandler = new ListView_DragHandler(this);
        return _listViewDragHandler;
      }
    }

    private IDropTarget _listViewDropHandler;

    public IDropTarget ListViewDropHandler
    {
      get
      {
        if (_listViewDropHandler is null)
          _listViewDropHandler = new ListView_DropHandler(this);
        return _listViewDropHandler;
      }
    }

    #endregion ListView Drag drop support

    #region TreeView Drag drop support

    private IDragSource _treeViewDragHandler;

    public IDragSource TreeViewDragHandler
    {
      get
      {
        if (_treeViewDragHandler is null)
          _treeViewDragHandler = new TreeView_DragHandler(this);
        return _treeViewDragHandler;
      }
    }

    private IDropTarget _treeViewDropHandler;

    public IDropTarget TreeViewDropHandler
    {
      get
      {
        if (_treeViewDropHandler is null)
          _treeViewDropHandler = new TreeView_DropHandler(this);
        return _treeViewDropHandler;
      }
    }

    #endregion TreeView Drag drop support
  }
}
