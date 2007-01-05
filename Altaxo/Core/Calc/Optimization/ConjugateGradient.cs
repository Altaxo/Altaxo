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
 * Conjugate Gradient.cs
 * 
 * Copyright (c) 2004, dnAnalytics Project. All rights reserved.
 * TODO: Add preconditioning and selection of either Fletcher-Reeves or Polak-Ribiere
*/

using System;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Optimization 
{
  ///<summary>Nonlinear Preconditioned Conjugate Gradient Method</summary>
  /// <remarks>
  /// <para>Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved. See <a>http://www.dnAnalytics.net</a> for details.</para>
  /// <para>Adopted to Altaxo (c) 2005 Dr. Dirk Lellinger.</para>
  /// </remarks>
  public class ConjugateGradient: FunctionMinimizeMethod 
  {
    ///<summary>Constructor for Conjugate Gradient Method. The constructor specifies the 
    /// costfunction and optionally user specified ending criteria and line search methods.</summary>
    ///<param name="costfunction">Nonlinear cost function to minimize.</param>
    ///<remarks>This class began as a port of CG+ by Guanghui Lui, Jorge Nocedal and Richard Waltz
    /// to C#</remarks>
    ///<seealso href="http://www.ece.northwestern.edu/~rwaltz/CG+.html">CG+</seealso>
    public ConjugateGradient(CostFunction costfunction)
      : this(costfunction, new EndCriteria()) {}
    ///<param name="endcriteria">User specified ending criteria.</param>
    public ConjugateGradient(CostFunction costfunction, EndCriteria endcriteria)
      : this(costfunction, endcriteria, new SecantLineSearch(costfunction, endcriteria)) {}
    ///<param name="lsm">User specified line search method, defaults to Secant line search method</param>
    public ConjugateGradient(CostFunction costfunction, EndCriteria endcriteria, LineSearchMethod lsm) 
    {
      this.costFunction_=costfunction;
      this.endCriteria_=endcriteria;
      this.lineSearchMethod_=lsm;
    }
    
    ///<summary>Number of iterations between restarts.  Must be a non-negative number.  If 0 is 
    /// specified then the number of iterations between restart is the number of variables </summary>
    public int RestartCount 
    {
      get { return this.restartCount; }
      set
      { 
        if (value>=0)
          this.restartCount = value; 
        else
          throw new OptimizationException("Restart Counter must be a non-negative number");
      }
    }
    
    DoubleVector g;
    //DoubleVector gold;
    
    private int restartCount=0;
    private int restartCounter=0;
    
    private LineSearchMethod lineSearchMethod_;
  
    ///<summary> Method Name </summary>
    public override string MethodName 
    {
      get { return "Conjugate Gradient Method"; }
    }
    
    private DoubleVector[] iterationDirections_;
    private double[] iterationTrialSteps_;
    private DoubleVector[] iterationGradients_;
    
    private double delta_new;
    private double delta_old;
    private double delta_mid;
    private DoubleVector s;
    
    ///<summary> Initialize the optimization method </summary>
    ///<remarks> The use of this function is intended for testing/debugging purposes only </remarks>
    public override void InitializeMethod(DoubleVector initialvector) 
    {
      g = GradientEvaluation(initialvector);
            
      // Calculate Diagonal preconditioner
      DoubleMatrix h = HessianEvaluation(initialvector);
      DoubleMatrix m_inv = new DoubleMatrix(initialvector.Length,initialvector.Length);
      for (int i=0; i<initialvector.Length; i++)
        m_inv[i,i] = 1/h[i,i];
      s = m_inv*g;
      
      DoubleVector d = -s;
      
      delta_new = g.GetDotProduct(d);
      
      restartCounter=0;
      /* ------------------------------ */
      this.iterationVectors_ = new DoubleVector[endCriteria_.maxIteration+1];
      this.iterationVectors_[0] = initialvector;
      
      this.iterationValues_ = new double[endCriteria_.maxIteration+1];
      this.iterationValues_[0] = FunctionEvaluation(this.iterationVectors_[0]);
      
      this.iterationGradients_ = new DoubleVector[endCriteria_.maxIteration+1];
      this.iterationGradients_[0] = new DoubleVector(g);
      
      this.iterationGradientNorms_ = new double[endCriteria_.maxIteration+1];
      this.iterationGradientNorms_[0] = g.GetNorm();
      
      this.iterationDirections_ = new DoubleVector[endCriteria_.maxIteration+1];
      this.iterationDirections_[0] = d;
      
      this.iterationTrialSteps_ = new double[endCriteria_.maxIteration+1];
      this.iterationTrialSteps_[0] = 1/this.iterationGradientNorms_[0];
    }
    
    ///<summary> Perform a single iteration of the optimization method </summary>
    ///<remarks> The use of this function is intended for testing/debugging purposes only </remarks>
    public override void IterateMethod() 
    {
    
      DoubleVector d = this.iterationDirections_[endCriteria_.iterationCounter-1];
      DoubleVector x = this.iterationVectors_[endCriteria_.iterationCounter-1];
      DoubleVector g = this.iterationGradients_[endCriteria_.iterationCounter-1];
      double stp = this.iterationTrialSteps_[endCriteria_.iterationCounter-1];
      
      // Shanno-Phua's Formula for Trial Step
      if (restartCounter==0 && endCriteria_.iterationCounter>1) 
      {
        double dg = d.GetDotProduct(g);
        double dg0 = this.iterationDirections_[endCriteria_.iterationCounter-2].GetDotProduct(
          this.iterationGradients_[endCriteria_.iterationCounter-2])/stp;
        stp = dg0/dg;
      }
      
      delta_mid = g.GetDotProduct(g);
      
      // Conduct line search
      x = lineSearchMethod_.Search(x,d,stp);
      g = GradientEvaluation(x);
      
      delta_old = delta_new;
      delta_mid = g.GetDotProduct(s);
      
      // Calculate Diagonal preconditioner
      DoubleMatrix h = HessianEvaluation(x);
      DoubleMatrix m_inv = new DoubleMatrix(x.Length,x.Length);
      for (int i=0; i<x.Length; i++)
        m_inv[i,i] = 1/h[i,i];
      s = m_inv*g;
  
      // Calculate Beta
      delta_new = g.GetDotProduct(s);
      double beta = (delta_new-delta_mid)/delta_old;

      // Check for restart conditions
      restartCounter++;
      if (restartCounter==restartCount || (restartCounter==x.Length && restartCount==0) || beta<=0) 
      {
        restartCount = 0;
        beta=0;
      }
      
      // Calculate next line search direction
      d = -s + beta*d;
      
      this.iterationVectors_[endCriteria_.iterationCounter] = x;
      this.iterationValues_[endCriteria_.iterationCounter] = FunctionEvaluation(x);
      this.iterationGradients_[endCriteria_.iterationCounter] = g;
      this.iterationGradientNorms_[endCriteria_.iterationCounter] = g.GetNorm();
      this.iterationDirections_[endCriteria_.iterationCounter] = d;
      this.iterationTrialSteps_[endCriteria_.iterationCounter] = stp;
    }
  }
}
