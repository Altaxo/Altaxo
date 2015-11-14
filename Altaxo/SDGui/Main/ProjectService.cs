#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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

using Altaxo.Graph.Gdi;
using Altaxo.Gui;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Altaxo.Main
{
	/// <summary>
	/// Handles administrative tasks concerning an Altaxo project.
	/// </summary>
	/// <remarks>This should be instantiated only once. You can reach the current project service
	/// by calling <see cref="Current.ProjectService" />.</remarks>
	public class ProjectService : IProjectService
	{
		/// <summary>
		/// The currently opened Altaxo project.
		/// </summary>
		private Altaxo.AltaxoDocument _currentProject;

		/// <summary>
		/// The file name of the currently opened Altaxo project.
		/// </summary>
		private string _currentProjectFileName;

		public ProjectService()
		{
		}

		public void SetCurrentProject(Altaxo.AltaxoDocument project, string projectFileName)
		{
			Altaxo.AltaxoDocument oldProject = _currentProject;
			string oldProjectFileName = _currentProjectFileName;

			if (null != _currentProject)
			{
				_currentProject.DirtyChanged -= EhProjectDirtyChanged;
				OnProjectClosed(new ProjectEventArgs(oldProject));
			}

			_currentProject = project;
			_currentProjectFileName = projectFileName;

			if (_currentProject != null)
			{
				_currentProject.DirtyChanged += this.EhProjectDirtyChanged;
			}

			if (!object.ReferenceEquals(oldProject, _currentProject)) // Project instance has changed
			{
				if (null != oldProject)
				{
					try
					{
						oldProject.Dispose();
					}
					catch (Exception ex)
					{
					}
				}

				OnProjectChanged();
			}
			else // Project instance has not changed
			{
				if (oldProjectFileName != _currentProjectFileName)
					OnRenameProject(new ProjectRenameEventArgs(_currentProject, oldProjectFileName, _currentProjectFileName));
			}
		}

		public void CreateInitialDocument()
		{
			if (null != _currentProject)
				throw new InvalidOperationException("There should be no document before creating the initial document");

			var newProject = new AltaxoDocument();
			SetCurrentProject(newProject, null);
			OnProjectOpened(new ProjectEventArgs(newProject));
		}

		/// <summary>
		/// The currently open Altaxo project.
		/// </summary>
		public Altaxo.AltaxoDocument CurrentOpenProject
		{
			get
			{
				return _currentProject;
			}
		}

		/// <summary>
		/// The name of the currently open project. Is either the file name or the untitled name.
		/// </summary>
		public string CurrentProjectFileName
		{
			get { return this._currentProjectFileName; }
		}

		/// <summary>
		/// Saves the state of the main window into a zipped file.
		/// </summary>
		/// <param name="zippedStream">The file stream of the zip file.</param>
		/// <param name="info">The serialization info used to serialize the state of the main window.</param>
		public void SaveWindowStateToZippedFile(ICompressedFileContainerStream zippedStream, Altaxo.Serialization.Xml.XmlStreamSerializationInfo info)
		{
			System.Text.StringBuilder errorText = new System.Text.StringBuilder();

			{
				// first, we save our own state
				zippedStream.StartFile("Workbench/MainWindow.xml", 0);
				try
				{
					info.BeginWriting(zippedStream.Stream);
					info.AddValue("MainWindow", Current.Workbench);
					info.EndWriting();
				}
				catch (Exception exc)
				{
					errorText.Append(exc.ToString());
				}
			}

			// second, we save all workbench windows into the Workbench/Views
			int i = 0;
			foreach (IViewContent ctrl in Current.Workbench.ViewContentCollection)
			{
				IMVCController mvc = null;
				var mvcc = ctrl as Altaxo.Gui.IMVCControllerWrapper;
				if (null != mvcc)
					mvc = mvcc.MVCController;

				if (mvc != null && info.IsSerializable(mvc.ModelObject))
				{
					i++;
					zippedStream.StartFile("Workbench/Views/View" + i.ToString() + ".xml", 0);
					try
					{
						info.BeginWriting(zippedStream.Stream);
						info.AddValue("WorkbenchViewContent", mvc.ModelObject);
						info.EndWriting();
					}
					catch (Exception exc)
					{
						errorText.Append(exc.ToString());
					}
				}
			}

			if (errorText.Length != 0)
				throw new ApplicationException(errorText.ToString());
		}

		/// <summary>
		/// Restores the state of the main window from a zipped Altaxo project file.
		/// </summary>
		/// <param name="zipFile">The zip file where the state file can be found into.</param>
		/// <param name="info">The deserialization info used to retrieve the data.</param>
		/// <param name="restoredDoc">The previously (also from the zip file!) restored Altaxo document.</param>
		public void RestoreWindowStateFromZippedFile(ZipFile zipFile, Altaxo.Serialization.Xml.XmlStreamDeserializationInfo info, AltaxoDocument restoredDoc)
		{
			System.Collections.ArrayList restoredControllers = new System.Collections.ArrayList();
			foreach (ZipEntry zipEntry in zipFile)
			{
				if (!zipEntry.IsDirectory && zipEntry.Name.StartsWith("Workbench/Views/"))
				{
					System.IO.Stream zipinpstream = zipFile.GetInputStream(zipEntry);
					info.BeginReading(zipinpstream);
					object readedobject = info.GetValue("Table", null);
					if (readedobject is ICSharpCode.SharpDevelop.Gui.IViewContent)
						restoredControllers.Add(readedobject);
					else if (readedobject is GraphViewLayout)
						restoredControllers.Add(readedobject);
					else if (readedobject is Altaxo.Worksheet.WorksheetViewLayout)
						restoredControllers.Add(readedobject);
					info.EndReading();
				}
			}

			info.AnnounceDeserializationEnd(restoredDoc, false);
			info.AnnounceDeserializationEnd(restoredDoc, false);

			// now give all restored controllers a view and show them in the Main view

			foreach (object o in restoredControllers)
			{
				if (o is GraphViewLayout)
				{
					var ctrl = new Altaxo.Gui.Graph.Viewing.GraphControllerWpf();
					ctrl.InitializeDocument(o as GraphViewLayout);
					Current.Gui.FindAndAttachControlTo(ctrl);
					Current.Workbench.ShowView(new Altaxo.Gui.SharpDevelop.SDGraphViewContent(ctrl));
				}
				else if (o is Altaxo.Worksheet.WorksheetViewLayout)
				{
					var wksViewLayout = (Altaxo.Worksheet.WorksheetViewLayout)o;
					if (null != wksViewLayout.WorksheetLayout && null != wksViewLayout.WorksheetLayout.DataTable)
					{
						var ctrl = new Altaxo.Gui.Worksheet.Viewing.WorksheetControllerWpf();
						ctrl.InitializeDocument(wksViewLayout);
						Current.Gui.FindAndAttachControlTo(ctrl);
						Current.Workbench.ShowView(new Altaxo.Gui.SharpDevelop.SDWorksheetViewContent(ctrl));
					}
				}
				else if (o is IViewContent)
				{
					var viewcontent = o as IViewContent;
					IMVCControllerWrapper wrapper = viewcontent as IMVCControllerWrapper;
					if (wrapper != null && wrapper.MVCController.ViewObject == null)
						Current.Gui.FindAndAttachControlTo(wrapper.MVCController);

					if (viewcontent.Control != null)
					{
						Current.Workbench.ShowView(viewcontent);
					}
				}
			}
		}

		/// <summary>
		/// Opens a Altaxo project. If the current project is dirty, and <paramref name="withoutUserInteraction"/> is <c>false</c>, the user is ask to save the current project before.
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="withoutUserInteraction">If <c>false</c>, the user will see dialog if the current project is dirty and needs to be saved. In addition, the user will see
		/// an error dialog if the opening of the new document fails due to exceptions. If this parameter is <c>true</c>, then the old document is forced
		/// to close (without saving). If there is a exception during opening, this exception is thrown.</param>
		public void OpenProject(string filename, bool withoutUserInteraction)
		{
			if (CurrentOpenProject != null)
			{
				System.ComponentModel.CancelEventArgs e = new System.ComponentModel.CancelEventArgs();
				if (this.CurrentOpenProject.IsDirty && withoutUserInteraction == false)
					AskForSavingOfProject(e);

				if (e.Cancel == true)
					return;

				CloseProject(true);
			}

			if (!FileUtility.TestFileExists(filename))
			{
				return;
			}
			WorkbenchSingleton.Workbench.StatusBar.SetMessage("${res:MainWindow.StatusBar.OpeningCombineMessage}");

			try
			{
				if (Path.GetExtension(filename).ToUpper() == ".AXOPRJ")
				{
					string validproject = Path.ChangeExtension(filename, ".axoprj");
					if (File.Exists(validproject))
					{
						InternalLoadProjectFromFile(validproject);
					}
				}
				else
				{
					InternalLoadProjectFromFile(filename);
				}
			}
			catch (Exception ex)
			{
				if (withoutUserInteraction)
					throw;
				else
					Current.Gui.ErrorMessageBox(ex.Message);
			}

			WorkbenchSingleton.Workbench.StatusBar.SetMessage("${res:MainWindow.StatusBar.ReadyMessage}");
		}

		/// <summary>
		/// Loads a existing Altaxo project with the provided name.
		/// </summary>
		/// <param name="filename">The file name of the project to load.</param>
		private void InternalLoadProjectFromFile(string filename)
		{
			if (!FileUtility.TestFileExists(filename))
			{
				return;
			}

			this.Load(filename);

			FileService.RecentOpen.AddLastProject(filename);
		}

		/// <summary>
		/// Opens a Altaxo project from a project file (without asking the user).
		/// </summary>
		/// <param name="filename"></param>
		private void Load(string filename)
		{
			string errorText;
			using (System.IO.FileStream myStream = new System.IO.FileStream(filename, System.IO.FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				errorText = InternalLoadProjectFromStream(myStream, filename);
				myStream.Close();
			}

			if (errorText.Length != 0)
				throw new ApplicationException(errorText);
		}

		/// <summary>
		/// Opens an Altaxo project from a stream.
		/// </summary>
		/// <param name="myStream">The stream from which to load the project.</param>
		public string LoadProject(System.IO.Stream myStream)
		{
			var errors = InternalLoadProjectFromStream(myStream, null);
			return errors;
		}

		/// <summary>
		/// Opens a Altaxo project from a stream.
		/// </summary>
		/// <param name="myStream">The stream from which to load the project.</param>
		/// <param name="filename">Either the filename of the file which stored the document, or null (e.g. myStream is a MemoryStream).</param>
		private string InternalLoadProjectFromStream(System.IO.Stream myStream, string filename)
		{
			var errorText = new System.Text.StringBuilder();

			ZipFile zipFile = null; ;
			AltaxoDocument newdocument = null; ;
			Altaxo.Serialization.Xml.XmlStreamDeserializationInfo info;
			ZipFileWrapper zipFileWrapper;

			try
			{
				zipFile = new ZipFile(myStream);
				info = new Altaxo.Serialization.Xml.XmlStreamDeserializationInfo();
				newdocument = new AltaxoDocument();
				zipFileWrapper = new ZipFileWrapper(zipFile);
			}
			catch (Exception exc)
			{
				errorText.Append(exc.ToString());
				return errorText.ToString(); // this is unrecoverable - we must return
			}

			try
			{
				using (var suspendToken = newdocument.SuspendGetToken())
				{
					newdocument.RestoreFromZippedFile(zipFileWrapper, info);
				}
			}
			catch (Exception exc)
			{
				errorText.Append(exc.ToString());
			}

			try
			{
				Current.Workbench.CloseAllViews();
				this.SetCurrentProject(newdocument, filename);
				RestoreWindowStateFromZippedFile(zipFile, info, newdocument);
				info.AnnounceDeserializationEnd(newdocument, true); // Final call to deserialization end

				this.CurrentOpenProject.IsDirty = false;

				info.AnnounceDeserializationHasCompletelyFinished(); // Annonce completly finished deserialization, activate data sources of the Altaxo document

				OnProjectOpened(new ProjectEventArgs(newdocument));
			}
			catch (Exception exc)
			{
				errorText.Append(exc.ToString());
			}
			return errorText.ToString();
		}

		/// <summary>
		/// Internal routine to save a project under a given name.
		/// </summary>
		/// <param name="filename"></param>
		private void Save(string filename)
		{
			bool fileAlreadyExists = System.IO.File.Exists(filename);

			System.IO.Stream myStream;
			string tempFileName = null;
			if (fileAlreadyExists)
			{
				tempFileName = System.IO.Path.GetTempFileName();
				myStream = new System.IO.FileStream(tempFileName, System.IO.FileMode.Create, FileAccess.Write, FileShare.None);
			}
			else
				myStream = new System.IO.FileStream(filename, System.IO.FileMode.Create, FileAccess.Write, FileShare.None);

			Exception savingException = SaveProject(myStream);

			try
			{
				if (savingException == null)
				{
					// Test the file for integrity
					string testfilename = tempFileName != null ? tempFileName : filename;
					myStream = new System.IO.FileStream(testfilename, System.IO.FileMode.Open, FileAccess.Read, FileShare.None);
					ZipFile zipFile = new ZipFile(myStream);
					foreach (ZipEntry zipEntry in zipFile)
					{
						if (!zipEntry.IsDirectory)
						{
							System.IO.Stream zipinpstream = zipFile.GetInputStream(zipEntry);
						}
					}
					zipFile.Close();
					// end test
				}
			}
			catch (Exception exc)
			{
				savingException = exc;
			}

			// now, if no exception happened, copy the temporary file back to the original file
			if (null != tempFileName && null == savingException)
			{
				System.IO.File.Copy(tempFileName, filename, true);
				System.IO.File.Delete(tempFileName);
			}

			if (null != savingException)
				throw savingException;

			this._currentProject.IsDirty = false;
		}

		public Exception SaveProject(System.IO.Stream myStream)
		{
			Altaxo.Serialization.Xml.XmlStreamSerializationInfo info = new Altaxo.Serialization.Xml.XmlStreamSerializationInfo();
			ZipOutputStream zippedStream = new ZipOutputStream(myStream);
			ZipOutputStreamWrapper zippedStreamWrapper = new ZipOutputStreamWrapper(zippedStream);

			Exception savingException = null;
			try
			{
				this._currentProject.SaveToZippedFile(zippedStreamWrapper, info);

				if (!Current.Gui.InvokeRequired())
					SaveWindowStateToZippedFile(zippedStreamWrapper, info);
			}
			catch (Exception exc)
			{
				savingException = exc;
			}

			zippedStream.Flush();
			zippedStream.Close();
			myStream.Close();
			return savingException;
		}

		/// <summary>
		/// Saves a project under the current file name.
		/// </summary>
		public void SaveProject()
		{
			SaveProject(_currentProjectFileName);
		}

		/// <summary>
		/// Saves the current project under a provided file name. If the provided file name differs
		/// from the current file name, a project renaming event is triggered.
		/// </summary>
		/// <param name="filename">The new project file name.</param>
		public void SaveProject(string filename)
		{
			string oldFileName = this._currentProjectFileName;
			this._currentProjectFileName = filename;
			if (oldFileName != filename)
				this.OnRenameProject(new ProjectRenameEventArgs(this._currentProject, oldFileName, filename));

			FileUtility.ObservedSave(new NamedFileOperationDelegate(this.Save),
				filename,
				ResourceService.GetString("Altaxo.Project.CantSaveProjectErrorText"),
				FileErrorPolicy.ProvideAlternative);
		}

		/// <summary>
		/// Asks the user for a file name for the current project, and then saves the project under the given name.
		/// </summary>
		public void SaveProjectAs()
		{
			SaveFileDialog fdiag = new SaveFileDialog();
			fdiag.OverwritePrompt = true;
			fdiag.AddExtension = true;

			fdiag.Filter = StringParser.Parse("${res:Altaxo.FileFilter.ProjectFiles}|*.axoprj|${res:Altaxo.FileFilter.AllFiles}|*.*");

			if (fdiag.ShowDialog() == DialogResult.OK)
			{
				string filename = fdiag.FileName;
				SaveProject(filename);
				//FileService.RecentOpen.AddLastProject(filename);
				WorkbenchSingleton.Workbench.StatusBar.SetMessage(filename + ": " + ResourceService.GetString("Altaxo.Project.ProjectSavedMessage"));
				//MessageService.ShowMessage(filename, ResourceService.GetString("Altaxo.Project.ProjectSavedMessage"));
			}
		}

		/// <summary>
		/// This command is used if in embedded object mode. It saves the current project to a file,
		/// but don't set the current file name of the project (in project service). Furthermore, the title in the title bar is not influenced by the saving.
		/// </summary>
		public void SaveProjectCoypAs()
		{
			SaveFileDialog fdiag = new SaveFileDialog();
			fdiag.OverwritePrompt = true;
			fdiag.AddExtension = true;

			fdiag.Filter = StringParser.Parse("${res:Altaxo.FileFilter.ProjectFiles}|*.axoprj|${res:Altaxo.FileFilter.AllFiles}|*.*");

			if (fdiag.ShowDialog() == DialogResult.OK)
			{
				string filename = fdiag.FileName;

				FileUtility.ObservedSave(
					new NamedFileOperationDelegate(this.Save),
					filename,
					ResourceService.GetString("Altaxo.Project.CantSaveProjectErrorText"),
					FileErrorPolicy.ProvideAlternative); FileService.RecentOpen.AddLastProject(filename);
			}
		}

		/// <summary>
		/// Asks the user whether or not the project should be saved, and saves it in case the user answers with yes.
		/// </summary>
		/// <param name="e">Cancel event args. On return, the e.Cancel property is set to true, if the users cancel the saving.</param>
		public virtual void AskForSavingOfProject(System.ComponentModel.CancelEventArgs e)
		{
			string text = ResourceService.GetString("Altaxo.Project.AskForSavingOfProjectDialog.Text");
			string caption = ResourceService.GetString("Altaxo.Project.AskForSavingOfProjectDialog.Caption");
			bool? dlgresult = Current.Gui.YesNoCancelMessageBox(text, caption, null);

			if (null == dlgresult) // Cancel
			{
				e.Cancel = true;
			}
			else if (true == dlgresult) // Yes
			{
				if (this.CurrentProjectFileName != null)
					this.SaveProject();
				else
					this.SaveProjectAs();

				if (this.CurrentOpenProject.IsDirty)
					e.Cancel = true; // Cancel if the saving was not successfull
			}
		}

		/// <summary>
		/// Closes a project. If the project is dirty, and <paramref name="forceClose"/> is <c>false</c>, the user is asked to save the project.
		/// </summary>
		/// <param name="forceClose">If <c>false</c> and the project is dirty, the user will be asked whether he really wants to close the project.
		/// If <c>true</c>, the project is closed without user interaction.</param>
		public void CloseProject(bool forceClose)
		{
			if (CurrentOpenProject != null)
			{
				System.ComponentModel.CancelEventArgs e = new System.ComponentModel.CancelEventArgs();
				if (this.CurrentOpenProject.IsDirty && !forceClose)
					AskForSavingOfProject(e);

				if (e.Cancel == false)
				{
					//if (saveCombinePreferencies)
					//  SaveCombinePreferences(CurrentOpenCombine, openCombineFileName);

					Altaxo.AltaxoDocument closedProject = CurrentOpenProject;
					//CurrentSelectedProject = null;
					//CurrentOpenCombine = CurrentSelectedCombine = null;
					_currentProjectFileName = null;
					WorkbenchSingleton.Workbench.CloseAllViews();
					OnProjectClosed(new ProjectEventArgs(closedProject));
					//closedProject.Dispose();

					// now create a new project
					var newProject = new Altaxo.AltaxoDocument();
					this.SetCurrentProject(newProject, null);

					// dispose the old project
					if (null != closedProject)
						closedProject.Dispose();

					OnProjectOpened(new ProjectEventArgs(newProject));
				}
			}
		}

		/// <summary>
		/// Disposes the whole project and sets the current project to null.
		/// </summary>
		public void DisposeProjectAndSetToNull()
		{
			if (this._currentProject != null)
			{
				this._currentProject.Dispose();
				this._currentProject = null;
			}
		}

		/// <summary>
		/// Returns all currently open views that show the given document object <code>document</code>.
		/// The IViewContent must implement <see cref="Altaxo.Gui.IMVCControllerWrapper" /> in order to be found by this routine.
		/// </summary>
		/// <param name="document">The document for which views must be found.</param>
		/// <param name="maxNumber">Max number of <see cref="IViewContent" /> to return.</param>
		/// <returns>An array containing all views that show the document table. If no view is found, an empty array is returned.</returns>
		public List<IViewContent> SearchContentForDocument(object document, int maxNumber)
		{
			List<IViewContent> contentList = new List<IViewContent>();
			// first step : look in all views

			foreach (IViewContent content in Current.Workbench.ViewContentCollection)
			{
				if (content is Altaxo.Gui.IMVCControllerWrapper)
				{
					object modelobject = ((Altaxo.Gui.IMVCControllerWrapper)content).MVCController.ModelObject;

					if (object.ReferenceEquals(modelobject, document))
					{
						contentList.Add(content);
					}
					else if (modelobject is Altaxo.Worksheet.WorksheetViewLayout)
					{
						var wvl = (Altaxo.Worksheet.WorksheetViewLayout)modelobject;
						if (object.ReferenceEquals(wvl.WorksheetLayout, document))
							contentList.Add(content);
						else if (wvl.WorksheetLayout != null && object.ReferenceEquals(wvl.WorksheetLayout.DataTable, document))
							contentList.Add(content);
					}
					else if (modelobject is GraphViewLayout)
					{
						var gvl = (GraphViewLayout)modelobject;
						if (object.ReferenceEquals(gvl.GraphDocument, document))
							contentList.Add(content);
					}
				}

				if (contentList.Count >= maxNumber)
					break;
			}

			return contentList;
		}

		/// <summary>
		/// Gets a set of all open documents, i.e. GraphDocuments, DataTables. (Not Worksheets).
		/// </summary>
		/// <returns>The set of all open documents.</returns>
		public HashSet<object> GetOpenDocuments()
		{
			var result = new HashSet<object>();
			foreach (IViewContent content in Current.Workbench.ViewContentCollection)
			{
				object modelobject = null;
				if (content is Altaxo.Gui.IMVCControllerWrapper)
				{
					modelobject = ((Altaxo.Gui.IMVCControllerWrapper)content).MVCController.ModelObject;

					if (modelobject is Altaxo.Worksheet.WorksheetLayout)
						modelobject = (modelobject as Altaxo.Worksheet.WorksheetLayout).DataTable;
				}
				if (null != modelobject && !result.Contains(modelobject))
					result.Add(modelobject);
			}
			return result;
		}

		/// <summary>
		/// Returns true if the given document has at least one open view in the workbench.
		/// </summary>
		/// <param name="document">The document.</param>
		/// <returns>True if there is at least one open view for the document.</returns>
		public bool HasDocumentAnOpenView(object document)
		{
			return SearchContentForDocument(document, 1).Count > 0;
		}

		/// <summary>
		/// Closes all open views for a given document.
		/// </summary>
		/// <param name="document"></param>
		public void CloseDocumentViews(object document)
		{
			Current.Gui.Execute(CloseDocumentViews_Unsynchronized, document);
		}

		private void CloseDocumentViews_Unsynchronized(object document)
		{
			var list = SearchContentForDocument(document, int.MaxValue);
			for (int i = list.Count - 1; i >= 0; i--)
			{
				list[i].WorkbenchWindow.CloseWindow(true);
			}
		}

		/// <summary>
		/// Shows a view for the given document.
		/// </summary>
		/// <param name="document">The document to open.</param>
		public void ShowDocumentView(object document)
		{
			Current.Gui.Execute(ShowDocumentView_Unsynchronized, document);
		}

		private void ShowDocumentView_Unsynchronized(object document)
		{
			if (document is Altaxo.Data.DataTable)
				OpenOrCreateWorksheetForTable((Altaxo.Data.DataTable)document);
			else if (document is Altaxo.Graph.Gdi.GraphDocument)
				OpenOrCreateGraphForGraphDocument((Altaxo.Graph.Gdi.GraphDocument)document);
		}

		private void SelectFirstAvailableView()
		{
			/*
			// the following sequence is related to a bug encountered when closing a tabbed window by the program:
			// the active view content is not updated because the dockpanel lost the focus
			// to circumvent this, we focus on a new viewcontent, in this case the first one
			IViewContent firstView = null;
			foreach (IViewContent v in Current.Workbench.ViewContentCollection)
			{
				firstView = v;
				break;
			}

			if (firstView != null)
				WorkbenchSingleton.Workbench.WorkbenchLayout.ShowView(firstView, true);
			 */
		}

		#region Worksheet functions

		/// <summary>
		/// Creates a table and the view content for that table.
		/// </summary>
		/// <param name="worksheetName">The name of the table to create.</param>
		/// <param name="bCreateDefaultColumns">If true, a default x and a y column is created.</param>
		/// <returns>The view content for the provided table.</returns>
		public Altaxo.Gui.Worksheet.Viewing.IWorksheetController CreateNewWorksheet(string worksheetName, bool bCreateDefaultColumns)
		{
			Altaxo.Data.DataTable dt1 = this.CurrentOpenProject.CreateNewTable(worksheetName, bCreateDefaultColumns);
			return CreateNewWorksheet(dt1);
		}

		/// <summary>
		/// Creates a view content for a table.
		/// </summary>
		/// <param name="bCreateDefaultColumns">If true, a default x column and a default value column are created in the table.</param>
		/// <returns>The view content for the provided table.</returns>
		public Altaxo.Gui.Worksheet.Viewing.IWorksheetController CreateNewWorksheet(bool bCreateDefaultColumns)
		{
			return CreateNewWorksheet(this.CurrentOpenProject.DataTableCollection.FindNewTableName(), bCreateDefaultColumns);
		}

		/// <summary>
		/// Creates a new table in the root folder and the view content for that table.
		/// </summary>
		/// <returns>The content controller for that table.</returns>
		public Altaxo.Gui.Worksheet.Viewing.IWorksheetController CreateNewWorksheet()
		{
			return CreateNewWorksheetInFolder(Main.ProjectFolder.RootFolderName);
		}

		/// <summary>
		/// Creates a new table in a specified folder and the view content for that table.
		/// </summary>
		/// <returns>The content controller for that table.</returns>
		public Altaxo.Gui.Worksheet.Viewing.IWorksheetController CreateNewWorksheetInFolder(string folder)
		{
			return CreateNewWorksheet(this.CurrentOpenProject.DataTableCollection.FindNewTableNameInFolder(folder), false);
		}

		/// <summary>
		/// Creates a view content for a table.
		/// </summary>
		/// <param name="table">The table which should be viewed.</param>
		/// <returns>The view content for the provided table.</returns>
		public Altaxo.Gui.Worksheet.Viewing.IWorksheetController CreateNewWorksheet(Altaxo.Data.DataTable table)
		{
			return Current.Gui.Evaluate(CreateNewWorksheet_Unsynchronized, table);
		}

		private Altaxo.Gui.Worksheet.Viewing.IWorksheetController CreateNewWorksheet_Unsynchronized(Altaxo.Data.DataTable table)
		{
			if (table.ParentObject == null)
				this.CurrentOpenProject.DataTableCollection.Add(table);

			return CreateNewWorksheet(table, this.CurrentOpenProject.CreateNewTableLayout(table));
		}

		/// <summary>
		/// Creates a view content for a table.
		/// </summary>
		/// <param name="table">The table which should be viewed.</param>
		/// <param name="layout">The layout for the table.</param>
		/// <returns>The view content for the provided table.</returns>
		public Altaxo.Gui.Worksheet.Viewing.IWorksheetController CreateNewWorksheet(Altaxo.Data.DataTable table, Altaxo.Worksheet.WorksheetLayout layout)
		{
			return Current.Gui.Evaluate(CreateNewWorksheet_Unsynchronized, table, layout);
		}

		/// <summary>
		/// Creates a view content for a table.
		/// </summary>
		/// <param name="table">The table which should be viewed.</param>
		/// <param name="layout">The layout for the table.</param>
		/// <returns>The view content for the provided table.</returns>
		private Altaxo.Gui.Worksheet.Viewing.IWorksheetController CreateNewWorksheet_Unsynchronized(Altaxo.Data.DataTable table, Altaxo.Worksheet.WorksheetLayout layout)
		{
			layout.DataTable = table;
			var ctrl = new Altaxo.Gui.SharpDevelop.SDWorksheetViewContent(layout);
			var view = new Altaxo.Gui.Worksheet.Viewing.WorksheetViewWpf();
			ctrl.Controller.ViewObject = view;

			if (null != Current.Workbench)
				Current.Workbench.ShowView(ctrl);

			return ctrl.Controller;
		}

		/// <summary>
		/// Opens a view that shows the table <code>table</code>. If no view for the table can be found,
		/// a new default view is created for the table.
		/// </summary>
		/// <param name="table">The table for which a view must be found.</param>
		/// <returns>The view content for the provided table.</returns>
		public object OpenOrCreateWorksheetForTable(Altaxo.Data.DataTable table)
		{
			return Current.Gui.Evaluate(OpenOrCreateWorksheetForTable_Unsynchronized, table);
		}

		/// <summary>
		/// Opens a view that shows the table <code>table</code>. If no view for the table can be found,
		/// a new default view is created for the table.
		/// </summary>
		/// <param name="table">The table for which a view must be found.</param>
		/// <returns>The view content for the provided table.</returns>
		private object OpenOrCreateWorksheetForTable_Unsynchronized(Altaxo.Data.DataTable table)
		{
			// if a content exist that show that table, activate that content
			List<IViewContent> foundContent = SearchContentForDocument(table, 1);
			if (foundContent.Count > 0)
			{
				foundContent[0].WorkbenchWindow.SelectWindow();
				return foundContent[0];
			}

			// otherwise create a new Worksheet
			return CreateNewWorksheet_Unsynchronized(table);
		}

		/// <summary>
		/// This function will delete a data table and close the corresponding views.
		/// </summary>
		/// <param name="table">The data table to delete</param>
		/// <param name="force">If true, the table is deleted without safety question,
		/// if false, the user is ask before the table is deleted.</param>
		public void DeleteTable(Altaxo.Data.DataTable table, bool force)
		{
			Current.Gui.Execute(DeleteTable_Unsynchronized, table, force);
		}

		private void DeleteTable_Unsynchronized(Altaxo.Data.DataTable table, bool force)
		{
			if (!force &&
				false == Current.Gui.YesNoMessageBox("Are you sure to remove the table and the corresponding views?", "Attention", false))
				return;

			// close all windows
			List<IViewContent> foundContent = SearchContentForDocument(table, int.MaxValue);
			foreach (IViewContent content in foundContent)
			{
				content.WorkbenchWindow.CloseWindow(true);
			}

			Current.Project.DataTableCollection.Remove(table);

			// the following sequence is related to a bug encountered when closing a tabbed window by the program:
			// the active view content is not updated because the dockpanel lost the focus
			// to circumvent this, we focus on a new viewcontent, in this case the first one
			SelectFirstAvailableView();
		}

		/// <summary>This will remove the Worksheet <paramref>ctrl</paramref> from the corresponding forms collection.</summary>
		/// <param name="ctrl">The Worksheet to remove.</param>
		/// <remarks>No exception is thrown if the Form frm is not a member of the worksheet forms collection.</remarks>
		public void RemoveWorksheet(Altaxo.Gui.Worksheet.Viewing.IWorksheetController ctrl)
		{
			Current.Gui.Execute(RemoveWorksheet_Unsynchronized, ctrl);
		}

		private void RemoveWorksheet_Unsynchronized(Altaxo.Gui.Worksheet.Viewing.IWorksheetController ctrl)
		{
			foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection)
			{
				if ((content is Altaxo.Gui.IMVCControllerWrapper) &&
					object.ReferenceEquals(((Altaxo.Gui.IMVCControllerWrapper)content).MVCController, ctrl))
				{
					content.WorkbenchWindow.CloseWindow(true);
					break;
				}
			}
		}

		#endregion Worksheet functions

		#region Graph functions

		/// <summary>
		/// Creates a new graph document and the view content..
		/// </summary>
		/// <returns>The view content for the newly created graph.</returns>
		public Altaxo.Gui.Graph.Viewing.IGraphController CreateNewGraph()
		{
			return Current.Gui.Evaluate(CreateNewGraph_Unsynchronized);
		}

		private Altaxo.Gui.Graph.Viewing.IGraphController CreateNewGraph_Unsynchronized()
		{
			var graph = Altaxo.Graph.Gdi.GraphTemplates.TemplateWithXYPlotLayerWithG2DCartesicCoordinateSystem.CreateGraph(
					PropertyExtensions.GetPropertyContextOfProjectFolder(ProjectFolder.RootFolderName), "GRAPH", ProjectFolder.RootFolderName, false);
			return CreateNewGraph_Unsynchronized(graph);
		}

		/// <summary>
		/// Creates a new graph document in a specified folder and the view content.
		/// </summary>
		/// <param name="folderName">The folder name where to create the graph.</param>
		/// <returns>The view content for the newly created graph.</returns>
		public Altaxo.Gui.Graph.Viewing.IGraphController CreateNewGraphInFolder(string folderName)
		{
			return Current.Gui.Evaluate(CreateNewGraphInFolder_Unsynchronized, folderName);
		}

		private Altaxo.Gui.Graph.Viewing.IGraphController CreateNewGraphInFolder_Unsynchronized(string folderName)
		{
			var graph = Altaxo.Graph.Gdi.GraphTemplates.TemplateWithXYPlotLayerWithG2DCartesicCoordinateSystem.CreateGraph(
				PropertyExtensions.GetPropertyContextOfProjectFolder(folderName), null, folderName, false);

			return CreateNewGraph_Unsynchronized(graph);
		}

		/// <summary>
		/// Creates a new graph document and the view content..
		/// </summary>
		/// <param name="preferredName">The preferred name the new graph document should have.</param>
		/// <returns>The view content for the newly created graph.</returns>
		public Altaxo.Gui.Graph.Viewing.IGraphController CreateNewGraph(string preferredName)
		{
			return Current.Gui.Evaluate(CreateNewGraph_Unsynchronized, preferredName);
		}

		private Altaxo.Gui.Graph.Viewing.IGraphController CreateNewGraph_Unsynchronized(string preferredName)
		{
			var folderName = Main.ProjectFolder.GetFolderPart(preferredName);
			var graph = Altaxo.Graph.Gdi.GraphTemplates.TemplateWithXYPlotLayerWithG2DCartesicCoordinateSystem.CreateGraph(
				PropertyExtensions.GetPropertyContextOfProjectFolder(folderName), preferredName, folderName, false);
			return CreateNewGraph_Unsynchronized(graph);
		}

		/// <summary>
		/// Creates a new view content for a graph document.
		/// </summary>
		/// <param name="graph">The graph document.</param>
		/// <returns>The view content for the provided graph document.</returns>
		public Altaxo.Gui.Graph.Viewing.IGraphController CreateNewGraph(Altaxo.Graph.Gdi.GraphDocument graph)
		{
			return Current.Gui.Evaluate(CreateNewGraph_Unsynchronized, graph);
		}

		private Altaxo.Gui.Graph.Viewing.IGraphController CreateNewGraph_Unsynchronized(Altaxo.Graph.Gdi.GraphDocument graph)
		{
			if (graph == null)
				graph = Altaxo.Graph.Gdi.GraphTemplates.TemplateWithXYPlotLayerWithG2DCartesicCoordinateSystem.CreateGraph(
					PropertyExtensions.GetPropertyContextOfProjectFolder(ProjectFolder.RootFolderName), null, ProjectFolder.RootFolderName, false);

			// make sure that new graph is contained in project
			if (!Current.Project.GraphDocumentCollection.Contains(graph.Name))
				Current.Project.GraphDocumentCollection.Add(graph);

			Altaxo.Gui.SharpDevelop.SDGraphViewContent ctrl = new Altaxo.Gui.SharpDevelop.SDGraphViewContent(graph);

			//Altaxo.Graph.GUI.GraphView view = new Altaxo.Graph.GUI.GraphView();
			var view = new Altaxo.Gui.Graph.Viewing.GraphViewWpf();

			ctrl.Controller.ViewObject = view;

			if (null != Current.Workbench)
				Current.Workbench.ShowView(ctrl);

			return ctrl.Controller;
		}

		/// <summary>
		/// Opens a view that shows the graph <code>graph</code>. If no view for the graph can be found,
		/// a new default view is created.
		/// </summary>
		/// <param name="graph">The graph for which a view must be found.</param>
		/// <returns>The view content for the provided graph.</returns>
		public object OpenOrCreateGraphForGraphDocument(Altaxo.Graph.Gdi.GraphDocument graph)
		{
			return Current.Gui.Evaluate(OpenOrCreateGraphForGraphDocument_Unsynchronized, graph);
		}

		private object OpenOrCreateGraphForGraphDocument_Unsynchronized(Altaxo.Graph.Gdi.GraphDocument graph)
		{
			// if a content exist that show that graph, activate that content
			List<IViewContent> foundContent = SearchContentForDocument(graph, 1);
			if (foundContent.Count > 0)
			{
				foundContent[0].WorkbenchWindow.SelectWindow();
				return foundContent[0];
			}

			// otherwise create a new graph view
			return CreateNewGraph_Unsynchronized(graph);
		}

		/// <summary>
		/// This function will delete a graph document and close all corresponding views.
		/// </summary>
		/// <param name="graph">The graph document to delete.</param>
		/// <param name="force">If true, the graph document is deleted without safety question,
		/// if false, the user is ask before the graph document is deleted.</param>
		public void DeleteGraphDocument(Altaxo.Graph.Gdi.GraphDocument graph, bool force)
		{
			Current.Gui.Execute(DeleteGraphDocument_Unsynchronized, graph, force);
		}

		private void DeleteGraphDocument_Unsynchronized(Altaxo.Graph.Gdi.GraphDocument graph, bool force)
		{
			if (!force &&
				false == Current.Gui.YesNoMessageBox("Are you sure to remove the graph document and the corresponding views?", "Attention", false))
				return;

			// close all windows
			List<IViewContent> foundContent = SearchContentForDocument(graph, int.MaxValue);
			foreach (IViewContent content in foundContent)
			{
				content.WorkbenchWindow.CloseWindow(true);
			}

			Current.Project.GraphDocumentCollection.Remove(graph);

			// the following sequence is related to a bug encountered when closing a tabbed window by the program:
			// the active view content is not updated because the dockpanel lost the focus
			// to circumvent this, we focus on a new viewcontent, in this case the first one
			SelectFirstAvailableView();
		}

		/// <summary>This will remove the GraphController <paramref>ctrl</paramref> from the graph forms collection.</summary>
		/// <param name="ctrl">The GraphController to remove.</param>
		/// <remarks>No exception is thrown if the Form frm is not a member of the graph forms collection.</remarks>
		public void RemoveGraph(Altaxo.Gui.Graph.Viewing.IGraphController ctrl)
		{
			Current.Gui.Execute(RemoveGraph_Unsynchronized, ctrl);
		}

		private void RemoveGraph_Unsynchronized(Altaxo.Gui.Graph.Viewing.IGraphController ctrl)
		{
			foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection)
			{
				if ((content is Altaxo.Gui.IMVCControllerWrapper) &&
						object.ReferenceEquals(((Altaxo.Gui.IMVCControllerWrapper)content).MVCController, ctrl))
				{
					content.WorkbenchWindow.CloseWindow(true);
					break;
				}
			}
		}

		#endregion Graph functions

		private void EhProjectDirtyChanged(object sender, EventArgs e)
		{
			OnProjectDirtyChanged(new Altaxo.Main.ProjectEventArgs(this._currentProject));
		}

		//********* own events

		/// <summary>
		/// Fires the ProjectOpened event.
		/// </summary>
		/// <param name="e">Event args indicating which project was opened.</param>
		protected virtual void OnProjectOpened(ProjectEventArgs e)
		{
			if (ProjectOpened != null)
			{
				ProjectOpened(this, e);
			}

			OnProjectChanged();
		}

		/// <summary>
		/// Fires the project closed event.
		/// </summary>
		/// <param name="e">Indicates which project was closed.</param>
		protected virtual void OnProjectClosed(ProjectEventArgs e)
		{
			if (ProjectClosed != null)
			{
				ProjectClosed(this, e);
			}

			OnProjectChanged();
		}

		/// <summary>
		/// Fires the <see cref="ProjectRenamed" /> event.
		/// </summary>
		/// <param name="e">Indicates which project was renamed, and the old and the new name of the project.</param>
		protected virtual void OnRenameProject(ProjectRenameEventArgs e)
		{
			if (ProjectRenamed != null)
			{
				ProjectRenamed(this, e);
			}

			OnProjectChanged();
		}

		/// <summary>
		/// Fires the <see cref="ProjectDirtyChanged" /> event.
		/// </summary>
		/// <param name="e">Indicats on which project the dirty flag changed.</param>
		protected virtual void OnProjectDirtyChanged(ProjectEventArgs e)
		{
			if (ProjectDirtyChanged != null)
			{
				ProjectDirtyChanged(this, e);
			}

			OnProjectChanged();
		}

		/// <summary>
		/// Fires the <see cref="ProjectChanged" /> event. This occurs <b>after</b> the events <see cref="ProjectOpened" />,
		/// <see cref="ProjectClosed" />, <see cref="ProjectRenamed" />, and <see cref="ProjectDirtyChanged" /> event. Usefull if
		/// you not want to subscribe to the above mentioned single events.
		/// </summary>
		protected virtual void OnProjectChanged()
		{
			if (ProjectChanged != null)
				ProjectChanged(this, new ProjectEventArgs(this.CurrentOpenProject));
		}

		/// <summary>
		/// Fired when a project is opened or a new empty project is created.
		/// </summary>
		public event ProjectEventHandler ProjectOpened;

		/// <summary>
		/// Fired when the current open project is closed.
		/// </summary>
		public event ProjectEventHandler ProjectClosed;

		/// <summary>
		/// Fired when the current open project is renamed.
		/// </summary>
		public event ProjectRenameEventHandler ProjectRenamed;

		/// <summary>
		/// Fired when the dirty state of the project changed.
		/// </summary>
		public event ProjectEventHandler ProjectDirtyChanged;

		/// <summary>
		/// Event fired <b>after</b> any of the following other events is fired: <see cref="ProjectOpened" />,
		/// <see cref="ProjectClosed" />, <see cref="ProjectRenamed" />, and <see cref="ProjectDirtyChanged" />.
		/// </summary>
		public event ProjectEventHandler ProjectChanged;
	}
}