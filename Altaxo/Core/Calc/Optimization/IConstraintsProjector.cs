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
    /// Determine whether all parameters are fixed (i.e., all parameters have constraints that fix their values).
    /// </summary>
    /// <value><c>true</c> if all parameters are fixed; otherwise, <c>false</c>.</value>
    bool AreAllParametersFixed { get; }

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

    /// <summary>
    /// Attempts to convert the constraints to simple box constraints for
    /// each parameter.
    /// </summary>
    /// <param name="tolerance">When determining whether a matrix entry is considered non-zero, this tolerance is used relative to the row norm. This allows for some numerical imprecision in the constraint definitions while still enabling conversion to boundary constraints when appropriate.</param>
    /// <returns>If the conversion fails, the return value is null. If the conversion succeeds, the return value is atuple consisting of the fixed values, lower bounds, lower bound exclusivity flags, upper bounds, and upper bound exclusivity flags for each parameter if the conversion is possible; otherwise, null.</returns>
    public (double?[] fixedValues, double?[]? LowerBounds, bool[]? isLowerBoundExclusive, double?[]? UpperBounds, bool[]? isUpperBoundExclusive)? TryConvertToBoxConstraints(double tolerance = 1e-12);


    /// <summary>
    /// Creates a new constraint projector with all fixed parameters eliminated from the constraints.
    /// </summary>
    /// <remarks>Use this method to obtain a projector that operates only on free parameters, with all
    /// parameters fixed by constraints removed. The returned array provides the values of the parameters that were
    /// fixed.</remarks>
    /// <returns>A tuple containing the new constraint projector with fixed parameters removed, and an array of nullable
    /// doubles representing the values of the fixed parameters.</returns>
    /// <exception cref="InvalidOperationException">Thrown if any fixed parameters remain after elimination, indicating an unexpected state.</exception>
    public (IConstraintsProjector Projector, double?[] FixedParameters) ToProjectorWithoutFixedParameters();

  }
}
