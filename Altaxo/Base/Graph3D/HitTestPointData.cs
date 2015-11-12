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

namespace Altaxo.Graph3D
{
	/// <summary>
	/// Holds information about a hitted point on the screen.
	/// </summary>
	public class HitTestPointData
	{
		/// <summary>Transformation of this item that transform world coordinates to page coordinates.</summary>
		private MatrixD3D _transformation;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="hitPointPageCoord">Page coordinates (unit: points).</param>
		/// <param name="pageScale">Current zoom factor, i.e. ration between displayed size on the screen and given size.</param>
		public HitTestPointData(MatrixD3D transformation)
		{
			_transformation = transformation;
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="from">Another HitTestData object to copy from.</param>
		public HitTestPointData(HitTestPointData from)
		{
			this._transformation = from._transformation;
		}

		/// <summary>
		/// Transformation of this item that transform world coordinates to page coordinates.
		/// </summary>
		public MatrixD3D Transformation
		{
			get
			{
				return _transformation;
			}
		}

		/// <summary>
		/// Gets the transformation of this item plus an additional transformation. Both together transform world coordinates to page coordinates.
		/// </summary>
		/// <param name="additionalTransformation">The additional transformation matrix.</param>
		/// <returns></returns>
		public MatrixD3D GetTransformation(MatrixD3D additionalTransformation)
		{
			MatrixD3D result = _transformation;
			result.PrependTransform(additionalTransformation);
			return result;
		}

		public HitTestPointData NewFromAdditionalTransformation(MatrixD3D additionalTransformation)
		{
			var result = new HitTestPointData(this);
			result._transformation.PrependTransform(additionalTransformation);
			return result;
		}

		/// <summary>
		/// Test if the triangle spanned by p0, p1 and p2 in the x-y plane (z component ignored) includes the point x=0, y=0.
		/// </summary>
		/// <param name="p0">The point p0.</param>
		/// <param name="p1">The point p1.</param>
		/// <param name="p2">The point p2.</param>
		/// <param name="z">The minimum z component of all three provided points.</param>
		/// <returns>True if the point x=0, y=0 is included in the triangle, otherwise false.</returns>
		private bool HitTestWithAlreadyTransformedPoints(PointD3D p0, PointD3D p1, PointD3D p2, out double z)
		{
			if (
				(p0.X * p1.Y - p0.Y * p1.X) <= 0 &&
				(p1.X * p2.Y - p1.Y * p2.X) <= 0 &&
				(p2.X * p0.Y - p2.Y * p0.X) <= 0
				)
			{
				z = Math.Min(Math.Min(p0.Z, p1.Z), p2.Z);
				return true;
			}
			else
			{
				z = double.NaN;
				return false;
			}
		}

		/// <summary>
		/// Determines whether the specified 3D-rectangle r is hit by a ray given by x=0, y=0, z>0.
		/// </summary>
		/// <param name="r">The rectangle r.</param>
		/// <param name="z">If there was a hit, this is the z coordinate of the hit.</param>
		/// <returns>True if the rectangle is hit by a ray given by x=0, y=0, z>0.</returns>
		public bool IsHit(RectangleD3D r, out double z)
		{
			PointD3D[] vertices = new PointD3D[8];

			int i = 0;
			foreach (var v in r.Vertices)
				vertices[i++] = _transformation.TransformPoint(v);

			foreach (var ti in r.TriangleIndices)
			{
				if (HitTestWithAlreadyTransformedPoints(vertices[ti.Item1], vertices[ti.Item2], vertices[ti.Item3], out z) && z < 0)
					return true;
			}

			z = double.NaN;
			return false;
		}
	}
}