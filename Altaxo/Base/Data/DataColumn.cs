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

#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Altaxo.Data
{
  /// <summary>
  /// This is the base class of all data columns in Altaxo. This base class provides readable, writeable
  /// columns with a defined count.
  /// </summary>
  public abstract class DataColumn :
    Main.SuspendableDocumentLeafNodeWithEventArgs,
    Main.IEventIndicatedDisposable,
    IReadableColumn,
    IWriteableColumn,
    IList<AltaxoVariant>,
    ICloneable
  {
    /// <summary>If the capacity of the column is not enough, a new array is aquired, with the new size
    /// newSize = addSpace+increaseFactor*oldSize.</summary>
    protected static double _increaseFactor = 2; // array space is increased by this factor plus addSpace

    /// <summary>If the capacity of the column is not enough, a new array is aquired, with the new size
    /// newSize = addSpace+increaseFactor*oldSize.</summary>
    protected static int _addSpace = 32; // array space is increased by multiplying with increasefactor + addspase

    #region Serialization

    /// <summary>
    /// This class is responsible for the serialization of the DataColumn (version 0).
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Altaxo.Data.DataColumn), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <summary>Serializes the DataColumn given by object obj.</summary>
      /// <param name="obj">The <see cref="DataColumn"/> instance which should be serialized.</param>
      /// <param name="info">The serialization info.</param>
      /// <remarks>I decided _not_ to serialize the parent object, because there are situations were we
      /// only want to serialize this column. But if we also serialize the parent table, we end up serializing all the object graph.
      /// </remarks>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (Altaxo.Data.DataColumn)obj;
      }

      /// <summary>
      /// Deserializes the <see cref="DataColumn"/> instance.
      /// </summary>
      /// <param name="o">The empty DataColumn instance, created by the runtime.</param>
      /// <param name="info">Serialization info.</param>
      /// <param name="parent">The parental object.</param>
      /// <returns>The deserialized object.</returns>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (Altaxo.Data.DataColumn?)o ?? throw new InvalidProgramException($"Expected {typeof(Altaxo.Data.DataColumn)}, but actually got {o}");
        // s.m_Table = (Altaxo.Data.DataTable)(info.GetValue("Parent",typeof(Altaxo.Data.DataTable)));
        return s;
      }
    }

    #endregion Serialization

    #region Abstract functions

    /// <summary>
    /// Creates a cloned instance of this object.
    /// </summary>
    /// <returns>The cloned instance of this object.</returns>
    public abstract object Clone();

    /// <summary>
    /// Gets the type of the colum's items.
    /// </summary>
    /// <value>
    /// The type of the item.
    /// </value>
    public abstract Type ItemType { get; }

    /// <summary>
    /// Returns the row count, i.e. the one more than the index to the last valid data element in the column.
    /// </summary>
    public abstract int Count { get; }

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
    /// Removes a number of rows (given by <paramref name="nCount"/>) beginning from nFirstRow,
    /// i.e. remove rows number nFirstRow ... nFirstRow+nCount-1.
    /// </summary>
    /// <param name="nFirstRow">Number of first row to delete.</param>
    /// <param name="nCount">Number of rows to delete.</param>
    public abstract void RemoveRows(int nFirstRow, int nCount); // removes nCount rows starting from nFirstRow

    /// <summary>
    /// Cuts the column to the provided length. The actual length of the column may be less than the provided parameter if the column contains empty elements.
    /// </summary>
    /// <param name="nCount">The maximum number of elements the column contains after this operation.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If <paramref name="nCount"/>is negative.</exception>
    public virtual void CutToMaximumLength(int nCount)
    {
      if (nCount < 0)
        throw new ArgumentOutOfRangeException("nCount must not be less than 0");

      var count = Count;
      if (Count > nCount)
      {
        RemoveRows(nCount, count - nCount);
      }
    }

    /// <summary>
    /// Inserts <paramref name="nCount"/> empty rows before row number <paramref name="nBeforeRow"/>.
    /// </summary>
    /// <param name="nBeforeRow">The row number before the additional rows are inserted.</param>
    /// <param name="nCount">Number of empty rows to insert.</param>
    public abstract void InsertRows(int nBeforeRow, int nCount); // inserts additional empty rows

    /// <summary>
    /// Copies the contents of another column o to this column. An exception should be thrown if the data types of
    /// both columns are incompatible.
    /// </summary>
    /// <param name="o">The column the data will be copied from.</param>
    public abstract void CopyDataFrom(object o);

    /// <summary>
    /// Returns the type of the associated ColumnStyle for use in a worksheet view.</summary>
    /// <returns>The type of the associated <see cref="Worksheet.ColumnStyle"/> class.</returns>
    /// <remarks>
    /// If this type of data column is not used in a datagrid, you can return null for this type.
    /// </remarks>
    // TODO: reimplement this using attributes in the style class
    public abstract System.Type? GetColumnStyleType();

    #endregion Abstract functions

    #region Construction/Disposal/Name/Parent

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
        return IsDisposeInProgress ? string.Empty : Main.AbsoluteDocumentPath.GetPathString(this, 3);
      }
    }

    /// <summary>
    /// Returns the column type followed by a backslash and the column name.
    /// </summary>
    public string TypeAndName
    {
      get
      {
        return TryGetName(out var name)  ? GetType().ToString() + "(\"" + name + "\")" : GetType().ToString() ;
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
    /// Get / sets the parent object of this data column.
    /// </summary>
    /// <remarks>Normally the parent object is a <see cref="DataColumnCollection" />. In this case this member is set during addition of the data column
    /// to the collection. If some other object owns the data column, it is responsible for setting the parent to himself.</remarks>
    public override Main.IDocumentNode? ParentObject
    {
      get { return _parent; }
      set
      {
        var oldParent = _parent;
        base.ParentObject = value;

        if (!object.ReferenceEquals(oldParent, _parent))
        {
          if (oldParent is Main.IChildChangedEventSink oldSink)
            oldSink.EhChildChanged(this, new Main.ParentChangedEventArgs(oldParent, _parent));
          if (_parent is Main.IChildChangedEventSink)
            _parent.EhChildChanged(this, new Main.ParentChangedEventArgs(oldParent, _parent));
        }
      }
    }

    #endregion Construction/Disposal/Name/Parent

    #region Suspend/Resume/Dirty

    /// <summary>
    /// Accumulates the change data of the child. Currently only a flag is set to signal that the table has changed.
    /// </summary>
    /// <param name="sender">The sender of the change notification (currently unused).</param>
    /// <param name="e">The change event args can provide details of the change (currently unused).</param>
    protected override void AccumulateChangeData(object? sender, EventArgs e)
    {
      var ea = e as DataColumnChangedEventArgs;
      if (ea is null)
        throw new ArgumentException("ChangeEventArgs expected in argument e");

      if (_accumulatedEventData is null)
        _accumulatedEventData = ea;
      else
        ((DataColumnChangedEventArgs)_accumulatedEventData).Accumulate(ea.MinRowChanged, ea.MaxRowChanged, ea.HasRowCountDecreased);
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
      if (_accumulatedEventData is null)
        _accumulatedEventData = new DataColumnChangedEventArgs(minRow, maxRow, rowCountDecreased);
      else
        ((DataColumnChangedEventArgs)_accumulatedEventData).Accumulate(minRow, maxRow, rowCountDecreased); // AccumulateNotificationData
    }

    /// <summary>
    /// This function has to be called by derived classes if any changes in their data occur.
    /// </summary>
    /// <param name="minRow">The minimum row number that changed.</param>
    /// <param name="maxRow">The maximum row number that changed plus 1.</param>
    /// <param name="rowCountDecreased">If true, the row count has decreased during the data change.</param>
    protected void EhSelfChanged(int minRow, int maxRow, bool rowCountDecreased)
    {
      if (!IsSomeoneListeningToChanges) // special optimization because DataColumns is often used without any parent
        return; // nobody is listening

      if (!IsSuspended)
      {
        var e = new DataColumnChangedEventArgs(minRow, maxRow, rowCountDecreased);
        // Notify parent
        if (_parent is Main.IChildChangedEventSink)
        {
          _parent.EhChildChanged(this, e); // parent may change our suspend state
        }

        if (!IsSuspended)
        {
          OnChanged(e); // Fire change event
          return;
        }
      }

      // at this point we are suspended for sure, or resume is still in progress
      AccumulateChangeData(minRow, maxRow, rowCountDecreased);  // child is unable to accumulate change data, we have to to it by ourself
    }

    /// <summary>
    /// Column is dirty if either there are new changed or deleted rows, and no data changed event was fired for notification.
    /// This value is reseted after the data changed event has notified the change.
    /// </summary>
    public bool IsDirty
    {
      get
      {
        return _accumulatedEventData is not null;
      }
    }

    #endregion Suspend/Resume/Dirty

    #region Data Access/Append/Clear

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
        SetValueAt(i, value);
      }
    }

    /// <summary>
    /// Provides a setter property to which another data column can be assigned to. Copies all elements of the other DataColumn to this column. An exception is thrown if the data types of both columns are incompatible.
    /// See also <see cref="CopyDataFrom"/>.</summary>
    public object Data
    {
      set { CopyDataFrom(value); }
    }

    /// <summary>
    /// Appends data from another column to this column
    /// </summary>
    /// <param name="v">Column to append.</param>
    public virtual void Append(Altaxo.Data.DataColumn v)
    {
      AppendToPosition(v, Count);
    }

    /// <summary>
    /// Appends data from another column to this column
    /// </summary>
    /// <param name="v">Column to append.</param>
    /// <param name="startingRow">Row where the first data item is copied to.</param>
    public virtual void AppendToPosition(Altaxo.Data.DataColumn v, int startingRow)
    {
      using (var suspendToken = SuspendGetToken())
      {
        for (int i = startingRow + v.Count - 1, j = v.Count - 1; j >= 0; i--, j--)
          this[i] = v[j];
      }
    }

    /// <summary>
    /// Clears all rows.
    /// </summary>
    public void Clear()
    {
      RemoveRows(0, Count);
    }

    #endregion Data Access/Append/Clear

    #region Very special copy operations

    /// <summary>
    /// Creates a new column, consisting only of the selected rows of the original column <c>x</c>. If x is null, a new <see cref="Altaxo.Data.DoubleColumn" /> will
    /// be returned, consisting of the selected row indices.
    /// </summary>
    /// <param name="x">The original column (can be null).</param>
    /// <param name="selectedRows">Selected row indices (can be null - then the entire column is used).</param>
    /// <param name="numrows">Number of rows to create. Must not more than contained in selectedRows.</param>
    /// <returns>A freshly created column consisting of x at the selected indices, or of the indices itself if x was null.</returns>
    public static DataColumn CreateColumnOfSelectedRows(Altaxo.Data.DataColumn? x, Altaxo.Collections.IAscendingIntegerCollection? selectedRows, int numrows)
    {
      Altaxo.Data.DataColumn result;
      if (x is not null)
      {
        result = (Altaxo.Data.DataColumn)x.Clone();
        result.Clear();
        for (int j = 0; j < numrows; j++)
        {
          int rowidx = selectedRows is not null ? selectedRows[j] : j;
          result[j] = x[rowidx];
        }
      }
      else // x is null
      {
        result = new Altaxo.Data.DoubleColumn();
        for (int j = 0; j < numrows; j++)
        {
          int rowidx = selectedRows is not null ? selectedRows[j] : j;
          result[j] = rowidx;
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
    public static DataColumn CreateColumnOfSelectedRows(Altaxo.Data.DataColumn? x, Altaxo.Collections.IAscendingIntegerCollection? selectedRows)
    {
      int numrows = 0;
      if (selectedRows is not null)
        numrows = selectedRows.Count;
      else if (x is not null)
        numrows = x.Count;

      return CreateColumnOfSelectedRows(x, selectedRows, numrows);
    }

    /// <summary>
    /// Creates a new column, consisting only of the selected rows of the original column.
    /// </summary>
    /// <param name="selectedRows">Selected row indices (can be null - then the entire column is used).</param>
    /// <returns>A freshly created column consisting of x at the selected indices.</returns>
    public DataColumn CreateColumnOfSelectedRows(Altaxo.Collections.IAscendingIntegerCollection? selectedRows)
    {
      return CreateColumnOfSelectedRows(this, selectedRows);
    }

    #endregion Very special copy operations

    #region Interface implementations

    #region IList<AltaxoVariant> Members

    public int IndexOf(AltaxoVariant item)
    {
      for (int i = 0; i < Count; i++)
        if (GetVariantAt(i) == item)
          return i;
      return -1;
    }

    /// <summary>
    /// Gets all indices where the element is equal to the value given in <paramref name="item"/>.
    /// </summary>
    /// <param name="item">The value to compare with</param>
    /// <returns>All indices where the element is equal to the value given in <paramref name="item"/>.</returns>
    public IEnumerable<int> IndicesOf(AltaxoVariant item)
    {
      for (int i = 0; i < Count; i++)
        if (GetVariantAt(i) == item)
          yield return i;
    }

    public void Insert(int index, AltaxoVariant item)
    {
      InsertRows(index, 1);
      SetValueAt(index, item);
    }

    public void RemoveAt(int index)
    {
      RemoveRows(index, 1);
    }

    #endregion IList<AltaxoVariant> Members

    #region ICollection<AltaxoVariant> Members

    public void Add(AltaxoVariant item)
    {
      SetValueAt(Count, item);
    }

    public bool Contains(AltaxoVariant item)
    {
      return IndexOf(item) >= 0;
    }

    public void CopyTo(AltaxoVariant[] array, int arrayIndex)
    {
      if (0 == Count)
        return;

      if (array is null)
        throw new ArgumentNullException("array");
      if (!((arrayIndex + Count) < array.Length))
        throw new ArgumentException("array to short");
      for (int i = 0; i < Count; i++)
        array[arrayIndex + i] = GetVariantAt(i);
    }

    public bool IsReadOnly
    {
      get { return false; }
    }

    public bool Remove(AltaxoVariant item)
    {
      int index = IndexOf(item);
      if (index >= 0)
        RemoveAt(index);
      return index >= 0;
    }

    #endregion ICollection<AltaxoVariant> Members

    #region IEnumerable<AltaxoVariant> Members

    public IEnumerator<AltaxoVariant> GetEnumerator()
    {
      for (int i = 0; i < Count; i++)
        yield return GetVariantAt(i);
    }

    #endregion IEnumerable<AltaxoVariant> Members

    #region IEnumerable Members

    IEnumerator IEnumerable.GetEnumerator()
    {
      for (int i = 0; i < Count; i++)
        yield return GetVariantAt(i);
    }

    #endregion IEnumerable Members

    #endregion Interface implementations

    #region Converters

    public static DataColumn CreateInstanceOfType(Type type)
    {
      if (type is null) throw new ArgumentNullException(nameof(type));
      if (!typeof(DataColumn).IsAssignableFrom(type))
        throw new ArgumentException($"Provided type must derive from {nameof(DataColumn)}");
      return (DataColumn)(Activator.CreateInstance(type) ?? throw new InvalidProgramException($"Can not create instance of type {type}. Is a public constructor missing?"));
    }


    /// <summary>
    /// Provides a setter property to which a vector can be assigned to. Copies all elements of the vector to this column.
    /// </summary>
    public virtual IReadOnlyList<double> AssignVector
    {
      set
      {
        throw new ArithmeticException(string.Format("Column {0} is a {1} and can thus not be converted to IROVector", Name, GetType()));
      }
    }

    int? IReadableColumn.Count
    {
      get
      {
        return Count;
      }
    }

    /// <summary>
    /// Creates a wrapper that implements IVector with starting index = 0 and number of elements = Count.
    /// </summary>
    /// <returns></returns>
    public virtual Altaxo.Calc.LinearAlgebra.IVector<double> ToVector()
    {
      return ToVector(0, Count);
    }

    /// <summary>
    /// Creates a wrapper that implements IROVector with starting index = 0 and number of elements = Count.
    /// </summary>
    /// <returns></returns>
    public virtual Altaxo.Calc.LinearAlgebra.IROVector<double> ToROVector()
    {
      return ToROVector(0, Count);
    }

    /// <summary>
    /// Creates a wrapper that implements IVector.
    /// </summary>
    /// <param name="start">Index of first element of this DataColumn, where the wrapper vector starts with.</param>
    /// <param name="count">Number of elements of the wrapper vector.</param>
    /// <returns>The wrapper vector.</returns>
    public virtual Altaxo.Calc.LinearAlgebra.IVector<double> ToVector(int start, int count)
    {
      throw new ArithmeticException(string.Format("Column {0} is a {1} and can thus not be converted to IVector", Name, GetType()));
    }

    /// <summary>
    /// Creates a wrapper that implements IROVector.
    /// </summary>
    /// <param name="start">Index of first element of this DataColumn, where the wrapper vector starts with.</param>
    /// <param name="count">Number of elements of the wrapper vector.</param>
    /// <returns>The wrapper vector.</returns>
    public virtual Altaxo.Calc.LinearAlgebra.IROVector<double> ToROVector(int start, int count)
    {
      throw new ArithmeticException(string.Format("Column {0} is a {1} and can thus not be converted to IROVector", Name, GetType()));
    }

    #endregion Converters

    #region Operators

    /// <summary>
    /// Adds another data column to this data column (item by item).
    /// </summary>
    /// <param name="a">The data column to add.</param>
    /// <param name="b">The result of the addition (this+a).</param>
    /// <returns>True if successful, false if this operation is not supported.</returns>
    public virtual bool vop_Addition(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_Addition_Rev(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_Addition(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_Addition_Rev(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    /// <summary>
    /// Subtracts another data column from this data column (item by item).
    /// </summary>
    /// <param name="a">The data column to subtract.</param>
    /// <param name="b">The result of the subtraction (this-a).</param>
    /// <returns>True if successful, false if this operation is not supported.</returns>
    public virtual bool vop_Subtraction(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_Subtraction_Rev(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_Subtraction(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_Subtraction_Rev(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    /// <summary>
    /// Multiplies another data column to this data column (item by item).
    /// </summary>
    /// <param name="a">The data column to multiply.</param>
    /// <param name="b">The result of the multiplication (this*a).</param>
    /// <returns>True if successful, false if this operation is not supported.</returns>
    public virtual bool vop_Multiplication(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_Multiplication_Rev(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_Multiplication(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_Multiplication_Rev(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    /// <summary>
    /// Divides this data column by another data column(item by item).
    /// </summary>
    /// <param name="a">The data column used for division.</param>
    /// <param name="b">The result of the division (this/a).</param>
    /// <returns>True if successful, false if this operation is not supported.</returns>
    public virtual bool vop_Division(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_Division_Rev(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_Division(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_Division_Rev(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_Modulo(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_Modulo_Rev(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_Modulo(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_Modulo_Rev(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_And(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_And_Rev(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_And(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_And_Rev(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_Or(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_Or_Rev(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_Or(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_Or_Rev(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_Xor(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_Xor_Rev(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_Xor(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_Xor_Rev(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_ShiftLeft(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_ShiftLeft_Rev(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_ShiftLeft(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_ShiftLeft_Rev(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_ShiftRight(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_ShiftRight_Rev(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_ShiftRight(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_ShiftRight_Rev(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_Lesser(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_Lesser_Rev(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_Lesser(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_Lesser_Rev(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_Greater(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_Greater_Rev(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_Greater(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_Greater_Rev(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_LesserOrEqual(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_LesserOrEqual_Rev(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_LesserOrEqual(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_LesserOrEqual_Rev(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_GreaterOrEqual(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_GreaterOrEqual_Rev(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_GreaterOrEqual(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_GreaterOrEqual_Rev(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    // Unary operators
    public virtual bool vop_Plus([MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_Minus([MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_Not([MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_Complement([MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_Increment([MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_Decrement([MaybeNullWhen(false)] out DataColumn b)
    { b = null; return false; }

    public virtual bool vop_True(out bool b)
    { b = false; return false; }

    public virtual bool vop_False(out bool b)
    { b = false; return false; }

    public static DataColumn operator +(DataColumn c1, DataColumn c2)
    {

      if (c1.vop_Addition(c2, out var c3))
        return c3;
      if (c2.vop_Addition_Rev(c1, out c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to add " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    public static DataColumn operator +(DataColumn c1, AltaxoVariant c2)
    {

      if (c1.vop_Addition(c2, out var c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to add " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    public static DataColumn operator +(AltaxoVariant c1, DataColumn c2)
    {

      if (c2.vop_Addition_Rev(c1, out var c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to add " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    public static DataColumn operator -(DataColumn c1, DataColumn c2)
    {

      if (c1.vop_Subtraction(c2, out var c3))
        return c3;
      if (c2.vop_Subtraction_Rev(c1, out c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to subtract " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    public static DataColumn operator -(DataColumn c1, AltaxoVariant c2)
    {

      if (c1.vop_Subtraction(c2, out var c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to subtract " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    public static DataColumn operator -(AltaxoVariant c1, DataColumn c2)
    {

      if (c2.vop_Subtraction_Rev(c1, out var c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to subtract " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    public static DataColumn operator *(DataColumn c1, DataColumn c2)
    {

      if (c1.vop_Multiplication(c2, out var c3))
        return c3;
      if (c2.vop_Multiplication_Rev(c1, out c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to multiply " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    public static DataColumn operator *(DataColumn c1, AltaxoVariant c2)
    {

      if (c1.vop_Multiplication(c2, out var c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to multiply " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    public static DataColumn operator *(AltaxoVariant c1, DataColumn c2)
    {

      if (c2.vop_Multiplication_Rev(c1, out var c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to multiply " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    public static DataColumn operator /(DataColumn c1, DataColumn c2)
    {

      if (c1.vop_Division(c2, out var c3))
        return c3;
      if (c2.vop_Division_Rev(c1, out c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to divide " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    public static DataColumn operator /(DataColumn c1, AltaxoVariant c2)
    {

      if (c1.vop_Division(c2, out var c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to divide " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    public static DataColumn operator /(AltaxoVariant c1, DataColumn c2)
    {

      if (c2.vop_Division_Rev(c1, out var c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to divide " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    public static DataColumn operator %(DataColumn c1, DataColumn c2)
    {

      if (c1.vop_Modulo(c2, out var c3))
        return c3;
      if (c2.vop_Modulo_Rev(c1, out c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to take modulus of " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    public static DataColumn operator %(DataColumn c1, AltaxoVariant c2)
    {

      if (c1.vop_Modulo(c2, out var c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to take modulus of " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    public static DataColumn operator %(AltaxoVariant c1, DataColumn c2)
    {

      if (c2.vop_Modulo_Rev(c1, out var c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to take modulus of " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    public static DataColumn operator &(DataColumn c1, DataColumn c2)
    {

      if (c1.vop_And(c2, out var c3))
        return c3;
      if (c2.vop_And_Rev(c1, out c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply and operator to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    public static DataColumn operator &(DataColumn c1, AltaxoVariant c2)
    {

      if (c1.vop_And(c2, out var c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply operator AND to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    public static DataColumn operator &(AltaxoVariant c1, DataColumn c2)
    {

      if (c2.vop_And_Rev(c1, out var c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply operator AND to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    public static DataColumn operator |(DataColumn c1, DataColumn c2)
    {

      if (c1.vop_Or(c2, out var c3))
        return c3;
      if (c2.vop_Or_Rev(c1, out c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply or operator to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    public static DataColumn operator |(DataColumn c1, AltaxoVariant c2)
    {

      if (c1.vop_Or(c2, out var c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply operator OR to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    public static DataColumn operator |(AltaxoVariant c1, DataColumn c2)
    {

      if (c2.vop_Or_Rev(c1, out var c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply operator OR to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    public static DataColumn operator ^(DataColumn c1, DataColumn c2)
    {

      if (c1.vop_Xor(c2, out var c3))
        return c3;
      if (c2.vop_Xor_Rev(c1, out c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply xor operator to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    public static DataColumn operator ^(DataColumn c1, AltaxoVariant c2)
    {

      if (c1.vop_Xor(c2, out var c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply operator XOR to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    public static DataColumn operator ^(AltaxoVariant c1, DataColumn c2)
    {

      if (c2.vop_Xor_Rev(c1, out var c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply operator XOR to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    public static DataColumn operator <<(DataColumn c1, int c2)
    {

      if (c1.vop_ShiftLeft(c2, out var c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply operator << to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    public static DataColumn operator >>(DataColumn c1, int c2)
    {

      if (c1.vop_ShiftRight(c2, out var c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply operator >> to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    public static DataColumn operator <(DataColumn c1, DataColumn c2)
    {

      if (c1.vop_Lesser(c2, out var c3))
        return c3;
      if (c2.vop_Lesser_Rev(c1, out c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply operator lesser to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    public static DataColumn operator <(DataColumn c1, AltaxoVariant c2)
    {

      if (c1.vop_Lesser(c2, out var c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply operator < to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    public static DataColumn operator <(AltaxoVariant c1, DataColumn c2)
    {

      if (c2.vop_Lesser_Rev(c1, out var c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply operator < to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    public static DataColumn operator >(DataColumn c1, DataColumn c2)
    {

      if (c1.vop_Greater(c2, out var c3))
        return c3;
      if (c2.vop_Greater_Rev(c1, out c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply operator greater to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    public static DataColumn operator >(DataColumn c1, AltaxoVariant c2)
    {

      if (c1.vop_Greater(c2, out var c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply operator > to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    public static DataColumn operator >(AltaxoVariant c1, DataColumn c2)
    {

      if (c2.vop_Greater_Rev(c1, out var c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply operator > to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    public static DataColumn operator <=(DataColumn c1, DataColumn c2)
    {

      if (c1.vop_LesserOrEqual(c2, out var c3))
        return c3;
      if (c2.vop_LesserOrEqual_Rev(c1, out c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply operator LesserOrEqual to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    public static DataColumn operator <=(DataColumn c1, AltaxoVariant c2)
    {

      if (c1.vop_LesserOrEqual(c2, out var c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply operator <= to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    public static DataColumn operator <=(AltaxoVariant c1, DataColumn c2)
    {

      if (c2.vop_LesserOrEqual_Rev(c1, out var c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply operator <= " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    public static DataColumn operator >=(DataColumn c1, DataColumn c2)
    {

      if (c1.vop_GreaterOrEqual(c2, out var c3))
        return c3;
      if (c2.vop_GreaterOrEqual_Rev(c1, out c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply operator GreaterOrEqual to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    public static DataColumn operator >=(DataColumn c1, AltaxoVariant c2)
    {

      if (c1.vop_GreaterOrEqual(c2, out var c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply operator >= to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    public static DataColumn operator >=(AltaxoVariant c1, DataColumn c2)
    {

      if (c2.vop_GreaterOrEqual_Rev(c1, out var c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply operator >= to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    public static DataColumn operator +(DataColumn c1)
    {

      if (c1.vop_Plus(out var c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply operator plus to " + c1.ToString() + " (" + c1.GetType() + ")");
    }

    public static DataColumn operator -(DataColumn c1)
    {

      if (c1.vop_Minus(out var c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply operator minus to " + c1.ToString() + " (" + c1.GetType() + ")");
    }

    public static DataColumn operator !(DataColumn c1)
    {

      if (c1.vop_Not(out var c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply operator not to " + c1.ToString() + " (" + c1.GetType() + ")");
    }

    public static DataColumn operator ~(DataColumn c1)
    {

      if (c1.vop_Complement(out var c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply operator complement to " + c1.ToString() + " (" + c1.GetType() + ")");
    }

    public static DataColumn operator ++(DataColumn c1)
    {

      if (c1.vop_Increment(out var c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply operator increment to " + c1.ToString() + " (" + c1.GetType() + ")");
    }

    public static DataColumn operator --(DataColumn c1)
    {

      if (c1.vop_Decrement(out var c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply operator decrement to " + c1.ToString() + " (" + c1.GetType() + ")");
    }

    public static bool operator true(DataColumn c1)
    {

      if (c1.vop_True(out var c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply operator TRUE to " + c1.ToString() + " (" + c1.GetType() + ")");
    }

    public static bool operator false(DataColumn c1)
    {

      if (c1.vop_False(out var c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply operator FALSE to " + c1.ToString() + " (" + c1.GetType() + ")");
    }

    #endregion Operators
  } // end of class Altaxo.Data.DataColumn
}
