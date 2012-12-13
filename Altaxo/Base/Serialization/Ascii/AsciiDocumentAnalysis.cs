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
using System.Text;
using System.Collections.Generic;
using System.Linq;

using Altaxo.Data;

namespace Altaxo.Serialization.Ascii
{

	/// <summary>
	/// Supports the analysis of Ascii files.
	/// </summary>
	public class AsciiDocumentAnalysis
	{
		public const int DefaultNumberOfLinesToAnalyze = 30;


		/// <summary>If the number of main header lines is known before this analysis, this list contains the main header lines.</summary>
		List<string> _headerLines;

		/// <summary>First lines of the document to analyze. Note that this not neccessarily have to be the first lines. The line number of the first line in this collection is designated by <see cref="_headerLines.Count"/>.</summary>
		List<string> _bodyLines;


		/// <summary>Global structure of the document.</summary>
		AsciiGlobalStructureAnalysis _globalStructure;

		/// <summary>List of all useful combinations of SeparationStrategy, NumberFormat and DateTimeFormat that should be tested.</summary>
		List<AsciiLineAnalysisOption> _lineAnalysisOptionsToTest;

		List<AsciiLineAnalysis> _lineAnalysisOfHeaderLines;

		List<AsciiLineAnalysis> _lineAnalysisOfBodyLines;

		Dictionary<AsciiLineAnalysisOption, NumberAndStructure> _lineAnalysisOptionsScoring;

		AsciiLineAnalysisOption _highestScoredLineAnalysisOption;
		AsciiLineStructure _highestScoredLineStructure;

		int _numberOfMainHeaderLines;

		int _indexOfCaptionLine;

		private AsciiDocumentAnalysis() { }


			/// <summary>
		/// Analyzes the first <code>nLines</code> of the ascii stream.
		/// </summary>
		/// <param name="importOptions">The import options. Some of the field can already be filled with useful values. Since it is not neccessary to determine the value of those known fields, the analysis will be run faster then.</param>
		/// <param name="stream">The ascii stream to analyze.</param>
		/// <returns>Import options that can be used in a following step to read in the ascii stream. If the stream contains no data, the returned import options will be not fully specified.
		/// The same instance is returned as given by the parameter <paramref name="importOptions"/>. If <paramref name="importOptions"/> was <c>null</c>, a new instance is created.</returns>
		public static AsciiImportOptions Analyze(AsciiImportOptions importOptions, System.IO.Stream stream)
		{
			return Analyze(importOptions, stream, AsciiDocumentAnalysisOptions.DefaultInstance);
		}

		/// <summary>
		/// Analyzes the first <code>nLines</code> of the ascii stream.
		/// </summary>
		/// <param name="importOptions">The import options. Some of the field can already be filled with useful values. Since it is not neccessary to determine the value of those known fields, the analysis will be run faster then.</param>
		/// <param name="stream">The ascii stream to analyze.</param>
		/// <param name="analysisOptions">Options that specify how many lines are analyzed, and what number formats and date/time formats will be tested.</param>
		/// <returns>Import options that can be used in a following step to read in the ascii stream. If the stream contains no data, the returned import options will be not fully specified.
		/// The same instance is returned as given by the parameter <paramref name="importOptions"/>. If <paramref name="importOptions"/> was <c>null</c>, a new instance is created.</returns>
		public static AsciiImportOptions Analyze(AsciiImportOptions importOptions, System.IO.Stream stream, AsciiDocumentAnalysisOptions analysisOptions)
		{
			if (importOptions == null)
				importOptions = new AsciiImportOptions();

			var analysis = new AsciiDocumentAnalysis();

			analysis.InternalAnalyze(importOptions, stream, analysisOptions);
			return importOptions;
		}
		/// <summary>
		/// Analyzes the first <code>nLines</code> of the ascii stream.
		/// </summary>
		/// <param name="importOptions">The import options. This can already contain known values. On return, this instance should be ready to be used to import ascii data, i.e. all fields should contain values unequal to <c>null</c>.</param>
		/// <param name="stream">The ascii stream to analyze.</param>
		/// <param name="analysisOptions">Options that specify how many lines are analyzed, and what number formats and date/time formats will be tested.</param>
		public void InternalAnalyze(AsciiImportOptions importOptions, System.IO.Stream stream, AsciiDocumentAnalysisOptions analysisOptions)
		{
			if (null == stream)
				throw new ArgumentNullException("Stream");
			if (null == analysisOptions)
				throw new ArgumentNullException("analysisOptions");
			if (null == importOptions)
				throw new ArgumentNullException("importOptions");


			// Read-in the lines into _bodyLines. If the number of header lines is already known, those header lines are read into _headerLines
			ReadLinesToAnalyze(stream, analysisOptions.NumberOfLinesToAnalyze, importOptions.NumberOfMainHeaderLines);

			if (_bodyLines.Count == 0)
				return; // there is nothing to analyze

			// Analyze the whitespace structure of the body lines, find out if there is a fixed column width
			_globalStructure = new AsciiGlobalStructureAnalysis(_bodyLines);

			// Sets all separation strategies to test for. If importOptions already contain a separation strategy, only this separation strategy is set
			SetLineAnalysisOptionsToTest(importOptions, analysisOptions);

			// Analyze each of the first few lines with all possible separation strategies
			_lineAnalysisOfBodyLines = new List<AsciiLineAnalysis>();
			for (int i = 0; i < _bodyLines.Count; i++)
				_lineAnalysisOfBodyLines.Add(new AsciiLineAnalysis(i, _bodyLines[i], _lineAnalysisOptionsToTest));

			// for debugging activate the next line and paste the data into notepad:
			// PutRecognizedStructuresToClipboard(result, separationStrategies);

			EvaluateScoringOfAllLineAnalysisOptions();

			// Evaluate the best separation strategy. Store the value in _highestScoredSeparationStrategy and the corresponding line structure in _highestScoredLineStructure;
			EvaluateHighestScoredLineAnalysisOption();

			// look how many header lines are in the file by comparing the structure of the first lines  with the _highestScoredLineStructure
			if (null == importOptions.NumberOfMainHeaderLines)
				EvaluateNumberOfMainHeaderLines();
			else
				_numberOfMainHeaderLines = importOptions.NumberOfMainHeaderLines.Value;


			// get the index of the caption line
			if (null == importOptions.IndexOfCaptionLine)
				EvaluateIndexOfCaptionLine();
			else
				_indexOfCaptionLine = importOptions.IndexOfCaptionLine.Value;

			importOptions.NumberOfMainHeaderLines = _numberOfMainHeaderLines;
			importOptions.IndexOfCaptionLine = _indexOfCaptionLine;

			importOptions.SeparationStrategy = _highestScoredLineAnalysisOption.SeparationStrategy;
			importOptions.NumberFormatCulture = _highestScoredLineAnalysisOption.NumberFormat;
			importOptions.DateTimeFormatCulture = _highestScoredLineAnalysisOption.DateTimeFormat;

			importOptions.RecognizedStructure = _lineAnalysisOptionsScoring[_highestScoredLineAnalysisOption].LineStructure;
		}

		private void ReadLinesToAnalyze(System.IO.Stream stream, int numberOfLinesToAnalyze, int? numberOfMainHeaderLines)
		{
			string sLine;

			stream.Position = 0;
			System.IO.StreamReader sr = new System.IO.StreamReader(stream, System.Text.Encoding.Default, true);

			bool reachingEOF = false;
			_headerLines = new List<string>();
			_bodyLines = new List<string>();
			if (numberOfMainHeaderLines.HasValue)
			{
				int numHeaderLines = numberOfMainHeaderLines.Value;
				for (int i = 0; i < numHeaderLines; ++i)
				{
					sLine = sr.ReadLine();
					if (null == sLine)
					{
						reachingEOF = true;
						break;
					}
					_headerLines.Add(sLine);
				}
			}

			if (!reachingEOF)
			{
				for (int i = 0; i < numberOfLinesToAnalyze; i++)
				{
					sLine = sr.ReadLine();
					if (null == sLine)
						break;
					_bodyLines.Add(sLine);
				}
			}
		}


		private void SetLineAnalysisOptionsToTest(AsciiImportOptions importOptions, AsciiDocumentAnalysisOptions analysisOptions)
		{
			var numberFormatsToTest = new List<System.Globalization.CultureInfo>();
			var dateTimeFormatsToTest = new List<System.Globalization.CultureInfo>();
			var separationStrategiesToTest = new List<IAsciiSeparationStrategy>();

			// all number formats to test
			if (null != importOptions.NumberFormatCulture)
			{
				numberFormatsToTest.Add( importOptions.NumberFormatCulture );
			}
			else
			{
				numberFormatsToTest.AddRange(analysisOptions.NumberFormatsToTest);
				if(0==numberFormatsToTest.Count)
					numberFormatsToTest.Add(System.Globalization.CultureInfo.InvariantCulture);
			}

			// all DateTime formats to test
			if (null != importOptions.DateTimeFormatCulture)
			{
				dateTimeFormatsToTest.Add(importOptions.DateTimeFormatCulture);
			}
			else
			{
				dateTimeFormatsToTest.AddRange(analysisOptions.DateTimeFormatsToTest);
				if(0==dateTimeFormatsToTest.Count)
					dateTimeFormatsToTest.Add(System.Globalization.CultureInfo.InvariantCulture);
			}

			// all separation strategies to test
			if (importOptions.SeparationStrategy != null) // if a separation strategy is given use only this
			{
				separationStrategiesToTest.Add(importOptions.SeparationStrategy);
			}
			else // no separation strategy given - we include the possible strategies here
			{
				if (_globalStructure.ContainsTabs)
					separationStrategiesToTest.Add(new SingleCharSeparationStrategy('\t'));
				if (_globalStructure.ContainsCommas)
					separationStrategiesToTest.Add(new SingleCharSeparationStrategy(','));
				if (_globalStructure.ContainsSemicolons)
					separationStrategiesToTest.Add(new SingleCharSeparationStrategy(';'));
				if (_globalStructure.FixedBoundaries != null)
				{
					if (_globalStructure.RecognizedTabSize == 1)
						separationStrategiesToTest.Add(new FixedColumnWidthWithoutTabSeparationStrategy(_globalStructure.FixedBoundaries));
					else
						separationStrategiesToTest.Add(new FixedColumnWidthWithTabSeparationStrategy(_globalStructure.FixedBoundaries, _globalStructure.RecognizedTabSize));
				}
				if (separationStrategiesToTest.Count == 0)
					separationStrategiesToTest.Add(new SkipWhiteSpaceSeparationStrategy());
			}

			// make a full outer join of all three categories
			var optionsToTest = new HashSet<AsciiLineAnalysisOption>();
			foreach (var s in separationStrategiesToTest)
				foreach (var n in numberFormatsToTest)
					foreach (var d in dateTimeFormatsToTest)
						optionsToTest.Add(new AsciiLineAnalysisOption(s, n, d));


			// remove all those keys where the char of the single char separation strategy is equal to the number format's decimal separator
			foreach (AsciiLineAnalysisOption k in optionsToTest.ToArray())
			{
				if (
					(k.SeparationStrategy is SingleCharSeparationStrategy) &&
					(((SingleCharSeparationStrategy)k.SeparationStrategy).SeparatorChar.ToString() == k.NumberFormat.NumberFormat.NumberDecimalSeparator)
					)
					optionsToTest.Remove(k);
			}

			_lineAnalysisOptionsToTest = new List<AsciiLineAnalysisOption>(optionsToTest);

		}

		/// <summary>
		/// Calculates the scoring of all separation strategies to test (in <see cref="_lineAnalysisOptionsToTest"/>),
		/// and puts the number of common lines and the line structure in the dictionary <see cref="_lineAnalysisOptionsScoring"/>.
		/// </summary>
		private void EvaluateScoringOfAllLineAnalysisOptions()
		{
			_lineAnalysisOptionsScoring = new Dictionary<AsciiLineAnalysisOption, NumberAndStructure>();

			// for each of the separation strategies, determine the maximum number of equal lines and the line with the highest score among the equal lines
			foreach (var analysisOption in _lineAnalysisOptionsToTest)
			{
				int maxNumberOfEqualLines;
				AsciiLineStructure mostFrequentLineStructure;
				CalculateScoreOfLineAnalysisOption(analysisOption, _lineAnalysisOfBodyLines, out maxNumberOfEqualLines, out mostFrequentLineStructure);
				if (null != mostFrequentLineStructure)
					_lineAnalysisOptionsScoring.Add(analysisOption, new NumberAndStructure() { NumberOfLines = maxNumberOfEqualLines, LineStructure = mostFrequentLineStructure });
			}
		}


		/// <summary>
		/// Evaluates the highest scored separation strategy, and stores the winning separation strategy in <see cref="_highestScoredLineAnalysisOption"/> and the corresponding line structure in <see cref="_highestScoredLineStructure"/>.
		/// </summary>
		private void EvaluateHighestScoredLineAnalysisOption()
		{
			// determine, which of the separation strategies results in the topmost total priority (product of number of lines and best line priority)
			double maxScore = int.MinValue;
			var maxScoredEntry = _lineAnalysisOptionsScoring.First();
			foreach (var entry in _lineAnalysisOptionsScoring)
			{
				double score = (double)entry.Value.NumberOfLines * entry.Value.LineStructure.LineStructureScoring;

				if (score > maxScore)
				{
					maxScore = score;
					maxScoredEntry = entry;
				}
				else if (score == maxScore && entry.Value.NumberOfLines > maxScoredEntry.Value.NumberOfLines)
				{
					maxScoredEntry = entry;
				}
			}
			_highestScoredLineAnalysisOption = maxScoredEntry.Key;
			_highestScoredLineStructure = maxScoredEntry.Value.LineStructure;
		}




		/// <summary>
		/// Evaluates the number of main header lines. The presumtion is here, that the number of main header lines was not known before.
		/// </summary>
		private void EvaluateNumberOfMainHeaderLines()
		{
			for (int i = 0; i < _lineAnalysisOfBodyLines.Count; i++)
			{
				if (_lineAnalysisOfBodyLines[i][_highestScoredLineAnalysisOption].IsCompatibleWith(_highestScoredLineStructure))
				{
					_numberOfMainHeaderLines = i;
					break;
				}
			}
		}

		/// <summary>
		/// Evaluates the index of caption line. If the caption line could not be recognized, <see cref="_indexOfCaptionLine"/> is set to -1.
		/// </summary>
		private void EvaluateIndexOfCaptionLine()
		{
			// try to guess which of the header lines is the caption line
			// we take the caption line to be the first column which has the same number of tokens as the recognized structure
			// if no line fulfilles this criteria, the IndexOfCaptionLine remain unchanged.
			_indexOfCaptionLine = -1; // no caption by default
			if (0 == _numberOfMainHeaderLines)
				return; // if we have no main header lines, we have no caption

			// here we have two cases: 
			// either the number of header lines was not known before (_headerLines.Count is zero, but _numberOfMainHeaderLines is greater than zero): here we have already analyzed the lines with different separation strategies
			// or the number of header lines was known before (then the _headerLines list contains the header lines) : here we need to analyze the header lines, but here only with the best separation strategy

			if (_headerLines.Count == 0) // number of header lies was not known before
			{
				for (int i = 0; i < _numberOfMainHeaderLines; i++)
				{
					if (_lineAnalysisOfBodyLines[i][_highestScoredLineAnalysisOption].Count == _highestScoredLineStructure.Count)
					{
						_indexOfCaptionLine = i;
						break;
					}
				}
			}
			else // number of header lines was known before. Thus we have to analyze those lines, but only with the best separation strategy
			{
				_lineAnalysisOfHeaderLines = new List<AsciiLineAnalysis>();
				var separationStrategiesToEvaluate = new List<AsciiLineAnalysisOption>();
				separationStrategiesToEvaluate.Add(_highestScoredLineAnalysisOption);

				for (int i = 0; i < _headerLines.Count; i++)
					_lineAnalysisOfHeaderLines.Add(new AsciiLineAnalysis(i, _headerLines[i], separationStrategiesToEvaluate));

				for (int i = 0; i < _numberOfMainHeaderLines; i++)
				{
					if (_lineAnalysisOfHeaderLines[i][_highestScoredLineAnalysisOption].Count == _highestScoredLineStructure.Count)
					{
						_indexOfCaptionLine = i;
						break;
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
		public static void CalculateScoreOfLineAnalysisOption(AsciiLineAnalysisOption analysisOption, List<AsciiLineAnalysis> result, out int maxNumberOfEqualLines, out AsciiLineStructure bestLine)
		{
			CalculateScoreOfLineAnalysisOption(analysisOption, result, null, out maxNumberOfEqualLines, out bestLine);
		}

		/// <summary>
		/// Determines, which lines are the most fr
		/// </summary>
		/// <param name="analysisOption"></param>
		/// <param name="result"></param>
		/// <param name="excludeLineStructureHashes"></param>
		/// <param name="maxNumberOfEqualLines"></param>
		/// <param name="bestLine"></param>
		public static void CalculateScoreOfLineAnalysisOption(AsciiLineAnalysisOption analysisOption, List<AsciiLineAnalysis> result, HashSet<int> excludeLineStructureHashes, out int maxNumberOfEqualLines, out AsciiLineStructure bestLine)
		{
			// Dictionary, Key is the hash of the line structure hash, Value is the number of lines that have this hash
			Dictionary<int, int> numberOfLinesForLineStructureHash = new Dictionary<int, int>();

			bestLine = null;
			for (int i = 0; i < result.Count; i++)
			{
				AsciiLineAnalysis lineResults = result[i];
				int lineStructureHash = lineResults[analysisOption].GetHashCode(); // and hash code
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

				if (null != excludeLineStructureHashes && excludeLineStructureHashes.Contains(lineStructureHash))
					continue;

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
				if (hashOfMostFrequentStructure == lineResults[analysisOption].GetHashCode())
				{
					int prty = lineResults[analysisOption].LineStructureScoring;
					if (prty >= maxPriorityOfMostFrequentLines)
					{
						maxPriorityOfMostFrequentLines = prty;
						bestLine = lineResults[analysisOption];
					}
				}// if
			} // for

			// if the bestLine is a line with a column count of zero, we should use the next best line
			// we achieve this by adding the best hash to a list of excluded hashes and call the function again
			if (bestLine != null && bestLine.Count == 0)
			{
				if (null != excludeLineStructureHashes && !excludeLineStructureHashes.Contains(hashOfMostFrequentStructure))
				{
					excludeLineStructureHashes.Add(hashOfMostFrequentStructure);
					CalculateScoreOfLineAnalysisOption(analysisOption, result, excludeLineStructureHashes, out maxNumberOfEqualLines, out bestLine);
					return;
				}
				else if (null == excludeLineStructureHashes)
				{
					excludeLineStructureHashes = new HashSet<int>() { hashOfMostFrequentStructure };
					CalculateScoreOfLineAnalysisOption(analysisOption, result, excludeLineStructureHashes, out maxNumberOfEqualLines, out bestLine);
					return;
				}
			}
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
