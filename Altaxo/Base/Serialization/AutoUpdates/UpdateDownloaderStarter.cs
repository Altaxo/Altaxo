﻿#region Copyright

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

#endregion Copyright

#nullable enable
using System;
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
      var updateSettings = Current.PropertyService.GetValue(Altaxo.Settings.AutoUpdateSettings.PropertyKeyAutoUpdate, Main.Services.RuntimePropertyKind.UserAndApplicationAndBuiltin, () => new Altaxo.Settings.AutoUpdateSettings());

      if (!updateSettings.EnableAutoUpdates)
        return;

      if (updateSettings.DownloadIntervalInDays > 0)
      {
        var lastCheckUtc = PackageInfo.GetLastUpdateCheckTimeUtc(updateSettings.DownloadUnstableVersion);
        if ((DateTime.UtcNow - lastCheckUtc).TotalDays < updateSettings.DownloadIntervalInDays)
          return;
      }


      string binPath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly()?.Location) ??
        throw new InvalidOperationException("Unable to determine directory name of entry assembly.");

      string downLoadExe = Path.Combine(binPath, "AltaxoUpdateDownloader.exe");

      var args = string.Format("{0}\t{1}", PackageInfo.GetStableIdentifier(updateSettings.DownloadUnstableVersion), System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version ?? throw new InvalidOperationException($"Unable to get version of entry assembly"));

      var processInfo = new System.Diagnostics.ProcessStartInfo(downLoadExe, args);

      if (updateSettings.ShowDownloadWindow)
      {
        processInfo.CreateNoWindow = false;
        processInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
      }
      else
      {
        processInfo.CreateNoWindow = true;
        processInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
      }

      try
      {
        var proc = System.Diagnostics.Process.Start(processInfo);
        if (proc is not null)
          proc.PriorityClass = System.Diagnostics.ProcessPriorityClass.BelowNormal;
      }
      catch (Exception ex)
      {
        if (Current.Console is { } console)
          console.WriteLine($"Exception while starting the update downloader: {ex.Message}");
      }
    }
  }
}
