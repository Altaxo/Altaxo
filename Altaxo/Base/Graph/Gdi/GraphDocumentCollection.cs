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

#endregion Copyright

using Altaxo.Main;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Altaxo.Graph.Gdi
{
	public class GraphDocumentCollection :
		ProjectItemCollectionBase<GraphDocument, GraphDocumentBase>,
		IEnumerable<GraphDocument>
	{
		public GraphDocumentCollection(AltaxoDocument parent, SortedDictionary<string, GraphDocumentBase> commonDictionaryForGraphs)
			: base(parent)
		{
			this._itemsByName = commonDictionaryForGraphs ?? throw new ArgumentNullException(nameof(commonDictionaryForGraphs));
		}

		public override Main.IDocumentNode ParentObject
		{
			get
			{
				return _parent;
			}
			set
			{
				if (null != value)
					throw new InvalidOperationException("ParentObject of GraphDocumentCollection is fixed and cannot be set");
				base.ParentObject = value; // allow setting to null
			}
		}

		public override string ItemBaseName { get { return "GRAPH"; } }

		/// <summary>
		/// Gets the parent GraphDocumentCollection of a child graph.
		/// </summary>
		/// <param name="child">A graph for which the parent collection is searched.</param>
		/// <returns>The parent GraphDocumentCollection, if it exists, or null otherwise.</returns>
		public static GraphDocumentCollection GetParentGraphDocumentCollectionOf(Main.IDocumentLeafNode child)
		{
			return (GraphDocumentCollection)Main.AbsoluteDocumentPath.GetRootNodeImplementing(child, typeof(GraphDocumentCollection));
		}
	}
}