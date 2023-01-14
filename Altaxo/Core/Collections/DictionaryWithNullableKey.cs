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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Altaxo.Collections
{
  /// <summary>
  /// Provides a dictionary, in which one of the keys (and only one) can be null.
  /// </summary>
  /// <typeparam name="TKey">Key type.</typeparam>
  /// <typeparam name="TValue">Value type.</typeparam>
  public class DictionaryWithNullableKey<TKey, TValue> : IDictionary<TKey, TValue> where TKey : notnull
  {
    /// <summary>Underlying dictionary for the normal keys (without the null key).</summary>
    private Dictionary<TKey, TValue> _dict = new Dictionary<TKey, TValue>();

    /// <summary>True if the value for the null key is set.</summary>
    private bool _nullValueSet;

    /// <summary>Value corresponding to the null key. Only valid if <see cref="_nullValueSet"/> is True.</summary>
#nullable disable
    private TValue _nullValue;
#nullable enable

    #region IDictionary<TKey,TValue> Members

    public void Add([MaybeNull] TKey key, TValue value)
    {
      if (key is null)
      {
        if (_nullValueSet)
          throw new ArgumentException("Null key can not be added because it is already set");
        _nullValue = value;
        _nullValueSet = true;
      }
      else
        _dict.Add(key, value);
    }

    public bool ContainsKey(TKey key)
    {
      return key is null ? _nullValueSet : _dict.ContainsKey(key);
    }

    public ICollection<TKey> Keys
    {
      get { throw new NotImplementedException(); }
    }

    public bool Remove(TKey key)
    {
      if (key is null)
      {
        bool wasSet = _nullValueSet;
        _nullValueSet = false;
        _nullValue = default;
        return wasSet;
      }
      else
      {
        return _dict.Remove(key);
      }
    }

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
      if (key is null)
      {
        if (_nullValueSet)
        {
          value = _nullValue;
          return true;
        }
        else
        {
          value = default;
          return false;
        }
      }
      else
        return _dict.TryGetValue(key, out value);
    }

    public ICollection<TValue> Values
    {
      get { throw new NotImplementedException(); }
    }

    public TValue this[TKey key]
    {
      get
      {
        if (key is null)
        {
          if (_nullValueSet)
            return _nullValue;
          else
            throw new ArgumentException("Null key is not present in this dictionary");
        }
        else
          return _dict[key];
      }
      set
      {
        if (key is null)
        {
          _nullValue = value;
          _nullValueSet = true;
        }
        else
          _dict[key] = value;
      }
    }

    #endregion IDictionary<TKey,TValue> Members

    #region ICollection<KeyValuePair<TKey,TValue>> Members

    public void Add(KeyValuePair<TKey, TValue> item)
    {
      if (item.Key is null)
      {
        if (_nullValueSet)
          throw new ArgumentException("Null key can not be added because it is already set");
        _nullValue = item.Value;
        _nullValueSet = true;
      }
      else
        _dict.Add(item.Key, item.Value);
    }

    public void Clear()
    {
      _dict.Clear();
      _nullValueSet = false;
      _nullValue = default;
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
      return ContainsKey(item.Key);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
      throw new NotImplementedException();
    }

    public int Count
    {
      get { return _nullValueSet ? _dict.Count : _dict.Count + 1; }
    }

    public bool IsReadOnly
    {
      get { return false; }
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
      return Remove(item.Key);
    }

    #endregion ICollection<KeyValuePair<TKey,TValue>> Members

    #region IEnumerable<KeyValuePair<TKey,TValue>> Members

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
      throw new NotImplementedException();
    }

    #endregion IEnumerable<KeyValuePair<TKey,TValue>> Members

    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      throw new NotImplementedException();
    }

    #endregion IEnumerable Members
  }
}
