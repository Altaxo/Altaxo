#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
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
using System.Globalization;
using System.IO;
using Altaxo.Data;

namespace Altaxo.Serialization.Jcamp
{
  /// <summary>
  /// Reader for Jcamp-Dx files.
  /// </summary>
  public class Import
  {
    private const string XLabelHeader = "##XLABEL=";
    private const string YLabelHeader = "##YLABEL=";
    private const string XUnitHeader = "##XUNITS=";
    private const string YUnitHeader = "##YUNITS=";
    private const string TimeHeader = "##TIME=";
    private const string DateHeader = "##DATE=";
    private const string FirstXHeader = "##FIRSTX=";
    private const string DeltaXHeader = "##DELTAX=";
    private const string XFactorHeader = "##XFACTOR=";
    private const string YFactorHeader = "##YFACTOR=";
    private const string XYBlockHeader = "##XYDATA=";
    private const string BlockEndHeader = "##END=";

    private static readonly char[] splitChars = new char[] { ' ', '\t' };
    private static readonly char[] _plusMinusChars = new char[] { '+', '-' };
    private static readonly char[] _trimCharsDateTime = new char[] { ' ', '\t', ';' };


    protected double _xFirst = double.NaN;
    protected double _xInc = double.NaN;
    protected double _xScale = double.NaN;
    protected double _yScale = double.NaN;


    public double XFirst => _xFirst;
    public double XIncrement => _xInc;
    public double XScale => _xScale;
    public double YScale => _yScale;

    /// <summary>The label of the x-axis.</summary>
    public string? XLabel { get; protected set; } = null;

    /// <summary>The label of the y-axis.</summary>
    public string? YLabel { get; protected set; } = null;

    /// <summary>The unit of the x-axis.</summary>
    public string? XUnit { get; protected set; } = null;

    /// <summary>The unit of the y-axis.</summary>
    public string? YUnit { get; protected set; } = null;

    /// <summary>
    /// Messages about any errors during the import of the Jcamp file.
    /// </summary>
    public string? ErrorMessages { get; protected set; } = null;

    /// <summary>
    /// Creation date/time of the Jcamp file. Be aware that due to different date/time formats, the creation time may be wrong.
    /// If the creation time could not be parsed, the value is <see cref="DateTime.MinValue"/>.
    /// </summary>
    public DateTime CreationTime { get; protected set; } = DateTime.MinValue;

    (double X, double Y)[] _xyValues;
    IReadOnlyList<(double X, double Y)> XYValues => _xyValues;




    /// <summary>
    /// Imports a Jcamp file. The file must not be a multi spectrum file (an exception is thrown in this case).
    /// </summary>
    /// <param name="stream">The stream where to import from.</param>
    /// <returns>Null if successful, otherwise an error description.</returns>
    public Import(System.IO.Stream stream, DataTable? table) : this(new StreamReader(stream))
    {
    }

    /// <summary>
    /// Imports a Jcamp file into an DataTable. The file must not be a multi spectrum file (an exception is thrown in this case).
    /// </summary>
    /// <param name="tr">A <see cref="System.IO.TextReader"/> where to import from.</param>
    /// <returns>Null if successful, otherwise an error description.</returns>
    public Import(TextReader tr)
    {
      DateTime dateValue = DateTime.MinValue;
      DateTime timeValue = DateTime.MinValue;

      var xyValues = new List<(double X, double Y)>();

      try
      {
        string? line;
        int lineCounter = 0;

        do
        {
          line = tr.ReadLine() ?? throw new InvalidDataException("Unexpected end of file");
          lineCounter++;

          try
          {

            if (line.StartsWith(XLabelHeader))
            {
              XLabel = line.Substring(XLabelHeader.Length).Trim();
            }
            else if (line.StartsWith(YLabelHeader))
            {
              YLabel = line.Substring(YLabelHeader.Length).Trim();
            }
            else if (line.StartsWith(XUnitHeader))
            {
              XUnit = line.Substring(XUnitHeader.Length).Trim();
            }
            else if (line.StartsWith(YUnitHeader))
            {
              YUnit = line.Substring(YUnitHeader.Length).Trim();
            }
            else if (line.StartsWith(TimeHeader))
            {
              var timeV = ParseTime(line.Substring(TimeHeader.Length));
              if (timeV.HasValue)
              {
                timeValue = timeV.Value;
              }
            }
            else if (line.StartsWith(DateHeader))
            {
              // Note: Parsing date and time is error prone
              // Maybe it can help to determine the number format first, and on that base try to guess the culture?

              var (dateV, timeV) = HandleDateLine(line.Substring(DateHeader.Length));

              if (dateV.HasValue)
                dateValue = dateV.Value;

              if (timeV.HasValue)
                timeValue = timeV.Value;
            }
            else if (line.StartsWith(FirstXHeader))
            {
              DoubleParse(line.Substring(FirstXHeader.Length), out _xFirst);
            }
            else if (line.StartsWith(DeltaXHeader))
            {
              DoubleParse(line.Substring(DeltaXHeader.Length), out _xInc);
            }
            else if (line.StartsWith(XFactorHeader))
            {
              DoubleParse(line.Substring(XFactorHeader.Length), out _xScale);
            }
            else if (line.StartsWith(YFactorHeader))
            {
              DoubleParse(line.Substring(YFactorHeader.Length), out _yScale);
            }
          }
          catch (Exception ex)
          {
            throw new FormatException($"Line {lineCounter}: {ex.Message}");
          }
        } while (!line.StartsWith(XYBlockHeader));



        // adjust some variables if not given
        if (double.IsNaN(_xInc))
          _xInc = 1;
        if (double.IsNaN(_xScale))
          _xScale = 1;
        if (double.IsNaN(_yScale))
          _yScale = 1;

        CreationTime = DateTime.MinValue;
        if (dateValue != DateTime.MinValue)
          CreationTime = dateValue;
        if (timeValue != DateTime.MinValue)
          CreationTime = CreationTime.Add(timeValue.TimeOfDay);

        if (line.StartsWith(XYBlockHeader))
        {
          for (; ; )
          {
            line = tr.ReadLine();
            lineCounter++;

            if (line is null || line.StartsWith(BlockEndHeader))
              break;
            string[] tokens = line.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 0)
              continue;
            if (tokens.Length == 1)
            {
              // then the line contains of numbers separated only by + or -
              tokens = SplitLineByPlusOrMinus(line);
            }

            // all tokens must contain numeric values, and the first token is the actual x value
            if (!DoubleTryParse(tokens[0], out var xValue))
              throw new FormatException("Non numeric value found in line " + lineCounter.ToString());

            for (int i = 1; i < tokens.Length; i++)
            {
              if (!DoubleTryParse(tokens[i], out var yValue))
                throw new FormatException("Non numeric value found in line" + lineCounter.ToString());

              xyValues.Add((xValue * _xScale, yValue * _yScale));
              xValue += _xInc;
            }
          }
          _xyValues = xyValues.ToArray();
        }
      }
      catch (Exception ex)
      {
        ErrorMessages = (ErrorMessages ?? String.Empty) + ex.Message;
      }
    }

    /// <summary>
    /// Imports the data of this <see cref="Import"/> instance into a <see cref="DataTable"/>.
    /// </summary>
    /// <param name="table">The table.</param>
    public void ToDataTable(DataTable table)
    {
      var xCol = new DoubleColumn();
      var yCol = new DoubleColumn();
      table.DataColumns.Add(xCol, string.IsNullOrEmpty(XLabel) ? "X" : XLabel, ColumnKind.X);
      table.DataColumns.Add(yCol, string.IsNullOrEmpty(YLabel) ? "Y" : YLabel, ColumnKind.V);

      if (CreationTime != DateTime.MinValue)
      {
        table.PropCols.Add(new DateTimeColumn(), "Date", ColumnKind.V);
        table.PropCols["Date"][1] = CreationTime;
      }

      if (!string.IsNullOrEmpty(XUnit) || !string.IsNullOrEmpty(YUnit))
      {
        table.PropCols.Add(new TextColumn(), "Unit", ColumnKind.V);
        table.PropCols["Unit"][0] = XUnit;
        table.PropCols["Unit"][1] = YUnit;
      }

      if (!string.IsNullOrEmpty(XLabel) || !string.IsNullOrEmpty(YLabel))
      {
        table.PropCols.Add(new TextColumn(), "Label", ColumnKind.V);
        table.PropCols["Label"][0] = XLabel;
        table.PropCols["Label"][1] = YLabel;
      }

      for (int i = 0; i < _xyValues.Length; ++i)
      {
        xCol[i] = _xyValues[i].X;
        yCol[i] = _xyValues[i].Y;
      }
    }

    public static string[] SplitLineByPlusOrMinus(string s)
    {
      var list = new List<string>();
      s = s.Trim();
      int idx;
      int start = 0;
      do
      {
        idx = s.IndexOfAny(_plusMinusChars, start + 1);
        if (idx > 1)
        {
          list.Add(s.Substring(start, idx - start));
          start = idx;
        }
        else
        {
          list.Add(s.Substring(start));
        }
      } while (idx >= 0);

      return list.ToArray();
    }


    /// <summary>
    /// Handles the date line (a line that has started with ##DATE=
    /// </summary>
    /// <param name="s">The remaining of the line</param>
    protected (DateTime? dateValue, DateTime? timeValue) HandleDateLine(string s)
    {
      DateTime? time = null;
      DateTime? date = null;
      // In some formats, the TIME string is included here

      var idxTime = s.IndexOf("TIME=");
      if (idxTime >= 0) // Handle the time string
      {
        time = ParseTime(s.Substring(idxTime + "TIME=".Length));
        s = s.Substring(0, idxTime);
      }

      date = ParseDate(s);

      return (date, time);
    }

    public static DateTime? ParseTime(string s)
    {
      s = s.Trim(_trimCharsDateTime);
      DateTime time;
      if (DateTime.TryParseExact(s, "HH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out time))
        return time;
      if (DateTime.TryParseExact(s, "HH:mm:ss.ff", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out time))
        return time;
      if (DateTime.TryParseExact(s, "HH:mm:ss.f", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out time))
        return time;
      if (DateTime.TryParseExact(s, "HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out time))
        return time;
      if (DateTime.TryParseExact(s, "HH-mm-ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out time))
        return time;

      return null;
    }


    public static DateTime? ParseDate(string s)
    {
      s = s.Trim(_trimCharsDateTime);
      DateTime date;
      if (DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out date) && IsDateReasonable(date))
        return date;
      if (DateTime.TryParseExact(s, "yy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out date) && IsDateReasonable(date))
        return date;
      if (DateTime.TryParseExact(s, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out date) && IsDateReasonable(date))
        return date;
      if (DateTime.TryParseExact(s, "dd-MM-yy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out date) && IsDateReasonable(date))
        return date;

      if (DateTime.TryParseExact(s, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out date) && IsDateReasonable(date))
        return date;
      if (DateTime.TryParseExact(s, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out date) && IsDateReasonable(date))
        return date;
      if (DateTime.TryParseExact(s, "MM/dd/yy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out date) && IsDateReasonable(date))
        return date;
      if (DateTime.TryParseExact(s, "dd/MM/yy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out date) && IsDateReasonable(date))
        return date;

      return null;
    }

    public static bool IsDateReasonable(DateTime date)
    {
      if (date.Year < 1980 || date.Year > DateTime.UtcNow.Year)
        return false;
      if (date.Month < 0 || date.Month > 12)
        return false;
      if (date.Day < 0 || date.Day > 31)
        return false;

      return true;
    }

    private static NumberFormatInfo _numberFormatCommaDecimalSeparator = new NumberFormatInfo() { NumberDecimalSeparator = "," };

    public static bool DoubleTryParse(string s, out double x)
    {
      if (double.TryParse(s, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out x))
        return true;

      return double.TryParse(s, NumberStyles.Float, _numberFormatCommaDecimalSeparator, out x);
    }

    public static void DoubleParse(string s, out double x)
    {
      if (double.TryParse(s, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out x))
        return;

      x = double.Parse(s, NumberStyles.Float, _numberFormatCommaDecimalSeparator);
    }


    /// <summary>
    /// Compare the values in a double array with values in a double column and see if they match.
    /// </summary>
    /// <param name="values">An array of double values.</param>
    /// <param name="col">A double column to compare with the double array.</param>
    /// <returns>True if the length of the array is equal to the length of the <see cref="DoubleColumn" /> and the values in
    /// both array match to each other, otherwise false.</returns>
    public static bool ValuesMatch(DoubleColumn values, DoubleColumn col)
    {
      if (values.Count != col.Count)
        return false;

      for (int i = 0; i < values.Count; i++)
        if (col[i] != values[i])
          return false;

      return true;
    }

    /// <summary>
    /// Imports a couple of JCAMP files into a table. The spectra are added as columns to the (one and only) table. If the x column
    /// of the rightmost column does not match the x-data of the spectra, a new x-column is also created.
    /// </summary>
    /// <param name="filenames">An array of filenames to import.</param>
    /// <param name="table">The table the spectra should be imported to.</param>
    /// <returns>Null if no error occurs, or an error description.</returns>
    public static string? ImportJcampFiles(string[] filenames, Altaxo.Data.DataTable table, JcampImportOptions importOptions)
    {
      DoubleColumn? xcol = null;
      DoubleColumn xvalues, yvalues;
      var errorList = new System.Text.StringBuilder();
      int lastColumnGroup = 0;

      if (table.DataColumns.ColumnCount > 0)
      {
        lastColumnGroup = table.DataColumns.GetColumnGroup(table.DataColumns.ColumnCount - 1);
        var xColumnOfRightMost = table.DataColumns.FindXColumnOfGroup(lastColumnGroup);
        if (xColumnOfRightMost is DoubleColumn dcolMostRight)
          xcol = dcolMostRight;
      }

      int idxYColumn = 0;
      foreach (string filename in filenames)
      {
        using var stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
        var localTable = new DataTable();
        var imported = new Import(stream, localTable);
        string? error = imported.ErrorMessages;
        stream.Close();

        if (error is not null)
        {
          errorList.Append(error);
          continue;
        }
        if (localTable is null)
          throw new InvalidProgramException();

        if (localTable.DataColumns.RowCount == 0)
          continue;
        xvalues = (DoubleColumn)localTable[0];
        yvalues = (DoubleColumn)localTable[1];

        // Add the necessary property columns
        for (int i = 0; i < localTable.PropCols.ColumnCount; i++)
        {
          string name = localTable.PropCols.GetColumnName(i);
          if (!table.PropCols.ContainsColumn(name))
          {
            table.PropCols.Add((DataColumn)localTable.PropCols[i].Clone(), name, localTable.PropCols.GetColumnKind(i));
          }
        }

        // first look if our default xcolumn matches the xvalues

        bool bMatchsXColumn = xcol is not null && ValuesMatch(xvalues, xcol);

        // if no match, then consider all xcolumns from right to left, maybe some fits
        if (!bMatchsXColumn)
        {
          for (int ncol = table.DataColumns.ColumnCount - 1; ncol >= 0; ncol--)
          {
            if ((ColumnKind.X == table.DataColumns.GetColumnKind(ncol)) &&
              (table.DataColumns[ncol] is DoubleColumn) &&
              (ValuesMatch(xvalues, (DoubleColumn)table.DataColumns[ncol]))
              )
            {
              xcol = (DoubleColumn)table.DataColumns[ncol];
              lastColumnGroup = table.DataColumns.GetColumnGroup(xcol);
              bMatchsXColumn = true;
              break;
            }
          }
        }

        // create a new x column if the last one does not match
        if (!bMatchsXColumn)
        {
          xcol = new Altaxo.Data.DoubleColumn();
          xcol.CopyDataFrom(xvalues);
          lastColumnGroup = table.DataColumns.GetUnusedColumnGroupNumber();
          table.DataColumns.Add(xcol, "X", Altaxo.Data.ColumnKind.X, lastColumnGroup);
        }

        // now add the y-values
        string columnName = importOptions.UseNeutralColumnName ?
                            $"{(string.IsNullOrEmpty(importOptions.NeutralColumnName) ? "Y" : importOptions.NeutralColumnName)}{idxYColumn}" :
                            System.IO.Path.GetFileNameWithoutExtension(filename);
        columnName = table.DataColumns.FindUniqueColumnName(columnName);
        var ycol = table.DataColumns.EnsureExistence(columnName, typeof(DoubleColumn), ColumnKind.V, lastColumnGroup);
        ++idxYColumn;
        ycol.CopyDataFrom(yvalues);

        if (importOptions.IncludeFilePathAsProperty)
        {
          // add also a property column named "FilePath" if not existing so far
          if (!table.PropCols.ContainsColumn("FilePath"))
            table.PropCols.Add(new Altaxo.Data.TextColumn(), "FilePath");

          // now set the file name property cell
          int yColumnNumber = table.DataColumns.GetColumnNumber(ycol);
          if (table.PropCols["FilePath"] is Altaxo.Data.TextColumn)
          {
            table.PropCols["FilePath"][yColumnNumber] = filename;
          }

          // set the other property columns
          for (int i = 0; i < localTable.PropCols.ColumnCount; i++)
          {
            string name = localTable.PropCols.GetColumnName(i);
            table.PropCols[name][yColumnNumber] = localTable.PropCols[i][1];
          }
        }
      } // foreache file

      // Make also a note from where it was imported
      {
        if (filenames.Length == 1)
          table.Notes.WriteLine($"Imported from {filenames[0]} at {DateTimeOffset.Now}");
        else if (filenames.Length > 1)
          table.Notes.WriteLine($"Imported from {filenames[0]} and more ({filenames.Length} files) at {DateTimeOffset.Now}");
      }

      return errorList.Length == 0 ? null : errorList.ToString();
    }

    /// <summary>
    /// Shows the SPC file import dialog, and imports the files to the table if the user clicked on "OK".
    /// </summary>
    /// <param name="table">The table to import the SPC files to.</param>
    public static void ShowDialog(DataTable table)
    {
      var options = new Altaxo.Gui.OpenFileOptions();
      options.AddFilter("*.dx;*.jdx", "JCamp-DX files (*.dx;*.jdx)");
      options.AddFilter("*.*", "All files (*.*)");
      options.FilterIndex = 0;
      options.Multiselect = true; // allow selecting more than one file

      if (Current.Gui.ShowOpenFileDialog(options))
      {
        // if user has clicked ok, import all selected files into Altaxo
        string[] filenames = options.FileNames;
        Array.Sort(filenames); // Windows seems to store the filenames reverse to the clicking order or in arbitrary order

        var importOptions = new JcampImportOptions();
        string? errors = ImportJcampFiles(filenames, table, importOptions);

        table.DataSource = new JcampImportDataSource(filenames, importOptions);

        if (errors is not null)
        {
          Current.Gui.ErrorMessageBox(errors, "Some errors occured during import!");
        }
      }
    }
  }
}
