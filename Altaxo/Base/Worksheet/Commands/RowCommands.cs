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
        
      IntegerValueInputController ivictrl = new IntegerValueInputController(
        0,
        new SingleValueDialog("New row position","Please enter the new position (>=0):")
        );

      ivictrl.Validator = new IntegerValueInputController.ZeroOrPositiveIntegerValidator();
      if(ivictrl.ShowDialog(ctrl.View.TableViewForm))
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


  }
}
