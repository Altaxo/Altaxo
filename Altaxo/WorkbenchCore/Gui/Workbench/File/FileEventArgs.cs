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

namespace Altaxo.Gui.Workbench
{
  /// <summary>
  /// Provides file-related event data.
  /// </summary>
  public class FileEventArgs : EventArgs
  {
    private string _fileName;
    private bool _isDirectory;

    /// <summary>
    /// Gets the affected file or directory name.
    /// </summary>
    public string FileName
    {
      get
      {
        return _fileName;
      }
    }

    /// <summary>
    /// Gets a value indicating whether the path refers to a directory.
    /// </summary>
    public bool IsDirectory
    {
      get
      {
        return _isDirectory;
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileEventArgs"/> class.
    /// </summary>
    /// <param name="fileName">The affected file or directory name.</param>
    /// <param name="isDirectory">Set to <see langword="true"/> if the path is a directory.</param>
    public FileEventArgs(string fileName, bool isDirectory)
    {
      this._fileName = fileName;
      this._isDirectory = isDirectory;
    }
  }

  /// <summary>
  /// Provides cancelable file-related event data.
  /// </summary>
  public class FileCancelEventArgs : FileEventArgs
  {
    private bool _cancel;

    /// <summary>
    /// Gets or sets a value indicating whether the operation should be canceled.
    /// </summary>
    public bool Cancel
    {
      get
      {
        return _cancel;
      }
      set
      {
        _cancel = value;
      }
    }

    private bool _operationAlreadyDone;

    /// <summary>
    /// Gets or sets a value indicating whether the operation has already been completed.
    /// </summary>
    public bool OperationAlreadyDone
    {
      get
      {
        return _operationAlreadyDone;
      }
      set
      {
        _operationAlreadyDone = value;
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileCancelEventArgs"/> class.
    /// </summary>
    /// <param name="fileName">The affected file or directory name.</param>
    /// <param name="isDirectory">Set to <see langword="true"/> if the path is a directory.</param>
    public FileCancelEventArgs(string fileName, bool isDirectory) : base(fileName, isDirectory)
    {
    }
  }
}
