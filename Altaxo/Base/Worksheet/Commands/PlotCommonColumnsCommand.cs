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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Worksheet.Commands
{
	/// <summary>
	/// Saves the options for the 'Plot common columns' command and contains the logic to execute that command with the stored options.
	/// </summary>
	public class PlotCommonColumnsCommand
	{
		private List<Altaxo.Data.DataTable> _tables = new List<Altaxo.Data.DataTable>();
		private List<string> _yCommonColumnNamesForPlotting = new List<string>();

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

		/// <summary>Determines whether all all tables included in this command are located in a single project folder.</summary>
		/// <param name="folderName">On return, if all tables are located in one project folder, the name of this folder is returned (with trailing directory separator char). Otherwise, an empty string is returned.</param>
		/// <returns><c>True</c> if all tables are located in one project folder; otherwise <c>False</c>.</returns>
		public bool AreAllTablesFromOneProjectFolder(out string folderName)
		{
			var folderHash = new HashSet<string>();

			foreach (DataTable table in _tables)
			{
				folderHash.Add(table.Folder);
			}

			if (folderHash.Count == 1)
			{
				folderName = folderHash.First();
				return true;
			}
			else
			{
				folderName = string.Empty;
				return false;
			}
		}

		/// <summary>
		/// Executes the 'Plot common column' command.
		/// </summary>
		public void Execute()
		{
			string folderName;

			Altaxo.Gui.Graph.Viewing.IGraphController graphctrl;

			if (AreAllTablesFromOneProjectFolder(out folderName))
				graphctrl = Current.ProjectService.CreateNewGraphInFolder(folderName);
			else
				graphctrl = Current.ProjectService.CreateNewGraph();

			var context = graphctrl.Doc.GetPropertyContext();
			var templateStyle = Altaxo.Worksheet.Commands.PlotCommands.PlotStyle_Line(context);

			var layer = new Altaxo.Graph.Gdi.XYPlotLayer(graphctrl.Doc.RootLayer);
			graphctrl.Doc.RootLayer.Layers.Add(layer);
			layer.CreateDefaultAxes(context);

			var processedColumns = new HashSet<Altaxo.Data.DataColumn>();
			foreach (string colname in _yCommonColumnNamesForPlotting)
			{
				// first create the plot items
				var columnList = new List<DataColumn>();
				foreach (var table in _tables)
					columnList.Add(table[colname]);

				var plotItemList = Altaxo.Worksheet.Commands.PlotCommands.CreatePlotItems(columnList, XCommonColumnNameForPlot, templateStyle, processedColumns, context);

				var plotGroup = new Altaxo.Graph.Gdi.Plot.PlotItemCollection();
				plotGroup.GroupStyles.Add(Altaxo.Graph.Plot.Groups.ColorGroupStyle.NewExternalGroupStyle());
				plotGroup.AddRange(plotItemList);
				layer.PlotItems.Add(plotGroup);
			}
		}
	}
}