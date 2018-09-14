#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

#region Original Copyright(s)

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

#endregion original copyright

#endregion Original Copyright(s)

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
  /// <code>
  /// Return log-normal distributed random deviates
  /// with given mean and standard deviation stdev
  /// according to the density function:
  ///                                                2
  ///                     1                (ln x - m)
  /// p   (x) dx =  -------------- exp( - ------------ ) dx  for x > 0
  ///  m,s          sqrt(2 pi x) s               2
  ///                                         2 s
  ///
  ///            = 0   otherwise
  ///
  /// where  m  and  s  are related to the arguments mean and stdev by:
  ///
  ///                        2
  ///                    mean
  /// m = ln ( --------------------- )
  ///                     2      2
  ///          sqrt( stdev + mean  )
  ///
  ///                     2      2
  ///                stdev + mean
  /// s = sqrt( ln( -------------- ) )
  ///                        2
  ///                    mean
  ///
  ///
  /// </code></remarks>
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
        return mu;
      }
      set
      {
        Initialize(value, sigma);
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
        return sigma;
      }
      set
      {
        Initialize(mu, value);
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

    #endregion instance fields

    #region construction

    /// <summary>
    /// Initializes a new instance of the <see cref="LognormalDistribution"/> class, using a
    ///   <see cref="StandardGenerator"/> as underlying random number generator.
    /// </summary>
    public LognormalDistribution()
      : this(DefaultGenerator)
    {
    }

    public LognormalDistribution(Generator generator)
      : this(0, 1, generator)
    {
    }

    public LognormalDistribution(double mu, double sigma)
      : this(mu, sigma, DefaultGenerator)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LognormalDistribution"/> class, using the specified
    ///   <see cref="Generator"/> as underlying random number generator.
    /// </summary>
    /// <param name="mu">First parameter of the distribution.</param>
    /// <param name="sigma">Second parameter of the distribution.</param>
    /// <param name="generator">A <see cref="Generator"/> object.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="generator"/> is NULL (<see langword="Nothing"/> in Visual Basic).
    /// </exception>
    public LognormalDistribution(double mu, double sigma, Generator generator)
      : base(generator)
    {
      normalDistribution = new NormalDistribution(0, 1, generator);
      Initialize(mu, sigma);
    }

    #endregion construction

    #region instance methods

    public void Initialize(double mu, double sigma)
    {
      if (!IsValidMu(mu))
        throw new ArgumentOutOfRangeException("Mu is out of range (Infinity and NaN now allowed)");
      if (!IsValidSigma(sigma))
        throw new ArgumentOutOfRangeException("Sigma is out of range (must be positive)");

      this.mu = mu;
      this.sigma = sigma;
    }

    /// <summary>
    /// Determines whether the specified value is valid for parameter <see cref="Mu"/>.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns><see langword="true"/>.</returns>
    public bool IsValidMu(double value)
    {
      return value >= double.MinValue && value <= double.MaxValue;
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

    #endregion instance methods

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
        return Math.Exp(mu + 0.5 * Math.Pow(sigma, 2.0));
      }
    }

    /// <summary>
    /// Gets the median of lognormal distributed random numbers.
    /// </summary>
    public override double Median
    {
      get
      {
        return Math.Exp(mu);
      }
    }

    /// <summary>
    /// Gets the variance of lognormal distributed random numbers.
    /// </summary>
    public override double Variance
    {
      get
      {
        return (Math.Exp(Math.Pow(sigma, 2.0)) - 1.0) * Math.Exp(2.0 * mu + Math.Pow(sigma, 2.0));
      }
    }

    /// <summary>
    /// Gets the mode of lognormal distributed random numbers.
    /// </summary>
    public override double[] Mode
    {
      get
      {
        return new double[] { Math.Exp(mu - Math.Pow(sigma, 2.0)) };
      }
    }

    /// <summary>
    /// Returns a lognormal distributed floating point random number.
    /// </summary>
    /// <returns>A lognormal distributed double-precision floating point number.</returns>
    public override double NextDouble()
    {
      return Math.Exp(normalDistribution.NextDouble() * sigma + mu);
    }

    #endregion overridden Distribution members

    #region CdFPdfQuantile

    private static double Sqr(double x)
    {
      return x * x;
    }

    private static readonly double _OneBySqrt2Pi = 1 / Math.Sqrt(2 * Math.PI);
    private static readonly double _OneBySqrt2 = 1 / Math.Sqrt(2);

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

    #endregion CdFPdfQuantile
  }
}
