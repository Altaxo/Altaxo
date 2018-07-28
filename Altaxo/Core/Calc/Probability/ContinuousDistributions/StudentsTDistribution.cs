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
  /// Provides generation of t-distributed random numbers.
  /// </summary>
  /// <remarks>
  /// The implementation of the <see cref="StudentsTDistribution"/> type bases upon information presented on
  ///   <a href="http://en.wikipedia.org/wiki/Student%27s_t-distribution">Wikipedia - Student's t-distribution</a> and
  ///   <a href="http://www.xycoon.com/stt_random.htm">Xycoon - Student t Distribution</a>.
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
        return this.nu;
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
    public StudentsTDistribution(double nu, Generator generator)
      : base(generator)
    {
      this.normalDistribution = new NormalDistribution(0, 1, generator);
      this.Initialize(nu);
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
      if (null == chiSquareDistribution)
        this.chiSquareDistribution = new ChiSquareDistribution(nu);
      else
        this.chiSquareDistribution.Alpha = this.nu;
    }

    #endregion instance methods

    #region overridden Distribution members

    /// <summary>
    /// Gets the minimum possible value of t-distributed random numbers.
    /// </summary>
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
    public override double Mean
    {
      get
      {
        if (this.nu > 1)
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
    public override double Variance
    {
      get
      {
        if (this.nu > 2)
        {
          return (double)this.nu / ((double)this.nu - 2.0);
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
    public override double NextDouble()
    {
      return this.normalDistribution.NextDouble() / Math.Sqrt(this.chiSquareDistribution.NextDouble() / (double)this.nu);
    }

    #endregion overridden Distribution members

    #region CdfPdfQuantile

    public override double CDF(double x)
    {
      return CDF(x, nu);
    }

    public static double CDF(double x, double n)
    {
      return (1 + (1 - GammaRelated.BetaIR(n / (n + (x * x)), n * 0.5, 0.5)) * Math.Sign(x)) / 2.0;
    }

    public override double PDF(double x)
    {
      return PDF(x, nu);
    }

    public static double PDF(double x, double n)
    {
      return Math.Pow(n / (n + (x * x)), (1 + n) / 2.0) / (Math.Sqrt(n) * GammaRelated.Beta(n / 2.0, 0.5));
    }

    public override double Quantile(double p)
    {
      return Quantile(p, nu);
    }

    public static double Quantile(double alpha, double n)
    {
      return Math.Sqrt(n) * Math.Sqrt(-1 + 1 / GammaRelated.InverseBetaRegularized(1 - Math.Abs(1 - 2 * alpha), n * 0.5, 0.5)) * Math.Sign(-1 + 2 * alpha);
    }

    #endregion CdfPdfQuantile
  }

  public class StudentTDistribution : StudentsTDistribution
  {
    #region construction

    /// <summary>
    /// Initializes a new instance of the <see cref="StudentsTDistribution"/> class, using a
    ///   <see cref="StandardGenerator"/> as underlying random number generator.
    /// </summary>
    public StudentTDistribution()
      : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StudentTDistribution"/> class, using the specified
    ///   <see cref="Generator"/> as underlying random number generator.
    /// </summary>
    /// <param name="generator">A <see cref="Generator"/> object.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="generator"/> is NULL (<see langword="Nothing"/> in Visual Basic).
    /// </exception>
    public StudentTDistribution(Generator generator)
      : base(generator)
    {
    }

    public StudentTDistribution(double nu)
      : base(nu)
    {
    }

    public StudentTDistribution(double nu, Generator generator)
      : base(nu, generator)
    {
    }

    #endregion construction
  }
}
