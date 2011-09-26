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

using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Calc.Probability
{
	/// <summary>
	/// Base class for continuous probability distribution functions.
	/// </summary>
  public abstract class ContinuousDistribution : Distribution
  {

		/// <summary>Initializes a new instance of the <see cref="ContinuousDistribution"/> class.</summary>
		/// <param name="generator">The random number generator used.</param>
    public ContinuousDistribution(Generator generator)
      : base(generator)
    {
    }

		/// <summary>Calculates the cumulative distribution function.</summary>
		/// <param name="x">Argument.</param>
		/// <returns>The probability that the random variable of this probability distribution will be found at a value less than or equal to <paramref name="x"/>.</returns>
    public virtual double CDF(double x)
    {
      throw new NotImplementedException();
    }

		/// <summary>Calculates the probability density function.</summary>
		/// <param name="x">Argument.</param>
		/// <returns>The relative likelihood for the random variable to occur at the point <paramref name="x"/>.</returns>
    public virtual double PDF(double x)
    {
      throw new NotImplementedException();
    }

		/// <summary>Calculates the quantile of the distribution function.</summary>
		/// <param name="p">The probability p.</param>
		/// <returns>The point x at which the cumulative distribution function <see cref="CDF"/>() of argument x is equal to <paramref name="p"/>.</returns>
    public virtual double Quantile(double p)
    {
      throw new NotImplementedException();
    }

    #region Helper functions and constants

    /// <summary>Maximum value of a System.Double.</summary>
    protected const double DBL_MAX = double.MaxValue;
    /// <summary>Smallest positive value of a System.Double.</summary>
    protected const double DBL_MIN = double.Epsilon; // god saves microsoft!
    /// <summary>Maximum binary exponent of a <see cref="System.Double"/>.</summary>
    protected const int DBL_MAX_EXP = 1024;
    /// <summary>Natural logarithm of 2.</summary>
    protected const double M_LN2 = 0.69314718055994530941723212145818;


    /// <summary>
    /// Return first number with sign of second number
    /// </summary>
    /// <param name="x">The first number.</param>
    /// <param name="y">The second number whose sign is used.</param>
    /// <returns>The first number x with the sign of the second argument y.</returns>
    protected static double CopySign(double x, double y)
    {
      return (y < 0) ? ((x < 0) ? x : -x) : ((x > 0) ? x : -x);
    }

    #endregion

  }
}
