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
  public class ComplexDoubleMatrixTest
  {
    private const double TOLERENCE = 0.001;

    //Test dimensions Constructor.
    [Test]
    public void CtorDimensions()
    {
      ComplexDoubleMatrix test = new ComplexDoubleMatrix(2,2);
      
      Assert.AreEqual(test.RowLength, 2);
      Assert.AreEqual(test.ColumnLength, 2);
      Assert.AreEqual(test[0,0], Complex.Zero);
      Assert.AreEqual(test[0,1], Complex.Zero);
      Assert.AreEqual(test[1,0], Complex.Zero);
      Assert.AreEqual(test[1,1], Complex.Zero);
    }

    //Test Intital Values Constructor.
    [Test]
    public void CtorInitialValues()
    {
      ComplexDoubleMatrix test = new ComplexDoubleMatrix(2,2,new Complex(1,1));
      
      Assert.AreEqual(test.RowLength, 2);
      Assert.AreEqual(test.ColumnLength, 2);
      Complex value = new Complex(1,1);
      Assert.AreEqual(test[0,0], value);
      Assert.AreEqual(test[0,1], value);
      Assert.AreEqual(test[1,0], value);
      Assert.AreEqual(test[1,1], value);
    }
    
    //Test Copy Constructor.
    [Test]
    public void CtorCopy()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2,2);
      a[0,0] = new Complex(1,1);      
      a[0,1] = new Complex(2,2);      
      a[1,0] = new Complex(3,3);      
      a[1,1] = new Complex(4,4);  
      
      ComplexDoubleMatrix b = new ComplexDoubleMatrix(a);
      
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
      ComplexDoubleMatrix a = null;
      ComplexDoubleMatrix b = new ComplexDoubleMatrix(a);
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
      
      ComplexDoubleMatrix b = new ComplexDoubleMatrix(a);
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
      ComplexDoubleMatrix b = new ComplexDoubleMatrix(a);
      
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
      
      ComplexDoubleMatrix b = new ComplexDoubleMatrix(a);
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
      ComplexDoubleMatrix b = new ComplexDoubleMatrix(a);
      
    }

    //Test Copy Constructor.
    [Test]
    public void CtorCopyDouble()
    {
      DoubleMatrix a = new DoubleMatrix(2,2);
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;

      ComplexDoubleMatrix b = new ComplexDoubleMatrix(a);
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
      ComplexDoubleMatrix b = new ComplexDoubleMatrix(a);

    }
 
    //Test Multiple Dimensional ComplexDoubleArray Constructor with Square array.
    [Test]
    public void CtorMultDimComplexDoubleSquare()
    {
      Complex[,] values= new Complex[2,2];
      
      values[0,0] = new Complex(1,1);
      values[0,1] = new Complex(2,2);
      values[1,0] = new Complex(3,3);
      values[1,1] = new Complex(4,4);
      
      ComplexDoubleMatrix test = new ComplexDoubleMatrix(values);
      
      Assert.AreEqual(test.RowLength, 2);
      Assert.AreEqual(test.ColumnLength, 2);
      Assert.AreEqual(test[0,0], values[0,0]);
      Assert.AreEqual(test[0,1], values[0,1]);
      Assert.AreEqual(test[1,0], values[1,0]);
      Assert.AreEqual(test[1,1], values[1,1]);
    }
    
    //Test Multiple Dimensional ComplexDoubleArray Constructor with wide array.
    [Test]
    public void CtorMultDimComplexDoubleWide()
    {
      Complex[,] values= new Complex[2,3];
      
      values[0,0] = new Complex(0,0);
      values[0,1] = new Complex(1,1);
      values[0,2] = new Complex(2,2);
      values[1,0] = new Complex(3,3);
      values[1,1] = new Complex(4,4);
      values[1,2] = new Complex(5,5);
      
      ComplexDoubleMatrix test = new ComplexDoubleMatrix(values);
      
      Assert.AreEqual(test.RowLength, 2);
      Assert.AreEqual(test.ColumnLength, 3);
      Assert.AreEqual(test[0,0], values[0,0]);
      Assert.AreEqual(test[0,1], values[0,1]);
      Assert.AreEqual(test[0,2], values[0,2]);
      Assert.AreEqual(test[1,0], values[1,0]);
      Assert.AreEqual(test[1,1], values[1,1]);
      Assert.AreEqual(test[1,2], values[1,2]);
    }
    
    //Test Multiple Dimensional ComplexDoubleArray Constructor with long array.
    [Test]
    public void CtorMultDimComplexDoubleLong()
    {
      Complex[,] values= new Complex[3,2];
      
      values[0,0] = new Complex(0,0);
      values[0,1] = new Complex(1,1);
      values[1,0] = new Complex(3,3);
      values[1,1] = new Complex(4,4);
      values[2,0] = new Complex(5,5);
      values[2,1] = new Complex(6,6);
      
      ComplexDoubleMatrix test = new ComplexDoubleMatrix(values);
      
      Assert.AreEqual(test.RowLength, 3);
      Assert.AreEqual(test.ColumnLength, 2);
      Assert.AreEqual(test[0,0], values[0,0]);
      Assert.AreEqual(test[0,1], values[0,1]);
      Assert.AreEqual(test[1,0], values[1,0]);
      Assert.AreEqual(test[1,1], values[1,1]);
      Assert.AreEqual(test[2,0], values[2,0]);
      Assert.AreEqual(test[2,1], values[2,1]);
    }
    
    //Test Multiple Dimensional Complex Array Constructor with null.
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CtorMultDimComplexDoubleNull()
    {
      double[,] values = null;
      ComplexDoubleMatrix test = new ComplexDoubleMatrix(values);
    }

    //Test Multiple Dimensional ComplexFloat Array Constructor with Square array.
    [Test]
    public void CtorMultDimComplexFloatSquare()
    {
      ComplexFloat[,] values= new ComplexFloat[2,2];
      
      values[0,0] = new ComplexFloat(0,0);
      values[0,1] = new ComplexFloat(1,1);
      values[1,0] = new ComplexFloat(2,2);
      values[1,1] = new ComplexFloat(3,3);
      
      ComplexDoubleMatrix test = new ComplexDoubleMatrix(values);
      
      Assert.AreEqual(test.RowLength, 2);
      Assert.AreEqual(test.ColumnLength, 2);
      Assert.AreEqual(test[0,0], values[0,0]);
      Assert.AreEqual(test[0,1], values[0,1]);
      Assert.AreEqual(test[1,0], values[1,0]);
      Assert.AreEqual(test[1,1], values[1,1]);
    }
    
    //Test Multiple Dimensional ComplexFloat Array Constructor with wide array.
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
      
      ComplexDoubleMatrix test = new ComplexDoubleMatrix(values);
      
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
      values[1,0] = new ComplexFloat(2,2);
      values[1,1] = new ComplexFloat(3,3);
      values[2,0] = new ComplexFloat(4,4);
      values[2,1] = new ComplexFloat(5,5);
      
      ComplexDoubleMatrix test = new ComplexDoubleMatrix(values);
      
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
      ComplexDoubleMatrix test = new ComplexDoubleMatrix(values);
    }
    
    //Test Multiple Dimensional DoubleArray Constructor with Square array.
    [Test]
    public void CtorMultDimDoubleSquare()
    {
      double[,] values= new double[2,2];

      values[0,0] = 1;
      values[0,1] = 2;
      values[1,0] = 3;
      values[1,1] = 4;

      ComplexDoubleMatrix test = new ComplexDoubleMatrix(values);

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
      double[,] values= new double[2,3];

      values[0,0] = 0;
      values[0,1] = 1;
      values[0,2] = 2;
      values[1,0] = 3;
      values[1,1] = 4;
      values[1,2] = 5;

      ComplexDoubleMatrix test = new ComplexDoubleMatrix(values);

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
      double[,] values= new double[3,2];

      values[0,0] = 0;
      values[0,1] = 1;
      values[1,0] = 2;
      values[1,1] = 3;
      values[2,0] = 4;
      values[2,1] = 5;

      ComplexDoubleMatrix test = new ComplexDoubleMatrix(values);

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
      double[,] values = null;
      ComplexDoubleMatrix test = new ComplexDoubleMatrix(values);
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

      ComplexDoubleMatrix test = new ComplexDoubleMatrix(values);

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

      ComplexDoubleMatrix test = new ComplexDoubleMatrix(values);

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

      ComplexDoubleMatrix test = new ComplexDoubleMatrix(values);

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
      ComplexDoubleMatrix test = new ComplexDoubleMatrix(values);
    }
  
    //Test Jagged Array  Constructor with null.
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CtorJaggedNull()
    {
      double[,] values = null;
      ComplexDoubleMatrix test = new ComplexDoubleMatrix(values);
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

      ComplexDoubleMatrix b = a;
      Assert.AreEqual(a.RowLength, b.RowLength);
      Assert.AreEqual(a.ColumnLength, b.ColumnLength);
      Assert.AreEqual(b[0,0], a[0,0]);
      Assert.AreEqual(b[0,1], a[0,1]);
      Assert.AreEqual(b[1,0], a[1,0]);
      Assert.AreEqual(b[1,1], a[1,1]);
    }

    //Test implicit conversion from null Complexfloatmatrix.
    [Test]
    public void ImplictComplexFloatMatrixNull()
    {
      ComplexFloatMatrix a = null;
      ComplexDoubleMatrix b = a;
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

      ComplexDoubleMatrix b = ComplexDoubleMatrix.ToComplexDoubleMatrix(a);
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
      ComplexDoubleMatrix b = ComplexDoubleMatrix.ToComplexDoubleMatrix(a);
      Assert.IsTrue(b == null);
    }

    //Test implicit conversion from Doublematrix.
    [Test]
    public void ImplictDoubleMatrix()
    {
      DoubleMatrix a = new DoubleMatrix(2,2);
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;

      ComplexDoubleMatrix b = a;
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
      DoubleMatrix a = null;
      ComplexDoubleMatrix b = a;
      Assert.IsTrue(b == null);
    }

    //Test implicit conversion from floatmatrix.
    [Test]
    public void ImplictToDoubleMatrix()
    {
      DoubleMatrix a = new DoubleMatrix(2,2);
      a[0,0] = 1;
      a[0,1] = 2;
      a[1,0] = 3;
      a[1,1] = 4;

      ComplexDoubleMatrix b = ComplexDoubleMatrix.ToComplexDoubleMatrix(a);
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
      DoubleMatrix a = null;
      ComplexDoubleMatrix b = ComplexDoubleMatrix.ToComplexDoubleMatrix(a);
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
      
      ComplexDoubleMatrix b = a;
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
      ComplexDoubleMatrix b = a;
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
      
      ComplexDoubleMatrix b = ComplexDoubleMatrix.ToComplexDoubleMatrix(a);
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
      ComplexDoubleMatrix b = ComplexDoubleMatrix.ToComplexDoubleMatrix(a);
      Assert.IsTrue(b == null);
    }

    //Test implicit conversion from Complex mult dim array.
    [Test]
    public void ImplictComplexDoubleMultArray()
    {
      Complex[,] a = new Complex[2,2];
      a[0,0] = new Complex(1,1);
      a[0,1] = new Complex(2,2);
      a[1,0] = new Complex(3,3);
      a[1,1] = new Complex(4,4);
      
      ComplexDoubleMatrix b = a;
      Assert.AreEqual(a[0,0], b[0,0]);
      Assert.AreEqual(a[0,1], b[0,1]);
      Assert.AreEqual(a[1,0], b[1,0]);
      Assert.AreEqual(a[1,1], b[1,1]);
    }
    
    //Test implicit conversion from null Complex mult dim array.
    [Test]
    public void ImplictComplexDoubleMultArrayNull()
    {
      double[,] a = null;
      ComplexDoubleMatrix b = a;
      Assert.IsTrue(b == null);
    }
    
    //Test implicit conversion from Complex mult dim array.
    [Test]
    public void ImplictToComplexDoubleMultArray()
    {
      Complex[,] a = new Complex[2,2];
      a[0,0] = new Complex(1,1);
      a[0,1] = new Complex(2,2);
      a[1,0] = new Complex(3,3);
      a[1,1] = new Complex(4,4);
      
      ComplexDoubleMatrix b = ComplexDoubleMatrix.ToComplexDoubleMatrix(a);
      Assert.AreEqual(a[0,0], b[0,0]);
      Assert.AreEqual(a[0,1], b[0,1]);
      Assert.AreEqual(a[1,0], b[1,0]);
      Assert.AreEqual(a[1,1], b[1,1]);
    }
    
    //Test implicit conversion from null Complex mult dim array.
    [Test]
    public void ImplictToComplexDoubleMultArrayNull()
    {
      Complex[,] a = null;
      ComplexDoubleMatrix b = ComplexDoubleMatrix.ToComplexDoubleMatrix(a);
      Assert.IsTrue(b == null);
    }


    //Test implicit conversion from ComplexFloat mult dim array.
    [Test]
    public void ImplictComplexFloatMultArray()
    {
      ComplexFloat [,] a = new ComplexFloat [2,2];
      a[0,0] = new ComplexFloat(1,1);
      a[0,1] = new ComplexFloat(2,2);
      a[1,0] = new ComplexFloat(3,3);
      a[1,1] = new ComplexFloat(4,4);
      
      ComplexDoubleMatrix b = a;
      Assert.AreEqual(a[0,0], b[0,0]);
      Assert.AreEqual(a[0,1], b[0,1]);
      Assert.AreEqual(a[1,0], b[1,0]);
      Assert.AreEqual(a[1,1], b[1,1]);
    }
    
    //Test implicit conversion from null ComplexFloat  mult dim array.
    [Test]
    public void ImplictComplexFloatMultArrayNull()
    {
      ComplexFloat [,] a = null;
      ComplexDoubleMatrix b = a;
      Assert.IsTrue(b == null);
    }
    
    //Test implicit conversion from ComplexFloat  mult dim array.
    [Test]
    public void ImplictToComplexFloatMultArray()
    {
      ComplexFloat [,] a = new ComplexFloat [2,2];
      a[0,0] = new ComplexFloat(1,1);
      a[0,1] = new ComplexFloat(2,2);
      a[1,0] = new ComplexFloat(3,3);
      a[1,1] = new ComplexFloat(4,4);
      
      ComplexDoubleMatrix b = ComplexDoubleMatrix.ToComplexDoubleMatrix(a);
      Assert.AreEqual(a[0,0], b[0,0]);
      Assert.AreEqual(a[0,1], b[0,1]);
      Assert.AreEqual(a[1,0], b[1,0]);
      Assert.AreEqual(a[1,1], b[1,1]);
    }
    
    //Test implicit conversion from null ComplexFloat  mult dim array.
    [Test]
    public void ImplictToComplexFloatMultArrayNull()
    {
      ComplexFloat [,] a = null;
      ComplexDoubleMatrix b = ComplexDoubleMatrix.ToComplexDoubleMatrix(a);
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

      ComplexDoubleMatrix b = a;
      Assert.AreEqual(a[0,0], b[0,0].Real);
      Assert.AreEqual(a[0,1], b[0,1].Real);
      Assert.AreEqual(a[1,0], b[1,0].Real);
      Assert.AreEqual(a[1,1], b[1,1].Real);
    }

    //Test implicit conversion from null double mult dim array.
    [Test]
    public void ImplictDoubleMultArrayNull()
    {
      double[,] a = null;
      ComplexDoubleMatrix b = a;
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

      ComplexDoubleMatrix b = ComplexDoubleMatrix.ToComplexDoubleMatrix(a);
      Assert.AreEqual(a[0,0], b[0,0].Real);
      Assert.AreEqual(a[0,1], b[0,1].Real);
      Assert.AreEqual(a[1,0], b[1,0].Real);
      Assert.AreEqual(a[1,1], b[1,1].Real);
    }

    //Test implicit conversion from null double mult dim array.
    [Test]
    public void ImplictToDoubleMultArrayNull()
    {
      double[,] a = null;
      ComplexDoubleMatrix b = ComplexDoubleMatrix.ToComplexDoubleMatrix(a);
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

      ComplexDoubleMatrix b = a;
      Assert.AreEqual(a[0,0], b[0,0].Real);
      Assert.AreEqual(a[0,1], b[0,1].Real);
      Assert.AreEqual(a[1,0], b[1,0].Real);
      Assert.AreEqual(a[1,1], b[1,1].Real);
    }

    //Test implicit conversion from null float mult dim array.
    [Test]
    public void ImplictFloatMultArrayNull()
    {
      float[,] a = null;
      ComplexDoubleMatrix b = a;
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

      ComplexDoubleMatrix b = ComplexDoubleMatrix.ToComplexDoubleMatrix(a);
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
      ComplexDoubleMatrix b = ComplexDoubleMatrix.ToComplexDoubleMatrix(a);
      Assert.IsTrue(b == null);
    }
    //test equals method
    [Test]
    public void Equals()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2,2,new Complex(4,4));
      ComplexDoubleMatrix b = new ComplexDoubleMatrix(2,2,new Complex(4,4));
      ComplexDoubleMatrix c = new ComplexDoubleMatrix(2,2);
      c[0,0] = new Complex(4,4);
      c[0,1] = new Complex(4,4);
      c[1,0] = new Complex(4,4);
      c[1,1] = new Complex(4,4);

      ComplexDoubleMatrix d = new ComplexDoubleMatrix(2,2,5);
      ComplexDoubleMatrix e = null;
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
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      a[0,0] = new Complex(2,2);
      a[0,1] = new Complex(2,2);
      a[1,0] = new Complex(2,2);
      a[1,1] = new Complex(2,2);

      int hash = a.GetHashCode();
      Assert.AreEqual(hash, 5);
    }
    
    //test ToArray
    [Test]
    public void ToArray()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      a[0,0] = new Complex(1,1);
      a[0,1] = new Complex(2,2);
      a[1,0] = new Complex(3,3);
      a[1,1] = new Complex(4,4);

      Complex[,] b = a.ToArray();

      Assert.AreEqual(a[0,0], b[0,0]);
      Assert.AreEqual(a[0,1], b[0,1]);
      Assert.AreEqual(a[1,0], b[1,0]);
      Assert.AreEqual(a[1,1], b[1,1]);
    }
    
    //test Transpose square
    [Test]
    public void TransposeSquare()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2,2);
      a[0,0] = new Complex(1);
      a[0,1] = new Complex(2);
      a[1,0] = new Complex(3);
      a[1,1] = new Complex(4);
      a.Transpose();
      Assert.AreEqual(a[0,0], new Complex(1));
      Assert.AreEqual(a[0,1], new Complex(3));
      Assert.AreEqual(a[1,0], new Complex(2));
      Assert.AreEqual(a[1,1], new Complex(4));
    }
    
    //test Transpose wide
    [Test]
    public void TransposeWide()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2,3);
      a[0,0] = new Complex(1);
      a[0,1] = new Complex(2);
      a[0,2] = new Complex(3);
      a[1,0] = new Complex(4);
      a[1,1] = new Complex(5);
      a[1,2] = new Complex(6);
      a.Transpose();
      Assert.AreEqual(a[0,0], new Complex(1));
      Assert.AreEqual(a[0,1], new Complex(4));
      Assert.AreEqual(a[1,0], new Complex(2));
      Assert.AreEqual(a[1,1], new Complex(5));
      Assert.AreEqual(a[2,0], new Complex(3));
      Assert.AreEqual(a[2,1], new Complex(6));
      Assert.AreEqual(a.RowLength, 3);
      Assert.AreEqual(a.ColumnLength, 2);
    }

    //test Transpose long
    [Test]
    public void TransposeLong()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(3,2);
      a[0,0] = new Complex(1);
      a[0,1] = new Complex(2);
      a[1,0] = new Complex(3);
      a[1,1] = new Complex(4);
      a[2,0] = new Complex(5);
      a[2,1] = new Complex(6);
      a.Transpose();
      Assert.AreEqual(a[0,0], new Complex(1));
      Assert.AreEqual(a[0,1], new Complex(3));
      Assert.AreEqual(a[0,2], new Complex(5));
      Assert.AreEqual(a[1,0], new Complex(2));
      Assert.AreEqual(a[1,1], new Complex(4));
      Assert.AreEqual(a[1,2], new Complex(6));
      Assert.AreEqual(a.RowLength, 2);
      Assert.AreEqual(a.ColumnLength, 3);
    }
    
    //test GetTranspose square
    [Test]
    public void GetTransposeSquare()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2,2);
      a[0,0] = new Complex(1);
      a[0,1] = new Complex(2);
      a[1,0] = new Complex(3);
      a[1,1] = new Complex(4);
      ComplexDoubleMatrix b = a.GetTranspose();
      Assert.AreEqual(b[0,0], a[0,0]);
      Assert.AreEqual(b[0,1], a[1,0]);
      Assert.AreEqual(b[1,0], a[0,1]);
      Assert.AreEqual(b[1,1], a[1,1]);
    }
    
    //test GetTranspose wide
    [Test]
    public void GetTransposeWide()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2,3);
      a[0,0] = new Complex(1);
      a[0,1] = new Complex(2);
      a[0,2] = new Complex(3);
      a[1,0] = new Complex(4);
      a[1,1] = new Complex(5);
      a[1,2] = new Complex(6);
      ComplexDoubleMatrix b = a.GetTranspose();
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
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(3,2);
      a[0,0] = new Complex(1);
      a[0,1] = new Complex(2);
      a[1,0] = new Complex(3);
      a[1,1] = new Complex(4);
      a[2,0] = new Complex(5);
      a[2,1] = new Complex(6);
      ComplexDoubleMatrix b = a.GetTranspose();
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
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2,2);
      a[0,0] = new Complex(2);
      a[0,1] = new Complex(4);
      a[1,0] = new Complex(3);
      a[1,1] = new Complex(7);
      a.Invert();
      Assert.AreEqual(a[0,0].Real, 3.5, 3.5E-15);
      Assert.AreEqual(a[0,1].Real, -2, 2E-15);
      Assert.AreEqual(a[1,0].Real, -1.5, 1.5E-15);
      Assert.AreEqual(a[1,1].Real, 1, 1E-15);
    }

    //test Invert singular
    [Test]
    [ExpectedException(typeof(SingularMatrixException))]
    public void InvertSingular()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2,2);
      a.Invert();
    }

    //test Invert not square
    [Test]
    [ExpectedException(typeof(NotSquareMatrixException))]
    public void InvertNotSquare()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(3,2);
      a[0,0] = new Complex(2);
      a[0,1] = new Complex(4);
      a[1,0] = new Complex(3);
      a[1,1] = new Complex(7);
      a[2,0] = new Complex(5);
      a[2,1] = new Complex(5);
      a.Invert();
    }

    //test GetInverse singular
    [Test]
    [ExpectedException(typeof(SingularMatrixException))]
    public void GetInverseSingular()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2,2);
      ComplexDoubleMatrix b = a.GetInverse();
    }

    //test GetInverse not square
    [Test]
    [ExpectedException(typeof(NotSquareMatrixException))]
    public void GetInverseNotSquare()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(3,2);
      a[0,0] = new Complex(2);
      a[0,1] = new Complex(4);
      a[1,0] = new Complex(3);
      a[1,1] = new Complex(7);
      a[2,0] = new Complex(5);
      a[2,1] = new Complex(5);
      ComplexDoubleMatrix b = a.GetInverse();
    }
    //test GetInverse
    [Test]
    public void GetInverse()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2,2);
      a[0,0] = new Complex(2);
      a[0,1] = new Complex(4);
      a[1,0] = new Complex(3);
      a[1,1] = new Complex(7);
      ComplexDoubleMatrix b = a.GetInverse();
      Assert.AreEqual(b[0,0].Real, 3.5, 3.5E-15);
      Assert.AreEqual(b[0,1].Real, -2, 2E-15);
      Assert.AreEqual(b[1,0].Real, -1.5, 1.5E-15);
      Assert.AreEqual(b[1,1].Real, 1, 1E-15);
    }   

    //test GetDeterminant
    [Test]
    public void GetDeterminant()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2,2);
      a[0,0] = new Complex(2);
      a[0,1] = new Complex(4);
      a[1,0] = new Complex(3);
      a[1,1] = new Complex(7);
      Complex b = a.GetDeterminant();
      Complex test = new Complex(2);
      Assert.AreEqual(b.Real, test.Real, 2E-15);
      Assert.AreEqual(b.Imag, test.Imag, 2E-15);
    }

    //test GetDeterminant
    [Test]
    [ExpectedException(typeof(NotSquareMatrixException))]
    public void GetDeterminantNotSquare()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(3,2); 
      a[0,0] = new Complex(2);
      a[0,1] = new Complex(4);
      a[1,0] = new Complex(3);
      a[1,1] = new Complex(7);
      a[2,0] = new Complex(5);
      a[2,1] = new Complex(5);
      Complex b = a.GetDeterminant();
    }   

    //test GetRow
    [Test]
    public void GetRow()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2,2);
      a[0,0] = new Complex(1);
      a[0,1] = new Complex(2);
      a[1,0] = new Complex(3);
      a[1,1] = new Complex(4);
      ComplexDoubleVector b = a.GetRow(0);
      Assert.AreEqual(b[0], a[0,0]);
      Assert.AreEqual(b[1], a[0,1]);
    }
    
    //test GetRow
    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetRowOutOfRange()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2,2);
      ComplexDoubleVector b = a.GetRow(3);
    }
    
    //test GetColumn
    [Test]
    public void GetColumn()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2,2);
      a[0,0] = new Complex(1);
      a[0,1] = new Complex(2);
      a[1,0] = new Complex(3);
      a[1,1] = new Complex(4);
      ComplexDoubleVector b = a.GetColumn(0);
      Assert.AreEqual(b[0], a[0,0]);
      Assert.AreEqual(b[1], a[1,0]);
    }
    
    //test GetColumn
    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetColumnOutOfRange()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2,2);
      ComplexDoubleVector b = a.GetColumn(3);
    } 

    //test GetDiagonal
    [Test]
    public void GetDiagonal()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2,2);
      a[0,0] = new Complex(1);
      a[0,1] = new Complex(2);
      a[1,0] = new Complex(3);
      a[1,1] = new Complex(4);
      ComplexDoubleVector b = a.GetDiagonal();
      Assert.AreEqual(b[0], a[0,0]);
      Assert.AreEqual(b[1], a[1,1]);
    }

    //test SetRow
    [Test]
    public void SetRow()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2,2);
      ComplexDoubleVector b = new ComplexDoubleVector(2);
      b[0] = new Complex(1,1);
      b[1] = new Complex(2,2);
      a.SetRow(0,b);
      Assert.AreEqual(b[0], a[0,0]);
      Assert.AreEqual(b[1], a[0,1]);
    }

    //test SetRow
    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void SetRowOutOfRange()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2,2);
      ComplexDoubleVector b = new ComplexDoubleVector(2);
      a.SetRow(2,b);
    }

    //test SetRow
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void SetRowWrongRank()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2,2);
      ComplexDoubleVector b = new ComplexDoubleVector(3);
      a.SetRow(1,b);
    }

    //test SetRow
    [Test]
    public void SetRowArray()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2,2);
      Complex[] b = new Complex[2];
      b[0] = new Complex(1,1);
      b[1] = new Complex(2,2);

      a.SetRow(0,b);
      Assert.AreEqual(b[0], a[0,0]);
      Assert.AreEqual(b[1], a[0,1]);
    }

    //test SetRow
    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void SetRowArrayOutOfRange()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2,2);
      Complex[] b = new Complex[2];
      a.SetRow(2,b);
    }

    //test SetRow
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void SetRowArrayWrongRank()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2,2);
      Complex[] b = new Complex[3];
      a.SetRow(1,b);
    }

    //test SetColumn
    [Test]
    public void SetColumn()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2,2);
      ComplexDoubleVector b = new ComplexDoubleVector(2);
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
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2,2);
      ComplexDoubleVector b = new ComplexDoubleVector(2);
      a.SetColumn(2,b);
    }

    //test SetColumn
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void SetColumnWrongRank()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2,2);
      ComplexDoubleVector b = new ComplexDoubleVector(3);
      a.SetColumn(1,b);
    }

    //test SetColumn
    [Test]
    public void SetColumnArray()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2,2);
      Complex[] b = new Complex[2];
      b[0] = new Complex(1,1);
      b[1] = new Complex(2,2);
      a.SetColumn(0,b);
      Assert.AreEqual(b[0], a[0,0]);
      Assert.AreEqual(b[1], a[1,0]);
    }

    //test SetColumn
    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void SetColumnArrayOutOfRange()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2,2);
      Complex[] b = new Complex[2];
      a.SetColumn(2,b);
    }

    //test SetColumn
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void SetColumnArrayWrongRank()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2,2);
      Complex[] b = new Complex[3];
      a.SetColumn(1,b);
    }

    //test SetDiagonal
    [Test]
    public void SetDiagonal()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2,2);
      ComplexDoubleVector b = new ComplexDoubleVector(2);
      b[0] = new Complex(1);
      b[1] = new Complex(2);
      a.SetDiagonal(b);
      Assert.AreEqual(b[0], a[0,0]);
      Assert.AreEqual(b[1], a[1,1]);
    }
    
    //test GetSubMatrix
    [Test]
    public void GetSubMatrix()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(4);
      a[0,0] = new Complex(1);
      a[0,1] = new Complex(2);
      a[0,2] = new Complex(3);
      a[0,3] = new Complex(4);
      a[1,0] = new Complex(5);
      a[1,1] = new Complex(6);
      a[1,2] = new Complex(7);
      a[1,3] = new Complex(8);
      a[2,0] = new Complex(9);
      a[2,1] = new Complex(10);
      a[2,2] = new Complex(11);
      a[2,3] = new Complex(12);
      a[3,0] = new Complex(13);
      a[3,1] = new Complex(14);
      a[3,2] = new Complex(15);
      a[3,3] = new Complex(16);
      ComplexDoubleMatrix b = a.GetSubMatrix(2,2);
      ComplexDoubleMatrix c = a.GetSubMatrix(0,1,2,2);
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
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(4);
      ComplexDoubleMatrix b = a.GetSubMatrix(-1,2);
    }
    //test GetSubMatrix
    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetSubMatrixOutRange2()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(4);
      ComplexDoubleMatrix b = a.GetSubMatrix(2,4);
    }

    //test GetSubMatrix
    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetSubMatrixOutRange3()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(4);
      ComplexDoubleMatrix b = a.GetSubMatrix(0,0,4,2);
    }

    //test GetSubMatrix
    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetSubMatrixOutRange4()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(4);
      ComplexDoubleMatrix b = a.GetSubMatrix(0,0,2,4);
    }

    //test GetSubMatrix
    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetSubMatrixOutRange5()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(4);
      ComplexDoubleMatrix b = a.GetSubMatrix(0,3,2,2);
    }
    
    //test GetUpperTriangle square matrix
    [Test]
    public void GetUpperTriangleSquare()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(3);
      a[0,0] = new Complex(1);
      a[0,1] = new Complex(2);
      a[0,2] = new Complex(3);
      a[1,0] = new Complex(4);
      a[1,1] = new Complex(5);
      a[1,2] = new Complex(6);
      a[2,0] = new Complex(7);
      a[2,1] = new Complex(8);
      a[2,2] = new Complex(9);
      ComplexDoubleMatrix b = a.GetUpperTriangle();
      
      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0,0], a[0,0]);
      Assert.AreEqual(b[0,1], a[0,1]);
      Assert.AreEqual(b[0,2], a[0,2]);
      Assert.AreEqual(b[1,0], Complex.Zero);
      Assert.AreEqual(b[1,1], a[1,1]);
      Assert.AreEqual(b[1,2], a[1,2]);
      Assert.AreEqual(b[2,0], Complex.Zero);
      Assert.AreEqual(b[2,1], Complex.Zero);    
      Assert.AreEqual(b[2,2], a[2,2]);
    }
    
    //test GetUpperTriangle long matrix
    [Test]
    public void GetUpperTriangleLong()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(3,2);
      a[0,0] = new Complex(1);
      a[0,1] = new Complex(2);
      a[1,0] = new Complex(4);
      a[1,1] = new Complex(5);
      a[2,0] = new Complex(7);
      a[2,1] = new Complex(8);
      ComplexDoubleMatrix b = a.GetUpperTriangle();
      
      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0,0], a[0,0]);
      Assert.AreEqual(b[0,1], a[0,1]);
      Assert.AreEqual(b[1,0], Complex.Zero);
      Assert.AreEqual(b[1,1], a[1,1]);
      Assert.AreEqual(b[2,0], Complex.Zero);
      Assert.AreEqual(b[2,1], Complex.Zero);    
    }

    //test GetUpperTriangle wide matrix
    [Test]
    public void GetUpperTriangleWide()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2,3);
      a[0,0] = new Complex(1);
      a[0,1] = new Complex(2);
      a[0,2] = new Complex(3);
      a[1,0] = new Complex(4);
      a[1,1] = new Complex(5);
      a[1,2] = new Complex(6);
      ComplexDoubleMatrix b = a.GetUpperTriangle();
      
      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0,0], a[0,0]);
      Assert.AreEqual(b[0,1], a[0,1]);
      Assert.AreEqual(b[0,2], a[0,2]);
      Assert.AreEqual(b[1,0], Complex.Zero);
      Assert.AreEqual(b[1,1], a[1,1]);
      Assert.AreEqual(b[1,2], a[1,2]);
    }

    //test GetStrictlyUpperTriangle square matrix
    [Test]
    public void GetStrictlyUpperTriangleSquare()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(3);
      a[0,0] = new Complex(1);
      a[0,1] = new Complex(2);
      a[0,2] = new Complex(3);
      a[1,0] = new Complex(4);
      a[1,1] = new Complex(5);
      a[1,2] = new Complex(6);
      a[2,0] = new Complex(7);
      a[2,1] = new Complex(8);
      a[2,2] = new Complex(9);
      ComplexDoubleMatrix b = a.GetStrictlyUpperTriangle();
      
      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0,0], Complex.Zero);
      Assert.AreEqual(b[0,1], a[0,1]);
      Assert.AreEqual(b[0,2], a[0,2]);
      Assert.AreEqual(b[1,0], Complex.Zero);
      Assert.AreEqual(b[1,1], Complex.Zero);
      Assert.AreEqual(b[1,2], a[1,2]);
      Assert.AreEqual(b[2,0], Complex.Zero);
      Assert.AreEqual(b[2,1], Complex.Zero);    
      Assert.AreEqual(b[2,2], Complex.Zero);
    }
    
    //test GetStrictlyUpperTriangle long matrix
    [Test]
    public void GetStrictlyUpperTriangleLong()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(3,2);
      a[0,0] = new Complex(1);
      a[0,1] = new Complex(2);
      a[1,0] = new Complex(4);
      a[1,1] = new Complex(5);
      a[2,0] = new Complex(7);
      a[2,1] = new Complex(8);
      ComplexDoubleMatrix b = a.GetStrictlyUpperTriangle();
      
      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0,0], Complex.Zero);
      Assert.AreEqual(b[0,1], a[0,1]);
      Assert.AreEqual(b[1,0], Complex.Zero);
      Assert.AreEqual(b[1,1], Complex.Zero);
      Assert.AreEqual(b[2,0], Complex.Zero);
      Assert.AreEqual(b[2,1], Complex.Zero);    
    }

    //test GetStrictlyUpperTriangle wide matrix
    [Test]
    public void GetStrictlyUpperTriangleWide()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2,3);
      a[0,0] = new Complex(1);
      a[0,1] = new Complex(2);
      a[0,2] = new Complex(3);
      a[1,0] = new Complex(4);
      a[1,1] = new Complex(5);
      a[1,2] = new Complex(6);
      ComplexDoubleMatrix b = a.GetStrictlyUpperTriangle();
      
      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0,0], Complex.Zero);
      Assert.AreEqual(b[0,1], a[0,1]);
      Assert.AreEqual(b[0,2], a[0,2]);
      Assert.AreEqual(b[1,0], Complex.Zero);
      Assert.AreEqual(b[1,1], Complex.Zero);
      Assert.AreEqual(b[1,2], a[1,2]);
    }


    //test GetLowerTriangle square matrix
    [Test]
    public void GetLowerTriangleSquare()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(3);
      a[0,0] = new Complex(1);
      a[0,1] = new Complex(2);
      a[0,2] = new Complex(3);
      a[1,0] = new Complex(4);
      a[1,1] = new Complex(5);
      a[1,2] = new Complex(6);
      a[2,0] = new Complex(7);
      a[2,1] = new Complex(8);
      a[2,2] = new Complex(9);
      ComplexDoubleMatrix b = a.GetLowerTriangle();
      
      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0,0], a[0,0]);
      Assert.AreEqual(b[0,1], Complex.Zero);
      Assert.AreEqual(b[0,2], Complex.Zero);
      Assert.AreEqual(b[1,0], a[1,0]);
      Assert.AreEqual(b[1,1], a[1,1]);
      Assert.AreEqual(b[1,2], Complex.Zero);
      Assert.AreEqual(b[2,0], a[2,0]);
      Assert.AreEqual(b[2,1], a[2,1]);    
      Assert.AreEqual(b[2,2], a[2,2]);
    }
    
    //test GetLowerTriangle long matrix
    [Test]
    public void GetLowerTriangleLong()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(3,2);
      a[0,0] = new Complex(1);
      a[0,1] = new Complex(2);
      a[1,0] = new Complex(4);
      a[1,1] = new Complex(5);
      a[2,0] = new Complex(7);
      a[2,1] = new Complex(8);
      ComplexDoubleMatrix b = a.GetLowerTriangle();
      
      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0,0], a[0,0]);
      Assert.AreEqual(b[0,1], Complex.Zero);
      Assert.AreEqual(b[1,0], b[1,0]);
      Assert.AreEqual(b[1,1], a[1,1]);
      Assert.AreEqual(b[2,0], b[2,0]);
      Assert.AreEqual(b[2,1], b[2,1]);    
    }

    //test GetLowerTriangle wide matrix
    [Test]
    public void GetLowerTriangleWide()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2,3);
      a[0,0] = new Complex(1);
      a[0,1] = new Complex(2);
      a[0,2] = new Complex(3);
      a[1,0] = new Complex(4);
      a[1,1] = new Complex(5);
      a[1,2] = new Complex(6);
      ComplexDoubleMatrix b = a.GetLowerTriangle();
      
      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0,0], a[0,0]);
      Assert.AreEqual(b[0,1], Complex.Zero);
      Assert.AreEqual(b[0,2], Complex.Zero);
      Assert.AreEqual(b[1,0], a[1,0]);
      Assert.AreEqual(b[1,1], a[1,1]);
      Assert.AreEqual(b[1,2], Complex.Zero);
    }

    //test GetStrictlyLowerTriangle square matrix
    [Test]
    public void GetStrictlyLowerTriangleSquare()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(3);
      a[0,0] = new Complex(1);
      a[0,1] = new Complex(2);
      a[0,2] = new Complex(3);
      a[1,0] = new Complex(4);
      a[1,1] = new Complex(5);
      a[1,2] = new Complex(6);
      a[2,0] = new Complex(7);
      a[2,1] = new Complex(8);
      a[2,2] = new Complex(9);
      ComplexDoubleMatrix b = a.GetStrictlyLowerTriangle();
      
      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0,0], Complex.Zero);
      Assert.AreEqual(b[0,1], Complex.Zero);
      Assert.AreEqual(b[0,2], Complex.Zero);
      Assert.AreEqual(b[1,0], a[1,0]);
      Assert.AreEqual(b[1,1], Complex.Zero);
      Assert.AreEqual(b[1,2], Complex.Zero);
      Assert.AreEqual(b[2,0], a[2,0]);
      Assert.AreEqual(b[2,1], a[2,1]);    
      Assert.AreEqual(b[2,2], Complex.Zero);
    }
    
    //test GetStrictlyLowerTriangle long matrix
    [Test]
    public void GetStrictlyLowerTriangleLong()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(3,2);
      a[0,0] = new Complex(1);
      a[0,1] = new Complex(2);
      a[1,0] = new Complex(4);
      a[1,1] = new Complex(5);
      a[2,0] = new Complex(7);
      a[2,1] = new Complex(8);
      ComplexDoubleMatrix b = a.GetStrictlyLowerTriangle();
      
      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0,0], Complex.Zero);
      Assert.AreEqual(b[0,1], Complex.Zero);
      Assert.AreEqual(b[1,0], b[1,0]);
      Assert.AreEqual(b[1,1], Complex.Zero);
      Assert.AreEqual(b[2,0], b[2,0]);
      Assert.AreEqual(b[2,1], b[2,1]);  
    }

    //test GetStrictlyLowerTriangle wide matrix
    [Test]
    public void GetStrictlyLowerTriangleWide()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2,3);
      a[0,0] = new Complex(1);
      a[0,1] = new Complex(2);
      a[0,2] = new Complex(3);
      a[1,0] = new Complex(4);
      a[1,1] = new Complex(5);
      a[1,2] = new Complex(6);
      ComplexDoubleMatrix b = a.GetStrictlyLowerTriangle();
      
      Assert.AreEqual(b.RowLength, a.RowLength);
      Assert.AreEqual(b.ColumnLength, a.ColumnLength);
      Assert.AreEqual(b[0,0], Complex.Zero);
      Assert.AreEqual(b[0,1], Complex.Zero);
      Assert.AreEqual(b[0,2], Complex.Zero);
      Assert.AreEqual(b[1,0], a[1,0]);
      Assert.AreEqual(b[1,1], Complex.Zero);
      Assert.AreEqual(b[1,2], Complex.Zero);
    }
    
    //static Negate
    [Test]
    public void Negate()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      a[0,0] = new Complex(1);
      a[0,1] = new Complex(2);
      a[1,0] = new Complex(3);
      a[1,1] = new Complex(4);

      ComplexDoubleMatrix b = ComplexDoubleMatrix.Negate(a);
      Assert.AreEqual(b[0,0],  new Complex(-1));
      Assert.AreEqual(b[0,1],  new Complex(-2));
      Assert.AreEqual(b[1,0],  new Complex(-3));
      Assert.AreEqual(b[1,1],  new Complex(-4));
    }
    
    //static NegateNull
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void NegateNull()
    {
      ComplexDoubleMatrix a = null;
      ComplexDoubleMatrix b = ComplexDoubleMatrix.Negate(a);
    }

    //static operator -
    [Test]
    public void OperatorMinus()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      a[0,0] = new Complex(1);
      a[0,1] = new Complex(2);
      a[1,0] = new Complex(3);
      a[1,1] = new Complex(4);

      ComplexDoubleMatrix b = -a;
      Assert.AreEqual(b[0,0],  new Complex(-1));
      Assert.AreEqual(b[0,1],  new Complex(-2));
      Assert.AreEqual(b[1,0],  new Complex(-3));
      Assert.AreEqual(b[1,1],  new Complex(-4));
    }

    //static operator - null
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void OperatorMinusNull()
    {
      ComplexDoubleMatrix a = null;
      ComplexDoubleMatrix b = -a;
    }   


    //static subtact two square matrices
    [Test]
    public void StaticSubtract()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      ComplexDoubleMatrix b = new ComplexDoubleMatrix(2);
      a[0,0] = b[0,0] = new Complex(1);
      a[0,1] = b[0,1] = new Complex(2);
      a[1,0] = b[1,0] = new Complex(3);
      a[1,1] = b[1,1] = new Complex(4);
      ComplexDoubleMatrix c = ComplexDoubleMatrix.Subtract(a,b);
      Assert.AreEqual(c[0,0],Complex.Zero);
      Assert.AreEqual(c[0,1],Complex.Zero);
      Assert.AreEqual(c[1,0],Complex.Zero);
      Assert.AreEqual(c[1,1],Complex.Zero);
    }

    //operator subtract two square matrices
    [Test]
    public void OperatorSubtract()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      ComplexDoubleMatrix b = new ComplexDoubleMatrix(2);
      a[0,0] = b[0,0] = new Complex(1);
      a[0,1] = b[0,1] = new Complex(2);
      a[1,0] = b[1,0] = new Complex(3);
      a[1,1] = b[1,1] = new Complex(4);
      ComplexDoubleMatrix c = a-b;
      Assert.AreEqual(c[0,0],Complex.Zero);
      Assert.AreEqual(c[0,1],Complex.Zero);
      Assert.AreEqual(c[1,0],Complex.Zero);
      Assert.AreEqual(c[1,1], Complex.Zero);
    }
  
    //member add subtract square matrices
    [Test]
    public void MemberSubtract()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      ComplexDoubleMatrix b = new ComplexDoubleMatrix(2);
      a[0,0] = b[0,0] = new Complex(1);
      a[0,1] = b[0,1] = new Complex(2);
      a[1,0] = b[1,0] = new Complex(3);
      a[1,1] = b[1,1] = new Complex(4);
      a.Subtract(b);
      Assert.AreEqual(a[0,0],Complex.Zero);
      Assert.AreEqual(a[0,1],Complex.Zero);
      Assert.AreEqual(a[1,0],Complex.Zero);
      Assert.AreEqual(a[1,1],Complex.Zero);
    }
    
    //static Subtract two square matrices, one null
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void StaticSubtractNull()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      ComplexDoubleMatrix b = null;
      ComplexDoubleMatrix c = ComplexDoubleMatrix.Subtract(a,b);
    }

    //operator Subtract two square matrices, one null
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void OperatorSubtractNull()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      ComplexDoubleMatrix b = null;
      ComplexDoubleMatrix c = a-b;
    }
  
    //member Subtract two square matrices, one null
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void MemberSubtractNull()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      ComplexDoubleMatrix b = null;
      a.Subtract(b);
    }

    //static Subtract two incompatible matrices
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void StaticSubtractIncompatible()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      ComplexDoubleMatrix b = new ComplexDoubleMatrix(3);
      ComplexDoubleMatrix c = ComplexDoubleMatrix.Subtract(a,b);
    }

    //operator Subtract two  incompatible matrices
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void OperatorSubtractIncompatible()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      ComplexDoubleMatrix b = new ComplexDoubleMatrix(3);
      ComplexDoubleMatrix c = a-b;
    }
  
    //member Subtract two  incompatible matricess
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void MemberSubtractIncompatible()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      ComplexDoubleMatrix b = new ComplexDoubleMatrix(3);
      a.Subtract(b);
    }

    //static add two square matrices
    [Test]
    public void StaticAdd()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      ComplexDoubleMatrix b = new ComplexDoubleMatrix(2);
      a[0,0] = b[0,0] = new Complex(1);
      a[0,1] = b[0,1] = new Complex(2);
      a[1,0] = b[1,0] = new Complex(3);
      a[1,1] = b[1,1] = new Complex(4);
      ComplexDoubleMatrix c = ComplexDoubleMatrix.Add(a,b);
      Assert.AreEqual(c[0,0],new Complex(2));
      Assert.AreEqual(c[0,1],new Complex(4));
      Assert.AreEqual(c[1,0],new Complex(6));
      Assert.AreEqual(c[1,1],new Complex(8));
    }

    //operator add two square matrices
    [Test]
    public void OperatorAdd()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      ComplexDoubleMatrix b = new ComplexDoubleMatrix(2);
      a[0,0] = b[0,0] = new Complex(1);
      a[0,1] = b[0,1] = new Complex(2);
      a[1,0] = b[1,0] = new Complex(3);
      a[1,1] = b[1,1] = new Complex(4);
      ComplexDoubleMatrix c = a+b;
      Assert.AreEqual(c[0,0],new Complex(2));
      Assert.AreEqual(c[0,1],new Complex(4));
      Assert.AreEqual(c[1,0],new Complex(6));
      Assert.AreEqual(c[1,1],new Complex(8));
    }
  
    //member add two square matrices
    [Test]
    public void MemberAdd()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      ComplexDoubleMatrix b = new ComplexDoubleMatrix(2);
      a[0,0] = b[0,0] = new Complex(1);
      a[0,1] = b[0,1] = new Complex(2);
      a[1,0] = b[1,0] = new Complex(3);
      a[1,1] = b[1,1] = new Complex(4);
      a.Add(b);
      Assert.AreEqual(a[0,0],new Complex(2));
      Assert.AreEqual(a[0,1],new Complex(4));
      Assert.AreEqual(a[1,0],new Complex(6));
      Assert.AreEqual(a[1,1],new Complex(8));
    }
    
    //static add two square matrices, one null
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void StaticAddNull()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      ComplexDoubleMatrix b = null;
      ComplexDoubleMatrix c = ComplexDoubleMatrix.Add(a,b);
    }

    //operator add two square matrices, one null
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void OperatorAddNull()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      ComplexDoubleMatrix b = null;
      ComplexDoubleMatrix c = a+b;
    }
  
    //member add two square matrices, one null
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void MemberAddNull()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      ComplexDoubleMatrix b = null;
      a.Add(b);
    }

    //static add two incompatible matrices
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void StaticAddIncompatible()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      ComplexDoubleMatrix b = new ComplexDoubleMatrix(3);
      ComplexDoubleMatrix c = ComplexDoubleMatrix.Add(a,b);
    }

    //operator add two  incompatible matrices
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void OperatorAddIncompatible()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      ComplexDoubleMatrix b = new ComplexDoubleMatrix(3);
      ComplexDoubleMatrix c = a+b;
    }
  
    //member add two  incompatible matricess
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void MemberAddIncompatible()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      ComplexDoubleMatrix b = new ComplexDoubleMatrix(3);
      a.Add(b);
    }


    //static divide matrix by double
    [Test]
    public void StaticDivide()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      a[0,0] = new Complex(2);
      a[0,1] = new Complex(4);
      a[1,0] = new Complex(6);
      a[1,1] = new Complex(8);
      ComplexDoubleMatrix b =  ComplexDoubleMatrix.Divide(a,2);
      Assert.AreEqual(b[0,0],new Complex(1));
      Assert.AreEqual(b[0,1],new Complex(2));
      Assert.AreEqual(b[1,0],new Complex(3));
      Assert.AreEqual(b[1,1],new Complex(4));
    }

    //operator divide matrix by double
    [Test]
    public void OperatorDivide()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      a[0,0] = new Complex(2);
      a[0,1] = new Complex(4);
      a[1,0] = new Complex(6);
      a[1,1] = new Complex(8);
      ComplexDoubleMatrix b = a/2;
      Assert.AreEqual(b[0,0],new Complex(1));
      Assert.AreEqual(b[0,1],new Complex(2));
      Assert.AreEqual(b[1,0],new Complex(3));
      Assert.AreEqual(b[1,1],new Complex(4));
    }
  
    //member divide matrix by double
    [Test]
    public void MemberDivide()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      a[0,0] = new Complex(2);
      a[0,1] = new Complex(4);
      a[1,0] = new Complex(6);
      a[1,1] = new Complex(8);
      a.Divide(2);
      Assert.AreEqual(a[0,0],new Complex(1));
      Assert.AreEqual(a[0,1],new Complex(2));
      Assert.AreEqual(a[1,0],new Complex(3));
      Assert.AreEqual(a[1,1],new Complex(4));
    }
    
    //static divide null matrix by double
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void StaticDivideNull()
    {
      ComplexDoubleMatrix a = null;
      ComplexDoubleMatrix b = ComplexDoubleMatrix.Divide(a,2);
    }

    //operator divide null matrix by double
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void OperatorDivideNull()
    {
      ComplexDoubleMatrix a = null;
      ComplexDoubleMatrix b = a/2;
    }

    //copy
    [Test]
    public void Copy()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      a[0,0] = new Complex(1);
      a[0,1] = new Complex(2);
      a[1,0] = new Complex(3);
      a[1,1] = new Complex(4);
      ComplexDoubleMatrix b = new ComplexDoubleMatrix(2);
      b.Copy(a);
      Assert.AreEqual(a[0,0],a[0,0]);
      Assert.AreEqual(a[0,1],b[0,1]);
      Assert.AreEqual(a[1,0],b[1,0]);
      Assert.AreEqual(a[1,1],b[1,1]);
    }

    //test multiply double matrix operator *
    [Test]
    public void OperatorMultiplyComplexDoubleMatrix()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      a[0,0] = new Complex(1);
      a[0,1] = new Complex(2);
      a[1,0] = new Complex(3);
      a[1,1] = new Complex(4);
      ComplexDoubleMatrix b = 2.0 * a;
      Assert.AreEqual(b[0,0],new Complex(2));
      Assert.AreEqual(b[0,1],new Complex(4));
      Assert.AreEqual(b[1,0],new Complex(6));
      Assert.AreEqual(b[1,1],new Complex(8));
    }
    
    //test multiply double null matrix operator * 
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void OperatorMultiplyComplexDoubleMatrixNull()
    {
      ComplexDoubleMatrix a = null;
      ComplexDoubleMatrix b = 2.0 * a;
    } 

    //test multiply  matrix double operator *
    [Test]
    public void OperatorMultiplyMatrixComplexDouble()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      a[0,0] = new Complex(1);
      a[0,1] = new Complex(2);
      a[1,0] = new Complex(3);
      a[1,1] = new Complex(4);
      ComplexDoubleMatrix b = a * 2.0;
      Assert.AreEqual(b[0,0],new Complex(2));
      Assert.AreEqual(b[0,1],new Complex(4));
      Assert.AreEqual(b[1,0],new Complex(6));
      Assert.AreEqual(b[1,1],new Complex(8));
    }
    
    //test multiply  null matrix double operator * 
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void OperatorMultiplyMatrixComplexDoubleNull()
    {
      ComplexDoubleMatrix a = null;
      ComplexDoubleMatrix b = a * 2;
    }
    
    //test static multiply double matrix 
    [Test]
    public void StaticMultiplyComplexDoubleMatrix()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      a[0,0] = new Complex(1);
      a[0,1] = new Complex(2);
      a[1,0] = new Complex(3);
      a[1,1] = new Complex(4);
      ComplexDoubleMatrix b = ComplexDoubleMatrix.Multiply(2.0,a);
      Assert.AreEqual(b[0,0].Real,2);
      Assert.AreEqual(b[0,1].Real,4);
      Assert.AreEqual(b[1,0].Real,6);
      Assert.AreEqual(b[1,1].Real,8);
    }
    
    //test static multiply double null matrix 
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void StaticMultiplyComplexDoubleMatrixNull()
    {
      ComplexDoubleMatrix a = null;
      ComplexDoubleMatrix b = ComplexDoubleMatrix.Multiply(2.0,a);

    } 

    //test static multiply  matrix double
    [Test]
    public void StaticMultiplyMatrixComplexDouble()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      a[0,0] = new Complex(1);
      a[0,1] = new Complex(2);
      a[1,0] = new Complex(3);
      a[1,1] = new Complex(4);
      ComplexDoubleMatrix b = ComplexDoubleMatrix.Multiply(a,2.0);

      Assert.AreEqual(b[0,0],new Complex(2));
      Assert.AreEqual(b[0,1],new Complex(4));
      Assert.AreEqual(b[1,0],new Complex(6));
      Assert.AreEqual(b[1,1],new Complex(8));
    }
    
    //test static multiply  null matrix double operator * 
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void StaticMultiplyMatrixComplexDoubleNull()
    {
      ComplexDoubleMatrix a = null;
      ComplexDoubleMatrix b = ComplexDoubleMatrix.Multiply(a,2.0);

    }

    //test member multiply  double 
    [Test]
    public void MemberMultiplyComplexDouble()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      a[0,0] = new Complex(1);
      a[0,1] = new Complex(2);
      a[1,0] = new Complex(3);
      a[1,1] = new Complex(4);
      a.Multiply(2.0);
      Assert.AreEqual(a[0,0],new Complex(2));
      Assert.AreEqual(a[0,1],new Complex(4));
      Assert.AreEqual(a[1,0],new Complex(6));
      Assert.AreEqual(a[1,1],new Complex(8));
    }

    //test multiply  matrix vector operator *
    [Test]
    public void OperatorMultiplyMatrixVector()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      a[0,0] = new Complex(1);
      a[0,1] = new Complex(2);
      a[1,0] = new Complex(3);
      a[1,1] = new Complex(4);
      ComplexDoubleVector b = new ComplexDoubleVector(2, 2.0);
      ComplexDoubleVector c = a * b;
      Assert.AreEqual(c[0],new Complex(6));
      Assert.AreEqual(c[1],new Complex(14));
    }
    
    //test multiply  matrix nonconform vector operator *
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void OperatorMultiplyMatrixNonConformVector()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      ComplexDoubleVector b = new ComplexDoubleVector(3, 2.0);
      ComplexDoubleVector c = a * b;
    }

    //test multiply null matrix vector operator *
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void OperatorMultiplyNullMatrixVector()
    {
      ComplexDoubleMatrix a = null;
      ComplexDoubleVector b = new ComplexDoubleVector(2, 2.0);
      ComplexDoubleVector c = a * b;
    }

    //test multiply matrix null vector operator *
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void OperatorMultiplyMatrixNullVector()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      ComplexDoubleVector b = null;
      ComplexDoubleVector c = a * b;
    }
    
    //test static multiply  matrix vector
    [Test]
    public void StaticMultiplyMatrixVector()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      a[0,0] = new Complex(1);
      a[0,1] = new Complex(2);
      a[1,0] = new Complex(3);
      a[1,1] = new Complex(4);
      ComplexDoubleVector b = new ComplexDoubleVector(2, 2.0);
      ComplexDoubleVector c = ComplexDoubleMatrix.Multiply(a,b);
      Assert.AreEqual(c[0],new Complex(6));
      Assert.AreEqual(c[1],new Complex(14));
    }

    //test static multiply  matrix nonconform vector
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void StaticMultiplyMatrixNonConformVector()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      ComplexDoubleVector b = new ComplexDoubleVector(3, 2.0);
      ComplexDoubleVector c = a * b;
    }

    //test static multiply null matrix vector
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void StaticMultiplyNullMatrixVector()
    {
      ComplexDoubleMatrix a = null;
      ComplexDoubleVector b = new ComplexDoubleVector(2, 2.0);
      ComplexDoubleVector c = ComplexDoubleMatrix.Multiply(a,b);
    }

    //test static multiply matrix null vector
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void StaticMultiplyMatrixNullVector()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      ComplexDoubleVector b = null;
      ComplexDoubleVector c = ComplexDoubleMatrix.Multiply(a,b);
    }

    //test member multiply vector
    [Test]
    public void MemberMultiplyVector()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      a[0,0] = new Complex(1);
      a[0,1] = new Complex(2);
      a[1,0] = new Complex(3);
      a[1,1] = new Complex(4);
      ComplexDoubleVector b = new ComplexDoubleVector(2, 2.0);
      a.Multiply(b);
      Assert.AreEqual(a[0,0],new Complex(6));
      Assert.AreEqual(a[1,0],new Complex(14));
      Assert.AreEqual(a.ColumnLength, 1);
      Assert.AreEqual(a.RowLength, 2);
    }
    
    //test member multiply  matrix nonconform vector
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void MemberMultiplyMatrixNonConformVector()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      ComplexDoubleVector b = new ComplexDoubleVector(3, 2.0);
      a.Multiply(b);
    }
    //test member multiply null vector
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void MemberMultiplyNullVector()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      ComplexDoubleVector b = null;
      a.Multiply(b);
    }

    //test multiply  matrix matrix operator *
    [Test]
    public void OperatorMultiplyMatrixMatrix()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      a[0,0] = new Complex(1);
      a[0,1] = new Complex(2);
      a[1,0] = new Complex(3);
      a[1,1] = new Complex(4);
      ComplexDoubleMatrix b = new ComplexDoubleMatrix(2, 2.0);
      ComplexDoubleMatrix c = a * b;
      Assert.AreEqual(c[0,0],new Complex(6));
      Assert.AreEqual(c[0,1],new Complex(6));
      Assert.AreEqual(c[1,0],new Complex(14));
      Assert.AreEqual(c[1,1],new Complex(14));
    }

    //test multiply  nonconform matrix matrix operator *
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void OperatorMultiplyMatrixNonConformMatrix()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      ComplexDoubleMatrix b = new ComplexDoubleMatrix(3, 2, 2.0);
      ComplexDoubleMatrix c = a * b;
    }

    //test multiply  long matrix wide matrix operator *
    [Test]
    public void OperatorMultiplyLongMatrixWideMatrix()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(3,2,1);
      ComplexDoubleMatrix b = new ComplexDoubleMatrix(2,3,2);
      ComplexDoubleMatrix c = a * b;
      Assert.AreEqual(c[0,0],new Complex(4));
      Assert.AreEqual(c[0,1],new Complex(4));
      Assert.AreEqual(c[0,1],new Complex(4));
      Assert.AreEqual(c[1,0],new Complex(4));
      Assert.AreEqual(c[1,1],new Complex(4));
      Assert.AreEqual(c[1,2],new Complex(4));
      Assert.AreEqual(c[2,0],new Complex(4));
      Assert.AreEqual(c[2,1],new Complex(4));
      Assert.AreEqual(c[2,2],new Complex(4));
    }

    //test multiply  wide matrix long matrix operator *
    [Test]
    public void OperatorMultiplyWideMatrixLongMatrix()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2,3,1);
      ComplexDoubleMatrix b = new ComplexDoubleMatrix(3,2,2);
      ComplexDoubleMatrix c = a * b;
      Assert.AreEqual(c[0,0],new Complex(6));
      Assert.AreEqual(c[0,1],new Complex(6));
      Assert.AreEqual(c[1,0],new Complex(6));
      Assert.AreEqual(c[1,1],new Complex(6));
    }

    //test multiply null matrix matrix operator *
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void OperatorMultiplyNullMatrixMatrix()
    {
      ComplexDoubleMatrix a = null;
      ComplexDoubleMatrix b = new ComplexDoubleMatrix(2, 2.0);
      ComplexDoubleMatrix c = a * b;
    }

    //test multiply matrix null matrix operator *
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void OperatorMultiplyMatrixNullMatrix()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2, 2.0);
      ComplexDoubleMatrix b = null;
      ComplexDoubleMatrix c = a * b;
    }

    //test static multiply  matrix matrix
    [Test]
    public void StaticMultiplyMatrixMatrix()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      a[0,0] = new Complex(1);
      a[0,1] = new Complex(2);
      a[1,0] = new Complex(3);
      a[1,1] = new Complex(4);
      ComplexDoubleMatrix b = new ComplexDoubleMatrix(2, 2, 2.0);
      ComplexDoubleMatrix c = ComplexDoubleMatrix.Multiply(a,b);
      Assert.AreEqual(c[0,0],new Complex(6));
      Assert.AreEqual(c[0,1],new Complex(6));
      Assert.AreEqual(c[1,0],new Complex(14));
      Assert.AreEqual(c[1,1],new Complex(14));
    }

    //test static multiply nonconform matrix matrix
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void StaticMultiplyMatrixNonConformMatrix()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      ComplexDoubleMatrix b = new ComplexDoubleMatrix(3, 2, 2.0);
      ComplexDoubleMatrix c = ComplexDoubleMatrix.Multiply(a,b);
    }

    //test static multiply  long matrix wide matrix
    [Test]
    public void StaticMultiplyLongMatrixWideMatrix()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(3,2,1);
      ComplexDoubleMatrix b = new ComplexDoubleMatrix(2,3,2);
      ComplexDoubleMatrix c = ComplexDoubleMatrix.Multiply(a,b);
      Assert.AreEqual(c[0,0],new Complex(4));
      Assert.AreEqual(c[0,1],new Complex(4));
      Assert.AreEqual(c[0,1],new Complex(4));
      Assert.AreEqual(c[1,0],new Complex(4));
      Assert.AreEqual(c[1,1],new Complex(4));
      Assert.AreEqual(c[1,2],new Complex(4));
      Assert.AreEqual(c[2,0],new Complex(4));
      Assert.AreEqual(c[2,1],new Complex(4));
      Assert.AreEqual(c[2,2],new Complex(4));
    }

    //test static multiply  wide matrix long matrix
    [Test]
    public void StaticMultiplyWideMatrixLongMatrix()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2,3,1);
      ComplexDoubleMatrix b = new ComplexDoubleMatrix(3,2,2);
      ComplexDoubleMatrix c = ComplexDoubleMatrix.Multiply(a,b);
      Assert.AreEqual(c[0,0],new Complex(6));
      Assert.AreEqual(c[0,1],new Complex(6));
      Assert.AreEqual(c[1,0],new Complex(6));
      Assert.AreEqual(c[1,1],new Complex(6));
    }

    //test static multiply null matrix matrix
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void StaticMultiplyNullMatrixMatrix()
    {
      ComplexDoubleMatrix a = null;
      ComplexDoubleMatrix b = new ComplexDoubleMatrix(2, 2.0);
      ComplexDoubleMatrix c = ComplexDoubleMatrix.Multiply(a,b);
    }

    //test static multiply matrix null matrix 
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void StaticMultiplyMatrixNullMatrix()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2, 2.0);
      ComplexDoubleMatrix b = null;
      ComplexDoubleMatrix c = ComplexDoubleMatrix.Multiply(a,b);
    }

    //test member multiply  matrix matrix
    [Test]
    public void MemberMultiplyMatrixMatrix()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      a[0,0] = new Complex(1);
      a[0,1] = new Complex(2);
      a[1,0] = new Complex(3);
      a[1,1] = new Complex(4);
      ComplexDoubleMatrix b = new ComplexDoubleMatrix(2, 2, 2.0);
      a.Multiply(b);
      Assert.AreEqual(a[0,0],new Complex(6));
      Assert.AreEqual(a[0,1],new Complex(6));
      Assert.AreEqual(a[1,0],new Complex(14));
      Assert.AreEqual(a[1,1],new Complex(14));
    }

    //test member multiply nonconform matrix matrix
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void MemberMultiplyMatrixNonConformMatrix()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      ComplexDoubleMatrix b = new ComplexDoubleMatrix(3, 2, 2.0);
      a.Multiply(b);
    }

    //test member multiply  long matrix wide matrix
    [Test]
    public void MemberMultiplyLongMatrixWideMatrix()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(3,2,1);
      ComplexDoubleMatrix b = new ComplexDoubleMatrix(2,3,2);
      a.Multiply(b);
      Assert.AreEqual(a[0,0],new Complex(4));
      Assert.AreEqual(a[0,1],new Complex(4));
      Assert.AreEqual(a[0,1],new Complex(4));
      Assert.AreEqual(a[1,0],new Complex(4));
      Assert.AreEqual(a[1,1],new Complex(4));
      Assert.AreEqual(a[1,2],new Complex(4));
      Assert.AreEqual(a[2,0],new Complex(4));
      Assert.AreEqual(a[2,1],new Complex(4));
      Assert.AreEqual(a[2,2],new Complex(4));
    }

    //test member multiply  wide matrix long matrix
    [Test]
    public void MemberMultiplyWideMatrixLongMatrix()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2,3,1);
      ComplexDoubleMatrix b = new ComplexDoubleMatrix(3,2,2);
      a.Multiply(b);
      Assert.AreEqual(a[0,0],new Complex(6));
      Assert.AreEqual(a[0,1],new Complex(6));
      Assert.AreEqual(a[1,0],new Complex(6));
      Assert.AreEqual(a[1,1],new Complex(6));
    }

    //test member multiply null matrix matrix
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void MemberMultiplyNullMatrixMatrix()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2, 2.0);
      ComplexDoubleMatrix b = null;
      a.Multiply(b);
    }

  
    //copy null
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CopyNull()
    {
      ComplexDoubleMatrix a =  null;
      ComplexDoubleMatrix b = new ComplexDoubleMatrix(2);
      b.Copy(a);
    }

    //Norm
    [Test]
    public void Norms()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      a[0,0] = new Complex(1.1, 1.1);
      a[0,1] = new Complex(2.2, -2.2);
      a[1,0] = new Complex(3.3, 3.3);
      a[1,1] = new Complex(4.4, -4.4);
      Assert.AreEqual(a.GetL1Norm(),9.334,TOLERENCE);
      Assert.AreEqual(a.GetL2Norm(),8.502,TOLERENCE);
      Assert.AreEqual(a.GetInfinityNorm(),10.889,TOLERENCE);
      Assert.AreEqual(a.GetFrobeniusNorm(),8.521,TOLERENCE);
    }

    //Wide Norm
    [Test]
    public void WideNorms()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2,3);
      a[0,0] = new Complex(1.1, 1.1);
      a[0,1] = new Complex(2.2, -2.2);
      a[0,2] = new Complex(3.3, 3.3);
      a[1,0] = new Complex(4.4, -4.4);
      a[1,1] = new Complex(5.5, 5.5);
      a[1,2] = new Complex(6.6, -6.6);
      Assert.AreEqual(a.GetL1Norm(),14.001,TOLERENCE);
      Assert.AreEqual(a.GetL2Norm(),13.845,TOLERENCE);
      Assert.AreEqual(a.GetInfinityNorm(),23.335,TOLERENCE);
      Assert.AreEqual(a.GetFrobeniusNorm(),14.840,TOLERENCE);
    }

    //Long Norm
    [Test]
    public void LongNorms()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(3,2);
      a[0,0] = new Complex(1.1, 1.1);
      a[0,1] = new Complex(2.2, -2.2);
      a[1,0] = new Complex(3.3, 3.3);
      a[1,1] = new Complex(4.4, -4.4);
      a[2,0] = new Complex(5.5, 5.5);
      a[2,1] = new Complex(6.6, -6.6);
      Assert.AreEqual(a.GetL1Norm(),18.668,TOLERENCE);
      Assert.AreEqual(a.GetL2Norm(),14.818,TOLERENCE);
      Assert.AreEqual(a.GetInfinityNorm(),17.112,TOLERENCE);
      Assert.AreEqual(a.GetFrobeniusNorm(),14.840,TOLERENCE);
    } 

    //Condition
    [Test]
    public void Condition()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      a[0,0] = new Complex(1.1, 1.1);
      a[0,1] = new Complex(2.2, -2.2);
      a[1,0] = new Complex(3.3, 3.3);
      a[1,1] = new Complex(4.4, -4.4);
      Assert.AreEqual(a.GetConditionNumber(),14.933,TOLERENCE);
    }

    //Wide Condition
    [Test]
    [ExpectedException(typeof(NotSquareMatrixException))]
    public void WideCondition()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2,3);
      a.GetConditionNumber();
    }

    //Long Condition
    [Test]
    [ExpectedException(typeof(NotSquareMatrixException))]
    public void LongCondition()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(3,2);
      a.GetConditionNumber();
    } 

    //clone
    [Test]
    public void Clone()
    {
      ComplexDoubleMatrix a = new ComplexDoubleMatrix(2);
      a[0,0] = new Complex(1);
      a[0,1] = new Complex(2);
      a[1,0] = new Complex(3);
      a[1,1] = new Complex(4);
      ComplexDoubleMatrix b = (ComplexDoubleMatrix)a.Clone();
      Assert.AreEqual(a[0,0],a[0,0]);
      Assert.AreEqual(a[0,1],b[0,1]);
      Assert.AreEqual(a[1,0],b[1,0]);
      Assert.AreEqual(a[1,1],b[1,1]);
    }
  }
}
