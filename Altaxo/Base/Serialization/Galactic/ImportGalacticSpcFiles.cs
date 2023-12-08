#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using Altaxo.Data;

namespace Altaxo.Serialization.Galactic
{
  /// <summary>
  /// Provides methods and internal structures to be able to import Galactic (R) SPC files.
  /// </summary>
  public class Import
  {
    [Flags]
    public enum Ftflgs : byte
    {
      TSPREC = 0x01,
      TCGRAM = 0x02,
      TMULTI = 0x04,
      TRANDM = 0x08,
      TORDRD = 0x10,
      TALABS = 0x20,
      TXYXYS = 0x40,
      TXVALS = 0x80,
    };


    /// <summary>
    /// The main header structure of the SPC file. This structure is located at the very beginning of the file.
    /// </summary>
    public struct SPCHDR
    {
      public Ftflgs ftflgs;
      public byte fversn;
      public byte fexper;
      public byte fexp;
      public int fnpts;
      public double ffirst;
      public double flast;
      public int fnsub;
    }

    /// <summary>
    /// This structure describes a Subheader, i.e. a single spectrum.
    /// </summary>
    public struct SUBHDR
    {
      /// <summary> subflgs : always 0</summary>
      public byte subflgs;

      /// <summary> subexp : y-values scaling exponent (set to 0x80 means floating point representation)</summary>
      public byte subexp;

      /// <summary>subindx :  Integer index number of trace subfile (0=first)</summary>
      public short subindx;

      /// <summary>subtime;  Floating time for trace (Z axis corrdinate)</summary>
      public float subtime;

      /// <summary>subnext;  Floating time for next trace (May be same as beg)</summary>
      public float subnext;

      /// <summary>subnois;  Floating peak pick noise level if high byte nonzero </summary>
      public float subnois;

      /// <summary>subnpts;  Integer number of subfile points for TXYXYS type </summary>
      public int subnpts;

      /// <summary>subscan; Integer number of co-added scans or 0 (for collect) </summary>
      public int subscan; //

      /// <summary>subwlevel;  Floating W axis value (if fwplanes non-zero) </summary>
      public float subwlevel;

      /// <summary>subresv[4];   Reserved area (must be set to zero) </summary>
      public int subresv;
    }

    /// <summary>
    /// Imports a Galactic SPC file into a x and an y array. The file must not be a multi spectrum file (an exception is thrown in this case).
    /// </summary>
    /// <param name="xvalues">The x values of the spectrum.</param>
    /// <param name="yvalues">The y values of the spectrum.</param>
    /// <param name="filename">The filename where to import from.</param>
    /// <returns>Null if successful, otherwise an error description.</returns>
    public static string? ToArrays(string filename, out double[]? xvalues, out List<double[]> listOfYArrays)
    {
      System.IO.Stream? stream = null;

      var spchdr = new SPCHDR();
      var subhdr = new SUBHDR();

      listOfYArrays = new List<double[]>();

      try
      {
        stream = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
        var binreader = new System.IO.BinaryReader(stream);

        spchdr.ftflgs = (Ftflgs)binreader.ReadByte(); // ftflgs : not-evenly spaced data
        spchdr.fversn = binreader.ReadByte(); // fversn : new version
        spchdr.fexper = binreader.ReadByte(); // fexper : general experimental technique
        spchdr.fexp = binreader.ReadByte(); // fexp   : fractional scaling exponent (0x80 for floating point)

        spchdr.fnpts = binreader.ReadInt32(); // fnpts  : number of points

        spchdr.ffirst = binreader.ReadDouble(); // ffirst : first x-value
        spchdr.flast = binreader.ReadDouble(); // flast : last x-value
        spchdr.fnsub = binreader.ReadInt32(); // fnsub : 1 (one) subfile only

        binreader.ReadByte(); //  Type of X axis units (see definitions below)
        binreader.ReadByte(); //  Type of Y axis units (see definitions below)
        binreader.ReadByte(); // Type of Z axis units (see definitions below)
        binreader.ReadByte(); // Posting disposition (see GRAMSDDE.H)

        binreader.Read(new byte[0x1E0], 0, 0x1E0); // rest of SPC header

        // ---------------------------------------------------------------------
        //   following the x-values array
        // ---------------------------------------------------------------------

        if (spchdr.fversn != 0x4B)
        {
          if (spchdr.fversn == 0x4D)
            throw new System.FormatException(string.Format("This SPC file has the old format version of {0}, the only version supported here is the new version {1}", spchdr.fversn, 0x4B));
          else
            throw new System.FormatException(string.Format("This SPC file has a version of {0}, the only version recognized here is {1}", spchdr.fversn, 0x4B));
        }

        if (spchdr.ftflgs.HasFlag(Ftflgs.TXVALS))
        {
          xvalues = new double[spchdr.fnpts];
          for (int i = 0; i < spchdr.fnpts; i++)
            xvalues[i] = binreader.ReadSingle();
        }
        else  // evenly spaced data
        {
          xvalues = new double[spchdr.fnpts];
          for (int i = 0; i < spchdr.fnpts; i++)
            xvalues[i] = spchdr.ffirst + i * (spchdr.flast - spchdr.ffirst) / (spchdr.fnpts - 1);
        }

        int numberOfSubFiles = 1;
        if (spchdr.ftflgs.HasFlag(Ftflgs.TMULTI))
          numberOfSubFiles = spchdr.fnsub;


        for (int idxSubFile = 0; idxSubFile < numberOfSubFiles; idxSubFile++)
        {

          // ---------------------------------------------------------------------
          //   following the y SUBHEADER
          // ---------------------------------------------------------------------

          subhdr.subflgs = binreader.ReadByte(); // subflgs : always 0
          subhdr.subexp = binreader.ReadByte(); // subexp : y-values scaling exponent (set to 0x80 means floating point representation)
          subhdr.subindx = binreader.ReadInt16(); // subindx :  Integer index number of trace subfile (0=first)

          subhdr.subtime = binreader.ReadSingle(); // subtime;   Floating time for trace (Z axis corrdinate)
          subhdr.subnext = binreader.ReadSingle(); // subnext;   Floating time for next trace (May be same as beg)
          subhdr.subnois = binreader.ReadSingle(); // subnois;   Floating peak pick noise level if high byte nonzero

          subhdr.subnpts = binreader.ReadInt32(); // subnpts;  Integer number of subfile points for TXYXYS type
          subhdr.subscan = binreader.ReadInt32(); // subscan; Integer number of co-added scans or 0 (for collect)
          subhdr.subwlevel = binreader.ReadSingle();        // subwlevel;  Floating W axis value (if fwplanes non-zero)
          subhdr.subresv = binreader.ReadInt32(); // subresv[4];   Reserved area (must be set to zero)

          // ---------------------------------------------------------------------
          //   following the y-values array
          // ---------------------------------------------------------------------
          var yvalues = new double[spchdr.fnpts];

          if (spchdr.fexp == 0x80) //floating point format
          {
            for (int i = 0; i < spchdr.fnpts; i++)
              yvalues[i] = binreader.ReadSingle();
          }
          else // fixed exponent format
          {
            for (int i = 0; i < spchdr.fnpts; i++)
              yvalues[i] = binreader.ReadInt32() * Math.Pow(2, spchdr.fexp - 32);
          }

          listOfYArrays.Add(yvalues);
        }
      }
      catch (Exception e)
      {
        xvalues = null;
        listOfYArrays.Clear();
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

    /// <summary>
    /// Imports a couple of SPC files into a table. The spectra are added as columns to the table. If the x column
    /// of the rightmost column does not match the x-data of the spectra, a new x-column is also created.
    /// </summary>
    /// <param name="filenames">An array of filenames to import.</param>
    /// <param name="table">The table the spectra should be imported to.</param>
    /// <returns>Null if no error occurs, or an error description.</returns>
    public static string? ImportSpcFiles(string[] filenames, Altaxo.Data.DataTable table, GalacticSPCImportOptions importOptions)
    {
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
        string? error = ToArrays(filename, out var xvalues, out var yvalues);
        if (error is not null)
        {
          errorList.Append(error);
          continue;
        }
        if (xvalues is null || yvalues is null)
          throw new InvalidProgramException();



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
          table.DataColumns.Add(xcol, "X", Altaxo.Data.ColumnKind.X, lastColumnGroup);
        }

        // now add the y-values

        for (int iSpectrum = 0; iSpectrum < yvalues.Count; ++iSpectrum)
        {
          string columnName = importOptions.UseNeutralColumnName ?
                              $"{(string.IsNullOrEmpty(importOptions.NeutralColumnName) ? "Y" : importOptions.NeutralColumnName)}{idxYColumn}" :
                              System.IO.Path.GetFileNameWithoutExtension(filename);
          columnName = table.DataColumns.FindUniqueColumnName(columnName);
          var ycol = table.DataColumns.EnsureExistence(columnName, typeof(DoubleColumn), ColumnKind.V, lastColumnGroup);
          ++idxYColumn;
          ycol.CopyDataFrom(yvalues[iSpectrum]);

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
        if (filenames.Length == 1)
          table.Notes.WriteLine($"Imported from {filenames[0]} at {DateTimeOffset.Now}");
        else if (filenames.Length > 1)
          table.Notes.WriteLine($"Imported from {filenames[0]} and more ({filenames.Length} files) at {DateTimeOffset.Now}");
      }

      return errorList.Length == 0 ? null : errorList.ToString();
    }

    /// <summary>
    /// Shows the SPC file import dialog, and imports the files to the table if the user clicked on "OK".
    /// </summary>
    /// <param name="table">The table to import the SPC files to.</param>
    public static void ShowDialog(Altaxo.Data.DataTable table)
    {
      var options = new Altaxo.Gui.OpenFileOptions();
      options.AddFilter("*.spc", "Galactic SPC files (*.spc)");
      options.AddFilter("*.*", "All files (*.*)");
      options.FilterIndex = 0;
      options.Multiselect = true; // allow selecting more than one file

      if (Current.Gui.ShowOpenFileDialog(options))
      {
        // if user has clicked ok, import all selected files into Altaxo
        string[] filenames = options.FileNames;
        Array.Sort(filenames); // Windows seems to store the filenames reverse to the clicking order or in arbitrary order

        var importOptions = new GalacticSPCImportOptions();
        string? errors = ImportSpcFiles(filenames, table, importOptions);

        table.DataSource = new GalacticSPCImportDataSource(filenames, importOptions);

        if (errors is not null)
        {
          Current.Gui.ErrorMessageBox(errors, "Some errors occured during import!");
        }
      }
    }
  }
}
