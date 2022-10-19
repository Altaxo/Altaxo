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
using System.Threading;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Collections;

namespace Altaxo.Calc.Optimization
{
  /// <summary>
  /// LevenbergMarquardtMinimizer, that doesn't allocate memory during the iterations.
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
  public class LevenbergMarquardtMinimizerNonAllocatingWrappedParameters : NonlinearMinimizerBaseNonAllocating
  {
    /// <summary>
    /// The default scale factor for initial mu.
    /// </summary>
    public const double DefaultInitialMu = 1E-3;


    /// <summary>
    /// The scale factor for initial mu
    /// </summary>
    public double InitialMu { get; set; } = DefaultInitialMu;

    /// <summary>
    /// Gets or sets the number of iterations after which the parameter scale is updated (if no user provided scale was set).
    /// The default value is 1, that means the parameter scale is updated in each iteration.
    /// Set the value to int.MaximumValue if no scale update is neccessary.
    /// </summary>
    public int ParameterScaleUpdatePeriod { get; set; } = 1;



    /// <summary>
    /// Non-linear least square fitting by the Levenberg-Marquardt algorithm.
    /// </summary>
    /// <param name="objective">The objective function, including model, observations, and parameter bounds.</param>
    /// <param name="initialGuess">The initial guess values.</param>
    /// <param name="cancellationToken">Token to cancel the evaluation</param>
    /// <param name="reportChi2Progress">Event handler that can be used to report the NumberOfIterations and Chi² value achived so far. Can be null</param>
    /// <returns>The result of the Levenberg-Marquardt minimization</returns>
    public NonlinearMinimizationResult FindMinimum(
     IObjectiveModelNonAllocating objective,
     IReadOnlyList<double> initialGuess,
     CancellationToken cancellationToken,
     Action<int, double>? reportChi2Progress
     )
    {
      return FindMinimum(objective, initialGuess, null, null, null, null, cancellationToken, reportChi2Progress);
    }

    /// <summary>
    /// Non-linear least square fitting by the Levenberg-Marquardt algorithm.
    /// </summary>
    /// <param name="objective">The objective function, including model, observations, and parameter bounds.</param>
    /// <param name="initialGuess">The initial guess values.</param>
    /// <param name="lowerBound">The lower bounds of the parameters. The array must have the same length as the parameter array. Provide null if not needed.</param>
    /// <param name="upperBound">The upper bounds of the parameters. The array must have the same length as the parameter array. Provide null if not needed.</param>
    /// <param name="scales">The scales of the parameters. The array must have the same length as the parameter array. Provide null if not needed.</param>
    /// <param name="isFixed">Array of booleans, which provide which parameters are fixed. Must have the same length as the parameter array. Provide null if not needed.</param>
    /// <param name="cancellationToken">Token to cancel the evaluation</param>
    /// <param name="reportChi2Progress">Event handler that can be used to report the NumberOfIterations and Chi² value achived so far. Can be null</param>
    /// <returns>The result of the Levenberg-Marquardt minimization</returns>
    public NonlinearMinimizationResult FindMinimum(
      IObjectiveModelNonAllocating objective,
      IReadOnlyList<double> initialGuess,
      IReadOnlyList<double?>? lowerBound,
      IReadOnlyList<double?>? upperBound,
      IReadOnlyList<double>? scales,
      IReadOnlyList<bool>? isFixed,
      CancellationToken cancellationToken,
      Action<int, double>? reportChi2Progress
      )
    {
      return Minimum(
        objective,
        initialGuess,
        lowerBound,
        upperBound,
        scales,
        isFixed,
        cancellationToken,
        reportChi2Progress,
        InitialMu,
        GradientTolerance,
        StepTolerance,
        functionTolerance: FunctionTolerance,
        minimalRSSImprovement: MinimalRSSImprovement,
        maximumIterations: MaximumIterations);
    }

    /// <summary>
    /// Non-linear least square fitting by the Levenberg-Marquardt algorithm.
    /// </summary>
    /// <param name="objective">The objective function, including model, observations, and parameter bounds.</param>
    /// <param name="initialGuess">The initial guess values.</param>
    /// <param name="lowerBound">The lower bounds of the parameters. The array must have the same length as the parameter array. Provide null if not needed.</param>
    /// <param name="upperBound">The upper bounds of the parameters. The array must have the same length as the parameter array. Provide null if not needed.</param>
    /// <param name="scales">The scales of the parameters. The array must have the same length as the parameter array. Provide null if not needed.</param>
    /// <param name="isFixed">Array of booleans, which provide which parameters are fixed. Must have the same length as the parameter array. Provide null if not needed.</param>
    /// <param name="cancellationToken">Token to cancel the evaluation</param>
    /// <param name="reportChi2Progress">Event handler that can be used to report the NumberOfIterations and Chi² value achived so far. Can be null</param>
    /// <returns>The result of the Levenberg-Marquardt minimization</returns>
    public NonlinearMinimizationResult FindMinimum(
      IObjectiveModelNonAllocating objective,
      double[] initialGuess,
      double?[]? lowerBound,
      double?[]? upperBound,
      double[]? scales,
      bool[]? isFixed,
      CancellationToken cancellationToken,
      Action<int, double>? reportChi2Progress
      )
    {
      if (objective is null)
        throw new ArgumentNullException(nameof(objective));
      if (initialGuess is null)
        throw new ArgumentNullException(nameof(initialGuess));

      return Minimum(
        objective,
        CreateVector.DenseOfArray(initialGuess),
        lowerBound,
        upperBound,
        scales,
        isFixed,
        cancellationToken,
        reportChi2Progress,
        InitialMu,
        GradientTolerance,
        StepTolerance,
        FunctionTolerance,
        MinimalRSSImprovement,
        MaximumIterations);
    }

    /// <summary>
    /// Non-linear least square fitting by the Levenberg-Marquardt algorithm.
    /// </summary>
    /// <param name="objective">The objective function, including model, observations, and parameter bounds.</param>
    /// <param name="initialGuess">The initial guess values.</param>
    /// <param name="lowerBound">The lower bounds of the parameters. The array must have the same length as the parameter array. Provide null if not needed.</param>
    /// <param name="upperBound">The upper bounds of the parameters. The array must have the same length as the parameter array. Provide null if not needed.</param>
    /// <param name="scales">The scales of the parameters. The array must have the same length as the parameter array. Provide null if not needed.</param>
    /// <param name="isFixedByUser">Array of booleans, which provide which parameters are fixed. Must have the same length as the parameter array. Provide null if not needed.</param>
    /// <param name="cancellationToken">Token to cancel the evaluation</param>
    /// <param name="reportChi2Progress">Event handler that can be used to report the NumberOfIterations and Chi² value achived so far. Can be null</param>
    /// <param name="initialMu">The initial damping parameter of mu.</param>
    /// <param name="gradientTolerance">The stopping threshold for infinity norm of the gradient vector.</param>
    /// <param name="stepTolerance">The stopping threshold for L2 norm of the change of parameters.</param>
    /// <param name="functionTolerance">The stopping threshold for L2 norm of the residuals.</param>
    /// <param name="minimalRSSImprovement">The minimal improvement of the Chi² value in 8 iterations. Must be in the range [0,1).</param>
    /// <param name="maximumIterations">The maximal number of iterations. Provide -1 if the number of iterations should be set automatically. Provide 0 if only a function evaluation should be done.</param>
    /// <returns>The result of the Levenberg-Marquardt minimization</returns>
    public NonlinearMinimizationResult Minimum(
      IObjectiveModelNonAllocating objective,
      IReadOnlyList<double> initialGuess,
      IReadOnlyList<double?>? lowerBound,
      IReadOnlyList<double?>? upperBound,
      IReadOnlyList<double>? scales,
      IReadOnlyList<bool>? isFixedByUser,
      CancellationToken cancellationToken,
      Action<int, double>? reportChi2Progress,
      double initialMu,
      double gradientTolerance,
      double stepTolerance,
      double functionTolerance,
      double minimalRSSImprovement,
      int? maximumIterations)
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

      var scaleFactors = Vector<double>.Build.Dense(initialGuess.Count);
      var pInt = Vector<double>.Build.Dense(initialGuess.Count);
      var pExt = Vector<double>.Build.DenseOfEnumerable(initialGuess);
      var Pstep = Vector<double>.Build.Dense(initialGuess.Count);
      var Pnew = Vector<double>.Build.Dense(initialGuess.Count);

      var diagonalOfHessian = Vector<double>.Build.Dense(initialGuess.Count);
      var diagonalOfHessianPlusMu = Vector<double>.Build.Dense(initialGuess.Count);
      var rssValueHistory = new RingBufferEnqueueableOnly<double>(8); // Stores the last 8 values of Chi².

      objective.SetParameters(initialGuess, isFixedByUser);
      ExitCondition exitCondition = ExitCondition.None;

      // First, calculate function values and setup variables
      ProjectToInternalParameters(pExt, pInt); // current internal parameters
      var RSS = EvaluateFunction(objective, pInt, pExt);  // Residual Sum of Squares = R'R
      reportChi2Progress?.Invoke(0, RSS);

      if (!maximumIterations.HasValue)
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


      var useAutomaticParameterScale = Scales is null;
      var iterationOfLastAutomaticParameterScaleEvaluation = 0;
      if (useAutomaticParameterScale)
      {
        objective.Hessian.Diagonal(diagonalOfHessian); // we can use the unscaled diagonalOfHessian here for temporary purpose, because it is overwritten immediately below
        ScaleFactorsOfJacobian(pInt, scaleFactors); // Calculate the scaleFactors for Hessian and Gradient with Scales==null (i.e., the Scales coming from the boundary conditions only))
        Scales = Vector<double>.Build.Dense(diagonalOfHessian.Count);
        diagonalOfHessian.Map2((x, y) => (x != 0 && y != 0) ? 1 / Math.Sqrt(Math.Abs(x * (y * y))) : 1, scaleFactors, Scales, Zeros.Include); // Scales now contain the scales that will bring the Hessian diagonal to 1
        ProjectToInternalParameters(pExt, pInt); // we need to calculate current internal parameters anew, because Scales has changed
      }

      // Evaluate gradient (already negated!) and Hessian
      var (NegativeGradient, Hessian) = EvaluateJacobian(objective, pInt, scaleFactors);
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
      var MuTimesPStepMinusGradient = Vector<double>.Build.Dense(initialGuess.Count);
      while (iterations < maximumIterations && exitCondition == ExitCondition.None)
      {
        iterations++;

        while (true)
        {
          cancellationToken.ThrowIfCancellationRequested();

          diagonalOfHessian.Add(mu, diagonalOfHessianPlusMu);
          Hessian.SetDiagonal(diagonalOfHessianPlusMu); // hessian[i, i] = hessian[i, i] + mu; see [2] eq. (12), page 3

          // solve normal equations
          Hessian.Solve(NegativeGradient, Pstep);

          // Test if there is convergence in the parameters
          // if max|hi/pi| < xTol, stop (see [2], Section 4.1.3 (page 5), second criterion
          var maxHiByPi = Pstep.Zip(pInt, (hi, pi) => hi == 0 ? 0 : pi == 0 ? double.PositiveInfinity : Math.Abs(hi / pi)).Max();
          if (maxHiByPi < stepTolerance)
          {
            exitCondition = ExitCondition.RelativePoints;
            break;
          }

          pInt.Add(Pstep, Pnew); // Pnew = PInt + Pstep; new parameters to test
          var RSSnew = EvaluateFunction(objective, Pnew, pExt); // evaluate function at Pnew

          double rho;
          if (double.IsNaN(RSSnew))
          {
            // if the new RSS value is NaN, this might be a sign that the step was chosen too wide,
            // causing some parameters out of the feasible range
            // that's why we do not exit here, but set rho to 0 in order to increase mu afterwards
            rho = 0;
          }
          else
          {

            // calculate the ratio of the actual to the predicted reduction, see [2], eq. 15, page 3 
            // ρ = (RSS - RSSnew) / (Δp'(μΔp - g))
            Pstep.Multiply(mu, MuTimesPStepMinusGradient); // calculate μΔp
            MuTimesPStepMinusGradient.Add(NegativeGradient, MuTimesPStepMinusGradient); // calculate (μΔp - g)
            var predictedReduction = Pstep.DotProduct(MuTimesPStepMinusGradient); // calculate (Δp'(μΔp - g))
            rho = (predictedReduction > 0)
                    ? (RSS - RSSnew) / predictedReduction
                    : 0;
          }

          if (rho > 0)
          {
            // accepted
            Pnew.CopyTo(pInt);
            RSS = RSSnew;
            rssValueHistory.Enqueue(RSS);
            reportChi2Progress?.Invoke(iterations, RSS);

            // update the parameter scales, if automatic scales was used (but only every 'ParameterScaleUpdatePeriod' iterations)
            if (useAutomaticParameterScale && iterations >= (ParameterScaleUpdatePeriod + iterationOfLastAutomaticParameterScaleEvaluation))
            {
              objective.Hessian.Diagonal(diagonalOfHessian); // we can use the unscaled diagonalOfHessian here for temporary purpose, because it is overwritten immediately below
              ScaleFactorsOfJacobian(pInt, scaleFactors); // Calculate the scaleFactors for Hessian and Gradient with the current Scales values
              diagonalOfHessian.Map2((x, y) => (x != 0 && y != 0) ? 1 / Math.Sqrt(Math.Abs(x * (y * y))) : 1, scaleFactors, scaleFactors, Zeros.Include); // in scaleFactors now are only the correction factors for Scales (should be not far from 1)
              Scales.PointwiseMultiply(scaleFactors, Scales); // multipliy Scales with the correction factors
              ProjectToInternalParameters(pExt, pInt); // we need to calculate current internal parameters anew, because Scales has changed
              iterationOfLastAutomaticParameterScaleEvaluation = iterations;
            }

            // update gradient and Hessian 
            (NegativeGradient, Hessian) = EvaluateJacobian(objective, pInt, scaleFactors);
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
            if (rssValueHistory.Count == rssValueHistory.Capacity)
            {
              var RSSImprovement = (rssValueHistory.OldestValue - rssValueHistory.NewestValue) / rssValueHistory.OldestValue;
              if (RSSImprovement < minimalRSSImprovement)
              {
                exitCondition = ExitCondition.Converged; // low RSS improvement
              }
            }

            // this step was accepted, thus decrease mu depending on the value of rho
            mu *= Math.Max(1.0 / 3.0, 1.0 - Pow3(2.0 * rho - 1.0)); // see [2], section 4.1.1, point 3
            nu = 2;

            break;
          }
          else
          {
            // this step was rejected, thus increase mu by multiplying with nu, and double nu, resulting in an exponential increase when consecutive steps are rejected
            mu *= nu;
            nu *= 2;

            if (!(mu < double.MaxValue && nu < double.MaxValue))
            {
              // if mu becomes too large, this is maybe because our function always delivers
              // an RSS value which is NaN 
              exitCondition = double.IsNaN(RSSnew) ? ExitCondition.InvalidValues : ExitCondition.RelativeGradient;
              break;
            }
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
  }
}
