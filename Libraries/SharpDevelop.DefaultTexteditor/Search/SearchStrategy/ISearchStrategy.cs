// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;

using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.Internal.Undo;

namespace ICSharpCode.TextEditor.Document
{
	/// <summary>
	/// This interface is the basic interface which all 
	/// search algorithms must implement.
	/// </summary>
	public interface ISearchStrategy
	{
		/// <remarks>
		/// Only with a call to this method the search strategy must
		/// update their pattern information. This method will be called 
		/// before the FindNext function.
		/// </remarks>
		void CompilePattern(SearchOptions options);
		
		/// <remarks>
		/// The find next method should search the next occurrence of the 
		/// compiled pattern in the text using the textIterator and options.
		/// </remarks>
		ISearchResult FindNext(ITextIterator textIterator, SearchOptions options);
	}
}
