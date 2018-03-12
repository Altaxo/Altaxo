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
		/// Determines whether the collection contains any project item with the specified name.
		/// </summary>
		/// <param name="projectItemName">Name of the project item.</param>
		/// <returns>True if the collection contains any project item with the specified name.</returns>
		bool ContainsAnyName(string projectItemName);

		bool Remove(string projectItemName);

		bool Remove(IProjectItem projectItem);

		IProjectItem this[string projectItemName] { get; }

		void Add(IProjectItem projectItem);

		/// <summary>
		/// Looks for the next free standard project item name in the specified folder.
		/// </summary>
		/// <param name="folder">The folder where to find a unique project item name.</param>
		/// <returns>New item name. Some items can not have a new name (e.g. items that exists only once per folder). In this case, the unique name is returned.</returns>
		string FindNewItemNameInFolder(string folder);
	}
}
