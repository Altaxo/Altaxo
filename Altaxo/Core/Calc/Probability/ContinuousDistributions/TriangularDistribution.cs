/*
 * Copyright © 2006 Stefan Troschütz (stefan@troschuetz.de)
 *
 * This file is part of Troschuetz.Random Class Library.
 *
 * Troschuetz.Random is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
 *
 * TriangularDistribution.cs, 21.09.2006
 *
 * 09.08.2006: Initial version
 * 21.09.2006: Adapted to change in base class (field "generator" declared private (formerly protected)
 *               and made accessible through new protected property "Generator")
 *
 */

#region original copyright

/* boost random/triangle_distribution.hpp header file
 *
 * Copyright Jens Maurer 2000-2001
 * Distributed under the Boost Software License, Version 1.0. (See
 * accompanying file LICENSE_1_0.txt or copy at
 * http://www.boost.org/LICENSE_1_0.txt)
 *
 * See http://www.boost.org for most recent version including documentation.
 *
 * $Id: triangle_distribution.hpp,v 1.11 2004/07/27 03:43:32 dgregor Exp $
 *
 * Revision history
 *  2001-02-18  moved to individual header files
 */

#endregion original copyright

using System;

namespace Altaxo.Calc.Probability
{
  /// <summary>
  /// Provides generation of triangular-distributed random numbers.
  /// </summary>
  /// <remarks>
  /// The parametrization is equal to the parametrization in Mathematica.
  /// </remarks>
  public class TriangularDistribution : ContinuousDistribution
  {
    #region instance fields

    /// <summary>
    /// Gets or sets the parameter Min (the left boundary of the PDF), which is used for generation of triangular distributed random numbers.
    /// </summary>
    /// <remarks>Call <see cref="IsValidMin"/> to determine whether a value is valid and therefor assignable.</remarks>
    public double Min
    {
      get
      {
        return _min;
      }
    }

    /// <summary>
    /// Stores the parameter alpha which is used for generation of triangular distributed random numbers.
    /// </summary>
    private double _min;

    /// <summary>
    /// Gets or sets the parameter Max (the right boundary of the PDF), which is used for generation of triangular distributed random numbers.
    /// </summary>
    /// <remarks>Call <see cref="IsValidMax"/> to determine whether a value is valid and therefor assignable.</remarks>
    public double Max
    {
      get
      {
        return _max;
      }
    }

    /// <summary>
    /// Stores the parameter beta which is used for generation of triangular distributed random numbers.
    /// </summary>
    private double _max;

    /// <summary>
    /// Gets or sets the parameter C (the location of the maximum of the PDF), which is used for generation of triangular distributed random numbers.
    /// </summary>
    /// <remarks>Call <see cref="IsValidC"/> to determine whether a value is valid and therefor assignable.</remarks>
    public double C
    {
      get
      {
        return _c;
      }
    }

    /// <summary>
    /// Stores the parameter gamma which is used for generation of triangular distributed random numbers.
    /// </summary>
    private double _c;

    /// <summary>
    /// Stores an intermediate result for generation of triangular distributed random numbers.
    /// </summary>
    /// <remarks>
    /// Speeds up random number generation cause this value only depends on distribution parameters
    ///   and therefor doesn't need to be recalculated in successive executions of <see cref="NextDouble"/>.
    /// </remarks>
    private double _c_min;

    /// <summary>
    /// Stores an intermediate result for generation of triangular distributed random numbers.
    /// </summary>
    /// <remarks>
    /// Speeds up random number generation cause this value only depends on distribution parameters
    ///   and therefor doesn't need to be recalculated in successive executions of <see cref="NextDouble"/>.
    /// </remarks>
    private double _max_min;

    /// <summary>
    /// Stores an intermediate result for generation of triangular distributed random numbers.
    /// </summary>
    /// <remarks>
    /// Speeds up random number generation cause this value only depends on distribution parameters
    ///   and therefor doesn't need to be recalculated in successive executions of <see cref="NextDouble"/>.
    /// </remarks>
    private double _sqrt_cmin_maxmin;

    /// <summary>
    /// Stores an intermediate result for generation of triangular distributed random numbers.
    /// </summary>
    /// <remarks>
    /// Speeds up random number generation cause this value only depends on distribution parameters
    ///   and therefor doesn't need to be recalculated in successive executions of <see cref="NextDouble"/>.
    /// </remarks>
    private double _sqrt_maxc;

    #endregion instance fields

    #region construction

    /// <summary>
    /// Initializes a new instance of the <see cref="TriangularDistribution"/> class, using a
    ///   <see cref="StandardGenerator"/> as underlying random number generator.
    /// </summary>
    public TriangularDistribution()
      : this(DefaultGenerator)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TriangularDistribution"/> class, using the specified
    ///   <see cref="Generator"/> as underlying random number generator.
    /// </summary>
    /// <param name="generator">A <see cref="Generator"/> object.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="generator"/> is NULL (<see langword="Nothing"/> in Visual Basic).
    /// </exception>
    public TriangularDistribution(Generator generator)
      : this(0, 1, 0.5, generator)
        {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TriangularDistribution"/> class.
    /// </summary>
    /// <param name="min">The left boundary of the PDF.</param>
    /// <param name="max">The right boundary of the PDF.</param>
    /// <param name="c">The location of the maximum of the PDF (must be between <paramref name="min"/> and <paramref name="max"/>).</param>
    public TriangularDistribution(double min, double max, double c)
      : this(min, max, c, DefaultGenerator)
        {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TriangularDistribution"/> class.
    /// </summary>
    /// <param name="min">The left boundary of the PDF.</param>
    /// <param name="max">The right boundary of the PDF.</param>
    /// <param name="c">The location of the maximum of the PDF (must be between <paramref name="min"/> and <paramref name="max"/>).</param>
    /// <param name="generator">The random number generator.</param>
    public TriangularDistribution(double min, double max, double c, Generator generator)
      : base(generator)
    {
      Initialize(min, max, c);
    }

    #endregion construction

    #region instance methods

    /// <summary>
    /// Initializes the distribution.
    /// </summary>
    /// <param name="min">The left boundary of the PDF.</param>
    /// <param name="max">The right boundary of the PDF.</param>
    /// <param name="c">The location of the maximum of the PDF (must be between <paramref name="min"/> and <paramref name="max"/>).</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The parameter combination is invalid.
    /// </exception>
    public void Initialize(double min, double max, double c)
        {
      if (!(min < max && min <= c))
        throw new ArgumentOutOfRangeException("Alpha out of range (must be < beta and <= gamma)");
      if (!(max > min && max >= c))
        throw new ArgumentOutOfRangeException("Beta out of range (have to be > alpha and >= gamma)");
      if (!(c >= min && c <= max))
        throw new ArgumentOutOfRangeException("Gamma out of range (have to be >= alpha and <= beta)");

      this._min = min;
      this._max = max;
      this._c = c;

      UpdateHelpers();
    }

    /// <summary>
    /// Determines whether the specified value is valid for parameter <see cref="Min"/>.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>
    /// <see langword="true"/> if value is less than <see cref="Max"/>, and less than or equal to
    ///   <see cref="C"/>; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsValidMin(double value)
    {
      return (value < _max && value <= _c);
    }

    /// <summary>
    /// Determines whether the specified value is valid for parameter <see cref="Max"/>.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>
    /// <see langword="true"/> if value is greater than <see cref="Min"/>, and greater than or equal to
    ///   <see cref="C"/>; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsValidMax(double value)
    {
      return (value > _min && value >= _c);
    }

    /// <summary>
    /// Determines whether the specified value is valid for parameter <see cref="C"/>.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>
    /// <see langword="true"/> if value is greater than or equal to <see cref="Min"/>, and greater than or equal
    ///   to <see cref="Max"/>; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsValidC(double value)
    {
      return (value >= _min && value <= _max);
    }

    /// <summary>
    /// Updates the helper variables that store intermediate results for generation of triangular distributed random
    ///   numbers.
    /// </summary>
    private void UpdateHelpers()
    {
      _c_min = _c - _min;
      _max_min = _max - _min;
      _sqrt_cmin_maxmin = Math.Sqrt(_c_min * _max_min);
      _sqrt_maxc = Math.Sqrt(_max - _c);
    }

    #endregion instance methods

    #region overridden Distribution members

    /// <summary>
    /// Gets the minimum possible value of triangular distributed random numbers.
    /// </summary>
    /// <inheritdoc/>
    public override double Minimum
    {
      get
      {
        return _min;
      }
    }

    /// <summary>
    /// Gets the maximum possible value of triangular distributed random numbers.
    /// </summary>
    /// <inheritdoc/>
    public override double Maximum
    {
      get
      {
        return _max;
      }
    }

    /// <summary>
    /// Gets the mean value of triangular distributed random numbers.
    /// </summary>
    /// <inheritdoc/>
    public override double Mean
    {
      get
      {
        return _min / 3.0 + _max / 3.0 + _c / 3.0;
      }
    }

    /// <summary>
    /// Gets the median of triangular distributed random numbers.
    /// </summary>
    /// <inheritdoc/>
    public override double Median
    {
      get
      {
        if (_c >= (_max - _min) / 2.0)
        {
          return _min +
              (Math.Sqrt((_max - _min) * (_c - _min)) / Math.Sqrt(2.0));
        }
        else
        {
          return _max -
              (Math.Sqrt((_max - _min) * (_max - _c)) / Math.Sqrt(2.0));
        }
      }
    }

    /// <summary>
    /// Gets the variance of triangular distributed random numbers.
    /// </summary>
    /// <inheritdoc/>
    public override double Variance
    {
      get
      {
        return (Math.Pow(_min, 2.0) + Math.Pow(_max, 2.0) + Math.Pow(_c, 2.0) -
          _min * _max - _min * _c - _max * _c) / 18.0;
      }
    }

    /// <summary>
    /// Gets the mode of triangular distributed random numbers.
    /// </summary>
    /// <inheritdoc/>
    public override double[] Mode
    {
      get
      {
        return new double[] { _c };
      }
    }

    /// <summary>
    /// Returns a triangular distributed floating point random number.
    /// </summary>
    /// <returns>A triangular distributed double-precision floating point number.</returns>
    /// <inheritdoc/>
    public override double NextDouble()
    {
      double genNum = Generator.NextDouble();
      if (genNum <= _c_min / _max_min)
      {
        return _min + Math.Sqrt(genNum) * _sqrt_cmin_maxmin;
      }
      else
      {
        return _max - Math.Sqrt(genNum * _max_min - _c_min) * _sqrt_maxc;
      }
    }

    #endregion overridden Distribution members

    #region CdfPdfQuantile

    /// <summary>
    /// Calculates the cumulative distribution function.
    /// </summary>
    /// <param name="x">Argument.</param>
    /// <returns>
    /// The probability that the random variable of this probability distribution will be found at a value less than or equal to <paramref name="x" />.
    /// </returns>
    public override double CDF(double x)
    {
      return CDF(x, _min, _max, _c);
    }

    /// <summary>
    /// Calculates the cumulative distribution function at the specified x.
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="min">The left boundary of the PDF.</param>
    /// <param name="max">The right boundary of the PDF.</param>
    /// <param name="c">The location of the maximum of the PDF (has to be inbetween (min, max).</param>
    /// <returns>The cumulative distribution function at x.</returns>
    public static double CDF(double x, double min, double max, double c)
    {
      if (x <= min)
        return 0;
      if (x >= max)
        return 1;
      if (x <= c)
        return (x - min) * (x - min) / ((max - min) * (c - min));
      else
        return 1 - (max - x) * (max - x) / ((max - min) * (max - c));
    }

    /// <summary>
    /// Calculates the probability density function.
    /// </summary>
    /// <param name="x">Argument.</param>
    /// <returns>
    /// The relative likelihood for the random variable to occur at the point <paramref name="x" />.
    /// </returns>
    public override double PDF(double x)
    {
      return PDF(x, _min, _max, _c);
    }

    /// <summary>
    /// Calculates the probability density at the specified x.
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="min">The left boundary of the PDF.</param>
    /// <param name="max">The right boundary of the PDF.</param>
    /// <param name="c">The location of the maximum of the PDF (has to be inbetween (min, max).</param>
    /// <returns>The probability density at x.</returns>
    public static double PDF(double x, double min, double max, double c)
    {
      if (x <= min)
        return 0;
      if (x >= max)
        return 0;
      if (x <= c)
        return 2 * (x - min) / ((max - min) * (c - min));
      else
        return 2 * (max - x) / ((max - min) * (max - c));
    }

    /// <summary>
    /// Calculates the quantile of the distribution function.
    /// </summary>
    /// <param name="p">The probability p.</param>
    /// <returns>
    /// The point x at which the cumulative distribution function <see cref="CDF(double)" /> of argument x is equal to <paramref name="p" />.
    /// </returns>
    public override double Quantile(double p)
    {
      return Quantile(p, _min, _max, _c);
    }

    /// <summary>
    /// Calculates the quantile at the specified probability p.
    /// </summary>
    /// <param name="p">The p.</param>
    /// <param name="min">The left boundary of the PDF.</param>
    /// <param name="max">The right boundary of the PDF.</param>
    /// <param name="c">The location of the maximum of the PDF (has to be inbetween (min, max).</param>
    /// <returns>The quantile at the probability p.</returns>
    public static double Quantile(double p, double min, double max, double c)
    {
      double x;
      if (!(p >= 0 && p <= 1))
        throw new ArgumentOutOfRangeException("p must be inbetween [0,1]");
      if (!(min < max))
        throw new ArgumentOutOfRangeException("A must be < B");
      if (!(c >= min))
        throw new ArgumentOutOfRangeException("C must be >= A");
      if (!(c <= max))
        throw new ArgumentOutOfRangeException("C must be <= B");

      x = min + Math.Sqrt(p * (min - max) * (min - c));

      if (x > c)
        x = max - Math.Sqrt((1 - p) * (max - min) * (max - c));

      return x;
    }

    #endregion CdfPdfQuantile
  }
}
