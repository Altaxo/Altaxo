#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
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

namespace Altaxo.Serialization.TA_Instruments
{
  public record Q800Importer : DataFileImporterBase, Main.IImmutable
  {
    /// <inheritdoc/>
    public override (IReadOnlyList<string> FileExtensions, string Explanation) GetFileExtensions()
    {
      return ([".0", ".1"], "TA instruments DMA files (*.0;*.1)");
    }

    /// <inheritdoc/>
    public override object CheckOrCreateImportOptions(object? importOptions)
    {
      return (importOptions as Q800ImportOptions) ?? new Q800ImportOptions();
    }

    /// <inheritdoc/>
    public override IAltaxoTableDataSource? CreateTableDataSource(IReadOnlyList<string> fileNames, object importOptions)
    {
      return new Q800ImportDataSource(fileNames, (Q800ImportOptions)importOptions);
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
        var result = Q800FileReader.FromStream(stream, false);
        if (result.NumberOfColumns > 0 && result.NumberOfRows > 0)
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
    /// Imports a couple of Q800 files into a table. The data are added as columns to the (one and only) table. If the x column
    /// of the rightmost column does not match the x-data of the spectra, a new x-column is also created.
    /// </summary>
    /// <param name="filenames">An array of filenames to import. Typically, only one file is imported.</param>
    /// <param name="table">The table the spectra should be imported to.</param>
    /// <returns>Null if no error occurs, or an error description.</returns>
    public override string? Import(IReadOnlyList<string> filenames, DataTable table, object importOptionsObj, bool attachDataSource = true)
    {
      var importOptions = (Q800ImportOptions)importOptionsObj;
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
      int idxFile = -1;
      bool multipleFiles = filenames.Count > 1;
      var pcolUnit = table.PropCols.EnsureExistence("Unit", typeof(TextColumn), ColumnKind.V, 0);
      foreach (string filename in filenames)
      {
        idxFile++;
        Q800FileReader imported;
        try
        {
          using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
          {
            imported = Q800FileReader.FromStream(stream, importOptions.ConvertUnitsToSIUnits);
          }
        }
        catch (Exception ex)
        {
          errorList.Append(ex.Message);
          continue;
        }

        if (imported.NumberOfColumns == 0 || imported.NumberOfRows == 0)
        {
          errorList.AppendLine($"The file {filename} does not contain any data.");
          continue;
        }

        for (int c = 0; c < imported.NumberOfColumns; c++)
        {
          var colName = imported.ColumnNames[c];
          if (multipleFiles)
          {
            // if multiple files are imported, then we add a file index to the column name
            colName = $"{colName}{idxFile}";
          }
          var col = table.DataColumns.EnsureExistence(colName, typeof(DoubleColumn), ColumnKind.V, idxFile);
          col.Data = imported.Data[c];
          pcolUnit[table.DataColumns.GetColumnNumber(col)] = imported.Units[c];
        }
      }

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
  }
}
