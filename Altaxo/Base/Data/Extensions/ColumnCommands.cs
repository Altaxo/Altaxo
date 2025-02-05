﻿#region Copyright

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
using Altaxo.Collections;
using Altaxo.Gui.Common;

namespace Altaxo.Data
{
  public static class ColumnCommands
  {
    #region Rename column

    /// <summary>
    /// Asks for a new name and then renames the selected data column or property column.
    /// </summary>
    /// <param name="col">The data column to rename.</param>
    public static bool ShowRenameColumnDialog(this DataColumn col)
    {
      var parent = col.ParentObject as DataColumnCollection;
      if (parent is null)
      {
        Current.Gui.ErrorMessageBox("Can not rename column since it is not a member of a DataColumnCollection");
        return false;
      }

      var tvctrl = new TextValueInputController(col.Name, "new column name:")
      {
        Validator = new ColumnRenameValidator(col, parent)
      };
      if (Current.Gui.ShowDialog(tvctrl, "Rename column", false))
      {
        parent.SetColumnName(col, tvctrl.InputText);
        return true;
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Helper class to make sure that user choosen data column name does not already exists.
    /// </summary>
    private class ColumnRenameValidator : TextValueInputController.NonEmptyStringValidator
    {
      private Altaxo.Data.DataColumn _col;
      private Altaxo.Data.DataColumnCollection _parent;

      public ColumnRenameValidator(Altaxo.Data.DataColumn col, Altaxo.Data.DataColumnCollection parent)
        : base("The column name must not be empty! Please enter a valid name.")
      {
        _col = col;
        _parent = parent;
      }

      public override string? Validate(string name)
      {
        var err = base.Validate(name);
        if (!string.IsNullOrEmpty(err))
          return err;

        if (_col.Name == name)
          return null;
        else if (_parent.ContainsColumn(name))
          return "This column name already exists, please choose another name!";
        else
          return null;
      }
    }

    #endregion Rename column

    #region Set group number

    /// <summary>
    /// Sets the group number of the currently selected columns to <code>nGroup</code>.
    /// </summary>
    /// <param name="datacoll">The data column collection to which to apply this procedure.</param>
    /// <param name="selected">Collection of column indices for which to set the group number.</param>
    /// <param name="nGroup">The group number to set for the selected columns.</param>
    public static void SetColumnGroupNumber(this DataColumnCollection datacoll, IAscendingIntegerCollection selected, int nGroup)
    {
      for (int i = 0; i < selected.Count; i++)
      {
        datacoll.SetColumnGroup(selected[i], nGroup);
      }
    }

    public static void SetColumnGroupNumber(
      this DataTable dataTable,
      IAscendingIntegerCollection selectedDataColumns,
      IAscendingIntegerCollection selectedPropColumns,
      int nGroup)
    {
      SetColumnGroupNumber(dataTable.DataColumns, selectedDataColumns, nGroup);
      SetColumnGroupNumber(dataTable.PropCols, selectedPropColumns, nGroup);
    }

    /// <summary>
    /// Sets the group number of the selected column
    /// </summary>
    /// <param name="dataTable">The data table</param>
    /// <param name="selectedDataColumns">Indices of the currently selected data column of the table</param>
    /// <param name="selectedPropColumns">Indices of the currently selected property columns of the table.</param>
    public static bool ShowSetColumnGroupNumberDialog(this DataTable dataTable,
      IAscendingIntegerCollection selectedDataColumns,
      IAscendingIntegerCollection selectedPropColumns)
    {
      if (selectedDataColumns.Count > 0 || selectedPropColumns.Count > 0)
      {
        int grpNumber = 0;
        if (selectedDataColumns.Count > 0)
          grpNumber = dataTable.DataColumns.GetColumnGroup(selectedDataColumns[0]);
        else if (selectedPropColumns.Count > 0)
          grpNumber = dataTable.PropertyColumns.GetColumnGroup(selectedPropColumns[0]);

        var ivictrl = new IntegerValueInputController(grpNumber, "Please enter a group number (>=0):")
        {
          Validator = new IntegerValueInputController.ZeroOrPositiveIntegerValidator()
        };
        if (Current.Gui.ShowDialog(ivictrl, "Set group number", false))
        {
          SetColumnGroupNumber(dataTable, selectedDataColumns, selectedPropColumns, ivictrl.EnteredContents);
          return true;
        }
      }
      return false;
    }

    #endregion Set group number

    #region Get all columns of a group

    /// <summary>
    /// Gets the indices of all columns that have the group number given by the argument.
    /// </summary>
    /// <param name="datacoll">The data column collection to which to apply this procedure.</param>
    /// <param name="nGroup">The group number to search for.</param>
    /// <returns>The indices of all columns that have the group number given by the argument.</returns>
    public static IAscendingIntegerCollection GetColumnIndicesWithGroupNumber(this DataColumnCollection datacoll, int nGroup)
    {
      var result = new AscendingIntegerCollection();
      for (int i = 0; i < datacoll.ColumnCount; ++i)
      {
        if (datacoll.GetColumnGroup(i) == nGroup)
        {
          result.Add(i);
        }
      }
      return result;
    }

    #endregion
  }
}
