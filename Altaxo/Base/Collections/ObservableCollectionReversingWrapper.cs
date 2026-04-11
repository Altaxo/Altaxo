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
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Altaxo.Collections
{
  /// <summary>
  /// Wraps an observable collection, so that the elements appear in reverse order compared to the original collection.
  /// </summary>
  /// <typeparam name="T">The item type.</typeparam>
  public class ObservableCollectionReversingWrapper<T> : IList<T>, IReadOnlyList<T>, INotifyCollectionChanged
  {
    /// <summary>
    /// The wrapped collection.
    /// </summary>
    private ObservableCollection<T> _coll;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableCollectionReversingWrapper{T}"/> class.
    /// </summary>
    /// <param name="coll">The wrapped collection.</param>
    public ObservableCollectionReversingWrapper(ObservableCollection<T> coll)
    {
      _coll = coll;
      _coll.CollectionChanged += new WeakEventHandler<NotifyCollectionChangedEventArgs>(EhOriginalCollectionChanged, _coll, nameof(_coll.CollectionChanged)).EventSink;
    }

    private void EhOriginalCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
      NotifyCollectionChangedEventArgs result;

      int newStart = e.NewItems is not null ? _coll.Count - e.NewStartingIndex - e.NewItems.Count : e.NewStartingIndex;

      int oldStart = e.OldItems is not null ? _coll.Count - e.OldStartingIndex - e.OldItems.Count : e.OldStartingIndex;

      switch (e.Action)
      {
        case NotifyCollectionChangedAction.Add:
          result = new NotifyCollectionChangedEventArgs(e.Action, e.NewItems, newStart);
          break;

        case NotifyCollectionChangedAction.Move:
          result = new NotifyCollectionChangedEventArgs(e.Action, e.NewItems, newStart, oldStart);
          break;

        case NotifyCollectionChangedAction.Remove:
          result = new NotifyCollectionChangedEventArgs(e.Action, e.OldItems, oldStart);
          break;

        case NotifyCollectionChangedAction.Replace:
          result = new NotifyCollectionChangedEventArgs(e.Action, e.NewItems!, e.OldItems!);
          break;

        case NotifyCollectionChangedAction.Reset:
          result = new NotifyCollectionChangedEventArgs(e.Action, e.OldItems, oldStart);
          break;

        default:
          throw new NotImplementedException(e.Action.ToString());
      }

      if (CollectionChanged is not null)
        CollectionChanged(this, result);
    }

    /// <inheritdoc />
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    #region IList<T>

    /// <inheritdoc />
    public int IndexOf(T item)
    {
      int r = _coll.IndexOf(item);
      return r < 0 ? r : _coll.Count - r;
    }

    /// <inheritdoc />
    public void Insert(int index, T item)
    {
      throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void RemoveAt(int index)
    {
      throw new NotImplementedException();
    }

    /// <inheritdoc />
    public T this[int index]
    {
      get
      {
        return _coll[_coll.Count - 1 - index];
      }
      set
      {
        throw new NotImplementedException();
      }
    }

    /// <inheritdoc />
    public void Add(T item)
    {
      throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void Clear()
    {
      throw new NotImplementedException();
    }

    /// <inheritdoc />
    public bool Contains(T item)
    {
      return _coll.Contains(item);
    }

    /// <inheritdoc />
    public void CopyTo(T[] array, int arrayIndex)
    {
      for (int i = _coll.Count - 1; i >= 0; i--)
        array[arrayIndex++] = _coll[i];
    }

    /// <inheritdoc />
    public int Count
    {
      get { return _coll.Count; }
    }

    /// <inheritdoc />
    public bool IsReadOnly
    {
      get { return true; }
    }

    /// <inheritdoc />
    public bool Remove(T item)
    {
      throw new NotImplementedException();
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator()
    {
      for (int i = _coll.Count - 1; i >= 0; i--)
        yield return _coll[i];
    }

    /// <inheritdoc/>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      for (int i = _coll.Count - 1; i >= 0; i--)
        yield return _coll[i];
    }

    #endregion IList<T>
  }
}
