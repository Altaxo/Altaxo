// <copyright file="IObjectiveFunction.cs" company="Math.NET">
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

using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Optimization
{
  /// <summary>
  /// Objective function with a frozen evaluation that must not be changed from the outside.
  /// </summary>
  public interface IObjectiveFunctionEvaluation
  {
    /// <summary>
    /// Creates a new unevaluated and independent copy of this objective function.
    /// </summary>
    /// <returns>A new independent objective function instance.</returns>
    public IObjectiveFunction CreateNew();

    /// <summary>
    /// Gets the point at which the objective function is currently evaluated.
    /// </summary>
    public Vector<double> Point { get; }

    /// <summary>
    /// Gets the objective function value at <see cref="Point"/>.
    /// </summary>
    public double Value { get; }

    /// <summary>
    /// Gets a value indicating whether gradient information is available.
    /// </summary>
    public bool IsGradientSupported { get; }

    /// <summary>
    /// Gets the gradient at <see cref="Point"/>.
    /// </summary>
    public Vector<double> Gradient { get; }

    /// <summary>
    /// Gets a value indicating whether Hessian information is available.
    /// </summary>
    public bool IsHessianSupported { get; }

    /// <summary>
    /// Gets the Hessian matrix at <see cref="Point"/>.
    /// </summary>
    public Matrix<double> Hessian { get; }
  }

  /// <summary>
  /// Objective function with a mutable evaluation.
  /// </summary>
  public interface IObjectiveFunction : IObjectiveFunctionEvaluation
  {
    /// <summary>
    /// Evaluates the objective function at the specified point.
    /// </summary>
    /// <param name="point">The point at which to evaluate the objective function.</param>
    public void EvaluateAt(Vector<double> point);

    /// <summary>
    /// Creates a new independent copy of this objective function, evaluated at the same point.
    /// </summary>
    /// <returns>A new independent objective function instance evaluated at the current point.</returns>
    public IObjectiveFunction Fork();
  }

  /// <summary>
  /// Represents the evaluation of a scalar objective function at a single point.
  /// </summary>
  public interface IScalarObjectiveFunctionEvaluation
  {
    /// <summary>
    /// Gets the point at which the function is evaluated.
    /// </summary>
    public double Point { get; }

    /// <summary>
    /// Gets the function value at <see cref="Point"/>.
    /// </summary>
    public double Value { get; }

    /// <summary>
    /// Gets the first derivative at <see cref="Point"/>.
    /// </summary>
    public double Derivative { get; }

    /// <summary>
    /// Gets the second derivative at <see cref="Point"/>.
    /// </summary>
    public double SecondDerivative { get; }
  }

  /// <summary>
  /// Represents a scalar objective function that can be evaluated at scalar points.
  /// </summary>
  public interface IScalarObjectiveFunction
  {
    /// <summary>
    /// Gets a value indicating whether the first derivative is supported.
    /// </summary>
    public bool IsDerivativeSupported { get; }

    /// <summary>
    /// Gets a value indicating whether the second derivative is supported.
    /// </summary>
    public bool IsSecondDerivativeSupported { get; }

    /// <summary>
    /// Evaluates the scalar objective function at the specified point.
    /// </summary>
    /// <param name="point">The point at which to evaluate the function.</param>
    /// <returns>The evaluation result at the specified point.</returns>
    public IScalarObjectiveFunctionEvaluation Evaluate(double point);
  }
}
