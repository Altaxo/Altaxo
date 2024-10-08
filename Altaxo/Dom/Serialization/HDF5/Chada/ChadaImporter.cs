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
using PureHDF;
using PureHDF.VOL.Native;

namespace Altaxo.Serialization.HDF5.Chada
{
  public record ChadaImporter : DataFileImporterBase
  {
    /// <inheritdoc/>
    public override (IReadOnlyList<string> FileExtensions, string Explanation) GetFileExtensions()
    {
      return ([".cha"], "Chada files (*.cha)");
    }

    /// <inheritdoc/>
    public override object CheckOrCreateImportOptions(object? importOptions)
    {
      return (importOptions as ChadaImportOptions) ?? new ChadaImportOptions();
    }

    /// <inheritdoc/>
    public override IAltaxoTableDataSource? CreateTableDataSource(IReadOnlyList<string> fileNames, object importOptions)
    {
      return null;
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
        var table = new DataTable();
        var result = Import([fileName], table, new ChadaImportOptions(), false);
        if (string.IsNullOrEmpty(result) &&
          table.DataColumnCount >= 1 && table.DataRowCount >= 1)
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
    /// Import a Chada file. Chada files are very simply structured HDF5 files, with only one or two datasets, consisting of two columns.
    /// </summary>
    public override string? Import(IReadOnlyList<string> fileNames, DataTable dataTable, object importOptionsObj, bool attachDataSource = true)
    {
      var stb = new StringBuilder();
      DataColumn? lastXColumn = null;
      int lastGroupNumber = -1;
      int lastYNumber = -1;
      for (int i = 0; i < fileNames.Count; ++i)
      {
        var err = ImportRamanCHADA(fileNames[i], dataTable, fileNames.Count, ref lastXColumn, ref lastGroupNumber, ref lastYNumber);
        if (!string.IsNullOrEmpty(err))
          stb.Append(err);
      }
      return stb.Length == 0 ? null : stb.ToString();
    }

    /// <summary>
    /// Imports a single RamanChada file.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="dataTable">The data table to write the data to.</param>
    /// <returns>Null if successful; otherwise the error message.</returns>
    public static string? ImportRamanCHADA(string fileName, DataTable dataTable)
    {
      DataColumn? lastXColumn = null;
      int lastGroupNumber = -1;
      int lastYNumber = -1;
      return ImportRamanCHADA(fileName, dataTable, 1, ref lastXColumn, ref lastGroupNumber, ref lastYNumber);
    }


    /// <summary>
    /// Imports a RamanChada file (aware of importing multiple files in one table).
    /// </summary>
    /// <param name="fileName">Name of the file to read from.</param>
    /// <param name="dataTable">The data table to write the data to.</param>
    /// <param name="totalNumberOfFiles">The total number of files in this import.</param>
    /// <param name="lastXColumn">The last x column. Can be null. At the first call to this function, the value should be null. Subsequent calls should provide the value returned from the previous call.</param>
    /// <param name="lastGroupNumber">The last group number. At the first call to this function, the value should be -1.  Subsequent calls should provide the value returned from the previous call.</param>
    /// <param name="lastNumberOfY">The last number of the y-column. At the first call to this function, the value should be -1.  Subsequent calls should provide the value returned from the previous call.</param>
    /// <returns>Null if successful; otherwise the error message.</returns>
    protected static string? ImportRamanCHADA(string fileName, DataTable dataTable, int totalNumberOfFiles, ref DataColumn? lastXColumn, ref int lastGroupNumber, ref int lastNumberOfY)
    {
      var file = H5File.OpenRead(fileName);
      var dataSet = file.Dataset("raw"); // there is exactly one dataset expected in the file, which is called 'raw'
      var labels = dataSet.Attribute("DIMENSION_LABELS"); // the data set has the attributes 'DIMENSION_LABELS' (string[2]), 'Original file' (string), 'CHADA generated on' (string)
      var dimension_labels = new string[2];
      labels.Read<string[]>(dimension_labels);
      var dataArrays = dataSet.Read<double[]>(); // multidimensional arrays only possible when using .NET6 or above
      var dimensions = dataSet.Space.Dimensions;

      var col = dataTable.DataColumns;

      if (dimensions.Length == 2 && dimensions[0] == 2)
      {
        var len = (int)dimensions[1];
        bool createNewX = lastXColumn is null || lastGroupNumber < 0;
        if (lastXColumn is not null)
        {
          for (int i = 0; i < len; ++i)
          {
            if (dataArrays[i] != lastXColumn[i])
            {
              createNewX = true;
              break;
            }
          }
        }

        if (createNewX)
        {
          lastGroupNumber++;
        }

        string xLabel = string.IsNullOrEmpty(dimension_labels[0].Trim()) ? "X" : dimension_labels[0].Trim();
        if (totalNumberOfFiles > 1)
        {
          xLabel += FormattableString.Invariant($".{lastGroupNumber}");
        }

        if (lastXColumn is null || createNewX)
        {
          lastXColumn = col.EnsureExistence(xLabel, typeof(DoubleColumn), ColumnKind.X, lastGroupNumber);
        }

        string yLabel = string.IsNullOrEmpty(dimension_labels[1].Trim()) ? "Y" : dimension_labels[1].Trim();

        ++lastNumberOfY;
        if (lastNumberOfY > 0)
        {
          yLabel += FormattableString.Invariant($".{lastNumberOfY}");
        }

        var yCol = col.EnsureExistence(yLabel, typeof(DoubleColumn), ColumnKind.V, lastGroupNumber);

        for (int i = 0; i < len; ++i)
        {
          lastXColumn[i] = dataArrays[i];
          yCol[i] = dataArrays[i + len];
        }
      }

      return null;
    }
  }
}
