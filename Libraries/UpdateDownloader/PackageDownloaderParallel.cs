using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Altaxo.Serialization.AutoUpdates
{
  /// <summary>
  /// Downloads packages in parallel from multiple candidate sources and keeps the first valid result.
  /// </summary>
  public class PackageDownloaderParallel
  {
    private string _storagePath;
    private readonly object _consoleSynchronizationObject = new object();

    /// <summary>
    /// Initializes a new instance of the <see cref="PackageDownloaderParallel"/> class.
    /// </summary>
    /// <param name="storagePath">The directory where the downloaded package and version file are stored.</param>
    public PackageDownloaderParallel(string storagePath)
    {
      _storagePath = storagePath;
    }

    /// <summary>
    /// Downloads the specified packages in parallel and keeps the first successfully verified package.
    /// </summary>
    /// <param name="packagesToDownload">The package candidates to download.</param>
    /// <returns>A task that represents the asynchronous download workflow.</returns>
    public async Task DownloadPackageFiles(PackageWithVersionDataAndBaseUrl[] packagesToDownload)
    {
      try
      {
        var downloadTasks = new (Task task, string tempFileName, byte[] versionData, PackageInfo package, string baseUrl, int consoleLine)[packagesToDownload.Length];

        var cts = new CancellationTokenSource();

        for (int i = 0; i < packagesToDownload.Length; i++)
        {
          var (versionData, package, baseUrl) = packagesToDownload[i];
          var tempFileName = Path.GetTempFileName();
          var consoleLine = ReserveConsoleLine();
          WriteDownloadProgress(consoleLine, package.FileNameOfPackageZipFile, 0, package.FileLength, "Queued");
          downloadTasks[i] = (DownloadPackageAsync(baseUrl + package.FileNameOfPackageZipFile, tempFileName, package.FileNameOfPackageZipFile, consoleLine, cts.Token), tempFileName, versionData, package, baseUrl, consoleLine);
        }

        // Wait for any of the download tasks to complete sucessfully, and then cancel the others
        while (downloadTasks.Length > 0)
        {
          var completedTask = await Task.WhenAny(downloadTasks.Select(t => t.task));
          var completedIndex = Array.FindIndex(downloadTasks, t => t.task == completedTask);
          if (completedTask.IsCompletedSuccessfully && downloadTasks[completedIndex].package.IsLengthAndHashOfPackageZipFileCorrect(new FileInfo(downloadTasks[completedIndex].tempFileName)))
          {
            // Move the successfully downloaded file to the final location
            var (task, tempFileName, versionData, package, baseUrl, consoleLine) = downloadTasks[completedIndex];
            var finalFileName = Path.Combine(_storagePath, package.FileNameOfPackageZipFile);
            var versionFileFullName = Path.Combine(_storagePath, PackageInfo.VersionFileName);
            using (var versionFileStream = new FileStream(versionFileFullName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            {
              versionFileStream.Seek(0, SeekOrigin.Begin);
              versionFileStream.Write(versionData, 0, versionData.Length);
              versionFileStream.SetLength(versionData.Length); // cut the stream at this length in case the existing file before was longer
              versionFileStream.Flush(); // write the new version to disc in order to change the write date
              File.Move(tempFileName, finalFileName);
            }

            WriteDownloadProgress(consoleLine, package.FileNameOfPackageZipFile, package.FileLength, package.FileLength, "Completed");

            // Cancel the other download tasks
            cts.Cancel();
            break;
          }
          else
          {
            // Remove the failed task from the list and delete the temporary file
            var (task, tempFileName, versionData, package, baseUrl, consoleLine) = downloadTasks[completedIndex];
            try
            {
              File.Delete(tempFileName);
            }
            catch
            {
            }

            if (completedTask.IsCanceled)
            {
              WriteDownloadProgress(consoleLine, package.FileNameOfPackageZipFile, 0, package.FileLength, "Cancelled");
            }
            else if (completedTask.IsFaulted)
            {
              WriteDownloadProgress(consoleLine, package.FileNameOfPackageZipFile, 0, package.FileLength, "Failed");
            }

            downloadTasks = downloadTasks.Where((t, index) => index != completedIndex).ToArray();
          }
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"An error occurred during the download process: {ex}");
      }
    }

    private async Task DownloadPackageAsync(string urlToDownload, string destinationFileName, string displayName, int consoleLine, CancellationToken cancellationToken)
    {
      try
      {
        using var httpClient = new System.Net.Http.HttpClient();

        WriteDownloadProgress(consoleLine, displayName, 0, 0, "Connecting");
        using var response = await httpClient.GetAsync(urlToDownload, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        response.EnsureSuccessStatusCode();

        var totalBytes = response.Content.Headers.ContentLength ?? 0;
        await using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        await using var fileStream = new FileStream(destinationFileName, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);

        var buffer = new byte[81920];
        long totalBytesRead = 0;
        int bytesRead;

        WriteDownloadProgress(consoleLine, displayName, 0, totalBytes, "Downloading");

        while ((bytesRead = await contentStream.ReadAsync(buffer, cancellationToken)) > 0)
        {
          await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
          totalBytesRead += bytesRead;
          WriteDownloadProgress(consoleLine, displayName, totalBytesRead, totalBytes, "Downloading");
        }
      }
      catch (TaskCanceledException)
      {
        WriteDownloadProgress(consoleLine, displayName, 0, 0, "Cancelled");
        try
        {
          File.Delete(destinationFileName);
        }
        catch
        {
        }
        throw;
      }
      catch (Exception ex)
      {
        WriteDownloadProgress(consoleLine, displayName, 0, 0, "Failed");
        lock (_consoleSynchronizationObject)
        {
          Console.WriteLine($"An error occurred while downloading {urlToDownload}: {ex}");
        }
        throw;
      }
    }

    private int ReserveConsoleLine()
    {
      lock (_consoleSynchronizationObject)
      {
        var line = Console.CursorTop;
        Console.WriteLine();
        return line;
      }
    }

    private void WriteDownloadProgress(int consoleLine, string displayName, long bytesReceived, long totalBytes, string status)
    {
      const int barWidth = 30;
      string shortenedDisplayName = displayName.Length <= 32 ? displayName : displayName.Substring(0, 29) + "...";
      double progressRatio = totalBytes > 0 ? Math.Clamp((double)bytesReceived / totalBytes, 0, 1) : 0;
      int filledWidth = (int)Math.Round(progressRatio * barWidth, MidpointRounding.AwayFromZero);
      string progressBar = new string('#', filledWidth) + new string('-', barWidth - filledWidth);
      string percentText = totalBytes > 0 ? $"{progressRatio * 100,6:0.0}%" : "   n/a";
      string sizeText = totalBytes > 0 ? $"{bytesReceived / 1024d / 1024d:0.00}/{totalBytes / 1024d / 1024d:0.00} MB" : $"{bytesReceived / 1024d / 1024d:0.00} MB";
      string text = $"{shortenedDisplayName,-32} [{progressBar}] {percentText} {sizeText,-18} {status}";

      lock (_consoleSynchronizationObject)
      {
        int currentLeft = Console.CursorLeft;
        int currentTop = Console.CursorTop;
        Console.SetCursorPosition(0, consoleLine);
        Console.Write(text.PadRight(Math.Max(Console.BufferWidth - 1, text.Length)));
        Console.SetCursorPosition(currentLeft, currentTop);
      }
    }

  }
}
