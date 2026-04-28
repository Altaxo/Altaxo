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
    /// Gets the opened file for the specified <see cref="FileName"/>, or returns <see langword="null"/> if it is not open.
    /// </summary>
    /// <param name="fileName">The name of the file to get.</param>
    /// <returns>The opened file, or <c>null</c> if the file is not opened.</returns>
    OpenedFile? GetOpenedFile(FileName fileName);

    /// <summary>
    /// Gets the opened file for the specified path, or returns <see langword="null"/> if it is not open.
    /// </summary>
    /// <param name="fileName">The name of the file to get.</param>
    /// <returns>The opened file, or <c>null</c> if the file is not opened.</returns>
    OpenedFile? GetOpenedFile(string fileName);

    /// <summary>
    /// Gets or creates an opened file for the specified <see cref="FileName"/>.
    /// Warning: the opened file will be a file without any views attached.
    /// Make sure to attach a view to it, or call CloseIfAllViewsClosed on the OpenedFile to
    /// unload the OpenedFile instance if no views were attached to it.
    /// </summary>
    /// <param name="fileName">The file to open or create.</param>
    /// <returns>The opened file.</returns>
    OpenedFile GetOrCreateOpenedFile(FileName fileName);

    /// <summary>
    /// Gets or creates an opened file for the specified path.
    /// Warning: the opened file will be a file without any views attached.
    /// Make sure to attach a view to it, or call CloseIfAllViewsClosed on the OpenedFile to
    /// unload the OpenedFile instance if no views were attached to it.
    /// </summary>
    /// <param name="fileName">The file to open or create.</param>
    /// <returns>The opened file.</returns>
    OpenedFile GetOrCreateOpenedFile(string fileName);

    /// <summary>
    /// Creates a new untitled OpenedFile.
    /// </summary>
    /// <value>The new opened file.</value>
    /// <param name="defaultName">The unsaved name of the file to create.</param>
    /// <param name="content">The content of the file to create.</param>
    /// <returns>The created opened file.</returns>
    OpenedFile CreateUntitledOpenedFile(string defaultName, byte[] content);

    #endregion OpenedFiles

    #region CheckFileName

    /// <summary>
    /// Checks if the path is valid <b>and shows a MessageBox if it is not valid</b>.
    /// Do not use in non-UI methods.
    /// </summary>
    /// <seealso cref="FileUtility.IsValidPath"/>
    /// <param name="path">The path to validate.</param>
    /// <returns><see langword="true"/> if the path is valid; otherwise, <see langword="false"/>.</returns>
    bool CheckFileName(string path);

    /// <summary>
    /// Checks that a single directory entry (file or subdirectory) name is valid
    ///  <b>and shows a MessageBox if it is not valid</b>.
    /// </summary>
    /// <param name="name">A single file name not the full path</param>
    /// <seealso cref="FileUtility.IsValidDirectoryEntryName"/>
    /// <returns><see langword="true"/> if the name is valid; otherwise, <see langword="false"/>.</returns>
    bool CheckDirectoryEntryName(string name);

    #endregion CheckFileName

    #region OpenFile (ViewContent)

    /// <summary>
    /// Gets whether the file is open in a view content.
    /// </summary>
    /// <param name="fileName">The name of the file to check.</param>
    /// <returns><see langword="true"/> if the file is open in a view content; otherwise, <see langword="false"/>.</returns>
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
    /// <param name="fileNames">The files to open.</param>
    /// <param name="switchToOpenedView">Specifies whether to switch to the view for the specified files.</param>
    /// <returns>The opened file view contents.</returns>
    IEnumerable<IFileViewContent> ShowOpenWithDialog(IEnumerable<FileName> fileNames, bool switchToOpenedView = true);

    /// <summary>
    /// Opens a new unsaved text file.
    /// </summary>
    /// <param name="defaultName">The unsaved name of the file to open.</param>
    /// <param name="content">The text content of the file to create.</param>
    /// <returns>The created file view content.</returns>
    IFileViewContent? NewFile(string defaultName, string content);

    /// <summary>
    /// Opens a new unsaved binary file.
    /// </summary>
    /// <param name="defaultName">The unsaved name of the file to open.</param>
    /// <param name="content">The binary content of the file to create.</param>
    /// <returns>The created file view content.</returns>
    IFileViewContent? NewFile(string defaultName, byte[] content);

    /// <summary>
    /// Gets a list of the names of the files that are open as primary files
    /// in view contents.
    /// </summary>
    /// <value>The names of the files that are open as primary files.</value>
    IReadOnlyList<FileName> OpenPrimaryFiles { get; }

    /// <summary>
    /// Gets the file view content for the specified file name, or returns <see langword="null"/> if it is not currently open.
    /// </summary>
    /// <param name="fileName">The name of the file to get.</param>
    /// <returns>The file view content, or <c>null</c> if the file is not open.</returns>
    IFileViewContent? GetOpenFile(FileName fileName);

    /// <summary>
    /// Opens the specified file and jumps to the specified file position.
    /// Line and column start counting at 1.
    /// </summary>
    /// <param name="fileName">The file to open.</param>
    /// <param name="line">The line number, starting at 1.</param>
    /// <param name="column">The column number, starting at 1.</param>
    /// <returns>The opened file view content.</returns>
    IFileViewContent? JumpToFilePosition(FileName fileName, int line, int column);

    #endregion OpenFile (ViewContent)

    #region Remove/Rename/Copy

    /// <summary>
    /// Removes a file, raising the appropriate events. This method may show message boxes.
    /// </summary>
    /// <param name="fileName">The file to remove.</param>
    /// <param name="isDirectory">Set to <see langword="true"/> if the file is a directory.</param>
    void RemoveFile(string fileName, bool isDirectory);

    /// <summary>
    /// Renames or moves a file, raising the appropriate events. This method may show message boxes.
    /// </summary>
    /// <param name="oldName">The current file name.</param>
    /// <param name="newName">The new file name.</param>
    /// <param name="isDirectory">Set to <see langword="true"/> if the file is a directory.</param>
    /// <returns><see langword="true"/> if the rename succeeded; otherwise, <see langword="false"/>.</returns>
    bool RenameFile(string oldName, string newName, bool isDirectory);

    /// <summary>
    /// Copies a file, raising the appropriate events. This method may show message boxes.
    /// </summary>
    /// <param name="oldName">The source file name.</param>
    /// <param name="newName">The destination file name.</param>
    /// <param name="isDirectory">Set to <see langword="true"/> if the file is a directory.</param>
    /// <param name="overwrite">Set to <see langword="true"/> to overwrite an existing file.</param>
    /// <returns><see langword="true"/> if the copy succeeded; otherwise, <see langword="false"/>.</returns>
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
