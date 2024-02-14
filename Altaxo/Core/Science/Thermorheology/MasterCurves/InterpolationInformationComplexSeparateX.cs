#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using Altaxo.Calc;
using Complex64 = System.Numerics.Complex;

namespace Altaxo.Science.Thermorheology.MasterCurves
{
  /// <summary>
  /// Stores information about an interpolation curve that interpolates the resulting shift curve for one group of columns, e.g. for the real part of measured values.
  /// </summary>
  public class InterpolationInformationComplexSeparateX : InterpolationInformationBase<double>
  {
    /// <summary>
    /// Gets the current interpolation function. The argument of the function is the x-value. The result is the interpolated y-value.
    /// </summary>
    public Func<double, Complex64> InterpolationFunction { get; set; }

    /// <summary>List of all x values of the points that are used for the interpolation.</summary>
    public IReadOnlyList<double> XValuesImaginary { get { return new WrapperIListToIRoVectorK<double>(ValuesImaginaryToInterpolate.Keys); } }


    /// <summary>List of all y values of the points that are used for the interpolation.</summary>
    public IReadOnlyList<double> YValuesImaginary { get { return new WrapperIListToIRoVectorV<double>(ValuesImaginaryToInterpolate.Values); } }

    /// <summary>
    /// List of all points used for the interpolation, sorted by the x values.
    /// Keys are the x-Values, values are the y-values, and the index of the curve the y-value belongs to.
    /// </summary>
    protected SortedList<double, (double yImaginary, int indexOfCurve)> ValuesImaginaryToInterpolate { get; }

    /// <summary>
    /// Initialized the instance.
    /// </summary>
    public InterpolationInformationComplexSeparateX()
    {
      ValuesImaginaryToInterpolate = new SortedList<double, (double, int)>();
      InterpolationFunction = new Func<double, Complex64>((x) => throw new InvalidOperationException("Interpolation was not yet done."));
    }

    /// <summary>
    /// Clears this instance.
    /// </summary>
    public override void Clear()
    {
      base.Clear();
      ValuesImaginaryToInterpolate.Clear();
      InterpolationFunction = new Func<double, Complex64>((x) => throw new InvalidOperationException("Interpolation was not yet done."));
    }

    /// <summary>
    /// Adds values to the data that should be interpolated, but does not evaluate a new interpolation (call <see cref="Interpolate(ShiftGroupDouble)"/>
    /// after this call if a new interpolation should be evaluated).
    /// </summary>
    /// <param name="shift">Shift value used to modify the x values.</param>
    /// <param name="indexOfCurve">Index of the curve in the group of curves.</param>
    /// <param name="x">Column of x values.</param>
    /// <param name="y">Column of y values.</param>
    /// <param name="groupNumber">Number of the curve group.</param>
    /// <param name="options">Options for creating the master curve.</param>
    public void AddXYColumn(double shift, int indexOfCurve, IReadOnlyList<double> x, IReadOnlyList<double> y, int groupNumber, ShiftGroupBase options)
    {
      var valuesToInterpolate = groupNumber switch
      {
        0 => ValuesToInterpolate,
        1 => ValuesImaginaryToInterpolate,
        _ => throw new NotImplementedException()
      };

      // first, Remove all points with indexOfCurve
      for (int i = valuesToInterpolate.Count - 1; i >= 0; --i)
        if (valuesToInterpolate.Values[i].indexOfCurve == indexOfCurve)
          valuesToInterpolate.RemoveAt(i);

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

          for (; valuesToInterpolate.Keys.Contains(xv);)
          {
            if (xv == 0)
              xv = DoubleConstants.SmallestPositiveValue / DoubleConstants.DBL_EPSILON;
            else
              xv *= (1 + 2 * DoubleConstants.DBL_EPSILON);
          }
          valuesToInterpolate.Add(xv, (yv, indexOfCurve));
          j++;
        }
      }

      // now built a new Interpolation
      InterpolationMaximumX = maxX;
      InterpolationMinimumX = minX;
    }
  }
}


