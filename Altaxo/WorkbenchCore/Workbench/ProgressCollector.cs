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
using Altaxo.Main;

namespace Altaxo.Workbench
{
  /// <summary>
  /// Collects progress using nested IProgressMonitors and provides it to a different thread using events.
  /// </summary>
  public sealed class ProgressCollector : INotifyPropertyChanged
  {
    private readonly ISynchronizeInvoke _eventThread;
    private readonly MonitorImpl _root;
    private readonly LinkedList<string> _namedMonitors = new LinkedList<string>();
    private readonly object _updateLock = new object();

    private string? _taskName;
    private double _progress;
    private OperationStatus _status;
    private bool _showingDialog;
    private bool _rootMonitorIsDisposed;

    public ProgressCollector(ISynchronizeInvoke eventThread, CancellationToken cancellationToken)
    {
      if (eventThread is null)
        throw new ArgumentNullException(nameof(eventThread));
      this._eventThread = eventThread;
      _root = new MonitorImpl(this, null, 1, cancellationToken);
    }

    public event EventHandler? ProgressMonitorDisposed;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
      if (PropertyChanged is not null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    public double Progress
    {
      get { return _progress; }
      private set
      {
        Debug.Assert(!_eventThread.InvokeRequired);
        if (_progress != value)
        {
          _progress = value;
          // Defensive programming: parallel processes like the build could change properites even
          // after the monitor is disposed (they shouldn't do that, but it could happen),
          // and we don't want to confuse consumers like the status bar by
          // raising events from disposed monitors.
          if (!_rootMonitorIsDisposed)
            OnPropertyChanged("Progress");
        }
      }
    }

    public bool ShowingDialog
    {
      get { return _showingDialog; }
      set
      {
        Debug.Assert(!_eventThread.InvokeRequired);
        if (_showingDialog != value)
        {
          _showingDialog = value;
          if (!_rootMonitorIsDisposed)
            OnPropertyChanged("ShowingDialog");
        }
      }
    }

    public string? TaskName
    {
      get { return _taskName; }
      private set
      {
        Debug.Assert(!_eventThread.InvokeRequired);
        if (_taskName != value)
        {
          _taskName = value;
          if (!_rootMonitorIsDisposed)
            OnPropertyChanged("TaskName");
        }
      }
    }

    public OperationStatus Status
    {
      get { return _status; }
      private set
      {
        Debug.Assert(!_eventThread.InvokeRequired);
        if (_status != value)
        {
          _status = value;
          if (!_rootMonitorIsDisposed)
            OnPropertyChanged("Status");
        }
      }
    }

    public IProgressMonitor ProgressMonitor
    {
      get { return _root; }
    }

    /// <summary>
    /// Gets whether the root progress monitor was disposed.
    /// </summary>
    public bool ProgressMonitorIsDisposed
    {
      get { return _rootMonitorIsDisposed; }
    }

    private bool hasUpdateScheduled;
    private double storedNewProgress = -1;
    private OperationStatus storedNewStatus;

    private void SetProgress(double newProgress)
    {
      // this method is always called within a lock(updateLock) block, so we don't
      // have to worry about thread safety when accessing hasUpdateScheduled and storedNewProgress

      storedNewProgress = newProgress;
      ScheduleUpdate();
    }

    private void ScheduleUpdate()
    {
      // This test ensures that only 1 update is scheduled at a single point in time. If updates
      // come in faster than the GUI can process them, we'll skip some and directly update to the newest value.
      if (!hasUpdateScheduled)
      {
        hasUpdateScheduled = true;
        _eventThread.BeginInvoke(
          (Action)delegate
          {
            lock (_updateLock)
            {
              Progress = storedNewProgress;
              Status = storedNewStatus;
              hasUpdateScheduled = false;
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

      storedNewStatus = newStatus;
      ScheduleUpdate();
    }

    private void SetShowingDialog(bool newValue)
    {
      _eventThread.BeginInvoke(
        (Action)delegate
        { ShowingDialog = newValue; },
        null
      );
    }

    private void OnRootMonitorDisposed()
    {
      _eventThread.BeginInvoke(
        (Action)delegate
        {
          if (_rootMonitorIsDisposed) // ignore double dispose
            return;
          _rootMonitorIsDisposed = true;
          if (ProgressMonitorDisposed is not null)
          {
            ProgressMonitorDisposed(this, EventArgs.Empty);
          }
        },
        null);
    }

    private void SetTaskName(string? newName)
    {
      _eventThread.BeginInvoke(
        (Action)delegate
        { TaskName = newName; },
        null);
    }

    private LinkedListNode<string> RegisterNamedMonitor(string name)
    {
      lock (_namedMonitors)
      {
        LinkedListNode<string> newEntry = _namedMonitors.AddLast(name);
        if (_namedMonitors.First == newEntry)
        {
          SetTaskName(name);
        }
        return newEntry;
      }
    }

    private void UnregisterNamedMonitor(LinkedListNode<string> nameEntry)
    {
      lock (_namedMonitors)
      {
        bool wasFirst = _namedMonitors.First == nameEntry;
        // Note: if Remove() crashes with "InvalidOperationException: The LinkedList node does not belong to current LinkedList.",
        // that's an indication that the progress monitor is being disposed multiple times concurrently.
        // (which is not allowed according to IProgressMonitor thread-safety documentation)
        _namedMonitors.Remove(nameEntry);
        if (wasFirst)
          SetTaskName(_namedMonitors.First is not null ? _namedMonitors.First.Value : null);
      }
    }

    private void ChangeName(LinkedListNode<string> nameEntry, string newName)
    {
      lock (_namedMonitors)
      {
        if (_namedMonitors.First == nameEntry)
          SetTaskName(newName);
        nameEntry.Value = newName;
      }
    }

    private sealed class MonitorImpl : IProgressMonitor
    {
      private readonly ProgressCollector _collector;
      private readonly MonitorImpl? _parent;
      private readonly double _scaleFactor;
      private readonly CancellationToken _cancellationToken;
      private LinkedListNode<string>? _nameEntry;
      private double currentProgress;
      private OperationStatus localStatus, currentStatus;
      private int childrenWithWarnings, childrenWithErrors;

      public MonitorImpl(ProgressCollector collector, MonitorImpl? parent, double scaleFactor, CancellationToken cancellationToken)
      {
        this._collector = collector;
        this._parent = parent;
        this._scaleFactor = scaleFactor;
        this._cancellationToken = cancellationToken;
      }

      public bool ShowingDialog
      {
        get { return _collector.ShowingDialog; }
        set { _collector.SetShowingDialog(value); }
      }

      public string? TaskName
      {
        get
        {
          if (_nameEntry is not null)
            return _nameEntry.Value;
          else
            return null;
        }
        set
        {
          if (_nameEntry is not null)
          {
            if (value is null)
            {
              _collector.UnregisterNamedMonitor(_nameEntry);
              _nameEntry = null;
            }
            else
            {
              if (_nameEntry.Value != value)
                _collector.ChangeName(_nameEntry, value);
            }
          }
          else
          {
            if (value is not null)
              _nameEntry = _collector.RegisterNamedMonitor(value);
          }
        }
      }

      public CancellationToken CancellationToken
      {
        get { return _cancellationToken; }
      }

      public double Progress
      {
        get { return currentProgress; }
        set
        {
          lock (_collector._updateLock)
          {
            UpdateProgress(value);
          }
        }
      }

      void IProgress<double>.Report(double value)
      {
        Progress = value;
      }

      private void UpdateProgress(double progress)
      {
        if (_parent is not null)
          _parent.UpdateProgress(_parent.currentProgress + (progress - currentProgress) * _scaleFactor);
        else
          _collector.SetProgress(progress);
        currentProgress = progress;
      }

      public OperationStatus Status
      {
        get { return localStatus; }
        set
        {
          if (localStatus != value)
          {
            localStatus = value;
            lock (_collector._updateLock)
            {
              UpdateStatus();
            }
          }
        }
      }

      private void UpdateStatus()
      {
        OperationStatus oldStatus = currentStatus;
        if (childrenWithErrors > 0)
          currentStatus = OperationStatus.Error;
        else if (childrenWithWarnings > 0 && localStatus != OperationStatus.Error)
          currentStatus = OperationStatus.Warning;
        else
          currentStatus = localStatus;
        if (oldStatus != currentStatus)
        {
          if (_parent is not null)
          {
            if (oldStatus == OperationStatus.Warning)
              _parent.childrenWithWarnings--;
            else if (oldStatus == OperationStatus.Error)
              _parent.childrenWithErrors--;

            if (currentStatus == OperationStatus.Warning)
              _parent.childrenWithWarnings++;
            else if (currentStatus == OperationStatus.Error)
              _parent.childrenWithErrors++;

            _parent.UpdateStatus();
          }
          else
          {
            _collector.SetStatus(currentStatus);
          }
        }
      }

      public IProgressMonitor CreateSubTask(double workAmount)
      {
        return new MonitorImpl(_collector, this, workAmount, _cancellationToken);
      }

      public IProgressMonitor CreateSubTask(double workAmount, CancellationToken cancellationToken)
      {
        return new MonitorImpl(_collector, this, workAmount, cancellationToken);
      }

      public void Dispose()
      {
        TaskName = null;
        if (_parent is null)
          _collector.OnRootMonitorDisposed();
      }
    }
  }
}
