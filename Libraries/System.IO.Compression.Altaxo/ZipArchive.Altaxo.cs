using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.IO.Compression
{
  public partial class ZipArchiveAxo
  {
    /// <summary>
    /// Copies an entry from another archive to this archive, using the entry name from the old archive.
    /// The compression level of the entry is maintained.
    /// </summary>
    /// <param name="entryFromAnotherArchive">The entry to copy from another archive.</param>
    /// <returns>The copied zip entry in this archive.</returns>
    public ZipArchiveEntryAxo CopyEntryFromAnotherArchive(ZipArchiveEntryAxo entryFromAnotherArchive)
    {
      return CopyEntryFromAnotherArchive(entryFromAnotherArchive, entryFromAnotherArchive.FullName);
    }
    /// <summary>
    /// Copies an entry from another archive to this archive. The compression level of the entry is maintained.
    /// </summary>
    /// <param name="entryFromAnotherArchive">The entry to copy from another archive.</param>
    /// <param name="destinationEntryName">Name of entry in this archive.</param>
    /// <returns>The copied zip entry in this archive.</returns>
    public ZipArchiveEntryAxo CopyEntryFromAnotherArchive(ZipArchiveEntryAxo entryFromAnotherArchive, string destinationEntryName)
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
      ZipArchiveEntryAxo entry = new ZipArchiveEntryAxo(this, destinationEntryName);

      AddEntry(entry);

      // copy the compressed data 1:1 from the other archive
      entry.CopyDataFromAnotherArchive(this, entryFromAnotherArchive);

      return entry;
    }

  }
}
