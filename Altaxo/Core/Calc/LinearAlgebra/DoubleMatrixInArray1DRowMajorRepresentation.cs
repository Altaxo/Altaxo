#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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

namespace Altaxo.Calc.LinearAlgebra
{
  /// <summary>
  /// Wraps a linear array to a read-only matrix.
  /// The array is in row-major order, i.e. the first elements of the linear array belong to the first row of the matrix (the column values change more quickly).
  /// The index of the linear array is calculated as <c>index = column + row*NumberOfColumns</c>.
  /// This representation is used for instance by C, C++, Mathematica, Pascal and Python.
  /// </summary>
  public class RODoubleMatrixInArray1DRowMajorRepresentation : IROMatrix<double>
  {
    /// <summary>
    /// The underlying array storing the matrix data in row-major order.
    /// </summary>
    protected double[] _arrayRowMajor;
    /// <summary>
    /// The number of rows in the matrix.
    /// </summary>
    protected int _numberOfRows;
    /// <summary>
    /// The number of columns in the matrix.
    /// </summary>
    protected int _numberOfColumns;

    #region IROMatrix Members

    /// <summary>
    /// Initializes a new instance of the <see cref="RODoubleMatrixInArray1DRowMajorRepresentation"/> class.
    /// </summary>
    /// <param name="rows">The number of rows in the matrix.</param>
    /// <param name="columns">The number of columns in the matrix.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when rows or columns are less than zero.</exception>
    public RODoubleMatrixInArray1DRowMajorRepresentation(int rows, int columns)
    {
      if (rows < 0 && columns < 0)
        throw new ArgumentOutOfRangeException("rows or columns is less than zero.");

      _arrayRowMajor = new double[rows * columns];
      _numberOfRows = rows;
      _numberOfColumns = columns;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RODoubleMatrixInArray1DRowMajorRepresentation"/> class.
    /// </summary>
    /// <param name="array">The array containing the matrix data in row-major order.</param>
    /// <param name="rows">The number of rows in the matrix.</param>
    /// <param name="columns">The number of columns in the matrix.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when rows or columns are less than zero.</exception>
    /// <exception cref="ArgumentNullException">Thrown when array is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the length of the array is not a multiple of the number of rows.</exception>
    public RODoubleMatrixInArray1DRowMajorRepresentation(double[] array, int rows, int columns)
    {
      if (rows < 0 && columns < 0)
        throw new ArgumentOutOfRangeException("rows or columns is less than zero.");
      if (array is null)
        throw new ArgumentNullException("array");
      if (array.Length < rows * columns)
        throw new ArgumentException(string.Format("Length of array {0} is not a multiple of nRows={1}", array.Length, rows));

      _arrayRowMajor = array;
      _numberOfRows = rows;
      _numberOfColumns = columns;
    }

    /// <summary>
    /// Gets the element at the specified row and column indices.
    /// </summary>
    /// <param name="row">The zero-based row index.</param>
    /// <param name="column">The zero-based column index.</param>
    /// <returns>The element at the specified row and column indices.</returns>
    public double this[int row, int column]
    {
      get
      {
        return _arrayRowMajor[column + row * _numberOfColumns];
      }
    }

    /// <summary>
    /// Gets the number of rows in the matrix.
    /// </summary>
    public int RowCount
    {
      get { return _numberOfRows; }
    }

    /// <summary>
    /// Gets the number of columns in the matrix.
    /// </summary>
    public int ColumnCount
    {
      get { return _numberOfColumns; }
    }

    #endregion IROMatrix Members
  }

  /// <summary>
  /// Wraps a linear array to a read-write matrix.
  /// The array is in row-major order, i.e. the first elements of the linear array belong to the first row of the matrix (the column values change more quickly).
  /// The index of the linear array is calculated as <c>index = column + row * NumberOfColumns</c>.
  /// This representation is used for instance by C, C++, Mathematica, Pascal and Python.
  /// </summary>
  public class DoubleMatrixInArray1DRowMajorRepresentation : RODoubleMatrixInArray1DRowMajorRepresentation, IMatrix<double>, IMatrixInArray1DRowMajorRepresentation<double>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DoubleMatrixInArray1DRowMajorRepresentation"/> class.
    /// </summary>
    /// <param name="rows">The number of rows in the matrix.</param>
    /// <param name="columns">The number of columns in the matrix.</param>
    /// <seealso cref="RODoubleMatrixInArray1DRowMajorRepresentation.RODoubleMatrixInArray1DRowMajorRepresentation(int, int)"/>
    public DoubleMatrixInArray1DRowMajorRepresentation(int rows, int columns)
      : base(rows, columns)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DoubleMatrixInArray1DRowMajorRepresentation"/> class.
    /// </summary>
    /// <param name="array">The array containing the matrix data in row-major order.</param>
    /// <param name="nRows">The number of rows in the matrix.</param>
    /// <param name="columns">The number of columns in the matrix.</param>
    /// <seealso cref="RODoubleMatrixInArray1DRowMajorRepresentation.RODoubleMatrixInArray1DRowMajorRepresentation(double[], int, int)"/>
    public DoubleMatrixInArray1DRowMajorRepresentation(double[] array, int nRows, int columns)
      : base(array, nRows, columns)
    {
    }

    #region IMatrix Members

    /// <summary>
    /// Gets or sets the element at the specified row and column indices.
    /// </summary>
    /// <param name="row">The zero-based row index.</param>
    /// <param name="column">The zero-based column index.</param>
    /// <returns>The element at the specified row and column indices.</returns>
    public new double this[int row, int column]
    {
      get
      {
        return _arrayRowMajor[column + row * _numberOfColumns];
      }

      set
      {
        _arrayRowMajor[column + row * _numberOfColumns] = value;
      }
    }

    /// <summary>
    /// Gets the underlying array storing the matrix data in row-major order.
    /// </summary>
    /// <returns>The underlying array.</returns>
    public double[] GetArray1DRowMajor()
    {
      return _arrayRowMajor;
    }

    #endregion IMatrix Members
  }
}
