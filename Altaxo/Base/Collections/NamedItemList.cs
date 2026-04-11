#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Altaxo.Collections
{
  /// <summary>
  /// Defines an item that has an associated name.
  /// </summary>
  public interface INamedItem
  {
    /// <summary>
    /// Gets the name of the item.
    /// </summary>
    string Name { get; }
  }

  /// <summary>
  /// Interface to an item that has a name, and whose name can be changed.
  /// </summary>
  /// <seealso cref="Altaxo.Collections.INamedItem" />
  public interface IRenameableItem : INamedItem
  {
    /// <summary>
    /// Occurs before an item is renamed. First argument is the <see cref="IRenameableItem"/> instance, 2nd argument is the proposed new name.
    /// The function must return true if renaming the item is OK.
    /// </summary>
    event Func<IRenameableItem, string, bool>? BeforeRename; // sender, newName, return true if ok, false if the action should be cancelled.

    /// <summary>
    /// Occurs after renaming the item. First argument is the item, already with the new name, 2nd argument is the old name of the item.
    /// </summary>
    event Action<IRenameableItem, string> AfterRename; // sender, oldName
  }

  /// <summary>
  /// List of <see cref="INamedItem"/> instances, with support for <see cref="System.Collections.Specialized.INotifyCollectionChanged"/>
  /// </summary>
  /// <typeparam name="T">The type of the named items in the list.</typeparam>
  /// <seealso cref="System.Collections.Generic.IEnumerable{T}" />
  /// <seealso cref="System.Collections.Specialized.INotifyCollectionChanged" />
  public class NamedItemList<T> : IEnumerable<T>, System.Collections.Specialized.INotifyCollectionChanged where T : INamedItem
  {
    /// <summary>
    /// The ordered list of items.
    /// </summary>
    protected List<T> _list = new List<T>();

    /// <summary>
    /// Maps item names to their indices in <see cref="_list"/>.
    /// </summary>
    protected Dictionary<string, int> _nameToIndex = new Dictionary<string, int>();

    /// <summary>
    /// Occurs when the collection changes.
    /// </summary>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="NamedItemList{T}"/> class.
    /// </summary>
    public NamedItemList()
    {
    }

    /// <summary>
    /// Gets the number of items in the list.
    /// </summary>
    public int Count { get { return _list.Count; } }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
      return _list.GetEnumerator();
    }

    /// <summary>
    /// Raises the <see cref="E:CollectionChanged" /> event.
    /// </summary>
    /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
    public virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
      CollectionChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Adds the specified item to the list.
    /// </summary>
    /// <param name="item">The item to add.</param>
    public virtual void Add(T item)
    {
      if (_nameToIndex.ContainsKey(item.Name))
        throw new ArgumentException("An item with the same name is already contained in the collection");

      _list.Add(item);
      _nameToIndex.Add(item.Name, _list.Count - 1);

      if (CollectionChanged is not null)
      {
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
      }
    }

    /// <summary>
    /// Gets the item at the specified index.
    /// </summary>
    /// <param name="i">The zero-based item index.</param>
    public T this[int i]
    {
      get
      {
        return _list[i];
      }
    }

    /// <summary>
    /// Replaces the item at the specified index.
    /// </summary>
    /// <param name="i">The zero-based item index.</param>
    /// <param name="item">The replacement item.</param>
    /// <returns>The item previously stored at the specified index.</returns>
    public virtual T SetElement(int i, T item)
    {
      // test for name crashes
      T oldItem = _list[i];

      if (oldItem.Name != item.Name && _nameToIndex.ContainsKey(item.Name))
        throw new ArgumentException("Can't set item because it is already contained elsewhere in this collection");

      _list[i] = item;
      if (oldItem.Name != item.Name)
      {
        _nameToIndex.Remove(oldItem.Name);
        _nameToIndex.Add(item.Name, i);
      }

      if (!(CollectionChanged is null)) // Optimization to avoid instancing
      {
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, oldItem));
      }
      return oldItem;
    }

    /// <summary>
    /// Gets the item with the specified name.
    /// </summary>
    /// <param name="name">The name of the item.</param>
    public T this[string name]
    {
      get
      {
        if (_nameToIndex.TryGetValue(name, out var index))
          return _list[index];
        else
          throw new ArgumentException("An item with the given name is not contained in the collection");
      }
    }


    /// <summary>
    /// Tries to get the item with the specified name.
    /// </summary>
    /// <param name="name">The name of the item to retrieve.</param>
    /// <param name="item">When this method returns, contains the item if found; otherwise the default value.</param>
    /// <returns><see langword="true"/> if the item was found; otherwise, <see langword="false"/>.</returns>
    public bool TryGetValue(string name, [MaybeNullWhen(false)] out T item)
    {
      if (_nameToIndex.TryGetValue(name, out var index))
      {
        item = _list[index];
        return true;
      }
      else
      {
        item = default;
        return false;
      }
    }

    /// <summary>
    /// Gets the index of the specified item.
    /// </summary>
    /// <param name="item">The item to locate.</param>
    /// <returns>The zero-based index of the item, or <c>-1</c> if it is not found.</returns>
    public int IndexOf(INamedItem item)
    {
      int idx = IndexOf(item.Name);
      if (idx >= 0 && !object.Equals(_list[idx], item))
        idx = -1;

      return idx;
    }

    /// <summary>
    /// Gets the index of the item with the specified name.
    /// </summary>
    /// <param name="itemName">The item name to locate.</param>
    /// <returns>The zero-based index of the item, or <c>-1</c> if it is not found.</returns>
    public int IndexOf(string itemName)
    {
      if (_nameToIndex.TryGetValue(itemName, out var index))
        return index;
      else
        return -1;
    }

    /// <summary>
    /// Determines whether the specified item exists in the list.
    /// </summary>
    /// <param name="item">The item to locate.</param>
    /// <returns><see langword="true"/> if the item exists; otherwise, <see langword="false"/>.</returns>
    public bool Contains(INamedItem item)
    {
      return IndexOf(item) >= 0;
    }

    /// <summary>
    /// Determines whether an item with the specified name exists in the list.
    /// </summary>
    /// <param name="itemName">The item name to locate.</param>
    /// <returns><see langword="true"/> if an item with the specified name exists; otherwise, <see langword="false"/>.</returns>
    public bool Contains(string itemName)
    {
      return IndexOf(itemName) >= 0;
    }

    /// <summary>
    /// Removes the item at the specified index.
    /// </summary>
    /// <param name="i">The zero-based index of the item to remove.</param>
    /// <returns>The removed item.</returns>
    public virtual T RemoveAt(int i)
    {
      T item = _list[i];
      int index = _nameToIndex[item.Name];
      if (i != index)
        throw new ApplicationException("Inconsistency found in collection class");

      _list.RemoveAt(i);
      _nameToIndex.Remove(item.Name);

      // rebuild the index starting with i
      for (int j = i; j < _list.Count; j++)
        _nameToIndex[_list[j].Name] = j;

      if (!(CollectionChanged is null))
      {
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
      }
      return item;
    }

    /// <summary>
    /// Removes all items from the list.
    /// </summary>
    public void Clear()
    {
      _nameToIndex.Clear();
      _list.Clear();
      if (!(CollectionChanged is null))
      {
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
      }
    }

    #region IEnumerable Members

    /// <inheritdoc/>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return _list.GetEnumerator();
    }

    #endregion IEnumerable Members
  }

  /// <summary>
  /// Represents a list of renameable items and keeps its name index synchronized with rename operations.
  /// </summary>
  /// <typeparam name="T">The type of renameable item.</typeparam>
  public class RenameableItemList<T> : NamedItemList<T> where T : IRenameableItem
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="RenameableItemList{T}"/> class.
    /// </summary>
    public RenameableItemList()
    {
    }

    /// <inheritdoc/>
    public override void Add(T item)
    {
      base.Add(item);
      item.BeforeRename += EhBeforeRename;
      item.AfterRename += EhAfterRename;
    }

    /// <inheritdoc/>
    public override T SetElement(int i, T item)
    {
      T oldItem = base.SetElement(i, item);
      oldItem.BeforeRename -= EhBeforeRename;
      oldItem.AfterRename -= EhAfterRename;

      item.BeforeRename += EhBeforeRename;
      item.AfterRename += EhAfterRename;

      return oldItem;
    }

    /// <inheritdoc/>
    public override T RemoveAt(int i)
    {
      T item = base.RemoveAt(i);
      item.BeforeRename -= EhBeforeRename;
      item.AfterRename -= EhAfterRename;

      return item;
    }

    private bool EhBeforeRename(IRenameableItem item, string newName)
    {
      if (_nameToIndex.ContainsKey(newName))
        return false;

      return true;
    }

    private void EhAfterRename(IRenameableItem item, string oldName)
    {
      int index = _nameToIndex[oldName];
      _nameToIndex.Remove(oldName);
      _nameToIndex.Add(item.Name, index);
    }
  }
}
