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
  public class TimedBackgroundMonitor : IProgressReporter
  {
    private System.Timers.Timer _timer = new System.Timers.Timer(200);
    private bool _shouldReport;
#pragma warning disable CS0649
    private CancellationToken _cancellationToken;
#pragma warning restore CS0649
    private string _reportText;
    private double _progressFraction = double.NaN;
    private OperationStatus _operationStatus;
    private string _taskName;

    public event System.Timers.ElapsedEventHandler? Elapsed;

    public TimedBackgroundMonitor()
    {
      _reportText = string.Empty;
      _taskName = nameof(TimedBackgroundMonitor);
      _timer.Elapsed += new System.Timers.ElapsedEventHandler(EhTimerElapsed);
    }

    public void Start()
    {
      _timer.Start();
    }

    public void Stop()
    {
      _timer.Stop();
    }

    public System.ComponentModel.ISynchronizeInvoke SynchronizingObject
    {
      get { return _timer.SynchronizingObject; }
      set { _timer.SynchronizingObject = value; }
    }

    #region IBackgroundMonitor Members

    public bool ShouldReportNow
    {
      get
      {
        return _shouldReport;
      }
    }

    public void ReportProgress(string text)
    {
      _shouldReport = false;
      _reportText = text;
    }

    public void ReportProgress(string text, double progressFraction)
    {
      _shouldReport = false;
      _reportText = text;
      _progressFraction = progressFraction;
    }

    public string ReportText
    {
      set { _reportText = value; }
      get { return _reportText; }
    }

    public double ProgressFraction
    {
      get { return _progressFraction; }
      set { _progressFraction = value; }
    }

    public bool CancellationPending
    {
      get
      {
        return _cancellationToken.IsCancellationRequested;
      }
    }

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

    public CancellationToken CancellationToken
    {
      get
      {
        return _cancellationToken;
      }
    }

    #endregion IBackgroundMonitor Members

    private void EhTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
      _shouldReport = true;
      Elapsed?.Invoke(sender, e);
    }

    public IProgressReporter CreateSubTask(double workAmount)
    {
      throw new NotImplementedException();
    }

    public IProgressReporter CreateSubTask(double workAmount, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public void Report(double value)
    {
      Progress = value;
    }

    public void Dispose()
    {
      _timer?.Dispose();
    }
  }
}
