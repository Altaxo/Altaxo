#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2020 Dr. Dirk Lellinger
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
using Altaxo.Main.Services;

namespace Altaxo.Main
{
  public enum ProjectEventKind
  {
    /// <summary>Occurs before a new project is opened from file. Not fired when a new project is created internally.</summary>
    ProjectOpening,

    /// <summary>Occurs after a new project is opened, both internally or from file.</summary>
    ProjectOpened,

    /// <summary>Occurs when a project is about to be closed.</summary>
    ProjectClosing,

    /// <summary>Occurs after a project is closed.</summary>
    ProjectClosed,

    /// <summary>Occurs after a project has been renamed.</summary>
    ProjectRenamed,

    /// <summary>Occurs when the dirty flag of a project has been changed.</summary>
    ProjectDirtyChanged,
  }

  /// <summary>
  /// Usefull to indicate the change of an Altaxo project.
  /// </summary>
  public class ProjectEventArgs : EventArgs
  {
    /// <summary>
    /// Gets the project. When opening a new project, this property is null.
    /// </summary>
    /// <value>
    /// The project.
    /// </value>
    public IProject? Project { get; private set; }

    /// <summary>
    /// Gets the kind of the project event.
    /// </summary>
    /// <value>
    /// The kind of the project event.
    /// </value>
    public ProjectEventKind ProjectEventKind { get; private set; }

    /// <summary>The name of the project after renaming. Null if no name is yet attached.</summary>
    public string? NewName { get; private set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="project">The project which was changed.</param>
    /// <param name="fileName">The file name of the project.</param>
    /// <param name="eventKind">The kind of the event.</param>
    public ProjectEventArgs(IProject? project, string? fileName, ProjectEventKind eventKind)
    {
      Project = project;
      NewName = fileName;
      ProjectEventKind = eventKind;
    }
  }

  /// <summary>
  /// Usefull to indicate the renaming of an Altaxo project.
  /// </summary>
  public class ProjectRenamedEventArgs : ProjectEventArgs
  {
    /// <summary>
    /// The name of the project before renaming. Null if the project 
    /// </summary>
    public string? OldName { get; private set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="renamedProject">The project renamed.</param>
    /// <param name="oldName">The old name of the project.</param>
    /// <param name="newName">The new name of the project.</param>
    public ProjectRenamedEventArgs(IProject renamedProject, string? oldName, string? newName)
        : base(renamedProject, newName, ProjectEventKind.ProjectRenamed)
    {
      OldName = oldName;
    }
  }

  /// <summary>
  /// Manages the currently open Altaxo project.
  /// </summary>
  [GlobalService("IProjectService", FallbackImplementation = typeof(ProjectServiceDummyImpl))]
  public interface IProjectService
  {
    /// <summary>
    /// Getter / setter for the currently open project.
    /// </summary>
    IProject? CurrentProject { get; }

    /// <summary>
    /// Gets the file name for the currently open project. Is null if the project has not got a file name for now.
    /// </summary>
    PathName? CurrentProjectFileName { get; }

    /// <summary>
    /// Gets the object that represents the storage of the current project on disk. If the current project was opened as a COM object, the value is null.
    /// </summary>
    /// <value>
    /// The current project storage, if the project was opened from the file system. The value is null if the project was opened from a COM stream.
    /// </value>
    IProjectArchiveManager? CurrentProjectArchiveManager { get; }

    /// <summary>
    /// Exchanges the current project archive manager without disposing the old manager. This function is intended to be used twice in succession:
    /// 1st to temporarily exchange the current archive manager by another on, and then to change back the new archive manager with the old one.
    /// </summary>
    /// <param name="newManager">The new manager that becomes the current manager after this call.</param>
    /// <returns>The archive manager that is currently set.</returns>
    IProjectArchiveManager? ExchangeCurrentProjectArchiveManagerTemporarilyWithoutDisposing(IProjectArchiveManager? newManager);



    /// <summary>
    /// Creates the very first document. Used internal into the autostart command.
    /// </summary>
    void CreateInitialProject();

    #region Opening a project

    /// <summary>
    /// Opens a Altaxo project. If the current project is dirty, and <paramref name="showUserInteraction"/> is <c>true</c>, the user is ask to save the current project before.
    /// </summary>
    /// <param name="pathName">Can be a <see cref="FileName"/>, if the project is file-based, or a <see cref="DirectoryName"/>, if the project is based on a directory.</param>
    /// <param name="showUserInteraction">If <c>true</c>, the user will see dialog if the current project is dirty and needs to be saved. In addition, the user will see
    /// an error dialog if the opening of the new document fails due to exceptions. If this parameter is <c>false</c>, then the old document is forced
    /// to close (without saving). If there is a exception during opening, this exception is thrown.</param>
    void OpenProject(PathName pathName, bool showUserInteraction);

    /// <summary>
    /// Opens a project from an archive. This function is intended for opening the project from a COM stream on a freshly started application.
    /// Thus it is assumed that no old project has to be saved before!
    /// </summary>
    /// <param name="istream">The input stream.</param>
    void OpenProjectFromArchive(IProjectArchive istream);

    #endregion Opening a project

    #region Saving a project

    /// <summary>
    /// Asks the user whether or not the project should be saved, and saves it in case the user answers with yes.
    /// </summary>
    /// <param name="e">Cancel event args. On return, the e.Cancel property is set to true, if the users cancel the saving.</param>
    void AskForSavingOfProject(System.ComponentModel.CancelEventArgs e);

    /// <summary>
    /// Asks the user for a file name for the current project, and then saves the project under the given name.
    /// </summary>
    void SaveProjectAs();

    /// <summary>
    /// This command is used if in embedded object mode.
    /// It saves the current project to a file,
    /// but don't set the current file name of the project (in project service).
    /// Furthermore, the title in the title bar is not influenced by the saving.
    /// </summary>
    void SaveProjectCopyAs();

    /// <summary>
    /// Saves the project under the given file name, using the standard archive format.
    /// </summary>
    /// <param name="fileOrFolderName">If the project should be saved into a file, is should be a <see cref="FileName"/>. If the project should be saved into a folder, use a <see cref="DirectoryName"/> instead.</param>
    void SaveProject(PathName fileOrFolderName);

    /// <summary>
    /// Saves the project in the provided archive. Is intended for saving the project into a COM stream only.
    /// For regular saving into the file system, use one of the other Save.. methods.
    /// </summary>
    /// <param name="archive">Archive to save the project into</param>
    /// <returns>A dictionary where the keys are the archive entry names that where used to store the project items that are the values. The dictionary contains only those project items that need further handling (e.g. late load handling).</returns>
    IDictionary<string, IProjectItem> SaveProject(IProjectArchive archive);

    /// <summary>
    /// Saves a project under the current file name.
    /// </summary>
    void SaveProject();

    #endregion Saving a project

    #region Closing a project

    /// <summary>
    /// Closes a project. If the project is dirty, and <paramref name="forceClose"/> is <c>false</c>, the user is asked to save the project.
    /// </summary>
    /// <param name="forceClose">If <c>false</c> and the project is dirty, the user will be asked whether he really wants to close the project.
    /// If <c>true</c>, the project is closed without user interaction.</param>
    /// <returns>True if the project was closed; false otherwise.</returns>
    bool CloseProject(bool forceClose);

    /// <summary>
    /// Disposes the entire project and sets the current project to null.
    /// Attention: This function is intended to be used during application shutdown only.
    /// </summary>
    void DisposeProjectAndSetToNull();

    #endregion Closing a project

    /// <summary>
    /// Returns true if the given document has at least one open view in the workbench.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <returns>True if there is at least one open view for the document.</returns>
    bool HasDocumentAnOpenView(object document);

    /// <summary>
    /// Gets a set of all open documents, i.e. GraphDocuments, DataTables. (Not Worksheets).
    /// </summary>
    /// <returns>The set of all open documents.</returns>
    HashSet<object> GetOpenDocuments();

    /// <summary>
    /// Closes all open views for a given document.
    /// </summary>
    /// <param name="document">The document.</param>
    void CloseDocumentViews(object document);

    /// <summary>
    /// Shows a view for the given document.
    /// </summary>
    /// <param name="document">The document to open.</param>
    void ShowDocumentView(object document);

    /// <summary>
    /// This function will delete a project item and close all corresponding views.
    /// </summary>
    /// <param name="document">The document (project item) to delete.</param>
    /// <param name="force">If true, the document is deleted without safety question; otherwise, the user is ask before the graph document is deleted.</param>
    void DeleteDocument(IProjectItem document, bool force);

    /// <summary>
    /// Fired when a project is opened or a new empty project is created.
    /// </summary>
    event EventHandler<ProjectEventArgs> ProjectOpened;

    /// <summary>
    /// Fired when the current open project is closed.
    /// </summary>
    event EventHandler<ProjectEventArgs> ProjectClosed;

    /// <summary>
    /// Fired when the current open project is renamed.
    /// </summary>
    event EventHandler<ProjectRenamedEventArgs> ProjectRenamed;

    /// <summary>
    /// Fired when the dirty state of the project changed.
    /// </summary>
    event EventHandler<ProjectEventArgs> ProjectDirtyChanged;

    /// <summary>
    /// Event fired <b>after</b> any of the following other events is fired: <see cref="ProjectOpened" />,
    /// <see cref="ProjectClosed" />, <see cref="ProjectRenamed" />, and <see cref="ProjectDirtyChanged" />.
    /// </summary>
    event EventHandler<ProjectEventArgs> ProjectChanged;

    /// <summary>
    /// Determines whether the provided file extension is a project extension.
    /// </summary>
    /// <param name="extension">The file extension (with point as the first character).</param>
    /// <returns>
    ///   <c>true</c> if the specified extension is a project extension; otherwise, <c>false</c>.
    /// </returns>
    bool IsProjectFileExtension(string extension);

    /// <summary>
    /// Gets all possible file extensions for acceptable project files.
    /// </summary>
    /// <value>
    /// All possible file extensions for acceptable project files.
    /// </value>
    IEnumerable<string> ProjectFileExtensions { get; }

    /// <summary>
    /// Tests if the provided file name to have an extension that is associated with the extension of a project item 
    /// (a specific item of the project), and if it is, tries to open the project item and adds it to the current project.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="forceTrialRegardlessOfExtension">If true, it is tried to deserialize the object in the file regardless of the file extension.</param>
    /// <returns>True if the fileName has an extension that is a project document extension, and the document could successfully be opened; otherwise, false.</returns>
    bool TryOpenProjectItemFile(FileName fileName, bool forceTrialRegardlessOfExtension);

    /// <summary>
    /// Gets the title that should be shown as the main window title.
    /// </summary>
    /// <returns>The title of the main window with respect to the current state.</returns>
    string GetMainWindowTitle();

    /// <summary>
    /// Executes any neccessary actions immediately before running the main application. When calling this, the services and the workbench are already initialized.
    /// The method can for instance load any files that are given in the command line.
    /// </summary>
    /// <param name="cmdArgs">The command line arguments (all of them, without preprocessing).</param>
    /// <param name="cmdParameter">The command line arguments that are parameters (i.e. those arguments beginning with '-' or '/').</param>
    /// <param name="cmdFiles">The command line arguments that are files (i.e. those arguments <b>not</b> beginning with '-' or '/').</param>
    void ExecuteActionsImmediatelyBeforeRunningApplication(string[] cmdArgs, string[] cmdParameter, string[] cmdFiles);
  }

  public class ProjectServiceDummyImpl : IProjectService
  {
    public IProject CurrentProject => throw new NotImplementedException();

    public PathName CurrentProjectFileName => throw new NotImplementedException();

    public IProjectArchiveManager CurrentProjectArchiveManager => throw new NotImplementedException();

    public IEnumerable<string> ProjectFileExtensions => throw new NotImplementedException();

#pragma warning disable CS0067
    public event EventHandler<ProjectEventArgs>? ProjectOpened;
    public event EventHandler<ProjectEventArgs>? ProjectClosed;
    public event EventHandler<ProjectRenamedEventArgs>? ProjectRenamed;
    public event EventHandler<ProjectEventArgs>? ProjectDirtyChanged;
    public event EventHandler<ProjectEventArgs>? ProjectChanged;
#pragma warning restore CS0067

    public void AskForSavingOfProject(CancelEventArgs e)
    {
      throw new NotImplementedException();
    }

    public void CloseDocumentViews(object document)
    {
      throw new NotImplementedException();
    }

    public bool CloseProject(bool forceClose)
    {
      throw new NotImplementedException();
    }

    public void CreateInitialProject()
    {
      throw new NotImplementedException();
    }

    public void DeleteDocument(IProjectItem document, bool force)
    {
      throw new NotImplementedException();
    }

    public void DisposeProjectAndSetToNull()
    {
      throw new NotImplementedException();
    }

    public IProjectArchiveManager? ExchangeCurrentProjectArchiveManagerTemporarilyWithoutDisposing(IProjectArchiveManager? newManager)
    {
      throw new NotImplementedException();
    }

    public void ExecuteActionsImmediatelyBeforeRunningApplication(string[] cmdArgs, string[] cmdParameter, string[] cmdFiles)
    {
      throw new NotImplementedException();
    }

    public string GetMainWindowTitle()
    {
      throw new NotImplementedException();
    }

    public HashSet<object> GetOpenDocuments()
    {
      throw new NotImplementedException();
    }

    public bool HasDocumentAnOpenView(object document)
    {
      throw new NotImplementedException();
    }

    public bool IsProjectFileExtension(string extension)
    {
      throw new NotImplementedException();
    }

    public void OpenProject(PathName pathName, bool showUserInteraction)
    {
      throw new NotImplementedException();
    }

    public void OpenProjectFromArchive(IProjectArchive istream)
    {
      throw new NotImplementedException();
    }

    public void SaveProject(PathName fileOrFolderName)
    {
      throw new NotImplementedException();
    }

    public IDictionary<string, IProjectItem> SaveProject(IProjectArchive archive)
    {
      throw new NotImplementedException();
    }

    public void SaveProject()
    {
      throw new NotImplementedException();
    }

    public void SaveProjectAs()
    {
      throw new NotImplementedException();
    }

    public void SaveProjectCopyAs()
    {
      throw new NotImplementedException();
    }

    public void ShowDocumentView(object document)
    {
      throw new NotImplementedException();
    }

    public bool TryOpenProjectItemFile(FileName fileName, bool forceTrialRegardlessOfExtension)
    {
      throw new NotImplementedException();
    }
  }

}
