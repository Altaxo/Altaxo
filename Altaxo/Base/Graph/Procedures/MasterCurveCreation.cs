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

using Altaxo.Data;
using Altaxo.Graph.Gdi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Procedures
{
  /// <summary>
  /// Commands for master curve creation, beginning with a data plot. Usually the plot should contain 1 or 2 plot groups. The items in those groups will be used as data groups for master curve creation.
  /// </summary>
  public static class MasterCurveCreation
  {
    public static void ShowMasterCurveCreationDialog(GraphDocument doc)
    {
      string error;
      Altaxo.Data.MasterCurveCreation.Options opt = new Data.MasterCurveCreation.Options();
      if (null != (error = FillDataListFromGraphDocument(doc, opt.ColumnGroups)))
      {
        Current.Gui.ErrorMessageBox(error);
        return;
      }

      Current.Gui.ShowDialog(ref opt, "Create master curve", false);
    }

    /// <summary>
    /// Tries to extract from the graph document
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="groupList"></param>
    /// <returns></returns>
    private static string FillDataListFromGraphDocument(GraphDocument doc, List<List<DoubleColumn>> groupList)
    {
      if (doc.RootLayer.Layers.Count == 0)
        return "Plot contains no layers and therefore no plot items";

      for (int i = 0; i < doc.RootLayer.Layers.Count; i++)
      {
        var xylayer = doc.RootLayer.Layers[i] as XYPlotLayer;
        if (null != xylayer)
          FillDataListFromLayer(xylayer, groupList);
      }

      return null;
    }

    private static void FillDataListFromLayer(XYPlotLayer layer, List<List<DoubleColumn>> groupList)
    {
      var plotItems = layer.PlotItems;
      if (plotItems.Count == 0)
        return;

      if (plotItems[0] is Altaxo.Graph.Gdi.Plot.PlotItemCollection)
      {
        var list = GetColumnListFromPlotItemCollection((Altaxo.Graph.Gdi.Plot.PlotItemCollection)plotItems[0]);
        if (list.Count > 0)
          groupList.Add(list);
      }

      // additional 2nd group
      if (plotItems.Count >= 2 && plotItems[1] is Altaxo.Graph.Gdi.Plot.PlotItemCollection)
      {
        var list = GetColumnListFromPlotItemCollection((Altaxo.Graph.Gdi.Plot.PlotItemCollection)plotItems[1]);
        if (list.Count > 0)
          groupList.Add(list);
      }

      // if list is still empty, we try to flatten the root plot item
      if (groupList.Count == 0)
      {
        var list = GetColumnListFromPlotItemCollection(plotItems);
        if (list.Count > 0)
          groupList.Add(list);
      }
    }

    private static List<DoubleColumn> GetColumnListFromPlotItemCollection(Altaxo.Graph.Gdi.Plot.PlotItemCollection plotItemCollection)
    {
      var flattenedContent = plotItemCollection.Flattened;
      List<DoubleColumn> columnList = new List<DoubleColumn>();
      foreach (var item in flattenedContent)
      {
        if (item is Gdi.Plot.XYColumnPlotItem)
        {
          var yCol = ((Gdi.Plot.XYColumnPlotItem)item).XYColumnPlotData.YColumn;
          if (yCol is DoubleColumn)
            columnList.Add((DoubleColumn)yCol);
        }
      }
      return columnList;
    }
  }
}
