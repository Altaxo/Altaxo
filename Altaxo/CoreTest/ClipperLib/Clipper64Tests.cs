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

using Clipper2Lib;
using Xunit;

namespace Altaxo.ClipperLib
{
  public class Clipper64Tests
  {
    /// <summary>
    /// Tests the correct union of two overlapping squares.
    /// </summary>
    [Fact]
    public void TestUnionOfOverlappingPolygons()
    {
      Path64 outer1 = Clipper.MakePath(new long[] { 0, 0, 100, 0, 100, 100, 0, 100 }); // CCW orientation
      Path64 outer2 = Clipper.MakePath(new long[] { 50, 0, 150, 0, 150, 100, 50, 100 }); // CCW orientation
      Paths64 twoOverlappingPolygons = new Paths64 { outer1, outer2 };

      // Basics: test the inside property. If it fails, the winding order of our polygons is probably wrong.
      Assert.True(PointInPolygonResult.IsInside == Clipper.PointInPolygon(new Point64(50, 50), outer1)); // Point 50,50 should be inside outer1
      Assert.True(PointInPolygonResult.IsOn == Clipper.PointInPolygon(new Point64(50, 50), outer2)); // Point 50,50 should be on the edge of outer2
      Assert.True(PointInPolygonResult.IsOn == Clipper.PointInPolygon(new Point64(100, 50), outer1)); // Point 100,50 should be on the edge of outer1
      Assert.True(PointInPolygonResult.IsInside == Clipper.PointInPolygon(new Point64(100, 50), outer2)); // Point 100,50 should be inside outer2

      // Perform the union of the two overlapping polygons
      Clipper64 clipper = new Clipper64();
      clipper.AddSubject(twoOverlappingPolygons);
      Paths64 solution = new Paths64();
      bool succeeded = clipper.Execute(ClipType.Union, FillRule.NonZero, solution);
      Assert.True(succeeded, "Clipper execution failed");
      Assert.Single(solution); // There should be only one polygon in the solution, because the two input polygons overlap
      Path64 resultPolygon = solution[0];
      // The resulting polygon should have 4 or more vertices (more than 4 if the polygon is not simplified)
      Assert.True(resultPolygon.Count >= 4);

      var simplifiedSolution = Clipper.SimplifyPaths(solution, 0);
      Assert.Single(simplifiedSolution); // There should be only one polygon in the solution, because the two input polygons overlap
      Path64 simplifiedPolygon = simplifiedSolution[0];
      // The resulting polygon should have new exactly 4 vertices 0,0, 150,0, 150,100, 50, 100
      Assert.True(simplifiedPolygon.Count == 4);
      Assert.Contains(new Point64(0, 0), simplifiedPolygon);
      Assert.Contains(new Point64(150, 0), simplifiedPolygon);
      Assert.Contains(new Point64(150, 100), simplifiedPolygon);
      Assert.Contains(new Point64(0, 100), simplifiedPolygon);
      Assert.True(PointInPolygonResult.IsInside == Clipper.PointInPolygon(new Point64(50, 50), simplifiedPolygon)); // Point 50,50 should be included in the union
      Assert.True(PointInPolygonResult.IsInside == Clipper.PointInPolygon(new Point64(100, 50), simplifiedPolygon)); // Point 100,50 should be included in the union
    }

    /// <summary>
    /// Tests the correct union of two overlapping squares.
    /// </summary>
    [Fact]
    public void TestUnionOfNonoverlappingPolygons()
    {
      Path64 outer1 = Clipper.MakePath(new long[] { 0, 0, 100, 0, 100, 100, 0, 100 }); // CCW orientation
      Path64 outer2 = Clipper.MakePath(new long[] { 200, 0, 300, 0, 300, 100, 200, 100 }); // CCW orientation
      Paths64 twoNonoverlappingPolygons = new Paths64 { outer1, outer2 };

      // Basics: test the inside property. If it fails, the winding order of our polygons is probably wrong.
      Assert.True(PointInPolygonResult.IsInside == Clipper.PointInPolygon(new Point64(50, 50), outer1)); // Point 50,50 should be inside outer1
      Assert.True(PointInPolygonResult.IsOn == Clipper.PointInPolygon(new Point64(100, 50), outer1)); // Point 100,50 should be on the edge of outer1
      Assert.True(PointInPolygonResult.IsOutside == Clipper.PointInPolygon(new Point64(150, 50), outer1)); // Point 150,50 should be outside outer1
      Assert.True(PointInPolygonResult.IsOutside == Clipper.PointInPolygon(new Point64(150, 50), outer2)); // Point 150,50 should be outside outer2

      // Perform the union of the two overlapping polygons
      Clipper64 clipper = new Clipper64();
      clipper.AddSubject(twoNonoverlappingPolygons);
      Paths64 solution = new Paths64();
      bool succeeded = clipper.Execute(ClipType.Union, FillRule.NonZero, solution);
      Assert.True(succeeded, "Clipper execution failed");
      Assert.True(2 == solution.Count); // There should be still 2 polygons
      // The resulting polygon should have 4 or more vertices (more than 4 if the polygon is not simplified)
      Assert.True(solution[0].Count == 4);
      Assert.True(solution[1].Count == 4);
    }

    /// <summary>
    /// Tests the correct union of four touching squares, that form a hole in the center. 
    /// </summary>
    [Fact]
    public void TestUnionOfTouchingPolygonsWithHole()
    {
      // the four outer rectangles create a square of 100x100 with a hole of 80x80 in the center
      Path64 outer1 = Clipper.MakePath(new long[] { 0, 0, 100, 0, 100, 10, 0, 10 }); // bottom rectangle
      Path64 outer2 = Clipper.MakePath(new long[] { 90, 10, 100, 10, 100, 90, 90, 90 }); // right rectangle
      Path64 outer3 = Clipper.MakePath(new long[] { 0, 90, 100, 90, 100, 100, 0, 100 }); // top rectangle
      Path64 outer4 = Clipper.MakePath(new long[] { 0, 10, 10, 10, 10, 90, 0, 90 }); // left rectangle
      Paths64 touchingPolygons = new Paths64 { outer1, outer2, outer3, outer4 };

      Assert.True(Clipper.IsPositive(outer1));
      Assert.True(Clipper.IsPositive(outer2));
      Assert.True(Clipper.IsPositive(outer3));
      Assert.True(Clipper.IsPositive(outer4));

      // Perform the union of the four polygons
      Clipper64 clipper = new Clipper64();
      clipper.AddSubject(touchingPolygons);
      var solution = new PolyTree64(); // we use PolyTree here to better handle polygons with holes
      bool succeeded = clipper.Execute(ClipType.Union, FillRule.EvenOdd, solution);
      Assert.True(succeeded, "Clipper execution failed");
      Assert.True(solution.Count == 1); // we now should have 2 polygons in the solution: the outer polygon (100x100) and as a child the hole (80x80)
      // The resulting (simplified) polygon should have exactly 4 vertices 0,0, 100,0, 100,100, 0, 100
      Assert.False(solution[0].IsHole);
      var outer = Clipper.SimplifyPath(solution[0].Polygon, 0);
      Assert.True(outer.Count == 4);
      Assert.Contains(new Point64(0, 0), outer);
      Assert.Contains(new Point64(100, 0), outer);
      Assert.Contains(new Point64(100, 100), outer);
      Assert.Contains(new Point64(0, 100), outer);

      // The hole should have exactly 4 vertices 10,10, 90,10, 90,90, 10,90
      Assert.True(solution[0].Count == 1);
      Assert.True(solution[0][0].IsHole);
      var hole = Clipper.SimplifyPath(solution[0][0].Polygon, 0);
      Assert.True(hole.Count == 4);
      Assert.Contains(new Point64(10, 10), hole);
      Assert.Contains(new Point64(90, 10), hole);
      Assert.Contains(new Point64(90, 90), hole);
      Assert.Contains(new Point64(10, 90), hole);
    }



    /// <summary>
    /// Tests the correct union of four overlapping squares, that form a hole in the center. An additional inset polygon is added to fill part of the hole.
    /// </summary>
    [Fact]
    public void TestUnionOfOverlappingPolygonsWithHoleAndInset()
    {
      // the four outer rectangles create a square of 100x100 with a hole of 80x80 in the center
      Path64 outer1 = Clipper.MakePath(new long[] { 0, 0, 100, 0, 100, 10, 0, 10 }); // bottom rectangle
      Path64 outer2 = Clipper.MakePath(new long[] { 90, 10, 100, 10, 100, 90, 90, 90 }); // right rectangle
      Path64 outer3 = Clipper.MakePath(new long[] { 0, 90, 100, 90, 100, 100, 0, 100 }); // top rectangle
      Path64 outer4 = Clipper.MakePath(new long[] { 0, 10, 10, 10, 10, 90, 0, 90 }); // left rectangle

      Path64 inset1 = Clipper.MakePath(new long[] { 40, 40, 60, 40, 60, 60, 40, 60 }); // inset
      Paths64 overlappingPolygons = new Paths64 { outer1, outer2, outer3, outer4, inset1 };

      // Perform the union of the two overlapping polygons
      Clipper64 clipper = new Clipper64();
      clipper.AddSubject(overlappingPolygons);
      var solution = new PolyTree64(); // we use PolyTree here to better handle complex polygons

      bool succeeded = clipper.Execute(ClipType.Union, FillRule.NonZero, solution);
      Assert.True(succeeded, "Clipper execution failed");

      // we now should have 3 polygons in the solution: the outer polygon (100x100), the hole (80x80), and the inset (20x20)

      Assert.True(solution.Count == 1); // we should have 1 top-level outer polygon 
      Assert.False(solution[0].IsHole);
      var outer = Clipper.SimplifyPath(solution[0].Polygon, 0);
      // The resulting polygon should have new exactly 4 vertices 0,0, 150,0, 150,100, 50, 100
      Assert.True(outer.Count == 4);
      Assert.Contains(new Point64(0, 0), outer);
      Assert.Contains(new Point64(100, 0), outer);
      Assert.Contains(new Point64(100, 100), outer);
      Assert.Contains(new Point64(0, 100), outer);

      // we should have 1 hole polygon
      Assert.True(solution[0].Count == 1);
      Assert.True(solution[0][0].IsHole);
      var hole = Clipper.SimplifyPath(solution[0][0].Polygon, 0);
      Assert.True(hole.Count == 4);
      Assert.Contains(new Point64(10, 10), hole);
      Assert.Contains(new Point64(90, 10), hole);
      Assert.Contains(new Point64(90, 90), hole);
      Assert.Contains(new Point64(10, 90), hole);

      // and finally, we should have 1 inset polygon (not a hole)
      Assert.True(solution[0][0].Count == 1);
      Assert.False(solution[0][0][0].IsHole);
      var inset = Clipper.SimplifyPath(solution[0][0][0].Polygon, 0);
      Assert.True(inset.Count == 4);
      Assert.Contains(new Point64(40, 40), inset);
      Assert.Contains(new Point64(60, 40), inset);
      Assert.Contains(new Point64(60, 60), inset);
      Assert.Contains(new Point64(40, 60), inset);

      // that's all, there should be no more polygons
      Assert.True(solution[0][0][0].Count == 0);
    }

    /// <summary>
    /// Tests the correct expansion of one polygon.
    /// </summary>
    [Fact]
    public void TestOnePolygonExpanded()
    {
      Path64 outer1 = Clipper.MakePath(new long[] { 0, 0, 100, 0, 100, 100, 0, 100 });
      Paths64 allPolygons = new Paths64 { outer1 };
      var offsetPolygon = Clipper.InflatePaths(allPolygons, 10, JoinType.Miter, EndType.Polygon); // EndType.Polygon for closed paths
      offsetPolygon = Clipper.SimplifyPaths(offsetPolygon, 0, true);
      Assert.Single(offsetPolygon);
      Assert.True(Clipper.IsPositive(offsetPolygon[0]));
      Assert.True(4 == offsetPolygon[0].Count);
      Assert.Contains(new Point64(-10, -10), offsetPolygon[0]);
      Assert.Contains(new Point64(110, -10), offsetPolygon[0]);
      Assert.Contains(new Point64(110, 110), offsetPolygon[0]);
      Assert.Contains(new Point64(-10, 110), offsetPolygon[0]);
    }


    /// <summary>
    /// Tests the correct union of four touching squares, that form a hole in the center. Then, the polygons are expanded and the union is tested again.
    /// </summary>
    [Fact]
    public void TestUnionOfTouchingPolygonsWithHoleExpanded()
    {
      // the four outer rectangles create a square of 100x100 with a hole of 80x80 in the center
      Path64 outer1 = Clipper.MakePath(new long[] { 0, 0, 100, 0, 100, 10, 0, 10 }); // bottom rectangle
      Path64 outer2 = Clipper.MakePath(new long[] { 90, 10, 100, 10, 100, 90, 90, 90 }); // right rectangle
      Path64 outer3 = Clipper.MakePath(new long[] { 0, 90, 100, 90, 100, 100, 0, 100 }); // top rectangle
      Path64 outer4 = Clipper.MakePath(new long[] { 0, 10, 10, 10, 10, 90, 0, 90 }); // left rectangle
      Paths64 touchingPolygons = new Paths64 { outer1, outer2, outer3, outer4 };

      // Perform the union of the four polygons
      Clipper64 clipper = new Clipper64();
      clipper.AddSubject(touchingPolygons);
      var solution = new Paths64(); // we use PolyTree here to better handle polygons with holes
      bool succeeded = clipper.Execute(ClipType.Union, FillRule.NonZero, solution);
      Assert.True(succeeded, "Clipper execution failed");
      solution = Clipper.SimplifyPaths(solution, 0, true);
      Assert.True(solution.Count == 2); // 2 Polygons expected.
      Assert.False(Clipper.IsPositive(solution[0]));
      Assert.True(Clipper.IsPositive(solution[1]));

      var offsetPolygon = Clipper.InflatePaths(solution, 10, JoinType.Miter, EndType.Polygon); // EndType.Polygon for closed paths
      offsetPolygon = Clipper.SimplifyPaths(offsetPolygon, 0, true);
      Assert.True(offsetPolygon.Count == 2); // after offsetting, there should still be outer polygon and hole
      var (indexOfOuter, indexOfHole) = Clipper.IsPositive(offsetPolygon[0]) ? (0, 1) : (1, 0);
      Assert.True(Clipper.IsPositive(offsetPolygon[indexOfOuter]));
      Assert.True(offsetPolygon[indexOfOuter].Count == 4);
      Assert.Contains(new Point64(-10, -10), offsetPolygon[indexOfOuter]);
      Assert.Contains(new Point64(110, -10), offsetPolygon[indexOfOuter]);
      Assert.Contains(new Point64(110, 110), offsetPolygon[indexOfOuter]);
      Assert.Contains(new Point64(-10, 110), offsetPolygon[indexOfOuter]);


      Assert.False(Clipper.IsPositive(offsetPolygon[indexOfHole]));
      Assert.True(offsetPolygon[indexOfHole].Count == 4);
      Assert.Contains(new Point64(20, 20), offsetPolygon[indexOfHole]);
      Assert.Contains(new Point64(80, 20), offsetPolygon[indexOfHole]);
      Assert.Contains(new Point64(80, 80), offsetPolygon[indexOfHole]);
      Assert.Contains(new Point64(20, 80), offsetPolygon[indexOfHole]);
    }
  }
}
