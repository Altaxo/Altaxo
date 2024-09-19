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
using Altaxo.Data;

namespace Altaxo.Serialization.Galactic
{
  /// <summary>
  /// Provides methods and internal structures to be able to import Galactic (R) SPC files.
  /// </summary>
  public class GalacticSPCReader
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

    public double[]? XValues { get; private set; }

    public IReadOnlyList<double[]> YValues { get; private set; } = [];

    public string? ErrorMessages { get; private set; }

    /// <summary>
    /// Constructs the reader by reading in a stream.
    /// </summary>
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
        double[] xvalues=null;
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
