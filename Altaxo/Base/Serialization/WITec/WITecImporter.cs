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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Altaxo.Data;

namespace Altaxo.Serialization.WITec
{
  public record WITecImporter : DataFileImporterBase
  {
    public override (IReadOnlyList<string> FileExtensions, string Explanation) GetFileExtensions()
    {
      return ([".wip"], "WiTec project files (*.wip)");
    }

    /// <inheritdoc/>
    public override object CheckOrCreateImportOptions(object? importOptions)
    {
      return (importOptions as WITecImportOptions) ?? new WITecImportOptions();
    }

    public override IAltaxoTableDataSource? CreateTableDataSource(IReadOnlyList<string> fileNames, object importOptions)
    {
      return new WITecImportDataSource(fileNames, (WITecImportOptions)importOptions);
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
        using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
          var (name, rootNode) = WITecTreeNode.Read(stream);
          if (rootNode.ChildNodes.TryGetValue("Data", out var dataNode) && dataNode.ChildNodes.Count > 0)
          {
            p += 0.5;
          }
        }
      }
      catch
      {
        p = 0;
      }

      return p;
    }

    public override string? Import(IReadOnlyList<string> fileNames, ImportOptionsInitial initialOptions)
    {
      var stb = new StringBuilder();
      var importOptions = (WITecImportOptions)initialOptions.ImportOptions;
      if (initialOptions.DistributeFilesToSeparateTables)
      {
        foreach (var fileName in fileNames)
        {
          if (initialOptions.DistributeDataPerFileToSeparateTables)
          {
            WITecReader reader;
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
              reader = new WITecReader(stream);
            }
            reader.ExtractSpectra();

            var spectra = (importOptions.IgnoreSecondaryData ?
              reader.Spectra.Where(s => s.GraphType == TDGraphClass.GraphClassType.SpectralData) :
              reader.Spectra).ToArray();

            int indexOfSpectrum = -1;
            foreach (var spectrum in spectra)
            {
              ++indexOfSpectrum;

              var dataTable = new DataTable();
              if (!string.IsNullOrEmpty(spectrum.Title) && initialOptions.UseMetaDataNameAsTableName)
              {
                dataTable.Name = spectrum.Title;
              }

              var localImportOptions = importOptions with { IndicesOfImportedGraphs = [indexOfSpectrum] };

              var result = Import([fileName], dataTable, localImportOptions);
              if (result is not null)
              {
                stb.AppendLine(result);
              }
              Current.Project.AddItemWithThisOrModifiedName(dataTable);
              Current.ProjectService.CreateNewWorksheet(dataTable);
              dataTable.DataSource = new WITecImportDataSource(fileNames, localImportOptions);

            }
          }
          else
          {
            var dataTable = new DataTable();
            var result = Import([fileName], dataTable, initialOptions.ImportOptions);
            if (result is not null)
            {
              stb.AppendLine(result);
            }
            Current.Project.AddItemWithThisOrModifiedName(dataTable);
            Current.ProjectService.CreateNewWorksheet(dataTable);
            dataTable.DataSource = new WITecImportDataSource(fileNames, importOptions);
          }
        }
      }
      else // all files into one table
      {
        var dataTable = new DataTable();
        var result = Import(fileNames, dataTable, initialOptions.ImportOptions);
        if (result is not null)
        {
          stb.AppendLine(result);
        }
        Current.Project.AddItemWithThisOrModifiedName(dataTable);
        Current.ProjectService.CreateNewWorksheet(dataTable);
        dataTable.DataSource = new WITecImportDataSource(fileNames, importOptions);
      }

      return stb.Length == 0 ? null : stb.ToString();
    }

    public override string? Import(IReadOnlyList<string> fileNames, DataTable table, object importOptionsObj, bool attachDataSource = true)
    {
      var importOptions = (WITecImportOptions)importOptionsObj;
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
      foreach (var fileName in fileNames)
      {
        WITecReader reader;
        using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
          reader = new WITecReader(stream);
        }
        reader.ExtractSpectra();

        IEnumerable<TDGraphClass> spectra = importOptions.IgnoreSecondaryData ?
          reader.Spectra.Where(r => r.GraphType == TDGraphClass.GraphClassType.SpectralData) :
          reader.Spectra;
        int indexOfSpectrum = -1;
        foreach (var spectrum in spectra)
        {
          ++indexOfSpectrum;
          if (!(importOptions.IndicesOfImportedGraphs.Count == 0 ||
               importOptions.IndicesOfImportedGraphs.Contains(indexOfSpectrum) ||
               importOptions.IndicesOfImportedGraphs.Contains(indexOfSpectrum - reader.Spectra.Count)
            ))
          {
            continue;
          }

          // first look if our default xcolumn matches the xvalues
          bool bMatchsXColumn = xcol is not null && ValuesMatch(spectrum.XValues, xcol);

          // if no match, then consider all xcolumns from right to left, maybe some fits
          if (!bMatchsXColumn)
          {
            for (int ncol = table.DataColumns.ColumnCount - 1; ncol >= 0; ncol--)
            {
              if ((ColumnKind.X == table.DataColumns.GetColumnKind(ncol)) &&
                (table.DataColumns[ncol] is DoubleColumn) &&
                (ValuesMatch(spectrum.XValues, (DoubleColumn)table.DataColumns[ncol]))
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
            xcol.CopyDataFrom(spectrum.XValues);
            lastColumnGroup = table.DataColumns.GetUnusedColumnGroupNumber();
            table.DataColumns.Add(xcol, "X", Altaxo.Data.ColumnKind.X, lastColumnGroup);
            if (!string.IsNullOrEmpty(spectrum.XUnitShortcut))
            {
              var xUSColumn = table.PropCols.EnsureExistence("Unit", typeof(TextColumn), ColumnKind.V, 0);
              xUSColumn[table.DataColumns.GetColumnNumber(xcol)] = spectrum.XUnitShortcut;
            }
            if (!string.IsNullOrEmpty(spectrum.XUnitDescription))
            {
              var xUSColumn = table.PropCols.EnsureExistence("UnitDescription", typeof(TextColumn), ColumnKind.V, 0);
              xUSColumn[table.DataColumns.GetColumnNumber(xcol)] = spectrum.XUnitDescription;
            }
          }

          // now add the y-values

          for (int iSpectrum = 0; iSpectrum < spectrum.ZValues.Count; ++iSpectrum)
          {
            string columnName = importOptions.UseNeutralColumnName ?
            $"{(string.IsNullOrEmpty(importOptions.NeutralColumnName) ? "Y" : importOptions.NeutralColumnName)}{idxYColumn}" :
                                System.IO.Path.GetFileNameWithoutExtension(fileName);
            columnName = table.DataColumns.FindUniqueColumnName(columnName);
            var ycol = table.DataColumns.EnsureExistence(columnName, typeof(DoubleColumn), ColumnKind.V, lastColumnGroup);
            ++idxYColumn;
            ycol.CopyDataFrom(spectrum.ZValues[iSpectrum]);

            if (!string.IsNullOrEmpty(spectrum.ZUnitShortcut))
            {
              var xPC = table.PropCols.EnsureExistence("Unit", typeof(TextColumn), ColumnKind.V, 0);
              xPC[table.DataColumns.GetColumnNumber(ycol)] = spectrum.ZUnitShortcut;
            }
            if (!string.IsNullOrEmpty(spectrum.ZUnitDescription))
            {
              var xPC = table.PropCols.EnsureExistence("UnitDescription", typeof(TextColumn), ColumnKind.V, 0);
              xPC[table.DataColumns.GetColumnNumber(ycol)] = spectrum.ZUnitDescription;
            }
            if (!string.IsNullOrEmpty(spectrum.Title))
            {
              var xPC = table.PropCols.EnsureExistence("Title", typeof(TextColumn), ColumnKind.V, 0);
              xPC[table.DataColumns.GetColumnNumber(ycol)] = spectrum.Title;
            }
            if (spectrum.ZMetaData is { } zMeta)
            {
              var xPC = table.PropCols.EnsureExistence(!string.IsNullOrEmpty(zMeta.ZUnitDescription) ? zMeta.ZUnitDescription : (!string.IsNullOrEmpty(zMeta.ZUnitShortcut) ? zMeta.ZUnitShortcut : "MetaData"), typeof(DoubleColumn), ColumnKind.V, 0);
              xPC[table.DataColumns.GetColumnNumber(ycol)] = zMeta.ZValues[iSpectrum];
            }

            if (importOptions.IncludeFilePathAsProperty)
            {
              // add also a property column named "FilePath" if not existing so far
              if (!table.PropCols.ContainsColumn("FilePath"))
                table.PropCols.Add(new Altaxo.Data.TextColumn(), "FilePath");

              // now set the file name property cell
              int yColumnNumber = table.DataColumns.GetColumnNumber(ycol);
              if (table.PropCols["FilePath"] is Altaxo.Data.TextColumn)
              {
                table.PropCols["FilePath"][yColumnNumber] = fileName;
              }
            }
          }
        } // for each spectrum
      } // for each file

      if (attachDataSource)
      {
        table.DataSource = CreateTableDataSource(fileNames, importOptions);
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
