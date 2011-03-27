using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Pads.ProjectBrowser
{
	using Altaxo;
	using Altaxo.Data;
	using Altaxo.Main;
	using Altaxo.Gui.Common;

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
		/// Moves the selected list items to a folder that is asked for by a dialog.
		/// </summary>
		/// <param name="ctrl">Project browse controller.</param>
		public static void MoveSelectedListItems(this ProjectBrowseController ctrl)
		{
			var list = ctrl.GetSelectedListItems();
			MoveDocuments(list);
		}

		/// <summary>
		/// Moves the selected list items to a folder that is asked for by a dialog.
		/// </summary>
		/// <param name="ctrl">Project browse controller.</param>
		public static void CopySelectedListItemsToFolder(this ProjectBrowseController ctrl)
		{
			var list = ctrl.GetSelectedListItems();

			string originalFolderName=null;
			bool areDocumentsFromOneFolder = ctrl.IsProjectFolderSelected( out originalFolderName);

			CopyDocuments(list, areDocumentsFromOneFolder, originalFolderName);
		}

		const string rootFolderDisplayName = "<<<root folder>>>";


		/// <summary>
		/// Delete the items given in the list (tables and graphs), with a confirmation dialog.
		/// </summary>
		/// <param name="list">List of items to delete.</param>
		public static void MoveDocuments(IList<object> list)
		{
			var names = Current.Project.Folders.GetSubfoldersAsStringList(ProjectFolder.RootFolderName, true);
			names.Insert(0, rootFolderDisplayName);
			var choices = new TextChoice(names.ToArray(), 0, true) { Description = "Choose or enter the folder to move the items into:" };
			if (!Current.Gui.ShowDialog(ref choices, "Folder choice", false))
				return;

			string newFolderName = rootFolderDisplayName == choices.Text ? ProjectFolder.RootFolderName : choices.Text;
			Current.Project.Folders.MoveItemsToFolder(list, newFolderName);
		}


		/// <summary>
		/// Copy the items given in the list (tables and graphs) to a folder, which is selected by the user via a dialog box.
		/// </summary>
		/// <param name="list">List of items to delete.</param>
		/// <param name="areDocumentsFromOneFolder">If true, the list contains objects origination from only one project folder (or from subfolders of that folder). In this case the paramenter <c>originalSourceFolder</c> contains the original project folder from which the items should be copied.</param>
		/// <param name="originalSourceFolder">Original folder from which the items originate (only valid if <c>areDocumentsFromOneFolder</c> is true.</param>
		public static void CopyDocuments(IList<object> list, bool areDocumentsFromOneFolder, string originalSourceFolder)
		{
			var names = Current.Project.Folders.GetSubfoldersAsStringList(ProjectFolder.RootFolderName, true);
			names.Insert(0, rootFolderDisplayName);
			var choices = new TextChoice(names.ToArray(), 0, true) { Description = "Choose or enter the folder to copy the items into:" };
			if (!Current.Gui.ShowDialog(ref choices, "Folder choice", false))
				return;
			string newFolderName = rootFolderDisplayName == choices.Text ? ProjectFolder.RootFolderName : choices.Text;


			DocNodePathReplacementOptions relocateOptions = null;
			if (areDocumentsFromOneFolder)
			{
				var relocateData = Current.Gui.YesNoCancelMessageBox("Do you want to relocate the references in the copied plots so that they point to the destination folder?", "Question", null);
				if (null == relocateData)
					return;

				if (true == relocateData)
				{
					relocateOptions = new DocNodePathReplacementOptions();
					AltaxoDocument.AddRelocationDataForTables(relocateOptions, originalSourceFolder + ProjectFolder.DirectorySeparatorChar, newFolderName + ProjectFolder.DirectorySeparatorChar);
				}
			}


			Current.Project.Folders.CopyItemsToFolder(list, newFolderName, relocateOptions);

		}


		public static void RenameSelectedListItem(this ProjectBrowseController ctrl)
		{
			var list = ctrl.GetSelectedListItems();
			if (list.Count != 1)
				return;

			var obj = list[0];

			if (obj is DataTable)
			{
				Altaxo.Data.DataTableOtherActions.ShowRenameDialog((Altaxo.Data.DataTable)obj);
			}
			else if (obj is Altaxo.Graph.Gdi.GraphDocument)
			{
				Altaxo.Graph.Gdi.GraphDocumentOtherActions.ShowRenameDialog((Altaxo.Graph.Gdi.GraphDocument)obj);
			}
			else if (obj is ProjectFolder)
			{
				ctrl.Project.Folders.ShowFolderRenameDialog((ProjectFolder)obj);
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
			var tables = new List<DataTable>();

			foreach (object item in list)
			{
				if (item is DataTable)
					tables.Add((DataTable)item);
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
			var dlg = new Gui.Common.MultiChoiceList() { Description = "Common columns:", ColumnNames = new string[] { "Column name" } };
			foreach (var name in tables[0].DataColumns.GetColumnNames())
			{
				// Note: we will add the column names in the order like in the first table
				if (commonColumnNames.Contains(name))
					dlg.List.Add(new Altaxo.Collections.SelectableListNode(name, name, false));
			}

			if (!Current.Gui.ShowDialog(ref dlg, "Plot common columns", false))
				return;

			var choosenColumns = new List<string>();
			foreach (var item in dlg.List)
			{
				if (item.IsSelected)
					choosenColumns.Add((string)item.Item);
			}

			var templateStyle = Altaxo.Worksheet.Commands.PlotCommands.PlotStyle_Line;
			var graphctrl = Current.ProjectService.CreateNewGraph();
			var layer = new Altaxo.Graph.Gdi.XYPlotLayer(graphctrl.Doc.DefaultLayerPosition, graphctrl.Doc.DefaultLayerSize);
			graphctrl.Doc.Layers.Add(layer);
			layer.CreateDefaultAxes();

			var processedColumns = new HashSet<DataColumn>();
			foreach (var colname in choosenColumns)
			{
				// first create the plot items
				var columnList = new List<DataColumn>();
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

			ctrl.Project.Folders.ShowFolderRenameDialog(folderName);
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
			DataTableCommands.AddStandardColumns(wks.DataTable);
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
