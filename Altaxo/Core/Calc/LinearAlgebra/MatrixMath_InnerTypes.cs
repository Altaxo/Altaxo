#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Altaxo.Calc.LinearAlgebra
{
  /// <summary>
  /// Class MatrixMath provides common static methods for matrix manipulation
  /// and arithmetic in tow dimensions.
  /// </summary>
  public static partial class MatrixMath
  {
    #region Scalar

    /// <summary>
    /// Implements a scalar as a special case of the matrix which has the dimensions (1,1).
    /// </summary>
    public class ScalarAsMatrix<T> : IMatrix<T>, IVector<T> where T : struct
    {
      /// <summary>
      /// Holds the only element of the matrix.
      /// </summary>
      private T _value;

      /// <summary>
      /// Creates the scalar and initializes it with the value val.
      /// </summary>
      /// <param name="val"></param>
      public ScalarAsMatrix(T val)
      {
        _value = val;
      }

      /// <inheritdoc/>
      public override string ToString()
      {
        return string.Format("Scalar: {0}", _value);
      }

      /// <inheritdoc/>
      public IEnumerator<T> GetEnumerator()
      {
        yield return _value;
      }

      /// <inheritdoc/>
      IEnumerator IEnumerable.GetEnumerator()
      {
        yield return _value;
      }

      /// <summary>
      /// Converts the scalar to a double if neccessary.
      /// </summary>
      /// <param name="s">The scalar to convert.</param>
      /// <returns>The value of the element[0,0], which is the only element of the scalar.</returns>
      public static implicit operator T(ScalarAsMatrix<T> s)
      {
        return s._value;
      }

      /// <summary>
      /// Converts a double to a scalar where neccessary.
      /// </summary>
      /// <param name="d">The double value to convert.</param>
      /// <returns>The scalar representation of this double value.</returns>
      public static implicit operator ScalarAsMatrix<T>(T d)
      {
        return new ScalarAsMatrix<T>(d);
      }

      #region IMatrix Members

      /// <summary>
      /// Element accessor. Both col and row should be zero, but this is not justified here. Always returns the value of the scalar.
      /// </summary>
      public T this[int i, int k]
      {
        get
        {
          return _value;
        }
        set
        {
          _value = value;
        }
      }

      /// <summary>
      /// Number of rows of the matrix. Always 1 (one).
      /// </summary>
      public int RowCount
      {
        get
        {
          return 1;
        }
      }

      /// <summary>
      /// Number of columns of the matrix. Always 1 (one).
      /// </summary>
      public int ColumnCount
      {
        get
        {
          return 1;
        }
      }

      #endregion IMatrix Members

      #region IVector Members

      public T this[int i]
      {
        get
        {
          return _value;
        }
        set
        {
          _value = value;
        }
      }

      #endregion IVector Members

      #region IROVector Members

      public int Length
      {
        get
        {
          return 1;
        }
      }

      public int Count
      {
        get
        {
          return 1;
        }
      }

      #endregion IROVector Members
    }

    #endregion Scalar

    #region MatrixWithOneRow

    /// <summary>
    /// Implements a horizontal vector, i.e. a matrix which has only one row, but many columns.
    /// </summary>
    public class MatrixWithOneRow<T> : IMatrix<T>, IVector<T> where T : struct
    {
      /// <summary>
      /// Holds the elements of the vector
      /// </summary>
      private T[] _array;

      /// <summary>
      /// Creates a Horizontal vector of length cols.
      /// </summary>
      /// <param name="cols">Initial length of the vector.</param>
      public MatrixWithOneRow(int cols)
      {
        _array = new T[cols];
      }

      /// <inheritdoc/>
      public override string ToString()
      {
        return MatrixMath.MatrixToString(null, this);
      }

      /// <inheritdoc/>
      public IEnumerator<T> GetEnumerator()
      {
        for (int i = 0; i < _array.Length; ++i)
          yield return _array[i];
      }

      /// <inheritdoc/>
      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < _array.Length; ++i)
          yield return _array[i];
      }

      #region IMatrix Members

      /// <summary>
      /// Element accessor. The argument rows should be zero, but no exception is thrown if it is not zero.
      /// </summary>
      public T this[int row, int col]
      {
        get
        {
          return _array[col];
        }
        set
        {
          _array[col] = value;
        }
      }

      /// <summary>
      /// Number of rows. Returns always 1 (one).
      /// </summary>
      public int RowCount
      {
        get
        {
          return 1;
        }
      }

      /// <summary>
      /// Number of columns, i.e. number of elements of the horizontal vector.
      /// </summary>
      public int ColumnCount
      {
        get
        {
          return _array.Length;
        }
      }

      #endregion IMatrix Members

      #region IVector Members

      /// <inheritdoc/>
      public T this[int i]
      {
        get
        {
          return _array[i];
        }
        set
        {
          _array[i] = value;
        }
      }

      #endregion IVector Members

      #region IROVector Members

      /// <inheritdoc/>
      public int Length
      {
        get
        {
          return _array.Length;
        }
      }

      /// <inheritdoc/>
      public int Count
      {
        get
        {
          return _array.Length;
        }
      }

      #endregion IROVector Members
    }

    #endregion MatrixWithOneRow

    #region MatrixWithOneColumn

    /// <summary>
    /// Implements a vertical vector, i.e. a matrix which has only one column, but many rows.
    /// </summary>
    public class MatrixWithOneColumn<T> : IMatrix<T>, IVector<T> where T : struct
    {
      /// <summary>
      /// Holds the elements of the vertical vector.
      /// </summary>
      private T[] _array;

      /// <summary>
      /// Creates a vertical vector which has an initial length of rows.
      /// </summary>
      /// <param name="rows">Initial length of the vertical vector.</param>
      public MatrixWithOneColumn(int rows)
      {
        _array = new T[rows];
      }

      /// <inheritdoc/>
      public override string ToString()
      {
        return MatrixMath.MatrixToString(null, this);
      }

      /// <inheritdoc/>
      public IEnumerator<T> GetEnumerator()
      {
        for (int i = 0; i < _array.Length; ++i)
          yield return _array[i];
      }

      /// <inheritdoc/>
      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < _array.Length; ++i)
          yield return _array[i];
      }

      #region IMatrix Members

      /// <summary>
      /// Element accessor. The argument col should be zero here, but no exception is thrown if it is not zero.
      /// </summary>
      public T this[int row, int col]
      {
        get
        {
          return _array[row];
        }
        set
        {
          _array[row] = value;
        }
      }

      /// <summary>
      /// Number of Rows = elements of the vector.
      /// </summary>
      public int RowCount
      {
        get
        {
          return _array.Length;
        }
      }

      /// <summary>
      /// Number of columns of the matrix, always 1 (one) since it is a vertical vector.
      /// </summary>
      public int ColumnCount
      {
        get
        {
          return 1;
        }
      }

      #endregion IMatrix Members

      #region IVector Members

      /// <inheritdoc/>
      public T this[int i]
      {
        get
        {
          return _array[i];
        }
        set
        {
          _array[i] = value;
        }
      }

      #endregion IVector Members

      #region IROVector Members

      /// <inheritdoc/>
      public int Length
      {
        get
        {
          return _array.Length;
        }
      }

      /// <inheritdoc/>
      public int Count
      {
        get
        {
          return _array.Length;
        }
      }

      #endregion IROVector Members
    }

    #endregion MatrixWithOneColumn

    #region MatrixRowROVector

    /// <summary>
    /// Wrapper for a row of an existing matrix to a read-only vector.
    /// </summary>
    public class MatrixRowROVector<T> : IROVector<T> where T : struct
    {
      private IROMatrix<T> _matrix;
      private int _row;
      private int _columnOffset;
      private int _length;

      /// <summary>
      /// Constructor of a matrix row vector by providing the matrix and the row number of that matrix that is wrapped.
      /// </summary>
      /// <param name="matrix">The matrix.</param>
      /// <param name="row">The row number of the matrix that is wrapped to a vector.</param>
      public MatrixRowROVector(IROMatrix<T> matrix, int row)
        : this(matrix, row, 0, matrix.ColumnCount)
      {
      }

      /// <summary>
      /// Constructor of a matrix row vector by providing the matrix and the row number of that matrix that is wrapped.
      /// </summary>
      /// <param name="matrix">The matrix.</param>
      /// <param name="row">The row number of the matrix that is wrapped to a vector.</param>
      /// <param name="columnOffset">First number of column that is included in the vector.</param>
      /// <param name="length">Length of the vector.</param>
      public MatrixRowROVector(IROMatrix<T> matrix, int row, int columnOffset, int length)
      {
        if (matrix is null)
          throw new ArgumentNullException("IROMatrix m is null");
        if (row < 0 || row >= matrix.RowCount)
          throw new ArgumentOutOfRangeException("The parameter row is either <0 or greater than the rows of the matrix");
        if (columnOffset + length > matrix.ColumnCount)
          throw new ArgumentException("Columnoffset+length exceed the number of columns of the matrix");

        _matrix = matrix;
        _row = row;
        _columnOffset = columnOffset;
        _length = length;
      }

      /// <inheritdoc/>
      public T this[int index]
      {
        get
        {
          return _matrix[_row, index + _columnOffset];
        }
      }

      #region IROVector Members

      /// <summary>The number of elements of this vector.</summary>
      public int Length
      {
        get
        {
          return _length;
        }
      }

      /// <inheritdoc/>
      public int Count
      {
        get
        {
          return _length;
        }
      }

      /// <inheritdoc/>
      public IEnumerator<T> GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }

      /// <inheritdoc/>
      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }

      #endregion IROVector Members
    }

    /// <summary>
    /// Wrapper for a matrix row to a vector.
    /// </summary>
    public class MatrixRowVector<T> : IVector<T> where T : struct
    {
      private IMatrix<T> _matrix;
      private int _row;
      private int _columnOffset;
      private int _length;

      /// <summary>
      /// Constructor of a matrix row vector by providing the matrix and the row number of that matrix that is wrapped.
      /// </summary>
      /// <param name="matrix">The matrix.</param>
      /// <param name="row">The row number of the matrix that is wrapped to a vector.</param>
      public MatrixRowVector(IMatrix<T> matrix, int row)
        : this(matrix, row, 0, matrix.ColumnCount)
      {
      }

      /// <summary>
      /// Constructor of a matrix row vector by providing the matrix and the row number of that matrix that is wrapped.
      /// </summary>
      /// <param name="matrix">The matrix.</param>
      /// <param name="row">The row number of the matrix that is wrapped to a vector.</param>
      /// <param name="columnOffset">First number of column that is included in the vector.</param>
      /// <param name="length">Length of the vector.</param>
      public MatrixRowVector(IMatrix<T> matrix, int row, int columnOffset, int length)
      {
        if (matrix is null)
          throw new ArgumentNullException("IMatrix m is null");
        if (row < 0 || row >= matrix.RowCount)
          throw new ArgumentOutOfRangeException("The parameter row is either <0 or greater than the rows of the matrix");
        if (columnOffset + length > matrix.ColumnCount)
          throw new ArgumentException("Columnoffset+length exceed the number of columns of the matrix");

        _matrix = matrix;
        _row = row;
        _columnOffset = columnOffset;
        _length = length;
      }

      #region IVector Members

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public T this[int i]
      {
        get
        {
          return _matrix[_row, i + _columnOffset];
        }
        set
        {
          _matrix[_row, i + _columnOffset] = value;
        }
      }

      #endregion IVector Members

      #region IROVector Members

      /// <summary>The number of elements of this vector.</summary>
      public int Length
      {
        get
        {
          return _length;
        }
      }

      /// <summary>The number of elements of this vector.</summary>
      public int Count
      {
        get
        {
          return _length;
        }
      }

      /// <inheritdoc/>
      public IEnumerator<T> GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }

      /// <inheritdoc/>
      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }

      #endregion IROVector Members
    }

    #endregion MatrixRowROVector

    #region MatrixColumnROVector

    /// <summary>
    /// Wrapper for a matrix column to a read-only vector.
    /// </summary>
    public class MatrixColumnROVector<T> : IROVector<T> where T : struct
    {
      private IROMatrix<T> _matrix;
      private int _column;

      /// <summary>
      /// Constructor of a matrix column vector by providing the matrix and the column number of that matrix that is wrapped.
      /// </summary>
      /// <param name="matrix">The matrix.</param>
      /// <param name="column">The column number of the matrix that is wrapped to a vector.</param>
      public MatrixColumnROVector(IROMatrix<T> matrix, int column)
      {
        if (matrix is null)
          throw new ArgumentNullException("IMatrix m is null");
        if (column < 0 || column >= matrix.ColumnCount)
          throw new ArgumentOutOfRangeException("The parameter row is either <0 or greater than the rows of the matrix");

        _matrix = matrix;
        _column = column;
      }

      #region IROVector Members

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public T this[int i]
      {
        get
        {
          return _matrix[i, _column];
        }
      }

      /// <summary>The number of elements of this vector.</summary>
      public int Length
      {
        get
        {
          return _matrix.RowCount;
        }
      }

      /// <inheritdoc/>
      public int Count
      {
        get
        {
          return _matrix.RowCount;
        }
      }

      /// <inheritdoc/>
      public IEnumerator<T> GetEnumerator()
      {
        for (int i = 0; i < _matrix.RowCount; ++i)
          yield return this[i];
      }

      /// <inheritdoc/>
      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < _matrix.RowCount; ++i)
          yield return this[i];
      }

      #endregion IROVector Members
    }

    #endregion MatrixColumnROVector

    #region MatrixColumnVector

    /// <summary>
    /// Wrapper for a matrix row to a vector.
    /// </summary>
    public class MatrixColumnVector<T> : IVector<T> where T : struct
    {
      private IMatrix<T> _matrix;
      private int _column;

      /// <summary>
      /// Constructor of a matrix row vector by providing the matrix and the row number of that matrix that is wrapped.
      /// </summary>
      /// <param name="matrix">The matrix.</param>
      /// <param name="column">The column number of the matrix that is wrapped to a vector.</param>
      public MatrixColumnVector(IMatrix<T> matrix, int column)
      {
        if (matrix is null)
          throw new ArgumentNullException("IMatrix m is null");
        if (column < 0 || column >= matrix.ColumnCount)
          throw new ArgumentOutOfRangeException("The parameter row is either <0 or greater than the rows of the matrix");

        _matrix = matrix;
        _column = column;
      }

      #region IVector Members

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public T this[int i]
      {
        get
        {
          return _matrix[i, _column];
        }
        set
        {
          _matrix[i, _column] = value;
        }
      }

      #endregion IVector Members

      #region IROVector Members

      /// <summary>The number of elements of this vector.</summary>
      public int Length
      {
        get
        {
          return _matrix.RowCount;
        }
      }

      /// <inheritdoc/>
      public int Count
      {
        get
        {
          return _matrix.RowCount;
        }
      }

      /// <inheritdoc/>
      public IEnumerator<T> GetEnumerator()
      {
        for (int i = 0; i < _matrix.RowCount; ++i)
          yield return this[i];
      }

      /// <inheritdoc/>
      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < _matrix.RowCount; ++i)
          yield return this[i];
      }

      #endregion IROVector Members
    }

    #endregion MatrixColumnVector

    #region SubMatrixROWrapper

    /// <summary>
    /// Wraps part of a matrix so that it can be used as read-only submatrix in operations.
    /// </summary>
    private class SubMatrixROWrapper<T> : IROMatrix<T> where T : struct
    {
      private IROMatrix<T> _matrix;
      private int _rowOffset, _columnOffset;
      private int _rows, _cols;

      #region IROMatrix Members

      /// <summary>
      /// Creates an instance of <see cref="SubMatrixROWrapper{T}"/>
      /// </summary>
      /// <param name="matrix">Matrix to wrap.</param>
      /// <param name="rowOffset">First row of the matrix that will become row 0 of the submatrix.</param>
      /// <param name="columnOffset">First column of the matrix that will become column 0 of the submatrix.</param>
      /// <param name="rows">Number of rows of the submatrix.</param>
      /// <param name="cols">Number of columns of the submatrix.</param>
      public SubMatrixROWrapper(IROMatrix<T> matrix, int rowOffset, int columnOffset, int rows, int cols)
      {
        _matrix = matrix;
        _rowOffset = rowOffset;
        _columnOffset = columnOffset;
        _rows = rows;
        _cols = cols;
      }

      /// <inheritdoc/>
      public T this[int row, int col]
      {
        get
        {
          return _matrix[row + _rowOffset, col + _columnOffset];
        }
      }

      /// <inheritdoc/>
      public int RowCount
      {
        get
        {
          return _rows;
        }
      }

      /// <inheritdoc/>
      public int ColumnCount
      {
        get
        {
          return _cols;
        }
      }

      #endregion IROMatrix Members
    }

    #endregion SubMatrixROWrapper

    #region SubMatrixWrapper

    /// <summary>
    /// Wraps part of a matrix so that it can be used as submatrix in operations.
    /// </summary>
    private class SubMatrixWrapper<T> : IMatrix<T> where T : struct
    {
      private IMatrix<T> _matrix;
      private int _rowOffset, _columnOffset;
      private int _rows, _cols;

      #region IROMatrix Members

      /// <summary>
      /// Creates an instance of <see cref="SubMatrixWrapper{T}"/>
      /// </summary>
      /// <param name="matrix">Matrix to wrap.</param>
      /// <param name="rowoffset">First row of the matrix that will become row 0 of the submatrix.</param>
      /// <param name="coloffset">First column of the matrix that will become column 0 of the submatrix.</param>
      /// <param name="rows">Number of rows of the submatrix.</param>
      /// <param name="cols">Number of columns of the submatrix.</param>
      public SubMatrixWrapper(IMatrix<T> matrix, int rowoffset, int coloffset, int rows, int cols)
      {
        _matrix = matrix;
        _rowOffset = rowoffset;
        _columnOffset = coloffset;
        _rows = rows;
        _cols = cols;
      }

      /// <inheritdoc/>
      public T this[int row, int col]
      {
        get
        {
          return _matrix[row + _rowOffset, col + _columnOffset];
        }
      }

      /// <inheritdoc/>
      public int RowCount
      {
        get
        {
          return _rows;
        }
      }

      /// <inheritdoc/>
      public int ColumnCount
      {
        get
        {
          return _cols;
        }
      }

      #endregion IROMatrix Members

      #region IMatrix Members

      /// <inheritdoc/>
      T Altaxo.Calc.LinearAlgebra.IMatrix<T>.this[int row, int col]
      {
        get
        {
          return _matrix[row + _rowOffset, col + _columnOffset];
        }
        set
        {
          _matrix[row + _rowOffset, col + _columnOffset] = value;
        }
      }

      #endregion IMatrix Members
    }

    #endregion SubMatrixWrapper

    #region DiagonalMatrix

    /// <summary>
    /// Wraps a vector to a diagonal matrix
    /// </summary>
    private class RODiagonalMatrixVectorWrapper<T> : IROMatrix<T> where T : struct
    {
      private IReadOnlyList<T> _diagonal;
      private int _offset, _matrixDimensions;

      /// <summary>
      /// Creates a wrapper that exposes a vector as a diagonal matrix.
      /// </summary>
      /// <param name="vector">The vector to wrap. This will form the diagonal of the matrix</param>
      /// <param name="offset">Offset in the vector. Element at offset is the first element of the matrix diagonal.</param>
      /// <param name="matrixdimensions">Number of rows = number of columns of the matrix.</param>
      public RODiagonalMatrixVectorWrapper(IReadOnlyList<T> vector, int offset, int matrixdimensions)
      {
        _diagonal = vector;
        _offset = offset;
        _matrixDimensions = matrixdimensions;
      }

      #region IROMatrix Members

      /// <inheritdoc/>
      public T this[int row, int col]
      {
        get
        {
          return row == col ? _diagonal[row + _offset] : default;
        }
      }


      /// <inheritdoc/>
      public int RowCount
      {
        get
        {
          return _matrixDimensions;
        }
      }

      /// <inheritdoc/>
      public int ColumnCount
      {
        get
        {
          return _matrixDimensions;
        }
      }

      #endregion IROMatrix Members
    }

    #endregion DiagonalMatrix

    #region Wrapper from linear array (column major order, i.e. LAPACK convention) to IROMatrix

    /// <summary>
    /// Wraps a linear array to a read-only matrix. The array is column oriented, i.e. consecutive elements
    /// belong mostly to one column. This is the convention used for LAPACK routines.
    /// </summary>
    public class ROMatrixFromColumnMajorLinearArray<T> : IROMatrix<T> where T : struct
    {
      protected T[] _array;
      protected int _rows;
      protected int _columns;

      #region IROMatrix Members

      /// <summary>
      /// Initializes a new instance of the <see cref="ROMatrixFromColumnMajorLinearArray{T}"/> class.
      /// </summary>
      /// <param name="array">The linear array in column major order.</param>
      /// <param name="nRows">The number of rows of the matrix. The number of columns are calculated from the length of the array and the number of rows.</param>
      /// <exception cref="System.ArgumentException"></exception>
      public ROMatrixFromColumnMajorLinearArray(T[] array, int nRows)
      {
        if (array.Length % nRows != 0)
          throw new ArgumentException(string.Format("Length of array {0} is not a multiple of nRows={1}", array.Length, nRows));

        _array = array;
        _rows = nRows;
        _columns = array.Length / nRows;
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="ROMatrixFromColumnMajorLinearArray{T}"/> class.
      /// </summary>
      /// <param name="wrapper">Wrapper that wraps a linear array in column major order and provides the number of rows and columns.</param>
      public ROMatrixFromColumnMajorLinearArray(MatrixWrapperStructForColumnMajorOrderLinearArray<T> wrapper)
      {
        _array = wrapper.Array;
        _rows = wrapper.RowCount;
        _columns = wrapper.ColumnCount;
      }

      /// <inheritdoc/>
      public T this[int row, int col]
      {
        get
        {
          return _array[col * _rows + row];
        }
      }

      /// <inheritdoc/>
      public int RowCount
      {
        get { return _rows; }
      }

      /// <inheritdoc/>
      public int ColumnCount
      {
        get { return _columns; }
      }

      #endregion IROMatrix Members
    }

    #endregion Wrapper from linear array (column major order, i.e. LAPACK convention) to IROMatrix

    #region Wrapper from linear array (column major order, i.e. LAPACK convention) to read/write matrix

    /// <summary>
    /// Wraps a linear array to a read-write matrix. The array is column oriented, i.e. consecutive elements
    /// belong mostly to one column. This is the convention used for LAPACK routines.
    /// </summary>
    public class MatrixFromColumnMajorLinearArray<T> : ROMatrixFromColumnMajorLinearArray<T>, IMatrix<T> where T : struct
    {
      /// <summary>
      /// Initializes a new instance of the <see cref="MatrixFromColumnMajorLinearArray{T}"/> class.
      /// </summary>
      /// <param name="array">The linear array in column major order.</param>
      /// <param name="nRows">The number of rows of the matrix. The number of colums are calculated from the length of the array and the number of rows.</param>
      /// <exception cref="System.ArgumentException"></exception>
      public MatrixFromColumnMajorLinearArray(T[] array, int nRows)
        : base(array, nRows)
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="MatrixFromColumnMajorLinearArray{T}"/> class.
      /// </summary>
      /// <param name="wrapper">Wrapper that wraps a linear array in column major order and provides the number of rows and columns.</param>
      public MatrixFromColumnMajorLinearArray(MatrixWrapperStructForColumnMajorOrderLinearArray<T> wrapper)
        : base(wrapper)
      {
      }

      /// <inheritdoc/>
      public new T this[int row, int col]
      {
        get
        {
          return _array[col * _rows + row];
        }

        set
        {
          _array[col * _rows + row] = value;
        }
      }
    }

    #endregion Wrapper from linear array (column major order, i.e. LAPACK convention) to read/write matrix

    #region Wrapper from linear array (row major order) to IROMatrix

    /// <summary>
    /// Wraps a linear array to a read-only matrix. The array is column oriented, i.e. consecutive elements
    /// belong mostly to one column. This is the convention used for LAPACK routines.
    /// </summary>
    public class ROMatrixFromRowMajorLinearArray<T> : IROMatrix<T> where T : struct
    {
      protected T[] _array;
      protected int _rows;
      protected int _columns;

      #region IROMatrix Members

      /// <summary>
      /// Initializes a new instance of the <see cref="ROMatrixFromColumnMajorLinearArray{T}"/> class.
      /// </summary>
      /// <param name="array">The linear array in column major order.</param>
      /// <param name="nColumns">The number of columns of the matrix. The number of rows are calculated from the length of the array and the number of columns.</param>
      /// <exception cref="System.ArgumentException"></exception>
      public ROMatrixFromRowMajorLinearArray(T[] array, int nColumns)
      {
        if (array.Length % nColumns != 0)
          throw new ArgumentException(string.Format("Length of array {0} is not a multiple of nColumns={1}", array.Length, nColumns));

        _array = array;
        _rows = array.Length / nColumns;
        _columns = nColumns;
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="ROMatrixFromRowMajorLinearArray{T}"/> class.
      /// </summary>
      /// <param name="wrapper">Wrapper that wraps a linear array in column major order and provides the number of rows and columns.</param>
      public ROMatrixFromRowMajorLinearArray(MatrixWrapperStructForRowMajorOrderLinearArray<T> wrapper)
      {
        _array = wrapper.Array;
        _rows = wrapper.RowCount;
        _columns = wrapper.ColumnCount;
      }

      /// <inheritdoc/>
      public T this[int row, int col]
      {
        get
        {
          return _array[row * _columns + col];
        }
      }

      /// <inheritdoc/>
      public int RowCount
      {
        get { return _rows; }
      }

      /// <inheritdoc/>
      public int ColumnCount
      {
        get { return _columns; }
      }

      #endregion IROMatrix Members
    }

    #endregion Wrapper from linear array (row major order) to IROMatrix

    #region Wrapper from linear array (row major order) to read/write matrix

    /// <summary>
    /// Wraps a linear array to a read-write matrix. The array is row oriented, i.e. consecutive elements
    /// belong mostly to one row.
    /// </summary>
    public class MatrixFromRowMajorLinearArray<T> : ROMatrixFromRowMajorLinearArray<T>, IMatrix<T> where T : struct
    {
      /// <summary>
      /// Initializes a new instance of the <see cref="MatrixFromColumnMajorLinearArray{T}"/> class.
      /// </summary>
      /// <param name="array">The linear array in column major order.</param>
      /// <param name="nRows">The number of rows of the matrix. The number of colums are calculated from the length of the array and the number of rows.</param>
      /// <exception cref="System.ArgumentException"></exception>
      public MatrixFromRowMajorLinearArray(T[] array, int nRows)
        : base(array, nRows)
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="MatrixFromColumnMajorLinearArray{T}"/> class.
      /// </summary>
      /// <param name="wrapper">Wrapper that wraps a linear array in column major order and provides the number of rows and columns.</param>
      public MatrixFromRowMajorLinearArray(MatrixWrapperStructForRowMajorOrderLinearArray<T> wrapper)
        : base(wrapper)
      {
      }

      /// <inheritdoc/>
      public new T this[int row, int col]
      {
        get
        {
          return _array[row * _columns + col];
        }

        set
        {
          _array[row * _columns + col] = value;
        }
      }
    }

    #endregion Wrapper from linear array (row major order) to read/write matrix

    #region Wrapper from 2-dimensional array

    /// <summary>
    /// Wraps a 2d array to a read-only matrix. 
    /// </summary>
    public class ROMatrixFrom2DArray<T> : IROMatrix<T> where T : struct
    {
      protected T[,] _array;
      int _rows;
      int _columns;

      #region IROMatrix Members

      /// <summary>
      /// Initializes a new instance of the <see cref="ROMatrixFrom2DArray{T}"/> class.
      /// </summary>
      /// <param name="array">The linear array in column major order.</param>
      /// <param name="nRows">The number of rows of the matrix. The number of colums are calculated from the length of the array and the number of rows.</param>
      /// <exception cref="System.ArgumentException"></exception>
      public ROMatrixFrom2DArray(T[,] array)
      {
        _array = array;
        _rows = array.GetLength(0);
        _columns = array.GetLength(1);
      }


      /// <inheritdoc/>
      public T this[int row, int col]
      {
        get
        {
          return _array[row, col];
        }
      }

      /// <inheritdoc/>
      public int RowCount
      {
        get { return _rows; }
      }

      /// <inheritdoc/>
      public int ColumnCount
      {
        get { return _columns; }
      }

      #endregion IROMatrix Members
    }

    /// <summary>
    /// Wraps a 2D array to a read-only matrix. 
    /// </summary>
    public class RWMatrixFrom2DArray<T> : IMatrix<T> where T : struct
    {
      protected T[,] _array;
      int _rows;
      int _columns;

      #region IROMatrix Members

      /// <summary>
      /// Initializes a new instance of the <see cref="ROMatrixFrom2DArray{T}"/> class.
      /// </summary>
      /// <param name="array">The linear array in column major order.</param>
      /// <exception cref="System.ArgumentException"></exception>
      public RWMatrixFrom2DArray(T[,] array)
      {
        _array = array;
        _rows = array.GetLength(0);
        _columns = array.GetLength(1);
      }


      /// <inheritdoc/>
      public T this[int row, int col]
      {
        get
        {
          return _array[row, col];
        }
        set
        {
          _array[row, col] = value;
        }
      }

      /// <inheritdoc/>
      public int RowCount
      {
        get { return _rows; }
      }

      /// <inheritdoc/>
      public int ColumnCount
      {
        get { return _columns; }
      }

      #endregion IROMatrix Members
    }

    public static IROMatrix<T> ToROMatrix<T>(this T[,] array) where T: struct
    {
      return new ROMatrixFrom2DArray<T>(array);
    }

    public static IMatrix<T> ToMatrix<T>(this T[,] array) where T : struct
    {
      return new RWMatrixFrom2DArray<T>(array);
    }

    #endregion

    #region LeftSpineJaggedArrayMatrix

    /// <summary>
    /// BEMatrix is a matrix implementation that is relatively easy to extend to the botton, i.e. to append rows.
    /// It is horizontal oriented, i.e. the storage is as a number of horizontal vectors.
    /// </summary>
    public class LeftSpineJaggedArrayMatrix<T> : IMatrix<T>, IBottomExtensibleMatrix<T> where T : struct
    {
      private static readonly T[][] _emptyArray = new T[0][];
      /// <summary>The rows of the matrix = number of double[] arrays in it.</summary>
      private int _rows;

      /// <summary>The cols of the matrix = length of each double[] array.</summary>
      private int _columns;

      /// <summary>The array which holds the matrix.</summary>
      private T[][] _array = _emptyArray;

      /// <summary>
      /// Sets up an empty matrix with dimension(row,cols).
      /// </summary>
      /// <param name="rows">Number of rows of the matrix.</param>
      /// <param name="cols">Number of cols of the matrix.</param>
      public LeftSpineJaggedArrayMatrix(int rows, int cols)
      {
        SetDimension(rows, cols);
      }

      /// <summary>
      /// Uses an already existing array for the matrix data.
      /// </summary>
      /// <param name="x">Jagged double array containing the matrix data. The data are used directly (no copy)!</param>
      public LeftSpineJaggedArrayMatrix(T[][] x)
      {
        _array = x;
        _rows = _array.Length;
        _columns = _rows == 0 ? 0 : _array[0].Length;
      }

      /// <summary>
      /// Uses an already existing array for the matrix data.
      /// </summary>
      /// <param name="wrapper">Wrapper around a left spine jagged array containing the matrix data. The data are used directly (no copy)!</param>
      public LeftSpineJaggedArrayMatrix(MatrixWrapperStructForLeftSpineJaggedArray<T> wrapper)
      {
        _array = wrapper.Array;
        _rows = wrapper.RowCount;
        _columns = wrapper.ColumnCount;
      }

      public void Clear()
      {
        SetDimension(0, 0);
      }

      public override string ToString()
      {
        return MatrixMath.MatrixToString(null, this);
      }

      #region IMatrix Members

      /// <summary>
      /// Set up the dimensions of the matrix. Discards the old content and reset the matrix with the new dimensions. All elements
      /// become zero.
      /// </summary>
      /// <param name="rows">Number of rows of the matrix.</param>
      /// <param name="cols">Number of columns of the matrix.</param>
      public void SetDimension(int rows, int cols)
      {
        _rows = rows;
        _columns = cols;
        _array = new T[2 * (rows + 32)][];
        for (int i = 0; i < _rows; i++)
          _array[i] = new T[cols];
      }

      /// <summary>
      /// Element accessor. Accesses the element [row, col] of the matrix.
      /// </summary>
      public T this[int row, int col]
      {
        get
        {
          return _array[row][col];
        }
        set
        {
          _array[row][col] = value;
        }
      }

      /// <summary>
      /// Number of Rows of the matrix.
      /// </summary>
      public int RowCount
      {
        get
        {
          return _rows;
        }
      }

      /// <summary>
      /// Number of columns of the matrix.
      /// </summary>
      public int ColumnCount
      {
        get
        {
          return _columns;
        }
      }

      #endregion IMatrix Members

      #region IBottomExtensibleMatrix Members

      /// <summary>
      /// Appends the matrix a at the bottom of this matrix. Either this matrix must be empty (dimensions (0,0)) or
      /// the matrix to append must have the same number of columns than this matrix.
      /// </summary>
      /// <param name="a">Matrix to append to the bottom of this matrix.</param>
      public void AppendBottom(IROMatrix<T> a)
      {
        if (a.RowCount == 0)
          return; // nothing to append

        if (ColumnCount > 0)
        {
          if (a.ColumnCount != ColumnCount) // throw an error if this column is not empty and the columns does not match
            throw new ArithmeticException(string.Format("The number of columns of this matrix ({0}) and of the matrix to append ({1}) does not match!", ColumnCount, a.ColumnCount));
        }
        else // if the matrix was empty before
        {
          _columns = a.ColumnCount;
        }

        int newRows = a.RowCount + RowCount;

        // we must reallocate the array if neccessary
        if (newRows >= _array.Length)
        {
          var newArray = new T[2 * (newRows + 32)][];

          for (int i = 0; i < _rows; i++)
            newArray[i] = _array[i]; // copy the existing horizontal vectors directly

          _array = newArray;
        }

        // copy the new rows now
        for (int i = _rows; i < newRows; i++)
        {
          _array[i] = new T[_columns]; // create new horizontal vectors for the elements to append
          for (int j = 0; j < _columns; j++)
#pragma warning disable CS8601 // Possible null reference assignment.
            _array[i][j] = a[i - _rows, j]; // copy the elements
#pragma warning restore CS8601 // Possible null reference assignment.
        }

        _rows = newRows;
      }

      #endregion IBottomExtensibleMatrix Members
    }

    #endregion LeftSpineJaggedArrayMatrix

    #region TopSpineJaggedArrayMatrix

    /// <summary>
    /// REMatrix is a matrix implementation that is relatively easy to extend to the right, i.e. to append columns.
    /// It is vertical oriented, i.e. the storage is as a number of vertical vectors.
    /// </summary>
    public class TopSpineJaggedArrayMatrix<T> : IMatrix<T>, IRightExtensibleMatrix<T> where T : struct
    {
      private static T[][] _emptyArray = new T[0][];

      /// <summary>The rows of the matrix = length of each double[] array.</summary>
      private int _rows;

      /// <summary>The cols of the matrix = number of double[] arrays in it.</summary>
      private int _columns;

      /// <summary>The array which holds the matrix.</summary>
      private T[][] _array = _emptyArray;

      /// <summary>
      /// Sets up an empty matrix with dimension(row,cols).
      /// </summary>
      /// <param name="rows">Number of rows of the matrix.</param>
      /// <param name="cols">Number of cols of the matrix.</param>
      public TopSpineJaggedArrayMatrix(int rows, int cols)
      {
        SetDimension(rows, cols);
      }

      /// <summary>
      /// Constructs an RE matrix from an array of double vectors. Attention! The double vectors (the second) dimensions are here
      /// the columns (!) of the matrix. The data is not copied.
      /// </summary>
      /// <param name="from"></param>
      public TopSpineJaggedArrayMatrix(T[][] from)
      {
        _columns = from.Length;
        _rows = from[0].Length;
        for (int i = 1; i < _columns; i++)
          if (from[i].Length != _rows)
            throw new ArgumentException("The columns of the provided array (second dimension here) are not of equal length");

        _array = from;
      }

      /// <summary>
      /// Constructs an top spine jagged array matrix from a wrapper.
      /// The data is not copied, but used directly.
      /// </summary>
      /// <param name="wrapper">Wrapper around a top spine jagged array.</param>
      public TopSpineJaggedArrayMatrix(MatrixWrapperStructForTopSpineJaggedArray<T> wrapper)
      {
        _array = wrapper.Array;
        _rows = wrapper.RowCount;
        _columns = wrapper.ColumnCount;
      }

      public void Clear()
      {
        SetDimension(0, 0);
      }

      public override string ToString()
      {
        return MatrixMath.MatrixToString(null, this);
      }

      #region IMatrix Members

      /// <summary>
      /// Set up the dimensions of the matrix. Discards the old content and reset the matrix with the new dimensions. All elements
      /// become zero.
      /// </summary>
      /// <param name="rows">Number of rows of the matrix.</param>
      /// <param name="cols">Number of columns of the matrix.</param>
      public void SetDimension(int rows, int cols)
      {
        _rows = rows;
        _columns = cols;
        _array = new T[2 * (cols + 32)][];
        for (int i = 0; i < _columns; i++)
          _array[i] = new T[rows];
      }

      /// <summary>
      /// Element accessor. Accesses the element [row, col] of the matrix.
      /// </summary>
      public T this[int row, int col]
      {
        get
        {
          return _array[col][row];
        }
        set
        {
          _array[col][row] = value;
        }
      }

      /// <summary>
      /// Number of Rows of the matrix.
      /// </summary>
      public int RowCount
      {
        get
        {
          return _rows;
        }
      }

      /// <summary>
      /// Number of columns of the matrix.
      /// </summary>
      public int ColumnCount
      {
        get
        {
          return _columns;
        }
      }

      #endregion IMatrix Members

      #region IRightExtensibleMatrix Members

      /// <summary>
      /// Appends the matrix a at the right of this matrix. Either this matrix must be empty (dimensions (0,0)) or
      /// the matrix to append must have the same number of rows than this matrix.
      /// </summary>
      /// <param name="a">Matrix to append to the right of this matrix.</param>
      public void AppendRight(IROMatrix<T> a)
      {
        if (a.ColumnCount == 0)
          return; // nothing to append

        if (RowCount > 0)
        {
          if (a.RowCount != RowCount) // throw an error if this column is not empty and the columns does not match
            throw new ArithmeticException(string.Format("The number of rows of this matrix ({0}) and of the matrix to append ({1}) does not match!", RowCount, a.RowCount));
        }
        else // if the matrix was empty before set the number of rows
        {
          _rows = a.RowCount;
        }

        int newCols = a.ColumnCount + ColumnCount;

        // we must newly allocate the bone array, if neccessary
        if (newCols >= _array.Length)
        {
          var newArray = new T[2 * (newCols + 32)][];

          for (int i = 0; i < _columns; i++)
            newArray[i] = _array[i]; // copy the existing horizontal vectors.

          _array = newArray;
        }

        // copy the new rows
        for (int i = _columns; i < newCols; i++)
        {
          _array[i] = new T[_rows]; // create new horizontal vectors for the elements to append
          for (int j = 0; j < _rows; j++)
#pragma warning disable CS8601 // Possible null reference assignment.
            _array[i][j] = a[j, i - _columns]; // copy the elements
#pragma warning restore CS8601 // Possible null reference assignment.
        }

        _columns = newCols;
      }

      #endregion IRightExtensibleMatrix Members
    }

    #endregion TopSpineJaggedArrayMatrix
  }
}
