// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

// Originated from: SharpDevelop, AvalonEdit.Addin, Src/BracketHighlightRenderer.cs

#if !NoBraceMatching

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Media;
using Altaxo.CodeEditing.BraceMatching;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace Altaxo.Gui.CodeEditing.BraceMatching
{
  public class BracketHighlightRenderer : IBackgroundRenderer
  {
    private BraceMatchingResult? result;
    private Pen borderPen;
    private Brush backgroundBrush;
    private TextView textView;

    public static readonly Color DefaultBackground = Color.FromArgb(22, 0, 0, 255);
    public static readonly Color DefaultBorder = Color.FromArgb(52, 0, 0, 255);

    public const string BracketHighlight = "Bracket highlight";

    public void SetHighlight(BraceMatchingResult? result)
    {
      if (!object.Equals(this.result, result))
      {
        this.result = result;
        textView.InvalidateLayer(Layer);
      }
    }

    public BracketHighlightRenderer(TextView textView)
    {
      this.textView = textView ?? throw new ArgumentNullException(nameof(textView));
      this.textView.BackgroundRenderers.Add(this);
      UpdateColors(DefaultBackground, DefaultBorder);
    }

    private void UpdateColors(Color background, Color foreground)
    {
      borderPen = new Pen(new SolidColorBrush(foreground), 1);
      borderPen.Freeze();

      backgroundBrush = new SolidColorBrush(background);
      backgroundBrush.Freeze();
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
      if (result.HasValue)
      {
        var match = result.Value;

        var builder = new BackgroundGeometryBuilder
        {
          CornerRadius = 1,
        };

        builder.AddSegment(textView, new TextSegment() { StartOffset = match.LeftSpan.Start, Length = match.LeftSpan.Length });
        builder.CloseFigure(); // prevent connecting the two segments
        builder.AddSegment(textView, new TextSegment() { StartOffset = match.RightSpan.Start, Length = match.RightSpan.Length });

        Geometry geometry = builder.CreateGeometry();
        if (geometry != null)
        {
          drawingContext.DrawGeometry(backgroundBrush, borderPen, geometry);
        }
      }
    }

    /*

		public static void ApplyCustomizationsToRendering(BracketHighlightRenderer renderer, IEnumerable<CustomizedHighlightingColor> customizations)
		{
			renderer.UpdateColors(DefaultBackground, DefaultBorder);
			foreach (CustomizedHighlightingColor color in customizations)
			{
				if (color.Name == BracketHighlight)
				{
					renderer.UpdateColors(color.Background ?? Colors.Blue, color.Foreground ?? Colors.Blue);
					// 'break;' is necessary because more specific customizations come first in the list
					// (language-specific customizations are first, followed by 'all languages' customizations)
					break;
				}
			}
		}
		*/
  }
}
#endif
