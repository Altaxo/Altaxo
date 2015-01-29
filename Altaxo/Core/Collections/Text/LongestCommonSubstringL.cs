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

//#define LinkedListDebug

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Collections.Text
{
	/// <summary>
	/// Problem solver for the longest common substring problems, operating in O(N) time (N being the text length), and using a doubly linked list of class instances. Thus a lot of intermediate objects is created. The
	/// code runs slightly slower than <see cref="LongestCommonSubstringA"/>, but is provided here because it is closer to the original code (see below) and easier to understand.
	/// </summary>
	/// <remarks>
	/// For details of the algorithm see the very nice paper by Michael Arnold and Enno Ohlebusch, 'Linear Time Algorithms for Generalizations of the Longest Common Substring Problem', Algorithmica (2011) 60; 806-818; DOI: 10.1007/s00453-009-9369-1.
	/// This code was by D.Lellinger adopted from the C++ sources from the web site of the authors at http://www.uni-ulm.de/in/theo/research/sequana.html.
	/// </remarks>
	internal class LongestCommonSubstringL : LongestCommonSubstringBaseL
	{
		// intermediate data neccessary for the algorithm

		private LLElement[] _textPtr;

		/// <summary>Initializes a new instance of the problem solver for the longest common substring problem.</summary>
		/// <param name="gsa">Generalized suffix array. It is neccessary that this was constructed with individual words.</param>
		public LongestCommonSubstringL(GeneralizedSuffixArray gsa)
			: base(gsa)
		{
		}

		/// <summary>Evaluates the longest common substring. After evaluation, the results can be accessed by the properties of this instance.</summary>
		/// <returns>This instance.</returns>
		public LongestCommonSubstringL Evaluate()
		{
			InitializeResults();

			InitializeIntermediates();

#if LinkedListDebug

			Console.WriteLine("Vor erstem Update:");
			print_debug();
			Test();
#endif

			for (int i = 0; i < _suffixArray.Length; ++i)
			{
				lcp_update(_LCP[i], i);

#if LinkedListDebug
				Console.WriteLine("Nach lcp_update, i={0}, lcp[{0}]={1}", i, _LCP[i]);
				print_debug();
				Test();
#endif

				list_update(i);

#if LinkedListDebug

				Console.WriteLine("Nach list_update, i={0}, lcp[{0}]={1}", i, _LCP[i]);
				print_debug();
				Test();
#endif
			}
			lcp_update(0, _suffixArray.Length);

#if LinkedListDebug
			Console.WriteLine("Nach letztem lcp_update");
			print_debug();
			Test();
#endif

			PostProcessResults();
			CleanIntermediates();
			return this;
		}

		/// <summary>Initialize all intermediate arrays and objects.</summary>
		private void InitializeIntermediates()
		{
			// initialize intermediates
			_lastLcp = new LLElement[_maximumLcp + 1];
			_ddlList = new LinkedList();
			_textPtr = new LLElement[_numberOfWords];
			for (int i = 0; i < _numberOfWords; ++i)
			{
				var ele = new LLElement();
				_ddlList.AddLast(ele);
				_textPtr[i] = ele;
			}
		}

		/// <summary>Cleans the intermediates so the garbage collector can get them.</summary>
		private void CleanIntermediates()
		{
			_ddlList = null;
			_textPtr = null;
			_lastLcp = null;
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

		/// <summary>Posts the process results. Here the maximum number of words that have at least one common substring is evaluated.</summary>
		private void PostProcessResults()
		{
			_maximumNumberOfWordsWithCommonSubstring = 0;
			for (int i = _lcsOfNumberOfWords.Length - 1; i >= 0; --i)
			{
				if (_lcsOfNumberOfWords[i] != 0)
				{
					_maximumNumberOfWordsWithCommonSubstring = i;
					break;
				}
			}
		}

		private void lcp_update(int lcp_i, int index)
		{
			var current = _ddlList.Last;
			var last_updated = current;
			current = current.Previous;
			int list_pos = 1;

			while (null != current && current.IntervalEnd.Lcp >= lcp_i)
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

				current = current.Previous;
			} // end while

			_ddlList.Last.IntervalEnd = last_updated;
			last_updated.IntervalBegin = _ddlList.Last;
			last_updated.IntervalSize = list_pos;
			last_updated.Lcp = lcp_i;
			_lastLcp[lcp_i] = last_updated;
		}

		private void create_interval(LLElement end, LLElement begin, int lcp, int size)
		{
			begin.IntervalBegin = begin;
			begin.IntervalEnd = end;

			end.Lcp = lcp;
			end.IntervalBegin = begin;
			end.IntervalEnd = end;
			end.IntervalSize = size;
		}

		private void list_update(int i)
		{
			var sa_i = _suffixArray[i];
			var textlcp = _LCPS[i];

			var nodeToRemove = _textPtr[_wordIndices[i]];

			if (_lastLcp[textlcp] != nodeToRemove || nodeToRemove.IntervalBegin != nodeToRemove)
			{
				// decrease interval size
				--_lastLcp[textlcp].IntervalSize;

				// if text_item is the end of an interval
				if (nodeToRemove == _lastLcp[textlcp])
				{
					create_interval(nodeToRemove.Next, nodeToRemove.IntervalBegin, nodeToRemove.Lcp, nodeToRemove.IntervalSize);
					_lastLcp[nodeToRemove.Lcp] = nodeToRemove.Next;
				}

			// if text_item is the beginning of an interval
				else if (nodeToRemove == _lastLcp[textlcp].IntervalBegin)
				{
					create_interval(nodeToRemove.IntervalEnd, nodeToRemove.Previous, nodeToRemove.IntervalEnd.Lcp, nodeToRemove.IntervalEnd.IntervalSize);
				}

				// reset interval pointers
				nodeToRemove.IntervalEnd = nodeToRemove;
				nodeToRemove.IntervalBegin = nodeToRemove;
				nodeToRemove.IntervalSize = 1;
			}

			if (_ddlList.Last != nodeToRemove)
			{
				// remove nodeToRemove from the list, and add it to the end
				_ddlList.Remove(nodeToRemove);
				_ddlList.AddLast(nodeToRemove);
				nodeToRemove.Previous.Lcp = _LCP[i];
			}

			nodeToRemove.Idx = i;
		}

#if LinkedListDebug

		void Test()
		{
			int totIntervalLen = 0;
			var e = _ddlList.Last;
			while (null != e)
			{
				if (e.IntervalSize <= 0 || e.IntervalSize > _ddlList.Count)
					throw new ArgumentOutOfRangeException();
				if (e.Idx < 0 || e.Idx >= _SA.Length)
					throw new ArgumentOutOfRangeException();
				if (!object.ReferenceEquals(e, e.IntervalBegin))
					throw new ArgumentOutOfRangeException();

				int size_from_last_node = e.IntervalSize;
				var last_interval_node = e;
				var first_interval_node = e.IntervalEnd;
				int currIntervalLen = 1;

				// test the intermediate nodes
				while (!object.ReferenceEquals(e, first_interval_node))
				{
					e = (DDLElement)e.Previous;
					currIntervalLen++;
					if (e.Idx < 0 || e.Idx >= _SA.Length)
						throw new ArgumentOutOfRangeException();
				}

				// Test the first interval node
				if (e.IntervalSize <= 0 || e.IntervalSize > _ddlList.Count)
					throw new ArgumentOutOfRangeException();
				if (e.Idx < 0 || e.Idx >= _SA.Length)
					throw new ArgumentOutOfRangeException();
				if (e.IntervalSize != currIntervalLen)
					throw new ArgumentOutOfRangeException();
				if (!object.ReferenceEquals(e.IntervalEnd, e))
					throw new ArgumentOutOfRangeException();
				if (!object.ReferenceEquals(e.IntervalBegin, last_interval_node))
					throw new ArgumentOutOfRangeException();
				if (!object.ReferenceEquals(last_interval_node.IntervalEnd, e))
					throw new ArgumentOutOfRangeException();
				if (!object.ReferenceEquals(last_interval_node.IntervalBegin, last_interval_node))
					throw new ArgumentOutOfRangeException();

				totIntervalLen += currIntervalLen;
				e = (DDLElement)e.Previous;
			}

			if (totIntervalLen != _ddlList.Count)
				throw new ArgumentOutOfRangeException();
		}

		void print_debug()
		{
			var e = _ddlList.Last;

			while (null != e)
			{
				e.print_debug();
				e = (DDLElement)e.Previous;
			}
		}

#endif
	}
}