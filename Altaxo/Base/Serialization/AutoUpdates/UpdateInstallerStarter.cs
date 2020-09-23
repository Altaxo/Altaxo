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

#endregion Copyright

#nullable enable
using System;
using System.IO;
using System.Text;

namespace Altaxo.Serialization.AutoUpdates
{
  /// <summary>
  /// Responsible for handling the installation of the automatic updates of Altaxo.
  /// </summary>
  public class UpdateInstallerStarter : Altaxo.Main.Services.IAutoUpdateInstallationService
  {
    private const string UpdateInstallerFileName = "AltaxoUpdateInstaller.exe";

    /// <summary>Starts the installer program, when all presumtions are fullfilled.</summary>
    /// <param name="isAltaxoCurrentlyStarting">If set to <c>true</c>, Altaxo will be restarted after the installation is done.</param>
    /// <param name="commandLineArgs">Original command line arguments. Can be <c>null</c> when calling this function on shutdown.</param>
    /// <returns>True if the installer program was started. Then Altaxo have to be shut down immediately. Returns <c>false</c> if the installer program was not started.</returns>
    public bool Run(bool isAltaxoCurrentlyStarting, string[]? commandLineArgs)
    {
      var updateSettings = Current.PropertyService.GetValue(Altaxo.Settings.AutoUpdateSettings.PropertyKeyAutoUpdate, Main.Services.RuntimePropertyKind.UserAndApplicationAndBuiltin, () => new Altaxo.Settings.AutoUpdateSettings());

      var installationRequested = (isAltaxoCurrentlyStarting && updateSettings.InstallAtStartup) || (!isAltaxoCurrentlyStarting && updateSettings.InstallAtShutdown);
      if (!installationRequested)
        return false;

      bool loadUnstable = updateSettings.DownloadUnstableVersion;

      FileStream? versionFileStream = null;
      FileStream? packageStream = null;

      // try to lock the version file in the download directory, thus no other process can modify it
      try
      {
        var downloadFolder = PackageInfo.GetDownloadDirectory(loadUnstable);
        var versionFileName = Path.Combine(downloadFolder, PackageInfo.VersionFileName);

        versionFileStream = new FileStream(versionFileName, FileMode.Open, FileAccess.Read, FileShare.Read);

        var info = PackageInfo.GetPresentDownloadedPackage(versionFileStream, downloadFolder, out packageStream);

        if (info is null || packageStream is null)
          return false;

        var entryAssembly = System.Reflection.Assembly.GetEntryAssembly() ?? throw new InvalidOperationException("Unable to get entry assembly");
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
        var entryAssemblyFolder = Path.GetDirectoryName(entryAssembly.Location) ?? throw new InvalidOperationException("Unable to get directory of entry assembly");
        var installerFullSrcName = Path.Combine(entryAssemblyFolder, UpdateInstallerFileName);
        var installerFullDestName = Path.Combine(downloadFolder, UpdateInstallerFileName);
        File.Copy(installerFullSrcName, installerFullDestName, true);

        // both the version file and the package stream are locked now
        // so we can start the updater program

        var eventName = System.Guid.NewGuid().ToString();
        var waitForRemoteStartSignal = new System.Threading.EventWaitHandle(false, System.Threading.EventResetMode.ManualReset, eventName);

        var processInfo = new System.Diagnostics.ProcessStartInfo
        {
          FileName = installerFullDestName
        };
        var stb = new StringBuilder();
        stb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,
          "\"{0}\"\t\"{1}\"\t{2}\t{3}\t{4}\t\"{5}\"",
          eventName,
          packageStream.Name,
          updateSettings.ShowInstallationWindow ? 1 : 0,
          updateSettings.InstallationWindowClosingTime,
          isAltaxoCurrentlyStarting ? 1 : 0,
          entryAssembly.Location);
        if (isAltaxoCurrentlyStarting && commandLineArgs is not null && commandLineArgs.Length > 0)
        {
          foreach (var s in commandLineArgs)
            stb.AppendFormat("\t\"{0}\"", s);
        }
        processInfo.Arguments = stb.ToString();
        processInfo.CreateNoWindow = false;
        processInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;

        // Start the updater program
        var process = System.Diagnostics.Process.Start(processInfo);

        if (process is not null)
        {
          for (; ; )
          {
            // we wait until the update program signals that it has now taken the VersionInfo file
            if (waitForRemoteStartSignal.WaitOne(100))
              break;
            if (process.HasExited)
              return false; // then something has gone wrong or the user has closed the window
          }
        }

        return true;
      }
      catch (Exception)
      {
        return false;
      }
      finally
      {
        if (packageStream is not null)
          packageStream.Close();
        if (versionFileStream is not null)
          versionFileStream.Close();
      }
    }
  }
}
