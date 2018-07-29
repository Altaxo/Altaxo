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
    public void Test01_AllSideOut()
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
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0), CSAxisSide.FirstDown));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 1), CSAxisSide.FirstUp));

      cs = new G2DCartesicCoordinateSystem() { IsYReverse = true };
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0), CSAxisSide.FirstDown));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 1), CSAxisSide.FirstUp));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0), CSAxisSide.FirstDown));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 1), CSAxisSide.FirstUp));

      cs = new G2DCartesicCoordinateSystem() { IsXReverse = true, IsYReverse = true };
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0), CSAxisSide.FirstDown));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 1), CSAxisSide.FirstUp));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0), CSAxisSide.FirstDown));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 1), CSAxisSide.FirstUp));

      cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true }; // Normal
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0), CSAxisSide.FirstDown));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 1), CSAxisSide.FirstUp));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0), CSAxisSide.FirstDown));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 1), CSAxisSide.FirstUp));

      cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true, IsXReverse = true };
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0), CSAxisSide.FirstDown));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 1), CSAxisSide.FirstUp));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0), CSAxisSide.FirstDown));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 1), CSAxisSide.FirstUp));

      cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true, IsYReverse = true };
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0), CSAxisSide.FirstDown));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 1), CSAxisSide.FirstUp));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0), CSAxisSide.FirstDown));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 1), CSAxisSide.FirstUp));

      cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true, IsXReverse = true, IsYReverse = true };
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0), CSAxisSide.FirstDown));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 1), CSAxisSide.FirstUp));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0), CSAxisSide.FirstDown));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 1), CSAxisSide.FirstUp));
    }

    [Test]
    public void Test02_AllSideIn()
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
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0), CSAxisSide.FirstUp));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 1), CSAxisSide.FirstDown));

      cs = new G2DCartesicCoordinateSystem() { IsYReverse = true };
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0), CSAxisSide.FirstUp));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 1), CSAxisSide.FirstDown));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0), CSAxisSide.FirstUp));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 1), CSAxisSide.FirstDown));

      cs = new G2DCartesicCoordinateSystem() { IsXReverse = true, IsYReverse = true };
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0), CSAxisSide.FirstUp));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 1), CSAxisSide.FirstDown));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0), CSAxisSide.FirstUp));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 1), CSAxisSide.FirstDown));

      cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true }; // Normal
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0), CSAxisSide.FirstUp));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 1), CSAxisSide.FirstDown));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0), CSAxisSide.FirstUp));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 1), CSAxisSide.FirstDown));

      cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true, IsXReverse = true };
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0), CSAxisSide.FirstUp));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 1), CSAxisSide.FirstDown));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0), CSAxisSide.FirstUp));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 1), CSAxisSide.FirstDown));

      cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true, IsYReverse = true };
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0), CSAxisSide.FirstUp));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 1), CSAxisSide.FirstDown));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0), CSAxisSide.FirstUp));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 1), CSAxisSide.FirstDown));

      cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true, IsXReverse = true, IsYReverse = true };
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0), CSAxisSide.FirstUp));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 1), CSAxisSide.FirstDown));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0), CSAxisSide.FirstUp));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 1), CSAxisSide.FirstDown));
    }

    [Test]
    public void Test03_AllSideRight()
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
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0.1), CSAxisSide.FirstUp));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0.9), CSAxisSide.FirstUp));

      cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true, IsYReverse = true };
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0.1), CSAxisSide.FirstDown));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0.9), CSAxisSide.FirstDown));

      cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true, IsXReverse = true, IsYReverse = true };
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0.1), CSAxisSide.FirstDown));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0.9), CSAxisSide.FirstDown));
    }

    [Test]
    public void Test04_AllSideLeft()
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
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0.1), CSAxisSide.FirstDown));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0.9), CSAxisSide.FirstDown));

      cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true, IsYReverse = true };
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0.1), CSAxisSide.FirstUp));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0.9), CSAxisSide.FirstUp));

      cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true, IsXReverse = true, IsYReverse = true };
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0.1), CSAxisSide.FirstUp));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(0, 0.9), CSAxisSide.FirstUp));
    }

    [Test]
    public void Test05_AllSideAbove()
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
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0.1), CSAxisSide.FirstDown));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0.9), CSAxisSide.FirstDown));

      cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true, IsYReverse = true };
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0.1), CSAxisSide.FirstUp));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0.9), CSAxisSide.FirstUp));

      cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true, IsXReverse = true, IsYReverse = true };
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0.1), CSAxisSide.FirstDown));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0.9), CSAxisSide.FirstDown));
    }

    [Test]
    public void Test06_AllSideBelow()
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
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0.1), CSAxisSide.FirstUp));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0.9), CSAxisSide.FirstUp));

      cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true, IsYReverse = true };
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0.1), CSAxisSide.FirstDown));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0.9), CSAxisSide.FirstDown));

      cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true, IsXReverse = true, IsYReverse = true };
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0.1), CSAxisSide.FirstUp));
      Assert.AreEqual(expectedSide, cs.GetAxisSideName(new CSLineID(1, 0.9), CSAxisSide.FirstUp));
    }

    [Test]
    public void Test07_AllEdgeLeft()
    {
      string expectedSide = "Left";
      var cs = new G2DCartesicCoordinateSystem(); // Normal
      Assert.AreEqual(expectedSide, cs.GetAxisName(new CSLineID(1, 0)));

      cs = new G2DCartesicCoordinateSystem() { IsXReverse = true };
      Assert.AreEqual(expectedSide, cs.GetAxisName(new CSLineID(1, 1)));

      cs = new G2DCartesicCoordinateSystem() { IsYReverse = true };
      Assert.AreEqual(expectedSide, cs.GetAxisName(new CSLineID(1, 0)));

      cs = new G2DCartesicCoordinateSystem() { IsXReverse = true, IsYReverse = true };
      Assert.AreEqual(expectedSide, cs.GetAxisName(new CSLineID(1, 1)));

      cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true }; // Normal
      Assert.AreEqual(expectedSide, cs.GetAxisName(new CSLineID(0, 0)));

      cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true, IsXReverse = true };
      Assert.AreEqual(expectedSide, cs.GetAxisName(new CSLineID(0, 0)));

      cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true, IsYReverse = true };
      Assert.AreEqual(expectedSide, cs.GetAxisName(new CSLineID(0, 1)));

      cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true, IsXReverse = true, IsYReverse = true };
      Assert.AreEqual(expectedSide, cs.GetAxisName(new CSLineID(0, 1)));
    }

    [Test]
    public void Test08_AllEdgeRight()
    {
      string expectedSide = "Right";
      var cs = new G2DCartesicCoordinateSystem(); // Normal
      Assert.AreEqual(expectedSide, cs.GetAxisName(new CSLineID(1, 1)));

      cs = new G2DCartesicCoordinateSystem() { IsXReverse = true };
      Assert.AreEqual(expectedSide, cs.GetAxisName(new CSLineID(1, 0)));

      cs = new G2DCartesicCoordinateSystem() { IsYReverse = true };
      Assert.AreEqual(expectedSide, cs.GetAxisName(new CSLineID(1, 1)));

      cs = new G2DCartesicCoordinateSystem() { IsXReverse = true, IsYReverse = true };
      Assert.AreEqual(expectedSide, cs.GetAxisName(new CSLineID(1, 0)));

      cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true }; // Normal
      Assert.AreEqual(expectedSide, cs.GetAxisName(new CSLineID(0, 1)));

      cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true, IsXReverse = true };
      Assert.AreEqual(expectedSide, cs.GetAxisName(new CSLineID(0, 1)));

      cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true, IsYReverse = true };
      Assert.AreEqual(expectedSide, cs.GetAxisName(new CSLineID(0, 0)));

      cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true, IsXReverse = true, IsYReverse = true };
      Assert.AreEqual(expectedSide, cs.GetAxisName(new CSLineID(0, 0)));
    }

    [Test]
    public void Test09_AllEdgeBottom()
    {
      string expectedSide = "Bottom";
      var cs = new G2DCartesicCoordinateSystem(); // Normal
      Assert.AreEqual(expectedSide, cs.GetAxisName(new CSLineID(0, 0)));

      cs = new G2DCartesicCoordinateSystem() { IsXReverse = true };
      Assert.AreEqual(expectedSide, cs.GetAxisName(new CSLineID(0, 0)));

      cs = new G2DCartesicCoordinateSystem() { IsYReverse = true };
      Assert.AreEqual(expectedSide, cs.GetAxisName(new CSLineID(0, 1)));

      cs = new G2DCartesicCoordinateSystem() { IsXReverse = true, IsYReverse = true };
      Assert.AreEqual(expectedSide, cs.GetAxisName(new CSLineID(0, 1)));

      cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true }; // Normal
      Assert.AreEqual(expectedSide, cs.GetAxisName(new CSLineID(1, 0)));

      cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true, IsXReverse = true };
      Assert.AreEqual(expectedSide, cs.GetAxisName(new CSLineID(1, 1)));

      cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true, IsYReverse = true };
      Assert.AreEqual(expectedSide, cs.GetAxisName(new CSLineID(1, 0)));

      cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true, IsXReverse = true, IsYReverse = true };
      Assert.AreEqual(expectedSide, cs.GetAxisName(new CSLineID(1, 1)));
    }

    [Test]
    public void Test09_AllEdgeTop()
    {
      string expectedSide = "Top";
      var cs = new G2DCartesicCoordinateSystem(); // Normal
      Assert.AreEqual(expectedSide, cs.GetAxisName(new CSLineID(0, 1)));

      cs = new G2DCartesicCoordinateSystem() { IsXReverse = true };
      Assert.AreEqual(expectedSide, cs.GetAxisName(new CSLineID(0, 1)));

      cs = new G2DCartesicCoordinateSystem() { IsYReverse = true };
      Assert.AreEqual(expectedSide, cs.GetAxisName(new CSLineID(0, 0)));

      cs = new G2DCartesicCoordinateSystem() { IsXReverse = true, IsYReverse = true };
      Assert.AreEqual(expectedSide, cs.GetAxisName(new CSLineID(0, 0)));

      cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true }; // Normal
      Assert.AreEqual(expectedSide, cs.GetAxisName(new CSLineID(1, 1)));

      cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true, IsXReverse = true };
      Assert.AreEqual(expectedSide, cs.GetAxisName(new CSLineID(1, 0)));

      cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true, IsYReverse = true };
      Assert.AreEqual(expectedSide, cs.GetAxisName(new CSLineID(1, 1)));

      cs = new G2DCartesicCoordinateSystem() { IsXYInterchanged = true, IsXReverse = true, IsYReverse = true };
      Assert.AreEqual(expectedSide, cs.GetAxisName(new CSLineID(1, 0)));
    }
  }
}
