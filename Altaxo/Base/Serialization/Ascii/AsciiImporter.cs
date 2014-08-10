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

using Altaxo.Data;
using Altaxo.Main;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Altaxo.Serialization.Ascii
{
	/// <summary>
	/// Central class for import of ascii data.
	/// </summary>
	public static class AsciiImporter
	{
		/// <summary>Prepend this string to a file name in order to designate the stream origin as file name origin.</summary>
		public const string FileUrlStart = @"file:///";

		#region Importing into existing table

		#region From stream

		/// <summary>
		/// Imports an Ascii stream into a table. The import options have to be known already.
		/// </summary>
		/// <param name="table">The table into which to import.</param>
		/// <param name="stream">The stream to read from.</param>
		/// <param name="streamOriginHint">Stream origin hint. If the stream was opened from a file, you should prepend <see cref=" FileUrlStart"/> to the file name.</param>
		/// <param name="importOptions">The Ascii import options. This parameter can be null, or the options can be not fully specified. In this case the method tries to determine the import options by analyzing the stream.</param>
		/// <exception cref="System.ArgumentNullException">
		/// Argument importOptions is null
		/// or
		/// Argument table is null
		/// </exception>
		/// <exception cref="System.ArgumentException">Argument importOptions: importOptions must be fully specified, i.e. all elements of importOptions must be valid. Please run a document analysis in-before to get appropriate values.</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// Unconsidered AsciiColumnType:  + impopt.RecognizedStructure[i].ToString()
		/// or
		/// Unknown switch case:  + impopt.HeaderLinesDestination.ToString()
		/// </exception>
		private static void InternalImportFromAsciiStream(this DataTable table, Stream stream, string streamOriginHint, ref AsciiImportOptions importOptions)
		{
			if (null == importOptions || !importOptions.IsFullySpecified)
			{
				var analysisOptions = GetDefaultAsciiDocumentAnalysisOptions(table);
				importOptions = AsciiDocumentAnalysis.Analyze(importOptions ?? new AsciiImportOptions(), stream, analysisOptions);
			}

			if (null == importOptions)
				throw new InvalidDataException("Import options could not be determined from the data stream. Possibly, the data stream is empty or it is not an Ascii data stream");
			if (!importOptions.IsFullySpecified)
				throw new InvalidDataException("Import options could not be fully determined from the data stream. Possibly, the data stream is empty or it is not an Ascii data stream");

			string sLine;
			stream.Position = 0; // rewind the stream to the beginning
			StreamReader sr = new StreamReader(stream, System.Text.Encoding.Default, true);
			DataColumnCollection newcols = new DataColumnCollection();

			DataColumnCollection newpropcols = new DataColumnCollection();

			// in case a structure is provided, allocate already the columsn

			if (null != importOptions.RecognizedStructure)
			{
				for (int i = 0; i < importOptions.RecognizedStructure.Count; i++)
				{
					switch (importOptions.RecognizedStructure[i].ColumnType)
					{
						case AsciiColumnType.Double:
							newcols.Add(new DoubleColumn());
							break;

						case AsciiColumnType.Int64:
							newcols.Add(new DoubleColumn());
							break;

						case AsciiColumnType.DateTime:
							newcols.Add(new DateTimeColumn());
							break;

						case AsciiColumnType.Text:
							newcols.Add(new TextColumn());
							break;

						case AsciiColumnType.DBNull:
							newcols.Add(new DBNullColumn());
							break;

						default:
							throw new ArgumentOutOfRangeException("Unconsidered AsciiColumnType: " + importOptions.RecognizedStructure[i].ToString());
					}
				}
			}

			// add also additional property columns if not enough there
			if (importOptions.NumberOfMainHeaderLines.HasValue && importOptions.NumberOfMainHeaderLines.Value > 0) // if there are more than one header line, allocate also property columns
			{
				int toAdd = importOptions.NumberOfMainHeaderLines.Value;
				for (int i = 0; i < toAdd; i++)
					newpropcols.Add(new Data.TextColumn());
			}

			// if decimal separator statistics is provided by impopt, create a number format info object
			System.Globalization.NumberFormatInfo numberFormatInfo = importOptions.NumberFormatCulture.NumberFormat;
			System.Globalization.DateTimeFormatInfo dateTimeFormat = importOptions.DateTimeFormatCulture.DateTimeFormat;

			var notesHeader = new System.Text.StringBuilder();
			notesHeader.Append("Imported");
			if (!string.IsNullOrEmpty(streamOriginHint))
				notesHeader.AppendFormat(" from {0}", streamOriginHint);
			notesHeader.AppendFormat(" at {0}", DateTime.Now);
			notesHeader.AppendLine();

			// first of all, read the header if existent
			for (int i = 0; i < importOptions.NumberOfMainHeaderLines; i++)
			{
				sLine = sr.ReadLine();
				if (null == sLine) break;

				var tokens = new List<string>(importOptions.SeparationStrategy.GetTokens(sLine));
				if (i == importOptions.IndexOfCaptionLine) // is it the column name line
				{
					for (int k = 0; k < tokens.Count; ++k)
					{
						var ttoken = tokens[k].Trim();
						if (!string.IsNullOrEmpty(ttoken))
						{
							string newcolname = newcols.FindUniqueColumnName(ttoken);
							newcols.SetColumnName(k, newcolname);
						}
					}
					continue;
				}

				switch (importOptions.HeaderLinesDestination)
				{
					case AsciiHeaderLinesDestination.Ignore:
						break;

					case AsciiHeaderLinesDestination.ImportToNotes:
						AppendLineToTableNotes(notesHeader, sLine);
						break;

					case AsciiHeaderLinesDestination.ImportToProperties:
						FillPropertyColumnWithTokens(newpropcols[i], tokens);
						break;

					case AsciiHeaderLinesDestination.ImportToPropertiesOrNotes:
						if (tokens.Count == importOptions.RecognizedStructure.Count)
							FillPropertyColumnWithTokens(newpropcols[i], tokens);
						else
							AppendLineToTableNotes(notesHeader, sLine);
						break;

					case AsciiHeaderLinesDestination.ImportToPropertiesAndNotes:
						FillPropertyColumnWithTokens(newpropcols[i], tokens);
						AppendLineToTableNotes(notesHeader, sLine);
						break;

					default:
						throw new ArgumentOutOfRangeException("Unknown switch case: " + importOptions.HeaderLinesDestination.ToString());
				}
			}

			// now the data lines
			for (int i = 0; true; i++)
			{
				sLine = sr.ReadLine();
				if (null == sLine)
					break;

				int maxcolumns = newcols.ColumnCount;

				int k = -1;
				foreach (string token in importOptions.SeparationStrategy.GetTokens(sLine))
				{
					k++;
					if (k >= maxcolumns)
						break;

					if (string.IsNullOrEmpty(token))
						continue;

					if (newcols[k] is DoubleColumn)
					{
						double val;
						if (double.TryParse(token, System.Globalization.NumberStyles.Any, numberFormatInfo, out val))
							((DoubleColumn)newcols[k])[i] = val;
					}
					else if (newcols[k] is DateTimeColumn)
					{
						DateTime val;
						if (DateTime.TryParse(token, dateTimeFormat, System.Globalization.DateTimeStyles.NoCurrentDateDefault, out val))
							((DateTimeColumn)newcols[k])[i] = val;
					}
					else if (newcols[k] is TextColumn)
					{
						((TextColumn)newcols[k])[i] = token.Trim();
					}
					else if (null == newcols[k] || newcols[k] is DBNullColumn)
					{
						bool bConverted = false;
						double val = Double.NaN;
						DateTime valDateTime = DateTime.MinValue;

						try
						{
							val = System.Convert.ToDouble(token);
							bConverted = true;
						}
						catch
						{
						}
						if (bConverted)
						{
							DoubleColumn newc = new DoubleColumn();
							newc[i] = val;
							newcols.Replace(k, newc);
						}
						else
						{
							try
							{
								valDateTime = System.Convert.ToDateTime(token);
								bConverted = true;
							}
							catch
							{
							}
							if (bConverted)
							{
								DateTimeColumn newc = new DateTimeColumn();
								newc[i] = valDateTime;

								newcols.Replace(k, newc);
							}
							else
							{
								TextColumn newc = new TextColumn();
								newc[i] = token;
								newcols.Replace(k, newc);
							}
						} // end outer if null==newcol
					}
				} // end of for all cols
			} // end of for all lines

			// insert the new columns or replace the old ones
			table.Suspend();
			bool tableWasEmptyBefore = table.DataColumns.ColumnCount == 0;
			for (int i = 0; i < newcols.ColumnCount; i++)
			{
				if (newcols[i] is DBNullColumn) // if the type is undefined, use a new DoubleColumn
					table.DataColumns.CopyOrReplaceOrAdd(i, new DoubleColumn(), newcols.GetColumnName(i));
				else
					table.DataColumns.CopyOrReplaceOrAdd(i, newcols[i], newcols.GetColumnName(i));

				// set the first column as x-column if the table was empty before, and there are more than one column
				if (i == 0 && tableWasEmptyBefore && newcols.ColumnCount > 1)
					table.DataColumns.SetColumnKind(0, ColumnKind.X);
			} // end for loop

			// add the property columns
			for (int i = 0; i < newpropcols.ColumnCount; i++)
			{
				if (newpropcols[i].Count > 0)
					table.PropCols.CopyOrReplaceOrAdd(i, newpropcols[i], newpropcols.GetColumnName(i));
			}

			table.Notes.Write(notesHeader.ToString());

			table.Resume();
		} // end of function ImportAscii

		/// <summary>
		/// Imports an Ascii stream into a table. The import options have to be known already.
		/// </summary>
		/// <param name="dataTable">The table into which to import.</param>
		/// <param name="stream">The stream to read from.</param>
		/// <param name="streamOriginHint">Stream origin hint. If the stream was opened from a file, you should prepend <see cref=" FileUrlStart"/> to the file name.</param>
		/// <param name="importOptions">The Ascii import options. This parameter must not be <c>null</c> and must contain valid options. The import is executed using these options.</param>
		/// <exception cref="System.ArgumentNullException">
		/// Argument importOptions is null
		/// or
		/// Argument table is null
		/// </exception>
		/// <exception cref="System.ArgumentException">Argument importOptions: importOptions must be fully specified, i.e. all elements of importOptions must be valid. Please run a document analysis in-before to get appropriate values.</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// Unconsidered AsciiColumnType:  + impopt.RecognizedStructure[i].ToString()
		/// or
		/// Unknown switch case:  + impopt.HeaderLinesDestination.ToString()
		/// </exception>
		public static void ImportFromAsciiStream(this DataTable dataTable, Stream stream, string streamOriginHint, AsciiImportOptions importOptions)
		{
			if (importOptions == null)
				throw new ArgumentNullException("Argument importOptions is null");
			if (!importOptions.IsFullySpecified)
				throw new ArgumentException("Argument importOptions: importOptions must be fully specified, i.e. all elements of importOptions must be valid. Please run a document analysis in-before to get appropriate values.");
			if (null == dataTable)
				throw new ArgumentNullException("Argument table is null");

			InternalImportFromAsciiStream(dataTable, stream, streamOriginHint, ref importOptions);
		}

		/// <summary>
		/// Imports Ascii data from a stream into the data table.
		/// </summary>
		/// <param name="dataTable">The table where to import into.</param>
		/// <param name="myStream">The stream to import from.</param>
		/// <param name="streamOriginHint">Designates a short hint where the provided stream originates from. Can be <c>Null</c> if the origin is unknown.</param>
		/// <param name="importOptions">On return, contains the recognized import options that were used to import from the provided stream.</param>
		public static void ImportFromAsciiStream(this DataTable dataTable, Stream myStream, string streamOriginHint, out AsciiImportOptions importOptions)
		{
			importOptions = null;
			InternalImportFromAsciiStream(dataTable, myStream, streamOriginHint, ref importOptions);

			// finally set or change the data source of the table
			AddOrUpdateAsciiImportDataSource(dataTable, streamOriginHint, importOptions);
		}

		/// <summary>
		/// Imports Ascii data from a stream into the data table.
		/// </summary>
		/// <param name="dataTable">The table where to import into.</param>
		/// <param name="myStream">The stream to import from.</param>
		/// <param name="streamOriginHint">Designates a hint where the provided stream originates from. Can be <c>Null</c> if the origin is unknown.</param>
		public static void ImportFromAsciiStream(this DataTable dataTable, Stream myStream, string streamOriginHint)
		{
			AsciiImportOptions dummy;
			ImportFromAsciiStream(dataTable, myStream, streamOriginHint, out dummy);
		}

		#endregion From stream

		#region From single file

		/// <summary>
		/// Imports from an ASCII file into an existing table. The import options have to be known already.
		/// </summary>
		/// <param name="dataTable">The data table to import into.</param>
		/// <param name="fileName">File name of the file to import.</param>
		/// <param name="importOptions">The import options.</param>
		public static void ImportFromAsciiFile(this DataTable dataTable, string fileName, AsciiImportOptions importOptions)
		{
			if (null == dataTable)
				throw new ArgumentNullException("Argument dataTable is null");
			if (string.IsNullOrEmpty(fileName))
				throw new ArgumentNullException("Argument fileName is null or empty");
			if (importOptions == null)
				throw new ArgumentNullException("Argument importOptions is null");
			if (!importOptions.IsFullySpecified)
				throw new ArgumentException("Argument importOptions: importOptions must be fully specified, i.e. all elements of importOptions must be valid. Please run a document analysis in-before to get appropriate values.");

			using (var myStream = GetAsciiInputFileStream(fileName))
			{
				ImportFromAsciiStream(dataTable, myStream, FileUrlStart + fileName, importOptions);
				myStream.Close();
			}
		}

		/// <summary>
		/// Imports from an ASCII file into an existing table.
		/// </summary>
		/// <param name="dataTable">The data table to import into.</param>
		/// <param name="fileName">File name of the file to import.</param>
		/// <param name="importOptions">On return, contains the import options that were used to import the file.</param>
		/// <exception cref="System.ArgumentNullException">
		/// Argument dataTable is null
		/// or
		/// Argument fileName is null or empty
		/// </exception>
		public static void ImportFromAsciiFile(this DataTable dataTable, string fileName, out AsciiImportOptions importOptions)
		{
			if (null == dataTable)
				throw new ArgumentNullException("Argument dataTable is null");
			if (string.IsNullOrEmpty(fileName))
				throw new ArgumentNullException("Argument fileName is null or empty");

			using (var myStream = GetAsciiInputFileStream(fileName))
			{
				ImportFromAsciiStream(dataTable, myStream, FileUrlStart + fileName, out importOptions);
				myStream.Close();
			}
		}

		/// <summary>
		/// Imports from an ASCII file into an existing table.
		/// </summary>
		/// <param name="dataTable">The data table to import into.</param>
		/// <param name="fileName">File name of the file to import.</param>
		/// <exception cref="System.ArgumentNullException">
		/// Argument dataTable is null
		/// or
		/// Argument fileName is null or empty
		/// </exception>
		public static void ImportFromAsciiFile(this DataTable dataTable, string fileName)
		{
			if (null == dataTable)
				throw new ArgumentNullException("Argument dataTable is null");
			if (string.IsNullOrEmpty(fileName))
				throw new ArgumentNullException("Argument fileName is null or empty");

			AsciiImportOptions dummy;
			ImportFromAsciiFile(dataTable, fileName, out dummy);
		}

		#endregion From single file

		#region From text

		/// <summary>
		/// Imports from an ASCII text provided as string into an existing table. The import options have to be known already.
		/// </summary>
		/// <param name="dataTable">The data table to import into.</param>
		/// <param name="asciiText">The Ascii text that is to be imported.</param>
		/// <param name="importOptions">The import options. This parameter must not be null, and the options must be fully specified.</param>
		public static void ImportFromAsciiText(this DataTable dataTable, string asciiText, AsciiImportOptions importOptions)
		{
			if (null == dataTable)
				throw new ArgumentNullException("Argument dataTable is null");
			if (null == asciiText)
				throw new ArgumentNullException("Argument asciiText is null");
			if (importOptions == null)
				throw new ArgumentNullException("Argument importOptions is null");
			if (!importOptions.IsFullySpecified)
				throw new ArgumentException("Argument importOptions: importOptions must be fully specified, i.e. all elements of importOptions must be valid. Please run a document analysis in-before to get appropriate values.");

			using (var memstream = new MemoryStream())
			{
				using (var textwriter = new StreamWriter(memstream))
				{
					textwriter.Write(asciiText);
					textwriter.Flush();
				}
				memstream.Position = 0;
				ImportFromAsciiStream(dataTable, memstream, "Ascii text", importOptions);
			}
		}

		/// <summary>
		/// Imports from an ASCII text provided as string into an existing table.
		/// </summary>
		/// <param name="dataTable">The data table to import into.</param>
		/// <param name="asciiText">The Ascii text that is to be imported.</param>
		/// <param name="importOptions">On return, contains the import options that were used to import the Ascii text.</param>
		public static void ImportFromAsciiText(this DataTable dataTable, string asciiText, out AsciiImportOptions importOptions)
		{
			if (null == dataTable)
				throw new ArgumentNullException("Argument dataTable is null");
			if (null == asciiText)
				throw new ArgumentNullException("Argument asciiText is null");

			using (var memstream = new MemoryStream())
			{
				using (var textwriter = new StreamWriter(memstream))
				{
					textwriter.Write(asciiText);
					textwriter.Flush();
				}
				memstream.Position = 0;
				ImportFromAsciiStream(dataTable, memstream, "Ascii text", out importOptions);
			}
		}

		/// <summary>
		/// Imports from an ASCII text provided as string into an existing table.
		/// </summary>
		/// <param name="dataTable">The data table to import into.</param>
		/// <param name="asciiText">The Ascii text that is to be imported.</param>
		public static void ImportFromAsciiText(this DataTable dataTable, string asciiText)
		{
			if (null == dataTable)
				throw new ArgumentNullException("Argument dataTable is null");
			if (null == asciiText)
				throw new ArgumentNullException("Argument asciiText is null");

			AsciiImportOptions importOptions;
			ImportFromAsciiText(dataTable, asciiText, out importOptions);
		}

		#endregion From text

		#region From multiple files

		#region Internal implementations (horizontal and vertical)

		/// <summary>
		/// Imports a couple of ASCII files into one (!) table. The first column of each file is considered to be the x-column, and if they match another x-column, the newly imported columns will get the same column group.
		/// </summary>
		/// <param name="table">The table the data should be imported to.</param>
		/// <param name="fileNames">An array of filenames to import.</param>
		/// <param name="sortFileNames">If <c>true</c>, the fileNames are sorted before usage in ascending order using the default string comparator.</param>
		/// <param name="importOptions">Options used to import the Ascii files. This parameter can be <c>null</c>. In this case the value on return is the determined import options of the first file (if <paramref name="determineImportOptionsSeparatelyForEachFile"/> is <c>false</c>) or of the last file (if <paramref name="determineImportOptionsSeparatelyForEachFile"/> is <c>true</c>).</param>
		/// <param name="determineImportOptionsSeparatelyForEachFile">
		/// If <c>true</c>, the import options are determined for each file separately. In this case the provided parameter <paramref name="importOptions"/> is ignored, but on return it contains the importOptions used to import the last file.
		/// If <c>false</c>, the import options are either provided by the parameter <paramref name="importOptions"/> (if not null and fully specified), or during import of the first file. The so determined importOptions are then used to import all other files.
		/// </param>
		/// <returns>Null if no error occurs, or an error description.</returns>
		private static string InternalImportFromMultipleAsciiFilesHorizontally(this DataTable table, IEnumerable<string> fileNames, bool sortFileNames, ref AsciiImportOptions importOptions, bool determineImportOptionsSeparatelyForEachFile)
		{
			DataColumn xcol = null;
			DataColumn xvalues;
			System.Text.StringBuilder errorList = new System.Text.StringBuilder();

			int lastColumnGroup = 0;

			if (table.DataColumns.ColumnCount > 0)
			{
				lastColumnGroup = table.DataColumns.GetColumnGroup(table.DataColumns.ColumnCount - 1);
				DataColumn xColumnOfRightMost = table.DataColumns.FindXColumnOfGroup(lastColumnGroup);
				xcol = (DoubleColumn)xColumnOfRightMost;
			}

			// add also a property column named "FilePath" if not existing so far
			TextColumn filePathPropCol = (TextColumn)table.PropCols.EnsureExistence("FilePath", typeof(TextColumn), ColumnKind.Label, 0);

			if (sortFileNames)
				fileNames = fileNames.OrderBy(x => x);

			foreach (string fileName in fileNames)
			{
				DataTable srcTable = new DataTable();
				if (determineImportOptionsSeparatelyForEachFile)
					ImportFromAsciiFile(srcTable, fileName);
				else if (null != importOptions && importOptions.IsFullySpecified)
					ImportFromAsciiFile(srcTable, fileName, importOptions);
				else
					ImportFromAsciiFile(srcTable, fileName, out importOptions);

				if (srcTable.DataColumns.ColumnCount == 0)
					continue;

				xvalues = srcTable.DataColumns[0];
				bool bMatchsXColumn = false;

				// first look if our default xcolumn matches the xvalues
				if (null != xcol)
					bMatchsXColumn = ValuesMatch(xvalues, xcol);

				// if no match, then consider all xcolumns from right to left, maybe some fits
				if (!bMatchsXColumn)
				{
					for (int ncol = table.DataColumns.ColumnCount - 1; ncol >= 0; ncol--)
					{
						if ((ColumnKind.X == table.DataColumns.GetColumnKind(ncol)) &&
							(ValuesMatch(xvalues, table.DataColumns[ncol]))
							)
						{
							xcol = table.DataColumns[ncol];
							lastColumnGroup = table.DataColumns.GetColumnGroup(xcol);
							bMatchsXColumn = true;
							break;
						}
					}
				}

				// create a new x column if the last one does not match
				if (!bMatchsXColumn)
				{
					xcol = (DataColumn)xvalues.Clone();
					lastColumnGroup = table.DataColumns.GetUnusedColumnGroupNumber();
					table.DataColumns.Add(xcol, srcTable.DataColumns.GetColumnName(0), ColumnKind.X, lastColumnGroup);
				}

				for (int i = 1; i < srcTable.DataColumns.ColumnCount; i++)
				{
					// now add the y-values
					DataColumn ycol = (DataColumn)srcTable.DataColumns[i].Clone();
					table.DataColumns.Add(ycol,
					table.DataColumns.FindUniqueColumnName(srcTable.DataColumns.GetColumnName(i)),
						ColumnKind.V,
						lastColumnGroup);

					// now set the file name property cell
					int destcolnumber = table.DataColumns.GetColumnNumber(ycol);
					filePathPropCol[destcolnumber] = fileName;

					// now set the imported property cells
					for (int s = 0; s < srcTable.PropCols.ColumnCount; s++)
					{
						DataColumn dest = table.PropCols.EnsureExistence(srcTable.PropCols.GetColumnName(s), srcTable.PropCols[s].GetType(), ColumnKind.V, 0);
						dest.SetValueAt(destcolnumber, srcTable.PropCols[s][i]);
					}
				}
			} // foreache file

			return errorList.Length == 0 ? null : errorList.ToString();
		}

		/// <summary>
		/// Imports a couple of ASCII files into one (!) table, vertically. If the names of the subsequently imported table columns match, the data
		/// will be written in the matching column. Otherwise new columns with the unmatched column names were created.
		/// Property columns will only be imported from the first table.
		/// </summary>
		/// <param name="table">The table the data should be imported to.</param>
		/// <param name="fileNames">An array of file names to import.</param>
		/// <param name="sortFileNames">If <c>true</c>, the fileNames are sorted before usage in ascending order using the default string comparator.</param>
		/// <param name="importOptions">Options used to import the Ascii files. This parameter can be <c>null</c>. In this case the value on return is the determined import options of the first file (if <paramref name="determineImportOptionsSeparatelyForEachFile"/> is <c>false</c>) or of the last file (if <paramref name="determineImportOptionsSeparatelyForEachFile"/> is <c>true</c>).</param>
		/// <param name="determineImportOptionsSeparatelyForEachFile">
		/// If <c>true</c>, the import options are determined for each file separately. In this case the provided parameter <paramref name="importOptions"/> is ignored, but on return it contains the importOptions used to import the last file.
		/// If <c>false</c>, the import options are either provided by the parameter <paramref name="importOptions"/> (if not null and fully specified), or during import of the first file. The so determined importOptions are then used to import all other files.
		/// </param>
		/// <returns>Null if no error occurs, or an error description.</returns>
		private static string InternalImportFromMultipleAsciiFilesVertically(this DataTable table, IEnumerable<string> fileNames, bool sortFileNames, ref AsciiImportOptions importOptions, bool determineImportOptionsSeparatelyForEachFile)
		{
			System.Text.StringBuilder errorList = new System.Text.StringBuilder();
			int lastDestinationRow = table.DataColumns.RowCount;
			int numberOfImportedTables = 0;

			// add also a property column named "FilePath" if not existing so far
			TextColumn filePathCol = (TextColumn)table.Col.EnsureExistence("FilePath", typeof(TextColumn), ColumnKind.Label, 0);

			if (sortFileNames)
				fileNames = fileNames.OrderBy(x => x);

			foreach (string fileName in fileNames)
			{
				DataTable srcTable = new DataTable();
				if (determineImportOptionsSeparatelyForEachFile)
					ImportFromAsciiFile(srcTable, fileName);
				else if (null != importOptions && importOptions.IsFullySpecified)
					ImportFromAsciiFile(srcTable, fileName, importOptions);
				else
					ImportFromAsciiFile(srcTable, fileName, out importOptions);

				if (srcTable.DataColumns.ColumnCount == 0)
					continue;

				// mark the beginning of the new file with the file path
				filePathCol[lastDestinationRow] = fileName;

				// transfer the data columns
				for (int srcDataColIdx = 0; srcDataColIdx < srcTable.DataColumns.ColumnCount; srcDataColIdx++)
				{
					var srcDataCol = srcTable.DataColumns[srcDataColIdx];

					var destDataCol = table.DataColumns.EnsureExistence(srcTable.DataColumns.GetColumnName(srcDataColIdx), srcTable.DataColumns[srcDataColIdx].GetType(), srcTable.DataColumns.GetColumnKind(srcDataColIdx), srcTable.DataColumns.GetColumnGroup(srcDataColIdx));
					int destDataColIdx = table.DataColumns.GetColumnNumber(destDataCol);

					// transfer the data of one data column
					for (int j = 0; j < srcDataCol.Count; j++)
						destDataCol[lastDestinationRow + j] = srcDataCol[j];

					// now also process the property columns
					for (int srcPropColIdx = 0; srcPropColIdx < srcTable.PropCols.ColumnCount; srcPropColIdx++)
					{
						var destPropCol = table.PropCols.EnsureExistence(srcTable.PropCols.GetColumnName(srcPropColIdx), srcTable.PropCols[srcPropColIdx].GetType(), srcTable.PropCols.GetColumnKind(srcPropColIdx), srcTable.PropCols.GetColumnGroup(srcPropColIdx));

						if (0 == numberOfImportedTables)
						{
							destPropCol[destDataColIdx] = srcTable.PropCols[srcPropColIdx][srcDataColIdx];
						}
						else if (destPropCol[destDataColIdx] != srcTable.PropCols[srcPropColIdx][srcDataColIdx])
						{
							destPropCol.SetElementEmpty(destDataColIdx);
						}
					}
				}

				lastDestinationRow += srcTable.DataColumns.RowCount;
				numberOfImportedTables++;
			} // foreache file

			return errorList.Length == 0 ? null : errorList.ToString();
		}

		#endregion Internal implementations (horizontal and vertical)

		#region public functions for horizonal import

		/// <summary>
		/// Imports multiple Ascii files into the provided table in horizontal order, i.e. in new columns. The provided <paramref name="importOptions"/> are used to import the files.
		/// The first column of each file is considered to be the x-column, and if they match another x-column, the newly imported columns will get the same column group.
		/// </summary>
		/// <param name="dataTable">The table the data should be imported to.</param>
		/// <param name="fileNames">An array of filenames to import.</param>
		/// <param name="sortFileNames">If <c>true</c>, the fileNames are sorted before usage in ascending order using the default string comparator.</param>
		/// <param name="importOptions">Options used to import the Ascii files. This parameter must not be null, and the options must be fully specified.</param>
		/// <returns>Null if no error occurs, or an error description.</returns>
		public static string ImportFromMultipleAsciiFilesHorizontally(this DataTable dataTable, IEnumerable<string> fileNames, bool sortFileNames, AsciiImportOptions importOptions)
		{
			if (null == dataTable)
				throw new ArgumentNullException("Argument dataTable is null");
			if (null == fileNames)
				throw new ArgumentNullException("Argument fileNames is null");
			if (importOptions == null)
				throw new ArgumentNullException("Argument importOptions is null");
			if (!importOptions.IsFullySpecified)
				throw new ArgumentException("Argument importOptions: importOptions must be fully specified, i.e. all elements of importOptions must be valid. Please run a document analysis in-before to get appropriate values.");

			return InternalImportFromMultipleAsciiFilesHorizontally(dataTable, fileNames, sortFileNames, ref importOptions, false);
		}

		/// <summary>
		/// Imports multiple Ascii files into the provided table in horizontal order, i.e. in new columns. The import options are determined from the first file, and then used to import all other files.
		/// The first column of each file is considered to be the x-column, and if they match another x-column, the newly imported columns will get the same column group.
		/// </summary>
		/// <param name="dataTable">The table the data should be imported to.</param>
		/// <param name="fileNames">An array of filenames to import.</param>
		/// <param name="sortFileNames">If <c>true</c>, the fileNames are sorted before usage in ascending order using the default string comparator.</param>
		/// <param name="importOptions">On return, contains the options used to import the first Ascii files. These options are also used to import all other Ascii files.</param>
		/// <returns>Null if no error occurs, or an error description.</returns>
		public static string ImportFromMultipleAsciiFilesHorizontally(this DataTable dataTable, IEnumerable<string> fileNames, bool sortFileNames, out AsciiImportOptions importOptions)
		{
			if (null == dataTable)
				throw new ArgumentNullException("Argument dataTable is null");
			if (null == fileNames)
				throw new ArgumentNullException("Argument fileNames is null");

			importOptions = null;
			return InternalImportFromMultipleAsciiFilesHorizontally(dataTable, fileNames, sortFileNames, ref importOptions, false);
		}

		/// <summary>
		/// Imports multiple Ascii files into the provided table in horizontal order, i.e. in new columns. Depending on the value of <paramref name="determineImportOptionsSeparatelyForEachFile"/>, the import options are either deterimined from the first file and then used for all other files, or determined separately for each file.
		/// The first column of each file is considered to be the x-column, and if they match another x-column, the newly imported columns will get the same column group.
		/// </summary>
		/// <param name="dataTable">The table the data should be imported to.</param>
		/// <param name="fileNames">An array of filenames to import.</param>
		/// <param name="sortFileNames">If <c>true</c>, the fileNames are sorted before usage in ascending order using the default string comparator.</param>
		/// <param name="determineImportOptionsSeparatelyForEachFile">If <c>true</c>, the import options are determined for each file separately. Otherwise, i.e. if <c>false</c>, the import options are determined from the first file that is imported.</param>
		/// <returns>Null if no error occurs, or an error description.</returns>
		public static string ImportFromMultipleAsciiFilesHorizontally(this DataTable dataTable, IEnumerable<string> fileNames, bool sortFileNames, bool determineImportOptionsSeparatelyForEachFile)
		{
			if (null == dataTable)
				throw new ArgumentNullException("Argument dataTable is null");
			if (null == fileNames)
				throw new ArgumentNullException("Argument fileNames is null");

			AsciiImportOptions importOptions = null;
			return InternalImportFromMultipleAsciiFilesHorizontally(dataTable, fileNames, sortFileNames, ref importOptions, determineImportOptionsSeparatelyForEachFile);
		}

		#endregion public functions for horizonal import

		#region public functions for vertical import

		/// <summary>
		/// Imports multiple Ascii files into the provided table in vertical order, i.e. in new rows. The provided <paramref name="importOptions"/> are used to import the files.
		/// If the names of the subsequently imported table columns match, the data will be written in the matching column. Otherwise new columns with the unmatched column names were created.
		/// Property columns will only be imported from the first table.
		/// </summary>
		/// <param name="dataTable">The table the data should be imported to.</param>
		/// <param name="fileNames">An array of filenames to import.</param>
		/// <param name="sortFileNames">If <c>true</c>, the fileNames are sorted before usage in ascending order using the default string comparator.</param>
		/// <param name="importOptions">Options used to import the Ascii files. This parameter must not be null, and the options must be fully specified.</param>
		/// <returns>Null if no error occurs, or an error description.</returns>
		public static string ImportFromMultipleAsciiFilesVertically(this DataTable dataTable, IEnumerable<string> fileNames, bool sortFileNames, AsciiImportOptions importOptions)
		{
			if (null == dataTable)
				throw new ArgumentNullException("Argument dataTable is null");
			if (null == fileNames)
				throw new ArgumentNullException("Argument fileNames is null");
			if (importOptions == null)
				throw new ArgumentNullException("Argument importOptions is null");
			if (!importOptions.IsFullySpecified)
				throw new ArgumentException("Argument importOptions: importOptions must be fully specified, i.e. all elements of importOptions must be valid. Please run a document analysis in-before to get appropriate values.");

			return InternalImportFromMultipleAsciiFilesVertically(dataTable, fileNames, sortFileNames, ref importOptions, false);
		}

		/// <summary>
		/// Imports multiple Ascii files into the provided table in vertical order, i.e. in new rows.. The import options are determined from the first file, and then used to import all other files.
		/// If the names of the subsequently imported table columns match, the data will be written in the matching column. Otherwise new columns with the unmatched column names were created.
		/// Property columns will only be imported from the first table.
		/// </summary>
		/// <param name="dataTable">The table the data should be imported to.</param>
		/// <param name="fileNames">An array of filenames to import.</param>
		/// <param name="sortFileNames">If <c>true</c>, the fileNames are sorted before usage in ascending order using the default string comparator.</param>
		/// <param name="importOptions">On return, contains the options used to import the first Ascii files. These options are also used to import all other Ascii files.</param>
		/// <returns>Null if no error occurs, or an error description.</returns>
		public static string ImportFromMultipleAsciiFilesVertically(this DataTable dataTable, IEnumerable<string> fileNames, bool sortFileNames, out AsciiImportOptions importOptions)
		{
			if (null == dataTable)
				throw new ArgumentNullException("Argument dataTable is null");
			if (null == fileNames)
				throw new ArgumentNullException("Argument fileNames is null");

			importOptions = null;
			return InternalImportFromMultipleAsciiFilesVertically(dataTable, fileNames, sortFileNames, ref importOptions, false);
		}

		/// <summary>
		/// Imports multiple Ascii files into the provided table in vertical order, i.e. in new rows.. Depending on the value of <paramref name="determineImportOptionsSeparatelyForEachFile"/>, the import options are either deterimined from the first file and then used for all other files, or determined separately for each file.
		/// If the names of the subsequently imported table columns match, the data will be written in the matching column. Otherwise new columns with the unmatched column names were created.
		/// Property columns will only be imported from the first table.
		/// </summary>
		/// <param name="dataTable">The table the data should be imported to.</param>
		/// <param name="fileNames">An array of filenames to import.</param>
		/// <param name="determineImportOptionsSeparatelyForEachFile">If <c>true</c>, the import options are determined for each file separately. Otherwise, i.e. if <c>false</c>, the import options are determined from the first file that is imported.</param>
		/// <returns>Null if no error occurs, or an error description.</returns>
		public static string ImportFromMultipleAsciiFilesVertically(this DataTable dataTable, IEnumerable<string> fileNames, bool sortFileNames, bool determineImportOptionsSeparatelyForEachFile)
		{
			if (null == dataTable)
				throw new ArgumentNullException("Argument dataTable is null");
			if (null == fileNames)
				throw new ArgumentNullException("Argument fileNames is null");

			AsciiImportOptions importOptions = null;
			return InternalImportFromMultipleAsciiFilesVertically(dataTable, fileNames, sortFileNames, ref importOptions, determineImportOptionsSeparatelyForEachFile);
		}

		#endregion public functions for vertical import

		#endregion From multiple files

		#endregion Importing into existing table

		#region Importing into newly created table

		/// <summary>
		/// Imports ascii from a memory stream into a table. Returns null (!) if nothing is imported.
		/// </summary>
		/// <param name="stream">The stream to import ascii from. Is not (!) closed at the end of this function.</param>
		/// <param name="streamOriginHint">Designates a short hint where the provided stream originates from. Can be <c>Null</c> if the origin is unknown.</param>
		/// <param name="defaultImportOptions">The default import options. The importer uses this options as base, but updates some fields by analyzing the data to import.</param>
		/// <returns>The table representation of the imported text, or null if nothing is imported.</returns>
		private static DataTable InternalImportStreamIntoNewTable(Stream stream, string streamOriginHint, AsciiImportOptions defaultImportOptions)
		{
			var importOptions = AsciiDocumentAnalysis.Analyze(defaultImportOptions ?? new AsciiImportOptions(), stream, GetDefaultAsciiDocumentAnalysisOptions(null));
			if (importOptions != null)
			{
				DataTable table = new DataTable();
				ImportFromAsciiStream(table, stream, streamOriginHint, importOptions);
				return table;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Imports ascii from a stream into a table. Returns null (!) if nothing is imported.
		/// </summary>
		/// <param name="stream">The stream to import ascii from. Is not (!) closed at the end of this function.</param>
		/// <param name="streamOriginHint">Designates a short hint where the provided stream originates from. Can be <c>Null</c> if the origin is unknown.</param>
		/// <returns>The table representation of the imported text, or null if nothing is imported.</returns>
		public static DataTable ImportStreamIntoNewTable(Stream stream, string streamOriginHint)
		{
			return InternalImportStreamIntoNewTable(stream, streamOriginHint, null);
		}

		/// <summary>
		/// Imports ascii from a string into a table. Returns null (!) if nothing is imported.
		/// </summary>
		/// <param name="text">The text to import as ascii.</param>
		/// <returns>The table representation of the imported text, or null if nothing is imported.</returns>
		public static DataTable ImportTextIntoNewTable(string text)
		{
			using (var memstream = new MemoryStream())
			{
				using (var textwriter = new StreamWriter(memstream))
				{
					textwriter.Write(text);
					textwriter.Flush();
				}
				memstream.Position = 0;

				return InternalImportStreamIntoNewTable(memstream, "text", null);
			}
		}

		/// <summary>
		/// Imports ascii from a memory stream into a table. Returns null (!) if nothing is imported.
		/// </summary>
		/// <param name="filename">The file name of the file from which to import.</param>
		/// <returns>The table representation of the imported text, or null if nothing is imported.</returns>
		public static DataTable ImportFileIntoNewTable(string filename)
		{
			if (string.IsNullOrEmpty(filename))
				throw new ArgumentNullException("filename is null or empty");

			using (var stream = GetAsciiInputFileStream((filename)))
			{
				return InternalImportStreamIntoNewTable(stream, FileUrlStart + filename, null);
			}
		}

		/// <summary>
		/// Imports ascii from a memory stream into a table. Returns null (!) if nothing is imported.
		/// </summary>
		/// <param name="filename">The file name of the file from which to import.</param>
		/// <param name="separatorChar">The character used to separate the columns</param>
		/// <returns>The table representation of the imported text, or null if nothing is imported.</returns>
		public static DataTable ImportFileIntoNewTable(string filename, char separatorChar)
		{
			if (string.IsNullOrEmpty(filename))
				throw new ArgumentNullException("filename is null or empty");

			var defaultImportOptions = new AsciiImportOptions();
			defaultImportOptions.SeparationStrategy = new SingleCharSeparationStrategy(separatorChar);
			using (var stream = GetAsciiInputFileStream(filename))
			{
				return InternalImportStreamIntoNewTable(stream, FileUrlStart + filename, defaultImportOptions);
			}
		}

		#region Multiple files in separate new tables

		/// <summary>
		/// Imports multiple Ascii files into newly created new tables (each file into a separate table).
		/// </summary>
		/// <param name="projectFolder">The project folder in which the new tables should be created.</param>
		/// <param name="fileNames">The names of the files to import.</param>
		/// <param name="sortFileNames">If <c>true</c>, the fileNames are sorted before usage in ascending order using the default string comparator.</param>
		/// <param name="importOptions">Options used to import ASCII. This parameter must not be null, and the options must be fully specified.</param>
		/// <param name="determineImportOptionsSeparatelyForEachFile">
		/// If <c>true</c>, the import options are determined for each file separately. In this case the provided parameter <paramref name="importOptions"/> is ignored, but on return it contains the importOptions used to import the last file.
		/// If <c>false</c>, the import options are either provided by the parameter <paramref name="importOptions"/> (if not null and fully specified), or during import of the first file. The so determined importOptions are then used to import all other files.
		/// </param>
		/// <returns>The list of tables created during the import.</returns>
		private static IList<DataTable> InternalImportFilesIntoSeparateNewTables(ProjectFolder projectFolder, IEnumerable<string> fileNames, bool sortFileNames, ref AsciiImportOptions importOptions, bool determineImportOptionsSeparatelyForEachFile)
		{
			var listOfNewTables = new List<DataTable>();

			if (sortFileNames)
				fileNames = fileNames.OrderBy(x => x);

			foreach (var fileName in fileNames)
			{
				var srcTable = new DataTable(projectFolder.Name + Path.GetFileNameWithoutExtension(fileName));
				Current.ProjectService.CreateNewWorksheet(srcTable);

				if (determineImportOptionsSeparatelyForEachFile)
					ImportFromAsciiFile(srcTable, fileName);
				else if (null != importOptions && importOptions.IsFullySpecified)
					ImportFromAsciiFile(srcTable, fileName, importOptions);
				else
					ImportFromAsciiFile(srcTable, fileName, out importOptions);

				listOfNewTables.Add(srcTable);
			}

			return listOfNewTables;
		}

		/// <summary>
		/// Imports multiple Ascii files into newly created new tables (each file into a separate table).
		/// </summary>
		/// <param name="projectFolder">The project folder in which the new tables should be created.</param>
		/// <param name="fileNames">The names of the files to import.</param>
		/// <param name="sortFileNames">If <c>true</c>, the fileNames are sorted before usage in ascending order using the default string comparator.</param>
		/// <param name="importOptions">Options used to import ASCII. This parameter must not be null, and the options must be fully specified.</param>
		/// <returns>The list of tables created during the import.</returns>
		public static IList<DataTable> ImportFilesIntoSeparateNewTables(this ProjectFolder projectFolder, IEnumerable<string> fileNames, bool sortFileNames, AsciiImportOptions importOptions)
		{
			if (null == projectFolder)
				throw new ArgumentNullException("projectFolder");
			if (null == fileNames)
				throw new ArgumentNullException("filenames");
			if (null == importOptions)
				throw new ArgumentNullException("importOptions");
			if (!importOptions.IsFullySpecified)
				throw new ArgumentException("Argument importOptions: importOptions must be fully specified, i.e. all elements of importOptions must be valid. Please run a document analysis in-before to get appropriate values.");

			return InternalImportFilesIntoSeparateNewTables(projectFolder, fileNames, sortFileNames, ref importOptions, false);
		}

		/// <summary>
		/// Imports multiple Ascii files into newly created new tables (each file into a separate table).
		/// </summary>
		/// <param name="projectFolder">The project folder in which the new tables should be created.</param>
		/// <param name="fileNames">The names of the files to import.</param>
		/// <param name="sortFileNames">If <c>true</c>, the fileNames are sorted before usage in ascending order using the default string comparator.</param>
		/// <param name="importOptions">On return, contains the options used to import the first Ascii files. These options are also used to import all other Ascii files.</param>
		/// <returns>The list of tables created during the import.</returns>
		public static IList<DataTable> ImportFilesIntoSeparateNewTables(this ProjectFolder projectFolder, IEnumerable<string> fileNames, bool sortFileNames, out AsciiImportOptions importOptions)
		{
			if (null == projectFolder)
				throw new ArgumentNullException("projectFolder");
			if (null == fileNames)
				throw new ArgumentNullException("filenames");

			importOptions = null;
			return InternalImportFilesIntoSeparateNewTables(projectFolder, fileNames, sortFileNames, ref importOptions, false);
		}

		/// <summary>
		/// Imports multiple Ascii files into newly created new tables (each file into a separate table).
		/// </summary>
		/// <param name="projectFolder">The project folder in which the new tables should be created.</param>
		/// <param name="fileNames">The names of the files to import.</param>
		/// <param name="sortFileNames">If <c>true</c>, the fileNames are sorted before usage in ascending order using the default string comparator.</param>
		/// <param name="determineImportOptionsSeparatelyForEachFile">If <c>true</c>, the import options are determined for each file separately. Otherwise, i.e. if <c>false</c>, the import options are determined from the first file that is imported.</param>
		/// <returns>The list of tables created during the import.</returns>
		public static IList<DataTable> ImportFilesIntoSeparateNewTables(this ProjectFolder projectFolder, IEnumerable<string> fileNames, bool sortFileNames, bool determineImportOptionsSeparatelyForEachFile)
		{
			if (null == projectFolder)
				throw new ArgumentNullException("projectFolder");
			if (null == fileNames)
				throw new ArgumentNullException("filenames");

			AsciiImportOptions importOptions = null;
			return InternalImportFilesIntoSeparateNewTables(projectFolder, fileNames, sortFileNames, ref importOptions, determineImportOptionsSeparatelyForEachFile);
		}

		#endregion Multiple files in separate new tables

		#endregion Importing into newly created table

		#region Public helper functions

		/// <summary>
		/// Helper function. Gets an <see cref="FileStream"/> by providing a file name. The stream is opened with read access and with the FileShare.Read flag.
		/// </summary>
		/// <param name="filename">The filename.</param>
		/// <returns>The stream. You are responsible for closing / disposing this stream.</returns>
		public static FileStream GetAsciiInputFileStream(string filename)
		{
			return new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		}

		#endregion Public helper functions

		#region Private helper functions

		/// <summary>
		/// Compare the values in a double array with values in a double column and see if they match.
		/// </summary>
		/// <param name="values">An array of double values.</param>
		/// <param name="col">A double column to compare with the double array.</param>
		/// <returns>True if the length of the array is equal to the length of the <see cref="DoubleColumn" /> and the values in
		/// both array match to each other, otherwise false.</returns>
		private static bool ValuesMatch(DataColumn values, DataColumn col)
		{
			if (values.Count != col.Count)
				return false;

			for (int i = 0; i < values.Count; i++)
				if (col[i] != values[i])
					return false;

			return true;
		}

		/// <summary>
		/// Fills the property column with tokens.
		/// </summary>
		/// <param name="newpropcol">The property column to fill.</param>
		/// <param name="tokens">The text tokens.</param>
		private static void FillPropertyColumnWithTokens(DataColumn newpropcol, List<string> tokens)
		{
			for (int k = 0; k < tokens.Count; ++k)
			{
				var ttoken = tokens[k].Trim();
				if (!string.IsNullOrEmpty(ttoken))
				{
					newpropcol[k] = ttoken; // set the properties
				}
			}
		}

		/// <summary>
		/// Appends the ASCII line to the table notes.
		/// </summary>
		/// <param name="stb">The <see cref="System.Text.StringBuilder"/> used to collect the table notes.</param>
		/// <param name="sLine">The line of ASCII text.</param>
		private static void AppendLineToTableNotes(System.Text.StringBuilder stb, string sLine)
		{
			stb.Append(sLine);
			stb.AppendLine();
		}

		private static AsciiDocumentAnalysisOptions GetDefaultAsciiDocumentAnalysisOptions(DataTable dataTable)
		{
			AsciiDocumentAnalysisOptions result = null;
			if (null != dataTable)
				result = dataTable.GetPropertyValue(AsciiDocumentAnalysisOptions.PropertyKeyAsciiDocumentAnalysisOptions, null);
			if (null == result)
				result = Current.PropertyService.GetValue(AsciiDocumentAnalysisOptions.PropertyKeyAsciiDocumentAnalysisOptions, Main.Services.RuntimePropertyKind.UserAndApplicationAndBuiltin);
			return result;
		}

		/// <summary>
		/// Adds (if not already present) or updates (if present) the ASCII import data source for the provided table. This can also mean that the data source of the table is set to null,
		/// for instance if the provided streamOriginHint is not a file Url.
		/// </summary>
		/// <param name="dataTable">The provided data table on which to set the import data source..</param>
		/// <param name="streamOriginHint">The stream origin hint. If this is a file, then this string starts with <see cref="FileUrlStart"/>.</param>
		/// <param name="importOptions">The Ascii import options that were used to import the file.</param>
		private static void AddOrUpdateAsciiImportDataSource(DataTable dataTable, string streamOriginHint, AsciiImportOptions importOptions)
		{
			string fileName = streamOriginHint.StartsWith(FileUrlStart) ? streamOriginHint.Substring(FileUrlStart.Length) : null;
			var dataSource = dataTable.DataSource as AsciiImportDataSource;

			if (!string.IsNullOrEmpty(fileName))
			{
				if (null != dataSource)
				{
					dataSource.SourceFileName = fileName;
					dataSource.AsciiImportOptions = importOptions;
				}
				else
				{
					dataTable.DataSource = new AsciiImportDataSource(streamOriginHint.Substring(FileUrlStart.Length), importOptions);
				}
			}
			else
			{
				dataTable.DataSource = null;
			}
		}

		#endregion Private helper functions
	} // end class
}