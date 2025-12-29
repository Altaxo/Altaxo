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
 * CauchyDistribution.cs, 21.09.2006
 *
 * 09.08.2006: Initial version
 * 21.09.2006: Adapted to change in base class (field "generator" declared private (formerly protected)
 *               and made accessible through new protected property "Generator")
 */

using System;

namespace Altaxo.Calc.Probability
{
  /// <summary>
  /// Provides generation of Cauchy distributed random numbers.
  /// </summary>
  /// <remarks>
  /// The implementation of the <see cref="CauchyDistribution"/> type is based upon information presented on
  ///   <a href="http://en.wikipedia.org/wiki/Cauchy_distribution">Wikipedia - Cauchy distribution</a> and
  ///   <a href="http://www.xycoon.com/cauchy2p_random.htm">Xycoon - Cauchy Distribution</a>.
  /// </remarks>
  public class CauchyDistribution : ContinuousDistribution
  {
    #region instance fields

    /// <summary>
    /// Gets or sets the parameter alpha (location parameter; designates the median) which is used for generation of Cauchy distributed random numbers.
    /// </summary>
    /// <remarks>Call <see cref="IsValidAlpha"/> to determine whether a value is valid and therefore assignable.</remarks>
    public double Alpha
    {
      get
      {
        return alpha;
      }
      set
      {
        Initialize(value, gamma);
      }
    }

    /// <summary>
    /// Stores the parameter alpha (location parameter; designates the median) which is used for generation of Cauchy distributed random numbers.
    /// </summary>
    private double alpha;

    /// <summary>
    /// Gets or sets the parameter gamma (scale parameter; designates the half-width at half-maximum) which is used for generation of Cauchy distributed random numbers.
    /// </summary>
    /// <remarks>Call <see cref="IsValidGamma"/> to determine whether a value is valid and therefore assignable.</remarks>
    public double Gamma
    {
      get
      {
        return gamma;
      }
      set
      {
        Initialize(alpha, value);
      }
    }

    /// <summary>
    /// Stores the parameter gamma which is used for generation of cauchy distributed random numbers.
    /// </summary>
    private double gamma;

    #endregion instance fields

    #region construction

    /// <summary>
    /// Initializes a new instance of the <see cref="CauchyDistribution"/> class, using a
    ///   <see cref="StandardGenerator"/> as underlying random number generator.
    /// </summary>
    public CauchyDistribution()
      : this(DefaultGenerator)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CauchyDistribution"/> class, using the specified
    ///   <see cref="Generator"/> as underlying random number generator.
    /// </summary>
    /// <param name="generator">A <see cref="Generator"/> object.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="generator"/> is NULL (<see langword="Nothing"/> in Visual Basic).
    /// </exception>
    public CauchyDistribution(Generator generator)
      : this(1, 1, generator)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CauchyDistribution"/> class, using the default random number generator.
    /// </summary>
    /// <param name="alpha">First parameter of the distribution.</param>
    /// <param name="gamma">Second parameter of the distribution.</param>
    public CauchyDistribution(double alpha, double gamma)
      : this(alpha, gamma, DefaultGenerator)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CauchyDistribution"/> class, using the specified
    ///   <see cref="Generator"/> as underlying random number generator.
    /// </summary>
    /// <param name="alpha">First parameter of the distribution.</param>
    /// <param name="gamma">Second parameter of the distribution.</param>
    /// <param name="generator">A <see cref="Generator"/> object.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="generator"/> is NULL (<see langword="Nothing"/> in Visual Basic).
    /// </exception>
    public CauchyDistribution(double alpha, double gamma, Generator generator)
      : base(generator)
    {
      Initialize(alpha, gamma);
    }

    #endregion construction

    #region instance methods

    /// <summary>
    /// Determines whether the specified value is valid for parameter <see cref="Alpha"/>.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>
    /// <see langword="true"/> if the value is within the range of <see cref="double"/>; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsValidAlpha(double value)
    {
      return value > double.MinValue && value < double.MaxValue;
    }

    /// <summary>
    /// Determines whether the specified value is valid for parameter <see cref="Gamma"/>.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>
    /// <see langword="true"/> if value is greater than 0.0; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsValidGamma(double value)
    {
      return value > 0.0;
    }

    /// <summary>
    /// Initializes this instance with the specified parameters.
    /// </summary>
    /// <param name="alpha">Location parameter.</param>
    /// <param name="gamma">Scale parameter.</param>
    public void Initialize(double alpha, double gamma)
    {
      if (!IsValidAlpha(alpha))
        throw new ArgumentOutOfRangeException("Alpha is out of range");
      if (!IsValidGamma(gamma))
        throw new ArgumentOutOfRangeException("Gamma is out of range (has to be positive)");

      this.alpha = alpha;
      this.gamma = gamma;
    }

    #endregion instance methods

    #region overridden Distribution members

    /// <inheritdoc/>
    public override double Minimum
    {
      get
      {
        return double.MinValue;
      }
    }

    /// <inheritdoc/>
    public override double Maximum
    {
      get
      {
        return double.MaxValue;
      }
    }

    /// <inheritdoc/>
    public override double Mean
    {
      get
      {
        return double.NaN;
      }
    }

    /// <inheritdoc/>
    public override double Median
    {
      get
      {
        return alpha;
      }
    }

    /// <inheritdoc/>
    public override double Variance
    {
      get
      {
        return double.NaN;
      }
    }

    /// <inheritdoc/>
    public override double[] Mode
    {
      get
      {
        return new double[] { alpha };
      }
    }

    /// <inheritdoc/>
    public override double NextDouble()
    {
      return alpha + gamma * Math.Tan(Math.PI * (Generator.NextDouble() - 0.5));
    }

    #endregion overridden Distribution members

    #region CdfPdfQuantile

    /// <summary>
    /// Computes the square of the specified value.
    /// </summary>
    /// <param name="x">The value.</param>
    /// <returns>The square of <paramref name="x"/>.</returns>
    private static double Pow2(double x)
    {
      return x * x;
    }

    /// <inheritdoc/>
    public override double CDF(double x)
    {
      return CDF(x, alpha, gamma);
    }

    /// <summary>
    /// Calculates the cumulative distribution function for the specified parameters.
    /// </summary>
    /// <param name="x">Argument.</param>
    /// <param name="a">Location parameter.</param>
    /// <param name="b">Scale parameter.</param>
    /// <returns>
    /// The probability that the random variable is less than or equal to <paramref name="x"/>.
    /// </returns>
    public static double CDF(double x, double a, double b)
    {
      return 0.5 + Math.Atan((-a + x) / b) / Math.PI;
    }

    /// <inheritdoc/>
    public override double PDF(double x)
    {
      return PDF(x, alpha, gamma);
    }

    /// <summary>
    /// Calculates the probability density function for the specified parameters.
    /// </summary>
    /// <param name="x">Argument.</param>
    /// <param name="a">Location parameter.</param>
    /// <param name="b">Scale parameter.</param>
    /// <returns>The relative likelihood for the random variable to occur at <paramref name="x"/>.</returns>
    public static double PDF(double x, double a, double b)
    {
      return 1 / (b * Math.PI * (1 + Pow2(-a + x) / Pow2(b)));
    }

    /// <inheritdoc/>
    public override double Quantile(double p)
    {
      return Quantile(p, alpha, gamma);
    }

    /// <summary>
    /// Calculates the quantile function for the specified parameters.
    /// </summary>
    /// <param name="p">The probability.</param>
    /// <param name="a">Location parameter.</param>
    /// <param name="b">Scale parameter.</param>
    /// <returns>
    /// The point <c>x</c> at which the cumulative distribution function is equal to <paramref name="p"/>.
    /// </returns>
    public static double Quantile(double p, double a, double b)
    {
      return a + b * Math.Tan((-0.5 + p) * Math.PI);
    }

    #endregion CdfPdfQuantile
  }
}
