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
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Altaxo.Main.Services.Files;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Manages the permanent storage of a projects into a Zip files. In contrast to <see cref="ZipFileProjectArchiveManager"/>,
  /// this manager does not use a cloned copy of the project file.
  /// </summary>
  /// <seealso cref="Altaxo.Main.Services.IFileBasedProjectArchiveManager" />
  public class ZipFileProjectArchiveManagerWithoutClonedFile : IFileBasedProjectArchiveManager
  {
    private bool _isDisposed;

    public event EventHandler<NameChangedEventArgs>? FileOrFolderNameChanged;

    /// <summary>
    /// The stream of the original project file that is kept open in order to prevent modifications.
    /// </summary>
    private FileStream? _originalFileStream;


    /// <summary>
    /// Gets the name of the file or folder. Can be null if no file or folder is set (up to now).
    /// </summary>
    /// <value>
    /// The name of the file or folder, if known. Otherwise, null is returned.
    /// </value>
    public PathName? FileOrFolderName => FileName.Create(_originalFileStream?.Name);

    /// <inheritdoc/>
    public bool IsDisposed => _isDisposed;

    public static ZipFileProjectArchiveManagerWithoutClonedFile CreateForSavingWithEmptyProjectFile(FileName fileName, bool allowOverwriting)
    {
      var result = new ZipFileProjectArchiveManagerWithoutClonedFile();
      // Open the stream

      if (allowOverwriting)
        result._originalFileStream = new FileStream(fileName.ToString(), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
      else
        result._originalFileStream = new FileStream(fileName.ToString(), FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read);

      return result;
    }

    /// <summary>
    /// Loads a project from a file.
    /// </summary>
    /// <param name="fileName">Name of the file to load from.</param>
    /// <param name="restoreProjectAndWindowsState">Delegate that is used to deserialize and restore the project and the windows state.</param>
    /// <param name="showUserInteraction">If true, and the file is read-only, a dialog box is asking the user whether to open the file in read-only mode.
    /// If false, and the file is read-only, the file will be opened in read-only-mode.</param>
    /// <exception cref="ObjectDisposedException"></exception>
    public void LoadFromFile(FileName fileName, RestoreProjectAndWindowsState restoreProjectAndWindowsState, bool showUserInteraction = true)
    {
      if (_isDisposed) throw new ObjectDisposedException(this.GetType().Name);

      var oldFileName = _originalFileStream?.Name;
      bool hasFileNameChanged = 0 != string.Compare(fileName, _originalFileStream?.Name, false);

      try
      {
        // Open the stream for reading ...
        _originalFileStream = new FileStream(fileName.ToString(), FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
      }
      catch (Exception ex1)
      {
        // try to open as readonly...
        FileStream roFileStream;
        try
        {
          roFileStream = new FileStream(fileName.ToString(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }
        catch (Exception)
        {
          // open as readonly has failed too, so we have to throw..
          _originalFileStream = null;
          throw ex1;
        }
        var shouldOpenReadonly = Current.Gui.YesNoMessageBox($"The file {fileName} seems to be read-only or currently in use.\r\n\r\nDo you want try to open it in read-only mode?", "Question", true);

        if (shouldOpenReadonly)
        {
          LoadFromFileStreamReadonly(roFileStream, restoreProjectAndWindowsState);
        }

        return;
      }

      // deserialize the project....
      using (var projectArchive = new Services.Files.ZipArchiveAsProjectArchive(_originalFileStream, ZipArchiveMode.Read, leaveOpen: true, archiveManager: this))
      {
        // Restore the state of the windows
        restoreProjectAndWindowsState(projectArchive);
      }

      if (hasFileNameChanged)
        FileOrFolderNameChanged?.Invoke(this, new NameChangedEventArgs(this, oldFileName, _originalFileStream?.Name));
    }

    /// <summary>
    /// Loads a project from a file stream in read-only mode. For that, it is tried to make a copy of the file stream, and then
    /// use the copy to read the project from.
    /// </summary>
    /// <param name="fileStream">The file stream to copy from.</param>
    /// <param name="restoreProjectAndWindowsState">Delegate that is used to deserialize and restore the project and the windows state.</param>
    protected void LoadFromFileStreamReadonly(FileStream fileStream, RestoreProjectAndWindowsState restoreProjectAndWindowsState)
    {
      fileStream.Seek(0, SeekOrigin.Begin);

      // now, deserialize the project from the cloned file....
      using (var projectArchive = new Services.Files.ZipArchiveAsProjectArchive(fileStream, ZipArchiveMode.Read, leaveOpen: true, archiveManager: this))
      {
        // Restore the state of the windows
        restoreProjectAndWindowsState(projectArchive);
      }
    }


    /// <summary>
    /// Saves the specified save project and windows state to the same file or folder that was used to open the project.
    /// </summary>
    /// <param name="saveProjectAndWindowsState">State of the save project and windows.</param>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="InvalidOperationException">Save is not possible because no file name was given up to now</exception>
    public void Save(SaveProjectAndWindowsStateDelegate saveProjectAndWindowsState)
    {
      if (_isDisposed) throw new ObjectDisposedException(this.GetType().Name);

      if (_originalFileStream is null)
        throw new InvalidOperationException("Save is not possible because no file name was given up to now");

      SaveAs(FileName.Create(_originalFileStream.Name), saveProjectAndWindowsState);
    }

    /// <summary>
    /// Saves the project with a name given in <paramref name="destinationFileName"/>. The name can or can not be the same name as was used before.
    /// </summary>
    /// <param name="destinationFileName">Name of the destination file.</param>
    /// <param name="saveProjectAndWindowsState">Delegate to store the project document and the windows state into an <see cref="IProjectArchive"/>.</param>
    /// <returns>A dictionary where the keys are the archive entry names that where used to store the project items that are the values. The dictionary contains only those project items that need further handling (e.g. late load handling).</returns>
    /// <exception cref="ObjectDisposedException"></exception>
    public IDictionary<string, IProjectItem> SaveAs(FileName destinationFileName, SaveProjectAndWindowsStateDelegate saveProjectAndWindowsState)
    {
      if (_isDisposed) throw new ObjectDisposedException(this.GetType().Name);

      IDictionary<string, IProjectItem>? dictionaryResult = null;

      var originalFileName = _originalFileStream?.Name;
      bool isNewDestinationFileName = originalFileName != destinationFileName.ToString();


      // Open the old archive, using the original stream
      _originalFileStream?.Seek(0, SeekOrigin.Begin);

      // Create a new archive, either with the name of the original file (if we have the cloned file), or with a temporary file name
      FileStream? newProjectArchiveFileStream = null;
      if (isNewDestinationFileName)
      {
        // create a new file stream for writing to
        newProjectArchiveFileStream = new FileStream(destinationFileName.ToString(), FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
      }

      // now serialize the data
      Exception? savingException = null;

      try
      {
        var eitherStream = newProjectArchiveFileStream ?? _originalFileStream ?? throw new InvalidProgramException("Either of both streams should be not null!");
        using (var newProjectArchive = new Services.Files.ZipArchiveAsProjectArchive(eitherStream, ZipArchiveMode.Create, leaveOpen: true, archiveManager: this))
        {
          dictionaryResult = saveProjectAndWindowsState(newProjectArchive, null);
        }
      }
      catch (Exception ex)
      {
        savingException = ex;
      }


      if (savingException is null)
      {
        // if saving was successfull, we can now clone the data from the new project archive again....
        if (isNewDestinationFileName)
        {
          // we have written to a new file
          _originalFileStream = newProjectArchiveFileStream;
        }
      }
      else // exceptions during saving have occured !!!
      {
        // if saving has failed, we have to restore the old state
        if (isNewDestinationFileName)
        {
          // there is nothing to do - except do close the new file stream
          // we leave it on disk for diagnosing purposes
          newProjectArchiveFileStream?.Close();
          newProjectArchiveFileStream?.Dispose();
        }
      }

      if (savingException is not null)
        throw savingException;

      if (isNewDestinationFileName)
        FileOrFolderNameChanged?.Invoke(this, new NameChangedEventArgs(this, originalFileName, _originalFileStream?.Name));

      return dictionaryResult!;
    }

    #region Clone task




    /// <summary>
    /// Gets an archive, for read-only purposes only. The call to this function should be thread-safe.
    /// It is required to call <see cref="ReleaseArchiveThreadSave(object, ref IProjectArchive)" /> to release the returned archive if it is no longer in use.
    /// </summary>
    /// <param name="claimer">The claimer. If the returned archive is no longer</param>
    /// <returns>
    /// The archive that can be used to retrieve data (read-only).
    /// </returns>
    public IProjectArchive GetArchiveReadOnlyThreadSave(object claimer)
    {
      var name = _originalFileStream?.Name ?? throw new InvalidOperationException("There is no file name just given for the project");
      var stream = new FileStream(name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
      var archive = new ZipArchiveAsProjectArchive(stream, ZipArchiveMode.Read, leaveOpen: false, archiveManager: this);
      return archive;
    }

    /// <summary>
    /// Releases the archive that was claimed with <see cref="GetArchiveReadOnlyThreadSave(object)" />.
    /// </summary>
    /// <param name="claimer">The claimer. This parameter should be identical to that used in the call to <see cref="GetArchiveReadOnlyThreadSave(object)" /></param>
    /// <param name="archive">The archive to release.</param>
    /// .
    public void ReleaseArchiveThreadSave(object claimer, ref IProjectArchive? archive)
    {
      archive?.Dispose();
      archive = null;
    }



    /// <inheritdoc/>
    public void Dispose()
    {
      if (!_isDisposed)
      {
        _isDisposed = true;
        _originalFileStream?.Dispose();
        _originalFileStream = null;
      }
    }

    #endregion

  }
}
