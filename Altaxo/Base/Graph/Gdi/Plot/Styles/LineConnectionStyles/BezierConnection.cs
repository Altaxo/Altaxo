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
	/// Connects 2 consecutive points by a straight line, then the next two points are not connected, and so on. Instances of this class have to be immutable.
	/// </summary>
	/// <seealso cref="Altaxo.Main.IImmutable" />
	public class BezierConnection : LineConnectionStyleBase
	{
		public static BezierConnection Instance { get; private set; } = new BezierConnection();

		#region Serialization

		/// <summary>
		/// 2016-05-09 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(BezierConnection), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				return Instance;
			}
		}

		#endregion Serialization

		/// <summary>
		/// Template to make a line draw.
		/// </summary>
		/// <param name="g">Graphics context.</param>
		/// <param name="allLinePoints">The plot data. Don't use the Range property of the pdata, since it is overriden by the next argument.</param>
		/// <param name="range">The plot range to use.</param>
		/// <param name="layer">Graphics layer.</param>
		/// <param name="linePen">The pen to draw the line.</param>
		/// <param name="symbolGap">The size of the symbol gap. Argument is the original index of the data. The return value is the absolute symbol gap at this index.
		/// This function is null if no symbol gap is required.</param>
		/// <param name="skipFrequency">Skip frequency. Normally 1, thus all gaps are taken into account. If 2, only every 2nd gap is taken into account, and so on.</param>
		/// <param name="connectCircular">If true, there is a line connecting the start and the end of the range.</param>
		/// <param name="linePlotStyle">The line plot style.</param>
		public override void PaintOneRange(
			Graphics g,
			PointF[] allLinePoints,
			IPlotRange range,
			IPlotArea layer,
			PenX linePen,
			Func<int, double> symbolGap,
			int skipFrequency,
			bool connectCircular,
			LinePlotStyle linePlotStyle)
		{
			// Bezier is only supported with point numbers n=4+3*k
			// so trim the range appropriately
			if (range.Length < 4)
				return; // then too less points are in this range

			if (connectCircular)
			{
				var circularLinePointsLengthM1 = 2 + TrimToValidBezierLength(range.Length);
				var circularLinePoints = new PointF[circularLinePointsLengthM1 + 1];
				Array.Copy(allLinePoints, range.LowerBound, circularLinePoints, 0, range.Length); // Extract
				circularLinePoints[circularLinePointsLengthM1] = circularLinePoints[0];

				// amend missing control points
				if (circularLinePointsLengthM1 - range.Length >= 1)
					circularLinePoints[circularLinePointsLengthM1 - 1] = GdiExtensionMethods.Interpolate(circularLinePoints[circularLinePointsLengthM1 - 3], circularLinePoints[circularLinePointsLengthM1], 0.5); // Last Control point should be halfway between
				if (circularLinePointsLengthM1 - range.Length >= 2)
					circularLinePoints[circularLinePointsLengthM1 - 2] = GdiExtensionMethods.Interpolate(circularLinePoints[circularLinePointsLengthM1 - 3], circularLinePoints[circularLinePointsLengthM1], 0.5); // Middle Control point should be halfway between previous fixed point and last(=first) fixed point

				if (null != symbolGap) // circular with symbol gap
				{
					var realSkipFrequency = skipFrequency % 3 == 0 ? skipFrequency : skipFrequency * 3; // least common multiple of skipFrequency and 3
					for (int i = 0; i < range.Length; i += realSkipFrequency)
					{
						var skipLinePointsLength = Math.Min(realSkipFrequency + 1, TrimToValidBezierLength(circularLinePoints.Length - i));
						if (skipLinePointsLength >= 4)
						{
							var skipLinePoints = new PointF[skipLinePointsLength];
							Array.Copy(circularLinePoints, i, skipLinePoints, 0, skipLinePointsLength); // Extract

							var gapAtStart = symbolGap(range.GetOriginalRowIndexFromPlotPointIndex(range.LowerBound + i));
							double gapAtEnd;
							if (connectCircular && realSkipFrequency >= (range.Length - 1 - i))
								gapAtEnd = symbolGap(range.OriginalFirstPoint);
							else if (realSkipFrequency <= (range.Length - 1 - i))
								gapAtEnd = symbolGap(range.GetOriginalRowIndexFromPlotPointIndex(range.LowerBound + i + realSkipFrequency));
							else
								gapAtEnd = 0;

							if (gapAtStart != 0 || gapAtEnd != 0)
							{
								skipLinePoints = GdiExtensionMethods.ShortenBezierCurve(skipLinePoints, gapAtStart / 2, gapAtEnd / 2);
							}

							if (null != skipLinePoints)
							{
								g.DrawBeziers(linePen, skipLinePoints);
							}
						}
					}
				}
				else // circular without symbol gap
				{
					g.DrawBeziers(linePen, circularLinePoints);
				}
			}
			else // not circular
			{
				if (null != symbolGap) // not circular with symbol gap
				{
					var realSkipFrequency = skipFrequency % 3 == 0 ? skipFrequency : skipFrequency * 3; // least common multiple of skipFrequency and 3
					for (int i = 0; i < range.Length; i += realSkipFrequency)
					{
						var skipLinePointsLength = Math.Min(realSkipFrequency + 1, TrimToValidBezierLength(range.Length - i));
						if (skipLinePointsLength >= 4)
						{
							var skipLinePoints = new PointF[skipLinePointsLength];
							Array.Copy(allLinePoints, range.LowerBound + i, skipLinePoints, 0, skipLinePointsLength); // Extract

							var gapAtStart = symbolGap(range.GetOriginalRowIndexFromPlotPointIndex(range.LowerBound + i));
							var gapAtEnd = symbolGap(range.GetOriginalRowIndexFromPlotPointIndex(range.LowerBound + i + skipLinePointsLength - 1));

							if (gapAtStart != 0 || gapAtEnd != 0)
							{
								skipLinePoints = GdiExtensionMethods.ShortenBezierCurve(skipLinePoints, gapAtStart / 2, gapAtEnd / 2);
							}

							if (null != skipLinePoints)
							{
								g.DrawBeziers(linePen, skipLinePoints);
							}
						}
					}
				}
				else // not circular without symbol gap
				{
					var trimmedLength = TrimToValidBezierLength(range.Length);
					var subLinePoints = new PointF[trimmedLength];
					Array.Copy(allLinePoints, range.LowerBound, subLinePoints, 0, trimmedLength); // Extract
					g.DrawBeziers(linePen, subLinePoints);
				}
			}
		}

		static int TrimToValidBezierLength(int x)
		{
			return (3 * ((x + 2) / 3) - 2);
		}




		/// <inheritdoc/>
		public override void FillOneRange(
		GraphicsPath gp,
			Processed2DPlotData pdata,
			IPlotRange range,
			IPlotArea layer,
			CSPlaneID fillDirection,
			bool ignoreMissingDataPoints,
			bool connectCircular,
			PointF[] allLinePoints,
			double logicalShiftX,
			double logicalShiftY
		)
		{
			if (range.Length < 4)
				return;


			if (connectCircular)
			{
				var circularLinePointsLengthM1 = 2 + TrimToValidBezierLength(range.Length);
				var circularLinePoints = new PointF[circularLinePointsLengthM1 + 1];
				Array.Copy(allLinePoints, range.LowerBound, circularLinePoints, 0, range.Length); // Extract
				circularLinePoints[circularLinePointsLengthM1] = circularLinePoints[0];

				// amend missing control points
				if (circularLinePointsLengthM1 - range.Length >= 1)
					circularLinePoints[circularLinePointsLengthM1 - 1] = GdiExtensionMethods.Interpolate(circularLinePoints[circularLinePointsLengthM1 - 3], circularLinePoints[circularLinePointsLengthM1], 0.5); // Last Control point should be halfway between
				if (circularLinePointsLengthM1 - range.Length >= 2)
					circularLinePoints[circularLinePointsLengthM1 - 2] = GdiExtensionMethods.Interpolate(circularLinePoints[circularLinePointsLengthM1 - 3], circularLinePoints[circularLinePointsLengthM1], 0.5); // Middle Control point should be halfway between previous fixed point and last(=first) fixed point

				FillOneRange_PreprocessedPoints(gp, pdata, range, layer, fillDirection, circularLinePoints, connectCircular, logicalShiftX, logicalShiftY);
			}
			else
			{
				var trimmedLinePointsLength = TrimToValidBezierLength(range.Length);
				var trimmedLinePoints = new PointF[trimmedLinePointsLength];
				Array.Copy(allLinePoints, range.LowerBound, trimmedLinePoints, 0, trimmedLinePointsLength); // Extract
				FillOneRange_PreprocessedPoints(gp, pdata, range, layer, fillDirection, trimmedLinePoints, connectCircular, logicalShiftX, logicalShiftY);
			}
		}

		/// <summary>
		/// Template to get a fill path.
		/// </summary>
		/// <param name="gp">Graphics path to fill with data.</param>
		/// <param name="pdata">The plot data. Don't use the Range property of the pdata, since it is overriden by the next argument.</param>
		/// <param name="range">The plot range to use.</param>
		/// <param name="layer">Graphics layer.</param>
		/// <param name="fillDirection">Designates a bound to fill to.</param>
		/// <param name="linePoints">The points that mark the line.</param>
		/// <param name="connectCircular">If true, a circular connection is drawn.</param>
		private void FillOneRange_PreprocessedPoints(
		GraphicsPath gp,
			Processed2DPlotData pdata,
			IPlotRange range,
			IPlotArea layer,
			CSPlaneID fillDirection,
			PointF[] linePoints,
			bool connectCircular,
			double logicalShiftX,
			double logicalShiftY
		)
		{
			if (connectCircular)
			{
				gp.AddBeziers(linePoints);
				gp.CloseFigure();

			}
			else
			{
				Logical3D r0 = layer.GetLogical3D(pdata, range.OriginalFirstPoint);
				r0.RX += logicalShiftX;
				r0.RY += logicalShiftY;
				layer.CoordinateSystem.GetIsolineFromPlaneToPoint(gp, fillDirection, r0);
				gp.AddBeziers(linePoints);

				Logical3D r1 = layer.GetLogical3D(pdata, range.GetOriginalRowIndexFromPlotPointIndex(range.LowerBound + linePoints.Length - 1));
				r1.RX += logicalShiftX;
				r1.RY += logicalShiftY;

				layer.CoordinateSystem.GetIsolineFromPointToPlane(gp, r1, fillDirection);
				layer.CoordinateSystem.GetIsolineOnPlane(gp, fillDirection, r1, r0);

				gp.CloseFigure();
			}

		}
	}
}