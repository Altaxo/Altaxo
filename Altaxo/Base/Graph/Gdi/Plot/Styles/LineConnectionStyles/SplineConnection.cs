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
	public class SplineConnection : LineConnectionStyleBase
	{
		public static SplineConnection Instance { get; private set; } = new SplineConnection();

		#region Serialization

		/// <summary>
		/// 2016-05-09 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SplineConnection), 0)]
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
			PointF[] linePoints = pdata.PlotPointsInAbsoluteLayerCoordinates;
			PointF[] linepts = new PointF[range.Length];
			Array.Copy(linePoints, range.LowerBound, linepts, 0, range.Length); // Extract
			int lastIdx = range.Length - 1;
			GraphicsPath gp = new GraphicsPath();
			var layerSize = layer.Size;

			if (linePlotStyle.FillArea)
			{
				FillOneRange(gp, pdata, range, layer, linePlotStyle.FillDirection, linepts, linePlotStyle);
				g.FillPath(linePlotStyle.FillBrush, gp);
				gp.Reset();
			}

			// unfortuately, there is no easy way to support line/symbol gaps
			// thats why I ignore this value and draw a curve through the points
			g.DrawCurve(linePen, linepts);
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
			PointF[] linePoints,
			LinePlotStyle linePlotStyle
		)
		{
			Logical3D r0 = layer.GetLogical3D(pdata, range.OriginalFirstPoint);
			layer.CoordinateSystem.GetIsolineFromPlaneToPoint(gp, fillDirection, r0);
			gp.AddCurve(linePoints);
			Logical3D r1 = layer.GetLogical3D(pdata, range.OriginalLastPoint);
			layer.CoordinateSystem.GetIsolineFromPointToPlane(gp, r1, fillDirection);
			layer.CoordinateSystem.GetIsolineOnPlane(gp, fillDirection, r1, r0);

			gp.CloseFigure();
		}
	}
}