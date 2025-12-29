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
// matpack-1.7.3\source\random\ranbinomial.cc

// This file is partly based on Troschuetz.Random Class Library (Author S.Troschuetz)

#endregion Further copyright(s)

using System;

namespace Altaxo.Calc.Probability
{
  /// <summary>
  /// Generates Binomial distributed random numbers.
  /// </summary>
  /// <remarks><code>
  /// Returns a binomial distributed deviate (integer returned in a double)
  /// according to the distribution:
  ///
  ///              j+eps                 / n \    j      n-j
  ///      integral       p   (m) dm  = |     |  q  (1-q)
  ///              j-eps   n,q           \ j /
  ///
  /// References:
  /// D. E. Knuth: The Art of Computer Programming, Vol. 2, Seminumerical
  /// Algorithms, pp. 120, 2nd edition, 1981.
  ///                             //
  /// W. H. Press, B. P. Flannery, S. A. Teukolsky, W. T. Vetterling,
  /// Numerical Recipies in C, Cambridge Univ. Press, 1988.
  /// </code></remarks>
  public class BinomialDistribution : DiscreteDistribution
  {
    protected double scale, scalepi, p, pc, plog, pclog, np, npexp, en, en1, gamen1, sq;
    protected int n;
    protected bool sym;

    /// <summary>
    /// Initializes this instance with the specified distribution parameters.
    /// </summary>
    /// <param name="pp">The success probability p in the range [0, 1].</param>
    /// <param name="nn">The number of trials n (must be greater than or equal to 0).</param>
    protected void Initialize(double pp, int nn)
    {
      if (nn < 0)
        throw new ArgumentOutOfRangeException("Num has to be >=0");
      if (pp < 0 || pp > 1)
        throw new ArgumentOutOfRangeException("Probability must be within [0,1]");

      scale = 1.0 / generator.Maximum;
      scalepi = Math.PI / generator.Maximum;

      if (pp <= 0.5)
      { // use invariance under  p  <==> 1-p
        p = pp;
        sym = false;
      }
      else
      {
        p = 1.0 - pp;
        sym = true;
      }

      n = nn;
      np = n * p;
      npexp = Math.Exp(-np);

      en = n;
      en1 = en + 1.0;
      gamen1 = GammaRelated.LnGamma(en1);
      pc = 1.0 - p;
      plog = Math.Log(p);
      pclog = Math.Log(pc);
      sq = Math.Sqrt(2 * np * pc);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BinomialDistribution"/> class, using a
    /// <see cref="StandardGenerator"/> as the underlying random number generator.
    /// </summary>
    public BinomialDistribution()
      : this(DefaultGenerator)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BinomialDistribution"/> class, using the specified
    /// <see cref="Generator"/> as the underlying random number generator.
    /// </summary>
    /// <param name="generator">A <see cref="Generator"/> object.</param>
    public BinomialDistribution(Generator generator)
      : this(1, 0.5, generator)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BinomialDistribution"/> class with the specified parameters,
    /// using a <see cref="StandardGenerator"/> as the underlying random number generator.
    /// </summary>
    /// <param name="num">The number of trials n.</param>
    /// <param name="prob">The success probability p in the range [0, 1].</param>
    public BinomialDistribution(int num, double prob)
      : this(num, prob, DefaultGenerator)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BinomialDistribution"/> class with the specified parameters,
    /// using the specified <see cref="Generator"/> as the underlying random number generator.
    /// </summary>
    /// <param name="num">The number of trials n.</param>
    /// <param name="prob">The success probability p in the range [0, 1].</param>
    /// <param name="ran">A <see cref="Generator"/> object.</param>
    public BinomialDistribution(int num, double prob, Generator ran)
      : base(ran)
    {
      Initialize(prob, num);
    }

    /// <inheritdoc />
    public override double NextDouble()
    {
      double bnl;

      if (n < 25)
      {   // direct method for moderate n
        bnl = 0.0;
        for (int j = 0; j < n; j++)
          if (scale * generator.Next() < p)
            bnl++;
      }
      else if (np < 1.0)
      { // use direct Poisson method
        int j;
        double t = 1.0;
        for (j = 0; j <= n; j++)
        {
          t *= scale * generator.Next();
          if (t < npexp)
            break;
        }
        bnl = (j <= n ? j : n);
      }
      else
      {     // use rejection method
        double em, y, t;
        do
        {
          do
          {
            y = Math.Tan(scalepi * generator.Next());
            em = sq * y + np;
          } while (em < 0.0 || em >= en1);
          em = Math.Floor(em);
          t = 1.2 * sq * (1.0 + y * y) * Math.Exp(gamen1
            - GammaRelated.LnGamma(em + 1.0)
            - GammaRelated.LnGamma(en1 - em)
            + em * plog
            + (en - em) * pclog);
        } while (scale * generator.Next() > t);
        bnl = em;
      }

      if (sym)
        bnl = n - bnl; // undo symmetry transformation

      return bnl;
    }

    /// <summary>
    /// Gets the parameter 'probability' of this distribution.
    /// </summary>
    public double Probability
    {
      get { return sym ? 1 - p : p; }
      set
      {
        Initialize(value, n);
      }
    }

    /// <summary>
    /// Gets the parameter 'maximum value' of this distribution.
    /// </summary>
    public int Number
    {
      get { return n; }
      set
      {
        Initialize(Probability, value);
      }
    }

    #region overridden Distribution members

    /// <summary>
    /// Gets the minimum possible value of binomial distributed random numbers.
    /// </summary>
    public override double Minimum
    {
      get
      {
        return 0.0;
      }
    }

    /// <summary>
    /// Gets the maximum possible value of binomial distributed random numbers.
    /// </summary>
    public override double Maximum
    {
      get
      {
        return n;
      }
    }

    /// <summary>
    /// Gets the mean value of binomial distributed random numbers.
    /// </summary>
    public override double Mean
    {
      get
      {
        return Probability * n;
      }
    }

    /// <summary>
    /// Gets the median of binomial distributed random numbers.
    /// </summary>
    public override double Median
    {
      get
      {
        return double.NaN;
      }
    }

    /// <summary>
    /// Gets the variance of binomial distributed random numbers.
    /// </summary>
    public override double Variance
    {
      get
      {
        return p * (1.0 - p) * n;
      }
    }

    /// <summary>
    /// Gets the mode of binomial distributed random numbers.
    /// </summary>
    public override double[] Mode
    {
      get
      {
        return new double[] { Math.Floor(Probability * (n + 1.0)) };
      }
    }

    #endregion overridden Distribution members

    #region CdfPdf

    /// <inheritdoc />
    public override double CDF(double x)
    {
      return CDF(x, Probability, n);
    }

    /// <summary>
    /// Returns the cumulative distribution function (CDF) of a binomial distribution.
    /// </summary>
    /// <param name="x">The value at which to evaluate the CDF.</param>
    /// <param name="p">The success probability p in the range [0, 1].</param>
    /// <param name="n">The number of trials.</param>
    /// <returns>The probability that a binomially distributed random variable is less than or equal to <paramref name="x"/>.</returns>
    public static double CDF(double x, double p, int n)
    {
      return Calc.GammaRelated.BetaRegularized(1 - p, n - Math.Floor(x), 1 + Math.Floor(x));
    }

    /// <inheritdoc />
    public override double PDF(double x)
    {
      return PDF(x, Probability, n);
    }

    /// <summary>
    /// Returns the probability mass function (PMF) of a binomial distribution.
    /// </summary>
    /// <param name="x">The value at which to evaluate the PMF.</param>
    /// <param name="p">The success probability p in the range [0, 1].</param>
    /// <param name="n">The number of trials.</param>
    /// <returns>The probability for <paramref name="x"/>.</returns>
    public static double PDF(double x, double p, int n)
    {
      return Math.Pow(1 - p, n - x) * Math.Pow(p, x) * Calc.GammaRelated.Binomial(n, x);
    }

    #endregion CdfPdf

    /// <inheritdoc />
    public override double Quantile(double x)
    {
      throw new NotSupportedException("Sorry, Quantile is not supported here since it is a discrete distribution");
    }
  }
}
