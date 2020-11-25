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
using System.Collections;
using Altaxo.Calc;
using Altaxo.Calc.LinearAlgebra;
using Xunit;

namespace AltaxoTest.Calc.LinearAlgebra
{

  public class ComplexDoubleVectorTest
  {
    private const double TOLERANCE = 0.001;

    //Test dimensions Constructor.
    [Fact]
    public void CtorDimensions()
    {
      var test = new ComplexDoubleVector(2);

      Assert.Equal(2, test.Length);
      Assert.Equal(test[0], new Complex(0));
      Assert.Equal(test[1], new Complex(0));
    }

    //Test Copy Constructor.
    [Fact]
    public void CtorDimensionsZero()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var test = new ComplexDoubleVector(0);
      });
    }

    //Test Copy Constructor.
    [Fact]
    public void CtorDimensionsNegative()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var test = new ComplexDoubleVector(-1);
      });
    }

    //Test Intital Values Constructor.
    [Fact]
    public void CtorInitialValues()
    {
      var test = new ComplexDoubleVector(2, new Complex(1, -1));

      Assert.Equal(2, test.Length);
      Assert.Equal(test[0], new Complex(1, -1));
      Assert.Equal(test[1], new Complex(1, -1));
    }

    //Test Array Constructor
    [Fact]
    public void CtorArray()
    {
      double[] testvector = new double[2] { 0, 1 };

      var test = new ComplexDoubleVector(testvector);
      Assert.Equal(test.Length, testvector.Length);
      Assert.Equal(test[0], new Complex(testvector[0]));
      Assert.Equal(test[1], new Complex(testvector[1]));
    }

    //*TODO IList Constructor

    //Test Copy Constructor.
    [Fact]
    public void CtorCopy()
    {
      var a = new ComplexDoubleVector(new double[2] { 0, 1 });
      var b = new ComplexDoubleVector(a);

      Assert.Equal(b.Length, a.Length);
      Assert.Equal(b[0], a[0]);
      Assert.Equal(b[1], a[1]);
    }

    //Test Copy Constructor.
    [Fact]
    public void CtorCopyNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        ComplexDoubleVector a = null;
        var b = new ComplexDoubleVector(a);
      });
    }

    //Test Index Access
    [Fact]
    public void IndexAccessGetNegative()
    {
      Assert.Throws<IndexOutOfRangeException>(() =>
      {
        var a = new ComplexDoubleVector(new double[2] { 0, 1 });
        Complex b = a[-1];
      });
    }

    //Test Index Access
    [Fact]
    public void IndexAccessSetNegative()
    {
      Assert.Throws<IndexOutOfRangeException>(() =>
      {
        var a = new ComplexDoubleVector(2)
        {
          [-1] = 1
        };
      });
    }

    //Test Index Access
    [Fact]
    public void IndexAccessGetOutOfRange()
    {
      Assert.Throws<IndexOutOfRangeException>(() =>
      {
        var a = new ComplexDoubleVector(new double[2] { 0, 1 });
        Complex b = a[2];
      });
    }

    //Test Index Access
    [Fact]
    public void IndexAccessSetOutOfRange()
    {
      Assert.Throws<IndexOutOfRangeException>(() =>
      {
        var a = new ComplexDoubleVector(2)
        {
          [2] = 1
        };
      });
    }

    //Test Equals
    [Fact]
    public void TestEquals()
    {
      var a = new ComplexDoubleVector(2, 4);
      var b = new ComplexDoubleVector(2, 4);
      var c = new ComplexDoubleVector(2)
      {
        [0] = 4,
        [1] = 4
      };

      var d = new ComplexDoubleVector(2, 5);
      ComplexDoubleVector e = null;
      var f = new FloatVector(2, 4);
      Assert.True(a.Equals(b));
      Assert.True(b.Equals(a));
      Assert.True(a.Equals(c));
      Assert.True(b.Equals(c));
      Assert.True(c.Equals(b));
      Assert.True(c.Equals(a));
      Assert.False(a.Equals(d));
      Assert.False(d.Equals(b));
      Assert.False(a.Equals(e));
      Assert.False(a.Equals(f));
    }

    //Test get real and imaginary components
    [Fact]
    public void RealImag()
    {
      var a = new ComplexDoubleVector(2)
      {
        [0] = new Complex(1, 2),
        [1] = new Complex(3, 4)
      };

      DoubleVector a_real = a.Real;
      DoubleVector a_imag = a.Imag;

      Assert.Equal(a_real[0], a[0].Real);
      Assert.Equal(a_imag[0], a[0].Imag);
      Assert.Equal(a_real[1], a[1].Real);
      Assert.Equal(a_imag[1], a[1].Imag);
      Assert.Equal(a_real.Length, a.Length);
      Assert.Equal(a_imag.Length, a.Length);
    }

    //test GetHashCode
    [Fact]
    public void TestHashCode()
    {
      var a = new ComplexDoubleVector(2)
      {
        [0] = 0,
        [1] = 1
      };

      int hash = a.GetHashCode();
      Assert.Equal(1, hash);
    }

    //Test GetInternalData
    [Fact]
    public void GetInternalData()
    {
      double[] testvector = new double[2] { 0, 1 };
      var test = new ComplexDoubleVector(testvector);
      Complex[] internaldata = test.GetInternalData();

      Assert.Equal(internaldata.Length, testvector.Length);
      Assert.Equal(internaldata[0], new Complex(testvector[0]));
      Assert.Equal(internaldata[1], new Complex(testvector[1]));
    }

    //Test ToArray
    [Fact]
    public void ToArray()
    {
      double[] testvector = new double[2] { 0, 1 };
      var test = new ComplexDoubleVector(testvector);
      Complex[] internaldata = test.ToArray();

      Assert.Equal(internaldata.Length, testvector.Length);
      Assert.Equal(internaldata[0], new Complex(testvector[0]));
      Assert.Equal(internaldata[1], new Complex(testvector[1]));
    }

    //Test GetSubVector
    [Fact]
    public void GetSubVector()
    {
      var test = new ComplexDoubleVector(new double[4] { 0, 1, 2, 3 });
      ComplexDoubleVector subvector = test.GetSubVector(1, 2);

      Assert.Equal(2, subvector.Length);
      Assert.Equal(subvector[0], test[1]);
      Assert.Equal(subvector[1], test[2]);
    }

    //Test Implicit cast conversion to ComplexDoubleVector
    [Fact]
    public void ImplicitConversion()
    {
      float[] a = new float[4] { 0, 1, 2, 3 };
      double[] b = new double[4] { 0, 1, 2, 3 };
      var c = new ComplexFloatVector(a);
      ComplexDoubleVector d, e, f;

      d = a;
      e = b;
      f = c;

      Assert.Equal(a.Length, d.Length);
      Assert.Equal((Complex)a[0], d[0]);
      Assert.Equal((Complex)a[1], d[1]);
      Assert.Equal((Complex)a[2], d[2]);
      Assert.Equal((Complex)a[3], d[3]);

      Assert.Equal(b.Length, e.Length);
      Assert.Equal((Complex)b[0], e[0]);
      Assert.Equal((Complex)b[1], e[1]);
      Assert.Equal((Complex)b[2], e[2]);
      Assert.Equal((Complex)b[3], e[3]);

      Assert.Equal(c.Length, f.Length);
      Assert.Equal((Complex)c[0], f[0]);
      Assert.Equal((Complex)c[1], f[1]);
      Assert.Equal((Complex)c[2], f[2]);
      Assert.Equal((Complex)c[3], f[3]);
    }

    //Test GetIndex functions
    [Fact]
    public void GetIndex()
    {
      var a = new ComplexDoubleVector(new double[4] { 1, 2, 3, 4 });
      var b = new ComplexDoubleVector(new double[4] { 3, 2, 1, 0 });
      var c = new ComplexDoubleVector(new double[4] { 0, -1, -2, -3 });
      var d = new ComplexDoubleVector(new double[4] { -3, -2, -1, 0 });

      Assert.Equal(3, a.GetAbsMaximumIndex());
      Assert.Equal(0, b.GetAbsMaximumIndex());
      Assert.Equal(3, c.GetAbsMaximumIndex());
      Assert.Equal(0, d.GetAbsMaximumIndex());

      Assert.Equal(a.GetAbsMaximum(), (Complex)4);
      Assert.Equal(b.GetAbsMaximum(), (Complex)3);
      Assert.Equal(c.GetAbsMaximum(), (Complex)(-3));
      Assert.Equal(d.GetAbsMaximum(), (Complex)(-3));

      Assert.Equal(0, a.GetAbsMinimumIndex());
      Assert.Equal(3, b.GetAbsMinimumIndex());
      Assert.Equal(0, c.GetAbsMinimumIndex());
      Assert.Equal(3, d.GetAbsMinimumIndex());

      Assert.Equal(a.GetAbsMinimum(), (Complex)1);
      Assert.Equal(b.GetAbsMinimum(), (Complex)0);
      Assert.Equal(c.GetAbsMinimum(), (Complex)0);
      Assert.Equal(d.GetAbsMinimum(), (Complex)0);
    }

    //Test invalid dimensions with copy
    [Fact]
    public void CopyException()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new ComplexDoubleVector(new double[4] { 0, 1, 2, 3 });
        var b = new ComplexDoubleVector(5);

        a.Copy(b);
      });
    }

    //Test invalid dimensions with swap
    [Fact]
    public void SwapException()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new ComplexDoubleVector(new double[4] { 0, 1, 2, 3 });
        var b = new ComplexDoubleVector(new double[5] { 4, 5, 6, 7, 8 });

        a.Swap(b);
      });
    }

    //Test Copy and Swap
    [Fact]
    public void CopySwap()
    {
      var a = new ComplexDoubleVector(new double[4] { 0, 1, 2, 3 });
      var b = new ComplexDoubleVector(new double[4] { 4, 5, 6, 7 });
      var c = new ComplexDoubleVector(4);
      var d = new ComplexDoubleVector(4);

      a.Copy(c);
      b.Copy(d);

      Assert.Equal(a.Length, c.Length);
      Assert.Equal(a[0], c[0]);
      Assert.Equal(a[1], c[1]);
      Assert.Equal(a[2], c[2]);
      Assert.Equal(a[3], c[3]);

      Assert.Equal(b.Length, d.Length);
      Assert.Equal(b[0], d[0]);
      Assert.Equal(b[1], d[1]);
      Assert.Equal(b[2], d[2]);
      Assert.Equal(b[3], d[3]);

      a.Swap(b);

      Assert.Equal(b.Length, c.Length);
      Assert.Equal(b[0], c[0]);
      Assert.Equal(b[1], c[1]);
      Assert.Equal(b[2], c[2]);
      Assert.Equal(b[3], c[3]);

      Assert.Equal(a.Length, d.Length);
      Assert.Equal(a[0], d[0]);
      Assert.Equal(a[1], d[1]);
      Assert.Equal(a[2], d[2]);
      Assert.Equal(a[3], d[3]);
    }

    //Test GetDotProduct
    [Fact]
    public void GetDotProduct()
    {
      var a = new ComplexDoubleVector(new double[4] { 0, 1, 2, 3 });
      var b = new ComplexDoubleVector(new double[4] { 4, 5, 6, 7 });

      Assert.Equal(a.GetDotProduct(), (Complex)14);
      Assert.Equal(b.GetDotProduct(), (Complex)126);
      Assert.Equal(a.GetDotProduct(b), (Complex)38);
      Assert.Equal(a.GetDotProduct(b), b.GetDotProduct(a));
    }

    //Test GetNorm
    [Fact]
    public void GetNorm()
    {
      var a = new ComplexDoubleVector(new double[4] { 0, 1, 2, 3 });
      var b = new ComplexDoubleVector(new double[4] { 4, 5, 6, 7 });

      Assert.Equal(a.GetNorm(), System.Math.Sqrt(14));
      Assert.Equal(a.GetNorm(), a.GetNorm(2));
      Assert.Equal(3, a.GetNorm(0));

      Assert.Equal(b.GetNorm(), 3 * System.Math.Sqrt(14));
      Assert.Equal(b.GetNorm(), b.GetNorm(2));
      Assert.Equal(7, b.GetNorm(0));
    }

    //Test GetSum
    [Fact]
    public void GetSum()
    {
      var a = new ComplexDoubleVector(new double[4] { 0, 1, 2, 3 });
      var b = new ComplexDoubleVector(new double[4] { 4, 5, 6, 7 });

      Assert.Equal(a.GetSum(), (Complex)6);
      Assert.Equal(6, a.GetSumMagnitudes());

      Assert.Equal(b.GetSum(), (Complex)22);
      Assert.Equal(22, b.GetSumMagnitudes());
    }

    //Test Axpy and Scale
    [Fact]
    public void Axpy()
    {
      var a = new ComplexDoubleVector(new double[4] { 0, 1, 2, 3 });
      double scal = 3;
      var b = new ComplexDoubleVector(4);

      b.Axpy(scal, a);
      a.Scale(scal);

      Assert.Equal(a[0], b[0]);
      Assert.Equal(a[1], b[1]);
      Assert.Equal(a[2], b[2]);
      Assert.Equal(a[3], b[3]);
    }

    //Test Negate
    [Fact]
    public void Negate()
    {
      double[] vec = new double[4] { 0, 1, 2, 3 };
      var a = new ComplexDoubleVector(vec);
      ComplexDoubleVector b = -a;

      a = ComplexDoubleVector.Negate(a);

      Assert.Equal(-(Complex)vec[0], a[0]);
      Assert.Equal(-(Complex)vec[1], a[1]);
      Assert.Equal(-(Complex)vec[2], a[2]);
      Assert.Equal(-(Complex)vec[3], a[3]);

      Assert.Equal(-(Complex)vec[0], b[0]);
      Assert.Equal(-(Complex)vec[1], b[1]);
      Assert.Equal(-(Complex)vec[2], b[2]);
      Assert.Equal(-(Complex)vec[3], b[3]);
    }

    //Test Subtract
    [Fact]
    public void Subtract()
    {
      var a = new ComplexDoubleVector(new double[4] { 0, 1, 2, 3 });
      var b = new ComplexDoubleVector(new double[4] { 4, 5, 6, 7 });
      var c = new ComplexDoubleVector(a.Length);
      var d = new ComplexDoubleVector(b.Length);

      c = a - b;
      d = ComplexDoubleVector.Subtract(a, b);

      Assert.Equal(c[0], a[0] - b[0]);
      Assert.Equal(c[1], a[1] - b[1]);
      Assert.Equal(c[2], a[2] - b[2]);
      Assert.Equal(c[3], a[3] - b[3]);

      Assert.Equal(d[0], c[0]);
      Assert.Equal(d[1], c[1]);
      Assert.Equal(d[2], c[2]);
      Assert.Equal(d[3], c[3]);

      a.Subtract(b);

      Assert.Equal(c[0], a[0]);
      Assert.Equal(c[1], a[1]);
      Assert.Equal(c[2], a[2]);
      Assert.Equal(c[3], a[3]);
    }

    //Test Add
    [Fact]
    public void Add()
    {
      var a = new ComplexDoubleVector(new double[4] { 0, 1, 2, 3 });
      var b = new ComplexDoubleVector(new double[4] { 4, 5, 6, 7 });
      var c = new ComplexDoubleVector(a.Length);
      var d = new ComplexDoubleVector(b.Length);

      c = a + b;
      d = ComplexDoubleVector.Add(a, b);

      Assert.Equal(c[0], a[0] + b[0]);
      Assert.Equal(c[1], a[1] + b[1]);
      Assert.Equal(c[2], a[2] + b[2]);
      Assert.Equal(c[3], a[3] + b[3]);

      Assert.Equal(d[0], c[0]);
      Assert.Equal(d[1], c[1]);
      Assert.Equal(d[2], c[2]);
      Assert.Equal(d[3], c[3]);

      a.Add(b);

      Assert.Equal(c[0], a[0]);
      Assert.Equal(c[1], a[1]);
      Assert.Equal(c[2], a[2]);
      Assert.Equal(c[3], a[3]);
    }

    //Test Scale Mult and Divide
    [Fact]
    public void ScalarMultiplyAndDivide()
    {
      var a = new ComplexDoubleVector(new double[4] { 0, 1, 2, 3 });
      var c = new ComplexDoubleVector(a);
      var d = new ComplexDoubleVector(a);
      double scal = -4;

      c.Multiply(scal);
      d.Divide(scal);

      Assert.Equal(c[0], a[0] * scal);
      Assert.Equal(c[1], a[1] * scal);
      Assert.Equal(c[2], a[2] * scal);
      Assert.Equal(c[3], a[3] * scal);

      Assert.Equal(d[0], a[0] / scal);
      Assert.Equal(d[1], a[1] / scal);
      Assert.Equal(d[2], a[2] / scal);
      Assert.Equal(d[3], a[3] / scal);

      c = a * scal;

      Assert.Equal(c[0], a[0] * scal);
      Assert.Equal(c[1], a[1] * scal);
      Assert.Equal(c[2], a[2] * scal);
      Assert.Equal(c[3], a[3] * scal);

      c = scal * a;

      Assert.Equal(c[0], a[0] * scal);
      Assert.Equal(c[1], a[1] * scal);
      Assert.Equal(c[2], a[2] * scal);
      Assert.Equal(c[3], a[3] * scal);
    }

    //Test Multiply
    [Fact]
    public void Multiply()
    {
      var a = new ComplexDoubleVector(new double[4] { 0, 1, 2, 3 });
      var b = new ComplexDoubleVector(new double[4] { 4, 5, 6, 7 });
      var c = new ComplexDoubleMatrix(a.Length, b.Length);
      var d = new ComplexDoubleMatrix(a.Length, b.Length);

      c = a * b;
      d = ComplexDoubleVector.Multiply(a, b);

      Assert.Equal(c[0, 0], a[0] * b[0]);
      Assert.Equal(c[0, 1], a[0] * b[1]);
      Assert.Equal(c[0, 2], a[0] * b[2]);
      Assert.Equal(c[0, 3], a[0] * b[3]);
      Assert.Equal(c[1, 0], a[1] * b[0]);
      Assert.Equal(c[1, 1], a[1] * b[1]);
      Assert.Equal(c[1, 2], a[1] * b[2]);
      Assert.Equal(c[1, 3], a[1] * b[3]);
      Assert.Equal(c[2, 0], a[2] * b[0]);
      Assert.Equal(c[2, 1], a[2] * b[1]);
      Assert.Equal(c[2, 2], a[2] * b[2]);
      Assert.Equal(c[2, 3], a[2] * b[3]);
      Assert.Equal(c[3, 0], a[3] * b[0]);
      Assert.Equal(c[3, 1], a[3] * b[1]);
      Assert.Equal(c[3, 2], a[3] * b[2]);
      Assert.Equal(c[3, 3], a[3] * b[3]);

      Assert.Equal(d[0, 0], a[0] * b[0]);
      Assert.Equal(d[0, 1], a[0] * b[1]);
      Assert.Equal(d[0, 2], a[0] * b[2]);
      Assert.Equal(d[0, 3], a[0] * b[3]);
      Assert.Equal(d[1, 0], a[1] * b[0]);
      Assert.Equal(d[1, 1], a[1] * b[1]);
      Assert.Equal(d[1, 2], a[1] * b[2]);
      Assert.Equal(d[1, 3], a[1] * b[3]);
      Assert.Equal(d[2, 0], a[2] * b[0]);
      Assert.Equal(d[2, 1], a[2] * b[1]);
      Assert.Equal(d[2, 2], a[2] * b[2]);
      Assert.Equal(d[2, 3], a[2] * b[3]);
      Assert.Equal(d[3, 0], a[3] * b[0]);
      Assert.Equal(d[3, 1], a[3] * b[1]);
      Assert.Equal(d[3, 2], a[3] * b[2]);
      Assert.Equal(d[3, 3], a[3] * b[3]);
    }

    //Test Divide
    [Fact]
    public void Divide()
    {
      var a = new ComplexDoubleVector(new double[4] { 0, 1, 2, 3 });
      var c = new ComplexDoubleVector(a);
      var d = new ComplexDoubleVector(a);
      double scal = -4;

      c = a / scal;
      d = ComplexDoubleVector.Divide(a, scal);

      Assert.Equal(c[0], a[0] / scal);
      Assert.Equal(c[1], a[1] / scal);
      Assert.Equal(c[2], a[2] / scal);
      Assert.Equal(c[3], a[3] / scal);

      Assert.Equal(d[0], a[0] / scal);
      Assert.Equal(d[1], a[1] / scal);
      Assert.Equal(d[2], a[2] / scal);
      Assert.Equal(d[3], a[3] / scal);
    }

    //Test Clone
    [Fact]
    public void Clone()
    {
      var a = new ComplexDoubleVector(new double[4] { 0, 1, 2, 3 });
      ComplexDoubleVector b = a.Clone();

      Assert.Equal(a[0], b[0]);
      Assert.Equal(a[1], b[1]);
      Assert.Equal(a[2], b[2]);
      Assert.Equal(a[3], b[3]);

      a = a * 2;

      Assert.Equal(a[0], b[0] * 2);
      Assert.Equal(a[1], b[1] * 2);
      Assert.Equal(a[2], b[2] * 2);
      Assert.Equal(a[3], b[3] * 2);
    }

    //Test IEnumerable and DoubleVectorEnumerator
    [Fact]
    public void GetEnumeratorException()
    {
      Assert.Throws<InvalidOperationException>(() =>
      {
        var a = new ComplexDoubleVector(new Complex[4] { 0, 1, 2, 3 });
        IEnumerator dve = a.GetEnumerator();

        var b = (Complex)dve.Current;
      });
    }

    //Test IEnumerable and DoubleVectorEnumerator
    [Fact]
    public void GetEnumerator()
    {
      var a = new ComplexDoubleVector(new Complex[4] { 0, 1, 2, 3 });
      IEnumerator dve = a.GetEnumerator();
      Complex b;
      bool c;

      c = dve.MoveNext();
      b = (Complex)dve.Current;
      Assert.True(c);
      Assert.Equal(b, (Complex)0);

      c = dve.MoveNext();
      b = (Complex)dve.Current;
      Assert.True(c);
      Assert.Equal(b, (Complex)1);

      c = dve.MoveNext();
      b = (Complex)dve.Current;
      Assert.True(c);
      Assert.Equal(b, (Complex)2);

      c = dve.MoveNext();
      b = (Complex)dve.Current;
      Assert.True(c);
      Assert.Equal(b, (Complex)3);

      c = dve.MoveNext();
      Assert.False(c);
    }

    //Partial ICollection tests
    [Fact]
    public void ICollection()
    {
      var a = new ComplexDoubleVector(new Complex[4] { 0, 1, 2, 3 });
      var b = new Complex[5];

      Assert.Equal(a.Count, a.Length);

      a.CopyTo(b, 1);
      Assert.Equal(b[0], (Complex)0);
      Assert.Equal(b[1], (Complex)0);
      Assert.Equal(b[2], (Complex)1);
      Assert.Equal(b[3], (Complex)2);
      Assert.Equal(b[4], (Complex)3);
    }

    // IList tests
    [Fact]
    public void IList()
    {
      var a = new ComplexDoubleVector(new Complex[4] { 0, 1, 2, 3 });

      Assert.False(a.IsFixedSize);
      Assert.False(a.IsReadOnly);

      a.Add((Complex)4.0);
      Assert.Equal(5, a.Length);
      Assert.Equal(a[4], (Complex)4);
      Assert.True(a.Contains((Complex)4.0));

      a.Insert(1, (Complex)5.0);
      Assert.Equal(6, a.Length);
      Assert.True(a.Contains((Complex)5.0));
      Assert.Equal(a[0], (Complex)0);
      Assert.Equal(a[1], (Complex)5);
      Assert.Equal(a[2], (Complex)1);
      Assert.Equal(a[3], (Complex)2);
      Assert.Equal(a[4], (Complex)3);
      Assert.Equal(a[5], (Complex)4);

      a.Remove((Complex)5.0);
      Assert.Equal(5, a.Length);
      Assert.False(a.Contains((Complex)5.0));
      Assert.Equal(a[0], (Complex)0);
      Assert.Equal(a[1], (Complex)1);
      Assert.Equal(a[2], (Complex)2);
      Assert.Equal(a[3], (Complex)3);
      Assert.Equal(a[4], (Complex)4);

      a.RemoveAt(2);
      Assert.Equal(4, a.Length);
      Assert.False(a.Contains((Complex)2.0));
      Assert.Equal(a[0], (Complex)0);
      Assert.Equal(a[1], (Complex)1);
      Assert.Equal(a[2], (Complex)3);
      Assert.Equal(a[3], (Complex)4);
    }
  }
}
