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

#nullable enable
using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Collections
{
  /// <summary>
  /// Class which holds unique items in the order in which they are added (like a List, but with the difference that only unique items can be contained).
  /// </summary>
  /// <typeparam name="T">Type of item.</typeparam>
  public class SetList<T> : ICollection<T>, IReadOnlyList<T> where T: notnull
  {
    /// <summary>
    /// Internal list storing items in insertion order.
    /// </summary>
    private List<T> _itemList = new List<T>();
    /// <summary>
    /// Internal dictionary mapping items to their index in the list.
    /// </summary>
    private Dictionary<T, int> _itemHash = new Dictionary<T, int>();

    #region ICollection<T> Members

    /// <inheritdoc/>
    public void Add(T item)
    {
      if (_itemHash.Count == 0)
        EnsureHashNotNull();

      if (_itemHash.ContainsKey(item))
        throw new ArgumentException("The item is already contained in the collection");

      _itemHash.Add(item, _itemList.Count);
      _itemList.Add(item);
    }

    /// <inheritdoc/>
    public bool Contains(T item)
    {
      if (_itemHash.Count == 0)
        EnsureHashNotNull();
      return _itemHash.ContainsKey(item);
    }

    /// <inheritdoc/>
    public void Clear()
    {
      _itemList.Clear();
      InvalidateHash();
    }

    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex)
    {
      _itemList.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public int Count
    {
      get { return _itemList.Count; }
    }

    /// <inheritdoc/>
    public bool IsReadOnly
    {
      get { return false; }
    }

    /// <inheritdoc/>
    public bool Remove(T item)
    {
      if (_itemHash.Count > 0)
      {
        if (_itemHash.ContainsKey(item))
        {
          _itemList.RemoveAt(_itemHash[item]);
          InvalidateHash();
          return true;
        }
        else
        {
          return false;
        }
      }
      else // item hash is null
      {
        for (int i = Count - 1; i >= 0; i--)
        {
          if (_itemList[i].Equals(item))
          {
            _itemList.RemoveAt(i);

            return true;
          }
        }
        return false;
      }
    }

    #endregion ICollection<T> Members

    #region IEnumerable<T> Members

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
      return _itemList.GetEnumerator();
    }

    #endregion IEnumerable<T> Members

    #region IEnumerable Members

    /// <inheritdoc/>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return _itemList.GetEnumerator();
    }

    #endregion IEnumerable Members

    #region other members

    /// <summary>
    /// Clears the internal hash dictionary.
    /// </summary>
    private void InvalidateHash()
    {
      _itemHash.Clear();
    }

    /// <summary>
    /// Rehashes the hash list if structural changes occur.
    /// </summary>
    private void EnsureHashNotNull()
    {
      if (_itemHash.Count > 0)
        return;

      for (int i = 0; i < _itemList.Count; i++)
      {
        _itemHash.Add(_itemList[i], i);
      }
    }

    /// <summary>
    /// Gets the index of the specified item.
    /// </summary>
    /// <param name="item">The item to search for.</param>
    /// <returns>The index of the item, or -1 if not found.</returns>
    public int IndexOf(T item)
    {
      if (_itemHash.Count == 0)
        EnsureHashNotNull();

      return _itemHash.TryGetValue(item, out var result) ? result : -1;
    }

    /// <inheritdoc/>
    public T this[int i]
    {
      get
      {
        return _itemList[i];
      }
    }

    /// <summary>
    /// Converts the list to an array.
    /// </summary>
    /// <returns>Array of items in insertion order.</returns>
    public T[] ToArray()
    {
      return _itemList.ToArray();
    }

    #endregion other members
  }
}
