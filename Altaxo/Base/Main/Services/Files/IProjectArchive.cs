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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Abstracts a zip file or a virtual file system where the project is stored into.
  /// </summary>
  public interface IProjectArchive
  {

    /// <summary>
    /// Gets the name of the file, or a describing name in case the project archive is not base on the file system.
    /// </summary>
    /// <value>
    /// The name of the file / describing name of the location.
    /// </value>
    string FileName { get; }

    // Write mode    
    /// <summary>
    /// Creates a new project archive entry.
    /// </summary>
    /// <param name="name">The name of the entry.</param>
    /// <returns>The created entry, in write mode.</returns>
    IProjectArchiveEntry CreateEntry(string name);



    // Read mode    
    /// <summary>
    /// Enumerates all entries in this project archive.
    /// </summary>
    /// <value>
    /// The entries.
    /// </value>
    IEnumerable<IProjectArchiveEntry> Entries { get; }


    /// <summary>
    /// Gets an already existing entry. If an entry with that name does not exist, the value Null is returned.
    /// </summary>
    /// <param name="entryName">Name of the entry.</param>
    /// <returns>The entry (if existent); otherwise, null.</returns>
    IProjectArchiveEntry GetEntry(string entryName);


    bool ContainsEntry(string entryName);

    /// <summary>
    /// Gets a value indicating whether entries in this archive can be opened simultaneously.
    /// </summary>
    /// <value>
    ///   <c>true</c> if entries in this archive can be opened simultaneously; otherwise, <c>false</c>.
    /// </value>
    bool SupportsDeferredLoading { get; }

    /// <summary>
    /// Gets a value indicating whether this type of archive supports the copying of entries from another archive. 
    /// </summary>
    /// <param name="archive">The archive to potentially copy from.</param>
    /// <returns>True if this archive supports the copying of entries from the archive given in the argument; otherwise, false. </returns>
    bool SupportsCopyEntryFrom(IProjectArchive archive);


    /// <summary>
    /// Copies the entry from another archive.
    /// </summary>
    /// <param name="archive">The archive to copy the entry from.</param>
    /// <param name="entryName">The name of the entry to copy.</param>
    void CopyEntryFrom(IProjectArchive archive, string entryName);



    /// <summary>
    /// Gets a memento that can later on used to read an entry of the archive.
    /// </summary>
    /// <param name="entryName">Name of the entry.</param>
    /// <returns></returns>
    IProjectArchiveEntryMemento GetEntryMemento(string entryName);
  }

  /// <summary>
  /// A class from which a project archive can be restored. Ideally, this object should not claim any resources.
  /// </summary>
  public interface IProjectArchiveEntryMemento : IDisposable
  {
    /// <summary>
    /// Gets the archive.
    /// </summary>
    /// <returns>The archive. Usually, after finishing working with the archive, it should be disposed.</returns>
    IProjectArchiveEntry GetArchiveEntry();
  }

  /// <summary>
  /// Abstracts an entry in the <see cref="IProjectArchive"/>
  /// </summary>
  public interface IProjectArchiveEntry
  {
    /// <summary>
    /// Gets the full name of the entry.
    /// </summary>
    /// <value>
    /// The full name.
    /// </value>
    string FullName { get; }

    /// <summary>
    /// Gets a stream for reading from the entry.
    /// </summary>
    /// <returns>Stream intended for reading from the entry.</returns>
    Stream OpenForReading();

    /// <summary>
    /// Gets a stream for writing to the entry.
    /// </summary>
    /// <returns>Stream intended for writing to the entry.</returns>
    Stream OpenForWriting();
  }

}
