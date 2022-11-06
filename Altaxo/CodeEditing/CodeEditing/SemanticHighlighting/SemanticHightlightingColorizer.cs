// Copyright Eli Arbel (no explicit copyright notice in original file), Apache License Version 2.0, January 2004

// Originated from: RoslynPad, RoslynPad.Editor.Windows, RoslynHighlightingColorizer.cs

#if !NoSemanticHighlighting

using System;
using Altaxo.Gui.CodeEditing.SemanticHighlighting;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using Microsoft.CodeAnalysis;

namespace Altaxo.CodeEditing.SemanticHighlighting
{
  public class SemanticHighlightingColorizer : HighlightingColorizer
  {
    private readonly Workspace _workspace;
    private readonly DocumentId _documentId;
    private readonly CodeEditorViewAdapterCSharp _adapter;

    private ISemanticHighlightingColors _highlightingColors;

    public SemanticHighlightingColorizer(CodeEditorViewAdapterCSharp adapter, Workspace workspace, DocumentId documentId, ISemanticHighlightingColors highlightingColors = null)
    {
      _workspace = workspace;
      _documentId = documentId;
      _adapter = adapter;
      _highlightingColors = highlightingColors ?? TextHighlightingColorsAltaxoStyle.Instance;
    }

    protected override IHighlighter CreateHighlighter(TextView textView, ICSharpCode.AvalonEdit.Document.TextDocument document)
    {
      return new SemanticHighlighter(_adapter, _workspace, _documentId, document, _highlightingColors);
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
  }
}
#endif
