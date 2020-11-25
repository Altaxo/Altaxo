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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Altaxo.Collections
{
  public class PartialDictionary<TKey, TBaseValue, TDerivValue> : IDictionary<TKey, TDerivValue> where TKey : notnull where TBaseValue : class where TDerivValue : class, TBaseValue
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
        return result as TDerivValue ?? throw new InvalidOperationException();
      }

      set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(value), "It is not allowed to store null values in this partial dictionary.");
        var baseValue = value as TBaseValue;
        if (baseValue is null)
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
          if (derivValue is not null)
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
      if (value is null)
        throw new ArgumentNullException(nameof(value), "It is not allowed to store null values in this partial dictionary.");
      var baseValue = value as TBaseValue;
      if (baseValue is null)
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
      return _parent.Contains(new KeyValuePair<TKey, TBaseValue>(item.Key, (TBaseValue)(item.Value)));
    }

    public bool ContainsKey(TKey key)
    {
      if (!_parent.TryGetValue(key, out var b))
        return false;

      return b is TDerivValue;
    }

    public void CopyTo(KeyValuePair<TKey, TDerivValue>[] array, int arrayIndex)
    {
      int i = arrayIndex;
      foreach (var entry in _parent)
      {
        var d = entry.Value as TDerivValue;
        if (d is not null)
          array[i++] = new KeyValuePair<TKey, TDerivValue>(entry.Key, d);
      }
    }

    public IEnumerator<KeyValuePair<TKey, TDerivValue>> GetEnumerator()
    {
      foreach (var entry in _parent)
        if (entry.Value is TDerivValue tdvalue)
          yield return new KeyValuePair<TKey, TDerivValue>(entry.Key, tdvalue);
    }

    public bool Remove(KeyValuePair<TKey, TDerivValue> item)
    {
      if (!_parent.TryGetValue(item.Key, out var b))
        return false;
      if (!(object.ReferenceEquals(item.Value, b)))
        return false;
      return _parent.Remove(item.Key);
    }

    public bool Remove(TKey key)
    {
      if (!_parent.TryGetValue(key, out var b))
        return false;
      if (!(b is TDerivValue))
        return false;
      return _parent.Remove(key);
    }

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TDerivValue value)
    {
      if (_parent.TryGetValue(key, out var d))
      {
        value = d as TDerivValue;
        return !(value is null);
      }
      else
      {
        value = null;
        return false;
      }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      foreach (var entry in _parent)
        if (entry.Value is TDerivValue tdvalue)
          yield return new KeyValuePair<TKey, TDerivValue>(entry.Key, tdvalue);
    }
  }
}
