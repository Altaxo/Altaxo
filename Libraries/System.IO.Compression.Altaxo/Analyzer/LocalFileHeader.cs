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


        private UInt32? _originalStreamPosition;
        public long OriginalStreamPosition => _originalStreamPosition.Value;

        /// <summary>
        /// Gets a value indicating whether this instance is written to disk (or in read mode: is on disk).
        /// The return value is false if this instance is not already on disk.
        /// </summary>
        public bool IsFileHeaderOnDisk => _originalStreamPosition.HasValue;

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
            get => _fileLastModificationTime;
            set
            {
                if (IsFileHeaderOnDisk)
                    throw new InvalidOperationException("Can not modify time: local file header was already written to disk.");
                _fileLastModificationTime = value;
            }
        }

        public string? FileName
        {
            get => _fileName;
            set
            {
                if (IsFileHeaderOnDisk)
                    throw new InvalidOperationException("Can't modify FileName: local file header was already written to disk.");
                _fileName = value;
            }
        }


        #endregion

        private LocalFileHeader()
        {

        }

        public LocalFileHeader(string fileName)
        {
            _versionNeeded = 0x14;
            _generalPurposeFlag = 0x0800; // Always use UTF8 coding for file name
            _compressionMethod = 0x08; // deflate method
            _fileLastModificationTime = 0;
            _crc = 0;
            _compressedSize = 0;
            _uncompressedSize = 0;
            _fileNameLength = 0;
            _fileName = fileName;
            _extraFieldLength = 0;
            _originalStreamPosition = null;
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
                _versionNeeded = this._versionNeeded,
                _generalPurposeFlag = this._generalPurposeFlag,
                _compressionMethod = this._compressionMethod,
                FileLastModificationTime = this.FileLastModificationTime,
                _crc = this.Crc,
                _compressedSize = this._compressedSize,
                _uncompressedSize = this._uncompressedSize,
                _fileNameLength = this._fileNameLength,
                _extraFieldLength = this._extraFieldLength,
                _fileName = this._fileName,
            };

            return r;
        }

        public static LocalFileHeader Create(Stream str)
        {
            var buffer = new byte[MinimumSizeOfStructure];
            var originalStreamPosition = str.Position;
            var br = str.Read(buffer, 4, buffer.Length - 4);

            if (br != buffer.Length - 4)
                throw new InvalidDataException($"Unexpected end of stream while reading file header, StreamPos={originalStreamPosition}");

            var result = new LocalFileHeader
            {
                _originalStreamPosition = (UInt32)(originalStreamPosition - 4),
                _versionNeeded = BitConverter.ToUInt16(buffer, 4),
                _generalPurposeFlag = BitConverter.ToUInt16(buffer, 6),
                _compressionMethod = BitConverter.ToUInt16(buffer, 8),
                _fileLastModificationTime = BitConverter.ToUInt32(buffer, 10),
                _crc = BitConverter.ToUInt32(buffer, 14),
                _compressedSize = BitConverter.ToUInt32(buffer, 18),
                _uncompressedSize = BitConverter.ToUInt32(buffer, 22),
                _fileNameLength = BitConverter.ToUInt16(buffer, 26),
                _extraFieldLength = BitConverter.ToUInt16(buffer, 28),
                // 30 .. 30 + FileNameLength-1 : FileName
                // 30 + FileNameLength ... 30 + FileNameLength + ExtraFieldLength -1 : ExtraField
                // 30 + FileNameLength + ExtraFieldLength .. 30 + FileNameLength + ExtraFieldLength + CompressedSize-1 : File content
            };

            return result;
        }

        public static LocalFileHeader Create(byte[] buffer, CentralDirectoryRecord cde)
        {
            var originalStreamPosition = cde.RelativeOffsetToLocalFileHeader;

            if (!(BitConverter.ToInt32(buffer, 0) == LocalFileHeaderSignature))
                throw new IOException($"No local file header signature found at stream position {originalStreamPosition}");


            var result = new LocalFileHeader
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

        internal void WriteSizesAndCrc(Stream uncompressedStream, long compressedSize, long uncompressedSize, uint crc)
        {
            _compressedSize = (UInt32)compressedSize;
            _uncompressedSize = (UInt32)uncompressedSize;
            _crc = crc;

            var buffer = new byte[12];
            LittleEndianConverter.ToBuffer(_crc, buffer, 0);
            LittleEndianConverter.ToBuffer(_compressedSize, buffer, 4);
            LittleEndianConverter.ToBuffer(_uncompressedSize, buffer, 8);

            var currentPosition = uncompressedStream.Position;
            uncompressedStream.Seek(OriginalStreamPosition + 14, SeekOrigin.Begin);
            uncompressedStream.Write(buffer, 0, 12);
            uncompressedStream.Position = currentPosition;
        }

        public static LocalFileHeader Write(Stream zipArchiveStream, string fileName, int compressionLevel)
        {
            var lfh = new LocalFileHeader(fileName);
            lfh.Write(zipArchiveStream);
            return lfh;
        }


        public void Write(Stream zipArchiveStream)
        {
            if (string.IsNullOrEmpty(FileName))
                throw new InvalidOperationException("FileName is not set yet!");

            _originalStreamPosition = (UInt32)zipArchiveStream.Position;
            var buffer = new byte[MinimumSizeOfStructure + 4 * FileName.Length];

            int bytesWritten = Encoding.UTF8.GetBytes(FileName, 0, FileName.Length, buffer, 30);
            if (bytesWritten > 65535)
                throw new ArgumentException("File name too long", nameof(FileName));
            _fileNameLength = (UInt16)bytesWritten;

            // now write the bytes to the buffer
            LittleEndianConverter.ToBuffer((UInt32)LocalFileHeaderSignature, buffer, 0);
            LittleEndianConverter.ToBuffer((short)VersionNeeded, buffer, 4);
            LittleEndianConverter.ToBuffer((short)GeneralPurposeFlag, buffer, 6);
            LittleEndianConverter.ToBuffer((short)CompressionMethod, buffer, 8);
            LittleEndianConverter.ToBuffer(FileLastModificationTime, buffer, 10);
            LittleEndianConverter.ToBuffer(Crc, buffer, 14);
            LittleEndianConverter.ToBuffer((UInt32)CompressedSize, buffer, 18);
            LittleEndianConverter.ToBuffer((UInt32)UncompressedSize, buffer, 22);
            LittleEndianConverter.ToBuffer((UInt16)FileNameLength, buffer, 26);
            LittleEndianConverter.ToBuffer((UInt16)ExtraFieldLength, buffer, 28); // Extra field length
            // File name is already in buffer
            zipArchiveStream.Write(buffer, 0, MinimumSizeOfStructure + FileNameLength);
        }


    }
}

