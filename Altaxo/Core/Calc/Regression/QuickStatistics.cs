﻿#region Copyright

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
  /// Uses the method of running sums to calculate mean, sample standard deviation, and standard deviation of a data set.
  /// </summary>
  /// <remarks>See wikipedia article "Standard deviation".</remarks>
  public class QuickStatistics
  {
    private long _n;
    private double _ai;
    private double _qi;
    private double _min = double.PositiveInfinity;
    private double _max = double.NegativeInfinity;

    /// <summary>
    /// Initializes the statistics.
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
    /// Adds data points to the statistics.
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
    /// Adds data points to the statistics.
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
    /// Returns the mean (average) of the data values.
    /// </summary>
    public double Mean
    {
      get
      {
        return _n > 0 ? _ai : double.NaN;
      }
    }

    /// <summary>
    /// Returns the minimum of the data values.
    /// </summary>
    public double Min
    {
      get
      {
        return _n > 0 ? _min : double.NaN;
      }
    }

    /// <summary>
    /// Returns the maximum of the data values.
    /// </summary>
    public double Max
    {
      get
      {
        return _n > 0 ? _max : double.NaN;
      }
    }

    /// <summary>
    /// Returns the number of data values that were added.
    /// </summary>
    public double N
    {
      get
      {
        return _n;
      }
    }

    /// <summary>
    /// Returns the sample standard deviation: square root of the error sum of squares divided by (N-1).
    /// </summary>
    public double SampleStandardDeviation
    {
      get
      {
        return _n > 1 ? Math.Sqrt(_qi / (_n - 1)) : double.NaN;
      }
    }

    /// <summary>
    /// Returns the standard deviation: square root of the error sum of squares divided by N.
    /// </summary>
    public double StandardDeviation
    {
      get
      {
        return _n > 0 ? Math.Sqrt(_qi / (_n)) : double.NaN;
      }
    }

    /// <summary>
    /// Returns the variance: error sum of squares divided by N.
    /// </summary>
    public double Variance
    {
      get
      {
        return _n > 0 ? _qi / (_n) : double.NaN;
      }
    }
  }
}
