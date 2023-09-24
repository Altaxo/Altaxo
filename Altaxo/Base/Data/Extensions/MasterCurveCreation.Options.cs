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
using System.Collections.Generic;
using Altaxo.Calc.Interpolation;

namespace Altaxo.Data
{
  public static partial class MasterCurveCreation
  {
    /// <summary>
    /// Contains options for master curve creation.
    /// </summary>
    public class Options
    {
      /// <summary>
      /// List of groups of columns. The outer list normally contains 2 items: a group of columns containing the real parts of the measured values,
      /// and another group of columns containing the imaginary part of the measured values.
      /// </summary>
      private List<List<DoubleColumn>> _columnGroups = new List<List<DoubleColumn>>();

      /// <summary>
      /// List of groups of columns. The outer list normally contains 2 items: a group of columns containing the real parts of the measured values,
      /// and another group of columns containing the imaginary part of the measured values.
      /// </summary>
      public List<List<DoubleColumn>> ColumnGroups { get { return _columnGroups; } }

      /// <summary>Index of the reference column, but index points to _columnGroups.</summary>
      public int IndexOfReferenceColumnInColumnGroup;

      /// <summary>Logarithmize x values before adding to the interpolation curve. (Only for interpolation).</summary>
      public bool LogarithmizeXForInterpolation { get; set; }

      /// <summary>Logarithmize y values before adding to the interpolation curve. (Only for interpolation).</summary>
      public bool LogarithmizeYForInterpolation { get; set; }

      /// <summary>
      /// Determines how to shift the x values: either by factor or by offset.
      /// </summary>
      public ShiftXBy XShiftBy { get; set; }

      /// <summary>
      /// Resulting list of shift offsets or ln(shiftfactors).
      /// </summary>
      private List<double> _resultingShiftFactors = new List<double>();

      /// <summary>
      /// Resulting list of shift offsets or ln(shiftfactors).
      /// </summary>
      public List<double> ResultingShifts { get { return _resultingShiftFactors; } }

      /// <summary>
      /// Gets the resulting interpolation curve for each group of columns.
      /// </summary>
      public InterpolationInformation[]? ResultingInterpolation { get; set; }

      /// <summary>
      /// Determines the method to best fit the data into the master curve.
      /// </summary>
      public OptimizationMethod OptimizationMethod { get; set; }

      protected Func<IReadOnlyList<double>, IReadOnlyList<double>, IReadOnlyList<double>?, Func<double, double>> _interpolationFunctionCreation = (x, y, dy) => new LinearInterpolationOptions().Interpolate(x, y, dy).GetYOfX;

      /// <summary>
      /// Gets or sets a function that creates the interpolation function.
      /// </summary>
      /// <value>
      /// The creation function for the interpolation function. Per default this is set to a function that creates a <see cref="LinearInterpolation"/>
      /// </value>
      public Func<IReadOnlyList<double>, IReadOnlyList<double>, IReadOnlyList<double>?, Func<double, double>> CreateInterpolationFunction
      {
        get { return _interpolationFunctionCreation; }
        set { _interpolationFunctionCreation = value ?? throw new ArgumentNullException(nameof(value)); }
      }

      protected int _numberOfIterations = 1;

      /// <summary>
      /// Gets or sets the number of iterations. Must be greater than or equal to 1.
      /// This number determines how many rounds the master curve is fitted. Increasing this value will in most cases
      /// increase the quality of the fit.
      /// </summary>
      /// <value>
      /// The number of iterations for master curve creation.
      /// </value>
      /// <exception cref="ArgumentOutOfRangeException">value - Must be a number >= 1</exception>
      public int NumberOfIterations
      {
        get { return _numberOfIterations; }
        set
        {
          if (!(value >= 1))
            throw new ArgumentOutOfRangeException(nameof(value), "Must be a number >= 1");

          _numberOfIterations = value;
        }
      }
    }
  }
}
