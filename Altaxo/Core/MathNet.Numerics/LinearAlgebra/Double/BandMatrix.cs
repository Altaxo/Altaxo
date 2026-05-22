// <copyright file="BandMatrix.cs" company="Math.NET">
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
using System.Collections.Generic;
using System.Diagnostics;
using Altaxo.Calc.LinearAlgebra.Double.Factorization;
using Altaxo.Calc.LinearAlgebra.Factorization;
using Altaxo.Calc.LinearAlgebra.Storage;

namespace Altaxo.Calc.LinearAlgebra.Double
{
  /// <summary>
  /// A matrix type for band matrices.
  /// </summary>
  [Serializable]
  [DebuggerDisplay("BandMatrix {RowCount}x{ColumnCount}-Double")]
  public class BandMatrix : Matrix
  {
    private readonly BandMatrixStorage<double> _storage;

    /// <summary>
    /// Create a new band matrix straight from an initialized matrix storage instance.
    /// The storage is used directly without copying.
    /// Intended for advanced scenarios where you're working directly with
    /// storage for performance or interop reasons.
    /// </summary>
    /// <param name="storage">The storage backing this matrix.</param>
    public BandMatrix(BandMatrixStorage<double> storage)
      : base(storage)
    {
      _storage = storage;
    }

    /// <summary>
    /// Gets the lower bandwidth of the matrix.
    /// </summary>
    public int LowerBandwidth => _storage.LowerBandwidth;

    /// <summary>
    /// Gets the upper bandwidth of the matrix.
    /// </summary>
    public int UpperBandwidth => _storage.UpperBandwidth;

    /// <summary>
    /// Create a new square band matrix with the given order and bandwidths.
    /// All stored cells of the matrix will be initialized to zero.
    /// </summary>
    /// <param name="order">The number of rows and columns.</param>
    /// <param name="lowerBandwidth">The number of sub-diagonals stored below the main diagonal.</param>
    /// <param name="upperBandwidth">The number of super-diagonals stored above the main diagonal.</param>
    public BandMatrix(int order, int lowerBandwidth, int upperBandwidth)
      : this(new BandMatrixStorage<double>(order, order, lowerBandwidth, upperBandwidth))
    {
    }

    /// <summary>
    /// Create a new band matrix with the given number of rows, columns, and bandwidths.
    /// All stored cells of the matrix will be initialized to zero.
    /// </summary>
    /// <param name="rows">The number of rows.</param>
    /// <param name="columns">The number of columns.</param>
    /// <param name="lowerBandwidth">The number of sub-diagonals stored below the main diagonal.</param>
    /// <param name="upperBandwidth">The number of super-diagonals stored above the main diagonal.</param>
    public BandMatrix(int rows, int columns, int lowerBandwidth, int upperBandwidth)
      : this(new BandMatrixStorage<double>(rows, columns, lowerBandwidth, upperBandwidth))
    {
    }

    /// <summary>
    /// Create a new band matrix with the given dimensions and bandwidths directly binding to a raw compact band array.
    /// The array is assumed to be in the compact band layout and is used directly without copying.
    /// Very efficient, but changes to the array and the matrix will affect each other.
    /// </summary>
    /// <param name="rows">The number of rows.</param>
    /// <param name="columns">The number of columns.</param>
    /// <param name="lowerBandwidth">The number of sub-diagonals stored below the main diagonal.</param>
    /// <param name="upperBandwidth">The number of super-diagonals stored above the main diagonal.</param>
    /// <param name="bandStorage">The compact band array backing this matrix.</param>
    public BandMatrix(int rows, int columns, int lowerBandwidth, int upperBandwidth, double[] bandStorage)
      : this(new BandMatrixStorage<double>(rows, columns, lowerBandwidth, upperBandwidth, bandStorage))
    {
    }

    /// <summary>
    /// Create a new band matrix as a copy of the given other matrix.
    /// This new matrix will be independent from the other matrix.
    /// A new memory block will be allocated for storing the matrix.
    /// </summary>
    /// <param name="matrix">The matrix to copy.</param>
    /// <param name="lowerBandwidth">The number of sub-diagonals stored below the main diagonal.</param>
    /// <param name="upperBandwidth">The number of super-diagonals stored above the main diagonal.</param>
    /// <returns>The created band matrix.</returns>
    public static BandMatrix OfMatrix(Matrix<double> matrix, int lowerBandwidth, int upperBandwidth)
    {
      return new BandMatrix(BandMatrixStorage<double>.OfMatrix(matrix.Storage, lowerBandwidth, upperBandwidth));
    }

    /// <summary>
    /// Create a new band matrix as a copy of the given two-dimensional array.
    /// This new matrix will be independent from the provided array.
    /// The array to copy from must not contain non-zero entries outside the requested band.
    /// </summary>
    /// <param name="array">The array to copy.</param>
    /// <param name="lowerBandwidth">The number of sub-diagonals stored below the main diagonal.</param>
    /// <param name="upperBandwidth">The number of super-diagonals stored above the main diagonal.</param>
    /// <returns>The created band matrix.</returns>
    public static BandMatrix OfArray(double[,] array, int lowerBandwidth, int upperBandwidth)
    {
      return new BandMatrix(BandMatrixStorage<double>.OfArray(array, lowerBandwidth, upperBandwidth));
    }

    /// <summary>
    /// Create a new band matrix and initialize each stored value using the provided init function.
    /// </summary>
    /// <param name="rows">The number of rows.</param>
    /// <param name="columns">The number of columns.</param>
    /// <param name="lowerBandwidth">The number of sub-diagonals stored below the main diagonal.</param>
    /// <param name="upperBandwidth">The number of super-diagonals stored above the main diagonal.</param>
    /// <param name="init">The initialization function for each stored entry.</param>
    /// <returns>The created band matrix.</returns>
    public static BandMatrix Create(int rows, int columns, int lowerBandwidth, int upperBandwidth, Func<int, int, double> init)
    {
      return new BandMatrix(BandMatrixStorage<double>.OfInit(rows, columns, lowerBandwidth, upperBandwidth, init));
    }

    /// <summary>
    /// Create a new band matrix and initialize each stored value to the same provided value.
    /// </summary>
    /// <param name="rows">The number of rows.</param>
    /// <param name="columns">The number of columns.</param>
    /// <param name="lowerBandwidth">The number of sub-diagonals stored below the main diagonal.</param>
    /// <param name="upperBandwidth">The number of super-diagonals stored above the main diagonal.</param>
    /// <param name="value">The value to initialize each stored element to.</param>
    /// <returns>The created band matrix.</returns>
    public static BandMatrix Create(int rows, int columns, int lowerBandwidth, int upperBandwidth, double value)
    {
      return new BandMatrix(BandMatrixStorage<double>.OfValue(rows, columns, lowerBandwidth, upperBandwidth, value));
    }

    /// <summary>
    /// Create a new band matrix as a copy of the given indexed enumerable.
    /// Keys must be provided at most once, zero is assumed if a key is omitted.
    /// This new matrix will be independent from the enumerable.
    /// A new memory block will be allocated for storing the matrix.
    /// </summary>
    /// <param name="rows">The number of rows.</param>
    /// <param name="columns">The number of columns.</param>
    /// <param name="lowerBandwidth">The number of sub-diagonals stored below the main diagonal.</param>
    /// <param name="upperBandwidth">The number of super-diagonals stored above the main diagonal.</param>
    /// <param name="enumerable">The indexed values to copy.</param>
    /// <returns>The created band matrix.</returns>
    public static BandMatrix OfIndexed(int rows, int columns, int lowerBandwidth, int upperBandwidth, IEnumerable<Tuple<int, int, double>> enumerable)
    {
      return new BandMatrix(BandMatrixStorage<double>.OfIndexedEnumerable(rows, columns, lowerBandwidth, upperBandwidth, enumerable));
    }

    /// <summary>
    /// Create a new band matrix as a copy of the given indexed enumerable.
    /// Keys must be provided at most once, zero is assumed if a key is omitted.
    /// This new matrix will be independent from the enumerable.
    /// A new memory block will be allocated for storing the matrix.
    /// </summary>
    /// <param name="rows">The number of rows.</param>
    /// <param name="columns">The number of columns.</param>
    /// <param name="lowerBandwidth">The number of sub-diagonals stored below the main diagonal.</param>
    /// <param name="upperBandwidth">The number of super-diagonals stored above the main diagonal.</param>
    /// <param name="enumerable">The indexed values to copy.</param>
    /// <returns>The created band matrix.</returns>
    public static BandMatrix OfIndexed(int rows, int columns, int lowerBandwidth, int upperBandwidth, IEnumerable<(int, int, double)> enumerable)
    {
      return new BandMatrix(BandMatrixStorage<double>.OfIndexedEnumerable(rows, columns, lowerBandwidth, upperBandwidth, enumerable));
    }

    /// <summary>
    /// Negate each element of this matrix and place the results into the result matrix.
    /// </summary>
    /// <param name="result">The result of the negation.</param>
    protected override void DoNegate(Matrix<double> result)
    {
      DoMultiply(-1.0, result);
    }

    /// <summary>
    /// Adds a scalar to each element of the matrix and stores the result in the result matrix.
    /// </summary>
    /// <param name="scalar">The scalar to add.</param>
    /// <param name="result">The matrix to store the result of the addition.</param>
    protected override void DoAdd(double scalar, Matrix<double> result)
    {
      base.DoAdd(scalar, result);
    }

    /// <summary>
    /// Adds another matrix to this matrix.
    /// </summary>
    /// <param name="other">The matrix to add to this matrix.</param>
    /// <param name="result">The matrix to store the result of the addition.</param>
    protected override void DoAdd(Matrix<double> other, Matrix<double> result)
    {
      result.Clear();

      if (other is BandMatrix bandOther)
      {
        CopyBandMatrixToResult(bandOther, result, 1.0);
      }
      else if (other.Storage is DiagonalMatrixStorage<double> diagonalStorage)
      {
        CopyDiagonalMatrixToResult(diagonalStorage, result, 1.0);
      }
      else if (other.Storage is SparseCompressedRowMatrixStorage<double> sparseStorage)
      {
        CopySparseMatrixToResult(sparseStorage, result, 1.0);
      }
      else if (other.Storage is DenseColumnMajorMatrixStorage<double> denseStorage)
      {
        CopyDenseMatrixToResult(denseStorage, result, 1.0);
      }
      else
      {
        other.CopyTo(result);
      }

      AddBandToResult(result, 1.0);
    }

    /// <summary>
    /// Subtracts a scalar from each element of the matrix and stores the result in the result matrix.
    /// </summary>
    /// <param name="scalar">The scalar to subtract.</param>
    /// <param name="result">The matrix to store the result of the subtraction.</param>
    protected override void DoSubtract(double scalar, Matrix<double> result)
    {
      base.DoSubtract(scalar, result);
    }

    /// <summary>
    /// Subtracts another matrix from this matrix.
    /// </summary>
    /// <param name="other">The matrix to subtract.</param>
    /// <param name="result">The matrix to store the result of the subtraction.</param>
    protected override void DoSubtract(Matrix<double> other, Matrix<double> result)
    {
      result.Clear();

      if (other is BandMatrix bandOther)
      {
        CopyBandMatrixToResult(bandOther, result, -1.0);
      }
      else if (other.Storage is DiagonalMatrixStorage<double> diagonalStorage)
      {
        CopyDiagonalMatrixToResult(diagonalStorage, result, -1.0);
      }
      else if (other.Storage is SparseCompressedRowMatrixStorage<double> sparseStorage)
      {
        CopySparseMatrixToResult(sparseStorage, result, -1.0);
      }
      else if (other.Storage is DenseColumnMajorMatrixStorage<double> denseStorage)
      {
        CopyDenseMatrixToResult(denseStorage, result, -1.0);
      }
      else
      {
        other.Negate(result);
      }

      AddBandToResult(result, 1.0);
    }

    /// <summary>
    /// Multiplies each element of the matrix by a scalar and places results into the result matrix.
    /// </summary>
    /// <param name="scalar">The scalar to multiply the matrix with.</param>
    /// <param name="result">The matrix to store the result of the multiplication.</param>
    protected override void DoMultiply(double scalar, Matrix<double> result)
    {
      if (scalar == 0.0)
      {
        result.Clear();
        return;
      }

      if (scalar == 1.0)
      {
        CopyTo(result);
        return;
      }

      if (result is BandMatrix bandResult && bandResult.LowerBandwidth == LowerBandwidth && bandResult.UpperBandwidth == UpperBandwidth)
      {
        for (var column = 0; column < ColumnCount; column++)
        {
          var rowStart = Math.Max(0, column - UpperBandwidth);
          var rowEnd = Math.Min(RowCount - 1, column + LowerBandwidth);
          for (var row = rowStart; row <= rowEnd; row++)
          {
            bandResult._storage.At(row, column, _storage.At(row, column) * scalar);
          }
        }

        return;
      }

      result.Clear();
      for (var column = 0; column < ColumnCount; column++)
      {
        var rowStart = Math.Max(0, column - UpperBandwidth);
        var rowEnd = Math.Min(RowCount - 1, column + LowerBandwidth);
        for (var row = rowStart; row <= rowEnd; row++)
        {
          result.At(row, column, this[row, column] * scalar);
        }
      }
    }

    /// <summary>
    /// Multiplies this matrix with a vector and places the results into the result vector.
    /// </summary>
    /// <param name="rightSide">The vector to multiply with.</param>
    /// <param name="result">The result of the multiplication.</param>
    protected override void DoMultiply(Vector<double> rightSide, Vector<double> result)
    {
      if (result.Storage is DenseVectorStorage<double> denseResult)
      {
        Array.Clear(denseResult.Data, 0, denseResult.Data.Length);
      }
      else
      {
        result.Clear();
      }

      for (var row = 0; row < RowCount; row++)
      {
        var sum = 0.0;
        var columnStart = Math.Max(0, row - LowerBandwidth);
        var columnEnd = Math.Min(ColumnCount - 1, row + UpperBandwidth);
        for (var column = columnStart; column <= columnEnd; column++)
        {
          sum += this[row, column] * rightSide[column];
        }

        result[row] = sum;
      }
    }

    /// <summary>
    /// Multiplies this matrix with another matrix and places the results into the result matrix.
    /// </summary>
    /// <param name="other">The matrix to multiply with.</param>
    /// <param name="result">The result of the multiplication.</param>
    protected override void DoMultiply(Matrix<double> other, Matrix<double> result)
    {
      result.Clear();

      if (other is BandMatrix bandOther)
      {
        MultiplyWithBandMatrix(bandOther, result);
        return;
      }

      if (other.Storage is DiagonalMatrixStorage<double> diagonalStorage)
      {
        MultiplyWithDiagonalMatrix(diagonalStorage, result);
        return;
      }

      if (other.Storage is SparseCompressedRowMatrixStorage<double> sparseStorage)
      {
        MultiplyWithSparseMatrix(sparseStorage, result);
        return;
      }

      if (other.Storage is DenseColumnMajorMatrixStorage<double> denseStorage)
      {
        MultiplyWithDenseMatrix(denseStorage, result);
        return;
      }

      MultiplyWithGenericMatrix(other, result);
    }

    /// <summary>
    /// Multiplies this matrix with transpose of another matrix and places the results into the result matrix.
    /// </summary>
    /// <param name="other">The matrix to multiply with.</param>
    /// <param name="result">The result of the multiplication.</param>
    protected override void DoTransposeAndMultiply(Matrix<double> other, Matrix<double> result)
    {
      result.Clear();

      if (other is BandMatrix bandOther)
      {
        TransposeAndMultiplyWithBandMatrix(bandOther, result);
        return;
      }

      if (other.Storage is DiagonalMatrixStorage<double> diagonalStorage)
      {
        TransposeAndMultiplyWithDiagonalMatrix(diagonalStorage, result);
        return;
      }

      if (other.Storage is SparseCompressedRowMatrixStorage<double> sparseStorage)
      {
        TransposeAndMultiplyWithSparseMatrix(sparseStorage, result);
        return;
      }

      if (other.Storage is DenseColumnMajorMatrixStorage<double> denseStorage)
      {
        TransposeAndMultiplyWithDenseMatrix(denseStorage, result);
        return;
      }

      TransposeAndMultiplyWithGenericMatrix(other, result);
    }

    /// <summary>
    /// Multiplies the transpose of this matrix with another matrix and places the results into the result matrix.
    /// </summary>
    /// <param name="other">The matrix to multiply with.</param>
    /// <param name="result">The result of the multiplication.</param>
    protected override void DoTransposeThisAndMultiply(Matrix<double> other, Matrix<double> result)
    {
      result.Clear();

      if (other is BandMatrix bandOther)
      {
        TransposeThisAndMultiplyWithBandMatrix(bandOther, result);
        return;
      }

      if (other.Storage is DiagonalMatrixStorage<double> diagonalStorage)
      {
        TransposeThisAndMultiplyWithDiagonalMatrix(diagonalStorage, result);
        return;
      }

      if (other.Storage is SparseCompressedRowMatrixStorage<double> sparseStorage)
      {
        TransposeThisAndMultiplyWithSparseMatrix(sparseStorage, result);
        return;
      }

      if (other.Storage is DenseColumnMajorMatrixStorage<double> denseStorage)
      {
        TransposeThisAndMultiplyWithDenseMatrix(denseStorage, result);
        return;
      }

      TransposeThisAndMultiplyWithGenericMatrix(other, result);
    }

    /// <summary>
    /// Multiplies this band matrix with another band matrix and accumulates the product in the result matrix.
    /// </summary>
    /// <param name="other">The right-hand band matrix.</param>
    /// <param name="result">The matrix receiving the product.</param>
    private void MultiplyWithBandMatrix(BandMatrix other, Matrix<double> result)
    {
      for (var row = 0; row < RowCount; row++)
      {
        var kStart = Math.Max(0, row - LowerBandwidth);
        var kEnd = Math.Min(ColumnCount - 1, row + UpperBandwidth);
        for (var k = kStart; k <= kEnd; k++)
        {
          var left = _storage.At(row, k);
          if (left == 0.0)
          {
            continue;
          }

          var columnStart = Math.Max(0, k - other.LowerBandwidth);
          var columnEnd = Math.Min(other.ColumnCount - 1, k + other.UpperBandwidth);
          for (var column = columnStart; column <= columnEnd; column++)
          {
            result.At(row, column, result.At(row, column) + (left * other._storage.At(k, column)));
          }
        }
      }
    }

    /// <summary>
    /// Adds the entries of this band matrix to the result matrix after scaling them.
    /// </summary>
    /// <param name="result">The matrix receiving the scaled band entries.</param>
    /// <param name="scale">The factor applied to each stored band entry before adding it.</param>
    private void AddBandToResult(Matrix<double> result, double scale)
    {
      for (var column = 0; column < ColumnCount; column++)
      {
        var rowStart = Math.Max(0, column - UpperBandwidth);
        var rowEnd = Math.Min(RowCount - 1, column + LowerBandwidth);
        for (var row = rowStart; row <= rowEnd; row++)
        {
          result.At(row, column, result.At(row, column) + (scale * _storage.At(row, column)));
        }
      }
    }

    /// <summary>
    /// Copies the stored entries of a band matrix into the result matrix after scaling them.
    /// </summary>
    /// <param name="other">The source band matrix.</param>
    /// <param name="result">The matrix receiving the scaled entries.</param>
    /// <param name="scale">The factor applied to each copied entry.</param>
    private static void CopyBandMatrixToResult(BandMatrix other, Matrix<double> result, double scale)
    {
      for (var column = 0; column < other.ColumnCount; column++)
      {
        var rowStart = Math.Max(0, column - other.UpperBandwidth);
        var rowEnd = Math.Min(other.RowCount - 1, column + other.LowerBandwidth);
        for (var row = rowStart; row <= rowEnd; row++)
        {
          result.At(row, column, scale * other._storage.At(row, column));
        }
      }
    }

    /// <summary>
    /// Copies the stored entries of a diagonal matrix into the result matrix after scaling them.
    /// </summary>
    /// <param name="other">The source diagonal storage.</param>
    /// <param name="result">The matrix receiving the scaled entries.</param>
    /// <param name="scale">The factor applied to each copied entry.</param>
    private static void CopyDiagonalMatrixToResult(DiagonalMatrixStorage<double> other, Matrix<double> result, double scale)
    {
      for (var index = 0; index < other.Data.Length; index++)
      {
        var value = other.Data[index];
        if (value != 0.0)
        {
          result.At(index, index, scale * value);
        }
      }
    }

    /// <summary>
    /// Copies the stored entries of a sparse matrix into the result matrix after scaling them.
    /// </summary>
    /// <param name="other">The source sparse storage.</param>
    /// <param name="result">The matrix receiving the scaled entries.</param>
    /// <param name="scale">The factor applied to each copied entry.</param>
    private static void CopySparseMatrixToResult(SparseCompressedRowMatrixStorage<double> other, Matrix<double> result, double scale)
    {
      for (var row = 0; row < other.RowCount; row++)
      {
        for (var index = other.RowPointers[row]; index < other.RowPointers[row + 1]; index++)
        {
          result.At(row, other.ColumnIndices[index], scale * other.Values[index]);
        }
      }
    }

    /// <summary>
    /// Copies the stored entries of a dense matrix into the result matrix after scaling them.
    /// </summary>
    /// <param name="other">The source dense storage.</param>
    /// <param name="result">The matrix receiving the scaled entries.</param>
    /// <param name="scale">The factor applied to each copied entry.</param>
    private static void CopyDenseMatrixToResult(DenseColumnMajorMatrixStorage<double> other, Matrix<double> result, double scale)
    {
      if (result.Storage is DenseColumnMajorMatrixStorage<double> denseResult)
      {
        var source = other.Data;
        var target = denseResult.Data;
        for (var i = 0; i < source.Length; i++)
        {
          target[i] = scale * source[i];
        }

        return;
      }

      for (var column = 0; column < other.ColumnCount; column++)
      {
        var columnOffset = column * other.RowCount;
        for (var row = 0; row < other.RowCount; row++)
        {
          var value = other.Data[columnOffset + row];
          if (value != 0.0)
          {
            result.At(row, column, scale * value);
          }
        }
      }
    }

    /// <summary>
    /// Multiplies this band matrix with a diagonal matrix and stores the product in the result matrix.
    /// </summary>
    /// <param name="other">The right-hand diagonal storage.</param>
    /// <param name="result">The matrix receiving the product.</param>
    private void MultiplyWithDiagonalMatrix(DiagonalMatrixStorage<double> other, Matrix<double> result)
    {
      var diagonalCount = Math.Min(other.Data.Length, ColumnCount);
      for (var column = 0; column < diagonalCount; column++)
      {
        var diagonal = other.Data[column];
        if (diagonal == 0.0)
        {
          continue;
        }

        var rowStart = Math.Max(0, column - UpperBandwidth);
        var rowEnd = Math.Min(RowCount - 1, column + LowerBandwidth);
        for (var row = rowStart; row <= rowEnd; row++)
        {
          result.At(row, column, _storage.At(row, column) * diagonal);
        }
      }
    }

    /// <summary>
    /// Multiplies this band matrix with a sparse matrix and stores the product in the result matrix.
    /// </summary>
    /// <param name="other">The right-hand sparse storage.</param>
    /// <param name="result">The matrix receiving the product.</param>
    private void MultiplyWithSparseMatrix(SparseCompressedRowMatrixStorage<double> other, Matrix<double> result)
    {
      for (var row = 0; row < RowCount; row++)
      {
        var kStart = Math.Max(0, row - LowerBandwidth);
        var kEnd = Math.Min(ColumnCount - 1, row + UpperBandwidth);
        for (var k = kStart; k <= kEnd; k++)
        {
          var left = _storage.At(row, k);
          if (left == 0.0)
          {
            continue;
          }

          for (var index = other.RowPointers[k]; index < other.RowPointers[k + 1]; index++)
          {
            var column = other.ColumnIndices[index];
            result.At(row, column, result.At(row, column) + (left * other.Values[index]));
          }
        }
      }
    }

    /// <summary>
    /// Multiplies this band matrix with a dense matrix and stores the product in the result matrix.
    /// </summary>
    /// <param name="other">The right-hand dense storage.</param>
    /// <param name="result">The matrix receiving the product.</param>
    private void MultiplyWithDenseMatrix(DenseColumnMajorMatrixStorage<double> other, Matrix<double> result)
    {
      for (var row = 0; row < RowCount; row++)
      {
        var kStart = Math.Max(0, row - LowerBandwidth);
        var kEnd = Math.Min(ColumnCount - 1, row + UpperBandwidth);
        for (var column = 0; column < other.ColumnCount; column++)
        {
          var sum = 0.0;
          var columnOffset = column * other.RowCount;
          for (var k = kStart; k <= kEnd; k++)
          {
            sum += _storage.At(row, k) * other.Data[columnOffset + k];
          }

          result.At(row, column, sum);
        }
      }
    }

    /// <summary>
    /// Multiplies this band matrix with a matrix using generic element access and stores the product in the result matrix.
    /// </summary>
    /// <param name="other">The right-hand matrix.</param>
    /// <param name="result">The matrix receiving the product.</param>
    private void MultiplyWithGenericMatrix(Matrix<double> other, Matrix<double> result)
    {
      for (var row = 0; row < RowCount; row++)
      {
        var kStart = Math.Max(0, row - LowerBandwidth);
        var kEnd = Math.Min(ColumnCount - 1, row + UpperBandwidth);
        for (var k = kStart; k <= kEnd; k++)
        {
          var left = _storage.At(row, k);
          if (left == 0.0)
          {
            continue;
          }

          for (var column = 0; column < other.ColumnCount; column++)
          {
            result.At(row, column, result.At(row, column) + (left * other.At(k, column)));
          }
        }
      }
    }

    /// <summary>
    /// Multiplies this band matrix with the transpose of another band matrix and stores the product in the result matrix.
    /// </summary>
    /// <param name="other">The matrix whose transpose is used on the right-hand side.</param>
    /// <param name="result">The matrix receiving the product.</param>
    private void TransposeAndMultiplyWithBandMatrix(BandMatrix other, Matrix<double> result)
    {
      for (var otherRow = 0; otherRow < other.RowCount; otherRow++)
      {
        var kStart = Math.Max(0, otherRow - other.LowerBandwidth);
        var kEnd = Math.Min(other.ColumnCount - 1, otherRow + other.UpperBandwidth);
        for (var k = kStart; k <= kEnd; k++)
        {
          var right = other._storage.At(otherRow, k);
          if (right == 0.0)
          {
            continue;
          }

          var rowStart = Math.Max(0, k - UpperBandwidth);
          var rowEnd = Math.Min(RowCount - 1, k + LowerBandwidth);
          for (var row = rowStart; row <= rowEnd; row++)
          {
            result.At(row, otherRow, result.At(row, otherRow) + (_storage.At(row, k) * right));
          }
        }
      }
    }

    /// <summary>
    /// Multiplies this band matrix with the transpose of a diagonal matrix and stores the product in the result matrix.
    /// </summary>
    /// <param name="other">The diagonal storage whose transpose is used on the right-hand side.</param>
    /// <param name="result">The matrix receiving the product.</param>
    private void TransposeAndMultiplyWithDiagonalMatrix(DiagonalMatrixStorage<double> other, Matrix<double> result)
    {
      var diagonalCount = Math.Min(other.Data.Length, ColumnCount);
      for (var column = 0; column < diagonalCount; column++)
      {
        var diagonal = other.Data[column];
        if (diagonal == 0.0)
        {
          continue;
        }

        var rowStart = Math.Max(0, column - UpperBandwidth);
        var rowEnd = Math.Min(RowCount - 1, column + LowerBandwidth);
        for (var row = rowStart; row <= rowEnd; row++)
        {
          result.At(row, column, _storage.At(row, column) * diagonal);
        }
      }
    }

    /// <summary>
    /// Multiplies this band matrix with the transpose of a sparse matrix and stores the product in the result matrix.
    /// </summary>
    /// <param name="other">The sparse storage whose transpose is used on the right-hand side.</param>
    /// <param name="result">The matrix receiving the product.</param>
    private void TransposeAndMultiplyWithSparseMatrix(SparseCompressedRowMatrixStorage<double> other, Matrix<double> result)
    {
      for (var otherRow = 0; otherRow < other.RowCount; otherRow++)
      {
        for (var index = other.RowPointers[otherRow]; index < other.RowPointers[otherRow + 1]; index++)
        {
          var k = other.ColumnIndices[index];
          var right = other.Values[index];
          var rowStart = Math.Max(0, k - UpperBandwidth);
          var rowEnd = Math.Min(RowCount - 1, k + LowerBandwidth);
          for (var row = rowStart; row <= rowEnd; row++)
          {
            result.At(row, otherRow, result.At(row, otherRow) + (_storage.At(row, k) * right));
          }
        }
      }
    }

    /// <summary>
    /// Multiplies this band matrix with the transpose of a dense matrix and stores the product in the result matrix.
    /// </summary>
    /// <param name="other">The dense storage whose transpose is used on the right-hand side.</param>
    /// <param name="result">The matrix receiving the product.</param>
    private void TransposeAndMultiplyWithDenseMatrix(DenseColumnMajorMatrixStorage<double> other, Matrix<double> result)
    {
      for (var row = 0; row < RowCount; row++)
      {
        var kStart = Math.Max(0, row - LowerBandwidth);
        var kEnd = Math.Min(ColumnCount - 1, row + UpperBandwidth);
        for (var column = 0; column < other.RowCount; column++)
        {
          var sum = 0.0;
          for (var k = kStart; k <= kEnd; k++)
          {
            sum += _storage.At(row, k) * other.Data[(k * other.RowCount) + column];
          }

          result.At(row, column, sum);
        }
      }
    }

    /// <summary>
    /// Multiplies this band matrix with the transpose of a matrix using generic element access and stores the product in the result matrix.
    /// </summary>
    /// <param name="other">The matrix whose transpose is used on the right-hand side.</param>
    /// <param name="result">The matrix receiving the product.</param>
    private void TransposeAndMultiplyWithGenericMatrix(Matrix<double> other, Matrix<double> result)
    {
      for (var row = 0; row < RowCount; row++)
      {
        var kStart = Math.Max(0, row - LowerBandwidth);
        var kEnd = Math.Min(ColumnCount - 1, row + UpperBandwidth);
        for (var column = 0; column < other.RowCount; column++)
        {
          var sum = 0.0;
          for (var k = kStart; k <= kEnd; k++)
          {
            sum += _storage.At(row, k) * other.At(column, k);
          }

          result.At(row, column, sum);
        }
      }
    }

    /// <summary>
    /// Multiplies the transpose of this band matrix with another band matrix and stores the product in the result matrix.
    /// </summary>
    /// <param name="other">The right-hand band matrix.</param>
    /// <param name="result">The matrix receiving the product.</param>
    private void TransposeThisAndMultiplyWithBandMatrix(BandMatrix other, Matrix<double> result)
    {
      for (var row = 0; row < other.RowCount; row++)
      {
        var otherColumnStart = Math.Max(0, row - other.LowerBandwidth);
        var otherColumnEnd = Math.Min(other.ColumnCount - 1, row + other.UpperBandwidth);
        var thisColumnStart = Math.Max(0, row - LowerBandwidth);
        var thisColumnEnd = Math.Min(ColumnCount - 1, row + UpperBandwidth);
        for (var thisColumn = thisColumnStart; thisColumn <= thisColumnEnd; thisColumn++)
        {
          var left = _storage.At(row, thisColumn);
          if (left == 0.0)
          {
            continue;
          }

          for (var otherColumn = otherColumnStart; otherColumn <= otherColumnEnd; otherColumn++)
          {
            result.At(thisColumn, otherColumn, result.At(thisColumn, otherColumn) + (left * other._storage.At(row, otherColumn)));
          }
        }
      }
    }

    /// <summary>
    /// Multiplies the transpose of this band matrix with a diagonal matrix and stores the product in the result matrix.
    /// </summary>
    /// <param name="other">The right-hand diagonal storage.</param>
    /// <param name="result">The matrix receiving the product.</param>
    private void TransposeThisAndMultiplyWithDiagonalMatrix(DiagonalMatrixStorage<double> other, Matrix<double> result)
    {
      var diagonalCount = Math.Min(other.Data.Length, RowCount);
      for (var row = 0; row < diagonalCount; row++)
      {
        var diagonal = other.Data[row];
        if (diagonal == 0.0)
        {
          continue;
        }

        var columnStart = Math.Max(0, row - LowerBandwidth);
        var columnEnd = Math.Min(ColumnCount - 1, row + UpperBandwidth);
        for (var column = columnStart; column <= columnEnd; column++)
        {
          result.At(column, row, _storage.At(row, column) * diagonal);
        }
      }
    }

    /// <summary>
    /// Multiplies the transpose of this band matrix with a sparse matrix and stores the product in the result matrix.
    /// </summary>
    /// <param name="other">The right-hand sparse storage.</param>
    /// <param name="result">The matrix receiving the product.</param>
    private void TransposeThisAndMultiplyWithSparseMatrix(SparseCompressedRowMatrixStorage<double> other, Matrix<double> result)
    {
      for (var row = 0; row < other.RowCount; row++)
      {
        var thisColumnStart = Math.Max(0, row - LowerBandwidth);
        var thisColumnEnd = Math.Min(ColumnCount - 1, row + UpperBandwidth);
        for (var thisColumn = thisColumnStart; thisColumn <= thisColumnEnd; thisColumn++)
        {
          var left = _storage.At(row, thisColumn);
          if (left == 0.0)
          {
            continue;
          }

          for (var index = other.RowPointers[row]; index < other.RowPointers[row + 1]; index++)
          {
            var otherColumn = other.ColumnIndices[index];
            result.At(thisColumn, otherColumn, result.At(thisColumn, otherColumn) + (left * other.Values[index]));
          }
        }
      }
    }

    /// <summary>
    /// Multiplies the transpose of this band matrix with a dense matrix and stores the product in the result matrix.
    /// </summary>
    /// <param name="other">The right-hand dense storage.</param>
    /// <param name="result">The matrix receiving the product.</param>
    private void TransposeThisAndMultiplyWithDenseMatrix(DenseColumnMajorMatrixStorage<double> other, Matrix<double> result)
    {
      for (var row = 0; row < other.RowCount; row++)
      {
        var thisColumnStart = Math.Max(0, row - LowerBandwidth);
        var thisColumnEnd = Math.Min(ColumnCount - 1, row + UpperBandwidth);
        for (var thisColumn = thisColumnStart; thisColumn <= thisColumnEnd; thisColumn++)
        {
          var left = _storage.At(row, thisColumn);
          if (left == 0.0)
          {
            continue;
          }

          for (var otherColumn = 0; otherColumn < other.ColumnCount; otherColumn++)
          {
            result.At(thisColumn, otherColumn, result.At(thisColumn, otherColumn) + (left * other.Data[(otherColumn * other.RowCount) + row]));
          }
        }
      }
    }

    /// <summary>
    /// Multiplies the transpose of this band matrix with a matrix using generic element access and stores the product in the result matrix.
    /// </summary>
    /// <param name="other">The right-hand matrix.</param>
    /// <param name="result">The matrix receiving the product.</param>
    private void TransposeThisAndMultiplyWithGenericMatrix(Matrix<double> other, Matrix<double> result)
    {
      for (var thisColumn = 0; thisColumn < ColumnCount; thisColumn++)
      {
        var rowStart = Math.Max(0, thisColumn - UpperBandwidth);
        var rowEnd = Math.Min(RowCount - 1, thisColumn + LowerBandwidth);
        for (var row = rowStart; row <= rowEnd; row++)
        {
          var left = _storage.At(row, thisColumn);
          if (left == 0.0)
          {
            continue;
          }

          for (var otherColumn = 0; otherColumn < other.ColumnCount; otherColumn++)
          {
            result.At(thisColumn, otherColumn, result.At(thisColumn, otherColumn) + (left * other.At(row, otherColumn)));
          }
        }
      }
    }

    /// <summary>
    /// Multiplies the transpose of this matrix with a vector and places the results into the result vector.
    /// </summary>
    /// <param name="rightSide">The vector to multiply with.</param>
    /// <param name="result">The result of the multiplication.</param>
    protected override void DoTransposeThisAndMultiply(Vector<double> rightSide, Vector<double> result)
    {
      if (result.Storage is DenseVectorStorage<double> denseResult)
      {
        Array.Clear(denseResult.Data, 0, denseResult.Data.Length);
      }
      else
      {
        result.Clear();
      }

      for (var column = 0; column < ColumnCount; column++)
      {
        var sum = 0.0;
        var rowStart = Math.Max(0, column - UpperBandwidth);
        var rowEnd = Math.Min(RowCount - 1, column + LowerBandwidth);
        for (var row = rowStart; row <= rowEnd; row++)
        {
          sum += this[row, column] * rightSide[row];
        }

        result[column] = sum;
      }
    }

    /// <inheritdoc/>
    public override void Solve(Vector<double> input, Vector<double> result)
    {
      object? tempStorage = null;
      Solve(input, result, ref tempStorage);
    }

    /// <summary>
    /// Solves a linear system represented by this matrix and the given right-hand side vector, placing the solution into the result vector.
    /// </summary>
    /// <param name="input">The right-hand side vector.</param>
    /// <param name="result">The vector receiving the solution.</param>
    /// <param name="tempStorage">Object that accomodates temporary storage. Can be reused in repeated calls.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public void Solve(Vector<double> input, Vector<double> result, ref object? tempStorage)
    {
      if (input is null)
      {
        throw new ArgumentNullException(nameof(input));
      }

      if (result is null)
      {
        throw new ArgumentNullException(nameof(result));
      }

      if (input.Count != RowCount)
      {
        throw DimensionsDontMatch<ArgumentException>(input, this);
      }

      if (result.Count != ColumnCount)
      {
        throw new ArgumentException("Result vector length must match the matrix column count.", nameof(result));
      }

      if (RowCount != ColumnCount)
      {
        base.Solve(input, result);
        return;
      }

      if (LowerBandwidth == 0 && UpperBandwidth == 0)
      {
        SolveDiagonal(input, result);
        return;
      }

      if (LowerBandwidth <= 1 && UpperBandwidth <= 1 && RowCount >= 2)
      {
        SolveTridiagonal(input, result, ref tempStorage);
        return;
      }

      if (LowerBandwidth <= 2 && UpperBandwidth <= 2 && RowCount >= 4)
      {
        SolvePentadiagonal(input, result, ref tempStorage);
        return;
      }

      base.Solve(input, result);
    }

    /// <summary>
    /// Solves a diagonal linear system represented by this matrix.
    /// </summary>
    /// <param name="input">The right-hand side vector.</param>
    /// <param name="result">The vector receiving the solution.</param>
    private void SolveDiagonal(Vector<double> input, Vector<double> result)
    {
      for (var i = 0; i < Math.Min(RowCount, ColumnCount); i++)
      {
        var diagonalValue = _storage.At(i, i);
        if (!(diagonalValue != 0))
        {
          throw new ArgumentException("Matrix must not be singular.");
        }

        result[i] = input[i] / diagonalValue;
      }
    }

    /// <summary>
    /// Solves a tridiagonal linear system represented by this matrix using a Thomas-algorithm-style elimination.
    /// </summary>
    /// <param name="input">The right-hand side vector.</param>
    /// <param name="result">The vector receiving the solution.</param>
    /// <param name="tempStorage">Object that accomodates temporary storage. Can be reused in repeated calls.</param>
    /// <remarks>
    /// <para>References:</para>
    /// <para>[1] Engeln-Müllges, Numerik-Algorithmen, 10th ed., page 165</para>
    /// </remarks>
    private void SolveTridiagonal(Vector<double> input, Vector<double> result, ref object? tempStorage)
    {
      var rhs = GetDenseInputData(input, result);
      var solution = GetDenseResultBuffer(result, out var copyBack);

      var n = Math.Min(RowCount, ColumnCount);

      double[] alpha, gamma, r;

      if (tempStorage is Tuple<double[], double[], double[]> tup &&
        tup.Item1?.Length >= n &&
        tup.Item2?.Length >= n &&
        tup.Item3?.Length >= n)
      {
        alpha = tup.Item1;
        gamma = tup.Item2;
        r = tup.Item3;
      }
      else
      {
        alpha = new double[n];
        gamma = new double[n];
        r = new double[n];
        tempStorage = new Tuple<double[], double[], double[]>(alpha, gamma, r);
      }

      // Start of algorithm 4.70, page 167, book of Engeln-Müllges, Numerik-Algorithmen, 10th ed.

      alpha[0] = _storage.At(0, 0);
      if (!(alpha[0] != 0))
      {
        throw new ArgumentException("Matrix must not be singular.");
      }

      gamma[0] = _storage.At(0, 1) / alpha[0];
      for (var i = 1; i < n - 1; ++i)
      {
        alpha[i] = _storage.At(i, i) - (_storage.At(i, i - 1) * gamma[i - 1]);
        if (!(alpha[i] != 0))
        {
          throw new ArgumentException("Matrix must not be singular.");
        }

        gamma[i] = _storage.At(i, i + 1) / alpha[i];
      }

      alpha[n - 1] = _storage.At(n - 1, n - 1) - (_storage.At(n - 1, n - 2) * gamma[n - 2]);
      if (!(alpha[n - 1] != 0))
      {
        throw new ArgumentException("Matrix must not be singular.");
      }

      // Forward elimination
      r[0] = rhs[0] / _storage.At(0, 0);
      for (var i = 1; i < n; ++i)
      {
        r[i] = (rhs[i] - (_storage.At(i, i - 1) * r[i - 1])) / alpha[i];
      }

      // Backward elimination
      solution[n - 1] = r[n - 1];
      for (var i = n - 2; i >= 0; --i)
      {
        solution[i] = r[i] - (gamma[i] * solution[i + 1]);
      }

      if (copyBack)
      {
        CopyBufferToResult(solution, result);
      }
    }

    /// <summary>
    /// Solves a pentadiagonal linear system represented by this matrix using specialized forward elimination and back substitution.
    /// </summary>
    /// <param name="input">The right-hand side vector.</param>
    /// <param name="result">The vector receiving the solution.</param>
    ///  /// <param name="tempStorage">Object that accomodates temporary storage. Can be reused in repeated calls.</param>
    /// <remarks>
    /// <para>References:</para>
    /// <para>[1] Engeln-Müllges, Numerik-Algorithmen, 10th ed., page 177</para>
    /// </remarks>
    private void SolvePentadiagonal(Vector<double> input, Vector<double> result, ref object? tempStorage)
    {
      var rhs = GetDenseInputData(input, result);
      var solution = GetDenseResultBuffer(result, out var copyBack);

      var n = Math.Min(RowCount, ColumnCount);

      double[] alpha, beta, gamma, delta, epsilon, r;

      if (tempStorage is Tuple<double[], double[], double[], double[], double[], double[]> tup &&
        tup.Item1?.Length >= n &&
        tup.Item2?.Length >= n &&
        tup.Item3?.Length >= n &&
        tup.Item4?.Length >= n &&
        tup.Item5?.Length >= n &&
        tup.Item6?.Length >= n)
      {
        alpha = tup.Item1;
        beta = tup.Item2;
        gamma = tup.Item3;
        delta = tup.Item4;
        epsilon = tup.Item5;
        r = tup.Item6;
      }
      else
      {
        alpha = new double[n];
        beta = new double[n];
        gamma = new double[n];
        delta = new double[n];
        epsilon = new double[n];
        r = new double[n];
        tempStorage = new Tuple<double[], double[], double[], double[], double[], double[]>(alpha, beta, gamma, delta, epsilon, r);
      }

      // Start of algorithm 4.77, page 178, book of Engeln-Müllges, "Numerik-Algorithmen", 10th ed.

      alpha[0] = _storage.At(0, 0); // 1.1
      if (!(alpha[0] != 0))
      {
        throw new ArgumentException("Matrix must not be singular.");
      }

      gamma[0] = _storage.At(0, 1) / alpha[0]; // 1.2
      delta[0] = _storage.At(0, 2) / alpha[0]; // 1.3
      beta[1] = _storage.At(1, 0); // 1.4 
      alpha[1] = _storage.At(1, 1) - (beta[1] * gamma[0]); // 1.5
      if (!(alpha[1] != 0))
      {
        throw new ArgumentException("Matrix must not be singular.");
      }

      gamma[1] = (_storage.At(1, 2) - (beta[1] * delta[0])) / alpha[1]; // 1.6
      delta[1] = _storage.At(1, 3) / alpha[1];

      for (var i = 2; i < n - 2; ++i)
      {
        beta[i] = _storage.At(i, i - 1) - (_storage.At(i, i - 2) * gamma[i - 2]); // 1.8.1
        alpha[i] = _storage.At(i, i) - (_storage.At(i, i - 2) * delta[i - 2]) - (beta[i] * gamma[i - 1]); // 1.8.2
        if (!(alpha[i] != 0))
        {
          throw new ArgumentException("Matrix must not be singular.");
        }

        gamma[i] = (_storage.At(i, i + 1) - (beta[i] * delta[i - 1])) / alpha[i]; // 1.8.3
        delta[i] = _storage.At(i, i + 2) / alpha[i]; // 1.8.4
      }

      beta[n - 2] = _storage.At(n - 2, n - 3) - (_storage.At(n - 2, n - 4) * gamma[n - 4]); // 1.9
      alpha[n - 2] = _storage.At(n - 2, n - 2) - (_storage.At(n - 2, n - 4) * delta[n - 4]) - (beta[n - 2] * gamma[n - 3]); // 1.10
      if (!(alpha[n - 2] != 0))
      {
        throw new ArgumentException("Matrix must not be singular.");
      }

      gamma[n - 2] = (_storage.At(n - 2, n - 1) - (beta[n - 2] * delta[n - 3])) / alpha[n - 2]; // 1.11
      beta[n - 1] = _storage.At(n - 1, n - 2) - (_storage.At(n - 1, n - 3) * gamma[n - 3]); // 1.12
      alpha[n - 1] = _storage.At(n - 1, n - 1) - (_storage.At(n - 1, n - 3) * delta[n - 3]) - (beta[n - 1] * gamma[n - 2]); // 1.13
      if (!(alpha[n - 1] != 0))
      {
        throw new ArgumentException("Matrix must not be singular.");
      }

      for (var i = 2; i < n; ++i) // 1.14
      {
        epsilon[i] = _storage.At(i, i - 2);
      }

      // forward elimination
      r[0] = rhs[0] / alpha[0]; // 2.1
      r[1] = (rhs[1] - (beta[1] * r[0])) / alpha[1]; // 2.2
      for (var i = 2; i < n; ++i) // 2.3
      {
        r[i] = (rhs[i] - (epsilon[i] * r[i - 2]) - (beta[i] * r[i - 1])) / alpha[i];
      }

      // backward elimination
      solution[n - 1] = r[n - 1]; // 3.1
      solution[n - 2] = r[n - 2] - (gamma[n - 2] * solution[n - 1]); // 3.2
      for (var i = n - 3; i >= 0; --i) // 3.3
      {
        solution[i] = r[i] - (gamma[i] * solution[i + 1]) - (delta[i] * solution[i + 2]);
      }

      if (copyBack)
      {
        CopyBufferToResult(solution, result);
      }
    }

    /// <summary>
    /// Solves a linear system represented by this matrix and the given right-hand side vector using Gaussian elimination with partial pivoting, placing the solution into the result vector.
    /// </summary>
    /// <param name="input">The right-hand side vector of the linear system.</param>
    /// <param name="result">The vector that will receive the solution.</param>
    /// <param name="tempStorage">Temporary storage object for internal use.</param>
    /// <exception cref="SingularMatrixException">Thrown if the matrix is singular.</exception>
    public void SolveByGaussianElimination(Vector<double> input, Vector<double> result, ref object? tempStorage)
    {
      int n = RowCount;
      var lowerBandwidth = LowerBandwidth;
      var upperBandwidth = UpperBandwidth;
      var width = lowerBandwidth + upperBandwidth + 1;

      // we convert the banded matrix into left spined storage format
      // this is useful because we can exchange the row of the matrix with no costs
      // and a is changed by the algorithm anyway
      double[][] a; // left spined storage for the matrix coefficients, where a[i][lowerBandwidth] is the diagonal element of row i
      double[] b;

      if (tempStorage is Tuple<double[][], double[]> tup &&
        tup.Item1?.Length >= n &&
        tup.Item2?.Length >= n)
      {
        a = tup.Item1;
        b = tup.Item2;
      }
      else
      {
        a = new double[n][];
        b = new double[n];
        tempStorage = new Tuple<double[][], double[]>(a, b);
      }

      // Fill a with the matrix coefficients and b with the input vector values, using left spined storage for a
      double[] aipk, ai;
      for (int i = 0; i < n; i++)
      {
        b[i] = input[i];
        a[i] = aipk = new double[width];
        for (int j = 0; j < width; j++)
        {
          var c = i + j - lowerBandwidth;
          aipk[j] = c >= 0 && c < ColumnCount ? _storage.At(i, c) : 0;
        }
      }

      // Start of algorithm 4.81, page 184, book of Engeln-Müllges, Numerik-Algorithmen, 10th ed.
      // note ml in the book is lowerBandwidth here, mr in the book is upperBandwidth here
      for (int i = 0; i < n - 1; ++i) // ATTENTION: this index is the column index
      {
        // Find row with largest absolute value of j-st element
        int maxIpk = i;
        double max_abs_aij = Math.Abs(a[i][lowerBandwidth]); // element aii
        int i_end = Math.Min(n, i + lowerBandwidth + 1);
        for (int ipk = i + 1; ipk < i_end; ++ipk)
        {
          aipk = a[ipk];
          var abs_aipk_i = Math.Abs(aipk[i - ipk + lowerBandwidth]); // 1.2.1
          if (abs_aipk_i > max_abs_aij)
          {
            maxIpk = ipk;
            max_abs_aij = abs_aipk_i;
          }
        }

        if (maxIpk != i) // switch rows 1.2.2
        {
          (a[i], a[maxIpk]) = (a[maxIpk], a[i]); // 1.2.2.2
          (b[i], b[maxIpk]) = (b[maxIpk], b[i]);
        }
        ai = a[i];

        var aii = ai[lowerBandwidth];
        if (!(aii != 0))
          throw new SingularMatrixException("Matrix is singular");

        i_end = Math.Min(n, i + lowerBandwidth + 1);
        for (int ipk = i + 1; ipk < i_end; ++ipk) // 1.2.3
        {
          aipk = a[ipk];
          var aij = (aipk[i - ipk + lowerBandwidth] /= aii); // 1.2.3.1 Element of L (left matrix)
          int ipj_end = Math.Min(i + upperBandwidth + 1, n);
          for (int ipj = i + 1; ipj < ipj_end; ++ipj) // 1.2.3.2, instead of j we use i+j
          {
            aipk[ipj - ipk + lowerBandwidth] -= aipk[i - ipk + lowerBandwidth] * ai[ipj - i + lowerBandwidth]; // 1.2.3.2
          }
          b[ipk] -= b[i] * aij;
        }
      }

      // now we have an L-R matrix

      // back substitution from bottom to top, using the R matrix
      // we use the fact that coefficients can not be more away from the diagonal than lowerBandwidth+upperBandwidth
      b[n - 1] /= a[n - 1][lowerBandwidth];
      result[n - 1] = b[n - 1];
      for (int i = n - 2; i >= 0; --i)
      {
        ai = a[i];
        var bi = b[i];
        int ipk_end = Math.Min(i + upperBandwidth + 1, n);
        for (int ipk = i + 1; ipk < ipk_end; ++ipk)
        {
          bi -= ai[ipk - i + lowerBandwidth] * b[ipk];
        }
        result[i] = b[i] = bi / ai[lowerBandwidth];
      }
    }

    /// <summary>
    /// Gets a dense buffer for the input vector, copying only when required to avoid aliasing with the result buffer.
    /// </summary>
    /// <param name="input">The input vector.</param>
    /// <param name="result">The result vector that may share storage with the input.</param>
    /// <returns>A dense buffer containing the input values.</returns>
    private static double[] GetDenseInputData(Vector<double> input, Vector<double> result)
    {
      if (input.Storage is DenseVectorStorage<double> denseInput)
      {
        if (result.Storage is DenseVectorStorage<double> denseResult && ReferenceEquals(denseInput.Data, denseResult.Data))
        {
          var inputCopy = new double[input.Count];
          Array.Copy(denseInput.Data, inputCopy, input.Count);
          return inputCopy;
        }

        return denseInput.Data;
      }

      var rhs = new double[input.Count];
      for (var i = 0; i < rhs.Length; i++)
      {
        rhs[i] = input[i];
      }

      return rhs;
    }

    /// <summary>
    /// Gets a dense buffer that can receive the computed solution.
    /// </summary>
    /// <param name="result">The result vector provided by the caller.</param>
    /// <param name="copyBack"><see langword="true"/> if the computed solution must be copied back into <paramref name="result"/> after solving; otherwise, <see langword="false"/>.</param>
    /// <returns>A dense buffer that can store the solution values.</returns>
    private static double[] GetDenseResultBuffer(Vector<double> result, out bool copyBack)
    {
      if (result.Storage is DenseVectorStorage<double> denseResult)
      {
        copyBack = false;
        return denseResult.Data;
      }

      copyBack = true;
      return new double[result.Count];
    }

    /// <summary>
    /// Copies a dense solution buffer into the target result vector.
    /// </summary>
    /// <param name="buffer">The dense buffer containing the solution.</param>
    /// <param name="result">The vector receiving the copied values.</param>
    private static void CopyBufferToResult(double[] buffer, Vector<double> result)
    {
      for (var i = 0; i < buffer.Length; i++)
      {
        result[i] = buffer[i];
      }
    }

    /// <inheritdoc/>
    public override Cholesky<double> Cholesky()
    {
      return BandCholesky.Create(this);
    }

    /// <inheritdoc/>
    public override LU<double> LU()
    {
      return BandLU.Create(this);
    }

    /// <inheritdoc/>
    public override QR<double> QR(QRMethod method = QRMethod.Thin)
    {
      return DenseQR.Create(DenseMatrix.OfMatrix(this), method);
    }

    /// <inheritdoc/>
    public override GramSchmidt<double> GramSchmidt()
    {
      return DenseGramSchmidt.Create(DenseMatrix.OfMatrix(this));
    }

    /// <inheritdoc/>
    public override Svd<double> Svd(bool computeVectors = true)
    {
      return DenseSvd.Create(DenseMatrix.OfMatrix(this), computeVectors);
    }

    /// <inheritdoc/>
    public override Evd<double> Evd(Symmetricity symmetricity = Symmetricity.Unknown)
    {
      return DenseEvd.Create(DenseMatrix.OfMatrix(this), symmetricity);
    }

    /// <inheritdoc/>
    public override Matrix<double> Inverse()
    {
      return LU().Inverse();
    }


  }
}
