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
  public class ComplexFloatSVDDecompTest 
  {
    private ComplexFloatMatrix a;
    private ComplexFloatMatrix wa;
    private ComplexFloatMatrix la;
    private ComplexFloatSVDDecomp svd;
    private ComplexFloatSVDDecomp lsvd;
    private ComplexFloatSVDDecomp wsvd;
    private const float TOLERENCE = 2.000E-6f;
    
    [TestFixtureSetUp]
    public void SetupTestCases() 
    {
      a = new ComplexFloatMatrix(3);
      a[0,0] = new ComplexFloat(1.1f, 1.1f);
      a[0,1] = new ComplexFloat(2.2f, -2.2f);
      a[0,2] = new ComplexFloat(3.3f, 3.3f);
      a[1,0] = new ComplexFloat(4.4f, -4.4f);
      a[1,1] = new ComplexFloat(5.5f, 5.5f);
      a[1,2] = new ComplexFloat(6.6f, -6.6f);
      a[2,0] = new ComplexFloat(7.7f, 7.7f);
      a[2,1] = new ComplexFloat(8.8f, -8.8f);
      a[2,2] = new ComplexFloat(9.9f, 9.9f);
      svd = new ComplexFloatSVDDecomp(a, true);
      
      wa = new ComplexFloatMatrix(2,4);
      wa[0,0] = new ComplexFloat(1.1f, 1.1f);
      wa[0,1] = new ComplexFloat(2.2f, -2.2f);
      wa[0,2] = new ComplexFloat(3.3f, 3.3f);
      wa[0,3] = new ComplexFloat(4.4f, -4.4f);
      wa[1,0] = new ComplexFloat(5.5f, 5.5f);
      wa[1,1] = new ComplexFloat(6.6f, -6.6f);
      wa[1,2] = new ComplexFloat(7.7f, 7.7f);
      wa[1,3] = new ComplexFloat(8.8f, -8.8f);
      wsvd = new ComplexFloatSVDDecomp(wa, true);
        
      la = new ComplexFloatMatrix(4,2);
      la[0,0] = new ComplexFloat(1.1f, 1.1f);
      la[0,1] = new ComplexFloat(2.2f, -2.2f);
      la[1,0] = new ComplexFloat(3.3f, 3.3f);
      la[1,1] = new ComplexFloat(4.4f, -4.4f);
      la[2,0] = new ComplexFloat(5.5f, 5.5f);
      la[2,1] = new ComplexFloat(6.6f, -6.6f);
      la[3,0] = new ComplexFloat(7.7f, 7.7f);
      la[3,1] = new ComplexFloat(8.8f, -8.8f);
      lsvd = new ComplexFloatSVDDecomp(la, true);
    } 
    
    [Test]
    public void Test()
    {
      ComplexFloatMatrix test = svd.U * svd.W * svd.V.GetConjugateTranspose();

      float e;
      float me = 0;
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
      ComplexFloatMatrix test = lsvd.U * lsvd.W * lsvd.V.GetConjugateTranspose();
      float e;
      float me = 0;
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
      ComplexFloatMatrix test = wsvd.U * wsvd.W * wsvd.V.GetConjugateTranspose();
      float e;
      float me = 0;
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

