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
 * LognormalDistribution.cs, 17.08.2006
 * 
 * 09.08.2006: Initial version
 * 17.08.2006: Renamed field storing NormalDistribution object and declared it private explicitely
 *             Renamed field my and property My to mu/Mu to consistently use english names
 * 
 */

#region original copyright
/* boost random/lognormal_distribution.hpp header file
 *
 * Copyright Jens Maurer 2000-2001
 * Distributed under the Boost Software License, Version 1.0. (See
 * accompanying file LICENSE_1_0.txt or copy at
 * http://www.boost.org/LICENSE_1_0.txt)
 *
 * See http://www.boost.org for most recent version including documentation.
 *
 * $Id: lognormal_distribution.hpp,v 1.16 2004/07/27 03:43:32 dgregor Exp $
 *
 * Revision history
 *  2001-02-18  moved to individual header files
 */
#endregion

using System;

namespace Altaxo.Calc.Probability
{
	/// <summary>
	/// Provides generation of lognormal distributed random numbers.
	/// </summary>
	/// <remarks>
    /// The implementation of the <see cref="LognormalDistribution"/> type bases upon information presented on
    ///   <a href="http://en.wikipedia.org/wiki/Log-normal_distribution">Wikipedia - Lognormal Distribution</a> and
    ///   the implementation in the <a href="http://www.boost.org/libs/random/index.html">Boost Random Number Library</a>.
    /// </remarks>
  public class LognormalDistribution : ContinuousDistribution
  {
    #region instance fields
    /// <summary>
    /// Gets or sets the parameter mu which is used for generation of lognormal distributed random numbers.
    /// </summary>
    /// <remarks>Call <see cref="IsValidMu"/> to determine whether a value is valid and therefor assignable.</remarks>
    public double Mu
    {
      get
      {
        return this.mu;
      }
      set
      {
        if (this.IsValidMu(value))
        {
          this.mu = value;
        }
      }
    }

    /// <summary>
    /// Stores the parameter mu which is used for generation of lognormal distributed random numbers.
    /// </summary>
    private double mu;

    /// <summary>
    /// Gets or sets the parameter sigma which is used for generation of lognormal distributed random numbers.
    /// </summary>
    /// <remarks>Call <see cref="IsValidSigma"/> to determine whether a value is valid and therefor assignable.</remarks>
    public double Sigma
    {
      get
      {
        return this.sigma;
      }
      set
      {
        if (this.IsValidSigma(value))
        {
          this.sigma = value;
        }
      }
    }

    /// <summary>
    /// Stores the parameter sigma which is used for generation of lognormal distributed random numbers.
    /// </summary>
    private double sigma;

    /// <summary>
    /// Stores a <see cref="NormalDistribution"/> object used for generation of lognormal distributed random numbers.
    /// </summary>
    private NormalDistribution normalDistribution;
    #endregion

    #region construction
    /// <summary>
    /// Initializes a new instance of the <see cref="LognormalDistribution"/> class, using a 
    ///   <see cref="StandardGenerator"/> as underlying random number generator.
    /// </summary>
    public LognormalDistribution()
      : this(new StandardGenerator())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LognormalDistribution"/> class, using the specified 
    ///   <see cref="Generator"/> as underlying random number generator.
    /// </summary>
    /// <param name="generator">A <see cref="Generator"/> object.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="generator"/> is NULL (<see langword="Nothing"/> in Visual Basic).
    /// </exception>
    public LognormalDistribution(Generator generator)
      : base(generator)
    {
      this.mu = 1.0;
      this.sigma = 1.0;
      this.normalDistribution = new NormalDistribution(generator);
      this.normalDistribution.Mu = 0.0;
      this.normalDistribution.Sigma = 1.0;
    }
    #endregion

    #region instance methods
    /// <summary>
    /// Determines whether the specified value is valid for parameter <see cref="Mu"/>.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns><see langword="true"/>.</returns>
    public bool IsValidMu(double value)
    {
      return true;
    }

    /// <summary>
    /// Determines whether the specified value is valid for parameter <see cref="Sigma"/>.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>
    /// <see langword="true"/> if value is greater than or equal to 0.0; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsValidSigma(double value)
    {
      return value >= 0.0;
    }
    #endregion

    #region overridden Distribution members
    /// <summary>
    /// Gets the minimum possible value of lognormal distributed random numbers.
    /// </summary>
    public override double Minimum
    {
      get
      {
        return 0.0;
      }
    }

    /// <summary>
    /// Gets the maximum possible value of lognormal distributed random numbers.
    /// </summary>
    public override double Maximum
    {
      get
      {
        return double.MaxValue;
      }
    }

    /// <summary>
    /// Gets the mean value of lognormal distributed random numbers.
    /// </summary>
    public override double Mean
    {
      get
      {
        return Math.Exp(this.mu + 0.5 * Math.Pow(this.sigma, 2.0));
      }
    }

    /// <summary>
    /// Gets the median of lognormal distributed random numbers.
    /// </summary>
    public override double Median
    {
      get
      {
        return Math.Exp(this.mu);
      }
    }

    /// <summary>
    /// Gets the variance of lognormal distributed random numbers.
    /// </summary>
    public override double Variance
    {
      get
      {
        return (Math.Exp(Math.Pow(this.sigma, 2.0)) - 1.0) * Math.Exp(2.0 * this.mu + Math.Pow(this.sigma, 2.0));
      }
    }

    /// <summary>
    /// Gets the mode of lognormal distributed random numbers.
    /// </summary>
    public override double[] Mode
    {
      get
      {
        return new double[] { Math.Exp(this.mu - Math.Pow(this.sigma, 2.0)) };
      }
    }

    /// <summary>
    /// Returns a lognormal distributed floating point random number.
    /// </summary>
    /// <returns>A lognormal distributed double-precision floating point number.</returns>
    public override double NextDouble()
    {
      return Math.Exp(this.normalDistribution.NextDouble() * this.sigma + this.mu);
    }
    #endregion

    #region CdFPdfQuantile
    static double Sqr(double x) { return x * x; }
    static readonly double _OneBySqrt2Pi = 1 / Math.Sqrt(2 * Math.PI);
    static readonly double _OneBySqrt2 = 1 / Math.Sqrt(2);

    public override double CDF(double z)
    {
      return CDF(z, mu, sigma);
    }
    public static double CDF(double z, double m, double s)
    {
      return 0.5 * (1 + Altaxo.Calc.ErrorFunction.Erf(_OneBySqrt2 * (Math.Log(z) - m) / s));
    }

    public override double PDF(double z)
    {
      return PDF(z, mu, sigma);
    }
    public static double PDF(double z, double m, double s)
    {
      if (z <= 0)
        return 0;
      else
        return _OneBySqrt2Pi * Math.Exp(-0.5 * Sqr((Math.Log(z) - m) / s)) / (s * z);
    }


    public override double Quantile(double p)
    {
      return Quantile(p, mu, sigma);
    }
    public static double Quantile(double p, double m, double s)
    {
      return Math.Exp(m + s * ErrorFunction.QuantileOfNormalDistribution01(p));
    }


    #endregion
  }
}