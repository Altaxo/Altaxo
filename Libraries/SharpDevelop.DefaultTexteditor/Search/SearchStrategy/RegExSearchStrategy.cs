// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Text.RegularExpressions;

using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.Internal.Undo;

namespace ICSharpCode.TextEditor.Document
{
	public class RegExSearchStrategy : ISearchStrategy
	{
		Regex regex = null;
		
		public void CompilePattern(SearchOptions options)
		{
			RegexOptions regexOptions = RegexOptions.Compiled;
			if (options.IgnoreCase) {
				regexOptions |= RegexOptions.IgnoreCase;
			}
			regex = new Regex(options.SearchPattern, regexOptions);
		}
		
		public ISearchResult FindNext(ITextIterator textIterator, SearchOptions options)
		{
			string document = textIterator.TextBuffer.GetText(0, textIterator.TextBuffer.Length);
			
			while (textIterator.MoveAhead(1)) {
				Match m = regex.Match(document, textIterator.Position);
				if (m == null || m.Index <= 0 || m.Length <= 0) {
					
				} else {
					int delta = m.Index - textIterator.Position;
					if (delta <= 0 || textIterator.MoveAhead(delta)) {
						return new DefaultSearchResult(m.Index, m.Length);
					} else {
						return null;
					}
				}
			}
			
			return null;
		}
	}
}
