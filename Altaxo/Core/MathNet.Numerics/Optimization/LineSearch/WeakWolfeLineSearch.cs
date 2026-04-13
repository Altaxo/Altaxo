// <copyright file="WeakWolfeLineSearch.cs" company="Math.NET">
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

namespace Altaxo.Calc.Optimization.LineSearch
{
  /// <summary>
  /// Search for a step size alpha that satisfies the weak Wolfe conditions. The weak Wolfe
  /// Conditions are
  /// i)  Armijo Rule:         f(x_k + alpha_k p_k) &lt;= f(x_k) + c1 alpha_k p_k^T g(x_k)
  /// ii) Curvature Condition: p_k^T g(x_k + alpha_k p_k) &gt;= c2 p_k^T g(x_k)
  /// where g(x) is the gradient of f(x), 0 &lt; c1 &lt; c2 &lt; 1.
  ///
  /// Implementation is based on http://www.math.washington.edu/~burke/crs/408/lectures/L9-weak-Wolfe.pdf
  ///
  /// references:
  /// http://en.wikipedia.org/wiki/Wolfe_conditions
  /// http://www.math.washington.edu/~burke/crs/408/lectures/L9-weak-Wolfe.pdf
  /// </summary>
  public class WeakWolfeLineSearch : WolfeLineSearch
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="WeakWolfeLineSearch"/> class.
    /// </summary>
    /// <param name="c1">The sufficient decrease constant.</param>
    /// <param name="c2">The curvature constant.</param>
    /// <param name="parameterTolerance">The tolerance used to detect lack of progress.</param>
    /// <param name="maxIterations">The maximum number of iterations.</param>
    public WeakWolfeLineSearch(double c1, double c2, double parameterTolerance, int maxIterations = 10)
        : base(c1, c2, parameterTolerance, maxIterations)
    {
      // Validation in base class
    }

    /// <inheritdoc />
    protected override ExitCondition WolfeExitCondition => ExitCondition.WeakWolfeCriteria;

    /// <inheritdoc />
    protected override bool WolfeCondition(double stepDd, double initialDd)
    {
      return stepDd < C2 * initialDd;
    }

    /// <inheritdoc />
    protected override void ValidateValue(IObjectiveFunctionEvaluation objective)
    {
      if (!IsFinite(objective.Value))
      {
        throw new EvaluationException(FormattableString.Invariant($"Non-finite value returned by objective function: {objective.Value}"), objective);
      }
    }

    /// <inheritdoc />
    protected override void ValidateInputArguments(IObjectiveFunctionEvaluation startingPoint, Vector<double> searchDirection, double initialStep, double upperBound)
    {
      if (!startingPoint.IsGradientSupported)
        throw new ArgumentException("objective function does not support gradient");
    }

    /// <inheritdoc />
    protected override void ValidateGradient(IObjectiveFunctionEvaluation objective)
    {
      foreach (double x in objective.Gradient)
      {
        if (!IsFinite(x))
        {
          throw new EvaluationException(FormattableString.Invariant($"Non-finite value returned by gradient: {x}"), objective);
        }
      }
    }

    private static bool IsFinite(double x)
    {
      return !(double.IsNaN(x) || double.IsInfinity(x));
    }
  }
}
