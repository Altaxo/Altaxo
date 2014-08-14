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
	public interface IContiguousIntegerRange : IEnumerable<int>, IROVector<int>, IAscendingIntegerCollection
	{
		/// <summary>
		/// Gets the first element of this integer range.
		/// </summary>
		int Start { get; }
	}

	/// <summary>
	/// Represents a range of consecutive integers. The range is designated by FROM and TO values.
	/// </summary>
	public struct ContiguousIntegerRange : IContiguousIntegerRange
	{
		private int _start;
		private int _count;

		/// <summary>
		/// Constructs an integer range from the first element and the number of element of the range.
		/// </summary>
		/// <param name="start">First element belonging to the range.</param>
		/// <param name="count">Number of consecutive integers belonging to the range.</param>
		public ContiguousIntegerRange(int start, int count)
		{
			_start = start;
			_count = count;
			EnsureValidity();
		}

		/// <summary>
		/// Constructs an integer range from another integer range.
		/// </summary>
		/// <param name="from">The integer range to copy from.</param>
		public ContiguousIntegerRange(IContiguousIntegerRange from)
		{
			_start = from.Start;
			_count = from.Count;
			EnsureValidity();
		}

		/// <summary>
		/// Ensures the validity of the integer range.
		/// </summary>
		private void EnsureValidity()
		{
			if (_count < 0)
				throw new ArgumentOutOfRangeException("count", "Argument 'count' has to be positive");
		}

		/// <summary>
		/// Constructs an integer range from the first element and the number of element of the range.
		/// </summary>
		/// <param name="start">First element belonging to the range.</param>
		/// <param name="count">Number of consecutive integers belonging to the range.</param>
		/// <returns>Newly constructed integer range.</returns>
		public static ContiguousIntegerRange FromStartAndCount(int start, int count)
		{
			if (count < 0)
				throw new ArgumentOutOfRangeException("count", "Count must be a positive integer");

			return new ContiguousIntegerRange(start, count);
		}

		/// <summary>
		/// Constructs an infinitely extended integer range.
		/// </summary>
		/// <param name="start">First element belonging to the range.</param>
		/// <returns>Newly constructed integer range.</returns>
		public static ContiguousIntegerRange FromStartToInfinity(int start)
		{
			return new ContiguousIntegerRange(start, int.MaxValue);
		}

		/// <summary>
		/// Constructs an integer range from the first element and the element following immediately after the last element.
		/// </summary>
		/// <param name="start">First element belonging to the range.</param>
		/// <param name="end">Element following immediately after the last element, i.e. <see cref="LastInclusive"/>+1.</param>
		/// <returns>Newly constructed integer range.</returns>
		static public ContiguousIntegerRange FromStartAndEndExclusive(int start, int end)
		{
			return new ContiguousIntegerRange(start, end - start);
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
		/// Constructs an integer range from the first and the last element of the range.
		/// </summary>
		/// <param name="start">First element belonging to the range.</param>
		/// <param name="last">Last element belonging to the range.</param>
		/// <returns>Newly constructed integer range.</returns>
		static public ContiguousIntegerRange FromFirstAndLastInclusive(int start, int last)
		{
			return new ContiguousIntegerRange(start, 1 + (last - start));
		}

		/// <summary>
		/// Start value of the integer range.
		/// </summary>
		public int Start
		{
			get { return _start; }
		}

		/// <summary>
		/// Number of elements of the integer range.Thus the integer range contains Start, Start+1, .. Start+Count-1.
		/// </summary>
		public int Count
		{
			get { return _count; }
		}

		/// <summary>
		/// Last valid element of the integer range (is equal to <see cref="Start"/>+<see cref="Count"/>-1).
		/// </summary>
		public int LastInclusive
		{
			get
			{
				if ((int.MaxValue - _count) < (_start - 1))
					return int.MaxValue;
				else
					return _start + _count - 1;
			}
		}

		/// <summary>
		/// Element immmediately <b>after</b> the last valid element (is equal to <see cref="Start"/>+<see cref="Count"/>).
		/// </summary>
		public int EndExclusive
		{
			get
			{
				if ((int.MaxValue - _count) < (_start))
					return int.MaxValue;
				else
					return _start + _count;
			}
		}

		/// <summary>
		/// Returns true if the integer range is infinite.
		/// </summary>
		public bool IsInfinite
		{
			get
			{
				return _count == int.MaxValue;
			}
		}

		/// <summary>
		/// Returns true if the range is empty, i.e. has no elements.
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				return _count <= 0;
			}
		}

		#region IEnumerable<int> Members

		/// <summary>
		/// Enumerates all elements of this range.
		/// </summary>
		/// <returns>Enumerator for all elements</returns>
		public IEnumerator<int> GetEnumerator()
		{
			for (int i = 0; i < _count; i++)
				yield return i + _start;
		}

		#endregion IEnumerable<int> Members

		#region IEnumerable Members

		/// <summary>
		/// Enumerates all elements of this range.
		/// </summary>
		/// <returns>Enumerator for all elements</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			for (int i = 0; i < _count; i++)
				yield return i + _start;
		}

		#endregion IEnumerable Members

		#region IROVector<int> Members

		public int this[int i]
		{
			get
			{
				if (i >= 0 && i < _count)
					return _start + i;
				else if (i < 0)
					throw new ArgumentOutOfRangeException("i is negative");
				else
					throw new ArgumentOutOfRangeException("i is greater or equal than count");
			}
		}

		#endregion IROVector<int> Members

		#region IAscendingIntegerCollection Members

		public bool Contains(int nValue)
		{
			nValue -= _start;
			return nValue >= 0 && nValue < _count;
		}

		public bool GetNextRangeAscending(ref int currentposition, out ContiguousIntegerRange result)
		{
			if (currentposition < 0 || currentposition >= _count)
			{
				result = ContiguousIntegerRange.Empty;
				return false;
			}
			else
			{
				result = new ContiguousIntegerRange(_start, 1 + currentposition);
				currentposition = _count;
				return true;
			}
		}

		public bool GetNextRangeDescending(ref int currentposition, out ContiguousIntegerRange result)
		{
			if (currentposition < 0 || currentposition >= _count)
			{
				result = ContiguousIntegerRange.Empty;
				return false;
			}
			else
			{
				result = new ContiguousIntegerRange(_start, 1 + currentposition);
				currentposition = -1;
				return true;
			}
		}

		#endregion IAscendingIntegerCollection Members

		#region ICloneable Members

		public object Clone()
		{
			return new ContiguousIntegerRange(_start, _count);
		}

		#endregion ICloneable Members
	}

	/// <summary>
	/// Represents a range of consecutive integers.
	/// </summary>
	public struct Int32RangeFromTo : IList<int>
	{
		private int _start, _lastInclusive;

		/// <summary>
		/// First element of the range (inclusive).
		/// </summary>
		public int First { get { return _start; } }

		/// <summary>
		/// Last element of the range (inclusive).
		/// </summary>
		public int LastInclusive { get { return _lastInclusive; } }

		public static Int32RangeFromTo FromFirstAndLastInclusive(int first, int lastInclusive)
		{
			if (!(first <= lastInclusive))
				throw new ArgumentOutOfRangeException("Argument lastInclusive has to be greater than or equal to argument first");
			return new Int32RangeFromTo { _start = first, _lastInclusive = lastInclusive };
		}

		public static Int32RangeFromTo FromStartAndCount(int first, int count)
		{
			if (count < 1)
				throw new ArgumentOutOfRangeException("count must be at least one!");

			return new Int32RangeFromTo { _start = first, _lastInclusive = first + (count - 1) };
		}

		public int Count
		{
			get
			{
				try
				{
					checked
					{
						return (_lastInclusive - _start) + 1;
					}
				}
				catch (Exception)
				{
					throw new InvalidOperationException("Range is too large to fit in an integer. Use LongCount instead");
				}
			}
		}

		public long LongCount
		{
			get
			{
				return ((long)_lastInclusive - _start) + 1;
			}
		}

		#region IList interface

		public int IndexOf(int item)
		{
			if (item >= _start && item <= _lastInclusive)
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
				if (index < 0 || !(_start + index <= _lastInclusive))
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
			return (item >= _start && item <= _lastInclusive);
		}

		public void CopyTo(int[] array, int arrayIndex)
		{
			for (int i = _start, j = arrayIndex; i <= _lastInclusive && j < array.Length; ++i, ++j)
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
			for (int i = _start; i < _lastInclusive; ++i)
				yield return i;
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion IList interface
	}
}