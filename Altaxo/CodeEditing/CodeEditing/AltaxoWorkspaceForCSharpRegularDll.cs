#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2017 Dr. Dirk Lellinger
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

//extern alias MCW;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Altaxo.CodeEditing.ReferenceHandling;

//using MCW::Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Altaxo.CodeEditing
{
  /// <summary>
  /// Workspace for a regular Dll. Contains exactly one solution, and one C# project.
  /// </summary>
  /// <seealso cref="Microsoft.CodeAnalysis.Workspace" />
  public class AltaxoWorkspaceForCSharpRegularDll : AltaxoWorkspaceBase
  {
    



    public AltaxoWorkspaceForCSharpRegularDll(
      RoslynHost roslynHost,
      string workingDirectory,
      IEnumerable<MetadataReference> staticReferences)
        : base(roslynHost, staticReferences, workingDirectory)
    {
    }

    public AltaxoWorkspaceForCSharpRegularDll(
    RoslynHost roslynHost,
    string workingDirectory,
    IEnumerable<System.Reflection.Assembly> staticReferences)
      : base(roslynHost, staticReferences, workingDirectory)
    {
    }

    protected override ProjectId CreateInitialProject()
    {
      var compilationOptions = CreateCompilationOptions();
      var parseOptions = CreateParseOptions();

      var name = "Prj" + Guid.NewGuid().ToString();
      var projectId = ProjectId.CreateNewId(name);
      var projectInfo = ProjectInfo.Create(
        projectId,
        VersionStamp.Create(),
        name, // project name
        name, // assembly name
        LanguageNames.CSharp, // language
        parseOptions: parseOptions,
        compilationOptions: compilationOptions,
        metadataReferences: StaticReferences
        );

      var newSolution = CurrentSolution.AddProject(projectInfo);
      base.SetCurrentSolution(newSolution);

      return projectId;
    }

    private CSharpCompilationOptions CreateCompilationOptions()
    {
      var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
          usings: null,
          allowUnsafe: true,
          sourceReferenceResolver: new SourceFileResolver([], WorkingDirectory),
            // all #r references are resolved by the editor/msbuild
            metadataReferenceResolver: DummyScriptMetadataResolver.Instance,
            nullableContextOptions: NullableContextOptions.Enable
        );
      return compilationOptions;
    }

    private CSharpParseOptions CreateParseOptions()
    {
      return new CSharpParseOptions(
                  languageVersion: LanguageVersion.Preview,
                  kind: SourceCodeKind.Regular,
                  preprocessorSymbols: PreprocessorSymbols
                  );
    }

    public override Compilation GetCompilation(string assemblyName)
    {
      var project = CurrentSolution.GetProject(ProjectId);

      var parseOptions = CreateParseOptions();
      var trees = project.Documents.Select(document => SyntaxFactory.ParseSyntaxTree(document.GetTextAsync().Result, parseOptions, string.Empty));

      var compilationOptions = new CSharpCompilationOptions(
          OutputKind.DynamicallyLinkedLibrary,
          mainTypeName: null,
          scriptClassName: null,
          usings: ImmutableArray<string>.Empty,
          optimizationLevel: OptimizationLevel.Debug, // TODO
          checkOverflow: false,                       // TODO
          allowUnsafe: true,                          // TODO
          platform: Platform.AnyCpu,
          warningLevel: 4,
          xmlReferenceResolver: null,
          sourceReferenceResolver: null,
          metadataReferenceResolver: null,
          assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default
      );
      //.WithTopLevelBinderFlags(BinderFlags.IgnoreCorLibraryDuplicatedTypes),

      var compilation = CSharpCompilation.Create(
        assemblyName, // Assembly name
        trees,
        AllReferences,
        compilationOptions);

      return compilation;
    }


    /// <summary>
    /// Stores the libaries referenced by each document in this workspace.
    /// </summary>
    private ImmutableDictionary<DocumentId, ImmutableHashSet<LibraryRef>> _documentToSetOfLibRefs = ImmutableDictionary<DocumentId, ImmutableHashSet<LibraryRef>>.Empty;

    /// <summary>
    /// Gets or sets the last used metadata references that are referenced by all the workspace code in #r directives.
    /// </summary>
    private List<MetadataReference> _lastUsedCodeMetadataReferences { get; set; } = [];

   public override async Task UpdateLibrariesAsync(DocumentId documentId, IEnumerable<LibraryRef> libraries, CancellationToken cancellationToken)
    {
      var newLibrarySet = libraries.ToImmutableHashSet();

      var newDictionary = _documentToSetOfLibRefs;

      if (newDictionary.TryGetValue(documentId, out var oldLibrarySet))
      {
        if (!oldLibrarySet.SetEquals(newLibrarySet))
        {
          newDictionary = newDictionary.SetItem(documentId, newLibrarySet);
        }
      }
      else
      {
        newDictionary = newDictionary.Add(documentId, newLibrarySet);
      }

      if (!object.ReferenceEquals(_documentToSetOfLibRefs,newDictionary))
      {
        await OnLibrariesUpdatedAsync(newDictionary, cancellationToken).ConfigureAwait(false);
      }
    }

    /// <summary>
    /// Called when the set of libraries referenced by #r directives has changed. The libraries are resolved and then added to the solution. No longer used references are removed.
    /// </summary>
    /// <param name="libraries">The new set of libraries referenced by #r directives</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task that can be awaited.</returns>
    /// <remarks>
    /// When the new solution (with the changed references) is committed, one also must commit (thread safe!) <paramref name="libraries"/> to <see cref="_documentToSetOfLibRefs"/>,
    /// and the resolved metadataReferences to <see cref="_lastUsedCodeMetadataReferences"/>.
    /// </remarks>
    protected async Task OnLibrariesUpdatedAsync(ImmutableDictionary<DocumentId, ImmutableHashSet<LibraryRef>> libraries, CancellationToken cancellationToken)
    {
      // create metadata references from the library references

      var metadataReferences = new List<MetadataReference>();

      foreach (var libset in libraries.Values)
      {
        foreach (var lib in libset)
        {
          if (lib.Kind == LibraryRef.RefKind.Reference)
          {
            if (System.IO.File.Exists(lib.Value))
            {
              metadataReferences.Add(RoslynHost.CreateMetadataReference(lib.Value));
            }
          }
          else if (lib.Kind == LibraryRef.RefKind.FrameworkReference)
          {
#if NETFRAMEWORK
            // Try to resolve this from the global assembly cache
            GlobalAssemblyCache.Instance.ResolvePartialName(lib.Value, out string location);
            if (location is not null)
            {
              metadataReferences.Add(RoslynHost.CreateMetadataReference(location));
            }
#else
            // in .NET, the global assembly cache is not supported, thus we threat it as a package reference
            var nugetRefs = await RoslynHost.NuGetReferenceResolver.ResolveReferencesAsync(lib.Value, lib.Version, cancellationToken).ConfigureAwait(false);
            metadataReferences.AddRange(nugetRefs);
#endif
          }
          else if (lib.Kind == LibraryRef.RefKind.PackageReference)
          {
            var nugetRefs = await RoslynHost.NuGetReferenceResolver.ResolveReferencesAsync(lib.Value, lib.Version, cancellationToken).ConfigureAwait(false);
            metadataReferences.AddRange(nugetRefs);
          }
        }
      }

      var lastUsedCodeMetadataReferences = _lastUsedCodeMetadataReferences; // make the variable local to avoid racing conditions
      var (referencesToRemove, referencesToAdd) = ReferenceDirectiveHelper.GetMetadataReferenceDifference(metadataReferences, lastUsedCodeMetadataReferences);

      var project = CurrentSolution.GetProject(ProjectId);

      if (project is not null)
      {
        foreach (var item in referencesToRemove)
        {
          project = project.RemoveMetadataReference(item);
        }
        project = project.AddMetadataReferences(referencesToAdd);
        var newSolution = project.Solution;
        lock (_lastUsedCodeMetadataReferences)
        {
          if (object.ReferenceEquals(_lastUsedCodeMetadataReferences, lastUsedCodeMetadataReferences)) // if no other thread has modified the _lastUsedCodeMetadataReferences in the meantime
          {
            _documentToSetOfLibRefs = libraries; // commit the libraries referenced by #r directives
            _lastUsedCodeMetadataReferences = metadataReferences; // commit the collected and resolved metadata references
            SetCurrentSolution(newSolution); // TODO I don't like to call two functions inside the lock - better idea?
            TryApplyChanges(newSolution);
          }
        }
      }
    }
  }
}
