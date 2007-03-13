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
 * Distribution.cs, 24.09.2006
 * 
 * 09.08.2006: Initial version
 * 21.09.2006: Changed the access modifier of field "generator" from protected to private 
 *               and made it accessible through the new protected property "Generator"
 * 24.09.2006: All exceptions are instantiated with localized messages
 * 
 */

using System;

namespace Altaxo.Calc.Probability
{
  using Resources;
	/// <summary>
    /// Declares common functionality for all random number distributions.
	/// </summary>
	public abstract class Distribution
    {
        #region instance fields
        /// <summary>
        /// Gets or sets a <see cref="Generator"/> object that can be used as underlying random number generator.
        /// </summary>
        protected Generator Generator
        {
            get
            {
                return this.generator;
            }
            set
            {
                this.generator = value;
            }
        }

        /// <summary>
        /// Stores a <see cref="Generator"/> object that can be used as underlying random number generator.
        /// </summary>
        private Generator generator;

        /// <summary>
        /// Gets a value indicating whether the random number distribution can be reset, so that it produces the same 
        ///   random number sequence again.
        /// </summary>
        public bool CanReset
        {
            get
            {
                return this.generator.CanReset;
            }
        }
        #endregion

        #region construction
        /// <summary>
        /// Initializes a new instance of the <see cref="Distribution"/> class, using a 
        ///   <see cref="StandardGenerator"/> as underlying random number generator.
        /// </summary>
        protected Distribution()
            : this(new StandardGenerator())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Distribution"/> class, using the specified 
        ///   <see cref="Generator"/> as underlying random number generator.
        /// </summary>
        /// <param name="generator">A <see cref="Generator"/> object.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="generator"/> is NULL (<see langword="Nothing"/> in Visual Basic).
        /// </exception>
        protected Distribution(Generator generator)
        {
            if (generator == null)
            {
                string message = string.Format(null, ExceptionMessages.ArgumentNull, "generator");
                throw new ArgumentNullException("generator", message);
            }
            this.generator = generator;
        }
        #endregion

        #region instance methods
        /// <summary>
        /// Resets the random number distribution, so that it produces the same random number sequence again.
        /// </summary>
        /// <returns>
        /// <see langword="true"/>, if the random number distribution was reset; otherwise, <see langword="false"/>.
        /// </returns>
        public bool Reset()
        {
            return this.generator.Reset();
        }
        #endregion

        #region abstract members
        /// <summary>
		/// Gets the minimum possible value of distributed random numbers.
        /// </summary>
        public abstract double Minimum
		{
			get;
		}

        /// <summary>
		/// Gets the maximum possible value of distributed random numbers.
        /// </summary>
        public abstract double Maximum
		{
			get;
		}

        /// <summary>
		/// Gets the mean of distributed random numbers.
        /// </summary>
        public abstract double Mean
		{
			get;
		}
		
		/// <summary>
		/// Gets the median of distributed random numbers.
		/// </summary>
        public abstract double Median
		{
			get;
		}

        /// <summary>
		/// Gets the variance of distributed random numbers.
        /// </summary>
        public abstract double Variance
		{
			get;
		}
		
		/// <summary>
		/// Gets the mode of distributed random numbers.
		/// </summary>
        public abstract double[] Mode
		{
			get;
		}
		
		/// <summary>
		/// Returns a distributed floating point random number.
		/// </summary>
		/// <returns>A distributed double-precision floating point number.</returns>
        public abstract double NextDouble();
        #endregion
    }
}