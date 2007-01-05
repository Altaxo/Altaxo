#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
  public class ComplexFloatMatrixTest
  {
    private const double TOLERENCE = 0.001;

    //Test dimensions Constructor.
    [Test]
    public void CtorDimensions()
    {
      ComplexFloatMatrix test = new ComplexFloatMatrix(2,2);
      
      Assert.AreEqual(test.RowLength, 2);
      Assert.AreEqual(test.ColumnLength, 2);
      Assert.AreEqual(test[0,0], ComplexFloat.Zero);
      Assert.AreEqual(test[0,1], ComplexFloat.Zero);
      Assert.AreEqual(test[1,0], ComplexFloat.Zero);
      Assert.AreEqual(test[1,1], ComplexFloat.Zero);
    }

    //Test Intital Values Constructor.
    [Test]
    public void CtorInitialValues()
    {
      ComplexFloatMatrix test = new ComplexFloatMatrix(2,2,new ComplexFloat(1,1));
      
      Assert.AreEqual(test.RowLength, 2);
      Assert.AreEqual(test.ColumnLength, 2);
      ComplexFloat value = new ComplexFloat(1,1);
      Assert.AreEqual(test[0,0], value);
      Assert.AreEqual(test[0,1], value);
      Assert.AreEqual(test[1,0], value);
      Assert.AreEqual(test[1,1], value);
    }
    
    //Test Copy Constructor.
    [Test]
    public void CtorCopy()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2,2);
      a[0,0] = new ComplexFloat(1,1);     
      a[0,1] = new ComplexFloat(2,2);     
      a[1,0] = new ComplexFloat(3,3);     
      a[1,1] = new ComplexFloat(4,4); 
      
      ComplexFloatMatrix b = new ComplexFloatMatrix(a);
      
      Assert.AreEqual(a.RowLength, b.RowLength);
      Assert.AreEqual(a.ColumnLength, b.ColumnLength);
      Assert.AreEqual(a[0,0], b[0,0]);
      Assert.AreEqual(a[0,1], b[0,1]);
      Assert.AreEqual(a[1,0], b[1,0]);
      Assert.AreEqual(a[1,1], b[1,1]);
    }
    
    //Test Copy Constructor.
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CtorCopyNull()
    {
      ComplexFloatMatrix a = null;
      ComplexFloatMatrix b = new ComplexFloatMatrix(a);
    }

    //Test Copy Constructor.
    [Test]
    public void CtorCopyComplexFloat()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2,2);
      a[0,0] = new ComplexFloat(1,1);     
      a[0,1] = new ComplexFloat(2,2);     
      a[1,0] = new ComplexFloat(3,3);     
      a[1,1] = new ComplexFloat(4,4); 
      
      ComplexFloatMatrix b = new ComplexFloatMatrix(a);
      Assert.AreEqual(a.RowLength, b.RowLength);
      Assert.AreEqual(a.ColumnLength, b.ColumnLength);
      Assert.AreEqual(a[0,0].Real, b[0,0].Real);
      Assert.AreEqual(a[0,1].Real, b[0,1].Real);
      Assert.AreEqual(a[1,0].Real, b[1,0].Real);
      Assert.AreEqual(a[1,1].Real, b[1,1].Real);
      Assert.AreEqual(a[0,0].Imag, b[0,0].Imag);
      Assert.AreEqual(a[0,1].Imag, b[0,1].Imag);
      Assert.AreEqual(a[1,0].Imag, b[1,0].Imag);
      Assert.AreEqual(a[1,1].Imag, b[1,1].Imag);
    }

    //Test Copy Constructor.
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CtorCopyComplexFloatNull()
    {
      FloatMatrix a = null;
      ComplexFloatMatrix b = new ComplexFloatMatrix(a);
      
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
      
      ComplexFloatMatrix b = new ComplexFloatMatrix(a);
      Assert.AreEqual(a.RowLength, b.RowLength);
      Assert.AreEqual(a.ColumnLength, b.ColumnLength);
      Assert.AreEqual(a[0,0], b[0,0].Real);
      Assert.AreEqual(a[0,1], b[0,1].Real);
      Assert.AreEqual(a[1,0], b[1,0].Real);
      Assert.AreEqual(a[1,1], b[1,1].Real);
      Assert.AreEqual(0, b[0,0].Imag);
      Assert.AreEqual(0, b[0,1].Imag);
      Assert.AreEqual(0, b[1,0].Imag);
      Assert.AreEqual(0, b[1,1].Imag);
    }

    //Test Copy Constructor.
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CtorCopyFloatNull()
    {
      FloatMatrix a = null;
      ComplexFloatMatrix b = new ComplexFloatMatrix(a);
      
    }

    //Test Copy Constructor.
    [Test]
    public void CtorCopyDouble()
    {
      FloatMatrix a = new FloatMatrix(2,2);
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;

      ComplexFloatMatrix b = new ComplexFloatMatrix(a);
      Assert.AreEqual(a.RowLength, b.RowLength);
      Assert.AreEqual(a.ColumnLength, b.ColumnLength);
      Assert.AreEqual(a[0,0], b[0,0].Real);
      Assert.AreEqual(a[0,1], b[0,1].Real);
      Assert.AreEqual(a[1,0], b[1,0].Real);
      Assert.AreEqual(a[1,1], b[1,1].Real);
      Assert.AreEqual(0, b[0,0].Imag);
      Assert.AreEqual(0, b[0,1].Imag);
      Assert.AreEqual(0, b[1,0].Imag);
      Assert.AreEqual(0, b[1,1].Imag);
    }

    //Test Copy Constructor.
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CtorCopyDoubleNull()
    {
      FloatMatrix a = null;
      ComplexFloatMatrix b = new ComplexFloatMatrix(a);

    }
 
    //Test Multiple Dimensional ComplexFloatArray Constructor with Square array.
    [Test]
    public void CtorMultDimComplexFloatSquare()
    {
      ComplexFloat[,] values= new ComplexFloat[2,2];
      
      values[0,0] = new ComplexFloat(1,1);
      values[0,1] = new ComplexFloat(2,2);
      values[1,0] = new ComplexFloat(3,3);
      values[1,1] = new ComplexFloat(4,4);
      
      ComplexFloatMatrix test = new ComplexFloatMatrix(values);
      
      Assert.AreEqual(test.RowLength, 2);
      Assert.AreEqual(test.ColumnLength, 2);
      Assert.AreEqual(test[0,0], values[0,0]);
      Assert.AreEqual(test[0,1], values[0,1]);
      Assert.AreEqual(test[1,0], values[1,0]);
      Assert.AreEqual(test[1,1], values[1,1]);
    }
    
    //Test Multiple Dimensional ComplexFloatArray Constructor with wide array.
    [Test]
    public void CtorMultDimComplexFloatWide()
    {
      ComplexFloat[,] values= new ComplexFloat[2,3];
      
      values[0,0] = new ComplexFloat(0,0);
      values[0,1] = new ComplexFloat(1,1);
      values[0,2] = new ComplexFloat(2,2);
      values[1,0] = new ComplexFloat(3,3);
      values[1,1] = new ComplexFloat(4,4);
      values[1,2] = new ComplexFloat(5,5);
      
      ComplexFloatMatrix test = new ComplexFloatMatrix(values);
      
      Assert.AreEqual(test.RowLength, 2);
      Assert.AreEqual(test.ColumnLength, 3);
      Assert.AreEqual(test[0,0], values[0,0]);
      Assert.AreEqual(test[0,1], values[0,1]);
      Assert.AreEqual(test[0,2], values[0,2]);
      Assert.AreEqual(test[1,0], values[1,0]);
      Assert.AreEqual(test[1,1], values[1,1]);
      Assert.AreEqual(test[1,2], values[1,2]);
    }
    
    //Test Multiple Dimensional ComplexFloatArray Constructor with long array.
    [Test]
    public void CtorMultDimComplexFloatLong()
    {
      ComplexFloat[,] values= new ComplexFloat[3,2];
      
      values[0,0] = new ComplexFloat(0,0);
      values[0,1] = new ComplexFloat(1,1);
      values[1,0] = new ComplexFloat(3,3);
      values[1,1] = new ComplexFloat(4,4);
      values[2,0] = new ComplexFloat(5,5);
      values[2,1] = new ComplexFloat(6,6);
      
      ComplexFloatMatrix test = new ComplexFloatMatrix(values);
      
      Assert.AreEqual(test.RowLength, 3);
      Assert.AreEqual(test.ColumnLength, 2);
      Assert.AreEqual(test[0,0], values[0,0]);
      Assert.AreEqual(test[0,1], values[0,1]);
      Assert.AreEqual(test[1,0], values[1,0]);
      Assert.AreEqual(test[1,1], values[1,1]);
      Assert.AreEqual(test[2,0], values[2,0]);
      Assert.AreEqual(test[2,1], values[2,1]);
    }
    
    //Test Multiple Dimensional ComplexFloat Array Constructor with null.
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CtorMultDimComplexFloatNull()
    {
      float[,] values = null;
      ComplexFloatMatrix test = new ComplexFloatMatrix(values);
    }

    //Test Multiple Dimensional DoubleArray Constructor with Square array.
    [Test]
    public void CtorMultDimDoubleSquare()
    {
      float[,] values= new float[2,2];

      values[0,0] = 1;
      values[0,1] = 2;
      values[1,0] = 3;
      values[1,1] = 4;

      ComplexFloatMatrix test = new ComplexFloatMatrix(values);

      Assert.AreEqual(test.RowLength, 2);
      Assert.AreEqual(test.ColumnLength, 2);
      Assert.AreEqual(values[0,0], test[0,0].Real);
      Assert.AreEqual(values[0,1], test[0,1].Real);
      Assert.AreEqual(values[1,0], test[1,0].Real);
      Assert.AreEqual(values[1,1], test[1,1].Real);
    }

    //Test Multiple Dimensional DoubleArray Constructor with wide array.
    [Test]
    public void CtorMultDimDoubleWide()
    {
      float[,] values= new float[2,3];

      values[0,0] = 0;
      values[0,1] = 1;
      values[0,2] = 2;
      values[1,0] = 3;
      values[1,1] = 4;
      values[1,2] = 5;

      ComplexFloatMatrix test = new ComplexFloatMatrix(values);

      Assert.AreEqual(test.RowLength, 2);
      Assert.AreEqual(test.ColumnLength, 3);
      Assert.AreEqual(test[0,0].Real, values[0,0]);
      Assert.AreEqual(test[0,1].Real, values[0,1]);
      Assert.AreEqual(test[0,2].Real, values[0,2]);
      Assert.AreEqual(test[1,0].Real, values[1,0]);
      Assert.AreEqual(test[1,1].Real, values[1,1]);
      Assert.AreEqual(test[1,2].Real, values[1,2]);
    }

    //Test Multiple Dimensional DoubleArray Constructor with long array.
    [Test]
    public void CtorMultDimDoubleLong()
    {
      float[,] values= new float[3,2];

      values[0,0] = 0;
      values[0,1] = 1;
      values[1,0] = 2;
      values[1,1] = 3;
      values[2,0] = 4;
      values[2,1] = 5;

      ComplexFloatMatrix test = new ComplexFloatMatrix(values);

      Assert.AreEqual(test.RowLength, 3);
      Assert.AreEqual(test.ColumnLength, 2);
      Assert.AreEqual(test[0,0].Real, values[0,0]);
      Assert.AreEqual(test[0,1].Real, values[0,1]);
      Assert.AreEqual(test[1,0].Real, values[1,0]);
      Assert.AreEqual(test[1,1].Real, values[1,1]);
      Assert.AreEqual(test[2,0].Real, values[2,0]);
      Assert.AreEqual(test[2,1].Real, values[2,1]);
    }

    //Test Multiple Dimensional Double Array Constructor with null.
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CtorMultDimDoubleNull()
    {
      float[,] values = null;
      ComplexFloatMatrix test = new ComplexFloatMatrix(values);
    }

    //Test Multiple Dimensional Float Array Constructor with Square array.
    [Test]
    public void CtorMultDimFloatSquare()
    {
      float[,] values= new float[2,2];

      values[0,0] = 0;
      values[0,1] = 1;
      values[1,0] = 2;
      values[1,1] = 3;

      ComplexFloatMatrix test = new ComplexFloatMatrix(values);

      Assert.AreEqual(test.RowLength, 2);
      Assert.AreEqual(test.ColumnLength, 2);
      Assert.AreEqual(test[0,0].Real, values[0,0]);
      Assert.AreEqual(test[0,1].Real, values[0,1]);
      Assert.AreEqual(test[1,0].Real, values[1,0]);
      Assert.AreEqual(test[1,1].Real, values[1,1]);
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

      ComplexFloatMatrix test = new ComplexFloatMatrix(values);

      Assert.AreEqual(test.RowLength, 2);
      Assert.AreEqual(test.ColumnLength, 3);
      Assert.AreEqual(test[0,0].Real, values[0,0]);
      Assert.AreEqual(test[0,1].Real, values[0,1]);
      Assert.AreEqual(test[0,2].Real, values[0,2]);
      Assert.AreEqual(test[1,0].Real, values[1,0]);
      Assert.AreEqual(test[1,1].Real, values[1,1]);
      Assert.AreEqual(test[1,2].Real, values[1,2]);
    }

    //Test Multiple Dimensional FloatArray Constructor with long array.
    [Test]
    public void CtorMultDimFloatLong()
    {
      float[,] values= new float[3,2];

      values[0,0] = 0;
      values[0,1] = 1;
      values[1,0] = 2;
      values[1,1] = 3;
      values[2,0] = 4;
      values[2,1] = 5;

      ComplexFloatMatrix test = new ComplexFloatMatrix(values);

      Assert.AreEqual(test.RowLength, 3);
      Assert.AreEqual(test.ColumnLength, 2);
      Assert.AreEqual(test[0,0].Real, values[0,0]);
      Assert.AreEqual(test[0,1].Real, values[0,1]);
      Assert.AreEqual(test[1,0].Real, values[1,0]);
      Assert.AreEqual(test[1,1].Real, values[1,1]);
      Assert.AreEqual(test[2,0].Real, values[2,0]);
      Assert.AreEqual(test[2,1].Real, values[2,1]);
    }

    //Test Multiple Dimensional Float Array Constructor with null.
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CtorMultDimFloatNull()
    {
      float[,] values = null;
      ComplexFloatMatrix test = new ComplexFloatMatrix(values);
    }

    //Test implicit conversion from ComplexFloatMatrix.
    [Test]
    public void ImplictComplexFloatMatrix()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2,2);
      a[0,0] = new ComplexFloat(1,1);
      a[0,1] = new ComplexFloat(2,2);
      a[1,0] = new ComplexFloat(3,3);
      a[1,1] = new ComplexFloat(4,4);

      ComplexFloatMatrix b = a;
      Assert.AreEqual(a.RowLength, b.RowLength);
      Assert.AreEqual(a.ColumnLength, b.ColumnLength);
      Assert.AreEqual(a[0,0], b[0,0]);
      Assert.AreEqual(a[0,1], b[0,1]);
      Assert.AreEqual(a[1,0], b[1,0]);
      Assert.AreEqual(a[1,1], b[1,1]);
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
      ComplexFloatMatrix a = new ComplexFloatMatrix(2,2);
      a[0,0] = new ComplexFloat(1,1);
      a[0,1] = new ComplexFloat(2,2);
      a[1,0] = new ComplexFloat(3,3);
      a[1,1] = new ComplexFloat(4,4);

      ComplexFloatMatrix b = ComplexFloatMatrix.ToComplexFloatMatrix(a);
      Assert.AreEqual(a.RowLength, b.RowLength);
      Assert.AreEqual(a.ColumnLength, b.ColumnLength);
      Assert.AreEqual(a[0,0], b[0,0]);
      Assert.AreEqual(a[0,1], b[0,1]);
      Assert.AreEqual(a[1,0], b[1,0]);
      Assert.AreEqual(a[1,1], b[1,1]);
    }

    //Test implicit conversion from null ComplexFoatmatrix.
    [Test]
    public void ImplictToComplexFloatMatrixNull()
    {
      ComplexFloatMatrix a = null;
      ComplexFloatMatrix b = ComplexFloatMatrix.ToComplexFloatMatrix(a);
      Assert.IsTrue(b == null);
    }

    //Test implicit conversion from Doublematrix.
    [Test]
    public void ImplictDoubleMatrix()
    {
      FloatMatrix a = new FloatMatrix(2,2);
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;

      ComplexFloatMatrix b = a;
      Assert.AreEqual(a.RowLength, b.RowLength);
      Assert.AreEqual(a.ColumnLength, b.ColumnLength);
      Assert.AreEqual(a[0,0], b[0,0].Real);
      Assert.AreEqual(a[0,1], b[0,1].Real);
      Assert.AreEqual(a[1,0], b[1,0].Real);
      Assert.AreEqual(a[1,1], b[1,1].Real);
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
      FloatMatrix a = new FloatMatrix(2,2);
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;

      ComplexFloatMatrix b = ComplexFloatMatrix.ToComplexFloatMatrix(a);
      Assert.AreEqual(a.RowLength, b.RowLength);
      Assert.AreEqual(a.ColumnLength, b.ColumnLength);
      Assert.AreEqual(a[0,0], b[0,0].Real);
      Assert.AreEqual(a[0,1], b[0,1].Real);
      Assert.AreEqual(a[1,0], b[1,0].Real);
      Assert.AreEqual(a[1,1], b[1,1].Real);
    }

    //Test implicit conversion from null floatmatrix.
    [Test]
    public void ImplictToDoubleMatrixMatrixNull()
    {
      FloatMatrix a = null;
      ComplexFloatMatrix b = ComplexFloatMatrix.ToComplexFloatMatrix(a);
      Assert.IsTrue(b == null);
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
      
      ComplexFloatMatrix b = a;
      Assert.AreEqual(a.RowLength, b.RowLength);
      Assert.AreEqual(a.ColumnLength, b.ColumnLength);
      Assert.AreEqual(a[0,0], b[0,0].Real);
      Assert.AreEqual(a[0,1], b[0,1].Real);
      Assert.AreEqual(a[1,0], b[1,0].Real);
      Assert.AreEqual(a[1,1], b[1,1].Real);
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
      FloatMatrix a = new FloatMatrix(2,2);
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;
      
      ComplexFloatMatrix b = ComplexFloatMatrix.ToComplexFloatMatrix(a);
      Assert.AreEqual(a.RowLength, b.RowLength);
      Assert.AreEqual(a.ColumnLength, b.ColumnLength);
      Assert.AreEqual(a[0,0], b[0,0].Real);
      Assert.AreEqual(a[0,1], b[0,1].Real);
      Assert.AreEqual(a[1,0], b[1,0].Real);
      Assert.AreEqual(a[1,1], b[1,1].Real);
    }
    
    //Test implicit conversion from null floatmatrix.
    [Test]
    public void ImplictToFloatMatrixNull()
    {
      FloatMatrix a = null;
      ComplexFloatMatrix b = ComplexFloatMatrix.ToComplexFloatMatrix(a);
      Assert.IsTrue(b == null);
    }

    //Test implicit conversion from ComplexFloat mult dim array.
    [Test]
    public void ImplictComplexFloatMultArray()
    {
      ComplexFloat[,] a = new ComplexFloat[2,2];
      a[0,0] = new ComplexFloat(1,1);
      a[0,1] = new ComplexFloat(2,2);
      a[1,0] = new ComplexFloat(3,3);
      a[1,1] = new ComplexFloat(4,4);
      
      ComplexFloatMatrix b = a;
      Assert.AreEqual(a[0,0], b[0,0]);
      Assert.AreEqual(a[0,1], b[0,1]);
      Assert.AreEqual(a[1,0], b[1,0]);
      Assert.AreEqual(a[1,1], b[1,1]);
    }
    
    //Test implicit conversion from ComplexFloat mult dim array.
    [Test]
    public void ImplictToComplexFloatMultArray()
    {
      ComplexFloat[,] a = new ComplexFloat[2,2];
      a[0,0] = new ComplexFloat(1,1);
      a[0,1] = new ComplexFloat(2,2);
      a[1,0] = new ComplexFloat(3,3);
      a[1,1] = new ComplexFloat(4,4);
      
      ComplexFloatMatrix b = ComplexFloatMatrix.ToComplexFloatMatrix(a);
      Assert.AreEqual(a[0,0], b[0,0]);
      Assert.AreEqual(a[0,1], b[0,1]);
      Assert.AreEqual(a[1,0], b[1,0]);
      Assert.AreEqual(a[1,1], b[1,1]);
    }
    
    //Test implicit conversion from null ComplexFloat mult dim array.
    [Test]
    public void ImplictToComplexFloatMultArrayNull()
    {
      ComplexFloat[,] a = null;
      ComplexFloatMatrix b = ComplexFloatMatrix.ToComplexFloatMatrix(a);
      Assert.IsTrue(b == null);
    }

    //Test implicit conversion from float mult dim array.
    [Test]
    public void ImplictToDoubleMultArray()
    {
      float[,] a = new float[2,2];
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;

      ComplexFloatMatrix b = ComplexFloatMatrix.ToComplexFloatMatrix(a);
      Assert.AreEqual(a[0,0], b[0,0].Real);
      Assert.AreEqual(a[0,1], b[0,1].Real);
      Assert.AreEqual(a[1,0], b[1,0].Real);
      Assert.AreEqual(a[1,1], b[1,1].Real);
    }

    //Test implicit conversion from null float mult dim array.
    [Test]
    public void ImplictToDoubleMultArrayNull()
    {
      float[,] a = null;
      ComplexFloatMatrix b = ComplexFloatMatrix.ToComplexFloatMatrix(a);
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

      ComplexFloatMatrix b = ComplexFloatMatrix.ToComplexFloatMatrix(a);
      Assert.AreEqual(a[0,0], b[0,0].Real);
      Assert.AreEqual(a[0,1], b[0,1].Real);
      Assert.AreEqual(a[1,0], b[1,0].Real);
      Assert.AreEqual(a[1,1], b[1,1].Real);
    }

    //Test implicit conversion from null float mult dim array.
    [Test]
    public void ImplictToFloatMultArrayNull()
    {
      float[,] a = null;
      ComplexFloatMatrix b = ComplexFloatMatrix.ToComplexFloatMatrix(a);
      Assert.IsTrue(b == null);
    }


    //Test explicit conversion from ComplexDoubleMatrix
    [Test]
    public void ExplicitComplexDoubleMatrix()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2,2);
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;
      
      ComplexFloatMatrix b = (ComplexFloatMatrix)a;
      Assert.AreEqual(a.RowLength, b.RowLength);
      Assert.AreEqual(a.ColumnLength, b.ColumnLength);
      Assert.AreEqual(b[0,0], a[0,0]);
      Assert.AreEqual(b[0,1], a[0,1]);
      Assert.AreEqual(b[1,0], a[1,0]);
      Assert.AreEqual(b[1,1], a[1,1]);
    }

    //Test explicit conversion from ComplexDoubleMatrix
    [Test]
    public void ExplicitComplexDoubleMatrixNull()
    {
      ComplexDoubleMatrix a = null;
      ComplexFloatMatrix b = (ComplexFloatMatrix)a;
      Assert.IsTrue(b == null);
    }

    //Test explicit conversion from ComplexDoubleMatrix
    [Test]
    public void ExplicitToComplexDoubleMatrix()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2,2);
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;
      
      ComplexFloatMatrix b = ComplexFloatMatrix.ToComplexFloatMatrix(a);
      Assert.AreEqual(a.RowLength, b.RowLength);
      Assert.AreEqual(a.ColumnLength, b.ColumnLength);
      Assert.AreEqual(b[0,0], a[0,0]);
      Assert.AreEqual(b[0,1], a[0,1]);
      Assert.AreEqual(b[1,0], a[1,0]);
      Assert.AreEqual(b[1,1], a[1,1]);
    }

    //Test explicit conversion from DoubleMatrix
    [Test]
    public void ExplicitToComplexDoubleMatrixNull()
    {
      DoubleMatrix a = null;
      ComplexFloatMatrix b = ComplexFloatMatrix.ToComplexFloatMatrix(a);
      Assert.IsTrue(b == null);
    }
    
    //Test explicit conversion from Complex array
    [Test]
    public void ExplicitComplexDoubleMultArray()
    {
      Complex[,] a = new Complex[2,2];
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;
      
      ComplexFloatMatrix b = (ComplexFloatMatrix)a;
      Assert.AreEqual(2, b.RowLength);
      Assert.AreEqual(2, b.ColumnLength);
      Assert.AreEqual(b[0,0], a[0,0]);
      Assert.AreEqual(b[0,1], a[0,1]);
      Assert.AreEqual(b[1,0], a[1,0]);
      Assert.AreEqual(b[1,1], a[1,1]);
    }

    //Test explicit conversion from Complex array
    [Test]
    public void ExplicitComplexDoubleMultArrayNull()
    {
      Complex[,] a = null;
      ComplexFloatMatrix b = (ComplexFloatMatrix)a;
      Assert.IsTrue(b == null);
    }

    //Test explicit conversion from Complex array
    [Test]
    public void ExplicitToComplexDoubleMultArray()
    {
      Complex[,] a = new Complex[2,2];
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;
      
      ComplexFloatMatrix b = ComplexFloatMatrix.ToComplexFloatMatrix(a);
      Assert.AreEqual(2, b.RowLength);
      Assert.AreEqual(2, b.ColumnLength);
      Assert.AreEqual(a[0,0], b[0,0]);
      Assert.AreEqual(a[0,1], b[0,1]);
      Assert.AreEqual(a[1,0], b[1,0]);
      Assert.AreEqual(a[1,1], b[1,1]);
    }

    //Test explicit conversion from Complex array
    [Test]
    public void ExplicitToComplexDoubleMultArrayNull()
    {
      Complex[,] a = null;
      ComplexFloatMatrix b =ComplexFloatMatrix.ToComplexFloatMatrix(a);
      Assert.IsTrue(b == null);
    }

    //Test explicit conversion from Jagged Complex array
    [Test]
    public void ExplicitComplexDoubleJaggedArrayNull()
    {
      Complex[,] a = null;
      ComplexFloatMatrix b = (ComplexFloatMatrix)a;
      Assert.IsTrue(b == null);
    }

    //test equals method
    [Test]
    public void Equals()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2,2,new ComplexFloat(4,4));
      ComplexFloatMatrix b = new ComplexFloatMatrix(2,2,new ComplexFloat(4,4));
      ComplexFloatMatrix c = new ComplexFloatMatrix(2,2);
      c[0,0] = new ComplexFloat(4,4);
      c[0,1] = new ComplexFloat(4,4);
      c[1,0] = new ComplexFloat(4,4);
      c[1,1] = new ComplexFloat(4,4);

      ComplexFloatMatrix d = new ComplexFloatMatrix(2,2,5);
      ComplexFloatMatrix e = null;
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
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      a[0,0] = new ComplexFloat(1,1);
      a[0,1] = new ComplexFloat(2,2);
      a[1,0] = new ComplexFloat(3,3);
      a[1,1] = new ComplexFloat(4,4);

      int hash = a.GetHashCode();
      Assert.AreEqual(hash, 7);
    }
    
    //test ToArray
    [Test]
    public void ToArray()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      a[0,0] = new ComplexFloat(1,1);
      a[0,1] = new ComplexFloat(2,2);
      a[1,0] = new ComplexFloat(3,3);
      a[1,1] = new ComplexFloat(4,4);

      ComplexFloat[,] b = a.ToArray();

      Assert.AreEqual(a[0,0], b[0,0]);
      Assert.AreEqual(a[0,1], b[0,1]);
      Assert.AreEqual(a[1,0], b[1,0]);
      Assert.AreEqual(a[1,1], b[1,1]);
    }

    //test Transpose square
    [Test]
    public void TransposeSquare()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2,2);
      a[0,0] = new ComplexFloat(1);
      a[0,1] = new ComplexFloat(2);
      a[1,0] = new ComplexFloat(3);
      a[1,1] = new ComplexFloat(4);
      a.Transpose();
      Assert.AreEqual(a[0,0], new ComplexFloat(1));
      Assert.AreEqual(a[0,1], new ComplexFloat(3));
      Assert.AreEqual(a[1,0], new ComplexFloat(2));
      Assert.AreEqual(a[1,1], new ComplexFloat(4));
    }
    
    //test Transpose wide
    [Test]
    public void TransposeWide()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2,3);
      a[0,0] = new ComplexFloat(1);
      a[0,1] = new ComplexFloat(2);
      a[0,2] = new ComplexFloat(3);
      a[1,0] = new ComplexFloat(4);
      a[1,1] = new ComplexFloat(5);
      a[1,2] = new ComplexFloat(6);
      a.Transpose();
      Assert.AreEqual(a[0,0], new ComplexFloat(1));
      Assert.AreEqual(a[0,1], new ComplexFloat(4));
      Assert.AreEqual(a[1,0], new ComplexFloat(2));
      Assert.AreEqual(a[1,1], new ComplexFloat(5));
      Assert.AreEqual(a[2,0], new ComplexFloat(3));
      Assert.AreEqual(a[2,1], new ComplexFloat(6));
      Assert.AreEqual(a.RowLength, 3);
      Assert.AreEqual(a.ColumnLength, 2);
    }

    //test Transpose long
    [Test]
    public void TransposeLong()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(3,2);
      a[0,0] = new ComplexFloat(1);
      a[0,1] = new ComplexFloat(2);
      a[1,0] = new ComplexFloat(3);
      a[1,1] = new ComplexFloat(4);
      a[2,0] = new ComplexFloat(5);
      a[2,1] = new ComplexFloat(6);
      a.Transpose();
      Assert.AreEqual(a[0,0], new ComplexFloat(1));
      Assert.AreEqual(a[0,1], new ComplexFloat(3));
      Assert.AreEqual(a[0,2], new ComplexFloat(5));
      Assert.AreEqual(a[1,0], new ComplexFloat(2));
      Assert.AreEqual(a[1,1], new ComplexFloat(4));
      Assert.AreEqual(a[1,2], new ComplexFloat(6));
      Assert.AreEqual(a.RowLength, 2);
      Assert.AreEqual(a.ColumnLength, 3);
    }
    
    //test GetTranspose square
    [Test]
    public void GetTransposeSquare()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2,2);
      a[0,0] = new ComplexFloat(1);
      a[0,1] = new ComplexFloat(2);
      a[1,0] = new ComplexFloat(3);
      a[1,1] = new ComplexFloat(4);
      ComplexFloatMatrix b = a.GetTranspose();
      Assert.AreEqual(b[0,0], a[0,0]);
      Assert.AreEqual(b[0,1], a[1,0]);
      Assert.AreEqual(b[1,0], a[0,1]);
      Assert.AreEqual(b[1,1], a[1,1]);
    }
    
    //test GetTranspose wide
    [Test]
    public void GetTransposeWide()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2,3);
      a[0,0] = new ComplexFloat(1);
      a[0,1] = new ComplexFloat(2);
      a[0,2] = new ComplexFloat(3);
      a[1,0] = new ComplexFloat(4);
      a[1,1] = new ComplexFloat(5);
      a[1,2] = new ComplexFloat(6);
      ComplexFloatMatrix b = a.GetTranspose();
      Assert.AreEqual(b[0,0], a[0,0]);
      Assert.AreEqual(b[0,1], a[1,0]);
      Assert.AreEqual(b[1,0], a[0,1]);
      Assert.AreEqual(b[1,1], a[1,1]);
      Assert.AreEqual(b[2,0], a[0,2]);
      Assert.AreEqual(b[2,1], a[1,2]);
      Assert.AreEqual(b.RowLength, a.ColumnLength);
      Assert.AreEqual(b.ColumnLength, a.RowLength);
    }

    //test GetTranspose long
    [Test]
    public void GetTransposeLong()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(3,2);
      a[0,0] = new ComplexFloat(1);
      a[0,1] = new ComplexFloat(2);
      a[1,0] = new ComplexFloat(3);
      a[1,1] = new ComplexFloat(4);
      a[2,0] = new ComplexFloat(5);
      a[2,1] = new ComplexFloat(6);
      ComplexFloatMatrix b = a.GetTranspose();
      Assert.AreEqual(b[0,0], a[0,0]);
      Assert.AreEqual(b[0,1], a[1,0]);
      Assert.AreEqual(b[0,2], a[2,0]);
      Assert.AreEqual(b[1,0], a[0,1]);
      Assert.AreEqual(b[1,1], a[1,1]);
      Assert.AreEqual(b[1,2], a[2,1]);
      Assert.AreEqual(b.RowLength, a.ColumnLength);
      Assert.AreEqual(b.ColumnLength, a.RowLength);
    }

    //test Invert
    [Test]
    public void Invert()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2,2);
      a[0,0] = new ComplexFloat(2);
      a[0,1] = new ComplexFloat(4);
      a[1,0] = new ComplexFloat(3);
      a[1,1] = new ComplexFloat(7);
      a.Invert();
      Assert.AreEqual(a[0,0].Real, 3.500,TOLERENCE);
      Assert.AreEqual(a[0,1].Real, -2.000,TOLERENCE);
      Assert.AreEqual(a[1,0].Real, -1.500,TOLERENCE);
      Assert.AreEqual(a[1,1].Real, 1.000,TOLERENCE);
    }

    //test Invert singular
    [Test]
    [ExpectedException(typeof(SingularMatrixException))]
    public void InvertSingular()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2,2);
      a.Invert();
    }

    //test Invert not square
    [Test]
    [ExpectedException(typeof(NotSquareMatrixException))]
    public void InvertNotSquare()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(3,2);
      a[0,0] = new ComplexFloat(2);
      a[0,1] = new ComplexFloat(4);
      a[1,0] = new ComplexFloat(3);
      a[1,1] = new ComplexFloat(7);
      a[2,0] = new ComplexFloat(5);
      a[2,1] = new ComplexFloat(5);
      a.Invert();
    }

    //test GetInverse singular
    [Test]
    [ExpectedException(typeof(SingularMatrixException))]
    public void GetInverseSingular()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2,2);
      ComplexFloatMatrix b = a.GetInverse();
    }

    //test GetInverse not square
    [Test]
    [ExpectedException(typeof(NotSquareMatrixException))]
    public void GetInverseNotSquare()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(3,2);
      a[0,0] = new ComplexFloat(2);
      a[0,1] = new ComplexFloat(4);
      a[1,0] = new ComplexFloat(3);
      a[1,1] = new ComplexFloat(7);
      a[2,0] = new ComplexFloat(5);
      a[2,1] = new ComplexFloat(5);
      ComplexFloatMatrix b = a.GetInverse();
    }
    //test GetInverse
    [Test]
    public void GetInverse()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2,2);
      a[0,0] = new ComplexFloat(2);
      a[0,1] = new ComplexFloat(4);
      a[1,0] = new ComplexFloat(3);
      a[1,1] = new ComplexFloat(7);
      ComplexFloatMatrix b = a.GetInverse();
      Assert.AreEqual(b[0,0].Real, 3.500,TOLERENCE);
      Assert.AreEqual(b[0,1].Real, -2.000,TOLERENCE);
      Assert.AreEqual(b[1,0].Real, -1.500,TOLERENCE);
      Assert.AreEqual(b[1,1].Real, 1.000,TOLERENCE);
    }   

    //test GetDeterminant
    [Test]
    public void GetDeterminant()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2,2);
      a[0,0] = new ComplexFloat(2);
      a[0,1] = new ComplexFloat(4);
      a[1,0] = new ComplexFloat(3);
      a[1,1] = new ComplexFloat(7);
      ComplexFloat b = a.GetDeterminant();
      Complex test = new Complex(2);
      Assert.AreEqual(b.Real, test.Real,4);
      Assert.AreEqual(b.Imag, test.Imag,4);
    }

    //test GetDeterminant
    [Test]
    [ExpectedException(typeof(NotSquareMatrixException))]
    public void GetDeterminantNotSquare()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(3,2);
      a[0,0] = new ComplexFloat(2);
      a[0,1] = new ComplexFloat(4);
      a[1,0] = new ComplexFloat(3);
      a[1,1] = new ComplexFloat(7);
      a[2,0] = new ComplexFloat(5);
      a[2,1] = new ComplexFloat(5);
      ComplexFloat b = a.GetDeterminant();
    }   

    //test GetRow
    [Test]
    public void GetRow()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2,2);
      a[0,0] = new ComplexFloat(1);
      a[0,1] = new ComplexFloat(2);
      a[1,0] = new ComplexFloat(3);
      a[1,1] = new ComplexFloat(4);
      ComplexFloatVector b = a.GetRow(0);
      Assert.AreEqual(b[0], a[0,0]);
      Assert.AreEqual(b[1], a[0,1]);
    }
    
    //test GetRow
    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetRowOutOfRange()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2,2);
      ComplexFloatVector b = a.GetRow(3);
    }
    
    //test GetColumn
    [Test]
    public void GetColumn()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2,2);
      a[0,0] = new ComplexFloat(1);
      a[0,1] = new ComplexFloat(2);
      a[1,0] = new ComplexFloat(3);
      a[1,1] = new ComplexFloat(4);
      ComplexFloatVector b = a.GetColumn(0);
      Assert.AreEqual(b[0], a[0,0]);
      Assert.AreEqual(b[1], a[1,0]);
    }
    
    //test GetColumn
    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetColumnOutOfRange()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2,2);
      ComplexFloatVector b = a.GetColumn(3);
    } 

    //test GetDiagonal
    [Test]
    public void GetDiagonal()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2,2);
      a[0,0] = new ComplexFloat(1);
      a[0,1] = new ComplexFloat(2);
      a[1,0] = new ComplexFloat(3);
      a[1,1] = new ComplexFloat(4);
      ComplexFloatVector b = a.GetDiagonal();
      Assert.AreEqual(b[0], a[0,0]);
      Assert.AreEqual(b[1], a[1,1]);
    }

    //test SetRow
    [Test]
    public void SetRow()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2,2);
      ComplexFloatVector b = new ComplexFloatVector(2);
      b[0] = new ComplexFloat(1,1);
      b[1] = new ComplexFloat(2,2);
      a.SetRow(0,b);
      Assert.AreEqual(b[0], a[0,0]);
      Assert.AreEqual(b[1], a[0,1]);
    }

    //test SetRow
    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void SetRowOutOfRange()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2,2);
      ComplexFloatVector b = new ComplexFloatVector(2);
      a.SetRow(2,b);
    }

    //test SetRow
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void SetRowWrongRank()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2,2);
      ComplexFloatVector b = new ComplexFloatVector(3);
      a.SetRow(1,b);
    }

    //test SetRow
    [Test]
    public void SetRowArray()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2,2);
      ComplexFloat[] b = new ComplexFloat[2];
      b[0] = new ComplexFloat(1,1);
      b[1] = new ComplexFloat(2,2);

      a.SetRow(0,b);
      Assert.AreEqual(b[0], a[0,0]);
      Assert.AreEqual(b[1], a[0,1]);
    }

    //test SetRow
    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void SetRowArrayOutOfRange()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2,2);
      ComplexFloat[] b = new ComplexFloat[2];
      a.SetRow(2,b);
    }

    //test SetRow
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void SetRowArrayWrongRank()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2,2);
      ComplexFloat[] b = new ComplexFloat[3];
      a.SetRow(1,b);
    }

    //test SetColumn
    [Test]
    public void SetColumn()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2,2);
      ComplexFloatVector b = new ComplexFloatVector(2);
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
      ComplexFloatMatrix a = new ComplexFloatMatrix(2,2);
      ComplexFloatVector b = new ComplexFloatVector(2);
      a.SetColumn(2,b);
    }

    //test SetColumn
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void SetColumnWrongRank()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2,2);
      ComplexFloatVector b = new ComplexFloatVector(3);
      a.SetColumn(1,b);
    }

    //test SetColumn
    [Test]
    public void SetColumnArray()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2,2);
      ComplexFloat[] b = new ComplexFloat[2];
      b[0] = new ComplexFloat(1,1);
      b[1] = new ComplexFloat(2,2);
      a.SetColumn(0,b);
      Assert.AreEqual(b[0], a[0,0]);
      Assert.AreEqual(b[1], a[1,0]);
    }

    //test SetColumn
    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void SetColumnArrayOutOfRange()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2,2);
      ComplexFloat[] b = new ComplexFloat[2];
      a.SetColumn(2,b);
    }

    //test SetColumn
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void SetColumnArrayWrongRank()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2,2);
      ComplexFloat[] b = new ComplexFloat[3];
      a.SetColumn(1,b);
    }

    //test SetDiagonal
    [Test]
    public void SetDiagonal()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2,2);
      ComplexFloatVector b = new ComplexFloatVector(2);
      b[0] = new ComplexFloat(1);
      b[1] = new ComplexFloat(2);
      a.SetDiagonal(b);
      Assert.AreEqual(b[0], a[0,0]);
      Assert.AreEqual(b[1], a[1,1]);
    }
    
    //test GetSubMatrix
    [Test]
    public void GetSubMatrix()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(4);
      a[0,0] = new ComplexFloat(1);
      a[0,1] = new ComplexFloat(2);
      a[0,2] = new ComplexFloat(3);
      a[0,3] = new ComplexFloat(4);
      a[1,0] = new ComplexFloat(5);
      a[1,1] = new ComplexFloat(6);
      a[1,2] = new ComplexFloat(7);
      a[1,3] = new ComplexFloat(8);
      a[2,0] = new ComplexFloat(9);
      a[2,1] = new ComplexFloat(10);
      a[2,2] = new ComplexFloat(11);
      a[2,3] = new ComplexFloat(12);
      a[3,0] = new ComplexFloat(13);
      a[3,1] = new ComplexFloat(14);
      a[3,2] = new ComplexFloat(15);
      a[3,3] = new ComplexFloat(16);
      ComplexFloatMatrix b = a.GetSubMatrix(2,2);
      ComplexFloatMatrix c = a.GetSubMatrix(0,1,2,2);
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
      ComplexFloatMatrix a = new ComplexFloatMatrix(4);
      ComplexFloatMatrix b = a.GetSubMatrix(-1,2);
    }
    //test GetSubMatrix
    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetSubMatrixOutRange2()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(4);
      ComplexFloatMatrix b = a.GetSubMatrix(2,4);
    }

    //test GetSubMatrix
    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetSubMatrixOutRange3()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(4);
      ComplexFloatMatrix b = a.GetSubMatrix(0,0,4,2);
    }

    //test GetSubMatrix
    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetSubMatrixOutRange4()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(4);
      ComplexFloatMatrix b = a.GetSubMatrix(0,0,2,4);
    }

    //test GetSubMatrix
    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetSubMatrixOutRange5()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(4);
      ComplexFloatMatrix b = a.GetSubMatrix(0,3,2,2);
    }
    
    //test GetUpperTriangle square matrix
    [Test]
    public void GetUpperTriangleSquare()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(3);
      a[0,0] = new ComplexFloat(1);
      a[0,1] = new ComplexFloat(2);
      a[0,2] = new ComplexFloat(3);
      a[1,0] = new ComplexFloat(4);
      a[1,1] = new ComplexFloat(5);
      a[1,2] = new ComplexFloat(6);
      a[2,0] = new ComplexFloat(7);
      a[2,1] = new ComplexFloat(8);
      a[2,2] = new ComplexFloat(9);
      ComplexFloatMatrix b = a.GetUpperTriangle();
      
      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0,0], a[0,0]);
      Assert.AreEqual(b[0,1], a[0,1]);
      Assert.AreEqual(b[0,2], a[0,2]);
      Assert.AreEqual(b[1,0], ComplexFloat.Zero);
      Assert.AreEqual(b[1,1], a[1,1]);
      Assert.AreEqual(b[1,2], a[1,2]);
      Assert.AreEqual(b[2,0], ComplexFloat.Zero);
      Assert.AreEqual(b[2,1], ComplexFloat.Zero);   
      Assert.AreEqual(b[2,2], a[2,2]);
    }
    
    //test GetUpperTriangle long matrix
    [Test]
    public void GetUpperTriangleLong()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(3,2);
      a[0,0] = new ComplexFloat(1);
      a[0,1] = new ComplexFloat(2);
      a[1,0] = new ComplexFloat(4);
      a[1,1] = new ComplexFloat(5);
      a[2,0] = new ComplexFloat(7);
      a[2,1] = new ComplexFloat(8);
      ComplexFloatMatrix b = a.GetUpperTriangle();
      
      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0,0], a[0,0]);
      Assert.AreEqual(b[0,1], a[0,1]);
      Assert.AreEqual(b[1,0], ComplexFloat.Zero);
      Assert.AreEqual(b[1,1], a[1,1]);
      Assert.AreEqual(b[2,0], ComplexFloat.Zero);
      Assert.AreEqual(b[2,1], ComplexFloat.Zero);   
    }

    //test GetUpperTriangle wide matrix
    [Test]
    public void GetUpperTriangleWide()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2,3);
      a[0,0] = new ComplexFloat(1);
      a[0,1] = new ComplexFloat(2);
      a[0,2] = new ComplexFloat(3);
      a[1,0] = new ComplexFloat(4);
      a[1,1] = new ComplexFloat(5);
      a[1,2] = new ComplexFloat(6);
      ComplexFloatMatrix b = a.GetUpperTriangle();
      
      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0,0], a[0,0]);
      Assert.AreEqual(b[0,1], a[0,1]);
      Assert.AreEqual(b[0,2], a[0,2]);
      Assert.AreEqual(b[1,0], ComplexFloat.Zero);
      Assert.AreEqual(b[1,1], a[1,1]);
      Assert.AreEqual(b[1,2], a[1,2]);
    }

    //test GetStrictlyUpperTriangle square matrix
    [Test]
    public void GetStrictlyUpperTriangleSquare()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(3);
      a[0,0] = new ComplexFloat(1);
      a[0,1] = new ComplexFloat(2);
      a[0,2] = new ComplexFloat(3);
      a[1,0] = new ComplexFloat(4);
      a[1,1] = new ComplexFloat(5);
      a[1,2] = new ComplexFloat(6);
      a[2,0] = new ComplexFloat(7);
      a[2,1] = new ComplexFloat(8);
      a[2,2] = new ComplexFloat(9);
      ComplexFloatMatrix b = a.GetStrictlyUpperTriangle();
      
      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0,0], ComplexFloat.Zero);
      Assert.AreEqual(b[0,1], a[0,1]);
      Assert.AreEqual(b[0,2], a[0,2]);
      Assert.AreEqual(b[1,0], ComplexFloat.Zero);
      Assert.AreEqual(b[1,1], ComplexFloat.Zero);
      Assert.AreEqual(b[1,2], a[1,2]);
      Assert.AreEqual(b[2,0], ComplexFloat.Zero);
      Assert.AreEqual(b[2,1], ComplexFloat.Zero);   
      Assert.AreEqual(b[2,2], ComplexFloat.Zero);
    }
    
    //test GetStrictlyUpperTriangle long matrix
    [Test]
    public void GetStrictlyUpperTriangleLong()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(3,2);
      a[0,0] = new ComplexFloat(1);
      a[0,1] = new ComplexFloat(2);
      a[1,0] = new ComplexFloat(4);
      a[1,1] = new ComplexFloat(5);
      a[2,0] = new ComplexFloat(7);
      a[2,1] = new ComplexFloat(8);
      ComplexFloatMatrix b = a.GetStrictlyUpperTriangle();
      
      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0,0], ComplexFloat.Zero);
      Assert.AreEqual(b[0,1], a[0,1]);
      Assert.AreEqual(b[1,0], ComplexFloat.Zero);
      Assert.AreEqual(b[1,1], ComplexFloat.Zero);
      Assert.AreEqual(b[2,0], ComplexFloat.Zero);
      Assert.AreEqual(b[2,1], ComplexFloat.Zero);   
    }

    //test GetStrictlyUpperTriangle wide matrix
    [Test]
    public void GetStrictlyUpperTriangleWide()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2,3);
      a[0,0] = new ComplexFloat(1);
      a[0,1] = new ComplexFloat(2);
      a[0,2] = new ComplexFloat(3);
      a[1,0] = new ComplexFloat(4);
      a[1,1] = new ComplexFloat(5);
      a[1,2] = new ComplexFloat(6);
      ComplexFloatMatrix b = a.GetStrictlyUpperTriangle();
      
      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0,0], ComplexFloat.Zero);
      Assert.AreEqual(b[0,1], a[0,1]);
      Assert.AreEqual(b[0,2], a[0,2]);
      Assert.AreEqual(b[1,0], ComplexFloat.Zero);
      Assert.AreEqual(b[1,1], ComplexFloat.Zero);
      Assert.AreEqual(b[1,2], a[1,2]);
    }


    //test GetLowerTriangle square matrix
    [Test]
    public void GetLowerTriangleSquare()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(3);
      a[0,0] = new ComplexFloat(1);
      a[0,1] = new ComplexFloat(2);
      a[0,2] = new ComplexFloat(3);
      a[1,0] = new ComplexFloat(4);
      a[1,1] = new ComplexFloat(5);
      a[1,2] = new ComplexFloat(6);
      a[2,0] = new ComplexFloat(7);
      a[2,1] = new ComplexFloat(8);
      a[2,2] = new ComplexFloat(9);
      ComplexFloatMatrix b = a.GetLowerTriangle();
      
      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0,0], a[0,0]);
      Assert.AreEqual(b[0,1], ComplexFloat.Zero);
      Assert.AreEqual(b[0,2], ComplexFloat.Zero);
      Assert.AreEqual(b[1,0], a[1,0]);
      Assert.AreEqual(b[1,1], a[1,1]);
      Assert.AreEqual(b[1,2], ComplexFloat.Zero);
      Assert.AreEqual(b[2,0], a[2,0]);
      Assert.AreEqual(b[2,1], a[2,1]);    
      Assert.AreEqual(b[2,2], a[2,2]);
    }
    
    //test GetLowerTriangle long matrix
    [Test]
    public void GetLowerTriangleLong()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(3,2);
      a[0,0] = new ComplexFloat(1);
      a[0,1] = new ComplexFloat(2);
      a[1,0] = new ComplexFloat(4);
      a[1,1] = new ComplexFloat(5);
      a[2,0] = new ComplexFloat(7);
      a[2,1] = new ComplexFloat(8);
      ComplexFloatMatrix b = a.GetLowerTriangle();
      
      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0,0], a[0,0]);
      Assert.AreEqual(b[0,1], ComplexFloat.Zero);
      Assert.AreEqual(b[1,0], b[1,0]);
      Assert.AreEqual(b[1,1], a[1,1]);
      Assert.AreEqual(b[2,0], b[2,0]);
      Assert.AreEqual(b[2,1], b[2,1]);    
    }

    //test GetLowerTriangle wide matrix
    [Test]
    public void GetLowerTriangleWide()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2,3);
      a[0,0] = new ComplexFloat(1);
      a[0,1] = new ComplexFloat(2);
      a[0,2] = new ComplexFloat(3);
      a[1,0] = new ComplexFloat(4);
      a[1,1] = new ComplexFloat(5);
      a[1,2] = new ComplexFloat(6);
      ComplexFloatMatrix b = a.GetLowerTriangle();
      
      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0,0], a[0,0]);
      Assert.AreEqual(b[0,1], ComplexFloat.Zero);
      Assert.AreEqual(b[0,2], ComplexFloat.Zero);
      Assert.AreEqual(b[1,0], a[1,0]);
      Assert.AreEqual(b[1,1], a[1,1]);
      Assert.AreEqual(b[1,2], ComplexFloat.Zero);
    }

    //test GetStrictlyLowerTriangle square matrix
    [Test]
    public void GetStrictlyLowerTriangleSquare()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(3);
      a[0,0] = new ComplexFloat(1);
      a[0,1] = new ComplexFloat(2);
      a[0,2] = new ComplexFloat(3);
      a[1,0] = new ComplexFloat(4);
      a[1,1] = new ComplexFloat(5);
      a[1,2] = new ComplexFloat(6);
      a[2,0] = new ComplexFloat(7);
      a[2,1] = new ComplexFloat(8);
      a[2,2] = new ComplexFloat(9);
      ComplexFloatMatrix b = a.GetStrictlyLowerTriangle();
      
      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0,0], ComplexFloat.Zero);
      Assert.AreEqual(b[0,1], ComplexFloat.Zero);
      Assert.AreEqual(b[0,2], ComplexFloat.Zero);
      Assert.AreEqual(b[1,0], a[1,0]);
      Assert.AreEqual(b[1,1], ComplexFloat.Zero);
      Assert.AreEqual(b[1,2], ComplexFloat.Zero);
      Assert.AreEqual(b[2,0], a[2,0]);
      Assert.AreEqual(b[2,1], a[2,1]);    
      Assert.AreEqual(b[2,2], ComplexFloat.Zero);
    }
    
    //test GetStrictlyLowerTriangle long matrix
    [Test]
    public void GetStrictlyLowerTriangleLong()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(3,2);
      a[0,0] = new ComplexFloat(1);
      a[0,1] = new ComplexFloat(2);
      a[1,0] = new ComplexFloat(4);
      a[1,1] = new ComplexFloat(5);
      a[2,0] = new ComplexFloat(7);
      a[2,1] = new ComplexFloat(8);
      ComplexFloatMatrix b = a.GetStrictlyLowerTriangle();
      
      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0,0], ComplexFloat.Zero);
      Assert.AreEqual(b[0,1], ComplexFloat.Zero);
      Assert.AreEqual(b[1,0], b[1,0]);
      Assert.AreEqual(b[1,1], ComplexFloat.Zero);
      Assert.AreEqual(b[2,0], b[2,0]);
      Assert.AreEqual(b[2,1], b[2,1]);  
    }

    //test GetStrictlyLowerTriangle wide matrix
    [Test]
    public void GetStrictlyLowerTriangleWide()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2,3);
      a[0,0] = new ComplexFloat(1);
      a[0,1] = new ComplexFloat(2);
      a[0,2] = new ComplexFloat(3);
      a[1,0] = new ComplexFloat(4);
      a[1,1] = new ComplexFloat(5);
      a[1,2] = new ComplexFloat(6);
      ComplexFloatMatrix b = a.GetStrictlyLowerTriangle();
      
      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0,0], ComplexFloat.Zero);
      Assert.AreEqual(b[0,1], ComplexFloat.Zero);
      Assert.AreEqual(b[0,2], ComplexFloat.Zero);
      Assert.AreEqual(b[1,0], a[1,0]);
      Assert.AreEqual(b[1,1], ComplexFloat.Zero);
      Assert.AreEqual(b[1,2], ComplexFloat.Zero);
    }
    
    //static Negate
    [Test]
    public void Negate()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      a[0,0] = new ComplexFloat(1);
      a[0,1] = new ComplexFloat(2);
      a[1,0] = new ComplexFloat(3);
      a[1,1] = new ComplexFloat(4);

      ComplexFloatMatrix b = ComplexFloatMatrix.Negate(a);
      Assert.AreEqual(b[0,0],  new ComplexFloat(-1));
      Assert.AreEqual(b[0,1],  new ComplexFloat(-2));
      Assert.AreEqual(b[1,0],  new ComplexFloat(-3));
      Assert.AreEqual(b[1,1],  new ComplexFloat(-4));
    }
    
    //static NegateNull
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void NegateNull()
    {
      ComplexFloatMatrix a = null;
      ComplexFloatMatrix b = ComplexFloatMatrix.Negate(a);
    }

    //static operator -
    [Test]
    public void OperatorMinus()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      a[0,0] = new ComplexFloat(1);
      a[0,1] = new ComplexFloat(2);
      a[1,0] = new ComplexFloat(3);
      a[1,1] = new ComplexFloat(4);

      ComplexFloatMatrix b = -a;
      Assert.AreEqual(b[0,0],  new ComplexFloat(-1));
      Assert.AreEqual(b[0,1],  new ComplexFloat(-2));
      Assert.AreEqual(b[1,0],  new ComplexFloat(-3));
      Assert.AreEqual(b[1,1],  new ComplexFloat(-4));
    }

    //static operator - null
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void OperatorMinusNull()
    {
      ComplexFloatMatrix a = null;
      ComplexFloatMatrix b = -a;
    }   


    //static subtact two square matrices
    [Test]
    public void StaticSubtract()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      ComplexFloatMatrix b = new ComplexFloatMatrix(2);
      a[0,0] = b[0,0] = new ComplexFloat(1);
      a[0,1] = b[0,1] = new ComplexFloat(2);
      a[1,0] = b[1,0] = new ComplexFloat(3);
      a[1,1] = b[1,1] = new ComplexFloat(4);
      ComplexFloatMatrix c = ComplexFloatMatrix.Subtract(a,b);
      Assert.AreEqual(c[0,0],ComplexFloat.Zero);
      Assert.AreEqual(c[0,1],ComplexFloat.Zero);
      Assert.AreEqual(c[1,0],ComplexFloat.Zero);
      Assert.AreEqual(c[1,1],ComplexFloat.Zero);
    }

    //operator subtract two square matrices
    [Test]
    public void OperatorSubtract()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      ComplexFloatMatrix b = new ComplexFloatMatrix(2);
      a[0,0] = b[0,0] = new ComplexFloat(1);
      a[0,1] = b[0,1] = new ComplexFloat(2);
      a[1,0] = b[1,0] = new ComplexFloat(3);
      a[1,1] = b[1,1] = new ComplexFloat(4);
      ComplexFloatMatrix c = a-b;
      Assert.AreEqual(c[0,0],ComplexFloat.Zero);
      Assert.AreEqual(c[0,1],ComplexFloat.Zero);
      Assert.AreEqual(c[1,0],ComplexFloat.Zero);
      Assert.AreEqual(c[1,1], ComplexFloat.Zero);
    }
  
    //member add subtract square matrices
    [Test]
    public void MemberSubtract()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      ComplexFloatMatrix b = new ComplexFloatMatrix(2);
      a[0,0] = b[0,0] = new ComplexFloat(1);
      a[0,1] = b[0,1] = new ComplexFloat(2);
      a[1,0] = b[1,0] = new ComplexFloat(3);
      a[1,1] = b[1,1] = new ComplexFloat(4);
      a.Subtract(b);
      Assert.AreEqual(a[0,0],ComplexFloat.Zero);
      Assert.AreEqual(a[0,1],ComplexFloat.Zero);
      Assert.AreEqual(a[1,0],ComplexFloat.Zero);
      Assert.AreEqual(a[1,1],ComplexFloat.Zero);
    }
    
    //static Subtract two square matrices, one null
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void StaticSubtractNull()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      ComplexFloatMatrix b = null;
      ComplexFloatMatrix c = ComplexFloatMatrix.Subtract(a,b);
    }

    //operator Subtract two square matrices, one null
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void OperatorSubtractNull()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      ComplexFloatMatrix b = null;
      ComplexFloatMatrix c = a-b;
    }
  
    //member Subtract two square matrices, one null
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void MemberSubtractNull()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      ComplexFloatMatrix b = null;
      a.Subtract(b);
    }

    //static Subtract two incompatible matrices
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void StaticSubtractIncompatible()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      ComplexFloatMatrix b = new ComplexFloatMatrix(3);
      ComplexFloatMatrix c = ComplexFloatMatrix.Subtract(a,b);
    }

    //operator Subtract two  incompatible matrices
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void OperatorSubtractIncompatible()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      ComplexFloatMatrix b = new ComplexFloatMatrix(3);
      ComplexFloatMatrix c = a-b;
    }
  
    //member Subtract two  incompatible matricess
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void MemberSubtractIncompatible()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      ComplexFloatMatrix b = new ComplexFloatMatrix(3);
      a.Subtract(b);
    }

    //static add two square matrices
    [Test]
    public void StaticAdd()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      ComplexFloatMatrix b = new ComplexFloatMatrix(2);
      a[0,0] = b[0,0] = new ComplexFloat(1);
      a[0,1] = b[0,1] = new ComplexFloat(2);
      a[1,0] = b[1,0] = new ComplexFloat(3);
      a[1,1] = b[1,1] = new ComplexFloat(4);
      ComplexFloatMatrix c = ComplexFloatMatrix.Add(a,b);
      Assert.AreEqual(c[0,0],new ComplexFloat(2));
      Assert.AreEqual(c[0,1],new ComplexFloat(4));
      Assert.AreEqual(c[1,0],new ComplexFloat(6));
      Assert.AreEqual(c[1,1],new ComplexFloat(8));
    }

    //operator add two square matrices
    [Test]
    public void OperatorAdd()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      ComplexFloatMatrix b = new ComplexFloatMatrix(2);
      a[0,0] = b[0,0] = new ComplexFloat(1);
      a[0,1] = b[0,1] = new ComplexFloat(2);
      a[1,0] = b[1,0] = new ComplexFloat(3);
      a[1,1] = b[1,1] = new ComplexFloat(4);
      ComplexFloatMatrix c = a+b;
      Assert.AreEqual(c[0,0],new ComplexFloat(2));
      Assert.AreEqual(c[0,1],new ComplexFloat(4));
      Assert.AreEqual(c[1,0],new ComplexFloat(6));
      Assert.AreEqual(c[1,1],new ComplexFloat(8));
    }
  
    //member add two square matrices
    [Test]
    public void MemberAdd()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      ComplexFloatMatrix b = new ComplexFloatMatrix(2);
      a[0,0] = b[0,0] = new ComplexFloat(1);
      a[0,1] = b[0,1] = new ComplexFloat(2);
      a[1,0] = b[1,0] = new ComplexFloat(3);
      a[1,1] = b[1,1] = new ComplexFloat(4);
      a.Add(b);
      Assert.AreEqual(a[0,0],new ComplexFloat(2));
      Assert.AreEqual(a[0,1],new ComplexFloat(4));
      Assert.AreEqual(a[1,0],new ComplexFloat(6));
      Assert.AreEqual(a[1,1],new ComplexFloat(8));
    }
    
    //static add two square matrices, one null
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void StaticAddNull()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      ComplexFloatMatrix b = null;
      ComplexFloatMatrix c = ComplexFloatMatrix.Add(a,b);
    }

    //operator add two square matrices, one null
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void OperatorAddNull()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      ComplexFloatMatrix b = null;
      ComplexFloatMatrix c = a+b;
    }
  
    //member add two square matrices, one null
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void MemberAddNull()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      ComplexFloatMatrix b = null;
      a.Add(b);
    }

    //static add two incompatible matrices
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void StaticAddIncompatible()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      ComplexFloatMatrix b = new ComplexFloatMatrix(3);
      ComplexFloatMatrix c = ComplexFloatMatrix.Add(a,b);
    }

    //operator add two  incompatible matrices
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void OperatorAddIncompatible()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      ComplexFloatMatrix b = new ComplexFloatMatrix(3);
      ComplexFloatMatrix c = a+b;
    }
  
    //member add two  incompatible matricess
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void MemberAddIncompatible()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      ComplexFloatMatrix b = new ComplexFloatMatrix(3);
      a.Add(b);
    }


    //static divide matrix by float
    [Test]
    public void StaticDivide()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      a[0,0] = new ComplexFloat(2);
      a[0,1] = new ComplexFloat(4);
      a[1,0] = new ComplexFloat(6);
      a[1,1] = new ComplexFloat(8);
      ComplexFloatMatrix b =  ComplexFloatMatrix.Divide(a,2);
      Assert.AreEqual(b[0,0],new ComplexFloat(1));
      Assert.AreEqual(b[0,1],new ComplexFloat(2));
      Assert.AreEqual(b[1,0],new ComplexFloat(3));
      Assert.AreEqual(b[1,1],new ComplexFloat(4));
    }

    //operator divide matrix by float
    [Test]
    public void OperatorDivide()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      a[0,0] = new ComplexFloat(2);
      a[0,1] = new ComplexFloat(4);
      a[1,0] = new ComplexFloat(6);
      a[1,1] = new ComplexFloat(8);
      ComplexFloatMatrix b = a/2;
      Assert.AreEqual(b[0,0],new ComplexFloat(1));
      Assert.AreEqual(b[0,1],new ComplexFloat(2));
      Assert.AreEqual(b[1,0],new ComplexFloat(3));
      Assert.AreEqual(b[1,1],new ComplexFloat(4));
    }
  
    //member divide matrix by float
    [Test]
    public void MemberDivide()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      a[0,0] = new ComplexFloat(2);
      a[0,1] = new ComplexFloat(4);
      a[1,0] = new ComplexFloat(6);
      a[1,1] = new ComplexFloat(8);
      a.Divide(2);
      Assert.AreEqual(a[0,0],new ComplexFloat(1));
      Assert.AreEqual(a[0,1],new ComplexFloat(2));
      Assert.AreEqual(a[1,0],new ComplexFloat(3));
      Assert.AreEqual(a[1,1],new ComplexFloat(4));
    }
    
    //static divide null matrix by float
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void StaticDivideNull()
    {
      ComplexFloatMatrix a = null;
      ComplexFloatMatrix b = ComplexFloatMatrix.Divide(a,2);
    }

    //operator divide null matrix by float
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void OperatorDivideNull()
    {
      ComplexFloatMatrix a = null;
      ComplexFloatMatrix b = a/2;
    }

    //copy
    [Test]
    public void Copy()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      a[0,0] = new ComplexFloat(1);
      a[0,1] = new ComplexFloat(2);
      a[1,0] = new ComplexFloat(3);
      a[1,1] = new ComplexFloat(4);
      ComplexFloatMatrix b = new ComplexFloatMatrix(2);
      b.Copy(a);
      Assert.AreEqual(a[0,0],a[0,0]);
      Assert.AreEqual(a[0,1],b[0,1]);
      Assert.AreEqual(a[1,0],b[1,0]);
      Assert.AreEqual(a[1,1],b[1,1]);
    }

    //test multiply float matrix operator *
    [Test]
    public void OperatorMultiplyComplexFloatMatrix()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      a[0,0] = new ComplexFloat(1);
      a[0,1] = new ComplexFloat(2);
      a[1,0] = new ComplexFloat(3);
      a[1,1] = new ComplexFloat(4);
      ComplexFloatMatrix b = 2.0f * a;
      Assert.AreEqual(b[0,0],new ComplexFloat(2));
      Assert.AreEqual(b[0,1],new ComplexFloat(4));
      Assert.AreEqual(b[1,0],new ComplexFloat(6));
      Assert.AreEqual(b[1,1],new ComplexFloat(8));
    }
    
    //test multiply float null matrix operator * 
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void OperatorMultiplyComplexFloatMatrixNull()
    {
      ComplexFloatMatrix a = null;
      ComplexFloatMatrix b = 2.0f * a;
    } 

    //test multiply  matrix float operator *
    [Test]
    public void OperatorMultiplyMatrixComplexFloat()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      a[0,0] = new ComplexFloat(1);
      a[0,1] = new ComplexFloat(2);
      a[1,0] = new ComplexFloat(3);
      a[1,1] = new ComplexFloat(4);
      ComplexFloatMatrix b = a * 2.0f;
      Assert.AreEqual(b[0,0],new ComplexFloat(2));
      Assert.AreEqual(b[0,1],new ComplexFloat(4));
      Assert.AreEqual(b[1,0],new ComplexFloat(6));
      Assert.AreEqual(b[1,1],new ComplexFloat(8));
    }
    
    //test multiply  null matrix float operator * 
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void OperatorMultiplyMatrixComplexFloatNull()
    {
      ComplexFloatMatrix a = null;
      ComplexFloatMatrix b = a * 2;
    }
    
    //test static multiply float matrix 
    [Test]
    public void StaticMultiplyComplexFloatMatrix()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      a[0,0] = new ComplexFloat(1);
      a[0,1] = new ComplexFloat(2);
      a[1,0] = new ComplexFloat(3);
      a[1,1] = new ComplexFloat(4);
      ComplexFloatMatrix b = ComplexFloatMatrix.Multiply(2.0f,a);
      Assert.AreEqual(b[0,0],new ComplexFloat(2));
      Assert.AreEqual(b[0,1],new ComplexFloat(4));
      Assert.AreEqual(b[1,0],new ComplexFloat(6));
      Assert.AreEqual(b[1,1],new ComplexFloat(8));
    }
    
    //test static multiply float null matrix 
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void StaticMultiplyComplexFloatMatrixNull()
    {
      ComplexFloatMatrix a = null;
      ComplexFloatMatrix b = ComplexFloatMatrix.Multiply(2.0f,a);

    } 

    //test static multiply  matrix float
    [Test]
    public void StaticMultiplyMatrixComplexFloat()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      a[0,0] = new ComplexFloat(1);
      a[0,1] = new ComplexFloat(2);
      a[1,0] = new ComplexFloat(3);
      a[1,1] = new ComplexFloat(4);
      ComplexFloatMatrix b = ComplexFloatMatrix.Multiply(a,2.0f);

      Assert.AreEqual(b[0,0],new ComplexFloat(2));
      Assert.AreEqual(b[0,1],new ComplexFloat(4));
      Assert.AreEqual(b[1,0],new ComplexFloat(6));
      Assert.AreEqual(b[1,1],new ComplexFloat(8));
    }
    
    //test static multiply  null matrix float operator * 
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void StaticMultiplyMatrixComplexFloatNull()
    {
      ComplexFloatMatrix a = null;
      ComplexFloatMatrix b = ComplexFloatMatrix.Multiply(a,2.0f);

    }

    //test member multiply  float 
    [Test]
    public void MemberMultiplyComplexFloat()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      a[0,0] = new ComplexFloat(1);
      a[0,1] = new ComplexFloat(2);
      a[1,0] = new ComplexFloat(3);
      a[1,1] = new ComplexFloat(4);
      a.Multiply(2.0f);
      Assert.AreEqual(a[0,0],new ComplexFloat(2));
      Assert.AreEqual(a[0,1],new ComplexFloat(4));
      Assert.AreEqual(a[1,0],new ComplexFloat(6));
      Assert.AreEqual(a[1,1],new ComplexFloat(8));
    }

    //test multiply  matrix vector operator *
    [Test]
    public void OperatorMultiplyMatrixVector()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      a[0,0] = new ComplexFloat(1);
      a[0,1] = new ComplexFloat(2);
      a[1,0] = new ComplexFloat(3);
      a[1,1] = new ComplexFloat(4);
      ComplexFloatVector b = new ComplexFloatVector(2, 2.0f);
      ComplexFloatVector c = a * b;
      Assert.AreEqual(c[0],new ComplexFloat(6));
      Assert.AreEqual(c[1],new ComplexFloat(14));
    }
    
    //test multiply  matrix nonconform vector operator *
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void OperatorMultiplyMatrixNonConformVector()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      ComplexFloatVector b = new ComplexFloatVector(3, 2.0f);
      ComplexFloatVector c = a * b;
    }

    //test multiply null matrix vector operator *
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void OperatorMultiplyNullMatrixVector()
    {
      ComplexFloatMatrix a = null;
      ComplexFloatVector b = new ComplexFloatVector(2, 2.0f);
      ComplexFloatVector c = a * b;
    }

    //test multiply matrix null vector operator *
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void OperatorMultiplyMatrixNullVector()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      ComplexFloatVector b = null;
      ComplexFloatVector c = a * b;
    }
    
    //test static multiply  matrix vector
    [Test]
    public void StaticMultiplyMatrixVector()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      a[0,0] = new ComplexFloat(1);
      a[0,1] = new ComplexFloat(2);
      a[1,0] = new ComplexFloat(3);
      a[1,1] = new ComplexFloat(4);
      ComplexFloatVector b = new ComplexFloatVector(2, 2.0f);
      ComplexFloatVector c = ComplexFloatMatrix.Multiply(a,b);
      Assert.AreEqual(c[0],new ComplexFloat(6));
      Assert.AreEqual(c[1],new ComplexFloat(14));
    }

    //test static multiply  matrix nonconform vector
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void StaticMultiplyMatrixNonConformVector()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      ComplexFloatVector b = new ComplexFloatVector(3, 2.0f);
      ComplexFloatVector c = a * b;
    }

    //test static multiply null matrix vector
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void StaticMultiplyNullMatrixVector()
    {
      ComplexFloatMatrix a = null;
      ComplexFloatVector b = new ComplexFloatVector(2, 2.0f);
      ComplexFloatVector c = ComplexFloatMatrix.Multiply(a,b);
    }

    //test static multiply matrix null vector
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void StaticMultiplyMatrixNullVector()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      ComplexFloatVector b = null;
      ComplexFloatVector c = ComplexFloatMatrix.Multiply(a,b);
    }

    //test member multiply vector
    [Test]
    public void MemberMultiplyVector()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      a[0,0] = new ComplexFloat(1);
      a[0,1] = new ComplexFloat(2);
      a[1,0] = new ComplexFloat(3);
      a[1,1] = new ComplexFloat(4);
      ComplexFloatVector b = new ComplexFloatVector(2, 2.0f);
      a.Multiply(b);
      Assert.AreEqual(a[0,0],new ComplexFloat(6));
      Assert.AreEqual(a[1,0],new ComplexFloat(14));
      Assert.AreEqual(a.ColumnLength, 1);
      Assert.AreEqual(a.RowLength, 2);
    }
    
    //test member multiply  matrix nonconform vector
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void MemberMultiplyMatrixNonConformVector()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      ComplexFloatVector b = new ComplexFloatVector(3, 2.0f);
      a.Multiply(b);
    }
    //test member multiply null vector
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void MemberMultiplyNullVector()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      ComplexFloatVector b = null;
      a.Multiply(b);
    }

    //test multiply  matrix matrix operator *
    [Test]
    public void OperatorMultiplyMatrixMatrix()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      a[0,0] = new ComplexFloat(1);
      a[0,1] = new ComplexFloat(2);
      a[1,0] = new ComplexFloat(3);
      a[1,1] = new ComplexFloat(4);
      ComplexFloatMatrix b = new ComplexFloatMatrix(2, 2.0f);
      ComplexFloatMatrix c = a * b;
      Assert.AreEqual(c[0,0],new ComplexFloat(6));
      Assert.AreEqual(c[0,1],new ComplexFloat(6));
      Assert.AreEqual(c[1,0],new ComplexFloat(14));
      Assert.AreEqual(c[1,1],new ComplexFloat(14));
    }

    //test multiply  nonconform matrix matrix operator *
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void OperatorMultiplyMatrixNonConformMatrix()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      ComplexFloatMatrix b = new ComplexFloatMatrix(3, 2, 2.0f);
      ComplexFloatMatrix c = a * b;
    }

    //test multiply  long matrix wide matrix operator *
    [Test]
    public void OperatorMultiplyLongMatrixWideMatrix()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(3,2,1);
      ComplexFloatMatrix b = new ComplexFloatMatrix(2,3,2);
      ComplexFloatMatrix c = a * b;
      Assert.AreEqual(c[0,0],new ComplexFloat(4));
      Assert.AreEqual(c[0,1],new ComplexFloat(4));
      Assert.AreEqual(c[0,1],new ComplexFloat(4));
      Assert.AreEqual(c[1,0],new ComplexFloat(4));
      Assert.AreEqual(c[1,1],new ComplexFloat(4));
      Assert.AreEqual(c[1,2],new ComplexFloat(4));
      Assert.AreEqual(c[2,0],new ComplexFloat(4));
      Assert.AreEqual(c[2,1],new ComplexFloat(4));
      Assert.AreEqual(c[2,2],new ComplexFloat(4));
    }

    //test multiply  wide matrix long matrix operator *
    [Test]
    public void OperatorMultiplyWideMatrixLongMatrix()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2,3,1);
      ComplexFloatMatrix b = new ComplexFloatMatrix(3,2,2);
      ComplexFloatMatrix c = a * b;
      Assert.AreEqual(c[0,0],new ComplexFloat(6));
      Assert.AreEqual(c[0,1],new ComplexFloat(6));
      Assert.AreEqual(c[1,0],new ComplexFloat(6));
      Assert.AreEqual(c[1,1],new ComplexFloat(6));
    }

    //test multiply null matrix matrix operator *
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void OperatorMultiplyNullMatrixMatrix()
    {
      ComplexFloatMatrix a = null;
      ComplexFloatMatrix b = new ComplexFloatMatrix(2, 2.0f);
      ComplexFloatMatrix c = a * b;
    }

    //test multiply matrix null matrix operator *
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void OperatorMultiplyMatrixNullMatrix()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2, 2.0f);
      ComplexFloatMatrix b = null;
      ComplexFloatMatrix c = a * b;
    }

    //test static multiply  matrix matrix
    [Test]
    public void StaticMultiplyMatrixMatrix()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      a[0,0] = new ComplexFloat(1);
      a[0,1] = new ComplexFloat(2);
      a[1,0] = new ComplexFloat(3);
      a[1,1] = new ComplexFloat(4);
      ComplexFloatMatrix b = new ComplexFloatMatrix(2, 2, 2.0f);
      ComplexFloatMatrix c = ComplexFloatMatrix.Multiply(a,b);
      Assert.AreEqual(c[0,0],new ComplexFloat(6));
      Assert.AreEqual(c[0,1],new ComplexFloat(6));
      Assert.AreEqual(c[1,0],new ComplexFloat(14));
      Assert.AreEqual(c[1,1],new ComplexFloat(14));
    }

    //test static multiply nonconform matrix matrix
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void StaticMultiplyMatrixNonConformMatrix()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      ComplexFloatMatrix b = new ComplexFloatMatrix(3, 2, 2.0f);
      ComplexFloatMatrix c = ComplexFloatMatrix.Multiply(a,b);
    }

    //test static multiply  long matrix wide matrix
    [Test]
    public void StaticMultiplyLongMatrixWideMatrix()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(3,2,1);
      ComplexFloatMatrix b = new ComplexFloatMatrix(2,3,2);
      ComplexFloatMatrix c = ComplexFloatMatrix.Multiply(a,b);
      Assert.AreEqual(c[0,0],new ComplexFloat(4));
      Assert.AreEqual(c[0,1],new ComplexFloat(4));
      Assert.AreEqual(c[0,1],new ComplexFloat(4));
      Assert.AreEqual(c[1,0],new ComplexFloat(4));
      Assert.AreEqual(c[1,1],new ComplexFloat(4));
      Assert.AreEqual(c[1,2],new ComplexFloat(4));
      Assert.AreEqual(c[2,0],new ComplexFloat(4));
      Assert.AreEqual(c[2,1],new ComplexFloat(4));
      Assert.AreEqual(c[2,2],new ComplexFloat(4));
    }

    //test static multiply  wide matrix long matrix
    [Test]
    public void StaticMultiplyWideMatrixLongMatrix()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2,3,1);
      ComplexFloatMatrix b = new ComplexFloatMatrix(3,2,2);
      ComplexFloatMatrix c = ComplexFloatMatrix.Multiply(a,b);
      Assert.AreEqual(c[0,0],new ComplexFloat(6));
      Assert.AreEqual(c[0,1],new ComplexFloat(6));
      Assert.AreEqual(c[1,0],new ComplexFloat(6));
      Assert.AreEqual(c[1,1],new ComplexFloat(6));
    }

    //test static multiply null matrix matrix
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void StaticMultiplyNullMatrixMatrix()
    {
      ComplexFloatMatrix a = null;
      ComplexFloatMatrix b = new ComplexFloatMatrix(2, 2.0f);
      ComplexFloatMatrix c = ComplexFloatMatrix.Multiply(a,b);
    }

    //test static multiply matrix null matrix 
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void StaticMultiplyMatrixNullMatrix()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2, 2.0f);
      ComplexFloatMatrix b = null;
      ComplexFloatMatrix c = ComplexFloatMatrix.Multiply(a,b);
    }

    //test member multiply  matrix matrix
    [Test]
    public void MemberMultiplyMatrixMatrix()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      a[0,0] = new ComplexFloat(1);
      a[0,1] = new ComplexFloat(2);
      a[1,0] = new ComplexFloat(3);
      a[1,1] = new ComplexFloat(4);
      ComplexFloatMatrix b = new ComplexFloatMatrix(2, 2, 2.0f);
      a.Multiply(b);
      Assert.AreEqual(a[0,0],new ComplexFloat(6));
      Assert.AreEqual(a[0,1],new ComplexFloat(6));
      Assert.AreEqual(a[1,0],new ComplexFloat(14));
      Assert.AreEqual(a[1,1],new ComplexFloat(14));
    }

    //test member multiply nonconform matrix matrix
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void MemberMultiplyMatrixNonConformMatrix()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      ComplexFloatMatrix b = new ComplexFloatMatrix(3, 2, 2.0f);
      a.Multiply(b);
    }

    //test member multiply  long matrix wide matrix
    [Test]
    public void MemberMultiplyLongMatrixWideMatrix()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(3,2,1);
      ComplexFloatMatrix b = new ComplexFloatMatrix(2,3,2);
      a.Multiply(b);
      Assert.AreEqual(a[0,0],new ComplexFloat(4));
      Assert.AreEqual(a[0,1],new ComplexFloat(4));
      Assert.AreEqual(a[0,1],new ComplexFloat(4));
      Assert.AreEqual(a[1,0],new ComplexFloat(4));
      Assert.AreEqual(a[1,1],new ComplexFloat(4));
      Assert.AreEqual(a[1,2],new ComplexFloat(4));
      Assert.AreEqual(a[2,0],new ComplexFloat(4));
      Assert.AreEqual(a[2,1],new ComplexFloat(4));
      Assert.AreEqual(a[2,2],new ComplexFloat(4));
    }

    //test member multiply  wide matrix long matrix
    [Test]
    public void MemberMultiplyWideMatrixLongMatrix()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2,3,1);
      ComplexFloatMatrix b = new ComplexFloatMatrix(3,2,2);
      a.Multiply(b);
      Assert.AreEqual(a[0,0],new ComplexFloat(6));
      Assert.AreEqual(a[0,1],new ComplexFloat(6));
      Assert.AreEqual(a[1,0],new ComplexFloat(6));
      Assert.AreEqual(a[1,1],new ComplexFloat(6));
    }

    //test member multiply null matrix matrix
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void MemberMultiplyNullMatrixMatrix()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2, 2.0f);
      ComplexFloatMatrix b = null;
      a.Multiply(b);
    }

  
    //copy null
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CopyNull()
    {
      ComplexFloatMatrix a =  null;
      ComplexFloatMatrix b = new ComplexFloatMatrix(2);
      b.Copy(a);
    }

    //Norm
    [Test]
    public void Norms()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      a[0,0] = new ComplexFloat(1.1f, 1.1f);
      a[0,1] = new ComplexFloat(2.2f, -2.2f);
      a[1,0] = new ComplexFloat(3.3f, 3.3f);
      a[1,1] = new ComplexFloat(4.4f, -4.4f);
      Assert.AreEqual(a.GetL1Norm(),9.334,TOLERENCE);
      Assert.AreEqual(a.GetL2Norm(),8.502,TOLERENCE);
      Assert.AreEqual(a.GetInfinityNorm(),10.889,TOLERENCE);
      Assert.AreEqual(a.GetFrobeniusNorm(),8.521,TOLERENCE);
    }

    //Wide Norm
    [Test]
    public void WideNorms()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2,3);
      a[0,0] = new ComplexFloat(1.1f, 1.1f);
      a[0,1] = new ComplexFloat(2.2f, -2.2f);
      a[0,2] = new ComplexFloat(3.3f, 3.3f);
      a[1,0] = new ComplexFloat(4.4f, -4.4f);
      a[1,1] = new ComplexFloat(5.5f, 5.5f);
      a[1,2] = new ComplexFloat(6.6f, -6.6f);
      Assert.AreEqual(a.GetL1Norm(),14.001,TOLERENCE);
      Assert.AreEqual(a.GetL2Norm(),13.845,TOLERENCE);
      Assert.AreEqual(a.GetInfinityNorm(),23.335,TOLERENCE);
      Assert.AreEqual(a.GetFrobeniusNorm(),14.840,TOLERENCE);
    }

    //Long Norm
    [Test]
    public void LongNorms()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(3,2);
      a[0,0] = new ComplexFloat(1.1f, 1.1f);
      a[0,1] = new ComplexFloat(2.2f, -2.2f);
      a[1,0] = new ComplexFloat(3.3f, 3.3f);
      a[1,1] = new ComplexFloat(4.4f, -4.4f);
      a[2,0] = new ComplexFloat(5.5f, 5.5f);
      a[2,1] = new ComplexFloat(6.6f, -6.6f);
      Assert.AreEqual(a.GetL1Norm(),18.668,TOLERENCE);
      Assert.AreEqual(a.GetL2Norm(),14.818,TOLERENCE);
      Assert.AreEqual(a.GetInfinityNorm(),17.112,TOLERENCE);
      Assert.AreEqual(a.GetFrobeniusNorm(),14.840,TOLERENCE);
    } 

    //Condition
    [Test]
    public void Condition()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      a[0,0] = new ComplexFloat(1.1f, 1.1f);
      a[0,1] = new ComplexFloat(2.2f, -2.2f);
      a[1,0] = new ComplexFloat(3.3f, 3.3f);
      a[1,1] = new ComplexFloat(4.4f, -4.4f);
      Assert.AreEqual(a.GetConditionNumber(),14.933,TOLERENCE);
    }

    //Wide Condition
    [Test]
    [ExpectedException(typeof(NotSquareMatrixException))]
    public void WideCondition()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2,3);
      a.GetConditionNumber();
    }

    //Long Condition
    [Test]
    [ExpectedException(typeof(NotSquareMatrixException))]
    public void LongCondition()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(3,2);
      a.GetConditionNumber();
    } 

    //clone
    [Test]
    public void Clone()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(2);
      a[0,0] = new ComplexFloat(1);
      a[0,1] = new ComplexFloat(2);
      a[1,0] = new ComplexFloat(3);
      a[1,1] = new ComplexFloat(4);
      ComplexFloatMatrix b = (ComplexFloatMatrix)a.Clone();
      Assert.AreEqual(a[0,0],a[0,0]);
      Assert.AreEqual(a[0,1],b[0,1]);
      Assert.AreEqual(a[1,0],b[1,0]);
      Assert.AreEqual(a[1,1],b[1,1]);
    }
  }
}
