using System;
using System.Threading;
using Altaxo.Calc.FitFunctions.Probability;
using Altaxo.Calc.Regression.Nonlinear;
using Xunit;

namespace Altaxo.Calc.Regression
{
  public class QuickLinearRegression_Test
  {

    [Fact]
    public void TestGauss_QuickOld()
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
      var fit = new QuickNonlinearRegressionOld(ff);
      fit.Fit(xx, yy, initialGuess, CancellationToken.None);
    }

    [Fact]
    public void TestGauss_Quick_1()
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

    [Fact]
    public void TestGauss_QuickNew_2()
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
    /// Create a Gaussian with width=1.5E-20, but limit the fit width to 2E-20.
    /// </summary>
    [Fact]
    public void TestGauss_Quick_1_LowerBoundWidth()
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
    /// Create a Gaussian with height 17E20, but limit the fit width to 16E20.
    /// </summary>
    [Fact]
    public void TestGauss_Quick_1_UpperBoundHeight()
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
    public void TestGauss_Wrapped_1_UpperBoundHeight()
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
      AssertEx.GreaterOrEqual(21, fitResult.Iterations); // it should not take more than 10 iterations
      AssertEx.GreaterOrEqual(1.86E40, fitResult.ModelInfoAtMinimum.Value); // Chi2 should be less than 1
      AssertEx.AreEqual(16E20, fitResult.MinimizingPoint[0], 0, 1E-8);
    }


    /// <summary>
    /// Create a Gaussian with position 5E-20, but limit the positions to 4E20 to 6E20.
    /// </summary>
    [Fact]
    public void TestGauss_Quick_1_LowerUpperPosition()
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

      var initialGuess = new double[3] { 12E20, 4.1E-20, 3E-20 };

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


    [Fact]
    public void TestGauss_QuickWrappedParameters()
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

    [Fact]
    public void TestGauss_QuickWrappedParameters_2()
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
      AssertEx.AreEqual(17E-20, fitResult.MinimizingPoint[0], 0, 1E-16);
      AssertEx.AreEqual(5E20, fitResult.MinimizingPoint[1], 0, 1E-16);
      AssertEx.AreEqual(1.5E20, fitResult.MinimizingPoint[2], 0, 1E-16);
    }
  }
}
