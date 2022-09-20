using System;
using System.Collections.Generic;
using System.Linq;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Optimization
{
  public class LevenbergMarquardtMinimizerNonAllocating : NonlinearMinimizerBaseNonAllocating
  {
    /// <summary>
    /// The scale factor for initial mu
    /// </summary>
    public double InitialMu { get; set; }

    public LevenbergMarquardtMinimizerNonAllocating(double initialMu = 1E-3, double gradientTolerance = 1E-15, double stepTolerance = 1E-15, double functionTolerance = 1E-15, int maximumIterations = -1)
        : base(gradientTolerance, stepTolerance, functionTolerance, maximumIterations)
    {
      InitialMu = initialMu;
    }

    public NonlinearMinimizationResult FindMinimum(IObjectiveModelNonAllocating objective, IReadOnlyList<double> initialGuess,
        IReadOnlyList<double?> lowerBound = null, IReadOnlyList<double?> upperBound = null, IReadOnlyList<double> scales = null, IReadOnlyList<bool> isFixed = null)
    {
      return Minimum(objective, initialGuess, lowerBound, upperBound, scales, isFixed, InitialMu, GradientTolerance, StepTolerance, FunctionTolerance, MaximumIterations);
    }

    public NonlinearMinimizationResult FindMinimum(IObjectiveModelNonAllocating objective, double[] initialGuess,
        double?[] lowerBound = null, double?[] upperBound = null, double[] scales = null, bool[] isFixed = null)
    {
      if (objective is null)
        throw new ArgumentNullException(nameof(objective));
      if (initialGuess is null)
        throw new ArgumentNullException(nameof(initialGuess));

      return Minimum(objective, CreateVector.DenseOfArray(initialGuess), lowerBound, upperBound, scales, isFixed, InitialMu, GradientTolerance, StepTolerance, FunctionTolerance, MaximumIterations);
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
        IReadOnlyList<double?> lowerBound = null, IReadOnlyList<double?> upperBound = null, IReadOnlyList<double> scales = null, IReadOnlyList<bool> isFixed = null,
        double initialMu = 1E-3, double gradientTolerance = 1E-15, double stepTolerance = 1E-15, double functionTolerance = 1E-15, int maximumIterations = -1)
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

      _pInt = Vector<double>.Build.Dense(initialGuess.Count);
      _pExt = Vector<double>.Build.Dense(initialGuess.Count);
      var Pstep = Vector<double>.Build.Dense(initialGuess.Count);
      var Pnew = Vector<double>.Build.Dense(initialGuess.Count);

      _diagonalOfHessian = Vector<double>.Build.Dense(initialGuess.Count);
      _diagonalOfHessianPlusMu = Vector<double>.Build.Dense(initialGuess.Count);

      objective.SetParameters(initialGuess, isFixed);
      ExitCondition exitCondition = ExitCondition.None;

      // First, calculate function values and setup variables
      var PInt = _pInt;
      var PExt = initialGuess;
      ProjectToInternalParameters(initialGuess, _pInt); // current internal parameters
      var RSS = EvaluateFunction(objective, PInt);  // Residual Sum of Squares = R'R

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
      var (NegativeGradient, Hessian) = EvaluateJacobian(objective, PInt, PExt);
      Hessian.Diagonal(_diagonalOfHessian); // diag(H)

      // if ||g||oo <= gtol, found and stop
      if (NegativeGradient.InfinityNorm() <= gradientTolerance)
      {
        exitCondition = ExitCondition.RelativeGradient;
      }

      if (exitCondition != ExitCondition.None)
      {
        return new NonlinearMinimizationResult(objective, -1, exitCondition);
      }

      double mu = initialMu * _diagonalOfHessian.Max(); // μ
      double nu = 2; //  ν
      int iterations = 0;
      var MuTimesPStepMinusGradient = Vector<double>.Build.Dense(initialGuess.Count);
      while (iterations < maximumIterations && exitCondition == ExitCondition.None)
      {
        iterations++;

        while (true)
        {
          _diagonalOfHessian.Add(mu, _diagonalOfHessianPlusMu);
          Hessian.SetDiagonal(_diagonalOfHessianPlusMu); // hessian[i, i] = hessian[i, i] + mu;

          // solve normal equations
          Hessian.Solve(NegativeGradient, Pstep);

          // if ||ΔP|| <= xTol * (||P|| + xTol), found and stop
          if (Pstep.L2Norm() <= stepTolerance * (stepTolerance + PInt.DotProduct(PInt)))
          {
            exitCondition = ExitCondition.RelativePoints;
            break;
          }

          PInt.Add(Pstep, Pnew); // Pnew = PInt + Pstep; new parameters to test
          var RSSnew = EvaluateFunction(objective, Pnew); // evaluate function at Pnew

          if (double.IsNaN(RSSnew))
          {
            exitCondition = ExitCondition.InvalidValues;
            break;
          }

          // calculate the ratio of the actual to the predicted reduction.
          // ρ = (RSS - RSSnew) / (Δp'(μΔp - g))
          Pstep.Multiply(mu, MuTimesPStepMinusGradient); MuTimesPStepMinusGradient.Add(NegativeGradient, MuTimesPStepMinusGradient); // mu * Pstep + NegativeGradient
          var predictedReduction = Pstep.DotProduct(MuTimesPStepMinusGradient);
          var rho = (predictedReduction != 0)
                  ? (RSS - RSSnew) / predictedReduction
                  : 0;

          if (rho > 0.0)
          {
            // accepted
            Pnew.CopyTo(PInt);
            RSS = RSSnew;

            // update gradient and Hessian (note: PExt is already updated before)
            (NegativeGradient, Hessian) = EvaluateJacobian(objective, PInt, PExt);
            Hessian.Diagonal(_diagonalOfHessian);

            // if ||g||_oo <= gtol, found and stop
            if (NegativeGradient.InfinityNorm() <= gradientTolerance)
            {
              exitCondition = ExitCondition.RelativeGradient;
            }

            // if ||R||^2 < fTol, found and stop
            if (RSS <= functionTolerance)
            {
              exitCondition = ExitCondition.Converged; // SmallRSS
            }

            mu = mu * Math.Max(1.0 / 3.0, 1.0 - Math.Pow(2.0 * rho - 1.0, 3));
            nu = 2;

            break;
          }
          else
          {
            // rejected, increased μ
            mu = mu * nu;
            nu = 2 * nu;
          }
        }
      }

      if (iterations >= maximumIterations)
      {
        exitCondition = ExitCondition.ExceedIterations;
      }

      return new NonlinearMinimizationResult(objective, iterations, exitCondition);
    }
  }
}
