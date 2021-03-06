﻿#region Copyright

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
using System.Threading.Tasks;

namespace Altaxo.Main.Services.Files
{
  /// <summary>
  /// Instances of this class can be used in project items to load data delayed (i.e. after the project has been loaded).
  /// The instance store a memento that points to an archive entry that can be used to load the data when needed.
  /// </summary>
  /// <seealso cref="Altaxo.Main.Services.IProjectArchiveEntryMemento" />
  /// <seealso cref="System.IDisposable" />
  public class FileSystemFileAsProjectArchiveMemento : IProjectArchiveEntryMemento, IDisposable
  {
    // fixed data
    private readonly string _entryName;

    // either archiveFileName or archiveManager is not null
    private readonly string? _archiveFolderName;
    private readonly IProjectArchiveManager? _archiveManager;

    // operational data
    private IProjectArchive? _archive;


    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectArchiveEntryMemento"/> class.
    /// </summary>
    /// <param name="entryName">Name of the entry.</param>
    /// <param name="archiveManager">The archive manager.</param>
    /// <param name="archiveFolderName">Name of the archive file. This parameter is used only if the provided <paramref name="archiveManager"/> is null or invalid.</param>
    public FileSystemFileAsProjectArchiveMemento(string entryName, IProjectArchiveManager? archiveManager, string? archiveFolderName)
    {
      if (archiveManager is null && string.IsNullOrEmpty(archiveFolderName))
        throw new ArgumentNullException(nameof(archiveFolderName), $"If {nameof(archiveManager)} is null, then the {nameof(archiveFolderName)} must be provided!");

      _entryName = entryName;
      _archiveManager = archiveManager;
      _archiveFolderName = archiveFolderName;
    }

    /// <inheritdoc/>
    public string EntryName
    {
      get
      {
        return _entryName;
      }
    }

    /// <inheritdoc/>
    public IProjectArchiveEntryMemento WithEntryName(string newName)
    {
      if (string.IsNullOrEmpty(newName))
        throw new ArgumentNullException(nameof(newName));

      if (_entryName == newName)
        return this;

      return new FileSystemFileAsProjectArchiveMemento(newName, _archiveManager, _archiveFolderName);
    }

    /// <inheritdoc/>
    public IProjectArchiveEntryMemento Clone()
    {
      return new FileSystemFileAsProjectArchiveMemento(_entryName, _archiveManager, _archiveFolderName);
    }

    /// <summary>
    /// Gets the archive entry that this memento refers to.
    /// </summary>
    /// <returns>
    /// The archive entry.
    /// </returns>
    public IProjectArchiveEntry GetArchiveEntry()
    {
      if (_archiveManager is not null)
      {
        if (_archiveManager.IsDisposed)
          throw new ObjectDisposedException(nameof(_archiveManager));

        _archive = _archiveManager.GetArchiveReadOnlyThreadSave(this);
        var entry = _archive.GetEntry(_entryName);
        if (entry is null)
          throw new InvalidDataException($"Archive {_archive} seems not to contain entry {_entryName} any more!");
        return entry;
      }
      else if (_archiveFolderName is not null)
      {
        var finalName = Path.Combine(_archiveFolderName, _entryName);
        return new FileSystemFileAsProjectArchiveEntry(_entryName, finalName);
      }
      else
      {
        throw new InvalidProgramException($"Either {nameof(_archiveManager)} or {nameof(_archiveFolderName)} should not be null here!");
      }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
      if (_archiveManager is { } archiveManager && !archiveManager.IsDisposed)
      {
        archiveManager.ReleaseArchiveThreadSave(this, ref _archive);
      }
      else
      {
        _archive?.Dispose();
        _archive = null;
      }
    }
  }
}
