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
  /// Manages a bunch of cached objects. The cached object are accessed by a key. In this instance more than one object with the same key can be stored, but it is presumed that all objects which
  /// are stored under the same key are considered equal.
  /// Usage: Try to take out a cached object with <see cref="TryTake(TKey, out TValue)"/>. If no such object exists, create a new one. After usage of the object, put it back with <see cref="Add(TKey, TValue)"/>.
  /// If after adding an object more than the allowed number of objects exists, the object which is used the least will be removed from that instance.
  /// The methods in this instance are thread safe.
  /// </summary>
  /// <typeparam name="TKey">The type of the key that characterizes the value.</typeparam>
  /// <typeparam name="TValue">The type of the value.</typeparam>
  /// <remarks>
  /// Two collections are neccessary to store the items: The values are stored in a doubly linked list as KeyValuePairs together with their key.
  /// The first item in the doubly linked list is the most recently added item. The last item in the doubly linked list is the least used item.
  /// To have fast access to the items by key, a dictionary can be accessed by those keys. The values of this dictionary are HashSets
  /// of LinkedListNodes, these LinkedListNodes that are members of the doubly linked list, and whose values are associated with the key.
  /// </remarks>
  public sealed class CachedObjectManagerByMaximumNumberOfItems<TKey, TValue> where TKey : notnull
  {
    private object _syncObj;

    /// <summary>
    /// Saves the LinkedListNodes that are associated with a key in a HashSet that is accessible by the key.
    /// </summary>
    private Dictionary<TKey, HashSet<LinkedListNode<KeyValuePair<TKey, TValue>>>> _keyDictionary;

    /// <summary>
    /// Saves all stored values together with their key.
    /// </summary>
    private LinkedList<KeyValuePair<TKey, TValue>> _valueList;

    /// <summary>
    /// Maximum number of items that are allowed to store here. If more items are added, the least used item is removed from the collection.
    /// </summary>
    private int _maxItemsAllowedToStore;

    /// <summary>
    /// Initializes a new instance of the <see cref="CachedObjectManagerByMaximumNumberOfItems{TKey, TValue}"/> class.
    /// </summary>
    /// <param name="maximumNumberOfItems">The maximum number of items allowed to store.</param>
    /// <exception cref="System.ArgumentException">maxItemsAllowedToStore must be > 0</exception>
    public CachedObjectManagerByMaximumNumberOfItems(int maximumNumberOfItems)
    {
      if (maximumNumberOfItems <= 0)
        throw new ArgumentException("maxItemsAllowedToStore must be > 0");

      _syncObj = new object();
      _maxItemsAllowedToStore = maximumNumberOfItems;
      _keyDictionary = new Dictionary<TKey, HashSet<LinkedListNode<KeyValuePair<TKey, TValue>>>>();
      _valueList = new LinkedList<KeyValuePair<TKey, TValue>>();
    }

    /// <summary>
    /// Try to take a stored object from this collection.
    /// If successfull, this function will return any object that was stored under the given key.
    /// </summary>
    /// <param name="key">The key of the object.</param>
    /// <param name="value">On success, the object taken from this collection.</param>
    /// <returns>True if the object was found; false otherwise.</returns>
    public bool TryTake(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
      lock (_syncObj)
      {
        return InternalTryTake_Unlocked(key, out value);
      }
    }

    /// <summary>
    /// Adds an object under the specified key.
    /// </summary>
    /// <param name="key">The key under which to store the object.</param>
    /// <param name="value">The value to store.</param>
    public void Add(TKey key, TValue value)
    {
      lock (_syncObj)
      {
        InternalAdd_Unlocked(key, value);
      }
    }

    /// <summary>
    /// Removes all items in the collection. If the values implement <see cref="IDisposable"/>, the items are disposed.
    /// </summary>
    public void Clear()
    {
      KeyValuePair<TKey, TValue>[] itemsToDispose;
      lock (_syncObj)
      {
        itemsToDispose = _valueList.ToArray();
        _valueList.Clear();
        _keyDictionary.Clear();
      }

      foreach (var itemToDispose in itemsToDispose.OfType<IDisposable>())
        itemToDispose.Dispose();
    }

    private bool InternalTryTake_Unlocked(TKey key, [MaybeNullWhen(false)] out TValue value)
    {

      if (_keyDictionary.TryGetValue(key, out var nodeSet))
      {
        var firstStackItem = nodeSet.First();
        nodeSet.Remove(firstStackItem);
        var linkedListItem = firstStackItem.Value;
        value = linkedListItem.Value;
        _valueList.Remove(linkedListItem);
        if (nodeSet.Count == 0)
          _keyDictionary.Remove(key);
        return true;
      }
      else
      {
        value = default;
        return false;
      }
    }

    private void InternalAdd_Unlocked(TKey key, TValue value)
    {
      if (!_keyDictionary.TryGetValue(key, out var nodeSet))
      {
        nodeSet = new HashSet<LinkedListNode<KeyValuePair<TKey, TValue>>>();
        _keyDictionary.Add(key, nodeSet);
      }

      // add new items as first in the list. Thus the least used items are at the end of the list.
      var linkedListItem = _valueList.AddFirst(new KeyValuePair<TKey, TValue>(key, value));
      nodeSet.Add(linkedListItem);

      if (_valueList.Count > _maxItemsAllowedToStore)
      {
        InternalRemoveLastItem_Unlocked();
      }
    }

    private void InternalRemoveLastItem_Unlocked()
    {
      if (_valueList.Last is { } lastItem)
      {
        _valueList.RemoveLast();
        var keyRemove = lastItem.Value.Key;
        var valueRemove = lastItem.Value.Value;
        var nodeSet = _keyDictionary[keyRemove];
        nodeSet.Remove(lastItem);
        if (0 == nodeSet.Count)
          _keyDictionary.Remove(keyRemove);

        if (valueRemove is IDisposable)
          ((IDisposable)valueRemove).Dispose();
      }
    }
  }
}
