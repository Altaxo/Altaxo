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
  /// Represents a lazily evaluated scalar objective function result.
  /// </summary>
  internal class LazyScalarObjectiveFunctionEvaluation : IScalarObjectiveFunctionEvaluation
  {
    private double? _value;
    private double? _derivative;
    private double? _secondDerivative;
    private readonly ScalarObjectiveFunction _objectiveObject;
    private readonly double _point;

    /// <summary>
    /// Initializes a new instance of the <see cref="LazyScalarObjectiveFunctionEvaluation"/> class.
    /// </summary>
    /// <param name="f">The objective function definition.</param>
    /// <param name="point">The evaluation point.</param>
    public LazyScalarObjectiveFunctionEvaluation(ScalarObjectiveFunction f, double point)
    {
      _objectiveObject = f;
      _point = point;
    }

    private double SetValue()
    {
      _value = _objectiveObject.Objective(_point);
      return _value.Value;
    }

    private double SetDerivative()
    {
      _derivative = _objectiveObject.Derivative(_point);
      return _derivative.Value;
    }

    private double SetSecondDerivative()
    {
      _secondDerivative = _objectiveObject.SecondDerivative(_point);
      return _secondDerivative.Value;
    }

    /// <inheritdoc />
    public double Point => _point;
    /// <inheritdoc />
    public double Value => _value ?? SetValue();
    /// <inheritdoc />
    public double Derivative => _derivative ?? SetDerivative();
    /// <inheritdoc />
    public double SecondDerivative => _secondDerivative ?? SetSecondDerivative();
  }

  /// <summary>
  /// Represents a scalar objective function with optional first and second derivatives.
  /// </summary>
  internal class ScalarObjectiveFunction : IScalarObjectiveFunction
  {
    /// <summary>
    /// Gets the objective function delegate.
    /// </summary>
    public Func<double, double> Objective { get; }
    /// <summary>
    /// Gets the first derivative delegate.
    /// </summary>
    public Func<double, double> Derivative { get; }
    /// <summary>
    /// Gets the second derivative delegate.
    /// </summary>
    public Func<double, double> SecondDerivative { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScalarObjectiveFunction"/> class.
    /// </summary>
    /// <param name="objective">The objective function delegate.</param>
    public ScalarObjectiveFunction(Func<double, double> objective)
    {
      Objective = objective;
      Derivative = null;
      SecondDerivative = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScalarObjectiveFunction"/> class.
    /// </summary>
    /// <param name="objective">The objective function delegate.</param>
    /// <param name="derivative">The first derivative delegate.</param>
    public ScalarObjectiveFunction(Func<double, double> objective, Func<double, double> derivative)
    {
      Objective = objective;
      Derivative = derivative;
      SecondDerivative = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScalarObjectiveFunction"/> class.
    /// </summary>
    /// <param name="objective">The objective function delegate.</param>
    /// <param name="derivative">The first derivative delegate.</param>
    /// <param name="secondDerivative">The second derivative delegate.</param>
    public ScalarObjectiveFunction(Func<double, double> objective, Func<double, double> derivative, Func<double, double> secondDerivative)
    {
      Objective = objective;
      Derivative = derivative;
      SecondDerivative = secondDerivative;
    }

    /// <inheritdoc />
    public bool IsDerivativeSupported => Derivative != null;

    /// <inheritdoc />
    public bool IsSecondDerivativeSupported => SecondDerivative != null;

    /// <inheritdoc />
    public IScalarObjectiveFunctionEvaluation Evaluate(double point)
    {
      return new LazyScalarObjectiveFunctionEvaluation(this, point);
    }
  }
}
