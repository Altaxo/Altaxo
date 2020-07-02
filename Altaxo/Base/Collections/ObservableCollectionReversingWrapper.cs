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
  /// <typeparam name="T"></typeparam>
  public class ObservableCollectionReversingWrapper<T> : IList<T>, IReadOnlyList<T>, INotifyCollectionChanged
  {
    private ObservableCollection<T> _coll;

    public ObservableCollectionReversingWrapper(ObservableCollection<T> coll)
    {
      _coll = coll;
      _coll.CollectionChanged += new WeakEventHandler<NotifyCollectionChangedEventArgs>(EhOriginalCollectionChanged, _coll, nameof(_coll.CollectionChanged)).EventSink;
    }

    private void EhOriginalCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
      NotifyCollectionChangedEventArgs result;

      int newStart = e.NewItems != null ? _coll.Count - e.NewStartingIndex - e.NewItems.Count : e.NewStartingIndex;

      int oldStart = e.OldItems != null ? _coll.Count - e.OldStartingIndex - e.OldItems.Count : e.OldStartingIndex;

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

      if (null != CollectionChanged)
        CollectionChanged(this, result);
    }

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    #region IList<T>

    public int IndexOf(T item)
    {
      int r = _coll.IndexOf(item);
      return r < 0 ? r : _coll.Count - r;
    }

    public void Insert(int index, T item)
    {
      throw new NotImplementedException();
    }

    public void RemoveAt(int index)
    {
      throw new NotImplementedException();
    }

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

    public void Add(T item)
    {
      throw new NotImplementedException();
    }

    public void Clear()
    {
      throw new NotImplementedException();
    }

    public bool Contains(T item)
    {
      return _coll.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
      for (int i = _coll.Count - 1; i >= 0; i--)
        array[arrayIndex++] = _coll[i];
    }

    public int Count
    {
      get { return _coll.Count; }
    }

    public bool IsReadOnly
    {
      get { return true; }
    }

    public bool Remove(T item)
    {
      throw new NotImplementedException();
    }

    public IEnumerator<T> GetEnumerator()
    {
      for (int i = _coll.Count - 1; i >= 0; i--)
        yield return _coll[i];
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      for (int i = _coll.Count - 1; i >= 0; i--)
        yield return _coll[i];
    }

    #endregion IList<T>
  }
}
