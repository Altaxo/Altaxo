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
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace Altaxo.Serialization.AutoUpdates
{
  /// <summary>
  /// Responsible for the download of the auto update files.
  /// </summary>
  public class UpdateDownloader
  {
    private string _storagePath;
    private Version _currentVersion;
    private bool _isDownloadOfPackageCompleted;
    private bool _isUnstableVersion;

    /// <summary>Initializes a new instance of the <see cref="UpdateDownloader"/> class.</summary>
    /// <param name="loadUnstable">If set to <c>true</c>, the <see cref="UpdateDownloader"/> take a look for the latest unstable version. If set to <c>false</c>, it
    /// looks for the latest stable version.</param>
    /// <param name="currentProgramVersion">The version of the currently installed Altaxo program.</param>
    public UpdateDownloader(bool loadUnstable, Version currentProgramVersion)
    {
      _isUnstableVersion = loadUnstable;
      _currentVersion = currentProgramVersion;

      _storagePath = PackageInfo.GetDownloadDirectory(loadUnstable);
    }

    /// <summary>Runs the <see cref="UpdateDownloader"/>.</summary>
    /// <remarks>
    /// The download is done in steps:
    /// <para>Firstly, the appropriate version file in the application data directory is locked,
    /// so that no other program can use it, until this program ends.</para>
    /// <para>Then, the version file is downloaded from the remote location.</para>
    /// <para>If there is already a valid version file in the download directory,
    /// and the version obtained from the remote version file is equal to the version obtained from the version file in the download directory,
    /// then the package was already downloaded before. Then we only check that the package file is also present and that it has the appropriate hash sum.</para>
    /// <para>Else, if the version obtained from the remote version file is higher than the program's current version,
    /// we download the package file from the remote location.</para>
    /// </remarks>
    /// <returns>A task that represents the asynchronous download workflow.</returns>
    public async Task RunAll()
    {
      // Verify that the download directory exists, and if not, create it. 
      if (!Directory.Exists(_storagePath))
      {
        Directory.CreateDirectory(_storagePath);
        SetDownloadDirectoryAccessRights(_storagePath);
      }

      // Full filename of the version file 'VersionInfo.json' in the download directory
      var versionFileFullName = Path.Combine(_storagePath, PackageInfo.VersionFileName);

      // Delete all files in the download directory that have the wrong length or hash, according to the existing version file.
      DeleteInvalidFilesInDownloadDirectory();



      // download the latest version information from sourceforge and github simultaneously,
      // wait at least 15 seconds, and then cancel the slower request after the first successful result is available
      var latestVersions = await GetLatestVersionsAsync();

      // from all parsed versions, choose that one that matches the requirements
      var bestPackages = latestVersions
        .Where(v => v is not null && v.versionData is not null)
        .Select(v => new PackageWithVersionDataAndBaseUrl(versionData: v.versionData, package: SystemRequirements.TryGetHighestVersion(v.packages), baseUrl: v.baseUrl))
        .ToArray();

      if (bestPackages.Length == 0)
      {
        Console.WriteLine("There are no files to download currently available on the remote servers. Please try again later.");
        return; // then there is nothing to do
      }


      Console.WriteLine("The best remote package versions are:");
      foreach (var entry in bestPackages)
      {
        Console.WriteLine($"{entry.package.FileNameOfPackageZipFile} from {entry.baseUrl}");
      }

      // now determine the highest version among the best packages, and only download this one, if there are different versions available from sourceforge and github
      var highestVersion = bestPackages.Max(v => v.package.Version);
      bestPackages = bestPackages.Where(v => v.package.Version == highestVersion).ToArray();

      // now sort out those packages where the file is already present in the download directory
      bestPackages = bestPackages
        .Where(v => v.package.Version > _currentVersion)
        .Where(v => !File.Exists(Path.Combine(_storagePath, v.package.FileNameOfPackageZipFile)))
        .ToArray();

      if (bestPackages.Length == 0)
      {
        Console.WriteLine("The package file that could be downloaded is already present and up to date. Therefore, no download is needed.");
        return; // then there is nothing to do
      }

      // because at SourceForge we have to separate download folders, it can happen that
      // now we have the same package in the unstable and in the stable version.
      // In this case, we only want to download the stable version.
      // Thus, if two packages have the same host name and the same filename, then the package which is stable wins over that which is unstable
      var dict = new Dictionary<(string host, string fileName), PackageWithVersionDataAndBaseUrl>();
      foreach (var entry in bestPackages)
      {
        var uri = new Uri(entry.baseUrl);
        var key = (host: uri.Host, fileName: entry.package.FileNameOfPackageZipFile);

        if (dict.TryGetValue(key, out var existingPackage))
        {
          if (existingPackage.package.IsUnstableVersion && !entry.package.IsUnstableVersion)
          {
            dict[key] = entry;
          }
        }
        else
        {
          dict[key] = entry;
        }
      }
      bestPackages = dict.Values.ToArray();


      // if there are multiple best packages with the same version, we make sure that the file hash is equal, otherwise we should raise an alarm
      for (int i = 1; i < bestPackages.Length; i++)
      {
        if (bestPackages[0].package.FileNameOfPackageZipFile == bestPackages[i].package.FileNameOfPackageZipFile && bestPackages[0].package.IsUnstableVersion == bestPackages[i].package.IsUnstableVersion &&
          (bestPackages[0].package.FileLength != bestPackages[i].package.FileLength || bestPackages[0].package.Hash != bestPackages[i].package.Hash)
           )
        {
          Console.WriteLine("A T T E N T I O N ! ! ! !\r\nThere are multiple packages with the same version, but different hash sums. This should not happen, thus we do not download any package. Please open an issue in Altaxo's GitHub pages!");

          Console.WriteLine("Details:");
          Console.WriteLine($"Both packages have the same file name: {bestPackages[0].package.FileNameOfPackageZipFile} and unstable flag: {bestPackages[0].package.IsUnstableVersion}");
          Console.WriteLine($"Both packages have length: {bestPackages[0].package.FileLength} and {bestPackages[i].package.FileLength}");
          Console.WriteLine($"Both packages have hash: {bestPackages[0].package.Hash} and {bestPackages[i].package.Hash}");

          return;
        }
      }


      Console.WriteLine("The package versions need to download are:");
      foreach (var entry in bestPackages)
      {
        Console.WriteLine($"{entry.package.FileNameOfPackageZipFile} from {entry.baseUrl}");
      }

      // now we try to download the best matching version simultaneously from either sourceforge or github, starting with github
      CleanDirectory(Path.Combine(_storagePath, PackageInfo.VersionFileName)); // Clean old downloaded files from the directory

      // if all packages have the same filename with the same length, we can use the adaptive parallel downloader

      bool canUseAdaptiveParallelDownloader = bestPackages.Length > 1 &&
                                              bestPackages.All(p => p.package.FileNameOfPackageZipFile == bestPackages[0].package.FileNameOfPackageZipFile &&
                                                                    p.package.FileLength == bestPackages[0].package.FileLength &&
                                                                    p.package.Hash == bestPackages[0].package.Hash
                                              );

      if (canUseAdaptiveParallelDownloader)
      {
        var downloader = new PackageDownloaderAdaptiveParallel(_storagePath, bestPackages);
        try
        {
          await downloader.DownloadPackageFiles();
          return;
        }
        catch { }
      }

      {
        // if the adaptive parallel downloader cannot be used, or if it fails,
        // we use the normal parallel downloader.
        var downloader = new PackageDownloaderParallel(_storagePath);
        await downloader.DownloadPackageFiles(bestPackages);
      }
    }

    private async Task<PackagesWithVersionDataAndBaseUrl?[]> GetLatestVersionsAsync()
    {
      using var waitAllCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(2000));
      using var waitAnyCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(60000));

      var sourceForgeDownLoader = new VersionDataDownloader_SourceForge(_isUnstableVersion);
      var githubDownLoader = new VersionDataDownloader_GitHub(_isUnstableVersion);

      var pendingTasks = new List<Task<PackagesWithVersionDataAndBaseUrl?>>
      {
        sourceForgeDownLoader.GetLatestVersionAsync(waitAnyCancellationTokenSource.Token),
        githubDownLoader.GetLatestVersionAsync(waitAnyCancellationTokenSource.Token)
      };

      if (_isUnstableVersion)
      {
        // in Sourceforge (not in Github) we have separate version files for stable and unstable versions.
        // Thus, if we are looking for an unstable version, then we also take a look for the latest stable version, because maybe the latest stable version is already higher than the currently installed version, and then there is no need to download the unstable version.
        pendingTasks.Add(new VersionDataDownloader_SourceForge(loadUnstable: false).GetLatestVersionAsync(waitAnyCancellationTokenSource.Token));
      }

      try
      {
        await Task.WhenAll(pendingTasks).WaitAsync(waitAllCancellationTokenSource.Token);
        await WhenAnySuccessful(pendingTasks);
      }
      catch (TaskCanceledException)
      {
        // ignore, because we expect that at least one of the tasks will complete within the given time, and then we cancel the other task
      }
      catch (Exception ex)
      {
        Console.WriteLine($"An error occurred while retrieving the latest versions: {ex.Message}");
      }

      waitAnyCancellationTokenSource.Cancel();

      var result = new List<PackagesWithVersionDataAndBaseUrl?>();
      foreach (var pendingTask in pendingTasks)
      {
        if (pendingTask.IsCompletedSuccessfully)
        {
          result.Add(pendingTask.Result);
        }
      }

      return result.ToArray();
    }

    /// <summary>
    /// Deletes files from the download directory whose length or hash no longer matches the version metadata.
    /// </summary>
    public void DeleteInvalidFilesInDownloadDirectory()
    {
      var versionFileFullName = Path.Combine(_storagePath, PackageInfo.VersionFileName);
      if (!File.Exists(versionFileFullName))
      {
        return;
      }

      using var versionFileStream = new FileStream(versionFileFullName, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
      var packages = PackageInfo.ReadPackagesFromJson(versionFileStream);
      foreach (var package in packages)
      {
        var fileRemoved = package.RemovePackageZipFileIfLengthOrHashDiffer(_storagePath);

        if (!string.IsNullOrEmpty(fileRemoved))
        {
          Console.WriteLine($"Removed file {fileRemoved} from download directory, because it has an invalid length or hash.");
        }
      }
    }
    /// <summary>It is necessary to modify the download directory access rights because, by default, only the creator or owner has the right to change the newly created directory.
    /// However, we want to allow all authenticated users to download auto updates. Thus, we modify the access rights of the download directory so that authenticated users have the right to modify files and folders.</summary>
    /// <param name="downloadDirectory">The download directory.</param>
    private void SetDownloadDirectoryAccessRights(string downloadDirectory)
    {
      try
      {
        var authenticatedUser = new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null);
        var inheritance = InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit;
        var propagation = PropagationFlags.None;
        var security = new DirectorySecurity();
        security.AddAccessRule(new FileSystemAccessRule(authenticatedUser, FileSystemRights.Modify, inheritance, propagation, AccessControlType.Allow));
        security.SetAccessRuleProtection(false, true);
        new DirectoryInfo(downloadDirectory).SetAccessControl(security);
      }
      catch (Exception)
      {
      }
    }

    /// <summary>Called when the download of the package file is completed.</summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.ComponentModel.AsyncCompletedEventArgs"/> instance containing the event data.</param>
    private void EhDownloadOfPackageFileCompleted(object? sender, System.ComponentModel.AsyncCompletedEventArgs e)
    {
      _isDownloadOfPackageCompleted = true;
    }

    /// <summary>Outputs the download progress to the console.</summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Net.DownloadProgressChangedEventArgs"/> instance containing the event data.</param>
    private void EhDownloadOfPackageFileProgressChanged(object? sender, System.Net.DownloadProgressChangedEventArgs e)
    {
      Console.Write("{0}%\r", e.ProgressPercentage);
    }

    /// <summary>Cleans the download directory from all package files.</summary>
    /// <param name="withExceptionOfThisFile">A file name, which should not be removed.</param>
    private void CleanDirectory(string withExceptionOfThisFile)
    {
      Console.Write("Cleaning download directory ...");
      try
      {
        var files = Directory.GetFiles(_storagePath, "Altaxo*.zip");
        foreach (var fileName in files)
        {
          if (0 == string.Compare(fileName, withExceptionOfThisFile, true))
            continue;
          File.Delete(fileName);
        }
      }
      catch (Exception)
      {
      }
      Console.WriteLine("done!");
    }
    /// <summary>
    /// Returns the result of the first task that completes successfully.
    /// </summary>
    /// <typeparam name="T">The result type returned by the tasks.</typeparam>
    /// <param name="tasks">The tasks to observe.</param>
    /// <param name="cancellationToken">A token that cancels the wait operation.</param>
    /// <returns>The result of the first successfully completed task.</returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    /// <exception cref="AggregateException">Thrown when all tasks fail.</exception>
    public static async Task<T> WhenAnySuccessful<T>(IEnumerable<Task<T>> tasks, CancellationToken cancellationToken = default)
    {
      var remaining = tasks.ToHashSet();

      while (remaining.Count > 0)
      {
        cancellationToken.ThrowIfCancellationRequested();

        var completed = await Task.WhenAny(remaining).WaitAsync(cancellationToken);

        if (completed.IsCompletedSuccessfully)
          return completed.Result;

        remaining.Remove(completed);
      }

      throw new AggregateException("All tasks failed.");
    }
  }
}
