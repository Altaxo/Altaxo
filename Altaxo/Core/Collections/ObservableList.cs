#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Altaxo.Collections
{
  /// <summary>
  /// Generic list that implements the <see cref=" INotifyCollectionChanged"/> interface to notify changed.
  /// Does additionally implement <see cref="MoveToLowerIndex(int)"/> and <see cref="MoveToHigherIndex(int)"/> as well as <see cref="AddRange(IEnumerable{T})"/>.
  /// </summary>
  /// <typeparam name="T">The type of the items in this list.</typeparam>
  /// <seealso cref="IList{T}" />
  /// <seealso cref="INotifyCollectionChanged" />
  /// <seealso cref="INotifyPropertyChanged" />
  public class ObservableList<T> : IList<T>, INotifyCollectionChanged, INotifyPropertyChanged
  {
    private IList<T> _innerList;

    /// <inheritdoc />
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    private static PropertyChangedEventArgs _countChangedEventArgs;

    static ObservableList()
    {
      _countChangedEventArgs = new PropertyChangedEventArgs(nameof(Count));
    }

    public ObservableList()
    {
      _innerList = new List<T>();
    }

    public ObservableList(IEnumerable<T> collection)
    {
      _innerList = new List<T>(collection);
    }

    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
      CollectionChanged?.Invoke(this, e);
    }

    /// <inheritdoc />
    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
    {
      PropertyChanged?.Invoke(this, e);
    }

    #region Read-only actions (IList)

    /// <inheritdoc />
    public int IndexOf(T item)
    {
      return _innerList.IndexOf(item);
    }

    /// <inheritdoc />
    public bool Contains(T item)
    {
      return _innerList.Contains(item);
    }

    /// <inheritdoc />
    public void CopyTo(T[] array, int arrayIndex)
    {
      _innerList.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc />
    public int Count
    {
      get { return _innerList.Count; }
    }

    /// <inheritdoc />
    public bool IsReadOnly
    {
      get { return _innerList.IsReadOnly; }
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator()
    {
      return _innerList.GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
      return _innerList.GetEnumerator();
    }

    #endregion Read-only actions (IList)

    #region Actions that change the inner list (IList)

    /// <inheritdoc />
    public void Insert(int index, T item)
    {
      _innerList.Insert(index, item);
      OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
      OnPropertyChanged(_countChangedEventArgs);
    }

    /// <inheritdoc />
    public void RemoveAt(int index)
    {
      T item = _innerList[index];
      _innerList.RemoveAt(index);
      OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
      OnPropertyChanged(_countChangedEventArgs);
    }

    /// <inheritdoc />
    public T this[int index]
    {
      get { return _innerList[index]; }
      set
      {
        var oldItem = _innerList[index];
        _innerList[index] = value;
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldItem, index));
      }
    }

    /// <inheritdoc />
    public void Add(T item)
    {
      _innerList.Add(item);
      OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, _innerList.Count - 1));
      OnPropertyChanged(_countChangedEventArgs);
    }

    /// <inheritdoc />
    public void Clear()
    {
      if (_innerList.Count > 0)
      {
        _innerList.Clear();
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        OnPropertyChanged(_countChangedEventArgs);
      }
    }

    /// <inheritdoc />
    public bool Remove(T item)
    {
      lock (this)
      {
        int index = _innerList.IndexOf(item);
        if (_innerList.Remove(item))
        {
          OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
          OnPropertyChanged(_countChangedEventArgs);
          return true;
        }
        else
        {
          return false;
        }
      }
    }

    #endregion Actions that change the inner list (IList)

    #region Additional features not included in an interface

    /// <summary>
    /// Adds a range of items to the list.
    /// </summary>
    /// <param name="itemsToAdd">The items to add.</param>
    public void AddRange(IEnumerable<T> itemsToAdd)
    {
      var listToAdd = new List<T>(itemsToAdd);
      _innerList.AddRange(listToAdd);
      OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, listToAdd));
      OnPropertyChanged(_countChangedEventArgs);
    }

    /// <summary>
    /// Moves the item at index <paramref name="idx"/> to the index <paramref name="idx"/>-1.
    /// The item at index <paramref name="idx"/>-1 is moved to index <paramref name="idx"/>.
    /// </summary>
    /// <param name="idx">The index of the item to move.</param>
    public void MoveToLowerIndex(int idx)
    {
      ExchangeItemsAtIndices(idx, idx - 1);
    }

    /// <summary>
    /// Moves the item at index <paramref name="idx"/> to the index <paramref name="idx"/>+1.
    /// The item at index <paramref name="idx"/>+1 is moved to index <paramref name="idx"/>.
    /// </summary>
    /// <param name="idx">The index of the item to move.</param>
    public void MoveToHigherIndex(int idx)
    {
      ExchangeItemsAtIndices(idx, idx + 1);
    }

    /// <summary>
    /// Exchanges the item at index <paramref name="idx1"/> with the item at <paramref name="idx2"/>.
    /// </summary>
    /// <param name="idx1">The index of the one item.</param>
    /// <param name="idx2">The index of the other item.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If a provided index is out of range.
    /// </exception>
    public void ExchangeItemsAtIndices(int idx1, int idx2)
    {
      if (!(0 <= idx1 && idx1 < _innerList.Count))
        throw new ArgumentOutOfRangeException(nameof(idx1));
      if (!(0 <= idx2 && idx2 < _innerList.Count))
        throw new ArgumentOutOfRangeException(nameof(idx2));

      if (idx1 != idx2)
      {
        T item1 = _innerList[idx1];
        T item2 = _innerList[idx2];
        _innerList[idx1] = item2;
        _innerList[idx2] = item1;
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item1, idx1, idx2));
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item2, idx2, idx1));
      }
    }

    #endregion Additional features not included in an interface
  }
}
