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
		Main.SuspendableDocumentNodeWithSetOfEventArgs,
		Main.IProjectItemCollection,
		IEnumerable<GraphDocument>
	{
		// Data
		//protected SortedDictionary<string, GraphDocument> _graphsByName = new SortedDictionary<string, GraphDocument>();
		protected SortedDictionary<string, IProjectItem> _allGraphsByName;

		protected bool _isDirty = false;

		// Events

		/// <summary>
		/// Fired when one or more graphs are added, deleted or renamed. Not fired when content in the graph has changed.
		/// </summary>
		public event EventHandler<Main.NamedObjectCollectionChangedEventArgs> CollectionChanged;

		public GraphDocumentCollection(AltaxoDocument parent, SortedDictionary<string, IProjectItem> commonDictionaryForGraphs)
		{
			if (null == commonDictionaryForGraphs)
				throw new ArgumentNullException(nameof(commonDictionaryForGraphs));

			this._parent = parent;
			this._allGraphsByName = commonDictionaryForGraphs;
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

		public bool IsDirty
		{
			get
			{
				return _isDirty;
			}
		}

		/// <summary>
		/// Gets the name of the <see cref="Altaxo.Graph.Gdi.GraphDocument"/>s sorted by name.
		/// </summary>
		/// <returns></returns>
		public string[] GetSortedGraphNames()
		{
			var list = new List<string>(_allGraphsByName.Where(entry => entry.Value is Altaxo.Graph.Gdi.GraphDocument).Select(entry => entry.Key));
			list.Sort();
			return list.ToArray();
		}

		public GraphDocument this[string name]
		{
			get
			{
				return (GraphDocument)_allGraphsByName[name];
			}
		}

		public bool ContainsAnyName(string projectItemName)
		{
			return null != projectItemName && _allGraphsByName.ContainsKey(projectItemName);
		}

		public bool Contains(string graphname)
		{
			IProjectItem doc;
			return null != graphname && _allGraphsByName.TryGetValue(graphname, out doc) && (doc is GraphDocument);
		}

		public bool Contains(GraphDocument doc)
		{
			IProjectItem containedDoc;
			return null != doc && null != doc.Name && _allGraphsByName.TryGetValue(doc.Name, out containedDoc) && object.ReferenceEquals(doc, containedDoc);
		}

		public bool TryGetValue(string graphName, out GraphDocument doc)
		{
			IProjectItem d;
			if (_allGraphsByName.TryGetValue(graphName, out d))
			{
				doc = d as GraphDocument;
				return null != doc;
			}
			else
			{
				doc = null;
				return false;
			}
		}

		public void Add(GraphDocument theGraph)
		{
			if (!string.IsNullOrEmpty(theGraph.Name) && Contains(theGraph))
				return; // do silently nothing if the graph (the same!) is already registered
			if (string.IsNullOrEmpty(theGraph.Name)) // if no table name provided
				theGraph.Name = FindNewName();                  // find a new one
			else if (_allGraphsByName.ContainsKey(theGraph.Name)) // else if this table name is already in use
				theGraph.Name = FindNewName(theGraph.Name); // find a new table name based on the original name

			// now the table has a unique name in any case
			_allGraphsByName.Add(theGraph.Name, theGraph);
			theGraph.ParentObject = this;
			this.EhSelfChanged(Main.NamedObjectCollectionChangedEventArgs.FromItemAdded(theGraph));
		}

		public void Remove(GraphDocument theGraph)
		{
			if (theGraph != null && theGraph.Name != null)
			{
				GraphDocument gr;
				if (!TryGetValue(theGraph.Name, out gr))
					return;

				if (null != Current.ComManager && object.ReferenceEquals(gr, Current.ComManager.EmbeddedObject)) // test if the graph is currently the embedded Com object
					return; // it is not allowed to remove the current embedded graph object.

				if (object.ReferenceEquals(gr, theGraph))
				{
					var changedEventArgs = Main.NamedObjectCollectionChangedEventArgs.FromItemRemoved(theGraph);
					_allGraphsByName.Remove(theGraph.Name);
					theGraph.Dispose();
					this.EhSelfChanged(changedEventArgs);
				}
			}
		}

		bool Main.IParentOfINameOwnerChildNodes.EhChild_CanBeRenamed(Main.INameOwner childNode, string newName)
		{
			if (_allGraphsByName.ContainsKey(newName) && !object.ReferenceEquals(_allGraphsByName[newName], childNode))
				return false;
			else
				return true;
		}

		void Main.IParentOfINameOwnerChildNodes.EhChild_HasBeenRenamed(Main.INameOwner item, string oldName)
		{
			IProjectItem containedItem;
			if (_allGraphsByName.TryGetValue(item.Name, out containedItem))
			{
				if (object.ReferenceEquals(containedItem, item))
					return; // alredy renamed
				else
					throw new ApplicationException("Graph with name " + item.Name + " already exists!");
			}

			if (_allGraphsByName.TryGetValue(oldName, out containedItem))
			{
				if (!object.ReferenceEquals(containedItem, item))
					throw new ApplicationException("Names between GraphDocumentCollection and graph not in sync");

				_allGraphsByName.Remove(oldName);
				_allGraphsByName.Add(item.Name, (GraphDocument)item);

				EhSelfChanged(Main.NamedObjectCollectionChangedEventArgs.FromItemRenamed(item, oldName));
			}
			else
			{
				throw new ApplicationException("Error renaming graph " + oldName + " : this graph name was not found in the collection!");
			}
		}

		void Main.IParentOfINameOwnerChildNodes.EhChild_ParentChanged(Main.INameOwner childNode, Main.IDocumentNode oldParent)
		{
			if (object.ReferenceEquals(this, oldParent) && _allGraphsByName.ContainsKey(childNode.Name))
				throw new InvalidProgramException("Unauthorized change of the graphs's parent");
		}

		/// <summary>
		/// Looks for the next free standard  name.
		/// </summary>
		/// <returns>A new table name unique for this set.</returns>
		public string FindNewName()
		{
			return FindNewName("GRAPH");
		}

		/// <summary>
		/// Looks for the next unique name base on a basic name.
		/// </summary>
		/// <returns>A new  name unique for this  set.</returns>
		public string FindNewName(string basicname)
		{
			for (int i = 0; ; i++)
			{
				if (!_allGraphsByName.ContainsKey(basicname + i.ToString()))
					return basicname + i;
			}
		}

		public override Main.IDocumentLeafNode GetChildObjectNamed(string name)
		{
			GraphDocument result = null;
			if (TryGetValue(name, out result))
				return result;
			else return null;
		}

		public override string GetNameOfChildObject(Main.IDocumentLeafNode o)
		{
			if (o is GraphDocument)
			{
				GraphDocument gr = (GraphDocument)o;
				if (_allGraphsByName.ContainsKey(gr.Name))
					return gr.Name;
			}
			return null;
		}

		protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			foreach (var entry in _allGraphsByName)
				if (entry.Value is GraphDocument)
					yield return new Main.DocumentNodeAndName(entry.Value, entry.Key);
		}

		#region Change event handling

		protected override void OnChanged(EventArgs e)
		{
			if (e is NamedObjectCollectionChangedEventArgs && null != CollectionChanged)
			{
				CollectionChanged(this, (NamedObjectCollectionChangedEventArgs)e);
			}

			base.OnChanged(e);
		}

		#endregion Change event handling

		/// <summary>
		/// Gets the parent GraphDocumentCollection of a child graph.
		/// </summary>
		/// <param name="child">A graph for which the parent collection is searched.</param>
		/// <returns>The parent GraphDocumentCollection, if it exists, or null otherwise.</returns>
		public static GraphDocumentCollection GetParentGraphDocumentCollectionOf(Main.IDocumentLeafNode child)
		{
			return (GraphDocumentCollection)Main.AbsoluteDocumentPath.GetRootNodeImplementing(child, typeof(GraphDocumentCollection));
		}

		#region IEnumerable<GraphDocument> Members

		IEnumerator<GraphDocument> IEnumerable<GraphDocument>.GetEnumerator()
		{
			return _allGraphsByName.Where(entry => entry.Value is GraphDocument).Select(entry => (GraphDocument)entry.Value).GetEnumerator();
		}

		#endregion IEnumerable<GraphDocument> Members

		#region IEnumerable Members

		public System.Collections.IEnumerator GetEnumerator()
		{
			return _allGraphsByName.Where(entry => entry.Value is GraphDocument).Select(entry => (GraphDocument)entry.Value).GetEnumerator();
		}

		#endregion IEnumerable Members
	}
}