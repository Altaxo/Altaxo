using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.IO.Compression
{
    public partial class ZipArchiveAxo
    {
        public ZipArchiveEntryAxo CopyEntryFromAnotherArchive(ZipArchiveEntryAxo entryFromAnotherArchive)
        {
            if (entryFromAnotherArchive == null)
                throw new ArgumentNullException(nameof(entryFromAnotherArchive));

            if (string.IsNullOrEmpty(entryFromAnotherArchive.Name))
                throw new ArgumentException(SR.CannotBeEmpty, nameof(entryFromAnotherArchive.Name));

            if (_mode == ZipArchiveMode.Read)
                throw new NotSupportedException(SR.CreateInReadMode);

            ThrowIfDisposed();

            if (object.ReferenceEquals(entryFromAnotherArchive.Archive, this))
                throw new ArgumentException("Entry has to be from another archive", nameof(entryFromAnotherArchive));

            // copy entries metadata, including crc, compressed and uncompressed sizes
            ZipArchiveEntryAxo entry = new ZipArchiveEntryAxo(this, entryFromAnotherArchive.FullName);

            AddEntry(entry);

            // copy the compressed data 1:1 from the other archive
            entry.CopyDataFromAnotherArchive(this, entryFromAnotherArchive);

            return entry;
        }

    }
}
