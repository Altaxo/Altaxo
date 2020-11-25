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
using System.Threading;
using System.Threading.Tasks;
using Altaxo.Main.Services.Files;
using Altaxo.Serialization;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Manages the permanent storage of projects into Zip files, including cloning to, and maintaining a safety copy.
  /// This manager uses Zip files zipped with the Altaxo provided Zip routines. This, progressive storage is supported, but the risk of data failures is higher.
  /// </summary>
  /// <seealso cref="Altaxo.Main.Services.IFileBasedProjectArchiveManager" />
  public abstract class ZipFileProjectArchiveManagerBase : IFileBasedProjectArchiveManager
  {
    private const string ClonedProjectRelativePath = "CurrProj";
    private const string ClonedProjectFileName = "CurrProj";
    private bool _isDisposed;
    protected StorageSettings? _storageSettings;

    /// <summary>
    /// Function delegate to create a new project archive.
    /// </summary>
    /// <param name="stream">The file stream this archive is based on.</param>
    /// <param name="zipArchiveMode">The zip archive mode.</param>
    /// <param name="leaveOpen">If set to <c>true</c>, the stream is left open after the project archived is closed (disposed).</param>
    /// <param name="archiveManager">The archive manager that manages this archive.</param>
    /// <returns>The newly created project archive.</returns>
    public delegate IProjectArchive ProjectArchiveCreationFunction(Stream stream, ZipArchiveMode zipArchiveMode, bool leaveOpen, IFileBasedProjectArchiveManager archiveManager);

    /// <summary>
    /// Creates a new project archive. This function is abstract and must be overridden in derived classes.
    /// </summary>
    protected abstract ProjectArchiveCreationFunction CreateProjectArchive { get; }


    public event EventHandler<NameChangedEventArgs>? FileOrFolderNameChanged;

    /// <summary>
    /// The stream of the original project file that is kept open in order to prevent modifications.
    /// </summary>
    private FileStream? _originalFileStream;

    /// <summary>
    /// The stream of a copy of the original project file. Is also kept open to prevent modifications.
    /// </summary>
    private FileStream? _clonedFileStream;
    private Task? _cloneTask;
    private CancellationTokenSource? _cloneTaskCancel;

    /// <summary>
    /// Gets the name of the file or folder. Can be null if no file or folder is set (up to now).
    /// </summary>
    /// <value>
    /// The name of the file or folder, if known. Otherwise, null is returned.
    /// </value>
    public PathName? FileOrFolderName => FileName.Create(_originalFileStream?.Name);

    /// <inheritdoc/>
    public bool IsDisposed => _isDisposed;

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

      CloneTask_CancelAndClearAll();

      try
      {
        // Open the stream for reading ...
        _originalFileStream = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
      }
      catch (Exception ex1)
      {
        // try to open as readonly...
        FileStream roFileStream;
        try
        {
          roFileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }
        catch (Exception)
        {
          // open as readonly has failed too, so we have to throw..
          _originalFileStream = null;
#pragma warning disable CA2200 // Rethrow to preserve stack details
          throw ex1;
#pragma warning restore CA2200 // Rethrow to preserve stack details
        }


        bool shouldOpenReadonly = true;
        if (showUserInteraction)
        {
          Current.Gui.YesNoMessageBox($"The file {fileName} seems to be read-only or currently in use.\r\n\r\nDo you want try to open it in read-only mode?", "Question", true);
        }

        if (shouldOpenReadonly)
        {
          LoadFromFileStreamReadonly(roFileStream, restoreProjectAndWindowsState);
        }

        return;
      }


      // deserialize the project....
      using (var projectArchive = CreateProjectArchive(_originalFileStream, ZipArchiveMode.Read, leaveOpen: true, archiveManager: this))
      {
        // Restore the state of the windows
        restoreProjectAndWindowsState(projectArchive);
      }

      // make a copy of the original file
      StartCloneTask();

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
      try
      {
        // Here, we can't copy the data to the cloned file in the background...
        // Instead, we have to wait for the end of the copy process, and then restore the project from the cloned file
        var clonedFileName = GetClonedFileName(fileStream.Name);
        fileStream.Seek(0, SeekOrigin.Begin);
        var clonedFileStream = new FileStream(clonedFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
        fileStream.CopyTo(clonedFileStream);
        _originalFileStream = null;
        _clonedFileStream = clonedFileStream;
      }
      finally
      {
        fileStream.Dispose();
      }

      // now, deserialize the project from the cloned file....
      using (var projectArchive = CreateProjectArchive(_clonedFileStream, ZipArchiveMode.Read, leaveOpen: true, archiveManager: this))
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
#nullable disable
      if (_isDisposed) throw new ObjectDisposedException(this.GetType().Name);

      var context = Current.Project.GetPropertyContext();
      var storageSettings = context.GetValue(Altaxo.Serialization.StorageSettings.PropertyKeyStorageSettings, new Serialization.StorageSettings());

      IDictionary<string, IProjectItem> dictionaryResult = null;

      var originalFileName = _originalFileStream?.Name;
      bool isNewDestinationFileName = originalFileName != (string)destinationFileName;

      TryFinishCloneTask();  // Force decision whether we have a cloned file of the original file or not
      bool useClonedStreamAsBackup = _clonedFileStream is not null;

      // Open the old archive, either using the copied stream or the original stream
      _clonedFileStream?.Seek(0, SeekOrigin.Begin);
      _originalFileStream?.Seek(0, SeekOrigin.Begin);

      // Create a new archive, either with the name of the original file (if we have the cloned file), or with a temporary file name
      FileStream newProjectArchiveFileStream = null;
      if (isNewDestinationFileName)
      {
        // create a new file stream for writing to
        newProjectArchiveFileStream = new FileStream(destinationFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
      }
      else if (useClonedStreamAsBackup)
      {
        // use the original file stream for writing to (but cut the length to zero)
        _originalFileStream.SetLength(0);
      }
      else // there is no cloned file in the local app folder yet
      {
        // create a file in the local app folder for writing to
        var instanceStorageService = Current.GetService<IInstanceStorageService>();
        var path = instanceStorageService.InstanceStoragePath;
        var clonedPath = Path.Combine(path, ClonedProjectRelativePath);
        var clonedFileName = Path.Combine(clonedPath, ClonedProjectFileName + Path.GetExtension(destinationFileName));
        Directory.CreateDirectory(clonedPath);
        newProjectArchiveFileStream = new FileStream(clonedFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
      }

      // now serialize the data
      Exception savingException = null;

      using (var oldProjectArchive = useClonedStreamAsBackup ?
                CreateProjectArchive(_clonedFileStream, ZipArchiveMode.Read, leaveOpen: true, archiveManager: this) :
                                _originalFileStream is not null ?
                  CreateProjectArchive(_originalFileStream, ZipArchiveMode.Read, leaveOpen: true, archiveManager: this) : null
            )
      {

        try
        {
          using (var newProjectArchive = CreateProjectArchive(newProjectArchiveFileStream ?? _originalFileStream, ZipArchiveMode.Create, leaveOpen: true, archiveManager: this))
          {
            dictionaryResult = saveProjectAndWindowsState(newProjectArchive, oldProjectArchive);
          }
          if (!ZipAnalyzerAxo.IsZipFileOkay(newProjectArchiveFileStream ?? _originalFileStream, ZipAnalyzerOptions.TestCentralDirectoryForNameDublettes | ZipAnalyzerOptions.TestStrictOrderOfLocalFileHeaders | ZipAnalyzerOptions.TestExistenceOfTheLocalFileHeaders, out string errorMessage))
          {
            savingException = new InvalidDataException($"Project file that was just saved is corrupt! Details: {errorMessage}. Switching off progressive saving might help.");
          }
        }
        catch (Exception ex)
        {
          savingException = ex;
        }
      }

      if (savingException is null)
      {
        // if saving was successfull, we can now clone the data from the new project archive again....
        if (isNewDestinationFileName)
        {
          // we have written to a new file, so we take it as the original file, and clone this file
          _originalFileStream?.Close();
          _originalFileStream?.Dispose();
          _originalFileStream = newProjectArchiveFileStream;
          StartCloneTask();
        }
        else if (useClonedStreamAsBackup)
        {
          // we have written to the original file, and now we need to clone the original file again
          StartCloneTask();
        }
        else
        {
          // we have written to the appdata folder directly
          _clonedFileStream = newProjectArchiveFileStream;
          // but we have now to copy this data to the original file

          _originalFileStream?.Close();
          _originalFileStream?.Dispose();
          _originalFileStream = null;

          var orgFileStream = new FileStream(destinationFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
          _clonedFileStream.Seek(0, SeekOrigin.Begin);
          _clonedFileStream.CopyTo(orgFileStream);
          _originalFileStream = orgFileStream;
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
          newProjectArchiveFileStream.Dispose();
        }
        else if (useClonedStreamAsBackup)
        {
          // we have written to the original file, thus this original file is now corrupted!
          // so we try to restore it with the data from the cloned stream
          // this we will __not__ do in the background - we wait until the file is really restored!
          _clonedFileStream.Seek(0, SeekOrigin.Begin);
          _originalFileStream.Seek(0, SeekOrigin.Begin);
          _clonedFileStream.CopyTo(_originalFileStream);
          _originalFileStream.Flush();
          _originalFileStream.SetLength(_clonedFileStream.Length);
        }
        else
        {
          // we have written to the appdata folder directly
          // so the cloned file is now corrupted
          // so we delete the cloned file; then we try to clone the original (!) file again
          newProjectArchiveFileStream.Close();
          newProjectArchiveFileStream.Dispose();
          StartCloneTask();
        }
      }

      if (savingException is not null)
        throw savingException;

      if (isNewDestinationFileName)
        FileOrFolderNameChanged?.Invoke(this, new NameChangedEventArgs(this, originalFileName, _originalFileStream?.Name));

      return dictionaryResult;
    }
#nullable enable
    #region Clone task

    /// <summary>
    /// Starts a task to clone the original file into a file located in the local app data folder.
    /// </summary>
    private void StartCloneTask()
    {
      if (_originalFileStream is null)
        throw new InvalidProgramException();

      _clonedFileStream?.Dispose(); // Close/dispose old cloned stream
      _clonedFileStream = null;

      var clonedFileName = GetClonedFileName();
      _cloneTaskCancel = new CancellationTokenSource();

      {
        var cancellationToken = _cloneTaskCancel.Token;
        var clonedFileStream = new FileStream(clonedFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
        var orgStream = new FileStream(_originalFileStream.Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        _cloneTask = Task.Run(async () =>
        {
          try
          {
            await orgStream.CopyToAsync(clonedFileStream, 81920, cancellationToken);
            await clonedFileStream.FlushAsync(cancellationToken);
            if (!ZipAnalyzerAxo.IsZipFileOkay(clonedFileStream, ZipAnalyzerOptions.TestCentralDirectoryForNameDublettes | ZipAnalyzerOptions.TestStrictOrderOfLocalFileHeaders | ZipAnalyzerOptions.TestExistenceOfTheLocalFileHeaders, out string errorMessage))
            {
              Current.Console.WriteLine($"Error: Clone of original project file {orgStream.Name} not used because it is corrupt.");
              Current.Console.WriteLine($"Details: {errorMessage}");
              throw new InvalidDataException("Cloned project file is corrupt");
            }
            orgStream.Dispose();
            _clonedFileStream = clonedFileStream;
          }
          catch (Exception) // catches all exception inclusive the TaskCancelledException
          {
            try
            {
              var fn = clonedFileStream?.Name;
              clonedFileStream?.Dispose();
              if (!string.IsNullOrEmpty(fn))
                File.Delete(fn);
            }
            catch
            {

            }
          }
        }, cancellationToken
        );
      }
    }

    private string GetClonedFileName(string? originalFileName = null)
    {
      var effectiveOriginalFileName = originalFileName ?? _originalFileStream?.Name;
      if (effectiveOriginalFileName is null)
        throw new InvalidProgramException("Both the file name provided in the argument as well as the existing project file stream are null.");

      var instanceStorageService = Current.GetRequiredService<IInstanceStorageService>();
      var path = instanceStorageService.InstanceStoragePath;
      var clonedFileDir = Path.Combine(path, ClonedProjectRelativePath);
      Directory.CreateDirectory(clonedFileDir);
      var clonedFileName = Path.Combine(clonedFileDir, ClonedProjectFileName + Path.GetExtension(effectiveOriginalFileName));
      return clonedFileName;
    }

    /// <summary>
    /// If the clone task is still active, cancels the clone task.
    /// Invalidates the clone stream in any case.
    /// </summary>
    private void CloneTask_CancelAndClearAll()
    {
      try
      {
        if (_cloneTask is not null)
        {
          _cloneTaskCancel?.Cancel();
          if (_cloneTask?.Status == TaskStatus.Running)
          {
            _cloneTask.Wait();
          }
          while (_cloneTask is { } cloneTask && !(cloneTask.Status == TaskStatus.RanToCompletion || cloneTask.Status == TaskStatus.Faulted || cloneTask.Status == TaskStatus.Canceled))
          {
            System.Threading.Thread.Sleep(1);
          }
          _cloneTask?.Dispose();
          _cloneTask = null;
          _cloneTaskCancel?.Dispose();
          _cloneTaskCancel = null;
        }
        _clonedFileStream?.Dispose();
        _clonedFileStream = null;
      }
      catch (Exception)
      {

      }
    }

    /// <summary>
    /// Tests the state of the clone task.
    /// If it is finished, the call returns.
    /// If it is yet not finished, the task is cancelled, and the cloned stream is disposed.
    /// </summary>
    private void TryFinishCloneTask()
    {
      if (_cloneTask is not null)
      {
        if (!_cloneTask.IsCompleted)
        {
          _cloneTaskCancel?.Cancel();
          if (_cloneTask?.Status == TaskStatus.Running)
          {
            _cloneTask?.Wait();
          }

          // System.Diagnostics.Debug.WriteLine($"Status of clone task is {_cloneTask.Status}");

          // int slept = 0;
          while (_cloneTask is { } cloneTask && !(cloneTask.Status == TaskStatus.RanToCompletion || cloneTask.Status == TaskStatus.Faulted || cloneTask.Status == TaskStatus.Canceled))
          {
            System.Threading.Thread.Sleep(1);
            // slept += 1;
          }

          // System.Diagnostics.Debug.WriteLine($"Status of clone task is now {_cloneTask.Status}, slept {slept} ms");

          _cloneTask?.Dispose();
          _cloneTask = null;
          _cloneTaskCancel?.Dispose();
          _cloneTaskCancel = null;
          _clonedFileStream?.Dispose();
          _clonedFileStream = null;
        }
        else // Clone task runs to completion
        {
          if (_cloneTask.Exception is not null) // Dispose the cloned stream if there was an exception
          {
            _cloneTask?.Dispose();
            _cloneTask = null;
            _cloneTaskCancel?.Dispose();
            _cloneTaskCancel = null;
            _clonedFileStream?.Dispose();
            _clonedFileStream = null;
          }
        }
      }
    }


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
      var name = _clonedFileStream?.Name ?? _originalFileStream?.Name ?? throw new InvalidProgramException($"This call should happen only if a file name is already given to the project");
      var stream = new FileStream(name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
      var archive = CreateProjectArchive(stream, ZipArchiveMode.Read, leaveOpen: false, archiveManager: this);
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
        CloneTask_CancelAndClearAll();
        _originalFileStream?.Dispose();
        _originalFileStream = null;
      }
    }

    #endregion

  }
}
