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
//    along with ctrl program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ICSharpCode.Core;
using Altaxo.Addins.OriginConnector;

namespace Altaxo.Worksheet.Commands
{
  public class SendTableToOrigin : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      var comm = new OriginConnection();
      comm.Connect(true);
      comm.PutTable(ctrl.DataTable, null, false);
      comm.Disconnect(false,null,false);
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

      if (err != null)
      {
        Current.Gui.ErrorMessageBox(err);
        return;
      }

    }
  }

  public class GetAllTablesFromOrigin : AbstractMenuCommand
  {
    public override void Run()
    {
      var conn = new OriginConnection();
      conn.Connect(true);
      if (!conn.IsConnected())
        return;

      int count = conn.Application.WorksheetPages.Count;
      for(int i=0;i<count;i++)
      {
        string name = conn.Application.WorksheetPages[i].Name;
        string lname = conn.Application.WorksheetPages[i].LongName;

        var newTable = new Altaxo.Data.DataTable();
        newTable.Name = lname;
        string err = conn.GetTable(name, newTable);
        if (null == err)
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
  }

  public class PushAllTablesToOrigin : AbstractMenuCommand
  {
    public override void Run()
    {
      var conn = new OriginConnection();
      conn.Connect(true);
      if (!conn.IsConnected())
        return;


      foreach (var table in Current.Project.DataTableCollection)
      {
        conn.PutTable(table, table.Name, false);
      }

      conn.Disconnect(false, null, false);
    }
  }
}
