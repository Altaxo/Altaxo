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
using Altaxo.Calc.LinearAlgebra;
using NUnit.Framework;

namespace AltaxoTest.Calc.LinearAlgebra
{
  [TestFixture]
  public class DoubleCholeskyDecompTest
  {
    private static DoubleCholeskyDecomp cd;
    private const double TOLERENCE = 0.001;

    static DoubleCholeskyDecompTest()
    {
      var a = new DoubleMatrix(3)
      {
        [0, 0] = 2,
        [0, 1] = 1,
        [0, 2] = 0,
        [1, 0] = 1,
        [1, 1] = 2,
        [1, 2] = 0,
        [2, 0] = 0,
        [2, 1] = 0,
        [2, 2] = 3
      };
      cd = new DoubleCholeskyDecomp(a);
    }

    [Test]
    public void CDWide()
    {
      Assert.Throws(typeof(NotSquareMatrixException), () =>
      {
        var wm = new DoubleMatrix(2, 3);
        var cd = new DoubleCholeskyDecomp(wm);
      });
    }

    [Test]
    public void CDLong()
    {
      Assert.Throws(typeof(NotSquareMatrixException), () =>
      {
        var lm = new DoubleMatrix(3, 2);
        var lcd = new DoubleCholeskyDecomp(lm);
      });
    }

    [Test]
    public void FactorTest()
    {
      Assert.AreEqual(cd.Factor[0, 0], 1.414, TOLERENCE);
      Assert.AreEqual(cd.Factor[0, 1], 0);
      Assert.AreEqual(cd.Factor[0, 2], 0);
      Assert.AreEqual(cd.Factor[1, 0], 0.707, TOLERENCE);
      Assert.AreEqual(cd.Factor[1, 1], 1.225, TOLERENCE);
      Assert.AreEqual(cd.Factor[1, 2], 0);
      Assert.AreEqual(cd.Factor[2, 0], 0);
      Assert.AreEqual(cd.Factor[2, 1], 0);
      Assert.AreEqual(cd.Factor[2, 2], 1.732, TOLERENCE);
    }

    [Test]
    public void NonSymmFactorTest()
    {
      var b = new DoubleMatrix(3)
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
      var dcd = new DoubleCholeskyDecomp(b);
      Assert.AreEqual(dcd.Factor[0, 0], 1.414, TOLERENCE);
      Assert.AreEqual(dcd.Factor[0, 1], 0.000, TOLERENCE);
      Assert.AreEqual(dcd.Factor[0, 2], 0.000, TOLERENCE);
      Assert.AreEqual(dcd.Factor[1, 0], 0.707, TOLERENCE);
      Assert.AreEqual(dcd.Factor[1, 1], 1.225, TOLERENCE);
      Assert.AreEqual(dcd.Factor[1, 2], 0.000, TOLERENCE);
      Assert.AreEqual(dcd.Factor[2, 0], 0.000, TOLERENCE);
      Assert.AreEqual(dcd.Factor[2, 1], 0.000, TOLERENCE);
      Assert.AreEqual(dcd.Factor[2, 2], 1.732, TOLERENCE);
    }

    [Test]
    public void IsPositiveDefiniteTest()
    {
      Assert.IsTrue(cd.IsPositiveDefinite);
      var b = new DoubleMatrix(3)
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
      var dcd = new DoubleCholeskyDecomp(b);
      Assert.IsFalse(dcd.IsPositiveDefinite);
    }

    [Test]
    public void GetDeterminantTest()
    {
      double det = cd.GetDeterminant();
      Assert.AreEqual(det, 9.000, TOLERENCE);
    }

    [Test]
    public void SolveMatrix()
    {
      var b = new DoubleMatrix(3)
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
      DoubleMatrix x = cd.Solve(b);
      Assert.AreEqual(x[0, 0], -3.000, TOLERENCE);
      Assert.AreEqual(x[0, 1], -3.000, TOLERENCE);
      Assert.AreEqual(x[0, 2], -3.000, TOLERENCE);
      Assert.AreEqual(x[1, 0], 8.000, TOLERENCE);
      Assert.AreEqual(x[1, 1], 8.000, TOLERENCE);
      Assert.AreEqual(x[1, 2], 8.000, TOLERENCE);
      Assert.AreEqual(x[2, 0], 8.333, TOLERENCE);
      Assert.AreEqual(x[2, 1], 8.333, TOLERENCE);
      Assert.AreEqual(x[2, 2], 8.333, TOLERENCE);

      b = new DoubleMatrix(3, 2)
      {
        [0, 0] = 2,
        [0, 1] = 2,
        [1, 0] = 13,
        [1, 1] = 13,
        [2, 0] = 25,
        [2, 1] = 25
      };
      x = cd.Solve(b);
      Assert.AreEqual(x[0, 0], -3.000, TOLERENCE);
      Assert.AreEqual(x[0, 1], -3.000, TOLERENCE);
      Assert.AreEqual(x[1, 0], 8.000, TOLERENCE);
      Assert.AreEqual(x[1, 1], 8.000, TOLERENCE);
      Assert.AreEqual(x[2, 0], 8.333, TOLERENCE);
      Assert.AreEqual(x[2, 1], 8.333, TOLERENCE);

      b = new DoubleMatrix(3, 4)
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
      Assert.AreEqual(x[0, 0], -3.000, TOLERENCE);
      Assert.AreEqual(x[0, 1], -3.000, TOLERENCE);
      Assert.AreEqual(x[0, 2], -3.000, TOLERENCE);
      Assert.AreEqual(x[0, 3], -3.000, TOLERENCE);
      Assert.AreEqual(x[1, 0], 8.000, TOLERENCE);
      Assert.AreEqual(x[1, 1], 8.000, TOLERENCE);
      Assert.AreEqual(x[1, 2], 8.000, TOLERENCE);
      Assert.AreEqual(x[1, 3], 8.000, TOLERENCE);
      Assert.AreEqual(x[2, 0], 8.333, TOLERENCE);
      Assert.AreEqual(x[2, 1], 8.333, TOLERENCE);
      Assert.AreEqual(x[2, 2], 8.333, TOLERENCE);
      Assert.AreEqual(x[2, 3], 8.333, TOLERENCE);
    }

    [Test]
    public void SolveVector()
    {
      var b = new DoubleVector(3)
      {
        [0] = 2,
        [1] = 13,
        [2] = 25
      };
      DoubleVector x = cd.Solve(b);
      Assert.AreEqual(x[0], -3.000, TOLERENCE);
      Assert.AreEqual(x[1], 8.000, TOLERENCE);
      Assert.AreEqual(x[2], 8.333, TOLERENCE);
    }

    [Test]
    public void GetInverseTest()
    {
      DoubleMatrix inv = cd.GetInverse();
      Assert.AreEqual(inv[0, 0], 0.666667, TOLERENCE);
      Assert.AreEqual(inv[0, 1], -0.333333, TOLERENCE);
      Assert.AreEqual(inv[0, 2], 0.000, TOLERENCE);
      Assert.AreEqual(inv[1, 0], -0.333333, TOLERENCE);
      Assert.AreEqual(inv[1, 1], 0.666667, TOLERENCE);
      Assert.AreEqual(inv[1, 2], 0.000, TOLERENCE);
      Assert.AreEqual(inv[2, 0], 0.000, TOLERENCE);
      Assert.AreEqual(inv[2, 1], 0.000, TOLERENCE);
      Assert.AreEqual(inv[2, 2], 0.333333, TOLERENCE);
    }

    [Test]
    public void GetInverseNotPositiveDefiniteTest()
    {
      Assert.Throws(typeof(NotPositiveDefiniteException), () =>
      {
        var a = new DoubleMatrix(3, 3);
        var dcd = new DoubleCholeskyDecomp(a);
        dcd.GetInverse();
      });
    }
  }
}
