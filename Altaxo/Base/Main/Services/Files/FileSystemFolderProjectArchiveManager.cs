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
  public class FileSystemFolderProjectArchiveManager : IFolderBasedProjectArchiveManager
  {
    private DirectoryName _folderName;
    private bool _isDisposed;

    public PathName? FileOrFolderName => _folderName;

    public bool IsDisposed => _isDisposed;

    public event EventHandler<NameChangedEventArgs>? FileOrFolderNameChanged;

    public void Dispose()
    {
      _isDisposed = true;
    }


    /// <summary>
    /// Loads a project from a folder. The files in this folder represent the project items.
    /// </summary>
    /// <param name="folderName">Name of the folder.</param>
    /// <param name="restoreProjectAndWindowsState">Delegate that is used to deserialize and restore the project and the windows state.</param>
    public void LoadFromFolder(DirectoryName folderName, RestoreProjectAndWindowsState restoreProjectAndWindowsState)
    {
      var oldName = _folderName;
      _folderName = folderName;
      using (var archive = new FileSystemFolderAsProjectArchive(folderName))
      {
        restoreProjectAndWindowsState(archive);
      }

      if (!(oldName == _folderName))
      {
        FileOrFolderNameChanged?.Invoke(this, new NameChangedEventArgs(this, oldName, _folderName));
      }
    }

    public void Save(SaveProjectAndWindowsStateDelegate saveProjectAndWindowsState)
    {
      EnsureDirectoryCreatedAndEmpty(_folderName);
      using (var archive = new FileSystemFolderAsProjectArchive(_folderName))
      {
        saveProjectAndWindowsState(archive, null);
      }

    }

    public IDictionary<string, IProjectItem> SaveAs(DirectoryName folderName, SaveProjectAndWindowsStateDelegate saveProjectAndWindowsState)
    {
      var oldName = _folderName;
      _folderName = folderName;

      EnsureDirectoryCreatedAndEmpty(_folderName);


      IDictionary<string, IProjectItem>? dictionaryResult = null;
      using (var archive = new FileSystemFolderAsProjectArchive(_folderName))
      {
        dictionaryResult = saveProjectAndWindowsState(archive, null);
      }

      if (!(oldName == _folderName))
      {
        FileOrFolderNameChanged?.Invoke(this, new NameChangedEventArgs(this, oldName, _folderName));
      }

      return dictionaryResult;
    }

    public IProjectArchive GetArchiveReadOnlyThreadSave(object claimer)
    {
      return new FileSystemFolderAsProjectArchive(_folderName);
    }


    public void ReleaseArchiveThreadSave(object claimer, ref IProjectArchive? archive)
    {
      archive?.Dispose();
    }

    private static void EnsureDirectoryCreatedAndEmpty(DirectoryName folderName)
    {
      var dir = new DirectoryInfo(folderName);
      if (!dir.Exists)
      {
        System.IO.Directory.CreateDirectory(folderName);
      }
      else
      {
        // it is harder if the directory already exist.
        // how do we ensure that we not inadvertely delete files from an arbitrary directory?
        // Answer: either the directory must contain no files (not even in subfolders),
        // or the directory must contain the file "DocumentIdentifier.xml".

        var files = dir.GetFiles("*.*", SearchOption.AllDirectories);
        if (files.Length != 0)
        {
          var isDocumentIdentfierPresent = files.Any((x) => x.Name.ToLowerInvariant() == "documentinformation.xml");

          if (isDocumentIdentfierPresent)
          {
            foreach (var subdir in dir.GetDirectories())
            {
              subdir.Delete(recursive: true);
            }

            foreach (var file in dir.GetFiles())
            {
              file.Delete();
            }
          }
          else
          {
            throw new InvalidOperationException($"To protect your data, the directory {folderName} can not be used as a project storage because it may contain files that are important for you.");
          }
        }
      }
    }
  }
}
