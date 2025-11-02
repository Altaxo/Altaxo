using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Altaxo.Collections
{
  /// <summary>
  /// Dictionary that retains the order of the entries by which they are added, thus effectively combining a list (of its values) with fast access by a key.
  /// The only two methods that can modify this class are <see cref="Clear"/> and <see cref="Add(TKey, TValue)"/>.
  /// </summary>
  /// <typeparam name="TKey">The type of the key.</typeparam>
  /// <typeparam name="TValue">The type of the value.</typeparam>
  /// <seealso cref="Altaxo.Collections.IReadOnlyListDictionary&lt;TKey, TValue&gt;" />
  public class ListDictionary<TKey, TValue> : IReadOnlyListDictionary<TKey, TValue> where TKey : notnull
  {
    private List<(TKey Key, TValue Value)> _list = new();
    private Dictionary<TKey, int> _dictionary = new();

    public TValue this[int index] => _list[index].Value;

    public TValue this[TKey key] => _list[_dictionary[key]].Value;

    public int Count => _list.Count;

    public IEnumerable<TKey> Keys => _list.Select(x => x.Key);


    public IEnumerable<TValue> Values => _list.Select(x => x.Value);

    public bool ContainsKey(TKey key)
    {
      return _dictionary.ContainsKey(key);
    }

    public IEnumerator<TValue> GetEnumerator()
    {
      for (int i = 0; i < _list.Count; i++)
        yield return _list[i].Value;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
      if (_dictionary.TryGetValue(key, out int index))
      {
        value = _list[index].Value;
        return true;
      }
      else
      {
        value = default;
        return false;
      }
    }

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
    {
      for (int i = 0; i < _list.Count; i++)
        yield return new KeyValuePair<TKey, TValue>(_list[i].Key, _list[i].Value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    #region Modification methods

    /// <summary>
    /// Clears all entries.
    /// </summary>
    public void Clear()
    {
      _list.Clear();
      _dictionary.Clear();
    }

    /// <summary>
    /// Adds and entry.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public void Add(TKey key, TValue value)
    {
      _dictionary.Add(key, _list.Count);
      _list.Add((key, value));
    }

    #endregion
  }
}

