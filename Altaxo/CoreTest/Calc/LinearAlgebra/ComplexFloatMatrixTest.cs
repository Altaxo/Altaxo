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
using NUnit.Framework;

namespace AltaxoTest.Calc.LinearAlgebra
{
  [TestFixture]
  public class ComplexFloatMatrixTest
  {
    private const double TOLERENCE = 0.001;

    //Test dimensions Constructor.
    [Test]
    public void CtorDimensions()
    {
      var test = new ComplexFloatMatrix(2, 2);

      Assert.AreEqual(test.RowLength, 2);
      Assert.AreEqual(test.ColumnLength, 2);
      Assert.AreEqual(test[0, 0], ComplexFloat.Zero);
      Assert.AreEqual(test[0, 1], ComplexFloat.Zero);
      Assert.AreEqual(test[1, 0], ComplexFloat.Zero);
      Assert.AreEqual(test[1, 1], ComplexFloat.Zero);
    }

    //Test Intital Values Constructor.
    [Test]
    public void CtorInitialValues()
    {
      var test = new ComplexFloatMatrix(2, 2, new ComplexFloat(1, 1));

      Assert.AreEqual(test.RowLength, 2);
      Assert.AreEqual(test.ColumnLength, 2);
      var value = new ComplexFloat(1, 1);
      Assert.AreEqual(test[0, 0], value);
      Assert.AreEqual(test[0, 1], value);
      Assert.AreEqual(test[1, 0], value);
      Assert.AreEqual(test[1, 1], value);
    }

    //Test Copy Constructor.
    [Test]
    public void CtorCopy()
    {
      var a = new ComplexFloatMatrix(2, 2)
      {
        [0, 0] = new ComplexFloat(1, 1),
        [0, 1] = new ComplexFloat(2, 2),
        [1, 0] = new ComplexFloat(3, 3),
        [1, 1] = new ComplexFloat(4, 4)
      };

      var b = new ComplexFloatMatrix(a);

      Assert.AreEqual(a.RowLength, b.RowLength);
      Assert.AreEqual(a.ColumnLength, b.ColumnLength);
      Assert.AreEqual(a[0, 0], b[0, 0]);
      Assert.AreEqual(a[0, 1], b[0, 1]);
      Assert.AreEqual(a[1, 0], b[1, 0]);
      Assert.AreEqual(a[1, 1], b[1, 1]);
    }

    //Test Copy Constructor.
    [Test]
    public void CtorCopyNull()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        ComplexFloatMatrix a = null;
        var b = new ComplexFloatMatrix(a);
      });
    }

    //Test Copy Constructor.
    [Test]
    public void CtorCopyComplexFloat()
    {
      var a = new ComplexFloatMatrix(2, 2)
      {
        [0, 0] = new ComplexFloat(1, 1),
        [0, 1] = new ComplexFloat(2, 2),
        [1, 0] = new ComplexFloat(3, 3),
        [1, 1] = new ComplexFloat(4, 4)
      };

      var b = new ComplexFloatMatrix(a);
      Assert.AreEqual(a.RowLength, b.RowLength);
      Assert.AreEqual(a.ColumnLength, b.ColumnLength);
      Assert.AreEqual(a[0, 0].Real, b[0, 0].Real);
      Assert.AreEqual(a[0, 1].Real, b[0, 1].Real);
      Assert.AreEqual(a[1, 0].Real, b[1, 0].Real);
      Assert.AreEqual(a[1, 1].Real, b[1, 1].Real);
      Assert.AreEqual(a[0, 0].Imag, b[0, 0].Imag);
      Assert.AreEqual(a[0, 1].Imag, b[0, 1].Imag);
      Assert.AreEqual(a[1, 0].Imag, b[1, 0].Imag);
      Assert.AreEqual(a[1, 1].Imag, b[1, 1].Imag);
    }

    //Test Copy Constructor.
    [Test]
    public void CtorCopyComplexFloatNull()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        FloatMatrix a = null;
        var b = new ComplexFloatMatrix(a);
      });
    }

    //Test Copy Constructor.
    [Test]
    public void CtorCopyFloat()
    {
      var a = new FloatMatrix(2, 2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };

      var b = new ComplexFloatMatrix(a);
      Assert.AreEqual(a.RowLength, b.RowLength);
      Assert.AreEqual(a.ColumnLength, b.ColumnLength);
      Assert.AreEqual(a[0, 0], b[0, 0].Real);
      Assert.AreEqual(a[0, 1], b[0, 1].Real);
      Assert.AreEqual(a[1, 0], b[1, 0].Real);
      Assert.AreEqual(a[1, 1], b[1, 1].Real);
      Assert.AreEqual(0, b[0, 0].Imag);
      Assert.AreEqual(0, b[0, 1].Imag);
      Assert.AreEqual(0, b[1, 0].Imag);
      Assert.AreEqual(0, b[1, 1].Imag);
    }

    //Test Copy Constructor.
    [Test]
    public void CtorCopyFloatNull()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        FloatMatrix a = null;
        var b = new ComplexFloatMatrix(a);
      });
    }

    //Test Copy Constructor.
    [Test]
    public void CtorCopyDouble()
    {
      var a = new FloatMatrix(2, 2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };

      var b = new ComplexFloatMatrix(a);
      Assert.AreEqual(a.RowLength, b.RowLength);
      Assert.AreEqual(a.ColumnLength, b.ColumnLength);
      Assert.AreEqual(a[0, 0], b[0, 0].Real);
      Assert.AreEqual(a[0, 1], b[0, 1].Real);
      Assert.AreEqual(a[1, 0], b[1, 0].Real);
      Assert.AreEqual(a[1, 1], b[1, 1].Real);
      Assert.AreEqual(0, b[0, 0].Imag);
      Assert.AreEqual(0, b[0, 1].Imag);
      Assert.AreEqual(0, b[1, 0].Imag);
      Assert.AreEqual(0, b[1, 1].Imag);
    }

    //Test Copy Constructor.
    [Test]
    public void CtorCopyDoubleNull()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        FloatMatrix a = null;
        var b = new ComplexFloatMatrix(a);
      });
    }

    //Test Multiple Dimensional ComplexFloatArray Constructor with Square array.
    [Test]
    public void CtorMultDimComplexFloatSquare()
    {
      var values = new ComplexFloat[2, 2];

      values[0, 0] = new ComplexFloat(1, 1);
      values[0, 1] = new ComplexFloat(2, 2);
      values[1, 0] = new ComplexFloat(3, 3);
      values[1, 1] = new ComplexFloat(4, 4);

      var test = new ComplexFloatMatrix(values);

      Assert.AreEqual(test.RowLength, 2);
      Assert.AreEqual(test.ColumnLength, 2);
      Assert.AreEqual(test[0, 0], values[0, 0]);
      Assert.AreEqual(test[0, 1], values[0, 1]);
      Assert.AreEqual(test[1, 0], values[1, 0]);
      Assert.AreEqual(test[1, 1], values[1, 1]);
    }

    //Test Multiple Dimensional ComplexFloatArray Constructor with wide array.
    [Test]
    public void CtorMultDimComplexFloatWide()
    {
      var values = new ComplexFloat[2, 3];

      values[0, 0] = new ComplexFloat(0, 0);
      values[0, 1] = new ComplexFloat(1, 1);
      values[0, 2] = new ComplexFloat(2, 2);
      values[1, 0] = new ComplexFloat(3, 3);
      values[1, 1] = new ComplexFloat(4, 4);
      values[1, 2] = new ComplexFloat(5, 5);

      var test = new ComplexFloatMatrix(values);

      Assert.AreEqual(test.RowLength, 2);
      Assert.AreEqual(test.ColumnLength, 3);
      Assert.AreEqual(test[0, 0], values[0, 0]);
      Assert.AreEqual(test[0, 1], values[0, 1]);
      Assert.AreEqual(test[0, 2], values[0, 2]);
      Assert.AreEqual(test[1, 0], values[1, 0]);
      Assert.AreEqual(test[1, 1], values[1, 1]);
      Assert.AreEqual(test[1, 2], values[1, 2]);
    }

    //Test Multiple Dimensional ComplexFloatArray Constructor with long array.
    [Test]
    public void CtorMultDimComplexFloatLong()
    {
      var values = new ComplexFloat[3, 2];

      values[0, 0] = new ComplexFloat(0, 0);
      values[0, 1] = new ComplexFloat(1, 1);
      values[1, 0] = new ComplexFloat(3, 3);
      values[1, 1] = new ComplexFloat(4, 4);
      values[2, 0] = new ComplexFloat(5, 5);
      values[2, 1] = new ComplexFloat(6, 6);

      var test = new ComplexFloatMatrix(values);

      Assert.AreEqual(test.RowLength, 3);
      Assert.AreEqual(test.ColumnLength, 2);
      Assert.AreEqual(test[0, 0], values[0, 0]);
      Assert.AreEqual(test[0, 1], values[0, 1]);
      Assert.AreEqual(test[1, 0], values[1, 0]);
      Assert.AreEqual(test[1, 1], values[1, 1]);
      Assert.AreEqual(test[2, 0], values[2, 0]);
      Assert.AreEqual(test[2, 1], values[2, 1]);
    }

    //Test Multiple Dimensional ComplexFloat Array Constructor with null.
    [Test]
    public void CtorMultDimComplexFloatNull()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        float[,] values = null;
        var test = new ComplexFloatMatrix(values);
      });
    }

    //Test Multiple Dimensional DoubleArray Constructor with Square array.
    [Test]
    public void CtorMultDimDoubleSquare()
    {
      float[,] values = new float[2, 2];

      values[0, 0] = 1;
      values[0, 1] = 2;
      values[1, 0] = 3;
      values[1, 1] = 4;

      var test = new ComplexFloatMatrix(values);

      Assert.AreEqual(test.RowLength, 2);
      Assert.AreEqual(test.ColumnLength, 2);
      Assert.AreEqual(values[0, 0], test[0, 0].Real);
      Assert.AreEqual(values[0, 1], test[0, 1].Real);
      Assert.AreEqual(values[1, 0], test[1, 0].Real);
      Assert.AreEqual(values[1, 1], test[1, 1].Real);
    }

    //Test Multiple Dimensional DoubleArray Constructor with wide array.
    [Test]
    public void CtorMultDimDoubleWide()
    {
      float[,] values = new float[2, 3];

      values[0, 0] = 0;
      values[0, 1] = 1;
      values[0, 2] = 2;
      values[1, 0] = 3;
      values[1, 1] = 4;
      values[1, 2] = 5;

      var test = new ComplexFloatMatrix(values);

      Assert.AreEqual(test.RowLength, 2);
      Assert.AreEqual(test.ColumnLength, 3);
      Assert.AreEqual(test[0, 0].Real, values[0, 0]);
      Assert.AreEqual(test[0, 1].Real, values[0, 1]);
      Assert.AreEqual(test[0, 2].Real, values[0, 2]);
      Assert.AreEqual(test[1, 0].Real, values[1, 0]);
      Assert.AreEqual(test[1, 1].Real, values[1, 1]);
      Assert.AreEqual(test[1, 2].Real, values[1, 2]);
    }

    //Test Multiple Dimensional DoubleArray Constructor with long array.
    [Test]
    public void CtorMultDimDoubleLong()
    {
      float[,] values = new float[3, 2];

      values[0, 0] = 0;
      values[0, 1] = 1;
      values[1, 0] = 2;
      values[1, 1] = 3;
      values[2, 0] = 4;
      values[2, 1] = 5;

      var test = new ComplexFloatMatrix(values);

      Assert.AreEqual(test.RowLength, 3);
      Assert.AreEqual(test.ColumnLength, 2);
      Assert.AreEqual(test[0, 0].Real, values[0, 0]);
      Assert.AreEqual(test[0, 1].Real, values[0, 1]);
      Assert.AreEqual(test[1, 0].Real, values[1, 0]);
      Assert.AreEqual(test[1, 1].Real, values[1, 1]);
      Assert.AreEqual(test[2, 0].Real, values[2, 0]);
      Assert.AreEqual(test[2, 1].Real, values[2, 1]);
    }

    //Test Multiple Dimensional Double Array Constructor with null.
    [Test]
    public void CtorMultDimDoubleNull()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        float[,] values = null;
        var test = new ComplexFloatMatrix(values);
      });
    }

    //Test Multiple Dimensional Float Array Constructor with Square array.
    [Test]
    public void CtorMultDimFloatSquare()
    {
      float[,] values = new float[2, 2];

      values[0, 0] = 0;
      values[0, 1] = 1;
      values[1, 0] = 2;
      values[1, 1] = 3;

      var test = new ComplexFloatMatrix(values);

      Assert.AreEqual(test.RowLength, 2);
      Assert.AreEqual(test.ColumnLength, 2);
      Assert.AreEqual(test[0, 0].Real, values[0, 0]);
      Assert.AreEqual(test[0, 1].Real, values[0, 1]);
      Assert.AreEqual(test[1, 0].Real, values[1, 0]);
      Assert.AreEqual(test[1, 1].Real, values[1, 1]);
    }

    //Test Multiple Dimensional Float Array Constructor with wide array.
    [Test]
    public void CtorMultDimFloatWide()
    {
      float[,] values = new float[2, 3];

      values[0, 0] = 0;
      values[0, 1] = 1;
      values[0, 2] = 2;
      values[1, 0] = 3;
      values[1, 1] = 4;
      values[1, 2] = 5;

      var test = new ComplexFloatMatrix(values);

      Assert.AreEqual(test.RowLength, 2);
      Assert.AreEqual(test.ColumnLength, 3);
      Assert.AreEqual(test[0, 0].Real, values[0, 0]);
      Assert.AreEqual(test[0, 1].Real, values[0, 1]);
      Assert.AreEqual(test[0, 2].Real, values[0, 2]);
      Assert.AreEqual(test[1, 0].Real, values[1, 0]);
      Assert.AreEqual(test[1, 1].Real, values[1, 1]);
      Assert.AreEqual(test[1, 2].Real, values[1, 2]);
    }

    //Test Multiple Dimensional FloatArray Constructor with long array.
    [Test]
    public void CtorMultDimFloatLong()
    {
      float[,] values = new float[3, 2];

      values[0, 0] = 0;
      values[0, 1] = 1;
      values[1, 0] = 2;
      values[1, 1] = 3;
      values[2, 0] = 4;
      values[2, 1] = 5;

      var test = new ComplexFloatMatrix(values);

      Assert.AreEqual(test.RowLength, 3);
      Assert.AreEqual(test.ColumnLength, 2);
      Assert.AreEqual(test[0, 0].Real, values[0, 0]);
      Assert.AreEqual(test[0, 1].Real, values[0, 1]);
      Assert.AreEqual(test[1, 0].Real, values[1, 0]);
      Assert.AreEqual(test[1, 1].Real, values[1, 1]);
      Assert.AreEqual(test[2, 0].Real, values[2, 0]);
      Assert.AreEqual(test[2, 1].Real, values[2, 1]);
    }

    //Test Multiple Dimensional Float Array Constructor with null.
    [Test]
    public void CtorMultDimFloatNull()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        float[,] values = null;
        var test = new ComplexFloatMatrix(values);
      });
    }

    //Test implicit conversion from ComplexFloatMatrix.
    [Test]
    public void ImplictComplexFloatMatrix()
    {
      var a = new ComplexFloatMatrix(2, 2)
      {
        [0, 0] = new ComplexFloat(1, 1),
        [0, 1] = new ComplexFloat(2, 2),
        [1, 0] = new ComplexFloat(3, 3),
        [1, 1] = new ComplexFloat(4, 4)
      };

      ComplexFloatMatrix b = a;
      Assert.AreEqual(a.RowLength, b.RowLength);
      Assert.AreEqual(a.ColumnLength, b.ColumnLength);
      Assert.AreEqual(a[0, 0], b[0, 0]);
      Assert.AreEqual(a[0, 1], b[0, 1]);
      Assert.AreEqual(a[1, 0], b[1, 0]);
      Assert.AreEqual(a[1, 1], b[1, 1]);
    }

    //Test implicit conversion from null Complexfloatmatrix.
    [Test]
    public void ImplictComplexFloatMatrixNull()
    {
      ComplexFloatMatrix a = null;
      ComplexFloatMatrix b = a;
      Assert.IsTrue(b == null);
    }

    //Test implicit conversion from Complexfloatmatrix.
    [Test]
    public void ImplictToComplexFloatMatrix()
    {
      var a = new ComplexFloatMatrix(2, 2)
      {
        [0, 0] = new ComplexFloat(1, 1),
        [0, 1] = new ComplexFloat(2, 2),
        [1, 0] = new ComplexFloat(3, 3),
        [1, 1] = new ComplexFloat(4, 4)
      };

      var b = ComplexFloatMatrix.ToComplexFloatMatrix(a);
      Assert.AreEqual(a.RowLength, b.RowLength);
      Assert.AreEqual(a.ColumnLength, b.ColumnLength);
      Assert.AreEqual(a[0, 0], b[0, 0]);
      Assert.AreEqual(a[0, 1], b[0, 1]);
      Assert.AreEqual(a[1, 0], b[1, 0]);
      Assert.AreEqual(a[1, 1], b[1, 1]);
    }

    //Test implicit conversion from null ComplexFoatmatrix.
    [Test]
    public void ImplictToComplexFloatMatrixNull()
    {
      ComplexFloatMatrix a = null;
      var b = ComplexFloatMatrix.ToComplexFloatMatrix(a);
      Assert.IsTrue(b == null);
    }

    //Test implicit conversion from Doublematrix.
    [Test]
    public void ImplictDoubleMatrix()
    {
      var a = new FloatMatrix(2, 2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };

      ComplexFloatMatrix b = a;
      Assert.AreEqual(a.RowLength, b.RowLength);
      Assert.AreEqual(a.ColumnLength, b.ColumnLength);
      Assert.AreEqual(a[0, 0], b[0, 0].Real);
      Assert.AreEqual(a[0, 1], b[0, 1].Real);
      Assert.AreEqual(a[1, 0], b[1, 0].Real);
      Assert.AreEqual(a[1, 1], b[1, 1].Real);
    }

    //Test implicit conversion from null Doublematrix.
    [Test]
    public void ImplictDoubleMatrixNull()
    {
      FloatMatrix a = null;
      ComplexFloatMatrix b = a;
      Assert.IsTrue(b == null);
    }

    //Test implicit conversion from floatmatrix.
    [Test]
    public void ImplictToDoubleMatrix()
    {
      var a = new FloatMatrix(2, 2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };

      var b = ComplexFloatMatrix.ToComplexFloatMatrix(a);
      Assert.AreEqual(a.RowLength, b.RowLength);
      Assert.AreEqual(a.ColumnLength, b.ColumnLength);
      Assert.AreEqual(a[0, 0], b[0, 0].Real);
      Assert.AreEqual(a[0, 1], b[0, 1].Real);
      Assert.AreEqual(a[1, 0], b[1, 0].Real);
      Assert.AreEqual(a[1, 1], b[1, 1].Real);
    }

    //Test implicit conversion from null floatmatrix.
    [Test]
    public void ImplictToDoubleMatrixMatrixNull()
    {
      FloatMatrix a = null;
      var b = ComplexFloatMatrix.ToComplexFloatMatrix(a);
      Assert.IsTrue(b == null);
    }

    //Test implicit conversion from floatmatrix.
    [Test]
    public void ImplictFloatMatrix()
    {
      var a = new FloatMatrix(2, 2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };

      ComplexFloatMatrix b = a;
      Assert.AreEqual(a.RowLength, b.RowLength);
      Assert.AreEqual(a.ColumnLength, b.ColumnLength);
      Assert.AreEqual(a[0, 0], b[0, 0].Real);
      Assert.AreEqual(a[0, 1], b[0, 1].Real);
      Assert.AreEqual(a[1, 0], b[1, 0].Real);
      Assert.AreEqual(a[1, 1], b[1, 1].Real);
    }

    //Test implicit conversion from null floatmatrix.
    [Test]
    public void ImplictFloatMatrixNull()
    {
      FloatMatrix a = null;
      ComplexFloatMatrix b = a;
      Assert.IsTrue(b == null);
    }

    //Test implicit conversion from floatmatrix.
    [Test]
    public void ImplictToFloatMatrix()
    {
      var a = new FloatMatrix(2, 2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };

      var b = ComplexFloatMatrix.ToComplexFloatMatrix(a);
      Assert.AreEqual(a.RowLength, b.RowLength);
      Assert.AreEqual(a.ColumnLength, b.ColumnLength);
      Assert.AreEqual(a[0, 0], b[0, 0].Real);
      Assert.AreEqual(a[0, 1], b[0, 1].Real);
      Assert.AreEqual(a[1, 0], b[1, 0].Real);
      Assert.AreEqual(a[1, 1], b[1, 1].Real);
    }

    //Test implicit conversion from null floatmatrix.
    [Test]
    public void ImplictToFloatMatrixNull()
    {
      FloatMatrix a = null;
      var b = ComplexFloatMatrix.ToComplexFloatMatrix(a);
      Assert.IsTrue(b == null);
    }

    //Test implicit conversion from ComplexFloat mult dim array.
    [Test]
    public void ImplictComplexFloatMultArray()
    {
      var a = new ComplexFloat[2, 2];
      a[0, 0] = new ComplexFloat(1, 1);
      a[0, 1] = new ComplexFloat(2, 2);
      a[1, 0] = new ComplexFloat(3, 3);
      a[1, 1] = new ComplexFloat(4, 4);

      ComplexFloatMatrix b = a;
      Assert.AreEqual(a[0, 0], b[0, 0]);
      Assert.AreEqual(a[0, 1], b[0, 1]);
      Assert.AreEqual(a[1, 0], b[1, 0]);
      Assert.AreEqual(a[1, 1], b[1, 1]);
    }

    //Test implicit conversion from ComplexFloat mult dim array.
    [Test]
    public void ImplictToComplexFloatMultArray()
    {
      var a = new ComplexFloat[2, 2];
      a[0, 0] = new ComplexFloat(1, 1);
      a[0, 1] = new ComplexFloat(2, 2);
      a[1, 0] = new ComplexFloat(3, 3);
      a[1, 1] = new ComplexFloat(4, 4);

      var b = ComplexFloatMatrix.ToComplexFloatMatrix(a);
      Assert.AreEqual(a[0, 0], b[0, 0]);
      Assert.AreEqual(a[0, 1], b[0, 1]);
      Assert.AreEqual(a[1, 0], b[1, 0]);
      Assert.AreEqual(a[1, 1], b[1, 1]);
    }

    //Test implicit conversion from null ComplexFloat mult dim array.
    [Test]
    public void ImplictToComplexFloatMultArrayNull()
    {
      ComplexFloat[,] a = null;
      var b = ComplexFloatMatrix.ToComplexFloatMatrix(a);
      Assert.IsTrue(b == null);
    }

    //Test implicit conversion from float mult dim array.
    [Test]
    public void ImplictToDoubleMultArray()
    {
      float[,] a = new float[2, 2];
      a[0, 0] = 1;
      a[0, 1] = 2;
      a[1, 0] = 3;
      a[1, 1] = 4;

      var b = ComplexFloatMatrix.ToComplexFloatMatrix(a);
      Assert.AreEqual(a[0, 0], b[0, 0].Real);
      Assert.AreEqual(a[0, 1], b[0, 1].Real);
      Assert.AreEqual(a[1, 0], b[1, 0].Real);
      Assert.AreEqual(a[1, 1], b[1, 1].Real);
    }

    //Test implicit conversion from null float mult dim array.
    [Test]
    public void ImplictToDoubleMultArrayNull()
    {
      float[,] a = null;
      var b = ComplexFloatMatrix.ToComplexFloatMatrix(a);
      Assert.IsTrue(b == null);
    }

    //Test implicit conversion from float mult dim array.
    [Test]
    public void ImplictToFloatMultArray()
    {
      float[,] a = new float[2, 2];
      a[0, 0] = 1;
      a[0, 1] = 2;
      a[1, 0] = 3;
      a[1, 1] = 4;

      var b = ComplexFloatMatrix.ToComplexFloatMatrix(a);
      Assert.AreEqual(a[0, 0], b[0, 0].Real);
      Assert.AreEqual(a[0, 1], b[0, 1].Real);
      Assert.AreEqual(a[1, 0], b[1, 0].Real);
      Assert.AreEqual(a[1, 1], b[1, 1].Real);
    }

    //Test implicit conversion from null float mult dim array.
    [Test]
    public void ImplictToFloatMultArrayNull()
    {
      float[,] a = null;
      var b = ComplexFloatMatrix.ToComplexFloatMatrix(a);
      Assert.IsTrue(b == null);
    }

    //Test explicit conversion from ComplexDoubleMatrix
    [Test]
    public void ExplicitComplexDoubleMatrix()
    {
      var a = new ComplexDoubleMatrix(2, 2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };

      var b = (ComplexFloatMatrix)a;
      Assert.AreEqual(a.RowLength, b.RowLength);
      Assert.AreEqual(a.ColumnLength, b.ColumnLength);
      Assert.AreEqual(b[0, 0], a[0, 0]);
      Assert.AreEqual(b[0, 1], a[0, 1]);
      Assert.AreEqual(b[1, 0], a[1, 0]);
      Assert.AreEqual(b[1, 1], a[1, 1]);
    }

    //Test explicit conversion from ComplexDoubleMatrix
    [Test]
    public void ExplicitComplexDoubleMatrixNull()
    {
      ComplexDoubleMatrix a = null;
      var b = (ComplexFloatMatrix)a;
      Assert.IsTrue(b == null);
    }

    //Test explicit conversion from ComplexDoubleMatrix
    [Test]
    public void ExplicitToComplexDoubleMatrix()
    {
      var a = new ComplexDoubleMatrix(2, 2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };

      var b = ComplexFloatMatrix.ToComplexFloatMatrix(a);
      Assert.AreEqual(a.RowLength, b.RowLength);
      Assert.AreEqual(a.ColumnLength, b.ColumnLength);
      Assert.AreEqual(b[0, 0], a[0, 0]);
      Assert.AreEqual(b[0, 1], a[0, 1]);
      Assert.AreEqual(b[1, 0], a[1, 0]);
      Assert.AreEqual(b[1, 1], a[1, 1]);
    }

    //Test explicit conversion from DoubleMatrix
    [Test]
    public void ExplicitToComplexDoubleMatrixNull()
    {
      DoubleMatrix a = null;
      var b = ComplexFloatMatrix.ToComplexFloatMatrix(a);
      Assert.IsTrue(b == null);
    }

    //Test explicit conversion from Complex array
    [Test]
    public void ExplicitComplexDoubleMultArray()
    {
      var a = new Complex[2, 2];
      a[0, 0] = 1;
      a[0, 1] = 2;
      a[1, 0] = 3;
      a[1, 1] = 4;

      var b = (ComplexFloatMatrix)a;
      Assert.AreEqual(2, b.RowLength);
      Assert.AreEqual(2, b.ColumnLength);
      Assert.AreEqual(b[0, 0], a[0, 0]);
      Assert.AreEqual(b[0, 1], a[0, 1]);
      Assert.AreEqual(b[1, 0], a[1, 0]);
      Assert.AreEqual(b[1, 1], a[1, 1]);
    }

    //Test explicit conversion from Complex array
    [Test]
    public void ExplicitComplexDoubleMultArrayNull()
    {
      Complex[,] a = null;
      var b = (ComplexFloatMatrix)a;
      Assert.IsTrue(b == null);
    }

    //Test explicit conversion from Complex array
    [Test]
    public void ExplicitToComplexDoubleMultArray()
    {
      var a = new Complex[2, 2];
      a[0, 0] = 1;
      a[0, 1] = 2;
      a[1, 0] = 3;
      a[1, 1] = 4;

      var b = ComplexFloatMatrix.ToComplexFloatMatrix(a);
      Assert.AreEqual(2, b.RowLength);
      Assert.AreEqual(2, b.ColumnLength);
      Assert.AreEqual(a[0, 0], b[0, 0]);
      Assert.AreEqual(a[0, 1], b[0, 1]);
      Assert.AreEqual(a[1, 0], b[1, 0]);
      Assert.AreEqual(a[1, 1], b[1, 1]);
    }

    //Test explicit conversion from Complex array
    [Test]
    public void ExplicitToComplexDoubleMultArrayNull()
    {
      Complex[,] a = null;
      var b = ComplexFloatMatrix.ToComplexFloatMatrix(a);
      Assert.IsTrue(b == null);
    }

    //Test explicit conversion from Jagged Complex array
    [Test]
    public void ExplicitComplexDoubleJaggedArrayNull()
    {
      Complex[,] a = null;
      var b = (ComplexFloatMatrix)a;
      Assert.IsTrue(b == null);
    }

    //test equals method
    [Test]
    public void Equals()
    {
      var a = new ComplexFloatMatrix(2, 2, new ComplexFloat(4, 4));
      var b = new ComplexFloatMatrix(2, 2, new ComplexFloat(4, 4));
      var c = new ComplexFloatMatrix(2, 2)
      {
        [0, 0] = new ComplexFloat(4, 4),
        [0, 1] = new ComplexFloat(4, 4),
        [1, 0] = new ComplexFloat(4, 4),
        [1, 1] = new ComplexFloat(4, 4)
      };

      var d = new ComplexFloatMatrix(2, 2, 5);
      ComplexFloatMatrix e = null;
      var f = new FloatMatrix(2, 2, 4);
      Assert.IsTrue(a.Equals(b));
      Assert.IsTrue(b.Equals(a));
      Assert.IsTrue(a.Equals(c));
      Assert.IsTrue(b.Equals(c));
      Assert.IsTrue(c.Equals(b));
      Assert.IsTrue(c.Equals(a));
      Assert.IsFalse(a.Equals(d));
      Assert.IsFalse(d.Equals(b));
      Assert.IsFalse(a.Equals(e));
      Assert.IsFalse(a.Equals(f));
    }

    //test GetHashCode
    [Test]
    public void TestHashCode()
    {
      var a = new ComplexFloatMatrix(2)
      {
        [0, 0] = new ComplexFloat(1, 1),
        [0, 1] = new ComplexFloat(2, 2),
        [1, 0] = new ComplexFloat(3, 3),
        [1, 1] = new ComplexFloat(4, 4)
      };

      int hash = a.GetHashCode();
      Assert.AreEqual(hash, 7);
    }

    //test ToArray
    [Test]
    public void ToArray()
    {
      var a = new ComplexFloatMatrix(2)
      {
        [0, 0] = new ComplexFloat(1, 1),
        [0, 1] = new ComplexFloat(2, 2),
        [1, 0] = new ComplexFloat(3, 3),
        [1, 1] = new ComplexFloat(4, 4)
      };

      ComplexFloat[,] b = a.ToArray();

      Assert.AreEqual(a[0, 0], b[0, 0]);
      Assert.AreEqual(a[0, 1], b[0, 1]);
      Assert.AreEqual(a[1, 0], b[1, 0]);
      Assert.AreEqual(a[1, 1], b[1, 1]);
    }

    //test Transpose square
    [Test]
    public void TransposeSquare()
    {
      var a = new ComplexFloatMatrix(2, 2)
      {
        [0, 0] = new ComplexFloat(1),
        [0, 1] = new ComplexFloat(2),
        [1, 0] = new ComplexFloat(3),
        [1, 1] = new ComplexFloat(4)
      };
      a.Transpose();
      Assert.AreEqual(a[0, 0], new ComplexFloat(1));
      Assert.AreEqual(a[0, 1], new ComplexFloat(3));
      Assert.AreEqual(a[1, 0], new ComplexFloat(2));
      Assert.AreEqual(a[1, 1], new ComplexFloat(4));
    }

    //test Transpose wide
    [Test]
    public void TransposeWide()
    {
      var a = new ComplexFloatMatrix(2, 3)
      {
        [0, 0] = new ComplexFloat(1),
        [0, 1] = new ComplexFloat(2),
        [0, 2] = new ComplexFloat(3),
        [1, 0] = new ComplexFloat(4),
        [1, 1] = new ComplexFloat(5),
        [1, 2] = new ComplexFloat(6)
      };
      a.Transpose();
      Assert.AreEqual(a[0, 0], new ComplexFloat(1));
      Assert.AreEqual(a[0, 1], new ComplexFloat(4));
      Assert.AreEqual(a[1, 0], new ComplexFloat(2));
      Assert.AreEqual(a[1, 1], new ComplexFloat(5));
      Assert.AreEqual(a[2, 0], new ComplexFloat(3));
      Assert.AreEqual(a[2, 1], new ComplexFloat(6));
      Assert.AreEqual(a.RowLength, 3);
      Assert.AreEqual(a.ColumnLength, 2);
    }

    //test Transpose long
    [Test]
    public void TransposeLong()
    {
      var a = new ComplexFloatMatrix(3, 2)
      {
        [0, 0] = new ComplexFloat(1),
        [0, 1] = new ComplexFloat(2),
        [1, 0] = new ComplexFloat(3),
        [1, 1] = new ComplexFloat(4),
        [2, 0] = new ComplexFloat(5),
        [2, 1] = new ComplexFloat(6)
      };
      a.Transpose();
      Assert.AreEqual(a[0, 0], new ComplexFloat(1));
      Assert.AreEqual(a[0, 1], new ComplexFloat(3));
      Assert.AreEqual(a[0, 2], new ComplexFloat(5));
      Assert.AreEqual(a[1, 0], new ComplexFloat(2));
      Assert.AreEqual(a[1, 1], new ComplexFloat(4));
      Assert.AreEqual(a[1, 2], new ComplexFloat(6));
      Assert.AreEqual(a.RowLength, 2);
      Assert.AreEqual(a.ColumnLength, 3);
    }

    //test GetTranspose square
    [Test]
    public void GetTransposeSquare()
    {
      var a = new ComplexFloatMatrix(2, 2)
      {
        [0, 0] = new ComplexFloat(1),
        [0, 1] = new ComplexFloat(2),
        [1, 0] = new ComplexFloat(3),
        [1, 1] = new ComplexFloat(4)
      };
      ComplexFloatMatrix b = a.GetTranspose();
      Assert.AreEqual(b[0, 0], a[0, 0]);
      Assert.AreEqual(b[0, 1], a[1, 0]);
      Assert.AreEqual(b[1, 0], a[0, 1]);
      Assert.AreEqual(b[1, 1], a[1, 1]);
    }

    //test GetTranspose wide
    [Test]
    public void GetTransposeWide()
    {
      var a = new ComplexFloatMatrix(2, 3)
      {
        [0, 0] = new ComplexFloat(1),
        [0, 1] = new ComplexFloat(2),
        [0, 2] = new ComplexFloat(3),
        [1, 0] = new ComplexFloat(4),
        [1, 1] = new ComplexFloat(5),
        [1, 2] = new ComplexFloat(6)
      };
      ComplexFloatMatrix b = a.GetTranspose();
      Assert.AreEqual(b[0, 0], a[0, 0]);
      Assert.AreEqual(b[0, 1], a[1, 0]);
      Assert.AreEqual(b[1, 0], a[0, 1]);
      Assert.AreEqual(b[1, 1], a[1, 1]);
      Assert.AreEqual(b[2, 0], a[0, 2]);
      Assert.AreEqual(b[2, 1], a[1, 2]);
      Assert.AreEqual(b.RowLength, a.ColumnLength);
      Assert.AreEqual(b.ColumnLength, a.RowLength);
    }

    //test GetTranspose long
    [Test]
    public void GetTransposeLong()
    {
      var a = new ComplexFloatMatrix(3, 2)
      {
        [0, 0] = new ComplexFloat(1),
        [0, 1] = new ComplexFloat(2),
        [1, 0] = new ComplexFloat(3),
        [1, 1] = new ComplexFloat(4),
        [2, 0] = new ComplexFloat(5),
        [2, 1] = new ComplexFloat(6)
      };
      ComplexFloatMatrix b = a.GetTranspose();
      Assert.AreEqual(b[0, 0], a[0, 0]);
      Assert.AreEqual(b[0, 1], a[1, 0]);
      Assert.AreEqual(b[0, 2], a[2, 0]);
      Assert.AreEqual(b[1, 0], a[0, 1]);
      Assert.AreEqual(b[1, 1], a[1, 1]);
      Assert.AreEqual(b[1, 2], a[2, 1]);
      Assert.AreEqual(b.RowLength, a.ColumnLength);
      Assert.AreEqual(b.ColumnLength, a.RowLength);
    }

    //test Invert
    [Test]
    public void Invert()
    {
      var a = new ComplexFloatMatrix(2, 2)
      {
        [0, 0] = new ComplexFloat(2),
        [0, 1] = new ComplexFloat(4),
        [1, 0] = new ComplexFloat(3),
        [1, 1] = new ComplexFloat(7)
      };
      a.Invert();
      Assert.AreEqual(a[0, 0].Real, 3.500, TOLERENCE);
      Assert.AreEqual(a[0, 1].Real, -2.000, TOLERENCE);
      Assert.AreEqual(a[1, 0].Real, -1.500, TOLERENCE);
      Assert.AreEqual(a[1, 1].Real, 1.000, TOLERENCE);
    }

    //test Invert singular
    [Test]
    public void InvertSingular()
    {
      Assert.Throws(typeof(SingularMatrixException), () =>
      {
        var a = new ComplexFloatMatrix(2, 2);
        a.Invert();
      });
    }

    //test Invert not square
    [Test]
    public void InvertNotSquare()
    {
      Assert.Throws(typeof(NotSquareMatrixException), () =>
      {
        var a = new ComplexFloatMatrix(3, 2)
        {
          [0, 0] = new ComplexFloat(2),
          [0, 1] = new ComplexFloat(4),
          [1, 0] = new ComplexFloat(3),
          [1, 1] = new ComplexFloat(7),
          [2, 0] = new ComplexFloat(5),
          [2, 1] = new ComplexFloat(5)
        };
        a.Invert();
      });
    }

    //test GetInverse singular
    [Test]
    public void GetInverseSingular()
    {
      Assert.Throws(typeof(SingularMatrixException), () =>
      {
        var a = new ComplexFloatMatrix(2, 2);
        ComplexFloatMatrix b = a.GetInverse();
      });
    }

    //test GetInverse not square
    [Test]
    public void GetInverseNotSquare()
    {
      Assert.Throws(typeof(NotSquareMatrixException), () =>
      {
        var a = new ComplexFloatMatrix(3, 2)
        {
          [0, 0] = new ComplexFloat(2),
          [0, 1] = new ComplexFloat(4),
          [1, 0] = new ComplexFloat(3),
          [1, 1] = new ComplexFloat(7),
          [2, 0] = new ComplexFloat(5),
          [2, 1] = new ComplexFloat(5)
        };
        ComplexFloatMatrix b = a.GetInverse();
      });
    }

    //test GetInverse
    [Test]
    public void GetInverse()
    {
      var a = new ComplexFloatMatrix(2, 2)
      {
        [0, 0] = new ComplexFloat(2),
        [0, 1] = new ComplexFloat(4),
        [1, 0] = new ComplexFloat(3),
        [1, 1] = new ComplexFloat(7)
      };
      ComplexFloatMatrix b = a.GetInverse();
      Assert.AreEqual(b[0, 0].Real, 3.500, TOLERENCE);
      Assert.AreEqual(b[0, 1].Real, -2.000, TOLERENCE);
      Assert.AreEqual(b[1, 0].Real, -1.500, TOLERENCE);
      Assert.AreEqual(b[1, 1].Real, 1.000, TOLERENCE);
    }

    //test GetDeterminant
    [Test]
    public void GetDeterminant()
    {
      var a = new ComplexFloatMatrix(2, 2)
      {
        [0, 0] = new ComplexFloat(2),
        [0, 1] = new ComplexFloat(4),
        [1, 0] = new ComplexFloat(3),
        [1, 1] = new ComplexFloat(7)
      };
      ComplexFloat b = a.GetDeterminant();
      var test = new Complex(2);
      Assert.AreEqual(b.Real, test.Real, 4);
      Assert.AreEqual(b.Imag, test.Imag, 4);
    }

    //test GetDeterminant
    [Test]
    public void GetDeterminantNotSquare()
    {
      Assert.Throws(typeof(NotSquareMatrixException), () =>
      {
        var a = new ComplexFloatMatrix(3, 2)
        {
          [0, 0] = new ComplexFloat(2),
          [0, 1] = new ComplexFloat(4),
          [1, 0] = new ComplexFloat(3),
          [1, 1] = new ComplexFloat(7),
          [2, 0] = new ComplexFloat(5),
          [2, 1] = new ComplexFloat(5)
        };
        ComplexFloat b = a.GetDeterminant();
      });
    }

    //test GetRow
    [Test]
    public void GetRow()
    {
      var a = new ComplexFloatMatrix(2, 2)
      {
        [0, 0] = new ComplexFloat(1),
        [0, 1] = new ComplexFloat(2),
        [1, 0] = new ComplexFloat(3),
        [1, 1] = new ComplexFloat(4)
      };
      ComplexFloatVector b = a.GetRow(0);
      Assert.AreEqual(b[0], a[0, 0]);
      Assert.AreEqual(b[1], a[0, 1]);
    }

    //test GetRow
    [Test]
    public void GetRowOutOfRange()
    {
      Assert.Throws(typeof(ArgumentOutOfRangeException), () =>
      {
        var a = new ComplexFloatMatrix(2, 2);
        ComplexFloatVector b = a.GetRow(3);
      });
    }

    //test GetColumn
    [Test]
    public void GetColumn()
    {
      var a = new ComplexFloatMatrix(2, 2)
      {
        [0, 0] = new ComplexFloat(1),
        [0, 1] = new ComplexFloat(2),
        [1, 0] = new ComplexFloat(3),
        [1, 1] = new ComplexFloat(4)
      };
      ComplexFloatVector b = a.GetColumn(0);
      Assert.AreEqual(b[0], a[0, 0]);
      Assert.AreEqual(b[1], a[1, 0]);
    }

    //test GetColumn
    [Test]
    public void GetColumnOutOfRange()
    {
      Assert.Throws(typeof(ArgumentOutOfRangeException), () =>
      {
        var a = new ComplexFloatMatrix(2, 2);
        ComplexFloatVector b = a.GetColumn(3);
      });
    }

    //test GetDiagonal
    [Test]
    public void GetDiagonal()
    {
      var a = new ComplexFloatMatrix(2, 2)
      {
        [0, 0] = new ComplexFloat(1),
        [0, 1] = new ComplexFloat(2),
        [1, 0] = new ComplexFloat(3),
        [1, 1] = new ComplexFloat(4)
      };
      ComplexFloatVector b = a.GetDiagonal();
      Assert.AreEqual(b[0], a[0, 0]);
      Assert.AreEqual(b[1], a[1, 1]);
    }

    //test SetRow
    [Test]
    public void SetRow()
    {
      var a = new ComplexFloatMatrix(2, 2);
      var b = new ComplexFloatVector(2)
      {
        [0] = new ComplexFloat(1, 1),
        [1] = new ComplexFloat(2, 2)
      };
      a.SetRow(0, b);
      Assert.AreEqual(b[0], a[0, 0]);
      Assert.AreEqual(b[1], a[0, 1]);
    }

    //test SetRow
    [Test]
    public void SetRowOutOfRange()
    {
      Assert.Throws(typeof(ArgumentOutOfRangeException), () =>
      {
        var a = new ComplexFloatMatrix(2, 2);
        var b = new ComplexFloatVector(2);
        a.SetRow(2, b);
      });
    }

    //test SetRow
    [Test]
    public void SetRowWrongRank()
    {
      Assert.Throws(typeof(ArgumentException), () =>
      {
        var a = new ComplexFloatMatrix(2, 2);
        var b = new ComplexFloatVector(3);
        a.SetRow(1, b);
      });
    }

    //test SetRow
    [Test]
    public void SetRowArray()
    {
      var a = new ComplexFloatMatrix(2, 2);
      var b = new ComplexFloat[2];
      b[0] = new ComplexFloat(1, 1);
      b[1] = new ComplexFloat(2, 2);

      a.SetRow(0, b);
      Assert.AreEqual(b[0], a[0, 0]);
      Assert.AreEqual(b[1], a[0, 1]);
    }

    //test SetRow
    [Test]
    public void SetRowArrayOutOfRange()
    {
      Assert.Throws(typeof(ArgumentOutOfRangeException), () =>
      {
        var a = new ComplexFloatMatrix(2, 2);
        var b = new ComplexFloat[2];
        a.SetRow(2, b);
      });
    }

    //test SetRow
    [Test]
    public void SetRowArrayWrongRank()
    {
      Assert.Throws(typeof(ArgumentException), () =>
      {
        var a = new ComplexFloatMatrix(2, 2);
        var b = new ComplexFloat[3];
        a.SetRow(1, b);
      });
    }

    //test SetColumn
    [Test]
    public void SetColumn()
    {
      var a = new ComplexFloatMatrix(2, 2);
      var b = new ComplexFloatVector(2)
      {
        [0] = 1,
        [1] = 2
      };
      a.SetColumn(0, b);
      Assert.AreEqual(b[0], a[0, 0]);
      Assert.AreEqual(b[1], a[1, 0]);
    }

    //test SetColumn
    [Test]
    public void SetColumnOutOfRange()
    {
      Assert.Throws(typeof(ArgumentOutOfRangeException), () =>
      {
        var a = new ComplexFloatMatrix(2, 2);
        var b = new ComplexFloatVector(2);
        a.SetColumn(2, b);
      });
    }

    //test SetColumn
    [Test]
    public void SetColumnWrongRank()
    {
      Assert.Throws(typeof(ArgumentException), () =>
      {
        var a = new ComplexFloatMatrix(2, 2);
        var b = new ComplexFloatVector(3);
        a.SetColumn(1, b);
      });
    }

    //test SetColumn
    [Test]
    public void SetColumnArray()
    {
      var a = new ComplexFloatMatrix(2, 2);
      var b = new ComplexFloat[2];
      b[0] = new ComplexFloat(1, 1);
      b[1] = new ComplexFloat(2, 2);
      a.SetColumn(0, b);
      Assert.AreEqual(b[0], a[0, 0]);
      Assert.AreEqual(b[1], a[1, 0]);
    }

    //test SetColumn
    [Test]
    public void SetColumnArrayOutOfRange()
    {
      Assert.Throws(typeof(ArgumentOutOfRangeException), () =>
      {
        var a = new ComplexFloatMatrix(2, 2);
        var b = new ComplexFloat[2];
        a.SetColumn(2, b);
      });
    }

    //test SetColumn
    [Test]
    public void SetColumnArrayWrongRank()
    {
      Assert.Throws(typeof(ArgumentException), () =>
      {
        var a = new ComplexFloatMatrix(2, 2);
        var b = new ComplexFloat[3];
        a.SetColumn(1, b);
      });
    }

    //test SetDiagonal
    [Test]
    public void SetDiagonal()
    {
      var a = new ComplexFloatMatrix(2, 2);
      var b = new ComplexFloatVector(2)
      {
        [0] = new ComplexFloat(1),
        [1] = new ComplexFloat(2)
      };
      a.SetDiagonal(b);
      Assert.AreEqual(b[0], a[0, 0]);
      Assert.AreEqual(b[1], a[1, 1]);
    }

    //test GetSubMatrix
    [Test]
    public void GetSubMatrix()
    {
      var a = new ComplexFloatMatrix(4)
      {
        [0, 0] = new ComplexFloat(1),
        [0, 1] = new ComplexFloat(2),
        [0, 2] = new ComplexFloat(3),
        [0, 3] = new ComplexFloat(4),
        [1, 0] = new ComplexFloat(5),
        [1, 1] = new ComplexFloat(6),
        [1, 2] = new ComplexFloat(7),
        [1, 3] = new ComplexFloat(8),
        [2, 0] = new ComplexFloat(9),
        [2, 1] = new ComplexFloat(10),
        [2, 2] = new ComplexFloat(11),
        [2, 3] = new ComplexFloat(12),
        [3, 0] = new ComplexFloat(13),
        [3, 1] = new ComplexFloat(14),
        [3, 2] = new ComplexFloat(15),
        [3, 3] = new ComplexFloat(16)
      };
      ComplexFloatMatrix b = a.GetSubMatrix(2, 2);
      ComplexFloatMatrix c = a.GetSubMatrix(0, 1, 2, 2);
      Assert.AreEqual(b.RowLength, 2);
      Assert.AreEqual(b.ColumnLength, 2);
      Assert.AreEqual(c.RowLength, 3);
      Assert.AreEqual(c.ColumnLength, 2);
      Assert.AreEqual(b[0, 0], a[2, 2]);
      Assert.AreEqual(b[0, 1], a[2, 3]);
      Assert.AreEqual(b[1, 0], a[3, 2]);
      Assert.AreEqual(b[1, 1], a[3, 3]);
      Assert.AreEqual(c[0, 0], a[0, 1]);
      Assert.AreEqual(c[0, 1], a[0, 2]);
      Assert.AreEqual(c[1, 0], a[1, 1]);
      Assert.AreEqual(c[1, 1], a[1, 2]);
      Assert.AreEqual(c[2, 0], a[2, 1]);
      Assert.AreEqual(c[2, 1], a[2, 2]);
    }

    //test GetSubMatrix
    [Test]
    public void GetSubMatrixOutRange1()
    {
      Assert.Throws(typeof(ArgumentOutOfRangeException), () =>
      {
        var a = new ComplexFloatMatrix(4);
        ComplexFloatMatrix b = a.GetSubMatrix(-1, 2);
      });
    }

    //test GetSubMatrix
    [Test]
    public void GetSubMatrixOutRange2()
    {
      Assert.Throws(typeof(ArgumentOutOfRangeException), () =>
      {
        var a = new ComplexFloatMatrix(4);
        ComplexFloatMatrix b = a.GetSubMatrix(2, 4);
      });
    }

    //test GetSubMatrix
    [Test]
    public void GetSubMatrixOutRange3()
    {
      Assert.Throws(typeof(ArgumentOutOfRangeException), () =>
      {
        var a = new ComplexFloatMatrix(4);
        ComplexFloatMatrix b = a.GetSubMatrix(0, 0, 4, 2);
      });
    }

    //test GetSubMatrix
    [Test]
    public void GetSubMatrixOutRange4()
    {
      Assert.Throws(typeof(ArgumentOutOfRangeException), () =>
      {
        var a = new ComplexFloatMatrix(4);
        ComplexFloatMatrix b = a.GetSubMatrix(0, 0, 2, 4);
      });
    }

    //test GetSubMatrix
    [Test]
    public void GetSubMatrixOutRange5()
    {
      Assert.Throws(typeof(ArgumentOutOfRangeException), () =>
      {
        var a = new ComplexFloatMatrix(4);
        ComplexFloatMatrix b = a.GetSubMatrix(0, 3, 2, 2);
      });
    }

    //test GetUpperTriangle square matrix
    [Test]
    public void GetUpperTriangleSquare()
    {
      var a = new ComplexFloatMatrix(3)
      {
        [0, 0] = new ComplexFloat(1),
        [0, 1] = new ComplexFloat(2),
        [0, 2] = new ComplexFloat(3),
        [1, 0] = new ComplexFloat(4),
        [1, 1] = new ComplexFloat(5),
        [1, 2] = new ComplexFloat(6),
        [2, 0] = new ComplexFloat(7),
        [2, 1] = new ComplexFloat(8),
        [2, 2] = new ComplexFloat(9)
      };
      ComplexFloatMatrix b = a.GetUpperTriangle();

      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0, 0], a[0, 0]);
      Assert.AreEqual(b[0, 1], a[0, 1]);
      Assert.AreEqual(b[0, 2], a[0, 2]);
      Assert.AreEqual(b[1, 0], ComplexFloat.Zero);
      Assert.AreEqual(b[1, 1], a[1, 1]);
      Assert.AreEqual(b[1, 2], a[1, 2]);
      Assert.AreEqual(b[2, 0], ComplexFloat.Zero);
      Assert.AreEqual(b[2, 1], ComplexFloat.Zero);
      Assert.AreEqual(b[2, 2], a[2, 2]);
    }

    //test GetUpperTriangle long matrix
    [Test]
    public void GetUpperTriangleLong()
    {
      var a = new ComplexFloatMatrix(3, 2)
      {
        [0, 0] = new ComplexFloat(1),
        [0, 1] = new ComplexFloat(2),
        [1, 0] = new ComplexFloat(4),
        [1, 1] = new ComplexFloat(5),
        [2, 0] = new ComplexFloat(7),
        [2, 1] = new ComplexFloat(8)
      };
      ComplexFloatMatrix b = a.GetUpperTriangle();

      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0, 0], a[0, 0]);
      Assert.AreEqual(b[0, 1], a[0, 1]);
      Assert.AreEqual(b[1, 0], ComplexFloat.Zero);
      Assert.AreEqual(b[1, 1], a[1, 1]);
      Assert.AreEqual(b[2, 0], ComplexFloat.Zero);
      Assert.AreEqual(b[2, 1], ComplexFloat.Zero);
    }

    //test GetUpperTriangle wide matrix
    [Test]
    public void GetUpperTriangleWide()
    {
      var a = new ComplexFloatMatrix(2, 3)
      {
        [0, 0] = new ComplexFloat(1),
        [0, 1] = new ComplexFloat(2),
        [0, 2] = new ComplexFloat(3),
        [1, 0] = new ComplexFloat(4),
        [1, 1] = new ComplexFloat(5),
        [1, 2] = new ComplexFloat(6)
      };
      ComplexFloatMatrix b = a.GetUpperTriangle();

      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0, 0], a[0, 0]);
      Assert.AreEqual(b[0, 1], a[0, 1]);
      Assert.AreEqual(b[0, 2], a[0, 2]);
      Assert.AreEqual(b[1, 0], ComplexFloat.Zero);
      Assert.AreEqual(b[1, 1], a[1, 1]);
      Assert.AreEqual(b[1, 2], a[1, 2]);
    }

    //test GetStrictlyUpperTriangle square matrix
    [Test]
    public void GetStrictlyUpperTriangleSquare()
    {
      var a = new ComplexFloatMatrix(3)
      {
        [0, 0] = new ComplexFloat(1),
        [0, 1] = new ComplexFloat(2),
        [0, 2] = new ComplexFloat(3),
        [1, 0] = new ComplexFloat(4),
        [1, 1] = new ComplexFloat(5),
        [1, 2] = new ComplexFloat(6),
        [2, 0] = new ComplexFloat(7),
        [2, 1] = new ComplexFloat(8),
        [2, 2] = new ComplexFloat(9)
      };
      ComplexFloatMatrix b = a.GetStrictlyUpperTriangle();

      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0, 0], ComplexFloat.Zero);
      Assert.AreEqual(b[0, 1], a[0, 1]);
      Assert.AreEqual(b[0, 2], a[0, 2]);
      Assert.AreEqual(b[1, 0], ComplexFloat.Zero);
      Assert.AreEqual(b[1, 1], ComplexFloat.Zero);
      Assert.AreEqual(b[1, 2], a[1, 2]);
      Assert.AreEqual(b[2, 0], ComplexFloat.Zero);
      Assert.AreEqual(b[2, 1], ComplexFloat.Zero);
      Assert.AreEqual(b[2, 2], ComplexFloat.Zero);
    }

    //test GetStrictlyUpperTriangle long matrix
    [Test]
    public void GetStrictlyUpperTriangleLong()
    {
      var a = new ComplexFloatMatrix(3, 2)
      {
        [0, 0] = new ComplexFloat(1),
        [0, 1] = new ComplexFloat(2),
        [1, 0] = new ComplexFloat(4),
        [1, 1] = new ComplexFloat(5),
        [2, 0] = new ComplexFloat(7),
        [2, 1] = new ComplexFloat(8)
      };
      ComplexFloatMatrix b = a.GetStrictlyUpperTriangle();

      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0, 0], ComplexFloat.Zero);
      Assert.AreEqual(b[0, 1], a[0, 1]);
      Assert.AreEqual(b[1, 0], ComplexFloat.Zero);
      Assert.AreEqual(b[1, 1], ComplexFloat.Zero);
      Assert.AreEqual(b[2, 0], ComplexFloat.Zero);
      Assert.AreEqual(b[2, 1], ComplexFloat.Zero);
    }

    //test GetStrictlyUpperTriangle wide matrix
    [Test]
    public void GetStrictlyUpperTriangleWide()
    {
      var a = new ComplexFloatMatrix(2, 3)
      {
        [0, 0] = new ComplexFloat(1),
        [0, 1] = new ComplexFloat(2),
        [0, 2] = new ComplexFloat(3),
        [1, 0] = new ComplexFloat(4),
        [1, 1] = new ComplexFloat(5),
        [1, 2] = new ComplexFloat(6)
      };
      ComplexFloatMatrix b = a.GetStrictlyUpperTriangle();

      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0, 0], ComplexFloat.Zero);
      Assert.AreEqual(b[0, 1], a[0, 1]);
      Assert.AreEqual(b[0, 2], a[0, 2]);
      Assert.AreEqual(b[1, 0], ComplexFloat.Zero);
      Assert.AreEqual(b[1, 1], ComplexFloat.Zero);
      Assert.AreEqual(b[1, 2], a[1, 2]);
    }

    //test GetLowerTriangle square matrix
    [Test]
    public void GetLowerTriangleSquare()
    {
      var a = new ComplexFloatMatrix(3)
      {
        [0, 0] = new ComplexFloat(1),
        [0, 1] = new ComplexFloat(2),
        [0, 2] = new ComplexFloat(3),
        [1, 0] = new ComplexFloat(4),
        [1, 1] = new ComplexFloat(5),
        [1, 2] = new ComplexFloat(6),
        [2, 0] = new ComplexFloat(7),
        [2, 1] = new ComplexFloat(8),
        [2, 2] = new ComplexFloat(9)
      };
      ComplexFloatMatrix b = a.GetLowerTriangle();

      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0, 0], a[0, 0]);
      Assert.AreEqual(b[0, 1], ComplexFloat.Zero);
      Assert.AreEqual(b[0, 2], ComplexFloat.Zero);
      Assert.AreEqual(b[1, 0], a[1, 0]);
      Assert.AreEqual(b[1, 1], a[1, 1]);
      Assert.AreEqual(b[1, 2], ComplexFloat.Zero);
      Assert.AreEqual(b[2, 0], a[2, 0]);
      Assert.AreEqual(b[2, 1], a[2, 1]);
      Assert.AreEqual(b[2, 2], a[2, 2]);
    }

    //test GetLowerTriangle long matrix
    [Test]
    public void GetLowerTriangleLong()
    {
      var a = new ComplexFloatMatrix(3, 2)
      {
        [0, 0] = new ComplexFloat(1),
        [0, 1] = new ComplexFloat(2),
        [1, 0] = new ComplexFloat(4),
        [1, 1] = new ComplexFloat(5),
        [2, 0] = new ComplexFloat(7),
        [2, 1] = new ComplexFloat(8)
      };
      ComplexFloatMatrix b = a.GetLowerTriangle();

      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0, 0], a[0, 0]);
      Assert.AreEqual(b[0, 1], ComplexFloat.Zero);
      Assert.AreEqual(b[1, 0], b[1, 0]);
      Assert.AreEqual(b[1, 1], a[1, 1]);
      Assert.AreEqual(b[2, 0], b[2, 0]);
      Assert.AreEqual(b[2, 1], b[2, 1]);
    }

    //test GetLowerTriangle wide matrix
    [Test]
    public void GetLowerTriangleWide()
    {
      var a = new ComplexFloatMatrix(2, 3)
      {
        [0, 0] = new ComplexFloat(1),
        [0, 1] = new ComplexFloat(2),
        [0, 2] = new ComplexFloat(3),
        [1, 0] = new ComplexFloat(4),
        [1, 1] = new ComplexFloat(5),
        [1, 2] = new ComplexFloat(6)
      };
      ComplexFloatMatrix b = a.GetLowerTriangle();

      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0, 0], a[0, 0]);
      Assert.AreEqual(b[0, 1], ComplexFloat.Zero);
      Assert.AreEqual(b[0, 2], ComplexFloat.Zero);
      Assert.AreEqual(b[1, 0], a[1, 0]);
      Assert.AreEqual(b[1, 1], a[1, 1]);
      Assert.AreEqual(b[1, 2], ComplexFloat.Zero);
    }

    //test GetStrictlyLowerTriangle square matrix
    [Test]
    public void GetStrictlyLowerTriangleSquare()
    {
      var a = new ComplexFloatMatrix(3)
      {
        [0, 0] = new ComplexFloat(1),
        [0, 1] = new ComplexFloat(2),
        [0, 2] = new ComplexFloat(3),
        [1, 0] = new ComplexFloat(4),
        [1, 1] = new ComplexFloat(5),
        [1, 2] = new ComplexFloat(6),
        [2, 0] = new ComplexFloat(7),
        [2, 1] = new ComplexFloat(8),
        [2, 2] = new ComplexFloat(9)
      };
      ComplexFloatMatrix b = a.GetStrictlyLowerTriangle();

      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0, 0], ComplexFloat.Zero);
      Assert.AreEqual(b[0, 1], ComplexFloat.Zero);
      Assert.AreEqual(b[0, 2], ComplexFloat.Zero);
      Assert.AreEqual(b[1, 0], a[1, 0]);
      Assert.AreEqual(b[1, 1], ComplexFloat.Zero);
      Assert.AreEqual(b[1, 2], ComplexFloat.Zero);
      Assert.AreEqual(b[2, 0], a[2, 0]);
      Assert.AreEqual(b[2, 1], a[2, 1]);
      Assert.AreEqual(b[2, 2], ComplexFloat.Zero);
    }

    //test GetStrictlyLowerTriangle long matrix
    [Test]
    public void GetStrictlyLowerTriangleLong()
    {
      var a = new ComplexFloatMatrix(3, 2)
      {
        [0, 0] = new ComplexFloat(1),
        [0, 1] = new ComplexFloat(2),
        [1, 0] = new ComplexFloat(4),
        [1, 1] = new ComplexFloat(5),
        [2, 0] = new ComplexFloat(7),
        [2, 1] = new ComplexFloat(8)
      };
      ComplexFloatMatrix b = a.GetStrictlyLowerTriangle();

      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0, 0], ComplexFloat.Zero);
      Assert.AreEqual(b[0, 1], ComplexFloat.Zero);
      Assert.AreEqual(b[1, 0], b[1, 0]);
      Assert.AreEqual(b[1, 1], ComplexFloat.Zero);
      Assert.AreEqual(b[2, 0], b[2, 0]);
      Assert.AreEqual(b[2, 1], b[2, 1]);
    }

    //test GetStrictlyLowerTriangle wide matrix
    [Test]
    public void GetStrictlyLowerTriangleWide()
    {
      var a = new ComplexFloatMatrix(2, 3)
      {
        [0, 0] = new ComplexFloat(1),
        [0, 1] = new ComplexFloat(2),
        [0, 2] = new ComplexFloat(3),
        [1, 0] = new ComplexFloat(4),
        [1, 1] = new ComplexFloat(5),
        [1, 2] = new ComplexFloat(6)
      };
      ComplexFloatMatrix b = a.GetStrictlyLowerTriangle();

      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0, 0], ComplexFloat.Zero);
      Assert.AreEqual(b[0, 1], ComplexFloat.Zero);
      Assert.AreEqual(b[0, 2], ComplexFloat.Zero);
      Assert.AreEqual(b[1, 0], a[1, 0]);
      Assert.AreEqual(b[1, 1], ComplexFloat.Zero);
      Assert.AreEqual(b[1, 2], ComplexFloat.Zero);
    }

    //static Negate
    [Test]
    public void Negate()
    {
      var a = new ComplexFloatMatrix(2)
      {
        [0, 0] = new ComplexFloat(1),
        [0, 1] = new ComplexFloat(2),
        [1, 0] = new ComplexFloat(3),
        [1, 1] = new ComplexFloat(4)
      };

      var b = ComplexFloatMatrix.Negate(a);
      Assert.AreEqual(b[0, 0], new ComplexFloat(-1));
      Assert.AreEqual(b[0, 1], new ComplexFloat(-2));
      Assert.AreEqual(b[1, 0], new ComplexFloat(-3));
      Assert.AreEqual(b[1, 1], new ComplexFloat(-4));
    }

    //static NegateNull
    [Test]
    public void NegateNull()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        ComplexFloatMatrix a = null;
        var b = ComplexFloatMatrix.Negate(a);
      });
    }

    //static operator -
    [Test]
    public void OperatorMinus()
    {
      var a = new ComplexFloatMatrix(2)
      {
        [0, 0] = new ComplexFloat(1),
        [0, 1] = new ComplexFloat(2),
        [1, 0] = new ComplexFloat(3),
        [1, 1] = new ComplexFloat(4)
      };

      ComplexFloatMatrix b = -a;
      Assert.AreEqual(b[0, 0], new ComplexFloat(-1));
      Assert.AreEqual(b[0, 1], new ComplexFloat(-2));
      Assert.AreEqual(b[1, 0], new ComplexFloat(-3));
      Assert.AreEqual(b[1, 1], new ComplexFloat(-4));
    }

    //static operator - null
    [Test]
    public void OperatorMinusNull()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        ComplexFloatMatrix a = null;
        ComplexFloatMatrix b = -a;
      });
    }

    //static subtact two square matrices
    [Test]
    public void StaticSubtract()
    {
      var a = new ComplexFloatMatrix(2);
      var b = new ComplexFloatMatrix(2);
      a[0, 0] = b[0, 0] = new ComplexFloat(1);
      a[0, 1] = b[0, 1] = new ComplexFloat(2);
      a[1, 0] = b[1, 0] = new ComplexFloat(3);
      a[1, 1] = b[1, 1] = new ComplexFloat(4);
      var c = ComplexFloatMatrix.Subtract(a, b);
      Assert.AreEqual(c[0, 0], ComplexFloat.Zero);
      Assert.AreEqual(c[0, 1], ComplexFloat.Zero);
      Assert.AreEqual(c[1, 0], ComplexFloat.Zero);
      Assert.AreEqual(c[1, 1], ComplexFloat.Zero);
    }

    //operator subtract two square matrices
    [Test]
    public void OperatorSubtract()
    {
      var a = new ComplexFloatMatrix(2);
      var b = new ComplexFloatMatrix(2);
      a[0, 0] = b[0, 0] = new ComplexFloat(1);
      a[0, 1] = b[0, 1] = new ComplexFloat(2);
      a[1, 0] = b[1, 0] = new ComplexFloat(3);
      a[1, 1] = b[1, 1] = new ComplexFloat(4);
      ComplexFloatMatrix c = a - b;
      Assert.AreEqual(c[0, 0], ComplexFloat.Zero);
      Assert.AreEqual(c[0, 1], ComplexFloat.Zero);
      Assert.AreEqual(c[1, 0], ComplexFloat.Zero);
      Assert.AreEqual(c[1, 1], ComplexFloat.Zero);
    }

    //member add subtract square matrices
    [Test]
    public void MemberSubtract()
    {
      var a = new ComplexFloatMatrix(2);
      var b = new ComplexFloatMatrix(2);
      a[0, 0] = b[0, 0] = new ComplexFloat(1);
      a[0, 1] = b[0, 1] = new ComplexFloat(2);
      a[1, 0] = b[1, 0] = new ComplexFloat(3);
      a[1, 1] = b[1, 1] = new ComplexFloat(4);
      a.Subtract(b);
      Assert.AreEqual(a[0, 0], ComplexFloat.Zero);
      Assert.AreEqual(a[0, 1], ComplexFloat.Zero);
      Assert.AreEqual(a[1, 0], ComplexFloat.Zero);
      Assert.AreEqual(a[1, 1], ComplexFloat.Zero);
    }

    //static Subtract two square matrices, one null
    [Test]
    public void StaticSubtractNull()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        var a = new ComplexFloatMatrix(2);
        ComplexFloatMatrix b = null;
        var c = ComplexFloatMatrix.Subtract(a, b);
      });
    }

    //operator Subtract two square matrices, one null
    [Test]
    public void OperatorSubtractNull()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        var a = new ComplexFloatMatrix(2);
        ComplexFloatMatrix b = null;
        ComplexFloatMatrix c = a - b;
      });
    }

    //member Subtract two square matrices, one null
    [Test]
    public void MemberSubtractNull()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        var a = new ComplexFloatMatrix(2);
        ComplexFloatMatrix b = null;
        a.Subtract(b);
      });
    }

    //static Subtract two incompatible matrices
    [Test]
    public void StaticSubtractIncompatible()
    {
      Assert.Throws(typeof(ArgumentException), () =>
      {
        var a = new ComplexFloatMatrix(2);
        var b = new ComplexFloatMatrix(3);
        var c = ComplexFloatMatrix.Subtract(a, b);
      });
    }

    //operator Subtract two  incompatible matrices
    [Test]
    public void OperatorSubtractIncompatible()
    {
      Assert.Throws(typeof(ArgumentException), () =>
      {
        var a = new ComplexFloatMatrix(2);
        var b = new ComplexFloatMatrix(3);
        ComplexFloatMatrix c = a - b;
      });
    }

    //member Subtract two  incompatible matricess
    [Test]
    public void MemberSubtractIncompatible()
    {
      Assert.Throws(typeof(ArgumentException), () =>
      {
        var a = new ComplexFloatMatrix(2);
        var b = new ComplexFloatMatrix(3);
        a.Subtract(b);
      });
    }

    //static add two square matrices
    [Test]
    public void StaticAdd()
    {
      var a = new ComplexFloatMatrix(2);
      var b = new ComplexFloatMatrix(2);
      a[0, 0] = b[0, 0] = new ComplexFloat(1);
      a[0, 1] = b[0, 1] = new ComplexFloat(2);
      a[1, 0] = b[1, 0] = new ComplexFloat(3);
      a[1, 1] = b[1, 1] = new ComplexFloat(4);
      var c = ComplexFloatMatrix.Add(a, b);
      Assert.AreEqual(c[0, 0], new ComplexFloat(2));
      Assert.AreEqual(c[0, 1], new ComplexFloat(4));
      Assert.AreEqual(c[1, 0], new ComplexFloat(6));
      Assert.AreEqual(c[1, 1], new ComplexFloat(8));
    }

    //operator add two square matrices
    [Test]
    public void OperatorAdd()
    {
      var a = new ComplexFloatMatrix(2);
      var b = new ComplexFloatMatrix(2);
      a[0, 0] = b[0, 0] = new ComplexFloat(1);
      a[0, 1] = b[0, 1] = new ComplexFloat(2);
      a[1, 0] = b[1, 0] = new ComplexFloat(3);
      a[1, 1] = b[1, 1] = new ComplexFloat(4);
      ComplexFloatMatrix c = a + b;
      Assert.AreEqual(c[0, 0], new ComplexFloat(2));
      Assert.AreEqual(c[0, 1], new ComplexFloat(4));
      Assert.AreEqual(c[1, 0], new ComplexFloat(6));
      Assert.AreEqual(c[1, 1], new ComplexFloat(8));
    }

    //member add two square matrices
    [Test]
    public void MemberAdd()
    {
      var a = new ComplexFloatMatrix(2);
      var b = new ComplexFloatMatrix(2);
      a[0, 0] = b[0, 0] = new ComplexFloat(1);
      a[0, 1] = b[0, 1] = new ComplexFloat(2);
      a[1, 0] = b[1, 0] = new ComplexFloat(3);
      a[1, 1] = b[1, 1] = new ComplexFloat(4);
      a.Add(b);
      Assert.AreEqual(a[0, 0], new ComplexFloat(2));
      Assert.AreEqual(a[0, 1], new ComplexFloat(4));
      Assert.AreEqual(a[1, 0], new ComplexFloat(6));
      Assert.AreEqual(a[1, 1], new ComplexFloat(8));
    }

    //static add two square matrices, one null
    [Test]
    public void StaticAddNull()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        var a = new ComplexFloatMatrix(2);
        ComplexFloatMatrix b = null;
        var c = ComplexFloatMatrix.Add(a, b);
      });
    }

    //operator add two square matrices, one null
    [Test]
    public void OperatorAddNull()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        var a = new ComplexFloatMatrix(2);
        ComplexFloatMatrix b = null;
        ComplexFloatMatrix c = a + b;
      });
    }

    //member add two square matrices, one null
    [Test]
    public void MemberAddNull()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        var a = new ComplexFloatMatrix(2);
        ComplexFloatMatrix b = null;
        a.Add(b);
      });
    }

    //static add two incompatible matrices
    [Test]
    public void StaticAddIncompatible()
    {
      Assert.Throws(typeof(ArgumentException), () =>
      {
        var a = new ComplexFloatMatrix(2);
        var b = new ComplexFloatMatrix(3);
        var c = ComplexFloatMatrix.Add(a, b);
      });
    }

    //operator add two  incompatible matrices
    [Test]
    public void OperatorAddIncompatible()
    {
      Assert.Throws(typeof(ArgumentException), () =>
      {
        var a = new ComplexFloatMatrix(2);
        var b = new ComplexFloatMatrix(3);
        ComplexFloatMatrix c = a + b;
      });
    }

    //member add two  incompatible matricess
    [Test]
    public void MemberAddIncompatible()
    {
      Assert.Throws(typeof(ArgumentException), () =>
      {
        var a = new ComplexFloatMatrix(2);
        var b = new ComplexFloatMatrix(3);
        a.Add(b);
      });
    }

    //static divide matrix by float
    [Test]
    public void StaticDivide()
    {
      var a = new ComplexFloatMatrix(2)
      {
        [0, 0] = new ComplexFloat(2),
        [0, 1] = new ComplexFloat(4),
        [1, 0] = new ComplexFloat(6),
        [1, 1] = new ComplexFloat(8)
      };
      var b = ComplexFloatMatrix.Divide(a, 2);
      Assert.AreEqual(b[0, 0], new ComplexFloat(1));
      Assert.AreEqual(b[0, 1], new ComplexFloat(2));
      Assert.AreEqual(b[1, 0], new ComplexFloat(3));
      Assert.AreEqual(b[1, 1], new ComplexFloat(4));
    }

    //operator divide matrix by float
    [Test]
    public void OperatorDivide()
    {
      var a = new ComplexFloatMatrix(2)
      {
        [0, 0] = new ComplexFloat(2),
        [0, 1] = new ComplexFloat(4),
        [1, 0] = new ComplexFloat(6),
        [1, 1] = new ComplexFloat(8)
      };
      ComplexFloatMatrix b = a / 2;
      Assert.AreEqual(b[0, 0], new ComplexFloat(1));
      Assert.AreEqual(b[0, 1], new ComplexFloat(2));
      Assert.AreEqual(b[1, 0], new ComplexFloat(3));
      Assert.AreEqual(b[1, 1], new ComplexFloat(4));
    }

    //member divide matrix by float
    [Test]
    public void MemberDivide()
    {
      var a = new ComplexFloatMatrix(2)
      {
        [0, 0] = new ComplexFloat(2),
        [0, 1] = new ComplexFloat(4),
        [1, 0] = new ComplexFloat(6),
        [1, 1] = new ComplexFloat(8)
      };
      a.Divide(2);
      Assert.AreEqual(a[0, 0], new ComplexFloat(1));
      Assert.AreEqual(a[0, 1], new ComplexFloat(2));
      Assert.AreEqual(a[1, 0], new ComplexFloat(3));
      Assert.AreEqual(a[1, 1], new ComplexFloat(4));
    }

    //static divide null matrix by float
    [Test]
    public void StaticDivideNull()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        ComplexFloatMatrix a = null;
        var b = ComplexFloatMatrix.Divide(a, 2);
      });
    }

    //operator divide null matrix by float
    [Test]
    public void OperatorDivideNull()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        ComplexFloatMatrix a = null;
        ComplexFloatMatrix b = a / 2;
      });
    }

    //copy
    [Test]
    public void Copy()
    {
      var a = new ComplexFloatMatrix(2)
      {
        [0, 0] = new ComplexFloat(1),
        [0, 1] = new ComplexFloat(2),
        [1, 0] = new ComplexFloat(3),
        [1, 1] = new ComplexFloat(4)
      };
      var b = new ComplexFloatMatrix(2);
      b.Copy(a);
      Assert.AreEqual(a[0, 0], a[0, 0]);
      Assert.AreEqual(a[0, 1], b[0, 1]);
      Assert.AreEqual(a[1, 0], b[1, 0]);
      Assert.AreEqual(a[1, 1], b[1, 1]);
    }

    //test multiply float matrix operator *
    [Test]
    public void OperatorMultiplyComplexFloatMatrix()
    {
      var a = new ComplexFloatMatrix(2)
      {
        [0, 0] = new ComplexFloat(1),
        [0, 1] = new ComplexFloat(2),
        [1, 0] = new ComplexFloat(3),
        [1, 1] = new ComplexFloat(4)
      };
      ComplexFloatMatrix b = 2.0f * a;
      Assert.AreEqual(b[0, 0], new ComplexFloat(2));
      Assert.AreEqual(b[0, 1], new ComplexFloat(4));
      Assert.AreEqual(b[1, 0], new ComplexFloat(6));
      Assert.AreEqual(b[1, 1], new ComplexFloat(8));
    }

    //test multiply float null matrix operator *
    [Test]
    public void OperatorMultiplyComplexFloatMatrixNull()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        ComplexFloatMatrix a = null;
        ComplexFloatMatrix b = 2.0f * a;
      });
    }

    //test multiply  matrix float operator *
    [Test]
    public void OperatorMultiplyMatrixComplexFloat()
    {
      var a = new ComplexFloatMatrix(2)
      {
        [0, 0] = new ComplexFloat(1),
        [0, 1] = new ComplexFloat(2),
        [1, 0] = new ComplexFloat(3),
        [1, 1] = new ComplexFloat(4)
      };
      ComplexFloatMatrix b = a * 2.0f;
      Assert.AreEqual(b[0, 0], new ComplexFloat(2));
      Assert.AreEqual(b[0, 1], new ComplexFloat(4));
      Assert.AreEqual(b[1, 0], new ComplexFloat(6));
      Assert.AreEqual(b[1, 1], new ComplexFloat(8));
    }

    //test multiply  null matrix float operator *
    [Test]
    public void OperatorMultiplyMatrixComplexFloatNull()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        ComplexFloatMatrix a = null;
        ComplexFloatMatrix b = a * 2;
      });
    }

    //test static multiply float matrix
    [Test]
    public void StaticMultiplyComplexFloatMatrix()
    {
      var a = new ComplexFloatMatrix(2)
      {
        [0, 0] = new ComplexFloat(1),
        [0, 1] = new ComplexFloat(2),
        [1, 0] = new ComplexFloat(3),
        [1, 1] = new ComplexFloat(4)
      };
      var b = ComplexFloatMatrix.Multiply(2.0f, a);
      Assert.AreEqual(b[0, 0], new ComplexFloat(2));
      Assert.AreEqual(b[0, 1], new ComplexFloat(4));
      Assert.AreEqual(b[1, 0], new ComplexFloat(6));
      Assert.AreEqual(b[1, 1], new ComplexFloat(8));
    }

    //test static multiply float null matrix
    [Test]
    public void StaticMultiplyComplexFloatMatrixNull()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        ComplexFloatMatrix a = null;
        var b = ComplexFloatMatrix.Multiply(2.0f, a);
      });
    }

    //test static multiply  matrix float
    [Test]
    public void StaticMultiplyMatrixComplexFloat()
    {
      var a = new ComplexFloatMatrix(2)
      {
        [0, 0] = new ComplexFloat(1),
        [0, 1] = new ComplexFloat(2),
        [1, 0] = new ComplexFloat(3),
        [1, 1] = new ComplexFloat(4)
      };
      var b = ComplexFloatMatrix.Multiply(a, 2.0f);

      Assert.AreEqual(b[0, 0], new ComplexFloat(2));
      Assert.AreEqual(b[0, 1], new ComplexFloat(4));
      Assert.AreEqual(b[1, 0], new ComplexFloat(6));
      Assert.AreEqual(b[1, 1], new ComplexFloat(8));
    }

    //test static multiply  null matrix float operator *
    [Test]
    public void StaticMultiplyMatrixComplexFloatNull()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        ComplexFloatMatrix a = null;
        var b = ComplexFloatMatrix.Multiply(a, 2.0f);
      });
    }

    //test member multiply  float
    [Test]
    public void MemberMultiplyComplexFloat()
    {
      var a = new ComplexFloatMatrix(2)
      {
        [0, 0] = new ComplexFloat(1),
        [0, 1] = new ComplexFloat(2),
        [1, 0] = new ComplexFloat(3),
        [1, 1] = new ComplexFloat(4)
      };
      a.Multiply(2.0f);
      Assert.AreEqual(a[0, 0], new ComplexFloat(2));
      Assert.AreEqual(a[0, 1], new ComplexFloat(4));
      Assert.AreEqual(a[1, 0], new ComplexFloat(6));
      Assert.AreEqual(a[1, 1], new ComplexFloat(8));
    }

    //test multiply  matrix vector operator *
    [Test]
    public void OperatorMultiplyMatrixVector()
    {
      var a = new ComplexFloatMatrix(2)
      {
        [0, 0] = new ComplexFloat(1),
        [0, 1] = new ComplexFloat(2),
        [1, 0] = new ComplexFloat(3),
        [1, 1] = new ComplexFloat(4)
      };
      var b = new ComplexFloatVector(2, 2.0f);
      ComplexFloatVector c = a * b;
      Assert.AreEqual(c[0], new ComplexFloat(6));
      Assert.AreEqual(c[1], new ComplexFloat(14));
    }

    //test multiply  matrix nonconform vector operator *
    [Test]
    public void OperatorMultiplyMatrixNonConformVector()
    {
      Assert.Throws(typeof(ArgumentException), () =>
      {
        var a = new ComplexFloatMatrix(2);
        var b = new ComplexFloatVector(3, 2.0f);
        ComplexFloatVector c = a * b;
      });
    }

    //test multiply null matrix vector operator *
    [Test]
    public void OperatorMultiplyNullMatrixVector()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        ComplexFloatMatrix a = null;
        var b = new ComplexFloatVector(2, 2.0f);
        ComplexFloatVector c = a * b;
      });
    }

    //test multiply matrix null vector operator *
    [Test]
    public void OperatorMultiplyMatrixNullVector()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        var a = new ComplexFloatMatrix(2);
        ComplexFloatVector b = null;
        ComplexFloatVector c = a * b;
      });
    }

    //test static multiply  matrix vector
    [Test]
    public void StaticMultiplyMatrixVector()
    {
      var a = new ComplexFloatMatrix(2)
      {
        [0, 0] = new ComplexFloat(1),
        [0, 1] = new ComplexFloat(2),
        [1, 0] = new ComplexFloat(3),
        [1, 1] = new ComplexFloat(4)
      };
      var b = new ComplexFloatVector(2, 2.0f);
      ComplexFloatVector c = ComplexFloatMatrix.Multiply(a, b);
      Assert.AreEqual(c[0], new ComplexFloat(6));
      Assert.AreEqual(c[1], new ComplexFloat(14));
    }

    //test static multiply  matrix nonconform vector
    [Test]
    public void StaticMultiplyMatrixNonConformVector()
    {
      Assert.Throws(typeof(ArgumentException), () =>
      {
        var a = new ComplexFloatMatrix(2);
        var b = new ComplexFloatVector(3, 2.0f);
        ComplexFloatVector c = a * b;
      });
    }

    //test static multiply null matrix vector
    [Test]
    public void StaticMultiplyNullMatrixVector()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        ComplexFloatMatrix a = null;
        var b = new ComplexFloatVector(2, 2.0f);
        ComplexFloatVector c = ComplexFloatMatrix.Multiply(a, b);
      });
    }

    //test static multiply matrix null vector
    [Test]
    public void StaticMultiplyMatrixNullVector()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        var a = new ComplexFloatMatrix(2);
        ComplexFloatVector b = null;
        ComplexFloatVector c = ComplexFloatMatrix.Multiply(a, b);
      });
    }

    //test member multiply vector
    [Test]
    public void MemberMultiplyVector()
    {
      var a = new ComplexFloatMatrix(2)
      {
        [0, 0] = new ComplexFloat(1),
        [0, 1] = new ComplexFloat(2),
        [1, 0] = new ComplexFloat(3),
        [1, 1] = new ComplexFloat(4)
      };
      var b = new ComplexFloatVector(2, 2.0f);
      a.Multiply(b);
      Assert.AreEqual(a[0, 0], new ComplexFloat(6));
      Assert.AreEqual(a[1, 0], new ComplexFloat(14));
      Assert.AreEqual(a.ColumnLength, 1);
      Assert.AreEqual(a.RowLength, 2);
    }

    //test member multiply  matrix nonconform vector
    [Test]
    public void MemberMultiplyMatrixNonConformVector()
    {
      Assert.Throws(typeof(ArgumentException), () =>
      {
        var a = new ComplexFloatMatrix(2);
        var b = new ComplexFloatVector(3, 2.0f);
        a.Multiply(b);
      });
    }

    //test member multiply null vector
    [Test]
    public void MemberMultiplyNullVector()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        var a = new ComplexFloatMatrix(2);
        ComplexFloatVector b = null;
        a.Multiply(b);
      });
    }

    //test multiply  matrix matrix operator *
    [Test]
    public void OperatorMultiplyMatrixMatrix()
    {
      var a = new ComplexFloatMatrix(2)
      {
        [0, 0] = new ComplexFloat(1),
        [0, 1] = new ComplexFloat(2),
        [1, 0] = new ComplexFloat(3),
        [1, 1] = new ComplexFloat(4)
      };
      var b = new ComplexFloatMatrix(2, 2.0f);
      ComplexFloatMatrix c = a * b;
      Assert.AreEqual(c[0, 0], new ComplexFloat(6));
      Assert.AreEqual(c[0, 1], new ComplexFloat(6));
      Assert.AreEqual(c[1, 0], new ComplexFloat(14));
      Assert.AreEqual(c[1, 1], new ComplexFloat(14));
    }

    //test multiply  nonconform matrix matrix operator *
    [Test]
    public void OperatorMultiplyMatrixNonConformMatrix()
    {
      Assert.Throws(typeof(ArgumentException), () =>
      {
        var a = new ComplexFloatMatrix(2);
        var b = new ComplexFloatMatrix(3, 2, 2.0f);
        ComplexFloatMatrix c = a * b;
      });
    }

    //test multiply  long matrix wide matrix operator *
    [Test]
    public void OperatorMultiplyLongMatrixWideMatrix()
    {
      var a = new ComplexFloatMatrix(3, 2, 1);
      var b = new ComplexFloatMatrix(2, 3, 2);
      ComplexFloatMatrix c = a * b;
      Assert.AreEqual(c[0, 0], new ComplexFloat(4));
      Assert.AreEqual(c[0, 1], new ComplexFloat(4));
      Assert.AreEqual(c[0, 1], new ComplexFloat(4));
      Assert.AreEqual(c[1, 0], new ComplexFloat(4));
      Assert.AreEqual(c[1, 1], new ComplexFloat(4));
      Assert.AreEqual(c[1, 2], new ComplexFloat(4));
      Assert.AreEqual(c[2, 0], new ComplexFloat(4));
      Assert.AreEqual(c[2, 1], new ComplexFloat(4));
      Assert.AreEqual(c[2, 2], new ComplexFloat(4));
    }

    //test multiply  wide matrix long matrix operator *
    [Test]
    public void OperatorMultiplyWideMatrixLongMatrix()
    {
      var a = new ComplexFloatMatrix(2, 3, 1);
      var b = new ComplexFloatMatrix(3, 2, 2);
      ComplexFloatMatrix c = a * b;
      Assert.AreEqual(c[0, 0], new ComplexFloat(6));
      Assert.AreEqual(c[0, 1], new ComplexFloat(6));
      Assert.AreEqual(c[1, 0], new ComplexFloat(6));
      Assert.AreEqual(c[1, 1], new ComplexFloat(6));
    }

    //test multiply null matrix matrix operator *
    [Test]
    public void OperatorMultiplyNullMatrixMatrix()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        ComplexFloatMatrix a = null;
        var b = new ComplexFloatMatrix(2, 2.0f);
        ComplexFloatMatrix c = a * b;
      });
    }

    //test multiply matrix null matrix operator *
    [Test]
    public void OperatorMultiplyMatrixNullMatrix()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        var a = new ComplexFloatMatrix(2, 2.0f);
        ComplexFloatMatrix b = null;
        ComplexFloatMatrix c = a * b;
      });
    }

    //test static multiply  matrix matrix
    [Test]
    public void StaticMultiplyMatrixMatrix()
    {
      var a = new ComplexFloatMatrix(2)
      {
        [0, 0] = new ComplexFloat(1),
        [0, 1] = new ComplexFloat(2),
        [1, 0] = new ComplexFloat(3),
        [1, 1] = new ComplexFloat(4)
      };
      var b = new ComplexFloatMatrix(2, 2, 2.0f);
      var c = ComplexFloatMatrix.Multiply(a, b);
      Assert.AreEqual(c[0, 0], new ComplexFloat(6));
      Assert.AreEqual(c[0, 1], new ComplexFloat(6));
      Assert.AreEqual(c[1, 0], new ComplexFloat(14));
      Assert.AreEqual(c[1, 1], new ComplexFloat(14));
    }

    //test static multiply nonconform matrix matrix
    [Test]
    public void StaticMultiplyMatrixNonConformMatrix()
    {
      Assert.Throws(typeof(ArgumentException), () =>
      {
        var a = new ComplexFloatMatrix(2);
        var b = new ComplexFloatMatrix(3, 2, 2.0f);
        var c = ComplexFloatMatrix.Multiply(a, b);
      });
    }

    //test static multiply  long matrix wide matrix
    [Test]
    public void StaticMultiplyLongMatrixWideMatrix()
    {
      var a = new ComplexFloatMatrix(3, 2, 1);
      var b = new ComplexFloatMatrix(2, 3, 2);
      var c = ComplexFloatMatrix.Multiply(a, b);
      Assert.AreEqual(c[0, 0], new ComplexFloat(4));
      Assert.AreEqual(c[0, 1], new ComplexFloat(4));
      Assert.AreEqual(c[0, 1], new ComplexFloat(4));
      Assert.AreEqual(c[1, 0], new ComplexFloat(4));
      Assert.AreEqual(c[1, 1], new ComplexFloat(4));
      Assert.AreEqual(c[1, 2], new ComplexFloat(4));
      Assert.AreEqual(c[2, 0], new ComplexFloat(4));
      Assert.AreEqual(c[2, 1], new ComplexFloat(4));
      Assert.AreEqual(c[2, 2], new ComplexFloat(4));
    }

    //test static multiply  wide matrix long matrix
    [Test]
    public void StaticMultiplyWideMatrixLongMatrix()
    {
      var a = new ComplexFloatMatrix(2, 3, 1);
      var b = new ComplexFloatMatrix(3, 2, 2);
      var c = ComplexFloatMatrix.Multiply(a, b);
      Assert.AreEqual(c[0, 0], new ComplexFloat(6));
      Assert.AreEqual(c[0, 1], new ComplexFloat(6));
      Assert.AreEqual(c[1, 0], new ComplexFloat(6));
      Assert.AreEqual(c[1, 1], new ComplexFloat(6));
    }

    //test static multiply null matrix matrix
    [Test]
    public void StaticMultiplyNullMatrixMatrix()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        ComplexFloatMatrix a = null;
        var b = new ComplexFloatMatrix(2, 2.0f);
        var c = ComplexFloatMatrix.Multiply(a, b);
      });
    }

    //test static multiply matrix null matrix
    [Test]
    public void StaticMultiplyMatrixNullMatrix()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        var a = new ComplexFloatMatrix(2, 2.0f);
        ComplexFloatMatrix b = null;
        var c = ComplexFloatMatrix.Multiply(a, b);
      });
    }

    //test member multiply  matrix matrix
    [Test]
    public void MemberMultiplyMatrixMatrix()
    {
      var a = new ComplexFloatMatrix(2)
      {
        [0, 0] = new ComplexFloat(1),
        [0, 1] = new ComplexFloat(2),
        [1, 0] = new ComplexFloat(3),
        [1, 1] = new ComplexFloat(4)
      };
      var b = new ComplexFloatMatrix(2, 2, 2.0f);
      a.Multiply(b);
      Assert.AreEqual(a[0, 0], new ComplexFloat(6));
      Assert.AreEqual(a[0, 1], new ComplexFloat(6));
      Assert.AreEqual(a[1, 0], new ComplexFloat(14));
      Assert.AreEqual(a[1, 1], new ComplexFloat(14));
    }

    //test member multiply nonconform matrix matrix
    [Test]
    public void MemberMultiplyMatrixNonConformMatrix()
    {
      Assert.Throws(typeof(ArgumentException), () =>
      {
        var a = new ComplexFloatMatrix(2);
        var b = new ComplexFloatMatrix(3, 2, 2.0f);
        a.Multiply(b);
      });
    }

    //test member multiply  long matrix wide matrix
    [Test]
    public void MemberMultiplyLongMatrixWideMatrix()
    {
      var a = new ComplexFloatMatrix(3, 2, 1);
      var b = new ComplexFloatMatrix(2, 3, 2);
      a.Multiply(b);
      Assert.AreEqual(a[0, 0], new ComplexFloat(4));
      Assert.AreEqual(a[0, 1], new ComplexFloat(4));
      Assert.AreEqual(a[0, 1], new ComplexFloat(4));
      Assert.AreEqual(a[1, 0], new ComplexFloat(4));
      Assert.AreEqual(a[1, 1], new ComplexFloat(4));
      Assert.AreEqual(a[1, 2], new ComplexFloat(4));
      Assert.AreEqual(a[2, 0], new ComplexFloat(4));
      Assert.AreEqual(a[2, 1], new ComplexFloat(4));
      Assert.AreEqual(a[2, 2], new ComplexFloat(4));
    }

    //test member multiply  wide matrix long matrix
    [Test]
    public void MemberMultiplyWideMatrixLongMatrix()
    {
      var a = new ComplexFloatMatrix(2, 3, 1);
      var b = new ComplexFloatMatrix(3, 2, 2);
      a.Multiply(b);
      Assert.AreEqual(a[0, 0], new ComplexFloat(6));
      Assert.AreEqual(a[0, 1], new ComplexFloat(6));
      Assert.AreEqual(a[1, 0], new ComplexFloat(6));
      Assert.AreEqual(a[1, 1], new ComplexFloat(6));
    }

    //test member multiply null matrix matrix
    [Test]
    public void MemberMultiplyNullMatrixMatrix()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        var a = new ComplexFloatMatrix(2, 2.0f);
        ComplexFloatMatrix b = null;
        a.Multiply(b);
      });
    }

    //copy null
    [Test]
    public void CopyNull()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        ComplexFloatMatrix a = null;
        var b = new ComplexFloatMatrix(2);
        b.Copy(a);
      });
    }

    //Norm
    [Test]
    public void Norms()
    {
      var a = new ComplexFloatMatrix(2)
      {
        [0, 0] = new ComplexFloat(1.1f, 1.1f),
        [0, 1] = new ComplexFloat(2.2f, -2.2f),
        [1, 0] = new ComplexFloat(3.3f, 3.3f),
        [1, 1] = new ComplexFloat(4.4f, -4.4f)
      };
      Assert.AreEqual(a.GetL1Norm(), 9.334, TOLERENCE);
      Assert.AreEqual(a.GetL2Norm(), 8.502, TOLERENCE);
      Assert.AreEqual(a.GetInfinityNorm(), 10.889, TOLERENCE);
      Assert.AreEqual(a.GetFrobeniusNorm(), 8.521, TOLERENCE);
    }

    //Wide Norm
    [Test]
    public void WideNorms()
    {
      var a = new ComplexFloatMatrix(2, 3)
      {
        [0, 0] = new ComplexFloat(1.1f, 1.1f),
        [0, 1] = new ComplexFloat(2.2f, -2.2f),
        [0, 2] = new ComplexFloat(3.3f, 3.3f),
        [1, 0] = new ComplexFloat(4.4f, -4.4f),
        [1, 1] = new ComplexFloat(5.5f, 5.5f),
        [1, 2] = new ComplexFloat(6.6f, -6.6f)
      };
      Assert.AreEqual(a.GetL1Norm(), 14.001, TOLERENCE);
      Assert.AreEqual(a.GetL2Norm(), 13.845, TOLERENCE);
      Assert.AreEqual(a.GetInfinityNorm(), 23.335, TOLERENCE);
      Assert.AreEqual(a.GetFrobeniusNorm(), 14.840, TOLERENCE);
    }

    //Long Norm
    [Test]
    public void LongNorms()
    {
      var a = new ComplexFloatMatrix(3, 2)
      {
        [0, 0] = new ComplexFloat(1.1f, 1.1f),
        [0, 1] = new ComplexFloat(2.2f, -2.2f),
        [1, 0] = new ComplexFloat(3.3f, 3.3f),
        [1, 1] = new ComplexFloat(4.4f, -4.4f),
        [2, 0] = new ComplexFloat(5.5f, 5.5f),
        [2, 1] = new ComplexFloat(6.6f, -6.6f)
      };
      Assert.AreEqual(a.GetL1Norm(), 18.668, TOLERENCE);
      Assert.AreEqual(a.GetL2Norm(), 14.818, TOLERENCE);
      Assert.AreEqual(a.GetInfinityNorm(), 17.112, TOLERENCE);
      Assert.AreEqual(a.GetFrobeniusNorm(), 14.840, TOLERENCE);
    }

    //Condition
    [Test]
    public void Condition()
    {
      var a = new ComplexFloatMatrix(2)
      {
        [0, 0] = new ComplexFloat(1.1f, 1.1f),
        [0, 1] = new ComplexFloat(2.2f, -2.2f),
        [1, 0] = new ComplexFloat(3.3f, 3.3f),
        [1, 1] = new ComplexFloat(4.4f, -4.4f)
      };
      Assert.AreEqual(a.GetConditionNumber(), 14.933, TOLERENCE);
    }

    //Wide Condition
    [Test]
    public void WideCondition()
    {
      Assert.Throws(typeof(NotSquareMatrixException), () =>
      {
        var a = new ComplexFloatMatrix(2, 3);
        a.GetConditionNumber();
      });
    }

    //Long Condition
    [Test]
    public void LongCondition()
    {
      Assert.Throws(typeof(NotSquareMatrixException), () =>
      {
        var a = new ComplexFloatMatrix(3, 2);
        a.GetConditionNumber();
      });
    }

    //clone
    [Test]
    public void Clone()
    {
      var a = new ComplexFloatMatrix(2)
      {
        [0, 0] = new ComplexFloat(1),
        [0, 1] = new ComplexFloat(2),
        [1, 0] = new ComplexFloat(3),
        [1, 1] = new ComplexFloat(4)
      };
      var b = a.Clone();
      Assert.AreEqual(a[0, 0], a[0, 0]);
      Assert.AreEqual(a[0, 1], b[0, 1]);
      Assert.AreEqual(a[1, 0], b[1, 0]);
      Assert.AreEqual(a[1, 1], b[1, 1]);
    }
  }
}
