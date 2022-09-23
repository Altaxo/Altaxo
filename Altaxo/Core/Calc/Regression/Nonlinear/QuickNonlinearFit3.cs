using System;
using System.Collections.Generic;
using System.Threading;
using Altaxo.Calc.Optimization;
using Altaxo.Calc.Optimization.ObjectiveFunctions;

namespace Altaxo.Calc.Regression.Nonlinear
{
  /// <summary>
  /// Allows a quick regression of one dependent variable in dependence on one independent variable, i.e. with a function R=>R.
  /// </summary>
  /// <seealso cref="Altaxo.Calc.Regression.Nonlinear.QuickNonlinearRegression" />
  public class QuickNonlinearRegression3 : QuickNonlinearRegression
  {
    private IFitFunction _function;

    public double InitialMu { get; set; } = 0.001;

    public double GradientTolerance { get; set; } = 1E-15;
    public double StepTolerance { get; set; } = 1E-15;
    public double FunctionTolerance { get; set; } = 1E-15;

    public int? MaximumNumberOfIterations { get; set; } = null;




    public QuickNonlinearRegression3(IFitFunction fitFunction) : base(fitFunction)
    {
      _function = fitFunction;
    }

    public NonlinearMinimizationResult Fit(IReadOnlyList<double> xValues, IReadOnlyList<double> yValues, IReadOnlyList<double> initialGuess, CancellationToken cancellationToken)
    {
      return Fit(xValues, yValues, initialGuess, new bool[initialGuess.Count], cancellationToken);
    }

    public NonlinearMinimizationResult Fit(IReadOnlyList<double> xValues, IReadOnlyList<double> yValues, IReadOnlyList<double> initialGuess, IReadOnlyList<bool> isFixed, CancellationToken cancellationToken)
    {
      return Fit(xValues, yValues, initialGuess, null, null, null, isFixed, cancellationToken);
    }

    public NonlinearMinimizationResult Fit(
      IReadOnlyList<double> xValues,
      IReadOnlyList<double> yValues,
      IReadOnlyList<double> initialGuess,
      IReadOnlyList<double?>? lowerBounds,
      IReadOnlyList<double?>? upperBounds,
      IReadOnlyList<double>? scales,
      IReadOnlyList<bool> isFixed,
      CancellationToken cancellationToken)
    {
      _isExecuted = false;
      if (xValues.Count != yValues.Count)
        throw new ArgumentException("Length of x array is unequal length of y array");

      if (initialGuess.Count != _fitFunction.NumberOfParameters)
        throw new ArgumentException("Number of provided parameters is unequal number of parameters of fit function");

      var model = new NonlinearObjectiveFunctionNonAllocating(
        _function.EvaluateMultiple,
        _function is IFitFunctionWithGradient fwg ? fwg.EvaluateGradient : null,
         1);
      model.SetObserved(xValues, yValues, null);
      var fit = new LevenbergMarquardtMinimizerNonAllocating(InitialMu, GradientTolerance, StepTolerance, FunctionTolerance, MaximumNumberOfIterations.HasValue ? MaximumNumberOfIterations.Value : -1);

      var result = fit.FindMinimum(model, initialGuess, lowerBounds, upperBounds, scales, isFixed, cancellationToken);

      _parameters = new double[initialGuess.Count];
      _parameterVariances = new double[initialGuess.Count];
      for (int i = 0; i < _parameters.Length; ++i)
      {
        _parameters[i] = result.MinimizingPoint[i];
        _parameterVariances[i] = result.StandardErrors[i];
      }

      _isExecuted = true;
      return result;
    }
  }
}
