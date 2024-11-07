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
using System.IO;
using System.Linq;

namespace Altaxo.Serialization.Renishaw
{
  /// <summary>
  /// Renishaw .wdf file reader. Renishaw WiRE (TM) .wdf files are used in Raman spectroscopy.
  /// </summary>
  /// <remarks>
  /// Code inspired by Henderson, Alex DOI:10.5281/zenodo.495477, and by the Python implementation <see href="https://github.com/alchem0x2A/py-wdf-reader/blob/master/renishawWiRE/wdfReader.py"/>
  /// <code>
  /// The wdf file format is separated into several DataBlocks, with starting 4-char 
  ///   strings such as (incomplete list):
  ///     `WDF1`: File header for information
  ///     `DATA`: Spectra data
  ///     `XLST`: Data for X-axis of data, usually the Raman shift or wavelength
  ///     `YLST`: Data for Y-axis of data, possibly not important
  ///     `WMAP`: Information for mapping, e.g.StreamLine or StreamLineHR mapping
  ///     `MAP `: Mapping information(?)
  ///     `ORGN`: Data for stage origin
  ///     `TEXT`: Annotation text etc
  ///     `WXDA`: ? TODO
  ///     `WXDM`: ? TODO
  ///     `ZLDC`: ? TODO
  ///     `BKXL`: ? TODO
  ///     `WXCS`: ? TODO
  ///     `WXIS`: ? TODO
  ///     `WHTL`: Whilte light image
  ///     Following the block name, there are two indicators:
  ///     Block uid: int32
  ///     Block size: int64
  ///     Args:
  ///     file_name(file) : File object for the wdf file
  ///    Attributes:
  ///     title(str) : Title of measurement
  ///    username(str) : Username
  ///   application_name(str) : Default WiRE
  ///     application_version(int,) * 4 : Version number, e.g. [4, 4, 0, 6602]
  ///    measurement_type(int) : Type of measurement
  ///                              0=unknown, 1=single, 2=multi, 3=mapping
  ///     scan_type(int) : Scan of type, see values in scan_types
  ///    laser_wavenumber(float32) : Wavenumber in cm^-1
  ///     count(int) : Numbers of experiments(same type), can be smaller than capacity
  ///   spectral_units(int) : Unit of spectra, see unit_types
  ///     xlist_type(int) : See unit_types
  ///     xlist_unit(int) : See unit_types
  ///     xlist_length(int) : Size for the xlist
  ///     xdata(numpy.array) : x-axis data
  ///     ylist_type(int) : Same as xlist_type
  ///     ylist_unit(int): Same as xlist_unit
  ///    ylist_length(int): Same as xlist_length
  ///   ydata(numpy.array): y-data, possibly not used
  ///  point_per_spectrum(int): Should be identical to xlist_length
  /// data_origin_count(int) : Number of rows in data origin list
  /// capacity(int) : Max number of spectra
  ///     accumulation_count(int) : Single or multiple measurements
  ///     block_info(dict) : Info block at least with following keys
  ///                        DATA, XLST, YLST, ORGN
  ///                         # TODO types?
  /// </code>
  /// </remarks>
  public class WdfFileReader
  {
    #region Inner classes

    private record class BlockInfo(int Uid, long Position, long Size)
    {
    }

    /// <summary>
    /// Offsets to the start of block
    /// </summary>
    private static class Offsets
    {
      // General offsets
      public const int block_name = 0x0;
      public const int block_id = 0x4;
      public const int block_data = 0x10;
      // offsets in WDF1 block
      public const int measurement_info = 0x3c;
      public const int spectral_info = 0x98;
      public const int file_info = 0xd0;
      public const int usr_name = 0xf0;
      public const int data_block = 0x200;
      // offsets in ORGN block
      public const int origin_info = 0x14;
      public const int origin_increment = 0x18;
      // offsets in WMAP block
      public const int wmap_origin = 0x18;
      public const int wmap_wh = 0x30;
      // offsets in WHTL block
      public const int jpeg_header = 0x10;
    }

    private enum ListKind
    {
      X,
      Y
    }

    /// <summary>
    /// Customized EXIF TAGS
    /// </summary>
    private enum ExifTags
    {

      // Standard EXIF TAGS
      FocalPlaneXResolution = 0xa20e,
      FocalPlaneYResolution = 0xa20f,
      FocalPlaneResolutionUnit = 0xa210,
      // Customized EXIF TAGS from Renishaw  
      FocalPlaneXYOrigins = 0xfea0,
      FieldOfViewXY = 0xfea1,
    }



    #endregion

    /// <summary>Gets the name of the Wdf file.</summary>
    public string FileName { get; private set; }

    /// <summary>Title of the measurement.</summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>Operator of the measurement.</summary>
    public string UserName { get; private set; } = string.Empty;

    /// <summary>Number of data points in each spectrum.</summary>
    public int NumberOfPointsPerSpectrum { get; private set; }

    /// <summary>Total number of experiments, can be greater than <see cref="Count"/>.</summary>
    public long Capacity { get; private set; }

    /// <summary>Number of experiments of the same type, can be smaller than <see cref="Capacity"/>.</summary>
    public long Count { get; private set; }

    /// <summary>True if the number of spectra in this file equals the total number of spectra for this measurement.</summary>
    public bool IsCompleted { get; private set; }

    /// <summary>Number of accumulations used for one spectrum.</summary>
    public int AccumulationCount { get; private set; }

    /// <summary>Number of rows in the data origin list.</summary>
    public int DataOriginCount { get; private set; }

    /// <summary>Name of the application that created this file.</summary>
    public string ApplicationName { get; private set; }

    /// <summary>Version of the application that created this file.</summary>
    public Version ApplicationVersion { get; private set; }

    /// <summary>Type of the scan that created this file.</summary>
    public ScanType ScanType { get; private set; }

    /// <summary>Type of the measurement that created this file.</summary>
    public MeasurementType MeasurementType { get; private set; }

    /// <summary>Unit of the y-values of the spectra.</summary>
    public UnitType SpectralUnit { get; private set; }

    /// <summary>Wavelength of the exciting laser in nm.</summary>
    public double LaserWavelength_nm { get; private set; }

    public double[] XPositions { get; private set; }
    public double[] YPositions { get; private set; }
    public double[] ZPositions { get; private set; }
    public UnitType XPositionUnit { get; private set; }
    public UnitType YPositionUnit { get; private set; }
    public UnitType ZPositionUnit { get; private set; }

    public (bool isY, DataType dataType, UnitType unitType, string annotation, Array array)[] OriginListHeader { get; private set; }



    /// <summary>
    /// Gets a value indicating whether this instance has map data.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance has map data; otherwise, <c>false</c>.
    /// </value>
    public bool HasMapData
    {
      get => _blockInfo.ContainsKey("WMAP");
    }

    public (int w, int h) MapShape { get; private set; }

    private Dictionary<string, object> _map_info = new();

    private IReadOnlyDictionary<string, object> MapInfo => _map_info;

    private (int Type, int Unit, int Length, float[] Data)[] listinfo = new (int Type, int Unit, int Length, float[] Data)[2];

    /// <summary>
    /// Gets the x data. This are usually the spectrum's x values (wavelength, shift, etc.).
    /// </summary>
    public float[] XData => listinfo[(int)ListKind.X].Data;

    /// <summary>
    /// Gets the y data. ATTENTION: this are usually <b>NOT</b> the spectrum's intensity values (these are stored in <see cref="Spectra"/>, or use <see cref="GetSpectrum(int)"/>).
    /// </summary>
    public float[] YData => listinfo[(int)ListKind.Y].Data;

    /// <summary>
    /// Gets the unit of the <see cref="XData"/> values.
    /// </summary>
    public UnitType XUnit => (UnitType)listinfo[(int)ListKind.X].Unit;

    /// <summary>
    /// Gets the unit of the <see cref="YData"/> values.
    /// </summary>
    public UnitType YUnit => (UnitType)listinfo[(int)ListKind.Y].Unit;


    /// <summary>
    /// Gets the spectra's intensity values.
    /// </summary>
    public float[][] Spectra { get; private set; } = [];

    /// <summary>
    /// Gets the spectrum's intensity values with index <paramref name="idx"/>. 
    /// </summary>
    /// <param name="idx">The index (must be &gt;=0 and &lt;<see cref="Count"/>).</param>
    /// <returns>The spectrum's intensity values.</returns>
    public float[] GetSpectrum(int idx)
    {
      return Spectra[idx];
    }

    /// <summary>
    /// Contains the images as bitstreams. See the remarks for an example how to use the bytes.
    /// </summary>
    /// <remarks>
    /// The following code will convert the Image #0 into an System.Drawing.Bitmap:
    /// <code>
    /// using var ms = new MemoryStream(Images[0]);
    /// var img = new System.Drawing.Bitmap(ms);
    /// var dict = new Dictionary&lt;int, System.Drawing.Imaging.PropertyItem&gt;();
    ///   foreach (var item in img.PropertyItems)
    ///     dict[item.Id] = item;
    /// </code>
    /// </remarks>
    public List<byte[]> Images { get; private set; } = new List<byte[]>();

    /// <summary>
    /// Contains information of all blocks. Key is the block's name. Value is a tuple of block's Uid, position, and size.
    /// </summary>
    private Dictionary<string, BlockInfo> _blockInfo = new();


    public static WdfFileReader FromFileName(string fileName)
    {
      using var rs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
      return FromStream(rs);
    }

    public static WdfFileReader FromStream(Stream rs)
    {
      return new WdfFileReader(rs);
    }

    public WdfFileReader(Stream rs)
    {
      if (rs is null)
        throw new ArgumentNullException(nameof(rs));
      if (!rs.CanSeek)
        throw new ArgumentException("Stream must be seekable", nameof(rs));

      if (rs is FileStream fs)
        FileName = fs.Name;

      LocateAllBlocks(rs);

      // Parse individual blocks
      TreatBlockData(rs, "WDF1");
      TreatBlockData(rs, "DATA");
      TreatBlockData(rs, "XLST");
      TreatBlockData(rs, "YLST");
      TreatBlockData(rs, "ORGN");
      TreatBlockData(rs, "WMAP", isOptional: true);
      TreatBlockData(rs, "WHTL", isOptional: true);
    }

    /// <summary>
    /// Get information for all data blocks and store them inside block_info
    /// </summary>
    /// <returns></returns>
    private void LocateAllBlocks(Stream sr)
    {

      long currentPosition = 0;
      while (currentPosition < sr.Length)
      {
        var (block_name, block_uid, block_size) = LocateSingleBlock(sr, currentPosition);
        if (block_size == 0)
        {
          throw new InvalidDataException("Block size of a single block is 0");
        }

        _blockInfo[block_name] = new BlockInfo(block_uid, currentPosition, block_size);
        currentPosition += block_size;
        sr.Seek(currentPosition, SeekOrigin.Begin);
      }
    }

    /// <summary>
    /// Gets the block information at the starting position
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="pos">The stream position.</param>
    /// <returns>Tuple consisting of the block's name, uid, and size.</returns>
    private (string Name, int Uid, long Size) LocateSingleBlock(Stream stream, long pos)
    {
      stream.Seek(pos, SeekOrigin.Begin);
      var buf = new byte[16];

/* Unmerged change from project 'AltaxoCore (net8.0)'
Before:
      stream.ForcedRead(buf, 0, buf.Length);
      var block_name = System.Text.ASCIIEncoding.ASCII.GetString(buf, 0, 4);
After:
      FileIOExtensions.ReadExactly(stream, buf, 0, buf.Length);
      var block_name = System.Text.ASCIIEncoding.ASCII.GetString(buf, 0, 4);
*/
      stream.ReadExactly(buf, 0, buf.Length);
      var block_name = System.Text.ASCIIEncoding.ASCII.GetString(buf, 0, 4);
      var block_uid = BitConverter.ToInt32(buf, 4);
      var block_size = BitConverter.ToInt64(buf, 8);
      return (block_name, block_uid, block_size);
    }

    /// <summary>
    /// Get data according to specific block name
    /// </summary>
    /// <param name="block_name">Name of the block.</param>
    /// <param name="isOptional">True if this block is optional, i.e. not mandatory.</param>
    /// <returns></returns>
    private void TreatBlockData(Stream sr, string block_name, bool isOptional = false)
    {
      if (!_blockInfo.TryGetValue(block_name, out var blockinfo))
      {
        if (isOptional)
          return;
        else
          throw new InvalidOperationException($"Block {block_name} is not present in the Wdf file.");
      }
      else
      {
        switch (block_name)
        {
          case "WDF1":
            ParseHeader(sr, blockinfo);
            break;
          case "DATA":
            ParseSpectra(sr, blockinfo);
            break;
          case "XLST":
            ParseXYList(sr, blockinfo, ListKind.X);
            break;
          case "YLST":
            ParseXYList(sr, blockinfo, ListKind.Y);
            break;
          case "ORGN":
            ParseOrginList(sr, blockinfo);
            break;
          case "WMAP":
            ParseWmap(sr, blockinfo);
            break;
          case "WHTL":
            ParseImg(sr, blockinfo);
            break;
          default:
            throw new ArgumentOutOfRangeException($"Unknown block type {block_name}");

        }
      }

    }

    /// <summary>
    /// Parses the header block WDF1
    /// </summary>
    /// <returns></returns>
    private void ParseHeader(Stream sr, BlockInfo blockinfo)
    {
      if (blockinfo.Size != Offsets.data_block)
      {
        throw new System.IO.InvalidDataException($"Unexpected size of header block");
      }

      sr.Seek(blockinfo.Position, SeekOrigin.Begin);
      var buf = new byte[blockinfo.Size];

/* Unmerged change from project 'AltaxoCore (net8.0)'
Before:
      sr.ForcedRead(buf, 0, buf.Length);
After:
      FileIOExtensions.ReadExactly(sr, buf, 0, buf.Length);
*/
      sr.ReadExactly(buf, 0, buf.Length);

      // TODO what are the digits in between?



      // The keys from the header
      NumberOfPointsPerSpectrum = BitConverter.ToInt32(buf, Offsets.measurement_info + 0);
      Capacity = BitConverter.ToInt64(buf, Offsets.measurement_info + 4);
      Count = BitConverter.ToInt64(buf, Offsets.measurement_info + 12);
      // If count < capacity, this measurement is not completed
      IsCompleted = (Count == Capacity);
      AccumulationCount = BitConverter.ToInt32(buf, Offsets.measurement_info + 20);
      listinfo[(int)ListKind.Y].Length = BitConverter.ToInt32(buf, Offsets.measurement_info + 24);
      listinfo[(int)ListKind.X].Length = BitConverter.ToInt32(buf, Offsets.measurement_info + 28);
      DataOriginCount = BitConverter.ToInt32(buf, Offsets.measurement_info + 32);
      ApplicationName = System.Text.Encoding.UTF8.GetString(buf, Offsets.measurement_info + 36, 24).TrimEnd('\0'); // Must be "WiRE"
      ApplicationVersion = new Version(
        BitConverter.ToUInt16(buf, Offsets.measurement_info + 60),
        BitConverter.ToUInt16(buf, Offsets.measurement_info + 62),
        BitConverter.ToUInt16(buf, Offsets.measurement_info + 64),
        BitConverter.ToUInt16(buf, Offsets.measurement_info + 66)
        );

      ScanType = (ScanType)BitConverter.ToInt32(buf, Offsets.measurement_info + 68);
      MeasurementType = (MeasurementType)BitConverter.ToInt32(buf, Offsets.measurement_info + 72);
      // For the units
      SpectralUnit = (UnitType)BitConverter.ToInt32(buf, Offsets.spectral_info + 0);
      LaserWavelength_nm = ConvertWavenumberToWavelength_nm(BitConverter.ToSingle(buf, Offsets.spectral_info + 4));  // in nm
                                                                                                                     // Username and title
      UserName = System.Text.Encoding.UTF8.GetString(buf, Offsets.file_info, Offsets.usr_name - Offsets.file_info).TrimEnd('\0');
      Title = System.Text.Encoding.UTF8.GetString(buf, Offsets.usr_name, Offsets.data_block - Offsets.usr_name).TrimEnd('\0');


    }

    /// <summary>
    /// Get information from DATA block
    /// </summary>
    /// <param name="sr">The sr.</param>
    /// <param name="blockinfo">The blockinfo.</param>
    /// <param name="start">The start.</param>
    /// <param name="end">The end.</param>
    /// <returns></returns>
    private void ParseSpectra(Stream sr, BlockInfo blockinfo, long start = 0, long end = -1)
    {
      end = (end < 0) ? (Count - 1) : end;        // take all spectra

      if (start < 0 || start >= Count)
        throw new ArgumentOutOfRangeException(nameof(start));
      if (end < 0 || end >= Count)
        throw new ArgumentOutOfRangeException(nameof(end));
      if (start > end)
        throw new ArgumentOutOfRangeException(nameof(start), $"{nameof(start)} cannot be larger than {nameof(end)}");

      // Determine start position

      var pos_start = blockinfo.Position + Offsets.block_data + sizeof(float) * start * NumberOfPointsPerSpectrum;
      var n_row = end - start + 1;
      sr.Seek(pos_start, SeekOrigin.Begin);
      var buf = new byte[NumberOfPointsPerSpectrum * sizeof(float)];
      Spectra = new float[n_row][];
      for (int i = 0; i < n_row; ++i)
      {

/* Unmerged change from project 'AltaxoCore (net8.0)'
Before:
        sr.ForcedRead(buf, 0, buf.Length);
        Spectra[i] = new float[NumberOfPointsPerSpectrum];
After:
        FileIOExtensions.ReadExactly(sr, buf, 0, buf.Length);
        Spectra[i] = new float[NumberOfPointsPerSpectrum];
*/
        sr.ReadExactly(buf, 0, buf.Length);
        Spectra[i] = new float[NumberOfPointsPerSpectrum];
        Buffer.BlockCopy(buf, 0, Spectra[i], 0, buf.Length);
      }
    }


    /// <summary>
    /// Get information from XLST or YLST blocks
    /// </summary>
    /// <param name="sr">The sr.</param>
    /// <param name="blockinfo">The blockinfo.</param>
    /// <param name="kind">The kind.</param>
    /// <returns></returns>
    private void ParseXYList(Stream sr, BlockInfo blockinfo, ListKind kind)
    {

      // uid, pos, size = self.block_info[name]
      sr.Seek(blockinfo.Position + Offsets.block_data, SeekOrigin.Begin);
      var buf = new byte[8];

/* Unmerged change from project 'AltaxoCore (net8.0)'
Before:
      sr.ForcedRead(buf, 0, buf.Length);
After:
      FileIOExtensions.ReadExactly(sr, buf, 0, buf.Length);
*/
      sr.ReadExactly(buf, 0, buf.Length);

      listinfo[(int)kind].Type = BitConverter.ToInt32(buf, 0);
      listinfo[(int)kind].Unit = BitConverter.ToInt32(buf, 4);
      int size = listinfo[(int)kind].Length;


      if (size == 0)           // Possibly not started
        throw new InvalidOperationException("Size of list not initialized");


      buf = new byte[size * sizeof(float)];


/* Unmerged change from project 'AltaxoCore (net8.0)'
Before:
      sr.ForcedRead(buf, 0, buf.Length);
After:
      FileIOExtensions.ReadExactly(sr, buf, 0, buf.Length);
*/
      sr.ReadExactly(buf, 0, buf.Length);

      var data = new float[size];
      Buffer.BlockCopy(buf, 0, data, 0, buf.Length);
      listinfo[(int)kind].Data = data;
    }

    /// <summary>
    /// Get information from OriginList
    /// </summary>
    /// <param name="sr">The sr.</param>
    /// <param name="blockinfo">The blockinfo.</param>
    /// <returns></returns>
    private void ParseOrginList(Stream sr, BlockInfo blockinfo)
    {
      // First confirm origin list type
      // uid, pos, size = self.block_info["ORGN"]
      // All possible to have x y and z positions!
      XPositions = new double[Count];
      YPositions = new double[Count];
      ZPositions = new double[Count];

      var list_increment = Offsets.origin_increment + sizeof(double) * Capacity;
      var curpos = blockinfo.Position + Offsets.origin_info;

      OriginListHeader = new (bool isY, DataType dataType, UnitType unitType, string annotation, Array array)[DataOriginCount];
      for (int i = 0; i < DataOriginCount; ++i)
      {
        sr.Seek(curpos, SeekOrigin.Begin);
        var buf = new byte[24];

/* Unmerged change from project 'AltaxoCore (net8.0)'
Before:
        sr.ForcedRead(buf, 0, buf.Length);
After:
        FileIOExtensions.ReadExactly(sr, buf, 0, buf.Length);
*/
        sr.ReadExactly(buf, 0, buf.Length);



        var p1 = BitConverter.ToInt32(buf, 0);
        var p2 = BitConverter.ToInt32(buf, 4);
        var s = System.Text.Encoding.UTF8.GetString(buf, 8, 0x10).TrimEnd('\0');
        // First index: is the list x, or y pos?
        var header_isY = (p1 >> 31 & 0b1) == 1;
        // Second: Data type of the row
        var header_dataType = (DataType)(p1 & ~(0b1 << 31));
        // Third: Unit
        var header_unit = (UnitType)(p2);
        // Fourth: annotation
        var header_annotation = s;
        // Last: the actual data
        // array = numpy.empty(self.count)
        //
        // Time appears to be recorded as int64 in 100 nanosecond intervals
        // Possibly using the .NET DateTime epoch
        // Reference does not appear to be  Unix Epoch time  
        // Set time[0] = 0 until timestamp reference can be determined
        // Resulting array will have unit of `FileTime` in seconds
        buf = new byte[Count * sizeof(double)];

/* Unmerged change from project 'AltaxoCore (net8.0)'
Before:
        sr.ForcedRead(buf, 0, buf.Length);
After:
        FileIOExtensions.ReadExactly(sr, buf, 0, buf.Length);
*/
        sr.ReadExactly(buf, 0, buf.Length);

        Array array;
        if (header_dataType == DataType.Time)
        {
          var refDate = new DateTime(1601, 1, 1);
          var dt = new DateTime[Count];
          for (int k = 0, j = 0; j < buf.Length; ++k, j += 8)
            dt[k] = refDate + TimeSpan.FromTicks(BitConverter.ToInt64(buf, j));
          array = dt;
        }
        else
        {
          array = new double[Count];
          Buffer.BlockCopy(buf, 0, array, 0, buf.Length);

        }

        OriginListHeader[i] = (header_isY, header_dataType, header_unit, header_annotation, array);


        // Set self.xpos or self.ypos
        if (header_dataType == DataType.Spatial_X)
        {
          XPositions = (double[])array;
          XPositionUnit = header_unit;
        }
        else if (header_dataType == DataType.Spatial_Y)
        {
          YPositions = (double[])array;
          YPositionUnit = header_unit;
        }
        else if (header_dataType == DataType.Spatial_Z)
        {
          ZPositions = (double[])array;
          ZPositionUnit = header_unit;
        }

        curpos += list_increment;
      }
    }

    /// <summary>
    /// Get information about mapping in StreamLine and StreamLineHR
    /// </summary>
    /// <param name="sr">The sr.</param>
    /// <param name="blockinfo">The blockinfo.</param>
    /// <returns></returns>
    private void ParseWmap(Stream sr, BlockInfo blockinfo)
    {
      sr.Seek(blockinfo.Position + Offsets.wmap_origin, SeekOrigin.Begin);
      var buf = new byte[32];

/* Unmerged change from project 'AltaxoCore (net8.0)'
Before:
      sr.ForcedRead(buf, 0, buf.Length);
After:
      FileIOExtensions.ReadExactly(sr, buf, 0, buf.Length);
*/
      sr.ReadExactly(buf, 0, buf.Length);

      var x_start = BitConverter.ToSingle(buf, 0);
      if (!IsClose(x_start, XPositions[0], 0, 1e-4))
        throw new InvalidDataException("WMAP Xpos is not same as in ORGN!");

      var y_start = BitConverter.ToSingle(buf, 4);
      if (!IsClose(y_start, YPositions[0], 0, 1e-4))
        throw new InvalidDataException("WMAP Ypos is not same as in ORGN!");

      var unknown1 = BitConverter.ToSingle(buf, 8);
      var x_pad = BitConverter.ToSingle(buf, 12);
      var y_pad = BitConverter.ToSingle(buf, 16);
      var unknown2 = BitConverter.ToSingle(buf, 20);
      var spectra_w = BitConverter.ToInt32(buf, 24);
      var spectra_h = BitConverter.ToInt32(buf, 28);

      // Determine if the xy-grid spacing is same as in x_pad and y_pad
      if ((XPositions?.Length ?? 0) > 1 && (YPositions?.Length ?? 0) > 1)
      {
        var xdist = XPositions.Select(x => Math.Abs(x - XPositions[0])).Where(x => x != 0);
        var ydist = XPositions.Select(y => Math.Abs(y - YPositions[0])).Where(y => y != 0);

        // Get minimal non-zero padding in the grid
        double x_pad_grid = 0, y_pad_grid = 0;
        try
        {
          x_pad_grid = xdist.Min();
        }
        catch (InvalidOperationException)
        {

        }
        try
        {
          y_pad_grid = ydist.Min();
        }
        catch (InvalidOperationException)
        {

        }
      }


      MapShape = (spectra_w, spectra_h);
      _map_info = new Dictionary<string, object>()
      {
        ["x_start"] = x_start,
        ["y_start"] = y_start,
        ["x_pad"] = x_pad,
        ["y_pad"] = y_pad,
        ["x_span"] = spectra_w * x_pad,
        ["y_span"] = spectra_h * y_pad,
        ["x_unit"] = XPositionUnit,
        ["y_unit"] = YPositionUnit
      };
    }

    /// <summary>
    /// Extract the white-light JPEG image. The size of while-light image is coded in its EXIF.
    /// </summary>
    /// <param name="sr">The sr.</param>
    /// <param name="blockinfo">The blockinfo.</param>
    /// <returns></returns>
    private void ParseImg(Stream sr, BlockInfo blockinfo)
    {
      // Read the bytes. `self.img` is a wrapped IO object mimicking a file
      sr.Seek(blockinfo.Position + Offsets.jpeg_header, SeekOrigin.Begin);
      var buf = new byte[blockinfo.Size - Offsets.jpeg_header];

/* Unmerged change from project 'AltaxoCore (net8.0)'
Before:
      sr.ForcedRead(buf, 0, buf.Length);
      Images.Add(buf);
After:
      FileIOExtensions.ReadExactly(sr, buf, 0, buf.Length);
      Images.Add(buf);
*/
      sr.ReadExactly(buf, 0, buf.Length);
      Images.Add(buf);

      /*

      // https://docs.microsoft.com/en-us/dotnet/desktop/winforms/advanced/how-to-read-image-metadata

      using var ms = new MemoryStream(buf);
      var img = new System.Drawing.Bitmap(ms);


      var dict = new Dictionary<int, System.Drawing.Imaging.PropertyItem>();
      foreach (var item in img.PropertyItems)
        dict[item.Id] = item;

      img_bytes = self.file_obj.read(size - Offsets.jpeg_header)
        self.img = io.BytesIO(img_bytes)
        # Handle image dimension if PIL is present
        if PIL is not None:
            pil_img = Image.open(self.img)
            # Weird missing header keys when Pillow >= 8.2.0.
            # see https://pillow.readthedocs.io/en/stable/releasenotes/8.2.0.html#image-getexif-exif-and-gps-ifd
            # Use fall-back _getexif method instead
            exif_header = dict(pil_img._getexif())
            try:
                # Get the width and height of image
                w_ = exif_header[ExifTags.FocalPlaneXResolution]
                h_ = exif_header[ExifTags.FocalPlaneYResolution]
                x_org_, y_org_ = exif_header[ExifTags.FocalPlaneXYOrigins]

                def rational2float(v):
                    """ Pillow<7.2.0 returns tuple, Pillow>=7.2.0 returns IFDRational """
                    if not isinstance(v, IFDRational):
                        return v[0] / v[1]
                    return float(v)

                w_, h_ = rational2float(w_), rational2float(h_)
                x_org_, y_org_ = rational2float(x_org_), rational2float(y_org_)

                # The dimensions (width, height)
# with unit `img_dimension_unit`
      self.img_dimensions = numpy.array([w_,
                                                   h_])
                # Origin of image is at upper right corner
                self.img_origins = numpy.array([x_org_,
                                                y_org_])
                # Default is microns (5)
                self.img_dimension_unit = UnitType(
                    exif_header[ExifTags.FocalPlaneResolutionUnit])
                # Give the box for cropping
                # Following the PIL manual
                # (left, upper, right, lower)
                self.img_cropbox = self.__calc_crop_box()

            except KeyError:
                if self.debug:
                    print(("Some keys in white light image header"
                           " cannot be read!"),
                          file = stderr)

      */
    }


    /// <summary>
    /// Convert wavenumber (cm^-1) to nm
    /// </summary>
    /// <param name="wn">The wavenumber.</param>
    /// <returns>Wavelength in nm</returns>
    private static double ConvertWavenumberToWavelength_nm(double wn) => 1 / (wn * 1e2) / 1e-9;

    private static bool IsClose(double x, double y, double atol = 0, double rtol = 0)
    {
      var tol = Math.Max(atol, Math.Max(Math.Abs(x), Math.Abs(y)) * rtol);
      return Math.Abs(x - y) <= tol;
    }
  }
}

