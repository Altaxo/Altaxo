// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System.Drawing;
using System.Windows.Forms;
using System;

using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Actions;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Gui.CompletionWindow;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Actions
{
	public class TemplateCompletion : AbstractEditAction
	{
		public override void Execute(TextArea services)
		{
			SharpDevelopTextAreaControl sdtac = (SharpDevelopTextAreaControl)services.MotherTextEditorControl;
			services.AutoClearSelection = false;
			sdtac.codeCompletionWindow = CodeCompletionWindow.ShowCompletionWindow(((Form)WorkbenchSingleton.Workbench), services.MotherTextEditorControl, services.MotherTextEditorControl.FileName, new TemplateCompletionDataProvider(), '\0');
		}
	}
	
	public class CodeCompletionPopup : AbstractEditAction
	{
		public override void Execute(TextArea services)
		{
			SharpDevelopTextAreaControl sdtac = (SharpDevelopTextAreaControl)services.MotherTextEditorControl;
			IParserService parserService = (IParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));
			sdtac.codeCompletionWindow = CodeCompletionWindow.ShowCompletionWindow(((Form)WorkbenchSingleton.Workbench), 
			                                                                       services.MotherTextEditorControl, 
			                                                                       services.MotherTextEditorControl.FileName,
			                                                                       sdtac.CreateCodeCompletionDataProvider(true), '\0');
		}
	}
}
