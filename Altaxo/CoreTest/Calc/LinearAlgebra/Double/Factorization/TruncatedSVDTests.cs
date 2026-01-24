#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
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
using Xunit;

namespace Altaxo.Calc.LinearAlgebra.Double.Factorization
{
  // Test matrices see https://resources.wolframcloud.com/FunctionRepository/resources/NonNegativeMatrixFactorization/
  // https://github.com/trigeorgis/Deep-Semi-NMF/blob/master/matlab/NNDSVD.m

  public class TruncatedSVDTests : FactorizationTestBase
  {
    [Fact]
    public void TestRandomizedSvd()
    {
      var originalLoadings = GetThreeSpectra();
      var originalScores = GetScores3D(NumberOfSpectra);
      var originalMatrix = originalScores * originalLoadings;
      var matrixX = originalMatrix.Clone();
      var (mfactors, S, mloads) = TruncatedSVD.RandomizedSvd(matrixX, 3);
      mfactors = mfactors.Multiply(CreateMatrix.DenseDiagonal<double>(S.Count, S.Count, i => S[i]));
      var relError = RelativeError(mfactors, mloads, originalMatrix);
      Assert.True(relError < 1E-13);
    }

    [Fact]
    public void CanRandomizedSvdHandleDifferentScales()
    {
      foreach (var scale in new double[] { 1, 1E10, 1E20, 1E30, 1E40, 1E-10, 1E-20, 1E-30, 1E-40 })
      {
        var originalLoadings = GetThreeSpectra();
        var originalScores = GetScores3D(NumberOfSpectra);
        var originalMatrix = originalScores * originalLoadings * scale;
        var matrixX = originalMatrix.Clone();
        var (mfactors, S, mloads) = TruncatedSVD.RandomizedSvd(matrixX, 3);
        mfactors = mfactors.Multiply(CreateMatrix.DenseDiagonal<double>(S.Count, S.Count, i => S[i]));
        var relError = RelativeError(mfactors, mloads, originalMatrix);
        Assert.True(relError < 1E-13);
      }
    }

    [Fact]
    public void TestBlockKrylovSvd()
    {
      var originalLoadings = GetThreeSpectra();
      var originalScores = GetScores3D(NumberOfSpectra);
      var originalMatrix = originalScores * originalLoadings;
      var matrixX = originalMatrix.Clone();
      var (mfactors, S, mloads) = TruncatedSVD.BlockKrylovSvd(matrixX, 3);
      mfactors = mfactors.Multiply(CreateMatrix.DenseDiagonal<double>(S.Count, S.Count, i => S[i]));
      var relError = RelativeError(mfactors, mloads, originalMatrix);
      Assert.True(relError < 2E-15);
    }

    [Fact]
    public void CanBlockKrylovSvdHandleDifferentScales()
    {
      foreach (var scale in new double[] { 1, 1E10, 1E20, 1E30, 1E40, 1E-10, 1E-20, 1E-30, 1E-40 })
      {
        var originalLoadings = GetThreeSpectra();
        var originalScores = GetScores3D(NumberOfSpectra);
        var originalMatrix = originalScores * originalLoadings * scale;
        var matrixX = originalMatrix.Clone();
        var (mfactors, S, mloads) = TruncatedSVD.BlockKrylovSvd(matrixX, 3, 2, 2);
        mfactors = mfactors.Multiply(CreateMatrix.DenseDiagonal<double>(S.Count, S.Count, i => S[i]));
        var relError = RelativeError(mfactors, mloads, originalMatrix);
        Assert.True(relError < 2E-15);
      }
    }

    [Fact]
    public void CompareBlockKrylovSvdWithFullSvd()
    {
      var originalLoadings = GetThreeSpectra();
      var originalScores = GetScores3D(NumberOfSpectra);
      var originalMatrix = originalScores * originalLoadings;
      var matrixX = originalMatrix.Clone();
      var (mfactors, S, mloads) = TruncatedSVD.BlockKrylovSvd(matrixX, 2);
      var fullSvd = matrixX.Svd();

      AssertEx.AreEqual(fullSvd.S[0], S[0], 0, 1E-14);
      AssertEx.AreEqual(fullSvd.S[1], S[1], 0, 1E-14);
    }
  }
}

