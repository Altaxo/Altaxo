#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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
using System.Text;

namespace Altaxo.Serialization.Omnic
{
  /// <summary>
  /// Reads data from a Thermo Nicolet OMNIC SPG formatted stream and exposes the
  /// x axis together with spectra and basic metadata.
  /// </summary>
  public class OmnicSPGReader
  {
    /// <summary>
    /// The x values common to all spectra.
    /// </summary>
    public double[] X { get; protected set; }

    /// <summary>
    /// The spectra intensities. Dimension is [spectrumIndex][pointIndex].
    /// </summary>
    public double[][] Y { get; protected set; }

    /// <summary>
    /// The number of spectra in the group.
    /// </summary>
    public int NumberOfSpectra { get; protected set; }

    /// <summary>
    /// The number of points per spectrum.
    /// </summary>
    public int NumberOfPoints { get; protected set; }

    /// <summary>The label of the x-axis.</summary>
    public string? XLabel { get; protected set; }

    /// <summary>The unit of the x-axis.</summary>
    public string? XUnit { get; protected set; }

    /// <summary>The label of the y-axis.</summary>
    public string? YLabel { get; protected set; }

    /// <summary>The unit of the y-axis.</summary>
    public string? YUnit { get; protected set; }

    /// <summary>
    /// Titles (names) of the individual spectra.
    /// </summary>
    public string[] SpectrumTitles { get; protected set; }

    /// <summary>
    /// Acquisition dates of the individual spectra (UTC).
    /// </summary>
    public DateTime[] AcquisitionDatesUtc { get; protected set; }

    /// <summary>
    /// Original group name stored in the file header.
    /// </summary>
    public string GroupName { get; protected set; } = string.Empty;

    /// <summary>
    /// Messages about any errors during the import of the file.
    /// </summary>
    public string? ErrorMessages { get; protected set; }

    /// <summary>
    /// Initializes a new instance of <see cref="OmnicSPGReader"/> and reads the data from the provided <paramref name="stream"/>.
    /// </summary>
    /// <param name="stream">
    /// A seekable <see cref="System.IO.Stream"/> that contains data in the Thermo Nicolet OMNIC SPG file format.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="stream"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="stream"/> is not seekable.</exception>
    /// <remarks>
    /// The constructor reads the OMNIC key table (starting at offset 304) to locate per-spectrum headers (key 0x02),
    /// intensity blocks (key 0x03) and title/date blocks (key 0x6B). It assumes that all spectra share a common x-axis.
    /// </remarks>
    public OmnicSPGReader(Stream stream)
    {
      if (stream is null)
        throw new ArgumentNullException(nameof(stream));
      if (!stream.CanSeek)
        throw new ArgumentException($"{nameof(stream)} must be seekable!");

      // Offsets and constants derived from the Omnic SPG structure.
      const int Pos_GroupName = 30; // 0x1E
      const int Len_GroupName = 256;
      const int Pos_NumberOfKeyLines = 294; // 0x126
      const int Pos_FirstKeyLine = 304; // 0x130
      const int KeyLineSize = 16;

      try
      {
        GroupName = ReadBText(stream, Pos_GroupName, Len_GroupName);

        stream.Seek(Pos_NumberOfKeyLines, SeekOrigin.Begin);
        var nlines = ReadUInt16(stream);
        if (nlines < 1)
          throw new InvalidDataException("SPG file contains no key table lines.");

        var keyValues = new byte[nlines];
        for (int i = 0; i < nlines; i++)
        {
          stream.Seek(Pos_FirstKeyLine + i * KeyLineSize, SeekOrigin.Begin);
          keyValues[i] = ReadByte(stream);
        }

        var pos02 = new List<int>();
        var pos03 = new List<int>();
        var pos6B = new List<int>();

        for (int i = 0; i < nlines; i++)
        {
          if (keyValues[i] == 2)
            pos02.Add(Pos_FirstKeyLine + i * KeyLineSize);
          else if (keyValues[i] == 3)
            pos03.Add(Pos_FirstKeyLine + i * KeyLineSize);
          else if (keyValues[i] == 107)
            pos6B.Add(Pos_FirstKeyLine + i * KeyLineSize);
        }

        if (pos02.Count == 0)
          throw new InvalidDataException("SPG file format not recognized - information markers (key 0x02) not found.");

        NumberOfSpectra = pos02.Count;
        if (pos03.Count < NumberOfSpectra)
          throw new InvalidDataException("SPG file key table incomplete - fewer intensity markers (key 0x03) than spectra.");
        if (pos6B.Count < NumberOfSpectra)
          throw new InvalidDataException("SPG file key table incomplete - fewer title/date markers (key 0x6B) than spectra.");

        // Read per-spectrum headers.
        var nx = new int[NumberOfSpectra];
        var firstx = new double[NumberOfSpectra];
        var lastx = new double[NumberOfSpectra];
        var xUnit = new string?[NumberOfSpectra];
        var xTitle = new string?[NumberOfSpectra];
        var yUnit = new string?[NumberOfSpectra];
        var yTitle = new string?[NumberOfSpectra];

        for (int i = 0; i < NumberOfSpectra; i++)
        {
          stream.Seek(pos02[i] + 2, SeekOrigin.Begin);
          var posHeader = checked((long)ReadUInt32(stream));
          var info = ReadHeader(stream, posHeader, isFirstSpectrum: i == 0);

          nx[i] = info.Nx;
          firstx[i] = info.FirstX;
          lastx[i] = info.LastX;
          xUnit[i] = info.XUnit;
          xTitle[i] = info.XTitle;
          yUnit[i] = info.YUnit;
          yTitle[i] = info.YTitle;
        }

        NumberOfPoints = nx[0];
        for (int i = 1; i < NumberOfSpectra; i++)
        {
          if (nx[i] != NumberOfPoints)
            throw new InvalidDataException("Inconsistent data set - number of x points per spectrum should be identical.");
          if (firstx[i] != firstx[0])
            throw new InvalidDataException("Inconsistent data set - x axis should start at same value.");
          if (lastx[i] != lastx[0])
            throw new InvalidDataException("Inconsistent data set - x axis should end at same value.");
          if (!string.Equals(xUnit[i], xUnit[0], StringComparison.Ordinal))
            throw new InvalidDataException("Inconsistent data set - x axis units should be identical.");
          if (!string.Equals(yUnit[i], yUnit[0], StringComparison.Ordinal))
            throw new InvalidDataException("Inconsistent data set - data units should be identical.");
        }

        XUnit = xUnit[0];
        XLabel = xTitle[0];
        YUnit = yUnit[0];
        YLabel = yTitle[0];

        // Build x-axis.
        X = new double[NumberOfPoints];
        if (NumberOfPoints == 1)
        {
          X[0] = firstx[0];
        }
        else
        {
          var min = firstx[0];
          var max = lastx[0];
          for (int i = 0; i < NumberOfPoints; i++)
          {
            var rel = i / (NumberOfPoints - 1d);
            X[i] = min * (1 - rel) + max * rel;
          }
        }

        // Read intensities.
        var y = new double[NumberOfSpectra][];
        for (int i = 0; i < NumberOfSpectra; i++)
        {
          var intensities = ReadIntensities(stream, pos03[i]);
          if (intensities.Length != NumberOfPoints)
            throw new InvalidDataException("Intensity block size does not match number of points.");

          var spectrum = new double[NumberOfPoints];
          for (int j = 0; j < NumberOfPoints; j++)
            spectrum[j] = intensities[j];

          y[i] = spectrum;
        }
        Y = y;

        // Read titles and acquisition dates.
        SpectrumTitles = new string[NumberOfSpectra];
        AcquisitionDatesUtc = new DateTime[NumberOfSpectra];
        for (int i = 0; i < NumberOfSpectra; i++)
        {
          stream.Seek(pos6B[i] + 2, SeekOrigin.Begin);
          var spaNamePos = checked((long)ReadUInt32(stream));
          SpectrumTitles[i] = ReadBText(stream, spaNamePos, 256);

          stream.Seek(spaNamePos + 256, SeekOrigin.Begin);
          var seconds = ReadUInt32(stream);
          AcquisitionDatesUtc[i] = OmnicEpochUtc.AddSeconds(seconds);
        }
      }
      catch (Exception ex)
      {
        ErrorMessages = ex.Message;
        X = Array.Empty<double>();
        Y = Array.Empty<double[]>();
        SpectrumTitles = Array.Empty<string>();
        AcquisitionDatesUtc = Array.Empty<DateTime>();
        NumberOfSpectra = 0;
        NumberOfPoints = 0;
      }
    }

    private static readonly DateTime OmnicEpochUtc = new DateTime(1899, 12, 31, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Reads a single byte from the stream.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <returns>The byte value.</returns>
    /// <exception cref="EndOfStreamException">Thrown when the stream ends unexpectedly.</exception>
    private static byte ReadByte(Stream stream)
    {
      var b = stream.ReadByte();
      if (b < 0)
        throw new EndOfStreamException();
      return (byte)b;
    }

    /// <summary>
    /// Reads an unsigned 16-bit integer (little-endian) from the stream.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <returns>The value read from the stream.</returns>
    /// <exception cref="EndOfStreamException">Thrown when the stream ends unexpectedly.</exception>
    private static ushort ReadUInt16(Stream stream)
    {
      Span<byte> buffer = stackalloc byte[2];
      stream.ReadExactly(buffer);
      return BitConverter.ToUInt16(buffer);
    }

    /// <summary>
    /// Reads an unsigned 32-bit integer (little-endian) from the stream.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <returns>The value read from the stream.</returns>
    /// <exception cref="EndOfStreamException">Thrown when the stream ends unexpectedly.</exception>
    private static uint ReadUInt32(Stream stream)
    {
      Span<byte> buffer = stackalloc byte[4];
      stream.ReadExactly(buffer);
      return BitConverter.ToUInt32(buffer);
    }

    /// <summary>
    /// Reads a 32-bit floating point value (little-endian) from the stream.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <returns>The value read from the stream.</returns>
    /// <exception cref="EndOfStreamException">Thrown when the stream ends unexpectedly.</exception>
    private static float ReadSingle(Stream stream)
    {
      Span<byte> buffer = stackalloc byte[4];
      stream.ReadExactly(buffer);
      return BitConverter.ToSingle(buffer);
    }

    /// <summary>
    /// Reads a text block at the specified <paramref name="pos"/>.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="pos">The absolute byte position in the stream where the text begins.</param>
    /// <param name="size">
    /// The number of bytes to read. If <c>null</c>, bytes are read until a null terminator (0x00) is found.
    /// </param>
    /// <returns>The decoded text.</returns>
    /// <remarks>
    /// OMNIC stores text in fixed-size blocks filled with 0x00. For compatibility with the Python reader logic,
    /// sequences of 0x00 are treated as separators and replaced by line breaks. The returned text is trimmed.
    /// </remarks>
    private static string ReadBText(Stream stream, long pos, int? size)
    {
      stream.Seek(pos, SeekOrigin.Begin);
      byte[] bytes;

      if (size is null)
      {
        using var ms = new MemoryStream();
        while (true)
        {
          var b = stream.ReadByte();
          if (b < 0)
            break;
          if (b == 0)
            break;
          ms.WriteByte((byte)b);
        }
        bytes = ms.ToArray();
      }
      else
      {
        bytes = new byte[size.Value];
        stream.ReadExactly(bytes, 0, bytes.Length);
      }

      // OMNIC uses sequences of 0x00 as separators. Replace them with newlines and trim.
      var text = Encoding.UTF8.GetString(bytes);
      text = text.Replace("\0", "\n", StringComparison.Ordinal);
      return text.Trim('\n', '\r', ' ', '\t');
    }

    /// <summary>
    /// Reads the float32 intensity vector referenced by a key-table entry (key 0x03).
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="keyLinePos">Absolute position of the key-table line for key 0x03.</param>
    /// <returns>Array of intensity values.</returns>
    /// <exception cref="InvalidDataException">Thrown when the intensity size is invalid.</exception>
    private static float[] ReadIntensities(Stream stream, int keyLinePos)
    {
      stream.Seek(keyLinePos + 2, SeekOrigin.Begin);
      var intensityPos = checked((long)ReadUInt32(stream));
      stream.Seek(keyLinePos + 6, SeekOrigin.Begin);
      var intensitySize = ReadUInt32(stream);
      var n = checked((int)(intensitySize / 4));
      if (n < 0)
        throw new InvalidDataException("Invalid intensity size.");

      stream.Seek(intensityPos, SeekOrigin.Begin);
      var result = new float[n];
      Span<byte> buf = stackalloc byte[4];
      for (int i = 0; i < n; i++)
      {
        stream.ReadExactly(buf);
        result[i] = BitConverter.ToSingle(buf);
      }
      return result;
    }

    /// <summary>
    /// Reads the spectrum header referenced by a key-table entry (key 0x02).
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="headerPos">Absolute position of the header block.</param>
    /// <param name="isFirstSpectrum">
    /// Indicates whether this is the first spectrum being read (used to keep behavior consistent with the Python reader,
    /// which only logs unknown unit keys for the first spectrum).
    /// </param>
    /// <returns>A <see cref="HeaderInfo"/> with nx, x-range and basic unit/title mappings.</returns>
    private static HeaderInfo ReadHeader(Stream stream, long headerPos, bool isFirstSpectrum)
    {
      // Based on the python reader's understanding of the OMNIC header.
      // Offsets are relative to headerPos.
      stream.Seek(headerPos + 4, SeekOrigin.Begin);
      var nx = checked((int)ReadUInt32(stream));

      stream.Seek(headerPos + 8, SeekOrigin.Begin);
      var xKey = ReadByte(stream);
      var (xUnit, xTitle) = xKey switch
      {
        1 => ("cm^-1", "wavenumbers"),
        2 => (null, "data points"),
        3 => ("nm", "wavelengths"),
        4 => ("um", "wavelengths"),
        32 => ("cm^-1", "raman shift"),
        _ => (null, "xaxis")
      };

      stream.Seek(headerPos + 12, SeekOrigin.Begin);
      var yKey = ReadByte(stream);
      var (yUnit, yTitle) = yKey switch
      {
        17 => ("absorbance", "absorbance"),
        16 => ("percent", "transmittance"),
        11 => ("percent", "reflectance"),
        12 => (null, "log(1/R)"),
        15 => (null, "single beam"),
        20 => ("Kubelka_Munk", "Kubelka-Munk"),
        21 => (null, "reflectance"),
        22 => ("V", "detector signal"),
        26 => (null, "photoacoustic"),
        31 => (null, "Raman intensity"),
        _ => (null, "intensity")
      };

      if (yKey is < 11 or > 31)
      {
        if (isFirstSpectrum)
        {
          // keep title "intensity" but no exception
        }
      }

      stream.Seek(headerPos + 16, SeekOrigin.Begin);
      var firstx = ReadSingle(stream);
      stream.Seek(headerPos + 20, SeekOrigin.Begin);
      var lastx = ReadSingle(stream);

      return new HeaderInfo(nx, firstx, lastx, xUnit, xTitle, yUnit, yTitle);
    }

    /// <summary>
    /// Holds the subset of header information needed for SPG import.
    /// </summary>
    private readonly record struct HeaderInfo(int Nx, double FirstX, double LastX, string? XUnit, string XTitle, string? YUnit, string YTitle);
  }
}
