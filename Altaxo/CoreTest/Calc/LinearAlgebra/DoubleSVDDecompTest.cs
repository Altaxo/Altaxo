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
  public class DoubleSVDDecompTest 
  {
    private DoubleMatrix a;
    private DoubleMatrix wa;
    private DoubleMatrix la;
    private DoubleSVDDecomp svd;
    private DoubleSVDDecomp lsvd;
    private DoubleSVDDecomp wsvd;
    private const double TOLERENCE = 2.000E-014;
    
    [TestFixtureSetUp]
    public void SetupTestCases() 
    {
      a = new DoubleMatrix(3);
      a[0,0] = 1.91;
      a[0,1] = 9.82;
      a[0,2] = 2.73;
      a[1,0] = 8.64;
      a[1,1] = 3.55;
      a[1,2] = 7.46;
      a[2,0] = 4.37;
      a[2,1] = 6.28;
      a[2,2] = 5.19;
      svd = new DoubleSVDDecomp(a, true);
      
      wa = new DoubleMatrix(2,4);
      wa[0,0] = 1.91;
      wa[0,1] = 9.82;
      wa[0,2] = 2.73;
      wa[0,3] = 8.64;
      wa[1,0] = 3.55;
      wa[1,1] = 7.46;
      wa[1,2] = 4.37;
      wa[1,3] = 6.28;
      wsvd = new DoubleSVDDecomp(wa, true);
        
      la = new DoubleMatrix(4,2);
      la[0,0] = 1.91;
      la[0,1] = 9.82;
      la[1,0] = 2.73;
      la[1,1] = 8.64;
      la[2,0] = 3.55;
      la[2,1] = 7.46;
      la[3,0] = 4.37;
      la[3,1] = 6.28;
      lsvd = new DoubleSVDDecomp(la, true);
    } 
    
    [Test]
    public void Test()
    {
      DoubleMatrix test = svd.U * svd.W * svd.V.GetTranspose();
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
      DoubleMatrix test = lsvd.U * lsvd.W * lsvd.V.GetTranspose();
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
      DoubleMatrix test = wsvd.U * wsvd.W * wsvd.V.GetTranspose();
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
      Assert.AreEqual(svd.Condition,29.701,.001);
    }   

    [Test]
    public void NormTest()
    {
      Assert.AreEqual(svd.Norm2,16.849,.001);
    }   
    
    [Test]
    public void LRankTest()
    {
      Assert.AreEqual(lsvd.Rank,2);
    }
    
    [Test]
    public void LConditionTest()
    {
      Assert.AreEqual(lsvd.Condition,6.551,.001);
    }
    
    [Test]
    public void LNormTest()
    {
      Assert.AreEqual(lsvd.Norm2,17.376,.001);
    }

    [Test]
    public void WRankTest()
    {
      Assert.AreEqual(wsvd.Rank,2);
    }
    
    [Test]
    public void WConditionTest()
    {
      Assert.AreEqual(wsvd.Condition,7.321,.001);
    }
    
    [Test]
    public void WNormTest()
    {
      Assert.AreEqual(wsvd.Norm2,17.416,.001);
    } 
  }
}

