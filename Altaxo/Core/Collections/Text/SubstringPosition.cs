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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Collections.Text
{
	/// <summary>
	/// Designates the position of a substring in a list of words.
	/// </summary>
	public struct SubstringPosition
	{
		int _wordIndex;
		int _start;
		int _count;

		/// <summary>Initializes a new instance of the <see cref="SubstringPosition"/> struct.</summary>
		/// <param name="wordIndex">Index of the word in the word list.</param>
		/// <param name="start">Starting position of the substring in the word.</param>
		/// <param name="count">Number of elements of the substring.</param>
		public SubstringPosition(int wordIndex, int start, int count)
		{
			_wordIndex = wordIndex;
			_start = start;
			_count = count;
		}

		/// <summary>Number of the word in the list of words.</summary>
		public int WordIndex
		{
			get
			{
				return _wordIndex;
			}
		}

		/// <summary>Starting position of the substring in the word.</summary>
		public int Start
		{
			get
			{
				return _start;
			}
		}

		/// <summary>Number of characters of the substring.</summary>
		public int Count
		{
			get
			{
				return _count;
			}
		}

		public override string ToString()
		{
			return string.Format("Word:{0}; Start:{1}; Length:{2}", _wordIndex, _start, _count);
		}

	}


	/// <summary>
	/// Designates the position of a substring in a list of words.
	/// </summary>
	public struct CommonSubstring
	{
		/// <summary>Suffix array.</summary>
		int[] _suffixArray;

		/// <summary>Word indices, i.e. each element at index i contains the word number that corresponds to the suffix _suffixArray[i].</summary>
		int[] _wordIndices;

		/// <summary>Array of length <c>NumberOfWords+1</c>, indicating the start of the individual words in the concenated text array.</summary>
		int[] _wordStartPositions;

		/// <summary>Starting index in suffix array marking the begin of a region in the suffix array.</summary>
		int _begin;

		/// <summary>Last index (including this index) in the suffix array marking the end of the region.</summary>
		int _end; // last index in _SA array;

		/// <summary>Length of the common substring.</summary>
		int _substringLength;

		/// <summary>Initializes a new instance of the <see cref="SubstringPosition"/> struct.</summary>
		/// <param name="wordIndex">Index of the word in the word list.</param>
		/// <param name="start">Starting position of the substring in the word.</param>
		/// <param name="count">Number of elements of the substring.</param>
		public CommonSubstring(int subStringLength, int beg, int end, int[] suffixArray, int[] wordIndices, int[] wordStartPositions)
		{
			_substringLength = subStringLength;
			_suffixArray = suffixArray;
			_wordIndices = wordIndices;
			_wordStartPositions = wordStartPositions;
			_begin = beg;
			_end = end;
		}

		public SubstringPosition FirstPosition
		{
			get
			{
				return GetSubstringPosition(_begin);
			}
		}


		public IEnumerable<SubstringPosition> Positions
		{
			get
			{
				for (int i = _begin; i <= _end; ++i)
					yield return GetSubstringPosition(i);
			}
		}

		private SubstringPosition GetSubstringPosition(int i)
		{
			int word = _wordIndices[i];
			return new SubstringPosition(word, _suffixArray[i] - _wordStartPositions[word], _substringLength);
		}


		public override string ToString()
		{
			return string.Format("CommonSubstring Length={0}, SuffixArray_Begin={1}, End={2}", _substringLength, _begin, _end);
		}
	}

}
