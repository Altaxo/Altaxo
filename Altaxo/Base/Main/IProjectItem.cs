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
	/// Interface that all Altaxo project items must support. Currently, project items are the base items of an Altaxo<see cref="T:Altaxo.Main.Document"/>,
	/// i.e. <see cref="T:Altaxo.Data.DataTable"/>s, <see cref="T:Altaxo.Graph.Gdi.GraphDocument"/>s and <see cref="T:Altaxo.Main.Properties.ProjectFolderPropertyDocument"/>.
	/// </summary>
	public interface IProjectItem :
		IDocumentLeafNode, // Project items are document nodes of the project tree
		INameOwner, // Project items must have a name and could be renamed
		IEventIndicatedDisposable, // project items must announce themselves when disposed
		ICloneable, // project items could be cloned
		ISuspendableByToken
	{
		/// <summary>
		/// Has to enumerate all references to other items in the project (<see cref="DocNodeProxy"/>) which are used in this project item and in all childs of this project item. The references
		/// has to be reported to the <paramref name="ProxyProcessing"/> function. This function is responsible for processing of the proxies, for instance to relocated the path.
		/// </summary>
		/// <param name="ProxyProcessing">Function that processes  the found <see cref="DocNodeProxy"/> instances.</param>
		void VisitDocumentReferences(DocNodeProxyReporter ProxyProcessing);

		/// <summary>
		/// Gets the time this project item was created (in universal time UTC).
		/// </summary>
		/// <value>
		/// The UTC creation time of the project item.
		/// </value>
		DateTime CreationTimeUtc { get; }
	}
}