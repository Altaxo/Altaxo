using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Collections
{
  public class DictionaryWithNullableKey<TKey,TValue> : IDictionary<TKey,TValue>
  {
    Dictionary<TKey, TValue> _dict = new Dictionary<TKey, TValue>();
    bool _nullValueSet;
    TValue _nullValue;

    #region IDictionary<TKey,TValue> Members

    public void Add(TKey key, TValue value)
    {
      if (null == key)
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
      return null == key ? _nullValueSet : _dict.ContainsKey(key); 
    }

    public ICollection<TKey> Keys
    {
      get { throw new NotImplementedException(); }
    }

    public bool Remove(TKey key)
    {
      if (null == key)
      {
        bool wasSet = _nullValueSet;
        _nullValueSet = false;
        _nullValue = default(TValue);
        return wasSet;
      }
      else
      {
        return _dict.Remove(key);
      }
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
      if (null == key)
      {
        if (_nullValueSet)
        {
          value = _nullValue;
          return true;
        }
        else
        {
          value = default(TValue);
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
        if (null == key)
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
        if (null == key)
        {
          _nullValue = value; _nullValueSet = true;
        }
        else
          _dict[key] = value;
      }
    }

    #endregion

    #region ICollection<KeyValuePair<TKey,TValue>> Members

    public void Add(KeyValuePair<TKey, TValue> item)
    {
      if (null == item.Key)
      {
        if (_nullValueSet)
          throw new ArgumentException("Null key can not be added because it is already set");
        _nullValue = item.Value;
        _nullValueSet = true;
      }
      else
        _dict.Add(item.Key,item.Value);
    }

    public void Clear()
    {
      _dict.Clear();
      _nullValueSet = false;
      _nullValue = default(TValue);
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

    #endregion

    #region IEnumerable<KeyValuePair<TKey,TValue>> Members

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
      throw new NotImplementedException();
    }

    #endregion

    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}
