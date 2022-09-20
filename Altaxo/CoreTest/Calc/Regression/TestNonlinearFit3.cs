using System;
using Altaxo.Calc.FitFunctions.Probability;
using Altaxo.Calc.Optimization;
using Altaxo.Calc.Optimization.ObjectiveFunctions;
using Altaxo.Calc.Regression.Nonlinear;
using Xunit;

namespace Altaxo.Calc.Regression
{
  public class TestNonlinearFit3
  {
    [Fact]
    private void TestGauss()
    {
      var xx = new double[10];
      var yy = new double[10];
      for (int i = 0; i < xx.Length; ++i)
      {
        xx[i] = i;
        double arg = (xx[i] - 5) / 1.5;
        yy[i] = 17 * Math.Exp(-0.5 * arg * arg);
      }

      var initialGuess = new double[3] { 10, 5, 3 };

      var ff = new GaussAmplitude(1, -1);
      var model = new NonlinearObjectiveFunctionNonAllocating(ff.EvaluateMultiple, ff.EvaluateGradient, 1);
      model.SetObserved(xx, yy, null);
      var fit = new LevenbergMarquardtMinimizerNonAllocating();
      var result = fit.FindMinimum(model, initialGuess, null, null, null, null);
    }

    [Fact]
    private void TestGaussAlt()
    {
      var xx = new double[10];
      var yy = new double[10];
      for (int i = 0; i < xx.Length; ++i)
      {
        xx[i] = i;
        double arg = (xx[i] - 5) / 1.5;
        yy[i] = 17 * Math.Exp(-0.5 * arg * arg);
      }

      var initialGuess = new double[3] { 10, 5, 3 };

      var ff = new GaussAmplitude(1, -1);

      var fit = new QuickNonlinearRegression2(ff);
      fit.Fit(xx, yy, initialGuess, default);
    }
  }
}
