// <copyright file="GradientHessianObjectiveFunction.cs" company="Math.NET">
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

namespace Altaxo.Calc.Optimization.ObjectiveFunctions
{
  /// <summary>
  /// Represents an objective function that evaluates the value, gradient, and Hessian together.
  /// </summary>
  internal class GradientHessianObjectiveFunction : IObjectiveFunction
  {
    private readonly Func<Vector<double>, (double, Vector<double>, Matrix<double>)> _function;

    /// <summary>
    /// Initializes a new instance of the <see cref="GradientHessianObjectiveFunction"/> class.
    /// </summary>
    /// <param name="function">The delegate that evaluates the value, gradient, and Hessian.</param>
    public GradientHessianObjectiveFunction(Func<Vector<double>, (double, Vector<double>, Matrix<double>)> function)
    {
      _function = function;
    }

    /// <inheritdoc />
    public IObjectiveFunction CreateNew()
    {
      return new GradientHessianObjectiveFunction(_function);
    }

    /// <inheritdoc />
    public IObjectiveFunction Fork()
    {
      // no need to deep-clone values since they are replaced on evaluation
      return new GradientHessianObjectiveFunction(_function)
      {
        Point = Point,
        Value = Value,
        Gradient = Gradient,
        Hessian = Hessian
      };
    }

    /// <inheritdoc />
    public bool IsGradientSupported => true;

    /// <inheritdoc />
    public bool IsHessianSupported => true;

    /// <inheritdoc />
    public void EvaluateAt(Vector<double> point)
    {
      Point = point;
      (Value, Gradient, Hessian) = _function(point);
    }

    /// <inheritdoc />
    public Vector<double> Point { get; private set; }
    /// <inheritdoc />
    public double Value { get; private set; }
    /// <inheritdoc />
    public Vector<double> Gradient { get; private set; }
    /// <inheritdoc />
    public Matrix<double> Hessian { get; private set; }
  }
}
