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
  /// Wraps a <see cref="ZipArchive"/> to implement <see cref="IProjectArchive"/>.
  /// With this class, the build-in Zip-Routines of the framework are used. Thus,
  /// this type of archive does not support deferred loading.
  /// </summary>
  /// <seealso cref="IProjectArchive" />
  public class ZipArchiveAsProjectArchiveNative : IProjectArchive
  {
    private bool _isDisposed;
    private Stream _stream;
    private ZipArchive _zipArchive;
    private bool _leaveOpen;

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
    /// <param name="mode">The mode in which to create the Zip archive.</param>
    /// <param name="leaveOpen">If true, the stream will be left open, even if this instance is disposed.</param>
    /// <exception cref="ArgumentNullException">zipArchive</exception>
    public ZipArchiveAsProjectArchiveNative(Stream stream, ZipArchiveMode mode, bool leaveOpen)
      : this(stream, mode, leaveOpen, null)
    {
      _stream = stream ?? throw new ArgumentNullException(nameof(stream));
      _zipArchive = new ZipArchive(stream, mode, leaveOpen);
      _leaveOpen = leaveOpen;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ZipArchiveAsProjectArchive"/> class.
    /// </summary>
    /// <param name="stream">The archive stream.</param>
    /// <param name="mode">The mode in which to create the Zip archive.</param>
    /// <param name="leaveOpen">If true, the stream will be left open, even if this instance is disposed.</param>
    /// <param name="archiveManager">The archive manager managing this archive.</param>
    /// <exception cref="ArgumentNullException">zipArchive</exception>
    public ZipArchiveAsProjectArchiveNative(Stream stream, ZipArchiveMode mode, bool leaveOpen, IProjectArchiveManager archiveManager)
    {
      _stream = stream ?? throw new ArgumentNullException(nameof(stream));
      _zipArchive = new ZipArchive(stream, mode, leaveOpen);
      _leaveOpen = leaveOpen;
      ArchiveManager = archiveManager;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ZipArchiveAsProjectArchive"/> class.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="mode">The mode used to open the project archive.</param>
    /// <exception cref="ArgumentException">mode</exception>
    public ZipArchiveAsProjectArchiveNative(string fileName, ZipArchiveMode mode)
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
      _zipArchive = new ZipArchive(_stream, mode, _leaveOpen);
    }

    /// <summary>
    /// Gets a value indicating whether entries in this archive can be opened simultaneously.
    /// For this instance, the return value is always false.
    /// </summary>
    /// <value>
    ///   <c>False</c> (this type of archive does not support deferred loading).</c>.
    /// </value>
    public bool SupportsDeferredLoading
    {
      get
      {
        return false;
      }
    }

    /// <inheritdoc/>
    public IEnumerable<IProjectArchiveEntry> Entries => _zipArchive.Entries.Select(entry => new ZipEntryAsProjectArchiveEntryNative(entry));

    /// <inheritdoc/>
    public IProjectArchiveEntry CreateEntry(string name)
    {
      return new ZipEntryAsProjectArchiveEntryNative(_zipArchive.CreateEntry(name));
    }

    /// <inheritdoc/>
    public IProjectArchiveEntry GetEntry(string entryName)
    {
      var e = _zipArchive.GetEntry(entryName);
      return e is null ? null : new ZipEntryAsProjectArchiveEntryNative(e);
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
      return false;
    }

    /// <inheritdoc/>
    public void CopyEntryFrom(IProjectArchive sourceArchive, string sourceEntryName, string destinationEntryName)
    {
      throw new NotSupportedException("This archive does not support 'CopyEntryFrom'");
    }
    #endregion
  }
}
