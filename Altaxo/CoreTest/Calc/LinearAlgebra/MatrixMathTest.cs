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
  /// <summary>
  /// Summary description for MatrixMathTests.
  /// </summary>

  public class MatrixMathTest
  {
    [Fact]
    public void PseudoInverse2x2Test()
    {
      var ma = CreateMatrix.Dense<double>(2, 2);
      {
        ma[0, 0] = 2;
        ma[0, 1] = 1;
        ma[1, 0] = 2;
        ma[1, 1] = 1;
      };

      IMatrix<double> mb = MatrixMath.PseudoInverse(ma);

      AssertEx.Equal(0.2, mb[0, 0], DoubleConstants.DBL_EPSILON);
      AssertEx.Equal(0.2, mb[0, 1], DoubleConstants.DBL_EPSILON);
      AssertEx.Equal(0.1, mb[1, 0], DoubleConstants.DBL_EPSILON);
      AssertEx.Equal(0.1, mb[1, 1], DoubleConstants.DBL_EPSILON);
    }

    [Fact]
    public void PseudoInverse3x2Test()
    {
      var ma = CreateMatrix.Dense<double>(3, 2);
      {
        ma[0, 0] = 2;
        ma[0, 1] = 1;
        ma[1, 0] = 2;
        ma[1, 1] = 1;
        ma[2, 0] = 1;
        ma[2, 1] = 2;      };

      IMatrix<double> mb = MatrixMath.PseudoInverse(ma);

      Assert.Equal(2, mb.RowCount);
      Assert.Equal(3, mb.ColumnCount);

      AssertEx.Equal(1.0 / 3.0, mb[0, 0], 3 * DoubleConstants.DBL_EPSILON);
      AssertEx.Equal(1.0 / 3.0, mb[0, 1], 3 * DoubleConstants.DBL_EPSILON);
      AssertEx.Equal(-1.0 / 3.0, mb[0, 2], 3 * DoubleConstants.DBL_EPSILON);
      AssertEx.Equal(-1.0 / 6.0, mb[1, 0], 3 * DoubleConstants.DBL_EPSILON);
      AssertEx.Equal(-1.0 / 6.0, mb[1, 1], 3 * DoubleConstants.DBL_EPSILON);
      AssertEx.Equal(2.0 / 3.0, mb[1, 2], 3 * DoubleConstants.DBL_EPSILON);
    }

    [Fact]
    public void PseudoInverse2x3Test()
    {
      var ma = CreateMatrix.Dense<double>(2, 3);
      {
        ma[0, 0] = 1;
        ma[0, 1] = 2;
        ma[0, 2] = 4;
        ma[1, 0] = 1;
        ma[1, 1] = 2;
        ma[1, 2] = 4;      };

      IMatrix<double> mb = MatrixMath.PseudoInverse(ma);

      Assert.Equal(3, mb.RowCount);
      Assert.Equal(2, mb.ColumnCount);

      AssertEx.Equal(1.0 / 42.0, mb[0, 0], 3 * DoubleConstants.DBL_EPSILON);
      AssertEx.Equal(1.0 / 42.0, mb[0, 1], 3 * DoubleConstants.DBL_EPSILON);
      AssertEx.Equal(1.0 / 21.0, mb[1, 0], 3 * DoubleConstants.DBL_EPSILON);
      AssertEx.Equal(1.0 / 21.0, mb[1, 1], 3 * DoubleConstants.DBL_EPSILON);
      AssertEx.Equal(2.0 / 21.0, mb[2, 0], 3 * DoubleConstants.DBL_EPSILON);
      AssertEx.Equal(2.0 / 21.0, mb[2, 1], 3 * DoubleConstants.DBL_EPSILON);
    }

    [Fact]
    public void PseudoInverse3x3Rank3Test()
    {
      var ma = CreateMatrix.Dense<double>(3, 3);
      {
        ma[0, 0] = 1;
        ma[0, 1] = 2;
        ma[0, 2] = 4;
        ma[1, 0] = 1;
        ma[1, 1] = 4;
        ma[1, 2] = 2;
        ma[2, 0] = 4;
        ma[2, 1] = 1;
        ma[2, 2] = 2;      };

      IMatrix<double> mb = MatrixMath.PseudoInverse(ma);

      Assert.Equal(3, mb.RowCount);
      Assert.Equal(3, mb.ColumnCount);

      AssertEx.Equal(-1.0 / 7.0, mb[0, 0], 3 * DoubleConstants.DBL_EPSILON);
      AssertEx.Equal(0, mb[0, 1], 3 * DoubleConstants.DBL_EPSILON);
      AssertEx.Equal(2.0 / 7.0, mb[0, 2], 3 * DoubleConstants.DBL_EPSILON);
      AssertEx.Equal(-1.0 / 7.0, mb[1, 0], 3 * DoubleConstants.DBL_EPSILON);
      AssertEx.Equal(1.0 / 3.0, mb[1, 1], 3 * DoubleConstants.DBL_EPSILON);
      AssertEx.Equal(-1.0 / 21.0, mb[1, 2], 3 * DoubleConstants.DBL_EPSILON);
      AssertEx.Equal(5.0 / 14.0, mb[2, 0], 3 * DoubleConstants.DBL_EPSILON);
      AssertEx.Equal(-1.0 / 6.0, mb[2, 1], 3 * DoubleConstants.DBL_EPSILON);
      AssertEx.Equal(-1.0 / 21.0, mb[2, 2], 3 * DoubleConstants.DBL_EPSILON);
    }

    [Fact]
    public void PseudoInverse3x3RankNearly2Test()
    {
      var ma = CreateMatrix.Dense<double>(3, 3);
      {
        ma[0, 0] = 1 + RMath.Pow(2, -40);
        ma[0, 1] = 2;
        ma[0, 2] = 4;
        ma[1, 0] = 1;
        ma[1, 1] = 4;
        ma[1, 2] = 2;
        ma[2, 0] = 1;
        ma[2, 1] = 2;
        ma[2, 2] = 4;
      };

      IMatrix<double> mb = MatrixMath.PseudoInverse(ma);

      Assert.Equal(3, mb.RowCount);
      Assert.Equal(3, mb.ColumnCount);

      AssertEx.Equal(1099511627776, mb[0, 0], 3E-4 * 1099511627776);
      AssertEx.Equal(0, mb[0, 1], 2E-4);
      AssertEx.Equal(-1099511627776, mb[0, 2], 3E-4 * 1099511627776);
      AssertEx.Equal(-549755813888 / 3.0, mb[1, 0], 3E-4 * 1099511627776);
      AssertEx.Equal(1 / 3.0, mb[1, 1], 2E-4);
      AssertEx.Equal(366503875925 / 2.0, mb[1, 2], 3E-4 * 1099511627776);
      AssertEx.Equal(-549755813888 / 3.0, mb[2, 0], 3E-4 * 1099511627776);
      AssertEx.Equal(-1.0 / 6.0, mb[2, 1], 2E-4);
      AssertEx.Equal(183251937963, mb[2, 2], 3E-4 * 1099511627776);
    }

    [Fact]
    public void PseudoInverse3x3Rank2Test()
    {
      var ma = CreateMatrix.Dense<double>(3, 3);
      {
        ma[0, 0] = 1;
        ma[0, 1] = 2;
        ma[0, 2] = 4;
        ma[1, 0] = 1;
        ma[1, 1] = 4;
        ma[1, 2] = 2;
        ma[2, 0] = 1;
        ma[2, 1] = 2;
        ma[2, 2] = 4;
      };

      IMatrix<double> mb = MatrixMath.PseudoInverse(ma);

      Assert.Equal(3, mb.RowCount);
      Assert.Equal(3, mb.ColumnCount);

      AssertEx.Equal(1.0 / 76.0, mb[0, 0], 3 * DoubleConstants.DBL_EPSILON);
      AssertEx.Equal(1.0 / 38.0, mb[0, 1], 3 * DoubleConstants.DBL_EPSILON);
      AssertEx.Equal(1.0 / 76.0, mb[0, 2], 3 * DoubleConstants.DBL_EPSILON);
      AssertEx.Equal(-13.0 / 152.0, mb[1, 0], 3 * DoubleConstants.DBL_EPSILON);
      AssertEx.Equal(25.0 / 76.0, mb[1, 1], 3 * DoubleConstants.DBL_EPSILON);
      AssertEx.Equal(-13.0 / 152.0, mb[1, 2], 3 * DoubleConstants.DBL_EPSILON);
      AssertEx.Equal(25.0 / 152.0, mb[2, 0], 3 * DoubleConstants.DBL_EPSILON);
      AssertEx.Equal(-13.0 / 76.0, mb[2, 1], 3 * DoubleConstants.DBL_EPSILON);
      AssertEx.Equal(25.0 / 152.0, mb[2, 2], 3 * DoubleConstants.DBL_EPSILON);
    }

    [Fact]
    public void PseudoInverse3x3Rank1Test()
    {
      var ma = CreateMatrix.Dense<double>(3, 3);
      {
        ma[0, 0] = 1;
        ma[0, 1] = 2;
        ma[0, 2] = 4;
        ma[1, 0] = 1;
        ma[1, 1] = 2;
        ma[1, 2] = 4;
        ma[2, 0] = 1;
        ma[2, 1] = 2;
        ma[2, 2] = 4;
      };

      IMatrix<double> mb = MatrixMath.PseudoInverse(ma);

      Assert.Equal(3, mb.RowCount);
      Assert.Equal(3, mb.ColumnCount);

      AssertEx.Equal(1.0 / 63.0, mb[0, 0], 3 * DoubleConstants.DBL_EPSILON);
      AssertEx.Equal(1.0 / 63.0, mb[0, 1], 3 * DoubleConstants.DBL_EPSILON);
      AssertEx.Equal(1.0 / 63.0, mb[0, 2], 3 * DoubleConstants.DBL_EPSILON);
      AssertEx.Equal(2.0 / 63.0, mb[1, 0], 3 * DoubleConstants.DBL_EPSILON);
      AssertEx.Equal(2.0 / 63.0, mb[1, 1], 3 * DoubleConstants.DBL_EPSILON);
      AssertEx.Equal(2.0 / 63.0, mb[1, 2], 3 * DoubleConstants.DBL_EPSILON);
      AssertEx.Equal(4.0 / 63.0, mb[2, 0], 3 * DoubleConstants.DBL_EPSILON);
      AssertEx.Equal(4.0 / 63.0, mb[2, 1], 3 * DoubleConstants.DBL_EPSILON);
      AssertEx.Equal(4.0 / 63.0, mb[2, 2], 3 * DoubleConstants.DBL_EPSILON);
    }
  }
}
