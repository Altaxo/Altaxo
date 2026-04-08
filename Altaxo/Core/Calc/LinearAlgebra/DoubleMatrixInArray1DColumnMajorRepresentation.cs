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
  /// The linear array is in column-major order, i.e. the first elements of the linear array belong to the first column of the matrix (i.e. the row values change more quickly).
  /// The index of the linear array is calculated as <c>index = row + column*NumberOfRows</c>. This representation is used for instance by LAPACK, Fortran, Julia, MATLAB, Octave, Scilab, GLSL and HLSL.
  /// </summary>
  public class RODoubleMatrixInArray1DColumnMajorRepresentation : IROMatrix<double>
  {
    /// <summary>
    /// The backing store for the matrix data, stored in column-major order.
    /// </summary>
    protected double[] _arrayColumnMajor;
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
    /// Initializes a new instance of the <see cref="RODoubleMatrixInArray1DColumnMajorRepresentation"/> class.
    /// </summary>
    /// <param name="rows">The number of rows in the matrix.</param>
    /// <param name="columns">The number of columns in the matrix.</param>
    public RODoubleMatrixInArray1DColumnMajorRepresentation(int rows, int columns)
    {
      if (rows < 0 && columns < 0)
        throw new ArgumentOutOfRangeException("rows or columns is less than zero.");

      _arrayColumnMajor = new double[rows * columns];
      _numberOfRows = rows;
      _numberOfColumns = columns;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RODoubleMatrixInArray1DColumnMajorRepresentation"/> class with an existing array.
    /// </summary>
    /// <param name="array">The array to wrap.</param>
    /// <param name="rows">The number of rows in the matrix.</param>
    /// <param name="columns">The number of columns in the matrix.</param>
    public RODoubleMatrixInArray1DColumnMajorRepresentation(double[] array, int rows, int columns)
    {
      if (rows < 0 && columns < 0)
        throw new ArgumentOutOfRangeException("rows or columns is less than zero.");
      if (array is null)
        throw new ArgumentNullException("array");
      if (array.Length < RowCount * columns)
        throw new ArgumentException(string.Format("Length of array {0} is not a multiple of nRows={1}", array.Length, rows));

      _arrayColumnMajor = array;
      _numberOfRows = rows;
      _numberOfColumns = columns;
    }

    /// <summary>
    /// Gets or sets the element at the specified row and column.
    /// </summary>
    /// <param name="row">The zero-based row index.</param>
    /// <param name="column">The zero-based column index.</param>
    /// <returns>The value at the specified row and column.</returns>
    public double this[int row, int column]
    {
      get
      {
        return _arrayColumnMajor[row + column * _numberOfRows];
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
  /// The linear array is in column-major order, i.e. the first elements of the linear array belong to the first column of the matrix (i.e. the row values change more quickly).
  /// The index of the linear array is calculated as <c>index = row + column*NumberOfRows</c>. This representation is used for instance by LAPACK, Fortran, Julia, MATLAB, Octave, Scilab, GLSL and HLSL.
  /// </summary>
  public class DoubleMatrixInArray1DColumnMajorRepresentation : RODoubleMatrixInArray1DColumnMajorRepresentation, IMatrix<double>, IMatrixInArray1DColumnMajorRepresentation<double>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DoubleMatrixInArray1DColumnMajorRepresentation"/> class.
    /// </summary>
    /// <param name="rows">The number of rows in the matrix.</param>
    /// <param name="columns">The number of columns in the matrix.</param>
    public DoubleMatrixInArray1DColumnMajorRepresentation(int rows, int columns)
      : base(rows, columns)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DoubleMatrixInArray1DColumnMajorRepresentation"/> class with an existing array.
    /// </summary>
    /// <param name="array">The array to wrap.</param>
    /// <param name="nRows">The number of rows in the matrix.</param>
    /// <param name="columns">The number of columns in the matrix.</param>
    public DoubleMatrixInArray1DColumnMajorRepresentation(double[] array, int nRows, int columns)
      : base(array, nRows, columns)
    {
    }

    #region IMatrix Members

    /// <summary>
    /// Gets or sets the element at the specified row and column.
    /// </summary>
    /// <param name="row">The zero-based row index.</param>
    /// <param name="column">The zero-based column index.</param>
    /// <returns>The value at the specified row and column.</returns>
    public new double this[int row, int column]
    {
      get
      {
        return _arrayColumnMajor[row + column * _numberOfRows];
      }

      set
      {
        _arrayColumnMajor[row + column * _numberOfRows] = value;
      }
    }

    /// <summary>
    /// Gets the underlying one-dimensional array that represents this matrix in column-major order.
    /// </summary>
    /// <returns>The one-dimensional array that represents this matrix.</returns>
    public double[] GetArray1DColumnMajor()
    {
      return _arrayColumnMajor;
    }

    #endregion IMatrix Members
  }
}
