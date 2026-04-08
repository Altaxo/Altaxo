// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Text;
using Altaxo.Main.Services;

namespace Altaxo.Gui.Workbench
{
  /// <summary>
  /// Manages the list files opened by view contents so that multiple view contents opening the same file can synchronize.
  /// Also provides events that can be used to listen to file operations performed in the IDE.
  /// </summary>
  [GlobalService("FileService")]
  public interface IFileService
  {
    #region Options

    /// <summary>
    /// Gets the recent file list service.
    /// </summary>
    IRecentOpen RecentOpen { get; }

    /// <summary>
    /// Gets or sets a value indicating whether deleted files are moved to the recycle bin.
    /// </summary>
    bool DeleteToRecycleBin { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether files are saved by using a temporary file first.
    /// </summary>
    bool SaveUsingTemporaryFile { get; set; }

    /// <summary>
    /// Gets the default file encoding.
    /// This property is thread-safe.
    /// </summary>
    Encoding DefaultFileEncoding { get; }

    /// <summary>
    /// Gets or sets the default file encoding information.
    /// </summary>
    EncodingInfo DefaultFileEncodingInfo { get; set; }

    /// <summary>
    /// Gets the list of available encodings.
    /// </summary>
    IReadOnlyList<EncodingInfo> AllEncodings { get; }

    #endregion Options

    #region BrowseForFolder

    /// <summary>
    /// Shows a 'browse for folder' dialog.
    /// </summary>
    /// <param name="description">Description shown in the dialog.</param>
    /// <param name="selectedPath">Optional: Initially selected folder.</param>
    /// <returns>The selected folder; or <c>null</c> if the user cancelled the dialog.</returns>
    string? BrowseForFolder(string description, string? selectedPath = null);

    #endregion BrowseForFolder

    #region OpenedFiles

    /// <summary>
    /// Gets a collection containing all currently opened files.
    /// The returned collection is a read-only copy of the currently opened files -
    /// it will not reflect future changes of the list of opened files.
    /// </summary>
    IReadOnlyList<OpenedFile> OpenedFiles { get; }

    /// <summary>
    /// Gets an opened file, or returns null if the file is not opened.
    /// </summary>
    OpenedFile? GetOpenedFile(FileName fileName);

    /// <summary>
    /// Gets an opened file, or returns null if the file is not opened.
    /// </summary>
    OpenedFile? GetOpenedFile(string fileName);

    /// <summary>
    /// Gets or creates an opened file.
    /// Warning: the opened file will be a file without any views attached.
    /// Make sure to attach a view to it, or call CloseIfAllViewsClosed on the OpenedFile to
    /// unload the OpenedFile instance if no views were attached to it.
    /// </summary>
    OpenedFile GetOrCreateOpenedFile(FileName fileName);

    /// <summary>
    /// Gets or creates an opened file.
    /// Warning: the opened file will be a file without any views attached.
    /// Make sure to attach a view to it, or call CloseIfAllViewsClosed on the OpenedFile to
    /// unload the OpenedFile instance if no views were attached to it.
    /// </summary>
    OpenedFile GetOrCreateOpenedFile(string fileName);

    /// <summary>
    /// Creates a new untitled OpenedFile.
    /// </summary>
    OpenedFile CreateUntitledOpenedFile(string defaultName, byte[] content);

    #endregion OpenedFiles

    #region CheckFileName

    /// <summary>
    /// Checks if the path is valid <b>and shows a MessageBox if it is not valid</b>.
    /// Do not use in non-UI methods.
    /// </summary>
    /// <seealso cref="FileUtility.IsValidPath"/>
    bool CheckFileName(string path);

    /// <summary>
    /// Checks that a single directory entry (file or subdirectory) name is valid
    ///  <b>and shows a MessageBox if it is not valid</b>.
    /// </summary>
    /// <param name="name">A single file name not the full path</param>
    /// <seealso cref="FileUtility.IsValidDirectoryEntryName"/>
    bool CheckDirectoryEntryName(string name);

    #endregion CheckFileName

    #region OpenFile (ViewContent)

    /// <summary>
    /// Gets whether the file is open in a view content.
    /// </summary>
    bool IsOpen(FileName fileName);

    /// <summary>
    /// Opens a view content for the specified file
    /// or returns the existing view content for the file if it is already open.
    /// </summary>
    /// <param name="fileName">The name of the file to open.</param>
    /// <param name="switchToOpenedView">Specifies whether to switch to the view for the specified file.</param>
    /// <returns>The existing or opened <see cref="IViewContent"/> for the specified file.</returns>
    IFileViewContent? OpenFile(FileName fileName, bool switchToOpenedView = true);

    /// <summary>
    /// Opens a view content for the specified file using the specified display binding.
    /// </summary>
    /// <param name="fileName">The name of the file to open.</param>
    /// <param name="displayBinding">The display binding to use for opening the file.</param>
    /// <param name="switchToOpenedView">Specifies whether to switch to the view for the specified file.</param>
    /// <returns>The existing or opened <see cref="IViewContent"/> for the specified file.</returns>
    IFileViewContent? OpenFileWith(FileName fileName, IDisplayBinding displayBinding, bool switchToOpenedView = true);

    /// <summary>
    /// Shows the 'Open With' dialog, allowing the user to pick a display binding for opening the specified files.
    /// </summary>
    IEnumerable<IFileViewContent> ShowOpenWithDialog(IEnumerable<FileName> fileNames, bool switchToOpenedView = true);

    /// <summary>
    /// Opens a new unsaved file.
    /// </summary>
    /// <param name="defaultName">The (unsaved) name of the to open</param>
    /// <param name="content">Content of the file to create</param>
    IFileViewContent? NewFile(string defaultName, string content);

    /// <summary>
    /// Opens a new unsaved file.
    /// </summary>
    /// <param name="defaultName">The (unsaved) name of the to open</param>
    /// <param name="content">Content of the file to create</param>
    IFileViewContent? NewFile(string defaultName, byte[] content);

    /// <summary>
    /// Gets a list of the names of the files that are open as primary files
    /// in view contents.
    /// </summary>
    IReadOnlyList<FileName> OpenPrimaryFiles { get; }

    /// <summary>
    /// Gets the IViewContent for a fileName. Returns null if the file is not opened currently.
    /// </summary>
    IFileViewContent? GetOpenFile(FileName fileName);

    /// <summary>
    /// Opens the specified file and jumps to the specified file position.
    /// Line and column start counting at 1.
    /// </summary>
    IFileViewContent? JumpToFilePosition(FileName fileName, int line, int column);

    #endregion OpenFile (ViewContent)

    #region Remove/Rename/Copy

    /// <summary>
    /// Removes a file, raising the appropriate events. This method may show message boxes.
    /// </summary>
    void RemoveFile(string fileName, bool isDirectory);

    /// <summary>
    /// Renames or moves a file, raising the appropriate events. This method may show message boxes.
    /// </summary>
    bool RenameFile(string oldName, string newName, bool isDirectory);

    /// <summary>
    /// Copies a file, raising the appropriate events. This method may show message boxes.
    /// </summary>
    bool CopyFile(string oldName, string newName, bool isDirectory, bool overwrite);

    /// <summary>
    /// Occurs before a file or directory is renamed.
    /// </summary>
    event EventHandler<FileRenamingEventArgs> FileRenaming;

    /// <summary>
    /// Occurs after a file or directory was renamed.
    /// </summary>
    event EventHandler<FileRenameEventArgs> FileRenamed;

    /// <summary>
    /// Occurs before a file or directory is copied.
    /// </summary>
    event EventHandler<FileRenamingEventArgs> FileCopying;

    /// <summary>
    /// Occurs after a file or directory was copied.
    /// </summary>
    event EventHandler<FileRenameEventArgs> FileCopied;

    /// <summary>
    /// Occurs before a file or directory is removed.
    /// </summary>
    event EventHandler<FileCancelEventArgs> FileRemoving;

    /// <summary>
    /// Occurs after a file or directory was removed.
    /// </summary>
    event EventHandler<FileEventArgs> FileRemoved;

    #endregion Remove/Rename/Copy

    #region FileCreated/Replaced

    /// <summary>
    /// Fires the event handlers for a file being created.
    /// </summary>
    /// <param name="fileName">The name of the file being created. This should be a fully qualified path.</param>
    /// <param name="isDirectory">Set to true if this is a directory</param>
    /// <returns>True if the operation can proceed, false if an event handler cancelled the operation.</returns>
    bool FireFileReplacing(string fileName, bool isDirectory);

    /// <summary>
    /// Fires the event handlers for a file being replaced.
    /// </summary>
    /// <param name="fileName">The name of the file being created. This should be a fully qualified path.</param>
    /// <param name="isDirectory">Set to true if this is a directory</param>
    void FireFileReplaced(string fileName, bool isDirectory);

    /// <summary>
    /// Fires the event handlers for a file being created.
    /// </summary>
    /// <param name="fileName">The name of the file being created. This should be a fully qualified path.</param>
    /// <param name="isDirectory">Set to true if this is a directory</param>
    void FireFileCreated(string fileName, bool isDirectory);

    /// <summary>
    /// Occurs after a file or directory was created.
    /// </summary>
    event EventHandler<FileEventArgs> FileCreated;

    /// <summary>
    /// Occurs before a file or directory is replaced.
    /// </summary>
    event EventHandler<FileCancelEventArgs> FileReplacing;

    /// <summary>
    /// Occurs after a file or directory was replaced.
    /// </summary>
    event EventHandler<FileEventArgs> FileReplaced;

    #endregion FileCreated/Replaced
  }
}
