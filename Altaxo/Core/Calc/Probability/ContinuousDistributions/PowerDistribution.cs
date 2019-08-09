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
 * PowerDistribution.cs, 21.09.2006
 *
 * 09.08.2006: Initial version
 * 17.08.2006: Moved call to UpdateHelpers from setter of Beta to setter of Alpha property where it's needed
 * 21.09.2006: Adapted to change in base class (field "generator" declared private (formerly protected)
 *               and made accessible through new protected property "Generator")
 *
 */

using System;

namespace Altaxo.Calc.Probability
{
  /// <summary>
  /// Provides generation of power distributed random numbers.
  /// </summary>
  /// <remarks>
  /// The implementation of the <see cref="PowerDistribution"/> type bases upon information presented on
  ///   <a href="http://www.xycoon.com/power.htm">Xycoon - Power Distribution</a>.
  /// </remarks>
  public class PowerDistribution : ContinuousDistribution
  {
    #region instance fields

    /// <summary>
    /// Gets or sets the parameter alpha which is used for generation of power distributed random numbers.
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
    /// Stores the parameter alpha which is used for generation of power distributed random numbers.
    /// </summary>
    private double alpha;

    /// <summary>
    /// Gets or sets the parameter beta which is used for generation of power distributed random numbers.
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
    /// Stores the parameter beta which is used for generation of power distributed random numbers.
    /// </summary>
    private double beta;

    /// <summary>
    /// Stores an intermediate result for generation of power distributed random numbers.
    /// </summary>
    /// <remarks>
    /// Speeds up random number generation cause this value only depends on distribution parameters
    ///   and therefor doesn't need to be recalculated in successive executions of <see cref="NextDouble"/>.
    /// </remarks>
    private double helper1;

    #endregion instance fields

    #region construction

    /// <summary>
    /// Initializes a new instance of the <see cref="PowerDistribution"/> class, using a
    ///   <see cref="StandardGenerator"/> as underlying random number generator.
    /// </summary>
    public PowerDistribution()
      : this(DefaultGenerator)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PowerDistribution"/> class, using the specified
    ///   <see cref="Generator"/> as underlying random number generator.
    /// </summary>
    /// <param name="generator">A <see cref="Generator"/> object.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="generator"/> is NULL (<see langword="Nothing"/> in Visual Basic).
    /// </exception>
    public PowerDistribution(Generator generator)
      : this(1, 1, generator)
    {
    }

    public PowerDistribution(double alpha, double beta)
      : this(alpha, beta, DefaultGenerator)
    {
    }

    public PowerDistribution(double alpha, double beta, Generator generator)
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
    /// <see langword="true"/> if value is greater than 0.0; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsValidAlpha(double value)
    {
      return value > 0.0;
    }

    /// <summary>
    /// Determines whether the specified value is valid for parameter <see cref="Beta"/>.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>
    /// <see langword="true"/> if value is greater than 0.0; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsValidBeta(double value)
    {
      return value > 0.0;
    }

    /// <summary>
    /// Updates the helper variables that store intermediate results for generation of power distributed random
    ///   numbers.
    /// </summary>
    public void Initialize(double alpha, double beta)
    {
      if (!IsValidAlpha(alpha))
        throw new ArgumentOutOfRangeException("Alpha out of range (must be positive)");
      if (!IsValidBeta(beta))
        throw new ArgumentOutOfRangeException("Beta out of range (must be positive)");

      this.alpha = alpha;
      this.beta = beta;
      helper1 = 1.0 / this.alpha;
    }

    #endregion instance methods

    #region overridden Distribution members

    /// <summary>
    /// Gets the minimum possible value of power distributed random numbers.
    /// </summary>
    public override double Minimum
    {
      get
      {
        return 0.0;
      }
    }

    /// <summary>
    /// Gets the maximum possible value of power distributed random numbers.
    /// </summary>
    public override double Maximum
    {
      get
      {
        return 1.0 / beta;
      }
    }

    /// <summary>
    /// Gets the mean value of power distributed random numbers.
    /// </summary>
    public override double Mean
    {
      get
      {
        return alpha / beta / (alpha + 1.0);
      }
    }

    /// <summary>
    /// Gets the median of power distributed random numbers.
    /// </summary>
    public override double Median
    {
      get
      {
        return double.NaN;
      }
    }

    /// <summary>
    /// Gets the variance of power distributed random numbers.
    /// </summary>
    public override double Variance
    {
      get
      {
        return alpha / Math.Pow(beta, 2.0) / Math.Pow(alpha + 1.0, 2.0) / (alpha + 2.0);
      }
    }

    /// <summary>
    /// Gets the mode of power distributed random numbers.
    /// </summary>
    public override double[] Mode
    {
      get
      {
        if (alpha > 1.0)
        {
          return new double[] { 1.0 / beta };
        }
        else if (alpha < 1.0)
        {
          return new double[] { 0.0 };
        }
        else
        {
          return new double[] { };
        }
      }
    }

    /// <summary>
    /// Returns a power distributed floating point random number.
    /// </summary>
    /// <returns>A power distributed double-precision floating point number.</returns>
    public override double NextDouble()
    {
      return Math.Pow(Generator.NextDouble(), helper1) / beta;
    }

    #endregion overridden Distribution members

    #region CdfPdfQuantile

    public override double CDF(double x)
    {
      return CDF(x, alpha, beta);
    }

    public static double CDF(double x, double A, double B)
    {
      if (x <= 0)
        return 0;
      if (x >= 1 / B)
        return 1;
      else
        return Math.Pow(B * x, A);
    }

    public override double PDF(double x)
    {
      return PDF(x, alpha, beta);
    }

    public static double PDF(double x, double A, double B)
    {
      if (x <= 0 || x >= 1 / B)
      {
        return 0;
      }
      else
      {
        return A * Math.Pow(B * x, A) / x;
      }
    }

    public override double Quantile(double p)
    {
      return Quantile(p, alpha, beta);
    }

    public static double Quantile(double p, double A, double B)
    {
      return Math.Pow(p, 1 / A) / B;
    }

    #endregion CdfPdfQuantile
  }
}
