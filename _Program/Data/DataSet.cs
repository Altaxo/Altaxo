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
	/// Summary description for Altaxo.Data.DataSet.
	/// </summary>
	[SerializationSurrogate(0,typeof(Altaxo.Data.DataSet.SerializationSurrogate0))]
	[SerializationVersion(0)]
	public class DataSet : System.Runtime.Serialization.IDeserializationCallback, System.Collections.ICollection
	{
		// Types
		public delegate void OnDataChanged(Altaxo.Data.DataSet sender);   // delegate declaration
		public delegate void OnDirtySet(Altaxo.Data.DataSet sender);

		// Data
		protected System.Collections.Hashtable tablesByName = new System.Collections.Hashtable();
		protected AltaxoDocument parent=null;

		// helper data
		public event OnDirtySet FireDirtySet;
		[NonSerialized()]
		protected System.Collections.Stack dirtyTables = new System.Collections.Stack(); // collection of tables marked as dirty
		[NonSerialized()]
		protected int  nDataEventsSuspendCount=0;
		[NonSerialized()]
		private   bool bDataEventsResumeInProgress=false;
		[NonSerialized()]
		protected bool bIsDirty=false;
		[NonSerialized()]
		private bool m_DeserializationFinished=false;


		public DataSet(AltaxoDocument _parent)
		{
			this.parent = _parent;
		}

		public AltaxoDocument ParentDocument
		{
			get { return this.parent; }
			set { this.parent=value; }
		}


		#region Serialization
		public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{
				Altaxo.Data.DataSet s = (Altaxo.Data.DataSet)obj;
				// info.AddValue("Parent",s.parent);
				info.AddValue("Tables",s.tablesByName);
			}
			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
			{
				Altaxo.Data.DataSet s = (Altaxo.Data.DataSet)obj;
				// s.parent = (AltaxoDocument)(info.GetValue("Parent",typeof(AltaxoDocument)));
				s.tablesByName = (System.Collections.Hashtable)(info.GetValue("Tables",typeof(System.Collections.Hashtable)));

				// setup helper objects
				s.dirtyTables = new System.Collections.Stack();
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
					dt.ParentDataSet = this;
					dt.OnDeserialization(finisher);
				}
			}
		}

		#endregion


		#region "ICollection support"

		public void CopyTo(Array array, int index)
		{
			tablesByName.Values.CopyTo(array,index);
		}

		public int Count 
		{
			get { return tablesByName.Count; }
		}

		public bool IsSynchronized
		{
			get { return tablesByName.IsSynchronized; }
		}

		public object SyncRoot
		{
			get { return tablesByName.SyncRoot; }
		}

		public System.Collections.IEnumerator GetEnumerator()
		{
			return tablesByName.Values.GetEnumerator();
		}



		#endregion

		protected internal void AddDirtyTable(Altaxo.Data.DataTable s)
		{
			dirtyTables.Push(s);
		}

		public void SuspendDataChangedNotifications()
		{
			nDataEventsSuspendCount++; // suspend one step higher
		}

		public void ResumeDataChangedNotifications()
		{
			this.nDataEventsSuspendCount--;
			if(this.nDataEventsSuspendCount<0) this.nDataEventsSuspendCount = 0;

			if(this.nDataEventsSuspendCount==0)
			{
				this.bDataEventsResumeInProgress = true;
				// first, Resume the data changed events for all child columns 
				foreach(Altaxo.Data.DataTable ta in dirtyTables)
					ta.ResumeDataChangedNotifications();
				dirtyTables.Clear();
				this.bDataEventsResumeInProgress = false;
			}
			}

		public void OnTableDataChanged(Altaxo.Data.DataTable sender)
		{
			bool bWasDirtyBefore = this.IsDirty;

			if(null!=sender)
			{
				bIsDirty = true;
				if(this.bDataEventsResumeInProgress==true)
					return;
			}

			if(this.nDataEventsSuspendCount==0)
			{
				// notify the parent
				parent.OnDirtySet(this);
			}
			if(this.nDataEventsSuspendCount==0) // we reevaluate this in case the document was changing this
			{
				// Fire data change event
			ResetDirty();
			}
			else  // Data events are disabled
			{
				// if Data events are Disabled, Disable it also in the table
				if(null!=sender)
				{
					dirtyTables.Push(sender);
					sender.SuspendDataChangedNotifications();
				}
				if(!bWasDirtyBefore && null!=FireDirtySet)
					FireDirtySet(this);
			}
		}

		public AltaxoDocument Document
		{
			get 
			{
				return parent;
			}
		}

		public bool IsDirty
		{
			get
			{
				return bIsDirty;
			}
		}

		protected internal void ResetDirty()
		{
			this.dirtyTables.Clear();
			bIsDirty=false;
		}


		public Altaxo.Data.DataTable this[string name]
		{
			get
			{
				return (Altaxo.Data.DataTable)tablesByName[name];
			}
		}

		public bool ContainsTable(string tablename)
		{
			return tablesByName.ContainsKey(tablename);
		}

		public void Add(Altaxo.Data.DataTable theTable)
		{
			if(null==theTable.TableName || 0==theTable.TableName.Length) // if no table name provided
				theTable.TableName = FindNewTableName();									// find a new one
			else if(tablesByName.ContainsKey(theTable.TableName)) // else if this table name is already in use
				theTable.TableName = FindNewTableName(theTable.TableName); // find a new table name based on the original name

			// now the table has a unique name in any case
			tablesByName.Add(theTable.TableName,theTable);
			theTable.ParentDataSet = this; 

			// raise data event to all listeners
			OnTableDataChanged(theTable);

		}

		public void Remove(Altaxo.Data.DataTable theTable)
		{
			if(tablesByName.ContainsValue(theTable))
				tablesByName.Remove(theTable.TableName);

			this.OnTableDataChanged(theTable);
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
				if(null==tablesByName[basicname+i.ToString()])
					return basicname+i; 
			}
		}	
	}
}
