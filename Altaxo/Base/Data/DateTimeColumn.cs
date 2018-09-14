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
using Altaxo.Serialization;

namespace Altaxo.Data
{
  /// <summary>
  /// Summary description for Altaxo.Data.DateTimeColumn.
  /// </summary>
  public class DateTimeColumn
    :
    Altaxo.Data.DataColumn,
    INumericColumn
  {
    private DateTime[] _data;
    private int _capacity; // shortcut to m_Array.Length;
    private int _count;
    public static readonly DateTime NullValue = DateTime.MinValue;

    public DateTimeColumn()
    {
    }

    public DateTimeColumn(int initialcapacity)
    {
      _count = 0;
      _data = new DateTime[initialcapacity];
      _capacity = initialcapacity;
    }

    public DateTimeColumn(DateTimeColumn from)
    {
      _count = from._count;
      _capacity = from._capacity;
      _data = null == from._data ? null : (DateTime[])from._data.Clone();
    }

    public override object Clone()
    {
      return new DateTimeColumn(this);
    }

    #region "Serialization"

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Altaxo.Data.DateTimeColumn), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (Altaxo.Data.DateTimeColumn)obj;
        // serialize the base class
        info.AddBaseValueEmbedded(s, typeof(Altaxo.Data.DataColumn));

        if (null == info.GetProperty("Altaxo.Data.DataColumn.SaveAsTemplate"))
          info.AddArray("Data", s._data, s._count);
        else
          info.AddArray("Data", s._data, 0);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        Altaxo.Data.DateTimeColumn s = null != o ? (Altaxo.Data.DateTimeColumn)o : new Altaxo.Data.DateTimeColumn();

        // deserialize the base class
        info.GetBaseValueEmbedded(s, typeof(Altaxo.Data.DataColumn), parent);

        int count = info.GetInt32Attribute("Count");
        s._data = new DateTime[count];
        info.GetArray(s._data, count);
        s._capacity = null == s._data ? 0 : s._data.Length;
        s._count = s._capacity;
        return s;
      }
    }

    #endregion "Serialization"

    /// <summary>
    /// Gets the type of the colum's items.
    /// </summary>
    /// <value>
    /// The type of the item.
    /// </value>
    public override Type ItemType { get { return typeof(DateTime); } }

    public override int Count
    {
      get
      {
        return _count;
      }
    }

    public DateTime[] Array
    {
      get
      {
        int len = Count;
        var arr = new DateTime[len];
        System.Array.Copy(_data, 0, arr, 0, len);
        return arr;
      }

      set
      {
        _data = (DateTime[])value.Clone();
        _count = _data.Length;
        _capacity = _data.Length;
        EhSelfChanged(0, _count, true);
      }
    }

    protected internal DateTime GetValueDirect(int idx)
    {
      return _data[idx];
    }

    public override System.Type GetColumnStyleType()
    {
      return typeof(Altaxo.Worksheet.DateTimeColumnStyle);
    }

    public override void CopyDataFrom(object o)
    {
      var oldCount = _count;
      _count = 0;

      if (o is DateTimeColumn)
      {
        var src = (DateTimeColumn)o;
        _data = null == src._data ? null : (DateTime[])src._data.Clone();
        _capacity = _data?.Length ?? 0;
        _count = src._count;
      }
      else
      {
        if (o is ICollection)
          Realloc((o as ICollection).Count); // Prealloc the array if count of the collection is known beforehand

        if (o is IEnumerable<DateTime>)
        {
          var src = (IEnumerable<DateTime>)o;
          _count = 0;
          foreach (var it in src)
          {
            if (_count >= _capacity)
              Realloc(_count);
            _data[_count++] = it;
          }
        }
        else if (o is IEnumerable<double>)
        {
          var src = (IEnumerable<double>)o;
          _count = 0;
          foreach (var it in src)
          {
            if (_count >= _capacity)
              Realloc(_count);
            _data[_count++] = new DateTime((long)(1e7 * it));
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
      for (; _count > 0 && _data[_count - 1] != DateTime.MinValue; _count--)
        ;
    }

    protected void Realloc(int i)
    {
      int newcapacity1 = (int)(_capacity * _increaseFactor + _addSpace);
      int newcapacity2 = i + _addSpace + 1;
      int newcapacity = newcapacity1 > newcapacity2 ? newcapacity1 : newcapacity2;

      var newarray = new DateTime[newcapacity];
      if (_count > 0)
      {
        System.Array.Copy(_data, newarray, _count);
      }

      _data = newarray;
      _capacity = _data.Length;
    }

    // indexers
    public override void SetValueAt(int i, AltaxoVariant val)
    {
      try
      {
        this[i] = val.ToDateTime();
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

    double Altaxo.Calc.LinearAlgebra.INumericSequence<double>.this[int i]
    {
      get
      {
        return i < _count ? this[i].Ticks / 1E7 : double.NaN;
      }
    }

    double Altaxo.Data.INumericColumn.this[int i]
    {
      get
      {
        return i < _count ? this[i].Ticks / 1E7 : double.NaN;
      }
    }

    public override bool IsElementEmpty(int i)
    {
      return i < _count ? (DateTime.MinValue == _data[i]) : true;
    }

    public override void SetElementEmpty(int i)
    {
      if (i < _count)
        this[i] = NullValue;
    }

    public new DateTime this[int i]
    {
      get
      {
        if (i >= 0 && i < _count)
          return _data[i];
        return DateTime.MinValue;
      }
      set
      {
        bool bCountDecreased = false;

        if (i < 0)
          throw new ArgumentOutOfRangeException(string.Format("Index<0 (i={0}) while trying to set element of column {1} ({2})", i, Name, FullName));

        if (value == DateTime.MinValue)
        {
          if (i < _count - 1) // i is inside the used range
          {
            _data[i] = value;
          }
          else if (i == (_count - 1)) // m_Count is then decreasing
          {
            for (_count = i; _count > 0 && (DateTime.MinValue == _data[_count - 1]); --_count)
              ;
            bCountDecreased = true;
            ;
          }
          else // i is above the used area
          {
            return; // no need for a change notification here
          }
        }
        else // value is a valid value
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
              _data[k] = DateTime.MinValue; // fill range between used range and new element with voids

            _data[i] = value;
            _count = i + 1;
          }
          else // i is outside of capacity, then realloc the array
          {
            Realloc(i);

            for (int k = _count; k < i; k++)
              _data[k] = DateTime.MinValue; // fill range between used range and new element with voids

            _data[i] = value;
            _count = i + 1;
          }
        }
        EhSelfChanged(i, i + 1, bCountDecreased);
      } // end set
    } // end indexer

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

    #region "Operators"

    // -----------------------------------------------------------------------------
    //
    //                        Operators
    //
    // -----------------------------------------------------------------------------

    // ----------------------- Addition operator -----------------------------------
    public static Altaxo.Data.DateTimeColumn operator +(Altaxo.Data.DateTimeColumn c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c1.Count < c2.Count ? c1.Count : c2.Count;
      var c3 = new Altaxo.Data.DateTimeColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = c1._data[i].AddSeconds(c2.GetValueDirect(i));
      }

      c3._count = len;

      return c3;
    }

    public static Altaxo.Data.DateTimeColumn operator +(Altaxo.Data.DateTimeColumn c1, double c2)
    {
      int len = c1._count;
      var c3 = new Altaxo.Data.DateTimeColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = c1._data[i].AddSeconds(c2);
      }

      c3._count = len;

      return c3;
    }

    public static Altaxo.Data.DoubleColumn operator -(Altaxo.Data.DateTimeColumn c1, Altaxo.Data.DateTimeColumn c2)
    {
      return Altaxo.Data.DoubleColumn.Subtraction(c1, c2);
    }

    public static Altaxo.Data.DateTimeColumn operator -(Altaxo.Data.DateTimeColumn c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c1.Count < c2.Count ? c1.Count : c2.Count;
      var c3 = new Altaxo.Data.DateTimeColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = c1._data[i].AddSeconds(-c2.GetValueDirect(i));
      }

      c3._count = len;

      return c3;
    }

    public static Altaxo.Data.DateTimeColumn operator -(Altaxo.Data.DateTimeColumn c1, double c2)
    {
      var c3 = new Altaxo.Data.DateTimeColumn(c1._count);
      int len = c1._count;
      for (int i = 0; i < len; i++)
      {
        c3._data[i] = c1._data[i].AddSeconds(-c2);
      }

      c3._count = len;

      return c3;
    }

    public static Altaxo.Data.DoubleColumn operator -(Altaxo.Data.DateTimeColumn c1, DateTime c2)
    {
      return Altaxo.Data.DoubleColumn.Subtraction(c1, c2);
    }

    public static Altaxo.Data.DoubleColumn operator -(DateTime c1, Altaxo.Data.DateTimeColumn c2)
    {
      return Altaxo.Data.DoubleColumn.Subtraction(c1, c2);
    }

    public override bool vop_Subtraction(DataColumn c2, out DataColumn c3)
    {
      if (c2 is Altaxo.Data.DateTimeColumn)
      {
        c3 = this - (Altaxo.Data.DateTimeColumn)c2;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_Subtraction_Rev(DataColumn c2, out DataColumn c3)
    {
      if (c2 is Altaxo.Data.DateTimeColumn)
      {
        c3 = (Altaxo.Data.DateTimeColumn)c2 - this;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_Subtraction(AltaxoVariant c2, out DataColumn c3)
    {
      if (c2.IsType(AltaxoVariant.Content.VDateTime))
      {
        var c22 = (DateTime)c2;
        c3 = this - c22;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_Subtraction_Rev(AltaxoVariant c2, out DataColumn c3)
    {
      if (c2.IsType(AltaxoVariant.Content.VDateTime))
      {
        var c22 = (DateTime)c2;
        c3 = c22 - this;
        return true;
      }
      c3 = null;
      return false;
    }

    #endregion "Operators"
  } // end Altaxo.Data.DateTimeColumn
}
