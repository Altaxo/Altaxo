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

using Altaxo.Main.Services;

namespace Altaxo.Gui.Common
{
	/// <summary>
	/// Interaction logic for BackgroundCancelDialogWpf.xaml
	/// </summary>
	public partial class BackgroundCancelDialogWpf : Window
	{
		System.Threading.ThreadStart _threadStart;
		System.Exception _threadException;
		System.Threading.Thread _thread;
		IExternalDrivenBackgroundMonitor _monitor;
		private bool _wasCancelledByUser;
		System.Windows.Threading.DispatcherTimer _timer;
		int _timerCounter;

		public BackgroundCancelDialogWpf()
		{
			InitializeComponent();
		}

		  public BackgroundCancelDialogWpf(  System.Threading.Thread thread, IExternalDrivenBackgroundMonitor monitor)
    {
      _thread = thread;
      _monitor = monitor;
      //
      // Required for Windows Form Designer support
      //
      InitializeComponent();

			_btCancel.Visibility = monitor != null ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
			_btInterrupt.Visibility = monitor == null ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
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


			_btCancel.Visibility = monitor != null ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
			_btInterrupt.Visibility = monitor == null ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
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

		private void EhTimer(object sender, EventArgs e)
		{
			_timerCounter++;

			if (_timerCounter == 50)
				this.Visibility = System.Windows.Visibility.Visible;

			if (_monitor != null)
			{
				if(_monitor.HasReportText)
					this._progressText.Text = _monitor.GetReportText();
				_monitor.SetShouldReportNow();
			}

			if (!_thread.IsAlive)
			{
				_timer.Stop();
				this.DialogResult = _wasCancelledByUser ? true : false;
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
			this._wasCancelledByUser = true;
			if (_monitor != null)
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
			_timer = new System.Windows.Threading.DispatcherTimer(new TimeSpan(0, 0, 0, 0, 100), System.Windows.Threading.DispatcherPriority.Normal, EhTimer, this.Dispatcher);
			_timer.Start();
		}
	}
}
