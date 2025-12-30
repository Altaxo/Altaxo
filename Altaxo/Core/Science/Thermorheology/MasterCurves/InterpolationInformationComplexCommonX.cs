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
  /// This specialization stores complex-valued y data where real and imaginary parts share a common x-axis.
  /// </summary>
  public class InterpolationInformationComplexCommonX : InterpolationInformationBase<Complex64>
  {
    /// <summary>
    /// Gets or sets the current interpolation function. The argument of the function is the x-value. The result is the interpolated complex y-value.
    /// The function should be set after building the interpolation; before that it throws an <see cref="InvalidOperationException"/>.
    /// </summary>
    public Func<double, Complex64> InterpolationFunction { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="InterpolationInformationComplexCommonX"/> class.
    /// The <see cref="InterpolationFunction"/> is initialized to a stub that throws if called before interpolation is performed.
    /// </summary>
    public InterpolationInformationComplexCommonX()
    {
      InterpolationFunction = new Func<double, Complex64>((x) => throw new InvalidOperationException("Interpolation was not yet done."));
    }

    /// <inheritdoc/>
    public override void Clear()
    {
      base.Clear();
      InterpolationFunction = new Func<double, Complex64>((x) => throw new InvalidOperationException("Interpolation was not yet done."));
    }

    /// <summary>
    /// Adds values to the data that should be interpolated, but does not evaluate a new interpolation.
    /// Existing points belonging to the specified curve index are removed before adding the new column.
    /// The method updates the tracked minimum and maximum x values used for interpolation.
    /// </summary>
    /// <param name="shift">Shift value used to modify the x values.</param>
    /// <param name="indexOfCurve">Index of the curve in the group of curves.</param>
    /// <param name="x">Column of x values.</param>
    /// <param name="y">Column of complex y values.</param>
    /// <param name="options">Options for creating the master curve (controls logarithmization and shift mode).</param>
    public void AddXYColumn(double shift, int indexOfCurve, IReadOnlyList<double> x, IReadOnlyList<Complex64> y, ShiftGroupComplexCommonX options)
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

      for (int i = 0, j = 0; i < count; i++)
      {
        bool isXUsable = shiftXByOffset || x[i] > 0; // if shift by factor, x must be >0
        double xv;
        if (doLogX)
          xv = shiftXByOffset ? Math.Log(x[i] + shift) : Math.Log(x[i]) + shift;
        else
          xv = shiftXByOffset ? x[i] + shift : x[i] * Math.Exp(shift);

        var yv = y[i];
        if (doLogY)
          yv = new Complex64(Math.Log(yv.Real), Math.Log(yv.Imaginary));

        if (isXUsable && xv.IsFinite() && yv.Real.IsFinite() && yv.Imaginary.IsFinite())
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
  }
}


