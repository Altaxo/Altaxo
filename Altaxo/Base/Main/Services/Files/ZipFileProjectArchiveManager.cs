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
using System.Threading;
using System.Threading.Tasks;
using Altaxo.Main.Services.Files;
using Altaxo.Serialization.Xml;

namespace Altaxo.Main.Services
{
  public class ZipFileProjectArchiveManager : IFileBasedProjectArchiveManager
  {
    const string ClonedProjectRelativePath = "CurrProj";
    const string ClonedProjectFileName = "CurrProj";

    /// <summary>
    /// The stream of the original project file that is kept open in order to prevent modifications.
    /// </summary>
    FileStream _originalFileStream;

    /// <summary>
    /// The stream of a copy of the original project file. Is also kept open to prevent modifications.
    /// </summary>
    FileStream _clonedFileStream;
    string _clonedFileName;
    Task _cloneTask;
    CancellationTokenSource _cloneTaskCancel;

    public PathName FileOrFolderName => FileName.Create(_originalFileStream?.Name);

    public string LoadFromFile(FileName fileName, RestoreProjectAndWindowsState restoreProjectAndWindowsState)
    {
      CloneTask_CancelAndClearAll();

      var errorText = new StringBuilder();

      try
      {
        // Open the stream for reading ...
        _originalFileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
      }
      catch (Exception ex)
      {
        errorText.AppendLine(ex.ToString());
        _originalFileStream = null;
        return errorText.ToString();
      }

      // deserialize the project....
      Services.Files.ZipArchiveAsProjectArchive projectArchive;
      try
      {
        projectArchive = new Services.Files.ZipArchiveAsProjectArchive(_originalFileStream, ZipArchiveMode.Read, false);
      }
      catch (Exception exc)
      {
        errorText.AppendLine(exc.ToString());
        return errorText.ToString();
      }

      // Restore the state of the windows
      var errors = restoreProjectAndWindowsState(projectArchive);

      if (null != errors)
      {
        errorText.Append(errors);
      }
      else
      {
        // make a copy of the original file
        StartCloneTask();
      }
      return errorText.Length == 0 ? null : errorText.ToString();
    }


    public string Save(SaveProjectAndWindowsStateDelegate saveProjectAndWindowsState)
    {
      if (null == _originalFileStream)
        return "Save is not possible because no file name was given up to now";

      return SaveAs(FileName.Create(_originalFileStream.Name), saveProjectAndWindowsState);
    }

    public string SaveAs(FileName destinationFileName, SaveProjectAndWindowsStateDelegate saveProjectAndWindowsState)
    {
      var errorText = new StringBuilder();
      var originalFileName = _originalFileStream?.Name;
      bool isNewDestinationFileName = destinationFileName != originalFileName;

      TryFinishCloneTask();  // Force decision whether we have a cloned file of the original file or not
      bool useClonedStreamAsBackup = _clonedFileStream != null;

      // Open the old archive, either using the copied stream or the original stream
      _clonedFileStream?.Seek(0, SeekOrigin.Begin);
      _originalFileStream?.Seek(0, SeekOrigin.Begin);

      var oldProjectArchive = _clonedFileStream != null ? new Services.Files.ZipArchiveAsProjectArchive(_clonedFileStream, ZipArchiveMode.Read, false) :
        _originalFileStream != null ? new Services.Files.ZipArchiveAsProjectArchive(_originalFileStream, ZipArchiveMode.Read, false) : null;

      // Create a new archive, either with the name of the original file (if we have the cloned file), or with a temporary file name
      FileStream newProjectArchiveFileStream;
      if (useClonedStreamAsBackup || isNewDestinationFileName)
      {
        _originalFileStream.Close();
        newProjectArchiveFileStream = new FileStream(destinationFileName, FileMode.Create, FileAccess.Write, FileShare.Read);
      }
      else
      {
        var instanceStorageService = Current.GetService<IInstanceStorageService>();
        var path = instanceStorageService.InstanceStoragePath;
        var clonedFileName = Path.Combine(path, ClonedProjectRelativePath, ClonedProjectFileName + Path.GetExtension(destinationFileName));
        newProjectArchiveFileStream = new FileStream(clonedFileName, FileMode.Create, FileAccess.Write, FileShare.Read);
      }

      // now serialize the data
      Exception savingException = null;
      using (var newProjectArchive = new Services.Files.ZipArchiveAsProjectArchive(newProjectArchiveFileStream, ZipArchiveMode.Create, false))
      {
        savingException = saveProjectAndWindowsState(newProjectArchive, oldProjectArchive);
      }

      if (null == savingException)
      {
        // if saving was successfull, we can now clone the data from the new project archive again....
        if (useClonedStreamAsBackup || isNewDestinationFileName)
        {
          // we have written to the original file, and now we need to clone the original file again
          _originalFileStream = newProjectArchiveFileStream;
          StartCloneTask();
        }
        else
        {
          // we have written to the appdata folder directly
          _clonedFileStream = newProjectArchiveFileStream;
          // but we have now to copy this data to the original file

          _originalFileStream?.Close();
          var orgFileStream = new FileStream(destinationFileName, FileMode.Create, FileAccess.Write, FileShare.Read);
          _clonedFileStream.Seek(0, SeekOrigin.Begin);
          _clonedFileStream.CopyTo(orgFileStream);
          _originalFileStream = orgFileStream;
        }
      }
      else
      {
        // if saving has failed, we have to restore the old state
        if (useClonedStreamAsBackup && !isNewDestinationFileName)
        {
          // we have written to the original file, thus the original file is corrupted, we try to restore it with the data from the cloned stream
          _clonedFileStream.Seek(0, SeekOrigin.Begin);
          var originalStreamToWriteTo = new FileStream(originalFileName, FileMode.Create, FileAccess.Write, FileShare.Read);
          _clonedFileStream.CopyTo(_originalFileStream);
          _originalFileStream = new FileStream(originalFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
          originalStreamToWriteTo.Close();
        }
        else if (!useClonedStreamAsBackup && null != _originalFileStream)
        {
          // we have written to the appdata folder directly, so we delete the cloned file; then we try to clone the original (!) file again
          newProjectArchiveFileStream?.Close();
          StartCloneTask();
        }
      }

      return errorText.Length == 0 ? null : errorText.ToString();
    }

    #region Clone task

    void StartCloneTask()
    {
      _clonedFileStream?.Dispose(); // Close/dispose old cloned stream

      var instanceStorageService = Current.GetService<IInstanceStorageService>();
      var path = instanceStorageService.InstanceStoragePath;
      var clonedFileDir = Path.Combine(path, ClonedProjectRelativePath);
      Directory.CreateDirectory(clonedFileDir);
      _clonedFileName = Path.Combine(clonedFileDir, ClonedProjectFileName + Path.GetExtension(_originalFileStream.Name));
      _clonedFileStream = new FileStream(_clonedFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
      _cloneTaskCancel = new CancellationTokenSource();
      _cloneTask = Task.Run(() =>
      {
        using (var orgStream = new FileStream(_originalFileStream.Name, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
          orgStream.CopyTo(_clonedFileStream);
          orgStream.Close();
        }
      }, _cloneTaskCancel.Token);
    }

    void CloneTask_CancelAndClearAll()
    {
      try
      {
        _cloneTaskCancel?.Cancel();
        _cloneTaskCancel?.Dispose();
        _cloneTaskCancel = null;
        _cloneTask?.Dispose();
        _cloneTask = null;
        _clonedFileStream?.Dispose();
        _clonedFileStream = null;
        _clonedFileName = null;
      }
      catch (Exception ex)
      {

      }
    }

    void TryFinishCloneTask()
    {
      if (null != _cloneTask)
      {
        if (!_cloneTask.IsCompleted)
        {
          _cloneTaskCancel.Cancel();
          _cloneTask.Dispose();
          _cloneTaskCancel.Dispose();
          _cloneTaskCancel = null;
          _clonedFileStream?.Dispose();
          _clonedFileStream = null;
          _clonedFileName = null;
        }
        else
        {
          if (!(_cloneTask.Exception is null))
          {
            _cloneTask?.Dispose();
            _cloneTask = null;
            _cloneTaskCancel?.Dispose();
            _cloneTaskCancel = null;
            _clonedFileStream?.Dispose();
            _clonedFileStream = null;
            _clonedFileName = null;
          }
        }
      }
    }

    public void Dispose()
    {
      throw new NotImplementedException();
    }

    #endregion

  }
}
