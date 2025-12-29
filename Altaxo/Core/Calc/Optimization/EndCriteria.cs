#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Copyright (c) 2003-2004, dnAnalytics. All rights reserved.
//
//    modified for Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

/*
 * EndCriteria.cs
 *
 * Copyright (c) 2004, dnAnalytics Project. All rights reserved.
 * NB: EndCriteria class inspired by the optimization framework in the QuantLib library
*/

using System;

namespace Altaxo.Calc.Optimization
{
  /// <summary>Defines criteria for ending an optimization.</summary>
  /// <remarks>
  /// <para>Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved. See <a>http://www.dnAnalytics.net</a> for details.</para>
  /// <para>Adopted for Altaxo (c) 2005 Dr. Dirk Lellinger.</para>
  /// </remarks>
  public class EndCriteria : IFormattable
  {
    /// <summary>Initializes a new instance of the <see cref="EndCriteria"/> class with default values.</summary>
    public EndCriteria()
      : this(1000, 1e-8, 10000, 100)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="EndCriteria"/> class.</summary>
    /// <param name="maxiteration">Maximum number of iterations.</param>
    /// <param name="epsilon">Minimum epsilon used for function/gradient/Hessian termination checks.</param>
    /// <param name="maxfunctionevaluation">Maximum number of function evaluations.</param>
    /// <param name="maxstationarypointiterations">Maximum number of consecutive stationary iterations before termination.</param>
    public EndCriteria(int maxiteration, double epsilon, int maxfunctionevaluation, int maxstationarypointiterations)
    {
      maxIteration = maxiteration;
      maxFunctionEvaluation = maxfunctionevaluation;
      maxGradientEvaluation = maxfunctionevaluation;
      maxHessianEvaluation = maxfunctionevaluation;
      maxStationaryPointIterations = maxstationarypointiterations;
      maxStationaryGradientIterations = maxstationarypointiterations;
      maxStationaryHessianIterations = maxstationarypointiterations;
      minFunctionEpsilon = epsilon;
      minGradientEpsilon = epsilon;
      minHessianEpsilon = epsilon;
      Reset();
    }

    /// <summary>Possible criteria used to end optimization.</summary>
    /// <remarks>
    /// Optimization can end because the maximum number of iterations has been reached, a stationary point has been reached,
    /// or the objective/gradient/Hessian meet their respective epsilon thresholds.
    /// </remarks>
    public enum CriteriaType
    {
      None, MaximumIteration,
      MaximumFunctionEvaluation, MaximumGradientEvaluation, MaximumHessianEvaluation,
      StationaryPoint, StationaryGradient, StationaryHessian,
      FunctionEpsilon, GradientEpsilon, HessianEpsilon
    };

    /// <summary>Maximum number of iterations.</summary>
    public int maxIteration;

    /// <summary>Current number of iterations.</summary>
    public int iterationCounter;

    /// <summary>Maximum number of function evaluations.</summary>
    public int maxFunctionEvaluation;

    /// <summary>Current number of function evaluations.</summary>
    public int functionEvaluationCounter;

    /// <summary>Maximum number of gradient evaluations.</summary>
    public int maxGradientEvaluation;

    /// <summary>Current number of gradient evaluations.</summary>
    public int gradientEvaluationCounter;

    /// <summary>Maximum number of Hessian evaluations.</summary>
    public int maxHessianEvaluation;

    /// <summary>Current number of Hessian evaluations.</summary>
    public int hessianEvaluationCounter;

    /// <summary>Minimum function epsilon.</summary>
    public double minFunctionEpsilon;

    /// <summary>Minimum gradient epsilon.</summary>
    public double minGradientEpsilon;

    /// <summary>Minimum Hessian epsilon.</summary>
    public double minHessianEpsilon;

    /// <summary>Maximum number of iterations at a stationary point.</summary>
    public int maxStationaryPointIterations;

    /// <summary>Current number of iterations at a stationary point.</summary>
    public int stationaryPointIterationsCounter;

    /// <summary>Maximum number of iterations at a stationary gradient.</summary>
    public int maxStationaryGradientIterations;

    /// <summary>Current number of iterations at a stationary gradient.</summary>
    public int stationaryGradientIterationsCounter;

    /// <summary>Maximum number of iterations at a stationary Hessian.</summary>
    public int maxStationaryHessianIterations;

    /// <summary>Current number of iterations at a stationary Hessian.</summary>
    public int stationaryHessianIterationsCounter;

    /// <summary>Indicates whether a stationary criterion is currently being evaluated.</summary>
    protected bool stationaryCriteria = false;

    /// <summary>The criterion that ended optimization.</summary>
    protected CriteriaType endCriteria = CriteriaType.None;

    /// <summary>Gets the criterion that was satisfied and thus ended optimization.</summary>
    public CriteriaType Criteria
    {
      get
      {
        return endCriteria;
      }
    }

    /// <summary>Checks whether the maximum iteration count has been reached.</summary>
    /// <remarks>
    /// If the iteration count is equal to or greater than the maximum number of iterations, the ending criterion is set to
    /// <c>CriteriaType.MaximumIteration</c> and the method returns <see langword="true"/>.
    /// </remarks>
    /// <returns><see langword="true"/> if the criterion is met; otherwise, <see langword="false"/>.</returns>
    public bool CheckIterations()
    {
      bool test = (iterationCounter >= maxIteration);
      if (test)
        endCriteria = CriteriaType.MaximumIteration;
      return test;
    }

    /// <summary>Checks whether the maximum number of function evaluations has been reached.</summary>
    /// <remarks>
    /// If the number of function evaluations is equal to or greater than the maximum number of function evaluations, the ending
    /// criterion is set to <c>CriteriaType.MaximumFunctionEvaluation</c> and the method returns <see langword="true"/>.
    /// </remarks>
    /// <returns><see langword="true"/> if the criterion is met; otherwise, <see langword="false"/>.</returns>
    public bool CheckFunctionEvaluations()
    {
      bool test = (functionEvaluationCounter >= maxFunctionEvaluation);
      if (test)
        endCriteria = CriteriaType.MaximumFunctionEvaluation;
      return test;
    }

    /// <summary>Checks whether the maximum number of gradient evaluations has been reached.</summary>
    /// <remarks>
    /// If the number of gradient evaluations is equal to or greater than the maximum number of gradient evaluations, the ending
    /// criterion is set to <c>CriteriaType.MaximumGradientEvaluation</c> and the method returns <see langword="true"/>.
    /// </remarks>
    /// <returns><see langword="true"/> if the criterion is met; otherwise, <see langword="false"/>.</returns>
    public bool CheckGradientEvaluations()
    {
      bool test = (gradientEvaluationCounter >= maxGradientEvaluation);
      if (test)
        endCriteria = CriteriaType.MaximumGradientEvaluation;
      return test;
    }

    /// <summary>Checks whether the maximum number of Hessian evaluations has been reached.</summary>
    /// <remarks>
    /// If the number of Hessian evaluations is equal to or greater than the maximum number of Hessian evaluations, the ending
    /// criterion is set to <c>CriteriaType.MaximumHessianEvaluation</c> and the method returns <see langword="true"/>.
    /// </remarks>
    /// <returns><see langword="true"/> if the criterion is met; otherwise, <see langword="false"/>.</returns>
    public bool CheckHessianEvaluations()
    {
      bool test = (hessianEvaluationCounter >= maxHessianEvaluation);
      if (test)
        endCriteria = CriteriaType.MaximumHessianEvaluation;
      return test;
    }

    /// <summary>Checks whether the objective function changed by less than the function epsilon.</summary>
    /// <param name="fold">The previous objective function value.</param>
    /// <param name="fnew">The current objective function value.</param>
    /// <remarks>
    /// If the change in objective function is less than the function epsilon, a possible stationary point has been found.
    /// If the number of repeated iterations at this possible stationary point exceeds the maximum, the ending criterion is set
    /// to <c>CriteriaType.StationaryPoint</c> and the method returns <see langword="true"/>.
    /// </remarks>
    /// <returns><see langword="true"/> if the criterion is met; otherwise, <see langword="false"/>.</returns>
    public bool CheckStationaryPoint(double fold, double fnew)
    {
      bool test = !((System.Math.Abs(fold - fnew) >= minFunctionEpsilon));
      if (test)
        stationaryPointIterationsCounter++;
      else if (stationaryPointIterationsCounter != 0)
        stationaryPointIterationsCounter = 0;
      if (stationaryPointIterationsCounter > maxStationaryPointIterations)
        endCriteria = CriteriaType.StationaryPoint;

      return (test && (stationaryPointIterationsCounter > maxStationaryPointIterations));
    }

    /// <summary>Checks whether the gradient changed by less than the gradient epsilon.</summary>
    /// <param name="gold">The previous gradient measure.</param>
    /// <param name="gnew">The current gradient measure.</param>
    /// <remarks>
    /// If the change in the gradient is less than the gradient epsilon, a possible stationary gradient has been found.
    /// If the number of repeated iterations at this possible stationary gradient exceeds the maximum, the ending criterion is set
    /// to <c>CriteriaType.StationaryGradient</c> and the method returns <see langword="true"/>.
    /// </remarks>
    /// <returns><see langword="true"/> if the criterion is met; otherwise, <see langword="false"/>.</returns>
    public bool CheckStationaryGradient(double gold, double gnew)
    {
      bool test = (System.Math.Abs(gold - gnew) < minGradientEpsilon);
      if (test)
        stationaryGradientIterationsCounter++;
      else if (stationaryGradientIterationsCounter != 0)
        stationaryPointIterationsCounter = 0;
      if (stationaryGradientIterationsCounter > maxStationaryGradientIterations)
        endCriteria = CriteriaType.StationaryGradient;

      return (test && (stationaryGradientIterationsCounter > maxStationaryGradientIterations));
    }

    /// <summary>Checks whether the Hessian changed by less than the Hessian epsilon.</summary>
    /// <param name="gold">The previous Hessian measure.</param>
    /// <param name="gnew">The current Hessian measure.</param>
    /// <remarks>
    /// If the change in the Hessian is less than the Hessian epsilon, a possible stationary Hessian has been found.
    /// If the number of repeated iterations at this possible stationary Hessian exceeds the maximum, the ending criterion is set
    /// to <c>CriteriaType.StationaryHessian</c> and the method returns <see langword="true"/>.
    /// </remarks>
    /// <returns><see langword="true"/> if the criterion is met; otherwise, <see langword="false"/>.</returns>
    public bool CheckStationaryHessian(double gold, double gnew)
    {
      bool test = (System.Math.Abs(gold - gnew) < minHessianEpsilon);
      if (test)
        stationaryHessianIterationsCounter++;
      else if (stationaryHessianIterationsCounter != 0)
        stationaryPointIterationsCounter = 0;
      if (stationaryHessianIterationsCounter > maxStationaryHessianIterations)
        endCriteria = CriteriaType.StationaryHessian;

      return (test && (stationaryHessianIterationsCounter > maxStationaryHessianIterations));
    }

    /// <summary>Checks whether the objective function value is less than the function epsilon.</summary>
    /// <param name="f">The objective function value.</param>
    /// <remarks>
    /// If the objective function value is less than the function epsilon, the ending criterion is set to
    /// <c>CriteriaType.FunctionEpsilon</c> and the method returns <see langword="true"/>.
    /// </remarks>
    /// <returns><see langword="true"/> if the criterion is met; otherwise, <see langword="false"/>.</returns>
    public bool CheckFunctionEpsilon(double f)
    {
      bool test = (f < minFunctionEpsilon);
      if (test)
        endCriteria = CriteriaType.FunctionEpsilon;
      return test;
    }

    /// <summary>Checks whether the norm of the gradient is less than the gradient epsilon.</summary>
    /// <param name="normDiff">The gradient norm (or a measure of the gradient magnitude).</param>
    /// <remarks>
    /// If the gradient norm is less than the gradient epsilon, the ending criterion is set to <c>CriteriaType.GradientEpsilon</c>
    /// and the method returns <see langword="true"/>.
    /// </remarks>
    /// <returns><see langword="true"/> if the criterion is met; otherwise, <see langword="false"/>.</returns>
    public bool CheckGradientEpsilon(double normDiff)
    {
      bool test = (normDiff < minGradientEpsilon);
      if (test)
        endCriteria = CriteriaType.GradientEpsilon;
      return test;
    }

    /// <summary>Checks whether the norm of the Hessian is less than the Hessian epsilon.</summary>
    /// <param name="normDiff">The Hessian norm (or a measure of the Hessian magnitude).</param>
    /// <remarks>
    /// If the Hessian norm is less than the Hessian epsilon, the ending criterion is set to <c>CriteriaType.HessianEpsilon</c>
    /// and the method returns <see langword="true"/>.
    /// </remarks>
    /// <returns><see langword="true"/> if the criterion is met; otherwise, <see langword="false"/>.</returns>
    public bool CheckHessianEpsilon(double normDiff)
    {
      bool test = (normDiff < minHessianEpsilon);
      if (test)
        endCriteria = CriteriaType.HessianEpsilon;
      return test;
    }

    /// <summary>Checks whether any ending criteria are met.</summary>
    /// <param name="fold">The previous objective function value.</param>
    /// <param name="fnew">The current objective function value.</param>
    /// <remarks>Returns <see langword="true"/> if optimization should continue; otherwise, returns <see langword="false"/>.</remarks>
    /// <returns><see langword="true"/> if optimization should continue; otherwise, <see langword="false"/>.</returns>
    public bool CheckCriteria(double fold, double fnew)
    {
      return !(
        CheckIterations() ||
        CheckFunctionEvaluations() ||
        CheckStationaryPoint(fold, fnew) ||
        CheckFunctionEpsilon(fnew) ||
        CheckFunctionEpsilon(fold)
        );
    }

    /// <summary>Checks whether any gradient-related ending criteria are met.</summary>
    /// <param name="normgold">The previous gradient norm.</param>
    /// <param name="normgnew">The current gradient norm.</param>
    /// <remarks>Returns <see langword="true"/> if optimization should continue; otherwise, returns <see langword="false"/>.</remarks>
    /// <returns><see langword="true"/> if optimization should continue; otherwise, <see langword="false"/>.</returns>
    public bool CheckGradientCriteria(double normgold, double normgnew)
    {
      return !(
        CheckGradientEvaluations() ||
        CheckStationaryGradient(normgnew, normgold) ||
        CheckGradientEpsilon(normgnew) ||
        CheckGradientEpsilon(normgold));
    }

    /// <summary>Resets all counters.</summary>
    public void Reset()
    {
      iterationCounter = 0;
      functionEvaluationCounter = 0;
      gradientEvaluationCounter = 0;
      hessianEvaluationCounter = 0;
      stationaryPointIterationsCounter = 0;
      stationaryGradientIterationsCounter = 0;
      stationaryHessianIterationsCounter = 0;
      endCriteria = CriteriaType.None;
    }

    // --- IFormattable Interface ---

    /// <inheritdoc/>
    public override string ToString()
    {
      return ToString(null, null);
    }

    /// <summary>A string representation of this <c>EndCriteria</c>.</summary>
    /// <param name="format">A format specification.</param>
    public string ToString(string format)
    {
      return ToString(format, null);
    }

    /// <summary>A string representation of this <c>EndCriteria</c>.</summary>
    /// <param name="formatProvider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information.</param>
    public string ToString(IFormatProvider formatProvider)
    {
      return ToString(null, formatProvider);
    }

    /// <summary>A string representation of this <c>EndCriteria</c>.</summary>
    /// <param name="format">A format specification.</param>
    /// <param name="formatProvider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information.</param>
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
      if (endCriteria == CriteriaType.MaximumFunctionEvaluation)
        return "Maximum Function Evaluations";
      else if (endCriteria == CriteriaType.MaximumIteration)
        return "Maximum Iterations";
      else if (endCriteria == CriteriaType.MaximumGradientEvaluation)
        return "Maximum Gradient Evaluations";
      else if (endCriteria == CriteriaType.StationaryPoint)
        return "Stationary Point";
      else if (endCriteria == CriteriaType.StationaryGradient)
        return "Stationary Gradient";
      else if (endCriteria == CriteriaType.FunctionEpsilon)
        return "Function Epsilon Exceeded";
      else if (endCriteria == CriteriaType.GradientEpsilon)
        return "Gradient Epsilon Exceeded";
      else
        return "None";
    }
  }
}
