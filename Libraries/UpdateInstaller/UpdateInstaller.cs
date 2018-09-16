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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;

namespace Altaxo.Serialization.AutoUpdates
{
  /// <summary>
  /// Responsible for installing the downloaded update.
  /// </summary>
  public class UpdateInstaller
  {
    private static System.Threading.EventWaitHandle _eventWaitHandle;

    private const string PackListRelativePath = "doc\\PackList.txt";

    /// <summary>Name of the event that signals to Altaxo that Altaxo now should shutdown in order to be updated.</summary>
    private string _eventName;

    /// <summary>Full name of the zip file that contains the update files.</summary>
    private string _packageName;

    /// <summary>Full name of the Altaxo executable that should be updated.</summary>
    private string _altaxoExecutableFullName;

    /// <summary>Full path to the installation directory (the directory in which the subdirs 'bin', 'doc', 'Addins' and 'data' resides).</summary>
    private string _pathToInstallation;

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
    public UpdateInstaller(string eventName, string packageFullFileName, string altaxoExecutableFullFileName)
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
      string pathToInstallation = Path.GetDirectoryName(_altaxoExecutableFullName);

      using (var fs = new FileStream(_packageName, FileMode.Open, FileAccess.Read, FileShare.Read))
      {
        fs.Seek(0, SeekOrigin.Begin);

        if (!_isWaitingForAltaxosEndDone)
        {
          // signal Altaxo, that we have the stream now
          SetEvent(_eventName);

          // warte auf das Close von Altaxo

          FileStream axoExe = null;
          DateTime startWaitingTime = DateTime.UtcNow;
          do
          {
            try
            {
              axoExe = new FileStream(_altaxoExecutableFullName, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (Exception)
            {
              if (ReportProgress(0, string.Format("Waiting for shutdown of Altaxo ... {0} s", Math.Round((DateTime.UtcNow - startWaitingTime).TotalSeconds, 1))))
                throw new System.Threading.ThreadInterruptedException("Installation cancelled by user");
              System.Threading.Thread.Sleep(250);
            }
            finally
            {
              if (null != axoExe)
                axoExe.Close();
            }
          } while (axoExe == null);
        }

        _isWaitingForAltaxosEndDone = true;
        ReportProgress(0, "Altaxo now has ended and is ready to be updated");

        if (!_isRemovingOldFilesDone)
        {
          // remove the old files
          ReportProgress(0, "Remove old installation files ...");
          RemoveOldInstallationFiles();
          if (ReportProgress(0, "Old installation files successfully removed!"))
            throw new System.Threading.ThreadInterruptedException("Installation cancelled by user");
        }
        _isRemovingOldFilesDone = true;

        if (!_isDeletingOrphanedDirectoriesDone)
        {
          // now delete all orphaned directories in the installation directory
          ReportProgress(0, "Deleting orphaned directories ...");
          try // note: it is not really essential to delete orphaned directories. Thus, if this fails, an error is reported, but the installation will be continued.
          {
            DeleteDirIfOrphaned(new DirectoryInfo(_pathToInstallation), ReportProgress);
            if (ReportProgress(0, "Orphaned directories successfully deleted!"))
              throw new System.Threading.ThreadInterruptedException("Installation cancelled by user");
          }
          catch (System.Threading.ThreadInterruptedException ex)
          {
            throw ex;
          }
          catch (Exception ex)
          {
            if (ReportProgress(0, "Failed to delete orphaned directories! Message: " + ex.Message))
              throw new System.Threading.ThreadInterruptedException("Installation cancelled by user");
          }
        }
        _isDeletingOrphanedDirectoriesDone = true;

        // and extract the new files
        ReportProgress(0, "Extracting new installation files ...");
        ExtractPackageFiles(fs, ReportProgress);
        ReportProgress(100, "All new installation files extracted, auto update finished successfully!");

        fs.Close();
      }
    }

    /// <summary>Creates and then sets the specified event.</summary>
    /// <param name="eventName">Name of the event.</param>
    public static void SetEvent(string eventName)
    {
      _eventWaitHandle = new System.Threading.EventWaitHandle(false, System.Threading.EventResetMode.ManualReset, eventName);
      _eventWaitHandle.Set();
    }

    /// <summary>Extracts the package files.</summary>
    /// <param name="fs">File stream of the package file (this is a zip file).</param>
    /// <param name="ReportProgress">Used to report the installation progress. Arguments are the progress in percent and a progress message. If this function returns true, the program must thow an <see cref="System.Threading.ThreadInterruptedException"/>.</param>
    private void ExtractPackageFiles(FileStream fs, Func<double, string, bool> ReportProgress)
    {
      var zipFile = new ZipFile(fs);
      byte[] buffer = new byte[4096];

      double totalNumberOfFiles = zipFile.Count;
      int currentProcessedFile = -1;
      foreach (ZipEntry entry in zipFile)
      {
        ++currentProcessedFile;
        var destinationFileName = Path.Combine(_pathToInstallation, entry.Name);
        var destinationPath = Path.GetDirectoryName(destinationFileName);
        ReportProgress(100 * currentProcessedFile / totalNumberOfFiles, string.Format("Updating file {0}", destinationFileName));

        if (!Directory.Exists(destinationPath))
          Directory.CreateDirectory(destinationPath);

        using (var entryStream = zipFile.GetInputStream(entry))
        {
          using (var destStream = new FileStream(destinationFileName, FileMode.Create, FileAccess.Write, FileShare.None))
          {
            int bytesReaded;
            while (0 != (bytesReaded = entryStream.Read(buffer, 0, buffer.Length)))
            {
              destStream.Write(buffer, 0, bytesReaded);
            }
          }
        }
        File.SetLastWriteTime(destinationFileName, entry.DateTime);
      }
    }

    /// <summary>Tests whether the pack list file exists in the installation directory (this is the file PackList.txt in the doc folder of the Altaxo installation).</summary>
    /// <returns>Returns <c>true</c> if the pack list file exists.</returns>
    public bool PackListFileExists()
    {
      return File.Exists(PackListFileFullName);
    }

    /// <summary>Gets the full name of the pack list file (this is the file PackList.txt in the doc folder of the Altaxo installation).</summary>
    /// <value>The full name of the pack list file.</value>
    public string PackListFileFullName
    {
      get
      {
        return Path.Combine(_pathToInstallation, PackListRelativePath);
      }
    }

    /// <summary>Tests whether or not the packs the list file is writeable (this is the file PackList.txt in the doc folder of the Altaxo installation).</summary>
    /// <returns>Returns <c>true</c> if the pack list file is writeable. If returning <c>false</c>, this is probably an indication that elevated privileges are required to update the installation.</returns>
    public bool PackListFileIsWriteable()
    {
      string fullName = Path.Combine(_pathToInstallation, PackListRelativePath);
      try
      {
        using (var fs = new FileStream(fullName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
        {
          fs.Close();
          return true;
        }
      }
      catch (Exception)
      {
      }
      return false;
    }

    /// <summary>Removes the old installation files. For this purpose, the pack list file of the old installation is opened, and all files that match the content of the pack list files are removed.</summary>
    private void RemoveOldInstallationFiles()
    {
      byte[] buff = null;
      using (var fso = new FileStream(Path.Combine(_pathToInstallation, PackListRelativePath), FileMode.Open, FileAccess.Read, FileShare.Read))
      {
        buff = new byte[fso.Length];
        fso.Read(buff, 0, buff.Length);
        fso.Close();
      }
      // thus we ensured that PackList.txt is closed, so that it can be deleted now

      var str = new StreamReader(new MemoryStream(buff));
      string line;
      while (null != (line = str.ReadLine()))
      {
        line = line.Trim();
        if (line.Length == 0)
          continue;
        // each line consist of the length of the file and the relative file name, separated by a tab
        int idx = line.IndexOf('\t');
        if (idx < 1)
          throw new InvalidOperationException("Unrecognized format of line in PackList.txt");
        long fileLength = long.Parse(line.Substring(0, idx));
        string fileName = line.Substring(idx + 1);

        var info = new FileInfo(Path.Combine(_pathToInstallation, fileName));

        if (info.Exists && info.Length == fileLength)
          info.Delete();
      }
    }

    /// <summary>Deletes the directory if it is orphaned, i.e. contains no files or subfolders. The call is recursive, i.e. prior to looking if this directory doesn't contain files or folders,
    /// the operation is applied to all subfolders of the directory.</summary>
    /// <param name="dir">The full path of the directory.</param>
    /// <returns>True if this directory was orphaned and thus was deleted.</returns>
    private static bool DeleteDirIfOrphaned(DirectoryInfo dir, Func<double, string, bool> ReportProgress)
    {
      bool isOrphaned = true;

      // delete orphaned subdirectories
      foreach (DirectoryInfo subdir in dir.GetDirectories())
      {
        if (!DeleteDirIfOrphaned(subdir, ReportProgress))
          isOrphaned = false;
      }

      if (dir.GetFiles().Length != 0)
        isOrphaned = false; // not orphaned
      if (dir.GetDirectories().Length != 0)
        isOrphaned = false; // not orphaned

      // if the directory is orphaned, the it can be deleted
      if (isOrphaned)
      {
        for (int i = 1, j = 3; j >= 0; ++i, --j)
        {
          try
          {
            dir.Delete();
            break;
          }
          catch (Exception ex)
          {
            ReportProgress(0, string.Format("Failed to delete orphaned directory '{0}' ({1}. try), Message: {2}. Please close all explorer windows or other programs that currently access this directory.", dir.FullName, i, ex.Message));
            System.Threading.Thread.Sleep(1000);
            if (j == 0)
              throw ex;
          }
        }
      }

      return isOrphaned;
    }
  }
}
