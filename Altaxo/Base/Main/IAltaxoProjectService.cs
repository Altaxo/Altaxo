using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Main
{
  public interface IAltaxoProjectService : IProjectService
  {
    AltaxoDocument CurrentOpenProject { get; }

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
    void DeleteGraphDocument(Graph.Gdi.GraphDocument graph, bool force);

    /// <summary>
    /// Creates a project item, and adds it to the appropriate collection in the current project.
    /// Note that there might exist more specialized function to create a certain project item.
    /// </summary>
    /// <typeparam name="T">The type of project item to create.</typeparam>
    /// <param name="inFolder">The folder into which the project item is created.</param>
    /// <returns>The created project item.</returns>
    T CreateDocument<T>(string inFolder) where T : IProjectItem;

    /// <summary>
    /// This function will delete a project document and close all corresponding views.
    /// </summary>
    /// <param name="document">The document (project item) to delete.</param>
    /// <param name="force">If true, the document is deleted without safety question; otherwise, the user is ask before the graph document is deleted.</param>
    void DeleteDocument(IProjectItem document, bool force);

    /// <summary>
    /// Creates a new table and the view content for the newly created table.
    /// </summary>
    /// <returns>The content controller for that table.</returns>
    Altaxo.Gui.Worksheet.Viewing.IWorksheetController CreateNewWorksheet();

    /// <summary>
    /// Creates a new table and the view content for the newly created table.
    /// </summary>
    /// <param name="folder">The folder where to create the worksheet. Set null for the root folder.</param>
    /// <returns>The content controller for that table.</returns>
    Altaxo.Gui.Worksheet.Viewing.IWorksheetController CreateNewWorksheetInFolder(string folder);

    /// <summary>
    /// Creates a view content for a table.
    /// </summary>
    /// <param name="table">The table which should be viewed.</param>
    /// <returns>The view content for the provided table.</returns>
    Altaxo.Gui.Worksheet.Viewing.IWorksheetController CreateNewWorksheet(Altaxo.Data.DataTable table);

    /// <summary>
    /// Creates a view content for a table.
    /// </summary>
    /// <param name="table">The table which should be viewed.</param>
    /// <param name="layout">The layout for the table.</param>
    /// <returns>The view content for the provided table.</returns>
    Altaxo.Gui.Worksheet.Viewing.IWorksheetController CreateNewWorksheet(Altaxo.Data.DataTable table, Altaxo.Worksheet.WorksheetLayout layout);

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
    void RemoveWorksheet(Altaxo.Gui.Worksheet.Viewing.IWorksheetController ctrl);

    /// <summary>
    /// Creates a new graph document and the view for this newly created graph document.
    /// </summary>
    /// <returns>The view content for the newly created graph.</returns>
    Altaxo.Gui.Graph.Gdi.Viewing.IGraphController CreateNewGraph();

    /// <summary>
    /// Creates a new graph document in a specified folder and the view for this newly created graph document.
    /// </summary>
    /// <param name="folderName">The folder where to create the new graph.</param>
    /// <returns>The view content for the newly created graph.</returns>
    Altaxo.Gui.Graph.Gdi.Viewing.IGraphController CreateNewGraphInFolder(string folderName);

    /// <summary>
    /// Creates a new graph document and the view for this newly created graph document.
    /// </summary>
    /// <param name="preferredName">The preferred name the new graph document should have.</param>
    /// <returns>The view content for the newly created graph.</returns>
    Altaxo.Gui.Graph.Gdi.Viewing.IGraphController CreateNewGraph(string preferredName);

    /// <summary>
    /// Creates a new view content for a graph document.
    /// </summary>
    /// <param name="graph">The graph document.</param>
    /// <returns>The view content for the provided graph document.</returns>
    Altaxo.Gui.Graph.Gdi.Viewing.IGraphController CreateNewGraph(Graph.Gdi.GraphDocument graph);

    /// <summary>
    /// Creates a new view content for a graph document.
    /// </summary>
    /// <param name="graph">The graph document.</param>
    /// <returns>The view content for the provided graph document.</returns>
    Altaxo.Gui.Graph.Graph3D.Viewing.IGraphController CreateNewGraph3D(Graph.Graph3D.GraphDocument graph);

    /// <summary>
    /// Opens a view that shows the graph <code>graph</code>. If no view for the graph can be found,
    /// a new default view is created.
    /// </summary>
    /// <param name="graph">The graph for which a view must be found.</param>
    /// <returns>The view content for the provided graph.</returns>
    object OpenOrCreateGraphForGraphDocument(Graph.Gdi.GraphDocument graph);

    /// <summary>
    /// Opens a view that shows the document <paramref name="document"/>. If no view for the document can be found,
    /// a new default view is created.
    /// </summary>
    /// <param name="document">The document for which a view must be found.</param>
    /// <returns>The view content for the provided graph.</returns>
    object OpenOrCreateViewContentForDocument(IProjectItem document);

    /// <summary>This will remove the Graph <paramref>ctrl</paramref> from the corresponding forms collection.</summary>
    /// <param name="ctrl">The Graph to remove.</param>
    /// <remarks>No exception is thrown if the Form frm is not a member of the workbench views collection.</remarks>
    void RemoveGraph(Altaxo.Gui.Graph.Gdi.Viewing.IGraphController ctrl);

    /// <summary>
    /// Gets an exporter that can be used to export an image of the provided project item.
    /// </summary>
    /// <param name="item">The item to export, for instance an item of type <see cref="Altaxo.Graph.Gdi.GraphDocument"/> or <see cref="Altaxo.Graph.Graph3D.GraphDocument"/>.</param>
    /// <returns>The image exporter class that can be used to export the item in graphical form, or null if no exporter could be found.</returns>
    IProjectItemImageExporter GetProjectItemImageExporter(IProjectItem item);
  }
}
