// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
#if !LINUX
using System;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;
using ICSharpCode.Core.Properties;

using ICSharpCode.Core.Services;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Project;

namespace ICSharpCode.SharpDevelop.Internal.Project
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class IncludeFilesDialog : System.Windows.Forms.Form
	{
		GroupBox    GroupBox1;
		RadioButton RadiookButton;
		RadioButton RadioButton1;
		CheckedListBox CheckedListBox1;
		Button okButton;
		Button selectAllButton;
		Button cancelButton;
		Label Label1;
		Label Label2;
		Button deselectAllButton;
		
		StringCollection newFiles;
		IProject         project;
		IResourceService resourceService = (IResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
		
		public IncludeFilesDialog(IProject project, StringCollection newFiles)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			
			MinimizeBox = MaximizeBox = false;
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			Text = stringParserService.Parse(resourceService.GetString("Dialog.IncludeFilesDialog.DialogName"), 
			                          new string[,] {{ "PROJECT",  project.Name}});
			
			Owner = (Form)WorkbenchSingleton.Workbench;
			StartPosition = FormStartPosition.CenterParent;
			Icon = null;
			RadioButton1.Checked = true;
			
			this.newFiles = newFiles;
			this.project  = project;
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			foreach (string file in newFiles) {
				CheckedListBox1.Items.Add(fileUtilityService.AbsoluteToRelativePath(project.BaseDirectory, file));
			}
			
			okButton.Click += new EventHandler(AcceptEvent);
			selectAllButton.Click += new EventHandler(SelectAll);
			deselectAllButton.Click += new EventHandler(DeselectAll);
		}
		
		void AcceptEvent(object sender, EventArgs e)
		{
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			for (int i = 0; i < CheckedListBox1.Items.Count; ++i) {
				string file = fileUtilityService.RelativeToAbsolutePath(project.BaseDirectory,CheckedListBox1.Items[i].ToString());
				ProjectFile finfo = new ProjectFile(file);
				if (CheckedListBox1.GetItemChecked(i)) {
					finfo.BuildAction = project.IsCompileable(file) ? BuildAction.Compile : BuildAction.Nothing;
				} else {
					finfo.BuildAction = BuildAction.Exclude;
				}
				project.ProjectFiles.Add(finfo);
			}
		}
		
		void SelectAll(object sender, EventArgs e)
		{
			for (int i = 0; i < CheckedListBox1.Items.Count; ++i) {
				CheckedListBox1.SetItemChecked(i, true);
			}
		}
		
		void DeselectAll(object sender, EventArgs e)
		{
			for (int i = 0; i < CheckedListBox1.Items.Count; ++i) {
				CheckedListBox1.SetItemChecked(i, false);
			}
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			this.GroupBox1 = new System.Windows.Forms.GroupBox();
			this.RadiookButton = new System.Windows.Forms.RadioButton();
			this.RadioButton1 = new System.Windows.Forms.RadioButton();
			this.CheckedListBox1 = new System.Windows.Forms.CheckedListBox();
			this.okButton = new System.Windows.Forms.Button();
			this.selectAllButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.Label1 = new System.Windows.Forms.Label();
			this.Label2 = new System.Windows.Forms.Label();
			this.deselectAllButton = new System.Windows.Forms.Button();
			this.GroupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// GroupBox1
			// 
			this.GroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left);
			this.GroupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.RadiookButton,
																					this.RadioButton1});
			this.GroupBox1.Location = new System.Drawing.Point(8, 232);
			this.GroupBox1.Name = "GroupBox1";
			this.GroupBox1.Size = new System.Drawing.Size(152, 98);
			this.GroupBox1.TabIndex = 2;
			this.GroupBox1.TabStop = false;
			this.GroupBox1.Text = resourceService.GetString("Dialog.IncludeFilesDialog.ViewGroupBoxText");
			GroupBox1.FlatStyle = FlatStyle.System;
			
			//
			// RadiookButton
			// 
			this.RadiookButton.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.RadiookButton.Location = new System.Drawing.Point(8, 40);
			this.RadiookButton.Name = "RadiookButton";
			this.RadiookButton.Size = new System.Drawing.Size(136, 24);
			this.RadiookButton.TabIndex = 4;
			this.RadiookButton.Text = resourceService.GetString("Dialog.IncludeFilesDialog.AllFilesRadioButton");
			RadiookButton.FlatStyle = FlatStyle.System;
			
			// 
			// RadioButton1
			// 
			this.RadioButton1.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.RadioButton1.Location = new System.Drawing.Point(8, 16);
			this.RadioButton1.Name = "RadioButton1";
			this.RadioButton1.Size = new System.Drawing.Size(136, 24);
			this.RadioButton1.TabIndex = 3;
			this.RadioButton1.Text = resourceService.GetString("Dialog.IncludeFilesDialog.NewFilesRadioButton");
			RadioButton1.FlatStyle = FlatStyle.System;
			
			// 
			// CheckedListBox1
			// 
			this.CheckedListBox1.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.CheckedListBox1.Location = new System.Drawing.Point(8, 24);
			this.CheckedListBox1.Name = "CheckedListBox1";
			this.CheckedListBox1.Size = new System.Drawing.Size(316, 199);
			this.CheckedListBox1.TabIndex = 1;
			// 
			// okButton
			// 
			this.okButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.okButton.Location = new System.Drawing.Point(172, 308);
			this.okButton.Name = "okButton";
			this.okButton.TabIndex = 5;
			this.okButton.Text = resourceService.GetString("Global.OKButtonText");
			this.okButton.DialogResult = DialogResult.OK;
			okButton.FlatStyle = FlatStyle.System;
			
			// 
			// selectAllButton
			// 
			this.selectAllButton.Location = new System.Drawing.Point(168, 232);
			this.selectAllButton.Name = "selectAllButton";
			this.selectAllButton.TabIndex = 2;
			this.selectAllButton.Size = new Size(96, 23);
			this.selectAllButton.Text = resourceService.GetString("Dialog.IncludeFilesDialog.SelectAllButton");
			selectAllButton.FlatStyle = FlatStyle.System;
			
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.cancelButton.Location = new System.Drawing.Point(252, 308);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.TabIndex = 6;
			this.cancelButton.Text = resourceService.GetString("Global.CancelButtonText");
			this.cancelButton.DialogResult = DialogResult.Cancel;
			cancelButton.FlatStyle = FlatStyle.System;
			
			// 
			// Label1
			// 
			this.Label1.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.Label1.Location = new System.Drawing.Point(8, 8);
			this.Label1.Name = "Label1";
			this.Label1.Size = new System.Drawing.Size(316, 16);
			this.Label1.TabIndex = 0;
			this.Label1.Text = resourceService.GetString("Dialog.IncludeFilesDialog.IncludeFilesLabel");
			
			// 
			// Label2
			// 
			this.Label2.Anchor = ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.Label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.Label2.Location = new System.Drawing.Point(168, 298);
			this.Label2.Name = "Label2";
			this.Label2.Size = new System.Drawing.Size(154, 3);
			this.Label2.TabIndex = 4;
			// 
			// deselectAllButton
			// 
			this.deselectAllButton.Location = new System.Drawing.Point(168, 264);
			this.deselectAllButton.Name = "deselectAllButton";
			this.deselectAllButton.TabIndex = 3;
			this.deselectAllButton.Size = new Size(96, 23);
			this.deselectAllButton.Text = resourceService.GetString("Dialog.IncludeFilesDialog.DeselectAllButton");
			deselectAllButton.FlatStyle = FlatStyle.System;
			
			// 
			// Form1
			// 
			this.AcceptButton = this.cancelButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.okButton;
			this.ClientSize = new System.Drawing.Size(330, 335);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {this.GroupBox1,
			                       this.CheckedListBox1,
			                       this.okButton,
			                       this.selectAllButton,
			                       this.cancelButton,
			                       this.Label1,
			                       this.Label2,
			                       this.deselectAllButton});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "Form1";
			this.GroupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
	}
}
#endif
