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
	/// Problem solver for the longest common substring problem, operating in O(N) time (N being the text length), and using an array of linked structures stored in a linear array instead of linked class instances.
	/// This code runs slightly faster than <see cref="LongestCommonSubstringL"/>, and avoids creating a lot of nodes for the linked list, in order to make it easier for the garbage collector.
	/// </summary>
	/// <remarks>
	/// For details of the algorithm see the very nice paper by Michael Arnold and Enno Ohlebusch, 'Linear Time Algorithms for Generalizations of the Longest Common Substring Problem', Algorithmica (2011) 60; 806-818; DOI: 10.1007/s00453-009-9369-1.
	/// This code was adopted by D.Lellinger from the C++ sources from the web site of the authors at http://www.uni-ulm.de/in/theo/research/sequana.html.
	/// </remarks>
	public class LongestCommonSubstringA : LongestCommonSubstringBaseA
	{
		// intermediate data neccessary for the algorithm
		protected int[] _items;

		/// <summary>Initializes a new instance of the problem solver for the longest common substring problem.</summary>
		/// <param name="gsa">Generalized suffix array. It is neccessary that this was constructed with individual words.</param>
		public LongestCommonSubstringA(GeneralizedSuffixArray gsa)
			: base(gsa)
		{
		}

		/// <summary>Evaluates the longest common substring. After evaluation, the results can be accessed by the properties of this instance. Please be aware that the amount of resulting information depends on
		/// the state of <see cref="P:StoreVerboseResults"/>.
		/// </summary>
		/// <returns>This instance.</returns>
		public LongestCommonSubstringA Evaluate()
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

			EvaluateMaximumNumberOfWordsWithCommonSubstring();
			CleanIntermediates();
			return this;
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

		/// <summary>Initializes all intermediate arrays.</summary>
		private void InitializeIntermediates()
		{
			// initialize intermediates
			_lastLcp = new int[_maximumLcp + 1];
			_ddlList = new LinkedList();
			_ddlList.Init(_numberOfWords);
			_items = new int[_numberOfWords];
			for (int i = 0; i < _numberOfWords; ++i) _items[i] = i;
		}

		/// <summary>Cleans the intermediates so the garbage collector can get them.</summary>
		protected void CleanIntermediates()
		{
			_ddlList.Clear();
			_items = null;
			_lastLcp = null;
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

		/// <summary>To understand the principles of this algorithm see the paper by Michael Arnold and Enno Ohlebusch given in the remarks of the class description (<see cref="LongestCommonSubstringA"/>).</summary>
		/// <param name="lcp_i">The lcp_i.</param>
		/// <param name="index">The index.</param>
		private void lcp_update(int lcp_i, int index)
		{
			var L = _ddlList.L;
			var current = _ddlList.Last;
			var last_updated = current;
			current = L[current].Previous;
			int list_pos = 1;
			while (current >= 0 && L[(L[current]).IntervalEnd].Lcp >= lcp_i)
			{
				last_updated = current = L[current].IntervalEnd;
				list_pos += L[current].IntervalSize;

				// Storing the results
				int currentLcp = L[current].Lcp;
				if (_useVerboseResults)
				{
					if (currentLcp >= _lcsOfNumberOfWords[list_pos])
					{
						bool lcslIsReallyGreaterThanBefore = currentLcp > _lcsOfNumberOfWords[list_pos];
						_lcsOfNumberOfWords[list_pos] = currentLcp;
						StoreVerboseResult(list_pos, L[current].Idx, index - 1, lcslIsReallyGreaterThanBefore);
					}
				}
				else
				{
					if (currentLcp > _lcsOfNumberOfWords[list_pos])
					{
						_lcsOfNumberOfWords[list_pos] = currentLcp;
						_singleResultOfNumberOfWords[list_pos] = new SuffixArrayRegion(L[current].Idx, index - 1);
					}
				} // end storing the results

				current = L[current].Previous;
			} // end while

			L[_ddlList.Last].IntervalEnd = last_updated;
			L[last_updated].IntervalBegin = _ddlList.Last;
			L[last_updated].IntervalSize = list_pos;
			L[last_updated].Lcp = lcp_i;
			_lastLcp[lcp_i] = last_updated;
		}

		/// <summary>To understand the principles of this algorithm see the paper by Michael Arnold and Enno Ohlebusch given in the remarks of the class description (<see cref="LongestCommonSubstringA"/>).</summary>
		/// <param name="end">The end.</param>
		/// <param name="begin">The begin.</param>
		/// <param name="lcp">The LCP.</param>
		/// <param name="size">The size.</param>
		private void create_interval(int end, int begin, int lcp, int size)
		{
			var L = _ddlList.L;
			L[begin].IntervalBegin = begin;
			L[begin].IntervalEnd = end;

			L[end].Lcp = lcp;
			L[end].IntervalBegin = begin;
			L[end].IntervalEnd = end;
			L[end].IntervalSize = size;
		}

		/// <summary>To understand the principles of this algorithm see the paper by Michael Arnold and Enno Ohlebusch given in the remarks of the class description (<see cref="LongestCommonSubstringA"/>).</summary>
		/// <param name="i">The i.</param>
		private void list_update(int i)
		{
			var L = _ddlList.L;
			var sa_i = _suffixArray[i];
			var textlcp = _LCPS[i];

			var currentIdx = _items[_wordIndices[i]];

			if (_lastLcp[textlcp] != currentIdx || L[currentIdx].IntervalBegin != currentIdx)
			{
				// decrease interval size
				--L[_lastLcp[textlcp]].IntervalSize;

				// if text_item is the end of an interval
				if (currentIdx == _lastLcp[textlcp])
				{
					create_interval(L[currentIdx].Next, L[currentIdx].IntervalBegin, L[currentIdx].Lcp, L[currentIdx].IntervalSize);
					_lastLcp[L[currentIdx].Lcp] = L[currentIdx].Next;
				}

				// if text_item is the beginning of an interval
				else if (currentIdx == L[_lastLcp[textlcp]].IntervalBegin)
				{
					create_interval(L[currentIdx].IntervalEnd, L[currentIdx].Previous, L[L[currentIdx].IntervalEnd].Lcp, L[L[currentIdx].IntervalEnd].IntervalSize);
				}

				// reset interval pointers
				L[currentIdx].IntervalEnd = currentIdx;
				L[currentIdx].IntervalBegin = currentIdx;
				L[currentIdx].IntervalSize = 1;
			}

			if (_ddlList.Last != currentIdx)
			{
				// remove nodeToRemove from the list, and add it to the end
				_ddlList.MoveToLast(currentIdx);
				L[L[currentIdx].Previous].Lcp = _LCP[i];
			}

			L[currentIdx].Idx = i;
		}

#if LinkedListDebug

		void Test()
		{
			var L = _ddlList.L;
			int totIntervalLen = 0;
			var eIdx = _ddlList.Last;	var e = L[eIdx];
			while (eIdx>=0)
			{
				e = L[eIdx];
				if (e.IntervalSize <= 0 || e.IntervalSize > _ddlList.L.Length)
					throw new ArgumentOutOfRangeException();
				if (e.Idx < 0 || e.Idx >= _SA.Length)
					throw new ArgumentOutOfRangeException();
				if (eIdx != e.IntervalBegin)
					throw new ArgumentOutOfRangeException();

				int size_from_last_node = e.IntervalSize;
				var last_interval_node = eIdx;
				var first_interval_node = e.IntervalEnd;
				int currIntervalLen = 1;

				// test the intermediate nodes
				while (eIdx != first_interval_node)
				{
					eIdx = e.Previous; e = L[eIdx];
					currIntervalLen++;
					if (e.Idx < 0 || e.Idx >= _SA.Length)
						throw new ArgumentOutOfRangeException();
				}

				// Test the first interval node
				if (e.IntervalSize <= 0 || e.IntervalSize > L.Length)
					throw new ArgumentOutOfRangeException();
				if (e.Idx < 0 || e.Idx >= _SA.Length)
					throw new ArgumentOutOfRangeException();
				if (e.IntervalSize != currIntervalLen)
					throw new ArgumentOutOfRangeException();
				if (e.IntervalEnd != eIdx)
					throw new ArgumentOutOfRangeException();
				if (e.IntervalBegin != last_interval_node)
					throw new ArgumentOutOfRangeException();
				if (L[last_interval_node].IntervalEnd != eIdx)
					throw new ArgumentOutOfRangeException();
				if (L[last_interval_node].IntervalBegin != last_interval_node)
					throw new ArgumentOutOfRangeException();

				totIntervalLen += currIntervalLen;
				eIdx = e.Previous;
			}

			if (totIntervalLen != L.Length)
				throw new ArgumentOutOfRangeException();
		}

		void print_debug()
		{
			var L = _ddlList.L;
			var eIdx = _ddlList.Last;
			var eEle = L[eIdx];

			while (eIdx>=0)
			{
				eEle = L[eIdx];
				Console.WriteLine("Id: {0}, Lcp={1}, Idx={2}, Size={3}, BegId={4}, EndId={5}", eIdx, eEle.Lcp, eEle.Idx, eEle.IntervalSize, eEle.IntervalBegin, eEle.IntervalEnd);
				eIdx = eEle.Previous;
			}
		}

#endif
	}
}
