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
  /// This is close to the original implementation, using a linked list of class instances. Thus, a
  /// lot (repeats * text length) of class instances will be created as intermediates. The implementation
  /// <see cref="LongestCommonGeneralizedRepeatA"/> avoids this by using an array of structs instead.
  /// </para>
  /// <para>
  /// For details of the algorithm, see the paper by Michael Arnold and Enno Ohlebusch, "Linear Time Algorithms for Generalizations of the Longest Common Substring Problem", Algorithmica (2011) 60; 806-818; DOI: 10.1007/s00453-009-9369-1.
  /// This code was adapted by D. Lellinger from the C++ sources from the authors' website at http://www.uni-ulm.de/in/theo/research/sequana.html.
  /// </para>
  /// </remarks>
  public class LongestCommonGeneralizedRepeatL : LongestCommonSubstringBase
  {
    #region internal types

    /// <summary>
    /// Element of the linked list array.
    /// </summary>
    protected class DDLElement
    {
      /// <summary>First occurrence in the suffix array.</summary>
      public int Idx;
      /// <summary>Longest common prefix.</summary>
      public int Lcp;
      /// <summary>Next list element in the array, or null if no such element exists.</summary>
      public DDLElement? Next;
      /// <summary>Previous list element in the array, or null if no such element exists.</summary>
      public DDLElement? Previous;
      /// <summary>First list element of the interval to which this list element belongs.</summary>
      public DDLElement IntervalBegin;
      /// <summary>Last list element of the interval to which this list element belongs.</summary>
      public DDLElement IntervalEnd;
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
      /// Initializes a new instance of the <see cref="DDLElement"/> class.
      /// </summary>
      public DDLElement()
      {
        IntervalBegin = this;
        IntervalEnd = this;
        IntervalSize = 1;
        Clean = true;
#if LinkedListDebug
        DebugId = ++DebugIdGen;
#endif
      }
      /// <summary>
      /// Initializes a new instance of the <see cref="DDLElement"/> class with the specified word index.
      /// </summary>
      /// <param name="text">The word index this element belongs to.</param>
      public DDLElement(int text)
      {
        Text = text;
        Clean = true;
        IntervalBegin = this;
        IntervalEnd = this;
        IntervalSize = 1;
#if LinkedListDebug
        DebugId = ++DebugIdGen;
#endif
      }
#if LinkedListDebug
      /// <summary>
      /// Prints debug information for this element.
      /// </summary>
      public void print_debug()
      {
        Console.WriteLine("Id: {0}, Lcp={1}, Idx={2}, Size={3}, BegId={4}, EndId={5}", DebugId, Lcp, Idx, IntervalSize, IntervalBegin.DebugId, IntervalEnd.DebugId);
      }
#endif
    }

    /// <summary>
    /// Maintains a list of linked <see cref="DDLElement"/> instances.
    /// </summary>
    protected class LinkedObjectList
    {
      private DDLElement? _first;
      private DDLElement? _last;
      private int _count;
      /// <summary>
      /// Adds the specified node to the end of the linked list.
      /// </summary>
      /// <param name="node">The node to add.</param>
      public void AddLast(DDLElement node)
      {
        if (_last is null)
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
      /// <summary>
      /// Removes the specified node from the linked list.
      /// </summary>
      /// <param name="node">The node to remove.</param>
      public void Remove(DDLElement node)
      {
        var prev = node.Previous;
        var next = node.Next;

        if (prev is not null)
          prev.Next = next;
        else
          _first = next;

        if (next is not null)
          next.Previous = prev;
        else
          _last = prev;

        --_count;
      }
      /// <summary>
      /// Gets the first element of the linked list.
      /// </summary>
      public DDLElement? First
      {
        get { return _first; }
      }
      /// <summary>
      /// Gets the last element of the linked list.
      /// </summary>
      public DDLElement? Last
      {
        get { return _last; }
      }
      /// <summary>
      /// Gets the number of elements in the linked list.
      /// </summary>
      public int Count
      {
        get { return _count; }
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
    protected LinkedObjectList? _ddlList;

    protected DDLElement[]? _lastLcp;

    // intermediate data neccessary for the algorithm
    private DDLElement[][]? _items;

    private int[] _x_repeats;
    private int[]? list_sizes;
    private int[]? _last_index;

    /// <summary>For a given number of words as the index, this array stores some preliminarily found suffix regions for some algorithms (e.g. generalized common repeat algorithm).</summary>
    protected List<PreResult>? _preResults;

    private MinimumOnSlidingWindow[]? _pqls;

    /// <summary>Initializes a new instance of the problem solver for the repeated longest common substring problem.</summary>
    /// <param name="gsa">Generalized suffix array. It is neccessary that this was constructed with individual words.</param>
    public LongestCommonGeneralizedRepeatL(GeneralizedSuffixArray gsa)
      : base(gsa)
    {
      _x_repeats = new int[_numberOfWords];
      for (int i = 0; i < _x_repeats.Length; ++i) _x_repeats[i] = 1;
    }

    /// <summary>Initializes a new instance of the problem solver for the repeated longest common substring problem.</summary>
    /// <param name="gsa">Generalized suffix array. It is neccessary that this was constructed with individual words.</param>
    /// <param name="x_repeats">Array (length: number of words) with the number of repeats that should occur in each of the words.</param>
    public LongestCommonGeneralizedRepeatL(GeneralizedSuffixArray gsa, int[] x_repeats)
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
    public LongestCommonGeneralizedRepeatL Evaluate()
    {
      return Evaluate(_x_repeats);
    }

    /// <summary>Evaluates the repeated longest common substring. After evaluation, the results can be accessed by the properties of this instance. Please be aware that the amount of resulting information depends on
    /// the state of <see cref="P:StoreVerboseResults"/>.
    /// </summary>
    /// <returns>This instance.</returns>
    public LongestCommonGeneralizedRepeatL Evaluate(int[] x_repeats)
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
      for (int i = 0; i < list_sizes.Length; ++i)
      {
        list_sizes[i] = (_x_repeats[i] > 0) ? _x_repeats[i] : 1;
      }

      _preResults = new List<PreResult>();
      _ddlList = new LinkedObjectList();
      _items = new DDLElement[_numberOfWords][];
      for (int i = 0; i < _items.Length; ++i)
      {
        int list_size_i = list_sizes[i];
        _items[i] = new DDLElement[list_size_i];
        for (int j = 0; j < list_size_i; ++j)
        {
          DDLElement ele;
          _items[i][j] = ele = new DDLElement() { Text = i };
          ele.IntervalSize = 0;
          _ddlList.AddLast(ele);
        }
        if (_x_repeats[i] > 0)
          _items[i][0].IntervalSize = 1;
      }

      // initialize intermediates
      _last_index = new int[_numberOfWords];
      _pqls = new MinimumOnSlidingWindow[_numberOfWords];
      for (int i = 0; i < _numberOfWords; ++i)
      {
        _pqls[i].Initialize(_x_repeats[i], 0);
        //_pqls[i].add_value(0);
      }

      _lastLcp = new DDLElement[_maximumLcp + 1];

      var begin = _ddlList.Last.Previous; // front.prev->prev;
      DDLElement end = _ddlList.First; // originally back.next
      begin.IntervalEnd = end;
      end.IntervalBegin = begin;
      if (_x_repeats[_ddlList.Last.Text] > 1)
      {
        end.IntervalSize = _numberOfWords;
      }
      else
      {
        end.IntervalSize = _numberOfWords - 1;
      }
      for (int i = 0; i < _numberOfWords - 1; ++i)
      {
        if (_x_repeats[i] == 0)
        {
          end.IntervalSize = -1;
          end.Clean = false;
        }
      }
      _lastLcp[0] = end;
    }

    /// <summary>Cleans the intermediates so the garbage collector can get them.</summary>
    private void CleanIntermediates()
    {
      _ddlList = null;
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

    private void create_interval(DDLElement end, DDLElement begin, int lcp, int size, bool clean)
    {
      begin.IntervalBegin = begin;
      begin.IntervalEnd = end;

      end.Lcp = lcp;
      end.IntervalBegin = begin;
      end.IntervalEnd = end;
      end.IntervalSize = size;
      end.Clean = clean;
    }

    /// <summary>To understand the principles of this algorithm see the paper by Michael Arnold and Enno Ohlebusch given in the remarks of the class description (<see cref="LongestCommonGeneralizedRepeatL"/>).</summary>
    /// <param name="i">The i.</param>
    private void list_update(int i)
    {
      var sa_i = _suffixArray[i];
      var textlcp = _LCPS[i];

      int wordIdx = _wordIndices[i];
      var text_item = _items[wordIdx][_last_index[wordIdx]];
      _last_index[wordIdx] = (_last_index[wordIdx] + 1) % list_sizes[wordIdx];

      _pqls[wordIdx].Add(textlcp);
      int former_textlcp = _pqls[wordIdx].MinimumValue;
      _pqls[wordIdx].Remove();
      textlcp = _pqls[wordIdx].MinimumValue;

      if (_x_repeats[wordIdx] > 1 && _lastLcp[textlcp].IntervalSize >= 0)
        _lastLcp[textlcp].IntervalSize++;

      if (_lastLcp[former_textlcp] != text_item || text_item.IntervalBegin != text_item)
      {
        // decrease interval size
        if (_x_repeats[wordIdx] != 0 && _lastLcp[textlcp].IntervalSize >= 0)
          _lastLcp[former_textlcp].IntervalSize--;

        // if text_item is the end of an interval
        if (text_item == _lastLcp[former_textlcp])
        {
          create_interval(text_item.Next, text_item.IntervalBegin, text_item.Lcp, text_item.IntervalSize, text_item.Clean);

          if (_lastLcp[text_item.Lcp] == text_item)
          {
            _lastLcp[text_item.Lcp] = text_item.Next;
          }
        }

        // if text_item is the beginning of an interval
        else if (text_item == _lastLcp[former_textlcp].IntervalBegin)
        {
          create_interval(text_item.IntervalEnd, text_item.Previous, text_item.IntervalEnd.Lcp, text_item.IntervalEnd.IntervalSize, text_item.IntervalEnd.Clean);
        }

        // reset interval pointers
        text_item.IntervalEnd = text_item;
        text_item.IntervalBegin = text_item;
      }

      if (_x_repeats[wordIdx] == 1)
        text_item.IntervalSize = 1;
      else
        text_item.IntervalSize = 0;

      if (_ddlList.Last != text_item)
      {
        // remove nodeToRemove from the list, and add it to the end
        _ddlList.Remove(text_item);
        _ddlList.AddLast(text_item);
        // update lcp value
        text_item.Previous.Lcp = _LCP[i];
      }
      if (_x_repeats[wordIdx] > 0)
        text_item.Clean = true;

      text_item.Idx = i;
    }

    /// <summary>To understand the principles of this algorithm see the paper by Michael Arnold and Enno Ohlebusch given in the remarks of the class description (<see cref="LongestCommonGeneralizedRepeatL"/>).</summary>
    /// <param name="lcp_i">The lcp_i.</param>
    /// <param name="index">The index.</param>
    /// <param name="clean"></param>
    private void lcp_update(int lcp_i, int index, bool clean)
    {
      var current = _ddlList.Last;
      var last_updated = current;
      clean &= current.Clean;
      int list_pos = 0;
      if (_x_repeats[current.Text] == 1)
        list_pos = 1;

      current = current.Previous;

      while (current is not null && current.IntervalEnd.Lcp >= lcp_i)
      {
        current = current.IntervalEnd;
        last_updated = current;
        clean &= current.Clean;
        list_pos += current.IntervalSize;

        // Storing the results
        if (clean)
        {
          if (current.Lcp >= _lcsOfNumberOfWords[list_pos])
          {
            StorePreResult(list_pos, current.Idx, index - 1, current.Lcp);
          }
        } // end storing the results

        current = current.Previous;
      } // end while

      _ddlList.Last.IntervalEnd = last_updated;
      last_updated.IntervalBegin = _ddlList.Last;
      last_updated.IntervalSize = list_pos;
      last_updated.Clean = clean;
      last_updated.Lcp = lcp_i;
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
			var e = _ddlList.Last;

			while (null != e)
			{
				e.print_debug();
				e = (DDLElement)e.Previous;
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
