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
    /// Create a Gaussian with height 17E20, but limit the fit width to 16E20.
    /// </summary>
    [Fact]
    public void TestGauss_QuickClamped_1_UpperBoundHeight()
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
      var upperBounds = new double?[3];
      upperBounds[0] = 16E20;
      var fitResult = fit.Fit(xx, yy, initialGuess, null, upperBounds, null, null, CancellationToken.None);
      AssertEx.GreaterOrEqual(10, fitResult.Iterations); // it should not take more than 10 iterations
      AssertEx.GreaterOrEqual(1.81E40, fitResult.ModelInfoAtMinimum.Value); // Chi2 should be less than 1
      AssertEx.AreEqual(16E20, fitResult.MinimizingPoint[0], 0, 1E-16);
    }

    /// <summary>
    /// Create a Gaussian with height 17E20, but limit the fit width to 16E20.
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
    /// Create a Gaussian with height 17E-20, but limit the fit width to 16E-20.
    /// </summary>
    [Fact]
    public void TestGauss_QuickClamped_2_UpperBoundHeight()
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
      var upperBounds = new double?[3];
      upperBounds[0] = 16E-20;
      var fitResult = fit.Fit(xx, yy, initialGuess, null, upperBounds, null, null, CancellationToken.None);
      AssertEx.GreaterOrEqual(11, fitResult.Iterations); // it should not take more than 10 iterations
      AssertEx.GreaterOrEqual(2E-40, fitResult.ModelInfoAtMinimum.Value); // Chi2 should be less than 1
      AssertEx.AreEqual(16E-20, fitResult.MinimizingPoint[0], 0, 1E-16);
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
  }
}
