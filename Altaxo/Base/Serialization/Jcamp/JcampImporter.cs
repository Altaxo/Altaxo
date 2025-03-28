﻿#region Copyright

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

namespace Altaxo.Serialization.Jcamp
{
  public record JcampImporter : DataFileImporterBase, Main.IImmutable
  {
    /// <inheritdoc/>
    public override (IReadOnlyList<string> FileExtensions, string Explanation) GetFileExtensions()
    {
      return ([".jcamp", ".jdx", ".dx"], "JCampDX files (*.jcamp;*.jdx;*.dx)");
    }

    /// <inheritdoc/>
    public override object CheckOrCreateImportOptions(object? importOptions)
    {
      return (importOptions as JcampImportOptions) ?? new JcampImportOptions();
    }

    /// <inheritdoc/>
    public override IAltaxoTableDataSource? CreateTableDataSource(IReadOnlyList<string> fileNames, object importOptions)
    {
      return new JcampImportDataSource(fileNames, (JcampImportOptions)importOptions);
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
        var result = new JcampReader(stream);
        if (string.IsNullOrEmpty(result.ErrorMessages) && result.Blocks.Count > 0 && result.Blocks[0].XValues.Length > 0 && result.Blocks[0].YValues.Length > 0)
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
    /// Imports a couple of JCAMP files into a table. The spectra are added as columns to the (one and only) table. If the x column
    /// of the rightmost column does not match the x-data of the spectra, a new x-column is also created.
    /// </summary>
    /// <param name="filenames">An array of filenames to import.</param>
    /// <param name="table">The table the spectra should be imported to.</param>
    /// <returns>Null if no error occurs, or an error description.</returns>
    public override string? Import(IReadOnlyList<string> filenames, DataTable table, object importOptionsObj, bool attachDataSource = true)
    {
      var importOptions = (JcampImportOptions)importOptionsObj;
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
        JcampReader imported;

        using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
          imported = new JcampReader(stream);
        }
        string? error = imported.ErrorMessages;

        if (error is not null)
        {
          errorList.Append(error);
          continue;
        }

        for (int idxBlock = 0; idxBlock < imported.Blocks.Count; ++idxBlock)
        {
          var block = imported.Blocks[idxBlock];

          if (block.XValues is null || block.YValues is null || block.XValues.Length == 0 || block.YValues.Length == 0)
            continue;

          xvalues = new DoubleColumn() { Data = block.XValues };
          yvalues = new DoubleColumn() { Data = block.YValues };

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
            string xColumnName = (!string.IsNullOrEmpty(block.XLabel) && !importOptions.UseNeutralColumnName) ? block.XLabel : "X";
            table.DataColumns.Add(xcol, xColumnName, Altaxo.Data.ColumnKind.X, lastColumnGroup);
            if (!string.IsNullOrEmpty(block.XUnit))
            {
              var pCol = table.PropCols.EnsureExistence("Unit", typeof(TextColumn), ColumnKind.V, 0);
              pCol[table.DataColumns.GetColumnNumber(xcol)] = block.XUnit;
            }
          }

          // now add the y-values
          string yColumnName = (!importOptions.UseNeutralColumnName && !string.IsNullOrEmpty(block.YLabel)) ? block.YLabel :
                               (imported.Blocks.Count == 1 ? "Y" : $"Y{idxBlock}");
          yColumnName = table.DataColumns.FindUniqueColumnName(yColumnName);
          var ycol = table.DataColumns.EnsureExistence(yColumnName, typeof(DoubleColumn), ColumnKind.V, lastColumnGroup);
          ++idxYColumn;
          ycol.CopyDataFrom(yvalues);
          int yColumnNumber = table.DataColumns.GetColumnNumber(ycol);
          if (!string.IsNullOrEmpty(block.YUnit))
          {
            var pCol = table.PropCols.EnsureExistence("Unit", typeof(TextColumn), ColumnKind.V, 0);
            pCol[yColumnNumber] = block.YUnit;
          }


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

          // set the other property columns
          if (!string.IsNullOrEmpty(block.Title))
          {
            var pcol = table.PropCols.EnsureExistence("Title", typeof(TextColumn), ColumnKind.V, 0);
            pcol[yColumnNumber] = block.Title;
          }
        } // foreach block
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
    /// Imports the data of this <see cref="JcampReader"/> instance into a <see cref="DataTable"/>.
    /// </summary>
    /// <param name="table">The table.</param>
    public void ToDataTable(JcampReader reader, DataTable table)
    {
      foreach (var block in reader.Blocks)
      {

        var xCol = new DoubleColumn();
        var yCol = new DoubleColumn();
        table.DataColumns.Add(xCol, string.IsNullOrEmpty(block.XLabel) ? "X" : block.XLabel, ColumnKind.X);
        table.DataColumns.Add(yCol, string.IsNullOrEmpty(block.YLabel) ? "Y" : block.YLabel, ColumnKind.V);

        if (block.CreationTime != DateTime.MinValue)
        {
          table.PropCols.Add(new DateTimeColumn(), "Date", ColumnKind.V);
          table.PropCols["Date"][1] = block.CreationTime;
        }

        if (!string.IsNullOrEmpty(block.XUnit) || !string.IsNullOrEmpty(block.YUnit))
        {
          table.PropCols.Add(new TextColumn(), "Unit", ColumnKind.V);
          table.PropCols["Unit"][0] = block.XUnit;
          table.PropCols["Unit"][1] = block.YUnit;
        }

        if (!string.IsNullOrEmpty(block.XLabel) || !string.IsNullOrEmpty(block.YLabel))
        {
          table.PropCols.Add(new TextColumn(), "Label", ColumnKind.V);
          table.PropCols["Label"][0] = block.XLabel;
          table.PropCols["Label"][1] = block.YLabel;
        }

        xCol.Data = block.XValues;
        yCol.Data = block.YValues;
      }
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
  }
}
