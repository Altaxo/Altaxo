#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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
#endregion

using System;
using NUnit.Framework;
using Altaxo.Calc;
using Altaxo.Calc.LinearAlgebra;

namespace AltaxoTest.Calc.LinearAlgebra 
{
  [TestFixture]
  public class DoubleMatrixTest
  {
    private const double TOLERENCE = 0.001;
    private const double DBL_EPSILON = DoubleConstants.DBL_EPSILON;

    //Test dimensions Constructor.
    [Test]
    public void CtorDimensions()
    {
      DoubleMatrix test = new DoubleMatrix(2,2);
      
      Assert.AreEqual(test.RowLength, 2);
      Assert.AreEqual(test.ColumnLength, 2);
      Assert.AreEqual(test[0,0], 0);
      Assert.AreEqual(test[0,1], 0);
      Assert.AreEqual(test[1,0], 0);
      Assert.AreEqual(test[1,1], 0);
    }

    //Test Intital Values Constructor.
    [Test]
    public void CtorInitialValues()
    {
      DoubleMatrix test = new DoubleMatrix(2,2,1);
      
      Assert.AreEqual(test.RowLength, 2);
      Assert.AreEqual(test.ColumnLength, 2);
      Assert.AreEqual(test[0,0], 1);
      Assert.AreEqual(test[0,1], 1);
      Assert.AreEqual(test[1,0], 1);
      Assert.AreEqual(test[1,1], 1);
    }
    
    //Test Copy Constructor.
    [Test]
    public void CtorCopy()
    {
      DoubleMatrix a = new DoubleMatrix(2,2);
      a[0,0] = 1;     
      a[0,1] = 2;     
      a[1,0] = 3;     
      a[1,1] = 4; 
      
      DoubleMatrix b = new DoubleMatrix(a);
      
      Assert.AreEqual(a.RowLength, b.RowLength);
      Assert.AreEqual(a.ColumnLength, b.ColumnLength);
      Assert.AreEqual(a[0,0], a[0,0]);
      Assert.AreEqual(a[0,1], b[0,1]);
      Assert.AreEqual(a[1,0], b[1,0]);
      Assert.AreEqual(a[1,1], b[1,1]);
    }
    
    //Test Copy Constructor.
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CtorCopyNull()
    {
      DoubleMatrix a = null;
      DoubleMatrix b = new DoubleMatrix(a);
    }

    //Test Copy Constructor.
    [Test]
    public void CtorCopyFloat()
    {
      FloatMatrix a = new FloatMatrix(2,2);
      a[0,0] = 1;     
      a[0,1] = 2;     
      a[1,0] = 3;     
      a[1,1] = 4; 
      
      DoubleMatrix b = new DoubleMatrix(a);
      Assert.AreEqual(a.RowLength, b.RowLength);
      Assert.AreEqual(a.ColumnLength, b.ColumnLength);
      Assert.AreEqual(a[0,0], a[0,0]);
      Assert.AreEqual(a[0,1], b[0,1]);
      Assert.AreEqual(a[1,0], b[1,0]);
      Assert.AreEqual(a[1,1], b[1,1]);
    }

    //Test Copy Constructor.
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CtorCopyFloatNull()
    {
      FloatMatrix a = null;
      DoubleMatrix b = new DoubleMatrix(a);
      
    }

    //Test Multiple Dimensional DoubleArray Constructor with Square array.
    [Test]
    public void CtorMultDimDoubleSquare()
    {
      double[,] values= new double[2,2];
      
      values[0,0] = 0;
      values[0,1] = 1;
      values[1,0] = 3;
      values[1,1] = 4;
      
      DoubleMatrix test = new DoubleMatrix(values);
      
      Assert.AreEqual(test.RowLength, 2);
      Assert.AreEqual(test.ColumnLength, 2);
      Assert.AreEqual(test[0,0], values[0,0]);
      Assert.AreEqual(test[0,1], values[0,1]);
      Assert.AreEqual(test[1,0], values[1,0]);
      Assert.AreEqual(test[1,1], values[1,1]);
    }
    
    //Test Multiple Dimensional DoubleArray Constructor with wide array.
    [Test]
    public void CtorMultDimDoubleWide()
    {
      double[,] values= new double[2,3];
      
      values[0,0] = 0;
      values[0,1] = 1;
      values[0,2] = 2;
      values[1,0] = 3;
      values[1,1] = 4;
      values[1,2] = 5;
      
      DoubleMatrix test = new DoubleMatrix(values);
      
      Assert.AreEqual(test.RowLength, 2);
      Assert.AreEqual(test.ColumnLength, 3);
      Assert.AreEqual(test[0,0], values[0,0]);
      Assert.AreEqual(test[0,1], values[0,1]);
      Assert.AreEqual(test[0,2], values[0,2]);
      Assert.AreEqual(test[1,0], values[1,0]);
      Assert.AreEqual(test[1,1], values[1,1]);
      Assert.AreEqual(test[1,2], values[1,2]);
    }
    
    //Test Multiple Dimensional DoubleArray Constructor with long array.
    [Test]
    public void CtorMultDimDoubleLong()
    {
      double[,] values= new double[3,2];
      
      values[0,0] = 0;
      values[0,1] = 1;
      values[1,0] = 3;
      values[1,1] = 4;
      values[2,0] = 6;
      values[2,1] = 7;
      
      DoubleMatrix test = new DoubleMatrix(values);
      
      Assert.AreEqual(test.RowLength, 3);
      Assert.AreEqual(test.ColumnLength, 2);
      Assert.AreEqual(test[0,0], values[0,0]);
      Assert.AreEqual(test[0,1], values[0,1]);
      Assert.AreEqual(test[1,0], values[1,0]);
      Assert.AreEqual(test[1,1], values[1,1]);
      Assert.AreEqual(test[2,0], values[2,0]);
      Assert.AreEqual(test[2,1], values[2,1]);
    }
    
    //Test Multiple Dimensional Double Array Constructor with null.
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CtorMultDimDoubleNull()
    {
      double[,] values = null;
      DoubleMatrix test = new DoubleMatrix(values);
    }

    //Test Multiple Dimensional Float Array Constructor with Square array.
    [Test]
    public void CtorMultDimFloatSquare()
    {
      float[,] values= new float[2,2];
      
      values[0,0] = 0;
      values[0,1] = 1;
      values[1,0] = 3;
      values[1,1] = 4;
      
      DoubleMatrix test = new DoubleMatrix(values);
      
      Assert.AreEqual(test.RowLength, 2);
      Assert.AreEqual(test.ColumnLength, 2);
      Assert.AreEqual(test[0,0], values[0,0]);
      Assert.AreEqual(test[0,1], values[0,1]);
      Assert.AreEqual(test[1,0], values[1,0]);
      Assert.AreEqual(test[1,1], values[1,1]);
    }
    
    //Test Multiple Dimensional Float Array Constructor with wide array.
    [Test]
    public void CtorMultDimFloatWide()
    {
      float[,] values= new float[2,3];
      
      values[0,0] = 0;
      values[0,1] = 1;
      values[0,2] = 2;
      values[1,0] = 3;
      values[1,1] = 4;
      values[1,2] = 5;
      
      DoubleMatrix test = new DoubleMatrix(values);
      
      Assert.AreEqual(test.RowLength, 2);
      Assert.AreEqual(test.ColumnLength, 3);
      Assert.AreEqual(test[0,0], values[0,0]);
      Assert.AreEqual(test[0,1], values[0,1]);
      Assert.AreEqual(test[0,2], values[0,2]);
      Assert.AreEqual(test[1,0], values[1,0]);
      Assert.AreEqual(test[1,1], values[1,1]);
      Assert.AreEqual(test[1,2], values[1,2]);
    }
    
    //Test Multiple Dimensional FloatArray Constructor with long array.
    [Test]
    public void CtorMultDimFloatLong()
    {
      float[,] values= new float[3,2];
      
      values[0,0] = 0;
      values[0,1] = 1;
      values[1,0] = 3;
      values[1,1] = 4;
      values[2,0] = 6;
      values[2,1] = 7;
      
      DoubleMatrix test = new DoubleMatrix(values);
      
      Assert.AreEqual(test.RowLength, 3);
      Assert.AreEqual(test.ColumnLength, 2);
      Assert.AreEqual(test[0,0], values[0,0]);
      Assert.AreEqual(test[0,1], values[0,1]);
      Assert.AreEqual(test[1,0], values[1,0]);
      Assert.AreEqual(test[1,1], values[1,1]);
      Assert.AreEqual(test[2,0], values[2,0]);
      Assert.AreEqual(test[2,1], values[2,1]);
    }
    
    //Test Multiple Dimensional Float Array Constructor with null.
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CtorMultDimFloatNull()
    {
      float[,] values = null;
      DoubleMatrix test = new DoubleMatrix(values);
    }
  
    //Test Jagged Array  Constructor with null.
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CtorJaggedNull()
    {
      double[,] values = null;
      DoubleMatrix test = new DoubleMatrix(values);
    }

    //Test implicit conversion from floatmatrix.
    [Test]
    public void ImplictFloatMatrix()
    {
      FloatMatrix a = new FloatMatrix(2,2);
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;
      
      DoubleMatrix b = a;
      Assert.AreEqual(a.RowLength, b.RowLength);
      Assert.AreEqual(a.ColumnLength, b.ColumnLength);
      Assert.AreEqual(a[0,0], b[0,0]);
      Assert.AreEqual(a[0,1], b[0,1]);
      Assert.AreEqual(a[1,0], b[1,0]);
      Assert.AreEqual(a[1,1], b[1,1]);
    }
    
    //Test implicit conversion from null floatmatrix.
    [Test]
    public void ImplictFloatMatrixNull()
    {
      FloatMatrix a = null;
      DoubleMatrix b = a;
      Assert.IsTrue(b == null);
    }
    
    //Test implicit conversion from floatmatrix.
    [Test]
    public void ImplictToFloatMatrix()
    {
      FloatMatrix a = new FloatMatrix(2,2);
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;
      
      DoubleMatrix b = DoubleMatrix.ToDoubleMatrix(a);
      Assert.AreEqual(a.RowLength, b.RowLength);
      Assert.AreEqual(a.ColumnLength, b.ColumnLength);
      Assert.AreEqual(a[0,0], b[0,0]);
      Assert.AreEqual(a[0,1], b[0,1]);
      Assert.AreEqual(a[1,0], b[1,0]);
      Assert.AreEqual(a[1,1], b[1,1]);
    }
    
    //Test implicit conversion from null floatmatrix.
    [Test]
    public void ImplictToFloatMatrixNull()
    {
      FloatMatrix a = null;
      DoubleMatrix b = DoubleMatrix.ToDoubleMatrix(a);
      Assert.IsTrue(b == null);
    }

    //Test implicit conversion from double mult dim array.
    [Test]
    public void ImplictDoubleMultArray()
    {
      double[,] a = new double[2,2];
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;
      
      DoubleMatrix b = a;
      Assert.AreEqual(a[0,0], b[0,0]);
      Assert.AreEqual(a[0,1], b[0,1]);
      Assert.AreEqual(a[1,0], b[1,0]);
      Assert.AreEqual(a[1,1], b[1,1]);
    }
    
    //Test implicit conversion from null double mult dim array.
    [Test]
    public void ImplictDoubleMultArrayNull()
    {
      double[,] a = null;
      DoubleMatrix b = a;
      Assert.IsTrue(b == null);
    }
    
    //Test implicit conversion from double mult dim array.
    [Test]
    public void ImplictToDoubleMultArray()
    {
      double[,] a = new double[2,2];
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;
      
      DoubleMatrix b = DoubleMatrix.ToDoubleMatrix(a);
      Assert.AreEqual(a[0,0], b[0,0]);
      Assert.AreEqual(a[0,1], b[0,1]);
      Assert.AreEqual(a[1,0], b[1,0]);
      Assert.AreEqual(a[1,1], b[1,1]);
    }
    
    //Test implicit conversion from null double mult dim array.
    [Test]
    public void ImplictToDoubleMultArrayNull()
    {
      double[,] a = null;
      DoubleMatrix b = DoubleMatrix.ToDoubleMatrix(a);
      Assert.IsTrue(b == null);
    }
    
    //Test implicit conversion from float mult dim array.
    [Test]
    public void ImplictFloatMultArray()
    {
      float[,] a = new float[2,2];
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;
      
      DoubleMatrix b = a;
      Assert.AreEqual(a[0,0], b[0,0]);
      Assert.AreEqual(a[0,1], b[0,1]);
      Assert.AreEqual(a[1,0], b[1,0]);
      Assert.AreEqual(a[1,1], b[1,1]);
    }
    
    //Test implicit conversion from null float mult dim array.
    [Test]
    public void ImplictFloatMultArrayNull()
    {
      float[,] a = null;
      DoubleMatrix b = a;
      Assert.IsTrue(b == null);
    }
    
    //Test implicit conversion from float mult dim array.
    [Test]
    public void ImplictToFloatMultArray()
    {
      float[,] a = new float[2,2];
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;
      
      DoubleMatrix b = DoubleMatrix.ToDoubleMatrix(a);
      Assert.AreEqual(a[0,0], b[0,0]);
      Assert.AreEqual(a[0,1], b[0,1]);
      Assert.AreEqual(a[1,0], b[1,0]);
      Assert.AreEqual(a[1,1], b[1,1]);
    }
    
    //Test implicit conversion from null float mult dim array.
    [Test]
    public void ImplictToFloatMultArrayNull()
    {
      float[,] a = null;
      DoubleMatrix b = DoubleMatrix.ToDoubleMatrix(a);
      Assert.IsTrue(b == null);
    }

    //test equals method
    [Test]
    public void Equals()
    {
      DoubleMatrix a = new DoubleMatrix(2,2,4);
      DoubleMatrix b = new DoubleMatrix(2,2,4);
      DoubleMatrix c = new DoubleMatrix(2,2);
      c[0,0] = 4;
      c[0,1] = 4;
      c[1,0] = 4;
      c[1,1] = 4;

      DoubleMatrix d = new DoubleMatrix(2,2,5);
      DoubleMatrix e = null;
      FloatMatrix f = new FloatMatrix(2,2,4);
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
      DoubleMatrix a = new DoubleMatrix(2);
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;

      int hash = a.GetHashCode();
      Assert.AreEqual(hash, 5);
    }
    
    //test ToArray
    [Test]
    public void ToArray()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;

      double[,] b = a.ToArray();

      Assert.AreEqual(a[0,0], b[0,0]);
      Assert.AreEqual(a[0,1], b[0,1]);
      Assert.AreEqual(a[1,0], b[1,0]);
      Assert.AreEqual(a[1,1], b[1,1]);
    }
    
    //test Transpose square
    [Test]
    public void TransposeSquare()
    {
      DoubleMatrix a = new DoubleMatrix(2,2);
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;
      a.Transpose();
      Assert.AreEqual(a[0,0], 1);
      Assert.AreEqual(a[0,1], 3);
      Assert.AreEqual(a[1,0], 2);
      Assert.AreEqual(a[1,1], 4);
    }
    
    //test Transpose wide
    [Test]
    public void TransposeWide()
    {
      DoubleMatrix a = new DoubleMatrix(2,3);
      a[0,0] = 1;
      a[0,1] = 2;
      a[0,2] = 3;
      a[1,0] = 4;
      a[1,1] = 5;
      a[1,2] = 6;
      a.Transpose();
      Assert.AreEqual(a[0,0], 1);
      Assert.AreEqual(a[0,1], 4);
      Assert.AreEqual(a[1,0], 2);
      Assert.AreEqual(a[1,1], 5);
      Assert.AreEqual(a[2,0], 3);
      Assert.AreEqual(a[2,1], 6);
      Assert.AreEqual(a.RowLength, 3);
      Assert.AreEqual(a.ColumnLength, 2);
    }

    //test Transpose long
    [Test]
    public void TransposeLong()
    {
      DoubleMatrix a = new DoubleMatrix(3,2);
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;
      a[2,0] = 5;
      a[2,1] = 6;
      a.Transpose();
      Assert.AreEqual(a[0,0], 1);
      Assert.AreEqual(a[0,1], 3);
      Assert.AreEqual(a[0,2], 5);
      Assert.AreEqual(a[1,0], 2);
      Assert.AreEqual(a[1,1], 4);
      Assert.AreEqual(a[1,2], 6);
      Assert.AreEqual(a.RowLength, 2);
      Assert.AreEqual(a.ColumnLength, 3);
    }
    
    //test GetTranspose square
    [Test]
    public void GetTransposeSquare()
    {
      DoubleMatrix a = new DoubleMatrix(2,2);
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;
      DoubleMatrix b = a.GetTranspose();
      Assert.AreEqual(b[0,0], 1);
      Assert.AreEqual(b[0,1], 3);
      Assert.AreEqual(b[1,0], 2);
      Assert.AreEqual(b[1,1], 4);
    }
    
    //test GetTranspose wide
    [Test]
    public void GetTransposeWide()
    {
      DoubleMatrix a = new DoubleMatrix(2,3);
      a[0,0] = 1;
      a[0,1] = 2;
      a[0,2] = 3;
      a[1,0] = 4;
      a[1,1] = 5;
      a[1,2] = 6;
      DoubleMatrix b = a.GetTranspose();
      Assert.AreEqual(b[0,0], 1);
      Assert.AreEqual(b[0,1], 4);
      Assert.AreEqual(b[1,0], 2);
      Assert.AreEqual(b[1,1], 5);
      Assert.AreEqual(b[2,0], 3);
      Assert.AreEqual(b[2,1], 6);
      Assert.AreEqual(b.RowLength, a.ColumnLength);
      Assert.AreEqual(b.ColumnLength, a.RowLength);
    }

    //test GetTranspose long
    [Test]
    public void GetTransposeLong()
    {
      DoubleMatrix a = new DoubleMatrix(3,2);
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;
      a[2,0] = 5;
      a[2,1] = 6;
      DoubleMatrix b = a.GetTranspose();
      Assert.AreEqual(b[0,0], 1);
      Assert.AreEqual(b[0,1], 3);
      Assert.AreEqual(b[0,2], 5);
      Assert.AreEqual(b[1,0], 2);
      Assert.AreEqual(b[1,1], 4);
      Assert.AreEqual(b[1,2], 6);
      Assert.AreEqual(b.RowLength, a.ColumnLength);
      Assert.AreEqual(b.ColumnLength, a.RowLength);
    }

    //test Invert
    [Test]
    public void Invert()
    {
      DoubleMatrix a = new DoubleMatrix(2,2);
      a[0,0] = 2;
      a[0,1] = 4;
      a[1,0] = 3;
      a[1,1] = 7;
      a.Invert();
      Assert.AreEqual(a[0,0], 3.5,  4e-15);
      Assert.AreEqual(a[0,1], -2,   4e-15);
      Assert.AreEqual(a[1,0], -1.5, 4e-15);
      Assert.AreEqual(a[1,1], 1,    4e-15);
    }

    //test Invert singular
    [Test]
    [ExpectedException(typeof(SingularMatrixException))]
    public void InvertSingular()
    {
      DoubleMatrix a = new DoubleMatrix(2,2);
      a.Invert();
    }

    //test Invert not square
    [Test]
    [ExpectedException(typeof(NotSquareMatrixException))]
    public void InvertNotSquare()
    {
      DoubleMatrix a = new DoubleMatrix(3,2);
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;
      a[2,0] = 5;
      a[2,1] = 6;
      a.Invert();
    }

    //test GetInverse singular
    [Test]
    [ExpectedException(typeof(SingularMatrixException))]
    public void GetInverseSingular()
    {
      DoubleMatrix a = new DoubleMatrix(2,2);
      DoubleMatrix b = a.GetInverse();
    }

    //test GetInverse not square
    [Test]
    [ExpectedException(typeof(NotSquareMatrixException))]
    public void GetInverseNotSquare()
    {
      DoubleMatrix a = new DoubleMatrix(3,2);
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;
      a[2,0] = 5;
      a[2,1] = 6;
      DoubleMatrix b = a.GetInverse();
    }
    //test GetInverse
    [Test]
    public void GetInverse()
    {
      DoubleMatrix a = new DoubleMatrix(2,2);
      a[0,0] = 2;
      a[0,1] = 4;
      a[1,0] = 3;
      a[1,1] = 7;
      DoubleMatrix b = a.GetInverse();
      Assert.AreEqual(b[0,0], 3.500,TOLERENCE);
      Assert.AreEqual(b[0,1], -2.000,TOLERENCE);
      Assert.AreEqual(b[1,0], -1.500,TOLERENCE);
      Assert.AreEqual(b[1,1], 1.000,TOLERENCE);
    }   

    //test GetDeterminant
    [Test]
    public void GetDeterminant()
    {
      DoubleMatrix a = new DoubleMatrix(2,2);
      a[0,0] = 2;
      a[0,1] = 4;
      a[1,0] = 3;
      a[1,1] = 7;
      double b = a.GetDeterminant();
      Assert.AreEqual(b, 2.000,TOLERENCE);
    }

    //test GetDeterminant
    [Test]
    [ExpectedException(typeof(NotSquareMatrixException))]
    public void GetDeterminantNotSquare()
    {
      DoubleMatrix a = new DoubleMatrix(3,2);
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;
      a[2,0] = 5;
      a[2,1] = 6;
      double b = a.GetDeterminant();
    }   

    //test GetRow
    [Test]
    public void GetRow()
    {
      DoubleMatrix a = new DoubleMatrix(2,2);
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;
      DoubleVector b = a.GetRow(0);
      Assert.AreEqual(b[0], a[0,0]);
      Assert.AreEqual(b[1], a[0,1]);
    }
    
    //test GetRow
    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetRowOutOfRange()
    {
      DoubleMatrix a = new DoubleMatrix(2,2);
      DoubleVector b = a.GetRow(3);
    }
    
    //test GetColumn
    [Test]
    public void GetColumn()
    {
      DoubleMatrix a = new DoubleMatrix(2,2);
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;
      DoubleVector b = a.GetColumn(0);
      Assert.AreEqual(b[0], a[0,0]);
      Assert.AreEqual(b[1], a[1,0]);
    }
    
    //test GetColumn
    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetColumnOutOfRange()
    {
      DoubleMatrix a = new DoubleMatrix(2,2);
      DoubleVector b = a.GetColumn(3);
    } 

    //test SetRow
    [Test]
    public void SetRow()
    {
      DoubleMatrix a = new DoubleMatrix(2,2);
      DoubleVector b = new DoubleVector(2);
      b[0] = 1;
      b[1] = 2;
      a.SetRow(0,b);
      Assert.AreEqual(b[0], a[0,0]);
      Assert.AreEqual(b[1], a[0,1]);
    }

    //test SetRow
    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void SetRowOutOfRange()
    {
      DoubleMatrix a = new DoubleMatrix(2,2);
      DoubleVector b = new DoubleVector(2);
      a.SetRow(2,b);
    }

    //test SetRow
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void SetRowWrongRank()
    {
      DoubleMatrix a = new DoubleMatrix(2,2);
      DoubleVector b = new DoubleVector(3);
      a.SetRow(1,b);
    }

    //test SetRow
    [Test]
    public void SetRowArray()
    {
      DoubleMatrix a = new DoubleMatrix(2,2);
      double[] b = {1,2};
      a.SetRow(0,b);
      Assert.AreEqual(b[0], a[0,0]);
      Assert.AreEqual(b[1], a[0,1]);
    }

    //test SetRow
    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void SetRowArrayOutOfRange()
    {
      DoubleMatrix a = new DoubleMatrix(2,2);
      double[] b = {1,2};
      a.SetRow(2,b);
    }

    //test SetRow
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void SetRowArrayWrongRank()
    {
      DoubleMatrix a = new DoubleMatrix(2,2);
      double[] b = {1,2,3};
      a.SetRow(1,b);
    }

    //test SetColumn
    [Test]
    public void SetColumn()
    {
      DoubleMatrix a = new DoubleMatrix(2,2);
      DoubleVector b = new DoubleVector(2);
      b[0] = 1;
      b[1] = 2;
      a.SetColumn(0,b);
      Assert.AreEqual(b[0], a[0,0]);
      Assert.AreEqual(b[1], a[1,0]);
    }

    //test SetColumn
    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void SetColumnOutOfRange()
    {
      DoubleMatrix a = new DoubleMatrix(2,2);
      DoubleVector b = new DoubleVector(2);
      a.SetColumn(2,b);
    }

    //test SetColumn
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void SetColumnWrongRank()
    {
      DoubleMatrix a = new DoubleMatrix(2,2);
      DoubleVector b = new DoubleVector(3);
      a.SetColumn(1,b);
    }

    //test SetColumn
    [Test]
    public void SetColumnArray()
    {
      DoubleMatrix a = new DoubleMatrix(2,2);
      double[] b = {1,2};
      a.SetColumn(0,b);
      Assert.AreEqual(b[0], a[0,0]);
      Assert.AreEqual(b[1], a[1,0]);
    }

    //test SetColumn
    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void SetColumnArrayOutOfRange()
    {
      DoubleMatrix a = new DoubleMatrix(2,2);
      double[] b = {1,2};
      a.SetColumn(2,b);
    }

    //test SetColumn
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void SetColumnArrayWrongRank()
    {
      DoubleMatrix a = new DoubleMatrix(2,2);
      double[] b = {1,2,3};
      a.SetColumn(1,b);
    }

    //test GetDiagonal
    [Test]
    public void GetDiagonal()
    {
      DoubleMatrix a = new DoubleMatrix(2,2);
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;
      DoubleVector b = a.GetDiagonal();
      Assert.AreEqual(b[0], a[0,0]);
      Assert.AreEqual(b[1], a[1,1]);
    }

    //test SetDiagonal
    [Test]
    public void SetDiagonal()
    {
      DoubleMatrix a = new DoubleMatrix(2,2);
      DoubleVector b = new DoubleVector(2);
      b[0] = 1;
      b[1] = 2;
      a.SetDiagonal(b);
      Assert.AreEqual(b[0], a[0,0]);
      Assert.AreEqual(b[1], a[1,1]);
    }
    
    //test GetSubMatrix
    [Test]
    public void GetSubMatrix()
    {
      DoubleMatrix a = new DoubleMatrix(4);
      a[0,0] = 1;
      a[0,1] = 2;
      a[0,2] = 3;
      a[0,3] = 4;
      a[1,0] = 5;
      a[1,1] = 6;
      a[1,2] = 7;
      a[1,3] = 8;
      a[2,0] = 9;
      a[2,1] = 10;
      a[2,2] = 11;
      a[2,3] = 12;
      a[3,0] = 13;
      a[3,1] = 14;
      a[3,2] = 15;
      a[3,3] = 16;
      DoubleMatrix b = a.GetSubMatrix(2,2);
      DoubleMatrix c = a.GetSubMatrix(0,1,2,2);
      Assert.AreEqual(b.RowLength, 2);
      Assert.AreEqual(b.ColumnLength, 2);
      Assert.AreEqual(c.RowLength, 3);
      Assert.AreEqual(c.ColumnLength, 2);
      Assert.AreEqual(b[0,0], a[2,2]);
      Assert.AreEqual(b[0,1], a[2,3]);
      Assert.AreEqual(b[1,0], a[3,2]);
      Assert.AreEqual(b[1,1], a[3,3]);
      Assert.AreEqual(c[0,0], a[0,1]);
      Assert.AreEqual(c[0,1], a[0,2]);
      Assert.AreEqual(c[1,0], a[1,1]);
      Assert.AreEqual(c[1,1], a[1,2]);    
      Assert.AreEqual(c[2,0], a[2,1]);
      Assert.AreEqual(c[2,1], a[2,2]);    
    }
    
    //test GetSubMatrix
    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetSubMatrixOutRange1()
    {
      DoubleMatrix a = new DoubleMatrix(4);
      DoubleMatrix b = a.GetSubMatrix(-1,2);
    }
    //test GetSubMatrix
    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetSubMatrixOutRange2()
    {
      DoubleMatrix a = new DoubleMatrix(4);
      DoubleMatrix b = a.GetSubMatrix(2,4);
    }

    //test GetSubMatrix
    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetSubMatrixOutRange3()
    {
      DoubleMatrix a = new DoubleMatrix(4);
      DoubleMatrix b = a.GetSubMatrix(0,0,4,2);
    }

    //test GetSubMatrix
    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetSubMatrixOutRange4()
    {
      DoubleMatrix a = new DoubleMatrix(4);
      DoubleMatrix b = a.GetSubMatrix(0,0,2,4);
    }

    //test GetSubMatrix
    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetSubMatrixOutRange5()
    {
      DoubleMatrix a = new DoubleMatrix(4);
      DoubleMatrix b = a.GetSubMatrix(0,3,2,2);
    }
    
    //test GetUpperTriangle square matrix
    [Test]
    public void GetUpperTriangleSquare()
    {
      DoubleMatrix a = new DoubleMatrix(3);
      a[0,0] = 1;
      a[0,1] = 2;
      a[0,2] = 3;
      a[1,0] = 4;
      a[1,1] = 5;
      a[1,2] = 6;
      a[2,0] = 7;
      a[2,1] = 8;
      a[2,2] = 9;
      DoubleMatrix b = a.GetUpperTriangle();
      
      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0,0], a[0,0]);
      Assert.AreEqual(b[0,1], a[0,1]);
      Assert.AreEqual(b[0,2], a[0,2]);
      Assert.AreEqual(b[1,0], 0);
      Assert.AreEqual(b[1,1], a[1,1]);
      Assert.AreEqual(b[1,2], a[1,2]);
      Assert.AreEqual(b[2,0], 0);
      Assert.AreEqual(b[2,1], 0);   
      Assert.AreEqual(b[2,2], a[2,2]);
    }
    
    //test GetUpperTriangle long matrix
    [Test]
    public void GetUpperTriangleLong()
    {
      DoubleMatrix a = new DoubleMatrix(3,2);
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 4;
      a[1,1] = 5;
      a[2,0] = 7;
      a[2,1] = 8;
      DoubleMatrix b = a.GetUpperTriangle();
      
      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0,0], a[0,0]);
      Assert.AreEqual(b[0,1], a[0,1]);
      Assert.AreEqual(b[1,0], 0);
      Assert.AreEqual(b[1,1], a[1,1]);
      Assert.AreEqual(b[2,0], 0);
      Assert.AreEqual(b[2,1], 0);   
    }

    //test GetUpperTriangle wide matrix
    [Test]
    public void GetUpperTriangleWide()
    {
      DoubleMatrix a = new DoubleMatrix(2,3);
      a[0,0] = 1;
      a[0,1] = 2;
      a[0,2] = 3;
      a[1,0] = 4;
      a[1,1] = 5;
      a[1,2] = 6;
      DoubleMatrix b = a.GetUpperTriangle();
      
      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0,0], a[0,0]);
      Assert.AreEqual(b[0,1], a[0,1]);
      Assert.AreEqual(b[0,2], a[0,2]);
      Assert.AreEqual(b[1,0], 0);
      Assert.AreEqual(b[1,1], a[1,1]);
      Assert.AreEqual(b[1,2], a[1,2]);
    }

    //test GetStrictlyUpperTriangle square matrix
    [Test]
    public void GetStrictlyUpperTriangleSquare()
    {
      DoubleMatrix a = new DoubleMatrix(3);
      a[0,0] = 1;
      a[0,1] = 2;
      a[0,2] = 3;
      a[1,0] = 4;
      a[1,1] = 5;
      a[1,2] = 6;
      a[2,0] = 7;
      a[2,1] = 8;
      a[2,2] = 9;
      DoubleMatrix b = a.GetStrictlyUpperTriangle();
      
      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0,0], 0);
      Assert.AreEqual(b[0,1], a[0,1]);
      Assert.AreEqual(b[0,2], a[0,2]);
      Assert.AreEqual(b[1,0], 0);
      Assert.AreEqual(b[1,1], 0);
      Assert.AreEqual(b[1,2], a[1,2]);
      Assert.AreEqual(b[2,0], 0);
      Assert.AreEqual(b[2,1], 0);   
      Assert.AreEqual(b[2,2], 0);
    }
    
    //test GetStrictlyUpperTriangle long matrix
    [Test]
    public void GetStrictlyUpperTriangleLong()
    {
      DoubleMatrix a = new DoubleMatrix(3,2);
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 4;
      a[1,1] = 5;
      a[2,0] = 7;
      a[2,1] = 8;
      DoubleMatrix b = a.GetStrictlyUpperTriangle();
      
      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0,0], 0);
      Assert.AreEqual(b[0,1], a[0,1]);
      Assert.AreEqual(b[1,0], 0);
      Assert.AreEqual(b[1,1], 0);
      Assert.AreEqual(b[2,0], 0);
      Assert.AreEqual(b[2,1], 0);   
    }

    //test GetStrictlyUpperTriangle wide matrix
    [Test]
    public void GetStrictlyUpperTriangleWide()
    {
      DoubleMatrix a = new DoubleMatrix(2,3);
      a[0,0] = 1;
      a[0,1] = 2;
      a[0,2] = 3;
      a[1,0] = 4;
      a[1,1] = 5;
      a[1,2] = 6;
      DoubleMatrix b = a.GetStrictlyUpperTriangle();
      
      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0,0], 0);
      Assert.AreEqual(b[0,1], a[0,1]);
      Assert.AreEqual(b[0,2], a[0,2]);
      Assert.AreEqual(b[1,0], 0);
      Assert.AreEqual(b[1,1], 0);
      Assert.AreEqual(b[1,2], a[1,2]);
    }


    //test GetLowerTriangle square matrix
    [Test]
    public void GetLowerTriangleSquare()
    {
      DoubleMatrix a = new DoubleMatrix(3);
      a[0,0] = 1;
      a[0,1] = 2;
      a[0,2] = 3;
      a[1,0] = 4;
      a[1,1] = 5;
      a[1,2] = 6;
      a[2,0] = 7;
      a[2,1] = 8;
      a[2,2] = 9;
      DoubleMatrix b = a.GetLowerTriangle();
      
      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0,0], a[0,0]);
      Assert.AreEqual(b[0,1], 0);
      Assert.AreEqual(b[0,2], 0);
      Assert.AreEqual(b[1,0], a[1,0]);
      Assert.AreEqual(b[1,1], a[1,1]);
      Assert.AreEqual(b[1,2], 0);
      Assert.AreEqual(b[2,0], a[2,0]);
      Assert.AreEqual(b[2,1], a[2,1]);    
      Assert.AreEqual(b[2,2], a[2,2]);
    }
    
    //test GetLowerTriangle long matrix
    [Test]
    public void GetLowerTriangleLong()
    {
      DoubleMatrix a = new DoubleMatrix(3,2);
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 4;
      a[1,1] = 5;
      a[2,0] = 7;
      a[2,1] = 8;
      DoubleMatrix b = a.GetLowerTriangle();
      
      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0,0], a[0,0]);
      Assert.AreEqual(b[0,1], 0);
      Assert.AreEqual(b[1,0], b[1,0]);
      Assert.AreEqual(b[1,1], a[1,1]);
      Assert.AreEqual(b[2,0], b[2,0]);
      Assert.AreEqual(b[2,1], b[2,1]);    
    }

    //test GetLowerTriangle wide matrix
    [Test]
    public void GetLowerTriangleWide()
    {
      DoubleMatrix a = new DoubleMatrix(2,3);
      a[0,0] = 1;
      a[0,1] = 2;
      a[0,2] = 3;
      a[1,0] = 4;
      a[1,1] = 5;
      a[1,2] = 6;
      DoubleMatrix b = a.GetLowerTriangle();
      
      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0,0], a[0,0]);
      Assert.AreEqual(b[0,1], 0);
      Assert.AreEqual(b[0,2], 0);
      Assert.AreEqual(b[1,0], a[1,0]);
      Assert.AreEqual(b[1,1], a[1,1]);
      Assert.AreEqual(b[1,2], 0);
    }

    //test GetStrictlyLowerTriangle square matrix
    [Test]
    public void GetStrictlyLowerTriangleSquare()
    {
      DoubleMatrix a = new DoubleMatrix(3);
      a[0,0] = 1;
      a[0,1] = 2;
      a[0,2] = 3;
      a[1,0] = 4;
      a[1,1] = 5;
      a[1,2] = 6;
      a[2,0] = 7;
      a[2,1] = 8;
      a[2,2] = 9;
      DoubleMatrix b = a.GetStrictlyLowerTriangle();
      
      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0,0], 0);
      Assert.AreEqual(b[0,1], 0);
      Assert.AreEqual(b[0,2], 0);
      Assert.AreEqual(b[1,0], a[1,0]);
      Assert.AreEqual(b[1,1], 0);
      Assert.AreEqual(b[1,2], 0);
      Assert.AreEqual(b[2,0], a[2,0]);
      Assert.AreEqual(b[2,1], a[2,1]);    
      Assert.AreEqual(b[2,2], 0);
    }
    
    //test GetStrictlyLowerTriangle long matrix
    [Test]
    public void GetStrictlyLowerTriangleLong()
    {
      DoubleMatrix a = new DoubleMatrix(3,2);
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 4;
      a[1,1] = 5;
      a[2,0] = 7;
      a[2,1] = 8;
      DoubleMatrix b = a.GetStrictlyLowerTriangle();
      
      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0,0], 0);
      Assert.AreEqual(b[0,1], 0);
      Assert.AreEqual(b[1,0], b[1,0]);
      Assert.AreEqual(b[1,1], 0);
      Assert.AreEqual(b[2,0], b[2,0]);
      Assert.AreEqual(b[2,1], b[2,1]);  
    }

    //test GetStrictlyLowerTriangle wide matrix
    [Test]
    public void GetStrictlyLowerTriangleWide()
    {
      DoubleMatrix a = new DoubleMatrix(2,3);
      a[0,0] = 1;
      a[0,1] = 2;
      a[0,2] = 3;
      a[1,0] = 4;
      a[1,1] = 5;
      a[1,2] = 6;
      DoubleMatrix b = a.GetStrictlyLowerTriangle();
      
      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0,0], 0);
      Assert.AreEqual(b[0,1], 0);
      Assert.AreEqual(b[0,2], 0);
      Assert.AreEqual(b[1,0], a[1,0]);
      Assert.AreEqual(b[1,1], 0);
      Assert.AreEqual(b[1,2], 0);
    }
    
    //static Negate
    [Test]
    public void Negate()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;

      DoubleMatrix b = DoubleMatrix.Negate(a);
      Assert.AreEqual(b[0,0], -1);
      Assert.AreEqual(b[0,1], -2);
      Assert.AreEqual(b[1,0], -3);
      Assert.AreEqual(b[1,1], -4);
    }
    
    //static NegateNull
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void NegateNull()
    {
      DoubleMatrix a = null;
      DoubleMatrix b = DoubleMatrix.Negate(a);
    }

    //static operator -
    [Test]
    public void OperatorMinus()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;

      DoubleMatrix b = -a;
      Assert.AreEqual(b[0,0], -1);
      Assert.AreEqual(b[0,1], -2);
      Assert.AreEqual(b[1,0], -3);
      Assert.AreEqual(b[1,1], -4);
    }

    //static operator - null
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void OperatorMinusNull()
    {
      DoubleMatrix a = null;
      DoubleMatrix b = -a;
    }   


    //static subtact two square matrices
    [Test]
    public void StaticSubtract()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      DoubleMatrix b = new DoubleMatrix(2);
      a[0,0] = b[0,0] = 1;
      a[0,1] = b[0,1] = 2;
      a[1,0] = b[1,0] = 3;
      a[1,1] = b[1,1] = 4;
      DoubleMatrix c = DoubleMatrix.Subtract(a,b);
      Assert.AreEqual(c[0,0],0);
      Assert.AreEqual(c[0,1],0);
      Assert.AreEqual(c[1,0],0);
      Assert.AreEqual(c[1,1],0);
    }

    //operator subtract two square matrices
    [Test]
    public void OperatorSubtract()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      DoubleMatrix b = new DoubleMatrix(2);
      a[0,0] = b[0,0] = 1;
      a[0,1] = b[0,1] = 2;
      a[1,0] = b[1,0] = 3;
      a[1,1] = b[1,1] = 4;
      DoubleMatrix c = a-b;
      Assert.AreEqual(c[0,0],0);
      Assert.AreEqual(c[0,1],0);
      Assert.AreEqual(c[1,0],0);
      Assert.AreEqual(c[1,1],0);
    }
  
    //member add subtract square matrices
    [Test]
    public void MemberSubtract()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      DoubleMatrix b = new DoubleMatrix(2);
      a[0,0] = b[0,0] = 1;
      a[0,1] = b[0,1] = 2;
      a[1,0] = b[1,0] = 3;
      a[1,1] = b[1,1] = 4;
      a.Subtract(b);
      Assert.AreEqual(a[0,0],0);
      Assert.AreEqual(a[0,1],0);
      Assert.AreEqual(a[1,0],0);
      Assert.AreEqual(a[1,1],0);
    }
    
    //static Subtract two square matrices, one null
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void StaticSubtractNull()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      DoubleMatrix b = null;
      DoubleMatrix c = DoubleMatrix.Subtract(a,b);
    }

    //operator Subtract two square matrices, one null
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void OperatorSubtractNull()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      DoubleMatrix b = null;
      DoubleMatrix c = a-b;
    }
  
    //member Subtract two square matrices, one null
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void MemberSubtractNull()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      DoubleMatrix b = null;
      a.Subtract(b);
    }

    //static Subtract two incompatible matrices
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void StaticSubtractIncompatible()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      DoubleMatrix b = new DoubleMatrix(3);
      DoubleMatrix c = DoubleMatrix.Subtract(a,b);
    }

    //operator Subtract two  incompatible matrices
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void OperatorSubtractIncompatible()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      DoubleMatrix b = new DoubleMatrix(3);
      DoubleMatrix c = a-b;
    }
  
    //member Subtract two  incompatible matricess
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void MemberSubtractIncompatible()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      DoubleMatrix b = new DoubleMatrix(3);
      a.Subtract(b);
    }

    //static add two square matrices
    [Test]
    public void StaticAdd()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      DoubleMatrix b = new DoubleMatrix(2);
      a[0,0] = b[0,0] = 1;
      a[0,1] = b[0,1] = 2;
      a[1,0] = b[1,0] = 3;
      a[1,1] = b[1,1] = 4;
      DoubleMatrix c = DoubleMatrix.Add(a,b);
      Assert.AreEqual(c[0,0],2);
      Assert.AreEqual(c[0,1],4);
      Assert.AreEqual(c[1,0],6);
      Assert.AreEqual(c[1,1],8);
    }

    //operator add two square matrices
    [Test]
    public void OperatorAdd()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      DoubleMatrix b = new DoubleMatrix(2);
      a[0,0] = b[0,0] = 1;
      a[0,1] = b[0,1] = 2;
      a[1,0] = b[1,0] = 3;
      a[1,1] = b[1,1] = 4;
      DoubleMatrix c = a+b;
      Assert.AreEqual(c[0,0],2);
      Assert.AreEqual(c[0,1],4);
      Assert.AreEqual(c[1,0],6);
      Assert.AreEqual(c[1,1],8);
    }
  
    //member add two square matrices
    [Test]
    public void MemberAdd()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      DoubleMatrix b = new DoubleMatrix(2);
      a[0,0] = b[0,0] = 1;
      a[0,1] = b[0,1] = 2;
      a[1,0] = b[1,0] = 3;
      a[1,1] = b[1,1] = 4;
      a.Add(b);
      Assert.AreEqual(a[0,0],2);
      Assert.AreEqual(a[0,1],4);
      Assert.AreEqual(a[1,0],6);
      Assert.AreEqual(a[1,1],8);
    }
    
    //static add two square matrices, one null
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void StaticAddNull()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      DoubleMatrix b = null;
      DoubleMatrix c = DoubleMatrix.Add(a,b);
    }

    //operator add two square matrices, one null
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void OperatorAddNull()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      DoubleMatrix b = null;
      DoubleMatrix c = a+b;
    }
  
    //member add two square matrices, one null
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void MemberAddNull()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      DoubleMatrix b = null;
      a.Add(b);
    }

    //static add two incompatible matrices
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void StaticAddIncompatible()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      DoubleMatrix b = new DoubleMatrix(3);
      DoubleMatrix c = DoubleMatrix.Add(a,b);
    }

    //operator add two  incompatible matrices
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void OperatorAddIncompatible()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      DoubleMatrix b = new DoubleMatrix(3);
      DoubleMatrix c = a+b;
    }
  
    //member add two  incompatible matricess
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void MemberAddIncompatible()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      DoubleMatrix b = new DoubleMatrix(3);
      a.Add(b);
    }


    //static divide matrix by double
    [Test]
    public void StaticDivide()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      a[0,0] = 2;
      a[0,1] = 4;
      a[1,0] = 6;
      a[1,1] = 8;
      DoubleMatrix b = DoubleMatrix.Divide(a,2);
      Assert.AreEqual(b[0,0],1);
      Assert.AreEqual(b[0,1],2);
      Assert.AreEqual(b[1,0],3);
      Assert.AreEqual(b[1,1],4);
    }

    //operator divide matrix by double
    [Test]
    public void OperatorDivide()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      a[0,0] = 2;
      a[0,1] = 4;
      a[1,0] = 6;
      a[1,1] = 8;
      DoubleMatrix b = a/2;
      Assert.AreEqual(b[0,0],1);
      Assert.AreEqual(b[0,1],2);
      Assert.AreEqual(b[1,0],3);
      Assert.AreEqual(b[1,1],4);
    }
  
    //member divide matrix by double
    [Test]
    public void MemberDivide()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      a[0,0] = 2;
      a[0,1] = 4;
      a[1,0] = 6;
      a[1,1] = 8;
      a.Divide(2);
      Assert.AreEqual(a[0,0],1);
      Assert.AreEqual(a[0,1],2);
      Assert.AreEqual(a[1,0],3);
      Assert.AreEqual(a[1,1],4);
    }
    
    //static divide null matrix by double
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void StaticDivideNull()
    {
      DoubleMatrix a = null;
      DoubleMatrix b = DoubleMatrix.Divide(a,2);
    }

    //operator divide null matrix by double
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void OperatorDivideNull()
    {
      DoubleMatrix a = null;
      DoubleMatrix b = a/2;
    }

    //copy
    [Test]
    public void Copy()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;
      DoubleMatrix b = new DoubleMatrix(2);
      b.Copy(a);
      Assert.AreEqual(a[0,0],a[0,0]);
      Assert.AreEqual(a[0,1],b[0,1]);
      Assert.AreEqual(a[1,0],b[1,0]);
      Assert.AreEqual(a[1,1],b[1,1]);
    }

    //test multiply double matrix operator *
    [Test]
    public void OperatorMultiplyDoubleMatrix()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;
      DoubleMatrix b = 2.0 * a;
      Assert.AreEqual(b[0,0],2);
      Assert.AreEqual(b[0,1],4);
      Assert.AreEqual(b[1,0],6);
      Assert.AreEqual(b[1,1],8);
    }
    
    //test multiply double null matrix operator * 
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void OperatorMultiplyDoubleMatrixNull()
    {
      DoubleMatrix a = null;
      DoubleMatrix b = 2.0 * a;
    } 

    //test multiply  matrix double operator *
    [Test]
    public void OperatorMultiplyMatrixDouble()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;
      DoubleMatrix b = a * 2.0;
      Assert.AreEqual(b[0,0],2);
      Assert.AreEqual(b[0,1],4);
      Assert.AreEqual(b[1,0],6);
      Assert.AreEqual(b[1,1],8);
    }
    
    //test multiply  null matrix double operator * 
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void OperatorMultiplyMatrixDoubleNull()
    {
      DoubleMatrix a = null;
      DoubleMatrix b = a * 2;
    }
    
    //test static multiply double matrix 
    [Test]
    public void StaticMultiplyDoubleMatrix()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;
      DoubleMatrix b = DoubleMatrix.Multiply(2.0,a);
      Assert.AreEqual(b[0,0],2);
      Assert.AreEqual(b[0,1],4);
      Assert.AreEqual(b[1,0],6);
      Assert.AreEqual(b[1,1],8);
    }
    
    //test static multiply double null matrix 
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void StaticMultiplyDoubleMatrixNull()
    {
      DoubleMatrix a = null;
      DoubleMatrix b = DoubleMatrix.Multiply(2.0,a);

    } 

    //test static multiply  matrix double
    [Test]
    public void StaticMultiplyMatrixDouble()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;
      DoubleMatrix b = DoubleMatrix.Multiply(a,2.0);

      Assert.AreEqual(b[0,0],2);
      Assert.AreEqual(b[0,1],4);
      Assert.AreEqual(b[1,0],6);
      Assert.AreEqual(b[1,1],8);
    }
    
    //test static multiply  null matrix double operator * 
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void StaticMultiplyMatrixDoubleNull()
    {
      DoubleMatrix a = null;
      DoubleMatrix b = DoubleMatrix.Multiply(a,2.0);

    }

    //test member multiply  double 
    [Test]
    public void MemberMultiplyDouble()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;
      a.Multiply(2.0);
      Assert.AreEqual(a[0,0],2);
      Assert.AreEqual(a[0,1],4);
      Assert.AreEqual(a[1,0],6);
      Assert.AreEqual(a[1,1],8);
    }

    //test multiply  matrix vector operator *
    [Test]
    public void OperatorMultiplyMatrixVector()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;
      DoubleVector b = new DoubleVector(2,2);
      DoubleVector c = a * b;
      Assert.AreEqual(c[0],6);
      Assert.AreEqual(c[1],14);
    }
    
    //test multiply  matrix nonconform vector operator *
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void OperatorMultiplyMatrixNonConformVector()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      DoubleVector b = new DoubleVector(3,2);
      DoubleVector c = a * b;
    }

    //test multiply null matrix vector operator *
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void OperatorMultiplyNullMatrixVector()
    {
      DoubleMatrix a = null;
      DoubleVector b = new DoubleVector(2,2);
      DoubleVector c = a * b;
    }

    //test multiply matrix null vector operator *
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void OperatorMultiplyMatrixNullVector()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;
      DoubleVector b = null;
      DoubleVector c = a * b;
    }
    
    //test static multiply  matrix vector
    [Test]
    public void StaticMultiplyMatrixVector()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;
      DoubleVector b = new DoubleVector(2,2);
      DoubleVector c = DoubleMatrix.Multiply(a,b);
      Assert.AreEqual(c[0],6);
      Assert.AreEqual(c[1],14);
    }

    //test static multiply  matrix nonconform vector
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void StaticMultiplyMatrixNonConformVector()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      DoubleVector b = new DoubleVector(3,2);
      DoubleVector c = a * b;
    }

    //test static multiply null matrix vector
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void StaticMultiplyNullMatrixVector()
    {
      DoubleMatrix a = null;
      DoubleVector b = new DoubleVector(2,2);
      DoubleVector c = DoubleMatrix.Multiply(a,b);
    }

    //test static multiply matrix null vector
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void StaticMultiplyMatrixNullVector()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;
      DoubleVector b = null;
      DoubleVector c = DoubleMatrix.Multiply(a,b);
    }

    //test member multiply vector
    [Test]
    public void MemberMultiplyVector()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;
      DoubleVector b = new DoubleVector(2,2);
      a.Multiply(b);
      Assert.AreEqual(a[0,0],6);
      Assert.AreEqual(a[1,0],14);
      Assert.AreEqual(a.ColumnLength, 1);
      Assert.AreEqual(a.RowLength, 2);
    }
    
    //test member multiply  matrix nonconform vector
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void MemberMultiplyMatrixNonConformVector()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      DoubleVector b = new DoubleVector(3,2);
      a.Multiply(b);
    }
    //test member multiply null vector
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void MemberMultiplyNullVector()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      DoubleVector b = null;
      a.Multiply(b);
    }

    //test multiply  matrix matrix operator *
    [Test]
    public void OperatorMultiplyMatrixMatrix()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;
      DoubleMatrix b = new DoubleMatrix(2,2,2.0);
      DoubleMatrix c = a * b;
      Assert.AreEqual(c[0,0],6);
      Assert.AreEqual(c[0,1],6);
      Assert.AreEqual(c[1,0],14);
      Assert.AreEqual(c[1,1],14);
    }

    //test multiply  nonconform matrix matrix operator *
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void OperatorMultiplyMatrixNonConformMatrix()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      DoubleMatrix b = new DoubleMatrix(3, 2, 2.0);
      DoubleMatrix c = a * b;
    }

    //test multiply  long matrix wide matrix operator *
    [Test]
    public void OperatorMultiplyLongMatrixWideMatrix()
    {
      DoubleMatrix a = new DoubleMatrix(3,2,1);
      DoubleMatrix b = new DoubleMatrix(2,3,2);
      DoubleMatrix c = a * b;
      Assert.AreEqual(c[0,0],4);
      Assert.AreEqual(c[0,1],4);
      Assert.AreEqual(c[0,1],4);
      Assert.AreEqual(c[1,0],4);
      Assert.AreEqual(c[1,1],4);
      Assert.AreEqual(c[1,2],4);
      Assert.AreEqual(c[2,0],4);
      Assert.AreEqual(c[2,1],4);
      Assert.AreEqual(c[2,2],4);
    }

    //test multiply  wide matrix long matrix operator *
    [Test]
    public void OperatorMultiplyWideMatrixLongMatrix()
    {
      DoubleMatrix a = new DoubleMatrix(2,3,1);
      DoubleMatrix b = new DoubleMatrix(3,2,2);
      DoubleMatrix c = a * b;
      Assert.AreEqual(c[0,0],6);
      Assert.AreEqual(c[0,1],6);
      Assert.AreEqual(c[1,0],6);
      Assert.AreEqual(c[1,1],6);
    }

    //test multiply null matrix matrix operator *
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void OperatorMultiplyNullMatrixMatrix()
    {
      DoubleMatrix a = null;
      DoubleMatrix b = new DoubleMatrix(2,2);
      DoubleMatrix c = a * b;
    }

    //test multiply matrix null matrix operator *
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void OperatorMultiplyMatrixNullMatrix()
    {
      DoubleMatrix a = new DoubleMatrix(2,2);
      DoubleMatrix b = null;
      DoubleMatrix c = a * b;
    }

    //test static multiply  matrix matrix
    [Test]
    public void StaticMultiplyMatrixMatrix()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;
      DoubleMatrix b = new DoubleMatrix(2, 2, 2.0);
      DoubleMatrix c = DoubleMatrix.Multiply(a,b);
      Assert.AreEqual(c[0,0],6);
      Assert.AreEqual(c[0,1],6);
      Assert.AreEqual(c[1,0],14);
      Assert.AreEqual(c[1,1],14);
    }

    //test static multiply nonconform matrix matrix
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void StaticMultiplyMatrixNonConformMatrix()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      DoubleMatrix b = new DoubleMatrix(3, 2, 2.0);
      DoubleMatrix c = DoubleMatrix.Multiply(a,b);
    }

    //test static multiply  long matrix wide matrix
    [Test]
    public void StaticMultiplyLongMatrixWideMatrix()
    {
      DoubleMatrix a = new DoubleMatrix(3,2,1);
      DoubleMatrix b = new DoubleMatrix(2,3,2);
      DoubleMatrix c = DoubleMatrix.Multiply(a,b);
      Assert.AreEqual(c[0,0],4);
      Assert.AreEqual(c[0,1],4);
      Assert.AreEqual(c[0,1],4);
      Assert.AreEqual(c[1,0],4);
      Assert.AreEqual(c[1,1],4);
      Assert.AreEqual(c[1,2],4);
      Assert.AreEqual(c[2,0],4);
      Assert.AreEqual(c[2,1],4);
      Assert.AreEqual(c[2,2],4);
    }

    //test static multiply  wide matrix long matrix
    [Test]
    public void StaticMultiplyWideMatrixLongMatrix()
    {
      DoubleMatrix a = new DoubleMatrix(2,3,1);
      DoubleMatrix b = new DoubleMatrix(3,2,2);
      DoubleMatrix c = DoubleMatrix.Multiply(a,b);
      Assert.AreEqual(c[0,0],6);
      Assert.AreEqual(c[0,1],6);
      Assert.AreEqual(c[1,0],6);
      Assert.AreEqual(c[1,1],6);
    }

    //test static multiply null matrix matrix
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void StaticMultiplyNullMatrixMatrix()
    {
      DoubleMatrix a = null;
      DoubleMatrix b = new DoubleMatrix(2,2);
      DoubleMatrix c = DoubleMatrix.Multiply(a,b);
    }

    //test static multiply matrix null matrix 
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void StaticMultiplyMatrixNullMatrix()
    {
      DoubleMatrix a = new DoubleMatrix(2,2);
      DoubleMatrix b = null;
      DoubleMatrix c = DoubleMatrix.Multiply(a,b);
    }

    //test member multiply  matrix matrix
    [Test]
    public void MemberMultiplyMatrixMatrix()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;
      DoubleMatrix b = new DoubleMatrix(2, 2, 2.0);
      a.Multiply(b);
      Assert.AreEqual(a[0,0],6);
      Assert.AreEqual(a[0,1],6);
      Assert.AreEqual(a[1,0],14);
      Assert.AreEqual(a[1,1],14);
    }

    //test member multiply nonconform matrix matrix
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void MemberMultiplyMatrixNonConformMatrix()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      DoubleMatrix b = new DoubleMatrix(3, 2, 2.0);
      a.Multiply(b);
    }

    //test member multiply  long matrix wide matrix
    [Test]
    public void MemberMultiplyLongMatrixWideMatrix()
    {
      DoubleMatrix a = new DoubleMatrix(3,2,1);
      DoubleMatrix b = new DoubleMatrix(2,3,2);
      a.Multiply(b);
      Assert.AreEqual(a[0,0],4);
      Assert.AreEqual(a[0,1],4);
      Assert.AreEqual(a[0,1],4);
      Assert.AreEqual(a[1,0],4);
      Assert.AreEqual(a[1,1],4);
      Assert.AreEqual(a[1,2],4);
      Assert.AreEqual(a[2,0],4);
      Assert.AreEqual(a[2,1],4);
      Assert.AreEqual(a[2,2],4);
    }

    //test member multiply  wide matrix long matrix
    [Test]
    public void MemberMultiplyWideMatrixLongMatrix()
    {
      DoubleMatrix a = new DoubleMatrix(2,3,1);
      DoubleMatrix b = new DoubleMatrix(3,2,2);
      a.Multiply(b);
      Assert.AreEqual(a[0,0],6);
      Assert.AreEqual(a[0,1],6);
      Assert.AreEqual(a[1,0],6);
      Assert.AreEqual(a[1,1],6);
    }

    //test member multiply null matrix matrix
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void MemberMultiplyNullMatrixMatrix()
    {
      DoubleMatrix a = new DoubleMatrix(2,2);
      DoubleMatrix b = null;
      a.Multiply(b);
    }

  
    //copy null
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CopyNull()
    {
      DoubleMatrix a =  null;
      DoubleMatrix b = new DoubleMatrix(2);
      b.Copy(a);
    }

    //Norm
    [Test]
    public void Norms()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      a[0,0] = 2;
      a[0,1] = 4;
      a[1,0] = 3;
      a[1,1] = 1;
      Assert.AreEqual(a.GetL1Norm(),5.000,TOLERENCE);
      Assert.AreEqual(a.GetL2Norm(),5.117,TOLERENCE);
      Assert.AreEqual(a.GetInfinityNorm(),6.000,TOLERENCE);
      Assert.AreEqual(a.GetFrobeniusNorm(),5.477,TOLERENCE);
    }

    //Wide Norm
    [Test]
    public void WideNorms()
    {
      DoubleMatrix a = new DoubleMatrix(2,3);
      a[0,0] = 2;
      a[0,1] = 4;
      a[0,2] = 5;
      a[1,0] = 3;
      a[1,1] = 1;
      a[1,2] = 6;
      Assert.AreEqual(a.GetL1Norm(),11.000,TOLERENCE);
      Assert.AreEqual(a.GetL2Norm(),9.247,TOLERENCE);
      Assert.AreEqual(a.GetInfinityNorm(),11.000,TOLERENCE);
      Assert.AreEqual(a.GetFrobeniusNorm(),9.539,TOLERENCE);
    }

    //Long Norm
    [Test]
    public void LongNorms()
    {
      DoubleMatrix a = new DoubleMatrix(3,2);
      a[0,0] = 2;
      a[0,1] = 4;
      a[1,0] = 3;
      a[1,1] = 1;
      a[2,0] = 5;
      a[2,1] = 6;
      Assert.AreEqual(a.GetL1Norm(),11.000,TOLERENCE);
      Assert.AreEqual(a.GetL2Norm(),9.337,TOLERENCE);
      Assert.AreEqual(a.GetInfinityNorm(),11.000,TOLERENCE);
      Assert.AreEqual(a.GetFrobeniusNorm(),9.539,TOLERENCE);
    } 

    //Condition
    [Test]
    public void Condition()
    {
      DoubleMatrix a = new DoubleMatrix(2);
      a[0,0] = 2;
      a[0,1] = 4;
      a[1,0] = 3;
      a[1,1] = 1;
      Assert.AreEqual(a.GetConditionNumber(),2.618,TOLERENCE);
    }

    //Wide Condition
    [Test]
    [ExpectedException(typeof(NotSquareMatrixException))]
    public void WideCondition()
    {
      DoubleMatrix a = new DoubleMatrix(2,3);
      a.GetConditionNumber();
    }

    //Long Condition
    [Test]
    [ExpectedException(typeof(NotSquareMatrixException))]
    public void LongCondition()
    {
      DoubleMatrix a = new DoubleMatrix(3,2);
      a.GetConditionNumber();
    } 
  }
}
