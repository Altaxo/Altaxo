#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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

using System.Collections.Generic;

using Altaxo.Data;
using Altaxo.Collections;
using Altaxo.Graph.Gdi.CS;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Graph.Gdi.Plot.Data;
using Altaxo.Graph.Gdi.Plot.Groups;
using Altaxo.Graph.GUI;




namespace Altaxo.Worksheet.Commands
{
  /// <summary>
  /// PlotCommands contains methods for creating different plot types.
  /// </summary>
  public class PlotCommands
  {

    public static List<IGPlotItem> CreatePlotAssociations(DataTable table, IAscendingIntegerCollection selectedColumns, G2DPlotStyleCollection templatePlotStyle)
    {
      int len = selectedColumns.Count;
      List<IGPlotItem> result = new List<IGPlotItem>();

      for (int i = 0; i < len; i++)
      {
        Altaxo.Data.DataColumn ycol = table[selectedColumns[i]];
        Altaxo.Data.DataColumn xcol = table.DataColumns.FindXColumnOf(ycol);
        XYColumnPlotData pa;
        if (null != xcol)
          pa = new XYColumnPlotData(xcol, ycol);
        else
          pa = new XYColumnPlotData(new Altaxo.Data.IndexerColumn(), ycol);

        G2DPlotStyleCollection ps = templatePlotStyle!=null? (G2DPlotStyleCollection)templatePlotStyle.Clone() : new G2DPlotStyleCollection();

        // if the next column is a label column, add it also
        if ((i + 1) < len && ColumnKind.Label == table.DataColumns.GetColumnKind(selectedColumns[i + 1]))
        {
          LabelPlotStyle labelStyle = new LabelPlotStyle(table.DataColumns[i]);
          ps.Add(labelStyle);
          i++;
        }

        result.Add(new XYColumnPlotItem(pa,ps));
      }

      return result;
    }

    /// <summary>
    /// Plots selected data columns of a table.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="selectedColumns">The data columns of the table that should be plotted.</param>
    /// <param name="bLine">If true, the line style is activated (the points are connected by lines).</param>
    /// <param name="bScatter">If true, the scatter style is activated (the points are plotted as symbols).</param>
    public static IGraphController Plot(DataTable table, 
      IAscendingIntegerCollection selectedColumns,
       G2DPlotStyleCollection templatePlotStyle)
    {
      List<IGPlotItem> pilist = CreatePlotAssociations(table, selectedColumns, templatePlotStyle);

      // now create a new Graph with this plot associations
      Altaxo.Graph.GUI.IGraphController gc = Current.ProjectService.CreateNewGraph();
      PlotItemCollection newPlotGroup = new PlotItemCollection(gc.Doc.Layers[0].PlotItems);
      foreach (IGPlotItem pi in pilist)
      {
        newPlotGroup.Add(pi);
      }

      newPlotGroup.CollectStyles(newPlotGroup.GroupStyles);
      gc.Doc.Layers[0].PlotItems.Add(newPlotGroup);

      return gc;
    }



    /// <summary>
    /// Plots the currently selected data columns of a worksheet.
    /// </summary>
    /// <param name="dg">The worksheet controller where the columns are selected in.</param>
    /// <param name="bLine">If true, a line is plotted.</param>
    /// <param name="bScatter">If true, scatter symbols are plotted.</param>
    public static void PlotLine(GUI.WorksheetController dg, bool bLine, bool bScatter)
    {
      PlotLine(dg.DataTable,dg.SelectedDataColumns,bLine,bScatter);
    }

    /// <summary>
    /// Plots selected data columns of a table.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="selectedColumns">The data columns of the table that should be plotted.</param>
    /// <param name="bLine">If true, the line style is activated (the points are connected by lines).</param>
    /// <param name="bScatter">If true, the scatter style is activated (the points are plotted as symbols).</param>
    public static void PlotLine(DataTable table, Altaxo.Collections.IAscendingIntegerCollection selectedColumns, bool bLine, bool bScatter)
    {
      G2DPlotStyleCollection templatePlotStyle;
      if(bLine && bScatter)
        templatePlotStyle  = new G2DPlotStyleCollection(LineScatterPlotStyleKind.LineAndScatter);
      else if (bLine)
        templatePlotStyle = new G2DPlotStyleCollection(LineScatterPlotStyleKind.Line);
      else
        templatePlotStyle = new G2DPlotStyleCollection(LineScatterPlotStyleKind.Scatter);


      Plot(table, selectedColumns, templatePlotStyle);
    }


    /// <summary>
    /// Plots the currently selected data columns of a worksheet as horzizontal bar diagram.
    /// </summary>
    /// <param name="dg">The worksheet controller where the columns are selected in.</param>
    public static void PlotHorizontalBarGraph(GUI.WorksheetController dg)
    {
      G2DPlotStyleCollection templatePlotStyle = new G2DPlotStyleCollection();
      templatePlotStyle.Add(new BarGraphPlotStyle());
      IGraphController gc = Plot(dg.DataTable, dg.SelectedDataColumns, templatePlotStyle);
      ((G2DCartesicCoordinateSystem)gc.Doc.Layers[0].CoordinateSystem).IsXYInterchanged = true;
    }
    /// <summary>
    /// Plots the currently selected data columns of a worksheet as horzizontal bar diagram.
    /// </summary>
    /// <param name="dg">The worksheet controller where the columns are selected in.</param>
    public static void PlotVerticalBarGraph(GUI.WorksheetController dg)
    {
      G2DPlotStyleCollection templatePlotStyle = new G2DPlotStyleCollection();
      templatePlotStyle.Add(new BarGraphPlotStyle());
      IGraphController gc = Plot(dg.DataTable, dg.SelectedDataColumns, templatePlotStyle);
    }



    /// <summary>
    /// Plots a density image of the selected columns.
    /// </summary>
    /// <param name="dg"></param>
    /// <param name="bLine"></param>
    /// <param name="bScatter"></param>
    public static void PlotDensityImage(GUI.WorksheetController dg, bool bLine, bool bScatter)
    {
      DensityImagePlotStyle plotStyle = new DensityImagePlotStyle();

      // if nothing is selected, assume that the whole table should be plotted
      int len = dg.SelectedDataColumns.Count;

      XYZMeshedColumnPlotData assoc = new XYZMeshedColumnPlotData(dg.Doc.DataColumns,len==0 ? null : dg.SelectedDataColumns);

      
      // now create a new Graph with this plot associations

      Altaxo.Graph.GUI.IGraphController gc = Current.ProjectService.CreateNewGraph();

      IGPlotItem pi = new DensityImagePlotItem(assoc,plotStyle);
      gc.Doc.Layers[0].PlotItems.Add(pi);

    }




  }
}
