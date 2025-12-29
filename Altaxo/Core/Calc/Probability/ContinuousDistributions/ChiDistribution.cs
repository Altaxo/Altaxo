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
 * ChiDistribution.cs, 17.08.2006
 *
 */

using System;

namespace Altaxo.Calc.Probability
{
  /// <summary>
  /// Provides generation of chi distributed random numbers.
  /// </summary>
  /// <remarks>
  /// The implementation of the <see cref="ChiDistribution"/> type is based upon information presented on
  ///   <a href="http://en.wikipedia.org/wiki/Chi_distribution">Wikipedia - Chi distribution</a>.
  /// </remarks>
  public class ChiDistribution : ContinuousDistribution
  {
    #region class fields

    /// <summary>
    /// Represents coefficients for the Lanczos approximation of the Gamma function.
    /// </summary>
    private static readonly double[] LanczosCoefficients = new double[] { 1.000000000190015, 76.18009172947146,
            -86.50532032941677, 24.01409824083091, -1.231739572450155, 1.208650973866179e-3, -5.395239384953e-6 };

    #endregion class fields

    #region instance fields

    /// <summary>
    /// Gets or sets the parameter N (degree of freedom) which is used for generation of chi distributed random numbers.
    /// </summary>
    /// <remarks>Call <see cref="IsValidN"/> to determine whether a value is valid and therefor assignable.</remarks>
    public int N
    {
      get
      {
        return _N;
      }
      set
      {
        Initialize(value);
      }
    }

    /// <summary>
    /// Stores the parameter alpha which is used for generation of chi distributed random numbers.
    /// </summary>
    private int _N;

    /// <summary>
    /// Stores a <see cref="NormalDistribution"/> object used for generation of chi distributed random numbers.
    /// </summary>
    private NormalDistribution _normalDistribution;

    #endregion instance fields

    #region construction

    /// <summary>
    /// Initializes a new instance of the <see cref="ChiDistribution"/> class, using a
    ///   <see cref="StandardGenerator"/> as underlying random number generator.
    /// </summary>
    public ChiDistribution()
      : this(DefaultGenerator)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChiDistribution"/> class, using the specified
    ///   <see cref="Generator"/> as underlying random number generator.
    /// </summary>
    /// <param name="generator">A <see cref="Generator"/> object.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="generator"/> is NULL (<see langword="Nothing"/> in Visual Basic).
    /// </exception>
    public ChiDistribution(Generator generator)
      : this(1, generator)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChiDistribution"/> class, using the default random number generator.
    /// </summary>
    /// <param name="N">Parameter of the distribution.</param>
    public ChiDistribution(int N)
      : this(N, DefaultGenerator)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChiDistribution"/> class, using the specified
    ///   <see cref="Generator"/> as underlying random number generator.
    /// </summary>
    /// <param name="N">Parameter of the distribution.</param>
    /// <param name="generator">A <see cref="Generator"/> object.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="generator"/> is NULL (<see langword="Nothing"/> in Visual Basic).
    /// </exception>
    public ChiDistribution(int N, Generator generator)
      : base(generator)
    {
      _normalDistribution = new NormalDistribution(0, 1, generator);
      Initialize(N);
    }

    #endregion construction

    #region instance methods

    /// <summary>
    /// Initializes the distribution with the specified degrees of freedom.
    /// </summary>
    /// <param name="N">Degrees of freedom.</param>
    public void Initialize(int N)
    {
      if (!IsValidN(N))
        throw new ArgumentOutOfRangeException("N out of range (must be &gt;0)");
      _N = N;
    }

    /// <summary>
    /// Determines whether the specified value is valid for parameter <see cref="N"/>.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>Returns <see langword="true"/> if value is greater than 0; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsValidN(int value)
    {
      return value > 0;
    }

    /// <summary>
    /// Represents a Lanczos approximation of the Gamma function.
    /// </summary>
    /// <param name="x">A double-precision floating point number.</param>
    /// <returns>
    /// A double-precision floating point number representing an approximation of Gamma(<paramref name="x"/>).
    /// </returns>
    private double Gamma(double x)
    {
      double sum = ChiDistribution.LanczosCoefficients[0];
      for (int index = 1; index <= 6; index++)
      {
        sum += ChiDistribution.LanczosCoefficients[index] / (x + index);
      }

      return Math.Sqrt(2.0 * Math.PI) / x * Math.Pow(x + 5.5, x + 0.5) / Math.Exp(x + 5.5) * sum;
    }

    #endregion instance methods

    #region overridden Distribution members

    /// <inheritdoc/>
    public override double Minimum
    {
      get
      {
        return 0.0;
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
        return Math.Sqrt(2.0) * Gamma((_N + 1.0) / 2.0) / Gamma(_N / 2.0);
      }
    }

    /// <inheritdoc/>
    public override double Median
    {
      get
      {
        return double.NaN;
      }
    }

    /// <inheritdoc/>
    public override double Variance
    {
      get
      {
        return _N - Math.Pow(Mean, 2.0);
      }
    }

    /// <inheritdoc/>
    public override double[] Mode
    {
      get
      {
        if (_N >= 1)
        {
          return new double[] { Math.Sqrt(_N - 1.0) };
        }
        else
        {
          return new double[] { };
        }
      }
    }

    /// <inheritdoc/>
    public override double NextDouble()
    {
      double sum = 0.0;
      for (int i = 0; i < _N; i++)
      {
        sum += Math.Pow(_normalDistribution.NextDouble(), 2);
      }

      return Math.Sqrt(sum);
    }

    #endregion overridden Distribution members

    #region CdfPdfQuantile

    /// <inheritdoc/>
    public override double CDF(double x)
    {
      return CDF(x, _N);
    }

    /// <summary>
    /// Calculates the cumulative distribution function for the specified degrees of freedom.
    /// </summary>
    /// <param name="x">Argument.</param>
    /// <param name="N">Degrees of freedom.</param>
    /// <returns>
    /// The probability that the random variable is less than or equal to <paramref name="x"/>.
    /// </returns>
    public static double CDF(double x, double N)
    {
      return Calc.GammaRelated.GammaRegularized(0.5 * N, 0, 0.5 * x * x);
    }

    /// <inheritdoc/>
    public override double PDF(double x)
    {
      return PDF(x, _N);
    }

    /// <summary>
    /// Calculates the probability density function for the specified degrees of freedom.
    /// </summary>
    /// <param name="x">Argument.</param>
    /// <param name="N">Degrees of freedom.</param>
    /// <returns>The relative likelihood for the random variable to occur at <paramref name="x"/>.</returns>
    public static double PDF(double x, double N)
    {
      return (Math.Pow(2, 1 - N / 2) * Math.Pow(x, -1 + N)) / (Math.Exp(x * x * 0.5) * GammaRelated.Gamma(N / 2));
    }

    /// <inheritdoc/>
    public override double Quantile(double p)
    {
      return Quantile(p, _N);
    }

    /// <summary>
    /// Calculates the quantile function for the specified degrees of freedom.
    /// </summary>
    /// <param name="p">The probability.</param>
    /// <param name="N">Degrees of freedom.</param>
    /// <returns>
    /// The point <c>x</c> at which the cumulative distribution function is equal to <paramref name="p"/>.
    /// </returns>
    public static double Quantile(double p, double N)
    {
      return Math.Sqrt(2) * Math.Sqrt(GammaRelated.InverseGammaRegularized(N / 2, 1 - p));
    }

    #endregion CdfPdfQuantile
  }
}
