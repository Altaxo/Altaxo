using System;

namespace Altaxo.Graph
{
	public class GraphSet : System.Runtime.Serialization.IDeserializationCallback, System.Collections.ICollection, Altaxo.Main.IDocumentNode
	{
		// Data
		protected System.Collections.Hashtable m_GraphsByName = new System.Collections.Hashtable();
		protected AltaxoDocument m_Parent=null;


		public GraphSet(AltaxoDocument _parent)
		{
			this.m_Parent = _parent;
		}

		public AltaxoDocument Parent
		{
			get { return this.parent; }
			set { this.parent=value; }
		}

		public object ParentObject
		{
			get { return this.parent; }
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
				Altaxo.Data.GraphSet s = (Altaxo.Data.GraphSet)obj;
				// info.AddValue("Parent",s.parent);
				info.AddValue("Graphs",s.tablesByName);
			}
			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
			{
				Altaxo.Data.GraphSet s = (Altaxo.Data.GraphSet)obj;
				// s.parent = (AltaxoDocument)(info.GetValue("Parent",typeof(AltaxoDocument)));
				s.tablesByName = (System.Collections.Hashtable)(info.GetValue("Graphs",typeof(System.Collections.Hashtable)));
			
				return s;
			}
		}

		public void OnDeserialization(object obj)
		{
			if(!m_DeserializationFinished && obj is DeserializationFinisher) // if deserialization has completely finished now
			{
				m_DeserializationFinished = true;
				DeserializationFinisher finisher = new DeserializationFinisher(this);
				// set the parent object for the data tables
				foreach(DataTable dt in tablesByName.Values)
				{
					dt.ParentGraphSet = this;
					dt.OnDeserialization(finisher);
				}
			}
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
			if(null==theGraph.Name || 0==theGraph.Name.Length) // if no table name provided
				theGraph.Name = FindNewGraphName();									// find a new one
			else if(m_GraphsByName.ContainsKey(theGraph.Name)) // else if this table name is already in use
				theGraph.Name = FindNewGraphName(theGraph.Name); // find a new table name based on the original name

			// now the table has a unique name in any case
			m_GraphsByName.Add(theTable.TableName,theTable);
			theGraph.Parent = this; 
		}

		public void Remove(Altaxo.Graph.GraphDocument theGraph)
		{
			if(m_GraphsByName.ContainsValue(theGraph))
				m_GraphsByName.Remove(theGraph.Name);

			this.OnTableDataChanged(theGraph);
		}


		/// <summary>
		/// Looks for the next free standard table name.
		/// </summary>
		/// <returns>A new table name unique for this data set.</returns>
		public string FindNewName()
		{
			return FindNewName("GRAPH");
		}	

		/// <summary>
		/// Looks for the next unique table name base on a basic name.
		/// </summary>
		/// <returns>A new table name unique for this data set.</returns>
		public string FindNewName(string basicname)
		{
			for(int i=0;;i++)
			{
				if(null==m_GraphsByName[basicname+i.ToString()])
					return basicname+i; 
			}
		}	
	}
}
