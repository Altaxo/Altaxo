#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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

namespace Altaxo.Main
{
  /// <summary>
  /// Interface to an Altaxo project. A project here is the topmost element of documents and contains all documents.
  /// </summary>
  public interface IProject : IDocumentNode, ICanBeDirty
  {
    public new bool IsDirty { get; set; }

    /// <summary>
    /// Clears the <see cref="IsDirty"/> flag in a more advanced manner,
    /// supporting the needs for late loading of data etc.
    /// It should update the data needed for deferred data loading before clearing the flag.
    /// </summary>
    /// <param name="archiveManager">The archive manager that currently manages the archive in which the project is stored.</param>
    /// <param name="entryNameToItemDictionary">A dictionary where the keys are the archive entry names that where used to store the project items that are the values. The dictionary contains only those project items that need further handling (e.g. late load handling).</param>
    public void ClearIsDirty(Services.IProjectArchiveManager archiveManager, IDictionary<string, IProjectItem>? entryNameToItemDictionary);

    /// <summary>
    /// Gets the types of project items currently supported in the project.
    /// </summary>
    /// <value>
    /// The project item types.
    /// </value>
    public IEnumerable<System.Type> ProjectItemTypes
    {
      get;
    }

    /// <summary>
    /// Gets the collection for a certain project item type.
    /// </summary>
    /// <param name="type">The type (must be a type that implements <see cref="Altaxo.Main.IProjectItem"/>).</param>
    /// <returns>The collection in which items of this type are stored.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public IProjectItemCollection GetCollectionForProjectItemType(System.Type type);

    /// <summary>
    /// Enumerates the collections of all project items.
    /// </summary>
    /// <value>
    /// The collections of all project items.
    /// </value>
    public IEnumerable<IProjectItemCollection> ProjectItemCollections { get; }


    /// <summary>
    /// Gets the root path for a given project item type.
    /// </summary>
    /// <param name="type">The type of project item.</param>
    /// <returns>The root path of this type of item.</returns>
    public AbsoluteDocumentPath GetRootPathForProjectItemType(System.Type type);

    /// <summary>
    /// Gets the document path for project item, using its type and name. It is not neccessary that the item is part of the project yet.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The document part for the project item, deduces from its type and its name.</returns>
    /// <exception cref="System.ArgumentNullException">item</exception>
    public AbsoluteDocumentPath GetDocumentPathForProjectItem(IProjectItem item)
    {
      if (item is null)
        throw new ArgumentNullException(nameof(item));

      return GetRootPathForProjectItemType(item.GetType()).Append(item.Name);
    }

    /// <summary>
    /// Tests if the project item given in the argument is already contained in this document.
    /// </summary>
    /// <param name="item">The item to test.</param>
    /// <returns>True if the item is already contained in the document, otherwise false.</returns>
    /// <exception cref="System.ArgumentNullException">item</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">The type of item is not yet considered here.</exception>
    public bool ContainsItem(IProjectItem item)
    {
      ArgumentNullException.ThrowIfNull(item);

      var coll = GetCollectionForProjectItemType(item.GetType());

      return coll.TryGetValue(item.Name, out var foundItem) && object.ReferenceEquals(foundItem, item);
    }

    /// <summary>
    /// Tries to get an existing project item with the same type and name as the provided item.
    /// </summary>
    /// <param name="item">The item to test for.</param>
    /// <param name="existingItem">If an item with the same type and name as the provided item exists in the project, that existing item is returned.</param>
    /// <returns>True if an item with the same type and name as the provided item exists in the project; otherwise, false.</returns>
    /// <exception cref="System.ArgumentNullException">item</exception>
    /// <exception cref="System.ArgumentOutOfRangeException"></exception>
    public bool TryGetExistingItemWithSameTypeAndName(IProjectItem item, [MaybeNullWhen(false)] out IProjectItem existingItem);

    /// <summary>
    /// Adds the provided project item to the Altaxo project, for instance a table or a graph, to the project. For <see cref="T:Altaxo.Main.Properties.ProjectFolderPropertyDocument"/>s,
    /// if a document with the same name is already present, the properties are merged.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <exception cref="System.ArgumentNullException">item</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">The type of item is not yet considered here.</exception>
    public void AddItem(IProjectItem item);

    /// <summary>
    /// Adds the provided project item to the Altaxo project, for instance a table or a graph, to the project. If another project item with the same name already exists,
    /// a new unique name for the item is found, based on the given name.
    /// For <see cref="T:Altaxo.Main.Properties.ProjectFolderPropertyDocument"/>s, if a document with the same name is already present, the properties are merged.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <exception cref="System.ArgumentNullException">item</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">The type of item is not yet considered here.</exception>
    public void AddItemWithThisOrModifiedName(IProjectItem item);

    /// <summary>
    /// Removes the provided project item to the Altaxo project, for instance a table or a graph, to the project.
    /// </summary>
    /// <param name="item">The item to remove.</param>
    /// <exception cref="System.ArgumentNullException">item</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">The type of item is not yet considered here.</exception>
    public bool RemoveItem(IProjectItem item);

    /// <summary>
    /// The properties associated with the project folders. Please note that the properties of the project are also stored inside this collection, with the name being an empty string (root folder node).
    /// </summary>
    public Altaxo.Main.Properties.ProjectFolderPropertyDocumentCollection ProjectFolderProperties { get; }

    /// <summary>
    /// Get information about the folders in this project.
    /// </summary>
    public Altaxo.Main.ProjectFolders Folders { get; }

  }
}
