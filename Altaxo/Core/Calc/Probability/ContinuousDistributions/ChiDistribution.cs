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
        /// Gets or sets the parameter N (degree of freedom) which is used for generation of chi distributed random numbers.
		/// </summary>
		/// <remarks>Call <see cref="IsValidN"/> to determine whether a value is valid and therefor assignable.</remarks>
		public int N
		{
			get
			{
                return this._N;
			}
			set
			{
        Initialize(value);
     	}
		}

		/// <summary>
        /// Stores the parameter alpha which is used for generation of chi distributed random numbers.
		/// </summary>
        private int _N;

        /// <summary>
        /// Stores a <see cref="NormalDistribution"/> object used for generation of chi distributed random numbers.
        /// </summary>
        private NormalDistribution _normalDistribution;
        #endregion

		#region construction
		/// <summary>
        /// Initializes a new instance of the <see cref="ChiDistribution"/> class, using a 
        ///   <see cref="StandardGenerator"/> as underlying random number generator.
		/// </summary>
        public ChiDistribution()
            : this(DefaultGenerator)
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
            : this(1,generator)
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
        public ChiDistribution(int N)
          : this(N,DefaultGenerator)
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
        public ChiDistribution(int N, Generator generator)
          : base(generator)
        {
          this._normalDistribution = new NormalDistribution(0,1,generator);
          Initialize(N);
        }


		#endregion
	
		#region instance methods

    public void Initialize(int N)
    {
      if (!IsValidN(N))
        throw new ArgumentOutOfRangeException("N out of range (must be >0)");
      this._N = N;
    }

		/// <summary>
        /// Determines whether the specified value is valid for parameter <see cref="Alpha"/>.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <returns>
		/// <see langword="true"/> if value is greater than 0; otherwise, <see langword="false"/>.
		/// </returns>
        public bool IsValidN(int value)
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
                return Math.Sqrt(2.0) * this.Gamma((this._N + 1.0) / 2.0) / this.Gamma(this._N / 2.0);
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
                return this._N - Math.Pow(this.Mean, 2.0);
			}
		}
		
		/// <summary>
        /// Gets the mode of chi distributed random numbers.
		/// </summary>
        public override double[] Mode
		{
            get
            {
                if (this._N >= 1)
                {
                    return new double[] { Math.Sqrt(this._N - 1.0) };
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
            for (int i = 0; i < this._N; i++)
            {
                sum += Math.Pow(this._normalDistribution.NextDouble(), 2);
            }

            return Math.Sqrt(sum);
		}
        #endregion

    #region CdfPdfQuantile

    public override double CDF(double x)
    {
      return CDF(x, _N);
    }
    public static double CDF(double x, double N)
    {
      return Calc.GammaRelated.GammaRegularized(0.5*N, 0, 0.5*x*x);
    }



    public override double PDF(double x)
    {
      return PDF(x, _N);
    }
    public static double PDF(double x, double N)
    {
      return (Math.Pow(2, 1 - N / 2) * Math.Pow(x, -1 + N)) / (Math.Exp(x * x * 0.5) * GammaRelated.Gamma(N / 2));
    }


    public override double Quantile(double p)
    {
      return Quantile(p, _N);
    }
    public static double Quantile(double p, double N)
    {
      return Math.Sqrt(2) * Math.Sqrt(GammaRelated.InverseGammaRegularized(N / 2, 1 - p));
    }


    #endregion
    }
}