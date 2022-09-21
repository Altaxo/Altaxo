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
    public const double E = System.Math.E;

    public const double PI = System.Math.PI;

    public static double Abs(double s)
    {
      return System.Math.Abs(s);
    }

    public static double Acos(double s)
    {
      return System.Math.Acos(s);
    }

    public static double Asin(double s)
    {
      return System.Math.Asin(s);
    }

    public static double Atan(double s)
    {
      return System.Math.Atan(s);
    }

    public static double Atan2(double y, double x)
    {
      return System.Math.Atan2(y, x);
    }

    public static double Ceiling(double s)
    {
      return System.Math.Ceiling(s);
    }

    public static double Cos(double s)
    {
      return System.Math.Cos(s);
    }

    public static double Cosh(double s)
    {
      return System.Math.Cosh(s);
    }

    public static double Exp(double s)
    {
      return System.Math.Exp(s);
    }

    public static double Floor(double s)
    {
      return System.Math.Floor(s);
    }

    public static double IEEERemainder(double x, double y)
    {
      return System.Math.IEEERemainder(x, y);
    }

    public static double Log(double s)
    {
      return System.Math.Log(s);
    }

    public static double Log10(double s)
    {
      return System.Math.Log10(s);
    }

    public static double Log(double s, double bas)
    {
      return System.Math.Log(s, bas);
    }

    public static double Max(double x, double y)
    {
      return System.Math.Max(x, y);
    }

    public static double Min(double x, double y)
    {
      return System.Math.Min(x, y);
    }

    public static double Pow(double x, double y)
    {
      return System.Math.Pow(x, y);
    }

    public static double Round(double x)
    {
      return System.Math.Round(x);
    }

    public static double Round(double x, int i)
    {
      return System.Math.Round(x, i);
    }

    public static double Sign(double s)
    {
      return System.Math.Sign(s);
    }

    public static double Sin(double s)
    {
      return System.Math.Sin(s);
    }

    public static double Sinh(double s)
    {
      return System.Math.Sinh(s);
    }

    public static double Sqrt(double s)
    {
      return System.Math.Sqrt(s);
    }

    public static double Tan(double s)
    {
      return System.Math.Tan(s);
    }

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

    public static Altaxo.Data.DoubleColumn Abs(Altaxo.Data.DoubleColumn s)
    {
      return Altaxo.Data.DoubleColumn.Abs(s);
    }

    public static Altaxo.Data.DoubleColumn Acos(Altaxo.Data.DoubleColumn s)
    {
      return Altaxo.Data.DoubleColumn.Acos(s);
    }

    public static Altaxo.Data.DoubleColumn Asin(Altaxo.Data.DoubleColumn s)
    {
      return Altaxo.Data.DoubleColumn.Asin(s);
    }

    public static Altaxo.Data.DoubleColumn Atan(Altaxo.Data.DoubleColumn s)
    {
      return Altaxo.Data.DoubleColumn.Atan(s);
    }

    public static Altaxo.Data.DoubleColumn Atan2(Altaxo.Data.DoubleColumn y, Altaxo.Data.DoubleColumn x)
    {
      return Altaxo.Data.DoubleColumn.Atan2(y, x);
    }

    public static Altaxo.Data.DoubleColumn Atan2(Altaxo.Data.DoubleColumn y, double x)
    {
      return Altaxo.Data.DoubleColumn.Atan2(y, x);
    }

    public static Altaxo.Data.DoubleColumn Atan2(double y, Altaxo.Data.DoubleColumn x)
    {
      return Altaxo.Data.DoubleColumn.Atan2(y, x);
    }

    public static Altaxo.Data.DoubleColumn Ceiling(Altaxo.Data.DoubleColumn s)
    {
      return Altaxo.Data.DoubleColumn.Ceiling(s);
    }

    public static Altaxo.Data.DoubleColumn Cos(Altaxo.Data.DoubleColumn s)
    {
      return Altaxo.Data.DoubleColumn.Cos(s);
    }

    public static Altaxo.Data.DoubleColumn Cosh(Altaxo.Data.DoubleColumn s)
    {
      return Altaxo.Data.DoubleColumn.Cosh(s);
    }

    public static Altaxo.Data.DoubleColumn Exp(Altaxo.Data.DoubleColumn s)
    {
      return Altaxo.Data.DoubleColumn.Exp(s);
    }

    public static Altaxo.Data.DoubleColumn Floor(Altaxo.Data.DoubleColumn s)
    {
      return Altaxo.Data.DoubleColumn.Floor(s);
    }

    public static Altaxo.Data.DoubleColumn IEEERemainder(Altaxo.Data.DoubleColumn x, Altaxo.Data.DoubleColumn y)
    {
      return Altaxo.Data.DoubleColumn.IEEERemainder(x, y);
    }

    public static Altaxo.Data.DoubleColumn IEEERemainder(Altaxo.Data.DoubleColumn x, double y)
    {
      return Altaxo.Data.DoubleColumn.IEEERemainder(x, y);
    }

    public static Altaxo.Data.DoubleColumn IEEERemainder(double x, Altaxo.Data.DoubleColumn y)
    {
      return Altaxo.Data.DoubleColumn.IEEERemainder(x, y);
    }

    public static Altaxo.Data.DoubleColumn Log(Altaxo.Data.DoubleColumn s)
    {
      return Altaxo.Data.DoubleColumn.Log(s);
    }

    public static Altaxo.Data.DoubleColumn Log(Altaxo.Data.DoubleColumn s, Altaxo.Data.DoubleColumn bas)
    {
      return Altaxo.Data.DoubleColumn.Log(s, bas);
    }

    public static Altaxo.Data.DoubleColumn Log(Altaxo.Data.DoubleColumn s, double bas)
    {
      return Altaxo.Data.DoubleColumn.Log(s, bas);
    }

    public static Altaxo.Data.DoubleColumn Log(double s, Altaxo.Data.DoubleColumn bas)
    {
      return Altaxo.Data.DoubleColumn.Log(s, bas);
    }

    public static Altaxo.Data.DoubleColumn Log10(Altaxo.Data.DoubleColumn s)
    {
      return Altaxo.Data.DoubleColumn.Log10(s);
    }

    public static Altaxo.Data.DoubleColumn Max(Altaxo.Data.DoubleColumn x, Altaxo.Data.DoubleColumn y)
    {
      return Altaxo.Data.DoubleColumn.Max(x, y);
    }

    public static Altaxo.Data.DoubleColumn Max(Altaxo.Data.DoubleColumn x, double y)
    {
      return Altaxo.Data.DoubleColumn.Max(x, y);
    }

    public static Altaxo.Data.DoubleColumn Max(double x, Altaxo.Data.DoubleColumn y)
    {
      return Altaxo.Data.DoubleColumn.Max(x, y);
    }

    public static Altaxo.Data.DoubleColumn Min(Altaxo.Data.DoubleColumn x, Altaxo.Data.DoubleColumn y)
    {
      return Altaxo.Data.DoubleColumn.Min(x, y);
    }

    public static Altaxo.Data.DoubleColumn Min(Altaxo.Data.DoubleColumn x, double y)
    {
      return Altaxo.Data.DoubleColumn.Min(x, y);
    }

    public static Altaxo.Data.DoubleColumn Min(double x, Altaxo.Data.DoubleColumn y)
    {
      return Altaxo.Data.DoubleColumn.Min(x, y);
    }

    public static Altaxo.Data.DoubleColumn Pow(Altaxo.Data.DoubleColumn x, Altaxo.Data.DoubleColumn y)
    {
      return Altaxo.Data.DoubleColumn.Pow(x, y);
    }

    public static Altaxo.Data.DoubleColumn Pow(Altaxo.Data.DoubleColumn x, double y)
    {
      return Altaxo.Data.DoubleColumn.Pow(x, y);
    }

    public static Altaxo.Data.DoubleColumn Pow(double x, Altaxo.Data.DoubleColumn y)
    {
      return Altaxo.Data.DoubleColumn.Pow(x, y);
    }

    public static Altaxo.Data.DoubleColumn Round(Altaxo.Data.DoubleColumn x)
    {
      return Altaxo.Data.DoubleColumn.Round(x);
    }

    public static Altaxo.Data.DoubleColumn Round(Altaxo.Data.DoubleColumn x, Altaxo.Data.DoubleColumn i)
    {
      return Altaxo.Data.DoubleColumn.Round(x, i);
    }

    public static Altaxo.Data.DoubleColumn Round(Altaxo.Data.DoubleColumn x, int i)
    {
      return Altaxo.Data.DoubleColumn.Round(x, i);
    }

    public static Altaxo.Data.DoubleColumn Round(double x, Altaxo.Data.DoubleColumn i)
    {
      return Altaxo.Data.DoubleColumn.Round(x, i);
    }

    public static Altaxo.Data.DoubleColumn Sign(Altaxo.Data.DoubleColumn s)
    {
      return Altaxo.Data.DoubleColumn.Sign(s);
    }

    public static Altaxo.Data.DoubleColumn Sin(Altaxo.Data.DoubleColumn s)
    {
      return Altaxo.Data.DoubleColumn.Sin(s);
    }

    public static Altaxo.Data.DoubleColumn Sinh(Altaxo.Data.DoubleColumn s)
    {
      return Altaxo.Data.DoubleColumn.Sinh(s);
    }

    public static Altaxo.Data.DoubleColumn Sqrt(Altaxo.Data.DoubleColumn s)
    {
      return Altaxo.Data.DoubleColumn.Sqrt(s);
    }

    public static Altaxo.Data.DoubleColumn Tan(Altaxo.Data.DoubleColumn s)
    {
      return Altaxo.Data.DoubleColumn.Tan(s);
    }

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
    public static Altaxo.Data.DoubleColumn Abs(Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Abs((Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Abs() to " + x.TypeAndName, "x");
    }

    public static Altaxo.Data.DoubleColumn Acos(Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Acos((Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Acos() to " + x.TypeAndName, "x");
    }

    public static Altaxo.Data.DoubleColumn Asin(Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Asin((Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Asin() to " + x.TypeAndName, "x");
    }

    public static Altaxo.Data.DoubleColumn Atan(Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Atan((Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Atan() to " + x.TypeAndName, "x");
    }

    public static Altaxo.Data.DoubleColumn Atan2(Altaxo.Data.DataColumn y, Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == y.GetType() && typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Atan2((Altaxo.Data.DoubleColumn)y, (Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Atan2() to " + y.TypeAndName + " and " + x.TypeAndName, "x");
    }

    public static Altaxo.Data.DoubleColumn Atan2(Altaxo.Data.DataColumn y, double x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == y.GetType())
        return Altaxo.Data.DoubleColumn.Atan2((Altaxo.Data.DoubleColumn)y, x);
      else
        throw new ArgumentException("Error: Try to apply Atan2() to " + y.TypeAndName + " and " + x.GetType(), "x");
    }

    public static Altaxo.Data.DoubleColumn Atan2(double y, Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Atan2(y, (Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Atan2() to " + y.GetType() + " and " + x.TypeAndName, "x");
    }

    public static Altaxo.Data.DoubleColumn Ceiling(Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Ceiling((Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Ceiling() to " + x.TypeAndName, "x");
    }

    public static Altaxo.Data.DoubleColumn Cos(Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Cos((Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Cos() to " + x.TypeAndName, "x");
    }

    public static Altaxo.Data.DoubleColumn Cosh(Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Cosh((Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Cosh() to " + x.TypeAndName, "x");
    }

    public static Altaxo.Data.DoubleColumn Exp(Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Exp((Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Exp() to " + x.TypeAndName, "x");
    }

    public static Altaxo.Data.DoubleColumn Floor(Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Floor((Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Floor() to " + x.TypeAndName, "x");
    }

    public static Altaxo.Data.DoubleColumn IEEERemainder(Altaxo.Data.DataColumn x, Altaxo.Data.DataColumn y)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType() && typeof(Altaxo.Data.DoubleColumn) == y.GetType())
        return Altaxo.Data.DoubleColumn.IEEERemainder((Altaxo.Data.DoubleColumn)x, (Altaxo.Data.DoubleColumn)y);
      else
        throw new ArgumentException("Error: Try to apply IEEERemainder() to " + x.TypeAndName + " and " + y.TypeAndName, "x");
    }

    public static Altaxo.Data.DoubleColumn IEEERemainder(Altaxo.Data.DataColumn x, double y)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.IEEERemainder((Altaxo.Data.DoubleColumn)x, y);
      else
        throw new ArgumentException("Error: Try to apply IEEERemainder() to " + x.TypeAndName + " and " + y.GetType() + " " + y.ToString(), "x");
    }

    public static Altaxo.Data.DoubleColumn IEEERemainder(double x, Altaxo.Data.DataColumn y)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == y.GetType())
        return Altaxo.Data.DoubleColumn.IEEERemainder(x, (Altaxo.Data.DoubleColumn)y);
      else
        throw new ArgumentException("Error: Try to apply IEEERemainder() to " + x.GetType() + " " + x.ToString() + " and " + y.TypeAndName, "x");
    }

    public static Altaxo.Data.DoubleColumn Log(Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Log((Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Log() to " + x.TypeAndName, "x");
    }

    public static Altaxo.Data.DoubleColumn Log(Altaxo.Data.DataColumn x, Altaxo.Data.DataColumn y)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType() && typeof(Altaxo.Data.DoubleColumn) == y.GetType())
        return Altaxo.Data.DoubleColumn.Log((Altaxo.Data.DoubleColumn)x, (Altaxo.Data.DoubleColumn)y);
      else
        throw new ArgumentException("Error: Try to apply Log() to " + x.TypeAndName + " and " + y.TypeAndName, "x");
    }

    public static Altaxo.Data.DoubleColumn Log(Altaxo.Data.DataColumn x, double y)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Log((Altaxo.Data.DoubleColumn)x, y);
      else
        throw new ArgumentException("Error: Try to apply Log() to " + x.TypeAndName + " and " + y.GetType() + " " + y.ToString(), "x");
    }

    public static Altaxo.Data.DoubleColumn Log(double x, Altaxo.Data.DataColumn y)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == y.GetType())
        return Altaxo.Data.DoubleColumn.Log(x, (Altaxo.Data.DoubleColumn)y);
      else
        throw new ArgumentException("Error: Try to apply Log() to " + x.GetType() + " " + x.ToString() + " and " + y.TypeAndName, "x");
    }

    public static Altaxo.Data.DoubleColumn Log10(Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Log10((Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Log10() to " + x.TypeAndName, "x");
    }

    public static Altaxo.Data.DoubleColumn Max(Altaxo.Data.DataColumn x, Altaxo.Data.DataColumn y)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType() && typeof(Altaxo.Data.DoubleColumn) == y.GetType())
        return Altaxo.Data.DoubleColumn.Max((Altaxo.Data.DoubleColumn)x, (Altaxo.Data.DoubleColumn)y);
      else
        throw new ArgumentException("Error: Try to apply Max() to " + x.TypeAndName + " and " + y.TypeAndName, "x");
    }

    public static Altaxo.Data.DoubleColumn Max(Altaxo.Data.DataColumn x, double y)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Max((Altaxo.Data.DoubleColumn)x, y);
      else
        throw new ArgumentException("Error: Try to apply Max() to " + x.TypeAndName + " and " + y.GetType() + " " + y.ToString(), "x");
    }

    public static Altaxo.Data.DoubleColumn Max(double x, Altaxo.Data.DataColumn y)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == y.GetType())
        return Altaxo.Data.DoubleColumn.Max(x, (Altaxo.Data.DoubleColumn)y);
      else
        throw new ArgumentException("Error: Try to apply Max() to " + x.GetType() + " " + x.ToString() + " and " + y.TypeAndName, "x");
    }

    public static Altaxo.Data.DoubleColumn Min(Altaxo.Data.DataColumn x, Altaxo.Data.DataColumn y)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType() && typeof(Altaxo.Data.DoubleColumn) == y.GetType())
        return Altaxo.Data.DoubleColumn.Min((Altaxo.Data.DoubleColumn)x, (Altaxo.Data.DoubleColumn)y);
      else
        throw new ArgumentException("Error: Try to apply Min() to " + x.TypeAndName + " and " + y.TypeAndName, "x");
    }

    public static Altaxo.Data.DoubleColumn Min(Altaxo.Data.DataColumn x, double y)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Min((Altaxo.Data.DoubleColumn)x, y);
      else
        throw new ArgumentException("Error: Try to apply Min() to " + x.TypeAndName + " and " + y.GetType() + " " + y.ToString(), "x");
    }

    public static Altaxo.Data.DoubleColumn Min(double x, Altaxo.Data.DataColumn y)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == y.GetType())
        return Altaxo.Data.DoubleColumn.Min(x, (Altaxo.Data.DoubleColumn)y);
      else
        throw new ArgumentException("Error: Try to apply Min() to " + x.GetType() + " " + x.ToString() + " and " + y.TypeAndName, "x");
    }

    public static Altaxo.Data.DoubleColumn Pow(Altaxo.Data.DataColumn x, Altaxo.Data.DataColumn y)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType() && typeof(Altaxo.Data.DoubleColumn) == y.GetType())
        return Altaxo.Data.DoubleColumn.Pow((Altaxo.Data.DoubleColumn)x, (Altaxo.Data.DoubleColumn)y);
      else
        throw new ArgumentException("Error: Try to apply Pow() to " + x.TypeAndName + " and " + y.TypeAndName, "x");
    }

    public static Altaxo.Data.DoubleColumn Pow(Altaxo.Data.DataColumn x, double y)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Pow((Altaxo.Data.DoubleColumn)x, y);
      else
        throw new ArgumentException("Error: Try to apply Pow() to " + x.TypeAndName + " and " + y.GetType() + " " + y.ToString(), "x");
    }

    public static Altaxo.Data.DoubleColumn Pow(double x, Altaxo.Data.DataColumn y)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == y.GetType())
        return Altaxo.Data.DoubleColumn.Pow(x, (Altaxo.Data.DoubleColumn)y);
      else
        throw new ArgumentException("Error: Try to apply Pow() to " + x.GetType() + " " + x.ToString() + " and " + y.TypeAndName, "x");
    }

    public static Altaxo.Data.DoubleColumn Round(Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Round((Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Round() to " + x.TypeAndName, "x");
    }

    public static Altaxo.Data.DoubleColumn Round(Altaxo.Data.DataColumn x, Altaxo.Data.DataColumn y)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType() && typeof(Altaxo.Data.DoubleColumn) == y.GetType())
        return Altaxo.Data.DoubleColumn.Round((Altaxo.Data.DoubleColumn)x, (Altaxo.Data.DoubleColumn)y);
      else
        throw new ArgumentException("Error: Try to apply Round() to " + x.TypeAndName + " and " + y.TypeAndName, "x");
    }

    public static Altaxo.Data.DoubleColumn Round(Altaxo.Data.DataColumn x, int y)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Round((Altaxo.Data.DoubleColumn)x, y);
      else
        throw new ArgumentException("Error: Try to apply Round() to " + x.TypeAndName + " and " + y.GetType() + " " + y.ToString(), "x");
    }

    public static Altaxo.Data.DoubleColumn Round(double x, Altaxo.Data.DataColumn y)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == y.GetType())
        return Altaxo.Data.DoubleColumn.Round(x, (Altaxo.Data.DoubleColumn)y);
      else
        throw new ArgumentException("Error: Try to apply Round() to " + x.GetType() + " " + x.ToString() + " and " + y.TypeAndName, "x");
    }

    public static Altaxo.Data.DoubleColumn Sign(Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Sign((Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Sign() to " + x.TypeAndName, "x");
    }

    public static Altaxo.Data.DoubleColumn Sin(Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Sin((Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Sin() to " + x.TypeAndName, "x");
    }

    public static Altaxo.Data.DoubleColumn Sinh(Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Sinh((Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Sinh() to " + x.TypeAndName, "x");
    }

    public static Altaxo.Data.DoubleColumn Sqrt(Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Sqrt((Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Sqrt() to " + x.TypeAndName, "x");
    }

    public static Altaxo.Data.DoubleColumn Tan(Altaxo.Data.DataColumn x)
    {
      if (typeof(Altaxo.Data.DoubleColumn) == x.GetType())
        return Altaxo.Data.DoubleColumn.Tan((Altaxo.Data.DoubleColumn)x);
      else
        throw new ArgumentException("Error: Try to apply Tan() to " + x.TypeAndName, "x");
    }

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

  public abstract class FitFunctionExeBase : ScriptExecutionBase, Altaxo.Calc.Regression.Nonlinear.IFitFunction
  {
    private static readonly string[] _emptyStringArray = new string[0];
    protected string[] _independentVariableNames = _emptyStringArray;
    protected string[] _dependentVariableNames = _emptyStringArray;
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

    public virtual void EvaluateMultiple(IROMatrix<double> independent, IReadOnlyList<double> P, IReadOnlyList<bool>? independentVariableChoice, IVector<double> FV)
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

        if (independentVariableChoice is null)
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
            if (independentVariableChoice[c] == true)
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
