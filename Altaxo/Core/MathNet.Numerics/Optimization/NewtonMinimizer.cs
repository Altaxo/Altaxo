// <copyright file="NewtonMinimizer.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
//
// Copyright (c) 2009-2017 Math.NET
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
// </copyright>

using System;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Optimization.LineSearch;

namespace Altaxo.Calc.Optimization
{
  /// <summary>
  /// Minimizes unconstrained objective functions with Newton's method.
  /// </summary>
  public sealed class NewtonMinimizer : IUnconstrainedMinimizer
  {
    /// <summary>
    /// Gets or sets the stopping threshold for the gradient norm.
    /// </summary>
    public double GradientTolerance { get; set; }
    /// <summary>
    /// Gets or sets the maximum number of iterations.
    /// </summary>
    public int MaximumIterations { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether a line search is used.
    /// </summary>
    public bool UseLineSearch { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NewtonMinimizer"/> class.
    /// </summary>
    /// <param name="gradientTolerance">The stopping threshold for the gradient norm.</param>
    /// <param name="maximumIterations">The maximum number of iterations.</param>
    /// <param name="useLineSearch">If set to <see langword="true"/>, use a line search.</param>
    public NewtonMinimizer(double gradientTolerance, int maximumIterations, bool useLineSearch = false)
    {
      GradientTolerance = gradientTolerance;
      MaximumIterations = maximumIterations;
      UseLineSearch = useLineSearch;
    }

    /// <inheritdoc/>
    public MinimizationResult FindMinimum(IObjectiveFunction objective, Vector<double> initialGuess)
    {
      return Minimum(objective, initialGuess, GradientTolerance, MaximumIterations, UseLineSearch);
    }

    /// <summary>
    /// Minimizes the objective function with Newton's method.
    /// </summary>
    /// <param name="objective">The objective function.</param>
    /// <param name="initialGuess">The initial parameter guess.</param>
    /// <param name="gradientTolerance">The stopping threshold for the gradient norm.</param>
    /// <param name="maxIterations">The maximum number of iterations.</param>
    /// <param name="useLineSearch">If set to <see langword="true"/>, use a line search.</param>
    /// <returns>The minimization result.</returns>
    public static MinimizationResult Minimum(IObjectiveFunction objective, Vector<double> initialGuess, double gradientTolerance = 1e-8, int maxIterations = 1000, bool useLineSearch = false)
    {
      if (!objective.IsGradientSupported)
      {
        throw new IncompatibleObjectiveException("Gradient not supported in objective function, but required for Newton minimization.");
      }

      if (!objective.IsHessianSupported)
      {
        throw new IncompatibleObjectiveException("Hessian not supported in objective function, but required for Newton minimization.");
      }

      // Check that we're not already done
      objective.EvaluateAt(initialGuess);
      ValidateGradient(objective);
      if (objective.Gradient.Norm(2.0) < gradientTolerance)
      {
        return new MinimizationResult(objective, 0, ExitCondition.AbsoluteGradient);
      }

      // Set up line search algorithm
      var lineSearcher = new WeakWolfeLineSearch(1e-4, 0.9, 1e-4, maxIterations: 1000);

      // Subsequent steps
      int iterations = 0;
      int totalLineSearchSteps = 0;
      int iterationsWithNontrivialLineSearch = 0;
      bool tmpLineSearch = false;
      while (objective.Gradient.Norm(2.0) >= gradientTolerance && iterations < maxIterations)
      {
        ValidateHessian(objective);

        var searchDirection = objective.Hessian.LU().Solve(-objective.Gradient);
        if (searchDirection * objective.Gradient >= 0)
        {
          searchDirection = -objective.Gradient;
          tmpLineSearch = true;
        }

        if (useLineSearch || tmpLineSearch)
        {
          LineSearchResult result;
          try
          {
            result = lineSearcher.FindConformingStep(objective, searchDirection, 1.0);
          }
          catch (Exception e)
          {
            throw new InnerOptimizationException("Line search failed.", e);
          }

          iterationsWithNontrivialLineSearch += result.Iterations > 0 ? 1 : 0;
          totalLineSearchSteps += result.Iterations;
          objective = result.FunctionInfoAtMinimum;
        }
        else
        {
          objective.EvaluateAt(objective.Point + searchDirection);
        }

        ValidateGradient(objective);

        tmpLineSearch = false;
        iterations += 1;
      }

      if (iterations == maxIterations)
      {
        throw new MaximumIterationsException(FormattableString.Invariant($"Maximum iterations ({maxIterations}) reached."));
      }

      return new MinimizationWithLineSearchResult(objective, iterations, ExitCondition.AbsoluteGradient, totalLineSearchSteps, iterationsWithNontrivialLineSearch);
    }

    private static void ValidateGradient(IObjectiveFunctionEvaluation eval)
    {
      foreach (var x in eval.Gradient)
      {
        if (double.IsNaN(x) || double.IsInfinity(x))
        {
          throw new EvaluationException("Non-finite gradient returned.", eval);
        }
      }
    }

    private static void ValidateHessian(IObjectiveFunctionEvaluation eval)
    {
      var hessian = eval.Hessian;
      for (int ii = 0; ii < hessian.RowCount; ++ii)
      {
        for (int jj = 0; jj < hessian.ColumnCount; ++jj)
        {
          if (double.IsNaN(hessian[ii, jj]) || double.IsInfinity(hessian[ii, jj]))
          {
            throw new EvaluationException("Non-finite Hessian returned.", eval);
          }
        }
      }
    }
  }
}
