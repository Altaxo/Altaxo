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
 * PoissonDistribution.cs, 21.09.2006
 * 
 * 09.08.2006: Initial version
 * 21.09.2006: Adapted to change in base class (field "generator" declared private (formerly protected) 
 *               and made accessible through new protected property "Generator")
 * 
 */

#region original copyrights
//   -*- C++ -*-
/*****************************************************************************
 *
 *   |_|_|_  |_|_    |_    |_|_|_  |_		     C O M M U N I C A T I O N
 * |_        |_  |_  |_  |_        |_		               N E T W O R K S
 * |_        |_  |_  |_  |_        |_		                     C L A S S
 *   |_|_|_  |_    |_|_    |_|_|_  |_|_|_|_	                 L I B R A R Y
 *
 * $Id: Poisson.c,v 1.2 2002/01/14 11:37:33 spee Exp $
 *
 * CNClass: CNPoisson --- CNPoisson distributed random numbers 
 *
 *****************************************************************************
 * Copyright (C) 1992-1996   Communication Networks
 *                           Aachen University of Technology
 *                           D-52056 Aachen
 *                           Germany
 *                           Email: cncl-adm@comnets.rwth-aachen.de
 *****************************************************************************
 * This file is part of the CN class library. All files marked with
 * this header are free software; you can redistribute it and/or modify
 * it under the terms of the GNU Library General Public License as
 * published by the Free Software Foundation; either version 2 of the
 * License, or (at your option) any later version.  This library is
 * distributed in the hope that it will be useful, but WITHOUT ANY
 * WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Library General Public
 * License for more details.  You should have received a copy of the GNU
 * Library General Public License along with this library; if not, write
 * to the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139,
 * USA.
 *****************************************************************************
 * original Copyright:
 * -------------------
 * Copyright (C) 1988 Free Software Foundation
 *    written by Dirk Grunwald (grunwald@cs.uiuc.edu)
 * 
 * This file is part of the GNU C++ Library.  This library is free
 * software; you can redistribute it and/or modify it under the terms of
 * the GNU Library General Public License as published by the Free
 * Software Foundation; either version 2 of the License, or (at your
 * option) any later version.  This library is distributed in the hope
 * that it will be useful, but WITHOUT ANY WARRANTY; without even the
 * implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
 * PURPOSE.  See the GNU Library General Public License for more details.
 * You should have received a copy of the GNU Library General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 *****************************************************************************/
#endregion

using System;

namespace Altaxo.Calc.Probability
{
	/// <summary>
	/// Provides generation of poisson distributed random numbers.
	/// </summary>
	/// <remarks>
	/// The poisson distribution generates only discrete numbers.<br />
    /// The implementation of the <see cref="PoissonDistribution"/> type bases upon information presented on
    ///   <a href="http://en.wikipedia.org/wiki/Poisson_distribution">Wikipedia - Poisson distribution</a>
    ///   and the implementation in the <a href="http://www.lkn.ei.tum.de/lehre/scn/cncl/doc/html/cncl_toc.html">
    ///   Communication Networks Class Library</a>.
    /// </remarks>
  public class PoissonDistribution : DiscreteDistribution
  {
    #region instance fields
    /// <summary>
    /// Gets or sets the parameter lambda which is used for generation of poisson distributed random numbers.
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
    /// Stores the the parameter lambda which is used for generation of poisson distributed random numbers.
    /// </summary>
    private double lambda;

    /// <summary>
    /// Stores an intermediate result for generation of poisson distributed random numbers.
    /// </summary>
    /// <remarks>
    /// Speeds up random number generation cause this value only depends on distribution parameters 
    ///   and therefor doesn't need to be recalculated in successive executions of <see cref="NextDouble"/>.
    /// </remarks>
    private double helper1;
    #endregion

    #region construction
    /// <summary>
    /// Initializes a new instance of the <see cref="PoissonDistribution"/> class, using a 
    ///   <see cref="StandardGenerator"/> as underlying random number generator.
    /// </summary>
    public PoissonDistribution()
      : this(new StandardGenerator())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PoissonDistribution"/> class, using the specified 
    ///   <see cref="Generator"/> as underlying random number generator.
    /// </summary>
    /// <param name="generator">A <see cref="Generator"/> object.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="generator"/> is NULL (<see langword="Nothing"/> in Visual Basic).
    /// </exception>
    public PoissonDistribution(Generator generator)
      : base(generator)
    {
      this.lambda = 1.0;
      this.UpdateHelpers();
    }
    #endregion

    #region instance methods
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
    /// Updates the helper variables that store intermediate results for generation of beta distributed random 
    ///   numbers.
    /// </summary>
    private void UpdateHelpers()
    {
      this.helper1 = Math.Exp(-1.0 * this.lambda);
    }

    /// <summary>
    /// Returns a poisson distributed random number.
    /// </summary>
    /// <returns>A poisson distributed 32-bit signed integer.</returns>
    public int Next()
    {
      int count = 0;
      for (double product = this.Generator.NextDouble(); product >= this.helper1; product *= this.Generator.NextDouble())
      {
        count++;
      }

      return count;
    }
    #endregion

    #region overridden Distribution members
    /// <summary>
    /// Gets the minimum possible value of poisson distributed random numbers.
    /// </summary>
    public override double Minimum
    {
      get
      {
        return 0.0;
      }
    }

    /// <summary>
    /// Gets the maximum possible value of poisson distributed random numbers.
    /// </summary>
    public override double Maximum
    {
      get
      {
        return double.MaxValue;
      }
    }

    /// <summary>
    /// Gets the mean value of poisson distributed random numbers. 
    /// </summary>
    public override double Mean
    {
      get
      {
        return this.lambda;
      }
    }

    /// <summary>
    /// Gets the median of poisson distributed random numbers.
    /// </summary>
    public override double Median
    {
      get
      {
        return double.NaN;
      }
    }

    /// <summary>
    /// Gets the variance of poisson distributed random numbers.
    /// </summary>
    public override double Variance
    {
      get
      {
        return this.lambda;
      }
    }

    /// <summary>
    /// Gets the mode of poisson distributed random numbers. 
    /// </summary>
    public override double[] Mode
    {
      get
      {
        // Check if the value of lambda is a whole number.
        if (this.lambda == Math.Floor(this.lambda))
        {
          return new double[] { this.lambda - 1.0, this.lambda };
        }
        else
        {
          return new double[] { Math.Floor(this.lambda) };
        }
      }
    }

    /// <summary>
    /// Returns a poisson distributed floating point random number.
    /// </summary>
    /// <returns>A poisson distributed double-precision floating point number.</returns>
    public override double NextDouble()
    {
      double count = 0.0;
      for (double product = this.Generator.NextDouble(); product >= this.helper1; product *= this.Generator.NextDouble())
      {
        count++;
      }

      return count;
    }
    #endregion

    #region CdfPdf
    public override double CDF(double x)
    {
      return CDF(x, lambda);
    }
    public static double CDF(double x, double m)
    {
      return Calc.GammaRelated.GammaRegularized(1 + Math.Floor(x), m);
    }

    public override double PDF(double x)
    {
      return PDF(x, lambda);
    }
    public static double PDF(double x, double m)
    {
      return Math.Exp(-m + x * Math.Log(m) - Calc.GammaRelated.LnGamma(x + 1));
    }


    #endregion
  }
}