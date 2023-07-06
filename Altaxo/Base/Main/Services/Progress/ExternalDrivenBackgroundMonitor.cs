#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
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
using System.ComponentModel;
using System.Threading;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Background monitor that can be externally driven (e.g. by a timer) to set <see cref="ShouldReportNow"/>.
  /// 
  /// </summary>
  /// <seealso cref="Altaxo.Main.Services.IExternalDrivenBackgroundMonitor" />
  public partial class ExternalDrivenBackgroundMonitor : IExternalDrivenBackgroundMonitor
  {
    const int NumberOfSupportedTextLevels = 3;

    // Predefined events in order to avoid too much of allocating
    static readonly PropertyChangedEventArgs _progressChangedEventArg = new PropertyChangedEventArgs(nameof(Progress));
    static readonly PropertyChangedEventArgs _progressAndETAChangedEventArg = new PropertyChangedEventArgs(nameof(ProgressAndETA));
    static readonly PropertyChangedEventArgs _text0ChangedEventArg = new PropertyChangedEventArgs(nameof(Text0));
    static readonly PropertyChangedEventArgs _text1ChangedEventArg = new PropertyChangedEventArgs(nameof(Text1));
    static readonly PropertyChangedEventArgs _text2ChangedEventArg = new PropertyChangedEventArgs(nameof(Text2));
    static readonly PropertyChangedEventArgs _disposedChangedEventArg = new PropertyChangedEventArgs(nameof(IsDisposed));
    static readonly PropertyChangedEventArgs _statusChangedEventArg = new PropertyChangedEventArgs(nameof(Status));

    private CancellationTokenSource _cancellationTokenSourceSoft;
    private CancellationTokenSource _cancellationTokenSourceHard;
    private DateTimeOffset _startTimeUtc = DateTimeOffset.UtcNow;
    public event PropertyChangedEventHandler? PropertyChanged;


    public string TaskName { get; init; } = nameof(ExternalDrivenBackgroundMonitor);

    #region ShouldReport

    /// <summary>
    /// If true, <see cref="ShouldReportNow"/> is set automatically every time one of the properties
    /// (<see cref="Progress"/>, <see cref="Text0"/>, <see cref="Text1"/>, ..) is read.
    /// </summary>
    public bool ShouldSetShouldReportAutomatically { get; init; }

    protected bool _shouldReport;

    public bool ShouldReportNow => _shouldReport;



    public virtual void SetShouldReportNow()
    {
      _shouldReport = true;
    }

    #endregion

    public ExternalDrivenBackgroundMonitor()
    {
      _cancellationTokenSourceSoft = new CancellationTokenSource();
      _cancellationTokenSourceHard = new CancellationTokenSource();
    }

    public static (ExternalDrivenBackgroundMonitor monitor, IProgressReporter reporter) NewMonitorAndReporter()
    {
      var monitor = new ExternalDrivenBackgroundMonitor();
      var reporter = monitor.GetProgressReporter();
      return (monitor, reporter);
    }

    #region Bindings

    private bool _isDisposed;

    public bool IsDisposed
    {
      get => _isDisposed;
      protected set
      {
        if (!(_isDisposed == value))
        {
          _isDisposed = value;
          PropertyChanged?.Invoke(this, _disposedChangedEventArg);
        }
      }
    }

    private OperationStatus _status;

    public OperationStatus Status
    {
      get => _status;
      protected set
      {
        value = (OperationStatus)Math.Max((byte)_status, (byte)value);
        if (!(_status == value))
        {
          _status = value;
          PropertyChanged?.Invoke(this, _statusChangedEventArg);
        }
      }
    }


    private double _progress;

    public double Progress
    {
      get
      {
        if (ShouldSetShouldReportAutomatically)
          _shouldReport = true;

        return _progress;
      }
      set
      {
        if (!(_progress == value))
        {
          _progress = value;
          PropertyChanged?.Invoke(this, _progressChangedEventArg);
          PropertyChanged?.Invoke(this, _progressAndETAChangedEventArg);
        }
      }
    }

    public string ProgressAndETA
    {
      get
      {
        var progress = _progress;
        if (progress > 0)
        {
          var elapsed = DateTimeOffset.UtcNow - _startTimeUtc;
          var endTime = (_startTimeUtc + TimeSpan.FromSeconds(elapsed.TotalSeconds / progress)).ToLocalTime();
          return $"Progress: {(100 * _progress):F1}%  ETA: {endTime:F}";
        }
        else
        {
          return $"Progress: {(100 * _progress):F1}%";
        }
      }
    }

    private string _text0 = "An operation is not yet completed. If you feel that the operation takes unusual long time, you can interrupt it.";

    public string Text0
    {
      get
      {
        if (ShouldSetShouldReportAutomatically)
          _shouldReport = true;

        return _text0;
      }
      set
      {
        if (!(_text0 == value))
        {
          _text0 = value;
          PropertyChanged?.Invoke(this, _text0ChangedEventArg);
        }
      }
    }

    private string _text1 = string.Empty;

    public string Text1
    {
      get
      {
        if (ShouldSetShouldReportAutomatically)
          _shouldReport = true;

        return _text1;
      }

      set
      {
        if (!(_text1 == value))
        {
          _text1 = value;
          PropertyChanged?.Invoke(this, _text1ChangedEventArg);
        }
      }
    }

    private string _text2 = string.Empty;

    public string Text2
    {
      get
      {
        if (ShouldSetShouldReportAutomatically)
          _shouldReport = true;

        return _text2;
      }

      set
      {
        if (!(_text2 == value))
        {
          _text2 = value;
          PropertyChanged?.Invoke(this, _text2ChangedEventArg);
        }
      }
    }

    #endregion

    private void EhSubTaskReport(Reporter subTask2, int level, double progress, string? text, OperationStatus status)
    {
      Progress = progress;
      Status = status;
      if (text is not null)
      {
        switch (level)
        {
          case 0:
            Text0 = text;
            Text1 = string.Empty;
            Text2 = string.Empty;
            break;
          case 1:
            Text1 = text;
            Text2 = string.Empty;
            break;
          case 2:
            Text2 = text;
            break;
        }
      }
      SetShouldReportNow();
    }

    public void SetCancellationPendingHard()
    {
      _cancellationTokenSourceSoft.Cancel();
      _cancellationTokenSourceHard.Cancel();
    }

    public void SetCancellationPendingSoft()
    {
      _cancellationTokenSourceSoft.Cancel();
    }

    public IProgressReporter GetProgressReporter()
    {
      _startTimeUtc = DateTime.UtcNow;
      return new Reporter(this, null, 0, 1, this.GetType().Name, _cancellationTokenSourceSoft.Token, _cancellationTokenSourceHard.Token);
    }

    public void Dispose()
    {
      IsDisposed = true;
      _cancellationTokenSourceSoft.Dispose();
      _cancellationTokenSourceHard.Dispose();
      GC.SuppressFinalize(this);
    }
  }

}
