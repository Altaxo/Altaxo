using System;
using Altaxo;
using Altaxo.Main;

namespace Altaxo.Graph
{
	public class GraphDocumentCollection : 
		System.Runtime.Serialization.IDeserializationCallback,
		System.Collections.ICollection,
		Altaxo.Main.IDocumentNode,
		Altaxo.IChangedEventSource,
		Altaxo.Main.INamedObjectCollection
	{
		// Data
		protected System.Collections.Hashtable m_GraphsByName = new System.Collections.Hashtable();
		protected AltaxoDocument m_Parent=null;
		protected bool bIsDirty=false;

		public GraphDocumentCollection(AltaxoDocument parent)
		{
			this.m_Parent = parent;
		}

		public AltaxoDocument Parent
		{
			get { return this.m_Parent; }
			set { this.m_Parent=value; }
		}

		public object ParentObject
		{
			get { return this.m_Parent; }
		}

		public string Name
		{
			get { return "Graphs"; }
		}

		#region Serialization
		public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{
				Altaxo.Graph.GraphDocumentCollection s = (Altaxo.Graph.GraphDocumentCollection)obj;
				// info.AddValue("Parent",s.parent);
				info.AddValue("Graphs",s.m_GraphsByName);
			}
			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
			{
				Altaxo.Graph.GraphDocumentCollection s = (Altaxo.Graph.GraphDocumentCollection)obj;
				// s.parent = (AltaxoDocument)(info.GetValue("Parent",typeof(AltaxoDocument)));
				s.m_GraphsByName = (System.Collections.Hashtable)(info.GetValue("Graphs",typeof(System.Collections.Hashtable)));
			
				return s;
			}
		}

		public void OnDeserialization(object obj)
		{
		}

		#endregion


		#region "ICollection support"

		public void CopyTo(Array array, int index)
		{
			m_GraphsByName.Values.CopyTo(array,index);
		}

		public int Count 
		{
			get { return m_GraphsByName.Count; }
		}

		public bool IsSynchronized
		{
			get { return m_GraphsByName.IsSynchronized; }
		}

		public object SyncRoot
		{
			get { return m_GraphsByName.SyncRoot; }
		}

		public System.Collections.IEnumerator GetEnumerator()
		{
			return m_GraphsByName.Values.GetEnumerator();
		}

		#endregion



		public bool IsDirty
		{
			get
			{
				return bIsDirty;
			}
		}

		public string[] GetSortedGraphNames()
		{
			string[] arr = new string[m_GraphsByName.Count];
			this.m_GraphsByName.Keys.CopyTo(arr,0);
			System.Array.Sort(arr);
			return arr;
		}


		public Altaxo.Graph.GraphDocument this[string name]
		{
			get
			{
				return (Altaxo.Graph.GraphDocument)m_GraphsByName[name];
			}
		}

		public bool Contains(string graphname)
		{
			return m_GraphsByName.ContainsKey(graphname);
		}

		public void Add(Altaxo.Graph.GraphDocument theGraph)
		{
			if(null!=theGraph.Name && string.Empty!=theGraph.Name && theGraph.Equals(m_GraphsByName[theGraph.Name]))
				return; // do silently nothing if the graph (the same!) is already registered
			if(null==theGraph.Name || string.Empty==theGraph.Name) // if no table name provided
				theGraph.Name = FindNewName();									// find a new one
			else if(m_GraphsByName.ContainsKey(theGraph.Name)) // else if this table name is already in use
				theGraph.Name = FindNewName(theGraph.Name); // find a new table name based on the original name

			// now the table has a unique name in any case
			m_GraphsByName.Add(theGraph.Name,theGraph);
			theGraph.ParentObject = this; 
			theGraph.NameChanged += new NameChangedEventHandler(this.EhChild_NameChanged);
		}

		public void Remove(Altaxo.Graph.GraphDocument theGraph)
		{
			if(theGraph!=null && theGraph.Name!=null)
			{
				GraphDocument gr = (GraphDocument)m_GraphsByName[theGraph.Name];
				if(gr.Equals(theGraph))
				{
					m_GraphsByName.Remove(theGraph.Name);
					theGraph.ParentObject = null;
					theGraph.NameChanged -= new NameChangedEventHandler(this.EhChild_NameChanged);
					this.OnChanged(theGraph);
				}
			}
		}

		protected void EhChild_NameChanged(object sender, NameChangedEventArgs e)
		{
			// we remove the old value from the hash and store it under the new value
			object graph = m_GraphsByName[e.OldName];
			if(graph!=null)
			{
				if(m_GraphsByName.ContainsKey(e.NewName))
					throw new ApplicationException(string.Format("The GraphDocumentCollection contains already a Graph named {0}, renaming the old graph {1} fails.",e.NewName,e.OldName));
				m_GraphsByName.Remove(e.OldName);
				m_GraphsByName[e.NewName] = graph;
				this.OnChanged(this);
			}
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
			for(int i=0;;i++)
			{
				if(null==m_GraphsByName[basicname+i.ToString()])
					return basicname+i; 
			}
		}	

		public virtual void OnChanged(object sender)
		{
			if(null!=Changed)
				Changed(this,System.EventArgs.Empty);
		}

		public object GetChildObjectNamed(string name)
		{
			return m_GraphsByName[name];
		}

		public string GetNameOfChildObject(object o)
		{
			if(o is GraphDocument)
			{
				GraphDocument gr = (GraphDocument)o;
				if(m_GraphsByName.ContainsKey(gr.Name))
					return gr.Name;
			}
			return null;
		}

		#region IChangedEventSource Members

		public event System.EventHandler Changed;

		#endregion
	}
}
