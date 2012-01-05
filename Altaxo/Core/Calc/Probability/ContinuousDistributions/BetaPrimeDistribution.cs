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
 * BetaPrimeDistribution.cs, 16.08.2006
 * 
 */

using System;

namespace Altaxo.Calc.Probability
{
    /// <summary>
    /// Provides generation of beta-prime distributed random numbers. This distribution is alsow known as inverted beta distribution.
    /// </summary>
    /// <remarks>
    /// The implementation of the <see cref="BetaPrimeDistribution"/> type bases upon information presented on
    ///   <a href="http://www.xycoon.com/ibeta.htm">Xycoon - Inverted Beta Distribution</a>.
    /// </remarks>
  public class BetaPrimeDistribution : ContinuousDistribution
    {
        #region instance fields
        /// <summary>
        /// Gets or sets the parameter alpha which is used for generation of beta-prime distributed random numbers.
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
              Initialize(value, beta);
            }
        }

        /// <summary>
        /// Stores the parameter alpha which is used for generation of beta-prime distributed random numbers.
        /// </summary>
        private double alpha;

        /// <summary>
        /// Gets or sets the parameter beta which is used for generation of beta-prime distributed random numbers.
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
              Initialize(alpha, value);
            }
        }

        /// <summary>
        /// Stores the parameter beta which is used for generation of beta-prime distributed random numbers.
        /// </summary>
        private double beta;

        /// <summary>
        /// Stores a <see cref="BetaDistribution"/> object used for generation of beta-prime distributed random numbers.
        /// </summary>
        private BetaDistribution betaDistribution;
        #endregion

        #region construction
        /// <summary>
        /// Initializes a new instance of the <see cref="BetaPrimeDistribution"/> class, using a 
        ///   <see cref="StandardGenerator"/> as underlying random number generator.
        /// </summary>
        public BetaPrimeDistribution()
            : this(DefaultGenerator)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BetaPrimeDistribution"/> class, using the specified 
        ///   <see cref="Generator"/> as underlying random number generator.
        /// </summary>
        /// <param name="generator">A <see cref="Generator"/> object.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="generator"/> is NULL (<see langword="Nothing"/> in Visual Basic).
        /// </exception>
        public BetaPrimeDistribution(Generator generator)
            : this(2,2,generator)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BetaPrimeDistribution"/> class, using the default generator.
        /// </summary>
        public BetaPrimeDistribution(double alpha, double beta)
          : this(alpha, beta, DefaultGenerator)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="BetaPrimeDistribution"/> class, using the specified 
        ///   <see cref="Generator"/> as underlying random number generator.
        /// </summary>
				/// <param name="alpha">First parameter of the distribution.</param>
				/// <param name="beta">Second parameter of the distribution.</param>
        /// <param name="generator">A <see cref="Generator"/> object.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="generator"/> is NULL (<see langword="Nothing"/> in Visual Basic).
        /// </exception>
        public BetaPrimeDistribution(double alpha, double beta, Generator generator)
          : base(generator)
        {
          Initialize(alpha, beta);
        }


#endregion

        #region instance methods
        /// <summary>
        /// Determines whether the specified value is valid for parameter <see cref="Alpha"/>.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>
        /// <see langword="true"/> if value is greater than 1.0; otherwise, <see langword="false"/>.
        /// </returns>
        public bool IsValidAlpha(double value)
        {
            return value > 1.0;
        }

        /// <summary>
        /// Determines whether the specified value is valid for parameter <see cref="Beta"/>.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>
        /// <see langword="true"/> if value is greater than 1.0; otherwise, <see langword="false"/>.
        /// </returns>
        public bool IsValidBeta(double value)
        {
            return value > 1.0;
        }

        /// <summary>
        /// Updates the helper variables that store intermediate results for generation of beta-prime distributed random 
        ///   numbers.
        /// </summary>
        public void Initialize(double alpha, double beta)
        {
          if (!IsValidAlpha(alpha))
            throw new ArgumentOutOfRangeException("alpha has to be greater than 1");
          if (!IsValidBeta(beta))
            throw new ArgumentOutOfRangeException("beta has to be greater than 1");
          
          this.alpha = alpha;
          this.beta = beta;
					this.betaDistribution = new BetaDistribution();
          this.betaDistribution.Initialize(this.alpha, this.beta);
        }
        #endregion

        #region overridden IDistribution members
        /// <summary>
        /// Gets the minimum possible value of beta-prime distributed random numbers.
        /// </summary>
        public override double Minimum
        {
            get
            {
                return 0.0;
            }
        }

        /// <summary>
        /// Gets the maximum possible value of beta-prime distributed random numbers.
        /// </summary>
        public override double Maximum
        {
            get
            {
                return double.MaxValue;
            }
        }

        /// <summary>
        /// Gets the mean value of beta-prime distributed random numbers.
        /// </summary>
        public override double Mean
        {
            get
            {
                return this.alpha / (this.beta - 1.0);
            }
        }

        /// <summary>
        /// Gets the median of beta-prime distributed random numbers.
        /// </summary>
        public override double Median
        {
            get
            {
                return double.NaN;
            }
        }

        /// <summary>
        /// Gets the variance of beta-prime distributed random numbers.
        /// </summary>
        public override double Variance
        {
            get
            {
                if (this.beta > 2)
                {
                    return this.alpha * (this.alpha + this.beta - 1.0) / (Math.Pow(this.beta - 1.0, 2) * (this.beta - 2.0));
                }
                else
                {
                    return double.NaN;
                }
            }
        }

        /// <summary>
        /// Gets the mode of beta-prime distributed random numbers.
        /// </summary>
        public override double[] Mode
        {
            get
            {
                return new double[] { (this.alpha - 1.0) / (this.beta + 1.0) };
            }
        }

        /// <summary>
        /// Returns a beta-prime distributed floating point random number.
        /// </summary>
        /// <returns>A beta-prime distributed double-precision floating point number.</returns>
        public override double NextDouble()
        {
            double betaVariate = this.betaDistribution.NextDouble();

            return betaVariate / (1.0 - betaVariate);
        }
        #endregion
    }
}