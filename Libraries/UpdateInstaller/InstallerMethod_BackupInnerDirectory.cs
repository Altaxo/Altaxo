#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2020 Dr. Dirk Lellinger
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using ICSharpCode.SharpZipLib.Zip;

namespace Altaxo.Serialization.AutoUpdates
{
  /// <summary>
  /// Installation method that backups Altaxo's directory to a temporary directory in the AppData folder, then delete all files,
  /// and install the new files in this directory.
  /// </summary>
  public class InstallerMethod_BackupInnerDirectory : InstallerMethodBase, IUpdateInstaller
  {
    /// <summary>Full name of the zip file that contains the update files.</summary>
    private string _packageName;

    /// <summary>Full name of the Altaxo executable that should be updated.</summary>
    private string _altaxoExecutableFullName;

    /// <summary>Initializes a new instance of the <see cref="Downloader"/> class.</summary>
    /// <param name="loadUnstable">If set to <c>true</c>, the <see cref="Downloader"/> take a look for the latest unstable version. If set to <c>false</c>, it
    /// looks for the latest stable version.</param>
    /// <param name="currentProgramVersion">The version of the currently installed Altaxo program.</param>
    public InstallerMethod_BackupInnerDirectory(string eventName, string packageFullFileName, string altaxoExecutableFullFileName)
    {
      _eventName = eventName;
      _packageName = packageFullFileName;
      _altaxoExecutableFullName = altaxoExecutableFullFileName;

      _pathToInstallation = Path.GetDirectoryName(_altaxoExecutableFullName);
      if (!Path.IsPathRooted(_pathToInstallation))
        throw new ArgumentException("Path to Altaxo executable is not an absolute path!");

      string subDirAltaxoShouldResideIn = "" + Path.DirectorySeparatorChar + "bin";
      if (_pathToInstallation.ToLowerInvariant().EndsWith(subDirAltaxoShouldResideIn))
      {
        _pathToInstallation = _pathToInstallation.Substring(0, _pathToInstallation.Length - subDirAltaxoShouldResideIn.Length);
      }
      else
      {
        throw new ArgumentException("Altaxo executable doesn't reside in 'bin' directory, thus this is not a normal installation and is therefore not updated");
      }
    }

    /// <summary>Runs the installer</summary>
    /// <param name="ReportProgress">Used to report the installation progress. Arguments are the progress in percent and a progress message. If this function returns true, the program must thow an <see cref="System.Threading.ThreadInterruptedException"/>.</param>
    public void Run(Func<double, string, bool> ReportProgress)
    {
      var pathToInstallationInfo = new DirectoryInfo(_pathToInstallation);
      var allFiles = pathToInstallationInfo.GetFiles("*.*", SearchOption.AllDirectories);
      var allTopDirs = pathToInstallationInfo.GetDirectories();

      // ----------------------------------------------------------------------------------------------------------------------
      // Create an update lock file
      // ----------------------------------------------------------------------------------------------------------------------

      var updateLockFileName = Path.Combine(_pathToInstallation, UpdateLockFileName);
      FileStream updateLockFile = null;
      try
      {
        updateLockFile = new FileStream(updateLockFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
      }
      catch (Exception)
      {
        ReportProgress(0, "It seems that another process is already updating Altaxo. Therefore, this updater is stopping now.");
        return;
      }

      Action cleanupAction = () => { };

      try
      {

        WaitForReadyForInstallation(ReportProgress);

        var temporaryAppDataDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        temporaryAppDataDirectory = Path.Combine(temporaryAppDataDirectory, "Altaxo");
        temporaryAppDataDirectory = Path.Combine(temporaryAppDataDirectory, Guid.NewGuid().ToString());



        Directory.CreateDirectory(temporaryAppDataDirectory);

        // ----------------------------------------------------------------------------------------------------------------------
        // Copy all files into temporary app data folder
        // ----------------------------------------------------------------------------------------------------------------------

        var allFilesHash = new HashSet<FileInfo>(allFiles);



        int initialCount = allFilesHash.Count;
        while (allFilesHash.Count > 0)
        {
          var file = allFilesHash.First();
          var newFileName = GetFileNameForNewDirectory(temporaryAppDataDirectory, _pathToInstallation, file.FullName);
          var newPath = Path.GetDirectoryName(newFileName);

          try
          {
            Directory.CreateDirectory(newPath);
            File.Copy(file.FullName, newFileName);
            allFilesHash.Remove(file);
            if (ReportProgress(0 + 33.33 * (1 - allFilesHash.Count / initialCount), "Create backup copy"))
            {
              throw new ThreadInterruptedException();
            }

          }
          catch (Exception ex)
          {
            if (ReportProgress(0 + 33.33 * (1 - allFilesHash.Count / initialCount), $"Exception backing up file {file.FullName}, Details: {ex.Message}"))
            {
              throw new ThreadInterruptedException();
            }
            continue;
          }
        }

        // ----------------------------------------------------------------------------------------------------------------------
        // the Backup copy is ready,
        // now we remove all files from the installation directory
        // ----------------------------------------------------------------------------------------------------------------------

        cleanupAction += () =>
        {
          System.Windows.MessageBox.Show(
            $"Installation has failed or was cancelled by the user.\r\n" +
            $" A backup copy of the old installation can be found in:\r\n" +
            $"{temporaryAppDataDirectory}\r\n" +
            $"Please copy the files from this backup copy back to \"{_pathToInstallation}\"\r\n\r\n" +
            $"After you press OK, an explorer window is opened in the backup folder.",
            "Installation has failed or was cancelled",
            System.Windows.MessageBoxButton.OK,
            System.Windows.MessageBoxImage.Exclamation);

          System.Diagnostics.Process.Start("explorer.exe", "\"" + temporaryAppDataDirectory + "\"");
        };
        allFilesHash = new HashSet<FileInfo>(allFiles);
        while (allFilesHash.Count > 0)
        {
          var file = allFilesHash.First();
          try
          {
            File.Delete(file.FullName);
            allFilesHash.Remove(file);
            if (ReportProgress(33.33 + 33.33 * (1 - allFilesHash.Count / initialCount), "Delete old files"))
            {
              cleanupAction();
              throw new ThreadInterruptedException();
            }

          }
          catch (Exception ex)
          {
            if (ReportProgress(33.33 + 33.33 * (1 - allFilesHash.Count / initialCount), $"Exception deleting old file {file.FullName}, Details: {ex.Message}"))
            {
              throw new ThreadInterruptedException();
            }
            continue;
          }
        }

        // ----------------------------------------------------------------------------------------------------------------------
        // remove also the (now empty) directories
        // ----------------------------------------------------------------------------------------------------------------------
        var allDirsHash = new HashSet<DirectoryInfo>(allTopDirs);
        while (allDirsHash.Count > 0)
        {
          var dir = allDirsHash.First();
          try
          {
            Directory.Delete(dir.FullName, true);
            allDirsHash.Remove(dir);
            if (ReportProgress(66.66, "Delete old directories"))
            {
              cleanupAction();
              throw new ThreadInterruptedException();
            }
          }
          catch (Exception ex)
          {
            if (ReportProgress(66.66, $"Exception deleting old directory {dir.FullName}, Details: {ex.Message}"))
            {
              throw new ThreadInterruptedException();
            }
            continue;
          }
        }

        // ----------------------------------------------------------------------------------------------------------------------
        // now we first copy the new files from the package zip file, and then the modified files from the old installation
        // ----------------------------------------------------------------------------------------------------------------------

        try
        {
          // 1st copy the new files into the directory
          using (var fs = new FileStream(_packageName, FileMode.Open, FileAccess.Read, FileShare.Read))
          {
            fs.Seek(0, SeekOrigin.Begin);
            ExtractPackageFiles(fs, _pathToInstallation, (d, s) => ReportProgress(66.66 + 0.3333 * d, s), null);
          }

          // 2nd: Copy all files from the old installation, that are __not__ on its packing list
          CopyFilesNewOrChanged(temporaryAppDataDirectory, _pathToInstallation);
        }
        catch (Exception)
        {
          throw;
        }

        // ----------------------------------------------------------------------------------------------------------------------
        // if everything went successfull, then delete the temporary app data directory
        // ----------------------------------------------------------------------------------------------------------------------

        try
        {
          Directory.Delete(temporaryAppDataDirectory, true);
        }
        catch
        {

        }

        ReportProgress(100, "All new installation files extracted, auto update finished successfully!");
      }
      catch (Exception ex)
      {
        ReportProgress(0, "An exception was thrown during installation. Details:\r\n\r\n" + ex.Message);
        cleanupAction();
      }
      finally
      {
        try
        {
          updateLockFile?.Dispose();
          if (File.Exists(updateLockFileName))
            File.Delete(updateLockFileName);
        }
        catch { }
      }
    }
  }
}
