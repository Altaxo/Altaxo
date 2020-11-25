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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Main
{
  /// <summary>
  /// Interface to an Altaxo project. A project here is the topmost element of documents and contains all documents.
  /// </summary>
  public interface IProject : IDocumentNode, ICanBeDirty
  {
    new bool IsDirty { get; set; }

    /// <summary>
    /// Clears the <see cref="IsDirty"/> flag in a more advanced manner,
    /// supporting the needs for late loading of data etc.
    /// It should update the data needed for deferred data loading before clearing the flag.
    /// </summary>
    /// <param name="archiveManager">The archive manager that currently manages the archive in which the project is stored.</param>
    /// <param name="entryNameToItemDictionary">A dictionary where the keys are the archive entry names that where used to store the project items that are the values. The dictionary contains only those project items that need further handling (e.g. late load handling).</param>
    void ClearIsDirty(Services.IProjectArchiveManager archiveManager, IDictionary<string, IProjectItem>? entryNameToItemDictionary);

    /// <summary>
    /// Gets the types of project items currently supported in the project.
    /// </summary>
    /// <value>
    /// The project item types.
    /// </value>
    IEnumerable<System.Type> ProjectItemTypes
    {
      get;
    }

    /// <summary>
    /// Gets the root path for a given project item type.
    /// </summary>
    /// <param name="type">The type of project item.</param>
    /// <returns>The root path of this type of item.</returns>
    AbsoluteDocumentPath GetRootPathForProjectItemType(System.Type type);
  }
}
