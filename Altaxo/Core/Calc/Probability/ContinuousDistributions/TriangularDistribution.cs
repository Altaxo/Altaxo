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
 * TriangularDistribution.cs, 21.09.2006
 * 
 * 09.08.2006: Initial version
 * 21.09.2006: Adapted to change in base class (field "generator" declared private (formerly protected) 
 *               and made accessible through new protected property "Generator")
 * 
 */

#region original copyright
/* boost random/triangle_distribution.hpp header file
 *
 * Copyright Jens Maurer 2000-2001
 * Distributed under the Boost Software License, Version 1.0. (See
 * accompanying file LICENSE_1_0.txt or copy at
 * http://www.boost.org/LICENSE_1_0.txt)
 *
 * See http://www.boost.org for most recent version including documentation.
 *
 * $Id: triangle_distribution.hpp,v 1.11 2004/07/27 03:43:32 dgregor Exp $
 *
 * Revision history
 *  2001-02-18  moved to individual header files
 */
#endregion

using System;

namespace Altaxo.Calc.Probability
{
	/// <summary>
	/// Provides generation of triangular distributed random numbers.
	/// </summary>
	/// <remarks>
    /// The implementation of the <see cref="TriangularDistribution"/> type bases upon information presented on
    ///   <a href="http://en.wikipedia.org/wiki/Triangular_distribution">Wikipedia - Triangular distribution</a>
    ///   and the implementation in the <a href="http://www.boost.org/libs/random/index.html">Boost Random Number Library</a>.
    /// </remarks>
  public class TriangularDistribution : ContinuousDistribution
	{
		#region instance fields
		/// <summary>
        /// Gets or sets the parameter alpha which is used for generation of triangular distributed random numbers.
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
        /// Stores the parameter alpha which is used for generation of triangular distributed random numbers.
		/// </summary>
        private double alpha;
		
		/// <summary>
        /// Gets or sets the parameter beta which is used for generation of triangular distributed random numbers.
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
        /// Stores the parameter beta which is used for generation of triangular distributed random numbers.
		/// </summary>
        private double beta;
		
		/// <summary>
        /// Gets or sets the parameter gamma which is used for generation of triangular distributed random numbers.
		/// </summary>
		/// <remarks>Call <see cref="IsValidGamma"/> to determine whether a value is valid and therefor assignable.</remarks>
		public double Gamma
		{
			get
			{
                return this.gamma;
			}
			set
			{
                if (this.IsValidGamma(value))
                {
                    this.gamma = value;
                    this.UpdateHelpers();
                }
			}
		}

		/// <summary>
        /// Stores the parameter gamma which is used for generation of triangular distributed random numbers.
		/// </summary>
        private double gamma;

        /// <summary>
        /// Stores an intermediate result for generation of triangular distributed random numbers.
        /// </summary>
        /// <remarks>
        /// Speeds up random number generation cause this value only depends on distribution parameters 
        ///   and therefor doesn't need to be recalculated in successive executions of <see cref="NextDouble"/>.
        /// </remarks>
        private double helper1;

        /// <summary>
        /// Stores an intermediate result for generation of triangular distributed random numbers.
        /// </summary>
        /// <remarks>
        /// Speeds up random number generation cause this value only depends on distribution parameters 
        ///   and therefor doesn't need to be recalculated in successive executions of <see cref="NextDouble"/>.
        /// </remarks>
        private double helper2;

        /// <summary>
        /// Stores an intermediate result for generation of triangular distributed random numbers.
        /// </summary>
        /// <remarks>
        /// Speeds up random number generation cause this value only depends on distribution parameters 
        ///   and therefor doesn't need to be recalculated in successive executions of <see cref="NextDouble"/>.
        /// </remarks>
        private double helper3;

        /// <summary>
        /// Stores an intermediate result for generation of triangular distributed random numbers.
        /// </summary>
        /// <remarks>
        /// Speeds up random number generation cause this value only depends on distribution parameters 
        ///   and therefor doesn't need to be recalculated in successive executions of <see cref="NextDouble"/>.
        /// </remarks>
        private double helper4;
        #endregion

		#region construction
		/// <summary>
        /// Initializes a new instance of the <see cref="TriangularDistribution"/> class, using a 
        ///   <see cref="StandardGenerator"/> as underlying random number generator.
		/// </summary>
        public TriangularDistribution()
            : this(new StandardGenerator())
		{
		}

		/// <summary>
        /// Initializes a new instance of the <see cref="TriangularDistribution"/> class, using the specified 
        ///   <see cref="Generator"/> as underlying random number generator.
        /// </summary>
        /// <param name="generator">A <see cref="Generator"/> object.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="generator"/> is NULL (<see langword="Nothing"/> in Visual Basic).
        /// </exception>
        public TriangularDistribution(Generator generator)
            : base(generator)
        {
            this.alpha = 0.0;
            this.beta = 1.0;
            this.gamma = 0.5;
            this.UpdateHelpers();
        }
		#endregion
	
		#region instance methods
		/// <summary>
        /// Determines whether the specified value is valid for parameter <see cref="Alpha"/>.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <returns>
		/// <see langword="true"/> if value is less than <see cref="Beta"/>, and less than or equal to 
		///   <see cref="Gamma"/>; otherwise, <see langword="false"/>.
		/// </returns>
		public bool IsValidAlpha(double value)
		{
			return (value < this.beta && value <= this.gamma);
		}
		
		/// <summary>
        /// Determines whether the specified value is valid for parameter <see cref="Beta"/>.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <returns>
		/// <see langword="true"/> if value is greater than <see cref="Alpha"/>, and greater than or equal to 
		///   <see cref="Gamma"/>; otherwise, <see langword="false"/>.
		/// </returns>
		public bool IsValidBeta(double value)
		{
			return (value > this.alpha && value >= this.gamma);
		}
		
		/// <summary>
        /// Determines whether the specified value is valid for parameter <see cref="Gamma"/>.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <returns>
		/// <see langword="true"/> if value is greater than or equal to <see cref="Alpha"/>, and greater than or equal 
		///   to <see cref="Beta"/>; otherwise, <see langword="false"/>.
		/// </returns>
		public bool IsValidGamma(double value)
		{
			return (value >= this.alpha && value <= this.beta);
		}

        /// <summary>
        /// Updates the helper variables that store intermediate results for generation of triangular distributed random 
        ///   numbers.
        /// </summary>
        private void UpdateHelpers()
        {
            this.helper1 = this.gamma - this.alpha;
            this.helper2 = this.beta - this.alpha;
            this.helper3 = Math.Sqrt(this.helper1 * this.helper2);
            this.helper4 = Math.Sqrt(this.beta - this.gamma);
        }
        #endregion

		#region overridden Distribution members
        /// <summary>
		/// Gets the minimum possible value of triangular distributed random numbers.
		/// </summary>
        public override double Minimum
		{
			get
			{
				return this.alpha;
			}
		}

		/// <summary>
		/// Gets the maximum possible value of triangular distributed random numbers.
		/// </summary>
        public override double Maximum
		{
			get
			{
				return this.beta;
			}
		}

		/// <summary>
		/// Gets the mean value of triangular distributed random numbers.
		/// </summary>
        public override double Mean
		{
			get
			{
				return this.alpha / 3.0 + this.beta / 3.0 + this.gamma / 3.0;
			}
		}
		
		/// <summary>
		/// Gets the median of triangular distributed random numbers.
		/// </summary>
        public override double Median
		{
			get
			{
                if (this.gamma >= (this.beta - this.alpha) / 2.0)
                {
                    return this.alpha +
                        (Math.Sqrt((this.beta - this.alpha) * (this.gamma - this.alpha)) / Math.Sqrt(2.0));
                }
                else
                {
                    return this.beta -
                        (Math.Sqrt((this.beta - this.alpha) * (this.beta - this.gamma)) / Math.Sqrt(2.0));
                }
			}
		}
		
		/// <summary>
		/// Gets the variance of triangular distributed random numbers.
		/// </summary>
        public override double Variance
		{
			get
			{
				return (Math.Pow(this.alpha, 2.0) + Math.Pow(this.beta, 2.0) + Math.Pow(this.gamma, 2.0) -
					this.alpha * this.beta - this.alpha * this.gamma - this.beta * this.gamma) / 18.0;
			}
		}
		
		/// <summary>
		/// Gets the mode of triangular distributed random numbers.
		/// </summary>
        public override double[] Mode
		{
			get
			{
				return new double[] {this.gamma};
			}
		}
		
		/// <summary>
		/// Returns a triangular distributed floating point random number.
		/// </summary>
		/// <returns>A triangular distributed double-precision floating point number.</returns>
        public override double NextDouble()
        {
            double genNum = this.Generator.NextDouble();
            if (genNum <= this.helper1 / this.helper2)
            {
                return this.alpha + Math.Sqrt(genNum) * this.helper3;
            }
            else
            {
                return this.beta - Math.Sqrt(genNum * this.helper2 - this.helper1) * this.helper4;
            }
        }
        #endregion
	}
}