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
using System.Collections.Generic;

namespace Altaxo.Calc
{
  /// <summary>
  /// Provides methods for real numbers, that were forgotten by the <see cref="System.Math" /> class.
  /// </summary>
  public static class RMath
  {
    #region Helper constants

    private const double GSL_DBL_EPSILON = 2.2204460492503131e-16;
    private const double GSL_SQRT_DBL_EPSILON = 1.4901161193847656e-08;

    private const double M_LN2 = 0.69314718055994530941723212146;     // ln(2)

    #endregion Helper constants

    #region Number tests

    /// <summary>
    /// Tests if x is finite, i.e. is in the interval [double.MinValue, double.MaxValue].
    /// </summary>
    /// <param name="x">Number to test.</param>
    /// <returns>True if x is finite. False if is is not finite or is double.NaN.</returns>
    public static bool IsFinite(double x) // Note that we don't use 'this' here, because 'IsFinite' is also included in the 'Math.Net' package
    {
      return double.MinValue <= x && x <= double.MaxValue;
    }

    /// <summary>
    /// Tests if x is finite, i.e. is in the interval [float.MinValue, float.MaxValue].
    /// </summary>
    /// <param name="x">Number to test.</param>
    /// <returns>True if x is finite. False if is is not finite or is float.NaN.</returns>
    public static bool IsFinite(this float x)
    {
      return float.MinValue <= x && x <= float.MaxValue;
    }

    /// <summary>
    /// Test if x is not a number.
    /// </summary>
    /// <param name="x">Number to test.</param>
    /// <returns>True if x is NaN.</returns>
    public static bool IsNaN(this double x)
    {
      return double.IsNaN(x);
    }

    /// <summary>
    /// Test if x is not a number.
    /// </summary>
    /// <param name="x">Number to test.</param>
    /// <returns>True if x is NaN.</returns>
    public static bool IsNaN(this float x)
    {
      return float.IsNaN(x);
    }

    /// <summary>
    /// Tests whether or not x is in the closed interval [xmin, xmax]. No test is done if xmin is less than xmax.
    /// </summary>
    /// <param name="x">The argument.</param>
    /// <param name="xmin">The lower boundary of the interval.</param>
    /// <param name="xmax">The upper boundary of the interval.</param>
    /// <returns>True if xmin &lt;= x and x &lt;= xmax.</returns>
    public static bool IsInIntervalCC(this double x, double xmin, double xmax)
    {
      return xmin <= x && x <= xmax;
    }

    /// <summary>
    /// Tests whether or not x is in the open interval (xmin, xmax). No test is done if xmin is less than xmax.
    /// </summary>
    /// <param name="x">The argument.</param>
    /// <param name="xmin">The lower boundary of the interval.</param>
    /// <param name="xmax">The upper boundary of the interval.</param>
    /// <returns>True if xmin &lt; x and x &lt; xmax.</returns>
    public static bool IsInIntervalOO(this double x, double xmin, double xmax)
    {
      return xmin < x && x < xmax;
    }

    /// <summary>
    /// Tests whether or not x is in the semi-open interval [xmin, xmax). No test is done if xmin is less than xmax.
    /// </summary>
    /// <param name="x">The argument.</param>
    /// <param name="xmin">The lower boundary of the interval.</param>
    /// <param name="xmax">The upper boundary of the interval.</param>
    /// <returns>True if xmin &lt;= x and x &lt; xmax.</returns>
    public static bool IsInIntervalCO(this double x, double xmin, double xmax)
    {
      return xmin <= x && x < xmax;
    }

    /// <summary>
    /// Tests whether or not x is in the semi-open interval (xmin, xmax]. No test is done if xmin is less than xmax.
    /// </summary>
    /// <param name="x">The argument.</param>
    /// <param name="xmin">The lower boundary of the interval.</param>
    /// <param name="xmax">The upper boundary of the interval.</param>
    /// <returns>True if xmin &lt; x and x &lt;= xmax.</returns>
    public static bool IsInIntervalOC(this double x, double xmin, double xmax)
    {
      return xmin < x && x <= xmax;
    }

    /// <summary>
    /// Tests if the interval [a0, a1] is overlapping with the interval [b0, b1]. The return value is true if
    /// the intervals overlap, or only touching each other (for instance, if a1==b0).
    /// </summary>
    /// <param name="a0">The start of the first interval (inclusive).</param>
    /// <param name="a1">The end of the first interval (inclusive).</param>
    /// <param name="b0">The start of the second interval (inclusive).</param>
    /// <param name="b1">The end of the second interval (inclusive).</param>
    /// <returns>True if both intervals overlap or touch; otherwise, false.</returns>
    public static bool AreIntervalsOverlappingCC(double a0, double a1, double b0, double b1)
    {
      if (!(a1 >= a0 && b1 >= b0))
        throw new ArgumentOutOfRangeException();
      return !(a0 > b1 || b0 > a1);
    }

    /// <summary>
    /// Tests if the interval [a0, a1) is overlapping with the interval [b0, b1). The return value is true if
    /// the intervals overlap. Because the end of the intervals are open, the return value is false if for instance a1==b0.
    /// </summary>
    /// <param name="a0">The start of the first interval (inclusive).</param>
    /// <param name="a1">The end of the first interval (exclusive).</param>
    /// <param name="b0">The start of the second interval (inclusive).</param>
    /// <param name="b1">The end of the second interval (exclusive).</param>
    /// <returns>True if both intervals overlap; otherwise, false.</returns>
    public static bool AreIntervalsOverlappingCO(double a0, double a1, double b0, double b1)
    {
      if (!(a1 >= a0 && b1 >= b0))
        throw new ArgumentOutOfRangeException();
      return !(a0 >= b1 || b0 >= a1);
    }

    /// <summary>
    /// Clamps the value <paramref name="x"/> to the interval [<paramref name="xmin"/>, <paramref name="xmax"/>]. If Double.NaN is provided, the return value is Double.NaN.
    /// </summary>
    /// <param name="x">The x value.</param>
    /// <param name="xmin">The interval minimum.</param>
    /// <param name="xmax">The interval maximum.</param>
    /// <returns>The value <paramref name="x"/> clamped to the interval [<paramref name="xmin"/>, <paramref name="xmax"/>]. If Double.NaN is provided, the return value is Double.NaN.</returns>
    public static double ClampToInterval(this double x, double xmin, double xmax)
    {
      if (!(xmin <= xmax))
        throw new ArgumentOutOfRangeException(nameof(xmin) + " should be less than or equal to " + nameof(xmax));

      if (x < xmin)
        x = xmin;
      if (x > xmax)
        x = xmax;
      return x;
    }

    /// <summary>
    /// If x is inside the interval [leftValue, rightValue] or [rightValue, leftValue], then this function returns a value in the range [0,1]
    /// designating the (linear) position inside that interval. I.e. if rightValue > leftValue, the return value would be (x-leftValue)/(rightValue-leftValue).
    /// </summary>
    /// <param name="x">The x value.</param>
    /// <param name="leftValue">The left value of the interval, might be less than or greater than <paramref name="rightValue"/>.</param>
    /// <param name="rightValue">The right value of the interval, might be less than or greater han <paramref name="leftValue"/>.</param>
    /// <returns></returns>
    public static double? InFractionOfUnorderedIntervalCC(this double x, double leftValue, double rightValue)
    {
      bool inverted = false;

      if (x == leftValue)
        return 0.0;
      else if (x == rightValue)
        return 1.0;

      if (rightValue < leftValue)
      {
        var h = rightValue;
        rightValue = leftValue;
        leftValue = h;
        inverted = true;
      }

      if (leftValue <= x && x <= rightValue)
      {
        var denom = rightValue - leftValue;
        var nom = x - leftValue;

        if (denom == 0)
          return 0.0;
        else
          return inverted ? 1 - nom / denom : nom / denom;
      }

      return null;
    }

    /// <summary>
    /// Interpolates linearly between <paramref name="leftValue"/> and <paramref name="rightValue"/>, using the parameter <paramref name="fraction"/>.
    /// </summary>
    /// <param name="fraction">The fraction value.</param>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>(1-fraction)*leftValue + fraction*rightValue. If fraction is either 0 or 1, only the leftValue or the rightValue will be used as return value, respectively.</returns>
    public static double InterpolateLinear(double fraction, double leftValue, double rightValue)
    {
      if (fraction == 0)
        return leftValue;
      else if (fraction == 1)
        return rightValue;
      else
        return (1 - fraction) * leftValue + fraction * rightValue;
    }


    /// <summary>
    /// Interpolates values of an array.
    /// </summary>
    /// <param name="index">The index into the array. Can be fractional.</param>
    /// <param name="array">The array.</param>
    /// <param name="extendToSides">If true and the value of index is out of range, the values of the array
    /// at the left side or the right side will be returned; otherwise, if the index is out of range, an exception will be thrown.</param>
    /// <returns>The interpolated value at the fractional index.</returns>
    public static double InterpolateLinear(double index, double[] array, bool extendToSides = false)
    {
      if (!(index >= 0))
      {
        if (extendToSides)
          return array[0];
        else
          throw new ArgumentOutOfRangeException(nameof(index));
      }
      else if (!(index <= array.Length - 1))
      {
        if (extendToSides)
          return array[array.Length - 1];
        else
          throw new ArgumentOutOfRangeException(nameof(index));
      }
      else if (index == array.Length - 1)
      {
        return array[array.Length - 1];
      }

      int idx = (int)index;
      return InterpolateLinear(index - idx, array[idx], array[idx + 1]);
    }

    #endregion Number tests

    public static double Log1p(double x)
    {
      double y;
      y = 1 + x;
      return Math.Log(y) - ((y - 1) - x) / y;  /* cancels errors with IEEE arithmetic */
    }

    private static readonly double OneMinusExp_SmallBound = Math.Pow(DoubleConstants.DBL_EPSILON * 3628800, 1 / 9.0);

    /// <summary>
    /// Evaluates the polynom. The polynomial coefficients are given in ascending order.
    /// </summary>
    /// <param name="x">The x value.</param>
    /// <param name="coefficients">The polynomila coefficients in ascending order.</param>
    /// <returns>The value of the evaluated polynom.</returns>
    public static double EvaluatePolynomOrderAscending(double x, IReadOnlyList<double> coefficients)
    {
      double sum = 0;
      for (int i = coefficients.Count - 1; i >= 0; --i)
      {
        sum *= x;
        sum += coefficients[i];
      }
      return sum;
    }

    /// <summary>
    /// Evaluates the polynom. The polynomial coefficients are given in ascending order.
    /// </summary>
    /// <param name="x">The x value.</param>
    /// <param name="coefficients">The polynomila coefficients in ascending order.</param>
    /// <returns>The value of the evaluated polynom.</returns>
    public static double EvaluatePolynomOrderDescending(double x, IReadOnlyList<double> coefficients)
    {
      double sum = 0;
      for (int i = 0; i < coefficients.Count; ++i)
      {
        sum *= x;
        sum += coefficients[i];
      }
      return sum;
    }


    /// <summary>
    /// Calculates 1-Exp(x) with better accuracy around x=0.
    /// </summary>
    /// <param name="x">Function argument</param>
    /// <returns>The value 1-Exp(x)</returns>
    public static double OneMinusExp(double x)
    {
      const double A1 = 1;
      const double A2 = 1 / 2.0;
      const double A3 = 1 / 6.0;
      const double A4 = 1 / 24.0;
      const double A5 = 1 / 120.0;
      const double A6 = 1 / 720.0;
      const double A7 = 1 / 5040.0;
      const double A8 = 1 / 40320.0;
      const double A9 = 1 / 362880.0;

      double ax = Math.Abs(x);
      if (ax < OneMinusExp_SmallBound)
      {
        if (ax < DoubleConstants.DBL_EPSILON)
          return -x;
        else
          return -(((((((((A9 * x) + A8) * x + A7) * x + A6) * x + A5) * x + A4) * x + A3) * x + A2) * x + A1) * x;
      }
      else
      {
        return 1 - Math.Exp(x);
      }
    }

    public static double Acosh(double x)
    {
      if (x > 1.0 / GSL_SQRT_DBL_EPSILON)
      {
        return Math.Log(x) + M_LN2;
      }
      else if (x > 2)
      {
        return Math.Log(2 * x - 1 / (Math.Sqrt(x * x - 1) + x));
      }
      else if (x > 1)
      {
        double t = x - 1;
        return Log1p(t + Math.Sqrt(2 * t + t * t));
      }
      else if (x == 1)
      {
        return 0;
      }
      else
      {
        return double.NaN;
      }
    }

    public static double Asinh(double x)
    {
      double a = Math.Abs(x);
      double s = (x < 0) ? -1 : 1;

      if (a > 1 / GSL_SQRT_DBL_EPSILON)
      {
        return s * (Math.Log(a) + M_LN2);
      }
      else if (a > 2)
      {
        return s * Math.Log(2 * a + 1 / (a + Math.Sqrt(a * a + 1)));
      }
      else if (a > GSL_SQRT_DBL_EPSILON)
      {
        double a2 = a * a;
        return s * Log1p(a + a2 / (1 + Math.Sqrt(1 + a2)));
      }
      else
      {
        return x;
      }
    }

    public static double Atanh(double x)
    {
      double a = Math.Abs(x);
      double s = (x < 0) ? -1 : 1;

      if (a > 1)
      {
        return double.NaN;
      }
      else if (a == 1)
      {
        return (x < 0) ? double.NegativeInfinity : double.PositiveInfinity;
      }
      else if (a >= 0.5)
      {
        return s * 0.5 * Log1p(2 * a / (1 - a));
      }
      else if (a > GSL_DBL_EPSILON)
      {
        return s * 0.5 * Log1p(2 * a + 2 * a * a / (1 - a));
      }
      else
      {
        return x;
      }
    }

    /// <summary>
    /// The standard hypot() function for two arguments taking care of overflows and zerodivides.
    /// </summary>
    /// <param name="x">First argument.</param>
    /// <param name="y">Second argument.</param>
    /// <returns>Square root of the sum of x-square and y-square.</returns>
    public static double Hypot(double x, double y)
    {
      double xabs = Math.Abs(x);
      double yabs = Math.Abs(y);
      double min, max;

      if (xabs < yabs)
      {
        min = xabs;
        max = yabs;
      }
      else
      {
        min = yabs;
        max = xabs;
      }

      if (min == 0)
      {
        return max;
      }

      {
        double u = min / max;
        return max * Math.Sqrt(1 + u * u);
      }
    }

    /// <summary>
    /// Calculates x^2 (square of x).
    /// </summary>
    /// <param name="x">Argument.</param>
    /// <returns><c>x</c> squared.</returns>
    public static double Pow2(this double x)
    {
      return x * x;
    }

    public static double Pow3(this double x)
    {
      return x * x * x;
    }

    public static double Pow4(this double x)
    {
      double x2 = x * x;
      return x2 * x2;
    }

    public static double Pow5(this double x)
    {
      double x2 = x * x;
      return x2 * x2 * x;
    }

    public static double Pow6(this double x)
    {
      double x2 = x * x;
      return x2 * x2 * x2;
    }

    public static double Pow7(this double x)
    {
      double x3 = x * x * x;
      return x3 * x3 * x;
    }

    public static double Pow8(this double x)
    {
      double x2 = x * x;
      double x4 = x2 * x2;
      return x4 * x4;
    }

    public static double Pow9(this double x)
    {
      double x3 = x * x * x;
      return x3 * x3 * x3;
    }

    /// <summary>
    /// Calculates x^n by repeated multiplications. The algorithm takes ld(n) multiplications.
    /// This algorithm can also be used with negative n.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="n"></param>
    /// <returns></returns>
    public static double Pow(this double x, int n)
    {
      double value = 1.0;

      bool inverse = (n < 0);
      if (n < 0)
      {
        n = -n;

        if (!(n > 0)) // if n was so big, that it could not be inverted in sign
          return double.NaN;
      }

      /* repeated squaring method
             * returns 0.0^0 = 1.0, so continuous in x
             */
      do
      {
        if (0 != (n & 1))
          value *= x;  /* for n odd */

        n >>= 1;
        x *= x;
      } while (n != 0);

      return inverse ? 1.0 / value : value;
    }

    /// <summary>
    /// Calculates x^n by repeated multiplications. The algorithm takes ld(n) multiplications.
    /// This algorithm can also be used with negative n.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="n"></param>
    /// <returns></returns>
    public static double Pow(this double x, long n)
    {
      double value = 1.0;

      bool inverse = (n < 0);
      if (n < 0)
      {
        n = -n;

        if (!(n > 0)) // if n was so big, that it could not be inverted in sign
          return double.NaN;
      }

      /* repeated squaring method
             * returns 0.0^0 = 1.0, so continuous in x
             */
      do
      {
        if (0 != (n & 1))
          value *= x;  /* for n odd */

        n >>= 1;
        x *= x;
      } while (n != 0);

      return inverse ? 1.0 / value : value;
    }

    /// <summary>
    /// Calculates x * 10^n.
    /// </summary>
    /// <param name="x">The scaling factor.</param>
    /// <param name="n">The decadic exponent.</param>
    /// <returns>The product x * 10^n.</returns>
    public static double ScaleDecadic(double x, int n)
    {
      return n < 0 ? x / Pow(10, -n) : x * Pow(10, n);
    }

    /// <summary>
    /// Gets the minimum and maximum of two values.
    /// </summary>
    /// <param name="a">First value.</param>
    /// <param name="b">Second value.</param>
    /// <returns>The minimum and maximum of the two values.</returns>
    public static (double Min, double Max) MinMax(double a, double b)
    {
      return a < b ? (a, b) : (b, a);
    }

    /// <summary>
    /// Gets the minimum and maximum of three values.
    /// </summary>
    /// <param name="a">First value.</param>
    /// <param name="b">Second value.</param>
    /// <param name="c">Third value.</param>
    /// <returns>The minimum and maximum of the three values.</returns>
    public static (double Min, double Max) MinMax(double a, double b, double c)
    {
      return (Math.Min(a, Math.Min(b, c)), Math.Max(a, Math.Max(b, c)));
    }

    /// <summary>
    /// Gets the minimum and maximum of four values.
    /// </summary>
    /// <param name="a">First value.</param>
    /// <param name="b">Second value.</param>
    /// <param name="c">Third value.</param>
    /// <param name="d">Fourth value.</param>
    /// <returns>The minimum and maximum of the four values.</returns>
    public static (double Min, double Max) MinMax(double a, double b, double c, double d)
    {
      return (Math.Min(a, Math.Min(b, Math.Min(c, d))), Math.Max(a, Math.Max(b, Math.Max(c, d))));
    }
  }
}
