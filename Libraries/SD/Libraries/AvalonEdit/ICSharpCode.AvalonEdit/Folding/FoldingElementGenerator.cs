﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Folding
{
	/// <summary>
	/// A <see cref="VisualLineElementGenerator"/> that produces line elements for folded <see cref="FoldingSection"/>s.
	/// </summary>
	public class FoldingElementGenerator : VisualLineElementGenerator
	{
		/// <summary>
		/// Gets/Sets the folding manager from which the foldings should be shown.
		/// </summary>
		public FoldingManager FoldingManager { get; set; }
		
		/// <inheritdoc/>
		public override void StartGeneration(ITextRunConstructionContext context)
		{
			base.StartGeneration(context);
			if (FoldingManager != null) {
				if (context.TextView != FoldingManager.textView)
					throw new ArgumentException("Invalid TextView");
				if (context.Document != FoldingManager.document)
					throw new ArgumentException("Invalid document");
			}
		}
		
		/// <inheritdoc/>
		public override int GetFirstInterestedOffset(int startOffset)
		{
			if (FoldingManager != null)
				return FoldingManager.GetNextFoldedFoldingStart(startOffset);
			else
				return -1;
		}
		
		/// <inheritdoc/>
		public override VisualLineElement ConstructElement(int offset)
		{
			if (FoldingManager == null)
				return null;
			int foldedUntil = -1;
			FoldingSection foldingSection = null;
			foreach (FoldingSection fs in FoldingManager.GetFoldingsAt(offset)) {
				if (fs.IsFolded) {
					if (fs.EndOffset > foldedUntil) {
						foldedUntil = fs.EndOffset;
						foldingSection = fs;
					}
				}
			}
			if (foldedUntil > offset && foldingSection != null) {
				string title = foldingSection.Title;
				if (string.IsNullOrEmpty(title))
					title = "...";
				var p = new VisualLineElementTextRunProperties(CurrentContext.GlobalTextRunProperties);
				p.SetForegroundBrush(Brushes.Gray);
				var textFormatter = TextFormatterFactory.Create(CurrentContext.TextView);
				var text = FormattedTextElement.PrepareText(textFormatter, title, p);
				return new FoldingLineElement(foldingSection, text, foldedUntil - offset);
			} else {
				return null;
			}
		}
		
		sealed class FoldingLineElement : FormattedTextElement
		{
			readonly FoldingSection fs;
			
			public FoldingLineElement(FoldingSection fs, TextLine text, int documentLength) : base(text, documentLength)
			{
				this.fs = fs;
			}
			
			public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
			{
				return new FoldingLineTextRun(this, this.TextRunProperties);
			}
			
			protected internal override void OnMouseDown(MouseButtonEventArgs e)
			{
				if (e.ClickCount == 2 && e.ChangedButton == MouseButton.Left) {
					fs.IsFolded = false;
					e.Handled = true;
				} else {
					base.OnMouseDown(e);
				}
			}
		}
		
		sealed class FoldingLineTextRun : FormattedTextRun
		{
			public FoldingLineTextRun(FormattedTextElement element, TextRunProperties properties)
				: base(element, properties)
			{
			}
			
			public override void Draw(DrawingContext drawingContext, Point origin, bool rightToLeft, bool sideways)
			{
				var metrics = Format(double.PositiveInfinity);
				Rect r = new Rect(origin.X, origin.Y - metrics.Baseline, metrics.Width, metrics.Height);
				drawingContext.DrawRectangle(null, new Pen(Brushes.Gray, 1), r);
				base.Draw(drawingContext, origin, rightToLeft, sideways);
			}
		}
	}
}
