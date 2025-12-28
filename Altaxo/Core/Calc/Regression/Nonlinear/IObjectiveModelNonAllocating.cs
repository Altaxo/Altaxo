#region Copyright

/////////////////////////////////////////////////////////////////////////////
// Altaxo:  a data processing and data plotting program
// Copyright (C) 2002-2022 Dr. Dirk Lellinger
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
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System.Collections.Generic;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Optimization
{
  /// <summary>
  /// Extends <see cref="IObjectiveModel"/> to help avoid memory allocations during execution of Levenberg-Marquardt.
  /// </summary>
  /// <seealso cref="Altaxo.Calc.Optimization.IObjectiveModel" />
  public interface IObjectiveModelNonAllocating : IObjectiveModel
  {
    /// <summary>
    /// Sets the initial parameters and the information whether some of the parameters are fixed.
    /// </summary>
    /// <param name="parameters">The initial parameter values.</param>
    /// <param name="isFixed">
    /// Array with the same length as <paramref name="parameters"/>. For every fixed parameter, the corresponding element is <see langword="true"/>.
    /// </param>
    void SetParameters(IReadOnlyList<double> parameters, IReadOnlyList<bool> isFixed);

    /// <summary>
    /// Evaluates the model with the given parameter set.
    /// The resulting Chi² value (i.e., the sum of squares of deviations between data and fit model) can be accessed via <see cref="IObjectiveModelEvaluation.Value"/>.
    /// </summary>
    /// <param name="parameter">The parameter vector.</param>
    void EvaluateAt(IReadOnlyList<double> parameter);

    /// <summary>
    /// Gets the negative gradient vector. <c>-G = -J'(y - f(x; p))</c>.
    /// </summary>
    Vector<double> NegativeGradient { get; }

    /// <summary>
    /// Gets or sets an array of the same length as the parameter array.
    /// If an element in this vector is <see langword="true"/>, that parameter is either fixed by the user or fixed because the corresponding parameter has reached a boundary.
    /// This array is updated only at the end of the minimization process.
    /// </summary>
    IReadOnlyList<bool> IsFixedByUserOrBoundary { get; set; }
  }
}
