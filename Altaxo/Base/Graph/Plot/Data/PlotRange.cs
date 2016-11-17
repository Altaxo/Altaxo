#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

namespace Altaxo.Graph.Plot.Data
{
	/// <summary>
	/// PlotRange represents a range of plotting points from index
	/// lowerBound to (upperBound-1)
	/// I use a class instead of a struct because it is intended to use with
	/// <see cref="System.Collections.ArrayList" />.
	/// </summary>
	/// <remarks>For use in a list, the UpperBound property is somewhat useless, since it should be equal
	/// to the LowerBound property of the next item.</remarks>
	[Serializable]
	public class PlotRange : IPlotRange
	{
		/// <summary>
		/// First index of a contiguous plot range in the plot point array (i.e. in the array of processed plot point data, <b>not</b> in the original data column).
		/// To calculate from which row index in the original data column this comes from, add to this value <see cref="OffsetToOriginal"/>.
		/// </summary>
		private int _lowerBound;

		/// <summary>
		/// Last index + 1 of a contiguous plot range in the plot point array (i.e. in the array of processed plot point data, <b>not</b> in the original data column).
		/// To calculate from which row index in the original data column this comes from, add to this value <see cref="OffsetToOriginal"/>.
		/// </summary>
		private int _upperBound;

		/// <summary>
		/// This gives the offset of an index in the plot point array to the original data row index.
		/// </summary>
		/// <example>If the LowerBound=4 and UpperBound==6, this means points 4 to 6 in the array of plot point locations (!)
		/// will be a contiguous range of plot points, and for this, they can be connected by a line etc.
		/// If OffsetToOriginal is 5, this means the point at index 4 in the plot point location array was created
		/// from row index 4+5=9, i.e. from row index 9 in the DataColumn.</example>
		private int _offsetToOriginal;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.PlotRange", 0)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PlotRange), 1)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				PlotRange s = (PlotRange)obj;
				info.AddValue("LowerBound", s._lowerBound);
				info.AddValue("UpperBound", s._upperBound);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				PlotRange s = null != o ? (PlotRange)o : new PlotRange(0, 0);

				s._lowerBound = info.GetInt32("LowerBound");
				s._upperBound = info.GetInt32("UpperBound");

				return s;
			}
		}

		#endregion Serialization

		public PlotRange(int lower, int upper)
		{
			_lowerBound = lower;
			_upperBound = upper;
			_offsetToOriginal = 0;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlotRange"/> class.
		/// </summary>
		/// <param name="lower">The lower index of the range. This index designates a location in the plot point array. It is not a row index of the original data!</param>
		/// <param name="upper">The upper index (one more than the last used) of the plot range.  This index designates a location in the plot point array. It is not a row index of the original data!</param>
		/// <param name="offset">The offset to the data row index. Example: if lower is 10, and offset is 5, then the plot point at lower was created from the original row 15.</param>
		public PlotRange(int lower, int upper, int offset)
		{
			_lowerBound = lower;
			_upperBound = upper;
			_offsetToOriginal = offset;
		}

		public PlotRange(PlotRange a)
		{
			_lowerBound = a._lowerBound;
			_upperBound = a._upperBound;
			_offsetToOriginal = a._offsetToOriginal;
		}

		/// <summary>
		/// Number of points in this plot range.
		/// </summary>
		public int Length
		{
			get { return _upperBound - _lowerBound; }
		}

		/// <summary>
		/// First index of a contiguous plot range in the plot point array (i.e. in the array of processed plot point data, <b>not</b> in the original data column).
		/// To calculate from which row index in the original data column this comes from, add to this value <see cref="OffsetToOriginal"/>.
		/// </summary>
		public int LowerBound
		{
			get { return _lowerBound; }
			set { _lowerBound = value; }
		}

		/// <summary>
		/// Last index + 1 of a contiguous plot range in the plot point array (i.e. in the array of processed plot point data, <b>not</b> in the original data column).
		/// To calculate from which row index in the original data column this comes from, add to this value <see cref="OffsetToOriginal"/>.
		/// </summary>
		public int UpperBound
		{
			get { return _upperBound; }
			set { _upperBound = value; }
		}

		public IPlotRange WithUpperBoundShortenedBy(int count)
		{
			if (!(count >= 0))
				throw new ArgumentOutOfRangeException(nameof(count), "must be >=0");

			return new PlotRange(_lowerBound, Math.Max(0, _upperBound - count), _offsetToOriginal);
		}

		/// <summary>
		/// This gives the offset of an index in the plot point array to the original data row index.
		/// </summary>
		/// <example>If the LowerBound=4 and UpperBound==6, this means points 4 to 6 in the array of plot point locations (!)
		/// will be a contiguous range of plot points, and for this, they are connected by a line etc.
		/// If OffsetToOriginal is 5, this means the point at index 4 in the plot point location array was created
		/// from row index 4+5=9, i.e. from row index 9 in the DataColumn.</example>
		public int OffsetToOriginal
		{
			get { return _offsetToOriginal; }
			set { _offsetToOriginal = value; }
		}

		/// <summary>
		/// Row index of the first point of this plot range in the original data column.
		/// </summary>
		public int OriginalFirstPoint
		{
			get { return _lowerBound + _offsetToOriginal; }
		}

		/// <summary>
		/// Row index of the last point of this plot range in the original data column.
		/// </summary>
		public int OriginalLastPoint
		{
			get { return _upperBound + _offsetToOriginal - 1; }
		}

		/// <summary>
		/// Enumerates through the original row indices in this plot range.
		/// </summary>
		/// <returns>A enumerable that enumerates through the orginal row indices.</returns>
		public IEnumerable<int> OriginalRowIndices()
		{
			for (int i = _lowerBound; i < _upperBound; ++i)
			{
				yield return i + _offsetToOriginal;
			}
		}
	}

	/// <summary>
	/// Holds a list of plot ranges. The list is not sorted automatically, but is assumed to be sorted.
	/// </summary>
	[Serializable]
	public class PlotRangeList : IEnumerable<PlotRange>
	{
		private List<PlotRange> InnerList = new List<PlotRange>();

		/// <summary>
		/// Getter to a plot range at index i.
		/// </summary>
		public PlotRange this[int i]
		{
			get { return InnerList[i]; }
		}

		public int Count { get { return InnerList.Count; } }

		/// <summary>
		/// Adds a plot range. This plot range should be above the previous added plot range.
		/// </summary>
		/// <param name="a"></param>
		public void Add(PlotRange a)
		{
			// test the argument
			if (a.UpperBound <= a.LowerBound)
				throw new ArgumentException("UpperBound is <= LowerBound");
			if (Count == 0 && a.LowerBound != 0)
				throw new ArgumentException("First item must have a LowerBound of 0");
			if (Count != 0 && a.LowerBound != this[Count - 1].UpperBound)
				throw new ArgumentException("This item must have a LowerBound equal to the UpperBound of the previous item");

			InnerList.Add(a);
		}

		/// <summary>
		/// This will get the row index into the data row belonging to a given plot index.
		/// </summary>
		/// <param name="idx">Index of point in the plot array.</param>
		/// <returns>Index into the original data.</returns>
		/// <remarks>Returns -1 if the point is not found.</remarks>
		public int GetRowIndexForPlotIndex(int idx)
		{
			for (int i = 0; i < Count; i++)
			{
				if (this[i].LowerBound <= idx && idx < this[i].UpperBound)
					return idx + this[i].OffsetToOriginal;
			}
			return -1;
		}

		/// <summary>
		/// Returns the total number of plot points.
		/// </summary>
		public int PlotPointCount
		{
			get
			{
				return Count == 0 ? 0 : InnerList[InnerList.Count - 1].UpperBound;
			}
		}

		/// <summary>
		/// By enumerating through this, you will get the original row indices (indices into the DataColumns).
		/// The number of items returned is equal to <see cref="PlotPointCount" />.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<int> OriginalRowIndices()
		{
			for (int i = 0; i < InnerList.Count; i++)
			{
				PlotRange r = InnerList[i];
				for (int j = r.LowerBound; j < r.UpperBound; j++)
					yield return j + r.OffsetToOriginal;
			}
		}

		/// <summary>Gets a dictionary that associates the original row indices to the current plot indices. Note that this dictionary reflectes the actual state of the PlotRangeList
		/// in the moment of the function call; subsequent changes in the list are <b>not</b> reflected in the dictionary.</summary>
		/// <returns>Dictionary that associates the original row indices to the current plot indices.</returns>
		public Dictionary<int, int> GetDictionaryOfOriginalRowIndicesToPlotIndices()
		{
			Dictionary<int, int> result = new Dictionary<int, int>();
			int plotIndex = 0;
			foreach (int originalIndex in OriginalRowIndices())
			{
				result.Add(originalIndex, plotIndex);
				++plotIndex;
			}
			return result;
		}

		#region IEnumerable<PlotRange> Members

		public IEnumerator<PlotRange> GetEnumerator()
		{
			return InnerList.GetEnumerator();
		}

		#endregion IEnumerable<PlotRange> Members

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return InnerList.GetEnumerator();
		}

		#endregion IEnumerable Members
	}
}