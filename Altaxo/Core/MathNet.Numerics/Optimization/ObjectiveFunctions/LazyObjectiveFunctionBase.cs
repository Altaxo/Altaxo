// <copyright file="LazyObjectiveFunctionBase.cs" company="Math.NET">
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
  /// Provides a base class for objective functions with lazy evaluation.
  /// </summary>
  public abstract class LazyObjectiveFunctionBase : IObjectiveFunction
  {
    private Vector<double> _point;

    /// <summary>
    /// Gets or sets a value indicating whether the function value is available.
    /// </summary>
    protected bool HasFunctionValue { get; set; }
    /// <summary>
    /// Gets or sets the cached function value.
    /// </summary>
    protected double FunctionValue { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the gradient is available.
    /// </summary>
    protected bool HasGradientValue { get; set; }
    /// <summary>
    /// Gets or sets the cached gradient.
    /// </summary>
    protected Vector<double> GradientValue { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the Hessian is available.
    /// </summary>
    protected bool HasHessianValue { get; set; }
    /// <summary>
    /// Gets or sets the cached Hessian.
    /// </summary>
    protected Matrix<double> HessianValue { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LazyObjectiveFunctionBase"/> class.
    /// </summary>
    /// <param name="gradientSupported">Whether gradients are supported.</param>
    /// <param name="hessianSupported">Whether Hessians are supported.</param>
    protected LazyObjectiveFunctionBase(bool gradientSupported, bool hessianSupported)
    {
      IsGradientSupported = gradientSupported;
      IsHessianSupported = hessianSupported;
    }

    /// <inheritdoc/>
    public abstract IObjectiveFunction CreateNew();

    /// <inheritdoc/>
    public virtual IObjectiveFunction Fork()
    {
      // we need to deep-clone values since they may be updated inplace on evaluation
      LazyObjectiveFunctionBase fork = (LazyObjectiveFunctionBase)CreateNew();
      fork._point = _point?.Clone();
      fork.HasFunctionValue = HasFunctionValue;
      fork.FunctionValue = FunctionValue;
      fork.HasGradientValue = HasGradientValue;
      fork.GradientValue = GradientValue?.Clone();
      fork.HasHessianValue = HasHessianValue;
      fork.HessianValue = HessianValue?.Clone();
      return fork;
    }

    /// <inheritdoc/>
    public bool IsGradientSupported { get; }
    /// <inheritdoc/>
    public bool IsHessianSupported { get; }

    /// <inheritdoc/>
    public void EvaluateAt(Vector<double> point)
    {
      _point = point;
      HasFunctionValue = false;
      HasGradientValue = false;
      HasHessianValue = false;
    }

    /// <summary>
    /// Evaluates the function value.
    /// </summary>
    protected abstract void EvaluateValue();

    /// <summary>
    /// Evaluates the gradient.
    /// </summary>
    protected virtual void EvaluateGradient()
    {
      Gradient = null;
    }

    /// <summary>
    /// Evaluates the Hessian.
    /// </summary>
    protected virtual void EvaluateHessian()
    {
      Hessian = null;
    }

    /// <inheritdoc/>
    public Vector<double> Point => _point;

    /// <inheritdoc/>
    public double Value
    {
      get
      {
        if (!HasFunctionValue)
        {
          EvaluateValue();
        }
        return FunctionValue;
      }
      protected set
      {
        FunctionValue = value;
        HasFunctionValue = true;
      }
    }

    /// <inheritdoc/>
    public Vector<double> Gradient
    {
      get
      {
        if (!HasGradientValue)
        {
          EvaluateGradient();
        }
        return GradientValue;
      }
      protected set
      {
        GradientValue = value;
        HasGradientValue = true;
      }
    }

    /// <inheritdoc/>
    public Matrix<double> Hessian
    {
      get
      {
        if (!HasHessianValue)
        {
          EvaluateHessian();
        }
        return HessianValue;
      }
      protected set
      {
        HessianValue = value;
        HasHessianValue = true;
      }
    }
  }
}
