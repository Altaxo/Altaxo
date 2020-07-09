#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2019 Dr. Dirk Lellinger
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

namespace Altaxo.Data
{
  public class BooleanColumn : DataColumn, Altaxo.Calc.LinearAlgebra.IROVector<bool?>
  {
    static readonly BitArray _emptyBitArray = new BitArray(0);
    private BitArray _data = _emptyBitArray;
    private BitArray _inUse = _emptyBitArray;
    private int _capacity;
    private int _count;
    private const int MaxCount = 268435455 * 8;

    #region "Serialization"

    /// <summary>
    /// 2019-10-07 Initial version
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Altaxo.Data.BooleanColumn), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (Altaxo.Data.BooleanColumn)obj;
        // serialize the base class
        info.AddBaseValueEmbedded(s, typeof(Altaxo.Data.DataColumn));

        if ("true" == info.GetProperty(DataTable.SerializationInfoProperty_SaveAsTemplate))
          info.AddArray("Data", s._data, s._inUse, 0);
        else
          info.AddArray("Data", s._data, s._inUse, s._count);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = o as BooleanColumn ?? new Altaxo.Data.BooleanColumn();

        // deserialize the base class
        info.GetBaseValueEmbedded(s, typeof(Altaxo.Data.DataColumn), parent);

        int count = info.GetInt32Attribute("Count");
        s._data = new BitArray(count);
        s._inUse = new BitArray(count);
        info.GetArray(s._data, s._inUse, count);
        s._capacity = null == s._data ? 0 : s._data.Length;
        s._count = s._capacity;

        return s;
      }
    }

    #endregion "Serialization"


    #region Construction

    public BooleanColumn()
    {

    }

    public BooleanColumn(int initialcapacity)
    {
      _count = 0;
      _inUse = new BitArray(initialcapacity);
      _data = new BitArray(initialcapacity);
      _capacity = initialcapacity;
    }

    public BooleanColumn(BooleanColumn from)
    {
      _count = from._count;
      _capacity = from._capacity;
      _inUse = (BitArray)from._inUse.Clone();
      _data = (BitArray)from._data.Clone();
    }

    #endregion

    #region Access

    public new bool? this[int i]
    {
      get
      {
        if (i >= 0 && i < _count)
          return _inUse[i] ? (bool?)_data[i] : null;
        else
          return null;
      }
      set
      {
        bool hasCountDecreased = false;

        if (i < 0)
          throw new ArgumentOutOfRangeException(string.Format("Index<0 (i={0}) while trying to set element of column {1} ({2})", i, Name, FullName));

        if (value is null)
        {
          if (i < _count - 1) // i is inside the used range
          {
            _inUse[i] = false;
            _data[i] = false;
          }
          else if (i == (_count - 1)) // m_Count is then decreasing
          {
            for (_count = i; _count > 0 && !_inUse[_count - 1]; --_count)
              ;
            hasCountDecreased = true;
            ;
          }
          else // i is above the used area
          {
            return; // no need for a change notification here
          }
        }
        else // value is not null
        {
          if (i < _count) // i is inside the used range
          {
            _inUse[i] = true;
            _data[i] = value.Value;
          }
          else if (i == _count && i < _capacity) // i is the next value after the used range
          {
            _inUse[i] = true;
            _data[i] = value.Value;
            _count = i + 1;
          }
          else if (i > _count && i < _capacity) // is is outside used range, but inside capacity of array
          {
            for (int k = _count; k < i; k++)
            {
              _inUse[k] = false;
              _data[k] = false; // fill range between used range and new element with voids
            }

            _inUse[i] = true;
            _data[i] = value.Value;
            _count = i + 1;
          }
          else // i is outside of capacity, then realloc the array
          {
            Realloc(i);

            for (int k = _count; k < i; k++)
            {
              _inUse[k] = false;
              _data[k] = false; // fill range between used range and new element with voids
            }

            _inUse[i] = true;
            _data[i] = value.Value;
            _count = i + 1;
          }
        }
        EhSelfChanged(i, i + 1, hasCountDecreased);
      } // end set
    } // end indexer

    protected void Realloc(int i)
    {
      int newcapacity1 = (int)(_capacity * _increaseFactor + _addSpace);
      int newcapacity2 = i + _addSpace + 1;
      int newcapacity = newcapacity1 > newcapacity2 ? newcapacity1 : newcapacity2;
      newcapacity += 32 - newcapacity % 32; // round to entire Int32
      newcapacity = Math.Min(newcapacity, MaxCount);

      if (i > newcapacity)
        throw new ArgumentOutOfRangeException(string.Format("Unable to allocate {0} rows!", i));

      if (_inUse is null || _data is null)
      {
        _inUse = new BitArray(newcapacity);
        _data = new BitArray(newcapacity);
      }
      else
      {
        _inUse.Length = newcapacity;
        _data.Length = newcapacity;
      }
      _capacity = _data.Length;
    }

    #endregion

    #region Overridden functions

    public override object Clone()
    {
      return new BooleanColumn(this);
    }

    public override int Count
    {
      get { return _count; }
    }

    int Calc.LinearAlgebra.IROVector<bool?>.Length
    {
      get { return _count; }
    }

    IEnumerator<bool?> IEnumerable<bool?>.GetEnumerator()
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
        this[i] = val.ToNullableBoolean();
      }
      catch (Exception ex)
      {
        throw new ApplicationException(string.Format("Error: Try to set {0}[{1}] with the string {2}, exception: {3}", TypeAndName, i, val.ToString(), ex.Message));
      }
    }

    public override AltaxoVariant GetVariantAt(int i)
    {
      var v = this[i];
      return v.HasValue ? (v.Value ? new AltaxoVariant(1.0) : new AltaxoVariant(0.0)) : new AltaxoVariant();
    }

    public override bool IsElementEmpty(int i)
    {
      return i >= _inUse.Length || !_inUse[i];
    }

    public override void SetElementEmpty(int i)
    {
      if (i < _count)
        this[i] = null;
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
      {
        _inUse[i] = _inUse[j];
        _data[i] = _data[j];
      }

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
      {
        _inUse[j] = _inUse[i];
        _data[j] = _data[i];
      }

      for (int i = nInsBeforeColumn + nInsCount - 1; i >= nInsBeforeColumn; i--)
      {
        _inUse[i] = false;
        _data[i] = false;
      }

      _count = newlen;
      EhSelfChanged(nInsBeforeColumn, _count, false);
    }

    public override void CopyDataFrom(object o)
    {
      var oldCount = _count;
      _count = 0;

      if (o is BooleanColumn src)
      {
        _inUse = (BitArray)src._inUse.Clone();
        _data = (BitArray)src._data.Clone();
        _capacity = _data?.Length ?? 0;
        _count = src._count;
      }
      else
      {
        if (o is ICollection ocoll)
          Realloc(ocoll.Count); // Prealloc the array if count of the collection is known beforehand

        if (o is IEnumerable<bool> srcBool)
        {
          _count = 0;
          foreach (var it in srcBool)
          {
            if (_count >= _capacity)
              Realloc(_count);
            _inUse[_count] = true;
            _data[_count] = it;
            ++_count;
          }
        }

        else if (o is IEnumerable<AltaxoVariant> srcVariant)
        {
          _count = 0;
          foreach (var it in srcVariant)
          {
            if (_count >= _capacity)
              Realloc(_count);

            var v = it.ToNullableBoolean();
            _inUse[_count] = v.HasValue;
            _data[_count] = v ?? false;
            ++_count;
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
      return typeof(Altaxo.Worksheet.BooleanColumnStyle);
    }

    #endregion Overridden functions


    public override Type ItemType => typeof(bool?);




  }
}
