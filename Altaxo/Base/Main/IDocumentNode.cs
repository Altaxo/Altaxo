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
using Altaxo.Collections;
using Altaxo.Main;

namespace Altaxo.Main
{
  /// <summary>
  /// Interface of a document node at the end of the hierarchy, i.e. a leaf node.
  /// </summary>
  public interface IDocumentLeafNode
    :
    INamedObject,
    Main.IChangedEventSource,
    ISuspendableByToken,
    ITunnelingEventSource,
    IDisposable,
    // ICloneable,
    ITreeNodeWithParent<IDocumentLeafNode>
  //Altaxo.Collections.INodeWithParentNode<IDocumentNode>
  {
    /// <summary>
    /// Gets or sets the parent document node of this leaf node.
    /// </summary>
    IDocumentNode? ParentObject { get; set; }

    /// <summary>
    /// Handles a tunneling event raised by a parent document node.
    /// </summary>
    /// <param name="sender">The parent node that forwarded the tunneling event.</param>
    /// <param name="originalSource">The original source node where the event was raised.</param>
    /// <param name="e">The event data associated with the tunneling event.</param>
    void EhParentTunnelingEventHappened(IDocumentNode sender, IDocumentNode originalSource, TunnelingEventArgs e);

    /// <summary>
    /// Gets a value indicating whether this instance is disposed.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is disposed; otherwise, <c>false</c>.
    /// </value>
    bool IsDisposed { get; }

    /// <summary>
    /// Gets a value indicating whether this instance is in dispose progress, or the instance is already disposed.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is in dispose progress or already disposed; otherwise, <c>false</c>.
    /// </value>
    bool IsDisposeInProgress { get; }

    /// <summary>
    /// Sets the flag that dispose is in progress for this node and all child nodes recursively.
    /// </summary>
    void SetDisposeInProgress();
  }

  /// <summary>
  /// Provides the document hierarchy by getting the parent node. The document node is required to have a name, thus it also implements <see cref="INamedObject"/>.
  /// </summary>
  public interface IDocumentNode : IDocumentLeafNode, IChildChangedEventSink, INamedObjectCollection
  {
  }

  /// <summary>
  /// Represents the dispose state of a document node.
  /// </summary>
  [Flags]
  public enum DisposeState
  {
    /// <summary>
    /// The node is not disposed.
    /// </summary>
    NotDisposed = 0,

    /// <summary>
    /// The node is currently in the process of being disposed.
    /// </summary>
    DisposeInProgress = 1,

    /// <summary>
    /// The node has been disposed.
    /// </summary>
    Disposed = 2
  }
}
