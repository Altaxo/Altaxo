#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;

namespace Altaxo.Calc.Regression.Nonlinear
{
  /// <summary>
  /// Represents the interface to a fitting function.
  /// </summary>
  public interface IFitFunction
  {
    /// <summary>
    /// Number of independent variables (i.e. x).
    /// </summary>
    int NumberOfIndependentVariables { get; }

    /// <summary>
    /// Number of dependent variables (i.e. y, in Altaxo this is commonly called v like value).
    /// </summary>
    int NumberOfDependentVariables { get; }

    /// <summary>
    /// Number of parameters of this fit function.
    /// </summary>
    int NumberOfParameters { get; }

    /// <summary>
    /// Returns the ith independent variable name.
    /// </summary>
    /// <param name="i">Index of the independent variable.</param>
    /// <returns>The name of the ith independent variable.</returns>
    string IndependentVariableName(int i);

    /// <summary>
    /// Returns the ith dependent variable name.
    /// </summary>
    /// <param name="i">Index of the dependent variable.</param>
    /// <returns>The name of the ith dependent variable.</returns>
    string DependentVariableName(int i);


    /// <summary>
    /// Returns the ith parameter name.
    /// </summary>
    /// <param name="i">Index of the parameter.</param>
    /// <returns>The name of the ith paramter.</returns>
    string ParameterName(int i);

    /// <summary>
    /// Returns a default parameter value. You must ensure that the fit function would generate
    /// values with those default parameters.
    /// </summary>
    /// <param name="i">Index of the parameter.</param>
    /// <returns>A default value for the parameter <c>i</c>.</returns>
    double DefaultParameterValue(int i);

    /// <summary>
    /// Returns the default variance scaling for the dependent variable <c>i</c>.
    /// </summary>
    /// <param name="i">Index of the dependent variable.</param>
    /// <returns>The variance scaling for that dependent variable. If <c>null</c> is returned, then the
    /// default variance scaling (weight==const.==1) is assumed.</returns>
    IVarianceScaling DefaultVarianceScaling(int i);

    /// <summary>
    /// This evaluates a function value. 
    /// </summary>
    /// <param name="independent">The independent variables.</param>
    /// <param name="parameters">Parameters for evaluation.</param>
    /// <param name="FV">On return, this array contains the one (or more) evaluated
    /// function values at the point (independent).</param>
    void Evaluate(double[] independent, double[] parameters, double[] FV);
  }

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
