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
    public void TestGauss_QuickNew()
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
      AssertEx.GreaterOrEqual(10, fitResult.Iterations); // it should not take more than 8 iterations
      AssertEx.GreaterOrEqual(1, fitResult.ModelInfoAtMinimum.Value); // Chi2 should be less than 1
      AssertEx.AreEqual(17E20, fitResult.MinimizingPoint[0], 0, 1E-16);
      AssertEx.AreEqual(5E-20, fitResult.MinimizingPoint[1], 0, 1E-16);
      AssertEx.AreEqual(1.5E-20, fitResult.MinimizingPoint[2], 0, 1E-16);
    }
  }
}
