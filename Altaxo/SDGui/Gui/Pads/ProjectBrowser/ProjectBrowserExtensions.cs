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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Pads.ProjectBrowser
{
	using Altaxo;
	using Altaxo.Data;
	using Altaxo.Gui.Common;
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
				else if (item is Altaxo.Main.Properties.ProjectFolderPropertyDocument)
					Current.Project.ProjectFolderProperties.Remove(item as Altaxo.Main.Properties.ProjectFolderPropertyDocument);
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

			string originalFolderName = null;
			bool areDocumentsFromOneFolder = ctrl.IsProjectFolderSelected(out originalFolderName);

			CopyDocuments(list, areDocumentsFromOneFolder, originalFolderName);
		}

		private const string rootFolderDisplayName = "<<<root folder>>>";

		/// <summary>
		/// Delete the items given in the list (tables and graphs), with a confirmation dialog.
		/// </summary>
		/// <param name="list">List of items to move.</param>
		public static void MoveDocuments(IList<object> list)
		{
			var names = Current.Project.Folders.GetSubfoldersAsDisplayFolderNameStringList(ProjectFolder.RootFolderName, true);
			names.Insert(0, rootFolderDisplayName);
			var choices = new TextChoice(names.ToArray(), 0, true) { Description = "Choose or enter the folder to move the items into:" };
			if (!Current.Gui.ShowDialog(ref choices, "Folder choice", false))
				return;

			string newFolderName = rootFolderDisplayName == choices.Text ? ProjectFolder.RootFolderName : ProjectFolder.ConvertDisplayFolderNameToFolderName(choices.Text);
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
			var names = Current.Project.Folders.GetSubfoldersAsDisplayFolderNameStringList(ProjectFolder.RootFolderName, true);
			names.Insert(0, rootFolderDisplayName);
			var choices = new TextChoice(names.ToArray(), 0, true) { Description = "Choose or enter the folder to copy the items into:" };
			if (!Current.Gui.ShowDialog(ref choices, "Folder choice", false))
				return;
			string newFolderName = rootFolderDisplayName == choices.Text ? ProjectFolder.RootFolderName : ProjectFolder.ConvertDisplayFolderNameToFolderName(choices.Text);

			DocNodePathReplacementOptions relocateOptions = null;
			if (areDocumentsFromOneFolder)
			{
				var relocateData = Current.Gui.YesNoCancelMessageBox("Do you want to relocate the references in the copied plots so that they point to the destination folder?", "Question", null);
				if (null == relocateData)
					return;

				if (true == relocateData)
				{
					relocateOptions = new DocNodePathReplacementOptions();
					AltaxoDocument.AddRelocationDataForTables(relocateOptions, originalSourceFolder, newFolderName);
				}
			}

			Current.Project.Folders.CopyItemsToFolder(list, newFolderName, null != relocateOptions ? relocateOptions.Visit : (DocNodeProxyReporter)null);
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
			var command = new Altaxo.Worksheet.Commands.PlotCommonColumnsCommand();
			foreach (object item in list)
			{
				if (item is DataTable)
					command.Tables.Add((DataTable)item);
			}

			if (command.Tables.Count == 0)
				return;

			var commonColumnNames = command.GetCommonColumnNamesUnordered();

			if (0 == commonColumnNames.Count)
			{
				Current.Gui.InfoMessageBox("The selected tables do not seem to have common columns", "Please note");
				return;
			}

			if (true == Current.Gui.ShowDialog(ref command, "Plot common columns", false))
				command.Execute();
		}

		#region Clipboard commands

		/// <summary>
		/// Moves the selected list items to a folder that is asked for by a dialog.
		/// </summary>
		/// <param name="ctrl">Project browse controller.</param>
		public static void CopySelectedListItemsToClipboard(this ProjectBrowseController ctrl)
		{
			var list = ctrl.GetSelectedListItems();

			// if the items are from the same folder, but are selected from the AllItems, AllWorksheet or AllGraphs nodes, we invalidate the folderName (because then we consider the items to be rooted)
			string folderName;
			if (!ctrl.IsProjectFolderSelected(out folderName))
				folderName = null;

			list = ctrl.ExpandItemListToSubfolderItems(list); // Expand the list to get rid of the ProjectFolder items
			Altaxo.Main.Commands.ProjectItemCommands.CopyItemsToClipboard(list, folderName);
		}

		public static bool CanPasteItemsFromClipboard(this ProjectBrowseController ctrl)
		{
			return Altaxo.Main.Commands.ProjectItemCommands.CanPasteItemsFromClipboard();
		}

		public static void PasteItemsFromClipboard(this ProjectBrowseController ctrl)
		{
			string folderName;
			if (!ctrl.IsProjectFolderSelected(out folderName))
				folderName = string.Empty;

			Altaxo.Main.Commands.ProjectItemCommands.PasteItemsFromClipboard(folderName);
		}

		#endregion Clipboard commands

		#endregion List item commands

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

		#endregion Tree node commands

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
				folderName = ProjectFolder.RootFolderName;
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
				folderName = ProjectFolder.RootFolderName;
			return Current.ProjectService.CreateNewGraphInFolder(folderName);
		}

		/// <summary>
		/// Creates a new empty property bag in the current project folder, and shows it in the document area.
		/// </summary>
		/// <param name="ctrl">Project browse controller.</param>
		/// <returns>The property bag.</returns>
		public static Altaxo.Main.Properties.ProjectFolderPropertyDocument CreateNewPropertyBag(this ProjectBrowseController ctrl)
		{
			string folderName;
			if (!ctrl.IsProjectFolderSelected(out folderName))
				folderName = ProjectFolder.RootFolderName;

			Altaxo.Main.Properties.ProjectFolderPropertyDocument bag;
			if (!Current.Project.ProjectFolderProperties.TryGetValue(folderName, out bag))
			{
				bag = new Altaxo.Main.Properties.ProjectFolderPropertyDocument(folderName);
				Current.Project.ProjectFolderProperties.Add(bag);
			}
			return bag;
		}

		#endregion Common Commands
	}
}