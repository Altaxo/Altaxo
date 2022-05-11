#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    This file is based on a translation from the Python SciPy package,
//    e.g. files _peak_finding.py, and the associated helper files and .pyx files.
//    See https://github.com/scipy/scipy/blob/main/scipy/signal/_peak_finding.py
//    The license of the Python code is the BSD-3 Clause License:
//    Copyright(c) 2001 - 2002 Enthought, Inc. 2003 - 2022, SciPy Developers.  All rights reserved.
//
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger (of the C# translation)
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
using System.Linq;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Science.Spectroscopy.PeakSearching
{
  /// <summary>
  /// Peak finder based on the Python SciPy package.
  /// </summary>
  public class PeakFinder
  {
    bool _isExecuted;

    private void CheckExecuted()
    {
      if (!_isExecuted)
        throw new InvalidOperationException($"Before accessing results, one of the Execute method of {nameof(PeakFinder)} has to be called.");
    }

    #region Parameters

    object? _height;

    /// <summary>
    /// Resets the height parameter, so that there is no requirement to the height of the peaks anymore.
    /// </summary>
    /// <returns>This instance.</returns>
    public PeakFinder ResetHeight()
    {
      _height = null;
      return this;
    }

    /// <summary>
    /// Sets the minimal height required for all peaks.
    /// </summary>
    /// <param name="value">The minimal height.</param>
    /// <returns>This instance.</returns>
    public PeakFinder SetHeight(double value)
    {
      _height = value;
      return this;
    }

    /// <summary>
    /// Sets the minimal and maximal height required for all peaks.
    /// </summary>
    /// <param name="value">The minimal and maximal height for all peaks.</param>
    /// <returns>This instance.</returns>
    public PeakFinder SetHeight((double Min, double Max) value)
    {
      _height = value;
      return this;
    }

    /// <summary>
    /// Sets the minimal height required for each peak.
    /// </summary>
    /// <param name="value">The minimal height for each peak.</param>
    /// <returns>This instance.</returns>
    public PeakFinder SetHeight(double[] value)
    {
      _height = value;
      return this;
    }

    /// <summary>
    /// Sets the minimal and maximal height required for each peak.
    /// </summary>
    /// <param name="value">The minimal and maximal height for each peak.</param>
    /// <returns>This instance.</returns>
    public PeakFinder SetHeight((double Min, double Max)[] value)
    {
      _height = value;
      return this;
    }

    object? _threshold;

    /// <summary>
    /// Resets the threshold parameter, so that there is no requirement to the threshold value of the peaks anymore.
    /// </summary>
    /// <returns>This instance.</returns>
    public PeakFinder ResetThreshold()
    {
      _threshold = null;
      return this;
    }

    /// <summary>
    /// Sets the minimal threshold value required for all peaks.
    /// Threshold is defined as minimum difference value between the peak height and its immediate neightbouring points.
    /// </summary>
    /// <param name="value">The minimal threshold value required for all peaks.</param>
    /// <returns>This instance.</returns>
    public PeakFinder SetThreshold(double value)
    {
      _threshold = value;
      return this;
    }
    /// <summary>
    /// Sets the minimal and maximal threshold value required for all peaks.
    /// Threshold is defined as minimum difference value between the peak height and its immediate neightbouring points.
    /// </summary>
    /// <param name="value">The minimal and maximal threshold value required for all peaks.</param>
    /// <returns>This instance.</returns>
    public PeakFinder SetThreshold((double Min, double Max) value)
    {
      _threshold = value;
      return this;
    }
    /// <summary>
    /// Sets the minimal threshold value required for each peak.
    /// Threshold is defined as minimum difference value between the peak height and its immediate neightbouring points.
    /// </summary>
    /// <param name="value">The minimal threshold value required for each peak.</param>
    /// <returns>This instance.</returns>
    public PeakFinder SetThreshold(double[] value)
    {
      _threshold = value;
      return this;
    }

    /// <summary>
    /// Sets the minimal and maximal threshold value required for each peak.
    /// Threshold is defined as minimum difference value between the peak height and its immediate neightbouring points.
    /// </summary>
    /// <param name="value">The minimal and maximal threshold values required for each peak.</param>
    /// <returns>This instance.</returns>
    public PeakFinder SetThreshold((double Min, double Max)[] value)
    {
      _threshold = value;
      return this;
    }



    double? _distance;


    /// <summary>
    /// Resets the distance parameter, so that there is no requirement to the distance of the peaks anymore.
    /// </summary>
    /// <returns>This instance.</returns>
    public PeakFinder ResetDistance()
    {
      _distance = null;
      return this;
    }

    /// <summary>
    /// Sets the minimal horizontal distance (&gt;= 1) between neighbouring peaks, required for all peaks.
    /// Smaller peaks are removed first until the condition is fulfilled for all remaining peaks.
    /// </summary>
    /// <param name="value">The minimal horizontal distance required between neighbouring peaks (in points).</param>
    /// <returns>This instance.</returns>
    public PeakFinder SetDistance(double value)
    {
      _distance = value;
      return this;
    }
   
    object? _prominence;

    /// <summary>
    /// Resets the prominence parameter, so that there is no requirement to the prominence values of the peaks anymore.
    /// </summary>
    /// <returns>This instance.</returns>
    public PeakFinder ResetProminence()
    {
      _prominence = null;
      return this;
    }

    /// <summary>
    /// Sets the minimal prominence value required for all peaks.
    /// Prominence is defined as the smaller of the two difference values between the peak height and the height of the neighbouring valleys.
    /// </summary>
    /// <param name="value">The minimal prominence value required for all peaks.</param>
    /// <returns>This instance.</returns>
    public PeakFinder SetProminence(double value)
    {
      _prominence = value;
      return this;
    }

    /// <summary>
    /// Sets the minimal and maximal prominence values required for all peaks.
    /// Prominence is defined as the smaller of the two difference values between the peak height and the height of the neighbouring valleys.
    /// </summary>
    /// <param name="value">The minimal and maximal prominence values required for all peaks.</param>
    /// <returns>This instance.</returns>
    public PeakFinder SetProminence((double Min, double Max) value)
    {
      _prominence = value;
      return this;
    }

    /// <summary>
    /// Sets the minimum prominence values required for each peak.
    /// Prominence is defined as the smaller of the two difference values between the peak height and the height of the neighbouring valleys.
    /// </summary>
    /// <param name="value">The minimal prominence values required for each peak.</param>
    /// <returns>This instance.</returns>
    public PeakFinder SetProminence(double[] value)
    {
      _prominence = value;
      return this;
    }

    /// <summary>
    /// Sets the minimal and maximal prominence values required for each peak.
    /// Prominence is defined as the smaller of the two difference values between the peak height and the height of the neighbouring valleys.
    /// </summary>
    /// <param name="value">The minimal and maximal prominence values required for each peak..</param>
    /// <returns>This instance.</returns>
    public PeakFinder SetProminence((double Min, double Max)[] value)
    {
      _prominence = value;
      return this;
    }

    object? _width;

    /// <summary>
    /// Resets the width parameter, so that there is no requirement to the width of the peaks anymore.
    /// </summary>
    /// <returns>This instance.</returns>
    public PeakFinder ResetWidth()
    {
      _width = null;
      return this;
    }

    /// <summary>
    /// Sets the minimal width value required for all peaks.
    /// The width of a peak is determined at the y-value, which is (prominence x relative height) less than the peak's y-value.
    /// </summary>
    /// <param name="value">The minimal width value required for all peaks.</param>
    /// <returns>This instance.</returns>
    public PeakFinder SetWidth(double value)
    {
      _width = value;
      return this;
    }

    /// <summary>
    /// Sets the minimal and maximal width value required for all peaks.
    /// The width of a peak is determined at the y-value, which is (prominence x relative height) less than the peak's y-value.
    /// </summary>
    /// <param name="value">The minimal and maximal width value required for all peaks.</param>
    /// <returns>This instance.</returns>
    public PeakFinder SetWidth((double Min, double Max) value)
    {
      _width = value;
      return this;
    }

    /// <summary>
    /// Sets the minimal width value required for each peak.
    /// The width of a peak is determined at the y-value, which is (prominence x relative height) less than the peak's y-value.
    /// </summary>
    /// <param name="value">The minimal width value required for each peak.</param>
    /// <returns>This instance.</returns>
    public PeakFinder SetWidth(double[] value)
    {
      _width = value;
      return this;
    }

    /// <summary>
    /// Sets the minimal and maximal width values required for each peak.
    /// The width of a peak is determined at the y-value, which is (prominence x relative height) less than the peak's y-value.
    /// </summary>
    /// <param name="value">The minimal and maximal width values required for each peak.</param>
    /// <returns>This instance.</returns>
    public PeakFinder SetWidth((double Min, double Max)[] value)
    {
      _width = value;
      return this;
    }

    int? _wlen;

    /// <summary>
    /// Resets the width of the search window for the neighbouring values around a peak to the full spectral range.
    /// </summary>
    /// <returns>This instance.</returns>
    public PeakFinder ResetWLen()
    {
      _wlen = null;
      return this;
    }

    /// <summary>
    /// Sets the width of the search window for the neighbouring values around a peak (in points).
    /// Used for calculation of the peaks prominences, thus it is only used if
    /// one of the parameters 'prominence' or 'width' is given. 
    /// </summary>
    /// <param name="value">width of the search window for the neighbouring values around a peak (in points).</param>
    /// <returns>This instance.</returns>
    public PeakFinder SetWLen(int value)
    {
      _wlen = value;
      return this;
    }

    double _rel_height = 0.5;

    /// <summary>
    /// Resets the relative height parameter to its default value of 0.5.
    /// </summary>
    /// <returns>This instance.</returns>
    public PeakFinder ResetRelativeHeight()
    {
      _rel_height = 0.5;
      return this;
    }

    /// <summary>
    /// Sets the relative height value that is used to determine the width of the peaks.
    /// The width of a peak is determined at the y-value, which is (prominence x relative height) below the peak's y-value.
    /// </summary>
    /// <param name="value">The relative height value.</param>
    /// <returns></returns>
    public PeakFinder SetRelativeHeight(double value)
    {
      _rel_height = value;
      return this;
    }

    object? _plateauSize;

    /// <summary>
    /// Resets the plateau size parameter, so that there is no requirement to the plateau size of the peaks anymore.
    /// </summary>
    /// <returns>This instance.</returns>
    public PeakFinder ResetPlateauSize()
    {
      _plateauSize = null;
      return this;
    }

    /// <summary>
    /// Sets the minimal plateau size value required for all peaks.
    /// </summary>
    /// <param name="value">The minimal plateau size value required for all peaks.</param>
    /// <returns>This instance.</returns>
    public PeakFinder SetPlateauSize(double value)
    {
      _plateauSize = value;
      return this;
    }

    /// <summary>
    /// Sets the minimal and maximal plateau size value required for all peaks.
    /// </summary>
    /// <param name="value">The minimal plateau size value required for all peaks.</param>
    /// <returns>This instance.</returns>
    public PeakFinder SetPlateauSize((double Min, double Max) value)
    {
      _plateauSize = value;
      return this;
    }

    /// <summary>
    /// Sets the minimal plateau size value required for each peak.
    /// </summary>
    /// <param name="value">The minimal plateau size value for required each peak.</param>
    /// <returns>This instance.</returns>
    public PeakFinder SetPlateauSize(double[] value)
    {
      _plateauSize = value;
      return this;
    }

    /// <summary>
    /// Sets the minimal and maximal plateau size value required for each peak.
    /// </summary>
    /// <param name="value">The minimal and maximal plateau size value required for each peak.</param>
    /// <returns>This instance.</returns>
    public PeakFinder SetPlateauSize((double Min, double Max)[] value)
    {
      _plateauSize = value;
      return this;
    }

    #endregion

    #region Resulting properties

    int[] _peaks = new int[0];

    /// <summary>
    /// The peak positions (indices of the y-array). This array is always set after a call to Execute.
    /// </summary>
    public int[] PeakPositions
    {
      get { CheckExecuted(); return _peaks; }
      private set { _peaks = value; }
    }

    int[]? _plateauSizes;

    /// <summary>
    /// The plateau sizes of each peak. This array is only set if the parameter PlateauSize was specified.
    /// </summary>
    public int[]? PlateauSizes
    {
      get { CheckExecuted(); return _plateauSizes; }
      private set { _plateauSizes = value; }
    }

    int[]? _leftEdges;
    /// <summary>
    /// Position of the first point to the left of each peak, whose value is different from the peak's value.
    /// This array is only set if the parameter 'PlateauSize' was specified.
    /// </summary>
    public int[]? LeftEdges
    {
      get { CheckExecuted(); return _leftEdges; }
      private set { _leftEdges = value; }
    }

    int[]? _rightEdges;
    /// <summary>
    /// Position of the first point to the right of each peak, whose value is different from the peak's value.
    /// This array is only set if the parameter 'PlateauSize' was specified.
    /// </summary>
    public int[]? RightEdges
    {
      get { CheckExecuted(); return _rightEdges; }
      private set { _rightEdges = value; }
    }

    double[]? _peakHeights;

    /// <summary>
    /// The peak height of each peak. This array is only set if the parameter Height was specified.
    /// </summary>
    public double[]? PeakHeights
    {
      get { CheckExecuted(); return _peakHeights; }
      private set { _peakHeights = value; }
    }

    double[]? _leftThresholds;

    /// <summary>
    /// The left thresholds of each peak. This array is only set if the parameter 'Threshold' was specified.
    /// </summary>
    public double[]? LeftThresholds
    {
      get { CheckExecuted(); return _leftThresholds; }
      private set { _leftThresholds = value; }
    }

    double[]? _rightThresholds;
    /// <summary>
    /// The right thresholds of each peak. This array is only set if the parameter 'Threshold' was specified.
    /// </summary>
    public double[]? RightThresholds
    {
      get { CheckExecuted(); return _rightThresholds; }
      private set { _rightThresholds = value; }
    }

    double[]? _prominences;

    /// <summary>
    /// The prominence values of each peak. This array is only set if the parameter 'Prominence' was specified.
    /// </summary>
    public double[]? Prominences
    {
      get { CheckExecuted(); return _prominences; }
      private set { _prominences = value; }
    }

    int[]? _leftBases;
    /// <summary>
    /// The position of the lowest valley point to the left of each peak. This array is only set if the parameter 'Prominence' was specified.
    /// </summary>
    public int[]? LeftBases
    {
      get { CheckExecuted(); return _leftBases; }
      private set { _leftBases = value; }
    }

    int[]? _rightBases;
    /// <summary>
    /// The position of the lowest valley point to the right of each peak. This array is only set if the parameter 'Prominence' was specified.
    /// </summary>
    public int[]? RightBases
    {
      get { CheckExecuted(); return _rightBases; }
      private set { _rightBases = value; }
    }

    double[]? _widths;
    /// <summary>
    /// The width of each peak. This array is only set if the parameter 'Width' was specified.
    /// </summary>
    public double[]? Widths
    {
      get { CheckExecuted(); return _widths; }
      private set { _widths = value; }
    }

    double[]? _widthHeights;

    /// <summary>
    /// The height at which the width of each peak was determined. This array is only set if the parameter 'Width' was specified.
    /// </summary>
    public double[]? WidthHeights
    {
      get { CheckExecuted(); return _widthHeights; }
      private set { _widthHeights = value; }
    }

    double[]? _leftIps;
    /// <summary>
    /// The left intersection points of each peak. This array is only set if the parameter 'Width' was specified.
    /// </summary>
    public double[]? LeftIps
    {
      get { CheckExecuted(); return _leftIps; }
      private set { _leftIps = value; }
    }

    double[]? _rightIps;
    /// <summary>
    /// The right intersection points of each peak. This array is only set if the parameter 'Width' was specified.
    /// </summary>
    public double[]? RightIps
    {
      get { CheckExecuted(); return _rightIps; }
      private set { _rightIps = value; }
    }

    string? _warnings;
    /// <summary>
    /// The warnings during the execution of the peak finder algorithm.
    /// </summary>
    public string? Warnings
    {
      get
      {
        CheckExecuted(); return _warnings; 
      }
    }

    #endregion

    /// <summary>
    /// Find peaks inside a signal based on peak properties.
    /// This function takes a 1-D array and finds all local maxima by
    /// simple comparison of neighboring values. Optionally, a subset of these
    /// peaks can be selected by specifying conditions for a peak's properties. For this call,
    /// the properties of this <see cref="PeakFinder"/> instance will be used (that were before set with the Set.. methods).
    /// </summary>
    /// 
    /// <returns>Indices of peaks in `x` that satisfy all given conditions. See also the other properties of this class
    /// for access to more results. Note that most of the properties are only set, if the corresponding parameter is specified in this call.
    /// </returns>
    /// <exception cref="System.ArgumentOutOfRangeException">distance` must be greater than or equal to 1 - distance</exception>
    public int[] Execute(double[] x)
    {
      return Execute(x, _height, _threshold, _distance, _prominence, _width, _wlen, _rel_height, _plateauSize);
    }

    /// <summary>
    /// Find peaks inside a signal based on peak properties.
    /// This function takes a 1-D array and finds all local maxima by
    /// simple comparison of neighboring values.Optionally, a subset of these
    /// peaks can be selected by specifying conditions for a peak's properties.
    /// </summary>
    /// 
    /// <param name="x">A signal with peaks.</param>
    /// 
    /// <param name="height">Required height of peaks. Either a number or null. The value is
    ///  always interpreted as the minimal required height.</param>
    ///  
    /// <param name="threshold">Required threshold of peaks, the vertical distance to its neighboring
    /// samples. Either a number or null. The value element is always
    /// interpreted as the minimal 
    /// required threshold.</param>
    /// 
    /// <param name="distance">Required minimal horizontal distance (&gt;= 1) in samples between
    /// neighbouring peaks. Smaller peaks are removed first until the condition
    /// is fulfilled for all remaining peaks.</param>
    /// 
    /// <param name="prominence">Required prominence of peaks. Either a number or null. The 
    /// value is always interpreted as the minimal required prominence.</param>
    /// 
    /// <param name="width">Required width of peaks in samples. Either a number or null. The value
    /// is always interpreted as the minimal required width.</param>
    /// 
    /// <param name="wlen">Used for calculation of the peaks prominences, thus it is only used if
    /// one of the arguments <paramref name="prominence"/> or <paramref name="width"/> is given. See argument
    /// <paramref name="wlen"/> in `peak_prominences` for a full description of its effects.</param>
    /// 
    /// <param name="rel_height">Used for calculation of the peaks width, thus it is only used if <paramref name="width"/>
    /// is given. Default value is 0.5. See argument  `rel_height` in <see cref="_peak_widths(double[], int[], double, double[], int[], int[])"/> for a full
    /// description of its effects.</param>
    /// 
    /// <param name="plateau_size">Required size of the flat top of peaks in samples. Either a number or null.
    /// The value is always interpreted as the minimal required plateau size.</param>
    /// 
    /// <returns>Indices of peaks in `x` that satisfy all given conditions. See also the other properties of this class
    /// for access to more results. Note that most of the properties are only set, if the corresponding parameter is specified in this call.
    /// </returns>
    /// <exception cref="System.ArgumentOutOfRangeException">distance` must be greater than or equal to 1 - distance</exception>
    public int[] Execute(
      double[] x,
      double? height = null,
      double? threshold = null,
      double? distance = null,
      double? prominence = null,
      double? width = null,
      int? wlen = null,
      double rel_height = 0.5,
      double? plateau_size = null
      )
    {
      return Execute(x, (object)height, (object)threshold, distance, prominence, width, wlen, rel_height, plateau_size);
    }

    /// <summary>
    /// Find peaks inside a signal based on peak properties.
    /// This function takes a 1-D array and finds all local maxima by
    /// simple comparison of neighboring values.Optionally, a subset of these
    /// peaks can be selected by specifying conditions for a peak's properties.
    /// </summary>
    /// 
    /// <param name="x">A signal with peaks.</param>
    /// 
    /// <param name="height">Required height of peaks. Either a number, null, an array matching
    ///  or a 2-element sequence of the former.The first element is
    ///  always interpreted as the minimal and the second, if supplied, as the
    ///  maximal required height.</param>
    ///  
    /// <param name="threshold">Required threshold of peaks, the vertical distance to its neighboring
    /// samples.Either a number, null, an array matching <paramref name="x"/> or a
    /// 2-element sequence of the former.The first element is always
    /// interpreted as the minimal and the second, if supplied, as the maximal
    /// required threshold.</param>
    /// 
    /// <param name="distance">Required minimal horizontal distance (&gt;= 1) in samples between
    /// neighbouring peaks. Smaller peaks are removed first until the condition
    /// is fulfilled for all remaining peaks.</param>
    /// 
    /// <param name="prominence">Required prominence of peaks. Either a number, null, an array
    /// matching <paramref name="x"/> or a 2-element sequence of the former.The first
    /// element is always interpreted as the minimal and the second, if
    /// supplied, as the maximal required prominence.</param>
    /// 
    /// <param name="width">Required width of peaks in samples. Either a number, null, an array
    /// matching <paramref name="x"/> or a 2-element sequence of the former.The first
    /// element is always interpreted as the minimal and the second, if
    /// supplied, as the maximal required width.</param>
    /// 
    /// <param name="wlen">Used for calculation of the peaks prominences, thus it is only used if
    /// one of the arguments `prominence` or <paramref name="width"/> is given.See argument
    /// <paramref name="wlen"/> in `peak_prominences` for a full description of its effects.</param>
    /// 
    /// <param name="rel_height">Used for calculation of the peaks width, thus it is only used if `width`
    /// is given. Default value is 0.5. See argument  `rel_height` in <see cref="_peak_widths(double[], int[], double, double[], int[], int[])"/> for a full
    /// description of its effects.</param>
    /// 
    /// <param name="plateau_size">Required size of the flat top of peaks in samples. Either a number,
    /// null, an array matching <paramref name="x"/> or a 2-element sequence of the former.
    ///  The first element is always interpreted as the minimal and the second,
    /// if supplied as the maximal required plateau size.</param>
    /// 
    /// <returns>Indices of peaks in `x` that satisfy all given conditions. See also the other properties of this class
    /// for access to more results. Note that most of the properties are only set, if the corresponding parameter is specified in this call.
    /// </returns>
    /// <exception cref="System.ArgumentOutOfRangeException">distance` must be greater than or equal to 1 - distance</exception>
    public int[] Execute(
      double[] x,
      object? height = null,
      object? threshold = null,
      double? distance = null,
      object prominence = null,
      object? width = null,
      int? wlen = null,
      double rel_height = 0.5,
      object? plateau_size = null
      )
    {
      if (distance.HasValue && !(distance >= 1))
        throw new ArgumentOutOfRangeException($"{nameof(distance)} must be greater than or equal to 1", nameof(distance));

      Reset();

      var (peaks, left_edges, right_edges) = _local_maxima_1d(x);

      if (plateau_size is not null)
      {
        // Evaluate plateau size
        var plateau_sizes = new int[right_edges.Length];
        VectorMath.Map(right_edges, left_edges, 1, (a, b, c) => a - b + c, plateau_sizes); // plateau_sizes = right_edges - left_edges + 1
        var (pmin, pmax) = _unpack_condition_args(plateau_size, x, peaks);
        var keep = _select_by_property(plateau_sizes, pmin, pmax);
        peaks = peaks.ElementsWhere(keep);
        _plateauSizes = plateau_sizes;
        _leftEdges = left_edges;
        _rightEdges = right_edges;
        CombArrays(keep); // properties = { key: array[keep] for key, array in properties.items()};
      }

      if (height is not null)
      {
        //  Evaluate height condition
        var peak_heights = x.ElementsAt(peaks);
        var (hmin, hmax) = _unpack_condition_args(height, x, peaks);
        var keep = _select_by_property(peak_heights, hmin, hmax);
        peaks = peaks.ElementsWhere(keep);
        _peakHeights = peak_heights;
        CombArrays(keep);  // properties = { key: array[keep] for key, array in properties.items()}
      }

      if (threshold is not null)
      {
        // Evaluate threshold condition
        var (tmin, tmax) = _unpack_condition_args(threshold, x, peaks);
        var (keep, left_thresholds, right_thresholds) = _select_by_peak_threshold(x, peaks, tmin, tmax);
        peaks = peaks.ElementsWhere(keep);
        _leftThresholds = left_thresholds;
        _rightThresholds = right_thresholds;
        CombArrays(keep); // properties = { key: array[keep] for key, array in properties.items()}
      }

      if (distance.HasValue)
      {
        // Evaluate distance condition
        var keep = _select_by_peak_distance(peaks, x.ElementsAt(peaks), distance.Value);
        peaks = peaks.ElementsWhere(keep);
        CombArrays(keep); // properties = { key: array[keep] for key, array in properties.items()}
      }

      if (prominence is not null || width is not null)
      {
        //  Calculate prominence (required for both conditions)
        var iwlen = _arg_wlen_as_expected(wlen);
        var (prominences, leftBase, rightBase, warnings) = _peak_prominences(x, peaks, iwlen);
        _prominences = prominences;
        _leftBases = leftBase;
        _rightBases = rightBase;
        if (warnings is not null)
        {
          _warnings = string.IsNullOrEmpty(_warnings) ? warnings : _warnings + "\r\n" + warnings;
        }
      }


      if (prominence is not null)
      {
        // Evaluate prominence condition
        var (pmin, pmax) = _unpack_condition_args(prominence, x, peaks);
        var keep = _select_by_property(_prominences, pmin, pmax);
        peaks = peaks.ElementsWhere(keep);
        CombArrays(keep); // properties = { key: array[keep] for key, array in properties.items()}
      }

      if (width is not null)
      {
        // Calculate widths
        var (widths, width_heights, left_ips, right_ips, warnings) = _peak_widths(
          x, peaks, rel_height, _prominences, _leftBases, _rightBases);

        _widths = widths;
        _widthHeights = width_heights;
        _leftIps = left_ips;
        _rightIps = right_ips;

        if (warnings is not null)
        {
          _warnings = string.IsNullOrEmpty(_warnings) ? warnings : _warnings + "\r\n" + warnings;
        }
        

        // Evaluate width condition
        var (wmin, wmax) = _unpack_condition_args(width, x, peaks);
        var keep = _select_by_property(_widths, wmin, wmax);
        peaks = peaks.ElementsWhere(keep);
        CombArrays(keep); // properties = { key: array[keep] for key, array in properties.items()}
      }

      _peaks = peaks;
      _isExecuted = true;

      return peaks;
    }

    protected void Reset()
    {
      _isExecuted = false;
      _peaks = new int[0];

      _plateauSizes = null;
      _leftEdges = null;
      _rightEdges = null;
      _peakHeights = null;
      _leftThresholds = null;
      _rightThresholds = null;
      _prominences = null;
      _leftBases = null;
      _rightBases = null;
      _widths = null;
      _widthHeights = null;
      _leftIps = null;
      _rightIps = null;

    }

    protected void CombArrays(bool[] keep)
    {
      CombArray(ref _plateauSizes, keep);
      CombArray(ref _leftEdges, keep);
      CombArray(ref _rightEdges, keep);
      CombArray(ref _peakHeights, keep);
      CombArray(ref _leftThresholds, keep);
      CombArray(ref _rightThresholds, keep);
      CombArray(ref _prominences, keep);
      CombArray(ref _leftBases, keep);
      CombArray(ref _rightBases, keep);
      CombArray(ref _widths, keep);
      CombArray(ref _widthHeights, keep);
      CombArray(ref _leftIps, keep);
      CombArray(ref _rightIps, keep);
    }

    protected void CombArray(ref int[]? array, bool[] keep)
    {
      array = array?.ElementsWhere(keep);
    }
    protected void CombArray(ref double[]? array, bool[] keep)
    {
      array = array?.ElementsWhere(keep);
    }

    protected (bool[] keep, double[] leftDiff, double[] rightDiff) _select_by_peak_threshold(double[] x, int[] peaks, double[] tmin, double[] tmax)
    {
      var leftDiff = new double[peaks.Length];
      var rightDiff = new double[peaks.Length];
      var keep = Enumerable.Repeat(true, peaks.Length).ToArray();

      for (int i = 0; i < peaks.Length; i++)
      {
        leftDiff[i] = x[peaks[i]] - x[peaks[i] - 1];
        rightDiff[i] = x[peaks[i]] - x[peaks[i] + 1];
      }

      // Stack thresholds on both sides to make min / max operations easier:
      // tmin is compared with the smaller, and tmax with the greater thresold to
      //  each peak's side
      if (tmin is not null)
      {
        for (int i = 0; i < peaks.Length; i++)
        {
          var min_threshold = Math.Min(leftDiff[i], rightDiff[i]);
          keep[i] &= (tmin[i] <= min_threshold);
        }
      }
      if (tmax is not null)
      {
        for (int i = 0; i < peaks.Length; i++)
        {
          var max_threshold = Math.Max(leftDiff[i], rightDiff[i]);

          keep[i] &= (max_threshold <= tmax[i]);
        }
      }
      return (keep, leftDiff, rightDiff);
    }


    /// <summary>
    /// Evaluate which peaks fulfill the distance condition.
    /// </summary>
    /// <param name="peaks">Indices of peaks in the spectrum.</param>
    /// <param name="priority"> An array matching <paramref name="peaks"/> used to determine priority of each peak.
    /// A peak with a higher priority value is kept over one with a lower one.</param>
    /// <param name="distanced">Minimal distance that peaks must be spaced.</param>
    /// <returns>A boolean mask evaluating to true where `peaks` fulfill the distance condition.</returns>
    protected bool[] _select_by_peak_distance(int[] peaks, double[] priority, double distanced)
    {
      var keep = Enumerable.Repeat(true, peaks.Length).ToArray();
      int i, j, k, peaks_size, distance_;

      peaks_size = peaks.Length;
      // Round up because actual peak distance can only be natural number
      distance_ = (int)Math.Ceiling(distanced);

      // Create map from `i` (index for `peaks` sorted by `priority`) to `j` (index
      // for `peaks` sorted by position). This allows to iterate `peaks` and `keep`
      // with `j` by order of `priority` while still maintaining the ability to
      // step to neighbouring peaks with (`j` + 1) or (`j` - 1).
      // priority_to_position = np.argsort(priority);
      var priority_to_position = Enumerable.Range(0, peaks.Length).ToArray();
      Array.Sort((Array)priority.Clone(), priority_to_position);

      // Highest priority first -> iterate in reverse order (decreasing)
      for (i = peaks_size - 1; i >= 0; --i)
      {
        // "Translate" `i` to `j` which points to current peak whose
        // neighbours are to be evaluated
        j = priority_to_position[i];
        if (keep[j] == false)
        { // Skip evaluation for peak already marked as "don't keep"
          continue;
        }

        k = j - 1;
        // Flag "earlier" peaks for removal until minimal distance is exceeded
        while (0 <= k && peaks[j] - peaks[k] < distance_)
        {
          keep[k] = false;
          k -= 1;
        }

        k = j + 1;
        // Flag "later" peaks for removal until minimal distance is exceeded
        while (k < peaks_size && peaks[k] - peaks[j] < distance_)
        {
          keep[k] = false;
          k += 1;
        }
      }

      return keep;  // Return as boolean array
    }


    protected (double[]? pmin, double[]? pmax) _unpack_condition_args(object interval, double[] x, int[] peaks)
    {
      IROVector<double> imin = null;
      IROVector<double> imax = null;
      if (interval is ValueTuple<int[], int[]> tupleIA)
      {
        double[] d1 = new double[tupleIA.Item1.Length];
        Array.Copy(tupleIA.Item1, d1, d1.Length);
        double[] d2 = new double[tupleIA.Item2.Length];
        Array.Copy(tupleIA.Item2, d2, d2.Length);
        imin = Vector<double>.AsWrapperFor(d1);
        imax = Vector<double>.AsWrapperFor(d2);
      }
      if (interval is ValueTuple<double[], double[]> tupleDA)
      {
        imin = Vector<double>.AsWrapperFor(tupleDA.Item1);
        imax = Vector<double>.AsWrapperFor(tupleDA.Item2);
      }
      else if (interval is ValueTuple<int, int> tupleI)
      {
        imin = VectorMath.GetConstantVector((double)tupleI.Item1, x.Length);
        imax = VectorMath.GetConstantVector((double)tupleI.Item2, x.Length);
      }
      else if (interval is ValueTuple<double, double> tupleD)
      {
        imin = VectorMath.GetConstantVector((double)tupleD.Item1, x.Length);
        imax = VectorMath.GetConstantVector((double)tupleD.Item2, x.Length);
      }
      else if (interval is int iV)
      {
        imin = VectorMath.GetConstantVector((double)iV, x.Length);
      }
      else if (interval is double dV)
      {
        imin = VectorMath.GetConstantVector(dV, x.Length);
      }
      else if(interval is null)
      {

      }
      else
      {
        throw new NotImplementedException($"Interval is {interval}, Type: {interval.GetType()}");
      }

      if (imin is not null && imin.Length != x.Length)
        throw new ArgumentException("array size of lower interval border must match x", nameof(interval));
      if (imax is not null && imax.Length != x.Length)
        throw new ArgumentException("array size of upper interval border must match x", nameof(interval));



      return (imin is null ? null : peaks.Select(idx => (double)imin[idx]).ToArray(), imax is null ? null : peaks.Select(idx => (double)imax[idx]).ToArray());
    }

    protected bool[] _select_by_property(double[] x, IReadOnlyList<double>? pmin = null, IReadOnlyList<double>? pmax = null)
    {
      var result = new bool[x.Length];
      for (int i = 0; i < result.Length; i++)
      {
        result[i] = (pmin is null || pmin[i] <= x[i]) && (pmax is null || x[i] <= pmax[i]);
      }
      return result;
    }

    protected bool[] _select_by_property(int[] x, IReadOnlyList<double>? pmin = null, IReadOnlyList<double>? pmax = null)
    {
      var result = new bool[x.Length];
      for (int i = 0; i < result.Length; i++)
      {
        result[i] = (pmin is null || pmin[i] <= x[i]) && (pmax is null || x[i] <= pmax[i]);
      }
      return result;
    }

    /// <summary>
    /// Ensure argument `wlen` is of type `np.intp` and larger than 1. Used in `peak_prominences` and `peak_widths`
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The original `value` rounded up to an integer or -1 if `value` was null.</returns>
    protected int _arg_wlen_as_expected(double? value)
    {
      int result;
      if (value is null)
      {
        // _peak_prominences expects an intp; -1 signals that no value was
        // supplied by the user
        result = -1;
      }
      else if (1 < value && value <= int.MaxValue)
      {
        // Round up to a positive integer
        result = (int)Math.Ceiling(value.Value);
      }
      else
      {
        throw new ArgumentOutOfRangeException($"wlen must be larger than 1, but was {value}");
      }
      return result;
    }

    /// <summary>
    /// Find local maxima in a 1D array.
    /// This function finds all local maxima in a 1D array and returns the indices
    /// for their edges and midpoints(rounded down for even plateau sizes).
    /// </summary>
    /// <param name="x">The array (e.g., a spectrum).</param>
    /// <returns>Tuple of midpoints (indices of midpoints of local maxima), LeftEdges: Indices of edges to the left of local maxima, and RightEdges: Indices of edges to the right of local maxima.</returns>
    /// <remarks>
    ///  A maxima is defined as one or more samples of equal value that are
    /// surrounded on both sides by at least one smaller sample.
    /// </remarks>
    public (int[] MidPoints, int[] LeftEdges, int[] RightEdges) _local_maxima_1d(double[] x)
    {
      var leftedges = new List<int>();
      var midpoints = new List<int>();
      var rightedges = new List<int>();

      int i, i_ahead, i_max;
      i_ahead = 0;

      i = 1;  //  Pointer to current sample, first one can't be maxima
      i_max = x.Length - 1; // Last sample can't be maxima
      while (i < i_max)
      {
        // Test if previous sample is smaller
        if (x[i - 1] < x[i])
        {
          i_ahead = i + 1; // Index to look ahead of current sample
        }

        // Find next sample that is unequal to x[i]
        while (i_ahead < i_max && x[i_ahead] == x[i])
        {
          ++i_ahead;
        }

        // Maxima is found if next unequal sample is smaller than x[i]
        if (x[i_ahead] < x[i])
        {
          leftedges.Add(i);
          rightedges.Add(i_ahead - 1);
          midpoints.Add((i + i_ahead - 1) / 2);
          // Skip samples that can't be maximum
          i = i_ahead;
        }
        ++i;
      }

      return (midpoints.ToArray(), leftedges.ToArray(), rightedges.ToArray());
    }

    /// <summary>
    /// Calculate the prominence of each peak in a signal.
    /// </summary>
    /// <param name="x">The signal, e.g. a spectrum with peaks.</param>
    /// <param name="peaks">The indices of the peak positions in <paramref name="x"/>.</param>
    /// <param name="wlen">A window length in samples (see `peak_prominences`) which is rounded up
    /// to the nearest odd integer.If smaller than 2 the entire signal `x` is
    /// used.</param>
    public (double[] Prominences, int[] LeftBase, int[] RightBase, string? warnings) _peak_prominences(double[] x, int[] peaks, int wlen)
    {
      int i;
      bool show_warning = false;
      double left_min, right_min;

      var prominences = new double[peaks.Length];
      var left_bases = new int[peaks.Length];
      var right_bases = new int[peaks.Length];

      for (int peak_nr = 0; peak_nr < peaks.Length; ++peak_nr)
      {
        var peak = peaks[peak_nr];
        var i_min = 0;
        var i_max = x.Length - 1;
        if (!(i_min <= peak && peak <= i_max))
        {
          throw new ArgumentException($"peak {peak} is not a valid index for `x`");
        }

        if (2 <= wlen)
        {
          // Adjust window around the evaluated peak (within bounds);
          // if wlen is even the resulting window length is is implicitly
          // rounded to next odd integer
          i_min = Math.Max(peak - wlen / 2, i_min);
          i_max = Math.Min(peak + wlen / 2, i_max);
        }

        // Find the left base in interval [i_min, peak]
        i = left_bases[peak_nr] = peak;
        left_min = x[peak];
        while (i_min <= i && x[i] <= x[peak])
        {
          if (x[i] < left_min)
          {
            left_min = x[i];
            left_bases[peak_nr] = i;
          }
          --i;
        }

        // Find the right base in interval [peak, i_max]
        i = right_bases[peak_nr] = peak;
        right_min = x[peak];
        while (i <= i_max && x[i] <= x[peak])
        {
          if (x[i] < right_min)
          {
            right_min = x[i];
            right_bases[peak_nr] = i;
          }
          ++i;
        }

        prominences[peak_nr] = x[peak] - Math.Max(left_min, right_min);
        if (prominences[peak_nr] == 0)
        {
          show_warning = true;
        }
      }

      string? warnings = null;
      if (show_warning)
      {
        warnings = "some peaks have a prominence of 0";
      }

      return (prominences, left_bases, right_bases, warnings);
    }


    /// <summary>
    ///  Calculate the width of each each peak in a signal.
    /// </summary>
    /// <param name="x">A signal with peaks.</param>
    /// <param name="peaks">Indices of peaks in <paramref name="x"/>.</param>
    /// <param name="rel_height"> Chooses the relative height at which the peak width is measured as a percentage of its prominence.</param>
    /// <param name="prominences">Prominences of each peak in `peaks` as returned by <see cref="_peak_prominences(double[], int[], int)"/>.</param>
    /// <param name="left_bases">Left bases of each peak in `peaks` as returned by <see cref="_peak_prominences(double[], int[], int)"/>.</param>
    /// <param name="right_bases">Right bases of each peak in `peaks` as returned by <see cref="_peak_prominences(double[], int[], int)"/>.</param>
    /// <returns>Tuple, consisting of widths: The widths for each peak in samples;
    /// width_heights : The height of the contour lines at which the `widths` where evaluated;
    /// left_ips, right_ips :  Interpolated positions of left and right intersection points of a horizontal line at the respective evaluation height.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// rel_height` must be greater than or equal to 0 - rel_height
    /// or
    /// arrays in `prominence_data` must have the same length as `peaks`
    /// </exception>
    /// <exception cref="System.InvalidOperationException">prominence data is invalid for peak {peak}</exception>
    protected (double[] widths, double[] width_heights, double[] left_ips, double[] right_ips, string? warnings) _peak_widths(
      double[] x,
      int[] peaks,
      double rel_height,
      double[] prominences,
      int[] left_bases,
      int[] right_bases)
    {
      double height, left_ip, right_ip;
      int p, peak, i, i_max, i_min;

      if (!(rel_height >= 0))
      {
        throw new ArgumentOutOfRangeException("rel_height` must be greater than or equal to 0", nameof(rel_height));
      }

      if (!(peaks.Length == prominences.Length && peaks.Length == left_bases.Length && peaks.Length == right_bases.Length))
      {
        throw new ArgumentOutOfRangeException("arrays in `prominence_data` must have the same length as `peaks`");
      }

      var show_warning = false;
      var widths = new double[peaks.Length];
      var width_heights = new double[peaks.Length];
      var left_ips = new double[peaks.Length];
      var right_ips = new double[peaks.Length];

      for (p = 0; p < peaks.Length; ++p)
      {
        i_min = left_bases[p];
        i_max = right_bases[p];
        peak = peaks[p];
        // Validate bounds and order
        if (!(0 <= i_min && i_min <= peak && peak <= i_max && i_max < x.Length))
        {
          throw new InvalidOperationException($"prominence data is invalid for peak {peak}");
        }
        height = width_heights[p] = x[peak] - prominences[p] * rel_height;


        // Find intersection point on left side
        i = peak;
        while (i_min < i && height < x[i])
        {
          --i;
        }
        left_ip = i;
        if (x[i] < height)
        { // Interpolate if true intersection height is between samples
          left_ip += (height - x[i]) / (x[i + 1] - x[i]);
        }

        // Find intersection point on right side
        i = peak;
        while (i < i_max && height < x[i])
        {
          ++i;
        }
        right_ip = i;
        if (x[i] < height)
        {

          // Interpolate if true intersection height is between samples
          right_ip -= (height - x[i]) / (x[i - 1] - x[i]);
        }

        widths[p] = right_ip - left_ip;
        if (widths[p] == 0)
        {
          show_warning = true;
        }
        left_ips[p] = left_ip;
        right_ips[p] = right_ip;
      }

      return (widths, width_heights, left_ips, right_ips, show_warning ? "some peaks have a width of 0" : null);
    }
  }
}
