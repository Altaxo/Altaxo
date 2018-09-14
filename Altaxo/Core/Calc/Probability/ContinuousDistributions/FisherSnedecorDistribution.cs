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
 * FisherSnedecorDistribution.cs, 16.08.2006
 *
 */

using System;

namespace Altaxo.Calc.Probability
{
  /// <summary>
  /// Provides generation of Fisher-Snedecor distributed random numbers.
  /// </summary>
  /// <remarks>
  /// The implementation of the <see cref="FisherSnedecorDistribution"/> type bases upon information presented on
  ///   <a href="http://en.wikipedia.org/wiki/F-distribution">Wikipedia - F-distribution</a>.
  /// <code>
  /// Return F-distributed (variance ratio distributed) random deviates
  /// with n numerator degrees of freedom and d denominator degrees of freedom
  /// according to the density:
  ///
  ///   p   (x) dx =  ... dx
  ///    a,b
  ///
  /// Both paramters alpha and beta must be positive.
  ///
  /// Method: The random numbers are directly generated from ratios of
  ///         ChiSquare variates according to:
  ///
  ///  F = (ChiSquare(alpha)/alpha) / (ChiSquare(beta)/beta)
  ///
  /// </code></remarks>
  public class FisherSnedecorDistribution : ContinuousDistribution
  {
    #region instance fields

    /// <summary>
    /// Gets or sets the parameter alpha which is used for generation of Fisher-Snedecor distributed random numbers.
    /// </summary>
    /// <remarks>Call <see cref="IsValidAlpha"/> to determine whether a value is valid and therefor assignable.</remarks>
    public double Alpha
    {
      get
      {
        return alpha;
      }
      set
      {
        Initialize(value, beta);
      }
    }

    /// <summary>
    /// Stores the parameter alpha which is used for generation of Fisher-Snedecor distributed random numbers.
    /// </summary>
    private double alpha;

    /// <summary>
    /// Gets or sets the parameter beta which is used for generation of Fisher-Snedecor distributed random numbers.
    /// </summary>
    /// <remarks>Call <see cref="IsValidBeta"/> to determine whether a value is valid and therefor assignable.</remarks>
    public double Beta
    {
      get
      {
        return beta;
      }
      set
      {
        Initialize(alpha, value);
      }
    }

    /// <summary>
    /// Stores the parameter beta which is used for generation of Fisher-Snedecor distributed random numbers.
    /// </summary>
    private double beta;

    public void Initialize(double alpha, double beta)
    {
      if (!IsValidAlpha(alpha))
        throw new ArgumentOutOfRangeException("Alpha out of range (must be positive)");
      if (!IsValidBeta(beta))
        throw new ArgumentOutOfRangeException("Beta out of range (must be positive");

      this.alpha = alpha;
      this.beta = beta;
      UpdateHelpers();
    }

    /// <summary>
    /// Stores a <see cref="ChiSquareDistribution"/> object used for generation of Fisher-Snedecor distributed random
    ///   numbers and configured with parameter <see cref="alpha"/>.
    /// </summary>
    private ChiSquareDistribution chiSquareDistributionAlpha;

    /// <summary>
    /// Stores a <see cref="ChiSquareDistribution"/> object used for generation of Fisher-Snedecor distributed random
    ///   numbers and configured with parameter <see cref="beta"/>.
    /// </summary>
    private ChiSquareDistribution chiSquareDistributionBeta;

    /// <summary>
    /// Stores an intermediate result for generation of Fisher-Snedecor distributed random numbers.
    /// </summary>
    /// <remarks>
    /// Speeds up random number generation cause this value only depends on distribution parameters
    ///   and therefor doesn't need to be recalculated in successive executions of <see cref="NextDouble"/>.
    /// </remarks>
    private double helper1;

    #endregion instance fields

    #region construction

    /// <summary>
    /// Initializes a new instance of the <see cref="FisherSnedecorDistribution"/> class, using a
    ///   <see cref="StandardGenerator"/> as underlying random number generator.
    /// </summary>
    public FisherSnedecorDistribution()
      : this(DefaultGenerator)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FisherSnedecorDistribution"/> class, using the specified
    ///   <see cref="Generator"/> as underlying random number generator.
    /// </summary>
    /// <param name="generator">A <see cref="Generator"/> object.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="generator"/> is NULL (<see langword="Nothing"/> in Visual Basic).
    /// </exception>
    public FisherSnedecorDistribution(Generator generator)
      : this(1, 1, generator)
    {
    }

    public FisherSnedecorDistribution(double alpha, double beta)
      : this(alpha, beta, DefaultGenerator)
    {
    }

    public FisherSnedecorDistribution(double alpha, double beta, Generator generator)
      : base(generator)
    {
      Initialize(alpha, beta);
    }

    #endregion construction

    #region instance methods

    /// <summary>
    /// Determines whether the specified value is valid for parameter <see cref="Alpha"/>.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>
    /// <see langword="true"/> if value is greater than 0; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsValidAlpha(double value)
    {
      return value > 0;
    }

    /// <summary>
    /// Determines whether the specified value is valid for parameter <see cref="Beta"/>.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>
    /// <see langword="true"/> if value is greater than 0; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsValidBeta(double value)
    {
      return value > 0;
    }

    /// <summary>
    /// Updates the helper variables that store intermediate results for generation of Fisher-Snedecor distributed random
    ///   numbers.
    /// </summary>
    private void UpdateHelpers()
    {
      if (null == chiSquareDistributionAlpha)
        chiSquareDistributionAlpha = new ChiSquareDistribution(alpha, generator);
      else
        chiSquareDistributionAlpha.Alpha = alpha;

      if (null == chiSquareDistributionBeta)
        chiSquareDistributionBeta = new ChiSquareDistribution(beta, generator);
      else
        chiSquareDistributionBeta.Alpha = beta;

      helper1 = beta / alpha;
    }

    #endregion instance methods

    #region overridden Distribution members

    /// <summary>
    /// Gets the minimum possible value of Fisher-Snedecor distributed random numbers.
    /// </summary>
    public override double Minimum
    {
      get
      {
        return 0.0;
      }
    }

    /// <summary>
    /// Gets the maximum possible value of Fisher-Snedecor distributed random numbers.
    /// </summary>
    public override double Maximum
    {
      get
      {
        return double.MaxValue;
      }
    }

    /// <summary>
    /// Gets the mean value of Fisher-Snedecor distributed random numbers.
    /// </summary>
    public override double Mean
    {
      get
      {
        if (beta > 2)
        {
          return beta / (beta - 2.0);
        }
        else
        {
          return double.NaN;
        }
      }
    }

    /// <summary>
    /// Gets the median of Fisher-Snedecor distributed random numbers.
    /// </summary>
    public override double Median
    {
      get
      {
        return double.NaN;
      }
    }

    /// <summary>
    /// Gets the variance of Fisher-Snedecor distributed random numbers.
    /// </summary>
    public override double Variance
    {
      get
      {
        if (this.beta > 4)
        {
          double alpha = this.alpha;
          double beta = this.beta;

          return 2 * Math.Pow(this.beta, 2.0) * (alpha + beta - 2.0) / alpha / Math.Pow(this.beta - 2.0, 2.0) /
              (this.beta - 4.0);
        }
        else
        {
          return double.NaN;
        }
      }
    }

    /// <summary>
    /// Gets the mode of Fisher-Snedecor distributed random numbers.
    /// </summary>
    public override double[] Mode
    {
      get
      {
        if (this.alpha > 2)
        {
          double alpha = this.alpha;
          double beta = this.beta;

          return new double[] { (alpha - 2.0) / alpha * beta / (beta + 2.0) };
        }
        else
        {
          return new double[] { };
        }
      }
    }

    /// <summary>
    /// Returns a Fisher-Snedecor distributed floating point random number.
    /// </summary>
    /// <returns>A Fisher-Snedecor distributed double-precision floating point number.</returns>
    public override double NextDouble()
    {
      return chiSquareDistributionAlpha.NextDouble() / chiSquareDistributionBeta.NextDouble() * helper1;
    }

    #endregion overridden Distribution members

    #region CdfPdfQuantile

    /// <summary>
    /// Returns the cumulated distribution function for value x with the distribution parameters numf and denomf.
    /// </summary>
    /// <param name="x">The function argument.</param>
    /// <returns>The cumulated distribution (probability) of the distribution at value x.</returns>
    public override double CDF(double x)
    {
      return CDF(x, alpha, beta);
    }

    /// <summary>
    /// Returns the cumulated distribution function for value x with the distribution parameters numf and denomf.
    /// </summary>
    /// <param name="x">The function argument.</param>
    /// <param name="alpha">First parameter of the distribution.</param>
    /// <param name="beta">Second paramenter of the distribution.</param>
    /// <returns>The cumulated distribution (probability) of the distribution at value x.</returns>
    public static double CDF(double x, double alpha, double beta)
    {
      double n1x = alpha * x;
      return GammaRelated.BetaIR(n1x / (beta + n1x), 0.5 * alpha, 0.5 * beta);
    }

    /// <summary>
    /// Returns the probability density function for value x with the distribution parameters p and q.
    /// </summary>
    /// <param name="x">The function argument.</param>
    /// <returns>The probability density of the distribution at value x.</returns>
    public override double PDF(double x)
    {
      return PDF(x, alpha, beta);
    }

    /// <summary>
    /// Returns the probability density function for value x with the distribution parameters p and q.
    /// </summary>
    /// <param name="x">The function argument.</param>
    /// <param name="alpha">First parameter of the distribution.</param>
    /// <param name="beta">Second paramenter of the distribution.</param>
    /// <returns>The probability density of the distribution at value x.</returns>
    public static double PDF(double x, double alpha, double beta)
    {
      return (Math.Pow(alpha, alpha / 2) * Math.Pow(beta, beta / 2) * Math.Pow(x, (-2 + alpha) / 2) * Math.Pow(beta + alpha * x, (-alpha - beta) / 2)) / GammaRelated.Beta(alpha / 2, beta / 2);
    }

    /// <summary>
    /// Quantile of the F-distribution.
    /// </summary>
    /// <param name="probability">Probability (0..1).</param>
    /// <returns>The quantile of the F-Distribution.</returns>
    public override double Quantile(double probability)
    {
      return Quantile(probability, alpha, beta);
    }

    /// <summary>
    /// Quantile of the F-distribution.
    /// </summary>
    /// <param name="probability">Probability (0..1).</param>
    /// <param name="alpha">First parameter of the distribution.</param>
    /// <param name="beta">Second parameter of the distribution.</param>
    /// <returns>The quantile of the F-Distribution.</returns>
    public static double Quantile(double probability, double alpha, double beta)
    {
      double inverse_beta = GammaRelated.InverseBetaRegularized(1 - probability, beta / 2, alpha / 2);
      return (beta / alpha) * (1.0 / inverse_beta - 1.0);
    }

    #endregion CdfPdfQuantile
  }

  /// <summary>
  /// Surrogate name for <see cref="FisherSnedecorDistribution"/> and included here only for convenience. No other instance members defined.
  /// </summary>
  public class FDistribution : FisherSnedecorDistribution
  {
    #region construction

    /// <summary>
    /// Initializes a new instance of the <see cref="FDistribution"/> class, using a
    ///   <see cref="StandardGenerator"/> as underlying random number generator.
    /// </summary>
    public FDistribution()
      : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FDistribution"/> class, using the specified
    ///   <see cref="Generator"/> as underlying random number generator.
    /// </summary>
    /// <param name="generator">A <see cref="Generator"/> object.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="generator"/> is NULL (<see langword="Nothing"/> in Visual Basic).
    /// </exception>
    public FDistribution(Generator generator)
      : base(generator)
    {
    }

    public FDistribution(double alpha, double beta)
      : base(alpha, beta)
    {
    }

    public FDistribution(double alpha, double beta, Generator generator)
      : base(alpha, beta, generator)
    {
    }

    #endregion construction
  }
}
