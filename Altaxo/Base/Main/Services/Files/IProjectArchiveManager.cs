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
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Delegate that should load the project from the given archive, and then restore the state of the Gui.
  /// </summary>
  /// <param name="archive">The archive to load the project from.</param>
  public delegate void RestoreProjectAndWindowsState(IProjectArchive archive);


  /// <summary>
  /// Saves the current project and the windows state to an archive.
  /// </summary>
  /// <param name="newArchive">The archive to save the project and windows state to.</param>
  /// <param name="oldArchive">The old project archive. Can be null. If not null, this archive represents the state of the project at the last saving.</param>
  public delegate void SaveProjectAndWindowsStateDelegate(IProjectArchive newArchive, IProjectArchive oldArchive);


  /// <summary>
  /// Manages an open project archive, including making and maintaining a safety copy of the project, and saving the project
  /// into a new or temporary file or folder.
  /// </summary>
  public interface IProjectArchiveManager : IDisposable
  {
    /// <summary>
    /// Gets the name of the file or folder. Can be null if no file or folder is set (up to now).
    /// </summary>
    /// <value>
    /// The name of the file or folder, if known. Otherwise, null is returned.
    /// </value>
    PathName FileOrFolderName { get; }

    /// <summary>
    /// Gets a value indicating whether this instance is disposed.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is disposed; otherwise, <c>false</c>.
    /// </value>
    bool IsDisposed { get; }

    /// <summary>
    /// Saves the specified save project and windows state to the same file or folder that was used to open the project.
    /// </summary>
    /// <param name="saveProjectAndWindowsState">State of the save project and windows.</param>
    void Save(SaveProjectAndWindowsStateDelegate saveProjectAndWindowsState);

    /// <summary>
    /// Intended for deferred reading of (typically only a single) project item from an archive.
    /// Gets an archive, for read-only purposes only. The call to this function should be thread-safe.
    /// It is required to call <see cref="ReleaseArchiveThreadSave(object, ref IProjectArchive)"/> to release the returned archive if it is no longer in use.
    /// </summary>
    /// <param name="claimer">The claimer. If the returned archive is no longer </param>
    /// <returns>The archive that can be used to retrieve data (read-only).</returns>
    IProjectArchive GetArchiveReadOnlyThreadSave(object claimer);

    /// <summary>
    /// Releases the archive that was claimed with <see cref="GetArchiveReadOnlyThreadSave(object)"/>.
    /// </summary>
    /// <param name="claimer">The claimer. This parameter should be identical to that used in the call to <see cref="GetArchiveReadOnlyThreadSave(object)"/></param>.
    /// <param name="archive">The archive to release.</param>
    void ReleaseArchiveThreadSave(object claimer, ref IProjectArchive archive);
  }

  /// <summary>
  /// Represents a file base (such as a Zip file, an XML file, etc.) project archive that is currently open.
  /// </summary>
  /// <seealso cref="Altaxo.Main.Services.IProjectArchiveManager" />
  public interface IFileBasedProjectArchiveManager : IProjectArchiveManager
  {
    /// <summary>
    /// Loads a project from a file.
    /// </summary>
    /// <param name="fileName">Name of the file to load from.</param>
    /// <param name="restoreProjectAndWindowsState">Delegate that is used to deserialize and restore the project and the windows state.</param>
    void LoadFromFile(FileName fileName, RestoreProjectAndWindowsState restoreProjectAndWindowsState);

    /// <summary>
    /// Saves a project into a Zip file using the name given in <paramref name="fileName"/>.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="saveProjectAndWindowsState">A delegate that saves the project document and the Gui state into a <see cref="IProjectArchive"/>.</param>
    void SaveAs(FileName fileName, SaveProjectAndWindowsStateDelegate saveProjectAndWindowsState);
  }

  /// <summary>
  /// Represents a folder based project archive (entries of the archive represented by files in that folder), that is currently open.
  /// </summary>
  /// <seealso cref="Altaxo.Main.Services.IProjectArchiveManager" />
  public interface IFolderBasedProjectArchiveManager : IProjectArchiveManager
  {
    /// <summary>
    /// Loads a project from a folder. The files in this folder represent the project items.
    /// </summary>
    /// <param name="folderName">Name of the folder.</param>
    /// <param name="restoreProjectAndWindowsState">Delegate that is used to deserialize and restore the project and the windows state.</param>
    /// <returns></returns>
    void LoadFromFolder(DirectoryName folderName, RestoreProjectAndWindowsState restoreProjectAndWindowsState);

    /// <summary>
    /// Saves a project into a file system folder (the project items are represented by files in that folder). The folder name is given in <paramref name="folderName"/>.
    /// </summary>
    /// <param name="folderName">Name of the folder.</param>
    /// <param name="saveProjectAndWindowsState">A delegate that saves the project document and the Gui state into a <see cref="IProjectArchive"/>.</param>
    void SaveAs(DirectoryName folderName, SaveProjectAndWindowsStateDelegate saveProjectAndWindowsState);
  }
}
