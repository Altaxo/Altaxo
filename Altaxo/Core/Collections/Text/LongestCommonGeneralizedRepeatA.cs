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
  /// Evaluates the longest string that is (i) common to a number of words and (ii) is repeated a certain number of times in those strings.
  /// The number of repeats the string should occur in each word is given by an array of integers.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This implementation uses an array of structs, thus avoiding the creation of a lot of class instances.
  /// If you want to compare the code with the literature, see <see cref="LongestCommonGeneralizedRepeatL"/> instead.
  /// </para>
  /// <para>
  /// For details of the algorithm, see the paper by Michael Arnold and Enno Ohlebusch, "Linear Time Algorithms for Generalizations of the Longest Common Substring Problem", Algorithmica (2011) 60; 806-818; DOI: 10.1007/s00453-009-9369-1.
  /// This code was adapted by D. Lellinger from the C++ sources from the authors' website at http://www.uni-ulm.de/in/theo/research/sequana.html.
  /// </para>
  /// </remarks>
  public class LongestCommonGeneralizedRepeatA : LongestCommonSubstringBase
  {
    #region internal types

    /// <summary>
    /// Element of the linked list array.
    /// </summary>
    protected struct DDLElement
    {
      /// <summary>First occurrence in the suffix array.</summary>
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
      /// <summary>Number of the word this element belongs to.</summary>
      public int Text;
      /// <summary>Indicates whether this element is clean.</summary>
      public bool Clean;
#if LinkedListDebug
      public int DebugId;
      static int DebugIdGen = -1;
#endif
      /// <summary>
      /// Initializes this element with the specified index and text.
      /// </summary>
      /// <param name="idx">The index of the element.</param>
      /// <param name="text">The word index this element belongs to.</param>
      public void Init(int idx, int text)
      {
        Next = idx + 1;
        Previous = idx - 1;
        Text = text;
        Clean = true;
        IntervalBegin = idx;
        IntervalEnd = idx;
        IntervalSize = 0;
#if LinkedListDebug
        DebugId = ++DebugIdGen;
#endif
      }
    }

    /// <summary>
    /// Maintains a list of linked <see cref="DDLElement"/> structures.
    /// </summary>
    protected struct LinkedObjectList
    {
      /// <summary>Index of the first element of the linked structures.</summary>
      public int First;
      /// <summary>Index of the last element of the linked structures.</summary>
      public int Last;
      /// <summary>List of linked structures.</summary>
      public DDLElement[] L;
      /// <summary>
      /// Moves the element at index <paramref name="node"/> to the last position in the linked list of elements. Only the links (<see cref="DDLElement.Next"/> and <see cref="DDLElement.Previous"/>) of the node change; the structure itself is not moved inside the array.
      /// </summary>
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

      /// <summary>
      /// Initializes the linked list of structures by allocating an array, and filling this array with the structures linked in ascending order.
      /// </summary>
      /// <param name="count">Number of structures.</param>
      public void Init(int count)
      {
        L = new DDLElement[count];
        First = 0;
        Last = count - 1;
      }
      /// <summary>
      /// Clears the linked list and releases its resources.
      /// </summary>
      public void Clear()
      {
        L = new DDLElement[0];
        First = Last = 0;
      }
    }

    /// <summary>
    /// Stores a preliminary result for a substring occurrence.
    /// </summary>
    protected struct PreResult
    {
      /// <summary>Start index of the suffix in the suffix array.</summary>
      public int Begin;
      /// <summary>End index of the suffix in the suffix array.</summary>
      public int End;
      /// <summary>Number of words that have this common substring.</summary>
      public int WordIdx;
      /// <summary>Longest common substring in the suffix array in the range [Begin, End].</summary>
      public int Lcs;
    }

    #endregion internal types

    // intermediate data neccessary for the algorithm
    protected LinkedObjectList _ddlList;

    protected int[]? _lastLcp;

    // intermediate data neccessary for the algorithm
    private int[][]? _items;

    private int[] _x_repeats;
    private int[]? list_sizes;
    private int[]? _last_index;

    /// <summary>For a given number of words as the index, this array stores some preliminarily found suffix regions for some algorithms (e.g. generalized common repeat algorithm).</summary>
    protected List<PreResult>? _preResults;

    private MinimumOnSlidingWindow[]? _pqls;

    /// <summary>Initializes a new instance of the problem solver for the repeated longest common substring problem.</summary>
    /// <param name="gsa">Generalized suffix array. It is neccessary that this was constructed with individual words.</param>
    public LongestCommonGeneralizedRepeatA(GeneralizedSuffixArray gsa)
      : base(gsa)
    {
      _x_repeats = new int[_numberOfWords];
      for (int i = 0; i < _x_repeats.Length; ++i) _x_repeats[i] = 1;
    }

    /// <summary>Initializes a new instance of the problem solver for the repeated longest common substring problem.</summary>
    /// <param name="gsa">Generalized suffix array. It is neccessary that this was constructed with individual words.</param>
    /// <param name="x_repeats">Array (length=number of words) with the number of repeats that should occur in each word.</param>
    public LongestCommonGeneralizedRepeatA(GeneralizedSuffixArray gsa, int[] x_repeats)
      : base(gsa)
    {
      if (x_repeats is null)
        throw new ArgumentNullException(nameof(x_repeats));
      if (x_repeats.Length < _numberOfWords)
        throw new ArgumentException("Length of array not sufficient", nameof(x_repeats));

      _x_repeats = x_repeats;
    }

    /// <summary>Evaluates the repeated longest common substring. After evaluation, the results can be accessed by the properties of this instance. Please be aware that the amount of resulting information depends on
    /// the state of <see cref="P:StoreVerboseResults"/>.
    /// </summary>
    /// <returns>This instance.</returns>
    public LongestCommonGeneralizedRepeatA Evaluate()
    {
      return Evaluate(_x_repeats);
    }

    /// <summary>Evaluates the repeated longest common substring. After evaluation, the results can be accessed by the properties of this instance. Please be aware that the amount of resulting information depends on
    /// the state of <see cref="P:StoreVerboseResults"/>.
    /// </summary>
    /// <returns>This instance.</returns>
    public LongestCommonGeneralizedRepeatA Evaluate(int[] x_repeats)
    {
      _x_repeats = x_repeats;

      InitializeResults();

      InitializeIntermediates();

#if LinkedListDebug
			Console.WriteLine("Vor erstem Update:");
			print_debug();
			Test();
#endif

      bool clean = true;
      for (int i = 0; i < _suffixArray.Length; ++i)
      {
        lcp_update(_LCP[i], i, clean);

        if (_x_repeats[_wordIndices[i]] == 0)
        {
          FlushResults(_LCP[i]);
        }

#if LinkedListDebug
				Console.WriteLine("Nach lcp_update, i={0}, lcp[{0}]={1}", i, _LCP[i]);
				print_debug();
				Test();
#endif

        list_update(i);

        clean = _x_repeats[_wordIndices[i]] > 0;

#if LinkedListDebug
				Console.WriteLine("Nach list_update, i={0}, lcp[{0}]={1}", i, _LCP[i]);
				print_debug();
				Test();
#endif
      }
      lcp_update(0, _suffixArray.Length - 1, clean);
      FlushResults(0);

#if LinkedListDebug
			Console.WriteLine("Nach letztem lcp_update");
			print_debug();
			Test();
#endif

      EvaluateMaximumNumberOfWordsWithCommonSubstring();
      CleanIntermediates();
      return this;
    }

#nullable disable
    private void InitializeIntermediates()
    {
      // initialize items
      list_sizes = new int[_numberOfWords];
      int totalItems = 0;
      for (int i = 0; i < list_sizes.Length; ++i)
      {
        list_sizes[i] = (_x_repeats[i] > 0) ? _x_repeats[i] : 1;
        totalItems += list_sizes[i];
      }

      _items = new int[_numberOfWords][];
      _preResults = new List<PreResult>();
      _ddlList = new LinkedObjectList();
      _ddlList.Init(totalItems);
      var L = _ddlList.L;
      int ddlListPtr = 0;
      for (int i = 0; i < _items.Length; ++i)
      {
        int list_size_i = list_sizes[i];
        _items[i] = new int[list_size_i];
        for (int j = 0; j < list_size_i; ++j)
        {
          L[ddlListPtr].Init(ddlListPtr, i);
          _items[i][j] = ddlListPtr;
          ++ddlListPtr;
        }
        if (_x_repeats[i] > 0)
          L[_items[i][0]].IntervalSize = 1;
      }

      // initialize intermediates
      _last_index = new int[_numberOfWords];
      _pqls = new MinimumOnSlidingWindow[_numberOfWords];
      for (int i = 0; i < _numberOfWords; ++i)
      {
        _pqls[i].Initialize(_x_repeats[i], 0);
        //_pqls[i].add_value(0);
      }

      _lastLcp = new int[_maximumLcp + 1];

      var begin = L[_ddlList.Last].Previous; // front.prev->prev;
      var end = _ddlList.First; // originally back.next
      L[begin].IntervalEnd = end;
      L[end].IntervalBegin = begin;
      if (_x_repeats[L[_ddlList.Last].Text] > 1)
      {
        L[end].IntervalSize = _numberOfWords;
      }
      else
      {
        L[end].IntervalSize = _numberOfWords - 1;
      }
      for (int i = 0; i < _numberOfWords - 1; ++i)
      {
        if (_x_repeats[i] == 0)
        {
          L[end].IntervalSize = -1;
          L[end].Clean = false;
        }
      }
      _lastLcp[0] = end;
    }

    /// <summary>Cleans the intermediates so the garbage collector can get them.</summary>
    private void CleanIntermediates()
    {
      _ddlList.Clear();
      _items = null;
      _lastLcp = null;
      _pqls = null;
      _preResults = null;
    }

    private void InitializeResults()
    {
      // initialize results
      _lcsOfNumberOfWords = new int[_numberOfWords + 1];
      _verboseResultsOfNumberOfWords = null;
      _singleResultOfNumberOfWords = null;
      if (_useVerboseResults)
        _verboseResultsOfNumberOfWords = new List<SuffixArrayRegion>[_numberOfWords + 1];
      else
        _singleResultOfNumberOfWords = new SuffixArrayRegion[_numberOfWords + 1];
    }

    private void create_interval(int end, int begin, int lcp, int size, bool clean)
    {
      var L = _ddlList.L;
      L[begin].IntervalBegin = begin;
      L[begin].IntervalEnd = end;

      L[end].Lcp = lcp;
      L[end].IntervalBegin = begin;
      L[end].IntervalEnd = end;
      L[end].IntervalSize = size;
      L[end].Clean = clean;
    }

    /// <summary>To understand the principles of this algorithm see the paper by Michael Arnold and Enno Ohlebusch given in the remarks of the class description (<see cref="LongestCommonGeneralizedRepeatA"/>).</summary>
    /// <param name="i">The i.</param>
    private void list_update(int i)
    {
      var L = _ddlList.L;
      var sa_i = _suffixArray[i];
      var textlcp = _LCPS[i];

      int wordIdx = _wordIndices[i];
      var text_item = _items[wordIdx][_last_index[wordIdx]];
      _last_index[wordIdx] = (_last_index[wordIdx] + 1) % list_sizes[wordIdx];

      _pqls[wordIdx].Add(textlcp);
      int former_textlcp = _pqls[wordIdx].MinimumValue;
      _pqls[wordIdx].Remove();
      textlcp = _pqls[wordIdx].MinimumValue;

      if (_x_repeats[wordIdx] > 1 && L[_lastLcp[textlcp]].IntervalSize >= 0)
        L[_lastLcp[textlcp]].IntervalSize++;

      if (_lastLcp[former_textlcp] != text_item || L[text_item].IntervalBegin != text_item)
      {
        // decrease interval size
        if (_x_repeats[wordIdx] != 0 && L[_lastLcp[textlcp]].IntervalSize >= 0)
          L[_lastLcp[former_textlcp]].IntervalSize--;

        // if text_item is the end of an interval
        if (text_item == _lastLcp[former_textlcp])
        {
          create_interval(L[text_item].Next, L[text_item].IntervalBegin, L[text_item].Lcp, L[text_item].IntervalSize, L[text_item].Clean);

          if (_lastLcp[L[text_item].Lcp] == text_item)
          {
            _lastLcp[L[text_item].Lcp] = L[text_item].Next;
          }
        }

        // if text_item is the beginning of an interval
        else if (text_item == L[_lastLcp[former_textlcp]].IntervalBegin)
        {
          create_interval(L[text_item].IntervalEnd, L[text_item].Previous, L[L[text_item].IntervalEnd].Lcp, L[L[text_item].IntervalEnd].IntervalSize, L[L[text_item].IntervalEnd].Clean);
        }

        // reset interval pointers
        L[text_item].IntervalEnd = text_item;
        L[text_item].IntervalBegin = text_item;
      }

      if (_x_repeats[wordIdx] == 1)
        L[text_item].IntervalSize = 1;
      else
        L[text_item].IntervalSize = 0;

      if (_ddlList.Last != text_item)
      {
        // remove nodeToRemove from the list, and add it to the end
        _ddlList.MoveToLast(text_item);
        // update lcp value
        L[L[text_item].Previous].Lcp = _LCP[i];
      }
      if (_x_repeats[wordIdx] > 0)
        L[text_item].Clean = true;

      L[text_item].Idx = i;
    }

    /// <summary>To understand the principles of this algorithm see the paper by Michael Arnold and Enno Ohlebusch given in the remarks of the class description (<see cref="LongestCommonGeneralizedRepeatA"/>).</summary>
    /// <param name="lcp_i">The lcp_i.</param>
    /// <param name="index">The index.</param>
    /// <param name="clean"></param>
    private void lcp_update(int lcp_i, int index, bool clean)
    {
      var L = _ddlList.L;
      var current = _ddlList.Last;
      var last_updated = current;
      clean &= L[current].Clean;
      int list_pos = 0;
      if (_x_repeats[L[current].Text] == 1)
        list_pos = 1;

      current = L[current].Previous;

      while (current >= 0 && L[L[current].IntervalEnd].Lcp >= lcp_i)
      {
        current = L[current].IntervalEnd;
        last_updated = current;
        clean &= L[current].Clean;
        list_pos += L[current].IntervalSize;

        // Storing the results
        if (clean)
        {
          if (L[current].Lcp >= _lcsOfNumberOfWords[list_pos])
          {
            StorePreResult(list_pos, L[current].Idx, index - 1, L[current].Lcp);
          }
        } // end storing the results

        current = L[current].Previous;
      } // end while

      L[_ddlList.Last].IntervalEnd = last_updated;
      L[last_updated].IntervalBegin = _ddlList.Last;
      L[last_updated].IntervalSize = list_pos;
      L[last_updated].Clean = clean;
      L[last_updated].Lcp = lcp_i;
      _lastLcp[lcp_i] = last_updated;
    }

    /// <summary>Posts the process results. Here the maximum number of words that have at least one common substring is evaluated.</summary>
    protected void EvaluateMaximumNumberOfWordsWithCommonSubstring()
    {
      _maximumNumberOfWordsWithCommonSubstring = 0;
      for (int i = _lcsOfNumberOfWords.Length - 1; i > 1; --i)
      {
        if (_lcsOfNumberOfWords[i] != 0)
        {
          _maximumNumberOfWordsWithCommonSubstring = i;
          break;
        }
      }
    }

    /// <summary>Stores a common substring occurence.</summary>
    /// <param name="list_pos">Number of words that have this common substring.</param>
    /// <param name="beg">Start index of the suffix in the suffix array.</param>
    /// <param name="end">End index of the suffix in the suffix array.</param>
    /// <param name="lcs">Longest common substring in the suffix array in the range [beg, end].</param>
    private void StorePreResult(int list_pos, int beg, int end, int lcs)
    {
      _preResults.Add(new PreResult { Begin = beg, End = end, WordIdx = list_pos, Lcs = lcs });
    }

    private void FlushResults(int length)
    {
      for (int i = 0; i < _preResults.Count; ++i)
      {
        var res = _preResults[i];
        if (res.Lcs > length)
        {
          bool reallyGreater = res.Lcs > _lcsOfNumberOfWords[res.WordIdx];
          if (res.Lcs >= _lcsOfNumberOfWords[res.WordIdx])
            StoreVerboseResult(res.WordIdx, res.Begin, res.End, reallyGreater);
          if (reallyGreater)
            _lcsOfNumberOfWords[res.WordIdx] = res.Lcs;
        }
      }
      _preResults.Clear();
    }

#nullable enable

#if LinkedListDebug
		protected virtual void print_debug()
		{
			var L = _ddlList.L;
			var e = _ddlList.Last;

			while (e>=0)
			{
				Console.WriteLine("Id: {0}, Lcp={1}, Idx={2}, Size={3}, BegId={4}, EndId={5}", L[e].DebugId, L[e].Lcp, L[e].Idx, L[e].IntervalSize, L[L[e].IntervalBegin].DebugId, L[L[e].IntervalEnd].DebugId);
				e = L[e].Previous;
			}

			Console.WriteLine("State of the priority queues:");
			for (int i = 0; i < _numberOfWords; ++i)
				Console.Write("{0} ", _pqls[i].MinimumValue);
			Console.WriteLine();
		}

		private void Test()
		{
		}
#endif
  }
}
