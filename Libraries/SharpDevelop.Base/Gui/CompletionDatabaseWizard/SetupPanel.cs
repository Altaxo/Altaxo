// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
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
	public class SetupPanel : AbstractWizardPanel
	{
		static FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
		void SetSuccessor(object sender, EventArgs e)
		{
			IsLastPanel = ((RadioButton)ControlDictionary["skipCreationRadioButton"]).Checked;
			
			if (((RadioButton)ControlDictionary["createNewRadioButton"]).Checked) {
				NextWizardPanelID = "ChooseLocationPanel";
			} else if (((RadioButton)ControlDictionary["useExistingRadioButton"]).Checked) {
				NextWizardPanelID = "UseExistingFilePanel";
			}
		}
		
		static PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		
		public SetupPanel() : base(propertyService.DataDirectory + @"\resources\panels\CompletionDatabaseWizard\WelcomePanel.xfrm")
		{
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			((TextBox)ControlDictionary["textBox"]).Lines = resourceService.GetString("Dialog.Wizards.CodeCompletionDatabaseWizard.SetupPanel.DescriptionText").Split('\n');
			
			((RadioButton)ControlDictionary["skipCreationRadioButton"]).CheckedChanged += new EventHandler(SetSuccessor);
			((RadioButton)ControlDictionary["createNewRadioButton"]).CheckedChanged += new EventHandler(SetSuccessor);
			((RadioButton)ControlDictionary["useExistingRadioButton"]).CheckedChanged += new EventHandler(SetSuccessor);
		}
	}
}
