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
  public class NicoletSPAReader
  {
    public double XFirst { get; protected set; }
    public double XLast { get; protected set; }
    public double XIncrement { get; protected set; }

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

    public string Comment = string.Empty;

    public double[] X { get; protected set; }
    public double[] Y { get; protected set; }


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
