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

using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Altaxo.Gui.Pads.ProjectBrowser
{
	/// <summary>
	/// Interaction logic for UserControl1.xaml
	/// </summary>
	public partial class ProjectBrowseControl : UserControl, IProjectBrowseView, ICSharpCode.SharpDevelop.Gui.IPadContent
	{
		private ProjectBrowseController _controller;

		private ContextMenu _treeNodeContextMenu;

		public ProjectBrowseControl()
		{
			InitializeComponent();

			_controller = new ProjectBrowseController();

			ContextMenu mnu1 = MenuService.CreateContextMenu(this._controller, "/Altaxo/Pads/ProjectBrowser/ItemList/ContextMenu");
			_listView.ContextMenu = mnu1;

			ContextMenu mnu2 = MenuService.CreateContextMenu(this._controller, "/Altaxo/Pads/ProjectBrowser/TreeView/ContextMenu");
			_treeView.ContextMenu = mnu2;

			_treeNodeContextMenu = MenuService.CreateContextMenu(this._controller, "/Altaxo/Pads/ProjectBrowser/TreeNode/ContextMenu");

			_controller.ViewObject = this;
		}

		private void EhTreeNodeAfterSelect(object sender, RoutedEventArgs e)
		{
			if (null != _controller)
			{
				_controller.EhTreeNodeAfterSelect((NGBrowserTreeNode)_treeView.SelectedValue);
			}
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);
			//_controller.ViewObject = this;
		}

		public object Control
		{
			get { return this; }
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

		public void InitializeCurrentFolder(string currentFolder)
		{
			_guiCurrentFolderName.Text = currentFolder;
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
			if (null != _controller)
				_controller.EhListViewDoubleClick();
		}

		private void EhListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SynchronizeListSelection();
			if (null != _controller)
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
			_controller.DeleteSelectedListItems();
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
			if (null != _controller)
			{
				e.CanExecute = _listView.SelectedItems.Count > 0;
				e.Handled = true;
			}
		}

		private void EhListViewCopyCommandExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			if (null != _controller)
			{
				_controller.CopySelectedListItemsToClipboard();
				e.Handled = true;
			}
		}

		private void EhListViewPasteCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			if (null != _controller)
			{
				e.CanExecute = _controller.CanPasteItemsFromClipboard();
				e.Handled = true;
			}
		}

		private void EhListViewPasteCommandExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			if (null != _controller)
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
			if (null != _controller)
				_controller.EhListView_OneFolderUp();
		}

		private void EhNavigateBackward(object sender, RoutedEventArgs e)
		{
			if (null != _controller)
				_controller.EhNavigateBackward();
		}

		private void EhNavigateForward(object sender, RoutedEventArgs e)
		{
			if (null != _controller)
				_controller.EhNavigateForward();
		}

		private GridViewColumnHeader _currentPrimarySortedColumnHeader = null;
		private Altaxo.Gui.Common.SortAdorner _currentPrimarySortAdorner = null;

		private GridViewColumnHeader _currentSecondarySortedColumnHeader = null;
		private Altaxo.Gui.Common.SortAdorner _currentSecondarySortAdorner = null;

		private void SetSortAdorner(Visual visual, ref Adorner adorner, bool isSorted, bool isDescendingSort, bool isSecondaryAdorner)
		{
			if (null != adorner)
			{
				AdornerLayer.GetAdornerLayer(visual).Remove(adorner);
				adorner = null;
			}

			if (isSorted)
			{
				adorner = new Common.SortAdorner((UIElement)visual, isDescendingSort ? ListSortDirection.Descending : ListSortDirection.Ascending, isSecondaryAdorner);
				var adornerLayer = AdornerLayer.GetAdornerLayer(visual); // adornerLayer is valid only after the visual was visible for the first time
				if (null != adornerLayer)
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

		private void EhListView_ColumnHeaderClicked_Name(object sender, RoutedEventArgs e)
		{
			if (null != _controller)
				_controller.EhToggleListSort_Name();
		}

		private void EhListView_ColumnHeaderClicked_CreationDate(object sender, RoutedEventArgs e)
		{
			if (null != _controller)
				_controller.EhToggleListSort_CreationDate();
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
			GridViewColumnHeader columnHeaderClicked = sender as GridViewColumnHeader;
			String sortPropertyName = columnHeaderClicked.Tag as String;

			bool clickedOnPrimarySortColumn = columnHeaderClicked.Equals(_currentPrimarySortedColumnHeader);
			bool clickedOnSecondarySortColumn = columnHeaderClicked.Equals(_currentSecondarySortedColumnHeader);

			_listView.Items.SortDescriptions.Clear();
			if (null != _currentSecondarySortedColumnHeader)
				AdornerLayer.GetAdornerLayer(_currentSecondarySortedColumnHeader).Remove(_currentSecondarySortAdorner);
			if (null != _currentPrimarySortedColumnHeader)
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
				if (null != _currentPrimarySortedColumnHeader)
				{
					_currentSecondarySortAdorner = new Common.SortAdorner(_currentPrimarySortedColumnHeader, _currentPrimarySortAdorner.Direction, true);
					_currentSecondarySortedColumnHeader = _currentPrimarySortedColumnHeader;
				}

				_currentPrimarySortAdorner = new Common.SortAdorner(columnHeaderClicked, newDir, false);
				_currentPrimarySortedColumnHeader = columnHeaderClicked;
			}

			AdornerLayer.GetAdornerLayer(_currentPrimarySortedColumnHeader).Add(_currentPrimarySortAdorner);
			_listView.Items.SortDescriptions.Add(new SortDescription(_currentPrimarySortedColumnHeader.Tag as string, _currentPrimarySortAdorner.Direction));

			if (null != _currentSecondarySortedColumnHeader)
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
	}
}