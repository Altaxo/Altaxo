using System;
using System.Collections.Generic;
using System.Linq;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Optimization
{
  public interface IObjectiveModelNonAllocating : IObjectiveModel
  {
    void SetParameters(IReadOnlyList<double> parameters, IReadOnlyList<bool> isFixed);

    /// <summary>
    /// Evaluates the ChiSquare value (i.e. the sum of squares of deviations between data and fitmodel).
    /// </summary>
    /// <param name="parameter">The parameter.</param>
    /// <returns>The ChiSquare value.</returns>
    void EvaluateAt(IReadOnlyList<double> parameter);

    /// <summary>
    /// Get the negative gradient vector. -G = -J'(y - f(x; p))
    /// </summary>
    Vector<double> NegativeGradient { get; }
  }


  public abstract class NonlinearMinimizerBaseNonAllocating
  {
    /// <summary>
    /// The stopping threshold for the function value or L2 norm of the residuals.
    /// </summary>
    public double FunctionTolerance { get; set; }

    /// <summary>
    /// The stopping threshold for L2 norm of the change of the parameters.
    /// </summary>
    public double StepTolerance { get; set; }

    /// <summary>
    /// The stopping threshold for infinity norm of the gradient.
    /// </summary>
    public double GradientTolerance { get; set; }

    /// <summary>
    /// The maximum number of iterations.
    /// </summary>
    public int MaximumIterations { get; set; }

    /// <summary>
    /// The lower bound of the parameters.
    /// </summary>
    public IReadOnlyList<double?> LowerBound { get; private set; }

    /// <summary>
    /// The upper bound of the parameters.
    /// </summary>
    public IReadOnlyList<double?> UpperBound { get; private set; }

    /// <summary>
    /// The scale factors for the parameters.
    /// </summary>
    public IReadOnlyList<double> Scales { get; private set; }

    protected bool IsBounded => LowerBound is not null || UpperBound is not null || Scales is not null;

    /// <summary>
    /// The external parameters (i.e. the parameters seen by the model; maybe bounded).
    /// </summary>
    protected Vector<double> _pExt;

    /// <summary>
    /// The internal parameters (i.e. the parameters used by the algorithm; unbounded).
    /// </summary>
    protected Vector<double> _pInt;

    protected Vector<double> _diagonalOfHessian;
    protected Vector<double> _diagonalOfHessianPlusMu;

    /// <summary>
    /// The scale factors
    /// </summary>
    protected Vector<double> _scaleFactors;


    protected NonlinearMinimizerBaseNonAllocating(double gradientTolerance = 1E-18, double stepTolerance = 1E-18, double functionTolerance = 1E-18, int maximumIterations = -1)
    {
      GradientTolerance = gradientTolerance;
      StepTolerance = stepTolerance;
      FunctionTolerance = functionTolerance;
      MaximumIterations = maximumIterations;
    }

    protected void ValidateBounds(IReadOnlyList<double> parameters, IReadOnlyList<double?> lowerBound = null, IReadOnlyList<double?> upperBound = null, IReadOnlyList<double> scales = null)
    {
      if (parameters is null)
      {
        throw new ArgumentNullException(nameof(parameters));
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
        scales = newScales;
      }
      Scales = scales;

    }

    protected double EvaluateFunction(IObjectiveModelNonAllocating objective, IReadOnlyList<double> Pint)
    {

      ProjectToExternalParameters(Pint, _pExt);
      objective.EvaluateAt(_pExt);
      return objective.Value;
    }

    /// <summary>
    /// Evaluates the jacobian, and the hessian of the objective function.
    /// </summary>
    /// <param name="objective">The objective.</param>
    /// <param name="Pint">The parameters (internal representation).</param>
    /// <returns></returns>
    protected (Vector<double> NegativeGradient, Matrix<double> Hessian) EvaluateJacobian(IObjectiveModelNonAllocating objective, IReadOnlyList<double> pInt, IReadOnlyList<double> pExt)
    {
      var negativeGradient = objective.NegativeGradient;
      var hessian = objective.Hessian;

      if (IsBounded)
      {
        ScaleFactorsOfJacobian(pInt, _scaleFactors); // the parameters argument is always internal.

        for (int i = 0; i < negativeGradient.Count; i++)
        {
          negativeGradient[i] = negativeGradient[i] * _scaleFactors[i];
        }

        for (int i = 0; i < hessian.RowCount; i++)
        {
          for (int j = 0; j < hessian.ColumnCount; j++)
          {
            hessian[i, j] = hessian[i, j] * _scaleFactors[i] * _scaleFactors[j];
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

          if (lowerBnd.HasValue && upperBnd.HasValue)
          {
            Pint[i] = Math.Asin((2.0 * (Pext[i] - lowerBnd.Value) / (upperBnd.Value - lowerBnd.Value)) - 1.0);
          }
          else if (lowerBnd.HasValue)
          {
            Pint[i] = (Scales is null)
              ? Math.Sqrt(RMath.Pow2(Pext[i] - lowerBnd.Value + 1.0) - 1.0)
              : Math.Sqrt(RMath.Pow2((Pext[i] - lowerBnd.Value) / Scales[i] + 1.0) - 1.0);
          }
          else if (upperBnd.HasValue)
          {
            Pint[i] = (Scales is null)
              ? Math.Sqrt(RMath.Pow2(upperBnd.Value - Pext[i] + 1.0) - 1.0)
              : Math.Sqrt(RMath.Pow2((upperBnd.Value - Pext[i]) / Scales[i] + 1.0) - 1.0);
          }
          else
          {
            Pint[i] = Pext[i] / (Scales?.ElementAt(i) ?? 1);
          }
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
          var lowerBnd = LowerBound?.ElementAt(i);
          var upperBnd = UpperBound?.ElementAt(i);

          if (lowerBnd.HasValue && upperBnd.HasValue)
          {
            Pext[i] = lowerBnd.Value + (upperBnd.Value / 2.0 - lowerBnd.Value / 2.0) * (Math.Sin(Pint[i]) + 1.0);
          }
          else if (lowerBnd.HasValue)
          {
            Pext[i] = (Scales is null)
              ? lowerBnd.Value + Math.Sqrt(Pint[i] * Pint[i] + 1.0) - 1.0
              : lowerBnd.Value + Scales[i] * (Math.Sqrt(Pint[i] * Pint[i] + 1.0) - 1.0);
          }
          else if (upperBnd.HasValue)
          {
            Pext[i] = (Scales is null)
             ? upperBnd.Value - Math.Sqrt(Pint[i] * Pint[i] + 1.0) + 1.0
             : upperBnd.Value - Scales[i] * (Math.Sqrt(Pint[i] * Pint[i] + 1.0) - 1.0);
          }
          else
          {
            Pext[i] = Pint[i] * (Scales?.ElementAt(i) ?? 1);
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
      var scale = Vector<double>.Build.Dense(Pint.Count, 1.0);


      if (LowerBound is not null || UpperBound is not null)
      {
        for (int i = 0; i < Pint.Count; i++)
        {
          var lowerBnd = LowerBound?.ElementAt(i);
          var upperBnd = UpperBound?.ElementAt(i);

          if (lowerBnd.HasValue && upperBnd.HasValue)
          {
            scale[i] = (upperBnd.Value - lowerBnd.Value) / 2.0 * Math.Cos(Pint[i]);
          }
          else if (upperBnd.HasValue)
          {
            scale[i] = (Scales is null)
              ? -Pint[i] / Math.Sqrt(Pint[i] * Pint[i] + 1.0)
              : -Scales[i] * Pint[i] / Math.Sqrt(Pint[i] * Pint[i] + 1.0);
          }
          else if (lowerBnd.HasValue)
          {
            scale[i] = (Scales is null)
              ? Pint[i] / Math.Sqrt(Pint[i] * Pint[i] + 1.0)
              : Scales[i] * Pint[i] / Math.Sqrt(Pint[i] * Pint[i] + 1.0);
          }
          else
          {
            result[i] = 1;
          }

        }
        return;
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
