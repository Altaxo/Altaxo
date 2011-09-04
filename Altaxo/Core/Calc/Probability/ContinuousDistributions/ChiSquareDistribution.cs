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
#endregion

#region Further copyright(s)
// This file is partly based on Matpack 1.7.3 sources (Author B.Gammel) 
// The following Matpack files were used here:
// matpack-1.7.3\source\random\ranchisqr.cc

// This file is partly based on Troschuetz.Random Class Library (Author S.Troschuetz)
#endregion

using System;

namespace Altaxo.Calc.Probability
{

  /// <summary>
  /// Generates central chi-square distributed random numbers.
  /// </summary>
  /// <remarks><code>
  /// Generates random deviates from a central chi-square distribution with 
  /// f degrees of freedom. f must be positive. 
  /// The density of this distribution is:
  ///
  ///
  ///                -f/2   f/2-1  -x/2
  ///               2      x      e
  ///  p (x) dx =  --------------------- dx  for x > 0
  ///   f               Gamma(f/2)
  ///
  ///           =  0                         otherwise
  ///
  /// The calculation uses the relation between chi-square and gamma distribution:
  ///
  ///  ChiSquare(f) = GammaDistribution(f/2,1/2)
  ///
  /// References:
  ///    K. Behnen, G. Neuhaus, "Grundkurs Stochastik", Teubner Studienbuecher
  ///    Mathematik, Teubner Verlag, Stuttgart, 1984.
  ///
  /// </code></remarks>
  public class ChiSquareDistribution : ContinuousDistribution
  {
    protected double F;
    protected GammaDistribution gamma;
    public ChiSquareDistribution() : this(DefaultGenerator) { }
    public ChiSquareDistribution(Generator gen) : this(1, gen) { }
    public ChiSquareDistribution(double f) : this(f, DefaultGenerator) { }
    public ChiSquareDistribution (double f, Generator ran) 
      : base(ran)
    {
      Initialize(f);
    }
    public void Initialize(double F)
    {
      if (!IsValidAlpha(F))
        throw new ArgumentOutOfRangeException("F is out of range (has to be positive)");

      this.F = F;
      if (gamma == null)
        gamma = new GammaDistribution(0.5*F,1);
      else
        gamma.Initialize(0.5 * F, 1);
    }
    /// <summary>
    /// Determines whether the specified value is valid for parameter <see cref="Alpha"/>.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>
    /// <see langword="true"/> if value is greater than 0; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsValidAlpha(double value)
    {
      return value > 0;
    }
    public override double NextDouble() 
    {
      return 2.0 * gamma.NextDouble();
    }
    public double Freedom  { get { return F; }}

    /// <summary>
    /// Gets or sets the parameter alpha which is used for generation of chi-square distributed random numbers.
    /// </summary>
    /// <remarks>Call <see cref="IsValidAlpha"/> to determine whether a value is valid and therefor assignable.</remarks>
    public double Alpha
    {
      get
      {
        return this.F;
      }
      set
      {
        Initialize(value);
      }
    }

    #region overridden Distribution members
    /// <summary>
    /// Gets the minimum possible value of chi-square distributed random numbers.
    /// </summary>
    public override double Minimum
    {
      get
      {
        return 0.0;
      }
    }

    /// <summary>
    /// Gets the maximum possible value of chi-square distributed random numbers.
    /// </summary>
    public override double Maximum
    {
      get
      {
        return double.MaxValue;
      }
    }

    /// <summary>
    /// Gets the mean value of chi-square distributed random numbers.
    /// </summary>
    public override double Mean
    {
      get
      {
        return this.F;
      }
    }

    /// <summary>
    /// Gets the median of chi-square distributed random numbers.
    /// </summary>
    public override double Median
    {
      get
      {
        return this.F - 2.0 / 3.0;
      }
    }

    /// <summary>
    /// Gets the variance of chi-square distributed random numbers.
    /// </summary>
    public override double Variance
    {
      get
      {
        return 2.0 * this.F;
      }
    }

    /// <summary>
    /// Gets the mode of chi-square distributed random numbers.
    /// </summary>
    public override double[] Mode
    {
      get
      {
        if (this.F >= 2)
        {
          return new double[] { this.F - 2.0 };
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
      return CDF(x, F);
    }
    public static double CDF(double x, double F)
    {
      return Calc.GammaRelated.GammaRegularized(0.5 * F, 0, 0.5 * x);
    }

    public override double PDF(double x)
    {
      return PDF(x, F);
    }
    public static double PDF(double x, double F)
    {
      return Math.Pow(x, -1 + 0.5 * F) / (Math.Pow(2, 0.5 * F) * Math.Exp(0.5 * x) * Calc.GammaRelated.Gamma(0.5 * F));
    }


    public override double Quantile(double p)
    {
      return Quantile(p, F);
    }
    public static double Quantile(double p, double F)
    {
      return 2 * GammaRelated.InverseGammaRegularized(0.5 * F, 1 - p);
    }

    #endregion
  }
}
#if false
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
 * ChiSquareDistribution.cs, 17.08.2006
 * 
 */

using System;

namespace Altaxo.Calc.Probability
{
	/// <summary>
    /// Provides generation of chi-square distributed random numbers.
	/// </summary>
	/// <remarks>
	/// The implementation of the <see cref="ChiSquareDistribution"/> type bases upon information presented on
    ///   <a href="http://en.wikipedia.org/wiki/Chi-square_distribution">Wikipedia - Chi-square distribution</a>.
    /// </remarks>
	public class ChiSquareDistribution : ContinuousDistribution
	{
		#region instance fields
		/// <summary>
        /// Gets or sets the parameter alpha which is used for generation of chi-square distributed random numbers.
		/// </summary>
		/// <remarks>Call <see cref="IsValidAlpha"/> to determine whether a value is valid and therefor assignable.</remarks>
		public int Alpha
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
                }
        	}
		}

		/// <summary>
        /// Stores the parameter alpha which is used for generation of chi-square distributed random numbers.
		/// </summary>
        private int alpha;

        /// <summary>
        /// Stores a <see cref="NormalDistribution"/> object used for generation of chi-square distributed random numbers.
        /// </summary>
        private NormalDistribution normalDistribution;
        #endregion

		#region construction
		/// <summary>
        /// Initializes a new instance of the <see cref="ChiSquareDistribution"/> class, using a 
        ///   <see cref="StandardGenerator"/> as underlying random number generator.
		/// </summary>
        public ChiSquareDistribution()
            : this(new StandardGenerator())
		{
		}
		
		/// <summary>
        /// Initializes a new instance of the <see cref="ChiSquareDistribution"/> class, using the specified 
        ///   <see cref="Generator"/> as underlying random number generator.
        /// </summary>
        /// <param name="generator">A <see cref="Generator"/> object.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="generator"/> is NULL (<see langword="Nothing"/> in Visual Basic).
        /// </exception>
        public ChiSquareDistribution(Generator generator)
            : base(generator)
        {
            this.alpha = 1;
            this.normalDistribution = new NormalDistribution(generator);
            this.normalDistribution.Mu = 0.0;
            this.normalDistribution.Sigma = 1.0;
        }
		#endregion
	
		#region instance methods
		/// <summary>
        /// Determines whether the specified value is valid for parameter <see cref="Alpha"/>.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <returns>
		/// <see langword="true"/> if value is greater than 0; otherwise, <see langword="false"/>.
		/// </returns>
        public bool IsValidAlpha(int value)
		{
			return value > 0;
		}
        #endregion

		#region overridden Distribution members
        /// <summary>
        /// Gets the minimum possible value of chi-square distributed random numbers.
		/// </summary>
        public override double Minimum
		{
			get
			{
				return 0.0;
			}
		}

		/// <summary>
        /// Gets the maximum possible value of chi-square distributed random numbers.
		/// </summary>
        public override double Maximum
		{
			get
			{
				return double.MaxValue;
			}
		}

		/// <summary>
        /// Gets the mean value of chi-square distributed random numbers.
		/// </summary>
        public override double Mean
		{
			get
			{
                return this.alpha;
			}
		}
		
		/// <summary>
        /// Gets the median of chi-square distributed random numbers.
		/// </summary>
        public override double Median
		{
			get
			{
				return this.alpha - 2.0 / 3.0;
			}
		}
		
		/// <summary>
        /// Gets the variance of chi-square distributed random numbers.
		/// </summary>
        public override double Variance
		{
			get
			{
                return 2.0 * this.alpha;
			}
		}
		
		/// <summary>
        /// Gets the mode of chi-square distributed random numbers.
		/// </summary>
        public override double[] Mode
		{
            get
            {
                if (this.alpha >= 2)
                {
                    return new double[] { this.alpha - 2.0 };
                }
                else
                {
                    return new double[] { };
                }
            }
		}
		
		/// <summary>
        /// Returns a chi-square distributed floating point random number.
		/// </summary>
        /// <returns>A chi-square distributed double-precision floating point number.</returns>
        public override double NextDouble()
		{
            double sum = 0.0;
            for (int i = 0; i < this.alpha; i++)
            {
                sum += Math.Pow(this.normalDistribution.NextDouble(), 2);
            }

            return sum;
		}
        #endregion

    #region CdfPdfQuantile
    public override double CDF(double x)
    {
      return CDF(x, alpha);
    }
    public static double CDF(double x, double F)
    {
      return Calc.GammaRelated.GammaRegularized(0.5 * F, 0, 0.5 * x);
    }

    public override double PDF(double x)
    {
      return PDF(x, alpha);
    }
    public static double PDF(double x, double F)
    {
      return Math.Pow(x, -1 + 0.5 * F) / (Math.Pow(2, 0.5 * F) * Math.Exp(0.5 * x) * Calc.GammaRelated.Gamma(0.5 * F));
    }


    public override double Quantile(double p)
    {
      return Quantile(p, alpha);
    }
    public static double Quantile(double p, double F)
    {
      return 2 * GammaRelated.InverseGammaRegularized(0.5 * F, 1 - p);
    }

    #endregion

  }
}

#endif