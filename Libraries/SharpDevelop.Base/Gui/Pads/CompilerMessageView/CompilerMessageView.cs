// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.CodeDom.Compiler;
using System.IO;
using System.Diagnostics;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	/// <summary>
	/// This class displays the errors and warnings which the compiler outputs and
	/// allows the user to jump to the source of the warnig / error
	/// </summary>
	public class CompilerMessageView : AbstractPadContent
	{
		RichTextBox     textEditorControl = new RichTextBox();
		ComboBox        messageCategory   = new ComboBox();
		Panel           myPanel           = new Panel();
		
		ResourceService resourceService   = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
		ArrayList       messageCategories = new ArrayList();
		
		public override Control Control {
			get {
				return myPanel;
			}
		}
		
		public CompilerMessageView() : base("${res:MainWindow.Windows.OutputWindow}", "Icons.16x16.OutputIcon")
		{
			AddCategory(new MessageViewCategory("Build", "${res:MainWindow.Windows.OutputWindow.BuildCategory}"));
			
			messageCategory.Dock = DockStyle.Top;
			messageCategory.DropDownStyle = ComboBoxStyle.DropDownList;
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			messageCategory.SelectedIndex = 0;
			messageCategory.SelectedIndexChanged += new EventHandler(MessageCategorySelectedIndexChanged);
			textEditorControl.Dock     = DockStyle.Fill;
			textEditorControl.ReadOnly = true;
			
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			messageCategory.Font = resourceService.LoadFont("Arial", 9, FontStyle.Bold);
			textEditorControl.Font = resourceService.LoadFont("Courier New", 10);
			myPanel.Controls.Add(textEditorControl);
			myPanel.Controls.Add(messageCategory);
			
			TaskService     taskService    = (TaskService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(TaskService));
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			
			taskService.CompilerOutputChanged += new EventHandler(SetCompilerOutput);
			
			projectService.StartBuild    += new EventHandler(ProjectServiceStartBuild);
			projectService.CombineOpened += new CombineEventHandler(ClearOnCombineEvent);
			
			textEditorControl.CreateControl();
		}
		
		public override void RedrawContent()
		{
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			messageCategory.Items.Clear();
			foreach (MessageViewCategory category in messageCategories) {
				messageCategory.Items.Add(stringParserService.Parse(category.DisplayCategory));
			}
			OnTitleChanged(null);
			OnIconChanged(null);
		}
		
		#region Category handling
		public void AddCategory(MessageViewCategory category)
		{
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			messageCategory.Items.Add(stringParserService.Parse(category.DisplayCategory));
			messageCategories.Add(category);
			category.TextChanged += new EventHandler(CategoryTextChanged);
		}
		
		void CategoryTextChanged(object sender, EventArgs e)
		{
			MessageViewCategory category = (MessageViewCategory)sender;
			SelectCategory(category.Category);
			OutputCategoryText();
		}
		
		public void SelectCategory(string categoryName)
		{
			for (int i = 0; i < messageCategories.Count; ++i) {
				MessageViewCategory category = (MessageViewCategory)messageCategories[i];
				if (category.Category == categoryName) {
					messageCategory.SelectedIndex = i;
					break;
				}
			}
			ActivateThisPad();
		}
		
		public MessageViewCategory GetCategory(string categoryName)
		{
			foreach (MessageViewCategory category in messageCategories) {
				if (category.Category == categoryName) {
					return category;
				}
			}
			return null;
		}
		#endregion
		
		#region Message Event Handlers (Build)
		void ProjectServiceStartBuild(object sender, EventArgs e)
		{
			MessageViewCategory buildCategory = GetCategory("Build");
			buildCategory.ClearText();
			SelectCategory("Build");
		}
		
		void ClearOnCombineEvent(object sender, CombineEventArgs e)
		{
			MessageViewCategory buildCategory = GetCategory("Build");
			buildCategory.ClearText();
		}
		
		void SetCompilerOutput(object sender, EventArgs e)
		{
			TaskService taskService = (TaskService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(TaskService));
			MessageViewCategory buildCategory = GetCategory("Build");
			buildCategory.SetText(taskService.CompilerOutput);
		}
		#endregion
		
		delegate void StringDelegate(string text);
		/// <summary>
		/// Non threadsafe way of setting text -> use OutputCategoryText() to do the job.
		/// </summary>
		void SetText(string text)
		{
			if (text == null) {
				text = String.Empty;
			}
			textEditorControl.Text = text;
			textEditorControl.Select(text.Length , 0);
			textEditorControl.Select();
			textEditorControl.ScrollToCaret();
		}
		
		/// <summary>
		/// Makes this pad visible (usually BEFORE build or debug events)
		/// </summary>
		void ActivateThisPad()
		{
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			WorkbenchSingleton.Workbench.WorkbenchLayout.ActivatePad(this);
		}
		
		/// <summary>
		/// Thread safe way to set the output text of the current selected category
		/// in the text area.
		/// </summary>
		void OutputCategoryText()
		{
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			string text = stringParserService.Parse(((MessageViewCategory)messageCategories[messageCategory.SelectedIndex]).Text);
			// use invoke when running on another thread (in #D all other treads are background threads)
			if (Thread.CurrentThread.IsBackground) {
				textEditorControl.Invoke(new StringDelegate(SetText), new object[] {text} );
			} else {
				SetText(text);
			}
		}
		
		void MessageCategorySelectedIndexChanged(object sender, EventArgs e)
		{
			OutputCategoryText();
		}
	}
}
