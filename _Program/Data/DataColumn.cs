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
	/// The main purpose of the column.
	/// </summary>
	[Serializable]
	public enum ColumnKind
	{
		/// <summary>
		/// Column values are the dependent variable (usually y in 2D-Plots, z in 3D-plots) 
		/// </summary>
		V=0,
		/// <summary>
		/// Column values are the first independent variable.
		/// </summary>
		X=1,
		/// <summary>
		/// Column values are the second independent variable.
		/// </summary>
		Y=2,			
		/// <summary>
		/// Column values are the third independent variable.
		/// </summary>
		Z=3,								
		/// <summary>
		/// Column values are +- error values.
		/// </summary>
		Err=4,		
		/// <summary>
		/// Column values are + error values.
		/// </summary>
		pErr=5,
		/// <summary>
		/// Column values are - error values.
		/// </summary>
		mErr=6,
		/// <summary>
		/// Column values are labels.
		/// </summary>
		Label=7,
		/// <summary>
		/// Column values are the plot condition, i.e. if zero, the row is ignored during plotting.
		/// </summary>
		Condition=8
	}



	/// <summary>
	/// This designates a vector structure, which holds elements. A single element at a given index can be read out
	/// by returning a AltaxoVariant.
	/// </summary>
	public interface IReadableColumn : ICloneable
	{

		/// <summary>
		/// The indexer property returns the element at index i as an AltaxoVariant.
		/// </summary>
		AltaxoVariant this[int i] 
		{
			get;
		}

		/// <summary>
		/// Returns true, if the value at index i of the column
		/// is null or invalid or in another state comparable to null or empty
		/// </summary>
		/// <param name="i">The index to the element.</param>
		/// <returns>true if element is null/empty, false if the element is valid</returns>
		bool IsElementEmpty(int i);

		/// <summary>
		/// FullName returns a descriptive name for a column
		/// for columns which belongs to a table, the table name and the column
		/// name, separated by a backslash, should be returned
		/// for other columns, a descriptive name should be returned so that the
		/// user knows the location of this column
		/// </summary>
		string FullName
		{
			get;
		}
	}

	/// <summary>
	/// A column, for which the elements can be set by assigning a AltaxoVariant to a element at index i.
	/// </summary>
	public interface IWriteableColumn : ICloneable
	{
		/// <summary>
		/// Indexer property for setting the element at index i by a AltaxoVariant.
		/// This function should throw an exeption, if the type of the variant do not match
		/// the type of the column.
		/// </summary>
		AltaxoVariant this[int i] 
		{
			set;
		}
	}


	/// <summary>
	/// This is a column with elements, which can be treated as numeric values. This is truly the case
	/// for columns which hold integer values or floating point values. Also true for DateTime columns, since they
	/// can converted in seconds since a given reference date.
	/// </summary>
	public interface INumericColumn : IReadableColumn, ICloneable
	{
		/// <summary>
		/// Returns the value of a column element at index i as numeric value (double).
		/// </summary>
		/// <param name="i">The index to the column element.</param>
		/// <returns>The value of the column element as double value.</returns>
		double GetDoubleAt(int i);
	}

	/// <summary>
	/// The indexer column is a simple readable numeric column. The value of an element is 
	/// it's index in the column, i.e. GetDoubleAt(i) simply returns the value i.
	/// </summary>
	[Serializable]
	public class IndexerColumn : INumericColumn, IReadableColumn, ICloneable
	{

		#region Serialization
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(IndexerColumn),0)]
			public new class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info, object parent)
			{
				IndexerColumn s = null!=o ? (IndexerColumn)o : new IndexerColumn();
				return s;
			}
		}
		#endregion
		/// <summary>
		/// Creates a cloned instance of this object.
		/// </summary>
		/// <returns>The cloned instance of this object.</returns>
		public object Clone()
		{
			return new IndexerColumn();
		}
		/// <summary>
		/// Simply returns the value i.
		/// </summary>
		/// <param name="i">The index i.</param>
		/// <returns>The index i.</returns>
		public double GetDoubleAt(int i)
		{
			return i;
		}

		/// <summary>
		/// This returns always true.
		/// </summary>
		/// <param name="i">The index i.</param>
		/// <returns>Always true.</returns>
		public bool IsElementEmpty(int i)
		{
			return false;
		}

		/// <summary>
		/// Returns the index i as AltaxoVariant.
		/// </summary>
		public AltaxoVariant this[int i] 
		{
			get 
			{
				return new AltaxoVariant((double)i);
			}
		} 

		/// <summary>
		/// The full name of a indexer column is "IndexerColumn".
		/// </summary>
		public string FullName
		{
			get { return "IndexerColumn"; }
		}

	}


	/// <summary>
	/// The EquallySpacedColumn is a simple readable numeric column. The value of an element is 
	/// calculated from y = a+b*i. This means the value of the first element is a, the values are equally spaced by b.
	/// </summary>
	[Serializable]
	public class EquallySpacedColumn : INumericColumn, IReadableColumn, ICloneable
	{
		/// <summary>The start value, i.e. the value at index 0.</summary>
		protected double m_Start=0;
		/// <summary>The spacing value between consecutive elements.</summary>
		protected double m_Increment=1;


		#region Serialization
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(EquallySpacedColumn),0)]
			public new class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				EquallySpacedColumn s = (EquallySpacedColumn)obj;
				info.AddValue("StartValue",s.m_Start);
				info.AddValue("Increment",s.m_Increment);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info, object parent)
			{
				EquallySpacedColumn s = null!=o ? (EquallySpacedColumn)o : new EquallySpacedColumn(0,1);
				s.m_Start = info.GetDouble("StartValue");
				s.m_Increment = info.GetDouble("Increment");
				return s;
			}
		}
		#endregion

		/// <summary>
		/// Creates a EquallySpacedColumn with starting value start and spacing increment.
		/// </summary>
		/// <param name="start">The starting value.</param>
		/// <param name="increment">The increment value (spacing value between consecutive elements).</param>
		public EquallySpacedColumn(double start, double increment)
		{
			m_Start = start;
			m_Increment = increment;
		}

		/// <summary>
		/// Creates a cloned instance of this object.
		/// </summary>
		/// <returns>The cloned instance of this object.</returns>
		public object Clone()
		{
			return new EquallySpacedColumn(m_Start, m_Increment);
		}


		/// <summary>
		/// Simply returns the value i.
		/// </summary>
		/// <param name="i">The index i.</param>
		/// <returns>The index i.</returns>
		public double GetDoubleAt(int i)
		{
			return m_Start + i*m_Increment;
		}

		/// <summary>
		/// This returns always true.
		/// </summary>
		/// <param name="i">The index i.</param>
		/// <returns>Always true.</returns>
		public bool IsElementEmpty(int i)
		{
			return false;
		}

		/// <summary>
		/// Returns the index i as AltaxoVariant.
		/// </summary>
		public AltaxoVariant this[int i] 
		{
			get 
			{
				return new AltaxoVariant((double)(m_Start+i*m_Increment));
			}
		} 

		/// <summary>
		/// The full name of a indexer column is "EquallySpacedColumn(start,increment)".
		/// </summary>
		public string FullName
		{
			get { return "EquallySpacedColumn("+m_Start.ToString()+","+m_Increment.ToString()+")"; }
		}

	}


	/// <summary>
	/// The interface to a column which has a definite number of elements.
	/// </summary>
	public interface IDefinedCount
	{
		/// <summary>
		/// Get the number of elements of the column.
		/// </summary>
		int Count
		{
			get;
		}
	}



	/// <summary>
	/// This is the base class of all data columns in Altaxo. This base class provides readable, writeable 
	/// columns with a defined count.
	/// </summary>
	[SerializationSurrogate(0,typeof(Altaxo.Data.DataColumn.SerializationSurrogate0))]
	[SerializationVersion(0)]
	[Serializable()]
	public abstract class DataColumn :
		IDisposable,		
		System.Runtime.Serialization.ISerializable,
		System.Runtime.Serialization.IDeserializationCallback, 
		IReadableColumn, 
		IWriteableColumn, 
		IDefinedCount,
		ICloneable,
		Altaxo.Main.IDocumentNode
	{
		///<summary>The name of the column.</summary>
		protected string m_ColumnName=null;
		
		/// <summary>The number (position) of the column in the data table.</summary>
		/// <remarks>For normal columns, this value is positive (zero for the first column).<para/>
		/// For property columns ("horizontal columns"), this value is negative (-1 for the first
		/// property column).</remarks>
		protected int m_ColumnNumber=0;

		/// <summary>
		/// The parent table this column belongs to.
		/// </summary>
		
		[NonSerialized]
		protected Altaxo.Data.DataColumnCollection m_Parent=null;

		/// <summary>The group number the column belongs to.</summary>
		/// <remarks>Normal columns are organized in groups. Every group of colums has either no or
		/// exactly one designated x-column (i.e. independend variable).</remarks>
		protected int m_Group=0; // number of group this column is belonging to
		
		/// <summary>The kind of the column, <see cref="ColumnKind"/></summary>
		protected ColumnKind m_Kind = ColumnKind.V; // kind of column

		/// <summary>The column count, i.e. one more than the index to the last valid element.</summary>
		protected int m_Count=0; // Index of last valid data + 1

		/// <summary>If the capacity of the column is not enough, a new array is aquired, with the new size
		/// newSize = addSpace+increaseFactor*oldSize.</summary>
		protected static double increaseFactor=2; // array space is increased by this factor plus addSpace
		/// <summary>If the capacity of the column is not enough, a new array is aquired, with the new size
		/// newSize = addSpace+increaseFactor*oldSize.</summary>
		protected static int    addSpace=32; // array space is increased by multiplying with increasefactor + addspase

		/// <summary>Lower bound of the area of rows, which changed during the data change event off period.</summary>
		protected int m_MinRowChanged=int.MaxValue;
		/// <summary>Upper bound of the area of rows, which changed during the data change event off period.</summary>
		protected int m_MaxRowChanged=int.MinValue;
		/// <summary>Indicates, if the row count decreased during the data change event off period. In this case it is neccessary
		/// to recalculate the row count of the table, since it is possible that the table row count also decreased in this case.</summary>
		protected bool m_bRowCountDecreased=false; // true if during event switch of period, the row m_Count  of this column decreases 
		
		/// <summary>Counter of how many suspends to data change event notifications are pending.</summary>
		/// <remarks>If this counter is zero, then every change to a element of this column fires a data change event. Applications doing a lot of changes at once can
		/// suspend this events for a better performance by calling <see cref="SuspendDataChangedNotifications"/>. After finishing the application has to
		/// call <see cref="ResumeDataChangedNotifications"/></remarks>
		protected int  m_DataEventsSuspendCount=0;

		/// <summary>
		/// Handler of data change events. The area of changed rows is provided by nMinRow and nMaxRow. If the row count
		/// has decreased this is indicated by bRowCountDecreased is true.
		/// </summary>
		public delegate void DataChangedHandler(Altaxo.Data.DataColumn sender, int nMinRow, int nMaxRow, bool bRowCountDecreased );   // delegate declaration
		public delegate void DirtySetHandler(Altaxo.Data.DataColumn sender);
		public delegate void DisposedHandler(Altaxo.Data.DataColumn sender);

		/// <summary>
		/// Data changed events are fired if any of the data of this column is changed. If the event is suspended by a call to
		/// <see cref="SuspendDataChangedNotifications"/>, the changes are stored internally. If the event is resumed, then
		/// the accumulated changes are returned by the <see cref="DataChangedHandler"/>.</summary>
		public event DataChangedHandler						DataChanged;
		
		/// <summary>This element is fired when the column is to be disposed.</summary><remarks>All instances, which have a reference
		/// to this column, should have a wire to this event. In case the event is fired, it indicates
		/// that the column should be disposed, so they have to unreference this column by setting the
		/// reference to null.
		/// </remarks>
		public event DisposedHandler							ColumnDisposed;

		/// <summary>
		/// This event is fired once (and only once) when something in the column has changed,
		/// and the column state switches from not dirty to dirty. Once the column state is dirty,
		/// further changes to the column will _not_ fire the event.
		/// </summary>
		protected internal event DirtySetHandler	DirtySet;
 

		#region Serialization




		/// <summary>
		/// This class is responsible for the serialization of the DataColumn (version 0).
		/// </summary>
		public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			/// <summary>Serializes the DataColumn given by object obj.</summary>
			/// <param name="obj">The <see cref="DataColumn"/> instance which should be serialized.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			/// <remarks>I decided _not_ to serialize the parent object, because there are situations were we
			/// only want to serialize this column. But if we also serialize the parent table, we end up serializing all the object graph.
			/// </remarks>
			public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{
				Altaxo.Data.DataColumn s = (Altaxo.Data.DataColumn)obj;
				// info.AddValue("Parent",s.m_Table); // not serialize the parent, see remarks
				info.AddValue("Name",s.m_ColumnName);
				info.AddValue("Number",s.m_ColumnNumber);
				info.AddValue("Count",s.m_Count);
				info.AddValue("Kind",(int)s.m_Kind);
				info.AddValue("Group",s.m_Group);
			}

			/// <summary>
			/// Deserializes the <see cref="DataColumn"/> instance.
			/// </summary>
			/// <param name="obj">The empty DataColumn instance, created by the runtime.</param>
			/// <param name="info">Serialization info.</param>
			/// <param name="context">The streaming context.</param>
			/// <param name="selector">The surrogate selector.</param>
			/// <returns>The deserialized object.</returns>
			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
			{
				Altaxo.Data.DataColumn s = (Altaxo.Data.DataColumn)obj;
				// s.m_Table = (Altaxo.Data.DataTable)(info.GetValue("Parent",typeof(Altaxo.Data.DataTable)));
				s.m_Parent = null;
				s.m_ColumnName = info.GetString("Name");
				s.m_ColumnNumber = info.GetInt32("Number");
				s.m_Count = info.GetInt32("Count");
				s.m_Kind  = (ColumnKind)info.GetInt32("Kind");
				s.m_Group = info.GetInt32("Group");

				// set the helper data
				s.m_MinRowChanged=int.MaxValue; // area of rows, which changed during event off period
				s.m_MaxRowChanged=int.MinValue;
				return s;
			}
		}


		/// <summary>
		/// This class is responsible for the serialization of the DataColumn (version 0).
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Altaxo.Data.DataColumn),0)]
		public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			/// <summary>Serializes the DataColumn given by object obj.</summary>
			/// <param name="obj">The <see cref="DataColumn"/> instance which should be serialized.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			/// <remarks>I decided _not_ to serialize the parent object, because there are situations were we
			/// only want to serialize this column. But if we also serialize the parent table, we end up serializing all the object graph.
			/// </remarks>
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info	)
			{
				Altaxo.Data.DataColumn s = (Altaxo.Data.DataColumn)obj;
				// info.AddValue("Parent",s.m_Table); // not serialize the parent, see remarks
				info.AddValue("Name",s.m_ColumnName);
				info.AddValue("Number",s.m_ColumnNumber);
				info.AddValue("Count",s.m_Count);
				info.AddValue("Kind",(int)s.m_Kind);
				info.AddValue("Group",s.m_Group);
			}

			/// <summary>
			/// Deserializes the <see cref="DataColumn"/> instance.
			/// </summary>
			/// <param name="obj">The empty DataColumn instance, created by the runtime.</param>
			/// <param name="info">Serialization info.</param>
			/// <param name="context">The streaming context.</param>
			/// <param name="selector">The surrogate selector.</param>
			/// <returns>The deserialized object.</returns>
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info, object parent)
			{
				Altaxo.Data.DataColumn s = (Altaxo.Data.DataColumn)o;
				// s.m_Table = (Altaxo.Data.DataTable)(info.GetValue("Parent",typeof(Altaxo.Data.DataTable)));
				
				s.m_ColumnName = info.GetString("Name");
				s.m_ColumnNumber = info.GetInt32("Number");
				s.m_Count = info.GetInt32("Count");
				s.m_Kind  = (ColumnKind)info.GetInt32("Kind");
				s.m_Group = info.GetInt32("Group");

				
				// s.m_Parent = parent as Altaxo.Data.DataColumnCollection; // remarks: this is done during parent deserialization!
				// set the helper data
				s.m_MinRowChanged=int.MaxValue; // area of rows, which changed during event off period
				s.m_MaxRowChanged=int.MinValue;
				return s;
			}
		}
		/// <summary>
		/// This function is called on end of deserialization.
		/// </summary>
		/// <param name="obj">The deserialized DataColumn instance.</param>
		public virtual void OnDeserialization(object obj)
		{
		}

		protected DataColumn(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			SetObjectData(this,info,context,null);
		}

		public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
		{
			return new SerializationSurrogate0().SetObjectData(this,info,context,null);
		}

		public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			new SerializationSurrogate0().GetObjectData(this,info,context);
		}

		#endregion

		public DataColumn(DataColumn from)
		{
			this.m_bRowCountDecreased			= false;
			this.m_ColumnName							= from.m_ColumnName;
			this.m_ColumnNumber						= from.m_ColumnNumber;
			this.m_Count									= from.m_Count;
			this.m_DataEventsSuspendCount = 0;
			this.m_Group									= from.m_Group;
			this.m_Kind										= from.m_Kind;
			this.m_MaxRowChanged					= int.MinValue;
			this.m_MinRowChanged					= int.MaxValue;
			this.m_Parent									= null;
		}


		/// <summary>
		/// Creates a cloned instance of this object.
		/// </summary>
		/// <returns>The cloned instance of this object.</returns>
		public abstract object Clone();


		/// <summary>
		/// A call to this function suspends data changed event notifications
		/// </summary>
		/// <remarks>If an application has
		/// to change a lot of data in a column at once, it should call this function to avoid the firing
		/// of the event every time it changes a single element. After processing all items, the application
		/// has to resume the data changed event notification by calling <see cref="ResumeDataChangedNotifications"/>
		/// </remarks>
		public void SuspendDataChangedNotifications()
		{
			m_DataEventsSuspendCount++;
		}

		/// <summary>
		/// This resumes the data changed notifications if the suspend counter has reached zero.
		/// </summary>
		/// <remarks>The area of changed rows is updated even in the suspend period. The suspend counter is
		/// decreased by a call to this function. If it reaches zero, the data changed event is fired,
		/// and the arguments of the handler contain the changed area of rows during the suspend time.</remarks>
		public void ResumeDataChangedNotifications()
		{
			m_DataEventsSuspendCount--;
			if(m_DataEventsSuspendCount<0) m_DataEventsSuspendCount=0;

			if(0==m_DataEventsSuspendCount && this.IsDirty)
				NotifyDataChanged(m_MinRowChanged,m_MaxRowChanged,m_bRowCountDecreased);
		}

		/// <summary>
		/// Copies the head of the column, i.e. the column name from another column.
		/// The column number is not copied, since this has to be set by the parent table.
		/// </summary>
		/// <param name="ano">Column the head is copied from.</param>
		/// <remarks>This function will throw an ApplicationException, if this column (the column the head should be
		/// copied to) already has a parent. This is because if the column has a parent, the parent object is
		/// responsible for naming the child columns, so the direct renaming done here is not allowed in this case.
		/// </remarks>
		public void CopyHeaderFrom(DataColumn ano)
		{
			// throw an exception, if the destination column has a parent, that is not supported!
			// because the m_Table has to set up the names and so on
			if(this.m_Parent!=null)
				throw new ApplicationException("The column " + this.ColumnName + " has the parent m_Table " + m_Parent.TableName);
			
			this.m_ColumnName = ano.m_ColumnName;
		}

		/// <value>The column group number this column belongs to.</value>
		public int Group
		{
			get { return m_Group; }
			set { m_Group = value; }
		}

		/// <summary>
		/// The kind of the column. See <see cref="ColumnKind"/>.
		/// </summary>
		public ColumnKind Kind
		{
			get { return m_Kind; }
			set 
			{
				m_Kind = value;
			}
		}

		/// <value>Get/sets if this column is the X column. A X column is the column, which holds the first
		/// independent variable in a group of columns.</value>
		public bool XColumn
		{
			get { return m_Kind==ColumnKind.X; }
			set
			{
				if(null!=this.m_Parent && true==value)
				{
					m_Parent.DeleteXProperty(m_Group);
				}
				if(true==value)
					m_Kind = ColumnKind.X;
				else
					m_Kind = ColumnKind.V;
			}
		}

		/// <summary>
		/// copies the header information, like label and so on
		/// from another column to this column
		/// ColumnName and ColumnNumber is not copied!
		/// </summary>
		/// <param name="ano"></param>
		public void CopyHeaderInformationFrom(DataColumn ano)
		{
		}

		/// <summary>
		/// This function fires the data changed event if the suspend count is zero.
		/// </summary>
		/// <param name="minRow">lower row number of the area of rows which was changed.</param>
		/// <param name="maxRow">upper row number in the area of rows which was changed.</param>
		/// <param name="rowCountDecreased">Must be true if the row count decreased.</param>
		/// <remarks>The data changed event is only fired when the data changed suspend counter is zero.
		/// If it is zero, then before the data changed event is fired, the column "contacts" its parent table by
		/// calling the function <see cref="DataColumnCollection.OnColumnDataChanged"/>, informing the parent table of this change. If the parent table
		/// has a not-zero suspend counter, then it will suspend data changed notifications also for this column and the event is not fired.
		/// If the suspend counter of the parent table is zero, it firstly informs its parent data set by calling the function
		/// <see cref="TableSet.OnTableDataChanged"/>. If the suspend counter of the TableSet is not zero, then it will suspend the data changed events of the table. And the table will
		/// then suspend the data changed events of this column, so the event is not fired in this case<para/>
		/// That means in the end: only if the suspend counter of this column, the parent data table, and the parent data set of this table are all zero,
		/// then the data changed event is fired at all.</remarks>
		protected void NotifyDataChanged(int minRow, int maxRow, bool rowCountDecreased)
		{
			bool bWasDirtyBefore = this.IsDirty;

			if(null!=m_Parent || null!=DataChanged)
			{
				if(minRow < m_MinRowChanged)
					m_MinRowChanged = minRow;
				if(maxRow > m_MaxRowChanged) 
					m_MaxRowChanged = maxRow;
				
				m_bRowCountDecreased |= rowCountDecreased;
			}

			if(null!=m_Parent && 0==m_DataEventsSuspendCount)
			{
				// always inform the parent first,
				// because the parent can change our bDataEventEnabled to false)
				this.m_Parent.OnColumnDataChanged(this,m_MinRowChanged,m_MaxRowChanged,m_bRowCountDecreased);
			}
			
			if(0==m_DataEventsSuspendCount) // look again for this variable, because the parent can change it during OnDataChanged
			{	
				if(null!=DataChanged)
					DataChanged(this, m_MinRowChanged,m_MaxRowChanged,m_bRowCountDecreased);
			
				ResetDirty();
			}
			else // Data events disabled
			{
				// if the row is not dirty up to now, add it to the DataTables dirty column collection
				if(!bWasDirtyBefore && null!=DirtySet)
					DirtySet(this);
			}
		}

		/// <summary>
		/// Returns the type of the associated ColumnStyle for use in a worksheet view.</summary>
		/// <returns>The type of the associated <see cref="Worksheet.ColumnStyle"/> class.</returns>
		/// <remarks>
		/// If this type of data column is not used in a datagrid, you can return null for this type.
		/// </remarks>
		public abstract System.Type GetColumnStyleType();

		/// <summary>
		/// Column is dirty if either there are new changed or deleted rows, and no data changed event was fired for notification.
		/// This value is reseted after the data changed event has notified the change.
		/// </summary>
		public bool IsDirty
		{
			get
			{
				return (m_MinRowChanged<=m_MaxRowChanged);
			}
		}

		/// <summary>
		/// Resets the dirty attribute.
		/// </summary>
		protected void ResetDirty()
		{
			m_MinRowChanged=int.MaxValue;
			m_MaxRowChanged=int.MinValue;
			m_bRowCountDecreased=false;
		}

		/// <value>
		/// The position of the column in the parent table, has to be syncronized by the
		/// parent table.
		/// </value>
		public int ColumnNumber
		{
			get
			{
				return m_ColumnNumber;
			}
		}

		/// <summary>
		/// Sets the column number, only the parent data table should do that!
		/// This is because the column number must be synchronized with the
		/// position of the column in the parent data table and only that table knows about the position.
		/// </summary>
		/// <param name="n">The position of the column in the parent data table.</param>
		protected internal void SetColumnNumber(int n)
		{
			if(null!=m_Parent && m_Parent[n]!=this) // test if the column is really there
				throw new ApplicationException("Try to set wrong column number to column " + this.ColumnName + ", m_Table " + m_Parent.TableName);

			m_ColumnNumber=n;
		}

		/// <summary>
		/// Constructs a data column with no name associated.
		/// </summary>
		protected DataColumn()
		{
		}

		/// <summary>
		/// Constructs a data column with the name <paramref name="name"/>
		/// </summary>
		/// <param name="name">The initial name of the data column.</param>
		protected DataColumn(string name)
		{
			m_ColumnName = name;
		}

		/// <summary>
		/// Constructs a data column with the name <paramref name="name"/>, which belongs to the parent data
		/// table parenttable. The column is <b>not</b> automatically inserted in the parent table!
		/// </summary>
		/// <param name="parentcoll">The parent DataColumnCollection this column belongs to.</param>
		/// <param name="name">The initial name of the column.</param>
		/// <remarks>This function is mainly intended for use by the parent table, since only the
		/// parent table knows about which name can be used for the column.</remarks>
		protected DataColumn(Altaxo.Data.DataColumnCollection parentcoll, string name)
		{
			this.m_Parent = parentcoll;
			this.m_ColumnName = name;
		}

		/// <summary>
		/// Gets/sets the column name. If the column belongs to a table, the new name is checked by
		/// the parent table for uniqueness.
		/// </summary>
		public string ColumnName
		{
			get
			{
				return m_ColumnName;
			}
			set
			{
				if(m_ColumnName!=value)
				{
					if(this.m_Parent==null)
						m_ColumnName = value; // set value directly if no parent table
					else // parent table is not null, so lets check the name by the parent
						m_ColumnName = m_Parent.FindUniqueColumnName(value);
				}
			}
		}

		/// <summary>
		/// Returns either the column name if the column has no parent table, or the parent table name, followed by
		/// a backslash and the column name if the column has a table.
		/// </summary>
		public string FullName
		{
			get 
			{
				return null==m_Parent ? m_ColumnName : String.Format("{0}\\{1}",m_Parent.TableName,m_ColumnName);
			}
		}


		/// <summary>
		/// Returns the row count, i.e. the one more than the index to the last valid data element in the column. 
		/// </summary>
		public int Count
		{
			get
			{
				return m_Count;
			}
		}
		
		/// <summary>
		/// Returns the column type followed by a backslash and the column name.
		/// </summary>
		public string TypeAndName
		{
			get
			{
				return null==m_ColumnName ? this.GetType().ToString(): this.GetType().ToString() + "(\"" + m_ColumnName + "\")";
			}
		}

		/// <summary>
		/// Gets/sets the parent table.
		/// </summary>
		public Altaxo.Data.DataTable ParentTable
		{
			get
			{
				return m_Parent.Parent;
			}
		}

		public Altaxo.Data.DataColumnCollection Parent
		{
			get { return m_Parent; }
			set { m_Parent = value; }
		}

		public virtual object ParentObject
		{
			get { return m_Parent; }
		}

		public virtual string Name
		{
			get { return m_ColumnName; }
		}

		protected internal void SetParent(DataColumnCollection parentcoll)
		{
			m_Parent = parentcoll;
		}

		// indexers
		/// <summary>
		/// Sets the value at a given index i with a value val, which is a AltaxoVariant.
		/// </summary>
		/// <param name="i">The index (row number) which is set to the value val.</param>
		/// <param name="val">The value val as <see cref="AltaxoVariant"/>.</param>
		/// <remarks>The derived class should throw an exeption when the data type in the AltaxoVariant value val
		/// do not match the column type.</remarks>
		public abstract void SetValueAt(int i, AltaxoVariant val);
		
		
		/// <summary>
		/// This returns the value at a given index i as AltaxoVariant.
		/// </summary>
		/// <param name="i">The index (row number) to the element returned.</param>
		/// <returns>The element at index i.</returns>
		public abstract AltaxoVariant GetVariantAt(int i);
		
		/// <summary>
		/// This function is used to determine if the element at index i is valid or not. If it is valid,
		/// the derived class function has to return false. If it is empty or not valid, the function has
		/// to return true.
		/// </summary>
		/// <param name="i">Index to the element in question.</param>
		/// <returns>True if the element is empty or not valid.</returns>
		public abstract bool IsElementEmpty(int i);


		/// <summary>
		/// Gets/sets the element at the index i by a value of type <see cref="AltaxoVariant"/>.
		/// </summary>
		public AltaxoVariant this[int i] 
		{
			get
			{
				return GetVariantAt(i);
			}
			set
			{
				SetValueAt(i,value);
			}
		}

		/// <summary>
		/// Clears the content of the column and fires the <see cref="ColumnDisposed"/> event.
		/// </summary>
		public void Dispose()
		{
			this.m_Parent=null;
			this.m_ColumnNumber = int.MinValue;
			this.Clear();
			if(null!=ColumnDisposed)
				ColumnDisposed(this);
		}

		/// <summary>
		/// Clears all rows.
		/// </summary>
		public void Clear()
		{
			RemoveRows(0,this.Count);
		}

		/// <summary>
		/// Copies all elements of another DataColumn to this column. An exception is thrown if the data types of both columns are incompatible. 
		/// See also <see cref="CopyDataFrom"/>.</summary>
		public DataColumn Data
		{
			set { CopyDataFrom(value); }
		}

		/// <summary>
		/// Copies the contents of another column v to this column. An exception should be thrown if the data types of
		/// both columns are incompatible.
		/// </summary>
		/// <param name="v">The column the data will be copied from.</param>
		public abstract void CopyDataFrom(Altaxo.Data.DataColumn v);
		
		/// <summary>
		/// Removes a number of rows (given by <paramref name="nCount"/>) beginning from nFirstRow,
		/// i.e. remove rows number nFirstRow ... nFirstRow+nCount-1.
		/// </summary>
		/// <param name="nFirstRow">Number of first row to delete.</param>
		/// <param name="nCount">Number of rows to delete.</param>
		public abstract void RemoveRows(int nFirstRow, int nCount); // removes nCount rows starting from nFirstRow 
		
		
		/// <summary>
		/// Inserts <paramref name="nCount"/> empty rows before row number <paramref name="nBeforeRow"/>. 
		/// </summary>
		/// <param name="nBeforeRow">The row number before the additional rows are inserted.</param>
		/// <param name="nCount">Number of empty rows to insert.</param>
		public abstract void InsertRows(int nBeforeRow, int nCount); // inserts additional empty rows
		// -----------------------------------------------------------------------------
		// 
		//                      Operators
		//
		// -----------------------------------------------------------------------------

		
		// Note: unfortunately (and maybe also undocumented) we can not use
		// the names op_Addition, op_Subtraction and so one, because these
		// names seems to be used by the compiler for the operators itself
		// so we use here vopAddition and so on (the v from virtual)

		/// <summary>
		/// Adds another data column to this data column (item by item). 
		/// </summary>
		/// <param name="a">The data column to add.</param>
		/// <param name="b">The result of the addition (this+a).</param>
		/// <returns>True if successful, false if this operation is not supported.</returns>
		public virtual bool vop_Addition(DataColumn a, out DataColumn b)
		{	b=null; return false; }

		public virtual bool vop_Addition_Rev(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Addition(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Addition_Rev(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }

		/// <summary>
		/// Subtracts another data column from this data column (item by item). 
		/// </summary>
		/// <param name="a">The data column to subtract.</param>
		/// <param name="b">The result of the subtraction (this-a).</param>
		/// <returns>True if successful, false if this operation is not supported.</returns>
		public virtual bool vop_Subtraction(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Subtraction_Rev(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Subtraction(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Subtraction_Rev(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }

		/// <summary>
		/// Multiplies another data column to this data column (item by item). 
		/// </summary>
		/// <param name="a">The data column to multiply.</param>
		/// <param name="b">The result of the multiplication (this*a).</param>
		/// <returns>True if successful, false if this operation is not supported.</returns>
		public virtual bool vop_Multiplication(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Multiplication_Rev(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Multiplication(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Multiplication_Rev(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }

		/// <summary>
		/// Divides this data column by another data column(item by item). 
		/// </summary>
		/// <param name="a">The data column used for division.</param>
		/// <param name="b">The result of the division (this/a).</param>
		/// <returns>True if successful, false if this operation is not supported.</returns>
		public virtual bool vop_Division(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Division_Rev(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Division(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Division_Rev(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }

		public virtual bool vop_Modulo(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Modulo_Rev(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Modulo(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Modulo_Rev(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }

		public virtual bool vop_And(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_And_Rev(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_And(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_And_Rev(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }

		public virtual bool vop_Or(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Or_Rev(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Or(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Or_Rev(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }

		public virtual bool vop_Xor(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Xor_Rev(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Xor(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Xor_Rev(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }

		public virtual bool vop_ShiftLeft(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_ShiftLeft_Rev(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_ShiftLeft(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_ShiftLeft_Rev(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }

		public virtual bool vop_ShiftRight(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_ShiftRight_Rev(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_ShiftRight(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_ShiftRight_Rev(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }

		public virtual bool vop_Lesser(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Lesser_Rev(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Lesser(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Lesser_Rev(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }

		public virtual bool vop_Greater(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Greater_Rev(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Greater(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_Greater_Rev(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }

		public virtual bool vop_LesserOrEqual(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_LesserOrEqual_Rev(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_LesserOrEqual(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_LesserOrEqual_Rev(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }

		public virtual bool vop_GreaterOrEqual(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_GreaterOrEqual_Rev(DataColumn a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_GreaterOrEqual(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }
		public virtual bool vop_GreaterOrEqual_Rev(AltaxoVariant a, out DataColumn b)
		{	b=null; return false; }

		// Unary operators
		public virtual bool vop_Plus(out DataColumn b)
		{	b=null; return false; }

		public virtual bool vop_Minus(out DataColumn b)
		{	b=null; return false; }
		
		public virtual bool vop_Not(out DataColumn b)
		{	b=null; return false; }
		
		public virtual bool vop_Complement(out DataColumn b)
		{	b=null; return false; }
		
		public virtual bool vop_Increment(out DataColumn b)
		{	b=null; return false; }
		
		public virtual bool vop_Decrement(out DataColumn b)
		{	b=null; return false; }
		
		public virtual bool        vop_True(out bool b)
		{	b=false; return false; }
		
		public virtual bool        vop_False(out bool b)
		{	b=false; return false; }

	
		
		public static DataColumn operator +(DataColumn c1, DataColumn c2)
		{
			DataColumn c3;

			if(c1.vop_Addition(c2, out c3))
				return c3;
			if(c2.vop_Addition_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to add " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}

		public static DataColumn operator +(DataColumn c1, AltaxoVariant c2)
		{
			DataColumn c3;

			if(c1.vop_Addition(c2, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to add " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}

		public static DataColumn operator +(AltaxoVariant c1, DataColumn c2)
		{
			DataColumn c3;

			if(c2.vop_Addition_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to add " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}

		public static DataColumn operator -(DataColumn c1, DataColumn c2)
		{
			DataColumn c3;

			if(c1.vop_Subtraction(c2, out c3))
				return c3;
			if(c2.vop_Subtraction_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to subtract " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator -(DataColumn c1, AltaxoVariant c2)
		{
			DataColumn c3;

			if(c1.vop_Subtraction(c2, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to subtract " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator -(AltaxoVariant c1, DataColumn c2)
		{
			DataColumn c3;

			if(c2.vop_Subtraction_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to subtract " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}


		
		
		public static DataColumn operator *(DataColumn c1, DataColumn c2)
		{
			DataColumn c3;

			if(c1.vop_Multiplication(c2, out c3))
				return c3;
			if(c2.vop_Multiplication_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to multiply " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator *(DataColumn c1, AltaxoVariant c2)
		{
			DataColumn c3;

			if(c1.vop_Multiplication(c2, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to multiply " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator *(AltaxoVariant c1, DataColumn c2)
		{
			DataColumn c3;

			if(c2.vop_Multiplication_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to multiply " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}




		public static DataColumn operator /(DataColumn c1, DataColumn c2)
		{
			DataColumn c3;

			if(c1.vop_Division(c2, out c3))
				return c3;
			if(c2.vop_Division_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to divide " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator /(DataColumn c1, AltaxoVariant c2)
		{
			DataColumn c3;

			if(c1.vop_Division(c2, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to divide " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator /(AltaxoVariant c1, DataColumn c2)
		{
			DataColumn c3;

			if(c2.vop_Division_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to divide " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}



		public static DataColumn operator %(DataColumn c1, DataColumn c2)
		{
			DataColumn c3;

			if(c1.vop_Modulo(c2, out c3))
				return c3;
			if(c2.vop_Modulo_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to take modulus of " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator %(DataColumn c1, AltaxoVariant c2)
		{
			DataColumn c3;

			if(c1.vop_Modulo(c2, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to take modulus of " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator %(AltaxoVariant c1, DataColumn c2)
		{
			DataColumn c3;

			if(c2.vop_Modulo_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to take modulus of " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}


		public static DataColumn operator &(DataColumn c1, DataColumn c2)
		{
			DataColumn c3;

			if(c1.vop_And(c2, out c3))
				return c3;
			if(c2.vop_And_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply and operator to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator &(DataColumn c1, AltaxoVariant c2)
		{
			DataColumn c3;

			if(c1.vop_And(c2, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator AND to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator &(AltaxoVariant c1, DataColumn c2)
		{
			DataColumn c3;

			if(c2.vop_And_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator AND to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}

		public static DataColumn operator |(DataColumn c1, DataColumn c2)
		{
			DataColumn c3;

			if(c1.vop_Or(c2, out c3))
				return c3;
			if(c2.vop_Or_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply or operator to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator |(DataColumn c1, AltaxoVariant c2)
		{
			DataColumn c3;

			if(c1.vop_Or(c2, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator OR to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator |(AltaxoVariant c1, DataColumn c2)
		{
			DataColumn c3;

			if(c2.vop_Or_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator OR to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}

		public static DataColumn operator ^(DataColumn c1, DataColumn c2)
		{
			DataColumn c3;

			if(c1.vop_Xor(c2, out c3))
				return c3;
			if(c2.vop_Xor_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply xor operator to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator ^(DataColumn c1, AltaxoVariant c2)
		{
			DataColumn c3;

			if(c1.vop_Xor(c2, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator XOR to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator ^(AltaxoVariant c1, DataColumn c2)
		{
			DataColumn c3;

			if(c2.vop_Xor_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator XOR to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}

		public static DataColumn operator <<(DataColumn c1, int c2)
		{
			DataColumn c3;

			if(c1.vop_ShiftLeft(c2, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator << to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}


		public static DataColumn operator >>(DataColumn c1, int c2)
		{
			DataColumn c3;

			if(c1.vop_ShiftRight(c2, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator >> to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}

		public static DataColumn operator <(DataColumn c1, DataColumn c2)
		{
			DataColumn c3;

			if(c1.vop_Lesser(c2, out c3))
				return c3;
			if(c2.vop_Lesser_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator lesser to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator <(DataColumn c1, AltaxoVariant c2)
		{
			DataColumn c3;

			if(c1.vop_Lesser(c2, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator < to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator <(AltaxoVariant c1, DataColumn c2)
		{
			DataColumn c3;

			if(c2.vop_Lesser_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator < to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}

		public static DataColumn operator >(DataColumn c1, DataColumn c2)
		{
			DataColumn c3;

			if(c1.vop_Greater(c2, out c3))
				return c3;
			if(c2.vop_Greater_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator greater to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator >(DataColumn c1, AltaxoVariant c2)
		{
			DataColumn c3;

			if(c1.vop_Greater(c2, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator > to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator >(AltaxoVariant c1, DataColumn c2)
		{
			DataColumn c3;

			if(c2.vop_Greater_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator > to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}

		public static DataColumn operator <=(DataColumn c1, DataColumn c2)
		{
			DataColumn c3;

			if(c1.vop_LesserOrEqual(c2, out c3))
				return c3;
			if(c2.vop_LesserOrEqual_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator LesserOrEqual to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator <=(DataColumn c1, AltaxoVariant c2)
		{
			DataColumn c3;

			if(c1.vop_LesserOrEqual(c2, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator <= to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator <=(AltaxoVariant c1, DataColumn c2)
		{
			DataColumn c3;

			if(c2.vop_LesserOrEqual_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator <= " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}

		public static DataColumn operator >=(DataColumn c1, DataColumn c2)
		{
			DataColumn c3;

			if(c1.vop_GreaterOrEqual(c2, out c3))
				return c3;
			if(c2.vop_GreaterOrEqual_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator GreaterOrEqual to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator >=(DataColumn c1, AltaxoVariant c2)
		{
			DataColumn c3;

			if(c1.vop_GreaterOrEqual(c2, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator >= to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}
		public static DataColumn operator >=(AltaxoVariant c1, DataColumn c2)
		{
			DataColumn c3;

			if(c2.vop_GreaterOrEqual_Rev(c1, out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator >= to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
		}

		public static DataColumn operator +(DataColumn c1)
		{
			DataColumn c3;

			if(c1.vop_Plus(out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator plus to " + c1.ToString() + " (" + c1.GetType() + ")");
		}

		public static DataColumn operator -(DataColumn c1)
		{
			DataColumn c3;

			if(c1.vop_Minus(out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator minus to " + c1.ToString() + " (" + c1.GetType() + ")");
		}

		public static DataColumn operator !(DataColumn c1)
		{
			DataColumn c3;

			if(c1.vop_Not(out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator not to " + c1.ToString() + " (" + c1.GetType() + ")");
		}

		public static DataColumn operator ~(DataColumn c1)
		{
			DataColumn c3;

			if(c1.vop_Complement(out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator complement to " + c1.ToString() + " (" + c1.GetType() + ")");
		}

		public static DataColumn operator ++(DataColumn c1)
		{
			DataColumn c3;

			if(c1.vop_Increment(out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator increment to " + c1.ToString() + " (" + c1.GetType() + ")");
		}

		public static DataColumn operator --(DataColumn c1)
		{
			DataColumn c3;

			if(c1.vop_Decrement(out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator decrement to " + c1.ToString() + " (" + c1.GetType() + ")");
		}

		public static bool operator true (DataColumn c1)
		{
			bool c3;

			if(c1.vop_True(out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator TRUE to " + c1.ToString() + " (" + c1.GetType() + ")");
		}

		public static bool operator false (DataColumn c1)
		{
			bool c3;

			if(c1.vop_False(out c3))
				return c3;

			throw new AltaxoOperatorException("Error: Try to apply operator FALSE to " + c1.ToString() + " (" + c1.GetType() + ")");
		}

	} // end of class Altaxo.Data.DataColumn
	

	

}
