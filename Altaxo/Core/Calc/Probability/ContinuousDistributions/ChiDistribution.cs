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
 * ChiDistribution.cs, 17.08.2006
 * 
 */

using System;

namespace Altaxo.Calc.Probability
{
	/// <summary>
    /// Provides generation of chi distributed random numbers.
	/// </summary>
	/// <remarks>
	/// The implementation of the <see cref="ChiDistribution"/> type bases upon information presented on
    ///   <a href="http://en.wikipedia.org/wiki/Chi_distribution">Wikipedia - Chi distribution</a>.
    /// </remarks>
  public class ChiDistribution : ContinuousDistribution
    {
        #region class fields
        /// <summary>
        /// Represents coefficients for the Lanczos approximation of the Gamma function.
        /// </summary>
        private static readonly double[] LanczosCoefficients = new double[] { 1.000000000190015, 76.18009172947146, 
            -86.50532032941677, 24.01409824083091, -1.231739572450155, 1.208650973866179e-3, -5.395239384953e-6 };
        #endregion

        #region instance fields
        /// <summary>
        /// Gets or sets the parameter alpha which is used for generation of chi distributed random numbers.
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
        /// Stores the parameter alpha which is used for generation of chi distributed random numbers.
		/// </summary>
        private int alpha;

        /// <summary>
        /// Stores a <see cref="NormalDistribution"/> object used for generation of chi distributed random numbers.
        /// </summary>
        private NormalDistribution normalDistribution;
        #endregion

		#region construction
		/// <summary>
        /// Initializes a new instance of the <see cref="ChiDistribution"/> class, using a 
        ///   <see cref="StandardGenerator"/> as underlying random number generator.
		/// </summary>
        public ChiDistribution()
            : this(new StandardGenerator())
		{
		}
		
		/// <summary>
        /// Initializes a new instance of the <see cref="ChiDistribution"/> class, using the specified 
        ///   <see cref="Generator"/> as underlying random number generator.
        /// </summary>
        /// <param name="generator">A <see cref="Generator"/> object.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="generator"/> is NULL (<see langword="Nothing"/> in Visual Basic).
        /// </exception>
        public ChiDistribution(Generator generator)
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

        /// <summary>
        /// Represents a Lanczos approximation of the Gamma function.
        /// </summary>
        /// <param name="x">A double-precision floating point number.</param>
        /// <returns>
        /// A double-precision floating point number representing an approximation of Gamma(<paramref name="x"/>).
        /// </returns>
        private double Gamma(double x)
        {
            double sum = ChiDistribution.LanczosCoefficients[0];
            for (int index = 1; index <= 6; index++)
            {
                sum += ChiDistribution.LanczosCoefficients[index] / (x + index);
            }

            return Math.Sqrt(2.0 * Math.PI) / x * Math.Pow(x + 5.5, x + 0.5) / Math.Exp(x + 5.5) * sum;
        }
        #endregion

		#region overridden Distribution members
        /// <summary>
        /// Gets the minimum possible value of chi distributed random numbers.
		/// </summary>
        public override double Minimum
		{
			get
			{
				return 0.0;
			}
		}

		/// <summary>
        /// Gets the maximum possible value of chi distributed random numbers.
		/// </summary>
        public override double Maximum
		{
			get
			{
				return double.MaxValue;
			}
		}

        /// <summary>
        /// Gets the mean value of chi distributed random numbers.
		/// </summary>
        public override double Mean
		{
			get
			{
                return Math.Sqrt(2.0) * this.Gamma((this.alpha + 1.0) / 2.0) / this.Gamma(this.alpha / 2.0);
			}
		}
		
		/// <summary>
        /// Gets the median of chi distributed random numbers.
		/// </summary>
        public override double Median
		{
			get
			{
				return double.NaN;
			}
		}
		
		/// <summary>
        /// Gets the variance of chi distributed random numbers.
		/// </summary>
        public override double Variance
		{
			get
			{
                return this.alpha - Math.Pow(this.Mean, 2.0);
			}
		}
		
		/// <summary>
        /// Gets the mode of chi distributed random numbers.
		/// </summary>
        public override double[] Mode
		{
            get
            {
                if (this.alpha >= 1)
                {
                    return new double[] { Math.Sqrt(this.alpha - 1.0) };
                }
                else
                {
                    return new double[] { };
                }
            }
		}
		
		/// <summary>
        /// Returns a chi distributed floating point random number.
		/// </summary>
        /// <returns>A chi distributed double-precision floating point number.</returns>
        public override double NextDouble()
		{
            double sum = 0.0;
            for (int i = 0; i < this.alpha; i++)
            {
                sum += Math.Pow(this.normalDistribution.NextDouble(), 2);
            }

            return Math.Sqrt(sum);
		}
        #endregion
    }
}