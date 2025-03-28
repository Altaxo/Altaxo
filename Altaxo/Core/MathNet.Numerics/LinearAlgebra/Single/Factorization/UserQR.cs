﻿// <copyright file="UserQR.cs" company="Math.NET">
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
using System.Linq;
using Altaxo.Calc.LinearAlgebra.Factorization;
using Altaxo.Calc.Threading;

namespace Altaxo.Calc.LinearAlgebra.Single.Factorization
{
  /// <summary>
  /// <para>A class which encapsulates the functionality of the QR decomposition.</para>
  /// <para>Any real square matrix A may be decomposed as A = QR where Q is an orthogonal matrix
  /// (its columns are orthogonal unit vectors meaning QTQ = I) and R is an upper triangular matrix
  /// (also called right triangular matrix).</para>
  /// </summary>
  /// <remarks>
  /// The computation of the QR decomposition is done at construction time by Householder transformation.
  /// </remarks>
  internal sealed class UserQR : QR
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="UserQR"/> class. This object will compute the
    /// QR factorization when the constructor is called and cache it's factorization.
    /// </summary>
    /// <param name="matrix">The matrix to factor.</param>
    /// <param name="method">The QR factorization method to use.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="matrix"/> is <c>null</c>.</exception>
    public static UserQR Create(Matrix<float> matrix, QRMethod method = QRMethod.Full)
    {
      if (matrix.RowCount < matrix.ColumnCount)
      {
        throw Matrix.DimensionsDontMatch<ArgumentException>(matrix);
      }

      Matrix<float> q;
      Matrix<float> r;

      var minmn = Math.Min(matrix.RowCount, matrix.ColumnCount);
      var u = new float[minmn][];

      if (method == QRMethod.Full)
      {
        r = matrix.Clone();
        q = Matrix<float>.Build.SameAs(matrix, matrix.RowCount, matrix.RowCount, fullyMutable: true);

        for (var i = 0; i < matrix.RowCount; i++)
        {
          q.At(i, i, 1.0f);
        }

        for (var i = 0; i < minmn; i++)
        {
          u[i] = GenerateColumn(r, i, i);
          ComputeQR(u[i], r, i, matrix.RowCount, i + 1, matrix.ColumnCount, Control.MaxDegreeOfParallelism);
        }

        for (var i = minmn - 1; i >= 0; i--)
        {
          ComputeQR(u[i], q, i, matrix.RowCount, i, matrix.RowCount, Control.MaxDegreeOfParallelism);
        }
      }
      else
      {
        q = matrix.Clone();

        for (var i = 0; i < minmn; i++)
        {
          u[i] = GenerateColumn(q, i, i);
          ComputeQR(u[i], q, i, matrix.RowCount, i + 1, matrix.ColumnCount, Control.MaxDegreeOfParallelism);
        }

        r = q.SubMatrix(0, matrix.ColumnCount, 0, matrix.ColumnCount);
        q.Clear();

        for (var i = 0; i < matrix.ColumnCount; i++)
        {
          q.At(i, i, 1.0f);
        }

        for (var i = minmn - 1; i >= 0; i--)
        {
          ComputeQR(u[i], q, i, matrix.RowCount, i, matrix.ColumnCount, Control.MaxDegreeOfParallelism);
        }
      }

      return new UserQR(q, r, method);
    }

    private UserQR(Matrix<float> q, Matrix<float> rFull, QRMethod method)
        : base(q, rFull, method)
    {
    }

    /// <summary>
    /// Generate column from initial matrix to work array
    /// </summary>
    /// <param name="a">Initial matrix</param>
    /// <param name="row">The first row</param>
    /// <param name="column">Column index</param>
    /// <returns>Generated vector</returns>
    private static float[] GenerateColumn(Matrix<float> a, int row, int column)
    {
      var ru = a.RowCount - row;
      var u = new float[ru];

      for (var i = row; i < a.RowCount; i++)
      {
        u[i - row] = a.At(i, row);
        a.At(i, row, 0.0f);
      }

      var norm = u.Sum(t => t * t);
      norm = (float)Math.Sqrt(norm);

      if (row == a.RowCount - 1 || norm == 0)
      {
        a.At(row, column, -u[0]);
        u[0] = (float)Constants.Sqrt2;
        return u;
      }

      var scale = 1.0f / norm;
      if (u[0] < 0.0)
      {
        scale *= -1.0f;
      }

      a.At(row, column, -1.0f / scale);

      for (var i = 0; i < ru; i++)
      {
        u[i] *= scale;
      }

      u[0] += 1.0f;
      var s = (float)Math.Sqrt(1.0 / u[0]);

      for (var i = 0; i < ru; i++)
      {
        u[i] *= s;
      }

      return u;
    }

    /// <summary>
    /// Perform calculation of Q or R
    /// </summary>
    /// <param name="u">Work array</param>
    /// <param name="a">Q or R matrices</param>
    /// <param name="rowStart">The first row</param>
    /// <param name="rowDim">The last row</param>
    /// <param name="columnStart">The first column</param>
    /// <param name="columnDim">The last column</param>
    /// <param name="availableCores">Number of available CPUs</param>
    private static void ComputeQR(float[] u, Matrix<float> a, int rowStart, int rowDim, int columnStart, int columnDim, int availableCores)
    {
      if (rowDim < rowStart || columnDim < columnStart)
      {
        return;
      }

      var tmpColCount = columnDim - columnStart;

      if ((availableCores > 1) && (tmpColCount > 200))
      {
        var tmpSplit = columnStart + (tmpColCount / 2);
        var tmpCores = availableCores / 2;

        CommonParallel.Invoke(
            () => ComputeQR(u, a, rowStart, rowDim, columnStart, tmpSplit, tmpCores),
            () => ComputeQR(u, a, rowStart, rowDim, tmpSplit, columnDim, tmpCores));
      }
      else
      {
        for (var j = columnStart; j < columnDim; j++)
        {
          var scale = 0.0f;
          for (var i = rowStart; i < rowDim; i++)
          {
            scale += u[i - rowStart] * a.At(i, j);
          }

          for (var i = rowStart; i < rowDim; i++)
          {
            a.At(i, j, a.At(i, j) - (u[i - rowStart] * scale));
          }
        }
      }
    }

    /// <summary>
    /// Solves a system of linear equations, <b>AX = B</b>, with A QR factorized.
    /// </summary>
    /// <param name="input">The right hand side <see cref="Matrix{T}"/>, <b>B</b>.</param>
    /// <param name="result">The left hand side <see cref="Matrix{T}"/>, <b>X</b>.</param>
    public override void Solve(Matrix<float> input, Matrix<float> result)
    {
      // The solution X should have the same number of columns as B
      if (input.ColumnCount != result.ColumnCount)
      {
        throw new ArgumentException("Matrix column dimensions must agree.");
      }

      // The dimension compatibility conditions for X = A\B require the two matrices A and B to have the same number of rows
      if (FullR.RowCount != input.RowCount)
      {
        throw new ArgumentException("Matrix row dimensions must agree.");
      }

      // The solution X row dimension is equal to the column dimension of A
      if (FullR.ColumnCount != result.RowCount)
      {
        throw new ArgumentException("Matrix column dimensions must agree.");
      }

      var inputCopy = input.Clone();

      // Compute Y = transpose(Q)*B
      var column = new float[FullR.RowCount];
      for (var j = 0; j < input.ColumnCount; j++)
      {
        for (var k = 0; k < FullR.RowCount; k++)
        {
          column[k] = inputCopy.At(k, j);
        }

        for (var i = 0; i < FullR.RowCount; i++)
        {
          float s = 0;
          for (var k = 0; k < FullR.RowCount; k++)
          {
            s += Q.At(k, i) * column[k];
          }

          inputCopy.At(i, j, s);
        }
      }

      // Solve R*X = Y;
      for (var k = FullR.ColumnCount - 1; k >= 0; k--)
      {
        for (var j = 0; j < input.ColumnCount; j++)
        {
          inputCopy.At(k, j, inputCopy.At(k, j) / FullR.At(k, k));
        }

        for (var i = 0; i < k; i++)
        {
          for (var j = 0; j < input.ColumnCount; j++)
          {
            inputCopy.At(i, j, inputCopy.At(i, j) - (inputCopy.At(k, j) * FullR.At(i, k)));
          }
        }
      }

      for (var i = 0; i < FullR.ColumnCount; i++)
      {
        for (var j = 0; j < inputCopy.ColumnCount; j++)
        {
          result.At(i, j, inputCopy.At(i, j));
        }
      }
    }

    /// <summary>
    /// Solves a system of linear equations, <b>Ax = b</b>, with A QR factorized.
    /// </summary>
    /// <param name="input">The right hand side vector, <b>b</b>.</param>
    /// <param name="result">The left hand side <see cref="Matrix{T}"/>, <b>x</b>.</param>
    public override void Solve(Vector<float> input, Vector<float> result)
    {
      // Ax=b where A is an m x n matrix
      // Check that b is a column vector with m entries
      if (FullR.RowCount != input.Count)
      {
        throw new ArgumentException("All vectors must have the same dimensionality.");
      }

      // Check that x is a column vector with n entries
      if (FullR.ColumnCount != result.Count)
      {
        throw Matrix.DimensionsDontMatch<ArgumentException>(FullR, result);
      }

      var inputCopy = input.Clone();

      // Compute Y = transpose(Q)*B
      var column = new float[FullR.RowCount];
      for (var k = 0; k < FullR.RowCount; k++)
      {
        column[k] = inputCopy[k];
      }

      for (var i = 0; i < FullR.RowCount; i++)
      {
        float s = 0;
        for (var k = 0; k < FullR.RowCount; k++)
        {
          s += Q.At(k, i) * column[k];
        }

        inputCopy[i] = s;
      }

      // Solve R*X = Y;
      for (var k = FullR.ColumnCount - 1; k >= 0; k--)
      {
        inputCopy[k] /= FullR.At(k, k);
        for (var i = 0; i < k; i++)
        {
          inputCopy[i] -= inputCopy[k] * FullR.At(i, k);
        }
      }

      for (var i = 0; i < FullR.ColumnCount; i++)
      {
        result[i] = inputCopy[i];
      }
    }
  }
}
