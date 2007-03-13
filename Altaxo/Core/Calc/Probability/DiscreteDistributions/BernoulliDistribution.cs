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
 * BernoulliDistribution.cs, 21.09.2006
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
  /// Provides generation of bernoulli distributed random numbers.
  /// </summary>
  /// <remarks>
  /// The bernoulli distribution generates only discrete numbers.<br />
  /// The implementation of the <see cref="BernoulliDistribution"/> type bases upon information presented on
  ///   <a href="http://en.wikipedia.org/wiki/Bernoulli_distribution">Wikipedia - Bernoulli distribution</a>.
  /// </remarks>
  public class BernoulliDistribution : DiscreteDistribution
  {
    #region instance fields
    /// <summary>
    /// Gets or sets the parameter alpha which is used for generation of bernoulli distributed random numbers.
    /// </summary>
    /// <remarks>Call <see cref="IsValidAlpha"/> to determine whether a value is valid and therefor assignable.</remarks>
    public double Alpha
    {
      get
      {
        return this.alpha;
      }
      set
      {
        Initialize(value);
      }
    }

    public void Initialize(double probability)
    {
      if (IsValidAlpha(probability))
        this.alpha = probability;
      else
        throw new ArgumentOutOfRangeException("probability has to be between 0 and 1");

    }

    /// <summary>
    /// Stores the parameter alpha which is used for generation of bernoulli distributed random numbers.
    /// </summary>
    private double alpha;
    #endregion

    #region construction
    /// <summary>
    /// Initializes a new instance of the <see cref="BernoulliDistribution"/> class, using a 
    ///   <see cref="StandardGenerator"/> as underlying random number generator.
    /// </summary>
    public BernoulliDistribution()
      : this(new StandardGenerator())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BernoulliDistribution"/> class, using the specified 
    ///   <see cref="Generator"/> as underlying random number generator.
    /// </summary>
    /// <param name="generator">A <see cref="Generator"/> object.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="generator"/> is NULL (<see langword="Nothing"/> in Visual Basic).
    /// </exception>
    public BernoulliDistribution(Generator generator)
      : this(0.5, generator)
    {
    }

    public BernoulliDistribution(double probability)
      : this(probability, new StandardGenerator())
    {
    }

    public BernoulliDistribution(double probability, Generator generator)
      : base(generator)
    {
      this.alpha = probability;
    }

    #endregion

    #region instance methods
    /// <summary>
    /// Determines whether the specified value is valid for parameter <see cref="Alpha"/>.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>
    /// <see langword="true"/> if value is greater than or equal to 0.0, and less than or equal to 1.0; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsValidAlpha(double value)
    {
      return (value >= 0.0 && value <= 1.0);
    }

    /// <summary>
    /// Returns a bernoulli distributed random number.
    /// </summary>
    /// <returns>A bernoulli distributed 32-bit signed integer.</returns>
    public int Next()
    {
      if (this.Generator.NextDouble() < this.alpha)
      {
        return 1;
      }
      else
      {
        return 0;
      }
    }
    #endregion

    #region overridden Distribution members
    /// <summary>
    /// Gets the minimum possible value of bernoulli distributed random numbers.
    /// </summary>
    public override double Minimum
    {
      get
      {
        return 0.0;
      }
    }

    /// <summary>
    /// Gets the maximum possible value of bernoulli distributed random numbers.
    /// </summary>
    public override double Maximum
    {
      get
      {
        return 1.0;
      }
    }

    /// <summary>
    /// Gets the mean value of bernoulli distributed random numbers.
    /// </summary>
    public override double Mean
    {
      get
      {
        return this.alpha;
      }
    }

    /// <summary>
    /// Gets the median of bernoulli distributed random numbers.
    /// </summary>
    public override double Median
    {
      get
      {
        return double.NaN;
      }
    }

    /// <summary>
    /// Gets the variance of bernoulli distributed random numbers.
    /// </summary>
    public override double Variance
    {
      get
      {
        return this.alpha * (1.0 - this.alpha);
      }
    }

    /// <summary>
    /// Gets the mode of bernoulli distributed random numbers.
    /// </summary>
    public override double[] Mode
    {
      get
      {
        if (this.alpha > (1 - this.alpha))
        {
          return new double[] { 1.0 };
        }
        else if (this.alpha < (1 - this.alpha))
        {
          return new double[] { 0.0 };
        }
        else
        {
          return new double[] { 0.0, 1.0 };
        }
      }
    }

    /// <summary>
    /// Returns a bernoulli distributed floating point random number.
    /// </summary>
    /// <returns>A bernoulli distributed double-precision floating point number.</returns>
    public override double NextDouble()
    {
      if (this.Generator.NextDouble() < this.alpha)
      {
        return 1.0;
      }
      else
      {
        return 0.0;
      }
    }
    #endregion

    #region CdfPdf
    public override double CDF(double x)
    {
      return CDF(x, this.Alpha);
    }
    public static double CDF(double x, double p)
    {
      if (x >= 0 && x < 1)
        return 1 - p;
      else if (x >= 1)
        return 1;
      else
        return 0;
    }

    public override double PDF(double x)
    {
      return PDF(x, this.Alpha);
    }
    public static double PDF(double x, double p)
    {
      if (x == 0)
        return 1 - p;
      else if (x == 1)
        return p;
      else
        return 0;
    }


    #endregion
  }
}