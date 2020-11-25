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

  public class ComplexDoubleSVDDecompTest
  {
    private ComplexDoubleMatrix a;
    private ComplexDoubleMatrix wa;
    private ComplexDoubleMatrix la;
    private ComplexDoubleSVDDecomp svd;
    private ComplexDoubleSVDDecomp lsvd;
    private ComplexDoubleSVDDecomp wsvd;
    private const double TOLERANCE = 2.000E-014;

    // [OneTimeSetUp]
    public ComplexDoubleSVDDecompTest()
    {
      a = new ComplexDoubleMatrix(3)
      {
        [0, 0] = new Complex(1.1, 1.1),
        [0, 1] = new Complex(2.2, -2.2),
        [0, 2] = new Complex(3.3, 3.3),
        [1, 0] = new Complex(4.4, -4.4),
        [1, 1] = new Complex(5.5, 5.5),
        [1, 2] = new Complex(6.6, -6.6),
        [2, 0] = new Complex(7.7, 7.7),
        [2, 1] = new Complex(8.8, -8.8),
        [2, 2] = new Complex(9.9, 9.9)
      };
      svd = new ComplexDoubleSVDDecomp(a, true);

      wa = new ComplexDoubleMatrix(2, 4)
      {
        [0, 0] = new Complex(1.1, 1.1),
        [0, 1] = new Complex(2.2, -2.2),
        [0, 2] = new Complex(3.3, 3.3),
        [0, 3] = new Complex(4.4, -4.4),
        [1, 0] = new Complex(5.5, 5.5),
        [1, 1] = new Complex(6.6, -6.6),
        [1, 2] = new Complex(7.7, 7.7),
        [1, 3] = new Complex(8.8, -8.8)
      };
      wsvd = new ComplexDoubleSVDDecomp(wa, true);

      la = new ComplexDoubleMatrix(4, 2)
      {
        [0, 0] = new Complex(1.1, 1.1),
        [0, 1] = new Complex(2.2, -2.2),
        [1, 0] = new Complex(3.3, 3.3),
        [1, 1] = new Complex(4.4, -4.4),
        [2, 0] = new Complex(5.5, 5.5),
        [2, 1] = new Complex(6.6, -6.6),
        [3, 0] = new Complex(7.7, 7.7),
        [3, 1] = new Complex(8.8, -8.8)
      };
      lsvd = new ComplexDoubleSVDDecomp(la, true);
    }

    [Fact]
    public void Test()
    {
      ComplexDoubleMatrix test = svd.U * svd.W * svd.V.GetConjugateTranspose();

      double e;
      double me = 0;
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
      ComplexDoubleMatrix test = lsvd.U * lsvd.W * lsvd.V.GetConjugateTranspose();
      double e;
      double me = 0;
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
      ComplexDoubleMatrix test = wsvd.U * wsvd.W * wsvd.V.GetConjugateTranspose();
      double e;
      double me = 0;
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
