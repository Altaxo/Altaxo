#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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
using Altaxo.Gui.Workbench;
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
    ProjectBrowseController Controller { get; set; }

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

  [ExpectedTypeOfView(typeof(IProjectBrowseView))]
  public class ProjectBrowseController : AbstractPadContent
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

      _projectDirectoryRoot = new NGProjectFolderTreeNode("\\") { Image = ProjectBrowseItemImage.OpenFolder, Tag = ProjectFolder.RootFolderName, IsExpanded = true, IsSelected = true };
      _directoryNodesByName.Add((string)_projectDirectoryRoot.Tag, _projectDirectoryRoot);
      _allItemsNode.Nodes.Add(_projectDirectoryRoot);

      Current.IProjectService.ProjectChanged += this.EhProjectChanged;

      Initialize(true);
    }

    private void Initialize(bool initData)
    {
      if (initData)
      {
        EhProjectChanged(this, new ProjectEventArgs(Current.Project, Current.Project.Name, ProjectEventKind.ProjectOpened));
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

    private void EhProjectChanged(object sender, ProjectEventArgs e)
    {
      if (e.ProjectEventKind == ProjectEventKind.ProjectOpened)
        Current.Dispatcher.InvokeIfRequired(EhProjectOpened_Unsynchronized, sender, e);
      else if (e.ProjectEventKind == ProjectEventKind.ProjectClosing)
        Current.Dispatcher.InvokeIfRequired(EhProjectClosing_Unsynchronized, sender, e);
    }

    private void EhProjectOpened_Unsynchronized(object sender, ProjectEventArgs e)
    {
      if (object.ReferenceEquals(_doc, e.Project))
        return;

      if (null != _doc && null != _doc.Folders)
      {
        _doc.Folders.CollectionChanged -= this.EhProjectDirectoryItemChanged;
      }

      _doc = e.Project as AltaxoDocument;

      if (null != _doc)
      {
        _doc.Folders.CollectionChanged += this.EhProjectDirectoryItemChanged;
      }

      RecreateDirectoryNodes();
      SetItemListHandler(new SpecificProjectFolderHandler(Altaxo.Main.ProjectFolder.RootFolderName));
    }

    private void EhProjectClosing_Unsynchronized(object sender, ProjectEventArgs e)
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
    }

    private void CreateDirectoryNode(string dir, NGBrowserTreeNode node)
    {
      var subfolders = _doc.Folders.GetSubfoldersAsStringList(dir, false);
      foreach (var subfolder in subfolders)
      {
        var subnode = new NGProjectFolderTreeNode(GetDisplayNameOfFolder(subfolder))
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

    private void EhProjectDirectoryItemChanged(object sender, NamedObjectCollectionChangedEventArgs e)
    {
      Current.Dispatcher.InvokeIfRequired(EhProjectDirectoryItemChanged_Unsynchronized, e.Changes, e.Item);
    }

    private void EhProjectDirectoryItemChanged_Unsynchronized(NamedObjectCollectionChangeType changeType, object item)
    {
      if (changeType == NamedObjectCollectionChangeType.MultipleChanges)
      {
        RecreateDirectoryNodes();
      }
      else
      {
        if (item is ProjectFolder)
          EhProjectDirectoryChanged(changeType, (ProjectFolder)item);
      }
    }

    private void EhProjectDirectoryChanged(NamedObjectCollectionChangeType changeType, ProjectFolder dir)
    {
      Current.Dispatcher.InvokeIfRequired(EhProjectDirectoryChanged_Unsynchronized, changeType, dir);
    }

    private void EhProjectDirectoryChanged_Unsynchronized(NamedObjectCollectionChangeType changeType, ProjectFolder folder)
    {
      if (folder.IsRootFolder)
        return; // for the root directory, we have already the node, and we can not add or remove them

      string dir = folder.Name;

      ProjectFolder.ThrowExceptionOnInvalidFullFolderPath(dir);

      switch (changeType)
      {
        case NamedObjectCollectionChangeType.ItemAdded:
          {
            string parDir = ProjectFolder.GetFoldersParentFolder(dir);
            var parNode = _directoryNodesByName[parDir];
            var curNode = new NGProjectFolderTreeNode(GetDisplayNameOfFolder(dir))
            {
              Image = ProjectBrowseItemImage.OpenFolder,
              Tag = dir,
              ContextMenu = null == _view ? null : _view.TreeNodeContextMenu
            };
            _directoryNodesByName.Add(dir, curNode);
            parNode.Nodes.Add(curNode);
          }
          break;

        case NamedObjectCollectionChangeType.ItemRemoved:
          {
            var curNode = _directoryNodesByName[dir];
            _directoryNodesByName.Remove(dir);
            string parDir = ProjectFolder.GetFoldersParentFolder(dir);
            var parNode = _directoryNodesByName[parDir];
            parNode.Nodes.Remove(curNode);
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
          ProjectBrowserExtensions.ShowDocumentsExclusively(Current.Project.Folders.GetExpandedProjectItemSet(GetAllListItems()));
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
      Current.Dispatcher.InvokeIfRequired(EhListItemHandlerListChange_Unsynchronized, list);
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

        if (node.Tag is Altaxo.Main.Properties.ProjectFolderPropertyDocument propDoc)
        {
          var propHierarchy = new Altaxo.Main.Properties.PropertyHierarchy(PropertyExtensions.GetPropertyBags(propDoc));
          Current.Gui.ShowDialog(new object[] { propHierarchy }, "Folder properties", true);
        }
        else if (node.Tag is ProjectFolder projFolder)
        {
          SetItemListHandler(new SpecificProjectFolderHandler(projFolder.Name));
        }
        else if (node.Tag is IProjectItem projItem)
        {
          Current.ProjectService.OpenOrCreateViewContentForDocument(projItem);
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

    private void AttachView()
    {
      _view.Controller = this;
    }

    private void DetachView()
    {
      _view.Controller = null;
    }

    public override object ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
        if (!object.ReferenceEquals(_view, value))
        {
          if (null != _view)
          {
            DetachView();
          }
          _view = value as IProjectBrowseView;
          if (null != _view)
          {
            Initialize(false);
            AttachView();
          }
        }
      }
    }

    public override object ModelObject
    {
      get { return _doc; }
    }

    public override void Dispose()
    {
      ViewObject = null;
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

    #region Drag-Drop support

    private ListViewDragDropDataObject _listViewDataObject;

    /// <summary>
    /// Indicates that a drag operation from the items list can be started.
    /// </summary>
    /// <returns><c>True</c> if a drag operation from the items list can be started; otherwise <c>false</c>.</returns>
    public bool ItemList_CanStartDrag()
    {
      return GetSelectedListItems().Count > 0;
    }

    /// <summary>
    /// Indicates that a drag operation from the items list can be started.
    /// </summary>
    /// <returns><c>True</c> if a drag operation from the items list can be started; otherwise <c>false</c>.</returns>
    public bool FolderTree_CanStartDrag()
    {
      if (_currentSelectedTreeNode == null)
        return false;

      if (_currentSelectedTreeNode.Tag is string)
        return true;

      if (object.ReferenceEquals(_currentSelectedTreeNode, _allItemsNode))
        return true;

      if (object.ReferenceEquals(_currentSelectedTreeNode, _allGraphsNode))
        return true;

      if (object.ReferenceEquals(_currentSelectedTreeNode, _allTablesNode))
        return true;

      return false;
    }

    /// <summary>
    /// Starts a drag operation from the items list.
    /// </summary>
    /// <param name="dao">On return, this contains the data object used during the drag operation.</param>
    /// <param name="canCopy">On return, this variable indicates if the drag operation allows a copy operation.</param>
    /// <param name="canMove">On return, this variable indicates if the drag operation allows a move operation.</param>
    public void ItemList_StartDrag(out Altaxo.Serialization.Clipboard.IDataObject dao, out bool canCopy, out bool canMove)
    {
      canCopy = canMove = false;
      dao = null;

      var list = GetSelectedListItems();

      if (list.Count == 0)
      {
        return;
      }

      // if the items are from the same folder, but are selected from the AllItems, AllWorksheet or AllGraphs nodes, we invalidate the folderName (because then we consider the items to be rooted)
      string folderName;
      if (!IsProjectFolderSelected(out folderName))
        folderName = null;

      _listViewDataObject = new ListViewDragDropDataObject();

      _listViewDataObject.FolderName = folderName;

      _listViewDataObject.ItemList = new List<IProjectItem>(Current.Project.Folders.GetExpandedProjectItemSet(list));

      dao = _listViewDataObject;
      canCopy = canMove = true;
    }

    /// <summary>
    /// Starts a drag operation from the items list.
    /// </summary>
    /// <param name="dao">On return, this contains the data object used during the drag operation.</param>
    /// <param name="canCopy">On return, this variable indicates if the drag operation allows a copy operation.</param>
    /// <param name="canMove">On return, this variable indicates if the drag operation allows a move operation.</param>
    public void FolderTree_StartDrag(out Altaxo.Serialization.Clipboard.IDataObject dao, out bool canCopy, out bool canMove)
    {
      canCopy = canMove = false;
      dao = null;

      if (_currentSelectedTreeNode == null)
        return;

      _listViewDataObject = new ListViewDragDropDataObject();

      if (_currentSelectedTreeNode.Tag is string)
      {
        var folderName = ((string)_currentSelectedTreeNode.Tag);
        _listViewDataObject.FolderName = folderName;
        var list = Current.Project.Folders.GetItemsInFolderAndSubfolders(folderName);
        _listViewDataObject.ItemList = list;
      }
      else if (object.ReferenceEquals(_currentSelectedTreeNode, _allGraphsNode))
      {
        _listViewDataObject.ItemList = new List<IProjectItem>(Current.Project.GraphDocumentCollection);
      }
      else if (object.ReferenceEquals(_currentSelectedTreeNode, _allTablesNode))
      {
        _listViewDataObject.ItemList = new List<IProjectItem>(Current.Project.DataTableCollection);
      }
      else if (object.ReferenceEquals(_currentSelectedTreeNode, _allItemsNode))
      {
        var list = new List<IProjectItem>();
        foreach (var coll in Current.Project.ProjectItemCollections)
        {
          list.AddRange(coll.ProjectItems);
        }
        _listViewDataObject.ItemList = list;
      }
      else
      {
        return;
      }

      dao = _listViewDataObject;
      canCopy = canMove = true;
    }

    /// <summary>
    /// Called when the drag is cancelled (i.e. it was not successfull).
    /// </summary>
    public void ItemList_DragCancelled()
    {
      _listViewDataObject = null;
    }

    /// <summary>
    /// Called when the drag is cancelled (i.e. it was not successfull).
    /// </summary>
    public void FolderTree_DragCancelled()
    {
      _listViewDataObject = null;
    }

    /// <summary>
    /// Called when the drag was successfully executed.
    /// </summary>
    /// <param name="isCopy">If set to <c>true</c>, the drag-drop was a copy operation.</param>
    /// <param name="isMove">If set to <c>true</c>, the drag-drop was a move operation.</param>
    public void ItemList_DragEnded(bool isCopy, bool isMove)
    {
      if (isMove && !isCopy && _listViewDataObject != null && _listViewDataObject.ItemListWasRendered) // ItemListWasRendered is true if the items are dropped in a foreign application. If it was dropped in the same app, we have used another rendering format (rendering references).
      {
        var list = _listViewDataObject.ItemList.OfType<IProjectItem>();
        Altaxo.Main.ProjectFolders.DeleteDocuments(list, false);
      }

      _listViewDataObject = null;
    }

    /// <summary>
    /// Called when the drag was successfully executed.
    /// </summary>
    /// <param name="isCopy">If set to <c>true</c>, the drag-drop was a copy operation.</param>
    /// <param name="isMove">If set to <c>true</c>, the drag-drop was a move operation.</param>
    public void FolderTree_DragEnded(bool isCopy, bool isMove)
    {
      if (isMove && !isCopy && _listViewDataObject != null && _listViewDataObject.ItemListWasRendered) // ItemListWasRendered is true if the items are dropped in a foreign application. If it was dropped in the same app, we have used another rendering format (rendering references).
      {
        var list = _listViewDataObject.ItemList.OfType<IProjectItem>();
        Altaxo.Main.ProjectFolders.DeleteDocuments(list, false);
      }

      _listViewDataObject = null;
    }

    /// <summary>
    /// Gets the resulting drag-drop operation. It is presumed that the data object in <paramref name="dao"/> is an <see cref="T:Altaxo.Serialization.IDataObject"/>.
    /// The access to the data object is neccessary in order to determine if the drag is from a foreign application or the own application.
    /// </summary>
    /// <param name="dao">The data object used during drag-drop.</param>
    /// <param name="isCtrlPressed">Indicates whether the Ctrl-key is pressed.</param>
    /// <param name="isShiftPressed">Indicates whether the Shift-key is pressed.</param>
    /// <param name="isSameApp">Result, that if it is <c>true</c> indicates that the drag operation was originated in the same application.</param>
    /// <param name="isCopy">Return value. If true, the resulting operation is a copy operation.</param>
    /// <param name="isMove">Return value. If true, the resulting operation is a move operation.</param>
    /// <returns>The drop effect that is used (dependend on same app/foreign app, and the states of shift and ctrl key.</returns>
    private void GetResultingEffect(Altaxo.Serialization.Clipboard.IDataObject dao, bool isCtrlPressed, bool isShiftPressed, out bool isSameApp, out bool isCopy, out bool isMove)
    {
      isCopy = false;
      isMove = false;

      var sourceAppId = (string)dao.GetData(ListViewDragDropDataObject.Format_ApplicationInstanceGuid);
      isSameApp = sourceAppId == Current.ApplicationInstanceGuid.ToString();

      if (isCtrlPressed && !isShiftPressed)
        isCopy = true; // with Ctrl always copy
      else if (isShiftPressed && !isCtrlPressed)
        isMove = true;
      else
      {
        if (isSameApp)
          isMove = true;
        else
          isCopy = true;
      }
    }

    #region DropCanAcceptData

    /// <summary>
    /// Tests if the item list can accept data to be dropped here.
    /// </summary>
    /// <param name="data">The data used during the drag-drop operation.</param>
    /// <param name="isCtrlPressed">Indicates whether the Ctrl-key is pressed.</param>
    /// <param name="isShiftPressed">Indicates whether the Shift-key is pressed.</param>
    /// <param name="isCopy">Return value. If true, the resulting operation is a copy operation.</param>
    /// <param name="isMove">Return value. If true, the resulting operation is a move operation.</param>
    public void ListView_DropCanAcceptData(object data, bool isCtrlPressed, bool isShiftPressed, out bool isCopy, out bool isMove)
    {
      // if the items are from the same folder, but are selected from the AllItems, AllWorksheet or AllGraphs nodes, we invalidate the folderName (because then we consider the items to be rooted)
      string targetFolder;
      if (!IsProjectFolderSelected(out targetFolder))
        targetFolder = null;

      Both_FolderTreeAndItemList_DropCanAcceptData(data, targetFolder, isCtrlPressed, isShiftPressed, out isCopy, out isMove);
    }

    /// <summary>
    /// Tests if the item list can accept data to be dropped here.
    /// </summary>
    /// <param name="data">The data used during the drag-drop operation.</param>
    /// <param name="targetItem">The tree node that is currently selected in the folder tree (or is the target of this drop).</param>
    /// <param name="isCtrlPressed">Indicates whether the Ctrl-key is pressed.</param>
    /// <param name="isShiftPressed">Indicates whether the Shift-key is pressed.</param>
    /// <param name="isCopy">Return value. If true, the resulting operation is a copy operation.</param>
    /// <param name="isMove">Return value. If true, the resulting operation is a move operation.</param>
    public void FolderTree_DropCanAcceptData(object data, NGTreeNode targetItem, bool isCtrlPressed, bool isShiftPressed, out bool isCopy, out bool isMove)
    {
      string targetFolder;
      if (targetItem is NGProjectFolderTreeNode)
      {
        targetFolder = (string)targetItem.Tag;
        Both_FolderTreeAndItemList_DropCanAcceptData(data, targetFolder, isCtrlPressed, isShiftPressed, out isCopy, out isMove);
      }
      else
      {
        isCopy = isMove = false; // we don't allow drops which are not specific to a certain project folder. Otherwise we could drop in the white area of the TreeView, which leads to unintended drops
        return;
      }
    }

    /// <summary>
    /// Tests if the item list can accept data to be dropped here.
    /// </summary>
    /// <param name="data">The data used during the drag-drop operation.</param>
    /// <param name="targetFolder">The project folder where the drop is currently targeted.</param>
    /// <param name="isCtrlPressed">Indicates whether the Ctrl-key is pressed.</param>
    /// <param name="isShiftPressed">Indicates whether the Shift-key is pressed.</param>
    /// <param name="isCopy">Return value. If true, the resulting operation is a copy operation.</param>
    /// <param name="isMove">Return value. If true, the resulting operation is a move operation.</param>
    protected void Both_FolderTreeAndItemList_DropCanAcceptData(object data, string targetFolder, bool isCtrlPressed, bool isShiftPressed, out bool isCopy, out bool isMove)
    {
      isCopy = false;
      isMove = false;

      var dao = data as Altaxo.Serialization.Clipboard.IDataObject;

      if (null == dao)
        return;

      if (!dao.GetDataPresent(ListViewDragDropDataObject.Format_ItemList))
        return;

      if (!dao.GetDataPresent(ListViewDragDropDataObject.Format_ApplicationInstanceGuid))
        return;

      // we can accept data when
      // 1. We have different applications
      // 2. We have the same app but different folders

      bool isSameApp;
      GetResultingEffect(dao, isCtrlPressed, isShiftPressed, out isSameApp, out isCopy, out isMove);

      if (isSameApp)
      {
        string sourceFolder = (string)dao.GetData(ListViewDragDropDataObject.Format_ProjectFolder);

        // drop is not allowed if (target and sourceFolder are null or empty) or (target folder and source folder exist but are the same)
        if ((string.IsNullOrEmpty(targetFolder) && string.IsNullOrEmpty(sourceFolder)) ||
            (targetFolder != null && sourceFolder != null && targetFolder == sourceFolder))
        {
          isCopy = isMove = false;
          return;
        }
      }
    }

    #endregion DropCanAcceptData

    #region Drop

    /// <summary>
    /// Executes a drop operation into the item list.
    /// </summary>
    /// <param name="data">The data used during drag-drop.</param>
    /// <param name="isCtrlPressed">Indicates whether the Ctrl-key is pressed.</param>
    /// <param name="isShiftPressed">Indicates whether the Shift-key is pressed.</param>
    /// <param name="isCopy">Return value. If true, the resulting drop operation is a copy operation.</param>
    /// <param name="isMove">Return value. If true, the resulting drop operation is a move operation.</param>
    public void ListView_Drop(object data, bool isCtrlPressed, bool isShiftPressed, out bool isCopy, out bool isMove)
    {
      string targetFolder;
      if (!IsProjectFolderSelected(out targetFolder))
        targetFolder = null;

      BothFolderTreeAndItemList_Drop(data, targetFolder, isCtrlPressed, isShiftPressed, out isCopy, out isMove);
    }

    /// <summary>
    /// Executes a drop operation into the item list.
    /// </summary>
    /// <param name="data">The data used during drag-drop.</param>
    /// <param name="targetItem">The tree node that is currently selected in the folder tree (or is the target of this drop).</param>
    /// <param name="isCtrlPressed">Indicates whether the Ctrl-key is pressed.</param>
    /// <param name="isShiftPressed">Indicates whether the Shift-key is pressed.</param>
    /// <param name="isCopy">Return value. If true, the resulting drop operation is a copy operation.</param>
    /// <param name="isMove">Return value. If true, the resulting drop operation is a move operation.</param>
    public void FolderTree_Drop(object data, NGTreeNode targetItem, bool isCtrlPressed, bool isShiftPressed, out bool isCopy, out bool isMove)
    {
      string targetFolder;
      if (targetItem is NGProjectFolderTreeNode)
        targetFolder = (string)targetItem.Tag;
      else
        targetFolder = ProjectFolder.RootFolderName;

      BothFolderTreeAndItemList_Drop(data, targetFolder, isCtrlPressed, isShiftPressed, out isCopy, out isMove);
    }

    /// <summary>
    /// Executes a drop operation into the item list.
    /// </summary>
    /// <param name="data">The data used during drag-drop.</param>
    /// <param name="targetFolder">The Altaxo folder where the new items should be dropped to.</param>
    /// <param name="isCtrlPressed">Indicates whether the Ctrl-key is pressed.</param>
    /// <param name="isShiftPressed">Indicates whether the Shift-key is pressed.</param>
    /// <param name="isCopy">Return value. If true, the resulting drop operation is a copy operation.</param>
    /// <param name="isMove">Return value. If true, the resulting drop operation is a move operation.</param>
    public void BothFolderTreeAndItemList_Drop(object data, string targetFolder, bool isCtrlPressed, bool isShiftPressed, out bool isCopy, out bool isMove)
    {
      isCopy = isMove = false;
      var dao = data as Altaxo.Serialization.Clipboard.IDataObject;

      if (dao != null)
      {
        bool isSameApp;
        GetResultingEffect(dao, isCtrlPressed, isShiftPressed, out isSameApp, out isCopy, out isMove);

        if (isSameApp && dao.GetDataPresent(ListViewDragDropDataObject.Format_ItemReferenceList))
        {
          // if we copy or move inside the same application, we deserialize only references to the items
          var str = (string)dao.GetData(ListViewDragDropDataObject.Format_ItemReferenceList);
          var items = Altaxo.Serialization.Clipboard.ClipboardSerialization.DeserializeObjectFromString<Altaxo.Main.Commands.ProjectItemCommands.ProjectItemReferenceClipboardList>(str);
          var projectItems = new List<object>(items.ProjectItemReferences.Select(x => x.DocumentObject).Where(x => x != null));

          if (isMove && !isCopy)
          {
            Current.Project.Folders.MoveItemsToFolder(projectItems, targetFolder);
          }
          else
          {
            DocNodePathReplacementOptions relocateOptions = null;
            if (items.BaseFolder != null) // if items are from the same folder, the basefolder is set to a non-null value
            {
              var relocateData = Current.Gui.YesNoCancelMessageBox("Do you want to relocate the references in the copied plots so that they point to the destination folder?", "Question", null);
              if (null == relocateData)
                return;

              if (true == relocateData)
              {
                relocateOptions = new DocNodePathReplacementOptions();
                relocateOptions.AddPathReplacementsForAllProjectItemTypes(items.BaseFolder, targetFolder);
              }
            }

            Current.Project.Folders.CopyItemsToFolder(projectItems, targetFolder, null != relocateOptions ? relocateOptions.Visit : (DocNodeProxyReporter)null, false);
          }
        }
        else
        {
          // we have to deserialize the full items
          var str = (string)dao.GetData(ListViewDragDropDataObject.Format_ItemList);
          var items = Altaxo.Serialization.Clipboard.ClipboardSerialization.DeserializeObjectFromString<Altaxo.Main.Commands.ProjectItemCommands.ProjectItemClipboardList>(str);
          Altaxo.Main.Commands.ProjectItemCommands.PasteItems(targetFolder, items);
        }
      }
    }

    #endregion Drop

    #endregion Drag-Drop support
  }
}
