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

#nullable disable warnings
using System;
using System.Collections;
using Altaxo.Gui;

namespace Altaxo.Main.Commands
{
  /// <summary>
  /// Provides helper methods and comparers for opening and sorting worksheet and graph windows.
  /// </summary>
  public static class WindowCommands
  {
    /// <summary>
    /// Compares worksheets and graphs by creation time.
    /// </summary>
    public class GraphTableCreationDateComparer : IComparer
    {
      #region IComparer Members

      /// <inheritdoc/>
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
        else
          dy = DateTime.MinValue;

        return dx.CompareTo(dy);
      }

      #endregion IComparer Members
    }

    /// <summary>
    /// Compares worksheets and graphs by name.
    /// </summary>
    public class GraphTableNameComparer : IComparer
    {
      #region IComparer Members

      /// <inheritdoc/>
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
        else
          dy = string.Empty;

        return dx.CompareTo(dy);
      }

      #endregion IComparer Members
    }

    /// <summary>
    /// Compares worksheets and graphs by type.
    /// </summary>
    public class GraphTableTypeComparer : IComparer
    {
      #region IComparer Members

      /// <inheritdoc/>
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
        else
          dy = y.GetType().ToString();

        return dx.CompareTo(dy);
      }

      #endregion IComparer Members
    }

    /// <summary>
    /// Combines two comparers into a multi-stage comparer.
    /// </summary>
    public class GraphTableMultiComparer : IComparer
    {
      private IComparer _c1, _c2;

      /// <summary>
      /// Initializes a new instance of the <see cref="GraphTableMultiComparer"/> class.
      /// </summary>
      /// <param name="c1">The primary comparer.</param>
      /// <param name="c2">The secondary comparer.</param>
      public GraphTableMultiComparer(IComparer c1, IComparer c2)
      {
        _c1 = c1;
        _c2 = c2;
      }

      #region IComparer Members

      /// <inheritdoc/>
      public int Compare(object x, object y)
      {
        int result = _c1.Compare(x, y);
        if (0 == result)
          result = _c2.Compare(x, y);
        return result;
      }

      #endregion IComparer Members
    }

    /// <summary>
    /// Adds all tables in the current project to the specified collection.
    /// </summary>
    /// <param name="arr">The collection to fill.</param>
    public static void GetAllTables(ArrayList arr)
    {
      foreach (Data.DataTable table in Current.Project.DataTableCollection)
        arr.Add(table);
    }

    /// <summary>
    /// Adds all tables without an open view to the specified collection.
    /// </summary>
    /// <param name="arr">The collection to fill.</param>
    public static void GetNonOpenTables(ArrayList arr)
    {
      foreach (Data.DataTable table in Current.Project.DataTableCollection)
        if (!Current.IProjectService.HasDocumentAnOpenView(table))
          arr.Add(table);
    }

    /// <summary>
    /// Adds all graph documents in the current project to the specified collection.
    /// </summary>
    /// <param name="arr">The collection to fill.</param>
    public static void GetAllGraphDocuments(ArrayList arr)
    {
      foreach (Graph.Gdi.GraphDocument graph in Current.Project.GraphDocumentCollection)
        arr.Add(graph);
    }

    /// <summary>
    /// Adds all graph documents without an open view to the specified collection.
    /// </summary>
    /// <param name="arr">The collection to fill.</param>
    public static void GetNonOpenGraphDocuments(ArrayList arr)
    {
      foreach (Graph.Gdi.GraphDocument graph in Current.Project.GraphDocumentCollection)
        if (!Current.IProjectService.HasDocumentAnOpenView(graph))
          arr.Add(graph);
    }

    /// <summary>
    /// Opens non-open graphs and tables and sorts them using the specified comparer.
    /// </summary>
    /// <param name="bGraphs"><see langword="true"/> to include graphs.</param>
    /// <param name="bTables"><see langword="true"/> to include tables.</param>
    /// <param name="comparer">The comparer used for sorting.</param>
    public static void OpenNonOpenGraphsAndTables(bool bGraphs, bool bTables, IComparer comparer)
    {
      var arr = new ArrayList();
      if (bGraphs)
        WindowCommands.GetNonOpenGraphDocuments(arr);
      if (bTables)
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

    /// <summary>
    /// Closes all open graph and worksheet views.
    /// </summary>
    public static void CloseAllGraphsAndTables()
    {
      Current.Workbench.CloseAllViews();
    }
  }

  /// <summary>
  /// Opens all worksheets.
  /// </summary>
  public class OpenAllWorksheets : SimpleCommand
  {
    /// <inheritdoc/>
    public override void Execute(object parameter)
    {
      WindowCommands.OpenNonOpenGraphsAndTables(false, true,
        new WindowCommands.GraphTableCreationDateComparer());
    }
  }

  /// <summary>
  /// Opens all graphs.
  /// </summary>
  public class OpenAllGraphs : SimpleCommand
  {
    /// <inheritdoc/>
    public override void Execute(object parameter)
    {
      WindowCommands.OpenNonOpenGraphsAndTables(true, false,
        new WindowCommands.GraphTableCreationDateComparer());
    }
  }

  /// <summary>
  /// Opens all worksheets and graphs.
  /// </summary>
  public class OpenAllWorksheetsAndGraphs : SimpleCommand
  {
    /// <inheritdoc/>
    public override void Execute(object parameter)
    {
      WindowCommands.OpenNonOpenGraphsAndTables(true, true,
        new WindowCommands.GraphTableCreationDateComparer());
    }
  }

  /// <summary>
  /// Reopens worksheets and graphs sorted by type and name.
  /// </summary>
  public class SortGraphTablesByTypeAndName : SimpleCommand
  {
    /// <inheritdoc/>
    public override void Execute(object parameter)
    {
      WindowCommands.CloseAllGraphsAndTables();
      WindowCommands.OpenNonOpenGraphsAndTables(true, true,
        new WindowCommands.GraphTableMultiComparer(
          new WindowCommands.GraphTableTypeComparer(),
          new WindowCommands.GraphTableNameComparer()
      ));
    }
  }

  /// <summary>
  /// Reopens worksheets and graphs sorted by type and creation time.
  /// </summary>
  public class SortGraphTablesByTypeAndCreationTime : SimpleCommand
  {
    /// <inheritdoc/>
    public override void Execute(object parameter)
    {
      WindowCommands.CloseAllGraphsAndTables();
      WindowCommands.OpenNonOpenGraphsAndTables(true, true,
        new WindowCommands.GraphTableMultiComparer(
          new WindowCommands.GraphTableTypeComparer(),
          new WindowCommands.GraphTableCreationDateComparer()
      ));
    }
  }

  /// <summary>
  /// Reopens worksheets and graphs sorted by creation time.
  /// </summary>
  public class SortGraphTablesByCreationTime : SimpleCommand
  {
    /// <inheritdoc/>
    public override void Execute(object parameter)
    {
      WindowCommands.CloseAllGraphsAndTables();
      WindowCommands.OpenNonOpenGraphsAndTables(true, true,
        new WindowCommands.GraphTableCreationDateComparer());
    }
  }

  /// <summary>
  /// Reopens worksheets and graphs sorted by name.
  /// </summary>
  public class SortGraphTablesByName : SimpleCommand
  {
    /// <inheritdoc/>
    public override void Execute(object parameter)
    {
      WindowCommands.CloseAllGraphsAndTables();
      WindowCommands.OpenNonOpenGraphsAndTables(true, true,
        new WindowCommands.GraphTableNameComparer());
    }
  }

  /// <summary>
  /// Toggles the main window between fullscreen and normal mode.
  /// </summary>
  public class ToggleFullscreenCommand : SimpleCommand
  {
    /// <inheritdoc/>
    public override void Execute(object parameter)
    {
      Current.Workbench.FullScreen = !Current.Workbench.FullScreen;
    }
  }
}
