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
 * StudentsTDistribution.cs, 17.08.2006
 *
 */

using System;

namespace Altaxo.Calc.Probability
{
  /// <summary>
  /// Provides generation of Student's t-distributed random numbers.
  /// </summary>
  /// <remarks>
  /// The implementation of the <see cref="StudentsTDistribution"/> type is based on information presented on
  /// <a href="http://en.wikipedia.org/wiki/Student%27s_t-distribution">Wikipedia - Student's t-distribution</a> and
  /// <a href="http://www.xycoon.com/stt_random.htm">Xycoon - Student t Distribution</a>.
  /// </remarks>
  public class StudentsTDistribution : ContinuousDistribution
  {
    #region instance fields

    /// <summary>
    /// Gets or sets the parameter nu which is used for generation of t-distributed random numbers.
    /// </summary>
    /// <remarks>Call <see cref="IsValidNu"/> to determine whether a value is valid and therefor assignable.</remarks>
    public double Nu
    {
      get
      {
        return nu;
      }
      set
      {
        Initialize(value);
      }
    }

    /// <summary>
    /// Stores the parameter nu which is used for generation of t-distributed random numbers.
    /// </summary>
    private double nu;

    /// <summary>
    /// Stores a <see cref="NormalDistribution"/> object used for generation of t-distributed random numbers.
    /// </summary>
    private NormalDistribution normalDistribution;

    /// <summary>
    /// Stores a <see cref="ChiSquareDistribution"/> object used for generation of t-distributed random numbers.
    /// </summary>
    private ChiSquareDistribution chiSquareDistribution;

    #endregion instance fields

    #region construction

    /// <summary>
    /// Initializes a new instance of the <see cref="StudentsTDistribution"/> class, using a
    ///   <see cref="StandardGenerator"/> as underlying random number generator.
    /// </summary>
    public StudentsTDistribution()
      : this(DefaultGenerator)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StudentsTDistribution"/> class, using a
    ///   <see cref="StandardGenerator"/> as underlying random number generator.
    /// </summary>
    public StudentsTDistribution(Generator generator)
      : this(1, generator)
    {
    }

    public StudentsTDistribution(double nu)
      : this(nu, DefaultGenerator)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StudentsTDistribution"/> class, using the specified
    ///   <see cref="Generator"/> as underlying random number generator.
    /// </summary>
    /// <param name="nu">Parameter of the distribution.</param>
    /// <param name="generator">A <see cref="Generator"/> object.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="generator"/> is NULL (<see langword="Nothing"/> in Visual Basic).
    /// </exception>
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    public StudentsTDistribution(double nu, Generator generator)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
      : base(generator)
    {
      normalDistribution = new NormalDistribution(0, 1, generator);
      Initialize(nu);
    }

    #endregion construction

    #region instance methods

    /// <summary>
    /// Determines whether the specified value is valid for parameter <see cref="Nu"/>.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>
    /// <see langword="true"/> if value is greater than 0; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsValidNu(double value)
    {
      return value > 0;
    }

    /// <summary>
    /// Updates the helper variables that store intermediate results for generation of t-distributed random
    ///   numbers.
    /// </summary>
    public void Initialize(double nu)
    {
      if (!IsValidNu(nu))
        throw new ArgumentOutOfRangeException("Nu out of range (must be positive");

      this.nu = nu;
      if (chiSquareDistribution is null)
        chiSquareDistribution = new ChiSquareDistribution(nu);
      else
        chiSquareDistribution.Alpha = this.nu;
    }

    #endregion instance methods

    #region overridden Distribution members

    /// <summary>
    /// Gets the minimum possible value of t-distributed random numbers.
    /// </summary>
    /// <value>Symbolic constant indicating the smallest possible value.</value>
    /// <remarks>
    /// For the t-distribution, this value is always negative infinity.
    /// </remarks>
    public override double Minimum
    {
      get
      {
        return double.MinValue;
      }
    }

    /// <summary>
    /// Gets the maximum possible value of t-distributed random numbers.
    /// </summary>
    /// <value>Symbolic constant indicating the largest possible value.</value>
    /// <remarks>
    /// For the t-distribution, this value is always positive infinity.
    /// </remarks>
    public override double Maximum
    {
      get
      {
        return double.MaxValue;
      }
    }

    /// <summary>
    /// Gets the mean value of t-distributed random numbers.
    /// </summary>
    /// <value>The mean of the distribution.</value>
    /// <remarks>
    /// The mean is defined only for <c>nu > 1</c>, and is zero for symmetric cases.
    /// </remarks>
    public override double Mean
    {
      get
      {
        if (nu > 1)
        {
          return 0.0;
        }
        else
        {
          return double.NaN;
        }
      }
    }

    /// <summary>
    /// Gets the median of t-distributed random numbers.
    /// </summary>
    /// <value>The median of the distribution.</value>
    /// <remarks>
    /// The median is zero, corresponding to the peak of the t-distribution.
    /// </remarks>
    public override double Median
    {
      get
      {
        return 0.0;
      }
    }

    /// <summary>
    /// Gets the variance of t-distributed random numbers.
    /// </summary>
    /// <value>
    /// The variance of the distribution, finite only for <c>nu > 2</c>.
    /// </value>
    /// <remarks>
    /// The variance measures the spread of the distribution and is defined as
    /// <c>nu / (nu - 2)</c> for <c>nu > 2</c>.
    /// </remarks>
    public override double Variance
    {
      get
      {
        if (nu > 2)
        {
          return nu / (nu - 2.0);
        }
        else
        {
          return double.NaN;
        }
      }
    }

    /// <summary>
    /// Gets the mode of t-distributed random numbers.
    /// </summary>
    /// <value>
    /// An array containing the mode(s) of the distribution.
    /// </value>
    /// <remarks>
    /// The t-distribution is symmetric around the mean for <c>nu > 1</c>,
    /// and its mode is defined as the value with the highest probability density (i.e., the peak of the distribution).
    /// </remarks>
    public override double[] Mode
    {
      get
      {
        return new double[] { 0.0 };
      }
    }

    /// <summary>
    /// Returns a t-distributed floating point random number.
    /// </summary>
    /// <returns>A t-distributed double-precision floating point number.</returns>
    /// <remarks>
    /// This method generates a random value from the t-distribution with the
    /// specified degrees of freedom, using the Box-Muller transform.
    /// </remarks>
    public override double NextDouble()
    {
      return normalDistribution.NextDouble() / Math.Sqrt(chiSquareDistribution.NextDouble() / nu);
    }

    #endregion overridden Distribution members

    #region CdfPdfQuantile

    /// <inheritdoc/>
    public override double CDF(double x)
    {
      return CDF(x, nu);
    }

    /// <summary>
    /// Returns the cumulative distribution function (CDF) of Student's t distribution.
    /// </summary>
    /// <param name="x">The value at which to evaluate the CDF.</param>
    /// <param name="n">The degrees of freedom.</param>
    /// <returns>The probability that a t-distributed random variable is less than or equal to <paramref name="x"/>.</returns>
    public static double CDF(double x, double n)
    {
      return (1 + (1 - GammaRelated.BetaIR(n / (n + (x * x)), n * 0.5, 0.5)) * Math.Sign(x)) / 2.0;
    }

    /// <inheritdoc/>
    public override double PDF(double x)
    {
      return PDF(x, nu);
    }

    /// <summary>
    /// Returns the probability density function (PDF) of Student's t distribution.
    /// </summary>
    /// <param name="x">The value at which to evaluate the PDF.</param>
    /// <param name="n">The degrees of freedom.</param>
    /// <returns>The relative likelihood for the random variable to occur at <paramref name="x"/>.</returns>
    public static double PDF(double x, double n)
    {
      return Math.Pow(n / (n + (x * x)), (1 + n) / 2.0) / (Math.Sqrt(n) * GammaRelated.Beta(n / 2.0, 0.5));
    }

    /// <inheritdoc/>
    public override double Quantile(double p)
    {
      return Quantile(p, nu);
    }

    /// <summary>
    /// Returns the quantile function (inverse CDF) of Student's t distribution.
    /// </summary>
    /// <param name="alpha">A probability in the range [0, 1].</param>
    /// <param name="n">The degrees of freedom.</param>
    /// <returns>The value <c>x</c> such that <c>CDF(x) = alpha</c>.</returns>
    public static double Quantile(double alpha, double n)
    {
      return Math.Sqrt(n) * Math.Sqrt(-1 + 1 / GammaRelated.InverseBetaRegularized(1 - Math.Abs(1 - 2 * alpha), n * 0.5, 0.5)) * Math.Sign(-1 + 2 * alpha);
    }

    #endregion CdfPdfQuantile
  }

  /// <summary>
  /// Backward-compatible alias for <see cref="StudentsTDistribution"/>.
  /// </summary>
  public class StudentTDistribution : StudentsTDistribution
  {
    #region construction

    /// <summary>
    /// Initializes a new instance of the <see cref="StudentTDistribution"/> class.
    /// </summary>
    public StudentTDistribution()
      : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StudentTDistribution"/> class using the specified
    /// random number generator.
    /// </summary>
    /// <param name="generator">A <see cref="Generator"/> object.</param>
    public StudentTDistribution(Generator generator)
      : base(generator)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StudentTDistribution"/> class with the specified degrees of freedom.
    /// </summary>
    /// <param name="nu">The degrees of freedom.</param>
    public StudentTDistribution(double nu)
      : base(nu)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StudentTDistribution"/> class with the specified degrees of freedom,
    /// using the specified random number generator.
    /// </summary>
    /// <param name="nu">The degrees of freedom.</param>
    /// <param name="generator">A <see cref="Generator"/> object.</param>
    public StudentTDistribution(double nu, Generator generator)
      : base(nu, generator)
    {
    }

    #endregion construction
  }
}
