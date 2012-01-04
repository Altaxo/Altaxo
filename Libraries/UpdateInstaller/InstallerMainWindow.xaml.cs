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
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class InstallerMainWindow : Window
	{
		public UpdateInstaller _installer;
		System.Threading.Tasks.Task _installerTask;

		public double _progress;
		public string _message = string.Empty;
		System.Windows.Threading.DispatcherTimer _timer;

		public InstallerMainWindow()
		{
			InitializeComponent();
		}

		public void SetErrorMessage(string message)
		{
			_guiProgress.Value = 0;
			_guiMessages.AppendText(message);
			_guiMessages.Background = new SolidColorBrush(Colors.LightPink);
		}


		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			if (null != _installer)
			{
				_btOk.IsEnabled = false;

				_timer = new System.Windows.Threading.DispatcherTimer();
				_timer.Tick += new EventHandler(EhTimerTick);
				_timer.Interval = new TimeSpan(0, 0, 0, 0, 250);
				_timer.Start();

				_installerTask = new System.Threading.Tasks.Task(RunInstaller);
				_installerTask.Start();
			}
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			if (_installerTask != null && !_installerTask.IsCompleted)
			{
				e.Cancel = true;
			}

			base.OnClosing(e);
		}

		void EhTimerTick(object sender, EventArgs e)
		{
			_guiProgress.Value = _progress;
			_guiMessages.Text = _message.ToString();

			if (_installerTask.IsCompleted)
			{
				_timer.Tick -= EhTimerTick;
				_timer.Stop();
				_timer = null;

				var exception = _installerTask.Exception;
				_btOk.IsEnabled = true;
				_installerTask.Dispose();
				_installerTask = null;

				if (null != exception)
					throw exception;
			}
		}


		private void ReportProgressAsync(double progress, string message)
		{
			_progress = progress;
			_message = message;
		}

		private void RunInstaller()
		{
			_installer.Run(ReportProgressAsync);
		}

		private void EhOk(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
