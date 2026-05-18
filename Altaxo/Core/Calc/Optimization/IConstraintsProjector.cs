#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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

using System;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Optimization
{
  /// <summary>
  /// Projects a set of parameters onto a constraint manifold.
  /// Examples include box constraints (fixed values, lower bounds, upper bounds) and more complex nonlinear constraints.
  /// </summary>
  public interface IConstraintsProjector
  {
    /// <summary>
    /// Determines whether the specified input values satisfy the feasibility criteria.
    /// </summary>
    /// <param name="inputValues">The vector of input values to evaluate.</param>
    /// <returns>Returns <c>true</c> if the input values are feasible; otherwise, <c>false</c>.</returns>
    public bool IsFeasible(Vector<double> inputValues);

    /// <summary>
    /// Projects the specified input vector onto a target space, applying constraints as indicated.
    /// </summary>
    /// <param name="inputValues">The input vector to be projected.</param>
    /// <param name="projectedValues">The resulting projected vector.</param>
    /// <param name="valuesConstrained">A span indicating which values are constrained during projection.</param>
    public void Project(Vector<double> inputValues, Vector<double> projectedValues, Span<bool> valuesConstrained);
  }
}
