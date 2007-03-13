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
 * GammaDistribution.cs, 21.09.2006
 * 
 * 09.08.2006: Initial version
 * 21.09.2006: Adapted to change in base class (field "generator" declared private (formerly protected) 
 *               and made accessible through new protected property "Generator")
 * 
 */

using System;

namespace Altaxo.Calc.Probability
{
	/// <summary>
	/// Provides generation of gamma distributed random numbers.
	/// </summary>
	/// <remarks>
    /// The implementation of the <see cref="GammaDistribution"/> type bases upon information presented on
    ///   <a href="http://en.wikipedia.org/wiki/Gamma_distribution">Wikipedia - Gamma distribution</a>.
    /// </remarks>
  public class GammaDistribution : ContinuousDistribution
	{
		#region instance fields
		/// <summary>
		/// Gets or sets the parameter alpha which is used for generation of gamma distributed random numbers.
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
		/// Stores the parameter alpha which is used for generation of gamma distributed random numbers.
		/// </summary>
		private double alpha;
		
		/// <summary>
		/// Gets or sets the parameter theta which is used for generation of gamma distributed random numbers.
		/// </summary>
		/// <remarks>Call <see cref="IsValidTheta"/> to determine whether a value is valid and therefor assignable.</remarks>
		public double Theta
		{
			get
			{
				return this.theta;
			}
			set
			{
                if (this.IsValidTheta(value))
                {
                    this.theta = value;
                }
        	}
		}

		/// <summary>
		/// Stores the parameter theta which is used for generation of gamma distributed random numbers.
		/// </summary>
		private double theta;

        /// <summary>
        /// Stores an intermediate result for generation of gamma distributed random numbers.
        /// </summary>
        /// <remarks>
        /// Speeds up random number generation cause this value only depends on distribution parameters 
        ///   and therefor doesn't need to be recalculated in successive executions of <see cref="NextDouble"/>.
        /// </remarks>
        private double helper1;

        /// <summary>
        /// Stores an intermediate result for generation of gamma distributed random numbers.
        /// </summary>
        /// <remarks>
        /// Speeds up random number generation cause this value only depends on distribution parameters 
        ///   and therefor doesn't need to be recalculated in successive executions of <see cref="NextDouble"/>.
        /// </remarks>
        private double helper2;
        #endregion

		#region construction, destruction
		/// <summary>
        /// Initializes a new instance of the <see cref="GammaDistribution"/> class, using a 
        ///   <see cref="StandardGenerator"/> as underlying random number generator.
		/// </summary>
        public GammaDistribution()
            : this(new StandardGenerator())
		{
		}

		/// <summary>
        /// Initializes a new instance of the <see cref="GammaDistribution"/> class, using the specified 
        ///   <see cref="Generator"/> as underlying random number generator.
        /// </summary>
        /// <param name="generator">A <see cref="Generator"/> object.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="generator"/> is NULL (<see langword="Nothing"/> in Visual Basic).
        /// </exception>
        public GammaDistribution(Generator generator)
            : base(generator)
        {
            this.alpha = 1.0;
            this.theta = 1.0;
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
			return value > 0;
		}
		
		/// <summary>
        /// Determines whether the specified value is valid for parameter <see cref="Theta"/>.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <returns>
		/// <see langword="true"/> if value is greater than 0.0; otherwise, <see langword="false"/>.
		/// </returns>
		public bool IsValidTheta(double value)
		{
			return value > 0;
		}

        /// <summary>
        /// Updates the helper variables that store intermediate results for generation of gamma distributed random 
        ///   numbers.
        /// </summary>
        private void UpdateHelpers()
        {
            this.helper1 = this.alpha - Math.Floor(this.alpha);
            this.helper2 = Math.E / (Math.E + this.helper1);
        }
        #endregion

		#region overridden Distribution members
        /// <summary>
		/// Gets the minimum possible value of gamma distributed random numbers.
		/// </summary>
		public override double Minimum
		{
			get
			{
				return 0.0;
			}
		}

		/// <summary>
		/// Gets the maximum possible value of gamma distributed random numbers.
		/// </summary>
        public override double Maximum
		{
			get
			{
				return double.MaxValue;
			}
		}

		/// <summary>
		/// Gets the mean value of gamma distributed random numbers.
		/// </summary>
        public override double Mean
		{
			get
			{
				return this.alpha * this.theta;
			}
		}
		
		/// <summary>
		/// Gets the median of gamma distributed random numbers.
		/// </summary>
        public override double Median
		{
			get
			{
				return double.NaN;
			}
		}
		
		/// <summary>
		/// Gets the variance of gamma distributed random numbers.
		/// </summary>
        public override double Variance
		{
			get
			{
				return this.alpha * Math.Pow(this.theta, 2.0);
			}
		}
		
		/// <summary>
		/// Gets the mode of gamma distributed random numbers.
		/// </summary>
        public override double[] Mode
		{
			get
			{
                if (this.alpha >= 1.0)
                {
                    return new double[] { (this.alpha - 1.0) * this.theta };
                }
                else
                {
                    return new double[] { };
                }
			}
		}
		
		/// <summary>
		/// Returns a gamma distributed floating point random number.
		/// </summary>
        /// <returns>A gamma distributed double-precision floating point number.</returns>
        public override double NextDouble()
		{
			double xi, eta, gen1, gen2;
			do
			{
				gen1 = 1.0 - this.Generator.NextDouble();
				gen2 = 1.0 - this.Generator.NextDouble();
                if (gen1 <= this.helper2)
				{
                    xi = Math.Pow(gen1 / this.helper2, 1.0 / this.helper1);
                    eta = gen2 * Math.Pow(xi, this.helper1 - 1.0);
				}
				else
				{
                    xi = 1.0 - Math.Log((gen1 - this.helper2) / (1.0 - this.helper2));
					eta = gen2 * Math.Pow(Math.E, -xi);
				}
            } while (eta > Math.Pow(xi, this.helper1 - 1.0) * Math.Pow(Math.E, -xi));

            for (int i = 1; i <= this.alpha; i++)
            {
                xi -= Math.Log(this.Generator.NextDouble());
            }

			return xi * this.theta;
		}
		#endregion

    #region CdfPdfQuantile
    public override double CDF(double x)
    {
      return CDF(x, alpha, theta);
    }
    public static double CDF(double x, double A, double B)
    {
      return GammaRelated.GammaRegularized(A, 0, B * x);
    }
    public override double PDF(double x)
    {
      return PDF(x, alpha, theta);
    }
    public static double PDF(double x, double A, double B)
    {
      return Math.Exp(-B * x) * Math.Pow(B * x, A - 1) * B / Calc.GammaRelated.Gamma(A);
    }

    public override double Quantile(double p)
    {
      return Quantile(p, alpha, theta);
    }
    public static double Quantile(double p, double A, double B)
    {
      return GammaRelated.InverseGammaRegularized(A, 1 - p) / B;
    }
#endregion
	}
}