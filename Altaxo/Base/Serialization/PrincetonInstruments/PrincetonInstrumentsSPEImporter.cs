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
using System.Text;
using Altaxo.Data;

namespace Altaxo.Serialization.PrincetonInstruments
{
  /// <summary>
  /// Imports Princeton Instruments SPE files into an Altaxo table.
  /// </summary>
  public record PrincetonInstrumentsSPEImporter : DataFileImporterBase, Main.IImmutable
  {
    /// <inheritdoc/>
    public override (IReadOnlyList<string> FileExtensions, string Explanation) GetFileExtensions()
    {
      return ([".spe"], "Princeton Instruments SPE files (*.spe)");
    }

    /// <inheritdoc/>
    public override object CheckOrCreateImportOptions(object? importOptions)
    {
      return (importOptions as PrincetonInstrumentsSPEImportOptions) ?? new PrincetonInstrumentsSPEImportOptions();
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
        var result = new PrincetonInstrumentsSPEReader(stream);
        if (result.NumberOfFrames > 0 && result.NumberOfRegionsOfInterest > 0)
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


    /// <inheritdoc/>
    public override IAltaxoTableDataSource? CreateTableDataSource(IReadOnlyList<string> fileNames, object importOptions)
    {
      return new PrincetonInstrumentsSPEImportDataSource(fileNames, (PrincetonInstrumentsSPEImportOptions)importOptions);
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

    /// <summary>
    /// Imports a couple of Princeton Instruments SPE files into a table. The spectra are added as columns to the (one and only) table. If the x column
    /// of the rightmost column does not match the x-data of the spectra, a new x-column is also created.
    /// </summary>
    /// <param name="filenames">An array of filenames to import.</param>
    /// <param name="table">The table the spectra should be imported to.</param>
    /// <param name="importOptionsObj">The import options. Has to be an instance of <see cref="PrincetonInstrumentsSPEImportOptions"/>.</param>
    /// <param name="attachDataSource"></param>
    /// <returns>Null if no error occurs, or an error description.</returns>
    public override string? Import(IReadOnlyList<string> filenames, Altaxo.Data.DataTable table, object importOptionsObj, bool attachDataSource = true)
    {
      var importOptions = (PrincetonInstrumentsSPEImportOptions)importOptionsObj;
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
        PrincetonInstrumentsSPEReader reader;
        using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
          try
          {

            reader = new PrincetonInstrumentsSPEReader(stream);
          }
          catch (Exception ex)
          {
            errorList.AppendLine($"Error reading file {filename} with the {typeof(PrincetonInstrumentsSPEReader)}: {ex.Message}");
            continue;
          }
        }


        // Add the necessary property columns
        if (importOptions.IncludeFrameMetaDataAsProperties)
        {
          for (int i = 0; i < reader.FrameMetaDataNames.Count; i++)
          {
            string name = reader.FrameMetaDataNames[i];
            table.PropCols.EnsureExistence(name, typeof(DoubleColumn), ColumnKind.V, 0);
          }
        }

        if (reader.NumberOfFrames > 1)
        {
          table.PropCols.EnsureExistence("FrameIndex", typeof(DoubleColumn), ColumnKind.V, 0);
        }

        if (reader.NumberOfRegionsOfInterest > 1)
        {
          table.PropCols.EnsureExistence("RegionIndex", typeof(DoubleColumn), ColumnKind.V, 0);
        }

        // first look if our default xcolumn matches the xvalues

        bool bMatchsXColumn = xcol is not null && ValuesMatch(reader.XValues, xcol);

        // if no match, then consider all xcolumns from right to left, maybe some fits
        if (!bMatchsXColumn && reader.XValues.Length > 0)
        {
          for (int ncol = table.DataColumns.ColumnCount - 1; ncol >= 0; ncol--)
          {
            if ((ColumnKind.X == table.DataColumns.GetColumnKind(ncol)) &&
              (table.DataColumns[ncol] is DoubleColumn) &&
              (ValuesMatch(reader.XValues, (DoubleColumn)table.DataColumns[ncol]))
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
          xcol.CopyDataFrom(reader.XValues);
          lastColumnGroup = table.DataColumns.GetUnusedColumnGroupNumber();
          table.DataColumns.Add(xcol, "X", Altaxo.Data.ColumnKind.X, lastColumnGroup);
        }

        // now add the y-values
        // note that each frame and each region should receive an own group

        for (int idxFrame = 0; idxFrame < reader.Data.Count; idxFrame++)
        {
          if (importOptions.IndicesOfImportedFrames.Count > 0 && !importOptions.IndicesOfImportedFrames.Contains(idxFrame))
            continue;
          var frameData = reader.Data[idxFrame];
          for (int idxRegion = 0; idxRegion < frameData.Count; idxRegion++)
          {
            if (importOptions.IndicesOfImportedRegions.Count > 0 && !importOptions.IndicesOfImportedRegions.Contains(idxRegion))
              continue;
            var regionData = frameData[idxRegion];
            for (int idxColumn = 0; idxColumn < regionData.GetLength(0); idxColumn++)
            {
              string columnName = importOptions.UseNeutralColumnName ?
                            $"{(string.IsNullOrEmpty(importOptions.NeutralColumnName) ? "Y" : importOptions.NeutralColumnName)}{idxYColumn}" :
                            System.IO.Path.GetFileNameWithoutExtension(filename);
              columnName = table.DataColumns.FindUniqueColumnName(columnName);
              var ycol = table.DataColumns.EnsureExistence(columnName, typeof(DoubleColumn), ColumnKind.V, lastColumnGroup);
              ++idxYColumn;
              for (int idxRow = 0; idxRow < regionData.GetLength(1); idxRow++)
              {
                ycol[idxRow] = regionData[idxColumn, idxRow];
              }

              int yColumnNumber = table.DataColumns.GetColumnNumber(ycol);
              if (importOptions.IncludeFilePathAsProperty)
              {
                // add also a property column named "FilePath" if not existing so far
                if (!table.PropCols.ContainsColumn("FilePath"))
                  table.PropCols.Add(new Altaxo.Data.TextColumn(), "FilePath");

                // now set the file name property cell
                if (table.PropCols["FilePath"] is Altaxo.Data.TextColumn)
                {
                  table.PropCols["FilePath"][yColumnNumber] = filename;
                }
              }

              if (importOptions.IncludeFrameMetaDataAsProperties)
              {
                // set the other property columns
                for (int i = 0; i < reader.FrameMetaDataNames.Count; i++)
                {
                  string name = reader.FrameMetaDataNames[i];
                  table.PropCols[name][yColumnNumber] = reader.FrameMetaDataValues[idxFrame, i];
                }
              }

              if (reader.NumberOfFrames > 1)
              {
                table.PropCols["FrameIndex"][yColumnNumber] = idxFrame;
              }

              if (reader.NumberOfRegionsOfInterest > 1)
              {
                table.PropCols["RegionIndex"][yColumnNumber] = idxRegion;
              }
            }
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


    public override string? Import(IReadOnlyList<string> fileNames, ImportOptionsInitial initialOptions)
    {
      var stb = new StringBuilder();
      var importOptions = (PrincetonInstrumentsSPEImportOptions)initialOptions.ImportOptions;
      if (initialOptions.DistributeFilesToSeparateTables)
      {
        foreach (var fileName in fileNames)
        {
          if (initialOptions.DistributeDataPerFileToSeparateTables)
          {
            // Here: we not only distribute each frame into a separate table, but also each region

            PrincetonInstrumentsSPEReader reader;
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
              reader = new PrincetonInstrumentsSPEReader(stream);
            }

            for (int idxFrame = 0; idxFrame < reader.NumberOfFrames; idxFrame++)
            {
              for (int idxRegion = 0; idxRegion < reader.NumberOfRegionsOfInterest; idxRegion++)
              {
                var dataTable = new DataTable();
                string title = string.Empty;

                if (initialOptions.UseMetaDataNameAsTableName)
                {
                  title = Path.GetFileNameWithoutExtension(fileName);
                }

                if (reader.NumberOfFrames > 1)
                {
                  title = $"{title} Frame#{idxFrame}";
                }
                if (reader.NumberOfRegionsOfInterest > 1)
                {
                  title = $"{title} ROI#{idxRegion}";
                }

                if (!string.IsNullOrEmpty(title))
                {
                  dataTable.Name = title;
                }

                var localImportOptions = importOptions with { IndicesOfImportedFrames = [idxFrame], IndicesOfImportedRegions = [idxRegion] };

                var result = Import([fileName], dataTable, localImportOptions);
                if (result is not null)
                {
                  stb.AppendLine(result);
                }
                Current.Project.AddItemWithThisOrModifiedName(dataTable);
                Current.ProjectService.CreateNewWorksheet(dataTable);
                dataTable.DataSource = CreateTableDataSource(fileNames, localImportOptions);
              }
            }
          }
          else
          {
            var dataTable = new DataTable();
            var result = Import([fileName], dataTable, importOptions);
            if (result is not null)
            {
              stb.AppendLine(result);
            }
            Current.Project.AddItemWithThisOrModifiedName(dataTable);
            Current.ProjectService.CreateNewWorksheet(dataTable);
            dataTable.DataSource = CreateTableDataSource(fileNames, importOptions);
          }
        }
      }
      else // all files into one table
      {
        var dataTable = new DataTable();
        var result = Import(fileNames, dataTable, importOptions);
        if (result is not null)
        {
          stb.AppendLine(result);
        }
        Current.Project.AddItemWithThisOrModifiedName(dataTable);
        Current.ProjectService.CreateNewWorksheet(dataTable);
        dataTable.DataSource = CreateTableDataSource(fileNames, importOptions);
      }

      return stb.Length == 0 ? null : stb.ToString();
    }

  }
}
