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
    protected double[] _arrayColumnMajor;
    protected int _numberOfRows;
    protected int _numberOfColumns;

    #region IROMatrix Members

    public RODoubleMatrixInArray1DColumnMajorRepresentation(int rows, int columns)
    {
      if (rows < 0 && columns < 0)
        throw new ArgumentOutOfRangeException("rows or columns is less than zero.");

      _arrayColumnMajor = new double[rows * columns];
      _numberOfRows = rows;
      _numberOfColumns = columns;
    }

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

    public double this[int row, int column]
    {
      get
      {
        return _arrayColumnMajor[row + column * _numberOfRows];
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
  /// The linear array is in column-major order, i.e. the first elements of the linear array belong to the first column of the matrix (i.e. the row values change more quickly).
  /// The index of the linear array is calculated as <c>index = row + column*NumberOfRows</c>. This representation is used for instance by LAPACK, Fortran, Julia, MATLAB, Octave, Scilab, GLSL and HLSL.
  /// </summary>
  public class DoubleMatrixInArray1DColumnMajorRepresentation : RODoubleMatrixInArray1DColumnMajorRepresentation, IMatrix<double>, IMatrixInArray1DColumnMajorRepresentation<double>
  {
    public DoubleMatrixInArray1DColumnMajorRepresentation(int rows, int columns)
      : base(rows, columns)
    {
    }

    public DoubleMatrixInArray1DColumnMajorRepresentation(double[] array, int nRows, int columns)
      : base(array, nRows, columns)
    {
    }

    #region IMatrix Members

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

    public double[] GetArray1DColumnMajor()
    {
      return _arrayColumnMajor;
    }

    #endregion IMatrix Members
  }
}
