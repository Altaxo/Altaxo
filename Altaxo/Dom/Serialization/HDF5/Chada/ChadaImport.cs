using System;
using Altaxo.Data;
using PureHDF;

namespace Altaxo.Serialization.HDF5.Chada
{
  public class ChadaImport
  {
    public static string? ImportRamanCHADA(string[] fileNames, DataTable dataTable)
    {
      var errors = ImportRamanCHADA(fileNames[0], dataTable);
      return errors;
    }

    public static string? ImportRamanCHADA(string fileName, DataTable dataTable)
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
        var xCol = col.EnsureExistence("X", typeof(DoubleColumn), ColumnKind.X, 0);
        var yCol = col.EnsureExistence("Y0", typeof(DoubleColumn), ColumnKind.V, 0);

        var len = (int)dimensions[1];
        for (int i = 0; i < len; ++i)
        {
          xCol[i] = dataArrays[i];
          yCol[i] = dataArrays[i + len];
        }
      }

      return null;
    }

    /// <summary>
    /// Shows the SPC file import dialog, and imports the files to the table if the user clicked on "OK".
    /// </summary>
    /// <param name="dataTable">The table to import the SPC files to.</param>
    public static void ShowImportRamanChadaDialog(DataTable dataTable)
    {
      var options = new Altaxo.Gui.OpenFileOptions();
      options.AddFilter("*.cha", "Raman CHADA files (*.cha)");
      options.AddFilter("*.*", "All files (*.*)");
      options.FilterIndex = 0;
      options.Multiselect = true; // allow selecting more than one file

      if (Current.Gui.ShowOpenFileDialog(options))
      {
        // if user has clicked ok, import all selected files into Altaxo
        string[] filenames = options.FileNames;
        Array.Sort(filenames); // Windows seems to store the filenames reverse to the clicking order or in arbitrary order

        string? errors = ImportRamanCHADA(filenames, dataTable);

        // table.DataSource = new RenishawImportDataSource(filenames, importOptions);

        if (errors is not null)
        {
          Current.Gui.ErrorMessageBox(errors, "Some errors occured during import!");
        }
      }
    }
  }
}
