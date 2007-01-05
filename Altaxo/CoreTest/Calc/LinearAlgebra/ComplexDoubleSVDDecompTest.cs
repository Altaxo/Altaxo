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
  public class ComplexDoubleSVDDecompTest 
  {
    private ComplexDoubleMatrix a;
    private ComplexDoubleMatrix wa;
    private ComplexDoubleMatrix la;
    private ComplexDoubleSVDDecomp svd;
    private ComplexDoubleSVDDecomp lsvd;
    private ComplexDoubleSVDDecomp wsvd;
    private const double TOLERENCE = 2.000E-014;
    
    [TestFixtureSetUp]
    public void SetupTestCases() 
    {
      a = new ComplexDoubleMatrix(3);
      a[0,0] = new Complex(1.1, 1.1);
      a[0,1] = new Complex(2.2, -2.2);
      a[0,2] = new Complex(3.3, 3.3);
      a[1,0] = new Complex(4.4, -4.4);
      a[1,1] = new Complex(5.5, 5.5);
      a[1,2] = new Complex(6.6, -6.6);
      a[2,0] = new Complex(7.7, 7.7);
      a[2,1] = new Complex(8.8, -8.8);
      a[2,2] = new Complex(9.9, 9.9);
      svd = new ComplexDoubleSVDDecomp(a, true);
      
      wa = new ComplexDoubleMatrix(2,4);
      wa[0,0] = new Complex(1.1, 1.1);
      wa[0,1] = new Complex(2.2, -2.2);
      wa[0,2] = new Complex(3.3, 3.3);
      wa[0,3] = new Complex(4.4, -4.4);
      wa[1,0] = new Complex(5.5, 5.5);
      wa[1,1] = new Complex(6.6, -6.6);
      wa[1,2] = new Complex(7.7, 7.7);
      wa[1,3] = new Complex(8.8, -8.8);
      wsvd = new ComplexDoubleSVDDecomp(wa, true);
        
      la = new ComplexDoubleMatrix(4,2);
      la[0,0] = new Complex(1.1, 1.1);
      la[0,1] = new Complex(2.2, -2.2);
      la[1,0] = new Complex(3.3, 3.3);
      la[1,1] = new Complex(4.4, -4.4);
      la[2,0] = new Complex(5.5, 5.5);
      la[2,1] = new Complex(6.6, -6.6);
      la[3,0] = new Complex(7.7, 7.7);
      la[3,1] = new Complex(8.8, -8.8);
      lsvd = new ComplexDoubleSVDDecomp(la, true);
    } 
    
    [Test]
    public void Test()
    {
      ComplexDoubleMatrix test = svd.U * svd.W * svd.V.GetConjugateTranspose();

      double e;
      double me = 0;
      for (int i = 0; i < test.RowLength; i++) 
      {
        for (int j = 0; j < test.ColumnLength ; j++) 
        {
          e = ComplexMath.Absolute((a[i, j] - test[i, j]) / a[i, j]);
          if (e > me) 
          {
            me = e;
          }
        }
      }
      Assert.IsTrue(me < TOLERENCE, "Maximum Error = " + me.ToString());      
    }

    [Test]
    public void LTest()
    {
      ComplexDoubleMatrix test = lsvd.U * lsvd.W * lsvd.V.GetConjugateTranspose();
      double e;
      double me = 0;
      for (int i = 0; i < test.RowLength; i++) 
      {
        for (int j = 0; j < test.ColumnLength ; j++) 
        {
          e = ComplexMath.Absolute((la[i, j] - test[i, j]) / la[i, j]);
          if (e > me) 
          {
            me = e;
          }
        }
      }
      Assert.IsTrue(me < TOLERENCE, "Maximum Error = " + me.ToString());      
    }
    
    [Test]
    public void WTest()
    {
      ComplexDoubleMatrix test = wsvd.U * wsvd.W * wsvd.V.GetConjugateTranspose();
      double e;
      double me = 0;
      for (int i = 0; i < test.RowLength; i++) 
      {
        for (int j = 0; j < test.ColumnLength ; j++) 
        {
          e = ComplexMath.Absolute((wa[i, j] - test[i, j]) / wa[i, j]);
          if (e > me) 
          {
            me = e;
          }
        }
      }
      Assert.IsTrue(me < TOLERENCE, "Maximum Error = " + me.ToString());      
    }
    
    [Test]
    public void RankTest()
    {
      Assert.AreEqual(svd.Rank,3);
    }   

    [Test]
    public void ConditionTest()
    {
      Assert.AreEqual(svd.Condition,14.650,.001);
    }   

    [Test]
    public void NormTest()
    {
      Assert.AreEqual(svd.Norm2,23.088,.001);
    }   
    
    [Test]
    public void LRankTest()
    {
      Assert.AreEqual(lsvd.Rank,2);
    }
    
    [Test]
    public void LConditionTest()
    {
      Assert.AreEqual(lsvd.Condition,22.764,.001);
    }
    
    [Test]
    public void LNormTest()
    {
      Assert.AreEqual(lsvd.Norm2,22.198,.001);
    }

    [Test]
    public void WRankTest()
    {
      Assert.AreEqual(wsvd.Rank,2);
    }
    
    [Test]
    public void WConditionTest()
    {
      Assert.AreEqual(wsvd.Condition,11.316,.001);
    }
    
    [Test]
    public void WNormTest()
    {
      Assert.AreEqual(wsvd.Norm2,22.133,.001);
    } 
  }
}

