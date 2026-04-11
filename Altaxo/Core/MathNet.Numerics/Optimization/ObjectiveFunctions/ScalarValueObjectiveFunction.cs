// <copyright file="ObjectiveFunction1D.cs" company="Math.NET">
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

namespace Altaxo.Calc.Optimization.ObjectiveFunctions
{
  /// <summary>
  /// Represents a scalar objective function evaluation that stores only the value.
  /// </summary>
  internal class ScalarValueObjectiveFunctionEvaluation : IScalarObjectiveFunctionEvaluation
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ScalarValueObjectiveFunctionEvaluation"/> class.
    /// </summary>
    /// <param name="point">The evaluation point.</param>
    /// <param name="value">The evaluated objective value.</param>
    public ScalarValueObjectiveFunctionEvaluation(double point, double value)
    {
      Point = point;
      Value = value;
    }

    /// <inheritdoc />
    public double Point { get; }
    /// <inheritdoc />
    public double Value { get; }

    /// <inheritdoc />
    public double Derivative => throw new NotSupportedException();

    /// <inheritdoc />
    public double SecondDerivative => throw new NotSupportedException();
  }

  /// <summary>
  /// Represents a scalar objective function that evaluates only the value.
  /// </summary>
  internal class ScalarValueObjectiveFunction : IScalarObjectiveFunction
  {
    /// <summary>
    /// Gets the objective function delegate.
    /// </summary>
    public Func<double, double> Objective { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScalarValueObjectiveFunction"/> class.
    /// </summary>
    /// <param name="objective">The objective function delegate.</param>
    public ScalarValueObjectiveFunction(Func<double, double> objective)
    {
      Objective = objective;
    }

    /// <inheritdoc />
    public bool IsDerivativeSupported => false;

    /// <inheritdoc />
    public bool IsSecondDerivativeSupported => false;

    /// <inheritdoc />
    public IScalarObjectiveFunctionEvaluation Evaluate(double point)
    {
      return new ScalarValueObjectiveFunctionEvaluation(point, Objective(point));
    }
  }
}
