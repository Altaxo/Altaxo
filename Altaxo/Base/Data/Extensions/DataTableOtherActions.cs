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
			TextValueInputController tvctrl = new TextValueInputController(table.Name, "Enter a name for the worksheet:");
			tvctrl.Validator = new WorksheetRenameValidator(table);
			if (Current.Gui.ShowDialog(tvctrl, "Rename worksheet", false))
				table.Name = tvctrl.InputText.Trim();
		}

		private class WorksheetRenameValidator : TextValueInputController.NonEmptyStringValidator
		{
			Altaxo.Data.DataTable _table;

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
				else if (Data.DataTableCollection.GetParentDataTableCollectionOf(_table).Contains(wksname))
					return "This worksheet name already exists, please choose another name!";
				else
					return null;
			}
		}
    



	}
}
