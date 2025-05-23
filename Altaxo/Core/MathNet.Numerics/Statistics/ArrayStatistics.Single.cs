﻿// <copyright file="ArrayStatistics.Single.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
//
// Copyright (c) 2009-2015 Math.NET
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

using System;

namespace Altaxo.Calc.Statistics
{
  public static partial class ArrayStatistics
  {
    /// <summary>
    /// Returns the smallest value from the unsorted data array.
    /// Returns NaN if data is empty or any entry is NaN.
    /// </summary>
    /// <param name="data">Sample array, no sorting is assumed.</param>
    public static float Minimum(float[] data)
    {
      if (data.Length == 0)
      {
        return float.NaN;
      }

      var min = float.PositiveInfinity;
      for (int i = 0; i < data.Length; i++)
      {
        if (data[i] < min || float.IsNaN(data[i]))
        {
          min = data[i];
        }
      }

      return min;
    }

    /// <summary>
    /// Returns the smallest value from the unsorted data array.
    /// Returns NaN if data is empty or any entry is NaN.
    /// </summary>
    /// <param name="data">Sample array, no sorting is assumed.</param>
    public static float Maximum(float[] data)
    {
      if (data.Length == 0)
      {
        return float.NaN;
      }

      var max = float.NegativeInfinity;
      for (int i = 0; i < data.Length; i++)
      {
        if (data[i] > max || float.IsNaN(data[i]))
        {
          max = data[i];
        }
      }

      return max;
    }

    /// <summary>
    /// Returns the smallest absolute value from the unsorted data array.
    /// Returns NaN if data is empty or any entry is NaN.
    /// </summary>
    /// <param name="data">Sample array, no sorting is assumed.</param>
    public static float MinimumAbsolute(float[] data)
    {
      if (data.Length == 0)
      {
        return float.NaN;
      }

      float min = float.PositiveInfinity;
      for (int i = 0; i < data.Length; i++)
      {
        if (Math.Abs(data[i]) < min || float.IsNaN(data[i]))
        {
          min = Math.Abs(data[i]);
        }
      }

      return min;
    }

    /// <summary>
    /// Returns the largest absolute value from the unsorted data array.
    /// Returns NaN if data is empty or any entry is NaN.
    /// </summary>
    /// <param name="data">Sample array, no sorting is assumed.</param>
    public static float MaximumAbsolute(float[] data)
    {
      if (data.Length == 0)
      {
        return float.NaN;
      }

      float max = 0.0f;
      for (int i = 0; i < data.Length; i++)
      {
        if (Math.Abs(data[i]) > max || float.IsNaN(data[i]))
        {
          max = Math.Abs(data[i]);
        }
      }

      return max;
    }

    /// <summary>
    /// Estimates the arithmetic sample mean from the unsorted data array.
    /// Returns NaN if data is empty or any entry is NaN.
    /// </summary>
    /// <param name="data">Sample array, no sorting is assumed.</param>
    public static double Mean(float[] data)
    {
      if (data.Length == 0)
      {
        return double.NaN;
      }

      double mean = 0;
      ulong m = 0;
      for (int i = 0; i < data.Length; i++)
      {
        mean += (data[i] - mean) / ++m;
      }

      return mean;
    }

    /// <summary>
    /// Evaluates the geometric mean of the unsorted data array.
    /// Returns NaN if data is empty or any entry is NaN.
    /// </summary>
    /// <param name="data">Sample array, no sorting is assumed.</param>
    public static double GeometricMean(float[] data)
    {
      if (data.Length == 0)
      {
        return double.NaN;
      }

      double sum = 0;
      for (int i = 0; i < data.Length; i++)
      {
        sum += Math.Log(data[i]);
      }

      return Math.Exp(sum / data.Length);
    }

    /// <summary>
    /// Evaluates the harmonic mean of the unsorted data array.
    /// Returns NaN if data is empty or any entry is NaN.
    /// </summary>
    /// <param name="data">Sample array, no sorting is assumed.</param>
    public static double HarmonicMean(float[] data)
    {
      if (data.Length == 0)
      {
        return double.NaN;
      }

      double sum = 0;
      for (int i = 0; i < data.Length; i++)
      {
        sum += 1.0 / data[i];
      }

      return data.Length / sum;
    }

    /// <summary>
    /// Estimates the unbiased population variance from the provided samples as unsorted array.
    /// On a dataset of size N will use an N-1 normalizer (Bessel's correction).
    /// Returns NaN if data has less than two entries or if any entry is NaN.
    /// </summary>
    /// <param name="samples">Sample array, no sorting is assumed.</param>
    public static double Variance(float[] samples)
    {
      if (samples.Length <= 1)
      {
        return double.NaN;
      }

      double variance = 0;
      double t = samples[0];
      for (int i = 1; i < samples.Length; i++)
      {
        t += samples[i];
        double diff = ((i + 1) * samples[i]) - t;
        variance += (diff * diff) / ((i + 1.0) * i);
      }

      return variance / (samples.Length - 1);
    }

    /// <summary>
    /// Evaluates the population variance from the full population provided as unsorted array.
    /// On a dataset of size N will use an N normalizer and would thus be biased if applied to a subset.
    /// Returns NaN if data is empty or if any entry is NaN.
    /// </summary>
    /// <param name="population">Sample array, no sorting is assumed.</param>
    public static double PopulationVariance(float[] population)
    {
      if (population.Length == 0)
      {
        return double.NaN;
      }

      double variance = 0;
      double t = population[0];
      for (int i = 1; i < population.Length; i++)
      {
        t += population[i];
        double diff = ((i + 1) * population[i]) - t;
        variance += (diff * diff) / ((i + 1.0) * i);
      }

      return variance / population.Length;
    }

    /// <summary>
    /// Estimates the unbiased population standard deviation from the provided samples as unsorted array.
    /// On a dataset of size N will use an N-1 normalizer (Bessel's correction).
    /// Returns NaN if data has less than two entries or if any entry is NaN.
    /// </summary>
    /// <param name="samples">Sample array, no sorting is assumed.</param>
    public static double StandardDeviation(float[] samples)
    {
      return Math.Sqrt(Variance(samples));
    }

    /// <summary>
    /// Evaluates the population standard deviation from the full population provided as unsorted array.
    /// On a dataset of size N will use an N normalizer and would thus be biased if applied to a subset.
    /// Returns NaN if data is empty or if any entry is NaN.
    /// </summary>
    /// <param name="population">Sample array, no sorting is assumed.</param>
    public static double PopulationStandardDeviation(float[] population)
    {
      return Math.Sqrt(PopulationVariance(population));
    }

    /// <summary>
    /// Estimates the arithmetic sample mean and the unbiased population variance from the provided samples as unsorted array.
    /// On a dataset of size N will use an N-1 normalizer (Bessel's correction).
    /// Returns NaN for mean if data is empty or any entry is NaN and NaN for variance if data has less than two entries or if any entry is NaN.
    /// </summary>
    /// <param name="samples">Sample array, no sorting is assumed.</param>
    public static (double Mean, double Variance) MeanVariance(float[] samples)
    {
      return (Mean(samples), Variance(samples));
    }

    /// <summary>
    /// Estimates the arithmetic sample mean and the unbiased population standard deviation from the provided samples as unsorted array.
    /// On a dataset of size N will use an N-1 normalizer (Bessel's correction).
    /// Returns NaN for mean if data is empty or any entry is NaN and NaN for standard deviation if data has less than two entries or if any entry is NaN.
    /// </summary>
    /// <param name="samples">Sample array, no sorting is assumed.</param>
    public static (double Mean, double StandardDeviation) MeanStandardDeviation(float[] samples)
    {
      return (Mean(samples), StandardDeviation(samples));
    }

    /// <summary>
    /// Estimates the unbiased population covariance from the provided two sample arrays.
    /// On a dataset of size N will use an N-1 normalizer (Bessel's correction).
    /// Returns NaN if data has less than two entries or if any entry is NaN.
    /// </summary>
    /// <param name="samples1">First sample array.</param>
    /// <param name="samples2">Second sample array.</param>
    public static double Covariance(float[] samples1, float[] samples2)
    {
      if (samples1.Length != samples2.Length)
      {
        throw new ArgumentException("All vectors must have the same dimensionality.");
      }

      if (samples1.Length <= 1)
      {
        return double.NaN;
      }

      double mean1 = Mean(samples1);
      double mean2 = Mean(samples2);
      double covariance = 0.0;
      for (int i = 0; i < samples1.Length; i++)
      {
        covariance += (samples1[i] - mean1) * (samples2[i] - mean2);
      }

      return covariance / (samples1.Length - 1);
    }

    /// <summary>
    /// Evaluates the population covariance from the full population provided as two arrays.
    /// On a dataset of size N will use an N normalizer and would thus be biased if applied to a subset.
    /// Returns NaN if data is empty or if any entry is NaN.
    /// </summary>
    /// <param name="population1">First population array.</param>
    /// <param name="population2">Second population array.</param>
    public static double PopulationCovariance(float[] population1, float[] population2)
    {
      if (population1.Length != population2.Length)
      {
        throw new ArgumentException("All vectors must have the same dimensionality.");
      }

      if (population1.Length == 0)
      {
        return double.NaN;
      }

      double mean1 = Mean(population1);
      double mean2 = Mean(population2);
      double covariance = 0.0;
      for (int i = 0; i < population1.Length; i++)
      {
        covariance += (population1[i] - mean1) * (population2[i] - mean2);
      }

      return covariance / population1.Length;
    }

    /// <summary>
    /// Estimates the root mean square (RMS) also known as quadratic mean from the unsorted data array.
    /// Returns NaN if data is empty or any entry is NaN.
    /// </summary>
    /// <param name="data">Sample array, no sorting is assumed.</param>
    public static double RootMeanSquare(float[] data)
    {
      if (data.Length == 0)
      {
        return double.NaN;
      }

      double mean = 0;
      ulong m = 0;
      for (int i = 0; i < data.Length; i++)
      {
        mean += (data[i] * data[i] - mean) / ++m;
      }

      return Math.Sqrt(mean);
    }

    /// <summary>
    /// Returns the order statistic (order 1..N) from the unsorted data array.
    /// WARNING: Works inplace and can thus causes the data array to be reordered.
    /// </summary>
    /// <param name="data">Sample array, no sorting is assumed. Will be reordered.</param>
    /// <param name="order">One-based order of the statistic, must be between 1 and N (inclusive).</param>
    public static float OrderStatisticInplace(float[] data, int order)
    {
      if (order < 1 || order > data.Length)
      {
        return float.NaN;
      }

      if (order == 1)
      {
        return Minimum(data);
      }

      if (order == data.Length)
      {
        return Maximum(data);
      }

      return SelectInplace(data, order - 1);
    }

    /// <summary>
    /// Estimates the median value from the unsorted data array.
    /// WARNING: Works inplace and can thus causes the data array to be reordered.
    /// </summary>
    /// <param name="data">Sample array, no sorting is assumed. Will be reordered.</param>
    public static float MedianInplace(float[] data)
    {
      var k = data.Length / 2;
      return data.Length.IsOdd()
          ? SelectInplace(data, k)
          : (SelectInplace(data, k - 1) + SelectInplace(data, k)) / 2.0f;
    }

    /// <summary>
    /// Estimates the p-Percentile value from the unsorted data array.
    /// If a non-integer Percentile is needed, use Quantile instead.
    /// Approximately median-unbiased regardless of the sample distribution (R8).
    /// WARNING: Works inplace and can thus causes the data array to be reordered.
    /// </summary>
    /// <param name="data">Sample array, no sorting is assumed. Will be reordered.</param>
    /// <param name="p">Percentile selector, between 0 and 100 (inclusive).</param>
    public static float PercentileInplace(float[] data, int p)
    {
      return QuantileInplace(data, p / 100.0d);
    }

    /// <summary>
    /// Estimates the first quartile value from the unsorted data array.
    /// Approximately median-unbiased regardless of the sample distribution (R8).
    /// WARNING: Works inplace and can thus causes the data array to be reordered.
    /// </summary>
    /// <param name="data">Sample array, no sorting is assumed. Will be reordered.</param>
    public static float LowerQuartileInplace(float[] data)
    {
      return QuantileInplace(data, 0.25d);
    }

    /// <summary>
    /// Estimates the third quartile value from the unsorted data array.
    /// Approximately median-unbiased regardless of the sample distribution (R8).
    /// WARNING: Works inplace and can thus causes the data array to be reordered.
    /// </summary>
    /// <param name="data">Sample array, no sorting is assumed. Will be reordered.</param>
    public static float UpperQuartileInplace(float[] data)
    {
      return QuantileInplace(data, 0.75d);
    }

    /// <summary>
    /// Estimates the inter-quartile range from the unsorted data array.
    /// Approximately median-unbiased regardless of the sample distribution (R8).
    /// WARNING: Works inplace and can thus causes the data array to be reordered.
    /// </summary>
    /// <param name="data">Sample array, no sorting is assumed. Will be reordered.</param>
    public static float InterquartileRangeInplace(float[] data)
    {
      return QuantileInplace(data, 0.75d) - QuantileInplace(data, 0.25d);
    }

    /// <summary>
    /// Estimates {min, lower-quantile, median, upper-quantile, max} from the unsorted data array.
    /// Approximately median-unbiased regardless of the sample distribution (R8).
    /// WARNING: Works inplace and can thus causes the data array to be reordered.
    /// </summary>
    /// <param name="data">Sample array, no sorting is assumed. Will be reordered.</param>
    public static float[] FiveNumberSummaryInplace(float[] data)
    {
      if (data.Length == 0)
      {
        return new[] { float.NaN, float.NaN, float.NaN, float.NaN, float.NaN };
      }

      // TODO: Benchmark: is this still faster than sorting the array then using SortedArrayStatistics instead?
      return new[] { Minimum(data), QuantileInplace(data, 0.25d), MedianInplace(data), QuantileInplace(data, 0.75d), Maximum(data) };
    }

    /// <summary>
    /// Estimates the tau-th quantile from the unsorted data array.
    /// The tau-th quantile is the data value where the cumulative distribution
    /// function crosses tau.
    /// Approximately median-unbiased regardless of the sample distribution (R8).
    /// WARNING: Works inplace and can thus causes the data array to be reordered.
    /// </summary>
    /// <param name="data">Sample array, no sorting is assumed. Will be reordered.</param>
    /// <param name="tau">Quantile selector, between 0.0 and 1.0 (inclusive).</param>
    /// <remarks>
    /// R-8, SciPy-(1/3,1/3):
    /// Linear interpolation of the approximate medians for order statistics.
    /// When tau &lt; (2/3) / (N + 1/3), use x1. When tau &gt;= (N - 1/3) / (N + 1/3), use xN.
    /// </remarks>
    public static float QuantileInplace(float[] data, double tau)
    {
      if (tau < 0d || tau > 1d || data.Length == 0)
      {
        return float.NaN;
      }

      double h = (data.Length + 1d / 3d) * tau + 1d / 3d;
      var hf = (int)h;

      if (hf <= 0 || tau == 0d)
      {
        return Minimum(data);
      }

      if (hf >= data.Length || tau == 1d)
      {
        return Maximum(data);
      }

      var a = SelectInplace(data, hf - 1);
      var b = SelectInplace(data, hf);
      return (float)(a + (h - hf) * (b - a));
    }

    /// <summary>
    /// Estimates the tau-th quantile from the unsorted data array.
    /// The tau-th quantile is the data value where the cumulative distribution
    /// function crosses tau. The quantile definition can be specified
    /// by 4 parameters a, b, c and d, consistent with Mathematica.
    /// WARNING: Works inplace and can thus causes the data array to be reordered.
    /// </summary>
    /// <param name="data">Sample array, no sorting is assumed. Will be reordered.</param>
    /// <param name="tau">Quantile selector, between 0.0 and 1.0 (inclusive)</param>
    /// <param name="a">a-parameter</param>
    /// <param name="b">b-parameter</param>
    /// <param name="c">c-parameter</param>
    /// <param name="d">d-parameter</param>
    public static float QuantileCustomInplace(float[] data, double tau, double a, double b, double c, double d)
    {
      if (tau < 0d || tau > 1d || data.Length == 0)
      {
        return float.NaN;
      }

      var x = a + (data.Length + b) * tau - 1;
      var ip = Math.Truncate(x);
      var fp = x - ip;

      if (Math.Abs(fp) < 1e-9)
      {
        return SelectInplace(data, (int)ip);
      }

      var lower = SelectInplace(data, (int)Math.Floor(x));
      var upper = SelectInplace(data, (int)Math.Ceiling(x));
      return (float)(lower + (upper - lower) * (c + d * fp));
    }

    /// <summary>
    /// Estimates the tau-th quantile from the unsorted data array.
    /// The tau-th quantile is the data value where the cumulative distribution
    /// function crosses tau. The quantile definition can be specified to be compatible
    /// with an existing system.
    /// WARNING: Works inplace and can thus causes the data array to be reordered.
    /// </summary>
    /// <param name="data">Sample array, no sorting is assumed. Will be reordered.</param>
    /// <param name="tau">Quantile selector, between 0.0 and 1.0 (inclusive)</param>
    /// <param name="definition">Quantile definition, to choose what product/definition it should be consistent with</param>
    public static float QuantileCustomInplace(float[] data, double tau, QuantileDefinition definition)
    {
      if (tau < 0d || tau > 1d || data.Length == 0)
      {
        return float.NaN;
      }

      if (tau == 0d || data.Length == 1)
      {
        return Minimum(data);
      }

      if (tau == 1d)
      {
        return Maximum(data);
      }

      switch (definition)
      {
        case QuantileDefinition.R1:
          {
            double h = data.Length * tau + 0.5d;
            return SelectInplace(data, (int)Math.Ceiling(h - 0.5d) - 1);
          }

        case QuantileDefinition.R2:
          {
            double h = data.Length * tau + 0.5d;
            return (SelectInplace(data, (int)Math.Ceiling(h - 0.5d) - 1) + SelectInplace(data, (int)(h + 0.5d) - 1)) * 0.5f;
          }

        case QuantileDefinition.R3:
          {
            double h = data.Length * tau;
            return SelectInplace(data, (int)Math.Round(h) - 1);
          }

        case QuantileDefinition.R4:
          {
            double h = data.Length * tau;
            var hf = (int)h;
            var lower = SelectInplace(data, hf - 1);
            var upper = SelectInplace(data, hf);
            return (float)(lower + (h - hf) * (upper - lower));
          }

        case QuantileDefinition.R5:
          {
            double h = data.Length * tau + 0.5d;
            var hf = (int)h;
            var lower = SelectInplace(data, hf - 1);
            var upper = SelectInplace(data, hf);
            return (float)(lower + (h - hf) * (upper - lower));
          }

        case QuantileDefinition.R6:
          {
            double h = (data.Length + 1) * tau;
            var hf = (int)h;
            var lower = SelectInplace(data, hf - 1);
            var upper = SelectInplace(data, hf);
            return (float)(lower + (h - hf) * (upper - lower));
          }

        case QuantileDefinition.R7:
          {
            double h = (data.Length - 1) * tau + 1d;
            var hf = (int)h;
            var lower = SelectInplace(data, hf - 1);
            var upper = SelectInplace(data, hf);
            return (float)(lower + (h - hf) * (upper - lower));
          }

        case QuantileDefinition.R8:
          {
            double h = (data.Length + 1 / 3d) * tau + 1 / 3d;
            var hf = (int)h;
            var lower = SelectInplace(data, hf - 1);
            var upper = SelectInplace(data, hf);
            return (float)(lower + (h - hf) * (upper - lower));
          }

        case QuantileDefinition.R9:
          {
            double h = (data.Length + 0.25d) * tau + 0.375d;
            var hf = (int)h;
            var lower = SelectInplace(data, hf - 1);
            var upper = SelectInplace(data, hf);
            return (float)(lower + (h - hf) * (upper - lower));
          }

        default:
          throw new NotSupportedException();
      }
    }

    private static float SelectInplace(float[] workingData, int rank)
    {
      // Numerical Recipes: select
      // http://en.wikipedia.org/wiki/Selection_algorithm
      if (rank <= 0)
      {
        return Minimum(workingData);
      }

      if (rank >= workingData.Length - 1)
      {
        return Maximum(workingData);
      }

      float[] a = workingData;
      int low = 0;
      int high = a.Length - 1;

      while (true)
      {
        if (high <= low + 1)
        {
          if (high == low + 1 && a[high] < a[low])
          {
            (a[low], a[high]) = (a[high], a[low]);
          }

          return a[rank];
        }

        int middle = (low + high) >> 1;

        (a[middle], a[low + 1]) = (a[low + 1], a[middle]);

        if (a[low] > a[high])
        {
          (a[low], a[high]) = (a[high], a[low]);
        }

        if (a[low + 1] > a[high])
        {
          (a[low + 1], a[high]) = (a[high], a[low + 1]);
        }

        if (a[low] > a[low + 1])
        {
          (a[low], a[low + 1]) = (a[low + 1], a[low]);
        }

        int begin = low + 1;
        int end = high;
        float pivot = a[begin];

        while (true)
        {
          do
          {
            begin++;
          }
          while (a[begin] < pivot);

          do
          {
            end--;
          }
          while (a[end] > pivot);

          if (end < begin)
          {
            break;
          }

          (a[begin], a[end]) = (a[end], a[begin]);
        }

        a[low + 1] = a[end];
        a[end] = pivot;

        if (end >= rank)
        {
          high = end - 1;
        }

        if (end <= rank)
        {
          low = begin;
        }
      }
    }

    /// <summary>
    /// Evaluates the rank of each entry of the unsorted data array.
    /// The rank definition can be specified to be compatible
    /// with an existing system.
    /// WARNING: Works inplace and can thus causes the data array to be reordered.
    /// </summary>
    public static float[] RanksInplace(float[] data, RankDefinition definition = RankDefinition.Default)
    {
      var ranks = new float[data.Length];
      var index = new int[data.Length];
      for (int i = 0; i < index.Length; i++)
      {
        index[i] = i;
      }

      if (definition == RankDefinition.First)
      {
        Sorting.SortAll(data, index);
        for (int i = 0; i < ranks.Length; i++)
        {
          ranks[index[i]] = i + 1;
        }

        return ranks;
      }

      Sorting.Sort(data, index);
      int previousIndex = 0;
      for (int i = 1; i < data.Length; i++)
      {
        if (Math.Abs(data[i] - data[previousIndex]) <= 0d)
        {
          continue;
        }

        if (i == previousIndex + 1)
        {
          ranks[index[previousIndex]] = i;
        }
        else
        {
          RanksTies(ranks, index, previousIndex, i, definition);
        }

        previousIndex = i;
      }

      RanksTies(ranks, index, previousIndex, data.Length, definition);
      return ranks;
    }

    private static void RanksTies(float[] ranks, int[] index, int a, int b, RankDefinition definition)
    {
      // TODO: potential for PERF optimization
      float rank;
      switch (definition)
      {
        case RankDefinition.Average:
          {
            rank = (b + a - 1) / 2f + 1;
            break;
          }

        case RankDefinition.Min:
          {
            rank = a + 1;
            break;
          }

        case RankDefinition.Max:
          {
            rank = b;
            break;
          }

        default:
          throw new NotSupportedException();
      }

      for (int k = a; k < b; k++)
      {
        ranks[index[k]] = rank;
      }
    }
  }
}
