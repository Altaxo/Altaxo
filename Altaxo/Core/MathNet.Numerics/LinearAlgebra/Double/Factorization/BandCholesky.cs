// <copyright file="BandCholesky.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
//
// Copyright (c) 2009-2026 Math.NET
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

namespace Altaxo.Calc.LinearAlgebra.Double.Factorization
{
  /// <summary>
  /// Encapsulates the functionality of a Cholesky factorization for symmetric positive definite band matrices.
  /// </summary>
  internal sealed class BandCholesky : Cholesky
  {
    private readonly BandMatrix _factor;

    /// <summary>
    /// Creates a banded Cholesky factorization of the specified matrix.
    /// </summary>
    /// <param name="matrix">The matrix to factor.</param>
    /// <returns>The computed Cholesky factorization.</returns>
    public static BandCholesky Create(BandMatrix matrix)
    {
      if (matrix is null)
      {
        throw new ArgumentNullException(nameof(matrix));
      }

      if (matrix.RowCount != matrix.ColumnCount)
      {
        throw new ArgumentException("Matrix must be square.");
      }

      var factor = FactorCore(matrix);
      return new BandCholesky(factor);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BandCholesky"/> class from an already computed factor.
    /// </summary>
    /// <param name="factor">The lower-triangular band Cholesky factor.</param>
    private BandCholesky(BandMatrix factor)
      : base(factor)
    {
      _factor = factor;
    }

    /// <inheritdoc/>
    public override void Solve(Matrix<double> input, Matrix<double> result)
    {
      if (result.RowCount != input.RowCount)
      {
        throw new ArgumentException("Matrix row dimensions must agree.");
      }

      if (result.ColumnCount != input.ColumnCount)
      {
        throw new ArgumentException("Matrix column dimensions must agree.");
      }

      if (input.RowCount != Factor.RowCount)
      {
        throw Matrix.DimensionsDontMatch<ArgumentException>(input, Factor);
      }

      for (var column = 0; column < input.ColumnCount; column++)
      {
        var rhs = new double[input.RowCount];
        for (var row = 0; row < input.RowCount; row++)
        {
          rhs[row] = input[row, column];
        }

        var solution = SolveCore(rhs);
        for (var row = 0; row < result.RowCount; row++)
        {
          result[row, column] = solution[row];
        }
      }
    }

    /// <inheritdoc/>
    public override void Solve(Vector<double> input, Vector<double> result)
    {
      if (input.Count != result.Count)
      {
        throw new ArgumentException("All vectors must have the same dimensionality.");
      }

      if (input.Count != Factor.RowCount)
      {
        throw Matrix.DimensionsDontMatch<ArgumentException>(input, Factor);
      }

      var rhs = new double[input.Count];
      for (var i = 0; i < rhs.Length; i++)
      {
        rhs[i] = input[i];
      }

      var solution = SolveCore(rhs);
      for (var i = 0; i < solution.Length; i++)
      {
        result[i] = solution[i];
      }
    }

    /// <inheritdoc/>
    public override void Factorize(Matrix<double> matrix)
    {
      if (matrix.RowCount != matrix.ColumnCount)
      {
        throw new ArgumentException("Matrix must be square.");
      }

      if (matrix.RowCount != Factor.RowCount || matrix.ColumnCount != Factor.ColumnCount)
      {
        throw Matrix.DimensionsDontMatch<ArgumentException>(matrix, Factor);
      }

      if (matrix is not BandMatrix bandMatrix)
      {
        throw new NotSupportedException("Can only do banded Cholesky factorization for band matrices.");
      }

      var newFactor = FactorCore(bandMatrix);
      CopyFactor(newFactor, _factor);
    }

    /// <summary>
    /// Computes the lower-triangular band Cholesky factor for the specified matrix.
    /// </summary>
    /// <param name="matrix">The symmetric positive definite band matrix to factor.</param>
    /// <returns>The computed lower-triangular band factor.</returns>
    private static BandMatrix FactorCore(BandMatrix matrix)
    {
      EnsureSymmetric(matrix);

      var factor = new BandMatrix(matrix.RowCount, matrix.ColumnCount, matrix.LowerBandwidth, 0);
      for (var row = 0; row < matrix.RowCount; row++)
      {
        var columnStart = Math.Max(0, row - matrix.LowerBandwidth);
        for (var column = columnStart; column <= row; column++)
        {
          var sum = matrix[row, column];
          var kStart = Math.Max(0, Math.Max(row - matrix.LowerBandwidth, column - matrix.LowerBandwidth));
          for (var k = kStart; k < column; k++)
          {
            sum -= factor[row, k] * factor[column, k];
          }

          if (row == column)
          {
            if (sum <= 0.0)
            {
              throw new ArgumentException("Matrix must be symmetric positive definite.");
            }

            factor[row, row] = Math.Sqrt(sum);
          }
          else
          {
            var diagonal = factor[column, column];
            if (diagonal == 0.0)
            {
              throw new ArgumentException("Matrix must be symmetric positive definite.");
            }

            factor[row, column] = sum / diagonal;
          }
        }
      }

      return factor;
    }

    /// <summary>
    /// Verifies that the specified band matrix is symmetric.
    /// </summary>
    /// <param name="matrix">The matrix to validate.</param>
    private static void EnsureSymmetric(BandMatrix matrix)
    {
      for (var row = 0; row < matrix.RowCount; row++)
      {
        for (var column = row + 1; column < matrix.ColumnCount; column++)
        {
          if (matrix[row, column] != matrix[column, row])
          {
            throw new ArgumentException("Matrix must be symmetric positive definite.");
          }
        }
      }
    }

    /// <summary>
    /// Copies a computed factor into the target factor matrix.
    /// </summary>
    /// <param name="source">The source factor matrix.</param>
    /// <param name="target">The target factor matrix to overwrite.</param>
    private static void CopyFactor(BandMatrix source, BandMatrix target)
    {
      target.Clear();
      for (var row = 0; row < source.RowCount; row++)
      {
        var columnStart = Math.Max(0, row - source.LowerBandwidth);
        for (var column = columnStart; column <= row; column++)
        {
          target[row, column] = source[row, column];
        }
      }
    }

    /// <summary>
    /// Solves a factored linear system for a dense right-hand side vector.
    /// </summary>
    /// <param name="rhs">The right-hand side values.</param>
    /// <returns>The computed solution vector.</returns>
    private double[] SolveCore(double[] rhs)
    {
      var n = _factor.RowCount;
      var y = new double[n];
      for (var row = 0; row < n; row++)
      {
        var sum = rhs[row];
        var columnStart = Math.Max(0, row - _factor.LowerBandwidth);
        for (var column = columnStart; column < row; column++)
        {
          sum -= _factor[row, column] * y[column];
        }

        var diagonal = _factor[row, row];
        if (diagonal == 0.0)
        {
          throw new ArgumentException("Matrix must be symmetric positive definite.");
        }

        y[row] = sum / diagonal;
      }

      var x = new double[n];
      for (var row = n - 1; row >= 0; row--)
      {
        var sum = y[row];
        var rowEnd = Math.Min(n - 1, row + _factor.LowerBandwidth);
        for (var column = row + 1; column <= rowEnd; column++)
        {
          sum -= _factor[column, row] * x[column];
        }

        x[row] = sum / _factor[row, row];
      }

      return x;
    }
  }
}
