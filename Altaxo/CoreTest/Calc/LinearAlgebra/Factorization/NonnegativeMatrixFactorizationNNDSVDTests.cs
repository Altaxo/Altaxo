using Xunit;

namespace Altaxo.Calc.LinearAlgebra.Factorization
{
  public class NonnegativeMatrixFactorizationNNDSVDTests : FactorizationTestBase
  {
    [Fact]
    public void TestNNDSVD()
    {
      var originalLoadings = GetThreeSpectra();
      var originalScores = GetScores3D(NumberOfSpectra);
      var originalMatrix = originalScores * originalLoadings;
      var matrixX = originalMatrix.Clone();

      var (mfactors, mloads) = NonnegativeMatrixFactorizationBase.NNDSVDWithZerosPossibleInResultV02(matrixX, 3);

      for (int i = 0; i < mfactors.RowCount; i++)
      {
        for (int j = 0; j < mfactors.ColumnCount; j++)
        {
          Assert.True(mfactors[i, j] >= 0.0);
        }
      }
      for (int i = 0; i < mloads.RowCount; i++)
      {
        for (int j = 0; j < mloads.ColumnCount; j++)
        {
          Assert.True(mloads[i, j] >= 0.0);
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
    public void TestNNDSVD_2()
    {
      var matrixX = GetTestMatrixNN4x3();
      var (mfactors, mloads) = NonnegativeMatrixFactorizationBase.NNDSVDWithZerosPossibleInResultV02(matrixX, 2);

      /*
      for (int i = 0; i < mfactors.RowCount; i++)
      {
        for (int j = 0; j < mfactors.ColumnCount; j++)
        {
          System.Diagnostics.Debug.WriteLine(FormattableString.Invariant($"AssertEx.AreEqual({mfactors[i, j]}, mfactors[{i},{j}],1E-12, 1E-12);"));
        }
      }
      for (int i = 0; i < mloads.RowCount; i++)
      {
        for (int j = 0; j < mloads.ColumnCount; j++)
        {
          System.Diagnostics.Debug.WriteLine(FormattableString.Invariant($"AssertEx.AreEqual({mloads[i, j]}, mloads[{i},{j}],1E-12, 1E-12);"));
        }
      }
      */

      AssertEx.AreEqual(1.905875503193971, mfactors[0, 0], 1E-12, 1E-12);
      AssertEx.AreEqual(1.4680037385563836, mfactors[0, 1], 1E-12, 1E-12);
      AssertEx.AreEqual(3.3405381261011238, mfactors[1, 0], 1E-12, 1E-12);
      AssertEx.AreEqual(0, mfactors[1, 1], 1E-12, 1E-12);
      AssertEx.AreEqual(1.5454323137188755, mfactors[2, 0], 1E-12, 1E-12);
      AssertEx.AreEqual(0, mfactors[2, 1], 1E-12, 1E-12);
      AssertEx.AreEqual(1.7893698196669536, mfactors[3, 0], 1E-12, 1E-12);
      AssertEx.AreEqual(0, mfactors[3, 1], 1E-12, 1E-12);
      AssertEx.AreEqual(2.8311043238329074, mloads[0, 0], 1E-12, 1E-12);
      AssertEx.AreEqual(2.5443928879447544, mloads[0, 1], 1E-12, 1E-12);
      AssertEx.AreEqual(2.4274832507727604, mloads[0, 2], 1E-12, 1E-12);
      AssertEx.AreEqual(0, mloads[1, 0], 1E-12, 1E-12);
      AssertEx.AreEqual(1.4680037385563836, mloads[1, 1], 1E-12, 1E-12);
      AssertEx.AreEqual(0, mloads[1, 2], 1E-12, 1E-12);

    }
  }
}
