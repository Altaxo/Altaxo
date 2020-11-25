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
    protected double[] _arrayRowMajor;
    protected int _numberOfRows;
    protected int _numberOfColumns;

    #region IROMatrix Members

    public RODoubleMatrixInArray1DRowMajorRepresentation(int rows, int columns)
    {
      if (rows < 0 && columns < 0)
        throw new ArgumentOutOfRangeException("rows or columns is less than zero.");

      _arrayRowMajor = new double[rows * columns];
      _numberOfRows = rows;
      _numberOfColumns = columns;
    }

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

    public double this[int row, int column]
    {
      get
      {
        return _arrayRowMajor[column + row * _numberOfColumns];
      }
    }

    public int RowCount
    {
      get { return _numberOfRows; }
    }

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
    public DoubleMatrixInArray1DRowMajorRepresentation(int rows, int columns)
      : base(rows, columns)
    {
    }

    public DoubleMatrixInArray1DRowMajorRepresentation(double[] array, int nRows, int columns)
      : base(array, nRows, columns)
    {
    }

    #region IMatrix Members

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

    public double[] GetArray1DRowMajor()
    {
      return _arrayRowMajor;
    }

    #endregion IMatrix Members
  }
}
