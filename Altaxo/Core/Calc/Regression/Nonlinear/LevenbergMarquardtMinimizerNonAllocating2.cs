using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Collections;

namespace Altaxo.Calc.Optimization
{
  /// <summary>
  /// LevenbergMarquardtMinimizer, that doesnt allocate memory during the iterations.
  /// </summary>
  /// <seealso cref="Altaxo.Calc.Optimization.NonlinearMinimizerBaseNonAllocating" />
  /// <remarks>
  /// <para>
  /// References:
  /// </para>
  /// <para>
  /// [1]. Madsen, K., H. B. Nielsen, and O. Tingleff,
  ///      "Methods for Non-Linear Least Squares Problems. Technical University of Denmark, 2004. Lecture notes." (2004),
  ///      Available Online from: <see href="http://orbit.dtu.dk/files/2721358/imm3215.pdf"/> 
  /// </para>
  /// <para>
  /// [2]. Gavin, Henri,
  ///      "The Levenberg-Marquardt method for nonlinear least squares curve-fitting problems."
  ///      Department of Civil and Environmental Engineering, Duke University (2017): 1-19,
  ///      Availble Online from: <see href="http://people.duke.edu/~hpgavin/ce281/lm.pdf"/> 
  /// </para>
  ///</remarks>
  public class LevenbergMarquardtMinimizerNonAllocating2 : NonlinearMinimizerBaseNonAllocating
  {
    /// <summary>
    /// The scale factor for initial mu
    /// </summary>
    public double InitialMu { get; set; }

    private RingBufferEnqueueableOnly<double> _valueHistory = new(8);

    public LevenbergMarquardtMinimizerNonAllocating2(double initialMu = 1E-3, double gradientTolerance = 1E-15, double stepTolerance = 1E-15, double functionTolerance = 1E-15, double minimalRSSImprovement = 1E-14, int maximumIterations = -1)
        : base(gradientTolerance, stepTolerance, functionTolerance, minimalRSSImprovement, maximumIterations)
    {
      InitialMu = initialMu;
    }

    public NonlinearMinimizationResult FindMinimum(IObjectiveModelNonAllocating objective, IReadOnlyList<double> initialGuess,
        IReadOnlyList<double?> lowerBound = null, IReadOnlyList<double?> upperBound = null, IReadOnlyList<double> scales = null, IReadOnlyList<bool> isFixed = null, CancellationToken cancellationToken = default)
    {
      return Minimum(objective, initialGuess, lowerBound, upperBound, scales, isFixed, cancellationToken, InitialMu, GradientTolerance, StepTolerance,
        functionTolerance: FunctionTolerance,
        minimalRSSImprovement: MinimalRSSImprovement,
        maximumIterations: MaximumIterations);
    }

    public NonlinearMinimizationResult FindMinimum(IObjectiveModelNonAllocating objective, double[] initialGuess,
        double?[] lowerBound = null, double?[] upperBound = null, double[] scales = null, bool[] isFixed = null, CancellationToken cancellationToken = default)
    {
      if (objective is null)
        throw new ArgumentNullException(nameof(objective));
      if (initialGuess is null)
        throw new ArgumentNullException(nameof(initialGuess));

      return Minimum(objective, CreateVector.DenseOfArray(initialGuess), lowerBound, upperBound, scales, isFixed, cancellationToken, InitialMu, GradientTolerance, StepTolerance, FunctionTolerance, MaximumIterations);
    }

    /// <summary>
    /// Non-linear least square fitting by the Levenberg-Marduardt algorithm.
    /// </summary>
    /// <param name="objective">The objective function, including model, observations, and parameter bounds.</param>
    /// <param name="initialGuess">The initial guess values.</param>
    /// <param name="initialMu">The initial damping parameter of mu.</param>
    /// <param name="gradientTolerance">The stopping threshold for infinity norm of the gradient vector.</param>
    /// <param name="stepTolerance">The stopping threshold for L2 norm of the change of parameters.</param>
    /// <param name="functionTolerance">The stopping threshold for L2 norm of the residuals.</param>
    /// <param name="maximumIterations">The max iterations.</param>
    /// <returns>The result of the Levenberg-Marquardt minimization</returns>
    public NonlinearMinimizationResult Minimum(IObjectiveModelNonAllocating objective, IReadOnlyList<double> initialGuess,
        IReadOnlyList<double?> lowerBound = null, IReadOnlyList<double?> upperBound = null, IReadOnlyList<double> scales = null, IReadOnlyList<bool> isFixed = null, CancellationToken cancellationToken = default,
        double initialMu = 1E-3, double gradientTolerance = 1E-15, double stepTolerance = 1E-15, double functionTolerance = 1E-15, double minimalRSSImprovement = 1E-15, int maximumIterations = -1)
    {
      // Non-linear least square fitting by the Levenberg-Marduardt algorithm.
      //
      // Levenberg-Marquardt is finding the minimum of a function F(p) that is a sum of squares of nonlinear functions.
      //
      // For given datum pair (x, y), uncertainties σ (or weighting W  =  1 / σ^2) and model function f = f(x; p),
      // let's find the parameters of the model so that the sum of the quares of the deviations is minimized.
      //
      //    F(p) = 1/2 * ∑{ Wi * (yi - f(xi; p))^2 }
      //    pbest = argmin F(p)
      //
      // We will use the following terms:
      //    Weighting W is the diagonal matrix and can be decomposed as LL', so L = 1/σ
      //    Residuals, R = L(y - f(x; p))
      //    Residual sum of squares, RSS = ||R||^2 = R.DotProduct(R)
      //    Jacobian J = df(x; p)/dp
      //    Gradient g = -J'W(y − f(x; p)) = -J'LR
      //    Approximated Hessian H = J'WJ
      //
      // The Levenberg-Marquardt algorithm is summarized as follows:
      //    initially let μ = τ * max(diag(H)).
      //    repeat
      //       solve linear equations: (H + μI)ΔP = -g
      //       let ρ = (||R||^2 - ||Rnew||^2) / (Δp'(μΔp - g)).
      //       if ρ > ε, P = P + ΔP; μ = μ * max(1/3, 1 - (2ρ - 1)^3); ν = 2;
      //       otherwise μ = μ*ν; ν = 2*ν;
      //
      // References:
      // [1]. Madsen, K., H. B. Nielsen, and O. Tingleff.
      //    "Methods for Non-Linear Least Squares Problems. Technical University of Denmark, 2004. Lecture notes." (2004).
      //    Available Online from: http://orbit.dtu.dk/files/2721358/imm3215.pdf
      // [2]. Gavin, Henri.
      //    "The Levenberg-Marquardt method for nonlinear least squares curve-fitting problems."
      //    Department of Civil and Environmental Engineering, Duke University (2017): 1-19.
      //    Availble Online from: http://people.duke.edu/~hpgavin/ce281/lm.pdf

      if (objective is null)
        throw new ArgumentNullException(nameof(objective));

      ValidateBounds(initialGuess, lowerBound, upperBound, scales);

      _scaleFactors = Vector<double>.Build.Dense(initialGuess.Count);
      var pExt = Vector<double>.Build.DenseOfEnumerable(initialGuess);
      var Pstep = Vector<double>.Build.Dense(initialGuess.Count);
      var Pnew = Vector<double>.Build.Dense(initialGuess.Count);

      var diagonalOfHessian = Vector<double>.Build.Dense(initialGuess.Count);
      var diagonalOfHessianPlusMu = Vector<double>.Build.Dense(initialGuess.Count);

      objective.SetParameters(initialGuess, isFixed);
      ExitCondition exitCondition = ExitCondition.None;

      // First, calculate function values and setup variables
      objective.EvaluateAt(pExt);
      var RSS = objective.Value;  // Residual Sum of Squares = R'R

      if (maximumIterations < 0)
      {
        maximumIterations = 200 * (initialGuess.Count + 1);
      }

      // if RSS == NaN, stop
      if (double.IsNaN(RSS))
      {
        exitCondition = ExitCondition.InvalidValues;
        return new NonlinearMinimizationResult(objective, -1, exitCondition);
      }

      // When only function evaluation is needed, set maximumIterations to zero,
      if (maximumIterations == 0)
      {
        exitCondition = ExitCondition.ManuallyStopped;
      }

      // if RSS <= fTol, stop
      if (RSS <= functionTolerance)
      {
        exitCondition = ExitCondition.Converged; // SmallRSS
      }

      // Evaluate gradient (already negated!) and Hessian
      var (NegativeGradient, Hessian) = EvaluateJacobian(objective, pExt);
      Hessian.Diagonal(diagonalOfHessian); // save the diagonal of the Hession diag(H) into the vector diagonalOfHessian

      // if ||g||oo <= gtol, found and stop
      if (NegativeGradient.InfinityNorm() <= gradientTolerance)
      {
        exitCondition = ExitCondition.RelativeGradient;
      }

      if (exitCondition != ExitCondition.None)
      {
        return new NonlinearMinimizationResult(objective, -1, exitCondition);
      }

      double mu = initialMu * diagonalOfHessian.Max(); // μ
      double nu = 2; //  ν
      int iterations = 0;
      int numberOfSolves = 0;
      var MuTimesPStepMinusGradient = Vector<double>.Build.Dense(initialGuess.Count);
      var isTemporarilyFixed = new bool[initialGuess.Count];
      while (iterations < maximumIterations && exitCondition == ExitCondition.None)
      {
        iterations++;

        while (true)
        {
          cancellationToken.ThrowIfCancellationRequested();
          diagonalOfHessian.Add(mu, diagonalOfHessianPlusMu);

          bool wasHessianModified = false;

          VectorMath.Copy(isFixed, isTemporarilyFixed); // Start with a Hessian in which only the fixed parameters are considered

          do
          {
            Hessian.SetDiagonal(diagonalOfHessianPlusMu); // hessian[i, i] = hessian[i, i] + mu; see [2] eq. (12), page 3

            // solve normal equations
            Hessian.Solve(NegativeGradient, Pstep);
            ++numberOfSolves;

            // if the step would violate the boundary conditions, we modify the Hessian and the gradient accordingly
            wasHessianModified = ModifyHessianAndGradient(pExt, Pstep, mu, Hessian, NegativeGradient, diagonalOfHessianPlusMu, isTemporarilyFixed);

          } while (wasHessianModified);




          //ThrowIfGradientAndStepHaveDifferentSign(NegativeGradient, Pstep);

          ClampStepToBoundaryConditions(pExt, Pstep, Pstep, Pnew); // Pnew: new parameters to test

          // Test if there is convergence in the parameters
          // if max|hi/pi| < xTol, stop (see [2], Section 4.1.3 (page 5), second criterion
          var maxHiByPi = Pstep.Zip(pExt, (hi, pi) => hi == 0 ? 0 : pi == 0 ? double.PositiveInfinity : Math.Abs(hi / pi)).Max();
          if (maxHiByPi < stepTolerance)
          {
            exitCondition = ExitCondition.RelativePoints;
            break;
          }

          objective.EvaluateAt(Pnew);
          var RSSnew = objective.Value; // evaluate function at Pnew

          if (double.IsNaN(RSSnew))
          {
            exitCondition = ExitCondition.InvalidValues;
            break;
          }

          // calculate the ratio of the actual to the predicted reduction, see [2], eq. 15, page 3 
          // ρ = (RSS - RSSnew) / (Δp'(μΔp - g))
          Pstep.Multiply(mu, MuTimesPStepMinusGradient); // calculate μΔp
          MuTimesPStepMinusGradient.Add(NegativeGradient, MuTimesPStepMinusGradient); // calculate (μΔp - g)
          var predictedReduction = Pstep.DotProduct(MuTimesPStepMinusGradient); // calculate (Δp'(μΔp - g))
          var rho = (predictedReduction != 0)
                  ? (RSS - RSSnew) / predictedReduction
                  : 0;

          if (rho > 0.0 && predictedReduction >= 0)
          {
            // accepted
            Pnew.CopyTo(pExt);
            RSS = RSSnew;
            _valueHistory.Enqueue(RSS);

            // update gradient and Hessian 
            (NegativeGradient, Hessian) = EvaluateJacobian(objective, pExt);
            Hessian.Diagonal(diagonalOfHessian);

            // Test if convergence of gradient is achieved, see [2], section 4.1.3 (page 5), first criterion
            // if ||g||_oo <= gtol, found and stop
            if (NegativeGradient.InfinityNorm() <= gradientTolerance)
            {
              exitCondition = ExitCondition.RelativeGradient;
            }

            // Test if convergence of χ² is achieved, see [2], section 4.1.3 (page 5), 3rd criterion
            // if ||R||^2 < fTol, found and stop
            if (RSS <= functionTolerance)
            {
              exitCondition = ExitCondition.Converged; // SmallRSS
            }

            // Test if improvement in RSS is so low, that exit condition is reached
            if (_valueHistory.Count == _valueHistory.Capacity)
            {
              var RSSImprovement = (_valueHistory.OldestValue - _valueHistory.NewestValue) / _valueHistory.OldestValue;
              if (RSSImprovement < minimalRSSImprovement)
              {
                exitCondition = ExitCondition.Converged; // low RSS improvement
              }
            }

            mu *= Math.Max(1.0 / 3.0, 1.0 - Pow3(2.0 * rho - 1.0)); // see [2], section 4.1.1, point 3
            nu = 2;

            break;
          }
          else
          {
            // rejected, increased μ
            mu *= nu;
            nu *= 2;
          }
        }
      }

      // Test if the maximum number of iterations is reached, see [2], section 4.1.3 (page 5), last paragraph
      if (iterations >= maximumIterations)
      {
        exitCondition = ExitCondition.ExceedIterations;
      }

      return new NonlinearMinimizationResult(objective, iterations, exitCondition);
    }



    private static double Pow3(double x) => x * x * x;

    private bool ThrowIfGradientAndStepHaveDifferentSign(IReadOnlyList<double> gradient, IReadOnlyList<double> step)
    {
      for (int i = 0; i < gradient.Count; ++i)
      {
        if (Math.Sign(gradient[i]) != Math.Sign(step[i]))
        {
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Makes a proposed step smaller (if neccessary), so that the resulting parameters don't violate the parameter boundaries.
    /// </summary>
    /// <param name="pExt">The given parameters.</param>
    /// <param name="pStep">The proposed step.</param>
    /// <param name="clampedStep">On return, contains the clamped step.</param>
    /// <param name="nextPExt">On return, contains the new parameters, i.e. <paramref name="pExt"/>+<paramref name="clampedStep"/>.</param>
    private bool ClampStepToBoundaryConditions(IReadOnlyList<double> pExt, IReadOnlyList<double> pStep, IVector<double> clampedStep, IVector<double> nextPExt)
    {
      double scaleFactor = 1; // Scale factor for step
      int idxLowestScale = -1;
      double valueParameterAtLowestScale = double.NaN;

      if (LowerBound is not null || UpperBound is not null)
      {
        for (int i = 0; i < pExt.Count; i++)
        {
          var lowerBnd = LowerBound?.ElementAt(i);
          var upperBnd = UpperBound?.ElementAt(i);

          if (lowerBnd.HasValue && pExt[i] > lowerBnd.Value)
          {
            if (pStep[i] < 0 && !(pExt[i] + scaleFactor * pStep[i] > lowerBnd.Value))
            {
              idxLowestScale = i;
              scaleFactor = (lowerBnd.Value - pExt[i]) / pStep[i];
              valueParameterAtLowestScale = lowerBnd.Value;
            }
          }
          if (upperBnd.HasValue && pExt[i] < upperBnd.Value)
          {
            if (pStep[i] > 0 && !(pExt[i] + scaleFactor * pStep[i] < upperBnd.Value))
            {
              idxLowestScale = i;
              scaleFactor = (upperBnd.Value - pExt[i]) / pStep[i];
              valueParameterAtLowestScale = upperBnd.Value;
            }
          }
        }
      }

      // calculate the clamped step, and the new parameters
      for (int i = 0; i < pExt.Count; i++)
      {
        clampedStep[i] = scaleFactor * pStep[i];
        nextPExt[i] = pExt[i] + clampedStep[i];
      }

      // in order to avoid small inaccuracies caused by scaleFactor, we set the nextParameter that caused the scaleFactor to its bound
      if (idxLowestScale >= 0)
      {
        nextPExt[idxLowestScale] = valueParameterAtLowestScale;
      }

      return idxLowestScale >= 0;
    }



    /// <summary>
    /// Evaluates the jacobian, and the hessian of the objective function.
    /// </summary>
    /// <param name="objective">The objective.</param>
    /// <param name="pInt">The parameters (internal representation).</param>
    /// <returns>The negative gradient and the hessian.</returns>
    protected new (Vector<double> NegativeGradient, Matrix<double> Hessian) EvaluateJacobian(IObjectiveModelNonAllocating objective, IReadOnlyList<double> pInt)
    {
      var negativeGradient = objective.NegativeGradient;
      var hessian = objective.Hessian;

      if (IsBounded)
      {
        ScaleFactorsOfJacobian(pInt, negativeGradient, _scaleFactors); // the parameters argument is always internal.

        for (int i = 0; i < negativeGradient.Count; i++)
        {
          negativeGradient[i] *= _scaleFactors[i];
        }

        for (int i = 0; i < hessian.RowCount; i++)
        {
          for (int j = 0; j < hessian.ColumnCount; j++)
          {
            hessian[i, j] *= _scaleFactors[i] * _scaleFactors[j];
          }
        }
      }

      return (negativeGradient, hessian);
    }

    /// <summary>
    /// Calculates the scale factor of the jacobian, taking into account the parameter transformations , and the parameter scales.
    /// </summary>
    /// <param name="Pint">The pint.</param>
    /// <param name="result">On return, contains the scale factors. The provided vector needs to have the same length as <paramref name="Pint"/></param>
    protected void ScaleFactorsOfJacobian(IReadOnlyList<double> Pint, IReadOnlyList<double> gradient, IVector<double> result)
    {
      if (false)
      {
        for (int i = 0; i < Pint.Count; i++)
        {
          var lowerBnd = LowerBound?.ElementAt(i);
          var upperBnd = UpperBound?.ElementAt(i);

          if (lowerBnd.HasValue && upperBnd.HasValue)
          {
            if (Pint[i] > lowerBnd.Value && Pint[i] < upperBnd.Value)
              result[i] = (Scales?.ElementAt(i) ?? 1);
            else if (!(Pint[i] > lowerBnd.Value))
              result[i] = gradient[i] > 0 ? (Scales?.ElementAt(i) ?? 1) : 0;
            else if (!(Pint[i] < upperBnd.Value))
              result[i] = gradient[i] < 0 ? (Scales?.ElementAt(i) ?? 1) : 0;
            else
              result[i] = 0;
          }
          else if (upperBnd.HasValue)
          {
            result[i] = Pint[i] < upperBnd.Value || gradient[i] < 0 ? (Scales?.ElementAt(i) ?? 1) : 0;
          }
          else if (lowerBnd.HasValue)
          {
            result[i] = Pint[i] > lowerBnd.Value || gradient[i] > 0 ? (Scales?.ElementAt(i) ?? 1) : 0;
          }
          else
          {
            result[i] = 1;
          }

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

    /// <summary>
    /// Modifies the hessian and gradient according to the boundary conditions of the parameters.
    /// </summary>
    /// <param name="Pint">The parameters.</param>
    /// <param name="pstep">The parameter step planned.</param>
    /// <param name="mu">The value of mu.</param>
    /// <param name="hessian">The Hessian matrix. If the return value is true, this matrix was modified during the call.</param>
    /// <param name="gradient">The negative gradient.  If the return value is true, this vector was modified during the call.</param>
    /// <param name="diagonalOfHessianPlusMu">The diagonal of the Hessian matrix plus mu.  If the return value is true, this vector was modified during the call.</param>
    /// <param name="isTemporaryFixed">The array of fixed parameters (parameters fixed from the beginning plus parameters that have reached the boundary). If the return value is true, this vector was modified during the call.</param>
    /// <returns>True if the Hessian and gradient were modified; otherwise, false.</returns>
    private bool ModifyHessianAndGradient(IReadOnlyList<double> Pint, IReadOnlyList<double> pstep, double mu, Matrix<double> hessian, Vector<double> gradient, Vector<double> diagonalOfHessianPlusMu, bool[] isTemporaryFixed)
    {
      bool wasModified = false;
      int numberOfFixedParameters = 0;
      if (LowerBound is not null || UpperBound is not null)
      {
        for (int i = 0; i < Pint.Count; i++)
        {
          if (isTemporaryFixed[i])
          {
            ++numberOfFixedParameters;
            continue;
          }

          var lowerBnd = LowerBound?.ElementAt(i);
          var upperBnd = UpperBound?.ElementAt(i);

          bool isAtBound;

          if (lowerBnd.HasValue && upperBnd.HasValue)
          {
            if (Pint[i] > lowerBnd.Value && Pint[i] < upperBnd.Value)
              isAtBound = false;
            else if (!(Pint[i] > lowerBnd.Value))
              isAtBound = pstep[i] < 0;
            else if (!(Pint[i] < upperBnd.Value))
              isAtBound = pstep[i] > 0;
            else
              isAtBound = true;
          }
          else if (upperBnd.HasValue)
          {
            isAtBound = !(Pint[i] < upperBnd.Value) && pstep[i] > 0;
          }
          else if (lowerBnd.HasValue)
          {
            isAtBound = !(Pint[i] > lowerBnd.Value) && pstep[i] < 0;
          }
          else
          {
            isAtBound = false;
          }


          if (isAtBound)
          {
            gradient[i] = 0;
            hessian.ClearColumn(i);
            hessian.ClearRow(i);
            diagonalOfHessianPlusMu[i] = mu;
            isTemporaryFixed[i] = true;
            wasModified = true;
            ++numberOfFixedParameters;
          }
        }
      }

      return wasModified;
    }

  }
}
