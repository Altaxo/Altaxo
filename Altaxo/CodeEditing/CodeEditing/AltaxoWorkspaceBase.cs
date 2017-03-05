// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Text;
using Roslyn.Utilities;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Altaxo.CodeEditing.Diagnostics;

namespace Altaxo.CodeEditing
{
	/// <summary>
	/// Abstract workspace that is the base for different kind of workspaces.
	/// </summary>
	/// <seealso cref="Microsoft.CodeAnalysis.Workspace" />
	public abstract class AltaxoWorkspaceBase : Workspace, IAltaxoWorkspace, IDiagnosticsEventSink
	{
		protected static readonly ImmutableArray<Type> _defaultReferenceAssemblyTypes = new[] {
						typeof(object), // mscorlib
						typeof(System.Threading.Thread), // mscorlib
						typeof(Task), // mscorlib
						typeof(List<>), // mscorlib
						typeof(System.Text.RegularExpressions.Regex), // system
						typeof(System.Text.StringBuilder), // mscorlib
						typeof(Uri), // system
						typeof(Enumerable), // system.core
						typeof(System.Collections.IEnumerable), // mscorlib
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

		/// <summary>
		/// Gets the static references of the project, i.e. all references that are not stated in the code by #r statements.
		/// </summary>
		/// <value>
		/// The static references.
		/// </value>
		public ImmutableArray<MetadataReference> StaticReferences { get; }

		/// <summary>
		/// Dictionary that holds all references directives that comes from the code (by #r statements).
		/// </summary>
		private readonly ConcurrentDictionary<string, DirectiveInfo> _referencesDirectives;

		/// <summary>
		/// Gets the reference to the roslyn host.
		/// </summary>
		/// <value>
		/// The roslyn host.
		/// </value>
		public RoslynHost RoslynHost { get; }

		/// <summary>
		/// The project Id of the single project that is contained in this workspace.
		/// </summary>
		public ProjectId ProjectId { get; private set; }

		public string WorkingDirectory { get; }

		/// <summary>
		/// Stores for each document in this workspace a handler that is called when the text has changed.
		/// </summary>
		protected Dictionary<DocumentId, Action<SourceText>> _sourceTextChangedHandlers = new Dictionary<DocumentId, Action<SourceText>>();

		/// <summary>Dictionary that holds for source document IDs an action that is called if the diagnostics for that source document has been updated.</summary>
		protected ConcurrentDictionary<DocumentId, Action<DiagnosticsUpdatedArgs>> _diagnosticsUpdatedNotifiers = new ConcurrentDictionary<DocumentId, Action<DiagnosticsUpdatedArgs>>();

		private static readonly ImmutableArray<string> DefaultPreprocessorSymbols = ImmutableArray.CreateRange(new[] { "TRACE", "DEBUG" });

		/// <summary>
		/// Gets the preprocessor symbols that are used for parsing and compilation.
		/// </summary>
		/// <value>
		/// The preprocessor symbols.
		/// </value>
		public ImmutableArray<string> PreprocessorSymbols { get; }

		public AltaxoWorkspaceBase(RoslynHost roslynHost, string workingDirectory, IEnumerable<MetadataReference> staticReferences)
				: base(roslynHost.MefHost, WorkspaceKind.Host)
		{
			_referencesDirectives = new ConcurrentDictionary<string, DirectiveInfo>();
			RoslynHost = roslynHost;
			WorkingDirectory = workingDirectory;
			StaticReferences = staticReferences.ToImmutableArray();
			PreprocessorSymbols = DefaultPreprocessorSymbols;
			ProjectId = CreateInitialProject();
		}

		protected abstract ProjectId CreateInitialProject();

		public abstract Compilation GetCompilation(string assemblyName);

		public virtual Document CreateAndOpenDocument(SourceTextContainer textContainer, Action<SourceText> onTextUpdated)
		{
			var project = this.CurrentSolution.GetProject(ProjectId);
			var documentId = DocumentId.CreateNewId(ProjectId);
			var newSolution = project.Solution.AddDocument(documentId, project.Name, textContainer.CurrentText);
			this.SetCurrentSolution(newSolution);
			this.OpenDocument(documentId, textContainer);

			if (null != onTextUpdated)
			{
				_sourceTextChangedHandlers[documentId] = onTextUpdated;
			}

			return newSolution.GetDocument(documentId);
		}

		/// <summary>
		/// Updates the document. The workspace containing the document is updated with the new document version.
		/// </summary>
		/// <param name="document">The updated document.</param>
		public virtual void UpdateDocument(Document document)
		{
			this.TryApplyChanges(document.Project.Solution);
		}

		public new void SetCurrentSolution(Solution solution)
		{
			var oldSolution = CurrentSolution;
			var newSolution = base.SetCurrentSolution(solution);
			RaiseWorkspaceChangedEventAsync(WorkspaceChangeKind.SolutionChanged, oldSolution, newSolution);
		}

		public override bool CanOpenDocuments => true;

		public override bool CanApplyChange(ApplyChangesKind feature)
		{
			switch (feature)
			{
				case ApplyChangesKind.ChangeDocument:
					return true;

				default:
					return false;
			}
		}

		/// <summary>
		/// Brings the document in the opened state.
		/// </summary>
		/// <param name="documentId">The document identifier.</param>
		/// <param name="textContainer">The text container.</param>
		public void OpenDocument(DocumentId documentId, SourceTextContainer textContainer)
		{
			OnDocumentOpened(documentId, textContainer);
			OnDocumentContextUpdated(documentId);
		}

		public override void CloseDocument(DocumentId documentId)
		{
			base.CloseDocument(documentId);
			OnDocumentClosed(documentId, TextLoader.From(TextAndVersion.Create(CurrentSolution.GetDocument(documentId).GetTextAsync().Result, VersionStamp.Create())));
		}

		/// <summary>
		/// Is called by the roslyn host if the diagnostics for a document of this workspace has been updated.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="diagnosticsUpdatedArgs">The diagnostics updated arguments.</param>
		public void OnDiagnosticsUpdated(object sender, DiagnosticsUpdatedArgs diagnosticsUpdatedArgs)
		{
			var documentId = diagnosticsUpdatedArgs?.DocumentId;
			if (documentId != null)
			{
				if (_diagnosticsUpdatedNotifiers.TryGetValue(documentId, out var notifier))
				{
					var document = CurrentSolution.GetDocument(documentId);
					ProcessReferenceDirectives(document).ConfigureAwait(false);
					notifier(diagnosticsUpdatedArgs);
				}
			}
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

			// enable diagnostics now, if not already enabled
			DiagnosticProvider.Enable(this, DiagnosticProvider.Options.Semantic);
		}

		protected override void Dispose(bool finalize)
		{
			DiagnosticProvider.Disable(this);
			base.Dispose(finalize);
		}

		protected override void ApplyDocumentTextChanged(DocumentId document, SourceText newText)
		{
			if (_sourceTextChangedHandlers.TryGetValue(document, out var action) && null != action)
				action.Invoke(newText);

			OnDocumentTextChanged(document, newText, PreservationMode.PreserveIdentity);
		}

		public new void ClearSolution()
		{
			base.ClearSolution();
		}

		internal void ClearOpenDocument(DocumentId documentId)
		{
			base.ClearOpenDocument(documentId);
		}

		internal new void RegisterText(SourceTextContainer textContainer)
		{
			base.RegisterText(textContainer);
		}

		internal new void UnregisterText(SourceTextContainer textContainer)
		{
			base.UnregisterText(textContainer);
		}

		public ImmutableArray<string> ReferencesDirectives => _referencesDirectives.Select(x => x.Key).ToImmutableArray();

		protected class DirectiveInfo
		{
			public MetadataReference MetadataReference { get; }

			public bool IsActive { get; set; }

			public DirectiveInfo(MetadataReference metadataReference)
			{
				MetadataReference = metadataReference;
				IsActive = true;
			}
		}

		/// <summary>
		/// Processes the reference directives. Searches the provided document for #r directives. Then it
		/// updates the <see cref="_referencesDirectives"/> dictionary. Directives no longer in the document are marked as passive
		/// in the reference dictionary, all that are there are marked active. If changes have occured, the project is updated
		/// to reference all active references.
		/// </summary>
		/// <param name="document">The document.</param>
		/// <returns></returns>
		public virtual async Task ProcessReferenceDirectives(Document document)
		{
			var project = document.Project;
			var directives = ((CompilationUnitSyntax)await document.GetSyntaxRootAsync().ConfigureAwait(false))
					.GetReferenceDirectives().Select(x => x.File.ValueText).ToImmutableHashSet();

			var changed = false;
			foreach (var referenceDirective in _referencesDirectives)
			{
				if (referenceDirective.Value.IsActive && !directives.Contains(referenceDirective.Key))
				{
					referenceDirective.Value.IsActive = false;
					changed = true;
				}
			}

			foreach (var directive in directives)
			{
				DirectiveInfo referenceDirective;
				if (_referencesDirectives.TryGetValue(directive, out referenceDirective))
				{
					if (!referenceDirective.IsActive)
					{
						referenceDirective.IsActive = true;
						changed = true;
					}
				}
				else
				{
					if (_referencesDirectives.TryAdd(directive, new DirectiveInfo(ResolveReference(directive))))
					{
						changed = true;
					}
				}
			}

			if (!changed)
				return;

			lock (_referencesDirectives)
			{
				var solution = project.Solution;
				var references =
						_referencesDirectives.Where(x => x.Value.IsActive)
								.Select(x => x.Value.MetadataReference)
								.WhereNotNull();
				var newSolution = solution.WithProjectMetadataReferences(project.Id,
						this.StaticReferences.Concat(references));

				SetCurrentSolution(newSolution);
			}
		}

		/// <summary>
		/// Gets all references currently references by the project, i.e. the <see cref="StaticReferences"/> plus the references
		/// referenced in the code by #r directives.
		/// </summary>
		/// <value>
		/// All references.
		/// </value>
		public IEnumerable<MetadataReference> AllReferences
		{
			get
			{
				return this.StaticReferences.Concat(
					_referencesDirectives
						.Where(x => x.Value.IsActive)
						.Select(x => x.Value.MetadataReference)
						.WhereNotNull()
						);
			}
		}

		protected virtual MetadataReference ResolveReference(string name)
		{
			if (File.Exists(name))
			{
				return RoslynHost.CreateMetadataReference(name);
			}
			try
			{
				var assemblyName = GlobalAssemblyCache.Instance.ResolvePartialName(name);
				if (assemblyName == null)
				{
					return null;
				}
				var assembly = Assembly.Load(assemblyName.ToString());
				return RoslynHost.CreateMetadataReference(assembly.Location);
			}
			catch (Exception)
			{
				return null;
			}
		}

		public virtual bool HasReference(string text)
		{
			DirectiveInfo info;
			if (_referencesDirectives.TryGetValue(text, out info))
			{
				return info.IsActive;
			}
			return false;
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
	}
}