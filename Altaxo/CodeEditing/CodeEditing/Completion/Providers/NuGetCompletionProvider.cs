// Copyright Eli Arbel (no explicit copyright notice in original file), Apache License Version 2.0, January 2004

// Originated from: RoslynPad, RoslynPad.Common.UI, ViewModels/NuGetViewModel.cs

using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using PackageSourceProvider = NuGet.Configuration.PackageSourceProvider;

namespace Altaxo.CodeEditing.Completion.Providers
{
  [Export, Export(typeof(INuGetCompletionProvider)), Shared]
  internal class NuGetCompletionProvider : INuGetCompletionProvider
  {
    private const int MaxSearchResults = 50;

    private readonly CommandLineSourceRepositoryProvider? _sourceRepositoryProvider;
    private readonly Exception _initializationException;

    public NuGetCompletionProvider()
    {
      var settings = new NuGet.Configuration.Settings(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), ".nuget.config");
      var packageSource = new PackageSourceProvider(settings);
      _sourceRepositoryProvider = new CommandLineSourceRepositoryProvider(packageSource);
    }

    async Task<IReadOnlyList<INuGetPackage>> INuGetCompletionProvider.SearchPackagesAsync(string searchString, bool exactMatch, CancellationToken cancellationToken)
    {
      var packages = await GetPackagesAsync(searchString, includePrerelease: true, exactMatch, cancellationToken).ConfigureAwait(false);
      return packages;
    }

    public async Task<IReadOnlyList<PackageData>> GetPackagesAsync(string searchTerm, bool includePrerelease, bool exactMatch, CancellationToken cancellationToken)
    {
      if (_initializationException is not null)
        throw _initializationException;

      if (_sourceRepositoryProvider is null)
      {
        return [];
      }

      var filter = new SearchFilter(includePrerelease);
      var packages = new List<PackageData>();

      foreach (var sourceRepository in _sourceRepositoryProvider.GetRepositories())
      {
        IPackageSearchMetadata[]? result;
        try
        {
          result = await SearchAsync(sourceRepository, searchTerm, filter, MaxSearchResults, cancellationToken).ConfigureAwait(false);
        }
        catch (FatalProtocolException)
        {
          continue;
        }

        if (exactMatch)
        {
          var match = result.FirstOrDefault(c => string.Equals(c.Identity.Id, searchTerm,
              StringComparison.OrdinalIgnoreCase));
          result = match != null ? [match] : null;
        }

        if (result?.Length > 0)
        {
          var repositoryPackages = result
                                   .Select(x => new PackageData(x))
                                   .ToArray();
          await Task.WhenAll(repositoryPackages.Select(x => x.Initialize())).ConfigureAwait(false);
          packages.AddRange(repositoryPackages);
        }
      }

      return packages;
    }


    public static async Task<IPackageSearchMetadata[]> SearchAsync(SourceRepository sourceRepository, string searchText, SearchFilter searchFilter, int pageSize, CancellationToken cancellationToken)
    {
      var searchResource = await sourceRepository.GetResourceAsync<PackageSearchResource>(cancellationToken).ConfigureAwait(false);

      if (searchResource != null)
      {
        var searchResults = await searchResource.SearchAsync(
            searchText,
            searchFilter,
            0,
            pageSize,
            NullLogger.Instance,
            cancellationToken).ConfigureAwait(false);

        if (searchResults != null)
        {
          return searchResults.ToArray();
        }
      }

      return [];
    }
  }
}
