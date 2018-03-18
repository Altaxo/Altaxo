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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Main
{
	/// <summary>
	/// Interface that all collections that store Altaxo project items must support.
	/// </summary>
	public interface IProjectItemCollection :
		IParentOfINameOwnerChildNodes,
		Altaxo.Main.INamedObjectCollection,
		Altaxo.Main.IDocumentNode
	{
		/// <summary>
		/// Determines whether the collection contains a project item with the specified name and with the type that this collection stores.
		/// </summary>
		/// <param name="itemName">Name of the project item.</param>
		/// <returns>True if the collection contains any project item with the specified name.</returns>
		bool Contains(string itemName);

		/// <summary>
		/// Determines whether the collection contains any project item with the specified name. This must not neccessarily
		/// a item of the type that this collection stores (some collections can have a shared name dictionary).
		/// In constrast, use <see cref="Contains(string)"/> to determine if the collection contains an item with the specified name and the native type that the collection stores.
		/// </summary>
		/// <param name="itemName">Name of the project item.</param>
		/// <returns>True if the collection contains any project item with the specified name.</returns>
		bool ContainsAnyName(string itemName);

		/// <summary>
		/// Removes the project item with the specified name.
		/// </summary>
		/// <param name="projectItemName">Name of the project item.</param>
		/// <returns>True if a project item with the specified name was part of the collection and could be removed; otherwise, false.</returns>
		bool Remove(string projectItemName);

		/// <summary>
		/// Removes the specified project item.
		/// </summary>
		/// <param name="projectItem">The project item.</param>
		/// <returns>True if the specified item was part of the collection and could be removed; otherwise, false.</returns>
		bool Remove(IProjectItem projectItem);

		/// <summary>
		/// Gets the <see cref="IProjectItem"/> with the specified project item name.
		/// </summary>
		/// <value>
		/// The <see cref="IProjectItem"/>.
		/// </value>
		/// <param name="projectItemName">Name of the project item.</param>
		/// <returns>The project item with the specified name. If such an item does not exist, an exception is thrown.</returns>
		IProjectItem this[string projectItemName] { get; }

		/// <summary>
		/// Adds the specified project item to the collection.
		/// </summary>
		/// <param name="projectItem">The project item.</param>
		void Add(IProjectItem projectItem);

		/// <summary>
		/// Looks for the next free standard project item name in the specified folder.
		/// </summary>
		/// <param name="folder">The folder where to find a unique project item name.</param>
		/// <returns>New item name. Some items can not have a new name (e.g. items that exists only once per folder). In this case, the unique name is returned.</returns>
		string FindNewItemNameInFolder(string folder);

		/// <summary>
		/// Looks for the next unique project item name base on a basic name.
		/// </summary>
		/// <returns>A new project item name unique for this collection.</returns>
		string FindNewItemName(string basicname);

		/// <summary>
		/// Looks for the next free standard project item name in the root folder.
		/// </summary>
		/// <returns>A new project item name that is unique in this collection.</returns>
		string FindNewItemName();

		/// <summary>
		/// Fired when one or more project items are added, deleted or renamed. Not fired when content in the project item has changed.
		/// Arguments are the type of change, the item that changed, the old name (if renamed), and the new name (if renamed).
		/// This event can not be suspended.
		/// </summary>
		event EventHandler<Main.NamedObjectCollectionChangedEventArgs> CollectionChanged;

		/// <summary>
		/// Enumerates all project items in the collection.
		/// </summary>
		/// <value>
		/// The project items.
		/// </value>
		IEnumerable<IProjectItem> ProjectItems { get; }
	}
}
