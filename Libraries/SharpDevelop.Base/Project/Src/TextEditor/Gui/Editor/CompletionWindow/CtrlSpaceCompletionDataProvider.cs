// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	public class CtrlSpaceCompletionDataProvider : CodeCompletionDataProvider
	{
		public CtrlSpaceCompletionDataProvider()
		{
		}
		
		public CtrlSpaceCompletionDataProvider(ExpressionContext overrideContext)
		{
			this.overrideContext = overrideContext;
		}
		
		bool forceNewExpression;
		
		/// <summary>
		/// Gets/Sets whether the CtrlSpaceCompletionDataProvider creates a new completion
		/// dropdown instead of completing an old expression.
		/// Default value is false.
		/// </summary>
		public bool ForceNewExpression {
			get {
				return forceNewExpression;
			}
			set {
				forceNewExpression = value;
			}
		}
		
		protected override void GenerateCompletionData(TextArea textArea, char charTyped)
		{
			if (forceNewExpression) {
				preSelection = "";
				if (charTyped != '\0') {
					preSelection = null;
				}
				ExpressionContext context = overrideContext;
				if (context == null) context = ExpressionContext.Default;
				AddResolveResults(ParserService.CtrlSpace(caretLineNumber, caretColumn, fileName, textArea.Document.TextContent, context), context);
				return;
			}
			
			ExpressionResult expressionResult = GetExpression(textArea);
			string expression = expressionResult.Expression;
			preSelection = null;
			if (expression == null || expression.Length == 0) {
				preSelection = "";
				if (charTyped != '\0') {
					preSelection = null;
				}
				AddResolveResults(ParserService.CtrlSpace(caretLineNumber, caretColumn, fileName, textArea.Document.TextContent, expressionResult.Context), expressionResult.Context);
				return;
			}
			
			int idx = expression.LastIndexOf('.');
			if (idx > 0) {
				preSelection = expression.Substring(idx + 1);
				expressionResult.Expression = expression.Substring(0, idx);
				if (charTyped != '\0') {
					preSelection = null;
				}
				GenerateCompletionData(textArea, expressionResult);
			} else {
				preSelection = expression;
				if (charTyped != '\0') {
					preSelection = null;
				}
				AddResolveResults(ParserService.CtrlSpace(caretLineNumber, caretColumn, fileName, textArea.Document.TextContent, expressionResult.Context), expressionResult.Context);
			}
		}
	}
}
