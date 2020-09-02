#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
//    along with ctrl program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

#nullable enable
using System;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Gui.Worksheet.Viewing;

namespace Altaxo.Worksheet.Commands
{
  /// <summary>
  /// Summary description for EditCommands.
  /// </summary>
  public class EditCommands
  {
    /// <summary>
    /// Creates a matrix from three selected columns. This must be a x-column, a y-column, and a value column.
    /// </summary>
    /// <param name="ctrl">Controller where the columns are selected in.</param>
    /// <returns>Null if no error occurs, or an error message.</returns>
    public static string? XYVToMatrix(IWorksheetController ctrl)
    {
      return ConvertXYVToMatrixActions.DoMakeActionWithoutDialog(ctrl.DataTable, ctrl.SelectedDataColumns);
    }

    /// <summary>
    /// Remove the selected columns, rows or property columns.
    /// </summary>
    public static void RemoveSelected(IWorksheetController ctrl)
    {
      using (var suspendToken = ctrl.DataTable.SuspendGetToken())
      {
        // Property columns are only deleted, if selected alone or in conjunction with data row selection
        if (ctrl.SelectedPropertyColumns.Count > 0 && ctrl.SelectedPropertyRows.Count == 0 && ctrl.SelectedDataColumns.Count == 0)
        {
          ctrl.DataTable.PropCols.RemoveColumns(ctrl.SelectedPropertyColumns);
          ctrl.SelectedPropertyColumns.Clear();
          ctrl.SelectedPropertyRows.Clear();
        }
        // note here: Property rows are only removed indirect by removing data columns

        // delete the selected columns if there are _only selected columns
        if (ctrl.SelectedDataColumns.Count > 0 && ctrl.SelectedDataRows.Count == 0)
        {
          ctrl.DataTable.RemoveColumns(ctrl.SelectedDataColumns);
          ctrl.SelectedDataColumns.Clear(); // now the columns are deleted, so they cannot be selected
        }

        // if rows are selected, remove them in all selected columns or in all columns (if no column selection=
        if (ctrl.SelectedDataRows.Count > 0)
        {
          ctrl.DataTable.DataColumns.RemoveRowsInColumns(
            ctrl.SelectedDataColumns.Count > 0 ? (IAscendingIntegerCollection)ctrl.SelectedDataColumns : ContiguousIntegerRange.FromStartAndCount(0, ctrl.DataTable.DataColumns.ColumnCount),
            ctrl.SelectedDataRows);

          ctrl.SelectedDataColumns.Clear();
          ctrl.SelectedDataRows.Clear();
        }

        // end code for the selected rows
        suspendToken.Dispose();
      }

      ctrl.TableAreaInvalidate(); // necessary because we changed the selections
    }

    /// <summary>
    /// Remove all but the selected columns, rows or property columns.
    /// </summary>
    public static void RemoveAllButSelected(IWorksheetController ctrl)
    {
      using (var suspendToken = ctrl.DataTable.SuspendGetToken())
      {
        // Property columns are only deleted, if selected alone or in conjunction with data row selection
        if (ctrl.SelectedPropertyColumns.Count > 0 && ctrl.SelectedPropertyRows.Count == 0 && ctrl.SelectedDataColumns.Count == 0)
        {
          for (int i = ctrl.DataTable.PropertyColumnCount - 1; i >= 0; i--)
          {
            if (!ctrl.SelectedPropertyColumns.Contains(i))
              ctrl.DataTable.PropertyColumns.RemoveColumn(i);
          }

          ctrl.SelectedPropertyColumns.Clear();
          ctrl.SelectedPropertyRows.Clear();
        }
        // note here: Property rows are only removed indirect by removing data columns

        // delete the selected columns if there are _only selected columns
        if (ctrl.SelectedDataColumns.Count > 0)
        {
          for (int i = ctrl.DataTable.DataColumnCount - 1; i >= 0; i--)
          {
            if (!ctrl.SelectedDataColumns.Contains(i))
              ctrl.DataTable.RemoveColumns(i, 1);
          }
          ctrl.SelectedDataColumns.Clear(); // now the columns are deleted, so they cannot be selected
        }

        // if rows are selected, remove them in all selected columns or in all columns (if no column selection=
        if (ctrl.SelectedDataRows.Count > 0)
        {
          for (int i = ctrl.DataTable.DataRowCount - 1; i >= 0; i--)
          {
            if (!ctrl.SelectedDataRows.Contains(i))
              ctrl.DataTable.DataColumns.RemoveRow(i);
          }

          ctrl.SelectedDataColumns.Clear();
          ctrl.SelectedDataRows.Clear();
        }

        // end code for the selected rows
        suspendToken.Dispose();
      }
      ctrl.TableAreaInvalidate(); // necessary because we changed the selections
    }

    /// <summary>
    /// This commands clean all selected cells.
    /// </summary>
    /// <param name="ctrl">The worksheet controller.</param>
    public static void CleanSelected(IWorksheetController ctrl)
    {
      using (var suspendToken = ctrl.DataTable.SuspendGetToken())
      {
        // clear whole columns when no specific
        if (ctrl.SelectedPropertyColumns.Count > 0)
        {
          if (ctrl.SelectedPropertyRows.Count == 0) // if only data columns but not rows selected, we can clean the data columns complete
          {
            foreach (int colidx in ctrl.SelectedPropertyColumns)
              ctrl.DataTable.PropertyColumns[colidx].Clear();
          }
          else // if property columns and also rows are selected, we have to clean the cells individually
          {
            foreach (int colidx in ctrl.SelectedPropertyColumns)
            {
              DataColumn col = ctrl.DataTable.PropertyColumns[colidx];
              foreach (int rowidx in ctrl.SelectedPropertyRows)
                col.SetElementEmpty(rowidx);
            }
          }
        }
        else if (ctrl.SelectedPropertyRows.Count > 0) // if only rows are selected, clean them complete
        {
          for (int colidx = ctrl.DataTable.PropertyColumnCount - 1; colidx >= 0; colidx--)
          {
            DataColumn col = ctrl.DataTable.PropertyColumns[colidx];
            foreach (int rowidx in ctrl.SelectedPropertyRows)
              col.SetElementEmpty(rowidx);
          }
        }

        // clear whole columns when no specific
        if (ctrl.SelectedDataColumns.Count > 0)
        {
          if (ctrl.SelectedDataRows.Count == 0) // if only data columns but not rows selected, we can clean the data columns complete
          {
            foreach (int colidx in ctrl.SelectedDataColumns)
              ctrl.DataTable.DataColumns[colidx].Clear();
          }
          else // if data columns and also rows are selected, we have to clean the cells individually
          {
            foreach (int colidx in ctrl.SelectedDataColumns)
            {
              DataColumn col = ctrl.DataTable.DataColumns[colidx];
              foreach (int rowidx in ctrl.SelectedDataRows)
                col.SetElementEmpty(rowidx);
            }
          }
        }
        else if (ctrl.SelectedDataRows.Count > 0) // if only rows are selected, clean them complete
        {
          for (int colidx = ctrl.DataTable.DataColumnCount - 1; colidx >= 0; colidx--)
          {
            DataColumn col = ctrl.DataTable.DataColumns[colidx];
            foreach (int rowidx in ctrl.SelectedDataRows)
              col.SetElementEmpty(rowidx);
          }
        }

        // end code for the selected rows
        suspendToken.Dispose();
      }
      ctrl.TableAreaInvalidate(); // necessary because we changed the selections
    }

    /// <summary>
    /// Opens a dialog to enter a row number,
    /// and then jumps to that row in the worksheet view.
    /// </summary>
    /// <param name="ctrl">The worksheet controller.</param>
    public static void GotoRow(IWorksheetController ctrl)
    {
      var controller = new Gui.Common.IntegerValueInputController(0, "Row number: ");
      if (true == Current.Gui.ShowDialog(controller, "Enter row number.."))
      {
        ctrl.VerticalScrollPosition = controller.EnteredContents;
      }
    }

    public static void CopyToClipboard(IWorksheetController dg)
    {
      Altaxo.Data.DataTable dt = dg.DataTable;
      var dataObject = Current.Gui.GetNewClipboardDataObject();

      if (dg.AreDataCellsSelected)
      {
        WriteAsciiToClipBoardIfDataCellsSelected(dg, dataObject);
      }
      else if (dg.ArePropertyCellsSelected && !(dg.AreDataCellsSelected))
      {
        WriteAsciiToClipBoardIfOnlyPropertyCellsSelected(dg, dataObject);
      }

      if (dg.AreColumnsOrRowsSelected)
      {
        // copy the data as table with the selected columns
        var tablememento = new Altaxo.Data.DataTable.ClipboardMemento(dg.DataTable, dg.SelectedDataColumns, dg.SelectedDataRows, dg.SelectedPropertyColumns, dg.SelectedPropertyRows);
        Altaxo.Serialization.Clipboard.ClipboardSerialization.PutObjectToDataObject("Altaxo.Data.DataTable.ClipboardMemento", tablememento, dataObject);
      }

      Current.Gui.SetClipboardDataObject(dataObject, true);
    }

    /// <summary>
    /// Writes ASCII to the clipboard if data cells are selected.
    /// </summary>
    /// <param name="dg">The worksheet controller</param>
    /// <param name="dao">The clipboard data object</param>
    protected static void WriteAsciiToClipBoardIfDataCellsSelected(IWorksheetController dg, Altaxo.Gui.IClipboardSetDataObject dao)
    {
      // columns are selected
      DataTable dt = dg.DataTable;
      Altaxo.Collections.AscendingIntegerCollection selCols = dg.SelectedDataColumns;
      Altaxo.Collections.AscendingIntegerCollection selRows = dg.SelectedDataRows;
      Altaxo.Collections.AscendingIntegerCollection selPropCols = dg.SelectedPropertyColumns;
      WriteAsciiToClipBoardIfDataCellsSelected(dt, selCols, selRows, selPropCols, dao);
    }

    /// <summary>
    /// Writes ASCII to the clipboard if data cells are selected.
    /// </summary>
    /// <param name="dt">The data table.</param>
    /// <param name="selCols">Selected data columns.</param>
    /// <param name="selRows">Selected data rows.</param>
    /// <param name="selPropCols">Selected property columns.</param>
    /// <param name="dao">The clipboard data object</param>
    public static void WriteAsciiToClipBoardIfDataCellsSelected(
      DataTable dt,
      Altaxo.Collections.AscendingIntegerCollection selCols,
      Altaxo.Collections.AscendingIntegerCollection selRows,
      Altaxo.Collections.AscendingIntegerCollection selPropCols,
      Altaxo.Gui.IClipboardSetDataObject dao)
    {
      if (selCols.Count == 0)
      {
        selCols = new Altaxo.Collections.AscendingIntegerCollection();
        selCols.AddRange(0, dt.DataColumnCount);
      }

      if (selRows.Count == 0)
      {
        selRows = new Altaxo.Collections.AscendingIntegerCollection();
        int nRows = 0; // count the rows since they are maybe less than in the hole worksheet
        for (int i = 0; i < selCols.Count; i++)
        {
          nRows = System.Math.Max(nRows, dt[selCols[i]].Count);
        }
        selRows.AddRange(0, nRows);
      }

      var str = new System.IO.StringWriter();
      for (int i = 0; i < selPropCols.Count; i++)
      {
        for (int j = 0; j < selCols.Count; j++)
        {
          str.Write(dt.PropertyColumns[selPropCols[i]][selCols[j]].ToString());
          if (j < selCols.Count - 1)
            str.Write(";");
          else
            str.WriteLine();
        }
      }

      for (int i = 0; i < selRows.Count; i++)
      {
        for (int j = 0; j < selCols.Count; j++)
        {
          str.Write(dt.DataColumns[selCols[j]][selRows[i]].ToString());
          if (j < selCols.Count - 1)
            str.Write(";");
          else
            str.WriteLine();
        }
      }
      dao.SetCommaSeparatedValues(str.ToString());

      // now also as tab separated text
      var sw = new System.IO.StringWriter();

      for (int i = 0; i < selPropCols.Count; i++)
      {
        for (int j = 0; j < selCols.Count; j++)
        {
          str.Write(dt.PropertyColumns[selPropCols[i]][selCols[j]].ToString());
          if (j < selCols.Count - 1)
            str.Write("\t");
          else
            str.WriteLine();
        }
      }
      for (int i = 0; i < selRows.Count; i++)
      {
        for (int j = 0; j < selCols.Count; j++)
        {
          sw.Write(dt.DataColumns[selCols[j]][selRows[i]].ToString());
          if (j < selCols.Count - 1)
            sw.Write("\t");
          else
            sw.WriteLine();
        }
      }
      dao.SetData(typeof(string), sw.ToString());
    }

    /// <summary>
    /// Writes ASCII to the clipboard if only property cells are selected.
    /// </summary>
    /// <param name="dg">The worksheet controller</param>
    /// <param name="dao">The clipboard data object</param>
    protected static void WriteAsciiToClipBoardIfOnlyPropertyCellsSelected(IWorksheetController dg, Altaxo.Gui.IClipboardSetDataObject dao)
    {
      // columns are selected
      DataTable dt = dg.DataTable;
      Altaxo.Collections.AscendingIntegerCollection selCols = dg.SelectedPropertyColumns;
      Altaxo.Collections.AscendingIntegerCollection selRows = dg.SelectedPropertyRows;
      if (selRows.Count == 0)
      {
        selRows = new Altaxo.Collections.AscendingIntegerCollection();
        selRows.AddRange(0, dg.DataTable.PropertyRowCount);
      }

      var str = new System.IO.StringWriter();
      for (int i = 0; i < selRows.Count; i++)
      {
        for (int j = 0; j < selCols.Count; j++)
        {
          str.Write(dt.PropertyColumns[selCols[j]][selRows[i]].ToString());
          if (j < selCols.Count - 1)
            str.Write(";");
          else
            str.WriteLine();
        }
      }
      dao.SetCommaSeparatedValues(str.ToString());

      // now also as tab separated text
      var sw = new System.IO.StringWriter();

      for (int i = 0; i < selRows.Count; i++)
      {
        for (int j = 0; j < selCols.Count; j++)
        {
          sw.Write(dt.PropertyColumns[selCols[j]][selRows[i]].ToString());
          if (j < selCols.Count - 1)
            sw.Write("\t");
          else
            sw.WriteLine();
        }
      }
      dao.SetData(typeof(string), sw.ToString());
    }

    /// <summary>
    /// Pastes data from a table (usually deserialized table from the clipboard) into a worksheet.
    /// The paste operation depends on the current selection of columns, rows, or property columns.
    /// </summary>
    /// <param name="ctrl">The worksheet to paste into.</param>
    /// <param name="sourcetable">The table which contains the data to paste into the worksheet.</param>
    /// <remarks>The paste operation is defined in the following way:
    /// If nothing is currently selected, the columns are appended to the end of the worksheet and the property data
    /// are set for that columns.
    /// If only columns are currently selected, the data is pasted in that columns (column by column). If number of
    /// selected columns not match the number of columns in the paste table, but match the number of rows in the paste table,
    /// the paste is done column by row.
    ///
    /// </remarks>
    public static void PasteFromTable(IWorksheetController ctrl, Altaxo.Data.DataTable sourcetable)
    {
      using (var suspendToken = ctrl.DataTable.SuspendGetToken())
      {
        if (!ctrl.AreColumnsOrRowsSelected)
        {
          PasteFromTableToUnselected(ctrl, sourcetable);
        }
        else if (ctrl.SelectedDataColumns.Count > 0 && ctrl.SelectedDataColumns.Count == sourcetable.DataColumns.ColumnCount)
        {
          PasteFromTableColumnsToSelectedColumns(ctrl, sourcetable);
        }
        else if (ctrl.SelectedDataColumns.Count > 0 && ctrl.SelectedDataColumns.Count == sourcetable.DataColumns.RowCount)
        {
          PasteFromTableRowsToSelectedColumns(ctrl, sourcetable);
        }
        else if (ctrl.SelectedDataRows.Count > 0 && ctrl.SelectedDataRows.Count == sourcetable.DataColumns.RowCount)
        {
          PasteFromTableRowsToSelectedRows(ctrl, sourcetable);
        }
        else if (ctrl.SelectedDataRows.Count > 0 && ctrl.SelectedDataRows.Count == sourcetable.DataColumns.ColumnCount)
        {
          PasteFromTableColumnsToSelectedRows(ctrl, sourcetable);
        }
        else if (ctrl.SelectedPropertyColumns.Count > 0 && ctrl.SelectedPropertyColumns.Count == sourcetable.DataColumns.ColumnCount)
        {
          PasteFromTableColumnsToSelectedPropertyColumns(ctrl, sourcetable);
        }
        // now look if the data are transposed
        else if (ctrl.SelectedPropertyColumns.Count > 0 && ctrl.SelectedPropertyColumns.Count == sourcetable.DataColumns.RowCount)
        {
          PasteFromTableColumnsTransposedToSelectedPropertyColumns(ctrl, sourcetable);
        }

        // now the not exact matches
        else if (ctrl.SelectedDataColumns.Count > 0)
        {
          PasteFromTableColumnsToSelectedColumns(ctrl, sourcetable);
        }
        else if (ctrl.SelectedDataRows.Count > 0)
        {
          PasteFromTableRowsToSelectedRows(ctrl, sourcetable);
        }

        suspendToken.Dispose();
      }
    }

    /// <summary>
    /// Pastes data from a table (usually deserialized table from the clipboard) into a worksheet, which has
    /// no current selections. This means that the data are appended to the end of the worksheet.
    /// </summary>
    /// <param name="dg">The worksheet to paste into.</param>
    /// <param name="sourcetable">The table which contains the data to paste into the worksheet.</param>
    protected static void PasteFromTableToUnselected(IWorksheetController dg, Altaxo.Data.DataTable sourcetable)
    {
      Altaxo.Data.DataTable desttable = dg.DataTable;
      Altaxo.Data.DataColumn[] propertycolumnmap = MapOrCreatePropertyColumns(desttable, sourcetable);

      // add first the data columns to the end of the table
      for (int nCol = 0; nCol < sourcetable.DataColumns.ColumnCount; nCol++)
      {
        string name = sourcetable.DataColumns.GetColumnName(nCol);
        int group = sourcetable.DataColumns.GetColumnGroup(nCol);
        Altaxo.Data.ColumnKind kind = sourcetable.DataColumns.GetColumnKind(nCol);
        var destcolumn = (Altaxo.Data.DataColumn)sourcetable.DataColumns[nCol].Clone();
        desttable.DataColumns.Add(destcolumn, name, kind, group);

        // also fill in the property values
        int nDestColumnIndex = desttable.DataColumns.GetColumnNumber(destcolumn);
        FillRow(propertycolumnmap, nDestColumnIndex, sourcetable.PropCols, nCol);
      } // for all data columns
    }

    /// <summary>
    /// This fills a row of destination columns (different columns at same index) with values from another column collection. Both collections must have
    /// the same number of columns and a 1:1 match of the column types.
    /// </summary>
    /// <param name="destColumns">The collection of destination columns.</param>
    /// <param name="destRowIndex">The row index of destination columns to fill.</param>
    /// <param name="sourceColumns">The source table's property column collection.</param>
    /// <param name="sourceRowIndex">The row index of the source columns to use.</param>
    private static void FillRow(Altaxo.Data.DataColumn[] destColumns, int destRowIndex, Altaxo.Data.DataColumnCollection sourceColumns, int sourceRowIndex)
    {
      for (int nCol = 0; nCol < sourceColumns.ColumnCount; nCol++)
      {
        destColumns[nCol][destRowIndex] = sourceColumns[nCol][sourceRowIndex];
      }
    }

    /// <summary>
    /// Pastes data from a table (usually deserialized table from the clipboard) into a worksheet, which has
    /// currently selected columns. The number of selected columns has to match the number of columns of the source table.
    /// </summary>
    /// <param name="dg">The worksheet to paste into.</param>
    /// <param name="sourcetable">The table which contains the data to paste into the worksheet.</param>
    /// <remarks>The operation is defined as follows: if the is no ro selection, the data are inserted beginning at row[0] of the destination table.
    /// If there is a row selection, the data are inserted in the selected rows, and then in the rows after the last selected rows.
    /// No exception is thrown if a column type does not match the corresponding source column type.
    /// The columns to paste into do not change their name, kind or group number. But property columns in the source table
    /// are pasted into the destination table.</remarks>
    protected static void PasteFromTableColumnsToSelectedColumns(IWorksheetController dg, Altaxo.Data.DataTable sourcetable)
    {
      Altaxo.Data.DataTable desttable = dg.DataTable;

      Altaxo.Data.DataColumn[] propertycolumnmap = MapOrCreatePropertyColumns(desttable, sourcetable);

      // use the selected columns, then use the following columns, then add columns
      int nDestCol = -1;
      for (int nSrcCol = 0; nSrcCol < sourcetable.DataColumns.ColumnCount; nSrcCol++)
      {
        nDestCol = nSrcCol < dg.SelectedDataColumns.Count ? dg.SelectedDataColumns[nSrcCol] : nDestCol + 1;
        Altaxo.Data.DataColumn destcolumn;
        if (nDestCol < desttable.DataColumns.ColumnCount)
        {
          destcolumn = desttable.DataColumns[nDestCol];
        }
        else
        {
          string name = sourcetable.DataColumns.GetColumnName(nSrcCol);
          int group = sourcetable.DataColumns.GetColumnGroup(nSrcCol);
          Altaxo.Data.ColumnKind kind = sourcetable.DataColumns.GetColumnKind(nSrcCol);
          var type = sourcetable.DataColumns[nSrcCol].GetType();
          destcolumn = DataColumn.CreateInstanceOfType(type);
          desttable.DataColumns.Add(destcolumn, name, kind, group);
        }

        // now fill the data into that column
        Altaxo.Data.DataColumn sourcecolumn = sourcetable.DataColumns[nSrcCol];

        try
        {
          int nDestRow = -1;
          for (int nSrcRow = 0; nSrcRow < sourcetable.DataColumns.RowCount; nSrcRow++)
          {
            nDestRow = nSrcRow < dg.SelectedDataRows.Count ? dg.SelectedDataRows[nSrcRow] : nDestRow + 1;
            destcolumn[nDestRow] = sourcecolumn[nSrcRow];
          }
        }
        catch (Exception)
        {
        }

        // also fill in the property values
        int nDestColumnIndex = desttable.DataColumns.GetColumnNumber(destcolumn);
        FillRow(propertycolumnmap, nDestColumnIndex, sourcetable.PropCols, nSrcCol);
      } // for all data columns
    }

    /// <summary>
    /// Pastes data from a table (usually deserialized table from the clipboard) into a worksheet, which has
    /// currently selected property columns. The number of selected property columns has to match the number of data (!) columns of the source table.
    /// </summary>
    /// <param name="dg">The worksheet to paste into.</param>
    /// <param name="sourcetable">The table which contains the data to paste into the worksheet.</param>
    /// <remarks>The operation is defined as follows: if the is no ro selection, the data are inserted beginning at row[0] of the destination table.
    /// If there is a row selection, the data are inserted in the selected rows, and then in the rows after the last selected rows.
    /// No exception is thrown if a column type does not match the corresponding source column type.
    /// The columns to paste into do not change their name, kind or group number. But property columns in the source table
    /// are pasted into the destination table.</remarks>
    protected static void PasteFromTableColumnsToSelectedPropertyColumns(IWorksheetController dg, Altaxo.Data.DataTable sourcetable)
    {
      Altaxo.Data.DataTable desttable = dg.DataTable;

      // use the selected columns, then use the following columns, then add columns
      int nDestCol = -1;
      for (int nSrcCol = 0; nSrcCol < sourcetable.DataColumns.ColumnCount; nSrcCol++)
      {
        nDestCol = nSrcCol < dg.SelectedPropertyColumns.Count ? dg.SelectedPropertyColumns[nSrcCol] : nDestCol + 1;
        Altaxo.Data.DataColumn destcolumn;
        if (nDestCol < desttable.PropertyColumns.ColumnCount)
        {
          destcolumn = desttable.PropertyColumns[nDestCol];
        }
        else
        {
          string name = sourcetable.DataColumns.GetColumnName(nSrcCol);
          int group = sourcetable.DataColumns.GetColumnGroup(nSrcCol);
          Altaxo.Data.ColumnKind kind = sourcetable.DataColumns.GetColumnKind(nSrcCol);
          var type = sourcetable.DataColumns[nSrcCol].GetType();
          destcolumn = DataColumn.CreateInstanceOfType(type);
          desttable.PropertyColumns.Add(destcolumn, name, kind, group);
        }

        // now fill the data into that column
        Altaxo.Data.DataColumn sourcecolumn = sourcetable.DataColumns[nSrcCol];

        try
        {
          int nDestRow = -1;
          for (int nSrcRow = 0; nSrcRow < sourcetable.DataColumns.RowCount; nSrcRow++)
          {
            nDestRow = nSrcRow < dg.SelectedPropertyRows.Count ? dg.SelectedPropertyRows[nSrcRow] : nDestRow + 1;
            destcolumn[nDestRow] = sourcecolumn[nSrcRow];
          }
        }
        catch (Exception)
        {
        }
      } // for all data columns
    }

    /// <summary>
    /// Pastes data from a table (usually deserialized table from the clipboard) into a worksheet, which has
    /// currently selected property columns. The sourceTable is transposed before pasting, i.e. the number of selected property columns has to match the number of data (!) rows of the source table.
    /// </summary>
    /// <param name="dg">The worksheet to paste into.</param>
    /// <param name="sourceTable">The table which contains the data to paste into the worksheet.</param>
    /// <remarks>The operation is defined as follows: if the is no ro selection, the data are inserted beginning at row[0] of the destination table.
    /// If there is a row selection, the data are inserted in the selected rows, and then in the rows after the last selected rows.
    /// No exception is thrown if a column type does not match the corresponding source column type.
    /// The columns to paste into do not change their name, kind or group number. But property columns in the source table
    /// are pasted into the destination table.</remarks>
    protected static void PasteFromTableColumnsTransposedToSelectedPropertyColumns(IWorksheetController dg, Altaxo.Data.DataTable sourceTable)
    {
      Altaxo.Data.DataTable destinationTable = dg.DataTable;

      // use the selected columns, then use the following columns, then add columns
      int destinationColumnIndex = -1;
      for (int sourceRowIndex = 0; sourceRowIndex < sourceTable.DataColumns.RowCount; sourceRowIndex++)
      {
        destinationColumnIndex = sourceRowIndex < dg.SelectedPropertyColumns.Count ? dg.SelectedPropertyColumns[sourceRowIndex] : destinationColumnIndex + 1;
        Altaxo.Data.DataColumn destinationPropertyColumn;
        if (destinationColumnIndex < destinationTable.PropertyColumns.ColumnCount)
        {
          destinationPropertyColumn = destinationTable.PropertyColumns[destinationColumnIndex];
        }
        else
        {
          string name = sourceTable.DataColumns.GetColumnName(0);
          int group = sourceTable.DataColumns.GetColumnGroup(0);
          Altaxo.Data.ColumnKind kind = sourceTable.DataColumns.GetColumnKind(0);
          destinationPropertyColumn = DataColumn.CreateInstanceOfType(sourceTable.DataColumns[0].GetType());
          destinationTable.PropertyColumns.Add(destinationPropertyColumn, name, kind, group);
        }

        // now fill the data into that column
        try
        {
          int destinationRowIndex = -1;
          for (int sourceColumnIndex = 0; sourceColumnIndex < sourceTable.DataColumns.ColumnCount; sourceColumnIndex++)
          {
            destinationRowIndex = sourceColumnIndex < dg.SelectedPropertyRows.Count ? dg.SelectedPropertyRows[sourceColumnIndex] : destinationRowIndex + 1;
            destinationPropertyColumn[destinationRowIndex] = sourceTable.DataColumns[sourceColumnIndex][sourceRowIndex];
          }
        }
        catch (Exception)
        {
        }
      } // for all data columns
    }

    /// <summary>
    /// Pastes data from a table (usually deserialized table from the clipboard) into a worksheet, which has
    /// currently selected rows. The number of selected rows has to match the number of rows of the source table.
    /// </summary>
    /// <param name="dg">The worksheet to paste into.</param>
    /// <param name="sourcetable">The table which contains the data to paste into the worksheet.</param>
    /// <remarks>The operation is defined as follows: if the is no column selection, the data are inserted beginning at the first column of the destination table.
    /// If there is a column selection, the data are inserted in the selected columns, and then in the columns after the last selected columns.
    /// No exception is thrown if a column type does not match the corresponding source column type.
    /// The columns to paste into do not change their name, kind or group number. Property columns in the source table
    /// are pasted into the destination table.</remarks>
    protected static void PasteFromTableRowsToSelectedRows(IWorksheetController dg, Altaxo.Data.DataTable sourcetable)
    {
      Altaxo.Data.DataTable desttable = dg.DataTable;

      Altaxo.Data.DataColumn[] propertycolumnmap = MapOrCreatePropertyColumns(desttable, sourcetable);
      Altaxo.Data.DataColumn[] destdatacolumnmap = MapOrCreateDataColumns(desttable, dg.SelectedDataColumns, sourcetable);

      for (int nCol = 0; nCol < sourcetable.DataColumns.ColumnCount; nCol++)
      {
        // now fill the data into that column

        try
        {
          int nDestRow = -1;
          for (int nSrcRow = 0; nSrcRow < sourcetable.DataColumns.RowCount; nSrcRow++)
          {
            nDestRow = nSrcRow < dg.SelectedDataRows.Count ? dg.SelectedDataRows[nSrcRow] : nDestRow + 1;
            destdatacolumnmap[nCol][nDestRow] = sourcetable.DataColumns[nCol][nSrcRow];
          }
        }
        catch (Exception)
        {
        }

        // also fill in the property values
        int nDestColumnIndex = desttable.DataColumns.GetColumnNumber(destdatacolumnmap[nCol]);
        FillRow(propertycolumnmap, nDestColumnIndex, sourcetable.PropCols, nCol);
      } // for all data columns
    }

    /// <summary>
    /// Pastes data columns from the source table (usually deserialized table from the clipboard) into rows of the destination table, which has
    /// currently selected rows. The number of selected rows has to match the number of columns of the source table.
    /// </summary>
    /// <param name="dg">The worksheet to paste into.</param>
    /// <param name="sourcetable">The table which contains the data to paste into the worksheet.</param>
    /// <remarks>The operation is defined as follows: if there is no column selection, the data are inserted beginning at the first column of the destination table.
    /// If there is a column selection, the data are inserted in the selected columns, and then in the columns after the last selected columns.
    /// No exception is thrown if a cell type does not match the corresponding source cell type.
    /// The columns to paste into do not change their name, kind or group number. Property columns in the source table
    /// are not used for this operation.</remarks>
    protected static void PasteFromTableColumnsToSelectedRows(IWorksheetController dg, Altaxo.Data.DataTable sourcetable)
    {
      Altaxo.Data.DataTable desttable = dg.DataTable;
      Altaxo.Data.DataColumn[] destdatacolumnmap = MapOrCreateDataColumnsToRows(desttable, dg.SelectedDataColumns, sourcetable);

      int nDestRow = -1;
      for (int nSrcCol = 0; nSrcCol < sourcetable.DataColumns.ColumnCount; nSrcCol++)
      {
        nDestRow = nSrcCol < dg.SelectedDataRows.Count ? dg.SelectedDataRows[nSrcCol] : nDestRow + 1;

        for (int nSrcRow = 0; nSrcRow < sourcetable.DataColumns.RowCount; nSrcRow++)
        {
          int nDestCol = nSrcRow;
          try
          { destdatacolumnmap[nDestCol][nDestRow] = sourcetable.DataColumns[nSrcCol][nSrcRow]; }
          catch (Exception) { }
        }
      }
    }

    protected static void PasteFromTableRowsToSelectedColumns(IWorksheetController dg, Altaxo.Data.DataTable sourcetable)
    {
      PasteFromTableColumnsToSelectedRows(dg, sourcetable);
    }

    /// <summary>
    /// Maps each property column of the source table to a corresponding property columns of the destination table. If no matching property
    /// column can be found in the destination table, a new matching property column is added to the destination table.
    /// </summary>
    /// <param name="desttable">The destination table.</param>
    /// <param name="sourcetable">The source table.</param>
    /// <returns>An array of columns. Each column of the array is a property column in the destination table, which
    /// matches the property column in the source table by index.</returns>
    /// <remarks>
    /// 1.) Since the returned columns are part of the PropCols collection of the destination table, you must not
    /// use these for inserting i.e. in other tables.
    /// 2.) The match is based on the names _and_ the types of the property columns. If there is no match,
    /// a new property column of the same type as in the source table and with a reasonable name is created.
    /// Therefore each mapped property column has the same type as its counterpart in the source table.
    /// </remarks>
    protected static Altaxo.Data.DataColumn[] MapOrCreatePropertyColumns(Altaxo.Data.DataTable desttable, Altaxo.Data.DataTable sourcetable)
    {
      var columnmap = new Altaxo.Data.DataColumn[sourcetable.PropCols.ColumnCount];
      for (int nCol = 0; nCol < sourcetable.PropCols.ColumnCount; nCol++)
      {
        string name = sourcetable.PropCols.GetColumnName(nCol);
        int group = sourcetable.PropCols.GetColumnGroup(nCol);
        Altaxo.Data.ColumnKind kind = sourcetable.PropCols.GetColumnKind(nCol);

        // if a property column with the same name and kind exist - use that one - else create a new one
        if (desttable.PropCols.ContainsColumn(name) && desttable.PropCols[name].GetType() == sourcetable.PropCols[nCol].GetType())
        {
          columnmap[nCol] = desttable.PropCols[name];
        }
        else
        {
          // the prop col must be empty - we will add the data later
          columnmap[nCol] = DataColumn.CreateInstanceOfType(sourcetable.PropCols[nCol].GetType());
          desttable.PropCols.Add(columnmap[nCol], name, kind, group);
        }
      }
      return columnmap;
    }

    /// <summary>
    /// Maps each data column of the source table to a corresponding data columns of the destination table.
    /// The matching is based on the index (order) and on the currently selected columns of the destination table.
    /// Attention: The match here does <b>not</b> mean that the two columns are data compatible to each other!
    /// </summary>
    /// <param name="desttable">The destination table.</param>
    /// <param name="selectedDestColumns">The currently selected columns of the destination table.</param>
    /// <param name="sourcetable">The source table.</param>
    /// <returns>An array of columns. Each column of the array is a data column in the destination table, which
    /// matches (by index) the data column in the source table.</returns>
    /// <remarks>
    /// 1.) Since the returned columns are part of the DataColumns collection of the destination table, you must not
    /// use these for inserting i.e. in other tables.
    /// 2.) The match is based on the index and the selected columns of the destination table. The rules are as follows: if there is
    /// no selection, the first column of the destination table matches the first column of the source table, and so forth.
    /// If there is a column selection, the first selected column of the destination table matches the first column of the source table,
    /// the second selected column of the destination table matches the second column of the source table. If more source columns than selected columns in the destination
    /// table exists, the match is done 1:1 after the last selected column of the destination table. If there is no further column in the destination
    /// table to match, new columns are created in the destination table.
    /// </remarks>
    protected static Altaxo.Data.DataColumn[] MapOrCreateDataColumns(Altaxo.Data.DataTable desttable, IAscendingIntegerCollection selectedDestColumns, Altaxo.Data.DataTable sourcetable)
    {
      var columnmap = new Altaxo.Data.DataColumn[sourcetable.DataColumns.ColumnCount];
      int nDestCol = -1;
      for (int nCol = 0; nCol < sourcetable.DataColumns.ColumnCount; nCol++)
      {
        nDestCol = nCol < selectedDestColumns.Count ? selectedDestColumns[nCol] : nDestCol + 1;

        string name = sourcetable.DataColumns.GetColumnName(nCol);
        int group = sourcetable.DataColumns.GetColumnGroup(nCol);
        Altaxo.Data.ColumnKind kind = sourcetable.DataColumns.GetColumnKind(nCol);

        if (nDestCol < desttable.DataColumns.ColumnCount)
        {
          columnmap[nCol] = desttable.DataColumns[nDestCol];
        }
        else
        {
          columnmap[nCol] = DataColumn.CreateInstanceOfType(sourcetable.DataColumns[nCol].GetType());
          desttable.DataColumns.Add(columnmap[nCol], name, kind, group);
        }
      }
      return columnmap;
    }

    /// <summary>
    /// Maps each data column of the source table to a corresponding data row (!) of the destination table.
    /// The matching is based on the index (order) and on the currently selected columns of the destination table.
    /// Attention: The match here does <b>not</b> mean that the data of destination columns and source rows are compatible to each other!
    /// </summary>
    /// <param name="desttable">The destination table.</param>
    /// <param name="selectedDestColumns">The currently selected columns of the destination table.</param>
    /// <param name="sourcetable">The source table.</param>
    /// <returns>An array of columns. Each column of the array is a data column in the destination table, which
    /// matches (by index) the data row (!) in the source table with the same index.</returns>
    /// <remarks>
    /// 1.) Since the returned columns are part of the DataColumns collection of the destination table, you must not
    /// use these for inserting i.e. in other tables.
    /// 2.) The match is based on the index and the selected columns of the destination table. The rules are as follows: if there is
    /// no selection, the first column of the destination table matches the first row of the source table, and so forth.
    /// If there is a column selection, the first selected column of the destination table matches the first row of the source table,
    /// the second selected column of the destination table matches the second row of the source table. If there are more source rows than selected columns in the destination
    /// table exists, the match is done 1:1 after the last selected column of the destination table. If there is no further column in the destination
    /// table to match, new columns are created in the destination table. The type of the newly created columns in the destination table is
    /// the same as the first column of the source table in this case.
    /// </remarks>
    protected static Altaxo.Data.DataColumn[] MapOrCreateDataColumnsToRows(Altaxo.Data.DataTable desttable, IAscendingIntegerCollection selectedDestColumns, Altaxo.Data.DataTable sourcetable)
    {
      var columnmap = new Altaxo.Data.DataColumn[sourcetable.DataColumns.RowCount];
      int nDestCol = -1;
      int group = 0;
      for (int nCol = 0; nCol < sourcetable.DataColumns.RowCount; nCol++)
      {
        nDestCol = nCol < selectedDestColumns.Count ? selectedDestColumns[nCol] : nDestCol + 1;

        if (nDestCol < desttable.DataColumns.ColumnCount)
        {
          group = desttable.DataColumns.GetColumnGroup(nDestCol); // we preserve the group of the last existing column for creation of new columns
          columnmap[nCol] = desttable.DataColumns[nDestCol];
        }
        else
        {
          columnmap[nCol] = DataColumn.CreateInstanceOfType(sourcetable.DataColumns[0].GetType());
          desttable.DataColumns.Add(columnmap[nCol], desttable.DataColumns.FindNewColumnName(), ColumnKind.V, group);
        }
      }
      return columnmap;
    }

    public static DataTable? GetTableFromClipboard()
    {
      var dao = Current.Gui.OpenClipboardDataObject();
      string[] formats = dao.GetFormats();
      //System.Diagnostics.Trace.WriteLine("Available formats:");
      //foreach(string format in formats) System.Diagnostics.Trace.WriteLine(format);

      if (dao.GetDataPresent("Altaxo.Data.DataTable.ClipboardMemento"))
      {
        var tablememento = (Altaxo.Data.DataTable.ClipboardMemento?)Altaxo.Serialization.Clipboard.ClipboardSerialization.GetObjectFromClipboard("Altaxo.Data.DataTable.ClipboardMemento");
        return tablememento?.DataTable;
      }

      object? clipboardobject = null;
      Altaxo.Data.DataTable? table = null;

      if (dao.GetDataPresent("Csv"))
        clipboardobject = dao.GetData("Csv");
      else if (dao.GetDataPresent("Text"))
        clipboardobject = dao.GetData("Text");

      if (clipboardobject is System.IO.MemoryStream)
        table = Altaxo.Serialization.Ascii.AsciiImporter.ImportStreamIntoNewTable((System.IO.Stream)clipboardobject, "clipboard");
      else if (clipboardobject is string)
        table = Altaxo.Serialization.Ascii.AsciiImporter.ImportTextIntoNewTable((string)clipboardobject);

      return table;
    }

    public static void PasteFromClipboard(IWorksheetController dg)
    {
      var table = GetTableFromClipboard();

      if (table is not null)
        PasteFromTable(dg, table);
    }
  }
}
