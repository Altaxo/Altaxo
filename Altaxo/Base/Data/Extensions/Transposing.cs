#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2019 Dr. Dirk Lellinger
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
using Altaxo.Collections;

namespace Altaxo.Data
{

  /// <summary>
  /// Contains methods to transpose a worksheet.
  /// </summary>
  public static class Transposing
  {
    /// <summary>
    /// Tests if the transpose of a table is possible.
    /// </summary>
    /// <param name="table">Table to test.</param>
    /// <param name="numConvertedDataColumns">Number of data columns (beginning from index 0) that will be converted to property columns.</param>
    /// <param name="indexOfProblematicColumn">On return, if transpose is not possible, will give the index of the first column which differs in type from the first transposed data column.</param>
    /// <returns>True when the transpose is possible without problems, false otherwise.</returns>
    public static bool TransposeIsPossible(this DataTable table, int numConvertedDataColumns, out int indexOfProblematicColumn)
    {
      if (numConvertedDataColumns < 0)
        throw new ArgumentOutOfRangeException("numConvertedDataColumns is less than zero");

      indexOfProblematicColumn = 0;
      if (numConvertedDataColumns >= table.DataColumnCount)
        return true; // when all columns convert to property columns, that will be no problem

      System.Type masterColumnType = table[numConvertedDataColumns].GetType();

      for (int i = numConvertedDataColumns + 1; i < table.DataColumnCount; i++)
      {
        if (table[i].GetType() != masterColumnType)
        {
          indexOfProblematicColumn = i;
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Tests whether or not all columns in this collection have the same type.
    /// </summary>
    /// <param name="col">The column collection containing the columns to test.</param>
    /// <param name="selectedColumnIndices">The column indices of the columns to test.</param>
    /// <param name="firstdifferentcolumnindex">Out: returns the first column that has a different type from the first column</param>.
    /// <returns>True if all selected columns are of the same type. True is also returned if the number of selected columns is zero.</returns>
    public static bool AreAllColumnsOfTheSameType(DataColumnCollection col, IContiguousIntegerRange selectedColumnIndices, out int firstdifferentcolumnindex)
    {
      firstdifferentcolumnindex = 0;

      if (0 == col.ColumnCount || 0 == selectedColumnIndices.Count)
        return true;

      System.Type firstType = col[selectedColumnIndices[0]].GetType();

      int len = selectedColumnIndices.Count;
      for (int i = 0; i < len; i++)
      {
        if (col[selectedColumnIndices[i]].GetType() != firstType)
        {
          firstdifferentcolumnindex = selectedColumnIndices[i];
          return false;
        }
      }

      return true;
    }

    /// <summary>
    /// Transpose transpose the table, i.e. exchange columns and rows
    /// this can only work if all columns in the table are of the same type
    /// </summary>
    /// <param name="srcTable">Table to transpose.</param>
    /// <param name="options">Options that control the transpose process.</param>
    /// <param name="destTable">Table in which the transposed table should be stored.</param>
    /// <exception cref="ArgumentNullException">
    /// </exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException">The data columns to transpose are not of the same type. The first column that has a deviating type is column number  + firstDifferentColumnIndex.ToString()</exception>
    public static void Transpose(this DataTable srcTable, DataTableTransposeOptions options, DataTable destTable)
    {
      if (srcTable is null)
        throw new ArgumentNullException(nameof(srcTable));
      if (destTable is null)
        throw new ArgumentNullException(nameof(destTable));
      if (object.ReferenceEquals(srcTable, destTable))
        throw new ArgumentException($"{nameof(srcTable)} and {nameof(destTable)} are identical. This inline transpose operation is not supported.");

      int numberOfDataColumnsChangeToPropertyColumns = Math.Min(options.DataColumnsMoveToPropertyColumns, srcTable.DataColumnCount);

      int numberOfPropertyColumnsChangeToDataColumns = Math.Min(options.PropertyColumnsMoveToDataColumns, srcTable.PropertyColumnCount);

      // number of data columns in the destination table that originates either from converted property columns or from the label column which contains the column names
      int numberOfPriorDestDataColumns = numberOfPropertyColumnsChangeToDataColumns + (options.StoreDataColumnNamesInFirstDataColumn ? 1 : 0);

      var dataColumnsToTransposeIndices = ContiguousIntegerRange.FromStartAndEndExclusive(numberOfDataColumnsChangeToPropertyColumns, srcTable.DataColumnCount);

      if (!AreAllColumnsOfTheSameType(srcTable.DataColumns, dataColumnsToTransposeIndices, out var firstDifferentColumnIndex))
      {
        throw new InvalidOperationException("The data columns to transpose are not of the same type. The first column that has a deviating type is column number " + firstDifferentColumnIndex.ToString());
      }

      using (var suspendToken = destTable.SuspendGetToken())
      {
        destTable.DataColumns.ClearData();
        destTable.PropCols.ClearData();

        // 0th, store the data column names in the first column
        if (options.StoreDataColumnNamesInFirstDataColumn)
        {
          var destCol = destTable.DataColumns.EnsureExistenceAtPositionStrictly(0, "DataColumnNames", typeof(TextColumn), ColumnKind.Label, 0);
          for (int j = numberOfDataColumnsChangeToPropertyColumns, k = 0; j < srcTable.DataColumnCount; ++j, ++k)
            destCol[k] = srcTable.DataColumns.GetColumnName(j);
        }

        int numberOfExtraPriorDestColumns = (options.StoreDataColumnNamesInFirstDataColumn ? 1 : 0);

        // 1st, copy the property columns to data columns
        for (int i = 0; i < numberOfPropertyColumnsChangeToDataColumns; ++i)
        {
          var destCol = destTable.DataColumns.EnsureExistenceAtPositionStrictly(i + numberOfExtraPriorDestColumns, srcTable.PropertyColumns.GetColumnName(i), srcTable.PropertyColumns[i].GetType(), srcTable.PropertyColumns.GetColumnKind(i), srcTable.PropertyColumns.GetColumnGroup(i));
          var srcCol = srcTable.PropertyColumns[i];
          for (int j = numberOfDataColumnsChangeToPropertyColumns, k = 0; j < srcCol.Count; ++j, ++k)
            destCol[k] = srcCol[j];
        }

        // 2rd, transpose the data columns
        int srcRows = 0;
        foreach (int i in dataColumnsToTransposeIndices)
          srcRows = Math.Max(srcRows, srcTable.DataColumns[i].Count);

        // create as many columns in destTable as srcRows and fill them with data
        Type? columnType = dataColumnsToTransposeIndices.Count > 0 ? srcTable.DataColumns[dataColumnsToTransposeIndices[0]].GetType() : null;
        for (int i = 0; i < srcRows; ++i)
        {
          string destColName = string.Format("{0}{1}", options.ColumnNamingPreString, i);
          if (options.UseFirstDataColumnForColumnNaming)
          {
            destColName = string.Format("{0}{1}", options.ColumnNamingPreString, srcTable.DataColumns[0][i]);
          }

          var destCol = destTable.DataColumns.EnsureExistenceAtPositionStrictly(numberOfPriorDestDataColumns + i, destColName, false, columnType!, ColumnKind.V, 0);
          int k = 0;
          foreach (int j in dataColumnsToTransposeIndices)
            destCol[k++] = srcTable.DataColumns[j][i];
        }

        // 3rd, copy the first data columns to property columns
        for (int i = 0; i < numberOfDataColumnsChangeToPropertyColumns; ++i)
        {
          var destCol = destTable.PropertyColumns.EnsureExistenceAtPositionStrictly(i, srcTable.DataColumns.GetColumnName(i), srcTable.DataColumns[i].GetType(), srcTable.DataColumns.GetColumnKind(i), srcTable.DataColumns.GetColumnGroup(i));
          var srcCol = srcTable.DataColumns[i];
          for (int j = numberOfPriorDestDataColumns, k = 0; k < srcCol.Count; ++j, ++k)
            destCol[j] = srcCol[k];
        }

        // 4th, fill the rest of the property columns with the rest of the data columns
        for (int i = 0; i < numberOfDataColumnsChangeToPropertyColumns; ++i)
        {
          for (int j = 0; j < numberOfPropertyColumnsChangeToDataColumns; ++j)
          {
            try
            {
              destTable.PropertyColumns[i][j + numberOfExtraPriorDestColumns] = srcTable.PropertyColumns[j][i];
            }
            catch { }
          }
        }

        // and 5th, copy the remaining property columns to property columns
        for (int i = numberOfPropertyColumnsChangeToDataColumns, j = numberOfDataColumnsChangeToPropertyColumns; i < srcTable.PropertyColumns.ColumnCount; ++i, ++j)
        {
          var destCol = destTable.PropertyColumns.EnsureExistenceAtPositionStrictly(j, srcTable.PropertyColumns.GetColumnName(i), false, srcTable.PropertyColumns[i].GetType(), srcTable.PropertyColumns.GetColumnKind(i), srcTable.DataColumns.GetColumnGroup(i));
          destCol.Data = srcTable.PropertyColumns[i];
        }

        suspendToken.Resume();
      }
    }
  }
}
