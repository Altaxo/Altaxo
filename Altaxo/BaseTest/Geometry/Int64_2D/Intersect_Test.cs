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
using NUnit.Framework;

namespace Altaxo.Geometry.Int64_2D
{
  [TestFixture]
  public class Intersect_Test : PolygonTestBase
  {
    private readonly (Int64LineSegment l1, Int64LineSegment l2)[] _intersectingLines = new (Int64LineSegment l1, Int64LineSegment l2)[]
    {
      (
        new Int64LineSegment(new IntPoint(-100, 0), new IntPoint(100, 0)), // horizontal
        new Int64LineSegment(new IntPoint(-100, 0), new IntPoint(100, 0))  // vertical
      ),
      (
        new Int64LineSegment(new IntPoint(0, 0), new IntPoint(100, 100)),
        new Int64LineSegment(new IntPoint(20, 20), new IntPoint(150, 40)) // touching at 20,20
      ),
      (
        new Int64LineSegment(new IntPoint(-100, -100), new IntPoint(-100, 100)), // vertical
        new Int64LineSegment(new IntPoint(-100, 0), new IntPoint(0, 0))  // horizontal
      ),
      (
        new Int64LineSegment(new IntPoint(0, 40), new IntPoint(40, 40)), // horizontal
        new Int64LineSegment(new IntPoint(40, 0), new IntPoint(40, 80))  // vertical with y overlap
      ),
      (
        new Int64LineSegment(new IntPoint(0, 0), new IntPoint(100, 100)),
        new Int64LineSegment(new IntPoint(20, 20), new IntPoint(60, 60))  // included
      ),
      (
        new Int64LineSegment(new IntPoint(20, 20), new IntPoint(180, -100)),
        new Int64LineSegment(new IntPoint(20, 20), new IntPoint(180, -100))  // same
      ),

    };
    private readonly (Int64LineSegment l1, Int64LineSegment l2)[] _nonintersectingLines = new (Int64LineSegment l1, Int64LineSegment l2)[]
{
      (
        new Int64LineSegment(new IntPoint(40, 40), new IntPoint(150, 150)),
        new Int64LineSegment(new IntPoint(60, 80), new IntPoint(80, 100)) // parallel with y overlap
      ),
      (
        new Int64LineSegment(new IntPoint(-80, 80), new IntPoint(-40, 20)),
        new Int64LineSegment(new IntPoint(-40, 60), new IntPoint(0, 0)) // parallel with just x overlap
      ),
      (
        new Int64LineSegment(new IntPoint(0, 0), new IntPoint(0, 20)), // vertical
        new Int64LineSegment(new IntPoint(40, 40), new IntPoint(40, 60))  // vertical no overlap
      ),
      (
        new Int64LineSegment(new IntPoint(0, 0), new IntPoint(0, 20)), // vertical
        new Int64LineSegment(new IntPoint(40, 40), new IntPoint(60, 40))  // horizontal no overlap
      ),
      (
        new Int64LineSegment(new IntPoint(-30, -30), new IntPoint(50, 50)),
        new Int64LineSegment(new IntPoint(70, 70), new IntPoint(120, 1200))  // colinear no overlap
      ),
      (
        new Int64LineSegment(new IntPoint(0, 0), new IntPoint(20, 20)),
        new Int64LineSegment(new IntPoint(10, 40), new IntPoint(40, 0))  // almost perpendicular with x and y overlap
      ),
      (
        new Int64LineSegment(new IntPoint(20, 20), new IntPoint(80, 20)),
        new Int64LineSegment(new IntPoint(40, 40), new IntPoint(60, 40))  // horizontal with x overlap
      ),
      (
        new Int64LineSegment(new IntPoint(40, 20), new IntPoint(40, 40)),
        new Int64LineSegment(new IntPoint(0, 80), new IntPoint(100, 0))  // 45° x and y overlap
      ),

};


    [Test]
    public void Test_Intersection()
    {
      foreach (var linePair in _intersectingLines)
      {
        Assert.True(ConcaveHull.DoLinesIntersectOrTouch(linePair.l1, linePair.l1)); // identical l1
        Assert.True(ConcaveHull.DoLinesIntersectOrTouch(linePair.l2, linePair.l2)); // identical l2

        Assert.True(ConcaveHull.DoLinesIntersectOrTouch(linePair.l1, linePair.l2));
        Assert.True(ConcaveHull.DoLinesIntersectOrTouch(linePair.l2, linePair.l1));

        // Permutations of the line points
        Assert.True(ConcaveHull.DoLinesIntersectOrTouch(new Int64LineSegment(linePair.l1.P1, linePair.l1.P0), linePair.l2));
        Assert.True(ConcaveHull.DoLinesIntersectOrTouch(linePair.l2, new Int64LineSegment(linePair.l1.P1, linePair.l1.P0)));

        Assert.True(ConcaveHull.DoLinesIntersectOrTouch(linePair.l1, new Int64LineSegment(linePair.l2.P1, linePair.l2.P0)));
        Assert.True(ConcaveHull.DoLinesIntersectOrTouch(new Int64LineSegment(linePair.l2.P1, linePair.l2.P0), linePair.l1));

        Assert.True(ConcaveHull.DoLinesIntersectOrTouch(new Int64LineSegment(linePair.l1.P1, linePair.l1.P0), new Int64LineSegment(linePair.l2.P1, linePair.l2.P0)));
        Assert.True(ConcaveHull.DoLinesIntersectOrTouch(new Int64LineSegment(linePair.l2.P1, linePair.l2.P0), new Int64LineSegment(linePair.l1.P1, linePair.l1.P0)));
      }
    }

    [Test]
    public void Test_NonIntersection()
    {
      foreach (var linePair in _nonintersectingLines)
      {
        Assert.False(ConcaveHull.DoLinesIntersectOrTouch(linePair.l1, linePair.l2));
        Assert.False(ConcaveHull.DoLinesIntersectOrTouch(linePair.l2, linePair.l1));

        // Permutations of the line points
        Assert.False(ConcaveHull.DoLinesIntersectOrTouch(new Int64LineSegment(linePair.l1.P1, linePair.l1.P0), linePair.l2));
        Assert.False(ConcaveHull.DoLinesIntersectOrTouch(linePair.l2, new Int64LineSegment(linePair.l1.P1, linePair.l1.P0)));

        Assert.False(ConcaveHull.DoLinesIntersectOrTouch(linePair.l1, new Int64LineSegment(linePair.l2.P1, linePair.l2.P0)));
        Assert.False(ConcaveHull.DoLinesIntersectOrTouch(new Int64LineSegment(linePair.l2.P1, linePair.l2.P0), linePair.l1));

        Assert.False(ConcaveHull.DoLinesIntersectOrTouch(new Int64LineSegment(linePair.l1.P1, linePair.l1.P0), new Int64LineSegment(linePair.l2.P1, linePair.l2.P0)));
        Assert.False(ConcaveHull.DoLinesIntersectOrTouch(new Int64LineSegment(linePair.l2.P1, linePair.l2.P0), new Int64LineSegment(linePair.l1.P1, linePair.l1.P0)));
      }
    }
  }
}
