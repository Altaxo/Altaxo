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
using System.Collections;
using NUnit.Framework;
using Altaxo.Calc;
using Altaxo.Calc.LinearAlgebra;

namespace AltaxoTest.Calc.LinearAlgebra 
{
  [TestFixture]
  public class ComplexDoubleVectorTest
  {
    private const double TOLERENCE = 0.001;

    //Test dimensions Constructor.
    [Test]
    public void CtorDimensions()
    {
      ComplexDoubleVector test = new ComplexDoubleVector(2);
      
      Assert.AreEqual(test.Length, 2);
      Assert.AreEqual(test[0],new Complex(0));
      Assert.AreEqual(test[1],new Complex(0));
    }
    
    //Test Copy Constructor.
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void CtorDimensionsZero() 
    {
      ComplexDoubleVector test = new ComplexDoubleVector(0);
    }
    
    //Test Copy Constructor.
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void CtorDimensionsNegative() 
    {
      ComplexDoubleVector test = new ComplexDoubleVector(-1);
    }
    
    //Test Intital Values Constructor.
    [Test]
    public void CtorInitialValues()
    {
      ComplexDoubleVector test = new ComplexDoubleVector(2, new Complex(1,-1));
      
      Assert.AreEqual(test.Length, 2);
      Assert.AreEqual(test[0],new Complex(1,-1));
      Assert.AreEqual(test[1],new Complex(1,-1));
    }
    
    //Test Array Constructor
    [Test]
    public void CtorArray()
    {
      double[] testvector = new double[2]{0,1};
      
      ComplexDoubleVector test = new ComplexDoubleVector(testvector);
      Assert.AreEqual(test.Length,testvector.Length);
      Assert.AreEqual(test[0],new Complex(testvector[0]));
      Assert.AreEqual(test[1],new Complex(testvector[1]));
    }
    
    //*TODO IList Constructor
    
    //Test Copy Constructor.
    [Test]
    public void CtorCopy()
    {
      ComplexDoubleVector a = new ComplexDoubleVector(new double[2]{0,1});
      ComplexDoubleVector b = new ComplexDoubleVector(a);
      
      Assert.AreEqual(b.Length,a.Length);
      Assert.AreEqual(b[0],a[0]);
      Assert.AreEqual(b[1],a[1]);
    }
    
    //Test Copy Constructor.
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CtorCopyNull()
    {
      ComplexDoubleVector a = null;
      ComplexDoubleVector b = new ComplexDoubleVector(a);
    }
    
    //Test Index Access
    [Test]
    [ExpectedException(typeof(IndexOutOfRangeException))]
    public void IndexAccessGetNegative()
    {
      ComplexDoubleVector a = new ComplexDoubleVector(new double[2]{0,1});
      Complex b = a[-1];
    }
    
    //Test Index Access
    [Test]
    [ExpectedException(typeof(IndexOutOfRangeException))]
    public void IndexAccessSetNegative()
    {
      ComplexDoubleVector a = new ComplexDoubleVector(2);
      a[-1]=1;
    }
    
    //Test Index Access
    [Test]
    [ExpectedException(typeof(IndexOutOfRangeException))]
    public void IndexAccessGetOutOfRange()
    {
      ComplexDoubleVector a = new ComplexDoubleVector(new double[2]{0,1});
      Complex b = a[2];
    }
    
    //Test Index Access
    [Test]
    [ExpectedException(typeof(IndexOutOfRangeException))]
    public void IndexAccessSetOutOfRange()
    {
      ComplexDoubleVector a = new ComplexDoubleVector(2);
      a[2]=1;
    }
    
    //Test Equals
    [Test]
    public void Equals()
    {
      ComplexDoubleVector a = new ComplexDoubleVector(2,4);
      ComplexDoubleVector b = new ComplexDoubleVector(2,4);
      ComplexDoubleVector c = new ComplexDoubleVector(2);
      c[0] = 4;
      c[1] = 4;

      ComplexDoubleVector d = new ComplexDoubleVector(2,5);
      ComplexDoubleVector e = null;
      FloatVector f = new FloatVector(2,4);
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
    
    //Test get real and imaginary components
    [Test]
    public void RealImag() 
    {
      ComplexDoubleVector a = new ComplexDoubleVector(2);
      a[0] = new Complex(1,2);
      a[1] = new Complex(3,4);
      
      DoubleVector a_real = a.Real;
      DoubleVector a_imag = a.Imag;
      
      Assert.AreEqual(a_real[0],a[0].Real);
      Assert.AreEqual(a_imag[0],a[0].Imag);
      Assert.AreEqual(a_real[1],a[1].Real);
      Assert.AreEqual(a_imag[1],a[1].Imag);
      Assert.AreEqual(a_real.Length,a.Length);
      Assert.AreEqual(a_imag.Length,a.Length);
    }
    
    //test GetHashCode
    [Test]
    public void TestHashCode()
    {
      ComplexDoubleVector a = new ComplexDoubleVector(2);
      a[0] = 0;
      a[1] = 1;

      int hash = a.GetHashCode();
      Assert.AreEqual(hash, 1);
    }
    
    //Test GetInternalData
    [Test]
    public void GetInternalData()
    {
      double[] testvector = new double[2]{0,1};
      ComplexDoubleVector test = new ComplexDoubleVector(testvector);
      Complex[] internaldata = test.GetInternalData();
      
      Assert.AreEqual(internaldata.Length,testvector.Length);
      Assert.AreEqual(internaldata[0],new Complex(testvector[0]));
      Assert.AreEqual(internaldata[1],new Complex(testvector[1]));
    }
    
    //Test ToArray
    [Test]
    public void ToArray()
    {
      double[] testvector = new double[2]{0,1};
      ComplexDoubleVector test = new ComplexDoubleVector(testvector);
      Complex[] internaldata = test.ToArray();
      
      Assert.AreEqual(internaldata.Length,testvector.Length);
      Assert.AreEqual(internaldata[0],new Complex(testvector[0]));
      Assert.AreEqual(internaldata[1],new Complex(testvector[1]));
    }
    
    //Test GetSubVector
    [Test]
    public void GetSubVector()
    {
      ComplexDoubleVector test = new ComplexDoubleVector(new double[4]{0,1,2,3});
      ComplexDoubleVector subvector = test.GetSubVector(1,2);
      
      Assert.AreEqual(subvector.Length,2);
      Assert.AreEqual(subvector[0],test[1]);
      Assert.AreEqual(subvector[1],test[2]);
    }
    
    //Test Implicit cast conversion to ComplexDoubleVector
    [Test]
    public void ImplicitConversion()
    {
      float[] a = new float[4]{0,1,2,3};
      double[] b = new double[4]{0,1,2,3};
      ComplexFloatVector c = new ComplexFloatVector(a);
      ComplexDoubleVector d, e, f;
      
      d = a; e=b; f=c;
      
      Assert.AreEqual(a.Length,d.Length);
      Assert.AreEqual((Complex)a[0],d[0]);
      Assert.AreEqual((Complex)a[1],d[1]);
      Assert.AreEqual((Complex)a[2],d[2]);
      Assert.AreEqual((Complex)a[3],d[3]);
      
      Assert.AreEqual(b.Length,e.Length);
      Assert.AreEqual((Complex)b[0],e[0]);
      Assert.AreEqual((Complex)b[1],e[1]);
      Assert.AreEqual((Complex)b[2],e[2]);
      Assert.AreEqual((Complex)b[3],e[3]);
      
      Assert.AreEqual(c.Length,f.Length);
      Assert.AreEqual((Complex)c[0],f[0]);
      Assert.AreEqual((Complex)c[1],f[1]);
      Assert.AreEqual((Complex)c[2],f[2]);
      Assert.AreEqual((Complex)c[3],f[3]);
    }
      
    //Test GetIndex functions
    [Test]
    public void GetIndex()
    {
      ComplexDoubleVector a = new ComplexDoubleVector(new double[4]{1,2,3,4});
      ComplexDoubleVector b = new ComplexDoubleVector(new double[4]{3,2,1,0});
      ComplexDoubleVector c = new ComplexDoubleVector(new double[4]{0,-1,-2,-3});
      ComplexDoubleVector d = new ComplexDoubleVector(new double[4]{-3,-2,-1,0});
      
      Assert.AreEqual(a.GetAbsMaximumIndex(),3);
      Assert.AreEqual(b.GetAbsMaximumIndex(),0);
      Assert.AreEqual(c.GetAbsMaximumIndex(),3);
      Assert.AreEqual(d.GetAbsMaximumIndex(),0);
      
      Assert.AreEqual(a.GetAbsMaximum(),(Complex)4);
      Assert.AreEqual(b.GetAbsMaximum(),(Complex)3);
      Assert.AreEqual(c.GetAbsMaximum(),(Complex)(-3));
      Assert.AreEqual(d.GetAbsMaximum(),(Complex)(-3));
      
      Assert.AreEqual(a.GetAbsMinimumIndex(),0);
      Assert.AreEqual(b.GetAbsMinimumIndex(),3);
      Assert.AreEqual(c.GetAbsMinimumIndex(),0);
      Assert.AreEqual(d.GetAbsMinimumIndex(),3);
      
      Assert.AreEqual(a.GetAbsMinimum(),(Complex)1);
      Assert.AreEqual(b.GetAbsMinimum(),(Complex)0);
      Assert.AreEqual(c.GetAbsMinimum(),(Complex)0);
      Assert.AreEqual(d.GetAbsMinimum(),(Complex)0); 
    }
    
    //Test invalid dimensions with copy
    [Test]
    [ExpectedException(typeof(System.ArgumentException))]
    public void CopyException()
    {
      ComplexDoubleVector a = new ComplexDoubleVector(new double[4]{0,1,2,3});
      ComplexDoubleVector b = new ComplexDoubleVector(5);
      
      a.Copy(b);
    }
    
    //Test invalid dimensions with swap
    [Test]
    [ExpectedException(typeof(System.ArgumentException))]
    public void SwapException()
    {
      ComplexDoubleVector a = new ComplexDoubleVector(new double[4]{0,1,2,3});
      ComplexDoubleVector b = new ComplexDoubleVector(new double[5]{4,5,6,7,8});
      
      a.Swap(b);
    }
    
    //Test Copy and Swap
    [Test]
    public void CopySwap()
    {
      ComplexDoubleVector a = new ComplexDoubleVector(new double[4]{0,1,2,3});
      ComplexDoubleVector b = new ComplexDoubleVector(new double[4]{4,5,6,7});
      ComplexDoubleVector c = new ComplexDoubleVector(4);
      ComplexDoubleVector d = new ComplexDoubleVector(4);
      
      a.Copy(c);
      b.Copy(d);
      
      Assert.AreEqual(a.Length,c.Length);
      Assert.AreEqual(a[0],c[0]);
      Assert.AreEqual(a[1],c[1]);
      Assert.AreEqual(a[2],c[2]);
      Assert.AreEqual(a[3],c[3]);
      
      Assert.AreEqual(b.Length,d.Length);
      Assert.AreEqual(b[0],d[0]);
      Assert.AreEqual(b[1],d[1]);
      Assert.AreEqual(b[2],d[2]);
      Assert.AreEqual(b[3],d[3]);
      
      a.Swap(b);
      
      Assert.AreEqual(b.Length,c.Length);
      Assert.AreEqual(b[0],c[0]);
      Assert.AreEqual(b[1],c[1]);
      Assert.AreEqual(b[2],c[2]);
      Assert.AreEqual(b[3],c[3]);
      
      Assert.AreEqual(a.Length,d.Length);
      Assert.AreEqual(a[0],d[0]);
      Assert.AreEqual(a[1],d[1]);
      Assert.AreEqual(a[2],d[2]);
      Assert.AreEqual(a[3],d[3]);
    }
    
    //Test GetDotProduct
    [Test]
    public void GetDotProduct()
    {
      ComplexDoubleVector a = new ComplexDoubleVector(new double[4]{0,1,2,3});
      ComplexDoubleVector b = new ComplexDoubleVector(new double[4]{4,5,6,7});
      
      Assert.AreEqual(a.GetDotProduct(),(Complex)14);
      Assert.AreEqual(b.GetDotProduct(),(Complex)126);
      Assert.AreEqual(a.GetDotProduct(b),(Complex)38);
      Assert.AreEqual(a.GetDotProduct(b),b.GetDotProduct(a));
      
    }
    
    //Test GetNorm
    [Test]
    public void GetNorm()
    {
      ComplexDoubleVector a = new ComplexDoubleVector(new double[4]{0,1,2,3});
      ComplexDoubleVector b = new ComplexDoubleVector(new double[4]{4,5,6,7});
      
      Assert.AreEqual(a.GetNorm(),System.Math.Sqrt(14));
      Assert.AreEqual(a.GetNorm(),a.GetNorm(2));
      Assert.AreEqual(a.GetNorm(0),3);
      
      Assert.AreEqual(b.GetNorm(),3*System.Math.Sqrt(14));
      Assert.AreEqual(b.GetNorm(),b.GetNorm(2));
      Assert.AreEqual(b.GetNorm(0),7);  
    }
    
    //Test GetSum
    [Test]
    public void GetSum()
    {
      ComplexDoubleVector a = new ComplexDoubleVector(new double[4]{0,1,2,3});
      ComplexDoubleVector b = new ComplexDoubleVector(new double[4]{4,5,6,7});
      
      Assert.AreEqual(a.GetSum(),(Complex)6);
      Assert.AreEqual(a.GetSumMagnitudes(),6);
      
      Assert.AreEqual(b.GetSum(),(Complex)22);
      Assert.AreEqual(b.GetSumMagnitudes(),22);
    }
    
    //Test Axpy and Scale
    [Test]
    public void Axpy()
    {
      ComplexDoubleVector a = new ComplexDoubleVector(new double[4]{0,1,2,3});
      Double scal = 3;
      ComplexDoubleVector b = new ComplexDoubleVector(4);
      
      b.Axpy(scal,a);
      a.Scale(scal);
      
      Assert.AreEqual(a[0],b[0]);
      Assert.AreEqual(a[1],b[1]);
      Assert.AreEqual(a[2],b[2]);
      Assert.AreEqual(a[3],b[3]);
    }
    
    //Test Negate
    [Test]
    public void Negate()
    {
      double[] vec = new double[4]{0,1,2,3};
      ComplexDoubleVector a = new ComplexDoubleVector(vec);
      ComplexDoubleVector b = -a;
      
      a = ComplexDoubleVector.Negate(a);
      
      Assert.AreEqual(-(Complex)vec[0],a[0]);
      Assert.AreEqual(-(Complex)vec[1],a[1]);
      Assert.AreEqual(-(Complex)vec[2],a[2]);
      Assert.AreEqual(-(Complex)vec[3],a[3]);
      
      Assert.AreEqual(-(Complex)vec[0],b[0]);
      Assert.AreEqual(-(Complex)vec[1],b[1]);
      Assert.AreEqual(-(Complex)vec[2],b[2]);
      Assert.AreEqual(-(Complex)vec[3],b[3]);
    }
    
    //Test Subtract
    [Test]
    public void Subtract()
    {
      ComplexDoubleVector a = new ComplexDoubleVector(new double[4]{0,1,2,3});
      ComplexDoubleVector b = new ComplexDoubleVector(new double[4]{4,5,6,7});
      ComplexDoubleVector c = new ComplexDoubleVector(a.Length);
      ComplexDoubleVector d = new ComplexDoubleVector(b.Length);
      
      c = a-b;
      d = ComplexDoubleVector.Subtract(a,b);
      
      Assert.AreEqual(c[0],a[0]-b[0]);
      Assert.AreEqual(c[1],a[1]-b[1]);
      Assert.AreEqual(c[2],a[2]-b[2]);
      Assert.AreEqual(c[3],a[3]-b[3]);
      
      Assert.AreEqual(d[0],c[0]);
      Assert.AreEqual(d[1],c[1]);
      Assert.AreEqual(d[2],c[2]);
      Assert.AreEqual(d[3],c[3]);
      
      a.Subtract(b);
      
      Assert.AreEqual(c[0],a[0]);
      Assert.AreEqual(c[1],a[1]);
      Assert.AreEqual(c[2],a[2]);
      Assert.AreEqual(c[3],a[3]);
    }
    
    //Test Add
    [Test]
    public void Add()
    {
      ComplexDoubleVector a = new ComplexDoubleVector(new double[4]{0,1,2,3});
      ComplexDoubleVector b = new ComplexDoubleVector(new double[4]{4,5,6,7});
      ComplexDoubleVector c = new ComplexDoubleVector(a.Length);
      ComplexDoubleVector d = new ComplexDoubleVector(b.Length);
      
      c = a+b;
      d = ComplexDoubleVector.Add(a,b);
      
      Assert.AreEqual(c[0],a[0]+b[0]);
      Assert.AreEqual(c[1],a[1]+b[1]);
      Assert.AreEqual(c[2],a[2]+b[2]);
      Assert.AreEqual(c[3],a[3]+b[3]);
      
      Assert.AreEqual(d[0],c[0]);
      Assert.AreEqual(d[1],c[1]);
      Assert.AreEqual(d[2],c[2]);
      Assert.AreEqual(d[3],c[3]);
      
      a.Add(b);
      
      Assert.AreEqual(c[0],a[0]);
      Assert.AreEqual(c[1],a[1]);
      Assert.AreEqual(c[2],a[2]);
      Assert.AreEqual(c[3],a[3]);
    }
    
    //Test Scale Mult and Divide
    [Test]
    public void ScalarMultiplyAndDivide()
    {
      ComplexDoubleVector a = new ComplexDoubleVector(new double[4]{0,1,2,3});
      ComplexDoubleVector c = new ComplexDoubleVector(a);
      ComplexDoubleVector d = new ComplexDoubleVector(a);
      double scal = -4;
        
      c.Multiply(scal);
      d.Divide(scal);
        
      Assert.AreEqual(c[0],a[0]*scal);
      Assert.AreEqual(c[1],a[1]*scal);
      Assert.AreEqual(c[2],a[2]*scal);
      Assert.AreEqual(c[3],a[3]*scal);  
      
      Assert.AreEqual(d[0],a[0]/scal);
      Assert.AreEqual(d[1],a[1]/scal);
      Assert.AreEqual(d[2],a[2]/scal);
      Assert.AreEqual(d[3],a[3]/scal);
      
      c = a*scal;
      
      Assert.AreEqual(c[0],a[0]*scal);
      Assert.AreEqual(c[1],a[1]*scal);
      Assert.AreEqual(c[2],a[2]*scal);
      Assert.AreEqual(c[3],a[3]*scal);  
      
      c = scal*a;
      
      Assert.AreEqual(c[0],a[0]*scal);
      Assert.AreEqual(c[1],a[1]*scal);
      Assert.AreEqual(c[2],a[2]*scal);
      Assert.AreEqual(c[3],a[3]*scal);
    }
    
    //Test Multiply
    [Test]
    public void Multiply()
    {
      ComplexDoubleVector a = new ComplexDoubleVector(new double[4]{0,1,2,3});
      ComplexDoubleVector b = new ComplexDoubleVector(new double[4]{4,5,6,7});
      ComplexDoubleMatrix c = new ComplexDoubleMatrix(a.Length,b.Length);
      ComplexDoubleMatrix d = new ComplexDoubleMatrix(a.Length,b.Length);
      
      c = a*b;
      d = ComplexDoubleVector.Multiply(a,b);
      
      Assert.AreEqual(c[0,0],a[0]*b[0]);
      Assert.AreEqual(c[0,1],a[0]*b[1]);
      Assert.AreEqual(c[0,2],a[0]*b[2]);
      Assert.AreEqual(c[0,3],a[0]*b[3]);
      Assert.AreEqual(c[1,0],a[1]*b[0]);
      Assert.AreEqual(c[1,1],a[1]*b[1]);
      Assert.AreEqual(c[1,2],a[1]*b[2]);
      Assert.AreEqual(c[1,3],a[1]*b[3]);
      Assert.AreEqual(c[2,0],a[2]*b[0]);
      Assert.AreEqual(c[2,1],a[2]*b[1]);
      Assert.AreEqual(c[2,2],a[2]*b[2]);
      Assert.AreEqual(c[2,3],a[2]*b[3]);
      Assert.AreEqual(c[3,0],a[3]*b[0]);
      Assert.AreEqual(c[3,1],a[3]*b[1]);
      Assert.AreEqual(c[3,2],a[3]*b[2]);
      Assert.AreEqual(c[3,3],a[3]*b[3]);
      
      Assert.AreEqual(d[0,0],a[0]*b[0]);
      Assert.AreEqual(d[0,1],a[0]*b[1]);
      Assert.AreEqual(d[0,2],a[0]*b[2]);
      Assert.AreEqual(d[0,3],a[0]*b[3]);
      Assert.AreEqual(d[1,0],a[1]*b[0]);
      Assert.AreEqual(d[1,1],a[1]*b[1]);
      Assert.AreEqual(d[1,2],a[1]*b[2]);
      Assert.AreEqual(d[1,3],a[1]*b[3]);
      Assert.AreEqual(d[2,0],a[2]*b[0]);
      Assert.AreEqual(d[2,1],a[2]*b[1]);
      Assert.AreEqual(d[2,2],a[2]*b[2]);
      Assert.AreEqual(d[2,3],a[2]*b[3]);
      Assert.AreEqual(d[3,0],a[3]*b[0]);
      Assert.AreEqual(d[3,1],a[3]*b[1]);
      Assert.AreEqual(d[3,2],a[3]*b[2]);
      Assert.AreEqual(d[3,3],a[3]*b[3]);
    }
    
    //Test Divide
    [Test]
    public void Divide()
    {
      ComplexDoubleVector a = new ComplexDoubleVector(new double[4]{0,1,2,3});
      ComplexDoubleVector c = new ComplexDoubleVector(a);
      ComplexDoubleVector d = new ComplexDoubleVector(a);
      double scal = -4;
        
      c = a/scal;
      d = ComplexDoubleVector.Divide(a,scal);
        
      Assert.AreEqual(c[0],a[0]/scal);
      Assert.AreEqual(c[1],a[1]/scal);
      Assert.AreEqual(c[2],a[2]/scal);
      Assert.AreEqual(c[3],a[3]/scal);  
      
      Assert.AreEqual(d[0],a[0]/scal);
      Assert.AreEqual(d[1],a[1]/scal);
      Assert.AreEqual(d[2],a[2]/scal);
      Assert.AreEqual(d[3],a[3]/scal);
    }
    
    //Test Clone
    [Test]
    public void Clone()
    {
      ComplexDoubleVector a = new ComplexDoubleVector(new double[4]{0,1,2,3});
      ComplexDoubleVector b = a.Clone();
      
      Assert.AreEqual(a[0],b[0]);
      Assert.AreEqual(a[1],b[1]);
      Assert.AreEqual(a[2],b[2]);
      Assert.AreEqual(a[3],b[3]);
      
      a=a*2;
      
      Assert.AreEqual(a[0],b[0]*2);
      Assert.AreEqual(a[1],b[1]*2);
      Assert.AreEqual(a[2],b[2]*2);
      Assert.AreEqual(a[3],b[3]*2);
    }
    
    //Test IEnumerable and DoubleVectorEnumerator
    [Test]
    [ExpectedException(typeof(InvalidOperationException))]
    public void GetEnumeratorException()
    {
      ComplexDoubleVector a = new ComplexDoubleVector(new Complex[4]{0,1,2,3});
      IEnumerator dve = a.GetEnumerator();
      
      Complex b = (Complex)dve.Current;
    }
    
    //Test IEnumerable and DoubleVectorEnumerator
    [Test]
    public void GetEnumerator()
    {
      ComplexDoubleVector a = new ComplexDoubleVector(new Complex[4]{0,1,2,3});
      IEnumerator dve = a.GetEnumerator();
      Complex b;
      bool c;
      
      c = dve.MoveNext();
      b = (Complex)dve.Current;
      Assert.AreEqual(c,true);
      Assert.AreEqual(b,(Complex)0);
      
      c = dve.MoveNext();
      b = (Complex)dve.Current;
      Assert.AreEqual(c,true);
      Assert.AreEqual(b,(Complex)1);
      
      c = dve.MoveNext();
      b = (Complex)dve.Current;
      Assert.AreEqual(c,true);
      Assert.AreEqual(b,(Complex)2);
      
      c = dve.MoveNext();
      b = (Complex)dve.Current;
      Assert.AreEqual(c,true);
      Assert.AreEqual(b,(Complex)3);
      
      c = dve.MoveNext();
      Assert.AreEqual(c,false);
    }
    
    //Partial ICollection tests
    [Test]
    public void ICollection()
    {
      ComplexDoubleVector a = new ComplexDoubleVector(new Complex[4]{0,1,2,3});
      Complex[] b = new Complex[5];
      
      Assert.AreEqual(a.Count,a.Length);
      
      a.CopyTo(b,1);
      Assert.AreEqual(b[0],(Complex)0);
      Assert.AreEqual(b[1],(Complex)0); 
      Assert.AreEqual(b[2],(Complex)1);
      Assert.AreEqual(b[3],(Complex)2);
      Assert.AreEqual(b[4],(Complex)3); 
    }
    
    // IList tests
    [Test]
    public void IList()
    {
      ComplexDoubleVector a = new ComplexDoubleVector(new Complex[4]{0,1,2,3});
      
      Assert.AreEqual(a.IsFixedSize,false);
      Assert.AreEqual(a.IsReadOnly,false);
      
      a.Add((Complex)4.0);
      Assert.AreEqual(a.Length,5);
      Assert.AreEqual(a[4],(Complex)4);
      Assert.AreEqual(a.Contains((Complex)4.0),true);
      
      a.Insert(1,(Complex)5.0);
      Assert.AreEqual(a.Length,6);
      Assert.AreEqual(a.Contains((Complex)5.0),true);
      Assert.AreEqual(a[0],(Complex)0);
      Assert.AreEqual(a[1],(Complex)5);
      Assert.AreEqual(a[2],(Complex)1);
      Assert.AreEqual(a[3],(Complex)2);
      Assert.AreEqual(a[4],(Complex)3);
      Assert.AreEqual(a[5],(Complex)4);
      
      a.Remove((Complex)5.0);
      Assert.AreEqual(a.Length,5);
      Assert.AreEqual(a.Contains((Complex)5.0),false);
      Assert.AreEqual(a[0],(Complex)0);
      Assert.AreEqual(a[1],(Complex)1);
      Assert.AreEqual(a[2],(Complex)2);
      Assert.AreEqual(a[3],(Complex)3);
      Assert.AreEqual(a[4],(Complex)4);
      
      a.RemoveAt(2);
      Assert.AreEqual(a.Length,4);
      Assert.AreEqual(a.Contains((Complex)2.0),false);
      Assert.AreEqual(a[0],(Complex)0);
      Assert.AreEqual(a[1],(Complex)1);
      Assert.AreEqual(a[2],(Complex)3);
      Assert.AreEqual(a[3],(Complex)4);
    }
  }
}
