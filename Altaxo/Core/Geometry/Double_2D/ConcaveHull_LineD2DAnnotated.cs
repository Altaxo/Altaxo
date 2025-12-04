/*
 *
Adapted from: https://github.com/Liagson/ConcaveHullGenerator
The MIT License (MIT)
Copyright (c) Alberto Aliaga

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
  public partial class ConcaveHull
  {
    /// <summary>
    /// Represents an annotated line segment between two 2D points.
    /// </summary>
    private class LineD2DAnnotated
    {
      /// <summary>
      /// Gets or sets a value indicating whether this line has been checked during hull construction.
      /// </summary>
      public bool IsChecked { get; set; }
      /// <summary>
      /// Gets the first point of the line segment.
      /// </summary>
      public PointD2DAnnotated P0 { get; private set; }
      /// <summary>
      /// Gets the second point of the line segment.
      /// </summary>
      public PointD2DAnnotated P1 { get; private set; }

      /// <summary>
      /// Initializes a new instance of the <see cref="LineD2DAnnotated"/> class.
      /// </summary>
      /// <param name="p0">The first point.</param>
      /// <param name="p1">The second point.</param>
      public LineD2DAnnotated(PointD2DAnnotated p0, PointD2DAnnotated p1)
      {
        P0 = p0;
        P1 = p1;
      }

      /// <summary>
      /// Gets an enumeration of the two points of the line segment.
      /// </summary>
      public IEnumerable<PointD2DAnnotated> Points
      {
        get
        {
          yield return P0;
          yield return P1;
        }
      }

      /// <summary>
      /// Gets the point at the specified index (0 for P0, 1 for P1).
      /// </summary>
      /// <param name="idx">Index of the point (0 or 1).</param>
      /// <returns>The point at the specified index.</returns>
      /// <exception cref="IndexOutOfRangeException">Thrown if index is not 0 or 1.</exception>
      public PointD2DAnnotated this[int idx]
      {
        get
        {
          switch (idx)
          {
            case 0:
              return P0;
            case 1:
              return P1;
            default:
              throw new IndexOutOfRangeException("Index out of range [0,1]");
          }
        }
      }

      /// <summary>
      /// Calculates the Euclidean distance between two annotated points.
      /// </summary>
      /// <param name="node1">The first point.</param>
      /// <param name="node2">The second point.</param>
      /// <returns>The distance between the two points.</returns>
      public static double GetDistance(PointD2DAnnotated node1, PointD2DAnnotated node2)
      {
        var dx = node1.X - node2.X;
        var dy = node1.Y - node2.Y;
        return Math.Sqrt(dx * dx + dy * dy);
      }
    }
  }
}
