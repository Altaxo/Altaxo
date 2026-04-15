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

#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Altaxo.Data
{
  /// <summary>
  /// Data column that stores <see cref="double"/> values.
  /// </summary>
  public class DoubleColumn
    :
    Altaxo.Data.DataColumn,
    INumericColumn,
    IReadOnlyList<double>
  {
    static readonly double[] _emptyDoubleArray = new double[0];
    private double[] _data = _emptyDoubleArray;
    private int _capacity; // shortcut to _data.Length;
    private int _count;
    /// <summary>
    /// Represents an empty value in this column.
    /// </summary>
    public static readonly double NullValue = double.NaN;
    // private const int MaxCount = 256 * 1024 * 1024 - 8; // this is the maximum possible number of double elements in 64-bit mode currently (Framework 4.0).
    private const int MaxCount = 2147483592; // MaxCount with gcAllowVeryLargeObjects set to true (see https://docs.microsoft.com/en-us/dotnet/framework/configure-apps/file-schema/runtime/gcallowverylargeobjects-element)
    #region Overridden functions

    /// <inheritdoc />
    public override object Clone()
    {
      return new DoubleColumn(this);
    }

    /// <inheritdoc />
    public override int Count
    {
      get { return _count; }
    }

    /// <inheritdoc/>
    IEnumerator<double> IEnumerable<double>.GetEnumerator()
    {
      var length = _count;
      for (int i = 0; i < length; ++i)
        yield return this[i];
    }

    // indexers
    /// <inheritdoc />
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

    /// <inheritdoc />
    public override AltaxoVariant GetVariantAt(int i)
    {
      return new AltaxoVariant(this[i]);
    }

    /// <inheritdoc />
    public override bool IsElementEmpty(int i)
    {
      return i < _count ? double.IsNaN(_data[i]) : true;
    }

    /// <inheritdoc />
    public override void SetElementEmpty(int i)
    {
      if (i < _count)
        this[i] = NullValue;
    }

    /// <inheritdoc />
    public override void RemoveRows(int nFirstRow, int nCount)
    {
      if (nFirstRow < 0)
        throw new ArgumentException("Row number must be greater or equal 0, but was " + nFirstRow.ToString(), "nDelFirstRow");

      if (nCount <= 0)
        return; // nothing to do here, but we dont catch it

      // we must be careful, since the range to delete can be
      // above the range this column actually holds, but
      // we must handle this the right way
      int i, j;
      for (i = nFirstRow, j = nFirstRow + nCount; j < _count; i++, j++)
        _data[i] = _data[j];

      int prevCount = _count;
      _count = i < _count ? i : _count; // m_Count can only decrease

      if (_count != prevCount) // raise a event only if something really changed
        EhSelfChanged(nFirstRow, prevCount, true);
    }

    /// <inheritdoc />
    public override void InsertRows(int nBeforeRow, int nCount)
    {
      if (nCount <= 0 || nBeforeRow >= Count)
        return; // nothing to do

      int newlen = _count + nCount;
      if (newlen > _capacity)
        Realloc(newlen);

      // copy values from m_Count downto nBeforeColumn
      for (int i = _count - 1, j = newlen - 1; i >= nBeforeRow; i--, j--)
        _data[j] = _data[i];

      for (int i = nBeforeRow + nCount - 1; i >= nBeforeRow; i--)
        _data[i] = NullValue;

      _count = newlen;
      EhSelfChanged(nBeforeRow, _count, false);
    }

    /// <inheritdoc />
    public override void CopyDataFrom(object o)
    {
      var oldCount = _count;
      _count = 0;

      if (o is DoubleColumn dcol)
      {
        _data = dcol._data.Length == 0 ? _emptyDoubleArray : (double[])dcol._data.Clone();
        _capacity = _data?.Length ?? 0;
        _count = dcol._count;
      }
      else
      {
        if (o is ICollection ocoll)
          Realloc(ocoll.Count); // Prealloc the array if count of the collection is known beforehand

        if (o is IEnumerable<double> srcd)
        {
          _count = 0;
          foreach (var it in srcd)
          {
            if (_count >= _capacity)
              Realloc(_count);
            _data[_count++] = it;
          }
        }
        else if (o is IEnumerable<float> srcf)
        {
          _count = 0;
          foreach (var it in srcf)
          {
            if (_count >= _capacity)
              Realloc(_count);
            _data[_count++] = it;
          }
        }
        else if (o is IEnumerable<int> srci)
        {
          _count = 0;
          foreach (var it in srci)
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
        else if (o is IEnumerable<AltaxoVariant> srcv)
        {
          _count = 0;
          foreach (var it in srcv)
          {
            if (_count >= _capacity)
              Realloc(_count);
            _data[_count++] = it;
          }
        }
        else
        {
          _count = 0;
          if (o is null)
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

    /// <inheritdoc />
    public override System.Type GetColumnStyleType()
    {
      return typeof(Altaxo.Worksheet.DoubleColumnStyle);
    }

    #endregion Overridden functions

    /// <summary>
    /// Initializes a new instance of the <see cref="DoubleColumn"/> class.
    /// </summary>
    public DoubleColumn()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DoubleColumn"/> class with the specified initial capacity.
    /// </summary>
    /// <param name="initialcapacity">The initial capacity.</param>
    public DoubleColumn(int initialcapacity)
    {
      _count = 0;
      _data = new double[initialcapacity];
      _capacity = initialcapacity;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DoubleColumn"/> class by copying another instance.
    /// </summary>
    /// <param name="from">The source column.</param>
    public DoubleColumn(DoubleColumn from)
    {
      _count = from._count;
      _capacity = from._capacity;
      _data = from._data.Length == 0 ? _emptyDoubleArray : (double[])from._data.Clone();
    }

    #region "Serialization"

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Altaxo.Data.DoubleColumn), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (Altaxo.Data.DoubleColumn)o;
        // serialize the base class
        info.AddBaseValueEmbedded(s, typeof(Altaxo.Data.DataColumn));

        if ("true" == info.GetProperty(DataTable.SerializationInfoProperty_SaveAsTemplate))
          info.AddArray("Data", s._data, 0);
        else
          info.AddArray("Data", s._data, s._count);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (Altaxo.Data.DoubleColumn?)o ?? new Altaxo.Data.DoubleColumn();

        // deserialize the base class
        info.GetBaseValueEmbedded(s, typeof(Altaxo.Data.DataColumn), parent);

        int count = info.GetInt32Attribute("Count");
        s._data = count == 0 ? _emptyDoubleArray : new double[count];
        info.GetArray(s._data, count);
        s._capacity = s._data.Length;
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

    /// <summary>
    /// Gets the raw value stored at the specified index.
    /// </summary>
    /// <param name="idx">The zero-based row index.</param>
    /// <returns>The stored value.</returns>
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

    /// <summary>
    /// Creates a column from an array of doubles.
    /// </summary>
    /// <param name="src">The source array.</param>
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

      if (srcarray is null || 0 == (srcarraycount = GetUsedLength(srcarray, Math.Min(srcarray.Length, count))))
      {
        _data = _emptyDoubleArray;
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
    /// The getter property creates a wrapper for this data column that implements IROVector. For short-time use only, since it reflects changes in the data, but not in the length of the DoubleColumn.
    /// </summary>
    /// <param name="start">The start index of the segment.</param>
    /// <param name="count">The number of elements in the segment.</param>
    /// <returns>A read-only wrapper around the requested segment.</returns>
    public override IReadOnlyList<double> ToROVector(int start, int count)
    {
      return new ROVector(this, start, count);
    }

    /// <inheritdoc />
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

      if (srcarray is null || 0 == (srcarraycount = Altaxo.Calc.LinearAlgebra.VectorMath.GetUsedLength(srcarray, Math.Min(srcarray.Count, count))))
      {
        _data = _emptyDoubleArray;
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

    /// <summary>
    /// Reallocates the internal storage so that the specified index can be addressed.
    /// </summary>
    /// <param name="i">The required index.</param>
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

    /// <summary>
    /// Gets or sets the <see cref="double"/> value at the specified row index.
    /// </summary>
    /// <param name="i">The zero-based row index.</param>
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

    private class ROVector : IReadOnlyList<double>
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
    /// <summary>
    /// Adds two double columns.
    /// </summary>
    /// <param name="c1">The first operand.</param>
    /// <param name="c2">The second operand.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Adds a scalar value to a double column.
    /// </summary>
    /// <param name="c1">The column.</param>
    /// <param name="c2">The scalar value.</param>
    /// <returns>The result column.</returns>
    public static Altaxo.Data.DoubleColumn operator +(Altaxo.Data.DoubleColumn c1, double c2)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
        c3._data[i] = c1._data[i] + c2;
      c3._count = len;
      return c3;
    }

    /// <summary>
    /// Adds a double column to a scalar value.
    /// </summary>
    /// <param name="c2">The scalar value.</param>
    /// <param name="c1">The column.</param>
    /// <returns>The result column.</returns>
    public static Altaxo.Data.DoubleColumn operator +(double c2, Altaxo.Data.DoubleColumn c1)
    {
      return c1 + c2;
    }

    /// <inheritdoc />
    public override bool vop_Addition(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a is Altaxo.Data.DoubleColumn)
      {
        b = this + (Altaxo.Data.DoubleColumn)a;
        return true;
      }
      b = null;
      return false;
    }

    /// <inheritdoc />
    public override bool vop_Addition_Rev(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    {
      return vop_Addition(a, out b);
    }

    /// <inheritdoc />
    public override bool vop_Addition(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a.IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = a;
        b = this + c22;
        return true;
      }
      b = null;
      return false;
    }

    /// <inheritdoc />
    public override bool vop_Addition_Rev(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    {
      return vop_Addition(a, out b);
    }

    // --------------------- Operator Subtract -------------------------------------

    /// <summary>
    /// Subtracts one double column from another.
    /// </summary>
    /// <param name="c1">The minuend column.</param>
    /// <param name="c2">The subtrahend column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Subtracts a scalar value from a double column.
    /// </summary>
    /// <param name="c1">The minuend column.</param>
    /// <param name="c2">The scalar value.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Subtracts a double column from a scalar value.
    /// </summary>
    /// <param name="c2">The scalar value.</param>
    /// <param name="c1">The subtrahend column.</param>
    /// <returns>The result column.</returns>
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

    /// <inheritdoc />
    public override bool vop_Subtraction(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a is Altaxo.Data.DoubleColumn)
      {
        b = this - (Altaxo.Data.DoubleColumn)a;
        return true;
      }
      b = null;
      return false;
    }

    /// <inheritdoc />
    public override bool vop_Subtraction_Rev(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a is Altaxo.Data.DoubleColumn)
      {
        b = (Altaxo.Data.DoubleColumn)a - this;
        return true;
      }
      b = null;
      return false;
    }

    /// <inheritdoc />
    public override bool vop_Subtraction(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a.IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = a;
        b = this - c22;
        return true;
      }
      b = null;
      return false;
    }

    /// <inheritdoc />
    public override bool vop_Subtraction_Rev(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a.IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = a;
        b = c22 - this;
        return true;
      }
      b = null;
      return false;
    }

    /// <summary>
    /// Calculates the difference in seconds between two date-time columns.
    /// </summary>
    /// <param name="c1">The minuend column.</param>
    /// <param name="c2">The subtrahend column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Calculates the difference in seconds between a date-time column and a constant date-time value.
    /// </summary>
    /// <param name="c1">The minuend column.</param>
    /// <param name="c2">The subtrahend date-time value.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Calculates the difference in seconds between a constant date-time value and a date-time column.
    /// </summary>
    /// <param name="c1">The minuend date-time value.</param>
    /// <param name="c2">The subtrahend column.</param>
    /// <returns>The result column.</returns>
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
    /// <summary>
    /// Multiplies two double columns.
    /// </summary>
    /// <param name="c1">The first operand.</param>
    /// <param name="c2">The second operand.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Multiplies a double column by a scalar value.
    /// </summary>
    /// <param name="c1">The column.</param>
    /// <param name="c2">The scalar value.</param>
    /// <returns>The result column.</returns>
    public static Altaxo.Data.DoubleColumn operator *(Altaxo.Data.DoubleColumn c1, double c2)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
        c3._data[i] = c1._data[i] * c2;
      c3._count = len;
      return c3;
    }

    /// <summary>
    /// Multiplies a scalar value by a double column.
    /// </summary>
    /// <param name="c2">The scalar value.</param>
    /// <param name="c1">The column.</param>
    /// <returns>The result column.</returns>
    public static Altaxo.Data.DoubleColumn operator *(double c2, Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
        c3._data[i] = c1._data[i] * c2;
      c3._count = len;
      return c3;
    }

    /// <inheritdoc />
    public override bool vop_Multiplication(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a is Altaxo.Data.DoubleColumn)
      {
        b = this * (Altaxo.Data.DoubleColumn)a;
        return true;
      }
      b = null;
      return false;
    }

    /// <inheritdoc />
    public override bool vop_Multiplication_Rev(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    {
      return vop_Multiplication(a, out b);
    }

    /// <inheritdoc />
    public override bool vop_Multiplication(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a.IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = a;
        b = this * c22;
        return true;
      }
      b = null;
      return false;
    }

    /// <inheritdoc />
    public override bool vop_Multiplication_Rev(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    {
      return vop_Multiplication(a, out b);
    }

    // ------------------------ Division operator --------------------------------

    /// <summary>
    /// Divides one double column by another.
    /// </summary>
    /// <param name="c1">The dividend column.</param>
    /// <param name="c2">The divisor column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Divides a double column by a scalar value.
    /// </summary>
    /// <param name="c1">The dividend column.</param>
    /// <param name="c2">The scalar divisor.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Divides a scalar value by a double column.
    /// </summary>
    /// <param name="c2">The scalar dividend.</param>
    /// <param name="c1">The divisor column.</param>
    /// <returns>The result column.</returns>
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

    /// <inheritdoc />
    public override bool vop_Division(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a is Altaxo.Data.DoubleColumn)
      {
        b = this / (Altaxo.Data.DoubleColumn)a;
        return true;
      }
      b = null;
      return false;
    }

    /// <inheritdoc />
    public override bool vop_Division_Rev(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a is Altaxo.Data.DoubleColumn)
      {
        b = (Altaxo.Data.DoubleColumn)a / this;
        return true;
      }
      b = null;
      return false;
    }

    /// <inheritdoc />
    public override bool vop_Division(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a.IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = a;
        b = this / c22;
        return true;
      }
      b = null;
      return false;
    }

    /// <inheritdoc />
    public override bool vop_Division_Rev(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a.IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = a;
        b = c22 / this;
        return true;
      }
      b = null;
      return false;
    }

    // -------------------------- operator % ----------------------------------------------
    /// <summary>
    /// Calculates the modulo of one double column by another.
    /// </summary>
    /// <param name="c1">The dividend column.</param>
    /// <param name="c2">The divisor column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Calculates the modulo of a double column by a scalar value.
    /// </summary>
    /// <param name="c1">The dividend column.</param>
    /// <param name="c2">The scalar divisor.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Calculates the modulo of a scalar value by a double column.
    /// </summary>
    /// <param name="c2">The scalar dividend.</param>
    /// <param name="c1">The divisor column.</param>
    /// <returns>The result column.</returns>
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

    /// <inheritdoc />
    public override bool vop_Modulo(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a is Altaxo.Data.DoubleColumn)
      {
        b = this % (Altaxo.Data.DoubleColumn)a;
        return true;
      }
      b = null;
      return false;
    }

    /// <inheritdoc />
    public override bool vop_Modulo_Rev(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a is Altaxo.Data.DoubleColumn)
      {
        b = (Altaxo.Data.DoubleColumn)a % this;
        return true;
      }
      b = null;
      return false;
    }

    /// <inheritdoc />
    public override bool vop_Modulo(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a.IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = a;
        b = this % c22;
        return true;
      }
      b = null;
      return false;
    }

    /// <inheritdoc />
    public override bool vop_Modulo_Rev(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a.IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = a;
        b = c22 % this;
        return true;
      }
      b = null;
      return false;
    }

    // ----------------------- AND operator -----------------------------------
    /// <summary>
    /// Applies a bitwise AND between two double columns after converting values to integers.
    /// </summary>
    /// <param name="c1">The first operand.</param>
    /// <param name="c2">The second operand.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies a bitwise AND between a double column and a scalar value after converting values to integers.
    /// </summary>
    /// <param name="c1">The column.</param>
    /// <param name="c2">The scalar value.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies a bitwise AND between a scalar value and a double column after converting values to integers.
    /// </summary>
    /// <param name="c2">The scalar value.</param>
    /// <param name="c1">The column.</param>
    /// <returns>The result column.</returns>
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

    /// <inheritdoc />
    public override bool vop_And(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a is Altaxo.Data.DoubleColumn)
      {
        b = this & (Altaxo.Data.DoubleColumn)a;
        return true;
      }
      b = null;
      return false;
    }

    /// <inheritdoc />
    public override bool vop_And_Rev(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a is Altaxo.Data.DoubleColumn)
      {
        b = (Altaxo.Data.DoubleColumn)a & this;
        return true;
      }
      b = null;
      return false;
    }

    /// <inheritdoc />
    public override bool vop_And(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a.IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = a;
        b = this & c22;
        return true;
      }
      b = null;
      return false;
    }

    /// <inheritdoc />
    public override bool vop_And_Rev(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a.IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = a;
        b = c22 & this;
        return true;
      }
      b = null;
      return false;
    }

    // ----------------------- OR operator -----------------------------------
    /// <summary>
    /// Applies a bitwise OR between two double columns after converting values to integers.
    /// </summary>
    /// <param name="c1">The first operand.</param>
    /// <param name="c2">The second operand.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies a bitwise OR between a double column and a scalar value after converting values to integers.
    /// </summary>
    /// <param name="c1">The column.</param>
    /// <param name="c2">The scalar value.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies a bitwise OR between a scalar value and a double column after converting values to integers.
    /// </summary>
    /// <param name="c2">The scalar value.</param>
    /// <param name="c1">The column.</param>
    /// <returns>The result column.</returns>
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

    /// <inheritdoc />
    public override bool vop_Or(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a is Altaxo.Data.DoubleColumn)
      {
        b = this | (Altaxo.Data.DoubleColumn)a;
        return true;
      }
      b = null;
      return false;
    }

    /// <inheritdoc />
    public override bool vop_Or_Rev(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a is Altaxo.Data.DoubleColumn)
      {
        b = (Altaxo.Data.DoubleColumn)a | this;
        return true;
      }
      b = null;
      return false;
    }

    /// <inheritdoc />
    public override bool vop_Or(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a.IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = a;
        b = this | c22;
        return true;
      }
      b = null;
      return false;
    }

    /// <inheritdoc />
    public override bool vop_Or_Rev(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a.IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = a;
        b = c22 | this;
        return true;
      }
      b = null;
      return false;
    }

    // ----------------------- XOR operator -----------------------------------
    /// <summary>
    /// Applies a bitwise XOR between two double columns after converting values to integers.
    /// </summary>
    /// <param name="c1">The first operand.</param>
    /// <param name="c2">The second operand.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies a bitwise XOR between a double column and a scalar value after converting values to integers.
    /// </summary>
    /// <param name="c1">The column.</param>
    /// <param name="c2">The scalar value.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies a bitwise XOR between a scalar value and a double column after converting values to integers.
    /// </summary>
    /// <param name="c2">The scalar value.</param>
    /// <param name="c1">The column.</param>
    /// <returns>The result column.</returns>
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

    /// <inheritdoc />
    public override bool vop_Xor(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a is Altaxo.Data.DoubleColumn)
      {
        b = this ^ (Altaxo.Data.DoubleColumn)a;
        return true;
      }
      b = null;
      return false;
    }

    /// <inheritdoc />
    public override bool vop_Xor_Rev(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a is Altaxo.Data.DoubleColumn)
      {
        b = (Altaxo.Data.DoubleColumn)a ^ this;
        return true;
      }
      b = null;
      return false;
    }

    /// <inheritdoc />
    public override bool vop_Xor(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a.IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = a;
        b = this ^ c22;
        return true;
      }
      b = null;
      return false;
    }

    /// <inheritdoc />
    public override bool vop_Xor_Rev(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a.IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = a;
        b = c22 ^ this;
        return true;
      }
      b = null;
      return false;
    }

    // ----------------------- ShiftLeft operator -----------------------------------

    /// <summary>
    /// Shifts a double column left by the specified bit count after converting values to integers.
    /// </summary>
    /// <param name="c1">The column.</param>
    /// <param name="c2">The shift count.</param>
    /// <returns>The result column.</returns>
    public static Altaxo.Data.DoubleColumn operator <<(Altaxo.Data.DoubleColumn c1, int c2)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
        c3._data[i] = ((long)c1._data[i]) << c2;
      c3._count = len;
      return c3;
    }

    /// <inheritdoc />
    public override bool vop_ShiftLeft(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a is Altaxo.Data.DoubleColumn)
      {
        Altaxo.Data.DoubleColumn c1 = this;
        var c22 = (DoubleColumn)a;
        int len = c1.Count < a.Count ? c1.Count : a.Count;
        var c33 = new Altaxo.Data.DoubleColumn(len);
        for (int i = 0; i < len; i++)
        {
          c33._data[i] = ((long)c1._data[i]) << ((int)c22._data[i]);
        }
        c33._count = len;
        b = c33;
        return true;
      }
      b = null;
      return false;
    }

    /// <inheritdoc />
    public override bool vop_ShiftLeft_Rev(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a is Altaxo.Data.DoubleColumn)
      {
        Altaxo.Data.DoubleColumn c1 = this;
        var c22 = (DoubleColumn)a;

        int len = c1.Count < a.Count ? c1.Count : a.Count;
        var c33 = new Altaxo.Data.DoubleColumn(len);
        for (int i = 0; i < len; i++)
        {
          c33._data[i] = ((long)c22._data[i]) << ((int)c1._data[i]);
        }
        c33._count = len;
        b = c33;
        return true;
      }
      b = null;
      return false;
    }

    /// <inheritdoc />
    public override bool vop_ShiftLeft(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a.IsType(AltaxoVariant.Content.VDouble))
      {
        int c22 = (int)(double)a;
        b = this << c22;
        return true;
      }
      b = null;
      return false;
    }

    /// <inheritdoc />
    public override bool vop_ShiftLeft_Rev(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a.IsType(AltaxoVariant.Content.VDouble))
      {
        DoubleColumn c1 = this;
        int len = c1._count;
        var c33 = new Altaxo.Data.DoubleColumn(len);
        long c22 = (long)(double)a;
        for (int i = 0; i < len; i++)
          c33._data[i] = c22 << ((int)c1._data[i]);
        c33._count = len;
        b = c33;
        return true;
      }
      b = null;
      return false;
    }

    // ----------------------- ShiftRight operator -----------------------------------

    /// <summary>
    /// Shifts a double column right by the specified bit count after converting values to integers.
    /// </summary>
    /// <param name="c1">The column.</param>
    /// <param name="c2">The shift count.</param>
    /// <returns>The result column.</returns>
    public static Altaxo.Data.DoubleColumn operator >>(Altaxo.Data.DoubleColumn c1, int c2)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
        c3._data[i] = ((long)c1._data[i]) >> c2;
      c3._count = len;
      return c3;
    }

    /// <inheritdoc />
    public override bool vop_ShiftRight(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a is Altaxo.Data.DoubleColumn)
      {
        DoubleColumn c1 = this;
        var c22 = (DoubleColumn)a;
        int len = c1.Count < a.Count ? c1.Count : a.Count;
        var c33 = new Altaxo.Data.DoubleColumn(len);
        for (int i = 0; i < len; i++)
        {
          c33._data[i] = ((long)c1._data[i]) >> ((int)c22._data[i]);
        }
        c33._count = len;
        b = c33;
        return true;
      }
      b = null;
      return false;
    }

    /// <inheritdoc />
    public override bool vop_ShiftRight_Rev(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a is Altaxo.Data.DoubleColumn)
      {
        Altaxo.Data.DoubleColumn c1 = this;
        var c22 = (DoubleColumn)a;
        int len = c1.Count < a.Count ? c1.Count : a.Count;
        var c33 = new Altaxo.Data.DoubleColumn(len);
        for (int i = 0; i < len; i++)
        {
          c33._data[i] = ((long)c22._data[i]) >> ((int)c1._data[i]);
        }
        c33._count = len;
        b = c33;
        return true;
      }
      b = null;
      return false;
    }

    /// <inheritdoc />
    public override bool vop_ShiftRight(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a.IsType(AltaxoVariant.Content.VDouble))
      {
        DoubleColumn c1 = this;
        int len = c1._count;
        var c33 = new Altaxo.Data.DoubleColumn(len);
        int c22 = (int)(double)a;
        for (int i = 0; i < len; i++)
          c33._data[i] = ((long)c1._data[i]) >> c22;
        c33._count = len;
        b = c33;
        return true;
      }
      b = null;
      return false;
    }

    /// <inheritdoc />
    public override bool vop_ShiftRight_Rev(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a.IsType(AltaxoVariant.Content.VDouble))
      {
        DoubleColumn c1 = this;
        int len = c1._count;
        var c33 = new Altaxo.Data.DoubleColumn(len);
        long c22 = (long)(double)a;
        for (int i = 0; i < len; i++)
          c33._data[i] = c22 >> ((int)c1._data[i]);
        c33._count = len;
        b = c33;
        return true;
      }
      b = null;
      return false;
    }

    // ----------------------- Lesser operator -----------------------------------
    /// <summary>
    /// Compares whether one double column is less than another.
    /// </summary>
    /// <param name="c1">The first operand.</param>
    /// <param name="c2">The second operand.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Compares whether a double column is less than a scalar value.
    /// </summary>
    /// <param name="c1">The column.</param>
    /// <param name="c2">The scalar value.</param>
    /// <returns>The result column.</returns>
    public static Altaxo.Data.DoubleColumn operator <(Altaxo.Data.DoubleColumn c1, double c2)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
        c3._data[i] = (c1._data[i] < c2) ? 1 : 0;
      c3._count = len;
      return c3;
    }

    /// <summary>
    /// Compares whether a scalar value is less than a double column.
    /// </summary>
    /// <param name="c2">The scalar value.</param>
    /// <param name="c1">The column.</param>
    /// <returns>The result column.</returns>
    public static Altaxo.Data.DoubleColumn operator <(double c2, Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
        c3._data[i] = (c2 < c1._data[i]) ? 1 : 0;
      c3._count = len;
      return c3;
    }

    /// <inheritdoc />
    public override bool vop_Lesser(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a is Altaxo.Data.DoubleColumn)
      {
        b = this < (Altaxo.Data.DoubleColumn)a;
        return true;
      }
      b = null;
      return false;
    }

    /// <inheritdoc />
    public override bool vop_Lesser_Rev(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a is Altaxo.Data.DoubleColumn)
      {
        b = (Altaxo.Data.DoubleColumn)a < this;
        return true;
      }
      b = null;
      return false;
    }

    /// <inheritdoc />
    public override bool vop_Lesser(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a.IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = a;
        b = this < c22;
        return true;
      }
      b = null;
      return false;
    }

    /// <inheritdoc />
    public override bool vop_Lesser_Rev(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a.IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = a;
        b = c22 < this;
        return true;
      }
      b = null;
      return false;
    }

    // ----------------------- Greater operator -----------------------------------
    /// <summary>
    /// Compares whether one double column is greater than another.
    /// </summary>
    /// <param name="c1">The first operand.</param>
    /// <param name="c2">The second operand.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Compares whether a double column is greater than a scalar value.
    /// </summary>
    /// <param name="c1">The column.</param>
    /// <param name="c2">The scalar value.</param>
    /// <returns>The result column.</returns>
    public static Altaxo.Data.DoubleColumn operator >(Altaxo.Data.DoubleColumn c1, double c2)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
        c3._data[i] = (c1._data[i] > c2) ? 1 : 0;
      c3._count = len;
      return c3;
    }

    /// <summary>
    /// Compares whether a scalar value is greater than a double column.
    /// </summary>
    /// <param name="c2">The scalar value.</param>
    /// <param name="c1">The column.</param>
    /// <returns>The result column.</returns>
    public static Altaxo.Data.DoubleColumn operator >(double c2, Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
        c3._data[i] = (c2 > c1._data[i]) ? 1 : 0;
      c3._count = len;
      return c3;
    }

    /// <inheritdoc />
    public override bool vop_Greater(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a is Altaxo.Data.DoubleColumn)
      {
        b = this > (Altaxo.Data.DoubleColumn)a;
        return true;
      }
      b = null;
      return false;
    }

    /// <inheritdoc />
    public override bool vop_Greater_Rev(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a is Altaxo.Data.DoubleColumn)
      {
        b = (Altaxo.Data.DoubleColumn)a > this;
        return true;
      }
      b = null;
      return false;
    }

    /// <inheritdoc />
    public override bool vop_Greater(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a.IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = a;
        b = this > c22;
        return true;
      }
      b = null;
      return false;
    }

    /// <inheritdoc />
    public override bool vop_Greater_Rev(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a.IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = a;
        b = c22 > this;
        return true;
      }
      b = null;
      return false;
    }

    // ----------------------- LesserOrEqual operator -----------------------------------
    /// <summary>
    /// Compares whether one double column is less than or equal to another.
    /// </summary>
    /// <param name="c1">The first operand.</param>
    /// <param name="c2">The second operand.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Compares whether a double column is less than or equal to a scalar value.
    /// </summary>
    /// <param name="c1">The column.</param>
    /// <param name="c2">The scalar value.</param>
    /// <returns>The result column.</returns>
    public static Altaxo.Data.DoubleColumn operator <=(Altaxo.Data.DoubleColumn c1, double c2)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
        c3._data[i] = (c1._data[i] <= c2) ? 1 : 0;
      c3._count = len;
      return c3;
    }

    /// <summary>
    /// Compares whether a scalar value is less than or equal to a double column.
    /// </summary>
    /// <param name="c2">The scalar value.</param>
    /// <param name="c1">The column.</param>
    /// <returns>The result column.</returns>
    public static Altaxo.Data.DoubleColumn operator <=(double c2, Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
        c3._data[i] = (c2 <= c1._data[i]) ? 1 : 0;
      c3._count = len;
      return c3;
    }

    /// <inheritdoc />
    public override bool vop_LesserOrEqual(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a is Altaxo.Data.DoubleColumn)
      {
        b = this <= (Altaxo.Data.DoubleColumn)a;
        return true;
      }
      b = null;
      return false;
    }

    /// <inheritdoc />
    public override bool vop_LesserOrEqual_Rev(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a is Altaxo.Data.DoubleColumn)
      {
        b = (Altaxo.Data.DoubleColumn)a <= this;
        return true;
      }
      b = null;
      return false;
    }

    /// <inheritdoc />
    public override bool vop_LesserOrEqual(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a.IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = a;
        b = this <= c22;
        return true;
      }
      b = null;
      return false;
    }

    /// <inheritdoc />
    public override bool vop_LesserOrEqual_Rev(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a.IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = a;
        b = c22 <= this;
        return true;
      }
      b = null;
      return false;
    }

    // ----------------------- GreaterOrEqual operator -----------------------------------
    /// <summary>
    /// Compares whether one double column is greater than or equal to another.
    /// </summary>
    /// <param name="c1">The first operand.</param>
    /// <param name="c2">The second operand.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Compares whether a double column is greater than or equal to a scalar value.
    /// </summary>
    /// <param name="c1">The column.</param>
    /// <param name="c2">The scalar value.</param>
    /// <returns>The result column.</returns>
    public static Altaxo.Data.DoubleColumn operator >=(Altaxo.Data.DoubleColumn c1, double c2)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
        c3._data[i] = (c1._data[i] >= c2) ? 1 : 0;
      c3._count = len;
      return c3;
    }

    /// <summary>
    /// Compares whether a scalar value is greater than or equal to a double column.
    /// </summary>
    /// <param name="c2">The scalar value.</param>
    /// <param name="c1">The column.</param>
    /// <returns>The result column.</returns>
    public static Altaxo.Data.DoubleColumn operator >=(double c2, Altaxo.Data.DoubleColumn c1)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
        c3._data[i] = (c2 >= c1._data[i]) ? 1 : 0;
      c3._count = len;
      return c3;
    }

    /// <inheritdoc />
    public override bool vop_GreaterOrEqual(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a is Altaxo.Data.DoubleColumn)
      {
        b = this >= (Altaxo.Data.DoubleColumn)a;
        return true;
      }
      b = null;
      return false;
    }

    /// <inheritdoc />
    public override bool vop_GreaterOrEqual_Rev(DataColumn a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a is Altaxo.Data.DoubleColumn)
      {
        b = (Altaxo.Data.DoubleColumn)a >= this;
        return true;
      }
      b = null;
      return false;
    }

    /// <inheritdoc />
    public override bool vop_GreaterOrEqual(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a.IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = a;
        b = this >= c22;
        return true;
      }
      b = null;
      return false;
    }

    /// <inheritdoc />
    public override bool vop_GreaterOrEqual_Rev(AltaxoVariant a, [MaybeNullWhen(false)] out DataColumn b)
    {
      if (a.IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = a;
        b = c22 >= this;
        return true;
      }
      b = null;
      return false;
    }

    // --------------------------------- Unary Plus ----------------------------
    /// <summary>
    /// Applies the unary plus operator to a double column.
    /// </summary>
    /// <param name="c1">The operand.</param>
    /// <returns>The result column.</returns>
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

    /// <inheritdoc />
    public override bool vop_Plus(out DataColumn b)
    {
      b = +this;
      return true;
    }

    // --------------------------------- Unary Minus ----------------------------
    /// <summary>
    /// Applies the unary minus operator to a double column.
    /// </summary>
    /// <param name="c1">The operand.</param>
    /// <returns>The result column.</returns>
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

    /// <inheritdoc />
    public override bool vop_Minus(out DataColumn b)
    {
      b = -this;
      return true;
    }

    // --------------------------------- Unary NOT ----------------------------
    /// <summary>
    /// Applies the logical NOT operator to a double column.
    /// </summary>
    /// <param name="c1">The operand.</param>
    /// <returns>The result column.</returns>
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

    /// <inheritdoc />
    public override bool vop_Not(out DataColumn b)
    {
      b = !this;
      return true;
    }

    // --------------------------------- Unary Complement ----------------------------
    /// <summary>
    /// Applies the bitwise complement operator to a double column after converting values to integers.
    /// </summary>
    /// <param name="c1">The operand.</param>
    /// <returns>The result column.</returns>
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

    /// <inheritdoc />
    public override bool vop_Complement(out DataColumn b)
    {
      b = ~this;
      return true;
    }

    // --------------------------------- Unary Increment ----------------------------
    /// <summary>
    /// Applies the increment operator to a double column.
    /// </summary>
    /// <param name="c1">The operand.</param>
    /// <returns>The result column.</returns>
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

    /// <inheritdoc />
    public override bool vop_Increment(out DataColumn b)
    {
      int len = _count;
      var c33 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c33._data[i] = _data[i] + 1;
      }
      c33._count = len;
      b = c33;
      return true;
    }

    // --------------------------------- Unary Decrement ----------------------------
    /// <summary>
    /// Applies the decrement operator to a double column.
    /// </summary>
    /// <param name="c1">The operand.</param>
    /// <returns>The result column.</returns>
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

    /// <inheritdoc />
    public override bool vop_Decrement(out DataColumn b)
    {
      int len = _count;
      var c33 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c33._data[i] = _data[i] - 1;
      }
      c33._count = len;
      b = c33;
      return true;
    }

    // -----------------------------------------------------------------------------
    //
    //               arithmetic Functions
    //
    // -----------------------------------------------------------------------------

    /// <summary>
    /// Applies <see cref="Math.Abs(double)"/> to each element of a double column.
    /// </summary>
    /// <param name="c1">The source column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies <see cref="Math.Acos(double)"/> to each element of a double column.
    /// </summary>
    /// <param name="c1">The source column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies <see cref="Math.Asin(double)"/> to each element of a double column.
    /// </summary>
    /// <param name="c1">The source column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies <see cref="Math.Atan(double)"/> to each element of a double column.
    /// </summary>
    /// <param name="c1">The source column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies <see cref="Math.Atan2(double, double)"/> element-wise to two double columns.
    /// </summary>
    /// <param name="c1">The first operand column.</param>
    /// <param name="c2">The second operand column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies <see cref="Math.Atan2(double, double)"/> to a double column and a scalar value.
    /// </summary>
    /// <param name="c1">The first operand column.</param>
    /// <param name="c2">The second operand scalar.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies <see cref="Math.Atan2(double, double)"/> to a scalar value and a double column.
    /// </summary>
    /// <param name="c1">The first operand scalar.</param>
    /// <param name="c2">The second operand column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies <see cref="Math.Ceiling(double)"/> to each element of a double column.
    /// </summary>
    /// <param name="c1">The source column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies <see cref="Math.Cos(double)"/> to each element of a double column.
    /// </summary>
    /// <param name="c1">The source column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies <see cref="Math.Cosh(double)"/> to each element of a double column.
    /// </summary>
    /// <param name="c1">The source column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies <see cref="Math.Exp(double)"/> to each element of a double column.
    /// </summary>
    /// <param name="c1">The source column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies <see cref="Math.Floor(double)"/> to each element of a double column.
    /// </summary>
    /// <param name="c1">The source column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies <see cref="Math.IEEERemainder(double, double)"/> element-wise to two double columns.
    /// </summary>
    /// <param name="c1">The first operand column.</param>
    /// <param name="c2">The second operand column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies <see cref="Math.IEEERemainder(double, double)"/> to a double column and a scalar value.
    /// </summary>
    /// <param name="c1">The first operand column.</param>
    /// <param name="c2">The second operand scalar.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies <see cref="Math.IEEERemainder(double, double)"/> to a scalar value and a double column.
    /// </summary>
    /// <param name="c1">The first operand scalar.</param>
    /// <param name="c2">The second operand column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies <see cref="Math.Log(double)"/> to each element of a double column.
    /// </summary>
    /// <param name="c1">The source column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies <see cref="Math.Log(double, double)"/> element-wise to two double columns.
    /// </summary>
    /// <param name="c1">The value column.</param>
    /// <param name="c2">The base column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies <see cref="Math.Log(double, double)"/> to a double column with a scalar base.
    /// </summary>
    /// <param name="c1">The value column.</param>
    /// <param name="c2">The base scalar.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies <see cref="Math.Log(double, double)"/> to a scalar value with element-wise bases from a double column.
    /// </summary>
    /// <param name="c1">The value scalar.</param>
    /// <param name="c2">The base column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies <see cref="Math.Log10(double)"/> to each element of a double column.
    /// </summary>
    /// <param name="c1">The source column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies <see cref="Math.Max(double, double)"/> element-wise to two double columns.
    /// </summary>
    /// <param name="c1">The first operand column.</param>
    /// <param name="c2">The second operand column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies <see cref="Math.Max(double, double)"/> to a double column and a scalar value.
    /// </summary>
    /// <param name="c1">The first operand column.</param>
    /// <param name="c2">The second operand scalar.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies <see cref="Math.Max(double, double)"/> to a scalar value and a double column.
    /// </summary>
    /// <param name="c1">The first operand scalar.</param>
    /// <param name="c2">The second operand column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies <see cref="Math.Min(double, double)"/> element-wise to two double columns.
    /// </summary>
    /// <param name="c1">The first operand column.</param>
    /// <param name="c2">The second operand column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies <see cref="Math.Min(double, double)"/> to a double column and a scalar value.
    /// </summary>
    /// <param name="c1">The first operand column.</param>
    /// <param name="c2">The second operand scalar.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies <see cref="Math.Min(double, double)"/> to a scalar value and a double column.
    /// </summary>
    /// <param name="c1">The first operand scalar.</param>
    /// <param name="c2">The second operand column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies <see cref="Math.Pow(double, double)"/> element-wise to two double columns.
    /// </summary>
    /// <param name="c1">The base column.</param>
    /// <param name="c2">The exponent column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Raises each element of a double column to the specified integer power.
    /// </summary>
    /// <param name="c1">The base column.</param>
    /// <param name="c2">The exponent.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Squares each element of a double column.
    /// </summary>
    /// <param name="c1">The source column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Cubes each element of a double column.
    /// </summary>
    /// <param name="c1">The source column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Raises each element of a double column to the fourth power.
    /// </summary>
    /// <param name="c1">The source column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Raises each element of a double column to the fifth power.
    /// </summary>
    /// <param name="c1">The source column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Raises each element of a double column to the sixth power.
    /// </summary>
    /// <param name="c1">The source column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Raises each element of a double column to the seventh power.
    /// </summary>
    /// <param name="c1">The source column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Raises each element of a double column to the eighth power.
    /// </summary>
    /// <param name="c1">The source column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Raises each element of a double column to the ninth power.
    /// </summary>
    /// <param name="c1">The source column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies <see cref="Math.Pow(double, double)"/> to a double column with a scalar exponent.
    /// </summary>
    /// <param name="c1">The base column.</param>
    /// <param name="c2">The exponent scalar.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies <see cref="Math.Pow(double, double)"/> to a scalar base with element-wise exponents from a double column.
    /// </summary>
    /// <param name="c1">The base scalar.</param>
    /// <param name="c2">The exponent column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies <see cref="Math.Round(double)"/> to each element of a double column.
    /// </summary>
    /// <param name="c1">The source column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies <see cref="Math.Round(double, int)"/> to a double column with element-wise precision values from another column.
    /// </summary>
    /// <param name="c1">The value column.</param>
    /// <param name="c2">The precision column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies <see cref="Math.Round(double, int)"/> to a double column with a scalar precision.
    /// </summary>
    /// <param name="c1">The value column.</param>
    /// <param name="c2">The precision.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies <see cref="Math.Round(double, int)"/> to a scalar value with element-wise precision values from a double column.
    /// </summary>
    /// <param name="c1">The value scalar.</param>
    /// <param name="c2">The precision column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies <see cref="Math.Sign(double)"/> to each element of a double column.
    /// </summary>
    /// <param name="c1">The source column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies <see cref="Math.Sin(double)"/> to each element of a double column.
    /// </summary>
    /// <param name="c1">The source column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies <see cref="Math.Sinh(double)"/> to each element of a double column.
    /// </summary>
    /// <param name="c1">The source column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies <see cref="Math.Sqrt(double)"/> to each element of a double column.
    /// </summary>
    /// <param name="c1">The source column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies <see cref="Math.Tan(double)"/> to each element of a double column.
    /// </summary>
    /// <param name="c1">The source column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies <see cref="Math.Tanh(double)"/> to each element of a double column.
    /// </summary>
    /// <param name="c1">The source column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies a unary mapping function to each element of a double column.
    /// </summary>
    /// <param name="function">The mapping function.</param>
    /// <param name="c1">The source column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies a binary mapping function element-wise to two double columns.
    /// </summary>
    /// <param name="function">The mapping function.</param>
    /// <param name="c1">The first operand column.</param>
    /// <param name="c2">The second operand column.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies a binary mapping function to a double column and a scalar value.
    /// </summary>
    /// <param name="function">The mapping function.</param>
    /// <param name="c1">The first operand column.</param>
    /// <param name="c2">The second operand scalar.</param>
    /// <returns>The result column.</returns>
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

    /// <summary>
    /// Applies a binary mapping function to a scalar value and a double column.
    /// </summary>
    /// <param name="function">The mapping function.</param>
    /// <param name="c1">The first operand scalar.</param>
    /// <param name="c2">The second operand column.</param>
    /// <returns>The result column.</returns>
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
    /// <returns>The fractional index if a containing interval is found; otherwise, <see langword="null"/>.</returns>
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
