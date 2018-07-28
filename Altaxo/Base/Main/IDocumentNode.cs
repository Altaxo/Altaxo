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

using Altaxo.Main;
using Altaxo.Collections;
using System;

namespace Altaxo.Main
{
  /// <summary>
  /// Interface of a document node at the end of the hierarchie, i.e. a leaf node.
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
    /// Retrieves the parent object.
    /// </summary>
    IDocumentNode ParentObject { get; set; }

    void EhParentTunnelingEventHappened(IDocumentNode sender, IDocumentNode originalSource, TunnelingEventArgs e);

    /// <summary>
    /// Gets a value indicating whether this instance is disposed.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is disposed; otherwise, <c>false</c>.
    /// </value>
    bool IsDisposed { get; }

    /// <summary>
    /// Gets a value indicating whether this instance is dispose in progress, or the instance is already disposed.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is dispose in progress or already disposed; otherwise, <c>false</c>.
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

  [Flags]
  public enum DisposeState
  {
    NotDisposed = 0,
    DisposeInProgress = 1,
    Disposed = 2
  }
}
