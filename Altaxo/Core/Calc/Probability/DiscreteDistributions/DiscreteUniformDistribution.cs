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
 * DiscreteUniformDistribution.cs, 21.09.2006
 *
 * 09.08.2006: Initial version
 * 21.09.2006: Adapted to change in base class (field "generator" declared private (formerly protected)
 *               and made accessible through new protected property "Generator")
 *
 */

#endregion Further copyright(s)

using System;

namespace Altaxo.Calc.Probability
{
  /// <summary>
  /// Provides generation of discrete uniformly distributed random numbers.
  /// </summary>
  /// <remarks>
  /// The discrete uniform distribution generates only discrete numbers.<br />
  /// The implementation of the <see cref="DiscreteUniformDistribution"/> type bases upon information presented on
  ///   <a href="http://en.wikipedia.org/wiki/Uniform_distribution_%28discrete%29">
  ///   Wikipedia - Uniform distribution (discrete)</a>.
  /// </remarks>
  public class DiscreteUniformDistribution : DiscreteDistribution
  {
    #region instance fields

    /// <summary>
    /// Gets or sets the parameter lower bound which is used for generation of uniformly distributed random numbers.
    /// </summary>
    public int LowerBound
    {
      get
      {
        return _lowerBound;
      }
    }

    /// <summary>
    /// Stores the parameter lower bound which is used for generation of uniformly distributed random numbers.
    /// </summary>
    private int _lowerBound;

    /// <summary>
    /// Gets or sets the upper boundary which is used for generation of uniformly distributed random numbers.
    /// </summary>
    public int UpperBound
    {
      get
      {
        return _upperBound;
      }
    }

    /// <summary>
    /// Stores the parameter beta which is used for generation of uniformly distributed random numbers.
    /// </summary>
    private int _upperBound;

    /// <summary>
    /// Stores an intermediate result for generation of uniformly distributed random numbers.
    /// </summary>
    /// <remarks>
    /// Speeds up random number generation cause this value only depends on distribution parameters
    ///   and therefor doesn't need to be recalculated in successive executions of <see cref="NextDouble"/>.
    /// </remarks>
    private int _upperPlus1;

    #endregion instance fields

    #region construction

    public DiscreteUniformDistribution()
      : this(DefaultGenerator)
    {
    }

    public DiscreteUniformDistribution(Generator gen)
      : this(0, 1, gen)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscreteUniformDistribution"/> class, using a
    ///   <see cref="StandardGenerator"/> as underlying random number generator.
    /// </summary>
    public DiscreteUniformDistribution(int lower, int upper)
      : this(lower, upper, DefaultGenerator)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscreteUniformDistribution"/> class, using the specified
    ///   <see cref="Generator"/> as underlying random number generator.
    /// </summary>
    /// <param name="lower">Lower boundary of the distributed values.</param>
    /// <param name="upper">Upper boundary of the distributed values.</param>
    /// <param name="generator">A <see cref="Generator"/> object.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="generator"/> is NULL (<see langword="Nothing"/> in Visual Basic).
    /// </exception>
    public DiscreteUniformDistribution(int lower, int upper, Generator generator)
      : base(generator)
    {
      Initialize(lower, upper);
    }

    #endregion construction

    #region instance methods

    /// <summary>
    /// Determines whether the specified value is valid for parameter <see cref="LowerBound"/>.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>
    /// <see langword="true"/> if value is less than or equal to <see cref="UpperBound"/>; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsValidLowerBound(int value)
    {
      return (value <= _upperBound);
    }

    /// <summary>
    /// Determines whether the specified value is valid for parameter <see cref="UpperBound"/>.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>
    /// <see langword="true"/> if value is greater than or equal to <see cref="LowerBound"/>, and less than
    ///   <see cref="int.MaxValue"/>; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsValidUpperBound(int value)
    {
      return (value >= _lowerBound && value < int.MaxValue);
    }

    /// <summary>
    /// Updates the helper variables that store intermediate results for generation of uniformly distributed random
    ///   numbers.
    /// </summary>
    private void Initialize(int lower, int upper)
    {
      if (upper < lower)
        throw new ArgumentException("Upper bound must be >= lower bound");
      if (upper == int.MaxValue)
        throw new ArgumentOutOfRangeException("Upper bound must be < int.MaxValue");

      _lowerBound = lower;
      _upperBound = upper;
      _upperPlus1 = _upperBound + 1;
    }

    /// <summary>
    /// Returns a uniformly distributed random number.
    /// </summary>
    /// <returns>A geometric distributed 32-bit signed integer.</returns>
    public int Next()
    {
      return Generator.Next(_lowerBound, _upperPlus1);
    }

    #endregion instance methods

    #region overridden Distribution members

    /// <summary>
    /// Gets the minimum possible value of uniformly distributed random numbers.
    /// </summary>
    public override double Minimum
    {
      get
      {
        return _lowerBound;
      }
    }

    /// <summary>
    /// Gets the maximum possible value of uniformly distributed random numbers.
    /// </summary>
    public override double Maximum
    {
      get
      {
        return _upperBound;
      }
    }

    /// <summary>
    /// Gets the mean value of the uniformly distributed random numbers.
    /// </summary>
    public override double Mean
    {
      get
      {
        return _lowerBound / 2.0 + _upperBound / 2.0;
      }
    }

    /// <summary>
    /// Gets the median of uniformly distributed random numbers.
    /// </summary>
    public override double Median
    {
      get
      {
        return _lowerBound / 2.0 + _upperBound / 2.0;
      }
    }

    /// <summary>
    /// Gets the variance of uniformly distributed random numbers.
    /// </summary>
    public override double Variance
    {
      get
      {
        return (Math.Pow(_upperBound - _lowerBound + 1.0, 2.0) - 1.0) / 12.0;
      }
    }

    /// <summary>
    /// Gets the mode of the uniformly distributed random numbers.
    /// </summary>
    public override double[] Mode
    {
      get
      {
        return new double[] { };
      }
    }

    /// <summary>
    /// Returns a uniformly distributed floating point random number.
    /// </summary>
    /// <returns>A uniformly distributed double-precision floating point number.</returns>
    public override double NextDouble()
    {
      return Generator.Next(_lowerBound, _upperPlus1);
    }

    #endregion overridden Distribution members

    #region CdfPdf

    public override double CDF(double x)
    {
      return CDF(x, LowerBound, UpperBound);
    }

    public static double CDF(double x, double low, double high)
    {
      double xi = Math.Floor(x);
      if (xi >= low && xi <= high)
        return (xi + 1 - low) / (high + 1 - low);
      else if (x >= 1)
        return 1;
      else
        return 0;
    }

    public override double PDF(double x)
    {
      return PDF(x, LowerBound, UpperBound);
    }

    public static double PDF(double x, double low, double high)
    {
      double xi = Math.Ceiling(x);
      if (xi == x && xi >= low && xi <= high)
        return 1.0 / (high + 1 - low);
      else
        return 0;
    }

    #endregion CdfPdf
  }
}
