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
using System.Collections.Generic;
using System.Linq;

namespace Altaxo.Collections
{
  /// <summary>
  /// Represents a generic ring buffer. This class is <b>not</b> thread safe.
  /// </summary>
  /// <typeparam name="T">Type of elements to store in the ring buffer.</typeparam>
  public class RingBuffer<T>
  {
    /// <summary>
    /// The index for the next insertion.
    /// </summary>
    private int _insertionPoint;
    /// <summary>
    /// The index for the next removal.
    /// </summary>
    private int _removalPoint;
    /// <summary>
    /// The underlying array storing buffer elements.
    /// </summary>
    private T[] _array;

    /// <summary>
    /// Initializes a new instance of the <see cref="RingBuffer{T}"/> class.
    /// </summary>
    /// <param name="capacity">The (fixed) capacity of the ring buffer. Has to be greater than 1.</param>
    public RingBuffer(int capacity)
    {
      if (!(capacity > 1))
        throw new ArgumentOutOfRangeException("Capacity must be greater than 1");

      _array = new T[capacity];
    }

    /// <summary>
    /// Clears (empties) the ring buffer.
    /// </summary>
    public void Clear()
    {
      _insertionPoint = _removalPoint = 0;
    }

    /// <summary>
    /// Gets the number of elements in the ring buffer.
    /// </summary>
    public int Count
    {
      get
      {
        int diff = _insertionPoint - _removalPoint;
        if (diff < 0)
          diff += _array.Length;
        return diff;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is empty.
    /// </summary>
    public bool IsEmpty
    {
      get
      {
        return _insertionPoint == _removalPoint;
      }
    }

    /// <summary>
    /// Adds the specified item to the buffer.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <exception cref="System.InvalidOperationException">Buffer overflow</exception>
    public void Add(T item)
    {
      _array[_insertionPoint] = item;
      int newInsertionPoint = _insertionPoint + 1;
      if (newInsertionPoint >= _array.Length)
        newInsertionPoint = 0;
      if (newInsertionPoint == _removalPoint)
        throw new InvalidOperationException("Buffer overflow");
      else
        _insertionPoint = newInsertionPoint;
    }

    /// <summary>
    /// Adds items to the buffer by using a read function that reads such items. For instance, <see cref="System.IO.Stream.Read(byte[], int, int)"/> is such a function.
    /// </summary>
    /// <param name="readFunc">The read function.
    /// 1st argument is the buffer array,
    /// 2nd argument is the position where to put the first data to,
    /// and 3rd argument is the maximum number of items to read.
    /// The return value has to be the number of items that were actually read.</param>
    /// <returns>The number of items that were added to the ring buffer.</returns>
    public int Add(Func<T[], int, int, int> readFunc)
    {
      int maxCountToRead;
      if (_removalPoint > _insertionPoint)
        maxCountToRead = _removalPoint - _insertionPoint - 1;
      else
        maxCountToRead = _array.Length - _insertionPoint;

      int readCount = 0;
      if (maxCountToRead > 0)
      {
        readCount = readFunc(_array, _insertionPoint, maxCountToRead);
        _insertionPoint += readCount;
        if (_insertionPoint == _array.Length)
          _insertionPoint = 0;
        return readCount;
      }
      return readCount;
    }

    /// <summary>
    /// Try to peek an item.
    /// </summary>
    /// <param name="item">If successful, the item.</param>
    /// <returns>True if there was an item in the buffer; false otherwise.</returns>
    public bool TryPeek(out T item)
    {
      if (_insertionPoint == _removalPoint)
      {
        item = default(T);
        return false;
      }
      else
      {
        item = _array[_removalPoint];
        return true;
      }
    }

    /// <summary>
    /// Gets the item at index. The index zero refers to the oldest item in the buffer, 1 to the second oldest item, etc.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns>The item at index <paramref name="index"/>.</returns>
    /// <exception cref="System.IndexOutOfRangeException">index</exception>
    public T ItemAt(int index)
    {
      if (index < 0 || index >= Count)
        throw new IndexOutOfRangeException(nameof(index));
      index += _insertionPoint;
      if (index >= _array.Length)
        index -= _array.Length;
      return _array[index];
    }

    /// <summary>
    /// Enumerates one single item at a time.
    /// </summary>
    /// <param name="increment">The increment. An increment of 1 enumerates every item in the buffer, an increment of 2 every second item, etc.</param>
    /// <returns>Enumeration of item and the corresponding index of the item.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">increment</exception>
    public IEnumerable<(T, int)> EnumerateOneItem(int increment)
    {
      if (increment <= 0)
        throw new ArgumentOutOfRangeException(nameof(increment));

      return EnumerateOneItem().SkipWhile(e => (e.Item2 % increment) != 0);
    }

    /// <summary>
    /// Enumerates one single item at a time.
    /// </summary>
    /// <returns>Enumeration of item and the corresponding index of the item.</returns>
    public IEnumerable<(T, int)> EnumerateOneItem()
    {
      int idx = 0;
      if (_removalPoint <= _insertionPoint)
      {
        for (int i = _removalPoint; i < _insertionPoint; ++i)
          yield return (_array[i], idx++);
      }
      else
      {
        for (int i = _removalPoint; i < _array.Length; ++i)
          yield return (_array[i], idx++);
        for (int i = 0; i < _insertionPoint; ++i)
          yield return (_array[i], idx++);
      }
    }

    /// <summary>
    /// Gets the two items at index <paramref name="index"/> and <paramref name="index"/>+1.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns>The two items at index and index+1.</returns>
    /// <exception cref="System.IndexOutOfRangeException">index</exception>
    public (T, T) TwoItemsAt(int index)
    {
      if (!(index >= 0 && index < Count - 1))
        throw new IndexOutOfRangeException(nameof(index));
      index += _insertionPoint;
      if (index >= _array.Length)
        index -= _array.Length;
      var item0 = _array[index];
      ++index;
      if (index >= _array.Length)
        index -= _array.Length;
      var item1 = _array[index];
      return (item0, item1);
    }


    /// <summary>
    /// Enumerates two items at a time.
    /// </summary>
    /// <param name="increment">The increment. An increment of 1 steps forward by 1, an increment of 2 steps forward by 2, etc.</param>
    /// <returns>Enumeration of 2 items and the corresponding index of the first returned item.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">increment</exception>
    public IEnumerable<(T, T, int)> EnumerateTwoItems(int increment)
    {
      if (increment <= 0)
        throw new ArgumentOutOfRangeException(nameof(increment));

      return EnumerateTwoItems().SkipWhile(e => (e.Item3 % increment) != 0);
    }

    /// <summary>
    /// Enumerates two items at a time. The forward step is nevertheless 1, i.e. in the next yield the second item is now the first one.
    /// </summary>
    /// <returns>Enumeration of 2 items and the corresponding index of the first item.</returns>
    public IEnumerable<(T, T, int)> EnumerateTwoItems()
    {
      int idx = 0;
      if (_removalPoint <= _insertionPoint)
      {
        for (int i = _removalPoint + 1; i < _insertionPoint; ++i)
          yield return (_array[i - 1], _array[i], idx++);
      }
      else
      {
        for (int i = _removalPoint + 1; i < _array.Length; ++i)
          yield return (_array[i - 1], _array[i], idx++);
        if (_insertionPoint > 0)
          yield return (_array[^1], _array[0], idx++);
        for (int i = 1; i < _insertionPoint; ++i)
          yield return (_array[i - 1], _array[i], idx++);
      }
    }


    /// <summary>
    /// Gets the three items at index, index+1, and index+2.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns>The three items at index, index+1, and index+2.</returns>
    /// <exception cref="System.IndexOutOfRangeException">index</exception>
    public (T, T, T) ThreeItemsAt(int index)
    {
      if (!(index >= 0 && index < Count - 2))
        throw new IndexOutOfRangeException(nameof(index));
      index += _insertionPoint;
      if (index >= _array.Length)
        index -= _array.Length;
      var item0 = _array[index];
      ++index;
      if (index >= _array.Length)
        index -= _array.Length;
      var item1 = _array[index];
      ++index;
      if (index >= _array.Length)
        index -= _array.Length;
      var item2 = _array[index];
      return (item0, item1, item2);
    }

    /// <summary>
    /// Enumerates three items at a time.
    /// </summary>
    /// <param name="increment">The increment. An increment of 1 steps forward by 1, an increment of 2 steps forward by 2, etc.</param>
    /// <returns>Enumeration of 3 items and the corresponding index of the first returned item.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">increment</exception>
    public IEnumerable<(T, T, T, int)> EnumerateThreeItems(int increment)
    {
      if (increment <= 0)
        throw new ArgumentOutOfRangeException(nameof(increment));

      foreach (var x in EnumerateThreeItems())
      {
        if (0 == (x.Item4 % increment))
          yield return x;
      }
    }

    /// <summary>
    /// Enumerates three items at a time. The forward step is nevertheless 1, i.e. in the next yield the second item is now the first one.
    /// </summary>
    /// <returns>Enumeration of 3 items and the corresponding index of the first item.</returns>
    public IEnumerable<(T, T, T, int)> EnumerateThreeItems()
    {
      int idx = 0;
      if (_removalPoint <= _insertionPoint)
      {
        for (int i = _removalPoint + 2; i < _insertionPoint; ++i)
          yield return (_array[i - 2], _array[i - 1], _array[i], idx++);
      }
      else
      {
        for (int i = _removalPoint + 2; i < _array.Length; ++i)
          yield return (_array[i - 2], _array[i - 1], _array[i], idx++);
        if (_insertionPoint > 0)
          yield return (_array[^2], _array[^1], _array[0], idx++);
        if (_insertionPoint > 1)
          yield return (_array[^1], _array[0], _array[1], idx++);
        for (int i = 2; i < _insertionPoint; ++i)
          yield return (_array[i - 2], _array[i - 2], _array[i], idx++);
      }
    }

    /// <summary>
    /// Gets the four items at index, index+1, index+2, and index+3.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns>The four items at index, index+1, index+2, and index+3.</returns>
    /// <exception cref="System.IndexOutOfRangeException">index</exception>
    public (T, T, T, T) FourItemsAt(int index)
    {
      if (!(index >= 0 && index < Count - 3))
        throw new IndexOutOfRangeException(nameof(index));
      index += _insertionPoint;
      if (index >= _array.Length)
        index -= _array.Length;
      var item0 = _array[index];
      ++index;
      if (index >= _array.Length)
        index -= _array.Length;
      var item1 = _array[index];
      ++index;
      if (index >= _array.Length)
        index -= _array.Length;
      var item2 = _array[index];
      ++index;
      if (index >= _array.Length)
        index -= _array.Length;
      var item3 = _array[index];

      return (item0, item1, item2, item3);
    }

    /// <summary>
    /// Enumerates four items at a time.
    /// </summary>
    /// <param name="increment">The increment. An increment of 1 steps forward by 1, an increment of 2 steps forward by 2, etc.</param>
    /// <returns>Enumeration of 4 items and the corresponding index of the first returned item.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">increment</exception>
    public IEnumerable<(T, T, T, T, int)> EnumerateFourItems(int increment)
    {
      if (increment <= 0)
        throw new ArgumentOutOfRangeException(nameof(increment));

      return EnumerateFourItems().SkipWhile(e => (e.Item5 % increment) != 0);
    }

    /// <summary>
    /// Enumerates four items at a time. The forward step is nevertheless 1, i.e. in the next yield the second item is now the first one.
    /// </summary>
    /// <returns>Enumeration of 4 items and the corresponding index of the first item.</returns>
    public IEnumerable<(T, T, T, T, int)> EnumerateFourItems()
    {
      int idx = 0;
      if (_removalPoint <= _insertionPoint)
      {
        for (int i = _removalPoint + 3; i < _insertionPoint; ++i)
          yield return (_array[i - 3], _array[i - 2], _array[i - 1], _array[i], idx++);
      }
      else
      {
        for (int i = _removalPoint + 3; i < _array.Length; ++i)
          yield return (_array[i - 3], _array[i - 2], _array[i - 1], _array[i], idx++);
        if (_insertionPoint > 0)
          yield return (_array[^3], _array[^2], _array[^1], _array[0], idx++);
        if (_insertionPoint > 1)
          yield return (_array[^2], _array[^1], _array[0], _array[1], idx++);
        if (_insertionPoint > 2)
          yield return (_array[^1], _array[0], _array[1], _array[2], idx++);
        for (int i = 3; i < _insertionPoint; ++i)
          yield return (_array[i - 3], _array[i - 2], _array[i - 2], _array[i], idx++);
      }
    }

    /// <summary>
    /// Removes the n oldest items from the buffer.
    /// </summary>
    /// <param name="numberOfItems">The number of items.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">numberOfItems</exception>
    public void RemoveItems(int numberOfItems)
    {
      if (numberOfItems < 0 || numberOfItems > Count)
        throw new ArgumentOutOfRangeException(nameof(numberOfItems));

      _removalPoint += numberOfItems;
      if (_removalPoint >= _array.Length)
        _removalPoint -= _array.Length;
    }

    /// <summary>
    /// Try to remove an item from the buffer.
    /// </summary>
    /// <param name="item">If successful, the item that was removed.</param>
    /// <returns>True if an item could be removed; otherwise, false.</returns>
    public bool TryRemove(out T item)
    {
      if (_insertionPoint == _removalPoint)
      {
        item = default(T);
        return false;
      }
      else
      {
        item = _array[_removalPoint];
        int newRemoval = _removalPoint + 1;
        if (newRemoval == _array.Length)
          newRemoval = 0;
        _removalPoint = newRemoval;
        return true;
      }
    }
  }
}
