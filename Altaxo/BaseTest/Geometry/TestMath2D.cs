using Altaxo.Geometry;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseTest.Geometry
{
	[TestFixture]
	public class TestMath2D
	{
		#region Polygon area

		[Test]
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
			Assert.AreEqual(10000, area);
		}

		[Test]
		public static void Test_PolygonArea02_Nothing()
		{
			var testPoints = new PointD2D[0];
			var area = Math2D.PolygonArea(testPoints);
			Assert.AreEqual(0, area);
		}

		[Test]
		public static void Test_PolygonArea03_OnePoint()
		{
			var testPoints = new PointD2D[]
				{
				new PointD2D(50,50 ),
			};
			var area = Math2D.PolygonArea(testPoints);
			Assert.AreEqual(0, area);
		}

		[Test]
		public static void Test_PolygonArea04_OneLine()
		{
			var testPoints = new PointD2D[]
				{
				new PointD2D(50,50 ),
				new PointD2D(100,-100)
			};
			var area = Math2D.PolygonArea(testPoints);
			Assert.AreEqual(0, area);
		}

		[Test]
		public static void Test_PolygonArea04_OneTriangle()
		{
			var testPoints = new PointD2D[]
				{
				new PointD2D(0,100 ),
				new PointD2D(-100,0),
				new PointD2D(100,0)
			};
			var area = Math2D.PolygonArea(testPoints);
			Assert.AreEqual(10000, area);
		}

		#endregion Polygon area

		#region Flood fill

		[Test]
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

			Assert.AreEqual(19, pixelsWith0);
		}

		#endregion Flood fill
	}
}