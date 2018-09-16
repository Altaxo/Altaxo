// Copyright Eli Arbel (no explicit copyright notice in original file), Apache License Version 2.0, January 2004

// Originated from: RoslynPad, RoslynPad.Editor.Windows, RoslynHighlightingColorizer.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    private ISemanticHighlightingColors _highlightingColors;

    public SemanticHighlightingColorizer(Workspace workspace, DocumentId documentId, ISemanticHighlightingColors highlightingColors = null)
    {
      _workspace = workspace;
      _documentId = documentId;
      _highlightingColors = highlightingColors ?? TextHighlightingColorsAltaxoStyle.Instance;
    }

    protected override IHighlighter CreateHighlighter(TextView textView, ICSharpCode.AvalonEdit.Document.TextDocument document)
    {
      return new SemanticHighlighter(_workspace, _documentId, document, _highlightingColors);
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
