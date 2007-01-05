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
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Commands;

namespace Altaxo.Main.Commands
{
  public static class WindowCommands
  {
    public class GraphTableCreationDateComparer : IComparer
    {
      #region IComparer Members

      public int Compare(object x, object y)
      {
        DateTime dx, dy;
        if (x is Data.DataTable)
          dx = ((Data.DataTable)x).CreationTimeUtc;
        else if (x is Graph.Gdi.GraphDocument)
          dx = ((Graph.Gdi.GraphDocument)x).CreationTimeUtc;
        else
          dx = DateTime.MinValue;

        if (y is Data.DataTable)
          dy = ((Data.DataTable)y).CreationTimeUtc;
        else if (y is Graph.Gdi.GraphDocument)
          dy = ((Graph.Gdi.GraphDocument)y).CreationTimeUtc;
        else dy = DateTime.MinValue;

        return dx.CompareTo(dy);
      }

      #endregion
    }
    public class GraphTableNameComparer : IComparer
    {
      #region IComparer Members

      public int Compare(object x, object y)
      {
        string dx, dy;
        if (x is Data.DataTable)
          dx = ((Data.DataTable)x).Name;
        else if (x is Graph.Gdi.GraphDocument)
          dx = ((Graph.Gdi.GraphDocument)x).Name;
        else
          dx = string.Empty;

        if (y is Data.DataTable)
          dy = ((Data.DataTable)y).Name;
        else if (y is Graph.Gdi.GraphDocument)
          dy = ((Graph.Gdi.GraphDocument)y).Name;
        else dy = string.Empty;

        return dx.CompareTo(dy);
      }

      #endregion
    }
    public class GraphTableTypeComparer : IComparer
    {
      #region IComparer Members

      public int Compare(object x, object y)
      {
        string dx, dy;
        if (x is Data.DataTable)
          dx = "AAA";
        else if (x is Graph.Gdi.GraphDocument)
          dx = "AAB";
        else
          dx = x.GetType().ToString();

        if (y is Data.DataTable)
          dy = "AAA";
        else if (y is Graph.Gdi.GraphDocument)
          dy = "AAB";
        else dy = y.GetType().ToString();

        return dx.CompareTo(dy);
      }

      #endregion
    }
    public class GraphTableMultiComparer : IComparer
    {
      IComparer _c1, _c2;
      public GraphTableMultiComparer(IComparer c1, IComparer c2)
      {
        _c1 = c1;
        _c2 = c2;
      }
      
      #region IComparer Members

      public int Compare(object x, object y)
      {
        int result = _c1.Compare(x, y);
        if (0 == result)
          result = _c2.Compare(x, y);
        return result;
      }

      #endregion
    }


    public static void GetAllTables(ArrayList arr)
    {
      foreach (Data.DataTable table in Current.Project.DataTableCollection)
        arr.Add(table);
    }

    public static void GetNonOpenTables(ArrayList arr)
    {
      foreach (Data.DataTable table in Current.Project.DataTableCollection)
        if(!Current.ProjectService.HasDocumentAnOpenView(table))
        arr.Add(table);
    }

    public static void GetAllGraphDocuments(ArrayList arr)
    {
      foreach (Graph.Gdi.GraphDocument graph in Current.Project.GraphDocumentCollection)
        arr.Add(graph);
    }

    public static void GetNonOpenGraphDocuments(ArrayList arr)
    {
      foreach (Graph.Gdi.GraphDocument graph in Current.Project.GraphDocumentCollection)
        if (!Current.ProjectService.HasDocumentAnOpenView(graph))
          arr.Add(graph);
    }

    public static void OpenNonOpenGraphsAndTables(bool bGraphs, bool bTables, IComparer comparer)
    {
      ArrayList arr = new ArrayList();
      if(bGraphs)
        WindowCommands.GetNonOpenGraphDocuments(arr);
      if(bTables)
        WindowCommands.GetNonOpenTables(arr);
      arr.Sort(comparer);

      foreach (object tab in arr)
      {
        if (tab is Data.DataTable)
          Current.ProjectService.OpenOrCreateWorksheetForTable((Data.DataTable)tab);
        else if (tab is Graph.Gdi.GraphDocument)
          Current.ProjectService.OpenOrCreateGraphForGraphDocument((Graph.Gdi.GraphDocument)tab);
      }
    }

    public static void CloseAllGraphsAndTables()
    {
      Current.Workbench.CloseAllViews();
    }

  }

  public class OpenAllWorksheets : AbstractCommand
  {
    public override void Run()
    {
      WindowCommands.OpenNonOpenGraphsAndTables(false, true,
        new WindowCommands.GraphTableCreationDateComparer());
    }
  }

  public class OpenAllGraphs : AbstractCommand
  {

    public override void Run()
    {
      WindowCommands.OpenNonOpenGraphsAndTables(true, false,
        new WindowCommands.GraphTableCreationDateComparer());
    }
  }

  public class OpenAllWorksheetsAndGraphs : AbstractCommand
  {
    public override void Run()
    {
      WindowCommands.OpenNonOpenGraphsAndTables(true, true,
        new WindowCommands.GraphTableCreationDateComparer());
    }
  }

  public class SortGraphTablesByTypeAndName : AbstractCommand
  {
    public override void Run()
    {
      WindowCommands.CloseAllGraphsAndTables();
      WindowCommands.OpenNonOpenGraphsAndTables(true, true,
        new WindowCommands.GraphTableMultiComparer(
          new WindowCommands.GraphTableTypeComparer(),
          new WindowCommands.GraphTableNameComparer()
      ));
    }
  }

  public class SortGraphTablesByTypeAndCreationTime : AbstractCommand
  {
    public override void Run()
    {
      WindowCommands.CloseAllGraphsAndTables();
      WindowCommands.OpenNonOpenGraphsAndTables(true, true,
        new WindowCommands.GraphTableMultiComparer(
          new WindowCommands.GraphTableTypeComparer(),
          new WindowCommands.GraphTableCreationDateComparer()
      ));
    }
  }


  public class SortGraphTablesByCreationTime : AbstractCommand
  {
    public override void Run()
    {
      WindowCommands.CloseAllGraphsAndTables();
      WindowCommands.OpenNonOpenGraphsAndTables(true, true, 
        new WindowCommands.GraphTableCreationDateComparer());
    }
  }

  public class SortGraphTablesByName : AbstractCommand
  {
    public override void Run()
    {
      WindowCommands.CloseAllGraphsAndTables();
      WindowCommands.OpenNonOpenGraphsAndTables(true, true, 
        new WindowCommands.GraphTableNameComparer());
    }
  }



}
