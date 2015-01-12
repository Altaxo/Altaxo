﻿#region Copyright

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Main
{
	/// <summary>
	/// Base class for a suspendable document node.
	/// This class supports document nodes that have children, and implements most of the code neccessary to handle child events and to suspend the childs when the parent is suspended.
	/// </summary>
	/// <remarks>If you don't need support for child events, consider using <see cref="SuspendableDocumentLeafNode{TEventArgs}"/> instead.</remarks>
	public abstract partial class SuspendableDocumentNode : SuspendableDocumentNodeBase, Main.IDocumentNode
	{
		private class StaticInstanceClass : IDocumentNode
		{
			public IDocumentNode ParentObject
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

			public string Name
			{
				get { return "DocumentNodeStaticInstance"; }
			}

			public event EventHandler Changed;

			public ISuspendToken SuspendGetToken()
			{
				throw new InvalidOperationException("This is a static instance of DocumentNode, intended for infrastructural purposes only.");
			}

			public void EhChildChanged(object child, EventArgs e)
			{
			}

			public void EhParentTunnelingEventHappened(IDocumentNode sender, IDocumentNode originalSource, TunnelingEventArgs e)
			{
			}

			public IDocumentLeafNode GetChildObjectNamed(string name)
			{
				throw new NotImplementedException();
			}

			public string GetNameOfChildObject(IDocumentLeafNode o)
			{
				throw new NotImplementedException();
			}

			public IEnumerable<IDocumentLeafNode> ChildNodes
			{
				get { throw new NotImplementedException(); }
			}

			public IDocumentLeafNode ParentNode
			{
				get { throw new NotImplementedException(); }
			}

			public void Dispose()
			{
				throw new NotImplementedException();
			}

			public event Action<object, object, TunnelingEventArgs> TunneledEvent;
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