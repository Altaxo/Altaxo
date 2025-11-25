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
using ClipperLib;
using Xunit;

namespace Altaxo.Geometry.Double_2D
{
  public class PolygonTestBase
  {
    /// <summary>
    /// Tests if all points given in <paramref name="allPoints"/> are on the hull or inside the hull.
    /// Attention: all points must have integer coordinates!!! (This is because we use the Clipper library for testing).
    /// </summary>
    /// <param name="hull">The hull.</param>
    /// <param name="allPoints">All points.</param>
    public void IncludenessTest(IReadOnlyList<PointD2DAnnotated> hull, IReadOnlyList<PointD2DAnnotated> allPoints)
    {

      Assert.NotNull(hull);        // convex hull has to be != null
      Assert.True(hull.Count >= 3); // at least 3 points form the convex hull


      // Test that the convex hull does not contain doubled points
      var hash = new HashSet<(int, int)>();
      for (var i = 0; i < hull.Count; ++i)
      {
        Assert.DoesNotContain(((int)hull[i].X, (int)hull[i].Y), hash);
        hash.Add(((int)hull[i].X, (int)hull[i].Y));
      }


      // Various tests with clipper
      var clipperPoly = new List<IntPoint>(hull.Select(dp => new IntPoint(dp.X, dp.Y)));

      // The area should be != 0
      Assert.True(Math.Abs(Clipper.Area(clipperPoly)) > 0); // Area should be != 0
      //Assert.Greater(ClipperLib.Clipper.Area(clipperPoly), 0); // Polygon should be positive oriented

      // The polygon should be simple
      var clipperPolys = Clipper.SimplifyPolygon(clipperPoly);
      Assert.Single(clipperPolys); // if polygon is simple, it can not be transformed into two or more polygons
      Assert.True(clipperPolys[0].Count <= clipperPoly.Count); // the resulting polygon should have the same number of points than the original one


      // Test whether all points are on the hull or inside the hull
      for (var i = 0; i < allPoints.Count; ++i)
      {
        Assert.NotEqual(0, Clipper.PointInPolygon(new IntPoint(allPoints[i].X, allPoints[i].Y), clipperPoly));
      }
    }
  }
}
