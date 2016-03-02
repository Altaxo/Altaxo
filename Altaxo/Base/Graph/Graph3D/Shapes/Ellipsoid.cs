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
using Altaxo.Graph.Graph3D.GraphicsContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Graph3D.Shapes
{
	using Geometry;

	/// <summary>
	///
	/// </summary>
	public class Ellipsoid : SolidBodyShapeBase
	{
		#region Serialization

		/// <summary>
		/// Deserialization constructor.
		/// </summary>
		/// <param name="info">The information.</param>
		protected Ellipsoid(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
		: base(info)
		{
		}

		/// <summary>
		/// 2016-03-01 initial version
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Ellipsoid), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (Ellipsoid)obj;
				info.AddBaseValueEmbedded(s, typeof(Ellipsoid).BaseType);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (Ellipsoid)o ?? new Ellipsoid(info);

				info.GetBaseValueEmbedded(s, typeof(Ellipsoid).BaseType, parent);

				return s;
			}
		}

		#endregion Serialization

		public Ellipsoid()
		{
			this.Size = new Geometry.VectorD3D(100, 100, 100);
		}

		public Ellipsoid(Ellipsoid from)
			: base(from)
		{
		}

		public override object Clone()
		{
			return new Ellipsoid(this);
		}

		public override void Paint(IGraphicContext3D g, IPaintContext context)
		{
			var buffers = g.GetPositionNormalIndexedTriangleBuffer(_material);

			if (null != buffers.PositionNormalIndexedTriangleBuffer)
			{
				var buffer = buffers.PositionNormalIndexedTriangleBuffer;

				var offs = buffer.VertexCount;

				var sphere = new SolidIcoSphere(3); // gives a sphere with radius = 1

				var bounds = this.Bounds;

				double sx = this.Bounds.SizeX / 2;
				double sy = this.Bounds.SizeY / 2;
				double sz = this.Bounds.SizeZ / 2;

				var dx = this.Bounds.X + sx;
				var dy = this.Bounds.Y + sy;
				var dz = this.Bounds.Z + sz;

				var transformation = Matrix4x3.NewScalingShearingRotationDegreesTranslation(sx, sy, sz, 0, 0, 0, 0, 0, 0, dx, dy, dz);
				transformation.AppendTransform(_transformation);

				var normalTransform = transformation.GetTransposedInverseMatrix3x3();

				foreach (var entry in sphere.VerticesAndNormalsForSphere)
				{
					var pt = transformation.Transform(entry.Item1);
					var nm = normalTransform.Transform(entry.Item2).Normalized;
					buffer.AddTriangleVertex(pt.X, pt.Y, pt.Z, nm.X, nm.Y, nm.Z);
				}
				foreach (var idx in sphere.TriangleIndicesForSphere)
				{
					buffer.AddTriangleIndices(idx.Item1 + offs, idx.Item2 + offs, idx.Item3 + offs);
				}
			}
		}
	}
}