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

using Altaxo.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Drawing.D3D
{
	public class LineCapContour
	{
		private PointD3D[] _vertices;
		private bool[] _isVertexSharp;
		private VectorD3D[] _normals;

		public PointD3D[] Vertices { get { return _vertices; } }
		public bool[] IsVertexSharp { get { return _isVertexSharp; } }
		public VectorD3D[] Normals { get { return _normals; } }

		public int NumberOfVertices { get { return _vertices.Length; } }
		public int NumberOfNormals { get { return _normals.Length; } }

		private void NormalizeNormals()
		{
			for (int i = 0; i < _normals.Length; ++i)
				_normals[i].Normalize();
		}

		public static LineCapContour Ball
		{
			get
			{
				const int segments = 4;
				var r = new LineCapContour();
				r._vertices = new PointD3D[2 + segments];
				r._normals = new VectorD3D[2 + segments];
				r._isVertexSharp = new bool[2 + segments];

				r._vertices[0] = new PointD3D(0, 0, 0);
				r._isVertexSharp[0] = false;
				r._normals[0] = new VectorD3D(0, -1, 0);

				for (int i = 0; i < segments; ++i)
				{
					double phi = i * (0.5 * Math.PI) / segments;
					r._vertices[i + 1] = new PointD3D(Math.Cos(phi), Math.Sin(phi), 0);
					r._normals[i + 1] = new VectorD3D(Math.Cos(phi), Math.Sin(phi), 0);
					r._isVertexSharp[i + 1] = false;
				}

				r._vertices[segments + 1] = new PointD3D(0, 1, 0);
				r._normals[segments + 1] = new VectorD3D(0, 1, 0);
				r._isVertexSharp[segments + 1] = false;

				return r;
			}
		}

		public static LineCapContour Arrow
		{
			get
			{
				LineCapContour r = new LineCapContour();
				r._vertices = new PointD3D[3];
				r._vertices[0] = new PointD3D(0, 0, 0);
				r._vertices[1] = new PointD3D(1, 0, 0);
				r._vertices[2] = new PointD3D(0, 1, 0);

				r._isVertexSharp = new bool[3];
				r._isVertexSharp[0] = false;
				r._isVertexSharp[1] = true;
				r._isVertexSharp[2] = false;

				r._normals = new VectorD3D[4];
				r._normals[0] = new VectorD3D(0, -1, 0);
				r._normals[1] = new VectorD3D(0, -1, 0);
				r._normals[2] = new VectorD3D(1, 1, 0);
				r._normals[3] = new VectorD3D(1, 1, 0);

				r.NormalizeNormals();

				return r;
			}
		}

		public static LineCapContour BigArrow
		{
			get
			{
				LineCapContour r = new LineCapContour();
				r._vertices = new PointD3D[3];
				r._vertices[0] = new PointD3D(0, 0, 0);
				r._vertices[1] = new PointD3D(2, 0, 0);
				r._vertices[2] = new PointD3D(0, 4, 0);

				r._isVertexSharp = new bool[3];
				r._isVertexSharp[0] = false;
				r._isVertexSharp[1] = true;
				r._isVertexSharp[2] = false;

				r._normals = new VectorD3D[4];
				r._normals[0] = new VectorD3D(0, -1, 0);
				r._normals[1] = new VectorD3D(0, -1, 0);
				r._normals[2] = new VectorD3D(4, 2, 0);
				r._normals[3] = new VectorD3D(4, 2, 0);

				r.NormalizeNormals();

				return r;
			}
		}

		public static LineCapContour Block
		{
			get
			{
				LineCapContour r = new LineCapContour();
				r._vertices = new PointD3D[4];
				r._vertices[0] = new PointD3D(0, 0, 0);
				r._vertices[1] = new PointD3D(1, 0, 0);
				r._vertices[2] = new PointD3D(1, 1, 0);
				r._vertices[3] = new PointD3D(0, 1, 0);

				r._isVertexSharp = new bool[4];
				r._isVertexSharp[0] = false;
				r._isVertexSharp[1] = true;
				r._isVertexSharp[2] = true;
				r._isVertexSharp[3] = false;

				r._normals = new VectorD3D[6];
				r._normals[0] = new VectorD3D(0, -1, 0);
				r._normals[1] = new VectorD3D(0, -1, 0);
				r._normals[2] = new VectorD3D(1, 0, 0);
				r._normals[3] = new VectorD3D(1, 0, 0);
				r._normals[4] = new VectorD3D(0, 1, 0);
				r._normals[5] = new VectorD3D(0, 1, 0);

				r.NormalizeNormals();

				return r;
			}
		}
	}
}