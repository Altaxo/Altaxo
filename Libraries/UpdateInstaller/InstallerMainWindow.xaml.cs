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
#endregion

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
			Loaded += new RoutedEventHandler(EhLoaded);
		}

		void EhLoaded(object sender, RoutedEventArgs e)
		{
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

		public void SetErrorMessage(string message)
		{
			_guiProgress.Value = 0;
			_guiMessages.AppendText(message);
			_guiMessages.Background = new SolidColorBrush(Colors.LightPink);
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
