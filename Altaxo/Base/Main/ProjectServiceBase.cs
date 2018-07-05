using Altaxo.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;
using Altaxo.Gui.Workbench;
using Altaxo.Gui;
using Altaxo.Main.Services;

namespace Altaxo.Dom
{
	public abstract class ProjectServiceBase : IProjectService
	{
		protected string _applicationName = "Application";

		protected IProject _currentProject;

		protected string _currentProjectFileName;

		public event ProjectEventHandler ProjectOpened;

		public event ProjectEventHandler ProjectClosed;

		public event ProjectRenameEventHandler ProjectRenamed;

		public event ProjectEventHandler ProjectDirtyChanged;

		public event ProjectEventHandler ProjectChanged;

		#region Current project and project file name handling

		/// <summary>
		/// Fires the <see cref="ProjectChanged" /> event. This occurs <b>after</b> the events <see cref="ProjectOpened" />,
		/// <see cref="ProjectClosed" />, <see cref="ProjectRenamed" />, and <see cref="ProjectDirtyChanged" /> event. Usefull if
		/// you not want to subscribe to the above mentioned single events.
		/// </summary>
		protected virtual void OnProjectChanged(ProjectEventArgs e)
		{
			switch (e.ProjectEventKind)
			{
				case ProjectEventKind.ProjectOpening:
					break;

				case ProjectEventKind.ProjectOpened:
					ProjectOpened?.Invoke(this, e);
					break;

				case ProjectEventKind.ProjectClosing:
					break;

				case ProjectEventKind.ProjectClosed:
					ProjectClosed?.Invoke(this, e);
					break;

				case ProjectEventKind.ProjectRenamed:
					ProjectRenamed?.Invoke(this, (ProjectRenamedEventArgs)e);
					break;

				case ProjectEventKind.ProjectDirtyChanged:
					ProjectDirtyChanged?.Invoke(this, e);
					break;

				default:
					break;
			}

			ProjectChanged?.Invoke(this, e);
		}

		/// <summary>
		/// Returns the currently open project.
		/// For setting, call <see cref="SetCurrentProject(IProject, string)"/>.
		/// </summary>
		public IProject CurrentProject { get => _currentProject; }

		/// <summary>
		/// Gets the file name for the currently open project. Is null if the project has not got a file name for now.
		/// For setting, call <see cref="SetCurrentProject(IProject, string)"/>.
		/// </summary>
		public virtual string CurrentProjectFileName
		{
			get { return _currentProjectFileName; }
		}

		/// <summary>
		/// Sets the current project instance and file name. No events raised (events should be raised by the caller).
		/// The old project instance will be disposed of.
		/// </summary>
		/// <param name="project">The new project.</param>
		/// <param name="projectFileName">Name of the new project file (for internally build instances, null).</param>
		protected void SetCurrentProject(IProject project, string projectFileName)
		{
			var oldProject = _currentProject;
			string oldProjectFileName = _currentProjectFileName;

			if (null != _currentProject)
			{
				_currentProject.IsDirtyChanged -= EhProjectDirtyChanged;
			}

			_currentProject = project;
			_currentProjectFileName = projectFileName;

			if (_currentProject != null)
			{
				_currentProject.IsDirtyChanged += this.EhProjectDirtyChanged;
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
						System.Diagnostics.Debug.WriteLine("Exception during disposing of old project. Details:");
						System.Diagnostics.Debug.Write(ex.ToString());
					}
				}
			}
		}

		private void EhProjectDirtyChanged(object sender, EventArgs e)
		{
			OnProjectChanged(new Altaxo.Main.ProjectEventArgs(this._currentProject, this._currentProject?.Name, ProjectEventKind.ProjectDirtyChanged));
		}

		#endregion Current project and project file name handling

		#region Project saving

		/// </inheritdoc>
		public abstract IEnumerable<string> ProjectFileExtensions { get; }

		/// </inheritdoc>
		public void AskForSavingOfProject(CancelEventArgs e)
		{
			string text = Current.ResourceService.GetString("Altaxo.Project.AskForSavingOfProjectDialog.Text");
			string caption = Current.ResourceService.GetString("Altaxo.Project.AskForSavingOfProjectDialog.Caption");
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

				if (this.CurrentProject.IsDirty)
					e.Cancel = true; // Cancel if the saving was not successfull
			}
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
		public virtual void SaveProject(string filename)
		{
			string oldFileName = this.CurrentProjectFileName;
			this._currentProjectFileName = filename; // set file name silently, because
			if (oldFileName != filename)
			{
				OnProjectChanged(new ProjectRenamedEventArgs(this._currentProject, oldFileName, filename));
			}

			FileUtility.ObservedSave(new NamedFileOperationDelegate(this.InternalSave),
					FileName.Create(filename),
					Current.ResourceService.GetString("Altaxo.Project.CantSaveProjectErrorText"),
					FileErrorPolicy.ProvideAlternative);
		}

		/// <summary>
		/// Asks the user for a file name for the current project, and then saves the project under the given name.
		/// </summary>
		public void SaveProjectAs()
		{
			SaveFileOptions options = new SaveFileOptions();
			var fileExtensions = "*" + string.Join(";*", ProjectFileExtensions);
			options.AddFilter(fileExtensions, string.Format("{0} ({1})", "Project files", fileExtensions));
			options.AddFilter("*.*", StringParser.Parse("${res: Altaxo.FileFilter.AllFiles}"));
			options.OverwritePrompt = true;
			options.AddExtension = true;

			if (Current.Gui.ShowSaveFileDialog(options))
			{
				string filename = options.FileName;
				SaveProject(filename);
				Current.GetService<IRecentOpen>()?.AddRecentProject(new FileName(filename));
				Current.StatusBar.SetMessage(filename + ": " + Current.ResourceService.GetString("Altaxo.Project.ProjectSavedMessage"));
			}
		}

		/// <summary>
		/// This command is used if in embedded object mode. It saves the current project to a file,
		/// but don't set the current file name of the project (in project service). Furthermore, the title in the title bar is not influenced by the saving.
		/// </summary>
		public void SaveProjectCopyAs()
		{
			SaveFileOptions options = new SaveFileOptions();
			var fileExtensions = "*" + string.Join(";*", ProjectFileExtensions);
			options.AddFilter(fileExtensions, string.Format("{0} ({1})", "Project files", fileExtensions));
			options.AddFilter("*.*", StringParser.Parse("${res: Altaxo.FileFilter.AllFiles}"));
			options.OverwritePrompt = true;
			options.AddExtension = true;

			if (Current.Gui.ShowSaveFileDialog(options))
			{
				string filename = options.FileName;

				FileUtility.ObservedSave(
						new NamedFileOperationDelegate(this.InternalSave),
						FileName.Create(filename),
						Current.ResourceService.GetString("Altaxo.Project.CantSaveProjectErrorText"),
						FileErrorPolicy.ProvideAlternative);

				var recentService = Current.GetService<IRecentOpen>();
				if (null == recentService)
					recentService.AddRecentProject(FileName.Create(filename));
			}
		}

		/// <summary>
		/// Internal routine to save a project under a given name.
		/// </summary>
		/// <param name="filename"></param>
		protected virtual void InternalSave(FileName filename)
		{
			bool fileAlreadyExists = System.IO.File.Exists(filename);

			string tempFileName = null;
			if (fileAlreadyExists)
			{
				tempFileName = System.IO.Path.GetTempFileName();
			}

			string testfilename = tempFileName ?? filename;

			Exception savingException = null;
			using (var myStream = new System.IO.FileStream(testfilename, System.IO.FileMode.Create, FileAccess.Write, FileShare.None))
			{
				savingException = SaveProject(myStream);

				if (null == savingException)
				{
					savingException = InternalTestIntegrityOfSavedProjectFile(myStream, testfilename);
				}
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

		protected virtual Exception InternalTestIntegrityOfSavedProjectFile(Stream myStream, string fileName)
		{
			return null;
		}

		public abstract Exception SaveProject(System.IO.Stream myStream);

		#endregion Project saving

		#region Project opening

		/// <summary>
		/// Loads the project from an (already open) input stream. Do not use this call when opening the project from a file (use <see cref="LoadProjectFromFile(string)"/> instead).
		/// </summary>
		/// <param name="istream">The input stream.</param>
		/// <returns>
		/// Null if the project was successfully loaded; or an error string otherwise.
		/// </returns>
		public string LoadProjectFromStream(Stream istream)
		{
			return InternalLoadProjectFromStream(istream, null);
		}

		/// <summary>
		/// Opens a project.
		/// If the current project is dirty, and <paramref name="withoutUserInteraction"/> is <c>false</c>, the user is ask to save the current project before.
		/// </summary>
		/// <param name="filename">The file name of the project to open.</param>
		/// <param name="withoutUserInteraction">If <c>false</c>, the user will see dialog if the current project is dirty and needs to be saved. In addition, the user will see
		/// an error dialog if the opening of the new document fails due to exceptions. If this parameter is <c>true</c>, then the old document is forced
		/// to close (without saving). If there is a exception during opening, this exception is thrown.</param>
		public void OpenProject(string filename, bool withoutUserInteraction)
		{
			if (!FileUtility.TestFileExists(filename))
			{
				return;
			}

			if (CurrentProject != null && CurrentProject.IsDirty && !withoutUserInteraction)
			{
				System.ComponentModel.CancelEventArgs e = new System.ComponentModel.CancelEventArgs();
				AskForSavingOfProject(e);

				if (e.Cancel == true)
					return;
			}

			Current.StatusBar.SetMessage("${res:MainWindow.StatusBar.OpeningCombineMessage}");

			try
			{
				bool wasValidProjectLoaded = false;
				foreach (var projFileExtension in ProjectFileExtensions)
				{
					if (Path.GetExtension(filename).ToUpperInvariant() == projFileExtension.ToUpperInvariant())
					{
						string validproject = Path.ChangeExtension(filename, projFileExtension);
						if (File.Exists(validproject))
						{
							LoadProjectFromFile(validproject);
							wasValidProjectLoaded = true;
							break;
						}
					}
				}
				if (!wasValidProjectLoaded)
				{
					LoadProjectFromFile(filename);
				}
			}
			catch (Exception ex)
			{
				if (withoutUserInteraction)
					throw;
				else
					Current.Gui.ErrorMessageBox(ex.Message);
			}

			Current.StatusBar.SetMessage("${res:MainWindow.StatusBar.ReadyMessage}");
		}

		/// <summary>
		/// Opens a Altaxo project from a project file (without asking the user). The old project is closed without asking the user.
		/// </summary>
		/// <param name="filename"></param>
		protected virtual void LoadProjectFromFile(string filename)
		{
			string errorText;
			using (System.IO.FileStream myStream = new System.IO.FileStream(filename, System.IO.FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				errorText = InternalLoadProjectFromStream(myStream, filename);
				myStream.Close();
			}

			if (errorText.Length != 0)
				throw new ApplicationException(errorText);

			var recentService = Current.GetService<IRecentOpen>();

			if (recentService != null)
				recentService.AddRecentProject(FileName.Create(filename));
		}

		/// <summary>
		/// Opens a Altaxo project from a stream. Any existing old project will be closed without confirmation.
		/// </summary>
		/// <param name="myStream">The stream from which to load the project.</param>
		/// <param name="filename">Either the filename of the file which stored the document, or null (e.g. myStream is a MemoryStream).</param>
		protected abstract string InternalLoadProjectFromStream(System.IO.Stream myStream, string filename);

		/// <inheritdoc/>
		public abstract bool TryOpenProjectDocumentFile(string fileName, bool forceTrialRegardlessOfExtension);

		#endregion Project opening

		public void CloseDocumentViews(object document)
		{
			foreach (var viewContent in GetViewContentsForDocument(document).ToArray())
			{
				Current.Workbench.CloseContent(viewContent);
			}
		}

		public virtual bool CloseProject(bool forceClose)
		{
			if (CurrentProject != null && _currentProject.IsDirty && !forceClose)
			{
				var e = new CancelEventArgs();
				AskForSavingOfProject(e);
				if (true == e.Cancel)
					return false;
			}

			var oldProject = _currentProject;
			var oldProjectName = _currentProjectFileName;

			if (oldProject != null)
				OnProjectChanged(new ProjectEventArgs(oldProject, oldProjectName, ProjectEventKind.ProjectClosing));

			Current.Workbench.CloseAllViews();
			SetCurrentProject(null, null);

			if (oldProject != null)
				OnProjectChanged(new ProjectEventArgs(oldProject, oldProjectName, ProjectEventKind.ProjectClosed));

			// now create a new project

			OnProjectChanged(new ProjectEventArgs(null, null, ProjectEventKind.ProjectOpening));
			var newProject = InternalCreateNewProject();
			SetCurrentProject(newProject, null);
			OnProjectChanged(new ProjectEventArgs(newProject, null, ProjectEventKind.ProjectOpened));

			return true;
		}

		public virtual void CreateInitialProject()
		{
			if (null != _currentProject)
				throw new InvalidOperationException("There should be no document before creating the initial document");

			OnProjectChanged(new ProjectEventArgs(null, null, ProjectEventKind.ProjectOpening));
			var newProject = InternalCreateNewProject();
			SetCurrentProject(newProject, null);
			OnProjectChanged(new ProjectEventArgs(newProject, null, ProjectEventKind.ProjectOpened));
		}

		/// <summary>
		/// Create a new project, and returns it, without wiring any infrastructure to it yet.
		/// </summary>
		/// <returns>A new project.</returns>
		protected abstract IProject InternalCreateNewProject();

		public void DisposeProjectAndSetToNull()
		{
			SetCurrentProject(null, null);
		}

		/// <inheritdoc/>
		public virtual string GetMainWindowTitle()
		{
			var comManager = Current.ComManager;
			if (comManager != null && comManager.IsInEmbeddedMode && null != comManager.EmbeddedObject)
				return GetMainWindowTitleWithComManagerInEmbeddedMode();
			else
				return GetMainWindowTitleWithoutComManagerInEmbeddedMode();
		}

		/// <summary>
		/// Gets the main window title without being COM manager in embedded mode.
		/// </summary>
		/// <returns>The title of the main window when the Com manager is not in embedded mode.</returns>
		protected virtual string GetMainWindowTitleWithoutComManagerInEmbeddedMode()
		{
			_applicationName = StringParser.Parse("${AppName}");
			var isDirty = _currentProject?.IsDirty ?? false;
			var fileName = string.IsNullOrEmpty(CurrentProjectFileName) ? "Untitled" : CurrentProjectFileName;
			return string.Format("{0} - {1}{2}", _applicationName, fileName, isDirty ? "*" : "");
		}

		/// <summary>
		/// Gets the main window title if the Com manager is in embedded mode.
		/// </summary>
		/// <returns>The main window title for the case that the Com manager is in embedded mode.</returns>
		/// <exception cref="InvalidProgramException">This function must be called only if Current.ComManager is in embedded mode and has an embedded object</exception>
		protected virtual string GetMainWindowTitleWithComManagerInEmbeddedMode()
		{
			var comManager = Current.ComManager;
			if (!(comManager != null && comManager.IsInEmbeddedMode && null != comManager.EmbeddedObject))
				throw new InvalidProgramException("This function must be called only if Current.ComManager is in embedded mode and has an embedded object");

			_applicationName = StringParser.Parse("${AppName}");

			// embedded mode - ComManager has the title
			System.Text.StringBuilder title = new System.Text.StringBuilder();
			// we are in embedded mode
			title.Append(_applicationName);
			title.Append(" ");
			title.Append(comManager.EmbeddedObject.GetType().Name);

			if (comManager.EmbeddedObject is IProjectItem projectItem)
			{
				title.Append(" ");
				title.Append(projectItem.Name);
			}

			if (Altaxo.Current.IProjectService.CurrentProject != null && Altaxo.Current.IProjectService.CurrentProject.IsDirty)
				title.Append("*");

			title.Append(" in ");
			title.Append(comManager.ContainerDocumentName);
			title.Append(" - ");
			title.Append(comManager.ContainerApplicationName);
			return title.ToString();
		}

		public HashSet<object> GetOpenDocuments()
		{
			throw new NotImplementedException();
		}

		public bool HasDocumentAnOpenView(object document)
		{
			return GetViewContentsForDocument(document).Any();
		}

		/// <summary>
		/// Determines whether the provided file extension is a project file extension. This function uses the
		/// abstract function <see cref="ProjectFileExtensions"/> to determine if the given argument is a project file extension
		/// </summary>
		/// <param name="extension">The file extension (with point as the first character).</param>
		/// <returns>
		///   <c>true</c> if the specified extension is a project file extension; otherwise, <c>false</c>.
		/// </returns>
		public virtual bool IsProjectFileExtension(string extension)
		{
			extension = extension.ToLowerInvariant();
			foreach (var ext in ProjectFileExtensions)
				if (ext.ToLowerInvariant() == extension)
					return true;

			return false;
		}

		public virtual void ShowDocumentView(object document)
		{
			var viewcontent = Current.Workbench.GetViewModel<IViewContent>(document); // search for an already present view content

			if (null == viewcontent) // if not found, try to create a new viewcontent
			{
				viewcontent = (IViewContent)Current.Gui.GetControllerAndControl(new object[] { document }, typeof(IViewContent));
			}

			if (null != viewcontent)
			{
				Current.Workbench.ShowView(viewcontent, true);
			}
		}

		/// <summary>
		/// This function will delete a project document and close all corresponding views.
		/// </summary>
		/// <param name="document">The document (project item) to delete.</param>
		/// <param name="force">If true, the document is deleted without safety question; otherwise, the user is ask before the graph document is deleted.</param>
		public abstract void DeleteDocument(IProjectItem document, bool force);

		/// <summary>
		/// Returns all currently open views that show the given document object <code>document</code>, either directly (<see cref="IMVCController.ModelObject"/> is the document),
		/// or indirectly, if <see cref="IMVCController.ModelObject"/> is of type <see cref="Main.IProjectItemPresentationModel"/>, by comparing with
		/// <see cref="Main.IProjectItemPresentationModel.Document"/>.
		/// </summary>
		/// <param name="document">The document for which views must be found.</param>
		/// <returns>An enumeration containing all views that show the given document.</returns>
		public virtual IEnumerable<IViewContent> GetViewContentsForDocument(object document)
		{
			foreach (IViewContent content in Current.Workbench.ViewContentCollection)
			{
				object modelobject = content.ModelObject;

				if (object.ReferenceEquals(modelobject, document))
				{
					yield return content;
				}
				else if (modelobject is Main.IProjectItemPresentationModel pipm1)
				{
					if (object.ReferenceEquals(document, pipm1.Document))
					{
						yield return content;
					}
					else if (pipm1.Document is Main.IProjectItemPresentationModel pipm2)
					{
						if (object.ReferenceEquals(document, pipm2.Document))
							yield return content;
					}
				}
			}
		}

		public abstract void ExecuteActionsImmediatelyBeforeRunningApplication(string[] cmdArgs, string[] cmdParameter, string[] cmdFiles);
	}
}
