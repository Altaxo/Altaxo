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
			foreach(int i in selectedColumns)
				selColumns.Add(table[i]);

			return CreatePlotItems(selColumns, templatePlotStyle);
    }

		public static List<IGPlotItem> CreatePlotItems(IEnumerable<DataColumn> selectedColumns, G2DPlotStyleCollection templatePlotStyle)
		{
			return CreatePlotItems(selectedColumns, templatePlotStyle, new HashSet<DataColumn>());
		}

			/// <summary>
		/// Creates a list of plot items from data columns, using a template plot style.
		/// </summary>
		/// <param name="selectedColumns">Columns for which to create plot items.</param>
		/// <param name="templatePlotStyle">The template plot style used to create the basic plot item.</param>
		/// <param name="processedColumns">On return, contains all columns that where used in creating the plot items. That are
		/// not only the columns given in the first argument, but maybe also columns that are right to those columns in the table and have special kinds, like
		/// labels, yerr, and so on.</param>
		/// <returns>List of plot items created.</returns>
		public static List<IGPlotItem> CreatePlotItems(IEnumerable<DataColumn> selectedColumns, G2DPlotStyleCollection templatePlotStyle, HashSet<DataColumn> processedColumns)
		{
			return CreatePlotItems(selectedColumns, string.Empty, templatePlotStyle, processedColumns);
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
		/// <returns>List of plot items created.</returns>
		public static List<IGPlotItem> CreatePlotItems(IEnumerable<DataColumn> selectedColumns, string xColumnName, G2DPlotStyleCollection templatePlotStyle, HashSet<DataColumn> processedColumns)
		{
			var result = new List<IGPlotItem>();
			foreach(DataColumn ycol in selectedColumns)
			{
				if (processedColumns.Contains(ycol))
					continue;
				else
					processedColumns.Add(ycol);

				DataColumnCollection table = DataColumnCollection.GetParentDataColumnCollectionOf(ycol);
				Altaxo.Data.DataColumn xcol;
				if (!string.IsNullOrEmpty(xColumnName) && null!=table && table.ContainsColumn(xColumnName))
					xcol = table[xColumnName];
				else
					xcol = null==table ? null : table.FindXColumnOf(ycol);

				XYColumnPlotData pa;
				if (null != xcol)
					pa = new XYColumnPlotData(xcol, ycol);
				else
					pa = new XYColumnPlotData(new Altaxo.Data.IndexerColumn(), ycol);

				G2DPlotStyleCollection ps = templatePlotStyle != null ? (G2DPlotStyleCollection)templatePlotStyle.Clone() : new G2DPlotStyleCollection();

				if (null == table)
					continue;

				ErrorBarPlotStyle unpairedPositiveError = null;
				ErrorBarPlotStyle unpairedNegativeError = null;

				bool foundMoreColumns = true;
				for (int idx = 1+table.GetColumnNumber(ycol); foundMoreColumns && idx < table.ColumnCount; idx++)
				{
					DataColumn col = table[idx];
					switch (table.GetColumnKind(idx))
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
						processedColumns.Add(table[idx]);
				}

				result.Add(new XYColumnPlotItem(pa, ps));
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
        IPlotGroupStyle s1 = ColorGroupStyle.NewExternalGroupStyle();
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
        IPlotGroupStyle s1 = ColorGroupStyle.NewExternalGroupStyle();
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
        IPlotGroupStyle s1 = ColorGroupStyle.NewExternalGroupStyle();
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
        
        IPlotGroupStyle s2 = ColorGroupStyle.NewExternalGroupStyle();
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

        IPlotGroupStyle s2 = ColorGroupStyle.NewExternalGroupStyle();
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

        IPlotGroupStyle s2 = ColorGroupStyle.NewExternalGroupStyle();
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
		public static Altaxo.Gui.Graph.Viewing.IGraphController Plot(DataTable table,
			IAscendingIntegerCollection selectedColumns,
			 G2DPlotStyleCollection templatePlotStyle,
			PlotGroupStyleCollection groupStyles)
		{
			return Plot(table, selectedColumns, templatePlotStyle, groupStyles, null);
		}

    /// <summary>
    /// Plots selected data columns of a table.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="selectedColumns">The data columns of the table that should be plotted.</param>
    /// <param name="templatePlotStyle">The plot style which is the template for all plot items.</param>
    /// <param name="groupStyles">The group styles for the newly built plot item collection.</param>
    /// <param name="preferredGraphName">Preferred name of the graph. Can be null if you have no preference.</param>
		public static Altaxo.Gui.Graph.Viewing.IGraphController Plot(DataTable table, 
      IAscendingIntegerCollection selectedColumns,
       G2DPlotStyleCollection templatePlotStyle,
      PlotGroupStyleCollection groupStyles,
			string preferredGraphName)
    {
      Altaxo.Graph.Gdi.GraphDocument graph = new Altaxo.Graph.Gdi.GraphDocument();
			if (string.IsNullOrEmpty(preferredGraphName))
			{
				string newnamebase = Altaxo.Main.ProjectFolder.CreateFullName(table.Name, "GRAPH");
				graph.Name = Current.Project.GraphDocumentCollection.FindNewName(newnamebase);
			}
			else
			{
				graph.Name = preferredGraphName;
			}
  
			Altaxo.Graph.Gdi.XYPlotLayer layer = new Altaxo.Graph.Gdi.XYPlotLayer(graph.DefaultLayerPosition, graph.DefaultLayerSize);
      layer.CreateDefaultAxes();
			graph.RootLayer.Layers.Add(layer);
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
		public static Altaxo.Gui.Graph.Viewing.IGraphController Plot(DataTable table,
      IAscendingIntegerCollection selectedColumns,
      Graph.Gdi.GraphDocument graph,
      G2DPlotStyleCollection templatePlotStyle,
      PlotGroupStyleCollection groupStyles)
    {
      List<IGPlotItem> pilist = CreatePlotItems(table, selectedColumns, templatePlotStyle);
      // now create a new Graph with this plot associations
      var gc = Current.ProjectService.CreateNewGraph(graph);
			var xylayer = (Altaxo.Graph.Gdi.XYPlotLayer)gc.Doc.RootLayer.Layers[0];

      // Set x and y axes according to the first plot item in the list
      if (pilist.Count > 0 && (pilist[0] is XYColumnPlotItem))
      {
        XYColumnPlotItem firstitem = (XYColumnPlotItem)pilist[0];
        if (firstitem.Data.XColumn is TextColumn)
					xylayer.Scales.SetScale(0, new TextScale());
        else if (firstitem.Data.XColumn is DateTimeColumn)
					xylayer.Scales.SetScale(0, new DateTimeScale());

        if (firstitem.Data.YColumn is TextColumn)
					xylayer.Scales.SetScale(1, new TextScale());
        else if (firstitem.Data.YColumn is DateTimeColumn)
					xylayer.Scales.SetScale(1, new DateTimeScale());
      }


			PlotItemCollection newPlotGroup = new PlotItemCollection(xylayer.PlotItems);
      foreach (IGPlotItem pi in pilist)
      {
        newPlotGroup.Add(pi);
      }
      if (groupStyles != null)
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
      PlotLine(dg.DataTable,dg.SelectedDataColumns,bLine,bScatter, null);
    }

    /// <summary>
    /// Plots selected data columns of a table.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="selectedColumns">The data columns of the table that should be plotted.</param>
    /// <param name="bLine">If true, the line style is activated (the points are connected by lines).</param>
    /// <param name="bScatter">If true, the scatter style is activated (the points are plotted as symbols).</param>
		/// <param name="preferredGraphName">Preferred name of the graph. Can be null if you have no preference.</param>
		public static void PlotLine(DataTable table, Altaxo.Collections.IAscendingIntegerCollection selectedColumns, bool bLine, bool bScatter, string preferredGraphName)
    {
      if (bLine && bScatter)
        Plot(table, selectedColumns, PlotStyle_Line_Symbol, GroupStyle_Color_Line_Symbol, preferredGraphName);
      else if (bLine)
				Plot(table, selectedColumns, PlotStyle_Line, GroupStyle_Color_Line, preferredGraphName);
      else
				Plot(table, selectedColumns, PlotStyle_Symbol, GroupStyle_Color_Symbol, preferredGraphName);
    }

    /// <summary>
    /// Plots the currently selected data columns of a worksheet.
    /// </summary>
    /// <param name="dg">The worksheet controller where the columns are selected in.</param>
    public static void PlotLineArea(IWorksheetController dg)
    {
      Plot(dg.DataTable, dg.SelectedDataColumns, PlotStyle_LineArea, GroupStyle_Color_Line);
    }
    /// <summary>
    /// Plots the currently selected data columns of a worksheet.
    /// </summary>
    /// <param name="dg">The worksheet controller where the columns are selected in.</param>
    public static void PlotLineStack(IWorksheetController dg)
    {
      Plot(dg.DataTable, dg.SelectedDataColumns, PlotStyle_Line, GroupStyle_Stack_Color_Line);
    }

    /// <summary>
    /// Plots the currently selected data columns of a worksheet.
    /// </summary>
    /// <param name="dg">The worksheet controller where the columns are selected in.</param>
    public static void PlotLineRelativeStack(IWorksheetController dg)
    {
      Plot(dg.DataTable, dg.SelectedDataColumns, PlotStyle_Line, GroupStyle_RelativeStack_Color_Line);
    }

    /// <summary>
    /// Plots the currently selected data columns of a worksheet.
    /// </summary>
    /// <param name="dg">The worksheet controller where the columns are selected in.</param>
    public static void PlotLineWaterfall(IWorksheetController dg)
    {
      Plot(dg.DataTable, dg.SelectedDataColumns, PlotStyle_Line, GroupStyle_Waterfall_Color_Line);
    }

    /// <summary>
    /// Plots the currently selected data columns of a worksheet.
    /// </summary>
    /// <param name="dg">The worksheet controller where the columns are selected in.</param>
    public static void PlotLinePolar(IWorksheetController dg)
    {
      Altaxo.Graph.Gdi.GraphDocument graph = new Altaxo.Graph.Gdi.GraphDocument();
      Altaxo.Graph.Gdi.XYPlotLayer layer = new Altaxo.Graph.Gdi.XYPlotLayer(graph.DefaultLayerPosition,graph.DefaultLayerSize,new Altaxo.Graph.Gdi.CS.G2DPolarCoordinateSystem());
      layer.CreateDefaultAxes();
			graph.RootLayer.Layers.Add(layer);
      Current.Project.GraphDocumentCollection.Add(graph);
      var gc = Plot(dg.DataTable, dg.SelectedDataColumns, graph, PlotStyle_Line, GroupStyle_Color_Line);
    }

    /// <summary>
    /// Plots the currently selected data columns of a worksheet as horzizontal bar diagram.
    /// </summary>
    /// <param name="dg">The worksheet controller where the columns are selected in.</param>
    public static void PlotBarChartNormal(IWorksheetController dg)
    {
      var gc = Plot(dg.DataTable, dg.SelectedDataColumns, PlotStyle_Bar, GroupStyle_Bar);
			var xylayer = (Altaxo.Graph.Gdi.XYPlotLayer)gc.Doc.RootLayer.Layers[0];
      ((G2DCartesicCoordinateSystem)xylayer.CoordinateSystem).IsXYInterchanged = true;
    }

    /// <summary>
    /// Plots the currently selected data columns of a worksheet as horzizontal bar diagram.
    /// </summary>
    /// <param name="dg">The worksheet controller where the columns are selected in.</param>
    public static void PlotBarChartStack(IWorksheetController dg)
    {
      var gc = Plot(dg.DataTable, dg.SelectedDataColumns, PlotStyle_Bar, GroupStyle_Stack_Bar);
			var xylayer = (Altaxo.Graph.Gdi.XYPlotLayer)gc.Doc.RootLayer.Layers[0];
      ((G2DCartesicCoordinateSystem)xylayer.CoordinateSystem).IsXYInterchanged = true;
    }

    /// <summary>
    /// Plots the currently selected data columns of a worksheet as horzizontal bar diagram.
    /// </summary>
    /// <param name="dg">The worksheet controller where the columns are selected in.</param>
    public static void PlotBarChartRelativeStack(IWorksheetController dg)
    {
      var gc = Plot(dg.DataTable, dg.SelectedDataColumns, PlotStyle_Bar, GroupStyle_RelativeStack_Bar);
			var xylayer = (Altaxo.Graph.Gdi.XYPlotLayer)gc.Doc.RootLayer.Layers[0];
      ((G2DCartesicCoordinateSystem)xylayer.CoordinateSystem).IsXYInterchanged = true;
    }


    /// <summary>
    /// Plots the currently selected data columns of a worksheet as horzizontal bar diagram.
    /// </summary>
    /// <param name="dg">The worksheet controller where the columns are selected in.</param>
    public static void PlotColumnChartNormal(IWorksheetController dg)
    {
      Plot(dg.DataTable, dg.SelectedDataColumns, PlotStyle_Bar, GroupStyle_Bar);
    }

    /// <summary>
    /// Plots the currently selected data columns of a worksheet as horzizontal bar diagram.
    /// </summary>
    /// <param name="dg">The worksheet controller where the columns are selected in.</param>
    public static void PlotColumnChartStack(IWorksheetController dg)
    {
      Plot(dg.DataTable, dg.SelectedDataColumns, PlotStyle_Bar, GroupStyle_Stack_Bar);
    }

    /// <summary>
    /// Plots the currently selected data columns of a worksheet as horzizontal bar diagram.
    /// </summary>
    /// <param name="dg">The worksheet controller where the columns are selected in.</param>
    public static void PlotColumnChartRelativeStack(IWorksheetController dg)
    {
      Plot(dg.DataTable, dg.SelectedDataColumns, PlotStyle_Bar, GroupStyle_RelativeStack_Bar);
    }


    /// <summary>
    /// Plots a density image of the selected columns.
    /// </summary>
    /// <param name="dg"></param>
    /// <param name="bLine"></param>
    /// <param name="bScatter"></param>
    public static void PlotDensityImage(IWorksheetController dg, bool bLine, bool bScatter)
    {
      Altaxo.Data.DataTable table = dg.DataTable;
      DensityImagePlotStyle plotStyle = new DensityImagePlotStyle();

      // if nothing is selected, assume that the whole table should be plotted
      int len = dg.SelectedDataColumns.Count;

      INumericColumn xColumn = new IndexerColumn();
      // find out if there is a xcolumn or not
      int group = len == 0 ? table.DataColumns.GetColumnGroup(0) : table.DataColumns.GetColumnGroup(dg.SelectedDataColumns[0]);
      DataColumn xcol = table.DataColumns.FindXColumnOfGroup(group);
      if (xcol is INumericColumn)
        xColumn = (INumericColumn)xcol;
      // remove the x-column of the collection of selected columns, since it should not be plotted
      if (xcol != null && dg.SelectedDataColumns.Contains(table.DataColumns.GetColumnNumber(xcol)))
        dg.SelectedDataColumns.Remove(table.DataColumns.GetColumnNumber(xcol));

      // find out if there is a y property column or not
      INumericColumn yColumn = new IndexerColumn();
      if (dg.SelectedPropertyColumns.Count > 0)
      {
        // then use the first numeric column as y column that you find
        for (int i = 0; i < dg.SelectedPropertyColumns.Count; i++)
        {
          if (table.PropCols[dg.SelectedPropertyColumns[i]] is INumericColumn)
          {
            yColumn = (INumericColumn)table.PropCols[dg.SelectedPropertyColumns[i]];
            break;
          }
        }
      }


       XYZMeshedColumnPlotData assoc = new XYZMeshedColumnPlotData(xColumn, yColumn, dg.DataTable.DataColumns,len==0 ? null : dg.SelectedDataColumns);

      
      // now create a new Graph with this plot associations

      var gc = Current.ProjectService.CreateNewGraphInFolder(Main.ProjectFolder.GetFolderPart(table.Name));
      var layer = new Altaxo.Graph.Gdi.XYPlotLayer(gc.Doc.DefaultLayerPosition, gc.Doc.DefaultLayerSize);
      layer.CreateDefaultAxes();
      gc.Doc.RootLayer.Layers.Add(layer);

      IGPlotItem pi = new DensityImagePlotItem(assoc,plotStyle);
      layer.PlotItems.Add(pi);

    }




  }
}
