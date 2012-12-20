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

			var installationRequested = (isAltaxoCurrentlyStarting && updateSettings.InstallAtStartup) || (!isAltaxoCurrentlyStarting && updateSettings.InstallAtShutdown);
			if(!installationRequested)
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

				if (updateSettings.ConfirmInstallation)
				{
					var question = string.Format(
						"A new Altaxo update is available (from current version {0} to new {1} version {2}).\r\n\r\n" +
						"If you don't want to have auto updates, please deactivate them by choosing 'Tools'->'Options' menu in Altaxo.\r\n" +
						"\r\n" +
					"Do you want to update to {1} version {2} now?", entryAssemblyVersion, PackageInfo.GetStableIdentifier(info.IsUnstableVersion).ToLower(), info.Version);

					if (false == Current.Gui.YesNoMessageBox(question, "Altaxo update available", true))
						return false; // user don't want to update
				}

				// copy the Updater executable to the download folder
				var entryAssemblyFolder = Path.GetDirectoryName(entryAssembly.Location);
				var installerFullSrcName = Path.Combine(entryAssemblyFolder, UpdateInstallerFileName);
				var installerFullDestName = Path.Combine(downloadFolder, UpdateInstallerFileName);
				File.Copy(installerFullSrcName, installerFullDestName, true);


				// both the version file and the package stream are locked now
				// so we can start the updater program

				var eventName = System.Guid.NewGuid().ToString();
				var waitForRemoteStartSignal = new System.Threading.EventWaitHandle(false, System.Threading.EventResetMode.ManualReset, eventName);

				var processInfo = new System.Diagnostics.ProcessStartInfo();
				processInfo.FileName = installerFullDestName;
				StringBuilder stb = new StringBuilder();
				stb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,
					"\"{0}\"\t\"{1}\"\t{2}\t{3}\t{4}\t\"{5}\"", 
					eventName, 
					packageStream.Name, 
					updateSettings.ShowInstallationWindow ? 1 : 0,
					updateSettings.InstallationWindowClosingTime,
					isAltaxoCurrentlyStarting ? 1 : 0, 
					entryAssembly.Location);
				if (isAltaxoCurrentlyStarting && commandLineArgs != null && commandLineArgs.Length > 0)
				{
					foreach (var s in commandLineArgs)
						stb.AppendFormat("\t\"{0}\"", s);
				}
				processInfo.Arguments = stb.ToString();
				processInfo.CreateNoWindow = false;
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

			catch (Exception)
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
