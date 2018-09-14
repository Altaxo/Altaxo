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

#endregion original copyright

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
        return alpha;
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
        return beta;
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
        return gamma;
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

    #endregion instance fields

    #region construction

    /// <summary>
    /// Initializes a new instance of the <see cref="TriangularDistribution"/> class, using a
    ///   <see cref="StandardGenerator"/> as underlying random number generator.
    /// </summary>
    public TriangularDistribution()
      : this(DefaultGenerator)
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
      : this(0, 0.5, 1, generator)
    {
    }

    public TriangularDistribution(double alpha, double gamma, double beta)
      : this(alpha, gamma, beta, DefaultGenerator)
    {
    }

    public TriangularDistribution(double alpha, double gamma, double beta, Generator generator)
      : base(generator)
    {
      Initialize(alpha, gamma, beta);
    }

    #endregion construction

    #region instance methods

    public void Initialize(double alpha, double gamma, double beta)
    {
      if (!(alpha < beta && alpha <= gamma))
        throw new ArgumentOutOfRangeException("Alpha out of range (must be < beta and <= gamma)");
      if (!(beta > alpha && beta >= gamma))
        throw new ArgumentOutOfRangeException("Beta out of range (have to be > alpha and >= gamma)");
      if (!(gamma >= alpha && gamma <= beta))
        throw new ArgumentOutOfRangeException("Gamma out of range (have to be >= alpha and <= beta)");

      this.alpha = alpha;
      this.beta = beta;
      this.gamma = gamma;

      UpdateHelpers();
    }

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
      return (value < beta && value <= gamma);
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
      return (value > alpha && value >= gamma);
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
      return (value >= alpha && value <= beta);
    }

    /// <summary>
    /// Updates the helper variables that store intermediate results for generation of triangular distributed random
    ///   numbers.
    /// </summary>
    private void UpdateHelpers()
    {
      helper1 = gamma - alpha;
      helper2 = beta - alpha;
      helper3 = Math.Sqrt(helper1 * helper2);
      helper4 = Math.Sqrt(beta - gamma);
    }

    #endregion instance methods

    #region overridden Distribution members

    /// <summary>
    /// Gets the minimum possible value of triangular distributed random numbers.
    /// </summary>
    public override double Minimum
    {
      get
      {
        return alpha;
      }
    }

    /// <summary>
    /// Gets the maximum possible value of triangular distributed random numbers.
    /// </summary>
    public override double Maximum
    {
      get
      {
        return beta;
      }
    }

    /// <summary>
    /// Gets the mean value of triangular distributed random numbers.
    /// </summary>
    public override double Mean
    {
      get
      {
        return alpha / 3.0 + beta / 3.0 + gamma / 3.0;
      }
    }

    /// <summary>
    /// Gets the median of triangular distributed random numbers.
    /// </summary>
    public override double Median
    {
      get
      {
        if (gamma >= (beta - alpha) / 2.0)
        {
          return alpha +
              (Math.Sqrt((beta - alpha) * (gamma - alpha)) / Math.Sqrt(2.0));
        }
        else
        {
          return beta -
              (Math.Sqrt((beta - alpha) * (beta - gamma)) / Math.Sqrt(2.0));
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
        return (Math.Pow(alpha, 2.0) + Math.Pow(beta, 2.0) + Math.Pow(gamma, 2.0) -
          alpha * beta - alpha * gamma - beta * gamma) / 18.0;
      }
    }

    /// <summary>
    /// Gets the mode of triangular distributed random numbers.
    /// </summary>
    public override double[] Mode
    {
      get
      {
        return new double[] { gamma };
      }
    }

    /// <summary>
    /// Returns a triangular distributed floating point random number.
    /// </summary>
    /// <returns>A triangular distributed double-precision floating point number.</returns>
    public override double NextDouble()
    {
      double genNum = Generator.NextDouble();
      if (genNum <= helper1 / helper2)
      {
        return alpha + Math.Sqrt(genNum) * helper3;
      }
      else
      {
        return beta - Math.Sqrt(genNum * helper2 - helper1) * helper4;
      }
    }

    #endregion overridden Distribution members

    #region CdfPdfQuantile

    public override double CDF(double x)
    {
      return CDF(x, Alpha, Beta, Gamma);
    }

    public static double CDF(double x, double A, double B, double C)
    {
      if (x <= A)
        return 0;
      if (x >= B)
        return 1;
      if (x <= C)
        return (x - A) * (x - A) / ((B - A) * (C - A));
      else
        return 1 - (B - x) * (B - x) / ((B - A) * (B - C));
    }

    public override double PDF(double x)
    {
      return PDF(x, Alpha, Beta, Gamma);
    }

    public static double PDF(double x, double A, double B, double C)
    {
      if (x <= A)
        return 0;
      if (x >= B)
        return 0;
      if (x <= C)
        return 2 * (x - A) / ((B - A) * (C - A));
      else
        return 2 * (B - x) / ((B - A) * (B - C));
    }

    public override double Quantile(double p)
    {
      return Quantile(p, Alpha, Beta, Gamma);
    }

    public static double Quantile(double p, double A, double B, double C)
    {
      double x;
      if (!(p >= 0 && p <= 1))
        throw new ArgumentOutOfRangeException("p must be inbetween [0,1]");
      if (!(A < B))
        throw new ArgumentOutOfRangeException("A must be < B");
      if (!(C >= A))
        throw new ArgumentOutOfRangeException("C must be >= A");
      if (!(C <= B))
        throw new ArgumentOutOfRangeException("C must be <= B");

      x = A + Math.Sqrt(p * (A - B) * (A - C));

      if (x > C)
        x = B - Math.Sqrt((1 - p) * (B - A) * (B - C));

      return x;
    }

    #endregion CdfPdfQuantile
  }
}
