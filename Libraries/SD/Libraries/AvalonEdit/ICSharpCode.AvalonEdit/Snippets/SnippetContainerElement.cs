﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision: 5529 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Documents;

using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Snippets
{
	/// <summary>
	/// A snippet element that has sub-elements.
	/// </summary>
	[Serializable]
	public class SnippetContainerElement : SnippetElement
	{
		NullSafeCollection<SnippetElement> elements = new NullSafeCollection<SnippetElement>();
		
		/// <summary>
		/// Gets the list of child elements.
		/// </summary>
		public IList<SnippetElement> Elements {
			get { return elements; }
		}
		
		/// <inheritdoc/>
		public override void Insert(InsertionContext context)
		{
			foreach (SnippetElement e in this.Elements) {
				e.Insert(context);
			}
		}
		
		/// <inheritdoc/>
		public override Inline ToTextRun()
		{
			Span span = new Span();
			foreach (SnippetElement e in this.Elements) {
				Inline r = e.ToTextRun();
				if (r != null)
					span.Inlines.Add(r);
			}
			return span;
		}
	}
}
