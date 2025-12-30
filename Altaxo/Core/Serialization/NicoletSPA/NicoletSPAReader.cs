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
using System.IO;

namespace Altaxo.Serialization.NicoletSPA
{
  /// <summary>
  /// Reads data from a Nicolet SPA formatted stream and exposes the
  /// x and y arrays together with metadata.
  /// </summary>
  public class NicoletSPAReader
  {
    /// <summary>
    /// The first x value (minimum wavenumber) read from the file.
    /// </summary>
    public double XFirst { get; protected set; }

    /// <summary>
    /// The last x value (maximum wavenumber) read from the file.
    /// </summary>
    public double XLast { get; protected set; }

    /// <summary>
    /// The increment between successive x values.
    /// </summary>
    public double XIncrement { get; protected set; }

    /// <summary>
    /// The number of points in the data arrays.
    /// </summary>
    public int NumberOfPoints { get; protected set; }

    /// <summary>The label of the x-axis.</summary>
    public string? XLabel { get; protected set; } = null;

    /// <summary>The label of the y-axis.</summary>
    public string? YLabel { get; protected set; } = null;

    /// <summary>The unit of the x-axis.</summary>
    public string? XUnit { get; protected set; } = null;

    /// <summary>The unit of the y-axis.</summary>
    public string? YUnit { get; protected set; } = null;

    /// <summary>
    /// Messages about any errors during the import of the file.
    /// </summary>
    public string? ErrorMessages { get; protected set; } = null;

    /// <summary>
    /// Comment text extracted from the file header. This is usually the
    /// comment block present in the SPA header and is trimmed of any
    /// trailing null characters.
    /// </summary>
    public string Comment = string.Empty;

    /// <summary>
    /// The x values read from the file as an array of doubles.
    /// </summary>
    public double[] X { get; protected set; }

    /// <summary>
    /// The y values read from the file as an array of doubles.
    /// </summary>
    public double[] Y { get; protected set; }


    /// <summary>
    /// Initializes a new instance of <see cref="NicoletSPAReader"/> and
    /// reads the data from the provided <paramref name="stream"/>.
    /// </summary>
    /// <param name="stream">A seekable <see cref="System.IO.Stream"/>
    /// that contains data in the Nicolet SPA file format.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="stream"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="stream"/> is not seekable.</exception>
    /// <remarks>
    /// The constructor reads fixed offsets from the SPA file header to
    /// determine comment text, number of points, minimum/maximum
    /// wavenumbers and the location of the binary data block. Existing
    /// inline comments in the implementation document the exact offsets
    /// and markers used.
    /// </remarks>
    public NicoletSPAReader(Stream stream)
    {
      const int Pos_BeginComment = 0x1E; // 30 dez, position where the comment starts
      const int Pos_EndComment = 0x100; // 255 dez, position where the comment ends (exclusive)
      const int Pos_StartSearchMarkerBeforeOffset = 0x120; // 288 dez, position at which to search for the start marker 0x0003, after which the offset to the data can be found
      const int StartMarkerForOffset = 0x0003;
      const int Pos_NumberOfPoints = 0x234; // 564 dez, position where to find the number of data points
      const int Pos_MinMax = 0x240; // 576 dez, position where to find minimum and maximum of the x-axis

      if (stream is null)
        throw new ArgumentNullException(nameof(stream));
      if (!stream.CanSeek)
        throw new ArgumentException($"{nameof(stream)} must be seekable!");

      stream.Seek(Pos_BeginComment, SeekOrigin.Begin); // Begin of the comment section
      var buffer = new byte[Pos_EndComment - Pos_BeginComment];

      stream.ReadExactly(buffer, 0, buffer.Length);
      var comment = System.Text.Encoding.UTF8.GetString(buffer);
      Comment = comment.TrimEnd('\0');
      stream.Seek(Pos_NumberOfPoints, SeekOrigin.Begin);

      stream.ReadExactly(buffer, 0, sizeof(Int32));
      var numberOfPoints = BitConverter.ToInt32(buffer, 0);

      // get minimum and maximum wavenumbers
      stream.Seek(Pos_MinMax, SeekOrigin.Begin);

      stream.ReadExactly(buffer, 0, 2 * sizeof(Single));
      var min = BitConverter.ToSingle(buffer, 0);
      var max = BitConverter.ToSingle(buffer, sizeof(Single));

      XFirst = min;
      XLast = max;
      XIncrement = (max - min) / (numberOfPoints - 1d);
      NumberOfPoints = numberOfPoints;

      // locate the offset to the data
      // search for the start marker 0x0003, after which the offset can be found
      stream.Seek(Pos_StartSearchMarkerBeforeOffset, SeekOrigin.Begin);
      do
      {
        stream.ReadExactly(buffer, 0, sizeof(Int16));
      } while (StartMarkerForOffset != BitConverter.ToInt16(buffer, 0));

      // now read the offset
      stream.ReadExactly(buffer, 0, sizeof(Int16));
      var offset = BitConverter.ToInt16(buffer, 0);

      var ybuffer = new byte[numberOfPoints * sizeof(float)];
      stream.Seek(offset, SeekOrigin.Begin);

      stream.ReadExactly(ybuffer, 0, ybuffer.Length);

      var x = new double[numberOfPoints];
      var y = new double[numberOfPoints];

      for (int i = 0; i < numberOfPoints; i++)
      {
        var rel = i / (numberOfPoints - 1d);
        x[i] = min * (1 - rel) + max * rel;
        y[i] = BitConverter.ToSingle(ybuffer, i * sizeof(float));
      }

      X = x;
      Y = y;
    }
  }
}
