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

using Altaxo.Calc.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Calc.Interpolation
{
  public class BivariateLinearSpline
  {
    #region Member variables

    private IReadOnlyList<double> _x;
    private IReadOnlyList<double> _y;
    private IROMatrix<double> _vmatrix;

    private bool _isXDecreasing;
    private bool _isYDecreasing;

    private int _lastIX;
    private int _lastIY;

    #endregion Member variables

    /// <summary>
    /// Constructor of a bivariate linear spline. The vectors and the data matrix are not cloned, so make sure that they don't change during usage of this instance.
    /// </summary>
    /// <param name="x">Vector of x values corresponding to the rows of the data matrix. Must be strongly increasing or decreasing.</param>
    /// <param name="y">Vector of y values corresponding to the columns of the data matrix. Must be strongly increasing or decreasing.</param>
    /// <param name="datamatrix"></param>
    public BivariateLinearSpline(IReadOnlyList<double> x, IReadOnlyList<double> y, IROMatrix<double> datamatrix)
    {
      _x = x;
      _y = y;
      _vmatrix = datamatrix;

      // check the arguments
      if (_x.Count < 2)
        throw new ArgumentException("x.Length is less or equal 1 (you can use univariate interpolation instead)");
      if (_y.Count < 2)
        throw new ArgumentException("y.Length is less or equal 1 (you can use univariate interpolation instead)");
      if (_x.Count != _vmatrix.RowCount)
        throw new ArgumentException("Length of vector x is not equal to datamatrix.Rows");
      if (_y.Count != _vmatrix.ColumnCount)
        throw new ArgumentException("Length of vector y is not equal to datamatrix.Columns");

      if (!VectorMath.IsStrictlyIncreasingOrDecreasing(_x, out _isXDecreasing))
        throw new ArgumentException("Vector x is not strictly increasing or decreasing");

      if (!VectorMath.IsStrictlyIncreasingOrDecreasing(_y, out _isYDecreasing))
        throw new ArgumentException("Vector y is not strictly increasing or decreasing");

      _lastIX = 0;
      _lastIY = 0;
    }

    private static int FindIndex(IReadOnlyList<double> v, bool isDecreasing, int lastIdx, double x)
    {
      if (isDecreasing) // strictly decreasing
      {
        if (x > v[lastIdx])
        {
          if (lastIdx == 0)
            return -1;
          if (x <= v[lastIdx - 1])
            return lastIdx - 1;
          return BinarySearchForIndex(v, isDecreasing, x);
        }
        else if (x < v[lastIdx + 1])
        {
          if (lastIdx + 2 <= v.Count)
            return -1;
          if (x >= v[lastIdx + 2])
            return lastIdx + 1;
          return BinarySearchForIndex(v, isDecreasing, x);
        }
        else
        {
          return lastIdx;
        }
      }
      else // strictly increasing
      {
        if (x < v[lastIdx])
        {
          if (lastIdx == 0)
            return -1;
          if (x >= v[lastIdx - 1])
            return lastIdx - 1;
          return BinarySearchForIndex(v, isDecreasing, x);
        }
        else if (x > v[lastIdx + 1])
        {
          if (lastIdx + 2 >= v.Count)
            return -1;
          if (x <= v[lastIdx + 2])
            return lastIdx + 1;
          return BinarySearchForIndex(v, isDecreasing, x);
        }
        else
        {
          return lastIdx;
        }
      }
    }

    /// <summary>
    /// Find the index idx where x is inbetween v[idx] and v[idx+1] by binary search.
    /// </summary>
    /// <param name="v"></param>
    /// <param name="isDecreasing"></param>
    /// <param name="x"></param>
    /// <returns></returns>
    private static int BinarySearchForIndex(IReadOnlyList<double> v, bool isDecreasing, double x)
    {
      int lenM1 = v.Count - 1;
      if (isDecreasing)
      {
        if (x > v[0] || x < v[lenM1])
          return -1;
        if (x == v[lenM1])
          return lenM1 - 1;

        int low = 0, high = lenM1;
        int idx = lenM1 / 2;
        for (; ; )
        {
          if (x > v[idx])
          {
            high = idx;
            idx = (low + high) / 2;
          }
          else if (x < v[idx + 1])
          {
            low = idx;
            idx = (low + high) / 2;
          }
          else
          {
            return idx;
          }
        }
      }
      else // strictly increasing
      {
        if (x < v[0] || x > v[lenM1])
          return -1;
        if (x == v[lenM1])
          return lenM1 - 1;

        int low = 0, high = lenM1;
        int idx = lenM1 / 2;
        for (; ; )
        {
          if (x < v[idx])
          {
            high = idx;
            idx = (low + high) / 2;
          }
          else if (x > v[idx + 1])
          {
            low = idx;
            idx = (low + high) / 2;
          }
          else
          {
            return idx;
          }
        }
      }
    }

    /// <summary>
    /// Interpolates the values with given x and y coordinates. Returns double.NaN if x or y are outside the bounds.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <returns>The interpolated value. Returns double.NaN if x or y is outside of the bound.</returns>
    public double Interpolate(double x, double y)
    {
      int ix = FindIndex(_x, _isXDecreasing, _lastIX, x);
      if (ix < 0)
        return double.NaN;

      int iy = FindIndex(_y, _isYDecreasing, _lastIY, y);
      if (iy < 0)
        return double.NaN;

      _lastIX = ix;
      _lastIY = iy;

      double rx = (x - _x[ix]) / (_x[ix + 1] - _x[ix]);
      double ry = (y - _y[iy]) / (_y[iy + 1] - _y[iy]);

      double v00 = _vmatrix[ix, iy];
      double a = _vmatrix[ix + 1, iy] - v00;
      double b = _vmatrix[ix, iy + 1] - v00;
      double c = _vmatrix[ix + 1, iy + 1] - a - b - v00;

      return v00 + a * rx + b * ry + c * rx * ry;
    }
  }
}
