// Copyright Eli Arbel (no explicit copyright notice in original file), Apache License Version 2.0, January 2004

// Originated from: RoslynPad, RoslynPad.Roslyn, RoslynHost.cs

// Strongly revised for the Altaxo project, Copyright Dr. D. Lellinger, 2017
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Altaxo.CodeEditing.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Host.Mef;
using Roslyn.Utilities;

namespace Altaxo.CodeEditing
{
  public class RoslynHost
  {
    public static RoslynHost Instance { get; private set; }

    private readonly ConcurrentDictionary<DocumentId, AltaxoWorkspaceBase> _workspaces = new();

    public ImmutableHashSet<string> DisabledDiagnostics { get; } = [];


    internal static readonly ImmutableArray<Assembly> DefaultCompositionAssemblies =
            ImmutableArray.Create(
                // Microsoft.CodeAnalysis.Workspaces
                typeof(WorkspacesResources).Assembly,
                // Microsoft.CodeAnalysis.CSharp.Workspaces
                typeof(CSharpWorkspaceResources).Assembly,
                // Microsoft.CodeAnalysis.Features
                typeof(FeaturesResources).Assembly,
                // Microsoft.CodeAnalysis.CSharp.Features
                typeof(CSharpFeaturesResources).Assembly,
                // RoslynPad.Roslyn
                typeof(RoslynHost).Assembly);

    internal static readonly ImmutableArray<Type> DefaultCompositionTypes =
       DefaultCompositionAssemblies.SelectMany(t => t.DefinedTypes).Select(t => t.AsType())
#if !NoDiagnostics
       .Concat(GetDiagnosticCompositionTypes())
#endif
       // .Concat(GetEditorFeaturesTypes())
       .ToImmutableArray();

#if !NoDiagnostics

    private static IEnumerable<Type> GetDiagnosticCompositionTypes() => MetadataUtil.LoadTypesByNamespaces(
        typeof(Microsoft.CodeAnalysis.CodeFixes.ICodeFixService).Assembly,
        "Microsoft.CodeAnalysis.Diagnostics",
        "Microsoft.CodeAnalysis.CodeFixes");

#endif
    /*
    private static IEnumerable<Type> GetEditorFeaturesTypes() => MetadataUtil.LoadTypesBy(
        typeof(CSharpEditorResources).Assembly, t => t.Name.EndsWith("OptionsStorage", StringComparison.Ordinal))
        .SelectMany(t => t.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic).Where(t => t.IsDefined(typeof(ExportLanguageServiceAttribute))));
    */

#if !NoDocumentation
    /// <summary>
    /// In order to have access to DocumentationProviderService, which is located inside a sealed internal class, you have i) to name the assembly "RoslynETAHost" and ii) sign the assembly with the roslyn private key.
    /// </summary>
    private readonly IDocumentationProviderService _documentationProviderService;
#endif
    /// <summary>
    /// Path to the framework assemblies. Used by the documentation service to get the xml documentation of the framework assemblies.
    /// </summary>
    private readonly string _referenceAssembliesPath = GetReferenceAssembliesPath();

    private readonly CompositionHost _compositionContext;

#if !NoDiagnostics
    // private Microsoft.CodeAnalysis.Diagnostics.IDiagnosticService _diagnosticsService;
#endif

    public MefHostServices MefHost { get; }

    public static void InitializeInstance(IEnumerable<Assembly> additionalAssembliesToIncludeInComposition = null)
    {
      Instance = new RoslynHost(additionalAssembliesToIncludeInComposition);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RoslynHost"/> class.
    /// </summary>
    /// <param name="additionalAssembliesToIncludeInComposition">Additional assemblies to include in the composition of the Roslyn assemblies.
    /// By default the following assemblies are included: Microsoft.CodeAnalysis, Microsoft.CodeAnalysis.CSharp,
    /// Microsoft.CodeAnalysis.Features and Microsoft.CodeAnalysis.CSharp.Features.
    /// </param>
    public RoslynHost(IEnumerable<Assembly> additionalAssemblies = null)
    {

      var partTypes = GetDefaultCompositionTypes();



      var assemblies = GetDefaultCompositionAssemblies();

      if (additionalAssemblies is not null)
      {
        partTypes = partTypes.Concat(additionalAssemblies.SelectMany(a => a.DefinedTypes).Select(t => t.AsType()));
      }

      // the following code is usefull if the composition fails
      // here every assembly is inspected seperately to make it easy to find the rogue assembly
      // Attention: some of the dependency DLLs needed for AltaxoCodeEditing might be replaced by other versions, e.g. if another component of Altaxo uses a newer or older version of the same DLL
      // Use FindOutRequiredDLLs to list all DLLs in the final compiled Altaxo bin folder, and compare with the libraries in AltaxoCodeEditing
      // If different, use binding redirects in AltaxoStartup
      /*
      foreach (var ass in assemblies)
      {
        ass.GetTypes();
      }
      */

      _compositionContext = new ContainerConfiguration()
          .WithParts(partTypes)
          .CreateContainer();

      MefHost = MefHostServices.Create(_compositionContext);

#if !NoDocumentation
      _documentationProviderService = new Altaxo.CodeEditing.Documentation.DocumentationProviderServiceFactory.DocumentationProviderService();
#endif

#if !NoDiagnostics
      /*
      _diagnosticsService = GetService<Microsoft.CodeAnalysis.Diagnostics.IDiagnosticService>(); // instantiate diagnostics service to get it working
      // Note that diagnosticsService must be enabled for the workspace by using DiagnosticProvider.Enable(..)

      _diagnosticsService.DiagnosticsUpdated += (s, e) =>
      {
        foreach (var diagnosticArg in e)
        {
          (diagnosticArg.Workspace as Altaxo.CodeEditing.Diagnostics.IDiagnosticsEventSink)?.OnDiagnosticsUpdated(s, diagnosticArg);
        }
      };
      */
#endif
    }

#if !NoDiagnostics
    public virtual IEnumerable<AnalyzerReference> GetSolutionAnalyzerReferences()
    {
      var loader = GetService<IAnalyzerAssemblyLoader>();
      yield return new AnalyzerFileReference(typeof(Compilation).Assembly.Location, loader);
      yield return new AnalyzerFileReference(typeof(CSharpResources).Assembly.Location, loader);
      yield return new AnalyzerFileReference(typeof(FeaturesResources).Assembly.Location, loader);
      yield return new AnalyzerFileReference(typeof(CSharpFeaturesResources).Assembly.Location, loader);
    }
#endif

    protected virtual IEnumerable<Assembly> GetDefaultCompositionAssemblies() =>
            DefaultCompositionAssemblies;

    protected virtual IEnumerable<Type> GetDefaultCompositionTypes() => DefaultCompositionTypes;


    private static string GetReferenceAssembliesPath()
    {
      var programFiles =
          Environment.GetFolderPath(Environment.Is64BitOperatingSystem
              ? Environment.SpecialFolder.ProgramFilesX86
              : Environment.SpecialFolder.ProgramFiles);
      var path = Path.Combine(programFiles, @"Reference Assemblies\Microsoft\Framework\.NETFramework");
      if (Directory.Exists(path))
      {
        var directory = Directory.EnumerateDirectories(path)
            .Select(x => new { path = x, version = GetFxVersionFromPath(x) })
            .OrderByDescending(x => x.version)
            .FirstOrDefault(x => File.Exists(Path.Combine(x.path, "System.dll")) &&
                                 File.Exists(Path.Combine(x.path, "System.xml")));
        return directory?.path;
      }
      return null;
    }

    private static Version GetFxVersionFromPath(string path)
    {
      var name = Path.GetFileName(path);
      if (name?.StartsWith("v", StringComparison.OrdinalIgnoreCase) == true)
      {
        if (Version.TryParse(name.Substring(1), out var version))
        {
          return version;
        }
      }

      return new Version(0, 0, 0);
    }

    public IEnumerable<string> TryGetFacadeAssemblies()
    {
      var facadesPath = Path.Combine(_referenceAssembliesPath, "Facades");
      if (Directory.Exists(facadesPath))
      {
        return Directory.EnumerateFiles(facadesPath, "*.dll");
      }

      return Array.Empty<string>();
    }

    /// <summary>
    /// Creates a metadata reference from an assembly location, and sets the corresponding documentation provider.
    /// </summary>
    /// <param name="assemblyLocation">The assembly location.</param>
    /// <returns>A metadata reference.</returns>
    public MetadataReference CreateMetadataReference(string assemblyLocation)
    {
#if !NoDocumentation
      return MetadataReference.CreateFromFile(assemblyLocation, documentation: GetDocumentationProvider(assemblyLocation));
#else
      return MetadataReference.CreateFromFile(assemblyLocation);
#endif
    }

#if !NoDocumentation

    public DocumentationProvider GetDocumentationProvider(string location)
    {
      if (File.Exists(Path.ChangeExtension(location, "xml")))
      {
        return _documentationProviderService.GetDocumentationProvider(location);
      }
      if (_referenceAssembliesPath != null)
      {
        var referenceLocation = Path.Combine(_referenceAssembliesPath, Path.GetFileName(location));
        if (File.Exists(Path.ChangeExtension(referenceLocation, "xml")))
        {
          return _documentationProviderService.GetDocumentationProvider(referenceLocation);
        }
      }
      return null;
    }
#endif

    public TService GetService<TService>()
    {
      return _compositionContext.GetExport<TService>();
    }

    public TService GetWorkspaceService<TService>(DocumentId documentId) where TService : IWorkspaceService =>
        _workspaces[documentId].Services.GetRequiredService<TService>();

    public void CloseWorkspace(Workspace workspace)
    {
      if (workspace is null)
        throw new ArgumentNullException(nameof(workspace));

      foreach (var documentId in workspace.CurrentSolution.Projects.SelectMany(p => p.DocumentIds))
      {
        _workspaces.TryRemove(documentId, out _);
      }

      workspace.Dispose();
    }

    public virtual T RegisterWorkspace<T>(T workspace) where T : AltaxoWorkspaceBase
    {
      // create the updater before any document is opened
      var diagnosticsUpdater = workspace.Services.GetRequiredService<IDiagnosticsUpdater>();
      diagnosticsUpdater.DisabledDiagnostics = DisabledDiagnostics;
      return workspace;
    }

    public void RegisterDocument(DocumentId documentId, AltaxoWorkspaceBase workspace)
    {
      if (documentId is null)
        throw new ArgumentNullException(nameof(documentId));
      if (workspace is null)
        throw new ArgumentNullException(nameof(workspace));
      _workspaces[documentId] = workspace;
    }

  }

  internal class MetadataUtil
  {
    public static IReadOnlyList<Type> LoadTypesByNamespaces(Assembly assembly, params string[] namespaces) =>
        LoadTypesBy(assembly, t => namespaces.Contains(t.Namespace));

    public static IReadOnlyList<Type> LoadTypesBy(Assembly assembly, Func<Type, bool> predicate)
    {
      using var context = new MetadataLoadContext(new PathAssemblyResolver(Directory.GetFiles(RuntimeEnvironment.GetRuntimeDirectory(), "*.dll")));
      var types = context.LoadFromAssemblyPath(assembly.Location).DefinedTypes;
      return types.Where(predicate).Select(t => assembly.GetType(t.FullName!)).WhereNotNull().ToReadOnlyCollection();
    }
  }

}
