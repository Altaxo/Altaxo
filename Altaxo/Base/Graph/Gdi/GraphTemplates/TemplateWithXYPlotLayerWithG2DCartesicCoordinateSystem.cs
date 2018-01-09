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

using Altaxo.Collections;
using Altaxo.Gui.Graph.Gdi;
using Altaxo.Main.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Gdi.GraphTemplates
{
	public static class TemplateWithXYPlotLayerWithG2DCartesicCoordinateSystem
	{
		public static readonly Main.Properties.PropertyKey<GraphDocument> PropertyKeyDefaultTemplate =
new Main.Properties.PropertyKey<GraphDocument>(
"A5C78616-F722-4C52-8687-977EC56616AF",
"Graph\\Templates\\LineScatterPlot (Cartesic)",
Main.Properties.PropertyLevel.AllUpToFolder,
typeof(object),
() => CreateBuiltinGraph(null)
)
{
	EditingControllerCreation = (doc) =>
	{
		var ctrl = new DefaultLineScatterGraphDocumentController { UseDocumentCopy = Gui.UseDocument.Copy };
		ctrl.InitializeDocument(doc);
		return ctrl;
	}
};

		/// <summary>
		/// Creates a brand new graph which has an x-y plot layer. The graph is not named, nor is it already part of the project.
		/// </summary>
		/// <param name="propertyContext">The property context. Can be retrieved for instance from the table the plot is initiated or the folder.</param>
		/// <returns>The created graph.</returns>
		private static GraphDocument CreateBuiltinGraph(IReadOnlyPropertyBag propertyContext)
		{
			if (null == propertyContext)
				propertyContext = PropertyExtensions.GetPropertyContextOfProject();

			var graph = new GraphDocument();
			TemplateBase.AddStandardPropertiesToGraph(graph, propertyContext);

			// apply the default location from the property in the path
			graph.RootLayer.Location.CopyFrom(propertyContext.GetValue(Altaxo.Graph.Gdi.GraphDocument.PropertyKeyDefaultRootLayerSize));
			Altaxo.Graph.Gdi.XYPlotLayer layer = new Altaxo.Graph.Gdi.XYPlotLayer(graph.RootLayer);
			layer.CreateDefaultAxes(propertyContext);
			layer.AxisStyles[CSLineID.X0].AxisLineStyle.FirstUpMajorTicks = false;
			layer.AxisStyles[CSLineID.X0].AxisLineStyle.FirstUpMinorTicks = false;
			layer.AxisStyles[CSLineID.Y0].AxisLineStyle.FirstUpMajorTicks = false;
			layer.AxisStyles[CSLineID.Y0].AxisLineStyle.FirstUpMinorTicks = false;
			graph.RootLayer.Layers.Add(layer);

			return graph;
		}

		/// <summary>
		/// Creates a new graph, which has an x-y plot layer. The name of the graph will be prepared, so that it is ready to be included in the project. However, it is not already included in the project.
		/// </summary>
		/// <param name="propertyContext">The property context. Can be retrieved for instance from the table the plot is initiated from or the folder.</param>
		/// <param name="preferredGraphName">The base graph name. If this name exist already, a new name is created, which is based on this argument.</param>
		/// <param name="anyNameInSameFolder">Any name of an item in the same folder. This name is used to determine the destination folder of the graph.</param>
		/// <param name="includeInProject">If true, the graph is included in the project.</param>
		/// <returns>The created graph. The graph is already part of the project. (But no view is created for the graph).</returns>
		public static GraphDocument CreateGraph(IReadOnlyPropertyBag propertyContext, string preferredGraphName, string anyNameInSameFolder, bool includeInProject)
		{
			if (null == propertyContext)
				propertyContext = PropertyExtensions.GetPropertyContextOfProject();

			GraphDocument graph;
			var graphTemplate = propertyContext.GetValue<GraphDocument>(PropertyKeyDefaultTemplate);
			var isBuiltinTemplate = object.ReferenceEquals(graphTemplate, Current.PropertyService.BuiltinSettings.GetValue<GraphDocument>(PropertyKeyDefaultTemplate));
			if (null != graphTemplate && !isBuiltinTemplate)
				graph = (GraphDocument)graphTemplate.Clone();
			else
				graph = CreateBuiltinGraph(propertyContext);

			if (string.IsNullOrEmpty(preferredGraphName))
			{
				string newnamebase = Altaxo.Main.ProjectFolder.CreateFullName(anyNameInSameFolder, Current.Project.GraphDocumentCollection.ItemBaseName);
				graph.Name = Current.Project.GraphDocumentCollection.FindNewItemName(newnamebase);
			}
			else
			{
				graph.Name = preferredGraphName;
			}

			if (includeInProject)
				Current.Project.GraphDocumentCollection.Add(graph);

			return graph;
		}

		public static bool IsGraphTemplateSuitable(GraphDocument graphTemplate, out string problemDescription)
		{
			// Make sure that the graph contains an XYPlotLayer

			if (0 == graphTemplate.RootLayer.Layers.Count)
			{
				problemDescription = "The graph does not contain any layers besides the root layer.";
				return false;
			}

			var xylayer = (XYPlotLayer)TreeNodeExtensions.AnyBetweenHereAndLeaves<HostLayer>(graphTemplate.RootLayer, x => x is XYPlotLayer);

			if (null == xylayer)
			{
				problemDescription = "The graph does not contain any x-y plot layer.";
				return false;
			}

			if (!(xylayer.CoordinateSystem is Altaxo.Graph.Gdi.CS.G2DCartesicCoordinateSystem))
			{
				problemDescription = "The first x-y plot layer found does not contain a cartesic coordinate system.";
				return false;
			}

			problemDescription = null;
			return true;
		}
	}
}