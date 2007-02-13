#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
using System.Text;

namespace Altaxo.Serialization.Ascii
{
  public class AsciiSeparationAnalyzer
  {
    /// <summary>True if any of the lines contains a tabulator char.</summary>
    bool _containsTabs;

    /// <summary>True if any of the lines contains a space char.</summary>
    bool _containsSpaces;

    /// <summary>True if any of the lines contains more than one whitespace successively.</summary>
    bool _containsRepeatingWhiteSpaces;

    List<int> _fixedBoundaries;

    int _recognizedTabSize;

    public AsciiSeparationAnalyzer(List<string> firstLines)
    {
      _containsTabs = ContainsTabs(firstLines);
      _containsSpaces = ContainsSpaces(firstLines);
      _containsRepeatingWhiteSpaces = ContainsRepeatingWhiteSpaces(firstLines);

      // if the document contains repeating white spaces, it is a good candidate for
      // fixed column width structure

      if (_containsRepeatingWhiteSpaces)
      {
        List<List<int>> listOfStringWordBounds = new List<List<int>>();
        foreach (string s in firstLines)
          listOfStringWordBounds.Add(GetStringWordBounds(s));

        if (!_containsTabs)
        {
          _fixedBoundaries = FindCommonWordBoundaries(listOfStringWordBounds);
        }
        else
        {
          List<List<int>> listOfTabbedWordBounds = new List<List<int>>();
          for (int i = 0; i < firstLines.Count; i++)
            listOfTabbedWordBounds.Add(GetTabbedWordBounds(firstLines[i], listOfStringWordBounds[i], 4));
          List<int> bounds4 = FindCommonWordBoundaries(listOfTabbedWordBounds);

          listOfTabbedWordBounds.Clear();
          for (int i = 0; i < firstLines.Count; i++)
            listOfTabbedWordBounds.Add(GetTabbedWordBounds(firstLines[i], listOfStringWordBounds[i], 8));
          List<int> bounds8 = FindCommonWordBoundaries(listOfTabbedWordBounds);

          _fixedBoundaries = (bounds4.Count > bounds8.Count) ? bounds4 : bounds8;
          _recognizedTabSize = (bounds4.Count > bounds8.Count) ? 4 : 8;
        }
      }
    }

    /// <summary>
    /// Tests if any of the lines contains a tabulator char.
    /// </summary>
    /// <param name="lines">The lines to test.</param>
    /// <returns>True if any of the lines contains a tabulator char.</returns>
    public static bool ContainsTabs(List<string> lines)
    {
      foreach (string s in lines)
        if (s.IndexOf('\t') >= 0)
          return true;

      return false;
    }

    /// <summary>
    /// Tests if any of the lines contains a space char.
    /// </summary>
    /// <param name="lines">The lines to test.</param>
    /// <returns>True if any of the lines contains a space char.</returns>
    public static bool ContainsSpaces(List<string> lines)
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

    public static bool ContainsRepeatingWhiteSpaces(List<string> lines)
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
    /// Finds common word boundaries.
    /// </summary>
    /// <param name="listOfTabbedWordBounds"></param>
    /// <returns></returns>
    List<int> FindCommonWordBoundaries(List<List<int>> listOfTabbedWordBounds)
    {
      double relThreshold = 0.5;
      int maxlen = MaxTabbedLineLength(listOfTabbedWordBounds);
      int[] inWords = new int[maxlen];
      int[] startOfWords = new int[maxlen];
      int[] endOfWords = new int[maxlen];

      ReportInsideOfWords(listOfTabbedWordBounds, inWords);
      ReportStartOfWords(listOfTabbedWordBounds, startOfWords);
      ReportEndOfWords(listOfTabbedWordBounds, endOfWords);

      List<int> bounds = new List<int>();
      bool lastLeftJustified = false;
      int unsharpBoundFields = 0;

      // now search for significant boundaries and if left or right-justified
      int nThresh = (int)(relThreshold * listOfTabbedWordBounds.Count);

      for (int nPos = 0; nPos < maxlen; )
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

          // if rightDecrement is higher than leftincrement, then the words are rightjustified
          // if not they are leftjustified

          if (rightdecrement > leftincrement && rightdecrement > nThresh)
          {
            if (true == lastLeftJustified && bounds.Count > 0)
            {
              // we have to find the boundary between the last left-justified field and 
              // the right justified field now, best in my opinion is the minimum of the word count
              int min = int.MaxValue;
              int minPos = nPos;
              for (int k = bounds[bounds.Count - 1]; k < nRightPos; k++)
              {
                if (inWords[k] < min)
                {
                  min = inWords[k];
                  minPos = k;
                }
              }
              bounds.Add(minPos);
            }
            bounds.Add(nRightPos);
            lastLeftJustified = false;
          }
          else if (leftincrement > rightdecrement && leftincrement > nThresh) // left justified
          {
            if (lastLeftJustified == true)
              bounds.Add(nLeftPos);
            lastLeftJustified = true;
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
      return bounds;
    }

    int MaxTabbedLineLength(List<List<int>> listOfTabbedWordBounds)
    {
      int maxlength = 0;
      foreach (List<int> list in listOfTabbedWordBounds)
        if (list.Count > 0)
          maxlength = Math.Max(maxlength, list[list.Count - 1]);

      return maxlength;
    }

    void ReportStartOfWords(List<List<int>> listOfTabbedWordBounds, int[] counterArray)
    {
      foreach (List<int> list in listOfTabbedWordBounds)
      {
        for (int i = 0; i < list.Count; i += 2)
        {
          counterArray[list[i]]++;
        }
      }
    }

    void ReportEndOfWords(List<List<int>> listOfTabbedWordBounds, int[] counterArray)
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
    /// Returns an array, where for each screen position (tabbed position) the number of lines is counted,
    /// where we are inside a word.
    /// </summary>
    /// <param name="listOfTabbedWordBounds">List of tabbed word bounds (for each line one tabbed words bound list).</param>
    /// <returns>Integer array with the line counts for each tabbed position.</returns>
    void ReportInsideOfWords(List<List<int>> listOfTabbedWordBounds, int[] counterArray)
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
    /// Gets the tabbed position for the start and end of all words in a line, assuming a certain tab width.
    /// </summary>
    /// <param name="sLine">Line to analyse.</param>
    /// <returns>List of positions of the word starts and ends (ends=next position after the word). The list therefore always has
    /// an even number of members. If there is a word, the list starts always with the start position of the word, followed by the position of the first character after this word.</returns>
    public List<int> GetTabbedWordBounds(string sLine, List<int> stringPositions, int tabSize)
    {
      List<int> result = new List<int>();

      int tabPos = 0;
      int stringPos = 0;
      int listPos = 0;
      for (int i = 0; i < sLine.Length && listPos<stringPositions.Count; )
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
          tabPos += tabSize-(tabPos%tabSize);
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

        if (cc == '\t' || cc==' ')
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

      System.Diagnostics.Debug.Assert(wordBounds.Count % 2 == 0); // must be even

      return wordBounds;
    }


  }
}
