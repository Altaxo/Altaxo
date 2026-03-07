#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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
using Altaxo.AddInItems;
using Altaxo.Gui.Graph.Graph3D.Common;

namespace Altaxo.Main
{
  /// <summary>
  /// <see cref="Graph3DExportBindingDoozer"/> generates a class with interface <see cref="Altaxo.Main.IProjectItemImageExporter"/>, which can be used
  /// to export the project item of type <see cref="Altaxo.Graph.Graph3D.GraphDocument"/> as an image to a stream.
  ///
  /// </summary>
  public class Graph3DExportBindingDoozer : IDoozer
  {
    /// <summary>
    /// Gets if the doozer handles codon conditions on its own.
    /// If this property return false, the item is excluded when the condition is not met.
    /// </summary>
    public bool HandleConditions
    {
      get
      {
        return false;
      }
    }

    /// <summary>
    /// Creates an item with the specified sub items. And the current
    /// Condition status for this item.
    /// </summary>
    public object BuildItem(BuildItemArgs args)
    {
      if (DirectXVersionAvailability.IsDirectX12Available)
        return new Graph3DExportBindingDescriptor(args.Codon, typeof(Altaxo.Graph.Graph3D.GraphDocument), typeof(Altaxo.Gui.Graph.Graph3D.Common.D3D12BitmapExporter));
      else
        return new Graph3DExportBindingDescriptor(args.Codon, typeof(Altaxo.Graph.Graph3D.GraphDocument), typeof(Altaxo.Gui.Graph.Graph3D.Common.D3D11BitmapExporter));
    }

    /// <summary>
    /// Descriptor for graph-export bindings.
    /// </summary>
    private class Graph3DExportBindingDescriptor : IProjectItemExportBindingDescriptor
    {
      /// <summary>
      /// Bound project-item type.
      /// </summary>
      private Type _projectItemType;
      /// <summary>
      /// Exporter type used for image export.
      /// </summary>
      private Type _graphicalExporterType;
      /// <summary>
      /// Gets the bound project-item type.
      /// </summary>
      public Type ProjectItemType { get { return _projectItemType; } }
      /// <summary>
      /// Gets the exporter type.
      /// </summary>
      public Type GraphicalExporterType { get { return _graphicalExporterType; } }

      /// <summary>
      /// Backing codon.
      /// </summary>
      private Codon _codon;

      /// <summary>
      /// Gets or sets the binding identifier.
      /// </summary>
      public string Id { get; set; }
      /// <summary>
      /// Gets or sets the display title.
      /// </summary>
      public string Title { get; set; }

      /// <summary>
      /// Initializes a new instance of the <see cref="Graph3DExportBindingDescriptor"/> class.
      /// </summary>
      public Graph3DExportBindingDescriptor(Codon codon, Type projectItemType, Type graphicalExporterType)
      {
        if (codon is null)
          throw new ArgumentNullException(nameof(codon));

        if (!Altaxo.Main.Services.ReflectionService.IsSubClassOfOrImplements(graphicalExporterType, typeof(Altaxo.Main.IProjectItemImageExporter)))
          throw new Exception(string.Format("Error in codon {0}: the provided type in argument {1} is not of type {2}", codon, nameof(graphicalExporterType), typeof(Altaxo.Main.IProjectItemImageExporter)));

        _codon = codon;
        Id = codon.Id;

        string title = codon.Properties["title"];
        if (string.IsNullOrEmpty(title))
          Title = codon.Id;
        else
          Title = title;

        _projectItemType = projectItemType;
        _graphicalExporterType = graphicalExporterType;
      }

      /// <summary>
      /// Returns a diagnostic string for this descriptor.
      /// </summary>
      public override string ToString()
      {
        return string.Format("[Graph3DExportBindingDescriptor ItemClass={0} ExporterClass={1}]", _projectItemType, _graphicalExporterType);
      }
    }
  }
}
