#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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

namespace Altaxo.Geometry
{
  /// <summary>
  /// Converter for points connected by a cardinal spline curve into Bezier segments.
  /// </summary>
  public static class CardinalSplineToBezierSegmentConverter
  {
    // see also source of wine at: http://source.winehq.org/source/dlls/gdiplus/graphicspath.c#L445
    //

    /// <summary>
    /// Calculates Bezier points from cardinal spline endpoints.
    /// </summary>
    /// <param name="end">The end point.</param>
    /// <param name="adj">The adjacent point next to the endpoint.</param>
    /// <param name="tension">The tension.</param>
    /// <returns></returns>
    /// <remarks>Original name in Wine sources: calc_curve_bezier_endp</remarks>
    private static PointD2D Calc_Curve_Bezier_Endpoint(PointD2D end, PointD2D adj, double tension)
    {
      // tangent at endpoints is the line from the endpoint to the adjacent point
      return new PointD2D(
      (tension * (adj.X - end.X) + end.X),
      (tension * (adj.Y - end.Y) + end.Y));
    }

    /// <summary>
    /// Calculates the control points of the incoming and outgoing Bezier segment around the original point at index i+1.
    /// </summary>
    /// <param name="pts">The original points thought to be connected by a cardinal spline.</param>
    /// <param name="i">The index i. To calculate the control points, the indices i, i+1 and i+2 are used from the point array <paramref name="pts"/>.</param>
    /// <param name="tension">The tension of the cardinal spline.</param>
    /// <param name="p1">The Bezier control point that controls the slope towards the point <paramref name="pts"/>[i+1].</param>
    /// <param name="p2">The Bezier control point that controls the slope outwards from the point <paramref name="pts"/>[i+1].</param>
    /// <remarks>Original name in Wine source: calc_curve_bezier</remarks>
    private static void Calc_Curve_Bezier(PointD2D[] pts, int i, double tension, out PointD2D p1, out PointD2D p2)
    {
      /* calculate tangent */
      var diff = pts[2 + i] - pts[i];

      /* apply tangent to get control points */
      p1 = pts[1 + i] - tension * diff;
      p2 = pts[1 + i] + tension * diff;
    }

    /// <summary>
    /// Calculates the control points of the incoming and outgoing Bezier segment around the original point <paramref name="p1"/>.
    /// </summary>
    /// <param name="pts0">The previous point on a cardinal spline curce.</param>
    /// <param name="pts1">The point on a cardinal spline curve for which to calculate the incoming and outgoing Bezier control points.</param>
    /// <param name="pts2">The nex point on the cardinal spline curve.</param>
    /// <param name="tension">The tension of the cardinal spline.</param>
    /// <param name="p1">The Bezier control point that controls the slope towards the point <paramref name="pts1"/>.</param>
    /// <param name="p2">The Bezier control point that controls the slope outwards from the point <paramref name="pts1"/>.</param>
    /// <remarks>This function is not in the original Wine sources. It is introduced here for optimization to avoid allocation of a new array when converting closed cardinal spline curves.</remarks>
    private static void Calc_Curve_Bezier(PointD2D pts0, PointD2D pts1, PointD2D pts2, double tension, out PointD2D p1, out PointD2D p2)
    {
      /* calculate tangent */
      var diff = pts2 - pts0;

      /* apply tangent to get control points */
      p1 = pts1 - tension * diff;
      p2 = pts1 + tension * diff;
    }

    /// <summary>
    /// Converts an open cardinal spline, given by the points in <paramref name="points"/>, to Bezier segments.
    /// </summary>
    /// <param name="points">The control points of the open cardinal spline curve.</param>
    /// <param name="count">Number of control points of the closed cardinal spline curve.</param>
    /// <param name="tension">The tension of the cardinal spline.</param>
    /// <returns>Bezier segments that constitute the closed curve.</returns>
    /// <remarks>Original name in Wine source: GdipAddPathClosedCurve2</remarks>
    public static PointD2D[] OpenCardinalSplineToBezierSegments(PointD2D[] points, int count, double tension)
    {
      const double TENSION_CONST = 0.3;

      int len_pt = count * 3 - 2;
      var pt = new PointD2D[len_pt];
      tension = tension * TENSION_CONST;

      var p1 = Calc_Curve_Bezier_Endpoint(points[0], points[1], tension);
      pt[0] = points[0];
      pt[1] = p1;
      var p2 = p1;

      for (int i = 0; i < count - 2; i++)
      {
        Calc_Curve_Bezier(points, i, tension, out p1, out p2);

        pt[3 * i + 2] = p1;
        pt[3 * i + 3] = points[i + 1];
        pt[3 * i + 4] = p2;
      }

      p1 = Calc_Curve_Bezier_Endpoint(points[count - 1], points[count - 2], tension);

      pt[len_pt - 2] = p1;
      pt[len_pt - 1] = points[count - 1];

      return pt;
    }

    /// <summary>
    /// Converts a closed cardinal spline, given by the points in <paramref name="points"/>, to Bezier segments.
    /// </summary>
    /// <param name="points">The control points of the closed cardinal spline curve.</param>
    /// <param name="count">Number of control points of the closed cardinal spline curve.</param>
    /// <param name="tension">The tension of the cardinal spline.</param>
    /// <returns>Bezier segments that constitute the closed curve.</returns>
    /// <remarks>Original name in Wine source: GdipAddPathClosedCurve2</remarks>
    public static PointD2D[] ClosedCardinalSplineToBezierSegments(PointD2D[] points, int count, double tension)
    {
      const double TENSION_CONST = 0.3;

      var len_pt = (count + 1) * 3 - 2;
      var pt = new PointD2D[len_pt];

      tension = tension * TENSION_CONST;

      PointD2D p1, p2;
      for (int i = 0; i < count - 2; i++)
      {
        Calc_Curve_Bezier(points, i, tension, out p1, out p2);

        pt[3 * i + 2] = p1;
        pt[3 * i + 3] = points[i + 1];
        pt[3 * i + 4] = p2;
      }

      // calculation around last point of the original curve
      Calc_Curve_Bezier(points[count - 2], points[count - 1], points[0], tension, out p1, out p2);
      pt[len_pt - 5] = p1;
      pt[len_pt - 4] = points[count - 1];
      pt[len_pt - 3] = p2;

      // calculation around first point of the original curve
      Calc_Curve_Bezier(points[count - 1], points[0], points[1], tension, out p1, out p2);
      pt[len_pt - 2] = p1;
      pt[len_pt - 1] = points[0]; // close path
      pt[0] = points[0];
      pt[1] = p2;

      return pt;
    }

    /// <summary>
    /// Shortens a Bezier segment and returns the Bezier points of the shortened segment.
    /// </summary>
    /// <param name="P1">Control point p1 of the bezier segment.</param>
    /// <param name="P2">Control point p2 of the bezier segment.</param>
    /// <param name="P3">Control point p3 of the bezier segment.</param>
    /// <param name="P4">Control point p4 of the bezier segment.</param>
    /// <param name="t0">Value in the range 0..1 to indicate the shortening at the beginning of the segment.</param>
    /// <param name="t1">Value in the range 0..1 to indicate the shortening at the beginning of the segment. This value must be greater than <paramref name="t0"/>.</param>
    /// <returns>The control points of the shortened Bezier segment.</returns>
    /// <remarks>
    /// <para>See this source <see href="http://stackoverflow.com/questions/11703283/cubic-bezier-curve-segment"/> for explanation.</para>
    /// <para>The assumtion here is that the Bezier curve is parametrized using</para>
    /// <para>B(t) = (1−t)³ P1 + 3(1−t)² t P2 + 3(1−t) t² P3 + t³ P4</para>
    /// </remarks>
    public static Tuple<PointD2D, PointD2D, PointD2D, PointD2D> ShortenBezierSegment(PointD2D P1, PointD2D P2, PointD2D P3, PointD2D P4, double t0, double t1)
    {
      // the assumption was that the curve was parametrized using
      // B(t) = (1−t)³ P1 + 3(1−t)² t P2 + 3(1−t) t² P3 + t³ P4

      var u0 = 1 - t0;
      var u1 = 1 - t1;

      var PS1 = u0 * u0 * u0 * P1 + (t0 * u0 * u0 + u0 * t0 * u0 + u0 * u0 * t0) * P2 + (t0 * t0 * u0 + u0 * t0 * t0 + t0 * u0 * t0) * P3 + t0 * t0 * t0 * P4;
      var PS2 = u0 * u0 * u1 * P1 + (t0 * u0 * u1 + u0 * t0 * u1 + u0 * u0 * t1) * P2 + (t0 * t0 * u1 + u0 * t0 * t1 + t0 * u0 * t1) * P3 + t0 * t0 * t1 * P4;
      var PS3 = u0 * u1 * u1 * P1 + (t0 * u1 * u1 + u0 * t1 * u1 + u0 * u1 * t1) * P2 + (t0 * t1 * u1 + u0 * t1 * t1 + t0 * u1 * t1) * P3 + t0 * t1 * t1 * P4;
      var PS4 = u1 * u1 * u1 * P1 + (t1 * u1 * u1 + u1 * t1 * u1 + u1 * u1 * t1) * P2 + (t1 * t1 * u1 + u1 * t1 * t1 + t1 * u1 * t1) * P3 + t1 * t1 * t1 * P4;

      return new Tuple<PointD2D, PointD2D, PointD2D, PointD2D>(PS1, PS2, PS3, PS4);
    }

    public static double FindDistanceFromStart(PointD2D[] points, double distanceFromStart)
    {
      var pivot = points[0];
      var dsqr = distanceFromStart * distanceFromStart;

      // Find a point (either curve point or control point that is farther away than required
      int i;
      for (i = 1; i < points.Length; ++i)
      {
        if ((points[i] - pivot).VectorLengthSquared >= dsqr)
          break;
      }

      if (i == points.Length)
        return double.NaN; // shortening results in no curve at all

      var segmentIndex = Math.Max(0, (i - 1) / 3);
      var segmentStart = segmentIndex * 3;
      if (pivot.DistanceSquaredTo(points[segmentStart + 3]) >= dsqr) // normal case: segment end point is further away than required
      {
      }

      return double.NaN;
    }

    public static PointD2D[]? ShortenBezierSegmentsByDistanceFromEndPoints(PointD2D[] points, double distanceFromStart, double distanceFromEnd)
    {
      return null;
    }
  }
}
