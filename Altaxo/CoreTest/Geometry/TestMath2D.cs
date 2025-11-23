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

using Xunit;

namespace Altaxo.Geometry
{

  public class TestMath2D
  {
    #region Polygon area

    [Fact]
    public static void Test_PolygonArea01_Rectangle()
    {
      var testPoints = new PointD2D[]
      {
        new PointD2D(50,50 ),
        new PointD2D(-50, 50),
        new PointD2D(-50, -50),
        new PointD2D(50,- 50)
      };

      var area = Math2D.PolygonArea(testPoints);
      Assert.Equal(10000, area);
    }

    [Fact]
    public static void Test_PolygonArea02_Nothing()
    {
      var testPoints = new PointD2D[0];
      var area = Math2D.PolygonArea(testPoints);
      Assert.Equal(0, area);
    }

    [Fact]
    public static void Test_PolygonArea03_OnePoint()
    {
      var testPoints = new PointD2D[]
        {
        new PointD2D(50,50 ),
      };
      var area = Math2D.PolygonArea(testPoints);
      Assert.Equal(0, area);
    }

    [Fact]
    public static void Test_PolygonArea04_OneLine()
    {
      var testPoints = new PointD2D[]
        {
        new PointD2D(50,50 ),
        new PointD2D(100,-100)
      };
      var area = Math2D.PolygonArea(testPoints);
      Assert.Equal(0, area);
    }

    [Fact]
    public static void Test_PolygonArea04_OneTriangle()
    {
      var testPoints = new PointD2D[]
        {
        new PointD2D(0,100 ),
        new PointD2D(-100,0),
        new PointD2D(100,0)
      };
      var area = Math2D.PolygonArea(testPoints);
      Assert.Equal(10000, area);
    }

    #endregion Polygon area

    #region Flood fill

    [Fact]
    public static void Test_FloodFill_01()
    {
      int[][] field = new int[][]
        {
        new int[]{1,1,1,1,1,1,1,1,1,1},
        new int[]{1,1,1,1,1,0,1,1,1,1},
        new int[]{1,1,1,1,0,0,0,1,1,1},
        new int[]{1,1,0,0,0,0,1,1,1,1},
        new int[]{1,1,0,0,0,1,1,1,1,1},
        new int[]{1,0,1,1,0,0,1,1,1,1},
        new int[]{1,1,0,1,0,0,0,0,1,1},
        new int[]{1,1,0,1,1,1,1,0,0,1},
        new int[]{1,1,0,1,1,1,1,1,1,1},
        new int[]{1,1,1,1,1,1,1,1,1,1},
        };

      int pixelsWith0 = 0;
      Math2D.FloodFill_4Neighbour(
        5,
        5,
        (x, y) => field[y][x] == 0, (x, y) => ++pixelsWith0,
        0, 0, 10, 10
        );

      Assert.Equal(19, pixelsWith0);
    }

    #endregion Flood fill
  }
}
