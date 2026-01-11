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

using Xunit;

namespace Altaxo.Calc.LinearAlgebra.Factorization
{
  public class PrincipalComponentAnalysisTests : FactorizationTestBase
  {
    [Fact]
    public void Test_NIPALS_HO()
    {
      var originalLoadings = GetThreeSpectra();
      var originalScores = GetScores3D(NumberOfSpectra);
      var originalMatrix = originalScores * originalLoadings;
      var matrixX = originalMatrix.Clone();

      var factors = new MatrixMath.TopSpineJaggedArrayMatrix<double>(0, 0);
      var loads = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(0, 0);
      var residualVariances = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(0, 0);
      var meanX = new MatrixMath.MatrixWithOneRow<double>(matrixX.ColumnCount);
      // first, center the matrix
      MatrixMath.ColumnsToZeroMean(matrixX, meanX);

      var originalMatrix2 = matrixX.Clone();

      MatrixMath.NIPALS_HO(matrixX, 3, 1E-9, factors, loads, residualVariances);

      var mfactors = CreateMatrix.Dense<double>(factors.RowCount, factors.ColumnCount);
      var mloads = CreateMatrix.Dense<double>(loads.RowCount, loads.ColumnCount);
      MatrixMath.Copy(factors, mfactors);
      MatrixMath.Copy(loads, mloads);

      var relError = RelativeError(mfactors, mloads, originalMatrix2);
      Assert.True(relError < 1E-15);
    }

    [Fact]
    public void Test_SVD_HO()
    {
      var originalLoadings = GetThreeSpectra();
      var originalScores = GetScores3D(NumberOfSpectra);
      var originalMatrix = originalScores * originalLoadings;
      var matrixX = originalMatrix.Clone();

      var meanX = new MatrixMath.MatrixWithOneRow<double>(matrixX.ColumnCount);
      // first, center the matrix
      //MatrixMath.ColumnsToZeroMean(matrixX, meanX);

      var originalMatrix2 = matrixX.Clone();

      var svd = matrixX.Svd();

      var mfactors = CreateMatrix.Dense<double>(NumberOfSpectra, NumberOfComponents);
      var mloads = CreateMatrix.Dense<double>(NumberOfComponents, SpectralPoints);

      for (int i = 0; i < NumberOfComponents; i++)
      {
        var singularValue = svd.S[i];
        for (int r = 0; r < NumberOfSpectra; r++)
        {
          mfactors[r, i] = svd.U[r, i] * singularValue;
        }
        for (int c = 0; c < SpectralPoints; c++)
        {
          mloads[i, c] = svd.VT[i, c];
        }
      }

      var relError = RelativeError(mfactors, mloads, originalMatrix2);
      Assert.True(relError < 1E-15);
    }

    [Fact]
    public void Test_SVD_HO_1()
    {
      var originalLoadings = GetThreeSpectra();
      var originalScores = GetScores3D(NumberOfSpectra);
      var originalMatrix = originalScores * originalLoadings;
      var matrixX = originalMatrix.Clone();

      var meanX = new MatrixMath.MatrixWithOneRow<double>(matrixX.ColumnCount);
      // first, center the matrix
      // MatrixMath.ColumnsToZeroMean(matrixX, meanX);

      var originalMatrix2 = matrixX.Clone();

      var (mfactors, mloadings) = PrincipalComponentAnalysis.BySVD(matrixX, NumberOfComponents);

      var relError = RelativeError(mfactors, mloadings, originalMatrix2);
      Assert.True(relError < 1E-15);
    }
  }
}
