#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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
using System.Threading.Tasks;

namespace Altaxo.Graph3D
{
	using Primitives;

	public abstract class GraphicContext3DBase : IGraphicContext3D
	{
		public abstract IPositionColorIndexedTriangleBuffer GetPositionColorIndexedTriangleBuffer(int numberOfVertices);

		public abstract IPositionColorTriangleBuffer GetPositionColorTriangleBuffer(int numberOfVertices);

		public abstract object SaveGraphicsState();

		public abstract void RestoreGraphicsState(object graphicsState);

		public abstract void MultiplyTransform(MatrixD3D m);

		#region Primitives rendering

		public virtual void DrawLine(PenX3D pen, PointD3D p0, PointD3D p1)
		{
			var line = new Polyline3D(pen.CrossSection, new[] { p0, p1 });

			var buf = GetPositionColorIndexedTriangleBuffer(0);

			var offset = buf.VertexCount;

			var color = pen.Color.Color;

			line.AddWithNormals(
				(position, normal) => buf.AddTriangleVertex((float)position.X, (float)position.Y, (float)position.Z, 1, color.ScR, color.ScG, color.ScB, color.ScA),
				(i0, i1, i2) => buf.AddTriangleIndices(i0, i1, i2),
				ref offset);
		}

		public virtual void DrawLine(PenX3D pen, ISweepPath3D path)
		{
			var asStraightLine = path as Primitives.StraightLineSweepPath3D;

			if (null != asStraightLine)
			{
				DrawLine(pen, asStraightLine.GetPoint(0), asStraightLine.GetPoint(1));
				return;
			}

			var asSweepPath = path as Primitives.SweepPath3D;
			if (null != asSweepPath)
			{
				var line = new Polyline3D(pen.CrossSection, asSweepPath.Points);

				var buf = GetPositionColorIndexedTriangleBuffer(0);

				var offset = buf.VertexCount;

				var color = pen.Color.Color;

				line.AddWithNormals(
					(position, normal) => buf.AddTriangleVertex((float)position.X, (float)position.Y, (float)position.Z, 1, color.ScR, color.ScG, color.ScB, color.ScA),
					(i0, i1, i2) => buf.AddTriangleIndices(i0, i1, i2),
					ref offset);

				return;
			}

			throw new NotImplementedException();
		}

		public virtual VectorD3D MeasureString(string text, FontX3D font, PointD3D pointD3D, System.Drawing.StringFormat strfmt)
		{
			return Text3D.MeasureString(text, font, strfmt);
		}

		public virtual void DrawString(string text, FontX3D font, IMaterial3D brush, PointD3D point, System.Drawing.StringFormat strfmt)
		{
			var pctb = GetPositionColorIndexedTriangleBuffer(3);

			var txt = new Text3D(text, font);
			var offs = pctb.VertexCount;

			txt.AddWithNormals(
				(pt, n) =>
				{
					PointD3D color;
					if (n.Z == 1)
						color = new PointD3D(1, 0, 0);
					else if (n.Z == -1)
						color = new PointD3D(0, 1, 0);
					else
						color = new PointD3D(0, 0, 1);

					pctb.AddTriangleVertex((float)pt.X, (float)pt.Y, (float)pt.Z, 1, (float)color.X, (float)color.Y, (float)color.Z, 1);
				},
				(i0, i1, i2) => pctb.AddTriangleIndices(i0, i1, i2),
				ref offs);
		}

		#endregion Primitives rendering
	}
}