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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Altaxo.Main
{
  /// <summary>
  /// Base class for a suspendable document node.
  /// This class supports document nodes that have children, and implements most of the code necessary to handle child events and to suspend child nodes when the parent is suspended.
  /// </summary>
  /// <remarks>If you don't need support for child events, consider using <see cref="T:Altaxo.Main.SuspendableDocumentLeafNode{TEventArgs}"/> instead.</remarks>
  public abstract partial class SuspendableDocumentNode : SuspendableDocumentNodeBase, Main.IDocumentNode
  {
    private class StaticInstanceClass : IDocumentNode
    {
      /// <summary>
      /// Gets or sets the parent object.
      /// </summary>
      public IDocumentNode? ParentObject
      {
        get
        {
          return null;
        }
        set
        {
          throw new InvalidOperationException("This is a static instance of DocumentNode, intended for infrastructural purposes only.");
        }
      }

      /// <summary>
      /// Gets the fixed name of this infrastructure node.
      /// </summary>
      public string Name
      {
        get { return "DocumentNodeStaticInstance"; }
      }

      /// <summary>
      /// Test if this item already has a name.
      /// </summary>
      /// <param name="name">On success, returns the name of the item.</param>
      /// <returns>True if the item already has a name; otherwise false.</returns>
      public virtual bool TryGetName([MaybeNullWhen(false)] out string name)
      {
        name = Name;
        return name is not null;
      }

      /// <inheritdoc/>
      public event EventHandler? Changed;

      /// <inheritdoc/>
      public ISuspendToken SuspendGetToken()
      {
        throw new InvalidOperationException("This is a static instance of DocumentNode, intended for infrastructural purposes only.");
      }

      /// <inheritdoc/>
      public bool IsSuspended
      {
        get
        {
          return false;
        }
      }

      protected void OnChanged()
      {
        Changed?.Invoke(this, EventArgs.Empty);
      }

      /// <inheritdoc/>
      public void EhChildChanged(object child, EventArgs e)
      {
      }

      /// <inheritdoc/>
      public void EhParentTunnelingEventHappened(IDocumentNode sender, IDocumentNode originalSource, TunnelingEventArgs e)
      {
      }

      /// <inheritdoc/>
      public IDocumentLeafNode? GetChildObjectNamed(string name)
      {
        return null;
      }

      /// <inheritdoc/>
      public string GetNameOfChildObject(IDocumentLeafNode o)
      {
        if (o is not null)
          return "Infrastructure object of type " + o.GetType().FullName;
        else
          return "<<null>>";
      }

      /// <inheritdoc/>
      public IEnumerable<IDocumentLeafNode> ChildNodes
      {
        get { yield break; }
      }

      /// <inheritdoc/>
      public IDocumentLeafNode? ParentNode
      {
        get { return null; }
      }

      /// <inheritdoc/>
      public void Dispose()
      {
      }

      /// <inheritdoc/>
      public event Action<object, object, TunnelingEventArgs>? TunneledEvent;

      protected void OnTunneledEvent(object origin, TunnelingEventArgs e)
      {
        TunneledEvent?.Invoke(this, origin, e);
      }

      /// <inheritdoc/>
      public bool IsDisposed
      {
        get { return false; }
      }

      /// <inheritdoc/>
      public bool IsDisposeInProgress
      {
        get { return false; }
      }

      /// <inheritdoc/>
      public void SetDisposeInProgress()
      {
      }
    }

    private static IDocumentNode _staticInstance = new StaticInstanceClass();

    /// <summary>
    /// Gets a single static instance that can be used to give some document nodes a parent, for instance those nodes that are defined as static (Brushes, Pens etc.).
    /// </summary>
    /// <value>
    /// A static instance of <see cref="IDocumentNode"/>.
    /// </value>
    public static IDocumentNode StaticInstance { get { return _staticInstance; } }
  }
}
