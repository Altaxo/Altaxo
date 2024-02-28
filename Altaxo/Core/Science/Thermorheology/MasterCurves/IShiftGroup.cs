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

using System.Collections.Generic;

namespace Altaxo.Science.Thermorheology.MasterCurves
{
  /// <summary>
  /// Interface for a shift group. A shift group consist of a number of curves, which when shifted properly, finally form a master curve.
  /// The curves can by scalar valued (y is of type <see cref="System.Double"/>, or complex valued.
  /// </summary>
  public interface IShiftGroup
  {

    /// <summary>
    /// Gets the number of curves in this shift group.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Determines how to shift the x values: either by factor or by offset. Use offset if the original data are already logarithmized.
    /// </summary>
    ShiftXBy XShiftBy { get; }

    /// <summary>Logarithmize x values before adding to the interpolation curve. (Only for interpolation).</summary>
    bool LogarithmizeXForInterpolation { get; }


    /// <summary>
    /// Gets the index of the curve in the given group with the most variation. The variation is determined by calculating the absolute slope,
    /// with applying the logarithmic transformations according to the interpolation settings in that group.
    /// </summary>
    /// <returns>The index of the curve with most variation. If no suitable curve was found, then the return value is null.</returns>
    int? GetCurveIndexWithMostVariation();

    /// <summary>
    /// Initializes the interpolation with initially no points in it.
    /// </summary>
    void InitializeInterpolation();

    /// <summary>
    /// Adds the data of the curve with index <paramref name="idxCurve"/> to the interpolation.
    /// If data for that curve are already present in the interpolation data, they are removed, and then added anew.
    /// </summary>
    /// <param name="idxCurve">The index of the curve to add.</param>
    /// <param name="shift">The current shift that should be used.</param>
    void AddCurveToInterpolation(int idxCurve, double shift);

    /// <summary>
    /// Creates the interpolation with the data previously added with <see cref="AddCurveToInterpolation(int, double)"/>.
    /// </summary>
    void Interpolate();

    /// <summary>
    /// Gets the minimum and maximum of the x-values,
    /// taking into account the logarithmize options, and whether the corresponding y-values are valid.
    /// </summary>
    /// <param name="idxCurve">Index of the curve.</param>
    /// <returns>Minimum and maximum of the x-values, for x and y values appropriate for the conditions given by the parameter.</returns>
    public (double min, double max) GetXMinimumMaximumOfCurvePointsSuitableForInterpolation(int idxCurve);

    /// <summary>
    /// Tracks the x minimum and x maximum of the master curve points.
    /// </summary>
    /// <param name="idxCurve">The index of the curve to consider.</param>
    /// <param name="shift">The shift value for this curve.</param>
    /// <param name="startNewTracking">If set to true, a new tracking will be started, i.e. the xmin and xmax of the curve (under consideration of the shift value) is
    /// set as the new tracked xminimum and xmaximum. If false, the xmin and xmax of the curve (under consideration) of the shift value is calculated, and then merged
    /// into the tracked xminimum and xmaximum.</param>
    public void TrackXMinimumMaximumOfMasterCurvePoints(int idxCurve, double shift, bool startNewTracking);

    /// <summary>
    /// Gets the tracked x minimum and x maximum values.
    /// For explanation, see <see cref="TrackXMinimumMaximumOfMasterCurvePoints(int, double, bool)"/>.
    /// The convention is, that when shifting by multiplication, the returned values are already logarithmized, whereas, if shifted by offset, the returned values are not logarithmized.
    /// That means that the possible shifts can always be calculated by subtraction.
    /// </summary>
    /// <returns>The tracked x-minimum and x-maximum values.</returns>
    public (double xmin, double xmax) GetTrackedXMinimumMaximum();

    /// <summary>
    /// Gets the minimum and maximum of the current x-values used for interpolation. Data points that belong
    /// to the curve with the index given in the argument are not taken into account.
    /// </summary>
    /// <param name="indexOfCurve">The index of curve.</param>
    /// <returns>The minimum and maximum of the x-values, except for those points that belong to the curve with index=<paramref name="indexOfCurve"/>.</returns>
    public (double min, double max) GetXMinimumMaximumOfInterpolationValuesExceptForCurveIndex(int indexOfCurve);

    /// <summary>
    /// Gets the mean difference between the y column and the interpolation function, provided that the x column is shifted by a factor.
    /// </summary>
    /// <param name="idxCurve">The index of the curve to fit.</param>
    /// <param name="shift">Shift offset (direct offset or natural logarithm of the shiftFactor for the new part of the master curve.</param>
    /// <returns>Returns the calculated penalty value (mean difference between interpolation curve and provided data),
    /// and the number of points (of the new part of the curve) used for calculating the penalty value.</returns>
    /// 
    public (double Penalty, int EvaluatedPoints) GetMeanAbsYDifference(int idxCurve, double shift);

    /// <summary>
    /// Gets the mean squared difference between the y column and the interpolation function, provided that the x column is shifted by a factor.
    /// </summary>
    /// <param name="idxCurve">The index of the curve to fit.</param>
    /// <param name="shift">Shift offset (direct offset or natural logarithm of the shiftFactor for the new part of the master curve.</param>
    /// <returns>Returns the calculated penalty value (mean difference between interpolation curve and provided data),
    /// and the number of points (of the new part of the curve) used for calculating the penalty value.</returns>
    /// 
    public (double Penalty, int EvaluatedPoints) GetMeanSquaredYDifference(int idxCurve, double shift);

    /// <summary>
    /// Gets a value indicating whether this group will participate in the fit, determined by the fit weight. When the fit weigth is zero, the return value is false.
    /// Otherwise, if the fit weight is positive, the group principally will participate in the fit, and the return value is true.
    /// </summary>
    public bool ParticipateInFitByFitWeight { get; }

    /// <summary>
    /// Determines whether the curve is suitable for participating in the fit. For instance, the curve must have at least two points.
    /// </summary>
    /// <param name="idxCurve">The curve index.</param>
    /// <returns>
    ///   <c>true</c> if the curve is suitable for participating in the fit; otherwise, <c>false</c>.
    /// </returns>
    public bool IsCurveSuitableForParticipatingInFit(int idxCurve);


    /// <summary>
    /// Gets the number of value components. For a usual curve, the return value is 1. For a curve consisting of
    /// complex values, the return value is 2.
    /// </summary>
    public int NumberOfValueComponents { get; }

    /// <summary>
    /// Gets the original curve points of the curve with the provided index.
    /// </summary>
    /// <param name="idxCurve">The index of the curve.</param>
    /// <param name="idxValueComponent">The index of the value component, see <see cref="NumberOfValueComponents"/>.</param>
    /// <returns>Pair of x and y arrays (of same length) representing the curve points. For complexed valued y, if <paramref name="idxValueComponent"/> is 0, the real part is returned as y, for <paramref name="idxValueComponent"/>==1 the imaginary part is returned.</returns>
    public (IReadOnlyList<double> X, IReadOnlyList<double> Y) GetCurvePoints(int idxCurve, int idxValueComponent);

    /// <summary>
    /// Gets the curve points of the shifted curve with  with the provided index.
    /// </summary>
    /// <param name="idxCurve">The index of the curve.</param>
    /// <param name="idxValueComponent">The index of the value component, see <see cref="NumberOfValueComponents"/>.</param>
    /// <param name="shiftValue">The actual shift value.</param>
    /// <returns>Pair of x and y arrays (of same length) representing the shifted curve points. For complexed valued y, if <paramref name="idxValueComponent"/> is 0, the real part is returned as y, for <paramref name="idxValueComponent"/>==1 the imaginary part is returned.</returns>
    public (IReadOnlyList<double> X, IReadOnlyList<double> Y) GetShiftedCurvePoints(int idxCurve, int idxValueComponent, double shiftValue);

    /// <summary>
    /// Gets the merged curve points that were used for interpolation. Before calling this function, the interpolation has to be initialized, and curves must be added to the interpolation.
    /// </summary>
    /// <param name="idxValueComponent">The index of the value component, see <see cref="NumberOfValueComponents"/>.</param>
    /// <returns>Pair of x and y arrays (of same length) representing the merged curve points. For complexed valued y, if <paramref name="idxValueComponent"/> is 0, the real part is returned as y, for <paramref name="idxValueComponent"/>==1 the imaginary part is returned.
    /// Furthermore, the array IndexOfCurve contains for every point to which curve it originally belongs.</returns>
    public (IReadOnlyList<double> X, IReadOnlyList<double> Y, IReadOnlyList<int> IndexOfCurve) GetMergedCurvePointsUsedForInterpolation(int idxValueComponent);

    /// <summary>
    /// Gets interpolated curve points between the minimum x-value and the maximum x-value.
    /// Before calling this function, the interpolation has to be initialized, curves must be added to the interpolation, and then <see cref="Interpolate"/> must be called.
    /// </summary>
    /// <param name="idxValueComponent">The index of the value component, see <see cref="NumberOfValueComponents"/>.</param>
    /// <param name="numberOfInterpolationPoints">Number of points used for the interpolation. The default value is 1001.</param>
    /// <returns>Pair of x and y arrays (of same length) representing the interpolated curve points. For complexed valued y, if <paramref name="idxValueComponent"/> is 0, the real part is returned as y, for <paramref name="idxValueComponent"/>==1 the imaginary part is returned.</returns>
    public (IReadOnlyList<double> X, IReadOnlyList<double> Y) GetInterpolatedCurvePoints(int idxValueComponent, int numberOfInterpolationPoints = 1001);

  }
}

