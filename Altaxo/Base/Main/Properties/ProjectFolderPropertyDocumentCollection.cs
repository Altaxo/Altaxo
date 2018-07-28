#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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
using System.Collections.Generic;

namespace Altaxo.Main.Properties
{
  public class ProjectFolderPropertyDocumentCollection :
    ProjectItemCollectionBase<ProjectFolderPropertyDocument>
  {
    public ProjectFolderPropertyDocumentCollection(AltaxoDocument parent)
      : base(parent)
    {
    }

    public override Main.IDocumentNode ParentObject
    {
      get { return this._parent; }
      set
      {
        if (null != value)
          throw new InvalidOperationException("ParentObject of ProjectFolderPropertyDocumentCollection is fixed and cannot be set");

        base.ParentObject = value; // allow setting Parent to null (required for dispose)
      }
    }

    public override string ItemBaseName { get { return ""; } }

    /// <summary>
    /// Ensures the existence of a certain <see cref="ProjectFolderPropertyDocument"/>. If the document already exists,
    /// the existing <see cref="ProjectFolderPropertyDocument"/> is returned; otherwise a new <see cref="ProjectFolderPropertyDocument"/> is
    /// created, added to the collection, and returned.
    /// </summary>
    /// <param name="itemName">Name of the <see cref="ProjectFolderPropertyDocument"/>.
    /// An exception is thrown if the name is not a folder name (it has to end with a backslash).
    /// </param>
    /// <returns>If the document with the given name already exists,
    /// the existing <see cref="ProjectFolderPropertyDocument"/> is returned; otherwise a new <see cref="ProjectFolderPropertyDocument"/> is
    /// created, added to the collection, and returned.</returns>
    public ProjectFolderPropertyDocument EnsureExistence(string itemName)
    {
      if (_itemsByName.TryGetValue(itemName, out var doc))
      {
        return doc;
      }
      else
      {
        doc = new ProjectFolderPropertyDocument(itemName);
        Add(doc);
        return doc;
      }
    }

    /// <summary>
    /// Gets the parent ProjectFolderPropertyBagCollection of a child graph.
    /// </summary>
    /// <param name="child">A graph for which the parent collection is searched.</param>
    /// <returns>The parent ProjectFolderPropertyBagCollection, if it exists, or null otherwise.</returns>
    public static ProjectFolderPropertyDocumentCollection GetParentProjectFolderPropertyBagCollectionOf(Main.IDocumentLeafNode child)
    {
      return (ProjectFolderPropertyDocumentCollection)Main.AbsoluteDocumentPath.GetRootNodeImplementing(child, typeof(ProjectFolderPropertyDocumentCollection));
    }
  }
}
