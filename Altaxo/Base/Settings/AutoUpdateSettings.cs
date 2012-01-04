using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Settings
{
	public class AutoUpdateSettings
	{
		public const string SettingsStoragePath = "Altaxo.Options.AutoUpdates";

		public AutoUpdateSettings()
		{
			EnableAutoUpdates = true;
			InstallAtShutdown = true;
		}

		public bool EnableAutoUpdates { get; set; }
		public bool DownloadUnstableVersion { get; set; }
		public int DownloadIntervalInDays { get; set; }
		public bool ShowDownloadWindow { get; set; }
		public bool InstallAtStartup { get; set; }
		public bool InstallAtShutdown { get; set; }
	}
}
