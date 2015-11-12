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

namespace Altaxo.Graph3D
{
	public static class Math3D
	{
		/// <summary>
		/// Calculates a vector, that is the result of a given is a vector n, that is mirrored at a plane given by its normal q.
		/// </summary>
		/// <param name="n">The vector to be mirrored. Is not neccessary to be normalized.</param>
		/// <param name="q">Normal of a plane where the vector n is mirrored. Is not neccessary to be normalized.</param>
		/// <returns>The vector which is the result of mirroring the vector n at a plane given by its normal q.</returns>
		public static VectorD3D GetMirroredVectorAtPlane(VectorD3D n, VectorD3D q)
		{
			double two_nq_qq = 2 * VectorD3D.DotProduct(n, q) / VectorD3D.DotProduct(q, q);
			return new VectorD3D(n.X - q.X * two_nq_qq, n.Y - q.Y * two_nq_qq, n.Z - q.Z * two_nq_qq);
		}

		/// <summary>
		/// Makes a given vector n orthogonal to another vector v. This is done by adding a fraction of v to n, so that the new vector is orthogonal to v.
		/// </summary>
		/// <param name="n">Given vector.</param>
		/// <param name="v">A vector, to which the returned vector should be perpendicular.</param>
		/// <returns>A new vector n+t*v, so that this vector is orthogonal to v (but not neccessarily normalized).</returns>
		public static VectorD3D GetOrthogonalVectorToVector(VectorD3D n, VectorD3D v)
		{
			double nv_vv = VectorD3D.DotProduct(n, v) / VectorD3D.DotProduct(v, v);
			return new VectorD3D(n.X - v.X * nv_vv, n.Y - v.Y * nv_vv, n.Z - v.Z * nv_vv);
		}

		/// <summary>
		/// Makes a given vector n orthogonal to another vector v. This is done by adding a fraction of v to n, so that the new vector is orthogonal to v.
		/// After this, the vector is normalized.
		/// </summary>
		/// <param name="n">Given vector.</param>
		/// <param name="v">A vector, to which the returned vector should be perpendicular.</param>
		/// <returns>A new vector n+t*v, so that this vector is orthogonal to v and normalized.</returns>
		public static VectorD3D GetOrthonormalVectorToVector(VectorD3D n, VectorD3D v)
		{
			double nv_vv = VectorD3D.DotProduct(n, v) / VectorD3D.DotProduct(v, v);
			var result = new VectorD3D(n.X - v.X * nv_vv, n.Y - v.Y * nv_vv, n.Z - v.Z * nv_vv);
			result.Normalize();
			return result;
		}

		/// <summary>
		/// Creates a transformation matrix that projects a 2D point to a 3D-plane spanned by <paramref name="e"/> and <paramref name="n"/>. Then the thus created 3D point is projected
		/// in the direction of <paramref name="v"/> to a plane that is defined by a point <paramref name="p"/> on the plane and the plane's normal <paramref name="q"/>.
		/// </summary>
		/// <param name="e">East vector: Spans one dimension of the projection of the 2D points to a 3D plane.</param>
		/// <param name="n">North vector: Spans the other dimension of the projection of the 2D input points to a 3D plane.</param>
		/// <param name="v">Direction of the projection of the 3D points to a plane.</param>
		/// <param name="p">Point on the projection plane.</param>
		/// <param name="q">Normal of the projection plane.</param>
		/// <returns>Matrix that transforms 2D points to a plane. (The 2D points are in fact 3D points with a z-coordinate that is ignored.</returns>
		public static MatrixD3D Get2DProjectionToPlaneToPlane(VectorD3D e, VectorD3D n, VectorD3D v, PointD3D p, VectorD3D q)
		{
			double qn = VectorD3D.DotProduct(q, e);
			double qw = VectorD3D.DotProduct(q, n);
			double qv = VectorD3D.DotProduct(q, v);

			double qn_qv = qn / qv;
			double qw_qv = qw / qv;

			return new MatrixD3D(
				e.X - v.X * qn_qv, e.Y - v.Y * qn_qv, e.Z - v.Z * qn_qv,
				n.X - v.X * qw_qv, n.Y - v.Y * qw_qv, n.Z - v.Z * qw_qv,
				0, 0, 0,
				p.X, p.Y, p.Z);
		}

		/// <summary>
		/// Creates a transformation matrix that projects 2D points (in fact: 3D-points with ignored z-coordinate) to a plane that is defined by 2 vectors (<paramref name="e"/> and <paramref name="n"/>) and a point
		/// on that plane <paramref name="p"/>. The x-coordinates of the original point is projected in the <paramref name="e"/> direction, the y-coordinate in the <paramref name="n"/> direction.
		/// </summary>
		/// <param name="e">East vector: direction, in which the x-coordinate of the original points is projected.</param>
		/// <param name="n">North vector: direction, in which the y-coordinate of the original points is projected.</param>
		/// <param name="p">The 3D point, which is the origin of the spanned plane (the original point with the coordinates (0,0) is projected to this point.</param>
		/// <returns>A transformation matrix that projects 2D points (in fact: 3D-points with ignored z-coordinate) to a plane in 3D space.</returns>
		public static MatrixD3D Get2DProjectionToPlane(VectorD3D e, VectorD3D n, PointD3D p)
		{
			return new MatrixD3D(
				e.X, e.Y, e.Z,
				n.X, n.Y, n.Z,
				0, 0, 0,
				p.X, p.Y, p.Z);
		}

		/// <summary>
		/// Gets the distance of a point <paramref name="a"/> to a plane defined by a point <paramref name="p"/> and a normal vector <paramref name="q"/>. The distance is considered to be positive
		/// if the point <paramref name="a"/> is located in the half space where the vector <paramref name="q"/> is pointing into.
		/// </summary>
		/// <param name="a">The point a.</param>
		/// <param name="p">A point on a plane.</param>
		/// <param name="q">The normal vector of that plane (can be not-normalized).</param>
		/// <returns></returns>
		public static double GetDistancePointToPlane(PointD3D a, PointD3D p, VectorD3D q)
		{
			return ((a.X - p.X) * q.X + (a.Y - p.Y) * q.Y + (a.Z - p.Z) * q.Z) / q.Length;
		}
	}
}