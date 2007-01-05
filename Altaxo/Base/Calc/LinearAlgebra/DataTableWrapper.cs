#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
  /// Wraps <see cref="Altaxo.Data.DataColumnCollection" />s to matrices.
  /// </summary>
  public class DataTableWrapper
  {

    #region Inner classes

    

    /// <summary>
    /// Wraps a set of <see cref="DataColumn" />s into a matrix so that the matrix columns corresponds to the <see cref="DataColumn" />s.
    /// </summary>
    class DataColumnToColumnROMatrixWrapper : IROMatrix
    {
      protected Altaxo.Data.INumericColumn[] _columns;
      protected Altaxo.Collections.IAscendingIntegerCollection _rows;

    
      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="collection">Collection of <see cref="DataColumn" />s.</param>
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
          return _columns[col][_rows[row]];
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
    /// Wraps a set of <see cref="DataColumn" />s into a matrix so that the matrix columns corresponds to the <see cref="DataColumn" />s. But the first column here consists of 1s (one), and the columns start with index 1.
    /// That means that the resulting matrix has a number of columns that is one larger that the number of data columns.
    /// </summary>
    class InterceptPlusDataColumnToColumnROMatrixWrapper : IROMatrix
    {
      protected Altaxo.Data.INumericColumn[] _columns;
      protected Altaxo.Collections.IAscendingIntegerCollection _rows;

    
      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="collection">Collection of <see cref="DataColumn" />s.</param>
      /// <param name="selectedColumns">Set set of indices into the collection that are part of the matrix.</param>
      /// <param name="selectedRows">The set of rows that are part of the matrix. This collection is not cloned here, therefore it must not be subsequently changed!</param>
      public InterceptPlusDataColumnToColumnROMatrixWrapper(Altaxo.Data.DataColumnCollection collection, Altaxo.Collections.IAscendingIntegerCollection selectedColumns, Altaxo.Collections.IAscendingIntegerCollection selectedRows)
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
          if(col==0)
            return 1;
          else return _columns[col-1][_rows[row]];
        }
      }

      /// <summary>
      /// Number of columns of the matrix.
      /// </summary>
      public int Columns
      {
        get
        {
          return _columns.Length+1;
        }
      }

      #endregion
    }

  
    /// <summary>
    /// Wraps a set of <see cref="DataColumn" />s into a matrix so that the matrix columns corresponds to the <see cref="DataColumn" />s.
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
      double IMatrix.this[int row, int col]
      {
        
        get
        {
          return _columns[col][_rows[row]];
        }
         
        set
        {
          ((IWriteableColumn)_columns[col])[_rows[row]] = value;
        }
      }


      #endregion
    }
  

    /// <summary>
    /// Wrapps a set of <see cref="DataColumn" />s into a matrix so that the matrix rows corresponds to the <see cref="DataColumn" />s.
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
          return _columns[row][_rows[col]];
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
    /// Wrapps a set of <see cref="DataColumn" />s into a matrix so that the matrix rows corresponds to the <see cref="DataColumn" />s.
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
      double IMatrix.this[int row, int col]
      {
        get
        {
          return _columns[row][_rows[col]];
        }
        set
        {
          ((IWriteableColumn)_columns[row])[_rows[col]] = value;
        }
      }
      #endregion
    }

    #endregion

    #region Readonly matrix wrapper functions

    /// <summary>
    /// Wraps a set of <see cref="DataColumn" />s into a readonly matrix so that the matrix columns corresponds to the <see cref="DataColumn" />s.
    /// </summary>
    /// <param name="collection">Collection of <see cref="DataColumn" />s.</param>
    /// <param name="selectedColumns">Set set of indices into the collection that are part of the matrix. You can subsequently change this parameter without affecting this wrapper.</param>
    /// <param name="selectedRows">The set of rows that are part of the matrix. This collection will be cloned here, i.e. you can subsequently change it without affecting this wrapper.</param>
    /// <returns>The wrapping read only matrix.</returns>
    public static IROMatrix ToROColumnMatrix(Altaxo.Data.DataColumnCollection collection, Altaxo.Collections.IAscendingIntegerCollection selectedColumns, Altaxo.Collections.IAscendingIntegerCollection selectedRows)
    {
      return new DataColumnToColumnROMatrixWrapper(collection, selectedColumns, (IAscendingIntegerCollection)selectedRows.Clone());
    }

    /// <summary>
    /// Wraps a set of <see cref="DataColumn" />s into a readonly matrix so that the matrix columns corresponds to the <see cref="DataColumn" />s But the first column consists of elements with a numerical value of 1. The number of columns
    /// of the resulting matrix is therefore 1 greater than the number of data columns in the argument.
    /// </summary>
    /// <param name="collection">Collection of <see cref="DataColumn" />s.</param>
    /// <param name="selectedColumns">Set set of indices into the collection that are part of the matrix. You can subsequently change this parameter without affecting this wrapper.</param>
    /// <param name="selectedRows">The set of rows that are part of the matrix. This collection will be cloned here, i.e. you can subsequently change it without affecting this wrapper.</param>
    /// <returns>The wrapping read only matrix.</returns>
    /// <remarks>This type of wrapper is usefull for instance for fitting purposes, where an intercept is needed.</remarks>
    public static IROMatrix ToROColumnMatrixWithIntercept(Altaxo.Data.DataColumnCollection collection, Altaxo.Collections.IAscendingIntegerCollection selectedColumns, Altaxo.Collections.IAscendingIntegerCollection selectedRows)
    {
      return new InterceptPlusDataColumnToColumnROMatrixWrapper(collection, selectedColumns, (IAscendingIntegerCollection)selectedRows.Clone());
    }

    /// <summary>
    /// Wraps a set of <see cref="DataColumn" />s into a readonly matrix so that the matrix columns corresponds to the <see cref="DataColumn" />s.
    /// </summary>
    /// <param name="collection">Collection of <see cref="DataColumn" />s.</param>
    /// <param name="selectedColumns">Set set of indices into the collection that are part of the matrix. You can subsequently change this parameter without affecting this wrapper.</param>
    /// <param name="nRows">The number of rows that are part of the matrix (starting from index 0).</param>
    public static IROMatrix ToROColumnMatrix(Altaxo.Data.DataColumnCollection collection, Altaxo.Collections.IAscendingIntegerCollection selectedColumns, int nRows)
    {
      return new DataColumnToColumnROMatrixWrapper(collection, selectedColumns, new Altaxo.Collections.IntegerRangeAsCollection(0,nRows));
    }




    /// <summary>
    /// Wraps a set of <see cref="DataColumn" />s into a readonly matrix so that the matrix rows corresponds to the <see cref="DataColumn" />s.
    /// </summary>
    /// <param name="collection">DataColumnCollection from which to select the data columns that are part of the matrix by their indices.</param>
    /// <param name="selectedColumns">The indices of the data columns in the collection that are part of the matrix. You can subsequently change this parameter without affecting this wrapper.</param>
    /// <param name="selectedRows">Selected rows of the data table that participate in the matrix. Remember that this are the columns of the wrapped matrix. This collection will be cloned here, i.e. you can subsequently change it without affecting this wrapper.</param>
    public static IROMatrix ToRORowMatrix(Altaxo.Data.DataColumnCollection collection, Altaxo.Collections.IAscendingIntegerCollection selectedColumns, Altaxo.Collections.IAscendingIntegerCollection selectedRows)
    {
      return new DataColumnToRowROMatrixWrapper(collection, selectedColumns, (IAscendingIntegerCollection)selectedRows.Clone());
    }

    /// <summary>
    /// Wrapps a set of <see cref="DataColumn" />s into a readonly matrix so that the matrix rows corresponds to the <see cref="DataColumn" />s.
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
    /// Wraps a set of <see cref="DataColumn" />s into a writeable matrix so that the matrix columns corresponds to the <see cref="DataColumn" />s.
    /// </summary>
    /// <param name="collection">Collection of <see cref="DataColumn" />s.</param>
    /// <param name="selectedColumns">Set set of indices into the collection that are part of the matrix. You can subsequently change this parameter without affecting this wrapper.</param>
    /// <param name="selectedRows">The set of rows that are part of the matrix. This collection will be cloned here, i.e. you can subsequently change it without affecting this wrapper.</param>
    public static IMatrix ToColumnMatrix(Altaxo.Data.DataColumnCollection collection, Altaxo.Collections.IAscendingIntegerCollection selectedColumns, Altaxo.Collections.IAscendingIntegerCollection selectedRows)
    {
      return new DataColumnToColumnMatrixWrapper(collection, selectedColumns, (IAscendingIntegerCollection)selectedRows.Clone());
    }

    /// <summary>
    /// Wraps a set of <see cref="DataColumn" />s into a writeable matrix so that the matrix columns corresponds to the <see cref="DataColumn" />s.
    /// </summary>
    /// <param name="collection">Collection of <see cref="DataColumn" />s.</param>
    /// <param name="selectedColumns">Set set of indices into the collection that are part of the matrix. You can subsequently change this parameter without affecting this wrapper.</param>
    /// <param name="nRows">The number of rows that are part of the matrix (starting from index 0).</param>
    public static IMatrix ToColumnMatrix(Altaxo.Data.DataColumnCollection collection, Altaxo.Collections.IAscendingIntegerCollection selectedColumns, int nRows)
    {
      return new DataColumnToColumnMatrixWrapper(collection, selectedColumns, new Altaxo.Collections.IntegerRangeAsCollection(0,nRows));
    }

    /// <summary>
    /// Wrapps a set of <see cref="DataColumn" />s into a writeable matrix so that the matrix rows corresponds to the <see cref="DataColumn" />s.
    /// </summary>
    /// <param name="collection">DataColumnCollection from which to select the data columns that are part of the matrix by their indices.</param>
    /// <param name="selectedColumns">The indices of the data columns in the collection that are part of the matrix. You can subsequently change this parameter without affecting this wrapper.</param>
    /// <param name="selectedRows">Selected rows of the data table that participate in the matrix. Remember that this are the columns of the wrapped matrix. This collection will be cloned here, i.e. you can subsequently change it without affecting this wrapper.</param>
    public static IMatrix ToRowMatrix(Altaxo.Data.DataColumnCollection collection, Altaxo.Collections.IAscendingIntegerCollection selectedColumns, Altaxo.Collections.IAscendingIntegerCollection selectedRows)
    {
      return new DataColumnToRowMatrixWrapper(collection, selectedColumns, (IAscendingIntegerCollection)selectedRows.Clone());
    }

    /// <summary>
    /// Wrapps a set of <see cref="DataColumn" />s into a writeable matrix so that the matrix rows corresponds to the <see cref="DataColumn" />s.
    /// </summary>
    /// <param name="collection">DataColumnCollection from which to select the data columns that are part of the matrix by their indices.</param>
    /// <param name="selectedColumns">The indices of the data columns in the collection that are part of the matrix. You can subsequently change this parameter without affecting this wrapper.</param>
    /// <param name="nRows">Number of rows of the data table that participate in the matrix (starting from index 0). Remember that this are the columns of the wrapped matrix.</param>
    public static IMatrix ToRowMatrix(Altaxo.Data.DataColumnCollection collection, Altaxo.Collections.IAscendingIntegerCollection selectedColumns, int nRows)
    {
      return new DataColumnToRowMatrixWrapper(collection, selectedColumns, new Altaxo.Collections.IntegerRangeAsCollection(0,nRows));
    }

    #endregion

    #region Valid numeric columns and rows helper functions


    /// <summary>
    /// Verifies that all selected columns are numeric and throws an exception if this is not the case.
    /// </summary>
    /// <param name="table">The data column collection with the columns.</param>
    /// <param name="selectedCols">The index collection of the columns in quest.</param>
    /// <param name="rowCount">Returns the minimum of the row count of all the selected columns. The value is 0 if there is at least one non-numeric column.</param>
    public static void VerifyAllColumnsNumeric(DataColumnCollection table, IAscendingIntegerCollection selectedCols, out int rowCount)
    {
      if(selectedCols.Count==0)
      {
        rowCount = 0;
        return;
      }

      rowCount = int.MaxValue;
      for(int i=0;i<selectedCols.Count;i++)
      {
        if(!(table[selectedCols[i]] is Altaxo.Data.INumericColumn))
          throw new ArgumentException(string.Format("The column \"{0}\" is not numeric!",table[selectedCols[i]].Name));

        rowCount = Math.Min(rowCount,table[selectedCols[i]].Count);
      }
    }

    /// <summary>
    /// Determines if all selected columns are numeric.
    /// </summary>
    /// <param name="table">The data column collection with the columns.</param>
    /// <param name="selectedCols">The index collection of the columns in quest.</param>
    /// <param name="rowCount">Returns the minimum of the row count of all the selected columns. The value is 0 if there is at least one non-numeric column.</param>
    /// <returns>True if all columns are numeric, false otherwise.</returns>
    public static bool AreAllColumnsNumeric(DataColumnCollection table, IAscendingIntegerCollection selectedCols, out int rowCount)
    {
      if(selectedCols.Count==0)
      {
        rowCount = 0;
        return true;
      }

      rowCount = int.MaxValue;
      for(int i=0;i<selectedCols.Count;i++)
      {
        if(!(table[selectedCols[i]] is Altaxo.Data.INumericColumn))
        {
          rowCount=0;
          return false;
        }

        rowCount = Math.Min(rowCount,table[selectedCols[i]].Count);
      }
      return true;
    }

    
    /// <summary>
    /// Determines which of the rows of a set of columns is truly numeric, i.e. all columns in this row contains a value, which is not double.NaN.
    /// </summary>
    /// <param name="table">The data column collection.</param>
    /// <param name="selectedCols">The indizes of the columns in question into the collection.</param>
    /// <param name="rowCount">The minimum row count of all the selected columns.</param>
    /// <returns>A boolean array. If an element of the array is true at a given index, that row contains valid numeric values in all columns.</returns>
    public static bool[] GetValidNumericRows(DataColumnCollection table, IAscendingIntegerCollection selectedCols, int rowCount)
    {
      // determine the number of valid rows
      bool[] rowValid = new bool[rowCount];
      for(int i=0;i<rowCount;i++)
        rowValid[i] = true;

      for(int i=0;i<selectedCols.Count;i++)
      {
        INumericColumn col = (INumericColumn)table[selectedCols[i]];
        for(int j=0;j<rowCount;j++)
        {
          if(double.IsNaN(col[j]))
            rowValid[j]=false;
        }
      }
      return rowValid;
    }

    /// <summary>
    /// Determines which of the rows of a set of columns is truly numeric, i.e. all columns in this row contains a value, which is not double.NaN.
    /// </summary>
    /// <param name="table">Array of numeric columns.</param>
    /// <param name="selectedCols">The indizes of the columns in question into the collection.</param>
    /// <param name="rowCount">The minimum row count of all the selected columns.</param>
    /// <returns>A boolean array. If an element of the array is true at a given index, that row contains valid numeric values in all columns.</returns>
    public static bool[] GetValidNumericRows(INumericColumn[] table, IAscendingIntegerCollection selectedCols, int rowCount)
    {
      // determine the number of valid rows
      bool[] rowValid = new bool[rowCount];
      for(int i=0;i<rowCount;i++)
        rowValid[i] = true;

      for(int i=0;i<selectedCols.Count;i++)
      {
        INumericColumn col = (INumericColumn)table[selectedCols[i]];
        for(int j=0;j<rowCount;j++)
        {
          if(double.IsNaN(col[j]))
            rowValid[j]=false;
        }
      }
      return rowValid;
    }
    
    /// <summary>
    /// Counts the number of valid rows from the array that is returned by for instance <see cref="GetValidNumericRows(INumericColumn[], IAscendingIntegerCollection, int)" />.
    /// </summary>
    /// <param name="array">The boolean array.</param>
    /// <returns>The number of valid numeric rows, i.e. the number of elements in the array which have the value of true.</returns>
    public static int GetNumberOfValidNumericRows(bool[] array)
    {
      // count the number of valid rows
      int numberOfValidRows = 0;
      for(int i=0;i<array.Length;i++)
      {
        if(array[i])
          numberOfValidRows++;
      }
      return numberOfValidRows;
    }
    

    /// <summary>
    /// Gets the collection of valid rows from the array that is returned by <see cref="GetValidNumericRows(INumericColumn[], IAscendingIntegerCollection, int)" />.
    /// </summary>
    /// <param name="array">The boolean array.</param>
    /// <returns>An collection of ascending integer values. These values are the indizes of valid numeric rows, i.e. the number of elements in the array which have the value of true.</returns>
    public static AscendingIntegerCollection GetCollectionOfValidNumericRows(bool[] array)
    {
      AscendingIntegerCollection result = new AscendingIntegerCollection();
      for(int i=0;i<array.Length;i++)
      {
        if(array[i])
          result.Add(i);
      }
      return result;
    }

    /// <summary>
    /// Gets the collection of valid rows from selected columns of a <see cref="DataColumnCollection" />.
    /// </summary>
    /// <param name="table">The collection of data columns.</param>
    /// <param name="selectedCols">The selected columns. An exception is thrown if one of these columns is non numeric.</param>
    /// <returns>An collection of ascending integer values. These values are the indizes of valid numeric rows, i.e. the number of elements in the array which have the value of true.</returns>
    public static AscendingIntegerCollection GetCollectionOfValidNumericRows(DataColumnCollection table, IAscendingIntegerCollection selectedCols)
    {
      int rowCount;
      VerifyAllColumnsNumeric(table,selectedCols,out rowCount);
      
      return GetCollectionOfValidNumericRows(GetValidNumericRows(table,selectedCols,rowCount));
    }

    #endregion

  }




 
}
