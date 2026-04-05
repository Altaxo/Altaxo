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
  /// There is a property <see cref="IsWindowVisible"/>, which is set to true after a given amount of time
  /// and then set to false when the window should close.
  /// There is another property, <see cref="IsExecutionInProgress"/>, that indicates whether a task or thread is currently executing.
  /// A task or thread is stopped in the following order:
  /// 1. Signal the CancellationTokenSoft
  /// 2. Signal the CancellationTokenHard
  /// 3. Interrupt the thread (only works for threads)
  /// 4. Abort the thread (only works for threads)
  /// </remarks>
  public class TaskCancelController : INotifyPropertyChanged, IDisposable
  {
    /// <summary>
    /// Describes the current cancellation stage.
    /// </summary>
    protected enum State
    {
      /// <summary>
      /// Execution is still running.
      /// </summary>
      Running = 0,

      /// <summary>
      /// A soft cancellation was requested.
      /// </summary>
      CancellationSoftRequested = 1,

      /// <summary>
      /// A hard cancellation was requested.
      /// </summary>
      CancellationHardRequested = 2,

      /// <summary>
      /// A thread interrupt was requested.
      /// </summary>
      InterruptRequested = 3,

      /// <summary>
      /// Abandoning the operation was requested.
      /// </summary>
      AbandonRequested = 5,
    };

    /// <summary>
    /// Polling timer that monitors task or thread completion.
    /// </summary>
    protected Timer? _timer;

    /// <summary>
    /// Delay before the progress window becomes visible, in milliseconds.
    /// </summary>
    protected int _delayMilliseconds;
    private Task? _task;
    private Thread? _thread;
    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskCancelController"/> class.
    /// </summary>
    public TaskCancelController()
    {
      CmdCancellationSoft = new RelayCommand(EhCancellationSoft);
      CmdCancellationHard = new RelayCommand(EhCancellationHard);
      CmdInterrupt = new RelayCommand(EhInterrupt);
      CmdAbandon = new RelayCommand(EhAbandon);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskCancelController"/> class for a thread.
    /// </summary>
    /// <param name="thread">The thread to monitor.</param>
    /// <param name="monitor">The progress monitor used for reporting and cancellation.</param>
    /// <param name="delayMilliseconds">The delay before the window becomes visible, in milliseconds.</param>
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

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskCancelController"/> class for a task.
    /// </summary>
    /// <param name="task">The task to monitor.</param>
    /// <param name="monitor">The progress monitor used for reporting and cancellation.</param>
    /// <param name="delayMilliseconds">The delay before the window becomes visible, in milliseconds.</param>
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

    /// <summary>
    /// Gets a value indicating whether the execution was cancelled by the user.
    /// </summary>
    public bool WasCancelledByUser => _stateOfCancelling != State.Running;

    /// <summary>
    /// Gets the command that requests soft cancellation.
    /// </summary>
    public ICommand CmdCancellationSoft { get; }

    /// <summary>
    /// Gets the command that requests hard cancellation.
    /// </summary>
    public ICommand CmdCancellationHard { get; }

    /// <summary>
    /// Gets the command that interrupts the thread.
    /// </summary>
    public ICommand CmdInterrupt { get; }

    /// <summary>
    /// Gets the command that abandons the operation.
    /// </summary>
    public ICommand CmdAbandon { get; }

    private State _stateOfCancelling;

    /// <summary>
    /// Gets or sets the current cancellation state.
    /// </summary>
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

    /// <summary>
    /// Gets a value indicating whether the soft-cancellation command should be visible.
    /// </summary>
    public bool IsCancellationSoftVisible => _stateOfCancelling == State.Running;
    /// <summary>
    /// Gets a value indicating whether the hard-cancellation command should be visible.
    /// </summary>
    public bool IsCancellationHardVisible => _stateOfCancelling == State.CancellationSoftRequested;
    /// <summary>
    /// Gets a value indicating whether the interrupt command should be visible.
    /// </summary>
    public bool IsInterruptVisible => IsThread ? _stateOfCancelling == State.CancellationHardRequested : false;
    /// <summary>
    /// Gets a value indicating whether the abandon command should be visible.
    /// </summary>
    public bool IsAbandonVisible => ((int)_stateOfCancelling) >= (IsThread ? (int)State.InterruptRequested : (int)State.CancellationHardRequested);


    private IProgressMonitor? _monitor;

    /// <summary>
    /// Gets or sets the progress monitor used by the controller.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the dialog title.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether the progress window should be visible.
    /// </summary>
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
    /// <summary>
    /// Gets or sets the current progress value.
    /// </summary>
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
    /// <summary>
    /// Gets or sets the current progress text.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether execution is still in progress.
    /// </summary>
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

    /// <summary>
    /// Gets a value indicating whether the controller is handling a thread.
    /// </summary>
    protected bool IsThread => _thread is not null;

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">The name of the changed property.</param>
    public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    /// <summary>
    /// Creates the polling timer if it does not yet exist.
    /// </summary>
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

    /// <summary>
    /// Handles an interrupt request.
    /// </summary>
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



    /// <summary>
    /// Handles an abandon request.
    /// </summary>
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

    /// <inheritdoc/>
    public void Dispose()
    {
      _timer?.Dispose();
      _timer = null;
      IsExecutionInProgress = false; // Order is critical here: first IsExecutionInProgress set to false, because on closing of the windows this is checked for false
      IsWindowVisible = false;
      Monitor?.Dispose();
    }

    /// <summary>
    /// Starts execution of the specified action on a background thread.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="delayInMilliseconds">The delay before the progress window becomes visible.</param>
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
