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

namespace Altaxo.Calc.Regression.Nonlinear
{
  public interface IFitFunctionWithGradient : IFitFunction
  {
    /// <summary>
    /// This evaluates the gradient of the function with respect to the parameters.
    /// </summary>
    /// <param name="independent">The independent variables.</param>
    /// <param name="parameters">Parameters for evaluation.</param>
    /// <param name="DF">On return, this array contains the one (or more) evaluated
    /// derivatives of the function values with respect to there parameters. See remarks for the order in which they are stored.</param>
    /// <remarks>
    /// The function values, that are calculated by <see cref="IFitFunction.Evaluate" />, are stored in the array <c>FV</c>. For every function value,
    /// the derivative to all given parameters must be calculated. Presumed we have 3 parameters and 2 function values,
    /// on return the array DF must contain:
    /// <code>
    /// DF[0][0] : df0/dp0
    /// DF[0][1] : df0/dp1
    /// DF[0][2] : df0/dp2
    /// DF[1][0] : df1/dp0
    /// DF[1][1] : df2/dp1
    /// DF[1][2] : df1/dp2
    /// </code>
    /// </remarks>
    void EvaluateGradient(double[] independent, double[] parameters, double[][] DF);
  }
}
