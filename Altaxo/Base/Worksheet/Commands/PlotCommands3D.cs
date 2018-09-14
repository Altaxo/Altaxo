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
using System.Threading.Tasks;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Graph.Graph3D;
using Altaxo.Graph.Graph3D.CS;
using Altaxo.Graph.Graph3D.Plot;
using Altaxo.Graph.Graph3D.Plot.Groups;
using Altaxo.Graph.Graph3D.Plot.Styles;
using Altaxo.Graph.Graph3D.Templates;
using Altaxo.Graph.Plot.Data;
using Altaxo.Graph.Plot.Groups;
using Altaxo.Graph.Scales;
using Altaxo.Gui.Graph.Graph3D.Viewing;
using Altaxo.Gui.Worksheet.Viewing;

namespace Altaxo.Worksheet.Commands
{
  /// <summary>
  /// PlotCommands contains methods for creating different plot types.
  /// </summary>
  public class PlotCommands3D
  {
    public static List<IGPlotItem> CreatePlotItems(DataTable table, IAscendingIntegerCollection selectedColumns, G3DPlotStyleCollection templatePlotStyle)
    {
      var selColumns = new List<DataColumn>(selectedColumns.Count);
      foreach (int i in selectedColumns)
        selColumns.Add(table[i]);

      return CreatePlotItems(selColumns, templatePlotStyle, table.GetPropertyContext());
    }

    public static List<IGPlotItem> CreatePlotItems(IEnumerable<DataColumn> selectedColumns, G3DPlotStyleCollection templatePlotStyle, Altaxo.Main.Properties.IReadOnlyPropertyBag context)
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
    public static List<IGPlotItem> CreatePlotItems(IEnumerable<DataColumn> selectedColumns, G3DPlotStyleCollection templatePlotStyle, HashSet<DataColumn> processedColumns, Altaxo.Main.Properties.IReadOnlyPropertyBag context)
    {
      return CreatePlotItems(selectedColumns, null, null, templatePlotStyle, processedColumns, context);
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
    public static List<IGPlotItem> CreatePlotItems(IEnumerable<DataColumn> selectedColumns, string xColumnName, string yColumnName, G3DPlotStyleCollection templatePlotStyle, HashSet<DataColumn> processedColumns, Altaxo.Main.Properties.IReadOnlyPropertyBag context)
    {
      var result = new List<IGPlotItem>();
      foreach (DataColumn vcol in selectedColumns)
      {
        if (processedColumns.Contains(vcol))
          continue;
        else
          processedColumns.Add(vcol);

        var table = DataTable.GetParentDataTableOf(vcol);
        var tablecoll = DataColumnCollection.GetParentDataColumnCollectionOf(vcol);
        int groupNumber = tablecoll.GetColumnGroup(vcol);

        Altaxo.Data.DataColumn xcol, ycol;
        if (!string.IsNullOrEmpty(xColumnName) && null != table && table.ContainsColumn(xColumnName))
          xcol = table[xColumnName];
        else
          xcol = null == table ? null : tablecoll.FindXColumnOf(vcol);

        if (!string.IsNullOrEmpty(yColumnName) && null != table && table.ContainsColumn(yColumnName))
          ycol = table[yColumnName];
        else
          ycol = null == table ? null : tablecoll.FindYColumnOf(vcol);

        var pa = new XYZColumnPlotData(
            table,
            groupNumber,
            xcol ?? (IReadableColumn)new IndexerColumn(),
            ycol ?? (IReadableColumn)new ConstantDoubleColumn(0),
            vcol);

        var ps = templatePlotStyle != null ? templatePlotStyle.Clone() : new G3DPlotStyleCollection();

        if (null == table)
          continue;

        ErrorBarPlotStyle unpairedPositiveError = null;
        ErrorBarPlotStyle unpairedNegativeError = null;

        bool foundMoreColumns = true;
        for (int idx = 1 + tablecoll.GetColumnNumber(vcol); foundMoreColumns && idx < tablecoll.ColumnCount; idx++)
        {
          DataColumn col = table[idx];
          switch (tablecoll.GetColumnKind(idx))
          {
            case ColumnKind.Label:
              var labelStyle = new LabelPlotStyle(col, context);
              ps.Insert(0, labelStyle);
              break;

            case ColumnKind.Err:
              var errStyle = new ErrorBarZPlotStyle(context)
              {
                UseCommonErrorColumn = true,
                CommonErrorColumn = col as INumericColumn
              };
              ps.Add(errStyle);
              break;

            case ColumnKind.pErr:
              if (null != unpairedNegativeError)
              {
                unpairedNegativeError.PositiveErrorColumn = col as INumericColumn;
                ;
                unpairedNegativeError = null;
              }
              else
              {
                unpairedPositiveError = new ErrorBarZPlotStyle(context) { UseCommonErrorColumn = false };

                unpairedPositiveError.PositiveErrorColumn = col as INumericColumn;
                ps.Add(unpairedPositiveError);
              }
              break;

            case ColumnKind.mErr:
              if (null != unpairedPositiveError)
              {
                unpairedPositiveError.NegativeErrorColumn = col as INumericColumn;
                unpairedPositiveError = null;
              }
              else
              {
                unpairedNegativeError = new ErrorBarZPlotStyle(context) { UseCommonErrorColumn = false };
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

        result.Add(new XYZColumnPlotItem(pa, ps));
      }
      return result;
    }

    #region Predefined plot style collections

    public static G3DPlotStyleCollection PlotStyle_Line(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
    {
      var result = new G3DPlotStyleCollection
      {
        new LinePlotStyle(context)
      };
      return result;
    }

    public static G3DPlotStyleCollection PlotStyle_Symbol(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
    {
      var result = new G3DPlotStyleCollection
      {
        new ScatterPlotStyle(context)
      };
      return result;
    }

    public static G3DPlotStyleCollection PlotStyle_Line_Symbol(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
    {
      var result = new G3DPlotStyleCollection
      {
        new ScatterPlotStyle(context),
        new LinePlotStyle(context) { UseSymbolGap = true }
      };
      return result;
    }

    /// <summary>
    /// Creates a plot style collection for a bar graph.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="isStacked">If set to <c>true</c>, the bar should be stacked.</param>
    /// <returns></returns>
    public static G3DPlotStyleCollection PlotStyle_Bar(Altaxo.Main.Properties.IReadOnlyPropertyBag context, bool isStacked)
    {
      var result = new G3DPlotStyleCollection();
      var ps1 = new BarGraphPlotStyle(context);
      if (isStacked)
      {
        ps1.StartAtPreviousItem = true;
      }
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

    public static PlotGroupStyleCollection GroupStyle_Color_Symbol
    {
      get
      {
        var c = new PlotGroupStyleCollection();
        var s1 = ColorGroupStyle.NewExternalGroupStyle();
        s1.IsStepEnabled = true;
        c.Add(s1);
        var s2 = new ScatterSymbolGroupStyle() { IsStepEnabled = true };
        c.Add(s2);
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
        var s1 = new BarSizePosition3DGroupStyle() { IsStepEnabled = true };
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
        var s1 = new BarSizePosition3DGroupStyle() { IsStepEnabled = false };
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
        var s1 = new BarSizePosition3DGroupStyle() { IsStepEnabled = false };
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
    public static IGraphController Plot(
      DataTable table,
      IAscendingIntegerCollection selectedColumns,
      GraphDocument graph,
      G3DPlotStyleCollection templatePlotStyle,
      PlotGroupStyleCollection groupStyles)
    {
      List<IGPlotItem> pilist = CreatePlotItems(table, selectedColumns, templatePlotStyle);
      // now create a new Graph with this plot associations
      var gc = Current.ProjectService.CreateNewGraph3D(graph);
      var layer = gc.Doc.RootLayer.Layers.OfType<XYZPlotLayer>().First();

      // Set x and y axes according to the first plot item in the list
      if (pilist.Count > 0 && (pilist[0] is XYZColumnPlotItem))
      {
        var firstitem = (XYZColumnPlotItem)pilist[0];
        if (firstitem.Data.XColumn is TextColumn)
          layer.Scales[0] = new TextScale();
        else if (firstitem.Data.XColumn is DateTimeColumn)
          layer.Scales[0] = new DateTimeScale();

        if (firstitem.Data.YColumn is TextColumn)
          layer.Scales[1] = new TextScale();
        else if (firstitem.Data.YColumn is DateTimeColumn)
          layer.Scales[1] = new DateTimeScale();

        if (firstitem.Data.ZColumn is TextColumn)
          layer.Scales[2] = new TextScale();
        else if (firstitem.Data.ZColumn is DateTimeColumn)
          layer.Scales[2] = new DateTimeScale();
      }

      var newPlotGroup = new PlotItemCollection(layer.PlotItems);
      foreach (IGPlotItem pi in pilist)
      {
        newPlotGroup.Add(pi);
      }
      if (groupStyles != null)
        newPlotGroup.GroupStyles = groupStyles;
      else
        newPlotGroup.CollectStyles(newPlotGroup.GroupStyles);

      layer.PlotItems.Add(newPlotGroup);

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
    public static void PlotLine(DataTable table, IAscendingIntegerCollection selectedColumns, bool bLine, bool bScatter, string preferredGraphName)
    {
      var graph = TemplateWithXYZPlotLayerWithG3DCartesicCoordinateSystem.CreateGraph(table.GetPropertyContext(), preferredGraphName, table.Name, true);
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
    public static void PlotLineStack(IWorksheetController dg)
    {
      var graph = TemplateWithXYZPlotLayerWithG3DCartesicCoordinateSystem.CreateGraph(dg.DataTable.GetPropertyContext(), null, dg.DataTable.Name, true);
      var context = graph.GetPropertyContext();
      Plot(dg.DataTable, dg.SelectedDataColumns, graph, PlotStyle_Line(context), GroupStyle_Stack_Color_Line);
    }

    /// <summary>
    /// Plots the currently selected data columns of a worksheet.
    /// </summary>
    /// <param name="dg">The worksheet controller where the columns are selected in.</param>
    public static void PlotLineRelativeStack(IWorksheetController dg)
    {
      var graph = TemplateWithXYZPlotLayerWithG3DCartesicCoordinateSystem.CreateGraph(dg.DataTable.GetPropertyContext(), null, dg.DataTable.Name, true);
      var context = graph.GetPropertyContext();
      Plot(dg.DataTable, dg.SelectedDataColumns, graph, PlotStyle_Line(context), GroupStyle_RelativeStack_Color_Line);
    }

    /// <summary>
    /// Plots the currently selected data columns of a worksheet as horizontal bar diagram.
    /// </summary>
    /// <param name="dg">The worksheet controller where the columns are selected in.</param>
    public static void PlotBarChartNormal(IWorksheetController dg)
    {
      var graph = TemplateWithXYZPlotLayerWithG3DCartesicCoordinateSystem.CreateGraph(dg.DataTable.GetPropertyContext(), null, dg.DataTable.Name, true);
      var layer = graph.RootLayer.Layers.OfType<XYZPlotLayer>().First();
      layer.CoordinateSystem = ((G3DCartesicCoordinateSystem)layer.CoordinateSystem).WithXYInterchanged(true);
      var context = graph.GetPropertyContext();
      Plot(dg.DataTable, dg.SelectedDataColumns, graph, PlotStyle_Bar(context, false), GroupStyle_Bar);
    }

    /// <summary>
    /// Plots the currently selected data columns of a worksheet as horzizontal bar diagram.
    /// </summary>
    /// <param name="dg">The worksheet controller where the columns are selected in.</param>
    public static void PlotBarChartStack(IWorksheetController dg)
    {
      var graph = TemplateWithXYZPlotLayerWithG3DCartesicCoordinateSystem.CreateGraph(dg.DataTable.GetPropertyContext(), null, dg.DataTable.Name, true);
      var layer = graph.RootLayer.Layers.OfType<XYZPlotLayer>().First();
      layer.CoordinateSystem = ((G3DCartesicCoordinateSystem)layer.CoordinateSystem).WithXYInterchanged(true);
      var context = graph.GetPropertyContext();

      Plot(dg.DataTable, dg.SelectedDataColumns, graph, PlotStyle_Bar(context, true), GroupStyle_Stack_Bar);
    }

    /// <summary>
    /// Plots the currently selected data columns of a worksheet as horzizontal bar diagram.
    /// </summary>
    /// <param name="dg">The worksheet controller where the columns are selected in.</param>
    public static void PlotBarChartRelativeStack(IWorksheetController dg)
    {
      var graph = TemplateWithXYZPlotLayerWithG3DCartesicCoordinateSystem.CreateGraph(dg.DataTable.GetPropertyContext(), null, dg.DataTable.Name, true);
      var layer = graph.RootLayer.Layers.OfType<XYZPlotLayer>().First();
      layer.CoordinateSystem = ((G3DCartesicCoordinateSystem)layer.CoordinateSystem).WithXYInterchanged(true);
      var context = graph.GetPropertyContext();

      Plot(dg.DataTable, dg.SelectedDataColumns, graph, PlotStyle_Bar(context, true), GroupStyle_RelativeStack_Bar);
    }

    /// <summary>
    /// Plots the currently selected data columns of a worksheet as horzizontal bar diagram.
    /// </summary>
    /// <param name="dg">The worksheet controller where the columns are selected in.</param>
    public static void PlotColumnChartNormal(IWorksheetController dg)
    {
      var graph = TemplateWithXYZPlotLayerWithG3DCartesicCoordinateSystem.CreateGraph(dg.DataTable.GetPropertyContext(), null, dg.DataTable.Name, true);
      var context = graph.GetPropertyContext();
      Plot(dg.DataTable, dg.SelectedDataColumns, graph, PlotStyle_Bar(context, false), GroupStyle_Bar);
    }

    /// <summary>
    /// Plots the currently selected data columns of a worksheet as horzizontal bar diagram.
    /// </summary>
    /// <param name="dg">The worksheet controller where the columns are selected in.</param>
    public static void PlotColumnChartStack(IWorksheetController dg)
    {
      var graph = TemplateWithXYZPlotLayerWithG3DCartesicCoordinateSystem.CreateGraph(dg.DataTable.GetPropertyContext(), null, dg.DataTable.Name, true);
      var context = graph.GetPropertyContext();
      Plot(dg.DataTable, dg.SelectedDataColumns, graph, PlotStyle_Bar(context, true), GroupStyle_Stack_Bar);
    }

    /// <summary>
    /// Plots the currently selected data columns of a worksheet as horzizontal bar diagram.
    /// </summary>
    /// <param name="dg">The worksheet controller where the columns are selected in.</param>
    public static void PlotColumnChartRelativeStack(IWorksheetController dg)
    {
      var graph = TemplateWithXYZPlotLayerWithG3DCartesicCoordinateSystem.CreateGraph(dg.DataTable.GetPropertyContext(), null, dg.DataTable.Name, true);
      var context = graph.GetPropertyContext();
      Plot(dg.DataTable, dg.SelectedDataColumns, graph, PlotStyle_Bar(context, true), GroupStyle_RelativeStack_Bar);
    }
  }
}
