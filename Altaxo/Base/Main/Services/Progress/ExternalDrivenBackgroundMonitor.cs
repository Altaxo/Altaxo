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
  /// Monitor, which gives the request to report the progress by calling <see cref="SetShouldReportNow"/>.
  /// </summary>
  /// <seealso cref="Altaxo.Main.Services.IExternalDrivenBackgroundMonitor" />
  public class ExternalDrivenBackgroundMonitor : IExternalDrivenBackgroundMonitor
  {
    protected bool _shouldReport;
    private string _reportText;
    private bool _hasFreshReportText;
    private double _progressFraction = double.NaN;
    private OperationStatus _operationStatus;
    private string _taskName;

    private bool _cancellationPending;
    private Lazy<CancellationTokenSource> _cancellationTokenSource;

    public ExternalDrivenBackgroundMonitor()
    {
      _reportText = string.Empty;
      _taskName = nameof(ExternalDrivenBackgroundMonitor);
      _cancellationTokenSource = new Lazy<CancellationTokenSource>(() => new CancellationTokenSource());
    }

    #region IBackgroundMonitor Members

    public virtual bool ShouldReportNow
    {
      get
      {
        return _shouldReport;
      }
    }

    public virtual void SetShouldReportNow()
    {
      _shouldReport = true;
    }

    public void ReportProgress(string text)
    {
      _shouldReport = false;
      _reportText = text;
      _hasFreshReportText = true;
    }

    public void ReportProgress(string text, double progressFraction)
    {
      _shouldReport = false;
      _reportText = text;
      _hasFreshReportText = true;
      _progressFraction = progressFraction;
    }

    public bool HasReportText
    {
      get { return _hasFreshReportText; }
    }

    public string GetReportText()
    {
      _hasFreshReportText = false;
      return _reportText;
    }

    public double GetProgressFraction()
    {
      return _progressFraction;
    }

    public bool CancellationPending
    {
      get
      {
        return _cancellationPending;
      }
    }

    public double Progress
    {
      get { return _progressFraction; }
      set { _progressFraction = value; }
    }

    public OperationStatus Status
    {
      get { return _operationStatus; }
      set { _operationStatus = value; }
    }

    public string TaskName
    {
      get { return _taskName; }
      set { _taskName = value; }
    }

    public CancellationToken CancellationToken
    {
      get
      {
        return _cancellationTokenSource.Value.Token;
      }
    }

    public void SetCancellationPending()
    {
      _cancellationPending = true;
      if (_cancellationTokenSource.IsValueCreated)
        _cancellationTokenSource.Value.Cancel();
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
      throw new NotImplementedException();
    }

    public void Dispose()
    {
      throw new NotImplementedException();
    }

    #endregion


    private class SubTask : IProgressReporter
    {
      private IProgressReporter _parent;
      private CancellationToken? _cancellationToken;
      private double _progress;
      private double _progressOffset;
      private double _progressSpan;

      public SubTask(IProgressReporter parent, double workSpan)
      {
        _parent = parent;
        _progressOffset = parent.Progress;
        _progressSpan = workSpan;
      }

      public SubTask(IProgressReporter parent, double workSpan, CancellationToken cancellationToken)
      {
        _parent = parent;
        _progressOffset = parent.Progress;
        _progressSpan = workSpan;
        _cancellationToken = cancellationToken;
      }


      public double Progress {
        get => _progress;
        set
        {
          _progress = value;
          _parent.Report(_progressOffset + _progressSpan * value);
        }
      }
      public OperationStatus Status
      {
        get => _parent.Status;
        set
        {
          if ((int)value > (int)(_parent.Status))
            _parent.Status =value;
        }
      }
      public string TaskName
      {
        get => _parent.TaskName;
        set => _parent.TaskName = value; }

      public CancellationToken CancellationToken => _cancellationToken ?? _parent.CancellationToken;

      public bool ShouldReportNow => _parent.ShouldReportNow;

      public bool CancellationPending => _parent.CancellationPending;

      public IProgressReporter CreateSubTask(double workAmount)
      {
        return new SubTask(this, workAmount);
      }

      public IProgressReporter CreateSubTask(double workAmount, CancellationToken cancellationToken)
      {
        return new SubTask(this, workAmount, cancellationToken);
      }

      public void Dispose()
      {
        _parent = null!;
      }

      public void Report(double value)
      {
        Progress = value;
      }

      public void ReportProgress(string text)
      {
        _parent.ReportProgress(text);
      }

      public void ReportProgress(string text, double progressValue)
      {
        Progress = progressValue;
        _parent.ReportProgress(text);
      }
    }

  }
}
