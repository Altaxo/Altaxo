#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Copyright (c) 2003-2004, dnAnalytics. All rights reserved.
//
//    modified for Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

/*
 * EndCriteria.cs
 * 
 * Copyright (c) 2004, dnAnalytics Project. All rights reserved.
 * NB: EndCriteria class inspired by the optimization framework in the QuantLib library
*/

using System;
using Altaxo.Calc.LinearAlgebra;
namespace Altaxo.Calc.Optimization
{

  ///<summary>Class to define criteria to end optimization</summary>
  /// <remarks>
  /// <para>Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved. See <a>http://www.dnAnalytics.net</a> for details.</para>
  /// <para>Adopted to Altaxo (c) 2005 Dr. Dirk Lellinger.</para>
  /// </remarks>
  public class EndCriteria : IFormattable 
  {
    ///<summary> Default constructor </summary>
    public EndCriteria() : this(1000,1e-8, 10000, 100) {}
    public EndCriteria(int maxiteration, double epsilon, int maxfunctionevaluation, int maxstationarypointiterations) 
    {
      this.maxIteration=maxiteration;
      this.maxFunctionEvaluation=maxfunctionevaluation;
      this.maxGradientEvaluation=maxfunctionevaluation;
      this.maxHessianEvaluation=maxfunctionevaluation;
      this.maxStationaryPointIterations=maxstationarypointiterations;
      this.maxStationaryGradientIterations=maxstationarypointiterations;
      this.maxStationaryHessianIterations=maxstationarypointiterations;
      this.minFunctionEpsilon=epsilon;
      this.minGradientEpsilon=epsilon;
      this.minHessianEpsilon=epsilon;
      Reset();
    }
    
    ///<summary>Possible criteria to end optimization</summary>
    ///<remarks>Optimization can end because the maximum number of iterations has been reached, 
    ///a stationary point has been reached, or a stationary gradient has been reached</remarks>
    public enum CriteriaType 
    {
      None, MaximumIteration, 
      MaximumFunctionEvaluation, MaximumGradientEvaluation, MaximumHessianEvaluation, 
      StationaryPoint, StationaryGradient, StationaryHessian,
      FunctionEpsilon, GradientEpsilon, HessianEpsilon};
  
    ///<summary> Maximum number of iterations </summary>
    public int maxIteration;
    ///<summary> Current number of iterations </summary>
    public int iterationCounter;
    ///<summary> Maximum number of function evaluations </summary>
    public int maxFunctionEvaluation;
    ///<summary> Current number of function evaluations </summary>
    public int functionEvaluationCounter;
    ///<summary> Maximum number of gradient evaluations </summary>
    public int maxGradientEvaluation;
    ///<summary> Current number of gradient evaluations </summary>
    public int gradientEvaluationCounter;
    ///<summary> Maximum number of hessian evaluations </summary>
    public int maxHessianEvaluation;
    ///<summary> Current number of hessian evaluations </summary>
    public int hessianEvaluationCounter;
    
    ///<summary> Minimum Function Epsilon</summary>
    public double minFunctionEpsilon; 
    ///<summary> Minimum Gradient Epsilon</summary>
    public double minGradientEpsilon;
    ///<summary> Gradient Epsilon </summary>
    public double minHessianEpsilon;
    
    ///<summary> Maximun number of iterations at a stationary point</summary>
    public int maxStationaryPointIterations;
    ///<summary> Current number of iterations at a stationary point</summary>
    public int stationaryPointIterationsCounter;
    ///<summary> Maximun number of iterations at a stationary gradient</summary>
    public int maxStationaryGradientIterations;
    ///<summary> Current number of iterations at a stationary gradient</summary>
    public int stationaryGradientIterationsCounter;
    ///<summary> Maximun number of iterations at a stationary hessian</summary>
    public int maxStationaryHessianIterations;
    ///<summary> Current number of iterations at a stationary hessian</summary>
    public int stationaryHessianIterationsCounter;
    ///<summary> </summary>
    protected bool stationaryCriteria = false;
  
    ///<summary> The criteria that ended optimization</summary>
    protected CriteriaType endCriteria = CriteriaType.None;
    ///<summary>Return the criteria that was satisfied and thus ended optimization</summary>
    public CriteriaType Criteria 
    {
      get 
      {
        return endCriteria;
      }
    }

    ///<summary>Check if the iteration number is less than the maximum iteration</summary>
    ///<remarks>
    ///If iteration count is equal to or greater than the maximum number of iterations then
    ///the ending criteria is set to <c>CriteriaType.MaximumIteration</c> and the function returns true.
    ///</remarks>
    public bool CheckIterations() 
    {
      bool test = (iterationCounter >= maxIteration);
      if (test)
        this.endCriteria = CriteriaType.MaximumIteration;
      return test;
    }
    
    ///<summary>Check if the number of function evaluations is less than the maximum </summary>
    ///<remarks>
    ///If the number of function evaluations is equal to or greater than the maximum number of 
    ///function evaluations then the ending criteria is set to <c>CriteriaType.MaximumFunctionEvaluation</c>
    /// and the function returns true.
    ///</remarks>
    public bool CheckFunctionEvaluations() 
    {
      bool test = (functionEvaluationCounter >= maxFunctionEvaluation);
      if (test)
        this.endCriteria = CriteriaType.MaximumFunctionEvaluation;
      return test;
    }
    
    ///<summary>Check if the number of gradient evaluations is less than the maximum </summary>
    ///<remarks>
    ///If the number of gradient evaluations is equal to or greater than the maximum number of 
    ///gradient evaluations then the ending criteria is set to <c>CriteriaType.MaximumGradientEvaluation</c>
    /// and the function returns true.
    ///</remarks>
    public bool CheckGradientEvaluations() 
    {
      bool test = (gradientEvaluationCounter >= maxGradientEvaluation);
      if (test)
        this.endCriteria = CriteriaType.MaximumGradientEvaluation;
      return test;
    }
    
    ///<summary>Check if the number of hessian evaluations is less than the maximum </summary>
    ///<remarks>
    ///If the number of hessian evaluations is equal to or greater than the maximum number of 
    ///hessian evaluations then the ending criteria is set to <c>CriteriaType.MaximumHessianEvaluation</c>
    /// and the function returns true.
    ///</remarks>
    public bool CheckHessianEvaluations() 
    {
      bool test = (hessianEvaluationCounter >= maxHessianEvaluation);
      if (test)
        this.endCriteria = CriteriaType.MaximumHessianEvaluation;
      return test;
    }

    ///<summary>Check if objective function changed by less than the function epsilon</summary>
    ///<remarks>
    /// If the change in objective function is less than the function epsilon then a possible stationary
    /// point has been found.  If the number of repeated iterations at this possible stationary point is
    /// greater than the maximum iterations at a station point then the ending criteria is set to
    /// <c>CriteriaType.StationaryPoint</c> and the function returns true;
    ///</remarks>
    public bool CheckStationaryPoint(double fold, double fnew) 
    {
      bool test = (System.Math.Abs(fold - fnew) < minFunctionEpsilon);
      if (test) 
        stationaryPointIterationsCounter++;
      else if (stationaryPointIterationsCounter != 0)
        stationaryPointIterationsCounter = 0;
      if (stationaryPointIterationsCounter > maxStationaryPointIterations)
        endCriteria = CriteriaType.StationaryPoint;
      
      return (test && (stationaryPointIterationsCounter > maxStationaryPointIterations));
    }
    
    ///<summary>Check if gradient function changed by less than the gradient epsilon</summary>
    ///<remarks>
    /// If the change in gradient function is less than the gradient epsilon then a possible stationary
    /// point has been found.  If the number of repeated iterations at this possible stationary point is
    /// greater than the maximum iterations at a station point then the ending criteria is set to
    /// <c>CriteriaType.StationaryPoint</c> and the function returns true;
    ///</remarks>
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
    
    ///<summary>Check if hessian function changed by less than the hessian epsilon</summary>
    ///<remarks>
    /// If the change in hessian function is less than the hessian epsilon then a possible stationary
    /// point has been found.  If the number of repeated iterations at this possible stationary point is
    /// greater than the maximum iterations at a station point then the ending criteria is set to
    /// <c>CriteriaType.StationaryPoint</c> and the function returns true;
    ///</remarks>
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

    
    ///<summary>Check if objective function value is less than the function epsilon</summary>
    ///<remarks>
    /// If the objective function value is less than the function epsilon and only positive optimization
    /// is allowed then the ending criteria is set to <c>CriteriaType.FunctionEpsilon</c> and the 
    /// function returns true;
    ///</remarks>
    public bool CheckFunctionEpsilon(double f) 
    {
      bool test = (f < minFunctionEpsilon);
      if (test)
        endCriteria = CriteriaType.FunctionEpsilon;
      return test;
    }

    ///<summary>Check if the norm of the gradient is less than the gradient epsilon</summary>
    ///<remarks>
    /// If the norm of the gradient is less than the gradient epsilon then the ending criteria is set
    /// to <c>CriteriaType.GradientEpsilon</c> and the function returns true;
    ///</remarks>
    public bool CheckGradientEpsilon (double normDiff) 
    {
      bool test = (normDiff < minGradientEpsilon);
      if (test)
        endCriteria = CriteriaType.GradientEpsilon;
      return test;
    }
    
    ///<summary>Check if the norm of the hessian is less than the hessian epsilon</summary>
    ///<remarks>
    /// If the norm of the hessian is less than the gradient epsilon then the ending criteria is set
    /// to <c>CriteriaType.HessianEpsilon</c> and the function returns true;
    ///</remarks>
    public bool CheckHessianEpsilon (double normDiff) 
    {
      bool test = (normDiff < minHessianEpsilon);
      if (test)
        endCriteria = CriteriaType.HessianEpsilon;
      return test;
    }

    ///<summary>Check if ending criteria are met</summary>
    ///<remarks>Returns true if one of the ending criteria is met, otherwise it returns true</remarks>
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
    
    ///<summary>Check if gradient criteria are met</summary>
    ///<remarks>Returns true if one of the gradient criteria is not met, otherwise it returns true</remarks>
    public bool CheckGradientCriteria(double normgold,double normgnew) 
    {
      return !( 
        CheckGradientEvaluations() ||
        CheckStationaryGradient(normgnew,normgold) ||
        CheckGradientEpsilon(normgnew) ||
        CheckGradientEpsilon(normgold) );
    }   
    
    ///<summary> Reset all Counters </summary>
    public void Reset () 
    {
      iterationCounter = 0;
      functionEvaluationCounter = 0;
      gradientEvaluationCounter = 0;
      hessianEvaluationCounter = 0;
      stationaryPointIterationsCounter = 0;
      stationaryGradientIterationsCounter = 0;
      stationaryHessianIterationsCounter = 0;
      this.endCriteria = CriteriaType.None;
    }
    
    // --- IFormattable Interface ---
    
    ///<summary>A string representation of this <c>EndCriteria</c>.</summary>
    public override string ToString() 
    {
      return ToString(null,null);
    }

    ///<summary>A string representation of this <c>EndCriteria</c>.</summary>
    ///<param name="format">A format specification.</param>
    public string ToString(string format) 
    {
      return ToString(format, null);
    }

    ///<summary>A string representation of this <c>EndCriteria</c>.</summary>
    ///<param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
    public string ToString(IFormatProvider formatProvider) 
    {
      return ToString(null,formatProvider);
    }

    ///<summary>A string representation of this <c>EndCriteria</c>.</summary>
    ///<param name="format">A format specification.</param>
    ///<param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
    public string ToString(string format, IFormatProvider formatProvider)
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
