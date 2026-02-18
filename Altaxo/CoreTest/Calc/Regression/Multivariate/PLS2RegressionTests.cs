using System;
using System.Linq;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Science.Spectroscopy;
using Altaxo.Science.Spectroscopy.EnsembleProcessing;
using Xunit;

namespace Altaxo.Calc.Regression.Multivariate
{
  public class PLS2RegressionTests
  {
    [Fact]
    public void TestPLS2Regression()
    {
      int numberOfSpectra = 10;
      int numberOfSpectralPoints = 100;
      int numberOfTargetVariables = 1;

      var sx = Enumerable.Range(0, numberOfSpectra).Select(x => (double)x).ToArray(); // spectral x values
      var sv = CreateMatrix.Dense<double>(numberOfSpectra, numberOfSpectralPoints); // spectral variables
      var tv = CreateMatrix.Dense<double>(numberOfSpectra, numberOfTargetVariables); // target variables

      var svMean = CreateVector.Dense<double>(numberOfSpectralPoints);
      var tvMean = CreateVector.Dense<double>(numberOfTargetVariables);

      for (int i = 0; i < numberOfSpectra; i++)
      {
        var t = i + 11;
        tv[i, 0] = t;

        for (int j = 0; j < numberOfSpectralPoints; j++)
        {
          sv[i, j] = t * Math.Exp(-0.5 * RMath.Pow2((j - numberOfSpectralPoints / 2d) / (numberOfSpectralPoints / 8d)));
        }
      }

      MatrixMath.ColumnsToZeroMean(sv, svMean);
      MatrixMath.ColumnsToZeroMean(tv, tvMean);

      var mvm = new PLS2Regression();
      mvm.AnalyzeFromPreprocessed(sv, tv, 3);
      Assert.Equal(3, mvm.NumberOfFactors);
      Assert.Equal(4, mvm.PRESS.Count);
      AssertEx.AreEqual(82.5, mvm.PRESS[0], 0, 1E-14);
      AssertEx.Greater(1E-29, mvm.PRESS[1]);
      AssertEx.Greater(1E-29, mvm.PRESS[2]);
      AssertEx.Greater(1E-29, mvm.PRESS[3]);
    }

    [Fact]
    public void TestPLS2Regression_Octane()
    {
      var (sx, sv, tv) = MultivariateRegressionTestData.GetOctaneData();

      var svMean = CreateVector.Dense<double>(sv.ColumnCount);
      var tvMean = CreateVector.Dense<double>(tv.ColumnCount);

      MatrixMath.ColumnsToZeroMean(sv, svMean);
      MatrixMath.ColumnsToZeroMean(tv, tvMean);

      var mvm = new PLS2Regression();
      mvm.AnalyzeFromPreprocessed(sv, tv, 4);
      Assert.Equal(4, mvm.NumberOfFactors);
      Assert.Equal(5, mvm.PRESS.Count);
      AssertEx.Greater(138.128, mvm.PRESS[0]);
      AssertEx.Greater(94.060, mvm.PRESS[1]);
      AssertEx.Greater(7.373, mvm.PRESS[2]);
      AssertEx.Greater(3.169, mvm.PRESS[3]);
      AssertEx.Greater(2.750, mvm.PRESS[4]);

      {
        // now test the prediction of the target variable from the spectral variables
        // 1) we use the full preprocessed spectral variables and the full preprocessed target variable
        var predictedTv = CreateMatrix.Dense<double>(tv.RowCount, tv.ColumnCount);
        mvm.PredictYFromPreprocessed(sv, 3, predictedTv);
        var rss = (predictedTv - tv).FrobeniusNorm();
        rss *= rss;
        AssertEx.Greater(3.169, rss);
        AssertEx.Less(2.750, rss);
      }

      {
        // now test the prediction of the target variable from the spectral variables
        // 2) we use each spectrum individually and predict the target variable for this spectrum

        var svRow = CreateMatrix.Dense<double>(1, sv.ColumnCount);
        var predictedTv = CreateMatrix.Dense<double>(1, 1);

        double rss = 0;
        for (int i = 0; i < tv.RowCount; i++)
        {
          svRow.SetRow(0, sv.Row(i));
          mvm.PredictYFromPreprocessed(svRow, 3, predictedTv);
          rss += RMath.Pow2(predictedTv[0, 0] - tv[i, 0]);
        }
        AssertEx.Greater(3.169, rss);
        AssertEx.Less(2.750, rss);
      }

    }

    [Fact]
    public void TestPLS2Regression_Octane_CrossValiation()
    {
      (ICrossValidationGroupingStrategy strategy, double expectedSpectraPerGroup, double[] expectedCrossPRESS)[] testCases = new (ICrossValidationGroupingStrategy, double, double[])[]
      {
        (new CrossValidationGroupingStrategyExcludeSingleMeasurements(), 1, new double[] {142.850, 105.842, 8.724, 3.991, 3.490}),
        (new CrossValidationGroupingStrategyExcludeGroupsOfSimilarMeasurements(),  1.4285714285714286, new double[] {146.029, 107.369, 9.023, 4.069, 3.600})
      };

      foreach (var testCase in testCases)
      {

        var (sx, sv, tv) = MultivariateRegressionTestData.GetOctaneData();

        var preprocessor = new SpectralPreprocessingOptionsList(new EnsembleMean());

        var numberOfSpectraPerGroup = PLS2Regression.GetCrossPRESS(sx, sv, tv, 4,
          testCase.strategy,
          preprocessor,
          new PLS2Regression(),
          out var crossPRESS);

        AssertEx.AreEqual(testCase.expectedSpectraPerGroup, numberOfSpectraPerGroup, 0, 1E-10);
        Assert.Equal(5, crossPRESS.Count);

        AssertEx.Greater(testCase.expectedCrossPRESS[0], crossPRESS[0]);
        AssertEx.Greater(testCase.expectedCrossPRESS[1], crossPRESS[1]);
        AssertEx.Greater(testCase.expectedCrossPRESS[2], crossPRESS[2]);
        AssertEx.Greater(testCase.expectedCrossPRESS[3], crossPRESS[3]);
        AssertEx.Greater(testCase.expectedCrossPRESS[4], crossPRESS[4]);

        var tvCrossPredicted = CreateMatrix.Dense<double>(tv.RowCount, tv.ColumnCount);
        PLS2Regression.GetCrossYPredicted(sx, sv, tv, 4,
          testCase.strategy,
          preprocessor,
          new PLS2Regression(),
          tvCrossPredicted);

        var rss = (tvCrossPredicted - tv).FrobeniusNorm();
        rss *= rss;
        AssertEx.Greater(testCase.expectedCrossPRESS[4], rss);

        // now shuffle the data by spectrum
        for (int trial = 0; trial < 10; ++trial)
        {
          var idx = Enumerable.Range(0, sv.RowCount).ToArray();
          Shuffle(idx);

          var svs = CreateMatrix.Dense<double>(sv.RowCount, sv.ColumnCount);
          var tvs = CreateMatrix.Dense<double>(tv.RowCount, tv.ColumnCount);

          for (int i = 0; i < sv.RowCount; ++i)
          {
            svs.SetRow(idx[i], sv.Row(i));
            tvs.SetRow(idx[i], tv.Row(i));
          }

          numberOfSpectraPerGroup = PLS2Regression.GetCrossPRESS(sx, svs, tvs, 4,
          testCase.strategy,
          preprocessor,
          new PLS2Regression(),
          out crossPRESS);

          AssertEx.AreEqual(testCase.expectedSpectraPerGroup, numberOfSpectraPerGroup, 0, 1E-10);
          Assert.Equal(5, crossPRESS.Count);

          AssertEx.Greater(testCase.expectedCrossPRESS[0], crossPRESS[0]);
          AssertEx.Greater(testCase.expectedCrossPRESS[1], crossPRESS[1]);
          AssertEx.Greater(testCase.expectedCrossPRESS[2], crossPRESS[2]);
          AssertEx.Greater(testCase.expectedCrossPRESS[3], crossPRESS[3]);
          AssertEx.Greater(testCase.expectedCrossPRESS[4], crossPRESS[4]);

          var tvCR = CreateMatrix.Dense<double>(tv.RowCount, tv.ColumnCount);
          PLS2Regression.GetCrossYPredicted(sx, svs, tvs, 4,
            testCase.strategy,
            preprocessor,
            new PLS2Regression(),
            tvCR);

          rss = (tvCR - tvs).FrobeniusNorm();
          rss *= rss;
          AssertEx.Greater(testCase.expectedCrossPRESS[4], rss);
        }

        // now shuffle the data by spectral point
        for (int trial = 0; trial < 10; ++trial)
        {
          var idx = Enumerable.Range(0, sv.ColumnCount).ToArray();
          Shuffle(idx);

          var svs = CreateMatrix.Dense<double>(sv.RowCount, sv.ColumnCount);
          var sxs = (double[])sx.Clone();

          for (int i = 0; i < sv.ColumnCount; ++i)
          {
            sxs[idx[i]] = sx[i];
            svs.SetColumn(idx[i], sv.Column(i));
          }

          numberOfSpectraPerGroup = PLS2Regression.GetCrossPRESS(sxs, svs, tv, 4,
          testCase.strategy,
          preprocessor,
          new PLS2Regression(),
          out crossPRESS);

          AssertEx.AreEqual(testCase.expectedSpectraPerGroup, numberOfSpectraPerGroup, 0, 1E-10);
          Assert.Equal(5, crossPRESS.Count);

          AssertEx.Greater(testCase.expectedCrossPRESS[0], crossPRESS[0]);
          AssertEx.Greater(testCase.expectedCrossPRESS[1], crossPRESS[1]);
          AssertEx.Greater(testCase.expectedCrossPRESS[2], crossPRESS[2]);
          AssertEx.Greater(testCase.expectedCrossPRESS[3], crossPRESS[3]);
          AssertEx.Greater(testCase.expectedCrossPRESS[4], crossPRESS[4]);

          var tvCR = CreateMatrix.Dense<double>(tv.RowCount, tv.ColumnCount);
          PLS2Regression.GetCrossYPredicted(sxs, svs, tv, 4,
            testCase.strategy,
            preprocessor,
            new PLS2Regression(),
            tvCR);

          rss = (tvCR - tv).FrobeniusNorm();
          rss *= rss;
          AssertEx.Greater(testCase.expectedCrossPRESS[4], rss);
        }
      }
    }

    [Fact]
    public void TestPLS2Regression_PBT_CrossValiation()
    {
      (ICrossValidationGroupingStrategy strategy, double expectedSpectraPerGroup, double[] expectedCrossPRESS)[] testCases = new (ICrossValidationGroupingStrategy, double, double[])[]
      {
        (new CrossValidationGroupingStrategyExcludeSingleMeasurements(), 1, new double[] {931.1367039239,295.00880720688883,84.83336785964389,53.563498322277184,36.629246655869274}),
        (new CrossValidationGroupingStrategyExcludeGroupsOfSimilarMeasurements(),  3, new double[] {1074.192,519.6040784496812,141.25057468547823,77.88464688162426,63.03753728241355})
      };

      foreach (var testCase in testCases)
      {

        var (sx, sv, tv) = MultivariateRegressionTestData.GetPBTData();

        var preprocessor = new SpectralPreprocessingOptionsList(new EnsembleMean());

        var numberOfSpectraPerGroup = PLS2Regression.GetCrossPRESS(sx, sv, tv, 4,
          testCase.strategy,
          preprocessor,
          new PLS2Regression(),
          out var crossPRESS);

        AssertEx.AreEqual(testCase.expectedSpectraPerGroup, numberOfSpectraPerGroup, 0, 1E-10);
        Assert.Equal(5, crossPRESS.Count);

        AssertEx.AreEqual(testCase.expectedCrossPRESS[0], crossPRESS[0], 0, 1E-6);
        AssertEx.AreEqual(testCase.expectedCrossPRESS[1], crossPRESS[1], 0, 1E-6);
        AssertEx.AreEqual(testCase.expectedCrossPRESS[2], crossPRESS[2], 0, 1E-6);
        AssertEx.AreEqual(testCase.expectedCrossPRESS[3], crossPRESS[3], 0, 1E-6);
        AssertEx.AreEqual(testCase.expectedCrossPRESS[4], crossPRESS[4], 0, 1E-6);

        var tvCrossPredicted = CreateMatrix.Dense<double>(tv.RowCount, tv.ColumnCount);
        PLS2Regression.GetCrossYPredicted(sx, sv, tv, 4,
          testCase.strategy,
          preprocessor,
          new PLS2Regression(),
          tvCrossPredicted);

        var rss = (tvCrossPredicted - tv).FrobeniusNorm();
        rss *= rss;
        AssertEx.AreEqual(testCase.expectedCrossPRESS[4], rss, 0, 1E-6);

        // now shuffle the data by spectrum
        for (int trial = 0; trial < 10; ++trial)
        {
          var idx = Enumerable.Range(0, sv.RowCount).ToArray();
          Shuffle(idx);

          var svs = CreateMatrix.Dense<double>(sv.RowCount, sv.ColumnCount);
          var tvs = CreateMatrix.Dense<double>(tv.RowCount, tv.ColumnCount);

          for (int i = 0; i < sv.RowCount; ++i)
          {
            svs.SetRow(idx[i], sv.Row(i));
            tvs.SetRow(idx[i], tv.Row(i));
          }

          numberOfSpectraPerGroup = PLS2Regression.GetCrossPRESS(sx, svs, tvs, 4,
          testCase.strategy,
          preprocessor,
          new PLS2Regression(),
          out crossPRESS);

          AssertEx.AreEqual(testCase.expectedSpectraPerGroup, numberOfSpectraPerGroup, 0, 1E-10);
          Assert.Equal(5, crossPRESS.Count);

          AssertEx.AreEqual(testCase.expectedCrossPRESS[0], crossPRESS[0], 0, 1E-6);
          AssertEx.AreEqual(testCase.expectedCrossPRESS[1], crossPRESS[1], 0, 1E-6);
          AssertEx.AreEqual(testCase.expectedCrossPRESS[2], crossPRESS[2], 0, 1E-6);
          AssertEx.AreEqual(testCase.expectedCrossPRESS[3], crossPRESS[3], 0, 1E-6);
          AssertEx.AreEqual(testCase.expectedCrossPRESS[4], crossPRESS[4], 0, 1E-6);

          var tvCR = CreateMatrix.Dense<double>(tv.RowCount, tv.ColumnCount);
          PLS2Regression.GetCrossYPredicted(sx, sv, tv, 4,
            testCase.strategy,
            preprocessor,
            new PLS2Regression(),
            tvCR);

          rss = (tvCR - tv).FrobeniusNorm();
          rss *= rss;
          AssertEx.AreEqual(testCase.expectedCrossPRESS[4], rss, 0, 1E-6);
        }
      }
    }



    public static void Shuffle(int[] array)
    {
      for (int i = array.Length - 1; i > 0; i--)
      {
        int j = System.Random.Shared.Next(i + 1); // random index 0..i
        (array[i], array[j]) = (array[j], array[i]); // swap
      }
    }
  }
}


