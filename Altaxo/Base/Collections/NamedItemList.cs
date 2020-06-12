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
  /// Interface to an items that has a name associated with it.
  /// </summary>
  public interface INamedItem
  {
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
  /// <typeparam name="T"></typeparam>
  /// <seealso cref="System.Collections.Generic.IEnumerable{T}" />
  /// <seealso cref="System.Collections.Specialized.INotifyCollectionChanged" />
  public class NamedItemList<T> : IEnumerable<T>, System.Collections.Specialized.INotifyCollectionChanged where T : INamedItem
  {
    protected List<T> _list = new List<T>();
    protected Dictionary<string, int> _nameToIndex = new Dictionary<string, int>();

    /// <summary>
    /// Occurs when the collection changes.
    /// </summary>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    public NamedItemList()
    {
    }

    public int Count { get { return _list.Count; } }

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

    public virtual void Add(T item)
    {
      if (_nameToIndex.ContainsKey(item.Name))
        throw new ArgumentException("An item with the same name is already contained in the collection");

      _list.Add(item);
      _nameToIndex.Add(item.Name, _list.Count - 1);

      if (null != CollectionChanged)
      {
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
      }
    }

    public T this[int i]
    {
      get
      {
        return _list[i];
      }
    }

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

    public int IndexOf(INamedItem item)
    {
      int idx = IndexOf(item.Name);
      if (idx >= 0 && !object.Equals(_list[idx], item))
        idx = -1;

      return idx;
    }

    public int IndexOf(string itemName)
    {
      if (_nameToIndex.TryGetValue(itemName, out var index))
        return index;
      else
        return -1;
    }

    public bool Contains(INamedItem item)
    {
      return IndexOf(item) >= 0;
    }

    public bool Contains(string itemName)
    {
      return IndexOf(itemName) >= 0;
    }

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

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return _list.GetEnumerator();
    }

    #endregion IEnumerable Members
  }

  public class RenameableItemList<T> : NamedItemList<T> where T : IRenameableItem
  {
    public RenameableItemList()
    {
    }

    public override void Add(T item)
    {
      base.Add(item);
      item.BeforeRename += EhBeforeRename;
      item.AfterRename += EhAfterRename;
    }

    public override T SetElement(int i, T item)
    {
      T oldItem = base.SetElement(i, item);
      oldItem.BeforeRename -= EhBeforeRename;
      oldItem.AfterRename -= EhAfterRename;

      item.BeforeRename += EhBeforeRename;
      item.AfterRename += EhAfterRename;

      return oldItem;
    }

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
