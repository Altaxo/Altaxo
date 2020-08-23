#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Altaxo.Collections;
using Altaxo.Geometry;
using Altaxo.Main.Properties;

namespace Altaxo.Graph.Graph3D.Templates
{
  public static class TemplateWithXYZPlotLayerWithG3DCartesicCoordinateSystem
  {
    public static readonly Main.Properties.PropertyKey<GraphDocument> PropertyKeyDefaultTemplate =
new Main.Properties.PropertyKey<GraphDocument>(
"76AEA5F9-B24D-4CBB-91D9-1AB7D3FAB973",
"Graph3D\\Templates\\Plot (Cartesic)",
Main.Properties.PropertyLevel.AllUpToFolder,
typeof(object),
() => CreateBuiltinGraph(null, null)
)
{
  EditingControllerCreation = (doc) =>
  {
  var ctrl = new Gui.Graph.Graph3D.Templates.DefaultCartesicPlotTemplateController { UseDocumentCopy = Gui.UseDocument.Copy };
  ctrl.InitializeDocument(doc);
  return ctrl;
}
};

    /// <summary>
    /// Creates a brand new graph which has an x-y plot layer. The graph is not named, nor is it already part of the project.
    /// </summary>
    /// <param name="propertyContext">The property context. Can be retrieved for instance from the table the plot is initiated or the folder.</param>
    /// <param name="folderName">Name of the folder.</param>
    /// <returns>The created graph.</returns>
    private static GraphDocument CreateBuiltinGraph(IReadOnlyPropertyBag? propertyContext, string? folderName)
    {
      if (propertyContext is null)
      {
        if (null != folderName)
          propertyContext = Altaxo.PropertyExtensions.GetPropertyContextOfProjectFolder(folderName);
        else
          propertyContext = PropertyExtensions.GetPropertyContextOfProject();
      }

      var graph = new GraphDocument();
      TemplateBase.AddStandardPropertiesToGraph(graph, propertyContext);

      // apply the default location from the property in the path
      graph.RootLayer.Location.CopyFrom(propertyContext.GetValue(GraphDocument.PropertyKeyDefaultRootLayerSize));
      var layer = new XYZPlotLayer(graph.RootLayer, new CS.G3DCartesicCoordinateSystem());
      graph.RootLayer.Layers.Add(layer);
      layer.CreateDefaultAxes(propertyContext);
      graph.ViewToRootLayerCenter(new VectorD3D(-1, -2, 1), new VectorD3D(0, 0, 1), 1);

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
    public static GraphDocument CreateGraph(IReadOnlyPropertyBag propertyContext, string? preferredGraphName, string? anyNameInSameFolder, bool includeInProject)
    {
      var folderName = null == anyNameInSameFolder ? null : Altaxo.Main.ProjectFolder.GetFolderPart(anyNameInSameFolder);

      if (propertyContext is null)
      {
        if (folderName is not null)
          propertyContext = Altaxo.PropertyExtensions.GetPropertyContextOfProjectFolder(folderName);
        else
          propertyContext = PropertyExtensions.GetPropertyContextOfProject();
      }

      GraphDocument graph;
      var graphTemplate = propertyContext.GetValue<GraphDocument>(PropertyKeyDefaultTemplate);
      var isBuiltinTemplate = object.ReferenceEquals(graphTemplate, Current.PropertyService.BuiltinSettings.GetValue<GraphDocument>(PropertyKeyDefaultTemplate));
      if (null != graphTemplate && !isBuiltinTemplate)
        graph = (GraphDocument)graphTemplate.Clone();
      else
        graph = CreateBuiltinGraph(propertyContext, folderName);

      if (string.IsNullOrEmpty(preferredGraphName))
      {
        string newnamebase = Altaxo.Main.ProjectFolder.CreateFullName(anyNameInSameFolder ?? string.Empty, "GRAPH");
        graph.Name = Current.Project.GraphDocumentCollection.FindNewItemName(newnamebase);
      }
      else
      {
        graph.Name = preferredGraphName;
      }

      if (includeInProject)
        Current.Project.AddItem(graph);

      return graph;
    }

    public static bool IsGraphTemplateSuitable(GraphDocument graphTemplate, [MaybeNullWhen(true)] out string problemDescription)
    {
      // Make sure that the graph contains an XYPlotLayer

      if (0 == graphTemplate.RootLayer.Layers.Count)
      {
        problemDescription = "The graph does not contain any layers besides the root layer.";
        return false;
      }

      var xyzlayer = (XYZPlotLayer?)TreeNodeExtensions.AnyBetweenHereAndLeaves<HostLayer>(graphTemplate.RootLayer, x => x is XYZPlotLayer);

      if (xyzlayer is null)
      {
        problemDescription = "The graph does not contain any x-y-z plot layer.";
        return false;
      }

      if (!(xyzlayer.CoordinateSystem is CS.G3DCartesicCoordinateSystem))
      {
        problemDescription = "The first x-y-z plot layer found does not contain a cartesic coordinate system.";
        return false;
      }

      problemDescription = null;
      return true;
    }
  }
}
