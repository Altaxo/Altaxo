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
	
	[SerializationSurrogate(0,typeof(Altaxo.Data.DataColumnCollection.SerializationSurrogate0))]
	[SerializationVersion(0)]
	public class DataColumnCollection :
		System.Runtime.Serialization.IDeserializationCallback, 
		Altaxo.Main.IDocumentNode,
		IDisposable,
		ICloneable,
		Main.INamedObjectCollection,
		Main.IResumable,
		Main.IChildChangedEventSink
	{
		// Types
		public delegate void OnDataChanged(Altaxo.Data.DataColumnCollection sender, int nMinCol, int nMaxCol, int nMinRow, int nMaxRow);   // delegate declaration
		public delegate void OnDirtySet(Altaxo.Data.DataColumnCollection sender);
		
		public class ChangeEventArgs : System.EventArgs
		{
			protected int m_MinColChanged;
			protected int m_MaxColChanged;
			protected int m_MinRowChanged;
			protected int m_MaxRowChanged;
			protected int m_MaxDecreasedToRow;

			public ChangeEventArgs(int columnNumber, int minRow, int maxRow, bool rowCountDecreased, int columnCount)
			{
				m_MinColChanged = columnNumber;
				m_MaxColChanged = columnNumber;
				m_MinRowChanged = minRow;
				m_MaxRowChanged = maxRow;
				m_MaxDecreasedToRow = rowCountDecreased ?  columnCount : int.MaxValue;
			}

			public void Accumulate(int columnNumber, int minRow, int maxRow, bool rowCountDecreased, int columnCount)
			{
				if(columnNumber<m_MinColChanged)
					m_MinColChanged=columnNumber;
				if(columnNumber>m_MaxColChanged)
					m_MaxColChanged=columnNumber;
				if(minRow < m_MinRowChanged)
					m_MinRowChanged=minRow;
				if(maxRow > m_MaxRowChanged)
					m_MaxRowChanged=maxRow;
				if(rowCountDecreased && columnCount>m_MaxDecreasedToRow)
					m_MaxDecreasedToRow = columnCount;
			}

			public void Accumulate(ChangeEventArgs args)
			{
				if(args.m_MinColChanged < this.m_MinColChanged)
					this.m_MinColChanged  = args.m_MinColChanged;
				
				if(args.m_MaxColChanged > this.m_MaxColChanged)
					this.m_MaxColChanged  = args.m_MaxColChanged;
				
				if(args.m_MinRowChanged < this.m_MinRowChanged)
					this.m_MinRowChanged = args.m_MinRowChanged;
				
				if(args.MaxRowChanged  > this.m_MaxRowChanged)
					this.m_MaxRowChanged = args.m_MaxRowChanged;
				
				if(args.m_MaxDecreasedToRow < this.m_MaxDecreasedToRow)
					this.m_MaxDecreasedToRow = args.m_MaxDecreasedToRow;
			}

			public static ChangeEventArgs CreateColumnRemoveArgs(int firstColumnNumber, int originalNumberOfColumns, int maxRowCountOfRemovedColumns)
			{
				ChangeEventArgs args = new ChangeEventArgs(firstColumnNumber,0,maxRowCountOfRemovedColumns,true,0);
				if(originalNumberOfColumns > args.m_MaxColChanged)
					args.m_MaxColChanged = originalNumberOfColumns;
				return args;
			}

			public int MinColChanged
			{
				get { return m_MinColChanged; }
			}
			public int MaxColChanged
			{
				get { return m_MaxColChanged; }
			}
			public int MinRowChanged
			{
				get { return m_MinRowChanged; }
			}
			public int MaxRowChanged
			{
				get { return m_MaxRowChanged; }
			}
			public int MaxDecreasedToRow
			{
				get { return m_MaxDecreasedToRow; }
			}
		}

		// Data

		/// <summary>
		/// The parent of this DataColumnCollection, normally a DataTable.
		/// </summary>
		protected object m_Parent=null; // the DataTable this set is belonging to
		
		/// <summary>
		/// The collection of the columns (accessible by number).
		/// </summary>
		protected System.Collections.ArrayList m_ColumnsByNumber = new System.Collections.ArrayList();
		
		
		/// <summary>
		/// Cached number of rows. This is the maximum of the Count of all DataColumns contained in this collection.
		/// </summary>
		protected int m_NumberOfRows=0; // the max. Number of Rows of the columns of the table


		/// <summary>
		/// ColumnScripts, key is the corresponding column, value is of type WorksheetColumnScript
		/// </summary>
		protected ColumnScriptCollection m_ColumnScripts = new ColumnScriptCollection();

		// Helper Data
		protected System.Collections.Hashtable m_ColumnsByName=new System.Collections.Hashtable();
		
		protected string m_LastColumnNameAdded=""; // name of the last column wich was added to the table
		protected string m_LastColumnNameGenerated=""; // name of the last column name that was automatically generated

		protected int m_SuspendCount=0;
		private  bool m_ResumeInProgress=false;
		protected System.Collections.ArrayList m_SuspendedChildCollection = new System.Collections.ArrayList(); // the collection of dirty columns
		protected ChangeEventArgs m_ChangeData;
		public event EventHandler Changed;

		private bool m_DeserializationFinished=false;



		#region "Serialization"
		public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{
				Altaxo.Data.DataColumnCollection s = (Altaxo.Data.DataColumnCollection)obj;

				info.AddValue("Columns",s.m_ColumnsByNumber);
				// serialize the column scripts
				info.AddValue("ColumnScripts",s.m_ColumnScripts);
			}

			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
			{
				Altaxo.Data.DataColumnCollection s = (Altaxo.Data.DataColumnCollection)obj;
				
				s.m_ColumnsByNumber = (System.Collections.ArrayList)(info.GetValue("Columns",typeof(System.Collections.ArrayList)));
				s.m_ColumnScripts = (ColumnScriptCollection)(info.GetValue("ColumnScripts",typeof(ColumnScriptCollection)));

				// set up helper objects
				s.m_ColumnsByName=new System.Collections.Hashtable();
				s.m_SuspendedChildCollection = new System.Collections.ArrayList();
				s.m_LastColumnNameAdded="";
				s.m_LastColumnNameGenerated="";
				return s;
			}
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Altaxo.Data.DataColumnCollection),0)]
		public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info	)
			{
				Altaxo.Data.DataColumnCollection s = (Altaxo.Data.DataColumnCollection)obj;

				info.CreateArray("ColumnArray",s.m_ColumnsByNumber.Count);
				for(int i=0;i<s.m_ColumnsByNumber.Count;i++)
					info.AddValue("Col",s.m_ColumnsByNumber[i]);
				info.CommitArray();

				// serialize the column scripts
				info.CreateArray("ColumnScripts",s.m_ColumnScripts.Count);
				foreach(System.Collections.DictionaryEntry entry in s.m_ColumnScripts)
				{
					info.CreateElement("Script");
					info.AddValue("ColName", ((Altaxo.Data.DataColumn)entry.Key).ColumnName);
					info.AddValue("Content",(Altaxo.Data.ColumnScript)entry.Value);
					info.CommitElement();
				}
				info.CommitArray();
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				Altaxo.Data.DataColumnCollection s = null!=o ? (Altaxo.Data.DataColumnCollection)o : new Altaxo.Data.DataColumnCollection();
	
				

				// deserialize the columns
				int count = info.OpenArray();
				for(int i=0;i<count;i++)
				{
					object col = info.GetValue(s);
					if(col!=null)
						s.Add((DataColumn)col);
				}
				info.CloseArray(count);

				// deserialize the scripts
				count = info.OpenArray();
				for(int i=0;i<count;i++)
				{
					info.OpenElement();
					string name = info.GetString();
					ColumnScript script = (ColumnScript)info.GetValue(s);
					info.CloseElement();
					s.ColumnScripts.Add(s[name],script);
				}
				info.CloseArray(count); // end script array
				return s;
			}
		}


		public virtual void OnDeserialization(object obj)
		{
			if(!m_DeserializationFinished && obj is DeserializationFinisher)
			{
				m_DeserializationFinished = true;
				DeserializationFinisher finisher = new DeserializationFinisher(this);

				// 1. Set the parent Data table of the columns,
				// because they may be feel lonesome
				int nCols = m_ColumnsByNumber.Count;
				m_NumberOfRows = 0;
				DataColumn dc;
				for(int i=0;i<nCols;i++)
				{
					dc = (DataColumn)m_ColumnsByNumber[i];
					dc.SetParent( this );
					dc.OnDeserialization(finisher);


					// add it also to the column name cache
					m_ColumnsByName.Add(dc.ColumnName,dc);

					// count the maximumn number of rows
					if(dc.Count>m_NumberOfRows)
						m_NumberOfRows = dc.Count;
				}

			}
		}

		#endregion


		public DataColumnCollection()
		{
			this.m_Parent = null;
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="from">The column collection to copy this data column collection from.</param>
		public DataColumnCollection(DataColumnCollection from)
		{
			this.m_LastColumnNameAdded = from.m_LastColumnNameAdded;
			this.m_LastColumnNameGenerated = from.m_LastColumnNameGenerated;

			// Copy all Columns
			for(int i=0;i<from.ColumnCount;i++)
				this.Add( (DataColumn)from[i].Clone());

			// Copy all Column scripts
			foreach(System.Collections.DictionaryEntry d in from.ColumnScripts)
			{
				DataColumn srccol = (DataColumn)d.Key; // the original column this script belongs to
				DataColumn destcol = this[srccol.ColumnName]; // the new (cloned) column the script now belongs to
				ColumnScript destscript = (ColumnScript)((ColumnScript)d.Value).Clone(); // the cloned script

				// add the cloned script to the own collection
				this.ColumnScripts.Add(destcol,destscript);
			}
		}

		public virtual object Clone()
		{
			return new DataColumnCollection(this);
		}

		public virtual void Dispose()
		{
			// first relase all column scripts
			foreach(System.Collections.DictionaryEntry d in this.m_ColumnScripts)
			{
				if(d.Value is IDisposable)
					((IDisposable)d.Value).Dispose();
			}
			m_ColumnScripts.Clear();


			// release all owned Data columns
			this.m_ColumnsByName.Clear();

			for(int i=0;i<m_ColumnsByNumber.Count;i++)
				this[i].Dispose();

			m_ColumnsByNumber.Clear();
			this.m_NumberOfRows = 0;
		}

		#region Column Addition / Access / Removal 

		public void AddColumns(System.Collections.ArrayList cols)
		{
			for(int i=0;i<cols.Count;i++)
			{
				if(cols[i] is Altaxo.Data.DBNullColumn)
					Add(new Altaxo.Data.DoubleColumn());
				else
					Add((Altaxo.Data.DataColumn)cols[i]);
			}
		}


		public void Add(Altaxo.Data.DataColumn datac)
		{
			Add(ColumnCount,datac);
		}

		public virtual void Add(int idx, Altaxo.Data.DataColumn datac)
		{
			bool bColumnWasReplaced = false;

			if(idx<ColumnCount) // we have to replace the column
			{
				if(this[idx].GetType()==datac.GetType()) // test whether the column types are equal
				{
					// if the types are equal, then copy the data and then the column name
					this[idx] = datac; // first copy the data
					//this[idx].CopyColumnInformationFrom(datac);
					// and now the names
					if(datac.ColumnName!=null && datac.ColumnName != this[idx].ColumnName)
					{
						// so we try to replace the column names
						m_ColumnsByName.Remove(this[idx].ColumnName);
						// Test for unique column name
						if(null!=m_ColumnsByName[datac.ColumnName])
						{
							this[idx].ColumnName = this.FindUniqueColumnName(datac.ColumnName);
							this.m_LastColumnNameGenerated = this[idx].ColumnName; // store the last generated column name
						}
						// put it back into the name array
						this[idx].ColumnName = datac.ColumnName;
						this[idx].CopyHeaderInformationFrom(datac);
						m_ColumnsByName.Add(this[idx].ColumnName,this[idx]);
					}
					bColumnWasReplaced = true;
					return; // we are ready in this case
				}
				else  // the types are not equal, so we have to remove the col and insert a new one
				{
					string oldcolname = this[idx].ColumnName;
					m_ColumnsByName.Remove(oldcolname);
					this[idx].Dispose();
					m_ColumnsByNumber[idx]=null;
					bColumnWasReplaced=true;

					if(datac.ColumnName==null)
						datac.ColumnName = oldcolname; // use if possible the name of the old column
				}
			}

			// Test for unique column name
			if(null==datac.ColumnName || null!=m_ColumnsByName[datac.ColumnName])
			{
				datac.ColumnName = this.FindUniqueColumnName(datac.ColumnName);
				this.m_LastColumnNameGenerated = datac.ColumnName; // store the last generated column name
			}

			// store this column name as the last column name that was added for purpose 
			// of finding new names in the future
			this.m_LastColumnNameAdded = datac.ColumnName;

			datac.SetParent( this ); // set the column parent
			if(idx<m_ColumnsByNumber.Count)
			{
				m_ColumnsByNumber[idx] = datac;
				datac.SetColumnNumber( idx ); // set the column number
			}
			else
			{
				m_ColumnsByNumber.Add(datac); // insert the column first, then
				datac.SetColumnNumber( ColumnCount -1 ); // set the column number
			}
			m_ColumnsByName.Add(datac.ColumnName,datac);	

			// raise data event to all listeners
			//OnColumnDataChanged(datac,0,datac.Count-1,bColumnWasReplaced);
			//this.OnChildChanged(datac,new DataColumn.ChangeEventArgs(0,datac.Count-1,bColumnWasReplaced));
			this.OnChildChanged(null, new ChangeEventArgs(datac.ColumnNumber,0,datac.Count,bColumnWasReplaced,datac.Count));
		}


		/// <summary>
		/// Insert a number of empty rows in all columns.
		/// </summary>
		/// <param name="nBeforeRow">The row number before which the additional rows should be inserted.</param>
		/// <param name="nRowsToInsert">The number of rows to insert.</param>
		public void InsertRows(int nBeforeRow, int nRowsToInsert)
		{
			Suspend();

			for(int i=0;i<ColumnCount;i++)
				this[i].InsertRows(nBeforeRow,nRowsToInsert);
		
			Resume();
		}

		/// <summary>
		/// Removes a number of rows from all columns.
		/// </summary>
		/// <param name="nFirstRow">Number of first row to remove.</param>
		/// <param name="nCount">Number of rows to remove, starting from nFirstRow.</param>
		public void RemoveRows(int nFirstRow, int nCount)
		{
			Suspend();

			for(int i=0;i<this.ColumnCount;i++)
				this[i].RemoveRows(nFirstRow,nCount);
		
			Resume();
		}


		public Altaxo.Data.DataColumn this[string s]
		{
			get
			{
				return (Altaxo.Data.DataColumn)m_ColumnsByName[s];
			}
			set
			{
				// setting a column should not change its name nor its other properties
				// only the data array and the related parameters should be changed
				Altaxo.Data.DataColumn c = (Altaxo.Data.DataColumn)m_ColumnsByName[s];

				if(null!=c)
				{
					c.CopyDataFrom(value);
				}
				else
				{
					throw new Exception("The column \"" + s + "\" in node \"" + Main.DocumentPath.GetAbsolutePath(this).ToString() + "\" does not exist.");
				}
			}
		}

		
		public Altaxo.Data.DataColumn this[int idx]
		{
			get
			{
				return (Altaxo.Data.DataColumn)m_ColumnsByNumber[idx];
			}
			set
			{
				// setting a column should not change its name nor its other properties
				// only the data array and the related parameters should be changed
				
				if(idx<m_ColumnsByNumber.Count)
				{
					Altaxo.Data.DataColumn c = (Altaxo.Data.DataColumn)m_ColumnsByNumber[idx];
					c.CopyDataFrom(value);
				}
				else
				{
					throw new Exception("The column[" + idx + "] in table \"" + Main.DocumentPath.GetAbsolutePath(this).ToString() + "\" does not exist.");
				}
			}
		}


		public virtual void RemoveColumns(int nFirstColumn, int nDelCount)
		{
			int nOriginalColumnCount = ColumnCount;
			// first, Dispose the columns and set the places to null
			for(int i=nFirstColumn+nDelCount-1;i>=nFirstColumn;i--)
			{
				this.m_ColumnsByName.Remove(this[i].ColumnName);
				this[i].Dispose();
			}
			this.m_ColumnsByNumber.RemoveRange(nFirstColumn, nDelCount);
			
			// renumber the remaining columns
			for(int i=m_ColumnsByNumber.Count-1;i>=nFirstColumn;i--)
				this[i].SetColumnNumber(i);

			// raise datachange event that some columns have changed
			this.OnChildChanged(null, ChangeEventArgs.CreateColumnRemoveArgs(nFirstColumn, nOriginalColumnCount, RowCount));
		}

		public void RemoveColumn(int nFirstColumn)
		{
			RemoveColumns(nFirstColumn,1);
		}

		public void DeleteRows(int nFirstRow, int nDelCount)
		{
			this.Suspend();
			
			for(int i=this.ColumnCount-1;i>=0;i--)
			{
				this[i].RemoveRows(nFirstRow,nDelCount);
			}

			this.Resume();
		}

		public void DeleteRow(int nFirstRow)
		{
			DeleteRows(nFirstRow,1);
		}




		#endregion // Column addition / access / removal


		#region Properties

		public int RowCount
		{
			get
			{
				return m_NumberOfRows;
			}
		}
	
		public int ColumnCount
		{
			get
			{
				return this.m_ColumnsByNumber.Count;
			}
		}
	

		public virtual object ParentObject
		{
			get { return m_Parent; }
			set { m_Parent=value; }
		}

		public virtual string Name
		{
			get 
			{
				Main.INamedObjectCollection noc = ParentObject as Main.INamedObjectCollection;
				return noc==null ? null : noc.GetNameOfChildObject(this);
			}
		}

		public ColumnScriptCollection ColumnScripts
		{
			get { return this.m_ColumnScripts; }
		}


		#endregion

		/// <summary>
		/// removes the x-property from all columns in the group nGroup
		/// </summary>
		/// <param name="nGroup">the group number for the columns from which to remove the x-property</param>
		public void DeleteXProperty(int nGroup, DataColumn exceptThisColumn)
		{
			int len = this.ColumnCount;
			for(int i=len-1;i>=0;i--)
			{
				DataColumn dc = (DataColumn)m_ColumnsByNumber[i];
				if(dc.Group==nGroup && dc.XColumn && !dc.Equals(exceptThisColumn))
				{
					dc.XColumn=false;
				}
			}
		}

		public Altaxo.Data.DataColumn FindXColumnOfGroup(int nGroup)
		{
			int len = this.ColumnCount;
			for(int i=len-1;i>=0;i--)
			{
				DataColumn dc = (DataColumn)m_ColumnsByNumber[i];
				if(dc.Group==nGroup && dc.XColumn)
				{
					return dc;
				}
			}
			return null;
		}


		#region Event handling


		public bool IsSuspended
		{
			get { return m_SuspendCount>0; }
		}

		public virtual void Suspend()
		{
			m_SuspendCount++;
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
				if(null!=m_ChangeData)
				{
					if(m_Parent is Main.IChildChangedEventSink && true==((Main.IChildChangedEventSink)m_Parent).OnChildChanged(this, m_ChangeData))
					{
						this.Suspend();
						// Note: AccumulateChangeData is not neccessary here, since we still have the ChangeData
					}
					else // parent is not suspended
					{
						OnChanged(m_ChangeData); // Fire the changed event
						m_ChangeData=null; // dispose the change data
					}		
				}
			}
		}



		void AccumulateChildChangeData(object sender, EventArgs e)
		{
			DataColumn.ChangeEventArgs changed = e as DataColumn.ChangeEventArgs;
			if(changed!=null && sender is DataColumn)
			{
				int columnNumber = ((DataColumn)sender).ColumnNumber;
				int columnCount = ((DataColumn)sender).Count;

				if(m_ChangeData==null)
					m_ChangeData = new ChangeEventArgs(columnNumber,changed.MinRowChanged,changed.MaxRowChanged,changed.RowCountDecreased, columnCount);
				else 
					m_ChangeData.Accumulate(columnNumber,changed.MinRowChanged,changed.MaxRowChanged,changed.RowCountDecreased, columnCount);

				// update the row count in case the nMaxRow+1 is greater than the chached row count
				if(columnCount > m_NumberOfRows)
					m_NumberOfRows = columnCount;
			}
			else if(e is ChangeEventArgs)
			{
				if(null==m_ChangeData)
					m_ChangeData = (ChangeEventArgs)e;
				else
					m_ChangeData.Accumulate((ChangeEventArgs)e);
			}
		}

		public void HandleImmediateChildChangeCases(object sender, System.EventArgs e)
		{
			ColumnKindChangeEventArgs ck = e as ColumnKindChangeEventArgs;
			if(ck!=null)
			{
				if(ck.NewKind==ColumnKind.X)
					this.DeleteXProperty(((DataColumn)sender).Group,(DataColumn)sender);
			}
		}

		public bool OnChildChanged(object sender, System.EventArgs e)
		{
			HandleImmediateChildChangeCases(sender, e);
			if(IsSuspended)
			{
				if(sender is Main.IResumable)
					m_SuspendedChildCollection.Add(sender); // add sender to suspended child
				else
					AccumulateChildChangeData(sender,e);	// AccumulateNotificationData
				return true; // signal the child that it should be suspend further notifications
			}
			else // not suspended
			{
				if(m_ResumeInProgress)
				{
					AccumulateChildChangeData(sender,e);// AccumulateNotificationData(...) -> not available here 
					return false;  // signal not suspended to the parent
				}
				else // no resume in progress
				{
					if(m_Parent is Main.IChildChangedEventSink && true==((Main.IChildChangedEventSink)m_Parent).OnChildChanged(this, m_ChangeData))
					{
						this.Suspend();
						return this.OnChildChanged(sender, e); // we call the function recursively, but now we are suspended
					}
					else // parent is not suspended
					{
						OnChanged(m_ChangeData); // Fire the changed event
						return false; // signal not suspended to the parent
					}
				}
			}
		}

		protected virtual void OnChanged(ChangeEventArgs e)
		{
			if(null!=Changed)
				Changed(this,e);
		}




		public void RefreshRowCount()
		{
			RefreshRowCount(true);
		}


		public void RefreshRowCount(bool bSearchOnlyUntilOldRowCountReached)
		{
			int rowCount=0;

			if(bSearchOnlyUntilOldRowCountReached)
			{
				foreach(DataColumn c in this.m_ColumnsByNumber)
				{
					rowCount = System.Math.Max(rowCount,c.Count);
					if(rowCount>=m_NumberOfRows) 
						break;
				}
			}
			else
			{
				foreach(DataColumn c in this.m_ColumnsByNumber)
					rowCount = System.Math.Max(rowCount,c.Count);
			}

			// now take over the new row count
			this.m_NumberOfRows =  rowCount;

		}


		#endregion


		/// <summary>
		/// Test if a column of a given name is present in this collection.
		/// </summary>
		/// <param name="columnname">The columnname to test for presence.</param>
		/// <returns>True if the column with the name is contained in the collection.</returns>
		public bool ContainsColumn(string columnname)
		{
			return m_ColumnsByName.ContainsKey(columnname);
		}

		public virtual bool IsDirty
		{
			get
			{
				return null!=m_ChangeData;
			}
		}

	



		#region Automatic column naming

		public string FindNewColumnName()
		{
			return FindUniqueColumnName(null);
		}


		public static string GetNextColumnName(string lastName)
		{
			int lastNameLength = lastName.Length;
			
			if(0==lastNameLength)
				return "A";
			else if(1==lastNameLength)
			{
				char _1st = lastName[0];
				_1st++;
				if(_1st>='A' && _1st<='Z')
					return _1st.ToString();
				else
					return "AA";
			}
			else if(2==lastNameLength)
			{
				char _1st = lastName[0];
				char _2nd = lastName[1];
				_2nd++;

				if(_2nd<'A' || _2nd>'Z')
				{
					_1st++;
					_2nd='A';
				}

				if(_1st>='A' && _1st<='Z')
					return _1st.ToString() + _2nd;
				else
					return ((uint)System.Guid.NewGuid().GetHashCode()).ToString("X8");
			}

			else
			{
				return ((uint)System.Guid.NewGuid().GetHashCode()).ToString("X8");
			}

		}

		public string FindUniqueColumnName(string _sbase)
		{
			// die ColumnNamen sollen hier wie folgt gehen
			// - von A bis Z
			// - AA ... AZ, BA ... BZ, ..... ZA...ZZ
			
			string sbase = _sbase==null ? "" : _sbase + ".";

			int lastNameLength = m_LastColumnNameAdded.Length;


			// for the very first try, if no base provided, try to use a name that follows the name of the last columns
			if(null==_sbase )
			{
				string tryName = GetNextColumnName(m_ColumnsByNumber.Count>0 ? this[ColumnCount-1].ColumnName : "");
				if(null==this[tryName])
					return tryName;
			}
			else // a base is provided, so try to use this name
			{
				if(null==this[_sbase])
					return _sbase;
			}

			// zuerst A bis Z probieren

			if(lastNameLength==1)
			{
				char _1st = m_LastColumnNameAdded[0];
				_1st++;
				for(char i=_1st;i<='Z';i++)
				{
					if(null==this[sbase+i])
						return sbase+i;
				}
			}
			else if(lastNameLength==2)
			{
				char _1st = m_LastColumnNameAdded[0];
				char _2nd = m_LastColumnNameAdded[1];
				_2nd++;

				// try it first with the unchanged 1st letter, but vary the second letter
				for(char j=_2nd;j<='Z';j++)
				{
					if(null==this[sbase + _1st.ToString() + j])
						return sbase + _1st.ToString() + j;
				}

				// Try it now also with variing first and second letter
				_1st++;
				for(char i=_1st;i<='Z';i++)
				{
					for(char j='A';j<='Z';j++)
					{
						if(null==this[sbase + i.ToString()+j])
							return sbase + i.ToString()+j;
					}
				}	
			}
			else if(lastNameLength==8) // maybe we have used a hash code before, so we use it again
			{
				bool bIsHex=true;

				try
				{
					System.Convert.ToUInt32(m_LastColumnNameAdded,16); // is it a hexadecimal hash code ?
				}
				catch(Exception)
				{
					bIsHex=false;
				}

				if(bIsHex) // if it was a hex code, then try immediatly another hash code
				{
					for(;;)
					{
						string hash = ((uint)System.Guid.NewGuid().GetHashCode()).ToString("X8");
						if(null==this[sbase + hash])
							return sbase + hash;
					}
				}
			}

			// if that all does not help, try it again with A to Z
			for(char i='A';i<='Z';i++)
			{
				if(null==this[sbase + i.ToString()])
					return sbase + i.ToString();
			}

			// Try it with the combination AA so that the next time the lastNameLength is 2 and the naming
			// goes further with AB, AC ...
			if(null==this[sbase + "AA"])
				return sbase + "AA";


			// if that also not helps, use the hash code of a guid
			for(;;)
			{
				string hash = ((uint)System.Guid.NewGuid().GetHashCode()).ToString("X8");
				if(null==this[sbase + hash])
					return sbase + hash;
			}
		}


		#endregion


		public string[] GetColumnNames()
		{
			string[] arr = new string[this.ColumnCount];
			for(int i=0;i<arr.Length;i++)
				arr[i] = this[i].ColumnName;

			return arr;
		}

		public bool AreAllColumnsOfTheSameType(out int firstdifferentcolumnindex)
		{
			firstdifferentcolumnindex=0;

			if(0==this.ColumnCount)
				return true;

			System.Type t1st = this[0].GetType();

			int len = this.ColumnCount;
			for(int i=0;i<len;i++)
			{
				if(this[i].GetType()!=t1st)
				{
					firstdifferentcolumnindex = i;
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Transpose transpose the table, i.e. exchange columns and rows
		/// this can only work if all columns in the table are of the same type
		/// </summary>
		/// <returns>null if succeeded, error string otherwise</returns>
		public virtual string Transpose()
		{
			int firstdifferent;

			if(!AreAllColumnsOfTheSameType(out firstdifferent))
			{
				return String.Format("Column[{0}] ({1}) has a different type than the first column transpose is not possible!",firstdifferent,this[firstdifferent].ColumnName);
			}

			// now we can start by adding additional columns of the row count is greater
			// than the column count
			this.Suspend();
			int originalrowcount = this.RowCount;
			int originalcolcount = this.ColumnCount;
			if(this.RowCount>this.ColumnCount)
			{
				int addcols = this.RowCount - this.ColumnCount;
				// this is a little tricky - we have to add the same type of
				// column like the first one
				System.Type coltype = this[0].GetType();
				for(int ii=0;ii<addcols;ii++)
				{
					Altaxo.Data.DataColumn dt = (Altaxo.Data.DataColumn)Activator.CreateInstance(coltype);
					this.Add(dt);
				}
			} // if RowCount>ColumnCount

			// now we can exchange the data

			int tocol;
			int i,j;
			if(originalcolcount>=originalrowcount)
			{
				for(i=0;i<originalrowcount;i++)
				{
					
					for(j=i+1;j<originalcolcount;j++)
					{
						Altaxo.Data.AltaxoVariant hlp = this[j][i];
						this[j][i] = this[i][j];
						this[i][j] = hlp;
					}
				}

			}
			else // originalrowcount>originalcolcount
			{
				tocol = originalcolcount;
				for(i=0;i<originalcolcount;i++)
				{
					for(j=i+1;j<originalrowcount;j++)
					{
						Altaxo.Data.AltaxoVariant hlp = this[i][j];
						this[i][j] = this[j][i];
						this[j][i] = hlp;
					}
				}

			}

			// now we should delete the superfluous columns when originalcolcount>originalrowcount
			if(originalcolcount>originalrowcount)
			{
				this.RemoveColumns(originalrowcount,originalcolcount-originalrowcount);
			}

			this.Resume();


			return null; // no error message
		}
		#region INamedObjectCollection Members

		public object GetChildObjectNamed(string name)
		{
			return this.m_ColumnsByName[name];
		}

		public string GetNameOfChildObject(object o)
		{
			DataColumn dc = o as DataColumn;
			if(dc!=null && m_ColumnsByName.ContainsKey(dc.Name))
				return dc.Name;

			return null;
		}

		#endregion
	} // end class Altaxo.Data.DataColumnCollection

}
