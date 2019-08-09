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
using System.Threading;
using System.Windows;
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
    protected IStatusBarView _statusBarView;
    protected bool _isStatusBarVisible = true;

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
        _statusBarView.IsStatusBarVisible = value;
        _isStatusBarVisible = value;
      }
    }

    public virtual void SetRightCornerText(string text)
    {
      _statusBarView.ModeStatusBarPanelContent = text;
    }

    public virtual void SetCaretPosition(int x, int y, int charOffset)
    {
      _statusBarView.CursorStatusBarPanelContent = StringParser.Parse(
          "${res:StatusBarService.CursorStatusBarPanelText}",
          new StringTagPair("Line", string.Format("{0,-10}", y)),
          new StringTagPair("Column", string.Format("{0,-5}", x)),
          new StringTagPair("Character", string.Format("{0,-5}", charOffset))
      );
    }

    public void SetSelectionSingle(int length)
    {
      if (length > 0)
      {
        _statusBarView.SelectionStatusBarPanelContent = StringParser.Parse(
            "${res:StatusBarService.SelectionStatusBarPanelTextSingle}",
            new StringTagPair("Length", string.Format("{0,-10}", length)));
      }
      else
      {
        _statusBarView.SelectionStatusBarPanelContent = null;
      }
    }

    public void SetSelectionMulti(int rows, int cols)
    {
      if (rows > 0 && cols > 0)
      {
        _statusBarView.SelectionStatusBarPanelContent = StringParser.Parse(
            "${res:StatusBarService.SelectionStatusBarPanelTextMulti}",
            new StringTagPair("Rows", string.Format("{0}", rows)),
            new StringTagPair("Cols", string.Format("{0}", cols)),
            new StringTagPair("Total", string.Format("{0}", rows * cols)));
      }
      else
      {
        _statusBarView.SelectionStatusBarPanelContent = null;
      }
    }

    public void SetInsertMode(bool insertMode)
    {
      _statusBarView.ModeStatusBarPanelContent = insertMode ? StringParser.Parse("${res:StatusBarService.CaretModes.Insert}") : StringParser.Parse("${res:StatusBarService.CaretModes.Overwrite}");
    }

    public void SetMessage(string message, bool highlighted, object icon)
    {
      _statusBarView.SetMessage(StringParser.Parse(message), highlighted, icon);
    }

    #region Progress Monitor

    private Stack<ProgressCollector> waitingProgresses = new Stack<ProgressCollector>();
    private ProgressCollector currentProgress;

    public IProgressReporter CreateProgressReporter(CancellationToken cancellationToken = default(CancellationToken))
    {
      var progress = new ProgressCollector(Current.Dispatcher.SynchronizingObject, cancellationToken);
      AddProgress(progress);
      return progress.ProgressReporter;
    }

    public void AddProgress(ProgressCollector progress)
    {
      if (progress == null)
        throw new ArgumentNullException(nameof(progress));
      Current.Dispatcher.VerifyAccess();
      if (currentProgress != null)
      {
        currentProgress.ProgressMonitorDisposed -= progress_ProgressMonitorDisposed;
        currentProgress.PropertyChanged -= progress_PropertyChanged;
      }
      waitingProgresses.Push(currentProgress); // push even if currentProgress==null
      SetActiveProgress(progress);
    }

    private void SetActiveProgress(ProgressCollector progress)
    {
      Current.Dispatcher.VerifyAccess();
      currentProgress = progress;
      if (progress == null)
      {
        _statusBarView.HideProgress();
        return;
      }

      progress.ProgressMonitorDisposed += progress_ProgressMonitorDisposed;
      if (progress.ProgressMonitorIsDisposed)
      {
        progress_ProgressMonitorDisposed(progress, null);
        return;
      }
      progress.PropertyChanged += progress_PropertyChanged;
    }

    private void progress_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      Debug.Assert(sender == currentProgress);
      _statusBarView.DisplayProgress(currentProgress.TaskName, currentProgress.Progress, currentProgress.Status);
    }

    private void progress_ProgressMonitorDisposed(object sender, EventArgs e)
    {
      Debug.Assert(sender == currentProgress);
      SetActiveProgress(waitingProgresses.Pop()); // stack is never empty: we push null as first element
    }

    #endregion Progress Monitor

    #region IMVCController

    public object ViewObject
    {
      get
      {
        return _statusBarView;
      }
      set
      {
        _statusBarView = value as IStatusBarView;
      }
    }

    public object ModelObject { get { return null; } }

    public virtual void Dispose()
    {
      _statusBarView = null;
    }

    #endregion IMVCController
  }
}
