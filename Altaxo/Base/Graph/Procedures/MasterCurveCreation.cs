using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Data;

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
			List<List<DoubleColumn>> groupList = new List<List<DoubleColumn>>();
			if (null != (error = FillDataListFromGraphDocument(doc, groupList)))
			{
				Current.Gui.ErrorMessageBox(error);
				return;
			}
		}

		/// <summary>
		/// Tries to extract from the graph document
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="groupList"></param>
		/// <returns></returns>
		static string FillDataListFromGraphDocument(GraphDocument doc, List<List<DoubleColumn>> groupList)
		{
			if (doc.Layers.Count == 0)
				return "Plot contains no layers and therefore no plot items";


			for(int i=0;i<doc.Layers.Count;i++)
			{
				FillDataListFromLayer(doc.Layers[i], groupList);
			}

			return null;
		}

		static void FillDataListFromLayer(XYPlotLayer layer, List<List<DoubleColumn>> groupList)
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

		static List<DoubleColumn> GetColumnListFromPlotItemCollection(Altaxo.Graph.Gdi.Plot.PlotItemCollection plotItemCollection)
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
