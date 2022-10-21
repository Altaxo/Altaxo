#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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

#nullable enable

using System.Collections.Generic;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Regression.Nonlinear
{
  public interface IFitFunctionWithDerivative : IFitFunction
  {
    /// <summary>
    /// This evaluates the gradient of the function with respect to the parameters.
    /// </summary>
    /// <param name="independent">The independent variables (x-values).
    /// Every row of that matrix corresponds to one observation.
    /// The columns of the matrix represent the different independent variables of the fit function.
    /// Thus, for a usual function of one variable, the number of columns is 1.</param>
    /// <param name="parameters">Parameters for evaluation.</param>
    /// <param name="isFixed">If not null, this list designates the parameters that are fixed. No derivative value
    /// for those parameters need to be calculated.</param>
    /// <param name="DF">On return, this array contains the one (or more) evaluated
    /// derivatives of the function values with respect to there parameters. See remarks for the order in which they are stored.</param>
    /// <param name="dependentVariableChoice">Determines which output variables are written to the output vector. See remarks.</param>
    /// <remarks>
    /// The derivative values are stored in the array <c>DF</c>. For each dependent variable of the fit function that is included in the output
    /// (see <paramref name="dependentVariableChoice"/>), the derivative to all given parameters must be calculated.
    /// Presumed we have 3 parameters (p0, p1 and p2) and 2 dependent variables (f0 and f1), for one observation the array DF must contain:
    /// <code>
    /// DF[0,0] : df0/dp0
    /// DF[0,1] : df0/dp1
    /// DF[0,2] : df0/dp2
    /// DF[1,0] : df1/dp0
    /// DF[1,1] : df2/dp1
    /// DF[1,2] : df1/dp2
    /// </code>
    ///
    /// Concerning <paramref name="dependentVariableChoice"/>: if this parameter is null, the derivatives of all dependent variables the fit function provides will be included in the output matrix <paramref name="DF"/>.
    /// If this parameter is not null, only the derivatives of those dependent variables, for which the element is true, are included in the output vector (at least one element of this array must be true).
    /// </remarks>
    void EvaluateDerivative(IROMatrix<double> independent, IReadOnlyList<double> parameters, IReadOnlyList<bool>? isFixed, IMatrix<double> DF, IReadOnlyList<bool>? dependentVariableChoice);
  }
}
