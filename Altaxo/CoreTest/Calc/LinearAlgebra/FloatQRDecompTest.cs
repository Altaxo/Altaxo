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

  public class FloatQRDecompTest
  {
    private FloatQRDecomp qr;
    private FloatQRDecomp wqr;
    private FloatQRDecomp lqr;
    private const float TOLERANCE = 0.001f;

    public FloatQRDecompTest()
    {
      var a = new FloatMatrix(3)
      {
        [0, 0] = -1.0f,
        [0, 1] = 5.0f,
        [0, 2] = 6.0f,
        [1, 0] = 3.0f,
        [1, 1] = -6.0f,
        [1, 2] = 1.0f,
        [2, 0] = 6.0f,
        [2, 1] = 8.0f,
        [2, 2] = 9.0f
      };
      qr = new FloatQRDecomp(a);

      a = new FloatMatrix(2, 3)
      {
        [0, 0] = -1.0f,
        [0, 1] = 5.0f,
        [0, 2] = 6.0f,
        [1, 0] = 3.0f,
        [1, 1] = -6.0f,
        [1, 2] = 1.0f
      };
      wqr = new FloatQRDecomp(a);

      a = new FloatMatrix(3, 2)
      {
        [0, 0] = -1.0f,
        [0, 1] = 5.0f,
        [1, 0] = 3.0f,
        [1, 1] = -6.0f,
        [2, 0] = 6.0f,
        [2, 1] = 8.0f
      };
      lqr = new FloatQRDecomp(a);
    }

    [Fact]
    public void NullTest()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        var test = new FloatQRDecomp(null);
      });
    }

    [Fact]
    public void QTest()
    {
      FloatMatrix Q = qr.Q;
      AssertEx.Equal(System.Math.Abs(Q[0, 0]), 0.147, TOLERANCE);
      AssertEx.Equal(System.Math.Abs(Q[0, 1]), 0.525, TOLERANCE);
      AssertEx.Equal(System.Math.Abs(Q[0, 2]), 0.838, TOLERANCE);
      AssertEx.Equal(System.Math.Abs(Q[1, 0]), 0.442, TOLERANCE);
      AssertEx.Equal(System.Math.Abs(Q[1, 1]), 0.723, TOLERANCE);
      AssertEx.Equal(System.Math.Abs(Q[1, 2]), 0.531, TOLERANCE);
      AssertEx.Equal(System.Math.Abs(Q[2, 0]), 0.885, TOLERANCE);
      AssertEx.Equal(System.Math.Abs(Q[2, 1]), 0.449, TOLERANCE);
      AssertEx.Equal(System.Math.Abs(Q[2, 2]), 0.126, TOLERANCE);
    }

    [Fact]
    public void RTest()
    {
      FloatMatrix R = qr.R;
      AssertEx.Equal(System.Math.Abs(R[0, 0]), 6.782, TOLERANCE);
      AssertEx.Equal(System.Math.Abs(R[0, 1]), 3.686, TOLERANCE);
      AssertEx.Equal(System.Math.Abs(R[0, 2]), 7.520, TOLERANCE);
      AssertEx.Equal(System.Math.Abs(R[1, 0]), 0.000, TOLERANCE);
      AssertEx.Equal(System.Math.Abs(R[1, 1]), 10.555, TOLERANCE);
      AssertEx.Equal(System.Math.Abs(R[1, 2]), 6.469, TOLERANCE);
      AssertEx.Equal(System.Math.Abs(R[2, 0]), 0.000, TOLERANCE);
      AssertEx.Equal(System.Math.Abs(R[2, 1]), 0.000, TOLERANCE);
      AssertEx.Equal(System.Math.Abs(R[2, 2]), 4.428, TOLERANCE);
    }

    [Fact]
    public void WideQTest()
    {
      FloatMatrix Q = wqr.Q;
      AssertEx.Equal(System.Math.Abs(Q[0, 0]), 0.316, TOLERANCE);
      AssertEx.Equal(System.Math.Abs(Q[0, 1]), 0.949, TOLERANCE);
      AssertEx.Equal(System.Math.Abs(Q[1, 0]), 0.949, TOLERANCE);
      AssertEx.Equal(System.Math.Abs(Q[1, 1]), 0.316, TOLERANCE);
    }

    [Fact]
    public void WideRTest()
    {
      FloatMatrix R = wqr.R;
      AssertEx.Equal(System.Math.Abs(R[0, 0]), 3.162, TOLERANCE);
      AssertEx.Equal(System.Math.Abs(R[0, 1]), 7.273, TOLERANCE);
      AssertEx.Equal(System.Math.Abs(R[0, 2]), 0.949, TOLERANCE);
      AssertEx.Equal(System.Math.Abs(R[1, 0]), 0.000, TOLERANCE);
      AssertEx.Equal(System.Math.Abs(R[1, 1]), 2.846, TOLERANCE);
      AssertEx.Equal(System.Math.Abs(R[1, 2]), 6.008, TOLERANCE);
    }

    [Fact]
    public void LongQTest()
    {
      FloatMatrix Q = lqr.Q;
      AssertEx.Equal(System.Math.Abs(Q[0, 0]), 0.147, TOLERANCE);
      AssertEx.Equal(System.Math.Abs(Q[0, 1]), 0.525, TOLERANCE);
      AssertEx.Equal(System.Math.Abs(Q[0, 2]), 0.838, TOLERANCE);
      AssertEx.Equal(System.Math.Abs(Q[1, 0]), 0.442, TOLERANCE);
      AssertEx.Equal(System.Math.Abs(Q[1, 1]), 0.723, TOLERANCE);
      AssertEx.Equal(System.Math.Abs(Q[1, 2]), 0.531, TOLERANCE);
      AssertEx.Equal(System.Math.Abs(Q[2, 0]), 0.885, TOLERANCE);
      AssertEx.Equal(System.Math.Abs(Q[2, 1]), 0.449, TOLERANCE);
      AssertEx.Equal(System.Math.Abs(Q[2, 2]), 0.126, TOLERANCE);
    }

    [Fact]
    public void LongRTest()
    {
      FloatMatrix R = lqr.R;
      AssertEx.Equal(System.Math.Abs(R[0, 0]), 6.782, TOLERANCE);
      AssertEx.Equal(System.Math.Abs(R[0, 1]), 3.686, TOLERANCE);
      AssertEx.Equal(System.Math.Abs(R[1, 0]), 0.000, TOLERANCE);
      AssertEx.Equal(System.Math.Abs(R[1, 1]), 10.555, TOLERANCE);
      AssertEx.Equal(System.Math.Abs(R[2, 0]), 0.000, TOLERANCE);
      AssertEx.Equal(System.Math.Abs(R[2, 1]), 0.000, TOLERANCE);
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
      FloatMatrix x = qr.Solve(b);
      AssertEx.Equal(x[0, 0], 2.965, TOLERANCE);
      AssertEx.Equal(x[0, 1], 2.965, TOLERANCE);
      AssertEx.Equal(x[0, 2], 2.965, TOLERANCE);
      AssertEx.Equal(x[1, 0], -0.479, TOLERANCE);
      AssertEx.Equal(x[1, 1], -0.479, TOLERANCE);
      AssertEx.Equal(x[1, 2], -0.479, TOLERANCE);
      AssertEx.Equal(x[2, 0], 1.227, TOLERANCE);
      AssertEx.Equal(x[2, 1], 1.227, TOLERANCE);
      AssertEx.Equal(x[2, 2], 1.227, TOLERANCE);

      b = new FloatMatrix(3, 2)
      {
        [0, 0] = 2,
        [0, 1] = 2,
        [1, 0] = 13,
        [1, 1] = 13,
        [2, 0] = 25,
        [2, 1] = 25
      };
      x = qr.Solve(b);
      AssertEx.Equal(x[0, 0], 2.965, TOLERANCE);
      AssertEx.Equal(x[0, 1], 2.965, TOLERANCE);
      AssertEx.Equal(x[1, 0], -0.479, TOLERANCE);
      AssertEx.Equal(x[1, 1], -0.479, TOLERANCE);
      AssertEx.Equal(x[2, 0], 1.227, TOLERANCE);
      AssertEx.Equal(x[2, 1], 1.227, TOLERANCE);

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
      x = qr.Solve(b);
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

      var A = new FloatMatrix(4, 3)
      {
        [0, 0] = -4.18f,
        [0, 1] = -5.011f,
        [0, 2] = -5.841f,
        [1, 0] = 4.986f,
        [1, 1] = 5.805f,
        [1, 2] = 6.624f,
        [2, 0] = 3.695f,
        [2, 1] = 3.687f,
        [2, 2] = 3.679f,
        [3, 0] = -5.489f,
        [3, 1] = -7.024f,
        [3, 2] = 8.56f
      };

      var qrd = new FloatQRDecomp(A);
      var B = new FloatMatrix(4, 1)
      {
        [0, 0] = 1,
        [1, 0] = 4,
        [2, 0] = 2,
        [3, 0] = 1
      };

      x = qrd.Solve(B);
      AssertEx.Equal(x[0, 0], 2.73529, TOLERANCE);
      AssertEx.Equal(x[1, 0], -2.15822, TOLERANCE);
      AssertEx.Equal(x[2, 0], 0.0998564, TOLERANCE);

      B = new FloatMatrix(4, 3)
      {
        [0, 0] = 1,
        [1, 0] = 4,
        [2, 0] = 2,
        [3, 0] = 1,
        [0, 1] = 1,
        [1, 1] = 4,
        [2, 1] = 2,
        [3, 1] = 1,
        [0, 2] = 1,
        [1, 2] = 4,
        [2, 2] = 2,
        [3, 2] = 1
      };

      x = qrd.Solve(B);
      AssertEx.Equal(x[0, 0], 2.73529, TOLERANCE);
      AssertEx.Equal(x[1, 0], -2.15822, TOLERANCE);
      AssertEx.Equal(x[2, 0], 0.0998564, TOLERANCE);
      AssertEx.Equal(x[0, 1], 2.73529, TOLERANCE);
      AssertEx.Equal(x[1, 1], -2.15822, TOLERANCE);
      AssertEx.Equal(x[2, 1], 0.0998564, TOLERANCE);
      AssertEx.Equal(x[0, 2], 2.73529, TOLERANCE);
      AssertEx.Equal(x[1, 2], -2.15822, TOLERANCE);
      AssertEx.Equal(x[2, 2], 0.0998564, TOLERANCE);
    }

    [Fact]
    public void GetDeterminantTest()
    {
      float det = qr.GetDeterminant();
      AssertEx.Equal(det, 317.000, TOLERANCE);
    }

    [Fact]
    public void GetWideDeterminantTest()
    {
      Assert.Throws<NotSquareMatrixException>(() =>
      {
        float det = wqr.GetDeterminant();
      });
    }

    [Fact]
    public void GetLongDeterminantTest()
    {
      Assert.Throws<NotSquareMatrixException>(() =>
      {
        float det = lqr.GetDeterminant();
      });
    }
  }
}
