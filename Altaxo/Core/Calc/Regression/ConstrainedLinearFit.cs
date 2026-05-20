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
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Optimization;

namespace Altaxo.Calc.Regression
{
  /// <summary>
  /// Constrained linear least-squares fit via SVD, using an
  /// <see cref="IConstraintsProjector"/> to enforce equality and
  /// inequality constraints on the parameter vector.
  ///
  /// The method iterates:
  ///   1. Project current beta onto the feasible set → learn which
  ///      parameters are fixed/constrained at this point.
  ///   2. Solve the reduced unconstrained LS (SVD) for the remaining
  ///      free parameters only.
  ///   3. Project the new beta back; repeat until convergence.
  ///
  /// For pure equality constraints this converges in one outer iteration.
  /// Inequality constraints may need a few iterations (active-set style).
  /// </summary>
  /// <remarks>
  /// <para>Usage example:</para>
  /// <code><![CDATA[
  /// // Equality: beta[0] + beta[1] = 1.0
  /// var A_eq = Matrix<double>.Build.DenseOfArray(new double[,] { { 1, 1, 0 } });
  /// var b_eq = Vector<double>.Build.Dense(new double[] { 1.0 });
  /// 
  /// // Inequality: beta[2] >= 0  →  -beta[2] <= 0
  /// var C_ineq = Matrix<double>.Build.DenseOfArray(new double[,] { { 0, 0, -1 } });
  /// var d_ineq = Vector<double>.Build.Dense(new double[] { 0.0 });
  /// 
  /// var projector = new LinearConstraintsProjector(A_eq, b_eq, C_ineq, d_ineq);
  /// 
  /// // Design matrix and observations
  /// var A = Matrix<double>.Build.DenseOfArray(new double[,] {
  ///   { 1, 0, 1 },
  ///   { 0, 1, 1 },
  ///   { 1, 1, 0 },
  ///   { 1, 0, 0 },
  /// });
  /// var y = Vector<double>.Build.Dense(new double[] { 1.5, 0.8, 1.0, 0.3 });
  /// 
  /// var beta = ConstrainedLinearFit.Fit(A, y, projector);
  /// // beta[0] + beta[1] == 1.0  ✓
  /// // beta[2] >= 0              ✓
  /// ]]></code>
  /// </remarks>
  public static class ConstrainedLinearFit
  {
    /// <summary>
    /// Fits beta such that A*beta ≈ y subject to the constraints encoded
    /// in <paramref name="projector"/>.
    /// </summary>
    /// <param name="A">Design matrix (n rows × p columns).</param>
    /// <param name="y">Observation vector (n).</param>
    /// <param name="projector">Constraint projector (p parameters).</param>
    /// <param name="svdTolerance">
    /// Relative singular-value cutoff for the SVD pseudo-inverse.
    /// Singular values smaller than svdTolerance * sigma_max are ignored.
    /// </param>
    /// <param name="maxOuterIterations">
    /// Maximum number of project → solve → project cycles.
    /// </param>
    /// <param name="convergenceTol">
    /// Stop when ||beta_new - beta_old|| &lt; convergenceTol.
    /// </param>
    /// <returns>Constrained least-squares solution beta (p).</returns>
    public static Vector<double> Fit(
        Matrix<double> A,
        Vector<double> y,
        IConstraintsProjector projector,
        double svdTolerance = 1e-10,
        int maxOuterIterations = 50,
        double convergenceTol = 1e-12)
    {
      if (projector is LinearConstraintsProjector linearProjector)
        return Fit(A, y, linearProjector, svdTolerance, maxOuterIterations, convergenceTol);

      return FitWithGenericProjector(A, y, projector, maxOuterIterations, convergenceTol);
    }

    private static Vector<double> Fit(
        Matrix<double> A,
        Vector<double> y,
        LinearConstraintsProjector projector,
        double svdTolerance,
        int maxOuterIterations,
        double convergenceTol)
    {
      int p = A.ColumnCount;

      // ----------------------------------------------------------------
      // Step 0: feasible starting point — project the zero vector so
      //         that fixed parameters get their constraint values and
      //         inequality constraints are satisfied.
      // ----------------------------------------------------------------
      var beta = Project(projector, Vector<double>.Build.Dense(p));

      for (int iter = 0; iter < maxOuterIterations; iter++)
      {
        // ------------------------------------------------------------
        // Step 1: project current beta and learn the active structure.
        // ------------------------------------------------------------
        beta = Project(projector, beta, out var isConstrained); // make sure beta is feasible

        // Indices of parameters that are completely free to vary here.
        var freeIndices = new int[p];
        int nFree = 0;
        for (int i = 0; i < p; i++)
        {
          if (!isConstrained[i])
            freeIndices[nFree++] = i;
        }

        // ------------------------------------------------------------
        // Step 2: build the reduced design matrix A_free that acts
        //         only on the free columns, after subtracting the
        //         contribution of the fixed/constrained columns.
        // ------------------------------------------------------------
        // Residual after accounting for all non-free parameters:
        //   r = y - A_fixed * beta_fixed
        var r = y.Clone();
        for (int j = 0; j < p; j++)
        {
          // Subtract every non-free column's contribution
          if (isConstrained[j])
            r -= A.Column(j) * beta[j];
        }

        if (nFree == 0)
          break;  // all parameters are constrained — nothing to optimise

        // Reduced design matrix: columns corresponding to free indices
        var Afree = Matrix<double>.Build.Dense(A.RowCount, nFree);
        for (int k = 0; k < nFree; k++)
          Afree.SetColumn(k, A.Column(freeIndices[k]));

        // ------------------------------------------------------------
        // Step 3: solve the reduced unconstrained LS via SVD.
        // ------------------------------------------------------------
        var betaFree = SolveLeastSquaresSVD(Afree, r, svdTolerance);

        // ------------------------------------------------------------
        // Step 4: write free-parameter solution back into beta.
        // ------------------------------------------------------------
        var betaNew = beta.Clone();
        for (int k = 0; k < nFree; k++)
          betaNew[freeIndices[k]] = betaFree[k];

        // ------------------------------------------------------------
        // Step 5: project the updated beta to restore feasibility
        //         (the SVD step may have slightly violated inequality
        //         constraints that weren't active in Step 1).
        // ------------------------------------------------------------
        betaNew = Project(projector, betaNew);

        // ------------------------------------------------------------
        // Step 6: check convergence.
        // ------------------------------------------------------------
        double delta = (betaNew - beta).L2Norm();
        beta = betaNew;

        if (delta < convergenceTol)
          break;
      }

      return beta;
    }

    private static Vector<double> FitWithGenericProjector(
        Matrix<double> A,
        Vector<double> y,
        IConstraintsProjector projector,
        int maxOuterIterations,
        double convergenceTol)
    {
      var beta = Project(projector, Vector<double>.Build.Dense(A.ColumnCount));
      var residual = A * beta - y;
      double objective = residual.DotProduct(residual);

      for (int iter = 0; iter < maxOuterIterations; iter++)
      {
        var gradient = A.TransposeThisAndMultiply(residual);
        double gradientNorm = gradient.L2Norm();
        if (gradientNorm < convergenceTol)
          break;

        double stepSize = 1.0;
        Vector<double>? betaNew = null;
        Vector<double>? residualNew = null;
        double objectiveNew = objective;

        while (stepSize > 1e-12)
        {
          betaNew = Project(projector, beta - stepSize * gradient);
          residualNew = A * betaNew - y;
          objectiveNew = residualNew.DotProduct(residualNew);

          if (objectiveNew <= objective)
            break;

          stepSize *= 0.5;
        }

        if (betaNew is null || residualNew is null)
          break;

        double delta = (betaNew - beta).L2Norm();
        beta = betaNew;
        residual = residualNew;
        objective = objectiveNew;

        if (delta < convergenceTol)
          break;
      }

      return beta;
    }

    // --------------------------------------------------------------------
    // SVD-based unconstrained least-squares solver
    //   min ||M*x - b||²
    // --------------------------------------------------------------------
    private static Vector<double> SolveLeastSquaresSVD(
        Matrix<double> M,
        Vector<double> b,
        double relativeTolerance)
    {
      var svd = M.Svd(computeVectors: true);

      var U = svd.U;
      var S = svd.S;
      var VT = svd.VT;

      int singularValueCount = S.Count;

      // U^T * b  (only the first singularValueCount rows are non-zero in thin SVD)
      var Utb = U.TransposeThisAndMultiply(b);

      double sMax = singularValueCount > 0 ? S[0] : 0.0;
      var SinvUtb = Vector<double>.Build.Dense(VT.RowCount);

      for (int i = 0; i < singularValueCount; i++)
      {
        if (S[i] > relativeTolerance * sMax)
          SinvUtb[i] = Utb[i] / S[i];
        // else: truncated — effectively regularised to zero
      }

      // x = V * SinvUtb
      return VT.TransposeThisAndMultiply(SinvUtb);
    }

    private static Vector<double> Project(IConstraintsProjector projector, Vector<double> inputValues)
    {
      var projectedValues = Vector<double>.Build.Dense(inputValues.Count);
      projector.Project(inputValues, projectedValues, new bool[inputValues.Count]);
      return projectedValues;
    }

    private static Vector<double> Project(IConstraintsProjector projector, Vector<double> inputValues, out bool[] isConstrained)
    {
      var projectedValues = Vector<double>.Build.Dense(inputValues.Count);
      isConstrained = new bool[inputValues.Count];
      projector.Project(inputValues, projectedValues, isConstrained);
      return projectedValues;
    }
  }
}
