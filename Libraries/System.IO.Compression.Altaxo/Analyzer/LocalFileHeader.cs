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
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.IO.Compression
{
  public class LocalFileHeader
  {
    public const int MinimumSizeOfStructure = LocalFileHeaderStruct.MinimumSizeOfStructure;

    public const int LocalFileHeaderSignature = LocalFileHeaderStruct.MinimumSizeOfStructure;
    private LocalFileHeaderStruct _lfh;


    // Data that are not part of the local file header structure

    public int VersionNeeded { get => _lfh.VersionNeeded; } // Position 4
    public int GeneralPurposeFlag { get => _lfh.GeneralPurposeFlag; } // Position 6
    public int CompressionMethod { get => _lfh.CompressionMethod; } // Position 8
    public UInt32 Crc { get => _lfh.Crc; } // Position 14

    public long CompressedSize => _lfh.CompressedSize;  // Position 18
    public long UncompressedSize => _lfh.UncompressedSize; // Position 22
    public int FileNameLength => _lfh.FileNameLength; // Position 26
    public int ExtraFieldLength => _lfh.ExtraFieldLength; // Position 28

    private long _originalStreamPosition = long.MinValue;
    public long OriginalStreamPosition => _originalStreamPosition;

    /// <summary>
    /// Gets a value indicating whether this instance is written to disk (or in read mode: is on disk).
    /// The return value is false if this instance is not already on disk.
    /// </summary>
    public bool IsFileHeaderOnDisk => _originalStreamPosition >= 0;

    /// <summary>
    /// Temporary storage of external file attributes in order to copy them to the central directory entry.
    /// This attribute is not part of the local file header!
    /// </summary>
    public int ExternalFileAttributes { get; set; }

    public long NextStreamPosition => OriginalStreamPosition + MinimumSizeOfStructure + FileNameLength + ExtraFieldLength + CompressedSize;


    public long StreamPositionOfEndOfLocalFile => OriginalStreamPosition + MinimumSizeOfStructure + FileNameLength + ExtraFieldLength + CompressedSize;

    public long StreamPositionOfContentBegin => OriginalStreamPosition + MinimumSizeOfStructure + FileNameLength + ExtraFieldLength;

    public int SizeOfLocalFileHeader => MinimumSizeOfStructure + FileNameLength + ExtraFieldLength;


    #region Properties

    public UInt32 FileLastModificationTime
    {
      get => _lfh.FileLastModificationTime;
      set
      {
        if (IsFileHeaderOnDisk)
          throw new InvalidOperationException("Can not modify time: local file header was already written to disk.");
        _lfh.FileLastModificationTime = value;
      }
    }

    public string FileName
    {
      get => _lfh.FileName;
      set
      {
        if (IsFileHeaderOnDisk)
          throw new InvalidOperationException("Can't modify FileName: local file header was already written to disk.");
        _lfh.FileName = value;
      }
    }


    #endregion

    private LocalFileHeader()
    {

    }

    public LocalFileHeader(string fileName)
    {
      _lfh = new LocalFileHeaderStruct(fileName);
      _originalStreamPosition = long.MinValue;
      ExternalFileAttributes = 0;
    }

    /// <summary>
    /// Gets the clone of the local file header, but with another original position.
    /// </summary>
    /// <returns></returns>
    public LocalFileHeader GetClone()
    {
      var r = new LocalFileHeader()
      {
        _lfh = this._lfh,
      };

      return r;
    }

    public static LocalFileHeader Create(byte[] buffer, CentralDirectoryRecord cde)
    {
      var originalStreamPosition = cde.RelativeOffsetToLocalFileHeader;

      if (!(BitConverter.ToInt32(buffer, 0) == LocalFileHeaderSignature))
        throw new IOException($"No local file header signature found at stream position {originalStreamPosition}");


      var result = new LocalFileHeader
      {
        _lfh = LocalFileHeaderStruct.Create(buffer, cde),
        _originalStreamPosition = (UInt32)originalStreamPosition,
      };

      if (result.FileNameLength != cde.FileNameLength)
        throw new InvalidOperationException("File name length of local header and central directory entry differ.");

      return result;
    }

    internal void WriteSizesAndCrc(Stream uncompressedStream, long compressedSize, long uncompressedSize, uint crc)
    {
      _lfh.WriteSizesAndCrc(uncompressedStream, _originalStreamPosition, compressedSize, uncompressedSize, crc);
    }

    public static LocalFileHeader Write(Stream zipArchiveStream, string fileName, int compressionLevel)
    {
      var lfh = new LocalFileHeader(fileName);
      lfh.Write(zipArchiveStream);
      return lfh;
    }


    public void Write(Stream zipArchiveStream)
    {
      _originalStreamPosition = zipArchiveStream.Position;
      _lfh.Write(zipArchiveStream);

    }


  }
}

