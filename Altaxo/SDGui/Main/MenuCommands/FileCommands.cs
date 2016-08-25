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

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using System;

namespace Altaxo.Main.Commands
{
	using Altaxo.Data;

	/// <summary>
	/// Loader for altaxo project files
	/// </summary>
	public class LoadProject : ICSharpCode.SharpDevelop.Project.IProjectLoader
	{
		public void Load(string fileName)
		{
			Current.ProjectService.OpenProject(fileName, false);
		}
	}

	/// <summary>
	/// Loader for altaxo project files
	/// </summary>
	public class LoadWorksheet : ICSharpCode.SharpDevelop.Project.IProjectLoader
	{
		public void Load(string fileName)
		{
			CreateNewWorksheetOrGraphFromFile.OpenWorksheetOrGraph(fileName);
		}
	}

	/// <summary>
	/// Loader for altaxo project files
	/// </summary>
	public class LoadGraph : ICSharpCode.SharpDevelop.Project.IProjectLoader
	{
		public void Load(string fileName)
		{
			CreateNewWorksheetOrGraphFromFile.OpenWorksheetOrGraph(fileName);
		}
	}

	public class CreateNewWorksheet : AbstractMenuCommand
	{
		public override void Run()
		{
			Current.ProjectService.CreateNewWorksheet();
		}
	}

	public class CreateNewStandardWorksheet : AbstractMenuCommand
	{
		public override void Run()
		{
			var controller = Current.ProjectService.CreateNewWorksheet();
			controller.DataTable.AddStandardColumns();
		}
	}

	public class CreateNewGraph : AbstractMenuCommand
	{
		public override void Run()
		{
			Current.ProjectService.CreateNewGraph();
		}
	}

	public class CreateNewWorksheetOrGraphFromFile : AbstractMenuCommand
	{
		public static void OpenWorksheetOrGraph(string filename)
		{
			object deserObject;
			Altaxo.Serialization.Xml.XmlStreamDeserializationInfo info;
			using (System.IO.Stream myStream = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
			{
				info = new Altaxo.Serialization.Xml.XmlStreamDeserializationInfo();
				info.BeginReading(myStream);
				deserObject = info.GetValue("Table", null);
				info.EndReading();
				myStream.Close();
			}

			if (deserObject is IProjectItem)
			{
				Current.Project.AddItemWithThisOrModifiedName((IProjectItem)deserObject);
				info.AnnounceDeserializationEnd(Current.Project, false); // fire the event to resolve path references
				Current.ProjectService.OpenOrCreateViewContentForDocument((IProjectItem)deserObject);
			}
			else if (deserObject is Altaxo.Worksheet.TablePlusLayout)
			{
				Altaxo.Worksheet.TablePlusLayout tableAndLayout = deserObject as Altaxo.Worksheet.TablePlusLayout;
				var table = tableAndLayout.Table;

				Current.Project.AddItemWithThisOrModifiedName(table);

				if (tableAndLayout.Layout != null)
					Current.Project.TableLayouts.Add(tableAndLayout.Layout);

				tableAndLayout.Layout.DataTable = table; // this is the table for the layout now

				info.AnnounceDeserializationEnd(Current.Project, false); // fire the event to resolve path references

				Current.ProjectService.CreateNewWorksheet(table, tableAndLayout.Layout);
			}

			info.AnnounceDeserializationEnd(Current.Project, true); // final deserialization end
		}

		public override void Run()
		{
			var openFileDialog1 = new Microsoft.Win32.OpenFileDialog();

			openFileDialog1.Filter = "Worksheet or graph files (*.axowks;*.axogrp)|*.axowks;*.axogrp|All files (*.*)|*.*";
			openFileDialog1.FilterIndex = 1;
			openFileDialog1.RestoreDirectory = true;

			if (true == openFileDialog1.ShowDialog((System.Windows.Window)Current.Workbench.ViewObject))
			{
				OpenWorksheetOrGraph(openFileDialog1.FileName);
			}
		}
	}

	public class FileOpen : AbstractMenuCommand
	{
		public override void Run()
		{
			if (Current.Project.IsDirty)
			{
				System.ComponentModel.CancelEventArgs cancelargs = new System.ComponentModel.CancelEventArgs();
				Current.ProjectService.AskForSavingOfProject(cancelargs);
				if (cancelargs.Cancel)
					return;
			}

			bool saveDirtyState = Current.Project.IsDirty; // save the dirty state of the project in case the user cancels the open file dialog
			Current.Project.IsDirty = false; // set document to non-dirty

			var openFileDialog1 = new Microsoft.Win32.OpenFileDialog();

			openFileDialog1.Filter = "Altaxo project files (*.axoprj)|*.axoprj|All files (*.*)|*.*";
			openFileDialog1.FilterIndex = 1;
			openFileDialog1.RestoreDirectory = true;

			if (true == openFileDialog1.ShowDialog((System.Windows.Window)Current.Workbench.ViewObject))
			{
				Current.ProjectService.OpenProject(openFileDialog1.FileName, false);
				FileService.RecentOpen.AddLastProject(openFileDialog1.FileName);
			}
			else // in case the user cancels the open file dialog
			{
				Current.Project.IsDirty = saveDirtyState; // restore the dirty state of the current project
			}
		}
	}

	public class FileSaveAs : AbstractMenuCommand
	{
		public override void Run()
		{
			Current.ProjectService.SaveProjectAs();
		}
	}

	public class FileSave : AbstractMenuCommand
	{
		public override void Run()
		{
			if (Current.ProjectService.CurrentProjectFileName != null)
				Current.ProjectService.SaveProject();
			else
				Current.ProjectService.SaveProjectAs();
		}
	}

	/// <summary>
	/// This command is used if in embedded object mode. It saves the current project to a file,
	/// but don't set the current file name of the project (in project service). Furthermore, the title in the title bar is not influenced by the saving.
	/// </summary>
	public class FileSaveCopyAs : AbstractMenuCommand
	{
		public override void Run()
		{
			Current.ProjectService.SaveProjectCopyAs();
		}
	}

	public class FileImportAscii : AbstractMenuCommand
	{
		public override void Run()
		{
			var content = Current.Workbench.ActiveViewContent as Altaxo.Gui.SharpDevelop.SDWorksheetViewContent;

			if (null != content)
			{
				var controller = (Altaxo.Gui.Worksheet.Viewing.WorksheetController)content.MVCController;

				if (null != controller)
					Altaxo.Data.FileCommands.ShowImportAsciiDialog(controller.DataTable);
			}
			else
			{
				Altaxo.Data.FileCommands.ShowImportAsciiDialog(null, true, false);
			}
		}
	}

	public class FileImportAsciiWithOptions : AbstractMenuCommand
	{
		public override void Run()
		{
			var content = Current.Workbench.ActiveViewContent as Altaxo.Gui.SharpDevelop.SDWorksheetViewContent;

			if (null != content)
			{
				var controller = (Altaxo.Gui.Worksheet.Viewing.WorksheetController)content.MVCController;

				if (null != controller)
					Altaxo.Data.FileCommands.ShowImportAsciiDialogAndOptions(controller.DataTable);
			}
			else
			{
				Altaxo.Data.FileCommands.ShowImportAsciiDialogAndOptions(null, true, false);
			}
		}
	}

	public class CloseProject : AbstractMenuCommand
	{
		public override void Run()
		{
			Current.ProjectService.CloseProject(false);
		}
	}

	/// <summary>
	/// Taken from Commands.MenuItemBuilders. See last line for change.
	/// </summary>
	public class RecentProjectsMenuBuilder : ICSharpCode.Core.Presentation.IMenuItemBuilder
	{
		public System.Collections.ICollection BuildItems(Codon codon, object owner)
		{
			RecentOpen recentOpen = FileService.RecentOpen;

			if (recentOpen.RecentProject.Count > 0)
			{
				var items = new System.Windows.Controls.MenuItem[recentOpen.RecentProject.Count];

				for (int i = 0; i < recentOpen.RecentProject.Count; ++i)
				{
					// variable inside loop, so that anonymous method refers to correct recent file
					string recentProject = recentOpen.RecentProject[i];
					string accelaratorKeyPrefix = i < 10 ? "_" + ((i + 1) % 10) + " " : "";
					items[i] = new System.Windows.Controls.MenuItem()
					{
						Header = accelaratorKeyPrefix + recentProject
					};
					items[i].Click += delegate
					{
						// Original SharpDevelop: ProjectService.LoadSolution(recentProject);
						FileUtility.ObservedLoad(new NamedFileOperationDelegate(fileName => Current.ProjectService.OpenProject(fileName, false)), recentProject);
					};
				}
				return items;
			}
			else
			{
				return new[] { new System.Windows.Controls.MenuItem {
						Header = StringParser.Parse("${res:Dialog.Componnents.RichMenuItem.NoRecentProjectsString}"),
						IsEnabled = false
					} };
			}

			/*

			if (recentOpen.RecentProject.Count > 0)
			{
				MenuCommand[] items = new MenuCommand[recentOpen.RecentProject.Count];
				for (int i = 0; i < recentOpen.RecentProject.Count; ++i)
				{
					string accelaratorKeyPrefix = i < 10 ? "&" + ((i + 1) % 10) + " " : "";
					items[i] = new MenuCommand(accelaratorKeyPrefix + recentOpen.RecentProject[i], new EventHandler(LoadRecentProject));
					items[i].Tag = recentOpen.RecentProject[i].ToString();
					items[i].Description = StringParser.Parse(ResourceService.GetString("Dialog.Componnents.RichMenuItem.LoadProjectDescription"),
																										new string[,] { { "PROJECT", recentOpen.RecentProject[i].ToString() } });
				}
				return items;
			}

			MenuCommand defaultMenu = new MenuCommand("${res:Dialog.Componnents.RichMenuItem.NoRecentProjectsString}");
			defaultMenu.Enabled = false;

			return new MenuCommand[] { defaultMenu };
			*/
		}
	}

	public class FileExit : AbstractMenuCommand
	{
		public override void Run()
		{
			((System.Windows.Window)Current.Workbench.ViewObject).Close();
		}
	}

	public class Duplicate : AbstractMenuCommand
	{
		public override void Run()
		{
			if (Current.Workbench.ActiveViewContent is Altaxo.Gui.SharpDevelop.SDWorksheetViewContent)
			{
				new Altaxo.Worksheet.Commands.WorksheetDuplicate().Run();
			}
			else if (Current.Workbench.ActiveViewContent is Altaxo.Gui.SharpDevelop.SDGraphViewContent)
			{
				new Altaxo.Graph.Commands.DuplicateGraph().Run();
			}
			else if (Current.Workbench.ActiveViewContent is Altaxo.Gui.IMVCControllerWrapper)
			{
				var wrapper = Current.Workbench.ActiveViewContent as Altaxo.Gui.IMVCControllerWrapper;
				var viewModel = wrapper.MVCController.ModelObject as IProjectItemViewModel;
				var doc = viewModel?.ProjectItem;
				if (null != doc)
				{
					var newDoc = (IProjectItem)doc.Clone();
					Current.Project.AddItem(newDoc);
					Current.ProjectService.ShowDocumentView(newDoc);
				}
			}
		}
	}

	public class NewDocumentIdentifier : AbstractMenuCommand
	{
		public override void Run()
		{
			object oldId = Current.Project.DocumentIdentifier;
			if (Current.Gui.ShowDialog(ref oldId, "Enter new document identifier", false))
			{
				string newIdentifier = (string)oldId;

				if (Current.Project.DocumentIdentifier != newIdentifier)
				{
					Current.Project.DocumentIdentifier = newIdentifier;
					//Current.ProjectService.
				}
			}
		}
	}

	public class HelpAboutAltaxo : AbstractMenuCommand
	{
		public override void Run()
		{
			var ctrl = new Altaxo.Gui.Common.HelpAboutControl();

			ctrl.ShowDialog();
		}
	}

	public class FileImportOriginOpj : AbstractMenuCommand
	{
		public override void Run()
		{
			var openFileDialog1 = new Microsoft.Win32.OpenFileDialog();
			{
				openFileDialog1.Filter = "OPJ files (*.opj)|*.opj|All files (*.*)|*.*";
				openFileDialog1.FilterIndex = 1;
				openFileDialog1.RestoreDirectory = true;
				openFileDialog1.Multiselect = false;

				if (true == openFileDialog1.ShowDialog((System.Windows.Window)Current.Workbench.ViewObject) && openFileDialog1.FileName.Length > 0)
				{
					string result = Altaxo.Serialization.Origin.Importer.Import(openFileDialog1.FileName);
					if (result != null)
						Current.Gui.ErrorMessageBox(result);
				}
			}
		}
	}

	public class NewInstanceScript : AbstractMenuCommand
	{
		public override void Run()
		{
			Altaxo.Scripting.IScriptText script = null; // or load it from somewhere

			if (script == null)
				script = new Altaxo.Scripting.ProgramInstanceScript();
			var options = new Altaxo.Gui.OpenFileOptions();
			options.Title = "Open a script or press Cancel to create a new script";
			options.AddFilter("*.cs", "C# files (*.cs)");
			options.AddFilter("*.*", "All files (*.*)");
			options.FilterIndex = 0;
			if (Current.Gui.ShowOpenFileDialog(options))
			{
				string scripttext;
				string err = OpenScriptText(options.FileName, out scripttext);
				if (null != err)
					Current.Gui.ErrorMessageBox(err);
				else
					script.ScriptText = scripttext;
			}

			object[] args = new object[] { script, new Altaxo.Gui.Scripting.ScriptExecutionHandler(this.EhScriptExecution) };
			if (Current.Gui.ShowDialog(args, "New instance script"))
			{
				string errors = null;
				do
				{
					errors = null;
					// store the script somewhere m_Table.TableScript = (TableScript)args[0];
					var saveOptions = new Altaxo.Gui.SaveFileOptions();
					saveOptions.Title = "Save your script";
					saveOptions.AddFilter("*.cs", "C# files (*.cs)");
					saveOptions.AddFilter("*.*", "All files (*.*)");
					saveOptions.FilterIndex = 0;
					if (Current.Gui.ShowSaveFileDialog(saveOptions))
					{
						errors = SaveScriptText(saveOptions.FileName, script.ScriptText);
						if (null != errors)
							Current.Gui.ErrorMessageBox(errors);
					}
				} while (null != errors);
			}
		}

		public bool EhScriptExecution(Altaxo.Scripting.IScriptText script, IProgressReporter reporter)
		{
			return ((Altaxo.Scripting.ProgramInstanceScript)script).Execute(reporter);
		}

		private string SaveScriptText(string filename, string text)
		{
			try
			{
				using (System.IO.FileStream stream = new System.IO.FileStream(filename, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.Read))
				{
					System.IO.StreamWriter wr = new System.IO.StreamWriter(stream);
					wr.Write(text);
					wr.Close();
					stream.Close();
				}
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
			return null;
		}

		private string OpenScriptText(string filename, out string scripttext)
		{
			scripttext = null;
			try
			{
				using (System.IO.FileStream stream = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
				{
					System.IO.StreamReader sr = new System.IO.StreamReader(stream);
					scripttext = sr.ReadToEnd();
					sr.Close();
					stream.Close();
				}
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
			return null;
		}
	}
}