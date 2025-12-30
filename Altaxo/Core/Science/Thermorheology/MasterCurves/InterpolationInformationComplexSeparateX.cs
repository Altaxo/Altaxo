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
  /// Stores information about interpolation curves that are used when building master curves for complex data
  /// where the real and imaginary parts are stored in separate x-columns.
  /// This class holds the interpolation data for the real part (via the base class) and for the imaginary part
  /// using separate collections.
  /// </summary>
  public class InterpolationInformationComplexSeparateX : InterpolationInformationBase<double>
  {
    /// <summary>
    /// Gets or sets the current interpolation function. The argument of the function is the x-value. The result is the interpolated complex value.
    /// The function should be set after building the interpolation; before that it throws an <see cref="InvalidOperationException"/>.
    /// </summary>
    public Func<double, Complex64> InterpolationFunction { get; set; }

    /// <summary>
    /// List of all x values of the imaginary-part points that are used for the interpolation.
    /// The returned list is a read-only wrapper around the internal storage for imaginary values.
    /// </summary>
    public IReadOnlyList<double> XValuesImaginary { get { return new WrapperIListToIRoVectorK<double>(ValuesImaginaryToInterpolate.Keys); } }

    /// <summary>
    /// List of all y values of the imaginary-part points that are used for the interpolation.
    /// The returned list is a read-only wrapper around the internal storage for imaginary values.
    /// </summary>
    public IReadOnlyList<double> YValuesImaginary { get { return new WrapperIListToIRoVectorV<double>(ValuesImaginaryToInterpolate.Values); } }

    /// <summary>
    /// List of the index of the imaginary curve to which each imaginary point belongs.
    /// The returned list is a read-only wrapper around the internal storage for imaginary values.
    /// </summary>
    public IReadOnlyList<int> IndexOfCurveImaginary { get { return new WrapperIListToIRoVectorV2<double>(ValuesImaginaryToInterpolate.Values); } }


    /// <summary>
    /// List of all imaginary-part points used for the interpolation, sorted by the x values.
    /// Keys are the x-Values, values are the imaginary y-values and the index of the curve the y-value belongs to.
    /// </summary>
    protected SortedList<double, (double yImaginary, int indexOfCurve)> ValuesImaginaryToInterpolate { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="InterpolationInformationComplexSeparateX"/> class.
    /// The interpolation function is initialized to a stub that throws until a real interpolation is created.
    /// </summary>
    public InterpolationInformationComplexSeparateX()
    {
      ValuesImaginaryToInterpolate = new SortedList<double, (double, int)>();
      InterpolationFunction = new Func<double, Complex64>((x) => throw new InvalidOperationException("Interpolation was not yet done."));
    }

    /// <inheritdoc/>
    public override void Clear()
    {
      base.Clear();
      ValuesImaginaryToInterpolate.Clear();
      InterpolationFunction = new Func<double, Complex64>((x) => throw new InvalidOperationException("Interpolation was not yet done."));
    }

    /// <summary>
    /// Adds values to the data that should be interpolated for either the real or the imaginary part, but does not evaluate a new interpolation.
    /// The <paramref name="groupNumber"/> selects which internal collection to use (0 = real/base collection, 1 = imaginary collection).
    /// Existing points belonging to the specified curve index are removed before adding the new column. The tracked minimum and maximum x values
    /// are updated to include the new points.
    /// </summary>
    /// <param name="shift">Shift value used to modify the x values.</param>
    /// <param name="indexOfCurve">Index of the curve in the group of curves.</param>
    /// <param name="x">Column of x values.</param>
    /// <param name="y">Column of y values.</param>
    /// <param name="groupNumber">Number of the curve group: 0 for real part (base storage), 1 for imaginary part (separate storage).</param>
    /// <param name="options">Options for creating the master curve (controls logarithmization and shift mode).</param>
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


