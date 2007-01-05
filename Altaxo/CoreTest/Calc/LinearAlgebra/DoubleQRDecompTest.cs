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
using Altaxo.Calc.LinearAlgebra;

namespace AltaxoTest.Calc.LinearAlgebra 
{
  [TestFixture]
  public class DoubleQRDecompTest 
  {
    private static DoubleQRDecomp qr;
    private static DoubleQRDecomp wqr;
    private static DoubleQRDecomp lqr;
    private const double TOLERENCE = 0.001;

    static DoubleQRDecompTest()
    {        
      DoubleMatrix a = new DoubleMatrix(3);
      a[0,0] = -1.0;
      a[0,1] = 5.0;
      a[0,2] = 6.0;
      a[1,0] = 3.0;
      a[1,1] = -6.0;
      a[1,2] = 1.0;
      a[2,0] = 6.0;
      a[2,1] = 8.0;
      a[2,2] = 9.0;
      qr = new DoubleQRDecomp(a);

      a = new DoubleMatrix(2,3);
      a[0,0] = -1.0;
      a[0,1] = 5.0;
      a[0,2] = 6.0;
      a[1,0] = 3.0;
      a[1,1] = -6.0;
      a[1,2] = 1.0;
      wqr = new DoubleQRDecomp(a);

      a = new DoubleMatrix(3,2);
      a[0,0] = -1.0;
      a[0,1] = 5.0;
      a[1,0] = 3.0;
      a[1,1] = -6.0;
      a[2,0] = 6.0;
      a[2,1] = 8.0;
      lqr = new DoubleQRDecomp(a);
    }

    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void NullTest()
    {
      DoubleQRDecomp test = new DoubleQRDecomp(null);
    }

    [Test]
    public void QTest()
    {
      DoubleMatrix Q = qr.Q;
      Assert.AreEqual(System.Math.Abs(Q[0,0]), 0.147,TOLERENCE);
      Assert.AreEqual(System.Math.Abs(Q[0,1]), 0.525,TOLERENCE);
      Assert.AreEqual(System.Math.Abs(Q[0,2]), 0.838,TOLERENCE);
      Assert.AreEqual(System.Math.Abs(Q[1,0]), 0.442,TOLERENCE);
      Assert.AreEqual(System.Math.Abs(Q[1,1]), 0.723,TOLERENCE);
      Assert.AreEqual(System.Math.Abs(Q[1,2]), 0.531,TOLERENCE);
      Assert.AreEqual(System.Math.Abs(Q[2,0]), 0.885,TOLERENCE);
      Assert.AreEqual(System.Math.Abs(Q[2,1]), 0.449,TOLERENCE);
      Assert.AreEqual(System.Math.Abs(Q[2,2]), 0.126,TOLERENCE);
    }

    [Test]
    public void RTest() 
    {
      DoubleMatrix R = qr.R;
      Assert.AreEqual(System.Math.Abs(R[0,0]), 6.782,TOLERENCE);
      Assert.AreEqual(System.Math.Abs(R[0,1]), 3.686,TOLERENCE);
      Assert.AreEqual(System.Math.Abs(R[0,2]), 7.520,TOLERENCE);
      Assert.AreEqual(System.Math.Abs(R[1,0]), 0.000,TOLERENCE);
      Assert.AreEqual(System.Math.Abs(R[1,1]),10.555,TOLERENCE);
      Assert.AreEqual(System.Math.Abs(R[1,2]), 6.469,TOLERENCE);
      Assert.AreEqual(System.Math.Abs(R[2,0]), 0.000,TOLERENCE);
      Assert.AreEqual(System.Math.Abs(R[2,1]), 0.000,TOLERENCE);
      Assert.AreEqual(System.Math.Abs(R[2,2]), 4.428,TOLERENCE);
    }


    [Test]
    public void WideQTest()
    {
      DoubleMatrix Q = wqr.Q;
      Assert.AreEqual(System.Math.Abs(Q[0,0]), 0.316,TOLERENCE);
      Assert.AreEqual(System.Math.Abs(Q[0,1]), 0.949,TOLERENCE);
      Assert.AreEqual(System.Math.Abs(Q[1,0]), 0.949,TOLERENCE);
      Assert.AreEqual(System.Math.Abs(Q[1,1]), 0.316,TOLERENCE);
    }

    [Test]
    public void WideRTest() 
    {
      DoubleMatrix R = wqr.R;
      Assert.AreEqual(System.Math.Abs(R[0,0]), 3.162,TOLERENCE);
      Assert.AreEqual(System.Math.Abs(R[0,1]), 7.273,TOLERENCE);
      Assert.AreEqual(System.Math.Abs(R[0,2]), 0.949,TOLERENCE);
      Assert.AreEqual(System.Math.Abs(R[1,0]), 0.000,TOLERENCE);
      Assert.AreEqual(System.Math.Abs(R[1,1]), 2.846,TOLERENCE);
      Assert.AreEqual(System.Math.Abs(R[1,2]), 6.008,TOLERENCE);
    }

    [Test]
    public void LongQTest()
    {
      DoubleMatrix Q = lqr.Q;
      Assert.AreEqual(System.Math.Abs(Q[0,0]), 0.147,TOLERENCE);
      Assert.AreEqual(System.Math.Abs(Q[0,1]), 0.525,TOLERENCE);
      Assert.AreEqual(System.Math.Abs(Q[0,2]), 0.838,TOLERENCE);
      Assert.AreEqual(System.Math.Abs(Q[1,0]), 0.442,TOLERENCE);
      Assert.AreEqual(System.Math.Abs(Q[1,1]), 0.723,TOLERENCE);
      Assert.AreEqual(System.Math.Abs(Q[1,2]), 0.531,TOLERENCE);
      Assert.AreEqual(System.Math.Abs(Q[2,0]), 0.885,TOLERENCE);
      Assert.AreEqual(System.Math.Abs(Q[2,1]), 0.449,TOLERENCE);
      Assert.AreEqual(System.Math.Abs(Q[2,2]), 0.126,TOLERENCE);
    }

    [Test]
    public void LongRTest() 
    {
      DoubleMatrix R = lqr.R;
      Assert.AreEqual(System.Math.Abs(R[0,0]), 6.782,TOLERENCE);
      Assert.AreEqual(System.Math.Abs(R[0,1]), 3.686,TOLERENCE);
      Assert.AreEqual(System.Math.Abs(R[1,0]), 0.000,TOLERENCE);
      Assert.AreEqual(System.Math.Abs(R[1,1]), 10.555,TOLERENCE);
      Assert.AreEqual(System.Math.Abs(R[2,0]), 0.000,TOLERENCE);
      Assert.AreEqual(System.Math.Abs(R[2,1]), 0.000,TOLERENCE);
    }

    [Test]
    public void SolveMatrix()
    {
      DoubleMatrix b = new DoubleMatrix(3);
      b[0,0] = 2;
      b[0,1] = 2;
      b[0,2] = 2;
      b[1,0] = 13;
      b[1,1] = 13;
      b[1,2] = 13;
      b[2,0] = 25;
      b[2,1] = 25;
      b[2,2] = 25;
      DoubleMatrix x = qr.Solve(b);
      Assert.AreEqual(x[0,0],2.965,TOLERENCE);
      Assert.AreEqual(x[0,1],2.965,TOLERENCE);
      Assert.AreEqual(x[0,2],2.965,TOLERENCE);
      Assert.AreEqual(x[1,0],-0.479,TOLERENCE);
      Assert.AreEqual(x[1,1],-0.479,TOLERENCE);
      Assert.AreEqual(x[1,2],-0.479,TOLERENCE);
      Assert.AreEqual(x[2,0],1.227,TOLERENCE);
      Assert.AreEqual(x[2,1],1.227,TOLERENCE);
      Assert.AreEqual(x[2,2],1.227,TOLERENCE);

      b = new DoubleMatrix(3,2);
      b[0,0] = 2;
      b[0,1] = 2;
      b[1,0] = 13;
      b[1,1] = 13;
      b[2,0] = 25;
      b[2,1] = 25;
      x = qr.Solve(b);
      Assert.AreEqual(x[0,0],2.965,TOLERENCE);
      Assert.AreEqual(x[0,1],2.965,TOLERENCE);
      Assert.AreEqual(x[1,0],-0.479,TOLERENCE);
      Assert.AreEqual(x[1,1],-0.479,TOLERENCE);
      Assert.AreEqual(x[2,0],1.227,TOLERENCE);
      Assert.AreEqual(x[2,1],1.227,TOLERENCE);

      b = new DoubleMatrix(3,4);
      b[0,0] = 2;
      b[0,1] = 2;
      b[0,2] = 2;
      b[0,3] = 2;
      b[1,0] = 13;
      b[1,1] = 13;
      b[1,2] = 13;
      b[1,3] = 13;
      b[2,0] = 25;
      b[2,1] = 25;
      b[2,2] = 25;
      b[2,3] = 25;
      x = qr.Solve(b);
      Assert.AreEqual(x[0,0],2.965,TOLERENCE);
      Assert.AreEqual(x[0,1],2.965,TOLERENCE);
      Assert.AreEqual(x[0,2],2.965,TOLERENCE);
      Assert.AreEqual(x[0,3],2.965,TOLERENCE);
      Assert.AreEqual(x[1,0],-0.479,TOLERENCE);
      Assert.AreEqual(x[1,1],-0.479,TOLERENCE);
      Assert.AreEqual(x[1,2],-0.479,TOLERENCE);
      Assert.AreEqual(x[1,3],-0.479,TOLERENCE);
      Assert.AreEqual(x[2,0],1.227,TOLERENCE);
      Assert.AreEqual(x[2,1],1.227,TOLERENCE);
      Assert.AreEqual(x[2,2],1.227,TOLERENCE);
      Assert.AreEqual(x[2,3],1.227,TOLERENCE);

      DoubleMatrix A = new DoubleMatrix(4,3);
      A[0,0] = -4.18;
      A[0,1] = -5.011;
      A[0,2] = -5.841;
      A[1,0] = 4.986;
      A[1,1] = 5.805;
      A[1,2] = 6.624;
      A[2,0] = 3.695;
      A[2,1] = 3.687;
      A[2,2] = 3.679;
      A[3,0] = -5.489;
      A[3,1] = -7.024;
      A[3,2] =  8.56;

      DoubleQRDecomp qrd = new DoubleQRDecomp(A);
      DoubleMatrix B = new DoubleMatrix(4,1);
      B[0,0] = 1;
      B[1,0] = 4;
      B[2,0] = 2;
      B[3,0] = 1;

      x = qrd.Solve(B);
      Assert.AreEqual(x[0,0],2.73529,TOLERENCE);
      Assert.AreEqual(x[1,0],-2.15822,TOLERENCE);
      Assert.AreEqual(x[2,0], 0.0998564,TOLERENCE);

      B = new DoubleMatrix(4,3);
      B[0,0] = 1;
      B[1,0] = 4;
      B[2,0] = 2;
      B[3,0] = 1;
      B[0,1] = 1;
      B[1,1] = 4;
      B[2,1] = 2;
      B[3,1] = 1;
      B[0,2] = 1;
      B[1,2] = 4;
      B[2,2] = 2;
      B[3,2] = 1;

      x = qrd.Solve(B);
      Assert.AreEqual(x[0,0],2.73529,TOLERENCE);
      Assert.AreEqual(x[1,0],-2.15822,TOLERENCE);
      Assert.AreEqual(x[2,0], 0.0998564,TOLERENCE);
      Assert.AreEqual(x[0,1],2.73529,TOLERENCE);
      Assert.AreEqual(x[1,1],-2.15822,TOLERENCE);
      Assert.AreEqual(x[2,1], 0.0998564,TOLERENCE);
      Assert.AreEqual(x[0,2],2.73529,TOLERENCE);
      Assert.AreEqual(x[1,2],-2.15822,TOLERENCE);
      Assert.AreEqual(x[2,2], 0.0998564,TOLERENCE);
    }

    [Test]
    public void GetDeterminantTest()
    {
      double det = qr.GetDeterminant();
      Assert.AreEqual(det, 317.000,TOLERENCE);
    }   

    [Test]
    [ExpectedException(typeof(NotSquareMatrixException))]
    public void GetWideDeterminantTest()
    {
      double det = wqr.GetDeterminant();
    }

    [Test]
    [ExpectedException(typeof(NotSquareMatrixException))]
    public void GetLongDeterminantTest()
    {
      double det = lqr.GetDeterminant();
    }
  }
}
