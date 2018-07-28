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
using System.Text;

namespace Altaxo.Serialization.Ascii
{
  /// <summary>
  /// Provides a coarse analysis of the first few lines of the ascii file. This analysis looks for separation chars,
  /// looks for whitespaces, and if appropriate, tests if there is a structure with fixed column widths.
  /// </summary>
  public class AsciiGlobalStructureAnalysis
  {
    /// <summary>True if any of the lines contains a tabulator char.</summary>
    private bool _containsTabs;

    /// <summary>True if any of the lines contains a comma char.</summary>
    private bool _containsCommas;

    /// <summary>True if any of the lines contains a semicolon char.</summary>
    private bool _containsSemicolons;

    /// <summary>True if any of the lines contains a space char.</summary>
    private bool _containsSpaces;

    /// <summary>True if any of the lines contains more than one whitespace successively.</summary>
    private bool _containsRepeatingWhiteSpaces;

    /// <summary>
    /// If a fixed column width structure was recognized, this element contains the tabbed start positions of
    /// the columns, assuming a tab size of <see cref="_recognizedTabSize"/>.
    /// </summary>
    private List<int> _fixedBoundaries;

    /// If a fixed column width structure was recognized, this element contains the recognized tab size.
    private int _recognizedTabSize = 1;

    /// <summary>True if any of the lines contains a tabulator char.</summary>
    public bool ContainsTabs
    {
      get
      {
        return _containsTabs;
      }
    }

    /// <summary>True if any of the lines contains a comma char.</summary>
    public bool ContainsCommas
    {
      get
      {
        return _containsCommas;
      }
    }

    /// <summary>True if any of the lines contains a semicolon char.</summary>
    public bool ContainsSemicolons
    {
      get
      {
        return _containsSemicolons;
      }
    }

    /// <summary>
    /// If a fixed column width structure was recognized, this element contains the tabbed start positions of
    /// the columns, assuming a tab size of <see cref="_recognizedTabSize"/>.
    /// </summary>
    public List<int> FixedBoundaries
    {
      get
      {
        return _fixedBoundaries;
      }
    }

    /// If a fixed column width structure was recognized, this element contains the recognized tab size. Otherwise it is 1.
    public int RecognizedTabSize
    {
      get
      {
        return _recognizedTabSize;
      }
    }

    /// <summary>
    /// Constructor for the analysis. You must provide the first few lines of a ascii file (say 30) in order to have a good analysis.
    /// </summary>
    /// <param name="firstLines">List of the first lines of an ascii file.</param>
    public AsciiGlobalStructureAnalysis(List<string> firstLines)
    {
      _containsTabs = TestForTabs(firstLines);
      _containsSpaces = TestForSpaces(firstLines);
      _containsCommas = TestForCommas(firstLines);
      _containsSemicolons = TestForSemicolons(firstLines);
      _containsRepeatingWhiteSpaces = TestForRepeatingWhiteSpaces(firstLines);

      // if the document contains repeating white spaces, it is a good candidate for
      // fixed column width structure

      if (_containsRepeatingWhiteSpaces || (_containsTabs && _containsSpaces))
      {
        List<List<int>> listOfStringWordBounds = new List<List<int>>();
        foreach (string s in firstLines)
          listOfStringWordBounds.Add(GetStringWordBounds(s));

        if (!_containsTabs)
        {
          int quality;
          _fixedBoundaries = FindCommonWordBoundaries(listOfStringWordBounds, out quality);
        }
        else
        {
          int quality4;
          int quality8;
          List<List<int>> listOfTabbedWordBounds = new List<List<int>>();
          for (int i = 0; i < firstLines.Count; i++)
            listOfTabbedWordBounds.Add(GetTabbedWordBounds(firstLines[i], listOfStringWordBounds[i], 4));
          List<int> bounds4 = FindCommonWordBoundaries(listOfTabbedWordBounds, out quality4);

          listOfTabbedWordBounds.Clear();
          for (int i = 0; i < firstLines.Count; i++)
            listOfTabbedWordBounds.Add(GetTabbedWordBounds(firstLines[i], listOfStringWordBounds[i], 8));
          List<int> bounds8 = FindCommonWordBoundaries(listOfTabbedWordBounds, out quality8);

          if (quality4 > quality8)
          {
            _fixedBoundaries = bounds4;
            _recognizedTabSize = 4;
          }
          else
          {
            _fixedBoundaries = bounds8;
            _recognizedTabSize = 8;
          }
        }
      }
    }

    /// <summary>
    /// Tests if any of the lines contains a tabulator char.
    /// </summary>
    /// <param name="lines">The lines to test.</param>
    /// <returns>True if any of the lines contains a tabulator char.</returns>
    public static bool TestForTabs(List<string> lines)
    {
      foreach (string s in lines)
        if (s.IndexOf('\t') >= 0)
          return true;

      return false;
    }

    /// <summary>
    /// Tests if any of the lines contains a comma char.
    /// </summary>
    /// <param name="lines">The lines to test.</param>
    /// <returns>True if any of the lines contains a tabulator char.</returns>
    public static bool TestForCommas(List<string> lines)
    {
      foreach (string s in lines)
        if (s.IndexOf(',') >= 0)
          return true;

      return false;
    }

    /// <summary>
    /// Tests if any of the lines contains a semicolon char.
    /// </summary>
    /// <param name="lines">The lines to test.</param>
    /// <returns>True if any of the lines contains a tabulator char.</returns>
    public static bool TestForSemicolons(List<string> lines)
    {
      foreach (string s in lines)
        if (s.IndexOf(';') >= 0)
          return true;

      return false;
    }

    /// <summary>
    /// Tests if any of the lines contains a space char.
    /// </summary>
    /// <param name="lines">The lines to test.</param>
    /// <returns>True if any of the lines contains a space char.</returns>
    public static bool TestForSpaces(List<string> lines)
    {
      foreach (string s in lines)
        if (s.IndexOf(' ') >= 0)
          return true;

      return false;
    }

    /// <summary>
    /// Tests if any of the lines contains at least two subsequent whitespaces.
    /// </summary>
    /// <param name="lines">The lines to test.</param>
    /// <returns>True if any of the lines contains at least two subsequent whitespaces.</returns>

    public static bool TestForRepeatingWhiteSpaces(List<string> lines)
    {
      foreach (string s in lines)
      {
        for (int i = 0; i < s.Length - 1; i++)
          if (char.IsWhiteSpace(s[i]) && char.IsWhiteSpace(s[i + 1]))
            return true;
      }
      return false;
    }

    /// <summary>
    /// Finds common word boundaries. You have to provide the word bounds for a few lines of text.
    /// </summary>
    /// <param name="listOfTabbedWordBounds">Tabbed word bounds for some lines of text.</param>
    /// <param name="qualityScore">Returns a number that gives some hint about the quality of the found word boundaries. The higher this number, the more lines have common word boundaries.</param>
    /// <returns>List of tabbed (!) positions that are most common to the majority of lines. Note that the first token
    /// always start at 0, the first element in the list is the position of the second token.</returns>
    private List<int> FindCommonWordBoundaries(List<List<int>> listOfTabbedWordBounds, out int qualityScore)
    {
      double relThreshold = 0.5;
      int maxlen = MaxTabbedLineLength(listOfTabbedWordBounds);
      // we allocate one element more to make it easier to handle the end of line
      int[] inWords = new int[maxlen + 1];
      int[] startOfWords = new int[maxlen + 1];
      int[] endOfWords = new int[maxlen + 1];
      int[] deepInWords = new int[maxlen + 1];

      ReportInsideOfWords(listOfTabbedWordBounds, inWords);
      ReportStartOfWords(listOfTabbedWordBounds, startOfWords);
      ReportEndOfWords(listOfTabbedWordBounds, endOfWords);
      ReportDeepInsideOfWords(listOfTabbedWordBounds, deepInWords);

      List<int> bounds = new List<int>();
      int? lastLeftJustifiedPreliminaryRightBoundary = null;

      int unsharpBoundFields = 0;

      // now search for significant boundaries and if left or right-justified
      int nThresh = (int)(relThreshold * listOfTabbedWordBounds.Count);

      for (int nPos = 0; nPos < maxlen;)
      {
        if (inWords[nPos] >= nThresh) // the majority of lines has a word at this position
        {
          int nLeftPos = nPos;
          int leftincrement = startOfWords[nLeftPos];
          int nRightPos = nPos;
          for (; nRightPos < maxlen; nRightPos++)
            if (inWords[nRightPos] < nThresh)
              break;
          int rightdecrement = endOfWords[nRightPos];

          if (rightdecrement >= leftincrement && rightdecrement > nThresh) // right justified because rightDecrement is higher than leftincrement
          {
            if (lastLeftJustifiedPreliminaryRightBoundary.HasValue) // last field was left justified, this field is right justified, so this is tricky
            {
              // we have to find the boundary between the last left-justified field and
              // the right justified field now, best in my opinion is the minimum of the word count
              // because often this the minimum is broad, we use the mean value of minLPos (start of the minimum) and minRPos (end of the minimum)
              int min = int.MaxValue;
              int minLPos = lastLeftJustifiedPreliminaryRightBoundary.Value;
              int minRPos = nLeftPos - 1;
              for (int k = lastLeftJustifiedPreliminaryRightBoundary.Value; k < nLeftPos; k++)
              {
                if (inWords[k] < min)
                {
                  min = inWords[k];
                  minLPos = k;
                  minRPos = k;
                }
                else if (inWords[k] == min)
                {
                  minRPos = k;
                }
              }
              bounds.Add((minLPos + minRPos) / 2);
            }

            bounds.Add(nRightPos);
            lastLeftJustifiedPreliminaryRightBoundary = null;
          }
          else if (leftincrement > rightdecrement && leftincrement > nThresh) // left justified
          {
            if (lastLeftJustifiedPreliminaryRightBoundary.HasValue) // last field was left justified, this field is left justified -> use this field's left boundary for the boundary between previous field and this field
              bounds.Add(nLeftPos);
            lastLeftJustifiedPreliminaryRightBoundary = nRightPos; // and store this fields preliminary right boundary
          }
          else // if both is not the case, this is a strong indicator that we don't have fixed positions
          {
            unsharpBoundFields++;
          }

          nPos = nRightPos;
        }
        else
        {
          nPos++;
        }
      }

      // calculate score of result
      qualityScore = 0;
      for (int i = 0; i < bounds.Count; ++i)
      {
        var pos = bounds[i];
        qualityScore += startOfWords[pos];
        qualityScore += endOfWords[pos];
        qualityScore -= deepInWords[pos];
      }

      return bounds;
    }

    /// <summary>
    /// Return the maximal tabbed line length of the provided list of tabbed word bounds.
    /// </summary>
    /// <param name="listOfTabbedWordBounds"></param>
    /// <returns></returns>
    private int MaxTabbedLineLength(List<List<int>> listOfTabbedWordBounds)
    {
      int maxlength = 0;
      foreach (List<int> list in listOfTabbedWordBounds)
        if (list.Count > 0)
          maxlength = Math.Max(maxlength, list[list.Count - 1]);

      return maxlength;
    }

    /// <summary>
    /// When a start of word is detected, the counterArray is incremented at this tabbed position by one.
    /// This is done for every word in all lines, meaning that if a word starts in all N lines at the same position,
    /// the counterArray is incremented at this position by N.
    /// </summary>
    /// <param name="listOfTabbedWordBounds">List of tabbed word bounds (for each line one tabbed words bound list).</param>
    /// <param name="counterArray">Integer array with the line counts for each tabbed position.</param>
    private void ReportStartOfWords(List<List<int>> listOfTabbedWordBounds, int[] counterArray)
    {
      foreach (List<int> list in listOfTabbedWordBounds)
      {
        for (int i = 0; i < list.Count; i += 2)
        {
          counterArray[list[i]]++;
        }
      }
    }

    /// <summary>
    /// When a end of word is detected, the counterArray is incremented at this tabbed position by one.
    /// This is done for every word in all lines, meaning that if a word ends in all N lines at the same position,
    /// the counterArray is incremented at this position by N.
    /// </summary>
    /// <param name="listOfTabbedWordBounds">List of tabbed word bounds (for each line one tabbed words bound list).</param>
    /// <param name="counterArray">Integer array with the line counts for each tabbed position.</param>
    private void ReportEndOfWords(List<List<int>> listOfTabbedWordBounds, int[] counterArray)
    {
      foreach (List<int> list in listOfTabbedWordBounds)
      {
        for (int i = 1; i < list.Count; i += 2)
        {
          counterArray[list[i]]++;
        }
      }
    }

    /// <summary>
    /// When our position is just inside of a word, the counterArray is incremented at this tabbed position by one.
    /// This is done for every position in all lines, meaning that if a word is present in all N lines at the same position,
    /// the counterArray is incremented at this position by N.
    /// </summary>
    /// <param name="listOfTabbedWordBounds">List of tabbed word bounds (for each line one tabbed words bound list).</param>
    /// <param name="counterArray">Integer array with the line counts for each tabbed position.</param>
    private void ReportInsideOfWords(List<List<int>> listOfTabbedWordBounds, int[] counterArray)
    {
      foreach (List<int> list in listOfTabbedWordBounds)
      {
        for (int i = 0; i < list.Count; i += 2)
        {
          int start = list[i];
          int end = list[i + 1];

          for (int k = start; k < end; k++)
            counterArray[k]++;
        }
      }
    }

    /// <summary>
    /// When our position is just deep* inside of a word, the counterArray is incremented at this tabbed position by one.
    /// This is done for every position in all lines, meaning that if a word is present in all N lines at the same position,
    /// the counterArray is incremented at this position by N.
    /// *Deep mean the position is neither on the start of the word nor at the end of the word.
    /// </summary>
    /// <param name="listOfTabbedWordBounds">List of tabbed word bounds (for each line one tabbed words bound list).</param>
    /// <param name="counterArray">Integer array with the line counts for each tabbed position.</param>
    private void ReportDeepInsideOfWords(List<List<int>> listOfTabbedWordBounds, int[] counterArray)
    {
      foreach (List<int> list in listOfTabbedWordBounds)
      {
        for (int i = 0; i < list.Count; i += 2)
        {
          int start = list[i];
          int end = list[i + 1];

          for (int k = start + 1; k < (end - 1); k++)
            counterArray[k]++;
        }
      }
    }

    /// <summary>
    /// Gets the tabbed position for the start and end of all words in a line, assuming a certain tab width.
    /// </summary>
    /// <param name="sLine">Line to analyse.</param>
    /// <param name="stringPositions"></param>
    /// <param name="tabSize"></param>
    /// <returns>List of positions of the word starts and ends (ends=next position after the word). The list therefore always has
    /// an even number of members. If there is a word, the list starts always with the start position of the word, followed by the position of the first character after this word.</returns>
    public List<int> GetTabbedWordBounds(string sLine, List<int> stringPositions, int tabSize)
    {
      List<int> result = new List<int>();

      int tabPos = 0;
      int listPos = 0;
      for (int i = 0; i < sLine.Length && listPos < stringPositions.Count;)
      {
        if (i == stringPositions[listPos]) // we have hitted a word
        {
          int nxt = stringPositions[listPos + 1];
          listPos += 2;
          result.Add(tabPos);
          tabPos += nxt - i;
          result.Add(tabPos);
          i = nxt;
        }
        else // it is not a word
        {
          char s = sLine[i];
          if (' ' == s) // Space increments tab position by 1
            tabPos++;
          else if ('\t' == s)
            tabPos += tabSize - (tabPos % tabSize); // Tab increments tab position depending on the tab size, but also depending on the current tab position

          i++;
        }
      }
      return result;
    }

    /// <summary>
    /// Gets the string position for the start and end of all words in a line.
    /// </summary>
    /// <param name="sLine">Line to analyse.</param>
    /// <returns>List of positions of the word starts and ends (ends=next position after the word). The list therefore always has
    /// an even number of members. If there is a word, the list starts always with the start position of the word, followed by the position of the first character after this word.</returns>
    public List<int> GetStringWordBounds(string sLine)
    {
      // foreach line, we find the end of the words
      List<int> wordBounds = new List<int>();

      int nLen = sLine.Length;
      if (nLen == 0)
        return wordBounds;

      bool bInWord = false;
      bool bInString = false; // true when starting with " char
      for (int i = 0; i < nLen; i++)
      {
        char cc = sLine[i];
        bool wasInWordBefore = bInWord;

        if (cc == '\t' || cc == ' ')
        {
          bInWord &= bInString;
        }
        else if (cc == '\"')
        {
          bInWord = !bInWord;
          bInString = !bInString;
        }
        else if (cc > ' ') // all other chars are no space chars
        {
          if (!bInWord)
            bInWord = true;
        }

        if (bInWord != wasInWordBefore)
          wordBounds.Add(i);
      }

      if (bInWord)
        wordBounds.Add(nLen);

      if (!(wordBounds.Count % 2 == 0))
        throw new InvalidProgramException(); // must be even

      return wordBounds;
    }
  }
}
