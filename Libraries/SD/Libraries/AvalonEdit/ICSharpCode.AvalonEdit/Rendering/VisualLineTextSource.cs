﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.Windows.Media.TextFormatting;

using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Rendering
{
	/// <summary>
	/// WPF TextSource implementation that creates TextRuns for a VisualLine.
	/// </summary>
	sealed class VisualLineTextSource : TextSource, ITextRunConstructionContext
	{
		public VisualLineTextSource(VisualLine visualLine)
		{
			this.VisualLine = visualLine;
		}
		
		public VisualLine VisualLine { get; private set; }
		public TextView TextView { get; set; }
		public TextDocument Document { get; set; }
		public TextRunProperties GlobalTextRunProperties { get; set; }
		
		public override TextRun GetTextRun(int textSourceCharacterIndex)
		{
			try {
				foreach (VisualLineElement element in VisualLine.Elements) {
					if (textSourceCharacterIndex >= element.VisualColumn
					    && textSourceCharacterIndex < element.VisualColumn + element.VisualLength)
					{
						int relativeOffset = textSourceCharacterIndex - element.VisualColumn;
						TextRun run = element.CreateTextRun(textSourceCharacterIndex, this);
						if (run == null)
							throw new ArgumentNullException(element.GetType().Name + ".CreateTextRun");
						if (run.Length == 0)
							throw new ArgumentException("The returned TextRun must not have length 0.", element.GetType().Name + ".Length");
						if (relativeOffset + run.Length > element.VisualLength)
							throw new ArgumentException("The returned TextRun is too long.", element.GetType().Name + ".CreateTextRun");
						InlineObjectRun inlineRun = run as InlineObjectRun;
						if (inlineRun != null) {
							inlineRun.VisualLine = VisualLine;
							TextView.textLayer.AddInlineObject(inlineRun);
						}
						return run;
					}
				}
				return new TextEndOfParagraph(1);
			} catch (Exception ex) {
				Debug.WriteLine(ex.ToString());
				throw;
			}
		}
		
		public override TextSpan<CultureSpecificCharacterBufferRange> GetPrecedingText(int textSourceCharacterIndexLimit)
		{
			try {
				foreach (VisualLineElement element in VisualLine.Elements) {
					if (textSourceCharacterIndexLimit > element.VisualColumn
					    && textSourceCharacterIndexLimit <= element.VisualColumn + element.VisualLength)
					{
						TextSpan<CultureSpecificCharacterBufferRange> span = element.GetPrecedingText(textSourceCharacterIndexLimit, this);
						if (span == null)
							break;
						int relativeOffset = textSourceCharacterIndexLimit - element.VisualColumn;
						if (span.Length > relativeOffset)
							throw new ArgumentException("The returned TextSpan is too long.", element.GetType().Name + ".GetPrecedingText");
						return span;
					}
				}
				CharacterBufferRange empty = CharacterBufferRange.Empty;
				return new TextSpan<CultureSpecificCharacterBufferRange>(empty.Length, new CultureSpecificCharacterBufferRange(null, empty));
			} catch (Exception ex) {
				Debug.WriteLine(ex.ToString());
				throw;
			}
		}
		
		public override int GetTextEffectCharacterIndexFromTextSourceCharacterIndex(int textSourceCharacterIndex)
		{
			throw new NotSupportedException();
		}
	}
}
