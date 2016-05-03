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

using Altaxo.Geometry;
using Altaxo.Graph.Graph3D;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Geometry
{
	using TD = Tuple<PointD3D, VectorD3D, VectorD3D>;

	[TestFixture]
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

		private static Tuple<PointD3D[], TD[]>[] _testCases = new Tuple<PointD3D[], TD[]>[]
		{
			new Tuple<PointD3D[], TD[]>(_input00, _output00),
			new Tuple<PointD3D[], TD[]>(_input01, _output01),
			new Tuple<PointD3D[], TD[]>(_input02, _output02),
			new Tuple<PointD3D[], TD[]>(_input03, _output03),
			new Tuple<PointD3D[], TD[]>(_input04, _output04),
			new Tuple<PointD3D[], TD[]>(_input05, _output05),
			new Tuple<PointD3D[], TD[]>(_input06, _output06),
		};

		[Test]
		public static void Test_GetPolylinePointsWithWestAndNorth_01()
		{
			const double maxDev = 1E-6;
			for (int caseNo = 0; caseNo < _testCases.Length; ++caseNo)
			{
				var points = _testCases[caseNo].Item1;
				var expectedOutput = _testCases[caseNo].Item2;

				var result = Math3D.GetPolylinePointsWithWestAndNorth(points).ToArray();

				// Verify results
				Assert.AreEqual(expectedOutput.Length, result.Length);

				for (int i = 0; i < expectedOutput.Length; ++i)
				{
					Assert.AreEqual(expectedOutput[i].Item1, result[i].Item1);

					Assert.AreEqual(expectedOutput[i].Item2.X, result[i].Item2.X, maxDev);
					Assert.AreEqual(expectedOutput[i].Item2.Y, result[i].Item2.Y, maxDev);
					Assert.AreEqual(expectedOutput[i].Item2.Z, result[i].Item2.Z, maxDev);

					Assert.AreEqual(expectedOutput[i].Item3.X, result[i].Item3.X, maxDev);
					Assert.AreEqual(expectedOutput[i].Item3.Y, result[i].Item3.Y, maxDev);
					Assert.AreEqual(expectedOutput[i].Item3.Z, result[i].Item3.Z, maxDev);
				}
			}
		}

		[Test]
		public static void Test_GetPolylinePointsWithWestAndNorth_02()
		{
			const double maxDev = 1E-6;
			var rnd = new System.Random();

			PointD3D[] testPoints = new PointD3D[1024];

			testPoints[0] = PointD3D.Empty;
			testPoints[1] = new PointD3D(0, 0.1, 0); // first line segment always in y direction, so that north is in z direction and west in -x direction
			for (int i = 2; i < testPoints.Length; ++i)
			{
				testPoints[i] = new PointD3D(rnd.NextDouble(), rnd.NextDouble(), rnd.NextDouble());
			}

			var result = Math3D.GetPolylinePointsWithWestAndNorth(testPoints).ToArray();

			for (int i = 1; i < result.Length; ++i)
			{
				var forwardRaw = result[i].Item1 - result[i - 1].Item1;
				var west = result[i].Item2;
				var north = result[i].Item3;

				Assert.AreNotEqual(forwardRaw.Length, 0); // GetPolylinePointsWithWestAndNorth should only deliver non-empty segments
				var forward = forwardRaw.Normalized;

				Assert.AreEqual(west.Length, 1, maxDev); // is west normalized
				Assert.AreEqual(north.Length, 1, maxDev); // is north normalized
				Assert.AreEqual(VectorD3D.DotProduct(west, forward), 0, maxDev); // is west perpendicular to forward
				Assert.AreEqual(VectorD3D.DotProduct(north, forward), 0, maxDev); // is north perpendicular to forward
				Assert.AreEqual(VectorD3D.DotProduct(west, north), 0, maxDev); // is west perpendicular to north
				var matrix = Altaxo.Geometry.Matrix4x3.NewFromBasisVectorsAndLocation(west, north, forward, PointD3D.Empty);
				Assert.AreEqual(matrix.Determinant, 1, maxDev); // west-north-forward are a right handed coordinate system
			}
		}

		[Test]
		public static void Test_GetWestNorthVectors_01()
		{
			const double maxDev = 1E-6;

			for (int iTheta = -89; iTheta < 90; ++iTheta)
			{
				double theta = Math.PI * iTheta / 180.0;
				for (int iPhi = 0; iPhi < 360; ++iPhi)
				{
					double phi = Math.PI * iPhi / 180.0;
					var v = new VectorD3D(Math.Cos(phi) * Math.Cos(theta), Math.Sin(phi) * Math.Cos(theta), Math.Sin(theta));
					Assert.AreEqual(v.Length, 1, maxDev); // is forward normalized

					var rawNorth = Math3D.GetRawNorthVectorAtStart(v);
					Assert.AreEqual(rawNorth.X, 0);
					Assert.AreEqual(rawNorth.Y, 0);
					Assert.AreEqual(rawNorth.Z, 1);

					var westNorth = Math3D.GetWestNorthVectors(new LineD3D(PointD3D.Empty, (PointD3D)v));

					var west = westNorth.Item1;
					var north = westNorth.Item2;

					Assert.AreEqual(west.Length, 1, maxDev); // is west normalized
					Assert.AreEqual(north.Length, 1, maxDev); // is north normalized
					Assert.AreEqual(VectorD3D.DotProduct(west, v), 0, maxDev); // is west perpendicular to forward
					Assert.AreEqual(VectorD3D.DotProduct(north, v), 0, maxDev); // is north perpendicular to forward
					Assert.AreEqual(VectorD3D.DotProduct(west, north), 0, maxDev); // is west perpendicular to north
					var matrix = Altaxo.Geometry.Matrix4x3.NewFromBasisVectorsAndLocation(west, north, v, PointD3D.Empty);
					Assert.AreEqual(matrix.Determinant, 1, maxDev);

					var westExpected = new VectorD3D(-Math.Sin(phi), Math.Cos(phi), 0);
					var northExpected = new VectorD3D(-Math.Cos(phi) * Math.Sin(theta), -Math.Sin(phi) * Math.Sin(theta), Math.Cos(theta));

					Assert.AreEqual(westExpected.X, west.X, maxDev);
					Assert.AreEqual(westExpected.Y, west.Y, maxDev);
					Assert.AreEqual(westExpected.Z, west.Z, maxDev);

					Assert.AreEqual(northExpected.X, north.X, maxDev);
					Assert.AreEqual(northExpected.Y, north.Y, maxDev);
					Assert.AreEqual(northExpected.Z, north.Z, maxDev);
				}
			}
		}

		[Test]
		public static void Test_GetWestNorthVectors_02()
		{
			const double maxDev = 1E-6;

			for (int i = -1; i <= 1; i += 2)
			{
				var v = new VectorD3D(0, 0, i);
				var westNorth = Math3D.GetWestNorthVectors(new LineD3D(PointD3D.Empty, (PointD3D)v));
				var west = westNorth.Item1;
				var north = westNorth.Item2;

				Assert.AreEqual(west.Length, 1, maxDev); // is west normalized
				Assert.AreEqual(north.Length, 1, maxDev); // is north normalized
				Assert.AreEqual(VectorD3D.DotProduct(west, v), 0, maxDev); // is west perpendicular to forward
				Assert.AreEqual(VectorD3D.DotProduct(north, v), 0, maxDev); // is north perpendicular to forward
				Assert.AreEqual(VectorD3D.DotProduct(west, north), 0, maxDev); // is west perpendicular to north
				var matrix = Altaxo.Geometry.Matrix4x3.NewFromBasisVectorsAndLocation(west, north, v, PointD3D.Empty);
				Assert.AreEqual(matrix.Determinant, 1, maxDev);

				var westExpected = new VectorD3D(-1, 0, 0);
				var northExpected = new VectorD3D(0, -i, 0);

				Assert.AreEqual(westExpected.X, west.X, maxDev);
				Assert.AreEqual(westExpected.Y, west.Y, maxDev);
				Assert.AreEqual(westExpected.Z, west.Z, maxDev);

				Assert.AreEqual(northExpected.X, north.X, maxDev);
				Assert.AreEqual(northExpected.Y, north.Y, maxDev);
				Assert.AreEqual(northExpected.Z, north.Z, maxDev);
			}
		}
	}
}