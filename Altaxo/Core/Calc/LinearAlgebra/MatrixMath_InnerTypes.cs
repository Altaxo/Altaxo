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
		public class ScalarAsMatrix<T> : IMatrix<T>, IVector<T>
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

			/// <summary>
			/// Creates the scalar with the default value of zero.
			/// </summary>
			public ScalarAsMatrix()
			{
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
			public int Rows
			{
				get
				{
					return 1;
				}
			}

			/// <summary>
			/// Number of columns of the matrix. Always 1 (one).
			/// </summary>
			public int Columns
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
					return this._value;
				}
				set
				{
					this._value = value;
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
		public class MatrixWithOneRow<T> : IMatrix<T>, IVector<T>
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
			public int Rows
			{
				get
				{
					return 1;
				}
			}

			/// <summary>
			/// Number of columns, i.e. number of elements of the horizontal vector.
			/// </summary>
			public int Columns
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
		public class MatrixWithOneColumn<T> : IMatrix<T>, IVector<T>
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
			public int Rows
			{
				get
				{
					return _array.Length;
				}
			}

			/// <summary>
			/// Number of columns of the matrix, always 1 (one) since it is a vertical vector.
			/// </summary>
			public int Columns
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
		public class MatrixRowROVector<T> : IROVector<T>
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
				: this(matrix, row, 0, matrix.Columns)
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
				if (matrix == null)
					throw new ArgumentNullException("IROMatrix m is null");
				if (row < 0 || row >= matrix.Rows)
					throw new ArgumentOutOfRangeException("The parameter row is either <0 or greater than the rows of the matrix");
				if (columnOffset + length > matrix.Columns)
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
		public class MatrixRowVector<T> : IVector<T>
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
				: this(matrix, row, 0, matrix.Columns)
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
				if (matrix == null)
					throw new ArgumentNullException("IMatrix m is null");
				if (row < 0 || row >= matrix.Rows)
					throw new ArgumentOutOfRangeException("The parameter row is either <0 or greater than the rows of the matrix");
				if (columnOffset + length > matrix.Columns)
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
		public class MatrixColumnROVector<T> : IROVector<T>
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
				if (matrix == null)
					throw new ArgumentNullException("IMatrix m is null");
				if (column < 0 || column >= matrix.Columns)
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
					return _matrix.Rows;
				}
			}

			/// <inheritdoc/>
			public int Count
			{
				get
				{
					return _matrix.Rows;
				}
			}

			/// <inheritdoc/>
			public IEnumerator<T> GetEnumerator()
			{
				for (int i = 0; i < _matrix.Rows; ++i)
					yield return this[i];
			}

			/// <inheritdoc/>
			IEnumerator IEnumerable.GetEnumerator()
			{
				for (int i = 0; i < _matrix.Rows; ++i)
					yield return this[i];
			}

			#endregion IROVector Members
		}

		#endregion MatrixColumnROVector

		#region MatrixColumnVector

		/// <summary>
		/// Wrapper for a matrix row to a vector.
		/// </summary>
		public class MatrixColumnVector<T> : IVector<T>
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
				if (matrix == null)
					throw new ArgumentNullException("IMatrix m is null");
				if (column < 0 || column >= matrix.Columns)
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
					return _matrix.Rows;
				}
			}

			/// <inheritdoc/>
			public int Count
			{
				get
				{
					return _matrix.Rows;
				}
			}

			/// <inheritdoc/>
			public IEnumerator<T> GetEnumerator()
			{
				for (int i = 0; i < _matrix.Rows; ++i)
					yield return this[i];
			}

			/// <inheritdoc/>
			IEnumerator IEnumerable.GetEnumerator()
			{
				for (int i = 0; i < _matrix.Rows; ++i)
					yield return this[i];
			}

			#endregion IROVector Members
		}

		#endregion MatrixColumnVector

		#region SubMatrixROWrapper

		/// <summary>
		/// Wraps part of a matrix so that it can be used as read-only submatrix in operations.
		/// </summary>
		private class SubMatrixROWrapper<T> : IROMatrix<T>
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
			public int Rows
			{
				get
				{
					return _rows;
				}
			}

			/// <inheritdoc/>
			public int Columns
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
		private class SubMatrixWrapper<T> : IMatrix<T>
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
			public int Rows
			{
				get
				{
					return _rows;
				}
			}

			/// <inheritdoc/>
			public int Columns
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
		private class RODiagonalMatrixVectorWrapper<T> : IROMatrix<T>
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
					if (row == col)
						return _diagonal[row + _offset];
					else
						return default(T);
				}
			}

			/// <inheritdoc/>
			public int Rows
			{
				get
				{
					return _matrixDimensions;
				}
			}

			/// <inheritdoc/>
			public int Columns
			{
				get
				{
					return _matrixDimensions;
				}
			}

			#endregion IROMatrix Members
		}

		#endregion DiagonalMatrix
	}
}