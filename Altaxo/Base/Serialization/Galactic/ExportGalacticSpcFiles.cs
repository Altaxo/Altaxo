﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

#nullable enable
using System;
using Altaxo.Collections;

namespace Altaxo.Serialization.Galactic
{
  namespace Options
  {
    /// <summary>
    /// Creating a spectrum either from a row or from a column.
    /// </summary>
    /// <remarks>Choosing creating a spectrum from a row, the values of a single table row (or parts of it) are used to create
    /// a spectrum, i.e. the y-values of the spectrum. In this case the x-values can come from a numeric property column.<para/>
    /// Creating a spectrum from a column (or parts of a column, i.e. some rows of it) means the y-values
    /// of the spectrum stem from a single column of the data table. The x-values of the spectrum then have
    /// to come from another (numeric) column of the table.</remarks>
    public enum CreateSpectrumFrom
    {
      /// <summary>The y-values of one spectrum stem from one single table row.</summary>
      Row,

      /// <summary>The y-values of one spectrum stem from one single table column.</summary>
      Column
    };

    /// <summary>Designates the source of the x data values.</summary>
    public enum XValuesFrom
    {
      /// <summary>The x data values are continuous numbers starting from 1.</summary>
      ContinuousNumber,

      /// <summary>The x data values are from a (numeric) column.</summary>
      Column
    };

    /// <summary>The option for file name extension.</summary>
    public enum ExtendFileNameWith
    {
      /// <summary>The file name is extended with a continuously increasing number.</summary>
      ContinuousNumber,

      /// <summary>The file name is extended by the contents of a data column.</summary>
      Column
    };
  }

  /// <summary>
  /// This class hosts all routines neccessary to export Galactic SPC files
  /// </summary>
  public class Export
  {
    /// <summary>
    /// Exports a couple of x and y values into a non-evenly spaced Galactic SPC file.
    /// </summary>
    /// <param name="xvalues">The x values of the spectrum.</param>
    /// <param name="yvalues">The y values of the spectrum.</param>
    /// <param name="filename">The filename where to export to.</param>
    /// <returns>Null if successful, otherwise an error description.</returns>
    public static string? FromArrays(double[] xvalues, double[] yvalues, string filename)
    {
      int len = xvalues.Length < yvalues.Length ? xvalues.Length : yvalues.Length;

      if (len == 0)
      {
        return "Nothing to export - either x-value or y-value array is empty!";
      }

      System.IO.Stream? stream = null;

      try
      {
        stream = new System.IO.FileStream(filename, System.IO.FileMode.CreateNew);
        var binwriter = new System.IO.BinaryWriter(stream);

        binwriter.Write((byte)0x80); // ftflgs : not-evenly spaced data
        binwriter.Write((byte)0x4B); // fversn : new version
        binwriter.Write((byte)0x00); // fexper : general experimental technique
        binwriter.Write((byte)0x80); // fexp   : fractional scaling exponent (0x80 for floating point)

        binwriter.Write(len); // fnpts  : number of points

        binwriter.Write(xvalues[0]); // ffirst : first x-value
        binwriter.Write(xvalues[len - 1]); // flast : last x-value
        binwriter.Write(1); // fnsub : 1 (one) subfile only

        binwriter.Write((byte)0); //  Type of X axis units (see definitions below)
        binwriter.Write((byte)0); //  Type of Y axis units (see definitions below)
        binwriter.Write((byte)0); // Type of Z axis units (see definitions below)
        binwriter.Write((byte)0); // Posting disposition (see GRAMSDDE.H)

        binwriter.Write(new byte[0x1E0]); // writing rest of SPC header

        // ---------------------------------------------------------------------
        //   following the x-values array
        // ---------------------------------------------------------------------

        for (int i = 0; i < len; i++)
          binwriter.Write((float)xvalues[i]);

        // ---------------------------------------------------------------------
        //   following the y SUBHEADER
        // ---------------------------------------------------------------------

        binwriter.Write((byte)0); // subflgs : always 0
        binwriter.Write((byte)0x80); // subexp : y-values scaling exponent (set to 0x80 means floating point representation)
        binwriter.Write((short)0); // subindx :  Integer index number of trace subfile (0=first)

        binwriter.Write((float)0); // subtime;   Floating time for trace (Z axis corrdinate)
        binwriter.Write((float)0); // subnext;   Floating time for next trace (May be same as beg)
        binwriter.Write((float)0); // subnois;   Floating peak pick noise level if high byte nonzero

        binwriter.Write(0); // subnpts;  Integer number of subfile points for TXYXYS type
        binwriter.Write(0); // subscan; Integer number of co-added scans or 0 (for collect)
        binwriter.Write((float)0);        // subwlevel;  Floating W axis value (if fwplanes non-zero)
        binwriter.Write(0); // subresv[4];   Reserved area (must be set to zero)

        // ---------------------------------------------------------------------
        //   following the y-values array
        // ---------------------------------------------------------------------

        for (int i = 0; i < len; i++)
          binwriter.Write((float)yvalues[i]);
      }
      catch (Exception e)
      {
        return e.ToString();
      }
      finally
      {
        if (stream is not null)
          stream.Close();
      }

      return null;
    }

    /// <summary>
    /// Exports to a single SPC spectrum from a single table row.
    /// </summary>
    /// <param name="filename">The name of the file where to export to.</param>
    /// <param name="table">The table from which to export.</param>
    /// <param name="rownumber">The number of the table row that contains the data to export.</param>
    /// <param name="xcolumn">The x column that contains the x data.</param>
    /// <param name="selectedColumns">The columns that where selected in the table, i.e. the columns which are exported. If this parameter is null
    /// or no columns are selected, then all data of a row will be exported.</param>
    /// <returns>Null if export was successfull, error description otherwise.</returns>
    public static string? FromRow(
      string filename,
      Altaxo.Data.DataTable table,
      int rownumber,
      Altaxo.Data.INumericColumn xcolumn,
      IAscendingIntegerCollection selectedColumns)
    {
      // test that all x and y cells have numeric values
      var useSel = selectedColumns is not null && selectedColumns.Count > 0 ? selectedColumns : null;
      int spectrumlen = useSel is null ? table.DataColumns.ColumnCount : useSel.Count;

      int i, j;

      for (j = 0; j < spectrumlen; j++)
      {
        i = useSel is null ? j : useSel[j];

        if (xcolumn[i] == double.NaN)
          return $"X column at index {i} has no numeric value!";

        if (!(table[i] is Altaxo.Data.INumericColumn))
          return $"Table column[{i}] ({table[i].FullName}) is not a numeric column!";

        if (double.IsNaN(((Altaxo.Data.INumericColumn)table[i])[rownumber]))
          return $"Table cell [{i},{rownumber}] (column {table[i].FullName}) has no numeric value!";
      }

      // this first test was successfull, so start exporting now

      double[] xvalues = new double[spectrumlen];
      double[] yvalues = new double[spectrumlen];

      for (j = 0; j < spectrumlen; j++)
      {
        i = useSel is null ? j : useSel[j];
        xvalues[j] = xcolumn[i];
        yvalues[j] = ((Altaxo.Data.INumericColumn)table[i])[rownumber];
      }
      return FromArrays(xvalues, yvalues, filename);
    }

    /// <summary>
    /// Exports to a single SPC spectrum from a single table column.
    /// </summary>
    /// <param name="filename">The name of the file where to export to.</param>
    /// <param name="table">The table from which to export.</param>
    /// <param name="columnnumber">The number of the table column that contains the data to export.</param>
    /// <param name="xcolumn">The x column that contains the x data.</param>
    /// <param name="selectedRows">The rows that where selected in the table, i.e. the rows which are exported. If this parameter is null
    /// or no rows are selected, then all data of a column will be exported.</param>
    /// <returns>Null if export was successfull, error description otherwise.</returns>
    public static string? FromColumn(
      string filename,
      Altaxo.Data.DataTable table,
      int columnnumber,
      Altaxo.Data.INumericColumn xcolumn,
      IAscendingIntegerCollection selectedRows)
    {
      if (!(table.DataColumns[columnnumber] is Altaxo.Data.INumericColumn))
        return string.Format("Table column[{0}] ({1}) is not a numeric column!", columnnumber, table.DataColumns[columnnumber].FullName);

      // test that all x and y cells have numeric values
      var useSel = selectedRows is not null && selectedRows.Count > 0 ? selectedRows : null;
      int spectrumlen = useSel is null ? table.DataColumns[columnnumber].Count : useSel.Count;

      int i, j;

      for (j = 0; j < spectrumlen; j++)
      {
        i = useSel is null ? j : useSel[j];

        if (double.IsNaN(xcolumn[i]))
          return $"X column at index {i} has no numeric value!";

        if (((Altaxo.Data.INumericColumn)table.DataColumns[columnnumber])[i] == double.NaN)
          return $"Table cell [{columnnumber},{i}] (column {table.DataColumns[columnnumber].FullName}) has no numeric value!";
      }

      // this first test was successfull, so start exporting now

      double[] xvalues = new double[spectrumlen];
      double[] yvalues = new double[spectrumlen];

      for (j = 0; j < spectrumlen; j++)
      {
        i = useSel is null ? j : useSel[j];
        xvalues[j] = xcolumn[i];
        yvalues[j] = ((Altaxo.Data.INumericColumn)table.DataColumns[columnnumber])[i];
      }
      return FromArrays(xvalues, yvalues, filename);
    }
  } // end of class Export
}
