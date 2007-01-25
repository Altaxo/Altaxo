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

using System.Collections.Generic;

using Altaxo.Data;
using Altaxo.Collections;
using Altaxo.Graph.Gdi.CS;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Graph.Gdi.Plot.Data;
using Altaxo.Graph.Gdi.Plot.Groups;
using Altaxo.Graph.Plot.Data;
using Altaxo.Graph.Plot.Groups;
using Altaxo.Graph.GUI;




namespace Altaxo.Worksheet.Commands
{
  /// <summary>
  /// PlotCommands contains methods for creating different plot types.
  /// </summary>
  public class PlotCommands
  {

    public static List<IGPlotItem> CreatePlotItems(DataTable table, IAscendingIntegerCollection selectedColumns, G2DPlotStyleCollection templatePlotStyle)
    {
      int len = selectedColumns.Count;
      int numColumns = table.DataColumnCount;

      List<IGPlotItem> result = new List<IGPlotItem>();
      ErrorBarPlotStyle unpairedPositiveError = null;
      ErrorBarPlotStyle unpairedNegativeError = null;
      AscendingIntegerCollection processedColumns = new AscendingIntegerCollection();

      int idx;
      for (int sci = 0; sci < len; sci++)
      {
        idx = selectedColumns[sci];
        if (processedColumns.Contains(idx))
          continue;
        else
          processedColumns.Add(idx);

        Altaxo.Data.DataColumn ycol = table[idx];
        Altaxo.Data.DataColumn xcol = table.DataColumns.FindXColumnOf(ycol);
        XYColumnPlotData pa;
        if (null != xcol)
          pa = new XYColumnPlotData(xcol, ycol);
        else
          pa = new XYColumnPlotData(new Altaxo.Data.IndexerColumn(), ycol);

        G2DPlotStyleCollection ps = templatePlotStyle!=null? (G2DPlotStyleCollection)templatePlotStyle.Clone() : new G2DPlotStyleCollection();

        bool foundMoreColumns = true;
        for(idx=idx+1;foundMoreColumns && idx<numColumns;idx++)
        {
          DataColumn col = table.DataColumns[idx];
          switch (table.DataColumns.GetColumnKind(idx))
          {
            case ColumnKind.Label:
              LabelPlotStyle labelStyle = new LabelPlotStyle(col);
              ps.Insert(0, labelStyle);
              break;
            case ColumnKind.Err:
              ErrorBarPlotStyle errStyle = new ErrorBarPlotStyle();
              errStyle.PositiveErrorColumn = col as INumericColumn;
              errStyle.NegativeErrorColumn = col as INumericColumn;
              ps.Add(errStyle);
              break;
            case ColumnKind.pErr:
              if (null != unpairedNegativeError)
              {
                unpairedNegativeError.PositiveErrorColumn = col as INumericColumn; ;
                unpairedNegativeError = null;
              }
              else
              {
                unpairedPositiveError = new ErrorBarPlotStyle();
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
                unpairedNegativeError = new ErrorBarPlotStyle();
                unpairedNegativeError.NegativeErrorColumn = col as INumericColumn;
                ps.Add(unpairedNegativeError);
              }
              break;
            default:
              foundMoreColumns = false;
              break;
          }

          if (foundMoreColumns)
            processedColumns.Add(idx);

        } 

        result.Add(new XYColumnPlotItem(pa,ps));
      }

      return result;
    }

    #region Predefined plot style collections

    public static G2DPlotStyleCollection PlotStyle_Line
    {
      get
      {
        G2DPlotStyleCollection result = new G2DPlotStyleCollection();
        result.Add(new LinePlotStyle());
        return result;
      }
    }
     public static G2DPlotStyleCollection PlotStyle_Symbol
    {
      get
      {
        G2DPlotStyleCollection result = new G2DPlotStyleCollection();
        result.Add(new ScatterPlotStyle());
        return result;
      }
    }
     public static G2DPlotStyleCollection PlotStyle_Line_Symbol
    {
      get
      {
        G2DPlotStyleCollection result = new G2DPlotStyleCollection();
        result.Add(new ScatterPlotStyle());
        result.Add(new LinePlotStyle());
        return result;
      }
    }
    public static G2DPlotStyleCollection PlotStyle_LineArea
    {
      get
      {
        G2DPlotStyleCollection result = new G2DPlotStyleCollection();
        LinePlotStyle ps1 = new LinePlotStyle();
        ps1.FillArea = true;
        ps1.FillDirection = Graph.CSPlaneID.Bottom;
        result.Add(ps1);
        return result;
      }
    }
    public static G2DPlotStyleCollection PlotStyle_Bar
    {
      get
      {
        G2DPlotStyleCollection result = new G2DPlotStyleCollection();
        BarGraphPlotStyle ps1 = new BarGraphPlotStyle();
        result.Add(ps1);
        return result;
      }
    }

    #endregion

    #region Predefined group styles
    public static PlotGroupStyleCollection GroupStyle_Color_Line
    {
      get
      {
        PlotGroupStyleCollection c = new PlotGroupStyleCollection();
        IPlotGroupStyle s1 = new ColorGroupStyle();
        c.Add(s1);
        IPlotGroupStyle s2 = new LineStyleGroupStyle();
        c.Add(s2,s1.GetType());
        return c;
      }
    }

    public static PlotGroupStyleCollection GroupStyle_Stack_Color_Line
    {
      get
      {
        PlotGroupStyleCollection c = GroupStyle_Color_Line;
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
        PlotGroupStyleCollection c = new PlotGroupStyleCollection();
        IPlotGroupStyle s1 = new ColorGroupStyle();
        c.Add(s1);
        IPlotGroupStyle s2 = new SymbolShapeStyleGroupStyle();
        c.Add(s2, s1.GetType());
        return c;
      }
    }
    public static PlotGroupStyleCollection GroupStyle_Color_Line_Symbol
    {
      get
      {
        PlotGroupStyleCollection c = new PlotGroupStyleCollection();
        IPlotGroupStyle s1 = new ColorGroupStyle();
        c.Add(s1);
        IPlotGroupStyle s2 = new LineStyleGroupStyle();
        c.Add(s2, s1.GetType());
        IPlotGroupStyle s3 = new SymbolShapeStyleGroupStyle();
        c.Add(s3, s2.GetType());
        return c;
      }
    }

    public static PlotGroupStyleCollection GroupStyle_Bar
    {
      get
      {
        PlotGroupStyleCollection c = new PlotGroupStyleCollection();
        IPlotGroupStyle s1 = new BarWidthPositionGroupStyle();
        c.Add(s1);
        
        IPlotGroupStyle s2 = new ColorGroupStyle();
        c.Add(s2);
        return c;
      }
    }
    public static PlotGroupStyleCollection GroupStyle_Stack_Bar
    {
      get
      {
        PlotGroupStyleCollection c = new PlotGroupStyleCollection();
        IPlotGroupStyle s1 = new BarWidthPositionGroupStyle();
        s1.IsStepEnabled = false;
        c.Add(s1);

        IPlotGroupStyle s2 = new ColorGroupStyle();
        c.Add(s2);

        c.CoordinateTransformingStyle = new AbsoluteStackTransform();
        return c;
      }
    }
    public static PlotGroupStyleCollection GroupStyle_RelativeStack_Bar
    {
      get
      {
        PlotGroupStyleCollection c = new PlotGroupStyleCollection();
        IPlotGroupStyle s1 = new BarWidthPositionGroupStyle();
        s1.IsStepEnabled = false;
        c.Add(s1);

        IPlotGroupStyle s2 = new ColorGroupStyle();
        c.Add(s2);

        c.CoordinateTransformingStyle = new RelativeStackTransform();
        return c;
      }
    }

    #endregion


    /// <summary>
    /// Plots selected data columns of a table.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="selectedColumns">The data columns of the table that should be plotted.</param>
    /// <param name="templatePlotStyle">The plot style which is the template for all plot items.</param>
    /// <param name="groupStyles">The group styles for the newly built plot item collection.</param>
    public static IGraphController Plot(DataTable table, 
      IAscendingIntegerCollection selectedColumns,
       G2DPlotStyleCollection templatePlotStyle,
      PlotGroupStyleCollection groupStyles)
    {
      Altaxo.Graph.Gdi.GraphDocument graph = new Altaxo.Graph.Gdi.GraphDocument();
      Altaxo.Graph.Gdi.XYPlotLayer layer = new Altaxo.Graph.Gdi.XYPlotLayer(graph.DefaultLayerPosition, graph.DefaultLayerSize);
      layer.CreateDefaultAxes();
      graph.Layers.Add(layer);
      Current.Project.GraphDocumentCollection.Add(graph);

      return Plot(table, selectedColumns, graph, templatePlotStyle, groupStyles);
    }


    /// <summary>
    /// Plots selected data columns of a table.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="selectedColumns">The data columns of the table that should be plotted.</param>
    /// <param name="graph">The graph document to plot into.</param>
    /// <param name="templatePlotStyle">The plot style which is the template for all plot items.</param>
    /// <param name="groupStyles">The group styles for the newly built plot item collection.</param>
    public static IGraphController Plot(DataTable table,
      IAscendingIntegerCollection selectedColumns,
      Graph.Gdi.GraphDocument graph,
      G2DPlotStyleCollection templatePlotStyle,
      PlotGroupStyleCollection groupStyles)
    {
      List<IGPlotItem> pilist = CreatePlotItems(table, selectedColumns, templatePlotStyle);
      // now create a new Graph with this plot associations
      Altaxo.Graph.GUI.IGraphController gc = Current.ProjectService.CreateNewGraph(graph);
      // Set x and y axes according to the first plot item in the list
      if (pilist.Count > 0 && (pilist[0] is XYColumnPlotItem))
      {
        XYColumnPlotItem firstitem = (XYColumnPlotItem)pilist[0];
        if (firstitem.Data.XColumn is TextColumn)
          gc.Doc.Layers[0].LinkedScales.SetScale(0, new Graph.Scales.TextScale());
        else if (firstitem.Data.XColumn is DateTimeColumn)
          gc.Doc.Layers[0].LinkedScales.SetScale(0, new Graph.Scales.DateTimeScale());

        if (firstitem.Data.YColumn is TextColumn)
          gc.Doc.Layers[0].LinkedScales.SetScale(1, new Graph.Scales.TextScale());
        else if (firstitem.Data.YColumn is DateTimeColumn)
          gc.Doc.Layers[0].LinkedScales.SetScale(1, new Graph.Scales.DateTimeScale());
      }


      PlotItemCollection newPlotGroup = new PlotItemCollection(gc.Doc.Layers[0].PlotItems);
      foreach (IGPlotItem pi in pilist)
      {
        newPlotGroup.Add(pi);
      }
      if (groupStyles != null)
        newPlotGroup.GroupStyles = groupStyles;
      else
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
      if (bLine && bScatter)
        Plot(table, selectedColumns, PlotStyle_Line_Symbol, GroupStyle_Color_Line_Symbol);
      else if (bLine)
        Plot(table, selectedColumns, PlotStyle_Line, GroupStyle_Color_Line);
      else
        Plot(table, selectedColumns, PlotStyle_Symbol, GroupStyle_Color_Symbol);
    }

    /// <summary>
    /// Plots the currently selected data columns of a worksheet.
    /// </summary>
    /// <param name="dg">The worksheet controller where the columns are selected in.</param>
    public static void PlotLineArea(GUI.WorksheetController dg)
    {
      Plot(dg.DataTable, dg.SelectedDataColumns, PlotStyle_LineArea, GroupStyle_Color_Line);
    }
    /// <summary>
    /// Plots the currently selected data columns of a worksheet.
    /// </summary>
    /// <param name="dg">The worksheet controller where the columns are selected in.</param>
    public static void PlotLineStack(GUI.WorksheetController dg)
    {
      Plot(dg.DataTable, dg.SelectedDataColumns, PlotStyle_Line, GroupStyle_Stack_Color_Line);
    }

    /// <summary>
    /// Plots the currently selected data columns of a worksheet.
    /// </summary>
    /// <param name="dg">The worksheet controller where the columns are selected in.</param>
    public static void PlotLineRelativeStack(GUI.WorksheetController dg)
    {
      Plot(dg.DataTable, dg.SelectedDataColumns, PlotStyle_Line, GroupStyle_RelativeStack_Color_Line);
    }

    /// <summary>
    /// Plots the currently selected data columns of a worksheet.
    /// </summary>
    /// <param name="dg">The worksheet controller where the columns are selected in.</param>
    public static void PlotLineWaterfall(GUI.WorksheetController dg)
    {
      Plot(dg.DataTable, dg.SelectedDataColumns, PlotStyle_Line, GroupStyle_Waterfall_Color_Line);
    }

    /// <summary>
    /// Plots the currently selected data columns of a worksheet.
    /// </summary>
    /// <param name="dg">The worksheet controller where the columns are selected in.</param>
    public static void PlotLinePolar(GUI.WorksheetController dg)
    {
      Altaxo.Graph.Gdi.GraphDocument graph = new Altaxo.Graph.Gdi.GraphDocument();
      Altaxo.Graph.Gdi.XYPlotLayer layer = new Altaxo.Graph.Gdi.XYPlotLayer(graph.DefaultLayerPosition,graph.DefaultLayerSize,new Altaxo.Graph.Gdi.CS.G2DPolarCoordinateSystem());
      layer.CreateDefaultAxes();
      graph.Layers.Add(layer);
      Current.Project.GraphDocumentCollection.Add(graph);
      IGraphController gc = Plot(dg.DataTable, dg.SelectedDataColumns, graph, PlotStyle_Line, GroupStyle_Color_Line);
    }

    /// <summary>
    /// Plots the currently selected data columns of a worksheet as horzizontal bar diagram.
    /// </summary>
    /// <param name="dg">The worksheet controller where the columns are selected in.</param>
    public static void PlotBarChartNormal(GUI.WorksheetController dg)
    {
      IGraphController gc = Plot(dg.DataTable, dg.SelectedDataColumns, PlotStyle_Bar, GroupStyle_Bar);
      ((G2DCartesicCoordinateSystem)gc.Doc.Layers[0].CoordinateSystem).IsXYInterchanged = true;
    }

    /// <summary>
    /// Plots the currently selected data columns of a worksheet as horzizontal bar diagram.
    /// </summary>
    /// <param name="dg">The worksheet controller where the columns are selected in.</param>
    public static void PlotBarChartStack(GUI.WorksheetController dg)
    {
      IGraphController gc = Plot(dg.DataTable, dg.SelectedDataColumns, PlotStyle_Bar, GroupStyle_Stack_Bar);
      ((G2DCartesicCoordinateSystem)gc.Doc.Layers[0].CoordinateSystem).IsXYInterchanged = true;
    }

    /// <summary>
    /// Plots the currently selected data columns of a worksheet as horzizontal bar diagram.
    /// </summary>
    /// <param name="dg">The worksheet controller where the columns are selected in.</param>
    public static void PlotBarChartRelativeStack(GUI.WorksheetController dg)
    {
      IGraphController gc = Plot(dg.DataTable, dg.SelectedDataColumns, PlotStyle_Bar, GroupStyle_RelativeStack_Bar);
      ((G2DCartesicCoordinateSystem)gc.Doc.Layers[0].CoordinateSystem).IsXYInterchanged = true;
    }


    /// <summary>
    /// Plots the currently selected data columns of a worksheet as horzizontal bar diagram.
    /// </summary>
    /// <param name="dg">The worksheet controller where the columns are selected in.</param>
    public static void PlotColumnChartNormal(GUI.WorksheetController dg)
    {
      Plot(dg.DataTable, dg.SelectedDataColumns, PlotStyle_Bar, GroupStyle_Bar);
    }

    /// <summary>
    /// Plots the currently selected data columns of a worksheet as horzizontal bar diagram.
    /// </summary>
    /// <param name="dg">The worksheet controller where the columns are selected in.</param>
    public static void PlotColumnChartStack(GUI.WorksheetController dg)
    {
      Plot(dg.DataTable, dg.SelectedDataColumns, PlotStyle_Bar, GroupStyle_Stack_Bar);
    }

    /// <summary>
    /// Plots the currently selected data columns of a worksheet as horzizontal bar diagram.
    /// </summary>
    /// <param name="dg">The worksheet controller where the columns are selected in.</param>
    public static void PlotColumnChartRelativeStack(GUI.WorksheetController dg)
    {
      Plot(dg.DataTable, dg.SelectedDataColumns, PlotStyle_Bar, GroupStyle_RelativeStack_Bar);
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
