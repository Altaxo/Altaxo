using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Altaxo.Main.Services;

namespace Altaxo.Gui.Common
{
  public class TaskCancelController : INotifyPropertyChanged, IDisposable
  {
    Task _task;
    System.Threading.Timer _timer;
    int _delayMilliseconds;

    bool _cancellationRequested;
    bool _interruptRequested;

    public event PropertyChangedEventHandler? PropertyChanged;
    public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public TaskCancelController(Task task, IProgressMonitor monitor, int delayMilliseconds)
    {
      _task = task;
      Monitor = monitor;
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
      Monitor.SetCancellationPendingHard();
      _interruptRequested = true;
      OnPropertyChanged(nameof(IsCancelVisible));
      OnPropertyChanged(nameof(IsInterruptVisible));
      OnPropertyChanged(nameof(IsAbortVisible));
    }

    private void EhCancel()
    {
      Monitor.SetCancellationPendingSoft();
      _cancellationRequested = true;
      OnPropertyChanged(nameof(IsCancelVisible));
      OnPropertyChanged(nameof(IsInterruptVisible));
      OnPropertyChanged(nameof(IsAbortVisible));
    }

    #region Bindings

    public IProgressMonitor Monitor { get; init; }

    public ICommand CmdCancel { get; }

    public ICommand CmdInterrupt { get; }

    public ICommand CmdAbort { get; }

    public bool IsCancelVisible => !_cancellationRequested && !_interruptRequested;
    public bool IsInterruptVisible => _cancellationRequested && !_interruptRequested;
    public bool IsAbortVisible => _cancellationRequested && _interruptRequested;


    private string _title = "Waiting for task completion ...";

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

    string _progressText = "An operation has not yet finished. If you feel that the operation takes unusual long time, you can interrupt it.";
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
      }
    }

    public void Dispose()
    {
      IsCompleted = true;
    }
  }
}
