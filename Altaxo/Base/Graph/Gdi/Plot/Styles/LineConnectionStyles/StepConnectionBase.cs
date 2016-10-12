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
	/// Connects by drawing a horizontal line to the x coordinate of the next point, and then a vertical line. Instances of this class have to be immutable.
	/// </summary>
	/// <seealso cref="Altaxo.Main.IImmutable" />
	public abstract class StepConnectionBase : LineConnectionStyleBase
	{
		/// <summary>
		/// Gets the sub points for a given range. For step connections, at least one points needs to be inserted inbetween two original points, for some step connection styles two points.
		/// </summary>
		/// <param name="pdata">The pdata.</param>
		/// <param name="range">The range.</param>
		/// <param name="layer">The layer.</param>
		/// <param name="connectCircular">if set to <c>true</c> [connect circular].</param>
		/// <param name="numberOfPointsPerOriginalPoint">The number of points per original point. For most step styles one additional point is inserted, thus the return value is 2. For some connection styles, two points are inserted inbetween two original points, thus the return value will be 3.</param>
		/// <param name="lastIndex">The last index.</param>
		/// <returns></returns>
		protected abstract PointF[] GetSubPoints(
		Processed2DPlotData pdata,
		PlotRange range,
		IPlotArea layer,
		bool connectCircular,
		out int numberOfPointsPerOriginalPoint,
		out int lastIndex);

		/// <summary>
		/// Template to make a line draw.
		/// </summary>
		/// <param name="g">Graphics context.</param>
		/// <param name="pdata">The plot data. Don't use the Range property of the pdata, since it is overriden by the next argument.</param>
		/// <param name="range">The plot range to use.</param>
		/// <param name="layer">Graphics layer.</param>
		/// <param name="linePen">The pen to draw the line.</param>
		/// <param name="symbolGap">The size of the symbol gap. Argument is the original index of the data. The return value is the absolute symbol gap at this index.
		/// This function is null if no symbol gap is required.</param>
		/// <param name="skipFrequency">Skip frequency. Normally 1, thus all gaps are taken into account. If 2, only every 2nd gap is taken into account, and so on.</param>
		/// <param name="connectCircular">If true, there is a line connecting the start and the end of the range.</param>
		/// <param name="linePlotStyle">The line plot style.</param>
		public override void Paint(
			Graphics g,
			Processed2DPlotData pdata,
			PlotRange range,
			IPlotArea layer,
			PenX linePen,
			Func<int, double> symbolGap,
			int skipFrequency,
			bool connectCircular,
			LinePlotStyle linePlotStyle)
		{
			if (range.Length < 2)
				return;

			int lastIdx;
			int numberOfPointsPerOriginalPoint;
			PointF[] linepts = GetSubPoints(pdata, range, layer, connectCircular, out numberOfPointsPerOriginalPoint, out lastIdx);
			PointF[] linePoints = pdata.PlotPointsInAbsoluteLayerCoordinates;

			GraphicsPath gp = new GraphicsPath();

			if (linePlotStyle.FillArea)
			{
				FillOneRange(gp, pdata, range, layer, linePlotStyle.FillDirection, linepts, linePlotStyle);
				g.FillPath(linePlotStyle.FillBrush, gp);
				gp.Reset();
			}

			if (null != symbolGap)
			{
				int end = range.UpperBound - 1;

				var subPointsLength = skipFrequency * numberOfPointsPerOriginalPoint + 1;
				var subPoints = new PointF[subPointsLength];
				for (int i = 0; i < range.Length; i += skipFrequency)
				{
					int originalIndexStart = range.OffsetToOriginal + i;
					int originalIndexEnd = range.OffsetToOriginal + ((i + skipFrequency) < range.Length ? (i + skipFrequency) : (connectCircular ? 0 : range.Length - 1));

					int copyLength = Math.Min(subPointsLength, linepts.Length - numberOfPointsPerOriginalPoint * i);
					if (subPoints.Length != copyLength)
						subPoints = new PointF[copyLength];

					Array.Copy(linepts, numberOfPointsPerOriginalPoint * i, subPoints, 0, copyLength);

					double gapAtStart = symbolGap(originalIndexStart);
					double gapAtEnd = symbolGap(originalIndexEnd);

					var polyline = subPoints.ShortenedBy(RADouble.NewAbs(gapAtStart / 2), RADouble.NewAbs(gapAtEnd / 2));

					if (null != polyline)
						g.DrawLines(linePen, polyline);
				}
			}
			else
			{
				if (connectCircular)
					g.DrawPolygon(linePen, linepts);
				else
					g.DrawLines(linePen, linepts);
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
		/// <param name="linePlotStyle">The line plot style.</param>
		public override void FillOneRange(
		GraphicsPath gp,
			Processed2DPlotData pdata,
			PlotRange range,
			IPlotArea layer,
			CSPlaneID fillDirection,
			bool connectCircular,
			LinePlotStyle linePlotStyle
		)
		{
			if (range.Length < 2)
				return;

			int lastIdx;
			int numberOfPointsPerOriginalPoint;
			PointF[] linepts = GetSubPoints(pdata, range, layer, connectCircular, out numberOfPointsPerOriginalPoint, out lastIdx);
			FillOneRange(gp, pdata, range, layer, fillDirection, linepts, linePlotStyle);
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
		/// <param name="linePlotStyle">The line plot style.</param>
		public virtual void FillOneRange(
		GraphicsPath gp,
			Processed2DPlotData pdata,
			PlotRange range,
			IPlotArea layer,
			CSPlaneID fillDirection,
			PointF[] linePoints,
			LinePlotStyle linePlotStyle
		)
		{
			Logical3D r0 = layer.GetLogical3D(pdata, range.OriginalFirstPoint);
			layer.CoordinateSystem.GetIsolineFromPlaneToPoint(gp, fillDirection, r0);
			gp.AddLines(linePoints);
			Logical3D r1 = layer.GetLogical3D(pdata, range.OriginalLastPoint);
			layer.CoordinateSystem.GetIsolineFromPointToPlane(gp, r1, fillDirection);
			layer.CoordinateSystem.GetIsolineOnPlane(gp, fillDirection, r1, r0);
			gp.CloseFigure();
		}
	}
}