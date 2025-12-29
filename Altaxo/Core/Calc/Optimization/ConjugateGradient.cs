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
 * Conjugate Gradient.cs
 *
 * Copyright (c) 2004, dnAnalytics Project. All rights reserved.
 * TODO: Add preconditioning and selection of either Fletcher-Reeves or Polak-Ribiere
*/

using System;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Optimization
{
  /// <summary>Nonlinear preconditioned conjugate gradient method.</summary>
  /// <remarks>
  /// <para>Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved. See <a>http://www.dnAnalytics.net</a> for details.</para>
  /// <para>Adopted for Altaxo (c) 2005 Dr. Dirk Lellinger.</para>
  /// </remarks>
  public class ConjugateGradient : FunctionMinimizeMethod
  {
#nullable disable
    private Vector<double> g;
    //Vector<double> gold;

    private int restartCount = 0;
    private int restartCounter = 0;

    private LineSearchMethod lineSearchMethod_;

    private Vector<double>[] iterationDirections_;
    private double[] iterationTrialSteps_;
    private Vector<double>[] iterationGradients_;

    private double delta_new;
    private double delta_old;
    private double delta_mid;
    private Vector<double> s;

#nullable enable

    /// <summary>
    /// Initializes a new instance of the <see cref="ConjugateGradient"/> class.
    /// </summary>
    /// <param name="costfunction">Nonlinear cost function to minimize.</param>
    /// <remarks>
    /// This class began as a port of CG+ by Guanghui Lui, Jorge Nocedal and Richard Waltz to C#.
    /// </remarks>
    /// <seealso href="http://www.ece.northwestern.edu/~rwaltz/CG+.html">CG+</seealso>
    public ConjugateGradient(CostFunction costfunction)
      : this(costfunction, new EndCriteria()) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConjugateGradient"/> class.
    /// </summary>
    /// <param name="costfunction">Nonlinear cost function to minimize.</param>
    /// <param name="endcriteria">User-specified ending criteria.</param>
    public ConjugateGradient(CostFunction costfunction, EndCriteria endcriteria)
      : this(costfunction, endcriteria, new SecantLineSearch(costfunction, endcriteria)) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConjugateGradient"/> class.
    /// </summary>
    /// <param name="costfunction">Nonlinear cost function to minimize.</param>
    /// <param name="endcriteria">User-specified ending criteria.</param>
    /// <param name="lsm">User-specified line search method; defaults to the secant line search method.</param>
    public ConjugateGradient(CostFunction costfunction, EndCriteria endcriteria, LineSearchMethod lsm)
    {
      costFunction_ = costfunction;
      endCriteria_ = endcriteria;
      lineSearchMethod_ = lsm;
    }

    /// <summary>
    /// Gets or sets the number of iterations between restarts.
    /// </summary>
    /// <remarks>
    /// Must be a non-negative number. If set to <c>0</c>, the number of iterations between restarts is the number of variables.
    /// </remarks>
    public int RestartCount
    {
      get { return restartCount; }
      set
      {
        if (value >= 0)
          restartCount = value;
        else
          throw new OptimizationException("Restart Counter must be a non-negative number");
      }
    }

    /// <inheritdoc/>
    public override string MethodName
    {
      get { return "Conjugate Gradient Method"; }
    }

    /// <inheritdoc/>
    /// <remarks>The use of this function is intended for testing/debugging purposes only.</remarks>
    public override void InitializeMethod(Vector<double> initialvector)
    {
      g = GradientEvaluation(initialvector);

      // Calculate Diagonal preconditioner
      var h = HessianEvaluation(initialvector);
      var m_inv = CreateMatrix.Dense<double>(initialvector.Count, initialvector.Count);
      for (int i = 0; i < initialvector.Count; i++)
        m_inv[i, i] = 1 / h[i, i];
      s = m_inv * g;

      Vector<double> d = -s;

      delta_new = g.DotProduct(d);

      restartCounter = 0;
      /* ------------------------------ */
      iterationVectors_ = new Vector<double>[endCriteria_.maxIteration + 1];
      iterationVectors_[0] = initialvector;

      iterationValues_ = new double[endCriteria_.maxIteration + 1];
      iterationValues_[0] = FunctionEvaluation(iterationVectors_[0]);

      iterationGradients_ = new Vector<double>[endCriteria_.maxIteration + 1];
      iterationGradients_[0] = g.Clone();

      iterationGradientNorms_ = new double[endCriteria_.maxIteration + 1];
      iterationGradientNorms_[0] = g.L2Norm();

      iterationDirections_ = new Vector<double>[endCriteria_.maxIteration + 1];
      iterationDirections_[0] = d;

      iterationTrialSteps_ = new double[endCriteria_.maxIteration + 1];
      iterationTrialSteps_[0] = 1 / iterationGradientNorms_[0];
    }

    /// <inheritdoc/>
    /// <remarks>The use of this function is intended for testing/debugging purposes only.</remarks>
    public override void IterateMethod()
    {
      Vector<double> d = iterationDirections_[endCriteria_.iterationCounter - 1];
      Vector<double> x = iterationVectors_[endCriteria_.iterationCounter - 1];
      Vector<double> g = iterationGradients_[endCriteria_.iterationCounter - 1];
      double stp = iterationTrialSteps_[endCriteria_.iterationCounter - 1];

      // Shanno-Phua's Formula for Trial Step
      if (restartCounter == 0 && endCriteria_.iterationCounter > 1)
      {
        double dg = d.DotProduct(g);
        double dg0 = iterationDirections_[endCriteria_.iterationCounter - 2].DotProduct(
          iterationGradients_[endCriteria_.iterationCounter - 2]) / stp;
        stp = dg0 / dg;
      }

      delta_mid = g.DotProduct(g);

      // Conduct line search
      x = lineSearchMethod_.Search(x, d, stp);
      g = GradientEvaluation(x);

      delta_old = delta_new;
      delta_mid = g.DotProduct(s);

      // Calculate Diagonal preconditioner
      var h = HessianEvaluation(x);
      var m_inv = CreateMatrix.Dense<double>(x.Count, x.Count);
      for (int i = 0; i < x.Count; i++)
        m_inv[i, i] = 1 / h[i, i];
      s = m_inv * g;

      // Calculate Beta
      delta_new = g.DotProduct(s);
      double beta = (delta_new - delta_mid) / delta_old;

      // Check for restart conditions
      restartCounter++;
      if (restartCounter == restartCount || (restartCounter == x.Count && restartCount == 0) || beta <= 0)
      {
        restartCount = 0;
        beta = 0;
      }

      // Calculate next line search direction
      d = -s + beta * d;

      iterationVectors_[endCriteria_.iterationCounter] = x;
      iterationValues_[endCriteria_.iterationCounter] = FunctionEvaluation(x);
      iterationGradients_[endCriteria_.iterationCounter] = g;
      iterationGradientNorms_[endCriteria_.iterationCounter] = g.L2Norm();
      iterationDirections_[endCriteria_.iterationCounter] = d;
      iterationTrialSteps_[endCriteria_.iterationCounter] = stp;
    }
  }
}
