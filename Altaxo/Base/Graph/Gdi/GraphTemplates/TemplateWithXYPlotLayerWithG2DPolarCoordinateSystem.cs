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
using Altaxo.Main.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Gdi.GraphTemplates
{
	public static class TemplateWithXYPlotLayerWithG2DPolarCoordinateSystem
	{
		public static readonly Main.Properties.PropertyKey<GraphDocument> PropertyKeyDefaultTemplate =
		new Main.Properties.PropertyKey<GraphDocument>(
		"F3BF6A1C-7929-4120-A1A7-2E8A5745E7C4",
		"Graph\\Templates\\PolarPlot",
		Main.Properties.PropertyLevel.AllUpToFolder,
		typeof(object),
		() => CreateBuiltinGraph(null)

		)
		{
			EditingControllerCreation = (doc) =>
			{
				var ctrl = new Gui.Graph.DefaultLineScatterGraphDocumentController { UseDocumentCopy = Gui.UseDocument.Copy };
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
				propertyContext = PropertyExtensions.GetPropertyHierarchyStartingFromUserSettings();

			Altaxo.Graph.Gdi.GraphDocument graph = new Altaxo.Graph.Gdi.GraphDocument();
			TemplateBase.AddStandardPropertiesToGraph(graph, propertyContext);
			graph.RootLayer.Location.CopyFrom(propertyContext.GetValue(Altaxo.Graph.Gdi.GraphDocument.PropertyKeyDefaultRootLayerSize)); 	// apply the default location from the property in the path

			Altaxo.Graph.Gdi.XYPlotLayer layer = new Altaxo.Graph.Gdi.XYPlotLayer(graph.RootLayer, new Altaxo.Graph.Gdi.CS.G2DPolarCoordinateSystem());
			layer.CreateDefaultAxes(propertyContext);
			graph.RootLayer.Layers.Add(layer);

			return graph;
		}

		/// <summary>
		/// Creates a new graph, which has an x-y plot layer with an polar coordinate system. The name of the graph will be prepared, so that it is ready to be included in the project. However, it is not already included in the project.
		/// </summary>
		/// <param name="propertyContext">The property context. Can be retrieved for instance from the table the plot is initiated from or the folder.</param>
		/// <param name="anyNameInSameFolder">Any name of an item in the same folder. This name is used to determine the destination folder of the graph.</param>
		/// <returns>The created graph. The graph is already part of the project. (But no view is created for the graph).</returns>
		public static GraphDocument CreateGraph(IReadOnlyPropertyBag propertyContext, string preferredGraphName, string anyNameInSameFolder, bool includeInProject)
		{
			if (null == propertyContext)
				propertyContext = PropertyExtensions.GetPropertyHierarchyStartingFromUserSettings();

			GraphDocument graph;
			var graphTemplate = propertyContext.GetValue<GraphDocument>(PropertyKeyDefaultTemplate);
			bool isBuiltinPolarPlotTemplate = object.ReferenceEquals(graphTemplate, Current.PropertyService.BuiltinSettings.GetValue<GraphDocument>(PropertyKeyDefaultTemplate));

			GraphDocument graphTemplateCartesic = null;
			bool isLineScatterTemplateBuiltin = false;

			if (isBuiltinPolarPlotTemplate)
			{
				graphTemplateCartesic = propertyContext.GetValue<GraphDocument>(TemplateWithXYPlotLayerWithG2DCartesicCoordinateSystem.PropertyKeyDefaultTemplate);
				isLineScatterTemplateBuiltin = object.ReferenceEquals(graphTemplate, Current.PropertyService.BuiltinSettings.GetValue<GraphDocument>(TemplateWithXYPlotLayerWithG2DCartesicCoordinateSystem.PropertyKeyDefaultTemplate));
			}

			if (!isLineScatterTemplateBuiltin && isBuiltinPolarPlotTemplate)
			{
				graph = (GraphDocument)graphTemplateCartesic.Clone();
				// because we use the template with cartesic coordinate system here, we have to replace it by a polar coordinate systen
				var layer = graph.RootLayer.Layers.OfType<XYPlotLayer>().First();

				layer.CoordinateSystem = new CS.G2DPolarCoordinateSystem();
				layer.AxisStyles.Clear();
				layer.CreateDefaultAxes(graph.GetPropertyContext());
			}
			else if (!isBuiltinPolarPlotTemplate)
			{
				graph = (GraphDocument)graphTemplate.Clone();
			}
			else
			{
				graph = CreateBuiltinGraph(null);
			}

			if (string.IsNullOrEmpty(preferredGraphName))
			{
				string newnamebase = Altaxo.Main.ProjectFolder.CreateFullName(anyNameInSameFolder, "GRAPH");
				graph.Name = Current.Project.GraphDocumentCollection.FindNewName(newnamebase);
			}
			else
			{
				graph.Name = preferredGraphName;
			}

			if (includeInProject)
				Current.Project.GraphDocumentCollection.Add(graph);

			return graph;
		}
	}
}