// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Gui.XmlForms;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs
{
	public class EditTemplateDialog : BaseSharpDevelopForm 
	{
		CodeTemplate codeTemplate;
		
		public CodeTemplate CodeTemplate {
			get {
				return codeTemplate;
			}
		}
		
		public EditTemplateDialog(CodeTemplate codeTemplate)
		{
			this.codeTemplate = codeTemplate;
			InitializeComponents();
		}
		
		void AcceptEvent(object sender, EventArgs e)
		{
			codeTemplate.Shortcut    = ControlDictionary["templateTextBox"].Text;
			codeTemplate.Description = ControlDictionary["descriptionTextBox"].Text;
		}
		
		void InitializeComponents()
		{
			base.SetupFromXml(Path.Combine(PropertyService.DataDirectory, @"resources\dialogs\EditTemplateDialog.xfrm"));
			
			ControlDictionary["templateTextBox"].Text    = codeTemplate.Shortcut;
			ControlDictionary["descriptionTextBox"].Text = codeTemplate.Description;
			
			ControlDictionary["okButton"].Click += new EventHandler(AcceptEvent);
			
			Owner = (Form)WorkbenchSingleton.Workbench;
			Icon  = null;
		}
	}
}
