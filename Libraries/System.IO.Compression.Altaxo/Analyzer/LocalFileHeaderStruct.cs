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
    /// <summary>
    /// Local file header of a zip archive, as a struct.
    /// </summary>
    public struct LocalFileHeaderStruct
    {
        public const int MinimumSizeOfStructure = 30;

        public const int LocalFileHeaderSignature = 0x04034b50; // Position 0

        private UInt16 _versionNeeded; // Position 4

        private UInt16 _generalPurposeFlag; // Position 6

        private UInt16 _compressionMethod; // Position 8

        private UInt32 _fileLastModificationTime; // Position 10

        private UInt32 _crc; // Position 14

        private UInt32 _compressedSize; // Position 18

        private UInt32 _uncompressedSize; // Position 22

        private UInt16 _fileNameLength; // Position 26

        private UInt16 _extraFieldLength; // Position 28

        private string? _fileName; // Position 30




        // Data that are not part of the local file header structure

        public int VersionNeeded { get => _versionNeeded; } // Position 4
        public int GeneralPurposeFlag { get => _generalPurposeFlag; } // Position 6
        public int CompressionMethod { get => _compressionMethod; } // Position 8
        public UInt32 Crc { get => _crc; } // Position 14

        public long CompressedSize => _compressedSize;  // Position 18
        public long UncompressedSize => _uncompressedSize; // Position 22
        public int FileNameLength => _fileNameLength; // Position 26
        public int ExtraFieldLength => _extraFieldLength; // Position 28


        private UInt32 _originalStreamPosition;
        public long OriginalStreamPosition => _originalStreamPosition;

        /// <summary>
        /// Gets a value indicating whether this instance is written to disk (or in read mode: is on disk).
        /// The return value is false if this instance is not already on disk.
        /// </summary>
        public bool IsFileHeaderOnDisk => _originalStreamPosition >= 0;



        public long NextStreamPosition => OriginalStreamPosition + MinimumSizeOfStructure + FileNameLength + ExtraFieldLength + CompressedSize;


        public long StreamPositionOfEndOfLocalFile => OriginalStreamPosition + MinimumSizeOfStructure + FileNameLength + ExtraFieldLength + CompressedSize;

        public long StreamPositionOfContentBegin => OriginalStreamPosition + MinimumSizeOfStructure + FileNameLength + ExtraFieldLength;

        public int SizeOfLocalFileHeader => MinimumSizeOfStructure + FileNameLength + ExtraFieldLength;


        #region Properties

        public UInt32 FileLastModificationTime => _fileLastModificationTime;

        public string FileName => _fileName;


        #endregion

        public static LocalFileHeaderStruct Create(byte[] buffer, CentralDirectoryRecord cde)
        {
            var originalStreamPosition = cde.RelativeOffsetToLocalFileHeader;

            if (!(BitConverter.ToInt32(buffer, 0) == LocalFileHeaderSignature))
                throw new IOException($"No local file header signature found at stream position {originalStreamPosition}");


            var result = new LocalFileHeaderStruct
            {
                _originalStreamPosition = (UInt32)originalStreamPosition,
                _versionNeeded = BitConverter.ToUInt16(buffer, 4),
                _generalPurposeFlag = BitConverter.ToUInt16(buffer, 6),
                _compressionMethod = BitConverter.ToUInt16(buffer, 8),
                _fileLastModificationTime = BitConverter.ToUInt32(buffer, 10),
                _crc = BitConverter.ToUInt32(buffer, 14),
                _compressedSize = BitConverter.ToUInt32(buffer, 18),
                _uncompressedSize = BitConverter.ToUInt32(buffer, 22),
                _fileNameLength = BitConverter.ToUInt16(buffer, 26),
                _extraFieldLength = BitConverter.ToUInt16(buffer, 28),
                _fileName = cde.FileName, // 30 .. 30 + FileNameLength-1 : FileName
                // 30 + FileNameLength ... 30 + FileNameLength + ExtraFieldLength -1 : ExtraField
                // 30 + FileNameLength + ExtraFieldLength .. 30 + FileNameLength + ExtraFieldLength + CompressedSize-1 : File content
            };

            if (result.FileNameLength != cde.FileNameLength)
                throw new InvalidOperationException("File name length of local header and central directory entry differ.");

            return result;
        }
    }
}

