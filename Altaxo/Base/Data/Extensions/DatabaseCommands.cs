using Altaxo.DataConnection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Data
{
	public static class DatabaseCommands
	{
		public static void ShowImportDatabaseDialog(this DataTable dataTable)
		{
			var src = dataTable.DataSource as AltaxoOleDbDataSource;

			if (null == src)
				src = new AltaxoOleDbDataSource(string.Empty, string.Empty);

			var ctrl = new Altaxo.Gui.DataConnection.ConnectionMainController(src);
			if (true == Current.Gui.ShowDialog(ctrl, "Connection main view", false))
			{
				try
				{
					var query = new Altaxo.DataConnection.AltaxoOleDbDataSource(ctrl.SelectionStatement, ctrl.ConnectionString);
					query.FillData(dataTable);
					dataTable.DataSource = query;
				}
				catch (Exception ex)
				{
					Current.Gui.ErrorMessageBox(ex.Message, "Import error");
				}
			}
		}
	}
}