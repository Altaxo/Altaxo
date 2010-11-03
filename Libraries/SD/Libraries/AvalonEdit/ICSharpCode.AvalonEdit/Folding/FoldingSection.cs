﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace ICSharpCode.AvalonEdit.Folding
{
	/// <summary>
	/// A section that can be folded.
	/// </summary>
	public sealed class FoldingSection : TextSegment
	{
		FoldingManager manager;
		bool isFolded;
		CollapsedLineSection collapsedSection;
		string title;
		
		/// <summary>
		/// Gets/sets if the section is folded.
		/// </summary>
		public bool IsFolded {
			get { return isFolded; }
			set {
				if (isFolded != value) {
					isFolded = value;
					if (value) {
						if (manager != null) {
							DocumentLine startLine = manager.document.GetLineByOffset(StartOffset);
							DocumentLine endLine = manager.document.GetLineByOffset(EndOffset);
							if (startLine != endLine) {
								DocumentLine startLinePlusOne = startLine.NextLine;
								collapsedSection = manager.textView.CollapseLines(startLinePlusOne, endLine);
							}
						}
					} else {
						RemoveCollapsedLineSection();
					}
					if (manager != null)
						manager.textView.Redraw(this, DispatcherPriority.Normal);
				}
			}
		}
		
		/// <summary>
		/// Gets/Sets the text used to display the collapsed version of the folding section.
		/// </summary>
		public string Title {
			get {
				return title;
			}
			set {
				if (title != value) {
					title = value;
					if (this.IsFolded && manager != null)
						manager.textView.Redraw(this, DispatcherPriority.Normal);
				}
			}
		}
		
		/// <summary>
		/// Gets/Sets an additional object associated with this folding section.
		/// </summary>
		public object Tag { get; set; }
		
		internal FoldingSection(FoldingManager manager, int startOffset, int endOffset)
		{
			this.manager = manager;
			this.StartOffset = startOffset;
			this.Length = endOffset - startOffset;
		}
		
		void RemoveCollapsedLineSection()
		{
			if (collapsedSection != null) {
				if (collapsedSection.Start != null)
					collapsedSection.Uncollapse();
				collapsedSection = null;
			}
		}
		
		internal void Removed()
		{
			manager = null;
		}
	}
}
