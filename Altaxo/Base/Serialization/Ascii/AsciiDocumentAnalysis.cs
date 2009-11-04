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
using System.Text;
using System.Collections.Generic;

using Altaxo.Data;

namespace Altaxo.Serialization.Ascii
{

  /// <summary>
  /// Supports the analysis of Ascii files.
  /// </summary>
  public static class AsciiDocumentAnalysis
  {
    /// <summary>
    /// Analyzes the first <code>nLines</code> of the ascii stream.
    /// </summary>
    /// <param name="stream">The ascii stream to analyze.</param>
    /// <param name="defaultImportOptions">The default import options.</param>
    /// <returns>Import options that can be used in a following step to read in the ascii stream. Null is returned if the stream contains no data.</returns>
    public static AsciiImportOptions Analyze(System.IO.Stream stream, AsciiImportOptions defaultImportOptions)
    {

      string sLine;

      int nLines = defaultImportOptions.NumberOfLinesToAnalyze;

      stream.Position = 0;
      System.IO.StreamReader sr = new System.IO.StreamReader(stream, System.Text.Encoding.Default, true);
      List<AsciiLineAnalysis> result = new List<AsciiLineAnalysis>();

      List<string> firstFewLines = new List<string>();
      for (int i = 0; i < nLines; i++)
      {
        sLine = sr.ReadLine();
        if (null == sLine)
          break;
        firstFewLines.Add(sLine);
      }
      if (firstFewLines.Count == 0)
        return null; // there is no line to analyze

      // Analyze the whitespace structure of the lines first, find out if there is a fixed column width
      AsciiGlobalStructureAnalysis globalStructure = new AsciiGlobalStructureAnalysis(firstFewLines);
      List<IAsciiSeparationStrategy> separationStrategies = new List<IAsciiSeparationStrategy>();

      if (defaultImportOptions.SeparationStrategy != null) // if a separation strategy is given use only this
      {
        separationStrategies.Add(defaultImportOptions.SeparationStrategy);
      }
      else // no separation strategy given - we include the possible strategies here
      {
        if (globalStructure.ContainsTabs)
          separationStrategies.Add(new SingleCharSeparationStrategy('\t'));
        if (globalStructure.ContainsCommas)
          separationStrategies.Add(new SingleCharSeparationStrategy(','));
        if (globalStructure.ContainsSemicolons)
          separationStrategies.Add(new SingleCharSeparationStrategy(';'));
        if (globalStructure.FixedBoundaries != null)
        {
          if (globalStructure.RecognizedTabSize == 1)
            separationStrategies.Add(new FixedColumnWidthWithoutTabSeparationStrategy(globalStructure.FixedBoundaries));
          else
            separationStrategies.Add(new FixedColumnWidthWithTabSeparationStrategy(globalStructure.FixedBoundaries, globalStructure.RecognizedTabSize));
        }
        if (separationStrategies.Count == 0)
          separationStrategies.Add(new SkipWhiteSpaceSeparationStrategy());
      }




      for (int i = 0; i < firstFewLines.Count; i++)
        result.Add(new AsciiLineAnalysis(i, firstFewLines[i], separationStrategies));

      if (result.Count == 0)
        return null; // there is nothing to analyze



      // for debugging activate the next line and paste the data into notepad:
      // PutRecognizedStructuresToClipboard(result, separationStrategies);



      var numEqLinesAndStructForSeparationStrategy = new Dictionary<IAsciiSeparationStrategy, NumberAndStructure>();


      // for each of the separation strategies, determine the maximum number of equal lines and the line with the best priority among the equal lines
      foreach (IAsciiSeparationStrategy strat in separationStrategies)
      {
        int maxNumberOfEqualLines;
        AsciiLineStructure mostFrequentLineStructure;
        GetPriorityOf(result, strat, out maxNumberOfEqualLines, out mostFrequentLineStructure);
        numEqLinesAndStructForSeparationStrategy.Add(strat, new NumberAndStructure() { NumberOfLines = maxNumberOfEqualLines, LineStructure = mostFrequentLineStructure });
      }

      // determine, which of the separation strategies results in the topmost total priority (product of number of lines and best line priority)
      int nMaxLines = int.MinValue;
      double maxprtylines = 0;
      IAsciiSeparationStrategy bestSeparationStragegy = null;
      foreach (var entry in numEqLinesAndStructForSeparationStrategy)
      {
        double prtylines = (double)entry.Value.NumberOfLines * entry.Value.LineStructure.Priority;
        if (prtylines == maxprtylines)
        {
          if (entry.Value.NumberOfLines > nMaxLines)
          {
            nMaxLines = entry.Value.NumberOfLines;
            bestSeparationStragegy = entry.Key;
          }
        }
        else if (prtylines > maxprtylines)
        {
          maxprtylines = prtylines;
          bestSeparationStragegy = entry.Key;
          nMaxLines = entry.Value.NumberOfLines;
        }
      }



      AsciiImportOptions opt = defaultImportOptions.Clone();

      opt.SeparationStrategy = bestSeparationStragegy;
      opt.RecognizedStructure = numEqLinesAndStructForSeparationStrategy[bestSeparationStragegy].LineStructure;


      // look how many header lines are in the file by comparing the structure of the first lines  with the recognized structure
      for (int i = 0; i < result.Count; i++)
      {
        opt.NumberOfMainHeaderLines = i;
        if (result[i][bestSeparationStragegy].IsCompatibleWith(opt.RecognizedStructure))
          break;
      }

      // try to guess which of the header lines is the caption line
      // we take the caption line to be the first column which has the same number of tokens as the recognized structure
      // if no line fulfilles this criteria, the IndexOfCaptionLine remain unchanged.
      for (int i = 0; i < result.Count; i++)
      {
        if (result[i][bestSeparationStragegy].Count == opt.RecognizedStructure.Count)
        {
          opt.IndexOfCaptionLine = i;
          break;
        }
      }

      // calculate the total statistics of decimal separators
      opt.DecimalSeparatorCommaCount = 0;
      opt.DecimalSeparatorDotCount = 0;
      for (int i = 0; i < result.Count; i++)
      {
        opt.DecimalSeparatorDotCount += result[i][bestSeparationStragegy].DecimalSeparatorDotCount;
        opt.DecimalSeparatorCommaCount += result[i][bestSeparationStragegy].DecimalSeparatorCommaCount;
      }



      return opt;

    }


  /// <summary>
  /// Determines, which lines are the most fr
  /// </summary>
  /// <param name="result"></param>
  /// <param name="sep"></param>
  /// <param name="maxNumberOfEqualLines"></param>
  /// <param name="bestLine"></param>
    public static void GetPriorityOf(List<AsciiLineAnalysis> result, IAsciiSeparationStrategy sep, 
      out int maxNumberOfEqualLines,
      out AsciiLineStructure bestLine)
    {
      // Dictionary, Key is the hash of the line structure hash, Value is the number of lines that have this hash
      Dictionary<int, int> numberOfLinesForLineStructureHash = new Dictionary<int, int>();
      
      bestLine = null;
      for (int i = 0; i < result.Count; i++)
      {
        AsciiLineAnalysis lineResults = result[i];
        int lineStructureHash = lineResults[sep].GetHashCode(); // and hash code
        if (numberOfLinesForLineStructureHash.ContainsKey(lineStructureHash))
          numberOfLinesForLineStructureHash[lineStructureHash] = 1 + numberOfLinesForLineStructureHash[lineStructureHash];
        else
          numberOfLinesForLineStructureHash.Add(lineStructureHash, 1);
      }
      
      
      // determine, which of the line structures is the most frequent one
      maxNumberOfEqualLines = 0;
      int hashOfMostFrequentStructure = 0;
      foreach (var dictEntry in numberOfLinesForLineStructureHash)
      {
        int lineStructureHash = dictEntry.Key;
        int numberOfLines = dictEntry.Value;
        if (maxNumberOfEqualLines < numberOfLines)
        {
          maxNumberOfEqualLines = numberOfLines;
          hashOfMostFrequentStructure = lineStructureHash;
        }
      } // for each


      // search for the maximum priority of those lines with the most frequent structure
      int maxPriorityOfMostFrequentLines = 0;
      for (int i = 0; i < result.Count; i++)
      {
        AsciiLineAnalysis lineResults = result[i];
        if (hashOfMostFrequentStructure == lineResults[sep].GetHashCode())
        {
          int prty = lineResults[sep].Priority;
          if (prty >= maxPriorityOfMostFrequentLines)
          {
            maxPriorityOfMostFrequentLines = prty;
            bestLine = lineResults[sep];
          }
        }// if
      } // for
    }

    private static void PutRecognizedStructuresToClipboard(List<AsciiLineAnalysis> analysisResults, IList<IAsciiSeparationStrategy> separationStrategies)
    {
      var stb = new StringBuilder();

      foreach (var sepStrat in separationStrategies)
      {
        stb.AppendLine("Stragegy " + sepStrat.ToString());
        stb.AppendLine("----------------------------------");
        foreach (var lineResult in analysisResults)
          stb.AppendLine(lineResult[sepStrat].ToString());

        stb.AppendLine();
      }

      var clip = Current.Gui.GetNewClipboardDataObject();
      clip.SetData(typeof(string), stb.ToString());
      Current.Gui.SetClipboardDataObject(clip);
    }

  }
}
