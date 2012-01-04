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

namespace Altaxo.Gui.Settings
{
	/// <summary>
	/// Interaction logic for AutoUpdateOptionsControl.xaml
	/// </summary>
	public partial class AutoUpdateSettingsControl : UserControl, IAutoUpdateSettingsView
	{
		public AutoUpdateSettingsControl()
		{
			InitializeComponent();
		}

		private void EhEnableAutoUpdatesChanged(object sender, RoutedEventArgs e)
		{
			var content = _guiMainGroup.Content as FrameworkElement;
			if(null!=content)
				content.IsEnabled = true == _guiEnableAutoUpdates.IsChecked;
		}

		public bool EnableAutoUpdates
		{
			get
			{
				return true == _guiEnableAutoUpdates.IsChecked;
			}
			set
			{
				_guiEnableAutoUpdates.IsChecked = value;
			}
		}

		public bool DownloadUnstableVersion
		{
			get
			{
				return true == _guiDownloadStableAndUnstable.IsChecked;
			}
			set
			{
				if (value == true)
					_guiDownloadStableAndUnstable.IsChecked = true;
				else
					_guiDownloadStableOnly.IsChecked = true;
			}
		}

		public bool ShowDownloadWindow
		{
			get
			{
				return true == _guiShowDownloadWindow.IsChecked;
			}
			set
			{
				_guiShowDownloadWindow.IsChecked = value;
			}
		}

		public int DownloadInterval
		{
			get
			{
				if (true == _guiDownloadMonthly.IsChecked)
					return 30;
				else if (true == _guiDownloadWeekly.IsChecked)
					return 7;
				else
					return 0;
			}
			set
			{
				if (value >= 30)
					_guiDownloadMonthly.IsChecked = true;
				else if (value >= 7)
					_guiDownloadWeekly.IsChecked = true;
				else
					_guiDownloadAlways.IsChecked = true;
			}
		}

		public int InstallAt
		{
			get
			{
				if (true == _guiInstallAtStartup.IsChecked)
					return 1;
				else if (true == _guiInstallAtShutdown.IsChecked)
					return 2;
				else
					return 3;
			}
			set
			{
				switch (value)
				{
					case 1:
						_guiInstallAtStartup.IsChecked = true;
						break;
					case 2:
						_guiInstallAtShutdown.IsChecked = true;
						break;
					default:
						_guiInstallEitherStartupOrShutdown.IsChecked = true;
						break;
				}
			}
		}

		

	
	}
}
