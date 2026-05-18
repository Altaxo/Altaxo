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
using System.Linq;
using System.Threading;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Collections;

namespace Altaxo.Calc.Optimization
{
  /// <summary>
  /// Levenberg-Marquardt minimizer with constraints, enforced via projection after each LM step.
  /// </summary>
  /// <remarks>
  /// <para>
  /// The projection approach keeps the algorithm working in the original
  /// (external) parameter space at all times — there is no internal/external
  /// parameter transformation. After every LM step the candidate point is
  /// projected back onto the feasible set. The active set returned by the
  /// projector directly identifies which parameters are constrained
  /// (and therefore temporarily fixed for the gradient/Hessian update).
  /// </para>
  /// <para>References:</para>
  /// <para>
  /// [1]. Madsen, K., H. B. Nielsen, and O. Tingleff,
  ///      "Methods for Non-Linear Least Squares Problems. Technical University of Denmark, 2004. Lecture notes." (2004),
  ///      Available online from: <see href="http://orbit.dtu.dk/files/2721358/imm3215.pdf"/>
  /// </para>
  /// <para>
  /// [2]. Gavin, Henri,
  ///      "The Levenberg-Marquardt method for nonlinear least squares curve-fitting problems."
  ///      Department of Civil and Environmental Engineering, Duke University (2017): 1-19,
  ///      Available online from: <see href="http://people.duke.edu/~hpgavin/ce281/lm.pdf"/>
  /// </para>
  /// </remarks>
  public class LevenbergMarquardtMinimizerWithConstraintsNonAllocating
  {
    #region Properties

    /// <summary>The default scale factor for the initial <c>mu</c>.</summary>
    public const double DefaultInitialMu = 1E-3;

    /// <summary>Gets or sets the scale factor for the initial <c>mu</c>.</summary>
    public double InitialMu { get; set; } = DefaultInitialMu;

    /// <summary>
    /// Gets or sets the number of iterations after which the parameter scale is updated
    /// (if no user-provided scale was set). Default is <c>1</c> (update every iteration).
    /// Set to <see cref="int.MaxValue"/> to disable automatic scale updates.
    /// </summary>
    public int ParameterScaleUpdatePeriod { get; set; } = 1;

    /// <inheritdoc cref="NonlinearMinimizerBaseNonAllocating.FunctionTolerance"/>
    public double FunctionTolerance { get; set; } = 0;

    /// <inheritdoc cref="NonlinearMinimizerBaseNonAllocating.StepTolerance"/>
    public double StepTolerance { get; set; } = 1E-16;

    /// <inheritdoc cref="NonlinearMinimizerBaseNonAllocating.GradientTolerance"/>
    public double GradientTolerance { get; set; } = 0;

    /// <inheritdoc cref="NonlinearMinimizerBaseNonAllocating.MaximumIterations"/>
    public int? MaximumIterations { get; set; }

    /// <inheritdoc cref="NonlinearMinimizerBaseNonAllocating.MinimalRSSImprovement"/>
    public double MinimalRSSImprovement { get; set; } = 1E-6;

    /// <summary>
    /// Gets the scale factors used for the parameters. Set automatically if not
    /// provided by the caller.
    /// </summary>
    public Vector<double>? Scales { get; private set; }

    /// <summary>
    /// Gets or sets the constraint projector. When <see langword="null"/>, the
    /// minimizer runs unconstrained (equivalent to the original behaviour without
    /// box constraints).
    /// </summary>
    public IConstraintsProjector? Projector { get; set; }

    #endregion

    // -----------------------------------------------------------------------
    // Public FindMinimum overloads (mirror the original API)
    // -----------------------------------------------------------------------

    /// <summary>
    /// Non-linear least squares fitting by the Levenberg-Marquardt algorithm,
    /// without constraints.
    /// </summary>
    public NonlinearMinimizationResult FindMinimum(
      IObjectiveModelNonAllocating objective,
      IReadOnlyList<double> initialGuess,
      CancellationToken cancellationToken,
      Action<int, double, IReadOnlyList<double>>? reportChi2Progress)
    {
      return FindMinimum(objective, initialGuess,
        scales: null, isFixed: null,
        cancellationToken, reportChi2Progress);
    }

    /// <summary>
    /// Non-linear least squares fitting by the Levenberg-Marquardt algorithm.
    /// </summary>
    /// <param name="objective">The objective function, including model and observations.</param>
    /// <param name="initialGuess">The initial guess values.</param>
    /// <param name="scales">
    ///   Optional scale factors for the parameters (same length as <paramref name="initialGuess"/>).
    ///   Pass <see langword="null"/> for automatic scaling.
    /// </param>
    /// <param name="isFixed">
    ///   Optional array indicating which parameters are permanently fixed.
    ///   Must have the same length as <paramref name="initialGuess"/>.
    /// </param>
    /// <param name="cancellationToken">Token to cancel the evaluation.</param>
    /// <param name="reportChi2Progress">
    ///   Callback reporting (iterationCount, chi², currentParameters). May be <see langword="null"/>.
    /// </param>
    public NonlinearMinimizationResult FindMinimum(
      IObjectiveModelNonAllocating objective,
      IReadOnlyList<double> initialGuess,
      IReadOnlyList<double>? scales,
      IReadOnlyList<bool>? isFixed,
      CancellationToken cancellationToken,
      Action<int, double, IReadOnlyList<double>>? reportChi2Progress)
    {
      return Minimum(
        objective, initialGuess, scales, isFixed,
        cancellationToken, reportChi2Progress,
        initialMu: InitialMu,
        gradientTolerance: GradientTolerance,
        stepTolerance: StepTolerance,
        functionTolerance: FunctionTolerance,
        minimalRSSImprovement: MinimalRSSImprovement,
        maximumIterations: MaximumIterations);
    }

    // -----------------------------------------------------------------------
    // Core algorithm
    // -----------------------------------------------------------------------

    /// <summary>
    /// Core Levenberg-Marquardt minimization with linear constraint projection.
    /// </summary>
    /// <param name="objective">The objective function.</param>
    /// <param name="initialGuess">Initial parameter values.</param>
    /// <param name="scales">
    ///   Parameter scales, or <see langword="null"/> for automatic scaling.
    /// </param>
    /// <param name="isFixedByUser">
    ///   Per-parameter fixed flags, or <see langword="null"/> if none are fixed.
    /// </param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="reportChi2Progress">Progress callback (may be <see langword="null"/>).</param>
    /// <param name="initialMu">Initial LM damping factor.</param>
    /// <param name="gradientTolerance">Stopping threshold: infinity norm of gradient.</param>
    /// <param name="stepTolerance">Stopping threshold: relative parameter change.</param>
    /// <param name="functionTolerance">Stopping threshold: RSS value.</param>
    /// <param name="minimalRSSImprovement">
    ///   Minimum relative RSS improvement over 8 iterations before stopping.
    /// </param>
    /// <param name="maximumIterations">
    ///   Maximum iterations. <see langword="null"/> = auto. <c>0</c> = evaluation only.
    /// </param>
    public NonlinearMinimizationResult Minimum(
      IObjectiveModelNonAllocating objective,
      IReadOnlyList<double> initialGuess,
      IReadOnlyList<double>? scales,
      IReadOnlyList<bool>? isFixedByUser,
      CancellationToken cancellationToken,
      Action<int, double, IReadOnlyList<double>>? reportChi2Progress,
      double initialMu,
      double gradientTolerance,
      double stepTolerance,
      double functionTolerance,
      double minimalRSSImprovement,
      int? maximumIterations)
    {
      // ── Argument checks ──────────────────────────────────────────────────
      if (objective is null)
        throw new ArgumentNullException(nameof(objective));
      if (initialGuess is null)
        throw new ArgumentNullException(nameof(initialGuess));

      int n = initialGuess.Count;

      // Validate initial guess is finite
      for (int i = 0; i < n; i++)
        if (double.IsNaN(initialGuess[i]))
          throw new ArgumentException($"Initial parameter[{i}] is NaN.");

      // Validate isFixedByUser length
      if (isFixedByUser is not null && isFixedByUser.Count != n)
        throw new ArgumentException(
          $"{nameof(isFixedByUser)} must have the same length as {nameof(initialGuess)}.",
          nameof(isFixedByUser));

      // Validate scales
      if (scales is not null)
      {
        if (scales.Count != n)
          throw new ArgumentException(
            $"{nameof(scales)} must have the same length as {nameof(initialGuess)}.",
            nameof(scales));
        if (scales.Any(s => double.IsInfinity(s) || double.IsNaN(s) || s == 0))
          throw new ArgumentException("All scale values must be finite and non-zero.");

        Scales = Vector<double>.Build.DenseOfEnumerable(scales.Select(Math.Abs));
      }
      else
      {
        Scales = null; // will be set automatically below
      }

      // ── Validate initial guess against constraints ────────────────────
      if (Projector is not null && !Projector.IsFeasible(
            Vector<double>.Build.DenseOfEnumerable(initialGuess)))
        throw new ArgumentException(
          "The initial guess violates the linear constraints.");

      // ── Count varying parameters ──────────────────────────────────────
      int numberOfVaryingParameters = n;
      if (isFixedByUser is not null)
      {
        numberOfVaryingParameters = n - isFixedByUser.Count(x => x);
        if (numberOfVaryingParameters == 0)
          maximumIterations = 0; // all fixed → evaluation only
      }

      // ── Pre-allocate working vectors ──────────────────────────────────
      var parameterValues = Vector<double>.Build.DenseOfEnumerable(initialGuess);
      var parameterStep = Vector<double>.Build.Dense(n);
      var scaledParameterStep = Vector<double>.Build.Dense(n);
      var newParameterValues = Vector<double>.Build.Dense(n);
      var projectedParameterValues = Vector<double>.Build.Dense(n);
      var isProjectedParameterConstrained = new bool[n];
      var diagonalOfHessian = Vector<double>.Build.Dense(n);
      var diagonalOfHessianPlusMu = Vector<double>.Build.Dense(n);
      var MuTimesPStepMinusGradient = Vector<double>.Build.Dense(n);
      var rssValueHistory = new RingBufferEnqueueableOnly<double>(8);

      // isFixedByUserOrBoundary: union of user-fixed and constraint-active flags
      var isFixedByUserOrBoundary = new bool[n];

      // Saved Hessian/gradient for rollback when a constraint becomes active
      Matrix<double>? savedHessian = null;
      Vector<double>? savedNegativeGradient = null;
      bool wasHessianAndNegativeGradientSaved = false;

      // ── Initial function evaluation ───────────────────────────────────
      objective.SetParameters(initialGuess, isFixedByUser);
      ExitCondition exitCondition = ExitCondition.None;

      objective.EvaluateAt(parameterValues);
      var RSS = objective.Value;
      reportChi2Progress?.Invoke(0, RSS, parameterValues);

      if (!maximumIterations.HasValue)
        maximumIterations = 200 * (n + 1);

      if (double.IsNaN(RSS))
      {
        exitCondition = ExitCondition.InvalidValues;
        return new NonlinearMinimizationResult(objective, -1, exitCondition);
      }

      if (maximumIterations == 0)
        exitCondition = ExitCondition.ManuallyStopped;

      if (RSS <= functionTolerance)
        exitCondition = ExitCondition.Converged;

      // ── Initial Jacobian / Hessian evaluation ─────────────────────────
      var (NegativeGradient, Hessian) = EvaluateJacobian(objective);
      wasHessianAndNegativeGradientSaved = false;

      bool useAutomaticParameterScale = Scales is null;
      int iterationOfLastAutomaticParameterScaleEvaluation = 0;

      if (useAutomaticParameterScale)
      {
        // Auto-scale: scale each parameter so that the Hessian diagonal ≈ 1
        Scales = Hessian.Diagonal().Map(
          x => x != 0 ? 1.0 / Math.Sqrt(Math.Abs(x)) : 1.0,
          Zeros.Include);

        // Repeat Jacobian evaluation now that scales are set
        (NegativeGradient, Hessian) = EvaluateJacobian(objective);
        wasHessianAndNegativeGradientSaved = false;
      }

      Hessian.Diagonal(diagonalOfHessian);

      if (NegativeGradient.InfinityNorm() <= gradientTolerance)
        exitCondition = ExitCondition.RelativeGradient;

      if (exitCondition != ExitCondition.None)
        return new NonlinearMinimizationResult(objective, -1, exitCondition);

      // ── Main LM loop ─────────────────────────────────────────────────
      double mu = initialMu * diagonalOfHessian.Max();
      double nu = 2.0;
      int iterations = 0;
      int numberOfSolves = 0;

      while (iterations < maximumIterations && exitCondition == ExitCondition.None)
      {
        iterations++;

        while (true) // inner loop: increase mu until a good step is found
        {
          cancellationToken.ThrowIfCancellationRequested();

          diagonalOfHessian.Add(mu, diagonalOfHessianPlusMu); // diag(H) + mu

          // ── Set up isFixedByUserOrBoundary ─────────────────────────
          // Start from user-fixed flags only; constraint-active flags are
          // added below after the projection.
          if (isFixedByUser is null)
            Array.Clear(isFixedByUserOrBoundary, 0, n);
          else
            VectorMath.Copy(isFixedByUser, isFixedByUserOrBoundary);

          bool wasHessianModified;
          int numberOfFreeParameters;

          // Solve (H + μI) Δp_scaled = -g  for the scaled step
          Hessian.SetDiagonal(diagonalOfHessianPlusMu);
          Hessian.Solve(NegativeGradient, scaledParameterStep);

          // Unscale to get the real parameter step
          scaledParameterStep.PointwiseMultiply(Scales, parameterStep);



          // ── Project the proposed new point onto the feasible set ────
          // p_candidate = parameterValues + parameterStep
          // p_new       = argmin ||x - p_candidate||²  s.t. constraints
          parameterValues.Add(parameterStep, newParameterValues); // p + Δp
          double projectionScaleFactor = 1.0; // analogous to clampScaleFactor

          if (Projector is not null)
          {
            var origNorm = parameterStep.L2Norm();

            Projector.Project(newParameterValues, projectedParameterValues, isProjectedParameterConstrained);

            // Update newParameterValues in-place with the projected point
            for (int i = 0; i < n; i++)
            {
              newParameterValues[i] = projectedParameterValues[i];
              isFixedByUserOrBoundary[i] = (isFixedByUser?[i] ?? false) || isProjectedParameterConstrained[i];
            }

            // Compute the effective step after projection
            newParameterValues.Subtract(parameterValues, parameterStep); // Δp_eff = p_new - p

            // Compute scale factor: how much of the original step was kept?
            // We use the L2 norm ratio (avoids division by zero when step is zero)
            if (origNorm > 0)
            {
              projectionScaleFactor = Math.Min(1.0, parameterStep.L2Norm() / origNorm);
            }
            parameterStep.PointwiseDivide(Scales, scaledParameterStep); // unscale the effective step for convergence checks
          }

          // ── Convergence test: relative parameter change ─────────────
          // max|Δp[i] / p[i]| < stepTolerance (see [2] §4.1.3, criterion 2)
          var maxHiByPi = parameterStep
            .Zip(parameterValues, (hi, pi) =>
              hi == 0 ? 0.0 : pi == 0 ? double.PositiveInfinity : Math.Abs(hi / pi))
            .Max();

          if (maxHiByPi < stepTolerance * projectionScaleFactor)
          {
            exitCondition = ExitCondition.RelativePoints;
            break;
          }

          // ── Evaluate objective at the projected new point ───────────
          objective.EvaluateAt(newParameterValues);
          var RSSnew = objective.Value;

          // ── Compute gain ratio ρ ────────────────────────────────────
          double rho;
          if (double.IsNaN(RSSnew))
          {
            // NaN result: step too wide or parameters out of domain
            rho = -1;
          }
          else if (projectionScaleFactor < 1E-4 && RSSnew <= RSS)
          {
            // Projection reduced the step to nearly zero but RSS improved:
            // accept to snap parameters onto the constraint boundary
            rho = 1;
          }
          else
          {
            // ρ = (RSS - RSSnew) / (Δp'(μΔp − g))  — see [2] eq. 15
            scaledParameterStep.Multiply(mu, MuTimesPStepMinusGradient);
            MuTimesPStepMinusGradient.Add(NegativeGradient, MuTimesPStepMinusGradient);
            var predictedReduction =
              scaledParameterStep.DotProduct(MuTimesPStepMinusGradient);
            rho = predictedReduction > 0 ? (RSS - RSSnew) / predictedReduction : 0;
          }

          if (rho > 0)
          {
            // ── Accept step ───────────────────────────────────────────
            newParameterValues.CopyTo(parameterValues);
            RSS = RSSnew;
            rssValueHistory.Enqueue(RSS);
            reportChi2Progress?.Invoke(iterations, RSS, parameterValues);

            // Update automatic parameter scale if due
            if (useAutomaticParameterScale &&
                iterations >= (ParameterScaleUpdatePeriod +
                               iterationOfLastAutomaticParameterScaleEvaluation))
            {
              Hessian.Diagonal(diagonalOfHessian);
              diagonalOfHessian.Map(
                x => x != 0 ? 1.0 / Math.Sqrt(Math.Abs(x)) : 1.0,
                Scales, Zeros.Include);
              iterationOfLastAutomaticParameterScaleEvaluation = iterations;
            }

            // Re-evaluate gradient and Hessian at new parameters
            (NegativeGradient, Hessian) = EvaluateJacobian(objective);
            Hessian.Diagonal(diagonalOfHessian);
            wasHessianAndNegativeGradientSaved = false;

            // Convergence checks
            if (NegativeGradient.InfinityNorm() <= gradientTolerance)
              exitCondition = ExitCondition.RelativeGradient;

            if (RSS <= functionTolerance)
              exitCondition = ExitCondition.Converged;

            if (rssValueHistory.Count == rssValueHistory.Capacity)
            {
              var improvement = (rssValueHistory.OldestValue - rssValueHistory.NewestValue)
                                / rssValueHistory.OldestValue;
              if (improvement < minimalRSSImprovement)
                exitCondition = ExitCondition.Converged;
            }

            // Decrease mu only for full (un-projected) steps
            if (projectionScaleFactor == 1.0)
            {
              mu *= Math.Max(1.0 / 3.0, 1.0 - Pow3(2.0 * rho - 1.0));
              nu = 2.0;
            }

            break;
          }
          else
          {
            // ── Reject step: increase damping ─────────────────────────
            mu *= nu;
            nu *= 2.0;

            if (wasHessianAndNegativeGradientSaved)
            {
              savedHessian!.CopyTo(Hessian);
              Hessian.Diagonal(diagonalOfHessian);
              savedNegativeGradient!.CopyTo(NegativeGradient);
            }

            if (!(mu < double.MaxValue && nu < double.MaxValue))
            {
              exitCondition = double.IsNaN(RSSnew)
                ? ExitCondition.InvalidValues
                : ExitCondition.RelativeGradient;
              break;
            }
          }
        } // end inner while
      } // end outer while

      if (iterations >= maximumIterations)
        exitCondition = ExitCondition.ExceedIterations;

      objective.IsFixedByUserOrBoundary = isFixedByUserOrBoundary;
      return new NonlinearMinimizationResult(objective, iterations, exitCondition, isFixedByUserOrBoundary);
    }

    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------

    /// <summary>Returns x³.</summary>
    private static double Pow3(double x) => x * x * x;

    /// <summary>
    /// Evaluates and scales the Jacobian-derived gradient and Hessian.
    /// </summary>
    private (Vector<double> NegativeGradient, Matrix<double> Hessian)
      EvaluateJacobian(IObjectiveModelNonAllocating objective)
    {
      var negativeGradient = objective.NegativeGradient;
      var hessian = objective.Hessian;

      if (Scales is not null)
      {
        for (int i = 0; i < negativeGradient.Count; i++)
        {
          negativeGradient[i] *= Scales[i];
          for (int j = 0; j < hessian.ColumnCount; j++)
          {
            // Parenthesised to avoid overflow/underflow when Scales[i]*Scales[j] is extreme
            hessian[i, j] = (hessian[i, j] * Scales[i]) * Scales[j];
          }
        }
      }

      return (negativeGradient, hessian);
    }


  }
}
