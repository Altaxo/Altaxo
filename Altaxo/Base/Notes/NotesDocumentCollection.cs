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

using Altaxo.Main;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Altaxo.Notes
{
	/// <summary>
	/// Stores the <see cref="NotesDocument"/>s of the project.
	/// </summary>
	public class NotesDocumentCollection :
		ProjectItemCollectionBase<NotesDocument, IProjectItem>,
		IEnumerable<NotesDocument>
	{
		public NotesDocumentCollection(AltaxoDocument parent, SortedDictionary<string, IProjectItem> commonDictionaryForProjectItems)
			: base(parent)
		{
			this._itemsByName = commonDictionaryForProjectItems ?? throw new ArgumentNullException(nameof(commonDictionaryForProjectItems));
		}

		/// <inheritdoc/>
		public override Main.IDocumentNode ParentObject
		{
			get
			{
				return _parent;
			}
			set
			{
				if (null != value)
					throw new InvalidOperationException("ParentObject of NotesDocumentCollection is fixed and cannot be set");
				base.ParentObject = value; // allow setting to null
			}
		}

		/// <inheritdoc/>
		public override string ItemBaseName { get { return "Note"; } }

		/// <summary>
		/// Gets the parent NotesDocumentCollection of a child NotesDocument.
		/// </summary>
		/// <param name="child">A NotesDocument for which the parent collection is searched.</param>
		/// <returns>The parent NotesDocumentCollection, if it exists, or null otherwise.</returns>
		public static NotesDocumentCollection GetParentNotesDocumentCollectionOf(Main.IDocumentLeafNode child)
		{
			return (NotesDocumentCollection)Main.AbsoluteDocumentPath.GetRootNodeImplementing(child, typeof(NotesDocumentCollection));
		}
	}
}
