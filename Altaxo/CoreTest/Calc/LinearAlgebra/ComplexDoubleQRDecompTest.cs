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
  
  public class ComplexDoubleQRDecompTest
  {
    [Fact]
    public void NullTest()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        var test = new ComplexDoubleQRDecomp(null);
      });
    }

    [Fact]
    public void SquareDecomp()
    {
      var a = new ComplexDoubleMatrix(3)
      {
        [0, 0] = new Complex(1.1, 1.1),
        [0, 1] = new Complex(2.2, -2.2),
        [0, 2] = new Complex(3.3, 3.3),
        [1, 0] = new Complex(4.4, -4.4),
        [1, 1] = new Complex(5.5, 5.5),
        [1, 2] = new Complex(6.6, -6.6),
        [2, 0] = new Complex(7.7, 7.7),
        [2, 1] = new Complex(8.8, -8.8),
        [2, 2] = new Complex(9.9, 9.9)
      };

      var qrd = new ComplexDoubleQRDecomp(a);
      ComplexDoubleMatrix qq = qrd.Q.GetConjugateTranspose() * qrd.Q;
      ComplexDoubleMatrix qr = qrd.Q * qrd.R;
      var I = ComplexDoubleMatrix.CreateIdentity(3);

      // determine the maximum relative error
      double MaxError = 0.0;
      for (int i = 0; i < 3; i++)
      {
        for (int j = 0; i < 3; i++)
        {
          double E = ComplexMath.Absolute((qq[i, j] - I[i, j]));
          if (E > MaxError)
          {
            MaxError = E;
          }
        }
      }
      Assert.True(MaxError < 1.0E-14);

      MaxError = 0.0;
      for (int i = 0; i < 3; i++)
      {
        for (int j = 0; i < 3; i++)
        {
          double E = ComplexMath.Absolute((qr[i, j] - a[i, j]) / a[i, j]);
          if (E > MaxError)
          {
            MaxError = E;
          }
        }
      }
      Assert.True(MaxError < 1.0E-14);
    }

    [Fact]
    public void WideDecomp()
    {
      var a = new ComplexDoubleMatrix(2, 4)
      {
        [0, 0] = new Complex(1.1, 1.1),
        [0, 1] = new Complex(2.2, -2.2),
        [0, 2] = new Complex(3.3, 3.3),
        [0, 3] = new Complex(4.4, -4.4),
        [1, 0] = new Complex(5.5, 5.5),
        [1, 1] = new Complex(6.6, -6.6),
        [1, 2] = new Complex(7.7, 7.7),
        [1, 3] = new Complex(8.8, -8.8)
      };

      var qrd = new ComplexDoubleQRDecomp(a);
      ComplexDoubleMatrix qq = qrd.Q.GetConjugateTranspose() * qrd.Q;
      ComplexDoubleMatrix qr = qrd.Q * qrd.R;
      var I = ComplexDoubleMatrix.CreateIdentity(2);

      // determine the maximum relative error
      double MaxError = 0.0;
      for (int i = 0; i < 2; i++)
      {
        for (int j = 0; j < 2; j++)
        {
          double E = ComplexMath.Absolute((qq[i, j] - I[i, j]));
          if (E > MaxError)
          {
            MaxError = E;
          }
        }
      }
      Assert.True(MaxError < 1.0E-14);

      MaxError = 0.0;
      for (int i = 0; i < 2; i++)
      {
        for (int j = 0; j < 4; j++)
        {
          double E = ComplexMath.Absolute((qr[i, j] - a[i, j]) / a[i, j]);
          if (E > MaxError)
          {
            MaxError = E;
          }
        }
      }
      Assert.True(MaxError < 1.0E-14);
    }

    [Fact]
    public void LongDecomp()
    {
      var a = new ComplexDoubleMatrix(4, 2)
      {
        [0, 0] = new Complex(1.1, 1.1),
        [0, 1] = new Complex(2.2, -2.2),
        [1, 0] = new Complex(3.3, 3.3),
        [1, 1] = new Complex(4.4, -4.4),
        [2, 0] = new Complex(5.5, 5.5),
        [2, 1] = new Complex(6.6, -6.6),
        [3, 0] = new Complex(7.7, 7.7),
        [3, 1] = new Complex(8.8, -8.8)
      };

      var qrd = new ComplexDoubleQRDecomp(a);
      ComplexDoubleMatrix qq = qrd.Q.GetConjugateTranspose() * qrd.Q;
      ComplexDoubleMatrix qr = qrd.Q * qrd.R;
      var I = ComplexDoubleMatrix.CreateIdentity(4);

      // determine the maximum relative error
      double MaxError = 0.0;
      for (int i = 0; i < 4; i++)
      {
        for (int j = 0; i < 4; i++)
        {
          double E = ComplexMath.Absolute((qq[i, j] - I[i, j]));
          if (E > MaxError)
          {
            MaxError = E;
          }
        }
      }
      Assert.True(MaxError < 1.0E-14);
      MaxError = 0.0;
      for (int i = 0; i < 4; i++)
      {
        for (int j = 0; j < 2; j++)
        {
          double E = ComplexMath.Absolute((qr[i, j] - a[i, j]) / a[i, j]);
          if (E > MaxError)
          {
            MaxError = E;
          }
        }
      }
      Assert.True(MaxError < 1.0E-14);
    }

    [Fact]
    public void SolveMatrix()
    {
      var a = new ComplexDoubleMatrix(3)
      {
        [0, 0] = new Complex(1.1, 1.1),
        [0, 1] = new Complex(2.2, -2.2),
        [0, 2] = new Complex(3.3, 3.3),
        [1, 0] = new Complex(4.4, -4.4),
        [1, 1] = new Complex(5.5, 5.5),
        [1, 2] = new Complex(6.6, -6.6),
        [2, 0] = new Complex(7.7, 7.7),
        [2, 1] = new Complex(8.8, -8.8),
        [2, 2] = new Complex(9.9, 9.9)
      };
      var qr = new ComplexDoubleQRDecomp(a);

      var b = new ComplexDoubleMatrix(3)
      {
        [0, 0] = new Complex(2.3, -3.2),
        [0, 1] = new Complex(2.3, -3.2),
        [0, 2] = new Complex(2.3, -3.2),
        [1, 0] = new Complex(6.7, 7.8),
        [1, 1] = new Complex(6.7, 7.8),
        [1, 2] = new Complex(6.7, 7.8),
        [2, 0] = new Complex(1.3, -9.7),
        [2, 1] = new Complex(1.3, -9.7),
        [2, 2] = new Complex(1.3, -9.7)
      };

      ComplexDoubleMatrix X = qr.Solve(b);

      Assert.True(Comparer.AreEqual(X[0, 0], new Complex(-0.57, 1.14), .01));
      Assert.True(Comparer.AreEqual(X[0, 1], new Complex(-0.57, 1.14), .01));
      Assert.True(Comparer.AreEqual(X[0, 2], new Complex(-0.57, 1.14), .01));
      Assert.True(Comparer.AreEqual(X[1, 0], new Complex(1.03, -0.16), .01));
      Assert.True(Comparer.AreEqual(X[1, 1], new Complex(1.03, -0.16), .01));
      Assert.True(Comparer.AreEqual(X[1, 2], new Complex(1.03, -0.16), .01));
      Assert.True(Comparer.AreEqual(X[2, 0], new Complex(0.16, -0.52), .01));
      Assert.True(Comparer.AreEqual(X[2, 1], new Complex(0.16, -0.52), .01));
      Assert.True(Comparer.AreEqual(X[2, 2], new Complex(0.16, -0.52), .01));

      a = new ComplexDoubleMatrix(3, 2)
      {
        [0, 0] = new Complex(1.1, 1.1),
        [0, 1] = new Complex(2.2, -2.2),
        [1, 0] = new Complex(4.4, -4.4),
        [1, 1] = new Complex(5.5, 5.5),
        [2, 0] = new Complex(7.7, 7.7),
        [2, 1] = new Complex(8.8, -8.8)
      };
      qr = new ComplexDoubleQRDecomp(a);

      b = new ComplexDoubleMatrix(3)
      {
        [0, 0] = new Complex(2.3, -3.2),
        [0, 1] = new Complex(2.3, -3.2),
        [0, 2] = new Complex(2.3, -3.2),
        [1, 0] = new Complex(6.7, 7.8),
        [1, 1] = new Complex(6.7, 7.8),
        [1, 2] = new Complex(6.7, 7.8),
        [2, 0] = new Complex(1.3, -9.7),
        [2, 1] = new Complex(1.3, -9.7),
        [2, 2] = new Complex(1.3, -9.7)
      };

      X = qr.Solve(b);

      Assert.True(Comparer.AreEqual(X[0, 0], new Complex(-0.344, 0.410), .01));
      Assert.True(Comparer.AreEqual(X[0, 1], new Complex(-0.344, 0.410), .01));
      Assert.True(Comparer.AreEqual(X[0, 2], new Complex(-0.344, 0.410), .01));
      Assert.True(Comparer.AreEqual(X[1, 0], new Complex(1.01, -0.170), .01));
      Assert.True(Comparer.AreEqual(X[1, 1], new Complex(1.01, -0.170), .01));
      Assert.True(Comparer.AreEqual(X[1, 2], new Complex(1.01, -0.170), .01));
    }
  }
}
