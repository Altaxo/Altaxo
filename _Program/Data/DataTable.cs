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
	[SerializationVersion(0)]
	public class DataTable : DataColumnCollection, System.Runtime.Serialization.IDeserializationCallback, ICloneable
		{
		// Types
		
		// Data
		/// <summary>
		/// The parent data set this table is belonging to.
		/// </summary>
		protected DataSet m_ParentDataSet=null; // the dataSet that this table is belonging to
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

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Altaxo.Data.DataTable),0)]
		public new class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo	info)
			{
				Altaxo.Data.DataTable s = (Altaxo.Data.DataTable)obj;
				info.AddValue("Name",s.m_TableName); // name of the Table
				info.AddBaseValueStandalone("DataCols",s,typeof(Altaxo.Data.DataColumnCollection));
				info.AddValue("PropCols", s.m_PropertyColumns); // the property columns of that table

			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo	info, object parent)
			{
				Altaxo.Data.DataTable s = null!=o ? (Altaxo.Data.DataTable)o : new Altaxo.Data.DataTable();
	
				s.m_TableName = info.GetString("Name");
				info.GetBaseValueStandalone("DataCols",s,typeof(Altaxo.Data.DataColumnCollection),s);
				s.m_PropertyColumns = (DataColumnCollection)info.GetValue(s);

				return s;
			}
		}

		public override void OnDeserialization(object obj)
		{
			base.Parent = this;
			base.OnDeserialization(obj);

			if(!m_Table_DeserializationFinished && obj is DeserializationFinisher)
			{
				m_Table_DeserializationFinished = true;
				// set the parent data table of the data column collection

				// now inform the dependent objects
				DeserializationFinisher finisher = new DeserializationFinisher(this);
				this.m_PropertyColumns.Parent = this;
				this.m_PropertyColumns.OnDeserialization(finisher);
			}
		}

		#endregion

		public DataTable()
			: base()
		{
			base.Parent = this;
			this.m_TableName = null;
			m_PropertyColumns.Parent = this;
		}

		public DataTable(string name)
			: base()
		{
			base.Parent = this;
			this.m_TableName = name;
			m_PropertyColumns.Parent = this;
		}

		public DataTable(Altaxo.Data.DataSet parent) : base()
		{
			base.Parent = this;
			this.m_ParentDataSet = parent;
			m_PropertyColumns.Parent = this;
		}

		public DataTable(Altaxo.Data.DataSet parent, string name) : base()
		{
			base.Parent = this;
			this.m_ParentDataSet = parent;
			this.m_TableName = name;
			m_PropertyColumns.Parent = this;
		}
  
		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="from">The data table to copy the structure from.</param>
		public DataTable(DataTable from)
			: base(from)
		{
			base.Parent = this;
			this.m_ParentDataSet = null; 
			this.m_TableName = from.m_TableName;
			this.m_PropertyColumns = (DataColumnCollection)from.m_PropertyColumns.Clone();
			this.m_PropertyColumns.Parent = this; // set the parent of the cloned PropertyColumns
		}

	
		public override object Clone()
		{
			return new DataTable(this);
		}

		public Altaxo.Data.DataSet ParentDataSet
		{
			get { return m_ParentDataSet; }
			set { m_ParentDataSet = value; }
		}


/// <summary>
/// get or sets the name of the Table
/// </summary>
		public new string TableName
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
							throw(new AltaxoUniqueNameException());
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
		




		public override void SuspendDataChangedNotifications()
		{
			base.SuspendDataChangedNotifications();
			m_PropertyColumns.SuspendDataChangedNotifications();
		}

		public override void ResumeDataChangedNotifications()
		{
			base.ResumeDataChangedNotifications();
			m_PropertyColumns.ResumeDataChangedNotifications();
		}



		public void OnColumnCollectionDataChanged(Altaxo.Data.DataColumnCollection sender)
		{
			if(null!=m_ParentDataSet)
				m_ParentDataSet.OnTableDataChanged(this);
		}


			public override bool IsDirty
			{
				get
				{
					return base.IsDirty | m_PropertyColumns.IsDirty;
				}
				set
				{
					base.IsDirty = value;
					m_PropertyColumns.IsDirty = value;
				}
			}


		public override void Add(int idx, Altaxo.Data.DataColumn datac)
		{
			SuspendDataChangedNotifications();
			
			base.Add(idx,datac); // add the column to the collection
			m_PropertyColumns.InsertRows(idx,1); // but now we have to insert a additional property row at exactly the new position of the column

			ResumeDataChangedNotifications();
		}


		public override void RemoveColumns(int nFirstColumn, int nDelCount)
		{
	
			SuspendDataChangedNotifications();
			
			base.RemoveColumns(nFirstColumn, nDelCount); // remove the columns from the collection
			m_PropertyColumns.RemoveRows(nFirstColumn, nDelCount); // remove also the corresponding rows from the Properties

			ResumeDataChangedNotifications();
		}
	} // end class Altaxo.Data.DataTable
	
}
