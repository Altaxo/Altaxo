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
using System.IO;

namespace Altaxo.Serialization.Galactic
{
  /// <summary>
  /// Provides methods and internal structures to be able to import Galactic (R) SPC files.
  /// </summary>
  public class GalacticSPCReader
  {
    /// <summary>
    /// Flags present in the SPC file header that describe file and axis formats.
    /// </summary>
    [Flags]
    public enum Ftflgs : byte
    {
      /// <summary>
      /// Unknown / additional precision flag for time series (TSPREC).
      /// </summary>
      TSPREC = 0x01,

      /// <summary>
      /// Indicates that X axis represents a CGRAM (TCGRAM).
      /// </summary>
      TCGRAM = 0x02,

      /// <summary>
      /// Multi-spectrum file flag (TMULTI).
      /// </summary>
      TMULTI = 0x04,

      /// <summary>
      /// Random order flag (TRANDM).
      /// </summary>
      TRANDM = 0x08,

      /// <summary>
      /// Ordered data flag (TORDRD).
      /// </summary>
      TORDRD = 0x10,

      /// <summary>
      /// Absolute scaling flag (TALABS).
      /// </summary>
      TALABS = 0x20,

      /// <summary>
      /// X and Y values stored together (TXYXYS).
      /// </summary>
      TXYXYS = 0x40,

      /// <summary>
      /// Explicit X values are present in the file (TXVALS).
      /// </summary>
      TXVALS = 0x80,
    };


    /// <summary>
    /// The main header structure of the SPC file. This structure is located at the very beginning of the file.
    /// </summary>
    public struct SPCHDR
    {
      /// <summary>
      /// File flags that describe axis and file format (see <see cref="Ftflgs"/>).
      /// </summary>
      public Ftflgs ftflgs;

      /// <summary>
      /// File version byte.
      /// </summary>
      public byte fversn;

      /// <summary>
      /// General experimental technique code.
      /// </summary>
      public byte fexper;

      /// <summary>
      /// Fractional scaling exponent for Y values (0x80 indicates floating point storage).
      /// </summary>
      public byte fexp;

      /// <summary>
      /// Number of points in each spectrum.
      /// </summary>
      public int fnpts;

      /// <summary>
      /// First X value (start of the X axis).
      /// </summary>
      public double ffirst;

      /// <summary>
      /// Last X value (end of the X axis).
      /// </summary>
      public double flast;

      /// <summary>
      /// Number of subfiles (spectra) contained in the file.
      /// </summary>
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
    /// Gets the X-axis values for the currently read spectrum (or null if reading failed).
    /// </summary>
    public double[]? XValues { get; private set; }

    /// <summary>
    /// Gets the Y-axis values as a read-only list of arrays. For multi-spectrum files,
    /// each array represents one subfile (spectrum).
    /// </summary>
    public IReadOnlyList<double[]> YValues { get; private set; } = [];

    /// <summary>
    /// Gets error information collected during reading, or null when no error occurred.
    /// </summary>
    public string? ErrorMessages { get; private set; }

    /// <summary>
    /// Constructs the reader by reading in a stream.
    /// </summary>
    /// <param name="stream">A seekable stream containing SPC file data.</param>
    public GalacticSPCReader(Stream stream)
    {

      var spchdr = new SPCHDR();
      var subhdr = new SUBHDR();

      try
      {
        using var binreader = new System.IO.BinaryReader(stream);

        spchdr.ftflgs = (Ftflgs)binreader.ReadByte(); // ftflgs : not-evenly spaced data
        spchdr.fversn = binreader.ReadByte(); // fversn : new version
        spchdr.fexper = binreader.ReadByte(); // fexper : general experimental technique
        spchdr.fexp = binreader.ReadByte(); // fexp   : fractional scaling exponent (0x80 for floating point)

        spchdr.fnpts = binreader.ReadInt32(); // fnpts  : number of points

        spchdr.ffirst = binreader.ReadDouble(); // ffirst : first x-value
        spchdr.flast = binreader.ReadDouble(); // flast : last x-value
        spchdr.fnsub = binreader.ReadInt32(); // fnsub : 1 (one) subfile only

        var xunit = binreader.ReadByte(); //  Type of X axis units (see definitions below)
        var yunit = binreader.ReadByte(); //  Type of Y axis units (see definitions below)
        var zunit = binreader.ReadByte(); // Type of Z axis units (see definitions below)
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

        if (!(spchdr.ftflgs.HasFlag(Ftflgs.TMULTI)) && (spchdr.ftflgs.HasFlag(Ftflgs.TRANDM) || spchdr.ftflgs.HasFlag(Ftflgs.TORDRD)))
        {
          throw new System.FormatException($"This SPC file has the TRANDM or TORDRD flag set, but not the TMULTI flag.");
        }

        if (spchdr.fnpts < 0)
        {
          throw new System.FormatException($"This SPC file has no sensible number of points: {spchdr.fnpts}");
        }
        if (spchdr.fnsub < 0)
        {
          throw new System.FormatException($"This SPC file has a negative number of subfiles: {spchdr.fnsub}");
        }
        else if (spchdr.fnsub > 65536)
        {
          throw new System.FormatException($"This SPC file has a too high number of subfiles: {spchdr.fnsub}");
        }

        if (spchdr.fexp != 0x80)
        {
          throw new System.NotImplementedException($"This SPC file has integer formatted values, which is not supported yet.");
        }

        double[] xvalues = null;
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

        var listOfYArrays = new List<double[]>();

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

        XValues = xvalues;
        YValues = listOfYArrays;
      }
      catch (Exception e)
      {
        XValues = null;
        YValues = [];
        ErrorMessages = e.ToString();
      }
    }
  }
}
