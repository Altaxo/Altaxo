#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

namespace Altaxo.Geometry
{
  /// <summary>
  /// Drawing2DRelated contains static methods related to mathematics and helper
  /// functions for classed from the System.Drawing namespace.
  /// </summary>
  public static class Math2D
  {
    /// <summary>
    /// Calculates the distance between two points.
    /// </summary>
    /// <param name="p1">First point.</param>
    /// <param name="p2">Second point.</param>
    /// <returns>The distance between points p1 and p2.</returns>
    public static double Distance(PointD2D p1, PointD2D p2)
    {
      double x = p1.X - p2.X;
      double y = p1.Y - p2.Y;
      return Math.Sqrt(x * x + y * y);
    }

    /// <summary>
    /// Calculates the squared distance between a finite line and a point.
    /// </summary>
    /// <param name="point">The location of the point.</param>
    /// <param name="lineOrg">The location of the line origin.</param>
    /// <param name="lineEnd">The location of the line end.</param>
    /// <returns>The squared distance between the line (threated as having a finite length) and the point.</returns>
    public static double SquareDistanceLineToPoint(PointD2D point, PointD2D lineOrg, PointD2D lineEnd)
    {
      var linex = lineEnd.X - lineOrg.X;
      var liney = lineEnd.Y - lineOrg.Y;
      var pointx = point.X - lineOrg.X;
      var pointy = point.Y - lineOrg.Y;

      var rsquare = linex * linex + liney * liney;
      var xx = linex * pointx + liney * pointy;
      if (xx <= 0) // the point is located before the line, so use
      {         // the distance of the line origin to the point
        return pointx * pointx + pointy * pointy;
      }
      else if (xx >= rsquare) // the point is located after the line, so use
      {                   // the distance of the line end to the point
        pointx = point.X - lineEnd.X;
        pointy = point.Y - lineEnd.Y;
        return pointx * pointx + pointy * pointy;
      }
      else // the point is located in the middle of the line, use the
      {     // distance from the line to the point
        var yy = liney * pointx - linex * pointy;
        return yy * yy / rsquare;
      }
    }

    /// <summary>
    /// Determines whether or not a given point (<c>point</c>) is into a <c>distance</c> to a finite line, that is spanned between
    /// two points <c>lineOrg</c> and <c>lineEnd</c>.
    /// </summary>
    /// <param name="point">Point under test.</param>
    /// <param name="distance">Distance.</param>
    /// <param name="lineOrg">Starting point of the line.</param>
    /// <param name="lineEnd">End point of the line.</param>
    /// <returns>True if the distance between point <c>point</c> and the line between <c>lineOrg</c> and <c>lineEnd</c> is less or equal to <c>distance</c>.</returns>
    public static bool IsPointIntoDistance(PointD2D point, double distance, PointD2D lineOrg, PointD2D lineEnd)
    {
      // first a quick test if the point is far outside the circle
      // that is spanned from the middle of the line and has at least
      // a radius of half of the line length plus the distance
      var xm = (lineOrg.X + lineEnd.X) / 2;
      var ym = (lineOrg.Y + lineEnd.Y) / 2;
      var r = Math.Abs(lineOrg.X - xm) + Math.Abs(lineOrg.Y - ym) + distance;
      if (Math.Max(Math.Abs(point.X - xm), Math.Abs(point.Y - ym)) > r)
        return false;
      else
        return SquareDistanceLineToPoint(point, lineOrg, lineEnd) <= distance * distance;
    }

    /// <summary>
    /// Determines whether or not a given point is into a certain distance of a polyline.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="distance">The distance.</param>
    /// <param name="polyline">The polyline.</param>
    /// <returns>
    ///   <c>true</c> if the distance  between point and polyline is less than or equal to the specified distance; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsPointIntoDistance(PointD2D point, double distance, IEnumerable<PointD2D> polyline)
    {
      using (var iterator = polyline.GetEnumerator())
      {
        if (!iterator.MoveNext())
          return false; // no points in polyline
        var prevPoint = iterator.Current;

        while (iterator.MoveNext())
        {
          if (IsPointIntoDistance(point, distance, iterator.Current, prevPoint))
            return true;
          prevPoint = iterator.Current;
        }
      }
      return false;
    }

    /// <summary>
    /// Calculates the area of a closed polygons. The polygon points are given in <paramref name="points"/>.
    /// The result is positive if, in a cartesic coordinate system with x to the right and y to the top, the polygon points are circulating counterclockwise around the enclosed area.
    /// Otherwise, the result is negative.
    /// </summary>
    /// <param name="points">The polygon points. There is no need to repeat the first point at the end of the enumeration.</param>
    /// <returns>The polygon area (in a cartesic coordinate system: positive for a counterclockwise oriented polygon, negative for a clockwise oriented polygon)</returns>
    public static double PolygonArea(IEnumerable<PointD2D> points)
    {
      bool isFirst = true;
      var firstPoint = default(PointD2D);
      var prevPoint = default(PointD2D);
      double area = 0;
      foreach (var point in points)
      {
        if (isFirst)
        {
          firstPoint = prevPoint = point;
          isFirst = false;
          continue;
        }
        // we substract the first point from all points in order to improve accuracy of calculation
        area += (prevPoint.X - firstPoint.X) * (point.Y - firstPoint.Y) - (prevPoint.Y - firstPoint.Y) * (point.X - firstPoint.X);
        prevPoint = point;
      }
      return 0.5 * area;
    }

    /// <summary>
    /// A very general flood fill algorithm (4-Neighbour algorithm).
    /// It tests iteratively the 4 neighbouring pixels.
    /// </summary>
    /// <param name="xStart">The start pixel's x coordinate.</param>
    /// <param name="yStart">The start pixel's x coordinate.</param>
    /// <param name="IsPixelToBeFilled">
    /// Function that evaluated if the pixel fulfills a condition so that it should be set to a new value.
    /// Args are the x and y pixel coordinates,
    /// the return value is true if the pixel should be set to a new value.
    /// </param>
    /// <param name="SetPixelToNewValue">
    /// Action to set the pixel to a new value.
    /// Args are the x and y coordinates of the pixel.
    /// Since the pixel's value is never tested again, there is no need to really set the pixel's value.
    /// Thus, this action can also be used to count the pixels that fulfill the condition given in <paramref name="IsPixelToBeFilled"/>, etc.
    /// </param>
    /// <param name="xLower">The lowest possible value for a pixel's x coordinate.</param>
    /// <param name="yLower">The lowest possible value for a pixel's y coordinate</param>
    /// <param name="xSize">The x size. The x coordinate can take values from <paramref name="xStart"/> to <paramref name="xStart"/>+<paramref name="xSize"/>-1.</param>
    /// <param name="ySize">The y size. The y coordinate can take values from <paramref name="yStart"/> to <paramref name="yStart"/>+<paramref name="ySize"/>-1.</param>
    public static void FloodFill_4Neighbour(int xStart, int yStart, Func<int, int, bool> IsPixelToBeFilled, Action<int, int> SetPixelToNewValue, int xLower, int yLower, int xSize, int ySize)
    {
      int xUpper = xLower + xSize;
      int yUpper = yLower + ySize;

      // Test arguments
      if (null == IsPixelToBeFilled)
        throw new ArgumentNullException(nameof(IsPixelToBeFilled));
      if (null == SetPixelToNewValue)
        throw new ArgumentNullException(nameof(SetPixelToNewValue));
      if (!(xLower < xUpper))
        throw new ArgumentException("xSize should be >0", nameof(xSize));
      if (!(yLower < yUpper))
        throw new ArgumentException("ySize should be >0", nameof(ySize));
      if (!(xLower <= xStart && xStart < xUpper))
        throw new ArgumentException("xStart should be >= xLower and < xLower+xSize", nameof(xStart));
      if (!(yLower <= yStart && yStart < yUpper))
        throw new ArgumentException("yStart should be >= yLower and < yLower+ySize", nameof(xStart));

      FloodFill_4Neighbour(
        xStart, yStart,
        IsPixelToBeFilled,
        SetPixelToNewValue,
        (x, y) => (x >= xLower && y >= yLower && x < xUpper && y < yUpper)
        );
    }

    /// <summary>
    /// A very general flood fill algorithm (4-Neighbour algorithm).
    /// It tests iteratively the 4 neighbouring pixels.
    /// </summary>
    /// <param name="IsPixelToBeFilled">
    /// Function that evaluated if the pixel fulfills a condition so that it should be set to a new value.
    /// Args are the x and y pixel coordinates,
    /// the return value is true if the pixel should be set to a new value.
    /// </param>
    /// <param name="SetPixelToNewValue">
    /// Action to set the pixel to a new value.
    /// Args are the x and y coordinates of the pixel.
    /// Since the pixel's value is never tested again, there is no need to really set the pixel's value.
    /// Thus, this action can also be used to count the pixels that fulfill the condition given in <paramref name="IsPixelToBeFilled"/>, etc.
    /// </param>
    /// <param name="xStart">The start pixel's x coordinate.</param>
    /// <param name="yStart">The start pixel's x coordinate.</param>
    /// <param name="IsPixelCoordinateValid">Tests the pixel coordinate to find out if it is in a valid range. Params are x, y of the pixel coordinate. The return value is true if the coordinate is valid, i.e. the pixel should be processed; otherwise, it is false.</param>
    public static void FloodFill_4Neighbour(int xStart, int yStart, Func<int, int, bool> IsPixelToBeFilled, Action<int, int> SetPixelToNewValue, Func<int, int, bool> IsPixelCoordinateValid)
    {
      // Test arguments
      if (null == IsPixelToBeFilled)
        throw new ArgumentNullException(nameof(IsPixelToBeFilled));
      if (null == SetPixelToNewValue)
        throw new ArgumentNullException(nameof(SetPixelToNewValue));
      if (null == IsPixelCoordinateValid)
        throw new ArgumentNullException(nameof(IsPixelCoordinateValid));
      if (!IsPixelCoordinateValid(xStart, yStart))
        throw new ArgumentException("Coordinates of the start pixel are not valid!");

      var pixelsConsidered = new HashSet<(int, int)>();
      var pixelsToProcess = new Stack<(int, int)>();

      pixelsConsidered.Add((xStart, yStart));
      pixelsToProcess.Push((xStart, yStart));

      (int x, int y) newPixel = (0, 0);
      while (pixelsToProcess.Count > 0)
      {
        var (x, y) = pixelsToProcess.Pop();
        if (IsPixelToBeFilled(x, y))
        {
          SetPixelToNewValue(x, y);

          newPixel = (x, y + 1);
          if (IsPixelCoordinateValid(newPixel.x, newPixel.y) && !pixelsConsidered.Contains(newPixel))
          {
            pixelsToProcess.Push(newPixel);
            pixelsConsidered.Add(newPixel);
          }

          newPixel = (x, y - 1);
          if (IsPixelCoordinateValid(newPixel.x, newPixel.y) && !pixelsConsidered.Contains(newPixel))
          {
            pixelsToProcess.Push(newPixel);
            pixelsConsidered.Add(newPixel);
          }

          newPixel = (x + 1, y);
          if (IsPixelCoordinateValid(newPixel.x, newPixel.y) && !pixelsConsidered.Contains(newPixel))
          {
            pixelsToProcess.Push(newPixel);
            pixelsConsidered.Add(newPixel);
          }

          newPixel = (x - 1, y);
          if (IsPixelCoordinateValid(newPixel.x, newPixel.y) && !pixelsConsidered.Contains(newPixel))
          {
            pixelsToProcess.Push(newPixel);
            pixelsConsidered.Add(newPixel);
          }
        }
      }
    }
  }
}
