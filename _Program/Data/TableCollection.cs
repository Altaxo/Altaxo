/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002 Dr. Dirk Lellinger
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

using System;
using Altaxo.Serialization;


namespace Altaxo.Data
{
	/// <summary>
	/// Summary description for Altaxo.Data.DataTableCollection.
	/// </summary>
	[SerializationSurrogate(0,typeof(Altaxo.Data.DataTableCollection.SerializationSurrogate0))]
	[SerializationVersion(0)]
	public class DataTableCollection 
		:
		System.Runtime.Serialization.IDeserializationCallback, 
		System.Collections.ICollection, 
		Altaxo.Main.IDocumentNode,
		Main.INamedObjectCollection,
		Altaxo.Main.IChangedEventSource,
		Altaxo.Main.IChildChangedEventSink,
		Main.IResumable
	{
		// Types
		public delegate void OnDataChanged(Altaxo.Data.DataTableCollection sender);   // delegate declaration
		public delegate void OnDirtySet(Altaxo.Data.DataTableCollection sender);

		// Data
		protected System.Collections.Hashtable m_TablesByName = new System.Collections.Hashtable();
		protected AltaxoDocument m_Parent=null;

		// helper data
		public event System.EventHandler Changed;
		[NonSerialized()]
		protected System.Collections.ArrayList m_SuspendedChildCollection = new System.Collections.ArrayList();
		[NonSerialized()]
		protected int  m_SuspendCount=0;
		[NonSerialized()]
		private   bool m_ResumeInProgress=false;
		[NonSerialized()]
		protected bool m_IsDirty=false;
		[NonSerialized()]
		private bool m_DeserializationFinished=false;


		public DataTableCollection(AltaxoDocument _parent)
		{
			this.m_Parent = _parent;
		}

		public AltaxoDocument ParentDocument
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
			get { return "Tables"; }
		}

		#region Serialization
		public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{
				Altaxo.Data.DataTableCollection s = (Altaxo.Data.DataTableCollection)obj;
				// info.AddValue("Parent",s.m_Parent);
				info.AddValue("Tables",s.m_TablesByName);
			}
			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
			{
				Altaxo.Data.DataTableCollection s = (Altaxo.Data.DataTableCollection)obj;
				// s.m_Parent = (AltaxoDocument)(info.GetValue("Parent",typeof(AltaxoDocument)));
				s.m_TablesByName = (System.Collections.Hashtable)(info.GetValue("Tables",typeof(System.Collections.Hashtable)));

				// setup helper objects
				s.m_SuspendedChildCollection = new System.Collections.ArrayList();
				return s;
			}
		}

		public void OnDeserialization(object obj)
		{
			if(!m_DeserializationFinished && obj is DeserializationFinisher) // if deserialization has completely finished now
			{
				m_DeserializationFinished = true;
				DeserializationFinisher finisher = new DeserializationFinisher(this);
				// set the m_Parent object for the data tables
				foreach(DataTable dt in m_TablesByName.Values)
				{
					dt.ParentDataSet = this;
					dt.OnDeserialization(finisher);
				}
			}
		}

		#endregion


		#region "ICollection support"

		public void CopyTo(Array array, int index)
		{
			m_TablesByName.Values.CopyTo(array,index);
		}

		public int Count 
		{
			get { return m_TablesByName.Count; }
		}

		public bool IsSynchronized
		{
			get { return m_TablesByName.IsSynchronized; }
		}

		public object SyncRoot
		{
			get { return m_TablesByName.SyncRoot; }
		}

		public System.Collections.IEnumerator GetEnumerator()
		{
			return m_TablesByName.Values.GetEnumerator();
		}



		#endregion

		#region Suspend and resume

		public bool IsSuspended
		{
			get { return m_SuspendCount>0; }
		}

		public void Suspend()
		{
			++m_SuspendCount; // suspend one step higher
		}

		public void Resume()
		{
			if(m_SuspendCount>0 && (--m_SuspendCount)==0)
			{
				this.m_ResumeInProgress = true;
				foreach(Main.IResumable obj in m_SuspendedChildCollection)
					obj.Resume();
				m_SuspendedChildCollection.Clear();
				this.m_ResumeInProgress = false;
			
				// send accumulated data if available and release it thereafter
				if(this.IsDirty)
				{
					OnChildChanged(null,EventArgs.Empty); // simulate a data changed event
					this.m_IsDirty = false;
				}

			
			}
		}


		void AccumulateChildChangeData(object sender, EventArgs e)
		{
			if(sender!=null)
				this.m_IsDirty = true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>This is the standard pseudocode for changed notifications:
		/// <code>
		/// bool OnChildChanged(object sender, ... (additional arguments specifing changed details)
		/// {
		/// if(IsSuspended)
		///		{
		///		if(sender is Main.IResumable)
		///     m_SuspendedChildCollection.Add(sender); // add sender to suspended child
		///   else
		///    	AccumulateChildChangeData(sender,e);
		///		return true; // signal the child that it should be suspend further notifications
		///		}
		///	else // not suspended
		///		{
		///		if(m_ResumeInProgress)
		///			{
		///			AccumulateChildChangeData(sender,e);	// if we have notification data, accumulate them
		///			return false;  // signal not suspended to the parent
		///			}
		///		else // no resume in progress
		///			{
		///			if(m_Parent is Main.IChildChangedEventSink and true==((Main.IChildChangedEventSink)m_Parent).OnChildChanged(this, EventArgs.Empty))
		///				{
		///				this.Suspend();
		///				return this.OnChildChanged(sender, e); // we call the function recursively, but now we are suspended
		///				}
		///			else // parent is not suspended
		///				{
		///				OnChanged(); // Fire the changed event
		///				return false; // signal not suspended to the parent
		///				}
		///			}
		///		}
		/// </code> 
		///</remarks>
		public bool OnChildChanged(object sender, System.EventArgs e)
		{
			if(IsSuspended)
			{
				if(sender is Main.IResumable)
					m_SuspendedChildCollection.Add(sender); // add sender to suspended child
				else
					AccumulateChildChangeData(sender,e);
				return true; // signal the child that it should be suspend further notifications
			}
			else // not suspended
			{
				if(m_ResumeInProgress)
				{
					AccumulateChildChangeData(sender,e);
					return false;  // signal not suspended to the parent
				}
				else // no resume in progress
				{
					if(m_Parent is Main.IChildChangedEventSink && true==((Main.IChildChangedEventSink)m_Parent).OnChildChanged(this, EventArgs.Empty))
					{
						this.Suspend();
						return this.OnChildChanged(sender, e); // we call the function recursively, but now we are suspended
					}
					else // parent is not suspended
					{
						OnChanged(EventArgs.Empty); // Fire the changed event
						return false; // signal not suspended to the parent
					}
				}
			}
		}

		protected virtual void OnChanged(EventArgs e)
		{
			if(null!=Changed)
				Changed(this,e);
		}

		#endregion


		public AltaxoDocument Document
		{
			get 
			{
				return m_Parent;
			}
		}

		public bool IsDirty
		{
			get
			{
				return m_IsDirty;
			}
		}

		public string[] GetSortedTableNames()
		{
			string[] arr = new string[m_TablesByName.Count];
			this.m_TablesByName.Keys.CopyTo(arr,0);
			System.Array.Sort(arr);
			return arr;
		}

		protected internal void ResetDirty()
		{
			this.m_SuspendedChildCollection.Clear();
			m_IsDirty=false;
		}


		public Altaxo.Data.DataTable this[string name]
		{
			get
			{
				return (Altaxo.Data.DataTable)m_TablesByName[name];
			}
		}

		public bool ContainsTable(string tablename)
		{
			return m_TablesByName.ContainsKey(tablename);
		}

		public void Add(Altaxo.Data.DataTable theTable)
		{
			if(null==theTable.TableName || 0==theTable.TableName.Length) // if no table name provided
				theTable.TableName = FindNewTableName();									// find a new one
			else if(m_TablesByName.ContainsKey(theTable.TableName)) // else if this table name is already in use
				theTable.TableName = FindNewTableName(theTable.TableName); // find a new table name based on the original name

			// now the table has a unique name in any case
			m_TablesByName.Add(theTable.TableName,theTable);
			theTable.ParentDataSet = this; 

			// raise data event to all listeners
			OnChanged(EventArgs.Empty);

		}

		public void Remove(Altaxo.Data.DataTable theTable)
		{
			if(m_TablesByName.ContainsValue(theTable))
				m_TablesByName.Remove(theTable.TableName);

			this.OnChanged(EventArgs.Empty);
		}


		/// <summary>
		/// Looks for the next free standard table name.
		/// </summary>
		/// <returns>A new table name unique for this data set.</returns>
		public string FindNewTableName()
		{
			return FindNewTableName("WKS");
		}	

		/// <summary>
		/// Looks for the next unique table name base on a basic name.
		/// </summary>
		/// <returns>A new table name unique for this data set.</returns>
		public string FindNewTableName(string basicname)
		{
			for(int i=0;;i++)
			{
				if(null==m_TablesByName[basicname+i.ToString()])
					return basicname+i; 
			}
		}	

		public object GetChildObjectNamed(string name)
		{
			return m_TablesByName[name];
		}

		public string GetNameOfChildObject(object o)
		{
			if(o is DataTable)
			{
				DataTable gr = (DataTable)o;
				if(m_TablesByName.ContainsKey(gr.Name))
					return gr.Name;
			}
			return null;
		}
	}
}
