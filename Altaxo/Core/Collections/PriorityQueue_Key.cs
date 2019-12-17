#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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
using System.Linq;
using System.Text;

namespace Altaxo.Collections
{
  /// <summary>
  /// Implements a heap based priority queue that hold only keys. The key with the minimum value can then retrieved from the queue. This class is not thread safe.
  /// </summary>
  /// <typeparam name="TKey">The type of the key.</typeparam>
  public class PriorityQueue<TKey> where TKey : IComparable<TKey>
  {
    private TKey[] _heap;
    private int _count;

    /// <summary>
    /// Initializes a new instance of the <see cref="PriorityQueue{TKey}"/> class.
    /// </summary>
    public PriorityQueue()
    {
      _heap = new TKey[64];
      _count = 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PriorityQueue{TKey}"/> class with a specified initial capacity.
    /// </summary>
    /// <param name="capacity">The initial capacity.</param>
    public PriorityQueue(int capacity)
    {
      _heap = new TKey[capacity];
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
    /// Adds the specified key to the queue.
    /// </summary>
    /// <param name="val">The key value.</param>
    public void Enqueue(TKey val)
    {
      if (_count == _heap.Length)
      {
        var newArray = new TKey[_heap.Length * 2];
        Array.Copy(_heap, newArray, _count);
        _heap = newArray;
      }

      _heap[_count] = val;
      UpHeap(_count); // Optimization: properly, _count should be increased before UpHeap(), and UpHeap be called with argument (_count -1). But because UpHeap doesn't use _count, I can do this here
      ++_count;
    }

    /// <summary>
    /// Peeks the element with the minimum key value. An exception is thrown if the queue is empty.
    /// </summary>
    /// <returns>Minimum key value.</returns>
    /// <exception cref="System.InvalidOperationException">Queue is empty.</exception>
    public TKey Peek()
    {
      if (_count == 0)
        throw new InvalidOperationException("Queue is empty");

      return _heap[0];
    }

    /// <summary>
    /// Dequeues the minimum key value. An exception is thrown if the queue is empty.
    /// </summary>
    /// <returns>Minimum key value.</returns>
    /// <exception cref="System.InvalidOperationException">Queue is empty</exception>
    public TKey Dequeue()
    {
      if (_count == 0)
        throw new InvalidOperationException("Queue is empty");

      var result = _heap[0];
      _heap[0] = _heap[--_count];
      if (_count > 0)
        DownHeap(0);
      return result;
    }

    private void UpHeap(int k)
    {
      int km1_2;
      var v = _heap[k];
      while (k > 0 && _heap[km1_2 = ((k - 1) / 2)].CompareTo(v) >= 0)
      {
        _heap[k] = _heap[km1_2];
        k = km1_2;
      }
      _heap[k] = v;
    }

    private void DownHeap(int k)
    {
      int j, jp1;
      var v = _heap[k];
      while ((j = (2 * k + 1)) < _count)
      {
        jp1 = j + 1;
        if (jp1 < _count && _heap[j].CompareTo(_heap[jp1]) > 0)
          j = jp1;
        if (v.CompareTo(_heap[j]) <= 0)
          break;
        _heap[k] = _heap[j];
        k = j;
      }
      _heap[k] = v;
    }
  }
}
