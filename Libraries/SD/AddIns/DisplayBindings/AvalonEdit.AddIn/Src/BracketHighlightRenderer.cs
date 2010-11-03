﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.AvalonEdit.AddIn
{
	public class BracketHighlightRenderer : IBackgroundRenderer
	{
		BracketSearchResult result;
		Pen borderPen;
		Brush backgroundBrush;
		TextView textView;
		
		public void SetHighlight(BracketSearchResult result)
		{
			if (this.result != result) {
				this.result = result;
				textView.InvalidateLayer(this.Layer);
			}
		}
		
		public BracketHighlightRenderer(TextView textView)
		{
			if (textView == null)
				throw new ArgumentNullException("textView");
			
			this.borderPen = new Pen(new SolidColorBrush(Color.FromArgb(52, 0, 0, 255)), 1);
			this.borderPen.Freeze();
			
			this.backgroundBrush = new SolidColorBrush(Color.FromArgb(22, 0, 0, 255));
			this.backgroundBrush.Freeze();
			
			this.textView = textView;
			
			this.textView.BackgroundRenderers.Add(this);
		}
		
		public KnownLayer Layer {
			get {
				return KnownLayer.Selection;
			}
		}
		
		public void Draw(TextView textView, DrawingContext drawingContext)
		{
			if (this.result == null)
				return;
			
			BackgroundGeometryBuilder builder = new BackgroundGeometryBuilder();
			
			builder.CornerRadius = 1;
			builder.AlignToMiddleOfPixels = true;
			
			builder.AddSegment(textView, new TextSegment() { StartOffset = result.OpeningBracketOffset, Length = result.OpeningBracketLength });
			builder.CloseFigure(); // prevent connecting the two segments
			builder.AddSegment(textView, new TextSegment() { StartOffset = result.ClosingBracketOffset, Length = result.ClosingBracketLength });
			
			Geometry geometry = builder.CreateGeometry();
			if (geometry != null) {
				drawingContext.DrawGeometry(backgroundBrush, borderPen, geometry);
			}
		}
	}
}
