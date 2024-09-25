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
using System.Globalization;
using System.IO;
using Altaxo.Data;

namespace Altaxo.Serialization.Jcamp
{
  /// <summary>
  /// Reader for Jcamp-Dx files.
  /// </summary>
  public class JcampReader
  {
    private const string TitleHeader = "##TITLE=";
    private const string OwnerHeader = "##OWNER=";
    private const string SystemNameHeader = "##SPECTROMETER/DATA SYSTEM=";
    private const string XLabelHeader = "##XLABEL=";
    private const string YLabelHeader = "##YLABEL=";
    private const string XUnitHeader = "##XUNITS=";
    private const string YUnitHeader = "##YUNITS=";
    private const string TimeHeader = "##TIME=";
    private const string DateHeader = "##DATE=";
    private const string NumberOfPointsHeader = "##NPOINTS=";
    private const string FirstXHeader = "##FIRSTX=";
    private const string LastXHeader = "##LASTX=";
    private const string DeltaXHeader = "##DELTAX=";
    private const string XFactorHeader = "##XFACTOR=";
    private const string YFactorHeader = "##YFACTOR=";
    private const string XYBlockHeader = "##XYDATA=";
    private const string BlockEndHeader = "##END=";

    private static readonly char[] splitChars = new char[] { ' ', '\t' };
    private static readonly char[] _plusMinusChars = new char[] { '+', '-' };
    private static readonly char[] _trimCharsDateTime = new char[] { ' ', '\t', ';' };


    protected double _xFirst = double.NaN;
    protected double _xLast = double.NaN;
    protected double _xInc = double.NaN;
    protected double _xScale = double.NaN;
    protected double _yScale = double.NaN;
    public int? _numberOfPoints = null;


    public double XFirst => _xFirst;
    public double XIncrement => _xInc;
    public double XScale => _xScale;
    public double YScale => _yScale;


    /// <summary>The title of the file.</summary>
    public string? Title { get; protected set; } = null;
    public string? Owner { get; protected set; } = null;
    public string? SystemName { get; protected set; } = null;

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

    private double[] _xValues;
    private double[] _yValues;

    public double[] XValues => _xValues;
    public double[] YValues => _yValues;

    /// <summary>
    /// Imports a Jcamp file. The file must not be a multi spectrum file (an exception is thrown in this case).
    /// </summary>
    /// <param name="stream">The stream where to import from.</param>
    /// <returns>Null if successful, otherwise an error description.</returns>
    public JcampReader(System.IO.Stream stream) : this(new StreamReader(stream))
    {
    }

    /// <summary>
    /// Imports a Jcamp file into an DataTable. The file must not be a multi spectrum file (an exception is thrown in this case).
    /// </summary>
    /// <param name="tr">A <see cref="System.IO.TextReader"/> where to import from.</param>
    /// <returns>Null if successful, otherwise an error description.</returns>
    public JcampReader(TextReader tr)
    {
      DateTime dateValue = DateTime.MinValue;
      DateTime timeValue = DateTime.MinValue;


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
            if (line.StartsWith(TitleHeader))
            {
              Title = line.Substring(TitleHeader.Length).Trim();
            }
            if (line.StartsWith(OwnerHeader))
            {
              Owner = line.Substring(OwnerHeader.Length).Trim();
            }
            if (line.StartsWith(SystemNameHeader))
            {
              SystemName = line.Substring(SystemNameHeader.Length).Trim();
            }
            else if (line.StartsWith(XLabelHeader))
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
            else if (line.StartsWith(LastXHeader))
            {
              DoubleParse(line.Substring(LastXHeader.Length), out _xLast);
            }
            else if (line.StartsWith(DeltaXHeader))
            {
              DoubleParse(line.Substring(DeltaXHeader.Length), out _xInc);
            }
            else if (line.StartsWith(NumberOfPointsHeader))
            {
              double numPoints;
              DoubleParse(line.Substring(NumberOfPointsHeader.Length), out numPoints);
              _numberOfPoints = (int)numPoints;
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
        {
          if (_numberOfPoints.HasValue && !double.IsNaN(_xFirst) && !double.IsNaN(_xLast))
            _xInc = (_xLast - _xFirst) / (_numberOfPoints.Value - 1d);
          else
            _xInc = 1;
        }
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
          var xValues = new List<double>();
          var yValues = new List<double>();
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

              xValues.Add(xValue * _xScale);
              yValues.Add(yValue * _yScale);
              xValue += _xInc;
            }
          }
          _xValues = xValues.ToArray();
          _yValues = yValues.ToArray();
        }
      }
      catch (Exception ex)
      {
        ErrorMessages = (ErrorMessages ?? String.Empty) + ex.Message;
      }
    }

    /// <summary>
    /// Imports the data of this <see cref="JcampReader"/> instance into a <see cref="DataTable"/>.
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

      xCol.Data = _xValues;
      yCol.Data = _yValues;
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





  }
}
