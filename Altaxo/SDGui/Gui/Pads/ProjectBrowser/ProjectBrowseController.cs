using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Altaxo.Main;
using Altaxo.Collections;

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
		void SynchronizeListSelection();
	}
	#endregion

	public class ProjectBrowseController : IMVCController
	{
		/// <summary>The gui view shown to the user.</summary>
		IProjectBrowseView _view;
		/// <summary>The current Altaxo project.</summary>
		AltaxoDocument _doc;

		/// <summary>Root node. Holds all other nodes in its nodes collection.</summary>
		NGBrowserTreeNode _rootNode;
		/// <summary>Root node of the project folders.</summary>
		NGBrowserTreeNode _projectDirectoryRoot;
		/// <summary>Project node. Equivalent to the whole project</summary>
		NGBrowserTreeNode _allItemsNode;
		/// <summary>When selected, shows all graphs in the project.</summary>
		NGBrowserTreeNode _allGraphsNode;
		/// <summary>When selected, shows all tables in the project.</summary>
		NGBrowserTreeNode _allTablesNode;

		/// <summary>Dictionary of project folders (keys) and the corresponing non-Gui nodes (values).</summary>
		Dictionary<string, NGBrowserTreeNode> _directoryNodesByName;

		/// <summary>Current selected tree node.</summary>
		NGBrowserTreeNode _currentSelectedTreeNode;

		/// <summary>Items currently shown in the list view.</summary>
		SelectableListNodeList _listViewItems = new SelectableListNodeList();
		/// <summary>Object that is responsible for filling the list with items and for tracking changes affecting the list items.</summary>
		AbstractItemHandler _listItemHandler;

		/// <summary>Action when selecting a tree node.</summary>
		ViewOnSelect _viewOnSelectTreeNode = ViewOnSelect.Off;
		/// <summary>Action when selection a list node. If true, the item is shown in the document area.</summary>
		bool _viewOnSelectListNodeOn = false;

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

		void Initialize(bool initData)
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
		static string GetDisplayNameOfFolder(string fullFolderPath)
		{
			return ProjectFolder.ConvertFolderNameToDisplayFolderName(fullFolderPath);
		}


		void RecreateDirectoryNodes()
		{
			_directoryNodesByName.Clear();
			_directoryNodesByName.Add((string)_projectDirectoryRoot.Tag, _projectDirectoryRoot);
			_projectDirectoryRoot.Nodes.Clear();

			CreateDirectoryNode((string)_projectDirectoryRoot.Tag, _projectDirectoryRoot);

			if (_view != null)
				((IGuiBrowserTreeNode)_projectDirectoryRoot.GuiTag).OnNodeMultipleChanges();


		}

		void CreateDirectoryNode(string dir, NGBrowserTreeNode node)
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

		private void EhProjectDirectoryItemChanged(Main.NamedObjectCollectionChangeType changeType, object item, string oldName, string newName)
		{
			Current.Gui.Execute(EhProjectDirectoryItemChanged_Unsynchronized, changeType, item, oldName, newName);
		}
		private void EhProjectDirectoryItemChanged_Unsynchronized(Main.NamedObjectCollectionChangeType changeType, object item, string oldName, string newName)
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

		private void EhProjectDirectoryChanged(Main.NamedObjectCollectionChangeType changeType, string dir)
		{
			Current.Gui.Execute(EhProjectDirectoryChanged_Unsynchronized, changeType, dir);
		}
		private void EhProjectDirectoryChanged_Unsynchronized(Main.NamedObjectCollectionChangeType changeType, string dir)
		{
			if (Main.ProjectFolder.IsRootFolderName(dir))
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

		#endregion

		#region List related stuff

		/// <summary>
		/// Internally sets the list item handler and wires the ListChange event.
		/// </summary>
		/// <param name="itemHandler"></param>
		void SetItemListHandler(AbstractItemHandler itemHandler)
		{
			if (null != _listItemHandler)
				_listItemHandler.ListChange -= EhListItemHandlerListChange;
			_listItemHandler = itemHandler;
			if (null != _listItemHandler)
				_listItemHandler.ListChange += EhListItemHandlerListChange;
		}

		/// <summary>
		/// Sets the ItemList handler according to the currently selected tree node.
		/// </summary>
		void RefreshItemListHandler()
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
					SetItemListHandler(new SpecificProjectFolderHandler(Main.ProjectFolder.RootFolderName));
				else if (_currentSelectedTreeNode.Tag is string)
					SetItemListHandler(new SpecificProjectFolderHandler((string)_currentSelectedTreeNode.Tag));
			}
		}


		/// <summary>
		/// Called if the list item handler announces a change in the item list.
		/// </summary>
		/// <param name="list"></param>
		void EhListItemHandlerListChange(SelectableListNodeList list)
		{
			Current.Gui.Execute(EhListItemHandlerListChange_Unsynchronized, list);
		}

		/// <summary>
		/// Called if the list item handler announces a change in the item list.
		/// </summary>
		/// <param name="list"></param>
		void EhListItemHandlerListChange_Unsynchronized(SelectableListNodeList list)
		{
			_listViewItems = list;
			//_listViewItems.Sort(NameComparism);
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
				if (item.IsSelected && !(item.Tag is ParentProjectFolder))
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
				if (!(item.Tag is ParentProjectFolder))
				{
					result.Add(item.Tag);
				}
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
				else if (node.Tag is ProjectFolder)
				{
					SetItemListHandler(new SpecificProjectFolderHandler((node.Tag as ProjectFolder).Name));
				}
				else if (node.Tag is ParentProjectFolder)
				{
					SetItemListHandler(new SpecificProjectFolderHandler((node.Tag as ParentProjectFolder).Name));
				}
			}
		}

		#endregion

		int NameComparism(SelectableListNode x, SelectableListNode y)
		{
			return string.Compare(x.Text, y.Text);
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

		#endregion



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
				folderName = Main.ProjectFolder.RootFolderName;
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

		#endregion
	}
}
