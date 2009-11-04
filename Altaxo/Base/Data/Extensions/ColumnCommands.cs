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
			if (null == parent)
			{
				Current.Gui.ErrorMessageBox("Can not rename column since it is not a member of a DataColumnCollection");
				return false;
			}

			TextValueInputController tvctrl = new TextValueInputController(col.Name, "new column name:");
			tvctrl.Validator = new ColumnRenameValidator(col, parent);
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
			Altaxo.Data.DataColumn _col;
			Altaxo.Data.DataColumnCollection _parent;

			public ColumnRenameValidator(Altaxo.Data.DataColumn col, Altaxo.Data.DataColumnCollection parent)
				: base("The column name must not be empty! Please enter a valid name.")
			{
				_col = col;
				_parent = parent;
			}

			public override string Validate(string name)
			{
				string err = base.Validate(name);
				if (null != err)
					return err;

				if (_col.Name == name)
					return null;
				else if (_parent.ContainsColumn(name))
					return "This column name already exists, please choose another name!";
				else
					return null;

			}
		}

		#endregion


		#region Set group number

		/// <summary>
		/// Sets the group number of the currently selected columns to <code>nGroup</code>.
		/// </summary>
		/// <param name="ctrl">The worksheet controller.</param>
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
		/// <param name="ctrl">The worksheet controller for the table.</param>
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

				IntegerValueInputController ivictrl = new IntegerValueInputController(grpNumber, "Please enter a group number (>=0):");
				ivictrl.Validator = new IntegerValueInputController.ZeroOrPositiveIntegerValidator();
				if (Current.Gui.ShowDialog(ivictrl, "Set group number", false))
				{
					SetColumnGroupNumber(dataTable, selectedDataColumns, selectedPropColumns, ivictrl.EnteredContents);
					return true;
				}
			}
			return false;
		}


		
		#endregion


	}
}
