#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using Altaxo.Geometry;
using Altaxo.Graph.Graph3D;
using Xunit;

namespace Altaxo.Geometry
{
  using TD = Tuple<PointD3D, VectorD3D, VectorD3D>;


  public class TestMath3D
  {
    // Straight line in y-direction
    private static PointD3D[] _input00 = new PointD3D[]
    {
      new PointD3D(7, 0, 11),
      new PointD3D(7, 1, 11)
    };

    private static TD[] _output00 = new TD[]
    {
      new TD( new PointD3D(7,0,11), new VectorD3D(-1,0,0), new VectorD3D(0,0,1)),
      new TD( new PointD3D(7,1,11), new VectorD3D(-1,0,0), new VectorD3D(0,0,1))
    };

    // Straight line in y-direction with coincident start point
    private static PointD3D[] _input01 = new PointD3D[]
    {
      new PointD3D(7, 0, 11),
      new PointD3D(7, 0, 11),
      new PointD3D(7, 1, 11)
    };

    private static TD[] _output01 = new TD[]
    {
      new TD( new PointD3D(7,0,11), new VectorD3D(-1,0,0), new VectorD3D(0,0,1)),
      new TD( new PointD3D(7,1,11), new VectorD3D(-1,0,0), new VectorD3D(0,0,1))
    };

    // Straight line in y-direction with coincident end point
    private static PointD3D[] _input02 = new PointD3D[]
    {
      new PointD3D(7, 0, 11),
      new PointD3D(7, 1, 11),
      new PointD3D(7, 1, 11)
    };

    private static TD[] _output02 = new TD[]
    {
      new TD( new PointD3D(7,0,11), new VectorD3D(-1,0,0), new VectorD3D(0,0,1)),
      new TD( new PointD3D(7,1,11), new VectorD3D(-1,0,0), new VectorD3D(0,0,1))
    };

    // Straight line in y-direction, then in z-direction
    private static PointD3D[] _input03 = new PointD3D[]
    {
      new PointD3D(7, 3, 11),
      new PointD3D(7, 5, 11),
      new PointD3D(7, 5, 23)
    };

    private static TD[] _output03 = new TD[]
    {
      new TD( new PointD3D(7, 3, 11), new VectorD3D(-1,0,0), new VectorD3D(0,0,1)),
      new TD( new PointD3D(7, 5, 11), new VectorD3D(-1,0,0), new VectorD3D(0,0,1)),
      new TD( new PointD3D(7, 5, 23), new VectorD3D(-1,0,0), new VectorD3D(0,-1,0))
    };

    // Straight line in y-direction, then exactly back
    private static PointD3D[] _input04 = new PointD3D[]
    {
      new PointD3D(7, 3, 11),
      new PointD3D(7, 5, 11),
      new PointD3D(7, 3, 11)
    };

    private static TD[] _output04 = new TD[]
    {
      new TD( new PointD3D(7, 3, 11), new VectorD3D(-1,0,0), new VectorD3D(0,0,1)),
      new TD( new PointD3D(7, 5, 11), new VectorD3D(-1,0,0), new VectorD3D(0,0,1)),
      new TD( new PointD3D(7, 3, 11), new VectorD3D(1,0,0), new VectorD3D(0,0,1))
    };

    // Straight line in y-direction, then in x-direction
    private static PointD3D[] _input05 = new PointD3D[]
    {
      new PointD3D(7, 3, 11),
      new PointD3D(7, 5, 11),
      new PointD3D(13, 5, 11)
    };

    private static TD[] _output05 = new TD[]
    {
      new TD( new PointD3D(7, 3, 11), new VectorD3D(-1,0,0), new VectorD3D(0,0,1)),
      new TD( new PointD3D(7, 5, 11), new VectorD3D(-1,0,0), new VectorD3D(0,0,1)),
      new TD( new PointD3D(13, 5, 11), new VectorD3D(0,1,0), new VectorD3D(0,0,1))
    };

    // Test 06: Straight line in y-direction, then continue in y-Direction
    private static PointD3D[] _input06 = new PointD3D[]
    {
      new PointD3D(7, 3, 11),
      new PointD3D(7, 5, 11),
      new PointD3D(7, 17, 11)
    };

    private static TD[] _output06 = new TD[]
    {
      new TD( new PointD3D(7, 3, 11), new VectorD3D(-1,0,0), new VectorD3D(0,0,1)),
      new TD( new PointD3D(7, 5, 11), new VectorD3D(-1,0,0), new VectorD3D(0,0,1)),
      new TD( new PointD3D(7, 17, 11), new VectorD3D(-1,0,0), new VectorD3D(0,0,1))
    };

    // Test 07: Straight line in x-direction, then in x-y-z direction, then again straight in x-direction
    private static PointD3D[] _input07 = new PointD3D[]
    {
      new PointD3D(-100, 0, 0),
      new PointD3D(0, 0, 0),
      new PointD3D(100, 100, 100),
      new PointD3D(200, 100, 100)
    };

    private static TD[] _output07 = new TD[]
  {
      new TD( new PointD3D(-100, 0, 0), new VectorD3D(0,1,0), new VectorD3D(0,0,1)),
      new TD( new PointD3D(0, 0, 0), new VectorD3D(0,1,0), new VectorD3D(0,0,1)),
      new TD( new PointD3D(100, 100, 100), new VectorD3D(-0.5773502691896258,0.7886751345948129,-0.2113248654051871), new VectorD3D(-0.5773502691896258,-0.2113248654051871,0.7886751345948129)),
      new TD( new PointD3D(200, 100, 100), new VectorD3D(0,1,0), new VectorD3D(0,0,1))
  };

    // Test 0^8: Straight line in x-direction, then in reverse x  direction, then again straight in x-direction
    private static PointD3D[] _input08 = new PointD3D[]
    {
      new PointD3D(0, 0, 0),
      new PointD3D(100, 0, 0),
      new PointD3D(50, 0, 0),
      new PointD3D(200, 0, 0)
    };

    private static TD[] _output08 = new TD[]
  {
      new TD( new PointD3D(0, 0, 0), new VectorD3D(0,1,0), new VectorD3D(0,0,1)),
      new TD( new PointD3D(100, 0, 0), new VectorD3D(0,1,0), new VectorD3D(0,0,1)),
      new TD( new PointD3D(50, 0, 0), new VectorD3D(0,-1,0), new VectorD3D(0,0,1)),
      new TD( new PointD3D(200, 0, 0), new VectorD3D(0,1,0), new VectorD3D(0,0,1))
  };

    // Test 09: Straight line in y-direction, then in reverse y  direction, then again straight in y-direction
    private static PointD3D[] _input09 = new PointD3D[]
    {
      new PointD3D(0, 0, 0),
      new PointD3D(0, 100, 0),
      new PointD3D(0, -150, 0),
      new PointD3D(0, 200, 0)
    };

    private static TD[] _output09 = new TD[]
{
      new TD( new PointD3D(0, 0, 0), new VectorD3D(-1,0,0), new VectorD3D(0,0,1)),
      new TD( new PointD3D(0, 100, 0), new VectorD3D(-1,0,0), new VectorD3D(0,0,1)),
      new TD( new PointD3D(0, -150, 0), new VectorD3D(1,0,0), new VectorD3D(0,0,1)),
      new TD( new PointD3D(0, 200, 0), new VectorD3D(-1,0,0), new VectorD3D(0,0,1))
};

    // Test 10: Straight line in z-direction, then in reverse z  direction, then again straight in z-direction
    private static PointD3D[] _input10 = new PointD3D[]
    {
      new PointD3D(0, 0, 0),
      new PointD3D(0, 0, 100),
      new PointD3D(0, 0, -150),
      new PointD3D(0, 0, 200)
    };

    private static TD[] _output10 = new TD[]
{
      new TD( new PointD3D(0, 0, 0), new VectorD3D(-1,0,0), new VectorD3D(0,-1,0)),
      new TD( new PointD3D(0, 0, 100), new VectorD3D(-1,0,0), new VectorD3D(0,-1,0)),
      new TD( new PointD3D(0, 0, -150), new VectorD3D(1,0,0), new VectorD3D(0,-1,0)),
      new TD( new PointD3D(0, 0, 200), new VectorD3D(-1,0,0), new VectorD3D(0,-1,0))
};

    // Test 10: Straight line in x-y-z-direction, then in reverse direction, then again straight in x-y-z-direction
    private static PointD3D[] _input11 = new PointD3D[]
    {
      new PointD3D(0, 0, 0),
      new PointD3D(100, 100, 100),
      new PointD3D(-150, -150, -150),
      new PointD3D(200, 200, 200)
    };

    private static TD[] _output11 = new TD[]
{
      new TD( new PointD3D(0, 0, 0), new VectorD3D(-0.707106781186,0.707106781186,0), new VectorD3D(-0.40824829046,-0.40824829046, 0.8164965809277)),
      new TD( new PointD3D(100, 100, 100), new VectorD3D(-0.707106781186,0.707106781186,0), new VectorD3D(-0.40824829046,-0.40824829046, 0.8164965809277)),
      new TD( new PointD3D(-150, -150, -150), new VectorD3D(0.707106781186,-0.707106781186,0), new VectorD3D(-0.40824829046,-0.40824829046, 0.8164965809277)),
      new TD( new PointD3D(200, 200, 200), new VectorD3D(-0.707106781186,0.707106781186,0), new VectorD3D(-0.40824829046,-0.40824829046, 0.8164965809277)),
};

    private static Tuple<PointD3D[], TD[]>[] _testCases = new Tuple<PointD3D[], TD[]>[]
    {
      new Tuple<PointD3D[], TD[]>(_input00, _output00),
      new Tuple<PointD3D[], TD[]>(_input01, _output01),
      new Tuple<PointD3D[], TD[]>(_input02, _output02),
      new Tuple<PointD3D[], TD[]>(_input03, _output03),
      new Tuple<PointD3D[], TD[]>(_input04, _output04),
      new Tuple<PointD3D[], TD[]>(_input05, _output05),
      new Tuple<PointD3D[], TD[]>(_input06, _output06),
      new Tuple<PointD3D[], TD[]>(_input07, _output07),
      new Tuple<PointD3D[], TD[]>(_input08, _output08),
      new Tuple<PointD3D[], TD[]>(_input09, _output09),
      new Tuple<PointD3D[], TD[]>(_input10, _output10),
      new Tuple<PointD3D[], TD[]>(_input11, _output11),
    };

    [Fact]
    public static void Test_GetPolylinePointsWithWestAndNorth_01()
    {
      const int maxDev = 6;
      for (int caseNo = 0; caseNo < _testCases.Length; ++caseNo)
      {
        var points = _testCases[caseNo].Item1;
        var expectedOutput = _testCases[caseNo].Item2;

        var result = PolylineMath3D.GetPolylinePointsWithWestAndNorth(points).ToArray();

        // Verify results
        Assert.Equal(expectedOutput.Length, result.Length);

        for (int i = 0; i < expectedOutput.Length; ++i)
        {
          string comment = string.Format("In case no. {0}, i={1}", caseNo, i);

          Assert.Equal(expectedOutput[i].Item1, result[i].Position);

          Assert.Equal(expectedOutput[i].Item2.X, result[i].WestVector.X, maxDev);
          Assert.Equal(expectedOutput[i].Item2.Y, result[i].WestVector.Y, maxDev);
          Assert.Equal(expectedOutput[i].Item2.Z, result[i].WestVector.Z, maxDev);

          Assert.Equal(expectedOutput[i].Item3.X, result[i].NorthVector.X, maxDev);
          Assert.Equal(expectedOutput[i].Item3.Y, result[i].NorthVector.Y, maxDev);
          Assert.Equal(expectedOutput[i].Item3.Z, result[i].NorthVector.Z, maxDev);
        }
      }
    }

    [Fact]
    public static void Test_GetPolylinePointsWithWestAndNorth_02()
    {
      const int maxDev = 6;
      var rnd = new System.Random();

      var testPoints = new PointD3D[1024];

      testPoints[0] = PointD3D.Empty;
      testPoints[1] = new PointD3D(0, 0.1, 0); // first line segment always in y direction, so that north is in z direction and west in -x direction
      for (int i = 2; i < testPoints.Length; ++i)
      {
        testPoints[i] = new PointD3D(rnd.NextDouble(), rnd.NextDouble(), rnd.NextDouble());
      }

      var result = PolylineMath3D.GetPolylinePointsWithWestAndNorth(testPoints).ToArray();

      for (int i = 1; i < result.Length; ++i)
      {
        var forwardRaw = result[i].Position - result[i - 1].Position;
        var west = result[i].WestVector;
        var north = result[i].NorthVector;

        Assert.NotEqual(0, forwardRaw.Length); // GetPolylinePointsWithWestAndNorth should only deliver non-empty segments
        var forward = forwardRaw.Normalized;

        Assert.Equal(1, west.Length, maxDev); // is west normalized
        Assert.Equal(1, north.Length, maxDev); // is north normalized
        Assert.Equal(0, VectorD3D.DotProduct(west, forward), maxDev); // is west perpendicular to forward
        Assert.Equal(0, VectorD3D.DotProduct(north, forward), maxDev); // is north perpendicular to forward
        Assert.Equal(0, VectorD3D.DotProduct(west, north), maxDev); // is west perpendicular to north
        var matrix = Altaxo.Geometry.Matrix4x3.NewFromBasisVectorsAndLocation(west, north, forward, PointD3D.Empty);
        Assert.Equal(1, matrix.Determinant, maxDev); // west-north-forward are a right handed coordinate system
      }
    }

    [Fact]
    public static void Test_GetWestNorthVectors_01()
    {
      const int maxDev = 6;

      for (int iTheta = -89; iTheta < 90; ++iTheta)
      {
        double theta = Math.PI * iTheta / 180.0;
        for (int iPhi = 0; iPhi < 360; ++iPhi)
        {
          double phi = Math.PI * iPhi / 180.0;
          var v = new VectorD3D(Math.Cos(phi) * Math.Cos(theta), Math.Sin(phi) * Math.Cos(theta), Math.Sin(theta));
          Assert.Equal(1, v.Length, maxDev); // is forward normalized

          var rawNorth = PolylineMath3D.GetRawNorthVectorAtStart(v);
          Assert.Equal(0, rawNorth.X);
          Assert.Equal(0, rawNorth.Y);
          Assert.Equal(1, rawNorth.Z);

          var westNorth = PolylineMath3D.GetWestNorthVectors(new LineD3D(PointD3D.Empty, (PointD3D)v));

          var west = westNorth.Item1;
          var north = westNorth.Item2;

          Assert.Equal(1, west.Length, maxDev); // is west normalized
          Assert.Equal(1, north.Length, maxDev); // is north normalized
          Assert.Equal(0, VectorD3D.DotProduct(west, v), maxDev); // is west perpendicular to forward
          Assert.Equal(0, VectorD3D.DotProduct(north, v), maxDev); // is north perpendicular to forward
          Assert.Equal(0, VectorD3D.DotProduct(west, north), maxDev); // is west perpendicular to north
          var matrix = Altaxo.Geometry.Matrix4x3.NewFromBasisVectorsAndLocation(west, north, v, PointD3D.Empty);
          Assert.Equal(1, matrix.Determinant, maxDev);

          var westExpected = new VectorD3D(-Math.Sin(phi), Math.Cos(phi), 0);
          var northExpected = new VectorD3D(-Math.Cos(phi) * Math.Sin(theta), -Math.Sin(phi) * Math.Sin(theta), Math.Cos(theta));

          Assert.Equal(westExpected.X, west.X, maxDev);
          Assert.Equal(westExpected.Y, west.Y, maxDev);
          Assert.Equal(westExpected.Z, west.Z, maxDev);

          Assert.Equal(northExpected.X, north.X, maxDev);
          Assert.Equal(northExpected.Y, north.Y, maxDev);
          Assert.Equal(northExpected.Z, north.Z, maxDev);
        }
      }
    }

    [Fact]
    public static void Test_GetWestNorthVectors_02()
    {
      const int maxDev = 6;

      for (int i = -1; i <= 1; i += 2)
      {
        var v = new VectorD3D(0, 0, i);
        var westNorth = PolylineMath3D.GetWestNorthVectors(new LineD3D(PointD3D.Empty, (PointD3D)v));
        var west = westNorth.Item1;
        var north = westNorth.Item2;

        Assert.Equal(1, west.Length, maxDev); // is west normalized
        Assert.Equal(1, north.Length, maxDev); // is north normalized
        Assert.Equal(0, VectorD3D.DotProduct(west, v), maxDev); // is west perpendicular to forward
        Assert.Equal(0, VectorD3D.DotProduct(north, v), maxDev); // is north perpendicular to forward
        Assert.Equal(0, VectorD3D.DotProduct(west, north), maxDev); // is west perpendicular to north
        var matrix = Altaxo.Geometry.Matrix4x3.NewFromBasisVectorsAndLocation(west, north, v, PointD3D.Empty);
        Assert.Equal(1, matrix.Determinant, maxDev);

        var westExpected = new VectorD3D(-1, 0, 0);
        var northExpected = new VectorD3D(0, -i, 0);

        Assert.Equal(westExpected.X, west.X, maxDev);
        Assert.Equal(westExpected.Y, west.Y, maxDev);
        Assert.Equal(westExpected.Z, west.Z, maxDev);

        Assert.Equal(northExpected.X, north.X, maxDev);
        Assert.Equal(northExpected.Y, north.Y, maxDev);
        Assert.Equal(northExpected.Z, north.Z, maxDev);
      }
    }

    [Fact]
    public static void Test_GetFractionalPolyline_01()
    {
      const int maxDev = 6;
      string comment = string.Empty;
      for (int caseNo = 0; caseNo < _testCases.Length; ++caseNo)
      {
        if (caseNo == 1 || caseNo == 2) // not with coincidenting points
          continue;

        var points = _testCases[caseNo].Item1;
        var expectedOutput = _testCases[caseNo].Item2;

        var maxEndIndex = points.Length - 1;

        for (double endIndex = 0.25; endIndex <= maxEndIndex; endIndex += 0.25)
        {
          for (double startIndex = 0; startIndex < endIndex; startIndex += 0.25)
          {
            var result = PolylineMath3D.GetPolylineWithFractionalStartAndEndIndex(
              points,
              expectedOutput[0].Item2, expectedOutput[0].Item3, (points[1] - points[0]).Normalized, startIndex, endIndex,
              false,
              false,
              new PolylinePointD3DAsClass(),
              false,
              false,
              new PolylinePointD3DAsClass()).ToArray();

            int iShift = (int)Math.Floor(startIndex);
            for (int i = (int)Math.Ceiling(startIndex); i < (int)Math.Floor(endIndex); ++i)
            {
              comment = string.Format("In case no. {0}, startIndex={1}, endIndex={2}, i={3}", caseNo, startIndex, endIndex, i);

              Assert.Equal(expectedOutput[i].Item1, result[i - iShift].Position);

              Assert.Equal(expectedOutput[i].Item2.X, result[i - iShift].WestVector.X, maxDev);
              Assert.Equal(expectedOutput[i].Item2.Y, result[i - iShift].WestVector.Y, maxDev);
              Assert.Equal(expectedOutput[i].Item2.Z, result[i - iShift].WestVector.Z, maxDev);

              Assert.Equal(expectedOutput[i].Item3.X, result[i - iShift].NorthVector.X, maxDev);
              Assert.Equal(expectedOutput[i].Item3.Y, result[i - iShift].NorthVector.Y, maxDev);
              Assert.Equal(expectedOutput[i].Item3.Z, result[i - iShift].NorthVector.Z, maxDev);
            }

            // start
            int startIndexInt = (int)Math.Floor(startIndex);
            double startIndexFrac = startIndex - startIndexInt;
            var expectedStartPoint = startIndexFrac == 0 ? points[startIndexInt] : PointD3D.Interpolate(points[startIndexInt], points[startIndexInt + 1], startIndexFrac);
            int vecIndex = startIndexFrac == 0 ? startIndexInt : startIndexInt + 1;

            comment = string.Format("In case no. {0}, startIndex={1}, endIndex={2}", caseNo, startIndex, endIndex);

            Assert.Equal(expectedStartPoint, result[0].Position);

            Assert.Equal(expectedOutput[vecIndex].Item2.X, result[0].WestVector.X, maxDev);
            Assert.Equal(expectedOutput[vecIndex].Item2.Y, result[0].WestVector.Y, maxDev);
            Assert.Equal(expectedOutput[vecIndex].Item2.Z, result[0].WestVector.Z, maxDev);

            Assert.Equal(expectedOutput[vecIndex].Item3.X, result[0].NorthVector.X, maxDev);
            Assert.Equal(expectedOutput[vecIndex].Item3.Y, result[0].NorthVector.Y, maxDev);
            Assert.Equal(expectedOutput[vecIndex].Item3.Z, result[0].NorthVector.Z, maxDev);

            // end
            int endIndexInt = (int)Math.Floor(endIndex);
            double endIndexFrac = endIndex - endIndexInt;
            var expectedEndPoint = endIndexFrac == 0 ? points[endIndexInt] : PointD3D.Interpolate(points[endIndexInt], points[endIndexInt + 1], endIndexFrac);
            vecIndex = endIndexFrac == 0 ? endIndexInt : endIndexInt + 1;
            var resultLast = result[result.Length - 1];

            Assert.Equal(expectedEndPoint, resultLast.Position);

            Assert.Equal(expectedOutput[vecIndex].Item2.X, resultLast.WestVector.X, maxDev);
            Assert.Equal(expectedOutput[vecIndex].Item2.Y, resultLast.WestVector.Y, maxDev);
            Assert.Equal(expectedOutput[vecIndex].Item2.Z, resultLast.WestVector.Z, maxDev);

            Assert.Equal(expectedOutput[vecIndex].Item3.X, resultLast.NorthVector.X, maxDev);
            Assert.Equal(expectedOutput[vecIndex].Item3.Y, resultLast.NorthVector.Y, maxDev);
            Assert.Equal(expectedOutput[vecIndex].Item3.Z, resultLast.NorthVector.Z, maxDev);

            // test first returned
          }
        }
      }
    }
  }
}
