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
 * ContinuousUniformDistribution.cs, 21.09.2006
 *
 * 09.08.2006: Initial version
 * 21.09.2006: Adapted to change in base class (field "generator" declared private (formerly protected)
 *               and made accessible through new protected property "Generator")
 *
 */

using System;

namespace Altaxo.Calc.Probability
{
  /// <summary>
  /// Provides generation of continuous uniformly distributed random numbers.
  /// </summary>
  /// <remarks>
  /// The implementation of the <see cref="ContinuousUniformDistribution"/> type bases upon information presented on
  ///   <a href="http://en.wikipedia.org/wiki/Uniform_distribution_%28continuous%29">
  ///   Wikipedia - Uniform distribution (continuous)</a>.
  /// </remarks>
  public class ContinuousUniformDistribution : ContinuousDistribution
  {
    #region instance fields

    /// <summary>
    /// Gets or sets the parameter alpha which is used for generation of uniformly distributed random numbers.
    /// </summary>
    /// <remarks>Call <see cref="IsValidAlpha"/> to determine whether a value is valid and therefor assignable.</remarks>
    public double Alpha
    {
      get
      {
        return this.alpha;
      }
    }

    /// <summary>
    /// Stores the parameter alpha which is used for generation of uniformly distributed random numbers.
    /// </summary>
    private double alpha;

    /// <summary>
    /// Gets or sets the parameter beta which is used for generation of uniformly distributed random numbers.
    /// </summary>
    /// <remarks>Call <see cref="IsValidBeta"/> to determine whether a value is valid and therefor assignable.</remarks>
    public double Beta
    {
      get
      {
        return this.beta;
      }
    }

    /// <summary>
    /// Stores the parameter beta which is used for generation of uniformly distributed random numbers.
    /// </summary>
    private double beta;

    /// <summary>
    /// Stores an intermediate result for generation of uniformly distributed random numbers.
    /// </summary>
    /// <remarks>
    /// Speeds up random number generation cause this value only depends on distribution parameters
    ///   and therefor doesn't need to be recalculated in successive executions of <see cref="NextDouble"/>.
    /// </remarks>
    private double helper1;

    #endregion instance fields

    #region construction

    /// <summary>
    /// Initializes a new instance of the <see cref="ContinuousUniformDistribution"/> class, using a
    ///   <see cref="StandardGenerator"/> as underlying random number generator.
    /// </summary>
    public ContinuousUniformDistribution()
      : this(DefaultGenerator)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContinuousUniformDistribution"/> class, using the specified
    ///   <see cref="Generator"/> as underlying random number generator.
    /// </summary>
    /// <param name="generator">A <see cref="Generator"/> object.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="generator"/> is NULL (<see langword="Nothing"/> in Visual Basic).
    /// </exception>
    public ContinuousUniformDistribution(Generator generator)
      : this(0, 1, generator)
    {
    }

    public ContinuousUniformDistribution(double lower, double upper)
      : this(lower, upper, DefaultGenerator)
    {
    }

    public ContinuousUniformDistribution(double lower, double upper, Generator generator)
      : base(generator)
    {
      Initialize(lower, upper);
    }

    #endregion construction

    #region instance methods

    public void Initialize(double lower, double upper)
    {
      if (!(lower <= upper))
        throw new ArgumentOutOfRangeException("Lower bound is not less or equal than upper bound");
      if (!IsValidAlpha(lower))
        throw new ArgumentOutOfRangeException("Lower bound is out of range");
      if (!IsValidBeta(upper))
        throw new ArgumentOutOfRangeException("Upper bound is out of range");

      this.alpha = lower;
      this.beta = upper;
      this.helper1 = this.beta - this.alpha;
    }

    /// <summary>
    /// Determines whether the specified value is valid for parameter <see cref="Alpha"/>.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>
    /// <see langword="true"/> if value is a valid number and not infinity; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsValidAlpha(double value)
    {
      return value >= double.MinValue && value <= double.MaxValue;
    }

    /// <summary>
    /// Determines whether the specified value is valid for parameter <see cref="Beta"/>.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>
    /// <see langword="true"/> if value is a valid number and not infinity; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsValidBeta(double value)
    {
      return value >= double.MinValue && value <= double.MaxValue;
    }

    #endregion instance methods

    #region overridden Distribution members

    /// <summary>
    /// Gets the minimum possible value of uniformly distributed random numbers.
    /// </summary>
    public override double Minimum
    {
      get
      {
        return this.alpha;
      }
    }

    /// <summary>
    /// Gets the maximum possible value of uniformly distributed random numbers.
    /// </summary>
    public override double Maximum
    {
      get
      {
        return this.beta;
      }
    }

    /// <summary>
    /// Gets the mean value of the uniformly distributed random numbers.
    /// </summary>
    public override double Mean
    {
      get
      {
        return this.alpha / 2.0 + this.beta / 2.0;
      }
    }

    /// <summary>
    /// Gets the median of uniformly distributed random numbers.
    /// </summary>
    public override double Median
    {
      get
      {
        return this.alpha / 2.0 + this.beta / 2.0;
      }
    }

    /// <summary>
    /// Gets the variance of uniformly distributed random numbers.
    /// </summary>
    public override double Variance
    {
      get
      {
        return Math.Pow(this.beta - this.alpha, 2.0) / 12.0;
      }
    }

    /// <summary>
    /// Gets the mode of the uniformly distributed random numbers.
    /// </summary>
    public override double[] Mode
    {
      get
      {
        return new double[] { };
      }
    }

    /// <summary>
    /// Returns a uniformly distributed floating point random number.
    /// </summary>
    /// <returns>A uniformly distributed double-precision floating point number.</returns>
    public override double NextDouble()
    {
      return this.alpha + this.Generator.NextDouble() * this.helper1;
    }

    #endregion overridden Distribution members

    #region CdfPdfQuantile

    public override double CDF(double z)
    {
      return CDF(z, alpha, beta);
    }

    public static double CDF(double z, double low, double high)
    {
      if (z < low)
        return 0;
      else if (z >= high)
        return 1;
      else
        return (z - low) / (high - low);
    }

    public override double PDF(double z)
    {
      return PDF(z, alpha, beta);
    }

    public static double PDF(double z, double low, double high)
    {
      if (z < low)
        return 0;
      else if (z >= high)
        return 0;
      else
        return 1.0 / (high - low);
    }

    public override double Quantile(double p)
    {
      return Quantile(p, alpha, beta);
    }

    public static double Quantile(double p, double low, double high)
    {
      if (p < 0 || p > 1)
        throw new ArgumentException("Probability p must be between 0 and 1");
      return low + p * (high - low);
    }

    #endregion CdfPdfQuantile
  }
}
