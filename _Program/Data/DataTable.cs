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

	/// <summary>DataTable is the central class of Altaxo, which holds the data organized in columns.</summary>
	/// <remarks>In contrast to common database
	/// programs, the data are not organized in rows, but in (relatively independent) columns. As in database programs,
	/// each column has a certain type, as <see cref="TextColumn"/> for holding strings, 
	/// <see cref="DoubleColumn"/> for storing numeric values, and <see cref="DateTimeColumn"/> for holding DateTimes.
	/// All these column types are derived from the base class <see cref="DataColumn"/>.<para/>
	/// There is also a similar concept like metadata in database programs: Each column can have some property values associated with. The property values
	/// are organized in property columns and can be retrieved by the <see cref="DataTable.PropCols"/> property of the table.</remarks>
	[SerializationSurrogate(0,typeof(Altaxo.Data.DataTable.SerializationSurrogate0))]
	[SerializationSurrogate(1,typeof(Altaxo.Data.DataTable.SerializationSurrogate1))]
	[SerializationVersion(1)]
	public class DataTable 
		:
		System.Runtime.Serialization.IDeserializationCallback, 
		ICloneable,
		Altaxo.Main.IDocumentNode,
		IDisposable,
		Main.INamedObjectCollection,
		Main.INameOwner
		{
		// Types
		
		// Data
		/// <summary>
		/// The parent data set this table is belonging to.
		/// </summary>
		protected TableSet m_ParentDataSet=null; // the dataSet that this table is belonging to
		/// <summary>
		/// The name of this table, has to be unique if there is a parent data set, since the tables in the parent data set
		/// can only be accessed by name.
		/// </summary>
		protected string m_TableName=null; // the name of the table

		/// <summary>
		/// Collection of property columns, i.e. "horizontal" columns.
		/// </summary>
		/// <remarks>Property columns can be used to give columns a certain property. This can be for instance the unit of the column or a
		/// descriptive name (the property column is then of type TextColumn).
		/// This can also be another parameter which corresponds with that column, i.e. frequency. In this case the property column would be of
		/// type DoubleColumn.</remarks>
		protected DataColumnCollection m_PropertyColumns = new DataColumnCollection();
		

		protected DataColumnCollection m_DataColumns = new DataColumnCollection();

		// Helper Data

		/// <summary>
		/// Used to indicate that the Deserialization process has finished.
		/// </summary>
		private bool  m_Table_DeserializationFinished=false;

		public event System.EventHandler TableNameChanged;


		#region "Serialization"
		public new class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{
				Altaxo.Data.DataTable s = (Altaxo.Data.DataTable)obj;
				System.Runtime.Serialization.ISurrogateSelector ss = AltaxoStreamingContext.GetSurrogateSelector(context);
				if(null!=ss)
				{
					System.Runtime.Serialization.ISerializationSurrogate surr =
						ss.GetSurrogate(typeof(Altaxo.Data.DataColumnCollection),context, out ss);
					surr.GetObjectData(obj,info,context); // stream the data of the base object
				}
				else 
				{
					throw new NotImplementedException(string.Format("Serializing a {0} without surrogate not implemented yet!",obj.GetType()));
				}

				info.AddValue("Name",s.m_TableName); // name of the Table
				info.AddValue("PropCols", s.m_PropertyColumns); // the property columns of that table

			}
			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
			{
				Altaxo.Data.DataTable s = (Altaxo.Data.DataTable)obj;
				System.Runtime.Serialization.ISurrogateSelector ss = AltaxoStreamingContext.GetSurrogateSelector(context);
				if(null!=ss)
				{
				System.Runtime.Serialization.ISerializationSurrogate surr =
					ss.GetSurrogate(typeof(Altaxo.Data.DataColumnCollection),context, out ss);
				surr.SetObjectData(obj,info,context,selector);
				}
				else 
				{
					throw new NotImplementedException(string.Format("Serializing a {0} without surrogate not implemented yet!",obj.GetType()));
				}

				s.m_TableName = info.GetString("Name");
				s.m_PropertyColumns = (DataColumnCollection)info.GetValue("PropCols",typeof(DataColumnCollection));

				return s;
			}
		}

		public new class SerializationSurrogate1 : System.Runtime.Serialization.ISerializationSurrogate
		{
			public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{
				Altaxo.Data.DataTable s = (Altaxo.Data.DataTable)obj;
	
				info.AddValue("Name",s.m_TableName); // name of the Table
				info.AddValue("DataCols", s.DataColumns); // the data columns of that table
				info.AddValue("PropCols", s.m_PropertyColumns); // the property columns of that table

			}
			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
			{
				Altaxo.Data.DataTable s = (Altaxo.Data.DataTable)obj;

				s.m_TableName = info.GetString("Name");
				s.m_DataColumns = (DataColumnCollection)info.GetValue("DataCols",typeof(DataColumnCollection));
				s.m_PropertyColumns = (DataColumnCollection)info.GetValue("PropCols",typeof(DataColumnCollection));

				return s;
			}
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Altaxo.Data.DataTable),0)]
		public new class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo	info)
			{
				Altaxo.Data.DataTable s = (Altaxo.Data.DataTable)obj;
				info.AddValue("Name",s.m_TableName); // name of the Table
				info.AddValue("DataCols",s.m_DataColumns);
				info.AddValue("PropCols", s.m_PropertyColumns); // the property columns of that table

			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo	info, object parent)
			{
				Altaxo.Data.DataTable s = null!=o ? (Altaxo.Data.DataTable)o : new Altaxo.Data.DataTable();
	
				

				s.m_TableName = info.GetString("Name");
				s.m_DataColumns = (DataColumnCollection)info.GetValue("DataCols",s);
				s.m_PropertyColumns = (DataColumnCollection)info.GetValue("PropCols",s);

				return s;
			}
		}

		public virtual void OnDeserialization(object obj)
		{
			//base.Parent = this;
			//base.OnDeserialization(obj);

			if(!m_Table_DeserializationFinished && obj is DeserializationFinisher)
			{
				m_Table_DeserializationFinished = true;
				// set the parent data table of the data column collection

				// now inform the dependent objects
				DeserializationFinisher finisher = new DeserializationFinisher(this);
				this.m_DataColumns.Parent = this;
				this.m_DataColumns.OnDeserialization(finisher);
				this.m_PropertyColumns.Parent = this;
				this.m_PropertyColumns.OnDeserialization(finisher);
			}
		}

		#endregion

		public DataTable()
			: base()
		{
			this.m_TableName = null;
			// base.Parent = this;
			m_DataColumns.Parent = this;
			m_PropertyColumns.Parent = this;
		}

		public DataTable(string name)
			: base()
		{
		
			this.m_TableName = name;
			// 	base.Parent = this;
			m_DataColumns.Parent = this;
			m_PropertyColumns.Parent = this;
		}

		public DataTable(Altaxo.Data.TableSet parent) : base()
		{
			
			this.m_ParentDataSet = parent;
			// 	base.Parent = this;
			m_DataColumns.Parent = this;
			m_PropertyColumns.Parent = this;
		}

		public DataTable(Altaxo.Data.TableSet parent, string name) : base()
		{
			
			this.m_ParentDataSet = parent;
			this.m_TableName = name;
			// 	base.Parent = this;
			m_DataColumns.Parent = this;
			m_PropertyColumns.Parent = this;
		}
  
		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="from">The data table to copy the structure from.</param>
		public DataTable(DataTable from)
		{
			
			this.m_ParentDataSet = null; 
			this.m_TableName = from.m_TableName;
			// 	base.Parent = this;
			this.m_DataColumns = (DataColumnCollection)from.m_DataColumns.Clone();
			m_DataColumns.Parent = this;

			this.m_PropertyColumns = (DataColumnCollection)from.m_PropertyColumns.Clone();
			this.m_PropertyColumns.Parent = this; // set the parent of the cloned PropertyColumns
		}

	
		public virtual object Clone()
		{
			return new DataTable(this);
		}

		public Altaxo.Data.TableSet ParentDataSet
		{
			get { return m_ParentDataSet; }
			set { m_ParentDataSet = value; }
		}

		public virtual object ParentObject
		{
			get { return m_ParentDataSet; }
		}

		public virtual string Name
		{
			get { return m_TableName; }
		}

/// <summary>
/// get or sets the name of the Table
/// </summary>
		public string TableName
		{
			get
			{
				return m_TableName;
			}
			set
			{
				if(m_TableName==null || m_TableName!=value)
				{
					if(null!=ParentDataSet)
					{
						if(null!=ParentDataSet[value])
						{
							throw new AltaxoUniqueNameException();
						}
					}
					m_TableName = value;
					if(null!=TableNameChanged)
						TableNameChanged(this,new System.EventArgs());
				}
			}
		}


		/// <summary>
		/// Returns the property collection of the table.
		/// </summary>
		/// <remarks>To get a certain property value for a certain data column of the table,
		/// use PropCols["propertyname", datacolumnnumber], where propertyname is the name of the property to retrieve and
		/// columnnumber is the number of the data column for which the property should be retrieved. Unfortunately you can not reference
		/// the data column here by name :-(, you have to know the number. Alternatively, you can reference the property (!) not by name, but
		/// by number by using PropCols[propertycolumnnumber, datacolumnnumber]. If you only have
		/// the data columns name, use PropCols("propertyname",this["datacolumsname"].Number] instead.
		/// </remarks>
		public DataColumnCollection PropCols
		{
			get { return m_PropertyColumns; }
		}
		


		public DataColumnCollection Col
		{
			get { return m_DataColumns; }
		}
		public DataColumnCollection DataColumns
		{
			get { return m_DataColumns; }
		}

		public DataColumn this[int i]
		{
			get { return m_DataColumns[i]; }
			set { m_DataColumns[i]=value; }
		}
		public DataColumn this[string name]
		{
			get { return m_DataColumns[name]; }
			set { m_DataColumns[name]=value; }
		}


		public virtual void SuspendDataChangedNotifications()
		{
			m_DataColumns.SuspendDataChangedNotifications();
			m_PropertyColumns.SuspendDataChangedNotifications();
		}

		public virtual void ResumeDataChangedNotifications()
		{
			m_DataColumns.ResumeDataChangedNotifications();
			m_PropertyColumns.ResumeDataChangedNotifications();
		}



		public void OnColumnCollectionDataChanged(Altaxo.Data.DataColumnCollection sender)
		{
			if(null!=m_ParentDataSet)
				m_ParentDataSet.OnTableDataChanged(this);
		}


		/// <summary>
		/// Transpose transpose the table, i.e. exchange columns and rows
		/// this can only work if all columns in the table are of the same type
		/// </summary>
		/// <returns>null if succeeded, error string otherwise</returns>
		public virtual string Transpose()
		{
			// TODO: do also look at the property columns for transposing
			m_DataColumns.Transpose();

			return null; // no error message
		}


			public virtual bool IsDirty
			{
				get
				{
					return m_DataColumns.IsDirty | m_PropertyColumns.IsDirty;
				}
				set
				{
					m_DataColumns.IsDirty = value;
					m_PropertyColumns.IsDirty = value;
				}
			}


		public virtual void Add(int idx, Altaxo.Data.DataColumn datac)
		{
			SuspendDataChangedNotifications();
			
			m_DataColumns.Add(idx,datac); // add the column to the collection
			m_PropertyColumns.InsertRows(idx,1); // but now we have to insert a additional property row at exactly the new position of the column

			ResumeDataChangedNotifications();
		}


		public virtual void RemoveColumns(int nFirstColumn, int nDelCount)
		{
	
			SuspendDataChangedNotifications();
			
			m_DataColumns.RemoveColumns(nFirstColumn, nDelCount); // remove the columns from the collection
			m_PropertyColumns.RemoveRows(nFirstColumn, nDelCount); // remove also the corresponding rows from the Properties

			ResumeDataChangedNotifications();
		}
		#region IDisposable Members

		public void Dispose()
		{
			m_DataColumns.Dispose();
			m_PropertyColumns.Dispose();
		}

		#endregion

		/// <summary>
		/// retrieves the object with the name <code>name</code>.
		/// </summary>
		/// <param name="name">The objects name.</param>
		/// <returns>The object with the specified name.</returns>
		public object GetChildObjectNamed(string name)
		{
			switch(name)
			{
				case "DataCols":
					return m_DataColumns;
				case "PropCols":
					return this.m_PropertyColumns;
			}
		return null;
		}

		/// <summary>
		/// Retrieves the name of the provided object.
		/// </summary>
		/// <param name="o">The object for which the name should be found.</param>
		/// <returns>The name of the object. Null if the object is not found. String.Empty if the object is found but has no name.</returns>
		public string GetNameOfChildObject(object o)
		{
			if(o==null)
				return null;
			else if(o.Equals(m_DataColumns))
				return "DataCols";
			else if(o.Equals(m_PropertyColumns))
				return "PropCols";
			else
				return null;
		}
	} // end class Altaxo.Data.DataTable
	
}
