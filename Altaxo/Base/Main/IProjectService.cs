using System;

namespace Altaxo.Main
{
	/// <summary>
	/// Manages the currently open Altaxo project.
	/// </summary>
	public interface IProjectService
	{
		Altaxo.AltaxoDocument CurrentOpenProject {	get; set; } 
		string CurrentProjectFileName	{	get; } 

		Altaxo.Worksheet.GUI.IWorksheetController CreateNewWorksheet();
		Altaxo.Worksheet.GUI.IWorksheetController CreateNewWorksheet(Altaxo.Data.DataTable table);
		void RemoveWorksheet(Altaxo.Worksheet.GUI.WorksheetController ctrl);

		Altaxo.Graph.GUI.IGraphController CreateNewGraph();
		Altaxo.Graph.GUI.IGraphController CreateNewGraph(Altaxo.Graph.GraphDocument graph);
		void RemoveGraph(Altaxo.Graph.GUI.GraphController ctrl);
	}
}
