#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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

using Altaxo.Graph3D;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseTest.Graph
{
	[TestFixture]
	public class TestMatrixD3D
	{
		private static System.Random _random = new Random(432);

		public static MatrixD3D GetRndMatrix()
		{
			Func<int> r = () => _random.Next(1, 100);
			return new MatrixD3D(r(), r(), r(), r(), r(), r(), r(), r(), r(), r(), r(), r());
		}

		public static PointD3D GetRndPoint()
		{
			Func<int> r = () => _random.Next(1, 100);
			return new PointD3D(r(), r(), r());
		}

		public static VectorD3D GetRndVector()
		{
			Func<int> r = () => _random.Next(1, 100);
			return new VectorD3D(r(), r(), r());
		}

		[Test]
		public static void Test_AppendTransformMatrix_WithPoints()
		{
			for (int i = 0; i < 5; ++i)
			{
				var m1 = GetRndMatrix();
				var m2 = GetRndMatrix();

				var p1 = GetRndPoint();

				var p2 = m1.Transform(p1);

				var p3 = m2.Transform(p2);

				var m3 = m1;
				m3.AppendTransform(m2);

				var p3s = m3.Transform(p1);

				Assert.AreEqual(p3.X, p3s.X, 0);
				Assert.AreEqual(p3.Y, p3s.Y, 0);
				Assert.AreEqual(p3.Z, p3s.Z, 0);
			}
		}

		[Test]
		public static void Test_PrependTransformMatrix_WithPoints()
		{
			for (int i = 0; i < 5; ++i)
			{
				var m1 = GetRndMatrix();
				var m2 = GetRndMatrix();

				var p1 = GetRndPoint();

				var p2 = m1.Transform(p1);

				var p3 = m2.Transform(p2);

				var m3 = m2;
				m3.PrependTransform(m1);

				var p3s = m3.Transform(p1);

				Assert.AreEqual(p3.X, p3s.X, 0);
				Assert.AreEqual(p3.Y, p3s.Y, 0);
				Assert.AreEqual(p3.Z, p3s.Z, 0);
			}
		}

		[Test]
		public static void Test_AppendTransformMatrix_WithVectors()
		{
			for (int i = 0; i < 5; ++i)
			{
				var m1 = GetRndMatrix();
				var m2 = GetRndMatrix();

				var p1 = GetRndVector();

				var p2 = m1.Transform(p1);

				var p3 = m2.Transform(p2);

				var m3 = m1;
				m3.AppendTransform(m2);

				var p3s = m3.Transform(p1);

				Assert.AreEqual(p3.X, p3s.X, 0);
				Assert.AreEqual(p3.Y, p3s.Y, 0);
				Assert.AreEqual(p3.Z, p3s.Z, 0);
			}
		}

		[Test]
		public static void Test_PrependTransformMatrix_WithVectors()
		{
			for (int i = 0; i < 5; ++i)
			{
				var m1 = GetRndMatrix();
				var m2 = GetRndMatrix();

				var p1 = GetRndVector();

				var p2 = m1.Transform(p1);

				var p3 = m2.Transform(p2);

				var m3 = m2;
				m3.PrependTransform(m1);

				var p3s = m3.Transform(p1);

				Assert.AreEqual(p3.X, p3s.X, 0);
				Assert.AreEqual(p3.Y, p3s.Y, 0);
				Assert.AreEqual(p3.Z, p3s.Z, 0);
			}
		}
	}
}