// <copyright file="QuantileDefinition.cs" company="Math.NET">
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

namespace Altaxo.Calc.Statistics
{
  /// <summary>
  /// Specifies the algorithm used to compute quantiles.
  /// </summary>
  public enum QuantileDefinition
  {
    /// <summary>
    /// R1 definition.
    /// </summary>
    R1 = 1,

    /// <summary>
    /// SAS3 definition.
    /// </summary>
    SAS3 = 1,

    /// <summary>
    /// Empirical inverse CDF definition.
    /// </summary>
    EmpiricalInvCDF = 1,

    /// <summary>
    /// R2 definition.
    /// </summary>
    R2 = 2,

    /// <summary>
    /// SAS5 definition.
    /// </summary>
    SAS5 = 2,

    /// <summary>
    /// Averaged empirical inverse CDF definition.
    /// </summary>
    EmpiricalInvCDFAverage = 2,

    /// <summary>
    /// R3 definition.
    /// </summary>
    R3 = 3,

    /// <summary>
    /// SAS2 definition.
    /// </summary>
    SAS2 = 3,

    /// <summary>
    /// Nearest-order-statistic definition.
    /// </summary>
    Nearest = 3,

    /// <summary>
    /// R4 definition.
    /// </summary>
    R4 = 4,

    /// <summary>
    /// SAS1 definition.
    /// </summary>
    SAS1 = 4,

    /// <summary>
    /// California definition.
    /// </summary>
    California = 4,

    /// <summary>
    /// R5 definition.
    /// </summary>
    R5 = 5,

    /// <summary>
    /// Hydrology definition.
    /// </summary>
    Hydrology = 5,

    /// <summary>
    /// Hazen definition.
    /// </summary>
    Hazen = 5,

    /// <summary>
    /// R6 definition.
    /// </summary>
    R6 = 6,

    /// <summary>
    /// SAS4 definition.
    /// </summary>
    SAS4 = 6,

    /// <summary>
    /// NIST definition.
    /// </summary>
    Nist = 6,

    /// <summary>
    /// Weibull definition.
    /// </summary>
    Weibull = 6,

    /// <summary>
    /// SPSS definition.
    /// </summary>
    SPSS = 6,

    /// <summary>
    /// R7 definition.
    /// </summary>
    R7 = 7,

    /// <summary>
    /// Excel definition.
    /// </summary>
    Excel = 7,

    /// <summary>
    /// Mode definition.
    /// </summary>
    Mode = 7,

    /// <summary>
    /// S definition.
    /// </summary>
    S = 7,

    /// <summary>
    /// R8 definition.
    /// </summary>
    R8 = 8,

    /// <summary>
    /// Median-unbiased definition.
    /// </summary>
    Median = 8,

    /// <summary>
    /// Default quantile definition.
    /// </summary>
    Default = 8,

    /// <summary>
    /// R9 definition.
    /// </summary>
    R9 = 9,

    /// <summary>
    /// Normal-unbiased definition.
    /// </summary>
    Normal = 9,
  }
}
