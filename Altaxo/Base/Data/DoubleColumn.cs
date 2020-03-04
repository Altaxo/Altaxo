#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

using System;
using System.Collections;
using System.Collections.Generic;

namespace Altaxo.Data
{
  /// <summary>
  /// Summary description for Altaxo.Data.DoubleColumn.
  /// </summary>
  public class DoubleColumn
    :
    Altaxo.Data.DataColumn,
    INumericColumn,
    Altaxo.Calc.LinearAlgebra.IROVector<double>
  {
    private double[] _data;
    private int _capacity; // shortcut to m_Array.Length;
    private int _count;
    public static readonly double NullValue = double.NaN;
    // private const int MaxCount = 256 * 1024 * 1024 - 8; // this is the maximum possible number of double elements in 64-bit mode currently (Framework 4.0).
    private const int MaxCount = 2147483592; // MaxCount with gcAllowVeryLargeObjects set to true (see https://docs.microsoft.com/en-us/dotnet/framework/configure-apps/file-schema/runtime/gcallowverylargeobjects-element)
    #region Overridden functions

    public override object Clone()
    {
      return new DoubleColumn(this);
    }

    public override int Count
    {
      get { return _count; }
    }

    int Calc.LinearAlgebra.IROVector<double>.Length
    {
      get { return _count; }
    }

    IEnumerator<double> IEnumerable<double>.GetEnumerator()
    {
      var length = _count;
      for (int i = 0; i < length; ++i)
        yield return this[i];
    }

    // indexers
    public override void SetValueAt(int i, AltaxoVariant val)
    {
      try
      {
        this[i] = val.ToDouble();
      }
      catch (Exception ex)
      {
        throw new ApplicationException(string.Format("Error: Try to set {0}[{1}] with the string {2}, exception: {3}", TypeAndName, i, val.ToString(), ex.Message));
      }
    }

    public override AltaxoVariant GetVariantAt(int i)
    {
      return new AltaxoVariant(this[i]);
    }

    public override bool IsElementEmpty(int i)
    {
      return i < _count ? double.IsNaN(_data[i]) : true;
    }

    public override void SetElementEmpty(int i)
    {
      if (i < _count)
        this[i] = NullValue;
    }

    public override void RemoveRows(int nDelFirstRow, int nDelCount)
    {
      if (nDelFirstRow < 0)
        throw new ArgumentException("Row number must be greater or equal 0, but was " + nDelFirstRow.ToString(), "nDelFirstRow");

      if (nDelCount <= 0)
        return; // nothing to do here, but we dont catch it

      // we must be careful, since the range to delete can be
      // above the range this column actually holds, but
      // we must handle this the right way
      int i, j;
      for (i = nDelFirstRow, j = nDelFirstRow + nDelCount; j < _count; i++, j++)
        _data[i] = _data[j];

      int prevCount = _count;
      _count = i < _count ? i : _count; // m_Count can only decrease

      if (_count != prevCount) // raise a event only if something really changed
        EhSelfChanged(nDelFirstRow, prevCount, true);
    }

    public override void InsertRows(int nInsBeforeColumn, int nInsCount)
    {
      if (nInsCount <= 0 || nInsBeforeColumn >= Count)
        return; // nothing to do

      int newlen = _count + nInsCount;
      if (newlen > _capacity)
        Realloc(newlen);

      // copy values from m_Count downto nBeforeColumn
      for (int i = _count - 1, j = newlen - 1; i >= nInsBeforeColumn; i--, j--)
        _data[j] = _data[i];

      for (int i = nInsBeforeColumn + nInsCount - 1; i >= nInsBeforeColumn; i--)
        _data[i] = NullValue;

      _count = newlen;
      EhSelfChanged(nInsBeforeColumn, _count, false);
    }

    public override void CopyDataFrom(object o)
    {
      var oldCount = _count;
      _count = 0;

      if (o is DoubleColumn)
      {
        var src = (DoubleColumn)o;
        _data = null == src._data ? null : (double[])src._data.Clone();
        _capacity = _data?.Length ?? 0;
        _count = src._count;
      }
      else
      {
        if (o is ICollection)
          Realloc((o as ICollection).Count); // Prealloc the array if count of the collection is known beforehand

        if (o is IEnumerable<double>)
        {
          var src = (IEnumerable<double>)o;
          _count = 0;
          foreach (var it in src)
          {
            if (_count >= _capacity)
              Realloc(_count);
            _data[_count++] = it;
          }
        }
        else if (o is IEnumerable<float>)
        {
          var src = (IEnumerable<float>)o;
          _count = 0;
          foreach (var it in src)
          {
            if (_count >= _capacity)
              Realloc(_count);
            _data[_count++] = it;
          }
        }
        else if (o is IEnumerable<int>)
        {
          var src = (IEnumerable<int>)o;
          _count = 0;
          foreach (var it in src)
          {
            if (_count >= _capacity)
              Realloc(_count);
            _data[_count++] = it;
          }
        }
        else if (o is IEnumerable<DateTime>)
        {
          var src = (IEnumerable<DateTime>)o;
          _count = 0;
          foreach (var it in src)
          {
            if (_count >= _capacity)
              Realloc(_count);
            _data[_count++] = it.Ticks / 1e7;
            ;
          }
        }
        else if (o is IEnumerable<AltaxoVariant>)
        {
          var src = (IEnumerable<AltaxoVariant>)o;
          _count = 0;
          foreach (var it in src)
          {
            if (_count >= _capacity)
              Realloc(_count);
            _data[_count++] = it;
          }
        }
        else
        {
          _count = 0;
          if (o == null)
            throw new ArgumentNullException("o");
          else
            throw new ArgumentException("Try to copy " + o.GetType() + " to " + GetType(), "o"); // throw exception
        }

        TrimEmptyElementsAtEnd();
      }

      if (oldCount > 0 || _count > 0) // message only if really was a change
        EhSelfChanged(0, oldCount > _count ? (oldCount) : (_count), _count < oldCount);
    }

    private void TrimEmptyElementsAtEnd()
    {
      for (; _count > 0 && IsElementEmpty(_count - 1); _count--)
        ;
    }

    public override System.Type GetColumnStyleType()
    {
      return typeof(Altaxo.Worksheet.DoubleColumnStyle);
    }

    #endregion Overridden functions

    public DoubleColumn()
    {
    }

    public DoubleColumn(int initialcapacity)
    {
      _count = 0;
      _data = new double[initialcapacity];
      _capacity = initialcapacity;
    }

    public DoubleColumn(DoubleColumn from)
    {
      _count = from._count;
      _capacity = from._capacity;
      _data = null == from._data ? null : (double[])from._data.Clone();
    }

    #region "Serialization"

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Altaxo.Data.DoubleColumn), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (Altaxo.Data.DoubleColumn)obj;
        // serialize the base class
        info.AddBaseValueEmbedded(s, typeof(Altaxo.Data.DataColumn));

        if ("true" == info.GetProperty(DataTable.SerializationInfoProperty_SaveAsTemplate))
          info.AddArray("Data", s._data, 0);
        else
          info.AddArray("Data", s._data, s._count);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        Altaxo.Data.DoubleColumn s = null != o ? (Altaxo.Data.DoubleColumn)o : new Altaxo.Data.DoubleColumn();

        // deserialize the base class
        info.GetBaseValueEmbedded(s, typeof(Altaxo.Data.DataColumn), parent);

        int count = info.GetInt32Attribute("Count");
        s._data = new double[count];
        info.GetArray(s._data, count);
        s._capacity = null == s._data ? 0 : s._data.Length;
        s._count = s._capacity;

        return s;
      }
    }

    #endregion "Serialization"

    /// <summary>
    /// Get/sets the data of this DoubleColumn. The data is copied (not directly used) to/from the array.
    /// </summary>
    public double[] Array
    {
      get
      {
        int len = Count;
        double[] arr = new double[len];
        System.Array.Copy(_data, 0, arr, 0, len);
        return arr;
      }

      set
      {
        _data = (double[])value.Clone();
        _count = _data.Length;
        _capacity = _data.Length;
        EhSelfChanged(0, _count, true);
      }
    }

    protected internal double GetValueDirect(int idx)
    {
      return _data[idx];
    }

    /// <summary>
    /// Gets the type of the colum's items.
    /// </summary>
    /// <value>
    /// The type of the item.
    /// </value>
    public override Type ItemType { get { return typeof(double); } }

    /// <summary>
    /// Returns the used length of the array. This is one plus the highest index of the number different from Double.NaN.
    /// </summary>
    /// <param name="values">The array for which the used length has to be determined.</param>
    /// <param name="currentlength">The current length of the array. Normally values.Length, but you can provide a value less than this.</param>
    /// <returns>The used length, i.e. numbers above the used length until the end of the array are NaNs.</returns>
    public static int GetUsedLength(double[] values, int currentlength)
    {
      for (int i = currentlength - 1; i >= 0; i--)
      {
        if (!double.IsNaN(values[i]))
          return i + 1;
      }
      return 0;
    }

    public static explicit operator DoubleColumn(double[] src)
    {
      var c = new DoubleColumn();
      c.CopyDataFrom(src);
      return c;
    }

    /// <summary>
    /// Copies the data from an array into the column. All data in the source array is copied.
    /// </summary>
    /// <param name="srcarray">The source array.</param>
    public void CopyDataFrom(double[] srcarray)
    {
      CopyDataFrom(srcarray, srcarray.Length);
    }

    /// <summary>
    /// Copies the data from an array into the column. The data from index 0 until <c>count-1</c> is copied to the destination.
    /// </summary>
    /// <param name="srcarray">Array containing the source data.</param>
    /// <param name="count">Length of the array (or length of the used range of the array, starting from index 0).</param>
    public void CopyDataFrom(double[] srcarray, int count)
    {
      int oldCount = _count;
      int srcarraycount = 0;

      if (null == srcarray || 0 == (srcarraycount = GetUsedLength(srcarray, Math.Min(srcarray.Length, count))))
      {
        _data = null;
        _capacity = 0;
        _count = 0;
      }
      else
      {
        if (_capacity < srcarraycount)
          _data = new double[srcarraycount];
        System.Array.Copy(srcarray, _data, srcarraycount);
        _capacity = _data.Length;
        _count = srcarraycount;
      }
      if (oldCount > 0 || _count > 0) // message only if really was a change
        EhSelfChanged(0, oldCount > _count ? (oldCount) : (_count), _count < oldCount);
    }

    /// <summary>
    /// Provides a setter property to which a vector can be assigned to. Copies all elements of the vector to this column.
    /// The getter property creates a wrapper for this data column that implements IVector. The length of the wrapped vector is set to the current Count of the DoubleColumn.
    /// </summary>
    public override IReadOnlyList<double> AssignVector
    {
      set
      {
        CopyDataFrom(value, value.Count);
      }
    }

    /// <summary>
    /// Provides a setter property to which a readonly vector can be assigned to. Copies all elements of the readonly vector to this column.
    /// The getter property creates a wrapper for this data column that implements IROVector. For short time use only, since it reflects changes in the data, but not in the length of the DoubleColumn.
    /// </summary>
    public override Altaxo.Calc.LinearAlgebra.IROVector<double> ToROVector(int start, int count)
    {
      return new ROVector(this, start, count);
    }

    public override Altaxo.Calc.LinearAlgebra.IVector<double> ToVector(int start, int count)
    {
      return new RWVector(this, start, count);
    }

    /// <summary>
    /// Gets a copy of the data (of actual row count, starting from position 0).
    /// </summary>
    /// <returns>The array with a copy of the data contained in this column.</returns>
    public double[] ToArray()
    {
      return ToArray(0, Count);
    }

    /// <summary>
    /// Gets a copy of the data (of actual length, starting from position 0).
    /// </summary>
    /// <param name="position">Index of the first row to copy.</param>
    /// <param name="length">Number of elements to copy.</param>
    /// <returns>The array with a copy of the data contained in this column.</returns>
    public double[] ToArray(int position, int length)
    {
      if (position < 0)
        throw new ArgumentOutOfRangeException("position is negative");
      if (position + length > Count)
        throw new ArgumentOutOfRangeException("end (position+length) exceeds row count");
      var result = new double[length];
      System.Array.Copy(_data, position, result, 0, length);
      return result;
    }

    /// <summary>
    /// Copies the data from an read-only into the column. The data from index 0 until <c>count-1</c> is copied to the destination.
    /// </summary>
    /// <param name="srcarray">Vector containing the source data.</param>
    /// <param name="count">Length of the array (or length of the used range of the array, starting from index 0).</param>
    public void CopyDataFrom(IReadOnlyList<double> srcarray, int count)
    {
      int oldCount = _count;
      int srcarraycount = 0;

      if (null == srcarray || 0 == (srcarraycount = Altaxo.Calc.LinearAlgebra.VectorMath.GetUsedLength(srcarray, Math.Min(srcarray.Count, count))))
      {
        _data = null;
        _capacity = 0;
        _count = 0;
      }
      else
      {
        if (_capacity < srcarraycount)
          _data = new double[srcarraycount];
        for (int i = 0; i < srcarraycount; ++i)
          _data[i] = srcarray[i];
        _capacity = _data.Length;
        _count = srcarraycount;
      }
      if (oldCount > 0 || _count > 0) // message only if really was a change
        EhSelfChanged(0, oldCount > _count ? (oldCount) : (_count), _count < oldCount);
    }

    protected void Realloc(int i)
    {
      int newcapacity1 = (int)(_capacity * _increaseFactor + _addSpace);
      int newcapacity2 = i + _addSpace + 1;
      int newcapacity = newcapacity1 > newcapacity2 ? newcapacity1 : newcapacity2;
      newcapacity = Math.Min(newcapacity, MaxCount);

      if (i > newcapacity)
        throw new ArgumentOutOfRangeException(string.Format("Unable to allocate {0} rows!", i));

      double[] newarray = new double[newcapacity];
      if (_count > 0)
      {
        System.Array.Copy(_data, newarray, _count);
      }

      _data = newarray;
      _capacity = _data.Length;
    }

    public new double this[int i]
    {
      get
      {
        if (i >= 0 && i < _count)
          return _data[i];
        return double.NaN;
      }
      set
      {
        bool bCountDecreased = false;

        if (i < 0)
          throw new ArgumentOutOfRangeException(string.Format("Index<0 (i={0}) while trying to set element of column {1} ({2})", i, Name, FullName));

        if (double.IsNaN(value))
        {
          if (i < _count - 1) // i is inside the used range
          {
            _data[i] = value;
          }
          else if (i == (_count - 1)) // m_Count is then decreasing
          {
            for (_count = i; _count > 0 && double.IsNaN(_data[_count - 1]); --_count)
              ;
            bCountDecreased = true;
            ;
          }
          else // i is above the used area
          {
            return; // no need for a change notification here
          }
        }
        else // value is not NaN
        {
          if (i < _count) // i is inside the used range
          {
            _data[i] = value;
          }
          else if (i == _count && i < _capacity) // i is the next value after the used range
          {
            _data[i] = value;
            _count = i + 1;
          }
          else if (i > _count && i < _capacity) // is is outside used range, but inside capacity of array
          {
            for (int k = _count; k < i; k++)
              _data[k] = double.NaN; // fill range between used range and new element with voids

            _data[i] = value;
            _count = i + 1;
          }
          else // i is outside of capacity, then realloc the array
          {
            Realloc(i);

            for (int k = _count; k < i; k++)
              _data[k] = double.NaN; // fill range between used range and new element with voids

            _data[i] = value;
            _count = i + 1;
          }
        }
        EhSelfChanged(i, i + 1, bCountDecreased);
      } // end set
    } // end indexer

    #region Vector decorators

    private class ROVector : Altaxo.Calc.LinearAlgebra.IROVector<double>
    {
      private DoubleColumn _col;
      private int _start;
      private int _count;

      public ROVector(DoubleColumn col, int start, int count)
      {
        _col = col;
        _start = start;
        _count = count;
      }

      #region IROVector Members

      public int Length
      {
        get { return _count; }
      }

      public int Count
      {
        get { return _count; }
      }

      #endregion IROVector Members

      #region INumericSequence Members

      public double this[int i]
      {
        get { return _col[_start + i]; }
      }

      public IEnumerator<double> GetEnumerator()
      {
        for (int i = 0; i < _count; ++i)
          yield return this[i];
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < _count; ++i)
          yield return this[i];
      }

      #endregion INumericSequence Members
    }

    private class RWVector : Altaxo.Calc.LinearAlgebra.IVector<double>
    {
      private DoubleColumn _col;
      private int _start;
      private int _count;

      public RWVector(DoubleColumn col, int start, int count)
      {
        _col = col;
        _start = start;
        _count = count;
      }

      #region IVector Members

      public double this[int i]
      {
        get
        {
          return _col[_start + i];
        }
        set
        {
          _col[_start + i] = value;
        }
      }

      #endregion IVector Members

      #region IROVector Members

      public int Length
      {
        get { return _count; }
      }

      public int Count
      {
        get { return _count; }
      }

      public IEnumerator<double> GetEnumerator()
      {
        for (int i = 0; i < _count; ++i)
          yield return this[i];
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < _count; ++i)
          yield return this[i];
      }

      #endregion IROVector Members
    }

    #endregion Vector decorators

    #region Operators

    // -----------------------------------------------------------------------------
    //
    //                        Operators
    //
    // -----------------------------------------------------------------------------

    // ----------------------- Addition operator -----------------------------------
    public static Altaxo.Data.DoubleColumn operator +(Altaxo.Data.DoubleColumn c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c1._count < c2._count ? c1._count : c2._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = c1._data[i] + c2._data[i];
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn operator +(Altaxo.Data.DoubleColumn c1, double c2)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
        c3._data[i] = c1._data[i] + c2;
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn operator +(double c2, Altaxo.Data.DoubleColumn c1)
    {
      return c1 + c2;
    }

    public override bool vop_Addition(DataColumn c2, out DataColumn c3)
    {
      if (c2 is Altaxo.Data.DoubleColumn)
      {
        c3 = this + (Altaxo.Data.DoubleColumn)c2;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_Addition_Rev(DataColumn c2, out DataColumn c3)
    {
      return vop_Addition(c2, out c3);
    }

    public override bool vop_Addition(AltaxoVariant c2, out DataColumn c3)
    {
      if (c2.IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = c2;
        c3 = this + c22;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_Addition_Rev(AltaxoVariant c2, out DataColumn c3)
    {
      return vop_Addition(c2, out c3);
    }

    // --------------------- Operator Subtract -------------------------------------

    public static Altaxo.Data.DoubleColumn operator -(Altaxo.Data.DoubleColumn c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c1._count < c2._count ? c1._count : c2._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = c1._data[i] - c2._data[i];
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn operator -(Altaxo.Data.DoubleColumn c1, double c2)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = c1._data[i] - c2;
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn operator -(double c2, Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = c2 - c1._data[i];
      }
      c3._count = len;
      return c3;
    }

    public override bool vop_Subtraction(DataColumn c2, out DataColumn c3)
    {
      if (c2 is Altaxo.Data.DoubleColumn)
      {
        c3 = this - (Altaxo.Data.DoubleColumn)c2;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_Subtraction_Rev(DataColumn c2, out DataColumn c3)
    {
      if (c2 is Altaxo.Data.DoubleColumn)
      {
        c3 = (Altaxo.Data.DoubleColumn)c2 - this;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_Subtraction(AltaxoVariant c2, out DataColumn c3)
    {
      if (c2.IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = c2;
        c3 = this - c22;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_Subtraction_Rev(AltaxoVariant c2, out DataColumn c3)
    {
      if (c2.IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = c2;
        c3 = c22 - this;
        return true;
      }
      c3 = null;
      return false;
    }

    public static Altaxo.Data.DoubleColumn Subtraction(Altaxo.Data.DateTimeColumn c1, Altaxo.Data.DateTimeColumn c2)
    {
      int len = c1.Count < c2.Count ? c1.Count : c2.Count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = (c1.GetValueDirect(i) - c2.GetValueDirect(i)).TotalSeconds;
      }

      c3._count = len;

      return c3;
    }

    public static Altaxo.Data.DoubleColumn Subtraction(Altaxo.Data.DateTimeColumn c1, DateTime c2)
    {
      int len = c1.Count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = (c1.GetValueDirect(i) - c2).TotalSeconds;
      }

      c3._count = len;

      return c3;
    }

    public static Altaxo.Data.DoubleColumn Subtraction(DateTime c1, Altaxo.Data.DateTimeColumn c2)
    {
      int len = c2.Count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = (c1 - c2.GetValueDirect(i)).TotalSeconds;
      }

      c3._count = len;

      return c3;
    }

    // ----------------------- Multiplication operator -----------------------------------
    public static Altaxo.Data.DoubleColumn operator *(Altaxo.Data.DoubleColumn c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c1._count < c2._count ? c1._count : c2._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = c1._data[i] * c2._data[i];
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn operator *(Altaxo.Data.DoubleColumn c1, double c2)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
        c3._data[i] = c1._data[i] * c2;
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn operator *(double c2, Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
        c3._data[i] = c1._data[i] * c2;
      c3._count = len;
      return c3;
    }

    public override bool vop_Multiplication(DataColumn c2, out DataColumn c3)
    {
      if (c2 is Altaxo.Data.DoubleColumn)
      {
        c3 = this * (Altaxo.Data.DoubleColumn)c2;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_Multiplication_Rev(DataColumn c2, out DataColumn c3)
    {
      return vop_Multiplication(c2, out c3);
    }

    public override bool vop_Multiplication(AltaxoVariant c2, out DataColumn c3)
    {
      if (c2.IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = c2;
        c3 = this * c22;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_Multiplication_Rev(AltaxoVariant c2, out DataColumn c3)
    {
      return vop_Multiplication(c2, out c3);
    }

    // ------------------------ Division operator --------------------------------

    public static Altaxo.Data.DoubleColumn operator /(Altaxo.Data.DoubleColumn c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c1._count < c2._count ? c1._count : c2._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = c1._data[i] / c2._data[i];
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn operator /(Altaxo.Data.DoubleColumn c1, double c2)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = c1._data[i] / c2;
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn operator /(double c2, Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = c2 / c1._data[i];
      }
      c3._count = len;
      return c3;
    }

    public override bool vop_Division(DataColumn c2, out DataColumn c3)
    {
      if (c2 is Altaxo.Data.DoubleColumn)
      {
        c3 = this / (Altaxo.Data.DoubleColumn)c2;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_Division_Rev(DataColumn c2, out DataColumn c3)
    {
      if (c2 is Altaxo.Data.DoubleColumn)
      {
        c3 = (Altaxo.Data.DoubleColumn)c2 / this;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_Division(AltaxoVariant c2, out DataColumn c3)
    {
      if (c2.IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = c2;
        c3 = this / c22;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_Division_Rev(AltaxoVariant c2, out DataColumn c3)
    {
      if (c2.IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = c2;
        c3 = c22 / this;
        return true;
      }
      c3 = null;
      return false;
    }

    // -------------------------- operator % ----------------------------------------------
    public static Altaxo.Data.DoubleColumn operator %(Altaxo.Data.DoubleColumn c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c1._count < c2._count ? c1._count : c2._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = c1._data[i] % c2._data[i];
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn operator %(Altaxo.Data.DoubleColumn c1, double c2)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = c1._data[i] % c2;
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn operator %(double c2, Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = c2 % c1._data[i];
      }
      c3._count = len;
      return c3;
    }

    public override bool vop_Modulo(DataColumn c2, out DataColumn c3)
    {
      if (c2 is Altaxo.Data.DoubleColumn)
      {
        c3 = this % (Altaxo.Data.DoubleColumn)c2;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_Modulo_Rev(DataColumn c2, out DataColumn c3)
    {
      if (c2 is Altaxo.Data.DoubleColumn)
      {
        c3 = (Altaxo.Data.DoubleColumn)c2 % this;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_Modulo(AltaxoVariant c2, out DataColumn c3)
    {
      if (c2.IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = c2;
        c3 = this % c22;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_Modulo_Rev(AltaxoVariant c2, out DataColumn c3)
    {
      if (c2.IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = c2;
        c3 = c22 % this;
        return true;
      }
      c3 = null;
      return false;
    }

    // ----------------------- AND operator -----------------------------------
    public static Altaxo.Data.DoubleColumn operator &(Altaxo.Data.DoubleColumn c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c1._count < c2._count ? c1._count : c2._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = ((long)c1._data[i]) & ((long)c2._data[i]);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn operator &(Altaxo.Data.DoubleColumn c1, double c2)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      long c22 = (long)c2;
      for (int i = 0; i < len; i++)
        c3._data[i] = ((long)c1._data[i]) & c22;
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn operator &(double c2, Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      long c22 = (long)c2;
      for (int i = 0; i < len; i++)
        c3._data[i] = c22 & ((long)c1._data[i]);
      c3._count = len;
      return c3;
    }

    public override bool vop_And(DataColumn c2, out DataColumn c3)
    {
      if (c2 is Altaxo.Data.DoubleColumn)
      {
        c3 = this & (Altaxo.Data.DoubleColumn)c2;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_And_Rev(DataColumn c2, out DataColumn c3)
    {
      if (c2 is Altaxo.Data.DoubleColumn)
      {
        c3 = (Altaxo.Data.DoubleColumn)c2 & this;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_And(AltaxoVariant c2, out DataColumn c3)
    {
      if (c2.IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = c2;
        c3 = this & c22;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_And_Rev(AltaxoVariant c2, out DataColumn c3)
    {
      if (c2.IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = c2;
        c3 = c22 & this;
        return true;
      }
      c3 = null;
      return false;
    }

    // ----------------------- OR operator -----------------------------------
    public static Altaxo.Data.DoubleColumn operator |(Altaxo.Data.DoubleColumn c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c1._count < c2._count ? c1._count : c2._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = ((long)c1._data[i]) | ((long)c2._data[i]);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn operator |(Altaxo.Data.DoubleColumn c1, double c2)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      long c22 = (long)c2;
      for (int i = 0; i < len; i++)
        c3._data[i] = ((long)c1._data[i]) | c22;
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn operator |(double c2, Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      long c22 = (long)c2;
      for (int i = 0; i < len; i++)
        c3._data[i] = c22 | ((long)c1._data[i]);
      c3._count = len;
      return c3;
    }

    public override bool vop_Or(DataColumn c2, out DataColumn c3)
    {
      if (c2 is Altaxo.Data.DoubleColumn)
      {
        c3 = this | (Altaxo.Data.DoubleColumn)c2;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_Or_Rev(DataColumn c2, out DataColumn c3)
    {
      if (c2 is Altaxo.Data.DoubleColumn)
      {
        c3 = (Altaxo.Data.DoubleColumn)c2 | this;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_Or(AltaxoVariant c2, out DataColumn c3)
    {
      if (c2.IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = c2;
        c3 = this | c22;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_Or_Rev(AltaxoVariant c2, out DataColumn c3)
    {
      if (c2.IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = c2;
        c3 = c22 | this;
        return true;
      }
      c3 = null;
      return false;
    }

    // ----------------------- XOR operator -----------------------------------
    public static Altaxo.Data.DoubleColumn operator ^(Altaxo.Data.DoubleColumn c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c1._count < c2._count ? c1._count : c2._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = ((long)c1._data[i]) ^ ((long)c2._data[i]);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn operator ^(Altaxo.Data.DoubleColumn c1, double c2)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      long c22 = (long)c2;
      for (int i = 0; i < len; i++)
        c3._data[i] = ((long)c1._data[i]) ^ c22;
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn operator ^(double c2, Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      long c22 = (long)c2;
      for (int i = 0; i < len; i++)
        c3._data[i] = c22 ^ ((long)c1._data[i]);
      c3._count = len;
      return c3;
    }

    public override bool vop_Xor(DataColumn c2, out DataColumn c3)
    {
      if (c2 is Altaxo.Data.DoubleColumn)
      {
        c3 = this ^ (Altaxo.Data.DoubleColumn)c2;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_Xor_Rev(DataColumn c2, out DataColumn c3)
    {
      if (c2 is Altaxo.Data.DoubleColumn)
      {
        c3 = (Altaxo.Data.DoubleColumn)c2 ^ this;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_Xor(AltaxoVariant c2, out DataColumn c3)
    {
      if (c2.IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = c2;
        c3 = this ^ c22;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_Xor_Rev(AltaxoVariant c2, out DataColumn c3)
    {
      if (c2.IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = c2;
        c3 = c22 ^ this;
        return true;
      }
      c3 = null;
      return false;
    }

    // ----------------------- ShiftLeft operator -----------------------------------

    public static Altaxo.Data.DoubleColumn operator <<(Altaxo.Data.DoubleColumn c1, int c2)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
        c3._data[i] = ((long)c1._data[i]) << c2;
      c3._count = len;
      return c3;
    }

    public override bool vop_ShiftLeft(DataColumn c2, out DataColumn c3)
    {
      if (c2 is Altaxo.Data.DoubleColumn)
      {
        Altaxo.Data.DoubleColumn c1 = this;
        var c22 = (DoubleColumn)c2;
        int len = c1.Count < c2.Count ? c1.Count : c2.Count;
        var c33 = new Altaxo.Data.DoubleColumn(len);
        for (int i = 0; i < len; i++)
        {
          c33._data[i] = ((long)c1._data[i]) << ((int)c22._data[i]);
        }
        c33._count = len;
        c3 = c33;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_ShiftLeft_Rev(DataColumn c2, out DataColumn c3)
    {
      if (c2 is Altaxo.Data.DoubleColumn)
      {
        Altaxo.Data.DoubleColumn c1 = this;
        var c22 = (DoubleColumn)c2;

        int len = c1.Count < c2.Count ? c1.Count : c2.Count;
        var c33 = new Altaxo.Data.DoubleColumn(len);
        for (int i = 0; i < len; i++)
        {
          c33._data[i] = ((long)c22._data[i]) << ((int)c1._data[i]);
        }
        c33._count = len;
        c3 = c33;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_ShiftLeft(AltaxoVariant c2, out DataColumn c3)
    {
      if (c2.IsType(AltaxoVariant.Content.VDouble))
      {
        int c22 = (int)(double)c2;
        c3 = this << c22;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_ShiftLeft_Rev(AltaxoVariant c2, out DataColumn c3)
    {
      if (c2.IsType(AltaxoVariant.Content.VDouble))
      {
        DoubleColumn c1 = this;
        int len = c1._count;
        var c33 = new Altaxo.Data.DoubleColumn(len);
        long c22 = (long)(double)c2;
        for (int i = 0; i < len; i++)
          c33._data[i] = c22 << ((int)c1._data[i]);
        c33._count = len;
        c3 = c33;
        return true;
      }
      c3 = null;
      return false;
    }

    // ----------------------- ShiftRight operator -----------------------------------

    public static Altaxo.Data.DoubleColumn operator >>(Altaxo.Data.DoubleColumn c1, int c2)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
        c3._data[i] = ((long)c1._data[i]) >> c2;
      c3._count = len;
      return c3;
    }

    public override bool vop_ShiftRight(DataColumn c2, out DataColumn c3)
    {
      if (c2 is Altaxo.Data.DoubleColumn)
      {
        DoubleColumn c1 = this;
        var c22 = (DoubleColumn)c2;
        int len = c1.Count < c2.Count ? c1.Count : c2.Count;
        var c33 = new Altaxo.Data.DoubleColumn(len);
        for (int i = 0; i < len; i++)
        {
          c33._data[i] = ((long)c1._data[i]) >> ((int)c22._data[i]);
        }
        c33._count = len;
        c3 = c33;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_ShiftRight_Rev(DataColumn c2, out DataColumn c3)
    {
      if (c2 is Altaxo.Data.DoubleColumn)
      {
        Altaxo.Data.DoubleColumn c1 = this;
        var c22 = (DoubleColumn)c2;
        int len = c1.Count < c2.Count ? c1.Count : c2.Count;
        var c33 = new Altaxo.Data.DoubleColumn(len);
        for (int i = 0; i < len; i++)
        {
          c33._data[i] = ((long)c22._data[i]) >> ((int)c1._data[i]);
        }
        c33._count = len;
        c3 = c33;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_ShiftRight(AltaxoVariant c2, out DataColumn c3)
    {
      if (c2.IsType(AltaxoVariant.Content.VDouble))
      {
        DoubleColumn c1 = this;
        int len = c1._count;
        var c33 = new Altaxo.Data.DoubleColumn(len);
        int c22 = (int)(double)c2;
        for (int i = 0; i < len; i++)
          c33._data[i] = ((long)c1._data[i]) >> c22;
        c33._count = len;
        c3 = c33;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_ShiftRight_Rev(AltaxoVariant c2, out DataColumn c3)
    {
      if (c2.IsType(AltaxoVariant.Content.VDouble))
      {
        DoubleColumn c1 = this;
        int len = c1._count;
        var c33 = new Altaxo.Data.DoubleColumn(len);
        long c22 = (long)(double)c2;
        for (int i = 0; i < len; i++)
          c33._data[i] = c22 >> ((int)c1._data[i]);
        c33._count = len;
        c3 = c33;
        return true;
      }
      c3 = null;
      return false;
    }

    // ----------------------- Lesser operator -----------------------------------
    public static Altaxo.Data.DoubleColumn operator <(Altaxo.Data.DoubleColumn c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c1._count < c2._count ? c1._count : c2._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = (c1._data[i] < c2._data[i]) ? 1 : 0;
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn operator <(Altaxo.Data.DoubleColumn c1, double c2)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
        c3._data[i] = (c1._data[i] < c2) ? 1 : 0;
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn operator <(double c2, Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
        c3._data[i] = (c2 < c1._data[i]) ? 1 : 0;
      c3._count = len;
      return c3;
    }

    public override bool vop_Lesser(DataColumn c2, out DataColumn c3)
    {
      if (c2 is Altaxo.Data.DoubleColumn)
      {
        c3 = this < (Altaxo.Data.DoubleColumn)c2;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_Lesser_Rev(DataColumn c2, out DataColumn c3)
    {
      if (c2 is Altaxo.Data.DoubleColumn)
      {
        c3 = (Altaxo.Data.DoubleColumn)c2 < this;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_Lesser(AltaxoVariant c2, out DataColumn c3)
    {
      if (c2.IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = c2;
        c3 = this < c22;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_Lesser_Rev(AltaxoVariant c2, out DataColumn c3)
    {
      if (c2.IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = c2;
        c3 = c22 < this;
        return true;
      }
      c3 = null;
      return false;
    }

    // ----------------------- Greater operator -----------------------------------
    public static Altaxo.Data.DoubleColumn operator >(Altaxo.Data.DoubleColumn c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c1._count < c2._count ? c1._count : c2._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = (c1._data[i] > c2._data[i]) ? 1 : 0;
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn operator >(Altaxo.Data.DoubleColumn c1, double c2)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
        c3._data[i] = (c1._data[i] > c2) ? 1 : 0;
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn operator >(double c2, Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
        c3._data[i] = (c2 > c1._data[i]) ? 1 : 0;
      c3._count = len;
      return c3;
    }

    public override bool vop_Greater(DataColumn c2, out DataColumn c3)
    {
      if (c2 is Altaxo.Data.DoubleColumn)
      {
        c3 = this > (Altaxo.Data.DoubleColumn)c2;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_Greater_Rev(DataColumn c2, out DataColumn c3)
    {
      if (c2 is Altaxo.Data.DoubleColumn)
      {
        c3 = (Altaxo.Data.DoubleColumn)c2 > this;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_Greater(AltaxoVariant c2, out DataColumn c3)
    {
      if (c2.IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = c2;
        c3 = this > c22;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_Greater_Rev(AltaxoVariant c2, out DataColumn c3)
    {
      if (c2.IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = c2;
        c3 = c22 > this;
        return true;
      }
      c3 = null;
      return false;
    }

    // ----------------------- LesserOrEqual operator -----------------------------------
    public static Altaxo.Data.DoubleColumn operator <=(Altaxo.Data.DoubleColumn c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c1._count < c2._count ? c1._count : c2._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = (c1._data[i] <= c2._data[i]) ? 1 : 0;
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn operator <=(Altaxo.Data.DoubleColumn c1, double c2)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
        c3._data[i] = (c1._data[i] <= c2) ? 1 : 0;
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn operator <=(double c2, Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
        c3._data[i] = (c2 <= c1._data[i]) ? 1 : 0;
      c3._count = len;
      return c3;
    }

    public override bool vop_LesserOrEqual(DataColumn c2, out DataColumn c3)
    {
      if (c2 is Altaxo.Data.DoubleColumn)
      {
        c3 = this <= (Altaxo.Data.DoubleColumn)c2;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_LesserOrEqual_Rev(DataColumn c2, out DataColumn c3)
    {
      if (c2 is Altaxo.Data.DoubleColumn)
      {
        c3 = (Altaxo.Data.DoubleColumn)c2 <= this;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_LesserOrEqual(AltaxoVariant c2, out DataColumn c3)
    {
      if (c2.IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = c2;
        c3 = this <= c22;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_LesserOrEqual_Rev(AltaxoVariant c2, out DataColumn c3)
    {
      if (c2.IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = c2;
        c3 = c22 <= this;
        return true;
      }
      c3 = null;
      return false;
    }

    // ----------------------- GreaterOrEqual operator -----------------------------------
    public static Altaxo.Data.DoubleColumn operator >=(Altaxo.Data.DoubleColumn c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c1._count < c2._count ? c1._count : c2._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = (c1._data[i] >= c2._data[i]) ? 1 : 0;
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn operator >=(Altaxo.Data.DoubleColumn c1, double c2)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
        c3._data[i] = (c1._data[i] >= c2) ? 1 : 0;
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn operator >=(double c2, Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
        c3._data[i] = (c2 >= c1._data[i]) ? 1 : 0;
      c3._count = len;
      return c3;
    }

    public override bool vop_GreaterOrEqual(DataColumn c2, out DataColumn c3)
    {
      if (c2 is Altaxo.Data.DoubleColumn)
      {
        c3 = this >= (Altaxo.Data.DoubleColumn)c2;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_GreaterOrEqual_Rev(DataColumn c2, out DataColumn c3)
    {
      if (c2 is Altaxo.Data.DoubleColumn)
      {
        c3 = (Altaxo.Data.DoubleColumn)c2 >= this;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_GreaterOrEqual(AltaxoVariant c2, out DataColumn c3)
    {
      if (c2.IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = c2;
        c3 = this >= c22;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_GreaterOrEqual_Rev(AltaxoVariant c2, out DataColumn c3)
    {
      if (c2.IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = c2;
        c3 = c22 >= this;
        return true;
      }
      c3 = null;
      return false;
    }

    // --------------------------------- Unary Plus ----------------------------
    public static Altaxo.Data.DoubleColumn operator +(Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = c1._data[i];
      }
      c3._count = len;
      return c3;
    }

    public override bool vop_Plus(out DataColumn c3)
    {
      c3 = +this;
      return true;
    }

    // --------------------------------- Unary Minus ----------------------------
    public static Altaxo.Data.DoubleColumn operator -(Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = -c1._data[i];
      }
      c3._count = len;
      return c3;
    }

    public override bool vop_Minus(out DataColumn c3)
    {
      c3 = -this;
      return true;
    }

    // --------------------------------- Unary NOT ----------------------------
    public static Altaxo.Data.DoubleColumn operator !(Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = 0 == c1._data[i] ? 1 : 0;
      }
      c3._count = len;
      return c3;
    }

    public override bool vop_Not(out DataColumn c3)
    {
      c3 = !this;
      return true;
    }

    // --------------------------------- Unary Complement ----------------------------
    public static Altaxo.Data.DoubleColumn operator ~(Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = ~((long)c1._data[i]);
      }
      c3._count = len;
      return c3;
    }

    public override bool vop_Complement(out DataColumn c3)
    {
      c3 = ~this;
      return true;
    }

    // --------------------------------- Unary Increment ----------------------------
    public static Altaxo.Data.DoubleColumn operator ++(Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = c1._data[i] + 1;
      }
      c3._count = len;
      return c3;
    }

    public override bool vop_Increment(out DataColumn c3)
    {
      int len = _count;
      var c33 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c33._data[i] = _data[i] + 1;
      }
      c33._count = len;
      c3 = c33;
      return true;
    }

    // --------------------------------- Unary Decrement ----------------------------
    public static Altaxo.Data.DoubleColumn operator --(Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = c1._data[i] - 1;
      }
      c3._count = len;
      return c3;
    }

    public override bool vop_Decrement(out DataColumn c3)
    {
      int len = _count;
      var c33 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c33._data[i] = _data[i] - 1;
      }
      c33._count = len;
      c3 = c33;
      return true;
    }

    // -----------------------------------------------------------------------------
    //
    //               arithmetic Functions
    //
    // -----------------------------------------------------------------------------

    public static Altaxo.Data.DoubleColumn Abs(Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = System.Math.Abs(c1._data[i]);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Acos(Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = System.Math.Acos(c1._data[i]);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Asin(Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = System.Math.Asin(c1._data[i]);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Atan(Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = System.Math.Atan(c1._data[i]);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Atan2(Altaxo.Data.DoubleColumn c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c1._count < c2._count ? c1._count : c2._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = System.Math.Atan2(c1._data[i], c2._data[i]);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Atan2(Altaxo.Data.DoubleColumn c1, double c2)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = System.Math.Atan2(c1._data[i], c2);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Atan2(double c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c2._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = System.Math.Atan2(c1, c2._data[i]);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Ceiling(Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = System.Math.Ceiling(c1._data[i]);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Cos(Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = System.Math.Cos(c1._data[i]);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Cosh(Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = System.Math.Cosh(c1._data[i]);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Exp(Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = System.Math.Exp(c1._data[i]);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Floor(Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = System.Math.Floor(c1._data[i]);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn IEEERemainder(Altaxo.Data.DoubleColumn c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c1._count < c2._count ? c1._count : c2._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = System.Math.IEEERemainder(c1._data[i], c2._data[i]);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn IEEERemainder(Altaxo.Data.DoubleColumn c1, double c2)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = System.Math.IEEERemainder(c1._data[i], c2);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn IEEERemainder(double c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c2._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = System.Math.IEEERemainder(c1, c2._data[i]);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Log(Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = System.Math.Log(c1._data[i]);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Log(Altaxo.Data.DoubleColumn c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c1._count < c2._count ? c1._count : c2._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = System.Math.Log(c1._data[i], c2._data[i]);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Log(Altaxo.Data.DoubleColumn c1, double c2)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = System.Math.Log(c1._data[i], c2);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Log(double c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c2._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = System.Math.Log(c1, c2._data[i]);
      }
      c3._count = len;
      return c3;
    }

    #region Log10

    public static Altaxo.Data.DoubleColumn Log10(Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = System.Math.Log10(c1._data[i]);
      }
      c3._count = len;
      return c3;
    }

    #endregion Log10

    public static Altaxo.Data.DoubleColumn Max(Altaxo.Data.DoubleColumn c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c1._count < c2._count ? c1._count : c2._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = System.Math.Max(c1._data[i], c2._data[i]);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Max(Altaxo.Data.DoubleColumn c1, double c2)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = System.Math.Max(c1._data[i], c2);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Max(double c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c2._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = System.Math.Max(c1, c2._data[i]);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Min(Altaxo.Data.DoubleColumn c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c1._count < c2._count ? c1._count : c2._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = System.Math.Min(c1._data[i], c2._data[i]);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Min(Altaxo.Data.DoubleColumn c1, double c2)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = System.Math.Min(c1._data[i], c2);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Min(double c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c2._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = System.Math.Min(c1, c2._data[i]);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Pow(Altaxo.Data.DoubleColumn c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c1._count < c2._count ? c1._count : c2._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = System.Math.Pow(c1._data[i], c2._data[i]);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Pow(Altaxo.Data.DoubleColumn c1, int c2)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = Altaxo.Calc.RMath.Pow(c1._data[i], c2);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Pow2(Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = Altaxo.Calc.RMath.Pow2(c1._data[i]);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Pow3(Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = Altaxo.Calc.RMath.Pow3(c1._data[i]);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Pow4(Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = Altaxo.Calc.RMath.Pow4(c1._data[i]);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Pow5(Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = Altaxo.Calc.RMath.Pow5(c1._data[i]);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Pow6(Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = Altaxo.Calc.RMath.Pow6(c1._data[i]);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Pow7(Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = Altaxo.Calc.RMath.Pow7(c1._data[i]);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Pow8(Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = Altaxo.Calc.RMath.Pow8(c1._data[i]);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Pow9(Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = Altaxo.Calc.RMath.Pow9(c1._data[i]);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Pow(Altaxo.Data.DoubleColumn c1, double c2)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = System.Math.Pow(c1._data[i], c2);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Pow(double c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c2._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = System.Math.Pow(c1, c2._data[i]);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Round(Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = System.Math.Round(c1._data[i]);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Round(Altaxo.Data.DoubleColumn c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c1._count < c2._count ? c1._count : c2._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = System.Math.Round(c1._data[i], (int)c2._data[i]);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Round(Altaxo.Data.DoubleColumn c1, int c2)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = System.Math.Round(c1._data[i], c2);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Round(double c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c2._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = System.Math.Round(c1, (int)c2._data[i]);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Sign(Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = System.Math.Sign(c1._data[i]);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Sin(Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = System.Math.Sin(c1._data[i]);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Sinh(Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = System.Math.Sinh(c1._data[i]);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Sqrt(Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = System.Math.Sqrt(c1._data[i]);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Tan(Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = System.Math.Tan(c1._data[i]);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Tanh(Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = System.Math.Tanh(c1._data[i]);
      }
      c3._count = len;
      return c3;
    }

    #endregion Operators

    #region Apply functions

    public static Altaxo.Data.DoubleColumn Map(Func<double, double> function, Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = function(c1._data[i]);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Map(Func<double, double, double> function, Altaxo.Data.DoubleColumn c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c1._count < c2._count ? c1._count : c2._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = function(c1._data[i], c2._data[i]);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Map(Func<double, double, double> function, Altaxo.Data.DoubleColumn c1, double c2)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = function(c1._data[i], c2);
      }
      c3._count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn Map(Func<double, double, double> function, double c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c2._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = function(c1, c2._data[i]);
      }
      c3._count = len;
      return c3;
    }

    #endregion Apply functions

    /// <summary>
    /// Searches for an index idx for which the value <paramref name="value"/> lies in the (unordered) interval [this[idx-1], this[idx]]. If
    /// such an index is found, the fractional index is returned. If no such index is found, the return value is null.
    /// </summary>
    /// <param name="value">The value to search.</param>
    /// <returns></returns>
    public double? FractionalIndexOf(double value)
    {
      for (int i = 1; i < Count; i++)
      {
        var frac = Altaxo.Calc.RMath.InFractionOfUnorderedIntervalCC(value, this[i - 1], this[i]);
        if (frac.HasValue)
        {
          return frac.Value == 1 ? i : (i - 1) + frac.Value;
        }
      }
      return null;
    }

    /// <summary>
    /// Counterpart to <see cref="FractionalIndexOf(double)"/>.
    /// Gets a linearly interpolated value at a given fractional index.
    /// If the given index is &lt; 0, the value at index 0 is returned; if the index is &gt;=Count-1, the value at index Count-1 is returned.
    /// </summary>
    /// <param name="fractionalIndex">The fractional index.</param>
    /// <returns>Linearly interpolated value between the values at Floor(fractionalIndex) and Ceiling(fractionalIndex).</returns>
    public double GetLinearlyInterpolatedValueAt(double fractionalIndex)
    {
      if (Count == 0)
        return double.NaN;
      else if (fractionalIndex < 0)
        return this[0];
      else if (fractionalIndex >= (Count - 1))
        return this[Count - 1];

      int idxBase = (int)Math.Floor(fractionalIndex);
      double idxFrac = fractionalIndex - idxBase;
      if (idxFrac == 0)
        return this[idxBase];
      else
        return (1 - idxFrac) * this[idxBase] + (idxFrac) * this[idxBase + 1];
    }
  } // end Altaxo.Data.DoubleColumn
}
