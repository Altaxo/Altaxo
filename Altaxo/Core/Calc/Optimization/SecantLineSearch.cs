#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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
 * SecantLineSearch.cs
 * 
 * Copyright (c) 2004, dnAnalytics Project. All rights reserved.
 * NB: Problem class inspired by the optimization frame in the QuantLib library
*/

using System;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Optimization
{
  ///<summary>Secant Line Search Method</summary>
  /// <remarks>
  /// <para>Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved. See <a>http://www.dnAnalytics.net</a> for details.</para>
  /// <para>Adopted to Altaxo (c) 2005 Dr. Dirk Lellinger.</para>
  /// </remarks>
  public class SecantLineSearch : LineSearchMethod 
  {
  
    ///<summary>Constructor for Secant Line Search</summary>
    public SecantLineSearch(CostFunction costfunction)
      : this(costfunction, new EndCriteria()) {}
    public SecantLineSearch(CostFunction costfunction, EndCriteria endcriteria) 
      : this(costfunction, endcriteria, 1) {}
    public SecantLineSearch(CostFunction costfunction, EndCriteria endcriteria, double sigma_0)
      : this(costfunction, endcriteria, sigma_0, 50, 1e-8) {}
    public SecantLineSearch(CostFunction costfunction, EndCriteria endcriteria, double sigma_0, 
      int maxiteration, double tolerance) 
    {
      this.costFunction_=costfunction;
      this.endCriteria_=endcriteria;
      this.sigma_0 = sigma_0;
      this.maxIteration=maxiteration;
      this.tolerance=tolerance;
    }
  
    ///<summary>Value of sigma for first step of each Secant method minimization</summary>
    public double sigma_0;
    
    private int maxIteration;
    private double tolerance;
  
    ///<summary> Method Name </summary>
    public override string MethodName 
    {
      get { return "Secant Line Search Method"; }
    }
    
    ///<summary> Minimize the given cost function </summary>
    public override DoubleVector Search(DoubleVector x, DoubleVector d, double step) 
    {
      DoubleVector ret = new DoubleVector(x);
      double j=0;
      double eta;
    
      double delta_d = d.GetDotProduct(d);
      double alpha = -sigma_0;
      double eta_prev = d.GetDotProduct(GradientEvaluation(ret+sigma_0*d));
      do 
      {
        eta = d.GetDotProduct(GradientEvaluation(ret));
        alpha = alpha*(eta/(eta_prev-eta));
        ret = ret + alpha*d;
        eta_prev = eta;
        j++;
      } while ((j<maxIteration) && (alpha*alpha*delta_d > tolerance*tolerance));
      return ret;
    }
  }
}
