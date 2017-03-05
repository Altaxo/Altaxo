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
						typeof(object), // mscorlib
						typeof(Thread), // mscorlib
						typeof(Task), // mscorlib
						typeof(List<>), // mscorlib
						typeof(Regex), // system
						typeof(StringBuilder), // mscorlib
						typeof(Uri), // system
						typeof(Enumerable), // system.core
						typeof(IEnumerable), // mscorlib
						typeof(Path), // mscorlib
						typeof(Assembly), // mscorlib
				}.ToImmutableArray();

		/// <summary>
		/// Gets a proposal for the default assemblies that should be referenced in compilations.
		/// </summary>
		/// <value>
		/// The default reference assemblies.
		/// </value>
		public static ImmutableArray<Assembly> DefaultReferenceAssemblies { get; protected set; } =
				_defaultReferenceAssemblyTypes.Select(x => x.Assembly).Concat(new[]
				{
								Assembly.Load("System.Runtime, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"),
								typeof(Microsoft.CSharp.RuntimeBinder.Binder).Assembly,
				}).Distinct().ToImmutableArray();

		internal static readonly ImmutableArray<string> PreprocessorSymbols = ImmutableArray.CreateRange(new[] { "TRACE", "DEBUG" });

		/// <summary>Dictionary that maps source document IDs to its corresponding workspace.</summary>
		private readonly ConcurrentDictionary<DocumentId, AltaxoWorkspace> _workspaces;

		/// <summary>Dictionary that holds for source document IDs an action that is called if the diagnostics for that source document has been updated.</summary>
		private readonly ConcurrentDictionary<DocumentId, Action<DiagnosticsUpdatedArgs>> _diagnosticsUpdatedNotifiers;

		private readonly CSharpParseOptions _parseOptions;

		/// <summary>
		/// In order to have access to DocumentationProviderService, which is located inside a sealed internal class, you have i) to name the assembly "RoslynETAHost" and ii) sign the assembly with the roslyn private key.
		/// </summary>
		private readonly Microsoft.CodeAnalysis.Host.IDocumentationProviderService _documentationProviderService;

		private readonly string _referenceAssembliesPath;
		private readonly CompositionHost _compositionContext;
		public MefHostServices MefHost { get; }

		private int _documentNumber;

		internal ImmutableArray<MetadataReference> DefaultReferences { get; }

		internal ImmutableArray<string> DefaultImports { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="RoslynHost"/> class.
		/// </summary>
		/// <param name="additionalAssemblies">Additional assemblies to include in the composition of the Roslyn assemblies.
		/// By default the following assemblies are included: Microsoft.CodeAnalysis, Microsoft.CodeAnalysis.CSharp,
		/// Microsoft.CodeAnalysis.Features and Microsoft.CodeAnalysis.CSharp.Features.
		/// </param>
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
							.ToArray();

			_compositionContext = new ContainerConfiguration()
					.WithParts(partTypes)
					.WithDefaultConventions(new AttributeFilterProvider())
					.CreateContainer();

			MefHost = MefHostServices.Create(_compositionContext);

			_parseOptions = new CSharpParseOptions(kind: SourceCodeKind.Script, preprocessorSymbols: PreprocessorSymbols);

			_referenceAssembliesPath = GetReferenceAssembliesPath();

			_documentationProviderService = new Altaxo.CodeEditing.Documentation.DocumentationProviderServiceFactory.DocumentationProviderService();

			DefaultReferences = DefaultReferenceAssemblies.Select(t => CreateMetadataReference(t.Location)).ToImmutableArray();

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

		/// <summary>
		/// Adds a document to be managed by this instance. For each individual document, a separate workspace is created, with a solution and a project,
		/// and the document is then added to the project.
		/// </summary>
		/// <param name="sourceTextContainer">The source text container managing the document's source text.</param>
		/// <param name="workingDirectory">The working directory. This is the root directory for resolving unknown metadata (assembly) references.</param>
		/// <param name="onTextUpdated">Action that is called every time when the source text is updated.</param>
		/// <param name="references">The project's static references, i.e. all references except those referenced in the code by #r statements.</param>
		/// <returns>The Id of the document that is created and added to this instance.</returns>
		public DocumentId AddDocument(SourceTextContainer sourceTextContainer, string workingDirectory, Action<SourceText> onTextUpdated, IEnumerable<MetadataReference> references)
		{
			if (sourceTextContainer == null)
				throw new ArgumentNullException(nameof(sourceTextContainer));
			if (string.IsNullOrEmpty(workingDirectory))
				throw new ArgumentNullException(nameof(workingDirectory));

			var workspace = new AltaxoWorkspace(this, workingDirectory, references);
			workspace.CreateDocument(sourceTextContainer, onTextUpdated);
			AddWorkspace(workspace);
			return workspace.OpenDocumentId;
		}

		public void AddWorkspace(AltaxoWorkspace workspace)
		{
			if (null == workspace)
				throw new ArgumentNullException(nameof(workspace));
			if (!object.ReferenceEquals(this, workspace.RoslynHost))
				throw new ArgumentException("The workspace was created using a different RoslynHost!", nameof(workspace));
			if (null == workspace.OpenDocumentId)
				throw new ArgumentException("The workspace does not contain an open document!", nameof(workspace));

			DiagnosticProvider.Enable(workspace, DiagnosticProvider.Options.Semantic);
			_workspaces.TryAdd(workspace.OpenDocumentId, workspace);
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

		/// <summary>
		/// Gets the current document that is identified by its document ID.
		/// </summary>
		/// <param name="documentId">The document identifier.</param>
		/// <returns>The current document corresponding to the document ID.</returns>
		public Document GetDocument(DocumentId documentId)
		{
			return _workspaces.TryGetValue(documentId, out var workspace) ? workspace.CurrentSolution.GetDocument(documentId) : null;
		}

		/// <summary>
		/// Gets the workspace that contains the document identified by its document ID.
		/// </summary>
		/// <param name="documentId">The document identifier.</param>
		/// <returns>The workspace that contains the document identified by its document ID.</returns>
		public Workspace GetWorkspace(DocumentId documentId)
		{
			return _workspaces.TryGetValue(documentId, out var workspace) ? workspace : null;
		}

		/// <summary>
		/// Updates the document. The workspace containing the document is updated with the new document version.
		/// </summary>
		/// <param name="document">The updated document.</param>
		public void UpdateDocument(Document document)
		{
			if (!_workspaces.TryGetValue(document.Id, out var workspace))
			{
				return;
			}

			workspace.TryApplyChanges(document.Project.Solution);
		}

		/// <summary>
		/// Closes the document. Currently it is assumed that one workspace contains only one document.
		/// Thus by closing the document, the corresponding workspace is closed, too, and removed from the workspaces collection.
		/// </summary>
		/// <param name="documentId">The document identifier.</param>
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

		private Project CreateSubmissionProject(Solution solution, CSharpCompilationOptions compilationOptions, IEnumerable<MetadataReference> references)
		{
			var name = "Program" + _documentNumber++;
			var projectId = ProjectId.CreateNewId(name);
			var projectInfo = ProjectInfo.Create(
				projectId,
				VersionStamp.Create(),
				name, // project name
				name, // assembly name
				LanguageNames.CSharp, // language
				parseOptions: _parseOptions,
				compilationOptions: compilationOptions.WithScriptClassName(name),
				metadataReferences: references
				);

			solution = solution.AddProject(projectInfo);
			return solution.GetProject(projectId);
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

		public static MetadataReferenceResolver CreateMetadataReferenceResolver(Workspace workspace, string workingDirectory)
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