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
	/// <summary>
	/// Represents the cross section of a line. It is assumed here that the midpoint of the line is the point (0,0) and that all
	/// edges can be connected to the midpoint without leaving the cross section.
	/// </summary>
	public class CrossSectionOfLine : ICrossSectionOfLine
	{
		protected double _size1, _size2;

		protected PointD3D[] _vertices;
		protected bool[] _isVertexSharp;
		protected VectorD3D[] _normals;

		public double Size1 { get { return _size1; } }
		public double Size2 { get { return _size2; } }

		public virtual ICrossSectionOfLine WithSize(double size1, double size2)
		{
			if (_size1 == size1 && _size2 == size2)
			{
				return this;
			}
			else
			{
				double scaleX = size1 / _size1;
				double scaleY = size2 / _size2;

				var result = new CrossSectionOfLine();
				result._size1 = size1;
				result._size2 = size2;
				result._vertices = new PointD3D[this._vertices.Length];
				for (int i = 0; i < _vertices.Length; ++i)
					result._vertices[i] = new PointD3D(_vertices[i].X * scaleX, _vertices[i].Y * scaleY, _vertices[i].Z);
				result._isVertexSharp = (bool[])this._isVertexSharp.Clone();
				result._normals = new VectorD3D[this._normals.Length];
				for (int i = 0; i < _normals.Length; ++i)
					result._normals[i] = new VectorD3D(_normals[i].X * scaleY, _normals[i].Y * scaleX, _normals[i].Z).Normalized;
				return result;
			}
		}

		public virtual ICrossSectionOfLine WithSize1(double size1)
		{
			return WithSize(size1, _size2);
		}

		public virtual ICrossSectionOfLine WithSize2(double size2)
		{
			return WithSize(_size1, size2);
		}

		public PointD3D[] Vertices { get { return _vertices; } }
		public bool[] IsVertexSharp { get { return _isVertexSharp; } }
		public VectorD3D[] Normals { get { return _normals; } }

		public int NumberOfVertices { get { return _vertices.Length; } }
		public int NumberOfNormals { get { return _normals.Length; } }

		public double GetMaximalDistanceFromCenter()
		{
			double result = 0;
			for (int i = 0; i < _vertices.Length; ++i)
			{
				var p = _vertices[i];
				double l = Math.Sqrt(p.X * p.X + p.Y * p.Y);
				result = Math.Max(result, l);
			}
			return result;
		}

		public double GetDistanceFromCenter(int i)
		{
			var p = _vertices[i];
			return Math.Sqrt(p.X * p.X + p.Y * p.Y);
		}

		public static CrossSectionOfLine GetRegularSharpPolygon(double radius, int edges)
		{
			CrossSectionOfLine result = new CrossSectionOfLine();

			result._vertices = new PointD3D[edges];
			result._isVertexSharp = new bool[edges];
			result._normals = new VectorD3D[2 * edges];

			for (int i = 0; i < edges; ++i)
			{
				double phi = (Math.PI * (2 * i)) / edges;
				result._vertices[i] = new PointD3D(radius * Math.Cos(phi), radius * Math.Sin(phi), 0);
				result._isVertexSharp[i] = true;
				double phim = (Math.PI * (2 * i - 1)) / edges;
				result._normals[2 * i] = new VectorD3D(Math.Cos(phim), Math.Sin(phim), 0);
				double phip = (Math.PI * (2 * i + 1)) / edges;
				result._normals[2 * i + 1] = new VectorD3D(Math.Cos(phip), Math.Sin(phip), 0);
			}

			// normals
			return result;
		}

		public static CrossSectionOfLine GetRegularRoundedPolygon(double radius, int edges)
		{
			CrossSectionOfLine result = new CrossSectionOfLine();

			result._vertices = new PointD3D[edges];
			result._isVertexSharp = new bool[edges];
			result._normals = new VectorD3D[edges];

			for (int i = 0; i < edges; ++i)
			{
				double phi = (2 * Math.PI * i) / edges;
				result._vertices[i] = new PointD3D(radius * Math.Cos(phi), radius * Math.Sin(phi), 0);
				result._isVertexSharp[i] = false;
				result._normals[i] = new VectorD3D(Math.Cos(phi), Math.Sin(phi), 0);
			}

			// normals
			return result;
		}

		public static CrossSectionOfLine GetStarShapedPolygon(double innerRadius, double outerRadius, int starArms)
		{
			CrossSectionOfLine result = new CrossSectionOfLine();

			int numVertices = starArms * 2;
			int numNormals = 2 * numVertices;
			result._vertices = new PointD3D[numVertices];
			result._isVertexSharp = new bool[numVertices];
			result._normals = new VectorD3D[numNormals];

			for (int i = 0; i < numVertices; ++i)
			{
				double phi = (Math.PI * (2 * i)) / numVertices;
				double radius = 0 == (i % 2) ? outerRadius : innerRadius;
				result._vertices[i] = new PointD3D(radius * Math.Cos(phi), radius * Math.Sin(phi), 0);
				result._isVertexSharp[i] = true;
			}

			// now the normals, we calculate them using the cross product with the z-axis
			VectorD3D zaxis = new VectorD3D(0, 0, 1);
			for (int i = 0; i < numVertices; ++i)
			{
				VectorD3D line = result._vertices[(i + 1) % numVertices] - result._vertices[i];
				VectorD3D cross = VectorD3D.CrossProduct(line, zaxis);
				cross.Normalize();
				result._normals[(2 * i + 1) % numNormals] = cross;
				result._normals[(2 * i + 2) % numNormals] = cross;
			}

			// normals
			return result;
		}

		public static CrossSectionOfLine GetQuadraticCrossSection(double width)
		{
			CrossSectionOfLine result = new CrossSectionOfLine();
			double w2 = width / 2;
			result._vertices = new PointD3D[4];
			result._isVertexSharp = new bool[4];
			result._normals = new VectorD3D[8];

			result._vertices[0] = new PointD3D(w2, -w2, 0);
			result._vertices[1] = new PointD3D(w2, w2, 0);
			result._vertices[2] = new PointD3D(-w2, w2, 0);
			result._vertices[3] = new PointD3D(-w2, -w2, 0);

			result._isVertexSharp[0] = true;
			result._isVertexSharp[1] = true;
			result._isVertexSharp[2] = true;
			result._isVertexSharp[3] = true;

			result.Normals[0] = new VectorD3D(0, -1, 0);
			result.Normals[1] = new VectorD3D(1, 0, 0);
			result.Normals[2] = new VectorD3D(1, 0, 0);
			result.Normals[3] = new VectorD3D(0, 1, 0);
			result.Normals[4] = new VectorD3D(0, 1, 0);
			result.Normals[5] = new VectorD3D(-1, 0, 0);
			result.Normals[6] = new VectorD3D(-1, 0, 0);
			result.Normals[7] = new VectorD3D(0, -1, 0);

			return result;
		}

		public static CrossSectionOfLine GetSquareCrossSection(double width, double height)
		{
			CrossSectionOfLine result = new CrossSectionOfLine();
			double w2 = width / 2;
			double h2 = height / 2;
			result._vertices = new PointD3D[4];
			result._isVertexSharp = new bool[4];
			result._normals = new VectorD3D[8];

			result._vertices[0] = new PointD3D(w2, -h2, 0);
			result._vertices[1] = new PointD3D(w2, h2, 0);
			result._vertices[2] = new PointD3D(-w2, h2, 0);
			result._vertices[3] = new PointD3D(-w2, -h2, 0);

			result._isVertexSharp[0] = true;
			result._isVertexSharp[1] = true;
			result._isVertexSharp[2] = true;
			result._isVertexSharp[3] = true;

			result.Normals[0] = new VectorD3D(0, -1, 0);
			result.Normals[1] = new VectorD3D(1, 0, 0);
			result.Normals[2] = new VectorD3D(1, 0, 0);
			result.Normals[3] = new VectorD3D(0, 1, 0);
			result.Normals[4] = new VectorD3D(0, 1, 0);
			result.Normals[5] = new VectorD3D(-1, 0, 0);
			result.Normals[6] = new VectorD3D(-1, 0, 0);
			result.Normals[7] = new VectorD3D(0, -1, 0);

			return result;
		}
	}
}