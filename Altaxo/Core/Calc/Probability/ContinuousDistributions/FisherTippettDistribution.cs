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
 * FisherTippettDistribution.cs, 24.09.2006
 * 
 * 17.08.2006: Initial version
 * 24.09.2006: Adapted to change in base class (field "generator" declared private (formerly protected) 
 *             and made accessible through new protected property "Generator")
 * 
 */

using System;

namespace Altaxo.Calc.Probability
{
	/// <summary>
	/// Provides generation of Fisher-Tippett distributed random numbers (also known as ExtremeValueDistribution).
	/// </summary>
	/// <remarks>
    /// The implementation of the <see cref="FisherTippettDistribution"/> type bases upon information presented on
    ///   <a href="http://en.wikipedia.org/wiki/Laplace_distribution">Wikipedia - Fisher-Tippett distribution</a>.
    /// </remarks>
  public class FisherTippettDistribution : ContinuousDistribution
	{
		#region instance fields
		/// <summary>
		/// Gets or sets the parameter alpha which is used for generation of Fisher-Tippett distributed random numbers.
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
        Initialize(mu, value);
      }
		}

		/// <summary>
		/// Stores the parameter alpha which is used for generation of Fisher-Tippett distributed random numbers.
		/// </summary>
		private double alpha;
		
		/// <summary>
		/// Gets or sets the parameter mu which is used for generation of Fisher-Tippett distributed random numbers.
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
        Initialize(value, alpha);
        	}
		}

		/// <summary>
		/// Stores the parameter mu which is used for generation of Fisher-Tippett distributed random numbers.
		/// </summary>
		private double mu;
        #endregion

		#region construction, destruction
		/// <summary>
        /// Initializes a new instance of the <see cref="FisherTippettDistribution"/> class, using a 
        ///   <see cref="StandardGenerator"/> as underlying random number generator.
		/// </summary>
        public FisherTippettDistribution()
            : this(DefaultGenerator)
		{
		}

		/// <summary>
        /// Initializes a new instance of the <see cref="FisherTippettDistribution"/> class, using the specified 
        ///   <see cref="Generator"/> as underlying random number generator.
        /// </summary>
        /// <param name="generator">A <see cref="Generator"/> object.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="generator"/> is NULL (<see langword="Nothing"/> in Visual Basic).
        /// </exception>
        public FisherTippettDistribution(Generator generator)
            : this(0,1,generator)
        {
        }

    public FisherTippettDistribution(double mu, double alpha)
      : this(mu, alpha, DefaultGenerator)
    {
    }
      

    public FisherTippettDistribution(double mu, double alpha, Generator generator)
      : base(generator)
    {
      Initialize(mu, alpha);
    }


		#endregion
	
		#region instance methods

    public void Initialize(double mu, double alpha)
    {
      if (!IsValidMu(mu))
        throw new ArgumentOutOfRangeException("Mu is out of range");
      if (!IsValidAlpha(alpha))
        throw new ArgumentOutOfRangeException("Alpha is out of range");

      this.mu = mu;
      this.alpha = alpha;
      
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
			return value > 0;
		}
		
		/// <summary>
        /// Determines whether the specified value is valid for parameter <see cref="Mu"/>.
		/// </summary>
		/// <param name="value">The value to check.</param>
        /// <returns><see langword="true"/>.</returns>
		public bool IsValidMu(double value)
		{
			return true;
		}
        #endregion

		#region overridden Distribution members
        /// <summary>
		/// Gets the minimum possible value of Fisher-Tippett distributed random numbers.
		/// </summary>
		public override double Minimum
		{
			get
			{
				return double.MinValue;
			}
		}

		/// <summary>
		/// Gets the maximum possible value of Fisher-Tippett distributed random numbers.
		/// </summary>
        public override double Maximum
		{
			get
			{
				return double.MaxValue;
			}
		}

		/// <summary>
		/// Gets the mean value of Fisher-Tippett distributed random numbers.
		/// </summary>
        public override double Mean
		{
			get
			{
                // 0.577.. is an approximate value for the Euler-Mascheroni constant
                return this.mu + this.alpha * 0.577215664901532860606512090082402431042159335;
			}
		}
		
		/// <summary>
		/// Gets the median of Fisher-Tippett distributed random numbers.
		/// </summary>
        public override double Median
		{
			get
			{
                return this.mu - this.alpha * Math.Log(Math.Log(2));
			}
		}
		
		/// <summary>
		/// Gets the variance of Fisher-Tippett distributed random numbers.
		/// </summary>
        public override double Variance
		{
			get
			{
                return Math.Pow(Math.PI, 2.0) / 6.0 * Math.Pow(this.alpha, 2.0);
			}
		}
		
		/// <summary>
		/// Gets the mode of Fisher-Tippett distributed random numbers.
		/// </summary>
        public override double[] Mode
		{
            get
            {
                return new double[] { this.mu };
            }
		}
		
		/// <summary>
		/// Returns a Fisher-Tippett distributed floating point random number.
		/// </summary>
        /// <returns>A Fisher-Tippett distributed double-precision floating point number.</returns>
        public override double NextDouble()
		{
            return this.mu - this.alpha * Math.Log(-Math.Log(1.0 - this.Generator.NextDouble()));
		}
		#endregion

    #region CdfPdfQuantile

    public override double CDF(double x)
    {
      return CDF(x, mu, alpha);
    }

    public static double CDF(double x, double alpha, double beta)
    {
      return Math.Exp(-Math.Exp(-(-alpha + x) / beta));
    }

    public override double PDF(double x)
    {
      return PDF(x, mu, alpha);
    }
    public static double PDF(double x, double alpha, double beta)
    {
      return Math.Exp(-Math.Exp((alpha - x) / beta) + (alpha - x) / beta) / beta;
    }


    public override double Quantile(double p)
    {
      return Quantile(p, mu, alpha);
    }
    public static double Quantile(double p, double alpha, double beta)
    {
      if (p < 0 || p > 1)
        throw new ArgumentException("Probability p must be between 0 and 1");
      return alpha - beta * Math.Log(Math.Log(1 / p));
    }


    #endregion

	}
}