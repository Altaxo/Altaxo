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

namespace Altaxo.Collections
{
  /// <summary>
  /// Represents a contiguous range of integers.
  /// </summary>
  public interface IContiguousIntegerRange : IReadOnlyList<int>, IAscendingIntegerCollection
  {
    /// <inheritdoc/>
    int Start { get; }
  }

  /// <summary>
  /// Represents a range of consecutive integers. The range can be empty (in this case both <see cref="Start"/> as well as <see cref="Count"/> are zero).
  /// </summary>
  public struct ContiguousIntegerRange : IContiguousIntegerRange
  {
    private int _start;
    private uint _count;

    #region Serialization of ContiguousIntegerRange

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Collections.IntegerRangeAsCollection", 0)]
    private class XmlSerializationSurrogate00 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new NotImplementedException("Should not serialize deprecated type");
        /*
                IntegerRangeAsCollection s = (IntegerRangeAsCollection)obj;
                info.AddValue("Start", s._start);
                info.AddValue("Count", s._count);
                */
      }
      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var start = info.GetInt32("Start");
        var count = info.GetInt32("Count");
        return ContiguousIntegerRange.FromStartAndCount(start, count);
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ContiguousIntegerRange), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ContiguousIntegerRange)obj;
        info.AddValue("Start", s.Start);
        info.AddValue("Count", s.Count);
      }
      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var start = info.GetInt32("Start");
        var count = info.GetInt32("Count");
        return ContiguousIntegerRange.FromStartAndCount(start, count);
      }
    }

    #endregion Serialization of ContiguousIntegerRange

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
    public static ContiguousIntegerRange FromFirstAndLastInclusive(int start, int last)
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
    public static ContiguousIntegerRange FromStartAndEndExclusive(int start, int end)
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
    public static ContiguousIntegerRange Empty
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
    /// Element immediately <b>after</b> the last valid element (is equal to <see cref="Start"/>+<see cref="Count"/>).
    /// </summary>
    public int EndExclusive
    {
      get
      {
        return _start + (int)_count;
      }
    }

    /// <inheritdoc/>
    public int Count
    {
      get
      {
        if (_count > int.MaxValue || (_count == 0 && _start != 0))
          throw new InvalidOperationException("This range is too large, thus integer is not sufficient for Count. Use LongCount instead.");
        return (int)_count;
      }
    }

    /// <summary>
    /// Number of elements of the integer range. Thus the integer range contains Start, Start+1, .. Start+Count-1.
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

    /// <inheritdoc/>
    /// <summary>
    /// Returns the zero-based index of the specified item within the range.
    /// </summary>
    /// <param name="item">The item to locate.</param>
    /// <returns>The zero-based index of <paramref name="item"/> if it is contained in the range; otherwise, <c>-1</c>.</returns>
    public int IndexOf(int item)
    {
      if (item >= _start && item <= LastInclusive)
        return item - _start;
      else
        return -1;
    }

    /// <inheritdoc/>
    /// <summary>
    /// Throws because this range is read-only.
    /// </summary>
    /// <param name="index">The insertion index.</param>
    /// <param name="item">The item to insert.</param>
    public void Insert(int index, int item)
    {
      throw new InvalidOperationException("This instance is read-only");
    }

    /// <inheritdoc/>
    /// <summary>
    /// Throws because this range is read-only.
    /// </summary>
    /// <param name="index">The index of the item to remove.</param>
    public void RemoveAt(int index)
    {
      throw new InvalidOperationException("This instance is read-only");
    }

    /// <inheritdoc/>
    /// <summary>
    /// Gets the item at the specified zero-based index.
    /// </summary>
    /// <param name="index">The zero-based index.</param>
    /// <returns>The item at the specified index.</returns>
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

    /// <summary>
    /// Throws because this range is read-only.
    /// </summary>
    /// <param name="item">The item to add.</param>
    public void Add(int item)
    {
      throw new InvalidOperationException("This instance is read-only");
    }

    /// <summary>
    /// Throws because this range is read-only.
    /// </summary>
    public void Clear()
    {
      throw new InvalidOperationException("This instance is read-only");
    }

    /// <summary>
    /// Determines whether the specified item is contained in the range.
    /// </summary>
    /// <param name="item">The item to test.</param>
    /// <returns><see langword="true"/> if the item is contained in the range; otherwise, <see langword="false"/>.</returns>
    public bool Contains(int item)
    {
      return (item >= _start && item <= LastInclusive);
    }

    /// <summary>
    /// Copies the contents of the range into the specified array.
    /// </summary>
    /// <param name="array">The destination array.</param>
    /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
    public void CopyTo(int[] array, int arrayIndex)
    {
      var lastInclusive = LastInclusive;
      for (int i = _start, j = arrayIndex; i <= lastInclusive && j < array.Length; ++i, ++j)
        array[j] = i;
    }

    /// <summary>
    /// Gets a value indicating that this range is read-only.
    /// </summary>
    public bool IsReadOnly
    {
      get { return true; }
    }

    /// <summary>
    /// Throws because this range is read-only.
    /// </summary>
    /// <param name="item">The item to remove.</param>
    /// <returns>This method never returns because it always throws.</returns>
    public bool Remove(int item)
    {
      throw new InvalidOperationException("This instance is read-only");
    }

    /// <summary>
    /// Returns an enumerator that iterates through all integers in the range.
    /// </summary>
    /// <returns>An enumerator for the integers in the range.</returns>
    public IEnumerator<int> GetEnumerator()
    {
      var lastInclusive = LastInclusive;

      for (int i = _start; i < lastInclusive; ++i) // use < instead of <= in order to avoid problems if lastInclusive==int.MaxValue
        yield return i;
      if (_start <= lastInclusive)
        yield return lastInclusive;
    }

    /// <inheritdoc/>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    #endregion IList interface

    #region IAscendingIntegerCollection

    /// <inheritdoc/>
    public IEnumerable<ContiguousIntegerRange> RangesAscending
    {
      get
      {
        yield return this;
      }
    }

    /// <inheritdoc/>
    public IEnumerable<ContiguousIntegerRange> RangesDescending
    {
      get
      {
        yield return this;
      }
    }

    #endregion IAscendingIntegerCollection

    #region ICloneable Members

    /// <inheritdoc/>
    /// <summary>
    /// Creates a copy of this range.
    /// </summary>
    /// <returns>A copy of this range.</returns>
    public object Clone()
    {
      return new ContiguousIntegerRange { _start = _start, _count = _count };
    }

    #endregion ICloneable Members
  }
}
