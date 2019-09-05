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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Delegate that should load the project from the given archive, and then restore the state of the Gui.
  /// </summary>
  /// <param name="archive">The archive to load the project from.</param>
  /// <returns>Null if successfull, otherwise the error messages.</returns>
  public delegate string RestoreProjectAndWindowsState(IProjectArchive archive);


  /// <summary>
  /// Saves the current project and the windows state to an archive.
  /// </summary>
  /// <param name="newArchive">The archive to save the project and windows state to.</param>
  /// <param name="oldArchive">The old project archive. Can be null. If not null, this archive represents the state of the project at the last saving.</param>
  /// <returns>Null if successfull, otherwise the exception.</returns>
  public delegate Exception SaveProjectAndWindowsStateDelegate(IProjectArchive newArchive, IProjectArchive oldArchive);


  /// <summary>
  /// Represents a project archive that is currently open.
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
    /// Saves the specified save project and windows state to the same file or folder that was used to open the project.
    /// </summary>
    /// <param name="saveProjectAndWindowsState">State of the save project and windows.</param>
    /// <returns></returns>
    string Save(SaveProjectAndWindowsStateDelegate saveProjectAndWindowsState);
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
    /// <returns>Null if successfull; otherwise, error messages.</returns>
    string LoadFromFile(FileName fileName, RestoreProjectAndWindowsState restoreProjectAndWindowsState);

    string SaveAs(FileName fileName, SaveProjectAndWindowsStateDelegate saveProjectAndWindowsState);
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
    string LoadFromFolder(DirectoryName folderName, RestoreProjectAndWindowsState restoreProjectAndWindowsState);

    string SaveAs(DirectoryName folderName, SaveProjectAndWindowsStateDelegate saveProjectAndWindowsState);
  }
}
