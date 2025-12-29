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
 * SecantLineSearch.cs
 *
 * Copyright (c) 2004, dnAnalytics Project. All rights reserved.
 * NB: Problem class inspired by the optimization frame in the QuantLib library
*/

using System;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Optimization
{
  /// <summary>Secant line search method.</summary>
  /// <remarks>
  /// <para>Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved. See <a>http://www.dnAnalytics.net</a> for details.</para>
  /// <para>Adopted for Altaxo (c) 2005 Dr. Dirk Lellinger.</para>
  /// </remarks>
  public class SecantLineSearch : LineSearchMethod
  {
    /// <summary>Initializes a new instance of the <see cref="SecantLineSearch"/> class.</summary>
    /// <param name="costfunction">Nonlinear cost function to minimize.</param>
    public SecantLineSearch(CostFunction costfunction)
      : this(costfunction, new EndCriteria()) { }

    /// <summary>Initializes a new instance of the <see cref="SecantLineSearch"/> class.</summary>
    /// <param name="costfunction">Nonlinear cost function to minimize.</param>
    /// <param name="endcriteria">User-specified ending criteria.</param>
    public SecantLineSearch(CostFunction costfunction, EndCriteria endcriteria)
      : this(costfunction, endcriteria, 1) { }

    /// <summary>Initializes a new instance of the <see cref="SecantLineSearch"/> class.</summary>
    /// <param name="costfunction">Nonlinear cost function to minimize.</param>
    /// <param name="endcriteria">User-specified ending criteria.</param>
    /// <param name="sigma_0">Initial step size used for the first evaluation of each secant minimization.</param>
    public SecantLineSearch(CostFunction costfunction, EndCriteria endcriteria, double sigma_0)
      : this(costfunction, endcriteria, sigma_0, 50, 1e-8) { }

    /// <summary>Initializes a new instance of the <see cref="SecantLineSearch"/> class.</summary>
    /// <param name="costfunction">Nonlinear cost function to minimize.</param>
    /// <param name="endcriteria">User-specified ending criteria.</param>
    /// <param name="sigma_0">Initial step size used for the first evaluation of each secant minimization.</param>
    /// <param name="maxiteration">Maximum number of iterations.</param>
    /// <param name="tolerance">Termination tolerance (based on the step size along the search direction).</param>
    public SecantLineSearch(CostFunction costfunction, EndCriteria endcriteria, double sigma_0,
      int maxiteration, double tolerance)
    {
      costFunction_ = costfunction;
      endCriteria_ = endcriteria;
      this.sigma_0 = sigma_0;
      maxIteration = maxiteration;
      this.tolerance = tolerance;
    }

    /// <summary>Initial step size used for the first evaluation of each secant minimization.</summary>
    public double sigma_0;

    /// <summary>Maximum number of iterations.</summary>
    private int maxIteration;

    /// <summary>Termination tolerance.</summary>
    private double tolerance;

    /// <inheritdoc/>
    public override string MethodName
    {
      get { return "Secant Line Search Method"; }
    }

    /// <inheritdoc/>
    public override Vector<double> Search(Vector<double> x, Vector<double> d, double step)
    {
      var ret = x.Clone();
      double j = 0;
      double eta;

      double delta_d = d.DotProduct(d);
      double alpha = -sigma_0;
      double eta_prev = d.DotProduct(GradientEvaluation(ret + sigma_0 * d));
      do
      {
        eta = d.DotProduct(GradientEvaluation(ret));
        alpha = alpha * (eta / (eta_prev - eta));
        ret = ret + alpha * d;
        eta_prev = eta;
        j++;
      } while ((j < maxIteration) && (alpha * alpha * delta_d > tolerance * tolerance));
      return ret;
    }
  }
}
