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
using System.ComponentModel;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;

using ICSharpCode.Core.AddIns.Codons;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels.CompletionDatabaseWizard
{
	public class ChooseLocationPanel : AbstractWizardPanel
	{
		IProperties properties;
		static FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
		
		public override bool ReceiveDialogMessage(DialogMessage message)
		{
			if (message == DialogMessage.Cancel) {
				properties.SetProperty("SharpDevelop.CodeCompletion.DataDirectory", String.Empty);
			} else if (message == DialogMessage.Next || message == DialogMessage.OK) {
				string path = null;				
				if (((RadioButton)ControlDictionary["specifyLocationRadioButton"]).Checked) {
					path = ControlDictionary["locationTextBox"].Text.TrimEnd(Path.DirectorySeparatorChar);
				} else if (((RadioButton)ControlDictionary["sharpDevelopDirRadioButton"]).Checked) {
					FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
					PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
				
					path = propertyService.DataDirectory + 
					       Path.DirectorySeparatorChar + "CodeCompletionData"; 
				} else {
					PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
					path = propertyService.ConfigDirectory + "CodeCompletionTemp";
				}
				
				if (!Directory.Exists(path)) {
					Directory.CreateDirectory(path);
				}
				
				properties.SetProperty("SharpDevelop.CodeCompletion.DataDirectory",
				                       path);
			}
	    	return true;
		}
		
		void SetEnableStatus(object sender, EventArgs e)
		{
			ControlDictionary["browseButton"].Enabled = ControlDictionary["locationTextBox"].Enabled = ((RadioButton)ControlDictionary["specifyLocationRadioButton"]).Checked;
			SetFinishedState(sender, e);
		}
		
		void BrowseLocationEvent(object sender, EventArgs e)
		{
			FolderDialog fd = new FolderDialog();
			if (fd.DisplayDialog("choose the location in which you want the code completion files to be generated") == DialogResult.OK) {
				ControlDictionary["locationTextBox"].Text = fd.Path;
			}
		}
		
		void SetFinishedState(object sender, EventArgs e)
		{
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			EnableFinish = EnableNext = !((RadioButton)ControlDictionary["specifyLocationRadioButton"]).Checked ||
  			                            (fileUtilityService.IsValidFileName(ControlDictionary["locationTextBox"].Text) && 
  			                            Directory.Exists(ControlDictionary["locationTextBox"].Text));
		}
		
		void SetValues(object sender, EventArgs e)
		{
			properties = (IProperties)CustomizationObject;
		}
		
		static PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		public ChooseLocationPanel() : base(propertyService.DataDirectory + @"\resources\panels\CompletionDatabaseWizard\CreateNewFilePanel.xfrm")
		{
			NextWizardPanelID = "CreateDatabasePanel";
			
			ControlDictionary["browseButton"].Click          += new EventHandler(BrowseLocationEvent);
			ControlDictionary["locationTextBox"].TextChanged += new EventHandler(SetFinishedState);
			
			((RadioButton)ControlDictionary["appDirRadioButton"]).CheckedChanged          += new EventHandler(SetEnableStatus);
			((RadioButton)ControlDictionary["sharpDevelopDirRadioButton"]).CheckedChanged += new EventHandler(SetEnableStatus);
			((RadioButton)ControlDictionary["specifyLocationRadioButton"]).CheckedChanged += new EventHandler(SetEnableStatus);
			
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			((TextBox)ControlDictionary["textBox"]).Lines = resourceService.GetString("Dialog.Wizards.CodeCompletionDatabaseWizard.ChooseLocationPanel.DescriptionText").Split('\n');
			
			SetFinishedState(this, EventArgs.Empty);
			SetEnableStatus(this, EventArgs.Empty);
			CustomizationObjectChanged += new EventHandler(SetValues);
		}
	}
}
