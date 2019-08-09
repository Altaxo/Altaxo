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
  ///<summary>Base Class for Function Minimization Optimization Methods</summary>
  /// <remarks>
  /// <para>Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved. See <a>http://www.dnAnalytics.net</a> for details.</para>
  /// <para>Adopted to Altaxo (c) 2005 Dr. Dirk Lellinger.</para>
  /// </remarks>
  public abstract class FunctionMinimizeMethod : OptimizationMethod
  {
    ///<summary> Iteration Solutions </summary>
    protected DoubleVector[] iterationVectors_;

    ///<summary> Iteration Values </summary>
    protected double[] iterationValues_;

    ///<summary> Iteration Gradient Norms </summary>
    protected double[] iterationGradientNorms_;

    ///<summary> Get solution vectors for all iterations </summary>
    public DoubleVector[] IterationVectors
    {
      get { return iterationVectors_; }
    }

    ///<summary> Get initial vector </summary>
    public DoubleVector InitialVector
    {
      get { return iterationVectors_[0]; }
    }

    ///<summary> Get minimum vector </summary>
    public DoubleVector SolutionVector
    {
      get { return iterationVectors_[endCriteria_.iterationCounter]; }
    }

    ///<summary> Get solution function values for all iterations </summary>
    public double[] IterationValues
    {
      get { return iterationValues_; }
    }

    ///<summary> Get initial value </summary>
    public double InitialValue
    {
      get { return iterationValues_[0]; }
    }

    ///<summary> Get value of minimum vector </summary>
    public double SolutionValue
    {
      get { return iterationValues_[endCriteria_.iterationCounter]; }
    }

    ///<summary> Get gradient norms  for all iterations </summary>
    public double[] IterationGradientNorms
    {
      get { return iterationGradientNorms_; }
    }

    ///<summary> Initialize the optimization method </summary>
    ///<remarks> The use of this function is intended for testing/debugging purposes only </remarks>
    public virtual void InitializeMethod(DoubleVector initialvector)
    {
      // Initialize optimization method
      iterationVectors_ = new DoubleVector[endCriteria_.maxIteration + 1];
      iterationVectors_[0] = initialvector;
      iterationValues_ = new double[endCriteria_.maxIteration + 1];
      iterationValues_[0] = costFunction_.Value(iterationVectors_[0]);
    }

    ///<summary> Perform a single iteration of the optimization method </summary>
    ///<remarks> The use of this function is intended for testing/debugging purposes only </remarks>
    public abstract void IterateMethod();

    ///<summary> Minimize the given cost function </summary>
    public virtual void Minimize(DoubleVector initialvector)
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

    ///<summary> Function to check that criteria is still satisfied </summary>
    ///<remarks> Returns true if criteria is still met and optimization continues </remarks>
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
