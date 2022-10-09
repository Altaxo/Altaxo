using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Optimization
{
  public class NonlinearMinimizationResult
  {
    public IObjectiveModel ModelInfoAtMinimum { get; }

    /// <summary>
    /// Returns the best fit parameters.
    /// </summary>
    public Vector<double> MinimizingPoint => ModelInfoAtMinimum.Point;

    /// <summary>
    /// Returns the standard errors of the corresponding parameters
    /// </summary>
    public Vector<double> StandardErrors { get; private set; }

    /// <summary>
    /// Returns the y-values of the fitted model that correspond to the independent values.
    /// </summary>
    public Vector<double> MinimizedValues => ModelInfoAtMinimum.ModelValues;

    /// <summary>
    /// Returns the covariance matrix at minimizing point.
    /// </summary>
    public Matrix<double> Covariance { get; private set; }

    /// <summary>
    ///  Returns the correlation matrix at minimizing point.
    /// </summary>
    public Matrix<double> Correlation { get; private set; }

    public int Iterations { get; }

    public ExitCondition ReasonForExit { get; }

    /// <summary>
    /// Gets for each parameter, whether it is fixed either because it was fixed by the user, or because it is stuck at a boundary.
    /// </summary>
    public IReadOnlyList<bool> IsFixedByUserOrBoundaries { get; }

    public NonlinearMinimizationResult(IObjectiveModel modelInfo, int iterations, ExitCondition reasonForExit, IReadOnlyList<bool> isFixed = null)
    {
      ModelInfoAtMinimum = modelInfo;
      Iterations = iterations;
      ReasonForExit = reasonForExit;
      IsFixedByUserOrBoundaries = isFixed == null ? Enumerable.Repeat(false, modelInfo.Point.Count).ToImmutableArray() : isFixed.ToImmutableArray();

      EvaluateCovariance(modelInfo, isFixed);
    }

    private void EvaluateCovariance(IObjectiveModel objective, IReadOnlyList<bool> isFixed)
    {
      objective.EvaluateAt(objective.Point); // Hessian may be not yet updated.

      var Hessian = objective.Hessian;
      if (Hessian == null || objective.DegreeOfFreedom < 1)
      {
        Covariance = null;
        Correlation = null;
        StandardErrors = null;
        return;
      }

      Covariance = Hessian.PseudoInverse() * objective.Value / objective.DegreeOfFreedom;

      if (Covariance != null)
      {
        if (isFixed != null)
        {
          for (int i = 0; i < Covariance.RowCount; ++i)
          {
            if (isFixed[i])
            {
              Covariance.ClearRow(i);
              Covariance.ClearColumn(i);
            }
          }
        }


        StandardErrors = Covariance.Diagonal().PointwiseMaximum(0).PointwiseSqrt();

        var correlation = Covariance.Clone();
        var d = correlation.Diagonal().PointwiseMaximum(0).PointwiseSqrt();
        var dd = d.OuterProduct(d);
        dd.PointwiseMaximum(double.Epsilon, dd); // avoid division by 0, when the parameter is fixed so that dd element is zero
        Correlation = correlation.PointwiseDivide(dd);
      }
      else
      {
        StandardErrors = null;
        Correlation = null;
      }
    }
  }
}
