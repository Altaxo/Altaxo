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
using NUnit.Framework;

namespace AltaxoTest.Calc.LinearAlgebra
{
  [TestFixture]
  public class ComplexFloatVectorTest
  {
    private const float TOLERENCE = 0.001f;

    //Test dimensions Constructor.
    [Test]
    public void CtorDimensions()
    {
      var test = new ComplexFloatVector(2);

      Assert.AreEqual(test.Length, 2);
      Assert.AreEqual(test[0], (ComplexFloat)0);
      Assert.AreEqual(test[1], (ComplexFloat)0);
    }

    //Test Copy Constructor.
    [Test]
    public void CtorDimensionsZero()
    {
      Assert.Throws(typeof(ArgumentException), () =>
      {
        var test = new ComplexFloatVector(0);
      });
    }

    //Test Copy Constructor.
    [Test]
    public void CtorDimensionsNegative()
    {
      Assert.Throws(typeof(ArgumentException), () =>
      {
        var test = new ComplexFloatVector(-1);
      });
    }

    //Test Intital Values Constructor.
    [Test]
    public void CtorInitialValues()
    {
      var test = new ComplexFloatVector(2, 1);

      Assert.AreEqual(test.Length, 2);
      Assert.AreEqual(test[0], (ComplexFloat)1);
      Assert.AreEqual(test[1], (ComplexFloat)1);
    }

    //Test Array Constructor
    [Test]
    public void CtorArray()
    {
      float[] testvector = new float[2] { 0, 1 };

      var test = new ComplexFloatVector(testvector);
      Assert.AreEqual(test.Length, testvector.Length);
      Assert.AreEqual(test[0], (ComplexFloat)testvector[0]);
      Assert.AreEqual(test[1], (ComplexFloat)testvector[1]);
    }

    //*TODO IList Constructor

    //Test Copy Constructor.
    [Test]
    public void CtorCopy()
    {
      var a = new ComplexFloatVector(new float[2] { 0, 1 });
      var b = new ComplexFloatVector(a);

      Assert.AreEqual(b.Length, a.Length);
      Assert.AreEqual(b[0], a[0]);
      Assert.AreEqual(b[1], a[1]);
    }

    //Test Copy Constructor.
    [Test]
    public void CtorCopyNull()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        ComplexFloatVector a = null;
        var b = new ComplexFloatVector(a);
      });
    }

    //Test Index Access
    [Test]
    public void IndexAccessGetNegative()
    {
      Assert.Throws(typeof(IndexOutOfRangeException), () =>
      {
        var a = new ComplexFloatVector(new float[2] { 0, 1 });
        ComplexFloat b = a[-1];
      });
    }

    //Test Index Access
    [Test]
    public void IndexAccessSetNegative()
    {
      Assert.Throws(typeof(IndexOutOfRangeException), () =>
      {
        var a = new ComplexFloatVector(2)
        {
          [-1] = 1
        };
      });
    }

    //Test Index Access
    [Test]
    public void IndexAccessGetOutOfRange()
    {
      Assert.Throws(typeof(IndexOutOfRangeException), () =>
      {
        var a = new ComplexFloatVector(new float[2] { 0, 1 });
        ComplexFloat b = a[2];
      });
    }

    //Test Index Access
    [Test]
    public void IndexAccessSetOutOfRange()
    {
      Assert.Throws(typeof(IndexOutOfRangeException), () =>
      {
        var a = new ComplexFloatVector(2)
        {
          [2] = 1
        };
      });
    }

    //Test Equals
    [Test]
    public void Equals()
    {
      var a = new ComplexFloatVector(2, 4);
      var b = new ComplexFloatVector(2, 4);
      var c = new ComplexFloatVector(2)
      {
        [0] = 4,
        [1] = 4
      };

      var d = new ComplexFloatVector(2, 5);
      ComplexFloatVector e = null;
      Assert.IsTrue(a.Equals(b));
      Assert.IsTrue(b.Equals(a));
      Assert.IsTrue(a.Equals(c));
      Assert.IsTrue(b.Equals(c));
      Assert.IsTrue(c.Equals(b));
      Assert.IsTrue(c.Equals(a));
      Assert.IsFalse(a.Equals(d));
      Assert.IsFalse(d.Equals(b));
      Assert.IsFalse(a.Equals(e));
    }

    //Test get real and imaginary components
    [Test]
    public void RealImag()
    {
      var a = new ComplexFloatVector(2)
      {
        [0] = new ComplexFloat(1, 2),
        [1] = new ComplexFloat(3, 4)
      };

      FloatVector a_real = a.Real;
      FloatVector a_imag = a.Imag;

      Assert.AreEqual(a_real[0], a[0].Real);
      Assert.AreEqual(a_imag[0], a[0].Imag);
      Assert.AreEqual(a_real[1], a[1].Real);
      Assert.AreEqual(a_imag[1], a[1].Imag);
      Assert.AreEqual(a_real.Length, a.Length);
      Assert.AreEqual(a_imag.Length, a.Length);
    }

    //test GetHashCode
    [Test]
    public void TestHashCode()
    {
      var a = new ComplexFloatVector(2)
      {
        [0] = 0,
        [1] = 1
      };

      int hash = a.GetHashCode();
      Assert.AreEqual(hash, 1);
    }

    //Test GetInternalData
    [Test]
    public void GetInternalData()
    {
      float[] testvector = new float[2] { 0, 1 };
      var test = new ComplexFloatVector(testvector);
      ComplexFloat[] internaldata = test.GetInternalData();

      Assert.AreEqual(internaldata.Length, testvector.Length);
      Assert.AreEqual(internaldata[0], (ComplexFloat)testvector[0]);
      Assert.AreEqual(internaldata[1], (ComplexFloat)testvector[1]);
    }

    //Test ToArray
    [Test]
    public void ToArray()
    {
      float[] testvector = new float[2] { 0, 1 };
      var test = new ComplexFloatVector(testvector);
      ComplexFloat[] internaldata = test.ToArray();

      Assert.AreEqual(internaldata.Length, testvector.Length);
      Assert.AreEqual(internaldata[0], (ComplexFloat)testvector[0]);
      Assert.AreEqual(internaldata[1], (ComplexFloat)testvector[1]);
    }

    //Test GetSubVector
    [Test]
    public void GetSubVector()
    {
      var test = new ComplexFloatVector(new float[4] { 0, 1, 2, 3 });
      ComplexFloatVector subvector = test.GetSubVector(1, 2);

      Assert.AreEqual(subvector.Length, 2);
      Assert.AreEqual(subvector[0], test[1]);
      Assert.AreEqual(subvector[1], test[2]);
    }

    //Test Implicit cast conversion to ComplexFloatVector
    [Test]
    public void ImplicitConversion()
    {
      float[] a = new float[4] { 0, 1, 2, 3 };
      float[] b = new float[4] { 0, 1, 2, 3 };
      var c = new ComplexFloatVector(a);
      ComplexFloatVector d, e, f;

      d = a;
      e = b;
      f = c;

      Assert.AreEqual(a.Length, d.Length);
      Assert.AreEqual((ComplexFloat)a[0], d[0]);
      Assert.AreEqual((ComplexFloat)a[1], d[1]);
      Assert.AreEqual((ComplexFloat)a[2], d[2]);
      Assert.AreEqual((ComplexFloat)a[3], d[3]);

      Assert.AreEqual(b.Length, e.Length);
      Assert.AreEqual((ComplexFloat)b[0], e[0]);
      Assert.AreEqual((ComplexFloat)b[1], e[1]);
      Assert.AreEqual((ComplexFloat)b[2], e[2]);
      Assert.AreEqual((ComplexFloat)b[3], e[3]);

      Assert.AreEqual(c.Length, f.Length);
      Assert.AreEqual(c[0], f[0]);
      Assert.AreEqual(c[1], f[1]);
      Assert.AreEqual(c[2], f[2]);
      Assert.AreEqual(c[3], f[3]);
    }

    //Test Explicit cast convertion to ComplexFloatVector
    [Test]
    public void ExplicitConversion()
    {
      var a = new ComplexDoubleVector(new double[4] { 0, 1, 2, 3 });
      var b = (ComplexFloatVector)a;

      Assert.AreEqual(a.Length, b.Length);
      Assert.AreEqual((ComplexFloat)a[0], b[0]);
      Assert.AreEqual((ComplexFloat)a[1], b[1]);
      Assert.AreEqual((ComplexFloat)a[2], b[2]);
      Assert.AreEqual((ComplexFloat)a[3], b[3]);
    }

    //Test GetIndex functions
    [Test]
    public void GetIndex()
    {
      var a = new ComplexFloatVector(new float[4] { 1, 2, 3, 4 });
      var b = new ComplexFloatVector(new float[4] { 3, 2, 1, 0 });
      var c = new ComplexFloatVector(new float[4] { 0, -1, -2, -3 });
      var d = new ComplexFloatVector(new float[4] { -3, -2, -1, 0 });

      Assert.AreEqual(a.GetAbsMaximumIndex(), 3);
      Assert.AreEqual(b.GetAbsMaximumIndex(), 0);
      Assert.AreEqual(c.GetAbsMaximumIndex(), 3);
      Assert.AreEqual(d.GetAbsMaximumIndex(), 0);

      Assert.AreEqual(a.GetAbsMaximum(), (ComplexFloat)4);
      Assert.AreEqual(b.GetAbsMaximum(), (ComplexFloat)3);
      Assert.AreEqual(c.GetAbsMaximum(), (ComplexFloat)(-3));
      Assert.AreEqual(d.GetAbsMaximum(), (ComplexFloat)(-3));

      Assert.AreEqual(a.GetAbsMinimumIndex(), 0);
      Assert.AreEqual(b.GetAbsMinimumIndex(), 3);
      Assert.AreEqual(c.GetAbsMinimumIndex(), 0);
      Assert.AreEqual(d.GetAbsMinimumIndex(), 3);

      Assert.AreEqual(a.GetAbsMinimum(), (ComplexFloat)1);
      Assert.AreEqual(b.GetAbsMinimum(), (ComplexFloat)0);
      Assert.AreEqual(c.GetAbsMinimum(), (ComplexFloat)0);
      Assert.AreEqual(d.GetAbsMinimum(), (ComplexFloat)0);
    }

    //Test invalid dimensions with copy
    [Test]
    public void CopyException()
    {
      Assert.Throws(typeof(ArgumentException), () =>
      {
        var a = new ComplexFloatVector(new float[4] { 0, 1, 2, 3 });
        var b = new ComplexFloatVector(5);

        a.Copy(b);
      });
    }

    //Test invalid dimensions with swap
    [Test]
    public void SwapException()
    {
      Assert.Throws(typeof(ArgumentException), () =>
      {
        var a = new ComplexFloatVector(new float[4] { 0, 1, 2, 3 });
        var b = new ComplexFloatVector(new float[5] { 4, 5, 6, 7, 8 });

        a.Swap(b);
      });
    }

    //Test Copy and Swap
    [Test]
    public void CopySwap()
    {
      var a = new ComplexFloatVector(new float[4] { 0, 1, 2, 3 });
      var b = new ComplexFloatVector(new float[4] { 4, 5, 6, 7 });
      var c = new ComplexFloatVector(4);
      var d = new ComplexFloatVector(4);

      a.Copy(c);
      b.Copy(d);

      Assert.AreEqual(a.Length, c.Length);
      Assert.AreEqual(a[0], c[0]);
      Assert.AreEqual(a[1], c[1]);
      Assert.AreEqual(a[2], c[2]);
      Assert.AreEqual(a[3], c[3]);

      Assert.AreEqual(b.Length, d.Length);
      Assert.AreEqual(b[0], d[0]);
      Assert.AreEqual(b[1], d[1]);
      Assert.AreEqual(b[2], d[2]);
      Assert.AreEqual(b[3], d[3]);

      a.Swap(b);

      Assert.AreEqual(b.Length, c.Length);
      Assert.AreEqual(b[0], c[0]);
      Assert.AreEqual(b[1], c[1]);
      Assert.AreEqual(b[2], c[2]);
      Assert.AreEqual(b[3], c[3]);

      Assert.AreEqual(a.Length, d.Length);
      Assert.AreEqual(a[0], d[0]);
      Assert.AreEqual(a[1], d[1]);
      Assert.AreEqual(a[2], d[2]);
      Assert.AreEqual(a[3], d[3]);
    }

    //Test GetDotProduct
    [Test]
    public void GetDotProduct()
    {
      var a = new ComplexFloatVector(new float[4] { 0, 1, 2, 3 });
      var b = new ComplexFloatVector(new float[4] { 4, 5, 6, 7 });

      Assert.AreEqual(a.GetDotProduct(), (ComplexFloat)14);
      Assert.AreEqual(b.GetDotProduct(), (ComplexFloat)126);
      Assert.AreEqual(a.GetDotProduct(b), (ComplexFloat)38);
      Assert.AreEqual(a.GetDotProduct(b), b.GetDotProduct(a));
    }

    //Test GetNorm
    [Test]
    public void GetNorm()
    {
      var a = new ComplexFloatVector(new float[4] { 0, 1, 2, 3 });
      var b = new ComplexFloatVector(new float[4] { 4, 5, 6, 7 });

      Assert.AreEqual(a.GetNorm(), System.Math.Sqrt(14), TOLERENCE);
      Assert.AreEqual(a.GetNorm(), a.GetNorm(2));
      Assert.AreEqual(a.GetNorm(0), 3);

      Assert.AreEqual(b.GetNorm(), 3 * System.Math.Sqrt(14), TOLERENCE);
      Assert.AreEqual(b.GetNorm(), b.GetNorm(2));
      Assert.AreEqual(b.GetNorm(0), 7);
    }

    //Test GetSum
    [Test]
    public void GetSum()
    {
      var a = new ComplexFloatVector(new float[4] { 0, 1, 2, 3 });
      var b = new ComplexFloatVector(new float[4] { 4, 5, 6, 7 });

      Assert.AreEqual(a.GetSum(), (ComplexFloat)6);
      Assert.AreEqual(6, a.GetSumMagnitudes());

      Assert.AreEqual(b.GetSum(), (ComplexFloat)22);
      Assert.AreEqual(22, b.GetSumMagnitudes());
    }

    //Test Axpy and Scale
    [Test]
    public void Axpy()
    {
      var a = new ComplexFloatVector(new float[4] { 0, 1, 2, 3 });
      float scal = 3;
      var b = new ComplexFloatVector(4);

      b.Axpy(scal, a);
      a.Scale(scal);

      Assert.AreEqual(a[0], b[0]);
      Assert.AreEqual(a[1], b[1]);
      Assert.AreEqual(a[2], b[2]);
      Assert.AreEqual(a[3], b[3]);
    }

    //Test Negate
    [Test]
    public void Negate()
    {
      float[] vec = new float[4] { 0, 1, 2, 3 };
      var a = new ComplexFloatVector(vec);
      ComplexFloatVector b = -a;

      a = ComplexFloatVector.Negate(a);

      Assert.AreEqual(-(ComplexFloat)vec[0], a[0]);
      Assert.AreEqual(-(ComplexFloat)vec[1], a[1]);
      Assert.AreEqual(-(ComplexFloat)vec[2], a[2]);
      Assert.AreEqual(-(ComplexFloat)vec[3], a[3]);

      Assert.AreEqual(-(ComplexFloat)vec[0], b[0]);
      Assert.AreEqual(-(ComplexFloat)vec[1], b[1]);
      Assert.AreEqual(-(ComplexFloat)vec[2], b[2]);
      Assert.AreEqual(-(ComplexFloat)vec[3], b[3]);
    }

    //Test Subtract
    [Test]
    public void Subtract()
    {
      var a = new ComplexFloatVector(new float[4] { 0, 1, 2, 3 });
      var b = new ComplexFloatVector(new float[4] { 4, 5, 6, 7 });
      var c = new ComplexFloatVector(a.Length);
      var d = new ComplexFloatVector(b.Length);

      c = a - b;
      d = ComplexFloatVector.Subtract(a, b);

      Assert.AreEqual(c[0], a[0] - b[0]);
      Assert.AreEqual(c[1], a[1] - b[1]);
      Assert.AreEqual(c[2], a[2] - b[2]);
      Assert.AreEqual(c[3], a[3] - b[3]);

      Assert.AreEqual(d[0], c[0]);
      Assert.AreEqual(d[1], c[1]);
      Assert.AreEqual(d[2], c[2]);
      Assert.AreEqual(d[3], c[3]);

      a.Subtract(b);

      Assert.AreEqual(c[0], a[0]);
      Assert.AreEqual(c[1], a[1]);
      Assert.AreEqual(c[2], a[2]);
      Assert.AreEqual(c[3], a[3]);
    }

    //Test Add
    [Test]
    public void Add()
    {
      var a = new ComplexFloatVector(new float[4] { 0, 1, 2, 3 });
      var b = new ComplexFloatVector(new float[4] { 4, 5, 6, 7 });
      var c = new ComplexFloatVector(a.Length);
      var d = new ComplexFloatVector(b.Length);

      c = a + b;
      d = ComplexFloatVector.Add(a, b);

      Assert.AreEqual(c[0], a[0] + b[0]);
      Assert.AreEqual(c[1], a[1] + b[1]);
      Assert.AreEqual(c[2], a[2] + b[2]);
      Assert.AreEqual(c[3], a[3] + b[3]);

      Assert.AreEqual(d[0], c[0]);
      Assert.AreEqual(d[1], c[1]);
      Assert.AreEqual(d[2], c[2]);
      Assert.AreEqual(d[3], c[3]);

      a.Add(b);

      Assert.AreEqual(c[0], a[0]);
      Assert.AreEqual(c[1], a[1]);
      Assert.AreEqual(c[2], a[2]);
      Assert.AreEqual(c[3], a[3]);
    }

    //Test Scale Mult and Divide
    [Test]
    public void ScalarMultiplyAndDivide()
    {
      var a = new ComplexFloatVector(new float[4] { 0, 1, 2, 3 });
      var c = new ComplexFloatVector(a);
      var d = new ComplexFloatVector(a);
      float scal = -4;

      c.Multiply(scal);
      d.Divide(scal);

      Assert.AreEqual(c[0], a[0] * scal);
      Assert.AreEqual(c[1], a[1] * scal);
      Assert.AreEqual(c[2], a[2] * scal);
      Assert.AreEqual(c[3], a[3] * scal);

      Assert.AreEqual(d[0], a[0] / scal);
      Assert.AreEqual(d[1], a[1] / scal);
      Assert.AreEqual(d[2], a[2] / scal);
      Assert.AreEqual(d[3], a[3] / scal);

      c = a * scal;

      Assert.AreEqual(c[0], a[0] * scal);
      Assert.AreEqual(c[1], a[1] * scal);
      Assert.AreEqual(c[2], a[2] * scal);
      Assert.AreEqual(c[3], a[3] * scal);

      c = scal * a;

      Assert.AreEqual(c[0], a[0] * scal);
      Assert.AreEqual(c[1], a[1] * scal);
      Assert.AreEqual(c[2], a[2] * scal);
      Assert.AreEqual(c[3], a[3] * scal);
    }

    //Test Multiply
    [Test]
    public void Multiply()
    {
      var a = new ComplexFloatVector(new float[4] { 0, 1, 2, 3 });
      var b = new ComplexFloatVector(new float[4] { 4, 5, 6, 7 });
      var c = new ComplexDoubleMatrix(a.Length, b.Length);
      var d = new ComplexDoubleMatrix(a.Length, b.Length);

      c = a * b;
      d = ComplexFloatVector.Multiply(a, b);

      Assert.AreEqual(c[0, 0], a[0] * b[0]);
      Assert.AreEqual(c[0, 1], a[0] * b[1]);
      Assert.AreEqual(c[0, 2], a[0] * b[2]);
      Assert.AreEqual(c[0, 3], a[0] * b[3]);
      Assert.AreEqual(c[1, 0], a[1] * b[0]);
      Assert.AreEqual(c[1, 1], a[1] * b[1]);
      Assert.AreEqual(c[1, 2], a[1] * b[2]);
      Assert.AreEqual(c[1, 3], a[1] * b[3]);
      Assert.AreEqual(c[2, 0], a[2] * b[0]);
      Assert.AreEqual(c[2, 1], a[2] * b[1]);
      Assert.AreEqual(c[2, 2], a[2] * b[2]);
      Assert.AreEqual(c[2, 3], a[2] * b[3]);
      Assert.AreEqual(c[3, 0], a[3] * b[0]);
      Assert.AreEqual(c[3, 1], a[3] * b[1]);
      Assert.AreEqual(c[3, 2], a[3] * b[2]);
      Assert.AreEqual(c[3, 3], a[3] * b[3]);

      Assert.AreEqual(d[0, 0], a[0] * b[0]);
      Assert.AreEqual(d[0, 1], a[0] * b[1]);
      Assert.AreEqual(d[0, 2], a[0] * b[2]);
      Assert.AreEqual(d[0, 3], a[0] * b[3]);
      Assert.AreEqual(d[1, 0], a[1] * b[0]);
      Assert.AreEqual(d[1, 1], a[1] * b[1]);
      Assert.AreEqual(d[1, 2], a[1] * b[2]);
      Assert.AreEqual(d[1, 3], a[1] * b[3]);
      Assert.AreEqual(d[2, 0], a[2] * b[0]);
      Assert.AreEqual(d[2, 1], a[2] * b[1]);
      Assert.AreEqual(d[2, 2], a[2] * b[2]);
      Assert.AreEqual(d[2, 3], a[2] * b[3]);
      Assert.AreEqual(d[3, 0], a[3] * b[0]);
      Assert.AreEqual(d[3, 1], a[3] * b[1]);
      Assert.AreEqual(d[3, 2], a[3] * b[2]);
      Assert.AreEqual(d[3, 3], a[3] * b[3]);
    }

    //Test Divide
    [Test]
    public void Divide()
    {
      var a = new ComplexFloatVector(new float[4] { 0, 1, 2, 3 });
      var c = new ComplexFloatVector(a);
      var d = new ComplexFloatVector(a);
      float scal = -4;

      c = a / scal;
      d = ComplexFloatVector.Divide(a, scal);

      Assert.AreEqual(c[0], a[0] / scal);
      Assert.AreEqual(c[1], a[1] / scal);
      Assert.AreEqual(c[2], a[2] / scal);
      Assert.AreEqual(c[3], a[3] / scal);

      Assert.AreEqual(d[0], a[0] / scal);
      Assert.AreEqual(d[1], a[1] / scal);
      Assert.AreEqual(d[2], a[2] / scal);
      Assert.AreEqual(d[3], a[3] / scal);
    }

    //Test Clone
    [Test]
    public void Clone()
    {
      var a = new ComplexFloatVector(new float[4] { 0, 1, 2, 3 });
      ComplexFloatVector b = a.Clone();

      Assert.AreEqual(a[0], b[0]);
      Assert.AreEqual(a[1], b[1]);
      Assert.AreEqual(a[2], b[2]);
      Assert.AreEqual(a[3], b[3]);

      a = a * 2;

      Assert.AreEqual(a[0], b[0] * 2);
      Assert.AreEqual(a[1], b[1] * 2);
      Assert.AreEqual(a[2], b[2] * 2);
      Assert.AreEqual(a[3], b[3] * 2);
    }

    //Test IEnumerable and DoubleVectorEnumerator
    [Test]
    public void GetEnumeratorException()
    {
      Assert.Throws(typeof(InvalidOperationException), () =>
      {
        var a = new ComplexFloatVector(new ComplexFloat[4] { 0, 1, 2, 3 });
        IEnumerator dve = a.GetEnumerator();

        var b = (ComplexFloat)dve.Current;
      });
    }

    //Test IEnumerable and DoubleVectorEnumerator
    [Test]
    public void GetEnumerator()
    {
      var a = new ComplexFloatVector(new ComplexFloat[4] { 0, 1, 2, 3 });
      IEnumerator dve = a.GetEnumerator();
      ComplexFloat b;
      bool c;

      c = dve.MoveNext();
      b = (ComplexFloat)dve.Current;
      Assert.AreEqual(c, true);
      Assert.AreEqual(b, (ComplexFloat)0);

      c = dve.MoveNext();
      b = (ComplexFloat)dve.Current;
      Assert.AreEqual(c, true);
      Assert.AreEqual(b, (ComplexFloat)1);

      c = dve.MoveNext();
      b = (ComplexFloat)dve.Current;
      Assert.AreEqual(c, true);
      Assert.AreEqual(b, (ComplexFloat)2);

      c = dve.MoveNext();
      b = (ComplexFloat)dve.Current;
      Assert.AreEqual(c, true);
      Assert.AreEqual(b, (ComplexFloat)3);

      c = dve.MoveNext();
      Assert.AreEqual(c, false);
    }

    //Partial ICollection tests
    [Test]
    public void ICollection()
    {
      var a = new ComplexFloatVector(new ComplexFloat[4] { 0, 1, 2, 3 });
      var b = new ComplexFloat[5];

      Assert.AreEqual(a.Count, a.Length);

      a.CopyTo(b, 1);
      Assert.AreEqual(b[0], (ComplexFloat)0);
      Assert.AreEqual(b[1], (ComplexFloat)0);
      Assert.AreEqual(b[2], (ComplexFloat)1);
      Assert.AreEqual(b[3], (ComplexFloat)2);
      Assert.AreEqual(b[4], (ComplexFloat)3);
    }

    // IList tests
    [Test]
    public void IList()
    {
      var a = new ComplexFloatVector(new ComplexFloat[4] { 0, 1, 2, 3 });

      Assert.AreEqual(a.IsFixedSize, false);
      Assert.AreEqual(a.IsReadOnly, false);

      a.Add((ComplexFloat)4.0);
      Assert.AreEqual(a.Length, 5);
      Assert.AreEqual(a[4], (ComplexFloat)4);
      Assert.AreEqual(a.Contains((ComplexFloat)4.0), true);

      a.Insert(1, (ComplexFloat)5.0);
      Assert.AreEqual(a.Length, 6);
      Assert.AreEqual(a.Contains((ComplexFloat)5.0), true);
      Assert.AreEqual(a[0], (ComplexFloat)0);
      Assert.AreEqual(a[1], (ComplexFloat)5);
      Assert.AreEqual(a[2], (ComplexFloat)1);
      Assert.AreEqual(a[3], (ComplexFloat)2);
      Assert.AreEqual(a[4], (ComplexFloat)3);
      Assert.AreEqual(a[5], (ComplexFloat)4);

      a.Remove((ComplexFloat)5.0);
      Assert.AreEqual(a.Length, 5);
      Assert.AreEqual(a.Contains((ComplexFloat)5.0), false);
      Assert.AreEqual(a[0], (ComplexFloat)0);
      Assert.AreEqual(a[1], (ComplexFloat)1);
      Assert.AreEqual(a[2], (ComplexFloat)2);
      Assert.AreEqual(a[3], (ComplexFloat)3);
      Assert.AreEqual(a[4], (ComplexFloat)4);

      a.RemoveAt(2);
      Assert.AreEqual(a.Length, 4);
      Assert.AreEqual(a.Contains((ComplexFloat)2.0), false);
      Assert.AreEqual(a[0], (ComplexFloat)0);
      Assert.AreEqual(a[1], (ComplexFloat)1);
      Assert.AreEqual(a[2], (ComplexFloat)3);
      Assert.AreEqual(a[3], (ComplexFloat)4);
    }
  }
}
