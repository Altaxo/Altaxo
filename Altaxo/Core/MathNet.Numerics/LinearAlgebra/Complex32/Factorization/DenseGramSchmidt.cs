﻿// <copyright file="DenseGramSchmidt.cs" company="Math.NET">
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
using Altaxo.Calc.LinearAlgebra.Factorization;
using Altaxo.Calc.Providers.LinearAlgebra;

namespace Altaxo.Calc.LinearAlgebra.Complex32.Factorization
{
  using Calc;

  /// <summary>
  /// <para>A class which encapsulates the functionality of the QR decomposition Modified Gram-Schmidt Orthogonalization.</para>
  /// <para>Any complex square matrix A may be decomposed as A = QR where Q is an unitary mxn matrix and R is an nxn upper triangular matrix.</para>
  /// </summary>
  /// <remarks>
  /// The computation of the QR decomposition is done at construction time by modified Gram-Schmidt Orthogonalization.
  /// </remarks>
  internal sealed class DenseGramSchmidt : GramSchmidt
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DenseGramSchmidt"/> class. This object creates an unitary matrix
    /// using the modified Gram-Schmidt method.
    /// </summary>
    /// <param name="matrix">The matrix to factor.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="matrix"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">If <paramref name="matrix"/> row count is less then column count</exception>
    /// <exception cref="ArgumentException">If <paramref name="matrix"/> is rank deficient</exception>
    public static DenseGramSchmidt Create(Matrix<Complex32> matrix)
    {
      if (matrix.RowCount < matrix.ColumnCount)
      {
        throw Matrix.DimensionsDontMatch<ArgumentException>(matrix);
      }

      var q = (DenseMatrix)matrix.Clone();
      var r = new DenseMatrix(matrix.ColumnCount, matrix.ColumnCount);
      Factorize(q.Values, q.RowCount, q.ColumnCount, r.Values);

      return new DenseGramSchmidt(q, r);
    }

    private DenseGramSchmidt(Matrix<Complex32> q, Matrix<Complex32> rFull)
        : base(q, rFull)
    {
    }

    /// <summary>
    /// Factorize matrix using the modified Gram-Schmidt method.
    /// </summary>
    /// <param name="q">Initial matrix. On exit is replaced by <see cref="Matrix{T}"/> Q.</param>
    /// <param name="rowsQ">Number of rows in <see cref="Matrix{T}"/> Q.</param>
    /// <param name="columnsQ">Number of columns in <see cref="Matrix{T}"/> Q.</param>
    /// <param name="r">On exit is filled by <see cref="Matrix{T}"/> R.</param>
    private static void Factorize(Complex32[] q, int rowsQ, int columnsQ, Complex32[] r)
    {
      for (var k = 0; k < columnsQ; k++)
      {
        var norm = 0.0f;
        for (var i = 0; i < rowsQ; i++)
        {
          norm += q[(k * rowsQ) + i].Magnitude * q[(k * rowsQ) + i].Magnitude;
        }

        norm = (float)Math.Sqrt(norm);
        if (norm == 0.0f)
        {
          throw new ArgumentException("Matrix must not be rank deficient.");
        }

        r[(k * columnsQ) + k] = norm;
        for (var i = 0; i < rowsQ; i++)
        {
          q[(k * rowsQ) + i] /= norm;
        }

        for (var j = k + 1; j < columnsQ; j++)
        {
          var k1 = k;
          var j1 = j;

          var dot = Complex32.Zero;
          for (var index = 0; index < rowsQ; index++)
          {
            dot += q[(k1 * rowsQ) + index].Conjugate() * q[(j1 * rowsQ) + index];
          }

          r[(j * columnsQ) + k] = dot;
          for (var i = 0; i < rowsQ; i++)
          {
            var value = q[(j * rowsQ) + i] - (q[(k * rowsQ) + i] * dot);
            q[(j * rowsQ) + i] = value;
          }
        }
      }
    }

    /// <summary>
    /// Solves a system of linear equations, <b>AX = B</b>, with A QR factorized.
    /// </summary>
    /// <param name="input">The right hand side <see cref="Matrix{T}"/>, <b>B</b>.</param>
    /// <param name="result">The left hand side <see cref="Matrix{T}"/>, <b>X</b>.</param>
    public override void Solve(Matrix<Complex32> input, Matrix<Complex32> result)
    {
      // The solution X should have the same number of columns as B
      if (input.ColumnCount != result.ColumnCount)
      {
        throw new ArgumentException("Matrix column dimensions must agree.");
      }

      // The dimension compatibility conditions for X = A\B require the two matrices A and B to have the same number of rows
      if (Q.RowCount != input.RowCount)
      {
        throw new ArgumentException("Matrix row dimensions must agree.");
      }

      // The solution X row dimension is equal to the column dimension of A
      if (Q.ColumnCount != result.RowCount)
      {
        throw new ArgumentException("Matrix column dimensions must agree.");
      }

      if (input is DenseMatrix dinput && result is DenseMatrix dresult)
      {
        LinearAlgebraControl.Provider.QRSolveFactored(((DenseMatrix)Q).Values, ((DenseMatrix)FullR).Values, Q.RowCount, FullR.ColumnCount, null, dinput.Values, input.ColumnCount, dresult.Values, QRMethod.Thin);
      }
      else
      {
        throw new NotSupportedException("Can only do GramSchmidt factorization for dense matrices at the moment.");
      }
    }

    /// <summary>
    /// Solves a system of linear equations, <b>Ax = b</b>, with A QR factorized.
    /// </summary>
    /// <param name="input">The right hand side vector, <b>b</b>.</param>
    /// <param name="result">The left hand side <see cref="Matrix{T}"/>, <b>x</b>.</param>
    public override void Solve(Vector<Complex32> input, Vector<Complex32> result)
    {
      // Ax=b where A is an m x n matrix
      // Check that b is a column vector with m entries
      if (Q.RowCount != input.Count)
      {
        throw new ArgumentException("All vectors must have the same dimensionality.");
      }

      // Check that x is a column vector with n entries
      if (Q.ColumnCount != result.Count)
      {
        throw Matrix.DimensionsDontMatch<ArgumentException>(Q, result);
      }

      if (input is DenseVector dinput && result is DenseVector dresult)
      {
        LinearAlgebraControl.Provider.QRSolveFactored(((DenseMatrix)Q).Values, ((DenseMatrix)FullR).Values, Q.RowCount, FullR.ColumnCount, null, dinput.Values, 1, dresult.Values, QRMethod.Thin);
      }
      else
      {
        throw new NotSupportedException("Can only do GramSchmidt factorization for dense vectors at the moment.");
      }
    }
  }
}
