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
  /// Rounding of numbers.
  /// </summary>
  public class Rounding
  {
    /// <summary>
    /// Returns the next number <c>k</c> with <c>k &gt;= i</c> and <c>k mod n == 0</c>.
    /// </summary>
    /// <param name="i">The number to round up.</param>
    /// <param name="n">The rounding step.</param>
    /// <returns>The smallest number <c>k</c> such that <c>k &gt;= i</c> and <c>k mod n == 0</c>.</returns>
    public static int RoundUp(int i, int n)
    {
      if (n <= 0)
        throw new ArgumentOutOfRangeException("n<=0");

      var r = i % n;
      return r <= 0 ? i - r : i + (n - r);
    }

    /// <summary>
    /// Returns the next number <c>k</c> with <c>k &lt;= i</c> and <c>k mod n == 0</c>.
    /// </summary>
    /// <param name="i">The number to round down.</param>
    /// <param name="n">The rounding step.</param>
    /// <returns>The greatest number <c>k</c> such that <c>k &lt;= i</c> and <c>k mod n == 0</c>.</returns>
    public static int RoundDown(int i, int n)
    {
      if (n <= 0)
        throw new ArgumentOutOfRangeException("n<=0");

      var r = i % n;
      return r >= 0 ? i - r : i - (n + r);
    }

    /// <summary>
    /// Returns the next number <c>k</c> with <c>k &gt;= i</c> and <c>k mod n == 0</c>.
    /// </summary>
    /// <param name="i">The number to round up.</param>
    /// <param name="n">The rounding step.</param>
    /// <returns>The smallest number <c>k</c> such that <c>k &gt;= i</c> and <c>k mod n == 0</c>.</returns>
    public static long RoundUp(long i, long n)
    {
      if (n <= 0)
        throw new ArgumentOutOfRangeException("n<=0");

      var r = i % n;
      return r <= 0 ? i - r : i + (n - r);
    }

    /// <summary>
    /// Returns the next number <c>k</c> with <c>k &lt;= i</c> and <c>k mod n == 0</c>.
    /// </summary>
    /// <param name="i">The number to round down.</param>
    /// <param name="n">The rounding step.</param>
    /// <returns>The greatest number <c>k</c> such that <c>k &lt;= i</c> and <c>k mod n == 0</c>.</returns>
    public static long RoundDown(long i, long n)
    {
      if (n <= 0)
        throw new ArgumentOutOfRangeException("n<=0");

      var r = i % n;
      return r >= 0 ? i - r : i - (n + r);
    }

    /// <summary>
    /// Rounds a double-precision value to the provided number of significant digits.
    /// </summary>
    /// <param name="x">The value to round.</param>
    /// <param name="significantDigits">The number of significant digits.</param>
    /// <param name="rounding">The midpoint rounding rule that should be applied.</param>
    /// <returns>The number, rounded to the provided number of significant digits.</returns>
    public static double RoundToNumberOfSignificantDigits(double x, int significantDigits, MidpointRounding rounding)
    {
      if (significantDigits < 0)
        throw new ArgumentOutOfRangeException("significantDigits<0");

      if (0 == x)
        return 0;

      int lg = (int)Math.Floor(Math.Log10(Math.Abs(x))) + 1;

      if (lg < 0)
      {
        double fac = RMath.Pow(10, -lg);
        double xpot = x * fac;
        xpot = Math.Round(xpot, significantDigits, rounding);
        return xpot / fac;
      }
      else
      {
        double fac = RMath.Pow(10, lg);
        double xpot = x / fac;
        xpot = Math.Round(xpot, significantDigits, rounding);
        return xpot * fac;
      }
    }

    /// <summary>
    /// Decomposes a double-precision floating-point number into its decimal mantissa and exponent.
    /// </summary>
    /// <remarks>
    /// If the input value is <see cref="double.NaN"/>, <see cref="double.PositiveInfinity"/>,  <see
    /// cref="double.NegativeInfinity"/>, or zero, the method returns the input value as the mantissa
    /// and 0 as the exponent.
    /// </remarks>
    /// <param name="x">The number to decompose. Must be a finite value or zero.</param>
    /// <returns>
    /// A tuple containing the decimal mantissa and exponent. The mantissa is a double-precision value in the range
    /// <c>-10 &lt; mantissa &lt; 10</c>, and the exponent is an integer representing the power of 10 such that
    /// <c>x == mantissa * 10^exponent</c> approximates the input value.
    /// </returns>
    public static (double mantissa, int exponent) SplitIntoDecimalMantissaAndExponent(double x)
    {
      if (double.IsNaN(x) || double.IsInfinity(x) || x == 0)
      {
        return (x, 0);
      }
      else
      {
        int lg = Math.Max(RMath.DoubleMinimalDecimalPower, (int)Math.Floor(Math.Log10(Math.Abs(x))));

        double mantissa = Math.Round(x / RMath.TenToThePowerOf(lg), 14);

        if (mantissa == 10)
        {
          mantissa = 1;
          lg++;
        }
        else if (mantissa == -10)
        {
          mantissa = -1;
          lg++;
        }

        return (mantissa, lg);
      }
    }
  }
}
