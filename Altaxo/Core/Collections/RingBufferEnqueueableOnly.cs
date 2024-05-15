#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
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

namespace Altaxo.Collections
{
  /// <summary>
  /// Ring buffer that stores elements in a ring-buffer like fashion.
  /// Elements can only be enqueued, but not dequeued.
  /// The oldest element will be overwritten, if a new elements is enqueued and the capacity is reached.
  /// Elements can be accessed by index; the newest element is accessed by index 0.
  /// </summary>
  /// <typeparam name="T">Type of element to store.</typeparam>
  /// <remarks>This instance is thread-safe.</remarks>
  public class RingBufferEnqueueableOnly<T> : IReadOnlyList<T>
  {
    private T[] _arr;
    private int _count;
    private int _pointer;
    private object _syncObject;

    /// <summary>
    /// Initializes a new instance of the <see cref="RingBufferEnqueueableOnly{T}"/> class.
    /// </summary>
    /// <param name="capacity">The capacity of the buffer.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">capacity</exception>
    public RingBufferEnqueueableOnly(int capacity)
    {
      if (!(capacity > 0))
        throw new ArgumentOutOfRangeException(nameof(capacity));

      _arr = new T[capacity];
      _syncObject = new object();
    }

    /// <summary>
    /// Gets the number of elements in this buffer.
    /// </summary>
    /// <value>
    /// The number of elements in this buffer.
    /// </value>
    public int Count => _count;

    public bool IsEmpty => _count == 0;

    /// <summary>
    /// Gets the maximum number of elements that this buffer can store.
    /// </summary>
    /// <value>
    /// The maximum number of elements that this buffer can store.
    /// </value>
    public int Capacity => _arr.Length;

    /// <summary>
    /// Enqueues (add) an element.
    /// </summary>
    /// <param name="value">The value to add.</param>
    public void Enqueue(T value)
    {
      lock (_syncObject)
      {
        _arr[_pointer++] = value;
        if (_pointer >= _arr.Length)
        {
          _pointer = 0;
        }
        _count = Math.Min(_arr.Length, _count + 1);
      }
    }

    /// <summary>
    /// Gets the oldest element available. An exception is thrown if no element is enqueued.
    /// </summary>
    /// <value>
    /// The oldest element.
    /// </value>
    /// <exception cref="System.InvalidOperationException">No element present in ring buffer</exception>
    public T OldestValue
    {
      get
      {
        lock (_syncObject)
        {
          if (_count == 0)
            throw new InvalidOperationException("No element present in ring buffer");
          var idx = _pointer - _count;
          if (idx < 0)
          {
            idx += _arr.Length;
          }

          return _arr[idx];
        }
      }
    }

    /// <summary>
    /// Gets the newest element available. An exception is thrown if no element is enqueued.
    /// </summary>
    /// <value>
    /// The oldest element.
    /// </value>
    /// <exception cref="System.InvalidOperationException">No element present in ring buffer</exception>
    public T NewestValue
    {
      get
      {
        lock (_syncObject)
        {
          if (_count == 0)
            throw new InvalidOperationException("No element present in ring buffer");
          var idx = _pointer - 1;
          if (idx < 0)
          {
            idx += _arr.Length;
          }

          return _arr[idx];
        }
      }
    }

    /// <summary>
    /// Gets the element with the specified index. The newest element is at index 0.
    /// </summary>
    /// <param name="idx">The index.</param>
    /// <returns>The element at index.</returns>
    /// <exception cref="System.IndexOutOfRangeException"/>
    public T this[int idx]
    {
      get
      {
        if (idx < 0)
        {
          throw new IndexOutOfRangeException(nameof(idx));
        }

        lock (_syncObject)
        {
          if (!(idx < _count))
            throw new IndexOutOfRangeException($"idx={idx}, count={_count}");

          var idxArr = _pointer - idx - 1;
          if (idx < 0)
          {
            idx += _arr.Length;
          }

          return _arr[idx];
        }
      }
    }

    /// <summary>
    /// Converts to an array. The newest element is the first element in the returned array.
    /// </summary>
    /// <returns>Array, with the newest element at first index.</returns>
    public T[] ToArray()
    {
      lock (_syncObject)
      {
        var result = new T[_count];
        for (int i = 0, j = _pointer - 1; i < _count; ++i, --j)
        {
          if (j < 0)
          {
            j += _arr.Length;
          }
          result[i] = _arr[j];
        }
        return result;
      }
    }

    public IEnumerator<T> GetEnumerator()
    {
      return (IEnumerator<T>)ToArray().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ToArray().GetEnumerator();
    }
  }
}
