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
 * BetaDistribution.cs, 16.08.2006
 * 
 * 09.08.2006: Initial version
 * 16.08.2006: Renamed fields storing GammaDistribution objects and declared them private explicitely
 * 
 */

using System;

namespace Altaxo.Calc.Probability
{
    /// <summary>
    /// Provides generation of beta distributed random numbers.
    /// </summary>
    /// <remarks>
    /// The implementation of the <see cref="BetaDistribution"/> type bases upon information presented on
    ///   <a href="http://en.wikipedia.org/wiki/Beta_distribution">Wikipedia - Beta distribution</a> and
    ///   <a href="http://www.xycoon.com/beta_randomnumbers.htm">Xycoon - Beta Distribution</a>.
    /// </remarks>
  public class BetaDistribution : ContinuousDistribution
  {
    #region instance fields
    /// <summary>
    /// Gets or sets the parameter alpha which is used for generation of beta distributed random numbers.
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
        if (this.IsValidAlpha(value))
        {
          this.alpha = value;
          this.UpdateHelpers();
        }
      }
    }

    /// <summary>
    /// Stores the parameter alpha which is used for generation of beta distributed random numbers.
    /// </summary>
    private double alpha;

    /// <summary>
    /// Gets or sets the parameter beta which is used for generation of beta distributed random numbers.
    /// </summary>
    /// <remarks>Call <see cref="IsValidBeta"/> to determine whether a value is valid and therefor assignable.</remarks>
    public double Beta
    {
      get
      {
        return this.beta;
      }
      set
      {
        if (this.IsValidBeta(value))
        {
          this.beta = value;
          this.UpdateHelpers();
        }
      }
    }

    /// <summary>
    /// Stores the parameter beta which is used for generation of beta distributed random numbers.
    /// </summary>
    private double beta;

    /// <summary>
    /// Stores a <see cref="GammaDistribution"/> object used for generation of beta distributed random numbers
    ///   and configured with parameter <see cref="alpha"/>.
    /// </summary>
    private GammaDistribution gammaDistributionAlpha;

    /// <summary>
    /// Stores a <see cref="GammaDistribution"/> object used for generation of beta distributed random numbers
    ///   and configured with parameter <see cref="beta"/>.
    /// </summary>
    private GammaDistribution gammaDistributionBeta;
    #endregion

    #region construction
    /// <summary>
    /// Initializes a new instance of the <see cref="BetaDistribution"/> class, using a 
    ///   <see cref="StandardGenerator"/> as underlying random number generator.
    /// </summary>
    public BetaDistribution()
      : this(new StandardGenerator())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BetaDistribution"/> class, using the specified 
    ///   <see cref="Generator"/> as underlying random number generator.
    /// </summary>
    /// <param name="generator">A <see cref="Generator"/> object.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="generator"/> is NULL (<see langword="Nothing"/> in Visual Basic).
    /// </exception>
    public BetaDistribution(Generator generator)
      : base(generator)
    {
      this.alpha = 1.0;
      this.beta = 1.0;
      this.gammaDistributionAlpha = new GammaDistribution(generator);
      this.gammaDistributionAlpha.Theta = 1.0;
      this.gammaDistributionBeta = new GammaDistribution(generator);
      this.gammaDistributionBeta.Theta = 1.0;
      this.UpdateHelpers();
    }
    #endregion

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
    /// Updates the helper variables that store intermediate results for generation of beta distributed random 
    ///   numbers.
    /// </summary>
    private void UpdateHelpers()
    {
      this.gammaDistributionAlpha.Alpha = this.alpha;
      this.gammaDistributionBeta.Alpha = this.beta;
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
        return this.alpha / (this.alpha + this.beta);
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
        return (this.alpha * this.beta) / (Math.Pow(this.alpha + this.beta, 2.0) * (this.alpha + this.beta + 1.0));
      }
    }

    /// <summary>
    /// Gets the mode of beta distributed random numbers.
    /// </summary>
    public override double[] Mode
    {
      get
      {
        if ((this.alpha > 1) && (this.beta > 1))
        {
          return new double[] { (this.alpha - 1.0) / (this.alpha + this.beta - 2.0) };
        }
        else if ((this.alpha < 1) && (this.beta < 1))
        {
          return new double[] { 0.0, 1.0 };
        }
        else if (((this.alpha < 1) && (this.beta >= 1)) || ((this.alpha == 1) && (this.beta > 1)))
        {
          return new double[] { 0.0 };
        }
        else if (((this.alpha >= 1) && (this.beta < 1)) || ((this.alpha > 1) && (this.beta == 1)))
        {
          return new double[] { 1.0 };
        }
        else
        {
          return new double[] { };
        }
      }
    }

    /// <summary>
    /// Returns a beta distributed floating point random number.
    /// </summary>
    /// <returns>A beta distributed double-precision floating point number.</returns>
    public override double NextDouble()
    {
      double x = this.gammaDistributionAlpha.NextDouble();

      return x / (x + this.gammaDistributionBeta.NextDouble());
    }
    #endregion

    #region CdfPdfQuantile

    public override double CDF(double x)
    {
      return CDF(x, alpha, beta);
    }
    public static double CDF(double x, double A, double B)
    {
      return Calc.GammaRelated.BetaRegularized(x, A, B);
    }



    public override double PDF(double x)
    {
      return PDF(x, alpha, beta);
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
      return Quantile(p, alpha, beta);
    }
    public static double Quantile(double p, double A, double B)
    {
      return Calc.GammaRelated.InverseBetaRegularized(p, A, B);
    }


    #endregion
  }
}