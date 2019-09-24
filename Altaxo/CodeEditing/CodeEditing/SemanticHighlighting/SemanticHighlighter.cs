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
using System.Threading;
using System.Threading.Tasks;
using Altaxo.Gui.CodeEditing.SemanticHighlighting;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.Text;

namespace Altaxo.CodeEditing.SemanticHighlighting
{
  /// <summary>
  /// Probably the simplest version of a semantic highlighter using Roslyn's <see cref="Classifier"/>.
  /// The hightlighting is done in the Gui thread. Caching is enabled, but used mostly while scrolling the document.
  /// </summary>
  /// <seealso cref="ICSharpCode.AvalonEdit.Highlighting.IHighlighter" />
  public class SemanticHighlighter : IHighlighter
  {
    private Workspace _workspace;
    private readonly IDocument _avalonEditTextDocument;
    private readonly DocumentId _documentId;
    private readonly CodeEditorViewAdapterCSharp _adapter;
    private ISemanticHighlightingColors _highlightingColors;

    /// <summary>
    /// The cached lines with highlighting information. Key is the line number, Value is the highlighted line.
    /// </summary>
    private readonly Dictionary<int, CachedLine> _cachedLines = new Dictionary<int, CachedLine>();

    /// <summary>
    /// Initializes a new instance of the <see cref="SemanticHighlighter" /> class.
    /// </summary>
    /// <param name="workspace">The workspace containing the document to highlight.</param>
    /// <param name="documentId">The document identifier.</param>
    /// <param name="avalonEditTextDocument">The corresponsing avalon edit text document.</param>
    public SemanticHighlighter(CodeEditorViewAdapterCSharp adapter, Workspace workspace, DocumentId documentId, IDocument avalonEditTextDocument, ISemanticHighlightingColors highlightingColors = null)
    {
      _workspace = workspace ?? throw new ArgumentNullException(nameof(workspace));
      _documentId = documentId ?? throw new ArgumentNullException(nameof(documentId));
      _adapter = adapter;
      _avalonEditTextDocument = avalonEditTextDocument ?? throw new ArgumentNullException(nameof(avalonEditTextDocument));
      _highlightingColors = highlightingColors ?? TextHighlightingColorsAltaxoStyle.Instance;
    }

    public ISemanticHighlightingColors HighlightingColors
    {
      get
      {
        return _highlightingColors;
      }
      set
      {
        _highlightingColors = value ?? throw new ArgumentNullException(nameof(value));
      }
    }

    #region IHighlighter interface

    /// <summary>
    /// Notification when the highlighter detects that the highlighting state at the
    /// <b>beginning</b> of the specified lines has changed.
    /// <c>fromLineNumber</c> and <c>toLineNumber</c> are both inclusive;
    /// the common case of a single-line change is represented by <c>fromLineNumber == toLineNumber</c>.
    ///
    /// During highlighting, the highlighting of line X will cause this event to be raised
    /// for line X+1 if the highlighting state at the end of line X has changed from its previous state.
    /// This event may also be raised outside of the highlighting process to signalize that
    /// changes to external data (not the document text; but e.g. semantic information)
    /// require a re-highlighting of the specified lines.
    /// </summary>
    /// <remarks>
    /// For implementers: there is the requirement that, during highlighting,
    /// if there was no state changed reported for the beginning of line X,
    /// and there were no document changes between the start of line X and the start of line Y (with Y > X),
    /// then this event must not be raised for any line between X and Y (inclusive).
    ///
    /// Equal input state + unchanged line = Equal output state.
    ///
    /// See the comment in the HighlightingColorizer.OnHighlightStateChanged implementation
    /// for details about the requirements for a correct custom IHighlighter.
    ///
    /// Outside of the highlighting process, this event can be raised without such restrictions.
    /// </remarks>
    public event HighlightingStateChangedEventHandler HighlightingStateChanged;

    /// <summary>
    /// Gets the underlying text document.
    /// </summary>
    public IDocument Document
    {
      get
      {
        return _avalonEditTextDocument;
      }
    }

    /// <summary>
    /// Gets the default color of the text.
    /// </summary>
    /// <value>
    /// The default color of the text.
    /// </value>
    public HighlightingColor DefaultTextColor
    {
      get
      {
        return _highlightingColors.DefaultColor;
      }
    }

    /// <summary>
    /// Begins the highlighting.
    /// </summary>
    public void BeginHighlighting()
    {
    }

    /// <summary>
    /// Ends the highlighting.
    /// </summary>
    public void EndHighlighting()
    {
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    public void Dispose()
    {
      HighlightingStateChanged = null;
    }

    /// <summary>
    /// Gets the stack of active colors (the colors associated with the active spans) at the end of the specified line.
    /// -> GetColorStack(1) returns the colors at the start of the second line.
    /// </summary>
    /// <remarks>
    /// GetColorStack(0) is valid and will return the empty stack.
    /// The elements are returned in inside-out order (first element of result enumerable is the color of the innermost span).
    /// </remarks>
    public IEnumerable<HighlightingColor> GetColorStack(int lineNumber)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Gets the color of the named.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns></returns>
    public HighlightingColor GetNamedColor(string name)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Highlights the specified document line.
    /// </summary>
    /// <param name="lineNumber">The line to highlight.</param>
    /// <returns>A <see cref="HighlightedLine"/> line object that represents the highlighted sections.</returns>
    public HighlightedLine HighlightLine(int lineNumber)
    {
      var documentLine = _avalonEditTextDocument.GetLineByNumber(lineNumber);
      var currentDocumentVersion = _avalonEditTextDocument.Version;

      // line properties to evaluate in Gui context
      var documentTextLength = _avalonEditTextDocument.TextLength;
      var offset = documentLine.Offset;
      var totalLength = documentLine.TotalLength;
      var endOffset = documentLine.EndOffset;

      if (_cachedLines.TryGetValue(lineNumber, out var cachedLine) && cachedLine.OldVersion == currentDocumentVersion)
      {
        //System.Diagnostics.Debug.WriteLine("SemanticHightlighter2 Line[{0}] from cache.", lineNumber);
        return cachedLine.HighlightedLine; // old info is still valid, thus we return it
      }
      else
      {
        var highlightedLine = new HighlightedLine(_avalonEditTextDocument, documentLine);

        var lastSemanticModel = _adapter.LastSemanticModel.SemanticModel; // most of the time we will use the semantic model created in the background
        if (lastSemanticModel is null) // but particularly at the beginning, the semantic model might not be created already
        {
          var document = _workspace.CurrentSolution.GetDocument(_documentId);
          lastSemanticModel = document.GetSemanticModelAsync().Result; // then we can not help it, we must wait for it! 
        }

        if (documentTextLength >= offset + totalLength)
        {
          var classifiedSpans = Classifier.GetClassifiedSpans(
              lastSemanticModel,
              new TextSpan(offset, totalLength),
              _workspace);

          foreach (var classifiedSpan in classifiedSpans)
          {
            if (IsSpanIntersectingDocumentLine(classifiedSpan, offset, endOffset, out var startOfIntersection, out var lengthOfIntersection))
            {
              highlightedLine.Sections.Add(new HighlightedSection
              {
                Color = _highlightingColors.GetColor(classifiedSpan.ClassificationType),
                Offset = startOfIntersection,
                Length = lengthOfIntersection
              });
            }
          }

          _cachedLines[lineNumber] = new CachedLine(highlightedLine, currentDocumentVersion);

        }
        return highlightedLine;
      }
    }

    private static bool IsSpanIntersectingDocumentLine(ClassifiedSpan classifiedSpan, int documentLineOffset, int documentLineEndOffset, out int startOfIntersection, out int lengthOfIntersection)
    {
      startOfIntersection = Math.Max(documentLineOffset, classifiedSpan.TextSpan.Start);
      var endOfIntersection = Math.Min(documentLineEndOffset, classifiedSpan.TextSpan.End);
      lengthOfIntersection = endOfIntersection - startOfIntersection;
      return lengthOfIntersection > 0;
    }

    /// <summary>
    /// Enforces a highlighting state update (triggering the HighlightingStateChanged event if necessary)
    /// for all lines up to (and inclusive) the specified line number.
    /// </summary>
    public void UpdateHighlightingState(int lineNumber)
    {
      if (lineNumber > 0)
      {
        var cnt = Math.Min(_cachedLines.Count - 1, lineNumber);
        for (int i = 0; i <= cnt; ++i)
          _cachedLines.Remove(i);

        HighlightingStateChanged?.Invoke(1, lineNumber);
      }
    }

    #endregion IHighlighter interface

    #region Caching

    // If a line gets edited and we need to display it while no parse information is ready for the
    // changed file, the line would flicker (semantic highlightings disappear temporarily).
    // We avoid this issue by storing the semantic highlightings and updating them on document changes
    // (using anchor movement)
    private class CachedLine
    {
      public readonly HighlightedLine HighlightedLine;
      public readonly ITextSourceVersion OldVersion;

      public CachedLine(HighlightedLine highlightedLine, ITextSourceVersion fileVersion)
      {
        HighlightedLine = highlightedLine ?? throw new ArgumentNullException(nameof(highlightedLine));
        OldVersion = fileVersion ?? throw new ArgumentNullException(nameof(fileVersion));
      }
    }

    #endregion Caching
  }
}
