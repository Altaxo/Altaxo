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
using Altaxo.Calc.LinearAlgebra;
using Xunit;

namespace AltaxoTest.Calc.LinearAlgebra
{

  public class FloatVectorTest
  {
    private const float TOLERANCE = 0.001f;

    //Test dimensions Constructor.
    [Fact]
    public void CtorDimensions()
    {
      var test = new FloatVector(2);

      Assert.Equal(2, test.Length);
      Assert.Equal(0, test[0]);
      Assert.Equal(0, test[1]);
    }

    //Test Copy Constructor.
    [Fact]
    public void CtorDimensionsNegative()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var test = new FloatVector(-1);
      });
    }

    //Test Intital Values Constructor.
    [Fact]
    public void CtorInitialValues()
    {
      var test = new FloatVector(2, 1);

      Assert.Equal(2, test.Length);
      Assert.Equal(1, test[0]);
      Assert.Equal(1, test[1]);
    }

    //Test Array Constructor
    [Fact]
    public void CtorArray()
    {
      float[] testvector = new float[2] { 0, 1 };

      var test = new FloatVector(testvector);
      Assert.Equal(test.Length, testvector.Length);
      Assert.Equal(test[0], testvector[0]);
      Assert.Equal(test[1], testvector[1]);
    }

    //*TODO IList Constructor

    //Test Copy Constructor.
    [Fact]
    public void CtorCopy()
    {
      var a = new FloatVector(new float[2] { 0, 1 });
      var b = new FloatVector(a);

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
        FloatVector a = null;
        var b = new FloatVector(a);
      });
    }

    //Test Index Access
    [Fact]
    public void IndexAccessGetNegative()
    {
      Assert.Throws<IndexOutOfRangeException>(() =>
      {
        var a = new FloatVector(new float[2] { 0, 1 });
        float b = a[-1];
      });
    }

    //Test Index Access
    [Fact]
    public void IndexAccessSetNegative()
    {
      Assert.Throws<IndexOutOfRangeException>(() =>
      {
        var a = new FloatVector(2)
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
        var a = new FloatVector(new float[2] { 0, 1 });
        float b = a[2];
      });
    }

    //Test Index Access
    [Fact]
    public void IndexAccessSetOutOfRange()
    {
      Assert.Throws<IndexOutOfRangeException>(() =>
      {
        var a = new FloatVector(2)
        {
          [2] = 1
        };
      });
    }

    //Test Equals
    [Fact]
    public void TestEquals()
    {
      var a = new FloatVector(2, 4);
      var b = new FloatVector(2, 4);
      var c = new FloatVector(2)
      {
        [0] = 4,
        [1] = 4
      };

      var d = new FloatVector(2, 5);
      FloatVector e = null;
      Assert.True(a.Equals(b));
      Assert.True(b.Equals(a));
      Assert.True(a.Equals(c));
      Assert.True(b.Equals(c));
      Assert.True(c.Equals(b));
      Assert.True(c.Equals(a));
      Assert.False(a.Equals(d));
      Assert.False(d.Equals(b));
      Assert.False(a.Equals(e));
    }

    //test GetHashCode
    [Fact]
    public void TestHashCode()
    {
      var a = new FloatVector(2)
      {
        [0] = 0,
        [1] = 1
      };
      int hash = a.GetHashCode();
      Assert.Equal(-1333788670, hash);
    }

    //Test GetInternalData
    [Fact]
    public void GetInternalData()
    {
      float[] testvector = new float[2] { 0, 1 };
      var test = new FloatVector(testvector);
      float[] internaldata = test.GetInternalData();

      Assert.Equal(internaldata.Length, testvector.Length);
      Assert.Equal(internaldata[0], testvector[0]);
      Assert.Equal(internaldata[1], testvector[1]);
    }

    //Test ToArray
    [Fact]
    public void ToArray()
    {
      float[] testvector = new float[2] { 0, 1 };
      var test = new FloatVector(testvector);
      float[] internaldata = test.ToArray();

      Assert.Equal(internaldata.Length, testvector.Length);
      Assert.Equal(internaldata[0], testvector[0]);
      Assert.Equal(internaldata[1], testvector[1]);
    }

    //Test GetSubVector
    [Fact]
    public void GetSubVector()
    {
      var test = new FloatVector(new float[4] { 0, 1, 2, 3 });
      FloatVector subvector = test.GetSubVector(1, 2);

      Assert.Equal(2, subvector.Length);
      Assert.Equal(subvector[0], test[1]);
      Assert.Equal(subvector[1], test[2]);
    }

    //Test Implicit cast conversion to FloatVector
    [Fact]
    public void ImplicitConversion()
    {
      float[] a = new float[4] { 0, 1, 2, 3 };
      float[] b = new float[4] { 0, 1, 2, 3 };
      var c = new FloatVector(a);
      FloatVector d, e, f;

      d = a;
      e = b;
      f = c;

      Assert.Equal(a.Length, d.Length);
      Assert.Equal(a[0], d[0]);
      Assert.Equal(a[1], d[1]);
      Assert.Equal(a[2], d[2]);
      Assert.Equal(a[3], d[3]);

      Assert.Equal(b.Length, e.Length);
      Assert.Equal(b[0], e[0]);
      Assert.Equal(b[1], e[1]);
      Assert.Equal(b[2], e[2]);
      Assert.Equal(b[3], e[3]);

      Assert.Equal(c.Length, f.Length);
      Assert.Equal(c[0], f[0]);
      Assert.Equal(c[1], f[1]);
      Assert.Equal(c[2], f[2]);
      Assert.Equal(c[3], f[3]);
    }

    //Test Explicit cast convertion to FloatVector
    [Fact]
    public void ExplicitConversion()
    {
      var a = new DoubleVector(new double[4] { 0, 1, 2, 3 });
      var b = (FloatVector)a;

      Assert.Equal(a.Length, b.Length);
      Assert.Equal((float)a[0], b[0]);
      Assert.Equal((float)a[1], b[1]);
      Assert.Equal((float)a[2], b[2]);
      Assert.Equal((float)a[3], b[3]);
    }

    //Test GetIndex functions
    [Fact]
    public void GetIndex()
    {
      var a = new FloatVector(new float[4] { 1, 2, 3, 4 });
      var b = new FloatVector(new float[4] { 3, 2, 1, 0 });
      var c = new FloatVector(new float[4] { 0, -1, -2, -3 });
      var d = new FloatVector(new float[4] { -3, -2, -1, 0 });

      Assert.Equal(3, a.GetAbsMaximumIndex());
      Assert.Equal(0, b.GetAbsMaximumIndex());
      Assert.Equal(3, c.GetAbsMaximumIndex());
      Assert.Equal(0, d.GetAbsMaximumIndex());

      Assert.Equal(4, a.GetAbsMaximum());
      Assert.Equal(3, b.GetAbsMaximum());
      Assert.Equal(c.GetAbsMaximum(), -3);
      Assert.Equal(d.GetAbsMaximum(), -3);

      Assert.Equal(0, a.GetAbsMinimumIndex());
      Assert.Equal(3, b.GetAbsMinimumIndex());
      Assert.Equal(0, c.GetAbsMinimumIndex());
      Assert.Equal(3, d.GetAbsMinimumIndex());

      Assert.Equal(1, a.GetAbsMinimum());
      Assert.Equal(0, b.GetAbsMinimum());
      Assert.Equal(0, c.GetAbsMinimum());
      Assert.Equal(0, d.GetAbsMinimum());
    }

    //Test invalid dimensions with copy
    [Fact]
    public void CopyException()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new FloatVector(new float[4] { 0, 1, 2, 3 });
        var b = new FloatVector(5);

        a.CopyFrom(b);
      });
    }

    //Test invalid dimensions with swap
    [Fact]
    public void SwapException()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new FloatVector(new float[4] { 0, 1, 2, 3 });
        var b = new FloatVector(new float[5] { 4, 5, 6, 7, 8 });

        a.Swap(b);
      });
    }

    //Test Copy and Swap
    [Fact]
    public void CopySwap()
    {
      var a = new FloatVector(new float[4] { 0, 1, 2, 3 });
      var b = new FloatVector(new float[4] { 4, 5, 6, 7 });
      var c = new FloatVector(4);
      var d = new FloatVector(4);

      a.CopyFrom(c);
      b.CopyFrom(d);

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
      var a = new FloatVector(new float[4] { 0, 1, 2, 3 });
      var b = new FloatVector(new float[4] { 4, 5, 6, 7 });

      Assert.Equal(14, a.GetDotProduct());
      Assert.Equal(126, b.GetDotProduct());
      Assert.Equal(38, a.GetDotProduct(b));
      Assert.Equal(a.GetDotProduct(b), b.GetDotProduct(a));
    }

    //Test GetNorm
    [Fact]
    public void GetNorm()
    {
      var a = new FloatVector(new float[4] { 0, 1, 2, 3 });
      var b = new FloatVector(new float[4] { 4, 5, 6, 7 });

      AssertEx.Equal(a.L2Norm, System.Math.Sqrt(14), TOLERANCE);
      Assert.Equal(a.L2Norm, a.LpNorm(2));
      Assert.Equal(3, a.LpNorm(0));

      AssertEx.Equal(b.L2Norm, 3 * System.Math.Sqrt(14), TOLERANCE);
      Assert.Equal(b.L2Norm, b.LpNorm(2));
      Assert.Equal(7, b.LpNorm(0));
    }

    //Test GetSum
    [Fact]
    public void GetSum()
    {
      var a = new FloatVector(new float[4] { 0, 1, 2, 3 });
      var b = new FloatVector(new float[4] { 4, 5, 6, 7 });

      Assert.Equal(6, a.GetSum());
      Assert.Equal(a.GetSum(), a.GetSumMagnitudes());

      Assert.Equal(22, b.GetSum());
      Assert.Equal(b.GetSum(), b.GetSumMagnitudes());
    }

    //Test Axpy and Scale
    [Fact]
    public void Axpy()
    {
      var a = new FloatVector(new float[4] { 0, 1, 2, 3 });
      float scal = 3;
      var b = new FloatVector(4);

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
      float[] vec = new float[4] { 0, 1, 2, 3 };
      var a = new FloatVector(vec);
      FloatVector b = -a;

      a = FloatVector.Negate(a);

      Assert.Equal(-vec[0], a[0]);
      Assert.Equal(-vec[1], a[1]);
      Assert.Equal(-vec[2], a[2]);
      Assert.Equal(-vec[3], a[3]);

      Assert.Equal(-vec[0], b[0]);
      Assert.Equal(-vec[1], b[1]);
      Assert.Equal(-vec[2], b[2]);
      Assert.Equal(-vec[3], b[3]);
    }

    //Test Subtract
    [Fact]
    public void Subtract()
    {
      var a = new FloatVector(new float[4] { 0, 1, 2, 3 });
      var b = new FloatVector(new float[4] { 4, 5, 6, 7 });
      var c = new FloatVector(a.Length);
      var d = new FloatVector(b.Length);

      c = a - b;
      d = FloatVector.Subtract(a, b);

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
      var a = new FloatVector(new float[4] { 0, 1, 2, 3 });
      var b = new FloatVector(new float[4] { 4, 5, 6, 7 });
      var c = new FloatVector(a.Length);
      var d = new FloatVector(b.Length);

      c = a + b;
      d = FloatVector.Add(a, b);

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
      var a = new FloatVector(new float[4] { 0, 1, 2, 3 });
      var c = new FloatVector(a);
      var d = new FloatVector(a);
      float scal = -4;

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
      var a = new FloatVector(new float[4] { 0, 1, 2, 3 });
      var b = new FloatVector(new float[4] { 4, 5, 6, 7 });
      var c = new DoubleMatrix(a.Length, b.Length);
      var d = new DoubleMatrix(a.Length, b.Length);

      c = a * b;
      d = FloatVector.Multiply(a, b);

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
      var a = new FloatVector(new float[4] { 0, 1, 2, 3 });
      var c = new FloatVector(a);
      var d = new FloatVector(a);
      float scal = -4;

      c = a / scal;
      d = FloatVector.Divide(a, scal);

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
      var a = new FloatVector(new float[4] { 0, 1, 2, 3 });
      FloatVector b = a.Clone();

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
      Assert.Throws<InvalidCastException>(() =>
      {
        var a = new FloatVector(new float[4] { 0, 1, 2, 3 });
        IEnumerator dve = a.GetEnumerator();

        double b = (double)dve.Current;
      });
    }

    //Test IEnumerable and DoubleVectorEnumerator
    [Fact]
    public void GetEnumerator()
    {
      var a = new FloatVector(new float[4] { 0, 1, 2, 3 });
      IEnumerator dve = a.GetEnumerator();
      float b;
      bool c;

      c = dve.MoveNext();
      b = (float)dve.Current;
      Assert.True(c);
      Assert.Equal(0, b);

      c = dve.MoveNext();
      b = (float)dve.Current;
      Assert.True(c);
      Assert.Equal(1, b);

      c = dve.MoveNext();
      b = (float)dve.Current;
      Assert.True(c);
      Assert.Equal(2, b);

      c = dve.MoveNext();
      b = (float)dve.Current;
      Assert.True(c);
      Assert.Equal(3, b);

      c = dve.MoveNext();
      Assert.False(c);
    }
  }
}
