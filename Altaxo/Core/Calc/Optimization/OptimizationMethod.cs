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
 * OptimizationMethod.cs
 * 
 * Copyright (c) 2004, dnAnalytics Project. All rights reserved.
 * NB: Constraint class inspired by the optimization frame in the QuantLib library
*/

using System;
using Altaxo.Calc.LinearAlgebra;
using System.Text;

namespace Altaxo.Calc.Optimization
{
  ///<summary>Base Class for Optimization Methods</summary>
  /// <remarks>
  /// <para>Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved. See <a>http://www.dnAnalytics.net</a> for details.</para>
  /// <para>Adopted to Altaxo (c) 2005 Dr. Dirk Lellinger.</para>
  /// </remarks>
  public abstract class OptimizationMethod : IFormattable 
  {
    
    ///<summary> Optimization ending critera </summary>
    public EndCriteria endCriteria_;
    ///<summary> Optimization Cost Function </summary>
    public ICostFunction costFunction_;
    
    ///<summary> Method Name </summary>
    public virtual string MethodName 
    {
      get { return "Unspecified Optimization Method"; }
    }
    
    ///<summary> Perform an evaluation of Cost Function value </summary>
    protected double FunctionEvaluation(DoubleVector x) 
    {
      this.endCriteria_.functionEvaluationCounter++;
      return costFunction_.Value(x);
    }
    
    ///<summary> Perform an evaluation of Cost Function gradient </summary>
    protected DoubleVector GradientEvaluation(DoubleVector x) 
    {
      this.endCriteria_.gradientEvaluationCounter++;
      return costFunction_.Gradient(x);
    }
    
    ///<summary> Perform an evaluation of Cost Function hessian </summary>
    protected DoubleMatrix HessianEvaluation(DoubleVector x) 
    {
      this.endCriteria_.hessianEvaluationCounter++;
      return costFunction_.Hessian(x);
    }
    
    // --- IFormattable Interface ---
    
    ///<summary>A string representation of this <c>OptimizationMethod</c>.</summary>
    public override string ToString() 
    {
      return ToString(null,null);
    }

    ///<summary>A string representation of this <c>OptimizationMethod</c>.</summary>
    ///<param name="format">A format specification.</param>
    public string ToString(string format) 
    {
      return ToString(format, null);
    }

    ///<summary>A string representation of this <c>OptimizationMethod</c>.</summary>
    ///<param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
    public string ToString(IFormatProvider formatProvider) 
    {
      return ToString(null,formatProvider);
    }

    ///<summary>A string representation of this <c>OptimizationMethod</c>.</summary>
    ///<param name="format">A format specification.</param>
    ///<param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
    public string ToString(string format, IFormatProvider formatProvider)
    {
      return MethodName;
      /*
      StringBuilder sb = new StringBuilder();
      sb.Append("Current function value: " + this.iterationValues_[endCriteria_.iterationCounter].ToString() + "\n");
      sb.Append("Iterations: " + endCriteria_.iterationCounter.ToString() + "\n");
      sb.Append("Function evaluations: " + endCriteria_.functionEvaluationCounter.ToString() + "\n");
      sb.Append("Optimization criteria satisfied: " + endCriteria_.ToString());
      return sb.ToString(); */
    }
  }

}
