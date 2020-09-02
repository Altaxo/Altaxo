#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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
using Altaxo.DataConnection;

namespace Altaxo.Data
{
  public static class DatabaseCommands
  {
    public static void ShowImportDatabaseDialog(this DataTable dataTable)
    {
      var src = dataTable.DataSource as AltaxoOleDbDataSource;

      if (dataTable.DataSource is not null && src is null)
      {
        if (false == Current.Gui.YesNoMessageBox(
          string.Format("There is a table data source (of type: {0}) already present for this table. Proceeding will override this data source. Do you want to continue?", dataTable.DataSource.GetType().Name),
          "Attention - risk of overriding table data source!",
          false))

          return;
      }

      if (src is null)
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
