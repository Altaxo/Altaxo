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
  public class LevenbergMarquardtMinimizerNonAllocating : NonlinearMinimizerBaseNonAllocating
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
     Action<int, double, IReadOnlyList<double>>? reportChi2Progress
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
    /// <param name="reportChi2Progress">Event handler that can be used to report the NumberOfIterations, Chi² value and current parameter set achived so far. Can be null</param>
    /// <returns>The result of the Levenberg-Marquardt minimization</returns>
    public NonlinearMinimizationResult FindMinimum(
      IObjectiveModelNonAllocating objective,
      IReadOnlyList<double> initialGuess,
      IReadOnlyList<double?>? lowerBound,
      IReadOnlyList<double?>? upperBound,
      IReadOnlyList<double>? scales,
      IReadOnlyList<bool>? isFixed,
      CancellationToken cancellationToken,
      Action<int, double, IReadOnlyList<double>>? reportChi2Progress
      )
    {
      return Minimum(objective,
        initialGuess,
        lowerBound,
        upperBound,
        scales,
        isFixed,
        cancellationToken,
        reportChi2Progress,
        initialMu: InitialMu,
        gradientTolerance: GradientTolerance,
        stepTolerance: StepTolerance,
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
      Action<int, double, IReadOnlyList<double>>? reportChi2Progress
      )
    {
      if (objective is null)
        throw new ArgumentNullException(nameof(objective));
      if (initialGuess is null)
        throw new ArgumentNullException(nameof(initialGuess));

      return Minimum(objective,
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
        Action<int, double, IReadOnlyList<double>>? reportChi2Progress,
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

      var numberOfVaryingParameters = initialGuess.Count;
      if (isFixedByUser is not null)
      {
        if (isFixedByUser.Count != initialGuess.Count)
        {
          throw new ArgumentException($"{nameof(isFixedByUser)} must have the same length as {nameof(initialGuess)}", nameof(isFixedByUser));
        }
        numberOfVaryingParameters = initialGuess.Count - isFixedByUser.Count(x => x);
        if (numberOfVaryingParameters == 0)
        {
          maximumIterations = 0; // if all parameters are fixed, we only do function evaluation by setting maximumIterations to 0
        }
      }



      var parameterValues = Vector<double>.Build.DenseOfEnumerable(initialGuess);
      var parameterStep = Vector<double>.Build.Dense(initialGuess.Count);
      var scaledParameterStep = Vector<double>.Build.Dense(initialGuess.Count);
      var newParameterValues = Vector<double>.Build.Dense(initialGuess.Count);
      var diagonalOfHessian = Vector<double>.Build.Dense(initialGuess.Count);
      var diagonalOfHessianPlusMu = Vector<double>.Build.Dense(initialGuess.Count);
      var rssValueHistory = new RingBufferEnqueueableOnly<double>(8); // Stores the last 8 values of Chi².

      objective.SetParameters(initialGuess, isFixedByUser);
      ExitCondition exitCondition = ExitCondition.None;


      // First, calculate function values and setup variables
      objective.EvaluateAt(parameterValues);
      var RSS = objective.Value;  // Residual Sum of Squares = R'R
      reportChi2Progress?.Invoke(0, RSS, parameterValues);

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



      // Evaluate at first the gradient (already negated!) and the Hessian
      var (NegativeGradient, Hessian) = EvaluateJacobian(objective, parameterValues);
      var useAutomaticParameterScale = Scales is null;
      var iterationOfLastAutomaticParameterScaleEvaluation = 0;
      if (useAutomaticParameterScale)
      {
        // if no scale for the parameters was given, calculate scale parameters in a way, that the resulting
        // gradient has equal elements (absolute value).
        // here, we use the diagonal of the Hessian to calculate the parameter scale, so that the Hessian would have values of 1 in the diagonal
        // alternatively, we could scale the parameters so that the gradient contains either 1 or -1 elements
        // Scales = NegativeGradient.Map(x => x != 0 ? 1 / Math.Abs(x) : 1, Zeros.Include); // autoscale using the gradient
        Scales = Hessian.Diagonal().Map(x => x != 0 ? 1 / Math.Sqrt(Math.Abs(x)) : 1, Zeros.Include); // autoscale using the diagonal of the Hessian

        // after the parameter scale was evaluated, we have to repeat the Jacobian and Hessian evaluation, this time with the parameter scale in operation
        (NegativeGradient, Hessian) = EvaluateJacobian(objective, parameterValues); // repeat Jacobian evaluation, now with scale
      }


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
      var isFixedByUserOrBoundary = new bool[initialGuess.Count];
      while (iterations < maximumIterations && exitCondition == ExitCondition.None)
      {
        iterations++;

        while (true)
        {
          cancellationToken.ThrowIfCancellationRequested();
          diagonalOfHessian.Add(mu, diagonalOfHessianPlusMu);



          // Modification of the Levenberg-Marquardt algorithm described in [2]:
          // in order to take the boundary conditions into account, we first solve the Hessian with only the user-fixed parameters considered
          // Then we evaluate which parameters are both at their boundaries and the step is directed outwards of the feasible region
          // Those parameters are set to temporarily fixed, and the Hessian is modified accordingly, and solved again
          // This is repeated until no more parameters are temporarily fixed.
          // Advantage of this method in comparison to the method of wrapped parameters is that:
          //    (1) the gradient is kept, even if the parameter is at a bound
          //    (2) it is clear which parameter is at a bound and which is not. Those parameters at a bound increase the degree of freedom.
          // Whereas with the method of wrapped parameters, if a parameter is at a bound, it can not be modified anymore, because its gradient value is zero.

          bool wasHessianModified = false;
          int numberOfFreeParameters;
          if (isFixedByUser is null)
          {
            Array.Clear(isFixedByUserOrBoundary, 0, isFixedByUserOrBoundary.Length);
          }
          else
          {
            VectorMath.Copy(isFixedByUser, isFixedByUserOrBoundary); // Start with a Hessian in which only the user-fixed parameters are considered, so that the other parameters, even those at a bound, have a chance to be varied
          }
          do
          {
            Hessian.SetDiagonal(diagonalOfHessianPlusMu); // hessian[i, i] = hessian[i, i] + mu; see [2] eq. (12), page 3

            // solve normal equations
            Hessian.Solve(NegativeGradient, scaledParameterStep); // by solving we get the scaled parameter step
            scaledParameterStep.PointwiseMultiply(Scales, parameterStep); // we evaluate the real (unscaled) parameter step

            ++numberOfSolves;

            // if the step would violate the boundary conditions, we modify the Hessian and the gradient accordingly
            (wasHessianModified, numberOfFreeParameters) = ModifyHessianAndGradient(parameterValues, parameterStep, mu, Hessian, NegativeGradient, diagonalOfHessianPlusMu, isFixedByUserOrBoundary);

          } while (wasHessianModified && numberOfFreeParameters > 0); // repeat, until no more parameters are temporarily fixed and at least one parameter can be varied

          if (numberOfFreeParameters == 0)
          {
            // all parameters are either fixed or at their bounds, thus no more parameters can be varied.
            exitCondition = ExitCondition.BoundTolerance;
            break;
          }

          // clamp the new step to the boundary conditions, and calculate the new parameter values
          var clampScaleFactor = ClampStepToBoundaryConditions(parameterValues, parameterStep, parameterStep, scaledParameterStep, newParameterValues); // newParameterValues: new parameters to test

          // Test if there is convergence in the parameters
          // if max|hi/pi| < xTol, stop (see [2], Section 4.1.3 (page 5), second criterion
          var maxHiByPi = parameterStep.Zip(parameterValues, (hi, pi) => hi == 0 ? 0 : pi == 0 ? double.PositiveInfinity : Math.Abs(hi / pi)).Max();
          if (maxHiByPi < stepTolerance * clampScaleFactor)
          {
            exitCondition = ExitCondition.RelativePoints;
            break;
          }

          objective.EvaluateAt(newParameterValues);
          var RSSnew = objective.Value; // evaluate function at the new parameter values

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
            scaledParameterStep.Multiply(mu, MuTimesPStepMinusGradient); // calculate μΔp
            MuTimesPStepMinusGradient.Add(NegativeGradient, MuTimesPStepMinusGradient); // calculate (μΔp - g)
            var predictedReduction = scaledParameterStep.DotProduct(MuTimesPStepMinusGradient); // calculate (Δp'(μΔp - g))
            rho = (predictedReduction > 0)
                    ? (RSS - RSSnew) / predictedReduction
                    : 0;
          }

          if (rho > 0)
          {
            // this step was accepted
            newParameterValues.CopyTo(parameterValues);
            RSS = RSSnew;
            rssValueHistory.Enqueue(RSS);
            reportChi2Progress?.Invoke(iterations, RSS, parameterValues);

            // update the parameter scales, if automatic scales was used (but only every 'ParameterScaleUpdatePeriod' iterations)
            if (useAutomaticParameterScale && iterations >= (ParameterScaleUpdatePeriod + iterationOfLastAutomaticParameterScaleEvaluation))
            {
              objective.Hessian.Diagonal(diagonalOfHessian); // we can use the unscaled diagonalOfHessian here for temporary purpose, because it is overwritten immediately below
              diagonalOfHessian.Map(x => x != 0 ? 1 / Math.Sqrt(Math.Abs(x)) : 1, Scales, Zeros.Include); // update scales from the diagonal of the Hessian
              iterationOfLastAutomaticParameterScaleEvaluation = iterations;
            }

            // update gradient and Hessian
            (NegativeGradient, Hessian) = EvaluateJacobian(objective, parameterValues);
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

      objective.IsFixedByUserOrBoundary = isFixedByUserOrBoundary; // set the fixed condition in the model
      return new NonlinearMinimizationResult(objective, iterations, exitCondition, isFixedByUserOrBoundary);
    }

    /// <summary>
    /// Returns x³.
    /// </summary>
    /// <param name="x">The argument x.</param>
    /// <returns>x³</returns>
    private static double Pow3(double x) => x * x * x;

    /// <summary>
    /// Makes a proposed step smaller if neccessary, so that the resulting parameters don't violate the parameter boundaries.
    /// </summary>
    /// <param name="parameterValues">The given parameters.</param>
    /// <param name="parameterStep">The proposed step.</param>
    /// <param name="clampedParameterStep">On return, contains the clamped step.</param>
    /// <param name="nextParameterValues">On return, contains the new parameters, i.e. <paramref name="parameterValues"/>+<paramref name="clampedParameterStep"/>.</param>
    /// <returns>The scale factor. The scale factor is either 1 (if the step was not scaled down), or less than 1 (if the step was scaled down).</returns>
    private double ClampStepToBoundaryConditions(IReadOnlyList<double> parameterValues, IReadOnlyList<double> parameterStep, IVector<double> clampedParameterStep, IVector<double> clampedScaledParameterStep, IVector<double> nextParameterValues)
    {
      double scaleFactor = 1; // Scale factor for step
      int idxLowestScale = -1;
      double valueParameterAtLowestScale = double.NaN;

      if (LowerBound is not null || UpperBound is not null)
      {
        for (int i = 0; i < parameterValues.Count; i++)
        {
          var lowerBnd = LowerBound?.ElementAt(i);
          var upperBnd = UpperBound?.ElementAt(i);

          if (lowerBnd.HasValue && parameterValues[i] > lowerBnd.Value)
          {
            if (parameterStep[i] < 0 && !(parameterValues[i] + scaleFactor * parameterStep[i] > lowerBnd.Value))
            {
              idxLowestScale = i;
              scaleFactor = (lowerBnd.Value - parameterValues[i]) / parameterStep[i];
              valueParameterAtLowestScale = lowerBnd.Value;
            }
          }
          if (upperBnd.HasValue && parameterValues[i] < upperBnd.Value)
          {
            if (parameterStep[i] > 0 && !(parameterValues[i] + scaleFactor * parameterStep[i] < upperBnd.Value))
            {
              idxLowestScale = i;
              scaleFactor = (upperBnd.Value - parameterValues[i]) / parameterStep[i];
              valueParameterAtLowestScale = upperBnd.Value;
            }
          }
        }
      }

      // calculate the clamped step, and the new parameters
      for (int i = 0; i < parameterValues.Count; i++)
      {
        var clampedParameterStep_i = scaleFactor * parameterStep[i];
        var nextValue = parameterValues[i] + clampedParameterStep_i;
        if (clampedParameterStep_i < 0 && nextValue < LowerBound?.ElementAt(i))
        {
          nextValue = LowerBound.ElementAt(i).Value;
          clampedParameterStep_i = nextValue - parameterValues[i];
        }
        else if (clampedParameterStep_i > 0 && nextValue > UpperBound?.ElementAt(i))
        {
          nextValue = UpperBound.ElementAt(i).Value;
          clampedParameterStep_i = nextValue - parameterValues[i];
        }

        clampedParameterStep[i] = clampedParameterStep_i;
        nextParameterValues[i] = nextValue;

        clampedScaledParameterStep[i] = clampedParameterStep[i] / Scales[i];
      }

      return scaleFactor;
    }



    /// <summary>
    /// Evaluates the jacobian, and the hessian of the objective function.
    /// </summary>
    /// <param name="objective">The objective.</param>
    /// <param name="parameterValues">The parameters.</param>
    /// <returns>The negative gradient and the Hessian matrix.</returns>
    protected new (Vector<double> NegativeGradient, Matrix<double> Hessian) EvaluateJacobian(IObjectiveModelNonAllocating objective, IReadOnlyList<double> parameterValues)
    {
      var negativeGradient = objective.NegativeGradient;
      var hessian = objective.Hessian;

      if (Scales is not null)
      {
        for (int i = 0; i < negativeGradient.Count; i++)
        {
          var scale_i = Scales.ElementAt(i);
          negativeGradient[i] *= scale_i;

          for (int j = 0; j < hessian.ColumnCount; j++)
          {
            hessian[i, j] = (hessian[i, j] * scale_i) * Scales.ElementAt(j); // the evaluation order forced by the braces will avoid overflow or underflow if scale_i*scale_j is too big or too small
          }
        }
      }

      return (negativeGradient, hessian);
    }

    /// <summary>
    /// Modifies the Hessian and gradient according to the boundary conditions of the parameters.
    /// </summary>
    /// <param name="Pint">The parameters.</param>
    /// <param name="pstep">The parameter step planned.</param>
    /// <param name="mu">The value of mu.</param>
    /// <param name="hessian">The Hessian matrix. If the return value is true, this matrix was modified during the call.</param>
    /// <param name="gradient">The negative gradient.  If the return value is true, this vector was modified during the call.</param>
    /// <param name="diagonalOfHessianPlusMu">The diagonal of the Hessian matrix plus mu.  If the return value is true, this vector was modified during the call.</param>
    /// <param name="isTemporaryFixed">The array of fixed parameters (parameters fixed from the beginning plus parameters that have reached the boundary). If the return value is true, this vector was modified during the call.</param>
    /// <returns>True if the Hessian and gradient were modified; otherwise, false. Additionally, the number of free parameters is returned.</returns>
    private (bool wasModified, int numberOfFreeParameters) ModifyHessianAndGradient(IReadOnlyList<double> Pint, IReadOnlyList<double> pstep, double mu, Matrix<double> hessian, Vector<double> gradient, Vector<double> diagonalOfHessianPlusMu, bool[] isTemporaryFixed)
    {
      bool wasModified = false;
      int numberOfFreeParameters = Pint.Count;
      if (LowerBound is not null || UpperBound is not null)
      {
        for (int i = 0; i < Pint.Count; i++)
        {
          if (isTemporaryFixed[i]) // if parameter i is already fixed, it doesn't need further consideration here.
          {
            --numberOfFreeParameters;
            continue;
          }

          var lowerBnd = LowerBound?.ElementAt(i);
          var upperBnd = UpperBound?.ElementAt(i);

          // if the parameter is beyond a bound and the step direction is in the direction of the outside of the feasible range,
          // then we have to make this parameter temporarily fixed, by setting the gradient value and the column and row of the Hessian to zero
          if (
                (upperBnd.HasValue && (!(Pint[i] < upperBnd.Value) && pstep[i] > 0)) ||
                (lowerBnd.HasValue && (!(Pint[i] > lowerBnd.Value) && pstep[i] < 0))
            )
          {
            gradient[i] = 0;
            hessian.ClearColumn(i);
            hessian.ClearRow(i);
            diagonalOfHessianPlusMu[i] = mu;
            isTemporaryFixed[i] = true;
            wasModified = true;
            --numberOfFreeParameters;
          }
        }
      }

      return (wasModified, numberOfFreeParameters);
    }

  }
}
