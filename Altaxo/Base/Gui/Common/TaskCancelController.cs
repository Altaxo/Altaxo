using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Altaxo.Main.Services;

namespace Altaxo.Gui.Common
{
  /// <summary>
  /// Controls the background execution of a thread or a task.
  /// </summary>
  /// <remarks>
  /// There is a property <see cref="IsWindowVisible"/>, which is set to true after a given amount of time,
  /// and then set to false when the window should close.
  /// There is another property <see cref="IsExecutionInProgress"/> that indicates if currently a task/thread is executed.
  /// The order a task or thread is stopped is this:
  /// 1. Signal the CancellationTokenSoft
  /// 2. Signal the CancellationTokenHard
  /// 3. Interrupt the thread (only works for threads)
  /// 4. Abort the thread (only works for threads)
  /// </remarks>
  public class TaskCancelController : INotifyPropertyChanged, IDisposable
  {
    protected enum State
    {
      Running = 0,
      CancellationSoftRequested = 1,
      CancellationHardRequested = 2,
      InterruptRequested = 3,
      AbandonRequested = 5,
    };

    protected Timer? _timer;
    protected int _delayMilliseconds;
    private Task? _task;
    private Thread? _thread;
    public event PropertyChangedEventHandler? PropertyChanged;

    public TaskCancelController()
    {
      CmdCancellationSoft = new RelayCommand(EhCancellationSoft);
      CmdCancellationHard = new RelayCommand(EhCancellationHard);
      CmdInterrupt = new RelayCommand(EhInterrupt);
      CmdAbandon = new RelayCommand(EhAbandon);
    }

    public TaskCancelController(Thread thread, IProgressMonitor monitor, int delayMilliseconds)
      : this()
    {
      _thread = thread;
      IsExecutionInProgress = true;

      Monitor = monitor;
      _delayMilliseconds = delayMilliseconds;
      _timer = new System.Threading.Timer(EhTimer, null, 100, 100);
      if (_delayMilliseconds <= 0)
        IsWindowVisible = true;
    }

    public TaskCancelController(Task task, IProgressMonitor monitor, int delayMilliseconds)
      : this()
    {
      _task = task;
      IsExecutionInProgress = true;

      Monitor = monitor;
      _delayMilliseconds = delayMilliseconds;
      _timer = new Timer(EhTimer, null, 100, 100);
      if (_delayMilliseconds <= 0)
        IsWindowVisible = true;
    }

    #region Bindings

    public bool WasCancelledByUser => _stateOfCancelling != State.Running;

    public ICommand CmdCancellationSoft { get; }

    public ICommand CmdCancellationHard { get; }

    public ICommand CmdInterrupt { get; }

    public ICommand CmdAbandon { get; }

    private State _stateOfCancelling;

    protected State StateOfCancelling
    {
      get => _stateOfCancelling;
      set
      {
        if (!(_stateOfCancelling == value))
        {
          _stateOfCancelling = value;
          OnPropertyChanged(nameof(StateOfCancelling));

          OnPropertyChanged(nameof(IsCancellationSoftVisible));
          OnPropertyChanged(nameof(IsCancellationHardVisible));
          OnPropertyChanged(nameof(IsInterruptVisible));
          OnPropertyChanged(nameof(IsAbandonVisible));
        }
      }
    }

    public bool IsCancellationSoftVisible => _stateOfCancelling == State.Running;
    public bool IsCancellationHardVisible => _stateOfCancelling == State.CancellationSoftRequested;
    public bool IsInterruptVisible => IsThread ? _stateOfCancelling == State.CancellationHardRequested : false;
    public bool IsAbandonVisible => ((int)_stateOfCancelling) >= (IsThread ? (int)State.InterruptRequested : (int)State.CancellationHardRequested);


    private IProgressMonitor? _monitor;

    public IProgressMonitor? Monitor
    {
      get => _monitor;
      set
      {
        if (!(_monitor == value))
        {
          _monitor = value;
          OnPropertyChanged(nameof(Monitor));
        }
      }
    }


    private string _title = "Waiting for completion ...";

    public string Title
    {
      get => _title;
      set
      {
        if (!(_title == value))
        {
          _title = value;
          OnPropertyChanged(nameof(Title));
        }
      }
    }

    private bool _isWindowVisible;

    public bool IsWindowVisible
    {
      get => _isWindowVisible;
      set
      {
        if (!(_isWindowVisible == value))
        {
          _isWindowVisible = value;
          OnPropertyChanged(nameof(IsWindowVisible));
        }
      }
    }


    private double _progressValue;
    public double ProgressValue
    {
      get
      {
        if (Monitor is IExternalDrivenBackgroundMonitor edbm)
        {
          edbm.SetShouldReportNow();
        }

        return _progressValue;

      }
      set
      {
        if (!(_progressValue == value))
        {
          _progressValue = value;
          OnPropertyChanged(nameof(ProgressValue));
        }
      }
    }

    private string _progressText = "An operation has not yet finished. If you feel that the operation takes unusual long time, you can interrupt it.";
    public string ProgressText
    {
      get
      {
        if (Monitor is IExternalDrivenBackgroundMonitor edbm)
        {
          edbm.SetShouldReportNow();
        }

        return _progressText;
      }
      set
      {
        if (!(_progressText == value))
        {
          _progressText = value;
          OnPropertyChanged(nameof(ProgressText));
        }
      }
    }

    private bool _isExecutionInProgress;

    public bool IsExecutionInProgress
    {
      get => _isExecutionInProgress;
      set
      {
        if (!(_isExecutionInProgress == value))
        {
          _isExecutionInProgress = value;
          OnPropertyChanged(nameof(IsExecutionInProgress));
        }
      }
    }


    #endregion

    protected bool IsThread => _thread is not null;

    public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    protected void CreateTimer()
    {
      if (_timer is null)
      {
        _timer = new System.Threading.Timer(EhTimer, null, 100, 100);
      }
    }

    private void EhCancellationSoft()
    {
      Monitor?.SetCancellationPendingSoft();
      StateOfCancelling = State.CancellationSoftRequested;
    }

    private void EhCancellationHard()
    {
      Monitor?.SetCancellationPendingHard();
      StateOfCancelling = State.CancellationHardRequested;
    }

    protected void EhInterrupt()
    {
      if (_thread is not null)
      {
        StateOfCancelling = State.InterruptRequested;

        if (_thread.IsAlive)
        {
          Monitor?.SetCancellationPendingHard();
          _thread.Interrupt();
        }
      }
      else
      {
        StateOfCancelling = State.AbandonRequested;
      }
    }



    protected void EhAbandon()
    {
      // there is no way to abort a task
      if (true == Current.Gui.YesNoMessageBox(
          "There is no further way to stop this task!\r\n" +
          "The only possibility is to abandon the task and let it run freely, without waiting for it to end.\r\n" +
          "Of course, this can have unwanted side effects.\r\n" +
          "Do you really want to abandon the task?",
          "Abandon the task?", false))
      {
        _timer?.Dispose();
        _timer = null;
        _thread = null;
        _task = null;
        StateOfCancelling = State.AbandonRequested;
        IsExecutionInProgress = false; // Order is critical here: first IsExecutionInProgress set to false, because on closing of the windows this is checked for false
        IsWindowVisible = false;
      }
    }

    private bool HasTaskOrThreadCompleted()
    {

      if (_thread is not null)
        return !_thread.IsAlive;
      else if (_task is not null)
        return _task.IsCompleted;
      else
        return true;
    }



    private void EhTimer(object? state)
    {
      _delayMilliseconds = Math.Max(0, _delayMilliseconds - 100);
      if (_delayMilliseconds <= 0)
      {
        IsWindowVisible = true;

        // if the monitor is externally driven, we give it a trigger every 100 ms
        if (Monitor is IExternalDrivenBackgroundMonitor m)
        {
          m.SetShouldReportNow();
        }
      }

      if (HasTaskOrThreadCompleted())
      {
        _timer?.Dispose();
        _timer = null;
        IsExecutionInProgress = false; // Order is critical here: first IsExecutionInProgress set to false, because on closing of the windows this is checked for false
        IsWindowVisible = false;
      }
    }

    public void Dispose()
    {
      _timer?.Dispose();
      _timer = null;
      IsExecutionInProgress = false; // Order is critical here: first IsExecutionInProgress set to false, because on closing of the windows this is checked for false
      IsWindowVisible = false;
      Monitor?.Dispose();
    }

    public void StartExecution(Action<IProgressReporter> action, int delayInMilliseconds)
    {
      var (monitor, reporter) = ExternalDrivenBackgroundMonitor.NewMonitorAndReporter();
      Exception? exception = null;
      var thread = new Thread(() =>
        {
          try
          {
            action(reporter);
          }
          catch (Exception ex)
          {
            exception = ex;
          }
        }
      );
      thread.Start();
      _thread = thread;
      StateOfCancelling = State.Running;
      IsExecutionInProgress = true;
      Monitor = monitor;

      _delayMilliseconds = delayInMilliseconds;
      CreateTimer();
      if (_delayMilliseconds <= 0)
      {
        IsWindowVisible = true;
      }
    }
  }
}
