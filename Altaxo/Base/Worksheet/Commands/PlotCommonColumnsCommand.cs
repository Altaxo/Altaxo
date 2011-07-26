using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Data;

namespace Altaxo.Worksheet.Commands
{
	/// <summary>
	/// Saves the options for the 'Plot common columns' command and contains the logic to execute that command with the stored options.
	/// </summary>
	public class PlotCommonColumnsCommand
	{
		List<Altaxo.Data.DataTable> _tables = new List<Altaxo.Data.DataTable>();
		List<string> _yCommonColumnNamesForPlotting = new List<string>();

		/// <summary>The tables that contain the common columns to plot.</summary>
		public List<Altaxo.Data.DataTable> Tables { get { return _tables; } }
		/// <summary>
		/// Name of the x column to use for the plot. If this member is null, the current X column of each y column to plot is used.
		/// </summary>
		public string XCommonColumnNameForPlot { get; set; }

		/// <summary>Names of the y columns to plot. For each name, the columns in all the tables where used to build a plot group of plot items, resulting in n plot groups containing m 
		/// plot items, where n is the number of selected column names, and m is the number of tables.</summary>
		public List<string> YCommonColumnNamesForPlotting { get { return _yCommonColumnNamesForPlotting; } }

		/// <summary>
		/// Gets all the names of the columns common to all tables in a unordered fashion.
		/// </summary>
		/// <returns>The names of all the columns common to all tables in a unordered fashion</returns>
		public HashSet<string> GetCommonColumnNamesUnordered()
		{
			if (_tables.Count == 0)
				return new HashSet<string>();

			// now determine which columns are common to all selected tables.
			var commonColumnNames = new HashSet<string>(_tables[0].DataColumns.GetColumnNames());
			for (int i = 1; i < _tables.Count; i++)
				commonColumnNames.IntersectWith(_tables[i].DataColumns.GetColumnNames());
			return commonColumnNames;
		}

		/// <summary>
		/// Gets all the names of the columns common to all tables in the order as the columns appear in the first table.
		/// </summary>
		/// <returns>The names of all the columns common to all tables in the order as the columns appear in the first table.</returns>
		public List<string> GetCommonColumnNamesOrderedByAppearanceInFirstTable()
		{
			if (_tables.Count == 0)
				return new List<string>();

			var commonColumnNames = GetCommonColumnNamesUnordered();
			var result = new List<string>();
			foreach (var name in _tables[0].DataColumns.GetColumnNames())
			{
				// Note: we will add the column names in the order like in the first table
				if (commonColumnNames.Contains(name))
					result.Add(name);
			}
			return result;
		}

		/// <summary>
		/// Executes the 'Plot common column' command.
		/// </summary>
		public void Execute()
		{
			var templateStyle = Altaxo.Worksheet.Commands.PlotCommands.PlotStyle_Line;
			var graphctrl = Current.ProjectService.CreateNewGraph();
			var layer = new Altaxo.Graph.Gdi.XYPlotLayer(graphctrl.Doc.DefaultLayerPosition, graphctrl.Doc.DefaultLayerSize);
			graphctrl.Doc.Layers.Add(layer);
			layer.CreateDefaultAxes();

			var processedColumns = new HashSet<Altaxo.Data.DataColumn>();
			foreach (string colname in _yCommonColumnNamesForPlotting)
			{
				// first create the plot items
				var columnList = new List<DataColumn>();
				foreach (var table in _tables)
					columnList.Add(table[colname]);

				var plotItemList = Altaxo.Worksheet.Commands.PlotCommands.CreatePlotItems(columnList, XCommonColumnNameForPlot, templateStyle, processedColumns);

				var plotGroup = new Altaxo.Graph.Gdi.Plot.PlotItemCollection();
				plotGroup.AddRange(plotItemList);
				layer.PlotItems.Add(plotGroup);
			}
		}
	}
}
