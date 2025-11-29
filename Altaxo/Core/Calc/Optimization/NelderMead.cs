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
 * NelderMead.cs
 *
 * Copyright (c) 2004, dnAnalytics Project. All rights reserved.
 * NB: Constraint class inspired by the optimization frame in the QuantLib library
*/

using System;
using System.Threading;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Optimization
{
  ///<summary>Nelder and Mead Simplex Minimization Method</summary>
  /// <remarks>
  /// <para>Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved. See <a>http://www.dnAnalytics.net</a> for details.</para>
  /// <para>Adopted to Altaxo (c) 2005 Dr. Dirk Lellinger.</para>
  /// </remarks>
  public class NelderMead : FunctionMinimizeMethod
  {
#nullable enable
    private Vector<double>[] x; // Array of Simplexes
    private double[] fx; //Array of simplex function values
#nullable disable

    ///<summary>Default constructor for simplex method</summary>
    /// <param name="costFunction">The cost function to minimize. Argument is a vector of parameters. The return value
    /// is the penalty value that is about to be minimized.</param>
    public NelderMead(Func<Vector<double>, double> costFunction)
      : this(new CostFunctionMockup(costFunction), new EndCriteria())
    {
    }

    ///<summary>Default constructor for simplex method</summary>
    public NelderMead(ICostFunction costfunction)
      : this(costfunction, new EndCriteria()) { }

    public NelderMead(ICostFunction costfunction, EndCriteria endcriteria)
    {
      costFunction_ = costfunction;
      endCriteria_ = endcriteria;
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
      set { delta = value; }
      get { return delta; }
    }

    private double rho_ = 1;
    private double chi_ = 2;
    private double psi_ = 0.5;
    private double sigma_ = 0.5;

    ///<summary> Coefficient of reflection (Rho) </summary>
    public double Rho
    {
      set { rho_ = value; }
      get { return rho_; }
    }

    ///<summary> Coefficient of expansion (Chi) </summary>
    public double Chi
    {
      set { chi_ = value; }
      get { return chi_; }
    }

    ///<summary> Coefficient of contraction (Psi) </summary>
    public double Psi
    {
      set { psi_ = value; }
      get { return psi_; }
    }

    ///<summary> Coefficient of shrinkage (Sigma) </summary>
    public double Sigma
    {
      set { sigma_ = value; }
      get { return sigma_; }
    }

    ///<summary> Delta used to generate initial simplex for zero value elements </summary>
    public double SimplexZeroDelta
    {
      set { zdelta = value; }
      get { return zdelta; }
    }

    ///<summary> Return the current iteration Simplex</summary>
    public Vector<double>[] Simplex
    {
      get { return x; }
    }

    ///<summary> Create an initial simplex </summary>
    private Vector<double>[] CreateSimplex(Vector<double> x)
    {
      int n = x.Count;
      var simplex = new Vector<double>[n + 1];
      //DoubleVector direction;
      simplex[0] = x.Clone();
      for (int i = 1; i <= n; i++)
      {
        simplex[i] = x.Clone();
        if (x[i - 1] != 0)
        {
          simplex[i][i - 1] = (1 + delta) * x[i - 1];
        }
        else
        {
          simplex[i][i - 1] = zdelta;
        }
      }
      return simplex;
    }

    private void RankVertices()
    {
      // determine the values of each vertice in the initial simplex
      for (int i = 0; i < x.Length; i++)
        fx[i] = FunctionEvaluation(x[i]);

      Array.Sort(fx, x);
    }


    /*  Below are overriden Methods */

    ///<summary> Method Name </summary>
    public override string MethodName
    {
      get { return "Nelder-Mead Downhill Simplex Method"; }
    }

    ///<summary> Minimize the given cost function </summary>
    public override void Minimize(Vector<double> initialvector)
    {
      Minimize(CreateSimplex(initialvector));
    }

    public void Minimize(Vector<double> initialvector, CancellationToken cancellationToken, Action<double> newMinimalValueFound)
    {
      Minimize(CreateSimplex(initialvector), cancellationToken, newMinimalValueFound);
    }

    ///<summary> Minimize the given cost function </summary>
    public void Minimize(Vector<double>[] initialsimplex, double rho, double chi, double psi, double sigma)
    {
      rho_ = rho;
      chi_ = chi;
      psi_ = psi;
      sigma_ = sigma;
      Minimize(initialsimplex);
    }

    ///<summary> Minimize the given cost function </summary>
    public void Minimize(Vector<double>[] initialsimplex)
    {
      Minimize(initialsimplex, CancellationToken.None, null);
    }

    ///<summary> Minimize the given cost function </summary>
    public void Minimize(Vector<double>[] initialsimplex, CancellationToken cancellationToken, Action<double> newMinimalValueFound)
    {
      endCriteria_.Reset();
      InitializeMethod(initialsimplex);

      var minCostSoFar = fx[0];

      // Iterate the optimization method
      do
      {
        endCriteria_.iterationCounter++;
        IterateMethod();

        if (fx[0] < minCostSoFar)
        {
          minCostSoFar = fx[0];
          newMinimalValueFound?.Invoke(minCostSoFar);
        }
      } while (!cancellationToken.IsCancellationRequested && !double.IsNaN(fx[0]) && endCriteria_.CheckCriteria(iterationValues_[endCriteria_.iterationCounter - 1], iterationValues_[endCriteria_.iterationCounter]));
    }

    ///<summary> Initialize the optimization method </summary>
    ///<remarks> The use of this function is intended for testing/debugging purposes only </remarks>
    public override void InitializeMethod(Vector<double> initialvector)
    {
      InitializeMethod(CreateSimplex(initialvector));
    }

    ///<summary> Initialize the optimization method </summary>
    ///<remarks> The use of this function is intended for testing/debugging purposes only </remarks>
    public void InitializeMethod(Vector<double>[] initialsimplex)
    {
      x = new Vector<double>[initialsimplex.Length];
      initialsimplex.CopyTo(x, 0);
      fx = new double[x.Length];
      RankVertices();
      laststep_ = Step.Initialization;
      iterationVectors_ = new Vector<double>[endCriteria_.maxIteration + 1];
      iterationVectors_[0] = initialsimplex[0];
      iterationValues_ = new double[endCriteria_.maxIteration + 1];
      iterationValues_[0] = FunctionEvaluation(iterationVectors_[0]);
      iterationGradientNorms_ = new double[endCriteria_.maxIteration + 1];
      iterationGradientNorms_[0] = 2.0 * System.Math.Abs(fx[x.Length - 1] - fx[0]) /
        (System.Math.Abs(fx[x.Length - 1]) + System.Math.Abs(fx[0]) + double.Epsilon);
    }

    ///<summary> Perform a single iteration of the optimization method </summary>
    ///<remarks> The use of this function is intended for testing/debugging purposes only </remarks>
    public override void IterateMethod()
    {
      Vector<double> xr, xe, xbar, xc, xcc;
      double fxr, fxe, fxc, fxcc;

      // Calculate centroid of n best points (ie excluding worst point)
      xbar = CreateVector.Dense<double>(x[0].Count);
      for (int i = 0; i < (x.Length - 1); i++)
        xbar = xbar + x[i];
      xbar = xbar / (x.Length - 1);

      // Calculate reflection point
      xr = (1 + rho_) * xbar - rho_ * x[x.Length - 1];
      fxr = FunctionEvaluation(xr);

      if (fxr < fx[x.Length - 2])
      {
        // reflection point is better than worst point
        if (fxr < fx[0])
        {
          //reflection point is better than best point - > expand
          xe = (1 + rho_ * chi_) * xbar - rho_ * chi_ * x[x.Length - 1];
          fxe = FunctionEvaluation(xe);
          if (fxe < fxr)
          {
            // accept expansion point
            x[x.Length - 1] = xe;
            fx[x.Length - 1] = fxe;
            laststep_ = Step.Expansion;
          }
          else
          {
            // accept reflection point
            x[x.Length - 1] = xr;
            fx[x.Length - 1] = fxr;
            laststep_ = Step.Reflection;
          }
        }
        else
        {
          // accept reflection point
          x[x.Length - 1] = xr;
          fx[x.Length - 1] = fxr;
          laststep_ = Step.Reflection;
        }
      }
      else
      {
        // Try a contraction
        if ((fx[x.Length - 2] <= fxr) & (fxr < fx[x.Length - 1]))
        {
          // perform an outside contraction
          xc = (1 + psi_ * rho_) * xbar - psi_ * rho_ * x[x.Length - 1];
          fxc = FunctionEvaluation(xc);
          if (fxc < fxr)
          {
            // accept the outside contraction
            x[x.Length - 1] = xc;
            fx[x.Length - 1] = fxc;
            laststep_ = Step.OutsideContraction;
          }
          else
          {
            // perform a shrink step
            for (int i = 1; i < x.Length - 1; i++)
            {
              x[i] = x[0] + sigma_ * (x[i] - x[0]);
              fx[i] = FunctionEvaluation(x[i]);
            }
            laststep_ = Step.Shrink;
          }
        }
        else
        {
          // perform an inside contraction
          xcc = (1 - psi_) * xbar + psi_ * x[x.Length - 1];
          fxcc = FunctionEvaluation(xcc);
          if (fxcc < fx[x.Length - 1])
          {
            // accept inside contraction
            x[x.Length - 1] = xcc;
            fx[x.Length - 1] = fxcc;
            laststep_ = Step.InsideContraction;
          }
          else
          {
            // perform a shrink step
            for (int i = 1; i < x.Length - 1; i++)
            {
              x[i] = x[0] + sigma_ * (x[i] - x[0]);
              fx[i] = FunctionEvaluation(x[i]);
            }
            laststep_ = Step.Shrink;
          }
        }
      }
      RankVertices();
      iterationVectors_[endCriteria_.iterationCounter] = x[0];
      iterationValues_[endCriteria_.iterationCounter] = fx[0];
      iterationGradientNorms_[endCriteria_.iterationCounter] = 2.0 * System.Math.Abs(fx[x.Length - 1] - fx[0]) /
        (System.Math.Abs(fx[x.Length - 1]) + System.Math.Abs(fx[0]) + double.Epsilon);
    }

    private class CostFunctionMockup : ICostFunction
    {
      public Func<Vector<double>, double> CostFunction;

      public CostFunctionMockup(Func<Vector<double>, double> costFunction)
      {
        CostFunction = costFunction;
      }

      public double Value(Altaxo.Calc.LinearAlgebra.Vector<double> v)
      {
        return CostFunction(v);
      }


      ///<summary>Method to override to calculate the grad_f, the first derivative of
      /// the cost function with respect to x</summary>
      public Altaxo.Calc.LinearAlgebra.Vector<double> Gradient(Altaxo.Calc.LinearAlgebra.Vector<double> x)
      {
        throw new NotImplementedException();
      }

      ///<summary>Method to override to calculate the hessian, the second derivative of
      /// the cost function with respect to x</summary>
      public Matrix<double> Hessian(Altaxo.Calc.LinearAlgebra.Vector<double> x)
      {
        throw new NotImplementedException();
      }

      ///<summary>Access the constraints for the given cost function </summary>
      public Altaxo.Calc.Optimization.ConstraintDefinition Constraint
      {
        get;
        set;
      }
    }
  }
}
