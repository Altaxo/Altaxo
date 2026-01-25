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
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Altaxo.Collections
{
  /// <summary>
  /// Represents a generic ring buffer. This class is <b>not</b> thread safe.
  /// Note that there is no automatic overflow handling; adding an item to a full buffer throws an exception.
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
    /// The number of elements in the buffer.
    /// </summary>
    private int _count;

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
        throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be greater than 1");

      _array = new T[capacity];
    }

    /// <summary>
    /// Clears (empties) the ring buffer.
    /// </summary>
    public void Clear()
    {
      _insertionPoint = _removalPoint = 0;
      _count = 0;
    }

    /// <summary>
    /// Gets the number of elements in the ring buffer.
    /// </summary>
    public int Count
    {
      get
      {
        return _count;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is empty.
    /// </summary>
    public bool IsEmpty
    {
      get
      {
        return _count == 0;
      }
    }

    /// <summary>
    /// Adds the specified item to the buffer.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <exception cref="System.InvalidOperationException">Buffer overflow</exception>
    public void Add(T item)
    {
      if (_count == _array.Length)
        throw new InvalidOperationException("Buffer overflow");
      else
        _count++;

      _array[_insertionPoint] = item;
      ++_insertionPoint;
      if (_insertionPoint == _array.Length)
        _insertionPoint = 0;
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
      var maxCountToRead = Math.Min(_array.Length - _count, _array.Length - _insertionPoint);
      int readCount = 0;
      if (maxCountToRead > 0)
      {
        readCount = readFunc(_array, _insertionPoint, maxCountToRead);
        _count += readCount;
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
    public bool TryPeek([MaybeNullWhen(false)] out T item)
    {
      if (_count == 0)
      {
        item = default;
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
      if (!(index >= 0 && index < _count))
        throw new IndexOutOfRangeException(nameof(index));
      index += _removalPoint;
      if (index >= _array.Length)
        index -= _array.Length;
      return _array[index];
    }

    /// <summary>
    /// Gets the two items at index <paramref name="index"/> and <paramref name="index"/>+1.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns>The two items at index and index+1.</returns>
    /// <exception cref="System.IndexOutOfRangeException">index</exception>
    public (T, T) TwoItemsAt(int index)
    {
      if (!(index >= 0 && index < _count - 1))
        throw new IndexOutOfRangeException(nameof(index));

      index += _removalPoint;
      if (index >= _array.Length)
        index -= _array.Length;
      var item0 = _array[index];
      ++index;
      if (index == _array.Length)
        index = 0;
      var item1 = _array[index];
      return (item0, item1);
    }

    /// <summary>
    /// Gets the three items at index, index+1, and index+2.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns>The three items at index, index+1, and index+2.</returns>
    /// <exception cref="System.IndexOutOfRangeException">index</exception>
    public (T, T, T) ThreeItemsAt(int index)
    {
      if (!(index >= 0 && index < _count - 2))
        throw new IndexOutOfRangeException(nameof(index));

      index += _removalPoint;
      if (index >= _array.Length)
        index -= _array.Length;
      var item0 = _array[index];
      ++index;
      if (index == _array.Length)
        index = 0;
      var item1 = _array[index];
      ++index;
      if (index == _array.Length)
        index = 0;
      var item2 = _array[index];
      return (item0, item1, item2);
    }

    /// <summary>
    /// Gets the four items at index, index+1, index+2, and index+3.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns>The four items at index, index+1, index+2, and index+3.</returns>
    /// <exception cref="System.IndexOutOfRangeException">index</exception>
    public (T, T, T, T) FourItemsAt(int index)
    {
      if (!(index >= 0 && index < _count - 3))
        throw new IndexOutOfRangeException(nameof(index));

      index += _removalPoint;
      if (index >= _array.Length)
        index -= _array.Length;
      var item0 = _array[index];
      ++index;
      if (index == _array.Length)
        index = 0;
      var item1 = _array[index];
      ++index;
      if (index == _array.Length)
        index = 0;
      var item2 = _array[index];
      ++index;
      if (index == _array.Length)
        index = 0;
      var item3 = _array[index];

      return (item0, item1, item2, item3);
    }



    /// <summary>
    /// Enumerates one single item at a time.
    /// </summary>
    /// <returns>Enumeration of item and the corresponding index of the item.</returns>
    public IEnumerable<(T, int)> EnumerateOneItem()
    {
      int len = _array.Length;
      int count = _count;
      for (int idx = 0, i = _removalPoint; idx < count; ++idx, ++i)
      {
        if (i == len)
          i = 0;
        yield return (_array[i], idx);
      }
    }

    /// <summary>
    /// Enumerates two items at a time. ATTENTION: The forward step is nevertheless 1, i.e. in the next yield the second item is now the first one.
    /// </summary>
    /// <returns>Enumeration of 2 items and the corresponding index of the first item.</returns>
    public IEnumerable<(T, T, int)> EnumerateTwoItems()
    {
      var len = _array.Length;
      var countMinus1 = _count - 1;
      var previousItem = _array[_removalPoint];
      var rp = _removalPoint + 1;

      for (int idx = 0; idx < countMinus1; ++idx, ++rp)
      {
        if (rp == len)
          rp = 0;

        yield return (previousItem, _array[rp], idx);
        previousItem = _array[rp];
      }
    }

    /// <summary>
    /// Enumerates three items at a time. The forward step is nevertheless 1, i.e. in the next yield the second item is now the first one.
    /// </summary>
    /// <returns>Enumeration of 3 items and the corresponding index of the first item.</returns>
    public IEnumerable<(T, T, T, int)> EnumerateThreeItems()
    {
      var len = _array.Length;
      var countMinus2 = _count - 2;
      var prevPrevItem = _array[_removalPoint];
      var rp = _removalPoint + 1;
      if (rp == len)
        rp = 0;
      var prevItem = _array[rp];
      ++rp;

      for (int idx = 0; idx < countMinus2; ++idx, ++rp)
      {
        if (rp == len)
          rp = 0;

        yield return (prevPrevItem, prevItem, _array[rp], idx);
        prevPrevItem = prevItem;
        prevItem = _array[rp];
      }
    }

    /// <summary>
    /// Enumerates four items at a time. The forward step is nevertheless 1, i.e. in the next yield the second item is now the first one.
    /// </summary>
    /// <returns>Enumeration of 4 items and the corresponding index of the first item.</returns>
    public IEnumerable<(T, T, T, T, int)> EnumerateFourItems()
    {
      var len = _array.Length;
      var countMinus3 = _count - 3;
      var prevPrevPrevItem = _array[_removalPoint];
      var rp = _removalPoint + 1;
      if (rp == len)
        rp = 0;
      var prevPrevItem = _array[rp];
      ++rp;
      if (rp == len)
        rp = 0;
      var prevItem = _array[rp];
      ++rp;

      for (int idx = 0; idx < countMinus3; ++idx, ++rp)
      {
        if (rp == len)
          rp = 0;

        yield return (prevPrevPrevItem, prevPrevItem, prevItem, _array[rp], idx);
        prevPrevPrevItem = prevPrevItem;
        prevPrevItem = prevItem;
        prevItem = _array[rp];
      }
    }

    /// <summary>
    /// Enumerates one single item at a time.
    /// </summary>
    /// <param name="increment">The increment. An increment of 1 enumerates every item in the buffer, an increment of 2 every second item, etc.</param>
    /// <returns>Enumeration of item and the corresponding index of the item.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">increment</exception>
    public IEnumerable<(T, int)> EnumerateOneItem(int increment)
    {
      ArgumentOutOfRangeException.ThrowIfNegativeOrZero(increment);

      return EnumerateOneItem().SkipWhile(e => (e.Item2 % increment) != 0);
    }

    /// <summary>
    /// Enumerates two items at a time.
    /// </summary>
    /// <param name="increment">The increment. An increment of 1 steps forward by 1, an increment of 2 steps forward by 2, etc.</param>
    /// <returns>Enumeration of 2 items and the corresponding index of the first returned item.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">increment</exception>
    public IEnumerable<(T, T, int)> EnumerateTwoItems(int increment)
    {
      ArgumentOutOfRangeException.ThrowIfNegativeOrZero(increment);

      return EnumerateTwoItems().SkipWhile(e => (e.Item3 % increment) != 0);
    }


    /// <summary>
    /// Enumerates three items at a time.
    /// </summary>
    /// <param name="increment">The increment. An increment of 1 steps forward by 1, an increment of 2 steps forward by 2, etc.</param>
    /// <returns>Enumeration of 3 items and the corresponding index of the first returned item.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">increment</exception>
    public IEnumerable<(T, T, T, int)> EnumerateThreeItems(int increment)
    {
      ArgumentOutOfRangeException.ThrowIfNegativeOrZero(increment);

      return EnumerateThreeItems().SkipWhile(e => (e.Item4 % increment) != 0);
    }


    /// <summary>
    /// Enumerates four items at a time.
    /// </summary>
    /// <param name="increment">The increment. An increment of 1 steps forward by 1, an increment of 2 steps forward by 2, etc.</param>
    /// <returns>Enumeration of 4 items and the corresponding index of the first returned item.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">increment</exception>
    public IEnumerable<(T, T, T, T, int)> EnumerateFourItems(int increment)
    {
      ArgumentOutOfRangeException.ThrowIfNegativeOrZero(increment);

      return EnumerateFourItems().SkipWhile(e => (e.Item5 % increment) != 0);
    }



    /// <summary>
    /// Removes the n oldest items from the buffer.
    /// </summary>
    /// <param name="numberOfItems">The number of items.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">numberOfItems</exception>
    public void RemoveItems(int numberOfItems)
    {
      if (!(numberOfItems >= 0 && numberOfItems <= _count))
        throw new ArgumentOutOfRangeException(nameof(numberOfItems));

      _removalPoint += numberOfItems;
      if (_removalPoint >= _array.Length)
        _removalPoint -= _array.Length;
      _count -= numberOfItems;
    }

    /// <summary>
    /// Try to remove an item from the buffer.
    /// </summary>
    /// <param name="item">If successful, the item that was removed.</param>
    /// <returns>True if an item could be removed; otherwise, false.</returns>
    public bool TryRemove([MaybeNullWhen(false)] out T item)
    {
      if (_count == 0)
      {
        item = default;
        return false;
      }
      else
      {
        item = _array[_removalPoint];
        _count--;
        _removalPoint++;
        if (_removalPoint == _array.Length)
          _removalPoint = 0;
        return true;
      }
    }
  }
}
