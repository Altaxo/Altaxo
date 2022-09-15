﻿// <copyright file="GaussLegendreRule.cs" company="Math.NET">
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
    internal double[] Abscissas { get; }

    internal double[] Weights { get; }

    internal double IntervalBegin { get; }

    internal double IntervalEnd { get; }

    internal int Order { get; }

    internal GaussPoint(double intervalBegin, double intervalEnd, int order, double[] abscissas, double[] weights)
    {
      Abscissas = abscissas;
      Weights = weights;
      IntervalBegin = intervalBegin;
      IntervalEnd = intervalEnd;
      Order = order;
    }

    internal GaussPoint(int order, double[] abscissas, double[] weights) : this(-1, 1, order, abscissas, weights)
    {
    }
  }
}
