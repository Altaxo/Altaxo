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
using ICSharpCode.SharpZipLib.Zip;

namespace Altaxo.Serialization.AutoUpdates
{
  /// <summary>
  /// Installation method using a temporary outer directory. In particular, it is tried to create a directory besides the installation directory,
  /// to install all files in it, and then to swap the new and the old installation directory, and then remove the old installation directory.
  /// This can only work if Altaxo is not installed in a root folder.
  /// </summary>
  public class InstallerMethod_UseOuterDirectory : UpdateInstallerBase, IUpdateInstaller
  {

    /// <summary>Name of the event that signals to Altaxo that Altaxo now should shutdown in order to be updated.</summary>
    private string _eventName;

    /// <summary>Full name of the zip file that contains the update files.</summary>
    private string _packageName;

    /// <summary>Full name of the Altaxo executable that should be updated.</summary>
    private string _altaxoExecutableFullName;

    /// <summary>Full path to the base installation directory (e.g. if AltaxoStartup.exe resides in C:\Altaxo\bin, the base directory is C:\.</summary>
    private string _pathToInstallationBaseDirectory;

    /// <summary>If true, this indicates that the waiting for the end of Altaxo was already done successfully. Thus in a possible second installation attempt we can skip this waiting.</summary>
    private bool _isWaitingForAltaxosEndDone;

    /// <summary>If true, this indicates that the removing of the old installation files was already done successfully. Thus in a possible second installation attempt we can skip this step.</summary>
    private bool _isRemovingOldFilesDone;

    /// <summary>If true, this indicates that the deletion of orphaned directories was already done successfully. Thus in a possible second installation attempt we can skip this step.</summary>
    private bool _isDeletingOrphanedDirectoriesDone;

    /// <summary>Initializes a new instance of the <see cref="Downloader"/> class.</summary>
    /// <param name="loadUnstable">If set to <c>true</c>, the <see cref="Downloader"/> take a look for the latest unstable version. If set to <c>false</c>, it
    /// looks for the latest stable version.</param>
    /// <param name="currentProgramVersion">The version of the currently installed Altaxo program.</param>
    public InstallerMethod_UseOuterDirectory(string eventName, string packageFullFileName, string altaxoExecutableFullFileName)
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

      _pathToInstallationBaseDirectory = GetBaseDirectory(_pathToInstallation);

      if (_pathToInstallationBaseDirectory == _pathToInstallation)
        throw new InvalidOperationException("This installation method can not be used if Altaxo's subdirectories are located in a root directory");
    }

    /// <summary>Runs the installer</summary>
    /// <param name="ReportProgress">Used to report the installation progress. Arguments are the progress in percent and a progress message. If this function returns true, the program must thow an <see cref="System.Threading.ThreadInterruptedException"/>.</param>
    public void Run(Func<double, string, bool> ReportProgress)
    {
      // ----------------------------------------------------------------------------------------------------------------------
      // Create an update lock file
      // ----------------------------------------------------------------------------------------------------------------------

      var updateLockFileName = Path.Combine(_pathToInstallation, "~AltaxoIsCurrentlyUpdated.txt");
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

      Action cleanupAction = () =>
      { try { updateLockFile?.Dispose(); if (File.Exists(updateLockFileName)) File.Delete(updateLockFileName); } catch { } };


      try
      {
        // ----------------------------------------------------------------------------------------------------------------------
        // Preparation - this work can be done independently whether or not Altaxo is running
        // ----------------------------------------------------------------------------------------------------------------------

        var altaxoOldDirSub = Path.GetFileName(_pathToInstallation);
        var altaxoNewDirSub = altaxoOldDirSub + "_NextInstallation";
        var pathToNewInstallation = Path.Combine(_pathToInstallationBaseDirectory, altaxoNewDirSub);
        var pathToPreviousInstallation = Path.Combine(_pathToInstallationBaseDirectory, altaxoOldDirSub + "_PreviousInstallation");

        if (Directory.Exists(pathToNewInstallation))
        {
          RemoveContentsOfDirectory(new DirectoryInfo(pathToNewInstallation), removeDirectoryItself: false);
        }

        // Install the content
        // 1st extract the content of the package file into the new directory
        using (var fs = new FileStream(_packageName, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
          fs.Seek(0, SeekOrigin.Begin);
          ExtractPackageFiles(fs, altaxoNewDirSub, ReportProgress);
        }

        // 2nd: Copy all files from the old installation, that are __not__ on its packing list
        CopyFilesNewOrChanged(_pathToInstallation, pathToNewInstallation);


        // now comes the critical point: we have to rename the old installation path, and then rename the new installation path

        if (!_isWaitingForAltaxosEndDone)
        {
          // signal Altaxo, that we have the stream now
          SetEvent(_eventName);

          // warte auf das Close von Altaxo

          bool isDirectoryRenamed = false;
          DateTime startWaitingTime = DateTime.UtcNow;
          do
          {
            try
            {
              // for this we have to release the lock file - TODO find another way
              updateLockFile?.Dispose();
              updateLockFile = null;
              if (File.Exists(updateLockFileName))
                File.Delete(updateLockFileName);

              Directory.Move(_pathToInstallation, pathToPreviousInstallation);
              isDirectoryRenamed = true;
            }
            catch (Exception)
            {
              if (ReportProgress(0, string.Format("Waiting for shutdown of Altaxo ... {0} s", Math.Round((DateTime.UtcNow - startWaitingTime).TotalSeconds, 1))))
                throw new System.Threading.ThreadInterruptedException("Installation cancelled by user");
              System.Threading.Thread.Sleep(250);
            }
            finally
            {
            }
          } while (!isDirectoryRenamed);
        }

        try
        {
          Directory.Move(pathToNewInstallation, _pathToInstallation);
        }
        catch (Exception ex)
        {
          // this has not worked, thus we try to re-rename the old installation
          Directory.Move(pathToPreviousInstallation, _pathToInstallation);
          throw;
        }

        // and if everything has worked, we remove the old directory
        RemoveContentsOfDirectory(new DirectoryInfo(pathToPreviousInstallation), removeDirectoryItself: true);
      }
      finally
      {
        cleanupAction();
      }
    }
  }
}
