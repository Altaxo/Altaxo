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
 * WeibullDistribution.cs, 21.09.2006
 *
 * 16.08.2006: Initial version
 * 21.09.2006: Adapted to change in base class (field "generator" declared private (formerly protected)
 *               and made accessible through new protected property "Generator")
 *
 */

using System;

namespace Altaxo.Calc.Probability
{
  /// <summary>
  /// Provides generation of weibull distributed random numbers.
  /// </summary>
  /// <remarks>
  /// The implementation of the <see cref="WeibullDistribution"/> type bases upon information presented on
  ///   <a href="http://en.wikipedia.org/wiki/Weibull_distribution">Wikipedia - Weibull distribution</a>.
  /// </remarks>
  public class WeibullDistribution : ContinuousDistribution
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
    /// Gets or sets the parameter alpha which is used for generation of weibull distributed random numbers.
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
        Initialize(value, lambda);
      }
    }

    /// <summary>
    /// Stores the parameter alpha which is used for generation of weibull distributed random numbers.
    /// </summary>
    private double alpha;

    /// <summary>
    /// Gets or sets the parameter lambda which is used for generation of erlang distributed random numbers.
    /// </summary>
    /// <remarks>Call <see cref="IsValidLambda"/> to determine whether a value is valid and therefor assignable.</remarks>
    public double Lambda
    {
      get
      {
        return lambda;
      }
      set
      {
        Initialize(alpha, value);
      }
    }

    /// <summary>
    /// Stores the parameter lambda which is used for generation of erlang distributed random numbers.
    /// </summary>
    private double lambda;

    /// <summary>
    /// Stores an intermediate result for generation of weibull distributed random numbers.
    /// </summary>
    /// <remarks>
    /// Speeds up random number generation cause this value only depends on distribution parameters
    ///   and therefor doesn't need to be recalculated in successive executions of <see cref="NextDouble"/>.
    /// </remarks>
    private double helper1;

    #endregion instance fields

    #region construction

    /// <summary>
    /// Initializes a new instance of the <see cref="WeibullDistribution"/> class, using a
    ///   <see cref="StandardGenerator"/> as underlying random number generator.
    /// </summary>
    public WeibullDistribution()
      : this(DefaultGenerator)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WeibullDistribution"/> class, using the specified
    ///   <see cref="Generator"/> as underlying random number generator.
    /// </summary>
    /// <param name="generator">A <see cref="Generator"/> object.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="generator"/> is NULL (<see langword="Nothing"/> in Visual Basic).
    /// </exception>
    public WeibullDistribution(Generator generator)
      : this(1, 1, generator)
    {
    }

    public WeibullDistribution(double alpha, double lambda)
      : this(alpha, lambda, DefaultGenerator)
    {
    }

    public WeibullDistribution(double alpha, double lambda, Generator generator)
      : base(generator)
    {
      Initialize(alpha, lambda);
    }

    #endregion construction

    #region instance methods

    public void Initialize(double alpha, double lambda)
    {
      if (!IsValidAlpha(alpha))
        throw new ArgumentOutOfRangeException("Alpha out of range");
      if (!IsValidLambda(lambda))
        throw new ArgumentOutOfRangeException("Lambda out of range");

      this.alpha = alpha;
      this.lambda = lambda;
      helper1 = 1.0 / this.alpha;
    }

    /// <summary>
    /// Determines whether the specified value is valid for parameter <see cref="Alpha"/>.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>
    /// <see langword="true"/> if value is greater than 0.0; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsValidAlpha(double value)
    {
      return value > 0.0;
    }

    /// <summary>
    /// Determines whether the specified value is valid for parameter <see cref="Lambda"/>.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>
    /// <see langword="true"/> if value is greater than 0.0; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsValidLambda(double value)
    {
      return value > 0.0;
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
      double sum = WeibullDistribution.LanczosCoefficients[0];
      for (int index = 1; index <= 6; index++)
      {
        sum += WeibullDistribution.LanczosCoefficients[index] / (x + index);
      }

      return Math.Sqrt(2.0 * Math.PI) / x * Math.Pow(x + 5.5, x + 0.5) / Math.Exp(x + 5.5) * sum;
    }

    #endregion instance methods

    #region overridden Distribution members

    /// <summary>
    /// Gets the minimum possible value of weibull distributed random numbers.
    /// </summary>
    public override double Minimum
    {
      get
      {
        return 0.0;
      }
    }

    /// <summary>
    /// Gets the maximum possible value of weibull distributed random numbers.
    /// </summary>
    public override double Maximum
    {
      get
      {
        return double.MaxValue;
      }
    }

    /// <summary>
    /// Gets the mean value of weibull distributed random numbers.
    /// </summary>
    public override double Mean
    {
      get
      {
        return lambda * Gamma(1.0 + 1.0 / alpha);
      }
    }

    /// <summary>
    /// Gets the median of weibull distributed random numbers.
    /// </summary>
    public override double Median
    {
      get
      {
        return lambda * Math.Pow(Math.Log(2.0), 1.0 / alpha);
      }
    }

    /// <summary>
    /// Gets the variance of weibull distributed random numbers.
    /// </summary>
    public override double Variance
    {
      get
      {
        return Math.Pow(lambda, 2.0) * Gamma(1.0 + 2.0 / alpha) - Math.Pow(Mean, 2.0);
      }
    }

    /// <summary>
    /// Gets the mode of weibull distributed random numbers.
    /// </summary>
    public override double[] Mode
    {
      get
      {
        if (alpha >= 1.0)
        {
          return new double[] { lambda * Math.Pow(1.0 - 1.0 / alpha, 1.0 / alpha) };
        }
        else
        {
          return new double[] { 0.0 };
        }
      }
    }

    /// <summary>
    /// Returns a weibull distributed floating point random number.
    /// </summary>
    /// <returns>A weibull distributed double-precision floating point number.</returns>
    public override double NextDouble()
    {
      // Subtract random number from 1.0 to avoid Math.Log(0.0)
      return lambda * Math.Pow(-Math.Log(1.0 - Generator.NextDouble()), helper1);
    }

    #endregion overridden Distribution members

    #region CdfPdfQuantile

    public override double CDF(double x)
    {
      return CDF(x, alpha, lambda);
    }

    public static double CDF(double x, double alpha, double lambda)
    {
      return 1 - Math.Exp(-Math.Pow(x / lambda, alpha));
    }

    public override double PDF(double x)
    {
      return PDF(x, alpha, lambda);
    }

    public static double PDF(double x, double alpha, double lambda)
    {
      return (alpha * Math.Pow(x, -1 + alpha)) / (Math.Exp(Math.Pow(x / lambda, alpha)) * Math.Pow(lambda, alpha));
    }

    public override double Quantile(double p)
    {
      return Quantile(p, alpha, lambda);
    }

    public static double Quantile(double p, double alpha, double lambda)
    {
      return lambda * Math.Pow(-Math.Log(1 - p), 1 / alpha);
    }

    #endregion CdfPdfQuantile
  }
}
