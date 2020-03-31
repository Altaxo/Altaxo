#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2020 Dr. Dirk Lellinger
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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Drawing;
using Altaxo.Drawing.DashPatternManagement;
using Altaxo.Geometry;
using Altaxo.Main;
using Altaxo.Main.Services;
using NUnit.Framework;

namespace Altaxo.Drawing
{
  [TestFixture]
  internal class TestPenX
  {


    public void Tester(PenX pen1, PenX pen2, string comment)
    {
      Assert.IsTrue(pen1 == pen2, comment);
      Assert.IsFalse(pen1 != pen2, comment);
      Assert.IsTrue(pen1.Equals(pen2), comment);
      Assert.IsTrue(pen2.Equals(pen1), comment);
      Assert.IsTrue(object.Equals(pen1, pen2), comment);
      Assert.IsFalse(object.ReferenceEquals(pen1, pen2), comment);
      Assert.AreEqual(pen1.GetHashCode(), pen2.GetHashCode(), comment);
    }

    public void Tester(PenXEnv pen1, PenXEnv pen2, string comment)
    {
      Assert.IsTrue(pen1 == pen2, comment);
      Assert.IsFalse(pen1 != pen2, comment);
      Assert.IsTrue(pen1.Equals(pen2), comment);
      Assert.IsTrue(pen2.Equals(pen1), comment);
      Assert.IsTrue(object.Equals(pen1, pen2), comment);
      Assert.IsFalse(object.ReferenceEquals(pen1, pen2), comment);
      Assert.AreEqual(pen1.GetHashCode(), pen2.GetHashCode(), comment);
    }


    private IEnumerable<(PenX pen1, PenX pen2, string comment)> TestGenerator()
    {
      PenX pen1 = new PenX(NamedColors.Green, 3.5);
      PenX pen2 = new PenX(NamedColors.Green, 3.5);
      yield return (pen1, pen2, "SolidPen");

      pen1 = new PenX(NamedColors.Green, 3.5).WithStartCap(new LineCaps.ArrowF10LineCap(8, 2));
      pen2 = new PenX(NamedColors.Green, 3.5).WithStartCap(new LineCaps.ArrowF10LineCap(8, 2));
      yield return (pen1, pen2, "SolidPen_WithStartCap");

      pen1 = new PenX(NamedColors.Green, 3.5).WithEndCap(new LineCaps.ArrowF20LineCap(8, 2));
      pen2 = new PenX(NamedColors.Green, 3.5).WithEndCap(new LineCaps.ArrowF20LineCap(8, 2));
      yield return (pen1, pen2, "SolidPen_WithEndCap");

      pen1 = new PenX(NamedColors.Green, 3.5).WithDashPattern(DashPatternListManager.Instance.BuiltinDefaultDashDot);
      pen2 = new PenX(NamedColors.Green, 3.5).WithDashPattern(DashPatternListManager.Instance.BuiltinDefaultDashDot);
      yield return (pen1, pen2, "SolidPen_WithDashPattern");

      pen1 = new PenX(NamedColors.Green, 3.5).WithTransformation(new Matrix3x2Class());
      pen2 = new PenX(NamedColors.Green, 3.5).WithTransformation(new Matrix3x2Class());
      yield return (pen1, pen2, "SolidPen_WithTransform");
    }

    [Test]
    public void TestHash_PenX()
    {
      foreach (var entry in TestGenerator())
      {
        Tester(entry.pen1, entry.pen2, entry.comment);
      }
    }

    [Test]
    public void TestHash_PenXEnv()
    {
      foreach (var entry in TestGenerator())
      {
        var env1 = new PenXEnv(entry.pen1, new Geometry.RectangleD2D(0, 0, 1000, 2000), 96);
        var env2 = new PenXEnv(entry.pen2, new Geometry.RectangleD2D(0, 0, 1000, 2000), 96);

        Tester(env1, env2, entry.comment);
      }
    }
  }
}
