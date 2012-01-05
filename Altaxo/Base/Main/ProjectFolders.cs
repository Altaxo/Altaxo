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

using Altaxo.Main;
using Altaxo.Collections;

namespace Altaxo.Main
{
  /// <summary>
  /// Keeps track of all folder names in the project.
  /// </summary>
	public class ProjectFolders
	{
		/// <summary>The parent document for which the folder structure is kept.</summary>
		AltaxoDocument _doc;

		/// <summary>Directory dictionary. Key is the directoryname. Value is a list of objects contained in the directory.</summary>
		Dictionary<string, HashSet<object>> _directories = new Dictionary<string, HashSet<object>>();

		/// <summary>
		/// Fired if a item or a directory is added or removed. Arguments are the type of change, the item, the old name and the new name.
		/// Note that for directories the item is of type string: it is the directory name.
		/// </summary>
		public event Action<Main.NamedObjectCollectionChangeType, object, string, string> CollectionChanged;

		/// <summary>True if the events are temporarily suspended.</summary>
		private bool _suspendEvents;

		/// <summary>
		/// Creates the instance of project folders, tracking the provided Altaxo project.
		/// </summary>
		/// <param name="doc">Altaxo project.</param>
		public ProjectFolders(AltaxoDocument doc)
		{
			EhProjectOpened(this, new ProjectEventArgs(doc));
		}

		#region Access to folders and items

		/// <summary>
		/// Determines if a given folder name is present.
		/// </summary>
		/// <param name="folder">Folder name.</param>
		/// <returns>True if the given folder name is present, i.e. if at least one item belongs to the folder.</returns>
		public bool ContainsFolder(string folder)
		{
			ProjectFolder.ThrowExceptionOnInvalidFullFolderPath(folder);

			return _directories.ContainsKey(folder);
		}

		/// <summary>
		/// Get a list of subfolders of the provided folder (as string list).
		/// </summary>
		/// <param name="parentFolder">Folder for which to get the subfolders.</param>
		/// <param name="recurseSubdirectories">If true, the function returns not only the direct subfolders, but also all subfolders deeper in the hierarchy.</param>
		/// <returns>List of subfolders of the provied folder.</returns>
		public List<string> GetSubfoldersAsStringList(string parentFolder, bool recurseSubdirectories)
		{
			ProjectFolder.ThrowExceptionOnInvalidFullFolderPath(parentFolder);

			var result = new List<string>();
			InternalGetSubfoldersAsStringList(parentFolder, recurseSubdirectories, result);
			return result;
		}

		/// <summary>
		/// Get a list of subfolders of the provided folder (as string list).
		/// </summary>
		/// <param name="parentFolder">Folder for which to get the subfolders.</param>
		/// <param name="recurseSubdirectories">If true, the function returns not only the direct subfolders, but also all subfolders deeper in the hierarchy.</param>
		/// <param name="result">List that is filled with the subfolders of the provied folder.</param>
		private void InternalGetSubfoldersAsStringList(string parentFolder, bool recurseSubdirectories, List<string> result)
		{
			HashSet<object> items;
			if (!_directories.TryGetValue(parentFolder, out items))
				throw new ArgumentOutOfRangeException(string.Format("The folder {0} does not exist in this project", parentFolder));

			foreach (var v in items)
			{
				if (v is string)
				{
					result.Add((string)v);
					if(recurseSubdirectories)
						InternalGetSubfoldersAsStringList((string)v, recurseSubdirectories, result);
				}
			}
		}

		/// <summary>
		/// Get a list of subfolders of the provided folder (as ProjectFolder list).
		/// </summary>
		/// <param name="parentFolder">Folder for which to get the subfolders.</param>
		/// <returns>List of subfolders of the provied folder.</returns>
		public List<ProjectFolder> GetSubfoldersAsProjectFolderList(string parentFolder)
		{
			ProjectFolder.ThrowExceptionOnInvalidFullFolderPath(parentFolder);

			var result = new List<ProjectFolder>();

			HashSet<object> items;
			if (!_directories.TryGetValue(parentFolder, out items))
				throw new ArgumentOutOfRangeException(string.Format("The folder {0} does not exist in this project", parentFolder));

			foreach (var v in items)
			{
				if (v is string)
					result.Add(new ProjectFolder((string)v));
			}
			return result;
		}

		/// <summary>
		/// Get the items (but not the subfolders) of the provided folder.
		/// </summary>
    /// <param name="folderName">Folder for which to retrieve the items.</param>
		/// <returns>List of items (but not the subfolders) of the provided folder.</returns>
    public List<object> GetItemsInFolder(string folderName)
		{
			ProjectFolder.ThrowExceptionOnInvalidFullFolderPath(folderName);

			var result = new List<object>();

      AddItemsInFolder(folderName, result);

			return result;
		}

		/// <summary>
		/// Get the items (but not the subfolders) of the provided folder.
		/// </summary>
    /// <param name="folderName">Folder for which to retrieve the items.</param>
		/// <returns>List of items (but not the subfolders) of the provided folder.</returns>
    public List<object> GetItemsInFolderAndSubfolders(string folderName)
		{
			ProjectFolder.ThrowExceptionOnInvalidFullFolderPath(folderName);

			var result = new List<object>();
      AddItemsInFolderAndSubfolders(folderName, result);

			return result;
		}

		/// <summary>
		/// Add the items  of the provided folder (but not of the subfolders) to the list.
		/// </summary>
		/// <param name="folderName">Folder for which to retrieve the items.</param>
		/// <param name="list">List where to add the items to.</param>
    public void AddItemsInFolder(string folderName, List<object> list)
		{
			HashSet<object> items;
			if (!_directories.TryGetValue(folderName, out items))
			{
				ProjectFolder.ThrowExceptionOnInvalidFullFolderPath(folderName);
				throw new ArgumentOutOfRangeException(string.Format("The folder {0} does not exist in this project", folderName));
			}

			foreach (var v in items)
			{
				if (!(v is string))
					list.Add(v);
			}
		}

		/// <summary>
		/// Add the items  of the provided folder and of the subfolders to the list.
		/// </summary>
    /// <param name="folderName">Folder for which to retrieve the items.</param>
		/// <param name="list">List where to add the items to.</param>
    public void AddItemsInFolderAndSubfolders(string folderName, List<object> list)
		{
			HashSet<object> items;
			if (!_directories.TryGetValue(folderName, out items))
			{
				ProjectFolder.ThrowExceptionOnInvalidFullFolderPath(folderName);
				throw new ArgumentOutOfRangeException(string.Format("The folder {0} does not exist in this project", folderName));
			}

			foreach (var v in items)
			{
				if (!(v is string))
					list.Add(v);
				else
				{
					var subfolder = (string)v;
					if (_directories.ContainsKey(subfolder))
						AddItemsInFolderAndSubfolders(subfolder, list);
				}
			}
		}

		/// <summary>
		/// Rename a folder to a new name. This is done by renaming all items contained in the folder and its subfolders to a new
		/// name starting with the new folder name.
		/// </summary>
		/// <param name="oldFolderName">Name of the existing folder.</param>
		/// <param name="newFolderName">New name of the folder.</param>
		public void RenameFolder(string oldFolderName, string newFolderName)
		{
			ProjectFolder.ThrowExceptionOnInvalidFullFolderPath(oldFolderName);
			ProjectFolder.ThrowExceptionOnInvalidFullFolderPath(newFolderName);

			var items = GetItemsInFolderAndSubfolders(oldFolderName);
			MoveItemsToNewFolder(oldFolderName, newFolderName, items);
		}

		/// <summary>
		/// Move the provided items from the old folder to a new folder. This is done by renaming all items in the given list by
		/// starting with the new folder name.
		/// </summary>
		/// <param name="oldFolderName">Name of the existing folder.</param>
		/// <param name="newFolderName">New name of the folder.</param>
		/// <param name="items">List of items that should be renamed.</param>
		public void MoveItemsToNewFolder(string oldFolderName, string newFolderName, IEnumerable<object> items)
		{
			ProjectFolder.ThrowExceptionOnInvalidFullFolderPath(oldFolderName);
			ProjectFolder.ThrowExceptionOnInvalidFullFolderPath(newFolderName);

			// The algorithm tries to continuously rename items that could be renamed
			// If no more items could be renamed, the rest of the items is renamed with a unique generated name using the new name as basis

			var itemsToRename = new HashSet<object>(items);
			int oldFolderNameLength = oldFolderName == null ? 0 : oldFolderName.Length;
			var itemsRenamed = new HashSet<object>();


			for (int stage = 0; stage <= 1; stage++)
			{
				do
				{
					itemsRenamed.Clear();
					foreach (var item in itemsToRename)
					{
						if (item is INameOwner)
						{
							string oldName = ((INameOwner)item).Name;
							string newName = (newFolderName == null ? "" : newFolderName) + oldName.Substring(oldFolderNameLength);
							if (item is Data.DataTable)
							{
								if (!_doc.DataTableCollection.Contains(newName))
								{
									((Data.DataTable)item).Name = newName;
									itemsRenamed.Add(item);
								}
								else if (1 == stage)
								{
									((Data.DataTable)item).Name = _doc.DataTableCollection.FindNewTableName(newName);
									itemsRenamed.Add(item);
								}
							}
							else if (item is Graph.Gdi.GraphDocument)
							{
								if (!_doc.GraphDocumentCollection.Contains(newName))
								{
									((Graph.Gdi.GraphDocument)item).Name = newName;
									itemsRenamed.Add(item);
								}
								if (1 == stage)
								{
									((Graph.Gdi.GraphDocument)item).Name = _doc.GraphDocumentCollection.FindNewName(newName);
									itemsRenamed.Add(item);
								}
							}
							else
							{
								throw new NotImplementedException("Unknown item type encountered: " + item.GetType().ToString());
							}
						}
						else
						{
							throw new NotImplementedException("Unknown item type encountered: " + item.GetType().ToString());
						}
					} // end foreach item

					itemsToRename.ExceptWith(itemsRenamed);
				} while (itemsRenamed.Count > 0 && stage == 0);
			} // for stage
		}



		#endregion

		private void Initialize()
		{
			_suspendEvents = true;

			_directories.Clear();
			_directories.Add(ProjectFolder.RootFolderName, new HashSet<object>()); // Root folder

			foreach (var v in _doc.DataTableCollection)
				ItemAdded(v, v.Name);

			foreach (Altaxo.Graph.Gdi.GraphDocument v in _doc.GraphDocumentCollection)
				ItemAdded(v, v.Name);

			_suspendEvents = false;
			OnCollectionChanged(NamedObjectCollectionChangeType.MultipleChanges, null, null, null);
		}



		private void EhProjectOpened(object sender, ProjectEventArgs e)
		{
			if (!object.ReferenceEquals(_doc, e.Project))
			{
				_doc = e.Project;
				_doc.DataTableCollection.CollectionChanged += EhItemCollectionChanged;
				_doc.GraphDocumentCollection.CollectionChanged += EhItemCollectionChanged;
				Initialize();
			}
		}

		private void EhProjectClosed(object sender, ProjectEventArgs e)
		{
			if (null != _doc)
			{
				_doc.DataTableCollection.CollectionChanged -= EhItemCollectionChanged;
				_doc.GraphDocumentCollection.CollectionChanged -= EhItemCollectionChanged;
				_doc = null;
				_directories.Clear();
				_directories.Add(null, new HashSet<object>());

				OnCollectionChanged(NamedObjectCollectionChangeType.MultipleChanges, null, null, null);
			}
		}

		private void EhItemCollectionChanged(Main.NamedObjectCollectionChangeType changeType, object item, string oldName, string newName)
		{
			switch (changeType)
			{
				case NamedObjectCollectionChangeType.MultipleChanges:
					Initialize();
					break;
				case NamedObjectCollectionChangeType.ItemAdded:
					ItemAdded(item, newName);
					break;
				case NamedObjectCollectionChangeType.ItemRemoved:
					ItemRemoved(item, oldName);
					break;
				case NamedObjectCollectionChangeType.ItemRenamed:
					ItemRenamed(item, oldName, newName);
					break;
			}
		}



		private void ItemAdded(object item, string itemName)
		{
			string itemDir = ProjectFolder.GetFolderPart(itemName);
			DirectoryAdded(itemDir);
			_directories[itemDir].Add(item);
			OnCollectionChanged(NamedObjectCollectionChangeType.ItemAdded, item, itemName, itemName);
		}

		private void ItemRemoved(object item, string itemName)
		{
			string itemDir = ProjectFolder.GetFolderPart(itemName);
			var s = _directories[itemDir];
			s.Remove(item);
			OnCollectionChanged(NamedObjectCollectionChangeType.ItemRemoved, item, itemName, itemName);

			if (null != itemDir && 0 == s.Count)
				DirectoryRemoved(itemDir);
		}

		private void ItemRenamed(object item, string oldName, string newName)
		{
			string oldDir = ProjectFolder.GetFolderPart(oldName);
			string newDir = ProjectFolder.GetFolderPart(newName);

			if (oldDir != newDir) // only then it is neccessary to do something
			{
				ItemAdded(item, newName);
				ItemRemoved(item, oldName);
			}
			else
			{
				OnCollectionChanged(NamedObjectCollectionChangeType.ItemRenamed, item, oldName, newName);
			}
		}

		/// <summary>
		/// Adds this directory as well as all parent directories.
		/// </summary>
		/// <param name="dir"></param>
		private void DirectoryAdded(string dir)
		{
			if (!_directories.ContainsKey(dir))
			{
				_directories.Add(dir, new HashSet<object>());

				string parDir = ProjectFolder.GetFoldersParentFolder(dir);
				DirectoryAdded(parDir);
				_directories[parDir].Add(dir);

				OnCollectionChanged(NamedObjectCollectionChangeType.ItemAdded, dir, dir, dir);
			}
		}

		private void DirectoryRemoved(string dir)
		{
			if (dir==null || dir==ProjectFolder.RootFolderName)
				return;

			_directories.Remove(dir);
			string parDir = ProjectFolder.GetFoldersParentFolder(dir);
			var s = _directories[parDir];
			s.Remove(dir);
			OnCollectionChanged(NamedObjectCollectionChangeType.ItemRemoved, dir, dir, dir);

			if (null != parDir && 0 == s.Count)
				DirectoryRemoved(parDir);

		}

		protected void OnCollectionChanged(Main.NamedObjectCollectionChangeType changeType, object item, string oldName, string newName)
		{
			if (null != CollectionChanged && !_suspendEvents)
				CollectionChanged(changeType, item, oldName, newName);
		}

		/// <summary>
		/// Determines if a folder can be renamed.
		/// </summary>
		/// <param name="oldFolderName">Name of an existing folder.</param>
		/// <param name="newFolderName">New name of the folder.</param>
		/// <returns>True if the folder can be renamed, i.e. if none of the new item names conflicts with an existing item name.</returns>
		public bool CanRenameFolder(string oldFolderName, string newFolderName)
		{
			ProjectFolder.ThrowExceptionOnInvalidFullFolderPath(oldFolderName);
			ProjectFolder.ThrowExceptionOnInvalidFullFolderPath(newFolderName);


			var oldList = GetItemsInFolderAndSubfolders(oldFolderName);
			var itemHashSet = new HashSet<object>(oldList);

			int oldFolderNameLength = oldFolderName == null ? 0 : oldFolderName.Length;
			foreach (var item in oldList)
			{
				if (item is INameOwner)
				{
					string oldName = ((INameOwner)item).Name;
					string newName = (newFolderName == null ? "" : newFolderName) + oldName.Substring(oldFolderNameLength);
					if (item is Data.DataTable)
					{
						if (_doc.DataTableCollection.Contains(newName) && !itemHashSet.Contains(_doc.DataTableCollection[newName]))
							return false;
					}
					else if (item is Graph.Gdi.GraphDocument)
					{
						if (_doc.GraphDocumentCollection.Contains(newName) && !itemHashSet.Contains(_doc.GraphDocumentCollection[newName]))
							return false;
					}
					else
					{
						throw new NotImplementedException("Unknown item type encountered: " + item.GetType().ToString());
					}
				}
				else
				{
					throw new NotImplementedException("Unknown item type encountered: " + item.GetType().ToString());
				}
			}
			return true;
		}

		/// <summary>
		/// Shows a dialog to rename a folder. Note that if the root folder is specified, the function
		/// returns without showing a dialog.
		/// </summary>
		/// <param name="folder">Folder to rename.</param>
		public void ShowFolderRenameDialog(ProjectFolder folder)
		{
			ShowFolderRenameDialog(folder.Name);
		}

		/// <summary>
		/// Shows a dialog to rename a folder. Note that if the root folder is specified, the function
		/// returns without showing a dialog.
		/// </summary>
		/// <param name="folderName">Folder to rename.</param>
		public void ShowFolderRenameDialog(string folderName)
		{
			ProjectFolder.ThrowExceptionOnInvalidFullFolderPath(folderName);

			if (folderName == ProjectFolder.RootFolderName)
				return;

			var tvctrl = new Altaxo.Gui.Common.TextValueInputController(ProjectFolder.ConvertFolderNameToDisplayFolderName(folderName), "Enter the new name of the folder:");

			if (!Current.Gui.ShowDialog(tvctrl, "Rename folder", false))
				return;

			var newFolderName = ProjectFolder.ConvertDisplayFolderNameToFolderName( tvctrl.InputText.Trim() );

		

			if (!CanRenameFolder(folderName, newFolderName))
			{
				if (false == Current.Gui.YesNoMessageBox(
					"Some of the new item names conflict with existing items. Those items will be renamed with " +
					"a generated name based on the old name. Do you want to continue?", "Attention", false))
					return;
			}

			RenameFolder(folderName, newFolderName);
		}


		/// <summary>
		/// Move items in a list to a other folder.
		/// </summary>
		/// <param name="list">List of items to move. Momentarily the item types <see cref="Altaxo.Data.DataTable"/>, <see cref="Altaxo.Graph.Gdi.GraphDocument"/> and <see cref="ProjectFolder"/></param> are supported.
		/// <param name="newFolderName">Name of the folder where to move the items into.</param>
		public void MoveItemsToFolder(IList<object> list, string newFolderName)
		{
			ProjectFolder.ThrowExceptionOnInvalidFullFolderPath(newFolderName);

			foreach (object item in list)
			{
				if (item is Altaxo.Data.DataTable)
				{
					var table = (Altaxo.Data.DataTable)item;
					var newName = Main.ProjectFolder.Combine(newFolderName, Main.ProjectFolder.GetNamePart(table.Name));
					if (Current.Project.DataTableCollection.Contains(newName))
						newName = Current.Project.DataTableCollection.FindNewTableName(newName);
					table.Name = newName;
				}
				else if (item is Altaxo.Graph.Gdi.GraphDocument)
				{
					var graph = (Altaxo.Graph.Gdi.GraphDocument)item;
					string newName = Main.ProjectFolder.Combine(newFolderName, Main.ProjectFolder.GetNamePart(graph.Name));
					if (Current.Project.GraphDocumentCollection.Contains(newName))
						newName = Current.Project.GraphDocumentCollection.FindNewName(newName);
					graph.Name = newName;
				}
				else if (item is ProjectFolder)
				{
					var folder = (ProjectFolder)item;
					string moveToFolder = ProjectFolder.Combine(newFolderName, Main.ProjectFolder.GetNamePart(folder.Name));
					RenameFolder(folder.Name, moveToFolder);
				}
			}
		}


		/// <summary>
		/// Copies the items given in the list (tables, graphs and folders) to another folder, which is given by newFolderName. The copying
		/// is done by cloning the items.
		/// </summary>
		/// <param name="list">List of items to copy.</param>
		/// <param name="destinationFolderName">Destination folder name.</param>
		/// <param name="ReportProxies">If not null, this argument is used to relocate references to other items (e.g. columns) to point to the destination folder.</param>
		public void CopyItemsToFolder(IList<object> list, string destinationFolderName, DocNodeProxyReporter ReportProxies)
		{
			ProjectFolder.ThrowExceptionOnInvalidFullFolderPath(destinationFolderName);

			foreach (object item in list)
			{
				CopyItemToFolder(item, destinationFolderName, ReportProxies);
			}
		}


		/// <summary>
		/// Copyies one item to another folder by cloning the item.
		/// </summary>
		/// <param name="item">Item to copy.</param>
		/// <param name="destinationFolderName">Destination folder name.</param>
		/// <param name="ReportProxies">If not null, this argument is used to relocate references to other items (e.g. columns) to point to the destination folder.</param>
		public void CopyItemToFolder(object item, string destinationFolderName, DocNodeProxyReporter ReportProxies)
		{
			ProjectFolder.ThrowExceptionOnInvalidFullFolderPath(destinationFolderName);

			if (item == null)
				throw new ArgumentNullException("Item is null");

			if (item is ProjectFolder)
			{
				var orgName = (item as ProjectFolder).Name;
				string destName = ProjectFolder.Combine(destinationFolderName, ProjectFolder.GetNamePart(orgName));
				foreach(var subitem in this.GetItemsInFolderAndSubfolders(orgName))
				{
					var oldItemFolder = ProjectFolder.GetFolderPart(((INameOwner)subitem).Name);
					var newItemFolder = oldItemFolder.Replace(orgName, destName);
					CopyItemToFolder(subitem, newItemFolder, ReportProxies);
				}
			}
			else if ((item is ICloneable) && (item is INameOwner))
			{
				var orgName = (item as INameOwner).Name;
				var clonedItem = (INameOwner)(item as ICloneable).Clone();
				clonedItem.Name = ProjectFolder.Combine(destinationFolderName, ProjectFolder.GetNamePart(orgName));
				Current.Project.AddItem(clonedItem);

				if (null!=ReportProxies)
				{
					if (clonedItem is Altaxo.Graph.Gdi.GraphDocument)
					{
						((Altaxo.Graph.Gdi.GraphDocument)clonedItem).VisitDocumentReferences(ReportProxies);
					}
				}
			}
			else
			{
				throw new NotImplementedException(string.Format("The item of type {0} can not be copied", item.GetType()));
			}
		}


		#region Helper Gui Functions

		/// <summary>
		/// Get a list of subfolders of the provided folder (as string list).
		/// </summary>
		/// <param name="parentFolder">Folder for which to get the subfolders.</param>
		/// <param name="recurseSubdirectories">If true, the function returns not only the direct subfolders, but also all subfolders deeper in the hierarchy.</param>
		/// <returns>List of subfolders of the provied folder.</returns>
		public List<string> GetSubfoldersAsDisplayFolderNameStringList(string parentFolder, bool recurseSubdirectories)
		{
			var result = GetSubfoldersAsStringList(parentFolder, recurseSubdirectories);


			for (int i = 0; i < result.Count; ++i)
				result[i] = ProjectFolder.ConvertFolderNameToDisplayFolderName(result[i]);

			return result;
		}


		/// <summary>
		/// Add a <see cref="NGTreeNode"/>s corresponding to the folder name recursively for all parts of the folder name so that a hierarchy of those nodes is built-up. 
		/// If the folder name is already represented by a tree node (i.e. is present in the folderDictionary), this node is returned. 
		/// If not, the node is created and added to the folder dictionary as well as to the nodes collection of the parent tree node. 
		/// If the parent tree node is not found in the folderDictionary, this function is called recursively to add the parent tree node.
		/// </summary>
		/// <param name="folderName">The folder name to add.</param>
		/// <param name="folderDictionary">Dictionary that relates the full folder name to the already built-up tree nodes. At least the root node (key is the <see cref="Main.ProjectFolder.RootFolderName"/> has to be present in the dictionary. The newly created folder nodes are also added to this dictionary.</param>
		/// <returns>The tree node corresponding to the provided folder name.</returns>
		public static NGTreeNode AddFolderNodeRecursively(string folderName, Dictionary<string, NGTreeNode> folderDictionary)
		{
			NGTreeNode folderNode;
			if (!folderDictionary.TryGetValue(folderName, out folderNode))
			{
				var parentNode = AddFolderNodeRecursively(Main.ProjectFolder.GetFoldersParentFolder(folderName), folderDictionary);


				folderNode = new NGTreeNode { Text = Main.ProjectFolder.ConvertFolderNameToDisplayFolderName(Main.ProjectFolder.GetFoldersLastFolderPart(folderName)), Tag = folderName };
				folderDictionary.Add(folderName, folderNode);
				parentNode.Nodes.Add(folderNode);
			}
			return folderNode;
		}


		/// <summary>
		/// Add a <see cref="NGTreeNode"/>s corresponding to the folder name recursively for all parts of the folder name so that a hierarchy of those nodes is built-up. 
		/// If the folder name is already represented by a tree node (i.e. is present in the folderDictionary), this node is returned. 
		/// If not, the node is created and added to the folder dictionary as well as to the nodes collection of the parent tree node. 
		/// If the parent tree node is not found in the folderDictionary, this function is called recursively to add the parent tree node.
		/// </summary>
		/// <param name="folderName">The folder name to add.</param>
		/// <param name="folderDictionary">Dictionary that relates the full folder name to the already built-up tree nodes. At least the root node (key is the <see cref="Main.ProjectFolder.RootFolderName"/> has to be present in the dictionary. The newly created folder nodes are also added to this dictionary.</param>
		/// <returns>The tree node corresponding to the provided folder name.</returns>
		public static NGTreeNode AddFolderNodeRecursively<T>(string folderName, Dictionary<string, NGTreeNode> folderDictionary) where T : NGTreeNode, new()
		{
			NGTreeNode folderNode;
			if (!folderDictionary.TryGetValue(folderName, out folderNode))
			{
				var parentNode = AddFolderNodeRecursively<T>(Main.ProjectFolder.GetFoldersParentFolder(folderName), folderDictionary);


				folderNode = new T { Text = Main.ProjectFolder.ConvertFolderNameToDisplayFolderName(Main.ProjectFolder.GetFoldersLastFolderPart(folderName)), Tag = folderName };
				folderDictionary.Add(folderName, folderNode);
				parentNode.Nodes.Add(folderNode);
			}
			return folderNode;
		}

		#endregion
	}
}
