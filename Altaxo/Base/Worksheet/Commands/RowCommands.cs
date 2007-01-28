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
using System.Text.RegularExpressions;
using Altaxo.Worksheet.GUI;
using Altaxo.Gui.Common;

namespace Altaxo.Worksheet.Commands
{
  /// <summary>
  /// Contains static functions for handling row commands.
  /// </summary>
  public class RowCommands
  {

    #region Set row position
    /// <summary>
    /// Moves the selected row(s) to a new position. The new position must be entered by the user.
    /// </summary>
    /// <param name="ctrl">The worksheet controller for the table.</param>
    public static string SetSelectedRowPosition(WorksheetController ctrl)
    {
      if(ctrl.SelectedDataRows.Count==0)
        return null; // nothing to do

      int newposition = int.MinValue;
        
      IntegerValueInputController ivictrl = new IntegerValueInputController(0,"Please enter the new position (>=0):");
      

      ivictrl.Validator = new IntegerValueInputController.ZeroOrPositiveIntegerValidator();
      if(Current.Gui.ShowDialog(ivictrl,"New row position",false))
      {
        newposition = ivictrl.EnteredContents;
      }
      else
        return null;

      SetSelectedRowPosition(ctrl,newposition);

      return null;
    }


    /// <summary>
    /// Moves the selected rows to a new position <code>nPosition</code>.
    /// </summary>
    /// <param name="ctrl">The worksheet controller.</param>
    /// <param name="nPosition">The new position for the selected rows.</param>
    public static void SetSelectedRowPosition(WorksheetController ctrl, int nPosition)
    {
      if(ctrl.SelectedDataRows.Count>0)
      {
        ctrl.DataTable.DataColumns.ChangeRowPosition(ctrl.SelectedDataRows, nPosition);
      }

      ctrl.ClearAllSelections();

      ctrl.UpdateTableView();
    }
    #endregion

    /// <summary>
    /// Insert a number of data rows into the controlled table.
    /// </summary>
    /// <param name="ctrl">The worksheet controller.</param>
    /// <param name="rowBeforeToInsert">Number of the row before which to insert the additional rows.</param>
    /// <param name="numberOfRows">Number of rows to insert.</param>
    public static void InsertDataRows(WorksheetController ctrl, int rowBeforeToInsert, int numberOfRows)
    {
      if (numberOfRows <= 0 || rowBeforeToInsert<0)
        return;

      ctrl.Doc.DataColumns.InsertRows(rowBeforeToInsert, numberOfRows);
      ctrl.ClearAllSelections();
      ctrl.UpdateTableView();
    }

    /// <summary>
    /// Ask the user for the number of data rows to insert in a data table.
    /// </summary>
    /// <param name="ctrl">The worksheet controller.</param>
    /// <param name="rowBeforeToInsert">Number of the row before which to insert the rows.</param>
    public static void InsertDataRows(WorksheetController ctrl,int rowBeforeToInsert)
    {
      // ask for the number of rows to insert
      Altaxo.Gui.Common.IntegerValueInputController ictrl = new IntegerValueInputController(1, "Enter the number of rows to insert:");
      if (Current.Gui.ShowDialog(ictrl, "Insert rows", false))
        InsertDataRows(ctrl, rowBeforeToInsert, ictrl.EnteredContents);
    }

    /// <summary>
    /// Inserts a single data row in a position just before the first selected row. 
    /// If no row is selected, the row is inserted before the first row.
    /// </summary>
    /// <param name="ctrl">The worksheet controller.</param>
    public static void InsertOneDataRow(WorksheetController ctrl)
    {
      if (ctrl.SelectedDataRows.Count > 0)
        InsertDataRows(ctrl, ctrl.SelectedDataRows[0], 1);
      else
        InsertDataRows(ctrl, 0, 1);
    }

    /// <summary>
    /// Inserts a user choosen number of rows just before the first selected row.
    /// If no row is selected, the row is inserted before the first row.
    /// </summary>
    /// <param name="ctrl">The worksheet controller.</param>
    public static void InsertDataRows(WorksheetController ctrl)
    {
      if (ctrl.SelectedDataRows.Count > 0)
        InsertDataRows(ctrl, ctrl.SelectedDataRows[0]);
      else
        InsertDataRows(ctrl, 0);
    }
  }
}
