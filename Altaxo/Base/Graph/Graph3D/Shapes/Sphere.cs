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
	public class Sphere : GraphicBase
	{
		private IMaterial _material = Materials.GetSolidMaterial(Drawing.NamedColors.LightGray);

		public Sphere()
			: base(new ItemLocationDirect())
		{
			this.Size = new Geometry.VectorD3D(100, 100, 100);
		}

		public Sphere(Sphere from)
			: base(from)
		{
		}

		public override bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;
			if (!base.CopyFrom(obj))
				return false;

			var from = obj as Sphere;
			if (null != from)
			{
				_material = from._material;

				EhSelfChanged(EventArgs.Empty);
				return true;
			}

			return false;
		}

		public override object Clone()
		{
			return new Sphere(this);
		}

		public override IHitTestObject HitTest(HitTestPointData parentHitData)
		{
			var localHitData = parentHitData.NewFromAdditionalTransformation(this._transformation);

			double z;
			if (localHitData.IsHit(Bounds, out z))
			{
				var result = new HitTestObject(new RectangularObjectOutline(this.Bounds, localHitData.Transformation), this);
				result.DoubleClick = null;
				return result;
			}
			else
			{
				return null;
			}
		}

		public override void Paint(IGraphicContext3D g, IPaintContext context)
		{
			var buffers = g.GetPositionNormalIndexedTriangleBuffer(_material);

			if (null != buffers.PositionNormalIndexedTriangleBuffer)
			{
				var buffer = buffers.PositionNormalIndexedTriangleBuffer;

				var offs = buffer.VertexCount;

				var sphere = new SolidIcoSphere(3);

				double sx = this.SizeX / 2;
				double sy = this.SizeY / 2;
				double sz = this.SizeZ / 2;

				double invsx = 1 / sx;
				double invsy = 1 / sy;
				double invsz = 1 / sz;

				var normalTransform = _transformation.GetTransposedInverse();

				foreach (var entry in sphere.VerticesAndNormalsForSphere)
				{
					var pt = entry.Item1;
					pt = new PointD3D(pt.X * sz, pt.Y * sy, pt.Z * sz);
					pt = _transformation.Transform(pt);
					var nm = entry.Item2;
					nm = new VectorD3D(nm.X * invsx, nm.Y * invsy, nm.Z * invsz);
					nm = normalTransform.Transform(nm);
					nm.Normalize();
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