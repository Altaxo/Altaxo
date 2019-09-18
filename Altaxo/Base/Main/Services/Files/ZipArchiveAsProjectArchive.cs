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
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Main.Services.Files
{
  /// <summary>
  /// Wraps a <see cref="ZipArchive"/> to implement <see cref="IProjectArchive"/>
  /// </summary>
  /// <seealso cref="IProjectArchive" />
  public class ZipArchiveAsProjectArchive : IProjectArchive
  {
    private bool _isDisposed;
    Stream _stream;
    ZipArchiveAxo _zipArchive;
    bool _leaveOpen;

    /// <inheritdoc/>
    public PathName FileName
    {
      get
      {
        if (_stream is FileStream fs)
          return new FileName(fs.Name);
        else
          return null;
      }
    }

    /// <inheritdoc/>
    public bool IsDisposed => _isDisposed;

    /// <inheritdoc/>
    public IProjectArchiveManager ArchiveManager { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ZipArchiveAsProjectArchive"/> class.
    /// </summary>
    /// <param name="stream">The archive stream.</param>
    /// <param name="leaveOpen">If true, the stream will be left open, even if this instance is disposed.</param>
    /// <exception cref="ArgumentNullException">zipArchive</exception>
    public ZipArchiveAsProjectArchive(Stream stream, ZipArchiveMode mode, bool leaveOpen)
    {
      _stream = stream ?? throw new ArgumentNullException(nameof(stream));
      _zipArchive = new ZipArchiveAxo(stream, mode, leaveOpen);
      _leaveOpen = leaveOpen;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ZipArchiveAsProjectArchive"/> class.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="mode">The mode used to open the project archive.</param>
    /// <exception cref="ArgumentException">mode</exception>
    public ZipArchiveAsProjectArchive(string fileName, ZipArchiveMode mode)
    {
      if (mode == ZipArchiveMode.Read)
        _stream = new System.IO.FileStream(fileName, System.IO.FileMode.Open, FileAccess.Read, FileShare.Read);
      else if (mode == ZipArchiveMode.Create)
        _stream = new System.IO.FileStream(fileName, System.IO.FileMode.Create, FileAccess.Write, FileShare.None);
      else if (mode == ZipArchiveMode.Update)
        _stream = new System.IO.FileStream(fileName, System.IO.FileMode.Open, FileAccess.Write, FileShare.None);
      else
        throw new ArgumentException(nameof(mode));

      _leaveOpen = false;
      _zipArchive = new ZipArchiveAxo(_stream, mode, _leaveOpen);
    }

    /// <inheritdoc/>
    public bool SupportsDeferredLoading
    {
      get
      {
        return _stream is FileStream;
      }
    }

    /// <inheritdoc/>
    public IEnumerable<IProjectArchiveEntry> Entries => _zipArchive.Entries.Select(entry => new ZipEntryAsProjectArchiveEntry(entry));

    /// <inheritdoc/>
    public IProjectArchiveEntry CreateEntry(string name)
    {
      return new ZipEntryAsProjectArchiveEntry(_zipArchive.CreateEntry(name));
    }

    /// <inheritdoc/>
    public IProjectArchiveEntry GetEntry(string entryName)
    {
      var e = _zipArchive.GetEntry(entryName);
      return e is null ? null : new ZipEntryAsProjectArchiveEntry(e);
    }

    /// <inheritdoc/>
    public bool ContainsEntry(string entryName)
    {
      return !(_zipArchive.GetEntry(entryName) is null);
    }

    #region IDisposable Support

    /// <inheritdoc/>
    public void Dispose()
    {
      if (!_isDisposed)
      {
        _zipArchive?.Dispose(); // dispose the zip archive __before__ flushing the stream!
        _zipArchive = null;
        if (!_leaveOpen)
        {
          _stream?.Close();
          _stream?.Dispose();
        }
        else
        {
          _stream?.Flush();
        }
        _stream = null;
      }
      _isDisposed = true;
    }



    /// <inheritdoc/>
    public IProjectArchiveEntryMemento GetEntryMemento(string entryName)
    {
      if (_stream is FileStream fs)
      {
        return new ProjectArchiveEntryMemento(entryName, ArchiveManager, fs.Name);
      }
      else
      {
        return null;
      }
    }

    /// <inheritdoc/>
    public bool SupportsCopyEntryFrom(IProjectArchive archive)
    {
      return archive is ZipArchiveAsProjectArchive;
    }

    /// <inheritdoc/>
    public void CopyEntryFrom(IProjectArchive sourceArchive, string sourceEntryName, string destinationEntryName)
    {
      if (!(sourceArchive is ZipArchiveAsProjectArchive zipArchiveWrapper))
        throw new ArgumentOutOfRangeException(nameof(sourceArchive), "Has to be a wrapper around a Zip archive");

      var entry = zipArchiveWrapper._zipArchive.GetEntry(sourceEntryName);

      if (entry is null)
        throw new ArgumentOutOfRangeException(nameof(sourceEntryName), $"Entry name {sourceEntryName} was not found in the archive to copy from");

      _zipArchive.CopyEntryFromAnotherArchive(entry, destinationEntryName: destinationEntryName);

    }
    #endregion
  }
}
