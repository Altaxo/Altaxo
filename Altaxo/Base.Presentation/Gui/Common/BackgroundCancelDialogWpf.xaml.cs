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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Altaxo.Main.Services;

namespace Altaxo.Gui.Common
{
  /// <summary>
  /// Interaction logic for BackgroundCancelDialogWpf.xaml
  /// </summary>
  public partial class BackgroundCancelDialogWpf : Window
  {
    private System.Threading.ThreadStart _threadStart;
    private System.Exception _threadException;
    private System.Threading.Thread _thread;
    private IExternalDrivenBackgroundMonitor _monitor;
    private bool _wasCancelledByUser;
    private System.Windows.Threading.DispatcherTimer _timer;
    private int _timerCounter;

    public BackgroundCancelDialogWpf()
    {
      InitializeComponent();
    }

    public BackgroundCancelDialogWpf(System.Threading.Thread thread, IExternalDrivenBackgroundMonitor monitor)
    {
      _thread = thread;
      _monitor = monitor;
      //
      // Required for Windows Form Designer support
      //
      InitializeComponent();

      _btCancel.Visibility = monitor is not null ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
      _btInterrupt.Visibility = monitor is null ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
      _btAbort.Visibility = System.Windows.Visibility.Collapsed;
      _timerCounter = 0;
    }

    public BackgroundCancelDialogWpf(System.Threading.ThreadStart threadstart, IExternalDrivenBackgroundMonitor monitor)
    {
      _monitor = monitor;

      _threadStart = threadstart;
      _threadException = null;
      _thread = new System.Threading.Thread(MonitoredThreadEntryPoint);
      _thread.Start();

      //
      // Required for Windows Form Designer support
      //
      InitializeComponent();

      _btCancel.Visibility = monitor is not null ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
      _btInterrupt.Visibility = monitor is null ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
      _btAbort.Visibility = System.Windows.Visibility.Collapsed;
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
        return _threadException;
      }
    }

    private void MonitoredThreadEntryPoint()
    {
      try
      {
        _threadStart();
      }
      catch (Exception ex)
      {
        _threadException = ex;
      }
    }

    private void EhTimer(object? sender, EventArgs e)
    {
      _timerCounter++;

      if (_timerCounter == 50)
        Visibility = System.Windows.Visibility.Visible;

      if (_monitor is not null)
      {
        if (_monitor.HasReportText)
        {
          _guiProgressText.Text = _monitor.GetReportText();
          double frac = _monitor.GetProgressFraction();
          if (!double.IsNaN(frac))
          {
            _guiProgressFraction.Value = Math.Min(1, Math.Max(0, frac));
          }
        }
        _monitor.SetShouldReportNow();
      }

      if (!_thread.IsAlive)
      {
        _timer.Stop();
        DialogResult = _wasCancelledByUser ? true : false;
      }
    }

    private void EhInterruptClicked(object sender, RoutedEventArgs e)
    {
      if (_thread.IsAlive)
      {
        _wasCancelledByUser = true;
        _thread.Interrupt();
      }
      _btInterrupt.Visibility = System.Windows.Visibility.Collapsed;
      _btAbort.Visibility = System.Windows.Visibility.Visible;
    }

    private void EhCancelClicked(object sender, RoutedEventArgs e)
    {
      _wasCancelledByUser = true;
      if (_monitor is not null)
      {
        _monitor.SetCancellationPending();
        _btCancel.Visibility = System.Windows.Visibility.Collapsed;
        _btInterrupt.Visibility = System.Windows.Visibility.Visible;
      }
      else
      {
        _btCancel.Visibility = System.Windows.Visibility.Collapsed;
        _btInterrupt.Visibility = System.Windows.Visibility.Visible;
        EhInterruptClicked(sender, e);
      }
    }

    private void EhAbortClicked(object sender, RoutedEventArgs e)
    {
      if (_thread.IsAlive)
      {
        _wasCancelledByUser = true;
        _thread.Abort();
      }
    }

    private void EhDialogClosing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if (_thread.IsAlive)
        e.Cancel = true;
    }

    private void EhDialogLoaded(object sender, RoutedEventArgs e)
    {
      _timer = new System.Windows.Threading.DispatcherTimer(new TimeSpan(0, 0, 0, 0, 100), System.Windows.Threading.DispatcherPriority.Normal, EhTimer, Dispatcher);
      _timer.Start();
    }
  }
}
