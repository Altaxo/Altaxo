using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Worksheet.Commands
{
	using Data;
	using Graph.Graph3D;
	using Graph.Graph3D.Plot;
	using Graph.Graph3D.Plot.Styles;
	using Graph.Plot.Data;

	/// <summary>
	/// Ensure that the PresentationCore dll is loaded.
	/// </summary>
	internal static class PresentationCoreLoader
	{
		/// <summary>
		/// Ensures that the presentation core DLL loaded.
		/// </summary>
		/// <returns></returns>
		public static string EnsurePresentationCoreLoaded()
		{
			// Problem: here maybe the PresentationCore is not referenced in this assembly, because it was up to now not needed
			// Result: the reflection subsystem will skip this assembly when searching for user controls, because it thinks that we have no dependency
			// on PresentationCore and hence no user controls can exist in this assembly
			// Solution: make sure that the presentation core is referenced, by referencing an arbitrary type in it
			System.Windows.TextAlignment alignment = new System.Windows.TextAlignment();
			string t = alignment.ToString();

			return t;
		}
	}

	public class Plot3D : AbstractWorksheetControllerCommand
	{
		public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
		{
			PresentationCoreLoader.EnsurePresentationCoreLoaded();

			var graph = GraphDocument3DBuilder.CreateNewStandardGraphWithXYZPlotLayer(null);

			var graphController = (Gui.Graph3D.Viewing.Graph3DControllerWpf)Current.Gui.GetControllerAndControl(new object[] { graph }, typeof(Gui.IMVCANController), Gui.UseDocument.Directly);

			if (null == graphController.ViewObject)
				Current.Gui.FindAndAttachControlTo(graphController);

			var viewContent = new Altaxo.Gui.SharpDevelop.SDGraph3DViewContent(graphController);

			if (null != Current.Workbench)
				Current.Workbench.ShowView(viewContent);
		}
	}

	public class PlotSurface3D : AbstractWorksheetControllerCommand
	{
		public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
		{
			PresentationCoreLoader.EnsurePresentationCoreLoaded();

			var graph = GraphDocument3DBuilder.CreateNewStandardGraphWithXYZPlotLayer(null);

			AddDensityImage(ctrl, graph);

			var graphController = (Gui.Graph3D.Viewing.Graph3DControllerWpf)Current.Gui.GetControllerAndControl(new object[] { graph }, typeof(Gui.IMVCANController), Gui.UseDocument.Directly);

			if (null == graphController.ViewObject)
				Current.Gui.FindAndAttachControlTo(graphController);

			var viewContent = new Altaxo.Gui.SharpDevelop.SDGraph3DViewContent(graphController);

			if (null != Current.Workbench)
				Current.Workbench.ShowView(viewContent);
		}

		/// <summary>
		/// Plots a density image of the selected columns.
		/// </summary>
		/// <param name="dg"></param>
		public static void AddDensityImage(Altaxo.Gui.Worksheet.Viewing.IWorksheetController dg, GraphDocument3D graph)
		{
			var xylayer = graph.RootLayer.Layers.OfType<XYPlotLayer3D>().First();
			var context = graph.GetPropertyContext();

			var plotStyle = new DensityImagePlotStyle();

			XYZMeshedColumnPlotData assoc = new XYZMeshedColumnPlotData(dg.DataTable, dg.SelectedDataRows, dg.SelectedDataColumns, dg.SelectedPropertyColumns);
			if (assoc.DataTableMatrix.RowHeaderColumn == null)
				assoc.DataTableMatrix.RowHeaderColumn = new IndexerColumn();
			if (assoc.DataTableMatrix.ColumnHeaderColumn == null)
				assoc.DataTableMatrix.ColumnHeaderColumn = new IndexerColumn();

			var pi = new DensityImagePlotItem(assoc, plotStyle);
			xylayer.PlotItems.Add(pi);
		}
	}
}