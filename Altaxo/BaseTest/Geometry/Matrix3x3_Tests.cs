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
      var rotOrg = new VectorD3D(0.25, Math.PI / 2, 0.125);
      var m = Matrix3x3.FromRotationRadian(rotOrg.X, rotOrg.Y, rotOrg.Z);
      var rot = m.DecomposeIntoRotations();
      var ms = Matrix3x3.FromRotationRadian(rot.X, rot.Y, rot.Z);

      AssertEx.AreEqual(0.25 + 0.125, rot.X, 1E-8, 1E-8);
      AssertEx.AreEqual(Math.PI / 2, rot.Y, 1E-8, 1E-8);
      AssertEx.AreEqual(0, rot.Z, 1E-8, 1E-8);
    }

    [Fact]
    public void TestDecomposition1()
    {
      var m = Matrix3x3.FromScaleShearRotationRadian(7, 11, 13, 1 / 2d, 1 / 3d, 1 / 5d, Math.PI / 12, Math.PI / 6, Math.PI / 4);
      var (scale, shear, rot) = m.DecomposeIntoScaleShearRotationRadian();

      AssertEx.AreEqual(1 / 2d, shear.X, 1E-8, 1E-8);
      AssertEx.AreEqual(1 / 3d, shear.Y, 1E-8, 1E-8);
      AssertEx.AreEqual(1 / 5d, shear.Z, 1E-8, 1E-8);

      AssertEx.AreEqual(7.0, scale.X, 1E-8, 1E-8);
      AssertEx.AreEqual(11.0, scale.Y, 1E-8, 1E-8);
      AssertEx.AreEqual(13.0, scale.Z, 1E-8, 1E-8);

      AssertEx.AreEqual(Math.PI / 12, rot.X, 1E-8, 1E-8);
      AssertEx.AreEqual(Math.PI / 6, rot.Y, 1E-8, 1E-8);
      AssertEx.AreEqual(Math.PI / 4, rot.Z, 1E-8, 1E-8);
    }


    [Fact]
    public void TestQRDecomposition()
    {
      var m = new Matrix3x3(1, 1, 0, 1, 0, 1, 0, 1, 1);
      var (q, r) = m.DecomposeIntoQR();

      AssertEx.AreEqual(1 / Math.Sqrt(2), q.M11, 1E-8, 1E-8);
      AssertEx.AreEqual(1 / Math.Sqrt(6), q.M12, 1E-8, 1E-8);
      AssertEx.AreEqual(-1 / Math.Sqrt(3), q.M13, 1E-8, 1E-8);

      AssertEx.AreEqual(1 / Math.Sqrt(2), q.M21, 1E-8, 1E-8);
      AssertEx.AreEqual(-1 / Math.Sqrt(6), q.M22, 1E-8, 1E-8);
      AssertEx.AreEqual(1 / Math.Sqrt(3), q.M23, 1E-8, 1E-8);

      AssertEx.AreEqual(0, q.M31, 1E-8, 1E-8);
      AssertEx.AreEqual(2 / Math.Sqrt(6), q.M32, 1E-8, 1E-8);
      AssertEx.AreEqual(1 / Math.Sqrt(3), q.M33, 1E-8, 1E-8);

      // Test R

      AssertEx.AreEqual(2 / Math.Sqrt(2), r.M11, 1E-8, 1E-8);
      AssertEx.AreEqual(1 / Math.Sqrt(2), r.M12, 1E-8, 1E-8);
      AssertEx.AreEqual(1 / Math.Sqrt(2), r.M13, 1E-8, 1E-8);

      AssertEx.AreEqual(0, r.M21, 1E-8, 1E-8);
      AssertEx.AreEqual(3 / Math.Sqrt(6), r.M22, 1E-8, 1E-8);
      AssertEx.AreEqual(1 / Math.Sqrt(6), r.M23, 1E-8, 1E-8);

      AssertEx.AreEqual(0, r.M31, 1E-8, 1E-8);
      AssertEx.AreEqual(0, r.M32, 1E-8, 1E-8);
      AssertEx.AreEqual(2 / Math.Sqrt(3), r.M33, 1E-8, 1E-8);
    }

    [Fact]
    public void TestRQDecomposition()
    {
      var m = new Matrix3x3(1, 1, 0, 1, 0, 1, 0, 1, 1);

      // the R-Q decomposition is the same as the QR decomposition
      // of the inverse matrix, and then invert the resulting q and r matrices
      var minv = m.Inverse;
      var (qinv1, rinv1) = minv.DecomposeIntoQR();
      var q1 = qinv1.Inverse;
      var r1 = rinv1.Inverse;

      var (r2, q2) = m.DecomposeIntoRQ();

      AssertEx.AreEqual(q1.M11, q2.M11, 1E-8, 1E-8);
      AssertEx.AreEqual(q1.M12, q2.M12, 1E-8, 1E-8);
      AssertEx.AreEqual(q1.M13, q2.M13, 1E-8, 1E-8);
      AssertEx.AreEqual(q1.M21, q2.M21, 1E-8, 1E-8);
      AssertEx.AreEqual(q1.M22, q2.M22, 1E-8, 1E-8);
      AssertEx.AreEqual(q1.M23, q2.M23, 1E-8, 1E-8);
      AssertEx.AreEqual(q1.M31, q2.M31, 1E-8, 1E-8);
      AssertEx.AreEqual(q1.M32, q2.M32, 1E-8, 1E-8);
      AssertEx.AreEqual(q1.M33, q2.M33, 1E-8, 1E-8);

      AssertEx.AreEqual(r1.M11, r2.M11, 1E-8, 1E-8);
      AssertEx.AreEqual(r1.M12, r2.M12, 1E-8, 1E-8);
      AssertEx.AreEqual(r1.M13, r2.M13, 1E-8, 1E-8);
      AssertEx.AreEqual(r1.M21, r2.M21, 1E-8, 1E-8);
      AssertEx.AreEqual(r1.M22, r2.M22, 1E-8, 1E-8);
      AssertEx.AreEqual(r1.M23, r2.M23, 1E-8, 1E-8);
      AssertEx.AreEqual(r1.M31, r2.M31, 1E-8, 1E-8);
      AssertEx.AreEqual(r1.M32, r2.M32, 1E-8, 1E-8);
      AssertEx.AreEqual(r1.M33, r2.M33, 1E-8, 1E-8);

    }
  }
}
