#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2019 Dr. Dirk Lellinger
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
using System.IO;
using System.IO.Compression;

namespace Altaxo.Main.Services.Files
{
  /// <summary>
  /// Wraps a <see cref="ZipArchiveEntryAxo"/> to implement <see cref="IProjectArchiveEntry"/>.
  /// </summary>
  /// <seealso cref="IProjectArchiveEntry" />
  public class ZipEntryAsProjectArchiveEntry : IProjectArchiveEntry
  {
    private ZipArchiveEntryAxo _entry;

    /// <summary>
    /// Initializes a new instance of the <see cref="ZipEntryAsProjectArchiveEntry"/> class.
    /// </summary>
    /// <param name="entry">The zip archive entry to wrap.</param>
    /// <exception cref="ArgumentNullException">entry</exception>
    public ZipEntryAsProjectArchiveEntry(ZipArchiveEntryAxo entry)
    {
      _entry = entry ?? throw new ArgumentNullException(nameof(entry));
    }

    /// <inheritdoc/>
    public string FullName { get { return _entry.FullName; } }

    /// <inheritdoc/>
    public Stream OpenForReading()
    {
      return _entry.Open();
    }

    /// <inheritdoc/>
    public Stream OpenForWriting()
    {
      return _entry.Open();
    }
  }
}
