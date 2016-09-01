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

using Altaxo.Drawing.D3D;
using Altaxo.Geometry;
using Altaxo.Graph.Graph3D.GraphicsContext;
using Altaxo.Serialization;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Altaxo.Graph.Graph3D.Plot.Styles.ScatterSymbols
{
	/// <summary>
	/// Represents the null symbol in a scatter plot, i.e. this symbol is not visible.
	/// </summary>
	/// <seealso cref="Altaxo.Graph.Graph3D.Plot.Styles.IScatterSymbol" />
	public sealed class Cube : ScatterSymbolShapeBase
	{
		public static Cube Instance { get; private set; } = new Cube();

		public static readonly double Sqrt1By3 = Math.Sqrt(1 / 3.0);

		#region Serialization

		/// <summary>
		/// 2016-05-09 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Cube), 0)]
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

		public override void Paint(IGraphicsContext3D g, IMaterial material, PointD3D centerLocation, double symbolSize)
		{
			// Note: the symbolSize provided in the argument is the diameter of an imaginary sphere in which the cube has to fit in
			// thus the cube's diagonal should be equal to the sphere's diameter

			var size = symbolSize * Sqrt1By3; // size of the cube
			var sizeBy2 = size * 0.5;
			var buffers = g.GetPositionNormalIndexedTriangleBuffer(material);
			if (null != buffers.PositionNormalIndexedTriangleBuffer)
			{
				var buf = buffers.PositionNormalIndexedTriangleBuffer;
				var voffs = buffers.PositionNormalIndexedTriangleBuffer.VertexCount;
				SolidCube.Add(
					centerLocation.X - sizeBy2, centerLocation.Y - sizeBy2, centerLocation.Z - sizeBy2,
					size, size, size,
					(point, normal) => buf.AddTriangleVertex(point.X, point.Y, point.Z, normal.X, normal.Y, normal.Z),
					(i1, i2, i3) => buf.AddTriangleIndices(i1 + voffs, i2 + voffs, i3 + voffs),
					ref voffs);
			}
		}
	}
}