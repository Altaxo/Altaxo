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
	/// Converts list of arbitrary objects into an integer array. This is done by creating an integer alphabet, which maps each unique element in the original list(s) to an integer value. The lexicographical order of the elements is maintained, i.e.
	/// when a list of elements of ascending order is mapped to an integer list, the integer list is also in ascending order.
	/// </summary>
	public class IntegerText
	{
		/// <summary>Original text, converted to an integer alphabet. Each unique element of the original text (or each unique list element) corresponds to an integer value. The order of this integer alphabet is the same as the order of the original elements. 
		/// Note that the value 0 is reserved for the internal algorithm. If the original text was separated in different words, the first <c>numberOfWords</c> integers (1..<c>numberOfWords</c>) are reserved as separator elements, too.
		/// </summary>
		int[] _text;
		
		/// <summary>
		/// Length of the text. This is the total length of the original text, plus, if the text was separated into words, the number of separator elements (which is equal to the number of words). Note that the 
		/// array <see cref="_text"/> is needed to be longer than <see cref="_textLength"/>, since some additional elements are neccessary for most algorithms.
		/// </summary>
		int _textLength;

		/// <summary>
		/// Number of additional elements of array <see cref="_textLength"/>. Thus the length of this array is <see cref="_paddingLength"/> + <see cref="_textLength"/>.
		/// </summary>
		int _paddingLength;

		/// <summary>
		/// Size of the alphabet, i.e. the number of unique elements that occur in the original text (or, number of unique list elements). If the text was separated into individual words, the number of words (= number of separator elements) also
		/// contribute to the alphabet size.
		/// </summary>
		int _alphabetSize;

		/// <summary>
		/// Number of words, if the text was separated into individual words. Otherwise, this field is equal to one.
		/// </summary>
		int _numberOfWords;

		/// <summary>
		/// Start positions of the words in which the original text was separated in the array <see cref="_text"/>.
		/// </summary>
		int[] _wordStartPositions;

		/// <summary>Original text, converted to an integer alphabet. Each unique element of the original text (or each unique list element) corresponds to an integer value. The order of this integer alphabet is the same as the order of the original elements. 
		/// Note that the value 0 is reserved for the internal algorithm. If the original text was separated in different words, the first <c>numberOfWords</c> integers (1..<c>numberOfWords</c>) are reserved as separator elements, too.
		/// </summary>
		public int[] Text
		{
			get
			{
				return _text;
			}
		}

		/// <summary>
		/// Length of the text. This is the total length of the original text, plus, if the text was separated into words, the number of separator elements (which is equal to the number of words). Note that the 
		/// array <see cref="_text"/> is needed to be longer than <see cref="_textLength"/>, since some additional elements are neccessary for most algorithms.
		/// </summary>
		public int TextLength
		{
			get { return _textLength; }
		}

		/// <summary>
		/// Number of additional elements of array <see cref="Text"/>. Thus the length of this array is <see cref="PaddingLength"/> + <see cref="TextLength"/>
		/// </summary>
		public int PaddingLength
		{
			get { return _paddingLength; }
		}

		/// <summary>
		/// Number of words, if the text was separated into individual words. Otherwise, this field is equal to one.
		/// </summary>
		public int NumberOfWords
		{
			get { return _numberOfWords; }
		}

		/// <summary>
		/// Size of the alphabet, i.e. the number of unique elements that occur in the original text (or, number of unique list elements). If the text was separated into individual words, the number of words (= number of separator elements) also
		/// contribute to the alphabet size.
		/// </summary>
		public int AlphabetSize
		{
			get { return _alphabetSize; }
		}

		/// <summary>
		/// Start positions of the words in which the original text was separated in the array <see cref="_text"/>.
		/// </summary>
		public int[] WordStartPositions
		{
			get
			{
				return _wordStartPositions;
			}
		}

		/// <summary>Generates an integer text from words (= a collection of strings). The algorithm determines the lexicographical order of all elements in all lists
		/// and then maps each unique element to an integer value, with increasing values in the lexicographical order of the elements.</summary>
		/// <param name="words">The list of individual words.</param>
		/// <param name="withSeparators">If set to <c>true</c>, the converted text will contain the concenated 'words', separated by special separator elements. If set to <c>false</c>, the converted text will contain the concenated 'words' without separator elements.</param>
		/// <param name="padding">Number of additional elements reserved in the allocated <see cref="Text"/> array. This is neccessary for some algorithms. The additional elements will contain zero values.</param>
		/// <param name="customComparer">Provides a custom comparer. If you don't want to provide an own comparer, leave this argument <c>null</c>.</param>
		/// <returns>The integer text data, which holds the text converted to an integer alphabet.</returns>
		public static IntegerText FromWords(IEnumerable<string> words, bool withSeparators, int padding, IComparer<char> customComparer)
		{
			IntegerText result = new IntegerText();

			int totalNumberOfElements = 0;
			var sSet = new SortedSet<char>();
			int listCount = 0;
			foreach (var list in words)
			{
				foreach (var ele in list)
				{
					sSet.Add(ele);
				}
				totalNumberOfElements += list.Length;
				++listCount;
			}

			int numberOfSeparators = (withSeparators ? listCount : 0);


			// preprocess the dictionary to give each unique element in the dictionary a unique number
			int startInt = 1 + numberOfSeparators; // list.Count integers offset for the separator char + the zero char to append for the suffix sort algorithm

			var dict = new Dictionary<char, int>();
			foreach (var key in sSet)
			{
				dict[key] = startInt++;
			}


			int[] text = new int[totalNumberOfElements + numberOfSeparators + padding];
			int[] wordStarts = new int[listCount + 1];
			int word = 0;
			int i = 0;
			int separator = 1;
			foreach (var list in words)
			{
				foreach (var ele in list)
				{
					text[i++] = dict[ele];
				}
				if (withSeparators)
					text[i++] = separator++; // add the separator

				wordStarts[++word] = i;
			}

			result._alphabetSize = dict.Count + numberOfSeparators;
			result._paddingLength = padding;
			result._textLength = totalNumberOfElements + numberOfSeparators;
			result._text = text;
			result._numberOfWords = listCount;
			result._wordStartPositions = wordStarts;

			return result;
		}
	
		


		/// <summary>Generates an integer text from arbitrary elements. Each list in <paramref name="lists"/> is treated as separate word. Each element is such a list is treated as character. The algorithm determines the lexicographical order of all elements in all lists
		/// and then maps each unique element to an integer value, with increasing values in the lexicographical order of the elements. A unique mapping is even possible, if the elements are not sortable (i.e. if they not implement IComparable).</summary>
		/// <typeparam name="T">Type of the list elements.</typeparam>
		/// <param name="lists">The list of individual words.</param>
		/// <param name="withSeparators">If set to <c>true</c>, the converted text will contain the concenated 'words', separated by special separator elements. If set to <c>false</c>, the converted text will contain the concenated 'words' without separator elements.</param>
		/// <param name="padding">Number of additional elements reserved in the allocated <see cref="Text"/> array. This is neccessary for some algorithms. The additional elements will contain zero values.</param>
		/// <param name="useSortedMapping">If this parameter is true, a sorted mapping of the elements T to integers will be used. The type T then has to implement IComparable. If this parameter is <c>false</c>, a unsorted <see cref="System.Collections.Generic.HashSet&lt;T&gt;"/> will be used to make a unique mapping of the elements to integers.</param>
		/// <param name="customSortingComparer">If <paramref name="useSortedMapping"/> is <c>true</c>, you can here provide a custom comparer for the elements of type T. Otherwise, if you want to use the default comparer, leave this parameter <c>null</c>.</param>
		/// <returns>The integer text data, which holds the text converted to an integer alphabet.</returns>
		public static IntegerText FromWords<T>(IEnumerable<IEnumerable<T>> lists, bool withSeparators, int padding, bool useSortedMapping, IComparer<T> customSortingComparer)
		{
			IntegerText result = new IntegerText();

			int totalNumberOfElements = 0;
			ISet<T> sSet;
			if (useSortedMapping)
			{
				sSet = null != customSortingComparer ? new SortedSet<T>(customSortingComparer) : new SortedSet<T>();
			}
			else
			{
				sSet = new HashSet<T>();
			}

			int listCount = 0;
			foreach (var list in lists)
			{
				foreach (var ele in list)
				{
					sSet.Add(ele);
					++totalNumberOfElements;
				}
				++listCount;
			}

			int numberOfSeparators = (withSeparators ? listCount : 0);


			// preprocess the dictionary to give each unique element in the dictionary a unique number
			int startInt = 1 + numberOfSeparators; // list.Count integers offset for the separator char + the zero char to append for the suffix sort algorithm

			var dict = new Dictionary<T,int>();
			foreach(var key in sSet)
			{
				dict[key] = startInt++;
			}


			int[] text = new int[totalNumberOfElements + numberOfSeparators + padding];
			int[] wordStarts = new int[listCount + 1];
			int word = 0;
			int i = 0;
			int separator = 1;
			foreach (var list in lists)
			{
				foreach (var ele in list)
				{
					text[i++] = dict[ele];
				}
				if(withSeparators)
					text[i++] = separator++; // add the separator

				wordStarts[++word] = i;
			}

			result._alphabetSize = dict.Count + numberOfSeparators;
			result._paddingLength = padding;
			result._textLength = totalNumberOfElements + numberOfSeparators;
			result._text = text;
			result._numberOfWords = listCount;
			result._wordStartPositions = wordStarts;

			return result;
		}
	}
}
