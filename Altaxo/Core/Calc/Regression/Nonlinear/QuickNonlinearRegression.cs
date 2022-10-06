#region Copyright

/////////////////////////////////////////////////////////////////////////////
// Altaxo:  a data processing and data plotting program
// Copyright (C) 2002-2022 Dr. Dirk Lellinger
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

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
  public class QuickNonlinearRegression
  {
    private IFitFunction _fitFunction;

    /// <summary>
    /// Gets or sets the initial mu value (sometimes also named lambda).
    /// </summary>
    public double InitialMu { get; set; } = LevenbergMarquardtMinimizerNonAllocating.DefaultInitialMu;

    /// <summary>
    /// The stopping threshold for infinity norm of the relative gradient value.
    /// The relative gradient is the gradient divided by the parameter value.
    /// </summary>
    public double GradientTolerance { get; set; } = LevenbergMarquardtMinimizerNonAllocating.DefaultGradientTolerance;

    /// <summary>
    /// The stopping threshold for L2 norm of the change of the parameters.
    /// </summary>
    public double StepTolerance { get; set; } = LevenbergMarquardtMinimizerNonAllocating.DefaultStepTolerance;

    /// <summary>
    /// The stopping threshold for the function value or L2 norm of the residuals.
    /// </summary>
    public double FunctionTolerance { get; set; } = LevenbergMarquardtMinimizerNonAllocating.DefaultFunctionTolerance;

    /// <summary>
    /// Gets or sets the minimal RSS (Chi²) improvement [0, 1).
    /// If after 8 iterations the Chi² improvement is smaller than this value, the evaluation is stopped.
    /// </summary>
    public double MinimalRSSImprovement { get; set; } = LevenbergMarquardtMinimizerNonAllocating.DefaultMinimalRSSImprovement;

    /// <summary>
    /// The maximum number of iterations.
    /// </summary>
    public int? MaximumNumberOfIterations { get; set; } = null;

    /// <summary>
    /// Gets or sets a value indicating whether or not it is allowed to use the derivatives of fit function.
    /// </summary>
    /// <value>
    /// If true, numeric approximation of the derivatives is used, even if the fit function supports analytic derivatives.
    /// </value>
    public bool DoNotAllowUsingDerivativesOfFitFunction { get; set; }

    /// <summary>
    /// Gets or sets the accuracy order of numerical derivatives (1..6). Default is 1.
    /// </summary>
    public int AccuracyOrderOfNumericalDerivatives { get; set; } = 1;


    /// <summary>
    /// Initializes a new instance of the quick nonlinear regression class.
    /// </summary>
    /// <param name="fitFunction">The fit function to use for regression.</param>
    public QuickNonlinearRegression(IFitFunction fitFunction)
    {
      _fitFunction = fitFunction;
    }

    /// <summary>
    /// Non-linear least square fitting by the Levenberg-Marquardt algorithm.
    /// </summary>
    /// <param name="xValues">The x-values of the model to fit.</param>
    /// <param name="yValues">The y-values of the model to fit.</param>
    /// <param name="initialGuess">The initially guessed parameter values.</param>
    /// <param name="cancellationToken">Token to cancel the evaluation</param>
    /// <returns>The result of the Levenberg-Marquardt minimization</returns>
    public NonlinearMinimizationResult Fit(
      IReadOnlyList<double> xValues,
      IReadOnlyList<double> yValues,
      IReadOnlyList<double> initialGuess,
      CancellationToken cancellationToken)
    {
      return Fit(xValues, yValues, initialGuess, null, cancellationToken);
    }

    /// <summary>
    /// Non-linear least square fitting by the Levenberg-Marquardt algorithm.
    /// </summary>
    /// <param name="xValues">The x-values of the model to fit.</param>
    /// <param name="yValues">The y-values of the model to fit.</param>
    /// <param name="initialGuess">The initially guessed parameter values.</param>
    /// <param name="isFixed">Array of booleans, which provide which parameters are fixed. Must have the same length as the parameter array. Provide null if not needed.</param>
    /// <param name="cancellationToken">Token to cancel the evaluation</param>
    /// <returns>The result of the Levenberg-Marquardt minimization</returns>
    public NonlinearMinimizationResult Fit(
      IReadOnlyList<double> xValues,
      IReadOnlyList<double> yValues,
      IReadOnlyList<double> initialGuess,
      IReadOnlyList<bool>? isFixed,
      CancellationToken cancellationToken)
    {
      return Fit(xValues, yValues, initialGuess, null, null, null, isFixed, cancellationToken);
    }

    /// <summary>
    /// Non-linear least square fitting by the Levenberg-Marquardt algorithm.
    /// </summary>
    /// <param name="xValues">The x-values of the model to fit.</param>
    /// <param name="yValues">The y-values of the model to fit.</param>
    /// <param name="initialGuess">The initially guessed parameter values.</param>
    /// <param name="lowerBounds">The lower bounds of the parameters. The array must have the same length as the parameter array. Provide null if not needed.</param>
    /// <param name="upperBounds">The upper bounds of the parameters. The array must have the same length as the parameter array. Provide null if not needed.</param>
    /// <param name="scales">The scales of the parameters. The array must have the same length as the parameter array. Provide null if not needed.</param>
    /// <param name="isFixed">Array of booleans, which provide which parameters are fixed. Must have the same length as the parameter array. Provide null if not needed.</param>
    /// <param name="cancellationToken">Token to cancel the evaluation</param>
    /// <returns>The result of the Levenberg-Marquardt minimization</returns>
    public NonlinearMinimizationResult Fit(
      IReadOnlyList<double> xValues,
      IReadOnlyList<double> yValues,
      IReadOnlyList<double> initialGuess,
      IReadOnlyList<double?>? lowerBounds,
      IReadOnlyList<double?>? upperBounds,
      IReadOnlyList<double>? scales,
      IReadOnlyList<bool>? isFixed,
      CancellationToken cancellationToken)
    {
      if (xValues.Count != yValues.Count)
        throw new ArgumentException("Length of x array is unequal length of y array");

      if (initialGuess.Count != _fitFunction.NumberOfParameters)
        throw new ArgumentException("Number of provided parameters is unequal number of parameters of fit function");

      var model = new NonlinearObjectiveFunctionNonAllocating(
        _fitFunction.Evaluate,
        _fitFunction is IFitFunctionWithDerivative fwg && !DoNotAllowUsingDerivativesOfFitFunction ? fwg.EvaluateDerivative : null,
         AccuracyOrderOfNumericalDerivatives);

      model.SetObserved(xValues, yValues, null);

      var fit = new LevenbergMarquardtMinimizerNonAllocating()
      {
        InitialMu = InitialMu,
        GradientTolerance = GradientTolerance,
        StepTolerance = StepTolerance,
        FunctionTolerance = FunctionTolerance,
        MinimalRSSImprovement = MinimalRSSImprovement,
        MaximumIterations = MaximumNumberOfIterations,
      };

      return fit.FindMinimum(model, initialGuess, lowerBounds, upperBounds, scales, isFixed, cancellationToken);
    }
  }
}
