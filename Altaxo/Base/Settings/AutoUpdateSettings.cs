using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Settings
{
	/// <summary>
	/// Maintains the settings for the Altaxo auto update feature.
	/// </summary>
	public class AutoUpdateSettings
	{
		/// <summary>Name, under which this setting is stored in the Altaxo settings.</summary>
		public const string SettingsStoragePath = "Altaxo.Options.AutoUpdates";

		/// <summary>Initializes a new instance of the <see cref="AutoUpdateSettings"/> class with default values.</summary>
		public AutoUpdateSettings()
		{
			EnableAutoUpdates = true;
			InstallAtShutdown = true;
		}

		/// <summary>Gets or sets a value indicating whether to globally enable auto updates or not.</summary>
		/// <value>If <see langword="true"/>, auto updates are enabled. If <see langword="false"/>, auto updates are disabled.</value>
		public bool EnableAutoUpdates { get; set; }

		/// <summary>Gets or sets a value indicating whether to download only stable versions or to download stable and unstable versions.</summary>
		/// <value>If <see langword="false"/>, only stable versions will be downloaded and installed. If <see langword="true"/>, both stable and unstable versions will be downloaded  installed.</value>
		public bool DownloadUnstableVersion { get; set; }

		/// <summary>Gets or sets the download interval in days.</summary>
		/// <value>Time interval in days, after which Altaxo looks, whether a new version is available.</value>
		public int DownloadIntervalInDays { get; set; }

		/// <summary>Gets or sets a value indicating whether to show the download window.</summary>
		/// <value>If	<see langword="true"/>, the download window is visible (e.g. for debugging purposes). Otherwise, it is hidden.</value>
		public bool ShowDownloadWindow { get; set; }

		/// <summary>Gets or sets a value indicating whether to install a new version of Altaxo at startup of Altaxo.</summary>
		/// <value>If <see langword="true"/> and a new version is available, Altaxo asks at startup whether to install the download.</value>
		public bool InstallAtStartup { get; set; }

		/// <summary>Gets or sets a value indicating whether to install a new version of Altaxo at shutdown of Altaxo.</summary>
		/// <value>If <see langword="true"/> and a new version is available, Altaxo asks at shutdown whether to install the download.</value>
		public bool InstallAtShutdown { get; set; }
	}
}
