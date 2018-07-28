#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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
using System.Linq;
using System.Text;

namespace Altaxo.Collections
{
  public class PartialDictionary<TKey, TBaseValue, TDerivValue> : IDictionary<TKey, TDerivValue> where TDerivValue : class where TBaseValue : class
  {
    private IDictionary<TKey, TBaseValue> _parent;

    public PartialDictionary(IDictionary<TKey, TBaseValue> parent)
    {
      _parent = parent;
    }

    public TDerivValue this[TKey key]
    {
      get
      {
        var result = _parent[key];
        return result as TDerivValue;
      }

      set
      {
        if (value == null)
          throw new ArgumentNullException(nameof(value), "It is not allowed to store null values in this partial dictionary.");
        var baseValue = value as TBaseValue;
        if (baseValue == null)
          throw new ArgumentException(nameof(value) + " can not be converted to the base value.");

        _parent[key] = baseValue;
      }
    }

    public int Count
    {
      get
      {
        int count = 0;
        foreach (var entry in _parent)
          if (entry.Value is TDerivValue)
            ++count;

        return count;
      }
    }

    public bool IsReadOnly
    {
      get
      {
        return _parent.IsReadOnly;
      }
    }

    public ICollection<TKey> Keys
    {
      get
      {
        var l = new List<TKey>();
        foreach (var entry in _parent)
        {
          if (entry.Value is TDerivValue)
            l.Add(entry.Key);
        }
        return l.AsReadOnly();
      }
    }

    public ICollection<TDerivValue> Values
    {
      get
      {
        var l = new List<TDerivValue>();
        foreach (var entry in _parent)
        {
          var derivValue = entry.Value as TDerivValue;
          if (null != derivValue)
            l.Add(derivValue);
        }
        return l.AsReadOnly();
      }
    }

    public void Add(KeyValuePair<TKey, TDerivValue> item)
    {
      Add(item.Key, item.Value);
    }

    public void Add(TKey key, TDerivValue value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof(value), "It is not allowed to store null values in this partial dictionary.");
      var baseValue = value as TBaseValue;
      if (baseValue == null)
        throw new ArgumentException(nameof(value) + " can not be converted to the base value.");

      _parent.Add(key, baseValue);
    }

    public void Clear()
    {
      var s = new List<KeyValuePair<TKey, TBaseValue>>();
      foreach (var entry in _parent)
        if (entry.Value is TDerivValue)
          s.Add(entry);

      foreach (var k in s)
        _parent.Remove(k);
    }

    public bool Contains(KeyValuePair<TKey, TDerivValue> item)
    {
      return _parent.Contains(new KeyValuePair<TKey, TBaseValue>(item.Key, item.Value as TBaseValue));
    }

    public bool ContainsKey(TKey key)
    {
      TBaseValue b;
      if (!_parent.TryGetValue(key, out b))
        return false;

      return b is TDerivValue;
    }

    public void CopyTo(KeyValuePair<TKey, TDerivValue>[] array, int arrayIndex)
    {
      int i = arrayIndex;
      foreach (var entry in _parent)
      {
        var d = entry.Value as TDerivValue;
        if (null != d)
          array[i++] = new KeyValuePair<TKey, TDerivValue>(entry.Key, d);
      }
    }

    public IEnumerator<KeyValuePair<TKey, TDerivValue>> GetEnumerator()
    {
      foreach (var entry in _parent)
        if (entry.Value is TDerivValue)
          yield return new KeyValuePair<TKey, TDerivValue>(entry.Key, entry.Value as TDerivValue);
    }

    public bool Remove(KeyValuePair<TKey, TDerivValue> item)
    {
      TBaseValue b;
      if (!_parent.TryGetValue(item.Key, out b))
        return false;
      if (!(object.ReferenceEquals(item.Value, b)))
        return false;
      return _parent.Remove(item.Key);
    }

    public bool Remove(TKey key)
    {
      TBaseValue b;
      if (!_parent.TryGetValue(key, out b))
        return false;
      if (!(b is TDerivValue))
        return false;
      return _parent.Remove(key);
    }

    public bool TryGetValue(TKey key, out TDerivValue value)
    {
      TBaseValue d;
      if (!_parent.TryGetValue(key, out d))
      {
        value = default(TDerivValue);
        return false;
      }
      value = d as TDerivValue;
      return null != value;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      foreach (var entry in _parent)
        if (entry.Value is TDerivValue)
          yield return new KeyValuePair<TKey, TDerivValue>(entry.Key, entry.Value as TDerivValue);
    }
  }
}
