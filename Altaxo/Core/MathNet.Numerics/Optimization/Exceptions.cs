// <copyright file="Exceptions.cs" company="Math.NET">
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

namespace Altaxo.Calc.Optimization
{
  /// <summary>
  /// Represents an optimization-specific exception.
  /// </summary>
  public class OptimizationException : Exception
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="OptimizationException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public OptimizationException(string message)
        : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="OptimizationException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that caused the current exception.</param>
    public OptimizationException(string message, Exception innerException)
        : base(message, innerException) { }
  }

  /// <summary>
  /// Represents an optimization failure caused by exceeding the maximum number of iterations.
  /// </summary>
  public class MaximumIterationsException : OptimizationException
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="MaximumIterationsException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public MaximumIterationsException(string message)
        : base(message) { }
  }

  /// <summary>
  /// Represents an optimization failure caused by an error while evaluating the objective function.
  /// </summary>
  public class EvaluationException : OptimizationException
  {
    /// <summary>
    /// Gets the objective function evaluation associated with the exception.
    /// </summary>
    public IObjectiveFunctionEvaluation ObjectiveFunction { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EvaluationException"/> class with a specified error message and evaluation.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="eval">The objective function evaluation associated with the error.</param>
    public EvaluationException(string message, IObjectiveFunctionEvaluation eval)
        : base(message)
    {
      ObjectiveFunction = eval;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EvaluationException"/> class with a specified error message, evaluation, and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="eval">The objective function evaluation associated with the error.</param>
    /// <param name="innerException">The exception that caused the current exception.</param>
    public EvaluationException(string message, IObjectiveFunctionEvaluation eval, Exception innerException)
        : base(message, innerException)
    {
      ObjectiveFunction = eval;
    }

  }

  /// <summary>
  /// Represents an optimization failure originating from an inner optimization step.
  /// </summary>
  public class InnerOptimizationException : OptimizationException
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="InnerOptimizationException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public InnerOptimizationException(string message)
        : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="InnerOptimizationException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that caused the current exception.</param>
    public InnerOptimizationException(string message, Exception innerException)
        : base(message, innerException) { }
  }

  /// <summary>
  /// Represents an optimization failure caused by an objective function incompatible with the algorithm.
  /// </summary>
  public class IncompatibleObjectiveException : OptimizationException
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="IncompatibleObjectiveException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public IncompatibleObjectiveException(string message)
        : base(message) { }
  }
}
