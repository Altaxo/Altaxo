// <copyright file="ExitCondition.cs" company="Math.NET">
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
  /// Specifies the reason why an optimization algorithm terminated.
  /// </summary>
  public enum ExitCondition
  {
    /// <summary>
    /// No exit condition has been recorded.
    /// </summary>
    None,
    /// <summary>
    /// Invalid values were encountered.
    /// </summary>
    InvalidValues,
    /// <summary>
    /// The maximum number of iterations was exceeded.
    /// </summary>
    ExceedIterations,
    /// <summary>
    /// Relative point change tolerance was reached.
    /// </summary>
    RelativePoints,
    /// <summary>
    /// Relative gradient tolerance was reached.
    /// </summary>
    RelativeGradient,
    /// <summary>
    /// Progress became insufficient.
    /// </summary>
    LackOfProgress,
    /// <summary>
    /// Absolute gradient tolerance was reached.
    /// </summary>
    AbsoluteGradient,
    /// <summary>
    /// The weak Wolfe criteria were satisfied.
    /// </summary>
    WeakWolfeCriteria,
    /// <summary>
    /// The bound tolerance was reached.
    /// </summary>
    BoundTolerance,
    /// <summary>
    /// The strong Wolfe criteria were satisfied.
    /// </summary>
    StrongWolfeCriteria,
    /// <summary>
    /// The algorithm converged successfully.
    /// </summary>
    Converged,
    /// <summary>
    /// The algorithm was stopped manually.
    /// </summary>
    ManuallyStopped
  }
}
