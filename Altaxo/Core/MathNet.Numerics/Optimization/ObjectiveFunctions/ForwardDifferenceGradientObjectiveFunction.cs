// <copyright file="ForwardDifferenceGradientObjectiveFunction.cs" company="Math.NET">
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
  /// Adapts an objective function with only value implemented
  /// to provide a gradient as well. Gradient calculation is
  /// done using the finite difference method, specifically
  /// forward differences.
  ///
  /// For each gradient computed, the algorithm requires an
  /// additional number of function evaluations equal to the
  /// functions's number of input parameters.
  /// </summary>
  public class ForwardDifferenceGradientObjectiveFunction : IObjectiveFunction
  {
    /// <summary>
    /// Gets or sets the wrapped objective function.
    /// </summary>
    public IObjectiveFunction InnerObjectiveFunction { get; protected set; }
    /// <summary>
    /// Gets or sets the lower bounds.
    /// </summary>
    protected Vector<double> LowerBound { get; set; }
    /// <summary>
    /// Gets or sets the upper bounds.
    /// </summary>
    protected Vector<double> UpperBound { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the objective value has been evaluated.
    /// </summary>
    protected bool ValueEvaluated { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the gradient has been evaluated.
    /// </summary>
    protected bool GradientEvaluated { get; set; }

    private Vector<double> _gradient;

    /// <summary>
    /// Gets or sets the minimum forward-difference increment.
    /// </summary>
    public double MinimumIncrement { get; set; }
    /// <summary>
    /// Gets or sets the relative forward-difference increment.
    /// </summary>
    public double RelativeIncrement { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ForwardDifferenceGradientObjectiveFunction"/> class.
    /// </summary>
    /// <param name="valueOnlyObj">The objective function that provides only values.</param>
    /// <param name="lowerBound">The lower bounds.</param>
    /// <param name="upperBound">The upper bounds.</param>
    /// <param name="relativeIncrement">The relative forward-difference increment.</param>
    /// <param name="minimumIncrement">The minimum forward-difference increment.</param>
    public ForwardDifferenceGradientObjectiveFunction(IObjectiveFunction valueOnlyObj, Vector<double> lowerBound, Vector<double> upperBound, double relativeIncrement = 1e-5, double minimumIncrement = 1e-8)
    {
      InnerObjectiveFunction = valueOnlyObj;
      LowerBound = lowerBound;
      UpperBound = upperBound;
      _gradient = new LinearAlgebra.Double.DenseVector(LowerBound.Count);
      RelativeIncrement = relativeIncrement;
      MinimumIncrement = minimumIncrement;
    }

    /// <summary>
    /// Marks the objective value as evaluated.
    /// </summary>
    protected void EvaluateValue()
    {
      ValueEvaluated = true;
    }

    /// <summary>
    /// Evaluates the gradient by forward differences.
    /// </summary>
    protected void EvaluateGradient()
    {
      if (!ValueEvaluated)
        EvaluateValue();

      var tmpPoint = Point.Clone();
      var tmpObj = InnerObjectiveFunction.CreateNew();
      for (int ii = 0; ii < _gradient.Count; ++ii)
      {
        var origPoint = tmpPoint[ii];
        var relIncr = origPoint * RelativeIncrement;
        var h = Math.Max(relIncr, MinimumIncrement);
        var mult = 1;
        if (origPoint + h > UpperBound[ii])
          mult = -1;

        tmpPoint[ii] = origPoint + mult * h;
        tmpObj.EvaluateAt(tmpPoint);
        double bumpedValue = tmpObj.Value;
        _gradient[ii] = (mult * bumpedValue - mult * InnerObjectiveFunction.Value) / h;

        tmpPoint[ii] = origPoint;
      }
      GradientEvaluated = true;
    }

    /// <inheritdoc/>
    public Vector<double> Gradient
    {
      get
      {
        if (!GradientEvaluated)
          EvaluateGradient();
        return _gradient;
      }
      protected set => _gradient = value;
    }

    /// <inheritdoc/>
    public Matrix<double> Hessian => throw new NotImplementedException();

    /// <inheritdoc/>
    public bool IsGradientSupported => true;

    /// <inheritdoc/>
    public bool IsHessianSupported => false;

    /// <inheritdoc/>
    public Vector<double> Point { get; protected set; }

    /// <inheritdoc/>
    public double Value
    {
      get
      {
        if (!ValueEvaluated)
          EvaluateValue();
        return this.InnerObjectiveFunction.Value;
      }
    }

    /// <inheritdoc/>
    public IObjectiveFunction CreateNew()
    {
      var tmp = new ForwardDifferenceGradientObjectiveFunction(InnerObjectiveFunction.CreateNew(), LowerBound, UpperBound, this.RelativeIncrement, this.MinimumIncrement);
      return tmp;
    }

    /// <inheritdoc/>
    public void EvaluateAt(Vector<double> point)
    {
      Point = point;
      ValueEvaluated = false;
      GradientEvaluated = false;
      InnerObjectiveFunction.EvaluateAt(point);
    }

    /// <inheritdoc/>
    public IObjectiveFunction Fork()
    {
      return new ForwardDifferenceGradientObjectiveFunction(InnerObjectiveFunction.Fork(), LowerBound, UpperBound, this.RelativeIncrement, this.MinimumIncrement)
      {
        Point = Point?.Clone(),
        GradientEvaluated = GradientEvaluated,
        ValueEvaluated = ValueEvaluated,
        _gradient = _gradient?.Clone()
      };
    }
  }
}
