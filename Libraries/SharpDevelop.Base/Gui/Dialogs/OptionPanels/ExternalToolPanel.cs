// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;
using ICSharpCode.Core.AddIns.Codons;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels
{
	public class ExternalToolPane : AbstractOptionPanel
	{
		
		static string[,] argumentQuickInsertMenu = new string[,] {
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.FullItemPath}",      "${ItemPath}"},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.FullItemDirectory}", "${ItemDir}"},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.ItemFileName}",      "${ItemFileName}"},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.ItemExtension}",     "${ItemExt}"},
			{"-", ""},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.CurrentLine}",   "${CurLine}"},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.CurrentColumn}", "${CurCol}"},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.CurrentText}",   "${CurText}"},
			{"-", ""},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.FullTargetPath}",  "${TargetPath}"},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.TargetDirectory}", "${TargetDir}"},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.TargetName}",      "${TargetName}"},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.TargetExtension}", "${TargetExt}"},
			{"-", ""},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.ProjectDirectory}", "${ProjectDir}"},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.ProjectFileName}",  "${ProjectFileName}"},
			{"-", ""},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.CombineDirectory}", "${CombineDir}"},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.CombineFileName}",  "${CombineFileName}"},
			{"-", ""},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.SharpDevelopStartupPath}",  "${StartupPath}"},
		};
		
		static string[,] workingDirInsertMenu = new string[,] {
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.FullItemDirectory}", "${ItemDir}"},
			{"-", ""},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.TargetDirectory}", "${TargetDir}"},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.TargetName}",      "${TargetName}"},
			{"-", ""},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.ProjectDirectory}", "${ProjectDir}"},
			{"-", ""},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.CombineDirectory}", "${CombineDir}"},
			{"-", ""},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.SharpDevelopStartupPath}",  "${StartupPath}"},
		};
		
		// these are the control names which are enabled/disabled depending if tool is selected
		static string[] dependendControlNames = new string[] {
			"titleTextBox", "commandTextBox", "argumentTextBox", 
			"workingDirTextBox", "promptArgsCheckBox", "useOutputPadCheckBox", 
			"titleLabel", "argumentLabel", "commandLabel", 
			"workingDirLabel", "browseButton", "argumentQuickInsertButton", 
			"workingDirQuickInsertButton", "moveUpButton", "moveDownButton"
		};
		
		public override void LoadPanelContents()
		{
			SetupFromXml(Path.Combine(PropertyService.DataDirectory, 
			                          @"resources\panels\ExternalToolOptions.xfrm"));
			
			((ListBox)ControlDictionary["toolListBox"]).BeginUpdate();
			try {
				foreach (object o in ToolLoader.Tool) {
					((ListBox)ControlDictionary["toolListBox"]).Items.Add(o);
				}
			} finally {
				((ListBox)ControlDictionary["toolListBox"]).EndUpdate();
			}
			
			MenuService.CreateQuickInsertMenu((TextBox)ControlDictionary["argumentTextBox"],
			                                  ControlDictionary["argumentQuickInsertButton"],
			                                  argumentQuickInsertMenu);
			
			MenuService.CreateQuickInsertMenu((TextBox)ControlDictionary["workingDirTextBox"],
			                                  ControlDictionary["workingDirQuickInsertButton"],
			                                  workingDirInsertMenu);
			
			((ListBox)ControlDictionary["toolListBox"]).SelectedIndexChanged += new EventHandler(selectEvent);
			ControlDictionary["removeButton"].Click   += new EventHandler(removeEvent);
			ControlDictionary["addButton"].Click      += new EventHandler(addEvent);
			ControlDictionary["moveUpButton"].Click   += new EventHandler(moveUpEvent);
			ControlDictionary["moveDownButton"].Click += new EventHandler(moveDownEvent);
			
			ControlDictionary["browseButton"].Click   += new EventHandler(browseEvent);
			
			
			selectEvent(this, EventArgs.Empty);
		}
		
		void browseEvent(object sender, EventArgs e)
		{
			using (OpenFileDialog fdiag  = new OpenFileDialog()) {
				fdiag.CheckFileExists = true;
				fdiag.Filter = StringParserService.Parse("${res:SharpDevelop.FileFilter.ExecutableFiles}|*.exe;*.com;*.pif;*.bat;*.cmd|${res:SharpDevelop.FileFilter.AllFiles}|*.*");
				
				if (fdiag.ShowDialog() == DialogResult.OK) {
					ControlDictionary["commandTextBox"].Text = fdiag.FileName;
				}
			}
		}
		
		
		void moveUpEvent(object sender, EventArgs e)
		{
			int index = ((ListBox)ControlDictionary["toolListBox"]).SelectedIndex;
			if (index > 0) {
				((ListBox)ControlDictionary["toolListBox"]).SelectedIndexChanged -= new EventHandler(selectEvent);
				try {
					object tmp = ((ListBox)ControlDictionary["toolListBox"]).Items[index - 1];
					((ListBox)ControlDictionary["toolListBox"]).Items[index - 1] = ((ListBox)ControlDictionary["toolListBox"]).Items[index];
					((ListBox)ControlDictionary["toolListBox"]).Items[index] = tmp;
					((ListBox)ControlDictionary["toolListBox"]).SetSelected(index, false);
					((ListBox)ControlDictionary["toolListBox"]).SetSelected(index - 1, true);
				} finally {
					((ListBox)ControlDictionary["toolListBox"]).SelectedIndexChanged += new EventHandler(selectEvent);
				}
			}
			
		}
		void moveDownEvent(object sender, EventArgs e)
		{
			int index = ((ListBox)ControlDictionary["toolListBox"]).SelectedIndex;
			if (index >= 0 && index < ((ListBox)ControlDictionary["toolListBox"]).Items.Count - 1) {
				((ListBox)ControlDictionary["toolListBox"]).SelectedIndexChanged -= new EventHandler(selectEvent);
				try {
					object tmp = ((ListBox)ControlDictionary["toolListBox"]).Items[index + 1];
					((ListBox)ControlDictionary["toolListBox"]).Items[index + 1] = ((ListBox)ControlDictionary["toolListBox"]).Items[index];
					((ListBox)ControlDictionary["toolListBox"]).Items[index] = tmp;
					((ListBox)ControlDictionary["toolListBox"]).SetSelected(index, false);
					((ListBox)ControlDictionary["toolListBox"]).SetSelected(index + 1, true);
				} finally {
					((ListBox)ControlDictionary["toolListBox"]).SelectedIndexChanged += new EventHandler(selectEvent);
				}
			}
		}
		
		public override bool StorePanelContents()
		{
			ArrayList newlist = new ArrayList();
			foreach (ExternalTool tool in ((ListBox)ControlDictionary["toolListBox"]).Items) {
				if (!FileUtilityService.IsValidFileName(tool.Command)) {
					MessageService.ShowError(String.Format("The command of tool \"{0}\" is invalid.", tool.MenuCommand));
					return false;
				}
				if ((tool.InitialDirectory != "") && (!FileUtilityService.IsValidFileName(tool.InitialDirectory))) {
					MessageService.ShowError(String.Format("The working directory of tool \"{0}\" is invalid.", tool.MenuCommand));
					return false;
				}
				newlist.Add(tool);
			}
			
			ToolLoader.Tool = newlist;
			ToolLoader.SaveTools();
			return true;
		}
		
		void propertyValueChanged(object sender, PropertyValueChangedEventArgs e)
		{
			foreach (ListViewItem item in ((ListView)ControlDictionary["toolListView"]).Items) {
				if (item.Tag != null) {
					item.Text = item.Tag.ToString();
				}
			}
			
		}
		
		void setToolValues(object sender, EventArgs e)
		{
			ExternalTool selectedItem = ((ListBox)ControlDictionary["toolListBox"]).SelectedItem as ExternalTool;
			
			selectedItem.MenuCommand        = ControlDictionary["titleTextBox"].Text;
			selectedItem.Command            = ControlDictionary["commandTextBox"].Text;
			selectedItem.Arguments          = ControlDictionary["argumentTextBox"].Text;
			selectedItem.InitialDirectory   = ControlDictionary["workingDirTextBox"].Text;
			selectedItem.PromptForArguments = ((CheckBox)ControlDictionary["promptArgsCheckBox"]).Checked;
			selectedItem.UseOutputPad       = ((CheckBox)ControlDictionary["useOutputPadCheckBox"]).Checked;
		}
		
		void selectEvent(object sender, EventArgs e)
		{
			SetEnabledStatus(((ListBox)ControlDictionary["toolListBox"]).SelectedItems.Count > 0, "removeButton");
			
			ControlDictionary["titleTextBox"].TextChanged      -= new EventHandler(setToolValues);
			ControlDictionary["commandTextBox"].TextChanged    -= new EventHandler(setToolValues);
			ControlDictionary["argumentTextBox"].TextChanged   -= new EventHandler(setToolValues);
			ControlDictionary["workingDirTextBox"].TextChanged   -= new EventHandler(setToolValues);
			((CheckBox)ControlDictionary["promptArgsCheckBox"]).CheckedChanged   -= new EventHandler(setToolValues);
			((CheckBox)ControlDictionary["useOutputPadCheckBox"]).CheckedChanged -= new EventHandler(setToolValues);
			
			if (((ListBox)ControlDictionary["toolListBox"]).SelectedItems.Count == 1) {
				ExternalTool selectedItem = ((ListBox)ControlDictionary["toolListBox"]).SelectedItem as ExternalTool;
				SetEnabledStatus(true, dependendControlNames);
				ControlDictionary["titleTextBox"].Text      = selectedItem.MenuCommand;
				ControlDictionary["commandTextBox"].Text    = selectedItem.Command;
				ControlDictionary["argumentTextBox"].Text   = selectedItem.Arguments;
				ControlDictionary["workingDirTextBox"].Text = selectedItem.InitialDirectory;
				((CheckBox)ControlDictionary["promptArgsCheckBox"]).Checked   = selectedItem.PromptForArguments;
				((CheckBox)ControlDictionary["useOutputPadCheckBox"]).Checked = selectedItem.UseOutputPad;
			} else {
				SetEnabledStatus(false, dependendControlNames);
				
				ControlDictionary["titleTextBox"].Text      = String.Empty;
				ControlDictionary["commandTextBox"].Text    = String.Empty;
				ControlDictionary["argumentTextBox"].Text   = String.Empty;
				ControlDictionary["workingDirTextBox"].Text = String.Empty;
				((CheckBox)ControlDictionary["promptArgsCheckBox"]).Checked   = false;
				((CheckBox)ControlDictionary["useOutputPadCheckBox"]).Checked = false;
			}
			
			ControlDictionary["titleTextBox"].TextChanged      += new EventHandler(setToolValues);
			ControlDictionary["commandTextBox"].TextChanged    += new EventHandler(setToolValues);
			ControlDictionary["argumentTextBox"].TextChanged   += new EventHandler(setToolValues);
			ControlDictionary["workingDirTextBox"].TextChanged += new EventHandler(setToolValues);
			((CheckBox)ControlDictionary["promptArgsCheckBox"]).CheckedChanged   += new EventHandler(setToolValues);
			((CheckBox)ControlDictionary["useOutputPadCheckBox"]).CheckedChanged += new EventHandler(setToolValues);
		}
		
		void removeEvent(object sender, EventArgs e)
		{
			((ListBox)ControlDictionary["toolListBox"]).BeginUpdate();
			try {
				int index = ((ListBox)ControlDictionary["toolListBox"]).SelectedIndex;
				object[] selectedItems = new object[((ListBox)ControlDictionary["toolListBox"]).SelectedItems.Count];
				((ListBox)ControlDictionary["toolListBox"]).SelectedItems.CopyTo(selectedItems, 0);
				((ListBox)ControlDictionary["toolListBox"]).SelectedIndexChanged -= new EventHandler(selectEvent);
				foreach (object item in selectedItems) {
					((ListBox)ControlDictionary["toolListBox"]).Items.Remove(item);
				}
				((ListBox)ControlDictionary["toolListBox"]).SelectedIndexChanged += new EventHandler(selectEvent);
				if (((ListBox)ControlDictionary["toolListBox"]).Items.Count == 0) {
					selectEvent(this, EventArgs.Empty);
				} else {
					((ListBox)ControlDictionary["toolListBox"]).SelectedIndex = Math.Min(index,((ListBox)ControlDictionary["toolListBox"]).Items.Count - 1);
				}
			} finally {
				((ListBox)ControlDictionary["toolListBox"]).EndUpdate();
			}
		}
		
		void addEvent(object sender, EventArgs e)
		{
			((ListBox)ControlDictionary["toolListBox"]).BeginUpdate();
			try {
				((ListBox)ControlDictionary["toolListBox"]).Items.Add(new ExternalTool());
				((ListBox)ControlDictionary["toolListBox"]).SelectedIndexChanged -= new EventHandler(selectEvent);
				((ListBox)ControlDictionary["toolListBox"]).ClearSelected();
				((ListBox)ControlDictionary["toolListBox"]).SelectedIndexChanged += new EventHandler(selectEvent);
				((ListBox)ControlDictionary["toolListBox"]).SelectedIndex = ((ListBox)ControlDictionary["toolListBox"]).Items.Count - 1;
			} finally {
				((ListBox)ControlDictionary["toolListBox"]).EndUpdate();
			}
		}
	}
}
