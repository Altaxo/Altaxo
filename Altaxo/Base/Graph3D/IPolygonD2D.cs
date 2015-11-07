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

using Altaxo.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph3D
{
	/// <summary>
	/// Represents a closed polygon in 2D space.
	/// </summary>
	public interface IPolygonD2D
	{
		/// <summary>
		/// Gets the points that form the closed polygon.
		/// </summary>
		/// <value>
		/// The points.
		/// </value>
		IList<PointD2D> Points { get; }

		/// <summary>
		/// Gets the points of the polygon which are sharp points. Points of the polygon which are not in this set are considered to be soft points.
		/// </summary>
		/// <value>
		/// The sharp points.
		/// </value>
		ISet<PointD2D> SharpPoints { get; }
	}

	/// <summary>
	/// Implementation of a closed polygon in 2D space.
	/// </summary>
	public class PolygonD2D
	{
		protected PointD2D[] _points;

		protected HashSet<PointD2D> _sharpPoints;

		protected bool? _isHole;

		/// <summary>
		/// Gets or sets the parent of this polygon.
		/// </summary>
		/// <value>
		/// The parent.
		/// </value>
		public PolygonD2D Parent { get; set; }

		public PolygonD2D(PointD2D[] points, HashSet<PointD2D> sharpPoints)
		{
			_points = points;
			_sharpPoints = sharpPoints;
		}

		public PolygonD2D(PolygonD2D template, double scale)
		{
			_points = template._points.Select(p => new PointD2D(p.X * scale, p.Y * scale)).ToArray();
			_sharpPoints = new HashSet<PointD2D>(template._sharpPoints.Select(p => new PointD2D(p.X * scale, p.Y * scale)));
		}

		/// <summary>
		/// Gets the points that form the closed polygon.
		/// </summary>
		/// <value>
		/// The points.
		/// </value>
		public PointD2D[] Points { get { return _points; } }

		/// <summary>
		/// Gets the points of the polygon which are sharp points. Points of the polygon which are not in this set are considered to be soft points.
		/// </summary>
		/// <value>
		/// The sharp points.
		/// </value>
		public HashSet<PointD2D> SharpPoints { get { return _sharpPoints; } }

		/// <summary>
		/// Gets or sets a value indicating whether this polygon is a hole. In this case it belongs to another, outer, polygon.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is a hole; otherwise, <c>false</c>.
		/// </value>
		/// <exception cref="System.NotImplementedException"></exception>
		public bool IsHole
		{
			get
			{
				if (!_isHole.HasValue)
				{
					throw new NotImplementedException();
					//return PolygonHelper.GetPolygonArea(_points) > 0;
				}
				else
				{
					return _isHole.Value;
				}
			}
			set
			{
				_isHole = value;
			}
		}
	}

	/// <summary>
	/// Stores a closed polygon and the normals. In order to distinguish between soft vertices and sharp vertices, the sharp vertices are
	/// stored twice, because every sharp vertex has two normals. Thus there is a 1:1 relationship between the indices of the normals and the points.
	/// </summary>
	public class PolygonWithNormalsD2D
	{
		private PointD2D[] _points;
		private PointD2D[] _normals;

		public PointD2D[] Points { get { return _points; } }
		public PointD2D[] Normals { get { return _normals; } }

		public PolygonWithNormalsD2D(PolygonD2D template)
		{
			var numPoints = template.Points.Length + template.SharpPoints.Count;

			_points = new PointD2D[numPoints];
			_normals = new PointD2D[numPoints];

			var srcPoints = template.Points;
			var srcCount = srcPoints.Length;
			var startPoint = srcPoints[srcCount - 1];

			int destIdx = 0;
			for (int i = 0; i < srcCount; ++i)
			{
				var toHereVector = srcPoints[i] - startPoint;
				var fromHereVector = srcPoints[(i + 1) % srcCount] - srcPoints[i];

				if (template.SharpPoints.Contains(srcPoints[i]))
				{
					var normal = GetNormal(toHereVector, template.IsHole);
					_points[destIdx] = srcPoints[i];
					_normals[destIdx] = normal;
					++destIdx;

					normal = GetNormal(fromHereVector, template.IsHole);
					_points[destIdx] = srcPoints[i];
					_normals[destIdx] = normal;
					++destIdx;
				}
				else
				{
					var normal = GetNormal(toHereVector + fromHereVector, template.IsHole);
					_points[destIdx] = srcPoints[i];
					_normals[destIdx] = normal;
					++destIdx;
				}

				startPoint = srcPoints[i];
			}

			if (numPoints != destIdx)
			{
				System.Diagnostics.Debug.Assert(destIdx == numPoints);
			}
		}

		private PointD2D GetNormal(PointD2D polygonVector, bool isHole)
		{
			return new PointD2D(-polygonVector.Y, polygonVector.X);
		}
	}
}