#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

#endregion Copyright

using System;
using Altaxo.Calc;
using Altaxo.Calc.LinearAlgebra;
using Xunit;

namespace AltaxoTest.Calc.LinearAlgebra
{

  public class ComplexFloatSVDDecompTest
  {
    private ComplexFloatMatrix a;
    private ComplexFloatMatrix wa;
    private ComplexFloatMatrix la;
    private ComplexFloatSVDDecomp svd;
    private ComplexFloatSVDDecomp lsvd;
    private ComplexFloatSVDDecomp wsvd;
    private const float TOLERANCE = 2.000E-6f;

    // [OneTimeSetUp]
    public ComplexFloatSVDDecompTest()
    {
      a = new ComplexFloatMatrix(3)
      {
        [0, 0] = new ComplexFloat(1.1f, 1.1f),
        [0, 1] = new ComplexFloat(2.2f, -2.2f),
        [0, 2] = new ComplexFloat(3.3f, 3.3f),
        [1, 0] = new ComplexFloat(4.4f, -4.4f),
        [1, 1] = new ComplexFloat(5.5f, 5.5f),
        [1, 2] = new ComplexFloat(6.6f, -6.6f),
        [2, 0] = new ComplexFloat(7.7f, 7.7f),
        [2, 1] = new ComplexFloat(8.8f, -8.8f),
        [2, 2] = new ComplexFloat(9.9f, 9.9f)
      };
      svd = new ComplexFloatSVDDecomp(a, true);

      wa = new ComplexFloatMatrix(2, 4)
      {
        [0, 0] = new ComplexFloat(1.1f, 1.1f),
        [0, 1] = new ComplexFloat(2.2f, -2.2f),
        [0, 2] = new ComplexFloat(3.3f, 3.3f),
        [0, 3] = new ComplexFloat(4.4f, -4.4f),
        [1, 0] = new ComplexFloat(5.5f, 5.5f),
        [1, 1] = new ComplexFloat(6.6f, -6.6f),
        [1, 2] = new ComplexFloat(7.7f, 7.7f),
        [1, 3] = new ComplexFloat(8.8f, -8.8f)
      };
      wsvd = new ComplexFloatSVDDecomp(wa, true);

      la = new ComplexFloatMatrix(4, 2)
      {
        [0, 0] = new ComplexFloat(1.1f, 1.1f),
        [0, 1] = new ComplexFloat(2.2f, -2.2f),
        [1, 0] = new ComplexFloat(3.3f, 3.3f),
        [1, 1] = new ComplexFloat(4.4f, -4.4f),
        [2, 0] = new ComplexFloat(5.5f, 5.5f),
        [2, 1] = new ComplexFloat(6.6f, -6.6f),
        [3, 0] = new ComplexFloat(7.7f, 7.7f),
        [3, 1] = new ComplexFloat(8.8f, -8.8f)
      };
      lsvd = new ComplexFloatSVDDecomp(la, true);
    }

    [Fact]
    public void Test()
    {
      ComplexFloatMatrix test = svd.U * svd.W * svd.V.GetConjugateTranspose();

      float e;
      float me = 0;
      for (int i = 0; i < test.RowLength; i++)
      {
        for (int j = 0; j < test.ColumnLength; j++)
        {
          e = ComplexMath.Absolute((a[i, j] - test[i, j]) / a[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.True(me < TOLERANCE, "Maximum Error = " + me.ToString());
    }

    [Fact]
    public void LTest()
    {
      ComplexFloatMatrix test = lsvd.U * lsvd.W * lsvd.V.GetConjugateTranspose();
      float e;
      float me = 0;
      for (int i = 0; i < test.RowLength; i++)
      {
        for (int j = 0; j < test.ColumnLength; j++)
        {
          e = ComplexMath.Absolute((la[i, j] - test[i, j]) / la[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.True(me < TOLERANCE, "Maximum Error = " + me.ToString());
    }

    [Fact]
    public void WTest()
    {
      ComplexFloatMatrix test = wsvd.U * wsvd.W * wsvd.V.GetConjugateTranspose();
      float e;
      float me = 0;
      for (int i = 0; i < test.RowLength; i++)
      {
        for (int j = 0; j < test.ColumnLength; j++)
        {
          e = ComplexMath.Absolute((wa[i, j] - test[i, j]) / wa[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.True(me < TOLERANCE, "Maximum Error = " + me.ToString());
    }

    [Fact]
    public void RankTest()
    {
      Assert.Equal(3, svd.Rank);
    }

    [Fact]
    public void ConditionTest()
    {
      AssertEx.Equal(svd.Condition, 14.650, .001);
    }

    [Fact]
    public void NormTest()
    {
      AssertEx.Equal(svd.Norm2, 23.088, .001);
    }

    [Fact]
    public void LRankTest()
    {
      Assert.Equal(2, lsvd.Rank);
    }

    [Fact]
    public void LConditionTest()
    {
      AssertEx.Equal(lsvd.Condition, 22.764, .001);
    }

    [Fact]
    public void LNormTest()
    {
      AssertEx.Equal(lsvd.Norm2, 22.198, .001);
    }

    [Fact]
    public void WRankTest()
    {
      Assert.Equal(2, wsvd.Rank);
    }

    [Fact]
    public void WConditionTest()
    {
      AssertEx.Equal(wsvd.Condition, 11.316, .001);
    }

    [Fact]
    public void WNormTest()
    {
      AssertEx.Equal(wsvd.Norm2, 22.133, .001);
    }
  }
}
