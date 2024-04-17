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
using Altaxo.Calc;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Regression;

namespace Altaxo.Science.Signals
{
  public static class SignalMath
  {
    /// <summary>
    /// Gets the indices of the zero crossings of the signal.
    /// </summary>
    /// <param name="y">The y-values of the signal.</param>
    /// <param name="zeroCrossings">A list that can be re-used as return value.</param>
    /// <returns>List of indices, where zero crossing of the signal occurs.</returns>
    public static List<int> GetIndicesOfZeroCrossings(IReadOnlyList<double> y, List<int>? zeroCrossings = null)
    {
      if (y is null)
        throw new ArgumentNullException(nameof(y));

      if (zeroCrossings is null)
        zeroCrossings = new List<int>(y.Count / 4);
      else
        zeroCrossings.Clear();

      if (y.Count > 1)
      {
        int il = 0;
        var yl = Math.Sign(y[0]);
        for (int i = 1; i < y.Count; ++i)
        {
          var ym = Math.Sign(y[i]);
          if (ym == 0)
            continue;
          else if (ym != yl)
            zeroCrossings.Add((il + i) / 2);

          il = i;
          if (ym != 0)
          {
            yl = ym;
          }
        }
      }
      return zeroCrossings;
    }

    /// <summary>
    /// Gets the indices of the extrema (minima and maxima) of the signal
    /// </summary>
    /// <param name="y">The y-values of the signal.</param>
    /// <param name="indicesOfMinima">A list that can be recycled as the list of indices of minima. If null, a new list for the indices of minima will be allocated.</param>
    /// <param name="indicesOfMaxima">A list that can be recycled as the list of indices of maxima. If null, a new list for the indices of maxima will be allocated.</param>
    /// <returns>A tuple, which contains the list of indices of the minima and maxima of the signal. Note that the start and end of the signal can not be a minimum or maximum.</returns>
    public static (List<int> IndicesOfMinima, List<int> IndicesOfMaxima) GetIndicesOfExtrema(IReadOnlyList<double> y, List<int>? indicesOfMinima = null, List<int>? indicesOfMaxima = null)
    {
      if (y is null)
        throw new ArgumentNullException(nameof(y));

      if (indicesOfMinima is null)
        indicesOfMinima = new List<int>(y.Count / 4);
      else
        indicesOfMinima.Clear();

      if (indicesOfMaxima is null)
        indicesOfMaxima = new List<int>(y.Count / 4);
      else
        indicesOfMaxima.Clear();

      if (y.Count > 2)
      {
        int il = 1;
        var yl = Math.Sign(y[1] - y[0]);
        for (int i = 2; i < y.Count; ++i)
        {
          if (yl == 0)
          {
            continue;
          }

          var ym = Math.Sign(y[i] - y[i - 1]);
          if (ym == 0)
          {
            continue;
          }
          else if (ym != yl)
          {
            if (ym > 0)
              indicesOfMinima.Add((il + i - 1) / 2);
            else
              indicesOfMaxima.Add((il + i - 1) / 2);
          }

          il = i;
          yl = ym;
        }
      }
      return (indicesOfMinima, indicesOfMaxima);
    }

    /// <summary>
    /// Gets the index of the provided value in an array of ascending elements.
    /// </summary>
    /// <param name="xArray">The x array.</param>
    /// <param name="x">The x value that is searched.</param>
    /// <param name="roundUp">If there is no exact match, and the parameter is false, the next lower index will be returned, else if true, the index with the higher value will be returned. If null, the index for which x is closest to the element will be returned..</param>
    /// <returns>For an exact match with x, the index of x in the array. Otherwise, either the index of a value lower than x (roundUp=false), higher than x (roundUp=true), or closest to x (roundUp=null).
    /// The return value is always a valid index into the provided array.
    /// </returns>
    public static int GetIndexOfXInAscendingArray(double[] xArray, double x, bool? roundUp)
    {
      if (xArray is null)
        throw new ArgumentNullException(nameof(xArray));
      else if (xArray.Length == 0)
        throw new ArgumentException("Array is empty", nameof(xArray));

      int r = Array.BinarySearch(xArray, x);

      if (r >= 0) // found!
      {
        return r;
      }
      else // not found - result depends on the rounding parameter
      {
        r = ~r;
        var rm1 = Math.Max(0, r - 1);
        r = Math.Min(xArray.Length - 1, r);

        return roundUp switch
        {
          false => rm1,
          true => r,
          null => Math.Abs(x - xArray[rm1]) < Math.Abs(x - xArray[r]) ? rm1 : rm1,
        };
      }
    }

    /// <summary>
    /// Estimates the noise level of a signal.
    /// </summary>
    /// <param name="signal">The signal.</param>
    /// <param name="order">The order (should be odd). A order of 1 (linear) requires at least 3 points. Order of 3 requires at least 5 points. A order of 2k+1 requires at least 2k+3 points.</param>
    /// <returns>The estimated noise level of the signal.</returns>
    public static double GetNoiseLevelEstimate(double[] signal, int order)
    {
      // make the order odd
      if (order % 2 == 0)
        order += 1;

      int numberOfPoints = order + 2;
      return GetNoiseLevelEstimate(signal, numberOfPoints, order);
    }

    /// <summary>
    /// Estimates the noise level of a signal.
    /// </summary>
    /// <param name="signal">The signal.</param>
    /// <param name="numberOfPoints">The number of points used. Must be at least (order + 2).</param>
    /// <param name="order">The order (should be odd). A order of 1 (linear) requires at least 3 points. Order of 3 requires at least 5 points. A order of 2k+1 requires at least 2k+3 points.</param>
    /// <returns>The estimated noise level of the signal.</returns>
    public static double GetNoiseLevelEstimate(double[] signal, int numberOfPoints, int order)
    {
      // make the order odd
      if (order % 2 == 0)
        order += 1;

      if (!(numberOfPoints >= (order + 2)))
        throw new ArgumentOutOfRangeException(nameof(numberOfPoints), "Must be >= (order + 2)");

      var p = new SavitzkyGolayParameters() { NumberOfPoints = numberOfPoints, PolynomialOrder = order, DerivativeOrder = 0 };

      var sv = new SavitzkyGolay(p);

      var result = new double[signal.Length];
      sv.Apply(signal, result);
      double sum = 0;
      for (int i = 0; i < signal.Length; i++)
      {
        sum += RMath.Pow2(signal[i] - result[i]);
      }

      // Now, correct the sum by the coefficients of the Savitzky-Golay
      var coeff = SavitzkyGolay.GetCentralCoefficients(p);
      coeff[(coeff.Length - 1) / 2] -= 1; // this represents the difference of the actual value to the Savitzky-Golay-Value
      var scale = VectorMath.L2Norm(coeff);

      return Math.Sqrt(sum / signal.Length) / scale;
    }



    /// <summary>
    /// Gets the minimal and maximal properties of an array of x-values.
    /// </summary>
    /// <param name="array">The array of x values.</param>
    /// <returns>
    /// The (absolute value) of the minimal distance between two consecutive data points,
    /// the (absolute value) of the maximal distance between two consecutive data points,
    /// the minimal value of all elements, and
    /// the maximal value of all elements.</returns>
    public static (double minimalDistance, double maximalDistance, double minimalValue, double maximalValue) GetMinimalAndMaximalProperties(ReadOnlySpan<double> array)
    {
      double min = double.PositiveInfinity;
      double max = double.NegativeInfinity;
      double minDist = double.PositiveInfinity;
      double maxDist = double.NegativeInfinity;
      double previousX = double.NaN;
      foreach (var x in array)
      {
        var dist = Math.Abs(x - previousX);

        if (dist > 0)
        {
          minDist = Math.Min(minDist, dist);
          maxDist = Math.Max(maxDist, dist);
        }

        min = Math.Min(min, x);
        max = Math.Max(max, x);
        previousX = x;
      }
      return (minDist, maxDist, min, max);
    }

    /// <summary>
    /// Gets the mean increment of an array of values, i.e. the mean of array[i]-array[i-1].
    /// </summary>
    /// <param name="array">The array.</param>
    /// <returns>The mean value of array[i]-array[i-1].</returns>
    public static double GetMeanIncrement(IReadOnlyList<double> array)
    {
      double sum = 0;
      for (int i = 1; i < array.Count; i++)
      {
        sum += (array[i] - array[i - 1]);
      }
      return sum / (array.Count - 1);
    }
  }
}
