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
  public class ZipArchiveAsProjectArchive : IProjectArchive, IDisposable
  {
    Stream _stream;
    ZipArchiveAxo _zipArchive;
    bool _leaveOpen;

    public string FileName
    {
      get
      {
        if (_stream is FileStream fs)
          return fs.Name;
        else if (null != _stream)
          return "From COM";
        else
          return string.Empty;
      }
    }

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

    public IProjectArchiveEntry GetEntry(string entryName)
    {
      var e = _zipArchive.GetEntry(entryName);
      return e is null ? null : new ZipEntryAsProjectArchiveEntry(e);
    }

    public bool ContainsEntry(string entryName)
    {
      return !(_zipArchive.GetEntry(entryName) is null);
    }

    #region IDisposable Support
    private bool _isDisposed = false; // To detect redundant calls

    protected virtual void Dispose(bool disposing)
    {
      if (!_isDisposed)
      {
        if (disposing)
        {
          _zipArchive?.Dispose();
          _zipArchive = null;
          if (!_leaveOpen)
          {
            _stream?.Dispose();
          }
          _stream = null;
        }
        _isDisposed = true;
      }
    }

    // This code added to correctly implement the disposable pattern.
    public void Dispose()
    {
      // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
      Dispose(true);
      // TODO: uncomment the following line if the finalizer is overridden above.
      // GC.SuppressFinalize(this);
    }

    public IProjectArchiveEntryMemento GetEntryMemento(string entryName)
    {
      if (_stream is FileStream fs)
      {
        return new ZipArchiveEntryMemento(fs.Name, entryName);
      }
      else
      {
        return null;
      }
    }

    public bool SupportsCopyEntryFrom(IProjectArchive archive)
    {
      return archive is ZipArchiveAsProjectArchive;
    }

    public void CopyEntryFrom(IProjectArchive archive, string entryName)
    {
      if (!(archive is ZipArchiveAsProjectArchive zipArchiveWrapper))
        throw new ArgumentOutOfRangeException(nameof(archive), "Has to be a wrapper around a Zip archive");

      var entry = zipArchiveWrapper._zipArchive.GetEntry(entryName);

      if (entry is null)
        throw new ArgumentOutOfRangeException(nameof(entryName), $"Entry name {entryName} was not found in the archive to copy from");

      _zipArchive.CopyEntryFromAnotherArchive(entry);

    }
    #endregion
  }

  /// <summary>
  /// Wraps a <see cref="ZipArchiveEntry"/> to implement <see cref="IProjectArchiveEntry"/>.
  /// </summary>
  /// <seealso cref="Altaxo.Main.Services.Files.IProjectArchiveEntry" />
  public class ZipEntryAsProjectArchiveEntry : IProjectArchiveEntry
  {
    ZipArchiveEntryAxo _entry;

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

  public class ZipArchiveEntryMemento : IProjectArchiveEntryMemento, IDisposable
  {
    string _fileName;
    string _entryName;
    ZipArchiveAxo _archive;


    public ZipArchiveEntryMemento(string archiveFileName, string entryName)
    {
      _fileName = archiveFileName;
      _entryName = entryName;
    }



    public IProjectArchiveEntry GetArchiveEntry()
    {
      var stream = new FileStream(_fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
      _archive = new ZipArchiveAxo(stream, ZipArchiveMode.Read, false);
      return new ZipEntryAsProjectArchiveEntry(_archive.GetEntry(_entryName));
    }

    public void Dispose()
    {
      _archive?.Dispose();
      _archive = null;
    }
  }
}
