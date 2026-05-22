// <copyright file="BandMatrixStorage.cs" company="Math.NET">
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
using System.Linq;
using System.Runtime.Serialization;

namespace Altaxo.Calc.LinearAlgebra.Storage
{
  /// <summary>
  /// Stores a band matrix in a linear array using the compact band layout.
  /// </summary>
  /// <typeparam name="T">The type of the stored values.</typeparam>
  [Serializable]
  [DataContract(Namespace = "urn:MathNet/Numerics/LinearAlgebra")]
  public class BandMatrixStorage<T> : MatrixStorage<T>
        where T : struct, IEquatable<T>, IFormattable
  {
    /// <summary>
    /// Gets the number of sub-diagonals stored below the main diagonal.
    /// </summary>
    [DataMember(Order = 3)]
    public readonly int LowerBandwidth;

    /// <summary>
    /// Gets the number of super-diagonals stored above the main diagonal.
    /// </summary>
    [DataMember(Order = 4)]
    public readonly int UpperBandwidth;

    /// <summary>
    /// Gets the compact band data.
    /// </summary>
    /// <remarks>
    /// The data is stored as a linearized band matrix with <c>LowerBandwidth + UpperBandwidth + 1</c>
    /// rows and <see cref="MatrixStorage{T}.ColumnCount"/> columns, using the index mapping
    /// <c>ColumnCount * (UpperBandwidth + row - column) + column</c> for an in-band element.
    /// </remarks>
    [DataMember(Order = 5)]
    public readonly T[] Data;

    private int BandRowCount => LowerBandwidth + UpperBandwidth + 1;

    /// <summary>
    /// Initializes a new instance of the <see cref="BandMatrixStorage{T}"/> class.
    /// </summary>
    /// <param name="rows">The number of rows.</param>
    /// <param name="columns">The number of columns.</param>
    /// <param name="lowerBandwidth">The number of sub-diagonals stored below the main diagonal.</param>
    /// <param name="upperBandwidth">The number of super-diagonals stored above the main diagonal.</param>
    internal BandMatrixStorage(int rows, int columns, int lowerBandwidth, int upperBandwidth)
      : base(rows, columns)
    {
      if (lowerBandwidth < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(lowerBandwidth), "The lower bandwidth must be non-negative.");
      }

      if (upperBandwidth < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(upperBandwidth), "The upper bandwidth must be non-negative.");
      }

      LowerBandwidth = lowerBandwidth;
      UpperBandwidth = upperBandwidth;
      Data = new T[(lowerBandwidth + upperBandwidth + 1) * columns];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BandMatrixStorage{T}"/> class with existing data.
    /// </summary>
    /// <param name="rows">The number of rows.</param>
    /// <param name="columns">The number of columns.</param>
    /// <param name="lowerBandwidth">The number of sub-diagonals stored below the main diagonal.</param>
    /// <param name="upperBandwidth">The number of super-diagonals stored above the main diagonal.</param>
    /// <param name="data">The compact band data.</param>
    internal BandMatrixStorage(int rows, int columns, int lowerBandwidth, int upperBandwidth, T[] data)
      : base(rows, columns)
    {
      if (lowerBandwidth < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(lowerBandwidth), "The lower bandwidth must be non-negative.");
      }

      if (upperBandwidth < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(upperBandwidth), "The upper bandwidth must be non-negative.");
      }

      if (data is null)
      {
        throw new ArgumentNullException(nameof(data));
      }

      var expectedLength = (lowerBandwidth + upperBandwidth + 1) * columns;
      if (data.Length != expectedLength)
      {
        throw new ArgumentOutOfRangeException(nameof(data), $"The given array has the wrong length. Should be {expectedLength}.");
      }

      LowerBandwidth = lowerBandwidth;
      UpperBandwidth = upperBandwidth;
      Data = data;
    }

    /// <inheritdoc/>
    public override bool IsDense => false;

    /// <inheritdoc/>
    public override bool IsFullyMutable => false;

    /// <inheritdoc/>
    public override bool IsMutableAt(int row, int column)
    {
      return IsWithinStoredBand(row, column);
    }

    /// <inheritdoc/>
    public override T At(int row, int column)
    {
      return IsWithinStoredBand(row, column) ? Data[DataIndex(row, column)] : Zero;
    }

    /// <inheritdoc/>
    public override void At(int row, int column, T value)
    {
      if (IsWithinStoredBand(row, column))
      {
        Data[DataIndex(row, column)] = value;
      }
      else if (!Zero.Equals(value))
      {
        throw new IndexOutOfRangeException("Cannot set an element outside the band of a band matrix.");
      }
    }

    /// <inheritdoc/>
    public override void Clear()
    {
      Array.Clear(Data, 0, Data.Length);
    }

    /// <summary>
    /// Creates a band matrix storage from another matrix storage.
    /// </summary>
    /// <param name="matrix">The source matrix storage.</param>
    /// <param name="lowerBandwidth">The lower bandwidth of the target storage.</param>
    /// <param name="upperBandwidth">The upper bandwidth of the target storage.</param>
    /// <returns>The initialized band matrix storage.</returns>
    public static BandMatrixStorage<T> OfMatrix(MatrixStorage<T> matrix, int lowerBandwidth, int upperBandwidth)
    {
      if (matrix is null)
      {
        throw new ArgumentNullException(nameof(matrix));
      }

      var storage = new BandMatrixStorage<T>(matrix.RowCount, matrix.ColumnCount, lowerBandwidth, upperBandwidth);
      matrix.CopyToUnchecked(storage, ExistingData.AssumeZeros);
      return storage;
    }

    /// <summary>
    /// Creates a band matrix storage from a two-dimensional array.
    /// </summary>
    /// <param name="array">The source array.</param>
    /// <param name="lowerBandwidth">The lower bandwidth of the target storage.</param>
    /// <param name="upperBandwidth">The upper bandwidth of the target storage.</param>
    /// <returns>The initialized band matrix storage.</returns>
    public static BandMatrixStorage<T> OfArray(T[,] array, int lowerBandwidth, int upperBandwidth)
    {
      if (array is null)
      {
        throw new ArgumentNullException(nameof(array));
      }

      var storage = new BandMatrixStorage<T>(array.GetLength(0), array.GetLength(1), lowerBandwidth, upperBandwidth);
      for (var i = 0; i < storage.RowCount; i++)
      {
        for (var j = 0; j < storage.ColumnCount; j++)
        {
          storage.At(i, j, array[i, j]);
        }
      }

      return storage;
    }

    /// <summary>
    /// Creates a band matrix storage initialized with a constant value on all stored entries.
    /// </summary>
    /// <param name="rows">The number of rows.</param>
    /// <param name="columns">The number of columns.</param>
    /// <param name="lowerBandwidth">The lower bandwidth of the target storage.</param>
    /// <param name="upperBandwidth">The upper bandwidth of the target storage.</param>
    /// <param name="value">The value assigned to each stored entry.</param>
    /// <returns>The initialized band matrix storage.</returns>
    public static BandMatrixStorage<T> OfValue(int rows, int columns, int lowerBandwidth, int upperBandwidth, T value)
    {
      var storage = new BandMatrixStorage<T>(rows, columns, lowerBandwidth, upperBandwidth);
      for (var j = 0; j < columns; j++)
      {
        var rowStart = Math.Max(0, j - upperBandwidth);
        var rowEnd = Math.Min(rows - 1, j + lowerBandwidth);
        for (var i = rowStart; i <= rowEnd; i++)
        {
          storage.Data[storage.DataIndex(i, j)] = value;
        }
      }

      return storage;
    }

    /// <summary>
    /// Creates a band matrix storage initialized by an index-based initializer.
    /// </summary>
    /// <param name="rows">The number of rows.</param>
    /// <param name="columns">The number of columns.</param>
    /// <param name="lowerBandwidth">The lower bandwidth of the target storage.</param>
    /// <param name="upperBandwidth">The upper bandwidth of the target storage.</param>
    /// <param name="init">The initializer function for in-band entries.</param>
    /// <returns>The initialized band matrix storage.</returns>
    public static BandMatrixStorage<T> OfInit(int rows, int columns, int lowerBandwidth, int upperBandwidth, Func<int, int, T> init)
    {
      if (init is null)
      {
        throw new ArgumentNullException(nameof(init));
      }

      var storage = new BandMatrixStorage<T>(rows, columns, lowerBandwidth, upperBandwidth);
      for (var j = 0; j < columns; j++)
      {
        var rowStart = Math.Max(0, j - upperBandwidth);
        var rowEnd = Math.Min(rows - 1, j + lowerBandwidth);
        for (var i = rowStart; i <= rowEnd; i++)
        {
          storage.Data[storage.DataIndex(i, j)] = init(i, j);
        }
      }

      return storage;
    }

    /// <summary>
    /// Creates a band matrix storage from indexed values.
    /// </summary>
    /// <param name="rows">The number of rows.</param>
    /// <param name="columns">The number of columns.</param>
    /// <param name="lowerBandwidth">The lower bandwidth of the target storage.</param>
    /// <param name="upperBandwidth">The upper bandwidth of the target storage.</param>
    /// <param name="data">The indexed values.</param>
    /// <returns>The initialized band matrix storage.</returns>
    public static BandMatrixStorage<T> OfIndexedEnumerable(int rows, int columns, int lowerBandwidth, int upperBandwidth, IEnumerable<(int row, int column, T value)> data)
    {
      if (data is null)
      {
        throw new ArgumentNullException(nameof(data));
      }

      var storage = new BandMatrixStorage<T>(rows, columns, lowerBandwidth, upperBandwidth);
      foreach (var (row, column, value) in data)
      {
        storage.At(row, column, value);
      }

      return storage;
    }

    /// <summary>
    /// Creates a band matrix storage from indexed tuple values.
    /// </summary>
    /// <param name="rows">The number of rows.</param>
    /// <param name="columns">The number of columns.</param>
    /// <param name="lowerBandwidth">The lower bandwidth of the target storage.</param>
    /// <param name="upperBandwidth">The upper bandwidth of the target storage.</param>
    /// <param name="data">The indexed values.</param>
    /// <returns>The initialized band matrix storage.</returns>
    public static BandMatrixStorage<T> OfIndexedEnumerable(int rows, int columns, int lowerBandwidth, int upperBandwidth, IEnumerable<Tuple<int, int, T>> data)
    {
      if (data is null)
      {
        throw new ArgumentNullException(nameof(data));
      }

      return OfIndexedEnumerable(rows, columns, lowerBandwidth, upperBandwidth, data.Select(x => (x.Item1, x.Item2, x.Item3)));
    }

    /// <inheritdoc/>
    internal override void CopyToUnchecked(MatrixStorage<T> target, ExistingData existingData)
    {
      if (target is BandMatrixStorage<T> bandTarget && LowerBandwidth == bandTarget.LowerBandwidth && UpperBandwidth == bandTarget.UpperBandwidth)
      {
        Array.Copy(Data, 0, bandTarget.Data, 0, Data.Length);
        return;
      }

      if (target is DenseColumnMajorMatrixStorage<T> denseTarget)
      {
        CopyToUnchecked(denseTarget, existingData);
        return;
      }

      if (existingData == ExistingData.Clear)
      {
        target.Clear();
      }

      for (int j = 0; j < ColumnCount; j++)
      {
        int rowStart = Math.Max(0, j - UpperBandwidth);
        int rowEnd = Math.Min(RowCount - 1, j + LowerBandwidth);
        for (int i = rowStart; i <= rowEnd; i++)
        {
          target.At(i, j, Data[DataIndex(i, j)]);
        }
      }
    }

    /// <summary>
    /// Copies the stored band entries into a dense column-major target storage.
    /// </summary>
    /// <param name="target">The dense storage receiving the copied entries.</param>
    /// <param name="existingData">Specifies whether existing target data should be cleared first.</param>
    private void CopyToUnchecked(DenseColumnMajorMatrixStorage<T> target, ExistingData existingData)
    {
      if (existingData == ExistingData.Clear)
      {
        target.Clear();
      }

      var targetData = target.Data;
      for (int j = 0; j < ColumnCount; j++)
      {
        int rowStart = Math.Max(0, j - UpperBandwidth);
        int rowEnd = Math.Min(RowCount - 1, j + LowerBandwidth);
        int targetIndex = j * target.RowCount + rowStart;
        for (int i = rowStart; i <= rowEnd; i++, targetIndex++)
        {
          targetData[targetIndex] = Data[DataIndex(i, j)];
        }
      }
    }

    /// <summary>
    /// Determines whether the specified matrix position lies within the stored band.
    /// </summary>
    /// <param name="row">The zero-based row index.</param>
    /// <param name="column">The zero-based column index.</param>
    /// <returns><see langword="true"/> if the position is stored explicitly; otherwise, <see langword="false"/>.</returns>
    private bool IsWithinStoredBand(int row, int column)
    {
      return row - column <= LowerBandwidth && column - row <= UpperBandwidth;
    }

    /// <summary>
    /// Computes the index in the compact data array for an element known to lie within the stored band.
    /// </summary>
    /// <param name="row">The zero-based row index.</param>
    /// <param name="column">The zero-based column index.</param>
    /// <returns>The linear index into <see cref="Data"/> for the specified in-band element.</returns>
    private int DataIndex(int row, int column)
    {
      return ColumnCount * (UpperBandwidth + row - column) + column;
    }
  }
}
