#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using static NuGet.Frameworks.FrameworkConstants;

/// <summary>
/// Provides functionality to resolve NuGet package references and their dependencies into metadata references for use
/// in compilation or analysis scenarios.
/// </summary>
/// <remarks>This class is designed to resolve NuGet packages from a specified source repository, download any
/// missing packages to the global cache, and extract library assemblies targeting specific frameworks. By default, it
/// supports .NET 9.0, .NET 8.0, and .NET Standard 2.0 frameworks. The global packages folder is determined using the
/// default NuGet settings.</remarks>
public class NuGetReferenceResolver
{
  private readonly SourceRepository _repository;
  private readonly ILogger _logger = NullLogger.Instance;
  private readonly string _globalPackagesFolder;
  private readonly List<NuGetFramework> _targetFrameworks;
  private readonly SourceCacheContext _cache;

  /// <summary>
  /// Initializes a new instance of the <see cref="NuGetReferenceResolver"/> class,  configured to resolve NuGet package
  /// references from the specified source URL.
  /// </summary>
  /// <remarks>This constructor initializes the resolver with a default set of target frameworks,  including
  /// .NET 9.0, .NET 8.0, and .NET Standard 2.0. It also uses the global packages  folder configured in the default
  /// NuGet settings.</remarks>
  /// <param name="sourceUrl">The URL of the NuGet package source to use for resolving package references.  Defaults to
  /// "https://api.nuget.org/v3/index.json" if not specified.</param>
  public NuGetReferenceResolver(string sourceUrl = "https://api.nuget.org/v3/index.json", NuGetFramework currentFramework = null)
  {
    _repository = Repository.Factory.GetCoreV3(sourceUrl);
    _globalPackagesFolder = SettingsUtility.GetGlobalPackagesFolder(Settings.LoadDefaultSettings(null));
    _cache = new SourceCacheContext();
    _targetFrameworks = new List<NuGetFramework>
    {
#if NET10_0
      new NuGetFramework(FrameworkIdentifiers.Net, new Version(10, 0, 0, 0)),
#else
#error "Please specify above the framework version that is currently compiled, and then some lesser versions below!"
#endif
      new NuGetFramework(FrameworkIdentifiers.Net, new Version(9, 0, 0, 0)),
      new NuGetFramework(FrameworkIdentifiers.Net, new Version(8, 0, 0, 0)),
      new NuGetFramework(FrameworkConstants.CommonFrameworks.NetStandard20)
    };
  }

  /// <summary>
  /// Resolves and retrieves a list of metadata references for the specified NuGet package and version, including all
  /// its dependencies, targeting the specified frameworks.
  /// </summary>
  /// <remarks>This method resolves the specified NuGet package and its dependencies, downloads any missing
  /// packages  to the global cache, and extracts the library assemblies for the specified target frameworks. Only 
  /// assemblies with a `.dll` extension are included in the result.</remarks>
  /// <param name="packageId">The ID of the NuGet package to resolve.</param>
  /// <param name="version">The version of the NuGet package to resolve. Must be a valid semantic version.</param>
  /// <returns>A task that represents the asynchronous operation. The task result contains a list of  <see
  /// cref="Microsoft.CodeAnalysis.MetadataReference"/> objects representing the resolved assemblies.</returns>
  public async Task<List<MetadataReference>> ResolveReferencesAsync(string packageId, string version, CancellationToken cancellationToken)
  {
    if (string.IsNullOrWhiteSpace(version))
    {
      version = "*"; // Default to any version if none is specified
    }

    PackageIdentity rootIdentity = null;
    if (NuGetVersion.TryParse(version, out var parsedVersion)) // if the version string is a fully specified version (and not a range)
    {
      rootIdentity = new PackageIdentity(packageId, NuGetVersion.Parse(version));
    }
    else if (VersionRange.TryParse(version, out var parsedVersionRange)) // else if the version string is a version range
    {
      rootIdentity = await GetBestMatchAsync(packageId, parsedVersionRange, cancellationToken).ConfigureAwait(false);
    }

    if (rootIdentity is null)
    {
      return new List<MetadataReference>();
    }

    // Get all identities of the dependencies of the root package, including the root package itself
    var allPackages = await GetAllDependenciesAsync(rootIdentity, new HashSet<PackageIdentity>(), cancellationToken).ConfigureAwait(false);

    var references = new List<MetadataReference>();

    // now, for each package, find the lib items for the target framework, and add them as references
    foreach (var pkg in allPackages)
    {
      var installPath = Path.Combine(_globalPackagesFolder, pkg.Id.ToLower(), pkg.Version.ToNormalizedString());

      if (!Directory.Exists(installPath))
      {
        await DownloadToGlobalCacheAsync(pkg, cancellationToken).ConfigureAwait(false);
      }

      var reader = new PackageFolderReader(installPath);
      var libItems = await reader.GetLibItemsAsync(cancellationToken).ConfigureAwait(false);

      FrameworkSpecificGroup group = null;
      foreach (var framework in _targetFrameworks)
      {
        group = libItems.FirstOrDefault(g => NuGetFrameworkFullComparer.Instance.Equals(framework, g.TargetFramework));
        if (group is not null)
        {
          break;
        }
      }

      group ??= libItems.FirstOrDefault(g => DefaultCompatibilityProvider.Instance.IsCompatible(_targetFrameworks[0], g.TargetFramework));

      try
      {
        if (group is null || !group.Items.Any())
        {
          // if no suitable package was found in libItems, we try it with reference items
          var refItems = await reader.GetReferenceItemsAsync(cancellationToken).ConfigureAwait(false);

          foreach (var framework in _targetFrameworks)
          {
            group = refItems.FirstOrDefault(g => NuGetFrameworkFullComparer.Instance.Equals(framework, g.TargetFramework));
            if (group is not null)
            {
              break;
            }
          }

          group ??= refItems.FirstOrDefault(g => DefaultCompatibilityProvider.Instance.IsCompatible(_targetFrameworks[0], g.TargetFramework));
        }
      }
      catch (Exception ex)
      {

      }




      if (group is not null && group.Items.Any())
      {
        foreach (var item in group.Items.Where(p => p.EndsWith(".dll")))
        {
          var pathToDll = Path.Combine(installPath, item);

          if (File.Exists(pathToDll))
          {
            var pathToXmlDocumentation = Path.ChangeExtension(pathToDll, ".xml"); // Look for XML documentation file alongside the DLL
            if (File.Exists(pathToXmlDocumentation)) // If XML documentation file exists, create XmlDocumentationProvider
            {
              var documentationProvider = XmlDocumentationProvider.CreateFromFile(pathToXmlDocumentation);
              references.Add(MetadataReference.CreateFromFile(pathToDll, documentation: documentationProvider)); // Create the MetadataReference with documentation
            }
            else
            {
              references.Add(MetadataReference.CreateFromFile(pathToDll)); // Create the MetadataReference without documentation
            }
          }
        }
        break;
      }
    }

    return references;
  }

  /// <summary>
  /// Downloads the specified package to the .nuget cache.
  /// </summary>
  /// <param name="identity">The identity of the package.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  private async Task DownloadToGlobalCacheAsync(PackageIdentity identity, CancellationToken cancellationToken)
  {
    var downloadResource = await _repository.GetResourceAsync<DownloadResource>().ConfigureAwait(false);
    await downloadResource.GetDownloadResourceResultAsync(
        identity,
        new PackageDownloadContext(_cache),
        _globalPackagesFolder,
        _logger,
        cancellationToken).ConfigureAwait(false);
  }

  /// <summary>
  /// Given a fully specified package, all dependencies of this package are found and returned. The returned set also contains the specified package itself.
  /// </summary>
  /// <param name="package">The specified package package.</param>
  /// <param name="collected">The hash set of collected packaged. Please provide an empty hash set.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The set with all dependencies of the specified package, and the package itself.</returns>
  private async Task<HashSet<PackageIdentity>> GetAllDependenciesAsync(PackageIdentity package, HashSet<PackageIdentity> collected, CancellationToken cancellationToken)
  {
    if (collected.Contains(package))
      return collected;

    collected.Add(package);

    var dependencyResource = await _repository.GetResourceAsync<DependencyInfoResource>().ConfigureAwait(false);
    PackageDependencyInfo packageInfo = null;

    foreach (var framework in _targetFrameworks)
    {
      packageInfo = await dependencyResource.ResolvePackage(package, framework, _cache, _logger, cancellationToken).ConfigureAwait(false);
      if (packageInfo is not null)
        break;
    }

    if (packageInfo is not null)
    {
      foreach (var dep in packageInfo.Dependencies)
      {
        var depIdentity = new PackageIdentity(dep.Id, dep.VersionRange.MinVersion);
        await GetAllDependenciesAsync(depIdentity, collected, cancellationToken).ConfigureAwait(false);
      }
    }

    return collected;
  }

  /// <summary>
  /// Given a version range, it finds the nuget package with the highest version number that is inside the specified range.
  /// </summary>
  /// <param name="packageId">The package identifier, e.g. 'System.Collections.Immutable'</param>
  /// <param name="range">The version range of the package.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The package identity with the highest version number that is inside the specified version range.</returns>
  public async Task<PackageIdentity> GetBestMatchAsync(string packageId, VersionRange range, CancellationToken cancellationToken)
  {
    var resource = await _repository.GetResourceAsync<FindPackageByIdResource>().ConfigureAwait(false);
    var availableVersionsWithPreview = await resource.GetAllVersionsAsync(packageId, _cache, _logger, cancellationToken).ConfigureAwait(false);
    var availableVersionsWithoutPreview = availableVersionsWithPreview.Where(v => !v.IsPrerelease);
    if (GetBestMatch(range, availableVersionsWithoutPreview) is NuGetVersion bestVersion1) // first try the versions without preview versions
      return new PackageIdentity(packageId, bestVersion1);
    if (GetBestMatch(range, availableVersionsWithPreview) is NuGetVersion bestVersion2) // if nothing found, then include the preview versions
      return new PackageIdentity(packageId, bestVersion2);
    else
      return null;
  }

  /// <summary>
  /// From the enumeration of available versions, it picks out the highest version that still is inside the specified range.
  /// </summary>
  /// <param name="range">The version range.</param>
  /// <param name="availableVersions">The available versions.</param>
  /// <returns>The highest version that still is inside the specified range, or null if no such version exists.</returns>
  public static NuGetVersion GetBestMatch(VersionRange range, IEnumerable<NuGetVersion> availableVersions)
  {
    if (range is null || availableVersions is null)
      return null;

    // Filter versions that satisfy the range
    var matchingVersions = availableVersions
        .Where(v => range.Satisfies(v))
        .OrderByDescending(v => v); // Highest version first

    return matchingVersions.FirstOrDefault();
  }

}
