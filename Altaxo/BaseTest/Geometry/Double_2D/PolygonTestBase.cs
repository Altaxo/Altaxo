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
using NUnit.Framework;

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

      Assert.IsNotNull(hull);        // convex hull has to be != null
      Assert.GreaterOrEqual(hull.Count, 3); // at least 3 points form the convex hull


      // Test that the convex hull does not contain doubled points
      var hash = new HashSet<(int, int)>();
      for (var i = 0; i < hull.Count; ++i)
      {
        Assert.IsFalse(hash.Contains(((int)hull[i].X, (int)hull[i].Y)));
        hash.Add(((int)hull[i].X, (int)hull[i].Y));
      }


      // Various tests with clipper
      var clipperPoly = new List<ClipperLib.IntPoint>(hull.Select(dp => new ClipperLib.IntPoint(dp.X, dp.Y)));

      // The area should be != 0
      Assert.Greater(Math.Abs(ClipperLib.Clipper.Area(clipperPoly)), 0); // Area should be != 0
      //Assert.Greater(ClipperLib.Clipper.Area(clipperPoly), 0); // Polygon should be positive oriented

      // The polygon should be simple
      var clipperPolys = ClipperLib.Clipper.SimplifyPolygon(clipperPoly);
      Assert.AreEqual(1, clipperPolys.Count); // if polygon is simple, it can not be transformed into two or more polygons
      Assert.LessOrEqual(clipperPolys[0].Count, clipperPoly.Count); // the resulting polygon should have the same number of points than the original one


      // Test whether all points are on the hull or inside the hull
      for (var i = 0; i < allPoints.Count; ++i)
      {
        Assert.AreNotEqual(0, ClipperLib.Clipper.PointInPolygon(new ClipperLib.IntPoint(allPoints[i].X, allPoints[i].Y), clipperPoly));
      }
    }
  }
}
