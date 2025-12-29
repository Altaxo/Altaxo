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

#region Further copyright(s)

// This file is partly based on Matpack 1.7.3 sources (Author B.Gammel)
// The following Matpack files were used here:
// matpack-1.7.3\source\random\ranpoisson.cc

// This file is partly based on Troschuetz.Random Class Library (Author S.Troschuetz)

#endregion Further copyright(s)

using System;

namespace Altaxo.Calc.Probability
{
  #region PoissonDistribution (Discrete)

  /// <summary>
  /// Generates Poisson distributed random numbers.
  /// </summary>
  /// <remarks><code>
  /// Returns a Poisson distributed deviate (integer returned in a double)
  /// from a distribution of mean m.
  /// The Poisson distribution gives the probability of a certain integer
  /// number m of unit rate Poisson random events occurring in a given
  /// interval of time x.
  ///                                   j  -x
  ///              j+eps               x  e
  ///      integral       p (m) dm  = -------
  ///              j-eps   x            j !
  ///
  /// References: The method follows the outlines of:
  /// W. H. Press, B. P. Flannery, S. A. Teukolsky, W. T. Vetterling,
  /// Numerical Recipes in C, Cambridge Univ. Press, 1988.
  /// </code></remarks>
  public class PoissonDistribution : DiscreteDistribution
  {
    protected double scale, scalepi, m, sq, alm, g;

    /// <summary>
    /// Initializes this instance with the specified mean value.
    /// </summary>
    /// <param name="mean">The mean (λ) of the Poisson distribution.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="mean"/> is not a valid number.</exception>
    public void Initialize(double mean)
    {
      if (!IsValidMu(mean))
        throw new ArgumentOutOfRangeException("Mean out of range (infinity or NaN?)");

      m = mean;
      scale = 1.0 / generator.Maximum;

      if (m < 12.0)
      { // direct method
        g = Math.Exp(-m);
      }
      else
      {   // rejection method
        scalepi = Math.PI / generator.Maximum;
        sq = Math.Sqrt(2.0 * m);
        alm = Math.Log(m);
        g = m * alm - GammaRelated.LnGamma(m + 1.0);
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PoissonDistribution"/> class with mean 1,
    /// using a <see cref="StandardGenerator"/> as the underlying random number generator.
    /// </summary>
    public PoissonDistribution()
      : this(DefaultGenerator)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PoissonDistribution"/> class with mean 1,
    /// using the specified <see cref="Generator"/> as the underlying random number generator.
    /// </summary>
    /// <param name="ran">A <see cref="Generator"/> object.</param>
    public PoissonDistribution(Generator ran)
      : this(1, ran)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PoissonDistribution"/> class with the specified mean,
    /// using a <see cref="StandardGenerator"/> as the underlying random number generator.
    /// </summary>
    /// <param name="mean">The mean (λ) of the Poisson distribution.</param>
    public PoissonDistribution(double mean)
      : this(mean, DefaultGenerator)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PoissonDistribution"/> class with the specified mean,
    /// using the specified <see cref="Generator"/> as the underlying random number generator.
    /// </summary>
    /// <param name="mean">The mean (λ) of the Poisson distribution.</param>
    /// <param name="ran">A <see cref="Generator"/> object.</param>
    public PoissonDistribution(double mean, Generator ran)
      : base(ran)
    {
      Initialize(mean);
    }

    /// <inheritdoc />
    public override double NextDouble()
    {
      double em, t, y;

      if (m < 12.0)
      {         // direct method
        em = -1.0;
        t = 1.0;
        do
        {
          em += 1.0;
          t *= generator.Next() * scale;
        } while (t > g);
      }
      else
      {                // rejection method
        do
        {
          do
          {
            y = Math.Tan(generator.Next() * scalepi);
            em = sq * y + m;
          } while (em < 0.0);
          em = Math.Floor(em);
          t = 0.9 * (1.0 + y * y) * Math.Exp(em * alm - GammaRelated.LnGamma(em + 1.0) - g);
        } while (generator.Next() * scale > t);
      }

      return em;
    }

    #region overridden Distribution members

    /// <summary>
    /// Determines whether the specified mean value is valid.
    /// </summary>
    /// <param name="mu">The mean value to validate.</param>
    /// <returns><see langword="true"/> if <paramref name="mu"/> is within the valid range; otherwise, <see langword="false"/>.</returns>
    public bool IsValidMu(double mu)
    {
      return mu >= double.MinValue && mu <= double.MaxValue;
    }

    /// <inheritdoc />
    public override double Minimum
    {
      get
      {
        return 0.0;
      }
    }

    /// <inheritdoc />
    public override double Maximum
    {
      get
      {
        return double.MaxValue;
      }
    }

    /// <inheritdoc />
    public override double Mean
    {
      get
      {
        return m;
      }
    }

    /// <inheritdoc />
    public override double Median
    {
      get
      {
        return double.NaN;
      }
    }

    /// <inheritdoc />
    public override double Variance
    {
      get
      {
        return m;
      }
    }

    /// <inheritdoc />
    public override double[] Mode
    {
      get
      {
        // Check if the value of lambda is a whole number.
        if (m == Math.Floor(m))
        {
          return new double[] { m - 1.0, m };
        }
        else
        {
          return new double[] { Math.Floor(m) };
        }
      }
    }

    #endregion overridden Distribution members

    #region CdfPdf

    /// <inheritdoc />
    public override double CDF(double x)
    {
      return CDF(x, m);
    }

    /// <summary>
    /// Returns the cumulative distribution function (CDF) of a Poisson distribution.
    /// </summary>
    /// <param name="x">The value at which to evaluate the CDF.</param>
    /// <param name="m">The mean (λ) of the Poisson distribution.</param>
    /// <returns>The probability that a Poisson-distributed random variable is less than or equal to <paramref name="x"/>.</returns>
    public static double CDF(double x, double m)
    {
      return Calc.GammaRelated.GammaRegularized(1 + Math.Floor(x), m);
    }

    /// <inheritdoc />
    public override double PDF(double x)
    {
      return PDF(x, m);
    }

    /// <summary>
    /// Returns the probability mass function (PMF) of a Poisson distribution.
    /// </summary>
    /// <param name="x">The value at which to evaluate the PMF.</param>
    /// <param name="m">The mean (λ) of the Poisson distribution.</param>
    /// <returns>The probability for <paramref name="x"/>.</returns>
    public static double PDF(double x, double m)
    {
      return Math.Exp(-m + x * Math.Log(m) - Calc.GammaRelated.LnGamma(x + 1));
    }

    #endregion CdfPdf

    /// <inheritdoc />
    public override double Quantile(double x)
    {
      throw new NotSupportedException("Sorry, Quantile is not supported here since it is a discrete distribution");
    }
  }

  #endregion PoissonDistribution (Discrete)
}
