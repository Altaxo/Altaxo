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

namespace Altaxo.Geometry.PolygonHull.Int64
{
  /// <summary>
  /// Mathematics for closed polygons.
  /// </summary>
  public static class PolygonMath
  {
    #region Closed Polygons

    /// <summary>
    /// Gets the circumference of a closed polygon.
    /// </summary>
    /// <param name="closedPolygon">The closed polygon.</param>
    /// <returns>The circumference of the given closed polygon.</returns>
    public static double GetClosedPolygonCircumference(IEnumerable<IntPoint> closedPolygon)
    {
      IntPoint firstPoint = default;
      IntPoint previous = default;

      double sum = 0.0;
      var numberOfPoints = 0;
      foreach (var pt in closedPolygon)
      {
        if (0 == numberOfPoints)
        {
          firstPoint = pt;
        }
        else
        {
          sum += Math.Sqrt((double)(Int128.Int128Mul(pt.X - previous.X, pt.X - previous.X) + Int128.Int128Mul(pt.Y - previous.Y, pt.Y - previous.Y)));
        }

        previous = pt;
        ++numberOfPoints;
      }

      if (numberOfPoints == 0)
      {
        throw new ArgumentException("Polygon is empty (has no point at all)", nameof(closedPolygon));
      }
      else
      {
        var pt = firstPoint;
        sum += Math.Sqrt((double)(Int128.Int128Mul(pt.X - previous.X, pt.X - previous.X) + Int128.Int128Mul(pt.Y - previous.Y, pt.Y - previous.Y)));
        return sum;
      }
    }

    /// <summary>
    /// Gets the area of a closed polygon.
    /// </summary>
    /// <param name="closedPolygon">The points forming a closed polygon.</param>
    /// <returns>The polygon area. The value is signed. The sign is positive if the polygon is counter-clockwise (in a coordinate system in which x is to the right and y is up).</returns>
    public static double GetClosedPolygonArea(IEnumerable<IntPoint> closedPolygon)
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
    public static PointD2D GetClosedPolygonCentroid(IEnumerable<IntPoint> closedPolygon)
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

    /// <summary>
    /// Calculates the 2nd moments of the given closed polygons. The polygons
    /// have to be non-selfintersecting!
    /// </summary>
    /// <param name="closedPolygons">The closed polygons.</param>
    /// <returns>The second moments, Ix, Iy, and Ixy, of the given polygons (with respect to the origin (y=0, y=0)).</returns>
    /// <seealso href="https://en.wikipedia.org/wiki/Second_moment_of_area"/>
    public static (double Ix, double Iy, double Ixy) GetClosedPolygonSecondMoments(IEnumerable<IEnumerable<IntPoint>> closedPolygons)
    {
      double sumX = 0, sumY = 0, sumXY = 0;

      foreach (var polygon in closedPolygons)
      {
        var (x, y, xy) = GetClosedPolygonSecondMoments(polygon);
        sumX += x;
        sumY += y;
        sumXY += xy;
      }
      return (sumX, sumY, sumXY);
    }

    /// <summary>
    /// Calculates the 2nd moments of the given closed polygon. The polygon
    /// has to be non-selfintersecting!
    /// </summary>
    /// <param name="closedPolygon">The closed polygon.</param>
    /// <returns>The second moments, Ix, Iy, and Ixy, of the given polygon.</returns>
    /// <seealso href="https://en.wikipedia.org/wiki/Second_moment_of_area"/>
    public static (double Ix, double Iy, double Ixy) GetClosedPolygonSecondMoments(IEnumerable<IntPoint> closedPolygon)
    {
      IntPoint firstPoint = default;
      IntPoint previous = default;
      var numberOfPoints = 0;

      double sumX = 0;
      double sumY = 0;
      double sumXY = 0;
      Int128 s;
      foreach (var pt in closedPolygon)
      {
        if (numberOfPoints == 0)
        {
          firstPoint = pt;
        }
        else
        {
          s = (Int128.Int128Mul(previous.X, pt.Y) - Int128.Int128Mul(previous.Y, pt.X));
          sumX += (Pow2(previous.Y) + previous.Y * pt.Y + Pow2(pt.Y)) * (double)s;
          sumY += (Pow2(previous.X) + previous.X * pt.X + Pow2(pt.X)) * (double)s;
          sumXY += (previous.X * pt.Y + 2 * previous.X * previous.Y + 2 * pt.X * pt.Y + pt.X * previous.Y) * (double)s;
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
        return (0, 0, 0);
      }
      else if (numberOfPoints == 2)
      {
        return (0, 0, 0);
      }
      else
      {
        s = (Int128.Int128Mul(previous.X, firstPoint.Y) - Int128.Int128Mul(previous.Y, firstPoint.X));
        var pt = firstPoint;
        sumX += (Pow2(previous.Y) + previous.Y * pt.Y + Pow2(pt.Y)) * (double)s;
        sumY += (Pow2(previous.X) + previous.X * pt.X + Pow2(pt.X)) * (double)s;
        sumXY += (previous.X * pt.Y + 2 * previous.X * previous.Y + 2 * pt.X * pt.Y + pt.X * previous.Y) * (double)s;

        sumX /= 6;
        sumY /= 6;
        sumXY /= 12;

        return (sumX, sumY, sumXY);
      }
    }

    #endregion Closed Polygons

    private static double Pow2(double x) => x * x;
  }
}
