// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
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
	public class CodeGenerationPanel : AbstractOptionPanel
	{
		CSharpCompilerParameters compilerParameters = null;
		
		public override void LoadPanelContents()
		{
			SetupFromXml(Path.Combine(PropertyService.DataDirectory, 
			                          @"resources\panels\ProjectOptions\CodeGenerationPanel.xfrm"));
			
			((ComboBox)ControlDictionary["compileTargetComboBox"]).Items.Add(StringParserService.Parse("${res:Dialog.Options.PrjOptions.Configuration.CompileTarget.Exe}"));
			((ComboBox)ControlDictionary["compileTargetComboBox"]).Items.Add(StringParserService.Parse("${res:Dialog.Options.PrjOptions.Configuration.CompileTarget.WinExe}"));
			((ComboBox)ControlDictionary["compileTargetComboBox"]).Items.Add(StringParserService.Parse("${res:Dialog.Options.PrjOptions.Configuration.CompileTarget.Library}"));
			((ComboBox)ControlDictionary["compileTargetComboBox"]).Items.Add(StringParserService.Parse("${res:Dialog.Options.PrjOptions.Configuration.CompileTarget.Module}"));
			
			ControlDictionary["browseWin32IconButton"].Click += new EventHandler(SelectWin32Icon);
			
			this.compilerParameters = (CSharpCompilerParameters)((IProperties)CustomizationObject).GetProperty("Config");
			
			((ComboBox)ControlDictionary["compileTargetComboBox"]).SelectedIndex = (int)compilerParameters.CompileTarget;
			ControlDictionary["symbolsTextBox"].Text   = compilerParameters.DefineSymbols;
			ControlDictionary["mainClassTextBox"].Text = compilerParameters.MainClass;
			ControlDictionary["win32IconTextBox"].Text = compilerParameters.Win32Icon;

			((CheckBox)ControlDictionary["generateDebugInformationCheckBox"]).Checked = compilerParameters.Debugmode;
			((CheckBox)ControlDictionary["generateXmlOutputCheckBox"]).Checked        = compilerParameters.GenerateXmlDocumentation;
			((CheckBox)ControlDictionary["enableOptimizationCheckBox"]).Checked       = compilerParameters.Optimize;
			((CheckBox)ControlDictionary["allowUnsafeCodeCheckBox"]).Checked       = compilerParameters.UnsafeCode;
			((CheckBox)ControlDictionary["generateOverflowChecksCheckBox"]).Checked       = compilerParameters.GenerateOverflowChecks;
			((CheckBox)ControlDictionary["warningsAsErrorsCheckBox"]).Checked       = !compilerParameters.RunWithWarnings;
			
			((NumericUpDown)ControlDictionary["warningLevelNumericUpDown"]).Value = compilerParameters.WarningLevel;
		}
		
		public override bool StorePanelContents()
		{
			if (compilerParameters == null) {
				return true;
			}
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			
			if (ControlDictionary["win32IconTextBox"].Text.Length > 0) {
				if (!fileUtilityService.IsValidFileName(ControlDictionary["win32IconTextBox"].Text)) {
					MessageBox.Show("Invalid Win32Icon specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
					return false;
				}
				if (!File.Exists(ControlDictionary["win32IconTextBox"].Text)) {
					MessageBox.Show("Win32Icon doesn't exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
					return false;
				}
			}
			
			compilerParameters.CompileTarget = (CompileTarget)((ComboBox)ControlDictionary["compileTargetComboBox"]).SelectedIndex;
			compilerParameters.DefineSymbols = ControlDictionary["symbolsTextBox"].Text;
			compilerParameters.MainClass     = ControlDictionary["mainClassTextBox"].Text;
			compilerParameters.Win32Icon     = ControlDictionary["win32IconTextBox"].Text;
			
			compilerParameters.Debugmode = ((CheckBox)ControlDictionary["generateDebugInformationCheckBox"]).Checked;
			compilerParameters.GenerateXmlDocumentation = ((CheckBox)ControlDictionary["generateXmlOutputCheckBox"]).Checked;
			compilerParameters.Optimize = ((CheckBox)ControlDictionary["enableOptimizationCheckBox"]).Checked;
			compilerParameters.UnsafeCode = ((CheckBox)ControlDictionary["allowUnsafeCodeCheckBox"]).Checked;
			compilerParameters.GenerateOverflowChecks = ((CheckBox)ControlDictionary["generateOverflowChecksCheckBox"]).Checked;
			compilerParameters.RunWithWarnings  = !((CheckBox)ControlDictionary["warningsAsErrorsCheckBox"]).Checked;
			compilerParameters.WarningLevel = (int)((NumericUpDown)ControlDictionary["warningLevelNumericUpDown"]).Value;
			return true;
		}
		
		void SelectWin32Icon(object sender, EventArgs e) 
		{
			using (OpenFileDialog fdiag  = new OpenFileDialog()) {
				fdiag.AddExtension    = true;
				fdiag.Filter          = StringParserService.Parse("Icons (*.ico)|*.ico|${res:SharpDevelop.FileFilter.AllFiles}|*.*");
				fdiag.Multiselect     = false;
				fdiag.CheckFileExists = true;
				
				if (fdiag.ShowDialog() == DialogResult.OK) {
					ControlDictionary["win32IconTextBox"].Text = fdiag.FileName;
				}
			}
		}
	}
}
