// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;

using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.Internal.Undo;

namespace ICSharpCode.TextEditor.Document
{
	/// <summary>
	///  Only for fallback purposes.
	/// </summary>
	public class BruteForceSearchStrategy : ISearchStrategy
	{
		string searchPattern;
		
		bool MatchCaseSensitive(ITextBufferStrategy document, int offset, string pattern)
		{
			for (int i = 0; i < pattern.Length; ++i) {
				if (offset + i >= document.Length || document.GetCharAt(offset + i) != pattern[i]) {
					return false;
				}
			}
			return true;
		}
		
		bool MatchCaseInsensitive(ITextBufferStrategy document, int offset, string pattern)
		{
			for (int i = 0; i < pattern.Length; ++i) {
				if (offset + i >= document.Length || Char.ToUpper(document.GetCharAt(offset + i)) != pattern[i]) {
					return false;
				}
			}
			return true;
		}
		
		bool IsWholeWordAt(ITextBufferStrategy document, int offset, int length)
		{
			return (offset - 1 < 0 || Char.IsWhiteSpace(document.GetCharAt(offset - 1))) &&
			       (offset + length + 1 >= document.Length || Char.IsWhiteSpace(document.GetCharAt(offset + length)));
		}
		
		int InternalFindNext(ITextIterator textIterator, SearchOptions options)
		{
			while (textIterator.MoveAhead(1)) {
				if (options.IgnoreCase ? MatchCaseInsensitive(textIterator.TextBuffer, textIterator.Position, searchPattern) :
				                         MatchCaseSensitive(textIterator.TextBuffer, textIterator.Position, searchPattern)) {
					if (!options.SearchWholeWordOnly || IsWholeWordAt(textIterator.TextBuffer, textIterator.Position, searchPattern.Length)) {
						return textIterator.Position;
					}
				}
			}
			return -1;
		}
		
		public void CompilePattern(SearchOptions options)
		{
			searchPattern = options.IgnoreCase ? options.SearchPattern.ToUpper() : options.SearchPattern;
		}
		
		public ISearchResult FindNext(ITextIterator textIterator, SearchOptions options)
		{
			int offset = InternalFindNext(textIterator, options);
			return offset >= 0 ? new DefaultSearchResult(offset, searchPattern.Length) : null;
		}
	}
}
