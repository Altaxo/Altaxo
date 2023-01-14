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

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Geometry
{
  public static class Math3D
  {
    /// <summary>
    /// Calculates the counterpart of the provided vector <paramref name="n"/>, so that this vector <paramref name="n"/> and it's conterpart are symmetrical,
    /// with the symmetry line provided by vector <paramref name="q"/>.
    /// </summary>
    /// <param name="n">The vector for which to find the symmectrical counterpart. Not required to be normalized.</param>
    /// <param name="q">Symmetry line.</param>
    /// <returns>The counterpart of the provided vector <paramref name="n"/>, so that this vector <paramref name="n"/> and it's conterpart are symmetrical,
    /// with the symmetry line given by vector <paramref name="q"/>.</returns>
    public static VectorD3D GetVectorSymmetricalToVector(VectorD3D n, VectorD3D q)
    {
      double two_nq_qq = 2 * VectorD3D.DotProduct(n, q) / VectorD3D.DotProduct(q, q);
      return new VectorD3D(q.X * two_nq_qq - n.X, q.Y * two_nq_qq - n.Y, q.Z * two_nq_qq - n.Z);
    }

    /// <summary>
    /// Calculates a vector which is symmectrical to the provided vector <paramref name="n"/> with respected to the symmetry plane given by the normal normal <paramref name="q"/>.
    /// The result is the same as if a ray is reflected on a miiror described by the plane, thus
    /// an incident vector is resulting in an outcoming vector, and an outcoming vector is resulting in an incident vector.
    /// </summary>
    /// <param name="n">The vector for which to find the symmectrical counterpart. Not required to be normalized.</param>
    /// <param name="q">Normal of a plane where the vector n is mirrored. Not required to be normalized.</param>
    /// <returns>A vector which is symmectrical to the provided vector <paramref name="n"/> with respected to the symmetry plane given by the normal normal <paramref name="q"/>.</returns>
    public static VectorD3D GetVectorSymmetricalToPlane(VectorD3D n, VectorD3D q)
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
    public static VectorD3D GetVectorOrthogonalToVector(VectorD3D n, VectorD3D v)
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
    public static VectorD3D GetNormalizedVectorOrthogonalToVector(VectorD3D n, VectorD3D v)
    {
      double nv_vv = VectorD3D.DotProduct(n, v) / VectorD3D.DotProduct(v, v);
      var result = VectorD3D.CreateNormalized(n.X - v.X * nv_vv, n.Y - v.Y * nv_vv, n.Z - v.Z * nv_vv);
      return result;
    }

    /// <summary>
    /// Gets a projection matrix that projects a point in the direction given by <paramref name="v"/> onto a plane with is given by an arbitrary point on the plane <paramref name="p"/> and the plane's normal <paramref name="q"/>.
    /// </summary>
    /// <param name="v">The projection direction. Not required to be normalized.</param>
    /// <param name="p">An arbitrary point onto the projection plane.</param>
    /// <param name="q">The projection plane's normal. Not required to be normalized.</param>
    /// <returns>The projection matrix that projects a point in the direction given by <paramref name="v"/> onto a plane with is given by an arbitrary point on the plane <paramref name="p"/> and the plane's normal <paramref name="q"/>.</returns>
    public static Matrix4x3 GetProjectionToPlane(VectorD3D v, PointD3D p, VectorD3D q)
    {
      double OneByQV = 1 / VectorD3D.DotProduct(q, v);
      double DotPQ = p.X * q.X + p.Y * q.Y + p.Z * q.Z;

      return new Matrix4x3(
        1 - q.X * v.X * OneByQV, -q.X * v.Y * OneByQV, -q.X * v.Z * OneByQV,
        -q.Y * v.X * OneByQV, 1 - q.Y * v.Y * OneByQV, -q.Y * v.Z * OneByQV,
        -q.Z * v.X * OneByQV, -q.Z * v.Y * OneByQV, 1 - q.Z * v.Z * OneByQV,
        DotPQ * v.X * OneByQV, DotPQ * v.Y * OneByQV, DotPQ * v.Z * OneByQV
        );
    }

    /// <summary>
    /// Creates a transformation matrix that does the following: First, it converts a 2D point into a 3D coordinate system with the origin given by <paramref name="p"/>, and the unit vectors <paramref name="e"/> and <paramref name="n"/>.
    /// Then the thus created 3D point is projected in the direction of <paramref name="v"/> onto a plane that is defined by the same point <paramref name="p"/> on the plane and the plane's normal <paramref name="q"/>.
    /// </summary>
    /// <param name="e">East vector: Spans one dimension of the projection of the 2D points to a 3D plane.</param>
    /// <param name="n">North vector: Spans the other dimension of the projection of the 2D input points to a 3D plane.</param>
    /// <param name="v">Direction of the projection of the 3D points to a plane.</param>
    /// <param name="p">Origin of the coordinate system, and point on the projection plane, too.</param>
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
    /// if the point <paramref name="a"/> is located in the half space into which the vector <paramref name="q"/> is pointing.
    /// </summary>
    /// <param name="a">The point a.</param>
    /// <param name="p">A point on a plane.</param>
    /// <param name="q">The normal vector of that plane (can be not-normalized).</param>
    /// <returns></returns>
    public static double GetDistancePointToPlane(PointD3D a, PointD3D p, VectorD3D q)
    {
      return ((a.X - p.X) * q.X + (a.Y - p.Y) * q.Y + (a.Z - p.Z) * q.Z) / q.Length;
    }

    /// <summary>
    /// Gets the fractional index of the point on a line that has a certain distance to another point <paramref name="ps"/>.
    /// </summary>
    /// <param name="p0">The start point of the line.</param>
    /// <param name="p1">The end point of the line.</param>
    /// <param name="ps">The other point.</param>
    /// <param name="distance">The given distance.</param>
    /// <returns>A relative index on the line [0..1] for the point on the line that has the provided distance to the point <paramref name="ps"/>. If the point <paramref name="ps"/> is too far away, the result will be double.NaN.
    /// If the point <paramref name="ps"/> is too close, the result can be outside the interval [0,1].</returns>
    public static double GetFractionalIndexOfPointOnLineInGivenDistanceToAnotherPoint(PointD3D p0, PointD3D p1, PointD3D ps, double distance)
    {
      VectorD3D p0s = p0 - ps;
      VectorD3D seg = p1 - ps;
      double dotps = VectorD3D.DotProduct(p0s, seg);
      double slen_p0s = p0s.SquareOfLength;
      double slen_seg = seg.SquareOfLength;

      double sqrt = Math.Sqrt(dotps * dotps + (distance * distance - slen_p0s) * slen_seg);

      double t1 = (-dotps - sqrt) / slen_seg;
      double t2 = (-dotps + sqrt) / slen_seg;

      return t1 >= 0 ? t1 : t2;
    }

    /// <summary>
    /// Dissects a straight line into individual line segments, using a dash pattern.
    /// </summary>
    /// <param name="line">The line to dissect.</param>
    /// <param name="dashPattern">The dash pattern.</param>
    /// <param name="dashPatternOffset">The dash pattern offset (relative units, i.e. same units as dashPattern itself).</param>
    /// <param name="dashPatternScale">The dash pattern scale.</param>
    /// <param name="dashPatternStartAbsolute">An absolute length. This parameter is similar to <paramref name="dashPatternOffset"/>, but in absolute units.</param>
    /// <returns></returns>
    public static IEnumerable<LineD3D> DissectStraightLineWithDashPattern(LineD3D line, IReadOnlyList<double> dashPattern, double dashPatternOffset, double dashPatternScale, double dashPatternStartAbsolute)
    {
      int dashIndex = 0;
      int dashCount = dashPattern.Count;

      // Fast forward in dash
      double remainingOffset = dashPatternOffset;
      double currDash = dashPattern[dashIndex];

      while (remainingOffset > 0)
      {
        if ((remainingOffset - currDash) >= 0)
        {
          dashIndex = (dashIndex + 1) % dashCount;
          remainingOffset = remainingOffset - currDash;
          currDash = dashPattern[dashIndex];
        }
        else
        {
          currDash -= remainingOffset;
          remainingOffset = 0;
        }
      }

      // now move forward to dashPatternStartAbsolute
      double remainingOffsetAbsolute = dashPatternStartAbsolute;
      while (remainingOffsetAbsolute > 0)
      {
        var diff = remainingOffsetAbsolute - currDash * dashPatternScale;
        if (diff >= 0)
        {
          dashIndex = (dashIndex + 1) % dashCount;
          remainingOffsetAbsolute = diff;
          currDash = dashPattern[dashIndex];
        }
        else
        {
          currDash -= remainingOffsetAbsolute / dashPatternScale;
          remainingOffsetAbsolute = 0;
        }
      }

      // now we are ready to start
      double lineLength = line.Length;

      double sumPrev = 0;
      double lengthPrev = 0;
      for (; lengthPrev < lineLength;)
      {
        double sumCurr = sumPrev + currDash;
        double lengthCurr = sumCurr * dashPatternScale;
        if (lengthCurr >= lineLength)
        {
          lengthCurr = lineLength;
        }

        if ((0 == dashIndex % 2) && (lengthCurr > lengthPrev))
        {
          yield return new LineD3D(
            line.GetPointAtLineFromRelativeValue(lengthPrev / lineLength),
            line.GetPointAtLineFromRelativeValue(lengthCurr / lineLength)
            );
        }

        sumPrev = sumCurr;
        lengthPrev = lengthCurr;
        dashIndex = (dashIndex + 1) % dashCount;
        currDash = dashPattern[dashIndex];
      }
    }

    /// <summary>
    /// Gets the relative positions of the two points on a line segment that have a given distance to a third point. The returned relative values are in the range [-Infinity, Infinity] and
    /// therefore don't neccessarily lie directly on the line segment. Furthermore, a solution not always exists (in this case the returned values are NaN).
    /// </summary>
    /// <param name="p0">The start point of the line segment..</param>
    /// <param name="p1">The end point of the line segment.</param>
    /// <param name="ps">The third point.</param>
    /// <param name="distance">The distance between a point on the line sigment and the third point.</param>
    /// <returns>The relative positions of the points on the line segment that have the provided distance to the third point. The returned relative values are in the range [-Infinity, Infinity] and
    /// therefore don't neccessarily lie directly on the line segment. Furthermore, a solution not always exists (in this case the returned values are NaN). </returns>
    public static Tuple<double, double> GetRelativePositionsOnLineSegmentForPointsAtDistanceToPoint(PointD3D p0, PointD3D p1, PointD3D ps, double distance)
    {
      // we rescale the problem so that p0 is becoming the origin
      p1 = new PointD3D(p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z);
      ps = new PointD3D(ps.X - p0.X, ps.Y - p0.Y, ps.Z - p0.Z);

      var p1Sq = p1.X * p1.X + p1.Y * p1.Y + p1.Z * p1.Z;
      var psSq = ps.X * ps.X + ps.Y * ps.Y + ps.Z * ps.Z;
      var p1ps = p1.X * ps.X + p1.Y * ps.Y + p1.Z * ps.Z;
      var squareRootTerm = Math.Sqrt(p1ps * p1ps - p1Sq * (psSq - distance * distance));
      var t1 = (p1ps - squareRootTerm) / p1Sq;
      var t2 = (p1ps + squareRootTerm) / p1Sq;
      return new Tuple<double, double>(t1, t2);
    }

    private static void CutPolylineForStartCap(IEnumerable<PointD3D> polyLine, double startCapLength, double polyLineThickness)
    {
      var startCapLengthSquare = startCapLength * startCapLength;
      var en = polyLine.GetEnumerator();
      if (!en.MoveNext())
        throw new ArgumentException("Polyline is empty", nameof(polyLine));

      var prevPoint = en.Current;
      var firstPoint = en.Current;

      while (en.MoveNext())
      {
        var currPoint = en.Current;

        if ((currPoint - firstPoint).SquareOfLength < startCapLengthSquare)
        {
          prevPoint = currPoint;
          continue; // Fast skip
        }

        // now we have to look at this segment in more detail
        // we need the angle between this segment and the
      }
    }
  }
}
