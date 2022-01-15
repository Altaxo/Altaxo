using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Altaxo.Main.Services;

namespace Altaxo.Gui.Common
{
  public class TaskCancelController : INotifyPropertyChanged, IDisposable
  {
    CancellationTokenSource _ctsSoft;
    CancellationTokenSource _ctsHard;
    Task _task;
    IProgressMonitor _monitor;
    System.Threading.Timer _timer;
    int _delayMilliseconds;

    bool _cancellationRequested;
    bool _interruptRequested;

    public event PropertyChangedEventHandler? PropertyChanged;
    public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public TaskCancelController(Task task, CancellationTokenSource ctsSoft, CancellationTokenSource ctsHard, IProgressMonitor monitor, int delayMilliseconds)
    {
      _task = task;
      _ctsSoft = ctsSoft;
      _ctsHard = ctsHard;
      _monitor = monitor;
      _delayMilliseconds = delayMilliseconds;
      CmdCancel = new RelayCommand(EhCancel);
      CmdInterrupt = new RelayCommand(EhInterrupt);
      CmdAbort = new RelayCommand(EhAbort);
      _timer = new System.Threading.Timer(EhTimer, null, 100, 100);
      if (_delayMilliseconds <= 0)
        IsWindowVisible = true;
    }



    private void EhAbort()
    {
      // there is no way to abort a task
      if (true == Current.Gui.YesNoMessageBox(
          "A task can not be aborted!\r\n" +
          "The only possibility is to abandon the task and let it run freely, without to wait for it to end.\r\n" +
          "Of course, this can have unwanted side effects.\r\n" +
          "Do you really want to abandon the task?",
          "Abandon the task?", false))
      {
        IsCompleted = true;
      }
    }

    private void EhInterrupt()
    {
      _ctsHard?.Cancel();
      _interruptRequested = true;
      OnPropertyChanged(nameof(IsCancelVisible));
      OnPropertyChanged(nameof(IsInterruptVisible));
      OnPropertyChanged(nameof(IsAbortVisible));
    }

    private void EhCancel()
    {
      _ctsSoft?.Cancel();
      _cancellationRequested = true;
      OnPropertyChanged(nameof(IsCancelVisible));
      OnPropertyChanged(nameof(IsInterruptVisible));
      OnPropertyChanged(nameof(IsAbortVisible));
    }

    #region Bindings

    public ICommand CmdCancel { get; }

    public ICommand CmdInterrupt { get; }

    public ICommand CmdAbort { get; }

    public bool IsCancelVisible => !_cancellationRequested && !_interruptRequested;
    public bool IsInterruptVisible => _cancellationRequested && !_interruptRequested;
    public bool IsAbortVisible => _cancellationRequested && _interruptRequested;


    bool _isCompleted;

    public bool IsCompleted
    {
      get => _isCompleted;
      set
      {
        if (!(_isCompleted == value) && value is true)
        {
          _isCompleted = value;
          _timer?.Dispose();
          OnPropertyChanged(nameof(IsCompleted));
        }
      }
    }

    bool _isWindowVisible;

    public bool IsWindowVisible
    {
      get => _isWindowVisible;
      set
      {
        if (!(_isWindowVisible == value) && value is true)
        {
          _isWindowVisible = value;
          OnPropertyChanged(nameof(IsWindowVisible));
        }
      }
    }


    double _progressValue;
    public double ProgressValue
    {
      get => _progressValue;
      set
      {
        if (!(_progressValue == value))
        {
          _progressValue = value;
          OnPropertyChanged(nameof(ProgressValue));
        }
      }
    }

    string _progressText = "An operation has not yet finished. If you feel that the operation takes unusual long time, you can interrupt it.";
    public string ProgressText
    {
      get => _progressText;
      set
      {
        if (!(_progressText == value))
        {
          _progressText = value;
          OnPropertyChanged(nameof(ProgressText));
        }
      }
    }

    #endregion

    private void EhTimer(object? state)
    {
      _delayMilliseconds -= 100;

      if (_delayMilliseconds <= 0)
      {
        IsWindowVisible = true;
      }

      if (_task.IsCompleted)
      {
        IsCompleted = true;
      }
      else
      {
        if (_monitor.HasReportText)
          ProgressText = _monitor.GetReportText();

        ProgressValue = _monitor.GetProgressFraction();
      }
    }

    public void Dispose()
    {
      IsCompleted = true;
    }
  }
}
