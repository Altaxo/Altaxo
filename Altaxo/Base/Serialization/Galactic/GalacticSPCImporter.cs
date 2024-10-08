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

namespace Altaxo.Serialization.Galactic
{
  public record GalacticSPCImporter : DataFileImporterBase, Main.IImmutable
  {
    /// <inheritdoc/>
    public override (IReadOnlyList<string> FileExtensions, string Explanation) GetFileExtensions()
    {
      return ([".spc"], "Galactic SPC files (*.spc)");
    }

    /// <inheritdoc/>
    public override object CheckOrCreateImportOptions(object? importOptions)
    {
      return (importOptions as GalacticSPCImportOptions) ?? new GalacticSPCImportOptions();
    }

    /// <inheritdoc/>
    public override IAltaxoTableDataSource? CreateTableDataSource(IReadOnlyList<string> fileNames, object importOptions)
    {
      return new GalacticSPCImportDataSource(fileNames, (GalacticSPCImportOptions)importOptions);
    }


    /// <inheritdoc/>
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
        var result = new GalacticSPCReader(stream);
        if (string.IsNullOrEmpty(result.ErrorMessages) &&
          result.XValues is not null && result.XValues.Length > 0
          && result.YValues is not null && result.YValues.Count > 0)
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

    /// <summary>
    /// Imports a couple of SPC files into a table. The spectra are added as columns to the table. If the x column
    /// of the rightmost column does not match the x-data of the spectra, a new x-column is also created.
    /// </summary>
    /// <param name="filenames">An array of filenames to import.</param>
    /// <param name="table">The table the spectra should be imported to.</param>
    /// <returns>Null if no error occurs, or an error description.</returns>
    public override string? Import(IReadOnlyList<string> filenames, Altaxo.Data.DataTable table, object importOptionsObj, bool attachDataSource)
    {
      var importOptions = (GalacticSPCImportOptions)importOptionsObj;
      Altaxo.Data.DoubleColumn? xcol = null;
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
        GalacticSPCReader result;
        using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
          result = new GalacticSPCReader(stream);
        }

        if (!string.IsNullOrEmpty(result.ErrorMessages))
        {
          errorList.Append(result.ErrorMessages);
          continue;
        }
        if (result.XValues is null || result.YValues is null)
          throw new InvalidProgramException();



        // first look if our default xcolumn matches the xvalues

        bool bMatchsXColumn = xcol is not null && ValuesMatch(result.XValues, xcol);

        // if no match, then consider all xcolumns from right to left, maybe some fits
        if (!bMatchsXColumn)
        {
          for (int ncol = table.DataColumns.ColumnCount - 1; ncol >= 0; ncol--)
          {
            if ((ColumnKind.X == table.DataColumns.GetColumnKind(ncol)) &&
              (table.DataColumns[ncol] is DoubleColumn) &&
              (ValuesMatch(result.XValues, (DoubleColumn)table.DataColumns[ncol]))
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
          xcol.CopyDataFrom(result.XValues);
          lastColumnGroup = table.DataColumns.GetUnusedColumnGroupNumber();
          table.DataColumns.Add(xcol, "X", Altaxo.Data.ColumnKind.X, lastColumnGroup);
        }

        // now add the y-values

        for (int iSpectrum = 0; iSpectrum < result.YValues.Count; ++iSpectrum)
        {
          string columnName = importOptions.UseNeutralColumnName ?
                              $"{(string.IsNullOrEmpty(importOptions.NeutralColumnName) ? "Y" : importOptions.NeutralColumnName)}{idxYColumn}" :
                              System.IO.Path.GetFileNameWithoutExtension(filename);
          columnName = table.DataColumns.FindUniqueColumnName(columnName);
          var ycol = table.DataColumns.EnsureExistence(columnName, typeof(DoubleColumn), ColumnKind.V, lastColumnGroup);
          ++idxYColumn;
          ycol.CopyDataFrom(result.YValues[iSpectrum]);

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
        } // foreach yarray in yvalues
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
    public static bool ValuesMatch(double[] values, DoubleColumn col)
    {
      if (values.Length != col.Count)
        return false;

      for (int i = 0; i < values.Length; i++)
        if (col[i] != values[i])
          return false;

      return true;
    }

  }
}
