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
// matpack-1.7.3\source\random\ranbeta.cc

// This file is partly based on Troschuetz.Random Class Library (Author S.Troschuetz)
#endregion

using System;

namespace Altaxo.Calc.Probability
{

  /// <summary>
  /// Generates Beta distributed random numbers.
  /// </summary>
  /// /// <remarks>
  /// The implementation of the <see cref="BetaDistribution"/> type bases upon information presented on
  ///   <a href="http://en.wikipedia.org/wiki/Beta_distribution">Wikipedia - Beta distribution</a> and
  ///   <a href="http://www.xycoon.com/beta_randomnumbers.htm">Xycoon - Beta Distribution</a>.
  /// </remarks>
  /// <remarks><code>
  ///                            
  /// Return Beta distributed random deviates according to the density    
  ///                             
  ///                 a-1       b-1             
  ///                x     (1-x)                 
  ///  p   (x) dx = --------------- dx   for 0 &lt; x &lt; 1          
  ///   a,b              B(a,b)                
  ///                        
  ///             =  0                   otherwise             
  ///                             
  /// References:                     
  ///                            
  /// R. C. H. Cheng, Generating Beta Variatew with Non-integral Shape      
  /// Parameters, Comm. ACM, 21, 317-322 (1978). (Algorithms BB and BC).   
  ///
  /// </code>
  ///   ///   <a href="http://en.wikipedia.org/wiki/Beta_distribution">Wikipedia - Beta distribution</a> and
  ///   <a href="http://www.xycoon.com/beta_randomnumbers.htm">Xycoon - Beta Distribution</a>.
 ///</remarks>

  public class BetaDistribution : ContinuousDistribution
  {
    protected double _alpha, _beta;
    protected double scale, a, hlpalpha, b, hlpbeta, delta, gamma, k1, k2, maxexp;
    protected bool algorithmBB;
    public void Initialize(double alpha, double beta)
    {
      // check parameters
      if (alpha <= 0.0)
        throw new ArgumentOutOfRangeException("Alpha out of range (must be positive)");
      if(beta <= 0.0)
        throw new ArgumentOutOfRangeException("Beta out of range (must be positive)");

      // store parameters
      _alpha = alpha;
      _beta = beta;

      // scale random long to (0,1) - boundaries are not allowed !
      scale = 1.0 / (generator.Maximum + 1.0);

      // maximal exponent for exp() function in evaluation "a*exp(v)" below
      maxexp = DBL_MAX_EXP * M_LN2 - 1;

      if (a > 1.0) maxexp -= Math.Ceiling(Math.Log(a));

      algorithmBB = Math.Min(_alpha, _beta) > 1.0;

      // initialize algorithm BB
      if (algorithmBB)
      {
        a = Math.Min(_alpha, _beta);
        b = Math.Max(_alpha, _beta);
        hlpalpha = a + b;
        hlpbeta = Math.Sqrt((hlpalpha - 2.0) / (2.0 * a * b - hlpalpha));
        gamma = a + 1.0 / hlpbeta;

        // initialize algorithm BC
      }
      else
      {
        a = Math.Max(_alpha, _beta);
        b = Math.Min(_alpha, _beta);
        hlpalpha = a + b;
        hlpbeta = 1.0 / b;
        delta = 1.0 + a - b;
        k1 = delta * (1.38889e-2 + 4.16667e-2 * b) / (a * hlpbeta - 0.777778);
        k2 = 0.25 + (0.5 + 0.25 / delta) * b;
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BetaDistribution"/> class, using a 
    ///   <see cref="StandardGenerator"/> as underlying random number generator.
    /// </summary>
    public BetaDistribution() : this(DefaultGenerator) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="BetaDistribution"/> class, using the specified 
    ///   <see cref="Generator"/> as underlying random number generator.
    /// </summary>
    /// <param name="generator">A <see cref="Generator"/> object.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="generator"/> is NULL (<see langword="Nothing"/> in Visual Basic).
    /// </exception>
    public BetaDistribution(Generator generator) 
      : this(1,1,generator)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BetaDistribution"/> class, using the specified 
    /// parameters and the default  <see cref="Generator"/>.
    /// </summary>
    /// <param name="alpha">First parameter of the distribution.</param>
    /// <param name="beta">Second parameter of the distribution.</param>
    public BetaDistribution(double alpha, double beta) : this(alpha, beta, DefaultGenerator) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="BetaDistribution"/> class, using the specified 
    ///   <see cref="Generator"/> as underlying random number generator.
    /// </summary>
    /// <param name="alpha">First parameter of the distribution.</param>
    /// <param name="beta">Second parameter of the distribution.</param>
    /// <param name="generator">A <see cref="Generator"/> object.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="generator"/> is NULL (<see langword="Nothing"/> in Visual Basic).
    /// </exception>
    public BetaDistribution(double alpha, double beta, Generator generator)
      : base(generator)
    {
      Initialize(alpha, beta); 
    }

    /// <summary>
    /// Returns a beta distributed floating point random number.
    /// </summary>
    /// <returns>A beta distributed double-precision floating point number.</returns>
    public override double NextDouble()
    {
      // returned on overflow
      const double infinity = DBL_MAX;

      double w;

      // Algorithm BB
      if (algorithmBB)
      {

        double r, t;
        do
        {
          double u1 = generator.Next() * scale;
          double u2 = generator.Next() * scale;
          double v = hlpbeta * Math.Log(u1 / (1.0 - u1));
          w = (v > maxexp) ? infinity : a * Math.Exp(v);
          double z = u1 * u1 * u2;
          r = gamma * v - 1.3862944;
          double s = a + r - w;
          if (s + 2.609438 >= 5.0 * z) break;
          t = Math.Log(z);
          if (s > t) break;
        } while (r + hlpalpha * Math.Log(hlpalpha / (b + w)) < t);

        // Algorithm BC
      }
      else
      {

      loop:

        double v, y, z;
        double u1 = generator.Next() * scale;
        double u2 = generator.Next() * scale;

        if (u1 < 0.5)
        {
          y = u1 * u2;
          z = u1 * y;
          if (0.25 * u2 + z - y >= k1) goto loop;
        }
        else
        {
          z = u1 * u1 * u2;
          if (z <= 0.25)
          {
            v = hlpbeta * Math.Log(u1 / (1.0 - u1));
            w = (v > maxexp) ? infinity : a * Math.Exp(v);
            goto fin;
          }
          if (z >= k2) goto loop;
        }
        v = hlpbeta * Math.Log(u1 / (1.0 - u1));
        w = (v > maxexp) ? infinity : a * Math.Exp(v);
        if (hlpalpha * (Math.Log(hlpalpha / (b + w)) + v) - 1.3862944 < Math.Log(z)) goto loop;

      fin: ;
      }

      // return result
      return (a == _alpha) ? w / (b + w) : b / (b + w);
    }

    #region instance fields
    /// <summary>
    /// Gets or sets the parameter alpha which is used for generation of beta distributed random numbers.
    /// </summary>
    /// <remarks>Call <see cref="IsValidAlpha"/> to determine whether a value is valid and therefor assignable.</remarks>
    public double Alpha
    {
      get
      {
        return this._alpha;
      }
      set
      {
        Initialize(value, _beta);
      }
    }

    /// <summary>
    /// Gets or sets the parameter beta which is used for generation of beta distributed random numbers.
    /// </summary>
    /// <remarks>Call <see cref="IsValidBeta"/> to determine whether a value is valid and therefor assignable.</remarks>
    public double Beta
    {
      get
      {
        return this._beta;
      }
      set
      {
        Initialize(_alpha, value);
      }
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
    #endregion

    #region overridden IDistribution members
    /// <summary>
    /// Gets the minimum possible value of beta distributed random numbers.
    /// </summary>
    public override double Minimum
    {
      get
      {
        return 0.0;
      }
    }

    /// <summary>
    /// Gets the maximum possible value of beta distributed random numbers.
    /// </summary>
    public override double Maximum
    {
      get
      {
        return 1.0;
      }
    }

    /// <summary>
    /// Gets the mean value of beta distributed random numbers.
    /// </summary>
    public override double Mean
    {
      get
      {
        return this._alpha / (this._alpha + this._beta);
      }
    }

    /// <summary>
    /// Gets the median of beta distributed random numbers.
    /// </summary>
    public override double Median
    {
      get
      {
        return double.NaN;
      }
    }

    /// <summary>
    /// Gets the variance of beta distributed random numbers.
    /// </summary>
    public override double Variance
    {
      get
      {
        return (this._alpha * this._beta) / (Math.Pow(this._alpha + this._beta, 2.0) * (this._alpha + this.b + 1.0));
      }
    }

    /// <summary>
    /// Gets the mode of beta distributed random numbers.
    /// </summary>
    public override double[] Mode
    {
      get
      {
        if ((this._alpha > 1) && (this._beta > 1))
        {
          return new double[] { (this._alpha - 1.0) / (this._alpha + this._beta - 2.0) };
        }
        else if ((this._alpha < 1) && (this._beta < 1))
        {
          return new double[] { 0.0, 1.0 };
        }
        else if (((this._alpha < 1) && (this._beta >= 1)) || ((this._alpha == 1) && (this._beta > 1)))
        {
          return new double[] { 0.0 };
        }
        else if (((this._alpha >= 1) && (this._beta < 1)) || ((this._alpha > 1) && (this._beta == 1)))
        {
          return new double[] { 1.0 };
        }
        else
        {
          return new double[] { };
        }
      }
    }
    #endregion

    #region CdfPdfQuantile

    public override double CDF(double x)
    {
      return CDF(x, _alpha, _beta);
    }
    public static double CDF(double x, double A, double B)
    {
      return Calc.GammaRelated.BetaRegularized(x, A, B);
    }



    public override double PDF(double x)
    {
      return PDF(x, _alpha, _beta);
    }
    public static double PDF(double x, double A, double B)
    {
      if (x < 0 || x > 1)
      {
        return 0;
      }
      else
      {
        double p;

        double gab = Calc.GammaRelated.LnGamma(A + B);
        double ga = Calc.GammaRelated.LnGamma(A);
        double gb = Calc.GammaRelated.LnGamma(B);

        p = Math.Exp(gab - ga - gb) * Math.Pow(x, A - 1) * Math.Pow(1 - x, B - 1);

        return p;
      }
    }


    public override double Quantile(double p)
    {
      return Quantile(p, _alpha, _beta);
    }
    public static double Quantile(double p, double A, double B)
    {
      return Calc.GammaRelated.InverseBetaRegularized(p, A, B);
    }


    #endregion
  }
}
