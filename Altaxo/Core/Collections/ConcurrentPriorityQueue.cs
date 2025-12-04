#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2020 Dr. Dirk Lellinger
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
using System.Text;

#nullable enable

namespace Altaxo.Collections
{
  /// <summary>
  /// Implements a heap-based priority queue that holds only keys. The key with the minimum value can then be retrieved from the queue. This class is not thread safe.
  /// </summary>
  /// <typeparam name="TKey">The type of the key.</typeparam>
  /// <typeparam name="TValue">The type of the value.</typeparam>
  public class ConcurrentPriorityQueue<TKey, TValue> where TKey : IComparable<TKey>
  {
    /// <summary>
    /// The heap array storing the key-value pairs.
    /// </summary>
    private (TKey Key, TValue Value)[] _heap;
    /// <summary>
    /// The number of elements in the queue.
    /// </summary>
    private int _count;
    /// <summary>
    /// Object used to synchronize this queue.
    /// </summary>
    private System.Threading.ReaderWriterLockSlim _syncLock = new System.Threading.ReaderWriterLockSlim();

    /// <summary>
    /// Initializes a new instance of the <see cref="ConcurrentPriorityQueue{TKey, TValue}"/> class.
    /// </summary>
    public ConcurrentPriorityQueue()
    {
      _heap = new (TKey, TValue)[64];
      _count = 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConcurrentPriorityQueue{TKey, TValue}"/> class with a specified initial capacity.
    /// </summary>
    /// <param name="capacity">The initial capacity.</param>
    public ConcurrentPriorityQueue(int capacity)
    {
      _heap = new (TKey, TValue)[Math.Max(4, capacity)];
      _count = 0;
    }

    /// <summary>
    /// Gets the number of elements in the queue.
    /// </summary>
    /// <value>
    /// Number of elements in the queue.
    /// </value>
    public int Count
    {
      get
      {
        return _count;
      }
    }

    /// <summary>
    /// Gets a value indicating whether the queue is empty.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the queue is empty; otherwise, <c>false</c>.
    /// </value>
    public bool IsEmpty
    {
      get
      {
        return 0 == _count;
      }
    }

    /// <summary>
    /// Adds the specified key and value to the queue.
    /// </summary>
    /// <param name="key">The key value.</param>
    /// <param name="value">The value.</param>
    public void Enqueue(TKey key, TValue value)
    {
      _syncLock.EnterWriteLock();
      try
      {
        if (_count == _heap.Length)
        {
          var newArray = new (TKey, TValue)[_heap.Length * 2];
          Array.Copy(_heap, newArray, _count);
          _heap = newArray;
        }

        _heap[_count] = (key, value);
        UpHeap(_count);
        ++_count;
      }
      finally
      {
        _syncLock.ExitWriteLock();
      }
    }

    /// <summary>
    /// Peeks the element with the minimum key value. Returns true if the queue is not empty; otherwise, false.
    /// </summary>
    /// <param name="key">The minimum key value, if found.</param>
    /// <param name="value">The value associated with the minimum key, if found.</param>
    /// <returns>True if the queue is not empty; otherwise, false.</returns>
    public bool TryPeek([MaybeNullWhen(false)] out TKey key, [MaybeNullWhen(false)] out TValue value)
    {
      _syncLock.EnterReadLock();
      try
      {
        if (_count > 0)
        {
          (key, value) = _heap[0];
          return true;
        }
      }
      finally
      {
        _syncLock.ExitReadLock();
      }

      key = default;
      value = default;

      return false;
    }

    /// <summary>
    /// Dequeues the minimum key value. Returns true if the queue is not empty; otherwise, false.
    /// </summary>
    /// <param name="key">The minimum key value, if found.</param>
    /// <param name="value">The value associated with the minimum key, if found.</param>
    /// <returns>True if the queue is not empty; otherwise, false.</returns>
    public bool TryDequeue([MaybeNullWhen(false)] out TKey key, [MaybeNullWhen(false)] out TValue value)
    {
      _syncLock.EnterUpgradeableReadLock();
      try
      {
        if (_count > 0)
        {
          _syncLock.EnterWriteLock();
          try
          {
            (key, value) = _heap[0];
            _heap[0] = _heap[--_count];
            if (_count > 0)
              DownHeap(0);

            return true;
          }
          finally
          {
            _syncLock.ExitWriteLock();
          }
        }
      }
      finally
      {
        _syncLock.ExitUpgradeableReadLock();
      }

      key = default;
      value = default;

      return false;
    }

    /// <summary>
    /// Dequeues the minimum key value if the predicate returns true for the minimum key. Returns true if the item was dequeued; otherwise, false.
    /// </summary>
    /// <param name="predicate">Predicate to test the minimum key value.</param>
    /// <param name="key">The minimum key value, if found and dequeued.</param>
    /// <param name="value">The value associated with the minimum key, if found and dequeued.</param>
    /// <returns>True if the item was dequeued; otherwise, false.</returns>
    public bool TryDequeueIf(Func<TKey, bool> predicate, [MaybeNullWhen(false)] out TKey key, [MaybeNullWhen(false)] out TValue value)
    {
      if (predicate is null)
        throw new ArgumentNullException(nameof(predicate));

      _syncLock.EnterUpgradeableReadLock();
      try
      {
        if (_count > 0 && predicate(_heap[0].Key))
        {
          _syncLock.EnterWriteLock();
          try
          {
            (key, value) = _heap[0];
            _heap[0] = _heap[--_count];
            if (_count > 0)
              DownHeap(0);

            return true;
          }
          finally
          {
            _syncLock.ExitWriteLock();
          }
        }
      }
      finally
      {
        _syncLock.ExitUpgradeableReadLock();
      }

      key = default;
      value = default;

      return false;
    }

    /// <summary>
    /// Dequeues the minimum key value if the predicate returns true for the minimum key and value. Returns true if the item was dequeued; otherwise, false.
    /// </summary>
    /// <param name="predicate">Predicate to test the minimum key and value.</param>
    /// <param name="key">The minimum key value, if found and dequeued.</param>
    /// <param name="value">The value associated with the minimum key, if found and dequeued.</param>
    /// <returns>True if the item was dequeued; otherwise, false.</returns>
    public bool TryDequeueIf(Func<TKey, TValue, bool> predicate, [MaybeNullWhen(false)] out TKey key, [MaybeNullWhen(false)] out TValue value)
    {
      if (predicate is null)
        throw new ArgumentNullException(nameof(predicate));

      _syncLock.EnterUpgradeableReadLock();
      try
      {
        if (_count > 0 && predicate(_heap[0].Key, _heap[0].Value))
        {
          _syncLock.EnterWriteLock();
          try
          {
            (key, value) = _heap[0];
            _heap[0] = _heap[--_count];
            if (_count > 0)
              DownHeap(0);

            return true;
          }
          finally
          {
            _syncLock.ExitWriteLock();
          }
        }
      }
      finally
      {
        _syncLock.ExitUpgradeableReadLock();
      }

      key = default;
      value = default;

      return false;
    }

    /// <summary>
    /// Restores the heap property by moving the element at index <paramref name="k"/> up the heap.
    /// </summary>
    /// <param name="k">The index of the element to move up.</param>
    private void UpHeap(int k)
    {
      int km1_2;
      var v = _heap[k];
      while (k > 0 && _heap[km1_2 = ((k - 1) / 2)].Key.CompareTo(v.Key) >= 0)
      {
        _heap[k] = _heap[km1_2];
        k = km1_2;
      }
      _heap[k] = v;
    }

    /// <summary>
    /// Restores the heap property by moving the element at index <paramref name="k"/> down the heap.
    /// </summary>
    /// <param name="k">The index of the element to move down.</param>
    private void DownHeap(int k)
    {
      int j, jp1;
      var v = _heap[k];
      while ((j = (2 * k + 1)) < _count)
      {
        jp1 = j + 1;
        if (jp1 < _count && _heap[j].Key.CompareTo(_heap[jp1].Key) > 0)
          j = jp1;
        if (v.Key.CompareTo(_heap[j].Key) <= 0)
          break;
        _heap[k] = _heap[j];
        k = j;
      }
      _heap[k] = v;
    }
  }
}
