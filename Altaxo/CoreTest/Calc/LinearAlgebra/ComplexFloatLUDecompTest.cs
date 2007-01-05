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
  public class ComplexFloatLUDecompTest 
  {
    private static ComplexFloatLUDecomp lu;
    private const double TOLERENCE = 0.001;

    static ComplexFloatLUDecompTest() 
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(3);
      a[0,0] = new ComplexFloat(-1,1);
      a[0,1] = 5;
      a[0,2] = 6;
      a[1,0] = 3;
      a[1,1] = -6;
      a[1,2] = 1;
      a[2,0] = 6;
      a[2,1] = 8;
      a[2,2] = 9;
      lu = new ComplexFloatLUDecomp(a);
    }

    [Test]
    [ExpectedException(typeof(NotSquareMatrixException))]
    public void LUWide()
    {
      ComplexFloatMatrix wm = new ComplexFloatMatrix(2,3);
      ComplexFloatLUDecomp wlu = new ComplexFloatLUDecomp(wm);

    }

    [Test]
    [ExpectedException(typeof(NotSquareMatrixException))]
    public void LULong()
    {
      ComplexFloatMatrix lm = new ComplexFloatMatrix(3,2);
      ComplexFloatLUDecomp llu = new ComplexFloatLUDecomp(lm);
    }

    [Test]
    public void LTest()
    {
      Assert.AreEqual(lu.L[0,0],ComplexFloat.One);
      Assert.AreEqual(lu.L[0,1],ComplexFloat.Zero);
      Assert.AreEqual(lu.L[0,2],ComplexFloat.Zero);
      Assert.AreEqual(lu.L[1,0].Real,.500,TOLERENCE);
      Assert.AreEqual(lu.L[1,1],ComplexFloat.One);
      Assert.AreEqual(lu.L[1,2],ComplexFloat.Zero);
      Assert.AreEqual(lu.L[2,0].Real,-.167,TOLERENCE);
      Assert.AreEqual(lu.L[2,1].Real,-.633,TOLERENCE);
      Assert.AreEqual(lu.L[2,0].Imag,.167,TOLERENCE);
      Assert.AreEqual(lu.L[2,1].Imag,.133,TOLERENCE);
      Assert.AreEqual(lu.L[2,2],ComplexFloat.One);

    }
    [Test]
    public void UTest()
    {
      Assert.AreEqual(lu.U[0,0].Real,6.000,TOLERENCE);
      Assert.AreEqual(lu.U[0,1].Real,8.000,TOLERENCE);
      Assert.AreEqual(lu.U[0,2].Real,9.000,TOLERENCE);
      Assert.AreEqual(lu.U[1,0],ComplexFloat.Zero);
      Assert.AreEqual(lu.U[1,1].Real,-10.000,TOLERENCE);
      Assert.AreEqual(lu.U[1,2].Real,-3.500,TOLERENCE);
      Assert.AreEqual(lu.U[2,0],ComplexFloat.Zero);
      Assert.AreEqual(lu.U[2,1],ComplexFloat.Zero);
      Assert.AreEqual(lu.U[2,2].Real,5.283,TOLERENCE);
      Assert.AreEqual(lu.U[2,2].Imag,-1.033,TOLERENCE);
    }

    [Test]
    public void GetDeterminantTest()
    {
      ComplexFloat det = lu.GetDeterminant();
      Assert.AreEqual(det.Real, 317.000,TOLERENCE);
      Assert.AreEqual(det.Imag, -62.000,TOLERENCE);
    }

    [Test]
    public void IsSingularTest()
    {
      Assert.IsFalse(lu.IsSingular);
      ComplexFloatMatrix b = new ComplexFloatMatrix(3);
      ComplexFloatLUDecomp dlu = new ComplexFloatLUDecomp(b);
      Assert.IsTrue(dlu.IsSingular);
    }

    [Test]
    public void GetInverseTest()
    {
      ComplexFloatMatrix inv = lu.GetInverse();
      Assert.AreEqual(inv[0,0].Real,-0.188378,TOLERENCE);
      Assert.AreEqual(inv[0,1].Real,0.009115,TOLERENCE);
      Assert.AreEqual(inv[0,2].Real,0.124572,TOLERENCE);
      Assert.AreEqual(inv[1,0].Real,-0.063805,TOLERENCE);
      Assert.AreEqual(inv[1,1].Real,-0.142074,TOLERENCE);
      Assert.AreEqual(inv[1,2].Real,0.058323,TOLERENCE);
      Assert.AreEqual(inv[2,0].Real,0.182301,TOLERENCE);
      Assert.AreEqual(inv[2,1].Real,0.120211,TOLERENCE);
      Assert.AreEqual(inv[2,2].Real,-0.02378,TOLERENCE);
      Assert.AreEqual(inv[0,0].Imag,-0.036844,TOLERENCE);
      Assert.AreEqual(inv[0,1].Imag,0.001783,TOLERENCE);
      Assert.AreEqual(inv[0,2].Imag,0.024364,TOLERENCE);
      Assert.AreEqual(inv[1,0].Imag,-0.012479,TOLERENCE);
      Assert.AreEqual(inv[1,1].Imag,0.000604,TOLERENCE);
      Assert.AreEqual(inv[1,2].Imag,0.008252,TOLERENCE);
      Assert.AreEqual(inv[2,0].Imag,0.035655,TOLERENCE);
      Assert.AreEqual(inv[2,1].Imag,-0.001725,TOLERENCE);
      Assert.AreEqual(inv[2,2].Imag,-0.023578,TOLERENCE);
    }


    [Test]
    [ExpectedException(typeof(SingularMatrixException))]
    public void GetInverseSingularTest()
    {
      ComplexFloatMatrix a = new ComplexFloatMatrix(3,3);
      ComplexFloatLUDecomp dlu = new ComplexFloatLUDecomp(a);
      dlu.GetInverse();
    }

    [Test]
    public void SolveMatrix()
    {
      ComplexFloatMatrix b = new ComplexFloatMatrix(3);
      b[0,0] = 2;
      b[0,1] = 2;
      b[0,2] = 2;
      b[1,0] = 13;
      b[1,1] = 13;
      b[1,2] = 13;
      b[2,0] = 25;
      b[2,1] = 25;
      b[2,2] = 25;
      ComplexFloatMatrix x = lu.Solve(b);
      Assert.AreEqual(x[0,0].Real,2.856,TOLERENCE);
      Assert.AreEqual(x[0,1].Real,2.856,TOLERENCE);
      Assert.AreEqual(x[0,2].Real,2.856,TOLERENCE);
      Assert.AreEqual(x[1,0].Real,-0.517,0.01);
      Assert.AreEqual(x[1,1].Real,-0.517,0.01);
      Assert.AreEqual(x[1,2].Real,-0.517,0.01);
      Assert.AreEqual(x[2,0].Real,1.333,TOLERENCE);
      Assert.AreEqual(x[2,1].Real,1.333,TOLERENCE);
      Assert.AreEqual(x[2,2].Real,1.333,TOLERENCE);
      Assert.AreEqual(x[0,0].Imag,0.559,TOLERENCE);
      Assert.AreEqual(x[0,1].Imag,0.559,TOLERENCE);
      Assert.AreEqual(x[0,2].Imag,0.559,TOLERENCE);
      Assert.AreEqual(x[1,0].Imag,0.189,TOLERENCE);
      Assert.AreEqual(x[1,1].Imag,0.189,TOLERENCE);
      Assert.AreEqual(x[1,2].Imag,0.189,TOLERENCE);
      Assert.AreEqual(x[2,0].Imag,-0.541,TOLERENCE);
      Assert.AreEqual(x[2,1].Imag,-0.541,TOLERENCE);
      Assert.AreEqual(x[2,2].Imag,-0.541,TOLERENCE);

      b = new ComplexFloatMatrix(3,2);
      b[0,0] = 2;
      b[0,1] = 2;
      b[1,0] = 13;
      b[1,1] = 13;
      b[2,0] = 25;
      b[2,1] = 25;
      x = lu.Solve(b);
      Assert.AreEqual(x[0,0].Real,2.856,TOLERENCE);
      Assert.AreEqual(x[0,1].Real,2.856,TOLERENCE);
      Assert.AreEqual(x[1,0].Real,-0.517,0.01);
      Assert.AreEqual(x[1,1].Real,-0.517,0.01);
      // Managed code on OSX returns -0.516
      Assert.AreEqual(x[2,0].Real,1.333,TOLERENCE);
      Assert.AreEqual(x[2,1].Real,1.333,TOLERENCE);
      Assert.AreEqual(x[0,0].Imag,0.559,TOLERENCE);
      Assert.AreEqual(x[0,1].Imag,0.559,TOLERENCE);
      Assert.AreEqual(x[1,0].Imag,0.189,TOLERENCE);
      Assert.AreEqual(x[1,1].Imag,0.189,TOLERENCE);
      Assert.AreEqual(x[2,0].Imag,-0.541,TOLERENCE);
      Assert.AreEqual(x[2,1].Imag,-0.541,TOLERENCE);
      
      b = new ComplexFloatMatrix(3,4);
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
      x = lu.Solve(b);
      Assert.AreEqual(x[0,0].Real,2.856,TOLERENCE);
      Assert.AreEqual(x[0,1].Real,2.856,TOLERENCE);
      Assert.AreEqual(x[0,2].Real,2.856,TOLERENCE);
      Assert.AreEqual(x[0,3].Real,2.856,TOLERENCE);
      Assert.AreEqual(x[1,0].Real,-0.517,0.01);
      Assert.AreEqual(x[1,1].Real,-0.517,0.01);
      Assert.AreEqual(x[1,2].Real,-0.517,0.01);
      Assert.AreEqual(x[1,3].Real,-0.517,0.01);
      Assert.AreEqual(x[2,0].Real,1.333,TOLERENCE);
      Assert.AreEqual(x[2,1].Real,1.333,TOLERENCE);
      Assert.AreEqual(x[2,2].Real,1.333,TOLERENCE);
      Assert.AreEqual(x[2,3].Real,1.333,TOLERENCE);
      Assert.AreEqual(x[0,0].Imag,0.559,TOLERENCE);
      Assert.AreEqual(x[0,1].Imag,0.559,TOLERENCE);
      Assert.AreEqual(x[0,2].Imag,0.559,TOLERENCE);
      Assert.AreEqual(x[0,3].Imag,0.559,TOLERENCE);
      Assert.AreEqual(x[1,0].Imag,0.189,TOLERENCE);
      Assert.AreEqual(x[1,1].Imag,0.189,TOLERENCE);
      Assert.AreEqual(x[1,2].Imag,0.189,TOLERENCE);
      Assert.AreEqual(x[1,3].Imag,0.189,TOLERENCE);
      Assert.AreEqual(x[2,0].Imag,-0.541,TOLERENCE);
      Assert.AreEqual(x[2,1].Imag,-0.541,TOLERENCE);
      Assert.AreEqual(x[2,2].Imag,-0.541,TOLERENCE);
      Assert.AreEqual(x[2,3].Imag,-0.541,TOLERENCE);
    }


    [Test]
    public void SolveVector()
    {
      ComplexFloatVector b = new ComplexFloatVector(3);
      b[0] = 2;
      b[1] = 13;
      b[2] = 25;
      ComplexFloatVector x = lu.Solve(b);
      Assert.AreEqual(x[0].Real,2.856,TOLERENCE);
      Assert.AreEqual(x[1].Real,-0.517,0.01);
      Assert.AreEqual(x[2].Real, 1.333,TOLERENCE);
      Assert.AreEqual(x[0].Imag,0.559,TOLERENCE);
      Assert.AreEqual(x[1].Imag,0.189,TOLERENCE);
      Assert.AreEqual(x[2].Imag,-0.541,TOLERENCE);     
    }
  }
}

