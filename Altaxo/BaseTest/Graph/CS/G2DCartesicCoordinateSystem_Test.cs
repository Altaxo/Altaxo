using Altaxo.Graph;
using Altaxo.Graph.Gdi.CS;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AltaxoTest.Graph.CS
{
	[TestFixture]
	public class G2DCartesicCoordinateSystem_Test
	{
		[Test]
		public void Test01_AllOut()
		{
			string expectedSide = "Out";
			var cs = new G2DCartesicCoordinateSystem(); // Normal
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0), CSAxisSide.FirstDown));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 1), CSAxisSide.FirstUp));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0), CSAxisSide.FirstDown));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 1), CSAxisSide.FirstUp));

			cs = new G2DCartesicCoordinateSystem() { IsXReverse = true };
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0), CSAxisSide.FirstDown));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 1), CSAxisSide.FirstUp));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0), CSAxisSide.FirstUp));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 1), CSAxisSide.FirstDown));

			cs = new G2DCartesicCoordinateSystem() { IsYReverse = true };
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0), CSAxisSide.FirstUp));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 1), CSAxisSide.FirstDown));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0), CSAxisSide.FirstDown));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 1), CSAxisSide.FirstUp));

			cs = new G2DCartesicCoordinateSystem() { IsXReverse = true, IsYReverse = true };
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0), CSAxisSide.FirstUp));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 1), CSAxisSide.FirstDown));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0), CSAxisSide.FirstUp));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 1), CSAxisSide.FirstDown));

			cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true }; // Normal
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0), CSAxisSide.FirstDown));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 1), CSAxisSide.FirstUp));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0), CSAxisSide.FirstDown));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 1), CSAxisSide.FirstUp));

			cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true, IsXReverse = true };
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0), CSAxisSide.FirstDown));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 1), CSAxisSide.FirstUp));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0), CSAxisSide.FirstUp));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 1), CSAxisSide.FirstDown));

			cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true, IsYReverse = true };
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0), CSAxisSide.FirstUp));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 1), CSAxisSide.FirstDown));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0), CSAxisSide.FirstDown));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 1), CSAxisSide.FirstUp));

			cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true, IsXReverse = true, IsYReverse = true };
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0), CSAxisSide.FirstUp));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 1), CSAxisSide.FirstDown));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0), CSAxisSide.FirstUp));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 1), CSAxisSide.FirstDown));
		}

		[Test]
		public void Test02_AllIn()
		{
			string expectedSide = "In";
			var cs = new G2DCartesicCoordinateSystem(); // Normal
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0), CSAxisSide.FirstUp));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 1), CSAxisSide.FirstDown));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0), CSAxisSide.FirstUp));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 1), CSAxisSide.FirstDown));

			cs = new G2DCartesicCoordinateSystem() { IsXReverse = true };
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0), CSAxisSide.FirstUp));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 1), CSAxisSide.FirstDown));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0), CSAxisSide.FirstDown));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 1), CSAxisSide.FirstUp));

			cs = new G2DCartesicCoordinateSystem() { IsYReverse = true };
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0), CSAxisSide.FirstDown));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 1), CSAxisSide.FirstUp));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0), CSAxisSide.FirstUp));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 1), CSAxisSide.FirstDown));

			cs = new G2DCartesicCoordinateSystem() { IsXReverse = true, IsYReverse = true };
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0), CSAxisSide.FirstDown));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 1), CSAxisSide.FirstUp));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0), CSAxisSide.FirstDown));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 1), CSAxisSide.FirstUp));

			cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true }; // Normal
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0), CSAxisSide.FirstUp));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 1), CSAxisSide.FirstDown));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0), CSAxisSide.FirstUp));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 1), CSAxisSide.FirstDown));

			cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true, IsXReverse = true };
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0), CSAxisSide.FirstUp));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 1), CSAxisSide.FirstDown));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0), CSAxisSide.FirstDown));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 1), CSAxisSide.FirstUp));

			cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true, IsYReverse = true };
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0), CSAxisSide.FirstDown));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 1), CSAxisSide.FirstUp));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0), CSAxisSide.FirstUp));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 1), CSAxisSide.FirstDown));

			cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true, IsXReverse = true, IsYReverse = true };
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0), CSAxisSide.FirstDown));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 1), CSAxisSide.FirstUp));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0), CSAxisSide.FirstDown));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 1), CSAxisSide.FirstUp));
		}

		[Test]
		public void Test03_AllRight()
		{
			string expectedSide = "Right";
			var cs = new G2DCartesicCoordinateSystem(); // Normal
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0.1), CSAxisSide.FirstUp));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0.9), CSAxisSide.FirstUp));

			cs = new G2DCartesicCoordinateSystem() { IsXReverse = true };
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0.1), CSAxisSide.FirstDown));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0.9), CSAxisSide.FirstDown));

			cs = new G2DCartesicCoordinateSystem() { IsYReverse = true };
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0.1), CSAxisSide.FirstUp));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0.9), CSAxisSide.FirstUp));

			cs = new G2DCartesicCoordinateSystem() { IsXReverse = true, IsYReverse = true };
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0.1), CSAxisSide.FirstDown));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0.9), CSAxisSide.FirstDown));

			cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true }; // Normal
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0.1), CSAxisSide.FirstUp));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0.9), CSAxisSide.FirstUp));

			cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true, IsXReverse = true };
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0.1), CSAxisSide.FirstDown));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0.9), CSAxisSide.FirstDown));

			cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true, IsYReverse = true };
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0.1), CSAxisSide.FirstUp));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0.9), CSAxisSide.FirstUp));

			cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true, IsXReverse = true, IsYReverse = true };
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0.1), CSAxisSide.FirstDown));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0.9), CSAxisSide.FirstDown));
		}

		[Test]
		public void Test04_AllLeft()
		{
			string expectedSide = "Left";
			var cs = new G2DCartesicCoordinateSystem(); // Normal
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0.1), CSAxisSide.FirstDown));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0.9), CSAxisSide.FirstDown));

			cs = new G2DCartesicCoordinateSystem() { IsXReverse = true };
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0.1), CSAxisSide.FirstUp));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0.9), CSAxisSide.FirstUp));

			cs = new G2DCartesicCoordinateSystem() { IsYReverse = true };
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0.1), CSAxisSide.FirstDown));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0.9), CSAxisSide.FirstDown));

			cs = new G2DCartesicCoordinateSystem() { IsXReverse = true, IsYReverse = true };
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0.1), CSAxisSide.FirstUp));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0.9), CSAxisSide.FirstUp));

			cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true }; // Normal
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0.1), CSAxisSide.FirstDown));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0.9), CSAxisSide.FirstDown));

			cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true, IsXReverse = true };
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0.1), CSAxisSide.FirstUp));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0.9), CSAxisSide.FirstUp));

			cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true, IsYReverse = true };
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0.1), CSAxisSide.FirstDown));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0.9), CSAxisSide.FirstDown));

			cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true, IsXReverse = true, IsYReverse = true };
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0.1), CSAxisSide.FirstUp));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0.9), CSAxisSide.FirstUp));
		}

		[Test]
		public void Test05_AllAbove()
		{
			string expectedSide = "Above";
			var cs = new G2DCartesicCoordinateSystem(); // Normal
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0.1), CSAxisSide.FirstUp));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0.9), CSAxisSide.FirstUp));

			cs = new G2DCartesicCoordinateSystem() { IsXReverse = true };
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0.1), CSAxisSide.FirstUp));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0.9), CSAxisSide.FirstUp));

			cs = new G2DCartesicCoordinateSystem() { IsYReverse = true };
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0.1), CSAxisSide.FirstDown));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0.9), CSAxisSide.FirstDown));

			cs = new G2DCartesicCoordinateSystem() { IsXReverse = true, IsYReverse = true };
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0.1), CSAxisSide.FirstDown));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0.9), CSAxisSide.FirstDown));

			cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true }; // Normal
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0.1), CSAxisSide.FirstUp));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0.9), CSAxisSide.FirstUp));

			cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true, IsXReverse = true };
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0.1), CSAxisSide.FirstUp));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0.9), CSAxisSide.FirstUp));

			cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true, IsYReverse = true };
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0.1), CSAxisSide.FirstDown));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0.9), CSAxisSide.FirstDown));

			cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true, IsXReverse = true, IsYReverse = true };
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0.1), CSAxisSide.FirstDown));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0.9), CSAxisSide.FirstDown));
		}

		[Test]
		public void Test06_AllBelow()
		{
			string expectedSide = "Below";
			var cs = new G2DCartesicCoordinateSystem(); // Normal
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0.1), CSAxisSide.FirstDown));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0.9), CSAxisSide.FirstDown));

			cs = new G2DCartesicCoordinateSystem() { IsXReverse = true };
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0.1), CSAxisSide.FirstDown));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0.9), CSAxisSide.FirstDown));

			cs = new G2DCartesicCoordinateSystem() { IsYReverse = true };
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0.1), CSAxisSide.FirstUp));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0.9), CSAxisSide.FirstUp));

			cs = new G2DCartesicCoordinateSystem() { IsXReverse = true, IsYReverse = true };
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0.1), CSAxisSide.FirstUp));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0.9), CSAxisSide.FirstUp));

			cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true }; // Normal
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0.1), CSAxisSide.FirstDown));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0.9), CSAxisSide.FirstDown));

			cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true, IsXReverse = true };
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0.1), CSAxisSide.FirstDown));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0.9), CSAxisSide.FirstDown));

			cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true, IsYReverse = true };
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0.1), CSAxisSide.FirstUp));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0.9), CSAxisSide.FirstUp));

			cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true, IsXReverse = true, IsYReverse = true };
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0.1), CSAxisSide.FirstUp));
			Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0.9), CSAxisSide.FirstUp));
		}
	}
}