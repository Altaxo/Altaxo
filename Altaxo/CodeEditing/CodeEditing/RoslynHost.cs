// Copyright Eli Arbel (no explicit copyright notice in original file), Apache License Version 2.0, January 2004

// Originated from: RoslynPad, RoslynPad.Roslyn, RoslynHost.cs

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition.Convention;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Text;
using Altaxo.CodeEditing.Diagnostics;

namespace Altaxo.CodeEditing
{
	public class RoslynHost
	{
		private static readonly ImmutableArray<Type> _defaultReferenceAssemblyTypes = new[] {
						typeof(object),
						typeof(Thread),
						typeof(Task),
						typeof(List<>),
						typeof(Regex),
						typeof(StringBuilder),
						typeof(Uri),
						typeof(Enumerable),
						typeof(IEnumerable),
						typeof(Path),
						typeof(Assembly),
				}.ToImmutableArray();

		private static readonly ImmutableArray<Assembly> _defaultReferenceAssemblies =
				_defaultReferenceAssemblyTypes.Select(x => x.Assembly).Distinct().Concat(new[]
				{
								Assembly.Load("System.Runtime, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"),
								typeof(Microsoft.CSharp.RuntimeBinder.Binder).Assembly,
				}).ToImmutableArray();

		internal static readonly ImmutableArray<string> PreprocessorSymbols = ImmutableArray.CreateRange(new[] { "TRACE", "DEBUG" });

		private readonly ConcurrentDictionary<DocumentId, AltaxoWorkspace> _workspaces;
		private readonly ConcurrentDictionary<DocumentId, Action<DiagnosticsUpdatedArgs>> _diagnosticsUpdatedNotifiers;

		private readonly CSharpParseOptions _parseOptions;

		/// <summary>
		/// In order to have access to DocumentationProviderService, which is located inside a sealed internal class, you have i) to name the assembly "RoslynETAHost" and ii) sign the assembly with the roslyn private key.
		/// </summary>
		private readonly Microsoft.CodeAnalysis.Host.IDocumentationProviderService _documentationProviderService;

		private readonly string _referenceAssembliesPath;
		private readonly CompositionHost _compositionContext;
		private readonly MefHostServices _host;

		private int _documentNumber;

		internal ImmutableArray<MetadataReference> DefaultReferences { get; }

		internal ImmutableArray<string> DefaultImports { get; }

		public RoslynHost(IEnumerable<Assembly> additionalAssemblies = null)
		{
			_workspaces = new ConcurrentDictionary<DocumentId, AltaxoWorkspace>();
			_diagnosticsUpdatedNotifiers = new ConcurrentDictionary<DocumentId, Action<DiagnosticsUpdatedArgs>>();

			var assemblies = new[]
			{
								Assembly.Load("Microsoft.CodeAnalysis"),
								Assembly.Load("Microsoft.CodeAnalysis.CSharp"),
								Assembly.Load("Microsoft.CodeAnalysis.Features"),
								Assembly.Load("Microsoft.CodeAnalysis.CSharp.Features"),
								typeof(RoslynHost).Assembly,
			};
			if (additionalAssemblies != null)
			{
				assemblies = assemblies.Concat(additionalAssemblies).ToArray();
			}

			// the following code is usefull if the composition fails
			// here every assembly is inspected seperately to make it easy to find the rogue assembly
			/*
			 foreach (var ass in assemblies)
			{
				ass.GetTypes();
			}
			*/

			var partTypes = MefHostServices.DefaultAssemblies.Concat(assemblies)
							.Distinct()
							.SelectMany(x => x.GetTypes())
							//.Concat(new[] { typeof(Altaxo.CodeEditing.Documentation.DocumentationProviderServiceFactory) })
							.ToArray();

			_compositionContext = new ContainerConfiguration()
					.WithParts(partTypes)
					.WithDefaultConventions(new AttributeFilterProvider())
					.CreateContainer();

			_host = MefHostServices.Create(_compositionContext);

			_parseOptions = new CSharpParseOptions(kind: SourceCodeKind.Script, preprocessorSymbols: PreprocessorSymbols);

			_referenceAssembliesPath = GetReferenceAssembliesPath();

			_documentationProviderService = new Altaxo.CodeEditing.Documentation.DocumentationProviderServiceFactory.DocumentationProviderService();

			DefaultReferences = _defaultReferenceAssemblies.Select(t => CreateMetadataReference(t.Location)).ToImmutableArray();

			DefaultImports = _defaultReferenceAssemblyTypes.Select(x => x.Namespace).Distinct().ToImmutableArray();

			GetService<IDiagnosticService>().DiagnosticsUpdated += OnDiagnosticsUpdated;
		}

		private void OnDiagnosticsUpdated(object sender, DiagnosticsUpdatedArgs diagnosticsUpdatedArgs)
		{
			var documentId = diagnosticsUpdatedArgs?.DocumentId;
			if (documentId == null)
				return;

			OnOpenedDocumentSyntaxChanged(GetDocument(documentId));

			Action<DiagnosticsUpdatedArgs> notifier;
			if (_diagnosticsUpdatedNotifiers.TryGetValue(documentId, out notifier))
			{
				notifier(diagnosticsUpdatedArgs);
			}
		}

		private async void OnOpenedDocumentSyntaxChanged(Document document)
		{
			if (_workspaces.TryGetValue(document.Id, out var workspace))
			{
				await workspace.ProcessReferenceDirectives(document).ConfigureAwait(false);
			}
		}

		private static string GetReferenceAssembliesPath()
		{
			var programFiles =
					Environment.GetFolderPath(Environment.Is64BitOperatingSystem
							? Environment.SpecialFolder.ProgramFilesX86
							: Environment.SpecialFolder.ProgramFiles);
			var path = Path.Combine(programFiles, @"Reference Assemblies\Microsoft\Framework\.NETFramework");
			if (Directory.Exists(path))
			{
				var directories = Directory.EnumerateDirectories(path).OrderByDescending(Path.GetFileName);
				return directories.FirstOrDefault();
			}
			return null;
		}

		/// <summary>
		/// Creates a metadata reference from an assembly location, and sets the corresponding documentation provider.
		/// </summary>
		/// <param name="assemblyLocation">The assembly location.</param>
		/// <returns>A metadata reference.</returns>
		public MetadataReference CreateMetadataReference(string assemblyLocation)
		{
			return MetadataReference.CreateFromFile(assemblyLocation, documentation: GetDocumentationProvider(assemblyLocation));
		}

		private DocumentationProvider GetDocumentationProvider(string location)
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

		public TService GetService<TService>()
		{
			return _compositionContext.GetExport<TService>();
		}

		public DocumentId AddDocument(SourceTextContainer sourceTextContainer, string workingDirectory, Action<SourceText> onTextUpdated, IEnumerable<MetadataReference> additionalReferences = null)
		{
			if (sourceTextContainer == null)
				throw new ArgumentNullException(nameof(sourceTextContainer));
			if (string.IsNullOrEmpty(workingDirectory))
				throw new ArgumentNullException(nameof(workingDirectory));

			var workspace = new AltaxoWorkspace(_host, this);
			if (onTextUpdated != null)
			{
				workspace.ApplyingTextChange += (d, s) => onTextUpdated(s);
			}

			DiagnosticProvider.Enable(workspace, DiagnosticProvider.Options.Semantic);

			var currentSolution = workspace.CurrentSolution;

			// create a new project in the overall solution
			var project = CreateSubmissionProject(currentSolution, CreateCompilationOptions(workspace, workingDirectory), additionalReferences);
			var currentDocument = SetSubmissionDocument(workspace, sourceTextContainer, project);

			_workspaces.TryAdd(currentDocument.Id, workspace);

			return currentDocument.Id;
		}

		public void SubscribeToDiagnosticsUpdateNotification(DocumentId documentId, Action<DiagnosticsUpdatedArgs> onDiagnosticsUpdated)
		{
			if (null == documentId)
				throw new ArgumentNullException(nameof(documentId));
			if (null == onDiagnosticsUpdated)
				throw new ArgumentNullException(nameof(onDiagnosticsUpdated));

			if (!_diagnosticsUpdatedNotifiers.TryAdd(documentId, onDiagnosticsUpdated))
			{
				if (_diagnosticsUpdatedNotifiers.TryGetValue(documentId, out var handler))
				{
					handler -= onDiagnosticsUpdated; // in case it is already registed
					handler += onDiagnosticsUpdated;
					throw new NotImplementedException("The previous code needs verification");
				}
			}
		}

		public Document GetDocument(DocumentId documentId)
		{
			return _workspaces.TryGetValue(documentId, out var workspace) ? workspace.CurrentSolution.GetDocument(documentId) : null;
		}

		public Workspace GetWorkspace(DocumentId documentId)
		{
			return _workspaces.TryGetValue(documentId, out var workspace) ? workspace : null;
		}

		public void UpdateDocument(Document document)
		{
			if (!_workspaces.TryGetValue(document.Id, out var workspace))
			{
				return;
			}

			workspace.TryApplyChanges(document.Project.Solution);
		}

		public void CloseDocument(DocumentId documentId)
		{
			if (_workspaces.TryGetValue(documentId, out var workspace))
			{
				DiagnosticProvider.Disable(workspace);
				workspace.Dispose();
				_workspaces.TryRemove(documentId, out workspace);
			}
			_diagnosticsUpdatedNotifiers.TryRemove(documentId, out var notifier);
		}

		private Project CreateSubmissionProject(Solution solution, CSharpCompilationOptions compilationOptions, IEnumerable<MetadataReference> additionalReferences)
		{
			var name = "Program" + _documentNumber++;
			var id = ProjectId.CreateNewId(name);
			solution = solution.AddProject(ProjectInfo.Create(id, VersionStamp.Create(), name, name, LanguageNames.CSharp,
					parseOptions: _parseOptions,
					compilationOptions: compilationOptions.WithScriptClassName(name),
					metadataReferences: additionalReferences == null ? DefaultReferences : DefaultReferences.Concat(additionalReferences)));
			return solution.GetProject(id);
		}

		private static Document SetSubmissionDocument(AltaxoWorkspace workspace, SourceTextContainer textContainer, Project project)
		{
			var id = DocumentId.CreateNewId(project.Id);
			var solution = project.Solution.AddDocument(id, project.Name, textContainer.CurrentText);
			workspace.SetCurrentSolution(solution);
			workspace.OpenDocument(id, textContainer);
			return solution.GetDocument(id);
		}

		private CSharpCompilationOptions CreateCompilationOptions(Workspace workspace, string workingDirectory)
		{
			var metadataReferenceResolver = CreateMetadataReferenceResolver(workspace, workingDirectory);
			var compilationOptions = new CSharpCompilationOptions(OutputKind.NetModule,
					usings: DefaultImports,
					allowUnsafe: true,
					sourceReferenceResolver: new SourceFileResolver(ImmutableArray<string>.Empty, workingDirectory),
					metadataReferenceResolver: metadataReferenceResolver);
			return compilationOptions;
		}

		private static MetadataReferenceResolver CreateMetadataReferenceResolver(Workspace workspace, string workingDirectory)
		{
			var resolver = Activator.CreateInstance(
					// can't access this type due to a name collision with Scripting assembly
					// can't use extern alias because of project.json
					// ReSharper disable once AssignNullToNotNullAttribute
					Type.GetType("Microsoft.CodeAnalysis.RelativePathResolver, Microsoft.CodeAnalysis.Workspaces"),
					ImmutableArray<string>.Empty,
					workingDirectory);
			return (MetadataReferenceResolver)Activator.CreateInstance(typeof(WorkspaceMetadataFileReferenceResolver),
					workspace.Services.GetService<IMetadataService>(),
					resolver);
		}

		#region Inner classes

		/// <summary>
		/// See 'Completion service too hard to instantiate - need improvements to MSBuildWorkspace.Create' (<see href="https://github.com/dotnet/roslyn/issues/12218"/>)
		/// for why this class is needed.
		/// </summary>
		private class AttributeFilterProvider : AttributedModelProvider
		{
			public override IEnumerable<Attribute> GetCustomAttributes(Type reflectedType, MemberInfo member)
			{
				var customAttributes = member.GetCustomAttributes().Where(x => !(x is ExtensionOrderAttribute)).ToArray();
				return customAttributes;
			}

			public override IEnumerable<Attribute> GetCustomAttributes(Type reflectedType, ParameterInfo member)
			{
				var customAttributes = member.GetCustomAttributes().Where(x => !(x is ExtensionOrderAttribute)).ToArray();
				return customAttributes;
			}
		}

		#endregion Inner classes
	}
}