#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
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

#if false
using System.Collections.Generic;
using ClipperLib;
using Xunit;

namespace Altaxo.ClipperLib
{
  public class Clipper1Tests
  {


    /// <summary>
    /// Tests the correct union of two overlapping squares.
    /// </summary>
    [Fact]
    public void TestUnionOfTouchingPolygonsWithHole()
    {
      var outer1 = new List<IntPoint>
      {
        new IntPoint(0, 0),
        new IntPoint(100, 0),
        new IntPoint(100, 10),
        new IntPoint(0, 10)
      };
      var outer2 = new List<IntPoint>
      {
        new IntPoint(90, 10),
        new IntPoint(100, 10),
        new IntPoint(100, 90),
        new IntPoint(90, 90)
      };
      var outer3 = new List<IntPoint>
      {
        new IntPoint(0, 90),
        new IntPoint(100, 90),
        new IntPoint(100, 100),
        new IntPoint(0, 100)
      };
      var outer4 = new List<IntPoint>
      {
        new IntPoint(0, 10),
        new IntPoint(10, 10),
        new IntPoint(10, 90),
        new IntPoint(0, 90)
      };

      var touchingPolygons = new List<List<IntPoint>> { outer1, outer2, outer3, outer4 };

      // Perform the union of the four polygons
      var clipper = new Clipper() { StrictlySimple = true };


      clipper.AddPaths(touchingPolygons, PolyType.ptSubject, true);
      var solution = new PolyTree(); // we use PolyTree here to better handle polygons with holes
      bool succeeded = clipper.Execute(ClipType.ctUnion, solution, PolyFillType.pftEvenOdd, PolyFillType.pftNegative);
      Assert.True(succeeded, "Clipper execution failed");
      Assert.True(solution.ChildCount == 1);
      Assert.False(solution.Childs[0].IsHole);
      Assert.True(solution.Childs[0].Contour.Count >= 4);
      Assert.True(solution.Childs[0].ChildCount == 1);
      Assert.True(solution.Childs[0].Childs[0].IsHole);
      Assert.Empty(solution.Childs[0].Childs[0].Childs);
    }
  }
}
#endif
