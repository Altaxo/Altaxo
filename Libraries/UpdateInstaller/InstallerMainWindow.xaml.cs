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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Altaxo.Serialization.AutoUpdates
{
  /// <summary>
  /// Interaction logic for the main window.
  /// </summary>
  public partial class InstallerMainWindow : Window
  {
    public IUpdateInstaller _installer;
    private System.Threading.Tasks.Task _installerTask;

    public double _progress;
    public string _message = string.Empty;
    private System.Windows.Threading.DispatcherTimer _timer;
    private Brush _normalBackground;
    private bool _isCancellationRequested = false;
    private bool _installerFinishedSuccessfully = false;

    private bool _showInstallationWindow;
    private int _timeoutAfterSucessfullInstallation;

    private int _timeLeftBeforeClosing;

    /// <summary>Initializes a new instance of the <see cref="InstallerMainWindow"/> class.</summary>
    public InstallerMainWindow(bool showInstallationWindow, int timeoutAfterSuccessfullInstallation)
    {
      _showInstallationWindow = showInstallationWindow;
      _timeoutAfterSucessfullInstallation = timeoutAfterSuccessfullInstallation;

      if (_showInstallationWindow)
        Visibility = System.Windows.Visibility.Visible;
      else
        Visibility = System.Windows.Visibility.Hidden;

      InitializeComponent();
      _normalBackground = _guiMessages.Background;
      Loaded += new RoutedEventHandler(EhLoaded);
    }

    /// <summary>Called when the window is loaded.</summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
    private void EhLoaded(object sender, RoutedEventArgs e)
    {
      if (null != _installer)
      {
        InstallerTaskSetupAndStart();
      }
    }

    /// <summary>Sets up the timer and the installer task, then starts the installer.</summary>
    private void InstallerTaskSetupAndStart()
    {
      _btOk.IsEnabled = false;
      _btCancel.IsEnabled = true;
      _btTryAgain.IsEnabled = false;

      _isCancellationRequested = false;
      _installerFinishedSuccessfully = false;
      _guiMessages.Background = _normalBackground;

      _timer = new System.Windows.Threading.DispatcherTimer();
      _timer.Tick += new EventHandler(EhInstallationTimerTick);
      _timer.Interval = new TimeSpan(0, 0, 0, 0, 250);
      _timer.Start();

      _installerTask = new System.Threading.Tasks.Task(RunInstaller);
      _installerTask.Start();
    }

    /// <summary>Cleans up the timer and the installer task after the task is finished.</summary>
    /// <returns>If the task has thrown an exception, this exeption is returned. Otherwise, the return value is <c>null</c>.</returns>
    private AggregateException InstallerTaskCleanup()
    {
      _timer.Tick -= EhInstallationTimerTick;
      _timer.Stop();
      _timer = null;

      var exception = _installerTask.Exception;
      _installerTask.Dispose();
      _installerTask = null;
      _isCancellationRequested = false;

      _installerFinishedSuccessfully = (null == exception);
      _btOk.IsEnabled = true;
      _btTryAgain.IsEnabled = !_installerFinishedSuccessfully;
      _btCancel.IsEnabled = false;

      return exception;
    }

    /// <summary>Sets a error message in the message window.</summary>
    /// <param name="message">The error message.</param>
    public void SetErrorMessage(string message)
    {
      _guiProgress.Value = 0;
      _guiMessages.Text = message;
      _guiMessages.Background = new SolidColorBrush(Colors.LightPink);

      Visibility = System.Windows.Visibility.Visible;
    }

    /// <summary>Called when the user requests to close the window.</summary>
    /// <param name="e">A <see cref="T:System.ComponentModel.CancelEventArgs"/> that contains the event data.</param>
    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
      if (_installerTask != null && !_installerTask.IsCompleted)
      {
        e.Cancel = true;
      }

      base.OnClosing(e);
    }

    /// <summary>Called when the timer event occured.</summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void EhInstallationTimerTick(object sender, EventArgs e)
    {
      _guiProgress.Value = _progress;
      _guiProgressText.Content = string.Format("{0:F1}% completed", _progress);
      _guiMessages.Text = _message.ToString();

      if (_installerTask.IsCompleted)
      {
        var exception = InstallerTaskCleanup();
        if (null != exception)
        {
          SetErrorMessage(UpdateInstallerMain.ErrorIntroduction + exception.ToString());
        }
        else
        {
          // Successfully finished the installation
          if (false == _showInstallationWindow || _timeoutAfterSucessfullInstallation <= 0)
          {
            Close(); // Close if either the window is not visible or the timeout is 0 sec
          }
          else if (_timeoutAfterSucessfullInstallation >= int.MaxValue)
          {
            // Time too high for count down, simply leave window open
          }
          else
          {
            // Set up a new timer that now count's down
            _timeLeftBeforeClosing = _timeoutAfterSucessfullInstallation;
            _timer = new System.Windows.Threading.DispatcherTimer();
            _timer.Tick += new EventHandler(EhCountDownTimerTick);
            _timer.Interval = new TimeSpan(0, 0, 0, 1, 0);
            _timer.Start();
          }
        }
      }
    }

    /// <summary>Called when the timer event occured.</summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void EhCountDownTimerTick(object sender, EventArgs e)
    {
      --_timeLeftBeforeClosing;

      if (_timeLeftBeforeClosing <= 0)
      {
        _timer.Tick -= EhCountDownTimerTick;
        _timer.Stop();
        _timer = null;
        Close();
      }
      else
      {
        _guiProgress.Value = 100 * (1 - _timeLeftBeforeClosing / (double)_timeoutAfterSucessfullInstallation);
        _guiProgressText.Content = string.Format("Time left before closing: {0} s", _timeLeftBeforeClosing);
      }
    }

    /// <summary>Reports the progress of the installer.</summary>
    /// <param name="progress">The progress in percent.</param>
    /// <param name="message">The message text.</param>
    /// <returns>True when the user has requested a cancellation.</returns>
    private bool ReportProgressAsync(double progress, string message)
    {
      _progress = progress;
      _message = message;
      return _isCancellationRequested;
    }

    /// <summary>Entry point for the task that runs the installer.</summary>
    private void RunInstaller()
    {
      _installer.Run(ReportProgressAsync);
    }

    /// <summary>Called when the OK button was pressed.</summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
    private void EhOk(object sender, RoutedEventArgs e)
    {
      Close();
    }

    /// <summary>Called when the TryAgain button was pressed.</summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
    private void EhTryAgain(object sender, RoutedEventArgs e)
    {
      if (null == _installerTask && null == _timer)
      {
        InstallerTaskSetupAndStart();
      }
    }

    /// <summary>Called when the Cancel button was pressed.</summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
    private void EhCancel(object sender, RoutedEventArgs e)
    {
      _isCancellationRequested = true;
    }
  }
}
