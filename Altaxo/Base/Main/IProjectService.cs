#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using Altaxo.Graph.Gdi;

namespace Altaxo.Main
{
  /// <summary>
  /// The event handler to indicate the changing of an Altaxo project.
  /// </summary>
  public delegate void ProjectEventHandler(object sender, ProjectEventArgs e);
  

  /// <summary>
  /// Usefull to indicate the change of an Altaxo project.
  /// </summary>
  public class ProjectEventArgs : EventArgs
  {
    private Altaxo.AltaxoDocument project;
    
    /// <summary>
    /// Returns the project which was changed.
    /// </summary>
    public Altaxo.AltaxoDocument Project
    {
      get 
      {
        return project;
      }
    }
    
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="renamedProject">The project which was changed.</param>
    public ProjectEventArgs(Altaxo.AltaxoDocument renamedProject)
    {
      this.project = renamedProject;
    }
  }


  /// <summary>
  /// The event handler to indicate the renaming of a project.
  /// </summary>
  public delegate void ProjectRenameEventHandler(object sender, ProjectRenameEventArgs e);
  
  /// <summary>
  /// Usefull to indicate the renaming of an Altaxo project.
  /// </summary>
  public class ProjectRenameEventArgs : ProjectEventArgs
  { 
  
    string   oldName;
    string   newName;
    
    /// <summary>
    /// The name of the project before renaming.
    /// </summary>
  
    public string OldName 
    {
      get 
      {
        return oldName;
      }
    }
    
    /// <summary>
    /// The name of the project after renaming.
    /// </summary>
    public string NewName 
    {
      get 
      {
        return newName;
      }
    }
    
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="renamedProject">The project renamed.</param>
    /// <param name="oldName">The old name of the project.</param>
    /// <param name="newName">The new name of the project.</param>
    public ProjectRenameEventArgs(Altaxo.AltaxoDocument renamedProject, string oldName, string newName)
      : base(renamedProject)
    {
      this.oldName = oldName;
      this.newName = newName;
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
    Altaxo.AltaxoDocument CurrentOpenProject {  get; set; } 

    /// <summary>
    /// Gets the file name for the currently open project. Is null if the project has not got a file name for now.
    /// </summary>
    string CurrentProjectFileName { get; } 

    /// <summary>
    /// Opens a Altaxo project. If the current project is dirty, the user is ask for saving the current project.
    /// </summary>
    /// <param name="filename"></param>
    void OpenProject(string filename);
    

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
    /// Saves a project under the current file name.
    /// </summary>
    void SaveProject();

    /// <summary>
    /// Closes a project. If the project is dirty, the user is asked for saving the project.
    /// </summary>
    void CloseProject();

    /// <summary>
    /// Saves the state of the main window into a zipped file.
    /// </summary>
    /// <param name="zippedStream">The file stream of the zip file.</param>
    /// <param name="info">The serialization info used to serialize the state of the main window.</param>
    void SaveWindowStateToZippedFile(Altaxo.Main.ICompressedFileContainerStream zippedStream, Altaxo.Serialization.Xml.XmlStreamSerializationInfo info);

      /// <summary>
    /// Returns true if the given document has at least one open view in the workbench.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <returns>True if there is at least one open view for the document.</returns>
    bool HasDocumentAnOpenView(object document);

    /// <summary>
    /// This function will delete a data table and close the corresponding views.
    /// </summary>
    /// <param name="table">The data table to delete.</param>
    /// <param name="force">If true, the table is deleted without safety question,
    /// if false, the user is ask before the table is deleted.</param>
    void DeleteTable(Altaxo.Data.DataTable table, bool force);

    /// <summary>
    /// This function will delete a graph document and close all corresponding views.
    /// </summary>
    /// <param name="graph">The graph document to delete.</param>
    /// <param name="force">If true, the graph document is deleted without safety question,
    /// if false, the user is ask before the graph document is deleted.</param>
    void DeleteGraphDocument(GraphDocument graph, bool force);

    /// <summary>
    /// Creates a new table and the view content for the newly created table.
    /// </summary>
    /// <returns>The content controller for that table.</returns>
    Altaxo.Worksheet.GUI.IWorksheetController CreateNewWorksheet();

    /// <summary>
    /// Creates a view content for a table.
    /// </summary>
    /// <param name="table">The table which should be viewed.</param>
    /// <returns>The view content for the provided table.</returns>
    Altaxo.Worksheet.GUI.IWorksheetController CreateNewWorksheet(Altaxo.Data.DataTable table);

    /// <summary>
    /// Creates a view content for a table.
    /// </summary>
    /// <param name="table">The table which should be viewed.</param>
    /// <param name="layout">The layout for the table.</param>
    /// <returns>The view content for the provided table.</returns>
    Altaxo.Worksheet.GUI.IWorksheetController CreateNewWorksheet(Altaxo.Data.DataTable table, Altaxo.Worksheet.WorksheetLayout layout);


    /// <summary>
    /// Opens a view that shows the table <code>table</code>. If no view for the table can be found,
    /// a new default view is created for the table.
    /// </summary>
    /// <param name="table">The table for which a view must be found.</param>
    /// <returns>The view content for the provided table.</returns>
    /// <remarks>The returned object is usually a MVC controller that is the controller for that table.</remarks>
    object OpenOrCreateWorksheetForTable(Altaxo.Data.DataTable table);

    /// <summary>This will remove the Worksheet <paramref>ctrl</paramref> from the corresponding forms collection.</summary>
    /// <param name="ctrl">The Worksheet to remove.</param>
    /// <remarks>No exception is thrown if the Form frm is not a member of the worksheet forms collection.</remarks>
    void RemoveWorksheet(Altaxo.Worksheet.GUI.WorksheetController ctrl);

    /// <summary>
    /// Creates a new graph document and the view for this newly created graph document.
    /// </summary>
    /// <returns>The view content for the newly created graph.</returns>
    Altaxo.Graph.GUI.IGraphController CreateNewGraph();

    /// <summary>
    /// Creates a new view content for a graph document.
    /// </summary>
    /// <param name="graph">The graph document.</param>
    /// <returns>The view content for the provided graph document.</returns>
    Altaxo.Graph.GUI.IGraphController CreateNewGraph(GraphDocument graph);

    /// <summary>
    /// Opens a view that shows the graph <code>graph</code>. If no view for the graph can be found,
    /// a new default view is created.
    /// </summary>
    /// <param name="graph">The graph for which a view must be found.</param>
    /// <returns>The view content for the provided graph.</returns>
    object OpenOrCreateGraphForGraphDocument(GraphDocument graph);

    /// <summary>This will remove the Graph <paramref>ctrl</paramref> from the corresponding forms collection.</summary>
    /// <param name="ctrl">The Graph to remove.</param>
    /// <remarks>No exception is thrown if the Form frm is not a member of the workbench views collection.</remarks>
    void RemoveGraph(Altaxo.Graph.GUI.GraphController ctrl);


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
  }
}
