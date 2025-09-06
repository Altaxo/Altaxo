// Copyright Eli Arbel (no explicit copyright notice in original file), Apache License Version 2.0, January 2004

// partially originated from: RoslynPad, RoslynPad.Roslyn, Completion/Providers/ReferenceDirectiveHelper.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Altaxo.CodeEditing.ReferenceHandling;


internal static class ReferenceDirectiveHelper
{
  private const string FxPrefix = "framework:";

  public const string NuGetPrefix = "nuget:";

  private static readonly char[] s_nugetSeparators = ['/', ','];

  /// <summary>
  /// Parses the #r references in a syntax tree and returns them as a list of <see cref="LibraryRef"/>.
  /// </summary>
  /// <param name="syntaxRoot">The syntax root.</param>
  /// <returns>A list of <see cref="LibraryRef"/>, representing either nuget references of references to DLLs in the file system.</returns>
  public static List<LibraryRef> ParseReferences(SyntaxNode syntaxRoot)
  {
    var libraries = new List<LibraryRef>();

    if (syntaxRoot is not CompilationUnitSyntax compilation)
    {
      return libraries;
    }

    foreach (var directive in compilation.GetReferenceDirectives())
    {
      var value = directive.File.ValueText;

      string? id, version;

      if (HasPrefix(ReferenceDirectiveHelper.NuGetPrefix, value)) // its a nuget reference
      {
        (id, version) = ReferenceDirectiveHelper.ParseNuGetReference(value);
        libraries.Add(LibraryRef.PackageReference(id, version ?? string.Empty));
      }
      else if (value.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase) ||
              value.EndsWith(".exe", StringComparison.InvariantCultureIgnoreCase)) // its a direct reference to a dll or exe
      {
        libraries.Add(LibraryRef.Reference(value));
      }
      else if (HasPrefix(FxPrefix, value))
      {
        libraries.Add(LibraryRef.FrameworkReference(value.Substring(FxPrefix.Length, value.Length - FxPrefix.Length)));
      }
      else
      {
        libraries.Add(LibraryRef.FrameworkReference(value));
      }
    }

    return libraries;
  }

  private static bool HasPrefix(string prefix, string value) => value.Length > prefix.Length && value.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase);


  /// <summary>
  /// Parses a NuGet package reference string into its package ID and version components.
  /// </summary>
  /// <remarks>The input string must conform to the expected format, which includes a prefix followed by the
  /// package ID, and optionally a version component separated by a delimiter. If the version component is not provided,
  /// the returned version will be <see langword="null"/>.</remarks>
  /// <param name="value">The NuGet package reference string to parse. The string is expected to start with a specific prefix and may
  /// optionally include a version component separated by a delimiter.</param>
  /// <returns>A tuple containing the package ID and version. The package ID is always non-null and trimmed of whitespace. The
  /// version is trimmed of whitespace and may be <see langword="null"/> if no version component is specified in the
  /// input string.</returns>
  public static (string id, string version) ParseNuGetReference(string value)
  {
    string id;
    string version;

    var indexOfSlash = value.IndexOfAny(s_nugetSeparators);
    if (indexOfSlash >= 0)
    {
      id = value.Substring(NuGetPrefix.Length, indexOfSlash - NuGetPrefix.Length);
      version = indexOfSlash != value.Length - 1 ? value.Substring(indexOfSlash + 1) : string.Empty;
    }
    else
    {
      id = value.Substring(NuGetPrefix.Length);
      version = null;
    }

    return (id.Trim(), version?.Trim());
  }


  // originated from DirectiveCompletionProviderUtilities
  internal static bool TryGetStringLiteralToken(this SyntaxTree tree, int position, SyntaxKind directiveKind, out SyntaxToken stringLiteral, CancellationToken cancellationToken)
  {
    if (tree.IsEntirelyWithinStringLiteral(position, cancellationToken))
    {
      var token = tree.GetRoot(cancellationToken).FindToken(position, findInsideTrivia: true);
      if (token.IsKind(SyntaxKind.EndOfDirectiveToken) || token.IsKind(SyntaxKind.EndOfFileToken))
      {
        token = token.GetPreviousToken(includeSkipped: true, includeDirectives: true);
      }

      if (token.IsKind(SyntaxKind.StringLiteralToken) && token.Parent?.IsKind(directiveKind) is true)
      {
        stringLiteral = token;
        return true;
      }
    }

    stringLiteral = default;
    return false;
  }

  /// <summary>
  /// Gets the metadata references asynchronous.
  /// </summary>
  /// <param name="libraryRefs">The <see cref="LibraryRef"/> instances that represent the libraries referenced by #r statements.</param>
  /// <param name="roslynHost">The roslyn host.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The <see cref="MetadataReference"/>s which could be resolved.</returns>
  public static async Task<List<MetadataReference>> GetMetadataReferencesAsync(IEnumerable<LibraryRef> libraryRefs, RoslynHost roslynHost, CancellationToken cancellationToken)
  {
    // create metadata references from the library references
    List<MetadataReference> metadataReferences = new List<MetadataReference>();

    foreach (var lib in libraryRefs)
    {
      if (lib.Kind == LibraryRef.RefKind.Reference)
      {
        if (System.IO.File.Exists(lib.Value))
        {
          metadataReferences.Add(roslynHost.CreateMetadataReference(lib.Value));
        }
      }
      else if (lib.Kind == LibraryRef.RefKind.FrameworkReference)
      {
#if NETFRAMEWORK
        // Try to resolve this from the global assembly cache
        GlobalAssemblyCache.Instance.ResolvePartialName(lib.Value, out string location);
        if (location is not null)
        {
          metadataReferences.Add(roslynHost.CreateMetadataReference(location));
        }
#else
        // in .NET, the global assembly cache is not supported, thus we threat it as a package reference
        await AddPackageReferencesAsync(lib, preferNugetFirst: false, metadataReferences, roslynHost, cancellationToken).ConfigureAwait(false);
#endif
      }
      else if (lib.Kind == LibraryRef.RefKind.PackageReference)
      {
        await AddPackageReferencesAsync(lib, preferNugetFirst: true, metadataReferences, roslynHost, cancellationToken).ConfigureAwait(false);
      }
    }
    return metadataReferences;
  }

  /// <summary>
  /// Adds package references to the specified metadata references collection, resolving them either from NuGet or from
  /// local assemblies based on the provided preferences.
  /// </summary>
  /// <remarks>If <paramref name="preferNugetFirst"/> is <see langword="true"/> and the package is successfully
  /// resolved from NuGet, the method returns early without checking local assemblies. If <paramref
  /// name="preferNugetFirst"/> is <see langword="false"/> or no NuGet references are found, the method attempts to
  /// resolve the package from local assemblies.</remarks>
  /// <param name="lib">The library reference containing the package name and version to resolve.</param>
  /// <param name="preferNugetFirst">A boolean value indicating whether to prioritize resolving the package from NuGet before checking for local
  /// assemblies. If <see langword="true"/>, NuGet will be checked first; otherwise, local assemblies will be
  /// prioritized.</param>
  /// <param name="metadataReferences">The collection of metadata references to which the resolved references will be added.</param>
  /// <param name="roslynHost">The Roslyn host used to resolve references and manage the compilation environment.</param>
  /// <param name="cancellationToken">A token to monitor for cancellation requests during the resolution process.</param>
  /// <returns>A task that is awaitable</returns>
  private static async Task AddPackageReferencesAsync(LibraryRef lib, bool preferNugetFirst, List<MetadataReference> metadataReferences, RoslynHost roslynHost, CancellationToken cancellationToken)
  {
    // even here: if we find out that the reference is already in the same directory as the application, we rather load this assembly than to
    // get it from nuget
    // this is because it is then highly probable that the version between the lib linked to Altaxo will crash with the version downloaded from nuget

    if (preferNugetFirst)
    {
      var nugetRefs = await roslynHost.NuGetReferenceResolver.ResolveReferencesAsync(lib.Value, lib.Version, cancellationToken).ConfigureAwait(false);
      metadataReferences.AddRange(nugetRefs);
      if (nugetRefs.Count != 0)
        return;
    }

    var r = TryGetPresentAssemblyReference(lib.Value, roslynHost);
    if (r is not null)
    {
      metadataReferences.AddRange([r]);
    }
    else if (!preferNugetFirst)
    {
      var nugetRefs = await roslynHost.NuGetReferenceResolver.ResolveReferencesAsync(lib.Value, lib.Version, cancellationToken).ConfigureAwait(false);
      metadataReferences.AddRange(nugetRefs);
    }
  }

  /// <summary>
  /// Tries to get a assembly that is already present in the app directory.
  /// </summary>
  /// <param name="name">The assembly name.</param>
  /// <param name="roslynHost">The roslyn host.</param>
  /// <returns>If the assembly was found in the app directory, the corresponding metadata reference is returned. Otherwise, the return value is null.</returns>
  private static MetadataReference? TryGetPresentAssemblyReference(string name, RoslynHost roslynHost)
  {
    var displayName = name;

    if (!name.ToUpperInvariant().EndsWith(".DLL"))
      name = name + ".dll";

    var path1 = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
    var path2 = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

    // first, we search in the top directories of entry assembly and executing assembly

    var file = System.IO.Directory.EnumerateFiles(path1, name, System.IO.SearchOption.TopDirectoryOnly).FirstOrDefault();

    if (file is null && path2 != path1)
    {
      file = System.IO.Directory.EnumerateFiles(path2, name, System.IO.SearchOption.TopDirectoryOnly).FirstOrDefault();
    }

    // if the search was not successful, we extend the search to the subdirectories
    if (file is null)
    {
      System.IO.Directory.EnumerateFiles(path1, name, System.IO.SearchOption.AllDirectories).FirstOrDefault();
    }

    if (file is null && path2 != path1)
    {
      file = System.IO.Directory.EnumerateFiles(path2, name, System.IO.SearchOption.AllDirectories).FirstOrDefault();
    }

    if (file is null)
    {


#if NETFRAMEWORK
      var assIdentity = GlobalAssemblyCache.Instance.ResolvePartialName(displayName);
      file = assIdentity is not null ? assIdentity.GetDisplayName() : null;
#else
      file = DotNetCoreSdkAssemblyResolver.Instance.Resolve(displayName);
#endif
    }


    return file is not null ? roslynHost.CreateMetadataReference(file) : null;
  }

  /// <summary>
  /// Gets the metadata references from a bunch of syntax trees. The syntax trees are scanned for #r directives, which are then resolved to <see cref="MetadataReference"/> instances.
  /// </summary>
  /// <param name="trees">The trees.</param>
  /// <param name="roslynHost">The Roslyn host.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The resolved metadata references that are referenced by #r statements in the syntax trees.</returns>
  public static async Task<List<MetadataReference>> GetMetadataReferencesAsync(IEnumerable<SyntaxTree> trees, RoslynHost roslynHost, CancellationToken cancellationToken)
  {
    var libraryRefs = new HashSet<LibraryRef>();
    foreach (var tree in trees)
    {
      foreach (var libRef in ParseReferences(await tree.GetRootAsync(cancellationToken).ConfigureAwait(false)))
      {
        libraryRefs.Add(libRef);
      }
    }

    // now resolve the libRef's
    return await GetMetadataReferencesAsync(libraryRefs, roslynHost, cancellationToken).ConfigureAwait(false);
  }

  /// <summary>
  /// Gets the metadata reference differences.
  /// </summary>
  /// <param name="newReferences">The new references.</param>
  /// <param name="oldReferences">The old references.</param>
  /// <returns>A tuple that contains the references to remove from the project, and the references that needs to be added to the project.</returns>
  public static (List<MetadataReference> toRemove, List<MetadataReference> toAdd) GetMetadataReferenceDifference(List<MetadataReference> newReferences, List<MetadataReference> oldReferences)
  {
    List<MetadataReference> toRemove = new List<MetadataReference>();
    List<MetadataReference> toAdd = new List<MetadataReference>();
    foreach (var oldRef in oldReferences)
    {
      bool found = false;
      foreach (var newRef in newReferences)
      {
        if (oldRef.Display == newRef.Display)
        {
          found = true;
          break;
        }
      }
      if (!found)
        toRemove.Add(oldRef);
    }
    foreach (var newRef in newReferences)
    {
      bool found = false;
      foreach (var oldRef in oldReferences)
      {
        if (oldRef.Display == newRef.Display)
        {
          found = true;
          break;
        }
      }
      if (!found)
        toAdd.Add(newRef);
    }
    return (toRemove, toAdd);
  }

}

