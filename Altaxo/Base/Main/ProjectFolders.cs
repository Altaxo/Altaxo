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

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Altaxo.Collections;

namespace Altaxo.Main
{
  /// <summary>
  /// Keeps track of all folder names in the project.
  /// </summary>
  public class ProjectFolders
    : SuspendableDocumentLeafNodeWithSetOfEventArgs
  {
    /// <summary>The parent document for which the folder structure is kept.</summary>
    private IProject AltaxoDocument { get { return (IProject)_parent!; } }

    /// <summary>Directory dictionary. Key is the directoryname. Value is a list of objects contained in the directory.</summary>
    private Dictionary<string, HashSet<object>> _directories = new Dictionary<string, HashSet<object>>();

    /// <summary>
    /// Fired if a item or a directory is added or removed. Arguments are the type of change, the item, the old name and the new name.
    /// Note that for directories the item is of type string: it is the directory name.
    /// </summary>
    public event EventHandler<Main.NamedObjectCollectionChangedEventArgs>? CollectionChanged;

    /// <summary>
    /// Creates the instance of project folders, tracking the provided Altaxo project.
    /// </summary>
    /// <param name="doc">Altaxo project.</param>
    public ProjectFolders(IProject doc)
    {
      if (doc is null)
        throw new ArgumentNullException(nameof(doc));

      foreach (var coll in doc.ProjectItemCollections)
        coll.CollectionChanged += EhItemCollectionChanged;
      Initialize(doc);

      _parent = doc; // Parent last because we dont want the changes above to be monitored by the parent
    }

    protected override void Dispose(bool isDisposing)
    {
      var doc = AltaxoDocument;
      if (doc is not null)
      {
        foreach (var coll in doc.ProjectItemCollections)
        {
          if (coll is not null)
          {
            coll.CollectionChanged += EhItemCollectionChanged;
          }
        }
      }

      base.Dispose(isDisposing);
    }

    public override IDocumentNode? ParentObject
    {
      get
      {
        return base.ParentObject;
      }
      set
      {
        if (value is not null)
          throw new InvalidOperationException("The parent is set in the constructor only, thus can not be changed here.");

        base.ParentObject = value;
      }
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
      if (!_directories.TryGetValue(parentFolder, out var items))
        throw new ArgumentOutOfRangeException(string.Format("The folder {0} does not exist in this project", parentFolder));

      foreach (var v in items)
      {
        if (v is string)
        {
          result.Add((string)v);
          if (recurseSubdirectories)
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

      if (!_directories.TryGetValue(parentFolder, out var items))
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
    /// <param name="throwIfFolderDoesNotExists">If true, and the provided folder does not exist, a <see cref="ArgumentOutOfRangeException"/></param> is thrown.
    /// <returns>List of items (but not the subfolders) of the provided folder.</returns>
    public List<IProjectItem> GetItemsInFolder(string folderName, bool throwIfFolderDoesNotExists = false)
    {
      ProjectFolder.ThrowExceptionOnInvalidFullFolderPath(folderName);
      var isFolderExisting = ContainsFolder(folderName);
      if (throwIfFolderDoesNotExists && !isFolderExisting)
        throw new ArgumentOutOfRangeException($"The folder {folderName} does not exist in this project");


      var result = new List<IProjectItem>();
      if (isFolderExisting)
      {
        AddItemsInFolder(folderName, result);
      }
      return result;
    }

    /// <summary>
    /// Get the items and the subfolders (as objects of type <see cref="ProjectFolder"/>) of the provided folder.
    /// The items in the subfolders are not included.
    /// </summary>
    /// <param name="folderName">Folder for which to retrieve the items.</param>
    /// <param name="throwIfFolderDoesNotExists">If true, and the provided folder does not exist, a <see cref="ArgumentOutOfRangeException"/></param> is thrown.
    /// <returns>List of project items and subfolder objects of the provided folder.</returns>
    public List<INamedObject> GetItemsAndSubfoldersInFolder(string folderName, bool throwIfFolderDoesNotExists = false)
    {
      ProjectFolder.ThrowExceptionOnInvalidFullFolderPath(folderName);
      var isFolderExisting = ContainsFolder(folderName);
      if (throwIfFolderDoesNotExists && !isFolderExisting)
        throw new ArgumentOutOfRangeException($"The folder {folderName} does not exist in this project");

      var result = new List<INamedObject>();
      if (isFolderExisting)
      {
        AddItemsAndSubFoldersInFolder(folderName, result);
      }
      return result;
    }

    /// <summary>
    /// Get the items (but not the subfolders) of the provided folder.
    /// </summary>
    /// <param name="folderName">Folder for which to retrieve the items.</param>
    /// <param name="throwIfFolderDoesNotExists">If true, and the provided folder does not exist, a <see cref="ArgumentOutOfRangeException"/></param> is thrown.
    /// <returns>List of items (but not the subfolders) of the provided folder.</returns>
    public List<IProjectItem> GetItemsInFolderAndSubfolders(string folderName, bool throwIfFolderDoesNotExists = false)
    {
      ProjectFolder.ThrowExceptionOnInvalidFullFolderPath(folderName);

      var isFolderExisting = ContainsFolder(folderName);
      if (throwIfFolderDoesNotExists && !isFolderExisting)
        throw new ArgumentOutOfRangeException($"The folder {folderName} does not exist in this project");


      var result = new List<IProjectItem>();

      if (isFolderExisting)
      {
        AddItemsInFolderAndSubfolders(folderName, result);
      }

      return result;
    }

    /// <summary>
    /// Get the items (but not the subfolders) of the subfolders of the provided folder.
    /// Note that the items contained in the provided folder are <b>not</b> included.
    /// </summary>
    /// <param name="folderName">Folder for which to retrieve the items.</param>
    /// <returns>List of items (but not the subfolders) of the provided folder.</returns>
    public List<IProjectItem> GetItemsInSubfolders(string folderName)
    {
      ProjectFolder.ThrowExceptionOnInvalidFullFolderPath(folderName);

      var result = new List<IProjectItem>();
      AddItemsInSubfolders(folderName, result);

      return result;
    }

    /// <summary>
    /// Add the items  of the provided folder (but not of the subfolders) to the list.
    /// </summary>
    /// <param name="folderName">Folder for which to retrieve the items.</param>
    /// <param name="list">List where to add the items to.</param>
    public void AddItemsInFolder(string folderName, ICollection<IProjectItem> list)
    {
      if (!_directories.TryGetValue(folderName, out var items))
      {
        ProjectFolder.ThrowExceptionOnInvalidFullFolderPath(folderName);
        throw new ArgumentOutOfRangeException(string.Format("The folder {0} does not exist in this project", folderName));
      }

      foreach (var v in items.OfType<IProjectItem>())
      {
        list.Add(v);
      }
    }

    /// <summary>
    /// Add the project items and the subfolders (of type <see cref="ProjectFolder"/>) that are in the provided folder (but not the items of the subfolders) to the list.
    /// </summary>
    /// <param name="folderName">Folder for which to retrieve the items.</param>
    /// <param name="list">List where to add the items to.</param>
    public void AddItemsAndSubFoldersInFolder(string folderName, ICollection<INamedObject> list)
    {
      if (!_directories.TryGetValue(folderName, out var items))
      {
        ProjectFolder.ThrowExceptionOnInvalidFullFolderPath(folderName);
        throw new ArgumentOutOfRangeException(string.Format("The folder {0} does not exist in this project", folderName));
      }

      foreach (var v in items.OfType<IProjectItem>())
      {
        list.Add(v);
      }

      foreach (var v in items.OfType<string>())
      {
        list.Add(new ProjectFolder(v));
      }
    }

    /// <summary>
    /// Add the items  of the provided folder and of the subfolders to the list.
    /// </summary>
    /// <param name="folderName">Folder for which to retrieve the items.</param>
    /// <param name="list">List where to add the items to.</param>
    public void AddItemsInFolderAndSubfolders(string folderName, ICollection<IProjectItem> list)
    {
      AddItemsInFolderAndSubfolders(folderName, list, true);
    }

    /// <summary>
    /// Add the items of the subfolders of the provided folder to the list.
    /// The items in the provided folder itself are <b>not</b> added to the list.
    /// </summary>
    /// <param name="folderName">Folder for which to retrieve the items.</param>
    /// <param name="list">List where to add the items to.</param>
    public void AddItemsInSubfolders(string folderName, ICollection<IProjectItem> list)
    {
      AddItemsInFolderAndSubfolders(folderName, list, false);
    }

    /// <summary>
    /// Add the items  of the provided folder and of the subfolders to the list.
    /// </summary>
    /// <param name="folderName">Folder for which to retrieve the items.</param>
    /// <param name="list">List where to add the items to.</param>
    /// <param name="includeItemsInFolder">If true, the items in the provided folder are included, too. If false, only items in subfolders of the provided folder are included.</param>
    public void AddItemsInFolderAndSubfolders(string folderName, ICollection<IProjectItem> list, bool includeItemsInFolder)
    {
      if (!_directories.TryGetValue(folderName, out var items))
      {
        ProjectFolder.ThrowExceptionOnInvalidFullFolderPath(folderName);
        throw new ArgumentOutOfRangeException(string.Format("The folder {0} does not exist in this project", folderName));
      }

      foreach (var v in items)
      {
        if (v is IProjectItem)
        {
          if (includeItemsInFolder)
          {
            list.Add((IProjectItem)v);
          }
        }
        else if (v is string)
        {
          var subfolder = (string)v;
          if (_directories.ContainsKey(subfolder))
            AddItemsInFolderAndSubfolders(subfolder, list, true);
        }
        else
        {
          throw new InvalidOperationException(string.Format("The folder {0} contains an item {1}, which is not considered here. Please report this exception to the forum.", folderName, v));
        }
      }
    }

    /// <summary>
    /// Transforms an enumeration that contains both project items and project folders to another set that contains the project items of the enumeration plus all project items in the project folders and its subfolders.
    /// It is guaranteed that each project item is contained only once.
    /// </summary>
    /// <param name="itemList">The enumeration that can contain both project items and project folders.</param>
    /// <returns>A hashset that contains the project items of the provided <paramref name="itemList"/> plus all project items of the folders and its subfolders, that are also contained in the <paramref name="itemList"/>.</returns>
    /// <exception cref="System.NotImplementedException"></exception>
    public HashSet<IProjectItem> GetExpandedProjectItemSet(IEnumerable<object> itemList)
    {
      var result = new HashSet<IProjectItem>();
      foreach (var obj in itemList)
      {
        if (obj is IProjectItem)
        {
          result.Add((IProjectItem)obj);
        }
        else if (obj is ProjectFolder)
        {
          var folder = (ProjectFolder)obj;
          AddItemsInFolderAndSubfolders(folder.Name, result);
        }
        else
        {
          throw new NotImplementedException(string.Format("The processing of item <<{0}>> is not implemented here. Please report this error to the forum.", obj));
        }
      }
      return result;
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
    public void MoveItemsToNewFolder(string oldFolderName, string newFolderName, IEnumerable<IProjectItem> items)
    {
      ProjectFolder.ThrowExceptionOnInvalidFullFolderPath(oldFolderName);
      ProjectFolder.ThrowExceptionOnInvalidFullFolderPath(newFolderName);

      // The algorithm tries to continuously rename items that could be renamed
      // If no more items could be renamed, the rest of the items is renamed with a unique generated name using the new name as basis

      // Suspend all items
      var suspendTokensOfProjectItems = items.Select(item => item.SuspendGetToken()).ToArray();

      var itemList = new List<IProjectItem>(items);
      SortItemsByDependencies(itemList); // items that have no dependencies should now be first in the list (thus also the first to be renamed)

      // first, we rename all items to unique names

      var oldNameDictionary = new Dictionary<IProjectItem, string>(); // stores the old names of the items
      foreach (var item in itemList)
      {
        oldNameDictionary[item] = item.Name;
        item.Name = Guid.NewGuid().ToString() + "\\"; // this name should be new, the backslash is to allow also folder property documents to be renamed.
      }

      int oldFolderNameLength = oldFolderName is null ? 0 : oldFolderName.Length;
      var itemsRenamed = new HashSet<IProjectItem>();

      foreach (var item in itemList)
      {
        string oldName = oldNameDictionary[item];
        string newName = (newFolderName is null ? "" : newFolderName) + oldName.Substring(oldFolderNameLength);

        if (item is Main.Properties.ProjectFolderPropertyDocument propDoc)
        {
          var coll = AltaxoDocument.ProjectFolderProperties;
          if (!coll.ContainsAnyName(newName))
          {
            item.Name = newName;
          }
          else // we integrate the properties in the other properties
          {
            var oldProps = coll[newName].PropertyBagNotNull;
            var propsToMerge = propDoc.PropertyBagNotNull;
            oldProps.MergePropertiesFrom(propsToMerge, false);
          }
        }
        else // normal case
        {
          var coll = AltaxoDocument.GetCollectionForProjectItemType(item.GetType());
          if (!coll.ContainsAnyName(newName))
          {
            item.Name = newName;
          }
          else
          {
            item.Name = coll.FindNewItemName(newName);
          }
        }
      } // end foreach item

      // Resume all items
      suspendTokensOfProjectItems.ForEachDo(token => token.Dispose());
    }

    /// <summary>
    /// Delete the items given in the list (tables and graphs), with a confirmation dialog.
    /// </summary>
    /// <param name="list">List of items to delete.</param>
    public static void DeleteDocuments(IEnumerable<IProjectItem> list)
    {
      DeleteDocuments(list, true);
    }

    /// <summary>
    /// Delete the items given in the list (tables and graphs), with a confirmation dialog.
    /// </summary>
    /// <param name="list">List of items to delete.</param>
    /// <param name="showConfirmationDialog">If <c>true</c>, shows the confirmation dialog to confirm that the items should really be deleted.</param>
    public static void DeleteDocuments(IEnumerable<IProjectItem> list, bool showConfirmationDialog)
    {
      if (showConfirmationDialog)
      {
        if (false == Current.Gui.YesNoMessageBox(string.Format("Are you sure to delete {0} items?", list.Count()), "Attention!", false))
          return;
      }

      foreach (object item in list)
      {
        if (item is IProjectItem)
          Current.IProjectService.DeleteDocument((IProjectItem)item, true);
      }
    }

    #endregion Access to folders and items

    private void Initialize(IProject doc)
    {
      _directories.Clear();
      _directories.Add(ProjectFolder.RootFolderName, new HashSet<object>()); // Root folder

      foreach (var coll in doc.ProjectItemCollections)
      {
        foreach (var v in coll.ProjectItems)
        {
          ItemAdded(v, v.Name, EventFiring.Suppressed);
        }
      }

      EhSelfChanged(Main.NamedObjectCollectionChangedEventArgs.FromMultipleChanges());
    }

    private void EhItemCollectionChanged(object? sender, Main.NamedObjectCollectionChangedEventArgs e)
    {
      if (e.WasMultipleItemsChanged)
      {
        Initialize(AltaxoDocument);
        return;
      }

      var item = e.Item as IProjectItem;
      if (item is null)
        throw new InvalidProgramException(string.Format("Item should be a project item, since we bind to the CollectionChanged events. But current item is {0}", e.Item));

      if (e.WasItemAdded)
      {
        ItemAdded(item, e.NewName, EventFiring.Enabled);
      }
      else if (e.WasItemRemoved)
      {
        ItemRemoved(item, e.OldName ?? string.Empty, EventFiring.Enabled);
      }

      if (e.WasItemRenamed)
      {
        ItemRenamed(item, e.OldName ?? string.Empty, e.NewName, EventFiring.Enabled);
      }
    }

    private void ItemAdded(IProjectItem item, string itemName, EventFiring firing)
    {
      string itemDir = ProjectFolder.GetFolderPart(itemName);
      DirectoryAdded(itemDir);
      _directories[itemDir].Add(item);

      if (firing == EventFiring.Enabled)
        EhSelfChanged(Main.NamedObjectCollectionChangedEventArgs.FromItemAdded(item));
    }

    private void ItemRemoved(
      IProjectItem item,
      string itemName, // itemName is neccessary because we misuse ItemRemove sometimes when the item is renamed
      EventFiring firing)
    {
      string itemDir = ProjectFolder.GetFolderPart(itemName);

      int remainingCount = int.MaxValue;
      if (_directories.ContainsKey(itemDir)) // it can happen that the directory is already removed, thus we test first if there still is the directory
      {
        var s = _directories[itemDir];
        s.Remove(item);
        remainingCount = s.Count;
      }

      if (firing == EventFiring.Enabled)
        EhSelfChanged(Main.NamedObjectCollectionChangedEventArgs.FromItemRemoved(item, itemName));

      if (itemDir is not null && 0 == remainingCount)
        DirectoryRemoved(itemDir);
    }

    private void ItemRenamed(IProjectItem item, string oldName, string newName, EventFiring firing)
    {
      string oldDir = ProjectFolder.GetFolderPart(oldName);
      string newDir = ProjectFolder.GetFolderPart(newName);

      if (oldDir != newDir) // only then it is neccessary to do something
      {
        ItemAdded(item, newName, firing); // to do it in this order (first add, then remove) helps to prevent unneccessary removal of folders and then re-creation
        ItemRemoved(item, oldName, firing);
      }
      else
      {
        if (firing == EventFiring.Enabled)
          EhSelfChanged(Main.NamedObjectCollectionChangedEventArgs.FromItemRenamed(item, oldName));
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

        EhSelfChanged(Main.NamedObjectCollectionChangedEventArgs.FromItemAdded(new ProjectFolder(dir)));
      }
    }

    private void DirectoryRemoved(string dir)
    {
      if (dir is null || dir == ProjectFolder.RootFolderName)
        return;

      _directories.Remove(dir);
      string parDir = ProjectFolder.GetFoldersParentFolder(dir);
      var s = _directories[parDir];
      s.Remove(dir);
      EhSelfChanged(Main.NamedObjectCollectionChangedEventArgs.FromItemRemoved(new ProjectFolder(dir)));

      if (parDir is not null && 0 == s.Count)
        DirectoryRemoved(parDir);
    }

    protected override void OnChanged(EventArgs e)
    {
      var eAsNOCC = e as Main.NamedObjectCollectionChangedEventArgs;
      if (eAsNOCC is not null)
      {
        OnCollectionChanged(eAsNOCC);
      }

      base.OnChanged(e);
    }

    protected void OnCollectionChanged(Main.NamedObjectCollectionChangedEventArgs args)
    {
      var ev = CollectionChanged;
      if (ev is not null)
        ev(this, args);
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

      int oldFolderNameLength = oldFolderName is null ? 0 : oldFolderName.Length;
      foreach (var item in oldList)
      {
        if (item is IProjectItem projItem)
        {
          string oldName = projItem.Name;
          string newName = (newFolderName ?? string.Empty) + oldName.Substring(oldFolderNameLength);

          var coll = AltaxoDocument.GetCollectionForProjectItemType(projItem.GetType());
          if (coll.ContainsAnyName(newName) && !itemHashSet.Contains(coll[newName]))
            return false;
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

      var newFolderName = ProjectFolder.ConvertDisplayFolderNameToFolderName(tvctrl.InputText.Trim());

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
    /// Validator for the name of a new folder, i.e. a folder that don't exist up to now.
    /// </summary>
    public class FolderIsNewValidator : Altaxo.Gui.Common.TextValueInputController.NonEmptyStringValidator
    {
      public FolderIsNewValidator()
        : base("The folder name you provided is empty!")
      {
      }

      public override string? Validate(string name)
      {
        string? err = base.Validate(name);
        if (err is not null)
          return err;

        if (!name.EndsWith(ProjectFolder.DirectorySeparatorString))
          name += ProjectFolder.DirectorySeparatorString;

        return Current.IProjectService.CurrentProject.Folders.ContainsFolder(name) ? "The folder name already exists! Please enter a name of a new folder." : null;
      }
    }

    #region Sorting of project items by dependencies

    /// <summary>
    /// Sorts the project items by dependencies. On return, the item with which has no dependencies is located at the beginning of the list. The item with the most dependencies on other items in the list is the last item in the list.
    /// </summary>
    /// <param name="list">The list to sort.</param>
    public void SortItemsByDependencies(List<IProjectItem> list)
    {
      var dependencies = new Dictionary<IProjectItem, HashSet<IProjectItem>>(); // key is the project item, value is the set of items this project item is dependent on

      foreach (var item in list)
      {
        dependencies.Add(item, new HashSet<IProjectItem>());
        DocNodeProxyReporter reporter = (proxy, owner, propertyName) => SortItemsByDependencyProxyReporter(proxy, owner, propertyName, dependencies[item]);
        item.VisitDocumentReferences(reporter);
      }

      list.Sort((item1, item2) => SortItemsByDependencyComparison(item1, item2, dependencies));
    }

    private int SortItemsByDependencyComparison(IProjectItem item1, IProjectItem item2, Dictionary<IProjectItem, HashSet<IProjectItem>> dependencies)
    {
      var item1Dependencies = dependencies[item1];
      var item2Dependencies = dependencies[item2];

      if (item1Dependencies.Contains(item2))
      {
        return item2Dependencies.Contains(item1) ? 0 : 1;
      }
      else // item1Dependencies does'nt contain item2
      {
        return item2Dependencies.Contains(item1) ? -1 : 0;
      }
    }

    private void SortItemsByDependencyProxyReporter(IProxy proxy, object owner, string propertyName, HashSet<IProjectItem> dependenciesOfItem)
    {
      if (proxy?.DocumentObject() is IDocumentLeafNode proxyDoc)
      {
        var dependentOnItem = AbsoluteDocumentPath.GetRootNodeImplementing<IProjectItem>(proxyDoc);
        if (dependentOnItem is not null)
          dependenciesOfItem.Add(dependentOnItem);
      }
    }

    #endregion Sorting of project items by dependencies

    private void GetNewNamesForMoveItemsToFolderOperation(IList<object> itemList, string newFolderName, Dictionary<object, string> renameDictionary)
    {
      ProjectFolder.ThrowExceptionOnInvalidFullFolderPath(newFolderName);

      foreach (object item in itemList)
      {
        if (item is ProjectFolder projFolder)
        {
          string moveToFolder = ProjectFolder.Combine(newFolderName, Main.ProjectFolder.GetFoldersLastFolderPart(projFolder.Name));
          RenameFolder(projFolder.Name, moveToFolder);
        }
        else if (item is Altaxo.Main.Properties.ProjectFolderPropertyDocument propDoc)
        {
          string newName = Main.ProjectFolder.Combine(newFolderName, Main.ProjectFolder.GetNamePart(propDoc.Name));
          if (AltaxoDocument.ProjectFolderProperties.Contains(newName))
          {
            // Project folders are unique for the specific folder, we can not simply rename it to another name
            // Thus I decided here to merge the moved property bag with the already existing property bag
            var existingDoc = AltaxoDocument.ProjectFolderProperties[newName];
            existingDoc.PropertyBagNotNull.MergePropertiesFrom(propDoc.PropertyBagNotNull, true);
          }
          else
          {
            propDoc.Name = newName;
          }
        }
        else if (item is IProjectItem projItem)
        {
          var coll = AltaxoDocument.GetCollectionForProjectItemType(item.GetType());
          var newName = Main.ProjectFolder.Combine(newFolderName, Main.ProjectFolder.GetNamePart(projItem.Name));
          if (coll.ContainsAnyName(newName))
            newName = coll.FindNewItemName(newName);
          projItem.Name = newName;
        }
      }
    }

    /// <summary>
    /// Move items in a list to another folder.
    /// </summary>
    /// <param name="list">List of items to move.</param> 
    /// <param name="newFolderName">Name of the folder where to move the items into.</param>
    public void MoveItemsToFolder(IList<object> list, string newFolderName)
    {
      ProjectFolder.ThrowExceptionOnInvalidFullFolderPath(newFolderName);

      foreach (object item in list)
      {
        if (item is ProjectFolder projFolder)
        {
          string moveToFolder = ProjectFolder.Combine(newFolderName, Main.ProjectFolder.GetFoldersLastFolderPart(projFolder.Name));
          RenameFolder(projFolder.Name, moveToFolder);
        }
        else if (item is Altaxo.Main.Properties.ProjectFolderPropertyDocument propDoc)
        {
          string newName = Main.ProjectFolder.Combine(newFolderName, Main.ProjectFolder.GetNamePart(propDoc.Name));
          if (AltaxoDocument.ProjectFolderProperties.Contains(newName))
          {
            // Project folders are unique for the specific folder, we can not simply rename it to another name
            // Thus I decided here to merge the moved property bag with the already existing property bag
            var existingDoc = AltaxoDocument.ProjectFolderProperties[newName];
            existingDoc.PropertyBagNotNull.MergePropertiesFrom(propDoc.PropertyBagNotNull, true);
          }
          else
          {
            propDoc.Name = newName;
          }
        }
        else if (item is IProjectItem projItem)
        {
          var coll = AltaxoDocument.GetCollectionForProjectItemType(item.GetType());
          var newName = Main.ProjectFolder.Combine(newFolderName, Main.ProjectFolder.GetNamePart(projItem.Name));
          if (coll.ContainsAnyName(newName))
            newName = coll.FindNewItemName(newName);
          projItem.Name = newName;
        }
      }
    }

    /// <summary>
    /// Copies project items from a source folder to a destination folder.
    /// </summary>
    /// <param name="sourceFolderName">Name of the source folder.</param>
    /// <param name="destinationFolderName">Name of the destination folder.</param>
    /// <param name="includeSubfolders">If set to <c>true</c>, items in subfolders are included in the operation, too.</param>
    /// <param name="relocateReferences">If set to <c>true</c>, the references inbetween the copied items are maintained.</param>
    /// <param name="overwriteExistingItemsOfSameType">If set to <c>true</c>, existing items of same type will be overwritten. If set to <c>false</c>, and an items of same type and name already exists, a new name is given to the copied item.</param>
    /// <param name="throwIfFolderDoesNotExists">If true, and the provided folder does not exist, a <see cref="ArgumentOutOfRangeException"/></param> is thrown.
    public void CopyItemsFromFolderToFolder(string sourceFolderName, string destinationFolderName, bool includeSubfolders = true, bool relocateReferences = true, bool overwriteExistingItemsOfSameType = false, bool throwIfFolderDoesNotExists = true)
    {
      var items = includeSubfolders ? new List<object>(GetItemsAndSubfoldersInFolder(sourceFolderName, throwIfFolderDoesNotExists)) : new List<object>(GetItemsInFolder(sourceFolderName, throwIfFolderDoesNotExists));

      DocNodePathReplacementOptions? relocateOptions = null;
      if (relocateReferences)
      {
        relocateOptions = new Altaxo.Main.DocNodePathReplacementOptions();
        relocateOptions.AddPathReplacementsForAllProjectItemTypes(sourceFolderName, destinationFolderName);
      }
      CopyItemsToFolder(items, destinationFolderName, relocateOptions is null ? null : relocateOptions.Visit, overwriteExistingItemsOfSameType: overwriteExistingItemsOfSameType);
    }

    /// <summary>
    /// Moves project items from a source folder to a destination folder.
    /// </summary>
    /// <param name="sourceFolderName">Name of the source folder.</param>
    /// <param name="destinationFolderName">Name of the destination folder.</param>
    /// <param name="includeSubfolders">If set to <c>true</c>, items in subfolders are included in the operation, too.</param>
    /// <param name="throwIfFolderDoesNotExists">If true, and the provided folder does not exist, a <see cref="ArgumentOutOfRangeException"/></param> is thrown.
    public void MoveItemsFromFolderToFolder(string sourceFolderName, string destinationFolderName, bool includeSubfolders = true, bool throwIfFolderDoesNotExists = true)
    {
      var items = includeSubfolders ? GetItemsInFolderAndSubfolders(sourceFolderName, throwIfFolderDoesNotExists) : GetItemsInFolder(sourceFolderName, throwIfFolderDoesNotExists);
      MoveItemsToNewFolder(sourceFolderName, destinationFolderName, items);
    }

    /// <summary>
    /// Deletes all project items in a folder.
    /// </summary>
    /// <param name="sourceFolderName">Name of folder in which the items should be deleted.</param>
    /// <param name="includeSubfolders">If set to <c>true</c>, the items in all subfolders are also deleted.</param>
    /// <param name="throwIfFolderDoesNotExists">If true, and the provided folder does not exist, a <see cref="ArgumentOutOfRangeException"/></param> is thrown.
    /// <param name="showConfirmationDialog">If <c>true</c>, shows the confirmation dialog to confirm that the items should really be deleted.</param>
    /// <returns>True if any item was deleted; otherwise, false.</returns>
    public bool DeleteItemsInFolder(string sourceFolderName, bool includeSubfolders = true, bool throwIfFolderDoesNotExists = false, bool showConfirmationDialog = true)
    {
      var items = includeSubfolders ? GetItemsInFolderAndSubfolders(sourceFolderName, throwIfFolderDoesNotExists) : GetItemsInFolder(sourceFolderName, throwIfFolderDoesNotExists);
      var isAny = items.Count > 0;
      DeleteDocuments(items, showConfirmationDialog: showConfirmationDialog);
      return isAny;
    }

    /// <summary>
    /// Deletes the folder, that means, this operation deletes the project items in the given folder and in all subfolders.
    /// </summary>
    /// <param name="sourceFolderName">Name of the source folder.</param>
    /// <param name="throwIfFolderDoesNotExists">If true, and the provided folder does not exist, a <see cref="ArgumentOutOfRangeException"/></param> is thrown.
    /// <param name="showConfirmationDialog">If <c>true</c>, shows the confirmation dialog to confirm that the items should really be deleted.</param>
    /// <returns>True if any item was deleted; otherwise, false.</returns>
    public bool DeleteFolder(string sourceFolderName, bool throwIfFolderDoesNotExists = false, bool showConfirmationDialog = true)
    {
      return DeleteItemsInFolder(sourceFolderName, includeSubfolders: true, throwIfFolderDoesNotExists: throwIfFolderDoesNotExists, showConfirmationDialog: showConfirmationDialog);
    }

    /// <summary>
    /// Copies the items given in the list (tables, graphs and folders) to another folder, which is given by newFolderName. The copying
    /// is done by cloning the items.
    /// </summary>
    /// <param name="list">List of items to copy.</param>
    /// <param name="destinationFolderName">Destination folder name.</param>
    /// <param name="ReportProxies">If not null, this argument is used to relocate references to other items (e.g. columns) to point to the destination folder.</param>
    /// <param name="overwriteExistingItemsOfSameType">If true, any item with the same name and same type will be replaced by the copied item. (if false, a new name is found for the copied item which not conflicts with the existing items).</param>
    public void CopyItemsToFolder(IList<object> list, string destinationFolderName, DocNodeProxyReporter? ReportProxies, bool overwriteExistingItemsOfSameType)
    {
      ProjectFolder.ThrowExceptionOnInvalidFullFolderPath(destinationFolderName);

      foreach (object item in list)
      {
        CopyItemToFolder(item, destinationFolderName, ReportProxies, overwriteExistingItemsOfSameType);
      }
    }

    /// <summary>
    /// Copyies one item to another folder by cloning the item.
    /// </summary>
    /// <param name="item">Item to copy. Has to be either a <see cref="ProjectFolder"/>, or a project item (<see cref="IProjectItem"/>).</param>
    /// <param name="destinationFolderName">Destination folder name.</param>
    /// <param name="ReportProxies">If not null, this argument is used to relocate references to other items (e.g. columns) to point to the destination folder.</param>
    /// <param name="overwriteExistingItemsOfSameType">If true, any item with the same name and same type will be replaced by the copied item. (if false, a new name is found for the copied item which not conflicts with the existing items).</param>
    public void CopyItemToFolder(object item, string destinationFolderName, DocNodeProxyReporter? ReportProxies, bool overwriteExistingItemsOfSameType)
    {
      ProjectFolder.ThrowExceptionOnInvalidFullFolderPath(destinationFolderName);

      if (item is null)
        throw new ArgumentNullException(nameof(item));

      if (item is ProjectFolder projFolder)
      {
        var orgName = projFolder.Name;
        string destName = ProjectFolder.Combine(destinationFolderName, ProjectFolder.GetFoldersLastFolderPart(orgName));
        foreach (var subitem in GetItemsInFolderAndSubfolders(orgName))
        {
          var oldItemFolder = ProjectFolder.GetFolderPart(subitem.Name);
          var newItemFolder = oldItemFolder.Replace(orgName, destName);
          CopyItemToFolder(subitem, newItemFolder, ReportProxies, overwriteExistingItemsOfSameType);
        }
      }
      else if (item is IProjectItem projectItem)
      {
        var orgName = projectItem.Name;
        var clonedItem = (IProjectItem)projectItem.Clone();
        clonedItem.Name = ProjectFolder.Combine(destinationFolderName, ProjectFolder.GetNamePart(orgName));

        if (overwriteExistingItemsOfSameType)
        {
          if (AltaxoDocument.TryGetExistingItemWithSameTypeAndName(clonedItem, out var existingItem))
          {
            Current.IProjectService.DeleteDocument(existingItem, true);
          }
        }

        AltaxoDocument.AddItem(clonedItem);

        if (ReportProxies is not null)
        {
          clonedItem.VisitDocumentReferences(ReportProxies);
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
    public List<string> GetSubfoldersAsDisplayFolderNameStringListSorted(string parentFolder, bool recurseSubdirectories)
    {
      var result = GetSubfoldersAsStringList(parentFolder, recurseSubdirectories);

      for (int i = 0; i < result.Count; ++i)
        result[i] = ProjectFolder.ConvertFolderNameToDisplayFolderName(result[i]);

      result.Sort();

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
      if (!folderDictionary.TryGetValue(folderName, out var folderNode))
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
      if (!folderDictionary.TryGetValue(folderName, out var folderNode))
      {
        var parentNode = AddFolderNodeRecursively<T>(Main.ProjectFolder.GetFoldersParentFolder(folderName), folderDictionary);

        folderNode = new T { Text = Main.ProjectFolder.ConvertFolderNameToDisplayFolderName(Main.ProjectFolder.GetFoldersLastFolderPart(folderName)), Tag = folderName };
        folderDictionary.Add(folderName, folderNode);
        parentNode.Nodes.Add(folderNode);
      }
      return folderNode;
    }

    #endregion Helper Gui Functions
  }
}
