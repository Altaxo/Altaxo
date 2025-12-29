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
 * OptimizationMethod.cs
 *
 * Copyright (c) 2004, dnAnalytics Project. All rights reserved.
 * NB: Constraint class inspired by the optimization frame in the QuantLib library
*/

using System;
using System.Text;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Optimization
{
  /// <summary>Base class for optimization methods.</summary>
  /// <remarks>
  /// <para>Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved. See <a>http://www.dnAnalytics.net</a> for details.</para>
  /// <para>Adopted for Altaxo (c) 2005 Dr. Dirk Lellinger.</para>
  /// </remarks>
  public abstract class OptimizationMethod : IFormattable
  {
#nullable disable
    /// <summary>Optimization ending criteria.</summary>
    public EndCriteria endCriteria_;

    /// <summary>Optimization cost function.</summary>
    public ICostFunction costFunction_;
#nullable enable

    /// <summary>Gets the method name.</summary>
    public virtual string MethodName
    {
      get { return "Unspecified Optimization Method"; }
    }

    /// <summary>
    /// Performs an evaluation of the cost function value.
    /// </summary>
    /// <param name="x">The point at which to evaluate the cost function.</param>
    /// <returns>The cost function value at <paramref name="x"/>.</returns>
    protected double FunctionEvaluation(Vector<double> x)
    {
      endCriteria_.functionEvaluationCounter++;
      return costFunction_.Value(x);
    }

    /// <summary>
    /// Performs an evaluation of the cost function gradient.
    /// </summary>
    /// <param name="x">The point at which to evaluate the gradient.</param>
    /// <returns>The gradient of the cost function at <paramref name="x"/>.</returns>
    protected Vector<double> GradientEvaluation(Vector<double> x)
    {
      endCriteria_.gradientEvaluationCounter++;
      return costFunction_.Gradient(x);
    }

    /// <summary>
    /// Performs an evaluation of the cost function Hessian.
    /// </summary>
    /// <param name="x">The point at which to evaluate the Hessian.</param>
    /// <returns>The Hessian matrix of the cost function at <paramref name="x"/>.</returns>
    protected Matrix<double> HessianEvaluation(Vector<double> x)
    {
      endCriteria_.hessianEvaluationCounter++;
      return costFunction_.Hessian(x);
    }

    // --- IFormattable Interface ---

    /// <inheritdoc/>
    public override string ToString()
    {
      return ToString(null, null);
    }

    /// <summary>A string representation of this <c>OptimizationMethod</c>.</summary>
    /// <param name="format">A format specification.</param>
    public string ToString(string format)
    {
      return ToString(format, null);
    }

    /// <summary>A string representation of this <c>OptimizationMethod</c>.</summary>
    /// <param name="formatProvider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information.</param>
    public string ToString(IFormatProvider formatProvider)
    {
      return ToString(null, formatProvider);
    }

    /// <summary>A string representation of this <c>OptimizationMethod</c>.</summary>
    /// <param name="format">A format specification.</param>
    /// <param name="formatProvider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information.</param>
    public string ToString(string? format, IFormatProvider? formatProvider)
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
