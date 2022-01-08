#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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

#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Altaxo.AddInItems;
using Altaxo.Gui;
using Altaxo.Gui.Main;
using Altaxo.Gui.Workbench;
using Altaxo.Main;
using Altaxo.Main.Services;

namespace Altaxo.Dom
{
  public abstract class ProjectServiceBase : IProjectService
  {
    protected string _applicationName = "Application";

    protected IProject? _currentProject;

    protected IProjectArchiveManager _currentProjectArchiveManager = new UnnamedProjectArchiveManager();

    public event EventHandler<ProjectEventArgs>? ProjectOpened;

    public event EventHandler<ProjectEventArgs>? ProjectClosed;

    public event EventHandler<ProjectRenamedEventArgs>? ProjectRenamed;

    public event EventHandler<ProjectEventArgs>? ProjectDirtyChanged;

    public event EventHandler<ProjectEventArgs>? ProjectChanged;

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
    /// For setting, call <see cref="SetCurrentProject(IProject, bool)"/>.
    /// </summary>
    public IProject? CurrentProject { get => _currentProject; }

    /// <summary>
    /// Gets the file name for the currently open project. Is null if the project has not got a file name for now.
    /// For setting, call <see cref="SetCurrentProject(IProject, bool)"/>.
    /// </summary>
    public virtual PathName? CurrentProjectFileName
    {
      get { return _currentProjectArchiveManager.FileOrFolderName; }
    }

    /// <summary>
    /// Gets or sets the current project archive manager.
    /// </summary>
    /// <value>
    /// The current project archive manager.
    /// </value>
    /// <exception cref="ArgumentNullException">CurrentProjectArchiveManager</exception>
    public virtual IProjectArchiveManager CurrentProjectArchiveManager
    {
      get
      {
        return _currentProjectArchiveManager;
      }
      set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(CurrentProjectArchiveManager));

        if (!object.ReferenceEquals(_currentProjectArchiveManager, value))
        {
          string? oldFileName = null;
          string? newFileName = null;
          if (!(_currentProjectArchiveManager is null))
          {
            oldFileName = _currentProjectArchiveManager.FileOrFolderName;
            _currentProjectArchiveManager.FileOrFolderNameChanged -= EhFileOrFolderNameChanged;
            _currentProjectArchiveManager.Dispose();
          }

          _currentProjectArchiveManager = value;

          if (!(_currentProjectArchiveManager is null))
          {
            _currentProjectArchiveManager.FileOrFolderNameChanged += EhFileOrFolderNameChanged;
            newFileName = _currentProjectArchiveManager.FileOrFolderName;
          }

          if (!(_currentProject is null))
            OnProjectChanged(new ProjectRenamedEventArgs(_currentProject, oldFileName, newFileName));
        }
      }
    }

    /// <summary>
    /// Exchanges the current project archive manager without disposing the old manager. This function is intended to be used twice in succession:
    /// 1st to temporarily exchange the current archive manager by another on, and then to change back the new archive manager with the old one.
    /// </summary>
    /// <param name="newManager">The new manager that becomes the current manager after this call.</param>
    /// <returns>The archive manager that is currently set.</returns>
    public IProjectArchiveManager? ExchangeCurrentProjectArchiveManagerTemporarilyWithoutDisposing(IProjectArchiveManager? newManager)
    {
      if (_currentProject is null)
        throw new InvalidProgramException();

      var oldManager = _currentProjectArchiveManager ?? throw new InvalidProgramException($"{nameof(_currentProjectArchiveManager)} should never be null");
      _currentProjectArchiveManager = newManager ?? throw new ArgumentNullException(nameof(newManager));
      OnProjectChanged(new ProjectRenamedEventArgs(_currentProject, oldManager.FileOrFolderName, newManager.FileOrFolderName));
      return oldManager;
    }

    protected virtual void EhFileOrFolderNameChanged(object? sender, NameChangedEventArgs e)
    {
      if (_currentProject is null)
        throw new InvalidProgramException();
      OnProjectChanged(new ProjectRenamedEventArgs(_currentProject, e.OldName, e.NewName));
    }

    /// <summary>
    /// Sets the current project instance, and set it's file name to null. No events raised (events should be raised by the caller).
    /// The old project instance will be disposed of.
    /// </summary>
    /// <param name="project">The new project to be set. The file name of this project will be null (thus the project is considered unnamed).</param>
    /// <param name="asUnnamedProject">If true, the current <see cref="CurrentProjectArchiveManager"/> is replaced by a dummy project archive manager that represents an unnamed project.</param>
    protected void SetCurrentProject(IProject? project, bool asUnnamedProject)
    {
      var oldProject = _currentProject;

      if (_currentProject is not null)
      {
        _currentProject.IsDirtyChanged -= EhProjectDirtyChanged;
      }

      _currentProject = project;
      if (asUnnamedProject)
      {
        CurrentProjectArchiveManager = new UnnamedProjectArchiveManager();
      }

      if (_currentProject is not null)
      {
        _currentProject.IsDirtyChanged += EhProjectDirtyChanged;
      }

      if (!object.ReferenceEquals(oldProject, _currentProject)) // Project instance has changed
      {
        if (oldProject is not null)
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

    protected virtual void EhProjectDirtyChanged(object? sender, EventArgs e)
    {
      OnProjectChanged(new Altaxo.Main.ProjectEventArgs(_currentProject, _currentProject?.Name, ProjectEventKind.ProjectDirtyChanged));
    }

    #endregion Current project and project file name handling

    #region Project saving

    /// <inheritdoc/>
    public abstract IEnumerable<string> ProjectFileExtensions { get; }

    /// <inheritdoc/>
    public void AskForSavingOfProject(CancelEventArgs e)
    {
      string? text = Current.ResourceService.GetString("Altaxo.Project.AskForSavingOfProjectDialog.Text");
      string? caption = Current.ResourceService.GetString("Altaxo.Project.AskForSavingOfProjectDialog.Caption");
      bool? dlgresult = Current.Gui.YesNoCancelMessageBox(text, caption, null);

      if (dlgresult is null) // Cancel
      {
        e.Cancel = true;
      }
      else if (true == dlgresult) // Yes
      {
        if (CurrentProjectFileName is not null)
          SaveProject();
        else
          SaveProjectAs();

        if (_currentProject?.IsDirty ?? false)
          e.Cancel = true; // Cancel if the saving was not successfull
      }
    }

    /// <summary>
    /// Saves a project under the current file name.
    /// </summary>
    public void SaveProject()
    {
      if (CurrentProjectFileName is null)
        throw new InvalidOperationException("The current project has not file name yet.");

      SaveProject(CurrentProjectFileName);
    }

    /// <summary>
    /// Saves the current project under a provided file name. If the provided file name differs
    /// from the current file name, a project renaming event is triggered.
    /// </summary>
    /// <param name="fileOrFolderName">If the project should be saved into a file, is should be a <see cref="FileName"/>. If the project should be saved into a folder, use a <see cref="DirectoryName"/> instead.</param>
    public virtual void SaveProject(PathName fileOrFolderName)
    {
      if (_currentProject is null)
        throw new InvalidProgramException();

      var oldFileName = CurrentProjectFileName;
      var currentProjectFileName = fileOrFolderName; // set file name silently
      if (oldFileName != fileOrFolderName)
      {
        OnProjectChanged(new ProjectRenamedEventArgs(_currentProject, oldFileName, fileOrFolderName));
      }

      FileUtility.ObservedSave(new NamedFileOrFolderOperationDelegate(InternalSave),
          fileOrFolderName,
          Current.ResourceService.GetString("Altaxo.Project.CantSaveProjectErrorText"),
          FileErrorPolicy.ProvideAlternative);
    }

    /// <summary>
    /// Asks the user for a file name for the current project, and then saves the project under the given name.
    /// </summary>
    public void SaveProjectAs()
    {
      var options = new SaveFileOptions();
      var fileExtensions = "*" + string.Join(";*", ProjectFileExtensions);
      options.AddFilter(fileExtensions, string.Format("{0} ({1})", "Project files", fileExtensions));
      options.AddFilter("*.*", StringParser.Parse("${res:Altaxo.FileFilter.AllFiles}"));
      options.OverwritePrompt = true;
      options.AddExtension = true;

      if (Current.Gui.ShowSaveFileDialog(options))
      {
        var filename = new FileName(options.FileName);
        SaveProject(filename);
        Current.GetService<IRecentOpen>()?.AddRecentProject(filename);
        Current.StatusBar.SetMessage(filename + ": " + Current.ResourceService.GetString("Altaxo.Project.ProjectSavedMessage"));
      }
    }

    /// <summary>
    /// This command is used if in embedded object mode. It saves the current project to a file,
    /// but don't set the current file name of the project (in project service). Furthermore, the title in the title bar is not influenced by the saving.
    /// </summary>
    public void SaveProjectCopyAs()
    {
      var options = new SaveFileOptions();
      var fileExtensions = "*" + string.Join(";*", ProjectFileExtensions);
      options.AddFilter(fileExtensions, string.Format("{0} ({1})", "Project files", fileExtensions));
      options.AddFilter("*.*", StringParser.Parse("${res:Altaxo.FileFilter.AllFiles}"));
      options.OverwritePrompt = true;
      options.AddExtension = true;

      if (Current.Gui.ShowSaveFileDialog(options))
      {
        string filename = options.FileName;

        FileUtility.ObservedSave(
            new NamedFileOrFolderOperationDelegate(InternalSave),
            FileName.Create(filename),
            Current.ResourceService.GetString("Altaxo.Project.CantSaveProjectErrorText"),
            FileErrorPolicy.ProvideAlternative);

        if (Current.GetService<IRecentOpen>() is { } recentService)
          recentService.AddRecentProject(FileName.Create(filename));
      }
    }

    /// <summary>
    /// Internal routine to save a project under a given name.
    /// </summary>
    /// <param name="filename"></param>
    protected virtual void InternalSave(PathName filename)
    {
      if (_currentProject is null)
        throw new InvalidProgramException();

      // a dictionary where the keys are the archive entry names that where used to store the project items that are the values. The dictionary contains only those project items that need further handling (e.g. late load handling)
      IDictionary<string, IProjectItem>? entryNameItemDictionary = null;

      if (!filename.Equals(CurrentProjectArchiveManager?.FileOrFolderName))
      {
        var saveProjectManager = InternalCreateProjectArchiveManagerFromFileOrFolderLocation(filename) ?? throw new ApplicationException($"Can't find a storage manager for file/folder {filename}");

        if (saveProjectManager.GetType() != CurrentProjectArchiveManager?.GetType())
        {
          CurrentProjectArchiveManager = saveProjectManager;
        }


        if (CurrentProjectArchiveManager is IFileBasedProjectArchiveManager fileBasedManager)
          entryNameItemDictionary = fileBasedManager.SaveAs((FileName)filename, SaveProjectAndWindowsState);
        else if (CurrentProjectArchiveManager is IFolderBasedProjectArchiveManager folderBasedManager)
          entryNameItemDictionary = folderBasedManager.SaveAs((DirectoryName)filename, SaveProjectAndWindowsState);
        else
          throw new NotImplementedException($"Storage manager type {CurrentProjectArchiveManager} is not implemented here.");

      }
      else
      {
        // we save the project under the same name, thus we can use the same storage manager
        CurrentProjectArchiveManager.Save(SaveProjectAndWindowsState);
      }

      _currentProject.ClearIsDirty(CurrentProjectArchiveManager, entryNameItemDictionary);
    }



    /// <summary>
    /// Saves a project.
    /// </summary>
    /// <param name="archiveToSaveTo">The project archive to save the project to.</param>
    /// <param name="archiveToCopyFrom">The project archive that represents the last state of saving before this saving Can be used to copy some of the data,
    /// that were not changed inbetween savings. This parameter can be null, for instance, if no such archive exists.</param>
    /// <returns>A dictionary where the keys are the entrynames that where used to store the project items that are the values.</returns>
    public abstract IDictionary<string, IProjectItem> SaveProjectAndWindowsState(IProjectArchive archiveToSaveTo, IProjectArchive? archiveToCopyFrom);

    /// <summary>
    /// Saves a project.
    /// </summary>
    /// <param name="archiveToSaveTo">The project archive to save the project to.</param>
    /// <returns>A dictionary where the keys are the entrynames that where used to store the project items that are the values.</returns>
    public IDictionary<string, IProjectItem> SaveProject(IProjectArchive archiveToSaveTo) => SaveProjectAndWindowsState(archiveToSaveTo, null);

    #endregion Project saving

    #region Project opening

    /// <summary>
    /// Loads the project from project archive.
    /// </summary>
    /// <param name="archive">The project archive to load from.</param>
    public void OpenProjectFromArchive(IProjectArchive archive)
    {
      InternalLoadProjectAndWindowsStateFromArchive(archive);
    }

    /// <summary>
    /// Opens a project.
    /// If the current project is dirty, and <paramref name="showUserInteraction"/> is <c>true</c>, the user is ask to save the current project before.
    /// </summary>
    /// <param name="fileOrFolderName">The file name of the project to open.</param>
    /// <param name="showUserInteraction">If <c>true</c>, the user will see dialog if the current project is dirty and needs to be saved. In addition, the user will see
    /// an error dialog if the opening of the new document fails due to exceptions. If this parameter is <c>false</c>, then the old document is forced
    /// to close (without saving). If there is a exception during opening, this exception is thrown.</param>
    public void OpenProject(PathName fileOrFolderName, bool showUserInteraction)
    {
      if (fileOrFolderName is null)
        throw new ArgumentNullException(nameof(fileOrFolderName));

      if (!fileOrFolderName.Exists())
      {
        return;
      }

      if (CurrentProject is not null && CurrentProject.IsDirty && showUserInteraction)
      {
        var e = new System.ComponentModel.CancelEventArgs();
        AskForSavingOfProject(e);

        if (e.Cancel == true)
          return;
      }

      Current.StatusBar.SetMessage("${res:MainWindow.StatusBar.OpeningCombineMessage}");

      try
      {
        LoadProjectFromFileOrFolder(fileOrFolderName, showUserInteraction);
      }
      catch (Exception ex)
      {
        if (showUserInteraction)
          Current.Gui.ErrorMessageBox(string.Concat(ex.Message, "\r\nDetails:\r\n", ex.ToString()));
        else
          throw;
      }

      Current.StatusBar.SetMessage("${res:MainWindow.StatusBar.ReadyMessage}");
    }

    /// <summary>
    /// Opens a Altaxo project from a project file (without asking the user). The old project is closed without asking the user.
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="showUserInteraction">If true, and the file is read-only, a dialog box is asking the user whether to open the file in read-only mode.
    /// If false, and the file is read-only, the file will be opened in read-only-mode.</param>
    protected virtual void LoadProjectFromFileOrFolder(PathName filename, bool showUserInteraction = true)
    {
      var projectArchiveManager = InternalCreateProjectArchiveManagerFromFileOrFolderLocation(filename) ?? throw new ApplicationException($"Can not find any archive manager that can handle the file / folder {filename}");
      CurrentProjectArchiveManager = projectArchiveManager;

      if (projectArchiveManager is IFileBasedProjectArchiveManager fileProjectArchiveManager)
        fileProjectArchiveManager.LoadFromFile((FileName)filename, InternalLoadProjectAndWindowsStateFromArchive, showUserInteraction);
      else if (projectArchiveManager is IFolderBasedProjectArchiveManager folderProjectArchiveManager)
        folderProjectArchiveManager.LoadFromFolder((DirectoryName)filename, InternalLoadProjectAndWindowsStateFromArchive);
      else
        throw new ApplicationException($"Unexpected type of ProjectArchiveManager: {projectArchiveManager.GetType().FullName}");

      var recentService = Current.GetService<IRecentOpen>();

      if (recentService is not null)
        recentService.AddRecentProject(filename);
    }

    /// <summary>
    /// Loads the project and the state of the windows from a project archive.
    /// </summary>
    /// <param name="archive">The project archive to load from.</param>
    protected abstract void InternalLoadProjectAndWindowsStateFromArchive(IProjectArchive archive);

    /// <summary>
    /// Enumerates the registered types for a manager that implements <see cref="IFileBasedProjectArchiveManager"/> and searches for the type that can handle the provided file or folder.
    /// </summary>
    /// <param name="fileOrFolderName">Name of the file or folder.</param>
    /// <returns></returns>
    protected abstract IProjectArchiveManager InternalCreateProjectArchiveManagerFromFileOrFolderLocation(PathName fileOrFolderName);

    /// <inheritdoc/>
    public abstract bool TryOpenProjectItemFile(FileName fileName, bool forceTrialRegardlessOfExtension);

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
      if (!(_currentProject is null) && _currentProject.IsDirty && !forceClose)
      {
        var e = new CancelEventArgs();
        AskForSavingOfProject(e);
        if (true == e.Cancel)
          return false;
      }

      var oldProject = _currentProject;
      var oldProjectName = CurrentProjectFileName;

      if (oldProject is not null)
        OnProjectChanged(new ProjectEventArgs(oldProject, oldProjectName, ProjectEventKind.ProjectClosing));

      Current.Workbench.CloseAllViews();
      // SetCurrentProject(null, asUnnamedProject: true);

      if (oldProject is not null)
        OnProjectChanged(new ProjectEventArgs(oldProject, oldProjectName, ProjectEventKind.ProjectClosed));

      // now create a new project

      OnProjectChanged(new ProjectEventArgs(null, null, ProjectEventKind.ProjectOpening));
      var newProject = InternalCreateNewProject();
      SetCurrentProject(newProject, asUnnamedProject: true);
      OnProjectChanged(new ProjectEventArgs(newProject, null, ProjectEventKind.ProjectOpened));

      return true;
    }

    public virtual void CreateInitialProject()
    {
      if (_currentProject is not null)
        throw new InvalidOperationException("There should be no document before creating the initial document");

      OnProjectChanged(new ProjectEventArgs(null, null, ProjectEventKind.ProjectOpening));
      var newProject = InternalCreateNewProject();
      SetCurrentProject(newProject, asUnnamedProject: true);
      OnProjectChanged(new ProjectEventArgs(newProject, null, ProjectEventKind.ProjectOpened));
    }

    /// <summary>
    /// Create a new project, and returns it, without wiring any infrastructure to it yet.
    /// </summary>
    /// <returns>A new project.</returns>
    protected abstract IProject InternalCreateNewProject();

    public void DisposeProjectAndSetToNull()
    {
      SetCurrentProject(InternalCreateNewProject(), asUnnamedProject: true);
    }

    /// <inheritdoc/>
    public virtual string GetMainWindowTitle()
    {
      if (Current.ComManager is { } comManager && comManager.IsInEmbeddedMode && comManager.EmbeddedObject is not null)
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
      if (!(comManager is not null && comManager.IsInEmbeddedMode && comManager.EmbeddedObject is not null))
        throw new InvalidProgramException("This function must be called only if Current.ComManager is in embedded mode and has an embedded object");

      _applicationName = StringParser.Parse("${AppName}");

      // embedded mode - ComManager has the title
      var title = new System.Text.StringBuilder();
      // we are in embedded mode
      title.Append(_applicationName);
      title.Append(" ");
      title.Append(comManager.EmbeddedObject.GetType().Name);

      if (comManager.EmbeddedObject is IProjectItem projectItem)
      {
        title.Append(" ");
        title.Append(projectItem.Name);
      }

      if (Altaxo.Current.IProjectService.CurrentProject is not null && Altaxo.Current.IProjectService.CurrentProject.IsDirty)
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

    public virtual object? ShowDocumentView(object document)
    {
      var viewcontent = Current.Workbench.GetViewModel<IViewContent>(document); // search for an already present view content

      if (viewcontent is null) // if not found, try to create a new viewcontent
      {
        viewcontent = (IViewContent?)Current.Gui.GetControllerAndControl(new object[] { document }, typeof(IViewContent));
      }

      if (viewcontent is not null)
      {
        Current.Workbench.ShowView(viewcontent, true);
      }

      return viewcontent;
    }

    /// <summary>
    /// This function will delete a project item and close the corresponding views.
    /// </summary>
    /// <param name="document">The project item to delete</param>
    /// <param name="force">If true, the document is deleted without safety question,
    /// if false, the user is ask before the document is deleted.</param>
    public void DeleteDocument(Main.IProjectItem document, bool force)
    {
      if (document is null)
        throw new ArgumentNullException(nameof(document));

      Current.Dispatcher.InvokeIfRequired(DeleteDocument_Unsynchronized, document, force);
    }



    protected virtual void DeleteDocument_Unsynchronized(Main.IProjectItem document, bool force)
    {
      if (!force &&
        false == Current.Gui.YesNoMessageBox(string.Format("Are you sure to remove the {0} and the corresponding views?", document.GetType().Name), "Attention", false))
        return;

      // close all windows
      var foundContent = Current.Workbench.GetViewModels<IViewContent>(document);
      foreach (IViewContent content in foundContent.ToArray()) // ToArray because we change the collection by closing
      {
        if (content.CloseCommand.CanExecute(true))
          content.CloseCommand.Execute(true);
      }

      Current.IProject.RemoveItem(document);

      // the following sequence is related to a bug encountered when closing a tabbed window by the program:
      // the active view content is not updated because the dockpanel lost the focus
      // to circumvent this, we focus on a new viewcontent, in this case the first one
      SelectFirstAvailableView();
    }
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

    /// <summary>
    /// Opens a view that shows the <paramref name="document"/>. If no view for the document can be found,
    /// a new default view is created.
    /// </summary>
    /// <param name="document">The document for which a view must be found. This parameter must not be null.</param>
    /// <returns>The view content for the provided document.</returns>
    public object OpenOrCreateViewContentForDocument(IProjectItem document)
    {
      if (document is null)
        throw new ArgumentNullException(nameof(document));

      return Current.Dispatcher.InvokeIfRequired(OpenOrCreateViewContentForDocument_Unsynchronized, document);
    }

    protected object OpenOrCreateViewContentForDocument_Unsynchronized(IProjectItem document)
    {
      // make sure the document is contained in our current project
      if (!Current.IProject.ContainsItem(document))
        Current.IProject.AddItemWithThisOrModifiedName(document);

      // if a content exist that show that graph, activate that content
      var foundContent = GetViewContentsForDocument(document).FirstOrDefault();
      if (foundContent is not null)
      {
        foundContent.IsActive = true;
        foundContent.IsSelected = true;
        return foundContent;
      }
      else // not found
      {
        return CreateNewViewContent_Unsynchronized(document);
      }
    }

    protected IMVCController CreateNewViewContent_Unsynchronized(IProjectItem document)
    {
      if (document is null)
        throw new ArgumentNullException(nameof(document));

      // make sure that the item is already contained in the project
      if (!Current.IProject.ContainsItem(document))
        Current.IProject.AddItemWithThisOrModifiedName(document);

      var types = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(AbstractViewContent));
      System.Reflection.ConstructorInfo? cinfo = null;
      object? viewContent = null;
      foreach (Type type in types)
      {
        if ((cinfo = type.GetConstructor(new Type[] { document.GetType() })) is not null)
        {
          var par = cinfo.GetParameters()[0];
          if (par.ParameterType != typeof(object)) // ignore view content which takes the most generic type
          {
            viewContent = cinfo.Invoke(new object[] { document });
            break;
          }
        }
      }

      if (viewContent is null)
      {
        foreach (IProjectItemDisplayBindingDescriptor descriptor in AddInTree.BuildItems<IProjectItemDisplayBindingDescriptor>("/Altaxo/Workbench/ProjectItemDisplayBindings", this, false))
        {
          if (descriptor.ProjectItemType == document.GetType())
          {
            if ((cinfo = descriptor.ViewContentType.GetConstructor(new Type[] { document.GetType() })) is not null)
            {
              var par = cinfo.GetParameters()[0];
              if (par.ParameterType != typeof(object)) // ignore view content which takes the most generic type
              {
                viewContent = cinfo.Invoke(new object[] { document });
                break;
              }
            }
          }
        }
      }

      var controller = viewContent as IMVCController;
      if (controller is not null && controller.ViewObject is null)
      {
        Current.Gui.FindAndAttachControlTo(controller);
      }

      if (Current.Workbench is { } wb && viewContent is { } vc)
        wb.ShowView(vc, true);


      return controller ?? throw new InvalidOperationException($"No controller found for project item of type {document.GetType()}");
    }


    /// <summary>This will remove the controller and its view from the Gui.</summary>
    /// <param name="controller">The view content to remove.</param>
    public void CloseViewContent(IViewContent controller)
    {
      Current.Dispatcher.InvokeIfRequired(CloseViewContent_Unsynchronized, controller);
    }

    private void CloseViewContent_Unsynchronized(IViewContent controller)
    {
      if (controller is not null)
        Current.Workbench.CloseContent(controller);
    }


    protected void SelectFirstAvailableView()
    {
      var content = Current.Workbench.ViewContentCollection.FirstOrDefault();

      if (content is not null)
      {
        content.IsActive = true;
        content.IsSelected = true;
      }
    }

    public abstract void ExecuteActionsImmediatelyBeforeRunningApplication(string[] cmdArgs, string[] cmdParameter, string[] cmdFiles);
  }
}
