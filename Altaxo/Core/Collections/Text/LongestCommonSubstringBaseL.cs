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

// #define LinkedListDebug

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Collections.Text
{
  /// <summary>
  /// Base class for problem solvers for longest common substring problems using a doubly linked list of class instances.
  /// </summary>
  /// <remarks>
  /// For details of the algorithm see the very nice paper by Michael Arnold and Enno Ohlebusch, 'Linear Time Algorithms for Generalizations of the Longest Common Substring Problem', Algorithmica (2011) 60; 806-818; DOI: 10.1007/s00453-009-9369-1.
  /// This code was adopted from the C++ sources from the web site of the authors at http://www.uni-ulm.de/in/theo/research/sequana.html.
  /// </remarks>
  public class LongestCommonSubstringBaseL : LongestCommonSubstringBase
  {
    #region internal types

    /// <summary>
    /// Element of the linked list array.
    /// </summary>
    protected class LLElement
    {
      /// <summary>First occurence in the suffix array.</summary>
      public int Idx;

      /// <summary>Longest common prefix.</summary>
      public int Lcp;

      /// <summary>Next list element in the array, or null if no such element exists.</summary>
      public LLElement Next;

      /// <summary>Previous list element in the array, or null if no such element exists.</summary>
      public LLElement Previous;

      /// <summary>First list element of the interval to which this list element belongs.</summary>
      public LLElement IntervalBegin;

      /// <summary>Last list element of the interval to which this list element belongs.</summary>
      public LLElement IntervalEnd;

      /// <summary>Length of the interval (number of nodes) that belong to the interval to which this list element belongs.</summary>
      public int IntervalSize;

#if LinkedListDebug
			public int DebugId;
			static int DebugIdGen = -1;
#endif

      public LLElement()
      {
        IntervalBegin = this;
        IntervalEnd = this;
        IntervalSize = 1;

#if LinkedListDebug
				DebugId = ++DebugIdGen;
#endif
      }

      public LLElement(int lcp, int idx)
      {
        Lcp = lcp;
        Idx = idx;
        IntervalBegin = this;
        IntervalEnd = this;
        IntervalSize = 1;
#if LinkedListDebug
				DebugId = ++DebugIdGen;
#endif
      }

#if LinkedListDebug

			public void print_debug()
			{
				Console.WriteLine("Id: {0}, Lcp={1}, Idx={2}, Size={3}, BegId={4}, EndId={5}", DebugId, Lcp, Idx, IntervalSize, IntervalBegin.DebugId, IntervalEnd.DebugId);
			}
#endif
    }

    /// <summary>
    /// Maintains a list of linked <see cref="LLElement"/> instances.
    /// </summary>
    protected class LinkedList
    {
      private LLElement _first;
      private LLElement _last;
      private int _count;

      public void AddLast(LLElement node)
      {
        if (null == _last)
        {
          _last = node;
          _first = node;
          node.Next = null;
          node.Previous = null;
        }
        else
        {
          node.Next = null;
          node.Previous = _last;
          _last.Next = node;
          _last = node;
        }
        ++_count;
      }

      public void Remove(LLElement node)
      {
        var prev = node.Previous;
        var next = node.Next;

        if (null != prev)
          prev.Next = next;
        else
          _first = next;

        if (null != next)
          next.Previous = prev;
        else
          _last = prev;

        --_count;
      }

      public LLElement First
      {
        get
        {
          return _first;
        }
      }

      public LLElement Last
      {
        get
        {
          return _last;
        }
      }

      public int Count
      {
        get
        {
          return _count;
        }
      }
    }

    #endregion internal types

    // intermediate data neccessary for the algorithm
    protected LinkedList _ddlList;

    protected LLElement[] _lastLcp;

    /// <summary>Initializes a new instance of the problem solver for the longest common substring problem.</summary>
    /// <param name="gsa">Generalized suffix array. It is neccessary that this was constructed with individual words.</param>
    public LongestCommonSubstringBaseL(GeneralizedSuffixArray gsa)
      : base(gsa)
    {
    }

#if LinkedListDebug
		protected virtual void print_debug()
		{
			var e = _ddlList.Last;

			while (null != e)
			{
				e.print_debug();
				e = (LLElement)e.Previous;
			}
		}
#endif
  }
}
