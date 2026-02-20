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
using System.Linq;
using System.Text;

namespace Altaxo.Serialization.Matlab
{
  /// <summary>
  /// MAT-file v5 writer.
  /// </summary>
  public sealed class MatFileV5Writer : IDisposable
  {
    private readonly MemoryStream _stream;
    private bool _headerWritten;

    // Method layout:
    // 1) Public API (write supported value types)
    // 2) Internal helpers (encode matrix payloads)
    // 3) Low-level write primitives (tags, integers, padding)
    // 4) Enums

    /// <summary>
    /// Initializes a new in-memory MAT-file writer.
    /// </summary>
    public MatFileV5Writer()
    {
      _stream = new MemoryStream();
    }

    /// <summary>
    /// Disposes the underlying in-memory stream.
    /// </summary>
    public void Dispose()
    {
      _stream.Dispose();
    }

    /// <summary>
    /// Returns the written MAT-file as a byte array.
    /// </summary>
    public byte[] ToArray()
    {
      EnsureHeader();
      return _stream.ToArray();
    }

    /// <summary>
    /// Writes a named variable as an <c>miMATRIX</c> element.
    /// </summary>
    /// <param name="name">Variable name.</param>
    /// <param name="value">Value to write.</param>
    /// <exception cref="NotSupportedException">Thrown if the value type is not supported by this writer.</exception>
    public void Write(string name, MatValue value)
    {
      ArgumentException.ThrowIfNullOrEmpty(name);
      ArgumentNullException.ThrowIfNull(value);

      EnsureHeader();

      using var payload = BuildMatrixPayload(name, value);
      WriteMatrixElementToFile(payload);
    }

    /// <summary>
    /// Writes a MATLAB character array (<c>mxCHAR_CLASS</c>) as a 1xN string.
    /// </summary>
    /// <param name="name">Variable name.</param>
    /// <param name="value">String content.</param>
    public void WriteString(string name, string value)
    {
      Write(name, new MatValue.String(value ?? string.Empty));
    }

    /// <summary>
    /// Writes a MATLAB logical scalar (<c>mxDOUBLE_CLASS</c> with logical flag) as a 1x1 value.
    /// </summary>
    /// <param name="name">Variable name.</param>
    /// <param name="value">Logical value.</param>
    public void WriteLogicalScalar(string name, bool value)
    {
      Write(name, new MatValue.LogicalScalar(value));
    }

    /// <summary>
    /// Writes a scalar double value.
    /// </summary>
    /// <param name="name">Variable name.</param>
    /// <param name="value">Scalar value.</param>
    public void WriteScalarDouble(string name, double value)
    {
      Write(name, new MatValue.Scalar(value));
    }

    /// <summary>
    /// Writes a double column vector (N x 1).
    /// </summary>
    /// <param name="name">Variable name.</param>
    /// <param name="data">Vector data.</param>
    public void WriteVectorDouble(string name, ReadOnlySpan<double> data)
    {
      Write(name, new MatValue.Vector(data.ToArray()));
    }

    /// <summary>
    /// Writes a double matrix with column-major data layout.
    /// </summary>
    /// <param name="name">Variable name.</param>
    /// <param name="rows">Row count.</param>
    /// <param name="cols">Column count.</param>
    /// <param name="dataColumnMajor">Matrix data in MATLAB column-major order.</param>
    public void WriteMatrixDouble(string name, int rows, int cols, ReadOnlySpan<double> dataColumnMajor)
    {
      if (rows < 0)
        throw new ArgumentOutOfRangeException(nameof(rows));
      if (cols < 0)
        throw new ArgumentOutOfRangeException(nameof(cols));
      if (dataColumnMajor.Length != rows * cols)
        throw new ArgumentException("Data length must be rows*cols", nameof(dataColumnMajor));

      Write(name, new MatValue.Matrix(rows, cols, dataColumnMajor.ToArray(), IsColumnMajor: true));
    }

    /// <summary>
    /// Builds the <c>miMATRIX</c> payload (flags, dimensions, name and data elements) for a given value.
    /// </summary>
    /// <param name="name">Variable name (empty for nested matrices inside cells/struct fields).</param>
    /// <param name="value">Value to encode.</param>
    /// <returns>A stream containing only the matrix payload (without the outer <c>miMATRIX</c> tag).</returns>
    private MemoryStream BuildMatrixPayload(string name, MatValue value)
    {
      var matrix = new MemoryStream();

      switch (value)
      {
        case MatValue.String s:
          {
            WriteArrayFlags(matrix, MatArrayClass.mxCHAR_CLASS);
            WriteDimensions(matrix, [1, s.Value.Length]);
            WriteName(matrix, name);

            var chars = s.Value.ToCharArray();
            var realLen = chars.Length * 2;
            WriteTag(matrix, type: (int)MatDataType.miUINT16, numBytes: realLen);
            Span<byte> b = stackalloc byte[2];
            foreach (var ch in chars)
            {
              BinaryPrimitives.WriteUInt16LittleEndian(b, ch);
              matrix.Write(b);
            }
            PadTo8(matrix, realLen);
            return matrix;
          }

        case MatValue.LogicalScalar ls:
          {
            WriteArrayFlags(matrix, MatArrayClass.mxDOUBLE_CLASS, isLogical: true);
            WriteDimensions(matrix, [1, 1]);
            WriteName(matrix, name);

            WriteTag(matrix, type: (int)MatDataType.miUINT8, numBytes: 1);
            matrix.WriteByte((byte)(ls.Value ? 1 : 0));
            PadTo8(matrix, 1);
            return matrix;
          }

        case MatValue.LogicalArray la:
          {
            var dims = la.Dimensions.ToArray();
            var elementCount = GetElementCount(dims);
            if (la.Data.Length != elementCount)
              throw new InvalidDataException("LogicalArray data length does not match dimensions");

            WriteArrayFlags(matrix, MatArrayClass.mxDOUBLE_CLASS, isLogical: true);
            WriteDimensions(matrix, dims);
            WriteName(matrix, name);

            var bytes = EnsureColumnMajor(la.Data.Span, dims, la.IsColumnMajor);
            WriteTag(matrix, type: (int)MatDataType.miUINT8, numBytes: bytes.Length);
            matrix.Write(bytes);
            PadTo8(matrix, bytes.Length);
            return matrix;
          }

        case MatValue.Scalar sc:
          {
            WriteArrayFlags(matrix, MatArrayClass.mxDOUBLE_CLASS);
            WriteDimensions(matrix, [1, 1]);
            WriteName(matrix, name);

            WriteTag(matrix, type: (int)MatDataType.miDOUBLE, numBytes: 8);
            WriteDouble(matrix, sc.Value);
            PadTo8(matrix, 8);
            return matrix;
          }

        case MatValue.Vector v:
          {
            var len = v.Data.Count;
            WriteArrayFlags(matrix, MatArrayClass.mxDOUBLE_CLASS);
            WriteDimensions(matrix, [len, 1]);
            WriteName(matrix, name);

            WriteTag(matrix, type: (int)MatDataType.miDOUBLE, numBytes: len * 8);
            for (int i = 0; i < len; i++)
              WriteDouble(matrix, v.Data[i]);
            PadTo8(matrix, len * 8);
            return matrix;
          }

        case MatValue.Matrix m:
          {
            var rows = m.RowCount;
            var cols = m.ColumnCount;
            if (m.Data.Length != rows * cols)
              throw new InvalidDataException("Matrix data length does not match dimensions");

            WriteArrayFlags(matrix, MatArrayClass.mxDOUBLE_CLASS);
            WriteDimensions(matrix, [rows, cols]);
            WriteName(matrix, name);

            var data = EnsureColumnMajor(m.Data.Span, rows, cols, m.IsColumnMajor);
            WriteTag(matrix, type: (int)MatDataType.miDOUBLE, numBytes: data.Length * 8);
            foreach (var d in data)
              WriteDouble(matrix, d);
            PadTo8(matrix, data.Length * 8);
            return matrix;
          }

        case MatValue.NumericArray na:
          {
            var dims = na.Dimensions.ToArray();
            var elementCount = GetElementCount(dims);
            if (na.Data.Length != elementCount)
              throw new InvalidDataException("NumericArray data length does not match dimensions");

            WriteArrayFlags(matrix, MatArrayClass.mxDOUBLE_CLASS);
            WriteDimensions(matrix, dims);
            WriteName(matrix, name);

            var data = EnsureColumnMajor(na.Data.Span, dims, na.IsColumnMajor);
            WriteTag(matrix, type: (int)MatDataType.miDOUBLE, numBytes: data.Length * 8);
            foreach (var d in data)
              WriteDouble(matrix, d);
            PadTo8(matrix, data.Length * 8);
            return matrix;
          }

        case MatValue.CellArray ca:
          {
            var dims = ca.Dimensions.ToArray();
            var elementCount = GetElementCount(dims);
            if (ca.Elements.Length != elementCount)
              throw new InvalidDataException("CellArray element count does not match dimensions");

            WriteArrayFlags(matrix, MatArrayClass.mxCELL_CLASS);
            WriteDimensions(matrix, dims);
            WriteName(matrix, name);

            foreach (var el in ca.Elements)
            {
              using var nested = BuildMatrixPayload(string.Empty, el);
              var payloadBytes = nested.ToArray();

              WriteTag(matrix, type: (int)MatDataType.miMATRIX, numBytes: payloadBytes.Length);
              matrix.Write(payloadBytes);
              PadTo8(matrix, payloadBytes.Length);
            }

            return matrix;
          }

        case MatValue.StructArray sa:
          return WriteStructOrObjectArrayPayload(matrix, name, arrayClass: MatArrayClass.mxSTRUCT_CLASS, className: null, sa.Fields, sa.Dimensions.ToArray());

        case MatValue.ObjectArray oa:
          return WriteStructOrObjectArrayPayload(matrix, name, arrayClass: MatArrayClass.mxOBJECT_CLASS, oa.ClassName, oa.Fields, oa.Dimensions.ToArray());

        default:
          matrix.Dispose();
          throw new NotSupportedException($"MAT v5 writing for type '{value.GetType().FullName}' is not implemented");
      }
    }

    /// <summary>
    /// Writes a struct or object array payload.
    /// </summary>
    /// <param name="matrix">The target stream for the matrix payload.</param>
    /// <param name="name">Variable name.</param>
    /// <param name="arrayClass">The array class (<c>mxSTRUCT_CLASS</c> or <c>mxOBJECT_CLASS</c>).</param>
    /// <param name="className">Class name for objects; ignored for structs.</param>
    /// <param name="fields">Field values, provided as arrays of length equal to the number of struct/object elements.</param>
    /// <param name="dims">Array dimensions.</param>
    /// <returns>The same stream instance passed in as <paramref name="matrix"/>.</returns>
    private MemoryStream WriteStructOrObjectArrayPayload(
      MemoryStream matrix,
      string name,
      MatArrayClass arrayClass,
      string? className,
      IReadOnlyDictionary<string, MatValue[]> fields,
      int[] dims)
    {
      var elementCount = GetElementCount(dims);
      if (fields.Count == 0)
        throw new InvalidDataException("Struct/Object arrays must have at least one field");

      foreach (var kvp in fields)
      {
        if (kvp.Value.Length != elementCount)
          throw new InvalidDataException($"Field '{kvp.Key}' element count does not match dimensions");
      }

      var fieldNames = fields.Keys.OrderBy(x => x, StringComparer.Ordinal).ToArray();
      var fieldNameLength = Math.Max(1, fieldNames.Max(x => x.Length) + 1);

      WriteArrayFlags(matrix, arrayClass);
      WriteDimensions(matrix, dims);
      WriteName(matrix, name);

      if (arrayClass == MatArrayClass.mxOBJECT_CLASS)
      {
        var cn = className ?? string.Empty;
        var cnBytes = Encoding.UTF8.GetBytes(cn);
        WriteTag(matrix, type: (int)MatDataType.miINT8, numBytes: cnBytes.Length + 1);
        matrix.Write(cnBytes);
        matrix.WriteByte(0);
        PadTo8(matrix, cnBytes.Length + 1);
      }

      // Field name length
      WriteTag(matrix, type: (int)MatDataType.miINT32, numBytes: 4);
      WriteInt32(matrix, fieldNameLength);
      PadTo8(matrix, 4);

      // Field names (ASCII, padded to fixed length)
      var fieldNamesBytesLen = fieldNameLength * fieldNames.Length;
      WriteTag(matrix, type: (int)MatDataType.miINT8, numBytes: fieldNamesBytesLen);
      foreach (var fn in fieldNames)
      {
        var fnBytes = Encoding.ASCII.GetBytes(fn);
        matrix.Write(fnBytes);
        for (int i = fnBytes.Length; i < fieldNameLength; i++)
          matrix.WriteByte(0);
      }
      PadTo8(matrix, fieldNamesBytesLen);

      // Field values (miMATRIX for each element, for each field)
      for (int i = 0; i < elementCount; i++)
      {
        foreach (var fn in fieldNames)
        {
          using var nested = BuildMatrixPayload(string.Empty, fields[fn][i]);
          var payloadBytes = nested.ToArray();

          WriteTag(matrix, type: (int)MatDataType.miMATRIX, numBytes: payloadBytes.Length);
          matrix.Write(payloadBytes);
          PadTo8(matrix, payloadBytes.Length);
        }
      }

      return matrix;
    }

    /// <summary>
    /// Writes the MAT v5 Array Flags element.
    /// </summary>
    /// <param name="matrix">Target stream.</param>
    /// <param name="arrayClass">MATLAB array class.</param>
    /// <param name="isLogical">Whether to set the logical flag.</param>
    private static void WriteArrayFlags(Stream matrix, MatArrayClass arrayClass, bool isLogical = false)
    {
      WriteTag(matrix, type: (int)MatDataType.miUINT32, numBytes: 8);
      WriteUInt32(matrix, (uint)arrayClass | (isLogical ? 0x0200u : 0u));
      WriteUInt32(matrix, 0);
      PadTo8(matrix, 8);
    }

    /// <summary>
    /// Writes the MAT v5 Dimensions element.
    /// </summary>
    /// <param name="matrix">Target stream.</param>
    /// <param name="dims">Dimensions (MATLAB order).</param>
    private static void WriteDimensions(Stream matrix, ReadOnlySpan<int> dims)
    {
      if (dims.Length == 0)
        dims = [0, 0];

      var len = dims.Length * 4;
      WriteTag(matrix, type: (int)MatDataType.miINT32, numBytes: len);
      for (int i = 0; i < dims.Length; i++)
        WriteInt32(matrix, dims[i]);
      PadTo8(matrix, len);
    }

    /// <summary>
    /// Calculates the total element count for a MATLAB array with the given dimensions.
    /// </summary>
    private static int GetElementCount(ReadOnlySpan<int> dims)
    {
      if (dims.Length == 0)
        return 0;

      int count = 1;
      for (int i = 0; i < dims.Length; i++)
        count *= dims[i];
      return count;
    }

    /// <summary>
    /// Ensures numeric matrix data is encoded in column-major order (MATLAB layout).
    /// </summary>
    private static double[] EnsureColumnMajor(ReadOnlySpan<double> data, int rows, int cols, bool isColumnMajor)
    {
      if (isColumnMajor)
        return data.ToArray();

      // Convert row-major to column-major
      var result = new double[data.Length];
      for (int r = 0; r < rows; r++)
      {
        for (int c = 0; c < cols; c++)
        {
          var srcIdx = c + cols * r;
          var dstIdx = r + rows * c;
          result[dstIdx] = data[srcIdx];
        }
      }
      return result;
    }

    /// <summary>
    /// Ensures numeric array data is encoded in column-major order.
    /// </summary>
    /// <remarks>
    /// For dimensions other than 2D, the current implementation assumes the provided data already uses column-major ordering.
    /// </remarks>
    private static double[] EnsureColumnMajor(ReadOnlySpan<double> data, int[] dims, bool isColumnMajor)
    {
      if (isColumnMajor || dims.Length != 2)
        return data.ToArray();

      return EnsureColumnMajor(data, dims[0], dims[1], isColumnMajor: false);
    }

    /// <summary>
    /// Ensures logical array data is encoded in column-major order.
    /// </summary>
    /// <remarks>
    /// For dimensions other than 2D, the current implementation assumes the provided data already uses column-major ordering.
    /// </remarks>
    private static byte[] EnsureColumnMajor(ReadOnlySpan<bool> data, int[] dims, bool isColumnMajor)
    {
      if (isColumnMajor || dims.Length != 2)
      {
        var bytes = new byte[data.Length];
        for (int i = 0; i < data.Length; i++)
          bytes[i] = (byte)(data[i] ? 1 : 0);
        return bytes;
      }

      var rows = dims[0];
      var cols = dims[1];
      var result = new byte[data.Length];
      for (int r = 0; r < rows; r++)
      {
        for (int c = 0; c < cols; c++)
        {
          var srcIdx = c + cols * r;
          var dstIdx = r + rows * c;
          result[dstIdx] = (byte)(data[srcIdx] ? 1 : 0);
        }
      }
      return result;
    }

    /// <summary>
    /// Ensures the MAT-file header is present at the beginning of the stream.
    /// </summary>
    private void EnsureHeader()
    {
      if (_headerWritten)
        return;

      WriteHeader(_stream);
      _headerWritten = true;
    }

    /// <summary>
    /// Writes the 128-byte MAT v5 header.
    /// </summary>
    private static void WriteHeader(Stream s)
    {
      var header = new byte[128];
      var desc = Encoding.ASCII.GetBytes("MATLAB 5.0 MAT-file, Platform: .NET, Created by Altaxo\0");
      Array.Copy(desc, header, Math.Min(desc.Length, 116));
      header[124] = 0;
      header[125] = 1; // version 0x0100
      header[126] = (byte)'I';
      header[127] = (byte)'M';
      s.Write(header);
    }

    /// <summary>
    /// Wraps a prepared <c>miMATRIX</c> payload into a top-level matrix element.
    /// </summary>
    private void WriteMatrixElementToFile(Stream matrixPayload)
    {
      var matrixBytes = ((MemoryStream)matrixPayload).ToArray();
      WriteTag(_stream, type: (int)MatDataType.miMATRIX, numBytes: matrixBytes.Length);
      _stream.Write(matrixBytes);
      PadTo8(_stream, matrixBytes.Length);
    }

    /// <summary>
    /// Writes the array name element for an <c>miMATRIX</c> payload.
    /// </summary>
    private static void WriteName(Stream matrix, string name)
    {
      var nameBytes = Encoding.UTF8.GetBytes(name);
      WriteTag(matrix, type: (int)MatDataType.miINT8, numBytes: nameBytes.Length);
      matrix.Write(nameBytes);
      PadTo8(matrix, nameBytes.Length);
    }

    /// <summary>
    /// Writes an 8-byte MAT v5 data tag (type + byte length).
    /// </summary>
    private static void WriteTag(Stream s, int type, int numBytes)
    {
      WriteInt32Raw(s, type);
      WriteInt32Raw(s, numBytes);
    }

    /// <summary>
    /// Writes a 32-bit signed integer (little-endian).
    /// </summary>
    private static void WriteInt32(Stream s, int v) => WriteInt32Raw(s, v);

    /// <summary>
    /// Writes a 32-bit signed integer (little-endian) without additional validation.
    /// </summary>
    private static void WriteInt32Raw(Stream s, int v)
    {
      Span<byte> b = stackalloc byte[4];
      BinaryPrimitives.WriteInt32LittleEndian(b, v);
      s.Write(b);
    }

    /// <summary>
    /// Writes a 32-bit unsigned integer (little-endian).
    /// </summary>
    private static void WriteUInt32(Stream s, uint v)
    {
      Span<byte> b = stackalloc byte[4];
      BinaryPrimitives.WriteUInt32LittleEndian(b, v);
      s.Write(b);
    }

    /// <summary>
    /// Writes a double (little-endian).
    /// </summary>
    private static void WriteDouble(Stream s, double v)
    {
      Span<byte> b = stackalloc byte[8];
      BinaryPrimitives.WriteDoubleLittleEndian(b, v);
      s.Write(b);
    }

    /// <summary>
    /// Pads with zero bytes so that the current element ends on an 8-byte boundary.
    /// </summary>
    private static void PadTo8(Stream s, int elementLength)
    {
      var pad = (8 - (elementLength % 8)) % 8;
      for (int i = 0; i < pad; i++)
        s.WriteByte(0);
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
      /// 64-bit IEEE 754 floating point data.
      /// </summary>
      miDOUBLE = 9,

      /// <summary>
      /// MATLAB matrix element (<c>miMATRIX</c>), used for variables, cells and struct/object fields.
      /// </summary>
      miMATRIX = 14,
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
      /// Double-precision numeric array.
      /// </summary>
      mxDOUBLE_CLASS = 6,
    }
  }
}
