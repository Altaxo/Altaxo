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
  /// Instances of this class can be used in project items to load data delayed (i.e. after the project has been loaded).
  /// The instance store a memento that points to an archive entry that can be used to load the data when needed.
  /// </summary>
  /// <seealso cref="Altaxo.Main.Services.IProjectArchiveEntryMemento" />
  /// <seealso cref="System.IDisposable" />
  public class ProjectArchiveEntryMemento : IProjectArchiveEntryMemento, IDisposable
  {
    // fixed data
    private readonly string _entryName;
    private readonly string? _fileName;
    private readonly IProjectArchiveManager? _archiveManager;

    // operational data
    private IProjectArchive? _archive;


    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectArchiveEntryMemento"/> class.
    /// </summary>
    /// <param name="entryName">Name of the entry.</param>
    /// <param name="archiveManager">The archive manager.</param>
    /// <param name="archiveFileName">Name of the archive file. This parameter is used only if the provided <paramref name="archiveManager"/> is null or invalid.</param>
    public ProjectArchiveEntryMemento(string entryName, IProjectArchiveManager? archiveManager, string? archiveFileName)
    {
      if (archiveManager is null && string.IsNullOrEmpty(archiveFileName))
        throw new ArgumentNullException(nameof(archiveFileName), $"If {nameof(archiveManager)} is null, then the {nameof(archiveFileName)} must be provided!");

      _entryName = entryName;
      _archiveManager = archiveManager;
      _fileName = archiveFileName;


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

      return new ProjectArchiveEntryMemento(newName, _archiveManager, _fileName);
    }

    /// <inheritdoc/>
    public IProjectArchiveEntryMemento Clone()
    {
      return new ProjectArchiveEntryMemento(_entryName, _archiveManager, _fileName);
    }

    /// <summary>
    /// Gets the archive entry that this memento refers to.
    /// </summary>
    /// <returns>
    /// The archive entry.
    /// </returns>
    public IProjectArchiveEntry? GetArchiveEntry()
    {
      if (!(_archiveManager is null))
      {
        if (_archiveManager.IsDisposed)
          throw new ObjectDisposedException(nameof(_archiveManager));

        _archive = _archiveManager.GetArchiveReadOnlyThreadSave(this);
        return _archive.GetEntry(_entryName);
      }
      else if (!(_fileName is null))
      {
        var stream = new FileStream(_fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        _archive = new ZipArchiveAsProjectArchive(stream, ZipArchiveMode.Read, false);
        return _archive.GetEntry(_entryName);
      }
      else
      {
        throw new InvalidProgramException($"Either {nameof(_archiveManager)} or {nameof(_fileName)} should not be null here!");
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
