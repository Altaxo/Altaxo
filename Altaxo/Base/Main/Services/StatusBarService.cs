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

#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Altaxo.Gui;
using Altaxo.Gui.Workbench;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Implements a standard <see cref="IStatusBarService"/>. This can be used also as a controller for the Gui component of the status bar.
  /// </summary>
  /// <seealso cref="IStatusBarService" />
  /// <seealso cref="IMVCController" />
  [ExpectedTypeOfView(typeof(IStatusBarView))]
  public class StatusBarService : IStatusBarService, IMVCController
  {
    private IStatusBarView? _statusBarView;
    protected bool _isStatusBarVisible = true;
    private bool _isDisposed;

    public StatusBarService()
    {

    }

    public StatusBarService(IStatusBarView statusBar)
    {
      _statusBarView = statusBar ?? throw new ArgumentNullException(nameof(statusBar));
    }

    public bool IsVisible
    {
      get
      {
        return _isStatusBarVisible;
      }
      set
      {
        if (_statusBarView is { } view)
          view.IsStatusBarVisible = value;
        _isStatusBarVisible = value;
      }
    }

    public virtual void SetRightCornerText(string text)
    {
      if (_statusBarView is { } view)
        view.ModeStatusBarPanelContent = text;
    }

    public virtual void SetCaretPosition(int x, int y, int charOffset)
    {
      if (_statusBarView is { } view)
      {
        view.CursorStatusBarPanelContent = StringParser.Parse(
          "${res:StatusBarService.CursorStatusBarPanelText}",
          new StringTagPair("Line", string.Format("{0,-10}", y)),
          new StringTagPair("Column", string.Format("{0,-5}", x)),
          new StringTagPair("Character", string.Format("{0,-5}", charOffset))
      );
      }
    }

    public void SetSelectionSingle(int length)
    {
      if (_statusBarView is { } view)
      {
        if (length > 0)
        {
          view.SelectionStatusBarPanelContent = StringParser.Parse(
              "${res:StatusBarService.SelectionStatusBarPanelTextSingle}",
              new StringTagPair("Length", string.Format("{0,-10}", length)));
        }
        else
        {
          view.SelectionStatusBarPanelContent = null;
        }
      }
    }

    public void SetSelectionMulti(int rows, int cols)
    {
      if (_statusBarView is { } view)
      {
        if (rows > 0 && cols > 0)
        {
          view.SelectionStatusBarPanelContent = StringParser.Parse(
              "${res:StatusBarService.SelectionStatusBarPanelTextMulti}",
              new StringTagPair("Rows", string.Format("{0}", rows)),
              new StringTagPair("Cols", string.Format("{0}", cols)),
              new StringTagPair("Total", string.Format("{0}", rows * cols)));
        }
        else
        {
          view.SelectionStatusBarPanelContent = null;
        }
      }
    }

    public void SetInsertMode(bool insertMode)
    {
      if (_statusBarView is { } view)
        view.ModeStatusBarPanelContent = insertMode ? StringParser.Parse("${res:StatusBarService.CaretModes.Insert}") : StringParser.Parse("${res:StatusBarService.CaretModes.Overwrite}");
    }

    public void SetMessage(string message, bool highlighted, object? icon)
    {
      if (_statusBarView is { } view)
        view.SetMessage(StringParser.Parse(message), highlighted, icon);
    }

    #region Progress Monitor

    private Stack<ProgressCollector?> _waitingProgresses = new Stack<ProgressCollector?>();
    private ProgressCollector? _currentProgress;

    private void ThrowIfDisposed()
    {
      if (_isDisposed)
        throw new ObjectDisposedException(this.GetType().Name);
    }

    public IProgressReporter CreateProgressReporter(CancellationToken cancellationToken = default(CancellationToken))
    {
      var progress = new ProgressCollector(Current.Dispatcher.SynchronizingObject, cancellationToken);
      AddProgress(progress);
      return progress.ProgressReporter;
    }

    public void AddProgress(ProgressCollector progress)
    {
      if (progress is null)
        throw new ArgumentNullException(nameof(progress));
      Current.Dispatcher.VerifyAccess();
      if (_currentProgress is not null)
      {
        _currentProgress.ProgressMonitorDisposed -= progress_ProgressMonitorDisposed;
        _currentProgress.PropertyChanged -= EhProgress_PropertyChanged;
      }
      _waitingProgresses.Push(_currentProgress); // push even if currentProgress==null
      SetActiveProgress(progress);
    }

    private void SetActiveProgress(ProgressCollector? progress)
    {
      Current.Dispatcher.VerifyAccess();
      _currentProgress = progress;
      if (progress is null)
      {
        _statusBarView?.HideProgress();
        return;
      }

      progress.ProgressMonitorDisposed += progress_ProgressMonitorDisposed;
      if (progress.ProgressMonitorIsDisposed)
      {
        progress_ProgressMonitorDisposed(progress, EventArgs.Empty);
        return;
      }
      progress.PropertyChanged += EhProgress_PropertyChanged;
    }

    private void EhProgress_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (_currentProgress is null)
        throw new InvalidProgramException();
      if (!(_currentProgress == sender))
        throw new InvalidProgramException();

      _statusBarView?.DisplayProgress(_currentProgress.TaskName, _currentProgress.Progress, _currentProgress.Status);
    }

    private void progress_ProgressMonitorDisposed(object? sender, EventArgs e)
    {
      Debug.Assert(sender == _currentProgress);
      SetActiveProgress(_waitingProgresses.Pop()); // stack is never empty: we push null as first element
    }

    #endregion Progress Monitor

    #region IMVCController

    public object? ViewObject
    {
      get
      {
        return _statusBarView;
      }
      set
      {
        ThrowIfDisposed();
        if (value is IStatusBarView view)
          _statusBarView = view;
        else if (value is null)
          throw new ArgumentNullException(nameof(ViewObject));
        else
          throw new ArgumentException("Wrong type", nameof(ViewObject));

      }
    }

    public object ModelObject { get { return new object(); } }

    public virtual void Dispose()
    {
      _isDisposed = true;
      _statusBarView = null!;
    }

    #endregion IMVCController
  }
}
