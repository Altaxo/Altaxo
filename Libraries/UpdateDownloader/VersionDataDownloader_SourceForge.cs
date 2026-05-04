using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Altaxo.Serialization.AutoUpdates
{
  /// <summary>
  /// Downloads Altaxo update metadata from SourceForge.
  /// </summary>
  public class VersionDataDownloader_SourceForge
  {
    /// <summary>
    /// The SourceForge URL for the latest stable Altaxo release.
    /// </summary>
    public const string _stableUrl = "http://downloads.sourceforge.net/project/altaxo/Altaxo/Altaxo-Latest-Stable/";

    /// <summary>
    /// The SourceForge URL for the latest unstable Altaxo release.
    /// </summary>
    public const string _unstableUrl = "http://downloads.sourceforge.net/project/altaxo/Altaxo/Altaxo-Latest-Unstable/";

    private readonly bool _loadUnstableVersion;

    /// <summary>
    /// Initializes a new instance of the <see cref="VersionDataDownloader_SourceForge"/> class.
    /// </summary>
    /// <param name="loadUnstable">If set to <c>true</c>, the downloader looks for the latest unstable version. If set to <c>false</c>, it looks for the latest stable version.</param>
    public VersionDataDownloader_SourceForge(bool loadUnstable)
    {
      _loadUnstableVersion = loadUnstable;
    }

    /// <summary>
    /// Gets the latest version information from the remote server.
    /// </summary>
    /// <param name="cancellationToken">The token used to cancel the asynchronous operation.</param>
    /// <returns>The downloaded version data together with the parsed package information, or <see langword="null"/> if no data is available.</returns>
    public async Task<PackagesWithVersionDataAndBaseUrl?> GetLatestVersionAsync(CancellationToken cancellationToken = default)
    {
      Console.WriteLine("Start downloading version file from SourceForge ...");

      var downloadUrl = _loadUnstableVersion ? _unstableUrl : _stableUrl;
      using var httpClient = new System.Net.Http.HttpClient();
      var versionData = await httpClient.GetByteArrayAsync(downloadUrl + PackageInfo.VersionFileName, cancellationToken);
      Console.WriteLine($"OK, {versionData.Length} bytes downloaded from SourceForge.");
      // we leave the file open, thus no other process can access it
      var parsedVersions = PackageInfo.ReadPackagesFromJson(new MemoryStream(versionData));

      return new PackagesWithVersionDataAndBaseUrl(versionData, parsedVersions, downloadUrl);
    }
  }
}
