using System;

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
		Altaxo.AltaxoDocument CurrentOpenProject {	get; set; } 
		string CurrentProjectFileName	{	get; } 

    /// <summary>
    /// Opens a Altaxo project. If the current project is dirty, the user is ask for saving the current project.
    /// </summary>
    /// <param name="filename"></param>
    void OpenProject(string filename);

    /// <summary>
    /// This function will delete a data table and close the corresponding views.
    /// </summary>
    /// <param name="table">The data table to delete</param>
    /// <param name="force">If true, the table is deleted without safety question,
    /// if false, the user is ask before the table is deleted.</param>
    void DeleteTable(Altaxo.Data.DataTable table, bool force);

    /// <summary>
    /// This function will delete a graph document and close all corresponding views.
    /// </summary>
    /// <param name="graph">The graph document to delete.</param>
    /// <param name="force">If true, the graph document is deleted without safety question,
    /// if false, the user is ask before the graph document is deleted.</param>
    void DeleteGraphDocument(Altaxo.Graph.GraphDocument graph, bool force);

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


		Altaxo.Worksheet.GUI.IWorksheetController CreateNewWorksheet();
		Altaxo.Worksheet.GUI.IWorksheetController CreateNewWorksheet(Altaxo.Data.DataTable table);
		object OpenOrCreateWorksheetForTable(Altaxo.Data.DataTable table);
		void RemoveWorksheet(Altaxo.Worksheet.GUI.WorksheetController ctrl);

		Altaxo.Graph.GUI.IGraphController CreateNewGraph();
		Altaxo.Graph.GUI.IGraphController CreateNewGraph(Altaxo.Graph.GraphDocument graph);
		object OpenOrCreateViewForGraph(Altaxo.Graph.GraphDocument graph);
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
    /// Event fired <b>after</b> any of the following other events is fired: <see>ProjectOpened</see>, 
    /// <see>ProjectClosed</see>, <see>ProjectRenamed</see>, and <see>ProjectDirtyChanged</see>.
    /// </summary>
    event ProjectEventHandler ProjectChanged;
	}
}
