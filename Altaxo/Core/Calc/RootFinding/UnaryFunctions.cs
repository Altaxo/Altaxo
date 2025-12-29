#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Copyright (C) bsargos, Software Developer, France
//    (see CodeProject article http://www.codeproject.com/Articles/16083/One-dimensional-root-finding-algorithms)
//    This source code file is licenced under the CodeProject open license (CPOL)
//
//    modified for Altaxo:  a data processing and data plotting program
//    Copyright (C) 2012 Dr. Dirk Lellinger
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

namespace Altaxo.Calc.RootFinding
{
  /// <summary>
  /// Provides helpers for creating and composing unary functions (<see cref="Func{T, TResult}"/> with <see cref="double"/> input and output).
  /// </summary>
  public static class UnaryFunctions
  {
    /// <summary>
    /// Creates the identity function <c>f(x) = x</c>.
    /// </summary>
    /// <returns>A function that returns its input unchanged.</returns>
    public static Func<double, double> Identity()
    {
      return new Func<double, double>(delegate (double x)
      { return x; });
    }

    /// <summary>
    /// Creates a constant function <c>f(x) = a</c>.
    /// </summary>
    /// <param name="a">The constant value to return.</param>
    /// <returns>A function that always returns <paramref name="a"/>.</returns>
    public static Func<double, double> Constant(double a)
    {
      return new Func<double, double>(delegate (double x)
      { return a; });
    }

    /// <summary>
    /// Creates a function that adds the results of two functions: <c>h(x) = f1(x) + f2(x)</c>.
    /// </summary>
    /// <param name="f1">The first addend function.</param>
    /// <param name="f2">The second addend function.</param>
    /// <returns>A function representing the pointwise sum of <paramref name="f1"/> and <paramref name="f2"/>.</returns>
    public static Func<double, double> Add(Func<double, double> f1, Func<double, double> f2)
    {
      return new Func<double, double>(delegate (double x)
      { return f1(x) + f2(x); });
    }

    /// <summary>
    /// Creates a function that scales another function by a constant factor: <c>h(x) = lambda * f(x)</c>.
    /// </summary>
    /// <param name="f">The function to scale.</param>
    /// <param name="lambda">The scaling factor.</param>
    /// <returns>A function representing the scaled function.</returns>
    public static Func<double, double> Multiply(Func<double, double> f, double lambda)
    {
      return new Func<double, double>(delegate (double x)
      { return lambda * f(x); });
    }

    /// <summary>
    /// Creates the negation of a function: <c>h(x) = -f(x)</c>.
    /// </summary>
    /// <param name="f">The function to negate.</param>
    /// <returns>A function representing the negation of <paramref name="f"/>.</returns>
    public static Func<double, double> Minus(Func<double, double> f)
    {
      return new Func<double, double>(delegate (double x)
      { return -f(x); });
    }

    /// <summary>
    /// Creates a function that subtracts the results of two functions: <c>h(x) = f1(x) - f2(x)</c>.
    /// </summary>
    /// <param name="f1">The minuend function.</param>
    /// <param name="f2">The subtrahend function.</param>
    /// <returns>A function representing the pointwise difference of <paramref name="f1"/> and <paramref name="f2"/>.</returns>
    public static Func<double, double> Subtract(Func<double, double> f1, Func<double, double> f2)
    {
      return new Func<double, double>(delegate (double x)
      { return f1(x) - f2(x); });
    }

    /// <summary>
    /// Creates a composite function: <c>h(x) = f1(f2(x))</c>.
    /// </summary>
    /// <param name="f1">The outer function.</param>
    /// <param name="f2">The inner function.</param>
    /// <returns>A function representing the composition <paramref name="f1"/>(<paramref name="f2"/>(x)).</returns>
    public static Func<double, double> Compound(Func<double, double> f1, Func<double, double> f2)
    {
      return new Func<double, double>(delegate (double x)
      { return f1(f2(x)); });
    }
  }
}
