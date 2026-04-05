#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using System.Linq;
using Altaxo.Main;

namespace Altaxo.Graph.Graph3D
{
  /// <summary>
  /// Represents a collection of three-dimensional graph documents.
  /// </summary>
  public class GraphDocumentCollection :
    ProjectItemCollectionBase<GraphDocument, IProjectItem>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="GraphDocumentCollection"/> class.
    /// </summary>
    /// <param name="parent">The parent document.</param>
    /// <param name="commonDictionaryForGraphs">The shared dictionary used to store graphs by name.</param>
    public GraphDocumentCollection(AltaxoDocument parent, SortedDictionary<string, IProjectItem> commonDictionaryForGraphs)
      : base(parent)
    {
      _itemsByName = commonDictionaryForGraphs ?? throw new ArgumentNullException(nameof(commonDictionaryForGraphs));
    }

    /// <inheritdoc/>
    public override Main.IDocumentNode? ParentObject
    {
      get
      {
        return _parent;
      }
      set
      {
        if (value is not null)
          throw new InvalidOperationException("ParentObject of GraphDocumentCollection is fixed and cannot be set");
        base.ParentObject = value; // allow setting to null
      }
    }

    /// <inheritdoc/>
    public override string ItemBaseName { get { return "GRAPH"; } }

    /// <summary>
    /// Gets the parent GraphDocumentCollection of a child graph.
    /// </summary>
    /// <param name="child">A graph for which the parent collection is searched.</param>
    /// <returns>The parent GraphDocumentCollection, if it exists, or null otherwise.</returns>
    public static GraphDocumentCollection? GetParentGraphDocumentCollectionOf(Main.IDocumentLeafNode child)
    {
      return (GraphDocumentCollection?)Main.AbsoluteDocumentPath.GetRootNodeImplementing(child, typeof(GraphDocumentCollection));
    }
  }
}
