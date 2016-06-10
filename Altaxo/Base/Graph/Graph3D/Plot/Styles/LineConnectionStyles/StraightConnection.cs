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
using Altaxo.Drawing.D3D;
using Altaxo.Geometry;
using Altaxo.Graph.Graph3D.GraphicsContext;
using Altaxo.Graph.Graph3D.Plot.Data;
using Altaxo.Graph.Plot.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Graph3D.Plot.Styles.LineConnectionStyles
{
	/// <summary>
	/// Represents a symbol shape for a 3D scatter plot. Instances of this class have to be immutable.
	/// </summary>
	/// <seealso cref="Altaxo.Main.IImmutable" />
	public class StraightConnection : LineConnectionStyleBase
	{
		public static StraightConnection Instance { get; private set; } = new StraightConnection();

		#region Serialization

		/// <summary>
		/// 2016-05-09 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(StraightConnection), 0)]
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
		/// <param name="pen">The pen to draw the line.</param>
		/// <param name="symbolGap">The size of the symbol gap. This parameter is zero if no symbol gap is required.</param>
		/// <param name="connectCircular">If true, the end of the line is connected with the start of the line.</param>
		public override void Paint(
			IGraphicsContext3D g,
			Processed3DPlotData pdata,
			PlotRange range,
			IPlotArea layer,
			PenX3D pen,
			double symbolGap,
			bool connectCircular)
		{
			var linePoints = pdata.PlotPointsInAbsoluteLayerCoordinates;
			var linepts = new PointD3D[range.Length + (connectCircular ? 1 : 0)];
			Array.Copy(linePoints, range.LowerBound, linepts, 0, range.Length); // Extract
			if (connectCircular) linepts[linepts.Length - 1] = linepts[0];
			int lastIdx = range.Length - 1 + (connectCircular ? 1 : 0);
			var layerSize = layer.Size;

			// special efforts are necessary to realize a line/symbol gap
			// I decided to use a path for this
			// and hope that not so many segments are added to the path due
			// to the exclusion criteria that a line only appears between two symbols (rel<0.5)
			// if the symbols do not overlap. So for a big array of points it is very likely
			// that the symbols overlap and no line between the symbols needs to be plotted
			if (symbolGap > 0)
			{
				for (int i = 0; i < lastIdx; i++)
				{
					var diff = linepts[i + 1] - linepts[i];
					var rel = symbolGap / diff.Length;
					if (rel < 0.5) // a line only appears if the relative gap is smaller 1/2
					{
						var start = linepts[i] + rel * diff;
						var stop = linepts[i + 1] - rel * diff;

						g.DrawLine(pen, start, stop);
					}
				} // end for
			}
			else // no line symbol gap required, so we can use DrawLines to draw the lines
			{
				if (linepts.Length > 1) // we don't want to have a drawing exception if number of points is only one
				{
					g.DrawLine(pen, new SharpPolylineD3D(linepts));
				}
			}
		}
	}
}