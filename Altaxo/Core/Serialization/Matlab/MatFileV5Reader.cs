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
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Altaxo.Serialization.Matlab
{
  /// <summary>
  /// Reader for MATLAB MAT-file version 5 streams.
  /// </summary>
  public class MatFileV5Reader
  {
    /// <summary>
    /// Underlying stream positioned at the first data element (right after the 128-byte MAT v5 header).
    /// </summary>
    private readonly Stream _stream;

    /// <summary>
    /// Whether the file endianness differs from the current platform and values must be byte-swapped.
    /// </summary>
    private readonly bool _swap;

    // Method layout:
    // 1) Public entry point
    // 2) Top-level data element traversal
    // 3) miMATRIX parsing (name + payload)
    // 4) Payload decoding (char, cell, struct/object, numeric)
    // 5) Low-level tag/data reading and padding helpers
    // 6) Enums

    /// <summary>
    /// Creates a reader instance.
    /// </summary>
    /// <param name="stream">Underlying stream positioned at the first data element (right after the 128 byte header).</param>
    /// <param name="swap">Whether the file endianness differs from the current platform.</param>
    private MatFileV5Reader(Stream stream, bool swap)
    {
      _stream = stream;
      _swap = swap;
    }

    /// <summary>
    /// Reads a MATLAB MAT-file v5 from the provided stream.
    /// </summary>
    /// <param name="stream">Readable stream positioned at the start of a MAT v5 file.</param>
    /// <returns>The parsed variables as a <see cref="MatFile"/>.</returns>
    public static MatFile Read(Stream stream)
    {
      ArgumentNullException.ThrowIfNull(stream);
      if (!stream.CanRead)
        throw new ArgumentException("Stream must be readable", nameof(stream));

      Span<byte> header = stackalloc byte[128];
      ReadExactly(stream, header);

      // bytes 124..125: version (0x0100), bytes 126..127: endian indicator ("IM" or "MI")
      var endian0 = header[126];
      var endian1 = header[127];

      bool swap;
      if (endian0 == (byte)'I' && endian1 == (byte)'M')
        swap = false; // little-endian
      else if (endian0 == (byte)'M' && endian1 == (byte)'I')
        swap = true; // big-endian file
      else
        throw new InvalidDataException("Not a MAT v5 file (invalid endian indicator)");

      // if swap==true, version bytes are swapped too, but we do not strictly require it.

      var reader = new MatFileV5Reader(stream, swap);
      var result = new MatFile();
      reader.ReadDataElements(result);
      return result;
    }

    /// <summary>
    /// Reads all top-level data elements from the current reader stream.
    /// </summary>
    /// <param name="file">The target file object collecting variables.</param>
    private void ReadDataElements(MatFile file)
    {
      while (TryReadDataTag(out var dataType, out var numBytes, out var smallData))
      {
        if (dataType == MatDataType.miCOMPRESSED)
        {
          using var elementStream = CreateElementStream(numBytes, smallData);
          using var decompressed = DecompressZlibToMemory(elementStream);
          var nestedReader = new MatFileV5Reader(decompressed, _swap);
          nestedReader.ReadDataElements(file);
          continue;
        }

        if (dataType != MatDataType.miMATRIX)
        {
          SkipBytes(numBytes, smallData);
          continue;
        }

        using var matrixStream = CreateElementStream(numBytes, smallData);
        ReadMatrix(matrixStream, file);
      }
    }

    /// <summary>
    /// Reads a single <c>miMATRIX</c> element from <paramref name="elementStream"/> and stores it in <paramref name="file"/>.
    /// </summary>
    private void ReadMatrix(Stream elementStream, MatFile file)
    {
      var (name, value) = ReadMatrixCore(elementStream);

      if (!string.IsNullOrEmpty(name) && value is not null)
        file.Add(name, value);
    }

    /// <summary>
    /// Reads a <c>miMATRIX</c> element from <paramref name="elementStream"/> and returns only the decoded value.
    /// </summary>
    /// <remarks>
    /// Used for nested matrices inside cells and struct/object fields, which typically carry an empty array name.
    /// This method is intentionally tolerant and returns <see langword="null"/> on malformed/truncated payloads.
    /// </remarks>
    private MatValue? ReadMatrixValue(Stream elementStream)
    {
      try
      {
        if (elementStream.CanSeek && (elementStream.Length - elementStream.Position) < 8)
          return null;

        var (_, value) = ReadMatrixCore(elementStream);
        return value;
      }
      catch (EndOfStreamException)
      {
        return null;
      }
      catch (InvalidDataException)
      {
        return null;
      }
    }

    /// <summary>
    /// Reads the common <c>miMATRIX</c> header (flags, dimensions, name) and then decodes the payload.
    /// </summary>
    /// <returns>The matrix name (may be empty for nested matrices) and the decoded value.</returns>
    private (string? Name, MatValue? Value) ReadMatrixCore(Stream elementStream)
    {
      // Array Flags
      var (flagsType, flagsLen) = ReadTag(elementStream);
      if (flagsType != MatDataType.miUINT32 || flagsLen < 8)
        throw new InvalidDataException("Invalid array flags");

      var flags0 = ReadUInt32(elementStream);
      var flags1 = ReadUInt32(elementStream);
      SkipPadding(elementStream, flagsLen);

      var arrayClass = (MatArrayClass)(flags0 & 0xFF);
      var isComplex = (flags0 & 0x0800) != 0;
      var isSparse = (flags0 & 0x0400) != 0;
      var isLogical = (flags0 & 0x0200) != 0;

      // Dimensions
      var (dimsType, dimsLen) = ReadTag(elementStream);
      if (dimsType != MatDataType.miINT32 && dimsType != MatDataType.miUINT32)
        throw new InvalidDataException("Invalid dimensions type");

      if (dimsLen < 8)
        throw new InvalidDataException("Invalid dimensions length");

      var dimCount = dimsLen / 4;
      Span<int> dims = dimCount <= 8 ? stackalloc int[dimCount] : new int[dimCount];
      for (int i = 0; i < dimCount; i++)
        dims[i] = (int)ReadUInt32(elementStream);
      SkipPadding(elementStream, dimsLen);

      int rows = dims.Length >= 1 ? dims[0] : 0;
      int cols = dims.Length >= 2 ? dims[1] : 1;

      // Array Name
      var (nameType, nameBytes) = ReadDataElement(elementStream);
      if (nameType != MatDataType.miINT8 && nameType != MatDataType.miUINT8)
        throw new InvalidDataException("Invalid array name type");

      var name = Encoding.UTF8.GetString(nameBytes);

      if (isComplex || isSparse)
      {
        ConsumeRemainingElements(elementStream);
        return (name, null);
      }

      var valueObj = ReadMatrixPayload(elementStream, arrayClass, isLogical, dims.ToArray());

      return (name, valueObj);
    }

    /// <summary>
    /// Decodes the payload of a <c>miMATRIX</c> element based on MATLAB array class and flags.
    /// </summary>
    private MatValue? ReadMatrixPayload(Stream elementStream, MatArrayClass arrayClass, bool isLogical, int[] dims)
    {
      // For all below, read the next data elements according to class.
      // Real part data element
      if (arrayClass == MatArrayClass.mxCHAR_CLASS)
      {
        var (dt, bytes) = ReadDataElement(elementStream);
        var s = DecodeCharArray(dt, bytes, dims);
        return new MatValue.String(s);
      }

      if (arrayClass == MatArrayClass.mxCELL_CLASS)
      {
        var elementCount = dims.Length == 0 ? 0 : dims.Aggregate(1, (a, b) => a * b);
        var elements = new MatValue[elementCount];
        for (int i = 0; i < elementCount; i++)
        {
          var (dt, len, smallData) = ReadTagWithPossibleSmallData(elementStream);
          if (!smallData.IsEmpty)
          {
            elements[i] = new MatValue.String(string.Empty);
            continue;
          }

          if (dt != MatDataType.miMATRIX)
          {
            SkipElementBody(elementStream, len);
            elements[i] = new MatValue.String(string.Empty);
            continue;
          }

          var buf = ReadBytes(elementStream, len);
          SkipAlignmentPadding(elementStream, len);
          using var nested = new MemoryStream(buf, writable: false);
          // Each cell is a full miMATRIX element. Cells/struct fields typically have empty array names.
          elements[i] = ReadMatrixValue(nested) ?? new MatValue.String(string.Empty);
        }
        return new MatValue.CellArray(elements, dims);
      }

      if (arrayClass == MatArrayClass.mxSTRUCT_CLASS || arrayClass == MatArrayClass.mxOBJECT_CLASS)
      {
        string? className = null;
        if (arrayClass == MatArrayClass.mxOBJECT_CLASS)
        {
          // Object class name (miINT8/miUINT8) comes before struct fields.
          var (cnType, cnBytes) = ReadDataElement(elementStream);
          if (cnType != MatDataType.miINT8 && cnType != MatDataType.miUINT8)
            return null;
          className = Encoding.UTF8.GetString(cnBytes).TrimEnd('\0');
        }

        // Field name length
        var (fnlType, fnlBytes) = ReadDataElement(elementStream);
        if (fnlType != MatDataType.miINT32 && fnlType != MatDataType.miUINT32)
          return null;
        if (fnlBytes.Length != 4)
          return null;
        var fieldNameLengthI32 = ReadInt32(fnlBytes);
        if (fieldNameLengthI32 <= 0)
          return null;

        // Field names
        var (fnType, fnBytes) = ReadDataElement(elementStream);
        if (fnType != MatDataType.miINT8 && fnType != MatDataType.miUINT8)
          return null;

        var fieldNames = new List<string>();
        for (int offset = 0; offset + fieldNameLengthI32 <= fnBytes.Length; offset += fieldNameLengthI32)
        {
          var chunk = fnBytes.AsSpan(offset, fieldNameLengthI32);
          int z = chunk.IndexOf((byte)0);
          if (z < 0) z = chunk.Length;
          var fname = Encoding.ASCII.GetString(chunk[..z]);
          if (!string.IsNullOrEmpty(fname))
            fieldNames.Add(fname);
        }

        var elementCount = dims.Length == 0 ? 0 : dims.Aggregate(1, (a, b) => a * b);
        var fields = new Dictionary<string, MatValue[]>(StringComparer.Ordinal);
        foreach (var fn in fieldNames)
          fields[fn] = new MatValue[elementCount];

        // Field values: for each element, for each field => miMATRIX
        for (int i = 0; i < elementCount; i++)
        {
          foreach (var fn in fieldNames)
          {
            var (dt, len, smallData) = ReadTagWithPossibleSmallData(elementStream);
            if (!smallData.IsEmpty)
            {
              fields[fn][i] = new MatValue.String(string.Empty);
              continue;
            }

            if (dt != MatDataType.miMATRIX)
            {
              SkipElementBody(elementStream, len);
              fields[fn][i] = new MatValue.String(string.Empty);
              continue;
            }

            var buf = ReadBytes(elementStream, len);
            SkipAlignmentPadding(elementStream, len);
            using var nested = new MemoryStream(buf, writable: false);
            fields[fn][i] = ReadMatrixValue(nested) ?? new MatValue.String(string.Empty);
          }
        }

        return arrayClass == MatArrayClass.mxOBJECT_CLASS
          ? new MatValue.ObjectArray(className ?? string.Empty, fields, dims)
          : new MatValue.StructArray(fields, dims);
      }

      // Numeric / logical classes
      {
        var (realType, realBytes) = ReadDataElement(elementStream);

        var nElements = dims.Length == 0 ? 0 : dims.Aggregate(1, (a, b) => a * b);
        if (nElements == 0)
          return null;

        if (isLogical || arrayClass == MatArrayClass.mxUINT8_CLASS)
        {
          // logical arrays are stored as uint8 0/1
          if (realType != MatDataType.miUINT8 && realType != MatDataType.miINT8)
            return null;
          var bools = new bool[nElements];
          for (int i = 0; i < bools.Length && i < realBytes.Length; i++)
            bools[i] = realBytes[i] != 0;

          if (nElements == 1)
            return new MatValue.LogicalScalar(bools[0]);

          return new MatValue.LogicalArray(bools, dims);
        }

        // Convert supported numeric storage to double[]
        var data = DecodeNumericToDouble(realType, realBytes, nElements);
        if (data is null)
          return null;

        if (dims.Length == 2)
        {
          var r = dims[0];
          var c = dims[1];
          if (r == 1 && c == 1)
            return new MatValue.Scalar(data[0]);
          if (r == 1 || c == 1)
            return new MatValue.Vector(data);
          return new MatValue.Matrix(r, c, data, IsColumnMajor: true);
        }

        return new MatValue.NumericArray(data, dims);
      }
    }

    /// <summary>
    /// Decodes a MATLAB character array data element to a managed string.
    /// </summary>
    private string DecodeCharArray(MatDataType dataType, byte[] bytes, int[] dims)
    {
      int rows = dims.Length >= 1 ? dims[0] : 0;
      int cols = dims.Length >= 2 ? dims[1] : 0;

      int elementSize = dataType switch
      {
        MatDataType.miINT8 or MatDataType.miUINT8 or MatDataType.miUTF8 => 1,
        MatDataType.miINT16 or MatDataType.miUINT16 or MatDataType.miUTF16 => 2,
        MatDataType.miUTF32 => 4,
        _ => 0
      };

      if (elementSize == 0 || bytes.Length == 0)
        return string.Empty;

      int elementCount = bytes.Length / elementSize;
      if (dims.Length >= 2 && rows > 0 && cols > 0)
        elementCount = Math.Min(elementCount, rows * cols);

      var trimmedBytes = bytes.AsSpan(0, elementCount * elementSize);

      // MATLAB stores array elements in column-major order; for multi-row char matrices,
      // reorder to row-major so that each row forms a readable line.
      byte[] rowMajorBytes;
      if (dims.Length >= 2 && rows > 1 && cols > 1)
      {
        rowMajorBytes = new byte[trimmedBytes.Length];
        for (int r = 0; r < rows; r++)
        {
          for (int c = 0; c < cols; c++)
          {
            int srcElementIndex = r + rows * c; // column-major
            int dstElementIndex = r * cols + c; // row-major

            var src = trimmedBytes.Slice(srcElementIndex * elementSize, elementSize);
            src.CopyTo(rowMajorBytes.AsSpan(dstElementIndex * elementSize, elementSize));
          }
        }
      }
      else
      {
        rowMajorBytes = trimmedBytes.ToArray();
      }

      Encoding encoding = dataType switch
      {
        MatDataType.miINT8 or MatDataType.miUINT8 or MatDataType.miUTF8 => Encoding.UTF8,
        MatDataType.miINT16 or MatDataType.miUINT16 or MatDataType.miUTF16 => _swap ? Encoding.BigEndianUnicode : Encoding.Unicode,
        MatDataType.miUTF32 => new UTF32Encoding(bigEndian: _swap, byteOrderMark: false, throwOnInvalidCharacters: false),
        _ => Encoding.UTF8
      };

      var allChars = encoding.GetString(rowMajorBytes).TrimEnd('\0');

      if (dims.Length >= 2 && rows > 1 && cols > 1 && allChars.Length >= rows * cols)
      {
        // MATLAB stores multi-row char arrays as fixed-width lines padded with spaces.
        // For import, normalize by trimming each line and concatenating them with a
        // single space so that wrapped text becomes a single paragraph.
        var sb = new StringBuilder(rows * cols);
        for (int r = 0; r < rows; r++)
        {
          var line = allChars.AsSpan(r * cols, cols).Trim("\0 ".AsSpan());
          if (line.IsEmpty)
            continue;

          if (sb.Length > 0)
            sb.Append(' ');

          sb.Append(line);
        }
        return sb.ToString().TrimEnd();
      }

      return allChars;
    }

    /// <summary>
    /// Converts the raw numeric data bytes of a MATLAB array to a <see cref="double"/> array.
    /// </summary>
    /// <param name="realType">Storage type of the numeric element.</param>
    /// <param name="raw">Raw bytes of the numeric element.</param>
    /// <param name="n">Number of elements to decode.</param>
    /// <returns>Decoded data as doubles, or <see langword="null"/> if the storage type is unsupported.</returns>
    private double[]? DecodeNumericToDouble(MatDataType realType, byte[] raw, int n)
    {
      var data = new double[n];
      switch (realType)
      {
        case MatDataType.miDOUBLE:
          for (int i = 0; i < n; i++)
          {
            var v = BinaryPrimitives.ReadDoubleLittleEndian(raw.AsSpan(i * 8, 8));
            if (_swap)
              v = BitConverter.Int64BitsToDouble(BinaryPrimitives.ReverseEndianness(BitConverter.DoubleToInt64Bits(v)));
            data[i] = v;
          }
          return data;
        case MatDataType.miSINGLE:
          for (int i = 0; i < n; i++)
          {
            var v = BinaryPrimitives.ReadSingleLittleEndian(raw.AsSpan(i * 4, 4));
            if (_swap)
              v = BitConverter.Int32BitsToSingle(BinaryPrimitives.ReverseEndianness(BitConverter.SingleToInt32Bits(v)));
            data[i] = v;
          }
          return data;
        case MatDataType.miINT8:
          for (int i = 0; i < n; i++) data[i] = unchecked((sbyte)raw[i]);
          return data;
        case MatDataType.miUINT8:
          for (int i = 0; i < n; i++) data[i] = raw[i];
          return data;
        case MatDataType.miINT16:
          for (int i = 0; i < n; i++)
          {
            short v = BinaryPrimitives.ReadInt16LittleEndian(raw.AsSpan(i * 2, 2));
            if (_swap) v = BinaryPrimitives.ReverseEndianness(v);
            data[i] = v;
          }
          return data;
        case MatDataType.miUINT16:
          for (int i = 0; i < n; i++)
          {
            ushort v = BinaryPrimitives.ReadUInt16LittleEndian(raw.AsSpan(i * 2, 2));
            if (_swap) v = BinaryPrimitives.ReverseEndianness(v);
            data[i] = v;
          }
          return data;
        case MatDataType.miINT32:
          for (int i = 0; i < n; i++)
          {
            int v = BinaryPrimitives.ReadInt32LittleEndian(raw.AsSpan(i * 4, 4));
            if (_swap) v = BinaryPrimitives.ReverseEndianness(v);
            data[i] = v;
          }
          return data;
        case MatDataType.miUINT32:
          for (int i = 0; i < n; i++)
          {
            uint v = BinaryPrimitives.ReadUInt32LittleEndian(raw.AsSpan(i * 4, 4));
            if (_swap) v = BinaryPrimitives.ReverseEndianness(v);
            data[i] = v;
          }
          return data;
        case MatDataType.miINT64:
          for (int i = 0; i < n; i++)
          {
            long v = BinaryPrimitives.ReadInt64LittleEndian(raw.AsSpan(i * 8, 8));
            if (_swap) v = BinaryPrimitives.ReverseEndianness(v);
            data[i] = v;
          }
          return data;
        case MatDataType.miUINT64:
          for (int i = 0; i < n; i++)
          {
            ulong v = BinaryPrimitives.ReadUInt64LittleEndian(raw.AsSpan(i * 8, 8));
            if (_swap) v = BinaryPrimitives.ReverseEndianness(v);
            data[i] = v;
          }
          return data;
        default:
          return null;
      }
    }

    /// <summary>
    /// Drains the remaining bytes of an element stream on a best-effort basis.
    /// </summary>
    private static void ConsumeRemainingElements(Stream s)
    {
      // best-effort: drain stream
      if (s.CanSeek)
        s.Seek(0, SeekOrigin.End);
      else
        _ = ReadBytes(s, (int)Math.Max(0, s.Length - s.Position));
    }

    /// <summary>
    /// Skips the body of a data element and its 8-byte alignment padding.
    /// </summary>
    private static void SkipElementBody(Stream s, int length)
    {
      _ = ReadBytes(s, length);
      SkipAlignmentPadding(s, length);
    }

    /// <summary>
    /// Decompresses a zlib stream (RFC1950) into an in-memory stream.
    /// </summary>
    private static MemoryStream DecompressZlibToMemory(Stream compressedElementStream)
    {
      // miCOMPRESSED wraps zlib stream (RFC1950). .NET's ZLibStream handles it.
      using var z = new ZLibStream(compressedElementStream, CompressionMode.Decompress, leaveOpen: true);
      var ms = new MemoryStream();
      z.CopyTo(ms);
      ms.Position = 0;
      return ms;
    }

    /// <summary>
    /// Reads a standard (non-small-data) MAT v5 tag.
    /// </summary>
    private (MatDataType Type, int Length) ReadTag(Stream s)
    {
      Span<byte> b = stackalloc byte[8];
      ReadExactly(s, b);
      var dt = ReadInt32(b[..4]);
      var len = ReadInt32(b[4..]);
      return ((MatDataType)dt, len);
    }

    /// <summary>
    /// Reads a MAT v5 tag and handles the "small data element" format.
    /// </summary>
    /// <remarks>
    /// Small data elements pack up to 4 bytes directly into the tag.
    /// </remarks>
    private (MatDataType Type, int Length, ReadOnlyMemory<byte> SmallData) ReadTagWithPossibleSmallData(Stream s)
    {
      Span<byte> b = stackalloc byte[8];
      ReadExactly(s, b);

      // Small data element format: [numBytes:16][type:16][data:32]
      var word0 = ReadUInt32(b[..4]);
      var word1 = ReadUInt32(b[4..]);

      var possibleType = (ushort)(word0 & 0xFFFF);
      var possibleBytes = (ushort)((word0 >> 16) & 0xFFFF);

      if (possibleBytes != 0 && (MatDataType)possibleType != 0 && possibleBytes <= 4)
      {
        Span<byte> data4 = stackalloc byte[4];
        BinaryPrimitives.WriteUInt32LittleEndian(data4, word1);
        if (_swap)
          data4.Reverse();
        return ((MatDataType)possibleType, possibleBytes, data4[..possibleBytes].ToArray());
      }

      var dt = ReadInt32(b[..4]);
      var len = ReadInt32(b[4..]);
      return ((MatDataType)dt, len, default);
    }

    /// <summary>
    /// Reads a data element (tag + body) and returns the type and body bytes.
    /// </summary>
    private (MatDataType Type, byte[] Data) ReadDataElement(Stream s)
    {
      var (type, len, smallData) = ReadTagWithPossibleSmallData(s);
      if (!smallData.IsEmpty)
        return (type, smallData.ToArray());

      var data = ReadBytes(s, len);
      SkipPadding(s, len);
      return (type, data);
    }

    /// <summary>
    /// Attempts to read the next top-level data tag from the underlying stream.
    /// </summary>
    /// <returns><see langword="true"/> when a tag was read; <see langword="false"/> on end of stream.</returns>
    private bool TryReadDataTag(out MatDataType type, out int numBytes, out ReadOnlyMemory<byte> smallData)
    {
      smallData = default;
      Span<byte> b = stackalloc byte[8];
      int read = _stream.Read(b);
      if (read == 0)
      {
        type = default;
        numBytes = 0;
        return false;
      }
      if (read != 8)
        throw new EndOfStreamException();

      // Small data element format: [numBytes:16][type:16][data:32]
      var word0 = ReadUInt32(b[..4]);
      var word1 = ReadUInt32(b[4..]);

      var possibleType = (ushort)(word0 & 0xFFFF);
      var possibleBytes = (ushort)((word0 >> 16) & 0xFFFF);

      if (possibleBytes != 0 && (MatDataType)possibleType != 0 && possibleBytes <= 4)
      {
        type = (MatDataType)possibleType;
        numBytes = possibleBytes;

        Span<byte> data4 = stackalloc byte[4];
        BinaryPrimitives.WriteUInt32LittleEndian(data4, word1);
        if (_swap)
          data4.Reverse();
        smallData = data4[..numBytes].ToArray();

        // small data element consumes exactly 8 bytes, already aligned
        return true;
      }

      type = (MatDataType)ReadInt32(b[..4]);
      numBytes = ReadInt32(b[4..]);
      return true;
    }

    /// <summary>
    /// Creates a stream for an element body (either from inline small data or by reading from the underlying stream).
    /// </summary>
    private Stream CreateElementStream(int numBytes, ReadOnlyMemory<byte> smallData)
    {
      if (!smallData.IsEmpty)
        return new MemoryStream(smallData.ToArray(), writable: false);

      var buf = ReadBytes(_stream, numBytes);
      SkipAlignmentPadding(numBytes);
      return new MemoryStream(buf, writable: false);
    }

    /// <summary>
    /// Skips a top-level element body (and padding) in the underlying stream.
    /// </summary>
    private void SkipBytes(int numBytes, ReadOnlyMemory<byte> smallData)
    {
      if (!smallData.IsEmpty)
        return;

      _ = ReadBytes(_stream, numBytes);
      SkipAlignmentPadding(numBytes);
    }

    /// <summary>
    /// Reads exactly <paramref name="count"/> bytes from <paramref name="s"/>.
    /// </summary>
    private static byte[] ReadBytes(Stream s, int count)
    {
      if (count == 0)
        return [];

      var buffer = new byte[count];
      ReadExactly(s, buffer);
      return buffer;
    }

    /// <summary>
    /// Reads until <paramref name="buffer"/> is filled, or throws on end of stream.
    /// </summary>
    private static void ReadExactly(Stream s, Span<byte> buffer)
    {
      int total = 0;
      while (total < buffer.Length)
      {
        int n = s.Read(buffer[total..]);
        if (n <= 0)
          throw new EndOfStreamException();
        total += n;
      }
    }

    /// <summary>
    /// Reads a 32-bit integer from a byte span, applying endianness swapping if needed.
    /// </summary>
    private int ReadInt32(ReadOnlySpan<byte> b)
    {
      var v = BinaryPrimitives.ReadInt32LittleEndian(b);
      return _swap ? BinaryPrimitives.ReverseEndianness(v) : v;
    }

    /// <summary>
    /// Reads an unsigned 32-bit integer from a byte span, applying endianness swapping if needed.
    /// </summary>
    private uint ReadUInt32(ReadOnlySpan<byte> b)
    {
      var v = BinaryPrimitives.ReadUInt32LittleEndian(b);
      return _swap ? BinaryPrimitives.ReverseEndianness(v) : v;
    }

    /// <summary>
    /// Reads an unsigned 32-bit integer from a stream, applying endianness swapping if needed.
    /// </summary>
    private uint ReadUInt32(Stream s)
    {
      Span<byte> b = stackalloc byte[4];
      ReadExactly(s, b);
      return ReadUInt32(b);
    }

    /// <summary>
    /// Skips (optional) intra-element padding to reach the next 8-byte boundary.
    /// </summary>
    private void SkipPadding(Stream s, int elementLength)
    {
      var pad = (8 - (elementLength % 8)) % 8;
      if (pad == 0)
        return;

      // Some files omit the optional padding. Be tolerant: only consume if the bytes are actually zero.
      if (s.CanSeek)
      {
        var remaining = s.Length - s.Position;
        if (remaining < pad)
          return;

        Span<byte> tmp = stackalloc byte[8];
        ReadExactly(s, tmp[..pad]);

        bool allZero = true;
        for (int i = 0; i < pad; i++)
        {
          if (tmp[i] != 0)
          {
            allZero = false;
            break;
          }
        }

        if (!allZero)
          s.Seek(-pad, SeekOrigin.Current);
      }
      else
      {
        Span<byte> tmp = stackalloc byte[8];
        ReadExactly(s, tmp[..pad]);
      }
    }

    /// <summary>
    /// Skips (optional) alignment padding in the underlying stream after reading a top-level element.
    /// </summary>
    private void SkipAlignmentPadding(int elementLength)
    {
      var pad = (8 - (elementLength % 8)) % 8;
      if (pad == 0)
        return;

      // Some MAT files (in the wild) omit the optional alignment padding between top-level data elements.
      // To be robust, only consume padding bytes if they are actually zero. If not, rewind.
      if (_stream.CanSeek)
      {
        var remaining = _stream.Length - _stream.Position;
        if (remaining < pad)
          return;

        Span<byte> tmp = stackalloc byte[8];
        ReadExactly(_stream, tmp[..pad]);

        bool allZero = true;
        for (int i = 0; i < pad; i++)
        {
          if (tmp[i] != 0)
          {
            allZero = false;
            break;
          }
        }

        if (!allZero)
          _stream.Seek(-pad, SeekOrigin.Current);
      }
      else
      {
        Span<byte> tmp = stackalloc byte[8];
        ReadExactly(_stream, tmp[..pad]);
      }
    }

    /// <summary>
    /// Skips (optional) alignment padding in the provided stream.
    /// </summary>
    private static void SkipAlignmentPadding(Stream s, int elementLength)
    {
      var pad = (8 - (elementLength % 8)) % 8;
      if (pad == 0)
        return;

      // Some files omit the optional alignment padding. Be tolerant: only consume if the bytes are actually zero.
      if (s.CanSeek)
      {
        var remaining = s.Length - s.Position;
        if (remaining < pad)
          return;

        Span<byte> tmp = stackalloc byte[8];
        ReadExactly(s, tmp[..pad]);

        bool allZero = true;
        for (int i = 0; i < pad; i++)
        {
          if (tmp[i] != 0)
          {
            allZero = false;
            break;
          }
        }

        if (!allZero)
          s.Seek(-pad, SeekOrigin.Current);
      }
      else
      {
        Span<byte> tmp = stackalloc byte[8];
        ReadExactly(s, tmp[..pad]);
      }
    }

    /// <summary>
    /// MAT v5 data types used in tags and data elements.
    /// </summary>
    private enum MatDataType : int
    {
      /// <summary>
      /// 8-bit signed integer data.
      /// </summary>
      miINT8 = 1,

      /// <summary>
      /// 8-bit unsigned integer data.
      /// </summary>
      miUINT8 = 2,

      /// <summary>
      /// 16-bit signed integer data.
      /// </summary>
      miINT16 = 3,

      /// <summary>
      /// 16-bit unsigned integer data.
      /// </summary>
      miUINT16 = 4,

      /// <summary>
      /// 32-bit signed integer data.
      /// </summary>
      miINT32 = 5,

      /// <summary>
      /// 32-bit unsigned integer data.
      /// </summary>
      miUINT32 = 6,

      /// <summary>
      /// 32-bit IEEE 754 floating point data.
      /// </summary>
      miSINGLE = 7,

      /// <summary>
      /// 64-bit IEEE 754 floating point data.
      /// </summary>
      miDOUBLE = 9,

      /// <summary>
      /// 64-bit signed integer data.
      /// </summary>
      miINT64 = 12,

      /// <summary>
      /// 64-bit unsigned integer data.
      /// </summary>
      miUINT64 = 13,

      /// <summary>
      /// MATLAB matrix element (<c>miMATRIX</c>), used for variables, cells and struct/object fields.
      /// </summary>
      miMATRIX = 14,

      /// <summary>
      /// Compressed data element (<c>miCOMPRESSED</c>) containing a zlib (RFC1950) stream.
      /// </summary>
      miCOMPRESSED = 15,

      /// <summary>
      /// UTF-8 encoded character data.
      /// </summary>
      miUTF8 = 16,

      /// <summary>
      /// UTF-16 encoded character data.
      /// </summary>
      miUTF16 = 17,

      /// <summary>
      /// UTF-32 encoded character data.
      /// </summary>
      miUTF32 = 18,
    }

    /// <summary>
    /// MATLAB array classes stored in the Array Flags element (<c>miUINT32</c> payload).
    /// </summary>
    private enum MatArrayClass : int
    {
      /// <summary>
      /// Cell array.
      /// </summary>
      mxCELL_CLASS = 1,

      /// <summary>
      /// Struct array.
      /// </summary>
      mxSTRUCT_CLASS = 2,

      /// <summary>
      /// Object array.
      /// </summary>
      mxOBJECT_CLASS = 3,

      /// <summary>
      /// Character array.
      /// </summary>
      mxCHAR_CLASS = 4,

      /// <summary>
      /// Sparse matrix.
      /// </summary>
      mxSPARSE_CLASS = 5,

      /// <summary>
      /// Double-precision numeric array.
      /// </summary>
      mxDOUBLE_CLASS = 6,

      /// <summary>
      /// Single-precision numeric array.
      /// </summary>
      mxSINGLE_CLASS = 7,

      /// <summary>
      /// 8-bit signed integer array.
      /// </summary>
      mxINT8_CLASS = 8,

      /// <summary>
      /// 8-bit unsigned integer array.
      /// </summary>
      mxUINT8_CLASS = 9,

      /// <summary>
      /// 16-bit signed integer array.
      /// </summary>
      mxINT16_CLASS = 10,

      /// <summary>
      /// 16-bit unsigned integer array.
      /// </summary>
      mxUINT16_CLASS = 11,

      /// <summary>
      /// 32-bit signed integer array.
      /// </summary>
      mxINT32_CLASS = 12,

      /// <summary>
      /// 32-bit unsigned integer array.
      /// </summary>
      mxUINT32_CLASS = 13,

      /// <summary>
      /// 64-bit signed integer array.
      /// </summary>
      mxINT64_CLASS = 14,

      /// <summary>
      /// 64-bit unsigned integer array.
      /// </summary>
      mxUINT64_CLASS = 15,
    }
  }
}
