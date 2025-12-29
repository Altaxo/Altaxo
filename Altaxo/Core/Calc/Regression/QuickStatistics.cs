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

using System;
using System.Collections.Generic;

namespace Altaxo.Calc.Regression
{
  /// <summary>
  /// Uses the method of running sums to calculate mean, sample standard deviation, and population standard deviation of a data set.
  /// </summary>
  /// <remarks>See the Wikipedia article "Standard deviation".</remarks>
  public class QuickStatistics
  {
    private long _n;
    private double _ai;
    private double _qi;
    private double _min = double.PositiveInfinity;
    private double _max = double.NegativeInfinity;

    /// <summary>
    /// Resets the statistics to their initial state.
    /// </summary>
    public void Clear()
    {
      _n = 0;
      _ai = 0;
      _qi = 0;
      _min = double.PositiveInfinity;
      _max = double.NegativeInfinity;
    }

    /// <summary>
    /// Adds a data point to the statistics.
    /// </summary>
    /// <param name="x">The value of the data point.</param>
    public void Add(double x)
    {
      _n++;
      double aim1 = _ai;
      double qim1 = _qi;
      _ai = aim1 + (x - aim1) / _n;
      _qi = qim1 + (x - aim1) * (x - _ai);
      if (_min > x)
        _min = x;
      if (_max < x)
        _max = x;
    }

    /// <summary>
    /// Adds a sequence of data points to the statistics.
    /// </summary>
    /// <param name="values">The data points to add.</param>
    /// <returns>This instance.</returns>
    public QuickStatistics AddRange(IEnumerable<double> values)
    {
      foreach (var value in values)
      {
        Add(value);
      }
      return this;
    }

    /// <summary>
    /// Adds a span of data points to the statistics.
    /// </summary>
    /// <param name="values">The data points to add.</param>
    /// <returns>This instance.</returns>
    public QuickStatistics AddRange(ReadOnlySpan<double> values)
    {
      foreach (var value in values)
      {
        Add(value);
      }
      return this;
    }

    /// <summary>
    /// Gets the arithmetic mean (average) of the data values.
    /// </summary>
    /// <value>
    /// The mean of the added values, or <see cref="double.NaN"/> if no values were added.
    /// </value>
    public double Mean
    {
      get
      {
        return _n > 0 ? _ai : double.NaN;
      }
    }

    /// <summary>
    /// Gets the minimum of the data values.
    /// </summary>
    /// <value>
    /// The minimum of the added values, or <see cref="double.NaN"/> if no values were added.
    /// </value>
    public double Min
    {
      get
      {
        return _n > 0 ? _min : double.NaN;
      }
    }

    /// <summary>
    /// Gets the maximum of the data values.
    /// </summary>
    /// <value>
    /// The maximum of the added values, or <see cref="double.NaN"/> if no values were added.
    /// </value>
    public double Max
    {
      get
      {
        return _n > 0 ? _max : double.NaN;
      }
    }

    /// <summary>
    /// Gets the number of data values that were added.
    /// </summary>
    public double N
    {
      get
      {
        return _n;
      }
    }

    /// <summary>
    /// Gets the sample standard deviation: square root of the error sum of squares divided by (N-1).
    /// </summary>
    public double StandardDeviation => SampleStandardDeviation;

    /// <summary>
    /// Gets the sample standard deviation: square root of the error sum of squares divided by (N-1).
    /// </summary>
    /// <value>
    /// The sample standard deviation, or <see cref="double.NaN"/> if fewer than two values were added.
    /// </value>
    public double SampleStandardDeviation
    {
      get
      {
        return _n > 1 ? Math.Sqrt(_qi / (_n - 1)) : double.NaN;
      }
    }

    /// <summary>
    /// Gets the population standard deviation: square root of the error sum of squares divided by N.
    /// </summary>
    /// <value>
    /// The population standard deviation, or <see cref="double.NaN"/> if no values were added.
    /// </value>
    public double PopulationStandardDeviation
    {
      get
      {
        return _n > 0 ? Math.Sqrt(_qi / (_n)) : double.NaN;
      }
    }

    /// <summary>
    /// Gets the sample variance: error sum of squares divided by N-1.
    /// </summary>
    public double Variance => SampleVariance;

    /// <summary>
    /// Gets the sample variance: error sum of squares divided by N-1.
    /// </summary>
    /// <value>
    /// The sample variance, or <see cref="double.NaN"/> if fewer than two values were added.
    /// </value>
    public double SampleVariance
    {
      get
      {
        return _n > 1 ? _qi / (_n - 1) : double.NaN;
      }
    }

    /// <summary>
    /// Gets the population variance: error sum of squares divided by N.
    /// </summary>
    /// <value>
    /// The population variance, or <see cref="double.NaN"/> if no values were added.
    /// </value>
    public double PopulationVariance
    {
      get
      {
        return _n > 0 ? _qi / (_n) : double.NaN;
      }
    }
  }
}
