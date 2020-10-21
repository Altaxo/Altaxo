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
        name = name.Replace('\\', '/'); // we try to name the entries exactly equal to the entries in the Zip file
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

    /// <summary>
    /// Returns false. See remarks why this is so.
    /// </summary>
    /// <remarks>
    /// We can not support deferred loading here, because the class <see cref="FileSystemFolderProjectArchiveManager"/> currently can
    /// not save copy the entire project folder to another location. But this is required because of the following scenario:
    /// - Altaxo loads a project from a folder, the table data are not deserialized if not needed
    /// - Before Altaxo saves the project again into the same folder, it must delete all files in it
    /// - During saving, the memento tries to copy the data from the old location to the new location
    /// - Because the old file was deleted prior to saving, the memento will not find its data => ERROR
    /// </remarks>
    public bool SupportsDeferredLoading => false;

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
      throw new NotSupportedException(); // see property SupportsDeferredLoading why this is not supported yet.
      // return new FileSystemFileAsProjectArchiveMemento(entryName, null, _baseFolder);
    }

    public bool SupportsCopyEntryFrom(IProjectArchive archive)
    {
      return archive is not FileSystemFolderAsProjectArchive ||
            (archive is FileSystemFolderAsProjectArchive arch && arch._baseFolder != this._baseFolder);
    }
  }
}
