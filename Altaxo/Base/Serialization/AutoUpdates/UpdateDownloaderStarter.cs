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

namespace Altaxo.Serialization.AutoUpdates
{
	/// <summary>
	/// Responsible to start the download of auto update files.
	/// </summary>
	public class UpdateDownloaderStarter
	{
		/// <summary>
		/// Starts the downloader.
		/// </summary>
		public static void Run()
		{
			var updateSettings = Current.PropertyService.Get(Altaxo.Settings.AutoUpdateSettings.SettingsStoragePath, new Altaxo.Settings.AutoUpdateSettings());

			if (!updateSettings.EnableAutoUpdates)
				return;

			if (updateSettings.DownloadIntervalInDays > 0)
			{
				var lastCheckUtc = PackageInfo.GetLastUpdateCheckTimeUtc(updateSettings.DownloadUnstableVersion);
				if ((DateTime.UtcNow - lastCheckUtc).TotalDays < updateSettings.DownloadIntervalInDays)
					return;
			}


			var entryAssembly = System.Reflection.Assembly.GetEntryAssembly();
			string assemblyLocation = entryAssembly.Location;
			string binPath = Path.GetDirectoryName(assemblyLocation);
			string downLoadExe = Path.Combine(binPath, "AltaxoUpdateDownloader.exe");

			var	args = string.Format("{0}\t{1}", PackageInfo.GetStableIdentifier(updateSettings.DownloadUnstableVersion), entryAssembly.GetName().Version);

			var processInfo = new System.Diagnostics.ProcessStartInfo(downLoadExe,args);

			processInfo.CreateNoWindow = true;
			processInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

			processInfo.CreateNoWindow = false;
			processInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;

			try
			{
				var proc = System.Diagnostics.Process.Start(processInfo);
				proc.PriorityClass = System.Diagnostics.ProcessPriorityClass.BelowNormal;
			}
			catch (Exception)
			{
			}
		}
	}
}
