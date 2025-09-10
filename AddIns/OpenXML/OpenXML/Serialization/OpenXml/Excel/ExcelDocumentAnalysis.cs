#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
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
using System.Linq;
using System.Text;
using Altaxo.Serialization.Ascii;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Altaxo.Serialization.OpenXml.Excel
{
  /// <summary>
  /// Supports the analysis of an Excel spreadsheet
  /// </summary>
  public class ExcelDocumentAnalysis
  {
    private List<AsciiLineComposition>? _lineAnalysisOfHeaderLines;

    private List<AsciiLineComposition>? _lineAnalysisOfBodyLines;

    private NumberOfLinesPerLineComposition _lineAnalysisOptionsScoring;

    public AsciiLineComposition? HighestScoredLineStructure { get; protected set; }

    public int NumberOfMainHeaderLines { get; protected set; }

    public int? IndexOfCaptionLine { get; protected set; }

    private ExcelDocumentAnalysis()
    {
    }

    /// <summary>
    /// Analyzes the first lines of the Excel spreadsheet.
    /// </summary>
    /// <param name="worksheetPart">The Excel worksheet to analyze</param>
    /// <param name="workbookPart">Exel workbook (needed to resolve references).</param>
    /// <param name="importOptions">The import options. Some of the field can already be filled with useful values. Since it is not neccessary to determine the value of those known fields, the analysis will be run faster then.</param>
    /// <returns>An analysis document.</returns>
    public ExcelDocumentAnalysis(WorksheetPart worksheetPart, WorkbookPart workbookPart, ExcelImportOptions importOptions)
    {
      importOptions ??= new ExcelImportOptions();
      InternalAnalyze(worksheetPart, workbookPart, importOptions);
    }

    /// <summary>
    /// Analyzes the first <code>nLines</code> of the ascii stream.
    /// </summary>
    /// <param name="importOptions">The import options. Some of the field can already be filled with useful values. Since it is not neccessary to determine the value of those known fields, the analysis will be run faster then.</param>
    /// <param name="stream">The ascii stream to analyze.</param>
    /// <param name="analysisOptions">Options that specify how many lines are analyzed, and what number formats and date/time formats will be tested.</param>
    /// <returns>Import options that can be used in a following step to read in the ascii stream. If the stream contains no data, the returned import options will be not fully specified.
    /// The same instance is returned as given by the parameter <paramref name="importOptions"/>. If <paramref name="importOptions"/> was <c>null</c>, a new instance is created.</returns>
    public static ExcelImportOptions Analyze(WorksheetPart worksheetPart, WorkbookPart workbookPart, ExcelImportOptions? importOptions)
    {
      importOptions ??= new ExcelImportOptions();
      var analysis = new ExcelDocumentAnalysis();
      analysis.InternalAnalyze(worksheetPart, workbookPart, importOptions);
      return importOptions;
    }

    /// <summary>
    /// Analyzes the first <code>nLines</code> of the ascii stream.
    /// </summary>
    /// <param name="importOptions">The import options. This can already contain known values. On return, this instance should be ready to be used to import ascii data, i.e. all fields should contain values unequal to <c>null</c>.</param>
    /// <param name="stream">The ascii stream to analyze.</param>
    /// <param name="analysisOptions">Options that specify how many lines are analyzed, and what number formats and date/time formats will be tested.</param>

    public void InternalAnalyze(WorksheetPart worksheetPart, WorkbookPart workbookPart, ExcelImportOptions importOptions)
    {
      if (importOptions is null)
        throw new ArgumentNullException(nameof(importOptions));

      var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

      // Analyze each of the first few lines with all possible separation strategies
      _lineAnalysisOfBodyLines = new List<AsciiLineComposition>();

      int numberOfLinesToSkip = importOptions.NumberOfMainHeaderLines.HasValue && importOptions.NumberOfMainHeaderLines.Value > 0 ? importOptions.NumberOfMainHeaderLines.Value : 0;

      foreach (Row row in sheetData.Elements<Row>().Skip(numberOfLinesToSkip))
      {
        _lineAnalysisOfBodyLines.Add(ExcelLineAnalysis.GetStructure(row));
      }

      (var maxNumberOfEqualLines, HighestScoredLineStructure) = CalculateScoreOfLineAnalysisOption(_lineAnalysisOfBodyLines);

      // look how many header lines are in the file by comparing the structure of the first lines  with the _highestScoredLineStructure
      if (importOptions.NumberOfMainHeaderLines is null)
        EvaluateNumberOfMainHeaderLines();
      else
        NumberOfMainHeaderLines = importOptions.NumberOfMainHeaderLines.Value;

      // get the index of the caption line
      if (importOptions.IndexOfCaptionLine is null)
        EvaluateIndexOfCaptionLine(sheetData);
      else
        IndexOfCaptionLine = importOptions.IndexOfCaptionLine.Value;
    }

    /// <summary>
    /// Evaluates the number of main header lines. The presumtion is here, that the number of main header lines was not known before.
    /// </summary>
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
    private void EvaluateIndexOfCaptionLine(SheetData sheetData)
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
        foreach (var row in sheetData.Elements<Row>())
        {
          _lineAnalysisOfHeaderLines.Add(ExcelLineAnalysis.GetStructure(row));
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
    /// Determines, which lines are the most fr
    /// </summary>
    /// <param name="analysisOption"></param>
    /// <param name="result"></param>
    /// <param name="maxNumberOfEqualLines"></param>
    /// <param name="bestLine"></param>
    public static (int MaxNumberOfEqualLines, AsciiLineComposition? LineComposition) CalculateScoreOfLineAnalysisOption(IReadOnlyList<AsciiLineComposition> result)
    {
      return CalculateScoreOfLineAnalysisOption(result, null);
    }

    /// <summary>
    /// Determines, which lines are the most fr
    /// </summary>
    /// <param name="analysisOption"></param>
    /// <param name="lines"></param>
    /// <param name="excludeLineStructureHashes"></param>
    /// <param name="maxNumberOfEqualLines"></param>
    /// <param name="bestLine"></param>
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

    private static void PutRecognizedStructuresToClipboard(IEnumerable<AsciiLineAnalysis> analysisResults, IEnumerable<AsciiLineAnalysisOption> lineAnalysisOptions)
    {
      var stb = new StringBuilder();

      foreach (var sepStrat in lineAnalysisOptions)
      {
        stb.AppendLine("Strategy " + sepStrat.ToString());
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
