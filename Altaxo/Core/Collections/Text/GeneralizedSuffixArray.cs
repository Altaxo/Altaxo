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
  /// Given a text string, the substring starting at position i and until the end of text is called the suffix starting at position i. For a text string with N characters there are N suffixes.
  /// This class provides an efficient algorithm to sort all N suffixes lexicographically, and to determine the length of the longest common prefix of each suffix with its lexicographical predecessor.
  /// This is generally done in O(N) time.
  /// </summary>
  /// <remarks>
  /// <para>
  /// First, an O(N) algorithm is used to calculate the suffix array in O(N) time. This algorithm is implemented according to J. Kärkkäinen and P. Sanders: "Simple linear work suffix array construction.",
  /// Proc. 30th International Colloquium on Automata, Languages and Programming, volume 2719 of Lecture Notes in Computer Science, pages 943-955, Berlin, 2003, Springer-Verlag.
  /// </para>
  /// <para>
  /// Second, an O(N) algorithm is necessary to calculate the length of the longest common prefix of two adjacent suffixes. This is implemented according to T. Kasai, G. Lee, H. Arimura, S. Arikawa, and K. Park:
  /// "Linear-Time Longest-Common-Prefix Computation in Suffix Arrays and Its Applications", Proc. 12th Annual Symposium on Combinatorial Pattern Matching. Lecture Notes in Computer Science, vol. 2089, pp. 181–192. Springer, Berlin (2001).
  /// </para>
  /// <para>
  /// See also the paper by Michael Arnold and Enno Ohlebusch, "Linear Time Algorithms for Generalizations of the Longest Common Substring Problem", Algorithmica (2011) 60; 806-818; DOI: 10.1007/s00453-009-9369-1.
  /// The code here was partially adapted from the C++ sources from the authors' website at http://www.uni-ulm.de/in/theo/research/sequana.html.
  /// </para>
  /// </remarks>
  public class GeneralizedSuffixArray
  {
    private static int[] _intArrayEmpty = new int[0];

    // Data from the original text

    /// <summary>Original text, converted to an integer alphabet. Each unique element of the original text (or each unique list element) corresponds to an integer value. The order of this integer alphabet is the same as the order of the original elements.
    /// Note that the value 0 is reserved for the internal algorithm. If the original text was separated into different words, the first <c>numberOfWords</c> integers (1..<c>numberOfWords</c>) are reserved as separator elements, too.
    /// </summary>
    private int[] _text;

    /// <summary>
    /// Length of the text. This is the total length of the original text, plus, if the text was separated into words, the number of separator elements (which is equal to the number of words). Note that the
    /// array <see cref="_text"/> must be longer than <see cref="_textLength"/>, since some additional elements are necessary for most algorithms.
    /// </summary>
    private int _textLength;

    /// <summary>
    /// Number of words, if the text was separated into individual words. Otherwise, this field is equal to one.
    /// </summary>
    private int _numberOfWords;

    /// <summary>
    /// Start positions of the words in which the original text was separated in the array <see cref="_text"/>.
    /// </summary>
    private int[] _wordStartPositions;

    /// <summary>
    /// Size of the alphabet, i.e. the number of unique elements that occur in the original text (or, number of unique list elements). If the text was separated into individual words, the number of words (= number of separator elements) also
    /// contributes to the alphabet size.
    /// </summary>
    private int _alphabetSize;

    /// <summary>Maps the lexicographical order position i of a suffix to the starting position of the suffix in the text, which is the value of the i-th element of this array.</summary>
    private int[] _suffixArray;

    /// <summary>
    /// Maps the suffix that starts at position i in the text to the lexicographical order position of this suffix, which is the value of the i-th element of this array.
    /// </summary>
    private int[] _inverseSuffixArray;

    /// <summary>
    /// Stores the length of the Longest Common Prefix of the lexicographically i-th suffix and its lexicographical predecessor (the lexicographically (i-1)-th suffix).
    /// The element at index 0 is always 0.
    /// </summary>
    private int[] _LCP;

    /// <summary>Stores the length of the Longest Common Prefix <see cref="_LCP"/>, but here only if two adjacent suffixes belong to the same word.
    /// In the other case, i.e. the suffix <c>_suffixArray[i-1]</c> belongs to another word than the suffix <c>_suffixArray[i]</c>, then <c>_LCPS[i]</c> is zero. </summary>
    private int[] _LCPS;

    /// <summary>
    /// Maps the lexicographical order position i of a suffix to the index of the word, in which this suffix starts. This means, for instance, the value of the i-th
    /// element contains the index of the word, in which the lexicographically i-th suffix that starts at position <see cref="_suffixArray"/>[i] begins.
    /// The contents of this array is only meaningful if you provided text that was separated into words, for instance for the longest common substring problem.
    /// </summary>
    private int[] _wordIndices = _intArrayEmpty;

    /// <summary>
    /// Maximum of all values in the <see cref="_LCP"/> array.
    /// </summary>
    private int _maximumLcp = -1;

    #region Constructors

    /// <summary>Constructs an instance of the <see cref="GeneralizedSuffixArray"/> class.</summary>
    /// <param name="textWithPadding">The text. The character of this texts are integers. Each unique element of the original text (or list of objects) is mapped to an unique integer value, while preserving the lexicographical order of the original characters in the integer values.</param>
    /// <param name="textLength">Number of characters of the text. This usually is smaller than the length of text array.</param>
    /// <param name="numberOfWords">The number of words in the original text, if the original text was separated into words. Otherwise, this parameter should be 1.</param>
    /// <param name="wordStartPositions">The start positions of the words in the text array, if the original text was separated into words. Otherwise, this parameter is ignored.</param>
    /// <param name="alphabetSize">Size of the alphabet. This is the number of unique characters (or objects) of the original text. If the text was separated into words, the numberOfWords is added, since each separator is a unique character.</param>
    protected GeneralizedSuffixArray(int[] textWithPadding, int textLength, int numberOfWords, int[] wordStartPositions, int alphabetSize)
    {
      if (textWithPadding is null)
        throw new ArgumentNullException("stringWithPadding is null");
      if (textWithPadding.Length < (textLength + RequiredTextPadding))
        throw new ArgumentNullException("array textWithPadding has to be at least 3 elements longer than the number of data (stringLength)");
      if (numberOfWords > 1)
      {
        if (wordStartPositions is null)
          throw new ArgumentNullException("wordBegins is null");
        if (wordStartPositions.Length < numberOfWords + 1)
          throw new ArgumentException("wordBegins must have at least (numberOfWords+1) elements");
        if (wordStartPositions[numberOfWords] != textLength)
          throw new ArgumentException("end element of array wordBegins is unequal to stringLength. Did you make a mistake in calculating the wordBegins (for instance, forgot taking separators into account)?");
      }

      _text = textWithPadding;
      _textLength = textLength;
      _numberOfWords = numberOfWords;
      _alphabetSize = alphabetSize;
      _wordStartPositions = wordStartPositions;

      _suffixArray = Skew.GetSuffixArray(_text, _textLength, _alphabetSize, new int[_textLength]);
      _inverseSuffixArray = CalculateInverseSuffixArray(_suffixArray);
      _LCP = CalculateLcpArray(_suffixArray, _inverseSuffixArray, _text, _textLength);

      if (numberOfWords >= 2)
      {
        _wordIndices = CalculateWordIndices(_suffixArray, _inverseSuffixArray, _wordStartPositions, _numberOfWords);
        _LCPS = calc_lcptabs(_suffixArray, _inverseSuffixArray, _text, _suffixArray.Length, _numberOfWords, _wordIndices);
      }
      else
      {
        _wordIndices = _intArrayEmpty;
        _LCPS = _intArrayEmpty;
      }
    }

    /// <summary>Constructs a new instance of the <see cref="GeneralizedSuffixArray"/> class from <see cref="IntegerText"/>.</summary>
    /// <param name="integerText">The integer text data.</param>
    public GeneralizedSuffixArray(IntegerText integerText)
      : this(integerText.Text, integerText.TextLength, integerText.NumberOfWords, integerText.WordStartPositions, integerText.AlphabetSize)
    {
    }

    /// <summary>Constructs the generalized suffix array from separate 'words'. Each list in the parameter <paramref name="words"/> is treated as 'word'. The elements are treated as characters. The elements are converted
    /// to an integer alphabet by means of the <see cref="IntegerText"/> class.</summary>
    /// <param name="words">The list of 'words'.</param>
    /// <param name="useSortedMapping">If this parameter is true, a sorted mapping of the elements T to integers will be used. The type T then has to implement IComparable. If this parameter is <c>false</c>, a unsorted <see cref="System.Collections.Generic.HashSet&lt;T&gt;"/> will be used to make a unique mapping of the elements to integers.</param>
    /// <returns>The generalized suffix array. Since each list in <paramref name="words"/> is treated as separate word, the generalized suffix array is prepared to search for the longest common substring in these words.</returns>
    public static GeneralizedSuffixArray FromSeparateWords(IEnumerable<string> words, bool useSortedMapping)
    {
      return FromWords(words, true, true, null);
    }

    /// <summary>Constructs the generalized suffix array from separate 'words'. Each list in the parameter <paramref name="words"/> is treated as 'word'. The elements are treated as characters. The elements are converted
    /// to an integer alphabet by means of the <see cref="IntegerText"/> class.</summary>
    /// <param name="words">The list of 'words'.</param>
    /// <param name="withSeparators">If set to <c>true</c>, the words were treated as individual words, and will be separated by special separator elements. If set to <c>false</c>, the converted text will contain the concenated 'words' without separator elements. If set to <c>false</c>, they will be concenated to form one single word.</param>
    /// <param name="useSortedMapping">If this parameter is true, a sorted mapping of the elements T to integers will be used. The type T then has to implement IComparable. If this parameter is <c>false</c>, a unsorted <see cref="System.Collections.Generic.HashSet&lt;T&gt;"/> will be used to make a unique mapping of the elements to integers.</param>
    /// <param name="customSortingComparer">If <paramref name="useSortedMapping"/> is <c>true</c>, you can here provide a custom comparer for the elements of type T. Otherwise, if you want to use the default comparer, leave this parameter <c>null</c>.</param>
    /// <returns>The generalized suffix array. Since each list in <paramref name="words"/> is treated as separate word, the generalized suffix array is prepared to search for the longest common substring in these words.</returns>
    public static GeneralizedSuffixArray FromWords(IEnumerable<string> words, bool withSeparators, bool useSortedMapping, IComparer<char>? customSortingComparer)
    {
      var integerText = IntegerText.FromWords(words, true, 3, customSortingComparer);
      var result = new GeneralizedSuffixArray(integerText.Text, integerText.TextLength, integerText.NumberOfWords, integerText.WordStartPositions, integerText.AlphabetSize);
      return result;
    }

    /// <summary>Constructs the generalized suffix array from separate 'words'. Each list in the parameter <paramref name="words"/> is treated as 'word'. The elements are treated as characters. The elements are converted
    /// to an integer alphabet by means of the <see cref="IntegerText"/> class.</summary>
    /// <typeparam name="T">The type of characters of the text. It must implement IComparable.</typeparam>
    /// <param name="words">The list of 'words'.</param>
    /// <param name="useSortedMapping">If this parameter is true, a sorted mapping of the elements T to integers will be used. The type T then has to implement IComparable. If this parameter is <c>false</c>, a unsorted <see cref="System.Collections.Generic.HashSet&lt;T&gt;"/> will be used to make a unique mapping of the elements to integers.</param>
    /// <returns>The generalized suffix array. Since each list in <paramref name="words"/> is treated as separate word, the generalized suffix array is prepared to search for the longest common substring in these words.</returns>
    public static GeneralizedSuffixArray FromSeparateWords<T>(IEnumerable<IEnumerable<T>> words, bool useSortedMapping) where T : notnull
    {
      return FromWords<T>(words, true, useSortedMapping, null);
    }

    /// <summary>Constructs the generalized suffix array from separate 'words'. Each list in the parameter <paramref name="words"/> is treated as 'word'. The elements are treated as characters. The elements are converted
    /// to an integer alphabet by means of the <see cref="IntegerText"/> class.</summary>
    /// <typeparam name="T">The type of characters of the text. It must implement IComparable.</typeparam>
    /// <param name="words">The list of 'words'.</param>
    /// <param name="withSeparators">If set to <c>true</c>, the words were treated as individual words, and will be separated by special separator elements. If set to <c>false</c>, the converted text will contain the concenated 'words' without separator elements. If set to <c>false</c>, they will be concenated to form one single word.</param>
    /// <param name="useSortedMapping">If this parameter is true, a sorted mapping of the elements T to integers will be used. The type T then has to implement IComparable. If this parameter is <c>false</c>, a unsorted <see cref="System.Collections.Generic.HashSet&lt;T&gt;"/> will be used to make a unique mapping of the elements to integers.</param>
    /// <param name="customSortingComparer">If <paramref name="useSortedMapping"/> is <c>true</c>, you can here provide a custom comparer for the elements of type T. Otherwise, if you want to use the default comparer, leave this parameter <c>null</c>.</param>
    /// <returns>The generalized suffix array. Since each list in <paramref name="words"/> is treated as separate word, the generalized suffix array is prepared to search for the longest common substring in these words.</returns>
    public static GeneralizedSuffixArray FromWords<T>(IEnumerable<IEnumerable<T>> words, bool withSeparators, bool useSortedMapping, IComparer<T>? customSortingComparer) where T : notnull
    {
      var integerText = IntegerText.FromWords<T>(words, true, 3, useSortedMapping, customSortingComparer);
      var result = new GeneralizedSuffixArray(integerText.Text, integerText.TextLength, integerText.NumberOfWords, integerText.WordStartPositions, integerText.AlphabetSize);
      return result;
    }

    #endregion Constructors

    #region Access to the fields

    /// <summary>Gets the required text padding, i.e. the number of additional elements at the end of the text array that must be left empty (zero).</summary>
    public const int RequiredTextPadding = 3;

    /// <summary>
    /// Number of words, if the text was separated into individual words. Otherwise, this field is equal to one.
    /// </summary>
    public int NumberOfWords
    {
      get { return _numberOfWords; }
    }

    /// <summary>
    /// Start positions of the words in which the original text was separated in the array <see cref="_text"/>.
    /// </summary>
    public int[] WordStartPositions
    {
      get { return _wordStartPositions; }
    }

    /// <summary>
    /// Maps the lexicographical order position i of a suffix to the starting position of the suffix in the text, which is the value of the i-th element of this array.
    /// </summary>
    public int[] SuffixArray
    {
      get { return _suffixArray; }
    }

    /// <summary>
    /// Maps the suffix that starts at position i in the text to the lexicographical order position of this suffix, which is the value of the i-th element of this array.
    /// </summary>
    public int[] InverseSuffixArray
    {
      get { return _inverseSuffixArray; }
    }

    /// <summary>
    /// Maps the lexicographical order position i of a suffix to the index of the word, in which this suffix starts. This means, that for instance the value of the i-th
    /// element contains the index of the word, in which the lexicographically i-th suffix that starts at position <see cref="_suffixArray"/>[i] begins.
    /// The contents of this array is only meaningful, if you provided text that was separated into words, for instance for the longest common substring problem.
    /// </summary>
    public int[] WordIndices
    {
      get { return _wordIndices; }
    }

    /// <summary>
    /// Stores the length of the Longest Common Prefix of the lexicographically i-th suffix and its lexicographical predecessor (the lexicographically (i-1)-th suffix).
    /// The element at index 0 is always 0.
    /// </summary>
    public int[] LCPArray
    {
      get { return _LCP; }
    }

    /// <summary>
    /// </summary>
    public int[] LCPSArray
    {
      get { return _LCPS; }
    }

    /// <summary>
    /// Maximum of all values in the <see cref="_LCP"/> array.
    /// </summary>
    public int MaximumLcp
    {
      get
      {
        if (_maximumLcp < 0)
          _maximumLcp = GetMaximum(_LCP);
        return _maximumLcp;
      }
    }

    #endregion Access to the fields

    /// <summary>Gets the maximum of all elements of <paramref name="arrayOfInt"/>.</summary>
    /// <param name="arrayOfInt">The array of integers.</param>
    /// <returns>The maximum value of all elements of the array.</returns>
    private static int GetMaximum(int[] arrayOfInt)
    {
      int max = arrayOfInt[0];
      for (int i = 1; i < arrayOfInt.Length; i++)
      {
        if (arrayOfInt[i] > max)
        {
          max = arrayOfInt[i];
        }
      }
      return max;
    }

    /// <summary>Calculates the word indices, <see cref="P:WordIndices"/>.</summary>
    /// <param name="suffixArray">The suffix array.</param>
    /// <param name="inverseSuffixArray">The inverse suffix array.</param>
    /// <param name="wordStartPositions">The word start positions.</param>
    /// <param name="numberOfWords">The number of words.</param>
    /// <returns>Array of word indices.</returns>
    private static int[] CalculateWordIndices(int[] suffixArray, int[] inverseSuffixArray, int[] wordStartPositions, int numberOfWords)
    {
      var wordIndices = new int[suffixArray.Length + 1];

      int index = 0;
      int j = 0;
      for (int i = 0; i < numberOfWords; i++)
      {
        int next_wordbegin = wordStartPositions[i + 1];
        for (; j < next_wordbegin; j++)
        {
          wordIndices[inverseSuffixArray[j]] = index;
        }
        index++;
      }
      wordIndices[suffixArray.Length] = 0;
      return wordIndices;
    }

    /// <summary>Calculates the inverse suffix array (<see cref="P:InverseSuffixArray"/>) from the <see cref="P:SuffixArray"/>.</summary>
    /// <param name="suffixArray">The suffix array.</param>
    /// <returns>The inverse suffix array.</returns>
    private static int[] CalculateInverseSuffixArray(int[] suffixArray)
    {
      var inverseSuffixArray = new int[suffixArray.Length];

      for (int i = 0; i < suffixArray.Length; ++i)
        inverseSuffixArray[suffixArray[i]] = i;

      return inverseSuffixArray;
    }

    /// <summary>Calculates the LCP (longest common prefix) array from the suffix array and the inverse suffix array.</summary>
    /// <param name="suffixArray">The suffix array, i.e. the array of indices that designate the lexicographical order of the suffixes.</param>
    /// <param name="inverseSuffixArray">Inverse suffix array.</param>
    /// <param name="text">The integer text. Each element of the original text (or object array) is represented by an integer, so that equal elements of the original text map to equal integers.</param>
    /// <param name="textLength">Text length, i.e. the number of text elements. Can be smaller than the size of the <paramref name="text"/> array, since for some algorithms, the text array need to be longer than the text.</param>
    /// <returns>The LCP array, which stores at index <c>i</c> the longest common prefix of the suffix at index <c>i</c> and the suffix at index <c>i-1</c>.</returns>
    private static int[] CalculateLcpArray(int[] suffixArray, int[] inverseSuffixArray, int[] text, int textLength)
    {
      var lcpArray = new int[textLength];
      lcpArray[0] = 0;
      int i, j, k, l = 0;

      for (i = 0; i < textLength; i++)
      {
        if (0 != (j = inverseSuffixArray[i]))
        {
          k = suffixArray[j - 1];
          while (text[k + l] == text[i + l])
          {
            ++l;
          }
          lcpArray[j] = l;
          l = Math.Max(l - 1, 0);
        }
      }
      return lcpArray;
    }

    /// <summary>Calculate lcp tabs for each text using the generalized suffix array</summary>
    /// <param name="suffixArray">The suffix array, i.e. the array of indices that designate the lexicographical order of the suffixes.</param>
    /// <param name="inverseSuffixArray">Inverse suffix array.</param>
    /// <param name="text">The text. Each element of the original text (or object array) is represented by an integer, so that equal elements of the original text map to equal integers.</param>
    /// <param name="numberOfWords">The num_words.</param>
    /// <param name="textLength">Number of text elements. Can be smaller than the size of the <paramref name="text"/> array, since for some algorithms, the text array have to be longer than the text.</param>
    /// <param name="wordIndices">The wordindex array.</param>
    private static int[] calc_lcptabs(int[] suffixArray, int[] inverseSuffixArray, int[] text, int textLength, int numberOfWords, int[] wordIndices)
    {
      var lcptabs = new int[textLength];

      int[] last_occ = new int[textLength];
      int[] last_wordocc = new int[numberOfWords];
      int[] l = new int[numberOfWords];
      for (int i = 0; i < numberOfWords; i++)
      {
        last_wordocc[i] = 0;
        l[i] = 0;
      }

      for (int i = 0; i < textLength; i++)
      {
        int t = wordIndices[i];
        last_occ[i] = last_wordocc[t];
        last_wordocc[t] = i;
      }

      int j, k;
      lcptabs[0] = 0;
      for (int i = 0; i < textLength; i++)
      {
        if (0 != (j = inverseSuffixArray[i]))
        {
          int t = wordIndices[j];
          k = suffixArray[last_occ[j]];
          while (text[k + l[t]] == text[i + l[t]])
          {
            l[t]++;
          }
          lcptabs[j] = l[t];
          l[t] = Math.Max(l[t] - 1, 0);
        }
      }

      return lcptabs;
    }

    /// <summary>
    /// Algorithm to calculate the suffix array in O(N) time.
    /// This algorithm is implemented according to
    /// "J. Kärkkäinen and P. Sanders. Simple linear work suffix array construction.
    /// In Proc. 30th International Colloquium on Automata, Languages and Programming, volume 2719
    /// of Lecture Notes in Computer Science, pages 943-955, Berlin, 2003, Springer-Verlag."
    /// </summary>
    private class Skew
    {
      private static bool leq(int a1, int a2, int b1, int b2)
      {
        return (a1 < b1 || a1 == b1 && a2 <= b2);
      }

      private static bool leq(int a1, int a2, int a3, int b1, int b2, int b3)
      {
        return (a1 < b1 || a1 == b1 && leq(a2, a3, b2, b3));
      }

      /// <summary>Radixes the pass.</summary>
      /// <param name="a">A.</param>
      /// <param name="b">The b.</param>
      /// <param name="r">The r.</param>
      /// <param name="offs">Offset into the array <c>r</c>.</param>
      /// <param name="n">Number of elements of the text.</param>
      /// <param name="K">Size of the alphabet (i.e. number of different elements in the text).</param>
      /// <param name="c">Temporary array. Must have length of (<c>K+1</c>. Will be changed during operation.</param>
      private static void radixPass(int[] a, int[] b, int[] r, int offs, int n, int K, int[] c)
      {
        Array.Clear(c, 0, c.Length); // fill c with zeros
        for (int i = 0; i < n; i++)
          c[r[offs + a[i]]]++;
        for (int i = 0, sum = 0; i <= K; i++)
        {
          int t = c[i];
          c[i] = sum;
          sum += t;
        }
        for (int i = 0; i < n; i++)
          b[c[r[offs + a[i]]]++] = a[i];
      }

      // find the suffix array SA of s[0..n-1] in {1..K}^n
      // require s[n]=s[n+1]=s[n+2]=0, n>=2

      /// <summary>Find the suffix array SA of s[0..n-1] in {1..K}^n</summary>
      /// <param name="s">The text. Each element of the original text (or object array) is represented by an integer, so that equal elements of the original text map to equal integers.
      /// The number of <b>different</b> elements that occur in the text is called the 'alphabet size' <c>K</c></param>
      /// <param name="n">The number of text elements (i.e. the text length). Note that the array <paramref name="s"/> must have a length at least 3 elements longer than the number of text elements.</param>
      /// <param name="K">The alphabet size, i.e. the number of really different elements that occur in the text (if the text contains separators, each separator count as a different element). K is the value of the highest integer value in <paramref name="s"/>.</param>
      /// <param name="SA">On return, contains the suffix array.</param>
      /// <returns>The suffix array that was provided as the parameter <paramref name="SA"/></returns>
      public static int[] GetSuffixArray(int[] s, int n, int K, int[] SA)
      {
        int n0 = (n + 2) / 3, n1 = (n + 1) / 3, n2 = n / 3, n02 = n0 + n2;
        int[] s12 = new int[n02 + 3];
        s12[n02] = s12[n02 + 1] = s12[n02 + 2] = 0;
        int[] SA12 = new int[n02 + 3];
        SA12[n02] = SA12[n02 + 1] = SA12[n02 + 2] = 0;
        int[] s0 = new int[n0];
        int[] SA0 = new int[n0];
        int[] tempC = new int[K + 1];

        for (int i = 0, j = 0; i < n + (n0 - n1); i++)
          if (i % 3 != 0)
            s12[j++] = i;

        // lsb radix sort the mod 1 and mod 2 triples
        radixPass(s12, SA12, s, 2, n02, K, tempC);
        radixPass(SA12, s12, s, 1, n02, K, tempC);
        radixPass(s12, SA12, s, 0, n02, K, tempC);

        // find lexicographic names of triples
        int name = 0, c0 = -1, c1 = -1, c2 = -1;
        for (int i = 0; i < n02; i++)
        {
          if (s[SA12[i]] != c0 || s[SA12[i] + 1] != c1 || s[SA12[i] + 2] != c2)
          {
            name++;
            c0 = s[SA12[i]];
            c1 = s[SA12[i] + 1];
            c2 = s[SA12[i] + 2];
          }
          if (SA12[i] % 3 == 1)
          { s12[SA12[i] / 3] = name; } // left half
          else
          { s12[SA12[i] / 3 + n0] = name; } // right half
        }

        // recurse if names are not yet unique
        if (name < n02)
        {
          GetSuffixArray(s12, n02, name, SA12);
          // store unique names in s12 using the suffix array
          for (int i = 0; i < n02; i++)
            s12[SA12[i]] = i + 1;
        }
        else // generate the suffix array of s12 directly
          for (int i = 0; i < n02; i++)
            SA12[s12[i] - 1] = i;

        // stably sort the mod 0 suffixes from SA12 by their first character
        for (int i = 0, j = 0; i < n02; i++)
          if (SA12[i] < n0)
            s0[j++] = 3 * SA12[i];
        radixPass(s0, SA0, s, 0, n0, K, tempC);

        // merge sorted SA0 suffixes and sorted SA12 suffixes
        for (int p = 0, t = n0 - n1, k = 0; k < n; k++)
        {
          // #define GetI() (SA12[t] < n0 ? SA12[t] * 3 + 1 : (SA12[t] - n0) * 3 + 2) // had to insert this macro at two places in the program... (see below)
          int i = (SA12[t] < n0 ? SA12[t] * 3 + 1 : (SA12[t] - n0) * 3 + 2); // pos of current offset 12 suffix  (1st place of using GetI() macro from definition above)
          int j = SA0[p]; // pos of current offset 0  suffix
          if (SA12[t] < n0 ?
              leq(s[i], s12[SA12[t] + n0], s[j], s12[j / 3]) :
              leq(s[i], s[i + 1], s12[SA12[t] - n0 + 1], s[j], s[j + 1], s12[j / 3 + n0]))
          { // suffix from SA12 is smaller
            SA[k] = i;
            t++;
            if (t == n02)
            { // done --- only SA0 suffixes left
              for (k++; p < n0; p++, k++)
                SA[k] = SA0[p];
            }
          }
          else
          {
            SA[k] = j;
            p++;
            if (p == n0)
            { // done --- only SA12 suffixes left
              for (k++; t < n02; t++, k++)
                SA[k] = (SA12[t] < n0 ? SA12[t] * 3 + 1 : (SA12[t] - n0) * 3 + 2); //  2nd place of using GetI() macro from definition above)
            }
          }
        }

        return SA;
      }
    }
  }
}
