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
using System.Linq;
using System.Text;

namespace Altaxo.Data.Selections
{
	public class RangeOfRows : Main.SuspendableDocumentLeafNodeWithEventArgs, IRowSelection, ICloneable
	{
		public int Start { get; private set; }

		public int Count { get; private set; }

		/// <summary>
		/// Gets the end of the range (the index of the last data row that will be plotted).
		/// </summary>
		/// <value>
		/// The end of the range (inclusive).
		/// </value>
		public int EndInclusive
		{
			get
			{
				return Start + Math.Min(int.MaxValue - Start, Count - 1);
			}
		}

		#region Serialization

		/// <summary>
		/// 2016-09-25 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(RangeOfRows), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (RangeOfRows)obj;

				info.AddValue("Start", s.Start);
				info.AddValue("Count", s.Count);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var start = info.GetInt32("Start");
				var count = info.GetInt32("Count");
				return new RangeOfRows(start, count);
			}
		}

		#endregion Serialization

		public RangeOfRows()
		{
			Start = 0;
			Count = int.MaxValue;
		}

		public RangeOfRows(int start, int count)
		{
			if (!(start >= 0))
				throw new ArgumentOutOfRangeException(nameof(start));
			if (!(count >= 0))
				throw new ArgumentOutOfRangeException(nameof(count));

			Start = start;
			Count = count;
		}

		public object Clone()
		{
			return new RangeOfRows(this.Start, this.Count);
		}

		/// <inheritdoc/>
		public IEnumerable<int> GetSelectedRowIndicesFromTo(int startIndex, int maxIndex)
		{
			int end = Start + Math.Min(Count, maxIndex - Start); // mathematical trick to avoid overflow if Count is int.MaximumValue
			int start = Math.Max(Start, startIndex);

			for (int r = start; r < end; ++r)
				yield return r;
		}

		/// <summary>
		/// Gets a value indicating whether this instance is spanning all rows.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is spanning all rows; otherwise, <c>false</c>.
		/// </value>
		public bool IsSpanningAllRows { get { return 0 == Start && int.MaxValue == Count; } }

		public override int GetHashCode()
		{
			return 13 * Start.GetHashCode() + 31 * Count.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			var from = obj as RangeOfRows;
			if (null != from)
				return this.Start == from.Start && this.Count == from.Count;
			else
				return false;
		}
	}
}