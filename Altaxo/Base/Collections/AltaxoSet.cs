#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
using System.Text;

namespace Altaxo.Collections
{
  /// <summary>
  /// Class which holds unique items in the order in wich they are added
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class AltaxoSet<T> : ICollection<T>
  {
    List<T> _itemList = new List<T>();
    Dictionary<T, int> _itemHash = new Dictionary<T, int>();

  

    #region ICollection<T> Members


    public void Add(T item)
    {
      if (_itemHash == null)
        EnsureHashNotNull();

      if (_itemHash.ContainsKey(item))
        throw new ArgumentException("The item is already contained in the collection");

      _itemHash.Add(item, _itemList.Count);
      _itemList.Add(item);
    }

    public bool Contains(T item)
    {
      if(_itemHash==null)
        EnsureHashNotNull();
      return _itemHash.ContainsKey(item);
    }

    public void Clear()
    {
      _itemList.Clear();
      InvalidateHash();
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
      _itemList.CopyTo(array, arrayIndex);
    }

    public int Count
    {
      get { return _itemList.Count; }
    }

    public bool IsReadOnly
    {
      get { return false; }
    }

    public bool Remove(T item)
    {
      if(_itemHash!=null)
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

    #endregion

    #region IEnumerable<T> Members

    public IEnumerator<T> GetEnumerator()
    {
      return _itemList.GetEnumerator();
    }

    #endregion

    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return _itemList.GetEnumerator();
    }

    #endregion

#region other members

    void InvalidateHash()
    {
      _itemHash = null;
    }

    /// <summary>
    /// Rehashes the hash list if structural changes occur.
    /// </summary>
    void EnsureHashNotNull()
    {
      if (null != _itemHash)
        return;

      _itemHash = new Dictionary<T, int>();
      for (int i = 0; i < _itemList.Count;i++)
      {
        _itemHash.Add(_itemList[i], i);
      }
    }


    public int IndexOf(T item)
    {
      if (_itemHash == null)
        EnsureHashNotNull();

      int result;
      return _itemHash.TryGetValue(item, out result) ? result : -1;
    }

    public T this[int i]
    {
      get
      {
        return _itemList[i];
      }
    }

#endregion

  }
}
