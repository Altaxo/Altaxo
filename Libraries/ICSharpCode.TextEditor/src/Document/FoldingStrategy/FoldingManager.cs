// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Collections;

namespace ICSharpCode.TextEditor.Document
{
	public class FoldingManager
	{
		ArrayList           foldMarker      = new ArrayList();
		IFoldingStrategy    foldingStrategy = null;
		IDocument document;
		public ArrayList FoldMarker {
			get {
				return foldMarker;
			}
		}
		
		public IFoldingStrategy FoldingStrategy {
			get {
				return foldingStrategy;
			}
			set {
				foldingStrategy = value;
			}
		}
		
		public FoldingManager(IDocument document)
		{
			this.document = document;
//			foldMarker.Add(new FoldMarker(0, 5, 3, 5));
//			
//			foldMarker.Add(new FoldMarker(5, 5, 10, 3));
//			foldMarker.Add(new FoldMarker(6, 0, 8, 2));
//			
//			FoldMarker fm1 = new FoldMarker(10, 4, 10, 7);
//			FoldMarker fm2 = new FoldMarker(10, 10, 10, 14);
//			
//			fm1.IsFolded = true;
//			fm2.IsFolded = true;
//			
//			foldMarker.Add(fm1);
//			foldMarker.Add(fm2);
		}
		
		public ArrayList GetFoldingsWithStart(int lineNumber)
		{
			ArrayList foldings = new ArrayList();
			
			foreach (FoldMarker fm in foldMarker) {
				if (fm.StartLine == lineNumber) {
					foldings.Add(fm);
				}
			}
			return foldings;
		}
		
		public ArrayList GetFoldingsWithEnd(int lineNumber)
		{
			ArrayList foldings = new ArrayList();
			
			foreach (FoldMarker fm in foldMarker) {
				if (fm.EndLine == lineNumber) {
					foldings.Add(fm);
				}
			}
			return foldings;
		}

		public bool IsFoldStart(int lineNumber)
		{
			foreach (FoldMarker fm in foldMarker) {
				if (fm.StartLine == lineNumber) {
					return true;
				}
			}
			return false;
		}
		
		public bool IsFoldEnd(int lineNumber)
		{
			foreach (FoldMarker fm in foldMarker) {
				if (fm.EndLine == lineNumber) {
					return true;
				}
			}
			return false;
		}
		
		public bool IsBetweenFolding(int lineNumber)
		{
			foreach (FoldMarker fm in foldMarker) {
				if (fm.StartLine < lineNumber && lineNumber < fm.EndLine) {
					return true;
				}
			}
			return false;
		}
		
		public bool IsLineVisible(int lineNumber)
		{
			foreach (FoldMarker fm in foldMarker) {
				if (fm.IsFolded && fm.StartLine < lineNumber && lineNumber <= fm.EndLine) {
					return false;
				}
			}
			return true;
		}
		
		ArrayList GetTopLevelFoldedFoldings()
		{
			ArrayList foldings = new ArrayList();
			foreach (FoldMarker fm in foldMarker) {
				if (fm.IsFolded) {
					foldings.Add(fm);
				}
			}
			return foldings;
		}
		
		public void UpdateFoldings(string fileName, object parseInfo)
		{
			ArrayList newFoldings = foldingStrategy.GenerateFoldMarkers(document, fileName, parseInfo);
			if (newFoldings != null) {
//				foreach (object o in newFoldings)  {
//					Console.WriteLine(o);
//				}
				// TODO : merge!!!
				this.foldMarker = newFoldings;
			}
		}
	}
}
