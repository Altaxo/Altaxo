#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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

using Altaxo.Collections;
using Altaxo.Scripting;
using Altaxo.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

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
	[SerializationSurrogate(0, typeof(Altaxo.Data.DataTable.SerializationSurrogate0))]
	[SerializationSurrogate(1, typeof(Altaxo.Data.DataTable.SerializationSurrogate1))]
	[SerializationVersion(1)]
	public class DataTable
		:
		Main.SuspendableDocumentNodeWithSetOfEventArgs,
		System.Runtime.Serialization.IDeserializationCallback,
		Main.IProjectItem,
		Main.Properties.IPropertyBagOwner
	{
		#region Members

		/// <summary>
		/// The name of this table, has to be unique if there is a parent data set, since the tables in the parent data set
		/// can only be accessed by name.
		/// </summary>
		protected string _tableName = null; // the name of the table

		/// <summary>
		/// Collection of property columns, i.e. "horizontal" columns.
		/// </summary>
		/// <remarks>Property columns can be used to give columns a certain property. This can be for instance the unit of the column or a
		/// descriptive name (the property column is then of type TextColumn).
		/// This can also be another parameter which corresponds with that column, i.e. frequency. In this case the property column would be of
		/// type DoubleColumn.</remarks>
		protected DataColumnCollection _propertyColumns;

		/// <summary>
		/// Collection of data columns, i.e. the normal, "vertical" columns.
		/// </summary>
		protected DataColumnCollection _dataColumns;

		/// <summary>
		/// The date/time of creation of this table.
		/// </summary>
		protected DateTime _creationTime;

		/// <summary>
		/// The date/time when this table was changed.
		/// </summary>
		protected DateTime _lastChangeTime;

		/// <summary>
		/// Notes concerning this table.
		/// </summary>
		protected Main.TextBackedConsole _notes;

		/// <summary>
		/// The table script that belongs to this table.
		/// </summary>
		protected TableScript _tableScript;

		/// <summary>
		/// Designates the source of the data the table was originally filled with.
		/// </summary>
		protected IAltaxoTableDataSource _tableDataSource;

		/// <summary>
		/// The table properties, key is a string, value is a property you want to store here.
		/// </summary>
		/// <remarks>The properties are saved on disc (with exception of those who starts with "tmp/".
		/// If the property you want to store is only temporary, the properties name should therefore
		/// start with "tmp/".</remarks>
		protected Main.Properties.PropertyBag _tableProperties;

		// Helper Data

		/// <summary>
		/// Used to indicate that the Deserialization process has finished.
		/// </summary>
		private bool _table_DeserializationFinished = false;

		/// <summary>
		/// Event to signal that the parent of this object has changed.
		/// </summary>
		[field: NonSerialized]
		public event Main.ParentChangedEventHandler ParentChanged;

		/// <summary>
		/// Event to signal that the name of this object has changed.
		/// </summary>
		[field: NonSerialized]
		public event Action<Main.INameOwner, string> NameChanged;

		/// <summary>
		/// Fired before the name of this table is changed.
		/// </summary>
		[field: NonSerialized]
		public event Action<Main.INameOwner, string, System.ComponentModel.CancelEventArgs> PreviewNameChange;

		#endregion Members

		#region Serialization

		public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			public void GetObjectData(object obj, System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			{
				Altaxo.Data.DataTable s = (Altaxo.Data.DataTable)obj;
				System.Runtime.Serialization.ISurrogateSelector ss = AltaxoStreamingContext.GetSurrogateSelector(context);
				if (null != ss)
				{
					System.Runtime.Serialization.ISerializationSurrogate surr =
						ss.GetSurrogate(typeof(Altaxo.Data.DataColumnCollection), context, out ss);
					surr.GetObjectData(obj, info, context); // stream the data of the base object
				}
				else
				{
					throw new NotImplementedException(string.Format("Serializing a {0} without surrogate not implemented yet!", obj.GetType()));
				}

				info.AddValue("Name", s._tableName); // name of the Table
				info.AddValue("PropCols", s._propertyColumns); // the property columns of that table
			}

			public object SetObjectData(object obj, System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context, System.Runtime.Serialization.ISurrogateSelector selector)
			{
				Altaxo.Data.DataTable s = (Altaxo.Data.DataTable)obj;
				System.Runtime.Serialization.ISurrogateSelector ss = AltaxoStreamingContext.GetSurrogateSelector(context);
				if (null != ss)
				{
					System.Runtime.Serialization.ISerializationSurrogate surr =
						ss.GetSurrogate(typeof(Altaxo.Data.DataColumnCollection), context, out ss);
					surr.SetObjectData(obj, info, context, selector);
				}
				else
				{
					throw new NotImplementedException(string.Format("Serializing a {0} without surrogate not implemented yet!", obj.GetType()));
				}

				s._tableName = info.GetString("Name");
				s._propertyColumns = (DataColumnCollection)info.GetValue("PropCols", typeof(DataColumnCollection));

				return s;
			}
		}

		public class SerializationSurrogate1 : System.Runtime.Serialization.ISerializationSurrogate
		{
			public void GetObjectData(object obj, System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			{
				Altaxo.Data.DataTable s = (Altaxo.Data.DataTable)obj;

				info.AddValue("Name", s._tableName); // name of the Table
				info.AddValue("DataCols", s.DataColumns); // the data columns of that table
				info.AddValue("PropCols", s._propertyColumns); // the property columns of that table
			}

			public object SetObjectData(object obj, System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context, System.Runtime.Serialization.ISurrogateSelector selector)
			{
				Altaxo.Data.DataTable s = (Altaxo.Data.DataTable)obj;

				s._tableName = info.GetString("Name");
				s._dataColumns = (DataColumnCollection)info.GetValue("DataCols", typeof(DataColumnCollection));
				s._propertyColumns = (DataColumnCollection)info.GetValue("PropCols", typeof(DataColumnCollection));

				return s;
			}
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Data.DataTable", 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				Altaxo.Data.DataTable s = (Altaxo.Data.DataTable)obj;
				info.AddValue("Name", s._tableName); // name of the Table
				info.AddValue("DataCols", s._dataColumns);
				info.AddValue("PropCols", s._propertyColumns); // the property columns of that table
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (Altaxo.Data.DataTable)o ?? new Altaxo.Data.DataTable(info);

				s._tableName = info.GetString("Name");
				s._dataColumns = (DataColumnCollection)info.GetValue("DataCols", s);
				s._dataColumns.ParentObject = s;
				s._propertyColumns = (DataColumnCollection)info.GetValue("PropCols", s);
				s._propertyColumns.ParentObject = s;

				return s;
			}
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Altaxo.Data.DataTable), 1)]
		private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				Altaxo.Data.DataTable s = (Altaxo.Data.DataTable)obj;
				info.AddValue("Name", s._tableName); // name of the Table
				info.AddValue("DataCols", s._dataColumns);
				info.AddValue("PropCols", s._propertyColumns); // the property columns of that table

				// new in version 1
				info.AddValue("TableScript", s._tableScript);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (Altaxo.Data.DataTable)o ?? new Altaxo.Data.DataTable(info);

				s._tableName = info.GetString("Name");
				s._dataColumns = (DataColumnCollection)info.GetValue("DataCols", s);
				s._dataColumns.ParentObject = s;
				s._propertyColumns = (DataColumnCollection)info.GetValue("PropCols", s);
				s._propertyColumns.ParentObject = s;

				// new in version 1
				s._tableScript = (TableScript)info.GetValue("TableScript", s);
				return s;
			}
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Data.DataTable", 2)]
		private class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new InvalidOperationException("Try to serialize old version");
				/*
				Altaxo.Data.DataTable s = (Altaxo.Data.DataTable)obj;
				info.AddValue("Name", s._tableName); // name of the Table
				info.AddValue("DataCols", s._dataColumns);
				info.AddValue("PropCols", s._propertyColumns); // the property columns of that table

				// new in version 1
				info.AddValue("TableScript", s._tableScript);

				// new in version 2 - Add table properties
				int numberproperties = s._tableProperties == null ? 0 : s._tableProperties.Count;
				info.CreateArray("TableProperties", numberproperties);
				if (s._tableProperties != null)
				{
					foreach (string propkey in s._tableProperties.Keys)
					{
						if (propkey.StartsWith("tmp/"))
							continue;
						info.CreateElement("e");
						info.AddValue("Key", propkey);
						object val = s._tableProperties[propkey];
						info.AddValue("Value", info.IsSerializable(val) ? val : null);
						info.CommitElement();
					}
				}
				info.CommitArray();
				*/
			}

			public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (Altaxo.Data.DataTable)o ?? new Altaxo.Data.DataTable(info);
				Deserialize(s, info, parent);
				return s;
			}

			public virtual void Deserialize(DataTable s, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				s._tableName = info.GetString("Name");
				s._dataColumns = (DataColumnCollection)info.GetValue("DataCols", s);
				s._dataColumns.ParentObject = s;
				s._propertyColumns = (DataColumnCollection)info.GetValue("PropCols", s);
				s._propertyColumns.ParentObject = s;

				// new in version 1
				s._tableScript = (TableScript)info.GetValue("TableScript", s);

				// new in version 2 - Add table properties
				int numberproperties = info.OpenArray(); // "TableProperties"
				for (int i = 0; i < numberproperties; i++)
				{
					info.OpenElement(); // "e"
					string propkey = info.GetString("Key");
					object propval = info.GetValue("Value", parent);
					info.CloseElement(); // "e"
					s.SetTableProperty(propkey, propval);
				}
				info.CloseArray(numberproperties);
			}
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Data.DataTable", 3)]
		private class XmlSerializationSurrogate3 : XmlSerializationSurrogate2
		{
			public override void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				base.Serialize(obj, info);
				DataTable s = (DataTable)obj;
				info.AddValue("Notes", s._notes.Text);
				info.AddValue("CreationTime", s._creationTime.ToLocalTime());
				info.AddValue("LastChangeTime", s._lastChangeTime.ToLocalTime());
			}

			public override void Deserialize(DataTable s, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				base.Deserialize(s, info, parent);
				s._notes.Text = info.GetString("Notes");
				s._creationTime = info.GetDateTime("CreationTime").ToUniversalTime();
				s._lastChangeTime = info.GetDateTime("LastChangeTime").ToUniversalTime();
			}
		}

		/// <summary>
		/// 2014-01-30 Table properties are now in it's own class
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Altaxo.Data.DataTable), 4)]
		private class XmlSerializationSurrogate4 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				Altaxo.Data.DataTable s = (Altaxo.Data.DataTable)obj;
				info.AddValue("Name", s._tableName); // name of the Table

				string originalSaveAsTemplateOption = null;
				bool saveDataAsTemplateRequired = null != s._tableDataSource && s._tableDataSource.ImportOptions.DoNotSaveCachedTableData;
				if (saveDataAsTemplateRequired)
				{
					originalSaveAsTemplateOption = info.GetProperty("Altaxo.Data.DataColumn.SaveAsTemplate");
					info.SetProperty("Altaxo.Data.DataColumn.SaveAsTemplate", "true");
				}

				info.AddValue("DataCols", s._dataColumns);

				if (saveDataAsTemplateRequired)
				{
					info.SetProperty("Altaxo.Data.DataColumn.SaveAsTemplate", originalSaveAsTemplateOption);
				}

				info.AddValue("PropCols", s._propertyColumns); // the property columns of that table
				info.AddValue("TableScript", s._tableScript);
				info.AddValue("Properties", s._tableProperties);
				info.AddValue("Notes", s._notes.Text);
				info.AddValue("CreationTime", s._creationTime.ToLocalTime());
				info.AddValue("LastChangeTime", s._lastChangeTime.ToLocalTime());
				if (null != s._tableDataSource)
					info.AddValue("TableDataSource", s._tableDataSource);
			}

			public virtual void Deserialize(DataTable s, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				s._tableName = info.GetString("Name");
				s._dataColumns = (DataColumnCollection)info.GetValue("DataCols", s);
				s._dataColumns.ParentObject = s;
				s._propertyColumns = (DataColumnCollection)info.GetValue("PropCols", s);
				s._propertyColumns.ParentObject = s;
				s._tableScript = (TableScript)info.GetValue("TableScript", s);
				s.PropertyBag = (Main.Properties.PropertyBag)info.GetValue("Properties");

				s._notes.Text = info.GetString("Notes");
				s._creationTime = info.GetDateTime("CreationTime").ToUniversalTime();
				s._lastChangeTime = info.GetDateTime("LastChangeTime").ToUniversalTime();
				if (info.CurrentElementName == "TableDataSource")
				{
					s._tableDataSource = (IAltaxoTableDataSource)info.GetValue("TableDataSource");
					if (null != s._tableDataSource)
					{
						s._tableDataSource.ParentObject = s;
						s._tableDataSource.OnAfterDeserialization();
					}
				}
			}

			public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (Altaxo.Data.DataTable)o ?? new Altaxo.Data.DataTable(info);
				Deserialize(s, info, parent);
				return s;
			}
		}

		public virtual void OnDeserialization(object obj)
		{
			//base.Parent = this;
			//base.OnDeserialization(obj);

			if (!_table_DeserializationFinished && obj is DeserializationFinisher)
			{
				_table_DeserializationFinished = true;
				// set the parent data table of the data column collection

				// now inform the dependent objects
				DeserializationFinisher finisher = new DeserializationFinisher(this);
				this._dataColumns.ParentObject = this;
				this._dataColumns.OnDeserialization(finisher);
				this._propertyColumns.ParentObject = this;
				this._propertyColumns.OnDeserialization(finisher);
			}
		}

		/// <summary>
		/// This class is responsible for the special purpose to serialize a data table for clipboard. Do not use
		/// it for permanent serialization purposes, since it does not contain version handling.
		/// </summary>
		[Serializable]
		public class ClipboardMemento : System.Runtime.Serialization.ISerializable
		{
			private DataTable _table;
			private IAscendingIntegerCollection _selectedDataColumns;
			private IAscendingIntegerCollection _selectedDataRows;
			private IAscendingIntegerCollection _selectedPropertyColumns;
			private IAscendingIntegerCollection _selectedPropertyRows;

			/// <summary>
			/// Constructor. Besides the table, the current selections must be provided. Only the areas that corresponds to the selections are
			/// serialized. The serialization process has to occur immediately after this constructor, because only a reference
			/// to the table is hold by this object.
			/// </summary>
			/// <param name="table">The table to serialize.</param>
			/// <param name="selectedDataColumns">The selected data columns.</param>
			/// <param name="selectedDataRows">The selected data rows.</param>
			/// <param name="selectedPropertyColumns">The selected property columns.</param>
			/// <param name="selectedPropertyRows">The selected property rows.</param>
			public ClipboardMemento(DataTable table, IAscendingIntegerCollection selectedDataColumns,
				IAscendingIntegerCollection selectedDataRows,
				IAscendingIntegerCollection selectedPropertyColumns,
				IAscendingIntegerCollection selectedPropertyRows
				)
			{
				this._table = table;
				this._selectedDataColumns = selectedDataColumns;
				this._selectedDataRows = selectedDataRows;
				this._selectedPropertyColumns = selectedPropertyColumns;
				this._selectedPropertyRows = selectedPropertyRows;
			}

			/// <summary>
			/// Returns the (deserialized) table.
			/// </summary>
			public DataTable DataTable
			{
				get { return _table; }
			}

			#region ISerializable Members

			public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			{
				info.AddValue("Name", _table.Name);

				// special case: if no data cell is selected, then serialize the property data as when they where data columns
				if (_selectedDataColumns.Count == 0 && _selectedDataRows.Count == 0)
				{
					// exchange data and properties
					info.AddValue("DataColumns", new DataColumnCollection.ClipboardMemento(_table.PropCols, _selectedPropertyColumns, _selectedPropertyRows, true));
					info.AddValue("PropertyColumns", new DataColumnCollection.ClipboardMemento(_table.DataColumns, _selectedDataColumns, _selectedDataRows, true));
				}
				else
				{ // normal serialization of data and properties
					info.AddValue("DataColumns", new DataColumnCollection.ClipboardMemento(_table.DataColumns, _selectedDataColumns, _selectedDataRows, true));
					info.AddValue("PropertyColumns", new DataColumnCollection.ClipboardMemento(_table.PropCols, _selectedPropertyColumns, _selectedDataColumns, true));
				}
			}

			public ClipboardMemento(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			{
				_table = new DataTable();
				_table.Name = info.GetString("Name");
				DataColumnCollection.ClipboardMemento datacolMemento = (DataColumnCollection.ClipboardMemento)info.GetValue("DataColumns", typeof(DataColumnCollection.ClipboardMemento));
				_table._dataColumns = datacolMemento.Collection;
				DataColumnCollection.ClipboardMemento propcolMemento = (DataColumnCollection.ClipboardMemento)info.GetValue("PropertyColumns", typeof(DataColumnCollection.ClipboardMemento));
				_table._propertyColumns = propcolMemento.Collection;
			}

			#endregion ISerializable Members
		}

		#endregion Serialization

		#region Construction

		/// <summary>
		/// Constructs an empty data table.
		/// </summary>
		public DataTable()
			: this(new DataColumnCollection(), new DataColumnCollection())
		{
		}

		/// <summary>
		/// Constructs an empty data table with the name provided by the argiment.
		/// </summary>
		/// <param name="name">The initial name of the table.</param>
		public DataTable(string name)
			: this()
		{
			this._tableName = name;
		}

		/// <summary>
		/// Constructs an empty table with the parent provided by the argument.
		/// </summary>
		/// <param name="parent">The initial parent of the table.</param>
		public DataTable(Altaxo.Data.DataTableCollection parent)
			: this()
		{
			this._parent = parent;
		}

		/// <summary>
		/// Constructs an empty table with the parent and the name provided by the argument.
		/// </summary>
		/// <param name="parent">The initial parent of the table.</param>
		/// <param name="name">The initial name of the table.</param>
		public DataTable(Altaxo.Data.DataTableCollection parent, string name)
			: this(name)
		{
			this._parent = parent;
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="from">The data table to copy the structure from.</param>
		public DataTable(DataTable from)
			: this((DataColumnCollection)from._dataColumns.Clone(), (DataColumnCollection)from._propertyColumns.Clone())
		{
			this._parent = null; // do not clone the parent
			this._tableName = from._tableName;
			this._tableScript = null == from._tableScript ? null : (TableScript)from._tableScript.Clone();
			this._creationTime = this._lastChangeTime = DateTime.UtcNow;
			this._notes = from._notes.Clone();
			this._notes.PropertyChanged += this.EhNotesChanged;

			// Clone also the table properties (deep copy)
			if (from._tableProperties != null && from._tableProperties.Count > 0)
			{
				PropertyBagNotNull.CopyFrom(from._tableProperties);
			}
			else
			{
				this._tableProperties = null;
			}

			this.DataSource = null == from.DataSource ? null : (IAltaxoTableDataSource)from.DataSource.Clone();
		}

		/// <summary>
		/// Constructor for internal use only. Takes the two DataColumnCollections as Data and Properties. These collections are used directly (not by cloning them).
		/// </summary>
		/// <param name="datacoll">The data columns.</param>
		/// <param name="propcoll">The property columns.</param>
		protected DataTable(DataColumnCollection datacoll, DataColumnCollection propcoll)
		{
			this._dataColumns = datacoll;
			_dataColumns.ParentObject = this;
			_dataColumns.ParentChanged += new Main.ParentChangedEventHandler(this.EhChildParentChanged);

			this._propertyColumns = propcoll;
			this._propertyColumns.ParentObject = this; // set the parent of the cloned PropertyColumns
			_propertyColumns.ParentChanged += new Main.ParentChangedEventHandler(this.EhChildParentChanged);
			_creationTime = _lastChangeTime = DateTime.UtcNow;
			_notes = new Main.TextBackedConsole();
			_notes.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(EhNotesChanged);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataTable"/> class for deserialization purposes only.
		/// </summary>
		/// <param name="info">The information.</param>
		protected DataTable(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
		{
			_notes = new Main.TextBackedConsole();
			_notes.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(EhNotesChanged);
		}

		/// <summary>
		/// Clones the table.
		/// </summary>
		/// <returns>A cloned version of this table. All data inside the table are cloned too (deep copy).</returns>
		public virtual object Clone()
		{
			return new DataTable(this);
		}

		#endregion Construction

		#region Suspend and resume

		/// <summary>
		/// Fires the change event with the EventArgs provided in the argument.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnChanged(EventArgs e)
		{
			// if the DataTableSource has changed, we need to update the table
			if (e is TableDataSourceChangedEventArgs)
			{
				// Note: we update the data table here (and not in HandleLowPriorityChildChangeCases)
				// the reason is that we first want to see whether the parent is suspending us
				// if OnChange is called, the parent has not suspended us, thus we can update from the table data source
				// Disadvantage: the parent sees to change events: (i) the event with TableDataSourceChangeEventArgs, and (ii) the change event that is caused by the updated table
				UpdateTableFromTableDataSource();
			}
			else
			{
				base.OnChanged(e);
			}
		}

		protected override void OnAboutToBeResumed(int eventCount)
		{
			if (_accumulatedEventData.Contains(TableDataSourceChangedEventArgs.Empty))
			{
				_accumulatedEventData.Remove(TableDataSourceChangedEventArgs.Empty);
				UpdateTableFromTableDataSource();
			}
		}

		#endregion Suspend and resume

		private void EhNotesChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			EhChildChanged(sender, e);
		}

		/// <summary>
		/// Get / sets the parent object of this table.
		/// </summary>
		public override Main.IDocumentNode ParentObject
		{
			get
			{
				return _parent;
			}
			set
			{
				object oldParent = _parent;
				_parent = value;
				if (!object.ReferenceEquals(oldParent, _parent))
				{
					OnParentChanged(oldParent, _parent);
				}
			}
		}

		/// <summary>
		/// Fires the parent change event.
		/// </summary>
		/// <param name="oldParent">The parent before the change.</param>
		/// <param name="newParent">The parent after the change.</param>
		protected virtual void OnParentChanged(object oldParent, object newParent)
		{
			if (ParentChanged != null)
				ParentChanged(this, new Main.ParentChangedEventArgs(oldParent, newParent));
		}

		/// <summary>
		/// Get or sets the full name of the table.
		/// </summary>
		public override string Name
		{
			get
			{
				return _tableName;
			}

			set
			{
				if (null == value)
					throw new ArgumentNullException("New name is null");

				if (_tableName != value)
				{
					var e = new System.ComponentModel.CancelEventArgs();
					if (null != PreviewNameChange)
					{
						PreviewNameChange(this, value, e);
					}
					if (!e.Cancel)
					{
						string oldName = _tableName;
						_tableName = value;
						OnNameChanged(oldName);
					}
					else
					{
						throw new ApplicationException(string.Format("Renaming of table {0} into {1} not possible, because name exists already", _tableName, value));
					}
				}
			}
		}

		/// <summary>
		/// Gets the project folder name of this table.
		/// If the table is located in the root folder, the <see cref="Main.ProjectFolder.RootFolderName"/> (an empty string) is returned.
		/// If the table is located in any other folder, the full folder name including the trailing <see cref="Main.ProjectFolder.DirectorySeparatorChar"/> is returned.
		/// </summary>
		public string FolderName
		{
			get
			{
				return Main.ProjectFolder.GetFolderPart(Name);
			}
		}

		/// <summary>
		/// Gets the project folder name of this table without the trailing <see cref="Main.ProjectFolder.DirectorySeparatorChar"/>.
		/// If the table is located in the root folder, an exception will be thrown, because the root folder name doesn't contain a <see cref="Main.ProjectFolder.DirectorySeparatorChar"/>.
		/// If the table is located in any other folder, the full folder name, but without the trailing <see cref="Main.ProjectFolder.DirectorySeparatorChar"/> is returned.
		/// </summary>
		public string FolderNameWithoutTrailingDirectorySeparatorChar
		{
			get
			{
				string folderName = Main.ProjectFolder.GetFolderPart(Name);
				if (folderName.Length == 0)
					throw new ArgumentOutOfRangeException(string.Format("The table <<{0}>> is located in the root folder, therefore it is not possible to get the folder name without trailing directory separator char", Name));
				return folderName.Substring(0, folderName.Length - 1);
			}
		}

		/// <summary>
		/// Gets the short name (i.e. without the folder name) of this table.
		/// </summary>
		public string ShortName
		{
			get
			{
				return Main.ProjectFolder.GetNamePart(Name);
			}
		}

		/// <summary>
		/// Fires the name change event.
		/// </summary>
		/// <param name="oldName">The name of the table before the change.</param>
		protected virtual void OnNameChanged(string oldName)
		{
			EhSelfTunnelingEventHappened(Main.DocumentPathChangedEventArgs.Empty);

			if (NameChanged != null)
				NameChanged(this, oldName);
		}

		/// <summary>
		/// The date/time of creation of this table.
		/// </summary>
		public DateTime CreationTimeUtc
		{
			get
			{
				return _creationTime;
			}
		}

		/// <summary>
		/// The date/time when this table was changed.
		/// </summary>
		public DateTime LastChangeTimeUtc
		{
			get
			{
				return _lastChangeTime;
			}
		}

		/// <summary>
		/// Notes concerning this table.
		/// </summary>
		public Main.ITextBackedConsole Notes
		{
			get
			{
				return _notes;
			}
		}

		/// <summary>
		/// Gets or sets the data source of this table. For instance, this could be the SQL query that was used to fill data into this table.
		/// </summary>
		/// <value>
		/// The data source.
		/// </value>
		public IAltaxoTableDataSource DataSource
		{
			get
			{
				return _tableDataSource;
			}
			set
			{
				var oldValue = _tableDataSource;
				_tableDataSource = value;

				if (!object.Equals(oldValue, value))
				{
					if (null != oldValue)
						oldValue.ParentObject = null;
					if (null != value)
						value.ParentObject = this;
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Updates the data in the table from the table data source.
		/// </summary>
		public void UpdateTableFromTableDataSource()
		{
			if (null == _tableDataSource)
				return;

			using (var suspendToken = SuspendGetToken())
			{
				try
				{
					_tableDataSource.FillData(this);

					try
					{
						if (_tableDataSource.ImportOptions.ExecuteTableScriptAfterImport && null != _tableScript)
							_tableScript.ExecuteWithoutExceptionCatching(this, null);
					}
					catch (Exception ex)
					{
						this.Notes.WriteLine("Exception during execution of the table script (after execution of the data source). Details follow:");
						this.Notes.WriteLine(ex.ToString());
					}
				}
				catch (Exception ex)
				{
					this.Notes.WriteLine("Exception during execution of the data source. Details follow:");
					this.Notes.WriteLine(ex.ToString());
				}
			}
		}

		/// <summary>
		/// EventHandler used to catch unauthorized parent changes in child objects.
		/// </summary>
		/// <param name="sender">The child object which signal a parent change.</param>
		/// <param name="e">The parent change details.</param>
		protected void EhChildParentChanged(object sender, Main.ParentChangedEventArgs e)
		{
			// this event should not happen, or someone try to
			// change the parents of my collection
			throw new ApplicationException("Unexpected change of DataColumnsCollection's parent belonging to table " + this.Name);
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
			get { return _propertyColumns; }
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
		public DataColumnCollection PropertyColumns
		{
			get { return _propertyColumns; }
		}

		/// <summary>
		/// Gets access to the property data structured as rows.
		/// </summary>
		public IList<DataRow> PropertyRows
		{
			get { return _propertyColumns; }
		}

		/// <summary>Return the number of data columns.</summary>
		/// <value>The number of data columns in the table.</value>
		public int DataColumnCount
		{
			get { return this._dataColumns.ColumnCount; }
		}

		/// <summary>Returns the number of property rows. This is the same as <see cref="DataColumnCount" /> and is only provided for completness.</summary>
		/// <value>The number of property rows = number of data columns in the table.</value>
		public int PropertyRowCount
		{
			get { return this._dataColumns.ColumnCount; }
		}

		/// <summary>Returns the number of property columns.</summary>
		/// <value>The number of property columns in the table.</value>
		public int PropertyColumnCount
		{
			get { return this._propertyColumns.ColumnCount; }
		}

		/// <summary>Returns the number of data rows.</summary>
		/// <value>The number of data rows in the table.</value>
		public int DataRowCount
		{
			get { return this._dataColumns.RowCount; }
		}

		public TableScript TableScript
		{
			get { return _tableScript; }
			set
			{
				_tableScript = value;
			}
		}

		/// <summary>
		/// Copies data to the data column with the provided index if both columns are of the same type. If they are not of the same type, the column is replaced by the provided column. If the index is beyoind the limit, the provided column is added.
		/// </summary>
		/// <param name="idx">The index of the column where to copy to, or replace.</param>
		/// <param name="datac">The column to copy.</param>
		/// <param name="name">The name of the column in the case the column is added or replaced.</param>
		public virtual void CopyOrReplaceOrAdd(int idx, Altaxo.Data.DataColumn datac, string name)
		{
			using (var suspendToken = SuspendGetToken())
			{
				_dataColumns.CopyOrReplaceOrAdd(idx, datac, name); // add the column to the collection
				// no need to insert a property row here (only when inserting)
			}
		}

		/// <summary>
		/// Deletes all data columns in the table, and then copy all data columns from the source table.
		/// </summary>
		/// <param name="src">The source table to copy the data columns from.</param>
		public void CopyDataColumnsFrom(DataTable src)
		{
			this.DataColumns.CopyAllColumnsFrom(src.DataColumns);
		}

		/// <summary>
		/// Appends data columns from DataTable src to the data in this table.
		/// </summary>
		/// <param name="src">Source table.</param>
		/// <param name="ignoreNames">If true, the data columns in this table and in src table are compared by index. If false,
		/// the data columns in this table and in src table are compared by their name.</param>
		public void AppendAllDataColumns(DataTable src, bool ignoreNames)
		{
			DataColumns.AppendAllColumns(src.DataColumns, ignoreNames);
		}

		/// <summary>
		/// Appends data columns from DataTable src to the data in this table leaving some rows free inbetween.
		/// </summary>
		/// <param name="src">Source table.</param>
		/// <param name="ignoreNames">If true, the data columns in this table and in src table are compared by index. If false,
		/// the data columns in this table and in src table are compared by their name.</param>
		/// <param name="rowSpace">Number of rows to leave free between data in this table and newly appended data.</param>
		public void AppendAllColumnsWithSpace(DataTable src, bool ignoreNames, int rowSpace)
		{
			DataColumns.AppendAllColumnsWithSpace(src.DataColumns, ignoreNames, rowSpace);
		}

		/// <summary>
		/// Appends data columns from DataTable src to the data in this table by copying the new data to a specified row.
		/// </summary>
		/// <param name="src">Source table.</param>
		/// <param name="ignoreNames">If true, the data columns in this table and in src table are compared by index. If false,
		/// the data columns in this table and in src table are compared by their name.</param>
		/// <param name="appendPosition">Row number of first row where the new data is copied to.</param>
		public void AppendAllDataColumnsToPosition(DataTable src, bool ignoreNames, int appendPosition)
		{
			DataColumns.AppendAllColumnsToPosition(src.DataColumns, ignoreNames, appendPosition);
		}

		/// <summary>
		/// Deletes all data and property columns in the table, and then copy all data and property columns from the source table.
		/// </summary>
		/// <param name="src">The source table to copy the data columns from.</param>
		public void CopyDataAndPropertyColumnsFrom(DataTable src)
		{
			using (var suspendToken = SuspendGetToken())
			{
				this.DataColumns.CopyAllColumnsFrom(src.DataColumns);
				this.PropCols.CopyAllColumnsFrom(src.PropCols);
			}
		}

		/// <summary>
		/// Deletes all data and property columns in the table, and then copy all data and property columns, from the source table.
		/// As you can specify, this can be done with or without the data.
		/// </summary>
		/// <param name="src">The source table to copy the data columns from.</param>
		/// <param name="copyDataColumnData">If true, the data from the data columns of the source table is also copied.</param>
		/// <param name="copyPropertyColumnData">If true, the data from the property columns of the source table is also copied.</param>
		public void CopyDataAndPropertyColumnsFrom(DataTable src, bool copyDataColumnData, bool copyPropertyColumnData)
		{
			using (var suspendToken = SuspendGetToken())
			{
				if (copyDataColumnData)
					this.DataColumns.CopyAllColumnsFrom(src.DataColumns);
				else
					this.DataColumns.CopyAllColumnsWithoutDataFrom(src.DataColumns);

				if (copyPropertyColumnData)
					this.PropCols.CopyAllColumnsFrom(src.PropCols);
				else
					this.PropCols.CopyAllColumnsWithoutDataFrom(src.PropCols);
			}
		}

		/// <summary>
		/// Returns the collection of data columns. Used as simplification in scripts to provide access in the form table["A"].Col[2].
		/// </summary>
		public DataColumnCollection Col
		{
			get { return _dataColumns; }
		}

		/// <summary>
		/// Returns the collection of data columns.
		/// </summary>
		public DataColumnCollection DataColumns
		{
			get { return _dataColumns; }
		}

		/// <summary>
		/// Gets access to the data, but structured as rows.
		/// </summary>
		public IList<DataRow> DataRows
		{
			get { return _dataColumns; }
		}

		/// <summary>
		/// Get/sets the data column at index i. Setting is done by copying data, if the two columns has the same type. If the two columns are not of
		/// the same type, an exception is thrown.
		/// </summary>
		public DataColumn this[int i]
		{
			get { return _dataColumns[i]; }
			set { _dataColumns[i] = value; }
		}

		/// <summary>
		/// Get/sets the data column with the given name. Setting is done by copying data, if the two columns has the same type. If the two columns are not of
		/// the same type, an exception is thrown.
		/// </summary>
		public DataColumn this[string name]
		{
			get { return _dataColumns[name]; }
			set { _dataColumns[name] = value; }
		}

		/// <summary>Gets or sets a property cell, with is specified by the data column name (1st argument), and the property column name (2nd argument).
		/// Attention: this order of arguments is opposite to the usual notation used for matrices (row, column)!</summary>
		public AltaxoVariant this[string dataColumName, string propertyName]
		{
			get
			{
				int columnNumber = _dataColumns.GetColumnNumber(_dataColumns[dataColumName]);
				return _propertyColumns[propertyName][columnNumber];
			}
			set
			{
				int columnNumber = _dataColumns.GetColumnNumber(_dataColumns[dataColumName]);
				_propertyColumns[propertyName][columnNumber] = value;
			}
		}

		/// <summary>
		/// Tests if the table contains a data column with the provided name.
		/// </summary>
		/// <param name="name">The name to look for.</param>
		/// <returns>True if the table contains a data column with the provided name.</returns>
		public bool ContainsColumn(string name)
		{
			return _dataColumns.ContainsColumn(name);
		}

		/// <summary>
		/// Tests if the table contains a data column with the provided name.
		/// </summary>
		/// <param name="name">The name to look for.</param>
		/// <returns>True if the table contains a data column with the provided name.</returns>
		public bool ContainsDataColumn(string name)
		{
			return _dataColumns.ContainsColumn(name);
		}

		/// <summary>
		/// Tests if the table contains a property column with the provided name.
		/// </summary>
		/// <param name="name">The name to look for.</param>
		/// <returns>True if the table contains a data column with the provided name.</returns>
		public bool ContainsPropertyColumn(string name)
		{
			return _propertyColumns.Contains(name);
		}

		/// <summary>
		/// Moves the selected columns along with their corresponding property values to a new position.
		/// </summary>
		/// <param name="selectedIndices">The indices of the columns to move.</param>
		/// <param name="newPosition">The index of the new position where the columns are moved to.</param>
		public void ChangeColumnPosition(Altaxo.Collections.IAscendingIntegerCollection selectedIndices, int newPosition)
		{
			this._dataColumns.ChangeColumnPosition(selectedIndices, newPosition);
			this._propertyColumns.ChangeRowPosition(selectedIndices, newPosition);
		}

		/// <summary>
		/// Transpose transpose the table, i.e. exchange columns and rows
		/// this can only work if all columns in the table are of the same type
		/// </summary>
		/// <returns>null if succeeded, error string otherwise</returns>
		public virtual string Transpose()
		{
			// TODO: do also look at the property columns for transposing
			_dataColumns.Transpose();

			return null; // no error message
		}

		/// <summary>
		/// Transpose transpose the table, i.e. exchange columns and rows
		/// this can only work if all columns in the table are of the same type
		/// </summary>
		/// <param name="numberOfDataColumnsChangeToPropertyColumns">Number of data columns that are changed to property columns before transposing the table.</param>
		/// <param name="numberOfPropertyColumnsChangeToDataColumns">Number of property columns that are changed to data columns before transposing the table.</param>
		/// <returns>null if succeeded, error string otherwise</returns>
		public virtual string Transpose(int numberOfDataColumnsChangeToPropertyColumns, int numberOfPropertyColumnsChangeToDataColumns)
		{
			numberOfDataColumnsChangeToPropertyColumns = Math.Max(numberOfDataColumnsChangeToPropertyColumns, 0);
			numberOfDataColumnsChangeToPropertyColumns = Math.Min(numberOfDataColumnsChangeToPropertyColumns, this.DataColumnCount);

			numberOfPropertyColumnsChangeToDataColumns = Math.Max(numberOfPropertyColumnsChangeToDataColumns, 0);
			numberOfPropertyColumnsChangeToDataColumns = Math.Min(numberOfPropertyColumnsChangeToDataColumns, this.PropertyColumnCount);

			// first, save the first data columns that are changed to property columns
			Altaxo.Data.DataColumnCollection savedDataColumns = new DataColumnCollection();
			Altaxo.Data.DataColumnCollection savedPropColumns = new DataColumnCollection();

			Altaxo.Collections.IAscendingIntegerCollection savedDataColIndices = Altaxo.Collections.ContiguousIntegerRange.FromStartAndCount(0, numberOfDataColumnsChangeToPropertyColumns);
			Altaxo.Collections.IAscendingIntegerCollection savedPropColIndices = Altaxo.Collections.ContiguousIntegerRange.FromStartAndCount(0, numberOfPropertyColumnsChangeToDataColumns);
			// store the columns temporarily in another collection and remove them from the original collections
			DataColumns.MoveColumnsTo(savedDataColumns, 0, savedDataColIndices);
			this.PropertyColumns.MoveColumnsTo(savedPropColumns, 0, savedPropColIndices);

			// now transpose the data columns
			_dataColumns.Transpose();

			savedDataColumns.InsertRows(0, numberOfPropertyColumnsChangeToDataColumns); // take offset caused by newly inserted prop columns->data columns into account
			savedDataColumns.MoveColumnsTo(this.PropertyColumns, 0, savedDataColIndices);

			savedPropColumns.RemoveRows(0, numberOfDataColumnsChangeToPropertyColumns); // take offset caused by data columns changed to property columns into account
			savedPropColumns.MoveColumnsTo(this.DataColumns, 0, savedPropColIndices);

			// now insert both the temporary stored DataColumnCollections at the beginning

			return null; // no error message
		}

		/// <summary>
		/// Signals that the table has changed, but has not currently signaled that change to it's parent.
		/// </summary>
		public virtual bool IsDirty
		{
			get
			{
				return _accumulatedEventData != null;
			}
		}

		/// <summary>
		/// Remove the data columns <b>and the corresponding property rows</b> beginning at index nFirstColumn.
		/// </summary>
		/// <param name="nFirstColumn">The index of the first column to remove.</param>
		/// <param name="nDelCount">The number of columns to remove.</param>
		public virtual void RemoveColumns(int nFirstColumn, int nDelCount)
		{
			RemoveColumns(ContiguousIntegerRange.FromStartAndCount(nFirstColumn, nDelCount));
		}

		/// <summary>
		/// Remove the selected data columns <b>and the corresponding property rows</b>.
		/// </summary>
		/// <param name="selectedColumns">A collection of the indizes to the columns that have to be removed.</param>
		public virtual void RemoveColumns(IAscendingIntegerCollection selectedColumns)
		{
			using (var suspendToken = SuspendGetToken())
			{
				_dataColumns.RemoveColumns(selectedColumns); // remove the columns from the collection
				_propertyColumns.RemoveRows(selectedColumns); // remove also the corresponding rows from the Properties
			}
		}

		/// <summary>
		/// Gets an arbitrary object that was stored as table property by <see cref="SetTableProperty" />.
		/// </summary>
		/// <param name="key">Name of the property.</param>
		/// <returns>The object, or null if no object under the provided name was stored here.</returns>
		public object GetTableProperty(string key)
		{
			object result = null;
			if (_tableProperties != null)
				_tableProperties.TryGetValue(key, out result);

			return result;
		}

		/// <summary>
		/// The table properties, key is a string, val is a object you want to store here.
		/// </summary>
		/// <remarks>The properties are saved on disc (with exception of those who's name starts with "tmp/".
		/// If the property you want to store is only temporary, the property name should therefore
		/// start with "tmp/".</remarks>
		public void SetTableProperty(string key, object val)
		{
			PropertyBagNotNull.SetValue(key, val);
		}

		/// <summary>
		/// Remove a table property, key is a string
		/// </summary>
		/// <returns>True if the property was found (and removed). False if there was no such property to remove.</returns>
		/// <remarks>The properties are saved on disc (with exception of those who's name starts with "tmp/".
		/// If the property you want to store is only temporary, the property name should therefore
		/// start with "tmp/".</remarks>
		public bool RemoveTableProperty(string key)
		{
			return null == _tableProperties ? false : _tableProperties.RemoveValue(key);
		}

		protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			yield return new Main.DocumentNodeAndName(_dataColumns, "DataCols");

			yield return new Main.DocumentNodeAndName(_propertyColumns, "PropCols");

			if (null != DataSource)
				yield return new Main.DocumentNodeAndName(DataSource, "DataSource");

			if (null != PropertyBag)
				yield return new Main.DocumentNodeAndName(PropertyBag, "PropertyBag");
		}

		/// <summary>
		/// Gets the directory part of the table name with trailing <see cref="Main.ProjectFolder.DirectorySeparatorChar"/>.
		/// If the table is located in the root folder, the <see cref="Main.ProjectFolder.RootFolderName"/>  (an empty string) is returned.
		/// </summary>
		public string Folder
		{
			get
			{
				return Main.ProjectFolder.GetFolderPart(this.Name);
			}
		}

		/// <summary>
		/// Get the parent data table of a DataColumnCollection.
		/// </summary>
		/// <param name="colcol">The DataColumnCollection for which the parent table has to be found.</param>
		/// <returns>The parent data table of the DataColumnCollection, or null if it was not found.</returns>
		public static Altaxo.Data.DataTable GetParentDataTableOf(DataColumnCollection colcol)
		{
			if (colcol.ParentObject is DataTable)
				return (DataTable)colcol.ParentObject;
			else
				return (DataTable)Main.DocumentPath.GetRootNodeImplementing(colcol, typeof(DataTable));
		}

		/// <summary>
		/// Gets the parent data table of the DataColumn column.
		/// </summary>
		/// <param name="column">The data column for wich the parent data table should be found.</param>
		/// <returns>The parent data table of this column, or null if it can not be found.</returns>
		public static Altaxo.Data.DataTable GetParentDataTableOf(DataColumn column)
		{
			if (column.ParentObject is DataColumnCollection)
				return GetParentDataTableOf((DataColumnCollection)column.ParentObject);
			else
				return (DataTable)Main.DocumentPath.GetRootNodeImplementing(column, typeof(DataTable));
		}

		/// <summary>
		/// Gets the parent data table of a child object.
		/// </summary>
		/// <param name="child">The child object for which the parent table should be found.</param>
		public static Altaxo.Data.DataTable GetParentDataTableOf(Main.IDocumentLeafNode child)
		{
			if (child == null)
				return null;
			else
				return (DataTable)Main.DocumentPath.GetRootNodeImplementing(child, typeof(DataTable));
		}

		#region IPropertyBagOwner

		public Main.Properties.PropertyBag PropertyBag
		{
			get { return _tableProperties; }
			protected set
			{
				_tableProperties = value;
				if (null != _tableProperties)
					_tableProperties.ParentObject = this;
			}
		}

		public Main.Properties.PropertyBag PropertyBagNotNull
		{
			get
			{
				if (null == _tableProperties)
					PropertyBag = new Main.Properties.PropertyBag();
				return _tableProperties;
			}
		}

		#endregion IPropertyBagOwner

		/// <summary>
		/// Has to enumerate all references to other items in the project (<see cref="T:Altaxo.Main.DocNodeProxy" />) which are used in this project item and in all childs of this project item. The references
		/// has to be reported to the <paramref name="ProxyProcessing" /> function. This function is responsible for processing of the proxies, for instance to relocated the path.
		/// </summary>
		/// <param name="ProxyProcessing">Function that processes  the found <see cref="T:Altaxo.Main.DocNodeProxy" /> instances.</param>
		public void VisitDocumentReferences(Main.DocNodeProxyReporter ProxyProcessing)
		{
			if (this._tableDataSource != null)
			{
				this._tableDataSource.VisitDocumentReferences(ProxyProcessing);
			}
		}
	} // end class Altaxo.Data.DataTable
}