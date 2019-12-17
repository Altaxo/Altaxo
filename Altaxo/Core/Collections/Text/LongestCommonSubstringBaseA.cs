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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Collections.Text
{
  /// <summary>
  /// Base class for problem solvers for longest common substring problems using an array of linked structures stored in a linear array instead of linked class instances. This should it make easier for the garbage collector.
  /// </summary>
  /// <remarks>
  /// For details of the algorithm see the very nice paper by Michael Arnold and Enno Ohlebusch, 'Linear Time Algorithms for Generalizations of the Longest Common Substring Problem', Algorithmica (2011) 60; 806-818; DOI: 10.1007/s00453-009-9369-1.
  /// This code was adopted from the C++ sources from the web site of the authors at http://www.uni-ulm.de/in/theo/research/sequana.html.
  /// </remarks>
  public class LongestCommonSubstringBaseA : LongestCommonSubstringBase
  {
    #region internal types

    /// <summary>
    /// Element of the linked list array.
    /// </summary>
    protected struct LLElement
    {
      /// <summary>First occurence in the suffix array.</summary>
      public int Idx;

      /// <summary>Longest common prefix.</summary>
      public int Lcp;

      /// <summary>Index of the next list element in the array, or -1 if no such element exists.</summary>
      public int Next;

      /// <summary>Index of the previous list element in the array, or -1 if no such element exists.</summary>
      public int Previous;

      /// <summary>Index of the first list element of the interval to which this list element belongs.</summary>
      public int IntervalBegin;

      /// <summary>Index of the last list element of the interval to which this list element belongs.</summary>
      public int IntervalEnd;

      /// <summary>Length of the interval (number of nodes) that belong to the interval to which this list element belongs.</summary>
      public int IntervalSize;
    }

    /// <summary>
    /// Maintains a list of linked <see cref="LLElement"/> structures.
    /// </summary>
    protected struct LinkedList
    {
      /// <summary>Index of the first element of the linked structures.</summary>
      public int First;

      /// <summary>Index of the last element of the linked structures.</summary>
      public int Last;

      /// <summary>List of linked structures.</summary>
      public LLElement[] L;

      /// <summary>Moves the element at index i to the last position in the linked list of elements. Only the links (<see cref="LLElement.Next"/> and <see cref="LLElement.Previous"/>) of the node change, the structure itself is not moved inside the array.</summary>
      /// <param name="node">The index of the node.</param>
      public void MoveToLast(int node)
      {
        if (Last != node)
        {
          int prev = L[node].Previous;
          int next = L[node].Next;

          if (prev >= 0)
            L[prev].Next = next;
          else
            First = next;

          if (next >= 0)
            L[next].Previous = prev;
          else
            Last = prev;

          L[node].Next = -1;
          L[node].Previous = Last;
          L[Last].Next = node;
          Last = node;
        }
      }

      /// <summary>Inits the linked list of structures by allocating an array, and filling this array with the structures linked in ascending order.</summary>
      /// <param name="count">Number of structures.</param>
      public void Init(int count)
      {
        L = new LLElement[count];
        for (int i = 0; i < count; ++i)
        {
          L[i] = new LLElement() { Next = i + 1, Previous = i - 1, IntervalBegin = i, IntervalEnd = i, IntervalSize = 1, Idx = 0, Lcp = 0 };
        }
        L[0].Previous = -1;
        L[count - 1].Next = -1;
        First = 0;
        Last = count - 1;
      }

      /// <summary>Clears this instance (i.e. frees the array).</summary>
      public void Clear()
      {
        L = null;
        First = Last = 0;
      }
    }

    #endregion internal types

    // intermediate data neccessary for the algorithm

    /// <summary>Keeps a linked list of <see cref="LLElement"/>s.</summary>
    protected LinkedList _ddlList;

    /// <summary></summary>
    protected int[] _lastLcp;

    /// <summary>Initializes a new instance of the problem solver for the longest common substring problem.</summary>
    /// <param name="gsa">Generalized suffix array. It is neccessary that this was constructed with individual words.</param>
    public LongestCommonSubstringBaseA(GeneralizedSuffixArray gsa)
      : base(gsa)
    {
    }

    /// <summary>Prints all linked list items for debugging purposes.</summary>
    protected virtual void print_debug()
    {
      var L = _ddlList.L;
      var eIdx = _ddlList.Last;
      var eEle = L[eIdx];

      while (eIdx >= 0)
      {
        eEle = L[eIdx];
        Console.WriteLine("Id: {0}, Lcp={1}, Idx={2}, Size={3}, BegId={4}, EndId={5}", eIdx, eEle.Lcp, eEle.Idx, eEle.IntervalSize, eEle.IntervalBegin, eEle.IntervalEnd);
        eIdx = eEle.Previous;
      }
    }
  }
}
