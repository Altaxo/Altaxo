using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Pads.ProjectBrowser
{
	using Altaxo.Main;

	public static class ProjectBrowserExtensions
	{
		#region List item commands

		/// <summary>
		/// Deletes the selected list items (with confirmation dialog shown).
		/// </summary>
		/// <param name="ctrl">Project browse controller.</param>
		public static void DeleteSelectedListItems(this ProjectBrowseController ctrl)
		{
			var list = ctrl.GetSelectedListItems();
			ctrl.ExpandItemListToSubfolderItems(list);

			DeleteDocuments(list);
		}

		/// <summary>
		/// Delete the items given in the list (tables and graphs), with a confirmation dialog.
		/// </summary>
		/// <param name="list">List of items to delete.</param>
		public static void DeleteDocuments(IList<object> list)
		{
			if (false == Current.Gui.YesNoMessageBox(string.Format("Are you sure to delete {0} items?", list.Count), "Attention!", false))
				return;

			foreach (object item in list)
			{
				if (item is Altaxo.Data.DataTable)
					Current.ProjectService.DeleteTable((Altaxo.Data.DataTable)item, true);
				else if (item is Altaxo.Graph.Gdi.GraphDocument)
					Current.ProjectService.DeleteGraphDocument((Altaxo.Graph.Gdi.GraphDocument)item, true);
			}
		}

		/// <summary>
		/// Show the items currently selected in the document area.
		/// </summary>
		/// <param name="ctrl">Project browse controller.</param>
		public static void ShowSelectedListItem(this ProjectBrowseController ctrl)
		{
			var list = ctrl.GetSelectedListItems();
			ctrl.ExpandItemListToSubfolderItems(list);

			foreach (object item in list)
			{
				Current.ProjectService.ShowDocumentView(item);
			}
		}

		/// <summary>
		/// Ensures that the items in the provided list will be shown in the document area, and
		/// all other items will be hidden.
		/// </summary>
		/// <param name="documents">List of items to show exclusively.</param>
		public static void ShowDocumentsExclusively(IEnumerable<object> documents)
		{
			var selItems = new HashSet<object>(documents);
			var openItems = Current.ProjectService.GetOpenDocuments();

			selItems.ExceptWith(openItems); // this items needs to be opened later on
			openItems.ExceptWith(documents); // this items needs to hide now

			// Hide items
			foreach (var item in openItems)
				Current.ProjectService.CloseDocumentViews(item);

			// Show items
			foreach (var item in selItems)
				Current.ProjectService.ShowDocumentView(item);
		}

		/// <summary>
		/// Shows the selected list items exclusively in the document area.
		/// </summary>
		/// <param name="ctrl">Project browse controller.</param>
		public static void ShowSelectedListItemsExclusively(this ProjectBrowseController ctrl)
		{
			var list = ctrl.GetSelectedListItems();
			ctrl.ExpandItemListToSubfolderItems(list);

			ShowDocumentsExclusively(list);
		}

		/// <summary>
		/// Hide the selected list items, so that they are not shown in the document area.
		/// </summary>
		/// <param name="ctrl"></param>
		public static void HideSelectedListItems(this ProjectBrowseController ctrl)
		{
			var list = ctrl.GetSelectedListItems();
			ctrl.ExpandItemListToSubfolderItems(list);

			foreach (object item in list)
			{
				Current.ProjectService.CloseDocumentViews(item);
			}
		}


    /// <summary>
    /// Plot common columns in two or more tables.
    /// </summary>
    /// <param name="ctrl">Project browse controller.</param>
    public static void PlotCommonColumns(this ProjectBrowseController ctrl)
    {
      var list = ctrl.GetSelectedListItems();
      var tables = new List<Data.DataTable>();

      foreach (object item in list)
      {
        if (item is Data.DataTable)
          tables.Add((Data.DataTable)item);
      }

      if (tables.Count == 0) 
        return;

      // now determine which columns are common to all selected tables.
      var commonColumnNames = new HashSet<string>(tables[0].DataColumns.GetColumnNames());
      for (int i = 1; i < tables.Count; i++)
        commonColumnNames.IntersectWith(tables[i].DataColumns.GetColumnNames());

      if (0 == commonColumnNames.Count)
      {
        Current.Gui.InfoMessageBox("The selected tables seem not to have common columns", "Please note");
        return;
      }



      // and show a dialog to let the user chose which columns to plot
      var dlg = new Gui.Common.MultiChoiceList() { Description = "Common columns:", ColumnNames = new string[] { "Column name" }};
      foreach (var name in tables[0].DataColumns.GetColumnNames())
      {
        // Note: we will add the column names in the order like in the first table
        if(commonColumnNames.Contains(name))
          dlg.List.Add(new Altaxo.Collections.SelectableListNode(name, name, false));
      }

      if (!Current.Gui.ShowDialog(ref dlg, "Plot common columns", false))
        return;

      var choosenColumns = new List<string>();
      foreach (var item in dlg.List)
      {
        if (item.Selected)
          choosenColumns.Add((string)item.Item);
      }

			var templateStyle = Altaxo.Worksheet.Commands.PlotCommands.PlotStyle_Line;
			var graphctrl = Current.ProjectService.CreateNewGraph();
			var layer = new Altaxo.Graph.Gdi.XYPlotLayer(graphctrl.Doc.DefaultLayerPosition, graphctrl.Doc.DefaultLayerSize);
			graphctrl.Doc.Layers.Add(layer);
			layer.CreateDefaultAxes();

			var processedColumns = new HashSet<Data.DataColumn>();
			foreach (var colname in choosenColumns)
			{
				// first create the plot items
				var columnList = new List<Data.DataColumn>();
				foreach (var table in tables)
					columnList.Add(table[colname]);
				var plotItemList = Altaxo.Worksheet.Commands.PlotCommands.CreatePlotItems(columnList, templateStyle, processedColumns);

				var plotGroup = new Altaxo.Graph.Gdi.Plot.PlotItemCollection();
				plotGroup.AddRange(plotItemList);
				layer.PlotItems.Add(plotGroup);
			}
    }

    #endregion

    #region Tree node commands

    /// <summary>
    /// Rename the project folder, if a project folder is selected in the tree view.
    /// </summary>
    /// <param name="ctrl">Project browse controller.</param>
    public static void RenameTreeNode(this ProjectBrowseController ctrl)
		{
			string folderName;
			if (!ctrl.IsProjectFolderSelected(out folderName))
				return;
			if (folderName == Main.ProjectFolder.RootFolderName)
				return;

			var tvctrl = new Altaxo.Gui.Common.TextValueInputController(folderName, "Enter the new name of the folder:");

			if (!Current.Gui.ShowDialog(tvctrl, "Rename folder", false))
				return;

			var newFolderName = tvctrl.InputText.Trim();

			if (newFolderName == string.Empty)
				newFolderName = null;

			if (!ctrl.Project.Folders.CanRenameFolder(folderName, newFolderName))
			{
				if (false == Current.Gui.YesNoMessageBox(
					"Some of the new item names conflict with existing items. Those items will be renamed with " +
					"a generated name based on the old name. Do you want to continue?", "Attention", false))
					return;
			}

			ctrl.Project.Folders.RenameFolder(folderName, newFolderName);
		}

		#endregion

		#region Common Commands

		/// <summary>
		/// Creates a new empty worksheet in the current project folder, and shows it in the document area.
		/// </summary>
		/// <param name="ctrl">Project browse controller.</param>
		/// <returns>The worksheet used to show the newly created table.</returns>
		public static Altaxo.Gui.Worksheet.Viewing.IWorksheetController CreateNewEmptyWorksheet(this ProjectBrowseController ctrl)
		{
			string folderName;
			if (!ctrl.IsProjectFolderSelected(out folderName))
				folderName = Main.ProjectFolder.RootFolderName;
			return Current.ProjectService.CreateNewWorksheetInFolder(folderName);
		}

		/// <summary>
		/// Creates a new worksheet with standard columns in the current project folder, and shows it in the document area.
		/// </summary>
		/// <param name="ctrl">Project browse controller.</param>
		/// <returns>The worksheet used to show the newly created table.</returns>
		public static Altaxo.Gui.Worksheet.Viewing.IWorksheetController CreateNewStandardWorksheet(this ProjectBrowseController ctrl)
		{
			var wks = ctrl.CreateNewEmptyWorksheet();
			Data.DataTableCommands.AddStandardColumns(wks.DataTable);
			return wks;
		}

		/// <summary>
		/// Creates a new empty graph in the current project folder, and shows it in the document area.
		/// </summary>
		/// <param name="ctrl">Project browse controller.</param>
		/// <returns>The graph controller used to show the newly created graph.</returns>
		public static Altaxo.Gui.Graph.Viewing.IGraphController CreateNewGraph(this ProjectBrowseController ctrl)
		{
			string folderName;
			if (!ctrl.IsProjectFolderSelected(out folderName))
				folderName = Main.ProjectFolder.RootFolderName;
			return Current.ProjectService.CreateNewGraphInFolder(folderName);
		}

		#endregion
	}
}
