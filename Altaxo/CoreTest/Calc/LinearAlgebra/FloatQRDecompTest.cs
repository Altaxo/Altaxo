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
  public class FloatQRDecompTest
  {
    private static FloatQRDecomp qr;
    private static FloatQRDecomp wqr;
    private static FloatQRDecomp lqr;
    private const float TOLERENCE = 0.001f;

    static FloatQRDecompTest()
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

    [Test]
    public void NullTest()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        var test = new FloatQRDecomp(null);
      });
    }

    [Test]
    public void QTest()
    {
      FloatMatrix Q = qr.Q;
      Assert.AreEqual(System.Math.Abs(Q[0, 0]), 0.147, TOLERENCE);
      Assert.AreEqual(System.Math.Abs(Q[0, 1]), 0.525, TOLERENCE);
      Assert.AreEqual(System.Math.Abs(Q[0, 2]), 0.838, TOLERENCE);
      Assert.AreEqual(System.Math.Abs(Q[1, 0]), 0.442, TOLERENCE);
      Assert.AreEqual(System.Math.Abs(Q[1, 1]), 0.723, TOLERENCE);
      Assert.AreEqual(System.Math.Abs(Q[1, 2]), 0.531, TOLERENCE);
      Assert.AreEqual(System.Math.Abs(Q[2, 0]), 0.885, TOLERENCE);
      Assert.AreEqual(System.Math.Abs(Q[2, 1]), 0.449, TOLERENCE);
      Assert.AreEqual(System.Math.Abs(Q[2, 2]), 0.126, TOLERENCE);
    }

    [Test]
    public void RTest()
    {
      FloatMatrix R = qr.R;
      Assert.AreEqual(System.Math.Abs(R[0, 0]), 6.782, TOLERENCE);
      Assert.AreEqual(System.Math.Abs(R[0, 1]), 3.686, TOLERENCE);
      Assert.AreEqual(System.Math.Abs(R[0, 2]), 7.520, TOLERENCE);
      Assert.AreEqual(System.Math.Abs(R[1, 0]), 0.000, TOLERENCE);
      Assert.AreEqual(System.Math.Abs(R[1, 1]), 10.555, TOLERENCE);
      Assert.AreEqual(System.Math.Abs(R[1, 2]), 6.469, TOLERENCE);
      Assert.AreEqual(System.Math.Abs(R[2, 0]), 0.000, TOLERENCE);
      Assert.AreEqual(System.Math.Abs(R[2, 1]), 0.000, TOLERENCE);
      Assert.AreEqual(System.Math.Abs(R[2, 2]), 4.428, TOLERENCE);
    }

    [Test]
    public void WideQTest()
    {
      FloatMatrix Q = wqr.Q;
      Assert.AreEqual(System.Math.Abs(Q[0, 0]), 0.316, TOLERENCE);
      Assert.AreEqual(System.Math.Abs(Q[0, 1]), 0.949, TOLERENCE);
      Assert.AreEqual(System.Math.Abs(Q[1, 0]), 0.949, TOLERENCE);
      Assert.AreEqual(System.Math.Abs(Q[1, 1]), 0.316, TOLERENCE);
    }

    [Test]
    public void WideRTest()
    {
      FloatMatrix R = wqr.R;
      Assert.AreEqual(System.Math.Abs(R[0, 0]), 3.162, TOLERENCE);
      Assert.AreEqual(System.Math.Abs(R[0, 1]), 7.273, TOLERENCE);
      Assert.AreEqual(System.Math.Abs(R[0, 2]), 0.949, TOLERENCE);
      Assert.AreEqual(System.Math.Abs(R[1, 0]), 0.000, TOLERENCE);
      Assert.AreEqual(System.Math.Abs(R[1, 1]), 2.846, TOLERENCE);
      Assert.AreEqual(System.Math.Abs(R[1, 2]), 6.008, TOLERENCE);
    }

    [Test]
    public void LongQTest()
    {
      FloatMatrix Q = lqr.Q;
      Assert.AreEqual(System.Math.Abs(Q[0, 0]), 0.147, TOLERENCE);
      Assert.AreEqual(System.Math.Abs(Q[0, 1]), 0.525, TOLERENCE);
      Assert.AreEqual(System.Math.Abs(Q[0, 2]), 0.838, TOLERENCE);
      Assert.AreEqual(System.Math.Abs(Q[1, 0]), 0.442, TOLERENCE);
      Assert.AreEqual(System.Math.Abs(Q[1, 1]), 0.723, TOLERENCE);
      Assert.AreEqual(System.Math.Abs(Q[1, 2]), 0.531, TOLERENCE);
      Assert.AreEqual(System.Math.Abs(Q[2, 0]), 0.885, TOLERENCE);
      Assert.AreEqual(System.Math.Abs(Q[2, 1]), 0.449, TOLERENCE);
      Assert.AreEqual(System.Math.Abs(Q[2, 2]), 0.126, TOLERENCE);
    }

    [Test]
    public void LongRTest()
    {
      FloatMatrix R = lqr.R;
      Assert.AreEqual(System.Math.Abs(R[0, 0]), 6.782, TOLERENCE);
      Assert.AreEqual(System.Math.Abs(R[0, 1]), 3.686, TOLERENCE);
      Assert.AreEqual(System.Math.Abs(R[1, 0]), 0.000, TOLERENCE);
      Assert.AreEqual(System.Math.Abs(R[1, 1]), 10.555, TOLERENCE);
      Assert.AreEqual(System.Math.Abs(R[2, 0]), 0.000, TOLERENCE);
      Assert.AreEqual(System.Math.Abs(R[2, 1]), 0.000, TOLERENCE);
    }

    [Test]
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
      Assert.AreEqual(x[0, 0], 2.965, TOLERENCE);
      Assert.AreEqual(x[0, 1], 2.965, TOLERENCE);
      Assert.AreEqual(x[0, 2], 2.965, TOLERENCE);
      Assert.AreEqual(x[1, 0], -0.479, TOLERENCE);
      Assert.AreEqual(x[1, 1], -0.479, TOLERENCE);
      Assert.AreEqual(x[1, 2], -0.479, TOLERENCE);
      Assert.AreEqual(x[2, 0], 1.227, TOLERENCE);
      Assert.AreEqual(x[2, 1], 1.227, TOLERENCE);
      Assert.AreEqual(x[2, 2], 1.227, TOLERENCE);

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
      Assert.AreEqual(x[0, 0], 2.965, TOLERENCE);
      Assert.AreEqual(x[0, 1], 2.965, TOLERENCE);
      Assert.AreEqual(x[1, 0], -0.479, TOLERENCE);
      Assert.AreEqual(x[1, 1], -0.479, TOLERENCE);
      Assert.AreEqual(x[2, 0], 1.227, TOLERENCE);
      Assert.AreEqual(x[2, 1], 1.227, TOLERENCE);

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
      Assert.AreEqual(x[0, 0], 2.965, TOLERENCE);
      Assert.AreEqual(x[0, 1], 2.965, TOLERENCE);
      Assert.AreEqual(x[0, 2], 2.965, TOLERENCE);
      Assert.AreEqual(x[0, 3], 2.965, TOLERENCE);
      Assert.AreEqual(x[1, 0], -0.479, TOLERENCE);
      Assert.AreEqual(x[1, 1], -0.479, TOLERENCE);
      Assert.AreEqual(x[1, 2], -0.479, TOLERENCE);
      Assert.AreEqual(x[1, 3], -0.479, TOLERENCE);
      Assert.AreEqual(x[2, 0], 1.227, TOLERENCE);
      Assert.AreEqual(x[2, 1], 1.227, TOLERENCE);
      Assert.AreEqual(x[2, 2], 1.227, TOLERENCE);
      Assert.AreEqual(x[2, 3], 1.227, TOLERENCE);

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
      Assert.AreEqual(x[0, 0], 2.73529, TOLERENCE);
      Assert.AreEqual(x[1, 0], -2.15822, TOLERENCE);
      Assert.AreEqual(x[2, 0], 0.0998564, TOLERENCE);

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
      Assert.AreEqual(x[0, 0], 2.73529, TOLERENCE);
      Assert.AreEqual(x[1, 0], -2.15822, TOLERENCE);
      Assert.AreEqual(x[2, 0], 0.0998564, TOLERENCE);
      Assert.AreEqual(x[0, 1], 2.73529, TOLERENCE);
      Assert.AreEqual(x[1, 1], -2.15822, TOLERENCE);
      Assert.AreEqual(x[2, 1], 0.0998564, TOLERENCE);
      Assert.AreEqual(x[0, 2], 2.73529, TOLERENCE);
      Assert.AreEqual(x[1, 2], -2.15822, TOLERENCE);
      Assert.AreEqual(x[2, 2], 0.0998564, TOLERENCE);
    }

    [Test]
    public void GetDeterminantTest()
    {
      float det = qr.GetDeterminant();
      Assert.AreEqual(det, 317.000, TOLERENCE);
    }

    [Test]
    public void GetWideDeterminantTest()
    {
      Assert.Throws(typeof(NotSquareMatrixException), () =>
      {
        float det = wqr.GetDeterminant();
      });
    }

    [Test]
    public void GetLongDeterminantTest()
    {
      Assert.Throws(typeof(NotSquareMatrixException), () =>
      {
        float det = lqr.GetDeterminant();
      });
    }
  }
}
