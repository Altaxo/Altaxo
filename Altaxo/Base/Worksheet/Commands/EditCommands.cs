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
    /// Creates a matrix from three selected columns. This must be a x-column, a y-column, and a value column.
    /// </summary>
    /// <param name="ctrl">Controller where the columns are selected in.</param>
    /// <returns>Null if no error occurs, or an error message.</returns>
    public static string XYVToMatrix(WorksheetController ctrl)
    {
      DataColumn xcol=null, ycol=null, vcol=null;

      // for this command to work, there must be exactly 3 data columns selected
      if (ctrl.SelectedDataColumns.Count == 3)
      {
        // use the last column that is a value column as v
        // and use the first column that is an x column as x
        for (int i = 2; i >= 0; i--)
        {
          if (ctrl.DataTable.DataColumns.GetColumnKind(ctrl.SelectedDataColumns[i]) == ColumnKind.V)
          {
            vcol = ctrl.DataTable.DataColumns[ctrl.SelectedDataColumns[i]];
            break;
          }
        }
        for (int i = 2; i >= 0; i--)
        {
          if (ctrl.DataTable.DataColumns.GetColumnKind(ctrl.SelectedDataColumns[i]) == ColumnKind.Y)
          {
            ycol = ctrl.DataTable.DataColumns[ctrl.SelectedDataColumns[i]];
            break;
          }
        }
        for (int i = 2; i >= 0; i--)
        {
          if (ctrl.DataTable.DataColumns.GetColumnKind(ctrl.SelectedDataColumns[i]) == ColumnKind.X)
          {
            xcol = ctrl.DataTable.DataColumns[ctrl.SelectedDataColumns[i]];
            break;
          }
        }

        if (xcol == null || ycol == null || vcol == null)
          return "The selected columns must be a x-column, a y-column, and a value column";
      }
      else
      {
        return "You must select exactly a x-column, a y-column, and a value column";
      }

      DataTable newtable;
      string msg = XYVToMatrix(xcol, ycol, vcol, out newtable);
      if (msg != null)
        return msg;

      Current.ProjectService.CreateNewWorksheet(newtable);

      return null;
    }

       /// <summary>
    /// Creates a matrix from three selected columns. This must be a x-column, a y-column, and a value column.
    /// </summary>
    /// <param name="xcol">Column of x-values.</param>
    /// <param name="ycol">Column of y-values.</param>
    /// <param name="vcol">Column of v-values.</param>
    /// <param name="newtable">On return, contains the newly created table matrix.</param>
    /// <returns>Null if no error occurs, or an error message.</returns>
    public static string XYVToMatrix(DataColumn xcol, DataColumn ycol, DataColumn vcol, out DataTable newtable)
    {
      newtable = null;
      System.Collections.SortedList xx = new System.Collections.SortedList();
      System.Collections.SortedList yy = new System.Collections.SortedList();
      int len = xcol.Count;
      len = Math.Min(len, ycol.Count);
      len = Math.Min(len, vcol.Count);
      
      // Fill the xx and yy lists
      for (int i = 0; i < len; ++i)
      {
        if(!xx.Contains(xcol[i]))
          xx.Add(xcol[i],null);

         if(!yy.Contains(ycol[i]))
          yy.Add(ycol[i],null);
      }

      DataColumn xnew = (DataColumn)Activator.CreateInstance(xcol.GetType());
      DataColumn ynew = (DataColumn)Activator.CreateInstance(ycol.GetType());
      xnew.Clear();
      ynew.Clear();

      for (int i = xx.Count - 1; i >= 0; --i)
      {
        xnew[i] = (AltaxoVariant)xx.GetKey(i);
        xx[xx.GetKey(i)] = i;
      }

      for (int i = yy.Count - 1; i >= 0; --i)
      {
        ynew[1 + i] = (AltaxoVariant)yy.GetKey(i); // 1 + is because the table will get an additional x-column
        yy[yy.GetKey(i)] = i; 
      }

      DataColumn vtemplate = (DataColumn)Activator.CreateInstance(vcol.GetType());

      // make a new table with yy.Count number of columns
      DataColumn[] vcols = new DataColumn[yy.Count];
      for (int i = yy.Count - 1; i >= 0; --i)
      {
        vcols[i] = (DataColumn)vtemplate.Clone();
      }

      // now fill the columns
      for (int i = 0; i < len; ++i)
      {
        int xidx = (int)xx[xcol[i]];
        int yidx = (int)yy[ycol[i]];

        vcols[yidx][xidx] = vcol[i];
      }

      // assemble all columns together in a table
      newtable = new DataTable();
      newtable.DataColumns.Add(xnew,"X",ColumnKind.X,0);
      newtable.PropertyColumns.Add(ynew,"Y",ColumnKind.Y,0);

      for (int i = 0; i < vcols.Length; ++i)
        newtable.DataColumns.Add(vcols[i], "V" + i.ToString(), ColumnKind.V, 0);

      return null;
    }

    /// <summary>
    /// Remove the selected columns, rows or property columns.
    /// </summary>
    public static void RemoveSelected(WorksheetController ctrl)
    {
      ctrl.DataTable.Suspend();


      // Property columns are only deleted, if selected alone or in conjunction with data row selection
      if(ctrl.SelectedPropertyColumns.Count>0 && ctrl.SelectedPropertyRows.Count==0 && ctrl.SelectedDataColumns.Count==0)
      {
        ctrl.DataTable.PropCols.RemoveColumns(ctrl.SelectedPropertyColumns);
        ctrl.SelectedPropertyColumns.Clear();
        ctrl.SelectedPropertyRows.Clear();
      }
      // note here: Property rows are only removed indirect by removing data columns


      // delete the selected columns if there are _only selected columns
      if(ctrl.SelectedDataColumns.Count>0 && ctrl.SelectedDataRows.Count==0)
      {
        ctrl.DataTable.RemoveColumns(ctrl.SelectedDataColumns);
        ctrl.SelectedDataColumns.Clear(); // now the columns are deleted, so they cannot be selected
      }

      // if rows are selected, remove them in all selected columns or in all columns (if no column selection=
      if(ctrl.SelectedDataRows.Count>0)
      {
        ctrl.DataTable.DataColumns.RemoveRowsInColumns(
          ctrl.SelectedDataColumns.Count>0 ? (IAscendingIntegerCollection)ctrl.SelectedDataColumns : new IntegerRangeAsCollection(0,ctrl.DataTable.DataColumns.ColumnCount),
          ctrl.SelectedDataRows);

        ctrl.SelectedDataColumns.Clear();
        ctrl.SelectedDataRows.Clear();
      }


      // end code for the selected rows
      ctrl.DataTable.Resume();
      ctrl.View.TableAreaInvalidate(); // necessary because we changed the selections



    }

    /// <summary>
    /// This commands clean all selected cells.
    /// </summary>
    /// <param name="ctrl">The worksheet controller.</param>
    public static void CleanSelected(WorksheetController ctrl)
    {
      ctrl.DataTable.Suspend();


      // Property columns are only deleted, if selected alone or in conjunction with data row selection
      if (ctrl.SelectedPropertyColumns.Count > 0 && ctrl.SelectedPropertyRows.Count == 0 && ctrl.SelectedDataColumns.Count == 0)
      {
        ctrl.DataTable.PropCols.RemoveColumns(ctrl.SelectedPropertyColumns);
        ctrl.SelectedPropertyColumns.Clear();
        ctrl.SelectedPropertyRows.Clear();
      }
      // note here: Property rows are only removed indirect by removing data columns




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
        for(int colidx = ctrl.DataTable.DataColumnCount-1;colidx>=0;colidx--)
        {
          DataColumn col = ctrl.DataTable.DataColumns[colidx];
          foreach (int rowidx in ctrl.SelectedDataRows)
            col.SetElementEmpty(rowidx);
        }
      }


      // end code for the selected rows
      ctrl.DataTable.Resume();
      ctrl.View.TableAreaInvalidate(); // necessary because we changed the selections
    }


    public static void CopyToClipboard(GUI.WorksheetController dg)
    {
      Altaxo.Data.DataTable dt = dg.DataTable;
      System.Windows.Forms.DataObject dao = new System.Windows.Forms.DataObject();
    
      if(dg.AreDataCellsSelected)
      {
        WriteAsciiToClipBoardIfDataCellsSelected(dg,dao);
      }
      else if(dg.ArePropertyCellsSelected && !(dg.AreDataCellsSelected))
      {
        WriteAsciiToClipBoardIfOnlyPropertyCellsSelected(dg,dao);
      }

      if(dg.AreColumnsOrRowsSelected)
      {
        // copy the data as table with the selected columns
        Altaxo.Data.DataTable.ClipboardMemento tablememento = new Altaxo.Data.DataTable.ClipboardMemento(
          dg.DataTable,dg.SelectedDataColumns,dg.SelectedDataRows,dg.SelectedPropertyColumns,dg.SelectedPropertyRows);
        dao.SetData("Altaxo.Data.DataTable.ClipboardMemento",tablememento);

        // now copy the data object to the clipboard
        System.Windows.Forms.Clipboard.SetDataObject(dao,true);
      
      
  
      
      }
    }


 
    /// <summary>
    /// Writes ASCII to the clipboard if data cells are selected.
    /// </summary>
    /// <param name="dg">The worksheet controller</param>
    /// <param name="dao">The clipboard data object</param>
    protected static void WriteAsciiToClipBoardIfDataCellsSelected(GUI.WorksheetController dg, System.Windows.Forms.DataObject dao)
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
      System.Windows.Forms.DataObject dao)
    {
      if(selCols.Count==0)
      {
        selCols = new Altaxo.Collections.AscendingIntegerCollection();
        selCols.AddRange(0,dt.DataColumnCount);
      }

      if(selRows.Count==0)
      {
        selRows = new Altaxo.Collections.AscendingIntegerCollection();
        int nRows=0; // count the rows since they are maybe less than in the hole worksheet
        for(int i=0;i<selCols.Count;i++)
        {
          nRows = System.Math.Max(nRows,dt[selCols[i]].Count);
        }
        selRows.AddRange(0,nRows);
      }
  
      System.IO.StringWriter str = new System.IO.StringWriter();
      for(int i=0;i<selPropCols.Count;i++)
      {
        for(int j=0;j<selCols.Count;j++)
        {
          str.Write(dt.PropertyColumns[selPropCols[i]][selCols[j]].ToString());
          if(j<selCols.Count-1)
            str.Write(";");
          else
            str.WriteLine();
        }
      }

      for(int i=0;i<selRows.Count;i++)
      {
        for(int j=0;j<selCols.Count;j++)
        {
          str.Write(dt.DataColumns[selCols[j]][selRows[i]].ToString());
          if(j<selCols.Count-1)
            str.Write(";");
          else
            str.WriteLine();
        }
      }
      dao.SetData(System.Windows.Forms.DataFormats.CommaSeparatedValue, str.ToString());


      // now also as tab separated text
      System.IO.StringWriter sw = new System.IO.StringWriter();
        
      for(int i=0;i<selPropCols.Count;i++)
      {
        for(int j=0;j<selCols.Count;j++)
        {
          str.Write(dt.PropertyColumns[selPropCols[i]][selCols[j]].ToString());
          if(j<selCols.Count-1)
            str.Write("\t");
          else
            str.WriteLine();
        }
      }
      for(int i=0;i<selRows.Count;i++)
      {
        for(int j=0;j<selCols.Count;j++)
        {
          sw.Write(dt.DataColumns[selCols[j]][selRows[i]].ToString());
          if(j<selCols.Count-1)
            sw.Write("\t");
          else
            sw.WriteLine();
        }
      }
      dao.SetData(sw.ToString());
    }
  

    /// <summary>
    /// Writes ASCII to the clipboard if only property cells are selected.
    /// </summary>
    /// <param name="dg">The worksheet controller</param>
    /// <param name="dao">The clipboard data object</param>
    protected static void WriteAsciiToClipBoardIfOnlyPropertyCellsSelected(GUI.WorksheetController dg, System.Windows.Forms.DataObject dao)
    {
      // columns are selected
      DataTable dt = dg.DataTable;
      Altaxo.Collections.AscendingIntegerCollection selCols = dg.SelectedPropertyColumns;
      Altaxo.Collections.AscendingIntegerCollection selRows = dg.SelectedPropertyRows;
      if(selRows.Count==0)
      {
        selRows = new Altaxo.Collections.AscendingIntegerCollection();
        selRows.AddRange(0,dg.Doc.PropertyRowCount);
      }

      System.IO.StringWriter str = new System.IO.StringWriter();
      for(int i=0;i<selRows.Count;i++)
      {
        for(int j=0;j<selCols.Count;j++)
        {
          str.Write(dt.PropertyColumns[selCols[j]][selRows[i]].ToString());
          if(j<selCols.Count-1)
            str.Write(";");
          else
            str.WriteLine();
        }
      }
      dao.SetData(System.Windows.Forms.DataFormats.CommaSeparatedValue, str.ToString());


      // now also as tab separated text
      System.IO.StringWriter sw = new System.IO.StringWriter();
        
      for(int i=0;i<selRows.Count;i++)
      {
        for(int j=0;j<selCols.Count;j++)
        {
          sw.Write(dt.PropertyColumns[selCols[j]][selRows[i]].ToString());
          if(j<selCols.Count-1)
            sw.Write("\t");
          else
            sw.WriteLine();
        }
      }
      dao.SetData(sw.ToString());
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
      else if(dg.SelectedDataColumns.Count>0 && dg.SelectedDataColumns.Count == sourcetable.DataColumns.ColumnCount)
      {
        PasteFromTableColumnsToSelectedColumns(dg,sourcetable);
      }
      else if(dg.SelectedDataColumns.Count>0 && dg.SelectedDataColumns.Count == sourcetable.DataColumns.RowCount)
      {
        PasteFromTableRowsToSelectedColumns(dg,sourcetable);
      }
      else if(dg.SelectedDataRows.Count>0 && dg.SelectedDataRows.Count == sourcetable.DataColumns.RowCount)
      {
        PasteFromTableRowsToSelectedRows(dg,sourcetable);
      }
      else if(dg.SelectedDataRows.Count>0 && dg.SelectedDataRows.Count == sourcetable.DataColumns.ColumnCount)
      {
        PasteFromTableColumnsToSelectedRows(dg,sourcetable);
      }
      else if(dg.SelectedPropertyColumns.Count>0 && dg.SelectedPropertyColumns.Count == sourcetable.DataColumns.ColumnCount)
      {
        PasteFromTableColumnsToSelectedPropertyColumns(dg,sourcetable);
      }

        // now the not exact matches
      else if(dg.SelectedDataColumns.Count>0)
      {
        PasteFromTableColumnsToSelectedColumns(dg,sourcetable);
      }
      else if(dg.SelectedDataRows.Count>0)
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
        nDestCol = nSrcCol<dg.SelectedDataColumns.Count ? dg.SelectedDataColumns[nSrcCol] : nDestCol+1;
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
            nDestRow = nSrcRow<dg.SelectedDataRows.Count ? dg.SelectedDataRows[nSrcRow] : nDestRow+1;
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
    /// currently selected property columns. The number of selected property columns has to match the number of data (!) columns of the source table.
    /// </summary>
    /// <param name="dg">The worksheet to paste into.</param>
    /// <param name="sourcetable">The table which contains the data to paste into the worksheet.</param>
    /// <remarks>The operation is defined as follows: if the is no ro selection, the data are inserted beginning at row[0] of the destination table.
    /// If there is a row selection, the data are inserted in the selected rows, and then in the rows after the last selected rows.
    /// No exception is thrown if a column type does not match the corresponding source column type.
    /// The columns to paste into do not change their name, kind or group number. But property columns in the source table
    /// are pasted into the destination table.</remarks>
    protected static void PasteFromTableColumnsToSelectedPropertyColumns(GUI.WorksheetController dg, Altaxo.Data.DataTable sourcetable)
    {
      Altaxo.Data.DataTable desttable = dg.DataTable;

      // use the selected columns, then use the following columns, then add columns
      int nDestCol=-1;
      for(int nSrcCol=0;nSrcCol<sourcetable.DataColumns.ColumnCount;nSrcCol++)
      {
        nDestCol = nSrcCol<dg.SelectedPropertyColumns.Count ? dg.SelectedPropertyColumns[nSrcCol] : nDestCol+1;
        Altaxo.Data.DataColumn destcolumn;
        if(nDestCol<desttable.PropertyColumns.ColumnCount)
        {
          destcolumn = desttable.PropertyColumns[nDestCol];
        }
        else
        {

          string name = sourcetable.DataColumns.GetColumnName(nSrcCol);
          int    group = sourcetable.DataColumns.GetColumnGroup(nSrcCol);
          Altaxo.Data.ColumnKind kind = sourcetable.DataColumns.GetColumnKind(nSrcCol);
          destcolumn = (Altaxo.Data.DataColumn)Activator.CreateInstance(sourcetable.DataColumns[nSrcCol].GetType());
          desttable.PropertyColumns.Add(destcolumn, name, kind, group);
        }
        
        // now fill the data into that column
        Altaxo.Data.DataColumn sourcecolumn = sourcetable.DataColumns[nSrcCol];

        try
        {
          int nDestRow=-1;
          for(int nSrcRow=0;nSrcRow<sourcetable.DataColumns.RowCount;nSrcRow++)
          {
            nDestRow = nSrcRow<dg.SelectedPropertyRows.Count ? dg.SelectedPropertyRows[nSrcRow] : nDestRow+1;
            destcolumn[nDestRow] = sourcecolumn[nSrcRow];
          }
        }
        catch(Exception)
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
    protected static void PasteFromTableRowsToSelectedRows(GUI.WorksheetController dg, Altaxo.Data.DataTable sourcetable)
    {
      Altaxo.Data.DataTable desttable = dg.DataTable;

      Altaxo.Data.DataColumn[] propertycolumnmap = MapOrCreatePropertyColumns(desttable,sourcetable);
      Altaxo.Data.DataColumn[] destdatacolumnmap = MapOrCreateDataColumns(desttable,dg.SelectedDataColumns,sourcetable);

      for(int nCol=0;nCol<sourcetable.DataColumns.ColumnCount;nCol++)
      {
        // now fill the data into that column

        try
        {
          int nDestRow=-1;
          for(int nSrcRow=0;nSrcRow<sourcetable.DataColumns.RowCount;nSrcRow++)
          {
            nDestRow = nSrcRow<dg.SelectedDataRows.Count ? dg.SelectedDataRows[nSrcRow] : nDestRow+1;
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
      Altaxo.Data.DataColumn[] destdatacolumnmap = MapOrCreateDataColumnsToRows(desttable,dg.SelectedDataColumns,sourcetable);


      int nDestRow=-1;
      for(int nSrcCol=0;nSrcCol<sourcetable.DataColumns.ColumnCount;nSrcCol++)
      {
        nDestRow = nSrcCol<dg.SelectedDataRows.Count ? dg.SelectedDataRows[nSrcCol] : nDestRow+1;

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
      //foreach(string format in formats) System.Diagnostics.Trace.WriteLine(format);

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
