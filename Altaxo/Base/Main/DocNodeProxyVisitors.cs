#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Main
{

	/// <summary>
	/// Handler to report owned <see cref="DocNodeProxy"/> instances to a visitor.
	/// </summary>
	/// <param name="proxy">The proxy that is owned by the instance.</param>
	/// <param name="owner">The instance that owns the proxy.</param>
	/// <param name="propertyName">Name of the property that accesses the proxy in this instance.</param>
	public delegate void DocNodeProxyReporter(DocNodeProxy proxy, object owner, string propertyName);


	/// <summary>
	/// Can be used to to relocate the <see cref="DocumentPath"/> that the <see cref="DocNodeProxy"/> is holding. The <see cref="Visit"/> function implements
	/// the <see cref="DocNodeProxyReporter"/> delegate that is used to enumerate all proxies to <see cref="Altaxo.Data.DataColumn"/>s and change there references
	/// to the corresponding <see cref="Altaxo.Data.DataColumn"/>s of another table.
	/// </summary>
	public class DocNodePathReplacementOptions
	{
		List<KeyValuePair<DocumentPath, DocumentPath>> _replacementDictionary = new List<KeyValuePair<DocumentPath, DocumentPath>>();

		/// <summary>
		/// Visits a <see cref="DocNodeProxy"/> and applies the modifications to the document path of that proxy.
		/// </summary>
		/// <param name="proxy">The <see cref="DocNodeProxy"/> to modify.</param>
		/// <param name="owner">The instance that owns the proxy.</param>
		/// <param name="propertyName">Name of the property that accesses the proxy in this instance.</param>
		public void Visit(DocNodeProxy proxy, object owner, string propertyName)
		{
			if (null == proxy)
				return;

			foreach (var entry in _replacementDictionary)
			{
				if (proxy.ReplacePathParts(entry.Key, entry.Value))
					break;
			}
		}

		/// <summary>
		/// Adds a replacement rule for the document paths.
		/// </summary>
		/// <param name="partPathToReplace">Part of a document part that should be replaced.</param>
		/// <param name="newPath">Part of a document part that acts as replacement for the old path.</param>
		public void AddPathReplacement(DocumentPath partPathToReplace, DocumentPath newPath)
		{
			_replacementDictionary.Add(new KeyValuePair<DocumentPath, DocumentPath>(partPathToReplace, newPath));
		}
	}
}
