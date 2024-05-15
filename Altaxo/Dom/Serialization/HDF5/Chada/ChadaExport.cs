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
using Altaxo.Data;
using PureHDF;

namespace Altaxo.Serialization.HDF5.Chada
{
  /// <summary>
  /// Export a Chada file. Chada files are very simply structured HDF5 files, with only one or two datasets, consisting of two columns.
  /// </summary>
  public class ChadaExport
  {

    /// <summary>
    /// Exports a table to RamanChada.
    /// </summary>
    /// <param name="dataTable">The data table. Must consist of exactly two data columns.</param>
    /// <param name="fileName">Name of the file to write to.</param>
    /// <returns></returns>
    public static string? ExportRamanChada(DataTable dataTable, string fileName)
    {
      var col = dataTable.DataColumns;
      if (col.ColumnCount != 2)
      {
        return $"The provided table '{dataTable.Name}' has {dataTable.DataColumnCount} columns, but the CHADA format only supports 2 columns.";
      }
      if (col[0] is not DoubleColumn xCol)
      {
        return $"The provided table's '{dataTable.Name}' first column is of type {col[0].GetType()}, but the CHADA format only supports numeric columns.";
      }
      if (col[1] is not DoubleColumn yCol)
      {
        return $"The provided table's '{dataTable.Name}' 2nd column is of type {col[1].GetType()}, but the CHADA format only supports numeric columns.";
      }

      var len = Math.Max(xCol.Count, xCol.Count);
      var dataArr = new double[2 * len];
      for (int i = 0; i < len; ++i)
      {
        dataArr[i] = xCol[i];
        dataArr[i + len] = yCol[i];
      }


      try
      {
        // len = 3;
        // dataArr = new double[len];
        var dataset = new H5Dataset(dataArr, fileDims: [2, (ulong)len]);
        dataset.Attributes.Add("DIMENSION_LABELS", new string[] { col.GetColumnName(xCol), col.GetColumnName(yCol) });
        var file = new H5File()
        {
          ["raw"] = dataset,
        };
        file.Write(fileName);
      }
      catch (Exception ex)
      {
        return $"Exception while exporting {dataTable.Name} to Chada file {fileName}: {ex.Message}";
      }

      return null;
    }

    /// <summary>
    /// Shows the CHADA file export dialog, and imports the files to the table if the user clicked on "OK".
    /// </summary>
    /// <param name="dataTable">The table to export the Chada files to.</param>
    public static void ShowExportRamanChadaDialog(DataTable dataTable)
    {
      var options = new Altaxo.Gui.SaveFileOptions();
      options.AddFilter("*.cha", "Raman CHADA files (*.cha)");
      options.AddFilter("*.*", "All files (*.*)");
      options.FilterIndex = 0;

      if (Current.Gui.ShowSaveFileDialog(options))
      {
        // if user has clicked ok, import all selected files into Altaxo
        string filename = options.FileName;
        string? errors = ExportRamanChada(dataTable, filename);

        if (errors is not null)
        {
          Current.Gui.ErrorMessageBox(errors, "Some errors occured during export!");
        }
      }
    }
  }
}
