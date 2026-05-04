namespace Altaxo.Serialization.AutoUpdates;


using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// Adaptive parallel downloader of the same package file from different servers. The servers that seem slower are throtteled in order to save bandwidth.
/// </summary>
public class PackageDownloaderAdaptiveParallel
{

  private string _storagePath;
  private PackageWithVersionDataAndBaseUrl[] _packages;

  private string _tempOutputFileName;
  private string _fileNameOfPackageZipFile;
  private byte[] _versionData;
  /// <summary>
  /// Initializes a new instance of the <see cref="PackageDownloaderAdaptiveParallel"/> class.
  /// </summary>
  /// <param name="storagePath">The directory where the downloaded package and version file are stored.</param>
  /// <param name="packagesToDownload">The package entries to download from different mirrors.</param>
  public PackageDownloaderAdaptiveParallel(string storagePath, PackageWithVersionDataAndBaseUrl[] packagesToDownload)
  {
    _storagePath = storagePath;
    _packages = packagesToDownload; // assuming all entries refer to the same package version, just different mirrors
  }



  /// <summary>
  /// Downloads the package file by using adaptive parallel downloads across multiple mirrors.
  /// </summary>
  /// <returns>A task that represents the asynchronous download operation.</returns>
  public async Task DownloadPackageFiles()
  {
    var serverUrls = _packages.Select(p => p.baseUrl + p.package.FileNameOfPackageZipFile).ToArray();
    var _tempOutputFileName = Path.GetTempFileName();
    var fileSize = _packages[0].package.FileLength;
    _fileNameOfPackageZipFile = _packages[0].package.FileNameOfPackageZipFile;
    _versionData = _packages[0].versionData;


    var parallelDownloader = new AdaptiveParallelDownloaderOfSameFile(serverUrls, _tempOutputFileName);

    Console.WriteLine($"Starting adaptive parallel download of {_fileNameOfPackageZipFile} from:");
    foreach (var url in serverUrls)
    {
      Console.WriteLine(url);
    }

    try
    {
      await parallelDownloader.DownloadAsync(fileSize);
    }
    catch (Exception ex)
    {
      Console.WriteLine($"✗ Download failed: {ex.Message}");
      if (File.Exists(_tempOutputFileName))
      {
        File.Delete(_tempOutputFileName);
      }
      throw;
    }

    var finalFileName = Path.Combine(_storagePath, _fileNameOfPackageZipFile);
    var versionFileFullName = Path.Combine(_storagePath, PackageInfo.VersionFileName);
    using (var versionFileStream = new FileStream(versionFileFullName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
    {
      versionFileStream.Seek(0, SeekOrigin.Begin);
      versionFileStream.Write(_versionData, 0, _versionData.Length);
      versionFileStream.SetLength(_versionData.Length); // cut the stream at this length in case the existing file before was longer
      versionFileStream.Flush(); // write the new version to disc in order to change the write date
      File.Move(_tempOutputFileName, finalFileName);
      Console.WriteLine($"\n✓ Finally saved to {finalFileName}");
    }
    var fileRemovedWhenFailed = _packages[0].package.RemovePackageZipFileIfLengthOrHashDiffer(_storagePath);

    if (string.IsNullOrEmpty(fileRemovedWhenFailed))
    {
      Console.WriteLine($"✓ Verified integrity of downloaded file {finalFileName}");
    }
    else
    {
      Console.WriteLine($"✗ Integrity check failed for {finalFileName}, removed file {fileRemovedWhenFailed}");
    }
  }

}
