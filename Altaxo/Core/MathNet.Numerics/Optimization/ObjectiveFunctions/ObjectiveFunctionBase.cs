// <copyright file="ObjectiveFunctionBase.cs" company="Math.NET">
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

namespace Altaxo.Calc.Optimization.ObjectiveFunctions
{
  /// <summary>
  /// Provides a base class for eagerly evaluated objective functions.
  /// </summary>
  public abstract class ObjectiveFunctionBase : IObjectiveFunction
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectiveFunctionBase"/> class.
    /// </summary>
    /// <param name="isGradientSupported">Whether gradients are supported.</param>
    /// <param name="isHessianSupported">Whether Hessians are supported.</param>
    protected ObjectiveFunctionBase(bool isGradientSupported, bool isHessianSupported)
    {
      IsGradientSupported = isGradientSupported;
      IsHessianSupported = isHessianSupported;
    }

    /// <inheritdoc/>
    public abstract IObjectiveFunction CreateNew();

    /// <inheritdoc/>
    public virtual IObjectiveFunction Fork()
    {
      // we need to deep-clone values since they may be updated inplace on evaluation
      ObjectiveFunctionBase objective = (ObjectiveFunctionBase)CreateNew();
      objective.Point = Point == null ? null : Point.Clone();
      objective.Value = Value;
      objective.Gradient = Gradient == null ? null : Gradient.Clone();
      objective.Hessian = Hessian == null ? null : Hessian.Clone();
      return objective;
    }

    /// <inheritdoc/>
    public bool IsGradientSupported { get; }
    /// <inheritdoc/>
    public bool IsHessianSupported { get; }

    /// <inheritdoc/>
    public void EvaluateAt(Vector<double> point)
    {
      Point = point;
      Evaluate();
    }

    /// <summary>
    /// Evaluates the objective function and derivative information.
    /// </summary>
    protected abstract void Evaluate();

    /// <inheritdoc/>
    public Vector<double> Point { get; private set; }
    /// <inheritdoc/>
    public double Value { get; protected set; }
    /// <inheritdoc/>
    public Vector<double> Gradient { get; protected set; }
    /// <inheritdoc/>
    public Matrix<double> Hessian { get; protected set; }
  }
}
