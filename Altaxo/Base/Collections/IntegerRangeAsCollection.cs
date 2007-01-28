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

namespace Altaxo.Collections
{
  /// <summary>
  /// This class represents a simple integer range specified by start and count, that can be used as a lightweight substitute for a <see cref="AscendingIntegerCollection" /> if 
  /// the selection is contiguous.
  /// </summary>
  public class IntegerRangeAsCollection : IAscendingIntegerCollection, ICloneable
  {
    /// <summary>
    /// The starting point of the integer range.
    /// </summary>
    int _start;

    /// <summary>
    /// The width of the integer range. The range is from <code>_start</code> until (including) <code>_start + _count-1</code>. 
    /// </summary>
    int _count;

    #region Serialization
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(IntegerRangeAsCollection),0)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo  info)
      {
        IntegerRangeAsCollection s = (IntegerRangeAsCollection)obj;
        info.AddValue("Start",s._start);
        info.AddValue("Count",s._count);

      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo  info, object parent)
      {
        IntegerRangeAsCollection s = null!=o ? (IntegerRangeAsCollection)o : new IntegerRangeAsCollection();

        s._start = info.GetInt32("Start");
        s._count = info.GetInt32("Count");

        return s;
      }
    }

    #endregion


    /// <summary>
    /// Constructs the range by giving a start and the width.
    /// </summary>
    /// <param name="start">The range start.</param>
    /// <param name="count">The range width, i.e the range is from start until (including) start+count-1.</param>
    public IntegerRangeAsCollection(int start, int count)
    {
      _start = start;
      _count = count;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="from">Object from which to copy the data.</param>
    public IntegerRangeAsCollection(IntegerRangeAsCollection from)
    {
      this._start = from._start;
      this._count = from._count;
    }

    /// <summary>
    /// Empty constructor for deserialisation.
    /// </summary>
    protected IntegerRangeAsCollection()
    {
      _start=_count=0;
    }

    /// <summary>
    /// The width of the range.
    /// </summary>
    public int Count 
    {
      get 
      {
        return _count;
      }
    }

    /// <summary>
    /// Returns the i-th number of the range, starting from the start of the range.
    /// </summary>
    public int this[int i]
    {
      get { return _start + i; }
    }

    /// <summary>
    /// Returns true, if the integer <code>nValue</code> is contained in this collection.
    /// </summary>
    /// <param name="nValue">The integer value to test for membership.</param>
    /// <returns>True if the integer value is member of the collection.</returns>
    public bool Contains(int nValue)
    {
      return nValue>=_start && nValue<(_start+_count);
    }


    /// <summary>
    /// Get the next range (i.e. a contiguous range of integers) in ascending order.
    /// </summary>
    /// <param name="currentposition">The current position into this collection. Use 0 for the first time. On return, this is the next position.</param>
    /// <param name="rangestart">Returns the starting index of the contiguous range.</param>
    /// <param name="rangecount">Returns the width of the range.</param>
    /// <returns>True if the returned data are valid, false if there is no more data.</returns>
    /// <remarks>You can use this function in a while loop:
    /// <code>
    /// int rangestart, rangecount;
    /// int currentPosition=0;
    /// while(GetNextRangeAscending(ref currentPosition, out rangestart, out rangecount))
    ///   {
    ///   // do your things here
    ///   }
    /// </code></remarks>
    public bool GetNextRangeAscending(ref int currentposition, ref int rangestart, ref int rangecount)
    {
      if(currentposition<0 || currentposition>=Count)
      {
        return false;
      }
      else
      {
        rangestart = _start + currentposition;
        rangecount = _start + _count - rangestart;
        currentposition = _count;
        return true;
      }
    }

    /// <summary>
    /// Get the next range (i.e. a contiguous range of integers) in descending order.
    /// </summary>
    /// <param name="currentposition">The current position into this collection. Use Count-1 for the first time. On return, this is the next position.</param>
    /// <param name="rangestart">Returns the starting index of the contiguous range.</param>
    /// <param name="rangecount">Returns the width of the range.</param>
    /// <returns>True if the range data are valid, false if there is no more data. Used as end-of-loop indicator.</returns>
    /// <remarks>You can use this function in a while loop:
    /// <code>
    /// int rangestart, rangecount;
    /// int currentPosition=selection.Count-1;
    /// while(selection.GetNextRangeAscending(currentPosition,out rangestart, out rangecount))
    ///   {
    ///   // do your things here
    ///   }
    /// </code></remarks>
    public bool GetNextRangeDescending(ref int currentposition, ref int rangestart, ref int rangecount)
    {
      if(currentposition<0 || currentposition>=Count)
      {
        return false;
      }
      else
      {
        rangestart = _start;
        rangecount = currentposition+1;
        currentposition = -1;
        return true;
      }
    }
    #region ICloneable Members

    public object Clone()
    {
      return new IntegerRangeAsCollection(this);
    }

    #endregion

    #region IEnumerable<int> Members

    public System.Collections.Generic.IEnumerator<int> GetEnumerator()
    {
      for (int i = 0; i < _count; i++)
        yield return i + _start;
    }

    #endregion

    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      for (int i = 0; i < _count; i++)
        yield return i + _start;
    }

    #endregion
  }
}
