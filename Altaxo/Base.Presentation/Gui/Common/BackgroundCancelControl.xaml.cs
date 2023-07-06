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

#nullable disable warnings
using System;
using System.Windows;
using System.Windows.Controls;
using Altaxo.Main.Services;

namespace Altaxo.Gui.Common
{
  /// <summary>
  /// Interaction logic for BackgroundCancelControl.xaml
  /// </summary>
  public partial class BackgroundCancelControl : UserControl
  {
    private const int TimerTick_ms = 100;
    private System.Threading.Thread _thread;
    private IExternalDrivenBackgroundMonitor _monitor;
    private bool _wasCancelledByUser;
    private System.Windows.Threading.DispatcherTimer _timer;
    private int _showUpDownConter;

    public event Action<bool>? ExecutionFinished;

    public event Action? StartDelayExpired;

    public bool ExecutionInProgress
    {
      get
      {
        return _thread?.IsAlive == true;
      }
    }

    public BackgroundCancelControl()
    {
      InitializeComponent();
    }

    public void StartExecution(Action<IProgressReporter> action, int milliSecondsUntilShowUp)
    {
      if (ExecutionInProgress)
        throw new ApplicationException("Background thread is still executed");

      if (_timer is not null)
        throw new ApplicationException("Timer is still active");

      (_monitor, var reporter) = ExternalDrivenBackgroundMonitor.NewMonitorAndReporter();
      this.DataContext = _monitor;
      _thread = new System.Threading.Thread(() => action(reporter));

      _btCancelSoft.Visibility = _monitor is not null ? Visibility.Visible : Visibility.Collapsed;
      _btInterrupt.Visibility = _monitor is null ? Visibility.Visible : Visibility.Collapsed;
      _btCancelHard.Visibility = Visibility.Collapsed;
      _btAbort.Visibility = Visibility.Collapsed;

      _showUpDownConter = milliSecondsUntilShowUp / TimerTick_ms;
      _thread.Start();

      _timer = new System.Windows.Threading.DispatcherTimer(new TimeSpan(0, 0, 0, 0, TimerTick_ms), System.Windows.Threading.DispatcherPriority.Normal, EhTimer, Dispatcher);
    }

    public System.Threading.Thread Thread
    {
      get
      {
        return _thread;
      }
    }

    public System.Exception ThreadException
    {
      get
      {
        return null;
      }
    }

    private void EhTimer(object? sender, EventArgs e)
    {
      if (_showUpDownConter == 0)
      {
        if (StartDelayExpired is not null)
          StartDelayExpired();
      }
      _showUpDownConter--;

      _monitor?.SetShouldReportNow();

      if (!ExecutionInProgress)
      {
        var timer = (System.Windows.Threading.DispatcherTimer)sender;
        timer.Tick -= EhTimer;
        timer.Stop();
        _timer = null;
        _thread = null;

        ExecutionFinished?.Invoke(_wasCancelledByUser ? false : true);
      }
    }

    private void EhCancelSoftClicked(object sender, RoutedEventArgs e)
    {
      _wasCancelledByUser = true;
      _btCancelSoft.Visibility = Visibility.Collapsed;
      if (_monitor is not null)
      {
        _btCancelHard.Visibility = Visibility.Visible;
        _monitor.SetCancellationPendingSoft();
      }
      else
      {
        _btInterrupt.Visibility = Visibility.Visible;
        EhInterruptClicked(sender, e);
      }
    }

    private void EhCancelHardClicked(object sender, RoutedEventArgs e)
    {
      _wasCancelledByUser = true;
      _btCancelHard.Visibility = Visibility.Collapsed;
      if (_monitor is not null)
      {
        _btInterrupt.Visibility = Visibility.Visible;
        _monitor.SetCancellationPendingHard();
      }
      else
      {
        _btInterrupt.Visibility = Visibility.Visible;
        EhInterruptClicked(sender, e);
      }
    }

    private void EhInterruptClicked(object sender, RoutedEventArgs e)
    {
      if (ExecutionInProgress)
      {
        _wasCancelledByUser = true;
        _thread.Interrupt();
      }
      _btInterrupt.Visibility = Visibility.Collapsed;
      _btAbort.Visibility = Visibility.Visible;
    }



    private void EhAbortClicked(object sender, RoutedEventArgs e)
    {
      if (ExecutionInProgress)
      {
        _wasCancelledByUser = true;
        _thread.Abort();
      }
    }
  }
}
