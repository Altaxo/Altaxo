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

using Altaxo.CodeEditing.Folding;
using ICSharpCode.AvalonEdit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Altaxo.CodeEditing.QuickInfo;
using ICSharpCode.AvalonEdit.Folding;
using Altaxo.CodeEditing.BraceMatching;
using Altaxo.CodeEditing.Diagnostics;
using System.Collections.Immutable;
using Altaxo.CodeEditing.ReferenceHighlighting;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Formatting;
using ICSharpCode.AvalonEdit.Indentation;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Altaxo.CodeEditing.Completion;
using Altaxo.CodeEditing.LiveDocumentFormatting;

namespace Altaxo.CodeEditing
{
	/// <summary>
	/// Implementation of <see cref="ICodeEditorViewAdapter"/> for CSharp.
	/// </summary>
	/// <seealso cref="Altaxo.CodeEditing.ICodeEditorViewAdapter" />
	public class CodeEditorViewAdapterCSharp : ICodeEditorViewAdapter
	{
		protected RoslynHost _roslynHost;

		public AltaxoWorkspaceBase Workspace { get; }

		/// <summary>
		/// Gets or sets the document identifier of the underlying document.
		/// </summary>
		/// <value>
		/// The document identifier.
		/// </value>
		public Microsoft.CodeAnalysis.DocumentId DocumentId { get; }

		public RoslynSourceTextContainerAdapter SourceTextAdapter { get; }

		public QuickInfo.IQuickInfoProvider QuickInfoProvider { get; set; }

		public SyntaxTreeFoldingStrategy FoldingStrategy { get; set; }

		public IBraceMatchingService BraceMatchingService { get; set; }

		public IDocumentHighlightsService ReferenceHighlightService { get; set; }

		public ICodeEditorCompletionProvider CompletionProvider { get; set; }

		public Renaming.IRenamingService RenamingService { get; set; }

		public IIndentationStrategy IndentationStrategy { get; set; }

		public ILiveDocumentFormatter LiveDocumentFormatter { get; set; }

		/// <summary>
		/// Responsible for getting an <see cref="ExternalHelp.ExternalHelpItem"/> from the symbol under the caret position.
		/// If this is successful, the event <see cref="ExternalHelpRequired"/> is fired.
		/// </summary>
		/// <value>
		/// The external help provider.
		/// </value>
		public ExternalHelp.IExternalHelpProvider ExternalHelpProvider { get; set; }

		/// <summary>
		/// Event that is fired when external help is required.
		/// </summary>
		public event Action<ExternalHelp.ExternalHelpItem> ExternalHelpRequired;

		public CodeEditorViewAdapterCSharp(AltaxoWorkspaceBase workspace, Microsoft.CodeAnalysis.DocumentId documentID, RoslynSourceTextContainerAdapter sourceText)
		{
			Workspace = workspace;
			_roslynHost = workspace.RoslynHost;
			DocumentId = documentID;
			SourceTextAdapter = sourceText;

			QuickInfoProvider = _roslynHost.GetService<QuickInfo.IQuickInfoProvider>();
			FoldingStrategy = new SyntaxTreeFoldingStrategy();
			BraceMatchingService = _roslynHost.GetService<IBraceMatchingService>();
			ReferenceHighlightService = new ReferenceHighlighting.CSharp.CSharpDocumentHighlightsService();
			CompletionProvider = new Completion.CodeEditorCompletionProvider(_roslynHost, Workspace, DocumentId);
			RenamingService = new Renaming.RenamingService();
			IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy();
			LiveDocumentFormatter = new LiveDocumentFormatterCSharp();
			ExternalHelpProvider = new ExternalHelp.ExternalHelpProvider();

			Workspace.SubscribeToDiagnosticsUpdateNotification(DocumentId, EhDiagnosticsUpdated);
		}

		#region Syntax highlighting

		/// <summary>
		/// Gets the highlighting service.
		/// </summary>
		/// <value>
		/// The highlighting service.
		/// </value>
		public ICSharpCode.AvalonEdit.Highlighting.IHighlightingDefinition HighlightingService { get; set; } = ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinitionByExtension(".cs");

		#endregion Syntax highlighting

		#region QuickInfo

		public virtual async Task<QuickInfo.QuickInfoItem> GetToolTipAsync(int cursorPosition)
		{
			// TODO: consider invoking this with a delay, then showing the tool-tip without one
			var document = Workspace.CurrentSolution.GetDocument(DocumentId);
			var quickInfoProvider = QuickInfoProvider;
			if (null != quickInfoProvider)
			{
				return await quickInfoProvider.GetItemAsync(document, cursorPosition, CancellationToken.None).ConfigureAwait(true);
			}
			else
			{
				return null;
			}
		}

		#endregion QuickInfo

		#region Folding

		/// <summary>
		/// Central routine of the folding strategy. Uses the document's syntax tree
		/// to calculate all folding positions.
		/// </summary>
		/// <returns>
		/// Enumeration of foldings.
		/// </returns>
		public IEnumerable<NewFolding> GetNewFoldings()
		{
			var document = Workspace.CurrentSolution.GetDocument(DocumentId);
			//var text = document.GetTextAsync().Result;

			var syntaxTree = document.GetSyntaxTreeAsync().Result;
			return FoldingStrategy?.GetNewFoldings(syntaxTree);
		}

		#endregion Folding

		#region Brace matching

		/// <summary>
		/// Gets the matching braces asynchronously.
		/// </summary>
		/// <param name="position">The cursor position.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>Matching braces</returns>
		public virtual async Task<BraceMatchingResult?> GetMatchingBracesAsync(int position, CancellationToken cancellationToken = default(CancellationToken))
		{
			var service = BraceMatchingService;

			if (null != service)
			{
				return await service.GetMatchingBracesAsync(Workspace.CurrentSolution.GetDocument(DocumentId), position, cancellationToken);
			}
			else
			{
				return null;
			}
		}

		#endregion Brace matching

		#region Diagnostics

		/// <summary>
		/// Occurs when the diagnostics was updated and new diagnostics is available (diagnostics is responsible for the wriggles under the text
		/// that show in advance the errors in code).
		/// </summary>
		public event Action<DiagnosticsUpdatedArgs> DiagnosticsUpdated;

		/// <summary>
		/// Called from the roslyn host when diagnostics was updated.
		/// </summary>
		/// <param name="a">a.</param>
		public void EhDiagnosticsUpdated(DiagnosticsUpdatedArgs a)
		{
			DiagnosticsUpdated?.Invoke(a);
		}

		#endregion Diagnostics

		#region Reference Highlighting

		/// <summary>
		/// Finds references to resolved expression in the current file.
		/// </summary>
		public virtual async Task<ImmutableArray<DocumentHighlights>> FindReferencesInCurrentFile(int cursorPosition)
		{
			var service = ReferenceHighlightService;

			if (null == service)
				return ImmutableArray<DocumentHighlights>.Empty;

			var document = Workspace.CurrentSolution.GetDocument(DocumentId);

			var builder = ImmutableHashSet<Document>.Empty.ToBuilder();
			builder.Add(document);

			return await service.GetDocumentHighlightsAsync(document, cursorPosition, builder.ToImmutable(), CancellationToken.None);
		}

		#endregion Reference Highlighting

		#region Completion

		public async Task<CompletionResult> GetCompletionData(int position, char? triggerChar, bool useSignatureHelp)
		{
			return await CompletionProvider?.GetCompletionData(position, triggerChar, useSignatureHelp);
		}

		#endregion Completion

		#region Symbol renaming

		/// <summary>
		/// Renames the symbol at the caret position or the start of the selection.
		/// </summary>
		/// <param name="caretPositionOrSelectionStart">The caret position or selection start.</param>
		/// <returns></returns>
		public async Task RenameSymbol(int caretPositionOrSelectionStart, object topLevelWindow, Action FocusBackOnEditor)
		{
			var service = RenamingService;

			if (null != service)
			{
				await service.RenameSymbol(Workspace, DocumentId, SourceTextAdapter, caretPositionOrSelectionStart, topLevelWindow, FocusBackOnEditor).ConfigureAwait(false);
			}
		}

		#endregion Symbol renaming

		#region QuickClassBrowser

		/// <summary>
		/// Finds references to resolved expression in the current file.
		/// </summary>
		public virtual async Task<SyntaxTree> GetDocumentSyntaxTreeAsync()
		{
			var document = Workspace.CurrentSolution.GetDocument(DocumentId);
			return await document.GetSyntaxTreeAsync();
		}

		#endregion QuickClassBrowser

		#region Document formatting

		public async Task FormatDocument()
		{
			var document = Workspace.CurrentSolution.GetDocument(this.DocumentId);

			var syntaxTree = await document.GetSyntaxRootAsync();
			var textChanges = await Formatter.GetFormattedTextChangesAsync(syntaxTree, Workspace);
			SourceTextAdapter.ApplyTextChanges(textChanges, (modifiedSourceText) => Workspace.TryApplyChanges(document.WithText(modifiedSourceText).Project.Solution));
		}

		/// <summary>
		/// Formats the document after entering a trigger character. Trigger chars are e.g. closing curly brace (then format whole paragraph)
		/// or semicolon (then format line).
		/// </summary>
		/// <param name="caretPosition">The caret position after (!) the trigger char.</param>
		/// <returns></returns>
		public async Task FormatDocumentAfterEnteringTriggerChar(int caretPosition, char triggerChar)
		{
			var formatter = LiveDocumentFormatter;

			if (null != formatter)
			{
				await formatter.FormatDocumentAfterEnteringTriggerChar(Workspace, DocumentId, SourceTextAdapter, caretPosition, triggerChar).ConfigureAwait(false);
			}
		}

		#endregion Document formatting

		#region External Help

		/// <summary>
		/// Gets the external help item for the symbol under the caret position.
		/// </summary>
		/// <param name="caretPosition">The caret position.</param>
		/// <returns>The external help item.</returns>
		public async Task GetExternalHelpItemAndFireHelpEvent(int caretPosition)
		{
			var helpProvider = ExternalHelpProvider;

			if (null != helpProvider) // && null != ExternalHelpRequired)
			{
				var document = Workspace.CurrentSolution.GetDocument(DocumentId);
				var helpItem = await helpProvider.GetExternalHelpItem(document, caretPosition, CancellationToken.None).ConfigureAwait(false);
				if (null != helpItem)
				{
					ExternalHelpRequired?.Invoke(helpItem);
				}
			}
		}

		#endregion External Help
	}
}