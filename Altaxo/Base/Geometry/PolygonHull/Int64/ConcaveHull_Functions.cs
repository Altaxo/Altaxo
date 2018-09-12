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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClipperLib;

/// <summary>
/// 
/// </summary>
namespace Altaxo.Geometry.PolygonHull.Int64
{
  public readonly struct Int64LineSegment
  {
    public readonly IntPoint P0;
    public readonly IntPoint P1;

    public Int64LineSegment(IntPoint p0, IntPoint p1)
    {
      P0 = p0;
      P1 = p1;
    }

    public static double GetDistance(IntPoint p0, IntPoint p1)
    {
      var dx = (double)(p1.X - p0.X);
      var dy = (double)(p1.Y - p0.Y);
      return Math.Sqrt(dx * dx + dy * dy);
    }

    /// <summary>
    /// Gets the angle between the lines pivot-a and pivot-b
    /// </summary>
    /// <param name="pivot">The pivot.</param>
    /// <param name="a">a.</param>
    /// <param name="b">The b.</param>
    /// <returns></returns>
    public static double GetCosOfAngle(IntPoint pivot, IntPoint a, IntPoint b)
    {
      var rX = (double)(a.X - pivot.X);
      var rY = (double)(a.Y - pivot.Y);
      var sX = (double)(b.X - pivot.X);
      var sY = (double)(b.Y - pivot.Y);
      var z = (rX * sX + rY * sY) / Math.Sqrt((rX * rX + rY * rY) * (sX * sX + sY * sY));
      return z;
    }
  }




  public partial class ConcaveHull
  {
    #region Line Intersection

    // see https://martin-thoma.com/how-to-check-if-two-line-segments-intersect/ for details

    /// <summary>
    /// Determines whether a point is right, left or on the (infinite long!) line that is defined by the provided line segment.
    /// </summary>
    /// <param name="a">The line segment.</param>
    /// <param name="b">The point to test.</param>
    /// <returns>0 if the point is on, -1 if it is right, and +1 if it is left on/to the infinite long line that is defined by the line segment.</returns>
    public static int SignOfCrossProduct(Int64LineSegment a, IntPoint b)
    {
      var aX = a.P1.X - a.P0.X;
      var aY = a.P1.Y - a.P0.Y;
      var bX = b.X - a.P0.X;
      var bY = b.Y - a.P0.Y;
      var d1 = Int128.Int128Mul(aX, bY);
      var d2 = Int128.Int128Mul(bX, aY);
      return d1 == d2 ? 0 : (d1 < d2 ? -1 : 1); // for non-integer we have to use some delta for comparison with 0
    }

    public static bool DoesPointTouchLine(IntPoint b, Int64LineSegment a)
    {
      if (
           Math.Min(a.P0.X, a.P1.X) > b.X // then a is right of b
        || Math.Max(a.P0.X, a.P1.X) < b.X // then a is left of b
        || Math.Min(a.P0.Y, a.P1.Y) > b.Y // then a is to the top of b
        || Math.Max(a.P0.Y, a.P1.Y) < b.Y // then a is to the bottom of b
        )
      {
        return false; // Bounding box of the line segment does not intersect with point
      }

      return SignOfCrossProduct(a, b) == 0;
    }


    /// <summary>
    /// Test if two line segments touch each other or intersect.
    /// </summary>
    /// <param name="a">Line segment a.</param>
    /// <param name="b">Line segment b.</param>
    /// <returns>True if the two line segments touch each other or intersect; otherwise, false.</returns>
    public static bool DoLinesIntersectOrTouch(Int64LineSegment a, Int64LineSegment b)
    {
      if (
           Math.Min(a.P0.X, a.P1.X) > Math.Max(b.P0.X, b.P1.X) // then a is right of b
        || Math.Max(a.P0.X, a.P1.X) < Math.Min(b.P0.X, b.P1.X) // then a is left of b
        || Math.Min(a.P0.Y, a.P1.Y) > Math.Max(b.P0.Y, b.P1.Y) // then a is to the top of b
        || Math.Max(a.P0.Y, a.P1.Y) < Math.Min(b.P0.Y, b.P1.Y) // then a is to the bottom of b
        )
      {
        return false; // Bounding boxes of the two line segments do not intersect
      }

      int s0, s1;

      var b_touchorcross_a =
           0 == (s0 = SignOfCrossProduct(a, b.P0)) // b.P0 is on infinite line a
        || 0 == (s1 = SignOfCrossProduct(a, b.P1)) // b.P1 is on infinite line a
        || ((s0 < 0) ^ (s1 < 0)); // b crosses infinite line a

      var a_touchorcross_b =
           0 == (s0 = SignOfCrossProduct(b, a.P0)) // b.P0 is on infinite line a
        || 0 == (s1 = SignOfCrossProduct(b, a.P1)) // b.P1 is on infinite line a
        || ((s0 < 0) ^ (s1 < 0)); // a crosses infinite line b

      return a_touchorcross_b && b_touchorcross_a;
    }

    private static double Pow2(double x) => x * x;

    /// <summary>
    /// Gets the polygon area.
    /// </summary>
    /// <param name="closedPolygon">The points forming a closed polygon.</param>
    /// <returns>The polygon area.</returns>
    public static double GetPolygonArea(IEnumerable<IntPoint> closedPolygon)
    {
      IntPoint firstPoint = default;
      IntPoint previous = default;

      var sum = new Int128(0);
      var numberOfPoints = 0;
      foreach (var pt in closedPolygon)
      {
        if (0 == numberOfPoints)
        {
          firstPoint = pt;
        }
        else
        {
          sum += (Int128.Int128Mul(previous.X, pt.Y) - Int128.Int128Mul(previous.Y, pt.X));
        }

        previous = pt;
        ++numberOfPoints;
      }

      if (numberOfPoints == 0)
      {
        throw new ArgumentException("Polygon is empty (has no point at all)", nameof(closedPolygon));
      }
      else if (numberOfPoints == 1)
      {
        return 0;
      }
      else if (numberOfPoints == 2)
      {
        return 0;
      }
      else
      {
        sum += (Int128.Int128Mul(previous.X, firstPoint.Y) - Int128.Int128Mul(previous.Y, firstPoint.X));
        return (double)sum / 2.0;
      }
    }

    /// <summary>
    /// Calculate the center of gravity (centroid) of the given closed polygon. The polygon
    /// has to be non-selfintersecting!
    /// </summary>
    /// <param name="closedPolygon">The closed polygon.</param>
    /// <returns>The center of gravity of the given polygon.</returns>
    /// <seealso href="https://en.wikipedia.org/wiki/Centroid"/>
    public static PointD2D GetPolygonCentroid(IEnumerable<IntPoint> closedPolygon)
    {
      IntPoint firstPoint = default;
      IntPoint previous = default;
      var numberOfPoints = 0;

      double sumX = 0;
      double sumY = 0;
      Int128 s;
      var sumS = new Int128(0);
      foreach (var pt in closedPolygon)
      {
        if (numberOfPoints == 0)
        {
          firstPoint = pt;
        }
        else
        {
          s = (Int128.Int128Mul(previous.X, pt.Y) - Int128.Int128Mul(previous.Y, pt.X));
          sumS += s;
          sumX += (previous.X + pt.X) * (double)s;
          sumY += (previous.Y + pt.Y) * (double)s;
        }


        previous = pt;
        ++numberOfPoints;
      }

      if (numberOfPoints == 0)
      {
        throw new ArgumentException("Polygon is empty (has no point at all)", nameof(closedPolygon));
      }
      else if (numberOfPoints == 1)
      {
        return new PointD2D(firstPoint.X, firstPoint.Y);
      }
      else if (numberOfPoints == 2)
      {
        return new PointD2D((firstPoint.X + previous.X) / 2, (firstPoint.Y + previous.Y) / 2);
      }
      else
      {
        s = (Int128.Int128Mul(previous.X, firstPoint.Y) - Int128.Int128Mul(previous.Y, firstPoint.X));
        sumS += s;
        sumX += (previous.X + firstPoint.X) * (double)s;
        sumY += (previous.Y + firstPoint.Y) * (double)s;

        sumX /= (3 * (double)sumS);
        sumY /= (3 * (double)sumS);

        return new PointD2D(sumX, sumY);
      }
    }

    #endregion




  }
}
