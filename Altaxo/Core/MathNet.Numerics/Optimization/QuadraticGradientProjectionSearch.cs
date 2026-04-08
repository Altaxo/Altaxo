// <copyright file="QuadraticGradientProjectionSearch.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
//
// Copyright (c) 2009-2017 Math.NET
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
using System.Collections.Generic;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Optimization
{
  /// <summary>
  /// Provides a quadratic gradient projection search for box-constrained subproblems.
  /// </summary>
  public static class QuadraticGradientProjectionSearch
  {
    /// <summary>
    /// Searches for the Cauchy point of the quadratic model under bound constraints.
    /// </summary>
    /// <param name="x0">The starting point.</param>
    /// <param name="gradient">The gradient at the starting point.</param>
    /// <param name="hessian">The Hessian approximation.</param>
    /// <param name="lowerBound">The lower bounds.</param>
    /// <param name="upperBound">The upper bounds.</param>
    /// <returns>The gradient projection result.</returns>
    public static GradientProjectionResult Search(Vector<double> x0, Vector<double> gradient, Matrix<double> hessian, Vector<double> lowerBound, Vector<double> upperBound)
    {
      List<bool> isFixed = new List<bool>(x0.Count);
      List<double> breakpoint = new List<double>(x0.Count);
      for (int ii = 0; ii < x0.Count; ++ii)
      {
        breakpoint.Add(0.0);
        isFixed.Add(false);
        if (gradient[ii] < 0)
          breakpoint[ii] = (x0[ii] - upperBound[ii]) / gradient[ii];
        else if (gradient[ii] > 0)
          breakpoint[ii] = (x0[ii] - lowerBound[ii]) / gradient[ii];
        else
        {
          if (Math.Abs(x0[ii] - upperBound[ii]) < 100 * double.Epsilon || Math.Abs(x0[ii] - lowerBound[ii]) < 100 * double.Epsilon)
            breakpoint[ii] = 0.0;
          else
            breakpoint[ii] = double.PositiveInfinity;
        }
      }

      var orderedBreakpoint = new List<double>(x0.Count);
      orderedBreakpoint.AddRange(breakpoint);
      orderedBreakpoint.Sort();

      // Compute initial state variables
      var d = -gradient;
      for (int ii = 0; ii < d.Count; ++ii)
        if (breakpoint[ii] <= 0.0)
          d[ii] *= 0.0;


      int jj = -1;
      var x = x0;
      var f1 = gradient * d;
      var f2 = 0.5 * d * hessian * d;
      var sMin = -f1 / f2;
      var maxS = orderedBreakpoint[0];

      if (sMin < maxS)
        return new GradientProjectionResult(x + sMin * d, 0, isFixed);

      // while minimum of the last quadratic piece observed is beyond the interval searched
      while (true)
      {
        if (jj + 1 >= orderedBreakpoint.Count - 1)
        {
          isFixed[isFixed.Count - 1] = true;
          return new GradientProjectionResult(x + maxS * d, lowerBound.Count, isFixed);
        }
        // update data to the beginning of the interval we're searching
        jj += 1;
        x += d * maxS;
        maxS = orderedBreakpoint[jj + 1] - orderedBreakpoint[jj];

        int fixedCount = 0;
        for (int ii = 0; ii < d.Count; ++ii)
          if (orderedBreakpoint[jj] >= breakpoint[ii])
          {
            d[ii] *= 0.0;
            isFixed[ii] = true;
            fixedCount += 1;
          }

        if (double.IsPositiveInfinity(orderedBreakpoint[jj + 1]))
          return new GradientProjectionResult(x, fixedCount, isFixed);

        f1 = gradient * d + (x - x0) * hessian * d;
        f2 = d * hessian * d;

        sMin = -f1 / f2;

        if (sMin < maxS)
          return new GradientProjectionResult(x + sMin * d, fixedCount, isFixed);
      }
    }

    /// <summary>
    /// Represents the result of a gradient projection search.
    /// </summary>
    public readonly struct GradientProjectionResult
    {
      /// <summary>
      /// Initializes a new instance of the <see cref="GradientProjectionResult"/> struct.
      /// </summary>
      /// <param name="cauchyPoint">The computed Cauchy point.</param>
      /// <param name="fixedCount">The number of fixed variables.</param>
      /// <param name="isFixed">Flags indicating fixed variables.</param>
      public GradientProjectionResult(Vector<double> cauchyPoint, int fixedCount, List<bool> isFixed)
      {
        CauchyPoint = cauchyPoint;
        FixedCount = fixedCount;
        IsFixed = isFixed;
      }
      /// <summary>
      /// Gets the computed Cauchy point.
      /// </summary>
      public Vector<double> CauchyPoint { get; }
      /// <summary>
      /// Gets the number of fixed variables.
      /// </summary>
      public int FixedCount { get; }
      /// <summary>
      /// Gets flags indicating which variables are fixed.
      /// </summary>
      public List<bool> IsFixed { get; }
    }
  }
}
