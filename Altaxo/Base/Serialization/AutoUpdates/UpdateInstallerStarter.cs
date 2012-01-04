using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace Altaxo.Serialization.AutoUpdates
{
	/// <summary>
	/// Responsible for handling the installation of the automatic updates of Altaxo.
	/// </summary>
	public class UpdateInstallerStarter
	{
		const string UpdateInstallerFileName = "AltaxoUpdateInstaller.exe";

		/// <summary>Starts the installer program, when all presumtions are fullfilled.</summary>
		/// <param name="isAltaxoCurrentlyStarting">If set to <c>true</c>, Altaxo will be restarted after the installation is done.</param>
		/// <param name="commandLineArgs">Original command line arguments. Can be <c>null</c> when calling this function on shutdown.</param>
		/// <returns>True if the installer program was started. Then Altaxo have to be shut down immediately. Returns <c>false</c> if the installer program was not started.</returns>
		public static bool Run(bool isAltaxoCurrentlyStarting, string[] commandLineArgs)
		{
			var updateSettings = Current.PropertyService.Get(Altaxo.Settings.AutoUpdateSettings.SettingsStoragePath, new Altaxo.Settings.AutoUpdateSettings());

			if ((updateSettings.InstallAtStartup && !isAltaxoCurrentlyStarting) &&
				 (updateSettings.InstallAtShutdown && isAltaxoCurrentlyStarting))
				return false;
			
			bool loadUnstable = updateSettings.DownloadUnstableVersion;


			FileStream versionFileStream = null;
			FileStream packageStream = null;

			// try to lock the version file in the download directory, thus no other process can modify it
			try
			{
				var downloadFolder = PackageInfo.GetDownloadDirectory(loadUnstable);
				var versionFileName = Path.Combine(downloadFolder, PackageInfo.VersionFileName);

				versionFileStream = new FileStream(versionFileName, FileMode.Open, FileAccess.Read, FileShare.Read);

				var info = PackageInfo.GetPresentDownloadedPackage(versionFileStream, downloadFolder, out packageStream);

				if (null == info || null == packageStream)
					return false;

				var entryAssembly = System.Reflection.Assembly.GetEntryAssembly();
				var entryAssemblyVersion = entryAssembly.GetName().Version;

				if (info.Version <= entryAssemblyVersion)
					return false; // no need to update


				// copy the Updater executable to the download folder
				var entryAssemblyFolder = Path.GetDirectoryName(entryAssembly.Location);
				var installerFullSrcName = Path.Combine(entryAssemblyFolder, UpdateInstallerFileName);
				var installerFullDestName = Path.Combine(downloadFolder, UpdateInstallerFileName);
				File.Copy(installerFullSrcName, installerFullDestName, true);


				// both the version file and the package stream are locked now
				// so we can start the updater program

				var eventName = System.Guid.NewGuid().ToString();
				var waitForRemoteStartSignal = new System.Threading.EventWaitHandle(false, System.Threading.EventResetMode.AutoReset, eventName);

				var processInfo = new System.Diagnostics.ProcessStartInfo();
				processInfo.FileName = installerFullDestName;
				StringBuilder stb = new StringBuilder();
				stb.AppendFormat("\"{0}\"\t\"{1}\"\t{2}\t\"{3}\"", eventName, packageStream.Name, isAltaxoCurrentlyStarting ? 1 : 0, entryAssembly.Location);
				if (isAltaxoCurrentlyStarting && commandLineArgs != null && commandLineArgs.Length > 0)
				{
					foreach (var s in commandLineArgs)
						stb.AppendFormat("\t\"{0}\"", s);
				}
				processInfo.Arguments = stb.ToString();
				processInfo.CreateNoWindow =false;
				processInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;

				// Start the updater program
				var process = System.Diagnostics.Process.Start(processInfo);

				for (; ; )
				{
					// we wait until the update program signals that it has now taken the VersionInfo file
					if (waitForRemoteStartSignal.WaitOne(100))
						break;
					if (process.HasExited)
						return false; // then something has gone wrong or the user has closed the window
				}

				return true;
			}

			catch (Exception ex)
			{
				return false;
			}
			finally
			{
				if (null != packageStream)
					packageStream.Close();
				if (null != versionFileStream)
					versionFileStream.Close();
			}
		}
	}
}
