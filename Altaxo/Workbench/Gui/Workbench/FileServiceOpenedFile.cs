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
using System.Diagnostics;
using Altaxo.Main.Services;

namespace Altaxo.Gui.Workbench
{
  internal sealed class FileServiceOpenedFile : OpenedFile
  {
    private readonly FileService _fileService;
    private List<IFileViewContent> _registeredViews = new List<IFileViewContent>();
    //private FileChangeWatcher fileChangeWatcher;

    protected override void ChangeFileName(FileName newValue)
    {
      _fileService.OpenedFileFileNameChange(this, FileName, newValue);
      base.ChangeFileName(newValue);
    }

    internal FileServiceOpenedFile(FileService fileService, FileName fileName)
    {
      this._fileService = fileService;
      FileName = fileName;
      IsUntitled = false;
      //fileChangeWatcher = new FileChangeWatcher(this);
    }

    internal FileServiceOpenedFile(FileService fileService, byte[] fileData)
    {
      this._fileService = fileService;
      SetData(fileData);
      IsUntitled = true;
      MakeDirty();
      //fileChangeWatcher = new FileChangeWatcher(this);
    }

    /// <summary>
    /// Gets the list of view contents registered with this opened file.
    /// </summary>
    public override IList<IFileViewContent> RegisteredViewContents
    {
      get { return _registeredViews.AsReadOnly(); }
    }

    public override void ForceInitializeView(IFileViewContent view)
    {
      if (view is null)
        throw new ArgumentNullException("view");
      if (!_registeredViews.Contains(view))
        throw new ArgumentException("registeredViews must contain view");

      base.ForceInitializeView(view);
    }

    public override void RegisterView(IFileViewContent view)
    {
      if (view is null)
        throw new ArgumentNullException("view");
      if (_registeredViews.Contains(view))
        throw new ArgumentException("registeredViews already contains view");

      _registeredViews.Add(view);

      if (Altaxo.Current.GetService<IWorkbench>() is not null)
      {
        Altaxo.Current.GetRequiredService<IWorkbenchEx>().ActiveViewContentChanged += WorkbenchActiveViewContentChanged;
        if (Altaxo.Current.Workbench.ActiveViewContent == view)
        {
          SwitchedToView(view);
        }
      }
#if DEBUG
      view.Disposed += ViewDisposed;
#endif
    }

    public override void UnregisterView(IFileViewContent view)
    {
      if (view is null)
        throw new ArgumentNullException("view");
      Debug.Assert(_registeredViews.Contains(view));

      if (Altaxo.Current.GetService<IWorkbench>() is not null)
      {
        Altaxo.Current.GetRequiredService<IWorkbenchEx>().ActiveViewContentChanged -= WorkbenchActiveViewContentChanged;
      }
#if DEBUG
      view.Disposed -= ViewDisposed;
#endif

      _registeredViews.Remove(view);
      if (_registeredViews.Count > 0)
      {
        if (_currentView == view)
        {
          SaveCurrentView();
          _currentView = null;
        }
      }
      else
      {
        // all views to the file were closed
        CloseIfAllViewsClosed();
      }
    }

    public override void CloseIfAllViewsClosed()
    {
      if (_registeredViews.Count == 0)
      {
        bool wasDirty = IsDirty;
        _fileService.OpenedFileClosed(this);

        FileClosed?.Invoke(this, EventArgs.Empty);
      }
    }

#if DEBUG

    private void ViewDisposed(object? sender, EventArgs e)
    {
      Debug.Fail("View was disposed while still registered with OpenedFile!");
    }

#endif

    private void WorkbenchActiveViewContentChanged(object? sender, EventArgs e)
    {
      var newView = Altaxo.Current.GetRequiredService<IWorkbench>().ActiveViewContent as IFileViewContent;

      if (newView is null || !_registeredViews.Contains(newView))
        return;

      SwitchedToView(newView);
    }

    public override void SaveToDisk()
    {
      try
      {
        base.SaveToDisk();
      }
      finally
      {
      }
    }

    public override event EventHandler? FileClosed;
  }
}
