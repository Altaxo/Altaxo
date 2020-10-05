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
using Xunit;

namespace Altaxo.Geometry.Int64_2D
{
  public class PolygonTestBase
  {
    /// <summary>
    /// Tests if all points given in <paramref name="allPoints"/> are on the hull or inside the hull.
    /// Attention: all points must have integer coordinates!!! (This is because we use the Clipper library for testing).
    /// </summary>
    /// <param name="hull">The hull.</param>
    /// <param name="allPoints">All points.</param>
    public void IncludenessTest(IReadOnlyList<(IntPoint point, int index)> hull, IReadOnlyList<IntPoint> allPoints)
    {

      Assert.NotNull(hull);        // convex hull has to be != null
      Assert.True(hull.Count >= 3); // at least 3 points form the convex hull


      // Test that the convex hull does not contain doubled points
      var hash = new HashSet<(int, int)>();
      for (var i = 0; i < hull.Count; ++i)
      {
        Assert.DoesNotContain(((int)hull[i].point.X, (int)hull[i].point.Y), hash);
        hash.Add(((int)hull[i].point.X, (int)hull[i].point.Y));
      }


      // Various tests with clipper
      var clipperPoly = new List<ClipperLib.IntPoint>(hull.Select(dp => new ClipperLib.IntPoint(dp.point.X, dp.point.Y)));

      // The area should be != 0
      Assert.True(Math.Abs(ClipperLib.Clipper.Area(clipperPoly)) > 0); // Area should be != 0
      Assert.True(ClipperLib.Clipper.Area(clipperPoly) > 0); // Polygon should be positive oriented

      // The polygon should be simple
      var clipperPolys = ClipperLib.Clipper.SimplifyPolygon(clipperPoly);

      if (clipperPolys.Count != 1)
      {
        // note: Clipper simplifies polygons even if the segments do not touch, but are near enough to each other
        // thus when clipper has created two or more polygons, we need some check with
        // or own library if this is "wrong" alarm
        CheckHullForIntersections(hull);
      }

      Assert.True(clipperPolys[0].Count <= clipperPoly.Count); // the resulting polygon should have the same number of points than the original one


      // Test whether all points are on the hull or inside the hull
      for (var i = 0; i < allPoints.Count; ++i)
      {
        Assert.NotEqual(0, ClipperLib.Clipper.PointInPolygon(new ClipperLib.IntPoint(allPoints[i].X, allPoints[i].Y), clipperPoly));
      }
    }

    public static void CheckHullForIntersections(IReadOnlyList<(IntPoint point, int index)> hull)
    {
      var segments = new Int64LineD2DAnnotated[hull.Count];

      for (var i = 0; i < hull.Count - 1; ++i)
      {
        segments[i] = new Int64LineD2DAnnotated(hull[i], hull[i + 1]);
      }

      segments[segments.Length - 1] = new Int64LineD2DAnnotated(hull[hull.Count - 1], hull[0]);

      for (var i = 0; i < segments.Length - 1; ++i)
      {
        for (var j = i + 2; j < segments.Length; ++j)
        {

          // two adjacent segments are allowed to touch,
          // thus either j==i+1, or i==0 and j==segments.Count-1 are allowed
          if (!(i == 0 && j == segments.Length - 1))
          {
            Assert.False(ConcaveHull.DoLinesIntersectOrTouch(segments[i].Line, segments[j].Line));
          }
        }
      }
    }
  }
}
