#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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
using Altaxo.Calc;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Science.Spectroscopy.EnsembleProcessing
{
  /// <summary>
  /// Removes spikes from an ensemble of spectra by using statistics computed across the ensemble.
  /// The preprocessor detects outlier points per wavelength (column) across spectra (rows) and patches
  /// positive spikes (and optionally also negative spikes) using a peak-elimination routine. Options control detection sensitivity and
  /// whether negative spikes are also eliminated.
  /// </summary>
  public record SpikeRemovalByEnsembleStatistics : IEnsemblePreprocessor
  {
    /// <summary>
    /// When <c>true</c>, negative spikes will be considered for elimination in addition to positive spikes.
    /// </summary>
    public bool EliminateNegativeSpikes { get; init; }

    /// <summary>
    /// Number of standard deviations used to extend percentile-based bounds when detecting outliers.
    /// Larger values make the spike detector less sensitive.
    /// </summary>
    public double NumberOfSigmas { get; init; } = 10;

    /// <summary>
    /// Maximum allowed width for a detected spike (in sample points). Wider events are not considered spikes
    /// and will be ignored by the peak elimination routine.
    /// </summary>
    public double MaximalWidth { get; init; } = 5;

    /// <summary>
    /// Minimal signal-to-noise ratio required for processing. Values below this threshold may be
    /// considered too noisy for reliable spike detection.
    /// </summary>
    public double MinimalSignalToNoiseRatio { get; init; } = 10;

    #region Serialization



    /// <summary>
    /// XML serialization surrogate for <see cref="SpikeRemovalByEnsembleStatistics"/> version 0.
    /// Provides custom serialization and deserialization logic for the record's fields.
    /// </summary>
    /// <remarks>V0: 2026-03-18</remarks>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SpikeRemovalByEnsembleStatistics), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (SpikeRemovalByEnsembleStatistics)obj;
        info.AddValue("NumberOfSigmas", s.NumberOfSigmas);
        info.AddValue("MinimalSignalToNoiseRatio", s.MinimalSignalToNoiseRatio);
        info.AddValue("MaximalWidth", s.MaximalWidth);
        info.AddValue("EliminateNegativeSpikes", s.EliminateNegativeSpikes);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var numberOfSigmas = info.GetDouble("NumberOfSigmas");
        var minimalSignalToNoiseRatio = info.GetDouble("MinimalSignalToNoiseRatio");
        var maximalWidth = info.GetInt32("MaximalWidth");
        var eliminateNegativeSpikes = info.GetBoolean("EliminateNegativeSpikes");

        return (o as SpikeRemovalByEnsembleStatistics ?? new SpikeRemovalByEnsembleStatistics()) with
        {
          NumberOfSigmas = numberOfSigmas,
          MinimalSignalToNoiseRatio = minimalSignalToNoiseRatio,
          MaximalWidth = maximalWidth,
          EliminateNegativeSpikes = eliminateNegativeSpikes,
        };
      }
    }
    #endregion



    /// <inheritdoc/>
    public (double[] x, Matrix<double> y, int[]? regions, IEnsembleProcessingAuxiliaryData? auxiliaryData) Execute(double[] x, Matrix<double> y, int[]? regions)
    {
      var yNew = Execute(x, y);

      return (x, yNew, regions, null);
    }

    /// <inheritdoc/>
    public (double[] x, double[] y, int[]? regions) Execute(double[] x, double[] y, int[]? regions)
    {
      return (x, y, regions);
    }

    /// <inheritdoc/>
    public (double[] x, Matrix<double> y, int[]? regions) ExecuteForPrediction(double[] x, Matrix<double> spectraMatrix, int[] regions, IEnsembleProcessingAuxiliaryData? auxillaryData)
    {
      var yNew = Execute(x, spectraMatrix);

      return (x, yNew, regions);
    }

    /// <summary>
    /// Identifies and patches outliers in the specified matrix using the provided input array.
    /// </summary>
    /// <param name="x">An array of double values used as input for the outlier patching process. The contents of this array influence
    /// how outliers in the matrix are corrected.</param>
    /// <param name="y">The matrix to analyze for outliers and to use as the basis for generating the patched result. Cannot be null.</param>
    /// <returns>A new matrix with outliers patched based on the input array and the original matrix. The returned matrix has the
    /// same dimensions as the input matrix.</returns>
    private Matrix<double> Execute(double[] x, Matrix<double> y)
    {
      var dict = FindOutliers(y);
      var yNew = y.Clone();
      PatchOutliers(x, y, yNew, dict);

      return yNew;
    }

    /// <summary>
    /// Finds outlier points per spectrum by examining the distribution of values across the ensemble
    /// at each wavelength (column). For every column the method determines percentile bounds and
    /// flags values outside the computed bounds as outliers.
    /// </summary>
    /// <param name="y">The ensemble matrix with spectra in rows and wavelengths in columns.</param>
    /// <returns>
    /// A sorted dictionary mapping spectrum indices (row indices) to lists of column indices (spectral point indices) where
    /// outliers were detected.
    /// </returns>
    public SortedDictionary<int, List<int>> FindOutliers(IROMatrix<double> y)
    {
      // first step: for every column of the matrix, extract the y-values in a list and order the list
      var yList = new double[y.RowCount];
      var idxArray = new int[y.RowCount];

      var lowerPercentile = 0.25;
      var upperPercentile = 0.75;

      if (y.RowCount > 100)
      {
        lowerPercentile = 0.1;
        upperPercentile = 0.9;
      }

      var dict = new SortedDictionary<int, List<int>>(); // key is number of spectrum, value are the list of outliers (spectral index)

      for (int idxColumn = 0; idxColumn < y.ColumnCount; ++idxColumn)
      {
        for (int idxRow = 0; idxRow < y.RowCount; ++idxRow)
        {
          yList[idxRow] = y[idxRow, idxColumn];
          idxArray[idxRow] = idxRow;
        }

        // now sort the array
        Array.Sort(yList, idxArray);
        var (lower, upper) = GetPercentileBounds(yList, lowerPercentile, upperPercentile, NumberOfSigmas);

        for (int idxRow = yList.Length - 1; idxRow >= 0; --idxRow)
        {
          if (yList[idxRow] <= upper)
          {
            if (idxRow < yList.Length - 1)
            {
              for (int k = idxRow + 1; k < yList.Length; ++k)
              {
                var outlier = idxArray[k];
                if (!dict.TryGetValue(outlier, out var list))
                {
                  list = [];
                  dict[outlier] = list;
                }
                list.Add(idxColumn);
              }
            }
            break;
          }
        }
      }

      return dict;
    }

    /// <summary>
    /// Identifies and corrects positive spike outliers in the specified regions of the input matrix, updating the
    /// results in a separate output matrix.
    /// </summary>
    /// <remarks>This method processes only the regions specified in the dictionary and leaves other data
    /// unchanged. The correction is applied in-place to the output matrix, allowing the original data to remain
    /// unmodified for comparison or further analysis.</remarks>
    /// <param name="x">The array of x-axis values corresponding to the columns of the input and output matrices.</param>
    /// <param name="y">The input matrix containing the original data to be processed for outlier correction.</param>
    /// <param name="yNew">The output matrix where the corrected data is written. Must have the same dimensions as the input matrix.</param>
    /// <param name="dict">A dictionary mapping spectrum indices to lists of column indices that define contiguous regions containing
    /// outliers to be patched.</param>
    private void PatchOutliers(double[] x, Matrix<double> y, Matrix<double> yNew, SortedDictionary<int, List<int>> dict)
    {
      var yy = new double[y.ColumnCount];
      var yyNew = new double[y.ColumnCount];

      foreach (var kvp in dict)
      {
        var idxSpectrum = kvp.Key;
        var list = kvp.Value;

        for (int i = 0; i < yy.Length; i++)
        {
          yyNew[i] = yy[i] = y[idxSpectrum, i];
        }

        foreach (var range in GetContiguousRanges(list))
        {
          System.Diagnostics.Debug.WriteLine($"({idxSpectrum}, {range.lowerIdx}, {range.upperIdx}),");

          // get the next local minimum to the left side
          // local minimum is OK here because we search for really big spikes
          var leftMin = y[idxSpectrum, 0];
          var rightMin = y[idxSpectrum, y.ColumnCount - 1];
          for (int k = range.lowerIdx - 1; k > 0; --k)
          {
            if (yy[k] <= yy[k - 1] && yy[k] < yy[k + 1])
            {
              leftMin = yy[k];
              break;
            }
          }
          for (int k = range.upperIdx + 1; k < y.ColumnCount - 1; ++k)
          {
            if (yy[k] < yy[k - 1] && yy[k] <= yy[k + 1])
            {
              rightMin = yy[k];
              break;
            }
          }
          var max = double.MinValue;
          int maxIdx = 0;
          for (int k = range.lowerIdx; k <= range.upperIdx; ++k)
          {
            if (yy[k] > max)
            {
              max = yy[k];
              maxIdx = k;
            }
          }
          var prominence = max - Math.Max(leftMin, rightMin);
          var halfLevel = max - prominence / 2;

          // now search left and right for the half level
          double leftHalfPos = 0, rightHalfPos = 0;
          for (int k = maxIdx; k > 0; --k)
          {
            if (RMath.IsInIntervalCC(halfLevel, yy[k - 1], yy[k]))
            {
              leftHalfPos = k - 1 + (halfLevel - yy[k - 1]) / (yy[k] - yy[k - 1]);
              break;
            }
          }
          for (int k = maxIdx; k < y.ColumnCount - 1; ++k)
          {
            if (RMath.IsInIntervalCC(halfLevel, yy[k + 1], yy[k]))
            {
              rightHalfPos = k + (halfLevel - yy[k]) / (yy[k + 1] - yy[k]);
              break;
            }
          }
          var fwhm = rightHalfPos - leftHalfPos;
          var rangeWidth = range.upperIdx + 1 - range.lowerIdx;
          if (fwhm > rangeWidth * 1.5 && rangeWidth >= 2)
            fwhm = rangeWidth;

          if (fwhm <= MaximalWidth) // we patch the range only if we do not exeed the MaximalWidth
          {
            // refine the maximum position if possible
            double peakPositionFine = maxIdx;
            if (maxIdx > 0 && maxIdx < y.ColumnCount - 1 && yy[maxIdx - 1] < yy[maxIdx] && yy[maxIdx + 1] < yy[maxIdx])
            {
              // refine the peak position by fitting a parabola to the peak and its two neighbors, and then calculating the maximum of that parabola
              peakPositionFine -= 0.5 * (yy[maxIdx + 1] - yy[maxIdx - 1]) / (yy[maxIdx + 1] - 2 * yy[maxIdx] + yy[maxIdx - 1]);
            }

            // now for patching this, we need to points to the left and right which are not affected by the spike
            // at first, we try to get them from the FWHM of the peak
            var leftPos = (int)Math.Max(0, Math.Floor(peakPositionFine - 1 - fwhm));
            var rightPos = (int)Math.Min(yy.Length - 1, Math.Ceiling(peakPositionFine + 1 + fwhm));
            // but we also ensure that leftPos is outside the range of the outliers, and likewise rightPos, so that the outer points of the SNIP algorithm are not outlier points
            leftPos = Math.Max(0, Math.Min(leftPos, range.lowerIdx - 1));
            rightPos = Math.Min(yy.Length - 1, Math.Max(rightPos, range.upperIdx + 1));
            // now use the SNIP algorithm to patch the range
            var snip = new BaselineEvaluation.SNIP_Linear() { IsHalfWidthInXUnits = false, HalfWidth = fwhm / 2 };
            snip.Execute(new ReadOnlySpan<double>(x, leftPos, rightPos + 1 - leftPos), new ReadOnlySpan<double>(yy, leftPos, rightPos + 1 - leftPos), new Span<double>(yyNew, leftPos, rightPos + 1 - leftPos));
          }
        }

        for (int i = 0; i < yy.Length; i++)
        {
          yNew[idxSpectrum, i] = yyNew[i];
        }
      }
    }

    /// <summary>
    /// Identifies contiguous ranges of consecutive integers within the specified list.
    /// </summary>
    /// <remarks>The input list must be sorted in ascending order for correct results. Each returned tuple
    /// contains the start and end values of a contiguous sequence. If the list contains only one element, a single
    /// range is returned.</remarks>
    /// <param name="list">The list of integers to analyze for contiguous ranges. The list must contain at least one element.</param>
    /// <returns>An enumerable collection of tuples, each representing the lower and upper indices of a contiguous range of
    /// consecutive integers found in the list.</returns>
    private IEnumerable<(int lowerIdx, int upperIdx)> GetContiguousRanges(List<int> list)
    {
      int start = list[0];
      int prev = start;
      for (int i = 1; i < list.Count; ++i)
      {
        if (list[i] != prev + 1)
        {
          yield return (start, prev);
          start = list[i];
        }
        prev = list[i];
      }
      yield return (start, prev);
    }

    /// <summary>
    /// Computes robust lower and upper bounds from the provided sorted data by taking the values
    /// at <paramref name="lowerPercentile"/> and <paramref name="upperPercentile"/> and extending
    /// the interval by <paramref name="multiplier"/> standard deviations estimated from the percentile range.
    /// </summary>
    /// <param name="sorted">A sorted array of values (ascending).</param>
    /// <param name="lowerPercentile">Lower percentile (e.g. 0.10).</param>
    /// <param name="upperPercentile">Upper percentile (e.g. 0.90).</param>
    /// <param name="multiplier">Number of sigmas to extend the bounds by.</param>
    /// <returns>A tuple with the computed lower and upper bounds.</returns>
    public static (double lowerBound, double upperBound) GetPercentileBounds(
    double[] sorted,
    double lowerPercentile,  // e.g. 0.10
    double upperPercentile,  // e.g. 0.90
    double multiplier)
    {
      double pLow = GetPercentile(sorted, lowerPercentile);
      double pHigh = GetPercentile(sorted, upperPercentile);
      double range = pHigh - pLow;

      double zLow = InverseNormalCDF(lowerPercentile);  // negative
      double zHigh = InverseNormalCDF(upperPercentile);  // positive
      var sigma = range / (zHigh - zLow);
      return (pLow - multiplier * sigma, pHigh + multiplier * sigma);
    }

    /// <summary>
    /// Calculates the value at the specified percentile within a sorted array using linear interpolation.
    /// </summary>
    /// <remarks>This method uses linear interpolation between adjacent elements, matching the default
    /// behavior of Excel and NumPy percentile calculations. The input array must be pre-sorted in ascending order for
    /// correct results.</remarks>
    /// <param name="sorted">A one-dimensional array of double values, sorted in ascending order. Must not be null or empty.</param>
    /// <param name="percentile">The percentile to compute, expressed as a value between 0.0 and 1.0 inclusive. For example, 0.5 returns the
    /// median.</param>
    /// <returns>The interpolated value at the specified percentile within the sorted array.</returns>
    private static double GetPercentile(double[] sorted, double percentile)
    {
      double index = percentile * (sorted.Length - 1);
      int lower = (int)Math.Floor(index);
      int upper = (int)Math.Ceiling(index);

      if (lower == upper) return sorted[lower];

      double fraction = index - lower;
      return sorted[lower] * (1 - fraction) + sorted[upper] * fraction;
    }

    /// <summary>
    /// Computes the inverse cumulative distribution function (quantile function) of the standard normal distribution
    /// for a specified probability (Abramowitz &amp; Stegun, accurate to ~4.5e-4).
    /// </summary>
    /// <param name="p">The probability for which to compute the quantile. Must be greater than 0 and less than 1.</param>
    /// <returns>The z-score such that the probability of a standard normal random variable being less than or equal to this
    /// value is equal to the specified probability.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="p"/> is less than or equal to 0, or greater than or equal to 1.</exception>
    private static double InverseNormalCDF(double p)
    {
      if (p <= 0 || p >= 1) throw new ArgumentOutOfRangeException(nameof(p));

      bool lower = p < 0.5;
      double t = Math.Sqrt(-2.0 * Math.Log(lower ? p : 1 - p));

      const double c0 = 2.515517, c1 = 0.802853, c2 = 0.010328;
      const double d1 = 1.432788, d2 = 0.189269, d3 = 0.001308;

      double z = t - (c0 + c1 * t + c2 * t * t)
                   / (1 + d1 * t + d2 * t * t + d3 * t * t * t);

      return lower ? -z : z;
    }
  }
}
