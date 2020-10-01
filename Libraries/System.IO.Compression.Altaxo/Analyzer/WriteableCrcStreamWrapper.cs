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
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.IO.Compression
{
    /// <summary>
    /// Represents a writable stream that compresses and then writes directly to the Zip archive.
    /// </summary>
    /// <seealso cref="System.IO.Stream" />
    public class WriteableCrcStreamWrapper : Stream
    {
        private LocalFileHeader _localFileHeader;
        private Stream _zipArchiveStream;
        private Stream _deflateStream;
        private UInt32 _crc;
        public bool IsClosedDisposed { get; private set; }


        /// <summary>
        /// Start of the (compressed content) in the Zip archive stream.
        /// </summary>
        private long _originalPosition;

        /// <summary>
        /// The number of (uncompressed) bytes written to this wrapper stream.
        /// </summary>
        private long _bytesWritten;

        /// <summary>
        /// Initializes a new instance of the <see cref="WriteableCrcStreamWrapper"/> class.
        /// </summary>
        /// <param name="uncompressedStream">The stream of the Zip archive. This stream
        /// must already being positioned immediately after a local file header.</param>
        /// <param name="localFileHeader">The local file header this stream is positioned after.</param>
        public WriteableCrcStreamWrapper(Stream uncompressedStream, LocalFileHeader localFileHeader)
        {
            if (!localFileHeader.IsFileHeaderOnDisk)
                throw new InvalidOperationException("Local file header is not written to disk");

            _zipArchiveStream = uncompressedStream;
            _originalPosition = uncompressedStream.Position;
            _deflateStream = new DeflateStream(uncompressedStream, System.IO.Compression.CompressionMode.Compress, leaveOpen: true);
            _localFileHeader = localFileHeader;
        }

        public override bool CanRead => false;

        public override bool CanSeek => false;

        public override bool CanWrite => _deflateStream.CanWrite;

        public override long Length => _deflateStream.Length;

        public override long Position
        {
            get => _bytesWritten;
            set => throw new NotSupportedException();
        }

        public override void Flush()
        {
            _deflateStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (IsClosedDisposed)
                throw new IOException("Stream is closed");

            _crc = Crc32.UpdateChecksum(_crc, buffer, offset, count);
            _deflateStream.Write(buffer, offset, count);
            _bytesWritten += count;
        }

        public override void Close()
        {
            if (IsClosedDisposed)
                return;
            IsClosedDisposed = true;
            _deflateStream.Close();
            _localFileHeader.WriteSizesAndCrc(_zipArchiveStream, _zipArchiveStream.Position - _originalPosition, _bytesWritten, _crc);
            _deflateStream = Stream.Null;
            _zipArchiveStream = Stream.Null;
        }

        protected override void Dispose(bool disposing)
        {
            Close();
        }
    }
}
