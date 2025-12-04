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
  /// Base class for problem solvers for the longest common substring problem.
  /// </summary>
  /// <remarks>
  /// For details of the algorithm, see the paper by Michael Arnold and Enno Ohlebusch, "Linear Time Algorithms for Generalizations of the Longest Common Substring Problem", Algorithmica (2011) 60; 806-818; DOI: 10.1007/s00453-009-9369-1.
  /// This code was adapted from the C++ sources from the authors' website at http://www.uni-ulm.de/in/theo/research/sequana.html.
  /// </remarks>
  public class LongestCommonSubstringBase
  {
    #region internal types

    /// <summary>
    /// Stores a region in the suffix array.
    /// </summary>
    protected struct SuffixArrayRegion
    {
      /// <summary>First index in the suffix array.</summary>
      public int Begin;
      /// <summary>Last index in the suffix array (this index is included in the region).</summary>
      public int End;
      /// <summary>
      /// Initializes a new instance of the <see cref="SuffixArrayRegion"/> struct.
      /// </summary>
      /// <param name="beg">The first index in the suffix array.</param>
      /// <param name="end">The last index in the suffix array.</param>
      public SuffixArrayRegion(int beg, int end)
      {
        Begin = beg;
        End = end;
      }
    }

    /// <summary>
    /// Given a sequence of numeric values that will be added to this instance, the algorithm keeps track of the minimum value of the last <c>numberOfItems</c> added values.
    /// The name of the algorithm is ascending minima algorithm, one of the algorithms in the class of "minimum on a sliding window algorithms".
    /// </summary>
    protected struct MinimumOnSlidingWindow
    {
      #region Item

      /// <summary>
      /// Internal data structure to store the value, and the generation number when this value moves out of the sliding window.
      /// </summary>
      private struct Bucket
      {
        /// <summary>Value of this bucket.</summary>
        public int Value;
        /// <summary>Number of generation, when this bucket can be considered as expired, i.e. is moved out of the sliding window, and therefore must be removed from the collection.</summary>
        public int ExpireGeneration;
      }

      #endregion Item

      /// <summary>Counter that is incremented each time an element is added.</summary>
      private int _generation;

      /// <summary>Array of Bucket structs storing the value and the generation when this value will become invalid.</summary>
      private Bucket[] _items;

      /// <summary>Index of the bucket with the lowest value in the bucket array.</summary>
      private int _minItemIdx;

      /// <summary>Index of the bucket that was the last added value in the array.</summary>
      private int _lastItemIdx;

      /// <summary>
      /// Initializes a new instance of the <see cref="MinimumOnSlidingWindow"/> class.
      /// </summary>
      /// <param name="numberOfItems">The number of items N. The algorithm evaluates the minimum of the last N items that were added to this instance.</param>
      /// <param name="startValue">The start value. This is the first entry to add to the instance. Thus, <see cref="MinimumValue"/> always returns a valid value.</param>
      public void Initialize(int numberOfItems, int startValue)
      {
        _items = new Bucket[numberOfItems];
        _items[_minItemIdx].ExpireGeneration = _generation + _items.Length;
        _items[_minItemIdx].Value = startValue;
        ++_generation;
      }

      /// <summary>
      /// Gets the current minimum value of the window.
      /// </summary>
      public int MinimumValue
      {
        get
        {
          return _items[_minItemIdx].Value;
        }
      }

      /// <summary>
      /// Removes the expired element from this window. Note: normally this is done when you use the <see cref="Add"/> function, thus there is no need to call this function separately.
      /// When the minimum item is the item that is expired now, then this function will remove this item from the collection and sets the current minimum to the next greater item.
      /// </summary>
      public void Remove()
      {
        if (_items[_minItemIdx].ExpireGeneration == _generation)
        {
          _minItemIdx++;
          if (_minItemIdx >= _items.Length)
            _minItemIdx = 0;
        }
      }

      /// <summary>
      /// Adds the specified value to the window, and removes the item that is now expired from the window.
      /// </summary>
      /// <param name="val">The value to add.</param>
      public void Add(int val)
      {
        if (_items[_minItemIdx].ExpireGeneration == _generation)
        {
          _minItemIdx++;
          if (_minItemIdx >= _items.Length)
            _minItemIdx = 0;
        }

        if (val <= _items[_minItemIdx].Value)
        {
          _items[_minItemIdx].Value = val;
          _items[_minItemIdx].ExpireGeneration = _generation + _items.Length;
          _lastItemIdx = _minItemIdx;
        }
        else
        {
          while (_items[_lastItemIdx].Value >= val)
          {
            if (_lastItemIdx == 0)
              _lastItemIdx = _items.Length;
            --_lastItemIdx;
          }
          ++_lastItemIdx;
          if (_lastItemIdx == _items.Length)
            _lastItemIdx = 0;

          _items[_lastItemIdx].Value = val;
          _items[_lastItemIdx].ExpireGeneration = _generation + _items.Length;
        }
        ++_generation;
      }
    }

    #endregion internal types

    protected const string ERROR_NO_RESULTS_YET = "There are no results available yet - please execute the algorithm first!";

    // copy-paste from the generalized suffix array

    /// <summary>Number of words the text was separated into.</summary>
    protected int _numberOfWords;

    /// <summary>Maps the lexicographical order position i of a suffix to the starting position of the suffix in the text, which is the value of the i-th element of this array.</summary>
    protected int[] _suffixArray;

    /// <summary>
    /// Maps the lexicographical order position i of a suffix to the index of the word, in which this suffix starts. This means, that for instance the value of the i-th
    /// element contains the index of the word, in which the lexicographically i-th suffix that starts at position <see cref="_suffixArray"/>[i] begins.
    /// The contents of this array is only meaningful, if you provided text that was separated into words, for instance for the longest common substring problem.
    /// </summary>
    protected int[] _wordIndices;

    /// <summary>
    /// Start positions of the words in which the original text was separated in the concenated text array.
    /// </summary>
    protected int[] _wordStartPositions;

    /// <summary>
    /// Stores the length of the Longest Common Prefix of the lexicographically i-th suffix and its lexicographical  predecessor (the lexicographically (i-1)-th suffix).
    /// The element at index 0 is always 0.
    /// </summary>
    protected int[] _LCP;

    /// <summary>Stores the length of the Longest Common Prefix <see cref="_LCP"/>, but here only if two adjacent suffixes belong to the same word.
    /// In the other case, i.e. the suffix <c>_suffixArray[i-1]</c> belongs to another word than the suffix <c>_suffixArray[i]</c>, then <c>_LCPS[i]</c> is zero. </summary>
    protected int[] _LCPS;

    /// <summary>
    /// Maximum of all values in the <see cref="_LCP"/> array.
    /// </summary>
    protected int _maximumLcp;

    // resulting data

    /// <summary>
    /// Stores in element <c>idx</c> the length of the longest substring that is common to <c>idx</c> number of words (it follows that index 0 and 1 are unused here).
    /// </summary>
    protected int[]? _lcsOfNumberOfWords;

    /// <summary>
    /// If <see cref="_verboseResultsOfNumberOfWords"/> is false, stores only the first report of a longest common string for the given number of words.
    /// The content of one element is the beginning and the end index in the suffix array that indicate all suffixes that have this substring in common.
    /// The length of this substring is stored in <see cref="_lcsOfNumberOfWords"/> at the same index.
    /// If <see cref="_verboseResultsOfNumberOfWords"/> is true, this array is not used and is set to <c>null</c>.
    /// </summary>
    protected SuffixArrayRegion[]? _singleResultOfNumberOfWords;

    /// <summary>
    /// If <see cref="_verboseResultsOfNumberOfWords"/> is true, this array stores, for a given number of words that have one or more substrings in common, a list with all positions where such common substrings occur.
    /// The content of one element of each list is the beginning and the end index in the suffix array that indicate all suffixes that have a substring in common. The length of this substring is stored in <see cref="_lcsOfNumberOfWords"/>
    /// If <see cref="_verboseResultsOfNumberOfWords"/> is false, this array is not used and is set to <c>null</c>.
    /// </summary>
    protected List<SuffixArrayRegion>[]? _verboseResultsOfNumberOfWords;

    /// <summary>
    /// Determines the amount of information to store during evaluation.
    /// </summary>
    protected bool _useVerboseResults = false;

    /// <summary>
    /// Contains the maximum number of words that have a common substring.
    /// </summary>
    protected int _maximumNumberOfWordsWithCommonSubstring;

    /// <summary>Initializes a new instance of the problem solver for the longest common substring problem.</summary>
    /// <param name="gsa">Generalized suffix array. It is neccessary that this was constructed with individual words.</param>
    public LongestCommonSubstringBase(GeneralizedSuffixArray gsa)
    {
      _numberOfWords = gsa.NumberOfWords;
      _wordIndices = gsa.WordIndices;
      _wordStartPositions = gsa.WordStartPositions;
      _suffixArray = gsa.SuffixArray;
      _LCP = gsa.LCPArray;
      _LCPS = gsa.LCPSArray;
      _maximumLcp = gsa.MaximumLcp;
    }

    /// <summary>Gets or sets a value indicating whether to store all longest common substrings for a given number of words or just one.</summary>
    /// <value>If <c>true</c>, all longest common substrings will be stored during evaluation. This reduces the speed of the evaluation considerably.
    /// If you are interested in just one longest common substring, set this property to <c>false</c>.</value>
    public bool StoreVerboseResults
    {
      get
      {
        return _useVerboseResults;
      }
      set
      {
        var oldValue = _useVerboseResults;
        _useVerboseResults = value;
        if (oldValue != value)
        {
          _verboseResultsOfNumberOfWords = null;
          _singleResultOfNumberOfWords = null;
        }
      }
    }

    /// <summary>Gets or sets the maximum number of words with a common substring.</summary>
    /// <value>The maximum number of words with a common substring.</value>
    public int MaximumNumberOfWordsWithCommonSubstring
    {
      get { return _maximumNumberOfWordsWithCommonSubstring; }
    }

    /// <summary>Returns the positions for common substrings for the maximum number of words that have at least one common substring. The result
    /// is identical to a call of <see cref="GetSubstringPositionsCommonToTheNumberOfWords"/> with the argument <see cref="MaximumNumberOfWordsWithCommonSubstring"/>
    /// </summary>
    public IEnumerable<CommonSubstring> CommonSubstringPositionsForMaximumNumberOfWords
    {
      get
      {
        return GetSubstringPositionsCommonToTheNumberOfWords(_maximumNumberOfWordsWithCommonSubstring);
      }
    }

    /// <summary>Returns the positions for common substrings for the given number of words</summary>
    /// <param name="numberOfWordsWithCommonSubstring">Number of words</param>
    /// <returns>An enumeration will all positions of substrings common to the given number of words. The amount of information returned depends on the state of <see cref="StoreVerboseResults"/>.</returns>
    public IEnumerable<CommonSubstring> GetSubstringPositionsCommonToTheNumberOfWords(int numberOfWordsWithCommonSubstring)
    {
      if (_lcsOfNumberOfWords is null)
        throw new InvalidOperationException(ERROR_NO_RESULTS_YET);

      int substringLength = _lcsOfNumberOfWords[numberOfWordsWithCommonSubstring];
      if (substringLength > 0)
      {
        if (_verboseResultsOfNumberOfWords is not null)
        {
          var list = _verboseResultsOfNumberOfWords[numberOfWordsWithCommonSubstring];
          if (list is not null)
          {
            foreach (var ele in list)
            {
              yield return new CommonSubstring(substringLength, ele.Begin, ele.End, _suffixArray, _wordIndices, _wordStartPositions);
            }
          }
        }
        else if (_singleResultOfNumberOfWords is not null)
        {
          var ele = _singleResultOfNumberOfWords[numberOfWordsWithCommonSubstring];
          yield return new CommonSubstring(substringLength, ele.Begin, ele.End, _suffixArray, _wordIndices, _wordStartPositions);
        }
      }
    }

    /// <summary>Stores a common substring occurence.</summary>
    /// <param name="list_pos">Number of words that have this common substring.</param>
    /// <param name="beg">Start index of the suffix in the suffix array.</param>
    /// <param name="end">End index of the suffix in the suffix array.</param>
    /// <param name="lcslIsReallyGreaterThanBefore">If set to <c>true</c>, the longest common substring length is really greater than evaluated before. Thus, it is neccessary to clear all results stored so far.</param>
    protected void StoreVerboseResult(int list_pos, int beg, int end, bool lcslIsReallyGreaterThanBefore)
    {
      if (_verboseResultsOfNumberOfWords is null)
        throw new InvalidProgramException();

      var list = _verboseResultsOfNumberOfWords[list_pos];
      if (list is null)
        _verboseResultsOfNumberOfWords[list_pos] = list = new List<SuffixArrayRegion>();
      else if (lcslIsReallyGreaterThanBefore)
        list.Clear();
      list.Add(new SuffixArrayRegion(beg, end));
    }
  }
}
