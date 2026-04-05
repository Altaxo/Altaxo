#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

namespace Altaxo.Main
{
  /// <summary>
  /// Provides a writeable stream for writing an item into a compressed file container.
  /// </summary>
  public interface ICompressedFileContainerStream
  {
    /// <summary>
    /// Starts a new file entry in the container.
    /// </summary>
    /// <param name="filename">Entry name.</param>
    /// <param name="level">Compression level.</param>
    void StartFile(string filename, int level);

    //ZipEntry ZipEntry = new ZipEntry(filename);
    //zippedStream.PutNextEntry(ZipEntry);
    //zippedStream.SetLevel(level);

    /// <summary>
    /// Gets the stream for writing the current entry.
    /// </summary>
    System.IO.Stream Stream { get; }
  }

  /// <summary>
  /// Represents an item in a compressed file container.
  /// </summary>
  public interface IFileContainerItem
  {
    /// <summary>
    /// Gets a value indicating whether the item is a directory.
    /// </summary>
    bool IsDirectory { get; }

    /// <summary>
    /// Gets the item name.
    /// </summary>
    string Name { get; }
  }

  /// <summary>
  /// Represents a container providing access to compressed items.
  /// </summary>
  public interface ICompressedFileContainer : System.Collections.IEnumerable
  {
    /// <summary>
    /// Gets an input stream for the specified item.
    /// </summary>
    /// <param name="item">Container item.</param>
    /// <returns>Readable stream.</returns>
    System.IO.Stream GetInputStream(IFileContainerItem item);
  }
}
