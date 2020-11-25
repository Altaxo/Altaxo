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

  public class DoubleLUDecompTest
  {
    private DoubleLUDecomp lu;
    private const double TOLERANCE = 0.001;

    public DoubleLUDecompTest()
    {
      var a = new DoubleMatrix(3)
      {
        [0, 0] = -1,
        [0, 1] = 5,
        [0, 2] = 6,
        [1, 0] = 3,
        [1, 1] = -6,
        [1, 2] = 1,
        [2, 0] = 6,
        [2, 1] = 8,
        [2, 2] = 9
      };
      lu = new DoubleLUDecomp(a);
    }

    [Fact]
    public void LUWide()
    {
      Assert.Throws<NotSquareMatrixException>(() =>
      {
        var wm = new DoubleMatrix(2, 3);
        var wlu = new DoubleLUDecomp(wm);
      });
    }

    [Fact]
    public void LULong()
    {
      Assert.Throws<NotSquareMatrixException>(() =>
      {
        var lm = new DoubleMatrix(3, 2);
        var llu = new DoubleLUDecomp(lm);
      });
    }

    [Fact]
    public void LTest()
    {
      AssertEx.Equal(lu.L[0, 0], 1.000, TOLERANCE);
      AssertEx.Equal(lu.L[0, 1], 0.000, TOLERANCE);
      AssertEx.Equal(lu.L[0, 2], 0.000, TOLERANCE);
      AssertEx.Equal(lu.L[1, 0], .500, TOLERANCE);
      AssertEx.Equal(lu.L[1, 1], 1.000, TOLERANCE);
      AssertEx.Equal(lu.L[1, 2], 0.000, TOLERANCE);
      AssertEx.Equal(lu.L[2, 0], -.167, TOLERANCE);
      AssertEx.Equal(lu.L[2, 1], -.633, TOLERANCE);
      AssertEx.Equal(lu.L[2, 2], 1.000, TOLERANCE);
    }

    [Fact]
    public void UTest()
    {
      AssertEx.Equal(lu.U[0, 0], 6.000, TOLERANCE);
      AssertEx.Equal(lu.U[0, 1], 8.000, TOLERANCE);
      AssertEx.Equal(lu.U[0, 2], 9.000, TOLERANCE);
      AssertEx.Equal(lu.U[1, 0], 0.000, TOLERANCE);
      AssertEx.Equal(lu.U[1, 1], -10.000, TOLERANCE);
      AssertEx.Equal(lu.U[1, 2], -3.500, TOLERANCE);
      AssertEx.Equal(lu.U[2, 0], 0.000, TOLERANCE);
      AssertEx.Equal(lu.U[2, 1], 0.000, TOLERANCE);
      AssertEx.Equal(lu.U[2, 2], 5.283, TOLERANCE);
    }

    [Fact]
    public void GetDeterminantTest()
    {
      double det = lu.GetDeterminant();
      AssertEx.Equal(det, 317.000, TOLERANCE);
    }

    [Fact]
    public void IsSingularTest()
    {
      Assert.False(lu.IsSingular);
      var b = new DoubleMatrix(3);
      var dlu = new DoubleLUDecomp(b);
      Assert.True(dlu.IsSingular);
    }

    [Fact]
    public void GetInverseTest()
    {
      DoubleMatrix inv = lu.GetInverse();
      AssertEx.Equal(inv[0, 0], -0.195584, TOLERANCE);
      AssertEx.Equal(inv[0, 1], 0.009464, TOLERANCE);
      AssertEx.Equal(inv[0, 2], 0.129338, TOLERANCE);
      AssertEx.Equal(inv[1, 0], -0.066246, TOLERANCE);
      AssertEx.Equal(inv[1, 1], -0.141956, TOLERANCE);
      AssertEx.Equal(inv[1, 2], 0.059937, TOLERANCE);
      AssertEx.Equal(inv[2, 0], 0.189274, TOLERANCE);
      AssertEx.Equal(inv[2, 1], 0.119874, TOLERANCE);
      AssertEx.Equal(inv[2, 2], -0.028391, TOLERANCE);
    }

    [Fact]
    public void GetInverseSingularTest()
    {
      Assert.Throws<SingularMatrixException>(() =>
      {
        var a = new DoubleMatrix(3, 3);
        var dlu = new DoubleLUDecomp(a);
        dlu.GetInverse();
      });
    }

    [Fact]
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
      DoubleMatrix x = lu.Solve(b);
      AssertEx.Equal(x[0, 0], 2.965, TOLERANCE);
      AssertEx.Equal(x[0, 1], 2.965, TOLERANCE);
      AssertEx.Equal(x[0, 2], 2.965, TOLERANCE);
      AssertEx.Equal(x[1, 0], -0.479, TOLERANCE);
      AssertEx.Equal(x[1, 1], -0.479, TOLERANCE);
      AssertEx.Equal(x[1, 2], -0.479, TOLERANCE);
      AssertEx.Equal(x[2, 0], 1.227, TOLERANCE);
      AssertEx.Equal(x[2, 1], 1.227, TOLERANCE);
      AssertEx.Equal(x[2, 2], 1.227, TOLERANCE);

      b = new DoubleMatrix(3, 2)
      {
        [0, 0] = 2,
        [0, 1] = 2,
        [1, 0] = 13,
        [1, 1] = 13,
        [2, 0] = 25,
        [2, 1] = 25
      };
      x = lu.Solve(b);
      AssertEx.Equal(x[0, 0], 2.965, TOLERANCE);
      AssertEx.Equal(x[0, 1], 2.965, TOLERANCE);
      AssertEx.Equal(x[1, 0], -0.479, TOLERANCE);
      AssertEx.Equal(x[1, 1], -0.479, TOLERANCE);
      AssertEx.Equal(x[2, 0], 1.227, TOLERANCE);
      AssertEx.Equal(x[2, 1], 1.227, TOLERANCE);

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
      x = lu.Solve(b);
      AssertEx.Equal(x[0, 0], 2.965, TOLERANCE);
      AssertEx.Equal(x[0, 1], 2.965, TOLERANCE);
      AssertEx.Equal(x[0, 2], 2.965, TOLERANCE);
      AssertEx.Equal(x[0, 3], 2.965, TOLERANCE);
      AssertEx.Equal(x[1, 0], -0.479, TOLERANCE);
      AssertEx.Equal(x[1, 1], -0.479, TOLERANCE);
      AssertEx.Equal(x[1, 2], -0.479, TOLERANCE);
      AssertEx.Equal(x[1, 3], -0.479, TOLERANCE);
      AssertEx.Equal(x[2, 0], 1.227, TOLERANCE);
      AssertEx.Equal(x[2, 1], 1.227, TOLERANCE);
      AssertEx.Equal(x[2, 2], 1.227, TOLERANCE);
      AssertEx.Equal(x[2, 3], 1.227, TOLERANCE);
    }

    [Fact]
    public void SolveVector()
    {
      var b = new DoubleVector(3)
      {
        [0] = 2,
        [1] = 13,
        [2] = 25
      };
      DoubleVector x = lu.Solve(b);
      AssertEx.Equal(x[0], 2.965, TOLERANCE);
      AssertEx.Equal(x[1], -0.479, TOLERANCE);
      AssertEx.Equal(x[2], 1.227, TOLERANCE);
    }
  }
}
