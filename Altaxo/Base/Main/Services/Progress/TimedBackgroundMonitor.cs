#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

#nullable enable
using System;
using System.Threading;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Provides a timer-driven progress reporter for background tasks.
  /// </summary>
  public class TimedBackgroundMonitor : IProgressReporter
  {
    private System.Timers.Timer _timer = new System.Timers.Timer(200);
    private bool _shouldReport;
#pragma warning disable CS0649
    private CancellationToken _cancellationTokenSoft;
    private CancellationToken _cancellationTokenHard;
#pragma warning restore CS0649
    private string _reportText;
    private double _progressFraction = double.NaN;
    private OperationStatus _operationStatus;
    private string _taskName;

    /// <summary>
    /// Occurs whenever the internal timer elapses.
    /// </summary>
    public event System.Timers.ElapsedEventHandler? Elapsed;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimedBackgroundMonitor"/> class.
    /// </summary>
    public TimedBackgroundMonitor()
    {
      _reportText = string.Empty;
      _taskName = nameof(TimedBackgroundMonitor);
      _timer.Elapsed += new System.Timers.ElapsedEventHandler(EhTimerElapsed);
    }

    /// <summary>
    /// Starts the internal timer.
    /// </summary>
    public void Start()
    {
      _timer.Start();
    }

    /// <summary>
    /// Stops the internal timer.
    /// </summary>
    public void Stop()
    {
      _timer.Stop();
    }

    /// <summary>
    /// Gets or sets the object used to marshal timer events to a specific thread.
    /// </summary>
    public System.ComponentModel.ISynchronizeInvoke SynchronizingObject
    {
      get { return _timer.SynchronizingObject; }
      set { _timer.SynchronizingObject = value; }
    }

    #region IBackgroundMonitor Members

    /// <inheritdoc/>
    public bool ShouldReportNow
    {
      get
      {
        return _shouldReport;
      }
    }

    /// <inheritdoc/>
    public void ReportProgress(string text)
    {
      _shouldReport = false;
      _reportText = text;
    }

    /// <inheritdoc/>
    public void ReportProgress(string text, double progressFraction)
    {
      _shouldReport = false;
      _reportText = text;
      _progressFraction = progressFraction;
    }

    /// <summary>
    /// Gets or sets the latest progress text.
    /// </summary>
    public string ReportText
    {
      set { _reportText = value; }
      get { return _reportText; }
    }

    /// <summary>
    /// Gets or sets the latest progress fraction.
    /// </summary>
    public double ProgressFraction
    {
      get { return _progressFraction; }
      set { _progressFraction = value; }
    }

    /// <inheritdoc/>
    public bool CancellationPending
    {
      get
      {
        return _cancellationTokenSoft.IsCancellationRequested || _cancellationTokenHard.IsCancellationRequested;
      }
    }

    /// <summary>
    /// Gets or sets the current progress fraction.
    /// </summary>
    public double Progress
    {
      get
      {
        return _progressFraction;
      }
      set
      {
        _progressFraction = value;
      }
    }

    /// <summary>
    /// Gets or sets the current operation status.
    /// </summary>
    public OperationStatus Status
    {
      get
      {
        return _operationStatus;
      }
      set
      {
        _operationStatus = value;
      }
    }

    /// <inheritdoc/>
    public string TaskName
    {
      get
      {
        return _taskName;
      }
      set
      {
        _taskName = value;
      }
    }

    /// <inheritdoc/>
    public CancellationToken CancellationToken
    {
      get
      {
        return _cancellationTokenSoft;
      }
    }

    /// <inheritdoc/>
    public CancellationToken CancellationTokenHard
    {
      get
      {
        return _cancellationTokenHard;
      }
    }

    #endregion IBackgroundMonitor Members

    private void EhTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
      _shouldReport = true;
      Elapsed?.Invoke(sender, e);
    }

    /// <inheritdoc/>
    public IProgressReporter GetSubTask(double workAmount)
    {
      throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public IProgressReporter GetSubTask(double workAmount, CancellationToken cancellationTokenSoft, CancellationToken cancellationTokenHard)
    {
      throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void Report(double value)
    {
      Progress = value;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
      _timer?.Dispose();
    }

    void IProgress<string>.Report(string value)
    {
    }

    void IProgress<(string text, double progressFraction)>.Report((string text, double progressFraction) value)
    {
      Progress = value.progressFraction;
    }

    /// <inheritdoc/>
    public void ReportStatus(OperationStatus status)
    {
    }
  }
}
