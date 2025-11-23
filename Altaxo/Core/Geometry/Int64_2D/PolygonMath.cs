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
using Clipper2Lib;

namespace Altaxo.Geometry.Int64_2D
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
    public static double GetClosedPolygonCircumference(IEnumerable<Point64> closedPolygon)
    {
      Point64 firstPoint = default;
      Point64 previous = default;

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
          sum += Math.Sqrt((double)((pt.X - previous.X) * (Int128)(pt.X - previous.X) + (pt.Y - previous.Y) * (Int128)(pt.Y - previous.Y)));
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
        sum += Math.Sqrt((double)((pt.X - previous.X) * (Int128)(pt.X - previous.X) + (pt.Y - previous.Y) * (Int128)(pt.Y - previous.Y)));
        return sum;
      }
    }

    /// <summary>
    /// Gets the area of a closed polygon.
    /// </summary>
    /// <param name="closedPolygon">The points forming a closed polygon.</param>
    /// <returns>The polygon area. The value is signed. The sign is positive if the polygon is counter-clockwise (in a coordinate system in which x is to the right and y is up).</returns>
    public static double GetClosedPolygonArea(IEnumerable<Point64> closedPolygon)
    {
      Point64 firstPoint = default;
      Point64 previous = default;

      Int128 sum = 0;
      var numberOfPoints = 0;
      foreach (var pt in closedPolygon)
      {
        if (0 == numberOfPoints)
        {
          firstPoint = pt;
        }
        else
        {
          sum += (previous.X * (Int128)pt.Y) - (previous.Y * (Int128)pt.X);
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
        sum += (previous.X * (Int128)firstPoint.Y) - (previous.Y * (Int128)firstPoint.X);
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
    public static PointD2D GetClosedPolygonCentroid(IEnumerable<Point64> closedPolygon)
    {
      Point64 firstPoint = default;
      Point64 previous = default;
      var numberOfPoints = 0;

      double sumX = 0;
      double sumY = 0;
      Int128 s;
      Int128 sumS = 0;
      foreach (var pt in closedPolygon)
      {
        if (numberOfPoints == 0)
        {
          firstPoint = pt;
        }
        else
        {
          s = (previous.X * (Int128)pt.Y) - (previous.Y * (Int128)pt.X);
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
        s = (previous.X * (Int128)firstPoint.Y) - (previous.Y * (Int128)firstPoint.X);
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
    /// <returns>The second moments, Ixx, Iyy, and Ixy, of the given polygons (with respect to the origin (x=0, y=0)).</returns>
    /// <seealso href="https://en.wikipedia.org/wiki/Second_moment_of_area"/>
    public static (double Ixx, double Iyy, double Ixy) GetClosedPolygonSecondMoments(IEnumerable<IEnumerable<Point64>> closedPolygons)
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
    /// <returns>The second moments, Ixx, Iyy, and Ixy, of the given polygon (with respect to the origin (x=0, y=0)).</returns>
    /// <seealso href="https://en.wikipedia.org/wiki/Second_moment_of_area"/>
    public static (double Ixx, double Iyy, double Ixy) GetClosedPolygonSecondMoments(IEnumerable<Point64> closedPolygon)
    {
      Point64 firstPoint = default;
      Point64 previous = default;
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
          s = (previous.X * (Int128)pt.Y) - (previous.Y * (Int128)pt.X);
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
        s = (previous.X * (Int128)firstPoint.Y) - (previous.Y * (Int128)firstPoint.X);
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

    /// <summary>
    /// Gets the orientation angle (in rad) of the principal axis from the second moments.
    /// </summary>
    /// <param name="Ixx">The second moment Ixx.</param>
    /// <param name="Iyy">The second moment Iyy.</param>
    /// <param name="Ixy">The second moment Ixy.</param>
    /// <returns>The orientation angle of the principal axis in rad.</returns>
    public static double GetOrientationAngleOfPrincipalAxisFromSecondMoments(double Ixx, double Iyy, double Ixy)
    {
      return 0.5 * Math.Atan2(2 * Ixy, Iyy - Ixx);
    }

    /// <summary>
    /// Gets the eigenvectors from the second moments.
    /// </summary>
    /// <param name="Ixx">The second moment Ixx.</param>
    /// <param name="Iyy">The second moment Iyy.</param>
    /// <param name="Ixy">The second moment Ixy.</param>
    /// <returns>The Eigenvectors corresponding to the provided second moments (Eigenvector for the largest EigenValue is first).</returns>
    public static (VectorD2D EigenVector1, VectorD2D EigenVector2) GetEigenVectorsFromSecondMoments(double Ixx, double Iyy, double Ixy)
    {
      var root = Math.Sqrt(4 * Pow2(Ixy) + Pow2(Ixx - Iyy));
      var t1 = (Iyy - Ixx - root) / (2 * Ixy);
      var t2 = (Iyy - Ixx + root) / (2 * Ixy);
      return (new VectorD2D(t1, 1), new VectorD2D(t2, 1));
    }

    /// <summary>
    /// Gets the eigenvalues from the second moments.
    /// </summary>
    /// <param name="Ixx">The second moment Ixx.</param>
    /// <param name="Iyy">The second moment Iyy.</param>
    /// <param name="Ixy">The second moment Ixy.</param>
    /// <returns>The Eigenvalues corresponding to the second moments (largest Eigenvalue is the first element).</returns>
    public static (double EigenValue1, double EigenValue2) GetEigenValuesFromSecondMoments(double Ixx, double Iyy, double Ixy)
    {
      var root = Math.Sqrt(4 * Pow2(Ixy) + Pow2(Ixx - Iyy));
      var t1 = 0.5 * (Ixx + Iyy + root);
      var t2 = 0.5 * (Ixx + Iyy - root);
      return (t1, t2);
    }

    #endregion Closed Polygons

    /// <summary>
    /// Creates integer polygon points from points consisting of doubles.
    /// The points can be transformed before converting to integers. First, the points are centered (x'=x-centerX, y'=y-centerY). Then the centered point
    /// is rotated. Lastly, the such transformated points is scaled.
    /// </summary>
    /// <param name="points">The original points.</param>
    /// <param name="centerX">The x coordinate of the center. (first transformation x' = x - centerX).</param>
    /// <param name="centerY">The y coordinate of the center. (first transformation y' = y - centerY).</param>
    /// <param name="rotation_rad">The rotation in rad (second transformation x'' = Cos(phi) * x' - Sin(phi) * y', y'' = Sin(phi) * x' + Cos(phi) * y').</param>
    /// <param name="scale">The scale (third transformation x''' = scale * x'', y''' = scale * y'').</param>
    /// <returns>The centered, rotated and scaled points, rounded to integers.</returns>
    public static IEnumerable<Point64> ToIntPoints(this IEnumerable<Altaxo.Geometry.PointD2D> points, double centerX = 0, double centerY = 0, double rotation_rad = 0, double scale = 1)
    {
      double cos = 1, sin = 0;
      if (0 != rotation_rad)
      {
        cos = Math.Cos(rotation_rad);
        sin = Math.Sin(rotation_rad);
      }

      foreach (var pt in points)
      {
        var x = pt.X - centerX;
        var y = pt.Y - centerY;

        if (0 != rotation_rad)
        {
          var xx = x;
          x = x * cos - y * sin;
          y = xx * sin + y * cos;
        }
        yield return new Point64((long)Math.Round(x * scale), (long)Math.Round(y * scale));
      }
    }

    /// <summary>
    /// Creates integer polygon points from points consisting of doubles.
    /// The points can be transformed before converting to integers. First, the points are centered (x'=x-centerX, y'=y-centerY). Then the centered point
    /// is rotated. Lastly, the such transformated points is scaled.
    /// </summary>
    /// <param name="points">The original points.</param>
    /// <param name="centerX">The x coordinate of the center. (first transformation x' = x - centerX).</param>
    /// <param name="centerY">The y coordinate of the center. (first transformation y' = y - centerY).</param>
    /// <param name="rotation_rad">The rotation in rad (second transformation x'' = Cos(phi) * x' - Sin(phi) * y', y'' = Sin(phi) * x' + Cos(phi) * y').</param>
    /// <param name="scale">The scale (third transformation x''' = scale * x'', y''' = scale * y'').</param>
    /// <returns>The centered, rotated and scaled points, rounded to integers.</returns>
    public static IEnumerable<Point64> ToIntPoints(this IEnumerable<(double X, double Y)> points, double centerX = 0, double centerY = 0, double rotation_rad = 0, double scale = 1)
    {
      double cos = 1, sin = 0;
      if (0 != rotation_rad)
      {
        cos = Math.Cos(rotation_rad);
        sin = Math.Sin(rotation_rad);
      }

      foreach (var pt in points)
      {
        var x = pt.X - centerX;
        var y = pt.Y - centerY;

        if (0 != rotation_rad)
        {
          var xx = x;
          x = x * cos - y * sin;
          y = xx * sin + y * cos;
        }
        yield return new Point64((long)Math.Round(x * scale), (long)Math.Round(y * scale));
      }
    }

    private static double Pow2(double x) => x * x;
  }
}
