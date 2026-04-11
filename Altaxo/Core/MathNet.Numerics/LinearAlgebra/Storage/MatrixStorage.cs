// <copyright file="MatrixStorage.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
//
// Copyright (c) 2009-2015 Math.NET
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
using System.Runtime.Serialization;

namespace Altaxo.Calc.LinearAlgebra.Storage
{
  /// <summary>
  /// Represents the storage backing a matrix.
  /// </summary>
  /// <typeparam name="T">The element type.</typeparam>
  [Serializable]
  [DataContract(Namespace = "urn:MathNet/Numerics/LinearAlgebra")]
  public abstract partial class MatrixStorage<T> : IEquatable<MatrixStorage<T>>
        where T : struct, IEquatable<T>, IFormattable
  {
    // [ruegg] public fields are OK here

    /// <summary>
    /// Gets the default zero value for the storage element type.
    /// </summary>
    protected static readonly T Zero = BuilderInstance<T>.Matrix.Zero;

    /// <summary>
    /// Gets the number of rows in the matrix.
    /// </summary>
    [DataMember(Order = 1)]
    public readonly int RowCount;

    /// <summary>
    /// Gets the number of columns in the matrix.
    /// </summary>
    [DataMember(Order = 2)]
    public readonly int ColumnCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="MatrixStorage{T}"/> class.
    /// </summary>
    /// <param name="rowCount">The number of rows.</param>
    /// <param name="columnCount">The number of columns.</param>
    protected MatrixStorage(int rowCount, int columnCount)
    {
      if (rowCount < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(rowCount), "The number of rows of a matrix must be non-negative.");
      }

      if (columnCount < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(columnCount), "The number of columns of a matrix must be non-negative.");
      }

      RowCount = rowCount;
      ColumnCount = columnCount;
    }

    /// <summary>
    /// True if the matrix storage format is dense.
    /// </summary>
    public abstract bool IsDense { get; }

    /// <summary>
    /// True if all fields of this matrix can be set to any value.
    /// False if some fields are fixed, like on a diagonal matrix.
    /// </summary>
    public abstract bool IsFullyMutable { get; }

    /// <summary>
    /// True if the specified field can be set to any value.
    /// False if the field is fixed, like an off-diagonal field on a diagonal matrix.
    /// </summary>
    /// <param name="row">The row index of the element.</param>
    /// <param name="column">The column index of the element.</param>
    /// <returns><see langword="true"/> if the specified entry can be assigned any value; otherwise, <see langword="false"/>.</returns>
    public abstract bool IsMutableAt(int row, int column);

    /// <summary>
    /// Gets or sets the value at the given row and column, with range checking.
    /// </summary>
    /// <param name="row">
    /// The row of the element.
    /// </param>
    /// <param name="column">
    /// The column of the element.
    /// </param>
    /// <value>The value to get or set.</value>
    /// <remarks>This method is ranged checked. <see cref="At(int,int)"/> and <see cref="At(int,int,T)"/>
    /// to get and set values without range checking.</remarks>
    public T this[int row, int column]
    {
      get
      {
        ValidateRange(row, column);
        return At(row, column);
      }

      set
      {
        ValidateRange(row, column);
        At(row, column, value);
      }
    }

    /// <summary>
    /// Retrieves the requested element without range checking.
    /// </summary>
    /// <param name="row">
    /// The row of the element.
    /// </param>
    /// <param name="column">
    /// The column of the element.
    /// </param>
    /// <returns>
    /// The requested element.
    /// </returns>
    /// <remarks>Not range-checked.</remarks>
    public abstract T At(int row, int column);

    /// <summary>
    /// Sets the element without range checking.
    /// </summary>
    /// <param name="row"> The row of the element. </param>
    /// <param name="column"> The column of the element. </param>
    /// <param name="value"> The value to set the element to. </param>
    /// <remarks>WARNING: This method is not thread safe. Use "lock" with it and be sure to avoid deadlocks.</remarks>
    public abstract void At(int row, int column, T value);

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">
    /// An object to compare with this object.
    /// </param>
    /// <returns>
    /// <c>true</c> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <c>false</c>.
    /// </returns>
    public bool Equals(MatrixStorage<T> other)
    {
      // Reject equality when the argument is null or has a different shape.
      if (other == null)
      {
        return false;
      }
      if (ColumnCount != other.ColumnCount || RowCount != other.RowCount)
      {
        return false;
      }

      // Accept if the argument is the same object as this.
      if (ReferenceEquals(this, other))
      {
        return true;
      }

      // Perform element wise comparison.
      return Find2Unchecked(other, (a, b) => !a.Equals(b), Zeros.AllowSkip) == null;
    }

    /// <summary>
    /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
    /// </summary>
    /// <returns>
    /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
    /// </returns>
    /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. </param>
    public sealed override bool Equals(object obj)
    {
      return Equals(obj as MatrixStorage<T>);
    }

    /// <summary>
    /// Serves as a hash function for a particular type.
    /// </summary>
    /// <returns>
    /// A hash code for the current <see cref="T:System.Object"/>.
    /// </returns>
    public override int GetHashCode()
    {
      var hashNum = Math.Min(RowCount * ColumnCount, 25);
      int hash = 17;
      unchecked
      {
        for (var i = 0; i < hashNum; i++)
        {
          int col;
          int row = Math.DivRem(i, ColumnCount, out col);
          hash = hash * 31 + At(row, col).GetHashCode();
        }
      }
      return hash;
    }

    // CLEARING

    /// <summary>
    /// Sets all matrix entries to zero.
    /// </summary>
    public virtual void Clear()
    {
      for (var i = 0; i < RowCount; i++)
      {
        for (var j = 0; j < ColumnCount; j++)
        {
          At(i, j, Zero);
        }
      }
    }

    /// <summary>
    /// Sets a submatrix to zero.
    /// </summary>
    /// <param name="rowIndex">The starting row index.</param>
    /// <param name="rowCount">The number of rows to clear.</param>
    /// <param name="columnIndex">The starting column index.</param>
    /// <param name="columnCount">The number of columns to clear.</param>
    public void Clear(int rowIndex, int rowCount, int columnIndex, int columnCount)
    {
      if (rowCount < 1 || columnCount < 1)
      {
        return;
      }

      if (rowIndex + rowCount > RowCount || rowIndex < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(rowIndex));
      }

      if (columnIndex + columnCount > ColumnCount || columnIndex < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(columnIndex));
      }

      ClearUnchecked(rowIndex, rowCount, columnIndex, columnCount);
    }

    /// <summary>
    /// Sets a submatrix to zero without validation.
    /// </summary>
    /// <param name="rowIndex">The starting row index of the region to clear.</param>
    /// <param name="rowCount">The number of rows to clear.</param>
    /// <param name="columnIndex">The starting column index of the region to clear.</param>
    /// <param name="columnCount">The number of columns to clear.</param>
    internal virtual void ClearUnchecked(int rowIndex, int rowCount, int columnIndex, int columnCount)
    {
      for (var i = rowIndex; i < rowIndex + rowCount; i++)
      {
        for (var j = columnIndex; j < columnIndex + columnCount; j++)
        {
          At(i, j, Zero);
        }
      }
    }

    /// <summary>
    /// Sets the specified rows to zero.
    /// </summary>
    /// <param name="rowIndices">The row indices to clear.</param>
    public void ClearRows(int[] rowIndices)
    {
      if (rowIndices.Length == 0)
      {
        return;
      }

      for (int k = 0; k < rowIndices.Length; k++)
      {
        if (rowIndices[k] < 0 || rowIndices[k] >= RowCount)
        {
          throw new ArgumentOutOfRangeException(nameof(rowIndices));
        }
      }

      ClearRowsUnchecked(rowIndices);
    }

    /// <summary>
    /// Sets the specified columns to zero.
    /// </summary>
    /// <param name="columnIndices">The column indices to clear.</param>
    public void ClearColumns(int[] columnIndices)
    {
      if (columnIndices.Length == 0)
      {
        return;
      }

      for (int k = 0; k < columnIndices.Length; k++)
      {
        if ((uint)columnIndices[k] >= (uint)ColumnCount)
        {
          throw new ArgumentOutOfRangeException(nameof(columnIndices));
        }
      }

      ClearColumnsUnchecked(columnIndices);
    }

    /// <summary>
    /// Sets the specified rows to zero without validation.
    /// </summary>
    /// <param name="rowIndices">The row indices to clear.</param>
    internal virtual void ClearRowsUnchecked(int[] rowIndices)
    {
      for (var k = 0; k < rowIndices.Length; k++)
      {
        int row = rowIndices[k];
        for (var j = 0; j < ColumnCount; j++)
        {
          At(row, j, Zero);
        }
      }
    }

    /// <summary>
    /// Sets the specified columns to zero without validation.
    /// </summary>
    /// <param name="columnIndices">The column indices to clear.</param>
    internal virtual void ClearColumnsUnchecked(int[] columnIndices)
    {
      for (var k = 0; k < columnIndices.Length; k++)
      {
        int column = columnIndices[k];
        for (var i = 0; i < RowCount; i++)
        {
          At(i, column, Zero);
        }
      }
    }

    // MATRIX COPY

    /// <summary>
    /// Copies this matrix into another storage instance.
    /// </summary>
    /// <param name="target">The target storage.</param>
    /// <param name="existingData">How existing target data is handled.</param>
    public void CopyTo(MatrixStorage<T> target, ExistingData existingData = ExistingData.Clear)
    {
      if (target == null)
      {
        throw new ArgumentNullException(nameof(target));
      }

      if (ReferenceEquals(this, target))
      {
        return;
      }

      if (RowCount != target.RowCount || ColumnCount != target.ColumnCount)
      {
        var message = $"Matrix dimensions must agree: op1 is {RowCount}x{ColumnCount}, op2 is {target.RowCount}x{target.ColumnCount}.";
        throw new ArgumentException(message, nameof(target));
      }

      CopyToUnchecked(target, existingData);
    }

    /// <summary>
    /// Copies this matrix into another storage instance without validation.
    /// </summary>
    /// <param name="target">The target storage.</param>
    /// <param name="existingData">How existing target data is handled.</param>
    internal virtual void CopyToUnchecked(MatrixStorage<T> target, ExistingData existingData)
    {
      for (int j = 0; j < ColumnCount; j++)
      {
        for (int i = 0; i < RowCount; i++)
        {
          target.At(i, j, At(i, j));
        }
      }
    }

    /// <summary>
    /// Copies a submatrix into another storage instance.
    /// </summary>
    /// <param name="target">The target storage.</param>
    /// <param name="sourceRowIndex">The starting source row index.</param>
    /// <param name="targetRowIndex">The starting target row index.</param>
    /// <param name="rowCount">The number of rows to copy.</param>
    /// <param name="sourceColumnIndex">The starting source column index.</param>
    /// <param name="targetColumnIndex">The starting target column index.</param>
    /// <param name="columnCount">The number of columns to copy.</param>
    /// <param name="existingData">How existing target data is handled.</param>
    public void CopySubMatrixTo(MatrixStorage<T> target,
        int sourceRowIndex, int targetRowIndex, int rowCount,
        int sourceColumnIndex, int targetColumnIndex, int columnCount,
        ExistingData existingData = ExistingData.Clear)
    {
      if (target == null)
      {
        throw new ArgumentNullException(nameof(target));
      }

      if (rowCount == 0 || columnCount == 0)
      {
        return;
      }

      if (sourceRowIndex == 0 && targetRowIndex == 0 && rowCount == RowCount && rowCount == target.RowCount
          && sourceColumnIndex == 0 && targetColumnIndex == 0 && columnCount == ColumnCount && columnCount == target.ColumnCount)
      {
        CopyTo(target);
        return;
      }

      if (ReferenceEquals(this, target))
      {
        throw new NotSupportedException();
      }

      ValidateSubMatrixRange(target,
          sourceRowIndex, targetRowIndex, rowCount,
          sourceColumnIndex, targetColumnIndex, columnCount);

      CopySubMatrixToUnchecked(target, sourceRowIndex, targetRowIndex, rowCount,
          sourceColumnIndex, targetColumnIndex, columnCount, existingData);
    }

    /// <summary>
    /// Copies a submatrix into another storage instance without validation.
    /// </summary>
    /// <param name="target">The target storage.</param>
    /// <param name="sourceRowIndex">The starting source row index.</param>
    /// <param name="targetRowIndex">The starting target row index.</param>
    /// <param name="rowCount">The number of rows to copy.</param>
    /// <param name="sourceColumnIndex">The starting source column index.</param>
    /// <param name="targetColumnIndex">The starting target column index.</param>
    /// <param name="columnCount">The number of columns to copy.</param>
    /// <param name="existingData">How existing target data is handled.</param>
    internal virtual void CopySubMatrixToUnchecked(MatrixStorage<T> target,
        int sourceRowIndex, int targetRowIndex, int rowCount,
        int sourceColumnIndex, int targetColumnIndex, int columnCount,
        ExistingData existingData)
    {
      for (int j = sourceColumnIndex, jj = targetColumnIndex; j < sourceColumnIndex + columnCount; j++, jj++)
      {
        for (int i = sourceRowIndex, ii = targetRowIndex; i < sourceRowIndex + rowCount; i++, ii++)
        {
          target.At(ii, jj, At(i, j));
        }
      }
    }

    // ROW COPY

    /// <summary>
    /// Copies a row into vector storage.
    /// </summary>
    /// <param name="target">The target vector storage.</param>
    /// <param name="rowIndex">The row index to copy.</param>
    /// <param name="existingData">How existing target data is handled.</param>
    public void CopyRowTo(VectorStorage<T> target, int rowIndex, ExistingData existingData = ExistingData.Clear)
    {
      if (target == null)
      {
        throw new ArgumentNullException(nameof(target));
      }

      ValidateRowRange(target, rowIndex);
      CopySubRowToUnchecked(target, rowIndex, 0, 0, ColumnCount, existingData);
    }

    /// <summary>
    /// Copies part of a row into vector storage.
    /// </summary>
    /// <param name="target">The target vector storage.</param>
    /// <param name="rowIndex">The source row index.</param>
    /// <param name="sourceColumnIndex">The starting source column index.</param>
    /// <param name="targetColumnIndex">The starting target column index.</param>
    /// <param name="columnCount">The number of columns to copy.</param>
    /// <param name="existingData">How existing target data is handled.</param>
    public void CopySubRowTo(VectorStorage<T> target, int rowIndex,
        int sourceColumnIndex, int targetColumnIndex, int columnCount,
        ExistingData existingData = ExistingData.Clear)
    {
      if (target == null)
      {
        throw new ArgumentNullException(nameof(target));
      }

      if (columnCount == 0)
      {
        return;
      }

      ValidateSubRowRange(target, rowIndex, sourceColumnIndex, targetColumnIndex, columnCount);
      CopySubRowToUnchecked(target, rowIndex, sourceColumnIndex, targetColumnIndex, columnCount, existingData);
    }

    /// <summary>
    /// Copies part of a row into vector storage without validation.
    /// </summary>
    /// <param name="target">The target vector storage.</param>
    /// <param name="rowIndex">The source row index.</param>
    /// <param name="sourceColumnIndex">The starting source column index.</param>
    /// <param name="targetColumnIndex">The starting target column index.</param>
    /// <param name="columnCount">The number of columns to copy.</param>
    /// <param name="existingData">How existing target data is handled.</param>
    internal virtual void CopySubRowToUnchecked(VectorStorage<T> target, int rowIndex,
        int sourceColumnIndex, int targetColumnIndex, int columnCount, ExistingData existingData)
    {
      for (int j = sourceColumnIndex, jj = targetColumnIndex; j < sourceColumnIndex + columnCount; j++, jj++)
      {
        target.At(jj, At(rowIndex, j));
      }
    }

    // COLUMN COPY

    /// <summary>
    /// Copies a column into vector storage.
    /// </summary>
    /// <param name="target">The target vector storage.</param>
    /// <param name="columnIndex">The column index to copy.</param>
    /// <param name="existingData">How existing target data is handled.</param>
    public void CopyColumnTo(VectorStorage<T> target, int columnIndex, ExistingData existingData = ExistingData.Clear)
    {
      if (target == null)
      {
        throw new ArgumentNullException(nameof(target));
      }

      ValidateColumnRange(target, columnIndex);
      CopySubColumnToUnchecked(target, columnIndex, 0, 0, RowCount, existingData);
    }

    /// <summary>
    /// Copies part of a column into vector storage.
    /// </summary>
    /// <param name="target">The target vector storage.</param>
    /// <param name="columnIndex">The source column index.</param>
    /// <param name="sourceRowIndex">The starting source row index.</param>
    /// <param name="targetRowIndex">The starting target row index.</param>
    /// <param name="rowCount">The number of rows to copy.</param>
    /// <param name="existingData">How existing target data is handled.</param>
    public void CopySubColumnTo(VectorStorage<T> target, int columnIndex,
        int sourceRowIndex, int targetRowIndex, int rowCount,
        ExistingData existingData = ExistingData.Clear)
    {
      if (target == null)
      {
        throw new ArgumentNullException(nameof(target));
      }

      if (rowCount == 0)
      {
        return;
      }

      ValidateSubColumnRange(target, columnIndex, sourceRowIndex, targetRowIndex, rowCount);
      CopySubColumnToUnchecked(target, columnIndex, sourceRowIndex, targetRowIndex, rowCount, existingData);
    }

    /// <summary>
    /// Copies part of a column into vector storage without validation.
    /// </summary>
    /// <param name="target">The target vector storage.</param>
    /// <param name="columnIndex">The source column index.</param>
    /// <param name="sourceRowIndex">The starting source row index.</param>
    /// <param name="targetRowIndex">The starting target row index.</param>
    /// <param name="rowCount">The number of rows to copy.</param>
    /// <param name="existingData">How existing target data is handled.</param>
    internal virtual void CopySubColumnToUnchecked(VectorStorage<T> target, int columnIndex,
        int sourceRowIndex, int targetRowIndex, int rowCount, ExistingData existingData)
    {
      for (int i = sourceRowIndex, ii = targetRowIndex; i < sourceRowIndex + rowCount; i++, ii++)
      {
        target.At(ii, At(i, columnIndex));
      }
    }

    // TRANSPOSE

    /// <summary>
    /// Copies the transpose of this matrix into another storage instance.
    /// </summary>
    /// <param name="target">The target storage.</param>
    /// <param name="existingData">How existing target data is handled.</param>
    public void TransposeTo(MatrixStorage<T> target, ExistingData existingData = ExistingData.Clear)
    {
      if (target == null)
      {
        throw new ArgumentNullException(nameof(target));
      }

      if (RowCount != target.ColumnCount || ColumnCount != target.RowCount)
      {
        var message = $"Matrix dimensions must agree: op1 is {RowCount}x{ColumnCount}, op2 is {target.RowCount}x{target.ColumnCount}.";
        throw new ArgumentException(message, nameof(target));
      }

      if (ReferenceEquals(this, target))
      {
        TransposeSquareInplaceUnchecked();
        return;
      }

      TransposeToUnchecked(target, existingData);
    }

    /// <summary>
    /// Copies the transpose of this matrix into another storage instance without validation.
    /// </summary>
    /// <param name="target">The target storage.</param>
    /// <param name="existingData">How existing target data is handled.</param>
    internal virtual void TransposeToUnchecked(MatrixStorage<T> target, ExistingData existingData)
    {
      for (int j = 0; j < ColumnCount; j++)
      {
        for (int i = 0; i < RowCount; i++)
        {
          target.At(j, i, At(i, j));
        }
      }
    }

    /// <summary>
    /// Transposes a square matrix in place without validation.
    /// </summary>
    internal virtual void TransposeSquareInplaceUnchecked()
    {
      for (int j = 0; j < ColumnCount; j++)
      {
        for (int i = 0; i < j; i++)
        {
          T swap = At(i, j);
          At(i, j, At(j, i));
          At(j, i, swap);
        }
      }
    }

    // EXTRACT

    /// <summary>
    /// Creates a row-major array copy of the matrix data.
    /// </summary>
    /// <returns>A new row-major array.</returns>
    public virtual T[] ToRowMajorArray()
    {
      var ret = new T[RowCount * ColumnCount];
      for (int i = 0; i < RowCount; i++)
      {
        var offset = i * ColumnCount;
        for (int j = 0; j < ColumnCount; j++)
        {
          ret[offset + j] = At(i, j);
        }
      }
      return ret;
    }

    /// <summary>
    /// Creates a column-major array copy of the matrix data.
    /// </summary>
    /// <returns>A new column-major array.</returns>
    public virtual T[] ToColumnMajorArray()
    {
      var ret = new T[RowCount * ColumnCount];
      for (int j = 0; j < ColumnCount; j++)
      {
        var offset = j * RowCount;
        for (int i = 0; i < RowCount; i++)
        {
          ret[offset + i] = At(i, j);
        }
      }
      return ret;
    }

    /// <summary>
    /// Creates jagged row-array copies of the matrix data.
    /// </summary>
    /// <returns>A new array of row arrays.</returns>
    public virtual T[][] ToRowArrays()
    {
      var ret = new T[RowCount][];
      for (int i = 0; i < RowCount; i++)
      {
        var row = new T[ColumnCount];
        for (int j = 0; j < ColumnCount; j++)
        {
          row[j] = At(i, j);
        }
        ret[i] = row;
      }
      return ret;
    }

    /// <summary>
    /// Creates jagged column-array copies of the matrix data.
    /// </summary>
    /// <returns>A new array of column arrays.</returns>
    public virtual T[][] ToColumnArrays()
    {
      var ret = new T[ColumnCount][];
      for (int j = 0; j < ColumnCount; j++)
      {
        var column = new T[RowCount];
        for (int i = 0; i < RowCount; i++)
        {
          column[i] = At(i, j);
        }
        ret[j] = column;
      }
      return ret;
    }

    /// <summary>
    /// Creates a rectangular array copy of the matrix data.
    /// </summary>
    /// <returns>A new two-dimensional array.</returns>
    public virtual T[,] ToArray()
    {
      var ret = new T[RowCount, ColumnCount];
      for (int i = 0; i < RowCount; i++)
      {
        for (int j = 0; j < ColumnCount; j++)
        {
          ret[i, j] = At(i, j);
        }
      }
      return ret;
    }

    /// <summary>
    /// Gets the underlying row-major array when directly available.
    /// </summary>
    /// <returns>The underlying row-major array, or <see langword="null"/> if unavailable.</returns>
    public virtual T[] AsRowMajorArray()
    {
      return null;
    }

    /// <summary>
    /// Gets the underlying column-major array when directly available.
    /// </summary>
    /// <returns>The underlying column-major array, or <see langword="null"/> if unavailable.</returns>
    public virtual T[] AsColumnMajorArray()
    {
      return null;
    }

    /// <summary>
    /// Gets the underlying row arrays when directly available.
    /// </summary>
    /// <returns>The underlying row arrays, or <see langword="null"/> if unavailable.</returns>
    public virtual T[][] AsRowArrays()
    {
      return null;
    }

    /// <summary>
    /// Gets the underlying column arrays when directly available.
    /// </summary>
    /// <returns>The underlying column arrays, or <see langword="null"/> if unavailable.</returns>
    public virtual T[][] AsColumnArrays()
    {
      return null;
    }

    /// <summary>
    /// Gets the underlying two-dimensional array when directly available.
    /// </summary>
    /// <returns>The underlying two-dimensional array, or <see langword="null"/> if unavailable.</returns>
    public virtual T[,] AsArray()
    {
      return null;
    }

    // ENUMERATION

    /// <summary>
    /// Enumerates all matrix values.
    /// </summary>
    /// <returns>A sequence of all matrix values.</returns>
    public virtual IEnumerable<T> Enumerate()
    {
      for (int i = 0; i < RowCount; i++)
      {
        for (int j = 0; j < ColumnCount; j++)
        {
          yield return At(i, j);
        }
      }
    }

    /// <summary>
    /// Enumerates all matrix values together with their indices.
    /// </summary>
    /// <returns>A sequence of indexed matrix values.</returns>
    public virtual IEnumerable<(int, int, T)> EnumerateIndexed()
    {
      for (int i = 0; i < RowCount; i++)
      {
        for (int j = 0; j < ColumnCount; j++)
        {
          yield return (i, j, At(i, j));
        }
      }
    }

    /// <summary>
    /// Enumerates all non-zero matrix values.
    /// </summary>
    /// <returns>A sequence of non-zero matrix values.</returns>
    public virtual IEnumerable<T> EnumerateNonZero()
    {
      for (int i = 0; i < RowCount; i++)
      {
        for (int j = 0; j < ColumnCount; j++)
        {
          var x = At(i, j);
          if (!Zero.Equals(x))
          {
            yield return x;
          }
        }
      }
    }

    /// <summary>
    /// Enumerates all non-zero matrix values together with their indices.
    /// </summary>
    /// <returns>A sequence of indexed non-zero matrix values.</returns>
    public virtual IEnumerable<(int, int, T)> EnumerateNonZeroIndexed()
    {
      for (int i = 0; i < RowCount; i++)
      {
        for (int j = 0; j < ColumnCount; j++)
        {
          var x = At(i, j);
          if (!Zero.Equals(x))
          {
            yield return (i, j, x);
          }
        }
      }
    }

    // FIND

    /// <summary>
    /// Finds the first element matching a predicate.
    /// </summary>
    /// <param name="predicate">The predicate used to test values.</param>
    /// <param name="zeros">How zeros are handled during the search.</param>
    /// <returns>The first matching indexed value, or <see langword="null"/> if none is found.</returns>
    public virtual Tuple<int, int, T> Find(Func<T, bool> predicate, Zeros zeros)
    {
      for (int i = 0; i < RowCount; i++)
      {
        for (int j = 0; j < ColumnCount; j++)
        {
          var item = At(i, j);
          if (predicate(item))
          {
            return new Tuple<int, int, T>(i, j, item);
          }
        }
      }
      return null;
    }

    /// <summary>
    /// Finds the first pair of elements from two matrices that matches a predicate.
    /// </summary>
    /// <typeparam name="TOther">The element type of the other matrix.</typeparam>
    /// <param name="other">The other matrix storage.</param>
    /// <param name="predicate">The predicate used to test value pairs.</param>
    /// <param name="zeros">How zeros are handled during the search.</param>
    /// <returns>The first matching indexed value pair, or <see langword="null"/> if none is found.</returns>
    public Tuple<int, int, T, TOther> Find2<TOther>(MatrixStorage<TOther> other, Func<T, TOther, bool> predicate, Zeros zeros)
        where TOther : struct, IEquatable<TOther>, IFormattable
    {
      if (other == null)
      {
        throw new ArgumentNullException(nameof(other));
      }

      if (RowCount != other.RowCount || ColumnCount != other.ColumnCount)
      {
        var message = $"Matrix dimensions must agree: op1 is {RowCount}x{ColumnCount}, op2 is {other.RowCount}x{other.ColumnCount}.";
        throw new ArgumentException(message, nameof(other));
      }

      return Find2Unchecked(other, predicate, zeros);
    }

    /// <summary>
    /// Finds the first pair of elements from two matrices that matches a predicate without validation.
    /// </summary>
    /// <typeparam name="TOther">The element type of the other matrix.</typeparam>
    /// <param name="other">The other matrix storage.</param>
    /// <param name="predicate">The predicate used to test each pair of values.</param>
    /// <param name="zeros">Controls whether zero values may be skipped.</param>
    /// <returns>The first matching indexed value pair, or <c>null</c> if none is found.</returns>
    internal virtual Tuple<int, int, T, TOther> Find2Unchecked<TOther>(MatrixStorage<TOther> other, Func<T, TOther, bool> predicate, Zeros zeros)
        where TOther : struct, IEquatable<TOther>, IFormattable
    {
      for (int i = 0; i < RowCount; i++)
      {
        for (int j = 0; j < ColumnCount; j++)
        {
          var item = At(i, j);
          var otherItem = other.At(i, j);
          if (predicate(item, otherItem))
          {
            return new Tuple<int, int, T, TOther>(i, j, item, otherItem);
          }
        }
      }
      return null;
    }

    // FUNCTIONAL COMBINATORS: MAP

    /// <summary>
    /// Applies a mapping function to each value in place.
    /// </summary>
    /// <param name="f">The mapping function.</param>
    /// <param name="zeros">How zeros are handled during the mapping.</param>
    public virtual void MapInplace(Func<T, T> f, Zeros zeros)
    {
      for (int i = 0; i < RowCount; i++)
      {
        for (int j = 0; j < ColumnCount; j++)
        {
          At(i, j, f(At(i, j)));
        }
      }
    }

    /// <summary>
    /// Applies an indexed mapping function to each value in place.
    /// </summary>
    /// <param name="f">The indexed mapping function.</param>
    /// <param name="zeros">How zeros are handled during the mapping.</param>
    public virtual void MapIndexedInplace(Func<int, int, T, T> f, Zeros zeros)
    {
      for (int i = 0; i < RowCount; i++)
      {
        for (int j = 0; j < ColumnCount; j++)
        {
          At(i, j, f(i, j, At(i, j)));
        }
      }
    }

    /// <summary>
    /// Maps each value into another matrix storage.
    /// </summary>
    /// <typeparam name="TU">The target element type.</typeparam>
    /// <param name="target">The target storage.</param>
    /// <param name="f">The mapping function.</param>
    /// <param name="zeros">How zeros are handled during the mapping.</param>
    /// <param name="existingData">How existing target data is handled.</param>
    public void MapTo<TU>(MatrixStorage<TU> target, Func<T, TU> f, Zeros zeros, ExistingData existingData)
        where TU : struct, IEquatable<TU>, IFormattable
    {
      if (target == null)
      {
        throw new ArgumentNullException(nameof(target));
      }

      if (RowCount != target.RowCount || ColumnCount != target.ColumnCount)
      {
        var message = $"Matrix dimensions must agree: op1 is {RowCount}x{ColumnCount}, op2 is {target.RowCount}x{target.ColumnCount}.";
        throw new ArgumentException(message, nameof(target));
      }

      MapToUnchecked(target, f, zeros, existingData);
    }

    /// <summary>
    /// Maps each value into another matrix storage without validation.
    /// </summary>
    /// <typeparam name="TU">The target element type.</typeparam>
    /// <param name="target">The target storage.</param>
    /// <param name="f">The mapping function.</param>
    /// <param name="zeros">How zeros are handled during the mapping.</param>
    /// <param name="existingData">How existing target data is handled.</param>
    internal virtual void MapToUnchecked<TU>(MatrixStorage<TU> target, Func<T, TU> f, Zeros zeros, ExistingData existingData)
        where TU : struct, IEquatable<TU>, IFormattable
    {
      for (int i = 0; i < RowCount; i++)
      {
        for (int j = 0; j < ColumnCount; j++)
        {
          target.At(i, j, f(At(i, j)));
        }
      }
    }

    /// <summary>
    /// Maps each indexed value into another matrix storage.
    /// </summary>
    /// <typeparam name="TU">The target element type.</typeparam>
    /// <param name="target">The target storage.</param>
    /// <param name="f">The indexed mapping function.</param>
    /// <param name="zeros">How zeros are handled during the mapping.</param>
    /// <param name="existingData">How existing target data is handled.</param>
    public void MapIndexedTo<TU>(MatrixStorage<TU> target, Func<int, int, T, TU> f, Zeros zeros, ExistingData existingData)
        where TU : struct, IEquatable<TU>, IFormattable
    {
      if (target == null)
      {
        throw new ArgumentNullException(nameof(target));
      }

      if (RowCount != target.RowCount || ColumnCount != target.ColumnCount)
      {
        var message = $"Matrix dimensions must agree: op1 is {RowCount}x{ColumnCount}, op2 is {target.RowCount}x{target.ColumnCount}.";
        throw new ArgumentException(message, nameof(target));
      }

      MapIndexedToUnchecked(target, f, zeros, existingData);
    }

    /// <summary>
    /// Maps each indexed value into another matrix storage without validation.
    /// </summary>
    /// <typeparam name="TU">The target element type.</typeparam>
    /// <param name="target">The target storage.</param>
    /// <param name="f">The indexed mapping function.</param>
    /// <param name="zeros">How zeros are handled during the mapping.</param>
    /// <param name="existingData">How existing target data is handled.</param>
    internal virtual void MapIndexedToUnchecked<TU>(MatrixStorage<TU> target, Func<int, int, T, TU> f, Zeros zeros, ExistingData existingData)
        where TU : struct, IEquatable<TU>, IFormattable
    {
      for (int j = 0; j < ColumnCount; j++)
      {
        for (int i = 0; i < RowCount; i++)
        {
          target.At(i, j, f(i, j, At(i, j)));
        }
      }
    }

    /// <summary>
    /// Maps an indexed submatrix into another matrix storage.
    /// </summary>
    /// <typeparam name="TU">The target element type.</typeparam>
    /// <param name="target">The target storage.</param>
    /// <param name="f">The indexed mapping function.</param>
    /// <param name="sourceRowIndex">The starting source row index.</param>
    /// <param name="targetRowIndex">The starting target row index.</param>
    /// <param name="rowCount">The number of rows to map.</param>
    /// <param name="sourceColumnIndex">The starting source column index.</param>
    /// <param name="targetColumnIndex">The starting target column index.</param>
    /// <param name="columnCount">The number of columns to map.</param>
    /// <param name="zeros">How zeros are handled during the mapping.</param>
    /// <param name="existingData">How existing target data is handled.</param>
    public void MapSubMatrixIndexedTo<TU>(MatrixStorage<TU> target, Func<int, int, T, TU> f,
        int sourceRowIndex, int targetRowIndex, int rowCount,
        int sourceColumnIndex, int targetColumnIndex, int columnCount,
        Zeros zeros, ExistingData existingData)
        where TU : struct, IEquatable<TU>, IFormattable
    {
      if (target == null)
      {
        throw new ArgumentNullException(nameof(target));
      }

      if (rowCount == 0 || columnCount == 0)
      {
        return;
      }

      if (ReferenceEquals(this, target))
      {
        throw new NotSupportedException();
      }

      ValidateSubMatrixRange(target,
          sourceRowIndex, targetRowIndex, rowCount,
          sourceColumnIndex, targetColumnIndex, columnCount);

      MapSubMatrixIndexedToUnchecked(target, f, sourceRowIndex, targetRowIndex, rowCount, sourceColumnIndex, targetColumnIndex, columnCount, zeros, existingData);
    }

    /// <summary>
    /// Maps an indexed submatrix into another matrix storage without validation.
    /// </summary>
    /// <typeparam name="TU">The target element type.</typeparam>
    /// <param name="target">The target storage.</param>
    /// <param name="f">The indexed mapping function.</param>
    /// <param name="sourceRowIndex">The starting source row index.</param>
    /// <param name="targetRowIndex">The starting target row index.</param>
    /// <param name="rowCount">The number of rows to map.</param>
    /// <param name="sourceColumnIndex">The starting source column index.</param>
    /// <param name="targetColumnIndex">The starting target column index.</param>
    /// <param name="columnCount">The number of columns to map.</param>
    /// <param name="zeros">How zeros are handled during the mapping.</param>
    /// <param name="existingData">How existing target data is handled.</param>
    internal virtual void MapSubMatrixIndexedToUnchecked<TU>(MatrixStorage<TU> target, Func<int, int, T, TU> f,
        int sourceRowIndex, int targetRowIndex, int rowCount,
        int sourceColumnIndex, int targetColumnIndex, int columnCount,
        Zeros zeros, ExistingData existingData)
        where TU : struct, IEquatable<TU>, IFormattable
    {
      for (int j = sourceColumnIndex, jj = targetColumnIndex; j < sourceColumnIndex + columnCount; j++, jj++)
      {
        for (int i = sourceRowIndex, ii = targetRowIndex; i < sourceRowIndex + rowCount; i++, ii++)
        {
          target.At(ii, jj, f(ii, jj, At(i, j)));
        }
      }
    }

    /// <summary>
    /// Maps pairs of values from two matrices into a target matrix.
    /// </summary>
    /// <param name="target">The target storage.</param>
    /// <param name="other">The other matrix storage.</param>
    /// <param name="f">The mapping function.</param>
    /// <param name="zeros">How zeros are handled during the mapping.</param>
    /// <param name="existingData">How existing target data is handled.</param>
    public void Map2To(MatrixStorage<T> target, MatrixStorage<T> other, Func<T, T, T> f, Zeros zeros, ExistingData existingData)
    {
      if (target == null)
      {
        throw new ArgumentNullException(nameof(target));
      }

      if (other == null)
      {
        throw new ArgumentNullException(nameof(other));
      }

      if (RowCount != target.RowCount || ColumnCount != target.ColumnCount)
      {
        var message = $"Matrix dimensions must agree: op1 is {RowCount}x{ColumnCount}, op2 is {target.RowCount}x{target.ColumnCount}.";
        throw new ArgumentException(message, nameof(target));
      }

      if (RowCount != other.RowCount || ColumnCount != other.ColumnCount)
      {
        var message = $"Matrix dimensions must agree: op1 is {RowCount}x{ColumnCount}, op2 is {other.RowCount}x{other.ColumnCount}.";
        throw new ArgumentException(message, nameof(other));
      }

      Map2ToUnchecked(target, other, f, zeros, existingData);
    }

    /// <summary>
    /// Maps pairs of values from two matrices into a target matrix without validation.
    /// </summary>
    /// <param name="target">The target storage.</param>
    /// <param name="other">The other matrix storage.</param>
    /// <param name="f">The mapping function.</param>
    /// <param name="zeros">How zeros are handled during the mapping.</param>
    /// <param name="existingData">How existing target data is handled.</param>
    internal virtual void Map2ToUnchecked(MatrixStorage<T> target, MatrixStorage<T> other, Func<T, T, T> f, Zeros zeros, ExistingData existingData)
    {
      for (int i = 0; i < RowCount; i++)
      {
        for (int j = 0; j < ColumnCount; j++)
        {
          target.At(i, j, f(At(i, j), other.At(i, j)));
        }
      }
    }

    // FUNCTIONAL COMBINATORS: FOLD

    /// <summary>
    /// Folds each row into a target state array.
    /// </summary>
    /// <typeparam name="TU">The accumulator type.</typeparam>
    /// <param name="target">The array receiving the finalized row states.</param>
    /// <param name="f">The accumulation function.</param>
    /// <param name="finalize">The finalization function.</param>
    /// <param name="state">The initial state array.</param>
    /// <param name="zeros">How zeros are handled during the fold.</param>
    /// <remarks>The state array will not be modified, unless it is the same instance as the target array, which is allowed.</remarks>
    public void FoldByRow<TU>(TU[] target, Func<TU, T, TU> f, Func<TU, int, TU> finalize, TU[] state, Zeros zeros)
    {
      if (target == null)
      {
        throw new ArgumentNullException(nameof(target));
      }
      if (target.Length != RowCount)
      {
        throw new ArgumentException("All vectors must have the same dimensionality.", nameof(target));
      }

      if (state == null)
      {
        throw new ArgumentNullException(nameof(state));
      }
      if (state.Length != RowCount)
      {
        throw new ArgumentException("All vectors must have the same dimensionality.", nameof(state));
      }

      FoldByRowUnchecked(target, f, finalize, state, zeros);
    }

    /// <summary>
    /// Folds each row into a target state array without validation.
    /// </summary>
    /// <typeparam name="TU">The accumulator type.</typeparam>
    /// <param name="target">The array receiving the finalized row states.</param>
    /// <param name="f">The accumulation function.</param>
    /// <param name="finalize">The finalization function.</param>
    /// <param name="state">The initial state array.</param>
    /// <param name="zeros">How zeros are handled during the fold.</param>
    /// <remarks>The state array will not be modified, unless it is the same instance as the target array (which is allowed).</remarks>
    /// <summary>
    /// Folds each row into a target state array without validation.
    /// </summary>
    internal virtual void FoldByRowUnchecked<TU>(TU[] target, Func<TU, T, TU> f, Func<TU, int, TU> finalize, TU[] state, Zeros zeros)
    {
      for (int i = 0; i < RowCount; i++)
      {
        TU s = state[i];
        for (int j = 0; j < ColumnCount; j++)
        {
          s = f(s, At(i, j));
        }
        target[i] = finalize(s, ColumnCount);
      }
    }

    /// <summary>
    /// Folds each column into a target state array.
    /// </summary>
    /// <typeparam name="TU">The accumulator type.</typeparam>
    /// <param name="target">The array receiving the finalized column states.</param>
    /// <param name="f">The accumulation function.</param>
    /// <param name="finalize">The finalization function.</param>
    /// <param name="state">The initial state array.</param>
    /// <param name="zeros">How zeros are handled during the fold.</param>
    /// <remarks>The state array will not be modified, unless it is the same instance as the target array, which is allowed.</remarks>
    public void FoldByColumn<TU>(TU[] target, Func<TU, T, TU> f, Func<TU, int, TU> finalize, TU[] state, Zeros zeros)
    {
      if (target == null)
      {
        throw new ArgumentNullException(nameof(target));
      }
      if (target.Length != ColumnCount)
      {
        throw new ArgumentException("All vectors must have the same dimensionality.", nameof(target));
      }

      if (state == null)
      {
        throw new ArgumentNullException(nameof(state));
      }
      if (state.Length != ColumnCount)
      {
        throw new ArgumentException("All vectors must have the same dimensionality.", nameof(state));
      }

      FoldByColumnUnchecked(target, f, finalize, state, zeros);
    }

    /// <summary>
    /// Folds each column into a target state array without validation.
    /// </summary>
    /// <typeparam name="TU">The accumulator type.</typeparam>
    /// <param name="target">The array receiving the finalized column states.</param>
    /// <param name="f">The accumulation function.</param>
    /// <param name="finalize">The finalization function.</param>
    /// <param name="state">The initial state array.</param>
    /// <param name="zeros">How zeros are handled during the fold.</param>
    /// <remarks>The state array will not be modified, unless it is the same instance as the target array (which is allowed).</remarks>
    /// <summary>
    /// Folds each column into a target state array without validation.
    /// </summary>
    internal virtual void FoldByColumnUnchecked<TU>(TU[] target, Func<TU, T, TU> f, Func<TU, int, TU> finalize, TU[] state, Zeros zeros)
    {
      for (int j = 0; j < ColumnCount; j++)
      {
        TU s = state[j];
        for (int i = 0; i < RowCount; i++)
        {
          s = f(s, At(i, j));
        }
        target[j] = finalize(s, RowCount);
      }
    }

    /// <summary>
    /// Folds values from this matrix and another matrix into a single state value.
    /// </summary>
    /// <typeparam name="TOther">The element type of the other matrix.</typeparam>
    /// <typeparam name="TState">The accumulator type.</typeparam>
    /// <param name="other">The other matrix storage.</param>
    /// <param name="f">The accumulation function.</param>
    /// <param name="state">The initial state.</param>
    /// <param name="zeros">How zeros are handled during the fold.</param>
    /// <returns>The final accumulated state.</returns>
    public TState Fold2<TOther, TState>(MatrixStorage<TOther> other, Func<TState, T, TOther, TState> f, TState state, Zeros zeros)
        where TOther : struct, IEquatable<TOther>, IFormattable
    {
      if (other == null)
      {
        throw new ArgumentNullException(nameof(other));
      }

      if (RowCount != other.RowCount || ColumnCount != other.ColumnCount)
      {
        var message = $"Matrix dimensions must agree: op1 is {RowCount}x{ColumnCount}, op2 is {other.RowCount}x{other.ColumnCount}.";
        throw new ArgumentException(message, nameof(other));
      }

      return Fold2Unchecked(other, f, state, zeros);
    }

    /// <summary>
    /// Folds values from this matrix and another matrix into a single state value without validation.
    /// </summary>
    /// <typeparam name="TOther">The element type of the other matrix.</typeparam>
    /// <typeparam name="TState">The accumulator state type.</typeparam>
    /// <param name="other">The other matrix storage.</param>
    /// <param name="f">The folding function.</param>
    /// <param name="state">The initial state.</param>
    /// <param name="zeros">Controls whether zero values may be skipped.</param>
    /// <returns>The final accumulated state.</returns>
    internal virtual TState Fold2Unchecked<TOther, TState>(MatrixStorage<TOther> other, Func<TState, T, TOther, TState> f, TState state, Zeros zeros)
        where TOther : struct, IEquatable<TOther>, IFormattable
    {
      for (int i = 0; i < RowCount; i++)
      {
        for (int j = 0; j < ColumnCount; j++)
        {
          state = f(state, At(i, j), other.At(i, j));
        }
      }

      return state;
    }
  }
}
