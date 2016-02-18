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

namespace Altaxo.Geometry
{
	public static class Math3D
	{
		/// <summary>
		/// Calculates a vector, that is the result of a given is a vector n, that is mirrored at a plane given by its normal q. Attention:
		/// The resulting vector is truely mirrored, i.e. an incident vector is mirrored to an outcoming vector!!!
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
		/// Calculates a vector, that is the result of a given vector n, that is mirrored at a plane given by its normal q. Attention:
		/// The resulting vector is symmetrically mirrored, i.e. an incident vector is mirrored to an incident vector, and an outcoming vector is mirrored to an outcoming vector.
		/// </summary>
		/// <param name="n">The vector to be mirrored. Is not neccessary to be normalized.</param>
		/// <param name="q">Normal of a plane where the vector n is mirrored. Is not neccessary to be normalized.</param>
		/// <returns>The vector which is the result of mirroring the vector n at a plane given by its normal q.</returns>
		public static VectorD3D GetSymmetricallyMirroredVectorAtPlane(VectorD3D n, VectorD3D q)
		{
			double two_nq_qq = 2 * VectorD3D.DotProduct(n, q) / VectorD3D.DotProduct(q, q);
			return new VectorD3D(q.X * two_nq_qq - n.X, q.Y * two_nq_qq - n.Y, q.Z * two_nq_qq - n.Z);
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
			var result = VectorD3D.CreateNormalized(n.X - v.X * nv_vv, n.Y - v.Y * nv_vv, n.Z - v.Z * nv_vv);
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
		public static Matrix4x3 Get2DProjectionToPlaneToPlane(VectorD3D e, VectorD3D n, VectorD3D v, PointD3D p, VectorD3D q)
		{
			double qn = VectorD3D.DotProduct(q, e);
			double qw = VectorD3D.DotProduct(q, n);
			double qv = VectorD3D.DotProduct(q, v);

			double qn_qv = qn / qv;
			double qw_qv = qw / qv;

			return new Matrix4x3(
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
		public static Matrix4x3 Get2DProjectionToPlane(VectorD3D e, VectorD3D n, PointD3D p)
		{
			return new Matrix4x3(
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

		private static VectorD3D _xVector = new VectorD3D(1, 0, 0);
		private static VectorD3D _yVector = new VectorD3D(0, 1, 0);
		private static VectorD3D _zVector = new VectorD3D(0, 0, 1);

		public static VectorD3D FindStartEastVector(IEnumerable<PointD3D> linePoints)
		{
			VectorD3D v = VectorD3D.Empty;
			bool isPrevPointValid = false;
			PointD3D prevPoint = PointD3D.Empty;
			foreach (var p in linePoints)
			{
				if (isPrevPointValid)
				{
					v = p - prevPoint;
					if (v.Length > 0)
						break;
				}
				prevPoint = p;
				isPrevPointValid = true;
			}

			if (!(v.Length > 0))
				throw new ArgumentException("Either too less points were given or the all the points fall together, thus no first valid vector could be determined", nameof(linePoints));

			return FindStartEastVector(v);
		}

		public static VectorD3D FindStartEastVector(VectorD3D v)
		{
			const double minAngle = 1E-4;
			const double maxAngle = Math.PI - minAngle;

			if (!(v.Length > 0))
				throw new ArgumentException("Start vector of the line is invalid or empty", nameof(v));

			double angle;
			angle = VectorD3D.AngleBetweenInRadians(v, _xVector);
			if (angle > minAngle && angle < maxAngle)
				return _xVector;
			angle = VectorD3D.AngleBetweenInRadians(v, _yVector);
			if (angle > minAngle && angle < maxAngle)
				return _yVector;
			angle = VectorD3D.AngleBetweenInRadians(v, _yVector);
			if (angle > minAngle && angle < maxAngle)
				return _zVector;

			// if this was still not successfull then use y if x.y is not null, or x if

			if (VectorD3D.DotProduct(v, _yVector) != 0)
				return _yVector;
			else if (VectorD3D.DotProduct(v, _xVector) != 0)
				return _xVector;
			else
				return _zVector;
		}

		/// <summary>
		/// Gets the east and north vector for a single straight line.
		/// </summary>
		/// <param name="line">The line.</param>
		/// <returns>The east and the north vector (Item1=east vector, Item2 = north vector).</returns>
		public static Tuple<VectorD3D, VectorD3D> GetEastNorthVector(LineD3D line)
		{
			var v = line.P1 - line.P0;
			var e = FindStartEastVector(v);
			var n = VectorD3D.CrossProduct(e, v).Normalized;
			e = VectorD3D.CrossProduct(v, n).Normalized;

			return new Tuple<VectorD3D, VectorD3D>(e, n);
		}

		/// <summary>
		/// Amends a polyline, given by its polyline points, with an east and a north vector for each polyline point.
		/// </summary>
		/// <param name="linePoints">The line points.</param>
		/// <returns>The polyline points, amended with east and north vector (Item1: polyline point, Item2: east vector, Item3: north vector).
		/// The number of points may be smaller than the original number of points, because empty line segments are not returned.
		/// The east and north vectors are valid for the segment going from the previous point to the current point (thus for the first and the second point the returned east and north vectors are equal).</returns>
		public static IEnumerable<Tuple<PointD3D, VectorD3D, VectorD3D>> GetPolylinePointsWithEastAndNorth(IEnumerable<PointD3D> linePoints)
		{
			bool prevPointIsValid = false;
			PointD3D prevPoint = PointD3D.Empty;

			VectorD3D n = VectorD3D.Empty;
			VectorD3D e = VectorD3D.Empty;
			VectorD3D previousSegment = VectorD3D.Empty;
			VectorD3D currentSegment = VectorD3D.Empty;

			foreach (var p in linePoints)
			{
				if (prevPointIsValid)
				{
					currentSegment = p - prevPoint;
					var len = currentSegment.Length;
					if (!(len > 0))
						continue;
					currentSegment /= len; // current segment normalized

					if (previousSegment.IsEmpty)
					{
						var startEastVector = FindStartEastVector(currentSegment);
						n = VectorD3D.CrossProduct(startEastVector, currentSegment);
						e = VectorD3D.CrossProduct(currentSegment, n);
					}

					yield return new Tuple<PointD3D, VectorD3D, VectorD3D>(prevPoint, e, n);

					if (!previousSegment.IsEmpty)
					{
						// if there was a previous segment, then calculate the new east and north vectors
						VectorD3D midPlaneNormal = 0.5 * (currentSegment - previousSegment);
						double dot_e = midPlaneNormal.X * e.X + midPlaneNormal.Y * e.Y + midPlaneNormal.Z * e.Z;
						double dot_n = midPlaneNormal.X * n.X + midPlaneNormal.Y * n.Y + midPlaneNormal.Z * n.Z;

						if (Math.Abs(dot_n) > Math.Abs(dot_e))
						{
							n = GetSymmetricallyMirroredVectorAtPlane(n, midPlaneNormal);
							n = GetOrthonormalVectorToVector(n, currentSegment); // make the north vector orthogonal (it should be already, but this corrects small deviations)
							e = VectorD3D.CrossProduct(currentSegment, n);
						}
						else if (Math.Abs(dot_e) > float.Epsilon) //
						{
							e = GetSymmetricallyMirroredVectorAtPlane(e, midPlaneNormal);
							e = GetOrthonormalVectorToVector(e, currentSegment); // make the north vector orthogonal (it should be already, but this corrects small deviations)
							n = VectorD3D.CrossProduct(e, currentSegment);
						}
						else // previous segment and current segment are either colinear or a perfect reflection. Keep the north vector, and calculate only the new east vector
						{
							n = GetOrthonormalVectorToVector(n, currentSegment); // make the north vector orthogonal (it should be already, but this corrects small deviations)
							e = VectorD3D.CrossProduct(currentSegment, n);
						}
					}

					previousSegment = currentSegment;
				}

				prevPoint = p;
				prevPointIsValid = true;
			}

			if (previousSegment.Length > 0)
				yield return new Tuple<PointD3D, VectorD3D, VectorD3D>(prevPoint, e, n);
		}
	}
}