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

  public class DoubleMatrixTest
  {
    private const double TOLERANCE = 0.001;
    private const double DBL_EPSILON = DoubleConstants.DBL_EPSILON;

    //Test dimensions Constructor.
    [Fact]
    public void CtorDimensions()
    {
      var test = new DoubleMatrix(2, 2);

      Assert.Equal(2, test.RowLength);
      Assert.Equal(2, test.ColumnLength);
      Assert.Equal(0, test[0, 0]);
      Assert.Equal(0, test[0, 1]);
      Assert.Equal(0, test[1, 0]);
      Assert.Equal(0, test[1, 1]);
    }

    //Test Intital Values Constructor.
    [Fact]
    public void CtorInitialValues()
    {
      var test = new DoubleMatrix(2, 2, 1);

      Assert.Equal(2, test.RowLength);
      Assert.Equal(2, test.ColumnLength);
      Assert.Equal(1, test[0, 0]);
      Assert.Equal(1, test[0, 1]);
      Assert.Equal(1, test[1, 0]);
      Assert.Equal(1, test[1, 1]);
    }

    //Test Copy Constructor.
    [Fact]
    public void CtorCopy()
    {
      var a = new DoubleMatrix(2, 2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };

      var b = new DoubleMatrix(a);

      Assert.Equal(a.RowLength, b.RowLength);
      Assert.Equal(a.ColumnLength, b.ColumnLength);
      Assert.Equal(a[0, 0], a[0, 0]);
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
        DoubleMatrix a = null;
        var b = new DoubleMatrix(a);
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

      var b = new DoubleMatrix(a);
      Assert.Equal(a.RowLength, b.RowLength);
      Assert.Equal(a.ColumnLength, b.ColumnLength);
      Assert.Equal(a[0, 0], a[0, 0]);
      Assert.Equal(a[0, 1], b[0, 1]);
      Assert.Equal(a[1, 0], b[1, 0]);
      Assert.Equal(a[1, 1], b[1, 1]);
    }

    //Test Copy Constructor.
    [Fact]
    public void CtorCopyFloatNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        FloatMatrix a = null;
        var b = new DoubleMatrix(a);
      });
    }

    //Test Multiple Dimensional DoubleArray Constructor with Square array.
    [Fact]
    public void CtorMultDimDoubleSquare()
    {
      double[,] values = new double[2, 2];

      values[0, 0] = 0;
      values[0, 1] = 1;
      values[1, 0] = 3;
      values[1, 1] = 4;

      var test = new DoubleMatrix(values);

      Assert.Equal(2, test.RowLength);
      Assert.Equal(2, test.ColumnLength);
      Assert.Equal(test[0, 0], values[0, 0]);
      Assert.Equal(test[0, 1], values[0, 1]);
      Assert.Equal(test[1, 0], values[1, 0]);
      Assert.Equal(test[1, 1], values[1, 1]);
    }

    //Test Multiple Dimensional DoubleArray Constructor with wide array.
    [Fact]
    public void CtorMultDimDoubleWide()
    {
      double[,] values = new double[2, 3];

      values[0, 0] = 0;
      values[0, 1] = 1;
      values[0, 2] = 2;
      values[1, 0] = 3;
      values[1, 1] = 4;
      values[1, 2] = 5;

      var test = new DoubleMatrix(values);

      Assert.Equal(2, test.RowLength);
      Assert.Equal(3, test.ColumnLength);
      Assert.Equal(test[0, 0], values[0, 0]);
      Assert.Equal(test[0, 1], values[0, 1]);
      Assert.Equal(test[0, 2], values[0, 2]);
      Assert.Equal(test[1, 0], values[1, 0]);
      Assert.Equal(test[1, 1], values[1, 1]);
      Assert.Equal(test[1, 2], values[1, 2]);
    }

    //Test Multiple Dimensional DoubleArray Constructor with long array.
    [Fact]
    public void CtorMultDimDoubleLong()
    {
      double[,] values = new double[3, 2];

      values[0, 0] = 0;
      values[0, 1] = 1;
      values[1, 0] = 3;
      values[1, 1] = 4;
      values[2, 0] = 6;
      values[2, 1] = 7;

      var test = new DoubleMatrix(values);

      Assert.Equal(3, test.RowLength);
      Assert.Equal(2, test.ColumnLength);
      Assert.Equal(test[0, 0], values[0, 0]);
      Assert.Equal(test[0, 1], values[0, 1]);
      Assert.Equal(test[1, 0], values[1, 0]);
      Assert.Equal(test[1, 1], values[1, 1]);
      Assert.Equal(test[2, 0], values[2, 0]);
      Assert.Equal(test[2, 1], values[2, 1]);
    }

    //Test Multiple Dimensional Double Array Constructor with null.
    [Fact]
    public void CtorMultDimDoubleNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        double[,] values = null;
        var test = new DoubleMatrix(values);
      });
    }

    //Test Multiple Dimensional Float Array Constructor with Square array.
    [Fact]
    public void CtorMultDimFloatSquare()
    {
      float[,] values = new float[2, 2];

      values[0, 0] = 0;
      values[0, 1] = 1;
      values[1, 0] = 3;
      values[1, 1] = 4;

      var test = new DoubleMatrix(values);

      Assert.Equal(2, test.RowLength);
      Assert.Equal(2, test.ColumnLength);
      Assert.Equal(test[0, 0], values[0, 0]);
      Assert.Equal(test[0, 1], values[0, 1]);
      Assert.Equal(test[1, 0], values[1, 0]);
      Assert.Equal(test[1, 1], values[1, 1]);
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

      var test = new DoubleMatrix(values);

      Assert.Equal(2, test.RowLength);
      Assert.Equal(3, test.ColumnLength);
      Assert.Equal(test[0, 0], values[0, 0]);
      Assert.Equal(test[0, 1], values[0, 1]);
      Assert.Equal(test[0, 2], values[0, 2]);
      Assert.Equal(test[1, 0], values[1, 0]);
      Assert.Equal(test[1, 1], values[1, 1]);
      Assert.Equal(test[1, 2], values[1, 2]);
    }

    //Test Multiple Dimensional FloatArray Constructor with long array.
    [Fact]
    public void CtorMultDimFloatLong()
    {
      float[,] values = new float[3, 2];

      values[0, 0] = 0;
      values[0, 1] = 1;
      values[1, 0] = 3;
      values[1, 1] = 4;
      values[2, 0] = 6;
      values[2, 1] = 7;

      var test = new DoubleMatrix(values);

      Assert.Equal(3, test.RowLength);
      Assert.Equal(2, test.ColumnLength);
      Assert.Equal(test[0, 0], values[0, 0]);
      Assert.Equal(test[0, 1], values[0, 1]);
      Assert.Equal(test[1, 0], values[1, 0]);
      Assert.Equal(test[1, 1], values[1, 1]);
      Assert.Equal(test[2, 0], values[2, 0]);
      Assert.Equal(test[2, 1], values[2, 1]);
    }

    //Test Multiple Dimensional Float Array Constructor with null.
    [Fact]
    public void CtorMultDimFloatNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        float[,] values = null;
        var test = new DoubleMatrix(values);
      });
    }

    //Test Jagged Array  Constructor with null.
    [Fact]
    public void CtorJaggedNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        double[,] values = null;
        var test = new DoubleMatrix(values);
      });
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

      DoubleMatrix b = a;
      Assert.Equal(a.RowLength, b.RowLength);
      Assert.Equal(a.ColumnLength, b.ColumnLength);
      Assert.Equal(a[0, 0], b[0, 0]);
      Assert.Equal(a[0, 1], b[0, 1]);
      Assert.Equal(a[1, 0], b[1, 0]);
      Assert.Equal(a[1, 1], b[1, 1]);
    }

    //Test implicit conversion from null floatmatrix.
    [Fact]
    public void ImplictFloatMatrixNull()
    {
      FloatMatrix a = null;
      DoubleMatrix b = a;
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

      var b = DoubleMatrix.ToDoubleMatrix(a);
      Assert.Equal(a.RowLength, b.RowLength);
      Assert.Equal(a.ColumnLength, b.ColumnLength);
      Assert.Equal(a[0, 0], b[0, 0]);
      Assert.Equal(a[0, 1], b[0, 1]);
      Assert.Equal(a[1, 0], b[1, 0]);
      Assert.Equal(a[1, 1], b[1, 1]);
    }

    //Test implicit conversion from null floatmatrix.
    [Fact]
    public void ImplictToFloatMatrixNull()
    {
      FloatMatrix a = null;
      var b = DoubleMatrix.ToDoubleMatrix(a);
      Assert.True(b is null);
    }

    //Test implicit conversion from double mult dim array.
    [Fact]
    public void ImplictDoubleMultArray()
    {
      double[,] a = new double[2, 2];
      a[0, 0] = 1;
      a[0, 1] = 2;
      a[1, 0] = 3;
      a[1, 1] = 4;

      DoubleMatrix b = a;
      Assert.Equal(a[0, 0], b[0, 0]);
      Assert.Equal(a[0, 1], b[0, 1]);
      Assert.Equal(a[1, 0], b[1, 0]);
      Assert.Equal(a[1, 1], b[1, 1]);
    }

    //Test implicit conversion from null double mult dim array.
    [Fact]
    public void ImplictDoubleMultArrayNull()
    {
      double[,] a = null;
      DoubleMatrix b = a;
      Assert.True(b is null);
    }

    //Test implicit conversion from double mult dim array.
    [Fact]
    public void ImplictToDoubleMultArray()
    {
      double[,] a = new double[2, 2];
      a[0, 0] = 1;
      a[0, 1] = 2;
      a[1, 0] = 3;
      a[1, 1] = 4;

      var b = DoubleMatrix.ToDoubleMatrix(a);
      Assert.Equal(a[0, 0], b[0, 0]);
      Assert.Equal(a[0, 1], b[0, 1]);
      Assert.Equal(a[1, 0], b[1, 0]);
      Assert.Equal(a[1, 1], b[1, 1]);
    }

    //Test implicit conversion from null double mult dim array.
    [Fact]
    public void ImplictToDoubleMultArrayNull()
    {
      double[,] a = null;
      var b = DoubleMatrix.ToDoubleMatrix(a);
      Assert.True(b is null);
    }

    //Test implicit conversion from float mult dim array.
    [Fact]
    public void ImplictFloatMultArray()
    {
      float[,] a = new float[2, 2];
      a[0, 0] = 1;
      a[0, 1] = 2;
      a[1, 0] = 3;
      a[1, 1] = 4;

      DoubleMatrix b = a;
      Assert.Equal(a[0, 0], b[0, 0]);
      Assert.Equal(a[0, 1], b[0, 1]);
      Assert.Equal(a[1, 0], b[1, 0]);
      Assert.Equal(a[1, 1], b[1, 1]);
    }

    //Test implicit conversion from null float mult dim array.
    [Fact]
    public void ImplictFloatMultArrayNull()
    {
      float[,] a = null;
      DoubleMatrix b = a;
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

      var b = DoubleMatrix.ToDoubleMatrix(a);
      Assert.Equal(a[0, 0], b[0, 0]);
      Assert.Equal(a[0, 1], b[0, 1]);
      Assert.Equal(a[1, 0], b[1, 0]);
      Assert.Equal(a[1, 1], b[1, 1]);
    }

    //Test implicit conversion from null float mult dim array.
    [Fact]
    public void ImplictToFloatMultArrayNull()
    {
      float[,] a = null;
      var b = DoubleMatrix.ToDoubleMatrix(a);
      Assert.True(b is null);
    }

    //test equals method
    [Fact]
    public void TestEquals()
    {
      var a = new DoubleMatrix(2, 2, 4);
      var b = new DoubleMatrix(2, 2, 4);
      var c = new DoubleMatrix(2, 2)
      {
        [0, 0] = 4,
        [0, 1] = 4,
        [1, 0] = 4,
        [1, 1] = 4
      };

      var d = new DoubleMatrix(2, 2, 5);
      DoubleMatrix e = null;
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
      var a = new DoubleMatrix(2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };

      int hash = a.GetHashCode();
      Assert.Equal(5, hash);
    }

    //test ToArray
    [Fact]
    public void ToArray()
    {
      var a = new DoubleMatrix(2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };

      double[,] b = a.ToArray();

      Assert.Equal(a[0, 0], b[0, 0]);
      Assert.Equal(a[0, 1], b[0, 1]);
      Assert.Equal(a[1, 0], b[1, 0]);
      Assert.Equal(a[1, 1], b[1, 1]);
    }

    //test Transpose square
    [Fact]
    public void TransposeSquare()
    {
      var a = new DoubleMatrix(2, 2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };
      a.Transpose();
      Assert.Equal(1, a[0, 0]);
      Assert.Equal(3, a[0, 1]);
      Assert.Equal(2, a[1, 0]);
      Assert.Equal(4, a[1, 1]);
    }

    //test Transpose wide
    [Fact]
    public void TransposeWide()
    {
      var a = new DoubleMatrix(2, 3)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [0, 2] = 3,
        [1, 0] = 4,
        [1, 1] = 5,
        [1, 2] = 6
      };
      a.Transpose();
      Assert.Equal(1, a[0, 0]);
      Assert.Equal(4, a[0, 1]);
      Assert.Equal(2, a[1, 0]);
      Assert.Equal(5, a[1, 1]);
      Assert.Equal(3, a[2, 0]);
      Assert.Equal(6, a[2, 1]);
      Assert.Equal(3, a.RowLength);
      Assert.Equal(2, a.ColumnLength);
    }

    //test Transpose long
    [Fact]
    public void TransposeLong()
    {
      var a = new DoubleMatrix(3, 2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4,
        [2, 0] = 5,
        [2, 1] = 6
      };
      a.Transpose();
      Assert.Equal(1, a[0, 0]);
      Assert.Equal(3, a[0, 1]);
      Assert.Equal(5, a[0, 2]);
      Assert.Equal(2, a[1, 0]);
      Assert.Equal(4, a[1, 1]);
      Assert.Equal(6, a[1, 2]);
      Assert.Equal(2, a.RowLength);
      Assert.Equal(3, a.ColumnLength);
    }

    //test GetTranspose square
    [Fact]
    public void GetTransposeSquare()
    {
      var a = new DoubleMatrix(2, 2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };
      DoubleMatrix b = a.GetTranspose();
      Assert.Equal(1, b[0, 0]);
      Assert.Equal(3, b[0, 1]);
      Assert.Equal(2, b[1, 0]);
      Assert.Equal(4, b[1, 1]);
    }

    //test GetTranspose wide
    [Fact]
    public void GetTransposeWide()
    {
      var a = new DoubleMatrix(2, 3)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [0, 2] = 3,
        [1, 0] = 4,
        [1, 1] = 5,
        [1, 2] = 6
      };
      DoubleMatrix b = a.GetTranspose();
      Assert.Equal(1, b[0, 0]);
      Assert.Equal(4, b[0, 1]);
      Assert.Equal(2, b[1, 0]);
      Assert.Equal(5, b[1, 1]);
      Assert.Equal(3, b[2, 0]);
      Assert.Equal(6, b[2, 1]);
      Assert.Equal(b.RowLength, a.ColumnLength);
      Assert.Equal(b.ColumnLength, a.RowLength);
    }

    //test GetTranspose long
    [Fact]
    public void GetTransposeLong()
    {
      var a = new DoubleMatrix(3, 2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4,
        [2, 0] = 5,
        [2, 1] = 6
      };
      DoubleMatrix b = a.GetTranspose();
      Assert.Equal(1, b[0, 0]);
      Assert.Equal(3, b[0, 1]);
      Assert.Equal(5, b[0, 2]);
      Assert.Equal(2, b[1, 0]);
      Assert.Equal(4, b[1, 1]);
      Assert.Equal(6, b[1, 2]);
      Assert.Equal(b.RowLength, a.ColumnLength);
      Assert.Equal(b.ColumnLength, a.RowLength);
    }

    //test Invert
    [Fact]
    public void Invert()
    {
      var a = new DoubleMatrix(2, 2)
      {
        [0, 0] = 2,
        [0, 1] = 4,
        [1, 0] = 3,
        [1, 1] = 7
      };
      a.Invert();
      AssertEx.Equal(a[0, 0], 3.5, 4e-15);
      AssertEx.Equal(a[0, 1], -2, 4e-15);
      AssertEx.Equal(a[1, 0], -1.5, 4e-15);
      AssertEx.Equal(a[1, 1], 1, 4e-15);
    }

    //test Invert singular
    [Fact]
    public void InvertSingular()
    {
      Assert.Throws<SingularMatrixException>(() =>
      {
        var a = new DoubleMatrix(2, 2);
        a.Invert();
      });
    }

    //test Invert not square
    [Fact]
    public void InvertNotSquare()
    {
      Assert.Throws<NotSquareMatrixException>(() =>
      {
        var a = new DoubleMatrix(3, 2)
        {
          [0, 0] = 1,
          [0, 1] = 2,
          [1, 0] = 3,
          [1, 1] = 4,
          [2, 0] = 5,
          [2, 1] = 6
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
        var a = new DoubleMatrix(2, 2);
        DoubleMatrix b = a.GetInverse();
      });
    }

    //test GetInverse not square
    [Fact]
    public void GetInverseNotSquare()
    {
      Assert.Throws<NotSquareMatrixException>(() =>
      {
        var a = new DoubleMatrix(3, 2)
        {
          [0, 0] = 1,
          [0, 1] = 2,
          [1, 0] = 3,
          [1, 1] = 4,
          [2, 0] = 5,
          [2, 1] = 6
        };
        DoubleMatrix b = a.GetInverse();
      });
    }

    //test GetInverse
    [Fact]
    public void GetInverse()
    {
      var a = new DoubleMatrix(2, 2)
      {
        [0, 0] = 2,
        [0, 1] = 4,
        [1, 0] = 3,
        [1, 1] = 7
      };
      DoubleMatrix b = a.GetInverse();
      AssertEx.Equal(b[0, 0], 3.500, TOLERANCE);
      AssertEx.Equal(b[0, 1], -2.000, TOLERANCE);
      AssertEx.Equal(b[1, 0], -1.500, TOLERANCE);
      AssertEx.Equal(b[1, 1], 1.000, TOLERANCE);
    }

    //test GetDeterminant
    [Fact]
    public void GetDeterminant()
    {
      var a = new DoubleMatrix(2, 2)
      {
        [0, 0] = 2,
        [0, 1] = 4,
        [1, 0] = 3,
        [1, 1] = 7
      };
      double b = a.GetDeterminant();
      AssertEx.Equal(b, 2.000, TOLERANCE);
    }

    //test GetDeterminant
    [Fact]
    public void GetDeterminantNotSquare()
    {
      Assert.Throws<NotSquareMatrixException>(() =>
      {
        var a = new DoubleMatrix(3, 2)
        {
          [0, 0] = 1,
          [0, 1] = 2,
          [1, 0] = 3,
          [1, 1] = 4,
          [2, 0] = 5,
          [2, 1] = 6
        };
        double b = a.GetDeterminant();
      });
    }

    //test GetRow
    [Fact]
    public void GetRow()
    {
      var a = new DoubleMatrix(2, 2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };
      DoubleVector b = a.GetRow(0);
      Assert.Equal(b[0], a[0, 0]);
      Assert.Equal(b[1], a[0, 1]);
    }

    //test GetRow
    [Fact]
    public void GetRowOutOfRange()
    {
      Assert.Throws<ArgumentOutOfRangeException>(() =>
      {
        var a = new DoubleMatrix(2, 2);
        DoubleVector b = a.GetRow(3);
      });
    }

    //test GetColumn
    [Fact]
    public void GetColumn()
    {
      var a = new DoubleMatrix(2, 2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };
      DoubleVector b = a.GetColumn(0);
      Assert.Equal(b[0], a[0, 0]);
      Assert.Equal(b[1], a[1, 0]);
    }

    //test GetColumn
    [Fact]
    public void GetColumnOutOfRange()
    {
      Assert.Throws<ArgumentOutOfRangeException>(() =>
      {
        var a = new DoubleMatrix(2, 2);
        DoubleVector b = a.GetColumn(3);
      });
    }

    //test SetRow
    [Fact]
    public void SetRow()
    {
      var a = new DoubleMatrix(2, 2);
      var b = new DoubleVector(2)
      {
        [0] = 1,
        [1] = 2
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
        var a = new DoubleMatrix(2, 2);
        var b = new DoubleVector(2);
        a.SetRow(2, b);
      });
    }

    //test SetRow
    [Fact]
    public void SetRowWrongRank()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new DoubleMatrix(2, 2);
        var b = new DoubleVector(3);
        a.SetRow(1, b);
      });
    }

    //test SetRow
    [Fact]
    public void SetRowArray()
    {
      var a = new DoubleMatrix(2, 2);
      double[] b = { 1, 2 };
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
        var a = new DoubleMatrix(2, 2);
        double[] b = { 1, 2 };
        a.SetRow(2, b);
      });
    }

    //test SetRow
    [Fact]
    public void SetRowArrayWrongRank()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new DoubleMatrix(2, 2);
        double[] b = { 1, 2, 3 };
        a.SetRow(1, b);
      });
    }

    //test SetColumn
    [Fact]
    public void SetColumn()
    {
      var a = new DoubleMatrix(2, 2);
      var b = new DoubleVector(2)
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
        var a = new DoubleMatrix(2, 2);
        var b = new DoubleVector(2);
        a.SetColumn(2, b);
      });
    }

    //test SetColumn
    [Fact]
    public void SetColumnWrongRank()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new DoubleMatrix(2, 2);
        var b = new DoubleVector(3);
        a.SetColumn(1, b);
      });
    }

    //test SetColumn
    [Fact]
    public void SetColumnArray()
    {
      var a = new DoubleMatrix(2, 2);
      double[] b = { 1, 2 };
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
        var a = new DoubleMatrix(2, 2);
        double[] b = { 1, 2 };
        a.SetColumn(2, b);
      });
    }

    //test SetColumn
    [Fact]
    public void SetColumnArrayWrongRank()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new DoubleMatrix(2, 2);
        double[] b = { 1, 2, 3 };
        a.SetColumn(1, b);
      });
    }

    //test GetDiagonal
    [Fact]
    public void GetDiagonal()
    {
      var a = new DoubleMatrix(2, 2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };
      DoubleVector b = a.GetDiagonal();
      Assert.Equal(b[0], a[0, 0]);
      Assert.Equal(b[1], a[1, 1]);
    }

    //test SetDiagonal
    [Fact]
    public void SetDiagonal()
    {
      var a = new DoubleMatrix(2, 2);
      var b = new DoubleVector(2)
      {
        [0] = 1,
        [1] = 2
      };
      a.SetDiagonal(b);
      Assert.Equal(b[0], a[0, 0]);
      Assert.Equal(b[1], a[1, 1]);
    }

    //test GetSubMatrix
    [Fact]
    public void GetSubMatrix()
    {
      var a = new DoubleMatrix(4)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [0, 2] = 3,
        [0, 3] = 4,
        [1, 0] = 5,
        [1, 1] = 6,
        [1, 2] = 7,
        [1, 3] = 8,
        [2, 0] = 9,
        [2, 1] = 10,
        [2, 2] = 11,
        [2, 3] = 12,
        [3, 0] = 13,
        [3, 1] = 14,
        [3, 2] = 15,
        [3, 3] = 16
      };
      DoubleMatrix b = a.GetSubMatrix(2, 2);
      DoubleMatrix c = a.GetSubMatrix(0, 1, 2, 2);
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
        var a = new DoubleMatrix(4);
        DoubleMatrix b = a.GetSubMatrix(-1, 2);
      });
    }

    //test GetSubMatrix
    [Fact]
    public void GetSubMatrixOutRange2()
    {
      Assert.Throws<ArgumentOutOfRangeException>(() =>
      {
        var a = new DoubleMatrix(4);
        DoubleMatrix b = a.GetSubMatrix(2, 4);
      });
    }

    //test GetSubMatrix
    [Fact]
    public void GetSubMatrixOutRange3()
    {
      Assert.Throws<ArgumentOutOfRangeException>(() =>
      {
        var a = new DoubleMatrix(4);
        DoubleMatrix b = a.GetSubMatrix(0, 0, 4, 2);
      });
    }

    //test GetSubMatrix
    [Fact]
    public void GetSubMatrixOutRange4()
    {
      Assert.Throws<ArgumentOutOfRangeException>(() =>
      {
        var a = new DoubleMatrix(4);
        DoubleMatrix b = a.GetSubMatrix(0, 0, 2, 4);
      });
    }

    //test GetSubMatrix
    [Fact]
    public void GetSubMatrixOutRange5()
    {
      Assert.Throws<ArgumentOutOfRangeException>(() =>
      {
        var a = new DoubleMatrix(4);
        DoubleMatrix b = a.GetSubMatrix(0, 3, 2, 2);
      });
    }

    //test GetUpperTriangle square matrix
    [Fact]
    public void GetUpperTriangleSquare()
    {
      var a = new DoubleMatrix(3)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [0, 2] = 3,
        [1, 0] = 4,
        [1, 1] = 5,
        [1, 2] = 6,
        [2, 0] = 7,
        [2, 1] = 8,
        [2, 2] = 9
      };
      DoubleMatrix b = a.GetUpperTriangle();

      Assert.Equal(b.RowLength, a.RowLength);
      Assert.Equal(b.ColumnLength, a.ColumnLength);
      Assert.Equal(b[0, 0], a[0, 0]);
      Assert.Equal(b[0, 1], a[0, 1]);
      Assert.Equal(b[0, 2], a[0, 2]);
      Assert.Equal(0, b[1, 0]);
      Assert.Equal(b[1, 1], a[1, 1]);
      Assert.Equal(b[1, 2], a[1, 2]);
      Assert.Equal(0, b[2, 0]);
      Assert.Equal(0, b[2, 1]);
      Assert.Equal(b[2, 2], a[2, 2]);
    }

    //test GetUpperTriangle long matrix
    [Fact]
    public void GetUpperTriangleLong()
    {
      var a = new DoubleMatrix(3, 2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 4,
        [1, 1] = 5,
        [2, 0] = 7,
        [2, 1] = 8
      };
      DoubleMatrix b = a.GetUpperTriangle();

      Assert.Equal(b.RowLength, a.RowLength);
      Assert.Equal(b.ColumnLength, a.ColumnLength);
      Assert.Equal(b[0, 0], a[0, 0]);
      Assert.Equal(b[0, 1], a[0, 1]);
      Assert.Equal(0, b[1, 0]);
      Assert.Equal(b[1, 1], a[1, 1]);
      Assert.Equal(0, b[2, 0]);
      Assert.Equal(0, b[2, 1]);
    }

    //test GetUpperTriangle wide matrix
    [Fact]
    public void GetUpperTriangleWide()
    {
      var a = new DoubleMatrix(2, 3)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [0, 2] = 3,
        [1, 0] = 4,
        [1, 1] = 5,
        [1, 2] = 6
      };
      DoubleMatrix b = a.GetUpperTriangle();

      Assert.Equal(b.RowLength, a.RowLength);
      Assert.Equal(b.ColumnLength, a.ColumnLength);
      Assert.Equal(b[0, 0], a[0, 0]);
      Assert.Equal(b[0, 1], a[0, 1]);
      Assert.Equal(b[0, 2], a[0, 2]);
      Assert.Equal(0, b[1, 0]);
      Assert.Equal(b[1, 1], a[1, 1]);
      Assert.Equal(b[1, 2], a[1, 2]);
    }

    //test GetStrictlyUpperTriangle square matrix
    [Fact]
    public void GetStrictlyUpperTriangleSquare()
    {
      var a = new DoubleMatrix(3)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [0, 2] = 3,
        [1, 0] = 4,
        [1, 1] = 5,
        [1, 2] = 6,
        [2, 0] = 7,
        [2, 1] = 8,
        [2, 2] = 9
      };
      DoubleMatrix b = a.GetStrictlyUpperTriangle();

      Assert.Equal(b.RowLength, a.RowLength);
      Assert.Equal(b.ColumnLength, a.ColumnLength);
      Assert.Equal(0, b[0, 0]);
      Assert.Equal(b[0, 1], a[0, 1]);
      Assert.Equal(b[0, 2], a[0, 2]);
      Assert.Equal(0, b[1, 0]);
      Assert.Equal(0, b[1, 1]);
      Assert.Equal(b[1, 2], a[1, 2]);
      Assert.Equal(0, b[2, 0]);
      Assert.Equal(0, b[2, 1]);
      Assert.Equal(0, b[2, 2]);
    }

    //test GetStrictlyUpperTriangle long matrix
    [Fact]
    public void GetStrictlyUpperTriangleLong()
    {
      var a = new DoubleMatrix(3, 2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 4,
        [1, 1] = 5,
        [2, 0] = 7,
        [2, 1] = 8
      };
      DoubleMatrix b = a.GetStrictlyUpperTriangle();

      Assert.Equal(b.RowLength, a.RowLength);
      Assert.Equal(b.ColumnLength, a.ColumnLength);
      Assert.Equal(0, b[0, 0]);
      Assert.Equal(b[0, 1], a[0, 1]);
      Assert.Equal(0, b[1, 0]);
      Assert.Equal(0, b[1, 1]);
      Assert.Equal(0, b[2, 0]);
      Assert.Equal(0, b[2, 1]);
    }

    //test GetStrictlyUpperTriangle wide matrix
    [Fact]
    public void GetStrictlyUpperTriangleWide()
    {
      var a = new DoubleMatrix(2, 3)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [0, 2] = 3,
        [1, 0] = 4,
        [1, 1] = 5,
        [1, 2] = 6
      };
      DoubleMatrix b = a.GetStrictlyUpperTriangle();

      Assert.Equal(b.RowLength, a.RowLength);
      Assert.Equal(b.ColumnLength, a.ColumnLength);
      Assert.Equal(0, b[0, 0]);
      Assert.Equal(b[0, 1], a[0, 1]);
      Assert.Equal(b[0, 2], a[0, 2]);
      Assert.Equal(0, b[1, 0]);
      Assert.Equal(0, b[1, 1]);
      Assert.Equal(b[1, 2], a[1, 2]);
    }

    //test GetLowerTriangle square matrix
    [Fact]
    public void GetLowerTriangleSquare()
    {
      var a = new DoubleMatrix(3)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [0, 2] = 3,
        [1, 0] = 4,
        [1, 1] = 5,
        [1, 2] = 6,
        [2, 0] = 7,
        [2, 1] = 8,
        [2, 2] = 9
      };
      DoubleMatrix b = a.GetLowerTriangle();

      Assert.Equal(b.RowLength, a.RowLength);
      Assert.Equal(b.ColumnLength, a.ColumnLength);
      Assert.Equal(b[0, 0], a[0, 0]);
      Assert.Equal(0, b[0, 1]);
      Assert.Equal(0, b[0, 2]);
      Assert.Equal(b[1, 0], a[1, 0]);
      Assert.Equal(b[1, 1], a[1, 1]);
      Assert.Equal(0, b[1, 2]);
      Assert.Equal(b[2, 0], a[2, 0]);
      Assert.Equal(b[2, 1], a[2, 1]);
      Assert.Equal(b[2, 2], a[2, 2]);
    }

    //test GetLowerTriangle long matrix
    [Fact]
    public void GetLowerTriangleLong()
    {
      var a = new DoubleMatrix(3, 2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 4,
        [1, 1] = 5,
        [2, 0] = 7,
        [2, 1] = 8
      };
      DoubleMatrix b = a.GetLowerTriangle();

      Assert.Equal(b.RowLength, a.RowLength);
      Assert.Equal(b.ColumnLength, a.ColumnLength);
      Assert.Equal(b[0, 0], a[0, 0]);
      Assert.Equal(0, b[0, 1]);
      Assert.Equal(b[1, 0], b[1, 0]);
      Assert.Equal(b[1, 1], a[1, 1]);
      Assert.Equal(b[2, 0], b[2, 0]);
      Assert.Equal(b[2, 1], b[2, 1]);
    }

    //test GetLowerTriangle wide matrix
    [Fact]
    public void GetLowerTriangleWide()
    {
      var a = new DoubleMatrix(2, 3)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [0, 2] = 3,
        [1, 0] = 4,
        [1, 1] = 5,
        [1, 2] = 6
      };
      DoubleMatrix b = a.GetLowerTriangle();

      Assert.Equal(b.RowLength, a.RowLength);
      Assert.Equal(b.ColumnLength, a.ColumnLength);
      Assert.Equal(b[0, 0], a[0, 0]);
      Assert.Equal(0, b[0, 1]);
      Assert.Equal(0, b[0, 2]);
      Assert.Equal(b[1, 0], a[1, 0]);
      Assert.Equal(b[1, 1], a[1, 1]);
      Assert.Equal(0, b[1, 2]);
    }

    //test GetStrictlyLowerTriangle square matrix
    [Fact]
    public void GetStrictlyLowerTriangleSquare()
    {
      var a = new DoubleMatrix(3)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [0, 2] = 3,
        [1, 0] = 4,
        [1, 1] = 5,
        [1, 2] = 6,
        [2, 0] = 7,
        [2, 1] = 8,
        [2, 2] = 9
      };
      DoubleMatrix b = a.GetStrictlyLowerTriangle();

      Assert.Equal(b.RowLength, a.RowLength);
      Assert.Equal(b.ColumnLength, a.ColumnLength);
      Assert.Equal(0, b[0, 0]);
      Assert.Equal(0, b[0, 1]);
      Assert.Equal(0, b[0, 2]);
      Assert.Equal(b[1, 0], a[1, 0]);
      Assert.Equal(0, b[1, 1]);
      Assert.Equal(0, b[1, 2]);
      Assert.Equal(b[2, 0], a[2, 0]);
      Assert.Equal(b[2, 1], a[2, 1]);
      Assert.Equal(0, b[2, 2]);
    }

    //test GetStrictlyLowerTriangle long matrix
    [Fact]
    public void GetStrictlyLowerTriangleLong()
    {
      var a = new DoubleMatrix(3, 2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 4,
        [1, 1] = 5,
        [2, 0] = 7,
        [2, 1] = 8
      };
      DoubleMatrix b = a.GetStrictlyLowerTriangle();

      Assert.Equal(b.RowLength, a.RowLength);
      Assert.Equal(b.ColumnLength, a.ColumnLength);
      Assert.Equal(0, b[0, 0]);
      Assert.Equal(0, b[0, 1]);
      Assert.Equal(b[1, 0], b[1, 0]);
      Assert.Equal(0, b[1, 1]);
      Assert.Equal(b[2, 0], b[2, 0]);
      Assert.Equal(b[2, 1], b[2, 1]);
    }

    //test GetStrictlyLowerTriangle wide matrix
    [Fact]
    public void GetStrictlyLowerTriangleWide()
    {
      var a = new DoubleMatrix(2, 3)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [0, 2] = 3,
        [1, 0] = 4,
        [1, 1] = 5,
        [1, 2] = 6
      };
      DoubleMatrix b = a.GetStrictlyLowerTriangle();

      Assert.Equal(b.RowLength, a.RowLength);
      Assert.Equal(b.ColumnLength, a.ColumnLength);
      Assert.Equal(0, b[0, 0]);
      Assert.Equal(0, b[0, 1]);
      Assert.Equal(0, b[0, 2]);
      Assert.Equal(b[1, 0], a[1, 0]);
      Assert.Equal(0, b[1, 1]);
      Assert.Equal(0, b[1, 2]);
    }

    //static Negate
    [Fact]
    public void Negate()
    {
      var a = new DoubleMatrix(2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };

      var b = DoubleMatrix.Negate(a);
      Assert.Equal(b[0, 0], -1);
      Assert.Equal(b[0, 1], -2);
      Assert.Equal(b[1, 0], -3);
      Assert.Equal(b[1, 1], -4);
    }

    //static NegateNull
    [Fact]
    public void NegateNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        DoubleMatrix a = null;
        var b = DoubleMatrix.Negate(a);
      });
    }

    //static operator -
    [Fact]
    public void OperatorMinus()
    {
      var a = new DoubleMatrix(2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };

      DoubleMatrix b = -a;
      Assert.Equal(b[0, 0], -1);
      Assert.Equal(b[0, 1], -2);
      Assert.Equal(b[1, 0], -3);
      Assert.Equal(b[1, 1], -4);
    }

    //static operator - null
    [Fact]
    public void OperatorMinusNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        DoubleMatrix a = null;
        DoubleMatrix b = -a;
      });
    }

    //static subtact two square matrices
    [Fact]
    public void StaticSubtract()
    {
      var a = new DoubleMatrix(2);
      var b = new DoubleMatrix(2);
      a[0, 0] = b[0, 0] = 1;
      a[0, 1] = b[0, 1] = 2;
      a[1, 0] = b[1, 0] = 3;
      a[1, 1] = b[1, 1] = 4;
      var c = DoubleMatrix.Subtract(a, b);
      Assert.Equal(0, c[0, 0]);
      Assert.Equal(0, c[0, 1]);
      Assert.Equal(0, c[1, 0]);
      Assert.Equal(0, c[1, 1]);
    }

    //operator subtract two square matrices
    [Fact]
    public void OperatorSubtract()
    {
      var a = new DoubleMatrix(2);
      var b = new DoubleMatrix(2);
      a[0, 0] = b[0, 0] = 1;
      a[0, 1] = b[0, 1] = 2;
      a[1, 0] = b[1, 0] = 3;
      a[1, 1] = b[1, 1] = 4;
      DoubleMatrix c = a - b;
      Assert.Equal(0, c[0, 0]);
      Assert.Equal(0, c[0, 1]);
      Assert.Equal(0, c[1, 0]);
      Assert.Equal(0, c[1, 1]);
    }

    //member add subtract square matrices
    [Fact]
    public void MemberSubtract()
    {
      var a = new DoubleMatrix(2);
      var b = new DoubleMatrix(2);
      a[0, 0] = b[0, 0] = 1;
      a[0, 1] = b[0, 1] = 2;
      a[1, 0] = b[1, 0] = 3;
      a[1, 1] = b[1, 1] = 4;
      a.Subtract(b);
      Assert.Equal(0, a[0, 0]);
      Assert.Equal(0, a[0, 1]);
      Assert.Equal(0, a[1, 0]);
      Assert.Equal(0, a[1, 1]);
    }

    //static Subtract two square matrices, one null
    [Fact]
    public void StaticSubtractNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        var a = new DoubleMatrix(2);
        DoubleMatrix b = null;
        var c = DoubleMatrix.Subtract(a, b);
      });
    }

    //operator Subtract two square matrices, one null
    [Fact]
    public void OperatorSubtractNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        var a = new DoubleMatrix(2);
        DoubleMatrix b = null;
        DoubleMatrix c = a - b;
      });
    }

    //member Subtract two square matrices, one null
    [Fact]
    public void MemberSubtractNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        var a = new DoubleMatrix(2);
        DoubleMatrix b = null;
        a.Subtract(b);
      });
    }

    //static Subtract two incompatible matrices
    [Fact]
    public void StaticSubtractIncompatible()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new DoubleMatrix(2);
        var b = new DoubleMatrix(3);
        var c = DoubleMatrix.Subtract(a, b);
      });
    }

    //operator Subtract two  incompatible matrices
    [Fact]
    public void OperatorSubtractIncompatible()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new DoubleMatrix(2);
        var b = new DoubleMatrix(3);
        DoubleMatrix c = a - b;
      });
    }

    //member Subtract two  incompatible matricess
    [Fact]
    public void MemberSubtractIncompatible()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new DoubleMatrix(2);
        var b = new DoubleMatrix(3);
        a.Subtract(b);
      });
    }

    //static add two square matrices
    [Fact]
    public void StaticAdd()
    {
      var a = new DoubleMatrix(2);
      var b = new DoubleMatrix(2);
      a[0, 0] = b[0, 0] = 1;
      a[0, 1] = b[0, 1] = 2;
      a[1, 0] = b[1, 0] = 3;
      a[1, 1] = b[1, 1] = 4;
      var c = DoubleMatrix.Add(a, b);
      Assert.Equal(2, c[0, 0]);
      Assert.Equal(4, c[0, 1]);
      Assert.Equal(6, c[1, 0]);
      Assert.Equal(8, c[1, 1]);
    }

    //operator add two square matrices
    [Fact]
    public void OperatorAdd()
    {
      var a = new DoubleMatrix(2);
      var b = new DoubleMatrix(2);
      a[0, 0] = b[0, 0] = 1;
      a[0, 1] = b[0, 1] = 2;
      a[1, 0] = b[1, 0] = 3;
      a[1, 1] = b[1, 1] = 4;
      DoubleMatrix c = a + b;
      Assert.Equal(2, c[0, 0]);
      Assert.Equal(4, c[0, 1]);
      Assert.Equal(6, c[1, 0]);
      Assert.Equal(8, c[1, 1]);
    }

    //member add two square matrices
    [Fact]
    public void MemberAdd()
    {
      var a = new DoubleMatrix(2);
      var b = new DoubleMatrix(2);
      a[0, 0] = b[0, 0] = 1;
      a[0, 1] = b[0, 1] = 2;
      a[1, 0] = b[1, 0] = 3;
      a[1, 1] = b[1, 1] = 4;
      a.Add(b);
      Assert.Equal(2, a[0, 0]);
      Assert.Equal(4, a[0, 1]);
      Assert.Equal(6, a[1, 0]);
      Assert.Equal(8, a[1, 1]);
    }

    //static add two square matrices, one null
    [Fact]
    public void StaticAddNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        var a = new DoubleMatrix(2);
        DoubleMatrix b = null;
        var c = DoubleMatrix.Add(a, b);
      });
    }

    //operator add two square matrices, one null
    [Fact]
    public void OperatorAddNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        var a = new DoubleMatrix(2);
        DoubleMatrix b = null;
        DoubleMatrix c = a + b;
      });
    }

    //member add two square matrices, one null
    [Fact]
    public void MemberAddNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        var a = new DoubleMatrix(2);
        DoubleMatrix b = null;
        a.Add(b);
      });
    }

    //static add two incompatible matrices
    [Fact]
    public void StaticAddIncompatible()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new DoubleMatrix(2);
        var b = new DoubleMatrix(3);
        var c = DoubleMatrix.Add(a, b);
      });
    }

    //operator add two  incompatible matrices
    [Fact]
    public void OperatorAddIncompatible()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new DoubleMatrix(2);
        var b = new DoubleMatrix(3);
        DoubleMatrix c = a + b;
      });
    }

    //member add two  incompatible matricess
    [Fact]
    public void MemberAddIncompatible()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new DoubleMatrix(2);
        var b = new DoubleMatrix(3);
        a.Add(b);
      });
    }

    //static divide matrix by double
    [Fact]
    public void StaticDivide()
    {
      var a = new DoubleMatrix(2)
      {
        [0, 0] = 2,
        [0, 1] = 4,
        [1, 0] = 6,
        [1, 1] = 8
      };
      var b = DoubleMatrix.Divide(a, 2);
      Assert.Equal(1, b[0, 0]);
      Assert.Equal(2, b[0, 1]);
      Assert.Equal(3, b[1, 0]);
      Assert.Equal(4, b[1, 1]);
    }

    //operator divide matrix by double
    [Fact]
    public void OperatorDivide()
    {
      var a = new DoubleMatrix(2)
      {
        [0, 0] = 2,
        [0, 1] = 4,
        [1, 0] = 6,
        [1, 1] = 8
      };
      DoubleMatrix b = a / 2;
      Assert.Equal(1, b[0, 0]);
      Assert.Equal(2, b[0, 1]);
      Assert.Equal(3, b[1, 0]);
      Assert.Equal(4, b[1, 1]);
    }

    //member divide matrix by double
    [Fact]
    public void MemberDivide()
    {
      var a = new DoubleMatrix(2)
      {
        [0, 0] = 2,
        [0, 1] = 4,
        [1, 0] = 6,
        [1, 1] = 8
      };
      a.Divide(2);
      Assert.Equal(1, a[0, 0]);
      Assert.Equal(2, a[0, 1]);
      Assert.Equal(3, a[1, 0]);
      Assert.Equal(4, a[1, 1]);
    }

    //static divide null matrix by double
    [Fact]
    public void StaticDivideNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        DoubleMatrix a = null;
        var b = DoubleMatrix.Divide(a, 2);
      });
    }

    //operator divide null matrix by double
    [Fact]
    public void OperatorDivideNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        DoubleMatrix a = null;
        DoubleMatrix b = a / 2;
      });
    }

    //copy
    [Fact]
    public void Copy()
    {
      var a = new DoubleMatrix(2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };
      var b = new DoubleMatrix(2);
      b.Copy(a);
      Assert.Equal(a[0, 0], a[0, 0]);
      Assert.Equal(a[0, 1], b[0, 1]);
      Assert.Equal(a[1, 0], b[1, 0]);
      Assert.Equal(a[1, 1], b[1, 1]);
    }

    //test multiply double matrix operator *
    [Fact]
    public void OperatorMultiplyDoubleMatrix()
    {
      var a = new DoubleMatrix(2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };
      DoubleMatrix b = 2.0 * a;
      Assert.Equal(2, b[0, 0]);
      Assert.Equal(4, b[0, 1]);
      Assert.Equal(6, b[1, 0]);
      Assert.Equal(8, b[1, 1]);
    }

    //test multiply double null matrix operator *
    [Fact]
    public void OperatorMultiplyDoubleMatrixNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        DoubleMatrix a = null;
        DoubleMatrix b = 2.0 * a;
      });
    }

    //test multiply  matrix double operator *
    [Fact]
    public void OperatorMultiplyMatrixDouble()
    {
      var a = new DoubleMatrix(2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };
      DoubleMatrix b = a * 2.0;
      Assert.Equal(2, b[0, 0]);
      Assert.Equal(4, b[0, 1]);
      Assert.Equal(6, b[1, 0]);
      Assert.Equal(8, b[1, 1]);
    }

    //test multiply  null matrix double operator *
    [Fact]
    public void OperatorMultiplyMatrixDoubleNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        DoubleMatrix a = null;
        DoubleMatrix b = a * 2;
      });
    }

    //test static multiply double matrix
    [Fact]
    public void StaticMultiplyDoubleMatrix()
    {
      var a = new DoubleMatrix(2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };
      var b = DoubleMatrix.Multiply(2.0, a);
      Assert.Equal(2, b[0, 0]);
      Assert.Equal(4, b[0, 1]);
      Assert.Equal(6, b[1, 0]);
      Assert.Equal(8, b[1, 1]);
    }

    //test static multiply double null matrix
    [Fact]
    public void StaticMultiplyDoubleMatrixNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        DoubleMatrix a = null;
        var b = DoubleMatrix.Multiply(2.0, a);
      });
    }

    //test static multiply  matrix double
    [Fact]
    public void StaticMultiplyMatrixDouble()
    {
      var a = new DoubleMatrix(2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };
      var b = DoubleMatrix.Multiply(a, 2.0);

      Assert.Equal(2, b[0, 0]);
      Assert.Equal(4, b[0, 1]);
      Assert.Equal(6, b[1, 0]);
      Assert.Equal(8, b[1, 1]);
    }

    //test static multiply  null matrix double operator *
    [Fact]
    public void StaticMultiplyMatrixDoubleNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        DoubleMatrix a = null;
        var b = DoubleMatrix.Multiply(a, 2.0);
      });
    }

    //test member multiply  double
    [Fact]
    public void MemberMultiplyDouble()
    {
      var a = new DoubleMatrix(2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };
      a.Multiply(2.0);
      Assert.Equal(2, a[0, 0]);
      Assert.Equal(4, a[0, 1]);
      Assert.Equal(6, a[1, 0]);
      Assert.Equal(8, a[1, 1]);
    }

    //test multiply  matrix vector operator *
    [Fact]
    public void OperatorMultiplyMatrixVector()
    {
      var a = new DoubleMatrix(2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };
      var b = new DoubleVector(2, 2);
      DoubleVector c = a * b;
      Assert.Equal(6, c[0]);
      Assert.Equal(14, c[1]);
    }

    //test multiply  matrix nonconform vector operator *
    [Fact]
    public void OperatorMultiplyMatrixNonConformVector()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new DoubleMatrix(2);
        var b = new DoubleVector(3, 2);
        DoubleVector c = a * b;
      });
    }

    //test multiply null matrix vector operator *
    [Fact]
    public void OperatorMultiplyNullMatrixVector()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        DoubleMatrix a = null;
        var b = new DoubleVector(2, 2);
        DoubleVector c = a * b;
      });
    }

    //test multiply matrix null vector operator *
    [Fact]
    public void OperatorMultiplyMatrixNullVector()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        var a = new DoubleMatrix(2)
        {
          [0, 0] = 1,
          [0, 1] = 2,
          [1, 0] = 3,
          [1, 1] = 4
        };
        DoubleVector b = null;
        DoubleVector c = a * b;
      });
    }

    //test static multiply  matrix vector
    [Fact]
    public void StaticMultiplyMatrixVector()
    {
      var a = new DoubleMatrix(2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };
      var b = new DoubleVector(2, 2);
      DoubleVector c = DoubleMatrix.Multiply(a, b);
      Assert.Equal(6, c[0]);
      Assert.Equal(14, c[1]);
    }

    //test static multiply  matrix nonconform vector
    [Fact]
    public void StaticMultiplyMatrixNonConformVector()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new DoubleMatrix(2);
        var b = new DoubleVector(3, 2);
        DoubleVector c = a * b;
      });
    }

    //test static multiply null matrix vector
    [Fact]
    public void StaticMultiplyNullMatrixVector()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        DoubleMatrix a = null;
        var b = new DoubleVector(2, 2);
        DoubleVector c = DoubleMatrix.Multiply(a, b);
      });
    }

    //test static multiply matrix null vector
    [Fact]
    public void StaticMultiplyMatrixNullVector()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        var a = new DoubleMatrix(2)
        {
          [0, 0] = 1,
          [0, 1] = 2,
          [1, 0] = 3,
          [1, 1] = 4
        };
        DoubleVector b = null;
        DoubleVector c = DoubleMatrix.Multiply(a, b);
      });
    }

    //test member multiply vector
    [Fact]
    public void MemberMultiplyVector()
    {
      var a = new DoubleMatrix(2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };
      var b = new DoubleVector(2, 2);
      a.Multiply(b);
      Assert.Equal(6, a[0, 0]);
      Assert.Equal(14, a[1, 0]);
      Assert.Equal(1, a.ColumnLength);
      Assert.Equal(2, a.RowLength);
    }

    //test member multiply  matrix nonconform vector
    [Fact]
    public void MemberMultiplyMatrixNonConformVector()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new DoubleMatrix(2);
        var b = new DoubleVector(3, 2);
        a.Multiply(b);
      });
    }

    //test member multiply null vector
    [Fact]
    public void MemberMultiplyNullVector()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        var a = new DoubleMatrix(2);
        DoubleVector b = null;
        a.Multiply(b);
      });
    }

    //test multiply  matrix matrix operator *
    [Fact]
    public void OperatorMultiplyMatrixMatrix()
    {
      var a = new DoubleMatrix(2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };
      var b = new DoubleMatrix(2, 2, 2.0);
      DoubleMatrix c = a * b;
      Assert.Equal(6, c[0, 0]);
      Assert.Equal(6, c[0, 1]);
      Assert.Equal(14, c[1, 0]);
      Assert.Equal(14, c[1, 1]);
    }

    //test multiply  nonconform matrix matrix operator *
    [Fact]
    public void OperatorMultiplyMatrixNonConformMatrix()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new DoubleMatrix(2);
        var b = new DoubleMatrix(3, 2, 2.0);
        DoubleMatrix c = a * b;
      });
    }

    //test multiply  long matrix wide matrix operator *
    [Fact]
    public void OperatorMultiplyLongMatrixWideMatrix()
    {
      var a = new DoubleMatrix(3, 2, 1);
      var b = new DoubleMatrix(2, 3, 2);
      DoubleMatrix c = a * b;
      Assert.Equal(4, c[0, 0]);
      Assert.Equal(4, c[0, 1]);
      Assert.Equal(4, c[0, 1]);
      Assert.Equal(4, c[1, 0]);
      Assert.Equal(4, c[1, 1]);
      Assert.Equal(4, c[1, 2]);
      Assert.Equal(4, c[2, 0]);
      Assert.Equal(4, c[2, 1]);
      Assert.Equal(4, c[2, 2]);
    }

    //test multiply  wide matrix long matrix operator *
    [Fact]
    public void OperatorMultiplyWideMatrixLongMatrix()
    {
      var a = new DoubleMatrix(2, 3, 1);
      var b = new DoubleMatrix(3, 2, 2);
      DoubleMatrix c = a * b;
      Assert.Equal(6, c[0, 0]);
      Assert.Equal(6, c[0, 1]);
      Assert.Equal(6, c[1, 0]);
      Assert.Equal(6, c[1, 1]);
    }

    //test multiply null matrix matrix operator *
    [Fact]
    public void OperatorMultiplyNullMatrixMatrix()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        DoubleMatrix a = null;
        var b = new DoubleMatrix(2, 2);
        DoubleMatrix c = a * b;
      });
    }

    //test multiply matrix null matrix operator *
    [Fact]
    public void OperatorMultiplyMatrixNullMatrix()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        var a = new DoubleMatrix(2, 2);
        DoubleMatrix b = null;
        DoubleMatrix c = a * b;
      });
    }

    //test static multiply  matrix matrix
    [Fact]
    public void StaticMultiplyMatrixMatrix()
    {
      var a = new DoubleMatrix(2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };
      var b = new DoubleMatrix(2, 2, 2.0);
      var c = DoubleMatrix.Multiply(a, b);
      Assert.Equal(6, c[0, 0]);
      Assert.Equal(6, c[0, 1]);
      Assert.Equal(14, c[1, 0]);
      Assert.Equal(14, c[1, 1]);
    }

    //test static multiply nonconform matrix matrix
    [Fact]
    public void StaticMultiplyMatrixNonConformMatrix()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new DoubleMatrix(2);
        var b = new DoubleMatrix(3, 2, 2.0);
        var c = DoubleMatrix.Multiply(a, b);
      });
    }

    //test static multiply  long matrix wide matrix
    [Fact]
    public void StaticMultiplyLongMatrixWideMatrix()
    {
      var a = new DoubleMatrix(3, 2, 1);
      var b = new DoubleMatrix(2, 3, 2);
      var c = DoubleMatrix.Multiply(a, b);
      Assert.Equal(4, c[0, 0]);
      Assert.Equal(4, c[0, 1]);
      Assert.Equal(4, c[0, 1]);
      Assert.Equal(4, c[1, 0]);
      Assert.Equal(4, c[1, 1]);
      Assert.Equal(4, c[1, 2]);
      Assert.Equal(4, c[2, 0]);
      Assert.Equal(4, c[2, 1]);
      Assert.Equal(4, c[2, 2]);
    }

    //test static multiply  wide matrix long matrix
    [Fact]
    public void StaticMultiplyWideMatrixLongMatrix()
    {
      var a = new DoubleMatrix(2, 3, 1);
      var b = new DoubleMatrix(3, 2, 2);
      var c = DoubleMatrix.Multiply(a, b);
      Assert.Equal(6, c[0, 0]);
      Assert.Equal(6, c[0, 1]);
      Assert.Equal(6, c[1, 0]);
      Assert.Equal(6, c[1, 1]);
    }

    //test static multiply null matrix matrix
    [Fact]
    public void StaticMultiplyNullMatrixMatrix()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        DoubleMatrix a = null;
        var b = new DoubleMatrix(2, 2);
        var c = DoubleMatrix.Multiply(a, b);
      });
    }

    //test static multiply matrix null matrix
    [Fact]
    public void StaticMultiplyMatrixNullMatrix()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        var a = new DoubleMatrix(2, 2);
        DoubleMatrix b = null;
        var c = DoubleMatrix.Multiply(a, b);
      });
    }

    //test member multiply  matrix matrix
    [Fact]
    public void MemberMultiplyMatrixMatrix()
    {
      var a = new DoubleMatrix(2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };
      var b = new DoubleMatrix(2, 2, 2.0);
      a.Multiply(b);
      Assert.Equal(6, a[0, 0]);
      Assert.Equal(6, a[0, 1]);
      Assert.Equal(14, a[1, 0]);
      Assert.Equal(14, a[1, 1]);
    }

    //test member multiply nonconform matrix matrix
    [Fact]
    public void MemberMultiplyMatrixNonConformMatrix()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new DoubleMatrix(2);
        var b = new DoubleMatrix(3, 2, 2.0);
        a.Multiply(b);
      });
    }

    //test member multiply  long matrix wide matrix
    [Fact]
    public void MemberMultiplyLongMatrixWideMatrix()
    {
      var a = new DoubleMatrix(3, 2, 1);
      var b = new DoubleMatrix(2, 3, 2);
      a.Multiply(b);
      Assert.Equal(4, a[0, 0]);
      Assert.Equal(4, a[0, 1]);
      Assert.Equal(4, a[0, 1]);
      Assert.Equal(4, a[1, 0]);
      Assert.Equal(4, a[1, 1]);
      Assert.Equal(4, a[1, 2]);
      Assert.Equal(4, a[2, 0]);
      Assert.Equal(4, a[2, 1]);
      Assert.Equal(4, a[2, 2]);
    }

    //test member multiply  wide matrix long matrix
    [Fact]
    public void MemberMultiplyWideMatrixLongMatrix()
    {
      var a = new DoubleMatrix(2, 3, 1);
      var b = new DoubleMatrix(3, 2, 2);
      a.Multiply(b);
      Assert.Equal(6, a[0, 0]);
      Assert.Equal(6, a[0, 1]);
      Assert.Equal(6, a[1, 0]);
      Assert.Equal(6, a[1, 1]);
    }

    //test member multiply null matrix matrix
    [Fact]
    public void MemberMultiplyNullMatrixMatrix()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        var a = new DoubleMatrix(2, 2);
        DoubleMatrix b = null;
        a.Multiply(b);
      });
    }

    //copy null
    [Fact]
    public void CopyNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        DoubleMatrix a = null;
        var b = new DoubleMatrix(2);
        b.Copy(a);
      });
    }

    //Norm
    [Fact]
    public void Norms()
    {
      var a = new DoubleMatrix(2)
      {
        [0, 0] = 2,
        [0, 1] = 4,
        [1, 0] = 3,
        [1, 1] = 1
      };
      AssertEx.Equal(a.GetL1Norm(), 5.000, TOLERANCE);
      AssertEx.Equal(a.GetL2Norm(), 5.117, TOLERANCE);
      AssertEx.Equal(a.GetInfinityNorm(), 6.000, TOLERANCE);
      AssertEx.Equal(a.GetFrobeniusNorm(), 5.477, TOLERANCE);
    }

    //Wide Norm
    [Fact]
    public void WideNorms()
    {
      var a = new DoubleMatrix(2, 3)
      {
        [0, 0] = 2,
        [0, 1] = 4,
        [0, 2] = 5,
        [1, 0] = 3,
        [1, 1] = 1,
        [1, 2] = 6
      };
      AssertEx.Equal(a.GetL1Norm(), 11.000, TOLERANCE);
      AssertEx.Equal(a.GetL2Norm(), 9.247, TOLERANCE);
      AssertEx.Equal(a.GetInfinityNorm(), 11.000, TOLERANCE);
      AssertEx.Equal(a.GetFrobeniusNorm(), 9.539, TOLERANCE);
    }

    //Long Norm
    [Fact]
    public void LongNorms()
    {
      var a = new DoubleMatrix(3, 2)
      {
        [0, 0] = 2,
        [0, 1] = 4,
        [1, 0] = 3,
        [1, 1] = 1,
        [2, 0] = 5,
        [2, 1] = 6
      };
      AssertEx.Equal(a.GetL1Norm(), 11.000, TOLERANCE);
      AssertEx.Equal(a.GetL2Norm(), 9.337, TOLERANCE);
      AssertEx.Equal(a.GetInfinityNorm(), 11.000, TOLERANCE);
      AssertEx.Equal(a.GetFrobeniusNorm(), 9.539, TOLERANCE);
    }

    //Condition
    [Fact]
    public void Condition()
    {
      var a = new DoubleMatrix(2)
      {
        [0, 0] = 2,
        [0, 1] = 4,
        [1, 0] = 3,
        [1, 1] = 1
      };
      AssertEx.Equal(a.GetConditionNumber(), 2.618, TOLERANCE);
    }

    //Wide Condition
    [Fact]
    public void WideCondition()
    {
      Assert.Throws<NotSquareMatrixException>(() =>
      {
        var a = new DoubleMatrix(2, 3);
        a.GetConditionNumber();
      });
    }

    //Long Condition
    [Fact]
    public void LongCondition()
    {
      Assert.Throws<NotSquareMatrixException>(() =>
      {
        var a = new DoubleMatrix(3, 2);
        a.GetConditionNumber();
      });
    }
  }
}
