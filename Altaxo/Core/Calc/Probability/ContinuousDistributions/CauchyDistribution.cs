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
 * CauchyDistribution.cs, 21.09.2006
 * 
 * 09.08.2006: Initial version
 * 21.09.2006: Adapted to change in base class (field "generator" declared private (formerly protected) 
 *               and made accessible through new protected property "Generator")
 */

using System;

namespace Altaxo.Calc.Probability
{
    /// <summary>
    /// Provides generation of cauchy distributed random numbers.
    /// </summary>
    /// <remarks>
    /// The implementation of the <see cref="CauchyDistribution"/> type bases upon information presented on
    ///   <a href="http://en.wikipedia.org/wiki/Cauchy_distribution">Wikipedia - Cauchy distribution</a> and
    ///   <a href="http://www.xycoon.com/cauchy2p_random.htm">Xycoon - Cauchy Distribution</a>.
    /// </remarks>
  public class CauchyDistribution : ContinuousDistribution
    {
        #region instance fields
        /// <summary>
        /// Gets or sets the parameter alpha of cauchy distributed random numbers which is used for their generation.
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
                }
            }
        }

        /// <summary>
        /// Stores the parametera alpha of cauchy distributed random numbers which is used for their generation.
        /// </summary>
        private double alpha;

        /// <summary>
        /// Gets or sets the parameter gamma which is used for generation of cauchy distributed random numbers.
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
                }
            }
        }

        /// <summary>
        /// Stores the parameter gamma which is used for generation of cauchy distributed random numbers.
        /// </summary>
        private double gamma;
        #endregion

        #region construction
        /// <summary>
        /// Initializes a new instance of the <see cref="CauchyDistribution"/> class, using a 
        ///   <see cref="StandardGenerator"/> as underlying random number generator. 
        /// </summary>
        public CauchyDistribution()
            : this(new StandardGenerator())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CauchyDistribution"/> class, using the specified 
        ///   <see cref="Generator"/> as underlying random number generator.
        /// </summary>
        /// <param name="generator">A <see cref="Generator"/> object.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="generator"/> is NULL (<see langword="Nothing"/> in Visual Basic).
        /// </exception>
        public CauchyDistribution(Generator generator)
            : base(generator)
        {
            this.alpha = 1.0;
            this.gamma = 1.0;
        }
        #endregion

        #region instance methods
        /// <summary>
        /// Determines whether the specified value is valid for parameter <see cref="Alpha"/>.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns><see langword="true"/>.</returns>
        public bool IsValidAlpha(double value)
        {
            return true;
        }
        
        /// <summary>
        /// Determines whether the specified value is valid for parameter <see cref="Gamma"/>.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>
        /// <see langword="true"/> if value is greater than 0.0; otherwise, <see langword="false"/>.
        /// </returns>
        public bool IsValidGamma(double value)
        {
            return value > 0.0;
        }
        #endregion

        #region overridden Distribution members
        /// <summary>
        /// Gets the minimum possible value of cauchy distributed random numbers.
        /// </summary>
        public override double Minimum
        {
            get
            {
                return double.MinValue;
            }
        }

        /// <summary>
        /// Gets the maximum possible value of cauchy distributed random numbers.
        /// </summary>
        public override double Maximum
        {
            get
            {
                return double.MaxValue;
            }
        }

        /// <summary>
        /// Gets the mean value of cauchy distributed random numbers.
        /// </summary>
        public override double Mean
        {
            get
            {
                return double.NaN;
            }
        }

        /// <summary>
        /// Gets the median of cauchy distributed random numbers.
        /// </summary>
        public override double Median
        {
            get
            {
                return this.alpha;
            }
        }

        /// <summary>
        /// Gets the variance of cauchy distributed random numbers.
        /// </summary>
        public override double Variance
        {
            get
            {
                return double.NaN;
            }
        }

        /// <summary>
        /// Gets the mode of cauchy distributed random numbers.
        /// </summary>
        public override double[] Mode
        {
            get
            {
                return new double[] { this.alpha };
            }
        }

        /// <summary>
        /// Returns a cauchy distributed floating point random number.
        /// </summary>
        /// <returns>A cauchy distributed double-precision floating point number.</returns>
        public override double NextDouble()
        {
            return this.alpha + this.gamma * Math.Tan(Math.PI * (this.Generator.NextDouble() - 0.5));
        }
        #endregion
    }
}