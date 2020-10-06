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

  public class FloatMatrixTest
  {
    private const double TOLERANCE = 0.001;

    //Test dimensions Constructor.
    [Fact]
    public void CtorDimensions()
    {
      var test = new FloatMatrix(2, 2);
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
      var test = new FloatMatrix(2, 2, 1);

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
      var a = new FloatMatrix(2, 2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };

      var b = new FloatMatrix(a);

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
        FloatMatrix a = null;
        var b = new FloatMatrix(a);
      });
    }

    //Test Multiple Dimensional FloatArray Constructor with Square array.
    [Fact]
    public void CtorMultDimFloatSquare()
    {
      float[,] values = new float[2, 2];

      values[0, 0] = 0;
      values[0, 1] = 1;
      values[1, 0] = 3;
      values[1, 1] = 4;

      var test = new FloatMatrix(values);

      Assert.Equal(2, test.RowLength);
      Assert.Equal(2, test.ColumnLength);
      Assert.Equal(test[0, 0], values[0, 0]);
      Assert.Equal(test[0, 1], values[0, 1]);
      Assert.Equal(test[1, 0], values[1, 0]);
      Assert.Equal(test[1, 1], values[1, 1]);
    }

    //Test Multiple Dimensional FloatArray Constructor with wide array.
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

      var test = new FloatMatrix(values);

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

      var test = new FloatMatrix(values);

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
        var test = new FloatMatrix(values);
      });
    }

    //Test Jagged Array  Constructor with null.
    [Fact]
    public void CtorJaggedNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        float[,] values = null;
        var test = new FloatMatrix(values);
      });
    }

    //Test explicit conversion from DoubleMatrix
    [Fact]
    public void ExplicitDoubleMatrix()
    {
      var a = new DoubleMatrix(2, 2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };

      var b = (FloatMatrix)a;
      Assert.Equal(a.RowLength, b.RowLength);
      Assert.Equal(a.ColumnLength, b.ColumnLength);
      Assert.Equal(a[0, 0], b[0, 0]);
      Assert.Equal(a[0, 1], b[0, 1]);
      Assert.Equal(a[1, 0], b[1, 0]);
      Assert.Equal(a[1, 1], b[1, 1]);
    }

    //Test explicit conversion from DoubleMatrix
    [Fact]
    public void ExplicitDoubleMatrixNull()
    {
      DoubleMatrix a = null;
      var b = (FloatMatrix)a;
      Assert.True(b is null);
    }

    //Test explicit conversion from DoubleMatrix
    [Fact]
    public void ExplicitToDoubleMatrix()
    {
      var a = new DoubleMatrix(2, 2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };

      var b = FloatMatrix.ToFloatMatrix(a);
      Assert.Equal(a.RowLength, b.RowLength);
      Assert.Equal(a.ColumnLength, b.ColumnLength);
      Assert.Equal(a[0, 0], b[0, 0]);
      Assert.Equal(a[0, 1], b[0, 1]);
      Assert.Equal(a[1, 0], b[1, 0]);
      Assert.Equal(a[1, 1], b[1, 1]);
    }

    //Test explicit conversion from DoubleMatrix
    [Fact]
    public void ExplicitToDoubleMatrixNull()
    {
      DoubleMatrix a = null;
      var b = FloatMatrix.ToFloatMatrix(a);
      Assert.True(b is null);
    }

    //Test explicit conversion from double array
    [Fact]
    public void ExplicitDoubleMultArray()
    {
      double[,] a = new double[2, 2];
      a[0, 0] = 1;
      a[0, 1] = 2;
      a[1, 0] = 3;
      a[1, 1] = 4;

      var b = (FloatMatrix)a;
      Assert.Equal(2, b.RowLength);
      Assert.Equal(2, b.ColumnLength);
      Assert.Equal(a[0, 0], b[0, 0]);
      Assert.Equal(a[0, 1], b[0, 1]);
      Assert.Equal(a[1, 0], b[1, 0]);
      Assert.Equal(a[1, 1], b[1, 1]);
    }

    //Test explicit conversion from double array
    [Fact]
    public void ExplicitDoubleMultArrayNull()
    {
      double[,] a = null;
      var b = (FloatMatrix)a;
      Assert.True(b is null);
    }

    //Test explicit conversion from double array
    [Fact]
    public void ExplicitToDoubleMultArray()
    {
      double[,] a = new double[2, 2];
      a[0, 0] = 1;
      a[0, 1] = 2;
      a[1, 0] = 3;
      a[1, 1] = 4;

      var b = FloatMatrix.ToFloatMatrix(a);
      Assert.Equal(2, b.RowLength);
      Assert.Equal(2, b.ColumnLength);
      Assert.Equal(a[0, 0], b[0, 0]);
      Assert.Equal(a[0, 1], b[0, 1]);
      Assert.Equal(a[1, 0], b[1, 0]);
      Assert.Equal(a[1, 1], b[1, 1]);
    }

    //Test explicit conversion from double array
    [Fact]
    public void ExplicitToDoubleMultArrayNull()
    {
      double[,] a = null;
      var b = FloatMatrix.ToFloatMatrix(a);
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

      FloatMatrix b = a;
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
      FloatMatrix b = a;
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

      var b = FloatMatrix.ToFloatMatrix(a);
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
      var b = FloatMatrix.ToFloatMatrix(a);
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

      FloatMatrix b = a;
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
      FloatMatrix b = a;
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

      var b = FloatMatrix.ToFloatMatrix(a);
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
      var b = FloatMatrix.ToFloatMatrix(a);
      Assert.True(b is null);
    }

    //test equals method
    [Fact]
    public void TestEquals()
    {
      var a = new FloatMatrix(2, 2, 4);
      var b = new FloatMatrix(2, 2, 4);
      var c = new FloatMatrix(2, 2)
      {
        [0, 0] = 4,
        [0, 1] = 4,
        [1, 0] = 4,
        [1, 1] = 4
      };

      var d = new FloatMatrix(2, 2, 5);
      FloatMatrix e = null;
      var f = new DoubleMatrix(2, 2, 4);
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
      var a = new FloatMatrix(2)
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
      var a = new FloatMatrix(2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };

      float[,] b = a.ToArray();

      Assert.Equal(a[0, 0], b[0, 0]);
      Assert.Equal(a[0, 1], b[0, 1]);
      Assert.Equal(a[1, 0], b[1, 0]);
      Assert.Equal(a[1, 1], b[1, 1]);
    }

    //test Transpose square
    [Fact]
    public void TransposeSquare()
    {
      var a = new FloatMatrix(2, 2)
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
      var a = new FloatMatrix(2, 3)
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
      var a = new FloatMatrix(3, 2)
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
      var a = new FloatMatrix(2, 2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };
      FloatMatrix b = a.GetTranspose();
      Assert.Equal(1, b[0, 0]);
      Assert.Equal(3, b[0, 1]);
      Assert.Equal(2, b[1, 0]);
      Assert.Equal(4, b[1, 1]);
    }

    //test GetTranspose wide
    [Fact]
    public void GetTransposeWide()
    {
      var a = new FloatMatrix(2, 3)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [0, 2] = 3,
        [1, 0] = 4,
        [1, 1] = 5,
        [1, 2] = 6
      };
      FloatMatrix b = a.GetTranspose();
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
      var a = new FloatMatrix(3, 2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4,
        [2, 0] = 5,
        [2, 1] = 6
      };
      FloatMatrix b = a.GetTranspose();
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
      var a = new FloatMatrix(2, 2)
      {
        [0, 0] = 2,
        [0, 1] = 4,
        [1, 0] = 3,
        [1, 1] = 7
      };
      a.Invert();
      AssertEx.Equal(a[0, 0], 3.500, TOLERANCE);
      AssertEx.Equal(a[0, 1], -2.00, TOLERANCE);
      AssertEx.Equal(a[1, 0], -1.500, TOLERANCE);
      AssertEx.Equal(a[1, 1], 1.000, TOLERANCE);
    }

    //test Invert singular
    [Fact]
    public void InvertSingular()
    {
      Assert.Throws<SingularMatrixException>(() =>
      {
        var a = new FloatMatrix(2, 2);
        a.Invert();
      });
    }

    //test Invert not square
    [Fact]
    public void InvertNotSquare()
    {
      Assert.Throws<NotSquareMatrixException>(() =>
      {
        var a = new FloatMatrix(3, 2)
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
        var a = new FloatMatrix(2, 2);
        FloatMatrix b = a.GetInverse();
      });
    }

    //test GetInverse not square
    [Fact]
    public void GetInverseNotSquare()
    {
      Assert.Throws<NotSquareMatrixException>(() =>
      {
        var a = new FloatMatrix(3, 2)
        {
          [0, 0] = 1,
          [0, 1] = 2,
          [1, 0] = 3,
          [1, 1] = 4,
          [2, 0] = 5,
          [2, 1] = 6
        };
        FloatMatrix b = a.GetInverse();
      });
    }

    //test GetInverse
    [Fact]
    public void GetInverse()
    {
      var a = new FloatMatrix(2, 2)
      {
        [0, 0] = 2,
        [0, 1] = 4,
        [1, 0] = 3,
        [1, 1] = 7
      };
      FloatMatrix b = a.GetInverse();
      AssertEx.Equal(b[0, 0], 3.500, TOLERANCE);
      AssertEx.Equal(b[0, 1], -2.000, TOLERANCE);
      AssertEx.Equal(b[1, 0], -1.500, TOLERANCE);
      AssertEx.Equal(b[1, 1], 1.000, TOLERANCE);
    }

    //test GetDeterminant
    [Fact]
    public void GetDeterminant()
    {
      var a = new FloatMatrix(2, 2)
      {
        [0, 0] = 2,
        [0, 1] = 4,
        [1, 0] = 3,
        [1, 1] = 7
      };
      float b = a.GetDeterminant();
      AssertEx.Equal(b, 2.000, TOLERANCE);
    }

    //test GetDeterminant
    [Fact]
    public void GetDeterminantNotSquare()
    {
      Assert.Throws<NotSquareMatrixException>(() =>
      {
        var a = new FloatMatrix(3, 2)
        {
          [0, 0] = 1,
          [0, 1] = 2,
          [1, 0] = 3,
          [1, 1] = 4,
          [2, 0] = 5,
          [2, 1] = 6
        };
        float b = a.GetDeterminant();
      });
    }

    //test GetRow
    [Fact]
    public void GetRow()
    {
      var a = new FloatMatrix(2, 2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };
      FloatVector b = a.GetRow(0);
      Assert.Equal(b[0], a[0, 0]);
      Assert.Equal(b[1], a[0, 1]);
    }

    //test GetRow
    [Fact]
    public void GetRowOutOfRange()
    {
      Assert.Throws<ArgumentOutOfRangeException>(() =>
      {
        var a = new FloatMatrix(2, 2);
        FloatVector b = a.GetRow(3);
      });
    }

    //test GetColumn
    [Fact]
    public void GetColumn()
    {
      var a = new FloatMatrix(2, 2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };
      FloatVector b = a.GetColumn(0);
      Assert.Equal(b[0], a[0, 0]);
      Assert.Equal(b[1], a[1, 0]);
    }

    //test GetColumn
    [Fact]
    public void GetColumnOutOfRange()
    {
      Assert.Throws<ArgumentOutOfRangeException>(() =>
      {
        var a = new FloatMatrix(2, 2);
        FloatVector b = a.GetColumn(3);
      });
    }

    //test GetDiagonal
    [Fact]
    public void GetDiagonal()
    {
      var a = new FloatMatrix(2, 2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };
      FloatVector b = a.GetDiagonal();
      Assert.Equal(b[0], a[0, 0]);
      Assert.Equal(b[1], a[1, 1]);
    }

    //test SetRow
    [Fact]
    public void SetRow()
    {
      var a = new FloatMatrix(2, 2);
      var b = new FloatVector(2)
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
        var a = new FloatMatrix(2, 2);
        var b = new FloatVector(2);
        a.SetRow(2, b);
      });
    }

    //test SetRow
    [Fact]
    public void SetRowWrongRank()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new FloatMatrix(2, 2);
        var b = new FloatVector(3);
        a.SetRow(1, b);
      });
    }

    //test SetRow
    [Fact]
    public void SetRowArray()
    {
      var a = new FloatMatrix(2, 2);
      float[] b = { 1, 2 };
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
        var a = new FloatMatrix(2, 2);
        float[] b = { 1, 2 };
        a.SetRow(2, b);
      });
    }

    //test SetRow
    [Fact]
    public void SetRowArrayWrongRank()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new FloatMatrix(2, 2);
        float[] b = { 1, 2, 3 };
        a.SetRow(1, b);
      });
    }

    //test SetColumn
    [Fact]
    public void SetColumn()
    {
      var a = new FloatMatrix(2, 2);
      var b = new FloatVector(2)
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
        var a = new FloatMatrix(2, 2);
        var b = new FloatVector(2);
        a.SetColumn(2, b);
      });
    }

    //test SetColumn
    [Fact]
    public void SetColumnWrongRank()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new FloatMatrix(2, 2);
        var b = new FloatVector(3);
        a.SetColumn(1, b);
      });
    }

    //test SetColumn
    [Fact]
    public void SetColumnArray()
    {
      var a = new FloatMatrix(2, 2);
      float[] b = { 1, 2 };
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
        var a = new FloatMatrix(2, 2);
        float[] b = { 1, 2 };
        a.SetColumn(2, b);
      });
    }

    //test SetColumn
    [Fact]
    public void SetColumnArrayWrongRank()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new FloatMatrix(2, 2);
        float[] b = { 1, 2, 3 };
        a.SetColumn(1, b);
      });
    }

    //test SetDiagonal
    [Fact]
    public void SetDiagonal()
    {
      var a = new FloatMatrix(2, 2);
      var b = new FloatVector(2)
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
      var a = new FloatMatrix(4)
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
      FloatMatrix b = a.GetSubMatrix(2, 2);
      FloatMatrix c = a.GetSubMatrix(0, 1, 2, 2);
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
        var a = new FloatMatrix(4);
        FloatMatrix b = a.GetSubMatrix(-1, 2);
      });
    }

    //test GetSubMatrix
    [Fact]
    public void GetSubMatrixOutRange2()
    {
      Assert.Throws<ArgumentOutOfRangeException>(() =>
      {
        var a = new FloatMatrix(4);
        FloatMatrix b = a.GetSubMatrix(2, 4);
      });
    }

    //test GetSubMatrix
    [Fact]
    public void GetSubMatrixOutRange3()
    {
      Assert.Throws<ArgumentOutOfRangeException>(() =>
      {
        var a = new FloatMatrix(4);
        FloatMatrix b = a.GetSubMatrix(0, 0, 4, 2);
      });
    }

    //test GetSubMatrix
    [Fact]
    public void GetSubMatrixOutRange4()
    {
      Assert.Throws<ArgumentOutOfRangeException>(() =>
      {
        var a = new FloatMatrix(4);
        FloatMatrix b = a.GetSubMatrix(0, 0, 2, 4);
      });
    }

    //test GetSubMatrix
    [Fact]
    public void GetSubMatrixOutRange5()
    {
      Assert.Throws<ArgumentOutOfRangeException>(() =>
      {
        var a = new FloatMatrix(4);
        FloatMatrix b = a.GetSubMatrix(0, 3, 2, 2);
      });
    }

    //test GetUpperTriangle square matrix
    [Fact]
    public void GetUpperTriangleSquare()
    {
      var a = new FloatMatrix(3)
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
      FloatMatrix b = a.GetUpperTriangle();

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
      var a = new FloatMatrix(3, 2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 4,
        [1, 1] = 5,
        [2, 0] = 7,
        [2, 1] = 8
      };
      FloatMatrix b = a.GetUpperTriangle();

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
      var a = new FloatMatrix(2, 3)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [0, 2] = 3,
        [1, 0] = 4,
        [1, 1] = 5,
        [1, 2] = 6
      };
      FloatMatrix b = a.GetUpperTriangle();

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
      var a = new FloatMatrix(3)
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
      FloatMatrix b = a.GetStrictlyUpperTriangle();

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
      var a = new FloatMatrix(3, 2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 4,
        [1, 1] = 5,
        [2, 0] = 7,
        [2, 1] = 8
      };
      FloatMatrix b = a.GetStrictlyUpperTriangle();

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
      var a = new FloatMatrix(2, 3)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [0, 2] = 3,
        [1, 0] = 4,
        [1, 1] = 5,
        [1, 2] = 6
      };
      FloatMatrix b = a.GetStrictlyUpperTriangle();

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
      var a = new FloatMatrix(3)
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
      FloatMatrix b = a.GetLowerTriangle();

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
      var a = new FloatMatrix(3, 2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 4,
        [1, 1] = 5,
        [2, 0] = 7,
        [2, 1] = 8
      };
      FloatMatrix b = a.GetLowerTriangle();

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
      var a = new FloatMatrix(2, 3)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [0, 2] = 3,
        [1, 0] = 4,
        [1, 1] = 5,
        [1, 2] = 6
      };
      FloatMatrix b = a.GetLowerTriangle();

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
      var a = new FloatMatrix(3)
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
      FloatMatrix b = a.GetStrictlyLowerTriangle();

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
      var a = new FloatMatrix(3, 2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 4,
        [1, 1] = 5,
        [2, 0] = 7,
        [2, 1] = 8
      };
      FloatMatrix b = a.GetStrictlyLowerTriangle();

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
      var a = new FloatMatrix(2, 3)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [0, 2] = 3,
        [1, 0] = 4,
        [1, 1] = 5,
        [1, 2] = 6
      };
      FloatMatrix b = a.GetStrictlyLowerTriangle();

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
      var a = new FloatMatrix(2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };

      var b = FloatMatrix.Negate(a);
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
        FloatMatrix a = null;
        var b = FloatMatrix.Negate(a);
      });
    }

    //static operator -
    [Fact]
    public void OperatorMinus()
    {
      var a = new FloatMatrix(2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };

      FloatMatrix b = -a;
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
        FloatMatrix a = null;
        FloatMatrix b = -a;
      });
    }

    //static subtact two square matrices
    [Fact]
    public void StaticSubtract()
    {
      var a = new FloatMatrix(2);
      var b = new FloatMatrix(2);
      a[0, 0] = b[0, 0] = 1;
      a[0, 1] = b[0, 1] = 2;
      a[1, 0] = b[1, 0] = 3;
      a[1, 1] = b[1, 1] = 4;
      var c = FloatMatrix.Subtract(a, b);
      Assert.Equal(0, c[0, 0]);
      Assert.Equal(0, c[0, 1]);
      Assert.Equal(0, c[1, 0]);
      Assert.Equal(0, c[1, 1]);
    }

    //operator subtract two square matrices
    [Fact]
    public void OperatorSubtract()
    {
      var a = new FloatMatrix(2);
      var b = new FloatMatrix(2);
      a[0, 0] = b[0, 0] = 1;
      a[0, 1] = b[0, 1] = 2;
      a[1, 0] = b[1, 0] = 3;
      a[1, 1] = b[1, 1] = 4;
      FloatMatrix c = a - b;
      Assert.Equal(0, c[0, 0]);
      Assert.Equal(0, c[0, 1]);
      Assert.Equal(0, c[1, 0]);
      Assert.Equal(0, c[1, 1]);
    }

    //member add subtract square matrices
    [Fact]
    public void MemberSubtract()
    {
      var a = new FloatMatrix(2);
      var b = new FloatMatrix(2);
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
        var a = new FloatMatrix(2);
        FloatMatrix b = null;
        var c = FloatMatrix.Subtract(a, b);
      });
    }

    //operator Subtract two square matrices, one null
    [Fact]
    public void OperatorSubtractNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        var a = new FloatMatrix(2);
        FloatMatrix b = null;
        FloatMatrix c = a - b;
      });
    }

    //member Subtract two square matrices, one null
    [Fact]
    public void MemberSubtractNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        var a = new FloatMatrix(2);
        FloatMatrix b = null;
        a.Subtract(b);
      });
    }

    //static Subtract two incompatible matrices
    [Fact]
    public void StaticSubtractIncompatible()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new FloatMatrix(2);
        var b = new FloatMatrix(3);
        var c = FloatMatrix.Subtract(a, b);
      });
    }

    //operator Subtract two  incompatible matrices
    [Fact]
    public void OperatorSubtractIncompatible()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new FloatMatrix(2);
        var b = new FloatMatrix(3);
        FloatMatrix c = a - b;
      });
    }

    //member Subtract two  incompatible matricess
    [Fact]
    public void MemberSubtractIncompatible()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new FloatMatrix(2);
        var b = new FloatMatrix(3);
        a.Subtract(b);
      });
    }

    //static add two square matrices
    [Fact]
    public void StaticAdd()
    {
      var a = new FloatMatrix(2);
      var b = new FloatMatrix(2);
      a[0, 0] = b[0, 0] = 1;
      a[0, 1] = b[0, 1] = 2;
      a[1, 0] = b[1, 0] = 3;
      a[1, 1] = b[1, 1] = 4;
      var c = FloatMatrix.Add(a, b);
      Assert.Equal(2, c[0, 0]);
      Assert.Equal(4, c[0, 1]);
      Assert.Equal(6, c[1, 0]);
      Assert.Equal(8, c[1, 1]);
    }

    //operator add two square matrices
    [Fact]
    public void OperatorAdd()
    {
      var a = new FloatMatrix(2);
      var b = new FloatMatrix(2);
      a[0, 0] = b[0, 0] = 1;
      a[0, 1] = b[0, 1] = 2;
      a[1, 0] = b[1, 0] = 3;
      a[1, 1] = b[1, 1] = 4;
      FloatMatrix c = a + b;
      Assert.Equal(2, c[0, 0]);
      Assert.Equal(4, c[0, 1]);
      Assert.Equal(6, c[1, 0]);
      Assert.Equal(8, c[1, 1]);
    }

    //member add two square matrices
    [Fact]
    public void MemberAdd()
    {
      var a = new FloatMatrix(2);
      var b = new FloatMatrix(2);
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
        var a = new FloatMatrix(2);
        FloatMatrix b = null;
        var c = FloatMatrix.Add(a, b);
      });
    }

    //operator add two square matrices, one null
    [Fact]
    public void OperatorAddNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        var a = new FloatMatrix(2);
        FloatMatrix b = null;
        FloatMatrix c = a + b;
      });
    }

    //member add two square matrices, one null
    [Fact]
    public void MemberAddNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        var a = new FloatMatrix(2);
        FloatMatrix b = null;
        a.Add(b);
      });
    }

    //static add two incompatible matrices
    [Fact]
    public void StaticAddIncompatible()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new FloatMatrix(2);
        var b = new FloatMatrix(3);
        var c = FloatMatrix.Add(a, b);
      });
    }

    //operator add two  incompatible matrices
    [Fact]
    public void OperatorAddIncompatible()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new FloatMatrix(2);
        var b = new FloatMatrix(3);
        FloatMatrix c = a + b;
      });
    }

    //member add two  incompatible matricess
    [Fact]
    public void MemberAddIncompatible()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new FloatMatrix(2);
        var b = new FloatMatrix(3);
        a.Add(b);
      });
    }

    //static divide matrix by float
    [Fact]
    public void StaticDivide()
    {
      var a = new FloatMatrix(2)
      {
        [0, 0] = 2,
        [0, 1] = 4,
        [1, 0] = 6,
        [1, 1] = 8
      };
      var b = FloatMatrix.Divide(a, 2);
      Assert.Equal(1, b[0, 0]);
      Assert.Equal(2, b[0, 1]);
      Assert.Equal(3, b[1, 0]);
      Assert.Equal(4, b[1, 1]);
    }

    //operator divide matrix by float
    [Fact]
    public void OperatorDivide()
    {
      var a = new FloatMatrix(2)
      {
        [0, 0] = 2,
        [0, 1] = 4,
        [1, 0] = 6,
        [1, 1] = 8
      };
      FloatMatrix b = a / 2;
      Assert.Equal(1, b[0, 0]);
      Assert.Equal(2, b[0, 1]);
      Assert.Equal(3, b[1, 0]);
      Assert.Equal(4, b[1, 1]);
    }

    //member divide matrix by float
    [Fact]
    public void MemberDivide()
    {
      var a = new FloatMatrix(2)
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

    //static divide null matrix by float
    [Fact]
    public void StaticDivideNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        FloatMatrix a = null;
        var b = FloatMatrix.Divide(a, 2);
      });
    }

    //operator divide null matrix by float
    [Fact]
    public void OperatorDivideNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        FloatMatrix a = null;
        FloatMatrix b = a / 2;
      });
    }

    //copy
    [Fact]
    public void Copy()
    {
      var a = new FloatMatrix(2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };
      var b = new FloatMatrix(2);
      b.Copy(a);
      Assert.Equal(a[0, 0], a[0, 0]);
      Assert.Equal(a[0, 1], b[0, 1]);
      Assert.Equal(a[1, 0], b[1, 0]);
      Assert.Equal(a[1, 1], b[1, 1]);
    }

    //test multiply float matrix operator *
    [Fact]
    public void OperatorMultiplyFloatMatrix()
    {
      var a = new FloatMatrix(2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };
      FloatMatrix b = 2.0f * a;
      Assert.Equal(2, b[0, 0]);
      Assert.Equal(4, b[0, 1]);
      Assert.Equal(6, b[1, 0]);
      Assert.Equal(8, b[1, 1]);
    }

    //test multiply float null matrix operator *
    [Fact]
    public void OperatorMultiplyFloatMatrixNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        FloatMatrix a = null;
        FloatMatrix b = 2.0f * a;
      });
    }

    //test multiply  matrix float operator *
    [Fact]
    public void OperatorMultiplyMatrixFloat()
    {
      var a = new FloatMatrix(2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };
      FloatMatrix b = a * 2.0f;
      Assert.Equal(2, b[0, 0]);
      Assert.Equal(4, b[0, 1]);
      Assert.Equal(6, b[1, 0]);
      Assert.Equal(8, b[1, 1]);
    }

    //test multiply  null matrix float operator *
    [Fact]
    public void OperatorMultiplyMatrixFloatNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        FloatMatrix a = null;
        FloatMatrix b = a * 2;
      });
    }

    //test static multiply float matrix
    [Fact]
    public void StaticMultiplyFloatMatrix()
    {
      var a = new FloatMatrix(2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };
      var b = FloatMatrix.Multiply(2.0f, a);
      Assert.Equal(2, b[0, 0]);
      Assert.Equal(4, b[0, 1]);
      Assert.Equal(6, b[1, 0]);
      Assert.Equal(8, b[1, 1]);
    }

    //test static multiply float null matrix
    [Fact]
    public void StaticMultiplyFloatMatrixNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        FloatMatrix a = null;
        var b = FloatMatrix.Multiply(2.0f, a);
      });
    }

    //test static multiply  matrix float
    [Fact]
    public void StaticMultiplyMatrixFloat()
    {
      var a = new FloatMatrix(2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };
      var b = FloatMatrix.Multiply(a, 2.0f);

      Assert.Equal(2, b[0, 0]);
      Assert.Equal(4, b[0, 1]);
      Assert.Equal(6, b[1, 0]);
      Assert.Equal(8, b[1, 1]);
    }

    //test static multiply  null matrix float operator *
    [Fact]
    public void StaticMultiplyMatrixFloatNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        FloatMatrix a = null;
        var b = FloatMatrix.Multiply(a, 2.0f);
      });
    }

    //test member multiply  float
    [Fact]
    public void MemberMultiplyFloat()
    {
      var a = new FloatMatrix(2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };
      a.Multiply(2.0f);
      Assert.Equal(2, a[0, 0]);
      Assert.Equal(4, a[0, 1]);
      Assert.Equal(6, a[1, 0]);
      Assert.Equal(8, a[1, 1]);
    }

    //test multiply  matrix vector operator *
    [Fact]
    public void OperatorMultiplyMatrixVector()
    {
      var a = new FloatMatrix(2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };
      var b = new FloatVector(2, 2.0f);
      FloatVector c = a * b;
      Assert.Equal(6, c[0]);
      Assert.Equal(14, c[1]);
    }

    //test multiply  matrix nonconform vector operator *
    [Fact]
    public void OperatorMultiplyMatrixNonConformVector()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new FloatMatrix(2);
        var b = new FloatVector(3, 2.0f);
        FloatVector c = a * b;
      });
    }

    //test multiply null matrix vector operator *
    [Fact]
    public void OperatorMultiplyNullMatrixVector()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        FloatMatrix a = null;
        var b = new FloatVector(2, 2.0f);
        FloatVector c = a * b;
      });
    }

    //test multiply matrix null vector operator *
    [Fact]
    public void OperatorMultiplyMatrixNullVector()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        var a = new FloatMatrix(2);
        FloatVector b = null;
        FloatVector c = a * b;
      });
    }

    //test static multiply  matrix vector
    [Fact]
    public void StaticMultiplyMatrixVector()
    {
      var a = new FloatMatrix(2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };
      var b = new FloatVector(2, 2.0f);
      FloatVector c = FloatMatrix.Multiply(a, b);
      Assert.Equal(6, c[0]);
      Assert.Equal(14, c[1]);
    }

    //test static multiply  matrix nonconform vector
    [Fact]
    public void StaticMultiplyMatrixNonConformVector()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var a = new FloatMatrix(2);
        var b = new FloatVector(3, 2.0f);
        FloatVector c = a * b;
      });
    }

    //test static multiply null matrix vector
    [Fact]
    public void StaticMultiplyNullMatrixVector()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        FloatMatrix a = null;
        var b = new FloatVector(2, 2.0f);
        FloatVector c = FloatMatrix.Multiply(a, b);
      });
    }

    //test static multiply matrix null vector
    [Fact]
    public void StaticMultiplyMatrixNullVector()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        var a = new FloatMatrix(2);
        FloatVector b = null;
        FloatVector c = FloatMatrix.Multiply(a, b);
      });
    }

    //test member multiply vector
    [Fact]
    public void MemberMultiplyVector()
    {
      var a = new FloatMatrix(2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };
      var b = new FloatVector(2, 2.0f);
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
        var a = new FloatMatrix(2);
        var b = new FloatVector(3, 2.0f);
        a.Multiply(b);
      });
    }

    //test member multiply null vector
    [Fact]
    public void MemberMultiplyNullVector()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        var a = new FloatMatrix(2);
        FloatVector b = null;
        a.Multiply(b);
      });
    }

    //test multiply  matrix matrix operator *
    [Fact]
    public void OperatorMultiplyMatrixMatrix()
    {
      var a = new FloatMatrix(2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };
      var b = new FloatMatrix(2, 2.0f);
      FloatMatrix c = a * b;
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
        var a = new FloatMatrix(2);
        var b = new FloatMatrix(3, 2, 2.0f);
        FloatMatrix c = a * b;
      });
    }

    //test multiply  long matrix wide matrix operator *
    [Fact]
    public void OperatorMultiplyLongMatrixWideMatrix()
    {
      var a = new FloatMatrix(3, 2, 1);
      var b = new FloatMatrix(2, 3, 2);
      FloatMatrix c = a * b;
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
      var a = new FloatMatrix(2, 3, 1);
      var b = new FloatMatrix(3, 2, 2);
      FloatMatrix c = a * b;
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
        FloatMatrix a = null;
        var b = new FloatMatrix(2, 2);
        FloatMatrix c = a * b;
      });
    }

    //test multiply matrix null matrix operator *
    [Fact]
    public void OperatorMultiplyMatrixNullMatrix()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        var a = new FloatMatrix(2, 2);
        FloatMatrix b = null;
        FloatMatrix c = a * b;
      });
    }

    //test static multiply  matrix matrix
    [Fact]
    public void StaticMultiplyMatrixMatrix()
    {
      var a = new FloatMatrix(2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };
      var b = new FloatMatrix(2, 2, 2.0f);
      var c = FloatMatrix.Multiply(a, b);
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
        var a = new FloatMatrix(2);
        var b = new FloatMatrix(3, 2, 2.0f);
        var c = FloatMatrix.Multiply(a, b);
      });
    }

    //test static multiply  long matrix wide matrix
    [Fact]
    public void StaticMultiplyLongMatrixWideMatrix()
    {
      var a = new FloatMatrix(3, 2, 1);
      var b = new FloatMatrix(2, 3, 2);
      var c = FloatMatrix.Multiply(a, b);
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
      var a = new FloatMatrix(2, 3, 1);
      var b = new FloatMatrix(3, 2, 2);
      var c = FloatMatrix.Multiply(a, b);
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
        FloatMatrix a = null;
        var b = new FloatMatrix(2, 2);
        var c = FloatMatrix.Multiply(a, b);
      });
    }

    //test static multiply matrix null matrix
    [Fact]
    public void StaticMultiplyMatrixNullMatrix()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        var a = new FloatMatrix(2, 2);
        FloatMatrix b = null;
        var c = FloatMatrix.Multiply(a, b);
      });
    }

    //test member multiply  matrix matrix
    [Fact]
    public void MemberMultiplyMatrixMatrix()
    {
      var a = new FloatMatrix(2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };
      var b = new FloatMatrix(2, 2, 2.0f);
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
        var a = new FloatMatrix(2);
        var b = new FloatMatrix(3, 2, 2.0f);
        a.Multiply(b);
      });
    }

    //test member multiply  long matrix wide matrix
    [Fact]
    public void MemberMultiplyLongMatrixWideMatrix()
    {
      var a = new FloatMatrix(3, 2, 1);
      var b = new FloatMatrix(2, 3, 2);
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
      var a = new FloatMatrix(2, 3, 1);
      var b = new FloatMatrix(3, 2, 2);
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
        var a = new FloatMatrix(2, 2);
        FloatMatrix b = null;
        a.Multiply(b);
      });
    }

    //copy null
    [Fact]
    public void CopyNull()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        FloatMatrix a = null;
        var b = new FloatMatrix(2);
        b.Copy(a);
      });
    }

    //clone
    [Fact]
    public void Clone()
    {
      var a = new FloatMatrix(2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };
      var b = a.Clone();
      Assert.Equal(a[0, 0], a[0, 0]);
      Assert.Equal(a[0, 1], b[0, 1]);
      Assert.Equal(a[1, 0], b[1, 0]);
      Assert.Equal(a[1, 1], b[1, 1]);
    }

    //Norm
    [Fact]
    public void Norms()
    {
      var a = new FloatMatrix(2)
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
      var a = new FloatMatrix(2, 3)
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
      var a = new FloatMatrix(3, 2)
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
      var a = new FloatMatrix(2)
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
        var a = new FloatMatrix(2, 3);
        a.GetConditionNumber();
      });
    }

    //Long Condition
    [Fact]
    public void LongCondition()
    {
      Assert.Throws<NotSquareMatrixException>(() =>
      {
        var a = new FloatMatrix(3, 2);
        a.GetConditionNumber();
      });
    }
  }
}
