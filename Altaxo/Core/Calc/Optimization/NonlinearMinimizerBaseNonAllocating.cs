#region Copyright

/////////////////////////////////////////////////////////////////////////////
// Altaxo:  a data processing and data plotting program
// Copyright (c) 2009-2010 Math.NET
// Copyright (C) 2022-2022 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using System.Linq;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Optimization
{
  public abstract class NonlinearMinimizerBaseNonAllocating
  {
    /// <summary>
    /// The default function tolerance. Since function tolerance is an absolute value and thus dependends on the scale of the y-values,
    /// the default value for it is zero.
    /// </summary>
    public const double DefaultFunctionTolerance = 0;

    /// <summary>
    /// The default gradient tolerance. Since the gradient tolerance is an absolute value and thus depends on the scale of the y-values, and the
    /// scale of the parameters, its default value is zero.
    /// </summary>
    public const double DefaultGradientTolerance = 0;

    /// <summary>
    /// The default step tolerance. This is a relative value (ratio of current step and value of the parameter).
    /// </summary>
    public const double DefaultStepTolerance = 1E-16;

    /// <summary>
    /// The default value for the minimal RSS (Chi²) improvement achieved during 8 iterations.
    /// </summary>
    public const double DefaultMinimalRSSImprovement = 1E-6;


    /// <summary>
    /// The stopping threshold for the function value or L2 norm of the residuals.
    /// </summary>
    public double FunctionTolerance { get; set; } = DefaultFunctionTolerance;

    /// <summary>
    /// The stopping threshold for L2 norm of the change of the parameters.
    /// </summary>
    public double StepTolerance { get; set; } = DefaultStepTolerance;

    /// <summary>
    /// The stopping threshold for infinity norm of the gradient.
    /// </summary>
    public double GradientTolerance { get; set; } = DefaultGradientTolerance;

    /// <summary>
    /// The maximum number of iterations. If null, the maximal number of iterations is determined automatically.
    /// </summary>
    public int? MaximumIterations { get; set; }

    /// <summary>
    /// Gets or sets the minimal RSS improvement.
    /// </summary>
    /// <value>
    /// The minimal RSS (Chi²) improvement. If after 8 iterations the improvement is smaller than this value, the evaluation is stopped.
    /// </value>
    public double MinimalRSSImprovement { get; set; } = DefaultMinimalRSSImprovement;

    /// <summary>
    /// The lower bound of the parameters.
    /// </summary>
    public IReadOnlyList<double?>? LowerBound { get; private set; }

    /// <summary>
    /// The upper bound of the parameters.
    /// </summary>
    public IReadOnlyList<double?>? UpperBound { get; private set; }

    /// <summary>
    /// The scale factors for the parameters.
    /// </summary>
    public Vector<double>? Scales { get; protected set; }

    protected bool IsBounded => LowerBound is not null || UpperBound is not null || Scales is not null;

    protected void ValidateBounds(IReadOnlyList<double> parameters, IReadOnlyList<double?> lowerBound = null, IReadOnlyList<double?> upperBound = null, IReadOnlyList<double> scales = null)
    {
      if (parameters is null)
      {
        throw new ArgumentNullException(nameof(parameters));
      }

      // test if parameters are numbers
      for (int i = 0; i < parameters.Count; i++)
      {
        if (double.IsNaN(parameters[i]))
        {
          throw new ArgumentException($"Initial parameter[{i}] is not a number (is NaN).");
        }
      }

      if (lowerBound is not null && lowerBound.Count(x => x.HasValue && (double.IsInfinity(x.Value) || double.IsNaN(x.Value))) > 0)
      {
        throw new ArgumentException("The lower bounds must be finite.");
      }
      if (lowerBound is not null && lowerBound.Count != parameters.Count)
      {
        throw new ArgumentException("The lower bounds can't have different size from the parameters.");
      }
      if (lowerBound is not null && lowerBound.Count(x => x.HasValue && x.Value > double.MinValue) > 0)
      {
        LowerBound = lowerBound;
      }
      else
      {
        LowerBound = null;
      }

      // test if parameters >= lowerBound
      if (LowerBound is not null)
      {
        for (int i = 0; i < LowerBound.Count; i++)
        {
          if (LowerBound[i].HasValue && !(parameters[i] >= LowerBound[i].Value))
          {
            throw new ArgumentException($"Initial value of parameter[{i}]={parameters[i]} violates the lower bound criterion LowerBound={LowerBound[i].Value}");
          }
        }
      }

      if (upperBound is not null && upperBound.Count(x => x.HasValue && (double.IsInfinity(x.Value) || double.IsNaN(x.Value))) > 0)
      {
        throw new ArgumentException("The upper bounds must be finite.");
      }
      if (upperBound is not null && upperBound.Count != parameters.Count)
      {
        throw new ArgumentException("The upper bounds can't have different size from the parameters.");
      }
      if (upperBound is not null && upperBound.Count(x => x.HasValue && x.Value < double.MaxValue) > 0)
      {
        UpperBound = upperBound;
      }
      else
      {
        UpperBound = null;
      }

      // test if parameters >= lowerBound
      if (UpperBound is not null)
      {
        for (int i = 0; i < UpperBound.Count; i++)
        {
          if (UpperBound[i].HasValue && !(parameters[i] <= UpperBound[i].Value))
          {
            throw new ArgumentException($"Initial value of parameter[{i}]={parameters[i]} violates the upper bound criterion UpperBound={UpperBound[i].Value}");
          }
        }
      }

      if (scales is not null && scales.Count(x => double.IsInfinity(x) || double.IsNaN(x) || x == 0) > 0)
      {
        throw new ArgumentException("The scales must be finite.");
      }
      if (scales is not null && scales.Count != parameters.Count)
      {
        throw new ArgumentException("The scales can't have different size from the parameters.");
      }
      if (scales is not null && scales.Count(x => x < 0) > 0)
      {
        var newScales = Vector<double>.Build.DenseOfEnumerable(scales);
        newScales.PointwiseAbs();
        Scales = newScales;
      }
      else if (scales is not null)
      {
        Scales = Vector<double>.Build.DenseOfEnumerable(scales);
      }
    }

    protected double EvaluateFunction(IObjectiveModelNonAllocating objective, IReadOnlyList<double> Pint, IVector<double> pExt)
    {

      ProjectToExternalParameters(Pint, pExt);
      objective.EvaluateAt(pExt);
      return objective.Value;
    }

    /// <summary>
    /// Evaluates the jacobian, and the hessian of the objective function.
    /// </summary>
    /// <param name="objective">The objective.</param>
    /// <param name="pInt">The parameters (internal representation).</param>
    /// <returns>The negative gradient and the hessian.</returns>
    protected (Vector<double> NegativeGradient, Matrix<double> Hessian) EvaluateJacobian(IObjectiveModelNonAllocating objective, IReadOnlyList<double> pInt, Vector<double> scaleFactors)
    {
      var negativeGradient = objective.NegativeGradient;
      var hessian = objective.Hessian;

      if (IsBounded)
      {
        ScaleFactorsOfJacobian(pInt, scaleFactors); // the parameters argument is always internal.

        for (int i = 0; i < negativeGradient.Count; i++)
        {
          negativeGradient[i] *= scaleFactors[i];
        }

        for (int i = 0; i < hessian.RowCount; i++)
        {
          var scaleFactor_i = scaleFactors[i];
          for (int j = 0; j < hessian.ColumnCount; j++)
          {
            hessian[i, j] = (hessian[i, j] * scaleFactor_i) * scaleFactors[j]; // the evaluation order forced by the braces will avoid overflow or underflow if scaleFactors[i]*scaleFactors[j] is too big or too small
          }
        }
      }

      return (negativeGradient, hessian);
    }

    #region Projection of Parameters

    // To handle the box constrained minimization as the unconstrained minimization,
    // the parameters are mapping by the following rules,
    // which are modified the rules shown in the ref[1] in order to introduce scales.
    //
    // 1. lower < Pext < upper
    //    Pint = asin(2 * (Pext - lower) / (upper - lower) - 1)
    //    Pext = lower + (sin(Pint) + 1) * (upper - lower) / 2
    //    dPext/dPint = (upper - lower) / 2 * cos(Pint)
    //
    // 2. lower < Pext
    //    Pint = sqrt((Pext/scale - lower/scale + 1)^2 - 1)
    //    Pext = lower + scale * (sqrt(Pint^2 + 1) - 1)
    //    dPext/dPint = scale * Pint / sqrt(Pint^2 + 1)
    //
    // 3. Pext < upper
    //    Pint = sqrt((upper / scale - Pext / scale + 1)^2 - 1)
    //    Pext = upper + scale - scale * sqrt(Pint^2 + 1)
    //    dPext/dPint = - scale * Pint / sqrt(Pint^2 + 1)
    //
    // 4. no bounds, but scales
    //    Pint = Pext / scale
    //    Pext = Pint * scale
    //    dPext/dPint = scale
    //
    // The rules are applied in ProjectParametersToInternal, ProjectParametersToExternal, and ScaleFactorsOfJacobian methods.
    //
    // References:
    // [1] https://lmfit.github.io/lmfit-py/bounds.html
    // [2] MINUIT User's Guide, https://root.cern.ch/download/minuit.pdf
    //
    // Except when it is initial guess, the parameters argument is always internal parameter.
    // So, first map the parameters argument to the external parameters in order to calculate function values.

    protected void ProjectToInternalParameters(IReadOnlyList<double> Pext, IVector<double> Pint)
    {
      if (LowerBound is not null || UpperBound is not null)
      {
        for (int i = 0; i < Pint.Count; i++)
        {
          var lowerBnd = LowerBound?.ElementAt(i);
          var upperBnd = UpperBound?.ElementAt(i);
          double pint;

          if (lowerBnd.HasValue && upperBnd.HasValue)
          {
            pint = Math.Asin((2 * Pext[i] - lowerBnd.Value - upperBnd.Value) / (upperBnd.Value - lowerBnd.Value));
          }
          else if (lowerBnd.HasValue)
          {
            pint = Math.Sqrt(RMath.Pow2((Pext[i] - lowerBnd.Value) + 1.0) - 1.0);
          }
          else if (upperBnd.HasValue)
          {
            pint = Math.Sqrt(RMath.Pow2((upperBnd.Value - Pext[i]) + 1.0) - 1.0);

          }
          else
          {
            pint = Pext[i];
          }

          Pint[i] = pint / (Scales?.ElementAt(i) ?? 1);
        }
      }
      else
      {
        for (int i = 0; i < Pint.Count; i++)
        {
          Pint[i] = Pext[i] / (Scales?.ElementAt(i) ?? 1);
        }
      }
    }

    /// <summary>
    /// Projects internal to external parameters.
    /// </summary>
    /// <param name="Pint">The internal parameters.</param>
    /// <param name="Pext">On return, contains the external parameters.</param>
    protected void ProjectToExternalParameters(IReadOnlyList<double> Pint, IVector<double> Pext)
    {
      if (LowerBound is not null || UpperBound is not null)
      {

        for (int i = 0; i < Pint.Count; i++)
        {
          double scale_pint = (Scales?.ElementAt(i) ?? 1) * Pint[i];
          var lowerBnd = LowerBound?.ElementAt(i);
          var upperBnd = UpperBound?.ElementAt(i);

          if (lowerBnd.HasValue && upperBnd.HasValue)
          {
            Pext[i] = lowerBnd.Value + (upperBnd.Value / 2.0 - lowerBnd.Value / 2.0) * (Math.Sin(scale_pint) + 1.0);
          }
          else if (lowerBnd.HasValue)
          {
            Pext[i] = lowerBnd.Value + (Math.Sqrt(scale_pint * scale_pint + 1.0) - 1.0);
          }
          else if (upperBnd.HasValue)
          {
            Pext[i] = upperBnd.Value - (Math.Sqrt(scale_pint * scale_pint + 1.0) - 1.0);
          }
          else
          {
            Pext[i] = scale_pint;
          }
        }
      }
      else
      {
        for (int i = 0; i < Pint.Count; i++)
        {
          Pext[i] = Pint[i] * (Scales?.ElementAt(i) ?? 1);
        }
      }
    }

    /// <summary>
    /// Calculates the scale factor of the jacobian, taking into account the parameter transformations , and the parameter scales.
    /// </summary>
    /// <param name="Pint">The pint.</param>
    /// <param name="result">On return, contains the scale factors. The provided vector needs to have the same length as <paramref name="Pint"/></param>
    protected void ScaleFactorsOfJacobian(IReadOnlyList<double> Pint, IVector<double> result)
    {
      if (LowerBound is not null || UpperBound is not null)
      {
        for (int i = 0; i < Pint.Count; i++)
        {
          var lowerBnd = LowerBound?.ElementAt(i);
          var upperBnd = UpperBound?.ElementAt(i);
          var scale_i = Scales?.ElementAt(i) ?? 1;
          var pint_scale = Pint[i] * scale_i;
          double result_i;

          if (lowerBnd.HasValue && upperBnd.HasValue)
          {
            result_i = 0.5 * (upperBnd.Value - lowerBnd.Value) * Math.Cos(pint_scale);
          }
          else if (upperBnd.HasValue)
          {
            result_i = -pint_scale / Math.Sqrt(pint_scale * pint_scale + 1.0);
          }
          else if (lowerBnd.HasValue)
          {
            result_i = pint_scale / Math.Sqrt(pint_scale * pint_scale + 1.0);
          }
          else
          {
            result_i = 1;
          }
          result[i] = scale_i * result_i;
        }
      }
      else
      {
        for (int i = 0; i < Pint.Count; i++)
        {
          result[i] = Scales?.ElementAt(i) ?? 1;
        }
      }
    }

    #endregion Projection of Parameters
  }
}
