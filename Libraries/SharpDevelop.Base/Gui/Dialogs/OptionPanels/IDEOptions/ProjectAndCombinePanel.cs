// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.Core.Services;
using ICSharpCode.Core.AddIns;
using ICSharpCode.SharpDevelop.Services;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels
{
	public class ProjectAndCombinePanel : AbstractOptionPanel
	{
		public override void LoadPanelContents()
		{
			SetupFromXml(Path.Combine(PropertyService.DataDirectory, 
			                          @"resources\panels\ProjectAndCombineOptions.xfrm"));
			
			// read properties
			ControlDictionary["projectLocationTextBox"].Text = PropertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.Dialogs.NewProjectDialog.DefaultPath", Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), 
			                                                                                                                                                                 "SharpDevelop Projects")).ToString();
			
			BeforeCompileAction action = (BeforeCompileAction)PropertyService.GetProperty("SharpDevelop.Services.DefaultParserService.BeforeCompileAction", BeforeCompileAction.SaveAllFiles);
			
			((RadioButton)ControlDictionary["saveChangesRadioButton"]).Checked = action == BeforeCompileAction.SaveAllFiles;
			((RadioButton)ControlDictionary["promptChangesRadioButton"]).Checked = action == BeforeCompileAction.PromptForSave;
			((RadioButton)ControlDictionary["noSaveRadioButton"]).Checked = action == BeforeCompileAction.Nothing;
			
			((CheckBox)ControlDictionary["loadPrevProjectCheckBox"]).Checked = (bool)PropertyService.GetProperty("SharpDevelop.LoadPrevProjectOnStartup", false);

			((CheckBox)ControlDictionary["showTaskListCheckBox"]).Checked = (bool)PropertyService.GetProperty("SharpDevelop.ShowTaskListAfterBuild", true);
			((CheckBox)ControlDictionary["showOutputCheckBox"]).Checked = (bool)PropertyService.GetProperty("SharpDevelop.ShowOutputWindowAtBuild", true);
			
			((Button)ControlDictionary["selectProjectLocationButton"]).Click += new EventHandler(SelectProjectLocationButtonClicked);
		}
		
		public override bool StorePanelContents()
		{
			// check for correct settings
			string projectPath = ControlDictionary["projectLocationTextBox"].Text;
			if (projectPath.Length > 0) {
				if (!FileUtilityService.IsValidFileName(projectPath)) {
					MessageService.ShowError("Invalid project path specified");
					return false;
				}
			}
			
			// set properties
			PropertyService.SetProperty("ICSharpCode.SharpDevelop.Gui.Dialogs.NewProjectDialog.DefaultPath", projectPath);
			
			if (((RadioButton)ControlDictionary["saveChangesRadioButton"]).Checked) {
				PropertyService.SetProperty("SharpDevelop.Services.DefaultParserService.BeforeCompileAction", BeforeCompileAction.SaveAllFiles);
			} else if (((RadioButton)ControlDictionary["promptChangesRadioButton"]).Checked) {
				PropertyService.SetProperty("SharpDevelop.Services.DefaultParserService.BeforeCompileAction", BeforeCompileAction.PromptForSave);
			} else if (((RadioButton)ControlDictionary["noSaveRadioButton"]).Checked) {
				PropertyService.SetProperty("SharpDevelop.Services.DefaultParserService.BeforeCompileAction", BeforeCompileAction.Nothing);
			}
			
			PropertyService.SetProperty("SharpDevelop.LoadPrevProjectOnStartup", ((CheckBox)ControlDictionary["loadPrevProjectCheckBox"]).Checked);
			
			PropertyService.SetProperty("SharpDevelop.ShowTaskListAfterBuild", ((CheckBox)ControlDictionary["showTaskListCheckBox"]).Checked);
			PropertyService.SetProperty("SharpDevelop.ShowOutputWindowAtBuild", ((CheckBox)ControlDictionary["showOutputCheckBox"]).Checked);
			
			return true;
		}
		
		void SelectProjectLocationButtonClicked(object sender, EventArgs e)
		{
			FolderDialog fdiag = new  FolderDialog();
			if (fdiag.DisplayDialog("Select default combile location") == DialogResult.OK) {
				ControlDictionary["projectLocationTextBox"].Text = fdiag.Path;
			}
		}
	}
}
