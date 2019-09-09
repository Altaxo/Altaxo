using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.IO.Compression
{
    public partial class ZipArchiveEntryAxo
    {

        /// <summary>
        /// Opens the compressed stream in read parallel mode
        /// </summary>
        /// <returns></returns>
        public Stream OpenCompressedStream()
        {
            ThrowIfNotOpenable(needToUncompress: false, needToLoadIntoMemory: false);

            return new SubReadStream(_archive.ArchiveStream, OffsetOfCompressedData, _compressedSize, isStreamOwner: false);
        }


        internal void CopyDataFromAnotherArchive(ZipArchiveAxo archive, ZipArchiveEntryAxo entryFromAnotherArchive)
        {


            if (entryFromAnotherArchive._compressedSize == 0)
            {
                this.WriteLocalFileHeader(isEmptyFile: true);
            }
            else
            {


                this._compressionLevel = entryFromAnotherArchive._compressionLevel;
                this.CompressionMethod = entryFromAnotherArchive.CompressionMethod;
                this._versionToExtract = entryFromAnotherArchive._versionToExtract;
                this._generalPurposeBitFlag = entryFromAnotherArchive._generalPurposeBitFlag;
                this._lastModified = entryFromAnotherArchive._lastModified;

                var _usedZip64inLH = WriteLocalFileHeader(isEmptyFile: false);



                using (var rdStream = entryFromAnotherArchive.OpenCompressedStream())
                {
                    var buffer = new byte[4096];

                    int rd;

                    while (0 != (rd = rdStream.Read(buffer, 0, buffer.Length)))
                    {
                        _archive.ArchiveStream.Write(buffer, 0, rd);
                    }
                }

                this._uncompressedSize = entryFromAnotherArchive._uncompressedSize;
                this._compressedSize = entryFromAnotherArchive._compressedSize;
                this._crc32 = entryFromAnotherArchive._crc32;


                // go back and finish writing
                if (_archive.ArchiveStream.CanSeek)
                    // finish writing local header if we have seek capabilities
                    WriteCrcAndSizesInLocalHeader(_usedZip64inLH);
                else
                    // write out data descriptor if we don't have seek capabilities
                    WriteDataDescriptor();
            }

            _everOpenedForWrite = true;
            _archive.ReleaseArchiveStream(this);
            _outstandingWriteStream = null;
        }

        /// <summary>
        /// Determines whether this instance can open a stream in parallel read mode.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this instance can open a stream in parallel read mode; otherwise, <c>false</c>.
        /// </returns>
        public bool CanOpenInReadModeParallel()
        {
            return
                IsOpenable(false, false, out var _) &&
                _archive.ArchiveStream is FileStream archiveFileStream &&
                archiveFileStream.CanWrite == false;
        }

        /// <summary>
        /// Opens the compressed stream in read parallel mode
        /// </summary>
        /// <returns></returns>
        public Stream OpenCompressedStreamInReadModeParallel()
        {
            ThrowIfNotOpenable(needToUncompress: false, needToLoadIntoMemory: false);

            if (_archive.ArchiveStream is FileStream archiveFileStream)
            {
                // Make a clone of the archive stream - do not use it directly because that's not thread safe!
                var newArchiveFileStream = new FileStream(archiveFileStream.Name, FileMode.Open, FileAccess.Read, FileShare.Read);
                var compressedStream = new SubReadStream(newArchiveFileStream, OffsetOfCompressedData, _compressedSize, isStreamOwner: true);
                return compressedStream;
            }
            else
            {
                throw new InvalidOperationException($"Calling {nameof(OpenCompressedStreamInReadModeParallel)} is only allowed for file based zip archives in read mode");
            }
        }

        /// <summary>
        /// Opens the (uncompressed) stream in read parallel mode
        /// </summary>
        /// <returns></returns>
        public Stream OpenStreamInReadModeParallel()
        {
            ThrowIfNotOpenable(needToUncompress: true, needToLoadIntoMemory: false);


            if (_archive.ArchiveStream is FileStream archiveFileStream)
            {
                if (!archiveFileStream.CanWrite)
                {
                    // Make a clone of the archive stream - do not use it directly because that's not thread safe!
                    var newArchiveFileStream = new FileStream(archiveFileStream.Name, FileMode.Open, FileAccess.Read, FileShare.Read);
                    var compressedStream = new SubReadStream(newArchiveFileStream, OffsetOfCompressedData, _compressedSize, isStreamOwner: true);
                    return GetDataDecompressor(compressedStream);
                }
                else
                {
                    throw new InvalidOperationException($"Calling {nameof(OpenStreamInReadModeParallel)} is only allowed for archives opened with a read-only stream");
                }
            }
            else
            {


                throw new InvalidOperationException($"Calling {nameof(OpenStreamInReadModeParallel)} is only allowed for file based zip archives");
            }
        }



    }
}
