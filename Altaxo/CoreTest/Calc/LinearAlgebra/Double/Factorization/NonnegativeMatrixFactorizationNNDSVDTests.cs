#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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
using Xunit;

namespace Altaxo.Calc.LinearAlgebra.Double.Factorization
{
  public class NonnegativeMatrixFactorizationNNDSVDTests : FactorizationTestBase
  {
    protected static void SystemDiagnosticsDebugWriteMatrix(Matrix<double> matrix, string name)
    {
      System.Diagnostics.Debug.WriteLine($"var {name} = CreateMatrix.DenseOfRowArrays<double>(");
      for (int i = 0; i < matrix.RowCount; i++)
      {
        System.Diagnostics.Debug.Write("[");
        for (int j = 0; j < matrix.ColumnCount; j++)
        {
          System.Diagnostics.Debug.Write(FormattableString.Invariant($"{matrix[i, j]}"));
          System.Diagnostics.Debug.Write(((j + 1) == matrix.ColumnCount ? "]" : ", "));
        }
        System.Diagnostics.Debug.WriteLine((i + 1) == matrix.RowCount ? "\r\n);" : ",");
      }
    }

    protected static void AssertAreEqual(Matrix<double> expected, Matrix<double> actual, double absError, double relError)
    {
      Assert.Equal(expected.RowCount, actual.RowCount);
      Assert.Equal(expected.ColumnCount, actual.ColumnCount);

      for (int i = 0; i < expected.RowCount; ++i)
      {
        for (int j = 0; j < expected.ColumnCount; ++j)
        {
          AssertEx.AreEqual(expected[i, j], actual[i, j], absError, relError);
        }
      }
    }

    /// <summary>
    /// Verifies that the NNDSVD algorithm with possible zeros in the result produces the expected factor and load
    /// matrices for a specific 4x3 test matrix and rank 2 decomposition.
    /// </summary>
    /// <remarks>This test ensures that the implementation of NNDSVDWithZerosPossibleInResultV02 returns
    /// consistent and correct results for a known input. The assertions compare the computed factors and loads to
    /// precomputed expected values within a specified numerical tolerance.
    /// Cross checked with https://github.com/trigeorgis/Deep-Semi-NMF/blob/master/matlab/NNDSVD.m using Octave 10.3 but without clamping the zeros to 0.1.</remarks>
    [Fact]
    public void TestNNDSVD()
    {
      var matrixX = CreateMatrix.DenseOfRowArrays<double>(
        [4, 7, 4],
        [10, 8, 8],
        [5, 3, 4],
        [5, 4, 5]
        );

      var expectedFactors = CreateMatrix.DenseOfRowArrays<double>(
        [1.905875503193971, 1.4680037385563836],
        [3.3405381261011238, 0],
        [1.5454323137188755, 0],
        [1.7893698196669536, 0]
        );

      var expectedLoads = CreateMatrix.DenseOfRowArrays<double>(
        [2.8311043238329074, 2.5443928879447544, 2.4274832507727604],
        [0, 1.4680037385563836, 0]
        );

      var method = new NNDSVD();

      var (mfactors, mloads) = method.GetInitialFactors(matrixX, 2);
      // SystemDiagnosticsDebugWriteMatrix(mfactors, "expectedFactors");
      // SystemDiagnosticsDebugWriteMatrix(mloads, "expectedLoads");

      AssertAreEqual(expectedFactors, mfactors, 0, 1E-12);
      AssertAreEqual(expectedLoads, mloads, 0, 1E-12);

      // Test also, if NNDSVD is independent of the scaling
      var scaling = 1E+20;
      (mfactors, mloads) = new NNDSVD().GetInitialFactors(matrixX * (scaling * scaling), 2);
      AssertAreEqual(expectedFactors * scaling, mfactors, 0, 1E-12);
      AssertAreEqual(expectedLoads * scaling, mloads, 0, 1E-12);

      /*
      scaling = 1E-20;
      (mfactors, mloads) = NonnegativeMatrixFactorizationBase.NNDSVDWithZerosPossibleInResultV02(matrixX * (scaling * scaling), 2);
      AssertAreEqual(expectedFactors * scaling, mfactors, 0, 1E-12);
      AssertAreEqual(expectedLoads * scaling, mloads, 0, 1E-12);
      */
    }
  }
}
