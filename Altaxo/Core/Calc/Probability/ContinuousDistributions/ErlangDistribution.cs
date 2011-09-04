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
// matpack-1.7.3\source\random\ranerlang.cc

// This file is partly based on Troschuetz.Random Class Library (Author S.Troschuetz)
#endregion

using System;

namespace Altaxo.Calc.Probability
{

/// <summary>
/// Generates Erlang distributed random numbers.
/// </summary>
/// <remarks><code>
///                           
/// Return Erlang distributed random deviates according to:        
///                             
///                      a-1  -bx              
///                b (bx)    e                  
///  p   (x) dx = ---------------- dx   for x > 0           
///   a,b             Gamma(a)                 
///                         
///             =  0                    otherwise          
///                             
/// The Erlang distribution is a special case of the Gamma distribution         
/// with integer valued order a.                       
///                            
/// References:                     
/// see references in:                       
/// W. H. Press, B. P. Flannery, S. A. Teukolsky, W. T. Vetterling,       
/// Numerical Recipies in C, Cambridge Univ. Press, 1988.         
/// </code></remarks>

public class ErlangDistribution : ContinuousDistribution
{
  protected int A;
  protected double B, a1, sq, scale, scale2,lambda;
  public void Initialize(int order, double lambda)
  {
    if (order < 1)
      throw new ArgumentException("order must be greater or equal than 1");
    else if (lambda == 0)
      throw new ArgumentException("location parameter must be non-zero");
    else
    {
      scale = 1.0 / generator.Maximum;   // scale long to [0,1]
      scale2 = 2.0 / generator.Maximum;   // auxilliary
      A = order;      // order of Erlang distribution
      a1 = A - 1.0;     // auxilliary
      sq = Math.Sqrt(2 * a1 + 1); // auxilliary
      B = 1/lambda;      // location parmeter
      this.lambda = lambda;
    }
  }
  public ErlangDistribution() : this(DefaultGenerator) { }
  public ErlangDistribution(Generator gen) : this(1, 1, gen) { }
  public ErlangDistribution(int order, double lambda) : this(order, lambda, DefaultGenerator) { }
  public ErlangDistribution(int order, double lambda, Generator ran)
    : base(ran)
  {
    Initialize(order, lambda);
  }
  public override double NextDouble()
  {
    if (A < 6)
    { // direct method
      double x;
      do
      {
        x = generator.Next() * scale;
        for (int i = 1; i < A; i++) x *= generator.Next() * scale;
      } while (x <= 0.0);
      return (-Math.Log(x) / B);

    }
    else
    {   // rejection method
      double x, y, b;
      do
      {
        do
        {
          double v1, v2;
          do
          {
            v1 = scale2 * generator.Next() - 1;
            v2 = scale2 * generator.Next() - 1;
          } while ((v1 == 0.0) || (v1 * v1 + v2 * v2 > 1.0));
          y = v2 / v1;
          x = sq * y + a1;
        } while (x <= 0.0);
        b = (1.0 + y * y) * Math.Exp(a1 * Math.Log(x / a1) - sq * y);
      } while ((scale * generator.Next()) > b);
      return x / B;
    }
  }



  public int Order { get { return A; } }
  public double Location { get { return B; } }

  /// <summary>
  /// Gets or sets the parameter alpha which is used for generation of erlang distributed random numbers.
  /// </summary>
  /// <remarks>Call <see cref="IsValidAlpha"/> to determine whether a value is valid and therefor assignable.</remarks>
  public int Alpha
  {
    get
    {
      return this.A;
    }
    set
    {
      Initialize(value, lambda);
    }
  }

  /// <summary>
  /// Gets or sets the parameter lambda which is used for generation of erlang distributed random numbers.
  /// </summary>
  /// <remarks>Call <see cref="IsValidLambda"/> to determine whether a value is valid and therefor assignable.</remarks>
  public double Lambda
  {
    get
    {
      return this.lambda;
    }
    set
    {
      Initialize(Order, value);
    }
  }

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
  /// Determines whether the specified value is valid for parameter <see cref="Lambda"/>.
  /// </summary>
  /// <param name="value">The value to check.</param>
  /// <returns>
  /// <see langword="true"/> if value is greater than 0.0; otherwise, <see langword="false"/>.
  /// </returns>
  public bool IsValidLambda(double value)
  {
    return value > 0.0;
  }


  #region overridden Distribution members
  /// <summary>
  /// Gets the minimum possible value of erlang distributed random numbers.
  /// </summary>
  public override double Minimum
  {
    get
    {
      return 0.0;
    }
  }

  /// <summary>
  /// Gets the maximum possible value of erlang distributed random numbers.
  /// </summary>
  public override double Maximum
  {
    get
    {
      return double.MaxValue;
    }
  }

  /// <summary>
  /// Gets the mean value of erlang distributed random numbers.
  /// </summary>
  public override double Mean
  {
    get
    {
      return this.A / this.lambda;
    }
  }

  /// <summary>
  /// Gets the median of erlang distributed random numbers.
  /// </summary>
  public override double Median
  {
    get
    {
      return double.NaN;
    }
  }

  /// <summary>
  /// Gets the variance of erlang distributed random numbers.
  /// </summary>
  public override double Variance
  {
    get
    {
      return this.A / Math.Pow(this.lambda, 2.0);
    }
  }

  /// <summary>
  /// Gets the mode of erlang distributed random numbers.
  /// </summary>
  public override double[] Mode
  {
    get
    {
      return new double[] { (this.A - 1) / this.lambda };
    }
  }

 
  #endregion

  /*
  public override double PDF(double x)
  {
    return Math.Exp(-B * x) * Math.Pow(B * x, A - 1) * B / Calc.GammaRelated.Gamma(A);
  }

  public override double CDF(double x)
  {
    return GammaRelated.GammaRegularized(A, 0, B * x);
  }

  public override double Quantile(double p)
  {
    return GammaRelated.InverseGammaRegularized(A, 1 - p) / B;
  }
  */
  #region CdfPdfQuantile
  public override double CDF(double x)
  {
    return CDF(x, A, lambda);
  }
  public static double CDF(double x, double A, double B)
  {
    return GammaRelated.GammaRegularized(A, 0, x / B);
  }
  public override double PDF(double x)
  {
    return PDF(x, A, lambda);
  }
  public static double PDF(double x, double A, double B)
  {
    return Math.Exp(-x / B) * Math.Pow(x / B, A) / (x * Calc.GammaRelated.Gamma(A));
  }

  public override double Quantile(double p)
  {
    return Quantile(p, A, lambda);
  }
  public static double Quantile(double x, double A, double B)
  {
    //return GammaRelated.InverseGammaRegularized(A, 1 - p) / B;
    return GammaRelated.InverseGammaRegularized(A, 1 - x) * B;
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
 * ErlangDistribution.cs, 21.09.2006
 * 
 * 16.08.2006: Initial version
 * 21.09.2006: Adapted to change in base class (field "generator" declared private (formerly protected) 
 *               and made accessible through new protected property "Generator")
 * 
 */

using System;

namespace Altaxo.Calc.Probability
{
	/// <summary>
	/// Provides generation of erlang distributed random numbers.
	/// </summary>
	/// <remarks>
    /// The implementation of the <see cref="ErlangDistribution"/> type bases upon information presented on
    ///   <a href="http://en.wikipedia.org/wiki/Erlang_distribution">Wikipedia - Erlang distribution</a> and
    ///   <a href="http://www.xycoon.com/erlang_random.htm">Xycoon - Erlang Distribution</a>.
    /// </remarks>
  public class ErlangDistribution : ContinuousDistribution
	{
		#region instance fields
		/// <summary>
		/// Gets or sets the parameter alpha which is used for generation of erlang distributed random numbers.
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
		/// Stores the parameter alpha which is used for generation of erlang distributed random numbers.
		/// </summary>
		private int alpha;
		
		/// <summary>
		/// Gets or sets the parameter lambda which is used for generation of erlang distributed random numbers.
		/// </summary>
        /// <remarks>Call <see cref="IsValidLambda"/> to determine whether a value is valid and therefor assignable.</remarks>
		public double Lambda
		{
			get
			{
				return this.lambda;
			}
			set
			{
                if (this.IsValidLambda(value))
                {
                    this.lambda = value;
                    this.UpdateHelpers();
                }
        	}
		}

		/// <summary>
		/// Stores the parameter lambda which is used for generation of erlang distributed random numbers.
		/// </summary>
		private double lambda;

        /// <summary>
        /// Stores an intermediate result for generation of erlang distributed random numbers.
        /// </summary>
        /// <remarks>
        /// Speeds up random number generation cause this value only depends on distribution parameters 
        ///   and therefor doesn't need to be recalculated in successive executions of <see cref="NextDouble"/>.
        /// </remarks>
        private double helper1;
        #endregion

		#region construction
		/// <summary>
        /// Initializes a new instance of the <see cref="ErlangDistribution"/> class, using a 
        ///   <see cref="StandardGenerator"/> as underlying random number generator.
		/// </summary>
        public ErlangDistribution()
            : this(new StandardGenerator())
		{
		}

		/// <summary>
        /// Initializes a new instance of the <see cref="ErlangDistribution"/> class, using the specified 
        ///   <see cref="Generator"/> as underlying random number generator.
        /// </summary>
        /// <param name="generator">A <see cref="Generator"/> object.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="generator"/> is NULL (<see langword="Nothing"/> in Visual Basic).
        /// </exception>
        public ErlangDistribution(Generator generator)
            : base(generator)
        {
            this.alpha = 1;
            this.lambda = 1.0;
            this.UpdateHelpers();
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
		/// Determines whether the specified value is valid for parameter <see cref="Lambda"/>.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <returns>
		/// <see langword="true"/> if value is greater than 0.0; otherwise, <see langword="false"/>.
		/// </returns>
        public bool IsValidLambda(double value)
		{
			return value > 0.0;
		}

        /// <summary>
        /// Updates the helper variables that store intermediate results for generation of erlang distributed random 
        ///   numbers.
        /// </summary>
        private void UpdateHelpers()
        {
            this.helper1 = -1.0 / this.lambda;
        }

        #endregion

		#region overridden Distribution members
        /// <summary>
		/// Gets the minimum possible value of erlang distributed random numbers.
		/// </summary>
		public override double Minimum
		{
			get
			{
				return 0.0;
			}
		}

		/// <summary>
		/// Gets the maximum possible value of erlang distributed random numbers.
		/// </summary>
        public override double Maximum
		{
			get
			{
				return double.MaxValue;
			}
		}

		/// <summary>
		/// Gets the mean value of erlang distributed random numbers.
		/// </summary>
        public override double Mean
		{
			get
			{
                return this.alpha / this.lambda;
			}
		}
		
		/// <summary>
		/// Gets the median of erlang distributed random numbers.
		/// </summary>
        public override double Median
		{
			get
			{
				return double.NaN;
			}
		}
		
		/// <summary>
		/// Gets the variance of erlang distributed random numbers.
		/// </summary>
        public override double Variance
		{
			get
			{
                return this.alpha / Math.Pow(this.lambda, 2.0);
			}
		}
		
		/// <summary>
		/// Gets the mode of erlang distributed random numbers.
		/// </summary>
        public override double[] Mode
		{
            get
            {
                return new double[] { (this.alpha - 1) / this.lambda };
            }
		}
		
		/// <summary>
		/// Returns a erlang distributed floating point random number.
		/// </summary>
		/// <returns>A erlang distributed double-precision floating point number.</returns>
        public override double NextDouble()
		{
            double product = 1.0;
            for (int i = 0; i < this.alpha; i++)
            {
                product *= this.Generator.NextDouble();
            }

            // Subtract product from 1.0 to avoid Math.Log(0.0)
            return this.helper1 * Math.Log(1.0 - product);
		}
		#endregion

    #region CdfPdfQuantile
    public override double CDF(double x)
    {
      return CDF(x, alpha, lambda);
    }
    public static double CDF(double x, int A, double B)
    {
      return GammaRelated.GammaRegularized(A, 0, B * x);
    }
    
    public override double PDF(double x)
    {
      return PDF(x, alpha, lambda);
    }
    public static double PDF(double x, int A, double B)
    {
      return Math.Exp(-B * x) * Math.Pow(B * x, A - 1) * B / Calc.GammaRelated.Gamma(A);
    }


    public override double Quantile(double p)
    {
      return Quantile(p, alpha, lambda);
    }
    public static double Quantile(double p, int A, double B)
    {
      return GammaRelated.InverseGammaRegularized(A, 1 - p) / B;
    }

    #endregion
  }
}
#endif