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

  public class ComplexFloatMatrixTest
  {
    private const double TOLERANCE = 0.001;

    //Test dimensions Constructor.
    [Fact]
    public void CtorDimensions()
    {
      var test = new ComplexFloatMatrix(2, 2);

      Assert.Equal(2, test.RowLength);
      Assert.Equal(2, test.ColumnLength);
      Assert.Equal(test[0, 0], ComplexFloat.Zero);
      Assert.Equal(test[0, 1], ComplexFloat.Zero);
      Assert.Equal(test[1, 0], ComplexFloat.Zero);
      Assert.Equal(test[1, 1], ComplexFloat.Zero);
    }

    //Test Intital Values Constructor.
    [Fact]
    public void CtorInitialValues()
    {
      var test = new ComplexFloatMatrix(2, 2, new ComplexFloat(1, 1));

      Assert.Equal(2, test.RowLength);
      Assert.Equal(2, test.ColumnLength);
      var value = new ComplexFloat(1, 1);
      Assert.Equal(test[0, 0], value);
      Assert.Equal(test[0, 1], value);
      Assert.Equal(test[1, 0], value);
      Assert.Equal(test[1, 1], value);
    }

    //Test Copy Constructor.
    [Fact]
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

      Assert.Equal(a.RowLength, b.RowLength);
      Assert.Equal(a.ColumnLength, b.ColumnLength);
      Assert.Equal(a[0, 0], b[0, 0]);
      Assert.Equal(a[0, 1], b[0, 1]);
      Assert.Equal(a[1, 0], b[1, 0]);
      Assert.Equal(a[1, 1], b[1, 1]);
    }

    //Test Copy Constructor.
    [Fact]
    public void CtorCopyNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        ComplexFloatMatrix a = null;
        var b = new ComplexFloatMatrix(a);
      });
    }

    //Test Copy Constructor.
    [Fact]
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
      Assert.Equal(a.RowLength, b.RowLength);
      Assert.Equal(a.ColumnLength, b.ColumnLength);
      Assert.Equal(a[0, 0].Real, b[0, 0].Real);
      Assert.Equal(a[0, 1].Real, b[0, 1].Real);
      Assert.Equal(a[1, 0].Real, b[1, 0].Real);
      Assert.Equal(a[1, 1].Real, b[1, 1].Real);
      Assert.Equal(a[0, 0].Imag, b[0, 0].Imag);
      Assert.Equal(a[0, 1].Imag, b[0, 1].Imag);
      Assert.Equal(a[1, 0].Imag, b[1, 0].Imag);
      Assert.Equal(a[1, 1].Imag, b[1, 1].Imag);
    }

    //Test Copy Constructor.
    [Fact]
    public void CtorCopyComplexFloatNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        FloatMatrix a = null;
        var b = new ComplexFloatMatrix(a);
      });
    }

    //Test Copy Constructor.
    [Fact]
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
      Assert.Equal(a.RowLength, b.RowLength);
      Assert.Equal(a.ColumnLength, b.ColumnLength);
      Assert.Equal(a[0, 0], b[0, 0].Real);
      Assert.Equal(a[0, 1], b[0, 1].Real);
      Assert.Equal(a[1, 0], b[1, 0].Real);
      Assert.Equal(a[1, 1], b[1, 1].Real);
      Assert.Equal(0, b[0, 0].Imag);
      Assert.Equal(0, b[0, 1].Imag);
      Assert.Equal(0, b[1, 0].Imag);
      Assert.Equal(0, b[1, 1].Imag);
    }

    //Test Copy Constructor.
    [Fact]
    public void CtorCopyFloatNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        FloatMatrix a = null;
        var b = new ComplexFloatMatrix(a);
      });
    }

    //Test Copy Constructor.
    [Fact]
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
      Assert.Equal(a.RowLength, b.RowLength);
      Assert.Equal(a.ColumnLength, b.ColumnLength);
      Assert.Equal(a[0, 0], b[0, 0].Real);
      Assert.Equal(a[0, 1], b[0, 1].Real);
      Assert.Equal(a[1, 0], b[1, 0].Real);
      Assert.Equal(a[1, 1], b[1, 1].Real);
      Assert.Equal(0, b[0, 0].Imag);
      Assert.Equal(0, b[0, 1].Imag);
      Assert.Equal(0, b[1, 0].Imag);
      Assert.Equal(0, b[1, 1].Imag);
    }

    //Test Copy Constructor.
    [Fact]
    public void CtorCopyDoubleNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        FloatMatrix a = null;
        var b = new ComplexFloatMatrix(a);
      });
    }

    //Test Multiple Dimensional ComplexFloatArray Constructor with Square array.
    [Fact]
    public void CtorMultDimComplexFloatSquare()
    {
      var values = new ComplexFloat[2, 2];

      values[0, 0] = new ComplexFloat(1, 1);
      values[0, 1] = new ComplexFloat(2, 2);
      values[1, 0] = new ComplexFloat(3, 3);
      values[1, 1] = new ComplexFloat(4, 4);

      var test = new ComplexFloatMatrix(values);

      Assert.Equal(2, test.RowLength);
      Assert.Equal(2, test.ColumnLength);
      Assert.Equal(test[0, 0], values[0, 0]);
      Assert.Equal(test[0, 1], values[0, 1]);
      Assert.Equal(test[1, 0], values[1, 0]);
      Assert.Equal(test[1, 1], values[1, 1]);
    }

    //Test Multiple Dimensional ComplexFloatArray Constructor with wide array.
    [Fact]
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

      Assert.Equal(2, test.RowLength);
      Assert.Equal(3, test.ColumnLength);
      Assert.Equal(test[0, 0], values[0, 0]);
      Assert.Equal(test[0, 1], values[0, 1]);
      Assert.Equal(test[0, 2], values[0, 2]);
      Assert.Equal(test[1, 0], values[1, 0]);
      Assert.Equal(test[1, 1], values[1, 1]);
      Assert.Equal(test[1, 2], values[1, 2]);
    }

    //Test Multiple Dimensional ComplexFloatArray Constructor with long array.
    [Fact]
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

      Assert.Equal(3, test.RowLength);
      Assert.Equal(2, test.ColumnLength);
      Assert.Equal(test[0, 0], values[0, 0]);
      Assert.Equal(test[0, 1], values[0, 1]);
      Assert.Equal(test[1, 0], values[1, 0]);
      Assert.Equal(test[1, 1], values[1, 1]);
      Assert.Equal(test[2, 0], values[2, 0]);
      Assert.Equal(test[2, 1], values[2, 1]);
    }

    //Test Multiple Dimensional ComplexFloat Array Constructor with null.
    [Fact]
    public void CtorMultDimComplexFloatNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        float[,] values = null;
        var test = new ComplexFloatMatrix(values);
      });
    }

    //Test Multiple Dimensional DoubleArray Constructor with Square array.
    [Fact]
    public void CtorMultDimDoubleSquare()
    {
      float[,] values = new float[2, 2];

      values[0, 0] = 1;
      values[0, 1] = 2;
      values[1, 0] = 3;
      values[1, 1] = 4;

      var test = new ComplexFloatMatrix(values);

      Assert.Equal(2, test.RowLength);
      Assert.Equal(2, test.ColumnLength);
      Assert.Equal(values[0, 0], test[0, 0].Real);
      Assert.Equal(values[0, 1], test[0, 1].Real);
      Assert.Equal(values[1, 0], test[1, 0].Real);
      Assert.Equal(values[1, 1], test[1, 1].Real);
    }

    //Test Multiple Dimensional DoubleArray Constructor with wide array.
    [Fact]
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

      Assert.Equal(2, test.RowLength);
      Assert.Equal(3, test.ColumnLength);
      Assert.Equal(test[0, 0].Real, values[0, 0]);
      Assert.Equal(test[0, 1].Real, values[0, 1]);
      Assert.Equal(test[0, 2].Real, values[0, 2]);
      Assert.Equal(test[1, 0].Real, values[1, 0]);
      Assert.Equal(test[1, 1].Real, values[1, 1]);
      Assert.Equal(test[1, 2].Real, values[1, 2]);
    }

    //Test Multiple Dimensional DoubleArray Constructor with long array.
    [Fact]
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

      Assert.Equal(3, test.RowLength);
      Assert.Equal(2, test.ColumnLength);
      Assert.Equal(test[0, 0].Real, values[0, 0]);
      Assert.Equal(test[0, 1].Real, values[0, 1]);
      Assert.Equal(test[1, 0].Real, values[1, 0]);
      Assert.Equal(test[1, 1].Real, values[1, 1]);
      Assert.Equal(test[2, 0].Real, values[2, 0]);
      Assert.Equal(test[2, 1].Real, values[2, 1]);
    }

    //Test Multiple Dimensional Double Array Constructor with null.
    [Fact]
    public void CtorMultDimDoubleNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        float[,] values = null;
        var test = new ComplexFloatMatrix(values);
      });
    }

    //Test Multiple Dimensional Float Array Constructor with Square array.
    [Fact]
    public void CtorMultDimFloatSquare()
    {
      float[,] values = new float[2, 2];

      values[0, 0] = 0;
      values[0, 1] = 1;
      values[1, 0] = 2;
      values[1, 1] = 3;

      var test = new ComplexFloatMatrix(values);

      Assert.Equal(2, test.RowLength);
      Assert.Equal(2, test.ColumnLength);
      Assert.Equal(test[0, 0].Real, values[0, 0]);
      Assert.Equal(test[0, 1].Real, values[0, 1]);
      Assert.Equal(test[1, 0].Real, values[1, 0]);
      Assert.Equal(test[1, 1].Real, values[1, 1]);
    }

    //Test Multiple Dimensional Float Array Constructor with wide array.
    [Fact]
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

      Assert.Equal(2, test.RowLength);
      Assert.Equal(3, test.ColumnLength);
      Assert.Equal(test[0, 0].Real, values[0, 0]);
      Assert.Equal(test[0, 1].Real, values[0, 1]);
      Assert.Equal(test[0, 2].Real, values[0, 2]);
      Assert.Equal(test[1, 0].Real, values[1, 0]);
      Assert.Equal(test[1, 1].Real, values[1, 1]);
      Assert.Equal(test[1, 2].Real, values[1, 2]);
    }

    //Test Multiple Dimensional FloatArray Constructor with long array.
    [Fact]
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

      Assert.Equal(3, test.RowLength);
      Assert.Equal(2, test.ColumnLength);
      Assert.Equal(test[0, 0].Real, values[0, 0]);
      Assert.Equal(test[0, 1].Real, values[0, 1]);
      Assert.Equal(test[1, 0].Real, values[1, 0]);
      Assert.Equal(test[1, 1].Real, values[1, 1]);
      Assert.Equal(test[2, 0].Real, values[2, 0]);
      Assert.Equal(test[2, 1].Real, values[2, 1]);
    }

    //Test Multiple Dimensional Float Array Constructor with null.
    [Fact]
    public void CtorMultDimFloatNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        float[,] values = null;
        var test = new ComplexFloatMatrix(values);
      });
    }

    //Test implicit conversion from ComplexFloatMatrix.
    [Fact]
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
      Assert.Equal(a.RowLength, b.RowLength);
      Assert.Equal(a.ColumnLength, b.ColumnLength);
      Assert.Equal(a[0, 0], b[0, 0]);
      Assert.Equal(a[0, 1], b[0, 1]);
      Assert.Equal(a[1, 0], b[1, 0]);
      Assert.Equal(a[1, 1], b[1, 1]);
    }

    //Test implicit conversion from null Complexfloatmatrix.
    [Fact]
    public void ImplictComplexFloatMatrixNull()
    {
      ComplexFloatMatrix a = null;
      ComplexFloatMatrix b = a;
      Assert.True(b is null);
    }

    //Test implicit conversion from Complexfloatmatrix.
    [Fact]
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
      Assert.Equal(a.RowLength, b.RowLength);
      Assert.Equal(a.ColumnLength, b.ColumnLength);
      Assert.Equal(a[0, 0], b[0, 0]);
      Assert.Equal(a[0, 1], b[0, 1]);
      Assert.Equal(a[1, 0], b[1, 0]);
      Assert.Equal(a[1, 1], b[1, 1]);
    }

    //Test implicit conversion from null ComplexFoatmatrix.
    [Fact]
    public void ImplictToComplexFloatMatrixNull()
    {
      ComplexFloatMatrix a = null;
      var b = ComplexFloatMatrix.ToComplexFloatMatrix(a);
      Assert.True(b is null);
    }

    //Test implicit conversion from Doublematrix.
    [Fact]
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
      Assert.Equal(a.RowLength, b.RowLength);
      Assert.Equal(a.ColumnLength, b.ColumnLength);
      Assert.Equal(a[0, 0], b[0, 0].Real);
      Assert.Equal(a[0, 1], b[0, 1].Real);
      Assert.Equal(a[1, 0], b[1, 0].Real);
      Assert.Equal(a[1, 1], b[1, 1].Real);
    }

    //Test implicit conversion from null Doublematrix.
    [Fact]
    public void ImplictDoubleMatrixNull()
    {
      FloatMatrix a = null;
      ComplexFloatMatrix b = a;
      Assert.True(b is null);
    }

    //Test implicit conversion from floatmatrix.
    [Fact]
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
      Assert.Equal(a.RowLength, b.RowLength);
      Assert.Equal(a.ColumnLength, b.ColumnLength);
      Assert.Equal(a[0, 0], b[0, 0].Real);
      Assert.Equal(a[0, 1], b[0, 1].Real);
      Assert.Equal(a[1, 0], b[1, 0].Real);
      Assert.Equal(a[1, 1], b[1, 1].Real);
    }

    //Test implicit conversion from null floatmatrix.
    [Fact]
    public void ImplictToDoubleMatrixMatrixNull()
    {
      FloatMatrix a = null;
      var b = ComplexFloatMatrix.ToComplexFloatMatrix(a);
      Assert.True(b is null);
    }

    //Test implicit conversion from floatmatrix.
    [Fact]
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
      Assert.Equal(a.RowLength, b.RowLength);
      Assert.Equal(a.ColumnLength, b.ColumnLength);
      Assert.Equal(a[0, 0], b[0, 0].Real);
      Assert.Equal(a[0, 1], b[0, 1].Real);
      Assert.Equal(a[1, 0], b[1, 0].Real);
      Assert.Equal(a[1, 1], b[1, 1].Real);
    }

    //Test implicit conversion from null floatmatrix.
    [Fact]
    public void ImplictFloatMatrixNull()
    {
      FloatMatrix a = null;
      ComplexFloatMatrix b = a;
      Assert.True(b is null);
    }

    //Test implicit conversion from floatmatrix.
    [Fact]
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
      Assert.Equal(a.RowLength, b.RowLength);
      Assert.Equal(a.ColumnLength, b.ColumnLength);
      Assert.Equal(a[0, 0], b[0, 0].Real);
      Assert.Equal(a[0, 1], b[0, 1].Real);
      Assert.Equal(a[1, 0], b[1, 0].Real);
      Assert.Equal(a[1, 1], b[1, 1].Real);
    }

    //Test implicit conversion from null floatmatrix.
    [Fact]
    public void ImplictToFloatMatrixNull()
    {
      FloatMatrix a = null;
      var b = ComplexFloatMatrix.ToComplexFloatMatrix(a);
      Assert.True(b is null);
    }

    //Test implicit conversion from ComplexFloat mult dim array.
    [Fact]
    public void ImplictComplexFloatMultArray()
    {
      var a = new ComplexFloat[2, 2];
      a[0, 0] = new ComplexFloat(1, 1);
      a[0, 1] = new ComplexFloat(2, 2);
      a[1, 0] = new ComplexFloat(3, 3);
      a[1, 1] = new ComplexFloat(4, 4);

      ComplexFloatMatrix b = a;
      Assert.Equal(a[0, 0], b[0, 0]);
      Assert.Equal(a[0, 1], b[0, 1]);
      Assert.Equal(a[1, 0], b[1, 0]);
      Assert.Equal(a[1, 1], b[1, 1]);
    }

    //Test implicit conversion from ComplexFloat mult dim array.
    [Fact]
    public void ImplictToComplexFloatMultArray()
    {
      var a = new ComplexFloat[2, 2];
      a[0, 0] = new ComplexFloat(1, 1);
      a[0, 1] = new ComplexFloat(2, 2);
      a[1, 0] = new ComplexFloat(3, 3);
      a[1, 1] = new ComplexFloat(4, 4);

      var b = ComplexFloatMatrix.ToComplexFloatMatrix(a);
      Assert.Equal(a[0, 0], b[0, 0]);
      Assert.Equal(a[0, 1], b[0, 1]);
      Assert.Equal(a[1, 0], b[1, 0]);
      Assert.Equal(a[1, 1], b[1, 1]);
    }

    //Test implicit conversion from null ComplexFloat mult dim array.
    [Fact]
    public void ImplictToComplexFloatMultArrayNull()
    {
      ComplexFloat[,] a = null;
      var b = ComplexFloatMatrix.ToComplexFloatMatrix(a);
      Assert.True(b is null);
    }

    //Test implicit conversion from float mult dim array.
    [Fact]
    public void ImplictToDoubleMultArray()
    {
      float[,] a = new float[2, 2];
      a[0, 0] = 1;
      a[0, 1] = 2;
      a[1, 0] = 3;
      a[1, 1] = 4;

      var b = ComplexFloatMatrix.ToComplexFloatMatrix(a);
      Assert.Equal(a[0, 0], b[0, 0].Real);
      Assert.Equal(a[0, 1], b[0, 1].Real);
      Assert.Equal(a[1, 0], b[1, 0].Real);
      Assert.Equal(a[1, 1], b[1, 1].Real);
    }

    //Test implicit conversion from null float mult dim array.
    [Fact]
    public void ImplictToDoubleMultArrayNull()
    {
      float[,] a = null;
      var b = ComplexFloatMatrix.ToComplexFloatMatrix(a);
      Assert.True(b is null);
    }

    //Test implicit conversion from float mult dim array.
    [Fact]
    public void ImplictToFloatMultArray()
    {
      float[,] a = new float[2, 2];
      a[0, 0] = 1;
      a[0, 1] = 2;
      a[1, 0] = 3;
      a[1, 1] = 4;

      var b = ComplexFloatMatrix.ToComplexFloatMatrix(a);
      Assert.Equal(a[0, 0], b[0, 0].Real);
      Assert.Equal(a[0, 1], b[0, 1].Real);
      Assert.Equal(a[1, 0], b[1, 0].Real);
      Assert.Equal(a[1, 1], b[1, 1].Real);
    }

    //Test implicit conversion from null float mult dim array.
    [Fact]
    public void ImplictToFloatMultArrayNull()
    {
      float[,] a = null;
      var b = ComplexFloatMatrix.ToComplexFloatMatrix(a);
      Assert.True(b is null);
    }

    //Test explicit conversion from ComplexDoubleMatrix
    [Fact]
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
      Assert.Equal(a.RowLength, b.RowLength);
      Assert.Equal(a.ColumnLength, b.ColumnLength);
      Assert.Equal(b[0, 0], a[0, 0]);
      Assert.Equal(b[0, 1], a[0, 1]);
      Assert.Equal(b[1, 0], a[1, 0]);
      Assert.Equal(b[1, 1], a[1, 1]);
    }

    //Test explicit conversion from ComplexDoubleMatrix
    [Fact]
    public void ExplicitComplexDoubleMatrixNull()
    {
      ComplexDoubleMatrix a = null;
      var b = (ComplexFloatMatrix)a;
      Assert.True(b is null);
    }

    //Test explicit conversion from ComplexDoubleMatrix
    [Fact]
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
      Assert.Equal(a.RowLength, b.RowLength);
      Assert.Equal(a.ColumnLength, b.ColumnLength);
      Assert.Equal(b[0, 0], a[0, 0]);
      Assert.Equal(b[0, 1], a[0, 1]);
      Assert.Equal(b[1, 0], a[1, 0]);
      Assert.Equal(b[1, 1], a[1, 1]);
    }

    //Test explicit conversion from DoubleMatrix
    [Fact]
    public void ExplicitToComplexDoubleMatrixNull()
    {
      DoubleMatrix a = null;
      var b = ComplexFloatMatrix.ToComplexFloatMatrix(a);
      Assert.True(b is null);
    }

    //Test explicit conversion from Complex array
    [Fact]
    public void ExplicitComplexDoubleMultArray()
    {
      var a = new Complex[2, 2];
      a[0, 0] = 1;
      a[0, 1] = 2;
      a[1, 0] = 3;
      a[1, 1] = 4;

      var b = (ComplexFloatMatrix)a;
      Assert.Equal(2, b.RowLength);
      Assert.Equal(2, b.ColumnLength);
      Assert.Equal(b[0, 0], a[0, 0]);
      Assert.Equal(b[0, 1], a[0, 1]);
      Assert.Equal(b[1, 0], a[1, 0]);
      Assert.Equal(b[1, 1], a[1, 1]);
    }

    //Test explicit conversion from Complex array
    [Fact]
    public void ExplicitComplexDoubleMultArrayNull()
    {
      Complex[,] a = null;
      var b = (ComplexFloatMatrix)a;
      Assert.True(b is null);
    }

    //Test explicit conversion from Complex array
    [Fact]
    public void ExplicitToComplexDoubleMultArray()
    {
      var a = new Complex[2, 2];
      a[0, 0] = 1;
      a[0, 1] = 2;
      a[1, 0] = 3;
      a[1, 1] = 4;

      var b = ComplexFloatMatrix.ToComplexFloatMatrix(a);
      Assert.Equal(2, b.RowLength);
      Assert.Equal(2, b.ColumnLength);
      Assert.Equal(a[0, 0], b[0, 0]);
      Assert.Equal(a[0, 1], b[0, 1]);
      Assert.Equal(a[1, 0], b[1, 0]);
      Assert.Equal(a[1, 1], b[1, 1]);
    }

    //Test explicit conversion from Complex array
    [Fact]
    public void ExplicitToComplexDoubleMultArrayNull()
    {
      Complex[,] a = null;
      var b = ComplexFloatMatrix.ToComplexFloatMatrix(a);
      Assert.True(b is null);
    }

    //Test explicit conversion from Jagged Complex array
    [Fact]
    public void ExplicitComplexDoubleJaggedArrayNull()
    {
      Complex[,] a = null;
      var b = (ComplexFloatMatrix)a;
      Assert.True(b is null);
    }

    //test equals method
    [Fact]
    public void TestEquals()
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

    //test GetHashCode
    [Fact]
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
      Assert.Equal(7, hash);
    }

    //test ToArray
    [Fact]
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

      Assert.Equal(a[0, 0], b[0, 0]);
      Assert.Equal(a[0, 1], b[0, 1]);
      Assert.Equal(a[1, 0], b[1, 0]);
      Assert.Equal(a[1, 1], b[1, 1]);
    }

    //test Transpose square
    [Fact]
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
      Assert.Equal(a[0, 0], new ComplexFloat(1));
      Assert.Equal(a[0, 1], new ComplexFloat(3));
      Assert.Equal(a[1, 0], new ComplexFloat(2));
      Assert.Equal(a[1, 1], new ComplexFloat(4));
    }

    //test Transpose wide
    [Fact]
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
      Assert.Equal(a[0, 0], new ComplexFloat(1));
      Assert.Equal(a[0, 1], new ComplexFloat(4));
      Assert.Equal(a[1, 0], new ComplexFloat(2));
      Assert.Equal(a[1, 1], new ComplexFloat(5));
      Assert.Equal(a[2, 0], new ComplexFloat(3));
      Assert.Equal(a[2, 1], new ComplexFloat(6));
      Assert.Equal(3, a.RowLength);
      Assert.Equal(2, a.ColumnLength);
    }

    //test Transpose long
    [Fact]
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
      Assert.Equal(a[0, 0], new ComplexFloat(1));
      Assert.Equal(a[0, 1], new ComplexFloat(3));
      Assert.Equal(a[0, 2], new ComplexFloat(5));
      Assert.Equal(a[1, 0], new ComplexFloat(2));
      Assert.Equal(a[1, 1], new ComplexFloat(4));
      Assert.Equal(a[1, 2], new ComplexFloat(6));
      Assert.Equal(2, a.RowLength);
      Assert.Equal(3, a.ColumnLength);
    }

    //test GetTranspose square
    [Fact]
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
      Assert.Equal(b[0, 0], a[0, 0]);
      Assert.Equal(b[0, 1], a[1, 0]);
      Assert.Equal(b[1, 0], a[0, 1]);
      Assert.Equal(b[1, 1], a[1, 1]);
    }

    //test GetTranspose wide
    [Fact]
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
      Assert.Equal(b[0, 0], a[0, 0]);
      Assert.Equal(b[0, 1], a[1, 0]);
      Assert.Equal(b[1, 0], a[0, 1]);
      Assert.Equal(b[1, 1], a[1, 1]);
      Assert.Equal(b[2, 0], a[0, 2]);
      Assert.Equal(b[2, 1], a[1, 2]);
      Assert.Equal(b.RowLength, a.ColumnLength);
      Assert.Equal(b.ColumnLength, a.RowLength);
    }

    //test GetTranspose long
    [Fact]
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
      Assert.Equal(b[0, 0], a[0, 0]);
      Assert.Equal(b[0, 1], a[1, 0]);
      Assert.Equal(b[0, 2], a[2, 0]);
      Assert.Equal(b[1, 0], a[0, 1]);
      Assert.Equal(b[1, 1], a[1, 1]);
      Assert.Equal(b[1, 2], a[2, 1]);
      Assert.Equal(b.RowLength, a.ColumnLength);
      Assert.Equal(b.ColumnLength, a.RowLength);
    }

    //test Invert
    [Fact]
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
      AssertEx.Equal(a[0, 0].Real, 3.500, TOLERANCE);
      AssertEx.Equal(a[0, 1].Real, -2.000, TOLERANCE);
      AssertEx.Equal(a[1, 0].Real, -1.500, TOLERANCE);
      AssertEx.Equal(a[1, 1].Real, 1.000, TOLERANCE);
    }

    //test Invert singular
    [Fact]
    public void InvertSingular()
    {
      Assert.Throws<SingularMatrixException>(() =>
      {
        var a = new ComplexFloatMatrix(2, 2);
        a.Invert();
      });
    }

    //test Invert not square
    [Fact]
    public void InvertNotSquare()
    {
      Assert.Throws<NotSquareMatrixException>(() =>
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
    [Fact]
    public void GetInverseSingular()
    {
      Assert.Throws<SingularMatrixException>(() =>
      {
        var a = new ComplexFloatMatrix(2, 2);
        ComplexFloatMatrix b = a.GetInverse();
      });
    }

    //test GetInverse not square
    [Fact]
    public void GetInverseNotSquare()
    {
      Assert.Throws<NotSquareMatrixException>(() =>
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
    [Fact]
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
      AssertEx.Equal(b[0, 0].Real, 3.500, TOLERANCE);
      AssertEx.Equal(b[0, 1].Real, -2.000, TOLERANCE);
      AssertEx.Equal(b[1, 0].Real, -1.500, TOLERANCE);
      AssertEx.Equal(b[1, 1].Real, 1.000, TOLERANCE);
    }

    //test GetDeterminant
    [Fact]
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
      Assert.Equal(b.Real, test.Real, 4);
      Assert.Equal(b.Imag, test.Imag, 4);
    }

    //test GetDeterminant
    [Fact]
    public void GetDeterminantNotSquare()
    {
      Assert.Throws<NotSquareMatrixException>(() =>
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
    [Fact]
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
      Assert.Equal(b[0], a[0, 0]);
      Assert.Equal(b[1], a[0, 1]);
    }

    //test GetRow
    [Fact]
    public void GetRowOutOfRange()
    {
      Assert.Throws<ArgumentOutOfRangeException>(() =>
      {
        var a = new ComplexFloatMatrix(2, 2);
        ComplexFloatVector b = a.GetRow(3);
      });
    }

    //test GetColumn
    [Fact]
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
      Assert.Equal(b[0], a[0, 0]);
      Assert.Equal(b[1], a[1, 0]);
    }

    //test GetColumn
    [Fact]
    public void GetColumnOutOfRange()
    {
      Assert.Throws<ArgumentOutOfRangeException>(() =>
      {
        var a = new ComplexFloatMatrix(2, 2);
        ComplexFloatVector b = a.GetColumn(3);
      });
    }

    //test GetDiagonal
    [Fact]
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
      Assert.Equal(b[0], a[0, 0]);
      Assert.Equal(b[1], a[1, 1]);
    }

    //test SetRow
    [Fact]
    public void SetRow()
    {
      var a = new ComplexFloatMatrix(2, 2);
      var b = new ComplexFloatVector(2)
      {
        [0] = new ComplexFloat(1, 1),
        [1] = new ComplexFloat(2, 2)
      };
      a.SetRow(0, b);
      Assert.Equal(b[0], a[0, 0]);
      Assert.Equal(b[1], a[0, 1]);
    }

    //test SetRow
    [Fact]
    public void SetRowOutOfRange()
    {
      Assert.Throws<ArgumentOutOfRangeException>(() =>
      {
        var a = new ComplexFloatMatrix(2, 2);
        var b = new ComplexFloatVector(2);
        a.SetRow(2, b);
      });
    }

    //test SetRow
    [Fact]
    public void SetRowWrongRank()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new ComplexFloatMatrix(2, 2);
        var b = new ComplexFloatVector(3);
        a.SetRow(1, b);
      });
    }

    //test SetRow
    [Fact]
    public void SetRowArray()
    {
      var a = new ComplexFloatMatrix(2, 2);
      var b = new ComplexFloat[2];
      b[0] = new ComplexFloat(1, 1);
      b[1] = new ComplexFloat(2, 2);

      a.SetRow(0, b);
      Assert.Equal(b[0], a[0, 0]);
      Assert.Equal(b[1], a[0, 1]);
    }

    //test SetRow
    [Fact]
    public void SetRowArrayOutOfRange()
    {
      Assert.Throws<ArgumentOutOfRangeException>(() =>
      {
        var a = new ComplexFloatMatrix(2, 2);
        var b = new ComplexFloat[2];
        a.SetRow(2, b);
      });
    }

    //test SetRow
    [Fact]
    public void SetRowArrayWrongRank()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new ComplexFloatMatrix(2, 2);
        var b = new ComplexFloat[3];
        a.SetRow(1, b);
      });
    }

    //test SetColumn
    [Fact]
    public void SetColumn()
    {
      var a = new ComplexFloatMatrix(2, 2);
      var b = new ComplexFloatVector(2)
      {
        [0] = 1,
        [1] = 2
      };
      a.SetColumn(0, b);
      Assert.Equal(b[0], a[0, 0]);
      Assert.Equal(b[1], a[1, 0]);
    }

    //test SetColumn
    [Fact]
    public void SetColumnOutOfRange()
    {
      Assert.Throws<ArgumentOutOfRangeException>(() =>
      {
        var a = new ComplexFloatMatrix(2, 2);
        var b = new ComplexFloatVector(2);
        a.SetColumn(2, b);
      });
    }

    //test SetColumn
    [Fact]
    public void SetColumnWrongRank()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new ComplexFloatMatrix(2, 2);
        var b = new ComplexFloatVector(3);
        a.SetColumn(1, b);
      });
    }

    //test SetColumn
    [Fact]
    public void SetColumnArray()
    {
      var a = new ComplexFloatMatrix(2, 2);
      var b = new ComplexFloat[2];
      b[0] = new ComplexFloat(1, 1);
      b[1] = new ComplexFloat(2, 2);
      a.SetColumn(0, b);
      Assert.Equal(b[0], a[0, 0]);
      Assert.Equal(b[1], a[1, 0]);
    }

    //test SetColumn
    [Fact]
    public void SetColumnArrayOutOfRange()
    {
      Assert.Throws<ArgumentOutOfRangeException>(() =>
      {
        var a = new ComplexFloatMatrix(2, 2);
        var b = new ComplexFloat[2];
        a.SetColumn(2, b);
      });
    }

    //test SetColumn
    [Fact]
    public void SetColumnArrayWrongRank()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new ComplexFloatMatrix(2, 2);
        var b = new ComplexFloat[3];
        a.SetColumn(1, b);
      });
    }

    //test SetDiagonal
    [Fact]
    public void SetDiagonal()
    {
      var a = new ComplexFloatMatrix(2, 2);
      var b = new ComplexFloatVector(2)
      {
        [0] = new ComplexFloat(1),
        [1] = new ComplexFloat(2)
      };
      a.SetDiagonal(b);
      Assert.Equal(b[0], a[0, 0]);
      Assert.Equal(b[1], a[1, 1]);
    }

    //test GetSubMatrix
    [Fact]
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
      Assert.Equal(2, b.RowLength);
      Assert.Equal(2, b.ColumnLength);
      Assert.Equal(3, c.RowLength);
      Assert.Equal(2, c.ColumnLength);
      Assert.Equal(b[0, 0], a[2, 2]);
      Assert.Equal(b[0, 1], a[2, 3]);
      Assert.Equal(b[1, 0], a[3, 2]);
      Assert.Equal(b[1, 1], a[3, 3]);
      Assert.Equal(c[0, 0], a[0, 1]);
      Assert.Equal(c[0, 1], a[0, 2]);
      Assert.Equal(c[1, 0], a[1, 1]);
      Assert.Equal(c[1, 1], a[1, 2]);
      Assert.Equal(c[2, 0], a[2, 1]);
      Assert.Equal(c[2, 1], a[2, 2]);
    }

    //test GetSubMatrix
    [Fact]
    public void GetSubMatrixOutRange1()
    {
      Assert.Throws<ArgumentOutOfRangeException>(() =>
      {
        var a = new ComplexFloatMatrix(4);
        ComplexFloatMatrix b = a.GetSubMatrix(-1, 2);
      });
    }

    //test GetSubMatrix
    [Fact]
    public void GetSubMatrixOutRange2()
    {
      Assert.Throws<ArgumentOutOfRangeException>(() =>
      {
        var a = new ComplexFloatMatrix(4);
        ComplexFloatMatrix b = a.GetSubMatrix(2, 4);
      });
    }

    //test GetSubMatrix
    [Fact]
    public void GetSubMatrixOutRange3()
    {
      Assert.Throws<ArgumentOutOfRangeException>(() =>
      {
        var a = new ComplexFloatMatrix(4);
        ComplexFloatMatrix b = a.GetSubMatrix(0, 0, 4, 2);
      });
    }

    //test GetSubMatrix
    [Fact]
    public void GetSubMatrixOutRange4()
    {
      Assert.Throws<ArgumentOutOfRangeException>(() =>
      {
        var a = new ComplexFloatMatrix(4);
        ComplexFloatMatrix b = a.GetSubMatrix(0, 0, 2, 4);
      });
    }

    //test GetSubMatrix
    [Fact]
    public void GetSubMatrixOutRange5()
    {
      Assert.Throws<ArgumentOutOfRangeException>(() =>
      {
        var a = new ComplexFloatMatrix(4);
        ComplexFloatMatrix b = a.GetSubMatrix(0, 3, 2, 2);
      });
    }

    //test GetUpperTriangle square matrix
    [Fact]
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

      Assert.Equal(b.RowLength, a.RowLength);
      Assert.Equal(b.ColumnLength, a.ColumnLength);
      Assert.Equal(b[0, 0], a[0, 0]);
      Assert.Equal(b[0, 1], a[0, 1]);
      Assert.Equal(b[0, 2], a[0, 2]);
      Assert.Equal(b[1, 0], ComplexFloat.Zero);
      Assert.Equal(b[1, 1], a[1, 1]);
      Assert.Equal(b[1, 2], a[1, 2]);
      Assert.Equal(b[2, 0], ComplexFloat.Zero);
      Assert.Equal(b[2, 1], ComplexFloat.Zero);
      Assert.Equal(b[2, 2], a[2, 2]);
    }

    //test GetUpperTriangle long matrix
    [Fact]
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

      Assert.Equal(b.RowLength, a.RowLength);
      Assert.Equal(b.ColumnLength, a.ColumnLength);
      Assert.Equal(b[0, 0], a[0, 0]);
      Assert.Equal(b[0, 1], a[0, 1]);
      Assert.Equal(b[1, 0], ComplexFloat.Zero);
      Assert.Equal(b[1, 1], a[1, 1]);
      Assert.Equal(b[2, 0], ComplexFloat.Zero);
      Assert.Equal(b[2, 1], ComplexFloat.Zero);
    }

    //test GetUpperTriangle wide matrix
    [Fact]
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

      Assert.Equal(b.RowLength, a.RowLength);
      Assert.Equal(b.ColumnLength, a.ColumnLength);
      Assert.Equal(b[0, 0], a[0, 0]);
      Assert.Equal(b[0, 1], a[0, 1]);
      Assert.Equal(b[0, 2], a[0, 2]);
      Assert.Equal(b[1, 0], ComplexFloat.Zero);
      Assert.Equal(b[1, 1], a[1, 1]);
      Assert.Equal(b[1, 2], a[1, 2]);
    }

    //test GetStrictlyUpperTriangle square matrix
    [Fact]
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

      Assert.Equal(b.RowLength, a.RowLength);
      Assert.Equal(b.ColumnLength, a.ColumnLength);
      Assert.Equal(b[0, 0], ComplexFloat.Zero);
      Assert.Equal(b[0, 1], a[0, 1]);
      Assert.Equal(b[0, 2], a[0, 2]);
      Assert.Equal(b[1, 0], ComplexFloat.Zero);
      Assert.Equal(b[1, 1], ComplexFloat.Zero);
      Assert.Equal(b[1, 2], a[1, 2]);
      Assert.Equal(b[2, 0], ComplexFloat.Zero);
      Assert.Equal(b[2, 1], ComplexFloat.Zero);
      Assert.Equal(b[2, 2], ComplexFloat.Zero);
    }

    //test GetStrictlyUpperTriangle long matrix
    [Fact]
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

      Assert.Equal(b.RowLength, a.RowLength);
      Assert.Equal(b.ColumnLength, a.ColumnLength);
      Assert.Equal(b[0, 0], ComplexFloat.Zero);
      Assert.Equal(b[0, 1], a[0, 1]);
      Assert.Equal(b[1, 0], ComplexFloat.Zero);
      Assert.Equal(b[1, 1], ComplexFloat.Zero);
      Assert.Equal(b[2, 0], ComplexFloat.Zero);
      Assert.Equal(b[2, 1], ComplexFloat.Zero);
    }

    //test GetStrictlyUpperTriangle wide matrix
    [Fact]
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

      Assert.Equal(b.RowLength, a.RowLength);
      Assert.Equal(b.ColumnLength, a.ColumnLength);
      Assert.Equal(b[0, 0], ComplexFloat.Zero);
      Assert.Equal(b[0, 1], a[0, 1]);
      Assert.Equal(b[0, 2], a[0, 2]);
      Assert.Equal(b[1, 0], ComplexFloat.Zero);
      Assert.Equal(b[1, 1], ComplexFloat.Zero);
      Assert.Equal(b[1, 2], a[1, 2]);
    }

    //test GetLowerTriangle square matrix
    [Fact]
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

      Assert.Equal(b.RowLength, a.RowLength);
      Assert.Equal(b.ColumnLength, a.ColumnLength);
      Assert.Equal(b[0, 0], a[0, 0]);
      Assert.Equal(b[0, 1], ComplexFloat.Zero);
      Assert.Equal(b[0, 2], ComplexFloat.Zero);
      Assert.Equal(b[1, 0], a[1, 0]);
      Assert.Equal(b[1, 1], a[1, 1]);
      Assert.Equal(b[1, 2], ComplexFloat.Zero);
      Assert.Equal(b[2, 0], a[2, 0]);
      Assert.Equal(b[2, 1], a[2, 1]);
      Assert.Equal(b[2, 2], a[2, 2]);
    }

    //test GetLowerTriangle long matrix
    [Fact]
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

      Assert.Equal(b.RowLength, a.RowLength);
      Assert.Equal(b.ColumnLength, a.ColumnLength);
      Assert.Equal(b[0, 0], a[0, 0]);
      Assert.Equal(b[0, 1], ComplexFloat.Zero);
      Assert.Equal(b[1, 0], b[1, 0]);
      Assert.Equal(b[1, 1], a[1, 1]);
      Assert.Equal(b[2, 0], b[2, 0]);
      Assert.Equal(b[2, 1], b[2, 1]);
    }

    //test GetLowerTriangle wide matrix
    [Fact]
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

      Assert.Equal(b.RowLength, a.RowLength);
      Assert.Equal(b.ColumnLength, a.ColumnLength);
      Assert.Equal(b[0, 0], a[0, 0]);
      Assert.Equal(b[0, 1], ComplexFloat.Zero);
      Assert.Equal(b[0, 2], ComplexFloat.Zero);
      Assert.Equal(b[1, 0], a[1, 0]);
      Assert.Equal(b[1, 1], a[1, 1]);
      Assert.Equal(b[1, 2], ComplexFloat.Zero);
    }

    //test GetStrictlyLowerTriangle square matrix
    [Fact]
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

      Assert.Equal(b.RowLength, a.RowLength);
      Assert.Equal(b.ColumnLength, a.ColumnLength);
      Assert.Equal(b[0, 0], ComplexFloat.Zero);
      Assert.Equal(b[0, 1], ComplexFloat.Zero);
      Assert.Equal(b[0, 2], ComplexFloat.Zero);
      Assert.Equal(b[1, 0], a[1, 0]);
      Assert.Equal(b[1, 1], ComplexFloat.Zero);
      Assert.Equal(b[1, 2], ComplexFloat.Zero);
      Assert.Equal(b[2, 0], a[2, 0]);
      Assert.Equal(b[2, 1], a[2, 1]);
      Assert.Equal(b[2, 2], ComplexFloat.Zero);
    }

    //test GetStrictlyLowerTriangle long matrix
    [Fact]
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

      Assert.Equal(b.RowLength, a.RowLength);
      Assert.Equal(b.ColumnLength, a.ColumnLength);
      Assert.Equal(b[0, 0], ComplexFloat.Zero);
      Assert.Equal(b[0, 1], ComplexFloat.Zero);
      Assert.Equal(b[1, 0], b[1, 0]);
      Assert.Equal(b[1, 1], ComplexFloat.Zero);
      Assert.Equal(b[2, 0], b[2, 0]);
      Assert.Equal(b[2, 1], b[2, 1]);
    }

    //test GetStrictlyLowerTriangle wide matrix
    [Fact]
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

      Assert.Equal(b.RowLength, a.RowLength);
      Assert.Equal(b.ColumnLength, a.ColumnLength);
      Assert.Equal(b[0, 0], ComplexFloat.Zero);
      Assert.Equal(b[0, 1], ComplexFloat.Zero);
      Assert.Equal(b[0, 2], ComplexFloat.Zero);
      Assert.Equal(b[1, 0], a[1, 0]);
      Assert.Equal(b[1, 1], ComplexFloat.Zero);
      Assert.Equal(b[1, 2], ComplexFloat.Zero);
    }

    //static Negate
    [Fact]
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
      Assert.Equal(b[0, 0], new ComplexFloat(-1));
      Assert.Equal(b[0, 1], new ComplexFloat(-2));
      Assert.Equal(b[1, 0], new ComplexFloat(-3));
      Assert.Equal(b[1, 1], new ComplexFloat(-4));
    }

    //static NegateNull
    [Fact]
    public void NegateNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        ComplexFloatMatrix a = null;
        var b = ComplexFloatMatrix.Negate(a);
      });
    }

    //static operator -
    [Fact]
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
      Assert.Equal(b[0, 0], new ComplexFloat(-1));
      Assert.Equal(b[0, 1], new ComplexFloat(-2));
      Assert.Equal(b[1, 0], new ComplexFloat(-3));
      Assert.Equal(b[1, 1], new ComplexFloat(-4));
    }

    //static operator - null
    [Fact]
    public void OperatorMinusNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        ComplexFloatMatrix a = null;
        ComplexFloatMatrix b = -a;
      });
    }

    //static subtact two square matrices
    [Fact]
    public void StaticSubtract()
    {
      var a = new ComplexFloatMatrix(2);
      var b = new ComplexFloatMatrix(2);
      a[0, 0] = b[0, 0] = new ComplexFloat(1);
      a[0, 1] = b[0, 1] = new ComplexFloat(2);
      a[1, 0] = b[1, 0] = new ComplexFloat(3);
      a[1, 1] = b[1, 1] = new ComplexFloat(4);
      var c = ComplexFloatMatrix.Subtract(a, b);
      Assert.Equal(c[0, 0], ComplexFloat.Zero);
      Assert.Equal(c[0, 1], ComplexFloat.Zero);
      Assert.Equal(c[1, 0], ComplexFloat.Zero);
      Assert.Equal(c[1, 1], ComplexFloat.Zero);
    }

    //operator subtract two square matrices
    [Fact]
    public void OperatorSubtract()
    {
      var a = new ComplexFloatMatrix(2);
      var b = new ComplexFloatMatrix(2);
      a[0, 0] = b[0, 0] = new ComplexFloat(1);
      a[0, 1] = b[0, 1] = new ComplexFloat(2);
      a[1, 0] = b[1, 0] = new ComplexFloat(3);
      a[1, 1] = b[1, 1] = new ComplexFloat(4);
      ComplexFloatMatrix c = a - b;
      Assert.Equal(c[0, 0], ComplexFloat.Zero);
      Assert.Equal(c[0, 1], ComplexFloat.Zero);
      Assert.Equal(c[1, 0], ComplexFloat.Zero);
      Assert.Equal(c[1, 1], ComplexFloat.Zero);
    }

    //member add subtract square matrices
    [Fact]
    public void MemberSubtract()
    {
      var a = new ComplexFloatMatrix(2);
      var b = new ComplexFloatMatrix(2);
      a[0, 0] = b[0, 0] = new ComplexFloat(1);
      a[0, 1] = b[0, 1] = new ComplexFloat(2);
      a[1, 0] = b[1, 0] = new ComplexFloat(3);
      a[1, 1] = b[1, 1] = new ComplexFloat(4);
      a.Subtract(b);
      Assert.Equal(a[0, 0], ComplexFloat.Zero);
      Assert.Equal(a[0, 1], ComplexFloat.Zero);
      Assert.Equal(a[1, 0], ComplexFloat.Zero);
      Assert.Equal(a[1, 1], ComplexFloat.Zero);
    }

    //static Subtract two square matrices, one null
    [Fact]
    public void StaticSubtractNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        var a = new ComplexFloatMatrix(2);
        ComplexFloatMatrix b = null;
        var c = ComplexFloatMatrix.Subtract(a, b);
      });
    }

    //operator Subtract two square matrices, one null
    [Fact]
    public void OperatorSubtractNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        var a = new ComplexFloatMatrix(2);
        ComplexFloatMatrix b = null;
        ComplexFloatMatrix c = a - b;
      });
    }

    //member Subtract two square matrices, one null
    [Fact]
    public void MemberSubtractNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        var a = new ComplexFloatMatrix(2);
        ComplexFloatMatrix b = null;
        a.Subtract(b);
      });
    }

    //static Subtract two incompatible matrices
    [Fact]
    public void StaticSubtractIncompatible()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new ComplexFloatMatrix(2);
        var b = new ComplexFloatMatrix(3);
        var c = ComplexFloatMatrix.Subtract(a, b);
      });
    }

    //operator Subtract two  incompatible matrices
    [Fact]
    public void OperatorSubtractIncompatible()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new ComplexFloatMatrix(2);
        var b = new ComplexFloatMatrix(3);
        ComplexFloatMatrix c = a - b;
      });
    }

    //member Subtract two  incompatible matricess
    [Fact]
    public void MemberSubtractIncompatible()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new ComplexFloatMatrix(2);
        var b = new ComplexFloatMatrix(3);
        a.Subtract(b);
      });
    }

    //static add two square matrices
    [Fact]
    public void StaticAdd()
    {
      var a = new ComplexFloatMatrix(2);
      var b = new ComplexFloatMatrix(2);
      a[0, 0] = b[0, 0] = new ComplexFloat(1);
      a[0, 1] = b[0, 1] = new ComplexFloat(2);
      a[1, 0] = b[1, 0] = new ComplexFloat(3);
      a[1, 1] = b[1, 1] = new ComplexFloat(4);
      var c = ComplexFloatMatrix.Add(a, b);
      Assert.Equal(c[0, 0], new ComplexFloat(2));
      Assert.Equal(c[0, 1], new ComplexFloat(4));
      Assert.Equal(c[1, 0], new ComplexFloat(6));
      Assert.Equal(c[1, 1], new ComplexFloat(8));
    }

    //operator add two square matrices
    [Fact]
    public void OperatorAdd()
    {
      var a = new ComplexFloatMatrix(2);
      var b = new ComplexFloatMatrix(2);
      a[0, 0] = b[0, 0] = new ComplexFloat(1);
      a[0, 1] = b[0, 1] = new ComplexFloat(2);
      a[1, 0] = b[1, 0] = new ComplexFloat(3);
      a[1, 1] = b[1, 1] = new ComplexFloat(4);
      ComplexFloatMatrix c = a + b;
      Assert.Equal(c[0, 0], new ComplexFloat(2));
      Assert.Equal(c[0, 1], new ComplexFloat(4));
      Assert.Equal(c[1, 0], new ComplexFloat(6));
      Assert.Equal(c[1, 1], new ComplexFloat(8));
    }

    //member add two square matrices
    [Fact]
    public void MemberAdd()
    {
      var a = new ComplexFloatMatrix(2);
      var b = new ComplexFloatMatrix(2);
      a[0, 0] = b[0, 0] = new ComplexFloat(1);
      a[0, 1] = b[0, 1] = new ComplexFloat(2);
      a[1, 0] = b[1, 0] = new ComplexFloat(3);
      a[1, 1] = b[1, 1] = new ComplexFloat(4);
      a.Add(b);
      Assert.Equal(a[0, 0], new ComplexFloat(2));
      Assert.Equal(a[0, 1], new ComplexFloat(4));
      Assert.Equal(a[1, 0], new ComplexFloat(6));
      Assert.Equal(a[1, 1], new ComplexFloat(8));
    }

    //static add two square matrices, one null
    [Fact]
    public void StaticAddNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        var a = new ComplexFloatMatrix(2);
        ComplexFloatMatrix b = null;
        var c = ComplexFloatMatrix.Add(a, b);
      });
    }

    //operator add two square matrices, one null
    [Fact]
    public void OperatorAddNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        var a = new ComplexFloatMatrix(2);
        ComplexFloatMatrix b = null;
        ComplexFloatMatrix c = a + b;
      });
    }

    //member add two square matrices, one null
    [Fact]
    public void MemberAddNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        var a = new ComplexFloatMatrix(2);
        ComplexFloatMatrix b = null;
        a.Add(b);
      });
    }

    //static add two incompatible matrices
    [Fact]
    public void StaticAddIncompatible()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new ComplexFloatMatrix(2);
        var b = new ComplexFloatMatrix(3);
        var c = ComplexFloatMatrix.Add(a, b);
      });
    }

    //operator add two  incompatible matrices
    [Fact]
    public void OperatorAddIncompatible()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new ComplexFloatMatrix(2);
        var b = new ComplexFloatMatrix(3);
        ComplexFloatMatrix c = a + b;
      });
    }

    //member add two  incompatible matricess
    [Fact]
    public void MemberAddIncompatible()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new ComplexFloatMatrix(2);
        var b = new ComplexFloatMatrix(3);
        a.Add(b);
      });
    }

    //static divide matrix by float
    [Fact]
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
      Assert.Equal(b[0, 0], new ComplexFloat(1));
      Assert.Equal(b[0, 1], new ComplexFloat(2));
      Assert.Equal(b[1, 0], new ComplexFloat(3));
      Assert.Equal(b[1, 1], new ComplexFloat(4));
    }

    //operator divide matrix by float
    [Fact]
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
      Assert.Equal(b[0, 0], new ComplexFloat(1));
      Assert.Equal(b[0, 1], new ComplexFloat(2));
      Assert.Equal(b[1, 0], new ComplexFloat(3));
      Assert.Equal(b[1, 1], new ComplexFloat(4));
    }

    //member divide matrix by float
    [Fact]
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
      Assert.Equal(a[0, 0], new ComplexFloat(1));
      Assert.Equal(a[0, 1], new ComplexFloat(2));
      Assert.Equal(a[1, 0], new ComplexFloat(3));
      Assert.Equal(a[1, 1], new ComplexFloat(4));
    }

    //static divide null matrix by float
    [Fact]
    public void StaticDivideNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        ComplexFloatMatrix a = null;
        var b = ComplexFloatMatrix.Divide(a, 2);
      });
    }

    //operator divide null matrix by float
    [Fact]
    public void OperatorDivideNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        ComplexFloatMatrix a = null;
        ComplexFloatMatrix b = a / 2;
      });
    }

    //copy
    [Fact]
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
      Assert.Equal(a[0, 0], a[0, 0]);
      Assert.Equal(a[0, 1], b[0, 1]);
      Assert.Equal(a[1, 0], b[1, 0]);
      Assert.Equal(a[1, 1], b[1, 1]);
    }

    //test multiply float matrix operator *
    [Fact]
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
      Assert.Equal(b[0, 0], new ComplexFloat(2));
      Assert.Equal(b[0, 1], new ComplexFloat(4));
      Assert.Equal(b[1, 0], new ComplexFloat(6));
      Assert.Equal(b[1, 1], new ComplexFloat(8));
    }

    //test multiply float null matrix operator *
    [Fact]
    public void OperatorMultiplyComplexFloatMatrixNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        ComplexFloatMatrix a = null;
        ComplexFloatMatrix b = 2.0f * a;
      });
    }

    //test multiply  matrix float operator *
    [Fact]
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
      Assert.Equal(b[0, 0], new ComplexFloat(2));
      Assert.Equal(b[0, 1], new ComplexFloat(4));
      Assert.Equal(b[1, 0], new ComplexFloat(6));
      Assert.Equal(b[1, 1], new ComplexFloat(8));
    }

    //test multiply  null matrix float operator *
    [Fact]
    public void OperatorMultiplyMatrixComplexFloatNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        ComplexFloatMatrix a = null;
        ComplexFloatMatrix b = a * 2;
      });
    }

    //test static multiply float matrix
    [Fact]
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
      Assert.Equal(b[0, 0], new ComplexFloat(2));
      Assert.Equal(b[0, 1], new ComplexFloat(4));
      Assert.Equal(b[1, 0], new ComplexFloat(6));
      Assert.Equal(b[1, 1], new ComplexFloat(8));
    }

    //test static multiply float null matrix
    [Fact]
    public void StaticMultiplyComplexFloatMatrixNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        ComplexFloatMatrix a = null;
        var b = ComplexFloatMatrix.Multiply(2.0f, a);
      });
    }

    //test static multiply  matrix float
    [Fact]
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

      Assert.Equal(b[0, 0], new ComplexFloat(2));
      Assert.Equal(b[0, 1], new ComplexFloat(4));
      Assert.Equal(b[1, 0], new ComplexFloat(6));
      Assert.Equal(b[1, 1], new ComplexFloat(8));
    }

    //test static multiply  null matrix float operator *
    [Fact]
    public void StaticMultiplyMatrixComplexFloatNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        ComplexFloatMatrix a = null;
        var b = ComplexFloatMatrix.Multiply(a, 2.0f);
      });
    }

    //test member multiply  float
    [Fact]
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
      Assert.Equal(a[0, 0], new ComplexFloat(2));
      Assert.Equal(a[0, 1], new ComplexFloat(4));
      Assert.Equal(a[1, 0], new ComplexFloat(6));
      Assert.Equal(a[1, 1], new ComplexFloat(8));
    }

    //test multiply  matrix vector operator *
    [Fact]
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
      Assert.Equal(c[0], new ComplexFloat(6));
      Assert.Equal(c[1], new ComplexFloat(14));
    }

    //test multiply  matrix nonconform vector operator *
    [Fact]
    public void OperatorMultiplyMatrixNonConformVector()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new ComplexFloatMatrix(2);
        var b = new ComplexFloatVector(3, 2.0f);
        ComplexFloatVector c = a * b;
      });
    }

    //test multiply null matrix vector operator *
    [Fact]
    public void OperatorMultiplyNullMatrixVector()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        ComplexFloatMatrix a = null;
        var b = new ComplexFloatVector(2, 2.0f);
        ComplexFloatVector c = a * b;
      });
    }

    //test multiply matrix null vector operator *
    [Fact]
    public void OperatorMultiplyMatrixNullVector()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        var a = new ComplexFloatMatrix(2);
        ComplexFloatVector b = null;
        ComplexFloatVector c = a * b;
      });
    }

    //test static multiply  matrix vector
    [Fact]
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
      Assert.Equal(c[0], new ComplexFloat(6));
      Assert.Equal(c[1], new ComplexFloat(14));
    }

    //test static multiply  matrix nonconform vector
    [Fact]
    public void StaticMultiplyMatrixNonConformVector()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new ComplexFloatMatrix(2);
        var b = new ComplexFloatVector(3, 2.0f);
        ComplexFloatVector c = a * b;
      });
    }

    //test static multiply null matrix vector
    [Fact]
    public void StaticMultiplyNullMatrixVector()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        ComplexFloatMatrix a = null;
        var b = new ComplexFloatVector(2, 2.0f);
        ComplexFloatVector c = ComplexFloatMatrix.Multiply(a, b);
      });
    }

    //test static multiply matrix null vector
    [Fact]
    public void StaticMultiplyMatrixNullVector()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        var a = new ComplexFloatMatrix(2);
        ComplexFloatVector b = null;
        ComplexFloatVector c = ComplexFloatMatrix.Multiply(a, b);
      });
    }

    //test member multiply vector
    [Fact]
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
      Assert.Equal(a[0, 0], new ComplexFloat(6));
      Assert.Equal(a[1, 0], new ComplexFloat(14));
      Assert.Equal(1, a.ColumnLength);
      Assert.Equal(2, a.RowLength);
    }

    //test member multiply  matrix nonconform vector
    [Fact]
    public void MemberMultiplyMatrixNonConformVector()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new ComplexFloatMatrix(2);
        var b = new ComplexFloatVector(3, 2.0f);
        a.Multiply(b);
      });
    }

    //test member multiply null vector
    [Fact]
    public void MemberMultiplyNullVector()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        var a = new ComplexFloatMatrix(2);
        ComplexFloatVector b = null;
        a.Multiply(b);
      });
    }

    //test multiply  matrix matrix operator *
    [Fact]
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
      Assert.Equal(c[0, 0], new ComplexFloat(6));
      Assert.Equal(c[0, 1], new ComplexFloat(6));
      Assert.Equal(c[1, 0], new ComplexFloat(14));
      Assert.Equal(c[1, 1], new ComplexFloat(14));
    }

    //test multiply  nonconform matrix matrix operator *
    [Fact]
    public void OperatorMultiplyMatrixNonConformMatrix()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new ComplexFloatMatrix(2);
        var b = new ComplexFloatMatrix(3, 2, 2.0f);
        ComplexFloatMatrix c = a * b;
      });
    }

    //test multiply  long matrix wide matrix operator *
    [Fact]
    public void OperatorMultiplyLongMatrixWideMatrix()
    {
      var a = new ComplexFloatMatrix(3, 2, 1);
      var b = new ComplexFloatMatrix(2, 3, 2);
      ComplexFloatMatrix c = a * b;
      Assert.Equal(c[0, 0], new ComplexFloat(4));
      Assert.Equal(c[0, 1], new ComplexFloat(4));
      Assert.Equal(c[0, 1], new ComplexFloat(4));
      Assert.Equal(c[1, 0], new ComplexFloat(4));
      Assert.Equal(c[1, 1], new ComplexFloat(4));
      Assert.Equal(c[1, 2], new ComplexFloat(4));
      Assert.Equal(c[2, 0], new ComplexFloat(4));
      Assert.Equal(c[2, 1], new ComplexFloat(4));
      Assert.Equal(c[2, 2], new ComplexFloat(4));
    }

    //test multiply  wide matrix long matrix operator *
    [Fact]
    public void OperatorMultiplyWideMatrixLongMatrix()
    {
      var a = new ComplexFloatMatrix(2, 3, 1);
      var b = new ComplexFloatMatrix(3, 2, 2);
      ComplexFloatMatrix c = a * b;
      Assert.Equal(c[0, 0], new ComplexFloat(6));
      Assert.Equal(c[0, 1], new ComplexFloat(6));
      Assert.Equal(c[1, 0], new ComplexFloat(6));
      Assert.Equal(c[1, 1], new ComplexFloat(6));
    }

    //test multiply null matrix matrix operator *
    [Fact]
    public void OperatorMultiplyNullMatrixMatrix()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        ComplexFloatMatrix a = null;
        var b = new ComplexFloatMatrix(2, 2.0f);
        ComplexFloatMatrix c = a * b;
      });
    }

    //test multiply matrix null matrix operator *
    [Fact]
    public void OperatorMultiplyMatrixNullMatrix()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        var a = new ComplexFloatMatrix(2, 2.0f);
        ComplexFloatMatrix b = null;
        ComplexFloatMatrix c = a * b;
      });
    }

    //test static multiply  matrix matrix
    [Fact]
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
      Assert.Equal(c[0, 0], new ComplexFloat(6));
      Assert.Equal(c[0, 1], new ComplexFloat(6));
      Assert.Equal(c[1, 0], new ComplexFloat(14));
      Assert.Equal(c[1, 1], new ComplexFloat(14));
    }

    //test static multiply nonconform matrix matrix
    [Fact]
    public void StaticMultiplyMatrixNonConformMatrix()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new ComplexFloatMatrix(2);
        var b = new ComplexFloatMatrix(3, 2, 2.0f);
        var c = ComplexFloatMatrix.Multiply(a, b);
      });
    }

    //test static multiply  long matrix wide matrix
    [Fact]
    public void StaticMultiplyLongMatrixWideMatrix()
    {
      var a = new ComplexFloatMatrix(3, 2, 1);
      var b = new ComplexFloatMatrix(2, 3, 2);
      var c = ComplexFloatMatrix.Multiply(a, b);
      Assert.Equal(c[0, 0], new ComplexFloat(4));
      Assert.Equal(c[0, 1], new ComplexFloat(4));
      Assert.Equal(c[0, 1], new ComplexFloat(4));
      Assert.Equal(c[1, 0], new ComplexFloat(4));
      Assert.Equal(c[1, 1], new ComplexFloat(4));
      Assert.Equal(c[1, 2], new ComplexFloat(4));
      Assert.Equal(c[2, 0], new ComplexFloat(4));
      Assert.Equal(c[2, 1], new ComplexFloat(4));
      Assert.Equal(c[2, 2], new ComplexFloat(4));
    }

    //test static multiply  wide matrix long matrix
    [Fact]
    public void StaticMultiplyWideMatrixLongMatrix()
    {
      var a = new ComplexFloatMatrix(2, 3, 1);
      var b = new ComplexFloatMatrix(3, 2, 2);
      var c = ComplexFloatMatrix.Multiply(a, b);
      Assert.Equal(c[0, 0], new ComplexFloat(6));
      Assert.Equal(c[0, 1], new ComplexFloat(6));
      Assert.Equal(c[1, 0], new ComplexFloat(6));
      Assert.Equal(c[1, 1], new ComplexFloat(6));
    }

    //test static multiply null matrix matrix
    [Fact]
    public void StaticMultiplyNullMatrixMatrix()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        ComplexFloatMatrix a = null;
        var b = new ComplexFloatMatrix(2, 2.0f);
        var c = ComplexFloatMatrix.Multiply(a, b);
      });
    }

    //test static multiply matrix null matrix
    [Fact]
    public void StaticMultiplyMatrixNullMatrix()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        var a = new ComplexFloatMatrix(2, 2.0f);
        ComplexFloatMatrix b = null;
        var c = ComplexFloatMatrix.Multiply(a, b);
      });
    }

    //test member multiply  matrix matrix
    [Fact]
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
      Assert.Equal(a[0, 0], new ComplexFloat(6));
      Assert.Equal(a[0, 1], new ComplexFloat(6));
      Assert.Equal(a[1, 0], new ComplexFloat(14));
      Assert.Equal(a[1, 1], new ComplexFloat(14));
    }

    //test member multiply nonconform matrix matrix
    [Fact]
    public void MemberMultiplyMatrixNonConformMatrix()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new ComplexFloatMatrix(2);
        var b = new ComplexFloatMatrix(3, 2, 2.0f);
        a.Multiply(b);
      });
    }

    //test member multiply  long matrix wide matrix
    [Fact]
    public void MemberMultiplyLongMatrixWideMatrix()
    {
      var a = new ComplexFloatMatrix(3, 2, 1);
      var b = new ComplexFloatMatrix(2, 3, 2);
      a.Multiply(b);
      Assert.Equal(a[0, 0], new ComplexFloat(4));
      Assert.Equal(a[0, 1], new ComplexFloat(4));
      Assert.Equal(a[0, 1], new ComplexFloat(4));
      Assert.Equal(a[1, 0], new ComplexFloat(4));
      Assert.Equal(a[1, 1], new ComplexFloat(4));
      Assert.Equal(a[1, 2], new ComplexFloat(4));
      Assert.Equal(a[2, 0], new ComplexFloat(4));
      Assert.Equal(a[2, 1], new ComplexFloat(4));
      Assert.Equal(a[2, 2], new ComplexFloat(4));
    }

    //test member multiply  wide matrix long matrix
    [Fact]
    public void MemberMultiplyWideMatrixLongMatrix()
    {
      var a = new ComplexFloatMatrix(2, 3, 1);
      var b = new ComplexFloatMatrix(3, 2, 2);
      a.Multiply(b);
      Assert.Equal(a[0, 0], new ComplexFloat(6));
      Assert.Equal(a[0, 1], new ComplexFloat(6));
      Assert.Equal(a[1, 0], new ComplexFloat(6));
      Assert.Equal(a[1, 1], new ComplexFloat(6));
    }

    //test member multiply null matrix matrix
    [Fact]
    public void MemberMultiplyNullMatrixMatrix()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        var a = new ComplexFloatMatrix(2, 2.0f);
        ComplexFloatMatrix b = null;
        a.Multiply(b);
      });
    }

    //copy null
    [Fact]
    public void CopyNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        ComplexFloatMatrix a = null;
        var b = new ComplexFloatMatrix(2);
        b.Copy(a);
      });
    }

    //Norm
    [Fact]
    public void Norms()
    {
      var a = new ComplexFloatMatrix(2)
      {
        [0, 0] = new ComplexFloat(1.1f, 1.1f),
        [0, 1] = new ComplexFloat(2.2f, -2.2f),
        [1, 0] = new ComplexFloat(3.3f, 3.3f),
        [1, 1] = new ComplexFloat(4.4f, -4.4f)
      };
      AssertEx.Equal(a.GetL1Norm(), 9.334, TOLERANCE);
      AssertEx.Equal(a.GetL2Norm(), 8.502, TOLERANCE);
      AssertEx.Equal(a.GetInfinityNorm(), 10.889, TOLERANCE);
      AssertEx.Equal(a.GetFrobeniusNorm(), 8.521, TOLERANCE);
    }

    //Wide Norm
    [Fact]
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
      AssertEx.Equal(a.GetL1Norm(), 14.001, TOLERANCE);
      AssertEx.Equal(a.GetL2Norm(), 13.845, TOLERANCE);
      AssertEx.Equal(a.GetInfinityNorm(), 23.335, TOLERANCE);
      AssertEx.Equal(a.GetFrobeniusNorm(), 14.840, TOLERANCE);
    }

    //Long Norm
    [Fact]
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
      AssertEx.Equal(a.GetL1Norm(), 18.668, TOLERANCE);
      AssertEx.Equal(a.GetL2Norm(), 14.818, TOLERANCE);
      AssertEx.Equal(a.GetInfinityNorm(), 17.112, TOLERANCE);
      AssertEx.Equal(a.GetFrobeniusNorm(), 14.840, TOLERANCE);
    }

    //Condition
    [Fact]
    public void Condition()
    {
      var a = new ComplexFloatMatrix(2)
      {
        [0, 0] = new ComplexFloat(1.1f, 1.1f),
        [0, 1] = new ComplexFloat(2.2f, -2.2f),
        [1, 0] = new ComplexFloat(3.3f, 3.3f),
        [1, 1] = new ComplexFloat(4.4f, -4.4f)
      };
      AssertEx.Equal(a.GetConditionNumber(), 14.933, TOLERANCE);
    }

    //Wide Condition
    [Fact]
    public void WideCondition()
    {
      Assert.Throws<NotSquareMatrixException>(() =>
      {
        var a = new ComplexFloatMatrix(2, 3);
        a.GetConditionNumber();
      });
    }

    //Long Condition
    [Fact]
    public void LongCondition()
    {
      Assert.Throws<NotSquareMatrixException>(() =>
      {
        var a = new ComplexFloatMatrix(3, 2);
        a.GetConditionNumber();
      });
    }

    //clone
    [Fact]
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
      Assert.Equal(a[0, 0], a[0, 0]);
      Assert.Equal(a[0, 1], b[0, 1]);
      Assert.Equal(a[1, 0], b[1, 0]);
      Assert.Equal(a[1, 1], b[1, 1]);
    }
  }
}
