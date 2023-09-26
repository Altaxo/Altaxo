#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
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

#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using Altaxo.Calc;

namespace Altaxo.Science.Thermorheology.MasterCurves
{
  /// <summary>
  /// Stores information about an interpolation curve that interpolates the resulting shift curve for one group of columns, e.g. for the real part of measured values.
  /// </summary>
  public class InterpolationInformation
  {
    /// <summary>
    /// Gets the current interpolation function. The argument of the function is the x-value. The result is the interpolated y-value.
    /// </summary>
    public Func<double, double> InterpolationFunction { get; set; }

    /// <summary>Minimum x value of the points used for interpolation.</summary>
    public double InterpolationMinimumX { get; private set; }

    /// <summary>Maximum x value of the points used for interpolation.</summary>
    public double InterpolationMaximumX { get; private set; }

    /// <summary>List of all x values of the points that are used for the interpolation.</summary>
    public IReadOnlyList<double> XValues { get { return new WrapperIListToIRoVectorK(ValuesToInterpolate.Keys); } }

    /// <summary>List of all y values of the points that are used for the interpolation.</summary>
    public IReadOnlyList<double> YValues { get { return new WrapperIListToIRoVectorV(ValuesToInterpolate.Values); } }

    /// <summary>
    /// List of all points used for the interpolation, sorted by the x values.
    /// Keys are the x-Values, values are the y-values, and the index of the curve the y-value belongs to.
    /// </summary>
    private SortedList<double, (double y, int indexOfCurve)> ValuesToInterpolate { get; }

    /// <summary>
    /// Initialized the instance.
    /// </summary>
    public InterpolationInformation()
    {
      InterpolationFunction = new Func<double, double>((x) => throw new InvalidOperationException("Interpolation was not yet done."));
      InterpolationMinimumX = double.MaxValue;
      InterpolationMaximumX = double.MinValue;
      ValuesToInterpolate = new SortedList<double, (double, int)>();
    }

    /// <summary>
    /// Clears this instance.
    /// </summary>
    public void Clear()
    {
      InterpolationFunction = new Func<double, double>((x) => throw new InvalidOperationException("Interpolation was not yet done."));
      InterpolationMinimumX = double.MaxValue;
      InterpolationMaximumX = double.MinValue;
      ValuesToInterpolate.Clear();
    }

    /// <summary>
    /// Adds values to the data that should be interpolated, but does not evaluate a new interpolation (call <see cref="Interpolate(MasterCurveCreationOptions)"/>
    /// after this call if a new interpolation should be evaluated).
    /// </summary>
    /// <param name="shift">Shift value used to modify the x values.</param>
    /// <param name="indexOfCurve">Index of the curve in the group of curves.</param>
    /// <param name="x">Column of x values.</param>
    /// <param name="y">Column of y values.</param>
    /// <param name="options">Options for creating the master curve.</param>
    public void AddXYColumn(double shift, int indexOfCurve, IReadOnlyList<double> x, IReadOnlyList<double> y, MasterCurveCreationOptions options)
    {
      // first, Remove all points with indexOfCurve
      for (int i = ValuesToInterpolate.Count - 1; i >= 0; --i)
        if (ValuesToInterpolate.Values[i].indexOfCurve == indexOfCurve)
          ValuesToInterpolate.RemoveAt(i);

      // now add the new values
      int count = Math.Min(x.Count, y.Count);

      bool doLogX = options.LogarithmizeXForInterpolation;
      bool doLogY = options.LogarithmizeYForInterpolation;
      bool shiftXByOffset = options.XShiftBy == ShiftXBy.Offset;
      double minX = InterpolationMinimumX;
      double maxX = InterpolationMaximumX;

      double interpolMin = InterpolationMinimumX;
      double interpolMax = InterpolationMaximumX;

      for (int i = 0, j = 0; i < count; i++)
      {
        bool isXUsable = shiftXByOffset || x[i] > 0; // if shift by factor, x must be >0
        double xv;
        if (doLogX)
          xv = shiftXByOffset ? Math.Log(x[i] + shift) : Math.Log(x[i]) + shift;
        else
          xv = shiftXByOffset ? x[i] + shift : x[i] * Math.Exp(shift);

        double yv = y[i];
        if (doLogY)
          yv = Math.Log(yv);


        if (isXUsable && xv.IsFinite() && yv.IsFinite())
        {
          maxX = Math.Max(maxX, xv);
          minX = Math.Min(minX, xv);

          for (; ValuesToInterpolate.Keys.Contains(xv);)
          {
            if (xv == 0)
              xv = DoubleConstants.SmallestPositiveValue / DoubleConstants.DBL_EPSILON;
            else
              xv *= (1 + 2 * DoubleConstants.DBL_EPSILON);
          }
          ValuesToInterpolate.Add(xv, (yv, indexOfCurve));
          j++;
        }
      }

      // now built a new Interpolation
      InterpolationMaximumX = maxX;
      InterpolationMinimumX = minX;
    }

    #region Wrapper classes

    /// <summary>
    /// Wraps an IList of double values to an IROVector
    /// </summary>
    private class WrapperIListToIRoVectorK : IReadOnlyList<double>
    {
      private IList<double> _list;

      public WrapperIListToIRoVectorK(IList<double> list)
      {
        _list = list;
      }

      #region IROVector Members

      public int Count
      {
        get { return _list.Count; }
      }

      #endregion IROVector Members

      #region INumericSequence Members

      public double this[int i]
      {
        get { return _list[i]; }
      }

      public IEnumerator<double> GetEnumerator()
      {
        var len = Count;
        for (int i = 0; i < len; ++i)
          yield return this[i];
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        var len = Count;
        for (int i = 0; i < len; ++i)
          yield return this[i];
      }

      #endregion INumericSequence Members
    }

    /// <summary>
    /// Wraps an IList of double values to an IROVector
    /// </summary>
    private class WrapperIListToIRoVectorV : IReadOnlyList<double>
    {
      private IList<(double, int)> _list;

      public WrapperIListToIRoVectorV(IList<(double, int)> list)
      {
        _list = list;
      }

      #region IROVector Members

      public int Count
      {
        get { return _list.Count; }
      }

      #endregion IROVector Members

      #region INumericSequence Members

      public double this[int i]
      {
        get { return _list[i].Item1; }
      }

      public IEnumerator<double> GetEnumerator()
      {
        var len = Count;
        for (int i = 0; i < len; ++i)
          yield return this[i];
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        var len = Count;
        for (int i = 0; i < len; ++i)
          yield return this[i];
      }

      #endregion INumericSequence Members
    }

    #endregion Wrapper class
  }
}


