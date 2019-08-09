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
using NUnit.Framework;

namespace AltaxoTest.Calc.LinearAlgebra
{
  [TestFixture]
  public class FloatSVDDecompTest
  {
    private FloatMatrix a;
    private FloatMatrix wa;
    private FloatMatrix la;
    private FloatSVDDecomp svd;
    private FloatSVDDecomp lsvd;
    private FloatSVDDecomp wsvd;
    private const float TOLERENCE = 2.000E-6f;

    [OneTimeSetUp]
    public void SetupTestCases()
    {
      a = new FloatMatrix(3)
      {
        [0, 0] = 1.91f,
        [0, 1] = 9.82f,
        [0, 2] = 2.73f,
        [1, 0] = 8.64f,
        [1, 1] = 3.55f,
        [1, 2] = 7.46f,
        [2, 0] = 4.37f,
        [2, 1] = 6.28f,
        [2, 2] = 5.19f
      };
      svd = new FloatSVDDecomp(a, true);

      wa = new FloatMatrix(2, 4)
      {
        [0, 0] = 1.91f,
        [0, 1] = 9.82f,
        [0, 2] = 2.73f,
        [0, 3] = 8.64f,
        [1, 0] = 3.55f,
        [1, 1] = 7.46f,
        [1, 2] = 4.37f,
        [1, 3] = 6.28f
      };
      wsvd = new FloatSVDDecomp(wa, true);

      la = new FloatMatrix(4, 2)
      {
        [0, 0] = 1.91f,
        [0, 1] = 9.82f,
        [1, 0] = 2.73f,
        [1, 1] = 8.64f,
        [2, 0] = 3.55f,
        [2, 1] = 7.46f,
        [3, 0] = 4.37f,
        [3, 1] = 6.28f
      };
      lsvd = new FloatSVDDecomp(la, true);
    }

    [Test]
    public void Test()
    {
      FloatMatrix test = svd.U * svd.W * svd.V.GetTranspose();
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
      Assert.IsTrue(me < TOLERENCE, "Maximum Error = " + me.ToString());
    }

    [Test]
    public void LTest()
    {
      FloatMatrix test = lsvd.U * lsvd.W * lsvd.V.GetTranspose();
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
      Assert.IsTrue(me < TOLERENCE, "Maximum Error = " + me.ToString());
    }

    [Test]
    public void WTest()
    {
      FloatMatrix test = wsvd.U * wsvd.W * wsvd.V.GetTranspose();
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
      Assert.IsTrue(me < TOLERENCE, "Maximum Error = " + me.ToString());
    }

    [Test]
    public void RankTest()
    {
      Assert.AreEqual(svd.Rank, 3);
    }

    [Test]
    public void ConditionTest()
    {
      Assert.AreEqual(svd.Condition, 29.701, .001);
    }

    [Test]
    public void NormTest()
    {
      Assert.AreEqual(svd.Norm2, 16.849, .001);
    }

    [Test]
    public void LRankTest()
    {
      Assert.AreEqual(lsvd.Rank, 2);
    }

    [Test]
    public void LConditionTest()
    {
      Assert.AreEqual(lsvd.Condition, 6.551, .001);
    }

    [Test]
    public void LNormTest()
    {
      Assert.AreEqual(lsvd.Norm2, 17.376, .001);
    }

    [Test]
    public void WRankTest()
    {
      Assert.AreEqual(wsvd.Rank, 2);
    }

    [Test]
    public void WConditionTest()
    {
      Assert.AreEqual(wsvd.Condition, 7.321, .001);
    }

    [Test]
    public void WNormTest()
    {
      Assert.AreEqual(wsvd.Norm2, 17.416, .001);
    }
  }
}
