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
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Collects progress using nested <see cref="IProgressReporter"/>s and provides it to a different thread using events.
  /// Note that the <see cref="IProgressReporter"/>(s) created by this instance support the progress value (0..1), but they do not support progress text.
  /// Use the <see cref="ProgressReporter"/> property of this instance to get the connected progress reporter.
  /// </summary>
  public sealed class ProgressCollector : INotifyPropertyChanged, IProgressMonitor
  {
    private readonly ISynchronizeInvoke _eventThread;
    private readonly IProgressReporter _rootReporter;
    private readonly LinkedList<string> _namedReporters = new LinkedList<string>();
    private readonly object _updateLock = new object();
    private string _taskName;
    private double _progressValue;
    private OperationStatus _statusValue;
    private bool _showingDialog;
    private bool _rootReporterIsDisposed;

    private bool _hasUpdateScheduled;
    private double _storedNewProgressValue = -1;
    private OperationStatus _storedNewStatusValue;

    public event EventHandler ProgressMonitorDisposed;

    public event PropertyChangedEventHandler PropertyChanged;

    public ProgressCollector(ISynchronizeInvoke eventThread, CancellationToken cancellationToken)
    {
      this._eventThread = eventThread ?? throw new ArgumentNullException(nameof(eventThread));
      this._rootReporter = new ReporterImpl(this, null, 1, cancellationToken);
    }

    private void OnPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public double Progress
    {
      get { return _progressValue; }
      private set
      {
        Debug.Assert(!_eventThread.InvokeRequired);
        if (_progressValue != value)
        {
          _progressValue = value;
          // Defensive programming: parallel processes like the build could change properties even
          // after the monitor is disposed (they shouldn't do that, but it could happen),
          // and we don't want to confuse consumers like the status bar by
          // raising events from disposed monitors.
          if (!_rootReporterIsDisposed)
            OnPropertyChanged(nameof(Progress));
        }
      }
    }

    #region IProgressMonitor implementation

    /// <summary>
    /// This monitor doesn't support progess text, thus here the return value is always false.
    /// </summary>
    public bool HasReportText { get { return false; } }

    /// <summary>
    /// Gets the report text. Since report text is not supported by this progress monitor, the return value is always an empty string.
    /// </summary>
    public string GetReportText()
    {
      return string.Empty;
    }

    /// <summary>Gets the progress as fraction. If you are not able to calculate the progress, this function should return <see cref="System.Double.NaN"/>.</summary>
    /// <returns>The progress as fraction value [0..1], or <see cref="System.Double.NaN"/>.</returns>
    public double GetProgressFraction()
    {
      return _progressValue;
    }

    #endregion IProgressMonitor implementation

    public bool ShowingDialog
    {
      get { return _showingDialog; }
      set
      {
        Debug.Assert(!_eventThread.InvokeRequired);
        if (_showingDialog != value)
        {
          _showingDialog = value;
          if (!_rootReporterIsDisposed)
            OnPropertyChanged(nameof(ShowingDialog));
        }
      }
    }

    public string TaskName
    {
      get { return _taskName; }
      private set
      {
        Debug.Assert(!_eventThread.InvokeRequired);
        if (_taskName != value)
        {
          _taskName = value;
          if (!_rootReporterIsDisposed)
            OnPropertyChanged(nameof(TaskName));
        }
      }
    }

    public OperationStatus Status
    {
      get { return _statusValue; }
      private set
      {
        Debug.Assert(!_eventThread.InvokeRequired);
        if (_statusValue != value)
        {
          _statusValue = value;
          if (!_rootReporterIsDisposed)
            OnPropertyChanged(nameof(Status));
        }
      }
    }

    public IProgressReporter ProgressReporter
    {
      get { return _rootReporter; }
    }

    /// <summary>
    /// Gets whether the root progress monitor was disposed.
    /// </summary>
    public bool ProgressMonitorIsDisposed
    {
      get { return _rootReporterIsDisposed; }
    }

    private void SetProgress(double newProgress)
    {
      // this method is always called within a lock(updateLock) block, so we don't
      // have to worry about thread safety when accessing hasUpdateScheduled and storedNewProgress

      _storedNewProgressValue = newProgress;
      ScheduleUpdate();
    }

    private void ScheduleUpdate()
    {
      // This test ensures that only 1 update is scheduled at a single point in time. If updates
      // come in faster than the GUI can process them, we'll skip some and directly update to the newest value.
      if (!_hasUpdateScheduled)
      {
        _hasUpdateScheduled = true;
        _eventThread.BeginInvoke(
            (Action)delegate
            {
              lock (_updateLock)
              {
                this.Progress = _storedNewProgressValue;
                this.Status = _storedNewStatusValue;
                _hasUpdateScheduled = false;
              }
            },
            null
        );
      }
    }

    private void SetStatus(OperationStatus newStatus)
    {
      // this method is always called within a lock(updateLock) block, so we don't
      // have to worry about thread safety when accessing hasUpdateScheduled and storedNewStatus

      _storedNewStatusValue = newStatus;
      ScheduleUpdate();
    }

    private void SetShowingDialog(bool newValue)
    {
      _eventThread.BeginInvoke(
          (Action)delegate
          { this.ShowingDialog = newValue; },
          null
      );
    }

    private void OnRootMonitorDisposed()
    {
      _eventThread.BeginInvoke(
          (Action)delegate
          {
            if (_rootReporterIsDisposed) // ignore double dispose
              return;
            _rootReporterIsDisposed = true;
            if (ProgressMonitorDisposed != null)
            {
              ProgressMonitorDisposed(this, EventArgs.Empty);
            }
          },
          null);
    }

    private void SetTaskName(string newName)
    {
      _eventThread.BeginInvoke(
          (Action)delegate
          { this.TaskName = newName; },
          null);
    }

    private LinkedListNode<string> RegisterNamedMonitor(string name)
    {
      lock (_namedReporters)
      {
        LinkedListNode<string> newEntry = _namedReporters.AddLast(name);
        if (_namedReporters.First == newEntry)
        {
          SetTaskName(name);
        }
        return newEntry;
      }
    }

    private void UnregisterNamedMonitor(LinkedListNode<string> nameEntry)
    {
      lock (_namedReporters)
      {
        bool wasFirst = _namedReporters.First == nameEntry;
        // Note: if Remove() crashes with "InvalidOperationException: The LinkedList node does not belong to current LinkedList.",
        // that's an indication that the progress monitor is being disposed multiple times concurrently.
        // (which is not allowed according to IProgressMonitor thread-safety documentation)
        _namedReporters.Remove(nameEntry);
        if (wasFirst)
          SetTaskName(_namedReporters.First != null ? _namedReporters.First.Value : null);
      }
    }

    private void ChangeName(LinkedListNode<string> nameEntry, string newName)
    {
      lock (_namedReporters)
      {
        if (_namedReporters.First == nameEntry)
          SetTaskName(newName);
        nameEntry.Value = newName;
      }
    }

    private sealed class ReporterImpl : IProgressReporter
    {
      private readonly ProgressCollector _progressCollector;
      private readonly ReporterImpl _parentReporter;
      private readonly double _scaleFactor;
      private readonly CancellationToken _cancellationToken;
      private LinkedListNode<string> _nameEntry;
      private double _currentProgressValue;
      private OperationStatus _localStatusValue, _currentStatusValue;
      private int _numberOfChildrenWithWarnings, _numberOfChildrenWithErrors;

      public ReporterImpl(ProgressCollector collector, ReporterImpl parent, double scaleFactor, CancellationToken cancellationToken)
      {
        this._progressCollector = collector;
        this._parentReporter = parent;
        this._scaleFactor = scaleFactor;
        this._cancellationToken = cancellationToken;
      }

      public bool ShowingDialog
      {
        get { return _progressCollector.ShowingDialog; }
        set { _progressCollector.SetShowingDialog(value); }
      }

      public string TaskName
      {
        get
        {
          if (_nameEntry != null)
            return _nameEntry.Value;
          else
            return null;
        }
        set
        {
          if (_nameEntry != null)
          {
            if (value == null)
            {
              _progressCollector.UnregisterNamedMonitor(_nameEntry);
              _nameEntry = null;
            }
            else
            {
              if (_nameEntry.Value != value)
                _progressCollector.ChangeName(_nameEntry, value);
            }
          }
          else
          {
            if (value != null)
              _nameEntry = _progressCollector.RegisterNamedMonitor(value);
          }
        }
      }

      public CancellationToken CancellationToken
      {
        get { return _cancellationToken; }
      }

      public double Progress
      {
        get { return _currentProgressValue; }
        set
        {
          lock (_progressCollector._updateLock)
          {
            UpdateProgress(value);
          }
        }
      }

      void IProgress<double>.Report(double value)
      {
        this.Progress = value;
      }

      private void UpdateProgress(double progress)
      {
        if (_parentReporter != null)
          _parentReporter.UpdateProgress(_parentReporter._currentProgressValue + (progress - this._currentProgressValue) * _scaleFactor);
        else
          _progressCollector.SetProgress(progress);
        this._currentProgressValue = progress;
      }

      public OperationStatus Status
      {
        get { return _localStatusValue; }
        set
        {
          if (_localStatusValue != value)
          {
            _localStatusValue = value;
            lock (_progressCollector._updateLock)
            {
              UpdateStatus();
            }
          }
        }
      }

      public bool ShouldReportNow
      {
        get
        {
          return true;
        }
      }

      public bool CancellationPending => CancellationToken.IsCancellationRequested;

      private void UpdateStatus()
      {
        OperationStatus oldStatus = _currentStatusValue;
        if (_numberOfChildrenWithErrors > 0)
          _currentStatusValue = OperationStatus.Error;
        else if (_numberOfChildrenWithWarnings > 0 && _localStatusValue != OperationStatus.Error)
          _currentStatusValue = OperationStatus.Warning;
        else
          _currentStatusValue = _localStatusValue;
        if (oldStatus != _currentStatusValue)
        {
          if (_parentReporter != null)
          {
            if (oldStatus == OperationStatus.Warning)
              _parentReporter._numberOfChildrenWithWarnings--;
            else if (oldStatus == OperationStatus.Error)
              _parentReporter._numberOfChildrenWithErrors--;

            if (_currentStatusValue == OperationStatus.Warning)
              _parentReporter._numberOfChildrenWithWarnings++;
            else if (_currentStatusValue == OperationStatus.Error)
              _parentReporter._numberOfChildrenWithErrors++;

            _parentReporter.UpdateStatus();
          }
          else
          {
            _progressCollector.SetStatus(_currentStatusValue);
          }
        }
      }

      public IProgressReporter CreateSubTask(double workAmount)
      {
        return new ReporterImpl(_progressCollector, this, workAmount, _cancellationToken);
      }

      public IProgressReporter CreateSubTask(double workAmount, CancellationToken cancellationToken)
      {
        return new ReporterImpl(_progressCollector, this, workAmount, cancellationToken);
      }

      public void Dispose()
      {
        this.TaskName = null;
        if (_parentReporter == null)
          _progressCollector.OnRootMonitorDisposed();
      }

      public void ReportProgress(string text)
      {
      }

      public void ReportProgress(string text, double progressValue)
      {
        Progress = progressValue;
      }
    }
  }
}
