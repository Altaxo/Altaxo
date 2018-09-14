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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Altaxo.Data;

namespace Altaxo.Serialization.Jcamp
{
  public class Import
  {
    private static readonly char[] splitChars = new char[] { ' ', '\t' };

    /// <summary>
    /// Imports a Jcamp file into an DataTable. The file must not be a multi spectrum file (an exception is thrown in this case).
    /// </summary>
    /// <param name="table">On return, contains the newly created data table with the spectral data.</param>
    /// <param name="stream">The stream where to import from.</param>
    /// <returns>Null if successful, otherwise an error description.</returns>
    public static string ToDataTable(System.IO.Stream stream, out DataTable table)
    {
      table = null;
      TextReader tr;
      try
      {
        tr = new StreamReader(stream);
        return ToDataTable(tr, out table);
      }
      catch (Exception ex)
      {
        return ex.Message;
      }
    }

    /// <summary>
    /// Imports a Jcamp file into an DataTable. The file must not be a multi spectrum file (an exception is thrown in this case).
    /// </summary>
    /// <param name="table">On return, contains the newly created data table with the spectral data.</param>
    /// <param name="tr">A <see cref="System.IO.TextReader"/> where to import from.</param>
    /// <returns>Null if successful, otherwise an error description.</returns>
    public static string ToDataTable(TextReader tr, out DataTable table)
    {
      const string XLabelHeader = "##XLABEL=";
      const string YLabelHeader = "##YLABEL=";
      const string XUnitHeader = "##XUNITS=";
      const string YUnitHeader = "##YUNITS=";
      const string TimeHeader = "##TIME=";
      const string DateHeader = "##DATE=";
      const string FirstXHeader = "##FIRSTX=";
      const string DeltaXHeader = "##DELTAX=";
      const string XFactorHeader = "##XFACTOR=";
      const string YFactorHeader = "##YFACTOR=";
      const string XYBlockHeader = "##XYDATA=(X++(Y..Y))";
      const string BlockEndHeader = "##END=";

      table = new DataTable();

      try
      {
        string line;
        int lineCounter = 0;

        double xFirst = double.NaN;
        double xInc = double.NaN;
        double xScale = double.NaN;
        double yScale = double.NaN;
        string xLabel = null, yLabel = null, xUnit = null, yUnit = null;
        DateTime dateValue = DateTime.MinValue;
        DateTime timeValue = DateTime.MinValue;

        do
        {
          line = tr.ReadLine();
          lineCounter++;

          if (line.StartsWith(XLabelHeader))
            xLabel = line.Substring(XLabelHeader.Length).Trim();
          else if (line.StartsWith(YLabelHeader))
            yLabel = line.Substring(YLabelHeader.Length).Trim();
          else if (line.StartsWith(XUnitHeader))
            xUnit = line.Substring(XUnitHeader.Length).Trim();
          else if (line.StartsWith(YUnitHeader))
            yUnit = line.Substring(YUnitHeader.Length).Trim();
          else if (line.StartsWith(TimeHeader))
            DateTime.TryParse(line.Substring(TimeHeader.Length), out timeValue);
          else if (line.StartsWith(DateHeader))
          {
            string dateString = line.Substring(DateHeader.Length);
            string[] tokens = dateString.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length != 3)
              throw new FormatException("Unknown Date format in line " + lineCounter.ToString());

            int year = int.Parse(tokens[0]);
            int month = int.Parse(tokens[1]);
            int day = int.Parse(tokens[2]);
            if (year < 100)
              year += 2000;
            // DateTime.TryParse(line.Substring(DateHeader.Length), System.Globalization.DateTimeFormatInfo.InvariantInfo, System.Globalization.DateTimeStyles.AssumeLocal, out dateValue);
            dateValue = DateTime.MinValue.AddYears(year - 1);
            dateValue = dateValue.AddMonths(month - 1);
            dateValue = dateValue.AddDays(day - 1);
          }
          else if (line.StartsWith(FirstXHeader))
            double.TryParse(line.Substring(FirstXHeader.Length), NumberStyles.Float, NumberFormatInfo.InvariantInfo, out xFirst);
          else if (line.StartsWith(DeltaXHeader))
            double.TryParse(line.Substring(DeltaXHeader.Length), NumberStyles.Float, NumberFormatInfo.InvariantInfo, out xInc);
          else if (line.StartsWith(XFactorHeader))
            double.TryParse(line.Substring(XFactorHeader.Length), NumberStyles.Float, NumberFormatInfo.InvariantInfo, out xScale);
          else if (line.StartsWith(YFactorHeader))
            double.TryParse(line.Substring(YFactorHeader.Length), NumberStyles.Float, NumberFormatInfo.InvariantInfo, out yScale);
        } while (!line.StartsWith(XYBlockHeader));

        // adjust some variables if not given
        if (double.IsNaN(xInc))
          xInc = 1;
        if (double.IsNaN(xScale))
          xScale = 1;
        if (double.IsNaN(yScale))
          yScale = 1;

        DateTime combinedTime = DateTime.MinValue;
        if (dateValue != null)
          combinedTime = dateValue;
        if (timeValue != null)
          combinedTime = combinedTime.Add(timeValue.TimeOfDay);

        if (line.StartsWith(XYBlockHeader))
        {
          var xCol = new DoubleColumn();
          var yCol = new DoubleColumn();
          table.DataColumns.Add(xCol, xLabel == null ? "X" : xLabel, ColumnKind.X);
          table.DataColumns.Add(yCol, yLabel == null ? "Y" : yLabel, ColumnKind.V);

          if (combinedTime != DateTime.MinValue)
          {
            table.PropCols.Add(new DateTimeColumn(), "Date", ColumnKind.V);
            table.PropCols["Date"][1] = combinedTime;
          }

          if (xUnit != null || yUnit != null)
          {
            table.PropCols.Add(new TextColumn(), "Unit", ColumnKind.V);
            table.PropCols["Unit"][0] = xUnit;
            table.PropCols["Unit"][1] = yUnit;
          }

          if (xLabel != null || yLabel != null)
          {
            table.PropCols.Add(new TextColumn(), "Label", ColumnKind.V);
            table.PropCols["Label"][0] = xLabel;
            table.PropCols["Label"][1] = yLabel;
          }

          for (; ; )
          {
            line = tr.ReadLine();
            lineCounter++;

            if (line == null || line.StartsWith(BlockEndHeader))
              break;
            string[] tokens = line.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 0)
              continue;

            // all tokens must contain numeric values, and the first token is the actual x value
            if (!double.TryParse(tokens[0], NumberStyles.Float, NumberFormatInfo.InvariantInfo, out var xValue))
              throw new FormatException("Non numeric value found in line " + lineCounter.ToString());

            for (int i = 1; i < tokens.Length; i++)
            {
              if (!double.TryParse(tokens[i], NumberStyles.Float, NumberFormatInfo.InvariantInfo, out var yValue))
                throw new FormatException("Non numeric value found in line" + lineCounter.ToString());

              xCol[xCol.Count] = xValue * xScale;
              yCol[yCol.Count] = yValue * yScale;
              xValue += xInc;
            }
          }
        }
      }
      catch (Exception ex)
      {
        return ex.Message;
      }

      return null;
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
    public static string ImportJcampFiles(string[] filenames, Altaxo.Data.DataTable table)
    {
      DoubleColumn xcol = null;
      DoubleColumn xvalues, yvalues;
      var errorList = new System.Text.StringBuilder();
      int lastColumnGroup = 0;

      if (table.DataColumns.ColumnCount > 0)
      {
        lastColumnGroup = table.DataColumns.GetColumnGroup(table.DataColumns.ColumnCount - 1);
        Altaxo.Data.DataColumn xColumnOfRightMost = table.DataColumns.FindXColumnOfGroup(lastColumnGroup);
        if (xColumnOfRightMost is Altaxo.Data.DoubleColumn)
          xcol = (Altaxo.Data.DoubleColumn)xColumnOfRightMost;
      }

      foreach (string filename in filenames)
      {
        var stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
        string error = ToDataTable(stream, out var localTable);
        stream.Close();

        if (null != error)
        {
          errorList.Append(error);
          continue;
        }
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
        var ycol = new Altaxo.Data.DoubleColumn();
        ycol.CopyDataFrom(yvalues);
        table.DataColumns.Add(ycol,
          table.DataColumns.FindUniqueColumnName(System.IO.Path.GetFileNameWithoutExtension(filename)),
          Altaxo.Data.ColumnKind.V,
          lastColumnGroup);

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
      } // foreache file

      return errorList.Length == 0 ? null : errorList.ToString();
    }

    /// <summary>
    /// Shows the SPC file import dialog, and imports the files to the table if the user clicked on "OK".
    /// </summary>
    /// <param name="table">The table to import the SPC files to.</param>
    public static void ShowDialog(Altaxo.Data.DataTable table)
    {
      var options = new Altaxo.Gui.OpenFileOptions();
      options.AddFilter("*.dx", "JCamp-DX files (*.dx)");
      options.AddFilter("*.*", "All files (*.*)");
      options.FilterIndex = 0;
      options.Multiselect = true; // allow selecting more than one file

      if (Current.Gui.ShowOpenFileDialog(options))
      {
        // if user has clicked ok, import all selected files into Altaxo
        string[] filenames = options.FileNames;
        Array.Sort(filenames); // Windows seems to store the filenames reverse to the clicking order or in arbitrary order

        string errors = ImportJcampFiles(filenames, table);

        if (errors != null)
        {
          Current.Gui.ErrorMessageBox(errors, "Some errors occured during import!");
        }
      }
    }
  }
}
