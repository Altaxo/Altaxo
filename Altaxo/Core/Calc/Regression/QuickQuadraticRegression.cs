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
  /// Class for doing a quick and dirty regression of order 2 only returning the parameters A0, A1 and A2 as regression parameters.
  /// Can not handle too big or too small input values.
  /// </summary>
  public class QuickQuadraticRegression
  {
    private double _n;
    private double _sx;
    private double _sxx;
    private double _sxxx;
    private double _sxxxx;
    private double _sy;
    private double _syx;
    private double _syxx;

    /// <summary>
    /// Adds a data point to the regression.
    /// </summary>
    /// <param name="x">The x value of the data point.</param>
    /// <param name="y">The y value of the data point.</param>
    public void Add(double x, double y)
    {
      _n += 1;
      _sx += x;
      _sxx += x * x;
      _sxxx += (x * x) * x;
      _sxxxx += (x * x) * (x * x);
      _sy += y;
      _syx += y * x;
      _syxx += y * x * x;
    }

    /// <summary>
    /// Adds data points to the statistics.
    /// </summary>
    /// <param name="values">The data points to add.</param>
    public void AddRange(IEnumerable<(double x, double y)> values)
    {
      foreach (var (x, y) in values)
      {
        Add(x, y);
      }
    }

    /// <summary>
    /// Gets the intercept value of the regression. Returns NaN if not enough data points entered.
    /// </summary>
    /// <returns>The intercept value or NaN if not enough data points are entered.</returns>
    public double GetA0()
    {
      return (-_sxxx * _sxxx * _sy + _sxx * _sxxxx * _sy + _sxx * _sxxx * _syx - _sx * _sxxxx * _syx - _sxx * _sxx * _syxx + _sx * _sxxx * _syxx) / GetDeterminant();
    }

    /// <summary>
    /// Gets the slope value of the regression. Returns NaN if not enough data points entered.
    /// </summary>
    /// <returns>The slope value or NaN if not enough data points are entered.</returns>
    public double GetA1()
    {
      return (_sxx * _sxxx * _sy - _sx * _sxxxx * _sy - _sxx * _sxx * _syx + _n * _sxxxx * _syx + _sx * _sxx * _syxx - _n * _sxxx * _syxx) / GetDeterminant();
    }

    /// <summary>
    /// Gets the quadratic parameter of the regression. Returns NaN if not enough data points entered.
    /// </summary>
    /// <returns>The slope value or NaN if not enough data points are entered.</returns>
    public double GetA2()
    {
      return (-_sxx * _sxx * _sy + _sx * _sxxx * _sy + _sx * _sxx * _syx - _n * _sxxx * _syx - _sx * _sx * _syxx + _n * _sxx * _syxx) / GetDeterminant();
    }

    /// <summary>
    /// Returns the determinant of regression. If zero, not enough data points have been entered.
    /// </summary>
    /// <returns>The determinant of the regression.</returns>
    public double GetDeterminant()
    {
      return _n * _sxx * _sxxxx - _sxx * _sxx * _sxx + 2 * _sx * _sxx * _sxxx - _n * _sxxx * _sxxx - _sx * _sx * _sxxxx;
    }

    /// <summary>
    /// Gets the y value for a given x value. Note that in every call of this function the polynomial coefficients a0, a2, and a1 are calculated again.
    /// For repeated calls, better use <see cref="GetYOfXFunction"/>, but note that this function represents the state of the regression at the time of this call.
    /// </summary>
    /// <param name="x">The x value.</param>
    /// <returns>The y value at the value x.</returns>
    public double GetYOfX(double x)
    {
      return (GetA2() * x + GetA1()) * x + GetA0();
    }

    /// <summary>
    /// Returns a function to calculate y in dependence of x. Please note note that the returned function represents the state of the regression at the time of the call, i.e. subsequent additions of data does not change the function.
    /// </summary>
    /// <returns>A function to calculate y in dependence of x.</returns>
    public Func<double, double> GetYOfXFunction()
    {
      var a0 = GetA0();
      var a1 = GetA1();
      var a2 = GetA2();
      return new Func<double, double>(x => ((a2 * x) + a1) * x + a0);
    }
  }
}
