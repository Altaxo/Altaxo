// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using MSjogren.GacTool.FusionNative;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs
{
	public class AssemblyReferencePanel : Panel, IReferencePanel
	{
		SelectReferenceDialog selectDialog;
		
		public AssemblyReferencePanel(SelectReferenceDialog selectDialog)
		{
			this.selectDialog = selectDialog;
			
			Button browseButton   = new Button();
			browseButton.Location = new Point(10, 10);
			browseButton.Text     = "Browse";
			browseButton.Click   += new EventHandler(SelectReferenceDialog);
			Controls.Add(browseButton);
		}
		
		void SelectReferenceDialog(object sender, EventArgs e)
		{
			using (OpenFileDialog fdiag  = new OpenFileDialog()) {
				fdiag.AddExtension    = true;
				StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
				fdiag.Filter = stringParserService.Parse("${res:SharpDevelop.FileFilter.AssemblyFiles}|*.dll;*.exe|${res:SharpDevelop.FileFilter.AllFiles}|*.*");
				fdiag.Multiselect     = true;
				fdiag.CheckFileExists = true;
				
				if (fdiag.ShowDialog() == DialogResult.OK) {
					foreach (string file in fdiag.FileNames) {
						selectDialog.AddReference(ReferenceType.Assembly,
						                          Path.GetFileName(file),
						                          file);
						
					}
				}
			}
		}
		
		public void AddReference(object sender, EventArgs e)
		{
			MessageBox.Show("This panel will contain a file browser, but so long use the browse button :)");
		}
	}
}
