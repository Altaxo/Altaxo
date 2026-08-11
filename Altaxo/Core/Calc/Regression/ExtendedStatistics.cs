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
using Altaxo.Calc.Statistics;
using StatisticsHelper = Altaxo.Calc.Statistics.Statistics;

namespace Altaxo.Calc.Regression
{
  /// <summary>
  /// Extends the <see cref="QuickStatistics"/> class with statistics that require access to all data points.
  /// </summary>
  public class ExtendedStatistics : QuickStatistics
  {
    private readonly List<double> _samples = new();

    /// <inheritdoc/>
    public override void Add(double x)
    {
      _samples.Add(x);
      base.Add(x);
    }

    /// <summary>
    /// Resets all accumulated statistics and removes all stored data points.
    /// </summary>
    public override void Clear()
    {
      _samples.Clear();
      base.Clear();
    }

    /// <summary>
    /// Adds a sequence of data points to the statistics.
    /// </summary>
    /// <param name="values">The data points to add.</param>
    /// <returns>This instance.</returns>
    public new ExtendedStatistics AddRange(IEnumerable<double> values)
    {
      foreach (var value in values)
      {
        Add(value);
      }

      return this;
    }

    /// <summary>
    /// Adds a span of data points to the statistics.
    /// </summary>
    /// <param name="values">The data points to add.</param>
    /// <returns>This instance.</returns>
    public new ExtendedStatistics AddRange(ReadOnlySpan<double> values)
    {
      foreach (var value in values)
      {
        Add(value);
      }

      return this;
    }

    /// <summary>
    /// Calculates the median of the data set.
    /// </summary>
    public double Median()
    {
      return StatisticsHelper.Median(_samples);
    }

    /// <summary>
    /// Calculates the minimum absolute value.
    /// </summary>
    /// <returns>The minimum absolute value.</returns>
    public double MinimumAbsolute()
    {
      return StatisticsHelper.MinimumAbsolute(_samples);
    }

    /// <summary>
    /// Calculates the maximum absolute value.
    /// </summary>
    /// <returns>The maximum absolute value.</returns>
    public double MaximumAbsolute()
    {
      return StatisticsHelper.MaximumAbsolute(_samples);
    }

    /// <summary>
    /// Calculates the geometric mean.
    /// </summary>
    /// <returns>The geometric mean.</returns>
    public double GeometricMean()
    {
      return StatisticsHelper.GeometricMean(_samples);
    }

    /// <summary>
    /// Calculates the harmonic mean.
    /// </summary>
    /// <returns>The harmonic mean.</returns>
    public double HarmonicMean()
    {
      return StatisticsHelper.HarmonicMean(_samples);
    }

    /// <summary>
    /// Calculates the sample skewness.
    /// </summary>
    /// <returns>The sample skewness.</returns>
    public double Skewness()
    {
      return StatisticsHelper.Skewness(_samples);
    }

    /// <summary>
    /// Calculates the population skewness.
    /// </summary>
    /// <returns>The population skewness.</returns>
    public double PopulationSkewness()
    {
      return StatisticsHelper.PopulationSkewness(_samples);
    }

    /// <summary>
    /// Calculates the sample kurtosis.
    /// </summary>
    /// <returns>The sample kurtosis.</returns>
    public double Kurtosis()
    {
      return StatisticsHelper.Kurtosis(_samples);
    }

    /// <summary>
    /// Calculates the population kurtosis.
    /// </summary>
    /// <returns>The population kurtosis.</returns>
    public double PopulationKurtosis()
    {
      return StatisticsHelper.PopulationKurtosis(_samples);
    }

    /// <summary>
    /// Calculates the mean and sample variance.
    /// </summary>
    /// <returns>A tuple containing mean and sample variance.</returns>
    public (double Mean, double Variance) MeanVariance()
    {
      return StatisticsHelper.MeanVariance(_samples);
    }

    /// <summary>
    /// Calculates the mean and sample standard deviation.
    /// </summary>
    /// <returns>A tuple containing mean and sample standard deviation.</returns>
    public (double Mean, double StandardDeviation) MeanStandardDeviation()
    {
      return StatisticsHelper.MeanStandardDeviation(_samples);
    }

    /// <summary>
    /// Calculates sample skewness and sample kurtosis.
    /// </summary>
    /// <returns>A tuple containing sample skewness and sample kurtosis.</returns>
    public (double Skewness, double Kurtosis) SkewnessKurtosis()
    {
      return StatisticsHelper.SkewnessKurtosis(_samples);
    }

    /// <summary>
    /// Calculates population skewness and population kurtosis.
    /// </summary>
    /// <returns>A tuple containing population skewness and population kurtosis.</returns>
    public (double Skewness, double Kurtosis) PopulationSkewnessKurtosis()
    {
      return StatisticsHelper.PopulationSkewnessKurtosis(_samples);
    }

    /// <summary>
    /// Calculates the sample covariance with another sequence.
    /// </summary>
    /// <param name="samples">The second sequence.</param>
    /// <returns>The sample covariance.</returns>
    public double Covariance(IEnumerable<double> samples)
    {
      return StatisticsHelper.Covariance(_samples, samples);
    }

    /// <summary>
    /// Calculates the sample covariance with another sequence.
    /// </summary>
    /// <param name="otherSamples">The second sample set.</param>
    /// <returns>The sample covariance.</returns>
    public double Covariance(ExtendedStatistics otherSamples)
    {
      return StatisticsHelper.Covariance(_samples, otherSamples._samples);
    }

    /// <summary>
    /// Calculates the population covariance with another sequence.
    /// </summary>
    /// <param name="population">The second sequence.</param>
    /// <returns>The population covariance.</returns>
    public double PopulationCovariance(IEnumerable<double> population)
    {
      return StatisticsHelper.PopulationCovariance(_samples, population);
    }

    /// <summary>
    /// Calculates the population covariance with another sequence.
    /// </summary>
    /// <param name="population">The second sequence.</param>
    /// <returns>The population covariance.</returns>
    public double PopulationCovariance(ExtendedStatistics population)
    {
      return StatisticsHelper.PopulationCovariance(_samples, population._samples);
    }

    /// <summary>
    /// Calculates the root mean square.
    /// </summary>
    /// <returns>The root mean square.</returns>
    public double RootMeanSquare()
    {
      return StatisticsHelper.RootMeanSquare(_samples);
    }

    /// <summary>
    /// Calculates a quantile for the given probability.
    /// </summary>
    /// <param name="tau">The probability in the interval [0, 1].</param>
    /// <returns>The quantile value.</returns>
    public double Quantile(double tau)
    {
      return StatisticsHelper.Quantile(_samples, tau);
    }

    /// <summary>
    /// Creates a quantile function.
    /// </summary>
    /// <returns>A function mapping a probability in [0, 1] to a quantile value.</returns>
    public Func<double, double> QuantileFunc()
    {
      return StatisticsHelper.QuantileFunc(_samples);
    }

    /// <summary>
    /// Calculates a quantile for the given probability and quantile definition.
    /// </summary>
    /// <param name="tau">The probability in the interval [0, 1].</param>
    /// <param name="definition">The quantile definition.</param>
    /// <returns>The quantile value.</returns>
    public double QuantileCustom(double tau, QuantileDefinition definition)
    {
      return StatisticsHelper.QuantileCustom(_samples, tau, definition);
    }

    /// <summary>
    /// Creates a quantile function using the specified quantile definition.
    /// </summary>
    /// <param name="definition">The quantile definition.</param>
    /// <returns>A function mapping a probability in [0, 1] to a quantile value.</returns>
    public Func<double, double> QuantileCustomFunc(QuantileDefinition definition)
    {
      return StatisticsHelper.QuantileCustomFunc(_samples, definition);
    }

    /// <summary>
    /// Calculates the percentile value.
    /// </summary>
    /// <param name="p">The percentile in the interval [0, 100].</param>
    /// <returns>The percentile value.</returns>
    public double Percentile(int p)
    {
      return StatisticsHelper.Percentile(_samples, p);
    }

    /// <summary>
    /// Creates a percentile function.
    /// </summary>
    /// <returns>A function mapping a percentile in [0, 100] to a value.</returns>
    public Func<int, double> PercentileFunc()
    {
      return StatisticsHelper.PercentileFunc(_samples);
    }

    /// <summary>
    /// Calculates the lower quartile.
    /// </summary>
    /// <returns>The lower quartile.</returns>
    public double LowerQuartile()
    {
      return StatisticsHelper.LowerQuartile(_samples);
    }

    /// <summary>
    /// Calculates the upper quartile.
    /// </summary>
    /// <returns>The upper quartile.</returns>
    public double UpperQuartile()
    {
      return StatisticsHelper.UpperQuartile(_samples);
    }

    /// <summary>
    /// Calculates the interquartile range.
    /// </summary>
    /// <returns>The interquartile range.</returns>
    public double InterquartileRange()
    {
      return StatisticsHelper.InterquartileRange(_samples);
    }

    /// <summary>
    /// Calculates the five-number summary.
    /// </summary>
    /// <returns>An array containing minimum, lower quartile, median, upper quartile, and maximum.</returns>
    public double[] FiveNumberSummary()
    {
      return StatisticsHelper.FiveNumberSummary(_samples);
    }

    /// <summary>
    /// Calculates the ranks of all values.
    /// </summary>
    /// <param name="definition">The rank definition.</param>
    /// <returns>An array containing ranks for all values.</returns>
    public double[] Ranks(RankDefinition definition = RankDefinition.Default)
    {
      return StatisticsHelper.Ranks(_samples, definition);
    }

    /// <summary>
    /// Calculates the quantile rank of a value.
    /// </summary>
    /// <param name="x">The value to rank.</param>
    /// <param name="definition">The rank definition.</param>
    /// <returns>The quantile rank.</returns>
    public double QuantileRank(double x, RankDefinition definition = RankDefinition.Default)
    {
      return StatisticsHelper.QuantileRank(_samples, x, definition);
    }

    /// <summary>
    /// Creates a quantile-rank function.
    /// </summary>
    /// <param name="definition">The rank definition.</param>
    /// <returns>A function mapping a value to its quantile rank.</returns>
    public Func<double, double> QuantileRankFunc(RankDefinition definition = RankDefinition.Default)
    {
      return StatisticsHelper.QuantileRankFunc(_samples, definition);
    }

    /// <summary>
    /// Calculates the empirical cumulative distribution value for a given sample value.
    /// </summary>
    /// <param name="x">The sample value.</param>
    /// <returns>The empirical cumulative probability.</returns>
    public double EmpiricalCDF(double x)
    {
      return StatisticsHelper.EmpiricalCDF(_samples, x);
    }

    /// <summary>
    /// Creates an empirical cumulative distribution function.
    /// </summary>
    /// <returns>A function mapping a sample value to a cumulative probability.</returns>
    public Func<double, double> EmpiricalCDFFunc()
    {
      return StatisticsHelper.EmpiricalCDFFunc(_samples);
    }

    /// <summary>
    /// Calculates the inverse empirical cumulative distribution value.
    /// </summary>
    /// <param name="tau">The probability in the interval [0, 1].</param>
    /// <returns>The corresponding sample value.</returns>
    public double EmpiricalInvCDF(double tau)
    {
      return StatisticsHelper.EmpiricalInvCDF(_samples, tau);
    }

    /// <summary>
    /// Creates an inverse empirical cumulative distribution function.
    /// </summary>
    /// <returns>A function mapping a probability in [0, 1] to a sample value.</returns>
    public Func<double, double> EmpiricalInvCDFFunc()
    {
      return StatisticsHelper.EmpiricalInvCDFFunc(_samples);
    }

    /// <summary>
    /// Calculates a moving average over the data points.
    /// </summary>
    /// <param name="windowSize">The window size for averaging.</param>
    /// <returns>A sequence containing the moving average values.</returns>
    public IEnumerable<double> MovingAverage(int windowSize)
    {
      return StatisticsHelper.MovingAverage(_samples, windowSize);
    }
  }
}
