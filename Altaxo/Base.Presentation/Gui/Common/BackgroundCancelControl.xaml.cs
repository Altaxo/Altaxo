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
	/// Interaction logic for BackgroundCancelControl.xaml
	/// </summary>
	public partial class BackgroundCancelControl : UserControl
	{
		const int TimerTick_ms = 100;
		System.Threading.ThreadStart _threadStart;
		System.Exception _threadException;
		System.Threading.Thread _thread;
		IExternalDrivenBackgroundMonitor _monitor;
		private bool _wasCancelledByUser;
		System.Windows.Threading.DispatcherTimer _timer;
		int _showUpDownConter;

		public event Action<bool> ExecutionFinished;
		public event Action StartDelayExpired;

		public bool ExecutionInProgress
		{
			get
			{
				return _thread != null && _thread.IsAlive;
			}
		}

		public BackgroundCancelControl()
		{
			InitializeComponent();
		}

		public void StartExecution(Action<IProgressReporter> action, int milliSecondsUntilShowUp)
    {
			if (_thread != null && _thread.IsAlive)
				throw new ApplicationException("Background thread is still executed");

			_monitor = new ExternalDrivenBackgroundMonitor();
			_thread = new System.Threading.Thread(() => action(_monitor));

			_btCancel.Visibility = _monitor != null ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
			_btInterrupt.Visibility = _monitor == null ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
			_btAbort.Visibility = System.Windows.Visibility.Collapsed;

			_showUpDownConter = milliSecondsUntilShowUp / TimerTick_ms;
			_thread.Start();

			_timer = new System.Windows.Threading.DispatcherTimer(new TimeSpan(0, 0, 0, 0, TimerTick_ms), System.Windows.Threading.DispatcherPriority.Normal, EhTimer, this.Dispatcher);
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
			if (_showUpDownConter == 0)
			{
				if (null != StartDelayExpired)
					StartDelayExpired();
			}
			_showUpDownConter--;

			if (_monitor != null)
			{
				if (_monitor.HasReportText)
					this._progressText.Text = _monitor.GetReportText();
				_monitor.SetShouldReportNow();
			}

			if (!_thread.IsAlive)
			{
				_timer.Stop();
				if (ExecutionFinished != null)
					ExecutionFinished(_wasCancelledByUser?false:true);
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
	}
}
