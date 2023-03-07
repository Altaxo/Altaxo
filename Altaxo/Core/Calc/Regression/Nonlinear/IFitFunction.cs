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
using System;
using System.Collections.Generic;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Regression.Nonlinear
{
  /// <summary>
  /// This evaluates a function value.
  /// </summary>
  /// <param name="independent">The independent variables.</param>
  /// <param name="parameters">Parameters for evaluation.</param>
  /// <param name="FV">On return, this array contains the one (or more) evaluated
  /// function value(s) at the point designated by <paramref name="independent"/>.</param>
  public delegate void FitEvaluationFunction(double[] independent, double[] parameters, double[] FV);

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
    /// Number of dependent variables (i.e. y, in Altaxo this is commonly called v (like value)).
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
    IVarianceScaling? DefaultVarianceScaling(int i);

    /// <summary>
    /// This evaluates a function value.
    /// </summary>
    /// <param name="independent">The independent variables.</param>
    /// <param name="parameters">Parameters for evaluation.</param>
    /// <param name="FV">On return, this array contains the one (or more) evaluated
    /// function values at the point (independent).</param>
    void Evaluate(double[] independent, double[] parameters, double[] FV);

    /// <summary>
    /// Evaluates the function values at multiple x-points.
    /// </summary>
    /// <param name="independent">The independent variables. Every row of that matrix corresponds to one observation. The columns of the matrix
    /// represent the different independent variables. Thus, for a usual function of one variable, the number of columns is 1.</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="FV">On return, contains the function values. Note that if the fit function has multiple dependent variables,
    /// those variables will be written in subsequent order to the output vector.</param>
    /// <param name="dependentVariableChoice">Determines which output variables are written to the output vector. See remarks.</param>
    /// <remarks>
    /// Concerning <paramref name="dependentVariableChoice"/>: if this parameter is null, all dependent variables the fit function provides, will be included in the output vector <paramref name="FV"/>.
    /// If this parameter is not null, only those dependent variables, for which the value is true, are included in the output vector (at least one element of this array must be true).
    /// </remarks>
    void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> parameters, IVector<double> FV, IReadOnlyList<bool>? dependentVariableChoice);

    /// <summary>
    /// Occurs when the fit function changed, including number or name of parameters, independent variables, dependent variables, or the scaling.
    /// </summary>
    event EventHandler? Changed;

    /// <summary>
    /// Gets the parameter boundaries that are really a hard limit, i.e. outside those limits,
    /// the function would probably evaluate NaN values, or makes no sense.
    /// </summary>
    /// <returns>The lower and upper hard limits for the parameter. If no parameter has a lower limit or an upper
    /// limit, the returned list will be null. Of only some of the parameters have no limit, than
    /// that element will be null.</returns>
    (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesHardLimit();

    /// <summary>
    /// Gets the intended parameter boundaries. This are soft limits, boundaries
    /// so that the intended purpose of the fit function is fullfilled.
    /// Example: in the exponential decay Exp(-a*t) a is intended to be positive. This is a soft limit,
    /// and not a hard limit, because a could be also negative, and the fit nevertheless would succeed.
    /// </summary>
    /// <returns>The lower and upper soft limits for the parameter. If no parameter has a lower limit or an upper
    /// limit, the returned list will be null. Of only some of the parameters have no limit, than
    /// that element will be null.</returns>
    (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesSoftLimit();
  }
}
