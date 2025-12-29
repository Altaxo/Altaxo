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
using System.Threading;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Optimization;

namespace Altaxo.Calc.Regression.Nonlinear
{
  /// <summary>
  /// Provides a quick nonlinear regression for one dependent variable as a function of one independent variable.
  /// </summary>
  /// <remarks>
  /// This class allocates temporary arrays and vectors during evaluation. Use <see cref="QuickNonlinearRegression"/>
  /// for a non-allocating variant.
  /// </remarks>
  [Obsolete("Please use QuickNonlinearRegression instead.")]
  public class QuickNonlinearRegressionAllocating
  {
    private WrapperToFitFunction _wrapperToFitFunction;
    private IFitFunction _fitFunction;


    /// <summary>
    /// Initializes a new instance of the <see cref="QuickNonlinearRegressionAllocating"/> class.
    /// </summary>
    /// <param name="fitFunction">The fit function to use for regression.</param>
    public QuickNonlinearRegressionAllocating(IFitFunction fitFunction)
    {
      _fitFunction = fitFunction;
      _wrapperToFitFunction = new WrapperToFitFunction(fitFunction);
    }

    /// <summary>
    /// Performs nonlinear least-squares fitting using the Levenberg-Marquardt algorithm.
    /// </summary>
    /// <param name="xValues">The x-values of the model to fit.</param>
    /// <param name="yValues">The y-values of the model to fit.</param>
    /// <param name="initialGuess">The initially guessed parameter values.</param>
    /// <param name="cancellationToken">Token to cancel the evaluation.</param>
    /// <returns>The result of the Levenberg-Marquardt minimization.</returns>
    public NonlinearMinimizationResult Fit(double[] xValues, double[] yValues, double[] initialGuess, CancellationToken cancellationToken)
    {
      return Fit(xValues, yValues, initialGuess, new bool[initialGuess.Length], cancellationToken);
    }

    /// <summary>
    /// Performs nonlinear least-squares fitting using the Levenberg-Marquardt algorithm.
    /// </summary>
    /// <param name="xValues">The x-values of the model to fit.</param>
    /// <param name="yValues">The y-values of the model to fit.</param>
    /// <param name="initialGuess">The initially guessed parameter values.</param>
    /// <param name="isFixed">
    /// Array of booleans indicating which parameters are fixed. Must have the same length as <paramref name="initialGuess"/>.
    /// </param>
    /// <param name="cancellationToken">Token to cancel the evaluation.</param>
    /// <returns>The result of the Levenberg-Marquardt minimization.</returns>
    public NonlinearMinimizationResult Fit(double[] xValues, double[] yValues, double[] initialGuess, bool[] isFixed, CancellationToken cancellationToken)
    {
      if (xValues.Length != yValues.Length)
        throw new ArgumentException("Length of x array is unequal length of y array");

      if (initialGuess.Length != _fitFunction.NumberOfParameters)
        throw new ArgumentException("Number of provided parameters is unequal number of parameters of fit function");


      var xVect = CreateVector.DenseOfArray(xValues);
      var yVect = CreateVector.DenseOfArray(yValues);
      IObjectiveModel obj;
      if (_wrapperToFitFunction.FitFunction is IFitFunctionWithDerivative)
        obj = ObjectiveFunction.NonlinearModel(_wrapperToFitFunction.Evaluate, _wrapperToFitFunction.EvaluateDerivative, xVect, yVect);
      else
        obj = ObjectiveFunction.NonlinearModel(_wrapperToFitFunction.Evaluate, xVect, yVect);

      var levmar = new LevenbergMarquardtMinimizer();
      return levmar.FindMinimum(obj, initialGuess, null, null, null, isFixed);
    }

    /// <summary>
    /// Wraps an <see cref="IFitFunction"/> to the delegate-based interface expected by the objective function.
    /// </summary>
    internal class WrapperToFitFunction
    {
      /// <summary>
      /// Gets the wrapped fit function.
      /// </summary>
      public IFitFunction FitFunction { get; private set; }

      private double[] _x = new double[1];
      private double[] _y = new double[1];

      /// <summary>
      /// Initializes a new instance of the <see cref="WrapperToFitFunction"/> class.
      /// </summary>
      /// <param name="f">The fit function to wrap.</param>
      public WrapperToFitFunction(IFitFunction f)
      {
        FitFunction = f;
      }

      /// <summary>
      /// Evaluates the fit function for the provided parameters and x-values.
      /// </summary>
      /// <param name="parameter">The parameter vector.</param>
      /// <param name="x">The vector of x-values.</param>
      /// <returns>The vector of calculated y-values.</returns>
      public Vector<double> Evaluate(Vector<double> parameter, Vector<double> x)
      {
        var yR = Vector<double>.Build.Dense(x.Count);
        FitFunction.Evaluate(MatrixMath.ToROMatrixWithOneColumn(x), parameter, yR, null);
        return yR;
      }

      /// <summary>
      /// Evaluates the derivatives of the fit function with respect to the parameters.
      /// </summary>
      /// <param name="parameter">The parameter vector.</param>
      /// <param name="x">The vector of x-values.</param>
      /// <returns>A Jacobian matrix with one row per x-value and one column per parameter.</returns>
      public Matrix<double> EvaluateDerivative(Vector<double> parameter, Vector<double> x)
      {
        var yR = Matrix<double>.Build.Dense(x.Count, parameter.Count);
        ((IFitFunctionWithDerivative)FitFunction).EvaluateDerivative(MatrixMath.ToROMatrixWithOneColumn(x), parameter, null, yR, null);
        return yR;
      }
    }
  }
}
