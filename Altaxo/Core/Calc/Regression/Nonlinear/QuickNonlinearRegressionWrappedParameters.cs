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
  /// Allows a quick regression of one dependent variable in dependence on one independent variable,
  /// i.e. with a function R =&gt; R.
  /// </summary>
  /// <seealso cref="Altaxo.Calc.Regression.Nonlinear.QuickNonlinearRegressionOld" />
  public class QuickNonlinearRegressionWrappedParameters
  {
    private IFitFunction _fitFunction;

    /// <summary>
    /// Gets or sets the initial mu value (sometimes also named lambda).
    /// </summary>
    public double InitialMu { get; set; } = LevenbergMarquardtMinimizerNonAllocatingWrappedParameters.DefaultInitialMu;

    /// <summary>
    /// Gets or sets the stopping threshold for the infinity norm of the relative gradient.
    /// </summary>
    /// <remarks>
    /// The relative gradient is the gradient divided by the parameter value.
    /// </remarks>
    public double GradientTolerance { get; set; } = LevenbergMarquardtMinimizerNonAllocatingWrappedParameters.DefaultGradientTolerance;

    /// <summary>
    /// Gets or sets the stopping threshold for the L2 norm of the change of the parameters.
    /// </summary>
    public double StepTolerance { get; set; } = LevenbergMarquardtMinimizerNonAllocatingWrappedParameters.DefaultStepTolerance;

    /// <summary>
    /// Gets or sets the stopping threshold for the function value or the L2 norm of the residuals.
    /// </summary>
    public double FunctionTolerance { get; set; } = LevenbergMarquardtMinimizerNonAllocatingWrappedParameters.DefaultFunctionTolerance;

    /// <summary>
    /// Gets or sets the minimal RSS (Chi²) improvement in the interval [0, 1).
    /// </summary>
    /// <remarks>
    /// If, after 8 iterations, the Chi² improvement is smaller than this value, the evaluation is stopped.
    /// </remarks>
    public double MinimalRSSImprovement { get; set; } = LevenbergMarquardtMinimizerNonAllocatingWrappedParameters.DefaultMinimalRSSImprovement;

    /// <summary>
    /// Gets or sets the maximum number of iterations.
    /// </summary>
    public int? MaximumNumberOfIterations { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether it is allowed to use analytic derivatives provided by the fit function.
    /// </summary>
    /// <value>
    /// If <c>true</c>, numeric approximation of the derivatives is used even if the fit function supports analytic derivatives.
    /// </value>
    public bool DoNotAllowUsingDerivativesOfFitFunction { get; set; }

    /// <summary>
    /// Gets or sets the accuracy order of numerical derivatives (1..6). Default is 1.
    /// </summary>
    public int AccuracyOrderOfNumericalDerivatives { get; set; } = 1;


    /// <summary>
    /// Initializes a new instance of the <see cref="QuickNonlinearRegressionWrappedParameters"/> class.
    /// </summary>
    /// <param name="fitFunction">The fit function to use for regression.</param>
    public QuickNonlinearRegressionWrappedParameters(IFitFunction fitFunction)
    {
      _fitFunction = fitFunction;
    }

    /// <summary>
    /// Non-linear least square fitting by the Levenberg-Marquardt algorithm.
    /// </summary>
    /// <param name="xValues">The x-values of the model to fit.</param>
    /// <param name="yValues">The y-values of the model to fit.</param>
    /// <param name="initialGuess">The initially guessed parameter values.</param>
    /// <param name="cancellationToken">Token to cancel the evaluation.</param>
    /// <returns>The result of the Levenberg-Marquardt minimization.</returns>
    public NonlinearMinimizationResult Fit(
      IReadOnlyList<double> xValues,
      IReadOnlyList<double> yValues,
      IReadOnlyList<double> initialGuess,
      CancellationToken cancellationToken)
    {
      return Fit(xValues, yValues, initialGuess, new bool[initialGuess.Count], cancellationToken);
    }

    /// <summary>
    /// Non-linear least square fitting by the Levenberg-Marquardt algorithm.
    /// </summary>
    /// <param name="xValues">The x-values of the model to fit.</param>
    /// <param name="yValues">The y-values of the model to fit.</param>
    /// <param name="initialGuess">The initially guessed parameter values.</param>
    /// <param name="isFixed">Array of booleans indicating which parameters are fixed. Must have the same length as the parameter array.</param>
    /// <param name="cancellationToken">Token to cancel the evaluation.</param>
    /// <returns>The result of the Levenberg-Marquardt minimization.</returns>
    public NonlinearMinimizationResult Fit(
      IReadOnlyList<double> xValues,
      IReadOnlyList<double> yValues,
      IReadOnlyList<double> initialGuess,
      IReadOnlyList<bool> isFixed,
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
    /// <param name="lowerBounds">
    /// The lower bounds of the parameters. The array must have the same length as the parameter array.
    /// Provide <see langword="null"/> if not needed.
    /// </param>
    /// <param name="upperBounds">
    /// The upper bounds of the parameters. The array must have the same length as the parameter array.
    /// Provide <see langword="null"/> if not needed.
    /// </param>
    /// <param name="scales">
    /// The scales of the parameters. The array must have the same length as the parameter array.
    /// Provide <see langword="null"/> if not needed.
    /// </param>
    /// <param name="isFixed">Array of booleans indicating which parameters are fixed. Must have the same length as the parameter array.</param>
    /// <param name="cancellationToken">Token to cancel the evaluation.</param>
    /// <returns>The result of the Levenberg-Marquardt minimization.</returns>
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
      if (xValues.Count != yValues.Count)
        throw new ArgumentException("Length of x array is unequal length of y array");

      if (initialGuess.Count != _fitFunction.NumberOfParameters)
        throw new ArgumentException("Number of provided parameters is unequal number of parameters of fit function");

      var model = new NonlinearObjectiveFunctionNonAllocating(
        _fitFunction.Evaluate,
        _fitFunction is IFitFunctionWithDerivative fwg && !DoNotAllowUsingDerivativesOfFitFunction ? fwg.EvaluateDerivative : null,
         AccuracyOrderOfNumericalDerivatives);

      model.SetObserved(xValues, yValues, null);

      var fit = new LevenbergMarquardtMinimizerNonAllocatingWrappedParameters()
      {
        InitialMu = InitialMu,
        GradientTolerance = GradientTolerance,
        StepTolerance = StepTolerance,
        FunctionTolerance = FunctionTolerance,
        MinimalRSSImprovement = MinimalRSSImprovement,
        MaximumIterations = MaximumNumberOfIterations,
      };

      return fit.FindMinimum(model, initialGuess, lowerBounds, upperBounds, scales, isFixed, cancellationToken, null);
    }
  }
}
