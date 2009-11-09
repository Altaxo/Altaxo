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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Altaxo.Collections;
using Altaxo.Gui.Worksheet.Viewing;
using Altaxo.Gui.Common;

namespace Altaxo.Data
{
  public static class RowCommands
  {

    /// <summary>
    /// Copies selected rows into newly created property columns and deletes the rows afterwards.
    /// </summary>
    /// <param name="table">Table which contains the data rows to change into property columns.</param>
    /// <param name="selectedDataRows">Indices of the rows to change.</param>
    /// <param name="selectedDataColumns">Indices of those columns which should be copies into the property column cells.</param>
    public static void ChangeRowsToPropertyColumns(this DataTable table, IAscendingIntegerCollection selectedDataRows, IAscendingIntegerCollection selectedDataColumns)
    {
      // first copy the rows into property columns
      foreach (var rowIdx in selectedDataRows)
      {
        var propColType = GetTypeOfColumnForRowReplacement(table.DataColumns, rowIdx, selectedDataColumns);
        var propCol = (DataColumn)Activator.CreateInstance(propColType);
        CopyRowToDataColumn(table.DataColumns, rowIdx, selectedDataColumns, propCol);
        table.PropCols.Add(propCol);
      }

      // now delete the rows
      table.DataColumns.RemoveRows(selectedDataRows);
    }

    /// <summary>
    /// Returns the type of column which is most appropriate for replacement of a data row.
    /// </summary>
    /// <param name="table">The table which contains the data row under investigation.</param>
    /// <param name="rowIdx">Index of the row under investigation.</param>
    /// <param name="selectedColumns">Collection of columns which is accounted for the determination.</param>
    /// <returns>The type of column which is most appropriate for replacement of a data row. This is the type which has the topmost non-empty cells.</returns>
    public static System.Type GetTypeOfColumnForRowReplacement(this DataColumnCollection table, int rowIdx, IAscendingIntegerCollection selectedColumns)
    {
      Dictionary<System.Type, int> typesToCount = new Dictionary<Type, int>();
      if (null == selectedColumns || 0==selectedColumns.Count)
        selectedColumns = new Altaxo.Collections.IntegerRangeAsCollection(0, table.ColumnCount);


    
      // count all non-empty cells according to the type
      foreach(var colIdx in selectedColumns)
        {
          DataColumn col = table[colIdx];
          System.Type colType = col.GetType();
          if (!col.IsElementEmpty(rowIdx)) // we only count non-empty cells
          {
            if (typesToCount.ContainsKey(col.GetType()))
              typesToCount[colType] = 1 + typesToCount[colType];
            else
              typesToCount.Add(colType, 1);
          }
        }
     
    

      // return the data column type with the topmost count
      int bestCount = 0;
      System.Type bestType = null;
      foreach (var entry in typesToCount)
      {
        if (entry.Value > bestCount)
        {
          bestCount = entry.Value;
          bestType = entry.Key;
        }
      }

      return bestType != null ? bestType : typeof(DoubleColumn);
    }

    /// <summary>
    /// Copies a specified row of the DataColumnCollection into another column.
    /// </summary>
    /// <param name="table">Table to copy from.</param>
    /// <param name="rowIdx">Index of the data row to copy.</param>
    /// <param name="selectedDataColumns">Columns to copy from. Can be null.</param>
    /// <param name="destinationColumn">DataColumn to copy the data to.</param>
    public static void CopyRowToDataColumn(this DataColumnCollection table, int rowIdx, IAscendingIntegerCollection selectedDataColumns, DataColumn destinationColumn)
    {
      if (null == selectedDataColumns || 0==selectedDataColumns.Count)
        selectedDataColumns = new Altaxo.Collections.IntegerRangeAsCollection(0, table.ColumnCount);

      foreach (var colIdx in selectedDataColumns)
      {
        destinationColumn[colIdx] = table[colIdx][rowIdx];
      }
    }
  }
}
