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
    private CancellationTokenSource _cancellationTokenSource;

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
        _cancellationTokenSource = _cancellationTokenSource ?? new CancellationTokenSource();
        return _cancellationTokenSource.Token;
      }
    }

    public void SetCancellationPending()
    {
      _cancellationPending = true;
      _cancellationTokenSource?.Cancel();
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

    #endregion IBackgroundMonitor Members
  }
}
