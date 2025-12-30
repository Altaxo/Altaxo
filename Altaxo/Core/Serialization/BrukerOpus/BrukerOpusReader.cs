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
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Altaxo.Serialization.BrukerOpus
{
  /// <summary>
  /// File reader for Bruker Opus spectral files.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>[1] Opus reader in Python: <see href="https://github.com/qedsoftware/brukeropusreader"/></para>
  /// </remarks>
  public class BrukerOpusReader
  {
    private const int SizeOfMetablock = 12;
    private const int PositionOfFirstMetablock = 24;

    /// <summary>
    /// Gets the error messages accumulated while reading the file, if any.
    /// </summary>
    public string? ErrorMessages { get; protected set; }

    /// <summary>
    /// Gets the x values of the spectrum (typically the abscissa values).
    /// </summary>
    public double[]? XValues { get; protected set; }

    /// <summary>
    /// Gets the y values of the spectrum (typically the measured intensities).
    /// </summary>
    public double[]? YValues { get; protected set; }

    private Dictionary<string, object> _parsedData = new();

    /// <summary>
    /// Gets the parsed metadata and data blocks indexed by their block names.
    /// </summary>
    public IReadOnlyDictionary<string, object> ParsedData => _parsedData;



    /// <summary>
    /// Creates a new <see cref="BrukerOpusReader"/> and parses the provided stream as a Bruker Opus file.
    /// </summary>
    /// <param name="stream">The stream containing the Bruker Opus file data. The stream must support seeking.</param>
    /// <exception cref="InvalidDataException">Thrown when metadata indicates a different number of points than the data array length.</exception>
    public BrukerOpusReader(Stream stream)
    {
      var streamLength = stream.Length;
      var metaBlock = new byte[SizeOfMetablock];
      var metablocks = new List<Metablock>();
      for (int positionMetaBlock = PositionOfFirstMetablock; positionMetaBlock + SizeOfMetablock <= streamLength; positionMetaBlock += SizeOfMetablock)
      {
        stream.Seek(positionMetaBlock, SeekOrigin.Begin);
        stream.ReadExactly(metaBlock, 0, metaBlock.Length);
        var dataType = metaBlock[0];
        var channelType = metaBlock[1];
        var textType = metaBlock[2];
        var chunkSize = BitConverter.ToInt32(metaBlock, 4);
        var offset = BitConverter.ToInt32(metaBlock, 8);
        if (offset <= 0)
        {
          break;
        }

        metablocks.Add(new Metablock(dataType, channelType, textType, chunkSize, offset));

        var nextPosition = offset + 4 * chunkSize;
        if (nextPosition >= streamLength)
        {
          break;
        }
      }


      // now parse the data that the metablocks are referring to
      foreach (var metablock in metablocks)
      {
        var (name, data) = metablock.ParseBlock(stream);
        if (!string.IsNullOrEmpty(name) && data is not null)
        {
          _parsedData[name] = data;
        }
      }

      // try to extract the spectral data from the parsed data
      if (_parsedData.TryGetValue("AB", out var arrobj) && arrobj is double[] arr)
      {
        YValues = arr;
      }
      if (_parsedData.TryGetValue("AB Data Parameter", out var dictobj) && dictobj is Dictionary<string, object> parameterDict)
      {
        var numberOfPoints = (int)parameterDict["NPT"];
        if (numberOfPoints != (YValues?.Length ?? 0))
          throw new InvalidDataException($"The number of points in the meta data is {numberOfPoints}, but the length of the array is {YValues?.Length}");

        var xstart = (double)parameterDict["FXV"];
        var xend = (double)parameterDict["LXV"];
        XValues = new double[numberOfPoints];
        for (int i = 0; i < numberOfPoints; i++)
        {
          var r = i / (numberOfPoints - 1d);
          XValues[i] = (1 - r) * xstart + r * xend;
        }
      }
    }

    /// <summary>
    /// Represents a single metablock described in the file's metablock table.
    /// This internal helper reads and interprets the block at the given offset.
    /// </summary>
    private class Metablock
    {
      private int _dataType, _channelType, _textType, _chunkSize, _offset;
      private static readonly Encoding _latin1Encoding = Encoding.GetEncoding("ISO-8859-1");


      /// <summary>
      /// Initializes a new instance of the <see cref="Metablock"/> class with the given header values.
      /// </summary>
      /// <param name="dataType">The data type identifier for the metablock.</param>
      /// <param name="channelType">The channel type identifier for the metablock.</param>
      /// <param name="textType">The text type identifier for the metablock.</param>
      /// <param name="chunkSize">The number of 4-byte words that make up the block payload.</param>
      /// <param name="offset">The offset (in bytes) from the start of the file where the block payload begins.</param>
      public Metablock(int dataType, int channelType, int textType, int chunkSize, int offset)
      {
        _dataType = dataType;
        _channelType = channelType;
        _textType = textType;
        _chunkSize = chunkSize;
        _offset = offset;
      }

      /// <summary>
      /// Parses the metablock payload and returns a tuple with the parsed block name and the associated data.
      /// If the block type is not recognized, returns (null, null).
      /// </summary>
      /// <param name="stream">The stream to read the block payload from. The stream must support seeking.</param>
      /// <returns>A tuple containing the block name and the parsed data object, or (null, null) if the block is unrecognized.</returns>
      public (string Name, object Data) ParseBlock(Stream stream)
      {
        switch (_dataType)
        {
          case 0:
            {
              switch (_textType)
              {
                case 8:
                  return ("Info Block", ParseParameter(stream));
                case 104:
                  return ("History", ParseText(stream));
                case 152:
                  return ("Curve Fit", ParseText(stream));
                case 168:
                  return ("Signature", ParseText(stream));
                case 240:
                  return ("Integration Method", ParseText(stream));
              }
            }
            break;
          case 7:
            {
              var name = _channelType switch
              {
                4 => "ScSm",
                8 => "IgSm",
                12 => "PhSm",
                56 => "PwSm",
                _ => null,
              };
              if (name is not null)
                return (name, ParseSeries(stream));
            }
            break;
          case 11:
            {
              var name = _channelType switch
              {
                4 => "ScRf",
                8 => "IgRf",
                12 => "PhRf",
                56 => "PwRf",
                _ => null,
              };
              if (name is not null)
                return (name, ParseSeries(stream));
            }
            break;
          case 15:
            return ("AB", ParseSeries(stream));
          case 23:
            {
              var name = _channelType switch
              {
                4 => "ScSm Data Parameter",
                8 => "IgSm Data Parameter",
                12 => "PhSm Data Parameter",
                56 => "PwSm Data Parameter",
                _ => null,
              };
              if (name is not null)
                return (name, ParseParameter(stream));
            }
            break;
          case 27:
            {
              var name = _channelType switch
              {
                4 => "ScRf Data Parameter",
                8 => "IgRf Data Parameter",
                12 => "PhRf Data Parameter",
                56 => "PwRf Data Parameter",
                _ => null,
              };
              if (name is not null)
                return (name, ParseParameter(stream));
            }
            break;
          default:
            {
              var name = _dataType switch
              {
                31 => "AB Data Parameter",
                32 => "Instrument",
                40 => "Instrument (Rf)",
                48 => "Acquisition",
                56 => "Acquisition (Rf)",
                64 => "Fourier Transformation",
                72 => "Fourier Transformation (Rf)",
                96 => "Optik",
                104 => "Optik (Rf)",
                160 => "Sample",
                _ => null,
              };
              if (name is not null)
                return (name, ParseParameter(stream));
            }
            break;
        }
        return (null, null);
      }

      /// <summary>
      /// Reads the raw block payload from the stream and returns it as a byte array.
      /// </summary>
      /// <param name="stream">The stream to read from. The stream must support seeking.</param>
      /// <returns>The payload of the metablock as a byte array.</returns>
      public byte[] ReadChunk(Stream stream)
      {
        var result = new byte[4 * _chunkSize];
        stream.Seek(_offset, SeekOrigin.Begin);
        stream.ReadExactly(result, 0, result.Length);
        return result;
      }

      /// <summary>
      /// Parses a parameter block payload into a dictionary of parameter names and their values.
      /// Values can be int, double, string or a byte array depending on the parameter's type.
      /// </summary>
      /// <param name="stream">The stream containing the parameter block.</param>
      /// <returns>A dictionary mapping parameter names (3-character codes) to their values.</returns>
      public Dictionary<string, object> ParseParameter(Stream stream)
      {
        var parameterDict = new Dictionary<string, object>();
        var buffer = ReadChunk(stream);
        int parameterSize = 0;
        for (int cursor = 0; ; cursor += 8 + 2 * parameterSize)
        {
          var parameterName = System.Text.UTF8Encoding.UTF8.GetString(buffer, cursor, 3);
          if (parameterName == "END")
          {
            break;
          }
          var typeIndex = BitConverter.ToInt16(buffer, cursor + 4);
          parameterSize = BitConverter.ToInt16(buffer, cursor + 6);

          switch (typeIndex)
          {
            case 0: // int
              parameterDict[parameterName] = BitConverter.ToInt32(buffer, cursor + 8);
              break;
            case 1: // double
              parameterDict[parameterName] = BitConverter.ToDouble(buffer, cursor + 8);
              break;
            case 2:
            case 3:
            case 4: // strings (null terminated)
              var len = parameterSize * 2;
              var idx = Array.IndexOf<byte>(buffer, 0, cursor + 8) - cursor - 8;
              if (idx >= 0)
                len = Math.Min(len, idx);
              parameterDict[parameterName] = _latin1Encoding.GetString(buffer, cursor + 8, len).TrimEnd('\0');
              break;
            default:
              var value = new byte[parameterSize];
              Array.Copy(buffer, cursor + 8, value, 0, parameterSize);
              parameterDict[parameterName] = value;
              break;
          }
        }
        return parameterDict;
      }

      /// <summary>
      /// Parses a text block payload and returns the decoded string.
      /// </summary>
      /// <param name="stream">The stream containing the text block.</param>
      /// <returns>The decoded text with trailing nulls trimmed.</returns>
      public string ParseText(Stream stream)
      {
        var buffer = ReadChunk(stream);
        var str = _latin1Encoding.GetString(buffer, 0, buffer.Length);
        return str.Trim('\0');
      }

      /// <summary>
      /// Parses a numeric series from the block payload. The series is stored as 4-byte floats and
      /// is converted to an array of doubles on return.
      /// </summary>
      /// <param name="stream">The stream containing the series block.</param>
      /// <returns>An array of doubles representing the parsed series values.</returns>
      public double[] ParseSeries(Stream stream)
      {
        var buffer = ReadChunk(stream);
        var arr = new double[buffer.Length / sizeof(float)];
        for (int i = 0; i < arr.Length; ++i)
        {
          arr[i] = BitConverter.ToSingle(buffer, i * sizeof(float));
        }
        return arr;
      }
    }
  }
}
