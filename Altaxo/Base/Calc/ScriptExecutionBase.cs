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

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Collections;
using Altaxo.Data;

namespace Altaxo.Calc
{
  /// <summary>
  /// ScriptExecutionBase provides the (mathematical) environment to execute scripts.
  /// </summary>
  /// <remarks>
  /// ScriptExecutionBase provides shortcuts for mathematical functions for single values,
  /// and the mathematical functions for columns.
  /// </remarks>
  public class ScriptExecutionBase
  {
    #region System Double_Mathematics

    // ------------------- Double Mathematics --------------------------------------
    /// <summary>
    /// Euler's number.
    /// </summary>
    public const double E = System.Math.E;

    /// <summary>
    /// The ratio of a circle's circumference to its diameter.
    /// </summary>
    public const double PI = System.Math.PI;

    /// <summary>
    /// Returns the absolute value of the specified number.
    /// </summary>
    public static double Abs(double s)
    {
      return System.Math.Abs(s);
    }

    /// <summary>
    /// Returns the arc cosine of the specified number.
    /// </summary>
    public static double Acos(double s)
    {
      return System.Math.Acos(s);
    }

    /// <summary>
    /// Returns the arc sine of the specified number.
    /// </summary>
    public static double Asin(double s)
    {
      return System.Math.Asin(s);
    }

    /// <summary>
    /// Returns the arc tangent of the specified number.
    /// </summary>
    public static double Atan(double s)
    {
      return System.Math.Atan(s);
    }

    /// <summary>
    /// Returns the angle whose tangent is the quotient of two specified numbers.
    /// </summary>
    public static double Atan2(double y, double x)
    {
      return System.Math.Atan2(y, x);
    }

    /// <summary>
    /// Returns the smallest integral value greater than or equal to the specified number.
    /// </summary>
    public static double Ceiling(double s)
    {
      return System.Math.Ceiling(s);
    }

    /// <summary>
    /// Returns the cosine of the specified angle.
    /// </summary>
    public static double Cos(double s)
    {
      return System.Math.Cos(s);
    }

    /// <summary>
    /// Returns the hyperbolic cosine of the specified angle.
    /// </summary>
    public static double Cosh(double s)
    {
      return System.Math.Cosh(s);
    }

    /// <summary>
    /// Returns e raised to the specified power.
    /// </summary>
    public static double Exp(double s)
    {
      return System.Math.Exp(s);
    }

    /// <summary>
    /// Returns the largest integral value less than or equal to the specified number.
    /// </summary>
    public static double Floor(double s)
    {
      return System.Math.Floor(s);
    }

    /// <summary>
    /// Returns the remainder resulting from the division of two specified numbers.
    /// </summary>
    public static double IEEERemainder(double x, double y)
    {
      return System.Math.IEEERemainder(x, y);
    }

    /// <summary>
    /// Returns the natural logarithm of the specified number.
    /// </summary>
    public static double Log(double s)
    {
      return System.Math.Log(s);
    }

    /// <summary>
    /// Returns the base-10 logarithm of the specified number.
    /// </summary>
    public static double Log10(double s)
    {
      return System.Math.Log10(s);
    }

    /// <summary>
    /// Returns the logarithm of the specified number in the specified base.
    /// </summary>
    public static double Log(double s, double bas)
    {
      return System.Math.Log(s, bas);
    }

    /// <summary>
    /// Returns the larger of two specified numbers.
    /// </summary>
    public static double Max(double x, double y)
    {
      return System.Math.Max(x, y);
    }

    /// <summary>
    /// Returns the smaller of two specified numbers.
    /// </summary>
    public static double Min(double x, double y)
    {
      return System.Math.Min(x, y);
    }

    /// <summary>
    /// Returns a specified number raised to the specified power.
    /// </summary>
    public static double Pow(double x, double y)
    {
      return System.Math.Pow(x, y);
    }

    /// <summary>
    /// Rounds a value to the nearest integral value.
    /// </summary>
    public static double Round(double x)
    {
      return System.Math.Round(x);
    }

    /// <summary>
    /// Rounds a value to the specified number of fractional digits.
    /// </summary>
    public static double Round(double x, int i)
    {
      return System.Math.Round(x, i);
    }

    /// <summary>
    /// Returns an integer that indicates the sign of the specified number.
    /// </summary>
    public static double Sign(double s)
    {
      return System.Math.Sign(s);
    }

    /// <summary>
    /// Returns the sine of the specified angle.
    /// </summary>
    public static double Sin(double s)
    {
      return System.Math.Sin(s);
    }

    /// <summary>
    /// Returns the hyperbolic sine of the specified angle.
    /// </summary>
    public static double Sinh(double s)
    {
      return System.Math.Sinh(s);
    }

    /// <summary>
    /// Returns the square root of the specified number.
    /// </summary>
    public static double Sqrt(double s)
    {
      return System.Math.Sqrt(s);
    }

    /// <summary>
    /// Returns the tangent of the specified angle.
    /// </summary>
    public static double Tan(double s)
    {
      return System.Math.Tan(s);
    }

    /// <summary>
    /// Returns the hyperbolic tangent of the specified angle.
    /// </summary>
    public static double Tanh(double s)
    {
      return System.Math.Tanh(s);
    }

    #endregion System Double_Mathematics

    #region Other Double Mathematics

    /// <summary>Calculates the 2nd power of <paramref name="x"/> (square of <paramref name="x"/>).</summary>
    /// <param name="x">A double-precision floating-point number.</param>
    /// <returns>The 2nd power of <paramref name="x"/>.</returns>
    public static double Pow2(double x)
    {
      return RMath.Pow2(x);
    }

    /// <summary>Calculates the 3rd power of <paramref name="x"/> (cube of <paramref name="x"/>).</summary>
    /// <param name="x">A double-precision floating-point number.</param>
    /// <returns>The 3rd power of <paramref name="x"/>.</returns>
    public static double Pow3(double x)
    {
      return RMath.Pow3(x);
    }

    /// <summary>Calculates the 4th power of <paramref name="x"/>.</summary>
    /// <param name="x">A double-precision floating-point number.</param>
    /// <returns>The 4th power of <paramref name="x"/>.</returns>
    public static double Pow4(double x)
    {
      return RMath.Pow4(x);
    }

    /// <summary>Calculates the 5th power of <paramref name="x"/>.</summary>
    /// <param name="x">A double-precision floating-point number.</param>
    /// <returns>The 5th power of <paramref name="x"/>.</returns>
    public static double Pow5(double x)
    {
      return RMath.Pow5(x);
    }

    /// <summary>Calculates the 6th power of <paramref name="x"/>.</summary>
    /// <param name="x">A double-precision floating-point number.</param>
    /// <returns>The 6th power of <paramref name="x"/>.</returns>
    public static double Pow6(double x)
    {
      return RMath.Pow6(x);
    }

    /// <summary>Calculates the 7th power of <paramref name="x"/>.</summary>
    /// <param name="x">A double-precision floating-point number.</param>
    /// <returns>The 7th power of <paramref name="x"/>.</returns>
    public static double Pow7(double x)
    {
      return RMath.Pow7(x);
    }

    /// <summary>Calculates the 8th power of <paramref name="x"/>.</summary>
    /// <param name="x">A double-precision floating-point number.</param>
    /// <returns>The 8th power of <paramref name="x"/>.</returns>
    public static double Pow8(double x)
    {
      return RMath.Pow8(x);
    }

    /// <summary>Calculates the 9th power of <paramref name="x"/>.</summary>
    /// <param name="x">A double-precision floating-point number.</param>
    /// <returns>The 9th power of <paramref name="x"/>.</returns>
    public static double Pow9(double x)
    {
      return RMath.Pow9(x);
    }

    /// <summary>Calculates the number <paramref name="x"/> raised to the specified power <paramref name="i"/>.</summary>
    /// <param name="x">A double-precision floating-point number to be raised to the power <paramref name="i"/>.</param>
    /// <param name="i">An integer number that specifies the power.</param>
    /// <returns>The number <paramref name="x"/> raised to the power <paramref name="i"/>.</returns>
    public static double Pow(double x, int i)
    {
      return RMath.Pow(x, i);
    }

    #endregion Other Double Mathematics

    #region AltaxoDoubleColumn_Mathematics

    // ---------------------- DoubleColumn mathematics -----------------------------

    /// <summary>
    /// Applies absolute value to a <see cref="Altaxo.Data.DoubleColumn"/>.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Abs(Altaxo.Data.DoubleColumn s)
    {
      return Altaxo.Data.DoubleColumn.Abs(s);
    }

    /// <summary>
    /// Applies arc cosine to a <see cref="Altaxo.Data.DoubleColumn"/>.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Acos(Altaxo.Data.DoubleColumn s)
    {
      return Altaxo.Data.DoubleColumn.Acos(s);
    }

    /// <summary>
    /// Applies arc sine to a <see cref="Altaxo.Data.DoubleColumn"/>.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Asin(Altaxo.Data.DoubleColumn s)
    {
      return Altaxo.Data.DoubleColumn.Asin(s);
    }

    /// <summary>
    /// Applies arc tangent to a <see cref="Altaxo.Data.DoubleColumn"/>.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Atan(Altaxo.Data.DoubleColumn s)
    {
      return Altaxo.Data.DoubleColumn.Atan(s);
    }

    /// <summary>
    /// Applies <see cref="Math.Atan2(double, double)"/> to two <see cref="Altaxo.Data.DoubleColumn"/> instances.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Atan2(Altaxo.Data.DoubleColumn y, Altaxo.Data.DoubleColumn x)
    {
      return Altaxo.Data.DoubleColumn.Atan2(y, x);
    }

    /// <summary>
    /// Applies <see cref="Math.Atan2(double, double)"/> to a <see cref="Altaxo.Data.DoubleColumn"/> and a scalar.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Atan2(Altaxo.Data.DoubleColumn y, double x)
    {
      return Altaxo.Data.DoubleColumn.Atan2(y, x);
    }

    /// <summary>
    /// Applies <see cref="Math.Atan2(double, double)"/> to a scalar and a <see cref="Altaxo.Data.DoubleColumn"/>.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Atan2(double y, Altaxo.Data.DoubleColumn x)
    {
      return Altaxo.Data.DoubleColumn.Atan2(y, x);
    }

    /// <summary>
    /// Applies <see cref="Math.Ceiling(double)"/> to a <see cref="Altaxo.Data.DoubleColumn"/>.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Ceiling(Altaxo.Data.DoubleColumn s)
    {
      return Altaxo.Data.DoubleColumn.Ceiling(s);
    }

    /// <summary>
    /// Applies cosine to a <see cref="Altaxo.Data.DoubleColumn"/>.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Cos(Altaxo.Data.DoubleColumn s)
    {
      return Altaxo.Data.DoubleColumn.Cos(s);
    }

    /// <summary>
    /// Applies hyperbolic cosine to a <see cref="Altaxo.Data.DoubleColumn"/>.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Cosh(Altaxo.Data.DoubleColumn s)
    {
      return Altaxo.Data.DoubleColumn.Cosh(s);
    }

    /// <summary>
    /// Applies exponentiation to a <see cref="Altaxo.Data.DoubleColumn"/>.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Exp(Altaxo.Data.DoubleColumn s)
    {
      return Altaxo.Data.DoubleColumn.Exp(s);
    }

    /// <summary>
    /// Applies <see cref="Math.Floor(double)"/> to a <see cref="Altaxo.Data.DoubleColumn"/>.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Floor(Altaxo.Data.DoubleColumn s)
    {
      return Altaxo.Data.DoubleColumn.Floor(s);
    }

    /// <summary>
    /// Applies <see cref="Math.IEEERemainder(double, double)"/> to two <see cref="Altaxo.Data.DoubleColumn"/> instances.
    /// </summary>
    public static Altaxo.Data.DoubleColumn IEEERemainder(Altaxo.Data.DoubleColumn x, Altaxo.Data.DoubleColumn y)
    {
      return Altaxo.Data.DoubleColumn.IEEERemainder(x, y);
    }

    /// <summary>
    /// Applies <see cref="Math.IEEERemainder(double, double)"/> to a <see cref="Altaxo.Data.DoubleColumn"/> and a scalar.
    /// </summary>
    public static Altaxo.Data.DoubleColumn IEEERemainder(Altaxo.Data.DoubleColumn x, double y)
    {
      return Altaxo.Data.DoubleColumn.IEEERemainder(x, y);
    }

    /// <summary>
    /// Applies <see cref="Math.IEEERemainder(double, double)"/> to a scalar and a <see cref="Altaxo.Data.DoubleColumn"/>.
    /// </summary>
    public static Altaxo.Data.DoubleColumn IEEERemainder(double x, Altaxo.Data.DoubleColumn y)
    {
      return Altaxo.Data.DoubleColumn.IEEERemainder(x, y);
    }

    /// <summary>
    /// Applies the natural logarithm to a <see cref="Altaxo.Data.DoubleColumn"/>.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Log(Altaxo.Data.DoubleColumn s)
    {
      return Altaxo.Data.DoubleColumn.Log(s);
    }

    /// <summary>
    /// Applies logarithm evaluation to two <see cref="Altaxo.Data.DoubleColumn"/> instances.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Log(Altaxo.Data.DoubleColumn s, Altaxo.Data.DoubleColumn bas)
    {
      return Altaxo.Data.DoubleColumn.Log(s, bas);
    }

    /// <summary>
    /// Applies logarithm evaluation to a <see cref="Altaxo.Data.DoubleColumn"/> and a scalar base.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Log(Altaxo.Data.DoubleColumn s, double bas)
    {
      return Altaxo.Data.DoubleColumn.Log(s, bas);
    }

    /// <summary>
    /// Applies logarithm evaluation to a scalar and a <see cref="Altaxo.Data.DoubleColumn"/> base.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Log(double s, Altaxo.Data.DoubleColumn bas)
    {
      return Altaxo.Data.DoubleColumn.Log(s, bas);
    }

    /// <summary>
    /// Applies the base-10 logarithm to a <see cref="Altaxo.Data.DoubleColumn"/>.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Log10(Altaxo.Data.DoubleColumn s)
    {
      return Altaxo.Data.DoubleColumn.Log10(s);
    }

    /// <summary>
    /// Computes the element-wise maximum of two <see cref="Altaxo.Data.DoubleColumn"/> instances.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Max(Altaxo.Data.DoubleColumn x, Altaxo.Data.DoubleColumn y)
    {
      return Altaxo.Data.DoubleColumn.Max(x, y);
    }

    /// <summary>
    /// Computes the element-wise maximum of a <see cref="Altaxo.Data.DoubleColumn"/> and a scalar.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Max(Altaxo.Data.DoubleColumn x, double y)
    {
      return Altaxo.Data.DoubleColumn.Max(x, y);
    }

    /// <summary>
    /// Computes the element-wise maximum of a scalar and a <see cref="Altaxo.Data.DoubleColumn"/>.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Max(double x, Altaxo.Data.DoubleColumn y)
    {
      return Altaxo.Data.DoubleColumn.Max(x, y);
    }

    /// <summary>
    /// Computes the element-wise minimum of two <see cref="Altaxo.Data.DoubleColumn"/> instances.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Min(Altaxo.Data.DoubleColumn x, Altaxo.Data.DoubleColumn y)
    {
      return Altaxo.Data.DoubleColumn.Min(x, y);
    }

    /// <summary>
    /// Computes the element-wise minimum of a <see cref="Altaxo.Data.DoubleColumn"/> and a scalar.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Min(Altaxo.Data.DoubleColumn x, double y)
    {
      return Altaxo.Data.DoubleColumn.Min(x, y);
    }

    /// <summary>
    /// Computes the element-wise minimum of a scalar and a <see cref="Altaxo.Data.DoubleColumn"/>.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Min(double x, Altaxo.Data.DoubleColumn y)
    {
      return Altaxo.Data.DoubleColumn.Min(x, y);
    }

    /// <summary>
    /// Applies element-wise power evaluation to two <see cref="Altaxo.Data.DoubleColumn"/> instances.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Pow(Altaxo.Data.DoubleColumn x, Altaxo.Data.DoubleColumn y)
    {
      return Altaxo.Data.DoubleColumn.Pow(x, y);
    }

    /// <summary>
    /// Raises each element of a <see cref="Altaxo.Data.DoubleColumn"/> to a scalar power.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Pow(Altaxo.Data.DoubleColumn x, double y)
    {
      return Altaxo.Data.DoubleColumn.Pow(x, y);
    }

    /// <summary>
    /// Raises a scalar base to element-wise powers from a <see cref="Altaxo.Data.DoubleColumn"/>.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Pow(double x, Altaxo.Data.DoubleColumn y)
    {
      return Altaxo.Data.DoubleColumn.Pow(x, y);
    }

    /// <summary>
    /// Rounds each element of a <see cref="Altaxo.Data.DoubleColumn"/>.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Round(Altaxo.Data.DoubleColumn x)
    {
      return Altaxo.Data.DoubleColumn.Round(x);
    }

    /// <summary>
    /// Rounds elements of a <see cref="Altaxo.Data.DoubleColumn"/> using element-wise precision values.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Round(Altaxo.Data.DoubleColumn x, Altaxo.Data.DoubleColumn i)
    {
      return Altaxo.Data.DoubleColumn.Round(x, i);
    }

    /// <summary>
    /// Rounds each element of a <see cref="Altaxo.Data.DoubleColumn"/> to the specified number of digits.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Round(Altaxo.Data.DoubleColumn x, int i)
    {
      return Altaxo.Data.DoubleColumn.Round(x, i);
    }

    /// <summary>
    /// Rounds a scalar value using element-wise precision values from a <see cref="Altaxo.Data.DoubleColumn"/>.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Round(double x, Altaxo.Data.DoubleColumn i)
    {
      return Altaxo.Data.DoubleColumn.Round(x, i);
    }

    /// <summary>
    /// Applies sign evaluation to a <see cref="Altaxo.Data.DoubleColumn"/>.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Sign(Altaxo.Data.DoubleColumn s)
    {
      return Altaxo.Data.DoubleColumn.Sign(s);
    }

    /// <summary>
    /// Applies sine to a <see cref="Altaxo.Data.DoubleColumn"/>.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Sin(Altaxo.Data.DoubleColumn s)
    {
      return Altaxo.Data.DoubleColumn.Sin(s);
    }

    /// <summary>
    /// Applies hyperbolic sine to a <see cref="Altaxo.Data.DoubleColumn"/>.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Sinh(Altaxo.Data.DoubleColumn s)
    {
      return Altaxo.Data.DoubleColumn.Sinh(s);
    }

    /// <summary>
    /// Applies square root to a <see cref="Altaxo.Data.DoubleColumn"/>.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Sqrt(Altaxo.Data.DoubleColumn s)
    {
      return Altaxo.Data.DoubleColumn.Sqrt(s);
    }

    /// <summary>
    /// Applies tangent to a <see cref="Altaxo.Data.DoubleColumn"/>.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Tan(Altaxo.Data.DoubleColumn s)
    {
      return Altaxo.Data.DoubleColumn.Tan(s);
    }

    /// <summary>
    /// Applies hyperbolic tangent to a <see cref="Altaxo.Data.DoubleColumn"/>.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Tanh(Altaxo.Data.DoubleColumn s)
    {
      return Altaxo.Data.DoubleColumn.Tanh(s);
    }

    #endregion AltaxoDoubleColumn_Mathematics

    #region Altaxo DoubleColumn Mathematics (Other)

    /// <summary>Calculates the 2nd power of each element of <paramref name="x"/> (square).</summary>
    /// <param name="x">A column containing double-precision elements.</param>
    /// <returns>A new column in which each each element is the 2nd power of the corresponding element of column <paramref name="x"/>.</returns>
    public static Altaxo.Data.DoubleColumn Pow2(Altaxo.Data.DoubleColumn x)
    {
      return Altaxo.Data.DoubleColumn.Pow2(x);
    }

    /// <summary>Calculates the 3rd power of each element of <paramref name="x"/> (cube).</summary>
    /// <param name="x">A column containing double-precision elements.</param>
    /// <returns>A new column in which each each element is the 3rd power of the corresponding element of column <paramref name="x"/>.</returns>
    public static Altaxo.Data.DoubleColumn Pow3(Altaxo.Data.DoubleColumn x)
    {
      return Altaxo.Data.DoubleColumn.Pow3(x);
    }

    /// <summary>Calculates the 4th power of each element of <paramref name="x"/>.</summary>
    /// <param name="x">A column containing double-precision elements.</param>
    /// <returns>A new column in which each each element is the 4th power of the corresponding element of column <paramref name="x"/>.</returns>
    public static Altaxo.Data.DoubleColumn Pow4(Altaxo.Data.DoubleColumn x)
    {
      return Altaxo.Data.DoubleColumn.Pow4(x);
    }

    /// <summary>Calculates the 5th power of each element of <paramref name="x"/>.</summary>
    /// <param name="x">A column containing double-precision elements.</param>
    /// <returns>A new column in which each each element is the 5th power of the corresponding element of column <paramref name="x"/>.</returns>
    public static Altaxo.Data.DoubleColumn Pow5(Altaxo.Data.DoubleColumn x)
    {
      return Altaxo.Data.DoubleColumn.Pow5(x);
    }

    /// <summary>Calculates the 6th power of each element of <paramref name="x"/>.</summary>
    /// <param name="x">A column containing double-precision elements.</param>
    /// <returns>A new column in which each each element is the 6th power of the corresponding element of column <paramref name="x"/>.</returns>
    public static Altaxo.Data.DoubleColumn Pow6(Altaxo.Data.DoubleColumn x)
    {
      return Altaxo.Data.DoubleColumn.Pow6(x);
    }

    /// <summary>Calculates the 7th power of each element of <paramref name="x"/>.</summary>
    /// <param name="x">A column containing double-precision elements.</param>
    /// <returns>A new column in which each each element is the 7th power of the corresponding element of column <paramref name="x"/>.</returns>
    public static Altaxo.Data.DoubleColumn Pow7(Altaxo.Data.DoubleColumn x)
    {
      return Altaxo.Data.DoubleColumn.Pow7(x);
    }

    /// <summary>Calculates the 8th power of each element of <paramref name="x"/>.</summary>
    /// <param name="x">A column containing double-precision elements.</param>
    /// <returns>A new column in which each each element is the 8th power of the corresponding element of column <paramref name="x"/>.</returns>
    public static Altaxo.Data.DoubleColumn Pow8(Altaxo.Data.DoubleColumn x)
    {
      return Altaxo.Data.DoubleColumn.Pow8(x);
    }

    /// <summary>Calculates the 9th power of each element of <paramref name="x"/>.</summary>
    /// <param name="x">A column containing double-precision elements.</param>
    /// <returns>A new column in which each each element is the 9th power of the corresponding element of column <paramref name="x"/>.</returns>
    public static Altaxo.Data.DoubleColumn Pow9(Altaxo.Data.DoubleColumn x)
    {
      return Altaxo.Data.DoubleColumn.Pow9(x);
    }

    /// <summary>Calculates the <paramref name="i"/>th power of each element of <paramref name="x"/>.</summary>
    /// <param name="x">A column containing double-precision elements.</param>
    /// <param name="i">An integer number that specifies the power.</param>
    /// <returns>A new column in which each each element is the <paramref name="i"/>th power of the corresponding element of column <paramref name="x"/>.</returns>
    public static Altaxo.Data.DoubleColumn Pow(Altaxo.Data.DoubleColumn x, int i)
    {
      return Altaxo.Data.DoubleColumn.Pow(x, i);
    }

    /// <summary>Applies the specified unary <paramref name="function"/> to each element in column <paramref name="x"/>.</summary>
    /// <param name="function">The function to apply.</param>
    /// <param name="x">The column containing the elements to which the specified <paramref name="function"/> should be applied.</param>
    /// <returns>A new column in which each element y[i] is the function applied to the element x[i], i.e. y[i] = function(x[i]).</returns>
    public static Altaxo.Data.DoubleColumn Map(Func<double, double> function, Altaxo.Data.DoubleColumn x)
    {
      return Altaxo.Data.DoubleColumn.Map(function, x);
    }

    /// <summary>Applies the specified binary <paramref name="function"/> to each element in column <paramref name="x"/> and column <paramref name="y"/>.</summary>
    /// <param name="function">The function to apply.</param>
    /// <param name="x">The column containing the elements to which the specified <paramref name="function"/> should be applied (as first argument of that function).</param>
    /// <param name="y">The column containing the elements to which the specified <paramref name="function"/> should be applied (as second argument of that function).</param>
    /// <returns>A new column in which each element y[i] is the function applied to the elements x[i] and y[i], i.e. y[i] = function(x[i], y[i]).</returns>
    public static Altaxo.Data.DoubleColumn Map(Func<double, double, double> function, Altaxo.Data.DoubleColumn x, Altaxo.Data.DoubleColumn y)
    {
      return Altaxo.Data.DoubleColumn.Map(function, x, y);
    }

    /// <summary>Applies the specified binary <paramref name="function"/> to each element in column <paramref name="x"/> and to paramenter <paramref name="y"/>.</summary>
    /// <param name="function">The function to apply.</param>
    /// <param name="x">The column containing the elements to which the specified <paramref name="function"/> should be applied (as first argument of that function).</param>
    /// <param name="y">A double-precision numeric value used as second argument of that function.</param>
    /// <returns>A new column in which each element y[i] is the function applied to the element x[i] and argument y, i.e. y[i] = function(x[i], y).</returns>
    public static Altaxo.Data.DoubleColumn Map(Func<double, double, double> function, Altaxo.Data.DoubleColumn x, double y)
    {
      return Altaxo.Data.DoubleColumn.Map(function, x, y);
    }

    /// <summary>Applies the specified binary <paramref name="function"/> to the specified number <paramref name="x"/> and to each element in column <paramref name="x"/>.</summary>
    /// <param name="function">The function to apply.</param>
    /// <param name="x">A double-precision numeric value used as first argument of that function.</param>
    /// <param name="y">The column containing the elements to which the specified <paramref name="function"/> should be applied (as second argument of that function).</param>
    /// <returns>A new column in which each element y[i] is the function applied to the specified value <paramref name="x"/> and the element y[i], i.e. y[i] = function(x, y[i]).</returns>
    public static Altaxo.Data.DoubleColumn Map(Func<double, double, double> function, double x, Altaxo.Data.DoubleColumn y)
    {
      return Altaxo.Data.DoubleColumn.Map(function, x, y);
    }

    #endregion Altaxo DoubleColumn Mathematics (Other)

    #region AltaxoDataColumn_Mathematics

    // ------------------------- Altaxo.Data.DataColumn mathematics ----------------------------
    /// <summary>
    /// Applies absolute value to a numeric data column.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Abs(Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Abs((Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Abs() to " + x.TypeAndName, "x");
    }

    /// <summary>
    /// Applies arc cosine to a numeric data column.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Acos(Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Acos((Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Acos() to " + x.TypeAndName, "x");
    }

    /// <summary>
    /// Applies arc sine to a numeric data column.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Asin(Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Asin((Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Asin() to " + x.TypeAndName, "x");
    }

    /// <summary>
    /// Applies arc tangent to a numeric data column.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Atan(Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Atan((Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Atan() to " + x.TypeAndName, "x");
    }

    /// <summary>
    /// Applies <see cref="Math.Atan2(double, double)"/> to two numeric data columns.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Atan2(Altaxo.Data.DataColumn y, Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == y.GetType() && typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Atan2((Altaxo.Data.DoubleColumn)y, (Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Atan2() to " + y.TypeAndName + " and " + x.TypeAndName, "x");
    }

    /// <summary>
    /// Applies <see cref="Math.Atan2(double, double)"/> to a data column and a scalar.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Atan2(Altaxo.Data.DataColumn y, double x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == y.GetType())
        return Altaxo.Data.DoubleColumn.Atan2((Altaxo.Data.DoubleColumn)y, x);
      else
        throw new ArgumentException("Error: Try to apply Atan2() to " + y.TypeAndName + " and " + x.GetType(), "x");
    }

    /// <summary>
    /// Applies <see cref="Math.Atan2(double, double)"/> to a scalar and a data column.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Atan2(double y, Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Atan2(y, (Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Atan2() to " + y.GetType() + " and " + x.TypeAndName, "x");
    }

    /// <summary>
    /// Applies <see cref="Math.Ceiling(double)"/> to a data column.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Ceiling(Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Ceiling((Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Ceiling() to " + x.TypeAndName, "x");
    }

    /// <summary>
    /// Applies <see cref="Math.Cos(double)"/> to a data column.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Cos(Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Cos((Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Cos() to " + x.TypeAndName, "x");
    }

    /// <summary>
    /// Applies <see cref="Math.Cosh(double)"/> to a data column.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Cosh(Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Cosh((Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Cosh() to " + x.TypeAndName, "x");
    }

    /// <summary>
    /// Applies <see cref="Math.Exp(double)"/> to a data column.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Exp(Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Exp((Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Exp() to " + x.TypeAndName, "x");
    }

    /// <summary>
    /// Applies <see cref="Math.Floor(double)"/> to a data column.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Floor(Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Floor((Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Floor() to " + x.TypeAndName, "x");
    }

    /// <summary>
    /// Applies <see cref="Math.IEEERemainder(double, double)"/> to two data columns.
    /// </summary>
    public static Altaxo.Data.DoubleColumn IEEERemainder(Altaxo.Data.DataColumn x, Altaxo.Data.DataColumn y)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType() && typeof(Altaxo.Data.DoubleColumn) == y.GetType())
        return Altaxo.Data.DoubleColumn.IEEERemainder((Altaxo.Data.DoubleColumn)x, (Altaxo.Data.DoubleColumn)y);
      else
        throw new ArgumentException("Error: Try to apply IEEERemainder() to " + x.TypeAndName + " and " + y.TypeAndName, "x");
    }

    /// <summary>
    /// Applies <see cref="Math.IEEERemainder(double, double)"/> to a data column and a scalar.
    /// </summary>
    public static Altaxo.Data.DoubleColumn IEEERemainder(Altaxo.Data.DataColumn x, double y)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.IEEERemainder((Altaxo.Data.DoubleColumn)x, y);
      else
        throw new ArgumentException("Error: Try to apply IEEERemainder() to " + x.TypeAndName + " and " + y.GetType() + " " + y.ToString(), "x");
    }

    /// <summary>
    /// Applies <see cref="Math.IEEERemainder(double, double)"/> to a scalar and a data column.
    /// </summary>
    public static Altaxo.Data.DoubleColumn IEEERemainder(double x, Altaxo.Data.DataColumn y)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == y.GetType())
        return Altaxo.Data.DoubleColumn.IEEERemainder(x, (Altaxo.Data.DoubleColumn)y);
      else
        throw new ArgumentException("Error: Try to apply IEEERemainder() to " + x.GetType() + " " + x.ToString() + " and " + y.TypeAndName, "x");
    }

    /// <summary>
    /// Applies the natural logarithm to a data column.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Log(Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Log((Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Log() to " + x.TypeAndName, "x");
    }

    /// <summary>
    /// Applies logarithm evaluation to two data columns.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Log(Altaxo.Data.DataColumn x, Altaxo.Data.DataColumn y)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType() && typeof(Altaxo.Data.DoubleColumn) == y.GetType())
        return Altaxo.Data.DoubleColumn.Log((Altaxo.Data.DoubleColumn)x, (Altaxo.Data.DoubleColumn)y);
      else
        throw new ArgumentException("Error: Try to apply Log() to " + x.TypeAndName + " and " + y.TypeAndName, "x");
    }

    /// <summary>
    /// Applies logarithm evaluation to a data column and a scalar.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Log(Altaxo.Data.DataColumn x, double y)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Log((Altaxo.Data.DoubleColumn)x, y);
      else
        throw new ArgumentException("Error: Try to apply Log() to " + x.TypeAndName + " and " + y.GetType() + " " + y.ToString(), "x");
    }

    /// <summary>
    /// Applies logarithm evaluation to a scalar and a data column.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Log(double x, Altaxo.Data.DataColumn y)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == y.GetType())
        return Altaxo.Data.DoubleColumn.Log(x, (Altaxo.Data.DoubleColumn)y);
      else
        throw new ArgumentException("Error: Try to apply Log() to " + x.GetType() + " " + x.ToString() + " and " + y.TypeAndName, "x");
    }

    /// <summary>
    /// Applies the base-10 logarithm to a data column.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Log10(Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Log10((Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Log10() to " + x.TypeAndName, "x");
    }

    /// <summary>
    /// Computes the element-wise maximum of two data columns.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Max(Altaxo.Data.DataColumn x, Altaxo.Data.DataColumn y)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType() && typeof(Altaxo.Data.DoubleColumn) == y.GetType())
        return Altaxo.Data.DoubleColumn.Max((Altaxo.Data.DoubleColumn)x, (Altaxo.Data.DoubleColumn)y);
      else
        throw new ArgumentException("Error: Try to apply Max() to " + x.TypeAndName + " and " + y.TypeAndName, "x");
    }

    /// <summary>
    /// Computes the element-wise maximum of a data column and a scalar.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Max(Altaxo.Data.DataColumn x, double y)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Max((Altaxo.Data.DoubleColumn)x, y);
      else
        throw new ArgumentException("Error: Try to apply Max() to " + x.TypeAndName + " and " + y.GetType() + " " + y.ToString(), "x");
    }

    /// <summary>
    /// Computes the element-wise maximum of a scalar and a data column.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Max(double x, Altaxo.Data.DataColumn y)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == y.GetType())
        return Altaxo.Data.DoubleColumn.Max(x, (Altaxo.Data.DoubleColumn)y);
      else
        throw new ArgumentException("Error: Try to apply Max() to " + x.GetType() + " " + x.ToString() + " and " + y.TypeAndName, "x");
    }

    /// <summary>
    /// Computes the element-wise minimum of two data columns.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Min(Altaxo.Data.DataColumn x, Altaxo.Data.DataColumn y)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType() && typeof(Altaxo.Data.DoubleColumn) == y.GetType())
        return Altaxo.Data.DoubleColumn.Min((Altaxo.Data.DoubleColumn)x, (Altaxo.Data.DoubleColumn)y);
      else
        throw new ArgumentException("Error: Try to apply Min() to " + x.TypeAndName + " and " + y.TypeAndName, "x");
    }

    /// <summary>
    /// Computes the element-wise minimum of a data column and a scalar.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Min(Altaxo.Data.DataColumn x, double y)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Min((Altaxo.Data.DoubleColumn)x, y);
      else
        throw new ArgumentException("Error: Try to apply Min() to " + x.TypeAndName + " and " + y.GetType() + " " + y.ToString(), "x");
    }

    /// <summary>
    /// Computes the element-wise minimum of a scalar and a data column.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Min(double x, Altaxo.Data.DataColumn y)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == y.GetType())
        return Altaxo.Data.DoubleColumn.Min(x, (Altaxo.Data.DoubleColumn)y);
      else
        throw new ArgumentException("Error: Try to apply Min() to " + x.GetType() + " " + x.ToString() + " and " + y.TypeAndName, "x");
    }

    /// <summary>
    /// Applies element-wise power evaluation to two data columns.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Pow(Altaxo.Data.DataColumn x, Altaxo.Data.DataColumn y)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType() && typeof(Altaxo.Data.DoubleColumn) == y.GetType())
        return Altaxo.Data.DoubleColumn.Pow((Altaxo.Data.DoubleColumn)x, (Altaxo.Data.DoubleColumn)y);
      else
        throw new ArgumentException("Error: Try to apply Pow() to " + x.TypeAndName + " and " + y.TypeAndName, "x");
    }

    /// <summary>
    /// Raises each element of a data column to a scalar power.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Pow(Altaxo.Data.DataColumn x, double y)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Pow((Altaxo.Data.DoubleColumn)x, y);
      else
        throw new ArgumentException("Error: Try to apply Pow() to " + x.TypeAndName + " and " + y.GetType() + " " + y.ToString(), "x");
    }

    /// <summary>
    /// Raises a scalar base to element-wise powers from a data column.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Pow(double x, Altaxo.Data.DataColumn y)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == y.GetType())
        return Altaxo.Data.DoubleColumn.Pow(x, (Altaxo.Data.DoubleColumn)y);
      else
        throw new ArgumentException("Error: Try to apply Pow() to " + x.GetType() + " " + x.ToString() + " and " + y.TypeAndName, "x");
    }

    /// <summary>
    /// Rounds each element of a data column.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Round(Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Round((Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Round() to " + x.TypeAndName, "x");
    }

    /// <summary>
    /// Rounds elements of a data column using element-wise precision from another column.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Round(Altaxo.Data.DataColumn x, Altaxo.Data.DataColumn y)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType() && typeof(Altaxo.Data.DoubleColumn) == y.GetType())
        return Altaxo.Data.DoubleColumn.Round((Altaxo.Data.DoubleColumn)x, (Altaxo.Data.DoubleColumn)y);
      else
        throw new ArgumentException("Error: Try to apply Round() to " + x.TypeAndName + " and " + y.TypeAndName, "x");
    }

    /// <summary>
    /// Rounds each element of a data column to the specified number of digits.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Round(Altaxo.Data.DataColumn x, int y)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Round((Altaxo.Data.DoubleColumn)x, y);
      else
        throw new ArgumentException("Error: Try to apply Round() to " + x.TypeAndName + " and " + y.GetType() + " " + y.ToString(), "x");
    }

    /// <summary>
    /// Rounds a scalar using element-wise precision from a data column.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Round(double x, Altaxo.Data.DataColumn y)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == y.GetType())
        return Altaxo.Data.DoubleColumn.Round(x, (Altaxo.Data.DoubleColumn)y);
      else
        throw new ArgumentException("Error: Try to apply Round() to " + x.GetType() + " " + x.ToString() + " and " + y.TypeAndName, "x");
    }

    /// <summary>
    /// Applies <see cref="Math.Sign(double)"/> to a data column.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Sign(Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Sign((Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Sign() to " + x.TypeAndName, "x");
    }

    /// <summary>
    /// Applies <see cref="Math.Sin(double)"/> to a data column.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Sin(Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Sin((Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Sin() to " + x.TypeAndName, "x");
    }

    /// <summary>
    /// Applies <see cref="Math.Sinh(double)"/> to a data column.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Sinh(Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Sinh((Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Sinh() to " + x.TypeAndName, "x");
    }

    /// <summary>
    /// Applies <see cref="Math.Sqrt(double)"/> to a data column.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Sqrt(Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Sqrt((Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Sqrt() to " + x.TypeAndName, "x");
    }

    /// <summary>
    /// Applies <see cref="Math.Tan(double)"/> to a data column.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Tan(Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Tan((Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Tan() to " + x.TypeAndName, "x");
    }

    /// <summary>
    /// Applies <see cref="Math.Tanh(double)"/> to a data column.
    /// </summary>
    public static Altaxo.Data.DoubleColumn Tanh(Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Tanh((Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Tanh() to " + x.TypeAndName, "x");
    }

    #region DataColumn Mathematics (other)

    /// <summary>Calculates the 2nd power of each element of <paramref name="x"/> (square).</summary>
    /// <param name="x">A column containing numeric elements.</param>
    /// <returns>A new column in which each each element is the 2nd power of the corresponding element of column <paramref name="x"/>.</returns>
    public static Altaxo.Data.DoubleColumn Square(Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return ((Altaxo.Data.DoubleColumn)x) * ((Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Square() to " + x.TypeAndName, "x");
    }

    /// <summary>Calculates the 2nd power of each element of <paramref name="x"/> (square).</summary>
    /// <param name="x">A column containing numeric elements.</param>
    /// <returns>A new column in which each each element is the 2nd power of the corresponding element of column <paramref name="x"/>.</returns>
    public static Altaxo.Data.DoubleColumn Pow2(Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Pow2((Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Pow2(x) to " + x.TypeAndName);
    }

    /// <summary>Calculates the 3rd power of each element of <paramref name="x"/> (cube).</summary>
    /// <param name="x">A column containing numeric elements.</param>
    /// <returns>A new column in which each each element is the 3rd power of the corresponding element of column <paramref name="x"/>.</returns>
    public static Altaxo.Data.DoubleColumn Pow3(Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Pow3((Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Pow3(x) to " + x.TypeAndName);
    }

    /// <summary>Calculates the 4th power of each element of <paramref name="x"/>.</summary>
    /// <param name="x">A column containing numeric elements.</param>
    /// <returns>A new column in which each each element is the 4th power of the corresponding element of column <paramref name="x"/>.</returns>
    public static Altaxo.Data.DoubleColumn Pow4(Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Pow4((Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Pow4(x) to " + x.TypeAndName);
    }

    /// <summary>Calculates the 5th power of each element of <paramref name="x"/>.</summary>
    /// <param name="x">A column containing numeric elements.</param>
    /// <returns>A new column in which each each element is the 5th power of the corresponding element of column <paramref name="x"/>.</returns>
    public static Altaxo.Data.DoubleColumn Pow5(Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Pow5((Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Pow5(x) to " + x.TypeAndName);
    }

    /// <summary>Calculates the 6th power of each element of <paramref name="x"/>.</summary>
    /// <param name="x">A column containing numeric elements.</param>
    /// <returns>A new column in which each each element is the 6th power of the corresponding element of column <paramref name="x"/>.</returns>
    public static Altaxo.Data.DoubleColumn Pow6(Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Pow6((Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Pow6(x) to " + x.TypeAndName);
    }

    /// <summary>Calculates the 7th power of each element of <paramref name="x"/>.</summary>
    /// <param name="x">A column containing numeric elements.</param>
    /// <returns>A new column in which each each element is the 7th power of the corresponding element of column <paramref name="x"/>.</returns>
    public static Altaxo.Data.DoubleColumn Pow7(Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Pow7((Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Pow7(x) to " + x.TypeAndName);
    }

    /// <summary>Calculates the 8th power of each element of <paramref name="x"/>.</summary>
    /// <param name="x">A column containing numeric elements.</param>
    /// <returns>A new column in which each each element is the 8th power of the corresponding element of column <paramref name="x"/>.</returns>
    public static Altaxo.Data.DoubleColumn Pow8(Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Pow8((Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Pow8(x) to " + x.TypeAndName);
    }

    /// <summary>Calculates the 9th power of each element of <paramref name="x"/>.</summary>
    /// <param name="x">A column containing numeric elements.</param>
    /// <returns>A new column in which each each element is the 9th power of the corresponding element of column <paramref name="x"/>.</returns>
    public static Altaxo.Data.DoubleColumn Pow9(Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Pow9((Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Pow9(x) to " + x.TypeAndName);
    }

    /// <summary>Calculates the <paramref name="i"/>th power of each element of <paramref name="x"/>.</summary>
    /// <param name="x">A column containing numeric elements.</param>
    /// <param name="i">An integer number that specifies the power.</param>
    /// <returns>A new column in which each each element is the <paramref name="i"/>th power of the corresponding element of column <paramref name="x"/>.</returns>
    public static Altaxo.Data.DoubleColumn Pow(Altaxo.Data.DataColumn x, int i)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Pow((Altaxo.Data.DoubleColumn)x, i);
      else
        throw new ArgumentException("Error: Try to apply Pow(x,i) to " + x.TypeAndName + " and " + i.GetType() + " " + i.ToString());
    }

    /// <summary>Applies the specified unary <paramref name="function"/> to each element in column <paramref name="x"/>.</summary>
    /// <param name="function">The function to apply.</param>
    /// <param name="x">The column containing the numerical elements to which the specified <paramref name="function"/> should be applied.</param>
    /// <returns>A new column in which each element y[i] is the function applied to the element x[i], i.e. y[i] = function(x[i]).</returns>
    public static Altaxo.Data.DoubleColumn Map(Func<double, double> function, Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Map(function, (Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Map(Func<double,double>, x) to " + x.TypeAndName);
    }

    /// <summary>Applies the specified binary <paramref name="function"/> to each element in column <paramref name="x"/> and column <paramref name="y"/>.</summary>
    /// <param name="function">The function to apply.</param>
    /// <param name="x">The column containing the numerical elements to which the specified <paramref name="function"/> should be applied (as first argument of that function).</param>
    /// <param name="y">The column containing the numerical elements to which the specified <paramref name="function"/> should be applied (as second argument of that function).</param>
    /// <returns>A new column in which each element y[i] is the function applied to the elements x[i] and y[i], i.e. y[i] = function(x[i], y[i]).</returns>
    public static Altaxo.Data.DoubleColumn Map(Func<double, double, double> function, Altaxo.Data.DataColumn x, Altaxo.Data.DataColumn y)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType() && typeof(Altaxo.Data.DoubleColumn) == y.GetType())
        return Altaxo.Data.DoubleColumn.Map(function, (Altaxo.Data.DoubleColumn)x, (Altaxo.Data.DoubleColumn)y);
      else
        throw new ArgumentException("Error: Try to apply Map(Func<double,double,double>, x, y) to " + x.TypeAndName + " and " + y.TypeAndName, "x");
    }

    /// <summary>Applies the specified binary <paramref name="function"/> to each element in column <paramref name="x"/> and to paramenter <paramref name="y"/>.</summary>
    /// <param name="function">The function to apply.</param>
    /// <param name="x">The column containing the numerical elements to which the specified <paramref name="function"/> should be applied (as first argument of that function).</param>
    /// <param name="y">A double-precision numeric value used as second argument of that function.</param>
    /// <returns>A new column in which each element y[i] is the function applied to the element x[i] and argument y, i.e. y[i] = function(x[i], y).</returns>
    public static Altaxo.Data.DoubleColumn Map(Func<double, double, double> function, Altaxo.Data.DataColumn x, double y)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Map(function, (Altaxo.Data.DoubleColumn)x, y);
      else
        throw new ArgumentException("Error: Try to apply Map(Func<double,double,double>, x, y) to " + x.TypeAndName + " and " + "double", "x");
    }

    /// <summary>Applies the specified binary <paramref name="function"/> to the specified number <paramref name="x"/> and to each element in column <paramref name="x"/>.</summary>
    /// <param name="function">The function to apply.</param>
    /// <param name="x">A double-precision numeric value used as first argument of that function.</param>
    /// <param name="y">The column containing the numerical elements to which the specified <paramref name="function"/> should be applied (as second argument of that function).</param>
    /// <returns>A new column in which each element y[i] is the function applied to the specified value <paramref name="x"/> and the element y[i], i.e. y[i] = function(x, y[i]).</returns>
    public static Altaxo.Data.DoubleColumn Map(Func<double, double, double> function, double x, Altaxo.Data.DataColumn y)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == y.GetType())
        return Altaxo.Data.DoubleColumn.Map(function, x, (Altaxo.Data.DoubleColumn)y);
      else
        throw new ArgumentException("Error: Try to apply Map(Func<double,double,double>, x, y) to " + "double" + " and " + y.TypeAndName, "x");
    }

    #endregion DataColumn Mathematics (other)

    #endregion AltaxoDataColumn_Mathematics

    #region Other helper functions

    /// <summary>
    /// Combines a path with a name to form a full name.
    /// </summary>
    /// <param name="directoryPart">Path. Can be null (in this case only the name is returned).</param>
    /// <param name="namePart">Name.</param>
    /// <returns>Full name as combination of path with a DirectorySeparatorChar with name.</returns>
    public string CombinePath(string directoryPart, string namePart)
    {
      return Main.ProjectFolder.Combine(directoryPart, namePart);
    }

    #endregion Other helper functions
  } // end of class ScriptExecutionBase

  /// <summary>
  /// ColScriptExeBase is the base class of all column scripts.
  /// </summary>
  /// <remarks>
  /// Every column script defines his own class, which is derived from here.
  /// There is only one method in ColScriptExeBase, Execute, which has
  /// to be overwritten by the column script in order to execute the column script.
  /// The method provided here is not defined as abstract, but instead does nothing.
  /// </remarks>
  public class ColScriptExeBase : ScriptExecutionBase
  {
    /// <summary>
    /// Version1 of the execute method. This method must be overriden by the column script in order to be able to execute the script.
    /// This method is the entry point of the column script
    /// </summary>
    /// <param name="myColumn">The column on which the column script is executed.</param>
    [Obsolete]
    public virtual void Execute(Altaxo.Data.DataColumn myColumn)
    {
    }

    /// <summary>
    /// Version 2 of the execute method. This method must be overriden by the column script in order to be able to execute the script.
    /// This method is the entry point of the column script. In order to maintain backward compatibility, if not overridden, this method calls the version 1 of the execute method.
    /// </summary>
    /// <param name="myColumn">The column on which the column script is executed.</param>
    /// <param name="reporter">Progress reporter that can be used by the script to report the progress of its work.</param>
    public virtual void Execute(Altaxo.Data.DataColumn myColumn, IProgressReporter reporter)
    {
#pragma warning disable  // calling the obsolete method is intended here
      Execute(myColumn);
#pragma warning restore
    }
  }

  /// <summary>
  /// TableScriptExeBase is the base class of all table scripts.
  /// </summary>
  /// <remarks>
  /// Every table script defines his own class, which is derived from here.
  /// There is only one method in TableScriptExeBase, Execute, which has
  /// to be overwritten by the table script in order to execute the script.
  /// The method provided here is not defined as abstract, but instead does nothing.
  /// </remarks>
  public class TableScriptExeBase : ScriptExecutionBase
  {
    /// <summary>
    /// Version 1 of the execute method. Obsole, so please use Version 2 now.
    /// </summary>
    /// <param name="myTable">The table on which the table script is executed.</param>
    public virtual void Execute(Altaxo.Data.DataTable myTable)
    {
    }

    /// <summary>
    /// Version 2 of the execute method. This method which must be overriden by the table script in order to be able to execute the script.
    /// This method is the entry point of the table script.In order to maintain backward compatibility, if not overridden, this method calls the version 1 of the execute method.
    /// </summary>
    /// <param name="myTable">The table on which the table script is executed.</param>
    /// <param name="reporter">Progress reporter that can be used by the script to report the progress of its work.</param>
    public virtual void Execute(Altaxo.Data.DataTable myTable, Altaxo.IProgressReporter reporter)
    {
      Execute(myTable);
    }
  }

  /// <summary>
  /// TableScriptExeBase is the base class of all table scripts.
  /// </summary>
  /// <remarks>
  /// Every table script defines his own class, which is derived from here.
  /// There is only one method in TableScriptExeBase, Execute, which has
  /// to be overwritten by the table script in order to execute the script.
  /// The method provided here is not defined as abstract, but instead does nothing.
  /// </remarks>
  public class FileImportScriptExeBase : ScriptExecutionBase
  {
    /// <summary>
    /// This method must be overriden by the import script in order to be able to execute the script.
    /// This method is the entry point of the import script.
    /// </summary>
    /// <param name="myTable">The table on which the table script is executed.</param>
    /// <param name="fileNames">The file name(s) of the file(s) to import.</param>
    /// <param name="reporter">Progress reporter that can be used by the script to report the progress of its work.</param>
    public virtual void Execute(Altaxo.Data.DataTable myTable, IReadOnlyList<string> fileNames, Altaxo.IProgressReporter reporter)
    {
    }

    /// <summary>
    /// Returns true if this import script can accept multiple files for one table.
    /// </summary>
    public virtual bool CanAcceptMultipleFiles { get { return false; } }

    /// <summary>
    /// Returns the file filters.
    /// </summary>
    public virtual (string Filter, string Description)[] FileFilters { get; } = new[] { ("*.*", "All files (*.*)") };
  }

  /// <summary>
  /// Base class for scripts that process multiple source tables into a destination table.
  /// </summary>
  public class ProcessSourceTablesScriptExeBase : ScriptExecutionBase
  {
    /// <summary>
    /// This method must be overriden by the import script in order to be able to execute the script.
    /// This method is the entry point of the import script.
    /// </summary>
    /// <param name="myTable">The table on which the table script is executed.</param>
    /// <param name="sourceTables">The list of source tables that should be processed by this script.</param>
    /// <param name="reporter">Progress reporter that can be used by the script to report the progress of its work.</param>
    public virtual void Execute(Altaxo.Data.DataTable myTable, IReadOnlyListDictionary<string, DataTable> sourceTables, Altaxo.IProgressReporter reporter)
    {
    }
  }


  /// <summary>
  /// Base class for a program instance script.
  /// </summary>
  public class ProgramInstanceExeBase : ScriptExecutionBase
  {
    /// <summary>
    /// Version 1 of the execute method. This method which must be overriden by the script in order to be able to execute the script.
    /// This method is the entry point of the  script
    /// </summary>
    [Obsolete("Please use version 2 of the Execute method")]
    public virtual void Execute()
    {
    }

    /// <summary>
    /// Version 2 of the execute method. This method which must be overriden by the script in order to be able to execute the script.
    /// This method is the entry point of the  script
    /// </summary>
    /// <param name="reporter">Can be used to report the execution progress of the script.</param>
    public virtual void Execute(IProgressReporter reporter)
    {
#pragma warning disable // calling the obsolete method is intended here
      Execute();
#pragma warning restore
    }
  }

  /// <summary>
  /// Base class of all "extract table values "table scripts.
  /// </summary>
  /// <remarks>
  /// Every table script defines his own class, which is derived from here.
  /// There is only one method , IsRowIncluded, which has
  /// to be overwritten by the table script in order to execute the script.
  /// The method provided here is not defined as abstract, but instead does nothing.
  /// </remarks>
  public class ExtractTableValuesExeBase : ScriptExecutionBase
  {
    /// <summary>
    /// This method which must be overriden by the extract table data script in order to be able to execute the script.
    /// This method is the entry point of the table script
    /// </summary>
    /// <param name="myTable">The table on which the table script is executed.</param>
    /// <param name="i">The row number of the data column collection, for which to determine if that row should be extracted or not.</param>
    /// <returns>True if that row should be extracted, false if it should not be extracted.</returns>
    public virtual bool IsRowIncluded(Altaxo.Data.DataTable myTable, int i)
    {
      return false;
    }
  }

  /// <summary>
  /// Base class for script-defined fit functions.
  /// </summary>
  public abstract class FitFunctionExeBase : ScriptExecutionBase, Altaxo.Calc.Regression.Nonlinear.IFitFunction
  {
    private static readonly string[] _emptyStringArray = new string[0];
    /// <summary>
    /// The names of the independent variables.
    /// </summary>
    protected string[] _independentVariableNames = _emptyStringArray;

    /// <summary>
    /// The names of the dependent variables.
    /// </summary>
    protected string[] _dependentVariableNames = _emptyStringArray;

    /// <summary>
    /// The names of the parameters.
    /// </summary>
    protected string[] _parameterNames = _emptyStringArray;

    /// <summary>
    /// Number of independent variables (i.e. x).
    /// </summary>
    public virtual int NumberOfIndependentVariables
    {
      get
      {
        return _independentVariableNames.Length;
      }
    }

    /// <summary>
    /// Number of dependent variables (i.e. y, in Altaxo this is commonly called v like value).
    /// </summary>
    public virtual int NumberOfDependentVariables
    {
      get
      {
        return _dependentVariableNames.Length;
      }
    }

    /// <summary>
    /// Number of parameters of this fit function.
    /// </summary>
    public virtual int NumberOfParameters
    {
      get
      {
        return _parameterNames.Length;
      }
    }

    /// <summary>
    /// Returns the ith independent variable name.
    /// </summary>
    /// <param name="i">Index of the independent variable.</param>
    /// <returns>The name of the ith independent variable.</returns>
    public virtual string IndependentVariableName(int i)
    {
      return _independentVariableNames[i];
    }

    /// <summary>
    /// Returns the ith dependent variable name.
    /// </summary>
    /// <param name="i">Index of the dependent variable.</param>
    /// <returns>The name of the ith dependent variable.</returns>
    public virtual string DependentVariableName(int i)
    {
      return _dependentVariableNames[i];
    }

    /// <summary>
    /// Returns the ith parameter name.
    /// </summary>
    /// <param name="i">Index of the parameter.</param>
    /// <returns>The name of the ith paramter.</returns>
    public virtual string ParameterName(int i)
    {
      return _parameterNames[i];
    }

    /// <summary>
    /// Returns the default parameter value of parameter <c>i</c>. Defaults to return 0. Scripts can
    /// override this function in order to provide more useful values.
    /// </summary>
    /// <param name="i">Index of parameter.</param>
    /// <returns>Returns 0 (zero) by default.</returns>
    public virtual double DefaultParameterValue(int i)
    {
      return 0;
    }

    /// <summary>
    /// Returns the default variance scaling for dependent variable <c>i</c>. Default returns <c>null</c>, which
    /// means a constant weight of 1 is assumed.
    /// </summary>
    /// <param name="i">Index of dependent variable.</param>
    /// <returns>Null by default. You can override this behaviour.</returns>
    public virtual Altaxo.Calc.Regression.Nonlinear.IVarianceScaling? DefaultVarianceScaling(int i)
    {
      return null;
    }

    /// <summary>
    /// This evaluates a function value.
    /// </summary>
    /// <param name="independent">The independent variables.</param>
    /// <param name="parameters">Parameters for evaluation.</param>
    /// <param name="result">On return, this array contains the one (or more) evaluated
    /// function values at the point (independent).</param>
    public abstract void Evaluate(double[] independent, double[] parameters, double[] result);

    /// <inheritdoc/>
    public virtual void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> P, IVector<double> FV, IReadOnlyList<bool>? dependentVariableChoice)
    {
      var xx = new double[NumberOfIndependentVariables];
      var yy = new double[NumberOfDependentVariables];
      var pp = P.ToArray();

      var rowCount = independent.RowCount;
      var colCount = independent.ColumnCount;
      int rd = 0;
      for (int r = 0; r < rowCount; ++r)
      {
        for (int c = 0; c < colCount; ++c)
        {
          xx[c] = independent[r, c];
        }

        Evaluate(xx, pp, yy);

        if (dependentVariableChoice is null)
        {
          for (int c = 0; c < yy.Length; ++c)
          {
            FV[rd++] = yy[c];
          }
        }
        else
        {
          for (int c = 0; c < yy.Length; ++c)
          {
            if (dependentVariableChoice[c] == true)
            {
              FV[rd++] = yy[c];
            }
          }
        }
      }
    }


    #region Change event

    /// <summary>
    /// Called when anything in this fit function has changed.
    /// </summary>
    protected virtual void OnChanged()
    {
      Changed?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Fired when the fit function changed.
    /// </summary>
    public event EventHandler? Changed;

    #endregion Change event

    /// <inheritdoc/>
    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesHardLimit()
    {
      return (null, null);
    }

    /// <inheritdoc/>
    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesSoftLimit()
    {
      return (null, null);
    }
  }

  /// <summary>
  /// Base class of all function evaluation scripts.
  /// </summary>
  public class FunctionEvaluationScriptBase : ScriptExecutionBase
  {
    /// <summary>
    /// This method which must be overriden by the function evaluation script in order to be able to execute the script.
    /// </summary>
    /// <param name="x">The x value for which the function is evaluated.</param>
    /// <returns>The calculated y value.</returns>
    public virtual double EvaluateFunctionValue(double x)
    {
      return double.NaN;
    }
  }
}
