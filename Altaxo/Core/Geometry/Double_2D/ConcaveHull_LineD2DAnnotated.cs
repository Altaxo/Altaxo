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
    private class LineD2DAnnotated
    {
      public bool IsChecked { get; set; }
      public PointD2DAnnotated P0 { get; private set; }
      public PointD2DAnnotated P1 { get; private set; }


      public LineD2DAnnotated(PointD2DAnnotated p0, PointD2DAnnotated p1)
      {
        P0 = p0;
        P1 = p1;
      }

      public IEnumerable<PointD2DAnnotated> Points
      {
        get
        {
          yield return P0;
          yield return P1;
        }
      }

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

      public static double GetDistance(PointD2DAnnotated node1, PointD2DAnnotated node2)
      {
        var dx = node1.X - node2.X;
        var dy = node1.Y - node2.Y;
        return Math.Sqrt(dx * dx + dy * dy);
      }
    }
  }
}
