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

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Altaxo.CodeEditing.BraceMatching;
using Altaxo.CodeEditing.Diagnostics;
using Altaxo.CodeEditing.ReferenceHighlighting;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Indentation;
using Microsoft.CodeAnalysis;

namespace Altaxo.CodeEditing
{
  /// <summary>
  /// Provides all the higher level functions, as highlighting, completion, tooltips, folding, brace matching, formatting etc. to the <see cref="Gui.CodeEditing.CodeEditorView"/>.
  /// </summary>
  public interface ICodeEditorViewAdapter
  {
    /// <summary>
    /// Gets the workspace where the document (see <see cref="DocumentId"/> is contained.
    /// </summary>
    /// <value>
    /// The workspace.
    /// </value>
    AltaxoWorkspaceBase Workspace { get; }

    /// <summary>
    /// Gets the document identifier of the underlying document.
    /// </summary>
    /// <value>
    /// The document identifier.
    /// </value>
    Microsoft.CodeAnalysis.DocumentId DocumentId { get; }

    /*
		/// <summary>
		/// Gets the highlighting service.
		/// </summary>
		/// <value>
		/// The highlighting service.
		/// </value>
		ICSharpCode.AvalonEdit.Highlighting.IHighlightingDefinition HighlightingService { get; }
		*/

    /// <summary>
    /// Gets the highlighting colorizer to colorize the code.
    /// </summary>
    /// <value>
    /// The highlighting colorizer.
    /// </value>
    ICSharpCode.AvalonEdit.Highlighting.HighlightingColorizer HighlightingColorizer { get; }

    /// <summary>
    /// Gets the quick info tool tip.
    /// </summary>
    /// <param name="cursorPosition">The cursor position.</param>
    /// <returns>The quick info tool tip, or null.</returns>
    Task<QuickInfo.QuickInfoItem> GetToolTipAsync(int cursorPosition);

    /// <summary>
    /// Central routine of the folding strategy. Uses the document's syntax tree
    /// to calculate all folding positions.
    /// </summary>
    /// <returns>Enumeration of foldings.</returns>
    IEnumerable<NewFolding> GetNewFoldings();

    /// <summary>
    /// Gets the matching braces asynchronously.
    /// </summary>
    /// <param name="position">The cursor position.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Matching braces</returns>
    Task<BraceMatchingResult?> GetMatchingBracesAsync(int position, CancellationToken cancellationToken = default(CancellationToken));

    /// <summary>
    /// Occurs when the diagnostics was updated and new diagnostics is available (diagnostics is responsible for the wriggles under the text
    /// that show in advance the errors in code).
    /// </summary>
    event Action<DiagnosticsUpdatedArgs> DiagnosticsUpdated;

    /// <summary>
    /// Occurs after the source text has changed. This event is routed from the <see cref="SourceTextAdapter"/>.
    /// </summary>
    event EventHandler<Microsoft.CodeAnalysis.Text.TextChangeEventArgs> SourceTextChanged;

    /// <summary>
    /// Finds references to resolved expression in the current file.
    /// </summary>
    Task<ImmutableArray<DocumentHighlights>> FindReferencesInCurrentFile(int cursorPosition);

    Task<CompletionResult> GetCompletionData(int position, char? triggerChar, bool useSignatureHelp);

    /// <summary>
    /// Gets the indentation strategy.
    /// </summary>
    /// <value>
    /// The indentation strategy.
    /// </value>
    IIndentationStrategy IndentationStrategy { get; }

    /// <summary>
    /// Gets the syntax tree of the document.
    /// </summary>
    Task<SyntaxTree> GetDocumentSyntaxTreeAsync();

    /// <summary>
    /// Formats the complete document.
    /// </summary>
    /// <returns></returns>
    Task FormatDocument();

    /// <summary>
    /// Formats the document after entering a trigger character. Trigger chars are e.g. closing curly brace (then format whole paragraph)
    /// or semicolon (then format line).
    /// </summary>
    /// <param name="caretPosition">The caret position after (!) the trigger char.</param>
    /// <param name="triggerChar">The trigger char.</param>
    /// <returns></returns>
    Task FormatDocumentAfterEnteringTriggerChar(int caretPosition, char triggerChar);

    /// <summary>
    /// Gets the external help item for the symbol under the caret position, and then fires the <see cref="ExternalHelpRequired"/> event.
    /// </summary>
    /// <param name="caretPosition">The caret position.</param>
    Task GetExternalHelpItemAndFireHelpEvent(int caretPosition);

    /// <summary>
    /// Event that is fired when external help is required.
    /// </summary>
    event Action<ExternalHelp.ExternalHelpItem> ExternalHelpRequired;

    /// <summary>
    /// Renames the symbol at the caret position or the start of the selection.
    /// </summary>
    /// <param name="caretPositionOrSelectionStart">The caret position or selection start.</param>
    /// <returns></returns>
    Task RenameSymbol(int caretPositionOrSelectionStart, object topLevelWindow, Action FocusBackOnEditor);

    /// <summary>
    /// Try to go to the definition of the symbol under the caret. This function is designed here for solutions
    /// containing only of a single code document.
    /// </summary>
    /// <param name="caretOffset">The caret offset.</param>
    /// <returns>The position of the symbol in the document where it is defined. If the symbol under the caret is not defined in the document,
    /// the return value is null.</returns>
    int? GoToDefinition(int caretOffset);
  }
}
