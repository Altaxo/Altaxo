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

namespace Altaxo.CodeEditing
{
	/// <summary>
	/// Workspace for script Dlls. Contains exactly one solution, and one project.
	/// </summary>
	/// <seealso cref="Microsoft.CodeAnalysis.Workspace" />
	public class AltaxoWorkspace : Workspace
	{
		/// <summary>
		/// Gets the static references, i.e. all the references that are not stated in the code.
		/// </summary>
		/// <value>
		/// The default references.
		/// </value>
		public ImmutableArray<MetadataReference> StaticReferences { get; }

		/// <summary>
		/// Dictionary that holds all references directives that comes from the code (by #r statements).
		/// </summary>
		private readonly ConcurrentDictionary<string, DirectiveInfo> _referencesDirectives;

		public RoslynHost RoslynHost { get; }

		public DocumentId OpenDocumentId { get; private set; }

		/// <summary>
		/// The project Id of the single project that is contained in this workspace.
		/// </summary>
		public ProjectId ProjectId { get; private set; }

		protected Dictionary<DocumentId, Action<SourceText>> _sourceTextChangedHandlers = new Dictionary<DocumentId, Action<SourceText>>();

		private static readonly ImmutableArray<string> PreprocessorSymbols = ImmutableArray.CreateRange(new[] { "TRACE", "DEBUG" });

		public AltaxoWorkspace(RoslynHost roslynHost, string workingDirectory, IEnumerable<MetadataReference> staticReferences)
				: base(roslynHost.MefHost, WorkspaceKind.Host)
		{
			_referencesDirectives = new ConcurrentDictionary<string, DirectiveInfo>();
			RoslynHost = roslynHost;
			StaticReferences = staticReferences.ToImmutableArray();

			var compilationOptions = CreateCompilationOptions(workingDirectory);
			var parseOptions = new CSharpParseOptions(kind: SourceCodeKind.Script, preprocessorSymbols: PreprocessorSymbols);
			CreateInitialProject(compilationOptions, parseOptions);
		}

		private void CreateInitialProject(CSharpCompilationOptions compilationOptions, CSharpParseOptions parseOptions)
		{
			var name = "Prj" + Guid.NewGuid().ToString();
			ProjectId = ProjectId.CreateNewId(name);
			var projectInfo = ProjectInfo.Create(
				ProjectId,
				VersionStamp.Create(),
				name, // project name
				name, // assembly name
				LanguageNames.CSharp, // language
				parseOptions: parseOptions,
				compilationOptions: compilationOptions.WithScriptClassName(name),
				metadataReferences: StaticReferences
				);

			var newSolution = this.CurrentSolution.AddProject(projectInfo);
			base.SetCurrentSolution(newSolution);
		}

		public Document CreateDocument(SourceTextContainer textContainer, Action<SourceText> onTextUpdated)
		{
			var project = this.CurrentSolution.GetProject(ProjectId);
			var documentId = DocumentId.CreateNewId(ProjectId);
			var newSolution = project.Solution.AddDocument(documentId, project.Name, textContainer.CurrentText);
			this.SetCurrentSolution(newSolution);
			this.OpenDocument(documentId, textContainer);
			OpenDocumentId = documentId;

			if (null != onTextUpdated)
			{
				_sourceTextChangedHandlers[documentId] = onTextUpdated;
			}

			return newSolution.GetDocument(documentId);
		}

		private CSharpCompilationOptions CreateCompilationOptions(string workingDirectory)
		{
			var metadataReferenceResolver = RoslynHost.CreateMetadataReferenceResolver(this, workingDirectory);
			var compilationOptions = new CSharpCompilationOptions(OutputKind.NetModule,
					usings: null,
					allowUnsafe: true,
					sourceReferenceResolver: new SourceFileResolver(ImmutableArray<string>.Empty, workingDirectory),
					metadataReferenceResolver: metadataReferenceResolver);
			return compilationOptions;
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

		public void OpenDocument(DocumentId documentId, SourceTextContainer textContainer)
		{
			OpenDocumentId = documentId;
			OnDocumentOpened(documentId, textContainer);
			OnDocumentContextUpdated(documentId);
		}

		//public event Action<DocumentId, SourceText> ApplyingTextChange;

		protected override void Dispose(bool finalize)
		{
			base.Dispose(finalize);

			// ApplyingTextChange = null;
		}

		protected override void ApplyDocumentTextChanged(DocumentId document, SourceText newText)
		{
			if (_sourceTextChangedHandlers.TryGetValue(document, out var action) && null != action)
				action.Invoke(newText);

			// ApplyingTextChange?.Invoke(document, newText);

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

		private class DirectiveInfo
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
		internal async Task ProcessReferenceDirectives(Document document)
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

		private MetadataReference ResolveReference(string name)
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

		public bool HasReference(string text)
		{
			DirectiveInfo info;
			if (_referencesDirectives.TryGetValue(text, out info))
			{
				return info.IsActive;
			}
			return false;
		}
	}
}