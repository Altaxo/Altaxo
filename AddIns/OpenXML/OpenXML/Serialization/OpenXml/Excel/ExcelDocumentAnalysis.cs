#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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

#nullable enable
using System;
using System.Collections.Generic;
using Altaxo.Serialization.Ascii;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Altaxo.Serialization.OpenXml.Excel
{
  /// <summary>
  /// Supports the analysis of an Excel worksheet.
  /// </summary>
  public class ExcelDocumentAnalysis
  {
    private List<AsciiLineComposition>? _lineAnalysisOfHeaderLines;

    private List<AsciiLineComposition>? _lineAnalysisOfBodyLines;

    private NumberOfLinesPerLineComposition _lineAnalysisOptionsScoring;

    /// <summary>
    /// Gets the line structure with the highest score (i.e., the most frequently occurring structure) as determined during analysis.
    /// </summary>
    public AsciiLineComposition? HighestScoredLineStructure { get; protected set; }

    /// <summary>
    /// Gets the number of main header lines.
    /// </summary>
    public int NumberOfMainHeaderLines { get; protected set; }

    /// <summary>
    /// Gets the index of the caption line (ATTENTION: zero based!) within the header lines, or <c>-1</c> if no caption line was recognized.
    /// </summary>
    public int? IndexOfCaptionLine { get; protected set; }

    private ExcelDocumentAnalysis()
    {
    }

    /// <summary>
    /// Analyzes the first lines of the Excel spreadsheet.
    /// </summary>
    /// <param name="worksheetPart">The Excel worksheet to analyze.</param>
    /// <param name="workbookPart">Excel workbook (needed to resolve references).</param>
    /// <param name="importOptions">The import options. Some of the fields can already be filled with useful values. Since it is not necessary to determine the value of those known fields, the analysis will run faster.</param>
    /// <returns>An analysis document.</returns>
    public ExcelDocumentAnalysis(List<List<Cell>> columns, ExcelImportOptions importOptions)
    {
      importOptions ??= new ExcelImportOptions();
      InternalAnalyze(columns, importOptions);
    }

    /// <summary>
    /// Analyzes an Excel worksheet and updates the provided import options.
    /// </summary>
    /// <param name="worksheetPart">The Excel worksheet to analyze.</param>
    /// <param name="workbookPart">Excel workbook (needed to resolve references).</param>
    /// <param name="importOptions">The import options to update. If <see langword="null"/>, a new instance is created.</param>
    /// <returns>The updated import options.</returns>
    public static ExcelImportOptions Analyze(List<List<Cell>> columns, ExcelImportOptions? importOptions)
    {
      importOptions ??= new ExcelImportOptions();
      var analysis = new ExcelDocumentAnalysis();
      analysis.InternalAnalyze(columns, importOptions);
      return importOptions;
    }

    /// <summary>
    /// Performs the worksheet analysis and writes results into the provided <paramref name="importOptions"/>.
    /// </summary>
    /// <param name="worksheetPart">The Excel worksheet to analyze.</param>
    /// <param name="workbookPart">Excel workbook (needed to resolve references).</param>
    /// <param name="importOptions">The import options to update.</param>

    public void InternalAnalyze(List<List<Cell>> columns, ExcelImportOptions importOptions)
    {
      if (importOptions is null)
        throw new ArgumentNullException(nameof(importOptions));

      // Analyze each of the first few lines with all possible separation strategies
      _lineAnalysisOfBodyLines = new List<AsciiLineComposition>();

      int numberOfLinesToSkip = importOptions.NumberOfMainHeaderLines.HasValue && importOptions.NumberOfMainHeaderLines.Value > 0 ? importOptions.NumberOfMainHeaderLines.Value : 0;

      for (int idxRow = numberOfLinesToSkip; idxRow < columns[0].Count; ++idxRow)
      {
        _lineAnalysisOfBodyLines.Add(ExcelRowAnalysis.GetStructure(columns, idxRow));
      }

      (var maxNumberOfEqualLines, HighestScoredLineStructure) = CalculateScoreOfLineAnalysisOption(_lineAnalysisOfBodyLines);

      // look how many header lines are in the file by comparing the structure of the first lines  with the _highestScoredLineStructure
      if (importOptions.NumberOfMainHeaderLines is null)
        EvaluateNumberOfMainHeaderLines();
      else
        NumberOfMainHeaderLines = importOptions.NumberOfMainHeaderLines.Value;

      // get the index of the caption line
      if (importOptions.IndexOfCaptionLine is null)
        EvaluateIndexOfCaptionLine(columns);
      else
        IndexOfCaptionLine = importOptions.IndexOfCaptionLine.Value;
    }

    /// <summary>
    /// Evaluates the number of main header lines.
    /// </summary>
    /// <remarks>
    /// The presumption is that the number of main header lines was not known before.
    /// </remarks>
    private void EvaluateNumberOfMainHeaderLines()
    {
      if (_lineAnalysisOfBodyLines is null || HighestScoredLineStructure is null)
        throw new InvalidProgramException();

      for (int i = 0; i < _lineAnalysisOfBodyLines.Count; i++)
      {
        if (_lineAnalysisOfBodyLines[i].IsCompatibleWith(HighestScoredLineStructure) && _lineAnalysisOfBodyLines[i].ShortCuts != new string('O', _lineAnalysisOfBodyLines[i].Count))
        {
          NumberOfMainHeaderLines = i;
          break;
        }
      }
    }

    /// <summary>
    /// Evaluates the index of caption line. If the caption line could not be recognized, <see cref="IndexOfCaptionLine"/> is set to -1.
    /// </summary>
    private void EvaluateIndexOfCaptionLine(List<List<Cell>> columns)
    {
      if (_lineAnalysisOfBodyLines is null || HighestScoredLineStructure is null)
        throw new InvalidProgramException();

      // try to guess which of the header lines is the caption line
      // we take the caption line to be the first column which has the same number of tokens as the recognized structure
      // if no line fulfilles this criteria, the IndexOfCaptionLine remain unchanged.
      IndexOfCaptionLine = -1; // no caption by default
      if (0 == NumberOfMainHeaderLines)
        return; // if we have no main header lines, we have no caption

      // here we have two cases:
      // either the number of header lines was not known before (_headerLines.Count is zero, but _numberOfMainHeaderLines is greater than zero): here we have already analyzed the lines with different separation strategies
      // or the number of header lines was known before (then the _headerLines list contains the header lines) : here we need to analyze the header lines, but here only with the best separation strategy

      if (NumberOfMainHeaderLines == 0) // number of header lines was not known before
      {
        for (int i = 0; i < NumberOfMainHeaderLines; i++)
        {
          if (_lineAnalysisOfBodyLines[i].Count == HighestScoredLineStructure.Count)
          {
            IndexOfCaptionLine = i;
            break;
          }
        }
      }
      else // number of header lines was known before. Thus we have to analyze those lines, but only with the best separation strategy
      {
        _lineAnalysisOfHeaderLines = new List<AsciiLineComposition>();
        for (int idxRow = 0; idxRow < NumberOfMainHeaderLines; ++idxRow)
        {
          _lineAnalysisOfHeaderLines.Add(ExcelRowAnalysis.GetStructure(columns, idxRow));
        }

        // preferredly, we search for a line with the same number of columns, but with every cell a text cell (shortcut 'T')
        var searchedLineStructure = new string('T', HighestScoredLineStructure.Count);
        for (int i = 0; i < NumberOfMainHeaderLines; i++)
        {
          if (_lineAnalysisOfHeaderLines[i].ShortCuts == searchedLineStructure)
          {
            IndexOfCaptionLine = i;
            break;
          }
        }

        if (-1 == IndexOfCaptionLine) // if still the caption line was not found, we search for a line with the same number of columns
        {
          for (int i = 0; i < NumberOfMainHeaderLines; i++)
          {
            if (_lineAnalysisOfHeaderLines[i].Count == HighestScoredLineStructure.Count)
            {
              IndexOfCaptionLine = i;
              break;
            }
          }
        }
      }
    }

    /// <summary>
    /// Determines the most frequent line composition.
    /// </summary>
    /// <param name="result">The analyzed line compositions.</param>
    /// <returns>
    /// A tuple containing the maximum number of equal lines and the corresponding line composition.
    /// </returns>
    public static (int MaxNumberOfEqualLines, AsciiLineComposition? LineComposition) CalculateScoreOfLineAnalysisOption(IReadOnlyList<AsciiLineComposition> result)
    {
      return CalculateScoreOfLineAnalysisOption(result, null);
    }

    /// <summary>
    /// Determines the most frequent line composition.
    /// </summary>
    /// <param name="lines">The analyzed line compositions.</param>
    /// <param name="excludeLineStructureHashes">Line compositions to exclude from consideration.</param>
    /// <returns>
    /// A tuple containing the maximum number of equal lines and the corresponding line composition.
    /// </returns>
    private static (int MaxNumberOfEqualLines, AsciiLineComposition? LineComposition) CalculateScoreOfLineAnalysisOption(IReadOnlyList<AsciiLineComposition> lines, HashSet<AsciiLineComposition>? excludeLineStructureHashes)
    {
      // Dictionary, Key is the line composition, Value is the number of lines that have this composition
      var numberOfLinesForLineStructureHash = new Dictionary<AsciiLineComposition, int>();

      for (int i = 0; i < lines.Count; i++)
      {
        var lineResults = lines[i];
        var lineComposition = lineResults;
        if (numberOfLinesForLineStructureHash.ContainsKey(lineComposition))
          numberOfLinesForLineStructureHash[lineComposition] += 1;
        else
          numberOfLinesForLineStructureHash.Add(lineComposition, 1);
      }

      // determine, which of the line structures is the most frequent one
      int maxNumberOfEqualLines = 0;
      AsciiLineComposition? bestLine = null;
      foreach (var dictEntry in numberOfLinesForLineStructureHash)
      {
        var lineComposition = dictEntry.Key;

        if (excludeLineStructureHashes is not null && excludeLineStructureHashes.Contains(lineComposition))
          continue;

        int numberOfLines = dictEntry.Value;
        if (maxNumberOfEqualLines < numberOfLines)
        {
          maxNumberOfEqualLines = numberOfLines;
          bestLine = lineComposition;
        }
      } // for each

      // if the bestLine is a line with a column count of zero, we should use the next best line
      // we achieve this by adding the best hash to a list of excluded hashes and call the function again
      if (bestLine is not null && bestLine.Count == 0)
      {
        if (excludeLineStructureHashes is not null && !excludeLineStructureHashes.Contains(bestLine))
        {
          excludeLineStructureHashes.Add(bestLine);
          return CalculateScoreOfLineAnalysisOption(lines, excludeLineStructureHashes);
        }
        else if (excludeLineStructureHashes is null)
        {
          excludeLineStructureHashes = new HashSet<AsciiLineComposition>() { bestLine };
          return CalculateScoreOfLineAnalysisOption(lines, excludeLineStructureHashes);
        }
      }

      return (maxNumberOfEqualLines, bestLine);
    }
  }
}
