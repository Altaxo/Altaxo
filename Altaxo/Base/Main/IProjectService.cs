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

namespace Altaxo.Main
{
	/// <summary>
	/// The event handler to indicate the changing of an Altaxo project.
	/// </summary>
	public delegate void ProjectEventHandler(object sender, ProjectEventArgs e);

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
		public IProject Project { get; private set; }

		/// <summary>
		/// Gets the kind of the project event.
		/// </summary>
		/// <value>
		/// The kind of the project event.
		/// </value>
		public ProjectEventKind ProjectEventKind { get; private set; }

		/// <summary>The name of the project after renaming.</summary>
		public string NewName { get; private set; }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="project">The project which was changed.</param>
		/// <param name="fileName">The file name of the project.</param>
		/// <param name="eventKind">The kind of the event.</param>
		public ProjectEventArgs(IProject project, string fileName, ProjectEventKind eventKind)
		{
			this.Project = project;
			this.NewName = fileName;
			this.ProjectEventKind = eventKind;
		}
	}

	/// <summary>
	/// The event handler to indicate the renaming of a project.
	/// </summary>
	public delegate void ProjectRenameEventHandler(object sender, ProjectRenamedEventArgs e);

	/// <summary>
	/// Usefull to indicate the renaming of an Altaxo project.
	/// </summary>
	public class ProjectRenamedEventArgs : ProjectEventArgs
	{
		/// <summary>
		/// The name of the project before renaming.
		/// </summary>
		public string OldName { get; private set; }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="renamedProject">The project renamed.</param>
		/// <param name="oldName">The old name of the project.</param>
		/// <param name="newName">The new name of the project.</param>
		public ProjectRenamedEventArgs(IProject renamedProject, string oldName, string newName)
				: base(renamedProject, newName, ProjectEventKind.ProjectRenamed)
		{
			this.OldName = oldName;
		}
	}

	/// <summary>
	/// Manages the currently open Altaxo project.
	/// </summary>
	public interface IProjectService
	{
		/// <summary>
		/// Getter / setter for the currently open project.
		/// </summary>
		IProject CurrentProject { get; }

		/// <summary>
		/// Gets the file name for the currently open project. Is null if the project has not got a file name for now.
		/// </summary>
		string CurrentProjectFileName { get; }

		/// <summary>
		/// Creates the very first document. Used internal into the autostart command.
		/// </summary>
		void CreateInitialProject();

		/// <summary>
		/// Opens a Altaxo project. If the current project is dirty, and <paramref name="withoutUserInteraction"/> is <c>false</c>, the user is ask to save the current project before.
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="withoutUserInteraction">If <c>false</c>, the user will see dialog if the current project is dirty and needs to be saved. In addition, the user will see
		/// an error dialog if the opening of the new document fails due to exceptions. If this parameter is <c>true</c>, then the old document is forced
		/// to close (without saving). If there is a exception during opening, this exception is thrown.</param>
		void OpenProject(string filename, bool withoutUserInteraction);

		/// <summary>
		/// Loads the project for an input stream
		/// </summary>
		/// <param name="istream">The input stream.</param>
		/// <returns>Null if the project was successfully loaded; or an error string otherwise.</returns>
		string LoadProjectFromStream(System.IO.Stream istream);

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
		/// This command is used if in embedded object mode. It saves the current project to a file,
		/// but don't set the current file name of the project (in project service). Furthermore, the title in the title bar is not influenced by the saving.
		/// </summary>
		void SaveProjectCopyAs();

		/// <summary>
		/// Saves the project under the given file name.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		void SaveProject(string fileName);

		/// <summary>
		/// Saves the project in the provided stream.
		/// </summary>
		/// <param name="stream">Stream to save the project into</param>
		/// <returns>Null if everything was saved sucessfully, or an exception if not.</returns>
		Exception SaveProject(System.IO.Stream stream);

		/// <summary>
		/// Saves a project under the current file name.
		/// </summary>
		void SaveProject();

		/// <summary>
		/// Closes a project. If the project is dirty, and <paramref name="forceClose"/> is <c>false</c>, the user is asked to save the project.
		/// </summary>
		/// <param name="forceClose">If <c>false</c> and the project is dirty, the user will be asked whether he really wants to close the project.
		/// If <c>true</c>, the project is closed without user interaction.</param>
		/// <returns>True if the project was closed; false otherwise.</returns>
		bool CloseProject(bool forceClose);

		/// <summary>
		/// Disposes the whole project and sets the current project to null.
		/// </summary>
		void DisposeProjectAndSetToNull();

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
		/// Fired when a project is opened or a new empty project is created.
		/// </summary>
		event ProjectEventHandler ProjectOpened;

		/// <summary>
		/// Fired when the current open project is closed.
		/// </summary>
		event ProjectEventHandler ProjectClosed;

		/// <summary>
		/// Fired when the current open project is renamed.
		/// </summary>
		event ProjectRenameEventHandler ProjectRenamed;

		/// <summary>
		/// Fired when the dirty state of the project changed.
		/// </summary>
		event ProjectEventHandler ProjectDirtyChanged;

		/// <summary>
		/// Event fired <b>after</b> any of the following other events is fired: <see cref="ProjectOpened" />,
		/// <see cref="ProjectClosed" />, <see cref="ProjectRenamed" />, and <see cref="ProjectDirtyChanged" />.
		/// </summary>
		event ProjectEventHandler ProjectChanged;

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
		/// Tests if the provided file name to have an extension that is associated with the extension of a project document
		/// (a specific part of the project), and if it is, tries to open the document and add it to the current project.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		/// <param name="forceTrialRegardlessOfExtension">If true, it is tried to deserialize the object in the file regardless of the file extension.</param>
		/// <returns>True if the fileName has an extension that is a project document extension, and the document could successfully be opened; otherwise, false.</returns>
		bool TryOpenProjectDocumentFile(string fileName, bool forceTrialRegardlessOfExtension);

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
}
