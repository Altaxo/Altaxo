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
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Interpolation
{
  /// <summary>
  /// Contains static methods for linear interpolation of data.
  /// </summary>
  public class LinearInterpolation : IInterpolationFunction
  {
    #region Members

    private Vector<double> _x = CreateVector.Dense<double>(0);
    private Vector<double> _y = CreateVector.Dense<double>(0);

    #endregion Members

    #region IInterpolationFunction Members

    public double GetYOfX(double xval)
    {
      int idx = CurveBase.FindInterval(xval, _x);
      if (idx < 0)
        throw new ArgumentOutOfRangeException("xval is smaller than the interpolation x range");
      if (idx + 1 > _x.Count)
        throw new ArgumentOutOfRangeException("xval is greater than the interpolation x range");

      if (idx + 1 == _x.Count)
        idx--;

      return Interpolate(xval, _x[idx], _x[idx + 1], _y[idx], _y[idx + 1]);
    }

    #endregion IInterpolationFunction Members

    #region IInterpolationCurve Members

    public void Interpolate(IReadOnlyList<double> xvec, IReadOnlyList<double> yvec)
    {
      if (_x is null)
        _x = CreateVector.Dense<double>(0);
      if (_y is null)
        _y = CreateVector.Dense<double>(0);

      _x = CreateVector.DenseOfEnumerable(xvec);
      _y = CreateVector.DenseOfEnumerable(yvec);
    }

    public double GetYOfU(double u)
    {
      return GetYOfX(u);
    }

    public double GetXOfU(double u)
    {
      return u;
    }

    #endregion IInterpolationCurve Members

    #region Static methods

    public static int GetNextIndexOfValidPair(IReadOnlyList<double> xcol, IReadOnlyList<double> ycol, int sourceLength, int currentIndex)
    {
      for (int sourceIndex = currentIndex; sourceIndex < sourceLength; sourceIndex++)
      {
        if (!double.IsNaN(xcol[sourceIndex]) && !double.IsNaN(ycol[sourceIndex]))
          return sourceIndex;
      }

      return -1;
    }

    public static double Interpolate(double x, double x0, double x1, double y0, double y1)
    {
      double r = (x - x0) / (x1 - x0);
      return (1 - r) * y0 + r * y1;
    }

    public static string? Interpolate(
      IReadOnlyList<double> xcol,
      IReadOnlyList<double> ycol,
      int sourceLength,
      double xstart, double xincrement, int numberOfValues,
      double yOutsideOfBounds,
      out double[] resultCol)
    {
      resultCol = new double[numberOfValues];

      int currentIndex = GetNextIndexOfValidPair(xcol, ycol, sourceLength, 0);
      if (currentIndex < 0)
        return "The two columns don't contain a valid pair of values";

      int nextIndex = GetNextIndexOfValidPair(xcol, ycol, sourceLength, currentIndex + 1);
      if (nextIndex < 0)
        return "The two columns contain only one valid pair of values, but at least two valid pairs are neccessary!";

      double x_current = xcol[currentIndex];
      double x_next = xcol[nextIndex];

      int resultIndex = 0;

      // handles values before interpolation range
      for (resultIndex = 0; resultIndex < numberOfValues; resultIndex++)
      {
        double x_result = xstart + resultIndex * xincrement;
        if (x_result >= x_current)
          break;

        resultCol[resultIndex] = yOutsideOfBounds;
      }

      // handle values in the interpolation range
      for (; resultIndex < numberOfValues; resultIndex++)
      {
        double x_result = xstart + resultIndex * xincrement;

tryinterpolation:
        if (x_result >= x_current && x_result <= x_next)
        {
          resultCol[resultIndex] = Interpolate(x_result, x_current, x_next, ycol[currentIndex], ycol[nextIndex]);
        }
        else
        {
          currentIndex = nextIndex;
          x_current = x_next;
          nextIndex = GetNextIndexOfValidPair(xcol, ycol, sourceLength, currentIndex + 1);
          if (nextIndex < 0)
            break;

          x_next = xcol[nextIndex];
          goto tryinterpolation;
        }
      }

      // handle values behind the interplation range
      for (; resultIndex < numberOfValues; resultIndex++)
      {
        resultCol[resultIndex] = yOutsideOfBounds;
      }

      return null;
    }

    public static string? Interpolate(
      IReadOnlyList<double> xcol,
      IReadOnlyList<double> ycol,
      int sourceLength,
      IReadOnlyList<double> xnewsampling, int numberOfValues,
      double yOutsideOfBounds,
      out double[] resultCol)
    {
      resultCol = new double[numberOfValues];

      int currentIndex = GetNextIndexOfValidPair(xcol, ycol, sourceLength, 0);
      if (currentIndex < 0)
        return "The two columns don't contain a valid pair of values";

      int nextIndex = GetNextIndexOfValidPair(xcol, ycol, sourceLength, currentIndex + 1);
      if (nextIndex < 0)
        return "The two columns contain only one valid pair of values, but at least two valid pairs are neccessary!";

      double x_current = xcol[currentIndex];
      double x_next = xcol[nextIndex];

      int resultIndex = 0;

      // handles values before interpolation range
      for (resultIndex = 0; resultIndex < numberOfValues; resultIndex++)
      {
        double x_result = xnewsampling[resultIndex];
        if (x_result >= x_current)
          break;

        resultCol[resultIndex] = yOutsideOfBounds;
      }

      // handle values in the interpolation range
      for (; resultIndex < numberOfValues; resultIndex++)
      {
        double x_result = xnewsampling[resultIndex];

tryinterpolation:
        if (x_result >= x_current && x_result <= x_next)
        {
          resultCol[resultIndex] = Interpolate(x_result, x_current, x_next, ycol[currentIndex], ycol[nextIndex]);
        }
        else
        {
          currentIndex = nextIndex;
          x_current = x_next;
          nextIndex = GetNextIndexOfValidPair(xcol, ycol, sourceLength, currentIndex + 1);
          if (nextIndex < 0)
            break;

          x_next = xcol[nextIndex];
          goto tryinterpolation;
        }
      }

      // handle values behind the interplation range
      for (; resultIndex < numberOfValues; resultIndex++)
      {
        resultCol[resultIndex] = yOutsideOfBounds;
      }

      return null;
    }

    #endregion Static methods
  }
}
