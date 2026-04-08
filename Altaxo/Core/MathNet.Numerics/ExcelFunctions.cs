// <copyright file="ExcelFunctions.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
//
// Copyright (c) 2009-2013 Math.NET
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
using System.Runtime;
using Altaxo.Calc.Distributions;
using Altaxo.Calc.Statistics;

// ReSharper disable InconsistentNaming

namespace Altaxo.Calc
{
  /// <summary>
  /// Collection of functions equivalent to those provided by Microsoft Excel
  /// but backed instead by Math.NET Numerics.
  /// We do not recommend to use them except in an intermediate phase when
  /// porting over solutions previously implemented in Excel.
  /// </summary>
  public static class ExcelFunctions
  {
    /// <summary>
    /// Returns the standard normal cumulative distribution value.
    /// </summary>
    /// <param name="z">The z-score.</param>
    /// <returns>The cumulative probability for the standard normal distribution.</returns>
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static double NormSDist(double z)
    {
      return Normal.CDF(0d, 1d, z);
    }

    /// <summary>
    /// Returns the inverse of the standard normal cumulative distribution.
    /// </summary>
    /// <param name="probability">The cumulative probability.</param>
    /// <returns>The z-score whose cumulative probability matches <paramref name="probability"/>.</returns>
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static double NormSInv(double probability)
    {
      return Normal.InvCDF(0d, 1d, probability);
    }

    /// <summary>
    /// Returns the normal distribution value for the specified parameters.
    /// </summary>
    /// <param name="x">The value at which to evaluate the distribution.</param>
    /// <param name="mean">The mean of the distribution.</param>
    /// <param name="standardDev">The standard deviation of the distribution.</param>
    /// <param name="cumulative"><see langword="true"/> to return the cumulative distribution value; <see langword="false"/> to return the density.</param>
    /// <returns>The requested normal distribution value.</returns>
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static double NormDist(double x, double mean, double standardDev, bool cumulative)
    {
      return cumulative ? Normal.CDF(mean, standardDev, x) : Normal.PDF(mean, standardDev, x);
    }

    /// <summary>
    /// Returns the inverse normal cumulative distribution value for the specified parameters.
    /// </summary>
    /// <param name="probability">The cumulative probability.</param>
    /// <param name="mean">The mean of the distribution.</param>
    /// <param name="standardDev">The standard deviation of the distribution.</param>
    /// <returns>The value whose cumulative probability matches <paramref name="probability"/>.</returns>
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static double NormInv(double probability, double mean, double standardDev)
    {
      return Normal.InvCDF(mean, standardDev, probability);
    }

    /// <summary>
    /// Returns the Student's t-distribution tail probability.
    /// </summary>
    /// <param name="x">The value at which to evaluate the distribution.</param>
    /// <param name="degreesFreedom">The degrees of freedom.</param>
    /// <param name="tails">The number of tails to include, either 1 or 2.</param>
    /// <returns>The upper-tail probability for the specified t-distribution.</returns>
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static double TDist(double x, int degreesFreedom, int tails)
    {
      switch (tails)
      {
        case 1:
          return 1d - StudentT.CDF(0d, 1d, degreesFreedom, x);
        case 2:
          return 1d - StudentT.CDF(0d, 1d, degreesFreedom, x) + StudentT.CDF(0d, 1d, degreesFreedom, -x);
        default:
          throw new ArgumentOutOfRangeException(nameof(tails));
      }
    }

    /// <summary>
    /// Returns the inverse Student's t-distribution value.
    /// </summary>
    /// <param name="probability">The two-tailed probability.</param>
    /// <param name="degreesFreedom">The degrees of freedom.</param>
    /// <returns>The t value whose two-tailed probability matches <paramref name="probability"/>.</returns>
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static double TInv(double probability, int degreesFreedom)
    {
      return -StudentT.InvCDF(0d, 1d, degreesFreedom, probability / 2);
    }

    /// <summary>
    /// Returns the upper-tail cumulative distribution value for the Fisher-Snedecor distribution.
    /// </summary>
    /// <param name="x">The value at which to evaluate the distribution.</param>
    /// <param name="degreesFreedom1">The first degrees of freedom parameter.</param>
    /// <param name="degreesFreedom2">The second degrees of freedom parameter.</param>
    /// <returns>The upper-tail probability.</returns>
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static double FDist(double x, int degreesFreedom1, int degreesFreedom2)
    {
      return 1d - FisherSnedecor.CDF(degreesFreedom1, degreesFreedom2, x);
    }

    /// <summary>
    /// Returns the inverse Fisher-Snedecor distribution value.
    /// </summary>
    /// <param name="probability">The upper-tail probability.</param>
    /// <param name="degreesFreedom1">The first degrees of freedom parameter.</param>
    /// <param name="degreesFreedom2">The second degrees of freedom parameter.</param>
    /// <returns>The F value whose upper-tail probability matches <paramref name="probability"/>.</returns>
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static double FInv(double probability, int degreesFreedom1, int degreesFreedom2)
    {
      return FisherSnedecor.InvCDF(degreesFreedom1, degreesFreedom2, 1d - probability);
    }

    /// <summary>
    /// Returns the beta cumulative distribution value.
    /// </summary>
    /// <param name="x">The value at which to evaluate the distribution.</param>
    /// <param name="alpha">The alpha shape parameter.</param>
    /// <param name="beta">The beta shape parameter.</param>
    /// <returns>The cumulative probability.</returns>
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static double BetaDist(double x, double alpha, double beta)
    {
      return Beta.CDF(alpha, beta, x);
    }

    /// <summary>
    /// Returns the inverse beta cumulative distribution value.
    /// </summary>
    /// <param name="probability">The cumulative probability.</param>
    /// <param name="alpha">The alpha shape parameter.</param>
    /// <param name="beta">The beta shape parameter.</param>
    /// <returns>The value whose cumulative probability matches <paramref name="probability"/>.</returns>
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static double BetaInv(double probability, double alpha, double beta)
    {
      return Beta.InvCDF(alpha, beta, probability);
    }

    /// <summary>
    /// Returns the gamma distribution value for the specified parameters.
    /// </summary>
    /// <param name="x">The value at which to evaluate the distribution.</param>
    /// <param name="alpha">The shape parameter.</param>
    /// <param name="beta">The scale parameter used by Excel semantics.</param>
    /// <param name="cumulative"><see langword="true"/> to return the cumulative distribution value; <see langword="false"/> to return the density.</param>
    /// <returns>The requested gamma distribution value.</returns>
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static double GammaDist(double x, double alpha, double beta, bool cumulative)
    {
      return cumulative ? Gamma.CDF(alpha, 1 / beta, x) : Gamma.PDF(alpha, 1 / beta, x);
    }

    /// <summary>
    /// Returns the inverse gamma cumulative distribution value.
    /// </summary>
    /// <param name="probability">The cumulative probability.</param>
    /// <param name="alpha">The shape parameter.</param>
    /// <param name="beta">The scale parameter used by Excel semantics.</param>
    /// <returns>The value whose cumulative probability matches <paramref name="probability"/>.</returns>
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static double GammaInv(double probability, double alpha, double beta)
    {
      return Gamma.InvCDF(alpha, 1 / beta, probability);
    }

    /// <summary>
    /// Returns the specified quartile of an array using Excel's quantile definition.
    /// </summary>
    /// <param name="array">The input data.</param>
    /// <param name="quant">The quartile selector, from 0 to 4.</param>
    /// <returns>The requested quartile value.</returns>
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static double Quartile(double[] array, int quant)
    {
      switch (quant)
      {
        case 0:
          return ArrayStatistics.Minimum(array);
        case 1:
          return array.QuantileCustom(0.25, QuantileDefinition.Excel);
        case 2:
          return array.QuantileCustom(0.5, QuantileDefinition.Excel);
        case 3:
          return array.QuantileCustom(0.75, QuantileDefinition.Excel);
        case 4:
          return ArrayStatistics.Maximum(array);
        default:
          throw new ArgumentOutOfRangeException(nameof(quant));
      }
    }

    /// <summary>
    /// Returns the percentile of an array using Excel's quantile definition.
    /// </summary>
    /// <param name="array">The input data.</param>
    /// <param name="k">The percentile fraction in the range from 0 to 1.</param>
    /// <returns>The percentile value.</returns>
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static double Percentile(double[] array, double k)
    {
      return array.QuantileCustom(k, QuantileDefinition.Excel);
    }

    /// <summary>
    /// Returns the rank of a value within an array as a percentile.
    /// </summary>
    /// <param name="array">The input data.</param>
    /// <param name="x">The value to rank.</param>
    /// <returns>The percentile rank of <paramref name="x"/> within <paramref name="array"/>.</returns>
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static double PercentRank(double[] array, double x)
    {
      return array.QuantileRank(x, RankDefinition.Min);
    }
  }
}

// ReSharper restore InconsistentNaming
