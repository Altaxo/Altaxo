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

using Altaxo.AddInItems;
using System;
using System.Collections;

namespace Altaxo.Main
{
  /// <summary>
  /// <see cref="GraphExportBindingDoozer"/> generates a class with interface <see cref="Altaxo.Main.IProjectItemImageExporter"/>, which can be used
  /// to export a project item derived from type <see cref="Altaxo.Graph.GraphDocumentBase"/> as an image to a stream.
  ///
  /// </summary>
  public class GraphExportBindingDoozer : IDoozer
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
      string docTypeString = args.Codon.Properties["ProjectItemClass"];
      string exporterTypeString = args.Codon.Properties["ExporterClass"];

      var docType = Type.GetType(docTypeString, false, false);
      var exporterType = Type.GetType(exporterTypeString, false, false);

      if (null == docType || null == exporterType)
        return null;

      if (!(typeof(IProjectItem).IsAssignableFrom(docType)))
        return null;
      if (!(typeof(Altaxo.Main.IProjectItemImageExporter).IsAssignableFrom(exporterType)))
        return null;

      return new Graph3DExportBindingDescriptor(args.Codon, docType, exporterType);
    }

    private class Graph3DExportBindingDescriptor : IProjectItemExportBindingDescriptor
    {
      private Type _projectItemType;
      private Type _graphicalExporterType;
      public Type ProjectItemType { get { return _projectItemType; } }
      public Type GraphicalExporterType { get { return _graphicalExporterType; } }

      private Codon _codon;

      public string Id { get; set; }
      public string Title { get; set; }

      public Graph3DExportBindingDescriptor(Codon codon, Type projectItemType, Type graphicalExporterType)
      {
        if (codon == null)
          throw new ArgumentNullException(nameof(codon));

        if (!Altaxo.Main.Services.ReflectionService.IsSubClassOfOrImplements(graphicalExporterType, typeof(Altaxo.Main.IProjectItemImageExporter)))
          throw new Exception(string.Format("Error in codon {0}: the provided type in argument {1} is not of type {2}", codon, nameof(graphicalExporterType), typeof(Altaxo.Main.IProjectItemImageExporter)));

        this._codon = codon;
        this.Id = codon.Id;

        string title = codon.Properties["title"];
        if (string.IsNullOrEmpty(title))
          this.Title = codon.Id;
        else
          this.Title = title;

        _projectItemType = projectItemType;
        _graphicalExporterType = graphicalExporterType;
      }

      public override string ToString()
      {
        return string.Format("[GraphExportBindingDescriptor ProjectItemClass={0} ExporterClass={1}]", _projectItemType, _graphicalExporterType);
      }
    }
  }
}
