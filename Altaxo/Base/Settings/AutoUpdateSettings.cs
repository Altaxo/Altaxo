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
#if DEBUG
			DownloadUnstableVersion = true;
#endif

			ConfirmInstallation = true;
			ShowInstallationWindow = true;
			InstallationWindowClosingTime = int.MaxValue;
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

		/// <summary>
		/// Gets or sets a value indicating whether the user has to confirm the installation of a new version
		/// </summary>
		/// <value>
		/// If <c>true</c>, the user is asked for confirmation before installing a new version.
		/// </value>
		public bool ConfirmInstallation { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether to show installation window. If <c>false</c>, the installation window will only be shown up if an error occurs.
		/// </summary>
		/// <value>
		/// <c>true</c> if the installation window is shown; otherwise, <c>false</c>.
		/// </value>
		public bool ShowInstallationWindow { get; set; }

		public int InstallationWindowClosingTime { get; set; }


		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AutoUpdateSettings), 0)]
		class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (AutoUpdateSettings)obj;
				info.AddValue("EnableAutoUpdates", s.EnableAutoUpdates);
				info.AddValue("DownloadUnstableVersion", s.DownloadUnstableVersion);
				info.AddValue("DownloadIntervalInDays", s.DownloadIntervalInDays);
				info.AddValue("ShowDownloadWindow", s.ShowDownloadWindow);

				info.AddValue("InstallAtStartup", s.InstallAtStartup);
				info.AddValue("InstallAtShutDown", s.InstallAtShutdown);
				info.AddValue("ConfirmInstallation", s.ConfirmInstallation);
				info.AddValue("ShowInstallationWindow", s.ShowInstallationWindow);
				info.AddValue("InstallationWindowClosingTime", s.InstallationWindowClosingTime);
			}
			protected virtual AutoUpdateSettings SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (o == null ? new AutoUpdateSettings() : (AutoUpdateSettings)o);

				s.EnableAutoUpdates = info.GetBoolean("EnableAutoUpdates");
				s.DownloadUnstableVersion = info.GetBoolean("DownloadUnstableVersion");
				s.DownloadIntervalInDays = info.GetInt32("DownloadIntervalInDays");
				s.ShowDownloadWindow = info.GetBoolean("ShowDownloadWindow");

				s.InstallAtStartup = info.GetBoolean("InstallAtStartup");
				s.InstallAtShutdown = info.GetBoolean("InstallAtShutDown");
				s.ConfirmInstallation = info.GetBoolean("ConfirmInstallation");
				s.ShowInstallationWindow = info.GetBoolean("ShowInstallationWindow");
				s.InstallationWindowClosingTime = info.GetInt32("InstallationWindowClosingTime");
				return s;
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{

				var s = SDeserialize(o, info, parent);
				return s;
			}
		}


		#endregion


	}
}
