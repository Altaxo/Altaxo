#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

#region Further copyright(s)
// This file is partly based on Matpack 1.7.3 sources (Author B.Gammel) 
// The following Matpack files were used here:
// matpack-1.7.3\source\random\ranpoisson.cc

// This file is partly based on Troschuetz.Random Class Library (Author S.Troschuetz)
#endregion

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
  /// Numerical Recipies in C, Cambridge Univ. Press, 1988.        
  /// </code></remarks>
  public class PoissonDistribution : DiscreteDistribution
  {
    protected double scale, scalepi, m, sq, alm, g;
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

    public PoissonDistribution()
      : this(DefaultGenerator)
    {
    }

    public PoissonDistribution(Generator ran)
      : this(1, ran)
    {
    }

    public PoissonDistribution(double mean) 
      : this(mean, DefaultGenerator)
    {
    }
    public PoissonDistribution(double mean, Generator ran)
      : base(ran)
    {
      Initialize(mean);
    }
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

    public bool IsValidMu(double mu)
    {
      return mu >= double.MinValue && mu <= double.MaxValue;
    }


    /// <summary>
    /// Gets the minimum possible value of poisson distributed random numbers.
    /// </summary>
    public override double Minimum
    {
      get
      {
        return 0.0;
      }
    }

    /// <summary>
    /// Gets the maximum possible value of poisson distributed random numbers.
    /// </summary>
    public override double Maximum
    {
      get
      {
        return double.MaxValue;
      }
    }

    /// <summary>
    /// Gets the mean value of poisson distributed random numbers. 
    /// </summary>
    public override double Mean
    {
      get
      {
        return this.m;
      }
    }

    /// <summary>
    /// Gets the median of poisson distributed random numbers.
    /// </summary>
    public override double Median
    {
      get
      {
        return double.NaN;
      }
    }

    /// <summary>
    /// Gets the variance of poisson distributed random numbers.
    /// </summary>
    public override double Variance
    {
      get
      {
        return this.m;
      }
    }

    /// <summary>
    /// Gets the mode of poisson distributed random numbers. 
    /// </summary>
    public override double[] Mode
    {
      get
      {
        // Check if the value of lambda is a whole number.
        if (this.m == Math.Floor(this.m))
        {
          return new double[] { this.m - 1.0, this.m };
        }
        else
        {
          return new double[] { Math.Floor(this.m) };
        }
      }
    }
    #endregion

    #region CdfPdf
    public override double CDF(double x)
    {
      return CDF(x, m);
    }
    public static double CDF(double x, double m)
    {
      return Calc.GammaRelated.GammaRegularized(1 + Math.Floor(x), m);
    }

    public override double PDF(double x)
    {
      return PDF(x, m);
    }
    public static double PDF(double x, double m)
    {
      return Math.Exp(-m + x * Math.Log(m) - Calc.GammaRelated.LnGamma(x + 1));
    }


    #endregion

    public override double Quantile(double x)
    {
      throw new NotSupportedException("Sorry, Quantile is not supported here since it is a discrete distribution");
    }

  }
  #endregion

}
