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
//    along with ctrl program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Worksheet.GUI;

namespace Altaxo.Worksheet.Commands
{
  /// <summary>
  /// Summary description for EditCommands.
  /// </summary>
  public class EditCommands
  {
	

    /// <summary>
    /// Remove the selected columns, rows or property columns.
    /// </summary>
    public static void RemoveSelected(WorksheetController ctrl)
    {
      ctrl.DataTable.Suspend();


      // Property columns are only deleted, if selected alone or in conjunction with data row selection
      if(ctrl.SelectedPropertyColumns.Count>0 && ctrl.SelectedPropertyRows.Count==0 && ctrl.SelectedColumns.Count==0)
      {
        ctrl.DataTable.PropCols.RemoveColumns(ctrl.SelectedPropertyColumns);
        ctrl.SelectedPropertyColumns.Clear();
        ctrl.SelectedPropertyRows.Clear();
      }
      // note here: Property rows are only removed indirect by removing data columns


      // delete the selected columns if there are _only selected columns
      if(ctrl.SelectedColumns.Count>0 && ctrl.SelectedRows.Count==0)
      {
        ctrl.DataTable.RemoveColumns(ctrl.SelectedColumns);
        ctrl.SelectedColumns.Clear(); // now the columns are deleted, so they cannot be selected
      }

      // if rows are selected, remove them in all selected columns or in all columns (if no column selection=
      if(ctrl.SelectedRows.Count>0)
      {
        ctrl.DataTable.DataColumns.RemoveRowsInColumns(
          ctrl.SelectedColumns.Count>0 ? (IAscendingIntegerCollection)ctrl.SelectedColumns : new IntegerRangeAsCollection(0,ctrl.DataTable.DataColumns.ColumnCount),
          ctrl.SelectedRows);

        ctrl.SelectedColumns.Clear();
        ctrl.SelectedRows.Clear();
      }


      // end code for the selected rows
      ctrl.DataTable.Resume();
      ctrl.View.TableAreaInvalidate(); // necessary because we changed the selections



    }


    public static void CopyToClipboard(GUI.WorksheetController dg)
    {
      Altaxo.Data.DataTable dt = dg.DataTable;
      System.Windows.Forms.DataObject dao = new System.Windows.Forms.DataObject();
      int i,j;
    
      if(dg.SelectedColumns.Count>0)
      {
        // columns are selected
        int nCols = dg.SelectedColumns.Count;
        int nRows=0; // count the rows since they are maybe less than in the hole worksheet
        for(i=0;i<nCols;i++)
        {
          nRows = System.Math.Max(nRows,dt[dg.SelectedColumns[i]].Count);
        }

        System.IO.StringWriter str = new System.IO.StringWriter();
        for(i=0;i<nRows;i++)
        {
          for(j=0;j<nCols;j++)
          {
            if(j<nCols-1)
              str.Write("{0};",dt[dg.SelectedColumns[j]][i].ToString());
            else
              str.WriteLine(dt[dg.SelectedColumns[j]][i].ToString());
          }
        }
        dao.SetData(System.Windows.Forms.DataFormats.CommaSeparatedValue, str.ToString());


        // now also as tab separated text
        System.IO.StringWriter sw = new System.IO.StringWriter();
        
        for(i=0;i<nRows;i++)
        {
          for(j=0;j<nCols;j++)
          {
            sw.Write(dt[dg.SelectedColumns[j]][i].ToString());
            if(j<nCols-1)
              sw.Write("\t");
            else
              sw.WriteLine();
          }
        }
        dao.SetData(sw.ToString());
      }

      if(dg.AreColumnsOrRowsSelected)
      {
        // copy the data as table with the selected columns
        Altaxo.Data.DataTable.ClipboardMemento tablememento = new Altaxo.Data.DataTable.ClipboardMemento(
          dg.DataTable,dg.SelectedColumns,dg.SelectedRows,dg.SelectedPropertyColumns,dg.SelectedPropertyRows);
        dao.SetData("Altaxo.Data.DataTable.ClipboardMemento",tablememento);

        // now copy the data object to the clipboard
        System.Windows.Forms.Clipboard.SetDataObject(dao,true);
      }
    }

    /// <summary>
    /// Pastes data from a table (usually deserialized table from the clipboard) into a worksheet.
    /// The paste operation depends on the current selection of columns, rows, or property columns.
    /// </summary>
    /// <param name="dg">The worksheet to paste into.</param>
    /// <param name="sourcetable">The table which contains the data to paste into the worksheet.</param>
    /// <remarks>The paste operation is defined in the following way:
    /// If nothing is currently selected, the columns are appended to the end of the worksheet and the property data
    /// are set for that columns.
    /// If only columns are currently selected, the data is pasted in that columns (column by column). If number of
    /// selected columns not match the number of columns in the paste table, but match the number of rows in the paste table,
    /// the paste is done column by row.
    /// 
    /// </remarks>
    public static void PasteFromTable(GUI.WorksheetController dg, Altaxo.Data.DataTable sourcetable)
    {
      if(!dg.AreColumnsOrRowsSelected)
      {
        PasteFromTableToUnselected(dg,sourcetable);
      }
      else if(dg.SelectedColumns.Count>0 && dg.SelectedColumns.Count == sourcetable.DataColumns.ColumnCount)
      {
        PasteFromTableColumnsToSelectedColumns(dg,sourcetable);
      }
      else if(dg.SelectedColumns.Count>0 && dg.SelectedColumns.Count == sourcetable.DataColumns.RowCount)
      {
        PasteFromTableRowsToSelectedColumns(dg,sourcetable);
      }
      else if(dg.SelectedRows.Count>0 && dg.SelectedRows.Count == sourcetable.DataColumns.RowCount)
      {
        PasteFromTableRowsToSelectedRows(dg,sourcetable);
      }
      else if(dg.SelectedRows.Count>0 && dg.SelectedRows.Count == sourcetable.DataColumns.ColumnCount)
      {
        PasteFromTableColumnsToSelectedRows(dg,sourcetable);
      }
        // here should follow the exact matches with property colums

        // now the not exact matches
      else if(dg.SelectedColumns.Count>0)
      {
        PasteFromTableColumnsToSelectedColumns(dg,sourcetable);
      }
      else if(dg.SelectedRows.Count>0)
      {
        PasteFromTableRowsToSelectedRows(dg,sourcetable);
      }
    }

    /// <summary>
    /// Pastes data from a table (usually deserialized table from the clipboard) into a worksheet, which has
    /// no current selections. This means that the data are appended to the end of the worksheet.
    /// </summary>
    /// <param name="dg">The worksheet to paste into.</param>
    /// <param name="sourcetable">The table which contains the data to paste into the worksheet.</param>
    protected static void PasteFromTableToUnselected(GUI.WorksheetController dg, Altaxo.Data.DataTable sourcetable)
    {
      Altaxo.Data.DataTable desttable = dg.DataTable;
      Altaxo.Data.DataColumn[] propertycolumnmap = MapOrCreatePropertyColumns(desttable,sourcetable);

      // add first the data columns to the end of the table
      for(int nCol=0;nCol<sourcetable.DataColumns.ColumnCount;nCol++)
      {
        string name = sourcetable.DataColumns.GetColumnName(nCol);
        int    group = sourcetable.DataColumns.GetColumnGroup(nCol);
        Altaxo.Data.ColumnKind kind = sourcetable.DataColumns.GetColumnKind(nCol);
        Altaxo.Data.DataColumn destcolumn = (Altaxo.Data.DataColumn)sourcetable.DataColumns[nCol].Clone();
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
    static private void FillRow(Altaxo.Data.DataColumn[] destColumns, int destRowIndex, Altaxo.Data.DataColumnCollection sourceColumns, int sourceRowIndex)
    {
      for(int nCol=0;nCol<sourceColumns.ColumnCount;nCol++)
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
    protected static void PasteFromTableColumnsToSelectedColumns(GUI.WorksheetController dg, Altaxo.Data.DataTable sourcetable)
    {
      Altaxo.Data.DataTable desttable = dg.DataTable;

      Altaxo.Data.DataColumn[] propertycolumnmap = MapOrCreatePropertyColumns(desttable,sourcetable);

      // use the selected columns, then use the following columns, then add columns
      int nDestCol=-1;
      for(int nSrcCol=0;nSrcCol<sourcetable.DataColumns.ColumnCount;nSrcCol++)
      {
        nDestCol = nSrcCol<dg.SelectedColumns.Count ? dg.SelectedColumns[nSrcCol] : nDestCol+1;
        Altaxo.Data.DataColumn destcolumn;
        if(nDestCol<desttable.DataColumns.ColumnCount)
        {
          destcolumn = desttable.DataColumns[nDestCol];
        }
        else
        {

          string name = sourcetable.DataColumns.GetColumnName(nSrcCol);
          int    group = sourcetable.DataColumns.GetColumnGroup(nSrcCol);
          Altaxo.Data.ColumnKind kind = sourcetable.DataColumns.GetColumnKind(nSrcCol);
          destcolumn = (Altaxo.Data.DataColumn)Activator.CreateInstance(sourcetable.DataColumns[nSrcCol].GetType());
          desttable.DataColumns.Add(destcolumn, name, kind, group);
        }
        
        // now fill the data into that column
        Altaxo.Data.DataColumn sourcecolumn = sourcetable.DataColumns[nSrcCol];

        try
        {
          int nDestRow=-1;
          for(int nSrcRow=0;nSrcRow<sourcetable.DataColumns.RowCount;nSrcRow++)
          {
            nDestRow = nSrcRow<dg.SelectedRows.Count ? dg.SelectedRows[nSrcRow] : nDestRow+1;
            destcolumn[nDestRow] = sourcecolumn[nSrcRow];
          }
        }
        catch(Exception)
        {
        }


        // also fill in the property values
        int nDestColumnIndex = desttable.DataColumns.GetColumnNumber(destcolumn);
        FillRow(propertycolumnmap,nDestColumnIndex,sourcetable.PropCols,nSrcCol);
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
    protected static void PasteFromTableRowsToSelectedRows(GUI.WorksheetController dg, Altaxo.Data.DataTable sourcetable)
    {
      Altaxo.Data.DataTable desttable = dg.DataTable;

      Altaxo.Data.DataColumn[] propertycolumnmap = MapOrCreatePropertyColumns(desttable,sourcetable);
      Altaxo.Data.DataColumn[] destdatacolumnmap = MapOrCreateDataColumns(desttable,dg.SelectedColumns,sourcetable);

      for(int nCol=0;nCol<sourcetable.DataColumns.ColumnCount;nCol++)
      {
        // now fill the data into that column

        try
        {
          int nDestRow=-1;
          for(int nSrcRow=0;nSrcRow<sourcetable.DataColumns.RowCount;nSrcRow++)
          {
            nDestRow = nSrcRow<dg.SelectedRows.Count ? dg.SelectedRows[nSrcRow] : nDestRow+1;
            destdatacolumnmap[nCol][nDestRow] = sourcetable.DataColumns[nCol][nSrcRow];
          }
        }
        catch(Exception)
        {
        }


        // also fill in the property values
        int nDestColumnIndex = desttable.DataColumns.GetColumnNumber(destdatacolumnmap[nCol]);
        FillRow(propertycolumnmap,nDestColumnIndex,sourcetable.PropCols,nCol);
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
    protected static void PasteFromTableColumnsToSelectedRows(GUI.WorksheetController dg, Altaxo.Data.DataTable sourcetable)
    {
      Altaxo.Data.DataTable desttable = dg.DataTable;
      Altaxo.Data.DataColumn[] destdatacolumnmap = MapOrCreateDataColumnsToRows(desttable,dg.SelectedColumns,sourcetable);


      int nDestRow=-1;
      for(int nSrcCol=0;nSrcCol<sourcetable.DataColumns.ColumnCount;nSrcCol++)
      {
        nDestRow = nSrcCol<dg.SelectedRows.Count ? dg.SelectedRows[nSrcCol] : nDestRow+1;

        for(int nSrcRow=0;nSrcRow<sourcetable.DataColumns.RowCount;nSrcRow++)
        {
          int nDestCol = nSrcRow;
          try { destdatacolumnmap[nDestCol][nDestRow] = sourcetable.DataColumns[nSrcCol][nSrcRow];  }
          catch(Exception) {}
        }
      }
    }


    protected static void PasteFromTableRowsToSelectedColumns(GUI.WorksheetController dg, Altaxo.Data.DataTable sourcetable)
    {
      PasteFromTableColumnsToSelectedRows(dg,sourcetable);
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
    static protected Altaxo.Data.DataColumn[] MapOrCreatePropertyColumns(Altaxo.Data.DataTable desttable, Altaxo.Data.DataTable sourcetable)
    {
      Altaxo.Data.DataColumn[] columnmap = new Altaxo.Data.DataColumn[sourcetable.PropCols.ColumnCount];
      for(int nCol=0;nCol<sourcetable.PropCols.ColumnCount;nCol++)
      {
        string name = sourcetable.PropCols.GetColumnName(nCol);
        int    group = sourcetable.PropCols.GetColumnGroup(nCol);
        Altaxo.Data.ColumnKind kind = sourcetable.PropCols.GetColumnKind(nCol);

        // if a property column with the same name and kind exist - use that one - else create a new one
        if(desttable.PropCols.ContainsColumn(name) && desttable.PropCols[name].GetType() == sourcetable.PropCols[nCol].GetType())
        {
          columnmap[nCol] = desttable.PropCols[name];
        }
        else
        {
          // the prop col must be empty - we will add the data later
          columnmap[nCol] = (DataColumn)Activator.CreateInstance(sourcetable.PropCols[nCol].GetType());
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
    static protected Altaxo.Data.DataColumn[] MapOrCreateDataColumns(Altaxo.Data.DataTable desttable, IAscendingIntegerCollection selectedDestColumns, Altaxo.Data.DataTable sourcetable)
    {
      Altaxo.Data.DataColumn[] columnmap = new Altaxo.Data.DataColumn[sourcetable.DataColumns.ColumnCount];
      int nDestCol=-1;
      for(int nCol=0;nCol<sourcetable.DataColumns.ColumnCount;nCol++)
      {
        nDestCol = nCol<selectedDestColumns.Count ? selectedDestColumns[nCol] : nDestCol+1;

        string name = sourcetable.DataColumns.GetColumnName(nCol);
        int    group = sourcetable.DataColumns.GetColumnGroup(nCol);
        Altaxo.Data.ColumnKind kind = sourcetable.DataColumns.GetColumnKind(nCol);

        if(nDestCol<desttable.DataColumns.ColumnCount)
        {
          columnmap[nCol] = desttable.DataColumns[nDestCol];
        }
        else
        {
          columnmap[nCol] = (DataColumn)Activator.CreateInstance(sourcetable.DataColumns[nCol].GetType());
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
    static protected Altaxo.Data.DataColumn[] MapOrCreateDataColumnsToRows(Altaxo.Data.DataTable desttable, IAscendingIntegerCollection selectedDestColumns, Altaxo.Data.DataTable sourcetable)
    {
      Altaxo.Data.DataColumn[] columnmap = new Altaxo.Data.DataColumn[sourcetable.DataColumns.RowCount];
      int nDestCol=-1;
      int group=0;
      for(int nCol=0;nCol<sourcetable.DataColumns.RowCount;nCol++) 
      {
        nDestCol = nCol<selectedDestColumns.Count ? selectedDestColumns[nCol] : nDestCol+1;

        if(nDestCol<desttable.DataColumns.ColumnCount)
        {
          group = desttable.DataColumns.GetColumnGroup(nDestCol); // we preserve the group of the last existing column for creation of new columns
          columnmap[nCol] = desttable.DataColumns[nDestCol];
        }
        else
        {
          columnmap[nCol] = (DataColumn)Activator.CreateInstance(sourcetable.DataColumns[0].GetType());
          desttable.DataColumns.Add(columnmap[nCol], desttable.DataColumns.FindNewColumnName(), ColumnKind.V, group);
        }
      }
      return columnmap;
    }

    public static void PasteFromClipboard(GUI.WorksheetController dg)
    {
      Altaxo.Data.DataTable dt = dg.DataTable;
      System.Windows.Forms.DataObject dao = System.Windows.Forms.Clipboard.GetDataObject() as System.Windows.Forms.DataObject;

      string[] formats = dao.GetFormats();
      System.Diagnostics.Trace.WriteLine("Available formats:");
      foreach(string format in formats)
        System.Diagnostics.Trace.WriteLine(format);

      if(dao.GetDataPresent("Altaxo.Data.DataTable.ClipboardMemento"))
      {
        Altaxo.Data.DataTable.ClipboardMemento tablememento = (Altaxo.Data.DataTable.ClipboardMemento)dao.GetData("Altaxo.Data.DataTable.ClipboardMemento");
        PasteFromTable(dg,tablememento.DataTable);
        return;
      }

      object clipboardobject=null;
      Altaxo.Data.DataTable table=null;

      if(dao.GetDataPresent("Csv"))
        clipboardobject = dao.GetData("Csv");
      else if(dao.GetDataPresent("Text"))
        clipboardobject = dao.GetData("Text");


      if(clipboardobject is System.IO.MemoryStream)
        table = Altaxo.Serialization.Ascii.AsciiImporter.Import((System.IO.Stream)clipboardobject);
      else if(clipboardobject is string)
        table = Altaxo.Serialization.Ascii.AsciiImporter.Import((string)clipboardobject);
      
      
      if(null!=table)
        PasteFromTable(dg,table);
    
    }

  

  }
}
