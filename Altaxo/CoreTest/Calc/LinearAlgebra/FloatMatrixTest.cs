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
  public class FloatMatrixTest
  {
    private const double TOLERENCE = 0.001;

    //Test dimensions Constructor.
    [Test]
    public void CtorDimensions()
    {
      var test = new FloatMatrix(2, 2);
      Assert.AreEqual(test.RowLength, 2);
      Assert.AreEqual(test.ColumnLength, 2);
      Assert.AreEqual(test[0, 0], 0);
      Assert.AreEqual(test[0, 1], 0);
      Assert.AreEqual(test[1, 0], 0);
      Assert.AreEqual(test[1, 1], 0);
    }

    //Test Intital Values Constructor.
    [Test]
    public void CtorInitialValues()
    {
      var test = new FloatMatrix(2, 2, 1);

      Assert.AreEqual(test.RowLength, 2);
      Assert.AreEqual(test.ColumnLength, 2);
      Assert.AreEqual(test[0, 0], 1);
      Assert.AreEqual(test[0, 1], 1);
      Assert.AreEqual(test[1, 0], 1);
      Assert.AreEqual(test[1, 1], 1);
    }

    //Test Copy Constructor.
    [Test]
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

      Assert.AreEqual(a.RowLength, b.RowLength);
      Assert.AreEqual(a.ColumnLength, b.ColumnLength);
      Assert.AreEqual(a[0, 0], a[0, 0]);
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
        FloatMatrix a = null;
        var b = new FloatMatrix(a);
      });
    }

    //Test Multiple Dimensional FloatArray Constructor with Square array.
    [Test]
    public void CtorMultDimFloatSquare()
    {
      float[,] values = new float[2, 2];

      values[0, 0] = 0;
      values[0, 1] = 1;
      values[1, 0] = 3;
      values[1, 1] = 4;

      var test = new FloatMatrix(values);

      Assert.AreEqual(test.RowLength, 2);
      Assert.AreEqual(test.ColumnLength, 2);
      Assert.AreEqual(test[0, 0], values[0, 0]);
      Assert.AreEqual(test[0, 1], values[0, 1]);
      Assert.AreEqual(test[1, 0], values[1, 0]);
      Assert.AreEqual(test[1, 1], values[1, 1]);
    }

    //Test Multiple Dimensional FloatArray Constructor with wide array.
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

      var test = new FloatMatrix(values);

      Assert.AreEqual(test.RowLength, 2);
      Assert.AreEqual(test.ColumnLength, 3);
      Assert.AreEqual(test[0, 0], values[0, 0]);
      Assert.AreEqual(test[0, 1], values[0, 1]);
      Assert.AreEqual(test[0, 2], values[0, 2]);
      Assert.AreEqual(test[1, 0], values[1, 0]);
      Assert.AreEqual(test[1, 1], values[1, 1]);
      Assert.AreEqual(test[1, 2], values[1, 2]);
    }

    //Test Multiple Dimensional FloatArray Constructor with long array.
    [Test]
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

      Assert.AreEqual(test.RowLength, 3);
      Assert.AreEqual(test.ColumnLength, 2);
      Assert.AreEqual(test[0, 0], values[0, 0]);
      Assert.AreEqual(test[0, 1], values[0, 1]);
      Assert.AreEqual(test[1, 0], values[1, 0]);
      Assert.AreEqual(test[1, 1], values[1, 1]);
      Assert.AreEqual(test[2, 0], values[2, 0]);
      Assert.AreEqual(test[2, 1], values[2, 1]);
    }

    //Test Multiple Dimensional Float Array Constructor with null.
    [Test]
    public void CtorMultDimFloatNull()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        float[,] values = null;
        var test = new FloatMatrix(values);
      });
    }

    //Test Jagged Array  Constructor with null.
    [Test]
    public void CtorJaggedNull()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        float[,] values = null;
        var test = new FloatMatrix(values);
      });
    }

    //Test explicit conversion from DoubleMatrix
    [Test]
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
      Assert.AreEqual(a.RowLength, b.RowLength);
      Assert.AreEqual(a.ColumnLength, b.ColumnLength);
      Assert.AreEqual(a[0, 0], b[0, 0]);
      Assert.AreEqual(a[0, 1], b[0, 1]);
      Assert.AreEqual(a[1, 0], b[1, 0]);
      Assert.AreEqual(a[1, 1], b[1, 1]);
    }

    //Test explicit conversion from DoubleMatrix
    [Test]
    public void ExplicitDoubleMatrixNull()
    {
      DoubleMatrix a = null;
      var b = (FloatMatrix)a;
      Assert.IsTrue(b == null);
    }

    //Test explicit conversion from DoubleMatrix
    [Test]
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
      Assert.AreEqual(a.RowLength, b.RowLength);
      Assert.AreEqual(a.ColumnLength, b.ColumnLength);
      Assert.AreEqual(a[0, 0], b[0, 0]);
      Assert.AreEqual(a[0, 1], b[0, 1]);
      Assert.AreEqual(a[1, 0], b[1, 0]);
      Assert.AreEqual(a[1, 1], b[1, 1]);
    }

    //Test explicit conversion from DoubleMatrix
    [Test]
    public void ExplicitToDoubleMatrixNull()
    {
      DoubleMatrix a = null;
      var b = FloatMatrix.ToFloatMatrix(a);
      Assert.IsTrue(b == null);
    }

    //Test explicit conversion from double array
    [Test]
    public void ExplicitDoubleMultArray()
    {
      double[,] a = new double[2, 2];
      a[0, 0] = 1;
      a[0, 1] = 2;
      a[1, 0] = 3;
      a[1, 1] = 4;

      var b = (FloatMatrix)a;
      Assert.AreEqual(2, b.RowLength);
      Assert.AreEqual(2, b.ColumnLength);
      Assert.AreEqual(a[0, 0], b[0, 0]);
      Assert.AreEqual(a[0, 1], b[0, 1]);
      Assert.AreEqual(a[1, 0], b[1, 0]);
      Assert.AreEqual(a[1, 1], b[1, 1]);
    }

    //Test explicit conversion from double array
    [Test]
    public void ExplicitDoubleMultArrayNull()
    {
      double[,] a = null;
      var b = (FloatMatrix)a;
      Assert.IsTrue(b == null);
    }

    //Test explicit conversion from double array
    [Test]
    public void ExplicitToDoubleMultArray()
    {
      double[,] a = new double[2, 2];
      a[0, 0] = 1;
      a[0, 1] = 2;
      a[1, 0] = 3;
      a[1, 1] = 4;

      var b = FloatMatrix.ToFloatMatrix(a);
      Assert.AreEqual(2, b.RowLength);
      Assert.AreEqual(2, b.ColumnLength);
      Assert.AreEqual(a[0, 0], b[0, 0]);
      Assert.AreEqual(a[0, 1], b[0, 1]);
      Assert.AreEqual(a[1, 0], b[1, 0]);
      Assert.AreEqual(a[1, 1], b[1, 1]);
    }

    //Test explicit conversion from double array
    [Test]
    public void ExplicitToDoubleMultArrayNull()
    {
      double[,] a = null;
      var b = FloatMatrix.ToFloatMatrix(a);
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

      FloatMatrix b = a;
      Assert.AreEqual(a.RowLength, b.RowLength);
      Assert.AreEqual(a.ColumnLength, b.ColumnLength);
      Assert.AreEqual(a[0, 0], b[0, 0]);
      Assert.AreEqual(a[0, 1], b[0, 1]);
      Assert.AreEqual(a[1, 0], b[1, 0]);
      Assert.AreEqual(a[1, 1], b[1, 1]);
    }

    //Test implicit conversion from null floatmatrix.
    [Test]
    public void ImplictFloatMatrixNull()
    {
      FloatMatrix a = null;
      FloatMatrix b = a;
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

      var b = FloatMatrix.ToFloatMatrix(a);
      Assert.AreEqual(a.RowLength, b.RowLength);
      Assert.AreEqual(a.ColumnLength, b.ColumnLength);
      Assert.AreEqual(a[0, 0], b[0, 0]);
      Assert.AreEqual(a[0, 1], b[0, 1]);
      Assert.AreEqual(a[1, 0], b[1, 0]);
      Assert.AreEqual(a[1, 1], b[1, 1]);
    }

    //Test implicit conversion from null floatmatrix.
    [Test]
    public void ImplictToFloatMatrixNull()
    {
      FloatMatrix a = null;
      var b = FloatMatrix.ToFloatMatrix(a);
      Assert.IsTrue(b == null);
    }

    //Test implicit conversion from float mult dim array.
    [Test]
    public void ImplictFloatMultArray()
    {
      float[,] a = new float[2, 2];
      a[0, 0] = 1;
      a[0, 1] = 2;
      a[1, 0] = 3;
      a[1, 1] = 4;

      FloatMatrix b = a;
      Assert.AreEqual(a[0, 0], b[0, 0]);
      Assert.AreEqual(a[0, 1], b[0, 1]);
      Assert.AreEqual(a[1, 0], b[1, 0]);
      Assert.AreEqual(a[1, 1], b[1, 1]);
    }

    //Test implicit conversion from null float mult dim array.
    [Test]
    public void ImplictFloatMultArrayNull()
    {
      float[,] a = null;
      FloatMatrix b = a;
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

      var b = FloatMatrix.ToFloatMatrix(a);
      Assert.AreEqual(a[0, 0], b[0, 0]);
      Assert.AreEqual(a[0, 1], b[0, 1]);
      Assert.AreEqual(a[1, 0], b[1, 0]);
      Assert.AreEqual(a[1, 1], b[1, 1]);
    }

    //Test implicit conversion from null float mult dim array.
    [Test]
    public void ImplictToFloatMultArrayNull()
    {
      float[,] a = null;
      var b = FloatMatrix.ToFloatMatrix(a);
      Assert.IsTrue(b == null);
    }

    //test equals method
    [Test]
    public void Equals()
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
      var a = new FloatMatrix(2)
      {
        [0, 0] = 1,
        [0, 1] = 2,
        [1, 0] = 3,
        [1, 1] = 4
      };

      int hash = a.GetHashCode();
      Assert.AreEqual(hash, 5);
    }

    //test ToArray
    [Test]
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

      Assert.AreEqual(a[0, 0], b[0, 0]);
      Assert.AreEqual(a[0, 1], b[0, 1]);
      Assert.AreEqual(a[1, 0], b[1, 0]);
      Assert.AreEqual(a[1, 1], b[1, 1]);
    }

    //test Transpose square
    [Test]
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
      Assert.AreEqual(a[0, 0], 1);
      Assert.AreEqual(a[0, 1], 3);
      Assert.AreEqual(a[1, 0], 2);
      Assert.AreEqual(a[1, 1], 4);
    }

    //test Transpose wide
    [Test]
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
      Assert.AreEqual(a[0, 0], 1);
      Assert.AreEqual(a[0, 1], 4);
      Assert.AreEqual(a[1, 0], 2);
      Assert.AreEqual(a[1, 1], 5);
      Assert.AreEqual(a[2, 0], 3);
      Assert.AreEqual(a[2, 1], 6);
      Assert.AreEqual(a.RowLength, 3);
      Assert.AreEqual(a.ColumnLength, 2);
    }

    //test Transpose long
    [Test]
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
      Assert.AreEqual(a[0, 0], 1);
      Assert.AreEqual(a[0, 1], 3);
      Assert.AreEqual(a[0, 2], 5);
      Assert.AreEqual(a[1, 0], 2);
      Assert.AreEqual(a[1, 1], 4);
      Assert.AreEqual(a[1, 2], 6);
      Assert.AreEqual(a.RowLength, 2);
      Assert.AreEqual(a.ColumnLength, 3);
    }

    //test GetTranspose square
    [Test]
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
      Assert.AreEqual(b[0, 0], 1);
      Assert.AreEqual(b[0, 1], 3);
      Assert.AreEqual(b[1, 0], 2);
      Assert.AreEqual(b[1, 1], 4);
    }

    //test GetTranspose wide
    [Test]
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
      Assert.AreEqual(b[0, 0], 1);
      Assert.AreEqual(b[0, 1], 4);
      Assert.AreEqual(b[1, 0], 2);
      Assert.AreEqual(b[1, 1], 5);
      Assert.AreEqual(b[2, 0], 3);
      Assert.AreEqual(b[2, 1], 6);
      Assert.AreEqual(b.RowLength, a.ColumnLength);
      Assert.AreEqual(b.ColumnLength, a.RowLength);
    }

    //test GetTranspose long
    [Test]
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
      Assert.AreEqual(b[0, 0], 1);
      Assert.AreEqual(b[0, 1], 3);
      Assert.AreEqual(b[0, 2], 5);
      Assert.AreEqual(b[1, 0], 2);
      Assert.AreEqual(b[1, 1], 4);
      Assert.AreEqual(b[1, 2], 6);
      Assert.AreEqual(b.RowLength, a.ColumnLength);
      Assert.AreEqual(b.ColumnLength, a.RowLength);
    }

    //test Invert
    [Test]
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
      Assert.AreEqual(a[0, 0], 3.500, TOLERENCE);
      Assert.AreEqual(a[0, 1], -2.00, TOLERENCE);
      Assert.AreEqual(a[1, 0], -1.500, TOLERENCE);
      Assert.AreEqual(a[1, 1], 1.000, TOLERENCE);
    }

    //test Invert singular
    [Test]
    public void InvertSingular()
    {
      Assert.Throws(typeof(SingularMatrixException), () =>
      {
        var a = new FloatMatrix(2, 2);
        a.Invert();
      });
    }

    //test Invert not square
    [Test]
    public void InvertNotSquare()
    {
      Assert.Throws(typeof(NotSquareMatrixException), () =>
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
    [Test]
    public void GetInverseSingular()
    {
      Assert.Throws(typeof(SingularMatrixException), () =>
      {
        var a = new FloatMatrix(2, 2);
        FloatMatrix b = a.GetInverse();
      });
    }

    //test GetInverse not square
    [Test]
    public void GetInverseNotSquare()
    {
      Assert.Throws(typeof(NotSquareMatrixException), () =>
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
    [Test]
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
      Assert.AreEqual(b[0, 0], 3.500, TOLERENCE);
      Assert.AreEqual(b[0, 1], -2.000, TOLERENCE);
      Assert.AreEqual(b[1, 0], -1.500, TOLERENCE);
      Assert.AreEqual(b[1, 1], 1.000, TOLERENCE);
    }

    //test GetDeterminant
    [Test]
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
      Assert.AreEqual(b, 2.000, TOLERENCE);
    }

    //test GetDeterminant
    [Test]
    public void GetDeterminantNotSquare()
    {
      Assert.Throws(typeof(NotSquareMatrixException), () =>
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
    [Test]
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
      Assert.AreEqual(b[0], a[0, 0]);
      Assert.AreEqual(b[1], a[0, 1]);
    }

    //test GetRow
    [Test]
    public void GetRowOutOfRange()
    {
      Assert.Throws(typeof(ArgumentOutOfRangeException), () =>
      {
        var a = new FloatMatrix(2, 2);
        FloatVector b = a.GetRow(3);
      });
    }

    //test GetColumn
    [Test]
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
      Assert.AreEqual(b[0], a[0, 0]);
      Assert.AreEqual(b[1], a[1, 0]);
    }

    //test GetColumn
    [Test]
    public void GetColumnOutOfRange()
    {
      Assert.Throws(typeof(ArgumentOutOfRangeException), () =>
      {
        var a = new FloatMatrix(2, 2);
        FloatVector b = a.GetColumn(3);
      });
    }

    //test GetDiagonal
    [Test]
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
      Assert.AreEqual(b[0], a[0, 0]);
      Assert.AreEqual(b[1], a[1, 1]);
    }

    //test SetRow
    [Test]
    public void SetRow()
    {
      var a = new FloatMatrix(2, 2);
      var b = new FloatVector(2)
      {
        [0] = 1,
        [1] = 2
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
        var a = new FloatMatrix(2, 2);
        var b = new FloatVector(2);
        a.SetRow(2, b);
      });
    }

    //test SetRow
    [Test]
    public void SetRowWrongRank()
    {
      Assert.Throws(typeof(ArgumentException), () =>
      {
        var a = new FloatMatrix(2, 2);
        var b = new FloatVector(3);
        a.SetRow(1, b);
      });
    }

    //test SetRow
    [Test]
    public void SetRowArray()
    {
      var a = new FloatMatrix(2, 2);
      float[] b = { 1, 2 };
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
        var a = new FloatMatrix(2, 2);
        float[] b = { 1, 2 };
        a.SetRow(2, b);
      });
    }

    //test SetRow
    [Test]
    public void SetRowArrayWrongRank()
    {
      Assert.Throws(typeof(ArgumentException), () =>
      {
        var a = new FloatMatrix(2, 2);
        float[] b = { 1, 2, 3 };
        a.SetRow(1, b);
      });
    }

    //test SetColumn
    [Test]
    public void SetColumn()
    {
      var a = new FloatMatrix(2, 2);
      var b = new FloatVector(2)
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
        var a = new FloatMatrix(2, 2);
        var b = new FloatVector(2);
        a.SetColumn(2, b);
      });
    }

    //test SetColumn
    [Test]
    public void SetColumnWrongRank()
    {
      Assert.Throws(typeof(ArgumentException), () =>
      {
        var a = new FloatMatrix(2, 2);
        var b = new FloatVector(3);
        a.SetColumn(1, b);
      });
    }

    //test SetColumn
    [Test]
    public void SetColumnArray()
    {
      var a = new FloatMatrix(2, 2);
      float[] b = { 1, 2 };
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
        var a = new FloatMatrix(2, 2);
        float[] b = { 1, 2 };
        a.SetColumn(2, b);
      });
    }

    //test SetColumn
    [Test]
    public void SetColumnArrayWrongRank()
    {
      Assert.Throws(typeof(ArgumentException), () =>
      {
        var a = new FloatMatrix(2, 2);
        float[] b = { 1, 2, 3 };
        a.SetColumn(1, b);
      });
    }

    //test SetDiagonal
    [Test]
    public void SetDiagonal()
    {
      var a = new FloatMatrix(2, 2);
      var b = new FloatVector(2)
      {
        [0] = 1,
        [1] = 2
      };
      a.SetDiagonal(b);
      Assert.AreEqual(b[0], a[0, 0]);
      Assert.AreEqual(b[1], a[1, 1]);
    }

    //test GetSubMatrix
    [Test]
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
        var a = new FloatMatrix(4);
        FloatMatrix b = a.GetSubMatrix(-1, 2);
      });
    }

    //test GetSubMatrix
    [Test]
    public void GetSubMatrixOutRange2()
    {
      Assert.Throws(typeof(ArgumentOutOfRangeException), () =>
      {
        var a = new FloatMatrix(4);
        FloatMatrix b = a.GetSubMatrix(2, 4);
      });
    }

    //test GetSubMatrix
    [Test]
    public void GetSubMatrixOutRange3()
    {
      Assert.Throws(typeof(ArgumentOutOfRangeException), () =>
      {
        var a = new FloatMatrix(4);
        FloatMatrix b = a.GetSubMatrix(0, 0, 4, 2);
      });
    }

    //test GetSubMatrix
    [Test]
    public void GetSubMatrixOutRange4()
    {
      Assert.Throws(typeof(ArgumentOutOfRangeException), () =>
      {
        var a = new FloatMatrix(4);
        FloatMatrix b = a.GetSubMatrix(0, 0, 2, 4);
      });
    }

    //test GetSubMatrix
    [Test]
    public void GetSubMatrixOutRange5()
    {
      Assert.Throws(typeof(ArgumentOutOfRangeException), () =>
      {
        var a = new FloatMatrix(4);
        FloatMatrix b = a.GetSubMatrix(0, 3, 2, 2);
      });
    }

    //test GetUpperTriangle square matrix
    [Test]
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

      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0, 0], a[0, 0]);
      Assert.AreEqual(b[0, 1], a[0, 1]);
      Assert.AreEqual(b[0, 2], a[0, 2]);
      Assert.AreEqual(b[1, 0], 0);
      Assert.AreEqual(b[1, 1], a[1, 1]);
      Assert.AreEqual(b[1, 2], a[1, 2]);
      Assert.AreEqual(b[2, 0], 0);
      Assert.AreEqual(b[2, 1], 0);
      Assert.AreEqual(b[2, 2], a[2, 2]);
    }

    //test GetUpperTriangle long matrix
    [Test]
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

      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0, 0], a[0, 0]);
      Assert.AreEqual(b[0, 1], a[0, 1]);
      Assert.AreEqual(b[1, 0], 0);
      Assert.AreEqual(b[1, 1], a[1, 1]);
      Assert.AreEqual(b[2, 0], 0);
      Assert.AreEqual(b[2, 1], 0);
    }

    //test GetUpperTriangle wide matrix
    [Test]
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

      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0, 0], a[0, 0]);
      Assert.AreEqual(b[0, 1], a[0, 1]);
      Assert.AreEqual(b[0, 2], a[0, 2]);
      Assert.AreEqual(b[1, 0], 0);
      Assert.AreEqual(b[1, 1], a[1, 1]);
      Assert.AreEqual(b[1, 2], a[1, 2]);
    }

    //test GetStrictlyUpperTriangle square matrix
    [Test]
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

      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0, 0], 0);
      Assert.AreEqual(b[0, 1], a[0, 1]);
      Assert.AreEqual(b[0, 2], a[0, 2]);
      Assert.AreEqual(b[1, 0], 0);
      Assert.AreEqual(b[1, 1], 0);
      Assert.AreEqual(b[1, 2], a[1, 2]);
      Assert.AreEqual(b[2, 0], 0);
      Assert.AreEqual(b[2, 1], 0);
      Assert.AreEqual(b[2, 2], 0);
    }

    //test GetStrictlyUpperTriangle long matrix
    [Test]
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

      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0, 0], 0);
      Assert.AreEqual(b[0, 1], a[0, 1]);
      Assert.AreEqual(b[1, 0], 0);
      Assert.AreEqual(b[1, 1], 0);
      Assert.AreEqual(b[2, 0], 0);
      Assert.AreEqual(b[2, 1], 0);
    }

    //test GetStrictlyUpperTriangle wide matrix
    [Test]
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

      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0, 0], 0);
      Assert.AreEqual(b[0, 1], a[0, 1]);
      Assert.AreEqual(b[0, 2], a[0, 2]);
      Assert.AreEqual(b[1, 0], 0);
      Assert.AreEqual(b[1, 1], 0);
      Assert.AreEqual(b[1, 2], a[1, 2]);
    }

    //test GetLowerTriangle square matrix
    [Test]
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

      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0, 0], a[0, 0]);
      Assert.AreEqual(b[0, 1], 0);
      Assert.AreEqual(b[0, 2], 0);
      Assert.AreEqual(b[1, 0], a[1, 0]);
      Assert.AreEqual(b[1, 1], a[1, 1]);
      Assert.AreEqual(b[1, 2], 0);
      Assert.AreEqual(b[2, 0], a[2, 0]);
      Assert.AreEqual(b[2, 1], a[2, 1]);
      Assert.AreEqual(b[2, 2], a[2, 2]);
    }

    //test GetLowerTriangle long matrix
    [Test]
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

      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0, 0], a[0, 0]);
      Assert.AreEqual(b[0, 1], 0);
      Assert.AreEqual(b[1, 0], b[1, 0]);
      Assert.AreEqual(b[1, 1], a[1, 1]);
      Assert.AreEqual(b[2, 0], b[2, 0]);
      Assert.AreEqual(b[2, 1], b[2, 1]);
    }

    //test GetLowerTriangle wide matrix
    [Test]
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

      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0, 0], a[0, 0]);
      Assert.AreEqual(b[0, 1], 0);
      Assert.AreEqual(b[0, 2], 0);
      Assert.AreEqual(b[1, 0], a[1, 0]);
      Assert.AreEqual(b[1, 1], a[1, 1]);
      Assert.AreEqual(b[1, 2], 0);
    }

    //test GetStrictlyLowerTriangle square matrix
    [Test]
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

      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0, 0], 0);
      Assert.AreEqual(b[0, 1], 0);
      Assert.AreEqual(b[0, 2], 0);
      Assert.AreEqual(b[1, 0], a[1, 0]);
      Assert.AreEqual(b[1, 1], 0);
      Assert.AreEqual(b[1, 2], 0);
      Assert.AreEqual(b[2, 0], a[2, 0]);
      Assert.AreEqual(b[2, 1], a[2, 1]);
      Assert.AreEqual(b[2, 2], 0);
    }

    //test GetStrictlyLowerTriangle long matrix
    [Test]
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

      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0, 0], 0);
      Assert.AreEqual(b[0, 1], 0);
      Assert.AreEqual(b[1, 0], b[1, 0]);
      Assert.AreEqual(b[1, 1], 0);
      Assert.AreEqual(b[2, 0], b[2, 0]);
      Assert.AreEqual(b[2, 1], b[2, 1]);
    }

    //test GetStrictlyLowerTriangle wide matrix
    [Test]
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

      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0, 0], 0);
      Assert.AreEqual(b[0, 1], 0);
      Assert.AreEqual(b[0, 2], 0);
      Assert.AreEqual(b[1, 0], a[1, 0]);
      Assert.AreEqual(b[1, 1], 0);
      Assert.AreEqual(b[1, 2], 0);
    }

    //static Negate
    [Test]
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
      Assert.AreEqual(b[0, 0], -1);
      Assert.AreEqual(b[0, 1], -2);
      Assert.AreEqual(b[1, 0], -3);
      Assert.AreEqual(b[1, 1], -4);
    }

    //static NegateNull
    [Test]
    public void NegateNull()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        FloatMatrix a = null;
        var b = FloatMatrix.Negate(a);
      });
    }

    //static operator -
    [Test]
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
      Assert.AreEqual(b[0, 0], -1);
      Assert.AreEqual(b[0, 1], -2);
      Assert.AreEqual(b[1, 0], -3);
      Assert.AreEqual(b[1, 1], -4);
    }

    //static operator - null
    [Test]
    public void OperatorMinusNull()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        FloatMatrix a = null;
        FloatMatrix b = -a;
      });
    }

    //static subtact two square matrices
    [Test]
    public void StaticSubtract()
    {
      var a = new FloatMatrix(2);
      var b = new FloatMatrix(2);
      a[0, 0] = b[0, 0] = 1;
      a[0, 1] = b[0, 1] = 2;
      a[1, 0] = b[1, 0] = 3;
      a[1, 1] = b[1, 1] = 4;
      var c = FloatMatrix.Subtract(a, b);
      Assert.AreEqual(c[0, 0], 0);
      Assert.AreEqual(c[0, 1], 0);
      Assert.AreEqual(c[1, 0], 0);
      Assert.AreEqual(c[1, 1], 0);
    }

    //operator subtract two square matrices
    [Test]
    public void OperatorSubtract()
    {
      var a = new FloatMatrix(2);
      var b = new FloatMatrix(2);
      a[0, 0] = b[0, 0] = 1;
      a[0, 1] = b[0, 1] = 2;
      a[1, 0] = b[1, 0] = 3;
      a[1, 1] = b[1, 1] = 4;
      FloatMatrix c = a - b;
      Assert.AreEqual(c[0, 0], 0);
      Assert.AreEqual(c[0, 1], 0);
      Assert.AreEqual(c[1, 0], 0);
      Assert.AreEqual(c[1, 1], 0);
    }

    //member add subtract square matrices
    [Test]
    public void MemberSubtract()
    {
      var a = new FloatMatrix(2);
      var b = new FloatMatrix(2);
      a[0, 0] = b[0, 0] = 1;
      a[0, 1] = b[0, 1] = 2;
      a[1, 0] = b[1, 0] = 3;
      a[1, 1] = b[1, 1] = 4;
      a.Subtract(b);
      Assert.AreEqual(a[0, 0], 0);
      Assert.AreEqual(a[0, 1], 0);
      Assert.AreEqual(a[1, 0], 0);
      Assert.AreEqual(a[1, 1], 0);
    }

    //static Subtract two square matrices, one null
    [Test]
    public void StaticSubtractNull()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        var a = new FloatMatrix(2);
        FloatMatrix b = null;
        var c = FloatMatrix.Subtract(a, b);
      });
    }

    //operator Subtract two square matrices, one null
    [Test]
    public void OperatorSubtractNull()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        var a = new FloatMatrix(2);
        FloatMatrix b = null;
        FloatMatrix c = a - b;
      });
    }

    //member Subtract two square matrices, one null
    [Test]
    public void MemberSubtractNull()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        var a = new FloatMatrix(2);
        FloatMatrix b = null;
        a.Subtract(b);
      });
    }

    //static Subtract two incompatible matrices
    [Test]
    public void StaticSubtractIncompatible()
    {
      Assert.Throws(typeof(ArgumentException), () =>
      {
        var a = new FloatMatrix(2);
        var b = new FloatMatrix(3);
        var c = FloatMatrix.Subtract(a, b);
      });
    }

    //operator Subtract two  incompatible matrices
    [Test]
    public void OperatorSubtractIncompatible()
    {
      Assert.Throws(typeof(ArgumentException), () =>
      {
        var a = new FloatMatrix(2);
        var b = new FloatMatrix(3);
        FloatMatrix c = a - b;
      });
    }

    //member Subtract two  incompatible matricess
    [Test]
    public void MemberSubtractIncompatible()
    {
      Assert.Throws(typeof(ArgumentException), () =>
      {
        var a = new FloatMatrix(2);
        var b = new FloatMatrix(3);
        a.Subtract(b);
      });
    }

    //static add two square matrices
    [Test]
    public void StaticAdd()
    {
      var a = new FloatMatrix(2);
      var b = new FloatMatrix(2);
      a[0, 0] = b[0, 0] = 1;
      a[0, 1] = b[0, 1] = 2;
      a[1, 0] = b[1, 0] = 3;
      a[1, 1] = b[1, 1] = 4;
      var c = FloatMatrix.Add(a, b);
      Assert.AreEqual(c[0, 0], 2);
      Assert.AreEqual(c[0, 1], 4);
      Assert.AreEqual(c[1, 0], 6);
      Assert.AreEqual(c[1, 1], 8);
    }

    //operator add two square matrices
    [Test]
    public void OperatorAdd()
    {
      var a = new FloatMatrix(2);
      var b = new FloatMatrix(2);
      a[0, 0] = b[0, 0] = 1;
      a[0, 1] = b[0, 1] = 2;
      a[1, 0] = b[1, 0] = 3;
      a[1, 1] = b[1, 1] = 4;
      FloatMatrix c = a + b;
      Assert.AreEqual(c[0, 0], 2);
      Assert.AreEqual(c[0, 1], 4);
      Assert.AreEqual(c[1, 0], 6);
      Assert.AreEqual(c[1, 1], 8);
    }

    //member add two square matrices
    [Test]
    public void MemberAdd()
    {
      var a = new FloatMatrix(2);
      var b = new FloatMatrix(2);
      a[0, 0] = b[0, 0] = 1;
      a[0, 1] = b[0, 1] = 2;
      a[1, 0] = b[1, 0] = 3;
      a[1, 1] = b[1, 1] = 4;
      a.Add(b);
      Assert.AreEqual(a[0, 0], 2);
      Assert.AreEqual(a[0, 1], 4);
      Assert.AreEqual(a[1, 0], 6);
      Assert.AreEqual(a[1, 1], 8);
    }

    //static add two square matrices, one null
    [Test]
    public void StaticAddNull()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        var a = new FloatMatrix(2);
        FloatMatrix b = null;
        var c = FloatMatrix.Add(a, b);
      });
    }

    //operator add two square matrices, one null
    [Test]
    public void OperatorAddNull()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        var a = new FloatMatrix(2);
        FloatMatrix b = null;
        FloatMatrix c = a + b;
      });
    }

    //member add two square matrices, one null
    [Test]
    public void MemberAddNull()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        var a = new FloatMatrix(2);
        FloatMatrix b = null;
        a.Add(b);
      });
    }

    //static add two incompatible matrices
    [Test]
    public void StaticAddIncompatible()
    {
      Assert.Throws(typeof(ArgumentException), () =>
      {
        var a = new FloatMatrix(2);
        var b = new FloatMatrix(3);
        var c = FloatMatrix.Add(a, b);
      });
    }

    //operator add two  incompatible matrices
    [Test]
    public void OperatorAddIncompatible()
    {
      Assert.Throws(typeof(ArgumentException), () =>
      {
        var a = new FloatMatrix(2);
        var b = new FloatMatrix(3);
        FloatMatrix c = a + b;
      });
    }

    //member add two  incompatible matricess
    [Test]
    public void MemberAddIncompatible()
    {
      Assert.Throws(typeof(ArgumentException), () =>
      {
        var a = new FloatMatrix(2);
        var b = new FloatMatrix(3);
        a.Add(b);
      });
    }

    //static divide matrix by float
    [Test]
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
      Assert.AreEqual(b[0, 0], 1);
      Assert.AreEqual(b[0, 1], 2);
      Assert.AreEqual(b[1, 0], 3);
      Assert.AreEqual(b[1, 1], 4);
    }

    //operator divide matrix by float
    [Test]
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
      Assert.AreEqual(b[0, 0], 1);
      Assert.AreEqual(b[0, 1], 2);
      Assert.AreEqual(b[1, 0], 3);
      Assert.AreEqual(b[1, 1], 4);
    }

    //member divide matrix by float
    [Test]
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
      Assert.AreEqual(a[0, 0], 1);
      Assert.AreEqual(a[0, 1], 2);
      Assert.AreEqual(a[1, 0], 3);
      Assert.AreEqual(a[1, 1], 4);
    }

    //static divide null matrix by float
    [Test]
    public void StaticDivideNull()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        FloatMatrix a = null;
        var b = FloatMatrix.Divide(a, 2);
      });
    }

    //operator divide null matrix by float
    [Test]
    public void OperatorDivideNull()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        FloatMatrix a = null;
        FloatMatrix b = a / 2;
      });
    }

    //copy
    [Test]
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
      Assert.AreEqual(a[0, 0], a[0, 0]);
      Assert.AreEqual(a[0, 1], b[0, 1]);
      Assert.AreEqual(a[1, 0], b[1, 0]);
      Assert.AreEqual(a[1, 1], b[1, 1]);
    }

    //test multiply float matrix operator *
    [Test]
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
      Assert.AreEqual(b[0, 0], 2);
      Assert.AreEqual(b[0, 1], 4);
      Assert.AreEqual(b[1, 0], 6);
      Assert.AreEqual(b[1, 1], 8);
    }

    //test multiply float null matrix operator *
    [Test]
    public void OperatorMultiplyFloatMatrixNull()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        FloatMatrix a = null;
        FloatMatrix b = 2.0f * a;
      });
    }

    //test multiply  matrix float operator *
    [Test]
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
      Assert.AreEqual(b[0, 0], 2);
      Assert.AreEqual(b[0, 1], 4);
      Assert.AreEqual(b[1, 0], 6);
      Assert.AreEqual(b[1, 1], 8);
    }

    //test multiply  null matrix float operator *
    [Test]
    public void OperatorMultiplyMatrixFloatNull()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        FloatMatrix a = null;
        FloatMatrix b = a * 2;
      });
    }

    //test static multiply float matrix
    [Test]
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
      Assert.AreEqual(b[0, 0], 2);
      Assert.AreEqual(b[0, 1], 4);
      Assert.AreEqual(b[1, 0], 6);
      Assert.AreEqual(b[1, 1], 8);
    }

    //test static multiply float null matrix
    [Test]
    public void StaticMultiplyFloatMatrixNull()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        FloatMatrix a = null;
        var b = FloatMatrix.Multiply(2.0f, a);
      });
    }

    //test static multiply  matrix float
    [Test]
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

      Assert.AreEqual(b[0, 0], 2);
      Assert.AreEqual(b[0, 1], 4);
      Assert.AreEqual(b[1, 0], 6);
      Assert.AreEqual(b[1, 1], 8);
    }

    //test static multiply  null matrix float operator *
    [Test]
    public void StaticMultiplyMatrixFloatNull()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        FloatMatrix a = null;
        var b = FloatMatrix.Multiply(a, 2.0f);
      });
    }

    //test member multiply  float
    [Test]
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
      Assert.AreEqual(a[0, 0], 2);
      Assert.AreEqual(a[0, 1], 4);
      Assert.AreEqual(a[1, 0], 6);
      Assert.AreEqual(a[1, 1], 8);
    }

    //test multiply  matrix vector operator *
    [Test]
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
      Assert.AreEqual(c[0], 6);
      Assert.AreEqual(c[1], 14);
    }

    //test multiply  matrix nonconform vector operator *
    [Test]
    public void OperatorMultiplyMatrixNonConformVector()
    {
      Assert.Throws(typeof(ArgumentException), () =>
      {
        var a = new FloatMatrix(2);
        var b = new FloatVector(3, 2.0f);
        FloatVector c = a * b;
      });
    }

    //test multiply null matrix vector operator *
    [Test]
    public void OperatorMultiplyNullMatrixVector()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        FloatMatrix a = null;
        var b = new FloatVector(2, 2.0f);
        FloatVector c = a * b;
      });
    }

    //test multiply matrix null vector operator *
    [Test]
    public void OperatorMultiplyMatrixNullVector()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        var a = new FloatMatrix(2);
        FloatVector b = null;
        FloatVector c = a * b;
      });
    }

    //test static multiply  matrix vector
    [Test]
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
      Assert.AreEqual(c[0], 6);
      Assert.AreEqual(c[1], 14);
    }

    //test static multiply  matrix nonconform vector
    [Test]
    public void StaticMultiplyMatrixNonConformVector()
    {
      Assert.Throws(typeof(ArgumentException), () =>
      {
        var a = new FloatMatrix(2);
        var b = new FloatVector(3, 2.0f);
        FloatVector c = a * b;
      });
    }

    //test static multiply null matrix vector
    [Test]
    public void StaticMultiplyNullMatrixVector()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        FloatMatrix a = null;
        var b = new FloatVector(2, 2.0f);
        FloatVector c = FloatMatrix.Multiply(a, b);
      });
    }

    //test static multiply matrix null vector
    [Test]
    public void StaticMultiplyMatrixNullVector()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        var a = new FloatMatrix(2);
        FloatVector b = null;
        FloatVector c = FloatMatrix.Multiply(a, b);
      });
    }

    //test member multiply vector
    [Test]
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
      Assert.AreEqual(a[0, 0], 6);
      Assert.AreEqual(a[1, 0], 14);
      Assert.AreEqual(a.ColumnLength, 1);
      Assert.AreEqual(a.RowLength, 2);
    }

    //test member multiply  matrix nonconform vector
    [Test]
    public void MemberMultiplyMatrixNonConformVector()
    {
      Assert.Throws(typeof(ArgumentException), () =>
      {
        var a = new FloatMatrix(2);
        var b = new FloatVector(3, 2.0f);
        a.Multiply(b);
      });
    }

    //test member multiply null vector
    [Test]
    public void MemberMultiplyNullVector()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        var a = new FloatMatrix(2);
        FloatVector b = null;
        a.Multiply(b);
      });
    }

    //test multiply  matrix matrix operator *
    [Test]
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
      Assert.AreEqual(c[0, 0], 6);
      Assert.AreEqual(c[0, 1], 6);
      Assert.AreEqual(c[1, 0], 14);
      Assert.AreEqual(c[1, 1], 14);
    }

    //test multiply  nonconform matrix matrix operator *
    [Test]
    public void OperatorMultiplyMatrixNonConformMatrix()
    {
      Assert.Throws(typeof(ArgumentException), () =>
      {
        var a = new FloatMatrix(2);
        var b = new FloatMatrix(3, 2, 2.0f);
        FloatMatrix c = a * b;
      });
    }

    //test multiply  long matrix wide matrix operator *
    [Test]
    public void OperatorMultiplyLongMatrixWideMatrix()
    {
      var a = new FloatMatrix(3, 2, 1);
      var b = new FloatMatrix(2, 3, 2);
      FloatMatrix c = a * b;
      Assert.AreEqual(c[0, 0], 4);
      Assert.AreEqual(c[0, 1], 4);
      Assert.AreEqual(c[0, 1], 4);
      Assert.AreEqual(c[1, 0], 4);
      Assert.AreEqual(c[1, 1], 4);
      Assert.AreEqual(c[1, 2], 4);
      Assert.AreEqual(c[2, 0], 4);
      Assert.AreEqual(c[2, 1], 4);
      Assert.AreEqual(c[2, 2], 4);
    }

    //test multiply  wide matrix long matrix operator *
    [Test]
    public void OperatorMultiplyWideMatrixLongMatrix()
    {
      var a = new FloatMatrix(2, 3, 1);
      var b = new FloatMatrix(3, 2, 2);
      FloatMatrix c = a * b;
      Assert.AreEqual(c[0, 0], 6);
      Assert.AreEqual(c[0, 1], 6);
      Assert.AreEqual(c[1, 0], 6);
      Assert.AreEqual(c[1, 1], 6);
    }

    //test multiply null matrix matrix operator *
    [Test]
    public void OperatorMultiplyNullMatrixMatrix()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        FloatMatrix a = null;
        var b = new FloatMatrix(2, 2);
        FloatMatrix c = a * b;
      });
    }

    //test multiply matrix null matrix operator *
    [Test]
    public void OperatorMultiplyMatrixNullMatrix()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        var a = new FloatMatrix(2, 2);
        FloatMatrix b = null;
        FloatMatrix c = a * b;
      });
    }

    //test static multiply  matrix matrix
    [Test]
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
      Assert.AreEqual(c[0, 0], 6);
      Assert.AreEqual(c[0, 1], 6);
      Assert.AreEqual(c[1, 0], 14);
      Assert.AreEqual(c[1, 1], 14);
    }

    //test static multiply nonconform matrix matrix
    [Test]
    public void StaticMultiplyMatrixNonConformMatrix()
    {
      Assert.Throws(typeof(ArgumentException), () =>
      {
        var a = new FloatMatrix(2);
        var b = new FloatMatrix(3, 2, 2.0f);
        var c = FloatMatrix.Multiply(a, b);
      });
    }

    //test static multiply  long matrix wide matrix
    [Test]
    public void StaticMultiplyLongMatrixWideMatrix()
    {
      var a = new FloatMatrix(3, 2, 1);
      var b = new FloatMatrix(2, 3, 2);
      var c = FloatMatrix.Multiply(a, b);
      Assert.AreEqual(c[0, 0], 4);
      Assert.AreEqual(c[0, 1], 4);
      Assert.AreEqual(c[0, 1], 4);
      Assert.AreEqual(c[1, 0], 4);
      Assert.AreEqual(c[1, 1], 4);
      Assert.AreEqual(c[1, 2], 4);
      Assert.AreEqual(c[2, 0], 4);
      Assert.AreEqual(c[2, 1], 4);
      Assert.AreEqual(c[2, 2], 4);
    }

    //test static multiply  wide matrix long matrix
    [Test]
    public void StaticMultiplyWideMatrixLongMatrix()
    {
      var a = new FloatMatrix(2, 3, 1);
      var b = new FloatMatrix(3, 2, 2);
      var c = FloatMatrix.Multiply(a, b);
      Assert.AreEqual(c[0, 0], 6);
      Assert.AreEqual(c[0, 1], 6);
      Assert.AreEqual(c[1, 0], 6);
      Assert.AreEqual(c[1, 1], 6);
    }

    //test static multiply null matrix matrix
    [Test]
    public void StaticMultiplyNullMatrixMatrix()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        FloatMatrix a = null;
        var b = new FloatMatrix(2, 2);
        var c = FloatMatrix.Multiply(a, b);
      });
    }

    //test static multiply matrix null matrix
    [Test]
    public void StaticMultiplyMatrixNullMatrix()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        var a = new FloatMatrix(2, 2);
        FloatMatrix b = null;
        var c = FloatMatrix.Multiply(a, b);
      });
    }

    //test member multiply  matrix matrix
    [Test]
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
      Assert.AreEqual(a[0, 0], 6);
      Assert.AreEqual(a[0, 1], 6);
      Assert.AreEqual(a[1, 0], 14);
      Assert.AreEqual(a[1, 1], 14);
    }

    //test member multiply nonconform matrix matrix
    [Test]
    public void MemberMultiplyMatrixNonConformMatrix()
    {
      Assert.Throws(typeof(ArgumentException), () =>
      {
        var a = new FloatMatrix(2);
        var b = new FloatMatrix(3, 2, 2.0f);
        a.Multiply(b);
      });
    }

    //test member multiply  long matrix wide matrix
    [Test]
    public void MemberMultiplyLongMatrixWideMatrix()
    {
      var a = new FloatMatrix(3, 2, 1);
      var b = new FloatMatrix(2, 3, 2);
      a.Multiply(b);
      Assert.AreEqual(a[0, 0], 4);
      Assert.AreEqual(a[0, 1], 4);
      Assert.AreEqual(a[0, 1], 4);
      Assert.AreEqual(a[1, 0], 4);
      Assert.AreEqual(a[1, 1], 4);
      Assert.AreEqual(a[1, 2], 4);
      Assert.AreEqual(a[2, 0], 4);
      Assert.AreEqual(a[2, 1], 4);
      Assert.AreEqual(a[2, 2], 4);
    }

    //test member multiply  wide matrix long matrix
    [Test]
    public void MemberMultiplyWideMatrixLongMatrix()
    {
      var a = new FloatMatrix(2, 3, 1);
      var b = new FloatMatrix(3, 2, 2);
      a.Multiply(b);
      Assert.AreEqual(a[0, 0], 6);
      Assert.AreEqual(a[0, 1], 6);
      Assert.AreEqual(a[1, 0], 6);
      Assert.AreEqual(a[1, 1], 6);
    }

    //test member multiply null matrix matrix
    [Test]
    public void MemberMultiplyNullMatrixMatrix()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        var a = new FloatMatrix(2, 2);
        FloatMatrix b = null;
        a.Multiply(b);
      });
    }

    //copy null
    [Test]
    public void CopyNull()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        FloatMatrix a = null;
        var b = new FloatMatrix(2);
        b.Copy(a);
      });
    }

    //clone
    [Test]
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
      Assert.AreEqual(a[0, 0], a[0, 0]);
      Assert.AreEqual(a[0, 1], b[0, 1]);
      Assert.AreEqual(a[1, 0], b[1, 0]);
      Assert.AreEqual(a[1, 1], b[1, 1]);
    }

    //Norm
    [Test]
    public void Norms()
    {
      var a = new FloatMatrix(2)
      {
        [0, 0] = 2,
        [0, 1] = 4,
        [1, 0] = 3,
        [1, 1] = 1
      };
      Assert.AreEqual(a.GetL1Norm(), 5.000, TOLERENCE);
      Assert.AreEqual(a.GetL2Norm(), 5.117, TOLERENCE);
      Assert.AreEqual(a.GetInfinityNorm(), 6.000, TOLERENCE);
      Assert.AreEqual(a.GetFrobeniusNorm(), 5.477, TOLERENCE);
    }

    //Wide Norm
    [Test]
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
      Assert.AreEqual(a.GetL1Norm(), 11.000, TOLERENCE);
      Assert.AreEqual(a.GetL2Norm(), 9.247, TOLERENCE);
      Assert.AreEqual(a.GetInfinityNorm(), 11.000, TOLERENCE);
      Assert.AreEqual(a.GetFrobeniusNorm(), 9.539, TOLERENCE);
    }

    //Long Norm
    [Test]
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
      Assert.AreEqual(a.GetL1Norm(), 11.000, TOLERENCE);
      Assert.AreEqual(a.GetL2Norm(), 9.337, TOLERENCE);
      Assert.AreEqual(a.GetInfinityNorm(), 11.000, TOLERENCE);
      Assert.AreEqual(a.GetFrobeniusNorm(), 9.539, TOLERENCE);
    }

    //Condition
    [Test]
    public void Condition()
    {
      var a = new FloatMatrix(2)
      {
        [0, 0] = 2,
        [0, 1] = 4,
        [1, 0] = 3,
        [1, 1] = 1
      };
      Assert.AreEqual(a.GetConditionNumber(), 2.618, TOLERENCE);
    }

    //Wide Condition
    [Test]
    public void WideCondition()
    {
      Assert.Throws(typeof(NotSquareMatrixException), () =>
      {
        var a = new FloatMatrix(2, 3);
        a.GetConditionNumber();
      });
    }

    //Long Condition
    [Test]
    public void LongCondition()
    {
      Assert.Throws(typeof(NotSquareMatrixException), () =>
      {
        var a = new FloatMatrix(3, 2);
        a.GetConditionNumber();
      });
    }
  }
}
