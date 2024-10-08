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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Altaxo.Data;

namespace Altaxo.Serialization.BrukerOpus
{
  /// <summary>
  /// Importer for Bruker Opus spectra data files.
  /// </summary>
  /// <seealso cref="Altaxo.Serialization.IDataFileImporter" />
  /// <remarks>
  /// </remarks>
  public record BrukerOpusImporter : DataFileImporterBase
  {
    public override (IReadOnlyList<string> FileExtensions, string Explanation) GetFileExtensions()
    {
      return ([".0"], "Bruker Opus files (*.0)");
    }

    /// <inheritdoc/>
    public override object CheckOrCreateImportOptions(object? importOptions)
    {
      return (importOptions as BrukerOpusImportOptions) ?? new BrukerOpusImportOptions();
    }


    public override IAltaxoTableDataSource? CreateTableDataSource(IReadOnlyList<string> fileNames, object importOptions)
    {
      return new BrukerOpusImportDataSource(fileNames, (BrukerOpusImportOptions)importOptions);
    }


    public override double GetProbabilityForBeingThisFileFormat(string fileName)
    {
      double p = 0;
      var fe = GetFileExtensions();
      if (fe.FileExtensions.ToHashSet().Contains(Path.GetExtension(fileName).ToLowerInvariant()))
      {
        p += 0.5;
      }

      try
      {
        using var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
        var result = new BrukerOpusReader(stream);
        if (string.IsNullOrEmpty(result.ErrorMessages) &&
          result.XValues is not null && result.XValues.Length > 0
          && result.YValues is not null && result.YValues.Length > 0)
        {
          p += 0.5;
        }
      }
      catch
      {
        p = 0;
      }

      return p;
    }




    public override string? Import(IReadOnlyList<string> filenames, DataTable table, object importOptionsObj, bool attachDataSource = true)
    {
      var importOptions = (BrukerOpusImportOptions)importOptionsObj;
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
        BrukerOpusReader opusdata;
        using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
          opusdata = new BrukerOpusReader(stream);
        }
        string? error = opusdata.ErrorMessages;

        if (error is not null)
        {
          errorList.Append(error);
          continue;
        }

        if (opusdata.XValues is null || opusdata.YValues is null || opusdata.YValues.Length == 0)
          continue;
        xvalues = new DoubleColumn() { Data = opusdata.XValues };
        yvalues = new DoubleColumn() { Data = opusdata.YValues };

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
        }
      } // foreache file

      // Make also a note from where it was imported
      {
        if (filenames.Count == 1)
          table.Notes.WriteLine($"Imported from {filenames[0]} at {DateTimeOffset.Now}");
        else if (filenames.Count > 1)
          table.Notes.WriteLine($"Imported from {filenames[0]} and more ({filenames.Count} files) at {DateTimeOffset.Now}");
      }

      if (attachDataSource)
      {
        table.DataSource = CreateTableDataSource(filenames, importOptions);
      }

      return errorList.Length == 0 ? null : errorList.ToString();
    }

    /// <summary>
    /// Compare the values in a double array with values in a double column and see if they match.
    /// </summary>
    /// <param name="values">An array of double values.</param>
    /// <param name="col">A double column to compare with the double array.</param>
    /// <returns>True if the length of the array is equal to the length of the <see cref="DoubleColumn" /> and the values in
    /// both array match to each other, otherwise false.</returns>
    public static bool ValuesMatch(IReadOnlyList<double> values, IReadOnlyList<double> col)
    {
      if (values.Count != col.Count)
        return false;

      for (int i = 0; i < values.Count; i++)
        if (col[i] != values[i])
          return false;

      return true;
    }
  }
}
