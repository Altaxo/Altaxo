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
		/// <param name="isAltaxoBeingRestartedAfterInstallation">If set to <c>true</c>, Altaxo will be restarted after the installation is done.</param>
		/// <returns>True if the installer program was started. Then Altaxo have to be shut down immediately. Returns <c>false</c> if the installer program was not started.</returns>
		public static bool Run(bool isAltaxoBeingRestartedAfterInstallation)
		{
			bool loadUnstable = true;

			FileStream versionFileStream = null;
			FileStream packageStream = null;

			// try to lock the version File
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
				processInfo.Arguments = string.Format("\"{0}\"\t\"{1}\"\t{2}\t\"{3}\"", eventName, packageStream.Name, isAltaxoBeingRestartedAfterInstallation ? 1 : 0, entryAssembly.Location);
				processInfo.CreateNoWindow =false;
				processInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
				var process = System.Diagnostics.Process.Start(processInfo);

				// Start the updater program

				for (; ; )
				{
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
