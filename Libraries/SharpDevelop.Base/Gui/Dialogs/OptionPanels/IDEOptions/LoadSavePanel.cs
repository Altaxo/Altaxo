// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.Core.AddIns.Codons;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels
{
	public enum LineTerminatorStyle {
		Windows,
		Macintosh,
		Unix
	}
	
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class LoadSavePanel : AbstractOptionPanel
	{
		const string loadUserDataCheckBox        = "loadUserDataCheckBox";
		const string createBackupCopyCheckBox    = "createBackupCopyCheckBox";
		const string lineTerminatorStyleComboBox = "lineTerminatorStyleComboBox";
		
		public override void LoadPanelContents()
		{
			SetupFromXml(Path.Combine(PropertyService.DataDirectory, 
			                          @"resources\panels\LoadSaveOptionPanel.xfrm"));
			
			((CheckBox)ControlDictionary[loadUserDataCheckBox]).Checked     = PropertyService.GetProperty("SharpDevelop.LoadDocumentProperties", true);
			((CheckBox)ControlDictionary[createBackupCopyCheckBox]).Checked = PropertyService.GetProperty("SharpDevelop.CreateBackupCopy", false);
			
			((ComboBox)ControlDictionary[lineTerminatorStyleComboBox]).Items.Add(StringParserService.Parse("${res:Dialog.Options.IDEOptions.LoadSaveOptions.WindowsRadioButton}"));
			((ComboBox)ControlDictionary[lineTerminatorStyleComboBox]).Items.Add(StringParserService.Parse("${res:Dialog.Options.IDEOptions.LoadSaveOptions.MacintoshRadioButton}"));
			((ComboBox)ControlDictionary[lineTerminatorStyleComboBox]).Items.Add(StringParserService.Parse("${res:Dialog.Options.IDEOptions.LoadSaveOptions.UnixRadioButton}"));
			
			((ComboBox)ControlDictionary[lineTerminatorStyleComboBox]).SelectedIndex = (int)(LineTerminatorStyle)PropertyService.GetProperty("SharpDevelop.LineTerminatorStyle", LineTerminatorStyle.Windows);
		}
		
		public override bool StorePanelContents()
		{
			PropertyService.SetProperty("SharpDevelop.LoadDocumentProperties", ((CheckBox)ControlDictionary[loadUserDataCheckBox]).Checked);
			PropertyService.SetProperty("SharpDevelop.CreateBackupCopy",       ((CheckBox)ControlDictionary[createBackupCopyCheckBox]).Checked);
			PropertyService.SetProperty("SharpDevelop.LineTerminatorStyle",    (LineTerminatorStyle)((ComboBox)ControlDictionary[lineTerminatorStyleComboBox]).SelectedIndex);
			
			return true;
		}
	}
}
