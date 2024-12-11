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
using System.IO;
using System.Threading;

namespace Altaxo.Serialization.AutoUpdates
{
  /// <summary>
  /// Installation method using a temporary outer directory. In particular, it is tried to create a directory besides the installation directory,
  /// to install all files in it, and then to swap the new and the old installation directory, and then remove the old installation directory.
  /// This can only work if Altaxo is not installed in a root folder.
  /// </summary>
  public class InstallerMethod_UseOuterDirectory : InstallerMethodBase, IUpdateInstaller
  {
    /// <summary>Full path to the base installation directory (e.g. if AltaxoStartup.exe resides in C:\Altaxo\bin, the base directory is C:\.</summary>
    private string _pathToInstallationBaseDirectory;

    /// <summary>Initializes a new instance of the <see cref="Downloader"/> class.</summary>
    /// <param name="loadUnstable">If set to <c>true</c>, the <see cref="Downloader"/> take a look for the latest unstable version. If set to <c>false</c>, it
    /// looks for the latest stable version.</param>
    /// <param name="currentProgramVersion">The version of the currently installed Altaxo program.</param>
    public InstallerMethod_UseOuterDirectory(string eventName, string packageFullFileName, string altaxoExecutableFullFileName)
      : base(eventName, packageFullFileName, altaxoExecutableFullFileName)
    {

      string subDirAltaxoShouldResideIn = "" + Path.DirectorySeparatorChar + "bin";
      if (_pathToInstallation.ToLowerInvariant().EndsWith(subDirAltaxoShouldResideIn))
      {
        _pathToInstallation = _pathToInstallation.Substring(0, _pathToInstallation.Length - subDirAltaxoShouldResideIn.Length);
      }
      else
      {
        throw new ArgumentException("Altaxo executable doesn't reside in 'bin' directory, thus this is not a normal installation and is therefore not updated");
      }

      _pathToInstallationBaseDirectory = GetBaseDirectory(_pathToInstallation);

      if (_pathToInstallationBaseDirectory == _pathToInstallation)
        throw new InvalidOperationException("This installation method can not be used if Altaxo's subdirectories are located in a root directory");
    }

    /// <summary>Runs the installer</summary>
    /// <param name="ReportProgress">Used to report the installation progress. Arguments are the progress in percent and a progress message. If this function returns true, the program must thow an <see cref="System.Threading.ThreadInterruptedException"/>.</param>
    public void Run(Func<double, string, MessageKind, bool> ReportProgress)
    {
      var altaxoOldDirSub = Path.GetFileName(_pathToInstallation);
      var altaxoNewDirSub = altaxoOldDirSub + "_NextInstallation";
      var pathToNewInstallation = Path.Combine(_pathToInstallationBaseDirectory, altaxoNewDirSub);
      var pathToPreviousInstallation = Path.Combine(_pathToInstallationBaseDirectory, altaxoOldDirSub + "_PreviousInstallation");


      // ----------------------------------------------------------------------------------------------------------------------
      // Create an update lock file
      // ----------------------------------------------------------------------------------------------------------------------

      var updateLockFileName = Path.Combine(_pathToInstallation, UpdateLockFileName);
      var updateLockFileName_NewInstallation = Path.Combine(pathToNewInstallation, UpdateLockFileName);

      FileStream? updateLockFile = null;
      FileStream? updateLockFile_NewInstallation = null;

      try
      {
        updateLockFile = new FileStream(updateLockFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
      }
      catch (Exception)
      {
        ReportProgress(0, "It seems that another process is already updating Altaxo. Therefore, this updater is stopping now.", MessageKind.Warning);
        return;
      }

      Action cleanupAction = () => { };

      if (!_isWaitingForAltaxosEndDone)
      {
        // signal Altaxo, that we have the stream now
        SetEvent(_eventName);
      }


      try
      {
        // ----------------------------------------------------------------------------------------------------------------------
        // Preparation - this work can be done independently whether or not Altaxo is running
        // ----------------------------------------------------------------------------------------------------------------------

        if (!Directory.Exists(pathToNewInstallation))
        {
          Directory.CreateDirectory(pathToNewInstallation);
        }

        // ----------------------------------------------------------------------------------------------------------------------
        // Create a second update lock file in the temporary installation directory
        // ----------------------------------------------------------------------------------------------------------------------
        try
        {
          updateLockFile_NewInstallation = new FileStream(updateLockFileName_NewInstallation, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
        }
        catch (Exception)
        {
          ReportProgress(0, "It seems that another process is already updating Altaxo. Therefore, this updater is stopping now.", MessageKind.Warning);
          return;
        }

        // Remove the contents of the temporary installation directory (with the exception of the lock file, of course)
        RemoveContentsOfDirectory(new DirectoryInfo(pathToNewInstallation), removeDirectoryItself: false);


        // Install the content
        // 1st extract the content of the package file into the new directory
        using (var fs = new FileStream(_packageName, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
          fs.Seek(0, SeekOrigin.Begin);
          ExtractPackageFiles(fs, pathToNewInstallation, ReportProgress);
        }

        // 2nd: Copy all files from the old installation, that are __not__ on its packing list
        CopyFilesNewOrChanged(_pathToInstallation, pathToNewInstallation);


        // ----------------------------------------------------------------------------------------------------------------------
        // Wait for all files in the original installation directory to become writeable
        // ----------------------------------------------------------------------------------------------------------------------
        WaitForReadyForInstallation(ReportProgress);

        // warte auf das Close von Altaxo
        // ----------------------------------------------------------------------------------------------------------------------
        // Rename the old installation directory to ..previous, but keep it in case something fails with the installation
        // ----------------------------------------------------------------------------------------------------------------------

        cleanupAction = () =>
        {
          System.Windows.MessageBox.Show(
           $"The new Altaxo files were installed successfully in the temporary folder:\"\n" +
           $"\"{pathToNewInstallation}\"\r\n" +
           $"The old installation files have not been changed and are still located in the folder:\r\n" +
           $"\"{_pathToInstallation}\"\r\n\r\n" +
           $"You can either keep the old installation (then no action is neccessary), or you can keep the new installation. " +
           $"In order to keep the new installation, delete the old installation directory \"{_pathToInstallation}\", and then " +
           $"rename the new installation directory \"{pathToNewInstallation}\" to \"{_pathToInstallation}\"\r\n\r\n" +
           $"After you press OK, an explorer window is opened within the parent folder of the two installation folders.",
           "Installation has failed or was cancelled",
           System.Windows.MessageBoxButton.OK,
           System.Windows.MessageBoxImage.Exclamation);

          System.Diagnostics.Process.Start("explorer.exe", "\"" + _pathToInstallationBaseDirectory + "\"");
        };

        bool isLockFileDeleted = false;
        bool isDirectoryRenamed = false;
        DateTime startWaitingTime = DateTime.UtcNow;

        do
        {
          try
          {
            // for this we have to release the lock file (but we have still the lock file on the new directory
            updateLockFile?.Dispose();
            updateLockFile = null;
            if (File.Exists(updateLockFileName))
            {
              File.Delete(updateLockFileName);
            }
            isLockFileDeleted = true;

            Directory.Move(_pathToInstallation, pathToPreviousInstallation);
            isDirectoryRenamed = true;
          }
          catch (Exception ex)
          {
            if (ReportProgress(99,
              $"Renaming the old Altaxo installation directory \"{_pathToInstallation}\" fails currently.\r\n" +
              (isLockFileDeleted ? "" : $"Because the lock file {updateLockFileName} could not be deleted.\r\n") +
              $"The exception message is: {ex.Message}\r\n\r\n" +
              $"Please make sure that no files, explorer windows or other file manager windows are open in this directory or its subdirectories!\r\n\r\n" +
              $"Waiting for the directory to be released... {Math.Round((DateTime.UtcNow - startWaitingTime).TotalSeconds)} s",
              MessageKind.Warning
              ))
            {
              throw new ThreadInterruptedException("Installation cancelled by the user");
            }
          }
          System.Threading.Thread.Sleep(250);
        } while (!isDirectoryRenamed);



        // ----------------------------------------------------------------------------------------------------------------------
        // Rename the newly created installation directory to the original directory name
        // ----------------------------------------------------------------------------------------------------------------------
        cleanupAction = () =>
        {
          System.Windows.MessageBox.Show(
           $"The new Altaxo files were installed successfully, but the temporary folder which contains the new installation could not be renamed:\r\n" +
           $"\"{pathToNewInstallation}\" has to be renamed to \"{_pathToInstallation}\".\r\n" +
           $"Please rename it manually!\r\n\r\n" +
           $"After you press OK, an explorer window is opened within the parent folder of the temporary folder.",
           "Installation has failed or was cancelled",
           System.Windows.MessageBoxButton.OK,
           System.Windows.MessageBoxImage.Exclamation);

          System.Diagnostics.Process.Start("explorer.exe", "\"" + _pathToInstallationBaseDirectory + "\"");
        };

        isDirectoryRenamed = false;
        do
        {
          try
          {
            updateLockFile_NewInstallation?.Dispose();
            updateLockFile_NewInstallation = null;
            if (File.Exists(updateLockFileName_NewInstallation))
              File.Delete(updateLockFileName_NewInstallation);

            Directory.Move(pathToNewInstallation, _pathToInstallation);
            isDirectoryRenamed = true;
          }
          catch (Exception ex)
          {
            if (ReportProgress(99,
              $"Renaming the new Altaxo installation directory \"{pathToNewInstallation}\" fails currently.\r\n" +
              $"The exception message is: {ex.Message}\r\n\r\n" +
              $"Please make sure that no files, explorer windows or other file manager windows are open in this directory or its subdirectories!\r\n\r\n" +
              $"Waiting for the directory to be released... {Math.Round((DateTime.UtcNow - startWaitingTime).TotalSeconds)} s",
              MessageKind.Warning
              ))
            {
              Directory.Move(pathToPreviousInstallation, _pathToInstallation);             // this has not worked, thus we try to re-rename the old installation
              throw new ThreadInterruptedException("Installation cancelled by the user");
            }
            System.Threading.Thread.Sleep(1000);
          }
        } while (!isDirectoryRenamed);

        // ----------------------------------------------------------------------------------------------------------------------
        // Remove the old installation (now named ..previous)
        // ----------------------------------------------------------------------------------------------------------------------

        cleanupAction = () =>
        {
          System.Windows.MessageBox.Show(
           $"Installation was successfull, but the old installation directory:\r\n" +
           $"\"{pathToPreviousInstallation}\"\r\n" +
           "could not be removed. Please remove it manually.\r\n\r\n" +
           "After you press OK, an explorer window is opened within this folder.",
           "Installation successfull, but without cleanup",
           System.Windows.MessageBoxButton.OK,
           System.Windows.MessageBoxImage.Exclamation);

          System.Diagnostics.Process.Start("explorer.exe", "\"" + pathToPreviousInstallation + "\"");
        };


        // and if everything has worked, we remove the old directory
        RemoveContentsOfDirectory(new DirectoryInfo(pathToPreviousInstallation), removeDirectoryItself: true);

        ReportProgress(100, "All new installation files extracted, auto update finished successfully!", MessageKind.Info);
      }
      catch (Exception ex)
      {
        ReportProgress(0, "An exception was thrown during installation. Details:\r\n\r\n" + ex.Message, MessageKind.Error);
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

        try
        {
          updateLockFile_NewInstallation?.Dispose();
          if (File.Exists(updateLockFileName_NewInstallation))
            File.Delete(updateLockFileName_NewInstallation);
        }
        catch { }
      }
    }
  }
}
