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
  public class ExternalDrivenBackgroundMonitor : IExternalDrivenBackgroundMonitor, IDisposable
  {
    protected bool _shouldReport;
    private string _reportText;
    private bool _hasFreshReportText;
    private double _progressFraction = double.NaN;
    private OperationStatus _operationStatus;
    private string _taskName;
    private Lazy<CancellationTokenSource> _cancellationTokenSourceSoft;
    private Lazy<CancellationTokenSource> _cancellationTokenSourceHard;

    public ExternalDrivenBackgroundMonitor()
    {
      _reportText = string.Empty;
      _taskName = nameof(ExternalDrivenBackgroundMonitor);
      _cancellationTokenSourceSoft = new Lazy<CancellationTokenSource>(() => new CancellationTokenSource());
      _cancellationTokenSourceHard = new Lazy<CancellationTokenSource>(() => new CancellationTokenSource());
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

    void System.IProgress<string>.Report(string text) => ReportProgress(text);
    void System.IProgress<(string text, double progressFraction)>.Report((string text, double progressFraction) tuple) => ReportProgress(tuple.text, tuple.progressFraction);

    


    public void ReportProgress(string text, double progressFraction)
    {
      _shouldReport = false;
      _reportText = text;
      _hasFreshReportText = true;
      _progressFraction = progressFraction;
    }

    public bool HasReportUpdate
    {
      get { return _hasFreshReportText; }
    }

    public (string text, double progressFraction) GetReportUpdate()
    {
      _hasFreshReportText = false;
      return (_reportText, _progressFraction);
    }

    public double GetProgressFraction()
    {
      return _progressFraction;
    }

    public bool CancellationPending
    {
      get
      {
        return (_cancellationTokenSourceSoft.IsValueCreated && _cancellationTokenSourceSoft.Value.IsCancellationRequested) ||
                (_cancellationTokenSourceHard.IsValueCreated && _cancellationTokenSourceHard.Value.IsCancellationRequested);
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
        return _cancellationTokenSourceSoft.Value.Token;
      }
    }

    public CancellationToken CancellationTokenHard
    {
      get
      {
        return _cancellationTokenSourceHard.Value.Token;
      }
    }

    public void SetCancellationPending()
    {
      if(_cancellationTokenSourceHard.IsValueCreated)
      {
        // we do the hard cancellation only if either there is not soft token source created,
        // or if the soft token source is already cancelled
        if (!_cancellationTokenSourceSoft.IsValueCreated || _cancellationTokenSourceSoft.Value.IsCancellationRequested)
        {
          _cancellationTokenSourceHard.Value.Cancel();
        }
      }
      if (_cancellationTokenSourceSoft.IsValueCreated)
      {
        _cancellationTokenSourceSoft.Value.Cancel();
      }
    }

    void IProgressMonitor.SetCancellationPendingSoft()
    {
      _cancellationTokenSourceSoft.Value.Cancel();
    }

    void IProgressMonitor.SetCancellationPendingHard()
    {
      _cancellationTokenSourceHard.Value.Cancel();
    }

    public IProgressReporter CreateSubTask(double workAmount)
    {
      throw new NotImplementedException();
    }

    public IProgressReporter CreateSubTask(double workAmount, CancellationToken cancellationTokenSoft, CancellationToken cancellationTokenHard)
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
      private CancellationToken? _cancellationTokenHard;
      private double _progress;
      private double _progressOffset;
      private double _progressSpan;

      public SubTask(IProgressReporter parent, double workSpan)
      {
        _parent = parent;
        _progressOffset = parent.Progress;
        _progressSpan = workSpan;
      }

      public SubTask(IProgressReporter parent, double workSpan, CancellationToken cancellationTokenSoft, CancellationToken cancellationTokenHard)
      {
        _parent = parent;
        _progressOffset = parent.Progress;
        _progressSpan = workSpan;
        _cancellationToken = cancellationTokenSoft;
        _cancellationTokenHard = cancellationTokenHard;
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
      public CancellationToken CancellationTokenHard => _cancellationTokenHard ?? _parent.CancellationTokenHard;

      public bool ShouldReportNow => _parent.ShouldReportNow;

      public bool CancellationPending => _parent.CancellationPending;

      public IProgressReporter CreateSubTask(double workAmount)
      {
        return new SubTask(this, workAmount);
      }

      public IProgressReporter CreateSubTask(double workAmount, CancellationToken cancellationTokenSoft, CancellationToken cancellationTokenHard)
      {
        return new SubTask(this, workAmount, cancellationTokenSoft, cancellationTokenHard);
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

      void System.IProgress<string>.Report(string text) => ReportProgress(text);
      void System.IProgress<(string text, double progressFraction)>.Report((string text, double progressFraction) tuple) => ReportProgress(tuple.text, tuple.progressFraction);

    }

  }
}
