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

using Altaxo.Data;

namespace Altaxo.Serialization.Ascii
{
	/// <summary>
	/// Central class for import of ascii data.
	/// </summary>
	public class AsciiImporter
	{
		static AsciiImportOptions _defaultImportOptions = new AsciiImportOptions();

		protected System.IO.Stream _stream;

		/// <summary>If set, this string designates the origin of the stream. This can be e.g. the filename of the file, the clipboard etc.</summary>
		private string _streamOriginHint;

		/// <summary>
		/// Constructor. You have to provide a stream here. Afterwards, you must call one of the methods, for instance
		/// Analyze if you are not sure about the structure of the ascii data, or ImportAscii if you know the import options.
		/// </summary>
		/// <param name="stream">Stream. This stream must be readable, and seekable.</param>
		/// <param name="streamOriginHint">Designates a short hint where the provided stream originates from. Can be <c>Null</c> if the origin is unknown.</param>
		public AsciiImporter(System.IO.Stream stream, string streamOriginHint)
		{
			this._stream = stream;
			this._streamOriginHint = streamOriginHint;
		}

		public void ImportAscii(AsciiImportOptions impopt, Altaxo.Data.DataTable table)
		{
			if (impopt == null)
				throw new ArgumentNullException("Argument importOptions is null");
			if (!impopt.IsFullySpecified)
				throw new ArgumentException("Argument importOptions: importOptions must be fully specified, i.e. all elements of importOptions must be valid. Please run a document analysis in-before to get appropriate values.");
			if (null == table)
				throw new ArgumentNullException("Argument table is null");

			string sLine;
			_stream.Position = 0; // rewind the stream to the beginning
			System.IO.StreamReader sr = new System.IO.StreamReader(_stream, System.Text.Encoding.Default, true);
			Altaxo.Data.DataColumnCollection newcols = new Altaxo.Data.DataColumnCollection();

			Altaxo.Data.DataColumnCollection newpropcols = new Altaxo.Data.DataColumnCollection();

			// in case a structure is provided, allocate already the columsn

			if (null != impopt.RecognizedStructure)
			{
				for (int i = 0; i < impopt.RecognizedStructure.Count; i++)
				{
					switch (impopt.RecognizedStructure[i])
					{
						case AsciiColumnType.Double:
							newcols.Add(new Altaxo.Data.DoubleColumn());
							break;
						case AsciiColumnType.Int64:
							newcols.Add(new Altaxo.Data.DoubleColumn());
							break;
						case AsciiColumnType.DateTime:
							newcols.Add(new Altaxo.Data.DateTimeColumn());
							break;
						case AsciiColumnType.Text:
							newcols.Add(new Altaxo.Data.TextColumn());
							break;
						case AsciiColumnType.DBNull:
							newcols.Add(new Altaxo.Data.DBNullColumn());
							break;
						default:
							throw new ArgumentOutOfRangeException("Unconsidered AsciiColumnType: " + impopt.RecognizedStructure[i].ToString());
					}
				}
			}

			// add also additional property columns if not enough there
			if (impopt.NumberOfMainHeaderLines.HasValue && impopt.NumberOfMainHeaderLines.Value > 0) // if there are more than one header line, allocate also property columns
			{
				int toAdd = impopt.NumberOfMainHeaderLines.Value;
				for (int i = 0; i < toAdd; i++)
					newpropcols.Add(new Data.TextColumn());
			}

			// if decimal separator statistics is provided by impopt, create a number format info object
			System.Globalization.NumberFormatInfo numberFormatInfo = impopt.NumberFormatCulture.NumberFormat;
			System.Globalization.DateTimeFormatInfo dateTimeFormat = impopt.DateTimeFormatCulture.DateTimeFormat;


			var notesHeader = new System.Text.StringBuilder();
			notesHeader.Append("Imported");
			if (!string.IsNullOrEmpty(_streamOriginHint))
				notesHeader.AppendFormat(" from {0}", _streamOriginHint);
			notesHeader.AppendFormat(" at {0}", DateTime.Now);
			notesHeader.AppendLine();


			// first of all, read the header if existent
			for (int i = 0; i < impopt.NumberOfMainHeaderLines; i++)
			{
				sLine = sr.ReadLine();
				if (null == sLine) break;

				var tokens = new List<string>(impopt.SeparationStrategy.GetTokens(sLine));
				if (i == impopt.IndexOfCaptionLine) // is it the column name line
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

				switch (impopt.HeaderLinesDestination)
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
						if (tokens.Count == impopt.RecognizedStructure.Count)
							FillPropertyColumnWithTokens(newpropcols[i], tokens);
						else
							AppendLineToTableNotes(notesHeader, sLine);
						break;
					case AsciiHeaderLinesDestination.ImportToPropertiesAndNotes:
						FillPropertyColumnWithTokens(newpropcols[i], tokens);
						AppendLineToTableNotes(notesHeader, sLine);
						break;
					default:
						throw new ArgumentOutOfRangeException("Unknown switch case: " + impopt.HeaderLinesDestination.ToString());
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
				foreach (string token in impopt.SeparationStrategy.GetTokens(sLine))
				{
					k++;
					if (k >= maxcolumns)
						break;

					if (string.IsNullOrEmpty(token))
						continue;

					if (newcols[k] is Altaxo.Data.DoubleColumn)
					{
						double val;
						if (double.TryParse(token, System.Globalization.NumberStyles.Any, numberFormatInfo, out val))
							((Altaxo.Data.DoubleColumn)newcols[k])[i] = val;
					}
					else if (newcols[k] is Altaxo.Data.DateTimeColumn)
					{
						DateTime val;
						if (DateTime.TryParse(token, dateTimeFormat, System.Globalization.DateTimeStyles.NoCurrentDateDefault, out val))
							((Altaxo.Data.DateTimeColumn)newcols[k])[i] = val;
					}
					else if (newcols[k] is Altaxo.Data.TextColumn)
					{
						((Altaxo.Data.TextColumn)newcols[k])[i] = token.Trim();
					}
					else if (null == newcols[k] || newcols[k] is Altaxo.Data.DBNullColumn)
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
							Altaxo.Data.DoubleColumn newc = new Altaxo.Data.DoubleColumn();
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
								Altaxo.Data.DateTimeColumn newc = new Altaxo.Data.DateTimeColumn();
								newc[i] = valDateTime;

								newcols.Replace(k, newc);
							}
							else
							{
								Altaxo.Data.TextColumn newc = new Altaxo.Data.TextColumn();
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
				if (newcols[i] is Altaxo.Data.DBNullColumn) // if the type is undefined, use a new DoubleColumn
					table.DataColumns.CopyOrReplaceOrAdd(i, new Altaxo.Data.DoubleColumn(), newcols.GetColumnName(i));
				else
					table.DataColumns.CopyOrReplaceOrAdd(i, newcols[i], newcols.GetColumnName(i));

				// set the first column as x-column if the table was empty before, and there are more than one column
				if (i == 0 && tableWasEmptyBefore && newcols.ColumnCount > 1)
					table.DataColumns.SetColumnKind(0, Altaxo.Data.ColumnKind.X);

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
		/// Fills the property column with tokens.
		/// </summary>
		/// <param name="newpropcol">The property column to fill.</param>
		/// <param name="tokens">The text tokens.</param>
		private static void FillPropertyColumnWithTokens(Altaxo.Data.DataColumn newpropcol, List<string> tokens)
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

		/// <summary>
		/// Helper function. Gets an <see cref="System.IO.FileStream"/> by providing a file name. The stream is opened with read access and with the FileShare.Read flag.
		/// </summary>
		/// <param name="filename">The filename.</param>
		/// <returns>The stream. You are responsible for closing / disposing this stream.</returns>
		public static System.IO.FileStream GetAsciiInputFileStream(string filename)
		{
			return new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
		}


		/// <summary>
		/// Imports ascii from a string into a table. Returns null (!) if nothing is imported.
		/// </summary>
		/// <param name="text">The text to import as ascii.</param>
		/// <returns>The table representation of the imported text, or null if nothing is imported.</returns>
		public static Altaxo.Data.DataTable ImportText(string text)
		{
			System.IO.MemoryStream memstream = new System.IO.MemoryStream();
			System.IO.TextWriter textwriter = new System.IO.StreamWriter(memstream);
			textwriter.Write(text);
			textwriter.Flush();
			memstream.Position = 0;

			Altaxo.Data.DataTable table = new Altaxo.Data.DataTable();
			var importer = new Altaxo.Serialization.Ascii.AsciiImporter(memstream, "text");

			Altaxo.Serialization.Ascii.AsciiImportOptions options = AsciiDocumentAnalysis.Analyze(_defaultImportOptions, memstream);

			if (options != null)
			{
				importer.ImportAscii(options, table);
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
		public static Altaxo.Data.DataTable Import(System.IO.Stream stream, string streamOriginHint)
		{
			var defaultImportOptions = new Altaxo.Serialization.Ascii.AsciiImportOptions();
			return Import(stream, streamOriginHint, defaultImportOptions);
		}

		/// <summary>
		/// Imports ascii from a memory stream into a table. Returns null (!) if nothing is imported.
		/// </summary>
		/// <param name="stream">The stream to import ascii from. Is not (!) closed at the end of this function.</param>
		/// <param name="streamOriginHint">Designates a short hint where the provided stream originates from. Can be <c>Null</c> if the origin is unknown.</param>
		/// <param name="defaultImportOptions">The default import options. The importer uses this options as base, but updates some fields by analyzing the data to import.</param>
		/// <returns>The table representation of the imported text, or null if nothing is imported.</returns>
		public static Altaxo.Data.DataTable Import(System.IO.Stream stream, string streamOriginHint, Altaxo.Serialization.Ascii.AsciiImportOptions defaultImportOptions)
		{
			Altaxo.Data.DataTable table = new Altaxo.Data.DataTable();
			var importer = new Altaxo.Serialization.Ascii.AsciiImporter(stream, streamOriginHint);
			Altaxo.Serialization.Ascii.AsciiImportOptions options = AsciiDocumentAnalysis.Analyze(defaultImportOptions, stream);
			if (options != null)
			{
				importer.ImportAscii(options, table);
				return table;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Imports ascii from a memory stream into a table. Returns null (!) if nothing is imported.
		/// </summary>
		/// <param name="filename">The file name of the file from which to import.</param>
		/// <returns>The table representation of the imported text, or null if nothing is imported.</returns>
		public static Altaxo.Data.DataTable ImportFile(string filename)
		{
			using (var str = GetAsciiInputFileStream((filename)))
			{
				return Import(str, "file: " + filename);
			}
		}

		/// <summary>
		/// Imports ascii from a memory stream into a table. Returns null (!) if nothing is imported.
		/// </summary>
		/// <param name="filename">The file name of the file from which to import.</param>
		/// <param name="separatorChar">The character used to separate the columns</param>
		/// <returns>The table representation of the imported text, or null if nothing is imported.</returns>
		public static Altaxo.Data.DataTable ImportFile(string filename, char separatorChar)
		{
			var defaultImportOptions = new Altaxo.Serialization.Ascii.AsciiImportOptions();
			defaultImportOptions.SeparationStrategy = new SingleCharSeparationStrategy(separatorChar);
			using (var str = GetAsciiInputFileStream(filename))
			{
				return Import(str, "file: " + filename, defaultImportOptions);
			}
		}


		/// <summary>
		/// Imports Ascii data from a stream into the data table.
		/// </summary>
		/// <param name="myStream">The stream to import from.</param>
		/// <param name="streamOriginHint">Designates a short hint where the provided stream originates from. Can be <c>Null</c> if the origin is unknown.</param>
		/// <param name="dataTable">The table where to import into.</param>
		public static void Import(System.IO.Stream myStream, string streamOriginHint, DataTable dataTable)
		{
			Import(myStream, streamOriginHint, dataTable, null);
		}

		/// <summary>
		/// Imports Ascii data from a stream into the data table.
		/// </summary>
		/// <param name="myStream">The stream to import from.</param>
		/// <param name="streamOriginHint">Designates a short hint where the provided stream originates from. Can be <c>Null</c> if the origin is unknown.</param>
		/// <param name="dataTable">The table where to import into.</param>
		/// <param name="importOptions">The options used to import ASCII. If this parameter is <c>null</c>, an analysis of the stream is done in order to determine the import stragegy to use.</param>
		public static void Import(System.IO.Stream myStream, string streamOriginHint, DataTable dataTable, AsciiImportOptions importOptions)
		{
			var importer = new AsciiImporter(myStream, streamOriginHint);
			if (null != importOptions)
			{
				importer.ImportAscii(importOptions, dataTable);
			}
			else
			{
				var recognizedOptions = AsciiDocumentAnalysis.Analyze(_defaultImportOptions, myStream);
				importer.ImportAscii(recognizedOptions, dataTable);
			}
		}


		/// <summary>
		/// Compare the values in a double array with values in a double column and see if they match.
		/// </summary>
		/// <param name="values">An array of double values.</param>
		/// <param name="col">A double column to compare with the double array.</param>
		/// <returns>True if the length of the array is equal to the length of the <see cref="Altaxo.Data.DoubleColumn" /> and the values in 
		/// both array match to each other, otherwise false.</returns>
		public static bool ValuesMatch(Altaxo.Data.DataColumn values, Altaxo.Data.DataColumn col)
		{
			if (values.Count != col.Count)
				return false;

			for (int i = 0; i < values.Count; i++)
				if (col[i] != values[i])
					return false;

			return true;
		}


		/// <summary>
		/// Imports a couple of ASCII files into one (!) table. The first column of each file is considered to be the x-column, and if they match another x-column, the newly imported columns will get the same column group.
		/// </summary>
		/// <param name="filenames">An array of filenames to import.</param>
		/// <param name="table">The table the data should be imported to.</param>
		/// <param name="importOptions">Options used to import ASCII. This parameter can be <c>null</c>. In this case the options are determined by analysis of each file.</param>
		/// <returns>Null if no error occurs, or an error description.</returns>
		public static string ImportMultipleAsciiHorizontally(string[] filenames, Altaxo.Data.DataTable table, AsciiImportOptions importOptions)
		{
			Altaxo.Data.DataColumn xcol = null;
			Altaxo.Data.DataColumn xvalues;
			System.Text.StringBuilder errorList = new System.Text.StringBuilder();
			int lastColumnGroup = 0;

			if (table.DataColumns.ColumnCount > 0)
			{
				lastColumnGroup = table.DataColumns.GetColumnGroup(table.DataColumns.ColumnCount - 1);
				Altaxo.Data.DataColumn xColumnOfRightMost = table.DataColumns.FindXColumnOfGroup(lastColumnGroup);
				xcol = (Altaxo.Data.DoubleColumn)xColumnOfRightMost;
			}

			// add also a property column named "FilePath" if not existing so far
			Altaxo.Data.TextColumn filePathPropCol = (Altaxo.Data.TextColumn)table.PropCols.EnsureExistence("FilePath", typeof(Altaxo.Data.TextColumn), Altaxo.Data.ColumnKind.Label, 0);

			foreach (string filename in filenames)
			{
				Altaxo.Data.DataTable newtable = null;
				using (var stream = GetAsciiInputFileStream(filename))
				{
					newtable = new DataTable();
					Import(stream, "file: " + filename, newtable, importOptions);
					stream.Close();
				}

				if (newtable.DataColumns.ColumnCount == 0)
					continue;


				xvalues = newtable.DataColumns[0];
				bool bMatchsXColumn = false;

				// first look if our default xcolumn matches the xvalues
				if (null != xcol)
					bMatchsXColumn = ValuesMatch(xvalues, xcol);

				// if no match, then consider all xcolumns from right to left, maybe some fits
				if (!bMatchsXColumn)
				{
					for (int ncol = table.DataColumns.ColumnCount - 1; ncol >= 0; ncol--)
					{
						if ((Altaxo.Data.ColumnKind.X == table.DataColumns.GetColumnKind(ncol)) &&
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
					xcol = (Altaxo.Data.DataColumn)xvalues.Clone();
					lastColumnGroup = table.DataColumns.GetUnusedColumnGroupNumber();
					table.DataColumns.Add(xcol, newtable.DataColumns.GetColumnName(0), Altaxo.Data.ColumnKind.X, lastColumnGroup);
				}

				for (int i = 1; i < newtable.DataColumns.ColumnCount; i++)
				{
					// now add the y-values
					Altaxo.Data.DataColumn ycol = (Altaxo.Data.DataColumn)newtable.DataColumns[i].Clone();
					table.DataColumns.Add(ycol,
					table.DataColumns.FindUniqueColumnName(newtable.DataColumns.GetColumnName(i)),
						Altaxo.Data.ColumnKind.V,
						lastColumnGroup);


					// now set the file name property cell
					int destcolnumber = table.DataColumns.GetColumnNumber(ycol);
					filePathPropCol[destcolnumber] = filename;

					// now set the imported property cells
					for (int s = 0; s < newtable.PropCols.ColumnCount; s++)
					{
						Altaxo.Data.DataColumn dest = table.PropCols.EnsureExistence(newtable.PropCols.GetColumnName(s), newtable.PropCols[s].GetType(), Altaxo.Data.ColumnKind.V, 0);
						dest.SetValueAt(destcolnumber, newtable.PropCols[s][i]);
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
		/// <param name="filenames">An array of filenames to import.</param>
		/// <param name="table">The table the data should be imported to.</param>
		/// <param name="importOptions">Options used to import ASCII. This parameter can be <c>null</c>. In this case the options are determined by analysis of each file.</param>
		/// <returns>Null if no error occurs, or an error description.</returns>
		public static string ImportMultipleAsciiVertically(string[] filenames, Altaxo.Data.DataTable table, AsciiImportOptions importOptions)
		{
			System.Text.StringBuilder errorList = new System.Text.StringBuilder();
			int lastDestinationRow = table.DataColumns.RowCount;
			int numberOfImportedTables = 0;



			// add also a property column named "FilePath" if not existing so far
			Altaxo.Data.TextColumn filePathCol = (Altaxo.Data.TextColumn)table.Col.EnsureExistence("FilePath", typeof(Altaxo.Data.TextColumn), Altaxo.Data.ColumnKind.Label, 0);

			foreach (string filename in filenames)
			{
				Altaxo.Data.DataTable srctable = null;
				using (var stream = GetAsciiInputFileStream(filename))
				{
					srctable = new DataTable();
					Import(stream, "file " + filename, srctable, importOptions);
					stream.Close();
				}

				if (srctable.DataColumns.ColumnCount == 0)
					continue;

				// mark the beginning of the new file with the file path
				filePathCol[lastDestinationRow] = filename;

				// transfer the data columns
				for (int srcDataColIdx = 0; srcDataColIdx < srctable.DataColumns.ColumnCount; srcDataColIdx++)
				{
					var srcDataCol = srctable.DataColumns[srcDataColIdx];

					var destDataCol = table.DataColumns.EnsureExistence(srctable.DataColumns.GetColumnName(srcDataColIdx), srctable.DataColumns[srcDataColIdx].GetType(), srctable.DataColumns.GetColumnKind(srcDataColIdx), srctable.DataColumns.GetColumnGroup(srcDataColIdx));
					int destDataColIdx = table.DataColumns.GetColumnNumber(destDataCol);

					// transfer the data of one data column
					for (int j = 0; j < srcDataCol.Count; j++)
						destDataCol[lastDestinationRow + j] = srcDataCol[j];


					// now also process the property columns
					for (int srcPropColIdx = 0; srcPropColIdx < srctable.PropCols.ColumnCount; srcPropColIdx++)
					{
						var destPropCol = table.PropCols.EnsureExistence(srctable.PropCols.GetColumnName(srcPropColIdx), srctable.PropCols[srcPropColIdx].GetType(), srctable.PropCols.GetColumnKind(srcPropColIdx), srctable.PropCols.GetColumnGroup(srcPropColIdx));

						if (0 == numberOfImportedTables)
						{
							destPropCol[destDataColIdx] = srctable.PropCols[srcPropColIdx][srcDataColIdx];
						}
						else if (destPropCol[destDataColIdx] != srctable.PropCols[srcPropColIdx][srcDataColIdx])
						{
							destPropCol.SetElementEmpty(destDataColIdx);
						}
					}
				}

				lastDestinationRow += srctable.DataColumns.RowCount;
				numberOfImportedTables++;
			} // foreache file


			return errorList.Length == 0 ? null : errorList.ToString();
		}


		/// <summary>
		/// Imports multiple Ascii files into the provided table and additionally created tables.
		/// </summary>
		/// <param name="dataTable">The data table where the first ascii file is imported to. Can be null.</param>
		/// <param name="filenames">The names of the files to import.</param>
		/// <param name="importOptions">Options used to import ASCII. This parameter can be <c>null</c>. In this case the options are determined by analyzation of each file.</param>
		public static void ImportAsciiToMultipleWorksheets(string[] filenames, DataTable dataTable, AsciiImportOptions importOptions)
		{
			int startrest = 0;

			Array.Sort(filenames); // Windows seems to store the filenames reverse to the clicking order or in arbitrary order

			if (dataTable != null)
			{
				using (var myStream = GetAsciiInputFileStream(filenames[0]))
				{
					Import(myStream, "file " + filenames[0],  dataTable, importOptions);
					myStream.Close();
					startrest = 1;
				}
			}

			// import also the other files, but this time we create new tables
			for (int i = startrest; i < filenames.Length; i++)
			{
				using (var myStream = GetAsciiInputFileStream(filenames[i]))
				{
					var newTable = new DataTable(System.IO.Path.GetFileNameWithoutExtension(filenames[i]));
					Current.ProjectService.CreateNewWorksheet(newTable);
					Import(myStream, "file " + filenames[i], newTable, importOptions);
					myStream.Close();
				}
			} // for all files
		}



	} // end class 
}
