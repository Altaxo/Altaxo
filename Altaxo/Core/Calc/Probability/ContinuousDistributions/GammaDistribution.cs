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

#endregion Copyright

#region Further copyright(s)

// This file is partly based on Matpack 1.7.3 sources (Author B.Gammel)
// The following Matpack files were used here:
// matpack-1.7.3\source\random\rangamma.cc

// This file is partly based on Troschuetz.Random Class Library (Author S.Troschuetz)

#endregion Further copyright(s)

using System;

namespace Altaxo.Calc.Probability
{
  /// <summary>
  /// Generates Gamma distributed random numbers.
  /// </summary>
  /// <remarks><code>
  /// Return Gamma distributed random deviates according to:
  ///
  ///                      a-1  -bx
  ///                b (bx)    e
  ///  p   (x) dx = ---------------- dx   for x > 0
  ///   a,b             Gamma(a)
  ///
  ///             =  0                    otherwise
  ///                             //
  /// The arguments must satisfy the conditions:
  /// a &gt; 0   (positive)
  /// b != 0  (non-zero)
  ///
  /// References:
  ///
  /// For parameter a &gt;= 1 corresponds to algorithm GD in:
  /// J. H. Ahrens and U. Dieter, Generating Gamma Variates by a
  /// Modified Rejection Technique, Comm. ACM, 25, 1, 47-54 (1982).
  /// For parameter 0 &lt; a &lt; 1 corresponds to algorithm GS in:
  /// J. H. Ahrens and U. Dieter, Computer Methods for Sampling
  /// from Gamma, Beta, Poisson and Binomial Distributions,
  /// Computing, 12, 223-246 (1974).
  /// </code></remarks>
  public class GammaDistribution : ContinuousDistribution // , public ExponentialDistribution
  {
    protected NormalDistribution normalDistribution;
    protected ExponentialDistribution exponentialDistribution;
    protected double alpha, theta, _invTheta, s, s2, d, r, q0, b, si, c, scale;
    protected bool algorithmGD;

    /// <summary>
    /// Initializes this instance with the specified distribution parameters.
    /// </summary>
    /// <param name="alpha">Order (shape) parameter (must be positive).</param>
    /// <param name="theta">Scale parameter (must be non-zero).</param>
    /// <exception cref="ArgumentException">
    /// <paramref name="alpha"/> is not positive, or <paramref name="theta"/> is zero.
    /// </exception>
    public void Initialize(double alpha, double theta)
    {
      // check parameters
      if (alpha <= 0)
        throw new ArgumentException("alpha must be greater than zero");
      if (theta == 0)
        throw new ArgumentException("theta parameter must be non-zero");

      // store parameters
      this.alpha = alpha;  // a is the mean of the standard gamma distribution (b = 0)
      this.theta = theta;
      _invTheta = 1 / theta;

      // scale random long to (0,1) - boundaries are not allowed !
      scale = 1.0 / (normalDistribution.Generator.Maximum + 1.0); // original: scale  = 1.0 / (NormalDistribution::max_val+1.0);

      // select algorithm
      algorithmGD = (alpha >= 1);

      // initialize algorithm GD
      if (algorithmGD)
      {
        // coefficients q(k) for q0 = sum(q(k)*a**(-k))
        const double
                q1 = 4.166669e-2,
                q2 = 2.083148e-2,
                q3 = 8.01191e-3,
                q4 = 1.44121e-3,
                q5 = -7.388e-5,
                q6 = 2.4511e-4,
                q7 = 2.424e-4;

        // calculates s, s2, and d
        s2 = alpha - 0.5;
        s = Math.Sqrt(s2);
        d = Math.Sqrt(32.0) - 12.0 * s;

        // calculate q0, b, si, and c
        r = 1.0 / alpha;
        q0 = ((((((q7 * r + q6) * r + q5) * r + q4) * r + q3) * r + q2) * r + q1) * r;

        // Approximation depending on size of parameter A.
        // The constants in the expressions for b, si, and
        // c were established by numerical experiments.

        if (alpha <= 3.686)
        {   // case 1.0 <= A <= 3.686
          b = 0.463 + s + 0.178 * s2;
          si = 1.235;
          c = 0.195 / s - 7.9e-2 + 1.6e-1 * s;
        }
        else if (alpha <= 13.022)
        { // case  3.686 < A <= 13.022
          b = 1.654 + 7.6e-3 * s2;
          si = 1.68 / s + 0.275;
          c = 6.2e-2 / s + 2.4e-2;
        }
        else
        {     // case A > 13.022
          b = 1.77;
          si = 0.75;
          c = 0.1515 / s;
        }

        // initialize algorithm GS
      }
      else
      {
        b = 1.0 + 0.3678794 * alpha;
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GammaDistribution"/> class, using a
    /// <see cref="StandardGenerator"/> as underlying random number generator.
    /// </summary>
    public GammaDistribution()
      : this(DefaultGenerator)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GammaDistribution"/> class, using the specified
    /// <see cref="Generator"/> as underlying random number generator.
    /// </summary>
    /// <param name="gen">A <see cref="Generator"/> object.</param>
    public GammaDistribution(Generator gen)
      : this(1, 1, gen)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GammaDistribution"/> class.
    /// </summary>
    /// <param name="alpha">Order (shape) parameter (must be positive).</param>
    /// <param name="theta">Scale parameter (must be non-zero).</param>
    public GammaDistribution(double alpha, double theta)
      : this(alpha, theta, DefaultGenerator)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GammaDistribution"/> class.
    /// </summary>
    /// <param name="alpha">Order (shape) parameter (must be positive).</param>
    /// <param name="theta">Scale parameter (must be non-zero).</param>
    /// <param name="ran">A <see cref="Generator"/> object.</param>
    public GammaDistribution(double alpha, double theta, Generator ran)
      : base(ran)
    {
      normalDistribution = new NormalDistribution(0.0, 1.0, ran);
      exponentialDistribution = new ExponentialDistribution(1.0, ran); // std. exponential

      Initialize(alpha, theta);
    }

    /// <inheritdoc/>
    public override double NextDouble()
    {
      // algorithm GD for A >= 1
      if (algorithmGD)
      {
        const double
                // coefficients a(k) for q = q0+(t*t/2)*sum(a(k)*v**k)
                a1 = 0.3333333,
                a2 = -0.250003,
                a3 = 0.2000062,
                a4 = -0.1662921,
                a5 = 0.1423657,
                a6 = -0.1367177,
                a7 = 0.1233795,
                // coefficients e(k) for exp(q)-1 = sum(e(k)*q**k)
                e1 = 1.0,
                e2 = 0.4999897,
                e3 = 0.166829,
                e4 = 4.07753E-2,
                e5 = 1.0293E-2;

        double q, w, gamdis;

        // standard normal deviate
        double t = normalDistribution.NextDouble(); // original  this->NormalDistribution::operator()();

        // (s,1/2)-normal deviate
        double x = s + 0.5 * t;

        // immediate acceptance
        gamdis = x * x;
        if (t >= 0.0)
          return gamdis / _invTheta;

        // (0,1) uniform sample, squeeze acceptance
        double u = normalDistribution.Generator.Next() * scale; // original NormalDistribution::gen->Long() * scale;
        if (d * u <= t * t * t)
          return gamdis / _invTheta;

        // no quotient test if x not positive
        if (x > 0.0)
        {
          // calculation of v and quotient q
          double vv = t / (s + s);
          if (Math.Abs(vv) <= 0.25)
            q = q0 + 0.5 * t * t * ((((((a7 * vv + a6) * vv + a5) * vv + a4) * vv + a3) * vv + a2) * vv + a1) * vv;
          else
            q = q0 - s * t + 0.25 * t * t + (s2 + s2) * Math.Log(1.0 + vv);

          // quotient acceptance
          if (Math.Log(1.0 - u) <= q)
            return gamdis / _invTheta;
        }

loop:

// stdandard exponential deviate
        double e = exponentialDistribution.NextDouble(); // original this->ExponentialDistribution::operator()();

        // (0,1) uniform deviate
        u = normalDistribution.Generator.Next() * scale; // NormalDistribution::gen->Long() * scale;

        u += (u - 1.0);

        // (b,si) double exponential (Laplace)
        t = b + CopySign(si * e, u);

        // rejection if t < tau(1) = -0.71874483771719
        if (t < -0.71874483771719)
          goto loop;

        // calculation of v and quotient q
        double v = t / (s + s);
        if (Math.Abs(v) <= 0.25)
          q = q0 + 0.5 * t * t * ((((((a7 * v + a6) * v + a5) * v + a4) * v + a3) * v + a2) * v + a1) * v;
        else
          q = q0 - s * t + 0.25 * t * t + (s2 + s2) * Math.Log(1.0 + v);

        // hat acceptance
        if (q <= 0.0)
          goto loop;

        if (q <= 0.5)
          w = ((((e5 * q + e4) * q + e3) * q + e2) * q + e1) * q;
        else
          w = Math.Exp(q) - 1.0;

        // if t is rejected, sample again
        if (c * Math.Abs(u) > w * Math.Exp(e - 0.5 * t * t))
          goto loop;

        x = s + 0.5 * t;
        gamdis = x * x;
        return gamdis / _invTheta;

        // algorithm GS for 0 < A < 1
      }
      else
      {
        double gamdis;
        for (; ; )
        {
          double p = b * normalDistribution.Generator.Next() * scale;
          if (p < 1.0)
          {
            gamdis = Math.Exp(Math.Log(p) / alpha);
            if (exponentialDistribution.NextDouble() >= gamdis)
              return gamdis / _invTheta;
          }
          else
          {
            gamdis = -Math.Log((b - p) / alpha);
            if (exponentialDistribution.NextDouble() >= (1.0 - alpha) * Math.Log(gamdis))
              return gamdis / _invTheta;
          }
        } // for
      }
    }

    #region instance fields

    /// <summary>
    /// Gets the order (shape) parameter of the distribution.
    /// </summary>
    public double Order { get { return alpha; } }

    /// <summary>
    /// Gets or sets the parameter alpha which is used for generation of Gamma distributed random numbers.
    /// </summary>
    /// <remarks>Call <see cref="IsValidAlpha"/> to determine whether a value is valid and therefore assignable.</remarks>
    public double Alpha
    {
      get
      {
        return alpha;
      }
      set
      {
        Initialize(value, theta);
      }
    }

    /// <summary>
    /// Gets or sets the location parameter.
    /// </summary>
    public double Location
    {
      get { return _invTheta; }
      set
      {
        Initialize(alpha, 1 / value);
      }
    }

    /// <summary>
    /// Gets or sets the parameter theta which is used for generation of Gamma distributed random numbers.
    /// </summary>
    /// <remarks>Call <see cref="IsValidTheta"/> to determine whether a value is valid and therefore assignable.</remarks>
    public double Theta
    {
      get
      {
        return theta;
      }
      set
      {
        Initialize(alpha, value);
      }
    }

    #endregion instance fields

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

    #region overridden Distribution members

    /// <inheritdoc/>
    public override double Minimum
    {
      get
      {
        return 0.0;
      }
    }

    /// <inheritdoc/>
    public override double Maximum
    {
      get
      {
        return double.MaxValue;
      }
    }

    /// <inheritdoc/>
    public override double Mean
    {
      get
      {
        return alpha * theta;
      }
    }

    /// <inheritdoc/>
    public override double Median
    {
      get
      {
        return double.NaN;
      }
    }

    /// <inheritdoc/>
    public override double Variance
    {
      get
      {
        return alpha * Math.Pow(theta, 2.0);
      }
    }

    /// <inheritdoc/>
    public override double[] Mode
    {
      get
      {
        if (alpha >= 1.0)
        {
          return new double[] { (alpha - 1.0) * theta };
        }
        else
        {
          return new double[] { };
        }
      }
    }

    #endregion overridden Distribution members

    #region CdfPdfQuantile

    /// <inheritdoc/>
    public override double CDF(double x)
    {
      return CDF(x, alpha, theta);
    }

    /// <summary>
    /// Computes the cumulative distribution function (CDF) for a Gamma distribution with the given parameters.
    /// </summary>
    /// <param name="x">The value at which to evaluate the CDF.</param>
    /// <param name="A">Order (shape) parameter.</param>
    /// <param name="B">Scale parameter.</param>
    /// <returns>The value of the cumulative distribution function at <paramref name="x"/>.</returns>
    public static double CDF(double x, double A, double B)
    {
      return GammaRelated.GammaRegularized(A, 0, x / B);
    }

    /// <inheritdoc/>
    public override double PDF(double x)
    {
      return PDF(x, alpha, theta);
    }

    /// <summary>
    /// Computes the probability density function (PDF) for a Gamma distribution with the given parameters.
    /// </summary>
    /// <param name="x">The value at which to evaluate the PDF.</param>
    /// <param name="A">Order (shape) parameter.</param>
    /// <param name="B">Scale parameter.</param>
    /// <returns>The value of the probability density function at <paramref name="x"/>.</returns>
    public static double PDF(double x, double A, double B)
    {
      return Math.Exp(-x / B) * Math.Pow(x / B, A) / (x * Calc.GammaRelated.Gamma(A));
    }

    /// <inheritdoc/>
    public override double Quantile(double p)
    {
      return Quantile(p, alpha, theta);
    }

    /// <summary>
    /// Computes the quantile (inverse CDF) for a Gamma distribution with the given parameters.
    /// </summary>
    /// <param name="x">The probability for which to compute the quantile.</param>
    /// <param name="A">Order (shape) parameter.</param>
    /// <param name="B">Scale parameter.</param>
    /// <returns>The quantile corresponding to <paramref name="x"/>.</returns>
    public static double Quantile(double x, double A, double B)
    {
      //return GammaRelated.InverseGammaRegularized(A, 1 - p) / B;
      return GammaRelated.InverseGammaRegularized(A, 1 - x) * B;
    }

    #endregion CdfPdfQuantile
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
        else
        {
          throw new ArgumentOutOfRangeException("Alpha is out of range");
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
                else
                {
                  throw new ArgumentOutOfRangeException("Theta is out of range");
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

#endregion instance fields

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

#endregion construction, destruction

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
        public void UpdateHelpers()
        {
            this.helper1 = this.alpha - Math.Floor(this.alpha);
            this.helper2 = Math.E / (Math.E + this.helper1);
        }

#endregion instance methods

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

#endregion overridden Distribution members

#region CdfPdfQuantile

    public override double CDF(double x)
    {
      return CDF(x, alpha, theta);
    }
    public static double CDF(double x, double A, double B)
    {
      return GammaRelated.GammaRegularized(A, 0, x/B);
    }
    public override double PDF(double x)
    {
      return PDF(x, alpha, theta);
    }
    public static double PDF(double x, double A, double B)
    {
      return Math.Exp(-x/B) * Math.Pow(x/B, A) / (x*Calc.GammaRelated.Gamma(A));
    }

    public override double Quantile(double p)
    {
      return Quantile(p, alpha, theta);
    }
    public static double Quantile(double x, double A, double B)
    {
      //return GammaRelated.InverseGammaRegularized(A, 1 - p) / B;
      return GammaRelated.InverseGammaRegularized(A, 1-x) * B;
    }

#endregion CdfPdfQuantile
	}
}

#endif
