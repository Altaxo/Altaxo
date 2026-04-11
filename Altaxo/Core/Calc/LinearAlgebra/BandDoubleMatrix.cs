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
  /// Represents a band matrix configuration for storing and manipulating a 2D array of double-precision floating-point numbers.
  /// </summary>
  public class BandDoubleMatrix : IMatrix<double>, IROBandMatrix<double>, IROMatrixLevel1<double>, IMatrixLevel1<double>
  {
    private double[][] _array;
    private int _rowCount;
    private int _columnCount;
    private int _lowerBandwidth;
    private int _upperBandwidth;

    /// <summary>
    /// Initializes a new instance of the <see cref="BandDoubleMatrix"/> class with the specified dimensions and bandwidths.
    /// </summary>
    /// <param name="numberOfRows">The number of rows in the matrix.</param>
    /// <param name="numberOfColumns">The number of columns in the matrix.</param>
    /// <param name="p">The lower bandwidth of the matrix.</param>
    /// <param name="q">The upper bandwidth of the matrix.</param>
    public BandDoubleMatrix(int numberOfRows, int numberOfColumns, int p, int q)
    {
      _rowCount = numberOfRows;
      _columnCount = numberOfColumns;
      _lowerBandwidth = p;
      _upperBandwidth = q;
      _array = new double[_rowCount][];
      for (int i = 0; i < _array.Length; ++i)
        _array[i] = new double[_columnCount];
    }

    /// <summary>
    /// Gets the internal data structure of the matrix.
    /// </summary>
    public MatrixWrapperStructForLeftSpineJaggedArray<double> InternalData
    {
      get
      {
        return new MatrixWrapperStructForLeftSpineJaggedArray<double>(_array, _rowCount, _columnCount);
      }
    }

    /// <summary>
    /// Gets or sets the element at the specified row and column indices.
    /// </summary>
    /// <param name="row">The zero-based row index of the element to get or set.</param>
    /// <param name="col">The zero-based column index of the element to get or set.</param>
    /// <returns>The value of the element at the specified row and column indices.</returns>
    public double this[int row, int col]
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
    /// Gets the element at the specified row and column indices.
    /// </summary>
    /// <param name="row">The zero-based row index of the element to get.</param>
    /// <param name="col">The zero-based column index of the element to get.</param>
    /// <returns>The value of the element at the specified row and column indices.</returns>
    double IROMatrix<double>.this[int row, int col]
    {
      get
      {
        return _array[row][col];
      }
    }

    /// <summary>
    /// Gets the number of rows in the matrix.
    /// </summary>
    public int RowCount => _rowCount;

    /// <summary>
    /// Gets the number of columns in the matrix.
    /// </summary>
    public int ColumnCount => _columnCount;

    /// <summary>
    /// Gets the lower bandwidth of the matrix.
    /// </summary>
    public int LowerBandwidth { get { return _lowerBandwidth; } }
    /// <summary>
    /// Gets the upper bandwidth of the matrix.
    /// </summary>
    public int UpperBandwidth { get { return _upperBandwidth; } }

    /// <summary>
    /// Applies the specified function to each element of the matrix, with the option to specify a result matrix and treatment of zeros.
    /// </summary>
    /// <param name="function">The function to apply to each element.</param>
    /// <param name="result">The matrix to store the results in.</param>
    /// <param name="zeros">Specifies how to treat zero elements.</param>
    public void MapIndexed(Func<int, int, double, double> function, IMatrix<double> result, Zeros zeros = Zeros.AllowSkip)
    {
      int rowCount = _rowCount;
      int columnCount = _columnCount;

      if (zeros != Zeros.Include && !(result is IROBandMatrix<double> bm && bm.LowerBandwidth == _lowerBandwidth && bm.UpperBandwidth == _upperBandwidth))
      {
        MatrixMath.Clear(result);
      }

      switch (zeros)
      {
        case Zeros.AllowSkip:
          {
            for (int i = 0; i < rowCount; ++i)
            {
              var array_i = _array[i];
              int j_start = Math.Max(0, i - _lowerBandwidth);
              int j_end = Math.Min(columnCount, i + _upperBandwidth + 1);
              for (int j = j_start; j < j_end; ++j)
                result[i, j] = function(i, j, array_i[j]);
            }
          }
          break;

        case Zeros.Include:
          {
            for (int i = 0; i < rowCount; ++i)
            {
              var array_i = _array[i];
              int j_start = Math.Max(0, i - _lowerBandwidth);
              int j_end = Math.Min(columnCount, i + _upperBandwidth + 1);
              for (int j = 0; j < j_start; ++j)
                result[i, j] = function(i, j, 0);
              for (int j = j_start; j < j_end; ++j)
                result[i, j] = function(i, j, array_i[j]);
              for (int j = j_end; j < columnCount; ++j)
                result[i, j] = function(i, j, 0);
            }
          }
          break;

        default:
          throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Applies the specified function to each element of the matrix, with an additional source parameter, and with the option to specify a result matrix and treatment of zeros.
    /// </summary>
    /// <typeparam name="T1">The type of the additional source parameter.</typeparam>
    /// <param name="sourceParameter1">The additional source parameter.</param>
    /// <param name="function">The function to apply to each element.</param>
    /// <param name="result">The matrix to store the results in.</param>
    /// <param name="zeros">Specifies how to treat zero elements.</param>
    public void MapIndexed<T1>(T1 sourceParameter1, Func<int, int, double, T1, double> function, IMatrix<double> result, Zeros zeros = Zeros.AllowSkip)
    {
      int rowCount = _rowCount;
      int columnCount = _columnCount;

      if (zeros != Zeros.Include && !(result is IROBandMatrix<double> bm && bm.LowerBandwidth == _lowerBandwidth && bm.UpperBandwidth == _upperBandwidth))
      {
        MatrixMath.Clear(result);
      }

      switch (zeros)
      {
        case Zeros.AllowSkip:
          {
            for (int i = 0; i < rowCount; ++i)
            {
              var array_i = _array[i];
              int j_start = Math.Max(0, i - _lowerBandwidth);
              int j_end = Math.Min(columnCount, i + _upperBandwidth + 1);

              for (int j = j_start; j < j_end; ++j)
              {
                result[i, j] = function(i, j, array_i[j], sourceParameter1);
              }
            }
          }
          break;

        case Zeros.Include:
          {
            for (int i = 0; i < rowCount; ++i)
            {
              var array_i = _array[i];
              int j_start = Math.Max(0, i - _lowerBandwidth);
              int j_end = Math.Min(columnCount, i + _upperBandwidth + 1);
              for (int j = 0; j < j_start; ++j)
                result[i, j] = function(i, j, 0, sourceParameter1);
              for (int j = j_start; j < j_end; ++j)
                result[i, j] = function(i, j, array_i[j], sourceParameter1);
              for (int j = j_end; j < columnCount; ++j)
                result[i, j] = function(i, j, 0, sourceParameter1);
            }
          }
          break;

        default:
          throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Returns an enumerable collection of the elements in the matrix, with their row and column indices, and with the option to specify treatment of zeros.
    /// </summary>
    /// <param name="zeros">Specifies how to treat zero elements.</param>
    /// <returns>An enumerable collection of the elements in the matrix.</returns>
    public IEnumerable<(int row, int column, double value)> EnumerateElementsIndexed(Zeros zeros = Zeros.AllowSkip)
    {
      var rowCount = _rowCount;
      var colCount = _columnCount;
      int i, j;

      switch (zeros)
      {
        case Zeros.AllowSkip:
          {
            for (i = 0; i < rowCount; ++i)
            {
              var array_i = _array[i];
              int j_start = Math.Max(0, i - _lowerBandwidth);
              int j_end = Math.Min(colCount, i + _upperBandwidth + 1);
              for (j = j_start; j < j_end; ++j)
              {
                yield return (i, j, array_i[j]);
              }
            }
          }
          break;

        case Zeros.Include:
          {
            for (i = 0; i < rowCount; ++i)
            {
              var array_i = _array[i];
              int j_start = Math.Max(0, i - _lowerBandwidth);
              int j_end = Math.Min(colCount, i + _upperBandwidth + 1);
              for (j = 0; i < j_start; ++j)
                yield return (i, j, 0);
              for (j = j_start; j < j_end; ++j)
                yield return (i, j, array_i[j]);
              for (j = j_end; j < colCount; ++j)
                yield return (i, j, 0);
            }
          }
          break;

        default:
          throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Clears the matrix by setting all elements to zero.
    /// </summary>
    public void Clear()
    {
      for (int i = 0; i < _array.Length; ++i)
      {
        Array.Clear(_array[i], 0, _array[i].Length);
      }
    }

    /// <summary>
    /// Copies the values from another matrix to this matrix.
    /// </summary>
    /// <param name="from">The source matrix to copy values from.</param>
    public void CopyFrom(IROMatrix<double> from)
    {
      if (this.RowCount != from.RowCount || this.ColumnCount != from.ColumnCount)
        throw new ArgumentException("Dimensions do not match", nameof(from));

      if (from is BandDoubleMatrix bdm)
      {
        for (int i = 0; i < _array.Length; ++i)
        {
          Array.Copy(bdm._array[i], _array[i], bdm._array[i].Length);
        }
      }
      else
      {
        MatrixMath.Copy(from, this);
      }
    }
  }
}
