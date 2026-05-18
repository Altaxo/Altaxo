using System;
using System.Collections.Generic;
using System.Threading;
using Altaxo.Calc.FitFunctions.Peaks;
using Altaxo.Calc.Optimization.ObjectiveFunctions;
using Xunit;

namespace Altaxo.Calc.Optimization
{
  public class LevenbergMarquardtMinimizerNonAllocatingWrappedParametersTests
  {
    [Fact]
    public void WrappedParameters_WithoutBounds_FindsGaussianMinimum()
    {
      var result = RunWrapped(new[] { 10.0, 5.0, 3.0 });

      AssertEx.GreaterOrEqual(2E-16, result.ModelInfoAtMinimum.Value);
      AssertParameter(result.MinimizingPoint, 0, 17.0, 1E-8);
      AssertParameter(result.MinimizingPoint, 1, 5.0, 1E-8);
      AssertParameter(result.MinimizingPoint, 2, 1.5, 1E-8);
    }

    [Fact]
    public void WrappedParameters_WithUserFixedCenter_KeepsCenterFixed()
    {
      var result = RunWrapped(
        initialGuess: new[] { 10.0, 5.25, 3.0 },
        isFixed: new[] { false, true, false });

      AssertParameter(result.MinimizingPoint, 1, 5.25, 1E-12);
      AssertEx.LessOrEqual(3.0, result.ModelInfoAtMinimum.Value);
    }

    private static NonlinearMinimizationResult RunWrapped(
      double[] initialGuess,
      double?[]? lowerBound = null,
      double?[]? upperBound = null,
      double[]? scales = null,
      bool[]? isFixed = null)
    {
      var objective = CreateGaussianObjective();
      var fit = new LevenbergMarquardtMinimizerNonAllocatingWrappedParameters();
      return fit.FindMinimum(objective, initialGuess, lowerBound, upperBound, scales, isFixed, CancellationToken.None, null);
    }

    private static NonlinearObjectiveFunctionNonAllocating CreateGaussianObjective()
    {
      var (x, y) = CreateGaussianData();
      var fitFunction = new GaussAmplitude(1, -1);
      var objective = new NonlinearObjectiveFunctionNonAllocating(fitFunction.Evaluate, fitFunction.EvaluateDerivative, 1);
      objective.SetObserved(x, y, null);
      return objective;
    }

    private static (double[] x, double[] y) CreateGaussianData()
    {
      var x = new double[10];
      var y = new double[10];

      for (int i = 0; i < x.Length; ++i)
      {
        x[i] = i;
        double arg = (x[i] - 5.0) / 1.5;
        y[i] = 17.0 * Math.Exp(-0.5 * arg * arg);
      }

      return (x, y);
    }

    private static void AssertParameter(IReadOnlyList<double> values, int index, double expected, double relativeTolerance)
    {
      AssertEx.AreEqual(expected, values[index], 1E-12, relativeTolerance);
    }
  }
}
