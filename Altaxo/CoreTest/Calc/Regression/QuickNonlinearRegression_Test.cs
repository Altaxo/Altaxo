using System;
using System.Threading;
using Altaxo.Calc.FitFunctions.Peaks;
using Altaxo.Calc.Regression.Nonlinear;
using Xunit;

namespace Altaxo.Calc.Regression
{
  /// <summary>
  /// Tests for quick nonlinear regression,
  /// especially focussing on parameter sets in which the parameters differ by several orders of magnitude.
  /// </summary>
  public class QuickNonlinearRegression_Test
  {
    #region NoBounds


    /// <summary>
    /// Create a Gaussian with height 17E20, position 5E-20, width=1.5E-20, and no bounds and then fit it.
    /// </summary>
    [Fact]
    public void TestGauss_QuickClamped_1_NoBounds()
    {
      var xx = new double[10];
      var yy = new double[10];
      for (int i = 0; i < xx.Length; ++i)
      {
        xx[i] = i * 1E-20;
        double arg = (xx[i] - 5E-20) / 1.5E-20;
        yy[i] = 17E20 * Math.Exp(-0.5 * arg * arg);
      }

      var initialGuess = new double[3] { 10E20, 5E-20, 3E-20 };

      var ff = new GaussAmplitude(1, -1);
      var fit = new QuickNonlinearRegression(ff);
      var fitResult = fit.Fit(xx, yy, initialGuess, CancellationToken.None);
      AssertEx.GreaterOrEqual(10, fitResult.Iterations); // it should not take more than 8 iterations
      AssertEx.GreaterOrEqual(1, fitResult.ModelInfoAtMinimum.Value); // Chi2 should be less than 1
      AssertEx.AreEqual(17E20, fitResult.MinimizingPoint[0], 0, 1E-16);
      AssertEx.AreEqual(5E-20, fitResult.MinimizingPoint[1], 0, 1E-16);
      AssertEx.AreEqual(1.5E-20, fitResult.MinimizingPoint[2], 0, 1E-16);
    }

    /// <summary>
    /// Create a Gaussian with height 17E20, position 5E-20, width=1.5E-20, and no bounds and then fit it.
    /// </summary>
    [Fact]
    public void TestGauss_QuickWrapped_1_NoBounds()
    {
      var xx = new double[10];
      var yy = new double[10];
      for (int i = 0; i < xx.Length; ++i)
      {
        xx[i] = i * 1E-20;
        double arg = (xx[i] - 5E-20) / 1.5E-20;
        yy[i] = 17E20 * Math.Exp(-0.5 * arg * arg);
      }

      var initialGuess = new double[3] { 10E20, 5E-20, 3E-20 };

      var ff = new GaussAmplitude(1, -1);
      var fit = new QuickNonlinearRegressionWrappedParameters(ff);
      var fitResult = fit.Fit(xx, yy, initialGuess, CancellationToken.None);
      AssertEx.GreaterOrEqual(12, fitResult.Iterations); // it should not take more than 8 iterations
      AssertEx.GreaterOrEqual(1, fitResult.ModelInfoAtMinimum.Value); // Chi2 should be less than 1
      AssertEx.AreEqual(17E20, fitResult.MinimizingPoint[0], 0, 1E-16);
      AssertEx.AreEqual(5E-20, fitResult.MinimizingPoint[1], 0, 1E-16);
      AssertEx.AreEqual(1.5E-20, fitResult.MinimizingPoint[2], 0, 1E-16);
    }

    /// <summary>
    /// Create a Gaussian with height 17E-20, position 5E20, width=1.5E20, and no bounds and then fit it.
    /// </summary>
    [Fact]
    public void TestGauss_QuickClamped_2_NoBounds()
    {
      var xx = new double[10];
      var yy = new double[10];
      for (int i = 0; i < xx.Length; ++i)
      {
        xx[i] = i * 1E20;
        double arg = (xx[i] - 5E20) / 1.5E20;
        yy[i] = 17E-20 * Math.Exp(-0.5 * arg * arg);
      }

      var initialGuess = new double[3] { 10E-20, 5E20, 3E20 };

      var ff = new GaussAmplitude(1, -1);
      var fit = new QuickNonlinearRegression(ff) { FunctionTolerance = 0, GradientTolerance = 0 };
      var fitResult = fit.Fit(xx, yy, initialGuess, CancellationToken.None);
      AssertEx.GreaterOrEqual(10, fitResult.Iterations); // it should not take more than 8 iterations
      AssertEx.GreaterOrEqual(1, fitResult.ModelInfoAtMinimum.Value); // Chi2 should be less than 1
      AssertEx.AreEqual(17E-20, fitResult.MinimizingPoint[0], 0, 1E-16);
      AssertEx.AreEqual(5E20, fitResult.MinimizingPoint[1], 0, 1E-16);
      AssertEx.AreEqual(1.5E20, fitResult.MinimizingPoint[2], 0, 1E-16);
    }

    /// <summary>
    /// Create a Gaussian with height 17E-20, position 5E20, width=1.5E20, and no bounds and then fit it.
    /// </summary>
    [Fact]
    public void TestGauss_QuickWrapped_2_NoBounds()
    {
      var xx = new double[10];
      var yy = new double[10];
      for (int i = 0; i < xx.Length; ++i)
      {
        xx[i] = i * 1E20;
        double arg = (xx[i] - 5E20) / 1.5E20;
        yy[i] = 17E-20 * Math.Exp(-0.5 * arg * arg);
      }

      var initialGuess = new double[3] { 10E-20, 5E20, 3E20 };

      var ff = new GaussAmplitude(1, -1);
      var fit = new QuickNonlinearRegressionWrappedParameters(ff) { FunctionTolerance = 0, GradientTolerance = 0 };
      var fitResult = fit.Fit(xx, yy, initialGuess, CancellationToken.None);
      AssertEx.GreaterOrEqual(12, fitResult.Iterations); // it should not take more than 8 iterations
      AssertEx.GreaterOrEqual(1, fitResult.ModelInfoAtMinimum.Value); // Chi2 should be less than 1
      AssertEx.AreEqual(17E-20, fitResult.MinimizingPoint[0], 0, 1E-8);
      AssertEx.AreEqual(5E20, fitResult.MinimizingPoint[1], 0, 1E-16);
      AssertEx.AreEqual(1.5E20, fitResult.MinimizingPoint[2], 0, 1E-16);
    }

    #endregion

    #region Lower bound

    /// <summary>
    /// Create a Gaussian with width=1.5E-20, but limit the fit width to 2E-20.
    /// </summary>
    [Fact]
    public void TestGauss_QuickClamped_1_LowerBoundWidth()
    {
      var xx = new double[10];
      var yy = new double[10];
      for (int i = 0; i < xx.Length; ++i)
      {
        xx[i] = i * 1E-20;
        double arg = (xx[i] - 5E-20) / 1.5E-20;
        yy[i] = 17E20 * Math.Exp(-0.5 * arg * arg);
      }

      var initialGuess = new double[3] { 10E20, 5E-20, 3E-20 };

      var ff = new GaussAmplitude(1, -1);
      var fit = new QuickNonlinearRegression(ff);
      var lowerBounds = new double?[3];
      lowerBounds[2] = 2E-20;
      var fitResult = fit.Fit(xx, yy, initialGuess, lowerBounds, null, null, null, CancellationToken.None);
      AssertEx.GreaterOrEqual(13, fitResult.Iterations); // it should not take more than 13 iterations
      AssertEx.GreaterOrEqual(3.04E41, fitResult.ModelInfoAtMinimum.Value); // Chi2 should be less than 1
      AssertEx.AreEqual(2E-20, fitResult.MinimizingPoint[2], 0, 1E-16);
    }

    /// <summary>
    /// Create a Gaussian with width=1.5E-20, but limit the fit width to 2E-20.
    /// </summary>
    [Fact]
    public void TestGauss_QuickWrapped_1_LowerBoundWidth()
    {
      var xx = new double[10];
      var yy = new double[10];
      for (int i = 0; i < xx.Length; ++i)
      {
        xx[i] = i * 1E-20;
        double arg = (xx[i] - 5E-20) / 1.5E-20;
        yy[i] = 17E20 * Math.Exp(-0.5 * arg * arg);
      }

      var initialGuess = new double[3] { 10E20, 5E-20, 3E-20 };

      var ff = new GaussAmplitude(1, -1);
      var fit = new QuickNonlinearRegressionWrappedParameters(ff);
      var lowerBounds = new double?[3];
      lowerBounds[2] = 2E-20;
      var fitResult = fit.Fit(xx, yy, initialGuess, lowerBounds, null, null, null, CancellationToken.None);
      AssertEx.GreaterOrEqual(13, fitResult.Iterations); // it should not take more than 13 iterations
      AssertEx.GreaterOrEqual(3.04E41, fitResult.ModelInfoAtMinimum.Value); // Chi2 should be less than 1
      AssertEx.AreEqual(2E-20, fitResult.MinimizingPoint[2], 0, 1E-16);
    }

    /// <summary>
    /// Create a Gaussian with width=1.5E20, but limit the fit width to 2E20.
    /// </summary>
    [Fact]
    public void TestGauss_QuickClamped_2_LowerBoundWidth()
    {
      var xx = new double[10];
      var yy = new double[10];
      for (int i = 0; i < xx.Length; ++i)
      {
        xx[i] = i * 1E20;
        double arg = (xx[i] - 5E20) / 1.5E20;
        yy[i] = 17E-20 * Math.Exp(-0.5 * arg * arg);
      }

      var initialGuess = new double[3] { 10E-20, 5E20, 3E20 };

      var ff = new GaussAmplitude(1, -1);
      var fit = new QuickNonlinearRegression(ff);
      var lowerBounds = new double?[3];
      lowerBounds[2] = 2E20;
      var fitResult = fit.Fit(xx, yy, initialGuess, lowerBounds, null, null, null, CancellationToken.None);
      AssertEx.GreaterOrEqual(13, fitResult.Iterations); // it should not take more than 13 iterations
      AssertEx.GreaterOrEqual(3.1E-39, fitResult.ModelInfoAtMinimum.Value); // test chi2
      AssertEx.AreEqual(2E20, fitResult.MinimizingPoint[2], 0, 1E-16);
    }

    /// <summary>
    /// Create a Gaussian with width=1.5E20, but limit the fit width to 2E20.
    /// </summary>
    [Fact]
    public void TestGauss_QuickWrapped_2_LowerBoundWidth()
    {
      var xx = new double[10];
      var yy = new double[10];
      for (int i = 0; i < xx.Length; ++i)
      {
        xx[i] = i * 1E20;
        double arg = (xx[i] - 5E20) / 1.5E20;
        yy[i] = 17E-20 * Math.Exp(-0.5 * arg * arg);
      }

      var initialGuess = new double[3] { 10E-20, 5E20, 3E20 };

      var ff = new GaussAmplitude(1, -1);
      var fit = new QuickNonlinearRegressionWrappedParameters(ff);
      var lowerBounds = new double?[3];
      lowerBounds[2] = 2E20;
      var fitResult = fit.Fit(xx, yy, initialGuess, lowerBounds, null, null, null, CancellationToken.None);
      AssertEx.GreaterOrEqual(30, fitResult.Iterations); // it should not take more than 13 iterations
      AssertEx.GreaterOrEqual(4E-39, fitResult.ModelInfoAtMinimum.Value); // test chi2
      AssertEx.AreEqual(2E20, fitResult.MinimizingPoint[2], 0, 1E-10);
    }

    #endregion

    #region Test upper bounds

    /// <summary>
    /// Create a Gaussian with height 17E20, but limit the fit height to 16E20.
    /// </summary>
    [Fact]
    public void TestGauss_QuickClamped_1_UpperBoundHeight()
    {
      const double amplitude = 17E20;
      const double upperlimit_amplitude = 16E20;
      const double position = 5E-20;
      const double width = 1.5E-20;

      var xx = new double[10];
      var yy = new double[10];
      for (int i = 0; i < xx.Length; ++i)
      {
        xx[i] = i * 1E-20;
        yy[i] = GaussAmplitude.GetYOfOneTerm(xx[i], amplitude, position, width);
      }

      var initialGuess = new double[3] { amplitude / 2.0, position + width / 8, width * 1.25 };

      var ff = new GaussAmplitude(1, -1);
      var fit = new QuickNonlinearRegression(ff);
      var upperBounds = new double?[3];
      upperBounds[0] = upperlimit_amplitude;
      var fitResult = fit.Fit(xx, yy, initialGuess, null, upperBounds, null, null, CancellationToken.None);

      Assert.True(fitResult.ReasonForExit == Optimization.ExitCondition.Converged);
      AssertEx.GreaterOrEqual(12, fitResult.Iterations); // it should not take more than 12 iterations
      AssertEx.GreaterOrEqual(1.81E40, fitResult.ModelInfoAtMinimum.Value); // Chi2 should be less than 1
      AssertEx.AreEqual(upperlimit_amplitude, fitResult.MinimizingPoint[0], 0, 1E-14);
      Assert.True(fitResult.IsFixedByUserOrBoundaries[0] == true);
      Assert.True(fitResult.IsFixedByUserOrBoundaries[1] == false);
      Assert.True(fitResult.IsFixedByUserOrBoundaries[2] == false);
      AssertEx.AreEqual(0, fitResult.StandardErrors[0], 0, 0);
      AssertEx.Less(0, fitResult.StandardErrors[1]);
      AssertEx.Less(0, fitResult.StandardErrors[2]);

      // Make exactly the same test again.
      // But now we start with the exact initial parameters for position and width
      // This is to test if the automatic Hessian scaling will cause problems when starting with exact initial values.

      initialGuess = new double[3] { amplitude / 2.0, position, width };
      fitResult = fit.Fit(xx, yy, initialGuess, null, upperBounds, null, null, CancellationToken.None);

      Assert.True(fitResult.ReasonForExit == Optimization.ExitCondition.Converged || fitResult.ReasonForExit == Optimization.ExitCondition.RelativePoints);
      AssertEx.GreaterOrEqual(12, fitResult.Iterations); // it should not take more than 12 iterations
      AssertEx.GreaterOrEqual(1.81E40, fitResult.ModelInfoAtMinimum.Value); // Chi2 should be less than 1
      AssertEx.AreEqual(upperlimit_amplitude, fitResult.MinimizingPoint[0], 0, 1E-14);
      Assert.True(fitResult.IsFixedByUserOrBoundaries[0] == true);
      Assert.True(fitResult.IsFixedByUserOrBoundaries[1] == false);
      Assert.True(fitResult.IsFixedByUserOrBoundaries[2] == false);
      AssertEx.AreEqual(0, fitResult.StandardErrors[0], 0, 0);
      AssertEx.Less(0, fitResult.StandardErrors[1]);
      AssertEx.Less(0, fitResult.StandardErrors[2]);
    }

    /// <summary>
    /// Create a Gaussian with height 17E20, but limit the fit height to 16E20.
    /// </summary>
    [Fact]
    public void TestGauss_QuickWrapped_1_UpperBoundHeight()
    {
      var xx = new double[10];
      var yy = new double[10];
      for (int i = 0; i < xx.Length; ++i)
      {
        xx[i] = i * 1E-20;
        double arg = (xx[i] - 5E-20) / 1.5E-20;
        yy[i] = 17E20 * Math.Exp(-0.5 * arg * arg);
      }

      var initialGuess = new double[3] { 10E20, 5E-20, 3E-20 };

      var ff = new GaussAmplitude(1, -1);
      var fit = new QuickNonlinearRegressionWrappedParameters(ff);
      var upperBounds = new double?[3];
      upperBounds[0] = 16E20;
      var fitResult = fit.Fit(xx, yy, initialGuess, null, upperBounds, null, null, CancellationToken.None);
      AssertEx.GreaterOrEqual(23, fitResult.Iterations); // it should not take more than 10 iterations
      AssertEx.GreaterOrEqual(1.86E40, fitResult.ModelInfoAtMinimum.Value); // Chi2 should be less than 1
      AssertEx.AreEqual(16E20, fitResult.MinimizingPoint[0], 0, 1E-8);
    }

    /// <summary>
    /// Create a Gaussian with height 17E-20, but limit the fit height to 16E-20.
    /// </summary>
    [Fact]
    public void TestGauss_QuickClamped_2_UpperBoundHeight()
    {
      const double amplitude = 17E-20;
      const double upperlimit_amplitude = 16E-20;
      const double position = 5E20;
      const double width = 1.5E20;

      var xx = new double[10];
      var yy = new double[10];
      for (int i = 0; i < xx.Length; ++i)
      {
        xx[i] = i * 1E20;
        yy[i] = GaussAmplitude.GetYOfOneTerm(xx[i], amplitude, position, width);
      }
      var initialGuess = new double[3] { amplitude / 2.0, position + width / 8, width * 1.25 };

      var ff = new GaussAmplitude(1, -1);
      var fit = new QuickNonlinearRegression(ff);
      var upperBounds = new double?[3];
      upperBounds[0] = upperlimit_amplitude;
      var fitResult = fit.Fit(xx, yy, initialGuess, null, upperBounds, null, null, CancellationToken.None);

      Assert.True(fitResult.ReasonForExit == Optimization.ExitCondition.Converged);
      AssertEx.GreaterOrEqual(12, fitResult.Iterations); // it should not take more than 12 iterations
      AssertEx.GreaterOrEqual(1.81E-40, fitResult.ModelInfoAtMinimum.Value); // Chi2 should be less than 1
      AssertEx.AreEqual(upperlimit_amplitude, fitResult.MinimizingPoint[0], 0, 1E-14);
      Assert.True(fitResult.IsFixedByUserOrBoundaries[0] == true);
      Assert.True(fitResult.IsFixedByUserOrBoundaries[1] == false);
      Assert.True(fitResult.IsFixedByUserOrBoundaries[2] == false);
      AssertEx.AreEqual(0, fitResult.StandardErrors[0], 0, 0);
      AssertEx.Less(0, fitResult.StandardErrors[1]);
      AssertEx.Less(0, fitResult.StandardErrors[2]);

      // Make exactly the same test again.
      // But now we start with the exact initial parameters for position and width
      // This is to test if the automatic Hessian scaling will cause problems when starting with exact initial values.

      initialGuess = new double[3] { amplitude / 2.0, position, width };
      fitResult = fit.Fit(xx, yy, initialGuess, null, upperBounds, null, null, CancellationToken.None);

      Assert.True(fitResult.ReasonForExit == Optimization.ExitCondition.Converged || fitResult.ReasonForExit == Optimization.ExitCondition.RelativePoints);
      AssertEx.GreaterOrEqual(12, fitResult.Iterations); // it should not take more than 12 iterations
      AssertEx.GreaterOrEqual(1.81E-40, fitResult.ModelInfoAtMinimum.Value); // Chi2 should be less than 1
      AssertEx.AreEqual(upperlimit_amplitude, fitResult.MinimizingPoint[0], 0, 1E-14);
      Assert.True(fitResult.IsFixedByUserOrBoundaries[0] == true);
      Assert.True(fitResult.IsFixedByUserOrBoundaries[1] == false);
      Assert.True(fitResult.IsFixedByUserOrBoundaries[2] == false);
      AssertEx.AreEqual(0, fitResult.StandardErrors[0], 0, 0);
      AssertEx.Less(0, fitResult.StandardErrors[1]);
      AssertEx.Less(0, fitResult.StandardErrors[2]);
    }

    /// <summary>
    /// Create a Gaussian with height 17E-20, but limit the fit width to 16E-20.
    /// </summary>
    [Fact]
    public void TestGauss_QuickWrapped_2_UpperBoundHeight()
    {
      var xx = new double[10];
      var yy = new double[10];
      for (int i = 0; i < xx.Length; ++i)
      {
        xx[i] = i * 1E20;
        double arg = (xx[i] - 5E20) / 1.5E20;
        yy[i] = 17E-20 * Math.Exp(-0.5 * arg * arg);
      }

      var initialGuess = new double[3] { 10E-20, 5E20, 3E20 };

      var ff = new GaussAmplitude(1, -1);
      var fit = new QuickNonlinearRegressionWrappedParameters(ff);
      var upperBounds = new double?[3];
      upperBounds[0] = 16E-20;
      var fitResult = fit.Fit(xx, yy, initialGuess, null, upperBounds, null, null, CancellationToken.None);
      AssertEx.GreaterOrEqual(11, fitResult.Iterations); // it should not take more than 10 iterations
      AssertEx.GreaterOrEqual(2E-40, fitResult.ModelInfoAtMinimum.Value); // Chi2 should be less than 1
      AssertEx.AreEqual(16E-20, fitResult.MinimizingPoint[0], 0, 1E-16);
    }

    #endregion

    #region Lower-Upper-Bounds

    /// <summary>
    /// Create a Gaussian with position 5E-20, but limit the positions to 4E20 to 6E20.
    /// </summary>
    [Fact]
    public void TestGauss_Clamped_1_LowerUpperPosition()
    {
      var xx = new double[10];
      var yy = new double[10];
      for (int i = 0; i < xx.Length; ++i)
      {
        xx[i] = i * 1E-20;
        double arg = (xx[i] - 5E-20) / 1.5E-20;
        yy[i] = 17E20 * Math.Exp(-0.5 * arg * arg);
      }

      var initialGuess = new double[3] { 12E20, 4E-20, 3E-20 };

      var ff = new GaussAmplitude(1, -1);
      var fit = new QuickNonlinearRegression(ff);
      var lowerBounds = new double?[3];
      var upperBounds = new double?[3];
      lowerBounds[1] = 4E-20;
      upperBounds[1] = 6E-20;
      var fitResult = fit.Fit(xx, yy, initialGuess, lowerBounds, upperBounds, null, null, CancellationToken.None);
      AssertEx.GreaterOrEqual(10, fitResult.Iterations); // it should not take more than 8 iterations
      AssertEx.GreaterOrEqual(1, fitResult.ModelInfoAtMinimum.Value); // Chi2 should be less than 1
      AssertEx.AreEqual(17E20, fitResult.MinimizingPoint[0], 0, 1E-16);
      AssertEx.AreEqual(5E-20, fitResult.MinimizingPoint[1], 0, 1E-16);
      AssertEx.AreEqual(1.5E-20, fitResult.MinimizingPoint[2], 0, 1E-16);
    }

    /// <summary>
    /// Create a Gaussian with position 5E-20, but limit the positions to 4E20 to 6E20.
    /// </summary>
    [Fact]
    public void TestGauss_Wrapped_1_LowerUpperPosition()
    {
      var xx = new double[10];
      var yy = new double[10];
      for (int i = 0; i < xx.Length; ++i)
      {
        xx[i] = i * 1E-20;
        double arg = (xx[i] - 5E-20) / 1.5E-20;
        yy[i] = 17E20 * Math.Exp(-0.5 * arg * arg);
      }

      var initialGuess = new double[3] { 12E20, 4E-20, 3E-20 };

      var ff = new GaussAmplitude(1, -1);
      var fit = new QuickNonlinearRegressionWrappedParameters(ff);
      var lowerBounds = new double?[3];
      var upperBounds = new double?[3];
      lowerBounds[1] = 4E-20;
      upperBounds[1] = 6E-20;
      var fitResult = fit.Fit(xx, yy, initialGuess, lowerBounds, upperBounds, null, null, CancellationToken.None);
      AssertEx.GreaterOrEqual(10, fitResult.Iterations); // it should not take more than 8 iterations
      AssertEx.GreaterOrEqual(1, fitResult.ModelInfoAtMinimum.Value); // Chi2 should be less than 1
      AssertEx.AreEqual(17E20, fitResult.MinimizingPoint[0], 0, 1E-16);
      AssertEx.AreEqual(5E-20, fitResult.MinimizingPoint[1], 0, 1E-16);
      AssertEx.AreEqual(1.5E-20, fitResult.MinimizingPoint[2], 0, 1E-16);
    }

    /// <summary>
    /// Create a Gaussian with position 5E20, but limit the positions to 4E20 to 6E20.
    /// </summary>
    [Fact]
    public void TestGauss_Clamped_2_LowerUpperPosition()
    {
      var xx = new double[10];
      var yy = new double[10];
      for (int i = 0; i < xx.Length; ++i)
      {
        xx[i] = i * 1E20;
        double arg = (xx[i] - 5E20) / 1.5E20;
        yy[i] = 17E-20 * Math.Exp(-0.5 * arg * arg);
      }

      var initialGuess = new double[3] { 12E-20, 4E20, 3E20 };

      var ff = new GaussAmplitude(1, -1);
      var fit = new QuickNonlinearRegression(ff);
      var lowerBounds = new double?[3];
      var upperBounds = new double?[3];
      lowerBounds[1] = 4E20;
      upperBounds[1] = 6E20;
      var fitResult = fit.Fit(xx, yy, initialGuess, lowerBounds, upperBounds, null, null, CancellationToken.None);
      AssertEx.GreaterOrEqual(10, fitResult.Iterations); // it should not take more than 8 iterations
      AssertEx.GreaterOrEqual(0, fitResult.ModelInfoAtMinimum.Value); // Chi2 should be less than 1
      AssertEx.AreEqual(17E-20, fitResult.MinimizingPoint[0], 0, 1E-16);
      AssertEx.AreEqual(5E20, fitResult.MinimizingPoint[1], 0, 1E-16);
      AssertEx.AreEqual(1.5E20, fitResult.MinimizingPoint[2], 0, 1E-16);
    }

    /// <summary>
    /// Create a Gaussian with position 5E20, but limit the positions to 4E20 to 6E20.
    /// </summary>
    [Fact]
    public void TestGauss_Wrapped_2_LowerUpperPosition()
    {
      var xx = new double[10];
      var yy = new double[10];
      for (int i = 0; i < xx.Length; ++i)
      {
        xx[i] = i * 1E20;
        double arg = (xx[i] - 5E20) / 1.5E20;
        yy[i] = 17E-20 * Math.Exp(-0.5 * arg * arg);
      }

      var initialGuess = new double[3] { 12E-20, 4E20, 3E20 };

      var ff = new GaussAmplitude(1, -1);
      var fit = new QuickNonlinearRegression(ff);
      var lowerBounds = new double?[3];
      var upperBounds = new double?[3];
      lowerBounds[1] = 4E20;
      upperBounds[1] = 6E20;
      var fitResult = fit.Fit(xx, yy, initialGuess, lowerBounds, upperBounds, null, null, CancellationToken.None);
      AssertEx.GreaterOrEqual(10, fitResult.Iterations); // it should not take more than 8 iterations
      AssertEx.GreaterOrEqual(0, fitResult.ModelInfoAtMinimum.Value); // Chi2 should be less than 1
      AssertEx.AreEqual(17E-20, fitResult.MinimizingPoint[0], 0, 1E-16);
      AssertEx.AreEqual(5E20, fitResult.MinimizingPoint[1], 0, 1E-16);
      AssertEx.AreEqual(1.5E20, fitResult.MinimizingPoint[2], 0, 1E-16);
    }

    #endregion

    #region Covariance matrix

    /// <summary>
    /// Calculation of covariance matrix is tested on a fit model y=a+bx with 3 data points, for which the covariance matrix can be calculated analytically.
    /// The x-axis is scaled then by 1E-40 and 1E40, and it is expected, that the covariance matrix scales accordingly, without loosing accuracy.
    /// 
    /// </summary>
    [Fact]
    public void TestCovariance_DifferentScales()
    {
      var xx = new double[3];
      var yy = new double[3];

      const double A0 = -0.0332225913621262458471761;
      const double A1 = 0.996677740863787375415282;

      const double Cov00 = 0.002218518559397799141289831238;
      const double Cov01 = -0.0001103740576814825443427776735;
      const double Cov10 = -0.0001103740576814825443427776735;
      const double Cov11 = 0.003311221730444476330283330206;

      xx[0] = -1;
      xx[1] = 0.1;
      xx[2] = 1;

      yy[0] = -1;
      yy[1] = 0;
      yy[2] = 1;

      var initialGuess = new double[2] { A0, A1 };
      var ff = new Altaxo.Calc.FitFunctions.General.Polynomial(1, 0);
      var fit = new QuickNonlinearRegression(ff) { MaximumNumberOfIterations = 0 }; // only calculating the result
      var fitResult = fit.Fit(xx, yy, initialGuess, CancellationToken.None);

      var cov = fitResult.Covariance;
      AssertEx.AreEqual(Cov00, cov[0, 0], 0, 1E-14);
      AssertEx.AreEqual(Cov01, cov[0, 1], 0, 1E-14);
      AssertEx.AreEqual(Cov10, cov[1, 0], 0, 1E-14);
      AssertEx.AreEqual(Cov11, cov[1, 1], 0, 1E-14);

      // Scale the x-axis by 1E-40, and calculate anew
      xx[0] = -1E-40;
      xx[1] = 0.1E-40;
      xx[2] = 1E-40;
      initialGuess = new double[2] { A0, A1 * 1E40 };
      ff = new Altaxo.Calc.FitFunctions.General.Polynomial(1, 0);
      fit = new QuickNonlinearRegression(ff) { MaximumNumberOfIterations = 0 }; // only calculating the result
      fitResult = fit.Fit(xx, yy, initialGuess, CancellationToken.None);

      cov = fitResult.Covariance;
      AssertEx.AreEqual(Cov00, cov[0, 0], 0, 1E-14);
      AssertEx.AreEqual(Cov01 * 1E40, cov[0, 1], 0, 1E-14);
      AssertEx.AreEqual(Cov10 * 1E40, cov[1, 0], 0, 1E-14);
      AssertEx.AreEqual(Cov11 * 1E80, cov[1, 1], 0, 1E-14);

      // Scale the x-axis by 1E+40, and calculate anew
      xx[0] = -1E40;
      xx[1] = 0.1E40;
      xx[2] = 1E40;

      initialGuess = new double[2] { A0, A1 * 1E-40 };
      ff = new Altaxo.Calc.FitFunctions.General.Polynomial(1, 0);
      fit = new QuickNonlinearRegression(ff) { MaximumNumberOfIterations = 0 }; // only calculating the result
      fitResult = fit.Fit(xx, yy, initialGuess, CancellationToken.None);

      cov = fitResult.Covariance;
      AssertEx.AreEqual(Cov00, cov[0, 0], 0, 1E-14);
      AssertEx.AreEqual(Cov01 * 1E-40, cov[0, 1], 0, 1E-14);
      AssertEx.AreEqual(Cov10 * 1E-40, cov[1, 0], 0, 1E-14);
      AssertEx.AreEqual(Cov11 * 1E-80, cov[1, 1], 0, 1E-14);
    }

    #endregion

    #region Low point count

    /// <summary>
    /// Create a signal with only 3 points, left and right point set to zero, and try to fit that.
    /// </summary>
    [Fact]
    public void TestGauss_3Points()
    {
      var xx = new double[3];
      var yy = new double[3];
      for (int i = 0; i < xx.Length; ++i)
      {
        xx[i] = i + 9;
        yy[i] = i == 1 ? 666 : 0;
      }

      var initialGuess = new double[3] { 650, 10.5, 1 };

      var ff = new GaussAmplitude(1, -1);
      var fit = new QuickNonlinearRegression(ff);
      var lowerBounds = new double?[3];
      lowerBounds[2] = 1E-2;
      // because of the left and right y-value set to zero, the sigma of the
      // Gaussian must approach our boundary
      var fitResult = fit.Fit(xx, yy, initialGuess, lowerBounds, null, null, null, CancellationToken.None);
      AssertEx.AreEqual(666, fitResult.MinimizingPoint[0], 0, 1E-2);
      AssertEx.AreEqual(10, fitResult.MinimizingPoint[1], 0, 1E-2);
      AssertEx.LessOrEqual(1E-2, fitResult.MinimizingPoint[2]);
    }



    #endregion

    #region Voigt Testing

    /// <summary>
    /// Create a Voigt with height 17E20, but limit the fit height to 16E20.
    /// </summary>
    [Fact]
    public void TestVoigt_QuickClamped_1_UpperBoundHeight()
    {
      const double amplitude = 17E20;
      const double upperlimit_area = 60;
      const double position = 5E-20;
      const double width = 1.5E-20;

      var xx = new double[10];
      var yy = new double[10];
      for (int i = 0; i < xx.Length; ++i)
      {
        xx[i] = i * 1E-20;
        var arg = (xx[i] - position) / width;
        yy[i] = amplitude * Math.Exp(-0.5 * Math.Pow(arg * arg, 1.05)); // This is a gauss, but with an exponent of 2.1 instead of 2
      }

      var initialGuess0 = new double[4] { upperlimit_area / 2.0, position + width / 8, width * 1.25, 0 };
      var initialGuess1 = new double[4] { upperlimit_area / 2.0, position + width / 8, width * 1.25, 0.5 };
      var initialGuess2 = new double[4] { upperlimit_area / 2.0, position + width / 8, width * 1.25, 1 };
      var initialGuess3 = new double[4] { upperlimit_area / 2.0, position, width, 0 };
      var initialGuess4 = new double[4] { upperlimit_area / 2.0, position, width, 0.5 };
      var initialGuess5 = new double[4] { upperlimit_area / 2.0, position, width, 1 };

      foreach (var initialGuess in new[] { initialGuess0, initialGuess1, initialGuess2, initialGuess3, initialGuess4, initialGuess5 })
      {
        var ff = new Altaxo.Calc.FitFunctions.Probability.VoigtAreaParametrizationNu(1, -1);
        var fit = new QuickNonlinearRegression(ff);
        var lowerBounds = new double?[4];
        var upperBounds = new double?[4];
        lowerBounds[0] = 0;
        lowerBounds[3] = 0;
        upperBounds[0] = upperlimit_area;
        upperBounds[3] = 1;
        var fitResult = fit.Fit(xx, yy, initialGuess, null, upperBounds, null, null, CancellationToken.None);

        Assert.True(fitResult.ReasonForExit == Optimization.ExitCondition.Converged || fitResult.ReasonForExit == Optimization.ExitCondition.RelativePoints);
        AssertEx.GreaterOrEqual(14, fitResult.Iterations); // it should not take more than 14 iterations
        AssertEx.GreaterOrEqual(1.4E40, fitResult.ModelInfoAtMinimum.Value); // Chi2 should be less than 1.4E40
        AssertEx.AreEqual(upperlimit_area, fitResult.MinimizingPoint[0], 0, 1E-14);
        AssertEx.AreEqual(1, fitResult.MinimizingPoint[3], 0, 1E-14);
        Assert.True(fitResult.IsFixedByUserOrBoundaries[0] == true);
        Assert.True(fitResult.IsFixedByUserOrBoundaries[1] == false);
        Assert.True(fitResult.IsFixedByUserOrBoundaries[2] == false);
        Assert.True(fitResult.IsFixedByUserOrBoundaries[3] == true);
        AssertEx.AreEqual(0, fitResult.StandardErrors[0], 0, 0);
        AssertEx.Less(0, fitResult.StandardErrors[1]);
        AssertEx.Less(0, fitResult.StandardErrors[2]);
        AssertEx.AreEqual(0, fitResult.StandardErrors[3], 0, 0);
      }
    }


    #endregion
  }
}
