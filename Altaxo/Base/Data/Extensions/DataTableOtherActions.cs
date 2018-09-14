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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Data
{
  using Altaxo.Gui.Common;

  /// <summary>
  /// Other commonly used actions on a <see cref="DataTable"/>.
  /// </summary>
  public static class DataTableOtherActions
  {
    /// <summary>
    /// Shows a dialog to rename the table.
    /// </summary>
    /// <param name="table">The table to rename.</param>
    public static void ShowRenameDialog(this DataTable table)
    {
      var tvctrl = new TextValueInputController(table.Name, "Enter a name for the worksheet:")
      {
        Validator = new WorksheetRenameValidator(table)
      };
      if (Current.Gui.ShowDialog(tvctrl, "Rename worksheet", false))
        table.Name = tvctrl.InputText.Trim();
    }

    private class WorksheetRenameValidator : TextValueInputController.NonEmptyStringValidator
    {
      private Altaxo.Data.DataTable _table;

      public WorksheetRenameValidator(DataTable table)
        : base("The worksheet name must not be empty! Please enter a valid name.")
      {
        _table = table;
      }

      public override string Validate(string wksname)
      {
        string err = base.Validate(wksname);
        if (null != err)
          return err;

        if (_table.Name == wksname)
          return null;
        else if (Data.DataTableCollection.GetParentDataTableCollectionOf(_table) == null)
          return null; // if there is no parent data set we can enter anything
        else if (Data.DataTableCollection.GetParentDataTableCollectionOf(_table).ContainsAnyName(wksname))
          return "This worksheet name already exists, please choose another name!";
        else
          return null;
      }
    }
  }
}
