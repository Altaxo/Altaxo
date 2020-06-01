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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Altaxo.Collections
{
  /// <summary>
  /// Concurrent linked list with the restriction that its members are unique, i.e. an item identified by a key can appear only once in the list.
  /// </summary>
  /// <typeparam name="TKey">The type of the key (token).</typeparam>
  /// <typeparam name="TValue">The type of the value.</typeparam>
  public class ConcurrentTokenizedLinkedList<TKey, TValue> where TKey : notnull
  {
    private LinkedList<Tuple<TKey, TValue>> _list;
    private Dictionary<TKey, LinkedListNode<Tuple<TKey, TValue>>> _dictionary;
    private System.Threading.ReaderWriterLockSlim _lock;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConcurrentTokenizedLinkedList{TKey, TValue}"/> class.
    /// </summary>
    public ConcurrentTokenizedLinkedList()
    {
      _list = new LinkedList<Tuple<TKey, TValue>>();
      _dictionary = new Dictionary<TKey, LinkedListNode<Tuple<TKey, TValue>>>();
      _lock = new System.Threading.ReaderWriterLockSlim();
    }

    /// <summary>
    /// Gets the number of items in the list.
    /// </summary>
    /// <value>
    /// Number of items in the list.
    /// </value>
    public int Count
    {
      get
      {
        return _list.Count;
      }
    }

    /// <summary>
    /// Clears all items in the list.
    /// </summary>
    public void Clear()
    {
      _lock.EnterWriteLock();
      try
      {
        _list.Clear();
        _dictionary.Clear();
      }
      finally
      {
        _lock.ExitWriteLock();
      }
    }

    /// <summary>
    /// Tries to take the first item from the list.
    /// </summary>
    /// <param name="key">If successful, contains the key that was used to add the item.</param>
    /// <param name="value">if successful, contains the value of the item.</param>
    /// <returns>True if the operation was successful. If the list contains no items, the return value is false.</returns>
    public bool TryTakeFirst([MaybeNullWhen(false)] out TKey key, [MaybeNullWhen(false)] out TValue value)
    {
      _lock.EnterUpgradeableReadLock();
      try
      {
        if (_list.First is { } node)
        {
          key = node.Value.Item1;
          value = node.Value.Item2;
          _lock.EnterWriteLock();
          try
          {
            _list.RemoveFirst();
            _dictionary.Remove(key);
            return true;
          }
          finally
          {
            _lock.ExitWriteLock();
          }
        }
        else
        {
          key = default;
          value = default;
          return false;
        }
      }
      finally
      {
        _lock.ExitUpgradeableReadLock();
      }
    }

    /// <summary>
    /// Tries to take the last item from the list.
    /// </summary>
    /// <param name="key">If successful, contains the key that was used to add the item.</param>
    /// <param name="value">if successful, contains the value of the item.</param>
    /// <returns>True if the operation was successful. If the list contains no items, the return value is false.</returns>
    public bool TryTakeLast([MaybeNullWhen(false)] out TKey key, [MaybeNullWhen(false)] out TValue value)
    {
      _lock.EnterUpgradeableReadLock();
      try
      {
        if (_list.Last is { } node)
        {
          key = node.Value.Item1;
          value = node.Value.Item2;
          _lock.EnterWriteLock();
          try
          {
            _list.RemoveLast();
            _dictionary.Remove(key);
            return true;
          }
          finally
          {
            _lock.ExitWriteLock();
          }
        }
        else
        {
          key = default;
          value = default;
          return false;
        }
      }
      finally
      {
        _lock.ExitUpgradeableReadLock();
      }
    }

    /// <summary>
    /// Tries to add an item as the first item of the list.
    /// </summary>
    /// <param name="key">The key that identifies the item.</param>
    /// <param name="value">The item's value.</param>
    /// <returns>True if the item was added to the list. If another item with the same key was already present in the list, the item is not added to the list, and the return value is <c>false</c>.</returns>
    public bool TryAddFirst(TKey key, TValue value)
    {
      _lock.EnterUpgradeableReadLock();
      try
      {
        if (!_dictionary.ContainsKey(key))
        {
          _lock.EnterWriteLock();
          try
          {
            var node = _list.AddFirst(new Tuple<TKey, TValue>(key, value));
            _dictionary.Add(key, node);
            return true;
          }
          finally
          {
            _lock.ExitWriteLock();
          }
        }
        else
        {
          return false;
        }
      }
      finally
      {
        _lock.ExitUpgradeableReadLock();
      }
    }

    /// <summary>
    /// Tries to add an item as the last item of the list.
    /// </summary>
    /// <param name="key">The key that identifies the item.</param>
    /// <param name="value">The item's value.</param>
    /// <returns>True if the item was added to the list. If another item with the same key was already present in the list, the item is not added to the list, and the return value is false.</returns>
    public bool TryAddLast(TKey key, TValue value)
    {
      _lock.EnterUpgradeableReadLock();
      try
      {
        if (!_dictionary.ContainsKey(key))
        {
          _lock.EnterWriteLock();
          try
          {
            var node = _list.AddLast(new Tuple<TKey, TValue>(key, value));
            _dictionary.Add(key, node);
            return true;
          }
          finally
          {
            _lock.ExitWriteLock();
          }
        }
        else
        {
          return false;
        }
      }
      finally
      {
        _lock.ExitUpgradeableReadLock();
      }
    }
  }
}
