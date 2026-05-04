using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Altaxo.Serialization.AutoUpdates
{
  /// <summary>
  /// Retrieves public release information for a GitHub repository without using authenticated access.
  /// </summary>
  public sealed class VersionDataDownloader_GitHub
  {
    const string _owner = "Altaxo";
    const string _repo = "Altaxo";

    private readonly bool _loadUnstableVersion;

    /// <summary>
    /// Initializes a new instance of the <see cref="VersionDataDownloader_GitHub"/> class.
    /// </summary>
    public VersionDataDownloader_GitHub(bool loadUnstableVersion)
    {
      _loadUnstableVersion = loadUnstableVersion;
    }



    /// <summary>
    /// Gets the latest stable release and the latest unstable release together with their downloadable assets.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The latest stable and unstable release information.</returns>
    public async Task<GithubReleaseLookupResult> GetLatestStableAndUnstableReleasesAsync(CancellationToken cancellationToken = default)
    {
      var releases = await GetReleasesAsync(cancellationToken).ConfigureAwait(false);

      var latestStableRelease = releases
          .Where(x => !x.Draft && !x.Prerelease)
          .OrderByDescending(GetReleaseSortDate)
          .FirstOrDefault();

      var latestUnstableRelease = releases
          .Where(x => !x.Draft && x.Prerelease)
          .OrderByDescending(GetReleaseSortDate)
          .FirstOrDefault();

      return new GithubReleaseLookupResult(
          ToReleaseInfo(latestStableRelease),
          ToReleaseInfo(latestUnstableRelease));
    }

    /// <summary>
    /// Asynchronously retrieves the latest version information, including the version data, associated packages, and
    /// release details from GitHub.  
    /// </summary>
    /// <remarks>If unstable versions are enabled and a newer unstable release is available, the method will
    /// return information from the unstable release instead of the stable one.</remarks>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A tuple containing the raw version data as a byte array, an array of parsed package information, and the details
    /// of the latest GitHub release.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the latest release does not contain a 'versioninfo.json' asset.</exception>
    public async Task<PackagesWithVersionDataAndBaseUrl?> GetLatestVersionAsync(CancellationToken cancellationToken = default)
    {
      var releases = await GetLatestStableAndUnstableReleasesAsync(cancellationToken);

      var latestRelease = releases.StableRelease;

      if (_loadUnstableVersion && (latestRelease is null || releases.UnstableRelease is not null && releases.UnstableRelease.PublishedAt > releases.StableRelease?.PublishedAt))
      {
        latestRelease = releases.UnstableRelease;
      }

      var url = latestRelease?.Assets.FirstOrDefault(i => 0 == string.Compare(i.FileName, "versioninfo.json", ignoreCase: true))?.DownloadUrl;

      if (string.IsNullOrEmpty(url))
      {
        throw new InvalidDataException("GithubDownloader : No VersionInfo.json asset found in the latest release.");
      }

      // now download the version info file
      using var httpClient = new System.Net.Http.HttpClient();

      Console.WriteLine("Start downloading version file from GitHub ...");
      var versionData = await httpClient.GetByteArrayAsync(url, cancellationToken);
      Console.WriteLine($"OK, {versionData.Length} bytes downloaded from GitHub.");
      // we leave the file open, thus no other process can access it
      var parsedVersions = PackageInfo.ReadPackagesFromJson(new MemoryStream(versionData));
      return new PackagesWithVersionDataAndBaseUrl(versionData, parsedVersions, url.Substring(0, url.Length - "VersionInfo.json".Length));
    }

    /// <summary>
    /// Gets all releases from the GitHub public releases endpoint.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The deserialized release list.</returns>
    private async Task<IReadOnlyList<GithubReleaseApiModel>> GetReleasesAsync(CancellationToken cancellationToken)
    {
      var requestUri = $"https://api.github.com/repos/{_owner}/{_repo}/releases";

      try
      {
        using var httpClient = CreateHttpClient();
        using var response = await httpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        var releases = await JsonSerializer.DeserializeAsync(responseStream, GithubApiJsonContext.Default.ListGithubReleaseApiModel, cancellationToken).ConfigureAwait(false);
        return releases ?? [];
      }
      catch (Exception ex)
      {
        Console.WriteLine($"GithubDownloader : Failed to retrieve releases from GitHub. Exception: {ex.Message}");
        return [];
      }
    }

    /// <summary>
    /// Converts a GitHub API release model into the downloader result model.
    /// </summary>
    /// <param name="release">The API release model.</param>
    /// <returns>The projected release information, or <see langword="null"/> if no release was provided.</returns>
    private static GithubReleaseInfo? ToReleaseInfo(GithubReleaseApiModel? release)
    {
      if (release is null)
      {
        return null;
      }

      var assets = release.Assets
          .Select(x => new GithubReleaseAssetInfo(x.Name, x.BrowserDownloadUrl))
          .ToArray();

      return new GithubReleaseInfo(
          release.Name,
          release.TagName,
          release.Prerelease,
          release.PublishedAt ?? release.CreatedAt,
          assets);
    }

    /// <summary>
    /// Gets the effective date used to determine the latest release.
    /// </summary>
    /// <param name="release">The release model.</param>
    /// <returns>The publication date, or the creation date if the publication date is missing.</returns>
    private static DateTimeOffset GetReleaseSortDate(GithubReleaseApiModel release)
    {
      return release.PublishedAt ?? release.CreatedAt;
    }

    /// <summary>
    /// Creates an HTTP client configured for GitHub API access.
    /// </summary>
    /// <returns>A configured HTTP client instance.</returns>
    private static HttpClient CreateHttpClient()
    {
      var httpClient = new HttpClient();
      httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("GithubUploader", "1.0"));
      httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
      return httpClient;
    }

    /// <summary>
    /// Represents the latest stable and unstable releases discovered in a repository.
    /// </summary>
    /// <param name="StableRelease">The latest stable release.</param>
    /// <param name="UnstableRelease">The latest unstable release.</param>
    public sealed record GithubReleaseLookupResult(GithubReleaseInfo? StableRelease, GithubReleaseInfo? UnstableRelease);

    /// <summary>
    /// Represents a single GitHub release together with its downloadable assets.
    /// </summary>
    /// <param name="Name">The display name of the release.</param>
    /// <param name="TagName">The Git tag of the release.</param>
    /// <param name="IsPrerelease">A value indicating whether the release is a prerelease.</param>
    /// <param name="PublishedAt">The publication date of the release.</param>
    /// <param name="Assets">The downloadable assets of the release.</param>
    public sealed record GithubReleaseInfo(string Name, string TagName, bool IsPrerelease, DateTimeOffset PublishedAt, IReadOnlyList<GithubReleaseAssetInfo> Assets);

    /// <summary>
    /// Represents a downloadable asset of a GitHub release.
    /// </summary>
    /// <param name="FileName">The file name of the asset.</param>
    /// <param name="DownloadUrl">The public browser download URL of the asset.</param>
    public sealed record GithubReleaseAssetInfo(string FileName, string DownloadUrl);

    internal sealed class GithubReleaseApiModel
    {
      [JsonPropertyName("name")]
      public string Name { get; set; } = string.Empty;

      [JsonPropertyName("tag_name")]
      public string TagName { get; set; } = string.Empty;

      [JsonPropertyName("draft")]
      public bool Draft { get; set; }

      [JsonPropertyName("prerelease")]
      public bool Prerelease { get; set; }

      [JsonPropertyName("created_at")]
      public DateTimeOffset CreatedAt { get; set; }

      [JsonPropertyName("published_at")]
      public DateTimeOffset? PublishedAt { get; set; }

      [JsonPropertyName("assets")]
      public List<GithubReleaseAssetApiModel> Assets { get; set; } = [];
    }

    internal sealed class GithubReleaseAssetApiModel
    {
      [JsonPropertyName("name")]
      public string Name { get; set; } = string.Empty;

      [JsonPropertyName("browser_download_url")]
      public string BrowserDownloadUrl { get; set; } = string.Empty;
    }
  }

  [JsonSerializable(typeof(List<VersionDataDownloader_GitHub.GithubReleaseApiModel>))]
  internal sealed partial class GithubApiJsonContext : JsonSerializerContext
  {
  }
}
