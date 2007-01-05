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
 * NelderMead.cs
 * 
 * Copyright (c) 2004, dnAnalytics Project. All rights reserved.
 * NB: Constraint class inspired by the optimization frame in the QuantLib library
*/

using System;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Optimization
{
  ///<summary>Nelder and Mead Simplex Minimization Method</summary>
  /// <remarks>
  /// <para>Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved. See <a>http://www.dnAnalytics.net</a> for details.</para>
  /// <para>Adopted to Altaxo (c) 2005 Dr. Dirk Lellinger.</para>
  /// </remarks>
  public class NelderMead: FunctionMinimizeMethod 
  {
  
    ///<summary>Default constructor for simplex method</summary>
    public NelderMead(ICostFunction costfunction)
      : this(costfunction, new EndCriteria()) {}
    
    public NelderMead(ICostFunction costfunction, EndCriteria endcriteria) 
    {
      this.costFunction_=costfunction;
      this.endCriteria_=endcriteria;
    }
    
    ///<summary> Types of steps the Nelder-Mead Simplex Algorithm can take</summary>
    public enum Step { Initialization, Reflection, Expansion, OutsideContraction, InsideContraction, Shrink };
    
    private Step laststep_ = Step.Initialization;
    
    ///<summary> Return the type of step for the last iteration </summary>
    public Step LastStep 
    {
      get { return laststep_; }
    }
    
    private double zdelta = 0.00025;
    private double delta = 0.05;
    
    ///<summary> Delta used to generate initial simplex for non-zero value elements </summary>
    public double SimplexDelta 
    {
      set { this.delta = value; }
      get { return this.delta; }
    }
    
    private double rho_ = 1;
    private double chi_ = 2;
    private double psi_ = 0.5;
    private double sigma_ = 0.5;
    
    ///<summary> Coefficient of reflection (Rho) </summary>
    public double Rho 
    {
      set { this.rho_ = value; }
      get { return this.rho_; }
    }
    
    ///<summary> Coefficient of expansion (Chi) </summary>
    public double Chi 
    {
      set { this.chi_ = value; }
      get { return this.chi_; }
    }
    
    ///<summary> Coefficient of contraction (Psi) </summary>
    public double Psi 
    {
      set { this.psi_ = value; }
      get { return this.psi_; }
    }
    
    ///<summary> Coefficient of shrinkage (Sigma) </summary>
    public double Sigma 
    {
      set { this.sigma_ = value; }
      get { return this.sigma_; }
    }
    
    ///<summary> Delta used to generate initial simplex for zero value elements </summary>
    public double SimplexZeroDelta 
    {
      set { this.zdelta = value; }
      get { return this.zdelta; }
    }
    
    ///<summary> Return the current iteration Simplex</summary>
    public DoubleVector[] Simplex 
    {
      get { return this.x; }
    }
    
    ///<summary> Create an initial simplex </summary>   
    private DoubleVector[] CreateSimplex(DoubleVector x) 
    {
      int n = x.Length;
      DoubleVector[] simplex = new DoubleVector[n+1];
      DoubleVector direction;
      simplex[0] = new DoubleVector(x);
      for (int i=1; i<=n; i++) 
      {
        simplex[i] = new DoubleVector(x);
        if (x[i-1] != 0) 
        {
          simplex[i][i-1] = (1+delta)*x[i-1];
        } 
        else 
        {
          simplex[i][i-1] = zdelta;
        }
      }
      return simplex;
    }
        
    private void RankVertices() 
    {
      // determine the values of each vertice in the initial simplex
      for (int i=0; i<this.x.Length; i++)
        fx[i] = FunctionEvaluation(this.x[i]);
        
      Array.Sort(fx,x);
    }
    
    private DoubleVector[] x; // Array of Simplexes
    private double[] fx; //Array of simplex function values
    
    /*  Below are overriden Methods */
    
    ///<summary> Method Name </summary>
    public override string MethodName 
    {
      get { return "Nelder-Mead Downhill Simplex Method"; }
    }
    
    ///<summary> Minimize the given cost function </summary>
    public override void Minimize(DoubleVector initialvector) 
    {
      Minimize(CreateSimplex(initialvector));
    }
    
    ///<summary> Minimize the given cost function </summary>
    public void Minimize(DoubleVector[] initialsimplex, double rho, double chi, double psi, double sigma) 
    {
      this.rho_ = rho;
      this.chi_ = chi;
      this.psi_ = psi;
      this.sigma_ = sigma;
      Minimize(initialsimplex);
    }
     
    ///<summary> Minimize the given cost function </summary>
    public void Minimize(DoubleVector[] initialsimplex) 
    {
      endCriteria_.Reset();
      InitializeMethod(initialsimplex);
      // Iterate the optimization method
      do 
      {
        endCriteria_.iterationCounter++;
        IterateMethod();
      } while (endCriteria_.CheckCriteria(iterationValues_[endCriteria_.iterationCounter-1], iterationValues_[endCriteria_.iterationCounter]));
    } 
    
    ///<summary> Initialize the optimization method </summary>
    ///<remarks> The use of this function is intended for testing/debugging purposes only </remarks>
    public override void InitializeMethod(DoubleVector initialvector) 
    {
      InitializeMethod(CreateSimplex(initialvector));
    }
    
    ///<summary> Initialize the optimization method </summary>
    ///<remarks> The use of this function is intended for testing/debugging purposes only </remarks>
    public void InitializeMethod(DoubleVector[] initialsimplex) 
    {
      this.x = new DoubleVector[initialsimplex.Length];
      initialsimplex.CopyTo(this.x,0);
      this.fx = new double[this.x.Length];
      RankVertices();
      this.laststep_ = Step.Initialization;
      this.iterationVectors_ = new DoubleVector[endCriteria_.maxIteration+1];
      this.iterationVectors_[0] = initialsimplex[0];
      this.iterationValues_ = new double[endCriteria_.maxIteration+1];
      this.iterationValues_[0] = FunctionEvaluation(this.iterationVectors_[0]);
      this.iterationGradientNorms_ = new double[endCriteria_.maxIteration+1];
      this.iterationGradientNorms_[0] = 2.0 * System.Math.Abs(fx[x.Length-1] - fx[0]) / 
        (System.Math.Abs(fx[x.Length-1]) + System.Math.Abs(fx[0]) + System.Double.Epsilon);
    }
    
    ///<summary> Perform a single iteration of the optimization method </summary>
    ///<remarks> The use of this function is intended for testing/debugging purposes only </remarks>
    public override void IterateMethod() 
    {
      DoubleVector xr, xe, xbar, xc, xcc;
      double fxr, fxe, fxc, fxcc;
      
      // Calculate centroid of n best points (ie excluding worst point)
      xbar = new DoubleVector(this.x[0].Length,0.0);
      for (int i=0; i<(this.x.Length-1); i++) 
        xbar = xbar + this.x[i];
      xbar = xbar / (this.x.Length-1);
      
      // Calculate reflection point
      xr = (1+rho_)*xbar - rho_*x[x.Length-1];
      fxr = FunctionEvaluation(xr);
      
      if (fxr < fx[x.Length-2]) 
      {
        // reflection point is better than worst point
        if (fxr < fx[0]) 
        {
          //reflection point is better than best point - > expand
          xe = (1+rho_*chi_)*xbar - rho_*chi_*x[x.Length-1];
          fxe = FunctionEvaluation(xe);
          if (fxe<fxr) 
          {
            // accept expansion point
            x[x.Length-1] = xe;
            fx[x.Length-1] = fxe;
            this.laststep_ = Step.Expansion;
          } 
          else 
          {
            // accept reflection point
            x[x.Length-1] = xr;
            fx[x.Length-1] = fxr;
            this.laststep_ = Step.Reflection;
          }
        } 
        else 
        {
          // accept reflection point
          x[x.Length-1] = xr;
          fx[x.Length-1] = fxr;
          this.laststep_ = Step.Reflection;
        }
      } 
      else 
      {
        // Try a contraction
        if ((fx[x.Length-2]<=fxr)  & (fxr< fx[x.Length-1])) 
        {
          // perform an outside contraction
          xc = (1+psi_*rho_)*xbar - psi_*rho_*x[x.Length-1];
          fxc = FunctionEvaluation(xc);
          if (fxc < fxr) 
          {
            // accept the outside contraction
            x[x.Length-1] = xc;
            fx[x.Length-1] = fxc;
            this.laststep_ = Step.OutsideContraction;
          } 
          else 
          {
            // perform a shrink step
            for (int i=1; i<x.Length-1; i++) 
            {
              x[i] = x[0] + sigma_*(x[i]-x[0]);
              fx[i] = FunctionEvaluation(x[i]);
            }
            this.laststep_ = Step.Shrink;
          }
        } 
        else 
        {
          // perform an inside contraction
          xcc = (1-psi_)*xbar + psi_*x[x.Length-1];
          fxcc = FunctionEvaluation(xcc);
          if (fxcc < fx[x.Length-1]) 
          {
            // accept inside contraction
            x[x.Length-1] = xcc;
            fx[x.Length-1] = fxcc;
            this.laststep_ = Step.InsideContraction;
          } 
          else 
          {
            // perform a shrink step
            for (int i=1; i<x.Length-1; i++) 
            {
              x[i] = x[0] + sigma_*(x[i]-x[0]);
              fx[i] = FunctionEvaluation(x[i]);
            }
            this.laststep_ = Step.Shrink;
          }
        }
      }
      RankVertices();
      this.iterationVectors_[endCriteria_.iterationCounter]=this.x[0];
      this.iterationValues_[endCriteria_.iterationCounter]=this.fx[0];
      this.iterationGradientNorms_[endCriteria_.iterationCounter]=2.0 * System.Math.Abs(fx[x.Length-1] - fx[0]) / 
        (System.Math.Abs(fx[x.Length-1]) + System.Math.Abs(fx[0]) + System.Double.Epsilon);
    }
  
  }

}
