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
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;

using ICSharpCode.Core.Services;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels 
{
	public class CodeTemplatePane : AbstractOptionPanel
	{
		ArrayList templateGroups;
		int       currentSelectedGroup = -1;
		
		public CodeTemplateGroup CurrentTemplateGroup {
			get {
				if (currentSelectedGroup < 0 || currentSelectedGroup >= templateGroups.Count) {
					return null;
				}
				return (CodeTemplateGroup)templateGroups[currentSelectedGroup];
			}
		}
		
		public override void LoadPanelContents()
		{
			templateGroups = CodeTemplateLoader.TemplateGroups;
			
			SetupFromXml(Path.Combine(PropertyService.DataDirectory,
			                          @"resources\panels\CodeTemplatePanel.xfrm"));
			
			ControlDictionary["removeButton"].Click += new System.EventHandler(RemoveEvent);
			ControlDictionary["addButton"].Click    += new System.EventHandler(AddEvent);
			ControlDictionary["editButton"].Click   += new System.EventHandler(EditEvent);
			
			ControlDictionary["addGroupButton"].Click    += new System.EventHandler(AddGroupEvent);
			ControlDictionary["removeGroupButton"].Click += new System.EventHandler(RemoveGroupEvent);
			
			
			((RichTextBox)ControlDictionary["templateRichTextBox"]).Font = new System.Drawing.Font("Courier New", 10f);
			((RichTextBox)ControlDictionary["templateRichTextBox"]).TextChanged += new EventHandler(TextChange);
			
			((ListView)ControlDictionary["templateListView"]).Activation = ItemActivation.Standard;
			((ListView)ControlDictionary["templateListView"]).ItemActivate         += new System.EventHandler(EditEvent);
			((ListView)ControlDictionary["templateListView"]).SelectedIndexChanged += new System.EventHandler(IndexChange);
			
			((ComboBox)ControlDictionary["groupComboBox"]).DropDown += new EventHandler(FillGroupBoxEvent);
			
			if (templateGroups.Count > 0) {
				currentSelectedGroup = 0;
			}
			
			FillGroupComboBox();
			BuildListView();
			IndexChange(null, null);
			SetEnabledStatus();
		}
		
		public override bool StorePanelContents()
		{
			CodeTemplateLoader.TemplateGroups = templateGroups;
			CodeTemplateLoader.SaveTemplates();
			return true;
		}
		
		void FillGroupBoxEvent(object sender, EventArgs e)
		{
			FillGroupComboBox();
		}
		
		void SetEnabledStatus()
		{
			bool groupSelected = CurrentTemplateGroup != null;
			bool groupsEmpty   = templateGroups.Count != 0;
			
			SetEnabledStatus(groupSelected, "addButton", "editButton", "removeButton", "templateListView", "templateRichTextBox");
			SetEnabledStatus(groupsEmpty, "groupComboBox", "extensionLabel");
			if (groupSelected) {
				bool oneItemSelected = ((ListView)ControlDictionary["templateListView"]).SelectedItems.Count == 1;
				bool isItemSelected  = ((ListView)ControlDictionary["templateListView"]).SelectedItems.Count > 0;
				SetEnabledStatus(oneItemSelected, "editButton", "templateRichTextBox");
				SetEnabledStatus(isItemSelected, "removeButton");
			}
		}
		
#region GroupComboBox event handler
		void SetGroupSelection(object sender, EventArgs e)
		{
			currentSelectedGroup = ((ComboBox)ControlDictionary["groupComboBox"]).SelectedIndex;
			BuildListView();
		}
		
		void GroupComboBoxTextChanged(object sender, EventArgs e)
		{
			if (((ComboBox)ControlDictionary["groupComboBox"]).SelectedIndex >= 0) {
				currentSelectedGroup = ((ComboBox)ControlDictionary["groupComboBox"]).SelectedIndex;
			}
			if (CurrentTemplateGroup != null) {
				CurrentTemplateGroup.ExtensionStrings = ((ComboBox)ControlDictionary["groupComboBox"]).Text.Split(';');
			}
		}
#endregion
		
#region Group Button events
		void AddGroupEvent(object sender, EventArgs e)
		{
			templateGroups.Add(new CodeTemplateGroup(".???"));
			FillGroupComboBox();
			((ComboBox)ControlDictionary["groupComboBox"]).SelectedIndex = templateGroups.Count - 1;
			SetEnabledStatus();
		}
		
		void RemoveGroupEvent(object sender, EventArgs e)
		{
			if (CurrentTemplateGroup != null) {
				templateGroups.RemoveAt(currentSelectedGroup);
				if (templateGroups.Count == 0) {
					currentSelectedGroup = -1;
				} else {
					((ComboBox)ControlDictionary["groupComboBox"]).SelectedIndex = Math.Min(currentSelectedGroup, templateGroups.Count - 1);
				}
				FillGroupComboBox();
				BuildListView();
				SetEnabledStatus();
			}
		}
#endregion
		
#region Template Button events
		void RemoveEvent(object sender, System.EventArgs e)
		{
			object[] selectedItems = new object[((ListView)ControlDictionary["templateListView"]).SelectedItems.Count];
			((ListView)ControlDictionary["templateListView"]).SelectedItems.CopyTo(selectedItems, 0);
			
			foreach (ListViewItem item in selectedItems) {
				((ListView)ControlDictionary["templateListView"]).Items.Remove(item);
			}
			StoreTemplateGroup();
		}
		
		void AddEvent(object sender, System.EventArgs e)
		{
			CodeTemplate newTemplate = new CodeTemplate();
			using (EditTemplateDialog etd = new EditTemplateDialog(newTemplate)) {
				if (etd.ShowDialog() == DialogResult.OK) {
					CurrentTemplateGroup.Templates.Add(newTemplate);
					((ListView)ControlDictionary["templateListView"]).SelectedItems.Clear();
					BuildListView();
					((ListView)ControlDictionary["templateListView"]).Select();
				}
			}
		}
		
		void EditEvent(object sender, System.EventArgs e)
		{
			int i = GetCurrentIndex();
			if (i != -1) {
				ListViewItem item = ((ListView)ControlDictionary["templateListView"]).SelectedItems[0];
				CodeTemplate template = (CodeTemplate)item.Tag;
				template = new CodeTemplate(template.Shortcut, template.Description, template.Text);
				
				using (EditTemplateDialog etd = new EditTemplateDialog(template)) {
					if (etd.ShowDialog() == DialogResult.OK) {
						item.Tag = template;
						StoreTemplateGroup();
					}
				}
				
				BuildListView();
			}
		}
#endregion
		
		void FillGroupComboBox()
		{
			((ComboBox)ControlDictionary["groupComboBox"]).TextChanged          -= new EventHandler(GroupComboBoxTextChanged);
			((ComboBox)ControlDictionary["groupComboBox"]).SelectedIndexChanged -= new EventHandler(SetGroupSelection);
			
			((ComboBox)ControlDictionary["groupComboBox"]).Items.Clear();
			foreach (CodeTemplateGroup templateGroup in templateGroups) {
				((ComboBox)ControlDictionary["groupComboBox"]).Items.Add(String.Join(";", templateGroup.ExtensionStrings));
			}
			((ComboBox)ControlDictionary["groupComboBox"]).Text = CurrentTemplateGroup != null ? ((ComboBox)ControlDictionary["groupComboBox"]).Items[currentSelectedGroup].ToString() : String.Empty;
			if (currentSelectedGroup >= 0) {
				((ComboBox)ControlDictionary["groupComboBox"]).SelectedIndex = currentSelectedGroup;
			}
			
			((ComboBox)ControlDictionary["groupComboBox"]).SelectedIndexChanged += new EventHandler(SetGroupSelection);
			((ComboBox)ControlDictionary["groupComboBox"]).TextChanged          += new EventHandler(GroupComboBoxTextChanged);
		}
		
		int GetCurrentIndex()
		{
			if (((ListView)ControlDictionary["templateListView"]).SelectedItems.Count == 1) {
				return ((ListView)ControlDictionary["templateListView"]).SelectedItems[0].Index;
			}
			return -1;
		}
		
		void IndexChange(object sender, System.EventArgs e)
		{
			int i = GetCurrentIndex();
			
			if (i != -1) {
				ControlDictionary["templateRichTextBox"].Text    = ((CodeTemplate)((ListView)ControlDictionary["templateListView"]).SelectedItems[0].Tag).Text;
			} else {
				ControlDictionary["templateRichTextBox"].Text    = String.Empty;
			}
			SetEnabledStatus();
		}
		
		void TextChange(object sender, EventArgs e)
		{
			int i = GetCurrentIndex();
			if (i != -1) {
				((CodeTemplate)((ListView)ControlDictionary["templateListView"]).SelectedItems[0].Tag).Text = ControlDictionary["templateRichTextBox"].Text;
			}
		}
		
		void StoreTemplateGroup()
		{
			if (CurrentTemplateGroup != null) {
				CurrentTemplateGroup.Templates.Clear();
				foreach (ListViewItem item in ((ListView)ControlDictionary["templateListView"]).Items) {
					CurrentTemplateGroup.Templates.Add(item.Tag);
				}
			}
		}
		
		void BuildListView()
		{
			((ListView)ControlDictionary["templateListView"]).Items.Clear();
			if (CurrentTemplateGroup != null) {
				foreach (CodeTemplate template in CurrentTemplateGroup.Templates) {
					ListViewItem newItem = new ListViewItem(new string[] { template.Shortcut, template.Description });
					newItem.Tag = template;
					((ListView)ControlDictionary["templateListView"]).Items.Add(newItem);
				}
			}
			IndexChange(this, EventArgs.Empty);
		}
	}
}
