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
using Xunit;

namespace AltaxoTest.Calc.LinearAlgebra
{

  public class FloatCholeskyDecompTest
  {
    private FloatCholeskyDecomp cd;
    private const double TOLERANCE = 0.001;

    public FloatCholeskyDecompTest()
    {
      var a = new FloatMatrix(3)
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
      cd = new FloatCholeskyDecomp(a);
    }

    [Fact]
    public void CDWide()
    {
      Assert.Throws<NotSquareMatrixException>(() =>
      {
        var wm = new FloatMatrix(2, 3);
        var wcd = new FloatCholeskyDecomp(wm);
      });
    }

    [Fact]
    public void CDLong()
    {
      Assert.Throws<NotSquareMatrixException>(() =>
      {
        var lm = new FloatMatrix(3, 2);
        var lcd = new FloatCholeskyDecomp(lm);
      });
    }

    [Fact]
    public void FactorTest()
    {
      AssertEx.Equal(cd.Factor[0, 0], 1.414, TOLERANCE);
      AssertEx.Equal(cd.Factor[0, 1], 0.000, TOLERANCE);
      AssertEx.Equal(cd.Factor[0, 2], 0.000, TOLERANCE);
      AssertEx.Equal(cd.Factor[1, 0], 0.707, TOLERANCE);
      AssertEx.Equal(cd.Factor[1, 1], 1.225, TOLERANCE);
      AssertEx.Equal(cd.Factor[1, 2], 0.000, TOLERANCE);
      AssertEx.Equal(cd.Factor[2, 0], 0.000, TOLERANCE);
      AssertEx.Equal(cd.Factor[2, 1], 0.000, TOLERANCE);
      AssertEx.Equal(cd.Factor[2, 2], 1.732, TOLERANCE);
    }

    [Fact]
    public void NonSymmFactorTest()
    {
      var b = new FloatMatrix(3)
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
      var dcd = new FloatCholeskyDecomp(b);
      AssertEx.Equal(dcd.Factor[0, 0], 1.414, TOLERANCE);
      AssertEx.Equal(dcd.Factor[0, 1], 0.000, TOLERANCE);
      AssertEx.Equal(dcd.Factor[0, 2], 0.000, TOLERANCE);
      AssertEx.Equal(dcd.Factor[1, 0], 0.707, TOLERANCE);
      AssertEx.Equal(dcd.Factor[1, 1], 1.225, TOLERANCE);
      AssertEx.Equal(dcd.Factor[1, 2], 0.000, TOLERANCE);
      AssertEx.Equal(dcd.Factor[2, 0], 0.000, TOLERANCE);
      AssertEx.Equal(dcd.Factor[2, 1], 0.000, TOLERANCE);
      AssertEx.Equal(dcd.Factor[2, 2], 1.732, TOLERANCE);
    }

    [Fact]
    public void IsPositiveDefiniteTest()
    {
      Assert.True(cd.IsPositiveDefinite);
      var b = new FloatMatrix(3)
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
      var dcd = new FloatCholeskyDecomp(b);
      Assert.False(dcd.IsPositiveDefinite);
    }

    [Fact]
    public void GetDeterminantTest()
    {
      double det = cd.GetDeterminant();
      AssertEx.Equal(det, 9, TOLERANCE);
    }

    [Fact]
    public void SolveMatrix()
    {
      var b = new FloatMatrix(3)
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
      FloatMatrix x = cd.Solve(b);
      AssertEx.Equal(x[0, 0], -3.000, TOLERANCE);
      AssertEx.Equal(x[0, 1], -3.000, TOLERANCE);
      AssertEx.Equal(x[0, 2], -3.000, TOLERANCE);
      AssertEx.Equal(x[1, 0], 8.000, TOLERANCE);
      AssertEx.Equal(x[1, 1], 8.000, TOLERANCE);
      AssertEx.Equal(x[1, 2], 8.000, TOLERANCE);
      AssertEx.Equal(x[2, 0], 8.333, TOLERANCE);
      AssertEx.Equal(x[2, 1], 8.333, TOLERANCE);
      AssertEx.Equal(x[2, 2], 8.333, TOLERANCE);

      b = new FloatMatrix(3, 2)
      {
        [0, 0] = 2,
        [0, 1] = 2,
        [1, 0] = 13,
        [1, 1] = 13,
        [2, 0] = 25,
        [2, 1] = 25
      };
      x = cd.Solve(b);
      AssertEx.Equal(x[0, 0], -3.000, TOLERANCE);
      AssertEx.Equal(x[0, 1], -3.000, TOLERANCE);
      AssertEx.Equal(x[1, 0], 8.000, TOLERANCE);
      AssertEx.Equal(x[1, 1], 8.000, TOLERANCE);
      AssertEx.Equal(x[2, 0], 8.333, TOLERANCE);
      AssertEx.Equal(x[2, 1], 8.333, TOLERANCE);

      b = new FloatMatrix(3, 4)
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
      AssertEx.Equal(x[0, 0], -3, TOLERANCE);
      AssertEx.Equal(x[0, 1], -3, TOLERANCE);
      AssertEx.Equal(x[0, 2], -3, TOLERANCE);
      AssertEx.Equal(x[0, 3], -3, TOLERANCE);
      AssertEx.Equal(x[1, 0], 8, TOLERANCE);
      AssertEx.Equal(x[1, 1], 8, TOLERANCE);
      AssertEx.Equal(x[1, 2], 8, TOLERANCE);
      AssertEx.Equal(x[1, 3], 8, TOLERANCE);
      AssertEx.Equal(x[2, 0], 8.333, TOLERANCE);
      AssertEx.Equal(x[2, 1], 8.333, TOLERANCE);
      AssertEx.Equal(x[2, 2], 8.333, TOLERANCE);
      AssertEx.Equal(x[2, 3], 8.333, TOLERANCE);
    }

    [Fact]
    public void SolveVector()
    {
      var b = new FloatVector(3)
      {
        [0] = 2,
        [1] = 13,
        [2] = 25
      };
      FloatVector x = cd.Solve(b);
      AssertEx.Equal(x[0], -3, TOLERANCE);
      AssertEx.Equal(x[1], 8, TOLERANCE);
      AssertEx.Equal(x[2], 8.333, TOLERANCE);
    }

    [Fact]
    public void GetInverseTest()
    {
      FloatMatrix inv = cd.GetInverse();
      AssertEx.Equal(inv[0, 0], 0.666667, TOLERANCE);
      AssertEx.Equal(inv[0, 1], -0.333333, TOLERANCE);
      Assert.Equal(0, inv[0, 2]);
      AssertEx.Equal(inv[1, 0], -0.333333, TOLERANCE);
      AssertEx.Equal(inv[1, 1], 0.666667, TOLERANCE);
      Assert.Equal(0, inv[1, 2]);
      Assert.Equal(0, inv[2, 0]);
      Assert.Equal(0, inv[2, 1]);
      AssertEx.Equal(inv[2, 2], 0.333333, TOLERANCE);
    }

    [Fact]
    public void GetInverseNotPositiveDefiniteTest()
    {
      Assert.Throws<NotPositiveDefiniteException>(() =>
      {
        var a = new FloatMatrix(3, 3);
        var dcd = new FloatCholeskyDecomp(a);
        dcd.GetInverse();
      });
    }
  }
}
