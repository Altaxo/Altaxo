#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion


using System;
using Altaxo.Serialization;

namespace Altaxo.Data
{

  /// <summary>
  /// This is the base class of all data columns in Altaxo. This base class provides readable, writeable 
  /// columns with a defined count.
  /// </summary>
  [SerializationSurrogate(0,typeof(Altaxo.Data.DataColumn.SerializationSurrogate0))]
  [SerializationVersion(0)]
  [Serializable()]
  public abstract class DataColumn :
    Main.IEventIndicatedDisposable,    
    System.Runtime.Serialization.ISerializable,
    System.Runtime.Serialization.IDeserializationCallback, 
    IReadableColumn, 
    IWriteableColumn, 
    IDefinedCount,
    ICloneable,
    Altaxo.Main.IDocumentNode,    
    Altaxo.Main.IChangedEventSource,
    Main.ISuspendable
  {
    
  

    /// <summary>
    /// The parent table this column belongs to.
    /// </summary>
    [NonSerialized]
    protected object m_Parent=null;

    /// <summary>If the capacity of the column is not enough, a new array is aquired, with the new size
    /// newSize = addSpace+increaseFactor*oldSize.</summary>
    protected static double increaseFactor=2; // array space is increased by this factor plus addSpace
    /// <summary>If the capacity of the column is not enough, a new array is aquired, with the new size
    /// newSize = addSpace+increaseFactor*oldSize.</summary>
    protected static int    addSpace=32; // array space is increased by multiplying with increasefactor + addspase

      
    /// <summary>Counter of how many suspends to data change event notifications are pending.</summary>
    /// <remarks>If this counter is zero, then every change to a element of this column fires a data change event. Applications doing a lot of changes at once can
    /// suspend this events for a better performance by calling <see cref="Suspend"/>. After finishing the application has to
    /// call <see cref="Resume"/></remarks>
    protected int  m_SuspendCount=0;

    /// <summary>
    /// Used to accumulate change data
    /// </summary>
    protected ChangeEventArgs m_ChangeData;


    
    /// <summary>This element is fired when the column is to be disposed.</summary><remarks>All instances, which have a reference
    /// to this column, should have a wire to this event. In case the event is fired, it indicates
    /// that the column should be disposed, so they have to unreference this column by setting the
    /// reference to null.
    /// </remarks>
    public event EventHandler Disposed;

  
    /// <summary>
    /// This event is fired if the data of the column or anything else changed.
    /// </summary>
    public event System.EventHandler Changed;


    #region ChangeEventArgs

    /// <summary>
    /// Stores the accumulated change data of a column.
    /// </summary>
    public class ChangeEventArgs : System.EventArgs
    {
      /// <summary>Lower bound of the area of rows, which changed during the data change event off period.</summary>
      protected int m_MinRowChanged;
      /// <summary>Upper bound (plus one) of the area of rows, which changed during the data change event off period. This in in the (plus one) convention,
      /// i.e. the value of this member is the maximum row number that changed plus one.</summary>
      protected int m_MaxRowChanged;
      /// <summary>Indicates, if the row count decreased during the data change event off period. In this case it is neccessary
      /// to recalculate the row count of the table, since it is possible that the table row count also decreased in this case.</summary>
      protected bool m_RowCountDecreased; // true if during event switch of period, the row m_Count  of this column decreases 

    

      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="minRow">Lower bound of the area of rows, which changed-</param>
      /// <param name="maxRow">Upper bound (plus one) of the area of rows, which changed.</param>
      /// <param name="rowCountDecreased">Indicates, if the row count decreased during the data change.</param>
      public ChangeEventArgs(int minRow, int maxRow, bool rowCountDecreased)
      {
        m_MinRowChanged = minRow;
        m_MaxRowChanged = maxRow;
        m_RowCountDecreased = rowCountDecreased;
      }

      /// <summary>
      /// Accumulates further data changes of a column into a already created object.
      /// </summary>
      /// <param name="minRow">Lower bound of the area of rows, which changed-</param>
      /// <param name="maxRow">Upper bound (plus one) of the area of rows, which changed.</param>
      /// <param name="rowCountDecreased">Indicates, if the row count decreased during the data change.</param>
      public void Accumulate(int minRow, int maxRow, bool rowCountDecreased)
      {
        if(minRow < m_MinRowChanged)
          m_MinRowChanged = minRow;
        if(maxRow > m_MaxRowChanged) 
          m_MaxRowChanged = maxRow;
        
        m_RowCountDecreased |= rowCountDecreased;
      }

      /// <summary>Lower bound of the area of rows, which changed during the data change event off period.</summary>
      public int MinRowChanged
      {
        get { return m_MinRowChanged; }
      }
      
      /// <summary>Upper bound (plus one) of the area of rows, which changed during the data change event off period. This in in the (plus one) convention,
      /// i.e. the value of this member is the maximum row number that changed plus one.</summary>
      public int MaxRowChanged
      {
        get { return m_MaxRowChanged; }
      }
  
      /// <summary>Indicates, if the row count decreased during the data change event off period. In this case it is neccessary
      /// to recalculate the row count of the table, since it is possible that the table row count also decreased in this case.</summary>
      public bool RowCountDecreased
      {
        get { return m_RowCountDecreased; }
      }
    }
    

    #endregion

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
      public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context  )
      {
        Altaxo.Data.DataColumn s = (Altaxo.Data.DataColumn)obj;
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

        // set the helper data
        return s;
      }
    }


    /// <summary>
    /// This class is responsible for the serialization of the DataColumn (version 0).
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Altaxo.Data.DataColumn),0)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <summary>Serializes the DataColumn given by object obj.</summary>
      /// <param name="obj">The <see cref="DataColumn"/> instance which should be serialized.</param>
      /// <param name="info">The serialization info.</param>
      /// <remarks>I decided _not_ to serialize the parent object, because there are situations were we
      /// only want to serialize this column. But if we also serialize the parent table, we end up serializing all the object graph.
      /// </remarks>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info )
      {
        Altaxo.Data.DataColumn s = (Altaxo.Data.DataColumn)obj;
      }

      /// <summary>
      /// Deserializes the <see cref="DataColumn"/> instance.
      /// </summary>
      /// <param name="o">The empty DataColumn instance, created by the runtime.</param>
      /// <param name="info">Serialization info.</param>
      /// <param name="parent">The parental object.</param>
      /// <returns>The deserialized object.</returns>
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        Altaxo.Data.DataColumn s = (Altaxo.Data.DataColumn)o;
        // s.m_Table = (Altaxo.Data.DataTable)(info.GetValue("Parent",typeof(Altaxo.Data.DataTable)));
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

    /// <summary>
    /// Special deserialization constructor (supports ISerializable).
    /// </summary>
    /// <param name="info">The serialization info.</param>
    /// <param name="context">The serialization context.</param>
    protected DataColumn(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
      SetObjectData(this,info,context,null);
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
      return new SerializationSurrogate0().SetObjectData(this,info,context,null);
    }

    /// <summary>Serializes the DataColumn given by object obj.</summary>
    /// <param name="info">The serialization info.</param>
    /// <param name="context">The streaming context.</param>
    /// <remarks>I decided _not_ to serialize the parent object, because there are situations were we
    /// only want to serialize this column. But if we also serialize the parent table, we end up serializing all the object graph.
    /// </remarks>
    public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
      new SerializationSurrogate0().GetObjectData(this,info,context);
    }

    #endregion


    /// <summary>
    /// Creates a cloned instance of this object.
    /// </summary>
    /// <returns>The cloned instance of this object.</returns>
    public abstract object Clone();





    /// <summary>
    /// Get the name of the column.
    /// </summary>
    public virtual string Name
    {
      get { return m_Parent is Main.INamedObjectCollection ? ((Main.INamedObjectCollection)m_Parent).GetNameOfChildObject(this) : null; }
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
    /// A call to this function suspends data changed event notifications
    /// </summary>
    /// <remarks>If an application has
    /// to change a lot of data in a column at once, it should call this function to avoid the firing
    /// of the event every time it changes a single element. After processing all items, the application
    /// has to resume the data changed event notification by calling <see cref="Resume"/>
    /// </remarks>
    public void Suspend()
    {
      System.Diagnostics.Debug.Assert(m_SuspendCount>=0,"SuspendCount must always be greater or equal to zero");    
      m_SuspendCount++;
    }

    public bool IsSuspended 
    {
      get { return m_SuspendCount>0; }
    }

    /// <summary>
    /// This resumes the data changed notifications if the suspend counter has reached zero.
    /// </summary>
    /// <remarks>The area of changed rows is updated even in the suspend period. The suspend counter is
    /// decreased by a call to this function. If it reaches zero, the data changed event is fired,
    /// and the arguments of the handler contain the changed area of rows during the suspend time.</remarks>
    public void Resume()
    {
      System.Diagnostics.Debug.Assert(m_SuspendCount>=0,"SuspendCount must always be greater or equal to zero");    

      if(m_SuspendCount>0 && (--m_SuspendCount)==0 && m_ChangeData!=null)
      {
        if(m_Parent is Main.IChildChangedEventSink)
          ((Main.IChildChangedEventSink)m_Parent).EhChildChanged(this, m_ChangeData);

        if(!IsSuspended)
          OnDataChanged(); // Fire the changed event
      }
    }

    /// <summary>
    /// Accumulates the change data provided in the arguments into the m_ChangeData member. 
    /// </summary>
    /// <param name="minRow">The min row number that changed.</param>
    /// <param name="maxRow">The max row number that changed.</param>
    /// <param name="rowCountDecreased">True if the row count of the column decreased by the change.</param>
    /// <remarks>In case the object in which to accumulate the change data is actually null, a new change data object is created.</remarks>
    protected void AccumulateChangeData(int minRow, int maxRow, bool rowCountDecreased)
    {
      if(m_ChangeData==null)
        m_ChangeData = new DataColumn.ChangeEventArgs(minRow,maxRow,rowCountDecreased);
      else
        m_ChangeData.Accumulate(minRow,maxRow,rowCountDecreased); // AccumulateNotificationData
    }

    /// <summary>
    /// This function has to be called by derived classes if any changes in their data occur.
    /// </summary>
    /// <param name="minRow">The minimum row number that changed.</param>
    /// <param name="maxRow">The maximum row number that changed plus 1.</param>
    /// <param name="rowCountDecreased">If true, the row count has decreased during the data change.</param>
    protected void NotifyDataChanged(int minRow, int maxRow, bool rowCountDecreased)
    {
      if(m_Parent==null && Changed==null)
        return; // nobody is listening

      AccumulateChangeData(minRow, maxRow, rowCountDecreased);
    
      if(IsSuspended)
        return;

      if(m_Parent is Main.IChildChangedEventSink)
        ((Main.IChildChangedEventSink)m_Parent).EhChildChanged(this, m_ChangeData);
  
      if(!IsSuspended) // parent is not suspended
        OnDataChanged(); // Fire the changed event 
    }
  
    
  
    /// <summary>
    /// Fires the Changed event with the actual (accumulated) change data. After the firing of the event the change data are removed.
    /// </summary>
    protected virtual void OnDataChanged()
    {
      if(null!=Changed)
        Changed(this, m_ChangeData);

      m_ChangeData=null;
    }


    /// <summary>
    /// Returns the type of the associated ColumnStyle for use in a worksheet view.</summary>
    /// <returns>The type of the associated <see cref="Worksheet.ColumnStyle"/> class.</returns>
    /// <remarks>
    /// If this type of data column is not used in a datagrid, you can return null for this type.
    /// </remarks>
    // TODO: reimplement this using attributes in the style class
    public abstract System.Type GetColumnStyleType();

    /// <summary>
    /// Column is dirty if either there are new changed or deleted rows, and no data changed event was fired for notification.
    /// This value is reseted after the data changed event has notified the change.
    /// </summary>
    public bool IsDirty
    {
      get
      {
        return null!=m_ChangeData;
      }
    }

  
    /// <summary>
    /// Constructs a data column with no name associated.
    /// </summary>
    protected DataColumn()
    {
    }
    
  
    /// <summary>
    /// Returns either the column name if the column has no parent table, or the parent table name, followed by
    /// a backslash and the column name if the column has a table.
    /// </summary>
    public string FullName
    {
      get 
      {
        return Main.DocumentPath.GetPathString(this,3);
      }
    }


    /// <summary>
    /// Returns the row count, i.e. the one more than the index to the last valid data element in the column. 
    /// </summary>
    public abstract int Count { get; }
    
    /// <summary>
    /// Returns the column type followed by a backslash and the column name.
    /// </summary>
    public string TypeAndName
    {
      get
      {
        return null==this.Name ? this.GetType().ToString(): this.GetType().ToString() + "(\"" + this.Name + "\")";
      }
    }

  
    /// <summary>
    /// Get / sets the parent object of this data column.
    /// </summary>
    /// <remarks>Normally the parent object is a <see cref="DataColumnCollection" />. In this case this member is set during addition of the data column
    /// to the collection. If some other object owns the data column, it is responsible for setting the parent to himself.</remarks>
    public virtual object ParentObject
    {
      get { return m_Parent; }
      set
      {
        object oldParent = m_Parent;
        m_Parent = value;

        if(!object.ReferenceEquals(oldParent,m_Parent))
        {
          if(oldParent is Main.IChildChangedEventSink)
            ((Main.IChildChangedEventSink)oldParent).EhChildChanged(this,new Main.ParentChangedEventArgs(oldParent,m_Parent));
          if(m_Parent is Main.IChildChangedEventSink)
            ((Main.IChildChangedEventSink)m_Parent).EhChildChanged(this,new Main.ParentChangedEventArgs(oldParent,m_Parent));

        }
      }
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
    /// This clears the cell at index i.
    /// </summary>
    /// <param name="i">The index where to clean.</param>
    public abstract void SetElementEmpty(int i);


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
    /// Clears the content of the column and fires the <see cref="Disposed"/> event.
    /// </summary>
    public void Dispose()
    {
      if (null != Disposed)
        Disposed(this, EventArgs.Empty);

      this.ParentObject=null;
      this.Clear();
     
    }

    /// <summary>
    /// Clears all rows.
    /// </summary>
    public void Clear()
    {
      RemoveRows(0,this.Count);
    }

    /// <summary>
    /// Provides a setter property to which another data column can be assigned to. Copies all elements of the other DataColumn to this column. An exception is thrown if the data types of both columns are incompatible. 
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
    /// Creates a new column, consisting only of the selected rows of the original column <c>x</c>. If x is null, a new <see cref="Altaxo.Data.DoubleColumn" /> will
    /// be returned, consisting of the selected row indices. 
    /// </summary>
    /// <param name="x">The original column (can be null).</param>
    /// <param name="selectedRows">Selected row indices (can be null - then the entire column is used).</param>
    /// <param name="numrows">Number of rows to create. Must not more than contained in selectedRows.</param>
    /// <returns>A freshly created column consisting of x at the selected indices, or of the indices itself if x was null.</returns>
    public static DataColumn CreateColumnOfSelectedRows(Altaxo.Data.DataColumn x, Altaxo.Collections.IAscendingIntegerCollection selectedRows, int numrows)
    {
      Altaxo.Data.DataColumn result;
      if(x!=null)
      {
        result = (Altaxo.Data.DataColumn)x.Clone();
        result.Clear();
        for(int j=0;j<numrows;j++)
        {
          int rowidx = selectedRows!=null ? selectedRows[j] : j;
          result[j] = x[rowidx];
        }
      }
      else // x is null
      {
        result = new Altaxo.Data.DoubleColumn();
        for(int j=0;j<numrows;j++)
        {
          int rowidx = selectedRows!=null ? selectedRows[j] : j;
          result[j] = (double)rowidx;
        }
      }
      return result;
    }

    /// <summary>
    /// Creates a new column, consisting only of the selected rows of the original column <c>x</c>. If x is null, a new <see cref="Altaxo.Data.DoubleColumn" /> will
    /// be returned, consisting of the selected row indices. 
    /// </summary>
    /// <param name="x">The original column (can be null).</param>
    /// <param name="selectedRows">Selected row indices (can be null - then the entire column is used).</param>
    /// <returns>A freshly created column consisting of x at the selected indices, or of the indices itself if x was null. If both x and selectedRows are null,
    /// an empty <see cref="DoubleColumn" /> is returned.</returns>
    public static DataColumn CreateColumnOfSelectedRows(Altaxo.Data.DataColumn x, Altaxo.Collections.IAscendingIntegerCollection selectedRows)
    {
      int numrows = 0;
      if(selectedRows!=null)
        numrows = selectedRows.Count;
      else if(x!=null)
        numrows = x.Count;

      return CreateColumnOfSelectedRows(x,selectedRows,numrows);
    }

    /// <summary>
    /// Creates a new column, consisting only of the selected rows of the original column.
    /// </summary>
    /// <param name="selectedRows">Selected row indices (can be null - then the entire column is used).</param>
    /// <returns>A freshly created column consisting of x at the selected indices.</returns>
    public DataColumn CreateColumnOfSelectedRows(Altaxo.Collections.IAscendingIntegerCollection selectedRows)
    {
      return CreateColumnOfSelectedRows(this,selectedRows);
    }

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
    { b=null; return false; }

    public virtual bool vop_Addition_Rev(DataColumn a, out DataColumn b)
    { b=null; return false; }
    public virtual bool vop_Addition(AltaxoVariant a, out DataColumn b)
    { b=null; return false; }
    public virtual bool vop_Addition_Rev(AltaxoVariant a, out DataColumn b)
    { b=null; return false; }

    /// <summary>
    /// Subtracts another data column from this data column (item by item). 
    /// </summary>
    /// <param name="a">The data column to subtract.</param>
    /// <param name="b">The result of the subtraction (this-a).</param>
    /// <returns>True if successful, false if this operation is not supported.</returns>
    public virtual bool vop_Subtraction(DataColumn a, out DataColumn b)
    { b=null; return false; }
    public virtual bool vop_Subtraction_Rev(DataColumn a, out DataColumn b)
    { b=null; return false; }
    public virtual bool vop_Subtraction(AltaxoVariant a, out DataColumn b)
    { b=null; return false; }
    public virtual bool vop_Subtraction_Rev(AltaxoVariant a, out DataColumn b)
    { b=null; return false; }

    /// <summary>
    /// Multiplies another data column to this data column (item by item). 
    /// </summary>
    /// <param name="a">The data column to multiply.</param>
    /// <param name="b">The result of the multiplication (this*a).</param>
    /// <returns>True if successful, false if this operation is not supported.</returns>
    public virtual bool vop_Multiplication(DataColumn a, out DataColumn b)
    { b=null; return false; }
    public virtual bool vop_Multiplication_Rev(DataColumn a, out DataColumn b)
    { b=null; return false; }
    public virtual bool vop_Multiplication(AltaxoVariant a, out DataColumn b)
    { b=null; return false; }
    public virtual bool vop_Multiplication_Rev(AltaxoVariant a, out DataColumn b)
    { b=null; return false; }

    /// <summary>
    /// Divides this data column by another data column(item by item). 
    /// </summary>
    /// <param name="a">The data column used for division.</param>
    /// <param name="b">The result of the division (this/a).</param>
    /// <returns>True if successful, false if this operation is not supported.</returns>
    public virtual bool vop_Division(DataColumn a, out DataColumn b)
    { b=null; return false; }
    public virtual bool vop_Division_Rev(DataColumn a, out DataColumn b)
    { b=null; return false; }
    public virtual bool vop_Division(AltaxoVariant a, out DataColumn b)
    { b=null; return false; }
    public virtual bool vop_Division_Rev(AltaxoVariant a, out DataColumn b)
    { b=null; return false; }

    public virtual bool vop_Modulo(DataColumn a, out DataColumn b)
    { b=null; return false; }
    public virtual bool vop_Modulo_Rev(DataColumn a, out DataColumn b)
    { b=null; return false; }
    public virtual bool vop_Modulo(AltaxoVariant a, out DataColumn b)
    { b=null; return false; }
    public virtual bool vop_Modulo_Rev(AltaxoVariant a, out DataColumn b)
    { b=null; return false; }

    public virtual bool vop_And(DataColumn a, out DataColumn b)
    { b=null; return false; }
    public virtual bool vop_And_Rev(DataColumn a, out DataColumn b)
    { b=null; return false; }
    public virtual bool vop_And(AltaxoVariant a, out DataColumn b)
    { b=null; return false; }
    public virtual bool vop_And_Rev(AltaxoVariant a, out DataColumn b)
    { b=null; return false; }

    public virtual bool vop_Or(DataColumn a, out DataColumn b)
    { b=null; return false; }
    public virtual bool vop_Or_Rev(DataColumn a, out DataColumn b)
    { b=null; return false; }
    public virtual bool vop_Or(AltaxoVariant a, out DataColumn b)
    { b=null; return false; }
    public virtual bool vop_Or_Rev(AltaxoVariant a, out DataColumn b)
    { b=null; return false; }

    public virtual bool vop_Xor(DataColumn a, out DataColumn b)
    { b=null; return false; }
    public virtual bool vop_Xor_Rev(DataColumn a, out DataColumn b)
    { b=null; return false; }
    public virtual bool vop_Xor(AltaxoVariant a, out DataColumn b)
    { b=null; return false; }
    public virtual bool vop_Xor_Rev(AltaxoVariant a, out DataColumn b)
    { b=null; return false; }

    public virtual bool vop_ShiftLeft(DataColumn a, out DataColumn b)
    { b=null; return false; }
    public virtual bool vop_ShiftLeft_Rev(DataColumn a, out DataColumn b)
    { b=null; return false; }
    public virtual bool vop_ShiftLeft(AltaxoVariant a, out DataColumn b)
    { b=null; return false; }
    public virtual bool vop_ShiftLeft_Rev(AltaxoVariant a, out DataColumn b)
    { b=null; return false; }

    public virtual bool vop_ShiftRight(DataColumn a, out DataColumn b)
    { b=null; return false; }
    public virtual bool vop_ShiftRight_Rev(DataColumn a, out DataColumn b)
    { b=null; return false; }
    public virtual bool vop_ShiftRight(AltaxoVariant a, out DataColumn b)
    { b=null; return false; }
    public virtual bool vop_ShiftRight_Rev(AltaxoVariant a, out DataColumn b)
    { b=null; return false; }

    public virtual bool vop_Lesser(DataColumn a, out DataColumn b)
    { b=null; return false; }
    public virtual bool vop_Lesser_Rev(DataColumn a, out DataColumn b)
    { b=null; return false; }
    public virtual bool vop_Lesser(AltaxoVariant a, out DataColumn b)
    { b=null; return false; }
    public virtual bool vop_Lesser_Rev(AltaxoVariant a, out DataColumn b)
    { b=null; return false; }

    public virtual bool vop_Greater(DataColumn a, out DataColumn b)
    { b=null; return false; }
    public virtual bool vop_Greater_Rev(DataColumn a, out DataColumn b)
    { b=null; return false; }
    public virtual bool vop_Greater(AltaxoVariant a, out DataColumn b)
    { b=null; return false; }
    public virtual bool vop_Greater_Rev(AltaxoVariant a, out DataColumn b)
    { b=null; return false; }

    public virtual bool vop_LesserOrEqual(DataColumn a, out DataColumn b)
    { b=null; return false; }
    public virtual bool vop_LesserOrEqual_Rev(DataColumn a, out DataColumn b)
    { b=null; return false; }
    public virtual bool vop_LesserOrEqual(AltaxoVariant a, out DataColumn b)
    { b=null; return false; }
    public virtual bool vop_LesserOrEqual_Rev(AltaxoVariant a, out DataColumn b)
    { b=null; return false; }

    public virtual bool vop_GreaterOrEqual(DataColumn a, out DataColumn b)
    { b=null; return false; }
    public virtual bool vop_GreaterOrEqual_Rev(DataColumn a, out DataColumn b)
    { b=null; return false; }
    public virtual bool vop_GreaterOrEqual(AltaxoVariant a, out DataColumn b)
    { b=null; return false; }
    public virtual bool vop_GreaterOrEqual_Rev(AltaxoVariant a, out DataColumn b)
    { b=null; return false; }

    // Unary operators
    public virtual bool vop_Plus(out DataColumn b)
    { b=null; return false; }

    public virtual bool vop_Minus(out DataColumn b)
    { b=null; return false; }
    
    public virtual bool vop_Not(out DataColumn b)
    { b=null; return false; }
    
    public virtual bool vop_Complement(out DataColumn b)
    { b=null; return false; }
    
    public virtual bool vop_Increment(out DataColumn b)
    { b=null; return false; }
    
    public virtual bool vop_Decrement(out DataColumn b)
    { b=null; return false; }
    
    public virtual bool        vop_True(out bool b)
    { b=false; return false; }
    
    public virtual bool        vop_False(out bool b)
    { b=false; return false; }

  
    
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
