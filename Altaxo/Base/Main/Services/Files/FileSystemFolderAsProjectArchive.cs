#region Copyright

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
  public class FileSystemFolderAsProjectArchive : IProjectArchive
  {
    private string _baseFolder;
    private FileInfo[] _files;

    /// <summary>
    /// The entry dictionary. Key is the file name relative to the base directory.
    /// </summary>
    private HashSet<string> _entryHash;

    public PathName? FileName => new DirectoryName(_baseFolder);

    public FileSystemFolderAsProjectArchive(DirectoryName folderName)
    {
      if (folderName is null)
        throw new ArgumentNullException(nameof(folderName));

      var dir = new DirectoryInfo(folderName);

      _baseFolder = dir.FullName;
      if (_baseFolder[_baseFolder.Length - 1] != Path.DirectorySeparatorChar)
        _baseFolder += Path.DirectorySeparatorChar;

      _files = dir.GetFiles("*.*", SearchOption.AllDirectories);

      _entryHash = new HashSet<string>();

      foreach (var file in _files)
      {
        var name = file.FullName.Substring(_baseFolder.Length);
        _entryHash.Add(name);
      }
    }

    public IEnumerable<IProjectArchiveEntry> Entries
    {
      get
      {
        foreach (var name in _entryHash)
        {
          yield return new FileSystemFileAsProjectArchiveEntry(_baseFolder, name);
        }
      }
    }

    public bool SupportsDeferredLoading => true;

    public bool ContainsEntry(string entryName)
    {
      return _entryHash.Contains(entryName);
    }

    public void CopyEntryFrom(IProjectArchive sourceArchive, string sourceEntryName, string destinationEntryName)
    {
      var destEntry = CreateEntry(destinationEntryName);
      var sourceEntry = sourceArchive.GetEntry(sourceEntryName);

      using var srcStream = sourceEntry.OpenForReading();
      using var destStream = destEntry.OpenForWriting();
      srcStream.CopyTo(destStream);
    }

    public IProjectArchiveEntry CreateEntry(string name)
    {
      name = name?.Trim();
      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException();

      if (_entryHash.Contains(name))
        throw new InvalidOperationException($"Entry '{name}' is already present in this project archive.");

      _entryHash.Add(name);
      return new FileSystemFileAsProjectArchiveEntry(_baseFolder, name);
    }

    public void Dispose()
    {
      // Nothing to dispose here!
    }

    public IProjectArchiveEntry? GetEntry(string entryName)
    {
      if (_entryHash.Contains(entryName))
        return new FileSystemFileAsProjectArchiveEntry(_baseFolder, entryName);
      else
        return null;
    }

    public IProjectArchiveEntryMemento? GetEntryMemento(string entryName)
    {
      throw new NotImplementedException();
    }

    public bool SupportsCopyEntryFrom(IProjectArchive archive)
    {
      return archive is not null;
    }
  }
}
