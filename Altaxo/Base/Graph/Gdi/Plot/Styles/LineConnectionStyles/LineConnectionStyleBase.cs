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

using Altaxo.Drawing;
using Altaxo.Geometry;
using Altaxo.Graph.Gdi.Plot.Data;
using Altaxo.Graph.Plot.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Gdi.Plot.Styles.LineConnectionStyles
{
	/// <summary>
	/// Represents a symbol shape for a 3D scatter plot. Instances of this class have to be immutable.
	/// This base class implements Equals and GetHashCode.
	/// </summary>
	/// <seealso cref="Altaxo.Main.IImmutable" />
	public abstract class LineConnectionStyleBase : ILineConnectionStyle
	{
		/// <summary>
		/// Template to make a line draw.
		/// </summary>
		/// <param name="g">Graphics context.</param>
		/// <param name="allLinePoints">The plot data. Don't use the Range property of the pdata, since it is overriden by the next argument.</param>
		/// <param name="range">The plot range to use.</param>
		/// <param name="layer">Graphics layer.</param>
		/// <param name="pen">The pen to draw the line.</param>
		/// <param name="symbolGap">The size of the symbol gap. Argument is the original index of the data. The return value is the absolute symbol gap at this index.
		/// This function is null if no symbol gap is required.</param>
		/// <param name="skipFrequency">Skip frequency. Normally 1, thus all gaps are taken into account. If 2, only every 2nd gap is taken into account, and so on.</param>
		/// <param name="connectCircular">If true, the line is connected circular, and the area is the polygon inside of that circular connection.</param>
		/// <param name="linePlotStyle">The line plot style.</param>
		public abstract void PaintOneRange(
			Graphics g,
			PointF[] allLinePoints,
			IPlotRange range,
			IPlotArea layer,
			PenX pen,
			Func<int, double> symbolGap,
			int skipFrequency,
			bool connectCircular,
			LinePlotStyle linePlotStyle
);

		/// <summary>
		/// Template to get a fill path.
		/// </summary>
		/// <param name="gp">Graphics path to fill with data.</param>
		/// <param name="pdata">The plot data. Do not use the plot point positions from here, since they are not shifted when shifting group styles are present.</param>
		/// <param name="range">The plot range to use.</param>
		/// <param name="layer">Graphics layer.</param>
		/// <param name="fillDirection">Designates a bound to fill to.</param>
		/// <param name="ignoreMissingDataPoints">If true, missing data points are ignored.</param>
		/// <param name="connectCircular">If true, the line is connected circular, and the area is the polygon inside of that circular connection.</param>
		/// <param name="allLinePointsShiftedAlready">The plot positions, already shifted when a logical shift needed to be applied. Don't use the Range property of the pdata, since it is overriden by the next argument.</param>
		/// <param name="logicalShiftX">The logical shift in x-direction.</param>
		/// <param name="logicalShiftY">The logical shift in x-direction.</param>
		public abstract void FillOneRange(
			GraphicsPath gp,
			Processed2DPlotData pdata,
			IPlotRange range,
			IPlotArea layer,
			CSPlaneID fillDirection,
			bool ignoreMissingDataPoints,
			bool connectCircular,
			PointF[] allLinePointsShiftedAlready,
			double logicalShiftX,
			double logicalShiftY
			);

		public override int GetHashCode()
		{
			return this.GetType().GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return this.GetType() == obj?.GetType();
		}

		#region Sub range enumeration

		protected struct SegmentRange
		{
			/// <summary>
			/// If true, the subrange should be treated as a full range closed curve, i.e. a connection of all points of the range and additionally a connection of the last point back to
			/// the first point. In this case, <see cref="IndexAtSubRangeEnd"/> is set to <see cref="IndexAtSubRangeStart"/>.
			/// </summary>
			public bool IsFullRangeClosedCurve;

			/// <summary>
			/// Zero based index of the start of the sub range.
			/// </summary>
			public int IndexAtSubRangeStart;

			/// <summary>
			/// Zero based index of the last valid point of the sub range (except in the case of <see cref="IsFullRangeClosedCurve"/>==true).
			/// </summary>
			public int IndexAtSubRangeEnd;

			/// <summary>
			/// The gap at the start of the sub range.
			/// </summary>
			public double GapAtSubRangeStart;

			/// <summary>
			/// The gap at the end of the sub range.
			/// </summary>
			public double GapAtSubRangeEnd;

			public int Length { get { return IndexAtSubRangeEnd - IndexAtSubRangeStart; } }
		}

		protected static IEnumerable<SegmentRange> GetSegmentRanges(
			IPlotRange range,
			Func<int, double> symbolGap,
			int skipFrequency, bool connectCircular)
		{
			int lengthNonCircular = range.Length;
			int lengthCircularMinus1 = range.Length - 1 + (connectCircular ? 1 : 0);
			// SubRange is a number of consecutive line segments, with joints where the symbol gap is zero
			double gapAtSubRangeStart = symbolGap(range.GetOriginalRowIndexFromPlotPointIndex(range.LowerBound)); // start index of the subrange
			double gapAtSubRangeEnd = 0;
			int indexAtSubRangeEnd;
			for (int indexAtSubRangeStart = 0; indexAtSubRangeStart < lengthCircularMinus1; indexAtSubRangeStart = indexAtSubRangeEnd, gapAtSubRangeStart = gapAtSubRangeEnd)
			{
				// search for the last index of this subrange, i.e. for a point where the gap is non-zero
				for (indexAtSubRangeEnd = indexAtSubRangeStart + 1, gapAtSubRangeEnd = 0; indexAtSubRangeEnd < lengthNonCircular; ++indexAtSubRangeEnd)
				{
					if (0 == indexAtSubRangeEnd % skipFrequency) // calculate gap only if in skip period
						gapAtSubRangeEnd = symbolGap(range.GetOriginalRowIndexFromPlotPointIndex(range.LowerBound + indexAtSubRangeEnd));
					if (0 != gapAtSubRangeEnd)
						break;
				}

				// if this is circular connected and is the last point, then use the gap of the very first point
				if (connectCircular && indexAtSubRangeEnd == lengthNonCircular)
					gapAtSubRangeEnd = symbolGap(range.GetOriginalRowIndexFromPlotPointIndex(range.LowerBound));

				// test if this is a closed polygon without any gaps -> draw a closed polygon and return
				if (connectCircular && 0 == gapAtSubRangeStart && indexAtSubRangeEnd == (lengthCircularMinus1 + 1))
				{
					// use the whole circular array to draw a closed polygon without any gaps
					yield return new SegmentRange()
					{
						IsFullRangeClosedCurve = true,
						IndexAtSubRangeStart = indexAtSubRangeStart,
						IndexAtSubRangeEnd = indexAtSubRangeStart,
						GapAtSubRangeStart = 0,
						GapAtSubRangeEnd = 0
					};
					break;
				}

				// adjust the end index to be valid
				if (!(indexAtSubRangeEnd <= lengthCircularMinus1))
					indexAtSubRangeEnd = lengthCircularMinus1;

				yield return new SegmentRange()
				{
					IsFullRangeClosedCurve = false,
					IndexAtSubRangeStart = indexAtSubRangeStart,
					IndexAtSubRangeEnd = indexAtSubRangeEnd,
					GapAtSubRangeStart = gapAtSubRangeStart,
					GapAtSubRangeEnd = gapAtSubRangeEnd
				};
			}
		}

		#endregion Sub range enumeration
	}
}