#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2020 Dr. Dirk Lellinger
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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.IO.Compression
{
  /// <summary>
  /// Represents the EndOfCentralDirectory structure at the very end of the Zip archive.
  /// </summary>
  public class EndOfCentralDirectoryRecord
  {
    /// <summary>
    /// Gets the minimum size, in bytes, of the end of central directory record.
    /// </summary>
    public const int MinimumSizeOfStructure = 22;

    /// <summary>
    /// Gets the signature of the end of central directory record.
    /// </summary>
    public const int EndOfCentralDirectorySignature = 0x06054b50; // Position 0

    /// <summary>
    /// Gets the number of this disk.
    /// </summary>
    public int NumberOfThisDisk { get; private set; } // Position 4

    /// <summary>
    /// Gets the disk number on which the central directory starts.
    /// </summary>
    public int DiskWhereCentralDirectoryStarts { get; private set; } // Position 6

    /// <summary>
    /// Gets the number of central directory records on this disk.
    /// </summary>
    public int NumberOfCentralDirectoryRecordsOnThisDisk { get; private set; } // Position 8

    /// <summary>
    /// Gets the total number of central directory records.
    /// </summary>
    public int TotalNumberOfCentralDirectoryRecords { get; private set; } // Position 10

    /// <summary>
    /// Gets the size, in bytes, of the central directory.
    /// </summary>
    public UInt32 SizeOfCentralDirectoryInBytes { get; private set; } // Position 12

    /// <summary>
    /// Gets the position of the central directory in the archive.
    /// </summary>
    public long PositionOfCentralDirectory { get; private set; } // Position 16

    /// <summary>
    /// Gets the ZIP file comment length.
    /// </summary>
    public int CommentLength { get; private set; } // Position 20


    // Data not part of the structure on disk

    /// <summary>
    /// Gets the original stream position of the record.
    /// </summary>
    public long OriginalStreamPosition { get; private set; }

    /// <summary>
    /// Gets the position immediately after this record in the stream.
    /// </summary>
    public long NextStreamPosition => OriginalStreamPosition + MinimumSizeOfStructure + CommentLength;


    /// <summary>
    /// Creates an end of central directory record from the specified stream.
    /// </summary>
    /// <param name="str">The stream positioned immediately after the signature.</param>
    /// <returns>The created end of central directory record.</returns>
    public static EndOfCentralDirectoryRecord Create(Stream str)
    {
      var buffer = new byte[MinimumSizeOfStructure];
      var originalStreamPosition = str.Position;
      var br = str.Read(buffer, 4, buffer.Length - 4);

      if (br != buffer.Length - 4)
        throw new InvalidDataException($"Unexpected end of stream while reading file header, StreamPos={str.Position}");

      var result = new EndOfCentralDirectoryRecord
      {
        OriginalStreamPosition = originalStreamPosition - 4,
        NumberOfThisDisk = BitConverter.ToInt16(buffer, 4),
        DiskWhereCentralDirectoryStarts = BitConverter.ToInt16(buffer, 6),
        NumberOfCentralDirectoryRecordsOnThisDisk = BitConverter.ToUInt16(buffer, 8),
        TotalNumberOfCentralDirectoryRecords = BitConverter.ToUInt16(buffer, 10),
        SizeOfCentralDirectoryInBytes = BitConverter.ToUInt32(buffer, 12),
        PositionOfCentralDirectory = BitConverter.ToUInt32(buffer, 16),
        CommentLength = BitConverter.ToUInt16(buffer, 20),
      };

      return result;
    }

    /// <summary>
    /// Creates an end of central directory record for writing.
    /// </summary>
    /// <param name="numberOfRecords">The number of central directory records.</param>
    /// <param name="positionOfCentralDirectory">The position of the central directory.</param>
    /// <param name="sizeOfCentralDirectory">The size, in bytes, of the central directory.</param>
    /// <returns>The created end of central directory record.</returns>
    public static EndOfCentralDirectoryRecord Create(int numberOfRecords, long positionOfCentralDirectory, long sizeOfCentralDirectory)
    {
      var r = new EndOfCentralDirectoryRecord();
      r.NumberOfCentralDirectoryRecordsOnThisDisk = numberOfRecords;
      r.TotalNumberOfCentralDirectoryRecords = numberOfRecords;
      r.SizeOfCentralDirectoryInBytes = (uint)sizeOfCentralDirectory;
      r.PositionOfCentralDirectory = (uint)positionOfCentralDirectory;
      return r;
    }

    /// <summary>
    /// Writes the end of central directory record to the specified ZIP archive stream.
    /// </summary>
    /// <param name="zipArchiveStream">The ZIP archive stream to write to.</param>
    public void Write(Stream zipArchiveStream)
    {
      OriginalStreamPosition = zipArchiveStream.Position;

      var buffer = new byte[MinimumSizeOfStructure];

      // now write the bytes to the buffer
      LittleEndianConverter.ToBuffer((UInt32)EndOfCentralDirectorySignature, buffer, 0);
      LittleEndianConverter.ToBuffer((UInt16)NumberOfThisDisk, buffer, 4);
      LittleEndianConverter.ToBuffer((UInt16)DiskWhereCentralDirectoryStarts, buffer, 6);
      LittleEndianConverter.ToBuffer((UInt16)NumberOfCentralDirectoryRecordsOnThisDisk, buffer, 8);
      LittleEndianConverter.ToBuffer((UInt16)TotalNumberOfCentralDirectoryRecords, buffer, 10);
      LittleEndianConverter.ToBuffer((UInt32)SizeOfCentralDirectoryInBytes, buffer, 12);
      LittleEndianConverter.ToBuffer((UInt32)PositionOfCentralDirectory, buffer, 16);
      LittleEndianConverter.ToBuffer((UInt16)CommentLength, buffer, 20);

      zipArchiveStream.Write(buffer, 0, MinimumSizeOfStructure);
    }

    /// <summary>
    /// Tries to locate the end of central directory record in the specified stream.
    /// </summary>
    /// <param name="str">The ZIP archive stream.</param>
    /// <param name="endOfCentralDirectory">When this method returns <c>true</c>, contains the located record; otherwise, <see langword="null"/>.</param>
    /// <returns><c>true</c> if the record was found; otherwise, <c>false</c>.</returns>
    public static bool TryGetEndOfCentralDirectory(Stream str, [MaybeNullWhen(false)] out EndOfCentralDirectoryRecord endOfCentralDirectory)
    {
      var length = str.Length;

      // Test if central directory is directly at the end (without comment)
      str.Seek(length - MinimumSizeOfStructure, SeekOrigin.Begin);
      if (ReadInt32(str) == EndOfCentralDirectorySignature)
      {
        endOfCentralDirectory = EndOfCentralDirectoryRecord.Create(str);
        return true;
      }
      else
      {
        int maxBufSize = (int)Math.Min(length, 65535 + MinimumSizeOfStructure); // Max Distance of stream end from begin of EndOfCentralDirectory
        var buffer = new byte[maxBufSize];
        var offs = length - maxBufSize;
        str.Seek(offs, SeekOrigin.Begin);
        if (!(maxBufSize == str.Read(buffer, 0, buffer.Length)))
          throw new InvalidOperationException();

        // now try to find the EOCD signature backwards
        for (int i = buffer.Length - 4; i >= 0; --i)
        {
          if (buffer[i] == 0x50 && buffer[i + 1] == 0x4b && buffer[i + 2] == 0x05 && buffer[i + 3] == 0x06)
          {
            str.Seek(offs + i + 4, SeekOrigin.Begin);
            endOfCentralDirectory = EndOfCentralDirectoryRecord.Create(str);
            return true;
          }
        }

        endOfCentralDirectory = null;
        return false;
      }
    }

    private static int ReadInt32(Stream str)
    {
      var buffer = new byte[4];
      var rd = str.Read(buffer, 0, 4);
      if (rd == 4)
        return BitConverter.ToInt32(buffer, 0);
      else
        throw new InvalidOperationException($"Unexpected end of file at position {str.Position}");
    }


  }
}
