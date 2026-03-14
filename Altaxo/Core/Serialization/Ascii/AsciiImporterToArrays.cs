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

#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Altaxo.Serialization.Ascii
{
  /// <summary>
  /// Central class for import of ascii data.
  /// </summary>
  public class AsciiImporterToArrays
  {
    /// <summary>Prepend this string to a file name in order to designate the stream origin as file name origin.</summary>
    public const string FileUrlStart = @"file:///";

    /// <summary>
    /// Gets the names of the imported columns.
    /// </summary>
    public IReadOnlyList<string> ColumnNames { get; private set; } = [];

    /// <summary>
    /// Gets the imported data columns.
    /// </summary>
    public IReadOnlyList<Array> DataColumns { get; private set; } = [];

    /// <summary>
    /// Gets the imported column property columns.
    /// </summary>
    public IReadOnlyList<Array> ColumnProperties { get; private set; } = [];

    /// <summary>
    /// Gets the notes created during import.
    /// </summary>
    public string Notes { get; private set; } = string.Empty;

    #region From stream

    /// <summary>
    /// Imports an Ascii stream into a table. The import options have to be known already.
    /// </summary>
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
    private void InternalImportFromAsciiStream(Stream stream, string streamOriginHint, [NotNull] ref AsciiImportOptions? importOptions)
    {
      if (importOptions is null || !importOptions.IsFullySpecified)
      {
        var analysisOptions = AsciiDocumentAnalysisOptions.GetDefaultSystemOptions();
        importOptions = AsciiDocumentAnalysis.Analyze(importOptions ?? new AsciiImportOptions(), stream, analysisOptions);
      }

      if (importOptions is null)
        throw new InvalidDataException("Import options could not be determined from the data stream. Possibly, the data stream is empty or it is not an Ascii data stream");
      if (!importOptions.IsFullySpecified)
        throw new InvalidDataException("Import options could not be fully determined from the data stream. Possibly, the data stream is empty or it is not an Ascii data stream");

      string? sLine;
      stream.Position = 0; // rewind the stream to the beginning
      var sr = new StreamReader(stream, importOptions.Encoding, importOptions.DetectEncodingFromByteOrderMarks);

      var newcolsName = new List<string>();
      var newcols = new List<object>();
      var newpropcols = new List<object>();

      // in case a structure is provided, allocate already the columsn

      if (importOptions.RecognizedStructure is not null)
      {
        for (int i = 0; i < importOptions.RecognizedStructure.Count; i++)
        {
          newcolsName.Add($"Column{i}");
          switch (importOptions.RecognizedStructure[i].ColumnType)
          {
            case AsciiColumnType.Double:
              newcols.Add(new List<double>());
              break;

            case AsciiColumnType.Int64:
              newcols.Add(new List<double>());
              break;

            case AsciiColumnType.DateTime:
              newcols.Add(new List<DateTime>());
              break;

            case AsciiColumnType.Text:
              newcols.Add(new List<string>());
              break;

            case AsciiColumnType.DBNull:
              newcols.Add(new List<object>());
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
          newpropcols.Add(new List<string>());
      }

      // if decimal separator statistics is provided by impopt, create a number format info object
      System.Globalization.NumberFormatInfo numberFormatInfo = importOptions.NumberFormatCulture!.NumberFormat;
      System.Globalization.DateTimeFormatInfo dateTimeFormat = importOptions.DateTimeFormatCulture!.DateTimeFormat;

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
        if (sLine is null)
          break;

        var tokens = new List<string>(importOptions.SeparationStrategy!.GetTokens(sLine));
        if (i == importOptions.IndexOfCaptionLine) // is it the column name line
        {
          for (int k = 0; k < Math.Min(tokens.Count, newcols.Count); ++k)
          {
            var ttoken = tokens[k].Trim();
            if (!string.IsNullOrEmpty(ttoken))
            {
              string newcolname = FindUniqueColumnName(ttoken, newcolsName);
              newcolsName[k] = newcolname;
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
            if (tokens.Count == importOptions.RecognizedStructure!.Count)
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
        if (sLine is null)
          break;
        else if ("\0" == sLine) // if pasting from excel, the stream ends with "\0", so we ignore it.
          continue;

        int maxcolumns = newcols.Count;

        int k = -1;
        foreach (string token in importOptions.SeparationStrategy!.GetTokens(sLine))
        {
          k++;
          if (k >= maxcolumns)
            break;

          if (string.IsNullOrEmpty(token))
            continue;

          if (newcols[k] is List<double>)
          {
            if (double.TryParse(token, System.Globalization.NumberStyles.Any, numberFormatInfo, out var val))
              ((List<double>)newcols[k])[i] = val;
          }
          else if (newcols[k] is List<DateTime>)
          {
            if (DateTime.TryParse(token, dateTimeFormat, System.Globalization.DateTimeStyles.NoCurrentDateDefault, out var val))
              ((List<DateTime>)newcols[k])[i] = val;
          }
          else if (newcols[k] is List<string>)
          {
            ((List<string>)newcols[k])[i] = token.Trim();
          }
          else if (newcols[k] is null || newcols[k] is List<object>)
          {
            bool bConverted = false;
            double val = double.NaN;
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
              var newc = new List<double>();
              newc[i] = val;
              newcols[k] = newc;
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
                var newc = new List<DateTime>();
                newc[i] = valDateTime;
                newcols[k] = newc;
              }
              else
              {
                var newc = new List<string>();
                newc[i] = token;
                newcols[k] = newc;
              }
            } // end outer if null==newcol
          }
        } // end of for all cols
      } // end of for all lines

      ColumnNames = newcolsName;

      var newcolsArrays = new List<Array>();
      for (int i = 0; i < newcols.Count; i++)
      {
        if (newcols[i] is List<double> doubleList)
          newcolsArrays.Add(doubleList.ToArray());
        else if (newcols[i] is List<DateTime> dateTimeList)
          newcolsArrays.Add(dateTimeList.ToArray());
        else if (newcols[i] is List<string> stringList)
          newcolsArrays.Add(stringList.ToArray());
        else if (newcols[i] is List<object> objectList)
          newcolsArrays.Add(objectList.ToArray());
      }
      DataColumns = newcolsArrays;

      var newpropcolsArrays = new List<Array>();
      for (int i = 0; i < newpropcols.Count; i++)
      {
        if (newpropcols[i] is List<string> stringList)
          newpropcolsArrays.Add(stringList.ToArray());
        else if (newpropcols[i] is List<double> doubleList)
          newpropcolsArrays.Add(doubleList.ToArray());
        else if (newpropcols[i] is List<DateTime> dateTimeList)
          newpropcolsArrays.Add(dateTimeList.ToArray());
        else if (newpropcols[i] is List<object> objectList)
          newpropcolsArrays.Add(objectList.ToArray());
      }
      ColumnProperties = newpropcolsArrays;

      Notes = notesHeader.ToString();

    } // end of function ImportAscii

    /// <summary>
    /// Imports an Ascii stream into a table. The import options have to be known already.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="streamOriginHint">Stream origin hint. If the stream was opened from a file, you should prepend <see cref=" FileUrlStart"/> to the file name.</param>
    /// <param name="importOptions">The Ascii import options. This parameter must not be <c>null</c>. If the provided options are not fully specified, it is tried to analyse the stream to get fully specified options.</param>
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
    public void ImportFromAsciiStream(Stream stream, string streamOriginHint, AsciiImportOptions importOptions)
    {
      if (importOptions is null)
        throw new ArgumentNullException("Argument importOptions is null");

      InternalImportFromAsciiStream(stream, streamOriginHint, ref importOptions);
    }

    /// <summary>
    /// Imports Ascii data from a stream into the data table.
    /// </summary>
    /// <param name="stream">The stream to import from.</param>
    /// <param name="streamOriginHint">Designates a short hint where the provided stream originates from. Can be <c>Null</c> if the origin is unknown.</param>
    /// <param name="importOptions">On return, contains the recognized import options that were used to import from the provided stream.</param>
    public void ImportFromAsciiStream(Stream stream, string streamOriginHint, out AsciiImportOptions importOptions)
    {
      AsciiImportOptions? impOptions = null;
      InternalImportFromAsciiStream(stream, streamOriginHint, ref impOptions);
      importOptions = impOptions;

    }

    /// <summary>
    /// Imports Ascii data from a stream into the data table.
    /// </summary>
    /// <param name="stream">The stream to import from.</param>
    /// <param name="streamOriginHint">Designates a hint where the provided stream originates from. Can be <c>Null</c> if the origin is unknown.</param>
    public void ImportFromAsciiStream(Stream stream, string streamOriginHint)
    {
      ImportFromAsciiStream(stream, streamOriginHint, out var dummy);
    }

    #endregion From stream

    #region From single file

    /// <summary>
    /// Imports from an ASCII file into an existing table. The import options have to be known already.
    /// </summary>
    /// <param name="dataTable">The data table to import into.</param>
    /// <param name="fileName">File name of the file to import.</param>
    /// <param name="importOptions">The import options. This parameter must not be null.</param>
    public void ImportFromAsciiFile(string fileName, AsciiImportOptions importOptions)
    {
      if (string.IsNullOrEmpty(fileName))
        throw new ArgumentNullException("Argument fileName is null or empty");
      if (importOptions is null)
        throw new ArgumentNullException("Argument importOptions is null");

      using (var myStream = GetAsciiInputFileStream(fileName))
      {
        ImportFromAsciiStream(myStream, FileUrlStart + fileName, importOptions);
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
    public void ImportFromAsciiFile(string fileName, out AsciiImportOptions importOptions)
    {
      if (string.IsNullOrEmpty(fileName))
        throw new ArgumentNullException("Argument fileName is null or empty");

      using (var myStream = GetAsciiInputFileStream(fileName))
      {
        ImportFromAsciiStream(myStream, FileUrlStart + fileName, out importOptions);
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
    public void ImportFromAsciiFile(string fileName)
    {
      if (string.IsNullOrEmpty(fileName))
        throw new ArgumentNullException("Argument fileName is null or empty");

      ImportFromAsciiFile(fileName, out var importOptions);
    }

    #endregion From single file

    #region From text

    /// <summary>
    /// Imports from an ASCII text provided as string into an existing table. The import options have to be known already.
    /// </summary>
    /// <param name="dataTable">The data table to import into.</param>
    /// <param name="asciiText">The Ascii text that is to be imported.</param>
    /// <param name="importOptions">The import options. This parameter must not be null, and the options must be fully specified.</param>
    public void ImportFromAsciiText(string asciiText, AsciiImportOptions importOptions)
    {
      if (asciiText is null)
        throw new ArgumentNullException("Argument asciiText is null");
      if (importOptions is null)
        throw new ArgumentNullException("Argument importOptions is null");

      using (var memstream = new MemoryStream())
      {
        using (var textwriter = new StreamWriter(memstream, System.Text.Encoding.UTF8, 512, true))
        {
          textwriter.Write(asciiText);
          textwriter.Flush();
        }

        memstream.Position = 0;
        ImportFromAsciiStream(memstream, "Ascii text", importOptions);
      }
    }

    /// <summary>
    /// Imports from an ASCII text provided as string into an existing table.
    /// </summary>
    /// <param name="dataTable">The data table to import into.</param>
    /// <param name="asciiText">The Ascii text that is to be imported.</param>
    /// <param name="importOptions">On return, contains the import options that were used to import the Ascii text.</param>
    public void ImportFromAsciiText(string asciiText, out AsciiImportOptions importOptions)
    {
      if (asciiText is null)
        throw new ArgumentNullException("Argument asciiText is null");

      using (var memstream = new MemoryStream())
      {
        using (var textwriter = new StreamWriter(memstream, System.Text.Encoding.UTF8, 512, true))
        {
          textwriter.Write(asciiText);
          textwriter.Flush();
        }
        memstream.Position = 0;
        ImportFromAsciiStream(memstream, "Ascii text", out importOptions);
      }
    }

    /// <summary>
    /// Imports from an ASCII text provided as string into an existing table.
    /// </summary>
    /// <param name="dataTable">The data table to import into.</param>
    /// <param name="asciiText">The Ascii text that is to be imported.</param>
    public void ImportFromAsciiText(string asciiText)
    {
      if (asciiText is null)
        throw new ArgumentNullException("Argument asciiText is null");

      ImportFromAsciiText(asciiText, out var importOptions);
    }

    #endregion From text

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
    /// Get a unique column name based on a provided string.
    /// </summary>
    /// <param name="sbase">The base name. Can be null.</param>
    /// <returns>An unique column name based on the provided string.</returns>
    public string FindUniqueColumnName(string? sbase, List<string> columnNames)
    {
      var columnNamesSet = new HashSet<string>(columnNames);
      return sbase is null ? FindUniqueColumnNameWithoutBase(columnNames, columnNamesSet) : FindUniqueColumnNameWithBase(sbase, columnNamesSet);
    }

    /// <summary>
    /// Get a unique column name based on regular naming from A to ZZ.
    /// </summary>
    /// <returns>An unique column name based on regular naming.</returns>
    protected string FindUniqueColumnNameWithoutBase(List<string> columnNames, HashSet<string> columnNamesSet, bool triedOutRegularNaming = false)
    {
      string? tryName;
      if (triedOutRegularNaming)
      {
        for (; ; )
        {
          tryName = ((uint)System.Guid.NewGuid().GetHashCode()).ToString("X8");
          if (!columnNames.Contains(tryName))
            return tryName;
        }
      }
      else
      {
        // First try it with the next name after the last column
        tryName = GetNextColumnName(columnNames.Count > 0 ? columnNames[^1] : "");
        if (!string.IsNullOrEmpty(tryName) && !columnNames.Contains(tryName))
          return tryName;

        // then try it with all names from A-ZZ
        for (tryName = "A"; tryName is not null; tryName = GetNextColumnName(tryName))
        {
          if (!columnNames.Contains(tryName))
            return tryName;
        }
        // if no success, set the _TriedOutRegularNaming
        return FindUniqueColumnNameWithoutBase(columnNames, columnNamesSet, triedOutRegularNaming: true);
      }
    }

    /// <summary>
    /// Get a unique column name based on a provided string. If a column with the name of the provided string
    /// already exists, a new name is created by appending a dot and then A-ZZ.
    /// </summary>
    /// <param name="sbase">A string which is the base of the new name. Must not be null!</param>
    /// <returns>An unique column name based on the provided string.</returns>
    protected string FindUniqueColumnNameWithBase(string sbase, HashSet<string> columnNames)
    {
      // try the name directly
      if (!columnNames.Contains(sbase))
        return sbase;

      sbase += ".";

      // then try it with all names from A-ZZ

      for (string? tryAppendix = "A"; tryAppendix is not null; tryAppendix = GetNextColumnName(tryAppendix))
      {
        if (!columnNames.Contains(sbase + tryAppendix))
          return sbase + tryAppendix;
      }

      // if no success, append a hex string
      for (; ; )
      {
        string tryName = sbase + ((uint)System.Guid.NewGuid().GetHashCode()).ToString("X8");
        if (!columnNames.Contains(tryName))
          return tryName;
      }
    }

    /// <summary>
    /// Calculates a new column name dependend on the last name. You have to check whether the returned name is already in use by yourself.
    /// </summary>
    /// <param name="lastName">The last name that was used to name a column.</param>
    /// <returns>The logical next name of a column calculated from the previous name. This name is in the range "A" to "ZZ". If
    /// no further name can be found, this function returns null.</returns>
    public static string? GetNextColumnName(string? lastName)
    {
      if (lastName is null)
        return "A";

      int lastNameLength = lastName.Length;
      if (1 == lastNameLength)
      {
        char _1st = lastName[0];
        _1st++;
        if (_1st >= 'A' && _1st <= 'Z')
          return _1st.ToString();
        else
          return "AA";
      }
      else if (2 == lastNameLength)
      {
        char _1st = lastName[0];
        char _2nd = lastName[1];
        _2nd++;

        if (_2nd < 'A' || _2nd > 'Z')
        {
          _1st++;
          _2nd = 'A';
        }

        if (_1st >= 'A' && _1st <= 'Z')
          return _1st.ToString() + _2nd;
        else
          return null;
      }
      else
      {
        return null;
      }
    }

    /// <summary>
    /// Fills the property column with tokens.
    /// </summary>
    /// <param name="newpropcol">The property column to fill.</param>
    /// <param name="tokens">The text tokens.</param>
    private static void FillPropertyColumnWithTokens(object newpropcol, List<string> tokens)
    {
      for (int k = 0; k < tokens.Count; ++k)
      {
        var ttoken = tokens[k].Trim();
        if (!string.IsNullOrEmpty(ttoken))
        {
          switch (newpropcol)
          {
            case List<string> strCol:
              strCol[k] = ttoken;
              break;
            case List<double> doubleCol:
              doubleCol[k] = double.Parse(ttoken);
              break;
            case List<DateTime> dateTimeCol:
              dateTimeCol[k] = DateTime.Parse(ttoken);
              break;
            case List<object> objectCol:
              objectCol[k] = ttoken; break;
          }
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


    #endregion Private helper functions
  } // end class
}
