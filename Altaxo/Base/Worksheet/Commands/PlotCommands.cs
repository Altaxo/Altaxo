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

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.CS;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Groups;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Graph.Plot.Data;
using Altaxo.Graph.Plot.Groups;
using Altaxo.Graph.Scales;
using Altaxo.Gui.Worksheet.Viewing;

namespace Altaxo.Worksheet.Commands
{
  /// <summary>
  /// PlotCommands contains methods for creating different plot types.
  /// </summary>
  public class PlotCommands
  {
    public static List<IGPlotItem> CreatePlotItems(DataTable table, IAscendingIntegerCollection selectedColumns, G2DPlotStyleCollection templatePlotStyle)
    {
      var selColumns = new List<DataColumn>(selectedColumns.Count);
      foreach (int i in selectedColumns)
        selColumns.Add(table[i]);

      return CreatePlotItems(selColumns, templatePlotStyle, table.GetPropertyContext());
    }

    public static List<IGPlotItem> CreatePlotItems(IEnumerable<DataColumn> selectedColumns, G2DPlotStyleCollection templatePlotStyle, Altaxo.Main.Properties.IReadOnlyPropertyBag context)
    {
      return CreatePlotItems(selectedColumns, templatePlotStyle, new HashSet<DataColumn>(), context);
    }

    /// <summary>
    /// Creates a list of plot items from data columns, using a template plot style.
    /// </summary>
    /// <param name="selectedColumns">Columns for which to create plot items.</param>
    /// <param name="templatePlotStyle">The template plot style used to create the basic plot item.</param>
    /// <param name="processedColumns">On return, contains all columns that where used in creating the plot items. That are
    /// not only the columns given in the first argument, but maybe also columns that are right to those columns in the table and have special kinds, like
    /// labels, yerr, and so on.</param>
    /// <param name="context">Property context used to determine default values, e.g. for the pen width or symbol size.</param>
    /// <returns>List of plot items created.</returns>
    public static List<IGPlotItem> CreatePlotItems(IEnumerable<DataColumn> selectedColumns, G2DPlotStyleCollection templatePlotStyle, HashSet<DataColumn> processedColumns, Altaxo.Main.Properties.IReadOnlyPropertyBag context)
    {
      return CreatePlotItems(selectedColumns, string.Empty, templatePlotStyle, processedColumns, context);
    }

    /// <summary>
    /// Creates a list of plot items from data columns, using a template plot style.
    /// </summary>
    /// <param name="selectedColumns">Columns for which to create plot items.</param>
    /// <param name="xColumnName">Name of the x column. If it is null or empty, or that column is not found in the table, the current assigned x column is used.</param>
    /// <param name="templatePlotStyle">The template plot style used to create the basic plot item.</param>
    /// <param name="processedColumns">On return, contains all columns that where used in creating the plot items. That are
    /// not only the columns given in the first argument, but maybe also columns that are right to those columns in the table and have special kinds, like
    /// labels, yerr, and so on.</param>
    /// <param name="context">Property context used to determine default values, e.g. for the pen width or symbol size.</param>
    /// <returns>List of plot items created.</returns>
    public static List<IGPlotItem> CreatePlotItems(IEnumerable<DataColumn> selectedColumns, string? xColumnName, G2DPlotStyleCollection templatePlotStyle, HashSet<DataColumn> processedColumns, Altaxo.Main.Properties.IReadOnlyPropertyBag context)
    {
      var result = new List<IGPlotItem>();
      foreach (DataColumn ycol in selectedColumns)
      {
        if (processedColumns.Contains(ycol))
          continue;
        else
          processedColumns.Add(ycol);

        var table = DataColumnCollection.GetParentDataColumnCollectionOf(ycol);
        Altaxo.Data.DataColumn? xcol;
        if (!string.IsNullOrEmpty(xColumnName) && table is not null && table.ContainsColumn(xColumnName))
          xcol = table[xColumnName];
        else
          xcol = table?.FindXColumnOf(ycol);

        int groupNumber = table?.GetColumnGroup(ycol) ?? 0;
        var parentTable = DataTable.GetParentDataTableOf(table);

        XYColumnPlotData pa;
        if (xcol is not null && parentTable is not null)
          pa = new XYColumnPlotData(parentTable, groupNumber, xcol, ycol);
        else if (parentTable is not null)
          pa = new XYColumnPlotData(parentTable, groupNumber, new Altaxo.Data.IndexerColumn(), ycol);
        else
          throw new InvalidOperationException($"Could not find a parent data table for this plot operation.");

        G2DPlotStyleCollection ps = templatePlotStyle is not null ? templatePlotStyle.Clone() : new G2DPlotStyleCollection();

        if (table is null)
          continue;

        ErrorBarPlotStyle? unpairedPositiveError = null;
        ErrorBarPlotStyle? unpairedNegativeError = null;

        bool foundMoreColumns = true;
        for (int idx = 1 + table.GetColumnNumber(ycol); foundMoreColumns && idx < table.ColumnCount; idx++)
        {
          DataColumn col = table[idx];
          switch (table.GetColumnKind(idx))
          {
            case ColumnKind.Label:
              var labelStyle = new LabelPlotStyle(col, context);
              ps.Insert(0, labelStyle);
              break;

            case ColumnKind.Err:
              ErrorBarPlotStyle errStyle = new ErrorBarYPlotStyle(context)
              {
                CommonErrorColumn = col as INumericColumn
              };
              ps.Add(errStyle);
              break;

            case ColumnKind.pErr:
              if (unpairedNegativeError is not null)
              {
                unpairedNegativeError.PositiveErrorColumn = col as INumericColumn;
                ;
                unpairedNegativeError = null;
              }
              else
              {
                unpairedPositiveError = new ErrorBarYPlotStyle(context) { UseCommonErrorColumn = false };
                unpairedPositiveError.PositiveErrorColumn = col as INumericColumn;
                ps.Add(unpairedPositiveError);
              }
              break;

            case ColumnKind.mErr:
              if (unpairedPositiveError is not null)
              {
                unpairedPositiveError.NegativeErrorColumn = col as INumericColumn;
                unpairedPositiveError = null;
              }
              else
              {
                unpairedNegativeError = new ErrorBarYPlotStyle(context) { UseCommonErrorColumn = false };
                unpairedNegativeError.NegativeErrorColumn = col as INumericColumn;
                ps.Add(unpairedNegativeError);
              }
              break;

            default:
              foundMoreColumns = false;
              break;
          }

          if (foundMoreColumns)
            processedColumns.Add(table[idx]);
        }

        result.Add(new XYColumnPlotItem(pa, ps));
      }
      return result;
    }

    #region Predefined plot style collections

    public static G2DPlotStyleCollection PlotStyle_Line(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
    {
      var result = new G2DPlotStyleCollection
      {
        new LinePlotStyle(context)
      };
      return result;
    }

    public static G2DPlotStyleCollection PlotStyle_Symbol(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
    {
      var result = new G2DPlotStyleCollection
      {
        new ScatterPlotStyle(context)
      };
      return result;
    }

    public static G2DPlotStyleCollection PlotStyle_Line_Symbol(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
    {
      var result = new G2DPlotStyleCollection
      {
        new ScatterPlotStyle(context),
        new LinePlotStyle(context) { UseSymbolGap = true }
      };
      return result;
    }

    public static G2DPlotStyleCollection PlotStyle_LineArea(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
    {
      var result = new G2DPlotStyleCollection();
      var ps1 = new LinePlotStyle(context);
      var ps2 = new DropAreaPlotStyle(context) { FillDirection = Graph.CSPlaneID.Bottom };
      result.Add(ps1);
      result.Add(ps2);
      return result;
    }

    public static G2DPlotStyleCollection PlotStyle_Bar(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
    {
      var result = new G2DPlotStyleCollection();
      var ps1 = new BarGraphPlotStyle(context);
      result.Add(ps1);
      return result;
    }

    #endregion Predefined plot style collections

    #region Predefined group styles

    public static PlotGroupStyleCollection GroupStyle_Color_Line
    {
      get
      {
        var c = new PlotGroupStyleCollection();
        var s1 = ColorGroupStyle.NewExternalGroupStyle();
        s1.IsStepEnabled = true;
        c.Add(s1);
        var s2 = new DashPatternGroupStyle() { IsStepEnabled = true };
        c.Add(s2, s1.GetType());
        return c;
      }
    }

    public static PlotGroupStyleCollection GroupStyle_Stack_Color_Line
    {
      get
      {
        var c = GroupStyle_Color_Line;
        c.CoordinateTransformingStyle = new AbsoluteStackTransform();
        return c;
      }
    }

    public static PlotGroupStyleCollection GroupStyle_RelativeStack_Color_Line
    {
      get
      {
        PlotGroupStyleCollection c = GroupStyle_Color_Line;
        c.CoordinateTransformingStyle = new RelativeStackTransform();
        return c;
      }
    }

    public static PlotGroupStyleCollection GroupStyle_Waterfall_Color_Line
    {
      get
      {
        PlotGroupStyleCollection c = GroupStyle_Color_Line;
        c.CoordinateTransformingStyle = new WaterfallTransform();
        return c;
      }
    }

    public static PlotGroupStyleCollection GroupStyle_Color_Symbol
    {
      get
      {
        var c = new PlotGroupStyleCollection();
        var s1 = ColorGroupStyle.NewExternalGroupStyle();
        s1.IsStepEnabled = true;
        c.Add(s1);
        var s2 = new ScatterSymbolGroupStyle() { IsStepEnabled = true };
        c.Add(s2, s1.GetType());
        return c;
      }
    }

    public static PlotGroupStyleCollection GroupStyle_Color_Line_Symbol
    {
      get
      {
        var c = new PlotGroupStyleCollection();
        var s1 = ColorGroupStyle.NewExternalGroupStyle();
        s1.IsStepEnabled = true;
        c.Add(s1);
        var s2 = new DashPatternGroupStyle() { IsStepEnabled = true };
        c.Add(s2, s1.GetType());
        var s3 = new ScatterSymbolGroupStyle() { IsStepEnabled = true };
        c.Add(s3, s2.GetType());
        return c;
      }
    }

    public static PlotGroupStyleCollection GroupStyle_Bar
    {
      get
      {
        var c = new PlotGroupStyleCollection();
        var s1 = new BarSizePosition2DGroupStyle() { IsStepEnabled = true };
        c.Add(s1);

        var s2 = ColorGroupStyle.NewExternalGroupStyle();
        c.Add(s2);
        return c;
      }
    }

    public static PlotGroupStyleCollection GroupStyle_Stack_Bar
    {
      get
      {
        var c = new PlotGroupStyleCollection();
        var s1 = new BarSizePosition2DGroupStyle() { IsStepEnabled = false };
        c.Add(s1);

        var s2 = ColorGroupStyle.NewExternalGroupStyle();
        c.Add(s2);

        c.CoordinateTransformingStyle = new AbsoluteStackTransform();
        return c;
      }
    }

    public static PlotGroupStyleCollection GroupStyle_RelativeStack_Bar
    {
      get
      {
        var c = new PlotGroupStyleCollection();
        var s1 = new BarSizePosition2DGroupStyle() { IsStepEnabled = false };
        c.Add(s1);

        var s2 = ColorGroupStyle.NewExternalGroupStyle();
        c.Add(s2);

        c.CoordinateTransformingStyle = new RelativeStackTransform();
        return c;
      }
    }

    #endregion Predefined group styles

    /// <summary>
    /// Plots selected data columns of a table.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="selectedColumns">The data columns of the table that should be plotted.</param>
    /// <param name="graph">The graph document to plot into.</param>
    /// <param name="templatePlotStyle">The plot style which is the template for all plot items.</param>
    /// <param name="groupStyles">The group styles for the newly built plot item collection.</param>
    public static Altaxo.Gui.Graph.Gdi.Viewing.IGraphController Plot(
      DataTable table,
      IAscendingIntegerCollection selectedColumns,
      Graph.Gdi.GraphDocument graph,
      G2DPlotStyleCollection templatePlotStyle,
      PlotGroupStyleCollection groupStyles)
    {
      List<IGPlotItem> pilist = CreatePlotItems(table, selectedColumns, templatePlotStyle);
      // now create a new Graph with this plot associations
      var gc = Current.ProjectService.CreateNewGraph(graph);
      var xylayer = gc.Doc.RootLayer.Layers.OfType<Altaxo.Graph.Gdi.XYPlotLayer>().First();

      // Set x and y axes according to the first plot item in the list
      if (pilist.Count > 0 && (pilist[0] is XYColumnPlotItem))
      {
        var firstitem = (XYColumnPlotItem)pilist[0];
        if (firstitem.Data.XColumn is TextColumn)
          xylayer.Scales[0] = new TextScale();
        else if (firstitem.Data.XColumn is DateTimeColumn)
          xylayer.Scales[0] = new DateTimeScale();

        if (firstitem.Data.YColumn is TextColumn)
          xylayer.Scales[1] = new TextScale();
        else if (firstitem.Data.YColumn is DateTimeColumn)
          xylayer.Scales[1] = new DateTimeScale();
      }

      var newPlotGroup = new PlotItemCollection(xylayer.PlotItems);
      foreach (IGPlotItem pi in pilist)
      {
        newPlotGroup.Add(pi);
      }
      if (groupStyles is not null)
        newPlotGroup.GroupStyles = groupStyles;
      else
        newPlotGroup.CollectStyles(newPlotGroup.GroupStyles);

      xylayer.PlotItems.Add(newPlotGroup);

      return gc;
    }

    /// <summary>
    /// Plots the currently selected data columns of a worksheet.
    /// </summary>
    /// <param name="dg">The worksheet controller where the columns are selected in.</param>
    /// <param name="bLine">If true, a line is plotted.</param>
    /// <param name="bScatter">If true, scatter symbols are plotted.</param>
    public static void PlotLine(IWorksheetController dg, bool bLine, bool bScatter)
    {
      PlotLine(dg.DataTable, dg.SelectedDataColumns, bLine, bScatter, null);
    }

    /// <summary>
    /// Plots selected data columns of a table.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="selectedColumns">The data columns of the table that should be plotted.</param>
    /// <param name="bLine">If true, the line style is activated (the points are connected by lines).</param>
    /// <param name="bScatter">If true, the scatter style is activated (the points are plotted as symbols).</param>
    /// <param name="preferredGraphName">Preferred name of the graph. Can be null if you have no preference.</param>
    public static void PlotLine(DataTable table, Altaxo.Collections.IAscendingIntegerCollection selectedColumns, bool bLine, bool bScatter, string? preferredGraphName)
    {
      var graph = Altaxo.Graph.Gdi.GraphTemplates.TemplateWithXYPlotLayerWithG2DCartesicCoordinateSystem.CreateGraph(table.GetPropertyContext(), preferredGraphName, table.Name, true);
      var context = graph.GetPropertyContext();

      if (bLine && bScatter)
        Plot(table, selectedColumns, graph, PlotStyle_Line_Symbol(context), GroupStyle_Color_Line_Symbol);
      else if (bLine)
        Plot(table, selectedColumns, graph, PlotStyle_Line(context), GroupStyle_Color_Line);
      else
        Plot(table, selectedColumns, graph, PlotStyle_Symbol(context), GroupStyle_Color_Symbol);
    }

    /// <summary>
    /// Plots the currently selected data columns of a worksheet.
    /// </summary>
    /// <param name="dg">The worksheet controller where the columns are selected in.</param>
    public static void PlotLineArea(IWorksheetController dg)
    {
      var graph = Altaxo.Graph.Gdi.GraphTemplates.TemplateWithXYPlotLayerWithG2DCartesicCoordinateSystem.CreateGraph(dg.DataTable.GetPropertyContext(), null, dg.DataTable.Name, true);
      var context = graph.GetPropertyContext();
      Plot(dg.DataTable, dg.SelectedDataColumns, graph, PlotStyle_LineArea(context), GroupStyle_Color_Line);
    }

    /// <summary>
    /// Plots the currently selected data columns of a worksheet.
    /// </summary>
    /// <param name="dg">The worksheet controller where the columns are selected in.</param>
    public static void PlotLineStack(IWorksheetController dg)
    {
      var graph = Altaxo.Graph.Gdi.GraphTemplates.TemplateWithXYPlotLayerWithG2DCartesicCoordinateSystem.CreateGraph(dg.DataTable.GetPropertyContext(), null, dg.DataTable.Name, true);
      var context = graph.GetPropertyContext();
      Plot(dg.DataTable, dg.SelectedDataColumns, graph, PlotStyle_Line(context), GroupStyle_Stack_Color_Line);
    }

    /// <summary>
    /// Plots the currently selected data columns of a worksheet.
    /// </summary>
    /// <param name="dg">The worksheet controller where the columns are selected in.</param>
    public static void PlotLineRelativeStack(IWorksheetController dg)
    {
      var graph = Altaxo.Graph.Gdi.GraphTemplates.TemplateWithXYPlotLayerWithG2DCartesicCoordinateSystem.CreateGraph(dg.DataTable.GetPropertyContext(), null, dg.DataTable.Name, true);
      var context = graph.GetPropertyContext();
      Plot(dg.DataTable, dg.SelectedDataColumns, graph, PlotStyle_Line(context), GroupStyle_RelativeStack_Color_Line);
    }

    /// <summary>
    /// Plots the currently selected data columns of a worksheet.
    /// </summary>
    /// <param name="dg">The worksheet controller where the columns are selected in.</param>
    public static void PlotLineWaterfall(IWorksheetController dg)
    {
      var graph = Altaxo.Graph.Gdi.GraphTemplates.TemplateWithXYPlotLayerWithG2DCartesicCoordinateSystem.CreateGraph(dg.DataTable.GetPropertyContext(), null, dg.DataTable.Name, true);
      var context = graph.GetPropertyContext();
      Plot(dg.DataTable, dg.SelectedDataColumns, graph, PlotStyle_Line(context), GroupStyle_Waterfall_Color_Line);
    }

    /// <summary>
    /// Plots the currently selected data columns of a worksheet.
    /// </summary>
    /// <param name="dg">The worksheet controller where the columns are selected in.</param>
    public static void PlotLinePolar(IWorksheetController dg)
    {
      var graph = Altaxo.Graph.Gdi.GraphTemplates.TemplateWithXYPlotLayerWithG2DPolarCoordinateSystem.CreateGraph(dg.DataTable.GetPropertyContext(), null, dg.DataTable.Name, true);
      var context = graph.GetPropertyContext();
      var gc = Plot(dg.DataTable, dg.SelectedDataColumns, graph, PlotStyle_Line(context), GroupStyle_Color_Line);
    }

    /// <summary>
    /// Plots the currently selected data columns of a worksheet as horzizontal bar diagram.
    /// </summary>
    /// <param name="dg">The worksheet controller where the columns are selected in.</param>
    public static void PlotBarChartNormal(IWorksheetController dg)
    {
      var graph = Altaxo.Graph.Gdi.GraphTemplates.TemplateWithXYPlotLayerWithG2DCartesicCoordinateSystem.CreateGraph(dg.DataTable.GetPropertyContext(), null, dg.DataTable.Name, true);
      var xylayer = graph.RootLayer.Layers.OfType<XYPlotLayer>().First();
      ((G2DCartesicCoordinateSystem)xylayer.CoordinateSystem).IsXYInterchanged = true;
      var context = graph.GetPropertyContext();
      Plot(dg.DataTable, dg.SelectedDataColumns, graph, PlotStyle_Bar(context), GroupStyle_Bar);
    }

    /// <summary>
    /// Plots the currently selected data columns of a worksheet as horzizontal bar diagram.
    /// </summary>
    /// <param name="dg">The worksheet controller where the columns are selected in.</param>
    public static void PlotBarChartStack(IWorksheetController dg)
    {
      var graph = Altaxo.Graph.Gdi.GraphTemplates.TemplateWithXYPlotLayerWithG2DCartesicCoordinateSystem.CreateGraph(dg.DataTable.GetPropertyContext(), null, dg.DataTable.Name, true);
      var xylayer = graph.RootLayer.Layers.OfType<XYPlotLayer>().First();
      ((G2DCartesicCoordinateSystem)xylayer.CoordinateSystem).IsXYInterchanged = true;
      var context = graph.GetPropertyContext();

      Plot(dg.DataTable, dg.SelectedDataColumns, graph, PlotStyle_Bar(context), GroupStyle_Stack_Bar);
    }

    /// <summary>
    /// Plots the currently selected data columns of a worksheet as horzizontal bar diagram.
    /// </summary>
    /// <param name="dg">The worksheet controller where the columns are selected in.</param>
    public static void PlotBarChartRelativeStack(IWorksheetController dg)
    {
      var graph = Altaxo.Graph.Gdi.GraphTemplates.TemplateWithXYPlotLayerWithG2DCartesicCoordinateSystem.CreateGraph(dg.DataTable.GetPropertyContext(), null, dg.DataTable.Name, true);
      var xylayer = graph.RootLayer.Layers.OfType<XYPlotLayer>().First();
      ((G2DCartesicCoordinateSystem)xylayer.CoordinateSystem).IsXYInterchanged = true;
      var context = graph.GetPropertyContext();

      Plot(dg.DataTable, dg.SelectedDataColumns, graph, PlotStyle_Bar(context), GroupStyle_RelativeStack_Bar);
    }

    /// <summary>
    /// Plots the currently selected data columns of a worksheet as horzizontal bar diagram.
    /// </summary>
    /// <param name="dg">The worksheet controller where the columns are selected in.</param>
    public static void PlotColumnChartNormal(IWorksheetController dg)
    {
      var graph = Altaxo.Graph.Gdi.GraphTemplates.TemplateWithXYPlotLayerWithG2DCartesicCoordinateSystem.CreateGraph(dg.DataTable.GetPropertyContext(), null, dg.DataTable.Name, true);
      var context = graph.GetPropertyContext();
      Plot(dg.DataTable, dg.SelectedDataColumns, graph, PlotStyle_Bar(context), GroupStyle_Bar);
    }

    /// <summary>
    /// Plots the currently selected data columns of a worksheet as horzizontal bar diagram.
    /// </summary>
    /// <param name="dg">The worksheet controller where the columns are selected in.</param>
    public static void PlotColumnChartStack(IWorksheetController dg)
    {
      var graph = Altaxo.Graph.Gdi.GraphTemplates.TemplateWithXYPlotLayerWithG2DCartesicCoordinateSystem.CreateGraph(dg.DataTable.GetPropertyContext(), null, dg.DataTable.Name, true);
      var context = graph.GetPropertyContext();
      Plot(dg.DataTable, dg.SelectedDataColumns, graph, PlotStyle_Bar(context), GroupStyle_Stack_Bar);
    }

    /// <summary>
    /// Plots the currently selected data columns of a worksheet as horzizontal bar diagram.
    /// </summary>
    /// <param name="dg">The worksheet controller where the columns are selected in.</param>
    public static void PlotColumnChartRelativeStack(IWorksheetController dg)
    {
      var graph = Altaxo.Graph.Gdi.GraphTemplates.TemplateWithXYPlotLayerWithG2DCartesicCoordinateSystem.CreateGraph(dg.DataTable.GetPropertyContext(), null, dg.DataTable.Name, true);
      var context = graph.GetPropertyContext();
      Plot(dg.DataTable, dg.SelectedDataColumns, graph, PlotStyle_Bar(context), GroupStyle_RelativeStack_Bar);
    }

    /// <summary>
    /// Plots a density image of the selected columns.
    /// </summary>
    /// <param name="dg"></param>
    /// <param name="bLine"></param>
    /// <param name="bScatter"></param>
    public static void PlotDensityImage(IWorksheetController dg, bool bLine, bool bScatter)
    {
      var graph = Altaxo.Graph.Gdi.GraphTemplates.TemplateWithXYPlotLayerWithG2DCartesicCoordinateSystem.CreateGraph(dg.DataTable.GetPropertyContext(), null, dg.DataTable.Name, true);
      var xylayer = graph.RootLayer.Layers.OfType<XYPlotLayer>().First();
      var context = graph.GetPropertyContext();

      var plotStyle = new DensityImagePlotStyle();

      var assoc = new XYZMeshedColumnPlotData(dg.DataTable, dg.SelectedDataRows, dg.SelectedDataColumns, dg.SelectedPropertyColumns);
      if (assoc.DataTableMatrix.RowHeaderColumn is null)
        assoc.DataTableMatrix.RowHeaderColumn = new IndexerColumn();
      if (assoc.DataTableMatrix.ColumnHeaderColumn is null)
        assoc.DataTableMatrix.ColumnHeaderColumn = new IndexerColumn();

      IGPlotItem pi = new DensityImagePlotItem(assoc, plotStyle);
      xylayer.PlotItems.Add(pi);
      Current.ProjectService.CreateNewGraph(graph);
    }
  }
}
