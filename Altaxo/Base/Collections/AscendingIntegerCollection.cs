#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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
  public class AscendingIntegerCollection : IAscendingIntegerCollection
  {
    protected System.Collections.SortedList _list = new System.Collections.SortedList();


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
    /// Returns the integer stored at position <code>i</code>.
    /// </summary>
    public int this[int i]
    {
      get { return (int)_list.GetKey(i); }
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
    /// Removes an integer value from the collection.
    /// </summary>
    /// <param name="nValue">The integer value to remove.</param>
    public void Remove(int nValue)
    {
      _list.Remove(nValue);
    }

    /// <summary>
    /// Clears the collection, i.e. removes all entries.
    /// </summary>
    public void Clear()
    {
      _list.Clear();
    }
  }
}
