// <copyright file="MinimizationWithLineSearchResult.cs" company="Math.NET">
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

namespace Altaxo.Calc.Optimization
{
  /// <summary>
  /// Represents the outcome of a minimization run that uses line search.
  /// </summary>
  public class MinimizationWithLineSearchResult : MinimizationResult
  {
    /// <summary>
    /// Gets the total number of line search iterations.
    /// </summary>
    public int TotalLineSearchIterations { get; }
    /// <summary>
    /// Gets the number of minimizer iterations that required a nontrivial line search.
    /// </summary>
    public int IterationsWithNonTrivialLineSearch { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MinimizationWithLineSearchResult"/> class.
    /// </summary>
    /// <param name="functionInfo">The objective function state at the minimum.</param>
    /// <param name="iterations">The number of performed minimizer iterations.</param>
    /// <param name="reasonForExit">The termination reason.</param>
    /// <param name="totalLineSearchIterations">The total number of line search iterations.</param>
    /// <param name="iterationsWithNonTrivialLineSearch">The number of iterations with a nontrivial line search.</param>
    public MinimizationWithLineSearchResult(IObjectiveFunction functionInfo, int iterations, ExitCondition reasonForExit, int totalLineSearchIterations, int iterationsWithNonTrivialLineSearch)
        : base(functionInfo, iterations, reasonForExit)
    {
      TotalLineSearchIterations = totalLineSearchIterations;
      IterationsWithNonTrivialLineSearch = iterationsWithNonTrivialLineSearch;
    }
  }
}
