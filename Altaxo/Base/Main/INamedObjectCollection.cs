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
using System;
using System.Collections.Generic;

namespace Altaxo.Main
{
  /// <summary>
  /// Interface for an object which has a name.
  /// </summary>
  public interface INamedObject
  {
    /// <summary>
    /// Gets the name of this instance.
    /// </summary>
    /// <value>
    /// The name of this instance.
    /// </value>
    string Name { get; }
  }

  /// <summary>
  /// This interface should be implemented by all objects which have an own name, i.e. member variable (do not implement this interface if the name
  /// is retrieved from somewhere else like the parent or so).
  /// </summary>
  public interface INameOwner : INamedObject
  {
    /// <summary>The name of the name owner. The set operation can throw an InvalidOperation exception if it is not allowed to set the name.</summary>
    new string Name { get; set; }
  }

  /// <summary>
  /// Designates the type of change in an collection of named items.
  /// </summary>
  [Flags]
  public enum NamedObjectCollectionChangeType
  {
    /// <summary>A new item was added to the collection.</summary>
    ItemAdded = 0x01,

    /// <summary>An item was removed from the collection.</summary>
    ItemRemoved = 0x02,

    /// <summary>An item was renamed.</summary>
    ItemRenamed = 0x04,

    /// <summary>More than one item was added, removed or renamed.</summary>
    MultipleChanges = 0x80
  }

  /// <summary>
  /// Summary description for INamedObjectCollection.
  /// </summary>
  public interface INamedObjectCollection
  {
    /// <summary>
    /// retrieves the object with the name <code>name</code>.
    /// </summary>
    /// <param name="name">The objects name.</param>
    /// <returns>The object with the specified name.</returns>
    IDocumentLeafNode GetChildObjectNamed(string name);

    /// <summary>
    /// Retrieves the name of the provided object.
    /// </summary>
    /// <param name="o">The object for which the name should be found.</param>
    /// <returns>The name of the object. Null if the object is not found. String.Empty if the object is found but has no name.</returns>
    string GetNameOfChildObject(IDocumentLeafNode o);
  }

  /// <summary>
  /// Interface of a parent node that holds child nodes which can be renamed.
  /// </summary>
  public interface IParentOfINameOwnerChildNodes
  {
    /// <summary>
    /// Determines whether the name of the child item can be changed to the provided new name. If the parent implementing this interface is not able to change the child's name, it must return <c>false</c>.
    /// </summary>
    /// <param name="childNode">The child node.</param>
    /// <param name="newName">The new name.</param>
    /// <returns><c>true</c> if the child can be renamed to the new name; otherwise <c>false</c>.</returns>
    bool EhChild_CanBeRenamed(Main.INameOwner childNode, string newName);

    /// <summary>
    /// Called if the child has been renamed.
    /// </summary>
    /// <param name="childNode">The child node.</param>
    /// <param name="oldName">The old name of the child name.</param>
    void EhChild_HasBeenRenamed(Main.INameOwner childNode, string oldName);

    /// <summary>
    /// Called if the child's parent changed.
    /// </summary>
    /// <param name="childNode">The child node.</param>
    /// <param name="oldParent">The old parent of the child node.</param>
    void EhChild_ParentChanged(Main.INameOwner childNode, IDocumentNode oldParent);

    /// <summary>
    /// Helper function called by a child of this collection to handle renaming of this child.
    /// </summary>
    /// <param name="child">The child's instance.</param>
    /// <param name="newName">The proposed new name of the child. The parent can modify this proposed name if another item with the same name is already contained in this collection.</param>
    /// <param name="setName">Action to set the name to the provided value. This function should <b>only</b> set the name field, but not raise any events etc.</param>
    /// <param name="raiseOnNameChanged">Action to raise the NameChanged event on the child.</param>
    /// <exception cref="ArgumentNullException">New name is null.</exception>
    void RenameChild(INameOwner child, string newName, Action<string> setName, Action<string> raiseOnNameChanged);
  }

  public class NamedObjectCollectionChangedEventArgs : Main.SelfAccumulateableEventArgs
  {
    /// <summary>
    /// Item that is used to indicate that multiple changes have occured.
    /// </summary>
    private static readonly object MultipleChangesItem = new object();

    private object _item;
    private string _oldItemName;
    private string _newItemName;
    private NamedObjectCollectionChangeType _operation;

    #region Properties

    public object Item { get { return _item; } }

    public string OldName { get { return _oldItemName; } }

    public string NewName { get { return _newItemName; } }

    public NamedObjectCollectionChangeType Changes { get { return _operation; } }

    public bool WasItemAdded { get { return _operation.HasFlag(NamedObjectCollectionChangeType.ItemAdded); } }

    public bool WasItemRemoved { get { return _operation.HasFlag(NamedObjectCollectionChangeType.ItemRemoved); } }

    public bool WasItemRenamed { get { return _operation.HasFlag(NamedObjectCollectionChangeType.ItemRenamed); } }

    public bool WasMultipleItemsChanged { get { return _operation.HasFlag(NamedObjectCollectionChangeType.MultipleChanges); } }

    #endregion Properties

    /// <summary>
    /// Returns an instance when an item was added.
    /// </summary>
    public static NamedObjectCollectionChangedEventArgs FromItemAdded(INamedObject item)
    {
      if (null == item)
        throw new ArgumentNullException("item");
      var result = new NamedObjectCollectionChangedEventArgs() { _item = item, _newItemName = item.Name, _oldItemName = item.Name, _operation = NamedObjectCollectionChangeType.ItemAdded };
      return result;
    }

    /// <summary>
    /// Returns an instance when an item was added.
    /// </summary>
    public static NamedObjectCollectionChangedEventArgs FromItemRemoved(INamedObject item)
    {
      if (null == item)
        throw new ArgumentNullException("item");
      var result = new NamedObjectCollectionChangedEventArgs() { _item = item, _newItemName = item.Name, _oldItemName = item.Name, _operation = NamedObjectCollectionChangeType.ItemRemoved };
      return result;
    }

    /// <summary>
    /// Returns an instance when an item was added. Here the additional parameter <paramref name="itemNameOverride"/> is used as the new and old name of the item.
    /// Use this only when absolutely sure about it.
    /// </summary>
    public static NamedObjectCollectionChangedEventArgs FromItemRemoved(INamedObject item, string itemNameOverride)
    {
      if (null == item)
        throw new ArgumentNullException("item");
      var result = new NamedObjectCollectionChangedEventArgs() { _item = item, _newItemName = itemNameOverride, _oldItemName = itemNameOverride, _operation = NamedObjectCollectionChangeType.ItemRemoved };
      return result;
    }

    /// <summary>
    /// Returns an instance when an item was added.
    /// </summary>
    public static NamedObjectCollectionChangedEventArgs FromItemRenamed(INamedObject item, string oldName)
    {
      if (null == item)
        throw new ArgumentNullException("item");
      var result = new NamedObjectCollectionChangedEventArgs() { _item = item, _newItemName = item.Name, _oldItemName = oldName, _operation = NamedObjectCollectionChangeType.ItemRenamed };
      return result;
    }

    /// <summary>
    /// Returns an instance when an item was added.
    /// </summary>
    public static NamedObjectCollectionChangedEventArgs FromMultipleChanges()
    {
      var result = new NamedObjectCollectionChangedEventArgs() { _item = MultipleChangesItem, _operation = NamedObjectCollectionChangeType.MultipleChanges };
      return result;
    }

    public override void Add(SelfAccumulateableEventArgs e)
    {
      var other = e as NamedObjectCollectionChangedEventArgs;
      if (other == null)
        throw new ArgumentOutOfRangeException("Argument e should be of type NamedObjectCollectionEventArgs");
      if (!object.ReferenceEquals(this._item, other._item))
        throw new ArgumentOutOfRangeException("Argument e has an item which is not identical to this item. This should not happen since Equals and GetHashCode are overriden.");

      // MultipleChanges overrrides everything
      if (this._operation.HasFlag(NamedObjectCollectionChangeType.MultipleChanges))
        return;
      if (other._operation.HasFlag(NamedObjectCollectionChangeType.MultipleChanges))
      {
        this._operation = NamedObjectCollectionChangeType.MultipleChanges;
        this._item = MultipleChangesItem;
        this._newItemName = null;
        this._oldItemName = null;
        return;
      }

      // Normal changes

      this._newItemName = other._newItemName;
      this._operation |= other._operation;

      if (other._operation.HasFlag(NamedObjectCollectionChangeType.ItemAdded))
      {
        this._operation = this._operation.WithClearedFlag(NamedObjectCollectionChangeType.ItemRemoved);
        if (_oldItemName != _newItemName)
          this._operation = this._operation.WithSetFlag(NamedObjectCollectionChangeType.ItemRenamed);
      }
      else if (other._operation.HasFlag(NamedObjectCollectionChangeType.ItemRemoved))
      {
        this._operation = this._operation.WithClearedFlag(NamedObjectCollectionChangeType.ItemAdded);
        this._operation = this._operation.WithClearedFlag(NamedObjectCollectionChangeType.ItemRenamed);
      }
    }

    /// <summary>
    /// Override so that two instances of this type, which contain exactly the same item are considered the same. This is to ensure that events are accumulated for each individual item of a collection during the suspended state.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
    /// </returns>
    public override int GetHashCode()
    {
      return 17 * this.GetType().GetHashCode() + 31 * this._item.GetHashCode();
    }

    /// <summary>
    /// Override so that two instances of this type, which contain exactly the same item are considered the same. This is to ensure that events are accumulated for each individual item of a collection during the suspended state.
    /// </summary>
    /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
    /// <returns>
    ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object obj)
    {
      if (null == obj || this.GetType() != obj.GetType())
        return false;

      var other = (NamedObjectCollectionChangedEventArgs)obj;

      return object.ReferenceEquals(this._item, other._item);
    }
  }
}
