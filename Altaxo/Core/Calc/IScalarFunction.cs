#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
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

using System;

namespace Altaxo.Calc
{
  /// <summary>
  /// Provides the interface to a function with one <see cref="double"/> argument and one resulting <see cref="double"/> value.
  /// </summary>
  public interface IScalarFunctionDD
  {
    /// <summary>
    /// Evaluates the function.
    /// </summary>
    /// <param name="x">The argument of the function.</param>
    /// <returns>The resulting value produced by the function.</returns>
    double Evaluate(double x);
  }

  /// <summary>
  /// Example of an <see cref="IScalarFunctionDD"/> which always returns 0 (zero).
  /// </summary>
  /// <seealso cref="Altaxo.Calc.IScalarFunctionDD" />
  public class NullFunction : IScalarFunctionDD
  {
    /// <summary>
    /// Gets the singleton instance of <see cref="NullFunction"/>.
    /// </summary>
    public static NullFunction Instance { get; } = new NullFunction();

    private NullFunction() { }

    /// <inheritdoc/>
    public double Evaluate(double x)
    {
      return 0;
    }
  }

  /// <summary>
  /// Provides the interface to a function with one <see cref="double"/> argument and one resulting <see cref="double"/> value.
  /// The evaluation is parameterized by one or more parameters.
  /// </summary>
  public interface IParametrizedScalarFunctionDD
  {
    /// <summary>
    /// Evaluates the function.
    /// </summary>
    /// <param name="x">The argument of the function.</param>
    /// <param name="parameters">The parameters of this function.</param>
    /// <returns>The resulting value produced by the function.</returns>
    double Evaluate(double x, double[] parameters);
  }

  /// <summary>
  /// Provides the interface to a function with one or more <see cref="double"/> arguments and one or more resulting <see cref="double"/> values.
  /// The evaluation is parameterized by one or more parameters.
  /// </summary>
  public interface IParametrizedFunctionDDD
  {
    /// <summary>
    /// Evaluates the function.
    /// </summary>
    /// <param name="independent">The independent variable values (function arguments).</param>
    /// <param name="parameters">The parameters of this function.</param>
    /// <param name="result">On output, contains the function result.</param>
    void Evaluate(double[] independent, double[] parameters, double[] result);
  }
}
