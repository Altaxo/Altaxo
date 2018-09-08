/*
 *
Adapted from: https://github.com/masphei/ConvexHull
The MIT License (MIT)
Copyright (c) 2013 masphei

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

using System;
using System.Collections.Generic;

namespace Altaxo.Geometry.PolygonHull
{
  /// <summary>
  /// Calculation of the convex hull of a set of points by a Graham scan.
  /// </summary>
  public static class GrahamScan
  {
    private const int TURN_LEFT = 1;
    private const int TURN_RIGHT = -1;
    private const int TURN_NONE = 0;


    private static int turn(Node p, Node q, Node r)
    {
      return ((q.x - p.x) * (r.y - p.y) - (r.x - p.x) * (q.y - p.y)).CompareTo(0);
    }

    private static void keepLeft(List<Node> hull, Node r)
    {
      while (hull.Count > 1 && turn(hull[hull.Count - 2], hull[hull.Count - 1], r) != TURN_LEFT)
      {
        hull.RemoveAt(hull.Count - 1);
      }
      if (hull.Count == 0 || hull[hull.Count - 1] != r)
      {
        hull.Add(r);
      }
    }

    private static double getAngle(Node p1, Node p2)
    {
      var xDiff = p2.x - p1.x;
      var yDiff = p2.y - p1.y;
      return Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI;
    }

    private static List<Node> MergeSort(Node p0, List<Node> arrPoint)
    {
      if (arrPoint.Count == 1)
      {
        return arrPoint;
      }
      var arrSortedInt = new List<Node>();
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
        else if (getAngle(p0, leftArray[leftptr]) < getAngle(p0, rightArray[rightptr]))
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
    public static IReadOnlyList<Node> GetConvexHull(IEnumerable<Node> points)
    {
      Node p0 = null;
      foreach (var value in points)
      {
        if (p0 == null)
        {
          p0 = value;
        }
        else
        {
          if (p0.y > value.y)
          {
            p0 = value;
          }
        }
      }
      var order = new List<Node>();
      foreach (var value in points)
      {
        if (p0 != value)
        {
          order.Add(value);
        }
      }

      order = MergeSort(p0, order);
      var result = new List<Node>
      {
        p0,
        order[0],
        order[1]
      };
      order.RemoveAt(0);
      order.RemoveAt(0);
      foreach (var value in order)
      {
        keepLeft(result, value);
      }
      return result;
    }
  }
}

