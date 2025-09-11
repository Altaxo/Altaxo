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

    /// <summary>
    /// Interpolates values of an array.
    /// </summary>
    /// <param name="index">The index into the array. Can be fractional.</param>
    /// <param name="array">The array.</param>
    /// <param name="extendToSides">If true and the value of index is out of range, the values of the array
    /// at the left side or the right side will be returned; otherwise, if the index is out of range, an exception will be thrown.</param>
    /// <returns>The interpolated value at the fractional index.</returns>
    public static double InterpolateLinear(double index, IReadOnlyList<double> array, bool extendToSides = false)
    {
      if (!(index >= 0))
      {
        if (extendToSides)
          return array[0];
        else
          throw new ArgumentOutOfRangeException(nameof(index));
      }
      else if (!(index <= array.Count - 1))
      {
        if (extendToSides)
          return array[array.Count - 1];
        else
          throw new ArgumentOutOfRangeException(nameof(index));
      }
      else if (index == array.Count - 1)
      {
        return array[array.Count - 1];
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
    /// <param name="coefficients">The polynomial coefficients in ascending order.</param>
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
    /// Evaluates the 1st derivative of a polynom. The polynomial coefficients are given in ascending order,
    /// thus a0 is the first element in the array of coefficients.
    /// </summary>
    /// <param name="x">The x value.</param>
    /// <param name="coefficients">The polynomial coefficients in ascending order.</param>
    /// <returns>The value of the 1st derivative of the polynom.</returns>
    public static double EvaluatePolynom1stDerivativeOrderAscending(double x, IReadOnlyList<double> coefficients)
    {
      double sum = 0;
      for (int i = coefficients.Count - 1; i >= 1; --i)
      {
        sum *= x;
        sum += coefficients[i] * i;
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

    /// <summary>
    /// Finds the (fractional) index of the nearest element in the xVector to the xValue.
    /// </summary>
    /// <param name="xVector">The x vector.</param>
    /// <param name="xValue">The x value.</param>
    /// <returns>A tuple, consisting of the nearest index, and a boolean value.
    /// If any element of the xVector is equal to xValue, the boolean value is true.
    /// Also, if xValue is between two elements of the xVector, the boolean value is true.
    /// Otherwise, the boolean value is false, for instance, if the xValue is outside the limit of the elements of the xVector.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">xVector</exception>
    /// <exception cref="System.ArgumentException">The list must not be empty. - xVector</exception>
    public static (double index, bool isExact) FindNearestIndex(IReadOnlyList<double> xVector, double xValue)
    {
      if (xVector is null)
        throw new ArgumentNullException(nameof(xVector));
      if (xVector.Count == 0)
        throw new ArgumentException("The list must not be empty.", nameof(xVector));
      if (xVector.Count == 1 || xVector[0] == xValue)
        return (0, true);

      double minDiff = Math.Abs(xVector[0] - xValue);
      double minIndex = 0;
      for (int i = 1; i < xVector.Count; i++)
      {
        if (xVector[i] == xValue)
        {
          return (i, true);
        }
        else if (xVector[i - 1] < xValue && xVector[i] > xValue)
        {
          return (i - 1 + (xValue - xVector[i - 1]) / (xVector[i] - xVector[i - 1]), true);
        }
        else if (xVector[i - 1] > xValue && xVector[i] < xValue)
        {
          return (i - 1 + (xValue - xVector[i - 1]) / (xVector[i] - xVector[i - 1]), true);
        }
        else
        {
          var diff = Math.Abs(xVector[i] - xValue);
          if (diff < minDiff)
          {
            minDiff = diff;
            minIndex = i;
          }
        }
      }
      return (minIndex, false);
    }

    /// <summary>
    /// Smallest power of 10 that can be parsed is -323. This means that 1E-323 is parsed to a number greater than zero, whereas 1E-323 is parsed to 0.
    /// </summary>
    public const int DoubleMinimalDecimalPower = -323;

    /// <summary>
    /// Smallest power of 10 that can be parsed without precision loss is -308. This means that 1E-308 is parsed to a number that is equal to 1E-308, whereas 1E-309 is parsed to a number not equal to 1E-309.
    /// </summary>
    public const int DoubleMinimalDecimalPowerWithoutPrecisionLoss = -308;

    /// <summary>
    /// Greatest power of 10 that can be parsed is 308. This means that 1E308 is parsed to a number greater than zero, whereas 1E309 is parsed to Infinity.
    /// </summary>
    public const int DoubleMaximalDecimalPower = 308;

    private static double[] _powersOfTen = new double[]
    {
      1E-323, 1E-322, 1E-321, 1E-320, 1E-319, 1E-318, 1E-317, 1E-316,
      1E-315, 1E-314, 1E-313, 1E-312, 1E-311, 1E-310, 1E-309, 1E-308,
      1E-307, 1E-306, 1E-305, 1E-304, 1E-303, 1E-302, 1E-301, 1E-300,
      1E-299, 1E-298, 1E-297, 1E-296, 1E-295, 1E-294, 1E-293, 1E-292,
      1E-291, 1E-290, 1E-289, 1E-288, 1E-287, 1E-286, 1E-285, 1E-284,
      1E-283, 1E-282, 1E-281, 1E-280, 1E-279, 1E-278, 1E-277, 1E-276,
      1E-275, 1E-274, 1E-273, 1E-272, 1E-271, 1E-270, 1E-269, 1E-268,
      1E-267, 1E-266, 1E-265, 1E-264, 1E-263, 1E-262, 1E-261, 1E-260,
      1E-259, 1E-258, 1E-257, 1E-256, 1E-255, 1E-254, 1E-253, 1E-252,
      1E-251, 1E-250, 1E-249, 1E-248, 1E-247, 1E-246, 1E-245, 1E-244,
      1E-243, 1E-242, 1E-241, 1E-240, 1E-239, 1E-238, 1E-237, 1E-236,
      1E-235, 1E-234, 1E-233, 1E-232, 1E-231, 1E-230, 1E-229, 1E-228,
      1E-227, 1E-226, 1E-225, 1E-224, 1E-223, 1E-222, 1E-221, 1E-220,
      1E-219, 1E-218, 1E-217, 1E-216, 1E-215, 1E-214, 1E-213, 1E-212,
      1E-211, 1E-210, 1E-209, 1E-208, 1E-207, 1E-206, 1E-205, 1E-204,
      1E-203, 1E-202, 1E-201, 1E-200, 1E-199, 1E-198, 1E-197, 1E-196,
      1E-195, 1E-194, 1E-193, 1E-192, 1E-191, 1E-190, 1E-189, 1E-188,
      1E-187, 1E-186, 1E-185, 1E-184, 1E-183, 1E-182, 1E-181, 1E-180,
      1E-179, 1E-178, 1E-177, 1E-176, 1E-175, 1E-174, 1E-173, 1E-172,
      1E-171, 1E-170, 1E-169, 1E-168, 1E-167, 1E-166, 1E-165, 1E-164,
      1E-163, 1E-162, 1E-161, 1E-160, 1E-159, 1E-158, 1E-157, 1E-156,
      1E-155, 1E-154, 1E-153, 1E-152, 1E-151, 1E-150, 1E-149, 1E-148,
      1E-147, 1E-146, 1E-145, 1E-144, 1E-143, 1E-142, 1E-141, 1E-140,
      1E-139, 1E-138, 1E-137, 1E-136, 1E-135, 1E-134, 1E-133, 1E-132,
      1E-131, 1E-130, 1E-129, 1E-128, 1E-127, 1E-126, 1E-125, 1E-124,
      1E-123, 1E-122, 1E-121, 1E-120, 1E-119, 1E-118, 1E-117, 1E-116,
      1E-115, 1E-114, 1E-113, 1E-112, 1E-111, 1E-110, 1E-109, 1E-108,
      1E-107, 1E-106, 1E-105, 1E-104, 1E-103, 1E-102, 1E-101, 1E-100,
      1E-99, 1E-98, 1E-97, 1E-96, 1E-95, 1E-94, 1E-93, 1E-92,
      1E-91, 1E-90, 1E-89, 1E-88, 1E-87, 1E-86, 1E-85, 1E-84,
      1E-83, 1E-82, 1E-81, 1E-80, 1E-79, 1E-78, 1E-77, 1E-76,
      1E-75, 1E-74, 1E-73, 1E-72, 1E-71, 1E-70, 1E-69, 1E-68,
      1E-67, 1E-66, 1E-65, 1E-64, 1E-63, 1E-62, 1E-61, 1E-60,
      1E-59, 1E-58, 1E-57, 1E-56, 1E-55, 1E-54, 1E-53, 1E-52,
      1E-51, 1E-50, 1E-49, 1E-48, 1E-47, 1E-46, 1E-45, 1E-44,
      1E-43, 1E-42, 1E-41, 1E-40, 1E-39, 1E-38, 1E-37, 1E-36,
      1E-35, 1E-34, 1E-33, 1E-32, 1E-31, 1E-30, 1E-29, 1E-28,
      1E-27, 1E-26, 1E-25, 1E-24, 1E-23, 1E-22, 1E-21, 1E-20,
      1E-19, 1E-18, 1E-17, 1E-16, 1E-15, 1E-14, 1E-13, 1E-12,
      1E-11, 1E-10, 1E-9, 1E-8, 1E-7, 1E-6, 1E-5, 1E-4,
      1E-3, 1E-2, 1E-1, 1E0, 1E1, 1E2, 1E3, 1E4,
      1E5, 1E6, 1E7, 1E8, 1E9, 1E10, 1E11, 1E12,
      1E13, 1E14, 1E15, 1E16, 1E17, 1E18, 1E19, 1E20,
      1E21, 1E22, 1E23, 1E24, 1E25, 1E26, 1E27, 1E28,
      1E29, 1E30, 1E31, 1E32, 1E33, 1E34, 1E35, 1E36,
      1E37, 1E38, 1E39, 1E40, 1E41, 1E42, 1E43, 1E44,
      1E45, 1E46, 1E47, 1E48, 1E49, 1E50, 1E51, 1E52,
      1E53, 1E54, 1E55, 1E56, 1E57, 1E58, 1E59, 1E60,
      1E61, 1E62, 1E63, 1E64, 1E65, 1E66, 1E67, 1E68,
      1E69, 1E70, 1E71, 1E72, 1E73, 1E74, 1E75, 1E76,
      1E77, 1E78, 1E79, 1E80, 1E81, 1E82, 1E83, 1E84,
      1E85, 1E86, 1E87, 1E88, 1E89, 1E90, 1E91, 1E92,
      1E93, 1E94, 1E95, 1E96, 1E97, 1E98, 1E99, 1E100,
      1E101, 1E102, 1E103, 1E104, 1E105, 1E106, 1E107, 1E108,
      1E109, 1E110, 1E111, 1E112, 1E113, 1E114, 1E115, 1E116,
      1E117, 1E118, 1E119, 1E120, 1E121, 1E122, 1E123, 1E124,
      1E125, 1E126, 1E127, 1E128, 1E129, 1E130, 1E131, 1E132,
      1E133, 1E134, 1E135, 1E136, 1E137, 1E138, 1E139, 1E140,
      1E141, 1E142, 1E143, 1E144, 1E145, 1E146, 1E147, 1E148,
      1E149, 1E150, 1E151, 1E152, 1E153, 1E154, 1E155, 1E156,
      1E157, 1E158, 1E159, 1E160, 1E161, 1E162, 1E163, 1E164,
      1E165, 1E166, 1E167, 1E168, 1E169, 1E170, 1E171, 1E172,
      1E173, 1E174, 1E175, 1E176, 1E177, 1E178, 1E179, 1E180,
      1E181, 1E182, 1E183, 1E184, 1E185, 1E186, 1E187, 1E188,
      1E189, 1E190, 1E191, 1E192, 1E193, 1E194, 1E195, 1E196,
      1E197, 1E198, 1E199, 1E200, 1E201, 1E202, 1E203, 1E204,
      1E205, 1E206, 1E207, 1E208, 1E209, 1E210, 1E211, 1E212,
      1E213, 1E214, 1E215, 1E216, 1E217, 1E218, 1E219, 1E220,
      1E221, 1E222, 1E223, 1E224, 1E225, 1E226, 1E227, 1E228,
      1E229, 1E230, 1E231, 1E232, 1E233, 1E234, 1E235, 1E236,
      1E237, 1E238, 1E239, 1E240, 1E241, 1E242, 1E243, 1E244,
      1E245, 1E246, 1E247, 1E248, 1E249, 1E250, 1E251, 1E252,
      1E253, 1E254, 1E255, 1E256, 1E257, 1E258, 1E259, 1E260,
      1E261, 1E262, 1E263, 1E264, 1E265, 1E266, 1E267, 1E268,
      1E269, 1E270, 1E271, 1E272, 1E273, 1E274, 1E275, 1E276,
      1E277, 1E278, 1E279, 1E280, 1E281, 1E282, 1E283, 1E284,
      1E285, 1E286, 1E287, 1E288, 1E289, 1E290, 1E291, 1E292,
      1E293, 1E294, 1E295, 1E296, 1E297, 1E298, 1E299, 1E300,
      1E301, 1E302, 1E303, 1E304, 1E305, 1E306, 1E307, 1E308,
    };

    /// <summary>
    /// Returns 10 to the power of i, i.e. 10^i.
    /// </summary>
    /// <param name="i">The exponent.</param>
    /// <returns>10 to the power of i, i.e. 10^i.</returns>
    public static double TenToThePowerOf(int i)
    {
      if (i < DoubleMinimalDecimalPower)
      {
        return 0;
      }
      else if (i > DoubleMaximalDecimalPower)
      {
        return double.PositiveInfinity;
      }
      else
      {
        return _powersOfTen[i - DoubleMinimalDecimalPower];
      }
    }
  }
}
