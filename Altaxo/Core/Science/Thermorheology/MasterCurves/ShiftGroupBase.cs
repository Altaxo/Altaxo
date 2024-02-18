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

namespace Altaxo.Science.Thermorheology.MasterCurves
{
  /// <summary>
  /// The base of a collection of multiple x-y curves (see <see cref="ShiftCurve{T}"/>) that will finally form one master curve.
  /// </summary>
  public class ShiftGroupBase
  {
    /// <summary>
    /// Determines how to shift the x values: either by factor or by offset. Use offset if the original data are already logarithmized.
    /// </summary>
    public ShiftXBy XShiftBy { get; }

    /// <summary>Logarithmize x values before adding to the interpolation curve. (Only for interpolation).</summary>
    public bool LogarithmizeXForInterpolation { get; }

    /// <summary>Logarithmize y values before adding to the interpolation curve. (Only for interpolation).</summary>
    public bool LogarithmizeYForInterpolation { get; }


    /// <summary>
    /// Initializes a new instance of the <see cref="ShiftGroupDouble"/> class.
    /// </summary>
    /// <param name="xShiftBy">Shift method, either additive or multiplicative.</param>
    /// <param name="logarithmizeXForInterpolation">If true, the x-values are logarithmized prior to participating in the interpolation function.</param>
    /// <param name="logarithmizeYForInterpolation">If true, the y-values are logartihmized prior to participating in the interpolation function.</param>
    public ShiftGroupBase(ShiftXBy xShiftBy, bool logarithmizeXForInterpolation, bool logarithmizeYForInterpolation)
    {
      XShiftBy = xShiftBy;
      LogarithmizeXForInterpolation = logarithmizeXForInterpolation;
      LogarithmizeYForInterpolation = logarithmizeYForInterpolation;
    }


    /// <summary>
    /// Creates a new <see cref="InvalidOperationException"/> that should be thrown if the interpolation information was not initialized before.
    /// </summary>
    public static Exception NewExceptionNoInterpolationInformation => new InvalidOperationException($"Interpolation information is not initialized!");

    /// <summary>
    /// Creates a new <see cref="InvalidOperationException"/> that should be thrown if currently no interpolation information is available.
    /// </summary>
    public static Exception NewExceptionNoInterpolation => new InvalidOperationException($"Currently, no interpolation is available");
  }
}

