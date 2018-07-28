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

using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Collections
{
  /// <summary>
  /// Represents a contiguous range of integers.
  /// </summary>
  public interface IContiguousIntegerRange : IReadOnlyList<int>, IAscendingIntegerCollection
  {
    /// <summary>
    /// Gets the first element of this integer range.
    /// </summary>
    int Start { get; }
  }

  /// <summary>
  /// Represents a range of consecutive integers. The range can be empty (in this case both <see cref="Start"/> as well as <see cref="Count"/> are zero).
  /// </summary>
  public struct ContiguousIntegerRange : IContiguousIntegerRange
  {
    private int _start;
    private uint _count;

    /// <summary>
    /// Constructs an integer range from another integer range.
    /// </summary>
    /// <param name="from">The integer range to copy from.</param>
    public ContiguousIntegerRange(IContiguousIntegerRange from)
    {
      AssertValidStartAndCount(from.Start, from.Count);
      _start = from.Start;
      _count = (uint)from.Count;
    }

    private static void AssertValidStartAndCount(int start, uint count)
    {
      if (start > int.MinValue)
      {
        uint maxcnt = (uint)(int.MaxValue - start);
        if (count > maxcnt)
          throw new ArgumentOutOfRangeException("Start + Count exceeds maximum range of integer values.");
      }
    }

    private static void AssertValidStartAndCount(int start, int count)
    {
      if (count < 0)
        throw new ArgumentOutOfRangeException("count is negative");
      if (start > int.MinValue)
      {
        uint maxcnt = (uint)(int.MaxValue - start);
        if (count > maxcnt)
          throw new ArgumentOutOfRangeException("Start + Count exceeds maximum range of integer values.");
      }
    }

    /// <summary>
    /// Constructs an integer range from the first element and the number of element of the range.
    /// An empty range (with start=0 and count=0) is constructed if <paramref name="count"/> is zero.
    /// </summary>
    /// <param name="start">First element belonging to the range.</param>
    /// <param name="count">Number of consecutive integers belonging to the range.</param>
    /// <returns>Newly constructed integer range.</returns>
    public static ContiguousIntegerRange FromStartAndCount(int start, int count)
    {
      if (0 == count)
        return new ContiguousIntegerRange();

      AssertValidStartAndCount(start, count);
      return new ContiguousIntegerRange { _start = start, _count = (uint)count };
    }

    /// <summary>
    /// Constructs an integer range from the first and the last element of the range.
    /// Note that because <paramref name="last"/> has to be greater than or equal to <paramref name="start"/>, it is not possible to create an empty range with this method.
    /// </summary>
    /// <param name="start">First element belonging to the range.</param>
    /// <param name="last">Last element belonging to the range.</param>
    /// <returns>Newly constructed integer range.</returns>
    static public ContiguousIntegerRange FromFirstAndLastInclusive(int start, int last)
    {
      if (!(last >= start))
        throw new ArgumentOutOfRangeException("Last has to be greater than or equal to start");

      return new ContiguousIntegerRange { _start = start, _count = (uint)(1 + (last - start)) };
    }

    /// <summary>
    /// Constructs an integer range from the first element and the element following immediately after the last element.
    /// An empty range  (with start=0 and count=0) is constructed if <paramref name="start"/> is equal to <paramref name="end"/>.
    /// </summary>
    /// <param name="start">First element belonging to the range.</param>
    /// <param name="end">Element following immediately after the last element, i.e. <see cref="LastInclusive"/>+1.</param>
    /// <returns>Newly constructed integer range.</returns>
    static public ContiguousIntegerRange FromStartAndEndExclusive(int start, int end)
    {
      if (end == start)
        return new ContiguousIntegerRange(); // return an empty range
      else if (end > start)
        return new ContiguousIntegerRange { _start = start, _count = (uint)(end - start) };
      else
        throw new ArgumentOutOfRangeException("End has to be greater than start");
    }

    /// <summary>
    /// Gets a standard empty integer range (<see cref="Start"/> and <see cref="Count"/> set to zero).
    /// </summary>
    static public ContiguousIntegerRange Empty
    {
      get
      {
        return new ContiguousIntegerRange();
      }
    }

    /// <summary>
    /// Returns true if the range is empty, i.e. has no elements.
    /// </summary>
    public bool IsEmpty
    {
      get
      {
        return _count == 0 && _start == 0;
      }
    }

    /// <summary>
    /// Start value of the integer range.
    /// </summary>
    public int Start
    {
      get { return _start; }
    }

    /// <summary>
    /// Start value of the integer range.
    /// </summary>
    public int First
    {
      get { return _start; }
    }

    /// <summary>
    /// Last valid element of the integer range (is equal to <see cref="Start"/>+<see cref="Count"/>-1).
    /// </summary>
    public int LastInclusive
    {
      get
      {
        return _start - 1 + (int)_count;
      }
    }

    /// <summary>
    /// Element immmediately <b>after</b> the last valid element (is equal to <see cref="Start"/>+<see cref="Count"/>).
    /// </summary>
    public int EndExclusive
    {
      get
      {
        return _start + (int)_count;
      }
    }

    /// <summary>
    /// Number of elements of the integer range.Thus the integer range contains Start, Start+1, .. Start+Count-1.
    /// </summary>
    public int Count
    {
      get
      {
        if (_count > int.MaxValue || (_count == 0 && _start != 0))
          throw new InvalidOperationException("This range is to large, thus integer is not sufficient for Count. Use LongCount instead.");
        return (int)_count;
      }
    }

    /// <summary>
    /// Number of elements of the integer range.Thus the integer range contains Start, Start+1, .. Start+Count-1.
    /// </summary>
    public long LongCount
    {
      get
      {
        if (_count == 0 && _start != 0)
          return 1L + uint.MaxValue;
        else
          return _count;
      }
    }

    #region IList interface

    public int IndexOf(int item)
    {
      if (item >= _start && item <= LastInclusive)
        return item - _start;
      else
        return -1;
    }

    public void Insert(int index, int item)
    {
      throw new InvalidOperationException("This instance is read-only");
    }

    public void RemoveAt(int index)
    {
      throw new InvalidOperationException("This instance is read-only");
    }

    public int this[int index]
    {
      get
      {
        if (index < 0 || !(_start + index <= LastInclusive))
          throw new IndexOutOfRangeException("index");
        return _start + index;
      }
      set
      {
        throw new InvalidOperationException("This instance is read-only");
      }
    }

    public void Add(int item)
    {
      throw new InvalidOperationException("This instance is read-only");
    }

    public void Clear()
    {
      throw new InvalidOperationException("This instance is read-only");
    }

    public bool Contains(int item)
    {
      return (item >= _start && item <= LastInclusive);
    }

    public void CopyTo(int[] array, int arrayIndex)
    {
      var lastInclusive = LastInclusive;
      for (int i = _start, j = arrayIndex; i <= lastInclusive && j < array.Length; ++i, ++j)
        array[j] = i;
    }

    public bool IsReadOnly
    {
      get { return true; }
    }

    public bool Remove(int item)
    {
      throw new InvalidOperationException("This instance is read-only");
    }

    public IEnumerator<int> GetEnumerator()
    {
      var lastInclusive = LastInclusive;

      for (int i = _start; i <= lastInclusive; ++i)
        yield return i;
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    #endregion IList interface

    #region IAscendingIntegerCollection

    public IEnumerable<ContiguousIntegerRange> RangesAscending
    {
      get
      {
        yield return this;
      }
    }

    public IEnumerable<ContiguousIntegerRange> RangesDescending
    {
      get
      {
        yield return this;
      }
    }

    #endregion IAscendingIntegerCollection

    #region ICloneable Members

    public object Clone()
    {
      return new ContiguousIntegerRange { _start = this._start, _count = this._count };
    }

    #endregion ICloneable Members
  }
}
