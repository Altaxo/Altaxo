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
using Xunit;

namespace AltaxoTest.Calc.LinearAlgebra
{

  public class ComplexDoubleLUDecompTest
  {
    private ComplexDoubleLUDecomp lu;
    private const double TOLERANCE = 0.001;

    public ComplexDoubleLUDecompTest()
    {
      var a = new ComplexDoubleMatrix(3)
      {
        [0, 0] = new Complex(-1, 1),
        [0, 1] = 5,
        [0, 2] = 6,
        [1, 0] = 3,
        [1, 1] = -6,
        [1, 2] = 1,
        [2, 0] = 6,
        [2, 1] = 8,
        [2, 2] = 9
      };
      lu = new ComplexDoubleLUDecomp(a);
    }

    [Fact]
    public void LUWide()
    {
      Assert.Throws<NotSquareMatrixException>(() =>
      {
        var wm = new ComplexDoubleMatrix(2, 3);
        var wlu = new ComplexDoubleLUDecomp(wm);
      });
    }

    [Fact]
    public void LULong()
    {
      Assert.Throws<NotSquareMatrixException>(() =>
      {
        var lm = new ComplexDoubleMatrix(3, 2);
        var llu = new ComplexDoubleLUDecomp(lm);
      });
    }

    [Fact]
    public void LTest()
    {
      Assert.Equal(lu.L[0, 0], Complex.One);
      Assert.Equal(lu.L[0, 1], Complex.Zero);
      Assert.Equal(lu.L[0, 2], Complex.Zero);
      AssertEx.Equal(lu.L[1, 0].Real, .500, TOLERANCE);
      Assert.Equal(lu.L[1, 1], Complex.One);
      Assert.Equal(lu.L[1, 2], Complex.Zero);
      AssertEx.Equal(lu.L[2, 0].Real, -.167, TOLERANCE);
      AssertEx.Equal(lu.L[2, 1].Real, -.633, TOLERANCE);
      AssertEx.Equal(lu.L[2, 0].Imag, .167, TOLERANCE);
      AssertEx.Equal(lu.L[2, 1].Imag, .133, TOLERANCE);
      Assert.Equal(lu.L[2, 2], Complex.One);
    }

    [Fact]
    public void UTest()
    {
      AssertEx.Equal(lu.U[0, 0].Real, 6.000, TOLERANCE);
      AssertEx.Equal(lu.U[0, 1].Real, 8.000, TOLERANCE);
      AssertEx.Equal(lu.U[0, 2].Real, 9.000, TOLERANCE);
      Assert.Equal(lu.U[1, 0], Complex.Zero);
      AssertEx.Equal(lu.U[1, 1].Real, -10.000, TOLERANCE);
      AssertEx.Equal(lu.U[1, 2].Real, -3.500, TOLERANCE);
      Assert.Equal(lu.U[2, 0], Complex.Zero);
      Assert.Equal(lu.U[2, 1], Complex.Zero);
      AssertEx.Equal(lu.U[2, 2].Real, 5.283, TOLERANCE);
      AssertEx.Equal(lu.U[2, 2].Imag, -1.033, TOLERANCE);
    }

    [Fact]
    public void GetDeterminantTest()
    {
      Complex det = lu.GetDeterminant();
      AssertEx.Equal(det.Real, 317.000, TOLERANCE);
      AssertEx.Equal(det.Imag, -62.000, TOLERANCE);
    }

    [Fact]
    public void IsSingularTest()
    {
      Assert.False(lu.IsSingular);
      var b = new ComplexDoubleMatrix(3);
      var dlu = new ComplexDoubleLUDecomp(b);
      Assert.True(dlu.IsSingular);
    }

    [Fact]
    public void GetInverseTest()
    {
      ComplexDoubleMatrix inv = lu.GetInverse();
      AssertEx.Equal(inv[0, 0].Real, -0.188378, TOLERANCE);
      AssertEx.Equal(inv[0, 1].Real, 0.009115, TOLERANCE);
      AssertEx.Equal(inv[0, 2].Real, 0.124572, TOLERANCE);
      AssertEx.Equal(inv[1, 0].Real, -0.063805, TOLERANCE);
      AssertEx.Equal(inv[1, 1].Real, -0.142074, TOLERANCE);
      AssertEx.Equal(inv[1, 2].Real, 0.058323, TOLERANCE);
      AssertEx.Equal(inv[2, 0].Real, 0.182301, TOLERANCE);
      AssertEx.Equal(inv[2, 1].Real, 0.120211, TOLERANCE);
      AssertEx.Equal(inv[2, 2].Real, -0.02378, TOLERANCE);
      AssertEx.Equal(inv[0, 0].Imag, -0.036844, TOLERANCE);
      AssertEx.Equal(inv[0, 1].Imag, 0.001783, TOLERANCE);
      AssertEx.Equal(inv[0, 2].Imag, 0.024364, TOLERANCE);
      AssertEx.Equal(inv[1, 0].Imag, -0.012479, TOLERANCE);
      AssertEx.Equal(inv[1, 1].Imag, 0.000604, TOLERANCE);
      AssertEx.Equal(inv[1, 2].Imag, 0.008252, TOLERANCE);
      AssertEx.Equal(inv[2, 0].Imag, 0.035655, TOLERANCE);
      AssertEx.Equal(inv[2, 1].Imag, -0.001725, TOLERANCE);
      AssertEx.Equal(inv[2, 2].Imag, -0.023578, TOLERANCE);
    }

    [Fact]
    public void GetInverseSingularTest()
    {
      Assert.Throws<SingularMatrixException>(() =>
      {
        var a = new ComplexDoubleMatrix(3, 3);
        var dlu = new ComplexDoubleLUDecomp(a);
        dlu.GetInverse();
      });
    }

    [Fact]
    public void SolveMatrix()
    {
      var b = new ComplexDoubleMatrix(3)
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
      ComplexDoubleMatrix x = lu.Solve(b);
      AssertEx.Equal(x[0, 0].Real, 2.856, TOLERANCE);
      AssertEx.Equal(x[0, 1].Real, 2.856, TOLERANCE);
      AssertEx.Equal(x[0, 2].Real, 2.856, TOLERANCE);
      AssertEx.Equal(x[1, 0].Real, -0.517, TOLERANCE);
      AssertEx.Equal(x[1, 1].Real, -0.517, TOLERANCE);
      AssertEx.Equal(x[1, 2].Real, -0.517, TOLERANCE);
      AssertEx.Equal(x[2, 0].Real, 1.333, TOLERANCE);
      AssertEx.Equal(x[2, 1].Real, 1.333, TOLERANCE);
      AssertEx.Equal(x[2, 2].Real, 1.333, TOLERANCE);
      AssertEx.Equal(x[0, 0].Imag, 0.559, TOLERANCE);
      AssertEx.Equal(x[0, 1].Imag, 0.559, TOLERANCE);
      AssertEx.Equal(x[0, 2].Imag, 0.559, TOLERANCE);
      AssertEx.Equal(x[1, 0].Imag, 0.189, TOLERANCE);
      AssertEx.Equal(x[1, 1].Imag, 0.189, TOLERANCE);
      AssertEx.Equal(x[1, 2].Imag, 0.189, TOLERANCE);
      AssertEx.Equal(x[2, 0].Imag, -0.541, TOLERANCE);
      AssertEx.Equal(x[2, 1].Imag, -0.541, TOLERANCE);
      AssertEx.Equal(x[2, 2].Imag, -0.541, TOLERANCE);

      b = new ComplexDoubleMatrix(3, 2)
      {
        [0, 0] = 2,
        [0, 1] = 2,
        [1, 0] = 13,
        [1, 1] = 13,
        [2, 0] = 25,
        [2, 1] = 25
      };
      x = lu.Solve(b);
      AssertEx.Equal(x[0, 0].Real, 2.856, TOLERANCE);
      AssertEx.Equal(x[0, 1].Real, 2.856, TOLERANCE);
      AssertEx.Equal(x[1, 0].Real, -0.517, TOLERANCE);
      AssertEx.Equal(x[1, 1].Real, -0.517, TOLERANCE);
      AssertEx.Equal(x[2, 0].Real, 1.333, TOLERANCE);
      AssertEx.Equal(x[2, 1].Real, 1.333, TOLERANCE);
      AssertEx.Equal(x[0, 0].Imag, 0.559, TOLERANCE);
      AssertEx.Equal(x[0, 1].Imag, 0.559, TOLERANCE);
      AssertEx.Equal(x[1, 0].Imag, 0.189, TOLERANCE);
      AssertEx.Equal(x[1, 1].Imag, 0.189, TOLERANCE);
      AssertEx.Equal(x[2, 0].Imag, -0.541, TOLERANCE);
      AssertEx.Equal(x[2, 1].Imag, -0.541, TOLERANCE);

      b = new ComplexDoubleMatrix(3, 4)
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
      AssertEx.Equal(x[0, 0].Real, 2.856, TOLERANCE);
      AssertEx.Equal(x[0, 1].Real, 2.856, TOLERANCE);
      AssertEx.Equal(x[0, 2].Real, 2.856, TOLERANCE);
      AssertEx.Equal(x[0, 3].Real, 2.856, TOLERANCE);
      AssertEx.Equal(x[1, 0].Real, -0.517, TOLERANCE);
      AssertEx.Equal(x[1, 1].Real, -0.517, TOLERANCE);
      AssertEx.Equal(x[1, 2].Real, -0.517, TOLERANCE);
      AssertEx.Equal(x[1, 3].Real, -0.517, TOLERANCE);
      AssertEx.Equal(x[2, 0].Real, 1.333, TOLERANCE);
      AssertEx.Equal(x[2, 1].Real, 1.333, TOLERANCE);
      AssertEx.Equal(x[2, 2].Real, 1.333, TOLERANCE);
      AssertEx.Equal(x[2, 3].Real, 1.333, TOLERANCE);
      AssertEx.Equal(x[0, 0].Imag, 0.559, TOLERANCE);
      AssertEx.Equal(x[0, 1].Imag, 0.559, TOLERANCE);
      AssertEx.Equal(x[0, 2].Imag, 0.559, TOLERANCE);
      AssertEx.Equal(x[0, 3].Imag, 0.559, TOLERANCE);
      AssertEx.Equal(x[1, 0].Imag, 0.189, TOLERANCE);
      AssertEx.Equal(x[1, 1].Imag, 0.189, TOLERANCE);
      AssertEx.Equal(x[1, 2].Imag, 0.189, TOLERANCE);
      AssertEx.Equal(x[1, 3].Imag, 0.189, TOLERANCE);
      AssertEx.Equal(x[2, 0].Imag, -0.541, TOLERANCE);
      AssertEx.Equal(x[2, 1].Imag, -0.541, TOLERANCE);
      AssertEx.Equal(x[2, 2].Imag, -0.541, TOLERANCE);
      AssertEx.Equal(x[2, 3].Imag, -0.541, TOLERANCE);
    }

    [Fact]
    public void SolveVector()
    {
      var b = new ComplexDoubleVector(3)
      {
        [0] = 2,
        [1] = 13,
        [2] = 25
      };
      ComplexDoubleVector x = lu.Solve(b);
      AssertEx.Equal(x[0].Real, 2.856, TOLERANCE);
      AssertEx.Equal(x[1].Real, -0.517, TOLERANCE);
      AssertEx.Equal(x[2].Real, 1.333, TOLERANCE);
      AssertEx.Equal(x[0].Imag, 0.559, TOLERANCE);
      AssertEx.Equal(x[1].Imag, 0.189, TOLERANCE);
      AssertEx.Equal(x[2].Imag, -0.541, TOLERANCE);
    }
  }
}
