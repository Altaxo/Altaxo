// <copyright file="GaussLegendreRule.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
//
// Copyright (c) 2009-2016 Math.NET
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

namespace Altaxo.Calc.Integration.GaussRule
{
  /// <summary>
  /// Contains the abscissas/weights, order, and intervalBegin/intervalEnd.
  /// </summary>
  internal class GaussPoint
  {
    /// <summary>
    /// Gets the abscissas of the quadrature rule.
    /// </summary>
    internal double[] Abscissas { get; }

    /// <summary>
    /// Gets the weights of the quadrature rule.
    /// </summary>
    internal double[] Weights { get; }

    /// <summary>
    /// Gets the beginning of the integration interval.
    /// </summary>
    internal double IntervalBegin { get; }

    /// <summary>
    /// Gets the end of the integration interval.
    /// </summary>
    internal double IntervalEnd { get; }

    /// <summary>
    /// Gets the order of the quadrature rule.
    /// </summary>
    internal int Order { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GaussPoint"/> class.
    /// </summary>
    /// <param name="intervalBegin">The beginning of the integration interval.</param>
    /// <param name="intervalEnd">The end of the integration interval.</param>
    /// <param name="order">The order of the quadrature rule.</param>
    /// <param name="abscissas">The abscissas of the quadrature rule.</param>
    /// <param name="weights">The weights of the quadrature rule.</param>
    internal GaussPoint(double intervalBegin, double intervalEnd, int order, double[] abscissas, double[] weights)
    {
      Abscissas = abscissas;
      Weights = weights;
      IntervalBegin = intervalBegin;
      IntervalEnd = intervalEnd;
      Order = order;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GaussPoint"/> class on the default interval [-1, 1].
    /// </summary>
    /// <param name="order">The order of the quadrature rule.</param>
    /// <param name="abscissas">The abscissas of the quadrature rule.</param>
    /// <param name="weights">The weights of the quadrature rule.</param>
    internal GaussPoint(int order, double[] abscissas, double[] weights) : this(-1, 1, order, abscissas, weights)
    {
    }
  }
}
