/*
 *
Adapted from: https://github.com/masphei/ConvexHull
The MIT License (MIT)
Copyright (c) 2013 masphei
Copyright (c) 2018 Dr. Dirk Lellinger (adapted to IntPoints, get rid of angle calculation)

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

*
*/

#nullable enable
using System;
using System.Collections.Generic;
using Clipper2Lib;

namespace Altaxo.Geometry.Int64_2D
{
  /// <summary>
  /// Calculation of the convex hull of a set of points by a Graham scan.
  /// </summary>
  public static class GrahamScan
  {
    /// <summary>
    /// Constant for a left turn.
    /// </summary>
    private const int TURN_LEFT = 1;
    /// <summary>
    /// Constant for a right turn.
    /// </summary>
    private const int TURN_RIGHT = -1;
    /// <summary>
    /// Constant for no turn.
    /// </summary>
    private const int TURN_NONE = 0;

    /// <summary>
    /// Determines the kind of turn formed by three points.
    /// </summary>
    /// <param name="p">First point.</param>
    /// <param name="q">Second point.</param>
    /// <param name="r">Third point.</param>
    /// <returns>TURN_LEFT, TURN_RIGHT, or TURN_NONE.</returns>
    private static int KindOfTurn(in Point64 p, in Point64 q, in Point64 r)
    {
      var d1 = (q.X - p.X) * (Int128)(r.Y - p.Y);
      var d2 = (r.X - p.X) * (Int128)(q.Y - p.Y);
      return d1 == d2 ? 0 : (d1 < d2 ? -1 : 1);
    }

    /// <summary>
    /// Keeps the hull left by removing points that do not form a left turn.
    /// </summary>
    /// <param name="hull">The current hull.</param>
    /// <param name="r">The candidate point.</param>
    private static void KeepLeft(List<(Point64 point, int index)> hull, in (Point64 point, int index) r)
    {
      while (hull.Count > 1 && KindOfTurn(hull[hull.Count - 2].point, hull[hull.Count - 1].point, r.point) != TURN_LEFT)
      {
        hull.RemoveAt(hull.Count - 1);
      }
      if (hull.Count == 0 || hull[hull.Count - 1].index != r.index)
      {
        hull.Add(r);
      }
    }

    /// <summary>
    /// Sorts points by turn direction using merge sort.
    /// </summary>
    /// <param name="p0">Reference point.</param>
    /// <param name="arrPoint">Array of points to sort.</param>
    /// <returns>Sorted list of points.</returns>
    private static List<(Point64 point, int index)> MergeSort(in Point64 p0, List<(Point64 point, int index)> arrPoint)
    {
      if (arrPoint.Count == 1)
      {
        return arrPoint;
      }
      var arrSortedInt = new List<(Point64 point, int index)>();
      var middle = arrPoint.Count / 2;
      var leftArray = arrPoint.GetRange(0, middle);
      var rightArray = arrPoint.GetRange(middle, arrPoint.Count - middle);
      leftArray = MergeSort(p0, leftArray);
      rightArray = MergeSort(p0, rightArray);
      var leftptr = 0;
      var rightptr = 0;
      for (var i = 0; i < leftArray.Count + rightArray.Count; i++)
      {
        if (leftptr == leftArray.Count)
        {
          arrSortedInt.Add(rightArray[rightptr]);
          rightptr++;
        }
        else if (rightptr == rightArray.Count)
        {
          arrSortedInt.Add(leftArray[leftptr]);
          leftptr++;
        }
        // else if (GetAngle(p0, leftArray[leftptr].point) < GetAngle(p0, rightArray[rightptr].point))
        else if (TURN_LEFT == KindOfTurn(p0, leftArray[leftptr].point, rightArray[rightptr].point))
        {
          arrSortedInt.Add(leftArray[leftptr]);
          leftptr++;
        }
        else
        {
          arrSortedInt.Add(rightArray[rightptr]);
          rightptr++;
        }
      }
      return arrSortedInt;
    }

    /// <summary>
    /// Gets the convex hull of a set of points.
    /// The returned points form a polygon in ccw direction (in a coordinate system in which y runs to the top, x runs to the right).
    /// </summary>
    /// <param name="points">The set of points for which to calculate the convex hull.</param>
    /// <returns>The ordered set of points that forms the hull.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="points"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the number of points is less than 3.</exception>
    public static IReadOnlyList<(Point64 point, int index)> GetConvexHull(IReadOnlyList<Point64> points)
    {
      if (points is null)
      {
        throw new ArgumentNullException(nameof(points));
      }

      if (!(points.Count >= 3))
      {
        throw new ArgumentException("Points list must at least contain 3 points", nameof(points));
      }

      var index0 = 0;
      var point0 = points[index0];
      for (var i = 1; i < points.Count; ++i)
      {
        if (point0.Y > points[i].Y)
        {
          index0 = i;
          point0 = points[i];
        }
      }



      var order = new List<(Point64 point, int index)>();
      for (var i = 0; i < points.Count; ++i)
      {
        if (index0 != i)
        {
          order.Add((points[i], i));
        }
      }

      order = MergeSort(point0, order);

      var result = new List<(Point64 point, int index)>
      {
        (point0, index0),
        order[0],
        order[1]
      };
      order.RemoveAt(0);
      order.RemoveAt(0);

      foreach (var value in order)
      {
        KeepLeft(result, value);
      }
      return result;
    }
  }
}
