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

using System;
using System.Collections.Generic;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Science.Signals
{
  /// <summary>
  /// Empirical mode decomposition.
  /// </summary>
  /// <remarks>
  /// <para>Reference:</para>
  /// <para>[1] Huang, N. E., Shen, Z., Long, S. R., Wu, M. C., Shih, H. H., Zheng, Q., … Liu, H. H. (1998). The empirical mode decomposition and the Hilbert spectrum for nonlinear and non-stationary time series analysis. Proceedings of the Royal Society of London. Series A: Mathematical, Physical and Engineering Sciences, 454(1971), 903–995. https://doi.org/10.1098/rspa.1998.0193</para>
  /// </remarks>
  public class EmpiricalModeDecomposition
  {
    /// <summary>
    /// Maximum number of siftings, during which the number of extrema and the number of zero crossings remain constant, before
    /// the sifting process is stopped.
    /// </summary>
    /// <remarks>In Ref. [1], a default value of 4 is proposed for this number. Here, we have additional stopping criteria, so this number can
    /// be slightly higher.</remarks>
    public int S { get; init; } = 10;

    /// <summary>
    /// Gets the maximum number of siftings for one IMF component.
    /// </summary>
    public int MaximumNumberOfSiftingsForOneIMF { get; init; } = 2000;

    /// <summary>
    /// Gets/sets the average by difference threshold.
    /// </summary>
    /// <remarks>This value is used in conjunction with <see cref="MaximumFractionAvgByDiffThresholdExceeded"/>. See remarks there.</remarks>
    public double AvgByDiffThreshold { get; init; } = 0.05;

    /// <summary>
    /// Gets the maximum fraction of points for which the average by difference threshold can be exceeded.
    /// </summary>
    /// <remarks>If the mean value of the lower and upper envelope, divided by the difference of upper and
    /// lower envelope, exceeds the <see cref="AvgByDiffThreshold"/> for more points than this value times the total number of points,
    /// the sifting process continues until this criterion is met.</remarks>
    public double MaximumFractionAvgByDiffThresholdExceeded { get; init; } = 0.05;

    /// <summary>
    /// Gets the maximum allowed average by difference value.
    /// </summary>
    /// <remarks>If for any point of the waveform, the mean value of the lower and upper envelope, divided by the difference of upper and
    /// lower envelope, exceeds this value, the sifting process continues until this criterion is met.</remarks>
    public double MaximumAllowedAvgByDiff { get; init; } = 0.5;


    /// <summary>
    /// Extracts the intrinsic mode function components (IMFCs) of a signal by empirical mode decomposition.
    /// </summary>
    /// <param name="x">The x-values of the signal.</param>
    /// <param name="y">The y-values of the signal.</param>
    /// <param name="maximumNumberOfModesToReturn">The maximum number of modes to return from this enumeration.</param>
    /// <param name="returnRest">If true, the returned enumeration will also include the rest.</param>
    /// <returns>An endless enumeration of signals, consisting of the mode y, the residual yResidual, and representing the modes of the original signal.</returns>
    public IEnumerable<(double[] yIMFC, double[] yResidual, int modeNumber)> ExtractIntrinsicModeFunctionComponents(double[] x, double[] y, int maximumNumberOfModesToReturn, bool returnRest)
    {

      var minimalL1NormOfResidual = 1E-10 * VectorMath.L1Norm(y);
      var yResidual = (double[])y.Clone();

      for (int iMode = 0; iMode < maximumNumberOfModesToReturn; ++iMode)
      {
        // break if residual is too small
        if (VectorMath.L1Norm(yResidual) < minimalL1NormOfResidual)
        {
          yield return (yResidual, yResidual, iMode);
          break;
        }

        var (yIMFC, success) = ExtractIntrinsicModeFunctionComponent(x, yResidual);
        if (success)
        {
          for (int i = 0; i < yResidual.Length; ++i)
          {
            yResidual[i] -= yIMFC[i];
          }
          yield return (yIMFC, yResidual, iMode);
        }
        else
        {
          if (returnRest)
          {
            Array.Clear(yResidual, 0, yResidual.Length);
            yield return (yIMFC, yResidual, -1); // return the rest, with a negative mode number
          }
          break;
        }
      }
    }

    /// <summary>
    /// Extracts an intrinsic mode function component (IMF) from the signal (x, y).
    /// </summary>
    /// <param name="x">The array of x-values of the signal.</param>
    /// <param name="y">The array of y-values of the signal.</param>
    /// <returns>The next mode extracted from the signal, and an indication, if this is an IMF or the rest.</returns>
    /// <remarks>The number of sift iterations is fixed to 10 here.</remarks>
    public (double[] yIMFC, bool isIMF) ExtractIntrinsicModeFunctionComponent(ReadOnlySpan<double> x, ReadOnlySpan<double> y)
    {
      bool isIMF = true;
      var signal = y.ToArray(); // clone signal because it is changed

      List<int>? minimaIndices = null, maximaIndices = null, zeroCrossings = null;

      int? previousMinimaCount = null, previousMaximaCount = 0;
      int? minimaMaximaCountRepetitions = null;

      for (int iIteration = 0; iIteration < MaximumNumberOfSiftingsForOneIMF; ++iIteration)
      {
        (minimaIndices, maximaIndices) = SignalMath.GetIndicesOfExtrema(signal, minimaIndices, maximaIndices);
        if ((minimaIndices.Count + maximaIndices.Count) < 3) // too less minima and maxima?
        {
          isIMF = false;
          break;
        }

        zeroCrossings = SignalMath.GetIndicesOfZeroCrossings(signal, zeroCrossings);

        // can we break?
        if (Math.Abs(minimaIndices.Count + maximaIndices.Count - zeroCrossings.Count) <= 1)
        {
          if (minimaMaximaCountRepetitions.HasValue && previousMinimaCount == minimaIndices.Count && previousMaximaCount == maximaIndices.Count)
          {
            minimaMaximaCountRepetitions++;
          }
          else
          {
            minimaMaximaCountRepetitions = 0;
            previousMinimaCount = minimaIndices.Count;
            previousMaximaCount = maximaIndices.Count;
          }
        }
        else
        {
          minimaMaximaCountRepetitions = null;
        }

        if (minimaMaximaCountRepetitions >= S)
        {
          break;
        }

        if (minimaIndices.Count + maximaIndices.Count <= 1)
        {
          isIMF = false;
          break;
        }

        var proceed = SubtractMeanEnvelope(x, signal, minimaIndices, maximaIndices);
        proceed |= !(Math.Abs(minimaIndices.Count + maximaIndices.Count - zeroCrossings.Count) <= 1);
        if (!proceed)
        {
          break;
        }
      }
      return (signal, isIMF);
    }

    /// <summary>
    /// Creates interpolation data for the envelope by mirroring the extrema at the boundaries.
    /// </summary>
    /// <param name="x">The x-values of the signal.</param>
    /// <param name="y">The y-values of the signal.</param>
    /// <param name="extremaIndices">The indices of the extrema to use for interpolation.</param>
    /// <param name="alternativeExtremaIndices">The indices of the alternative extrema (used for spacing in case of single extrema).</param>
    /// <returns>The x and y values for interpolation.</returns>
    public static (double[] xs, double[] ys) CreateInterpolationDataByMirroring(ReadOnlySpan<double> x, double[] y, IReadOnlyList<int> extremaIndices, IReadOnlyList<int> alternativeExtremaIndices)
    {
      // prepare new arrays that can be used as input for the interpolations
      // of the lower envelope ...
      var xs = new double[extremaIndices.Count + 4];
      var ys = new double[extremaIndices.Count + 4];

      for (int i = 0, j = 2; i < extremaIndices.Count; ++i, ++j)
      {
        xs[j] = x[extremaIndices[i]];
        ys[j] = y[extremaIndices[i]];
      }

      xs[1] = -xs[2];
      ys[1] = ys[2];

      if (extremaIndices.Count > 1)
      {
        xs[0] = -xs[3];
        ys[0] = ys[3];
      }
      else
      {
        xs[0] = -3 * xs[2];
        ys[0] = ys[2];
      }

      xs[^2] = x[^1] * 2 - xs[^3];
      ys[^2] = ys[^3];
      if (extremaIndices.Count > 1)
      {
        xs[^1] = x[^1] * 2 - xs[^4];
        ys[^1] = ys[^4];
      }
      else
      {
        xs[^1] = x[^1] * 4 - xs[^3] * 3;
        ys[^1] = ys[^3];
      }

      return (xs, ys);
    }

    /// <summary>
    /// Creates interpolation data for the envelope by continuing the extrema trend at the boundaries.
    /// </summary>
    /// <param name="x">The x-values of the signal.</param>
    /// <param name="y">The y-values of the signal.</param>
    /// <param name="extremaIndices">The indices of the extrema to use for interpolation.</param>
    /// <param name="alternativeExtremaIndices">The indices of the alternative extrema (used for spacing in case of single extrema).</param>
    /// <returns>The x and y values for interpolation.</returns>
    public static (double[] xs, double[] ys) CreateInterpolationDataByContinuation(ReadOnlySpan<double> x, double[] y, IReadOnlyList<int> extremaIndices, IReadOnlyList<int> alternativeExtremaIndices)
    {
      // prepare new arrays that can be used as input for the interpolations
      // of the lower envelope ...
      var xs = new double[extremaIndices.Count + 4];
      var ys = new double[extremaIndices.Count + 4];

      for (int i = 0, j = 2; i < extremaIndices.Count; ++i, ++j)
      {
        xs[j] = x[extremaIndices[i]];
        ys[j] = y[extremaIndices[i]];
      }

      if (extremaIndices.Count >= 2)
      {
        // start of range

        xs[1] = xs[2] * 2 - xs[3];
        ys[1] = ys[2];
        if (extremaIndices.Count == 2)
        {
          xs[0] = xs[2] * 3 - xs[3] * 2;
          ys[0] = ys[3];
        }
        else
        {
          xs[0] = xs[2] * 2 - xs[4];
          ys[0] = ys[4];
        }

        // end of range
        xs[^2] = xs[^3] * 2 - xs[^4];
        ys[^2] = ys[^3];
        if (extremaIndices.Count == 2)
        {
          xs[^1] = xs[^3] * 3 - xs[^4] * 2;
          ys[^1] = ys[^4];
        }
        else
        {
          xs[^1] = xs[^3] * 2 - xs[^5];
          ys[^1] = ys[^5];
        }
      }
      else
      {
        var ds = x[alternativeExtremaIndices[1]] - x[alternativeExtremaIndices[0]];
        xs[1] = xs[2] - ds;
        xs[0] = xs[2] - ds * 2;
        ys[1] = ys[2];
        ys[0] = ys[2];

        xs[^2] = xs[^3] + ds;
        xs[^1] = xs[^3] + ds * 2;
        ys[^2] = ys[^3];
        ys[^1] = ys[^3];
      }

      return (xs, ys);
    }

    /// <summary>
    /// Subtracts the mean envelope from the signal <c>signal</c>.
    /// </summary>
    /// <param name="x">The x values of the signal.</param>
    /// <param name="y">The y-values of the signal. On return, this array is modified (mean envelope is subtracted).</param>
    /// <param name="minimaIndices">The minima indices of the signal.</param>
    /// <param name="maximaIndices">The maxima indices of the signal.</param>
    public bool SubtractMeanEnvelope(ReadOnlySpan<double> x, double[] y, IReadOnlyList<int> minimaIndices, IReadOnlyList<int> maximaIndices)
    {
      if (minimaIndices.Count == 1 && maximaIndices.Count == 1)
      {
        SubtractMeanEnvelope_1_1(x, y, minimaIndices[0], maximaIndices[0]);
        return true;
      }

      var (xs, ys) = CreateInterpolationDataByMirroring(x, y, minimaIndices, maximaIndices);
      var interpolationLowerEnvelope = Altaxo.Calc.Interpolation.CubicSpline.InterpolateNaturalSorted(xs, ys);

      (xs, ys) = CreateInterpolationDataByMirroring(x, y, maximaIndices, minimaIndices);
      var interpolationUpperEnvelope = Altaxo.Calc.Interpolation.CubicSpline.InterpolateNaturalSorted(xs, ys);

      // subtract from the signal the mean of the lower and upper envelope
      int numberOfThreshold1Exceeds = 0;
      bool threshold2Exceeded = false;
      for (int i = 0; i < y.Length; ++i)
      {
        double xv = x[i];
        var lo = interpolationLowerEnvelope.Interpolate(xv);
        var up = interpolationUpperEnvelope.Interpolate(xv);

        var avg = 0.5 * (lo + up); // average of lower and upper envelope
        var dif = 0.5 * (up - lo); // difference of upper and lower envelope
        var quo = Math.Abs(avg / dif); // quotient of both
        numberOfThreshold1Exceeds += quo > AvgByDiffThreshold ? 1 : 0;
        threshold2Exceeded |= quo > MaximumAllowedAvgByDiff;

        y[i] -= avg;
      }

      bool proceed = numberOfThreshold1Exceeds > MaximumFractionAvgByDiffThresholdExceeded * y.Length || threshold2Exceeded;
      return proceed;
    }

    /// <summary>
    /// Subtracts the mean envelope from the signal in the special case where there is exactly one minimum and one maximum.
    /// </summary>
    /// <param name="x">The x values of the signal.</param>
    /// <param name="signal">The y-values of the signal. On return, this array is modified (mean envelope is subtracted).</param>
    /// <param name="minIndex">The index of the minimum.</param>
    /// <param name="maxIndex">The index of the maximum.</param>
    public static void SubtractMeanEnvelope_1_1(ReadOnlySpan<double> x, double[] signal, int minIndex, int maxIndex)
    {
      var x0 = x[minIndex];
      var y0 = signal[minIndex];
      var x1 = x[maxIndex];
      var y1 = signal[maxIndex];

      for (int i = 0; i < x.Length; ++i)
      {
        var r = (x[i] - x0) / (x1 - x0);
        var y = (1 - r) * y0 + r * y1;
        signal[i] -= y;
      }
    }
  }
}
