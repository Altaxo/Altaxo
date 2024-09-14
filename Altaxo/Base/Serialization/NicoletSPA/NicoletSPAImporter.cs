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

using System;
using System.IO;
using Altaxo.Data;

namespace Altaxo.Serialization.NicoletSPA
{
  /// <summary>
  /// Imports Nicolet SPA spectra.
  /// </summary>
  /// <remarks>
  /// See
  /// Ref1: <see href="https://stackoverflow.com/questions/2887151/how-to-read-data-from-a-nicolet-ftir-spectral-file-with-extension-of-spa"/>
  /// Ref2: <see href="https://github.com/aitap/spa2txt/blob/master/spa.c"/>
  /// Ref3: <see href="https://gist.github.com/mgaitan/53ea9f5ff47781f34c153c433474ea51"/>
  /// SPA file format:
  /// <para>The comment is at position 30:255. It is UTF8 coded.</para>
  /// <para>At position 564, there is an integer designating the offset to the data. (little endian)</para>
  /// </remarks>
  public class NicoletSPAImporter
  {
    public double XFirst { get; protected set; }
    public double XLast { get; protected set; }
    public double XIncrement { get; protected set; }

    public int NumberOfPoints { get; protected set; }

    /// <summary>The label of the x-axis.</summary>
    public string? XLabel { get; protected set; } = null;

    /// <summary>The label of the y-axis.</summary>
    public string? YLabel { get; protected set; } = null;

    /// <summary>The unit of the x-axis.</summary>
    public string? XUnit { get; protected set; } = null;

    /// <summary>The unit of the y-axis.</summary>
    public string? YUnit { get; protected set; } = null;

    /// <summary>
    /// Messages about any errors during the import of the file.
    /// </summary>
    public string? ErrorMessages { get; protected set; } = null;

    public string Comment = string.Empty;

    public double[] X { get; protected set; }
    public double[] Y { get; protected set; }


    public NicoletSPAImporter(Stream stream)
    {
      const int Pos_BeginComment = 0x1E; // 30 dez, position where the comment starts
      const int Pos_EndComment = 0x100; // 255 dez, position where the comment ends (exclusive)
      const int Pos_StartSearchMarkerBeforeOffset = 0x120; // 288 dez, position at which to search for the start marker 0x0003, after which the offset to the data can be found
      const int StartMarkerForOffset = 0x0003;
      const int Pos_NumberOfPoints = 0x234; // 564 dez, position where to find the number of data points
      const int Pos_MinMax = 0x240; // 576 dez, position where to find minimum and maximum of the x-axis

      if (stream is null)
        throw new ArgumentNullException(nameof(stream));
      if (!stream.CanSeek)
        throw new ArgumentException($"{nameof(stream)} must be seekable!");

      stream.Seek(Pos_BeginComment, SeekOrigin.Begin); // Begin of the comment section
      var buffer = new byte[Pos_EndComment - Pos_BeginComment];
      stream.Read(buffer, 0, buffer.Length);
      var comment = System.Text.Encoding.UTF8.GetString(buffer);
      Comment = comment.TrimEnd('\0');
      stream.Seek(Pos_NumberOfPoints, SeekOrigin.Begin);
      stream.Read(buffer, 0, sizeof(Int32));
      var numberOfPoints = BitConverter.ToInt32(buffer, 0);

      // get minimum and maximum wavenumbers
      stream.Seek(Pos_MinMax, SeekOrigin.Begin);
      stream.Read(buffer, 0, 2 * sizeof(Single));
      var min = BitConverter.ToSingle(buffer, 0);
      var max = BitConverter.ToSingle(buffer, sizeof(Single));

      XFirst = min;
      XLast = max;
      XIncrement = (max - min) / (numberOfPoints - 1d);
      NumberOfPoints = numberOfPoints;

      // locate the offset to the data
      // search for the start marker 0x0003, after which the offset can be found
      stream.Seek(Pos_StartSearchMarkerBeforeOffset, SeekOrigin.Begin);
      do
      {
        stream.Read(buffer, 0, sizeof(Int16));
      } while (StartMarkerForOffset != BitConverter.ToInt16(buffer, 0));

      // now read the offset
      stream.Read(buffer, 0, sizeof(Int16));
      var offset = BitConverter.ToInt16(buffer, 0);

      var ybuffer = new byte[numberOfPoints * sizeof(float)];
      stream.Seek(offset, SeekOrigin.Begin);
      stream.Read(ybuffer, 0, ybuffer.Length);

      var x = new double[numberOfPoints];
      var y = new double[numberOfPoints];

      for (int i = 0; i < numberOfPoints; i++)
      {
        var rel = i / (numberOfPoints - 1d);
        x[i] = min * (1 - rel) + max * rel;
        y[i] = BitConverter.ToSingle(ybuffer, i * sizeof(float));
      }

      X = x;
      Y = y;
    }

    /// <summary>
    /// Imports the data of this <see cref="Import"/> instance into a <see cref="DataTable"/>.
    /// </summary>
    /// <param name="table">The table.</param>
    public NicoletSPAImporter(Stream stream, DataTable table) : this(stream)
    {
      var xCol = new DoubleColumn();
      var yCol = new DoubleColumn();
      table.DataColumns.Add(xCol, string.IsNullOrEmpty(XLabel) ? "X" : XLabel, ColumnKind.X);
      table.DataColumns.Add(yCol, string.IsNullOrEmpty(YLabel) ? "Y" : YLabel, ColumnKind.V);

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

      xCol.Data = X;
      yCol.Data = Y;
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
    /// Imports a couple of Nicolet SPA files into a table. The spectra are added as columns to the (one and only) table. If the x column
    /// of the rightmost column does not match the x-data of the spectra, a new x-column is also created.
    /// </summary>
    /// <param name="filenames">An array of filenames to import.</param>
    /// <param name="table">The table the spectra should be imported to.</param>
    /// <returns>Null if no error occurs, or an error description.</returns>
    public static string? ImportNicoletSPAFiles(string[] filenames, Altaxo.Data.DataTable table, NicoletSPAImportOptions importOptions)
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
        var imported = new NicoletSPAImporter(stream, localTable);
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
      options.AddFilter("*.spa", "Nicolets SPA files (*.spa)");
      options.AddFilter("*.*", "All files (*.*)");
      options.FilterIndex = 0;
      options.Multiselect = true; // allow selecting more than one file

      if (Current.Gui.ShowOpenFileDialog(options))
      {
        // if user has clicked ok, import all selected files into Altaxo
        string[] filenames = options.FileNames;
        Array.Sort(filenames); // Windows seems to store the filenames reverse to the clicking order or in arbitrary order

        var importOptions = new NicoletSPAImportOptions();
        string? errors = ImportNicoletSPAFiles(filenames, table, importOptions);

        table.DataSource = new NicoletSPAImportDataSource(filenames, importOptions);

        if (errors is not null)
        {
          Current.Gui.ErrorMessageBox(errors, "Some errors occured during import!");
        }
      }
    }
  }
}
