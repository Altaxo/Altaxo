using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Optimization
{
  /// <summary>
  /// Represents the result of a nonlinear minimization, including the minimizing point and derived statistics
  /// such as covariance, correlation, and parameter standard errors.
  /// </summary>
  public class NonlinearMinimizationResult
  {
    /// <summary>
    /// Gets the objective model evaluated at the minimizing point.
    /// </summary>
    public IObjectiveModel ModelInfoAtMinimum { get; }

    /// <summary>
    /// Returns the best fit parameters.
    /// </summary>
    public Vector<double> MinimizingPoint => ModelInfoAtMinimum.Point;

    /// <summary>
    /// Returns the standard errors of the corresponding parameters
    /// </summary>
    public Vector<double>? StandardErrors { get; private set; }

    /// <summary>
    /// Returns the y-values of the fitted model that correspond to the independent values.
    /// </summary>
    public Vector<double> MinimizedValues => ModelInfoAtMinimum.ModelValues;

    /// <summary>
    /// Returns the covariance matrix at minimizing point.
    /// </summary>
    public Matrix<double>? Covariance { get; private set; }

    /// <summary>
    /// Returns the correlation matrix at the minimizing point.
    /// </summary>
    public Matrix<double>? Correlation { get; private set; }

    /// <summary>
    /// Gets the number of iterations performed by the minimizer.
    /// </summary>
    public int Iterations { get; }

    /// <summary>
    /// Gets the exit condition reported by the minimizer.
    /// </summary>
    public ExitCondition ReasonForExit { get; }

    /// <summary>
    /// Gets for each parameter, whether it is fixed either because it was fixed by the user, or because it is stuck at a boundary.
    /// </summary>
    public IReadOnlyList<bool> IsFixedByUserOrBoundaries { get; }

    /// <summary>
    /// Gets the number of free (non-fixed) parameters.
    /// </summary>
    public int NumberOfFreeParameters => IsFixedByUserOrBoundaries.Where(x => !x).Count();

    /// <summary>
    /// Gets the number of fixed parameters (fixed either by the user, or because they are stuck at a boundary).
    /// </summary>
    public int NumberOfFixedParameters => IsFixedByUserOrBoundaries.Where(x => x).Count();

    /// <summary>
    /// Gets the total number of parameters.
    /// </summary>
    public int NumberOfParameters => IsFixedByUserOrBoundaries.Count;

    /// <summary>
    /// Initializes a new instance of the <see cref="NonlinearMinimizationResult"/> class.
    /// </summary>
    /// <param name="modelInfo">The objective model at the minimizing point.</param>
    /// <param name="iterations">The number of iterations performed.</param>
    /// <param name="reasonForExit">The reason for termination of the minimization.</param>
    /// <param name="isFixed">
    /// Optional flags that indicate whether each parameter is fixed either by the user or due to being stuck at a boundary.
    /// </param>
    public NonlinearMinimizationResult(IObjectiveModel modelInfo, int iterations, ExitCondition reasonForExit, IReadOnlyList<bool>? isFixed = null)
    {
      ModelInfoAtMinimum = modelInfo;
      Iterations = iterations;
      ReasonForExit = reasonForExit;
      IsFixedByUserOrBoundaries = isFixed is null ? Enumerable.Repeat(false, modelInfo.Point.Count).ToImmutableArray() : isFixed.ToImmutableArray();

      EvaluateCovariance(modelInfo, isFixed);
    }

    /// <summary>
    /// Computes covariance, correlation, and standard errors based on the Hessian at the minimizing point.
    /// </summary>
    /// <param name="objective">The objective model.</param>
    /// <param name="isFixed">Optional flags that indicate which parameters are fixed.</param>
    private void EvaluateCovariance(IObjectiveModel objective, IReadOnlyList<bool>? isFixed)
    {
      objective.EvaluateAt(objective.Point); // Hessian may be not yet updated.
      var RSS = objective.Value;
      var Hessian = objective.Hessian;
      if (!(RSS >= 0) || Hessian is null || objective.DegreeOfFreedom < 1)
      {
        Covariance = null;
        Correlation = null;
        StandardErrors = null;
        return;
      }

      Covariance = PseudoInverseWithScaling(Hessian) * objective.Value / objective.DegreeOfFreedom;


      if (isFixed is not null)
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

    /// <summary>
    /// Computes the Moore-Penrose pseudo-inverse of the provided matrix.
    /// This method takes care that the pseudo-inverse is calculated correctly even if the provided matrix has diagonal
    /// elements that differ by orders of magnitude.
    /// This is often the case when the parameters of the fit have very different orders of magnitude.
    /// </summary>
    public static Matrix<double> PseudoInverseWithScaling(Matrix<double> m)
    {
      // 1st scale the matrix m : scaledM = S*m*S
      // where S is a diagonal matrix, consisting of 1/Sqrt of the diagonal elements of m
      var S = CreateMatrix.Diagonal<double>(m.RowCount, m.ColumnCount, (i) => m[i, i] != 0 ? 1 / Math.Sqrt(Math.Abs(m[i, i])) : 1);
      var scaledTemp = S * m; // TODO: if there is a BLAS function to multiply a diagonal from left and right, then use that instead
      var scaledM = scaledTemp * S;

      // scaledM now contains only 1 or -1 or 0 in the diagonal elements
      // thus we can safely calculate the PseudoInverse
      var ps = scaledM.PseudoInverse();

      // after doing the PseudoInverse, we have to rescale again, using the same scaling matrix
      // result = S*PseudoInverse*S
      S.Multiply(ps, scaledTemp);
      scaledTemp.Multiply(S, scaledM);

      return scaledM;
    }
  }
}
