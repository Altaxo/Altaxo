// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

// Originated from: SharpDevelop, AvalonEdit.Addin, Src/BracketHighlightRenderer.cs

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Windows.Media;
using Altaxo.CodeEditing.ReferenceHighlighting;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace Altaxo.Gui.CodeEditing.ReferenceHightlighting
{
  /// <summary>
  /// Highlights expressions (references to expression under current caret).
  /// </summary>
  public class ExpressionHighlightRenderer : IBackgroundRenderer
  {
    private ImmutableArray<DocumentHighlights> renderedReferences;
    private Pen borderPen;
    private Brush backgroundBrush;
    private TextView textView;
    private readonly Color borderColor = Color.FromArgb(52, 30, 130, 255);  //Color.FromArgb(180, 70, 230, 70))
    private readonly Color fillColor = Color.FromArgb(22, 30, 130, 255);  //Color.FromArgb(40, 60, 255, 60)
    private readonly int borderThickness = 1;
    private readonly int cornerRadius = 1;

    public void SetHighlight(ImmutableArray<DocumentHighlights> renderedReferences)
    {
      if (this.renderedReferences != renderedReferences)
      {
        this.renderedReferences = renderedReferences;
        textView.InvalidateLayer(Layer);
      }
    }

    public void ClearHighlight()
    {
      SetHighlight(ImmutableArray<DocumentHighlights>.Empty);
    }

    public ExpressionHighlightRenderer(TextView textView)
    {
      if (textView == null)
        throw new ArgumentNullException(nameof(textView));
      this.textView = textView;
      borderPen = new Pen(new SolidColorBrush(borderColor), borderThickness);
      backgroundBrush = new SolidColorBrush(fillColor);
      borderPen.Freeze();
      backgroundBrush.Freeze();
      this.textView.BackgroundRenderers.Add(this);
    }

    public KnownLayer Layer
    {
      get
      {
        return KnownLayer.Selection;
      }
    }

    public void Draw(TextView textView, DrawingContext drawingContext)
    {
      if (renderedReferences == null)
        return;
      var builder = new BackgroundGeometryBuilder
      {
        CornerRadius = cornerRadius,
        AlignToMiddleOfPixels = true
      };
      foreach (var reference in renderedReferences)
      {
        foreach (var span in reference.HighlightSpans)
        {
          builder.AddSegment(textView, new TextSegment()
          {
            StartOffset = span.TextSpan.Start,
            Length = span.TextSpan.Length
          });
          builder.CloseFigure();
        }
      }
      Geometry geometry = builder.CreateGeometry();
      if (geometry != null)
      {
        drawingContext.DrawGeometry(backgroundBrush, borderPen, geometry);
      }
    }
  }
}
