﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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

#nullable enable
using System;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.LinearAlgebra
{
  /// <summary>
  /// Wrapps a set of <see>DataColumns</see> into a matrix so that the matrix columns corresponds to the <see>DataColumns</see>.
  /// </summary>
  internal class DataColumnToColumnMatrixWrapper : IROMatrix<double>
  {
    private Altaxo.Data.INumericColumn[] _columns;
    private int _rows;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="collection">Collection of <see>DataColumns</see>.</param>
    /// <param name="selectedColumns">Set set of indices into the collection that are part of the matrix.</param>
    /// <param name="nRows">The number of rows that are part of the matrix. (Starting from index 0).</param>
    public DataColumnToColumnMatrixWrapper(Altaxo.Data.DataColumnCollection collection, Altaxo.Collections.IAscendingIntegerCollection selectedColumns, int nRows)
    {
      _columns = new Altaxo.Data.INumericColumn[selectedColumns.Count];
      for (int i = selectedColumns.Count - 1; i >= 0; i--)
        _columns[i] = (Altaxo.Data.INumericColumn)collection[selectedColumns[i]];

      _rows = nRows;
    }

    #region IROMatrix Members

    /// <summary>
    /// Number of rows of the matrix.
    /// </summary>
    public int RowCount
    {
      get
      {
        return _rows;
      }
    }

    /// <summary>
    /// Element accessor.
    /// </summary>
    public double this[int row, int col]
    {
      get
      {
        return _columns[col][row];
      }
    }

    /// <summary>
    /// Number of columns of the matrix.
    /// </summary>
    public int ColumnCount
    {
      get
      {
        return _columns.Length;
      }
    }

    #endregion IROMatrix Members
  }

  /// <summary>
  /// Wrapps a set of <see>DataColumns</see> into a matrix so that the matrix rows corresponds to the <see>DataColumns</see>.
  /// </summary>
  internal class DataColumnToRowMatrixWrapper : IROMatrix<double>
  {
    private Altaxo.Data.INumericColumn[] _columns;
    private int _nColumns;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="collection">DataColumnCollection from which to select the data columns that are part of the matrix by their indices.</param>
    /// <param name="selectedColumns">The indices of the data columns in the collection that are part of the matrix</param>
    /// <param name="nColumns">The number of columns of the matrix. This parameter is equivalent to the number of rows of the data columns that are part of the matrix.</param>
    public DataColumnToRowMatrixWrapper(Altaxo.Data.DataColumnCollection collection, Altaxo.Collections.IAscendingIntegerCollection selectedColumns, int nColumns)
    {
      _columns = new Altaxo.Data.INumericColumn[selectedColumns.Count];
      for (int i = selectedColumns.Count - 1; i >= 0; i--)
        _columns[i] = (Altaxo.Data.INumericColumn)collection[selectedColumns[i]];

      _nColumns = nColumns;
    }

    #region IROMatrix Members

    /// <summary>
    /// Number of rows of the matrix.
    /// </summary>
    public int RowCount
    {
      get
      {
        return _columns.Length;
      }
    }

    /// <summary>
    /// Element accessor.
    /// </summary>
    public double this[int row, int col]
    {
      get
      {
        return _columns[row][col];
      }
    }

    /// <summary>
    /// Number of columns of the matrix.
    /// </summary>
    public int ColumnCount
    {
      get
      {
        return _nColumns;
      }
    }

    #endregion IROMatrix Members
  }
}
