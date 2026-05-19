#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Optimization
{
  // =========================================================================
  // Result type
  // =========================================================================

  /// <summary>
  /// Result of a projection onto the feasible set.
  /// </summary>
  public class ProjectionResult
  {
    /// <summary>The projected (feasible) parameter vector.</summary>
    public Vector<double> Parameters { get; init; } = null!;

    /// <summary>
    /// For each parameter index: the value to which the parameter is fixed
    /// throughout the whole feasible set, or <see langword="null"/> if it is not fixed.
    /// </summary>
    public double?[] FixedValues { get; init; } = null!;

    /// <summary>
    /// For each parameter index: true if it is influenced by at least
    /// one active constraint at the projection solution.
    /// </summary>
    public bool[] IsConstrained { get; init; } = null!;

    /// <summary>
    /// Indices of active constraints at the solution, in the merged system:
    ///   [0 .. nEq-1]          → equality rows  (always active)
    ///   [nEq .. nEq+nLeq-1]   → inequality rows (C·x &lt;= d), active when binding
    /// </summary>
    public IReadOnlyList<int> ActiveConstraintIndices { get; init; } = null!;

    /// <summary>Indices of parameters touched by no active constraint.</summary>
    public IReadOnlyList<int> FreeParameterIndices { get; init; } = null!;

    /// <summary>Indices of parameters touched by at least one active constraint.</summary>
    public IReadOnlyList<int> ConstrainedParameterIndices { get; init; } = null!;

    /// <summary>Indices of parameters that are fixed to a single value throughout the feasible set.</summary>
    public IReadOnlyList<int> FixedParameterIndices { get; init; } = null!;
  }

  // =========================================================================
  // Projector
  // =========================================================================

  /// <summary>
  /// Projects a point onto the feasible set of parameters <c>x</c> defined by:
  ///
  ///   A·x  =  b      (equality constraints)
  ///   C·x  &lt;= d     (inequality constraints)
  ///
  /// Any other linear constraint form can be reduced to these two:
  ///   C·x >= d   →   -C·x &lt;= -d
  ///   lb &lt;= x[i]  →   -x[i] &lt;= -lb[i]
  ///   x[i] &lt;= ub  →    x[i] &lt;= ub[i]   (directly)
  ///
  /// The projection solves the convex QP:
  ///   min  ½ ||x - p||²
  ///   s.t. A·x = b,  C·x &lt;= d
  ///
  /// using an Active Set method. The active set at convergence identifies
  /// which constraints are binding and therefore which parameters are
  /// constrained vs. free.
  ///
  /// Requires MathNet.Numerics.
  /// </summary>
  public class LinearConstraintsProjector : IConstraintsProjector
  {
    /// <summary>
    /// Describes how the active-set solver terminated during the last projection.
    /// </summary>
    internal enum ProjectionTerminationReason
    {
      None,
      NoConstraints,
      OptimalStationaryPoint,
      DroppedConstraintWithNegativeMultiplier,
      KktSolveFailed,
      LeastSquaresFallback,
      MaxIterationsReached,
    }

    // Equality:   A·x = b
    /// <summary>
    /// Gets the equality-constraint matrix in <c>A·x = b</c>.
    /// </summary>
    private readonly Matrix<double>? _A;

    /// <summary>
    /// Gets the right-hand side vector for the equality constraints.
    /// </summary>
    private readonly Vector<double>? _b;

    // Inequality: C·x <= d
    /// <summary>
    /// Gets the inequality-constraint matrix in <c>C·x &lt;= d</c>.
    /// </summary>
    private readonly Matrix<double>? _C;

    /// <summary>
    /// Gets the right-hand side vector for the inequality constraints.
    /// </summary>
    private Vector<double>? _d;

    /// <summary>
    /// Gets the number of externally visible parameters.
    /// </summary>
    private readonly int _n;

    private readonly double _toleranceFeasibility = 1e-8;

    /// <summary>
    /// Translates an externally visible parameter index to the current constraint-matrix column index,
    /// or to -1 when the parameter is fixed and therefore eliminated from the matrices.
    /// </summary>
    private int[] _externalIndexToColumnIndex;

    /// <summary>
    /// Translates a current constraint-matrix column index to the externally visible parameter index.
    /// </summary>
    private int[] _columnIndexToExternalIndex;

    /// <summary>
    /// Gets the values to which parameters are fixed throughout the feasible set.
    /// </summary>
    private double?[] _valuesFixedByConstraints;

    /// <summary>
    /// Indicates for each externally visible parameter whether it can vary freely because it does not occur in any constraint.
    /// </summary>
    private bool[] _parametersFullyFree;

    /// <summary>
    /// Indicates whether inequality-only projections should prefer an explicitly constructed feasible start over heuristic snapping.
    /// </summary>
    private readonly bool _preferConstructedFeasibleStartForProjection;

    /// <summary>
    /// Indicates whether KKT recovery was used during the last projection.
    /// </summary>
    private bool _lastProjectionUsedKktRecovery;

    /// <summary>
    /// Gets the number of KKT recovery candidates evaluated during the last projection.
    /// </summary>
    private int _lastProjectionKktRecoveryAttemptCount;

    /// <summary>
    /// Gets the inequality index dropped for the selected KKT recovery during the last projection.
    /// </summary>
    private int? _lastProjectionRecoveredByDroppingInequalityIndex;

    /// <summary>
    /// Gets the number of active-set iterations executed during the last projection.
    /// </summary>
    private int _lastProjectionIterationCount;

    /// <summary>
    /// Indicates whether the last projection used a least-squares fallback KKT solve.
    /// </summary>
    private bool _lastProjectionUsedLeastSquaresFallback;

    /// <summary>
    /// Gets how the active-set solver terminated during the last projection.
    /// </summary>
    private ProjectionTerminationReason _lastProjectionTerminationReason;

    /// <summary>
    /// Gets the number of equality rows supplied to the constructor before preprocessing.
    /// </summary>
    private int _inputEqualityRowCount;

    /// <summary>
    /// Gets the number of inequality rows supplied to the constructor before preprocessing.
    /// </summary>
    private int _inputInequalityRowCount;

    /// <summary>
    /// Gets the number of equality rows remaining after the initial normalization pass.
    /// </summary>
    private int _initialNormalizedEqualityRowCount;

    /// <summary>
    /// Gets the number of inequality rows remaining after the initial normalization pass.
    /// </summary>
    private int _initialNormalizedInequalityRowCount;

    /// <summary>
    /// Gets the number of fully free parameters eliminated during construction.
    /// </summary>
    private int _preprocessingEliminatedFullyFreeParameterCount;

    /// <summary>
    /// Gets the number of fixed parameters eliminated during construction.
    /// </summary>
    private int _preprocessingEliminatedFixedParameterCount;

    // ---------------------------------------------------------------
    // Constructor
    // ---------------------------------------------------------------

    /// <param name="A">Equality matrix (nEq × n). The columns of the matrix correspond to the parameters to be projected. The rows correspond to the different equality constraints.</param>
    /// <param name="b">Equality RHS (nEq). The elements of the vector correspond to the right-hand side values of the equality constraints.</param>
    /// <param name="C">Inequality matrix for C·x &lt;= d (nLeq × n). The columns of the matrix correspond to the parameters to be projected. The rows correspond to the different inequality constraints.</param>
    /// <param name="d">Inequality RHS (nLeq). The elements of the vector correspond to the right-hand side values of the inequality constraints.</param>
    public LinearConstraintsProjector(
        Matrix<double>? A = null, Vector<double>? b = null,
        Matrix<double>? C = null, Vector<double>? d = null)
      : this(A, b, C, d, validateCompatibility: true, preferConstructedFeasibleStartForProjection: true)
    {
    }

    /// <summary>
    /// Creates a new projector using the nonpublic construction options.
    /// This member is intended only for internal diagnostics and tests.
    /// </summary>
    /// <param name="A">Equality matrix (nEq × n).</param>
    /// <param name="b">Equality RHS (nEq).</param>
    /// <param name="C">Inequality matrix for C·x &lt;= d (nLeq × n).</param>
    /// <param name="d">Inequality RHS (nLeq).</param>
    /// <param name="validateCompatibility">If <see langword="true"/>, checks whether the constraint set is feasible.</param>
    /// <param name="preferConstructedFeasibleStartForProjection">If <see langword="true"/>, inequality-only projections prefer an explicitly constructed feasible start over heuristic snapping.</param>
    /// <returns>A projector created with the supplied internal construction options.</returns>
    internal static LinearConstraintsProjector CreateForDiagnostics(
      Matrix<double>? A,
      Vector<double>? b,
      Matrix<double>? C,
      Vector<double>? d,
      bool validateCompatibility = true,
      bool preferConstructedFeasibleStartForProjection = false)
    {
      return new LinearConstraintsProjector(A, b, C, d, validateCompatibility, preferConstructedFeasibleStartForProjection);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinearConstraintsProjector"/> class.
    /// </summary>
    /// <param name="A">Equality matrix (nEq × n). The columns of the matrix correspond to the parameters to be projected. The rows correspond to the different equality constraints.</param>
    /// <param name="b">Equality RHS (nEq). The elements of the vector correspond to the right-hand side values of the equality constraints.</param>
    /// <param name="C">Inequality matrix for C·x &lt;= d (nLeq × n). The columns of the matrix correspond to the parameters to be projected. The rows correspond to the different inequality constraints.</param>
    /// <param name="d">Inequality RHS (nLeq). The elements of the vector correspond to the right-hand side values of the inequality constraints.</param>
    /// <param name="validateCompatibility">If <see langword="true"/>, checks whether the constraint set is feasible.</param>
    /// <param name="preferConstructedFeasibleStartForProjection">If <see langword="true"/>, inequality-only projections prefer an explicitly constructed feasible start over heuristic snapping.</param>
    private LinearConstraintsProjector(
        Matrix<double>? A, Vector<double>? b,
        Matrix<double>? C, Vector<double>? d,
        bool validateCompatibility,
        bool preferConstructedFeasibleStartForProjection)
    {

      if (A is null && C is null)
        throw new ArgumentNullException(nameof(A), "At least one of the matrices A or C must be provided.");
      if (A is not null && b is null)
        throw new ArgumentNullException(nameof(b), "Equality constraints provided without right-hand side vector.");
      if (A is null && b is not null)
        throw new ArgumentNullException(nameof(A), "Equality constraints right-hand side vector provided without constraint matrix.");
      if (C is not null && d is null)
        throw new ArgumentNullException(nameof(d), "Inequality constraints provided without right-hand side vector.");
      if (C is null && d is not null)
        throw new ArgumentNullException(nameof(C), "Inequality constraints right-hand side vector provided without constraint matrix.");
      if (A is not null && C is not null && A.ColumnCount != C.ColumnCount)
        throw new ArgumentException("Equality and inequality constraint matrices must have the same number of columns.");
      if (A is not null && A.RowCount != b.Count)
        throw new ArgumentException("Equality constraint matrix row count must match the size of the equality right-hand side vector.");
      if (C is not null && C.RowCount != d.Count)
        throw new ArgumentException("Inequality constraint matrix row count must match the size of the inequality right-hand side vector.");

      _n = A?.ColumnCount ?? C!.ColumnCount;
      _inputEqualityRowCount = A?.RowCount ?? 0;
      _inputInequalityRowCount = C?.RowCount ?? 0;

      // First, eliminate empty rows
      // in the equality constraints,
      if (A is not null && b is not null)
        (A, b) = EliminateEmptyRows(A, b, isEqualitySystem: true);

      // and in the inequality constraints.
      if (C is not null && d is not null)
        (C, d) = EliminateEmptyRows(C, d, isEqualitySystem: false);

      // Second, eliminate trivial fixed constraints (equality constraints where only one column of the matrix has a nonzero entry)
      _valuesFixedByConstraints = new double?[_n];
      _columnIndexToExternalIndex = Enumerable.Range(0, _n).ToArray();
      (A, b, C, d, _valuesFixedByConstraints, _columnIndexToExternalIndex) = EliminateTrivialFixedParameters(A, b, C, d, _valuesFixedByConstraints, _columnIndexToExternalIndex);

      // Third, eliminate fully free parameters (columns that contain only zeros and therefore do not affect feasibility and can vary freely). This is not only a simplification but also improves numerical stability of the projection.
      _parametersFullyFree = new bool[_n];
      (A, b, C, d, _columnIndexToExternalIndex, _parametersFullyFree) = EliminateFullyFreeParameters(A, b, C, d, _columnIndexToExternalIndex, _parametersFullyFree);

      _preferConstructedFeasibleStartForProjection = preferConstructedFeasibleStartForProjection;

      // Fourth, normalize the constraints
      (A, b, C, d) = NormalizeConstraints(A, b, C, d);

      // After normalization, eliminate fixed parameters again
      (A, b, C, d, _valuesFixedByConstraints, _columnIndexToExternalIndex) = EliminateTrivialFixedParameters(A, b, C, d, _valuesFixedByConstraints, _columnIndexToExternalIndex);

      // Sixth, detect fixed parameters maybe fixed by straight inequalities etc.
      (A, b, C, d, _valuesFixedByConstraints, _columnIndexToExternalIndex) = EliminateParametersFixedByEqualities(A, b, C, d, _valuesFixedByConstraints, _columnIndexToExternalIndex);

      int internalParameterCount = GetConstraintColumnCount(A, C);


      _initialNormalizedEqualityRowCount = A?.RowCount ?? 0;
      _initialNormalizedInequalityRowCount = C?.RowCount ?? 0;


      _externalIndexToColumnIndex = Enumerable.Range(0, _n).Select(i => _columnIndexToExternalIndex.IndexOf(i)).ToArray();

      if (validateCompatibility && HasIncompatibleConstraints(_columnIndexToExternalIndex.Length, A, b, C, d))
        throw new InvalidDataException("The specified constraints are incompatible.");

      (_A, _b, _C, _d) = (A, b, C, d);
    }



    /// <summary>
    /// Creates a new linear constraint projector with all fixed parameters eliminated from the constraints.
    /// </summary>
    /// <remarks>Use this method to obtain a projector that operates only on free parameters, with all
    /// parameters fixed by constraints removed. The returned array provides the values of the parameters that were
    /// fixed.</remarks>
    /// <returns>A tuple containing the new linear constraint projector with fixed parameters removed, and an array of nullable
    /// doubles representing the values of the fixed parameters.</returns>
    /// <exception cref="InvalidOperationException">Thrown if any fixed parameters remain after elimination, indicating an unexpected state.</exception>
    public (LinearConstraintsProjector Projector, IReadOnlyList<double?> FixedParameters) ToProjectorWithoutFixedParameters()
    {
      var result = new LinearConstraintsProjector(A: _A?.Clone(), b: _b?.Clone(), C: _C?.Clone(), d: _d?.Clone(), true, _preferConstructedFeasibleStartForProjection);

      if (result._valuesFixedByConstraints.Any(v => v.HasValue))
        throw new InvalidProgramException("Unexpected fixed parameters remain after elimination. That should not happen. Try to add a test for it!");
      if (result._parametersFullyFree.Any(v => v))
        throw new InvalidProgramException("Unexpected fully free parameters remain after elimination. That should not happen. Try to add a test for it!");

      int newNumberOfParameters = _valuesFixedByConstraints.Count(v => !v.HasValue);
      int[] oldExternalIndexToNewExternalIndex = new int[_n];
      int[] newExternalIndexToOldExternalIndex = new int[newNumberOfParameters];
      Array.Fill(oldExternalIndexToNewExternalIndex, -1);
      for (int iOldIndex = 0, iNewIndex = 0; iOldIndex < _n; iOldIndex++)
      {
        if (!_valuesFixedByConstraints[iOldIndex].HasValue)
        {
          oldExternalIndexToNewExternalIndex[iOldIndex] = iNewIndex;
          newExternalIndexToOldExternalIndex[iNewIndex] = iOldIndex;
          ++iNewIndex;
        }
      }

      bool[] newParametersFullyFree = new bool[newNumberOfParameters];
      for (int i = 0; i < newParametersFullyFree.Length; i++)
      {
        newParametersFullyFree[i] = _parametersFullyFree[newExternalIndexToOldExternalIndex[i]];
      }

      int[] newColumnIndexToExternalIndex = new int[_columnIndexToExternalIndex.Length];
      int[] newExternalIndexToColumnIndex = new int[newNumberOfParameters];
      Array.Fill(newExternalIndexToColumnIndex, -1);
      for (int i = 0; i < newColumnIndexToExternalIndex.Length; ++i)
      {
        var newExternalIndex = oldExternalIndexToNewExternalIndex[_columnIndexToExternalIndex[i]];
        newColumnIndexToExternalIndex[i] = newExternalIndex;
        newExternalIndexToColumnIndex[newExternalIndex] = i;
      }

      result._parametersFullyFree = newParametersFullyFree;
      result._externalIndexToColumnIndex = newExternalIndexToColumnIndex;
      result._columnIndexToExternalIndex = newColumnIndexToExternalIndex;

      return (result, this.ValuesFixedByConstraints);
    }


    /// <summary>
    /// Attempts to create a new instance of the LinearConstraintsProjector using the specified equality and inequality
    /// constraints.  
    /// </summary>
    /// <remarks>This method does not validate the compatibility of the provided matrices and vectors beyond
    /// checking for incompatible constraints. If the constraints are incompatible, the returned projector will be null
    /// and the error message will describe the issue.</remarks>
    /// <param name="A">The matrix representing the coefficients of the equality constraints. May be null if no equality constraints are
    /// specified.</param>
    /// <param name="b">The vector representing the right-hand side of the equality constraints. Must be compatible with <paramref
    /// name="A"/> if provided. May be null if no equality constraints are specified.</param>
    /// <param name="C">The matrix representing the coefficients of the inequality constraints. May be null if no inequality constraints
    /// are specified.</param>
    /// <param name="d">The vector representing the right-hand side of the inequality constraints. Must be compatible with <paramref
    /// name="C"/> if provided. May be null if no inequality constraints are specified.</param>
    /// <returns>A tuple containing the created LinearConstraintsProjector if the constraints are compatible, or null and an error
    /// message if the constraints are incompatible.</returns>
    public static (LinearConstraintsProjector? Projector, string? ErrorMessage) TryCreate(Matrix<double>? A, Vector<double>? b, Matrix<double>? C, Vector<double>? d)
    {

      var projector = new LinearConstraintsProjector(A, b, C, d, validateCompatibility: false, preferConstructedFeasibleStartForProjection: true);
      if (HasIncompatibleConstraints(projector._columnIndexToExternalIndex.Length, projector._A, projector._b, projector._C, projector._d))
        return (null, "The specified constraints are incompatible.");
      else
        return (projector, null);
    }


    // ---------------------------------------------------------------
    // Public API
    // ---------------------------------------------------------------

    /// <summary>
    /// Gets the values to which parameters are fixed throughout the feasible set.
    /// </summary>
    public IReadOnlyList<double?> ValuesFixedByConstraints => _valuesFixedByConstraints;

    /// <summary>
    /// Gets the number of parameters required by the operation or function.
    /// </summary>
    public int NumberOfParameters => _n;

    /// <summary>
    /// Gets the number of parameters that are fixed to a single value throughout the feasible set by the constraints.
    /// </summary>
    public int NumberOfParametersFixed => _valuesFixedByConstraints.Count(v => v.HasValue);

    /// <summary>
    /// Gets the number of parameters whose values are not fixed by constraints and can vary. 
    /// </summary>
    public int NumberOfParametersThatCanVary => _valuesFixedByConstraints.Count(v => !v.HasValue);

    /// <summary>
    /// Gets the number of parameters that are fully free, meaning they do not occur in any constraint and can vary without affecting feasibility.
    /// </summary>
    public int NumberOfParametersFullyFree => _parametersFullyFree.Count(v => v);

    /// <summary>
    /// Gets the number of parameters that are neither fixed by constraints nor fully free, meaning they occur in at least one constraint but are not fixed to a single value throughout the feasible set.
    /// </summary>
    public int NumberOfParametersNeitherFixedNorFullyFree => _valuesFixedByConstraints.Count(v => !v.HasValue) - _parametersFullyFree.Count(v => v);

    /// <summary>
    /// Gets a value indicating whether all parameters have been assigned fixed values by constraints.
    /// </summary>
    public bool AreAllParametersFixed => _valuesFixedByConstraints.All(v => v.HasValue);

    /// <summary>
    /// Gets a value indicating whether all parameters are fully free (thus this <see cref="LinearConstraintsProjector"/> has no constraints at all and therefore is not needed).
    /// </summary>
    public bool AreAllParametersFullyFree => _parametersFullyFree.All(v => v);

    // Diagnostic properties (internal only)

    /// <summary>
    /// Gets the number of equality constraints, i.e. the row count of the equality-constraint matrix <c>A</c>. Returns 0 if there are no equality constraints.
    /// </summary>
    internal int A_RowCount => _A?.RowCount ?? 0;

    /// <summary>
    /// Gets the number of parameters that occur in at least one equality constraint, i.e. the column count of the equality-constraint matrix <c>A</c>. Returns 0 if there are no equality constraints.
    /// </summary>
    internal int A_ColumnCount => _A?.ColumnCount ?? 0;

    /// <summary>
    /// Gets the number of inequality constraints, i.e. the row count of the inequality-constraint matrix <c>C</c>. Returns 0 if there are no inequality constraints.
    /// </summary>
    internal int C_RowCount => _C?.RowCount ?? 0;

    /// <summary>
    /// Gets the number of parameters that occur in at least one inequality constraint, i.e. the column count of the inequality-constraint matrix <c>C</c>. Returns 0 if there are no inequality constraints.
    /// </summary>
    internal int C_ColumnCount => _C?.ColumnCount ?? 0;

    /// <summary>
    /// Gets a value indicating whether the last projection used KKT recovery.
    /// </summary>
    internal bool LastProjectionUsedKktRecovery => _lastProjectionUsedKktRecovery;

    /// <summary>
    /// Gets the number of recovery candidates evaluated during the last projection.
    /// </summary>
    internal int LastProjectionKktRecoveryAttemptCount => _lastProjectionKktRecoveryAttemptCount;

    /// <summary>
    /// Gets the inequality index dropped for the selected recovery during the last projection.
    /// </summary>
    internal int? LastProjectionRecoveredByDroppingInequalityIndex => _lastProjectionRecoveredByDroppingInequalityIndex;

    /// <summary>
    /// Gets the number of active-set iterations executed during the last projection.
    /// </summary>
    internal int LastProjectionIterationCount => _lastProjectionIterationCount;

    /// <summary>
    /// Gets a value indicating whether the last projection used a least-squares fallback KKT solve.
    /// </summary>
    internal bool LastProjectionUsedLeastSquaresFallback => _lastProjectionUsedLeastSquaresFallback;

    /// <summary>
    /// Gets how the active-set solver terminated during the last projection.
    /// </summary>
    internal ProjectionTerminationReason LastProjectionTerminationReason => _lastProjectionTerminationReason;

    /// <summary>
    /// Gets the number of equality rows supplied to the constructor before preprocessing.
    /// </summary>
    internal int InputEqualityRowCount => _inputEqualityRowCount;

    /// <summary>
    /// Gets the number of inequality rows supplied to the constructor before preprocessing.
    /// </summary>
    internal int InputInequalityRowCount => _inputInequalityRowCount;

    /// <summary>
    /// Gets the number of equality rows remaining after the initial normalization pass.
    /// </summary>
    internal int InitialNormalizedEqualityRowCount => _initialNormalizedEqualityRowCount;

    /// <summary>
    /// Gets the number of inequality rows remaining after the initial normalization pass.
    /// </summary>
    internal int InitialNormalizedInequalityRowCount => _initialNormalizedInequalityRowCount;

    /// <summary>
    /// Gets the number of fully free parameters eliminated during construction.
    /// </summary>
    internal int PreprocessingEliminatedFullyFreeParameterCount => _preprocessingEliminatedFullyFreeParameterCount;

    /// <summary>
    /// Gets the number of fixed parameters eliminated during construction.
    /// </summary>
    internal int PreprocessingEliminatedFixedParameterCount => _preprocessingEliminatedFixedParameterCount;

    /// <summary>
    /// Executes the KKT recovery path for supplied active inequalities and returns the recovered subproblem solution.
    /// This member is intended only for internal diagnostics and tests.
    /// </summary>
    /// <param name="x">The current iterate.</param>
    /// <param name="p">The original point to be projected.</param>
    /// <param name="activeRows">The active inequality rows to use for the recovery subproblem.</param>
    /// <param name="activeRhs">The right-hand side values corresponding to <paramref name="activeRows"/>.</param>
    /// <param name="activeSet">The active inequality indices represented by <paramref name="activeRows"/>.</param>
    /// <param name="tol">The numerical tolerance.</param>
    /// <returns>The recovered solution, or <see langword="null"/> if none is found.</returns>
    internal Vector<double>? ExecuteKktRecoveryForDiagnostics(
      Vector<double> x,
      Vector<double> p,
      Matrix<double> activeRows,
      Vector<double> activeRhs,
      IReadOnlyCollection<int> activeSet,
      double tol = 1e-10)
    {
      if (x is null)
        throw new ArgumentNullException(nameof(x));
      if (p is null)
        throw new ArgumentNullException(nameof(p));
      if (activeRows is null)
        throw new ArgumentNullException(nameof(activeRows));
      if (activeRhs is null)
        throw new ArgumentNullException(nameof(activeRhs));
      if (activeSet is null)
        throw new ArgumentNullException(nameof(activeSet));
      if (activeRows.RowCount != activeRhs.Count)
        throw new ArgumentException("The number of active rows must match the size of the active right-hand side vector.");

      ResetLastProjectionDiagnostics();
      var grad = x - p;
      var mutableActiveSet = new HashSet<int>(activeSet);
      return SolveProjectionSubproblemWithRecovery(x, p, grad, ref mutableActiveSet, activeRows, activeRhs, tol, activeRowsSource: activeRows, activeRhsSource: activeRhs);
    }

    /// <summary>
    /// Attempts to convert the current set of linear constraints to simple lower and upper boundary constraints for
    /// each parameter.
    /// </summary>
    /// <param name="tolerance">When determining whether a matrix entry is considered non-zero, this tolerance is used relative to the row norm. This allows for some numerical imprecision in the constraint definitions while still enabling conversion to boundary constraints when appropriate.</param>
    /// <returns>A tuple containing arrays of nullable doubles representing the lower and upper bounds for each parameter if the
    /// constraints can be represented as simple boundaries; otherwise, null if the conversion is not possible.</returns>
    /// <remarks>The conversion succeeds only if there are no equality constraints and each inequality
    /// constraint involves exactly one parameter. If there are no inequality constraints, all parameters are considered
    /// unconstrained.</remarks>
    public (double?[]? LowerBounds, double?[]? UpperBounds)? TryConvertToBoundaryConstraints(double tolerance = 1e-12)
    {
      if (_A is not null && _A.RowCount > 0) // there should be no equality constraints, because the fixed constraints should have been eliminated in the constructor
      {
        return null;
      }

      if (_C is null || _C.RowCount == 0) // if there are no inequality constraints, we can treat all parameters as fully free (unconstrained)
      {
        return (null, null); // no constraints at all
      }


      // now, in order to be able to convert the constraints to simple boundary constraints, each row of _C must have exactly one non-zero entry,

      var lb = new double?[_n];
      var ub = new double?[_n];
      var rowNorms = _C.RowNorms(2);
      for (int iR = 0; iR < _C.RowCount; ++iR)
      {
        int nonZeroCount = 0;
        int iCNonZeroElement = -1;


        for (int iC = 0; iC < _C.ColumnCount; ++iC)
        {
          if (Math.Abs(_C[iR, iC]) > tolerance * rowNorms[iR])
          {
            nonZeroCount++;
            iCNonZeroElement = iC;
            if (nonZeroCount > 1)
            {
              return null; // more than one non-zero entry in this row, cannot convert to boundary constraints
            }
          }

          if (nonZeroCount == 1)
          {
            double bound = _d[iR] / _C[iR, iCNonZeroElement];
            int externalIndex = _columnIndexToExternalIndex[iCNonZeroElement];

            if (_C[iR, iCNonZeroElement] > 0)
            {
              if (!ub[externalIndex].HasValue || bound < ub[externalIndex]!.Value)
                ub[externalIndex] = bound;
            }
            else
            {
              if (!lb[externalIndex].HasValue || bound > lb[externalIndex]!.Value)
                lb[externalIndex] = bound;
            }
          }
        }
      }

      if (!lb.Any(x => x.HasValue))
        lb = null;
      if (!ub.Any(x => x.HasValue))
        ub = null;

      return (lb, ub);
    }



    /// <summary>
    /// Projects the specified point onto the feasible set and returns only the projected point.
    /// </summary>
    /// <param name="p">The point to project.</param>
    /// <returns>The projected feasible point.</returns>
    public Vector<double> Project(Vector<double> p)
        => ProjectWithInfo(p).Parameters;

    /// <inheritdoc/>
    public void Project(Vector<double> inputValues, Vector<double> projectedValues, Span<bool> valuesConstrained)
    {
      var result = ProjectWithInfo(inputValues);
      result.Parameters.CopyTo(projectedValues);
      result.IsConstrained.CopyTo(valuesConstrained);
    }


    /// <summary>
    /// Projects the specified point onto the feasible set and returns full constraint-activity information.
    /// </summary>
    /// <param name="p">The point to project.</param>
    /// <returns>A <see cref="ProjectionResult"/> containing the projected point and activity information.</returns>
    public ProjectionResult ProjectWithInfo(Vector<double> p)
    {
      ValidateExternalParameterCount(p, nameof(p));

      ResetLastProjectionDiagnostics();

      var reducedPoint = ReduceToInternalParameters(p);

      if (_A == null && _C == null)
      {
        _lastProjectionTerminationReason = ProjectionTerminationReason.NoConstraints;
        return BuildResult(reducedPoint.Clone(), new HashSet<int>(), p);
      }

      var (x, activeSet) = SolveProjectionQP(reducedPoint, reducedPoint.Clone());
      return BuildResult(x, activeSet, p);
    }

    /// <summary>
    /// Resets the diagnostics captured for the most recent projection run.
    /// </summary>
    private void ResetLastProjectionDiagnostics()
    {
      _lastProjectionUsedKktRecovery = false;
      _lastProjectionKktRecoveryAttemptCount = 0;
      _lastProjectionRecoveredByDroppingInequalityIndex = null;
      _lastProjectionIterationCount = 0;
      _lastProjectionUsedLeastSquaresFallback = false;
      _lastProjectionTerminationReason = ProjectionTerminationReason.None;
    }

    /// <summary>
    /// Determines whether the specified point satisfies all constraints within the given tolerance.
    /// </summary>
    /// <param name="inputValues">The point to test.</param>
    /// <returns><see langword="true"/> if the point is feasible; otherwise, <see langword="false"/>.</returns>
    public bool IsFeasible(Vector<double> inputValues)
    {
      ValidateExternalParameterCount(inputValues, nameof(inputValues));

      if (inputValues.Any(v => double.IsNaN(v) || double.IsInfinity(v)))
        return false;

      for (int externalIndex = 0; externalIndex < _n; externalIndex++)
      {
        if (_valuesFixedByConstraints[externalIndex].HasValue && Math.Abs(inputValues[externalIndex] - _valuesFixedByConstraints[externalIndex]!.Value) > _toleranceFeasibility)
          return false;
      }

      var reducedPoint = ReduceToInternalParameters(inputValues);

      if (_A != null)
      {
        var equalityResidual = _A * reducedPoint - _b!;
        if (equalityResidual.Any(v => double.IsNaN(v) || double.IsInfinity(v)) || equalityResidual.L2Norm() > _toleranceFeasibility)
          return false;
      }

      if (_C != null)
      {
        var inequalityResidual = _C * reducedPoint - _d!;
        if (inequalityResidual.Any(v => double.IsNaN(v) || double.IsInfinity(v) || v > _toleranceFeasibility))
          return false;
      }

      return true;
    }

    /// <summary>
    /// Validates that an externally supplied parameter vector has the expected dimension.
    /// </summary>
    /// <param name="externalParameters">The externally supplied parameter vector.</param>
    /// <param name="paramName">The parameter name used for error reporting.</param>
    private void ValidateExternalParameterCount(Vector<double> externalParameters, string paramName)
    {
      if (externalParameters is null)
        throw new ArgumentNullException(paramName);

      if (externalParameters.Count != _n)
        throw new ArgumentException($"The parameter vector must contain exactly {_n} elements, but contains {externalParameters.Count}.", paramName);
    }

    // ---------------------------------------------------------------
    // Active Set QP solver
    // min ½||x - p||²  s.t.  A·x = b,  C·x <= d
    // ---------------------------------------------------------------

    /// <summary>
    /// Solves the quadratic projection problem with an active-set method.
    /// </summary>
    /// <param name="p">The original point to be projected.</param>
    /// <param name="x0">The initial iterate for the active-set solver.</param>
    /// <returns>
    /// A tuple containing the projected point and the set of active inequality-constraint indices.
    /// </returns>
    private (Vector<double> x, HashSet<int> activeSet) SolveProjectionQP(
        Vector<double> p, Vector<double> x0)
    {
      const int maxIter = 200;
      const double tol = 1e-10;
      int internalParameterCount = _columnIndexToExternalIndex.Length;

      var x = x0.Clone();
      var activeSet = GetFeasibleStartForProjection(ref x, tol, preferConstructedFeasibleStart: _preferConstructedFeasibleStartForProjection && _A is null);
      int nIneq = _C?.RowCount ?? 0;

      for (int iter = 0; iter < maxIter; iter++)
      {
        _lastProjectionIterationCount = iter + 1;
        var (Cact, dAct) = ExtractActiveRows(activeSet);

        var grad = x - p;
        var sol = SolveProjectionSubproblemWithRecovery(x, p, grad, ref activeSet, Cact, dAct, tol);
        if (sol == null)
        {
          _lastProjectionTerminationReason = ProjectionTerminationReason.KktSolveFailed;
          break;
        }

        if ((Cact?.RowCount ?? 0) != activeSet.Count)
        {
          (Cact, dAct) = ExtractActiveRows(activeSet);
        }

        int nEq = _A?.RowCount ?? 0;
        int nAct = activeSet.Count;
        int inequalityMultiplierCount = Math.Max(0, sol.Count - internalParameterCount - nEq);

        var step = sol.SubVector(0, internalParameterCount);
        var lambdaIneq = inequalityMultiplierCount > 0
            ? sol.SubVector(internalParameterCount + nEq, inequalityMultiplierCount)
            : null;

        if (step.L2Norm() < tol)
        {
          // Stationary point — check multipliers for optimality.
          // For C·x <= d the KKT multipliers must be >= 0 at the optimum.
          if (lambdaIneq == null || lambdaIneq.All(l => l >= -tol))
          {
            _lastProjectionTerminationReason = ProjectionTerminationReason.OptimalStationaryPoint;
            break; // Optimal
          }

          // Drop the active constraint with the most negative multiplier.
          int worstLocal = 0;
          double worstMultiplier = lambdaIneq[0];
          for (int i = 1; i < lambdaIneq.Count; i++)
          {
            if (lambdaIneq[i] < worstMultiplier)
            {
              worstMultiplier = lambdaIneq[i];
              worstLocal = i;
            }
          }

          int worstGlobal = activeSet.OrderBy(i => i).ElementAt(worstLocal);
          activeSet.Remove(worstGlobal);
          _lastProjectionTerminationReason = ProjectionTerminationReason.DroppedConstraintWithNegativeMultiplier;
        }
        else
        {
          // Line search: largest alpha in [0,1] keeping C·x <= d
          double alpha = 1.0;
          int blockingIdx = -1;

          if (_C != null)
          {
            for (int i = 0; i < nIneq; i++)
            {
              if (activeSet.Contains(i)) continue;

              double num = _d![i] - _C.Row(i) * x;
              double den = _C.Row(i) * step;

              // Constraint becomes tighter as we move in direction step
              // when den > 0; check if we hit it before alpha = 1
              if (den > tol)
              {
                double alphaI = num / den;
                if (alphaI >= 0 && alphaI < alpha)
                {
                  alpha = alphaI;
                  blockingIdx = i;
                }
              }
            }
          }

          x += alpha * step;

          if (blockingIdx >= 0)
            activeSet.Add(blockingIdx);
        }
      }

      if (_lastProjectionTerminationReason == ProjectionTerminationReason.None)
        _lastProjectionTerminationReason = ProjectionTerminationReason.MaxIterationsReached;

      return (x, activeSet);
    }

    /// <summary>
    /// Solves the current active-set subproblem and tries to recover from dependent active inequalities.
    /// </summary>
    /// <param name="x">The current iterate.</param>
    /// <param name="p">The original point to be projected.</param>
    /// <param name="grad">The objective gradient at <paramref name="x"/>.</param>
    /// <param name="activeSet">The currently active inequality set, which may be reduced if recovery is needed.</param>
    /// <param name="Cact">The active inequality matrix.</param>
    /// <param name="dAct">The active inequality right-hand side vector.</param>
    /// <param name="tol">The numerical tolerance.</param>
    /// <param name="activeRowsSource">The source inequality matrix used when extracting reduced active rows during recovery. If <see langword="null"/>, the projector's stored inequality matrix is used.</param>
    /// <param name="activeRhsSource">The source inequality right-hand side vector used when extracting reduced active rows during recovery. If <see langword="null"/>, the projector's stored inequality right-hand side is used.</param>
    /// <returns>The KKT solution, or <see langword="null"/> if no recoverable solve is found.</returns>
    private Vector<double>? SolveProjectionSubproblemWithRecovery(
        Vector<double> x,
        Vector<double> p,
        Vector<double> grad,
        ref HashSet<int> activeSet,
        Matrix<double>? Cact,
        Vector<double>? dAct,
        double tol,
        Matrix<double>? activeRowsSource = null,
        Vector<double>? activeRhsSource = null)
    {
      var KKT = BuildKKT(Cact);
      var rhs = BuildKKTRhs(x, grad, Cact, dAct);
      var sol = SolveKKTDirect(KKT, rhs);
      if (IsValidKktSolution(sol))
        return sol;

      if (activeSet.Count == 0)
        return SolveKKTLeastSquaresWithDiagnostics(KKT, rhs);

      Vector<double>? bestSolution = null;
      HashSet<int>? bestReducedActiveSet = null;
      double bestStepNorm = double.PositiveInfinity;
      int bestDroppedIndex = int.MaxValue;

      foreach (int activeIndexToDrop in activeSet.OrderByDescending(index => index).ToArray())
      {
        _lastProjectionKktRecoveryAttemptCount++;

        var reducedActiveSet = new HashSet<int>(activeSet);
        reducedActiveSet.Remove(activeIndexToDrop);

        var (reducedCact, reducedDact) = ExtractActiveRows(reducedActiveSet, activeRowsSource, activeRhsSource);
        var recoveredKkt = BuildKKT(reducedCact);
        var recoveredRhs = BuildKKTRhs(x, grad, reducedCact, reducedDact);
        var recoveredSolution = SolveKKTDirect(recoveredKkt, recoveredRhs);

        if (!IsValidKktSolution(recoveredSolution))
          continue;

        double recoveredStepNorm = recoveredSolution!.SubVector(0, _columnIndexToExternalIndex.Length).L2Norm();
        if (recoveredStepNorm < bestStepNorm - tol ||
          (Math.Abs(recoveredStepNorm - bestStepNorm) <= tol && activeIndexToDrop < bestDroppedIndex))
        {
          bestSolution = recoveredSolution;
          bestReducedActiveSet = reducedActiveSet;
          bestStepNorm = recoveredStepNorm;
          bestDroppedIndex = activeIndexToDrop;
        }
      }

      if (bestSolution is null || bestReducedActiveSet is null)
        return SolveKKTLeastSquaresWithDiagnostics(KKT, rhs);

      _lastProjectionUsedKktRecovery = true;
      _lastProjectionRecoveredByDroppingInequalityIndex = bestDroppedIndex;
      activeSet = bestReducedActiveSet;
      return bestSolution;
    }

    /// <summary>
    /// Solves the specified KKT system with a least-squares fallback and records diagnostic usage.
    /// </summary>
    /// <param name="KKT">The KKT matrix.</param>
    /// <param name="rhs">The right-hand side vector.</param>
    /// <returns>The solution vector, or <see langword="null"/> if the system cannot be solved.</returns>
    private Vector<double>? SolveKKTLeastSquaresWithDiagnostics(Matrix<double> KKT, Vector<double> rhs)
    {
      var solution = SolveKKTLeastSquares(KKT, rhs);
      if (IsValidKktSolution(solution))
      {
        _lastProjectionUsedLeastSquaresFallback = true;
        _lastProjectionTerminationReason = ProjectionTerminationReason.LeastSquaresFallback;
      }

      return solution;
    }

    /// <summary>
    /// Determines whether the specified KKT solution is usable for further iterations.
    /// </summary>
    /// <param name="solution">The candidate KKT solution.</param>
    /// <returns><see langword="true"/> if the solution is non-null and finite; otherwise, <see langword="false"/>.</returns>
    private static bool IsValidKktSolution(Vector<double>? solution)
    {
      return solution is not null && solution.All(value => !double.IsNaN(value) && !double.IsInfinity(value));
    }

    /// <summary>
    /// Gets a feasible starting point and initial active inequality set for the projection solver.
    /// </summary>
    /// <param name="x">The initial point on entry and the feasible starting point on exit.</param>
    /// <param name="tol">The feasibility tolerance.</param>
    /// <param name="preferConstructedFeasibleStart">If <see langword="true"/>, tries to construct a feasible point before falling back to heuristic snapping.</param>
    /// <returns>The indices of the inequalities that are active at the feasible starting point.</returns>
    private HashSet<int> GetFeasibleStartForProjection(ref Vector<double> x, double tol, bool preferConstructedFeasibleStart)
    {
      var activeSet = new HashSet<int>();
      if (_C is null || _d is null)
        return activeSet;

      if (IsReducedPointFeasible(x, tol))
        return GetActiveInequalityIndices(x, tol);

      if (_A is not null && _b is not null)
      {
        var feasibleStartWithEqualities = FindFeasiblePointForEqualitiesAndInequalities(_A, _b, _C, _d, tol);
        if (feasibleStartWithEqualities is not null)
        {
          x = feasibleStartWithEqualities;
          return GetActiveInequalityIndices(x, tol);
        }
      }

      if (preferConstructedFeasibleStart && _A is null)
      {
        var feasibleStart = FindFeasiblePointForInequalities(_columnIndexToExternalIndex.Length, _C, _d, tol);
        if (feasibleStart is not null)
        {
          x = feasibleStart;
          return GetActiveInequalityIndices(x, tol);
        }
      }

      for (int i = 0; i < _C.RowCount; i++)
      {
        var row = _C.Row(i);
        double slack = _d[i] - row * x;

        if (slack < -tol)
        {
          double rowNormSq = row * row;
          if (rowNormSq > 0)
            x += (slack / rowNormSq) * row;
          activeSet.Add(i);
        }
        else if (slack <= tol)
        {
          activeSet.Add(i);
        }
      }

      if (IsReducedPointFeasible(x, tol))
        return GetActiveInequalityIndices(x, tol);

      return activeSet;
    }

    // ---------------------------------------------------------------
    // Result construction
    // ---------------------------------------------------------------

    /// <summary>
    /// Builds the projection result object from the projected point and active inequalities.
    /// </summary>
    /// <param name="x">The projected point.</param>
    /// <param name="activeIneqSet">The indices of the active inequality constraints.</param>
    /// <param name="originalExternalPoint">The original point in external coordinates, used to preserve fully free parameters.</param>
    /// <returns>A populated <see cref="ProjectionResult"/> instance.</returns>
    private ProjectionResult BuildResult(Vector<double> x, HashSet<int> activeIneqSet, Vector<double> originalExternalPoint)
    {
      var constrained = ComputeConstrainedMask(activeIneqSet);

      int nEq = _A?.RowCount ?? 0;
      var activeAll = Enumerable.Range(0, nEq)
          .Concat(activeIneqSet.OrderBy(i => i).Select(i => nEq + i))
          .ToList();

      return new ProjectionResult
      {
        Parameters = ExpandToExternalParameters(x, originalExternalPoint),
        FixedValues = (double?[])_valuesFixedByConstraints.Clone(),
        IsConstrained = constrained,
        ActiveConstraintIndices = activeAll.AsReadOnly(),
        FreeParameterIndices = Enumerable.Range(0, _n)
                                          .Where(i => !constrained[i]).ToList(),
        ConstrainedParameterIndices = Enumerable.Range(0, _n)
                                          .Where(i => constrained[i]).ToList(),
        FixedParameterIndices = Enumerable.Range(0, _n)
                                          .Where(i => _valuesFixedByConstraints[i].HasValue).ToList(),
      };
    }

    /// <summary>
    /// Computes a mask that indicates which parameters are constrained by active constraints.
    /// </summary>
    /// <param name="activeIneqSet">The indices of the active inequality constraints.</param>
    /// <param name="tol">The tolerance used to decide whether a matrix entry is treated as non-zero.</param>
    /// <returns>An array whose entries are <see langword="true"/> for constrained parameters.</returns>
    private bool[] ComputeConstrainedMask(HashSet<int> activeIneqSet, double tol = 1e-10)
    {
      var mask = new bool[_n];

      for (int externalIndex = 0; externalIndex < _n; externalIndex++)
      {
        if (_valuesFixedByConstraints[externalIndex].HasValue)
          mask[externalIndex] = true;
      }

      if (_A != null)
        MarkColumns(mask, _A, _columnIndexToExternalIndex, Enumerable.Range(0, _A.RowCount), tol);

      if (_C != null && activeIneqSet.Count > 0)
        MarkColumns(mask, _C, _columnIndexToExternalIndex, activeIneqSet, tol);

      return mask;
    }

    /// <summary>
    /// Marks all columns participating in the specified rows as constrained.
    /// </summary>
    /// <param name="mask">The mask to update.</param>
    /// <param name="M">The constraint matrix whose rows are inspected.</param>
    /// <param name="columnIndexToExternalIndex">Maps retained matrix columns back to the externally visible parameter indices.</param>
    /// <param name="rows">The row indices to inspect.</param>
    /// <param name="tol">The tolerance used to decide whether a matrix entry is treated as non-zero.</param>
    private static void MarkColumns(
        bool[] mask, Matrix<double> M,
        IReadOnlyList<int> columnIndexToExternalIndex,
        IEnumerable<int> rows, double tol)
    {
      foreach (int r in rows)
        for (int c = 0; c < M.ColumnCount; c++)
          if (Math.Abs(M[r, c]) > tol)
            mask[columnIndexToExternalIndex[c]] = true;
    }

    // ---------------------------------------------------------------
    // KKT system
    //
    // [ I      A^T    Cact^T ]   [ dx          ]   [ -grad ]
    // [ A      0      0      ] * [ lambda_eq   ] = [  0    ]
    // [ Cact   0      0      ]   [ lambda_ineq ]   [  0    ]
    // ---------------------------------------------------------------

    /// <summary>
    /// Builds the KKT matrix for the current active-set subproblem.
    /// </summary>
    /// <param name="Cact">The matrix containing the active inequality rows, or <see langword="null"/> if none are active.</param>
    /// <returns>The assembled KKT matrix.</returns>
    private Matrix<double> BuildKKT(Matrix<double>? Cact)
    {
      int internalParameterCount = _columnIndexToExternalIndex.Length;
      int nEq = _A?.RowCount ?? 0;
      int nAct = Cact?.RowCount ?? 0;
      int sz = internalParameterCount + nEq + nAct;

      var KKT = Matrix<double>.Build.Dense(sz, sz);

      for (int i = 0; i < internalParameterCount; i++)
        KKT[i, i] = 1.0;

      if (_A != null)
      {
        KKT.SetSubMatrix(internalParameterCount, 0, _A);
        KKT.SetSubMatrix(0, internalParameterCount, _A.Transpose());
      }

      if (Cact != null)
      {
        KKT.SetSubMatrix(internalParameterCount + nEq, 0, Cact);
        KKT.SetSubMatrix(0, internalParameterCount + nEq, Cact.Transpose());
      }

      return KKT;
    }

    /// <summary>
    /// Builds the right-hand side vector for the KKT system.
    /// </summary>
    /// <param name="x">The current iterate.</param>
    /// <param name="grad">The gradient of the objective at the current iterate.</param>
    /// <param name="Cact">The matrix containing the active inequality rows, or <see langword="null"/> if none are active.</param>
    /// <param name="dAct">The right-hand side values corresponding to <paramref name="Cact"/>, or <see langword="null"/> if none are active.</param>
    /// <returns>The right-hand side vector for the KKT solve.</returns>
    private Vector<double> BuildKKTRhs(Vector<double> x, Vector<double> grad, Matrix<double>? Cact, Vector<double>? dAct)
    {
      int internalParameterCount = _columnIndexToExternalIndex.Length;
      int nEq = _A?.RowCount ?? 0;
      int nAct = Cact?.RowCount ?? 0;
      var rhs = Vector<double>.Build.Dense(internalParameterCount + nEq + nAct);

      for (int i = 0; i < internalParameterCount; i++)
        rhs[i] = -grad[i];

      if (_A != null)
      {
        var equalityResidual = _b! - _A * x;
        for (int i = 0; i < nEq; i++)
          rhs[internalParameterCount + i] = equalityResidual[i];
      }

      if (Cact != null && dAct != null)
      {
        var activeInequalityResidual = dAct - Cact * x;
        for (int i = 0; i < nAct; i++)
          rhs[internalParameterCount + nEq + i] = activeInequalityResidual[i];
      }

      return rhs;
    }

    /// <summary>
    /// Solves the specified KKT system.
    /// </summary>
    /// <param name="KKT">The KKT matrix.</param>
    /// <param name="rhs">The right-hand side vector.</param>
    /// <returns>The solution vector, or <see langword="null"/> if the system cannot be solved.</returns>
    private static Vector<double>? SolveKKTDirect(Matrix<double> KKT, Vector<double> rhs)
    {
      try
      {
        return KKT.Solve(rhs);
      }
      catch
      {
        return null;
      }
    }

    /// <summary>
    /// Solves the specified KKT system with a least-squares fallback.
    /// </summary>
    /// <param name="KKT">The KKT matrix.</param>
    /// <param name="rhs">The right-hand side vector.</param>
    /// <returns>The solution vector, or <see langword="null"/> if the system cannot be solved.</returns>
    private static Vector<double>? SolveKKTLeastSquares(Matrix<double> KKT, Vector<double> rhs)
    {
      try
      {
        return KKT.PseudoInverse() * rhs;
      }
      catch
      {
        return null;
      }
    }

    // ---------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------

    /// <summary>
    /// Simplifies the constraint system and promotes tight opposing inequalities to equalities.
    /// </summary>
    /// <param name="A">The equality-constraint matrix.</param>
    /// <param name="b">The equality-constraint right-hand side vector.</param>
    /// <param name="C">The inequality-constraint matrix.</param>
    /// <param name="d">The inequality-constraint right-hand side vector.</param>
    /// <returns>The normalized constraint system, with the same number of columns, but maybe fewer rows in the inequality system. In the equality system, they can be larger.</returns>
    internal static (Matrix<double>? A, Vector<double>? b, Matrix<double>? C, Vector<double>? d) NormalizeConstraints(
      Matrix<double>? A,
      Vector<double>? b,
      Matrix<double>? C,
      Vector<double>? d)
    {
      var simplifiedEqualities = SimplifyEqualityConstraints(A, b);
      var simplifiedInequalities = SimplifyInequalityConstraints(C, d);
      return PromoteTightOpposingInequalitiesToEqualities(
        simplifiedEqualities.A,
        simplifiedEqualities.b,
        simplifiedInequalities.C,
        simplifiedInequalities.d);
    }

    /// <summary>
    /// Gets the number of matrix columns in the current constraint system.
    /// </summary>
    /// <param name="A">The equality-constraint matrix.</param>
    /// <param name="C">The inequality-constraint matrix.</param>
    /// <returns>The number of matrix columns.</returns>
    private static int GetConstraintColumnCount(Matrix<double>? A, Matrix<double>? C)
    {
      return A?.ColumnCount ?? C?.ColumnCount ?? 0;
    }

    /// <summary>
    /// Builds the reduced parameter vector that corresponds to the currently retained matrix columns.
    /// </summary>
    /// <param name="externalParameters">The externally visible parameter vector.</param>
    /// <returns>The parameter vector in reduced coordinates.</returns>
    private Vector<double> ReduceToInternalParameters(Vector<double> externalParameters)
    {
      var internalParameters = Vector<double>.Build.Dense(_columnIndexToExternalIndex.Length);
      for (int columnIndex = 0; columnIndex < _columnIndexToExternalIndex.Length; columnIndex++)
        internalParameters[columnIndex] = externalParameters[_columnIndexToExternalIndex[columnIndex]];

      return internalParameters;
    }

    /// <summary>
    /// Expands a reduced parameter vector back to the externally visible parameter space.
    /// </summary>
    /// <param name="internalParameters">The reduced parameter vector.</param>
    /// <param name="externalReference">The external reference point providing values for fully free parameters.</param>
    /// <returns>The expanded externally visible parameter vector.</returns>
    private Vector<double> ExpandToExternalParameters(Vector<double> internalParameters, Vector<double> externalReference)
    {
      var externalParameters = externalReference.Clone();

      for (int externalIndex = 0; externalIndex < _n; externalIndex++)
        if (_valuesFixedByConstraints[externalIndex].HasValue)
          externalParameters[externalIndex] = _valuesFixedByConstraints[externalIndex]!.Value;

      for (int columnIndex = 0; columnIndex < _columnIndexToExternalIndex.Length; columnIndex++)
        externalParameters[_columnIndexToExternalIndex[columnIndex]] = internalParameters[columnIndex];

      return externalParameters;
    }

    internal static (Matrix<double>? A, Vector<double>? b, Matrix<double>? C, Vector<double>? d, int[] columnIndexToExternalIndex, bool[] parametersFullyFree) EliminateFullyFreeParameters(Matrix<double>? A, Vector<double>? b, Matrix<double>? C, Vector<double>? d, int[] columnIndexToExternalIndex, bool[] parametersFullyFree)
    {
      int internalParameterCount = columnIndexToExternalIndex.Length;
      if (internalParameterCount == 0)
        return (A, b, C, d, columnIndexToExternalIndex, parametersFullyFree);

      var keepColumnIndices = new List<int>(internalParameterCount);
      for (int columnIndex = 0; columnIndex < internalParameterCount; columnIndex++)
      {
        bool occursInEquality = ColumnHasNonZeroEntry(A, columnIndex, tol: 0);
        bool occursInInequality = ColumnHasNonZeroEntry(C, columnIndex, tol: 0);

        if (occursInEquality || occursInInequality)
        {
          keepColumnIndices.Add(columnIndex);
        }
        else
        {
          int externalIndex = columnIndexToExternalIndex[columnIndex];
          parametersFullyFree[externalIndex] = true;
        }
      }

      if (keepColumnIndices.Count == internalParameterCount)
        return (A, b, C, d, columnIndexToExternalIndex, parametersFullyFree);

      var newColumnIndexToExternalIndex = new int[keepColumnIndices.Count];
      for (int newColumnIndex = 0; newColumnIndex < keepColumnIndices.Count; newColumnIndex++)
      {
        int oldColumnIndex = keepColumnIndices[newColumnIndex];
        int externalIndex = columnIndexToExternalIndex[oldColumnIndex];
        newColumnIndexToExternalIndex[newColumnIndex] = externalIndex;
      }

      A = RetainConstraintColumns(A, keepColumnIndices);
      C = RetainConstraintColumns(C, keepColumnIndices);

      if (A is not null && A.ColumnCount == 0)
      {
        A = null;
        b = null;
      }

      if (C is not null && C.ColumnCount == 0)
      {
        C = null;
        d = null;
      }

      return (A, b, C, d, newColumnIndexToExternalIndex, parametersFullyFree);
    }

    /// <summary>
    /// Determines whether a reduced parameter vector satisfies the retained constraints within the given tolerance.
    /// </summary>
    /// <param name="internalParameters">The reduced parameter vector.</param>
    /// <param name="tol">The tolerance used when evaluating constraint satisfaction.</param>
    /// <returns><see langword="true"/> if the point is feasible; otherwise, <see langword="false"/>.</returns>
    private bool IsReducedPointFeasible(Vector<double> internalParameters, double tol)
    {
      if (internalParameters.Any(v => double.IsNaN(v) || double.IsInfinity(v)))
        return false;

      if (_A != null)
      {
        var equalityResidual = _A * internalParameters - _b!;
        if (equalityResidual.Any(v => double.IsNaN(v) || double.IsInfinity(v)) || equalityResidual.L2Norm() > tol)
          return false;
      }

      if (_C != null)
      {
        var inequalityResidual = _C * internalParameters - _d!;
        if (inequalityResidual.Any(v => double.IsNaN(v) || double.IsInfinity(v) || v > tol))
          return false;
      }

      return true;
    }

    /// <summary>
    /// Eliminates parameters that are fixed by the equality constraints alone.
    /// </summary>
    /// <param name="A">The equality-constraint matrix.</param>
    /// <param name="b">The equality-constraint right-hand side vector.</param>
    /// <param name="C">The inequality-constraint matrix.</param>
    /// <param name="d">The inequality-constraint right-hand side vector.</param>
    /// <param name="valuesFixedByConstraints">The externally indexed array that stores parameter values fixed by constraints.</param>
    /// <param name="columnIndexToExternalIndex">The mapping from retained internal column indices to external parameter indices.</param>
    /// <param name="tol">The comparison tolerance.</param>
    /// <returns>The reduced constraint system together with the updated externally indexed fixed values and column mapping.</returns>
    internal static (Matrix<double>? A, Vector<double>? b, Matrix<double>? C, Vector<double>? d, double?[] valuesFixedByConstraints, int[] columnIndexToExternalIndex) EliminateParametersFixedByEqualities(
      Matrix<double>? A,
      Vector<double>? b,
      Matrix<double>? C,
      Vector<double>? d,
      double?[] valuesFixedByConstraints,
      int[] columnIndexToExternalIndex,
      double tol = 1e-12)
    {
      if (A is null || b is null || A.RowCount == 0)
        return (A, b, C, d, valuesFixedByConstraints, columnIndexToExternalIndex);

      var fixedValues = new double?[A.ColumnCount];
      var feasibilityTolerance = Math.Max(1e-8, 100 * tol);
      var equalityPoint = A.PseudoInverse() * b;
      if ((A * equalityPoint - b).L2Norm() > feasibilityTolerance)
        return (A, b, C, d, valuesFixedByConstraints, columnIndexToExternalIndex);

      var svd = A.Svd(true);
      int rank = svd.Rank;
      int nullity = A.ColumnCount - rank;
      var equalityNullSpaceBasis = BuildNullSpaceBasis(A.ColumnCount, svd.VT, rank, nullity);
      MarkParametersFixedByNullSpace(fixedValues, equalityPoint, equalityNullSpaceBasis, tol);

      return EliminateParametersWithKnownFixedValues(A, b, C, d, fixedValues, valuesFixedByConstraints, columnIndexToExternalIndex);
    }

    /// <summary>
    /// Eliminates parameters whose fixed values are already known in the current matrix representation.
    /// </summary>
    /// <param name="A">The equality-constraint matrix.</param>
    /// <param name="b">The equality-constraint right-hand side vector.</param>
    /// <param name="C">The inequality-constraint matrix.</param>
    /// <param name="d">The inequality-constraint right-hand side vector.</param>
    /// <param name="fixedValues">The fixed parameter values in the current matrix representation.</param>
    /// <param name="valuesFixedByConstraints">The externally indexed array that stores parameter values fixed by constraints.</param>
    /// <param name="columnIndexToExternalIndex">The mapping from retained internal column indices to external parameter indices.</param>
    /// <returns>The reduced constraint system together with the updated externally indexed fixed values and column mapping.</returns>
    private static (Matrix<double>? A, Vector<double>? b, Matrix<double>? C, Vector<double>? d, double?[] valuesFixedByConstraints, int[] columnIndexToExternalIndex) EliminateParametersWithKnownFixedValues(
      Matrix<double>? A,
      Vector<double>? b,
      Matrix<double>? C,
      Vector<double>? d,
      double?[] fixedValues,
      double?[] valuesFixedByConstraints,
      int[] columnIndexToExternalIndex)
    {
      if (!fixedValues.Any(value => value.HasValue))
        return (A, b, C, d, valuesFixedByConstraints, columnIndexToExternalIndex);

      var keepColumnIndices = Enumerable.Range(0, fixedValues.Length)
        .Where(columnIndex => !fixedValues[columnIndex].HasValue)
        .ToList();

      for (int columnIndex = 0; columnIndex < fixedValues.Length; columnIndex++)
      {
        if (!fixedValues[columnIndex].HasValue)
          continue;

        int externalIndex = columnIndexToExternalIndex[columnIndex];
        valuesFixedByConstraints[externalIndex] = fixedValues[columnIndex]!.Value;
      }

      var newColumnIndexToExternalIndex = new int[keepColumnIndices.Count];
      for (int newColumnIndex = 0; newColumnIndex < keepColumnIndices.Count; newColumnIndex++)
      {
        int oldColumnIndex = keepColumnIndices[newColumnIndex];
        int externalIndex = columnIndexToExternalIndex[oldColumnIndex];
        newColumnIndexToExternalIndex[newColumnIndex] = externalIndex;
      }

      (A, b) = EliminateFixedColumnsFromConstraints(A, b, fixedValues, keepColumnIndices);
      (C, d) = EliminateFixedColumnsFromConstraints(C, d, fixedValues, keepColumnIndices);
      return (A, b, C, d, valuesFixedByConstraints, newColumnIndexToExternalIndex);
    }

    /// <summary>
    /// Eliminates fixed parameter columns from a constraint system and moves their contributions to the right-hand side.
    /// </summary>
    /// <param name="matrix">The constraint matrix.</param>
    /// <param name="rhs">The right-hand side vector.</param>
    /// <param name="fixedValues">The fixed values in the current matrix representation.</param>
    /// <param name="keepColumnIndices">The column indices that remain in the matrix.</param>
    /// <returns>The reduced constraint system.</returns>
    private static (Matrix<double>? Matrix, Vector<double>? RightHandSide) EliminateFixedColumnsFromConstraints(
      Matrix<double>? matrix,
      Vector<double>? rhs,
      double?[] fixedValues,
      IReadOnlyList<int> keepColumnIndices)
    {
      if (matrix is null || rhs is null)
        return (matrix, rhs);

      var reducedMatrix = Matrix<double>.Build.Dense(matrix.RowCount, keepColumnIndices.Count);
      var reducedRhs = rhs.Clone();

      for (int rowIndex = 0; rowIndex < matrix.RowCount; rowIndex++)
      {
        double fixedContribution = 0;
        for (int columnIndex = 0; columnIndex < fixedValues.Length; columnIndex++)
        {
          if (fixedValues[columnIndex].HasValue)
            fixedContribution += matrix[rowIndex, columnIndex] * fixedValues[columnIndex]!.Value;
        }

        reducedRhs[rowIndex] -= fixedContribution;

        for (int newColumnIndex = 0; newColumnIndex < keepColumnIndices.Count; newColumnIndex++)
        {
          int oldColumnIndex = keepColumnIndices[newColumnIndex];
          reducedMatrix[rowIndex, newColumnIndex] = matrix[rowIndex, oldColumnIndex];
        }
      }

      return (reducedMatrix, reducedRhs);
    }

    /// <summary>
    /// Retains only the specified columns of a constraint matrix.
    /// </summary>
    /// <param name="matrix">The constraint matrix.</param>
    /// <param name="keepColumnIndices">The column indices that remain in the matrix.</param>
    /// <returns>The reduced matrix, or <see langword="null"/> if the input matrix is <see langword="null"/>.</returns>
    private static Matrix<double>? RetainConstraintColumns(Matrix<double>? matrix, IReadOnlyList<int> keepColumnIndices)
    {
      if (matrix is null)
        return null;

      var reducedMatrix = Matrix<double>.Build.Dense(matrix.RowCount, keepColumnIndices.Count);
      for (int rowIndex = 0; rowIndex < matrix.RowCount; rowIndex++)
      {
        for (int newColumnIndex = 0; newColumnIndex < keepColumnIndices.Count; newColumnIndex++)
        {
          int oldColumnIndex = keepColumnIndices[newColumnIndex];
          reducedMatrix[rowIndex, newColumnIndex] = matrix[rowIndex, oldColumnIndex];
        }
      }

      return reducedMatrix;
    }

    /// <summary>
    /// Determines whether the specified matrix column contains a non-zero entry.
    /// </summary>
    /// <param name="matrix">The matrix to inspect.</param>
    /// <param name="columnIndex">The column index to inspect.</param>
    /// <param name="tol">The tolerance used to decide whether an entry is treated as non-zero.</param>
    /// <returns><see langword="true"/> if the column contains a non-zero entry; otherwise, <see langword="false"/>.</returns>
    private static bool ColumnHasNonZeroEntry(Matrix<double>? matrix, int columnIndex, double tol)
    {
      if (matrix is null)
        return false;

      for (int rowIndex = 0; rowIndex < matrix.RowCount; rowIndex++)
      {
        if (Math.Abs(matrix[rowIndex, columnIndex]) > tol)
          return true;
      }

      return false;
    }

    /// <summary>
    /// Simplifies equality constraints by removing duplicate rows that represent the same equation.
    /// </summary>
    /// <param name="A">The equality-constraint matrix.</param>
    /// <param name="b">The equality-constraint right-hand side vector.</param>
    /// <returns>The simplified equality-constraint system, with the same number of columns, but maybe fewer rows.</returns>
    internal static (Matrix<double>? A, Vector<double>? b) SimplifyEqualityConstraints(Matrix<double>? A, Vector<double>? b)
    {
      ValidateConstraintDimensions(A, b, nameof(A), nameof(b));

      if (A is null || b is null || A.RowCount == 0)
        return (A, b);

      const double tol = 1e-12;
      var uniqueRows = new List<Vector<double>>();
      var uniqueRhs = new List<double>();

      for (int i = 0; i < A.RowCount; i++)
      {
        var row = A.Row(i);
        double rhs = b[i];

        if (row.L2Norm() <= tol)
        {
          if (Math.Abs(rhs) <= tol)
            continue;

          uniqueRows.Add(row);
          uniqueRhs.Add(rhs);
          continue;
        }

        bool isDuplicate = false;
        for (int j = 0; j < uniqueRows.Count; j++)
        {
          if (TryGetEqualityScale(uniqueRows[j], uniqueRhs[j], row, rhs, tol, out _))
          {
            isDuplicate = true;
            break;
          }
        }

        if (!isDuplicate && uniqueRows.Count > 0)
        {
          var currentSystem = BuildConstraintSystem(uniqueRows, uniqueRhs, A.ColumnCount);
          if (currentSystem.M is not null)
          {
            var coefficients = currentSystem.M.Transpose().PseudoInverse() * row;
            var reconstructedRow = currentSystem.M.Transpose() * coefficients;
            double reconstructedRhs = 0;
            for (int j = 0; j < coefficients.Count; j++)
              reconstructedRhs += coefficients[j] * uniqueRhs[j];

            if ((reconstructedRow - row).L2Norm() <= tol * Math.Max(1.0, row.L2Norm())
              && Math.Abs(reconstructedRhs - rhs) <= tol * Math.Max(1.0, Math.Max(Math.Abs(reconstructedRhs), Math.Abs(rhs))))
            {
              isDuplicate = true;
            }
          }
        }

        if (!isDuplicate)
        {
          uniqueRows.Add(row);
          uniqueRhs.Add(rhs);
        }
      }

      return BuildConstraintSystem(uniqueRows, uniqueRhs, A.ColumnCount);
    }

    /// <summary>
    /// Simplifies inequality constraints by removing duplicate rows and keeping only the tightest bound for equivalent left-hand sides.
    /// </summary>
    /// <param name="C">The inequality-constraint matrix.</param>
    /// <param name="d">The inequality-constraint right-hand side vector.</param>
    /// <returns>The simplified inequality-constraint system, with the same number of columns, but maybe fewer rows.</returns>
    internal static (Matrix<double>? C, Vector<double>? d) SimplifyInequalityConstraints(Matrix<double>? C, Vector<double>? d)
    {
      ValidateConstraintDimensions(C, d, nameof(C), nameof(d));

      if (C is null || d is null || C.RowCount == 0)
        return (C, d);

      const double tol = 1e-12;
      var uniqueRows = new List<Vector<double>>();
      var uniqueRhs = new List<double>();

      for (int i = 0; i < C.RowCount; i++)
      {
        var row = C.Row(i);
        double rhs = d[i];

        if (row.L2Norm() <= tol)
        {
          if (rhs >= -tol)
            continue;

          uniqueRows.Add(row);
          uniqueRhs.Add(rhs);
          continue;
        }

        int equivalentIndex = -1;
        double scale = 1;
        for (int j = 0; j < uniqueRows.Count; j++)
        {
          if (TryGetPositiveScale(uniqueRows[j], row, tol, out scale))
          {
            equivalentIndex = j;
            break;
          }
        }

        if (equivalentIndex >= 0)
        {
          double normalizedRhs = rhs / scale;
          if (normalizedRhs < uniqueRhs[equivalentIndex])
            uniqueRhs[equivalentIndex] = normalizedRhs;
        }
        else
        {
          uniqueRows.Add(row);
          uniqueRhs.Add(rhs);
        }
      }

      return BuildConstraintSystem(uniqueRows, uniqueRhs, C.ColumnCount);
    }

    /// <summary>
    /// Determines whether the specified constraint system contains incompatible equality or inequality constraints.
    /// </summary>
    /// <param name="n">The number of parameters.</param>
    /// <param name="A">The equality-constraint matrix in <c>A·x = b</c>.</param>
    /// <param name="b">The equality-constraint right-hand side vector.</param>
    /// <param name="C">The inequality-constraint matrix in <c>C·x &lt;= d</c>.</param>
    /// <param name="d">The inequality-constraint right-hand side vector.</param>
    /// <param name="tol">The tolerance used for proportionality, bound, and feasibility comparisons.</param>
    /// <returns><see langword="true"/> if incompatible constraints are detected; otherwise, <see langword="false"/>.</returns>
    internal static bool HasIncompatibleConstraints(int n, Matrix<double>? A, Vector<double>? b, Matrix<double>? C, Vector<double>? d, double tol = 1e-12)
    {
      ValidateConstraintDimensions(A, b, nameof(A), nameof(b));
      ValidateConstraintDimensions(C, d, nameof(C), nameof(d));

      ValidateConstraintColumnCount(n, A, nameof(A));
      ValidateConstraintColumnCount(n, C, nameof(C));

      var feasibilityTolerance = Math.Max(1e-8, 100 * tol);

      if ((A is null || b is null || A.RowCount == 0) && (C is null || d is null || C.RowCount == 0))
        return false;

      if (b is not null && (A is null || A.ColumnCount == 0))
      {
        for (int rowIndex = 0; rowIndex < b.Count; rowIndex++)
        {
          if (Math.Abs(b[rowIndex]) > feasibilityTolerance)
            return true;
        }
      }

      if (b is not null && (A is null || A.ColumnCount == 0 || A.RowCount == 0))
      {
        A = null;
        b = null;
      }


      if (d is not null && (C is null || C.ColumnCount == 0))
      {
        for (int rowIndex = 0; rowIndex < d.Count; rowIndex++)
        {
          if (d[rowIndex] < -feasibilityTolerance)
            return true;
        }
      }

      if (d is not null && (C is null || C.ColumnCount == 0 || C.RowCount == 0))
      {
        C = null;
        d = null;
      }

      Matrix<double>? equalityNullSpaceBasis = null;

      if (A is not null && b is not null && A.RowCount > 0)
      {
        var equalityPoint = A.PseudoInverse() * b;
        if ((A * equalityPoint - b).L2Norm() > feasibilityTolerance)
          return true;

        var svd = A.Svd(true);
        int rank = svd.Rank;
        int nullity = A.ColumnCount - rank;

        equalityNullSpaceBasis = BuildNullSpaceBasis(A.ColumnCount, svd.VT, rank, nullity);

        if (C is null || d is null || C.RowCount == 0)
          return false;

        var reducedRhs = d - C * equalityPoint;

        if (nullity == 0)
        {
          return reducedRhs.Any(v => v < -feasibilityTolerance);
        }

        var reducedC = C * equalityNullSpaceBasis;
        if (HasDirectlyIncompatibleInequalities(reducedC, reducedRhs, tol))
          return true;

        var reducedFeasiblePoint = FindFeasiblePointForInequalities(nullity, reducedC, reducedRhs, feasibilityTolerance);
        if (reducedFeasiblePoint is null)
          return true;

        var feasiblePointWithEqualities = equalityPoint + equalityNullSpaceBasis * reducedFeasiblePoint;

        return false;
      }

      if (C is not null && d is not null)
      {
        if (HasDirectlyIncompatibleInequalities(C, d, tol))
          return true;

        var feasiblePoint = FindFeasiblePointForInequalities(n, C, d, feasibilityTolerance);
        if (feasiblePoint is null)
          return true;
      }

      return false;
    }

    /// <summary>
    /// Attempts to find a feasible point for a system with equality and inequality constraints by solving the inequalities in null-space coordinates.
    /// </summary>
    /// <param name="A">The equality-constraint matrix.</param>
    /// <param name="b">The equality-constraint right-hand side vector.</param>
    /// <param name="C">The inequality-constraint matrix.</param>
    /// <param name="d">The inequality-constraint right-hand side vector.</param>
    /// <param name="tol">The feasibility tolerance.</param>
    /// <returns>A feasible point if one is found; otherwise, <see langword="null"/>.</returns>
    private static Vector<double>? FindFeasiblePointForEqualitiesAndInequalities(Matrix<double> A, Vector<double> b, Matrix<double> C, Vector<double> d, double tol)
    {
      var equalityPoint = A.PseudoInverse() * b;
      if ((A * equalityPoint - b).L2Norm() > tol)
        return null;

      var svd = A.Svd(true);
      int rank = svd.Rank;
      int nullity = A.ColumnCount - rank;
      if (nullity == 0)
      {
        var reducedRhs0 = d - C * equalityPoint;
        return reducedRhs0.Any(value => value < -tol) ? null : equalityPoint;
      }

      var equalityNullSpaceBasis = BuildNullSpaceBasis(A.ColumnCount, svd.VT, rank, nullity);
      var reducedRhs = d - C * equalityPoint;
      var reducedC = C * equalityNullSpaceBasis;
      var reducedFeasiblePoint = FindFeasiblePointForInequalities(nullity, reducedC, reducedRhs, tol);
      if (reducedFeasiblePoint is null)
        return null;

      return equalityPoint + equalityNullSpaceBasis * reducedFeasiblePoint;
    }

    /// <summary>
    /// Builds a basis of the null space of an equality-constraint matrix from its singular-value decomposition.
    /// </summary>
    /// <param name="columnCount">The number of columns of the original matrix.</param>
    /// <param name="vt">The transposed right-singular-vector matrix.</param>
    /// <param name="rank">The rank of the original matrix.</param>
    /// <param name="nullity">The nullity of the original matrix.</param>
    /// <returns>
    /// A matrix whose columns span the null space, or an empty matrix when the null space is trivial.
    /// </returns>
    private static Matrix<double> BuildNullSpaceBasis(int columnCount, Matrix<double> vt, int rank, int nullity)
    {
      var nullSpaceBasis = Matrix<double>.Build.Dense(columnCount, nullity);
      for (int basisColumn = 0; basisColumn < nullity; basisColumn++)
      {
        int vtRow = rank + basisColumn;
        for (int row = 0; row < columnCount; row++)
          nullSpaceBasis[row, basisColumn] = vt[vtRow, row];
      }

      return nullSpaceBasis;
    }

    /// <summary>
    /// Stores the values of parameters that are fixed by the equality constraints alone.
    /// </summary>
    /// <param name="fixedValues">The fixed-value array to update.</param>
    /// <param name="feasibleEqualityPoint">A point satisfying the equality constraints.</param>
    /// <param name="nullSpaceBasis">The null-space basis of the equality-constraint matrix.</param>
    /// <param name="tol">The tolerance used to decide whether an entry is treated as non-zero.</param>
    private static void MarkParametersFixedByNullSpace(double?[] fixedValues, Vector<double> feasibleEqualityPoint, Matrix<double> nullSpaceBasis, double tol)
    {
      if (nullSpaceBasis.ColumnCount == 0)
      {
        for (int parameterIndex = 0; parameterIndex < feasibleEqualityPoint.Count; parameterIndex++)
          fixedValues[parameterIndex] = feasibleEqualityPoint[parameterIndex];

        return;
      }

      for (int parameterIndex = 0; parameterIndex < nullSpaceBasis.RowCount; parameterIndex++)
      {
        bool canVary = false;
        for (int basisColumn = 0; basisColumn < nullSpaceBasis.ColumnCount; basisColumn++)
        {
          if (Math.Abs(nullSpaceBasis[parameterIndex, basisColumn]) > tol)
          {
            canVary = true;
            break;
          }
        }

        if (!canVary)
          fixedValues[parameterIndex] = feasibleEqualityPoint[parameterIndex];
      }
    }

    /// <summary>
    /// Removes rows from the matrix and vector where all matrix elements are zero and the corresponding vector element
    /// is also zero.
    /// </summary>
    /// <param name="A">The matrix to process for empty rows.</param>
    /// <param name="b">The vector associated with the matrix rows.</param>
    /// <param name="isEqualitySystem">Indicates whether the matrix/vector system is the equality system or not.</param>
    /// <param name="matrixNameForDiagnostics">The name of the matrix parameter for diagnostic messages.</param>
    /// <param name="vectorNameForDiagnostics">The name of the vector parameter for diagnostic messages.</param>
    /// <returns>A tuple containing the matrix and vector with empty rows removed.</returns>
    /// <exception cref="InvalidDataException">Thrown when an empty matrix row corresponds to a nonzero vector element.</exception>
    /// <remarks>If no elements were needed to remove, the original matrix and vector is returned; otherwise, shrinked copies are returned.</remarks>
    private static (Matrix<double>? A, Vector<double>? b) EliminateEmptyRows(Matrix<double> A, Vector<double> b, bool isEqualitySystem, [CallerArgumentExpression(nameof(A))] string matrixNameForDiagnostics = "", [CallerArgumentExpression(nameof(b))] string vectorNameForDiagnostics = "")
    {
      var isRowEmpty = new bool[A.RowCount];

      int numberOfEmptyRows = 0;
      for (int iR = 0; iR < A.RowCount; ++iR)
      {
        bool isEmpty = true;
        for (int iC = 0; iC < A.ColumnCount; ++iC)
          if (!(A[iR, iC] == 0))
          {
            isEmpty = false;
            break;
          }
        isRowEmpty[iR] = isEmpty;
        if (isEmpty)
        {
          ++numberOfEmptyRows;

          if (isEqualitySystem)
          {
            if (!(b[iR] == 0))
              throw new InvalidDataException($"Unfeasible: The row [{iR}] of matrix {matrixNameForDiagnostics} is empty, but the vector element {vectorNameForDiagnostics}[{iR}] contains a value of {b[iR]}");
          }
          else
          {
            if (!(b[iR] >= 0))
              throw new InvalidDataException($"Unfeasible: The row [{iR}] of matrix {matrixNameForDiagnostics} is empty, but the vector element {vectorNameForDiagnostics}[{iR}] contains a value of {b[iR]}");
          }
        }
      }

      if (numberOfEmptyRows == 0)
      {
        return (A, b);
      }
      else
      {
        var An = CreateMatrix.Dense<double>(A.RowCount - numberOfEmptyRows, A.ColumnCount);
        var bn = CreateVector.Dense<double>(A.RowCount - numberOfEmptyRows);

        for (int iR = 0, iRn = 0; iR < A.RowCount; ++iR)
        {
          if (isRowEmpty[iR])
            continue;

          for (int iC = 0; iC < A.ColumnCount; ++iC)
            An[iRn, iC] = A[iR, iC];

          bn[iRn] = b[iR];

          ++iRn;
        }

        return An.RowCount == 0 ? (null, null) : (An, bn);
      }
    }

    /// <summary>
    /// Eliminates trivial fixed constraints from the equality and inequality constraint matrices, updating fixed
    /// variable values and mapping indices accordingly.
    /// </summary>
    /// <param name="A">The equality constraint matrix.</param>
    /// <param name="b">The right-hand side vector for equality constraints.</param>
    /// <param name="C">The inequality constraint matrix.</param>
    /// <param name="d">The right-hand side vector for inequality constraints.</param>
    /// <param name="valuesFixedByConstraints">The array storing values fixed by constraints for each variable.</param>
    /// <param name="columnIndexToExternalIndex">The mapping from internal column indices to external indices.</param>
    /// <returns>A tuple containing the updated equality and inequality constraint matrices and vectors, the updated fixed values
    /// array, and the updated column index mapping.</returns>
    /// <exception cref="InvalidDataException">Thrown when conflicting constraints lead to infeasible fixed values for a variable.</exception>
    internal static (Matrix<double>? A, Vector<double>? b, Matrix<double>? C, Vector<double>? d, double?[] valuesFixedByConstraints, int[] columnIndexToExternalIndex) EliminateTrivialFixedParameters(Matrix<double>? A, Vector<double>? b, Matrix<double>? C, Vector<double>? d, double?[] valuesFixedByConstraints, int[] columnIndexToExternalIndex)
    {
      if (A is null || b is null || A.ColumnCount == 0)
        return (A, b, C, d, valuesFixedByConstraints, columnIndexToExternalIndex); // nothing to do because no equality constraints are present

      var fixedValue = new double?[A.ColumnCount];
      var isRowWithFixedValue = new bool[A.RowCount];

      for (int iR = 0; iR < A.RowCount; ++iR)
      {
        int? indexOfNonZeroElement = null;

        for (int iC = 0; iC < A.ColumnCount; ++iC)
        {
          if (!(A[iR, iC] == 0))
          {
            if (indexOfNonZeroElement.HasValue)
            {
              indexOfNonZeroElement = null;
              break; // this row does not contain a trivial fixed constraint, because we have at least two non-zero elements in the row
            }
            else
            {
              indexOfNonZeroElement = iC;
            }
          }
        }
        if (indexOfNonZeroElement.HasValue)
        {
          isRowWithFixedValue[iR] = true;
          var fixedV = b[iR] / A[iR, indexOfNonZeroElement.Value];

          if (fixedValue[indexOfNonZeroElement.Value] is { } existingFixed && !(fixedV == existingFixed))
            throw new InvalidDataException($"Infeasible: Two constraints leading to fixed values of {fixedV} and {existingFixed}");

          if (valuesFixedByConstraints[columnIndexToExternalIndex[indexOfNonZeroElement.Value]] is { } existingFixed2 && !(existingFixed2 == fixedV))
            throw new InvalidDataException($"Infeasible: Two constraints leading to fixed values of {fixedV} and {existingFixed2}");

          fixedValue[indexOfNonZeroElement.Value] = fixedV;
          valuesFixedByConstraints[columnIndexToExternalIndex[indexOfNonZeroElement.Value]] = fixedV;
        }
      }

      // now remove the columns corresponding to the fixed values from _A and _C

      int numberOfFixedValues = fixedValue.Count(x => x.HasValue);

      if (numberOfFixedValues == 0)
      {
        return (A, b, C, d, valuesFixedByConstraints, columnIndexToExternalIndex); // nothing to do because no trivial fixed constraints are present
      }


      int numberOfRowsWithFixedValues = isRowWithFixedValue.Count(x => x == true);

      var columnIndexToExternalIndexNew = new int[A.ColumnCount - numberOfFixedValues];
      for (int iC = 0, iCn = 0; iC < A.ColumnCount; ++iC)
      {
        if (!fixedValue[iC].HasValue)
        {
          columnIndexToExternalIndexNew[iCn] = columnIndexToExternalIndex[iC];
          iCn++;
        }
      }

      var An = CreateMatrix.Dense<double>(A.RowCount - numberOfRowsWithFixedValues, A.ColumnCount - numberOfFixedValues);
      var bn = CreateVector.Dense<double>(A.RowCount - numberOfRowsWithFixedValues);

      for (int iR = 0, iRn = 0; iR < A.RowCount; ++iR)
      {
        if (!isRowWithFixedValue[iR])
        {
          bn[iRn] = b[iR];
          for (int iC = 0, iCn = 0; iC < A.ColumnCount; ++iC)
          {
            if (fixedValue[iC] is { } fixedV)
            {
              bn[iRn] -= A[iR, iC] * fixedV;
            }
            else
            {
              An[iRn, iCn] = A[iR, iC];
              iCn++;
            }
          }
          ++iRn;
        }
      }

      var Cn = C;
      var dn = d;
      if (C is not null && d is not null)
      {
        Cn = CreateMatrix.Dense<double>(C.RowCount, C.ColumnCount - numberOfFixedValues);
        dn = CreateVector.Dense<double>(C.RowCount);

        for (int iR = 0; iR < C.RowCount; ++iR)
        {
          dn[iR] = d[iR];
          for (int iC = 0, iCn = 0; iC < C.ColumnCount; ++iC)
          {
            if (fixedValue[iC] is { } fixedV)
            {
              dn[iR] -= C[iR, iC] * fixedV;
            }
            else
            {
              Cn[iR, iCn] = C[iR, iC];
              iCn++;
            }
          }
        }
      }

      if (An.ColumnCount == 0 || An.RowCount == 0)
      {
        // all values in bn should be zero then, otherwise the equality constraints would not be feasible
        if (bn.Any(x => !(x == 0)))
          throw new InvalidDataException($"Infeasible: the equality constraints do not allow a feasible solution");

        An = null;
        bn = null;
      }

      if (Cn is not null && (Cn.ColumnCount == 0 || Cn.RowCount == 0))
      {
        // all values in dn should be zero or positive then, otherwise the inequality constraints would not be feasible
        if (dn.Any(x => !(x >= 0)))
          throw new InvalidDataException($"Infeasible: the inequality constraints do not allow a feasible solution");

        Cn = null;
        dn = null;
      }

      return (An, bn, Cn, dn, valuesFixedByConstraints, columnIndexToExternalIndexNew);
    }

    /// <summary>
    /// Converts pairs of tight opposing inequalities into equalities.
    /// </summary>
    /// <param name="A">The equality-constraint matrix.</param>
    /// <param name="b">The equality-constraint right-hand side vector.</param>
    /// <param name="C">The inequality-constraint matrix.</param>
    /// <param name="d">The inequality-constraint right-hand side vector.</param>
    /// <param name="tol">The proportionality tolerance.</param>
    /// <returns>The combined constraint system with promoted equalities, with the same number of columns, but maybe fewer rows.</returns>
    internal static (Matrix<double>? A, Vector<double>? b, Matrix<double>? C, Vector<double>? d) PromoteTightOpposingInequalitiesToEqualities(
      Matrix<double>? A,
      Vector<double>? b,
      Matrix<double>? C,
      Vector<double>? d,
      double tol = 1e-12)
    {
      if (C is null || d is null || C.RowCount == 0)
        return (A, b, C, d);

      var equalityRows = new List<Vector<double>>();
      var equalityRhs = new List<double>();
      if (A is not null && b is not null)
      {
        for (int i = 0; i < A.RowCount; i++)
        {
          equalityRows.Add(A.Row(i));
          equalityRhs.Add(b[i]);
        }
      }

      var usedInequalities = new bool[C.RowCount];

      for (int i = 0; i < C.RowCount; i++)
      {
        if (usedInequalities[i])
          continue;

        var referenceRow = C.Row(i);
        for (int j = i + 1; j < C.RowCount; j++)
        {
          if (usedInequalities[j])
            continue;

          if (!TryGetNonZeroScale(referenceRow, C.Row(j), tol, out double scale) || scale >= -tol)
            continue;

          double lowerBound = d[j] / scale;
          double boundTolerance = tol * Math.Max(1.0, Math.Max(Math.Abs(lowerBound), Math.Abs(d[i])));
          if (Math.Abs(lowerBound - d[i]) > boundTolerance)
            continue;

          equalityRows.Add(referenceRow);
          equalityRhs.Add(d[i]);
          usedInequalities[i] = true;
          usedInequalities[j] = true;
          break;
        }
      }

      var remainingRows = new List<Vector<double>>();
      var remainingRhs = new List<double>();
      for (int i = 0; i < C.RowCount; i++)
      {
        if (!usedInequalities[i])
        {
          remainingRows.Add(C.Row(i));
          remainingRhs.Add(d[i]);
        }
      }

      var promotedEqualities = BuildConstraintSystem(equalityRows, equalityRhs, C.ColumnCount);
      var remainingInequalities = BuildConstraintSystem(remainingRows, remainingRhs, C.ColumnCount);
      var simplifiedEqualities = SimplifyEqualityConstraints(promotedEqualities.M, promotedEqualities.v);
      var simplifiedInequalities = SimplifyInequalityConstraints(remainingInequalities.M, remainingInequalities.v);
      return (simplifiedEqualities.A, simplifiedEqualities.b, simplifiedInequalities.C, simplifiedInequalities.d);
    }

    /// <summary>
    /// Gets the indices of the inequality constraints that are active at the specified point.
    /// </summary>
    /// <param name="x">The point to test.</param>
    /// <param name="tol">The tolerance used to detect active inequalities.</param>
    /// <returns>The indices of the active inequality constraints.</returns>
    private HashSet<int> GetActiveInequalityIndices(Vector<double> x, double tol)
    {
      var activeSet = new HashSet<int>();
      if (_C is null || _d is null)
        return activeSet;

      for (int i = 0; i < _C.RowCount; i++)
      {
        double slack = _d[i] - _C.Row(i) * x;
        if (Math.Abs(slack) <= tol)
          activeSet.Add(i);
      }

      return activeSet;
    }

    /// <summary>
    /// Attempts to find a feasible point for an inequality-only system by enumerating active sets.
    /// </summary>
    /// <param name="parameterCount">The number of parameters.</param>
    /// <param name="C">The inequality-constraint matrix.</param>
    /// <param name="d">The inequality-constraint right-hand side vector.</param>
    /// <param name="tol">The feasibility tolerance.</param>
    /// <returns>A feasible point if one is found; otherwise, <see langword="null"/>.</returns>
    private static Vector<double>? FindFeasiblePointForInequalities(int parameterCount, Matrix<double> C, Vector<double> d, double tol)
    {
      var seen = new HashSet<string>(StringComparer.Ordinal);

      for (int subsetSize = 0; subsetSize <= Math.Min(parameterCount, C.RowCount); subsetSize++)
      {
        foreach (var subset in EnumerateIndexSubsets(C.RowCount, subsetSize))
        {
          string key = string.Join(",", subset);
          if (!seen.Add(key))
            continue;

          Vector<double> candidate;
          if (subset.Count == 0)
          {
            candidate = Vector<double>.Build.Dense(parameterCount);
          }
          else
          {
            var activeRows = subset.Select(index => C.Row(index)).ToList();
            var activeRhs = subset.Select(index => d[index]).ToList();
            var (Aeq, beq) = BuildConstraintSystem(activeRows, activeRhs, parameterCount);
            if (Aeq is null || beq is null)
              continue;

            try
            {
              candidate = Aeq.PseudoInverse() * beq;
            }
            catch
            {
              continue;
            }

            var equalityResidual = Aeq * candidate - beq;
            if (equalityResidual.Any(value => double.IsNaN(value) || double.IsInfinity(value)) || equalityResidual.L2Norm() > tol)
              continue;
          }

          var slack = d - C * candidate;
          if (slack.Any(value => double.IsNaN(value) || double.IsInfinity(value) || value < -tol))
            continue;

          return candidate;
        }
      }

      return null;
    }

    /// <summary>
    /// Enumerates all index subsets of the specified size.
    /// </summary>
    /// <param name="count">The total number of available indices.</param>
    /// <param name="subsetSize">The number of indices per subset.</param>
    /// <returns>An enumeration of index subsets.</returns>
    private static IEnumerable<IReadOnlyList<int>> EnumerateIndexSubsets(int count, int subsetSize)
    {
      if (subsetSize == 0)
      {
        yield return Array.Empty<int>();
        yield break;
      }

      var indices = Enumerable.Range(0, subsetSize).ToArray();

      while (true)
      {
        yield return (int[])indices.Clone();

        int position = subsetSize - 1;
        while (position >= 0 && indices[position] == count - subsetSize + position)
          position--;

        if (position < 0)
          yield break;

        indices[position]++;
        for (int i = position + 1; i < subsetSize; i++)
          indices[i] = indices[i - 1] + 1;
      }
    }

    /// <summary>
    /// Determines whether the specified inequality system contains directly incompatible constraints.
    /// </summary>
    /// <param name="C">The inequality-constraint matrix in <c>C·x &lt;= d</c>.</param>
    /// <param name="d">The inequality-constraint right-hand side vector.</param>
    /// <param name="tol">The tolerance used for proportionality and bound comparisons.</param>
    /// <returns><see langword="true"/> if directly incompatible inequalities are detected; otherwise, <see langword="false"/>.</returns>
    private static bool HasDirectlyIncompatibleInequalities(Matrix<double>? C, Vector<double>? d, double tol)
    {
      if (C is null || d is null || C.RowCount == 0)
        return false;

      for (int i = 0; i < C.RowCount; i++)
      {
        var row = C.Row(i);
        if (row.L2Norm() <= tol && d[i] < -tol)
          return true;
      }

      for (int i = 0; i < C.RowCount; i++)
      {
        var referenceRow = C.Row(i);
        double upperBound = d[i];

        for (int j = i + 1; j < C.RowCount; j++)
        {
          if (!TryGetNonZeroScale(referenceRow, C.Row(j), tol, out var scale) || scale >= -tol)
            continue;

          double lowerBound = d[j] / scale;
          double boundTolerance = tol * Math.Max(1.0, Math.Max(Math.Abs(lowerBound), Math.Abs(upperBound)));
          if (lowerBound > upperBound + boundTolerance)
            return true;
        }
      }

      return false;
    }

    /// <summary>
    /// Validates that a constraint matrix and right-hand side vector use compatible dimensions.
    /// </summary>
    /// <param name="matrix">The constraint matrix.</param>
    /// <param name="rhs">The right-hand side vector.</param>
    /// <param name="matrixParamName">The parameter name of the matrix argument.</param>
    /// <param name="rhsParamName">The parameter name of the vector argument.</param>
    private static void ValidateConstraintDimensions(Matrix<double>? matrix, Vector<double>? rhs, string matrixParamName, string rhsParamName)
    {
      if ((matrix is null) != (rhs is null))
        throw new ArgumentException("Constraint matrix and right-hand side must either both be provided or both be null.", matrix is null ? rhsParamName : matrixParamName);

      if (matrix is not null && rhs is not null && matrix.RowCount != rhs.Count)
        throw new ArgumentException("The number of constraint rows must match the number of right-hand side entries.", rhsParamName);
    }

    /// <summary>
    /// Validates that a constraint matrix uses the expected parameter dimension.
    /// </summary>
    /// <param name="parameterCount">The expected number of parameters.</param>
    /// <param name="matrix">The constraint matrix.</param>
    /// <param name="matrixParamName">The parameter name of the matrix argument.</param>
    private static void ValidateConstraintColumnCount(int parameterCount, Matrix<double>? matrix, string matrixParamName)
    {
      if (matrix is not null && matrix.ColumnCount != parameterCount)
        throw new ArgumentException("The number of constraint columns must match the number of parameters.", matrixParamName);
    }

    /// <summary>
    /// Builds a constraint matrix and right-hand side vector from collected rows.
    /// </summary>
    /// <param name="rows">The constraint rows.</param>
    /// <param name="rhs">The right-hand side values.</param>
    /// <param name="columnCount">The number of matrix columns.</param>
    /// <returns>The constructed constraint system, or <see langword="null"/> values if no rows are present.</returns>
    private static (Matrix<double>? M, Vector<double>? v) BuildConstraintSystem(IReadOnlyList<Vector<double>> rows, IReadOnlyList<double> rhs, int columnCount)
    {
      if (rows.Count == 0)
        return (null, null);

      var matrix = Matrix<double>.Build.Dense(rows.Count, columnCount);
      for (int r = 0; r < rows.Count; r++)
        for (int c = 0; c < columnCount; c++)
          matrix[r, c] = rows[r][c];

      var vector = Vector<double>.Build.DenseOfEnumerable(rhs);
      return (matrix, vector);
    }

    /// <summary>
    /// Determines whether two equality constraints are equivalent up to a non-zero scalar factor.
    /// </summary>
    /// <param name="referenceRow">The reference constraint row.</param>
    /// <param name="referenceRhs">The reference right-hand side value.</param>
    /// <param name="candidateRow">The candidate constraint row.</param>
    /// <param name="candidateRhs">The candidate right-hand side value.</param>
    /// <param name="tol">The comparison tolerance.</param>
    /// <param name="scale">The scalar factor that maps the reference row to the candidate row.</param>
    /// <returns><see langword="true"/> if the constraints are equivalent; otherwise, <see langword="false"/>.</returns>
    private static bool TryGetEqualityScale(Vector<double> referenceRow, double referenceRhs, Vector<double> candidateRow, double candidateRhs, double tol, out double scale)
    {
      if (!TryGetNonZeroScale(referenceRow, candidateRow, tol, out scale))
        return false;

      return Math.Abs(candidateRhs - scale * referenceRhs) <= tol * Math.Max(1.0, Math.Max(Math.Abs(candidateRhs), Math.Abs(scale * referenceRhs)));
    }

    /// <summary>
    /// Determines whether two inequality left-hand sides are equivalent up to a positive scalar factor.
    /// </summary>
    /// <param name="referenceRow">The reference constraint row.</param>
    /// <param name="candidateRow">The candidate constraint row.</param>
    /// <param name="tol">The comparison tolerance.</param>
    /// <param name="scale">The positive scalar factor that maps the reference row to the candidate row.</param>
    /// <returns><see langword="true"/> if the left-hand sides are equivalent; otherwise, <see langword="false"/>.</returns>
    private static bool TryGetPositiveScale(Vector<double> referenceRow, Vector<double> candidateRow, double tol, out double scale)
    {
      if (!TryGetNonZeroScale(referenceRow, candidateRow, tol, out scale))
        return false;

      return scale > tol;
    }

    /// <summary>
    /// Determines whether two constraint rows are proportional by a non-zero scalar factor.
    /// </summary>
    /// <param name="referenceRow">The reference constraint row.</param>
    /// <param name="candidateRow">The candidate constraint row.</param>
    /// <param name="tol">The comparison tolerance.</param>
    /// <param name="scale">The scalar factor that maps the reference row to the candidate row.</param>
    /// <returns><see langword="true"/> if the rows are proportional; otherwise, <see langword="false"/>.</returns>
    private static bool TryGetNonZeroScale(Vector<double> referenceRow, Vector<double> candidateRow, double tol, out double scale)
    {
      scale = 0;

      if (referenceRow.Count != candidateRow.Count)
        return false;

      int pivotIndex = -1;
      for (int i = 0; i < referenceRow.Count; i++)
      {
        if (Math.Abs(referenceRow[i]) > tol)
        {
          pivotIndex = i;
          break;
        }
      }

      if (pivotIndex < 0)
      {
        for (int i = 0; i < candidateRow.Count; i++)
        {
          if (Math.Abs(candidateRow[i]) > tol)
            return false;
        }

        return false;
      }

      if (Math.Abs(candidateRow[pivotIndex]) <= tol)
        return false;

      scale = candidateRow[pivotIndex] / referenceRow[pivotIndex];
      for (int i = 0; i < referenceRow.Count; i++)
      {
        if (Math.Abs(candidateRow[i] - scale * referenceRow[i]) > tol * Math.Max(1.0, Math.Max(Math.Abs(candidateRow[i]), Math.Abs(scale * referenceRow[i]))))
          return false;
      }

      return Math.Abs(scale) > tol;
    }


    /// <summary>
    /// Extracts the active inequality rows and their right-hand side values.
    /// </summary>
    /// <param name="activeSet">The set of active inequality-constraint indices.</param>
    /// <returns>
    /// A tuple containing the active inequality matrix and right-hand side vector, or <see langword="null"/> values if no inequalities are active.
    /// </returns>
    private (Matrix<double>? C, Vector<double>? d) ExtractActiveRows(
        HashSet<int> activeSet)
    {
      return ExtractActiveRows(activeSet, _C, _d);
    }

    /// <summary>
    /// Extracts the active inequality rows and their right-hand side values from the specified source system.
    /// </summary>
    /// <param name="activeSet">The set of active inequality-constraint indices.</param>
    /// <param name="sourceC">The source inequality matrix.</param>
    /// <param name="sourceD">The source inequality right-hand side vector.</param>
    /// <returns>
    /// A tuple containing the active inequality matrix and right-hand side vector, or <see langword="null"/> values if no inequalities are active.
    /// </returns>
    private static (Matrix<double>? C, Vector<double>? d) ExtractActiveRows(
        HashSet<int> activeSet,
        Matrix<double>? sourceC,
        Vector<double>? sourceD)
    {
      if (sourceC == null || activeSet.Count == 0) return (null, null);

      var indices = activeSet.OrderBy(i => i).ToList();
      var C = Matrix<double>.Build.DenseOfRowVectors(
                  indices.Select(i => sourceC.Row(i)));
      var d = Vector<double>.Build.DenseOfEnumerable(
                  indices.Select(i => sourceD![i]));
      return (C, d);
    }

  }
}
