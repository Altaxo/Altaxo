// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Resources;
using System.IO;
using System.Text;
using System.Reflection;
using System.Runtime.CompilerServices;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core.Properties;

using ICSharpCode.Core.Services;

using ICSharpCode.SharpDevelop.Internal.Project.Collections;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Services;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs
{
	public class AboutSharpDevelopTabPage : UserControl
	{
		Label      buildLabel   = new Label();
		TextBox    buildTextBox = new TextBox();
		
		Label      versionLabel   = new Label();
		TextBox    versionTextBox = new TextBox();
		
		Label      sponsorLabel   = new Label();
		
		public AboutSharpDevelopTabPage()
		{
			Version v = Assembly.GetEntryAssembly().GetName().Version;
			versionTextBox.Text = v.Major + "." + v.Minor;
			buildTextBox.Text   = v.Revision + "." + v.Build;
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			versionLabel.Location = new System.Drawing.Point(8, 8);
			versionLabel.Text = resourceService.GetString("Dialog.About.label1Text");
			versionLabel.Size = new System.Drawing.Size(64, 16);
			versionLabel.TabIndex = 1;
			Controls.Add(versionLabel);
			
			versionTextBox.Location = new System.Drawing.Point(64 + 8 + 4, 8);
			versionTextBox.ReadOnly = true;
			versionTextBox.TabIndex = 4;
			versionTextBox.Size = new System.Drawing.Size(48, 20);
			Controls.Add(versionTextBox);
			
			buildLabel.Location = new System.Drawing.Point(64 + 12 + 48 + 4, 8);
			buildLabel.Text = resourceService.GetString("Dialog.About.label2Text");
			buildLabel.Size = new System.Drawing.Size(48, 16);
			buildLabel.TabIndex = 2;
			Controls.Add(buildLabel);
			
			buildTextBox.Location = new System.Drawing.Point(64 + 12 + 48 + 4 + 48 + 4, 8);
			buildTextBox.ReadOnly = true;
			buildTextBox.TabIndex = 3;
			buildTextBox.Size = new System.Drawing.Size(72, 20);
			Controls.Add(buildTextBox);
			
			sponsorLabel.Location = new System.Drawing.Point(8, 34);
			sponsorLabel.Text = "Released under the GNU General Public license.\n\n" + 
				                "Sponsored by AlphaSierraPapa\n" +
			                    "                   http://www.AlphaSierraPapa.com";
			sponsorLabel.Size = new System.Drawing.Size(362, 74);
			sponsorLabel.TabIndex = 8;
			Controls.Add(sponsorLabel);
			Dock = DockStyle.Fill;
		}
	}
	
	public class AuthorAboutTabPage : ICSharpCode.SharpDevelop.Gui.HtmlControl.HtmlControl
	{
		public AuthorAboutTabPage()
		{
			try {
				FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
				PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
				
				string html = ConvertXml.ConvertToString(fileUtilityService.SharpDevelopRootPath +
				                   Path.DirectorySeparatorChar + "doc" +
				                   Path.DirectorySeparatorChar + "AUTHORS.xml",
				                   
				                   propertyService.DataDirectory +
				                   Path.DirectorySeparatorChar + "ConversionStyleSheets" + 
				                   Path.DirectorySeparatorChar + "ShowAuthors.xsl");
				
				Dock = DockStyle.Fill;
				
				base.CascadingStyleSheet = propertyService.DataDirectory + Path.DirectorySeparatorChar +
				                           "resources" + Path.DirectorySeparatorChar +
				                           "css" + Path.DirectorySeparatorChar +
				                           "SharpDevelopStandard.css";
				base.Html = html;
			} catch (Exception e) {
				IMessageService messageService = (IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
				messageService.ShowError(e);
			}
		}
	}
	
	public class ChangeLogTabPage : ICSharpCode.SharpDevelop.Gui.HtmlControl.HtmlControl
	{
		public ChangeLogTabPage()
		{
			try {
				FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
				PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
				
				string html = ConvertXml.ConvertToString(fileUtilityService.SharpDevelopRootPath +
				                   Path.DirectorySeparatorChar + "doc" +
				                   Path.DirectorySeparatorChar + "ChangeLog.xml",
				                   
				                   propertyService.DataDirectory +
				                   Path.DirectorySeparatorChar + "ConversionStyleSheets" + 
				                   Path.DirectorySeparatorChar + "ShowChangeLog.xsl");
				
				Dock = DockStyle.Fill;
				
				base.CascadingStyleSheet = propertyService.DataDirectory + Path.DirectorySeparatorChar +
				                           "resources" + Path.DirectorySeparatorChar +
				                           "css" + Path.DirectorySeparatorChar +
				                           "SharpDevelopStandard.css";
				base.Html = html;
			} catch (Exception e) {
				IMessageService messageService = (IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
				messageService.ShowError(e);
			}
		}
	}
	
	public class VersionInformationTabPage : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ListView listView;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.Button button;
		private System.Windows.Forms.ColumnHeader columnHeader;
		
		public VersionInformationTabPage()
		{
			InitializeComponent();
			Dock = DockStyle.Fill;
			FillListView();
		}
		
		void FillListView()
		{
			listView.BeginUpdate();
			foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies()) {
				AssemblyName name = asm.GetName();
				ListViewItem newItem = new ListViewItem(name.Name);
				newItem.SubItems.Add(name.Version.ToString());
				try {
					newItem.SubItems.Add(asm.Location);
				} catch (Exception) {
					newItem.SubItems.Add("dynamic");
				}
				
				listView.Items.Add(newItem);
			}
			listView.EndUpdate();
		}
		
		void CopyButtonClick(object sender, EventArgs e)
		{
			StringBuilder versionInfo = new StringBuilder();
			foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies()) {
				AssemblyName name = asm.GetName();
				versionInfo.Append(name.Name);
				versionInfo.Append(",");
				versionInfo.Append(name.Version.ToString());
				versionInfo.Append(",");
				try {
					versionInfo.Append(asm.Location);
				} catch (Exception) {
					versionInfo.Append("dynamic");
				}
				
				versionInfo.Append(Environment.NewLine);
			}
			
			Clipboard.SetDataObject(new DataObject(System.Windows.Forms.DataFormats.Text, versionInfo.ToString()), true);
		}
		
		// THIS METHOD IS MAINTAINED BY THE FORM DESIGNER
		// DO NOT EDIT IT MANUALLY! YOUR CHANGES ARE LIKELY TO BE LOST
		void InitializeComponent() {
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			this.columnHeader = new System.Windows.Forms.ColumnHeader();
			this.button = new System.Windows.Forms.Button();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.listView = new System.Windows.Forms.ListView();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.SuspendLayout();
			
			// 
			// columnHeader
			// 
			this.columnHeader.Text = resourceService.GetString("Dialog.About.VersionInfoTabName.NameColumn");
			this.columnHeader.Width = 130;
			
			// 
			// button
			// 
			this.button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.button.Location = new System.Drawing.Point(8, 184);
			this.button.Name = "button";
			this.button.TabIndex = 1;
			this.button.Text = resourceService.GetString("Dialog.About.VersionInfoTabName.CopyButton");
			this.button.Click += new EventHandler(CopyButtonClick);
			this.button.FlatStyle = FlatStyle.System;
			
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = resourceService.GetString("Dialog.About.VersionInfoTabName.VersionColumn");
			this.columnHeader2.Width = 100;
			
			// 
			// listView
			// 
			this.listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
						| System.Windows.Forms.AnchorStyles.Left) 
						| System.Windows.Forms.AnchorStyles.Right)));
			this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
						this.columnHeader,
						this.columnHeader2,
						this.columnHeader3});
			this.listView.FullRowSelect = true;
			this.listView.GridLines = true;
			this.listView.Sorting   = SortOrder.Ascending;
			this.listView.Location = new System.Drawing.Point(0, 0);
			this.listView.Name = "listView";
			this.listView.Size = new System.Drawing.Size(248, 176);
			this.listView.TabIndex = 0;
			this.listView.View = System.Windows.Forms.View.Details;
			
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = resourceService.GetString("Dialog.About.VersionInfoTabName.PathColumn");
			this.columnHeader3.Width = 150;
			
			//
			// CreatedUserControl
			// 
			this.Controls.Add(this.button);
			this.Controls.Add(this.listView);
			this.Name = "CreatedUserControl";
			this.Size = new System.Drawing.Size(248, 216);
			this.ResumeLayout(false);
		}
	}
}
