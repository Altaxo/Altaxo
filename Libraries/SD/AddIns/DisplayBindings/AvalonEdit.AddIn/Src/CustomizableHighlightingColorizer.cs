﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Applies a set of customizations to syntax highlighting.
	/// </summary>
	public class CustomizableHighlightingColorizer : HighlightingColorizer
	{
		public const string DefaultTextAndBackground = "Default text/background";
		public const string SelectedText = "Selected text";
		public const string NonPrintableCharacters = "Non-printable characters";
		public const string LineNumbers = "Line numbers";
		
		public static void ApplyCustomizationsToDefaultElements(TextEditor textEditor, IEnumerable<CustomizedHighlightingColor> customizations)
		{
			textEditor.ClearValue(TextEditor.BackgroundProperty);
			textEditor.ClearValue(TextEditor.ForegroundProperty);
			textEditor.ClearValue(TextEditor.LineNumbersForegroundProperty);
			textEditor.TextArea.ClearValue(TextArea.SelectionBorderProperty);
			textEditor.TextArea.ClearValue(TextArea.SelectionBrushProperty);
			textEditor.TextArea.ClearValue(TextArea.SelectionForegroundProperty);
			textEditor.TextArea.TextView.ClearValue(TextView.NonPrintableCharacterBrushProperty);
			
			// 'assigned' flags are used so that the first matching customization wins.
			// This is necessary because more specific customizations come first in the list
			// (language-specific customizations are first, followed by 'all languages' customizations)
			bool assignedDefaultText = false;
			bool assignedSelectedText = false;
			bool assignedNonPrintableCharacter = false;
			bool assignedLineNumbers = false;
			foreach (CustomizedHighlightingColor color in customizations) {
				switch (color.Name) {
					case DefaultTextAndBackground:
						if (assignedDefaultText)
							continue;
						assignedDefaultText = true;
						
						if (color.Background != null)
							textEditor.Background = CreateFrozenBrush(color.Background.Value);
						if (color.Foreground != null)
							textEditor.Foreground = CreateFrozenBrush(color.Foreground.Value);
						break;
					case SelectedText:
						if (assignedSelectedText)
							continue;
						assignedSelectedText = true;
						
						if (color.Background != null) {
							Pen pen = new Pen(CreateFrozenBrush(color.Background.Value), 1);
							pen.Freeze();
							textEditor.TextArea.SelectionBorder = pen;
							SolidColorBrush back = new SolidColorBrush(color.Background.Value);
							back.Opacity = 0.7; // TODO : remove this constant, let the use choose the opacity.
							back.Freeze();
							textEditor.TextArea.SelectionBrush = back;
						}
						if (color.Foreground != null) {
							textEditor.TextArea.SelectionForeground = CreateFrozenBrush(color.Foreground.Value);
						}
						break;
					case NonPrintableCharacters:
						if (assignedNonPrintableCharacter)
							continue;
						assignedNonPrintableCharacter = true;
						
						if (color.Foreground != null)
							textEditor.TextArea.TextView.NonPrintableCharacterBrush = CreateFrozenBrush(color.Foreground.Value);
						break;
					case LineNumbers:
						if (assignedLineNumbers)
							continue;
						assignedLineNumbers = true;
						
						if (color.Foreground != null)
							textEditor.LineNumbersForeground = CreateFrozenBrush(color.Foreground.Value);
						break;
				}
			}
		}
		
		readonly IEnumerable<CustomizedHighlightingColor> customizations;
		
		public CustomizableHighlightingColorizer(HighlightingRuleSet ruleSet, IEnumerable<CustomizedHighlightingColor> customizations)
			: base(ruleSet)
		{
			if (customizations == null)
				throw new ArgumentNullException("customizations");
			this.customizations = customizations;
		}
		
		protected override IHighlighter CreateHighlighter(TextView textView, TextDocument document)
		{
			return new CustomizingHighlighter(customizations, base.CreateHighlighter(textView, document));
		}
		
		sealed class CustomizingHighlighter : IHighlighter
		{
			readonly IEnumerable<CustomizedHighlightingColor> customizations;
			readonly IHighlighter baseHighlighter;
			
			public CustomizingHighlighter(IEnumerable<CustomizedHighlightingColor> customizations, IHighlighter baseHighlighter)
			{
				Debug.Assert(customizations != null);
				this.customizations = customizations;
				this.baseHighlighter = baseHighlighter;
			}

			public TextDocument Document {
				get { return baseHighlighter.Document; }
			}
			
			public ICSharpCode.AvalonEdit.Utils.ImmutableStack<HighlightingSpan> GetSpanStack(int lineNumber)
			{
				return baseHighlighter.GetSpanStack(lineNumber);
			}
			
			public HighlightedLine HighlightLine(int lineNumber)
			{
				HighlightedLine line = baseHighlighter.HighlightLine(lineNumber);
				foreach (HighlightedSection section in line.Sections) {
					section.Color = CustomizeColor(section.Color);
				}
				return line;
			}
			
			HighlightingColor CustomizeColor(HighlightingColor color)
			{
				if (color == null || color.Name == null)
					return color;
				foreach (CustomizedHighlightingColor customization in customizations) {
					if (customization.Name == color.Name) {
						return new HighlightingColor {
							Name = color.Name,
							Background = CreateBrush(customization.Background),
							Foreground = CreateBrush(customization.Foreground),
							FontWeight = customization.Bold ? FontWeights.Bold : FontWeights.Normal,
							FontStyle = customization.Italic ? FontStyles.Italic : FontStyles.Normal
						};
					}
				}
				return color;
			}
			
			static HighlightingBrush CreateBrush(Color? color)
			{
				if (color == null)
					return null;
				else
					return new CustomizedBrush(color.Value);
			}
		}
		
		sealed class CustomizedBrush : HighlightingBrush
		{
			readonly SolidColorBrush brush;
			
			public CustomizedBrush(Color color)
			{
				brush = CreateFrozenBrush(color);
			}
			
			public override Brush GetBrush(ITextRunConstructionContext context)
			{
				return brush;
			}
			
			public override string ToString()
			{
				return brush.ToString();
			}
		}
		
		static SolidColorBrush CreateFrozenBrush(Color color)
		{
			SolidColorBrush brush = new SolidColorBrush(color);
			brush.Freeze();
			return brush;
		}
	}
}
