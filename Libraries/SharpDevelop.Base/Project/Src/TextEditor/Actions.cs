﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 915 $</version>
// </file>

using System.Drawing;
using System.Windows.Forms;
using System;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Actions;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Gui.CompletionWindow;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Actions
{
	public class TemplateCompletion : AbstractEditAction
	{
		public override void Execute(TextArea services)
		{
			SharpDevelopTextAreaControl sdtac = (SharpDevelopTextAreaControl)services.MotherTextEditorControl;
			services.AutoClearSelection = false;
			sdtac.ShowCompletionWindow(new TemplateCompletionDataProvider(), '\0');
		}
	}
	
	public class CodeCompletionPopup : AbstractEditAction
	{
		public override void Execute(TextArea services)
		{
			SharpDevelopTextAreaControl sdtac = (SharpDevelopTextAreaControl)services.MotherTextEditorControl;
			
			sdtac.ShowCompletionWindow(new CtrlSpaceCompletionDataProvider(), '\0');
		}
	}
	
	#if DEBUG
	public class DebugCodeCompletionAction : AbstractEditAction
	{
		public override void Execute(TextArea services)
		{
			SharpDevelopTextAreaControl sdtac = (SharpDevelopTextAreaControl)services.MotherTextEditorControl;
			CodeCompletionDataProvider ccdp = new CodeCompletionDataProvider();
			ccdp.DebugMode = true;
			sdtac.ShowCompletionWindow(ccdp, '.');
		}
	}
	#endif
	
	public class GoToDefinition : AbstractEditAction
	{
		public override void Execute(TextArea textArea)
		{
			TextEditorControl textEditorControl = textArea.MotherTextEditorControl;
			IDocument document = textEditorControl.Document;
			string textContent = document.TextContent;
			
			int caretLineNumber = document.GetLineNumberForOffset(textEditorControl.ActiveTextAreaControl.Caret.Offset) + 1;
			int caretColumn     = textEditorControl.ActiveTextAreaControl.Caret.Offset - document.GetLineSegment(caretLineNumber - 1).Offset + 1;
			
			IExpressionFinder expressionFinder = ParserService.GetExpressionFinder(textEditorControl.FileName);
			if (expressionFinder == null)
				return;
			ExpressionResult expression = expressionFinder.FindFullExpression(textContent, textEditorControl.ActiveTextAreaControl.Caret.Offset);
			if (expression.Expression == null || expression.Expression.Length == 0)
				return;
			ResolveResult result = ParserService.Resolve(expression, caretLineNumber, caretColumn, textEditorControl.FileName, textContent);
			if (result != null) {
				FilePosition pos = result.GetDefinitionPosition();
				if (pos != null) {
					try {
						if (pos.Position.IsEmpty)
							FileService.OpenFile(pos.Filename);
						else
							FileService.JumpToFilePosition(pos.Filename, pos.Position.X - 1, pos.Position.Y - 1);
					} catch (Exception ex) {
						MessageService.ShowError(ex, "Error jumping to '" + pos.Filename + "'.");
					}
				}
			}
		}
	}
}
