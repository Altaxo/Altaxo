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

	public class AltaxoUniqueNameException : System.ApplicationException
	{
	}

	[SerializationSurrogate(0,typeof(Altaxo.Data.DataTable.SerializationSurrogate0))]
	[SerializationVersion(0)]
	public class DataTable :System.Runtime.Serialization.IDeserializationCallback
		{
		// Types
		public delegate void OnDataChanged(Altaxo.Data.DataTable sender, int nMinCol, int nMaxCol, int nMinRow, int nMaxRow);   // delegate declaration
		public delegate void OnDirtySet(Altaxo.Data.DataTable sender);
		
		// Data
		protected DataSet m_Parent=null; // the dataSet that this table is belonging to
		protected string tableName=null; // the name of the table
		protected System.Collections.ArrayList columnsByNumber = new System.Collections.ArrayList();
		protected int nNumberOfRows=0; // the max. Number of Rows of the columns of the table

		/// <summary>
		/// ColumnScripts, key is the corresponding column, value is of type WorksheetColumnScript
		/// </summary>
		protected ColumnScriptCollection m_ColumnScripts = new ColumnScriptCollection();

		// Helper Data
		public event OnDataChanged FireDataChanged;
		protected internal event OnDirtySet FireDirtySet;
		protected System.Collections.Hashtable columnsByName=new System.Collections.Hashtable();
		protected System.Collections.Stack dirtyColumns = new System.Collections.Stack(); // the collection of dirty columns
		
		protected string lastColumnNameAdded=""; // name of the last column wich was added to the table
		protected string lastColumnNameGenerated=""; // name of the last column name that was automatically generated

		protected int nDataEventsSuspendCount=0;
		private  bool bResumeDataEventsInProgress=false;

		protected int nMinColChanged = int.MaxValue;
		protected int nMaxColChanged = int.MinValue;
		protected int nMinRowChanged = int.MaxValue;
		protected int nMaxRowChanged = int.MinValue;
		protected int nMaxDecreasedToRow = int.MinValue;
		protected bool bIsDirty    = false;

		private bool m_DeserializationFinished=false;

		public DataTable(string name)
		{
			this.tableName = name;
		}

		public DataTable(Altaxo.Data.DataSet parent)
		{
			this.m_Parent = parent;
		}

		public DataTable(Altaxo.Data.DataSet parent, string name)
		{
			this.m_Parent = parent;
			this.tableName = name;
		}
  

		#region "Serialization"
		public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{
				Altaxo.Data.DataTable s = (Altaxo.Data.DataTable)obj;
				// info.AddValue("Parent",s.m_Parent); // not serialized
				info.AddValue("Name",s.tableName);
				info.AddValue("NumberOfRows",s.nNumberOfRows);
				//info.AddValue("ColumnsByName",s.columnsByName);
				info.AddValue("Columns",s.columnsByNumber);

				// serialize the column scripts
				info.AddValue("ColumnScripts",s.m_ColumnScripts);
			}
			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
			{
				Altaxo.Data.DataTable s = (Altaxo.Data.DataTable)obj;
				//s.m_Parent = (Altaxo.Data.DataSet)(info.GetValue("Parent",typeof(Altaxo.Data.DataSet)));
				s.tableName = info.GetString("Name");
				s.columnsByNumber = (System.Collections.ArrayList)(info.GetValue("Columns",typeof(System.Collections.ArrayList)));
				s.m_ColumnScripts = (ColumnScriptCollection)(info.GetValue("ColumnScripts",typeof(ColumnScriptCollection)));

				// set up helper objects
				s.columnsByName=new System.Collections.Hashtable();
				s.dirtyColumns = new System.Collections.Stack();
				s.lastColumnNameAdded="";
				s.lastColumnNameGenerated="";
				s.nMinColChanged = int.MaxValue;
				s.nMaxColChanged = int.MinValue;
				s.nMinRowChanged = int.MaxValue;
				s.nMaxRowChanged = int.MinValue;
				s.nMaxDecreasedToRow = int.MinValue;
				return s;
			}
		}

		public void OnDeserialization(object obj)
		{
			if(!m_DeserializationFinished && obj is DeserializationFinisher)
			{
				m_DeserializationFinished = true;
				DeserializationFinisher finisher = new DeserializationFinisher(this);

				// 1. Set the parent Data table of the columns,
				// because they may be feel lonesome
				int nCols = columnsByNumber.Count;
				DataColumn dc;
				for(int i=0;i<nCols;i++)
				{
					dc = (DataColumn)columnsByNumber[i];
					
					dc.ParentTable = this;
					dc.OnDeserialization(finisher);

					// add it also to the column name cache
					columnsByName.Add(dc.ColumnName,dc);
				}
			}
		}

		#endregion

		public Altaxo.Data.DataSet ParentDataSet
		{
			get { return m_Parent; }
			set { m_Parent = value; }
		}

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

		public void Add(int idx, Altaxo.Data.DataColumn datac)
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
						columnsByName.Remove(this[idx].ColumnName);
						// Test for unique column name
						if(null!=columnsByName[datac.ColumnName])
						{
							this[idx].ColumnName = this.FindUniqueColumnName(datac.ColumnName);
							this.lastColumnNameGenerated = this[idx].ColumnName; // store the last generated column name
						}
						// put it back into the name array
						this[idx].ColumnName = datac.ColumnName;
						this[idx].CopyHeaderInformationFrom(datac);
						columnsByName.Add(this[idx].ColumnName,this[idx]);
					}
					bColumnWasReplaced = true;
					return; // we are ready in this case
				}
				else  // the types are not equal, so we have to remove the col and insert a new one
				{
					string oldcolname = this[idx].ColumnName;
					columnsByName.Remove(oldcolname);
					this[idx].Dispose();
					columnsByNumber[idx]=null;
					bColumnWasReplaced=true;

					if(datac.ColumnName==null)
						datac.ColumnName = oldcolname; // use if possible the name of the old column
				}
			}

			// Test for unique column name
			if(null==datac.ColumnName || null!=columnsByName[datac.ColumnName])
				{
				datac.ColumnName = this.FindUniqueColumnName(datac.ColumnName);
				this.lastColumnNameGenerated = datac.ColumnName; // store the last generated column name
				}

			// store this column name as the last column name that was added for purpose 
			// of finding new names in the future
			this.lastColumnNameAdded = datac.ColumnName;

			datac.ParentTable = this; // set the column parent
			if(idx<columnsByNumber.Count)
			{
				columnsByNumber[idx] = datac;
				datac.SetColumnNumber( idx ); // set the column number
			}
			else
			{
				columnsByNumber.Add(datac); // insert the column first, then
				datac.SetColumnNumber( ColumnCount -1 ); // set the column number
			}
			columnsByName.Add(datac.ColumnName,datac);	

			// raise data event to all listeners
			OnColumnDataChanged(datac,0,datac.Count,bColumnWasReplaced);
		}

		public Altaxo.Data.DataColumn this[string s]
		{
			get
			{
				return (Altaxo.Data.DataColumn)columnsByName[s];
			}
			set
			{
				// setting a column should not change its name nor its other properties
				// only the data array and the related parameters should be changed
				Altaxo.Data.DataColumn c = (Altaxo.Data.DataColumn)columnsByName[s];

				if(null!=c)
				{
					c.CopyDataFrom(value);
				}
				else
				{
					throw new Exception("The column \"" + s + "\" in table \"" + this.TableName + "\" does not exist.");
				}
			}
		}

		
		public Altaxo.Data.DataColumn this[int idx]
		{
			get
			{
				return (Altaxo.Data.DataColumn)columnsByNumber[idx];
			}
			set
			{
				// setting a column should not change its name nor its other properties
				// only the data array and the related parameters should be changed
				
				if(idx<columnsByNumber.Count)
				{
					Altaxo.Data.DataColumn c = (Altaxo.Data.DataColumn)columnsByNumber[idx];
					c.CopyDataFrom(value);
				}
				else
				{
					throw new Exception("The column[" + idx + "] in table \"" + this.TableName + "\" does not exist.");
				}
			}
		}

/*
		public AltaxoDataColumnCollection Columns
			{
			get
				{
				return columns;
				}
			}
*/
		public int RowCount
		{
			get
			{
				return nNumberOfRows;
			}
		}
		public int ColumnCount
		{
			get
			{
				return this.columnsByNumber.Count;
			}
		}




/// <summary>
/// get or sets the name of the Table
/// </summary>
		public string TableName
		{
			get
			{
				return tableName;
			}
			set
			{

				if(null!=ParentDataSet)
				{
					if(null!=ParentDataSet[value])
					{
						throw(new AltaxoUniqueNameException());
					}
				}
				tableName = value;
			}
		}

/*
		public Altaxo.Data.DataColumn this[int i]
			{
			get
				{
				return (Altaxo.Data.DataColumn)columns[i];
				}
			set
				{
				// hier ist etwas mehr Aufwand notwendig, wir dürfen nur die Daten
				// übernehmen, nicht jedoch die Kopfdaten der Columne
				if(i>=0 && i<columns.Count)
					{
					((Altaxo.Data.DataColumn)columns[i]).CopyDataFrom(value);
					}
				} // end set	
			} // end indexer [int i]
*/

		/// <summary>
		/// removes the x-property from all columns in the group nGroup
		/// </summary>
		/// <param name="nGroup">the group number for the columns from which to remove the x-property</param>
		public void DeleteXProperty(int nGroup)
		{
			int len = this.ColumnCount;
			for(int i=len-1;i>=0;i--)
			{
				DataColumn dc = (DataColumn)columnsByNumber[i];
				if(dc.Group==nGroup && dc.XColumn)
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
				DataColumn dc = (DataColumn)columnsByNumber[i];
				if(dc.Group==nGroup && dc.XColumn)
				{
					return dc;
				}
			}
			return null;
		}




		public void SuspendDataChangedNotifications()
		{
			nDataEventsSuspendCount++;
		}

		public void ResumeDataChangedNotifications()
		{
			nDataEventsSuspendCount--;
			if(nDataEventsSuspendCount<0) nDataEventsSuspendCount=0;

			// first, Resume the data changed events for all child columns 
			if(nDataEventsSuspendCount==0)
			{
				bResumeDataEventsInProgress=true;

				foreach(Altaxo.Data.DataColumn tc in dirtyColumns)
					tc.ResumeDataChangedNotifications();
				dirtyColumns.Clear();

				bResumeDataEventsInProgress=false;
				// then Resume it for the table itself
				if(this.IsDirty)
					OnColumnDataChanged(null,0,0,false); // simulate a data changed event
			}
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
				foreach(DataColumn c in this.columnsByNumber)
				{
					rowCount = System.Math.Max(rowCount,c.Count);
					if(rowCount>=nNumberOfRows) 
						break;
				}
			}
			else
			{
				foreach(DataColumn c in this.columnsByNumber)
					rowCount = System.Math.Max(rowCount,c.Count);
			}

			// now take over the new row count
			this.nNumberOfRows =  rowCount;

		}


		public void OnColumnDataChanged(Altaxo.Data.DataColumn sender, int nMinRow, int nMaxRow, bool rowCountDecreased)
		{
			if(null!=ParentDataSet)
			{
				bool bWasDirtyBefore = this.IsDirty;

				if(null!=sender)
				{
					int nCol = sender.ColumnNumber;
					if(nCol<nMinColChanged) nMinColChanged=nCol;
					if(nCol>nMaxColChanged) nMaxColChanged=nCol;
					if(nMinRow<nMinRowChanged) nMinRowChanged=nMinRow;
					if(nMaxRow>nMaxRowChanged) nMaxRowChanged=nMaxRow;
					if(rowCountDecreased && sender.Count>nMaxDecreasedToRow) nMaxDecreasedToRow = sender.Count;

					// update the row count in case the nMaxRow+1 is greater than the chached row count
					if(nMaxRow+1>nNumberOfRows)
						nNumberOfRows=nMaxRow+1;
					
					bIsDirty = true;
		
					if(bResumeDataEventsInProgress)
						return; // only update the data if resume is in progress
				}
				else // null==sender, so probably from myself
				{
					bIsDirty=true;
					if(nMinRow<nMinRowChanged) nMinRowChanged=nMinRow;
					if(nMaxRow>nMaxRowChanged) nMaxRowChanged=nMaxRow;
					if(rowCountDecreased) 
						nMaxDecreasedToRow = nMinRow;
				}


				if(nDataEventsSuspendCount==0)
				{
					// inform the parent first
					this.ParentDataSet.OnTableDataChanged(this);
				}
				
				if(nDataEventsSuspendCount==0) // reevaluate this variable because parent can change it during notification
				{
					if(nMaxDecreasedToRow>=0) // if some row count decreased to this value 
					{
						RefreshRowCount(true);
					}
		
					if(null!=FireDataChanged)
						FireDataChanged(this, nMinColChanged,nMaxColChanged,nMinRowChanged,nMaxRowChanged);
			
					ResetDirty();
				}
				else // Data events disabled
				{
					// if Data events are Disabled, Disable it also in the column
					if(null!=sender)
					{
					this.dirtyColumns.Push(sender);
					sender.SuspendDataChangedNotifications();
					}

					if(!bWasDirtyBefore && null!=FireDirtySet)
						FireDirtySet(this);
					
				}
			}
		}

			public bool IsDirty
			{
				get
				{
					return bIsDirty;
				}
				set
				{
					bIsDirty |= value;
				}
			}
		protected void ResetDirty()
		{
			nMinColChanged = int.MaxValue;
			nMaxColChanged = int.MinValue;
			nMinRowChanged = int.MaxValue;
			nMaxRowChanged = int.MinValue;
			nMaxDecreasedToRow = int.MinValue;
			bIsDirty = false;
		}


		protected internal void AddDirtyColumn(Altaxo.Data.DataColumn s)
		{
			dirtyColumns.Push(s);
		}

		public void OnColumnDirtySet(Altaxo.Data.DataColumn sender)
		{
			AddDirtyColumn(sender);
			bool bWasDirtyBefore = this.IsDirty;
			bIsDirty = true;
			if(!bWasDirtyBefore)
				FireDirtySet(this);
		}


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

			int lastNameLength = lastColumnNameAdded.Length;


			// for the very first try, if no base provided, try to use a name that follows the name of the last columns
			if(null==_sbase )
			{
				string tryName = GetNextColumnName(columnsByNumber.Count>0 ? this[ColumnCount-1].ColumnName : "");
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
				char _1st = lastColumnNameAdded[0];
				_1st++;
				for(char i=_1st;i<='Z';i++)
				{
					if(null==this[sbase+i])
						return sbase+i;
				}
			}
			else if(lastNameLength==2)
			{
				char _1st = lastColumnNameAdded[0];
				char _2nd = lastColumnNameAdded[1];
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
					System.Convert.ToUInt32(lastColumnNameAdded,16); // is it a hexadecimal hash code ?
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


		public void RemoveColumns(int nFirstColumn, int nDelCount)
		{
			int nOriginalColumnCount = ColumnCount;
			// first, Dispose the columns and set the places to null
			for(int i=nFirstColumn+nDelCount-1;i>=nFirstColumn;i--)
			{
				this.columnsByName.Remove(this[i].ColumnName);
				this[i].Dispose();
			}
			this.columnsByNumber.RemoveRange(nFirstColumn, nDelCount);
			
			// renumber the remaining columns
			for(int i=columnsByNumber.Count-1;i>=nFirstColumn;i--)
				this[i].SetColumnNumber(i);

			// raise datachange event that some columns have changed
			if(nFirstColumn<nMinColChanged) nMinColChanged=nFirstColumn;
			if(nOriginalColumnCount>nMaxColChanged) nMaxColChanged=nOriginalColumnCount;
			if(0<nMinRowChanged) nMinRowChanged=0;
			if(RowCount>nMaxRowChanged) nMaxRowChanged=RowCount;
			nMaxDecreasedToRow = 0; // because in the worst case, the row count can now be null if all other columns are empty
			OnColumnDataChanged(null,0,RowCount,true);
		}

		public void RemoveColumn(int nFirstColumn)
		{
			RemoveColumns(nFirstColumn,1);
		}

		public void DeleteRows(int nFirstRow, int nDelCount)
		{
			this.SuspendDataChangedNotifications();
			
			for(int i=this.ColumnCount-1;i>=0;i--)
			{
				this[i].RemoveRows(nFirstRow,nDelCount);
			}

			this.ResumeDataChangedNotifications();
		}

		public void DeleteRow(int nFirstRow)
		{
			DeleteRows(nFirstRow,1);
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
		public string Transpose()
		{
			int firstdifferent;

			if(!AreAllColumnsOfTheSameType(out firstdifferent))
			{
				return String.Format("Column[{0}] ({1}) has a different type than the first column transpose is not possible!",firstdifferent,this[firstdifferent].ColumnName);
			}

			// now we can start by adding additional columns of the row count is greater
			// than the column count
			this.SuspendDataChangedNotifications();
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

			this.ResumeDataChangedNotifications();


			return null; // no error message
		}


		public ColumnScriptCollection ColumnScripts
		{
			get { return this.m_ColumnScripts; }
		}



	} // end class Altaxo.Data.DataTable
	
}
