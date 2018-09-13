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

namespace Altaxo.Geometry.PolygonHull.Int64
{
  [TestFixture]
  public class ConcaveHull_Test : PolygonTestBase
  {
    private Random _random = new Random(1);

    [Test]
    public void Test1()
    {
      const int numberOfPoints = 5;

      var a = new double[2 * numberOfPoints]
      {
      -567,-912,
      -651,-882,
      365,-157,
      -149,-573,
      -634,611,
      };


      var arr = new IntPoint[numberOfPoints];
      for (var i = 0; i < numberOfPoints; ++i)
      {
        arr[i] = new IntPoint(a[2 * i], a[2 * i + 1]);
      }

      var concaveCalc = new ConcaveHull(arr, 0, 2);
      IncludenessTest(concaveCalc.ConvexHullPoints, arr);
      IncludenessTest(concaveCalc.ConcaveHullPoints, arr);
    }

    [Test]
    public void Test_Includeness()
    {
      var hash = new HashSet<(int, int)>();

      for (var numberOfTests = 0; numberOfTests < 10; ++numberOfTests)
      {
        var numberOfPoints = 20 + numberOfTests * 10;
        var arr = new IntPoint[numberOfPoints];
        var arrxy = new int[numberOfPoints, 2];


        hash.Clear();
        for (var i = 0; i < numberOfPoints;)
        {
          var x = _random.Next(-1000, 1000);
          var y = _random.Next(-1000, 1000);

          if (!hash.Contains((x, y)))
          {
            hash.Add((x, y));
            arr[i] = new IntPoint(x, y);
            arrxy[i, 0] = x;
            arrxy[i, 1] = y;
            ++i;
          }
        }

        for (double concaveness = 1; concaveness >= -1; concaveness -= 1 / 16.0)
        {
          var concaveCalc = new ConcaveHull(arr, concaveness, 2);

          IncludenessTest(concaveCalc.ConvexHullPoints, arr);
          IncludenessTest(concaveCalc.ConcaveHullPoints, arr);
        }
      }
    }

    [Test]
    public void Test_FivePoints()
    {
      var arr = new IntPoint[5];

      arr[0] = new IntPoint(-90, -10);
      arr[1] = new IntPoint(80, -20);
      arr[2] = new IntPoint(-100, 0);
      arr[3] = new IntPoint(100, 0);
      arr[4] = new IntPoint(0, -10);
      var concaveCalc = new ConcaveHull(arr, -1, 2);
      IncludenessTest(concaveCalc.ConvexHullPoints, arr);
      IncludenessTest(concaveCalc.ConcaveHullPoints, arr);
    }

    [Test]
    public void Test_FivePoints2()
    {
      var arr = new IntPoint[5];

      arr[0] = new IntPoint(-100, -100);
      arr[1] = new IntPoint(100, -100);
      arr[2] = new IntPoint(100, 100);
      arr[3] = new IntPoint(0, 90);
      arr[4] = new IntPoint(-100, 100);
      var concaveCalc = new ConcaveHull(arr, Math.Cos(Math.PI / 8), 2);
      IncludenessTest(concaveCalc.ConvexHullPoints, arr);
      IncludenessTest(concaveCalc.ConcaveHullPoints, arr);
      Assert.AreEqual(5, concaveCalc.ConcaveHullPoints.Count);
    }

    [Test]
    public void Test_ColinearPointsHorizontal()
    {
      var arr = new IntPoint[7];

      arr[0] = new IntPoint(-1000, -1000);
      arr[1] = new IntPoint(1000, -1000);
      arr[2] = new IntPoint(1000, 0); // 1000, 0, 0,0 and -1000, 0 are colinear
      arr[3] = new IntPoint(500, -10); // a little bit under this colinear line another point
      arr[4] = new IntPoint(0, 0);
      arr[5] = new IntPoint(-500, -10); // here another one a little bit under the colinear line
      arr[6] = new IntPoint(-1000, 0);
      var concaveCalc = new ConcaveHull(arr, 0, 2);

      // will the points 500, -10 and -500, -10 be found?
      Assert.AreEqual(4, concaveCalc.ConvexHullPoints.Count);
      Assert.AreEqual(7, concaveCalc.ConcaveHullPoints.Count);

      IncludenessTest(concaveCalc.ConvexHullPoints, arr);
      IncludenessTest(concaveCalc.ConcaveHullPoints, arr);
    }

    [Test]
    public void Test_ForbiddenPointOnHull()
    {
      var arr = new IntPoint[5];

      arr[0] = new IntPoint(-1100, -20);
      arr[1] = new IntPoint(1100, -20); // lower line somewhat larger in order to be processed first
      arr[2] = new IntPoint(1000, 0); // 1000, 0, 0,0 and -1000, 0 are colinear
      arr[3] = new IntPoint(0, 0); // this point must not be catched by the lower horz line!
      arr[4] = new IntPoint(-1000, 0);
      var concaveCalc = new ConcaveHull(arr, 0, 2);

      // convex and concave hull may contain 0,0, but only as part of the upper horizontal line
      Assert.AreEqual(4, concaveCalc.ConvexHullPoints.Count);

      IncludenessTest(concaveCalc.ConvexHullPoints, arr);
      IncludenessTest(concaveCalc.ConcaveHullPoints, arr);
    }

    [Test]
    public void Test_Polygon_Area()
    {
      var arr = new IntPoint[4];

      arr[0] = new IntPoint(-200, -100);
      arr[1] = new IntPoint(100, -100);
      arr[2] = new IntPoint(100, 100);
      arr[3] = new IntPoint(-200, 100);

      var area = ConcaveHull.GetPolygonArea(arr);
      Assert.AreEqual(60000, area); // counterclockwise should give positive area
    }

    [Test]
    public void Test_Polygon_Centroid()
    {
      var arr = new IntPoint[4];

      arr[0] = new IntPoint(-100, -200);
      arr[1] = new IntPoint(100, -200);
      arr[2] = new IntPoint(100, 200);
      arr[3] = new IntPoint(-100, 200);

      var x = 77;
      var y = 44;

      for (var i = 0; i < arr.Length; ++i)
      {
        arr[i] = new IntPoint(x + arr[i].X, y + arr[i].Y);
      }

      var area = ConcaveHull.GetPolygonArea(arr);
      Assert.AreEqual(80000, area); // counterclockwise should give positive area
      var centroid = ConcaveHull.GetPolygonCentroid(arr);
      Assert.AreEqual(x, centroid.X);
      Assert.AreEqual(y, centroid.Y);
    }
  }
}
