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
using Altaxo.Calc.LinearAlgebra;

namespace AltaxoTest.Calc.LinearAlgebra 
{
  [TestFixture]
  public class DoubleCholeskyDecompTest 
  {
    private static DoubleCholeskyDecomp cd;
    private const double TOLERENCE = 0.001;

    static DoubleCholeskyDecompTest()
    {
      DoubleMatrix a = new DoubleMatrix(3);
      a[0,0] = 2;
      a[0,1] = 1;
      a[0,2] = 0;
      a[1,0] = 1;
      a[1,1] = 2;
      a[1,2] = 0;
      a[2,0] = 0;
      a[2,1] = 0;
      a[2,2] = 3;
      cd = new DoubleCholeskyDecomp(a);
    }

    [Test]
    [ExpectedException(typeof(NotSquareMatrixException))]
    public void CDWide()
    {
      DoubleMatrix wm = new DoubleMatrix(2,3);
      DoubleCholeskyDecomp cd = new DoubleCholeskyDecomp(wm);
    }

    [Test]
    [ExpectedException(typeof(NotSquareMatrixException))]
    public void CDLong()
    {
      DoubleMatrix lm = new DoubleMatrix(3,2);
      DoubleCholeskyDecomp lcd = new DoubleCholeskyDecomp(lm);
    }

    [Test]
    public void FactorTest()
    {
      Assert.AreEqual(cd.Factor[0,0],1.414,TOLERENCE);
      Assert.AreEqual(cd.Factor[0,1],0);
      Assert.AreEqual(cd.Factor[0,2],0);
      Assert.AreEqual(cd.Factor[1,0],0.707,TOLERENCE);
      Assert.AreEqual(cd.Factor[1,1],1.225,TOLERENCE);
      Assert.AreEqual(cd.Factor[1,2],0);
      Assert.AreEqual(cd.Factor[2,0],0);
      Assert.AreEqual(cd.Factor[2,1],0);
      Assert.AreEqual(cd.Factor[2,2],1.732,TOLERENCE);
    }

    [Test]
    public void NonSymmFactorTest()
    {
      DoubleMatrix b = new DoubleMatrix(3);
      b[0,0] = 2;
      b[0,1] = 1;
      b[0,2] = 1;
      b[1,0] = 1;
      b[1,1] = 2;
      b[1,2] = 0;
      b[2,0] = 0;
      b[2,1] = 0;
      b[2,2] = 3;
      DoubleCholeskyDecomp dcd = new DoubleCholeskyDecomp(b);
      Assert.AreEqual(dcd.Factor[0,0],1.414,TOLERENCE);
      Assert.AreEqual(dcd.Factor[0,1],0.000,TOLERENCE);
      Assert.AreEqual(dcd.Factor[0,2],0.000,TOLERENCE);
      Assert.AreEqual(dcd.Factor[1,0],0.707,TOLERENCE);
      Assert.AreEqual(dcd.Factor[1,1],1.225,TOLERENCE);
      Assert.AreEqual(dcd.Factor[1,2],0.000,TOLERENCE);
      Assert.AreEqual(dcd.Factor[2,0],0.000,TOLERENCE);
      Assert.AreEqual(dcd.Factor[2,1],0.000,TOLERENCE);
      Assert.AreEqual(dcd.Factor[2,2],1.732,TOLERENCE);
    }

    [Test]
    public void IsPositiveDefiniteTest()
    {
      Assert.IsTrue(cd.IsPositiveDefinite);
      DoubleMatrix b = new DoubleMatrix(3);
      b[0,0] = -2;
      b[0,1] = 1;
      b[0,2] = 0;
      b[1,0] = 1;
      b[1,1] = 2;
      b[1,2] = 0;
      b[2,0] = 0;
      b[2,1] = 0;
      b[2,2] = 3;
      DoubleCholeskyDecomp dcd = new DoubleCholeskyDecomp(b);
      Assert.IsFalse(dcd.IsPositiveDefinite);
    }

    [Test]
    public void GetDeterminantTest()
    {
      double det = cd.GetDeterminant();
      Assert.AreEqual(det, 9.000,TOLERENCE);
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
      DoubleMatrix x = cd.Solve(b);
      Assert.AreEqual(x[0,0],-3.000,TOLERENCE);
      Assert.AreEqual(x[0,1],-3.000,TOLERENCE);
      Assert.AreEqual(x[0,2],-3.000,TOLERENCE);
      Assert.AreEqual(x[1,0],8.000,TOLERENCE);
      Assert.AreEqual(x[1,1],8.000,TOLERENCE);
      Assert.AreEqual(x[1,2],8.000,TOLERENCE);
      Assert.AreEqual(x[2,0],8.333,TOLERENCE);
      Assert.AreEqual(x[2,1],8.333,TOLERENCE);
      Assert.AreEqual(x[2,2],8.333,TOLERENCE);

      b = new DoubleMatrix(3,2);
      b[0,0] = 2;
      b[0,1] = 2;
      b[1,0] = 13;
      b[1,1] = 13;
      b[2,0] = 25;
      b[2,1] = 25;
      x = cd.Solve(b);
      Assert.AreEqual(x[0,0],-3.000,TOLERENCE);
      Assert.AreEqual(x[0,1],-3.000,TOLERENCE);
      Assert.AreEqual(x[1,0],8.000,TOLERENCE);
      Assert.AreEqual(x[1,1],8.000,TOLERENCE);
      Assert.AreEqual(x[2,0],8.333,TOLERENCE);
      Assert.AreEqual(x[2,1],8.333,TOLERENCE);
    
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
      x = cd.Solve(b);
      Assert.AreEqual(x[0,0],-3.000,TOLERENCE);
      Assert.AreEqual(x[0,1],-3.000,TOLERENCE);
      Assert.AreEqual(x[0,2],-3.000,TOLERENCE);
      Assert.AreEqual(x[0,3],-3.000,TOLERENCE);
      Assert.AreEqual(x[1,0],8.000,TOLERENCE);
      Assert.AreEqual(x[1,1],8.000,TOLERENCE);
      Assert.AreEqual(x[1,2],8.000,TOLERENCE);
      Assert.AreEqual(x[1,3],8.000,TOLERENCE);
      Assert.AreEqual(x[2,0],8.333,TOLERENCE);
      Assert.AreEqual(x[2,1],8.333,TOLERENCE);
      Assert.AreEqual(x[2,2],8.333,TOLERENCE);
      Assert.AreEqual(x[2,3],8.333,TOLERENCE);
    }
    
    [Test]
    public void SolveVector()
    {
      DoubleVector b = new DoubleVector(3);
      b[0] = 2;
      b[1] = 13;
      b[2] = 25;
      DoubleVector x = cd.Solve(b);
      Assert.AreEqual(x[0],-3.000,TOLERENCE);
      Assert.AreEqual(x[1],8.000,TOLERENCE);
      Assert.AreEqual(x[2],8.333,TOLERENCE);
    }
    [Test]
    public void GetInverseTest()
    {
      DoubleMatrix inv = cd.GetInverse();
      Assert.AreEqual(inv[0,0],0.666667,TOLERENCE);
      Assert.AreEqual(inv[0,1],-0.333333,TOLERENCE);
      Assert.AreEqual(inv[0,2],0.000,TOLERENCE);
      Assert.AreEqual(inv[1,0],-0.333333,TOLERENCE);
      Assert.AreEqual(inv[1,1],0.666667,TOLERENCE);
      Assert.AreEqual(inv[1,2],0.000,TOLERENCE);
      Assert.AreEqual(inv[2,0],0.000,TOLERENCE);
      Assert.AreEqual(inv[2,1],0.000,TOLERENCE);
      Assert.AreEqual(inv[2,2],0.333333,TOLERENCE);
    }

    [Test]
    [ExpectedException(typeof(NotPositiveDefiniteException))]
    public void GetInverseNotPositiveDefiniteTest()
    {
      DoubleMatrix a = new DoubleMatrix(3,3);
      DoubleCholeskyDecomp dcd = new DoubleCholeskyDecomp(a);
      dcd.GetInverse();
    }
  }
}

