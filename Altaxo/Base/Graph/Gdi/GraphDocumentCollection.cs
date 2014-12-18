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

using Altaxo;
using Altaxo.Main;
using System;
using System.Collections.Generic;

namespace Altaxo.Graph.Gdi
{
	public class GraphDocumentCollection :
		Main.SuspendableDocumentNodeWithSetOfEventArgs,
		System.Runtime.Serialization.IDeserializationCallback,
		IEnumerable<GraphDocument>,
		Altaxo.Main.INamedObjectCollection
	{
		// Data
		protected SortedDictionary<string, GraphDocument> _graphsByName = new SortedDictionary<string, GraphDocument>();

		protected bool _isDirty = false;

		// Events

		/// <summary>
		/// Fired when one or more graphs are added, deleted or renamed. Not fired when content in the graph has changed.
		/// </summary>
		public event EventHandler<Main.NamedObjectCollectionChangedEventArgs> CollectionChanged;

		public GraphDocumentCollection(AltaxoDocument parent)
		{
			this._parent = parent;
		}

		public override Main.IDocumentNode ParentObject
		{
			get
			{
				return _parent;
			}
			set
			{
				throw new InvalidOperationException("ParentObject of GraphDocumentCollection is fixed and cannot be set");
			}
		}

		public override string Name
		{
			get { return "Graphs"; }
			set
			{
				throw new InvalidOperationException("Name of GraphDocumentCollection is fixed and cannot be set");
			}
		}

		#region Serialization

		public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			public void GetObjectData(object obj, System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			{
				GraphDocumentCollection s = (GraphDocumentCollection)obj;
				// info.AddValue("Parent",s.parent);
				info.AddValue("Graphs", s._graphsByName);
			}

			public object SetObjectData(object obj, System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context, System.Runtime.Serialization.ISurrogateSelector selector)
			{
				GraphDocumentCollection s = (GraphDocumentCollection)obj;
				// s.parent = (AltaxoDocument)(info.GetValue("Parent",typeof(AltaxoDocument)));
				s._graphsByName = (SortedDictionary<string, GraphDocument>)(info.GetValue("Graphs", typeof(SortedDictionary<string, GraphDocument>)));

				return s;
			}
		}

		public void OnDeserialization(object obj)
		{
		}

		#endregion Serialization

		public bool IsDirty
		{
			get
			{
				return _isDirty;
			}
		}

		public string[] GetSortedGraphNames()
		{
			string[] arr = new string[_graphsByName.Count];
			this._graphsByName.Keys.CopyTo(arr, 0);
			System.Array.Sort(arr);
			return arr;
		}

		public GraphDocument this[string name]
		{
			get
			{
				return (GraphDocument)_graphsByName[name];
			}
		}

		public bool Contains(string graphname)
		{
			return _graphsByName.ContainsKey(graphname);
		}

		public bool TryGetValue(string graphName, out GraphDocument doc)
		{
			return _graphsByName.TryGetValue(graphName, out doc);
		}

		public void Add(GraphDocument theGraph)
		{
			if (!string.IsNullOrEmpty(theGraph.Name) && _graphsByName.ContainsKey(theGraph.Name) && theGraph.Equals(_graphsByName[theGraph.Name]))
				return; // do silently nothing if the graph (the same!) is already registered
			if (string.IsNullOrEmpty(theGraph.Name)) // if no table name provided
				theGraph.Name = FindNewName();                  // find a new one
			else if (_graphsByName.ContainsKey(theGraph.Name)) // else if this table name is already in use
				theGraph.Name = FindNewName(theGraph.Name); // find a new table name based on the original name

			// now the table has a unique name in any case
			_graphsByName.Add(theGraph.Name, theGraph);
			theGraph.ParentObject = this;
			theGraph.NameChanged += EhChild_NameChanged;
			this.EhSelfChanged(Main.NamedObjectCollectionChangedEventArgs.FromItemAdded(theGraph));
		}

		public void Remove(GraphDocument theGraph)
		{
			if (theGraph != null && theGraph.Name != null)
			{
				GraphDocument gr = (GraphDocument)_graphsByName[theGraph.Name];

				if (null != Current.ComManager && object.ReferenceEquals(gr, Current.ComManager.EmbeddedObject)) // test if the graph is currently the embedded Com object
					return; // it is not allowed to remove the current embedded graph object.

				if (object.ReferenceEquals(gr, theGraph))
				{
					var changedEventArgs = Main.NamedObjectCollectionChangedEventArgs.FromItemRemoved(theGraph);
					_graphsByName.Remove(theGraph.Name);
					theGraph.ParentObject = null;
					theGraph.NameChanged -= EhChild_NameChanged;
					this.EhSelfChanged(changedEventArgs);
				}
			}
		}

		protected void EhChild_NameChanged(Main.INameOwner item, string oldName)
		{
			if (_graphsByName.ContainsKey(item.Name))
			{
				if (object.ReferenceEquals(_graphsByName[item.Name], item))
					return;
				else
					throw new ApplicationException(string.Format("The GraphDocumentCollection contains already a Graph named {0}, renaming the old graph {1} fails.", item.Name, oldName));
			}
			_graphsByName.Remove(oldName);
			_graphsByName[item.Name] = (GraphDocument)item;
			this.EhSelfChanged(Main.NamedObjectCollectionChangedEventArgs.FromItemRenamed(item, oldName));
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
				if (!_graphsByName.ContainsKey(basicname + i.ToString()))
					return basicname + i;
			}
		}

		public object GetChildObjectNamed(string name)
		{
			GraphDocument result = null;
			if (_graphsByName.TryGetValue(name, out result))
				return result;
			else return null;
		}

		public string GetNameOfChildObject(object o)
		{
			if (o is GraphDocument)
			{
				GraphDocument gr = (GraphDocument)o;
				if (_graphsByName.ContainsKey(gr.Name))
					return gr.Name;
			}
			return null;
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
			return (GraphDocumentCollection)Main.DocumentPath.GetRootNodeImplementing(child, typeof(GraphDocumentCollection));
		}

		#region IEnumerable<GraphDocument> Members

		IEnumerator<GraphDocument> IEnumerable<GraphDocument>.GetEnumerator()
		{
			return _graphsByName.Values.GetEnumerator();
		}

		#endregion IEnumerable<GraphDocument> Members

		#region IEnumerable Members

		public System.Collections.IEnumerator GetEnumerator()
		{
			return _graphsByName.Values.GetEnumerator();
		}

		#endregion IEnumerable Members
	}
}