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
        await AddPackageReferences(lib, metadataReferences, roslynHost, cancellationToken).ConfigureAwait(false);
#endif
      }
      else if (lib.Kind == LibraryRef.RefKind.PackageReference)
      {
        await AddPackageReferences(lib, metadataReferences, roslynHost, cancellationToken).ConfigureAwait(false);
      }
    }
    return metadataReferences;
  }

  static async Task AddPackageReferences(LibraryRef lib, List<MetadataReference> metadataReferences, RoslynHost roslynHost, CancellationToken cancellationToken)
  {
    // even here: if we find out that the reference is already in the same directory as the application, we rather load this assembly than to
    // get it from nuget
    // this is because it is then highly probable that the version between the lib linked to Altaxo will crash with the version downloaded from nuget

    var r = TryGetPresentAssemblyReference(lib.Value, roslynHost);
    if (r is not null)
    {
      metadataReferences.AddRange([r]);
    }
    else
    {
      var nugetRefs = await roslynHost.NuGetReferenceResolver.ResolveReferencesAsync(lib.Value, lib.Version, cancellationToken).ConfigureAwait(false);
      metadataReferences.AddRange(nugetRefs);
    }
  }


  static MetadataReference? TryGetPresentAssemblyReference(string name, RoslynHost roslynHost)
  {

    if (!name.ToUpperInvariant().EndsWith(".DLL"))
      name = name + ".dll";

    var path1 = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
    var path2 = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

    // first, we search in the top directories of entry assembly and executing assembly

    var file = System.IO.Directory.EnumerateFiles(path1, name, System.IO.SearchOption.TopDirectoryOnly).FirstOrDefault();

    if(file is null && path2 != path1)
    {
      file = System.IO.Directory.EnumerateFiles(path2, name, System.IO.SearchOption.TopDirectoryOnly).FirstOrDefault();
    }

    // if the search was not successful, we extend the search to the subdirectories
    if(file is null)
    {
      System.IO.Directory.EnumerateFiles(path1, name, System.IO.SearchOption.AllDirectories).FirstOrDefault();
    }

    if (file is null && path2 != path1)
    {
      file = System.IO.Directory.EnumerateFiles(path2, name, System.IO.SearchOption.AllDirectories).FirstOrDefault();
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

