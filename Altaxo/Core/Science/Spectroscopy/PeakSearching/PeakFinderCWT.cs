#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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
  /// Peak finder using continuous wavelet transformation.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>[1] P.Du, W.A. Kibbe, S.M.Lin, "Improved peak detection in mass spectrum by incorporating continuous wavelet transform-based pattern matching", Bioinformatics (2006) 22 (17): pp. 2059-2065, doi: 10.1093/bioinformatics/btl355</para>
  /// </remarks>
  public partial class PeakFinderCWT
  {
    // https://github.com/scipy/scipy/blob/main/scipy/signal/_peak_finding.py
   

    /// <summary>
    /// Ricker wavelet function, also known as the "Mexican hat wavelet".
    /// </summary>
    /// <param name="x">The argument x.</param>
    /// <param name="w">The width of the wavelet.</param>
    /// <returns>The function value for the argument <paramref name="x"/>.</returns>
    /// <remarks>
    /// <para>It models the function:</para>
    /// <code>
    /// A* (1 - (x/a)**2) * exp(-0.5*(x/a)**2),
    /// where A = 2/(sqrt(3*a)*(pi**0.25)).
    /// </code>
    /// </remarks>
    public static double Ricker(double x, double w)
    {
      const double Prefactor = 0.86732507058407751832; // 2 / (Pi^0.25 * Sqrt(3))
      var xw = x / w;
      var sqrxw = xw * xw;
      return Prefactor * (1 - sqrxw) * Math.Exp(-0.5 * sqrxw) / Math.Sqrt(w);
    }

    /// <summary>
    /// Normalized 2nd derivative of the Ricker wavelet function.
    /// </summary>
    /// <param name="x">The argument x.</param>
    /// <param name="w">The width of the wavelet.</param>
    /// <returns>The function value for the argument <paramref name="x"/>.</returns>
    /// <remarks>
    /// <para>It models the function:</para>
    /// <code>
    /// A* (3 - 6*(x/a)**2 + (x/a)**4) * exp(-0.5*(x/a)**2),
    /// where A = 4/(sqrt(105*a)*(pi**0.25)).
    /// </code>
    /// </remarks>
    public static double Ricker2ndDerivative(double x, double w)
    {
      const double Prefactor = 0.29320938945473762851; // 4 / (Pi^0.25 * Sqrt(105))
      var xw = x / w;
      var sqrxw = xw * xw;
      return Prefactor * (3 - 6*sqrxw + sqrxw*sqrxw) * Math.Exp(-0.5 * sqrxw) / Math.Sqrt(w);
    }

    /// <summary>
    /// Gets a boolean array, in which local extrema in a spectrum are marked as true.
    /// </summary>
    /// <param name="data">The data array.</param>
    /// <param name="comparator">The comparator. To find maxima, use the greater operator (<c>(x,y)=>x&gt;y</c>)</param>
    /// <param name="order">The order. Must at least 1. The order are the number of points to the left and right of the point under consideration, that must fullfill the comparator condition.</param>
    /// <returns>An array in which points where a local extrema was found are marked with true.</returns>
    public static bool[] GetRelativeExtrema(
      IReadOnlyList<double> data,
      Func<double, double, bool> comparator,
      int order = 1
      )
    {
      var datalen = data.Count;
      var result = Enumerable.Repeat(true, datalen).ToArray();

      for (int i = 0; i < datalen; ++i)
      {
        bool iResult = result[i];
        for (int j = 1; iResult && j <= order; ++j)
        {
          if ((i + j) < datalen)
            iResult &= comparator(data[i], data[i + j]);
          if ((i - j) >= 0)
            iResult &= comparator(data[i], data[i - j]);
        }
        result[i] = iResult;
      }
      return result;
    }

    /// <summary>
    /// Gets a boolean matrix of the same dimensions as the provided <paramref name="data"/> matrix,
    /// in which the points at which a relative extrema is found, are set to true.
    /// </summary>
    /// <param name="data">The data matrix.</param>
    /// <param name="comparator">The comparator function.</param>
    /// <param name="axis">The axis which is iterated over. 0: for each column, the maxima of the row values will be evaluated.
    /// 1: for each row, the maxima of the column values will be evaluated.</param>
    /// <param name="order">The number of points to compare to the left and right of the considered point Must be &gt;=1.</param>
    /// <returns></returns>
    /// <exception cref="System.NotImplementedException"></exception>
    public static bool[,] GetRelativeExtrema(
      IROMatrix<double> data,
      Func<double, double, bool> comparator,
      int axis = 0,
      int order = 1
      )
    {
      var result = new bool[data.RowCount, data.ColumnCount];
      for (int r = 0; r < data.RowCount; ++r)
        for (int c = 0; c < data.ColumnCount; ++c)
          result[r, c] = true;

      switch (axis)
      {
        case 0:
          {
            var datalen = data.RowCount;
            for (int c = 0; c < data.ColumnCount; ++c)
            {
              for (int i = 0; i < datalen; ++i)
              {
                bool iResult = result[i, c];
                for (int j = 1; iResult && j <= order; ++j)
                {
                  if ((i + j) < datalen)
                    iResult &= comparator(data[i, c], data[i + j, c]);
                  if ((i - j) >= 0)
                    iResult &= comparator(data[i, c], data[i - j, c]);
                }
                result[i, c] = iResult;
              }
            }
          }
          break;

        case 1:
          {
            var datalen = data.ColumnCount;
            for (int r = 0; r < data.RowCount; ++r)
            {
              for (int i = 0; i < datalen; ++i)
              {
                bool iResult = result[r, i];
                for (int j = 1; iResult && j <= order; ++j)
                {
                  if ((i + j) < datalen)
                    iResult &= comparator(data[r, i], data[r, i + j]);
                  if ((i - j) >= 0)
                    iResult &= comparator(data[r, i], data[r, i - j]);
                }
                result[r, i] = iResult;
              }
            }
          }
          break;

        default:
          throw new NotImplementedException();
      }
      return result;
    }


    private class ActiveRidgeLine
    {
      public RidgeLine Line = new();
      public int Column;
      public int GapNumber;
      public ActiveRidgeLine(int r, int c, double cwtCoefficient)
      {
        Line.Add((r, c, cwtCoefficient));
        Column = c;
        GapNumber = 0;
      }
    }

    private static int InsertSorted(List<ActiveRidgeLine> l, ActiveRidgeLine r)
    {
      for (int i = 0; i < l.Count; ++i)
      {
        if (r.Column <= l[i].Column)
        {
          l.Insert(i, r);
          return i;
        }
      }
      l.Add(r);
      return l.Count - 1;
    }



    /// <summary>
    /// Identifies the ridge lines in a continous wavelet transformation matrix
    /// </summary>
    /// <param name="matr">The continuous wavelet transformation (Cwt) matrix. Each row in the matrix represents the transformation for one width.
    /// It is assumed that the width increases with increasing row number.</param>
    /// <param name="max_distances">A function, using the row number as argument, and returning the maximal distance (in points) that is allowed
    /// between the current x value of the ridge line and the x-position of a maximum identified in the Cwt matrix.</param>
    /// <param name="maximalGap">The maximal number of rows that is allowed to bridge between the end of a ridge line and a maximum in the Cwt matrix.</param>
    /// <returns>List of ridge lines. Each ridge line is represented by a list of tuples (row number, column number, and Cwt coefficient).</returns>
    public static List<RidgeLine> IdentifyRidgeLines(
      IROMatrix<double> matr,
      Func<int, int> max_distances,
      int maximalGap)
    {
      // Create a boolean matrix of same shape as matr, which is true at places with local extrema in a row
      var all_max_cols = GetRelativeExtrema(matr, (x, y) => x > y, axis: 1, order: 1);


      var activeRidgeLines = new List<ActiveRidgeLine>();
      var finalizedRidgeLines = new List<RidgeLine>();

      int rowCount = matr.RowCount;
      int columnCount = matr.ColumnCount;

      for (int idxRow = rowCount - 1; idxRow >= 0; --idxRow)
      {
        var max_distance = max_distances(idxRow);
        int startIndexRidgeLineSearch = 0;
        for (int idxCol = 0; idxCol < columnCount; ++idxCol)
        {
          if (all_max_cols[idxRow, idxCol]) // if there is a local maxima at this place
          {
            bool isNewRidge = true; // assume it is the start of a new ridge, until we prove it is part of an existing ridge

            for (int l = startIndexRidgeLineSearch; l < activeRidgeLines.Count; ++l)
            {
              if ((Math.Abs(activeRidgeLines[l].Column - idxCol) <= max_distance) &&
                   ((l + 1) >= activeRidgeLines.Count || Math.Abs(activeRidgeLines[l + 1].Column - idxCol) > Math.Abs(activeRidgeLines[l].Column - idxCol))
                 )
              {
                // we have a match between the ridge and the current maximum
                activeRidgeLines[l].GapNumber = 0; // reset gap number
                activeRidgeLines[l].Column = idxCol; // update the actual column number of this ridge
                activeRidgeLines[l].Line.Add((idxRow, idxCol, matr[idxRow, idxCol])); // add this point to the ridge line
                startIndexRidgeLineSearch = l + 1; // for the next maximum, we need to search only ridges with column numbers greater than this one
                isNewRidge = false; // the current position is not the start of a new ridge
                break;
              }
              else if (activeRidgeLines[l].Column < idxCol)
              {
                // For all lines to the left of the current position, that have no match
                // with the current maximum position, we increase GapNumber by one
                activeRidgeLines[l].GapNumber++;
                startIndexRidgeLineSearch = l + 1; // for the next maximum, we need to search only ridges with column numbers greater than this one
              }
              else if (activeRidgeLines[l].Column > (idxCol + max_distance))
              {
                break; // further search to the right is not necessary
              }
            }

            if (isNewRidge)
            {
              var idxInsertPosition = InsertSorted(activeRidgeLines, new ActiveRidgeLine(idxRow, idxCol, matr[idxRow, idxCol]));
              startIndexRidgeLineSearch = idxInsertPosition + 1;
            }
          }
        }

        // put ridge lines for which gap thresh is exceeded into the list of finalized lines
        for (int i = activeRidgeLines.Count - 1; i >= 0; --i)
        {
          if (activeRidgeLines[i].GapNumber > maximalGap)
          {
            finalizedRidgeLines.Add(activeRidgeLines[i].Line);
            activeRidgeLines.RemoveAt(i);
          }
        }
      }

      // put the last active lines also to finalized lines
      for (int i = activeRidgeLines.Count - 1; i >= 0; --i)
      {
        finalizedRidgeLines.Add(activeRidgeLines[i].Line);
      }

      return finalizedRidgeLines;
    }

    public static double ScoreAtPercentile(List<double> arr, double percentile)
    {
      if (!(percentile >= 0 && percentile <= 1))
        throw new ArgumentOutOfRangeException("Percentil must be >=0 && <=1");

      arr.Sort();
      var di = (arr.Count - 1) * percentile;
      var i = (int)di;
      if (i == arr.Count - 1)
      {
        return arr[arr.Count - 1];
      }
      else
      {
        var r = di - i;
        return r * arr[i + 1] + (1 - r) * arr[i];
      }
    }

    /// <summary>
    /// Gets the noise level along the x-axis.
    /// </summary>
    /// <param name="cwt">The matrix with the continuous wavelet transformation. Only the first row (stage 0) of that matrix is used for evaluating the noise.</param>
    /// <param name="window_size">Size of the sliding window to evaluate the noise.</param>
    /// <param name="noise_perc">The percentile (but given in value from 0..1) at which the noise level is evaluated.</param>
    /// <returns>Array with a noise level for each point of the spectrum.</returns>
    public static double[] GetNoiseLevel(IROMatrix<double> cwt, int? window_size = null, double noise_perc = 0.5)
    {
      var num_points = cwt.ColumnCount;

      var noises = new double[num_points];

      window_size ??= (int)Math.Ceiling((double)num_points / 20);
      var hf_window = window_size.Value / 2;
      var odd = window_size.Value % 2;


      var windowPoints = new List<double>();
      for (int c = 0; c < num_points; ++c)
      {
        var window_start = Math.Max(0, c - hf_window);
        var window_end = Math.Min(c + hf_window + odd, num_points);
        windowPoints.Clear();
        for (int i = window_start; i < window_end; ++i)
        {
          windowPoints.Add(Math.Abs(cwt[0, i]));
        }
        noises[c] = ScoreAtPercentile(windowPoints, noise_perc);
      }
      return noises;
    }

    /// <summary>
    /// Filter ridge lines according to prescribed criteria. Intended to be used for finding relative maxima.
    /// </summary>
    /// <param name="cwt">Continuous wavelet transform from which the <paramref name="ridge_lines"/> were defined.</param>
    /// <param name="ridge_lines">The ridge lines. Each line consist of a list of tuples (row, column).</param>
    /// <param name="window_sizen">Size of window to use to calculate noise floor.  Default is cwt.NumberOfColumns/20.</param>
    /// <param name="min_lengthn">Minimum length a ridge line needs to be acceptable.  Default is cwt.NumberOfRows / 4.</param>
    /// <param name="min_snr">Minimum SNR ratio. Default 1. The signal is the value of
    /// the cwt matrix at the shortest length scale(``cwt[0, loc]``), the
    /// noise is the `noise_perc`th percentile of datapoints contained within a
    /// window of `window_size` around ``cwt[0, loc]``.</param>
    /// <param name="noise_perc">When calculating the noise floor, percentile of data points (0..1) examined below which to consider noise. Calculated using scoreatpercentile. Default value: 0.1</param>
    /// <returns>Only those ridge lines that match the criteria.</returns>
    public static List<RidgeLine> FilterRidgeLines(
      IROMatrix<double> cwt,
      List<RidgeLine> ridge_lines,
      int? window_sizen = null,
      int? min_lengthn = null,
                        double min_snr = 1,
                        double noise_perc = 0.5)
    {

      var num_points = cwt.ColumnCount;

      var min_length = min_lengthn ?? (int)Math.Ceiling((double)cwt.RowCount / 4);
      var window_size = window_sizen ?? (int)Math.Ceiling((double)num_points / 20);

      var hf_window = window_size / 2;
      var odd = window_size % 2;

      // Filter based on SNR
      // Noise level is defined as the 'noise_perc' quantile of the absolute values of the Cwt coefficients at level 0
      var noises = new double[cwt.ColumnCount];
      var windowPoints = new List<double>();
      for (int c = 0; c < cwt.ColumnCount; ++c)
      {
        var window_start = Math.Max(0, c - hf_window);
        var window_end = Math.Min(c + hf_window + odd, num_points);
        windowPoints.Clear();
        for (int i = window_start; i < window_end; ++i)
        {
          windowPoints.Add(Math.Abs(cwt[0, i]));
        }
        noises[c] = ScoreAtPercentile(windowPoints, noise_perc);
      }

      bool filt_func(List<(int Row, int Column, double CwtCoefficient)> line)
      {
        if (line.Count < min_length)
        {
          return false; // ridge line too short
        }
        var snr = Math.Abs(cwt[line[line.Count-1].Row, line[line.Count-1].Column] / noises[line[line.Count-1].Row]);
        if (snr < min_snr)
        {
          return false;
        }
        return true;
      }

      return ridge_lines.Where(rline => filt_func(rline)).ToList();
    }


    /// <summary>
    /// Find peaks in a 1-D array with wavelet transformation.
    /// The general approach is to smooth `vector` by convolving it with
    /// wavelet(width) for each width in widths. Relative maxima which
    /// appear at enough length scales, and with sufficiently high SNR, are
    /// accepted.
    /// </summary>
    /// <param name="vector">1-D array in which to find the peaks.</param>
    /// <param name="widths">Enumeration of widths to use for calculating
    /// the CWT matrix. In general, this range should cover the expected width of peaks of interest.</param>
    /// <param name="wavelet">Should take two parameters and return a 1-D array to convolve
    /// with `vector`. The first parameter determines the number of points
    /// of the returned wavelet array, the second parameter is the scale
    /// (`width`) of the wavelet.Should be normalized and symmetric.
    ///  Default is the ricker wavelet.</param>
    /// <param name="max_distances">At each row, a ridge line is only connected if the relative max at
    /// row[n] is within ``max_distances[n]`` from the relative max at
    /// ``row[n + 1]``.  Default value is ``widths/4``.</param>
    /// <param name="gap_thresh">If a relative maximum is not found within `max_distances`,
    /// there will be a gap.A ridge line is discontinued if there are more
    /// than `gap_thresh` points without connecting a new relative maximum.
    /// Default is the first value of the widths array i.e.widths[0].</param>
    /// <returns>A tuple: all ridge lines that were found, and the matrix of Cwt coefficients. The list of ridge lines is not sorted.
    /// Use a filter function (e.g. <see cref="FilterRidgeLines"/>) in order to filter out the not essential lines.
    /// </returns>
    public static (List<RidgeLine> RidgeLines, double[,] CwtMatrix) Execute(double[] vector,
                        IEnumerable<double> widths,
                        Func<double, double, double> wavelet = null,
                        Func<int, int>? max_distances = null,
                        double? gap_thresh = null)
    {
      var widthsA = widths.ToArray();
      gap_thresh ??= Math.Ceiling(widthsA[0]);
      max_distances ??= (idx) => (int)Math.Ceiling(widthsA[idx] / 4.0);
      wavelet ??= Ricker;

      var cwtMatrix = ContinuousWaveletTransformation(vector, wavelet, widths);
      var ridge_lines = IdentifyRidgeLines(cwtMatrix.ToROMatrix(), max_distances, (int)gap_thresh);
      return (ridge_lines, cwtMatrix);
    }


    /// <summary>
    /// Executes a continuous wavelet transform over the data <paramref name="data"/>.
    /// </summary>
    /// <param name="data">The data to transform.</param>
    /// <param name="wavelet">The wavelet function. First parameter is the x value of the function, second parameter is the width.</param>
    /// <param name="widths">Enumeration of widths to evaluate. The values should be increasing.</param>
    /// <returns>A matrix in which each row represents the waveform transformation at a certain width.</returns>
    public static double[,] ContinuousWaveletTransformation(double[] data, Func<double, double, double> wavelet, IEnumerable<double> widths)
    {
      var result = new double[widths.Count(), data.Length];
      var scratch = new double[0];
      var res = new double[0];
      var wavelet_data = new double[0];

      int r = 0;
      foreach (var width in widths)
      {
        var N = Altaxo.Calc.BinaryMath.NextPowerOfTwoGreaterOrEqualThan((int)(data.Length + 10 * width));
        var N2 = N / 2;

        // Fill array with wavelet data...
        if (N != wavelet_data.Length)
        {
          wavelet_data = new double[N];
        }
        for (int i = 0; i < N; i++)
        {
          var x = i < N2 ? i : i - N;
          wavelet_data[i] = wavelet(x, width);
        }

        // Fill scratch array with data to transform, and make some special filling at the end of the array
        // in order to avoid fringe effects
        if (N != scratch.Length)
        {
          scratch = new double[N];
          res = new double[N];
          Array.Copy(data, 0, scratch, 0, data.Length);
          var m = (scratch.Length + data.Length) / 2;

          // if data has a bias, filling the rest of the array with zeros would lead to big amplitudes at the start and the end of the resulting transformation
          // in order to avoid that, we fill the rest of array first with data from the end, then with data from the start
          // TODO: it would be even better to evaluate the trend at the start of data and the end of data, and continue the trend at the rest of the scratch array. 
          var v1 = data[data.Length - 1];
          for (int i = data.Length; i < m; ++i)
            scratch[i] = v1;
          var v0 = data[0];
          for (int i = m; i < scratch.Length; ++i)
            scratch[i] = v0;
        }
        Altaxo.Calc.Fourier.FastHartleyTransform.CyclicRealConvolution(scratch, wavelet_data, res, N);

        for (int c = 0; c < data.Length; c++)
          result[r, c] = res[c];
        ++r;
      }

      return result;
    }

  }
}
