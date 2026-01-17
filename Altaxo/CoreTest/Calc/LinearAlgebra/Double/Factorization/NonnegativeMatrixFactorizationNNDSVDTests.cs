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


    protected static void AssertAreNonnegative(Matrix<double> actual)
    {
      for (int i = 0; i < actual.RowCount; ++i)
      {
        for (int j = 0; j < actual.ColumnCount; ++j)
        {
          Assert.True(actual[i, j] >= 0);
        }
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

    protected static void AssertAreEqualIfExpectedIsNonZero(Matrix<double> expected, Matrix<double> actual, double absError, double relError)
    {
      Assert.Equal(expected.RowCount, actual.RowCount);
      Assert.Equal(expected.ColumnCount, actual.ColumnCount);

      for (int i = 0; i < expected.RowCount; ++i)
      {
        for (int j = 0; j < expected.ColumnCount; ++j)
        {
          if (expected[i, j] != 0)
          {
            AssertEx.AreEqual(expected[i, j], actual[i, j], absError, relError);
          }
        }
      }
    }

    protected static void AssertActualIsLessOrEqualIfPatternIsZero(Matrix<double> pattern, Matrix<double> expected, Matrix<double> actual)
    {
      Assert.Equal(expected.RowCount, actual.RowCount);
      Assert.Equal(expected.ColumnCount, actual.ColumnCount);

      for (int i = 0; i < expected.RowCount; ++i)
      {
        for (int j = 0; j < expected.ColumnCount; ++j)
        {
          if (pattern[i, j] == 0)
          {
            Assert.True(actual[i, j] < expected[i, j]);
          }
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
      foreach (double scaling in new double[] { 1, 1E10 })
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

        var (mfactors, mloads) = method.GetInitialFactors(matrixX * (scaling * scaling), 2);
        // SystemDiagnosticsDebugWriteMatrix(mfactors, "expectedFactors");
        // SystemDiagnosticsDebugWriteMatrix(mloads, "expectedLoads");

        AssertAreNonnegative(mfactors);
        AssertAreNonnegative(mloads);
        AssertAreEqual(expectedFactors * scaling, mfactors, 0, 1E-12);
        AssertAreEqual(expectedLoads * scaling, mloads, 0, 1E-12);
      }
    }

    /// <summary>
    /// Tests that the NNDSVDa algorithm produces the expected factor and load.
    /// </summary>
    [Fact]
    public void TestNNDSVDa()
    {
      foreach (double scaling in new double[] { 1, 1E10 })
      {
        var matrixX = CreateMatrix.DenseOfRowArrays<double>(
        [4, 7, 4],
        [10, 8, 8],
        [5, 3, 4],
        [5, 4, 5]
        );

        var expectedFactors = CreateMatrix.DenseOfRowArrays<double>(
          [1.905875503193971, 1.4680037385563836],
          [3.3405381261011238, 1.2561524376546633],
          [1.5454323137188755, 1.2561524376546633],
          [1.7893698196669536, 1.2561524376546633]
          );
        var expectedLoads = CreateMatrix.DenseOfRowArrays<double>(
         [2.8311043238329074, 2.5443928879447544, 2.4274832507727604],
         [1.545164033517801, 1.4680037385563836, 1.545164033517801]
         );

        var method = new NNDSVDa();

        var (mfactors, mloads) = method.GetInitialFactors(matrixX * (scaling * scaling), 2);
        SystemDiagnosticsDebugWriteMatrix(mfactors, "expectedFactors");
        SystemDiagnosticsDebugWriteMatrix(mloads, "expectedLoads");

        AssertAreNonnegative(mfactors);
        AssertAreNonnegative(mloads);
        AssertAreEqual(expectedFactors * scaling, mfactors, 0, 1E-12);
        AssertAreEqual(expectedLoads * scaling, mloads, 0, 1E-12);
      }
    }

    /// <summary>
    /// Verifies that the NNDSVDar algorithm with possible zeros in the result produces the expected factor and load.
    /// </summary>
    [Fact]
    public void TestNNDSVDar()
    {
      foreach (double scaling in new double[] { 1, 1E10 })
      {
        var matrixX = CreateMatrix.DenseOfRowArrays<double>(
          [4, 7, 4],
          [10, 8, 8],
          [5, 3, 4],
          [5, 4, 5]
          );
        var matrixXScaled = matrixX * (scaling * scaling);
        var (mfactorsN, mloadsN) = new NNDSVD().GetInitialFactors(matrixXScaled, 2);
        var (mfactorsA, mloadsA) = new NNDSVDa().GetInitialFactors(matrixXScaled, 2);
        var (mfactors, mloads) = new NNDSVDar().GetInitialFactors(matrixXScaled, 2);

        AssertAreNonnegative(mfactors);
        AssertAreNonnegative(mloads);
        AssertAreEqualIfExpectedIsNonZero(mfactorsN, mfactors, 0, 0); // all non-zero entries must exactly be equal to NNDSVD
        AssertActualIsLessOrEqualIfPatternIsZero(mfactorsN, mfactorsA, mfactors); // all zero entries must be less or equal than in NNDSVDa
      }
    }

    [Fact]
    public void TestSVD()
    {
      foreach (double scaling in new double[] { 1, 1E40, 1E-40 })
      {
        var matrixX = CreateMatrix.DenseOfRowArrays<double>(
          [4, 7, 4],
          [10, 8, 8],
          [5, 3, 4],
          [5, 4, 5]
          );

        /* Values computed with Mathematica in this way:
        *  matrix = {{4, 7, 4}, {10, 8, 8}, {5, 3, 4}, {5, 4, 5}}
        *  {u, w, v} = SingularValueDecomposition[matrix];
        *  N[u, 20]
        *  N[w, 20]
        *  N[Transpose[v], 20]
        */

        var expectedU = CreateMatrix.DenseOfRowArrays<double>(
          [0.42215668141346500811, -0.87675136383986560620, 0.047995632277482876247, -0.22536015980024164867],
          [0.73993840997833451205, 0.22051356023428738286, -0.46959889408365937068, 0.42818430362045913247],
          [0.34231752064357875227, 0.38153678983509828442, -0.062299290485614282346, -0.85636860724091826495],
          [0.39635035112528586728, 0.19264084136045230895, 0.87937028398887675954, 0.18028812784019331894]
          );

        var expectedW = CreateMatrix.DenseOfRowArrays<double>(
          [20.381761793431507357, 0, 0],
          [0, 3.0100938773437216581, 0],
          [0, 0, 0.72327107324536682962],
          [0, 0, 0]
          );


        var expectedVT = CreateMatrix.DenseOfRowArrays<double>(
          [0.62709741747647217739, 0.56359004351894387921, 0.53769423638407554691],
          [0.52125228212010301763, -0.81657829629776043208, 0.24798375833919346295],
          [-0.57883062063001098569, -0.12476437336740771587, 0.80584673714008074090]
          );



        var matrixXScaled = matrixX * scaling;
        var result = matrixXScaled.Svd(computeVectors: true);
        SystemDiagnosticsDebugWriteMatrix(result.U, "U");
        SystemDiagnosticsDebugWriteMatrix(result.W, "W");
        SystemDiagnosticsDebugWriteMatrix(result.VT, "VT");

        var reconstructedX = result.U * result.W * result.VT;
        AssertAreEqual(matrixXScaled, reconstructedX, 0, 1E-14);

        var signU = new DiagonalMatrix(4); // The sign of the columns for SVD is not unique, thus we need to adopt it
        for (int i = 0; i < expectedU.ColumnCount; ++i)
          signU[i, i] = expectedU[0, i] * result.U[0, i] < 0 ? -1 : 1;
        AssertAreEqual(expectedU * signU, result.U, 0, 1E-14); // U should be independent of scaling

        AssertAreEqual(expectedW * scaling, result.W, 0, 1E-14); // W should be scaled by scaling

        var signVT = new DiagonalMatrix(3); // The sign of the columns for SVD is not unique, thus we need to adopt it
        for (int i = 0; i < expectedVT.ColumnCount; ++i)
          signVT[i, i] = expectedVT[0, i] * result.VT[0, i] < 0 ? -1 : 1;
        AssertAreEqual(expectedVT * signVT, result.VT, 0, 1E-14); // VT should be independent of scaling


      }
    }
  }
}
