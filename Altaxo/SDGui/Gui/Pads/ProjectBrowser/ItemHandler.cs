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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Altaxo.Collections;

namespace Altaxo.Gui.Pads.ProjectBrowser
{
	/// <summary>
	/// Abstracts the handling of the list items that are currently shown. 
	/// </summary>
	public abstract class AbstractItemHandler
	{
		protected SelectableListNodeList _list;
		protected event Action<SelectableListNodeList> _listChange;

		/// <summary>
		/// Signals that the list has changed, so the view can update the list. When the first receiver registers for 
		/// the event, the function <see cref="BeginTracking"/> will be called. If the last receiver unregisters for
		/// the event, the function <see cref="EndTracking"/> will be called.
		/// </summary>
		public event Action<SelectableListNodeList> ListChange
		{
			add
			{
				bool wasEmpty = null == _listChange;
				_listChange += value;
				if (wasEmpty && null != _listChange)
					BeginTracking();
			}
			remove
			{
				bool wasEmpty = null == _listChange;
				_listChange -= value;
				if (!wasEmpty && null == _listChange)
					EndTracking();
			}
		}

		/// <summary>
		/// Fires the <see cref="ListChange"/> event.
		/// </summary>
		protected virtual void OnListChange()
		{
			if (null != _listChange)
				_listChange(_list);
		}

		/// <summary>
		/// Fills the list with items.
		/// </summary>
		/// <returns>The list of items that are shown in the listbox.</returns>
		public abstract SelectableListNodeList GetItemList();

		/// <summary>
		/// Begins monitoring of changes that can affect the items in the list.
		/// </summary>
		public abstract void BeginTracking();

		/// <summary>
		/// Ends monitoring of changes that can affect the items in the list.
		/// </summary>
		public abstract void EndTracking();
	}

	/// <summary>
	/// Shows all available items (tables, graphs, ..) in the project.
	/// </summary>
	public class ProjectAllItemHandler : AbstractItemHandler
	{
		/// <summary>
		/// Fills the list with items.
		/// </summary>
		/// <returns>The list of items that are shown in the listbox.</returns>
		public override SelectableListNodeList GetItemList()
		{
			_list = new SelectableListNodeList();
			foreach (var t in Current.Project.DataTableCollection)
				_list.Add(new BrowserListItem(t.Name, t, false) { Image = ProjectBrowseItemImage.Worksheet });
			foreach (Altaxo.Graph.Gdi.GraphDocument t in Current.Project.GraphDocumentCollection)
				_list.Add(new BrowserListItem(t.Name, t, false) { Image = ProjectBrowseItemImage.Graph });

			return _list;
		}

		/// <summary>
		/// Starts monitoring of the table and graph collection.
		/// </summary>
		public override void BeginTracking()
		{
			GetItemList();
			Current.Project.DataTableCollection.CollectionChanged += EhCollectionChanged;
			Current.Project.GraphDocumentCollection.CollectionChanged += EhCollectionChanged;
			OnListChange();
		}

		/// <summary>
		/// Ends monitoring of the table and graph collection.
		/// </summary>
		public override void EndTracking()
		{
			Current.Project.DataTableCollection.CollectionChanged -= EhCollectionChanged;
			Current.Project.GraphDocumentCollection.CollectionChanged -= EhCollectionChanged;
		}

		void EhCollectionChanged(Altaxo.Main.NamedObjectCollectionChangeType changeType, object item, string oldName, string newName)
		{
			GetItemList();
			OnListChange();
		}
	}

	/// <summary>
	/// Shows all tables in the project.
	/// </summary>
	public class AllWorksheetHandler : AbstractItemHandler
	{
		/// <summary>
		/// Fills the list with all tables in the project.
		/// </summary>
		/// <returns>List of all tables.</returns>
		public override SelectableListNodeList GetItemList()
		{
			_list = new SelectableListNodeList();
			foreach (var t in Current.Project.DataTableCollection)
				_list.Add(new BrowserListItem(t.Name, t, false) { Image = ProjectBrowseItemImage.Worksheet });

			return _list;
		}

		/// <summary>
		/// Starts monitoring of the table collection.
		/// </summary>
		public override void BeginTracking()
		{
			GetItemList();
			Current.Project.DataTableCollection.CollectionChanged += EhCollectionChanged;
			OnListChange();
		}

		/// <summary>
		/// Ends monitoring of the table collection.
		/// </summary>
		public override void EndTracking()
		{
			Current.Project.DataTableCollection.CollectionChanged -= EhCollectionChanged;
		}

		void EhCollectionChanged(Altaxo.Main.NamedObjectCollectionChangeType changeType, object item, string oldName, string newName)
		{
			GetItemList();
			OnListChange();
		}
	}

	/// <summary>
	/// Shows all graphs in the project.
	/// </summary>
	public class AllGraphHandler : AbstractItemHandler
	{
		public override SelectableListNodeList GetItemList()
		{
			_list = new SelectableListNodeList();
			foreach (Altaxo.Graph.Gdi.GraphDocument t in Current.Project.GraphDocumentCollection)
				_list.Add(new BrowserListItem(t.Name, t, false) { Image = ProjectBrowseItemImage.Graph });

			return _list;
		}

		/// <summary>
		/// Starts monitoring of the graph collection.
		/// </summary>
		public override void BeginTracking()
		{
			GetItemList();
			Current.Project.GraphDocumentCollection.CollectionChanged += EhCollectionChanged;
			OnListChange();
		}

		/// <summary>
		/// Ends monitoring of the graph collection.
		/// </summary>
		public override void EndTracking()
		{
			Current.Project.GraphDocumentCollection.CollectionChanged -= EhCollectionChanged;
		}

		void EhCollectionChanged(Altaxo.Main.NamedObjectCollectionChangeType changeType, object item, string oldName, string newName)
		{
			GetItemList();
			OnListChange();
		}
	}

	/// <summary>
	/// Shows the items in a specified project folder.
	/// </summary>
	public class SpecificProjectFolderHandler : AbstractItemHandler
	{
		string _folderName;


		/// <summary>
		/// Creates the handler.
		/// </summary>
		/// <param name="folder">The project folder for which to show the items.</param>
		public SpecificProjectFolderHandler(string folder)
		{
			_folderName = folder;
		}

		/// <summary>Gets the current project folder (or null if this list has no current project folder).</summary>
		public string CurrentProjectFolder { get { return _folderName; } }


		/// <summary>
		/// Fills the list with all items (tables, graphs, and subfolders) of the current project folder.
		/// </summary>
		/// <returns>List of items.</returns>
		public override SelectableListNodeList GetItemList()
		{
			_list = new SelectableListNodeList();
			if (!Current.Project.Folders.ContainsFolder(_folderName))
				return _list;


			if (!Altaxo.Main.ProjectFolder.IsRootFolderName(_folderName))
				_list.Add(new BrowserListItem(
					"..",
					new ParentProjectFolder(Main.ProjectFolder.GetFoldersParentFolder(_folderName)),
					false)
					{
						Image = ProjectBrowseItemImage.OpenFolder
					}
					);

			var subfolderList = Current.Project.Folders.GetSubfoldersAsStringList(_folderName, false);
			subfolderList.Sort();
			foreach (var o in subfolderList)
			{
				_list.Add(new BrowserListItem(Main.ProjectFolder.ConvertFolderNameToDisplayFolderLastPart(o), new Main.ProjectFolder(o), false) { Image = ProjectBrowseItemImage.OpenFolder });
			}

			var itemList = Current.Project.Folders.GetItemsInFolder(_folderName);
			itemList.Sort(CompareItemsByName);
			foreach (var o in itemList)
			{
				if (o is Altaxo.Data.DataTable)
				{
					var newItem = new BrowserListItem(Main.ProjectFolder.GetNamePart(((Main.INameOwner)o).Name), o, false)
					{
						Image = ProjectBrowseItemImage.Worksheet
					};
					_list.Add(newItem);
				}

				if (o is Altaxo.Graph.Gdi.GraphDocument)
				{
					var newItem = new BrowserListItem(Main.ProjectFolder.GetNamePart(((Main.INameOwner)o).Name), o, false)
					{
						Image = ProjectBrowseItemImage.Graph
					};
					_list.Add(newItem);
				}
			}

			return _list;
		}

		private static int CompareItemsByName(object a, object b)
		{
			string sa = (a is Main.INameOwner) ? ((Main.INameOwner)a).Name : string.Empty;
			string sb = (b is Main.INameOwner) ? ((Main.INameOwner)b).Name : string.Empty;
			return string.Compare(sa, sb);
		}

		/// <summary>
		/// Starts monitoring of item changes in the current project folder.
		/// </summary>
		public override void BeginTracking()
		{
			GetItemList();
			Current.Project.Folders.CollectionChanged += EhCollectionChanged;
			OnListChange();
		}

		/// <summary>
		/// Ends monitoring of item changes in the current project folder.
		/// </summary>
		public override void EndTracking()
		{
			Current.Project.Folders.CollectionChanged += EhCollectionChanged;
		}

		void EhCollectionChanged(Altaxo.Main.NamedObjectCollectionChangeType changeType, object item, string oldName, string newName)
		{
			GetItemList();
			OnListChange();
		}

	}

}
