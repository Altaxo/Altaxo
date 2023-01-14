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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClipperLib;

namespace Altaxo.Geometry.Int64_2D
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


    /// <summary>
    /// Gets the angle between the lines pivot-a and pivot-b
    /// </summary>
    /// <param name="pivot">The pivot point.</param>
    /// <param name="a">The first point.</param>
    /// <param name="b">The second point.</param>
    /// <param name="returnPositiveValueIf180Degrees">If both lines are antiparallel (180 degrees), the angle is not unique (can be -Pi or +Pi).
    /// Per default a negative angle (-Pi) is returned, but
    /// if this parameter is set to true, then a positive angle (+Pi) will be returned.</param>
    /// <returns>The angle between the two lines.</returns>
    public static double GetAngle(IntPoint pivot, IntPoint a, IntPoint b, bool returnPositiveValueIf180Degrees)
    {
      var aX = (double)(a.X - pivot.X);
      var aY = (double)(a.Y - pivot.Y);
      var bX = (double)(b.X - pivot.X);
      var bY = (double)(b.Y - pivot.Y);
      var d1 = aX * bY - aY * bX;
      var d2 = aX * bX + aY * bY;

      if (d1 == 0 && d2 < 0 && returnPositiveValueIf180Degrees)
        return Math.PI;
      else
        return Math.Atan2(d1, d2);
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
    public static int SignOfCrossProduct(in Int64LineSegment a, in IntPoint b)
    {
      var aX = a.P1.X - a.P0.X;
      var aY = a.P1.Y - a.P0.Y;
      var bX = b.X - a.P0.X;
      var bY = b.Y - a.P0.Y;
      var d1 = Int128.Int128Mul(aX, bY);
      var d2 = Int128.Int128Mul(bX, aY);
      return d1 == d2 ? 0 : (d1 < d2 ? -1 : 1); // for non-integer we have to use some delta for comparison with 0
    }

    public static bool DoesPointTouchLine(in IntPoint b, in Int64LineSegment a)
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
    public static bool DoLinesIntersectOrTouch(in Int64LineSegment a, in Int64LineSegment b)
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



    #endregion




  }
}
