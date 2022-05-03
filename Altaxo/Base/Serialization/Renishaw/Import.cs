#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger, T.Tian, Alex Henderson
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Data;

namespace Altaxo.Serialization.Renishaw
{
  /// <summary>
  /// Supplies static methods to import Renishaw .wdf files.
  /// </summary>
  public class Import
  {
    /// <summary>
    /// Shows the SPC file import dialog, and imports the files to the table if the user clicked on "OK".
    /// </summary>
    /// <param name="table">The table to import the SPC files to.</param>
    public static void ShowDialog(Altaxo.Data.DataTable table)
    {
      var options = new Altaxo.Gui.OpenFileOptions();
      options.AddFilter("*.wdf", "Renishaw WiRE™ files (*.wdf)");
      options.AddFilter("*.*", "All files (*.*)");
      options.FilterIndex = 0;
      options.Multiselect = true; // allow selecting more than one file

      if (Current.Gui.ShowOpenFileDialog(options))
      {
        // if user has clicked ok, import all selected files into Altaxo
        string[] filenames = options.FileNames;
        Array.Sort(filenames); // Windows seems to store the filenames reverse to the clicking order or in arbitrary order

        string? errors = ImportRenishawWdfFiles(filenames, table);

        if (errors is not null)
        {
          Current.Gui.ErrorMessageBox(errors, "Some errors occured during import!");
        }
      }
    }

    /// <returns>Null if no error occurs, or an error description.</returns>
    public static string? ImportRenishawWdfFiles(string[] filenames, Altaxo.Data.DataTable table)
    {
      DoubleColumn? xcol = null;
      var errorList = new System.Text.StringBuilder();
      int lastColumnGroup = 0;

      if (table.DataColumns.ColumnCount > 0)
      {
        lastColumnGroup = table.DataColumns.GetColumnGroup(table.DataColumns.ColumnCount - 1);
        Altaxo.Data.DataColumn? xColumnOfRightMost = table.DataColumns.FindXColumnOfGroup(lastColumnGroup);
        if (xColumnOfRightMost is Altaxo.Data.DoubleColumn dcolMostRight)
          xcol = dcolMostRight;
      }

      foreach (string filename in filenames)
      {
        WdfFileReader wdfFile;
        try
        {
          wdfFile = WdfFileReader.FromFileName(filename);
        }
        catch(Exception ex)
        {
          errorList.Append(ex.Message);
          continue;
        }

        var xvalues = wdfFile.XData;
        var yvalues = wdfFile.Spectra;

        // Add the necessary property columns
        // TODO: which properties should be converted to property columns?
        // for instance: Scan Positions, etc.

        // first look if our default xcolumn matches the xvalues
        bool bMatchsXColumn = xcol is not null && ValuesMatch(xvalues, xcol);
        if (!bMatchsXColumn)
        {
          // if no match, then consider all xcolumns from right to left, maybe some fits
          for (int ncol = table.DataColumns.ColumnCount - 1; ncol >= 0; ncol--)
          {
            if ((ColumnKind.X == table.DataColumns.GetColumnKind(ncol)) &&
              (table.DataColumns[ncol] is DoubleColumn xdc) &&
              (ValuesMatch(xvalues, xdc))
              )
            {
              xcol = xdc;
              lastColumnGroup = table.DataColumns.GetColumnGroup(xcol);
              bMatchsXColumn = true;
              break;
            }
          }
        }

        // if there is still no matching x-column, create a new x column 
        if (!bMatchsXColumn)
        {
          xcol = new Altaxo.Data.DoubleColumn();
          xcol.CopyDataFrom(xvalues);
          lastColumnGroup = table.DataColumns.GetUnusedColumnGroupNumber();
          table.DataColumns.Add(xcol, "X", Altaxo.Data.ColumnKind.X, lastColumnGroup);
        }

        // now add the y-values
        for (int iSpectrum = 0; iSpectrum < wdfFile.Count; iSpectrum++)
        {
          var ycol = new Altaxo.Data.DoubleColumn();
          ycol.CopyDataFrom(wdfFile.GetSpectrum(iSpectrum));
          table.DataColumns.Add(ycol,
                                table.DataColumns.FindUniqueColumnName(System.IO.Path.GetFileNameWithoutExtension(filename)),
                                Altaxo.Data.ColumnKind.V,
                                lastColumnGroup);

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
      } // foreach file

      return errorList.Length == 0 ? null : errorList.ToString();
    }

    /// <summary>
    /// Compare the values in a double array with values in a double column and see if they match.
    /// </summary>
    /// <param name="values">An array of double values.</param>
    /// <param name="col">A double column to compare with the double array.</param>
    /// <returns>True if the length of the array is equal to the length of the <see cref="DoubleColumn" /> and the values in
    /// both array match to each other, otherwise false.</returns>
    public static bool ValuesMatch(float[] values, DoubleColumn col)
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
