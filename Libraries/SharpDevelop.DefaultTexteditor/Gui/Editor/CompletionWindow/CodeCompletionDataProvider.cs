// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Reflection;
using System.Collections;
using System.Collections.Specialized;
using System.Xml;

using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;
using SharpDevelop.Internal.Parser;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	/// <summary>
	/// Data provider for code completion.
	/// </summary>
	public class CodeCompletionDataProvider : ICompletionDataProvider
	{
		static ClassBrowserIconsService classBrowserIconService = (ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService));
//		static AmbienceService          ambienceService = (AmbienceService)ServiceManager.Services.GetService(typeof(AmbienceService));
		Hashtable insertedElements           = new Hashtable();
		Hashtable insertedPropertiesElements = new Hashtable();
		Hashtable insertedEventElements      = new Hashtable();
		
		public ImageList ImageList {
			get {
				return classBrowserIconService.ImageList;
			}
		}
		
		int caretLineNumber;
		int caretColumn;
		string fileName;
		string preSelection = null;
		
		public string PreSelection {
			get {
				return preSelection;
			}
		}
		ArrayList completionData = null;
		bool ctrlSpace;
		
		public CodeCompletionDataProvider(bool ctrlSpace)
		{
			this.ctrlSpace = ctrlSpace;
		}
		
		public ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped)
		{
			IDocument document =  textArea.Document;
			completionData = new ArrayList();
			this.fileName = fileName;
			
			// the parser works with 1 based coordinates
			caretLineNumber      = document.GetLineNumberForOffset(textArea.Caret.Offset) + 1;
			caretColumn          = textArea.Caret.Offset - document.GetLineSegment(caretLineNumber - 1).Offset + 1;
			IParserService parserService = (IParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));
			IExpressionFinder expressionFinder = parserService.GetExpressionFinder(fileName);
			string expression    = expressionFinder == null ? TextUtilities.GetExpressionBeforeOffset(textArea, textArea.Caret.Offset) : expressionFinder.FindExpression(textArea.Document.GetText(0, textArea.Caret.Offset), textArea.Caret.Offset - 1);
			ResolveResult results;
			preSelection  = null;
			
			if (ctrlSpace) {
				if (expression == null || expression.Length == 0) {
					preSelection = "";
					AddResolveResults(parserService.CtrlSpace(parserService, caretLineNumber, caretColumn, fileName));
					return (ICompletionData[])completionData.ToArray(typeof(ICompletionData));
				}
				int idx = expression.LastIndexOf('.');
				if (idx > 0) {
					preSelection = expression.Substring(idx + 1);
					expression = expression.Substring(0, idx);
				} else {
					preSelection = expression;
					AddResolveResults(parserService.CtrlSpace(parserService, caretLineNumber, caretColumn, fileName));
					return (ICompletionData[])completionData.ToArray(typeof(ICompletionData));
				}
			}
			
			Console.WriteLine("Expression: >{0}<", expression);
			
			if (expression == null || expression.Length == 0) {
				return null;
			}
			//// do not instantiate service here as some checks might fail
			//IParserService parserService = (IParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));
			if (charTyped == ' ' && (expression.LastIndexOf("using")>=0 || expression.ToUpper().LastIndexOf("IMPORTS")>=0)) {
				if (expression == "using" || expression.EndsWith(" using") || expression.EndsWith("\tusing")|| expression.EndsWith("\nusing")|| expression.EndsWith("\rusing") ||
				    expression.ToUpper() == "IMPORTS" || expression.ToUpper().EndsWith(" IMPORTS") || expression.ToUpper().EndsWith("\tIMPORTS")|| expression.ToUpper().EndsWith("\nIMPORTS")|| expression.ToUpper().EndsWith("\rIMPORTS")) {
					string[] namespaces = parserService.GetNamespaceList("");
					AddResolveResults(namespaces);
				}
			} else {
				//// we don't need to run parser on blank char here
				if (charTyped==' ') {
					return null;
				}
				results = parserService.Resolve(expression,
				                                caretLineNumber,
				                                caretColumn,
				                                fileName,
				                                document.TextContent);
				AddResolveResults(results);
			}
			
			return (ICompletionData[])completionData.ToArray(typeof(ICompletionData));
		}
		
		void AddResolveResults(ICollection list) 
		{
			if (list == null) {
				return;
			}
			completionData.Capacity += list.Count;
			foreach (object o in list) {
				if (o is string) {
					completionData.Add(new CodeCompletionData(o.ToString(), classBrowserIconService.NamespaceIndex));
				} else if (o is IClass) {
					completionData.Add(new CodeCompletionData((IClass)o));
				} else if (o is IProperty) {
					IProperty property = (IProperty)o;
					if (property.Name != null && insertedPropertiesElements[property.Name] == null) {
						completionData.Add(new CodeCompletionData(property));
						insertedPropertiesElements[property.Name] = property;
					}
				} else if (o is IMethod) {
					IMethod method = (IMethod)o;
					
					if (method.Name != null &&!method.IsConstructor) {
						CodeCompletionData ccd = new CodeCompletionData(method);
						if (insertedElements[method.Name] == null) {
							completionData.Add(ccd);
							insertedElements[method.Name] = ccd;
						} else {
							CodeCompletionData oldMethod = (CodeCompletionData)insertedElements[method.Name];
							++oldMethod.Overloads;
						}
					}
				} else if (o is IField) {
					completionData.Add(new CodeCompletionData((IField)o));
				} else if (o is IEvent) {
					IEvent e = (IEvent)o;
					if (e.Name != null && insertedEventElements[e.Name] == null) {
						completionData.Add(new CodeCompletionData(e));
						insertedEventElements[e.Name] = e;
					}
				}
			}
		}
			
		void AddResolveResults(ResolveResult results)
		{
			if (results != null) {
				AddResolveResults(results.Namespaces);
				AddResolveResults(results.Members);
			}
		}
	}
}
