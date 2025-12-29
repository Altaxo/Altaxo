#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    combined version of Interval from https://github.com/AdamWhiteHat/IntervalArithmetic and https://github.com/selmaohneh/IntSharp
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
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
using System.Globalization;
using System.Linq;
using System.Numerics;

namespace Altaxo.Calc
{
  /// <summary>
  /// An interval numeric data type that is based on a floating-point type <typeparamref name="T"/>.
  /// This type supports basic arithmetic operations (+, -, *, /) with correctly maintained interval bounds.
  /// Instead of representing a value as a single number, an interval represents each value as a range of possibilities
  /// that you can perform arithmetic on.
  /// </summary>
  public struct Interval<T>
    : IAdditionOperators<Interval<T>, Interval<T>, Interval<T>>,
      ISubtractionOperators<Interval<T>, Interval<T>, Interval<T>>,
      IMultiplyOperators<Interval<T>, Interval<T>, Interval<T>>,
      IDivisionOperators<Interval<T>, Interval<T>, Interval<T>>,
      IEqualityOperators<Interval<T>, Interval<T>, bool>,
      IComparisonOperators<Interval<T>, Interval<T>, bool>,
      IEquatable<Interval<T>>,
      IComparable<Interval<T>>,
      IComparable
    where T :
          unmanaged,
          IFloatingPoint<T>,
          IRootFunctions<T>,
          ITrigonometricFunctions<T>
  {
    /// <summary>
    /// Gets a value that represents the number zero as a single-point interval, [0,0].
    /// </summary>
    public static Interval<T> Zero { get; }

    /// <summary>
    /// Gets a value that represents the number one as a single-point interval, [1,1].
    /// </summary>
    public static Interval<T> One { get; }

    /// <summary>
    /// Gets a value that represents the number two as a single-point interval, [2,2].
    /// </summary>
    public static Interval<T> Two { get; }

    /// <summary>
    /// Gets a function that inflates a value towards negative infinity (used to ensure a conservative lower bound).
    /// </summary>
    public static Func<T, T> InflateDown { get; private set; } = (x) => x;

    /// <summary>
    /// Gets a function that inflates a value towards positive infinity (used to ensure a conservative upper bound).
    /// </summary>
    public static Func<T, T> InflateUp { get; private set; } = (x) => x;

    /// <summary>
    /// Gets the inflated lower and upper bounds for a computed value.
    /// </summary>
    /// <param name="x">The value to inflate.</param>
    /// <returns>A tuple containing the inflated <c>min</c> and <c>max</c> bounds.</returns>
    public static (T min, T max) GetInflatedBounds(T x)
    {
      return (InflateDown(x), InflateUp(x));
    }


    /// <summary>
    /// Represents the lower bound of this interval.
    /// </summary>
    /// <value>The minimum.</value>
    public T Min { get; private set; }

    /// <summary>
    /// Represents the upper bound of this interval.
    /// </summary>
    /// <value>The maximum.</value>
    public T Max { get; private set; }

    /// <summary>
    /// Gets the size of this interval, calculated as <see cref="Max"/> minus <see cref="Min"/>.
    /// </summary>
    /// <value>The size.</value>
    public T Size { get { return Max - Min; } }

    /// <summary>
    /// Gets the arithmetic average of this interval.
    /// </summary>
    /// <value>The arithmetic average.</value>
    public T ArithmeticAverage { get { return (Min + Max) / (T.One + T.One); } }

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="Interval{T}"/> struct.
    /// </summary>
    public Interval()
    {
      Min = T.Zero;
      Max = T.Zero;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Interval{T}"/> struct as a single-point interval.
    /// </summary>
    /// <param name="value">The value.</param>
    private Interval(T value) : this(value, value) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Interval{T}"/> struct with the minimum and maximum specified.
    /// </summary>
    /// <param name="min">The minimum.</param>
    /// <param name="max">The maximum.</param>
    private Interval(T min, T max)
    {
      Min = min;
      Max = max;
    }

    /// <summary>
    /// Creates a single-point interval without inflating bounds.
    /// </summary>
    /// <param name="value">The exact value.</param>
    /// <returns>An interval equal to <c>[value, value]</c>.</returns>
    public static Interval<T> FromExact(T value)
    {
      return new Interval<T>(value, value);
    }

    /// <summary>
    /// Creates a single-point interval and inflates bounds conservatively.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>An interval containing <paramref name="value"/>, with conservatively inflated bounds.</returns>
    public static Interval<T> From(T value)
    {
      return new Interval<T>(InflateDown(value), InflateUp(value));
    }

    /// <summary>
    /// Creates an interval with exact bounds (no inflation).
    /// </summary>
    /// <param name="min">The minimum.</param>
    /// <param name="max">The maximum.</param>
    /// <returns>An interval equal to <c>[min, max]</c>.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="min"/> is greater than <paramref name="max"/>.</exception>
    public static Interval<T> FromExact(T min, T max)
    {
      if (!(min <= max))
      {
        throw new ArgumentOutOfRangeException(nameof(min), "min must be less than or equal to max.");
      }
      return new Interval<T>(min, max);
    }

    /// <summary>
    /// Creates an interval with conservatively inflated bounds.
    /// </summary>
    /// <param name="min">The minimum.</param>
    /// <param name="max">The maximum.</param>
    /// <returns>An interval containing <c>[min, max]</c>, with conservatively inflated bounds.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="min"/> is greater than <paramref name="max"/>.</exception>
    public static Interval<T> From(T min, T max)
    {
      if (!(min <= max))
      {
        throw new ArgumentOutOfRangeException(nameof(min), "min must be less than or equal to max.");
      }
      return new Interval<T>(InflateDown(min), InflateUp(max));
    }



    /// <summary>
    /// Static constructor. Initializes the static members of the <see cref="Interval{T}"/> type.
    /// </summary>
    static Interval()
    {
      Zero = new Interval<T>(T.Zero, T.Zero);
      One = new Interval<T>(T.One, T.One);
      Two = new Interval<T>(T.One + T.One, T.One + T.One);

      if (typeof(T) == typeof(System.Double))
      {
        InflateDown = (x) => T.CreateChecked(System.Double.BitDecrement(double.CreateChecked(x)));
        InflateUp = (x) => T.CreateChecked(System.Double.BitIncrement(double.CreateChecked(x)));
      }
      else if (typeof(T) == typeof(Single))
      {
        InflateDown = (x) => T.CreateChecked(System.Single.BitDecrement(float.CreateChecked(x)));
        InflateUp = (x) => T.CreateChecked(System.Single.BitIncrement(float.CreateChecked(x)));
      }
      else if (typeof(T) == typeof(System.Half))
      {
        InflateDown = (x) => T.CreateChecked(System.Half.BitDecrement(System.Half.CreateChecked(x)));
        InflateUp = (x) => T.CreateChecked(System.Half.BitIncrement(System.Half.CreateChecked(x)));
      }
      else
      {
        InflateDown = (x) => x;
        InflateUp = (x) => x;
      }
    }

    /// <summary>
    /// Combines two intervals that meet. This is similar to a union, but the intervals must meet.
    /// </summary>
    /// <param name="left">The left interval.</param>
    /// <param name="right">The right interval.</param>
    /// <returns>The combined interval.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="left"/> and <paramref name="right"/> are disjoint.</exception>
    public static Interval<T> Combine(Interval<T> left, Interval<T> right)
    {
      if (Interval<T>.IsDisjoint(left, right)) { throw new ArgumentOutOfRangeException($"Parameters {nameof(left)} and {nameof(right)} must meet."); }

      Interval<T> first = left;
      Interval<T> second = right;
      if (right < left)
      {
        first = right;
        second = left;
      }
      return new Interval<T>(CollectionMin(first.Min, second.Min), CollectionMax(first.Max, second.Max));
    }

    #endregion

    #region Operations

    /// <summary>
    /// Adds two intervals and returns the sum.
    /// </summary>
    /// <param name="augend">The augend.</param>
    /// <param name="addend">The addend.</param>
    /// <returns>The sum.</returns>
    public static Interval<T> Add(Interval<T> augend, Interval<T> addend)
    {
      return new Interval<T>(InflateDown(augend.Min + addend.Min), InflateUp(augend.Max + addend.Max));
    }

    /// <summary>
    /// Subtracts two intervals and returns the difference.
    /// </summary>
    /// <param name="minuend">The minuend.</param>
    /// <param name="subtrahend">The subtrahend.</param>
    /// <returns>The difference.</returns>
    public static Interval<T> Subtract(Interval<T> minuend, Interval<T> subtrahend)
    {
      return new Interval<T>(InflateDown(minuend.Min - subtrahend.Max), InflateUp(minuend.Max - subtrahend.Min));
    }

    /// <summary>
    /// Divides two intervals and returns the quotient.
    /// </summary>
    /// <param name="dividend">The dividend.</param>
    /// <param name="divisor">The divisor.</param>
    /// <returns>The quotient.</returns>
    /// <exception cref="DivideByZeroException">Thrown when <paramref name="divisor"/> contains zero.</exception>
    public static Interval<T> Divide(Interval<T> dividend, Interval<T> divisor)
    {
      if (divisor.Contains(T.Zero))
        throw new DivideByZeroException("The divisor interval contains zero, division is undefined.");

      Span<T> r = stackalloc T[8];
      (r[0], r[1]) = GetInflatedBounds(dividend.Min / divisor.Min);
      (r[2], r[3]) = GetInflatedBounds(dividend.Min / divisor.Max);
      (r[4], r[5]) = GetInflatedBounds(dividend.Max / divisor.Min);
      (r[6], r[7]) = GetInflatedBounds(dividend.Max / divisor.Max);
      return CollectionMinMax(r);
    }

    /// <summary>
    /// Multiplies two intervals and returns the product.
    /// </summary>
    /// <param name="multiplicand">The multiplicand.</param>
    /// <param name="multiplier">The multiplier.</param>
    /// <returns>The product.</returns>
    public static Interval<T> Multiply(Interval<T> multiplicand, Interval<T> multiplier)
    {
      Span<T> r = stackalloc T[8];
      (r[0], r[1]) = GetInflatedBounds(multiplicand.Min * multiplier.Min);
      (r[2], r[3]) = GetInflatedBounds(multiplicand.Min * multiplier.Max);
      (r[4], r[5]) = GetInflatedBounds(multiplicand.Max * multiplier.Min);
      (r[6], r[7]) = GetInflatedBounds(multiplicand.Max * multiplier.Max);
      return CollectionMinMax(r);
    }


    /// <summary>
    /// Returns an interval that represents the square root of this interval.
    /// </summary>
    /// <returns>The square-root interval.</returns>
    public readonly Interval<T> Sqrt()
    {
      return new Interval<T>(InflateDown(Min), InflateUp(Max));
    }




    /// <summary>
    /// Creates a copy of the specified interval.
    /// </summary>
    /// <param name="value">The interval to clone.</param>
    /// <returns>A copy of <paramref name="value"/>.</returns>
    public static Interval<T> Clone(Interval<T> value) => new Interval<T>(value.Min, value.Max);

    /// <summary>
    /// Returns the minimum value from the provided sequence.
    /// </summary>
    /// <param name="elements">The elements to inspect.</param>
    /// <returns>The minimum value.</returns>
    private static T CollectionMin(params T[] elements) { return elements.Min(); }

    /// <summary>
    /// Returns the maximum value from the provided sequence.
    /// </summary>
    /// <param name="elements">The elements to inspect.</param>
    /// <returns>The maximum value.</returns>
    private static T CollectionMax(params T[] elements) { return elements.Max(); }

    /// <summary>
    /// Returns the minimum and maximum values from the provided span.
    /// </summary>
    /// <param name="elements">The elements to inspect.</param>
    /// <returns>An interval spanning the minimum and maximum values.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="elements"/> is empty.</exception>
    private static Interval<T> CollectionMinMax(Span<T> elements)
    {
      if (elements.Length > 1)
      {
        T min = elements[0];
        T max = elements[0];
        for (int i = 1; i < elements.Length; i++)
        {
          if (elements[i] < min)
          {
            min = elements[i];
          }
          if (elements[i] > max)
          {
            max = elements[i];
          }
        }
        return new Interval<T>(min, max);
      }
      else if (elements.Length == 1)
      {
        return new Interval<T>(elements[0], elements[0]);
      }
      else
      {
        throw new ArgumentOutOfRangeException(nameof(elements), "Span must contain at least one element.");
      }
    }


    #endregion

    #region Operators

    #region Arithmetic Operators

    /// <summary>
    /// Adds two intervals together and returns their sum.
    /// </summary>
    /// <param name="augend">The augend.</param>
    /// <param name="addend">The addend.</param>
    /// <returns>The sum.</returns>
    public static Interval<T> operator +(Interval<T> augend, Interval<T> addend) => Interval<T>.Add(augend, addend);

    /// <summary>
    /// Subtracts two intervals and returns their difference.
    /// </summary>
    /// <param name="minuend">The minuend.</param>
    /// <param name="subtrahend">The subtrahend.</param>
    /// <returns>The difference.</returns>
    public static Interval<T> operator -(Interval<T> minuend, Interval<T> subtrahend)
    {
      return Subtract(minuend, subtrahend);
    }

    /// <summary>
    /// Multiplies two intervals and returns the product.
    /// </summary>
    /// <param name="multiplicand">The multiplicand.</param>
    /// <param name="multiplier">The multiplier.</param>
    /// <returns>The product.</returns>
    public static Interval<T> operator *(Interval<T> multiplicand, Interval<T> multiplier) => Interval<T>.Multiply(multiplicand, multiplier);

    /// <summary>
    /// Divides two intervals and returns the quotient.
    /// </summary>
    /// <param name="dividend">The dividend.</param>
    /// <param name="divisor">The divisor.</param>
    /// <returns>The quotient.</returns>
    public static Interval<T> operator /(Interval<T> dividend, Interval<T> divisor) => Interval<T>.Divide(dividend, divisor);

    /// <summary>
    /// Returns the negated interval.
    /// </summary>
    /// <param name="x">The interval to negate.</param>
    /// <returns>The negated interval.</returns>
    public static Interval<T> operator -(Interval<T> x)
    {
      return new Interval<T>(-x.Max, -x.Min);
    }

    /// <summary>
    /// Returns the absolute value interval.
    /// </summary>
    /// <returns>The absolute value interval.</returns>
    public Interval<T> Abs()
    {
      var minlt0 = this.Min < T.Zero;
      var maxlt0 = this.Max < T.Zero;
      bool bIsZero = minlt0 ^ maxlt0;
      var amin = minlt0 ? -Min : Min;
      var amax = maxlt0 ? -Max : Max;

      if (bIsZero) // if one of the intervals is zero, the min value of abs is zero
      {
        return new Interval<T>(T.Zero, amin > amax ? amin : amax);
      }
      else // otherwise, we just mirror the interval
      {
        return new Interval<T>(amin < amax ? amin : amax, amin > amax ? amin : amax);
      }
    }

    /// <summary>
    /// Returns the interval that is considered the maximum when comparing bounds.
    /// </summary>
    /// <param name="x">The first interval.</param>
    /// <param name="y">The second interval.</param>
    /// <returns>The selected maximum interval.</returns>
    public static Interval<T> Maxx(Interval<T> x, Interval<T> y)
    {
      if (x.Max > y.Max)
        return x;
      else if (y.Max > x.Max)
        return y;
      else if (x.Min > y.Min)
        return x;
      else
        return y;
    }

    #endregion

    #region Comparison Operators

    /// <inheritdoc/>
    public static bool operator ==(Interval<T> left, Interval<T> right) => Interval<T>.Equals(left, right);

    /// <inheritdoc/>
    public static bool operator !=(Interval<T> left, Interval<T> right) => !Interval<T>.Equals(left, right);

    /// <inheritdoc/>
    public static bool operator <(Interval<T> left, Interval<T> right) => (left.Min <= right.Min) && (left.Max < right.Max);

    /// <inheritdoc/>
    public static bool operator >(Interval<T> left, Interval<T> right) => (right.Max >= left.Max) && (right.Min > left.Min);

    /// <inheritdoc/>
    public static bool operator <=(Interval<T> left, Interval<T> right) => (left < right) || (left.Max == right.Max);

    /// <inheritdoc/>
    public static bool operator >=(Interval<T> left, Interval<T> right) => (right > left) || (right.Min == left.Min);

    #endregion

    #region Conversion Operators

    /// <summary>
    /// Performs an implicit conversion from <typeparamref name="T"/> to <see cref="Interval{T}"/>.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator Interval<T>(T value) => new Interval<T>(value);

    #endregion

    #endregion

    #region Equality / CompareTo


    /// <summary>
    /// Compares two values for equality.
    /// </summary>
    /// <param name="left">The value to compare with <paramref name="right"/>.</param>
    /// <param name="right">The value to compare with <paramref name="left"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="left"/> is equal to <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
    public static bool Equals(Interval<T> left, Interval<T> right)
    {
      return (left.Min == right.Min && left.Max == right.Max);
    }

    /// <inheritdoc/>
    public readonly bool Equals(Interval<T> other)
    {
      return Equals(this, other);
    }

    /// <inheritdoc/>
    public readonly int CompareTo(object? obj)
    {
      if (obj is Interval<T> other)
      {
        return CompareTo(other);
      }
      else if (obj is T t)
      {
        return CompareTo(new Interval<T>(t));
      }
      else
      {
        throw new ArgumentException("Object is not an Interval.");
      }
    }

    /// <inheritdoc/>
    public readonly int CompareTo(Interval<T> other)
    {
      if (Interval<T>.Equals(this, other))
      {
        return 0;
      }
      else if (this > other)
      {
        return 1;
      }
      else if (this < other)
      {
        return -1;
      }
      else if (this >= other)
      {
        return 1;
      }
      else if (this <= other)
      {
        return -1;
      }
      else { throw new Exception(); }
    }

    #endregion

    #region Membership

    /// <summary>
    /// Determines whether this interval contains the specified value.
    /// </summary>
    /// <param name="value">The value to test for membership.</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> is within the interval; otherwise, <see langword="false"/>.</returns>
    public bool Contains(T value)
    {
      return (value >= this.Min && value <= this.Max);
    }

    /// <summary>
    /// Determines whether two intervals are disjoint.
    /// Two intervals are disjoint if no part of one interval is contained in the other.
    /// </summary>
    /// <param name="left">The left interval.</param>
    /// <param name="right">The right interval.</param>
    /// <returns><see langword="true"/> if no part of the left interval is contained in the right interval; otherwise, <see langword="false"/>.</returns>
    public static bool IsDisjoint(Interval<T> left, Interval<T> right)
    {
      return !(left.Contains(right.Min) || left.Contains(right.Max));
    }

    #endregion

    #region Overrides

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Interval<T> o ? this.Equals(o) : false;

    /// <inheritdoc/>
    public override int GetHashCode()
    {
      return new Tuple<T, T>(this.Min, this.Max).GetHashCode();
    }

    /// <inheritdoc/>
    public override string ToString()
    {
      return ToString(null, NumberFormatInfo.CurrentInfo);
    }

    /// <summary>
    /// Returns a string that represents this instance.
    /// </summary>
    /// <param name="format">
    /// The format to use.
    /// -or-
    /// <see langword="null"/> to use the default format defined for the type of the <see cref="IFormattable"/> implementation.
    /// </param>
    /// <param name="formatProvider">
    /// The provider to use to format the value.
    /// -or-
    /// <see langword="null"/> to obtain the numeric format information from the current locale setting of the operating system.
    /// </param>
    /// <returns>A string that represents this instance.</returns>
    public string ToString(string? format, IFormatProvider formatProvider)
    {
      if (Min == Max)
      {
        return Min.ToString(format, formatProvider);
      }
      else
      {
        return $"[{Min.ToString(format, formatProvider)},{Max.ToString(format, formatProvider)}]";
      }
    }

    /// <summary>
    /// Attempts to parse a string into an <see cref="Interval{T}"/>.
    /// </summary>
    /// <param name="s">The string representation of a value.</param>
    /// <param name="style">A bitwise combination of number styles permitted in <paramref name="s"/>.</param>
    /// <param name="provider">An object that supplies culture-specific formatting information about <paramref name="s"/>.</param>
    /// <param name="result">When this method returns, contains the parsed interval.</param>
    /// <returns><see langword="true"/> if <paramref name="s"/> was parsed successfully; otherwise, <see langword="false"/>.</returns>
    public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out Interval<T> result)
    {
      var r = T.TryParse(s, style, provider, out T value);
      if (r)
      {
        result = From(value);
        return true;
      }
      else
      {
        result = Zero;
        return false;
      }
    }



    #endregion

  }
}
