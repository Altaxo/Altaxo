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

using System;
using System.Collections.Generic;
using Complex64 = System.Numerics.Complex;

namespace Altaxo.Science.Thermorheology.MasterCurves
{
  /// <summary>
  /// A collection of multiple x-y curves (see <see cref="ShiftCurve{Double}"/>) that will finally form one master curve.
  /// </summary>
  public class ShiftGroupComplex : ShiftGroupBase<Complex64>
  {
    /// <summary>
    /// Creates the fit function. Argument is the tuple consisting of X, Y, and optional YErr. Return value is a function that calculates y for a given x.
    /// </summary>
    public Func<(IReadOnlyList<double> X, IReadOnlyList<Complex64> Y, IReadOnlyList<Complex64>? YErr), Func<double, Complex64>>? CreateInterpolationFunction { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ShiftGroup"/> class.
    /// </summary>
    /// <param name="data">Collection of multiple x-y curves that will finally form one master curve.</param>
    /// <param name="xShiftBy">Shift method, either additive or multiplicative.</param>
    /// <param name="fitWeight">The weight with which to participate in the fit. Has to be &gt; 0.</param>
    /// <param name="logarithmizeXForInterpolation">If true, the x-values are logarithmized prior to participating in the interpolation function.</param>
    /// <param name="logarithmizeYForInterpolation">If true, the y-values are logartihmized prior to participating in the interpolation function.</param>
    /// <param name="createInterpolationFunction">Function that creates the interpolation. Input are the x-array, y-array, and optionally, the array of y-errors. Output is an interpolation function which returns an interpolated y-value for a given x-value.</param>
    public ShiftGroupComplex(IEnumerable<ShiftCurve<Complex64>> data, ShiftXBy xShiftBy, double fitWeight, bool logarithmizeXForInterpolation, bool logarithmizeYForInterpolation, Func<(IReadOnlyList<double> X, IReadOnlyList<Complex64> Y, IReadOnlyList<Complex64>? YErr), Func<double, Complex64>>? createInterpolationFunction = null)
      : base(data, xShiftBy, fitWeight, logarithmizeXForInterpolation, logarithmizeYForInterpolation)
    {
      CreateInterpolationFunction = createInterpolationFunction;
    }
  }
}

