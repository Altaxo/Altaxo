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
//    along with ctrl program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Addins.OriginConnector;
using Altaxo.Gui;
using Altaxo.Gui.AddInItems;

namespace Altaxo.Worksheet.Commands
{
  public class SendTableToOrigin : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      var comm = new OriginConnection();
      comm.Connect(true);
      comm.PutTable(ctrl.DataTable, false);
      comm.Disconnect(false, null, false);
    }
  }

  public class GetTableFromOrigin : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      var comm = new OriginConnection();
      comm.Connect(true);
      if (!comm.IsConnected())
        return;

      string err = comm.GetTable(ctrl.DataTable.Name, ctrl.DataTable);
      comm.Disconnect(false, null, false);

      if (err is not null)
      {
        Current.Gui.ErrorMessageBox(err);
        return;
      }
    }
  }

  public class GetAllTablesFromOrigin : SimpleCommand
  {
    public override void Execute(object parameter)
    {
      var conn = new OriginConnection();
      conn.Connect(true);
      if (!conn.IsConnected())
        return;

      var list = new List<Tuple<string, Origin.WorksheetPage>>();
      ListAllWorksheetsInFolderAndSubfolders(conn.Application.RootFolder, string.Empty, list);

      int count = conn.Application.WorksheetPages.Count;
      for (int i = 0; i < list.Count; i++)
      {
        var path = list[i].Item1;
        var page = list[i].Item2;
        string name = page.Name;
        string lname = page.LongName;

        var newTable = new Altaxo.Data.DataTable
        {
          Name = path + (string.IsNullOrEmpty(lname) ? name : lname)
        };
        string err = WorksheetActions.GetTable(page, newTable);
        if (err is null)
        {
          Current.ProjectService.CreateNewWorksheet(newTable);
        }
        else
        {
          Current.Gui.ErrorMessageBox(err);
        }
      }

      conn.Disconnect(false, null, false);
    }

    /// <summary>
    /// Collects a lists of all worksheets in the provided folder and its subfolders.
    /// </summary>
    /// <param name="folder">The folder to begin the collection with.</param>
    /// <param name="path">The current path name of the provided <paramref name="folder"/>.</param>
    /// <param name="list">The list used to collect all worksheets.</param>
    private void ListAllWorksheetsInFolderAndSubfolders(Origin.Folder folder, string path, List<Tuple<string, Origin.WorksheetPage>> list)
    {
      for (int i = 0; i < folder.PageBases.Count; ++i)
      {
        var page = folder.PageBases[i];
        if (page.Type == (int)Origin.PAGETYPES.OPT_WORKSHEET)
          list.Add(new Tuple<string, Origin.WorksheetPage>(path, page as Origin.WorksheetPage));
      }

      for (int i = 0; i < folder.Folders.Count; ++i)
      {
        var newPath = path + (!string.IsNullOrEmpty(folder.Folders[i].LongName) ? folder.Folders[i].LongName : folder.Folders[i].Name) + '\\';
        ListAllWorksheetsInFolderAndSubfolders(folder.Folders[i], newPath, list);
      }
    }
  }

  public class PushAllTablesToOrigin : SimpleCommand
  {
    public override void Execute(object parameter)
    {
      var conn = new OriginConnection();
      conn.Connect(true);
      if (!conn.IsConnected())
        return;

      foreach (var table in Current.Project.DataTableCollection)
      {
        conn.PutTable(table, false);
      }

      conn.Disconnect(false, null, false);
    }
  }
}
