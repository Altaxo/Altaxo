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

namespace Altaxo.Calc
{
  /// <summary>
  /// Provides the interface to a function with one double argument, and one resulting double value.
  /// </summary>
  public interface IScalarFunctionDD
  {
    /// <summary>
    /// The function evaluation.
    /// </summary>
    /// <param name="x">The argument of the function.</param>
    /// <returns>The resulting value that the function evaluates.</returns>
    double Evaluate(double x);
  }

  /// <summary>
  /// Provides the interface to a function with one double argument, and one resulting double value.
  /// The evaluation is parametrized by one or more parameters.
  /// </summary>
  public interface IParametrizedScalarFunctionDD
  {
    /// <summary>
    /// The function evaluation.
    /// </summary>
    /// <param name="x">The argument of the function.</param>
    /// <param name="parameters">The parameters of this function</param>
    /// <returns>The resulting value that the function evaluates.</returns>
    double Evaluate(double x, double[] parameters);
  }

  /// <summary>
  /// Provides the interface to a function with one double argument, and one resulting double value.
  /// The evaluation is parametrized by one or more parameters.
  /// </summary>
  public interface IParametrizedFunctionDDD
  {
    /// <summary>
    /// The function evaluation.
    /// </summary>
    /// <param name="independent">The argument of the function.</param>
    /// <param name="parameters">The parameters of this function</param>
    /// <param name="result">On output, this is the function result.</param>
    /// <returns>The resulting value that the function evaluates.</returns>
    void Evaluate(double[] independent, double[] parameters, double[] result);
  }
}
