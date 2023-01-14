/*
 *
Adapted from: https://github.com/masphei/ConvexHull
The MIT License (MIT)
Copyright (c) 2013 masphei
Copyright (c) 2018 Dr. Dirk Lellinger

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

namespace Altaxo.Geometry.Double_2D
{
  /// <summary>
  /// Calculation of the convex hull of a set of points by a Graham scan.
  /// </summary>
  public static class GrahamScan
  {
    private const int TURN_LEFT = 1;
    private const int TURN_RIGHT = -1;
    private const int TURN_NONE = 0;


    private static int Turn(PointD2DAnnotated p, PointD2DAnnotated q, PointD2DAnnotated r)
    {
      return ((q.X - p.X) * (r.Y - p.Y) - (r.X - p.X) * (q.Y - p.Y)).CompareTo(0);
    }

    private static void KeepLeft(List<PointD2DAnnotated> hull, PointD2DAnnotated r)
    {
      while (hull.Count > 1 && Turn(hull[hull.Count - 2], hull[hull.Count - 1], r) != TURN_LEFT)
      {
        hull.RemoveAt(hull.Count - 1);
      }
      if (hull.Count == 0 || hull[hull.Count - 1].ID != r.ID)
      {
        hull.Add(r);
      }
    }

    private static double GetAngle(PointD2DAnnotated p1, PointD2DAnnotated p2)
    {
      var xDiff = p2.X - p1.X;
      var yDiff = p2.Y - p1.Y;
      return Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI;
    }

    private static List<PointD2DAnnotated> MergeSort(PointD2DAnnotated p0, List<PointD2DAnnotated> arrPoint)
    {
      if (arrPoint.Count == 1)
      {
        return arrPoint;
      }
      var arrSortedInt = new List<PointD2DAnnotated>();
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
        else if (GetAngle(p0, leftArray[leftptr]) < GetAngle(p0, rightArray[rightptr]))
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
    /// Gets the convex hull of a set of points
    /// </summary>
    /// <param name="points">The points.</param>
    /// <returns>The ordered set of points that forms the hull.</returns>
    public static IReadOnlyList<PointD2DAnnotated> GetConvexHull(IEnumerable<PointD2DAnnotated> points)
    {
      PointD2DAnnotated p0 = default;
      var is_p0_initialized = false;
      foreach (var value in points)
      {
        if (!is_p0_initialized)
        {
          p0 = value;
          is_p0_initialized = true;
        }
        else
        {
          if (p0.Y > value.Y)
          {
            p0 = value;
          }
        }
      }

      if (!is_p0_initialized)
      {
        throw new ArgumentException("Enumeration is empty", nameof(points));
      }

      var order = new List<PointD2DAnnotated>();
      foreach (var value in points)
      {
        if (p0.ID != value.ID)
        {
          order.Add(value);
        }
      }

      order = MergeSort(p0, order);
      var result = new List<PointD2DAnnotated>
      {
        p0,
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
