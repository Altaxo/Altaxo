﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2017 Dr. Dirk Lellinger
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
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Altaxo.Main
{
  /// <summary>
  /// Base class for collections that hold project items, which are uniquely named.
  /// </summary>
  public abstract class ProjectItemCollectionBase<TItem, TDictionaryItem>
      :
      Main.SuspendableDocumentNodeWithSetOfEventArgs,
      Main.IProjectItemCollection,
      ICollection<TItem> where TItem : TDictionaryItem where TDictionaryItem : IProjectItem
  {
    // Data
    protected SortedDictionary<string, TDictionaryItem> _itemsByName = new SortedDictionary<string, TDictionaryItem>();

    /// <summary>
    /// Fired when one or more project items are added, deleted or renamed. Not fired when content in the project item has changed.
    /// Arguments are the type of change, the item that changed, the old name (if renamed), and the new name (if renamed).
    /// This event can not be suspended.
    /// </summary>
    public event EventHandler<Main.NamedObjectCollectionChangedEventArgs>? CollectionChanged;

    #region Abstract members

    /// <summary>
    /// Gets the base name of a project item. The base name will be combined with a number to find a unique name.
    /// </summary>
    /// <value>
    /// The base name of a project item in this collection.
    /// </value>
    public abstract string ItemBaseName { get; }

    public virtual void UnwireItem(TDictionaryItem item)
    {
    }

    public virtual void WireItem(TItem item)
    {
    }

    #endregion Abstract members

    public ProjectItemCollectionBase(IDocumentNode parent)
    {
      _parent = parent;
    }

    #region ICollection<DataTable> Members

    public virtual void Clear()
    {
      var items = _itemsByName.Values.ToArray();

      InternalClear();

      using (var suspendToken = SuspendGetToken())
      {
        foreach (var item in items)
        {
          UnwireItem(item);
          item.Dispose();
          EhSelfChanged(Main.NamedObjectCollectionChangedEventArgs.FromItemRemoved(item));
        }
        suspendToken.Resume();
      }
    }

    protected virtual void InternalClear()
    {
      _itemsByName.Clear();
    }

    public bool Contains(TItem item)
    {
      if (item is null)
        throw new ArgumentNullException(nameof(item));

      if (_itemsByName.TryGetValue(item.Name, out var foundItem))
        return object.ReferenceEquals(foundItem, item);
      else
        return false;
    }

    bool IProjectItemCollection.TryGetValue(string projectItemName, [MaybeNullWhen(false)] out IProjectItem projectItem)
    {
      var result = _itemsByName.TryGetValue(projectItemName, out var item);
      projectItem = item;
      return result && !(projectItem is null);
    }

    public void CopyTo(TItem[] array, int arrayIndex)
    {
      foreach (var item in _itemsByName.Values.OfType<TItem>())
        array[arrayIndex++] = item;
    }

    public bool IsReadOnly
    {
      get { return false; }
    }

    bool ICollection<TItem>.Remove(TItem item)
    {
      return Remove(item);
    }

    public int Count
    {
      get { return _itemsByName.Values.OfType<TItem>().Count(); }
    }

    #endregion ICollection<DataTable> Members

    #region IEnumerable<DataTable> Members

    IEnumerator<TItem> IEnumerable<TItem>.GetEnumerator()
    {
      return _itemsByName.Values.OfType<TItem>().GetEnumerator();
    }

    #endregion IEnumerable<DataTable> Members

    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return _itemsByName.Values.GetEnumerator();
    }

    #endregion IEnumerable Members

    #region Suspend and resume

    protected override void OnChanged(EventArgs e)
    {
      if (e is Main.NamedObjectCollectionChangedEventArgs nocc)
      {
        CollectionChanged?.Invoke(this, nocc);
      }

      base.OnChanged(e);
    }

    #endregion Suspend and resume

    public bool IsDirty
    {
      get
      {
        return _accumulatedEventData is not null;
      }
    }

    /// <summary>
    /// Gets the name of the items sorted by name.
    /// </summary>
    /// <returns>Array of sorted names.</returns>
    public string[] GetSortedItemNames()
    {
      var list = new List<string>(_itemsByName.Where(entry => entry.Value is TItem).Select(entry => entry.Key));
      list.Sort();
      return list.ToArray();
    }

    public TItem this[string name]
    {
      get
      {
        if (_itemsByName.TryGetValue(name, out var result))
        {
          if (result is TItem item)
            return item;
          else
            throw new ArgumentOutOfRangeException(string.Format("The item \"{1}\" stored here is of another type ({0})!", typeof(TItem).FullName, name));
        }
        else
          throw new ArgumentOutOfRangeException(string.Format("The {0} \"{1}\" does not exist!", typeof(TItem).Name, name));
      }
    }

    public bool TryGetValue(string name, [MaybeNullWhen(false)] out TItem item)
    {
      if (_itemsByName.TryGetValue(name, out var result) && result is TItem item1)
      {
        item = item1;
        return true;
      }
      else
      {
        item = default;
        return false;
      }
    }

    /// <summary>
    /// Determines whether the collection contains any project item with the specified name. This must not neccessarily
    /// a item of the type that this collection stores (some collections can have a shared name dictionary).
    /// In constrast, use <see cref="Contains(string)"/> to determine if the collection contains an item with the specified name and the native type that the collection stores.
    /// </summary>
    /// <param name="itemName">Name of the project item.</param>
    /// <returns>True if the collection contains any project item with the specified name.</returns>
    public bool ContainsAnyName(string itemName)
    {
      return itemName is not null && _itemsByName.ContainsKey(itemName);
    }

    /// <summary>
    /// Determines whether the collection contains a project item with the specified name and with the type that this collection stores.
    /// </summary>
    /// <param name="itemName">Name of the project item.</param>
    /// <returns>True if the collection contains any project item with the specified name.</returns>
    public bool Contains(string itemName)
    {
      return itemName is not null && _itemsByName.TryGetValue(itemName, out var result) && result is TItem;
    }

    public IEnumerable<string> Names
    {
      get
      {
        return _itemsByName.Keys;
      }
    }

    public virtual void Add(TItem item)
    {
      if (item is null)
        throw new ArgumentNullException(nameof(item));

      if (item.TryGetName(out var itemName) && _itemsByName.TryGetValue(itemName, out var existingItem) && object.Equals(item, existingItem))
        throw new InvalidOperationException($"The item {item.Name} is already contained in the collection!");

      if (!item.TryGetName(out itemName)) // if no name provided (an empty string is a valid name)
        item.Name = FindNewItemName();                 // find a new one
      else if (_itemsByName.ContainsKey(itemName)) // else if this name is already in use
        item.Name = FindNewItemName(itemName); // find a new  name based on the original name

      // now the project item has a unique name in any case
      InternalAdd(item);
      item.ParentObject = this;
      WireItem(item);

      // raise data event to all listeners
      EhSelfChanged(Main.NamedObjectCollectionChangedEventArgs.FromItemAdded(item));
    }

    protected virtual void InternalAdd(TItem item)
    {
      _itemsByName.Add(item.Name, item);
    }

    /// <summary>
    /// Removes the item from the collection and disposes it. Only items that belong to this collection will be removed and disposed.
    /// </summary>
    /// <param name="item">The item to remove and dispose.</param>
    /// <returns>True if the item was found in the collection and thus removed successfully.</returns>
    public virtual bool Remove(TItem item)
    {
      if (item is null)
        throw new ArgumentNullException(nameof(item));

      bool success = false;
      if (InternalRemove(item))
      {
        UnwireItem(item);
        item.Dispose();
        EhSelfChanged(Main.NamedObjectCollectionChangedEventArgs.FromItemRemoved(item));
        success = true;
      }

      if (success)
        EhSelfChanged(Main.NamedObjectCollectionChangedEventArgs.FromItemRemoved(item));

      return success;
    }

    /// <summary>
    /// Removes the item from the collection. This function should only work on the internal structures, but do nothing else.
    /// </summary>
    /// <param name="item">The item to remove from the internal structures.</param>
    /// <returns>True if the item was found in the collection and thus removed successfully.</returns>
    protected virtual bool InternalRemove(TItem item)
    {
      return _itemsByName.Remove(item.Name);
    }

    /// <summary>
    /// Removes the item, referenced by its name, from the collection and disposes it.
    /// Only items that belong to this collection will be removed and disposed.
    /// </summary>
    /// <param name="itemName">The name of the item to remove and dispose.</param>
    /// <returns>True if the item was found in the collection and thus removed successfully.</returns>
    public bool Remove(string itemName)
    {
      if (_itemsByName.TryGetValue(itemName, out var result) && result is TItem item)
        return Remove(item);
      else
        return false;
    }

    /// <summary>
    /// Replaces the old item with a new item while keeping the item's index.
    /// </summary>
    /// <param name="oldItem">The old item. This item has to be part of the collection.</param>
    /// <param name="newItem">The item that replaces the old item.</param>
    public virtual void Exchange(TItem oldItem, TItem newItem)
    {
      if (object.ReferenceEquals(oldItem, newItem))
        return;

      if (!_itemsByName.TryGetValue(oldItem.Name, out var oldItemInThis) || !object.ReferenceEquals(oldItem, oldItemInThis))
        throw new ArgumentOutOfRangeException(nameof(oldItem), "The item to replace is not part of this collection");

      // if the new document has a name that is already present, try to find a new one.
      if (!(oldItem.Name == newItem.Name) && _itemsByName.ContainsKey(newItem.Name))
      {
        newItem.Name = FindNewItemName(newItem.Name);
      }

      InternalExchange(oldItem, newItem);

      UnwireItem(oldItem);
      WireItem(newItem);

      var oldEventArgs = Main.NamedObjectCollectionChangedEventArgs.FromItemRemoved(oldItem);
      var newEventArgs = Main.NamedObjectCollectionChangedEventArgs.FromItemAdded(newItem);

      EhSelfChanged(oldEventArgs);
      EhSelfChanged(newEventArgs);

      oldItem.Dispose();
    }

    protected virtual void InternalExchange(TItem oldItem, TItem newItem)
    {
      _itemsByName.Remove(oldItem.Name);
      _itemsByName.Add(newItem.Name, newItem);
    }

    bool Main.IParentOfINameOwnerChildNodes.EhChild_CanBeRenamed(Main.INameOwner childNode, string newName)
    {
      if (_itemsByName.ContainsKey(newName) && !object.ReferenceEquals(_itemsByName[newName], childNode))
        return false;
      else
        return true;
    }

    void Main.IParentOfINameOwnerChildNodes.EhChild_HasBeenRenamed(Main.INameOwner item, string? oldName)
    {
      if (_itemsByName.ContainsKey(item.Name))
      {
        if (object.ReferenceEquals(_itemsByName[item.Name], item))
          return; // Item alredy renamed
        else
          throw new ApplicationException(string.Format("{0} with name " + item.Name + " already exists!", typeof(TItem).Name));
      }

      if (!(oldName is null) && _itemsByName.ContainsKey(oldName))
      {
        if (!object.ReferenceEquals(_itemsByName[oldName], item))
          throw new ApplicationException(string.Format("Names between parent collection and {0} not in sync", typeof(TItem).Name));

        _itemsByName.Remove(oldName);
        _itemsByName.Add(item.Name, (TItem)item);

        EhSelfChanged(Main.NamedObjectCollectionChangedEventArgs.FromItemRenamed(item, oldName));
      }
      else
      {
        throw new ApplicationException(string.Format("Error renaming {0} " + oldName + " : this {0} name was not found in the collection!", typeof(TItem).Name));
      }
    }

    void Main.IParentOfINameOwnerChildNodes.EhChild_ParentChanged(Main.INameOwner childNode, Main.IDocumentNode? oldParent)
    {
      if (ReferenceEquals(this, oldParent) && _itemsByName.ContainsKey(childNode.Name))
        throw new InvalidProgramException(string.Format("Unauthorized change of the {0}'s parent", typeof(TItem).Name));
    }

    /// <summary>
    /// Looks for the next free standard project item name in the root folder.
    /// </summary>
    /// <returns>A new project item name that is unique in this collection.</returns>
    public string FindNewItemName()
    {
      return FindNewItemNameInFolder(string.Empty);
    }

    /// <summary>
    /// Looks for the next free standard project item name in the specified folder.
    /// </summary>
    /// <param name="folder">The folder where to find a unique project item name.</param>
    /// <returns></returns>
    public string FindNewItemNameInFolder(string folder)
    {
      return FindNewItemName(Main.ProjectFolder.Combine(folder, ItemBaseName));
    }

    /// <summary>
    /// Looks for the next unique project item name base on a basic name.
    /// </summary>
    /// <returns>A new project item name unique for this collection.</returns>
    public string FindNewItemName(string basicname)
    {
      for (int i = 0; ; i++)
      {
        if (!_itemsByName.ContainsKey(basicname + i))
          return basicname + i;
      }
    }

    /// <summary>
    /// Helper function called by a child of this collection to handle renaming of this child.
    /// </summary>
    /// <param name="newName">The new name of the child.</param>
    /// <param name="child">The child's instance.</param>
    /// <param name="setName">Action to set the name to the provided value. This function should only do set the name, but not raise any evens etc.</param>
    /// <param name="raiseOnNameChanged">Action to raise the NameChanged event on the child.</param>
    /// <exception cref="ArgumentNullException">newName - New name is null</exception>
    /// <exception cref="ApplicationException"></exception>
    public void RenameChild(INameOwner child, string newName, Action<string> setName, Action<string> raiseOnNameChanged)
    {
      var oldName = child.Name;

      if (newName is null)
        throw new ArgumentNullException(nameof(newName), string.Format("New name of {0} is null (the old name was: {1})", child?.GetType(), oldName));

      if (newName == oldName)
        return; // nothing changed

      var parentAs = (IParentOfINameOwnerChildNodes)this;
      var canBeRenamed = parentAs.EhChild_CanBeRenamed(child, newName);

      if (canBeRenamed)
      {
        setName(newName);

        parentAs.EhChild_HasBeenRenamed(child, oldName);

        raiseOnNameChanged(oldName);
      }
      else
      {
        throw new ApplicationException(string.Format("Renaming of {0} {1} into {2} not possible, because name exists already", child.GetType().Name, oldName, newName));
      }
    }

    public override Main.IDocumentLeafNode? GetChildObjectNamed(string name)
    {
      if (_itemsByName.TryGetValue(name, out var result))
        return result;

      return null;
    }

    public override string? GetNameOfChildObject(Main.IDocumentLeafNode obj)
    {
      if (obj is TItem item)
      {
        if (_itemsByName.ContainsKey(item.Name))
          return item.Name;
      }
      return null;
    }

    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      foreach (var entry in _itemsByName)
        if (entry.Value is TItem)
          yield return new Main.DocumentNodeAndName(entry.Value, entry.Key);
    }

    protected override void Dispose(bool isDisposing)
    {
      if (!IsDisposed)
      {
        var itemsByName = _itemsByName;
        _itemsByName = new SortedDictionary<string, TDictionaryItem>();
        foreach (var entry in itemsByName)
          entry.Value?.Dispose();
      }
      base.Dispose(isDisposing);
    }

    #region IProjectItemCollection hidden implementations

    IProjectItem IProjectItemCollection.this[string name]
    {
      get { return this[name]; }
    }

    void IProjectItemCollection.Add(IProjectItem projectItem)
    {
      if (projectItem is TItem titem)
        Add(titem);
      else if (projectItem is not null)
        throw new ArgumentException("Item is not of expected type " + typeof(TItem).Name, nameof(projectItem));
      else
        throw new ArgumentNullException(nameof(projectItem));
    }

    bool IProjectItemCollection.Remove(IProjectItem projectItem)
    {
      if (projectItem is TItem titem)
        return Remove(titem);
      else if (projectItem is not null)
        throw new ArgumentException("Item is not of expected type " + typeof(TItem).GetType(), nameof(projectItem));
      else
        throw new ArgumentNullException(nameof(projectItem));
    }

    IEnumerable<IProjectItem> IProjectItemCollection.ProjectItems
    {
      get
      {
        foreach (var item in _itemsByName.Values.OfType<TItem>())
          yield return item;
      }
    }

    #endregion IProjectItemCollection hidden implementations
  }
}
