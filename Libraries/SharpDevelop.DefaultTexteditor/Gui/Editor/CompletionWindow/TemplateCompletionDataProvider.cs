// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Collections;

using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	public class TemplateCompletionDataProvider : ICompletionDataProvider
	{
		public ImageList ImageList {
			get {
				return null;
			}
		}
		
		public ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped)
		{
			CodeTemplateGroup templateGroup = CodeTemplateLoader.GetTemplateGroupPerFilename(fileName);
			if (templateGroup == null) {
				return null;
			}
			ArrayList completionData = new ArrayList();
			foreach (CodeTemplate template in templateGroup.Templates) {
				completionData.Add(new TemplateCompletionData(template));
			}
			
			return (ICompletionData[])completionData.ToArray(typeof(ICompletionData));
		}
		
		class TemplateCompletionData : ICompletionData
		{
			CodeTemplate template;
			
			public int ImageIndex {
				get {
					return 0;
				}
			}
			
			public string[] Text {
				get {
					return new string[] { template.Shortcut, template.Description };
				}
			}
			
			public string Description {
				get {
					return template.Text;
				}
			}
			
			public void InsertAction(TextEditorControl control)
			{
				((SharpDevelopTextAreaControl)control).InsertTemplate(template);
			}
			
			public TemplateCompletionData(CodeTemplate template) 
			{
				this.template = template;
			}
		}
	}
}
