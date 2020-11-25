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
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

#nullable enable
using System;
using System.Text.RegularExpressions;
using Altaxo.Data;
using Altaxo.Gui.Common;
using Altaxo.Gui.Worksheet.Viewing;

namespace Altaxo.Worksheet.Commands
{
  /// <summary>
  /// Contains static functions for handling column commands.
  /// </summary>
  public class ColumnCommands
  {
    #region Rename column

    /// <summary>
    /// Renames the selected data column or property column.
    /// </summary>
    /// <param name="ctrl">The worksheet controller for the table.</param>
    public static void RenameSelectedColumn(IWorksheetController ctrl)
    {
      if (ctrl.SelectedDataColumns.Count == 1 && ctrl.SelectedPropertyColumns.Count == 0)
      {
        ctrl.DataTable.DataColumns[ctrl.SelectedDataColumns[0]].ShowRenameColumnDialog();
      }
      else if (ctrl.SelectedDataColumns.Count == 0 && ctrl.SelectedPropertyColumns.Count == 1)
      {
        ctrl.DataTable.PropCols[ctrl.SelectedPropertyColumns[0]].ShowRenameColumnDialog();
      }
    }

    #endregion Rename column

    #region Set group number

    /// <summary>
    /// Sets the group number of the selected column
    /// </summary>
    /// <param name="ctrl">The worksheet controller for the table.</param>
    public static void ShowSetColumnGroupNumberDialog(IWorksheetController ctrl)
    {
      if (ctrl.DataTable.ShowSetColumnGroupNumberDialog(ctrl.SelectedDataColumns, ctrl.SelectedPropertyColumns))
      {
        ctrl.ClearAllSelections();
        ctrl.TableAreaInvalidate();
      }
    }

    #endregion Set group number

    #region Set column position

    /// <summary>
    /// Moves the selected column to a new position. The new position must be entered by the user.
    /// </summary>
    /// <param name="ctrl">The worksheet controller for the table.</param>
    /// <returns>Null if successful; otherwise, an error message.</returns>
    public static string? SetSelectedColumnPosition(IWorksheetController ctrl)
    {
      // check condition - either DataColumns or propertycolumns can be selected - but not both
      if (ctrl.SelectedDataColumns.Count > 0 && ctrl.SelectedPropertyColumns.Count > 0)
        return "Don't know what to do - both data and property columns are selected";

      if (ctrl.SelectedDataColumns.Count == 0 && ctrl.SelectedPropertyColumns.Count == 0)
        return null; // nothing to do

      int newposition = int.MinValue;

      var ivictrl = new IntegerValueInputController(0, "Please enter the new position (>=0):")
      {
        Validator = new IntegerValueInputController.ZeroOrPositiveIntegerValidator()
      };
      if (Current.Gui.ShowDialog(ivictrl, "New column position", false))
      {
        newposition = ivictrl.EnteredContents;
      }
      else
        return null;

      SetSelectedColumnPosition(ctrl, newposition);

      return null;
    }

    /// <summary>
    /// Moves the selected columns to a new position <code>nPosition</code>.
    /// </summary>
    /// <param name="ctrl">The worksheet controller.</param>
    /// <param name="nPosition">The new position for the selected columns.</param>
    public static void SetSelectedColumnPosition(IWorksheetController ctrl, int nPosition)
    {
      if (ctrl.SelectedDataColumns.Count > 0)
      {
        if (ctrl.SelectedDataColumns.Count + nPosition > ctrl.DataTable.DataColumnCount)
          nPosition = Math.Max(0, ctrl.DataTable.DataColumnCount - ctrl.SelectedDataColumns.Count);

        ctrl.DataTable.ChangeColumnPosition(ctrl.SelectedDataColumns, nPosition);
      }

      if (ctrl.SelectedPropertyColumns.Count > 0)
      {
        if (ctrl.SelectedPropertyColumns.Count + nPosition > ctrl.DataTable.PropertyColumnCount)
          nPosition = Math.Max(0, ctrl.DataTable.PropertyColumnCount - ctrl.SelectedDataColumns.Count);

        ctrl.DataTable.PropertyColumns.ChangeColumnPosition(ctrl.SelectedPropertyColumns, nPosition);
      }

      ctrl.ClearAllSelections();

      ctrl.TableAreaInvalidate();
    }

    #endregion Set column position

    #region Set column to X, Value, Label...

    /// <summary>
    /// Sets the column kind of the first selected column or property column to a X column.
    /// </summary>
    public static void SetSelectedColumnAsX(IWorksheetController ctrl)
    {
      bool bChanged = false;
      if (ctrl.SelectedDataColumns.Count > 0)
      {
        ctrl.DataTable.DataColumns.SetColumnKind(ctrl.SelectedDataColumns[0], Altaxo.Data.ColumnKind.X);
        bChanged = true;
      }
      if (ctrl.SelectedPropertyColumns.Count > 0)
      {
        ctrl.DataTable.PropertyColumns.SetColumnKind(ctrl.SelectedPropertyColumns[0], Altaxo.Data.ColumnKind.X);
        bChanged = true;
      }
      if (bChanged)
        ctrl.TableAreaInvalidate(); // draw new because
    }

    /// <summary>
    /// Sets the column kind of the first selected column or property column to a X column.
    /// </summary>
    public static void SetSelectedColumnAsY(IWorksheetController ctrl)
    {
      bool bChanged = false;
      if (ctrl.SelectedDataColumns.Count > 0)
      {
        ctrl.DataTable.DataColumns.SetColumnKind(ctrl.SelectedDataColumns[0], Altaxo.Data.ColumnKind.Y);
        bChanged = true;
      }
      if (ctrl.SelectedPropertyColumns.Count > 0)
      {
        ctrl.DataTable.PropertyColumns.SetColumnKind(ctrl.SelectedPropertyColumns[0], Altaxo.Data.ColumnKind.Y);
        bChanged = true;
      }
      if (bChanged)
        ctrl.TableAreaInvalidate(); // draw new because
    }

    /// <summary>
    /// Sets the column kind of all selected columns or property columns to a label column.
    /// </summary>
    public static void SetSelectedColumnAsLabel(IWorksheetController ctrl)
    {
      bool bChanged = false;
      if (ctrl.SelectedDataColumns.Count > 0)
      {
        for (int i = 0; i < ctrl.SelectedDataColumns.Count; i++)
          ctrl.DataTable.DataColumns.SetColumnKind(ctrl.SelectedDataColumns[i], Altaxo.Data.ColumnKind.Label);
        bChanged = true;
      }
      if (ctrl.SelectedPropertyColumns.Count > 0)
      {
        for (int i = 0; i < ctrl.SelectedPropertyColumns.Count; i++)
          ctrl.DataTable.PropertyColumns.SetColumnKind(ctrl.SelectedPropertyColumns[i], Altaxo.Data.ColumnKind.Label);
        bChanged = true;
      }

      if (bChanged)
        ctrl.TableAreaInvalidate(); // draw new because
    }

    /// <summary>
    /// Sets the column kind of the first selected column to a value column.
    /// </summary>
    public static void SetSelectedColumnAsValue(IWorksheetController ctrl)
    {
      bool bChanged = false;
      if (ctrl.SelectedDataColumns.Count > 0)
      {
        for (int i = 0; i < ctrl.SelectedDataColumns.Count; i++)
          ctrl.DataTable.DataColumns.SetColumnKind(ctrl.SelectedDataColumns[i], Altaxo.Data.ColumnKind.V);
        bChanged = true;
      }
      if (ctrl.SelectedPropertyColumns.Count > 0)
      {
        for (int i = 0; i < ctrl.SelectedPropertyColumns.Count; i++)
          ctrl.DataTable.PropertyColumns.SetColumnKind(ctrl.SelectedPropertyColumns[i], Altaxo.Data.ColumnKind.V);
        bChanged = true;
      }
      if (bChanged)
        ctrl.TableAreaInvalidate(); // draw new because
    }

    /// <summary>
    /// Sets the column kind of the first selected column to the specified column kind
    /// </summary>
    public static void SetSelectedColumnAsKind(IWorksheetController ctrl, Altaxo.Data.ColumnKind kind)
    {
      bool bChanged = false;
      if (ctrl.SelectedDataColumns.Count > 0)
      {
        for (int i = 0; i < ctrl.SelectedDataColumns.Count; i++)
          ctrl.DataTable.DataColumns.SetColumnKind(ctrl.SelectedDataColumns[i], kind);
        bChanged = true;
      }
      if (ctrl.SelectedPropertyColumns.Count > 0)
      {
        for (int i = 0; i < ctrl.SelectedPropertyColumns.Count; i++)
          ctrl.DataTable.PropertyColumns.SetColumnKind(ctrl.SelectedPropertyColumns[i], kind);
        bChanged = true;
      }
      if (bChanged)
        ctrl.TableAreaInvalidate(); // draw new because
    }

    #endregion Set column to X, Value, Label...

    #region Extract property values

    /// <summary>
    /// Extracts the property values of the selected property columns.
    /// </summary>
    /// <param name="ctrl">The controller that controls the table.</param>
    public static void ExtractPropertyValues(IWorksheetController ctrl)
    {
      for (int i = 0; i < ctrl.SelectedPropertyColumns.Count; i++)
      {
        Altaxo.Data.DataColumn col = ctrl.DataTable.PropCols[ctrl.SelectedPropertyColumns[i]];
        ExtractPropertiesFromColumn(col, ctrl.DataTable.PropCols);
      }
      ctrl.ClearAllSelections();
    }

    /// <summary>
    /// This function searches for patterns like aaa=bbb in the items of the text column. If it finds such a item, it creates a column named aaa
    /// and stores the value bbb at the same position in it as in the text column.
    /// </summary>
    /// <param name="col">The column where to search for the patterns described above.</param>
    /// <param name="store">The column collection where to store the newly created columns of properties.</param>
    public static void ExtractPropertiesFromColumn(Altaxo.Data.DataColumn col, Altaxo.Data.DataColumnCollection store)
    {
      for (int nRow = 0; nRow < col.Count; nRow++)
      {
        ExtractPropertiesFromString(col[nRow].ToString(), store, nRow);
      }
    }

    /// <summary>
    /// This function searches for patterns like aaa=bbb in the provided string. If it finds such a item, it creates a column named aaa
    /// and stores the value bbb at the same position in it as in the text column.
    /// </summary>
    /// <param name="strg">The string where to search for the patterns described above.</param>
    /// <param name="store">The column collection where to store the newly created columns of properties.</param>
    /// <param name="index">The index into the column where to store the property value.</param>
    public static void ExtractPropertiesFromString(string strg, Altaxo.Data.DataColumnCollection store, int index)
    {
      string pat;
      pat = @"(\S+)=(\S+)";

      var r = new Regex(pat, RegexOptions.Compiled | RegexOptions.IgnoreCase);

      for (Match m = r.Match(strg); m.Success; m = m.NextMatch())
      {
        string propname = m.Groups[1].ToString();
        string propvalue = m.Groups[2].ToString();

        // System.Diagnostics.Trace.WriteLine("Found the pair " + propname + " : " + propvalue);

        if (!store.ContainsColumn(propname))
        {
          Altaxo.Data.DataColumn col;
          if (Altaxo.Serialization.DateTimeParsing.IsDateTime(propvalue))
            col = new Altaxo.Data.DateTimeColumn();
          else if (Altaxo.Serialization.NumberConversion.IsNumeric(propvalue))
            col = new Altaxo.Data.DoubleColumn();
          else
            col = new Altaxo.Data.TextColumn();

          store.Add(col, propname); // add the column to the collection
        }

        // now the column is present we can store the value in it.
        store[propname][index] = new Altaxo.Data.AltaxoVariant(propvalue);
      }
    }

    #endregion Extract property values

    #region Set column values

    /*
    public static void SetColumnValues(IWorksheetController ctrl)
    {
      if(ctrl.SelectedDataColumns.Count<=0)
        return; // no column selected

      Altaxo.Data.DataColumn dataCol = ctrl.DataTable[ctrl.SelectedDataColumns[0]];
      if(null==dataCol)
        return;

      //Data.ColumnScript colScript = (Data.ColumnScript)altaxoDataGrid1.columnScripts[dataCol];

      Data.ColumnScript colScript = (Data.ColumnScript)(ctrl.DataTable.DataColumns.ColumnScripts[dataCol]);

      DialogFactory.ShowColumnScriptDialog(ctrl.View.TableViewForm,ctrl.DataTable,dataCol,colScript);
    }
     */

    #endregion Set column values
  }
}
