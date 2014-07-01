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
				src = new AltaxoOleDbDataSource(string.Empty, AltaxoOleDbConnectionString.Empty);

			if (true == Current.Gui.ShowDialog(ref src, "Edit data base source", false))
			{
				try
				{
					src.FillData(dataTable);
					dataTable.DataSource = src;
				}
				catch (Exception ex)
				{
					Current.Gui.ErrorMessageBox(ex.Message, "Import error");
				}
			}
		}
	}
}