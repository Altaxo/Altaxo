// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Collections;
using ICSharpCode.SharpDevelop.Services;
using SharpDevelop.Internal.Parser;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	/// <summary>
	/// A simple folding strategy which calculates the folding level
	/// using the indent level of the line.
	/// </summary>
	public class ParserFoldingStrategy : IFoldingStrategy
	{
		void AddClassMembers(IClass c, ArrayList foldMarkers, IDocument document)
		{
			if (c.ClassType == ClassType.Delegate) {
				return;
			}
			if (c.Region.EndLine  > 0) {
				foldMarkers.Add(new FoldMarker(document, c.Region.BeginLine - 1, c.Region.BeginColumn - 1,
				                               c.Region.EndLine - 1, c.Region.EndColumn, FoldType.TypeBody));
			}
			foreach (IClass innerClass in c.InnerClasses) {
				AddClassMembers(innerClass, foldMarkers, document);
			}
			foreach (IMethod m in c.Methods) {
				if (m.BodyRegion != null && m.BodyRegion.EndLine > 0) {
					foldMarkers.Add(new FoldMarker(document, m.Region.EndLine - 1, m.Region.EndColumn - 1,
					                               m.BodyRegion.EndLine - 1, m.BodyRegion.EndColumn - 1, FoldType.MemberBody));
				}
			}
			
			foreach (IIndexer indexer in c.Indexer) {
				if (indexer.BodyRegion != null && indexer.BodyRegion.EndLine > 0) {
					foldMarkers.Add(new FoldMarker(document, indexer.Region.EndLine - 1,    indexer.Region.EndColumn - 1,
					                               indexer.BodyRegion.EndLine- 1, indexer.BodyRegion.EndColumn - 1, FoldType.MemberBody));
				}
			}
			
			foreach (IProperty p in c.Properties) {
				if (p.BodyRegion != null && p.BodyRegion.EndLine > 0) {
					foldMarkers.Add(new FoldMarker(document, p.Region.EndLine - 1, p.Region.EndColumn - 1,
					                               p.BodyRegion.EndLine- 1, p.BodyRegion.EndColumn - 1, FoldType.MemberBody));
				}
			}
			
			foreach (IEvent evt in c.Events) {
				if (evt.BodyRegion != null && evt.BodyRegion.EndLine > 0) {
					if (evt.BodyRegion != null) {
						foldMarkers.Add(new FoldMarker(document, evt.Region.EndLine - 1, evt.Region.EndColumn - 1,
						                               evt.BodyRegion.EndLine- 1, evt.BodyRegion.EndColumn - 1, FoldType.MemberBody));
					}
				}
			}
		}
		
		/// <remarks>
		/// Calculates the fold level of a specific line.
		/// </remarks>
		public ArrayList GenerateFoldMarkers(IDocument document, string fileName, object parseInfo)
		{
			ParseInformation parseInformation = parseInfo as ParseInformation;
			if (parseInformation == null || parseInformation.MostRecentCompilationUnit == null) {
				return null;
			}
			
			ArrayList foldMarkers = new ArrayList();
			ICompilationUnit cu = (ICompilationUnit)parseInformation.MostRecentCompilationUnit;
			bool firstTime = document.FoldingManager.FoldMarker.Count == 0;
			foreach (FoldingRegion foldingRegion in cu.FoldingRegions) {
				foldMarkers.Add(new FoldMarker(document, foldingRegion.Region.BeginLine - 1, foldingRegion.Region.BeginColumn - 1,
				                               foldingRegion.Region.EndLine - 1, foldingRegion.Region.EndColumn - 1, FoldType.Region, foldingRegion.Name, firstTime));
				
			}
			foreach (IClass c in cu.Classes) {
				AddClassMembers(c, foldMarkers, document);
			}
			
			if (cu.DokuComments != null) {
				foreach (Comment c in cu.DokuComments) {
					foldMarkers.Add(new FoldMarker(document, c.Region.BeginLine - 1, c.Region.BeginColumn - 1,
					                               c.Region.EndLine - 1, c.Region.EndColumn - 1));
				}
			}
			
			return foldMarkers;
		}
	}
}
