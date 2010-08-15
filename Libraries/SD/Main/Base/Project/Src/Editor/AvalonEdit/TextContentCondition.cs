﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 6033 $</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Editor.AvalonEdit
{
	/// <summary>
	/// Tests the name of the highlighting strategy of the text editor.
	/// </summary>
	/// <attribute name="textcontent">
	/// The name of the highlighting strategy that should be active.
	/// </attribute>
	/// <example title="Test if any XML file is being edited">
	/// &lt;Condition name = "TextContent" textcontent="XML"&gt;
	/// </example>
	public class TextContentConditionEvaluator : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			string textcontent = condition.Properties["textcontent"];
			IViewContent content = WorkbenchSingleton.Workbench.ActiveViewContent;
			if (content is ITextEditorProvider) {
				var ctrl = (content as ITextEditorProvider).TextEditor.GetService(typeof(ICSharpCode.AvalonEdit.TextEditor)) as ICSharpCode.AvalonEdit.TextEditor;
				if (ctrl != null && ctrl.SyntaxHighlighting != null) {
					return string.Equals(textcontent, ctrl.SyntaxHighlighting.Name, StringComparison.OrdinalIgnoreCase);
				}
			}
			return false;
		}
	}
}
