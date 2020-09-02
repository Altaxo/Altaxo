#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Calc.LinearAlgebra
{
  /// <summary>
  /// Very thin wrapper structure around a jagged array just to provided number of rows and columns along with the array itself.
  /// The spine array is oriented vertically, thus access to the array is down by array[row][column].
  /// </summary>
  /// <typeparam name="T">Type of scaler value.</typeparam>
  public struct MatrixWrapperStructForLeftSpineJaggedArray<T> : IMatrix<T> where T : struct
  {
    /// <summary>
    /// Gets the underlying array. Access to elements is done using Array[row][column].
    /// </summary>
    /// <value>
    /// The underlying array.
    /// </value>
    public T[][] Array { get; private set; }

    /// <summary>
    /// Gets the number of rows of the matrix.
    /// </summary>
    /// <value>
    /// Number of rows of the matrix.
    /// </value>
    public int RowCount { get; private set; }

    /// <summary>
    /// Gets the number of columns of the matrix.
    /// </summary>
    /// <value>
    /// Number of columns of the matrix.
    /// </value>
    public int ColumnCount { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MatrixWrapperStructForLeftSpineJaggedArray{T}"/> struct by wrapping the provided
    /// array.
    /// </summary>
    /// <param name="array">The array to wrap.</param>
    /// <param name="rows">Number of rows.</param>
    /// <param name="cols">Number of columns.</param>
    public MatrixWrapperStructForLeftSpineJaggedArray(T[][] array, int rows, int cols)
    {
      if (array is null)
        throw new ArgumentNullException(nameof(array));
      if (rows < 0 || rows > array.Length)
        throw new ArgumentOutOfRangeException(nameof(rows), "Provided array is shorter than number of rows");
      if (cols < 0 || (rows > 0 && cols > array[0].Length))
        throw new ArgumentOutOfRangeException(nameof(cols), "Provided array is shorter than number of cols");

      Array = array;
      RowCount = rows;
      ColumnCount = cols;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MatrixWrapperStructForLeftSpineJaggedArray{T}"/> struct and creates
    /// the jagged matrix array.
    /// </summary>
    /// <param name="rows">The number of rows.</param>
    /// <param name="cols">The number of columns.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// rows - Number of rows has to be >=0
    /// or
    /// cols - Number of cols has to be >=0
    /// </exception>
    public MatrixWrapperStructForLeftSpineJaggedArray(int rows, int cols)
    {
      if (!(rows >= 0))
        throw new ArgumentOutOfRangeException(nameof(rows), "Number of rows has to be >=0");
      if (!(cols >= 0))
        throw new ArgumentOutOfRangeException(nameof(cols), "Number of cols has to be >=0");
      RowCount = rows;
      ColumnCount = cols;
      Array = new T[RowCount][];
      for (int i = 0; i < RowCount; ++i)
      {
        Array[i] = new T[ColumnCount];
      }
    }

    T IROMatrix<T>.this[int row, int col]
    {
      get
      {
        return Array[row][col];
      }
    }

    public T this[int row, int col]
    {
      get
      {
        return Array[row][col];
      }
      set
      {
        Array[row][col] = value;
      }
    }
  }

  /// <summary>
  /// Very thin wrapper structure around a jagged array just to provided number of rows and columns along with the array itself.
  /// The spine array is oriented vertically, i.e. the rows protruding to the right from the spine array.
  /// Access to the underlying array is done by array[row][column].
  /// </summary>
  /// <typeparam name="T">Type of scaler value.</typeparam>
  public struct MatrixWrapperStructForTopSpineJaggedArray<T> : IMatrix<T> where T : struct
  {
    /// <summary>
    /// Gets the underlying array. Access to elements is done using Array[column][row], i.e. with exchanged column and row.
    /// </summary>
    /// <value>
    /// The underlying array.
    /// </value>
    public T[][] Array { get; private set; }

    /// <summary>
    /// Gets the number of rows of the matrix.
    /// </summary>
    /// <value>
    /// Number of rows of the matrix.
    /// </value>
    public int RowCount { get; private set; }

    /// <summary>
    /// Gets the number of columns of the matrix.
    /// </summary>
    /// <value>
    /// Number of columns of the matrix.
    /// </value>
    public int ColumnCount { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MatrixWrapperStructForLeftSpineJaggedArray{T}"/> struct by wrapping the provided
    /// array.
    /// </summary>
    /// <param name="array">The array to wrap.</param>
    /// <param name="rows">Number of rows.</param>
    /// <param name="cols">Number of columns.</param>
    public MatrixWrapperStructForTopSpineJaggedArray(T[][] array, int rows, int cols)
    {
      if (array is null)
        throw new ArgumentNullException(nameof(array));
      if (cols < 0 || cols > array.Length)
        throw new ArgumentOutOfRangeException(nameof(rows), "Provided array is shorter than number of columns");
      if (rows < 0 || (cols > 0 && rows > array[0].Length))
        throw new ArgumentOutOfRangeException(nameof(cols), "Provided array is shorter than number of rows");

      Array = array;
      RowCount = rows;
      ColumnCount = cols;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MatrixWrapperStructForLeftSpineJaggedArray{T}"/> struct and creates
    /// the jagged matrix array. This array is a jagged array structure where the spine array is located at the top of the matrix,
    /// and the columns 'are hanging down' from this spine array. Access to elements of the underlying array is done by array[col][row], i.e. with col and row exchanged.
    /// </summary>
    /// <param name="rows">The number of rows.</param>
    /// <param name="cols">The number of columns.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// rows - Number of rows has to be >=0
    /// or
    /// cols - Number of cols has to be >=0
    /// </exception>
    public MatrixWrapperStructForTopSpineJaggedArray(int rows, int cols)
    {
      if (!(rows >= 0))
        throw new ArgumentOutOfRangeException(nameof(rows), "Number of rows has to be >=0");
      if (!(cols >= 0))
        throw new ArgumentOutOfRangeException(nameof(cols), "Number of cols has to be >=0");
      RowCount = rows;
      ColumnCount = cols;

      Array = new T[cols][];
      for (int i = 0; i < cols; ++i)
      {
        Array[i] = new T[rows];
      }
    }

    T IROMatrix<T>.this[int row, int col]
    {
      get
      {
        return Array[col][row];
      }
    }

    public T this[int row, int col]
    {
      get
      {
        return Array[col][row];
      }
      set
      {
        Array[col][row] = value;
      }
    }
  }

  /// <summary>
  /// Very thin wrapper structure that wraps a column major order linear array, i.e. consecutive elements of the linear array belong most probably to the same column, to provide information on number of rows and columns.
  /// Attention: this is <b>not</b> LAPACK convention (!)).
  /// </summary>
  /// <typeparam name="T">Element type.</typeparam>
  public struct MatrixWrapperStructForColumnMajorOrderLinearArray<T> : IMatrix<T> where T : struct
  {
    /// <summary>
    /// Gets the underlying array. Access to elements is done using Array[column * Rows + row].
    /// </summary>
    /// <value>
    /// The underlying array.
    /// </value>
    public T[] Array { get; private set; }

    /// <summary>
    /// Gets the number of rows of the matrix.
    /// </summary>
    /// <value>
    /// Number of rows of the matrix.
    /// </value>
    public int RowCount { get; private set; }

    /// <summary>
    /// Gets the number of columns of the matrix.
    /// </summary>
    /// <value>
    /// Number of columns of the matrix.
    /// </value>
    public int ColumnCount { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MatrixWrapperStructForLeftSpineJaggedArray{T}"/> struct by wrapping the provided
    /// array.
    /// </summary>
    /// <param name="array">The array to wrap.</param>
    /// <param name="rows">Number of rows.</param>
    /// <param name="cols">Number of columns.</param>
    public MatrixWrapperStructForColumnMajorOrderLinearArray(T[] array, int rows, int cols)
    {
      if (array is null)
        throw new ArgumentNullException(nameof(array));
      if (rows < 0)
        throw new ArgumentOutOfRangeException(nameof(rows), "Provided array is shorter than number of rows");
      if (cols < 0 || (rows * cols > array.Length))
        throw new ArgumentOutOfRangeException(nameof(cols), "Provided array is shorter than number of cols");

      Array = array;
      RowCount = rows;
      ColumnCount = cols;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MatrixWrapperStructForLeftSpineJaggedArray{T}"/> struct and creates
    /// the jagged matrix array.
    /// </summary>
    /// <param name="rows">The number of rows.</param>
    /// <param name="cols">The number of columns.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// rows - Number of rows has to be >=0
    /// or
    /// cols - Number of cols has to be >=0
    /// </exception>
    public MatrixWrapperStructForColumnMajorOrderLinearArray(int rows, int cols)
    {
      if (!(rows >= 0))
        throw new ArgumentOutOfRangeException(nameof(rows), "Number of rows has to be >=0");
      if (!(cols >= 0))
        throw new ArgumentOutOfRangeException(nameof(cols), "Number of cols has to be >=0");
      RowCount = rows;
      ColumnCount = cols;
      Array = new T[RowCount * ColumnCount];
    }

    T IROMatrix<T>.this[int row, int col]
    {
      get
      {
        return Array[col * RowCount + row];
      }
    }

    public T this[int row, int col]
    {
      get
      {
        return Array[col * RowCount + row];
      }
      set
      {
        Array[col * RowCount + row] = value;
      }
    }
  }

  /// <summary>
  /// Thin wrapper structure that wraps a row major order linear array, i.e. consecutive elements of the linear array belong most probably to the same row,  to provide information on number of rows and columns.
  /// Attention: this is <b>not</b> LAPACK convention (!)). If using LAPACK, you need column major order (<see cref="MatrixWrapperStructForColumnMajorOrderLinearArray{T}"/>).
  /// </summary>
  /// <typeparam name="T">Element type.</typeparam>
  public struct MatrixWrapperStructForRowMajorOrderLinearArray<T> : IMatrix<T> where T : struct
  {
    /// <summary>
    /// Gets the underlying array. Access to elements is done using Array[row * Columns + column].
    /// </summary>
    /// <value>
    /// The underlying array.
    /// </value>
    public T[] Array { get; private set; }

    /// <summary>
    /// Gets the number of rows of the matrix.
    /// </summary>
    /// <value>
    /// Number of rows of the matrix.
    /// </value>
    public int RowCount { get; private set; }

    /// <summary>
    /// Gets the number of columns of the matrix.
    /// </summary>
    /// <value>
    /// Number of columns of the matrix.
    /// </value>
    public int ColumnCount { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MatrixWrapperStructForLeftSpineJaggedArray{T}"/> struct by wrapping the provided row oriented
    /// array.
    /// </summary>
    /// <param name="array">The array to wrap.</param>
    /// <param name="rows">Number of rows.</param>
    /// <param name="cols">Number of columns.</param>
    public MatrixWrapperStructForRowMajorOrderLinearArray(T[] array, int rows, int cols)
    {
      if (array is null)
        throw new ArgumentNullException(nameof(array));
      if (rows < 0)
        throw new ArgumentOutOfRangeException(nameof(rows), "Provided array is shorter than number of rows");
      if (cols < 0 || (rows * cols > array.Length))
        throw new ArgumentOutOfRangeException(nameof(cols), "Provided array is shorter than number of cols");

      Array = array;
      RowCount = rows;
      ColumnCount = cols;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MatrixWrapperStructForLeftSpineJaggedArray{T}"/> struct and creates
    /// the underlying row oriented array.
    /// </summary>
    /// <param name="rows">The number of rows.</param>
    /// <param name="cols">The number of columns.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// rows - Number of rows has to be >=0
    /// or
    /// cols - Number of cols has to be >=0
    /// </exception>
    public MatrixWrapperStructForRowMajorOrderLinearArray(int rows, int cols)
    {
      if (!(rows >= 0))
        throw new ArgumentOutOfRangeException(nameof(rows), "Number of rows has to be >=0");
      if (!(cols >= 0))
        throw new ArgumentOutOfRangeException(nameof(cols), "Number of cols has to be >=0");
      RowCount = rows;
      ColumnCount = cols;
      Array = new T[RowCount * ColumnCount];
    }

    T IROMatrix<T>.this[int row, int col]
    {
      get
      {
        return Array[row * ColumnCount + col];
      }
    }

    public T this[int row, int col]
    {
      get
      {
        return Array[row * ColumnCount + col];
      }
      set
      {
        Array[row * ColumnCount + col] = value;
      }
    }
  }
}
