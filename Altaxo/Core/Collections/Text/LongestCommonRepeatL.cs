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
  /// Evaluates the longest string, that is i) common to a number of words and ii) is repeated a certain number of times in those strings.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This is close to the original implementation, using a linked list of class instances. Thus, a
  /// lot (repeats * textlength) class instances will be created as intermediates. The alternative implementation
  /// <see cref="LongestCommonRepeatA"/> is avoiding this by using an array of struct's instead.
  /// </para>
  /// <para>
  /// For details of the algorithm see the very nice paper by Michael Arnold and Enno Ohlebusch, 'Linear Time Algorithms for Generalizations of the Longest Common Substring Problem', Algorithmica (2011) 60; 806-818; DOI: 10.1007/s00453-009-9369-1.
  /// This code was adopted by D.Lellinger from the C++ sources from the web site of the authors at http://www.uni-ulm.de/in/theo/research/sequana.html.
  /// </para>
  /// </remarks>
  public class LongestCommonRepeatL : LongestCommonSubstringBaseL
  {
    // intermediate data neccessary for the algorithm
    private LLElement[][]? _items;

    private int _x_repeats;
    private int[]? _last_index;
    private MinimumOnSlidingWindow[]? _pqls;

    /// <summary>Initializes a new instance of the problem solver for the repeated longest common substring problem.</summary>
    /// <param name="gsa">Generalized suffix array. It is neccessary that this was constructed with individual words.</param>
    public LongestCommonRepeatL(GeneralizedSuffixArray gsa)
      : base(gsa)
    {
    }

    /// <summary>Initializes a new instance of the problem solver for the repeated longest common substring problem.</summary>
    /// <param name="gsa">Generalized suffix array. It is neccessary that this was constructed with individual words.</param>
    /// <param name="x_repeats">Number of repeats in each of the words.</param>
    public LongestCommonRepeatL(GeneralizedSuffixArray gsa, int x_repeats)
      : base(gsa)
    {
      _x_repeats = x_repeats;
    }

    /// <summary>Evaluates the repeated longest common substring. After evaluation, the results can be accessed by the properties of this instance. Please be aware that the amount of resulting information depends on
    /// the state of <see cref="P:LongestCommonSubstringBase.StoreVerboseResults"/>.
    /// </summary>
    /// <returns>This instance.</returns>
    public LongestCommonRepeatL Evaluate()
    {
      if (_x_repeats <= 1)
        throw new InvalidOperationException("_x_repeats<1, did you forget to initialize _x_repeats?");

      return Evaluate(_x_repeats);
    }

    /// <summary>Evaluates the repeated longest common substring. After evaluation, the results can be accessed by the properties of this instance. Please be aware that the amount of resulting information depends on
    /// the state of <see cref="P:StoreVerboseResults"/>.
    /// </summary>
    /// <returns>This instance.</returns>
    public LongestCommonRepeatL Evaluate(int x_repeats)
    {
      if (x_repeats < 1)
        throw new ArgumentOutOfRangeException("x_repeats<1, but has to be >=1");

      _x_repeats = x_repeats;

      InitializeResults();

      InitializeIntermediates();

#if LinkedListDebug
			Console.WriteLine("Vor erstem Update:");
			print_debug();
			//Test();
#endif

      for (int i = 0; i < _suffixArray.Length; ++i)
      {
        lcp_update(_LCP[i], i);

#if LinkedListDebug
				Console.WriteLine("Nach lcp_update, i={0}, lcp[{0}]={1}", i, _LCP[i]);
				print_debug();
				//Test();
#endif

        list_update(i);

#if LinkedListDebug
				Console.WriteLine("Nach list_update, i={0}, lcp[{0}]={1}", i, _LCP[i]);
				print_debug();
				//Test();
#endif
      }
      lcp_update(0, _suffixArray.Length);

#if LinkedListDebug
			Console.WriteLine("Nach letztem lcp_update");
			print_debug();
			//Test();
#endif

      EvaluateMaximumNumberOfWordsWithCommonSubstring();
      CleanIntermediates();
      return this;
    }

    /// <summary>Initializes all intermediates arrays and objects.</summary>
    private void InitializeIntermediates()
    {
      // initialize items
      _items = new LLElement[_numberOfWords][];
      _ddlList = new LinkedList();
      for (int i = 0; i < _items.Length; ++i)
      {
        _items[i] = new LLElement[_x_repeats];
        for (int j = 0; j < _x_repeats; ++j)
        {
          LLElement ele;
          _items[i][j] = ele = new LLElement();
          ele.IntervalSize = 0;
          _ddlList.AddLast(ele);
        }
        _items[i][0].IntervalSize = 1;
      }

      // initialize intermediates
      _last_index = new int[_numberOfWords];
      _pqls = new MinimumOnSlidingWindow[_numberOfWords];
      for (int i = 0; i < _numberOfWords; ++i)
      {
        _pqls[i].Initialize(_x_repeats, 0);
      }

      _lastLcp = new LLElement[_maximumLcp + 1];

      var begin = _ddlList.Last!.Previous!; // front.prev->prev;
      LLElement end = _ddlList.First!; // originally back.next
      begin.IntervalEnd = end;
      end.IntervalBegin = begin;
      if (_x_repeats > 1)
      {
        end.IntervalSize = _numberOfWords;
      }
      else
      {
        end.IntervalSize = _numberOfWords - 1;
      }
      _ddlList.Last.IntervalEnd = _ddlList.First!;
      _ddlList.First!.IntervalBegin = _ddlList.Last;
      _ddlList.First!.IntervalSize = _numberOfWords;
      _lastLcp[0] = _ddlList.First;
    }

    /// <summary>Cleans the intermediates so the garbage collector can get them.</summary>
    private void CleanIntermediates()
    {
      _ddlList = null;
      _items = null;
      _lastLcp = null;
      _pqls = null;
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

#nullable disable // disable for private functions

    private void create_interval(LLElement end, LLElement begin, int lcp, int size)
    {
      begin.IntervalBegin = begin;
      begin.IntervalEnd = end;

      end.Lcp = lcp;
      end.IntervalBegin = begin;
      end.IntervalEnd = end;
      end.IntervalSize = size;
    }

    /// <summary>To understand the principles of this algorithm see the paper by Michael Arnold and Enno Ohlebusch given in the remarks of the class description (<see cref="LongestCommonRepeatL"/>).</summary>
    /// <param name="i">The i.</param>
    private void list_update(int i)
    {
      var sa_i = _suffixArray[i];
      var textlcp = _LCPS[i];

      int wordIdx = _wordIndices[i];
      var text_item = _items[wordIdx][_last_index[wordIdx]];
      _last_index[wordIdx] = (_last_index[wordIdx] + 1) % _x_repeats;

      _pqls[wordIdx].Add(textlcp);
      int former_textlcp = _pqls[wordIdx].MinimumValue;
      _pqls[wordIdx].Remove();
      textlcp = _pqls[wordIdx].MinimumValue; // GetMin

      _lastLcp[textlcp].IntervalSize++;

      if (_lastLcp[former_textlcp] != text_item || text_item.IntervalBegin != text_item)
      {
        // decrease interval size
        _lastLcp[former_textlcp].IntervalSize--;

        // if text_item is the end of an interval
        if (text_item == _lastLcp[former_textlcp])
        {
          create_interval(text_item.Next, text_item.IntervalBegin, text_item.Lcp, text_item.IntervalSize);

          if (_lastLcp[text_item.Lcp] == text_item)
          {
            _lastLcp[text_item.Lcp] = text_item.Next;
          }
        }

        // if text_item is the beginning of an interval
        else if (text_item == _lastLcp[former_textlcp].IntervalBegin)
        {
          create_interval(text_item.IntervalEnd, text_item.Previous, text_item.IntervalEnd.Lcp, text_item.IntervalEnd.IntervalSize);
        }

        // reset interval pointers
        text_item.IntervalEnd = text_item;
        text_item.IntervalBegin = text_item;
      }
      else
      {
        _lastLcp[text_item.Lcp] = null;
      }

      if (_x_repeats == 1)
      {
        text_item.IntervalSize = 1;
      }
      else
      {
        text_item.IntervalSize = 0;
      }
      if (_ddlList.Last != text_item)
      {
        // remove nodeToRemove from the list, and add it to the end
        _ddlList.Remove(text_item);
        _ddlList.AddLast(text_item);
        // update lcp value
        text_item.Previous.Lcp = _LCP[i];
      }

      text_item.Idx = i;
    }

    /// <summary>To understand the principles of this algorithm see the paper by Michael Arnold and Enno Ohlebusch given in the remarks of the class description (<see cref="LongestCommonRepeatL"/>).</summary>
    /// <param name="lcp_i">The lcp_i.</param>
    /// <param name="index">The index.</param>
    private void lcp_update(int lcp_i, int index)
    {
      var current = _ddlList.Last;
      var last_updated = current;
      current = current.Previous;
      int list_pos = 0;

      while (current is not null && current.IntervalEnd.Lcp >= lcp_i)
      {
        current = current.IntervalEnd;
        last_updated = current;
        list_pos += current.IntervalSize;

        // Storing the results
        if (_useVerboseResults)
        {
          if (current.Lcp >= _lcsOfNumberOfWords[list_pos])
          {
            bool lcslIsReallyGreaterThanBefore = current.Lcp > _lcsOfNumberOfWords[list_pos];
            _lcsOfNumberOfWords[list_pos] = current.Lcp;
            StoreVerboseResult(list_pos, current.Idx, index - 1, lcslIsReallyGreaterThanBefore);
          }
        }
        else
        {
          if (current.Lcp > _lcsOfNumberOfWords[list_pos])
          {
            _lcsOfNumberOfWords[list_pos] = current.Lcp;
            _singleResultOfNumberOfWords[list_pos] = new SuffixArrayRegion(current.Idx, index - 1);
          }
        } // end storing the results

        if (_lastLcp[last_updated.Lcp] == last_updated)
        {
          _lastLcp[last_updated.Lcp] = null;
        }
        current = current.Previous;
      } // end while

      _ddlList.Last.IntervalEnd = last_updated;
      last_updated.IntervalBegin = _ddlList.Last;
      last_updated.IntervalSize = list_pos;
      if (_lastLcp[last_updated.Lcp] == last_updated)
      {
        _lastLcp[last_updated.Lcp] = null;
      }
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

#nullable enable


#if LinkedListDebug
		protected override void print_debug()
		{
			base.print_debug();
			Console.WriteLine("State of the priority queues:");
			for (int i = 0; i < _numberOfWords; ++i)
				Console.Write("{0} ", _pqls[i].MinimumValue);
			Console.WriteLine();
		}
#endif
  }
}
