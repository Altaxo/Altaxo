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

extern alias MCW;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Altaxo.CodeEditing.Diagnostics;
using MCW::Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Text;
using Roslyn.Utilities;

namespace Altaxo.CodeEditing
{
  /// <summary>
  /// Abstract workspace that is the base for different kind of workspaces.
  /// </summary>
  /// <seealso cref="Microsoft.CodeAnalysis.Workspace" />
  public abstract class AltaxoWorkspaceBase :
    Workspace,
    IAltaxoWorkspace
#if !NoDiagnostics
    , IDiagnosticsEventSink
#endif
  {
    /// <summary>
    /// Gets the static references of the project, i.e. all references that are not stated in the code by #r statements.
    /// </summary>
    /// <value>
    /// The static references.
    /// </value>
    public ImmutableArray<MetadataReference> StaticReferences { get; }

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

#if !NoDiagnostics
    /// <summary>Dictionary that holds for source document IDs an action that is called if the diagnostics for that source document has been updated.</summary>
    protected ConcurrentDictionary<DocumentId, Action<DiagnosticsUpdatedArgs>> _diagnosticsUpdatedNotifiers = new ConcurrentDictionary<DocumentId, Action<DiagnosticsUpdatedArgs>>();
#endif

    private static readonly ImmutableArray<string> DefaultPreprocessorSymbols = ImmutableArray.CreateRange(new[] { "TRACE", "DEBUG" });

    /// <summary>
    /// Gets the preprocessor symbols that are used for parsing and compilation.
    /// </summary>
    /// <value>
    /// The preprocessor symbols.
    /// </value>
    public ImmutableArray<string> PreprocessorSymbols { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AltaxoWorkspaceBase"/> class.
    /// </summary>
    /// <param name="roslynHost">The roslyn host.</param>
    /// <param name="staticReferences">The static references, i.e. project references that are not stated in the code by #r statements.</param>
    /// <param name="workingDirectory">The working directory. Is used only for script workspaces, otherwise, it can be null.</param>
    public AltaxoWorkspaceBase(RoslynHost roslynHost, IEnumerable<System.Reflection.Assembly> staticReferences, string workingDirectory)
        : this(
            roslynHost,
            staticReferences?.Select(ass => roslynHost.CreateMetadataReference(ass.Location)),
            workingDirectory)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AltaxoWorkspaceBase"/> class.
    /// </summary>
    /// <param name="roslynHost">The roslyn host.</param>
    /// <param name="staticReferences">The static references, i.e. project references that are not stated in the code by #r statements.</param>
    /// <param name="workingDirectory">The working directory. Is used only for script workspaces, otherwise, it can be null.</param>
    public AltaxoWorkspaceBase(RoslynHost roslynHost, IEnumerable<MetadataReference> staticReferences, string workingDirectory)
        : base(roslynHost.MefHost, WorkspaceKind.Host)
    {
      RoslynHost = roslynHost;
      WorkingDirectory = workingDirectory;
      StaticReferences = staticReferences.ToImmutableArray();
      PreprocessorSymbols = DefaultPreprocessorSymbols;
      ProjectId = CreateInitialProject();
    }

    protected override void Dispose(bool finalize)
    {
      DiagnosticProvider.Disable(this);
      _sourceTextChangedHandlers.Clear();

#if !NoDiagnostics
      _diagnosticsUpdatedNotifiers.Clear();
#endif
      base.Dispose(finalize);
    }

    /// <summary>
    /// Creates the initial project. Details depend on the nature of the workspace.
    /// </summary>
    /// <returns>The project id of the initial project that is created in this workspace.</returns>
    protected abstract ProjectId CreateInitialProject();

    /// <summary>
    /// Gets a compilation from the content of this workspace.
    /// </summary>
    /// <param name="assemblyName">Name of the assembly to create.</param>
    /// <returns></returns>
    public abstract Compilation GetCompilation(string assemblyName);

    public virtual Document CreateAndOpenDocument(SourceTextContainer textContainer, Action<SourceText> onTextUpdated)
    {
      var project = CurrentSolution.GetProject(ProjectId);
      var documentId = DocumentId.CreateNewId(ProjectId);
      var newSolution = project.Solution.AddDocument(documentId, project.Name, textContainer.CurrentText);
      SetCurrentSolution(newSolution);
      OpenDocument(documentId, textContainer);

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
      TryApplyChanges(document.Project.Solution);
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

    /// <summary>
    /// Closes the document.
    /// </summary>
    /// <param name="documentId">The document identifier.</param>
    public override void CloseDocument(DocumentId documentId)
    {
      base.CloseDocument(documentId);
      OnDocumentClosed(documentId, TextLoader.From(TextAndVersion.Create(CurrentSolution.GetDocument(documentId).GetTextAsync().Result, VersionStamp.Create())));
    }

#if !NoDiagnostics
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


#endif

    /// <summary>
    /// Must be overridden by script workspaces. Processes the reference directives. Searches the provided document for #r directives. Then it
    /// updates the <see cref="_referencesDirectives"/> dictionary. Directives no longer in the document are marked as passive
    /// in the reference dictionary, all that are there are marked active. If changes have occured, the project is updated
    /// to reference all active references.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <returns></returns>
    public virtual Task ProcessReferenceDirectives(Document document)
    {
      return Task.CompletedTask;
    }


    protected override void ApplyDocumentTextChanged(DocumentId document, SourceText newText)
    {
      if (_sourceTextChangedHandlers.TryGetValue(document, out var action) && null != action)
        action.Invoke(newText);

      OnDocumentTextChanged(document, newText, PreservationMode.PreserveIdentity);
    }

    /// <summary>
    /// Gets all references currently references by the project, i.e. the <see cref="StaticReferences"/> plus (only if it is a script workspace) the references
    /// referenced in the code by #r directives.
    /// </summary>
    /// <value>
    /// All references.
    /// </value>
    public virtual IEnumerable<MetadataReference> AllReferences
    {
      get
      {
        return StaticReferences;
      }
    }
  }
}
