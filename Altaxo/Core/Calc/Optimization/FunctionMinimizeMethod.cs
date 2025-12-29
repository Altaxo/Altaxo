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
 * MinimizationMethod.cs
 *
 * Copyright (c) 2004, dnAnalytics Project. All rights reserved.
 * NB: Constraint class inspired by the optimization frame in the QuantLib library
*/

using System;
using System.Text;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Optimization
{
  /// <summary>Base class for function-minimization optimization methods.</summary>
  /// <remarks>
  /// <para>Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved. See <a>http://www.dnAnalytics.net</a> for details.</para>
  /// <para>Adopted for Altaxo (c) 2005 Dr. Dirk Lellinger.</para>
  /// </remarks>
  public abstract class FunctionMinimizeMethod : OptimizationMethod
  {
#nullable disable
    /// <summary>Iteration solutions.</summary>
    protected Vector<double>[] iterationVectors_;

    /// <summary>Iteration values.</summary>
    protected double[] iterationValues_;

    /// <summary>Iteration gradient norms.</summary>
    protected double[] iterationGradientNorms_;

#nullable enable

    /// <summary>Gets the solution vectors for all iterations.</summary>
    public Vector<double>[] IterationVectors
    {
      get { return iterationVectors_; }
    }

    /// <summary>Gets the initial vector.</summary>
    public Vector<double> InitialVector
    {
      get { return iterationVectors_[0]; }
    }

    /// <summary>Gets the solution vector (the current best estimate of the minimum).</summary>
    public Vector<double> SolutionVector
    {
      get { return iterationVectors_[endCriteria_.iterationCounter]; }
    }

    /// <summary>Gets the solution function values for all iterations.</summary>
    public double[] IterationValues
    {
      get { return iterationValues_; }
    }

    /// <summary>Gets the initial value.</summary>
    public double InitialValue
    {
      get { return iterationValues_[0]; }
    }

    /// <summary>Gets the value of the solution vector.</summary>
    public double SolutionValue
    {
      get { return iterationValues_[endCriteria_.iterationCounter]; }
    }

    /// <summary>Gets the gradient norms for all iterations.</summary>
    public double[] IterationGradientNorms
    {
      get { return iterationGradientNorms_; }
    }

    /// <summary>Initializes the optimization method.</summary>
    /// <param name="initialvector">The initial guess.</param>
    /// <remarks>The use of this function is intended for testing/debugging purposes only.</remarks>
    public virtual void InitializeMethod(Vector<double> initialvector)
    {
      // Initialize optimization method
      iterationVectors_ = new Vector<double>[endCriteria_.maxIteration + 1];
      iterationVectors_[0] = initialvector;
      iterationValues_ = new double[endCriteria_.maxIteration + 1];
      iterationValues_[0] = costFunction_.Value(iterationVectors_[0]);
    }

    /// <summary>Performs a single iteration of the optimization method.</summary>
    /// <remarks>The use of this function is intended for testing/debugging purposes only.</remarks>
    public abstract void IterateMethod();

    /// <summary>Minimizes the given cost function.</summary>
    /// <param name="initialvector">The starting vector for the minimization.</param>
    public virtual void Minimize(Vector<double> initialvector)
    {
      endCriteria_.Reset();
      // Method specific implementation
      InitializeMethod(initialvector);

      // Iterate the optimization method
      do
      {
        endCriteria_.iterationCounter++;
        IterateMethod();
      } while (endCriteria_.CheckCriteria(iterationValues_[endCriteria_.iterationCounter - 1], iterationValues_[endCriteria_.iterationCounter]));
    }

    /// <summary>Checks whether the criteria are still satisfied.</summary>
    /// <returns><see langword="true"/> if optimization should continue; otherwise, <see langword="false"/>.</returns>
    /// <remarks>Returns <see langword="true"/> if criteria are still met and optimization continues.</remarks>
    protected virtual bool CheckCriteria()
    {
      return !(
        endCriteria_.CheckIterations() ||
        endCriteria_.CheckFunctionEvaluations() ||
        endCriteria_.CheckStationaryPoint(iterationValues_[endCriteria_.iterationCounter - 1], iterationValues_[endCriteria_.iterationCounter]) ||
        endCriteria_.CheckFunctionEpsilon(iterationValues_[endCriteria_.iterationCounter]) ||
        endCriteria_.CheckFunctionEpsilon(iterationValues_[endCriteria_.iterationCounter - 1])
        );
    }
  }
}
