#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2021 Dr. Dirk Lellinger
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
using System.Threading.Tasks;
using Xunit;

namespace Altaxo.Geometry
{
  public class Matrix3x3_Tests
  {
    [Fact]
    public void TestRotation1()
    {
      var rotOrg = new VectorD3D(0.25, 0.375, 0.125);
      var m = Matrix3x3.FromRotationRadian(rotOrg.X, rotOrg.Y, rotOrg.Z);
      var rot = m.DecomposeIntoRotations();

      AssertEx.AreEqual(0.25, rot.X, 1E-8, 1E-8);
      AssertEx.AreEqual(0.375, rot.Y, 1E-8, 1E-8);
      AssertEx.AreEqual(0.125, rot.Z, 1E-8, 1E-8);

      var ms = Matrix3x3.FromRotationRadian(rot.X, rot.Y, rot.Z);
    }
    [Fact]
    public void TestRotation2()
    {
      var rotOrg = new VectorD3D(0.25, Math.PI/2, 0.125);
      var m = Matrix3x3.FromRotationRadian(rotOrg.X, rotOrg.Y, rotOrg.Z);
      var rot = m.DecomposeIntoRotations();
      var ms = Matrix3x3.FromRotationRadian(rot.X, rot.Y, rot.Z);

      AssertEx.AreEqual(0.25+0.125, rot.X, 1E-8, 1E-8);
      AssertEx.AreEqual(Math.PI/2, rot.Y, 1E-8, 1E-8);
      AssertEx.AreEqual(0, rot.Z, 1E-8, 1E-8);
    }

    [Fact]
    public void TestDecomposition1()
    {
      var m = Matrix3x3.FromScaleShearRotationRadian(7, 11, 13, 1 / 2d, 1 / 3d, 1 / 5d, Math.PI / 12, Math.PI / 6, Math.PI / 4);
      var (scale, shear, rot) = m.DecomposeIntoScaleShearRotationRadian();

      AssertEx.AreEqual(1/2d, shear.X, 1E-8, 1E-8);
      AssertEx.AreEqual(1/3d, shear.Y, 1E-8, 1E-8);
      AssertEx.AreEqual(1/5d, shear.Z, 1E-8, 1E-8);

      AssertEx.AreEqual(7.0, scale.X, 1E-8, 1E-8);
      AssertEx.AreEqual(11.0, scale.Y, 1E-8, 1E-8);
      AssertEx.AreEqual(13.0, scale.Z, 1E-8, 1E-8);

      AssertEx.AreEqual(Math.PI/12, rot.X, 1E-8, 1E-8); 
      AssertEx.AreEqual(Math.PI/6, rot.Y, 1E-8, 1E-8);
      AssertEx.AreEqual(Math.PI/4, rot.Z, 1E-8, 1E-8);
    }
  }
}
