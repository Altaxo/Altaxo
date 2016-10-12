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
	public class StepHorizontalConnection : StepConnectionBase
	{
		public static StepHorizontalConnection Instance { get; private set; } = new StepHorizontalConnection();

		#region Serialization

		/// <summary>
		/// 2016-05-09 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(StepHorizontalConnection), 0)]
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

		protected override PointF[] GetSubPoints(
		Processed2DPlotData pdata,
		PlotRange range,
		IPlotArea layer,
		bool connectCircular,
		out int numberOfPointsPerOriginalPoint,
		out int lastIndex)
		{
			numberOfPointsPerOriginalPoint = 2;

			PointF[] linePoints = pdata.PlotPointsInAbsoluteLayerCoordinates;
			PointF[] linepts = new PointF[range.Length * 2 - 1 + (connectCircular ? 2 : 0)];
			int end = range.UpperBound - 1;
			int i, j;
			for (i = 0, j = range.LowerBound; j < end; i += 2, j++)
			{
				linepts[i] = linePoints[j];
				linepts[i + 1].X = linePoints[j + 1].X;
				linepts[i + 1].Y = linePoints[j].Y;
			}
			linepts[i] = linePoints[j];
			lastIndex = i;

			if (connectCircular)
			{
				linepts[i + 1] = new PointF(linePoints[0].X, linePoints[j].Y);
				linepts[i + 2] = linePoints[0];
				lastIndex = i + 2;
			}
			return linepts;
		}
	}
}