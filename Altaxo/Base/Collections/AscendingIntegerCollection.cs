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
  /// Stores a number of arbitrary integer values in ascending order.
  /// </summary>
  public class AscendingIntegerCollection : IAscendingIntegerCollection, System.ICloneable
  {
    protected System.Collections.Generic.SortedList<int,object> _list = new System.Collections.Generic.SortedList<int,object>();

    #region Serialization
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AscendingIntegerCollection),0)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo  info)
      {
        AscendingIntegerCollection s = (AscendingIntegerCollection)obj;
        int count = s.GetRangeCount();
        info.CreateArray("Ranges",count);
        int currentpos=0, rangestart=0, rangecount=0;
        while(s.GetNextRangeAscending(ref currentpos, ref rangestart, ref rangecount))
        {
          info.CreateElement("e");
          info.AddValue("Start",rangestart);
          info.AddValue("Count",rangecount);
          info.CommitElement();
        }
        info.CommitArray();

      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo  info, object parent)
      {
        AscendingIntegerCollection s = null!=o ? (AscendingIntegerCollection)o : new AscendingIntegerCollection();

        int count = info.OpenArray();

        for(int i=0;i<count;i++)
        {
          info.OpenElement();
          int rangestart = info.GetInt32("Start");
          int rangecount = info.GetInt32("Count");
          info.CloseElement();
          s.AddRange(rangestart,rangecount);
        }
        info.CloseArray(count);
        return s;
      }
    }

    #endregion

    /// <summary>
    /// Creates an empty collection.
    /// </summary>
    public AscendingIntegerCollection()
    {
    }

    /// <summary>
    /// Creates a collection cloned from another <see cref="AscendingIntegerCollection" />.
    /// </summary>
    /// <param name="from"></param>
    public AscendingIntegerCollection(AscendingIntegerCollection from)
    {
      _list = new System.Collections.Generic.SortedList<int,object>(from._list);
    }

    /// <summary>
    /// Creates the collection copied from another <see cref="IAscendingIntegerCollection" />.
    /// </summary>
    /// <param name="from"></param>
    public AscendingIntegerCollection(IAscendingIntegerCollection from)
    {
      for(int i=0;i<from.Count;i++)
        this.Add(from[i]);
    }

    /// <summary>
    /// Number of integer values stored in this collection
    /// </summary>
    public int Count
    {
      get
      {
        return _list.Count;
      }
    }

    /// <summary>
    /// Returns the number of integer ranges this collection represents.
    /// </summary>
    /// <returns>The number of contiguous integer ranges.</returns>
    public int GetRangeCount()
    {
      int result=0;
      int currentpos=0, rangestart=0, rangecount=0;
      while(GetNextRangeAscending(ref currentpos, ref rangestart, ref rangecount))
        result++;

      return result;
    }

    /// <summary>
    /// Returns the integer stored at position <code>i</code>.
    /// </summary>
    public int this[int i]
    {
      get { return _list.Keys[i]; }
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
        rangestart = this[currentposition];
        int previous = rangestart;
        rangecount=1;
        for(currentposition=currentposition+1;currentposition<Count;currentposition++)
        {
          if(this[currentposition]==(previous+1))
          {
            previous++;
            rangecount++;
          }
          else
          {
            break;
          }
        }

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
        rangestart = this[currentposition];
        rangecount=1;
        for(currentposition=currentposition-1;currentposition>=0;currentposition--)
        {
          if(this[currentposition]==(rangestart-1))
          {
            rangestart--;
            rangecount++;
          }
          else
          {
            break;
          }
        }

        return true;
      }
    }

    /// <summary>
    /// Returns true, if the integer <code>nValue</code> is contained in this collection.
    /// </summary>
    /// <param name="nValue">The integer value to test for membership.</param>
    /// <returns>True if the integer value is member of the collection.</returns>
    public bool Contains(int nValue)
    {
      return _list.ContainsKey(nValue);
    }


    /// <summary>
    /// Adds an integer value to the collection.
    /// </summary>
    /// <param name="nValue">The integer value to add.</param>
    public void Add(int nValue)
    {
      _list.Add(nValue,null);
    }

    /// <summary>
    /// Adds all values from another <see cref="IAscendingIntegerCollection" />.
    /// </summary>
    /// <param name="from">The source collection.</param>
    public void Add(IAscendingIntegerCollection from)
    {
      int end = from.Count;
      for(int i=0;i<end;i++)
        Add(from[i]);
    }

    /// <summary>
    /// Adds an integer range given by start and count to the collection.
    /// </summary>
    /// <param name="rangestart">First number of the integer range.</param>
    /// <param name="rangecount">Length of the integer range.</param>
    public void AddRange(int rangestart, int rangecount)
    {
      for(int i=0;i<rangecount;i++)
        Add(rangestart+i);
    }
    /// <summary>
    /// Removes an integer value from the collection.
    /// </summary>
    /// <param name="nValue">The integer value to remove.</param>
    public void Remove(int nValue)
    {
      _list.Remove(nValue);
    }

    /// <summary>
    /// Removes an integer at a given position from the collection (e.g. in general not the integer you provide as argument is removed (!)).
    /// </summary>
    /// <param name="position"></param>
    public void RemoveAt(int position)
    {
      _list.RemoveAt(position);
    }

    /// <summary>
    /// Clears the collection, i.e. removes all entries.
    /// </summary>
    public void Clear()
    {
      _list.Clear();
    }
    #region ICloneable Members

    public object Clone()
    {
      return new AscendingIntegerCollection(this);
    }

    #endregion

    #region IEnumerable<int> Members

    public System.Collections.Generic.IEnumerator<int> GetEnumerator()
    {
      return _list.Keys.GetEnumerator();
    }

    #endregion

    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return _list.Keys.GetEnumerator();
    }

    #endregion
  }
}
