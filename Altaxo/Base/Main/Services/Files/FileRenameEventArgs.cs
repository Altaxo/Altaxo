﻿// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
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

#nullable enable
using System;

namespace Altaxo.Main.Services
{
  public class FileRenamingEventArgs : FileRenameEventArgs
  {
    private bool cancel;

    public bool Cancel
    {
      get
      {
        return cancel;
      }
      set
      {
        cancel = value;
      }
    }

    private bool operationAlreadyDone;

    public bool OperationAlreadyDone
    {
      get
      {
        return operationAlreadyDone;
      }
      set
      {
        operationAlreadyDone = value;
      }
    }

    public FileRenamingEventArgs(string sourceFile, string targetFile, bool isDirectory)
        : base(sourceFile, targetFile, isDirectory)
    {
    }
  }

  public class FileRenameEventArgs : EventArgs
  {
    private bool isDirectory;

    private string sourceFile;
    private string targetFile;

    public string SourceFile
    {
      get
      {
        return sourceFile;
      }
    }

    public string TargetFile
    {
      get
      {
        return targetFile;
      }
    }

    public bool IsDirectory
    {
      get
      {
        return isDirectory;
      }
    }

    public FileRenameEventArgs(string sourceFile, string targetFile, bool isDirectory)
    {
      this.sourceFile = sourceFile;
      this.targetFile = targetFile;
      this.isDirectory = isDirectory;
    }
  }
}
