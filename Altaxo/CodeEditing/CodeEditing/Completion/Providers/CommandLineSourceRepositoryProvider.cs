// Copyright Eli Arbel (no explicit copyright notice in original file), Apache License Version 2.0, January 2004

// Originated from: RoslynPad, RoslynPad.Common.UI, ViewModels/NuGetViewModel.cs

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using PackageSource = NuGet.Configuration.PackageSource;
using PackageSourceProvider = NuGet.Configuration.PackageSourceProvider;

namespace Altaxo.CodeEditing.Completion.Providers
{
  internal class CommandLineSourceRepositoryProvider : ISourceRepositoryProvider
  {
    private readonly List<Lazy<INuGetResourceProvider>> _resourceProviders;
    private readonly List<SourceRepository> _repositories;

    // There should only be one instance of the source repository for each package source.
    private static readonly ConcurrentDictionary<PackageSource, SourceRepository> s_cachedSources
        = new();

    public CommandLineSourceRepositoryProvider(IPackageSourceProvider packageSourceProvider)
    {
      PackageSourceProvider = packageSourceProvider;

      _resourceProviders = [.. Repository.Provider.GetCoreV3()];

      // Create repositories
      _repositories = PackageSourceProvider.LoadPackageSources()
          .Where(s => s.IsEnabled)
          .Select(CreateRepository)
          .ToList();
    }

    public IEnumerable<SourceRepository> GetRepositories()
    {
      return _repositories;
    }

    public SourceRepository CreateRepository(PackageSource source)
    {
      return s_cachedSources.GetOrAdd(source, new SourceRepository(source, _resourceProviders));
    }

    public SourceRepository CreateRepository(PackageSource source, FeedType type)
    {
      return s_cachedSources.GetOrAdd(source, new SourceRepository(source, _resourceProviders, type));
    }

    public IPackageSourceProvider PackageSourceProvider { get; }
  }
}
