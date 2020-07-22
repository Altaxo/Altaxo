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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Altaxo.Main;
using Altaxo.Main.Services;

namespace Altaxo.Gui.Workbench
{
  /// <summary>
  /// Represents an opened file.
  /// </summary>
  public abstract class OpenedFile : ICanBeDirty
  {
    protected IFileViewContent? _currentView;
    private bool _inLoadOperation;
    private bool _inSaveOperation;

    /// <summary>
    /// holds unsaved file content in memory when view containing the file was closed but no other view
    /// activated
    /// </summary>
    private byte[]? _fileData;

    #region IsDirty

    private bool _isDirty;

    public event EventHandler? IsDirtyChanged;

    /// <summary>
    /// Gets/sets if the file is has unsaved changes.
    /// </summary>
    public bool IsDirty
    {
      get { return _isDirty; }
      set
      {
        if (_isDirty != value)
        {
          _isDirty = value;

          IsDirtyChanged?.Invoke(this, EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Marks the file as dirty if it currently is not in a load operation.
    /// </summary>
    public virtual void MakeDirty()
    {
      if (!_inLoadOperation)
      {
        IsDirty = true;
      }
    }

    #endregion IsDirty

    private bool _isUntitled;

    /// <summary>
    /// Gets if the file is untitled. Untitled files show a "Save as" dialog when they are saved.
    /// </summary>
    public bool IsUntitled
    {
      get { return _isUntitled; }
      protected set { _isUntitled = value; }
    }

    private FileName? _fileName;

    /// <summary>
    /// Gets the name of the file.
    /// </summary>
    [MaybeNull]
    public FileName FileName
    {
      get { return _fileName; }
      set
      {
        if (_fileName is null || _fileName != value)
        {
          ChangeFileName(value);
        }
      }
    }

    protected virtual void ChangeFileName(FileName newValue)
    {
      Altaxo.Current.Dispatcher.VerifyAccess();

      _fileName = newValue;

      FileNameChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Occurs when the file name has changed.
    /// </summary>
    public event EventHandler? FileNameChanged;

    public abstract event EventHandler? FileClosed;

    /// <summary>
    /// Use this method to save the file to disk using a new name.
    /// </summary>
    public void SaveToDisk(FileName newFileName)
    {
      FileName = newFileName;
      IsUntitled = false;
      SaveToDisk();
    }

    public abstract void RegisterView(IFileViewContent view);

    public abstract void UnregisterView(IFileViewContent view);

    public virtual void CloseIfAllViewsClosed()
    {
    }

    /// <summary>
    /// Forces initialization of the specified view.
    /// </summary>
    public virtual void ForceInitializeView(IFileViewContent view)
    {
      if (view == null)
        throw new ArgumentNullException("view");

      bool success = false;
      try
      {
        if (_currentView != view)
        {
          if (_currentView == null)
          {
            SwitchedToView(view);
          }
          else
          {
            try
            {
              _inLoadOperation = true;
              using (Stream sourceStream = OpenRead())
              {
                view.Load(this, sourceStream);
              }
            }
            finally
            {
              _inLoadOperation = false;
            }
          }
        }
        success = true;
      }
      finally
      {
        // Only in case of exceptions:
        // (try-finally with bool is better than try-catch-rethrow because it causes the debugger to stop
        // at the original error location, not at the rethrow)
        if (!success)
        {
          view.Dispose();
        }
      }
    }

    /// <summary>
    /// Gets the list of view contents registered with this opened file.
    /// </summary>
    public abstract IList<IFileViewContent> RegisteredViewContents
    {
      get;
    }

    /// <summary>
    /// Gets the view content that currently edits this file.
    /// If there are multiple view contents registered, this returns the view content that was last
    /// active. The property might return null even if view contents are registered if the last active
    /// content was closed. In that case, the file is stored in-memory and loaded when one of the
    /// registered view contents becomes active.
    /// </summary>
    public IFileViewContent? CurrentView
    {
      get { return _currentView; }
    }

    /// <summary>
    /// Opens the file for reading.
    /// </summary>
    public virtual Stream OpenRead()
    {
      if (_fileData != null)
      {
        return new MemoryStream(_fileData, false);
      }
      else
      {
        return new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
      }
    }

    /// <summary>
    /// Sets the internally stored data to the specified byte array.
    /// This method should only be used when there is no current view or by the
    /// current view.
    /// </summary>
    /// <remarks>
    /// Use this method to specify the initial file content if you use a OpenedFile instance
    /// for a file that doesn't exist on disk but should be automatically created when a view
    /// with the file is saved, e.g. for .resx files created by the forms designer.
    /// </remarks>
    public virtual void SetData(byte[] fileData)
    {
      if (fileData == null)
        throw new ArgumentNullException("fileData");
      if (_inLoadOperation)
        throw new InvalidOperationException("SetData cannot be used while loading");
      if (_inSaveOperation)
        throw new InvalidOperationException("SetData cannot be used while saving");

      this._fileData = fileData;
    }

    /// <summary>
    /// Save the file to disk using the current name.
    /// </summary>
    public virtual void SaveToDisk()
    {
      if (IsUntitled)
        throw new InvalidOperationException("Cannot save an untitled file to disk!");

      Current.Log.Debug("Save " + FileName);
      bool safeSaving = Altaxo.Current.GetRequiredService<IFileService>().SaveUsingTemporaryFile && File.Exists(FileName);
      string saveAs = safeSaving ? FileName + ".bak" : FileName;
      using (var fs = new FileStream(saveAs, FileMode.Create, FileAccess.Write))
      {
        if (safeSaving)
        {
          // Copy creation time from source file
          // Because setting the time requires opening the file for write access,
          // we can't use System.IO.File.SetCreationTimeUtc for this, as it would open the file twice,
          // which causes problems when another process is monitoring the directory
          // and reading our new file as soon as we're done writing.
          // TODO Lellid: is this neccessary for Altaxo ?   --  NativeMethods.SetFileCreationTime(fs.SafeFileHandle, File.GetCreationTimeUtc(FileName));
        }
        if (_currentView != null)
        {
          SaveCurrentViewToStream(fs);
        }
        else if (_fileData != null)
        {
          fs.Write(_fileData, 0, _fileData.Length);
        }
      }
      if (safeSaving)
      {
        // TODO: we should probably use Win32 MoveFileEx to atomically move while replacing the old file
        File.Delete(FileName);
        try
        {
          File.Move(saveAs, FileName);
        }
        catch (UnauthorizedAccessException)
        {
          // sometime File.Move raise exception (TortoiseSVN, Anti-vir ?)
          // try again after short delay
          System.Threading.Thread.Sleep(250);
          File.Move(saveAs, FileName);
        }
      }
      IsDirty = false;
    }

    //		/// <summary>
    //		/// Called before saving the current view. This event is raised both when saving to disk and to memory (for switching between views).
    //		/// </summary>
    //		public event EventHandler SavingCurrentView;
    //
    //		/// <summary>
    //		/// Called after saving the current view. This event is raised both when saving to disk and to memory (for switching between views).
    //		/// </summary>
    //		public event EventHandler SavedCurrentView;

    private void SaveCurrentViewToStream(Stream stream)
    {
      //			if (SavingCurrentView != null)
      //				SavingCurrentView(this, EventArgs.Empty);
      _inSaveOperation = true;
      try
      {
        if (null != _currentView)
          _currentView.Save(this, stream);
      }
      finally
      {
        _inSaveOperation = false;
      }
      //			if (SavedCurrentView != null)
      //				SavedCurrentView(this, EventArgs.Empty);
    }

    protected void SaveCurrentView()
    {
      using (var memoryStream = new MemoryStream())
      {
        SaveCurrentViewToStream(memoryStream);
        _fileData = memoryStream.ToArray();
      }
    }

    public void SwitchedToView(IFileViewContent newView)
    {
      if (newView == null)
        throw new ArgumentNullException("newView");
      if (_currentView == newView)
        return;
      if (_currentView != null)
      {
        if (newView.SupportsSwitchToThisWithoutSaveLoad(this, _currentView)
                || _currentView.SupportsSwitchFromThisWithoutSaveLoad(this, newView))
        {
          // switch without Save/Load
          _currentView.SwitchFromThisWithoutSaveLoad(this, newView);
          newView.SwitchToThisWithoutSaveLoad(this, _currentView);

          _currentView = newView;
          return;
        }
        SaveCurrentView();
      }
      try
      {
        _inLoadOperation = true;
        var memento = GetMemento(newView);
        using (Stream sourceStream = OpenRead())
        {
          var oldView = _currentView;
          bool success = false;
          try
          {
            _currentView = newView;
            // don't reset fileData if the file is untitled, because OpenRead() wouldn't be able to read it otherwise
            if (IsUntitled == false)
              _fileData = null;
            newView.Load(this, sourceStream);
            success = true;
          }
          finally
          {
            // Use finally instead of catch+rethrow so that the debugger
            // breaks at the original crash location.
            if (!success)
            {
              // stay with old view in case of exceptions
              _currentView = oldView;
            }
          }
        }
        RestoreMemento(newView, memento);
      }
      finally
      {
        _inLoadOperation = false;
      }
    }

    public virtual void ReloadFromDisk()
    {
      if (FileName is null)
        throw new InvalidOperationException($"{nameof(FileName)} is null");

      var r = FileUtility.ObservedLoad(ReloadFromDiskInternal, FileName);
      if (r == FileOperationResult.Failed)
      {
        if (_currentView != null)
        {
          _currentView.CloseCommand.Execute(_currentView);
        }
      }
    }

    private void ReloadFromDiskInternal()
    {
      _fileData = null;
      if (_currentView != null)
      {
        try
        {
          _inLoadOperation = true;
          var memento = GetMemento(_currentView);
          using (Stream sourceStream = OpenRead())
          {
            _currentView.Load(this, sourceStream);
          }
          IsDirty = false;
          RestoreMemento(_currentView, memento);
        }
        finally
        {
          _inLoadOperation = false;
        }
      }
    }

    private static object? GetMemento(IFileViewContent viewContent)
    {
      return viewContent.GetService<IMementoCapable>()?.CreateMemento();
    }

    private static void RestoreMemento(IFileViewContent viewContent, object? memento)
    {
      if (memento != null)
      {
        ((IMementoCapable)viewContent).SetMemento(memento);
      }
    }
  }
}
