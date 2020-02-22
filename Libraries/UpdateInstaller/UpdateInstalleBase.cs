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
  /// Responsible for installing the downloaded update.
  /// </summary>
  public class UpdateInstallerBase
  {
    protected const string PackListRelativePath = "doc\\PackList.txt";


    private static System.Threading.EventWaitHandle _eventWaitHandle;

    /// <summary>Full path to the installation directory (the directory in which the subdirs 'bin', 'doc', 'Addins' and 'data' resides).</summary>
    protected string _pathToInstallation;


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

    /// <summary>
    /// Determines whether the installation directory is writeable. Is is done by creating and then removing a temporary file.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if the installation directory is writeable; otherwise, <c>false</c>.
    /// </returns>
    public bool IsInstallationDirectoryWriteable()
    {

      // First of all, test whether the installation directory is writeable
      var tempFileName = Path.Combine(_pathToInstallation, Guid.NewGuid().ToString());

      try
      {
        using (var fs = new FileStream(tempFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
        {
          fs.Close();
          File.Delete(tempFileName);
          return true;
        }
      }
      catch (Exception)
      {
        return false;
      }
    }

    public bool IsParentDirectoryOfInstallationDirectoryWriteable()
    {
      var parentDir = GetBaseDirectory(_pathToInstallation);

      var tempDirName = Path.Combine(_pathToInstallation, Guid.NewGuid().ToString());

      try
      {
        Directory.CreateDirectory(tempDirName);
      }
      catch (Exception)
      {
        return false;
      }

      // First of all, test whether the installation directory is writeable
      var tempFileName = Path.Combine(tempDirName, Guid.NewGuid().ToString());

      try
      {
        using (var fs = new FileStream(tempFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
        {
          fs.Close();
          File.Delete(tempFileName);
        }
      }
      catch (Exception)
      {
        return false;
      }

      try
      {
        Directory.Delete(tempDirName, true);
      }
      catch (Exception)
      {
        return false;
      }

      return true;
    }

    /// <summary>Tests whether or not the packaging file is writeable (this is the file PackList.txt in the doc folder of the Altaxo installation).</summary>
    /// <returns>Returns <c>true</c> if the packaging file is writeable. If returning <c>false</c>, this is probably an indication that elevated privileges are required to update the installation.</returns>
    public bool IsPackListFileWriteable()
    {
      try
      {
        using (var fs = new FileStream(PackListFileFullName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
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


    /// <summary>Creates and then sets the specified event.</summary>
    /// <param name="eventName">Name of the event.</param>
    public static void SetEvent(string eventName)
    {
      _eventWaitHandle = new System.Threading.EventWaitHandle(false, System.Threading.EventResetMode.ManualReset, eventName);
      _eventWaitHandle.Set();
    }


    /// <summary>Deletes the directory if it is orphaned, i.e. contains no files or subfolders. The call is recursive, i.e. prior to looking if this directory doesn't contain files or folders,
    /// the operation is applied to all subfolders of the directory.</summary>
    /// <param name="dir">The full path of the directory.</param>
    /// <param name="ReportProgress">Function to report the progress. 1st arg is the progress (0..1), 2nd arg is the progress text. The function must return a boolean value, and if that value is true, the function is cancelled.</param>
    /// <returns>True if this directory was orphaned and thus was deleted.</returns>
    protected static bool DeleteDirIfOrphaned(DirectoryInfo dir, Func<double, string, bool> ReportProgress)
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

    public static string GetBaseDirectory(string directoryName)
    {
      return Path.GetDirectoryName(directoryName);
    }

    public static string GetLastPathPart(string fullFileName, string baseDirectory)
    {
      if (fullFileName.StartsWith(baseDirectory))
        return fullFileName.Substring(baseDirectory.Length);

      if (fullFileName.ToLowerInvariant().StartsWith(baseDirectory.ToLowerInvariant()))
        return fullFileName.Substring(baseDirectory.Length);

      throw new InvalidOperationException($"Can not determine last part part. File name is {fullFileName}, base directory is {baseDirectory}");
    }

    /// <summary>
    /// Removes the contents of the given directory, i.e. all files and all subdirectories in the given directory.
    /// The provided directory itself is not deleted.
    /// </summary>
    /// <param name="directory">The directory.</param>
    public static void RemoveContentsOfDirectory(DirectoryInfo directory, bool removeDirectoryItself)
    {
      if (removeDirectoryItself)
      {
        directory.Delete(true);
      }
      else
      {
        var subDirectories = directory.GetDirectories();

        foreach (var subDir in subDirectories)
        {
          subDir.Delete(true);
        }

        var allFiles = directory.GetFiles();
        foreach (var file in allFiles)
        {
          file.Delete();
        }
      }
    }

    /// <summary>Reads the packaging list of an Altaxo installation into a dictionary. Each entry consists of the relative file name,
    /// and the file length.</summary>
    protected static Dictionary<string, long> ReadPackagingList(string pathToInstallation)
    {
      var dictionary = new Dictionary<string, long>();

      byte[] buff = null;
      using (var fso = new FileStream(Path.Combine(pathToInstallation, PackListRelativePath), FileMode.Open, FileAccess.Read, FileShare.Read))
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

        dictionary[fileName] = fileLength;
      }

      return dictionary;
    }

    /// <summary>
    /// Analyzes the package list of the old installation, and then copies all files that are new or changed (in length) to the new installation directory.
    /// </summary>
    /// <param name="pathToOldInstallation">The path to old installation.</param>
    /// <param name="pathToNewInstallation">The path to new installation.</param>
    protected static void CopyFilesNewOrChanged(string pathToOldInstallation, string pathToNewInstallation)
    {
      var oldPackageFiles = ReadPackagingList(pathToOldInstallation);
      var files = new DirectoryInfo(pathToOldInstallation).GetFiles("*.*", SearchOption.AllDirectories);
      foreach (var file in files)
      {
        if (!file.FullName.StartsWith(pathToOldInstallation))
          continue;
        var relativeName = file.FullName.Substring(pathToOldInstallation.Length);
        relativeName = relativeName.TrimStart(new char[] { '\\' });

        if (!oldPackageFiles.ContainsKey(relativeName) || oldPackageFiles[relativeName] != file.Length)
          File.Copy(file.FullName, Path.Combine(pathToNewInstallation, relativeName));
      }
    }

    /// <summary>Extracts the package files.</summary>
    /// <param name="fs">File stream of the package file (this is a zip file).</param>
    /// <param name="ReportProgress">Used to report the installation progress. Arguments are the progress in percent and a progress message. If this function returns true, the program must thow an <see cref="System.Threading.ThreadInterruptedException"/>.</param>
    /// <param name="cleanupAction">Clean up actions that should be called before you throw an exception.</param>
    protected static void ExtractPackageFiles(FileStream fs, string pathToInstallation, Func<double, string, bool> ReportProgress, Action cleanupAction = null)
    {
      var zipFile = new ZipFile(fs);
      byte[] buffer = new byte[1024 * 1024];

      double totalNumberOfFiles = zipFile.Count;
      int currentProcessedFile = -1;
      foreach (ZipEntry entry in zipFile)
      {
        ++currentProcessedFile;
        var destinationFileName = Path.Combine(pathToInstallation, entry.Name);
        var destinationPath = Path.GetDirectoryName(destinationFileName);

        try
        {
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

          if (ReportProgress(100 * currentProcessedFile / totalNumberOfFiles, string.Format("Updating file {0}", destinationFileName)))
          {
            cleanupAction?.Invoke();
            throw new ThreadInterruptedException();
          }
        }
        catch (Exception)
        {
          cleanupAction();
          throw;
        }
      }
    }

    /// <summary>
    /// Waits for the moment that Altaxo's installation directory is ready for installation.
    /// </summary>
    public void WaitForReadyForInstallation(Func<double, string, bool> ReportProgress)
    {
      var startWaitingTime = DateTime.UtcNow;


      var dir = new DirectoryInfo(_pathToInstallation);
      var allFiles = dir.GetFiles("*.*", SearchOption.AllDirectories);

      var exeFiles = allFiles.Where(fi => (Path.GetExtension(fi.FullName).ToLowerInvariant() == ".exe")).ToArray();
      var othFiles = allFiles.Where(fi => !(Path.GetExtension(fi.FullName).ToLowerInvariant() == ".exe")).ToArray();

      var exeFileStreams = new FileStream[exeFiles.Length];

      for (; ; )
      {
        bool success = true;

        for (int i = 0; i < exeFiles.Length; ++i)
        {
          try
          {
            exeFileStreams[i] = new FileStream(exeFiles[i].FullName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
          }
          catch
          {
            for (int j = i - 1; j >= 0; --j)
            {
              exeFileStreams[j].Dispose();
              exeFileStreams[j] = null;
            }
            success = false;

            if (ReportProgress(0, string.Format("Waiting for shutdown of Altaxo ... {0} s \r\n\r\nFile still in use: \"{1}\"", Math.Round((DateTime.UtcNow - startWaitingTime).TotalSeconds, 1), exeFiles[i])))
              throw new System.Threading.ThreadInterruptedException("Installation cancelled by user");

            break;
          }
        }

        if (success)
          break;
        else
          System.Threading.Thread.Sleep(1000);
      }

      // now, while holding the handles to the exefiles, we try to open all other files


      for (; ; )
      {
        var success = true;
        foreach (var file in othFiles)
        {
          try
          {
            using (var s = new FileStream(file.FullName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
            {

            }
          }
          catch
          {
            if (ReportProgress(0, string.Format("Waiting for shutdown of Altaxo ... {0} s \r\n\r\nFile still in use: \"{1}\"", Math.Round((DateTime.UtcNow - startWaitingTime).TotalSeconds, 1), file.FullName)))
              throw new System.Threading.ThreadInterruptedException("Installation cancelled by user");

            success = false;
            break;
          }
        }

        if (success)
          break;
        else
          System.Threading.Thread.Sleep(1000);
      }

      // now release the handles to the exe-files
      for (int i = exeFileStreams.Length - 1; i >= 0; --i)
      {
        exeFileStreams[i].Dispose();
        exeFileStreams[i] = null;
      }

      // Altaxo is now ready for installation
    }


  }
}
