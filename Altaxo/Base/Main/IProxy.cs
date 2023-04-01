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

#nullable enable
using System;

namespace Altaxo.Main
{
  /// <summary>
  /// Interface for all classes which are not proxies themselfs, but which contain references to other document nodes by using <see cref="IProxy"/>s.
  /// </summary>
  public interface IHasDocumentReferences
  {
    /// <summary>
    /// Visits the document references of this instance. All proxies that
    /// this instance contain should be reported by the <paramref name="ReportProxies"/> function.
    /// This function is responsible for processing of the proxies, for instance to relocated the path.
    /// </summary>
    /// <param name="ReportProxies">The function which is used to report the <see cref="DocNodeProxy"/> instances that are contained in this class.</param>
    void VisitDocumentReferences(Main.DocNodeProxyReporter ReportProxies);
  }

  /// <summary>
  /// Holds a reference to an object. If the object is part of the document, i.e. a document node (implements <see cref="IDocumentLeafNode" />),
  /// then only a weak reference is held to this node, and special measures are used to track the node by its path.
  /// The path to the node is stored, and if a new document node with that path exists, the reference to the object is restored.
  /// <see cref="IProxy"/> can also hold non-document objects. To those objects a strong reference is established.
  /// The property <see cref="DocumentPath"/> then returns an empty path, and <see cref="M:ReplacePathParts"/> does nothing at all.
  /// </summary>
  public interface IProxy : ICloneable
  {
    /// <summary>
    /// True when both the document is null and there is also no stored path to the document.
    /// </summary>
    bool IsEmpty { get; }

    /// <summary>
    /// Returns the document node. If the stored doc node is null, it is tried to resolve the stored document path.
    /// If that fails too, null is returned.
    /// </summary>
    object? DocumentObject();

    /// <summary>
    /// Gets the path to the document, if the <see cref="DocumentObject"/> is or was part of the document.
    /// For non-document objects, an empty path is returned.
    /// </summary>
    /// <value>
    /// The document path.
    /// </value>
    Main.AbsoluteDocumentPath DocumentPath();

    /// <summary>
    /// This functionality is provided only for document nodes. For non-document nodes, this function does nothing.
    /// Replaces parts of the path of the document node by another part. If the replacement was successful, the original document node is cleared.
    /// See <see cref="M:DocumentPath.ReplacePathParts"/> for details of the part replacement.
    /// </summary>
    /// <param name="partToReplace">Part of the path that should be replaced. This part has to match the beginning of this part. The last item of the part
    /// is allowed to be given only partially.</param>
    /// <param name="newPart">The new part to replace that piece of the path, that match the <c>partToReplace</c>.</param>
    /// <param name="rootNode">Any document node in the hierarchy that is used to find the root node of the hierarchy.</param>
    /// <returns>True if the path could be replaced. Returns false if the path does not fulfill the presumptions given above.</returns>
    /// <remarks>
    /// As stated above, the last item of the partToReplace can be given only partially. As an example, the path (here separated by space)
    /// <para>Tables Preexperiment1/WDaten Time</para>
    /// <para>should be replaced by </para>
    /// <para>Tables Preexperiment2\WDaten Time</para>
    /// <para>To make this replacement, the partToReplace should be given by</para>
    /// <para>Tables Preexperiment1/</para>
    /// <para>and the newPart should be given by</para>
    /// <para>Tables Preexperiment2\</para>
    /// <para>Note that Preexperiment1\ and Preexperiment2\ are only partially defined items of the path.</para>
    /// </remarks>
    bool ReplacePathParts(AbsoluteDocumentPath partToReplace, AbsoluteDocumentPath newPart, IDocumentLeafNode rootNode);
  }
}
