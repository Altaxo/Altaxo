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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO.Compression
{
    public struct CentralDirectoryRecord

    {
        public const int MinimumSizeOfStructure = 46;
        public const int MaximumSizeOfStructure = MinimumSizeOfStructure + 65535 + 65535 + 65535;

        public const int CentralDirectoryEntrySignature = 0x02014b50; // Position 0
        public int VersionMadeBy { get; private set; } // Position 4
        public int VersionNeeded { get; private set; } // Position 6
        public int GeneralPurposeFlag { get; private set; } // Position 8
        public int CompressionMethod { get; private set; } // Position 10
        public UInt32 FileLastModificationTime { get; private set; } // Position 12
        public UInt32 Crc { get; private set; } // Position 16

        public long CompressedSize { get; private set; } // Position 20
        public long UncompressedSize { get; private set; } // Position 24
        public int FileNameLength { get; private set; } // Position 28
        public int ExtraFieldLength { get; private set; } // Position 30
        public int FileCommentLength { get; private set; } // Position 32

        public int DiskNumberWhereFileStarts { get; private set; } // Position 34
        public int InternalFileAttributes { get; private set; } // Position 36
        public int ExternalFileAttributes { get; private set; } // Position 38

        public long RelativeOffsetToLocalFileHeader { get; private set; } // Position 42

        public string? FileName { get; private set; } // Position 46

        // public byte[] ExtraField { get; private set; } // Position 46 + FileNameLength

        public string? FileComment { get; private set; } // Position 46 + FileNameLength + ExtraFieldLength

        // Data not part of the structure on disk
        public long OriginalStreamPosition { get; private set; } // Position 20

        public long NextStreamPosition => OriginalStreamPosition + MinimumSizeOfStructure + FileNameLength + ExtraFieldLength + FileCommentLength;

        public int TotalSize => MinimumSizeOfStructure + FileNameLength + ExtraFieldLength + FileCommentLength;

        public static CentralDirectoryRecord FromLocalFileHeader(LocalFileHeader lfh)
        {
            var cde = new CentralDirectoryRecord();
            cde.VersionMadeBy = lfh.VersionNeeded;
            cde.VersionNeeded = lfh.VersionNeeded;
            cde.GeneralPurposeFlag = lfh.GeneralPurposeFlag;
            cde.CompressionMethod = lfh.CompressionMethod;
            cde.FileLastModificationTime = lfh.FileLastModificationTime;
            cde.Crc = lfh.Crc;
            cde.CompressedSize = lfh.CompressedSize;
            cde.UncompressedSize = lfh.UncompressedSize;
            cde.FileNameLength = lfh.FileNameLength;
            cde.ExtraFieldLength = lfh.ExtraFieldLength;
            cde.FileCommentLength = 0;
            cde.DiskNumberWhereFileStarts = 0;
            cde.InternalFileAttributes = 0;
            cde.ExternalFileAttributes = lfh.ExternalFileAttributes;
            cde.RelativeOffsetToLocalFileHeader = lfh.OriginalStreamPosition;
            cde.FileName = lfh.FileName;
            return cde;
        }

        private static ThreadLocal<byte[]> _nameBuffer = new ThreadLocal<byte[]>();

        public static CentralDirectoryRecord Create(Stream str, bool readFileName = false, bool readFileComment = false)
        {
            var buffer = new byte[MinimumSizeOfStructure];
            var originalStreamPosition = str.Position;
            var br = str.Read(buffer, 4, buffer.Length - 4);

            if (br != buffer.Length - 4)
                throw new InvalidDataException($"Unexpected end of stream while reading central directory entry, StreamPos={originalStreamPosition}");

            var result = new CentralDirectoryRecord
            {
                OriginalStreamPosition = originalStreamPosition - 4,
                VersionMadeBy = BitConverter.ToInt16(buffer, 4),
                VersionNeeded = BitConverter.ToInt16(buffer, 6),
                GeneralPurposeFlag = BitConverter.ToInt16(buffer, 8),
                CompressionMethod = BitConverter.ToInt16(buffer, 10),
                FileLastModificationTime = BitConverter.ToUInt32(buffer, 12),
                Crc = BitConverter.ToUInt32(buffer, 16),
                CompressedSize = BitConverter.ToUInt32(buffer, 20),
                UncompressedSize = BitConverter.ToUInt32(buffer, 24),
                FileNameLength = BitConverter.ToUInt16(buffer, 28),
                ExtraFieldLength = BitConverter.ToUInt16(buffer, 30),
                FileCommentLength = BitConverter.ToUInt16(buffer, 32),
                DiskNumberWhereFileStarts = BitConverter.ToUInt16(buffer, 34),
                InternalFileAttributes = BitConverter.ToInt16(buffer, 36),
                ExternalFileAttributes = BitConverter.ToInt32(buffer, 38),
                RelativeOffsetToLocalFileHeader = BitConverter.ToUInt32(buffer, 42),
            };

            if (readFileName)
            {
                if (_nameBuffer.Value is null)
                    _nameBuffer.Value = new byte[65535];

                if (result.FileNameLength != str.Read(_nameBuffer.Value, 0, result.FileNameLength))
                    throw new InvalidOperationException();

                if ((result.GeneralPurposeFlag & 0x0800) != 0) // Read file name as UTF8
                {
                    result.FileName = Encoding.UTF8.GetString(_nameBuffer.Value, 0, result.FileNameLength);
                }
                else
                {
                    result.FileName = CodePagesEncodingProvider.Instance.GetEncoding(437).GetString(_nameBuffer.Value, 0, result.FileNameLength);
                }
            }

            return result;
        }

        /// <summary>
        /// Creates a central directory record from a stream buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset into the buffer where the record starts.</param>
        /// <param name="streamPositionOfBufferStart">The original stream position of the first byte (at index 0) of the entire buffer.</param>
        /// <returns>The central directory record.</returns>
        /// <exception cref="IOException">No central directory signature at stream position {streamPositionOfBufferStart + offset}</exception>
        public static CentralDirectoryRecord Create(byte[] buffer, int offset, long streamPositionOfBufferStart)
        {

            if (BitConverter.ToInt32(buffer, offset + 0) != CentralDirectoryEntrySignature)
                throw new IOException($"No central directory signature at stream position {streamPositionOfBufferStart + offset}");

            var result = new CentralDirectoryRecord
            {
                OriginalStreamPosition = streamPositionOfBufferStart + offset,
                VersionMadeBy = BitConverter.ToInt16(buffer, offset + 4),
                VersionNeeded = BitConverter.ToInt16(buffer, offset + 6),
                GeneralPurposeFlag = BitConverter.ToInt16(buffer, offset + 8),
                CompressionMethod = BitConverter.ToInt16(buffer, offset + 10),
                FileLastModificationTime = BitConverter.ToUInt32(buffer, offset + 12),
                Crc = BitConverter.ToUInt32(buffer, offset + 16),
                CompressedSize = BitConverter.ToUInt32(buffer, offset + 20),
                UncompressedSize = BitConverter.ToUInt32(buffer, offset + 24),
                FileNameLength = BitConverter.ToUInt16(buffer, offset + 28),
                ExtraFieldLength = BitConverter.ToUInt16(buffer, offset + 30),
                FileCommentLength = BitConverter.ToUInt16(buffer, offset + 32),
                DiskNumberWhereFileStarts = BitConverter.ToUInt16(buffer, offset + 34),
                InternalFileAttributes = BitConverter.ToInt16(buffer, offset + 36),
                ExternalFileAttributes = BitConverter.ToInt32(buffer, offset + 38),
                RelativeOffsetToLocalFileHeader = BitConverter.ToUInt32(buffer, offset + 42),
            };

            if ((result.GeneralPurposeFlag & 0x0800) != 0) // Read file name as UTF8
            {
                result.FileName = Encoding.UTF8.GetString(buffer, offset + MinimumSizeOfStructure, result.FileNameLength);
            }
            else
            {
                result.FileName = Encoding.GetEncoding(437).GetString(buffer, offset + MinimumSizeOfStructure, result.FileNameLength);
            }

            return result;
        }

        public void Write(Stream zipArchiveStream)
        {
            var fileName = FileName;
            if (string.IsNullOrEmpty(fileName))
                throw new InvalidOperationException("FileName is not set yet!");

            OriginalStreamPosition = zipArchiveStream.Position;
            var buffer = new byte[MinimumSizeOfStructure + 4 * FileNameLength];
            int bytesWritten = Encoding.UTF8.GetBytes(fileName, 0, fileName.Length, buffer, 46);
            if (bytesWritten != FileNameLength)
                throw new ArgumentException("FileNameLength was wrongly set");




            // now write the bytes to the buffer
            LittleEndianConverter.ToBuffer((UInt32)CentralDirectoryEntrySignature, buffer, 0);
            LittleEndianConverter.ToBuffer((short)VersionMadeBy, buffer, 4);
            LittleEndianConverter.ToBuffer((short)VersionNeeded, buffer, 6);
            LittleEndianConverter.ToBuffer((short)GeneralPurposeFlag, buffer, 8);
            LittleEndianConverter.ToBuffer((short)CompressionMethod, buffer, 10);
            LittleEndianConverter.ToBuffer((UInt32)FileLastModificationTime, buffer, 12);
            LittleEndianConverter.ToBuffer((UInt32)Crc, buffer, 16);
            LittleEndianConverter.ToBuffer((UInt32)CompressedSize, buffer, 20);
            LittleEndianConverter.ToBuffer((UInt32)UncompressedSize, buffer, 24);
            LittleEndianConverter.ToBuffer((UInt16)FileNameLength, buffer, 28);
            LittleEndianConverter.ToBuffer((UInt16)ExtraFieldLength, buffer, 30); // Extra field length
            LittleEndianConverter.ToBuffer((UInt16)FileCommentLength, buffer, 32);
            LittleEndianConverter.ToBuffer((UInt16)DiskNumberWhereFileStarts, buffer, 34);
            LittleEndianConverter.ToBuffer((UInt16)InternalFileAttributes, buffer, 36);
            LittleEndianConverter.ToBuffer((UInt16)ExternalFileAttributes, buffer, 38);
            LittleEndianConverter.ToBuffer((UInt32)RelativeOffsetToLocalFileHeader, buffer, 42);
            // File name is already in buffer
            zipArchiveStream.Write(buffer, 0, MinimumSizeOfStructure + FileNameLength);
        }
    }
}
