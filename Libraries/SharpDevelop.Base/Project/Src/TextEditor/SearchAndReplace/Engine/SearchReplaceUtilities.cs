﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Collections;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace SearchAndReplace
{
	public sealed class SearchReplaceUtilities
	{
		public static bool IsTextAreaSelected {
			get {
				return WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null &&
					WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent is ITextEditorControlProvider;
			}
		}
		
		public static TextEditorControl GetActiveTextEditor()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			if (window != null && (window.ViewContent is ITextEditorControlProvider)) {
				return ((ITextEditorControlProvider)window.ViewContent).TextEditorControl;
			}
			return null;
		}
		
		public static bool IsWholeWordAt(ITextBufferStrategy document, int offset, int length)
		{
			return (offset - 1 < 0 || Char.IsWhiteSpace(document.GetCharAt(offset - 1))) &&
				(offset + length + 1 >= document.Length || Char.IsWhiteSpace(document.GetCharAt(offset + length)));
		}
		
		public static ISearchStrategy CreateSearchStrategy(SearchStrategyType type)
		{
			switch (type) {
				case SearchStrategyType.Normal:
					return new BruteForceSearchStrategy(); // new KMPSearchStrategy();
				case SearchStrategyType.RegEx:
					return new RegExSearchStrategy();
				case SearchStrategyType.Wildcard:
					return new WildcardSearchStrategy();
				default:
					throw new System.NotImplementedException("CreateSearchStrategy for type " + type);
			}
		}
		
		public static IDocumentIterator CreateDocumentIterator(DocumentIteratorType type)
		{
			switch (type) {
				case DocumentIteratorType.CurrentDocument:
				case DocumentIteratorType.CurrentSelection:
					return new CurrentDocumentIterator();
				case DocumentIteratorType.Directory:
					try {
						if (!Directory.Exists(SearchOptions.LookIn)) {
							MessageService.ShowMessageFormatted("${res:Dialog.NewProject.SearchReplace.SearchStringNotFound.Title}", "${res:Dialog.NewProject.SearchReplace.LookIn.DirectoryNotFound}", Path.GetFullPath(SearchOptions.LookIn));
							return new DummyDocumentIterator();
						}
					} catch (Exception ex) {
						MessageService.ShowMessage(ex.Message);
						return new DummyDocumentIterator();
					}
					return new DirectoryDocumentIterator(SearchOptions.LookIn,
					                                     SearchOptions.LookInFiletypes,
					                                     SearchOptions.IncludeSubdirectories);
				case DocumentIteratorType.AllOpenFiles:
					return new AllOpenDocumentIterator();
				case DocumentIteratorType.WholeProject:
					return new WholeProjectDocumentIterator();
				case DocumentIteratorType.WholeSolution:
					return new WholeSolutionDocumentIterator();
				default:
					throw new System.NotImplementedException("CreateDocumentIterator for type " + type);
			}
		}
		
		static ArrayList excludedFileExtensions;
		
		public static bool IsSearchable(string fileName)
		{
			if (fileName == null)
				return false;
			
			if (excludedFileExtensions == null) {
				excludedFileExtensions = AddInTree.BuildItems("/AddIns/DefaultTextEditor/Search/ExcludedFileExtensions", null, false);
			}
			string extension = Path.GetExtension(fileName);
			if (extension != null) {
				foreach (string excludedExtension in excludedFileExtensions) {
					if (String.Compare(excludedExtension, extension, true) == 0) {
						return false;
					}
				}
			}
			return true;
		}
		
		public static void SelectText(TextEditorControl textArea, int offset, int endOffset)
		{
			int textLength = textArea.ActiveTextAreaControl.Document.TextLength;
			if (textLength < endOffset) {
				endOffset = textLength - 1;
			}
			textArea.ActiveTextAreaControl.Caret.Position = textArea.Document.OffsetToPosition(endOffset);
			textArea.ActiveTextAreaControl.TextArea.SelectionManager.ClearSelection();
			textArea.ActiveTextAreaControl.TextArea.SelectionManager.SetSelection(new DefaultSelection(textArea.Document, textArea.Document.OffsetToPosition(offset),
			                                                                                           textArea.Document.OffsetToPosition(endOffset)));
			textArea.Refresh();
		}
	}
}
