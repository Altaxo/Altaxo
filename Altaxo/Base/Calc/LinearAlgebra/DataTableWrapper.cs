#region Copyright
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
#endregion

using System;
using Altaxo.Calc;
using Altaxo.Data;
using Altaxo.Collections;

namespace Altaxo.Calc.LinearAlgebra
{

  /// <summary>
  /// Wraps <see>Altaxo.Data.DataColumnCollection</see>s to matrices.
  /// </summary>
  public class DataTableWrapper
  {

    #region Inner classes

    

    /// <summary>
    /// Wraps a set of <see>DataColumns</see> into a matrix so that the matrix columns corresponds to the <see>DataColumns</see>.
    /// </summary>
    class DataColumnToColumnROMatrixWrapper : IROMatrix
    {
      protected Altaxo.Data.INumericColumn[] _columns;
      protected Altaxo.Collections.IAscendingIntegerCollection _rows;

    
      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="collection">Collection of <see>DataColumns</see>.</param>
      /// <param name="selectedColumns">Set set of indices into the collection that are part of the matrix.</param>
      /// <param name="selectedRows">The set of rows that are part of the matrix. This collection is not cloned here, therefore it must not be subsequently changed!</param>
      public DataColumnToColumnROMatrixWrapper(Altaxo.Data.DataColumnCollection collection, Altaxo.Collections.IAscendingIntegerCollection selectedColumns, Altaxo.Collections.IAscendingIntegerCollection selectedRows)
      {
        _columns = new Altaxo.Data.INumericColumn[selectedColumns.Count];
        for(int i=selectedColumns.Count-1;i>=0;i--)
          _columns[i]=(Altaxo.Data.INumericColumn)collection[selectedColumns[i]];

        _rows = selectedRows;
      }
      #region IROMatrix Members

      /// <summary>
      /// Number of rows of the matrix.
      /// </summary>
      public int Rows
      {
        get
        {
          return _rows.Count;
        }
      }

      /// <summary>
      /// Element accessor.
      /// </summary>
      public double this[int row, int col]
      {
        get
        {
          return _columns[col].GetDoubleAt(_rows[row]);
        }
      }

      /// <summary>
      /// Number of columns of the matrix.
      /// </summary>
      public int Columns
      {
        get
        {
          return _columns.Length;
        }
      }

      #endregion
    }

  
    /// <summary>
    /// Wraps a set of <see>DataColumns</see> into a matrix so that the matrix columns corresponds to the <see>DataColumns</see>.
    /// </summary>
    class DataColumnToColumnMatrixWrapper : DataColumnToColumnROMatrixWrapper, IMatrix
    {
      public DataColumnToColumnMatrixWrapper(Altaxo.Data.DataColumnCollection collection, Altaxo.Collections.IAscendingIntegerCollection selectedColumns, Altaxo.Collections.IAscendingIntegerCollection selectedRows)
        : base(collection,selectedColumns,selectedRows)
      {
        /*
        // check the writeability
        for(int i=selectedColumns.Count-1;i>=0;i--)
          if(!(collection[selectedColumns[i]] is IWriteableColumn))
            throw new ArgumentException(string.Format("Column not writeable! Index in matrix: {0}, index in data column collection: {1}, column name: {2}",i,selectedColumns[i],collection[selectedColumns[i]].Name));
        */
        }

      #region IMatrix Members

      /// <summary>
      /// Element accessor.
      /// </summary>
      public double this[int row, int col]
      {
        get
        {
          return _columns[col].GetDoubleAt(_rows[row]);
        }
        set
        {
          ((IWriteableColumn)_columns[col])[_rows[row]] = value;
        }
      }


      #endregion
    }
  

    /// <summary>
    /// Wrapps a set of <see>DataColumns</see> into a matrix so that the matrix rows corresponds to the <see>DataColumns</see>.
    /// </summary>
    class DataColumnToRowROMatrixWrapper : IROMatrix
    {
      protected Altaxo.Data.INumericColumn[] _columns;
      protected Altaxo.Collections.IAscendingIntegerCollection _rows;


      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="collection">DataColumnCollection from which to select the data columns that are part of the matrix by their indices.</param>
      /// <param name="selectedColumns">The indices of the data columns in the collection that are part of the matrix</param>
      /// <param name="selectedRows">Selected rows of the data table that participate in the matrix. Remember that this are the columns of the wrapped matrix. This collection is not cloned here, therefore it must not be subsequently changed!</param>
      public DataColumnToRowROMatrixWrapper(Altaxo.Data.DataColumnCollection collection, Altaxo.Collections.IAscendingIntegerCollection selectedColumns, Altaxo.Collections.IAscendingIntegerCollection selectedRows)
      {
        _columns = new Altaxo.Data.INumericColumn[selectedColumns.Count];
        for(int i=selectedColumns.Count-1;i>=0;i--)
          _columns[i]=(Altaxo.Data.INumericColumn)collection[selectedColumns[i]];

        _rows = selectedRows;
      }
      #region IROMatrix Members

      /// <summary>
      /// Number of rows of the matrix.
      /// </summary>
      public int Rows
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
          return _columns[row].GetDoubleAt(_rows[col]);
        }
      }

      /// <summary>
      /// Number of columns of the matrix.
      /// </summary>
      public int Columns
      {
        get
        {
          return _rows.Count;
        }
      }

      #endregion
    }


    /// <summary>
    /// Wrapps a set of <see>DataColumns</see> into a matrix so that the matrix rows corresponds to the <see>DataColumns</see>.
    /// </summary>
    class DataColumnToRowMatrixWrapper : DataColumnToRowROMatrixWrapper, IMatrix
    {
      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="collection">DataColumnCollection from which to select the data columns that are part of the matrix by their indices.</param>
      /// <param name="selectedColumns">The indices of the data columns in the collection that are part of the matrix</param>
      /// <param name="selectedRows">Selected rows of the data table that participate in the matrix. Remember that this are the columns of the wrapped matrix. This collection is not cloned here, therefore it must not be subsequently changed!</param>
      public DataColumnToRowMatrixWrapper(Altaxo.Data.DataColumnCollection collection, Altaxo.Collections.IAscendingIntegerCollection selectedColumns, Altaxo.Collections.IAscendingIntegerCollection selectedRows)
        : base(collection,selectedColumns,selectedRows)
      {
      }

      #region IMatrix member
      /// <summary>
      /// Element accessor.
      /// </summary>
      public double this[int row, int col]
      {
        get
        {
          return _columns[row].GetDoubleAt(_rows[col]);
        }
        set
        {
          ((IWriteableColumn)_columns[row])[_rows[col]] = value;
        }
      }
      #endregion
    }

    #endregion

    #region readonly matrix wrapper functions

    /// <summary>
    /// Wraps a set of <see>DataColumns</see> into a readonly matrix so that the matrix columns corresponds to the <see>DataColumns</see>.
    /// </summary>
    /// <param name="collection">Collection of <see>DataColumns</see>.</param>
    /// <param name="selectedColumns">Set set of indices into the collection that are part of the matrix. You can subsequently change this parameter without affecting this wrapper.</param>
    /// <param name="selectedRows">The set of rows that are part of the matrix. This collection will be cloned here, i.e. you can subsequently change it without affecting this wrapper.</param>
    public static IROMatrix ToROColumnMatrix(Altaxo.Data.DataColumnCollection collection, Altaxo.Collections.IAscendingIntegerCollection selectedColumns, Altaxo.Collections.IAscendingIntegerCollection selectedRows)
    {
      return new DataColumnToColumnROMatrixWrapper(collection, selectedColumns, (IAscendingIntegerCollection)selectedRows.Clone());
    }

    /// <summary>
    /// Wraps a set of <see>DataColumns</see> into a readonly matrix so that the matrix columns corresponds to the <see>DataColumns</see>.
    /// </summary>
    /// <param name="collection">Collection of <see>DataColumns</see>.</param>
    /// <param name="selectedColumns">Set set of indices into the collection that are part of the matrix. You can subsequently change this parameter without affecting this wrapper.</param>
    /// <param name="nRows">The number of rows that are part of the matrix (starting from index 0).</param>
    public static IROMatrix ToROColumnMatrix(Altaxo.Data.DataColumnCollection collection, Altaxo.Collections.IAscendingIntegerCollection selectedColumns, int nRows)
    {
      return new DataColumnToColumnROMatrixWrapper(collection, selectedColumns, new Altaxo.Collections.IntegerRangeAsCollection(0,nRows));
    }

    /// <summary>
    /// Wrapps a set of <see>DataColumns</see> into a readonly matrix so that the matrix rows corresponds to the <see>DataColumns</see>.
    /// </summary>
    /// <param name="collection">DataColumnCollection from which to select the data columns that are part of the matrix by their indices.</param>
    /// <param name="selectedColumns">The indices of the data columns in the collection that are part of the matrix. You can subsequently change this parameter without affecting this wrapper.</param>
    /// <param name="selectedRows">Selected rows of the data table that participate in the matrix. Remember that this are the columns of the wrapped matrix. This collection will be cloned here, i.e. you can subsequently change it without affecting this wrapper.</param>
    public static IROMatrix ToRORowMatrix(Altaxo.Data.DataColumnCollection collection, Altaxo.Collections.IAscendingIntegerCollection selectedColumns, Altaxo.Collections.IAscendingIntegerCollection selectedRows)
    {
      return new DataColumnToRowROMatrixWrapper(collection, selectedColumns, (IAscendingIntegerCollection)selectedRows.Clone());
    }

    /// <summary>
    /// Wrapps a set of <see>DataColumns</see> into a readonly matrix so that the matrix rows corresponds to the <see>DataColumns</see>.
    /// </summary>
    /// <param name="collection">DataColumnCollection from which to select the data columns that are part of the matrix by their indices.</param>
    /// <param name="selectedColumns">The indices of the data columns in the collection that are part of the matrix. You can subsequently change this parameter without affecting this wrapper.</param>
    /// <param name="nRows">Number of rows of the data table that participate in the matrix (starting from index 0). Remember that this are the columns of the wrapped matrix.</param>
    public static IROMatrix ToRORowMatrix(Altaxo.Data.DataColumnCollection collection, Altaxo.Collections.IAscendingIntegerCollection selectedColumns, int nRows)
    {
      return new DataColumnToRowROMatrixWrapper(collection, selectedColumns, new Altaxo.Collections.IntegerRangeAsCollection(0,nRows));
    }

    #endregion

    #region Writeable matrix wrapper functions
    /// <summary>
    /// Wraps a set of <see>DataColumns</see> into a writeable matrix so that the matrix columns corresponds to the <see>DataColumns</see>.
    /// </summary>
    /// <param name="collection">Collection of <see>DataColumns</see>.</param>
    /// <param name="selectedColumns">Set set of indices into the collection that are part of the matrix. You can subsequently change this parameter without affecting this wrapper.</param>
    /// <param name="selectedRows">The set of rows that are part of the matrix. This collection will be cloned here, i.e. you can subsequently change it without affecting this wrapper.</param>
    public static IMatrix ToColumnMatrix(Altaxo.Data.DataColumnCollection collection, Altaxo.Collections.IAscendingIntegerCollection selectedColumns, Altaxo.Collections.IAscendingIntegerCollection selectedRows)
    {
      return new DataColumnToColumnMatrixWrapper(collection, selectedColumns, (IAscendingIntegerCollection)selectedRows.Clone());
    }

    /// <summary>
    /// Wraps a set of <see>DataColumns</see> into a writeable matrix so that the matrix columns corresponds to the <see>DataColumns</see>.
    /// </summary>
    /// <param name="collection">Collection of <see>DataColumns</see>.</param>
    /// <param name="selectedColumns">Set set of indices into the collection that are part of the matrix. You can subsequently change this parameter without affecting this wrapper.</param>
    /// <param name="nRows">The number of rows that are part of the matrix (starting from index 0).</param>
    public static IMatrix ToColumnMatrix(Altaxo.Data.DataColumnCollection collection, Altaxo.Collections.IAscendingIntegerCollection selectedColumns, int nRows)
    {
      return new DataColumnToColumnMatrixWrapper(collection, selectedColumns, new Altaxo.Collections.IntegerRangeAsCollection(0,nRows));
    }

    /// <summary>
    /// Wrapps a set of <see>DataColumns</see> into a writeable matrix so that the matrix rows corresponds to the <see>DataColumns</see>.
    /// </summary>
    /// <param name="collection">DataColumnCollection from which to select the data columns that are part of the matrix by their indices.</param>
    /// <param name="selectedColumns">The indices of the data columns in the collection that are part of the matrix. You can subsequently change this parameter without affecting this wrapper.</param>
    /// <param name="selectedRows">Selected rows of the data table that participate in the matrix. Remember that this are the columns of the wrapped matrix. This collection will be cloned here, i.e. you can subsequently change it without affecting this wrapper.</param>
    public static IMatrix ToRowMatrix(Altaxo.Data.DataColumnCollection collection, Altaxo.Collections.IAscendingIntegerCollection selectedColumns, Altaxo.Collections.IAscendingIntegerCollection selectedRows)
    {
      return new DataColumnToRowMatrixWrapper(collection, selectedColumns, (IAscendingIntegerCollection)selectedRows.Clone());
    }

    /// <summary>
    /// Wrapps a set of <see>DataColumns</see> into a writeable matrix so that the matrix rows corresponds to the <see>DataColumns</see>.
    /// </summary>
    /// <param name="collection">DataColumnCollection from which to select the data columns that are part of the matrix by their indices.</param>
    /// <param name="selectedColumns">The indices of the data columns in the collection that are part of the matrix. You can subsequently change this parameter without affecting this wrapper.</param>
    /// <param name="nRows">Number of rows of the data table that participate in the matrix (starting from index 0). Remember that this are the columns of the wrapped matrix.</param>
    public static IMatrix ToRowMatrix(Altaxo.Data.DataColumnCollection collection, Altaxo.Collections.IAscendingIntegerCollection selectedColumns, int nRows)
    {
      return new DataColumnToRowMatrixWrapper(collection, selectedColumns, new Altaxo.Collections.IntegerRangeAsCollection(0,nRows));
    }

#endregion


  }
  
}
