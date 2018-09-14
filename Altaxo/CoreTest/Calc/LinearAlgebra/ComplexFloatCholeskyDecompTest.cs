#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using Altaxo.Calc;
using Altaxo.Calc.LinearAlgebra;
using NUnit.Framework;

namespace AltaxoTest.Calc.LinearAlgebra
{
  [TestFixture]
  public class ComplexFloatCholeskyDecompTest
  {
    private static ComplexFloatCholeskyDecomp cd;
    private const double TOLERENCE = 0.001;

    static ComplexFloatCholeskyDecompTest()
    {
      var a = new ComplexFloatMatrix(3)
      {
        [0, 0] = 2,
        [0, 1] = new ComplexFloat(1, -1),
        [0, 2] = 0,
        [1, 0] = new ComplexFloat(1, -1),
        [1, 1] = 2,
        [1, 2] = 0,
        [2, 0] = 0,
        [2, 1] = 0,
        [2, 2] = 3
      };
      cd = new ComplexFloatCholeskyDecomp(a);
    }

    [Test]
    public void CDWide()
    {
      Assert.Throws(typeof(NotSquareMatrixException), () =>
      {
        var wm = new ComplexFloatMatrix(2, 3);
        var wcd = new ComplexFloatCholeskyDecomp(wm);
      });
    }

    [Test]
    public void CDLong()
    {
      Assert.Throws(typeof(NotSquareMatrixException), () =>
      {
        var lm = new ComplexFloatMatrix(3, 2);
        var lcd = new ComplexFloatCholeskyDecomp(lm);
      });
    }

    [Test]
    public void FactorTest()
    {
      Assert.AreEqual(cd.Factor[0, 0].Real, 1.414, TOLERENCE);
      Assert.AreEqual(cd.Factor[0, 1].Real, 0.000, TOLERENCE);
      Assert.AreEqual(cd.Factor[0, 2].Real, 0.000, TOLERENCE);
      Assert.AreEqual(cd.Factor[1, 0].Real, 0.707, TOLERENCE);
      Assert.AreEqual(cd.Factor[1, 0].Imag, -0.707, TOLERENCE);
      Assert.AreEqual(cd.Factor[1, 1].Real, 1.000, TOLERENCE);
      Assert.AreEqual(cd.Factor[1, 2].Real, 0.000, TOLERENCE);
      Assert.AreEqual(cd.Factor[2, 0].Real, 0.000, TOLERENCE);
      Assert.AreEqual(cd.Factor[2, 1].Real, 0.000, TOLERENCE);
      Assert.AreEqual(cd.Factor[2, 2].Real, 1.732, TOLERENCE);
    }

    [Test]
    public void NonSymmFactorTest()
    {
      var b = new ComplexFloatMatrix(3)
      {
        [0, 0] = 2,
        [0, 1] = 1,
        [0, 2] = 1,
        [1, 0] = 1,
        [1, 1] = 2,
        [1, 2] = 0,
        [2, 0] = 0,
        [2, 1] = 0,
        [2, 2] = 3
      };
      var dcd = new ComplexFloatCholeskyDecomp(b);
      Assert.AreEqual(dcd.Factor[0, 0].Real, 1.414, TOLERENCE);
      Assert.AreEqual(dcd.Factor[0, 1].Real, 0.000, TOLERENCE);
      Assert.AreEqual(dcd.Factor[0, 2].Real, 0.000, TOLERENCE);
      Assert.AreEqual(dcd.Factor[1, 0].Real, 0.707, TOLERENCE);
      Assert.AreEqual(dcd.Factor[1, 1].Real, 1.225, TOLERENCE);
      Assert.AreEqual(dcd.Factor[1, 2].Real, 0.000, TOLERENCE);
      Assert.AreEqual(dcd.Factor[2, 0].Real, 0.000, TOLERENCE);
      Assert.AreEqual(dcd.Factor[2, 1].Real, 0.000, TOLERENCE);
      Assert.AreEqual(dcd.Factor[2, 2].Real, 1.732, TOLERENCE);
    }

    [Test]
    public void IsPositiveDefiniteTest()
    {
      Assert.IsTrue(cd.IsPositiveDefinite);
      var b = new ComplexFloatMatrix(3)
      {
        [0, 0] = -2,
        [0, 1] = 1,
        [0, 2] = 0,
        [1, 0] = 1,
        [1, 1] = 2,
        [1, 2] = 0,
        [2, 0] = 0,
        [2, 1] = 0,
        [2, 2] = 3
      };
      var dcd = new ComplexFloatCholeskyDecomp(b);
      Assert.IsFalse(dcd.IsPositiveDefinite);
    }

    [Test]
    public void GetDeterminantTest()
    {
      ComplexFloat det = cd.GetDeterminant();
      Assert.AreEqual(det.Real, 6.000, TOLERENCE);
      Assert.AreEqual(det.Imag, 0.000, TOLERENCE);
    }

    [Test]
    public void SolveMatrix()
    {
      var b = new ComplexFloatMatrix(3)
      {
        [0, 0] = 2,
        [0, 1] = 2,
        [0, 2] = 2,
        [1, 0] = 13,
        [1, 1] = 13,
        [1, 2] = 13,
        [2, 0] = 25,
        [2, 1] = 25,
        [2, 2] = 25
      };
      ComplexFloatMatrix x = cd.Solve(b);
      Assert.AreEqual(x[0, 0].Real, -4.500, TOLERENCE);
      Assert.AreEqual(x[0, 1].Real, -4.500, TOLERENCE);
      Assert.AreEqual(x[0, 2].Real, -4.500, TOLERENCE);
      Assert.AreEqual(x[1, 0].Real, 12.000, TOLERENCE);
      Assert.AreEqual(x[1, 1].Real, 12.000, TOLERENCE);
      Assert.AreEqual(x[1, 2].Real, 12.000, TOLERENCE);
      Assert.AreEqual(x[2, 0].Real, 8.333, TOLERENCE);
      Assert.AreEqual(x[2, 1].Real, 8.333, TOLERENCE);
      Assert.AreEqual(x[2, 2].Real, 8.333, TOLERENCE);
      Assert.AreEqual(x[0, 0].Imag, -6.500, TOLERENCE);
      Assert.AreEqual(x[0, 1].Imag, -6.500, TOLERENCE);
      Assert.AreEqual(x[0, 2].Imag, -6.500, TOLERENCE);
      Assert.AreEqual(x[1, 0].Imag, 1.000, TOLERENCE);
      Assert.AreEqual(x[1, 1].Imag, 1.000, TOLERENCE);
      Assert.AreEqual(x[1, 2].Imag, 1.000, TOLERENCE);
      Assert.AreEqual(x[2, 0].Imag, 0.000, TOLERENCE);
      Assert.AreEqual(x[2, 1].Imag, 0.000, TOLERENCE);
      Assert.AreEqual(x[2, 2].Imag, 0.000, TOLERENCE);

      b = new ComplexFloatMatrix(3, 2)
      {
        [0, 0] = 2,
        [0, 1] = 2,
        [1, 0] = 13,
        [1, 1] = 13,
        [2, 0] = 25,
        [2, 1] = 25
      };
      x = cd.Solve(b);
      Assert.AreEqual(x[0, 0].Real, -4.500, TOLERENCE);
      Assert.AreEqual(x[0, 1].Real, -4.500, TOLERENCE);
      Assert.AreEqual(x[1, 0].Real, 12.000, TOLERENCE);
      Assert.AreEqual(x[1, 1].Real, 12.000, TOLERENCE);
      Assert.AreEqual(x[2, 0].Real, 8.333, TOLERENCE);
      Assert.AreEqual(x[2, 1].Real, 8.333, TOLERENCE);
      Assert.AreEqual(x[0, 0].Imag, -6.500, TOLERENCE);
      Assert.AreEqual(x[0, 1].Imag, -6.500, TOLERENCE);
      Assert.AreEqual(x[1, 0].Imag, 1.000, TOLERENCE);
      Assert.AreEqual(x[1, 1].Imag, 1.000, TOLERENCE);
      Assert.AreEqual(x[2, 0].Imag, 0.000, TOLERENCE);
      Assert.AreEqual(x[2, 1].Imag, 0.000, TOLERENCE);

      b = new ComplexFloatMatrix(3, 4)
      {
        [0, 0] = 2,
        [0, 1] = 2,
        [0, 2] = 2,
        [0, 3] = 2,
        [1, 0] = 13,
        [1, 1] = 13,
        [1, 2] = 13,
        [1, 3] = 13,
        [2, 0] = 25,
        [2, 1] = 25,
        [2, 2] = 25,
        [2, 3] = 25
      };
      x = cd.Solve(b);
      Assert.AreEqual(x[0, 0].Real, -4.500, TOLERENCE);
      Assert.AreEqual(x[0, 1].Real, -4.500, TOLERENCE);
      Assert.AreEqual(x[0, 2].Real, -4.500, TOLERENCE);
      Assert.AreEqual(x[0, 3].Real, -4.500, TOLERENCE);
      Assert.AreEqual(x[1, 0].Real, 12.000, TOLERENCE);
      Assert.AreEqual(x[1, 1].Real, 12.000, TOLERENCE);
      Assert.AreEqual(x[1, 2].Real, 12.000, TOLERENCE);
      Assert.AreEqual(x[1, 3].Real, 12.000, TOLERENCE);
      Assert.AreEqual(x[2, 0].Real, 8.333, TOLERENCE);
      Assert.AreEqual(x[2, 1].Real, 8.333, TOLERENCE);
      Assert.AreEqual(x[2, 2].Real, 8.333, TOLERENCE);
      Assert.AreEqual(x[2, 3].Real, 8.333, TOLERENCE);
      Assert.AreEqual(x[0, 0].Imag, -6.500, TOLERENCE);
      Assert.AreEqual(x[0, 1].Imag, -6.500, TOLERENCE);
      Assert.AreEqual(x[0, 2].Imag, -6.500, TOLERENCE);
      Assert.AreEqual(x[0, 3].Imag, -6.500, TOLERENCE);
      Assert.AreEqual(x[1, 0].Imag, 1.000, TOLERENCE);
      Assert.AreEqual(x[1, 1].Imag, 1.000, TOLERENCE);
      Assert.AreEqual(x[1, 2].Imag, 1.000, TOLERENCE);
      Assert.AreEqual(x[1, 3].Imag, 1.000, TOLERENCE);
      Assert.AreEqual(x[2, 0].Imag, 0.000, TOLERENCE);
      Assert.AreEqual(x[2, 1].Imag, 0.000, TOLERENCE);
      Assert.AreEqual(x[2, 2].Imag, 0.000, TOLERENCE);
      Assert.AreEqual(x[2, 3].Imag, 0.000, TOLERENCE);
    }

    [Test]
    public void SolveVector()
    {
      var b = new ComplexFloatVector(3)
      {
        [0] = 2,
        [1] = 13,
        [2] = 25
      };
      ComplexFloatVector x = cd.Solve(b);
      Assert.AreEqual(x[0].Real, -4.500, TOLERENCE);
      Assert.AreEqual(x[1].Real, 12.000, TOLERENCE);
      Assert.AreEqual(x[2].Real, 8.333, TOLERENCE);
      Assert.AreEqual(x[0].Imag, -6.500, TOLERENCE);
      Assert.AreEqual(x[1].Imag, 1.000, TOLERENCE);
      Assert.AreEqual(x[2].Imag, 0.000, TOLERENCE);
    }

    [Test]
    public void GetInverseTest()
    {
      ComplexFloatMatrix inv = cd.GetInverse();
      Assert.AreEqual(inv[0, 0].Real, 1.000, TOLERENCE);
      Assert.AreEqual(inv[0, 1].Real, -0.500, TOLERENCE);
      Assert.AreEqual(inv[0, 2].Real, 0.000, TOLERENCE);
      Assert.AreEqual(inv[1, 0].Real, -0.500, TOLERENCE);
      Assert.AreEqual(inv[1, 1].Real, 1.000, TOLERENCE);
      Assert.AreEqual(inv[1, 2].Real, 0.000, TOLERENCE);
      Assert.AreEqual(inv[2, 0].Real, 0.000, TOLERENCE);
      Assert.AreEqual(inv[2, 1].Real, 0.000, TOLERENCE);
      Assert.AreEqual(inv[2, 2].Real, 0.333333, TOLERENCE);

      Assert.AreEqual(inv[0, 0].Imag, 0.000, TOLERENCE);
      Assert.AreEqual(inv[0, 1].Imag, -0.500, TOLERENCE);
      Assert.AreEqual(inv[0, 2].Imag, 0.000, TOLERENCE);
      Assert.AreEqual(inv[1, 0].Imag, 0.500, TOLERENCE);
      Assert.AreEqual(inv[1, 1].Imag, 0.000, TOLERENCE);
      Assert.AreEqual(inv[1, 2].Imag, 0.000, TOLERENCE);
      Assert.AreEqual(inv[2, 0].Imag, 0.000, TOLERENCE);
      Assert.AreEqual(inv[2, 1].Imag, 0.000, TOLERENCE);
      Assert.AreEqual(inv[2, 2].Imag, 0.000, TOLERENCE);
    }

    [Test]
    public void GetInverseNotPositiveDefiniteTest()
    {
      Assert.Throws(typeof(NotPositiveDefiniteException), () =>
      {
        var a = new ComplexFloatMatrix(3, 3);
        var dcd = new ComplexFloatCholeskyDecomp(a);
        dcd.GetInverse();
      });
    }
  }
}
