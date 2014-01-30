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

using Altaxo.Collections;
using Altaxo.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Pads.ProjectBrowser
{
	#region Interfaces

	/// <summary>
	///
	/// </summary>
	/// <remarks>The image indices for the browser tree nodes and list nodes are set as following:
	/// 0: Project, 1: closed folder, 2: open folder, 3: worksheet, 4: graph.</remarks>
	public interface IProjectBrowseView
	{
		/// <summary>Sets the browser root node.</summary>
		/// <param name="root">The root node of the project browser.</param>
		void InitializeTree(Altaxo.Collections.NGTreeNode root);

		void SilentSelectTreeNode(Altaxo.Collections.NGTreeNode node);

		object TreeNodeContextMenu { get; }

		void InitializeList(SelectableListNodeList list);

		/// <summary>Sets the display in what folder we are currently in.</summary>
		/// <param name="currentFolder">Name of the current folder.</param>
		void InitializeCurrentFolder(string currentFolder);

		void SetSortIndicator_NameColumn(bool isSorted, bool isDescendingSort, bool isSecondaryAdorner);

		void SetSortIndicator_CreationDateColumn(bool isSorted, bool isDescendingSort, bool isSecondaryAdorner);

		void SynchronizeListSelection();
	}

	#endregion Interfaces

	public class ProjectBrowseController : IMVCController
	{
		/// <summary>The gui view shown to the user.</summary>
		private IProjectBrowseView _view;

		/// <summary>The current Altaxo project.</summary>
		private AltaxoDocument _doc;

		/// <summary>Root node. Holds all other nodes in its nodes collection.</summary>
		private NGBrowserTreeNode _rootNode;

		/// <summary>Root node of the project folders.</summary>
		private NGBrowserTreeNode _projectDirectoryRoot;

		/// <summary>Project node. Equivalent to the whole project</summary>
		private NGBrowserTreeNode _allItemsNode;

		/// <summary>When selected, shows all graphs in the project.</summary>
		private NGBrowserTreeNode _allGraphsNode;

		/// <summary>When selected, shows all tables in the project.</summary>
		private NGBrowserTreeNode _allTablesNode;

		/// <summary>Dictionary of project folders (keys) and the corresponing non-Gui nodes (values).</summary>
		private Dictionary<string, NGBrowserTreeNode> _directoryNodesByName;

		/// <summary>Current selected tree node.</summary>
		private NGBrowserTreeNode _currentSelectedTreeNode;

		/// <summary>Items currently shown in the list view.</summary>
		private SelectableListNodeList _listViewItems = new SelectableListNodeList();

		/// <summary>Object that is responsible for filling the list with items and for tracking changes affecting the list items.</summary>
		private AbstractItemHandler _listItemHandler;

		/// <summary>Action when selecting a tree node.</summary>
		private ViewOnSelect _viewOnSelectTreeNode = ViewOnSelect.Off;

		/// <summary>Action when selection a list node. If true, the item is shown in the document area.</summary>
		private bool _viewOnSelectListNodeOn = false;

		private NavigationList<NavigationPoint> _navigationPoints = new NavigationList<NavigationPoint>();

		/// <summary>Creates the project browse controller.</summary>
		public ProjectBrowseController()
		{
			_rootNode = new NGBrowserTreeNode("Root");
			_directoryNodesByName = new Dictionary<string, NGBrowserTreeNode>();

			_allItemsNode = new NGBrowserTreeNode("Project") { Image = ProjectBrowseItemImage.Project };
			_rootNode.Nodes.Add(_allItemsNode);

			_allGraphsNode = new NGBrowserTreeNode("Graphs") { Image = ProjectBrowseItemImage.OpenFolder };
			_allItemsNode.Nodes.Add(_allGraphsNode);

			_allTablesNode = new NGBrowserTreeNode("Tables") { Image = ProjectBrowseItemImage.OpenFolder };
			_allItemsNode.Nodes.Add(_allTablesNode);

			_projectDirectoryRoot = new NGBrowserTreeNode("\\") { Image = ProjectBrowseItemImage.OpenFolder, Tag = ProjectFolder.RootFolderName };
			_directoryNodesByName.Add((string)_projectDirectoryRoot.Tag, _projectDirectoryRoot);
			_allItemsNode.Nodes.Add(_projectDirectoryRoot);

			Current.ProjectService.ProjectOpened += this.EhProjectOpened;
			Current.ProjectService.ProjectClosed += this.EhProjectClosed;

			Initialize(true);
		}

		private void Initialize(bool initData)
		{
			if (initData)
			{
				EhProjectOpened(this, new ProjectEventArgs(Current.Project));
			}

			if (null != _view)
			{
				_allGraphsNode.ContextMenu = _view.TreeNodeContextMenu;
				_allTablesNode.ContextMenu = _view.TreeNodeContextMenu;
				_projectDirectoryRoot.SetContextMenuRecursively(_view.TreeNodeContextMenu);

				_view.InitializeTree(_rootNode);
				_view.InitializeCurrentFolder(GetLocationStringFromCurrentState());

				UpdateSortIndicatorsInView();
			}
		}

		/// <summary>
		/// Gets the project this controller is visualizing.
		/// </summary>
		public AltaxoDocument Project
		{
			get { return _doc; }
		}

		private void EhProjectOpened(object sender, ProjectEventArgs e)
		{
			Current.Gui.Execute(EhProjectOpened_Unsynchronized, sender, e);
		}

		private void EhProjectOpened_Unsynchronized(object sender, ProjectEventArgs e)
		{
			if (object.ReferenceEquals(_doc, e.Project))
				return;

			if (null != _doc)
			{
				_doc.Folders.CollectionChanged -= this.EhProjectDirectoryItemChanged;
			}

			_doc = e.Project;

			if (null != _doc)
			{
				_doc.Folders.CollectionChanged += this.EhProjectDirectoryItemChanged;
			}

			RecreateDirectoryNodes();
			SetItemListHandler(new ProjectAllItemHandler());
		}

		private void EhProjectClosed(object sender, ProjectEventArgs e)
		{
			Current.Gui.Execute(EhProjectClosed_Unsynchronized, sender, e);
		}

		private void EhProjectClosed_Unsynchronized(object sender, ProjectEventArgs e)
		{
			if (null != _doc)
			{
				_doc.Folders.CollectionChanged -= this.EhProjectDirectoryItemChanged;
				_doc = null;
				SetItemListHandler(null);
			}
		}

		#region Tree related stuff

		/// <summary>
		/// Returns the name that is displayed in the TreeView
		/// </summary>
		/// <param name="fullFolderPath"></param>
		/// <returns></returns>
		private static string GetDisplayNameOfFolder(string fullFolderPath)
		{
			return ProjectFolder.ConvertFolderNameToDisplayFolderLastPart(fullFolderPath);
		}

		private void RecreateDirectoryNodes()
		{
			_directoryNodesByName.Clear();
			_directoryNodesByName.Add((string)_projectDirectoryRoot.Tag, _projectDirectoryRoot);
			_projectDirectoryRoot.Nodes.Clear();

			CreateDirectoryNode((string)_projectDirectoryRoot.Tag, _projectDirectoryRoot);

			if (_view != null)
				((IGuiBrowserTreeNode)_projectDirectoryRoot.GuiTag).OnNodeMultipleChanges();
		}

		private void CreateDirectoryNode(string dir, NGBrowserTreeNode node)
		{
			var subfolders = _doc.Folders.GetSubfoldersAsStringList(dir, false);
			foreach (var subfolder in subfolders)
			{
				var subnode = new NGBrowserTreeNode(GetDisplayNameOfFolder(subfolder))
				{
					Image = ProjectBrowseItemImage.OpenFolder,
					Tag = subfolder,
					ContextMenu = null == _view ? null : _view.TreeNodeContextMenu,
				};
				node.Nodes.Add(subnode);
				_directoryNodesByName.Add(subfolder, subnode);
				CreateDirectoryNode(subfolder, subnode);
			}
		}

		private void EhProjectDirectoryItemChanged(NamedObjectCollectionChangeType changeType, object item, string oldName, string newName)
		{
			Current.Gui.Execute(EhProjectDirectoryItemChanged_Unsynchronized, changeType, item, oldName, newName);
		}

		private void EhProjectDirectoryItemChanged_Unsynchronized(NamedObjectCollectionChangeType changeType, object item, string oldName, string newName)
		{
			if (changeType == NamedObjectCollectionChangeType.MultipleChanges)
			{
				RecreateDirectoryNodes();
			}
			else
			{
				if (item is string)
					EhProjectDirectoryChanged(changeType, (string)item);
			}
		}

		private void EhProjectDirectoryChanged(NamedObjectCollectionChangeType changeType, string dir)
		{
			Current.Gui.Execute(EhProjectDirectoryChanged_Unsynchronized, changeType, dir);
		}

		private void EhProjectDirectoryChanged_Unsynchronized(NamedObjectCollectionChangeType changeType, string dir)
		{
			if (ProjectFolder.IsRootFolderName(dir))
				return; // for the root directory, we have already the node, and we can not add or remove them

			ProjectFolder.ThrowExceptionOnInvalidFullFolderPath(dir);

			switch (changeType)
			{
				case NamedObjectCollectionChangeType.ItemAdded:
					{
						string parDir = ProjectFolder.GetFoldersParentFolder(dir);
						var parNode = _directoryNodesByName[parDir];
						var curNode = new NGBrowserTreeNode(GetDisplayNameOfFolder(dir))
						{
							Image = ProjectBrowseItemImage.OpenFolder,
							Tag = dir,
							ContextMenu = null == _view ? null : _view.TreeNodeContextMenu
						};
						_directoryNodesByName.Add(dir, curNode);
						parNode.Nodes.Add(curNode);
						if (parNode.GuiTag is IGuiBrowserTreeNode)
							((IGuiBrowserTreeNode)parNode.GuiTag).OnNodeAdded(curNode);
					}
					break;

				case NamedObjectCollectionChangeType.ItemRemoved:
					{
						var curNode = _directoryNodesByName[dir];
						_directoryNodesByName.Remove(dir);
						string parDir = ProjectFolder.GetFoldersParentFolder(dir);
						var parNode = _directoryNodesByName[parDir];
						parNode.Nodes.Remove(curNode);
						if (parNode.GuiTag is IGuiBrowserTreeNode)
							((IGuiBrowserTreeNode)parNode.GuiTag).OnNodeRemoved(curNode);
					}
					break;

				default:
					throw new ArgumentOutOfRangeException("Unexpected changeType: " + changeType.ToString());
			}
		}

		/// <summary>
		/// Called from the view after selecting a tree node.
		/// </summary>
		/// <param name="node">Non-Gui node that was selected.</param>
		public void EhTreeNodeAfterSelect(NGBrowserTreeNode node)
		{
			var oldSelectedNode = _currentSelectedTreeNode;
			_currentSelectedTreeNode = node;

			if (!object.ReferenceEquals(oldSelectedNode, _currentSelectedTreeNode))
			{
				RefreshItemListHandler();
			}

			// Handle view on select
			switch (_viewOnSelectTreeNode)
			{
				case ViewOnSelect.Off: // do nothing
					break;

				case ViewOnSelect.ItemsInFolder:
					ProjectBrowserExtensions.ShowDocumentsExclusively(GetAllListItems());
					break;

				case ViewOnSelect.ItemsInFolderAndSubfolders:
					ProjectBrowserExtensions.ShowDocumentsExclusively(ExpandItemListToSubfolderItems(GetAllListItems()));
					break;
			}
		}

		#endregion Tree related stuff

		#region List related stuff

		/// <summary>
		/// Internally sets the list item handler and wires the ListChange event.
		/// </summary>
		/// <param name="itemHandler"></param>
		private void SetItemListHandler(AbstractItemHandler itemHandler)
		{
			if (null != _listItemHandler)
			{
				_listItemHandler.ListChange -= EhListItemHandlerListChange;
			}

			_listItemHandler = itemHandler;

			if (null != _listItemHandler)
			{
				_listItemHandler.ListChange += EhListItemHandlerListChange;
				StoreNavigationPoint();

				if (null != _view)
					_view.InitializeCurrentFolder(GetLocationStringFromCurrentState());
			}
		}

		/// <summary>
		/// Sets the ItemList handler according to the currently selected tree node.
		/// </summary>
		private void RefreshItemListHandler()
		{
			if (null != _currentSelectedTreeNode)
			{
				if (object.ReferenceEquals(_currentSelectedTreeNode, _allItemsNode))
					SetItemListHandler(new ProjectAllItemHandler());
				else if (object.ReferenceEquals(_currentSelectedTreeNode, _allTablesNode))
					SetItemListHandler(new AllWorksheetHandler());
				else if (object.ReferenceEquals(_currentSelectedTreeNode, _allGraphsNode))
					SetItemListHandler(new AllGraphHandler());
				else if (object.ReferenceEquals(_currentSelectedTreeNode, _projectDirectoryRoot))
					SetItemListHandler(new SpecificProjectFolderHandler(ProjectFolder.RootFolderName));
				else if (_currentSelectedTreeNode.Tag is string)
					SetItemListHandler(new SpecificProjectFolderHandler((string)_currentSelectedTreeNode.Tag));
			}
		}

		/// <summary>
		/// Called if the list item handler announces a change in the item list.
		/// </summary>
		/// <param name="list"></param>
		private void EhListItemHandlerListChange(SelectableListNodeList list)
		{
			Current.Gui.Execute(EhListItemHandlerListChange_Unsynchronized, list);
		}

		/// <summary>
		/// Called if the list item handler announces a change in the item list.
		/// </summary>
		/// <param name="list"></param>
		private void EhListItemHandlerListChange_Unsynchronized(SelectableListNodeList list)
		{
			SortItemList(list);

			_listViewItems = list;

			if (_listItemHandler is SpecificProjectFolderHandler)
			{
				if (!_directoryNodesByName.TryGetValue(((SpecificProjectFolderHandler)_listItemHandler).CurrentProjectFolder, out _currentSelectedTreeNode))
					_currentSelectedTreeNode = _projectDirectoryRoot;
				if (null != _view)
					_view.SilentSelectTreeNode(_currentSelectedTreeNode);
			}

			if (null != _view)
			{
				_view.InitializeList(_listViewItems);
			}
		}

		/// <summary>
		/// Get a list of selected items (tables, graphs and project folders).
		/// </summary>
		/// <returns></returns>
		public List<object> GetSelectedListItems()
		{
			var result = new List<object>();

			foreach (BrowserListItem item in _listViewItems)
			{
				if (item.IsSelected)
				{
					result.Add(item.Tag);
				}
			}
			return result;
		}

		/// <summary>
		/// Get all items in the list.
		/// </summary>
		/// <returns>List of items.</returns>
		public List<object> GetAllListItems()
		{
			var result = new List<object>();

			foreach (BrowserListItem item in _listViewItems)
			{
				result.Add(item.Tag);
			}
			return result;
		}

		/// <summary>
		/// Expands the list of items by recursively replacing project folders by the items in those project folders.
		/// </summary>
		/// <param name="list">List of items.</param>
		/// <returns>The same instance of the list that was given as argument, now expanded by subfolder items.</returns>
		public List<object> ExpandItemListToSubfolderItems(List<object> list)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i] is ProjectFolder)
				{
					var folder = (list[i] as ProjectFolder);
					var subfolders = _doc.Folders.GetSubfoldersAsProjectFolderList(folder.Name);
					foreach (var item in subfolders)
						list.Add(item);

					var subitems = _doc.Folders.GetItemsInFolder(folder.Name);
					list.AddRange(subitems);

					list.RemoveAt(i);
					i--;
				}
			}
			return list;
		}

		/// <summary>
		/// Returns the number of currently selected list items.
		/// </summary>
		/// <returns>Number of selected list items.</returns>
		public int GetNumberOfSelectedListItems()
		{
			if (null == _view)
				return 0;
			_view.SynchronizeListSelection();
			int count = 0;
			foreach (BrowserListItem item in _listViewItems)
			{
				if (item.IsSelected)
				{
					count++;
				}
			}
			return count;
		}

		/// <summary>
		/// Called from the view if the selection of items in the list has changed.
		/// </summary>
		public void EhListViewAfterSelect()
		{
			if (_viewOnSelectListNodeOn && 1 == GetNumberOfSelectedListItems())
			{
				ProjectBrowserExtensions.ShowSelectedListItem(this);
			}
		}

		/// <summary>
		/// Called from the view if a list item was double clicked.
		/// </summary>
		public void EhListViewDoubleClick()
		{
			foreach (var node in this._listViewItems)
			{
				if (!node.IsSelected)
					continue;

				// table nodes
				if (node.Tag is Altaxo.Data.DataTable)
				{
					// tag is the name of the table clicked, so look for a view that has the table or
					// create a new one
					Current.ProjectService.OpenOrCreateWorksheetForTable((Altaxo.Data.DataTable)node.Tag);
				}
				else if (node.Tag is Altaxo.Graph.Gdi.GraphDocument)
				{
					// tag is the name of the table clicked, so look for a view that has the table or create a new one
					Current.ProjectService.OpenOrCreateGraphForGraphDocument((Altaxo.Graph.Gdi.GraphDocument)node.Tag);
				}
				else if (node.Tag is Altaxo.Main.Properties.ProjectFolderPropertyBag)
				{
					var propHierarchy = new Altaxo.Main.Properties.PropertyHierarchy(PropertyExtensions.GetPropertyBags(node.Tag as Altaxo.Main.Properties.ProjectFolderPropertyBag));
					Current.Gui.ShowDialog(new object[] { propHierarchy }, "Folder properties", true);
				}
				else if (node.Tag is ProjectFolder)
				{
					SetItemListHandler(new SpecificProjectFolderHandler((node.Tag as ProjectFolder).Name));
				}
			}
		}

		/// <summary>
		/// Navigate on folder up, to the parent folder.
		/// </summary>
		public void EhListView_OneFolderUp()
		{
			var spfh = _listItemHandler as SpecificProjectFolderHandler;
			if (null != spfh && !string.IsNullOrEmpty(spfh.CurrentProjectFolder))
			{
				var parentFolder = ProjectFolder.GetFoldersParentFolder(spfh.CurrentProjectFolder);
				SetItemListHandler(new SpecificProjectFolderHandler(parentFolder));
			}
		}

		#endregion List related stuff

		#region Navigation

		public void EhNavigateBackward()
		{
			NavigationPoint p;
			if (_navigationPoints.TryNavigateBackward(out p, IsNavigationPointValid, true))
			{
				NavigateTo(p);
			}
		}

		public void EhNavigateForward()
		{
			NavigationPoint p;
			if (_navigationPoints.TryNavigateForward(out p, IsNavigationPointValid, true))
			{
				NavigateTo(p);
			}
		}

		private bool IsNavigationPointValid(NavigationPoint p)
		{
			if (p.Kind != NavigationPoint.KindOfNavigationPoint.ProjectFolder)
				return true;
			else
				return Current.Project.Folders.ContainsFolder(p.Folder);
		}

		private void NavigateTo(NavigationPoint p)
		{
			switch (p.Kind)
			{
				case NavigationPoint.KindOfNavigationPoint.ProjectFolder:
					SetItemListHandler(new SpecificProjectFolderHandler(p.Folder));
					break;

				case NavigationPoint.KindOfNavigationPoint.AllTables:
					SetItemListHandler(new AllWorksheetHandler());
					break;

				case NavigationPoint.KindOfNavigationPoint.AllGraphs:
					SetItemListHandler(new AllGraphHandler());
					break;

				case NavigationPoint.KindOfNavigationPoint.AllProjectItems:
					SetItemListHandler(new ProjectAllItemHandler());
					break;
			}
		}

		private void StoreNavigationPoint()
		{
			_navigationPoints.AddNavigationPoint(GetNavigationPointFromCurrentState());
		}

		private NavigationPoint GetNavigationPointFromCurrentState()
		{
			NavigationPoint result;
			if (_listItemHandler is ProjectAllItemHandler)
			{
				result = new NavigationPoint() { Kind = NavigationPoint.KindOfNavigationPoint.AllProjectItems, Folder = null };
			}
			else if (_listItemHandler is AllWorksheetHandler)
			{
				result = new NavigationPoint() { Kind = NavigationPoint.KindOfNavigationPoint.AllTables, Folder = null };
			}
			else if (_listItemHandler is AllGraphHandler)
			{
				result = new NavigationPoint() { Kind = NavigationPoint.KindOfNavigationPoint.AllGraphs, Folder = null };
			}
			else if (_listItemHandler is SpecificProjectFolderHandler)
			{
				result = new NavigationPoint() { Kind = NavigationPoint.KindOfNavigationPoint.ProjectFolder, Folder = ((SpecificProjectFolderHandler)_listItemHandler).CurrentProjectFolder };
			}
			else
			{
				result = new NavigationPoint() { Kind = NavigationPoint.KindOfNavigationPoint.AllProjectItems, Folder = null };
			}
			return result;
		}

		private string GetLocationStringFromCurrentState()
		{
			string result;
			if (_listItemHandler is ProjectAllItemHandler)
			{
				result = "<<< All items >>>";
			}
			else if (_listItemHandler is AllWorksheetHandler)
			{
				result = "<<< All tables >>>";
			}
			else if (_listItemHandler is AllGraphHandler)
			{
				result = "<<< All graphs >>>";
			}
			else if (_listItemHandler is SpecificProjectFolderHandler)
			{
				result = ((SpecificProjectFolderHandler)_listItemHandler).CurrentProjectFolder;
				if (string.IsNullOrEmpty(result))
					result = "<<< Root folder >>>";
			}
			else
			{
				result = string.Empty;
			}
			return result;
		}

		#endregion Navigation

		private int NameComparism(SelectableListNode x, SelectableListNode y)
		{
			return string.Compare(x.Text, y.Text);
		}

		private BrowserListItem.SortKind _primaryListSortKind = BrowserListItem.SortKind.Name;
		private bool _primaryListSortDescending;
		private BrowserListItem.SortKind _secondaryListSortKind;
		private bool _secondaryListSortDescending;

		public void EhToggleListSort_Name()
		{
			EhToggleListSort(BrowserListItem.SortKind.Name);
		}

		public void EhToggleListSort_CreationDate()
		{
			EhToggleListSort(BrowserListItem.SortKind.CreationDate);
		}

		private void EhToggleListSort(BrowserListItem.SortKind clickedSort)
		{
			bool newDirection = false; // Ascending

			if (clickedSort == _primaryListSortKind) // clicked on primary sort column
			{
				_primaryListSortDescending = !_primaryListSortDescending;
			}
			else if (clickedSort == _secondaryListSortKind) // clicked on secondary column
			{
				newDirection = _secondaryListSortDescending;
				_secondaryListSortKind = _primaryListSortKind;
				_secondaryListSortDescending = _primaryListSortDescending;
				_primaryListSortDescending = newDirection;
				_primaryListSortKind = clickedSort;
			}
			else // clicked in any other column
			{
				_secondaryListSortKind = _primaryListSortKind;
				_secondaryListSortDescending = _primaryListSortDescending;

				_primaryListSortKind = clickedSort;
				_primaryListSortDescending = false;
			}

			// now sort the item list
			SortItemList(_listViewItems);

			// and indicate the sorting as arrows in the column headers of the view
			UpdateSortIndicatorsInView();
		}

		private void SortItemList(SelectableListNodeList list)
		{
			BrowserListItem.Comparer comparer = null;

			if (_secondaryListSortKind != BrowserListItem.SortKind.None)
				comparer = new BrowserListItem.Comparer(_primaryListSortKind, _primaryListSortDescending, _secondaryListSortKind, _secondaryListSortDescending);
			else
				comparer = new BrowserListItem.Comparer(_primaryListSortKind, _primaryListSortDescending);

			if (null != comparer)
				BrowserListItem.Sort(list, comparer);
		}

		private void UpdateSortIndicatorsInView()
		{
			if (null != _view)
			{
				bool isNameColSorted;
				bool isNameColDescending;
				bool isColSecondary;

				if (BrowserListItem.SortKind.Name == _primaryListSortKind)
				{
					isNameColSorted = true;
					isNameColDescending = _primaryListSortDescending;
					isColSecondary = false;
				}
				else if (BrowserListItem.SortKind.Name == _secondaryListSortKind)
				{
					isNameColSorted = true;
					isNameColDescending = _secondaryListSortDescending;
					isColSecondary = true;
				}
				else
				{
					isNameColSorted = false;
					isNameColDescending = false;
					isColSecondary = false;
				}
				_view.SetSortIndicator_NameColumn(isNameColSorted, isNameColDescending, isColSecondary);

				if (BrowserListItem.SortKind.CreationDate == _primaryListSortKind)
				{
					isNameColSorted = true;
					isNameColDescending = _primaryListSortDescending;
					isColSecondary = false;
				}
				else if (BrowserListItem.SortKind.CreationDate == _secondaryListSortKind)
				{
					isNameColSorted = true;
					isNameColDescending = _secondaryListSortDescending;
					isColSecondary = true;
				}
				else
				{
					isNameColSorted = false;
					isNameColDescending = false;
					isColSecondary = false;
				}
				_view.SetSortIndicator_CreationDateColumn(isNameColSorted, isNameColDescending, isColSecondary);
			}
		}

		#region IMVCController Members

		public object ViewObject
		{
			get
			{
				return _view;
			}
			set
			{
				if (null != _view)
				{
				}
				_view = value as IProjectBrowseView;
				if (null != _view)
				{
					Initialize(false);
				}
			}
		}

		public object ModelObject
		{
			get { return _doc; }
		}

		#endregion IMVCController Members

		#region List commands

		/// <summary>
		/// Determines if a project folder is selected in the tree view.
		/// </summary>
		/// <param name="folderName">If a project folder is selected, returns the name of the folder.</param>
		/// <returns>True if a project folder is selected in the tree view.</returns>
		public bool IsProjectFolderSelected(out string folderName)
		{
			if (_currentSelectedTreeNode.Tag is string)
			{
				folderName = ((string)_currentSelectedTreeNode.Tag);
				return true;
			}
			else
			{
				folderName = ProjectFolder.RootFolderName;
				return false;
			}
		}

		/// <summary>
		/// Gets/sets the action when selecting a list item. If true, when selection the list item, it is shown in the document area.
		/// </summary>
		public bool ViewOnSelectListNodeOn
		{
			get { return _viewOnSelectListNodeOn; }
			set { _viewOnSelectListNodeOn = value; }
		}

		/// <summary>
		/// Gets/sets the action when selecting a tree node.
		/// </summary>
		public ViewOnSelect ViewOnSelectTreeNode
		{
			get { return _viewOnSelectTreeNode; }
			set { _viewOnSelectTreeNode = value; }
		}

		#endregion List commands
	}
}