// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.Core.Services;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;

namespace CSharpBinding
{
	public class OutputOptionsPanel : AbstractOptionPanel
	{
		CSharpCompilerParameters compilerParameters;
		
		public override void LoadPanelContents()
		{
			SetupFromXml(Path.Combine(PropertyService.DataDirectory, 
			                          @"resources\panels\ProjectOptions\OutputPanel.xfrm"));
			
			ControlDictionary["browseButton"].Click += new EventHandler(SelectFolder);
			ControlDictionary["browseButton2"].Click += new EventHandler(SelectFile2);
			ControlDictionary["browseButton3"].Click += new EventHandler(SelectFile3);
			ControlDictionary["browseButton4"].Click += new EventHandler(SelectFile4);
			
			this.compilerParameters = (CSharpCompilerParameters)((IProperties)CustomizationObject).GetProperty("Config");
			
			Console.WriteLine("SET BLABLUB");
			ControlDictionary["assemblyNameTextBox"].Text    = compilerParameters.OutputAssembly;
			ControlDictionary["outputDirectoryTextBox"].Text = compilerParameters.OutputDirectory;
			ControlDictionary["parametersTextBox"].Text      = compilerParameters.CommandLineParameters;
			ControlDictionary["executeScriptTextBox"].Text   = compilerParameters.ExecuteScript;
			ControlDictionary["executeBeforeTextBox"].Text   = compilerParameters.ExecuteBeforeBuild;
			ControlDictionary["executeAfterTextBox"].Text    = compilerParameters.ExecuteAfterBuild;
			
			((CheckBox)ControlDictionary["pauseConsoleOutputCheckBox"]).Checked = compilerParameters.PauseConsoleOutput;
		}
		
		public override bool StorePanelContents()
		{
			Console.WriteLine("store contents");
			
			if (compilerParameters == null) {
				return true;
			}
			
			Console.WriteLine("1");
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			if (!fileUtilityService.IsValidFileName(ControlDictionary["assemblyNameTextBox"].Text)) {
				MessageService.ShowError("Invalid assembly name specified");
				return false;
			}
			if (!fileUtilityService.IsValidFileName(ControlDictionary["outputDirectoryTextBox"].Text)) {
				MessageService.ShowError("Invalid output directory specified");
				return false;
			}
			
			Console.WriteLine("2");
			compilerParameters.OutputAssembly        = ControlDictionary["assemblyNameTextBox"].Text;
			compilerParameters.OutputDirectory       = ControlDictionary["outputDirectoryTextBox"].Text;
			compilerParameters.CommandLineParameters = ControlDictionary["parametersTextBox"].Text;
			compilerParameters.ExecuteBeforeBuild    = ControlDictionary["executeBeforeTextBox"].Text;
			compilerParameters.ExecuteAfterBuild     = ControlDictionary["executeAfterTextBox"].Text;
			compilerParameters.ExecuteScript         = ControlDictionary["executeScriptTextBox"].Text;
			
			compilerParameters.PauseConsoleOutput = ((CheckBox)ControlDictionary["pauseConsoleOutputCheckBox"]).Checked;
			return true;
		}
		
		void SelectFolder(object sender, EventArgs e)
		{
			FolderDialog fdiag = new  FolderDialog();
			
			if (fdiag.DisplayDialog("${res:Dialog.Options.PrjOptions.Configuration.FolderBrowserDescription}") == DialogResult.OK) {
				ControlDictionary["outputDirectoryTextBox"].Text = fdiag.Path;
			}
		}
		
		void SelectFile2(object sender, EventArgs e)
		{
			OpenFileDialog fdiag = new OpenFileDialog();
			fdiag.Filter      = StringParserService.Parse("${res:SharpDevelop.FileFilter.AllFiles}|*.*");
			fdiag.Multiselect = false;
			
			if(fdiag.ShowDialog() == DialogResult.OK) {
				ControlDictionary["executeBeforeTextBox"].Text = fdiag.FileName;
			}
		}
		
		void SelectFile3(object sender, EventArgs e)
		{
			OpenFileDialog fdiag = new OpenFileDialog();
			fdiag.Filter      = StringParserService.Parse("${res:SharpDevelop.FileFilter.AllFiles}|*.*");
			fdiag.Multiselect = false;
			
			if(fdiag.ShowDialog() == DialogResult.OK) {
				ControlDictionary["executeAfterTextBox"].Text = fdiag.FileName;
			}
		}
		
		void SelectFile4(object sender, EventArgs e)
		{
			OpenFileDialog fdiag = new OpenFileDialog();
			fdiag.Filter      = StringParserService.Parse("${res:SharpDevelop.FileFilter.AllFiles}|*.*");
			fdiag.Multiselect = false;
			
			if(fdiag.ShowDialog() == DialogResult.OK) {
				ControlDictionary["executeScriptTextBox"].Text = fdiag.FileName;
			}
		}
		
		
	}

}
