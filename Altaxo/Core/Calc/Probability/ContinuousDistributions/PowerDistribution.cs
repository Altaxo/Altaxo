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
  /// Provides generation of power-distributed random numbers.
  /// </summary>
  /// <remarks>
  /// The implementation of the <see cref="PowerDistribution"/> and the order of parameters is based on
  /// the implementation in Mathematica.
  /// </remarks>
  public class PowerDistribution : ContinuousDistribution
  {
    #region instance fields

    /// <summary>
    /// Gets or sets the parameter alpha which is used for generation of power distributed random numbers.
    /// </summary>
    /// <remarks>Call <see cref="IsValidAlpha"/> to determine whether a value is valid and therefor assignable.</remarks>
    public double A
    {
      get
      {
        return _a;
      }
      set
      {
        Initialize(_k, value);
      }
    }

    /// <summary>
    /// Stores the shape parameter a which is used for generation of power distributed random numbers.
    /// </summary>
    private double _a;

    /// <summary>
    /// Gets or sets the domain parameter k which is used for generation of power distributed random numbers.
    /// </summary>
    /// <remarks>Call <see cref="IsValidBeta"/> to determine whether a value is valid and therefor assignable.</remarks>
    public double K
    {
      get
      {
        return _k;
      }
      set
      {
        Initialize(value, _a);
      }
    }

    /// <summary>
    /// Stores the domain parameter which is used for generation of power distributed random numbers.
    /// </summary>
    private double _k;

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

    /// <summary>
    /// Initializes a new instance of the <see cref="PowerDistribution"/> class with
    /// domain parameter <paramref name="k"/> and shape parameter <paramref name="a"/>.
    /// </summary>
    /// <param name="k">The domain parameter k.</param>
    /// <param name="a">The shape parameter a.</param>
    public PowerDistribution(double k, double a)
      : this(k, a, DefaultGenerator)
        {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PowerDistribution"/> class with
    /// domain parameter <paramref name="k"/> and shape parameter <paramref name="a"/> using the specified
    /// random number generator.
    /// </summary>
    /// <param name="k">The domain parameter k.</param>
    /// <param name="a">The shape parameter a.</param>
    /// <param name="generator">The random number generator.</param>
    public PowerDistribution(double k, double a, Generator generator)
      : base(generator)
    {
      Initialize(k, a);
    }

    #endregion construction

    #region instance methods

    /// <summary>
    /// Determines whether the specified value is valid for parameter <see cref="A"/>.
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
    /// Determines whether the specified value is valid for parameter <see cref="K"/>.
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
    /// Updates the helper variables that store intermediate results for generation of power-distributed random
    /// numbers.
    /// </summary>
    /// <param name="k">The domain parameter <c>k</c>.</param>
    /// <param name="a">The shape parameter <c>a</c>.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="a"/> is not valid.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="k"/> is not valid.
    /// </exception>
    public void Initialize(double k, double a)
    {
      if (!IsValidAlpha(a))
        throw new ArgumentOutOfRangeException($"Parameter {nameof(a)} out of range (must be positive)");
      if (!IsValidBeta(k))
        throw new ArgumentOutOfRangeException($"Parameter {nameof(k)} out of range (must be positive)");

      this._a = a;
      this._k = k;
      helper1 = 1.0 / this._a;
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
        return 1.0 / _k;
      }
    }

    /// <inheritdoc/>
    public override double Mean
    {
      get
      {
        return _a / _k / (_a + 1.0);
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
        return _a / Math.Pow(_k, 2.0) / Math.Pow(_a + 1.0, 2.0) / (_a + 2.0);
      }
    }

    /// <inheritdoc/>
    public override double[] Mode
    {
      get
      {
        if (_a > 1.0)
        {
          return new double[] { 1.0 / _k };
        }
        else if (_a < 1.0)
        {
          return new double[] { 0.0 };
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
      return Math.Pow(Generator.NextDouble(), helper1) / _k;
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
      return CDF(x, _k, _a);
    }

    /// <summary>
    /// Calculates the cumulative distribution function at the specified <paramref name="x"/>.
    /// </summary>
    /// <param name="x">The value at which to evaluate the CDF.</param>
    /// <param name="k">The domain parameter <c>k</c>.</param>
    /// <param name="a">The shape parameter <c>a</c>.</param>
    /// <returns>
    /// The probability that the random variable is less than or equal to <paramref name="x"/>.
    /// </returns>
    public static double CDF(double x, double k, double a)
    {
      if (0 < x && x <= 1 / k)
      {
        return Math.Pow(k * x, a);
      }
      else if (x <= 0)
      {
        return 0;
      }
      else
      {
        return 1;
      }
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
      return PDF(x, _k, _a);
    }

    /// <summary>
    /// Calculates the probability density function at the specified <paramref name="x"/>.
    /// </summary>
    /// <param name="x">The value at which to evaluate the PDF.</param>
    /// <param name="k">The domain parameter <c>k</c>.</param>
    /// <param name="a">The shape parameter <c>a</c>.</param>
    /// <returns>The relative likelihood for the random variable to occur at <paramref name="x"/>.</returns>
    public static double PDF(double x, double k, double a)
    {
      if (0 < x && x <= 1 / k)
      {
        return a * Math.Pow(k * x, a) / x;
      }
      else
      {
        return 0;
      }
    }

    /// <summary>
    /// Calculates the quantile of the distribution function.
    /// </summary>
    /// <param name="p">The probability p.</param>
    /// <returns>
    /// The point x at which the cumulative distribution function <see cref="CDF(double)"/> of argument x is equal to <paramref name="p" />.
    /// </returns>
    public override double Quantile(double p)
    {
      return Quantile(p, _k, _a);
    }

    /// <summary>
    /// Calculates the quantile at the specified probability <paramref name="p"/>.
    /// </summary>
    /// <param name="p">The probability.</param>
    /// <param name="k">The domain parameter <c>k</c>.</param>
    /// <param name="a">The shape parameter <c>a</c>.</param>
    /// <returns>The quantile at the specified probability <paramref name="p"/>.</returns>
    public static double Quantile(double p, double k, double a)
    {
      if (0 < p && p <= 1)
      {
        return Math.Pow(p, 1 / a) / k;
      }
      else if(p<=0)
      {
        return 0;
      }
      else
      {
        return 1 / k;
      }

    }

    #endregion CdfPdfQuantile
  }
}
