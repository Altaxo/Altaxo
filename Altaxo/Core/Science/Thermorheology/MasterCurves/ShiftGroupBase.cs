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
  public abstract class ShiftGroupBase
  {
    /// <summary>
    /// Determines how to shift the x values: either by factor or by offset. Use offset if the original data are already logarithmized.
    /// </summary>
    public ShiftXBy XShiftBy { get; }

    /// <summary>
    /// If true, x values are logarithmized before being added to the interpolation curve. (Only used for interpolation.)
    /// </summary>
    public bool LogarithmizeXForInterpolation { get; }

    /// <summary>
    /// If true, y values are logarithmized before being added to the interpolation curve. (Only used for interpolation.)
    /// </summary>
    public bool LogarithmizeYForInterpolation { get; }


    protected double _trackedXMinimum, _trackedXMaximum;

    /// <summary>
    /// Gets the tracked x minimum and x maximum values. For explanation, see <see cref="TrackXMinimumMaximumOfMasterCurvePoints(int, double, bool)"/>.
    /// The convention is that when shifting by multiplication, the returned values are already logarithmized; whereas if shifted by offset, the returned values are not logarithmized.
    /// That means that the possible shifts can always be calculated by subtraction.
    /// </summary>
    /// <returns>The tracked x-minimum and x-maximum values.</returns>
    public (double xmin, double xmax) GetTrackedXMinimumMaximum() => (_trackedXMinimum, _trackedXMaximum);

    /// <summary>
    /// Tracks the x minimum and x maximum of the master curve points.
    /// The convention used is: when shifting by multiplication, the values are already logarithmized; when shifting by offset, the values are not logarithmized.
    /// </summary>
    /// <param name="idxCurve">The index of the curve to consider.</param>
    /// <param name="shift">The shift value for this curve.</param>
    /// <param name="startNewTracking">If set to <c>true</c>, a new tracking will be started: the xmin and xmax of the curve (taking the shift value into account)
    /// are set as the new tracked x minimum and x maximum. If <c>false</c>, the xmin and xmax of the curve (taking the shift value into account) are calculated
    /// and then merged into the tracked x minimum and x maximum.</param>
    public void TrackXMinimumMaximumOfMasterCurvePoints(int idxCurve, double shift, bool startNewTracking = false)
    {
      var (min, max) = GetXMinimumMaximumOfCurvePointsSuitableForInterpolation(idxCurve);
      (min, max) = (XShiftBy, LogarithmizeXForInterpolation) switch
      {
        (ShiftXBy.Offset, false) => (min + shift, max + shift),
        (ShiftXBy.Offset, true) => (Math.Exp(min) + shift, Math.Exp(max) + shift),
        (ShiftXBy.Factor, false) => (Math.Log(min) + shift, Math.Log(max) + shift),
        (ShiftXBy.Factor, true) => (min + shift, max + shift),
        _ => throw new NotImplementedException(),
      };
      if (startNewTracking == true)
      {
        (_trackedXMinimum, _trackedXMaximum) = (min, max);
      }
      else
      {
        (_trackedXMinimum, _trackedXMaximum) = (Math.Min(min, _trackedXMinimum), Math.Max(max, _trackedXMaximum));
      }
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="ShiftGroupBase"/> class.
    /// </summary>
    /// <param name="xShiftBy">Shift method, either additive or multiplicative.</param>
    /// <param name="logarithmizeXForInterpolation">If <c>true</c>, the x-values are logarithmized prior to participating in the interpolation function.</param>
    /// <param name="logarithmizeYForInterpolation">If <c>true</c>, the y-values are logarithmized prior to participating in the interpolation function.</param>
    public ShiftGroupBase(ShiftXBy xShiftBy, bool logarithmizeXForInterpolation, bool logarithmizeYForInterpolation)
    {
      XShiftBy = xShiftBy;
      LogarithmizeXForInterpolation = logarithmizeXForInterpolation;
      LogarithmizeYForInterpolation = logarithmizeYForInterpolation;
    }

    /// <summary>
    /// Gets the minimum and maximum of the x-values,
    /// taking into account the logarithmize options, and whether the corresponding y-values are valid.
    /// </summary>
    /// <param name="idxCurve">Index of the curve.</param>
    /// <returns>Minimum and maximum of the x-values, for x and y values appropriate for the conditions given by the parameter.</returns>
    public abstract (double min, double max) GetXMinimumMaximumOfCurvePointsSuitableForInterpolation(int idxCurve);

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

